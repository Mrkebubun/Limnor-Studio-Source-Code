/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support - Web Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Limnor.WebServerBuilder
{
	public interface IWebServerComponentCreator : IWebServerComponentCreatorBase
	{
		void CreateServerComponentPhp(StringCollection objectDecl, StringCollection initCode, ServerScriptHolder scripts);
	}
	public interface IWebServerComponentCreatorBase
	{
		bool RemoveFromComponentInitializer(string propertyName);
	}
}
