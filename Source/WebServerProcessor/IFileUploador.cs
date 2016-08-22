/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project -- ASP.NET Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace WebServerProcessor
{
	public interface IFileUploador
	{
		void SetPostedFile(HttpPostedFile file);
		void SetAspxPage(System.Web.UI.Page page);
	}
}
