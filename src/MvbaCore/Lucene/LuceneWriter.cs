using System;
using System.IO;
using System.Linq;

using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;

using Directory = System.IO.Directory;
using Version = Lucene.Net.Util.Version;

namespace MvbaCore.Lucene
{
	public interface ILuceneWriter
	{
		void AddDocument(Document document);
		void Close();
		void Commit();
		void DeleteDocuments(Term term);
	}

	public class LuceneWriter : ILuceneWriter, IDisposable
	{
		private readonly ILuceneFileSystem _luceneFileSystem;
		private IndexWriter _writer;

		public LuceneWriter(ILuceneFileSystem luceneFileSystem)
		{
			_luceneFileSystem = luceneFileSystem;
		}

		public void Dispose()
		{
			Close();
		}

		public void Close()
		{
			if (_writer != null)
			{
				_writer.Commit();
				_writer.Close();
				_writer = null;
			}
		}

		public void DeleteDocuments(Term term)
		{
			Open();
			_writer.DeleteDocuments(term);
			Commit();
		}

		public void Commit()
		{
			if (_writer == null)
			{
				return;
			}
			_writer.Commit();
		}

		public void AddDocument(Document document)
		{
			Open();
			_writer.AddDocument(document);
		}

		private void Open()
		{
			if (_writer != null)
			{
				return;
			}
			var analyzer = new StandardAnalyzer(Version.LUCENE_29);
			string luceneDirectory = _luceneFileSystem.GetLuceneDirectory();
			var fsDirectory = FSDirectory.Open(new DirectoryInfo(luceneDirectory));
			bool create = !Directory.GetFiles(luceneDirectory).Any();
			_writer = new IndexWriter(fsDirectory, analyzer, create, new IndexWriter.MaxFieldLength(1000));
		}
	}
}