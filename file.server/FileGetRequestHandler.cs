// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using dotnet.lib.Http;
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.CoreAnnex.exception;
using System.Security.Permissions;
using System.Security;
using dotnet.lib.Http.server;
using dotnet.lib.Http.headers;

namespace dotnet.lib.Http.file.server
{
    public class FileGetRequestHandler : RequestHandler
    {
        private static Log log = Log.getLog(typeof(FileGetRequestHandler));

        ////////////////////////////////////////////////////////////////////////////
        private String _rootFolder;

        ////////////////////////////////////////////////////////////////////////////
        private FileInfo _docRootFile;


		////////////////////////////////////////////////////////////////////////////
		private CacheControl _cacheControl = null;

		public CacheControl CacheControl
		{
			get
			{
				return _cacheControl;
			}

			set
			{
				_cacheControl = value;
			}
		}

		////////////////////////////////////////////////////////////////////////////

		public FileGetRequestHandler(String rootFolder)
        {
            _rootFolder = rootFolder;
            _docRootFile = new FileInfo(rootFolder);

            if (!_docRootFile.Exists)
            {
                log.warnFormat("!_docRootFile.Exists; _docRootFile.FullName = '{0}'", _docRootFile.FullName);
            }
        }



        private static void validateRequestUri(String requestUri)
        {
            log.debug(requestUri, "requestUri");

            if ('/' != requestUri[0])
            {

                log.errorFormat("'/' != requestUri.charAt(0); requestUri = '{0}'", requestUri);
                throw HttpErrorHelper.forbidden403FromOriginator(typeof(FileGetRequestHandler));
            }

            if (-1 != requestUri.IndexOf("/."))
            { // UNIX hidden files

                log.errorFormat("-1 != requestUri.indexOf( \"/.\"); requestUri = '{0}'", requestUri);
                throw HttpErrorHelper.forbidden403FromOriginator(typeof(FileGetRequestHandler));

            }

            if (-1 != requestUri.IndexOf(".."))
            { // parent directory

                log.errorFormat("-1 != requestUri.indexOf( \"..\"); requestUri = '{0}'", requestUri);
                throw HttpErrorHelper.forbidden403FromOriginator(typeof(FileGetRequestHandler));

            }
		
        }


	    private static void validateMimeTypeForRequestUri( String requestUri ) {
		
		    if( null == MimeTypes.getMimeTypeForPath( requestUri ) ) {
			
			    log.errorFormat( "null == getMimeTypeForRequestUri( requestUri ); requestUri = '{0}'", requestUri ); 
			    throw HttpErrorHelper.forbidden403FromOriginator( typeof(FileGetRequestHandler) );
            }
			
		}

		public String getETag(FileInfo fileInfo)
		{
			return String.Format("\"{0}\"", fileInfo.LastWriteTimeUtc.Ticks );
		}

		private FileInfo toAbsoluteFileInfo(String relativePath)
		{
			// Replace the forward slashes with back-slashes to make a file name
			// string filename = relativePath.Replace('/', '\\');
			string filename = relativePath;
			// log.debug (filename, "filename");

			FileInfo answer = new FileInfo(_rootFolder + filename);

			return answer;

		}

        public Entity readFile(FileInfo fileInfo)
        {

            // Make sure they aren't trying in funny business by checking that the
            // resulting canonical name of the file has the doc root as a subset.
            var filePath = fileInfo.FullName;
            if (!filePath.StartsWith(_docRootFile.FullName))
            {
				log.errorFormat("!filePath.StartsWith(_docRootFile.FullName); filePath = {0}; _docRootFile.FullName = {1}", filePath, _docRootFile.FullName);
                throw HttpErrorHelper.forbidden403FromOriginator(this);

            }

            if (!fileInfo.Exists)
            {
                log.errorFormat("!fileInfo.Exists; file.FullName = {0}", fileInfo.FullName);
                throw HttpErrorHelper.notFound404FromOriginator(this);
            }


            // can read ? 
            { 

                FileIOPermission readPermission = new FileIOPermission(FileIOPermissionAccess.Read, fileInfo.FullName);
                if (!SecurityManager.IsGranted(readPermission))
                {
                    log.errorFormat("!SecurityManager.IsGranted(readPermission); fileInfo.FullName = {0}", fileInfo.FullName);
                    throw HttpErrorHelper.forbidden403FromOriginator(this);
                }
            }

            
            try
            {
                FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
                StreamEntity answer = new StreamEntity(fileInfo.Length, fs);
                return answer;
            }
            catch (Exception e)
            {
                log.errorFormat("exception caught trying open file; fileInfo.FullName = {0}; e.Message = {1}", fileInfo.FullName, e.Message);
                throw HttpErrorHelper.notFound404FromOriginator(this);
            }

        }

        public HttpResponse processRequest(HttpRequest request)
        {

            String requestUri = request.RequestUri;

            if (requestUri.EndsWith("/"))
            {
                requestUri = requestUri + "index.html";
            }

            { // some validation
                validateRequestUri(requestUri);
                validateMimeTypeForRequestUri(requestUri);
            }

			FileInfo absoluteFileInfo = toAbsoluteFileInfo(requestUri);


			String eTag = getETag(absoluteFileInfo);

			HttpResponse answer;

			String ifNoneMatch = request.getHttpHeader("if-none-match");
			if (null != ifNoneMatch && ifNoneMatch.Equals(eTag))
			{
				answer = new HttpResponse(HttpStatus.NOT_MODIFIED_304);
			}
			else {

				Entity body = readFile(absoluteFileInfo);
				answer = new HttpResponse(HttpStatus.OK_200, body);
				String contentType = MimeTypes.getMimeTypeForPath(requestUri);
				answer.setContentType(contentType);

			}

			if (null != _cacheControl)
			{
				answer.putHeader(_cacheControl.getName(), _cacheControl.getValue());
			}
				

			{
				answer.putHeader("Date", DateTime.UtcNow.ToString("R"));
			}

			{
				var lastModified = absoluteFileInfo.LastWriteTimeUtc;
				answer.putHeader("Last-Modified", lastModified.ToString("R"));
			}


			answer.putHeader("ETag", eTag);
			return answer;

        }

        public String getProcessorUri()
        {
            return "/";
        }


    }
}
