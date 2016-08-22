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
	public interface IWebServerCompiler
	{
		void AppendServerPagePhpCode(StringCollection code);
		void AppendServerExecutePhpCode(StringCollection code);
	}
	public interface IWebServerCompilerHolder
	{
		void SetWebServerCompiler(IWebServerCompiler compiler);
		void OnCreatedWebServerEventHandler(IWebServerCompiler compiler, string eventName, string handlerName);
	}
}
