// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace jsonbroker.library.common.http
{
    public class HttpStatus
    {


        public class ErrorDomain
        {
            public static readonly String BAD_REQUEST_400 = "jsonbroker.HttpStatus.BAD_REQUEST_400";
            public static readonly String UNAUTHORIZED_401 = "jsonbroker.HttpStatus.UNAUTHORIZED_401";
        }

        public static readonly int OK_200 = 200;
	    public static readonly int NO_CONTENT_204 = 204;
	    public static readonly int PARTIAL_CONTENT_206 = 206;
	

	    public static readonly int BAD_REQUEST_400 = 400;
	    public static readonly int UNAUTHORIZED_401 = 401;
	    public static readonly int FORBIDDEN_403 = 403;
	    public static readonly int NOT_FOUND_404 = 404;
	    public static readonly int REQUEST_ENTITY_TOO_LARGE_413 = 413;
	
	    public static readonly int INTERNAL_SERVER_ERROR_500 = 500;
        public static readonly int NOT_IMPLEMENTED_501 = 501;


        public static String getReason(int statusCode)
        {

            switch (statusCode)
            {
                case 200:
                    return "OK";
                case 204:
                    return "No Content";
                case 206:
                    return "Partial Content";

                case 400:
                    return "Bad Request";
                case 401:
                    return "Unauthorized";
                case 402:
                    return "Payment Required";
                case 403:
                    return "Forbidden";
                case 404:
                    return "Not Found";
                case 405:
                    return "Method Not Allowed";
                case 406:
                    return "Not Acceptable";
                case 407:
                    return "Proxy Authentication Required";
                case 408:
                    return "Request Time-out";
                case 409:
                    return "Conflict";
                case 410:
                    return "Gone";
                case 411:
                    return "Length Required";
                case 412:
                    return "Precondition Failed";
                case 413:
                    return "Request Entity Too Large";
                case 414:
                    return "Request-URI Too Large";
                case 415:
                    return "Unsupported Media Type";
                case 416:
                    return "Requested range not satisfiable";
                case 417:
                    return "Expectation Failed";

                case 500:
                    return "Internal Server Error";
                case 501:
                    return "Not Implemented";
                case 502:
                    return "Bad Gateway";
                case 503:
                    return "Service Unavailable";
                case 504:
                    return "Gateway Time-out";
                case 505:
                    return "HTTP Version not supported";
                default:
                    return "Unknown";
            }
        }

    }
}
