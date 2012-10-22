using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Web.Hosting;

using JetBrains.Annotations;

namespace MvbaCore
{
	public interface IFileSystemService
	{
		void AppendAllText([NotNull] string filePath, [NotNull] string textToAppend);

		[NotNull]
		DirectoryInfo CreateDirectory([NotNull] string directoryPath);

		[NotNull]
		StreamWriter CreateFile([NotNull] string filePath);

		[NotNull]
		TextWriter CreateTextFile([NotNull] string filePath);

		void DeleteDirectoryContents([NotNull] string dirPath);

		void DeleteDirectoryRecursive([NotNull] string dirPath);
		bool DeleteFile([NotNull] string filePath);
		bool DirectoryExists([NotNull] string directoryPath);
		bool FileExists([NotNull] string filePath);

		[NotNull]
		string GetCurrentWebApplicationPath();

		[NotNull]
		DirectoryInfo GetDirectoryInfo([NotNull] string directoryPath);

		string[] GetFiles(string filePath, string searchPattern);

		[NotNull]
		IEnumerable<string> GetNamesOfFilesInDirectory([NotNull] string directoryPath);

		void MoveFile([NotNull] string oldFilePath, [NotNull] string newfilePath);

		[NotNull]
		string[] ReadAllLines([NotNull] string filePath);

		[NotNull]
		string ReadAllText([NotNull] string filePath);

		void Writefile(string filePath, Stream data);

		void Writefile(string filePath, string data);
	}

	public class FileSystemService : IFileSystemService
	{
		public string GetCurrentWebApplicationPath()
		{
			string env = HostingEnvironment.ApplicationPhysicalPath;
			return env;
		}

		public DirectoryInfo GetDirectoryInfo(string directoryPath)
		{
			return new DirectoryInfo(directoryPath);
		}

		public IEnumerable<string> GetNamesOfFilesInDirectory(string directoryPath)
		{
			return Directory.GetFiles(directoryPath);
		}

		public bool DirectoryExists(string directoryPath)
		{
			return Directory.Exists(directoryPath);
		}

		public DirectoryInfo CreateDirectory(string directoryPath)
		{
			if (!Directory.Exists(directoryPath))
			{
				return Directory.CreateDirectory(directoryPath);
			}
			return new DirectoryInfo(directoryPath);
		}

		public TextWriter CreateTextFile(string filePath)
		{
			return File.CreateText(filePath);
		}

		public bool FileExists(string filePath)
		{
			return File.Exists(filePath);
		}

		public void Writefile(string filePath, Stream data)
		{
			File.WriteAllBytes(filePath, data.ReadAllBytes());
		}

		public void Writefile(string filePath, string data)
		{
			File.WriteAllText(filePath, data);
		}

		public StreamWriter CreateFile(string filePath)
		{
			try
			{
				return new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write));
			}
			catch (DirectoryNotFoundException exception)
			{
				throw new EnvironmentException("Output Directory Not Found", exception);
			}
			catch (SecurityException exception)
			{
				throw new EnvironmentException("Unable to Open Or Create output file", exception);
			}
			catch (UnauthorizedAccessException exception)
			{
				throw new EnvironmentException("Unauthorized to Open or Create output file", exception);
			}
			catch (PathTooLongException exception)
			{
				throw new EnvironmentException("Output Directory is > 248 characters or output file is > 260 characters", exception);
			}
		}

		public void MoveFile(string oldFilePath, string newFilePath)
		{
			try
			{
				File.Move(oldFilePath, newFilePath);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("unable to move file " + oldFilePath + " to " + newFilePath, ex);
			}
		}

		public void AppendAllText(string filePath, string textToAppend)
		{
			File.AppendAllText(filePath, textToAppend);
		}

		public string[] ReadAllLines(string filePath)
		{
			return File.ReadAllLines(filePath);
		}

		public string ReadAllText(string filePath)
		{
			return File.ReadAllText(filePath);
		}

		public bool DeleteFile(string filePath)
		{
			if (!FileExists(filePath))
			{
				return true;
			}
			try
			{
				File.Delete(filePath);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void DeleteDirectoryContents(string dirPath)
		{
			if (!Directory.Exists(dirPath))
			{
				return;
			}
			foreach (string directory in Directory.GetDirectories(dirPath))
			{
				DeleteDirectoryRecursive(directory);
			}
			foreach (string file in Directory.GetFiles(dirPath))
			{
				File.Delete(file);
			}
		}

		public void DeleteDirectoryRecursive(string dirPath)
		{
			if (!Directory.Exists(dirPath) || dirPath.Split(Path.DirectorySeparatorChar).Last() == ".svn")
			{
				return;
			}

			DeleteDirectoryContents(dirPath);

			Directory.Delete(dirPath);
		}

		public string[] GetFiles(string filePath, string searchPattern)
		{
			return Directory.GetFiles(filePath, searchPattern);
		}
	}
}