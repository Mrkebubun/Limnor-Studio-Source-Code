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
using System.CodeDom;
using System.Web.UI;

namespace Limnor.WebServerBuilder
{
	public interface IPhpCodeType
	{
		void CreateActionPhpScript(string objectName, string methodName, StringCollection code, StringCollection parameters, string returnReceiver);
	}
	public interface IWebServerProgrammingSupport : IPhpCodeType
	{
		bool IsWebServerProgrammingSupported(EnumWebServerProcessor webServerProcessor);
		Dictionary<string, string> GetPhpFilenames();
		/// <summary>
		/// if it is true then the component instance declaration will be removed from the page
		/// </summary>
		/// <returns></returns>
		bool DoNotCreate();
		void OnRequestStart(Page webPage);
		void CreateOnRequestClientDataPhp(StringCollection code);
		void CreateOnRequestStartPhp(StringCollection code);
		void CreateOnRequestFinishPhp(StringCollection code);
		bool ExcludePropertyForPhp(string name);
		/// <summary>
		/// true:component name needs to be passed into PHP constructor 
		/// </summary>
		bool NeedObjectName { get; }
	}
	public interface IWebServerInternalPhp
	{
		void CreateOnRequestExecutePhp(StringCollection code);
	}
	public interface IAspxPageUser
	{
		void SetAspxPage(System.Web.UI.Page page);
	}
}
