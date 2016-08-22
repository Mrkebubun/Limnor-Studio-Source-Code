/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Xml;
using Limnor.WebBuilder;

namespace VPL
{
	public interface IWebPage
	{
		IComponent AddComponent(Type t);
		bool EnableBrowserPageCache { get; }
		void AddWebControl(IDesignerLoaderHost host);
		bool UpdateHtmlFile();
		void SaveHtmlFile();
		void SetClosing();
		void OnClosing();
		bool HtmlChanged { get; }
		Dictionary<string, string> HtmlParts { get; }
		XmlNode SctiptNode { get; }
		string CloseDialogPrompt { get; }
		string CancelDialogPrompt { get; }
		UInt32 LoginPageId { get; }
		string LoginPageName { get; }
		string LoginPageFileName { get; }
		int UserLevel { get; }
		void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positionType, int groupId);
		string MapEventOwnerName(string eventName);
	}
	public interface IWebDataRepeater
	{
		string GetElementGetter();
		string CodeName { get; }
	}
}
