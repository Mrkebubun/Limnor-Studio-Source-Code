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
	/// <summary>
	/// runtime version of LimnorWebApp
	/// </summary>
	public class WebAppAspx
	{
		static private JsonWebServerProcessor _currentPage;
		public WebAppAspx()
		{
		}
		static public void SetCurrentPage(JsonWebServerProcessor page)
		{
			_currentPage = page;
		}
		static public string GetCookie(string name)
		{
			return _currentPage.GetCookie(name);
		}
		static public void SetCookie(string name, string value)
		{
			_currentPage.SetCookie(name, value);
		}
		static public CookieCollection Cookies
		{
			get
			{
				return _currentPage.Cookies;
			}
		}
	}
}
