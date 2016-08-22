/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project -- ASP.NET Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace WebServerProcessor
{
	public class CookieCollection
	{
		private JsonWebServerProcessor _page;
		public CookieCollection(JsonWebServerProcessor page)
		{
			_page = page;
		}
		public string this[string name]
		{
			get
			{
				return _page.GetCookie(name);
			}
			set
			{
				_page.SetCookie(name, value);
			}
		}
	}
}
