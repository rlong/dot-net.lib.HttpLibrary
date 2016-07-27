using System;
using dotnet.lib.Http.headers;

namespace dotnet.lib.HttpLibrary
{
	public class LastModified : HttpHeader
	{
		public LastModified()
		{
		}


		public String getName()
		{
			return "Last-Modified";
		}

		public String getValue()
		{
			//var x = new DateTime();
			//x.ToString()
			return "";
		}

	}
}

