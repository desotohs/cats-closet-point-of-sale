using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CatsCloset.Imports {
	public class CsvFile : IDisposable {
		private StreamReader Reader;
		private Dictionary<string, int> Columns;
		private string[] Row;
		private bool Disposed;

		public bool EndOfStream {
			get {
				if (Disposed) {
					throw new ObjectDisposedException("CsvFile");
				}
				return Reader.EndOfStream;
			}
		}

		public string this[string key] {
			get {
				if (Disposed) {
					throw new ObjectDisposedException("CsvFile");
				}
				return Row[Columns[key]];
			}
		}

		public string this[string key, string def] {
			get {
				if (Disposed) {
					throw new ObjectDisposedException("CsvFile");
				}
				if (Columns.ContainsKey(key)) {
					return Row[Columns[key]];
				} else {
					return def;
				}
			}
		}

		public double this[string key, double def] {
			get {
				if (Disposed) {
					throw new ObjectDisposedException("CsvFile");
				}
				if (Columns.ContainsKey(key)) {
					return double.Parse(Row[Columns[key]]);
				} else {
					return def;
				}
			}
		}

		public bool NextLine() {
			if (Disposed) {
				throw new ObjectDisposedException("CsvFile");
			}
			if (EndOfStream) {
				return false;
			} else {
				Row = Reader.ReadLine().Split(',');
				return true;
			}
		}

		public void Dispose() {
			if (!Disposed) {
				Dispose(true);
			}
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				Reader.Close();
				Disposed = true;
			}
		}

		public CsvFile(Stream stream) {
			Reader = new StreamReader(stream);
			Columns = new Dictionary<string, int>();
			string[] columns = Reader.ReadLine().Split(',');
			for (int i = 0; i < columns.Length; ++i) {
				Columns[columns[i]] = i;
			}
			Disposed = false;
		}

		~CsvFile() {
			Dispose(false);
		}
	}
}

