/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LimnorDesigner.Web
{
	public interface IWebHost
	{
		string HtmlUrl { get; }
		string PhysicalFolder { get; }
		bool EditorStarted { get; }
		string HtmlFile { get; }
		bool IsSaving { get; }
		bool HtmlChanged{get;}
		bool EditorReady();
		void ApplyTextBoxChanges();
		object InvoleJs(string cmd, params object[] ps);
		bool checkHtmlChange();
		void SelectHtmlElementByGuid(Guid guid);
		string GetMaps();
		string GetAreas(string mapId);
		string CreateNewMap(string guid);
		void UpdateMapAreas(string mapId, string areas);
		void SetUseMap(string imgId, string mapId);
		void SetWebProperty(string name, string value);
		string AppendArchiveFile(string name, string archiveFilePath);
		string CreateOrGetIdForCurrentElement(string newGuid);
		bool SetGuidById(string id, string guid);
		Dictionary<string, string> getIdList();
		string getIdByGuid(string guid);
		string getTagNameById(string id);
		bool Visible { get; set; }
		void StartEditor();
		void Save();
		void SetEncode(Encoding encode);
		void SetDocType(string docType);
		void LoadIntoControl();
		void CopyToProjectFolder();
		void ResetSavingFlag();
		void DoCopy();
		void DoPaste();
		void PasteToHtmlInput(string txt, int selStart, int selEnd);
	}
	public interface IDataSourceBinder
	{
		IComponent DataSource { get; }
	}
	public interface IHtmlElementCreateContents
	{
		void CreateHtmlContent(HtmlNode node);
		string id { get; }
	}
}
