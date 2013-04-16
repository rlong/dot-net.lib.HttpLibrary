// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using jsonbroker.library.common.http;
using jsonbroker.library.common.log;
using jsonbroker.library.common.exception;
using System.Security.Permissions;
using System.Security;

namespace jsonbroker.library.server.http.reqest_handler
{
    public class FileRequestHandler : RequestHandler
    {
        private static Log log = Log.getLog(typeof(FileRequestHandler));

        ////////////////////////////////////////////////////////////////////////////
        private String _rootFolder;

        ////////////////////////////////////////////////////////////////////////////
        private FileInfo _docRootFile;


        public FileRequestHandler(String rootFolder)
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
                throw HttpErrorHelper.forbidden403FromOriginator(typeof(FileRequestHandler));
            }

            if (-1 != requestUri.IndexOf("/."))
            { // UNIX hidden files

                log.errorFormat("-1 != requestUri.indexOf( \"/.\"); requestUri = '{0}'", requestUri);
                throw HttpErrorHelper.forbidden403FromOriginator(typeof(FileRequestHandler));

            }

            if (-1 != requestUri.IndexOf(".."))
            { // parent directory

                log.errorFormat("-1 != requestUri.indexOf( \"..\"); requestUri = '{0}'", requestUri);
                throw HttpErrorHelper.forbidden403FromOriginator(typeof(FileRequestHandler));

            }
		
        }


	    private static void validateMimeTypeForRequestUri( String requestUri ) {
		
		    if( null == MimeTypes.getMimeTypeForPath( requestUri ) ) {
			
			    log.errorFormat( "null == getMimeTypeForRequestUri( requestUri ); requestUri = '{0}'", requestUri ); 
			    throw HttpErrorHelper.forbidden403FromOriginator( typeof(FileRequestHandler) );
            }
			
		}


        public Entity readFile(String relativePath)
        {

            // Replace the forward slashes with back-slashes to make a file name
            string filename = relativePath.Replace('/', '\\');

            FileInfo file = new FileInfo(_rootFolder + filename);


            // Make sure they aren't trying in funny business by checking that the
            // resulting canonical name of the file has the doc root as a subset.
            filename = file.FullName;
            if (!filename.StartsWith(_docRootFile.FullName))
            {
                log.errorFormat("!filename.StartsWith(_docRootFile.FullName); filename = {0}; _docRootFile.FullName = {1}", filename, _docRootFile.FullName);
                throw HttpErrorHelper.forbidden403FromOriginator(this);

            }

            if (!file.Exists)
            {
                log.errorFormat("!file.Exists; file.FullName = {0}", file.FullName);
                throw HttpErrorHelper.notFound404FromOriginator(this);
            }


            // can read ? 
            { 

                FileIOPermission readPermission = new FileIOPermission(FileIOPermissionAccess.Read, file.FullName);
                if (!SecurityManager.IsGranted(readPermission))
                {
                    log.errorFormat("!SecurityManager.IsGranted(readPermission); file.FullName = {0}", file.FullName);
                    throw HttpErrorHelper.forbidden403FromOriginator(this);
                }
            }

            
            try
            {
                FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                StreamEntity answer = new StreamEntity(file.Length, fs);
                return answer;
            }
            catch (Exception e)
            {
                log.errorFormat("exception caught trying open file; file.FullName = {0}; e.Message = {1}", file.FullName, e.Message);
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

            try
            {
                Entity body = readFile(requestUri);
                HttpResponse answer = new HttpResponse(HttpStatus.OK_200, body);
                String contentType = MimeTypes.getMimeTypeForPath(requestUri);
                answer.setContentType(contentType);
                return answer;
            }
            catch (BaseException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                log.errorFormat("exception caught trying process request; e.Message = {0}", e.Message);
                throw HttpErrorHelper.notFound404FromOriginator(this);
            }
        }

        public String getProcessorUri()
        {
            return "/";
        }


    }
}
