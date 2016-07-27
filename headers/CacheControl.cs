using System;

namespace dotnet.lib.Http.headers
{
	public class CacheControl : HttpHeader
	{

		String _value;

		public CacheControl( string value )
		{
			_value = value;
		}

		// max-age=86400 ... 1 day
		public CacheControl(int maxAge)
		{
			_value = "max-age=" + maxAge;
		}

		public String getName()
		{
			return "Cache-Control";
		}

		public String getValue()
		{
			return _value;
		}

	}
}

