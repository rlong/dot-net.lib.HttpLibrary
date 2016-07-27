
using System;
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.Http.multi_part;


namespace dotnet.lib.Http.file.server
{
	public class FileSaveMultiPartHandler : MultiPartHandler
	{

		private static Log log = Log.getLog(typeof(FileSaveMultiPartHandler));

		private string _folderPath;


		public FileSaveMultiPartHandler ( string folderPath ) {
			_folderPath = folderPath;
		}

		public PartHandler FoundPartDelimiter() {
			return new FileSavePartHandler ( _folderPath );
		}

		public void HandleException(Exception e) {
			log.error (e);
		}

		public void FoundCloseDelimiter() {
			log.enteredMethod ();
		}

	}
}

