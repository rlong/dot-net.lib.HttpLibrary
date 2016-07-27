// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet.lib.Http.server
{
    public class MimeTypes
    {

        private static Dictionary<String, String> MIME_TYPES;

        public static String APPLICATION_JSON = "application/json";

        static MimeTypes()
        {

            MIME_TYPES = new Dictionary<String, String>();

            MIME_TYPES[".css"] = "text/css";
            MIME_TYPES[".eot"] = "application/vnd.ms-fontobject"; // http://symbolset.com/blog/properly-serve-webfonts/
            MIME_TYPES[".html"] = "text/html";
            MIME_TYPES[".gif"] = "image/gif";
            MIME_TYPES[".ico"] = "image/x-icon";
            MIME_TYPES[".jpeg"] = "image/jpeg";
            MIME_TYPES[".jpg"] = "image/jpeg";
            MIME_TYPES[".js"] = "application/javascript";
            MIME_TYPES[".json"] = APPLICATION_JSON;
            MIME_TYPES[".map"] = APPLICATION_JSON; // http://stackoverflow.com/questions/19911929/what-mime-type-should-i-use-for-source-map-files
            MIME_TYPES[".png"] = "image/png";
            MIME_TYPES[".svg"] = "image/svg+xml"; // http://www.ietf.org/rfc/rfc3023.txt, section 8.19
            MIME_TYPES[".ts"] = "text/x.typescript"; // http://stackoverflow.com/questions/13213787/whats-the-mime-type-of-typescript
            MIME_TYPES[".ttf"] = "application/x-font-ttf"; // http://symbolset.com/blog/properly-serve-webfonts/
            MIME_TYPES[".woff"] = "application/x-font-woff"; // http://symbolset.com/blog/properly-serve-webfonts/
        }


        public static String getMimeTypeForPath(String path)
        {
            int lastDot = path.LastIndexOf('.');

            if (-1 == lastDot)
            {
                return null;
            }

            String extension = path.Substring(lastDot);
            if (!MIME_TYPES.ContainsKey(extension))
            {
                return null;
            }

            String answer = MIME_TYPES[extension];
            return answer;
        }

    }
}
