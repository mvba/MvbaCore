﻿//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using MvbaCore.Logging;
using MvbaCore.Services;

namespace MvbaCore.FileSystem
{

	public interface IFileHandler
	{
		bool CanHandle(FileWrapper fileWrapper);
		bool Handle(FileWrapper fileWrapper);
		void Quiesce();
	}

	public class FileWrapper
	{
		public string FileName { get; set; }
		public bool Processed { get; set; }
		public DateTime FileDate { get; set; }
	}

	public class FileWatcher
	{
		private readonly string _archiveDirectory = "Archive";
		private readonly string _errorDirectory = "Errors";
		private readonly IList<IFileHandler> _fileHandlers;
		private readonly IFileSystemService _fileSystemService;
		private readonly string _resultsDirectory = "Results";
		private readonly string _sourceDir;
		private readonly Stopwatch _timer;

		private bool _directoryChanged;
		private FileSystemWatcher _fileSystemWatcher;
		private List<FileWrapper> _files;
		private volatile bool _running;
		private TimeSpan _sleepTimeout;
		private Thread _thread;

		public FileWatcher(string sourceDir,
						   IFileSystemService fileSystemService,
						   params IFileHandler[] fileHandlers)
		{
			_files = new List<FileWrapper>();
			_sourceDir = sourceDir;
			_fileSystemService = fileSystemService;
			_fileHandlers = fileHandlers;
			_sleepTimeout = new TimeSpan(0, 0, 0, 0, 100);
			_timer = new Stopwatch();
			_timer.Start();
			_directoryChanged = true;

			_errorDirectory = Path.Combine(_sourceDir, _errorDirectory);
			EnsureDirectoryExists("Error", _errorDirectory);

			_archiveDirectory = Path.Combine(_sourceDir, _archiveDirectory);
			EnsureDirectoryExists("Archive", _archiveDirectory);

			_resultsDirectory = Path.Combine(_sourceDir, _resultsDirectory);
			EnsureDirectoryExists("Results", _resultsDirectory);
		}


		private void EnsureDirectoryExists(string name, string path)
		{
			if (!_fileSystemService.DirectoryExists(path))
			{
				Logger.Log(NotificationSeverity.Info, "Creating " + name + " directory ");
				try
				{
					var directory = _fileSystemService.CreateDirectory(path);
					Logger.Log(NotificationSeverity.Info, "Created " + name + " directory " + directory.FullName);
				}
				catch (Exception e)
				{
					Logger.Log(NotificationSeverity.Error, "Unable to create " + name + " directory: " + e);
				}
			}
		}

		private void LoadFile(ICollection<FileWrapper> files, FileInfo file)
		{
			try
			{
				var fileWrapper = new FileWrapper
				{
					FileName = file.FullName,
					FileDate = file.LastWriteTime,
				};
				files.Add(fileWrapper);
			}
			catch (IOException ioException)
			{
				_directoryChanged = true;
				Logger.Log(NotificationSeverity.Warning, "=> file " + file + " is in use... ", ioException);
			}
		}

		public void LoadFiles()
		{
			if (_directoryChanged && _timer.Elapsed.TotalSeconds > 10 || !_files.Any() || _files.All(x => x.Processed))
			{
				_directoryChanged = false;
				var fileList = _fileSystemService
					.GetDirectoryInfo(_sourceDir)
					.GetFiles()
					.OrderBy(x => x.LastWriteTime);
				var currentFiles = _files.Select(x => x.FileName).ToHashSet();
				var fileWrappers = new List<FileWrapper>();
				foreach (var file in fileList.Where(x => !currentFiles.Contains(x.FullName)))
				{
					LoadFile(fileWrappers, file);
				}
				if (fileWrappers.Any())
				{
					_files = _files
						.Where(x => !x.Processed)
						.Concat(fileWrappers)
						.OrderBy(x => x.FileDate)
						.ToList();
				}
				else
				{
					_files = new List<FileWrapper>();
				}

				_timer.Restart();
			}
		}

		private void MoveFileToErrorDirectory(string file)
		{
			try
			{
// ReSharper disable AssignNullToNotNullAttribute
				_fileSystemService.MoveFile(file, Path.Combine(_errorDirectory, Path.GetFileName(file)));
// ReSharper restore AssignNullToNotNullAttribute
			}
			catch (ApplicationException aException)
			{
				var handled = false;
				if (aException.InnerException != null && aException.InnerException is IOException)
				{
					var ioException = aException.InnerException as IOException;
					if (ioException.Message.StartsWith("Cannot create a file when that file already exists"))
					{
						try
						{
							_fileSystemService.DeleteFile(file);
							handled = true;
						}
						catch (Exception exception)
						{
							Logger.Log(NotificationSeverity.Error, "=> Will not overwrite file that also exists in Error folder " + file, exception);
						}
					}
				}
				if (!handled)
				{
					Logger.Log(NotificationSeverity.Error, "=> Unable to move bad file " + file, aException);
				}
			}
			catch (Exception moveFileException)
			{
				Logger.Log(NotificationSeverity.Error, "=> Unable to move bad file " + file, moveFileException);
			}
		}

		private void OnFileCreated(object sender, FileSystemEventArgs e)
		{
			_directoryChanged = true;
		}

		public void ProcessFile(FileWrapper fileWrapper)
		{
			try
			{
				var handler = _fileHandlers.FirstOrDefault(x => x.CanHandle(fileWrapper));
				if (handler == null)
				{
					MoveFileToErrorDirectory(fileWrapper.FileName);
					Logger.Log(NotificationSeverity.Error, "=> Don't have a handler for " + fileWrapper.FileName);
					fileWrapper.Processed = true;
					return;
				}
				var deleteFile = handler.Handle(fileWrapper);
				if (deleteFile)
				{
// ReSharper disable AssignNullToNotNullAttribute
					var newFile = Path.Combine(_archiveDirectory, Path.GetFileName(fileWrapper.FileName));
// ReSharper restore AssignNullToNotNullAttribute
					if (_fileSystemService.FileExists(newFile))
					{
						_fileSystemService.DeleteFile(newFile);
					}
					_fileSystemService.MoveFile(fileWrapper.FileName, newFile);
				}
				else
				{
					MoveFileToErrorDirectory(fileWrapper.FileName);
				}
				fileWrapper.Processed = true;
			}
			catch (InvalidOperationException e)
			{
				MoveFileToErrorDirectory(fileWrapper.FileName);
				Logger.Log(NotificationSeverity.Error, "=> Error while processing " + fileWrapper.FileName, e);
				fileWrapper.Processed = true;
			}
			catch (Exception e)
			{
				if (e.GetType().Name.Contains("LazyInitializationException"))
				{
					if (!e.Message.EndsWith("Could not initialize proxy - no Session."))
					{
						MoveFileToErrorDirectory(fileWrapper.FileName);
						Logger.Log(NotificationSeverity.Error, "=> Error while processing " + fileWrapper.FileName, e);
						fileWrapper.Processed = true;
					}
				}
				else
				{
					MoveFileToErrorDirectory(fileWrapper.FileName);
					Logger.Log(NotificationSeverity.Error, "=> Error while processing " + fileWrapper.FileName, e);
					fileWrapper.Processed = true;
				}
			}
		}

		public void Start()
		{
			// try to re-run previous failures on startup
			try
			{
				var previousErrorHeaderFiles = _fileSystemService.GetFiles(_errorDirectory, "*");
				foreach (var headerFile in previousErrorHeaderFiles)
				{
// ReSharper disable AssignNullToNotNullAttribute
					_fileSystemService.MoveFile(headerFile, Path.Combine(_sourceDir, Path.GetFileName(headerFile)));
// ReSharper restore AssignNullToNotNullAttribute
				}
			}
			catch
			{
			}

			_thread = new Thread(WatchForFiles)
			{
				IsBackground = true
			};
			_running = true;
			_thread.Start();
		}

		public void Stop()
		{
			Logger.Log(NotificationSeverity.Info, "Stopping...");
			_running = false;
			try
			{
				_thread.Join(new TimeSpan(0, 0, 0, 10));
			}
			catch
			{
			}

			for (var i = 0; i < Math.Max(_sleepTimeout.TotalSeconds, 20); i++)
			{
				if (_thread.IsAlive)
				{
					Thread.Sleep(new TimeSpan(0, 0, 0, 2));
				}
			}

			try
			{
				if (_thread.IsAlive)
				{
					_thread.Abort();
				}
			}
			catch
			{
			}
			Logger.Log(NotificationSeverity.Info, "Stopped...");
		}

		private void WatchForFiles()
		{
			_fileSystemWatcher = new FileSystemWatcher(_sourceDir, "*");
			_fileSystemWatcher.Created += OnFileCreated;
			_fileSystemWatcher.EnableRaisingEvents = true;
			while (_running)
			{
				LoadFiles();

				var fileWrapper = _files.FirstOrDefault(x => !x.Processed);
				if (fileWrapper != null)
				{
					Logger.Log(NotificationSeverity.Info, "=> Processing " + Path.GetFileName(fileWrapper.FileName));

					ProcessFile(fileWrapper);

					if (!_running)
					{
						break;
					}

					if (_files.Count > 0)
					{
						continue;
					}

					foreach (var handler in _fileHandlers)
					{
						handler.Quiesce();
					}
				}

				if (_running)
				{
					Thread.Sleep(_sleepTimeout);
				}
			}
		}
	}
}