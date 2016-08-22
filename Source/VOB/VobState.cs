/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;

namespace VOB
{
	public sealed class VobState
	{
		const string SECT_MainForm = "MainForm";
		const string ITEM_Width = "Width";
		const string ITEM_Height = "Height";
		const string ITEM_Left = "Left";
		const string ITEM_Top = "Top";
		const string ITEM_WindowState = "WindowState";
		const string ITEM_ClearBuildLog = "ClearBuildLog";
		//
		const string SECT_Solution = "Solution";
		const string ITEM_Item = "Item";
		const string ATT_file = "file";
		const string ATT_config = "config";
		//
		const string SECT_Help = "Help";
		const string ITEM_UserGuide = "UserGuide";
		const string ITEM_Tutorial = "Tutorial";
		const string ITEM_Reference = "Reference";
		//
		private static XmlDocument settings;
		private VobState()
		{

		}
		private static void loadSettings()
		{
			if (settings == null)
			{
				settings = new XmlDocument();
				string sCfg = System.IO.Path.Combine(VobUtil.AppDataFolder, "VobState.xml");
				if (System.IO.File.Exists(sCfg))
				{
					try
					{
						settings.Load(sCfg);
					}
					catch
					{
					}
				}
			}
		}
		public static GetPropertyGridFocus GetMainPropertyGridFocus;
		public static EventHandler SendDeleteToMainPropertyGridTextEditor;
		public static void SaveSettings()
		{
			if (settings != null)
			{
				string sCfg = System.IO.Path.Combine(VobUtil.AppDataFolder, "VobState.xml");
				settings.Save(sCfg);
			}
		}
		public static string GetSolutionConfig(string solutionFile)
		{
			XmlNode node = CreateSolutionNode(solutionFile);
			string s = GetAttribute(node, ATT_config);
			if (string.IsNullOrEmpty(s))
			{
				s = "Release";
			}
			return s;
		}
		public static void SetSolutionConfig(string solutionFile, string config)
		{
			XmlNode node = CreateSolutionNode(solutionFile);
			SetAttribute(node, ATT_config, config);
		}
		public static XmlNode CreateSolutionSection()
		{
			return CreateSection(SECT_Solution);
		}
		public static XmlNode CreateSolutionNode(string solutionFile)
		{
			XmlNode node = CreateSolutionSection();
			XmlNodeList nds = node.SelectNodes(ITEM_Item);

			foreach (XmlNode nd0 in nds)
			{
				string s = GetAttribute(nd0, ATT_file);
				if (string.Compare(solutionFile, s, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return nd0;
				}
			}
			XmlNode nd = node.OwnerDocument.CreateElement(ITEM_Item);
			node.AppendChild(nd);
			SetAttribute(nd, ATT_file, solutionFile);
			return nd;
		}
		public static XmlNode CreateSection(string sectionName)
		{
			XmlNode nd = GetSection(sectionName);
			if (nd == null)
			{
				nd = settings.CreateElement(sectionName);
				if (settings.DocumentElement == null)
				{
					XmlNode nr = settings.CreateElement("Root");
					settings.AppendChild(nr);
				}
				settings.DocumentElement.AppendChild(nd);
			}
			return nd;
		}
		public static XmlNode GetSection(string name)
		{
			loadSettings();
			XmlNodeList lst = settings.GetElementsByTagName(name);
			if (lst != null && lst.Count > 0)
				return lst[0];
			return null;
		}
		static void SetData(string sectionName, string valueName, object value)
		{
			XmlNode nd = CreateSection(sectionName);
			XmlAttribute xa = settings.CreateAttribute(valueName);
			xa.Value = value.ToString();
			nd.Attributes.Append(xa);
		}
		static string GetData(string sectionName, string valueName)
		{
			XmlNode nd = GetSection(sectionName);
			if (nd == null)
			{
				return "";
			}
			XmlAttribute xa = nd.Attributes[valueName];
			if (xa == null)
				return "";
			return xa.Value;
		}
		static int GetInt(string sectionName, string valueName)
		{
			string v = GetData(sectionName, valueName);
			if (v == null || v.Length == 0)
				return 0;
			try
			{
				return Convert.ToInt32(v);
			}
			catch
			{
			}
			return 0;
		}
		static bool GetBool(string sectionName, string valueName, bool defaultVale)
		{
			string v = GetData(sectionName, valueName);
			if (v == null || v.Length == 0)
				return defaultVale;
			try
			{
				return Convert.ToBoolean(v);
			}
			catch
			{
			}
			return false;
		}
		static public int MainWinowWidth
		{
			get
			{
				return GetInt(SECT_MainForm, ITEM_Width);
			}
			set
			{
				SetData(SECT_MainForm, ITEM_Width, value);
			}
		}
		static public int MainWinowHeight
		{
			get
			{
				return GetInt(SECT_MainForm, ITEM_Height);
			}
			set
			{
				SetData(SECT_MainForm, ITEM_Height, value);
			}
		}
		static public int MainWinowLeft
		{
			get
			{
				return GetInt(SECT_MainForm, ITEM_Left);
			}
			set
			{
				SetData(SECT_MainForm, ITEM_Left, value);
			}
		}
		static public int MainWinowTop
		{
			get
			{
				return GetInt(SECT_MainForm, ITEM_Top);
			}
			set
			{
				SetData(SECT_MainForm, ITEM_Top, value);
			}
		}
		static public FormWindowState MainWindowState
		{
			get
			{
				return (FormWindowState)Enum.Parse(typeof(FormWindowState), GetData(SECT_MainForm, ITEM_WindowState));
			}
			set
			{
				SetData(SECT_MainForm, ITEM_WindowState, value);
			}
		}
		static public bool ClearBuildLog
		{
			get
			{
				return GetBool(SECT_MainForm, ITEM_ClearBuildLog, true);
			}
			set
			{
				SetData(SECT_MainForm, ITEM_ClearBuildLog, value);
			}
		}
		static public string UserGuide
		{
			get
			{
				string s = GetData(SECT_Help, ITEM_UserGuide);
				if (string.IsNullOrEmpty(s))
				{
					return "http://www.limnor.com/support";
				}
				return s;
			}
			set
			{
				SetData(SECT_Help, ITEM_UserGuide, value);
			}
		}
		static public string TutorialFolder
		{
			get
			{
				return RegistryData.GetTutorialFolder();
			}
		}
		static public string Reference
		{
			get
			{
				string s = GetData(SECT_Help, ITEM_Reference);
				if (string.IsNullOrEmpty(s))
				{
					return "http://www.limnor.com/support";
				}
				return s;
			}
			set
			{
				SetData(SECT_Help, ITEM_Reference, value);
			}
		}
		public static string GetAttribute(XmlNode node, string name)
		{
			if (node != null)
			{
				if (!string.IsNullOrEmpty(name))
				{

					if (node.Attributes != null)
					{
						XmlAttribute xa = node.Attributes[name];
						if (xa != null)
						{
							return xa.Value;
						}
					}
				}
			}
			return "";
		}
		public static void SetAttribute(XmlNode node, string name, string value)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa == null)
				{
					xa = node.OwnerDocument.CreateAttribute(name);
					node.Attributes.Append(xa);
				}
				xa.Value = value;
			}
		}
	}
	public class VobOptions
	{
		public VobOptions()
		{
		}
		[Description("True:the previous build log will be remove before each build; False: build log will be appended to previous logs")]
		public bool ClearBuildLog
		{
			get
			{
				return VobState.ClearBuildLog;
			}
			set
			{
				VobState.ClearBuildLog = value;
			}
		}
		[Description("Gets and sets the folder where downloaded Users' Guide files are saved. By downloading the files to a local folder the files can be opened without connecting to the internet")]
		public string UsersGuideFolder
		{
			get
			{
				return VobState.UserGuide;
			}
			set
			{
				VobState.UserGuide = value;
			}
		}
		[Description("Gets the folder where the tutorial slides are installed. The tutorials can be doanloaded from http://www.limnor.com/studio/setuptutorials.msi")]
		public string TutorialsFolder
		{
			get
			{
				return VobState.TutorialFolder;
			}
		}
		[Description("Gets and sets the folder where downloaded References files are saved. By downloading the files to a local folder the files can be opened without connecting to the internet")]
		public string ReferencesFolder
		{
			get
			{
				return VobState.Reference;
			}
			set
			{
				VobState.Reference = value;
			}
		}
		private static bool _enableIdeProfiling;
		[Description("If it is set to True then the times of some operations of the IDE will be recorded in the log file. If the IDE runs slower than expected then the logging may give information about which operations are taking too long to finish. Please report the logging to support@limnor.com to improve the IDE. The setting is not saved on exiting the IDE. The setting is False when the IDE starts.")]
		public bool EnableProfiling
		{
			get
			{
				return _enableIdeProfiling;
			}
			set
			{
				_enableIdeProfiling = value;
			}
		}
		public static bool EnableIdeProfiling()
		{
			return _enableIdeProfiling;
		}
	}
}
