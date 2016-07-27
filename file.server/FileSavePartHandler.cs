using System;
using System.IO;
using dotnet.lib.CoreAnnex.auxiliary;
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.Http.multi_part;
using dotnet.lib.Http.headers;
using dotnet.lib.Http.server;

namespace dotnet.lib.Http.file.server
{
	public class FileSavePartHandler : PartHandler
	{
		private static Log log = Log.getLog(typeof(FileSavePartHandler));

		int _bytesHandled;
		BinaryWriter _fileWriter = null;

		private string _folderPath;


		public FileSavePartHandler ( string folderPath )
		{
			_folderPath = folderPath;

		}

		public void HandleHeader(String name, String value) {

			log.debug (name, "name");
			log.debug (value, "value");

			if (name.Equals ("content-disposition")) {

				ContentDisposition contentDisposition = ContentDisposition.buildFromString (value);
				string filename = contentDisposition.getDispositionParameter ("filename", null);
				if (null != filename) {

					var filePath = _folderPath + filename;
					log.info (filePath, "filePath");

					var fileStream = new FileStream (filePath, FileMode.Create);
					_fileWriter = new BinaryWriter(fileStream);

				}

			}

		}

		public void HandleBytes(byte[] bytes, int offset, int length) {

			if (null != _fileWriter) {
				_fileWriter.Write (bytes, offset, length);
			} else {
				if (Log.isDebugEnabled()) {
					Data data = new Data (bytes, offset, length);
					log.debug (data, "data");
				}
			}

			_bytesHandled += length;
		}

		public void HandleException(Exception e) {
			
			log.enteredMethod ();

			if (null != _fileWriter) {
				
				_fileWriter.Flush();
				_fileWriter.Close();
				_fileWriter = null;

			}

		}

		public void PartCompleted() {

			if (null != _fileWriter) {
				_fileWriter.Flush();
				_fileWriter.Close();
				_fileWriter = null;
			}

			log.info (_bytesHandled, "_bytesHandled");
		}


	}

}

