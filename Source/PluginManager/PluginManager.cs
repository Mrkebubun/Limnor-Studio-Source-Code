/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Plugin Manager
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Xml;
using XmlUtility;
using System.Reflection;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;

namespace LimnorVisualProgramming
{
	[Designer(typeof(ComponentDocumentDesigner), typeof(ComponentDesigner))]
	[Description("It manages plugins")]
	[ToolboxBitmap(typeof(DialogPluginManager), "Resources.pluginMan.bmp")]
	public class PluginManager<PluginType> : IComponent, IPluginManager
	{
		#region fields and constructors
		const string XML_Item = "Item";
		const string XMLATT_file = "file";
		private List<PluginObject> _plugins;
		private List<string> _dllFiles;
		public PluginManager()
		{
		}
		public PluginManager(IContainer c)
		{
			if (c != null)
			{
				c.Add(this);
			}
		}
		#endregion

		#region IComponent Members
		[Browsable(false)]
		public event EventHandler Disposed;

		[ReadOnly(true)]
		[Browsable(false)]
		public ISite Site
		{
			get;
			set;
		}

		#endregion

		#region IDisposable Members
		[Browsable(false)]
		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region internal methods
		internal IList<PluginItem> GetAllPluginItems()
		{
			List<PluginItem> items = new List<PluginItem>();
			string cfgFile = PluginConfigurationFileFullpath;
			if (!string.IsNullOrEmpty(cfgFile))
			{
				if (File.Exists(cfgFile))
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(cfgFile);
					if (doc.DocumentElement != null)
					{
						XmlNodeList nds = doc.DocumentElement.SelectNodes(XML_Item);
						if (nds != null && nds.Count > 0)
						{
							foreach (XmlNode nd in nds)
							{
								PluginItem item = new PluginItem();
								item.LoadFromXmlNode(nd);
								if (item.PluginItemType != null && typeof(PluginType).IsAssignableFrom(item.PluginItemType))
								{
									items.Add(item);
								}
							}
						}
					}
				}
			}
			return items;
		}
		#endregion

		#region Properties
		string _name = string.Empty;
		[ParenthesizePropertyName(true)]
		public string Name
		{
			get
			{
				if (Site != null)
				{
					return Site.Name;
				}
				return _name;
			}
			set
			{
				if (Site != null)
				{
					Site.Name = value;
				}
				_name = value;
			}
		}
		[Description("Gets full path of the configuration file")]
		public string PluginConfigurationFileFullpath
		{
			get
			{
				string p = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
				if (!string.IsNullOrEmpty(PluginConfigurationFoldername))
				{
					p = Path.Combine(p, PluginConfigurationFoldername.Trim());
					if (!Directory.Exists(p))
					{
						Directory.CreateDirectory(p);
					}
				}
				if (!string.IsNullOrEmpty(PluginConfigurationFilename))
				{
					string s = PluginConfigurationFilename.Trim();
					if (s.Length > 0)
					{
						return Path.Combine(p, s);
					}
				}
				return string.Empty;
			}
		}
		[Description("Gets object type of the plugins")]
		public Type PlginType
		{
			get
			{
				return typeof(PluginType);
			}
		}
		[Description("Gets all plugins")]
		public IList<PluginObject> Plugins
		{
			get
			{
				if (_plugins == null)
				{
					_plugins = new List<PluginObject>();
				}
				return _plugins;
			}
		}
		[Description("Gets all plugin files")]
		public IList<string> PluginDllFiles
		{
			get
			{
				return _dllFiles;
			}
		}
		[Description("Gets and sets folder name for the configuration file recording the plugins. The full path of the configuration file is formed by {ProgramData folder}\\{PluginConfigurationFoldername}\\{PluginConfigurationFilename}")]
		public string PluginConfigurationFoldername
		{
			get;
			set;
		}
		[Description("Gets and sets file name recording the plugins. The full path of the configuration file is formed by {ProgramData folder}\\{PluginConfigurationFoldername}\\{PluginConfigurationFilename}")]
		public string PluginConfigurationFilename
		{
			get;
			set;
		}
		#endregion

		#region Methods
		public virtual void OnNotifyDataChanged(IPlugin sender, EventArgsDataName e)
		{
			if (_plugins != null && _plugins.Count > 0)
			{
				foreach (PluginObject obj in _plugins)
				{
					IPlugin plugin = obj.Plugin as IPlugin;
					if (plugin != null)
					{
						if (plugin != sender)
						{
							plugin.OnDataChanged(this, e);
						}
					}
				}
			}
		}
		public PluginType GetPluginByName(string pluginName, bool caseInsensitive)
		{
			if (_plugins != null && _plugins.Count > 0)
			{
				foreach (PluginObject obj in _plugins)
				{
					if (caseInsensitive)
					{
						if (string.Compare(obj.Name, pluginName, StringComparison.OrdinalIgnoreCase) == 0)
						{
							return obj.Plugin;
						}
					}
					else
					{
						if (string.CompareOrdinal(obj.Name, pluginName) == 0)
						{
							return obj.Plugin;
						}
					}
				}
			}
			return default(PluginType);
		}
		[Description("Load plugins from the configuration file")]
		public void RefreshPlugins()
		{
			_dllFiles = new List<string>();
			List<PluginObject> plugins = new List<PluginObject>();
			IList<PluginItem> items = GetAllPluginItems();
			foreach (PluginItem item in items)
			{
				if (!_dllFiles.Contains(item.Filepath))
				{
					_dllFiles.Add(item.Filepath);
				}
				if (item.Enabled)
				{
					bool bExists = false;
					if (_plugins != null)
					{
						foreach (PluginObject pt in _plugins)
						{
							if (pt.Plugin.GetType().Equals(item.PluginItemType))
							{
								plugins.Add(pt);
								bExists = true;
								break;
							}
						}
					}
					if (!bExists)
					{
						PluginType obj = (PluginType)Activator.CreateInstance(item.PluginItemType);
						plugins.Add(new PluginManager<PluginType>.PluginObject(item.Name, obj));
						IPlugin plugin = obj as IPlugin;
						if (plugin != null)
						{
							plugin.DataChanged += new EventHandlerDataChanged(plugin_DataChanged);
							plugin.OnInitialize(this);
						}
					}
				}
			}
			_plugins = plugins;
		}

		void plugin_DataChanged(IPlugin sender, EventArgsDataName e)
		{
			OnNotifyDataChanged(sender, e);
		}
		[Description("Add and remove plugins. Returns True if the user accepted the changes.")]
		public bool ManagePlugin(Form form)
		{
			DialogPluginManager dlg = new DialogPluginManager();
			if (dlg.LoadData(this, form))
			{
				if (dlg.ShowDialog(form) == DialogResult.OK)
				{
					IList<PluginItem> lst = dlg.GetResult();
					XmlDocument doc = new XmlDocument();
					XmlNode rootNode = doc.CreateElement("PluginConfigure");
					doc.AppendChild(rootNode);
					foreach (PluginItem s in lst)
					{
						XmlNode node = doc.CreateElement(XML_Item);
						rootNode.AppendChild(node);
						s.SaveToXmlNode(node);
					}
					doc.Save(this.PluginConfigurationFileFullpath);
					RefreshPlugins();
					return true;
				}
			}
			return false;
		}
		#endregion

		#region PluginObject
		public class PluginObject
		{
			private string _name;
			private PluginType _plugin;
			internal PluginObject(string name, PluginType obj)
			{
				_name = name;
				_plugin = obj;
			}
			public string Name
			{
				get
				{
					return _name;
				}
			}
			public PluginType Plugin
			{
				get
				{
					return _plugin;
				}
			}
			public class AAA
			{
			}
		}
		#endregion
	}
	#region class Item
	internal class PluginItem
	{
		const string XMLATT_enabled = "enabled";
		private string _filepath;
		public PluginItem()
		{
			Enabled = true;
		}
		public string Name { get; set; }
		public Type PluginItemType { get; set; }
		public bool Enabled { get; set; }
		public bool Modified { get; set; }
		public string Filepath
		{
			get
			{
				return _filepath;
			}
		}
		public override string ToString()
		{
			return Name;
		}
		public void LoadFromXmlNode(XmlNode node)
		{
			Name = XmlUtil.GetNameAttribute(node);
			Enabled = XmlUtil.GetAttributeBoolDefTrue(node, XMLATT_enabled);
			PluginItemType = XmlUtil.GetLibTypeAttribute(node);
			if (PluginItemType != null && PluginItemType.Assembly != null && PluginItemType.Assembly.Location != null)
			{
				_filepath = PluginItemType.Assembly.Location.ToLowerInvariant();
			}
		}
		public void SaveToXmlNode(XmlNode node)
		{
			XmlUtil.SetNameAttribute(node, Name);
			XmlUtil.SetAttribute(node, XMLATT_enabled, Enabled);
			if (PluginItemType != null)
			{
				XmlUtil.SetLibTypeAttribute(node, PluginItemType);
			}
		}
	}
	#endregion
}
