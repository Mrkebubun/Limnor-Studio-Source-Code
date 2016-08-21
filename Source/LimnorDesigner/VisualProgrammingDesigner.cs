/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using VSPrj;
using XmlUtility;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Specialized;

namespace LimnorDesigner
{
	public class VisualProgrammingDesigner
	{
		#region fields and constructors
		const string VP_Name = "Name";
		const string VP_File = "File";
		const string VP_Enabled = "Enabled";
		const string VP_Desc = "Description";
		private string _name;
		private string _file;
		private bool _enabled;
		private string _desc;
		private string _error;
		private static StringCollection _buildinNames;
		private static StringCollection _excludedDesigners = new StringCollection();
		private static Dictionary<string, VisualProgrammingDesigner> _designers;
		static VisualProgrammingDesigner()
		{
			_buildinNames = new StringCollection();
			_buildinNames.Add("XHost.ObjectExplorerView");
			_buildinNames.Add("LimnorDesigner.ObjectExplorerView");
			_buildinNames.Add("XHost.FormViewer");
			_buildinNames.Add("Longflow.VSMain.FormViewer");
			_buildinNames.Add("LimnorDesigner.EventMap.EventPathHolder");
		}
		public VisualProgrammingDesigner(string name, string file, bool enabled, string desc)
		{
			_name = name;
			_file = file;
			_enabled = enabled;
			_desc = desc;
			CanRemove = true;
		}
		#endregion

		#region Properties
		public string Name
		{
			get
			{
				return _name;
			}
		}
		public string File
		{
			get
			{
				return _file;
			}
		}
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}
		public bool CanRemove { get; set; }
		public string Description
		{
			get
			{
				return _desc;
			}
		}
		public string Error
		{
			get
			{
				return _error;
			}
		}
		#endregion

		#region Methods
		public bool IsBuiltIn()
		{
			return _buildinNames.Contains(_name);
		}
		public Dictionary<string, string> CreateProperties()
		{
			Dictionary<string, string> ls = new Dictionary<string, string>();
			ls.Add(VP_Name, _name);
			ls.Add(VP_Enabled, _enabled.ToString());
			ls.Add(VP_Desc, _desc);
			ls.Add(VP_File, _file);
			return ls;
		}
		public Type GetDesignerType()
		{
			Type t = null;
			if (string.IsNullOrEmpty(_name))
			{
				_error = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Missing designer name. Designer file: {0}", _file);
			}
			else
			{
				t = Type.GetType(_name);
				if (t == null)
				{
					if (string.IsNullOrEmpty(_file))
					{
						_error = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"Missing designer file. Designer name: {0}", _name);
					}
					else
					{
						if (System.IO.File.Exists(_file))
						{
							try
							{
								Assembly a = Assembly.LoadFile(_file);
								if (a == null)
								{
								}
								else
								{
									Type[] tps = a.GetExportedTypes();
									if (tps != null && tps.Length > 0)
									{
										for (int i = 0; i < tps.Length; i++)
										{
											if (string.CompareOrdinal(_name, tps[i].FullName) == 0)
											{
												t = tps[i];
												break;
											}
										}
									}
								}
							}
							catch (Exception err)
							{
								_error = string.Format(System.Globalization.CultureInfo.InvariantCulture,
										"Designer file [{0}] cannot be loaded. Error message: {1}. Designer name: {2}", _file, err.Message, _name);
							}
						}
						else
						{
							_error = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"Designer file [{0}] does not exist. Designer name: {1}", _file, _name);
						}
					}
				}
			}
			return t;
		}
		#endregion

		#region Static utility
		public static void AddExcludedDesigner(string designer)
		{
			_excludedDesigners.Add(designer);
		}
		public static Dictionary<string, VisualProgrammingDesigner> Designers
		{
			get
			{
				return _designers;
			}
		}
		private static string vplFile
		{
			get
			{
				return System.IO.Path.Combine(DesignUtil.GetApplicationDataFolder(), "VPS.xml");
			}
		}
		public static bool DesignerEnabled(Type t)
		{
			if (_designers.ContainsKey(t.FullName))
			{
				VisualProgrammingDesigner d = _designers[t.FullName];
				return d.Enabled;
			}
			return true;
		}
		public static bool DesignerBuiltIn(Type t)
		{
			if (_buildinNames.Contains(t.FullName))
			{
				return true;
			}
			if (_designers.ContainsKey(t.FullName))
			{
				VisualProgrammingDesigner d = _designers[t.FullName];
				return !d.CanRemove;
			}
			return false;
		}
		public static VisualProgrammingDesigner CreateDesigner(Dictionary<string, string> ls)
		{
			if (ls.ContainsKey(VP_Name) && ls.ContainsKey(VP_File))
			{
				string name = ls[VP_Name];
				string file = ls[VP_File];
				if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(file))
				{
					if (!_excludedDesigners.Contains(name))
					{
						bool enabled = true;
						string desc = "";
						if (ls.ContainsKey(VP_Enabled))
						{
							enabled = (string.Compare("True", ls[VP_Enabled], StringComparison.OrdinalIgnoreCase) == 0);
						}
						if (ls.ContainsKey(VP_Desc))
						{
							desc = ls[VP_Desc];
						}
						return new VisualProgrammingDesigner(name, file, enabled, desc);
					}
				}
			}
			return null;
		}
		public static void SaveDesigners(Dictionary<string, VisualProgrammingDesigner> designers)
		{
			XmlDocument doc = new XmlDocument();
			XmlNode XmlData = doc.CreateElement("Root");
			doc.AppendChild(XmlData);
			XmlNode node = XmlData.OwnerDocument.CreateElement(XmlTags.XML_VPList);
			XmlData.AppendChild(node);
			foreach (VisualProgrammingDesigner p in designers.Values) // in bag)
			{
				XmlNode nd = XmlData.OwnerDocument.CreateElement(XmlTags.XML_Item);
				node.AppendChild(nd);
				Dictionary<string, string> props = p.CreateProperties();
				foreach (KeyValuePair<string, string> kv in props)
				{
					XmlUtil.SetAttribute(nd, kv.Key, kv.Value);
				}
			}
			doc.Save(vplFile);
			_designers = designers;
		}
		public static Dictionary<string, VisualProgrammingDesigner> LoadDesigners(List<Type> builtinDesgners)
		{
			Dictionary<string, VisualProgrammingDesigner> ret = new Dictionary<string, VisualProgrammingDesigner>();
			string f = vplFile;
			if (System.IO.File.Exists(f))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(f);
				if (doc.DocumentElement != null)
				{
					PropertyBag list = new PropertyBag();
					XmlNodeList nlist = doc.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					 "{0}/{1}",
					XmlTags.XML_VPList, XmlTags.XML_Item));
					if (nlist != null && nlist.Count > 0)
					{
						foreach (XmlNode node in nlist)
						{
							Dictionary<string, string> props = new Dictionary<string, string>();
							foreach (XmlAttribute xa in node.Attributes)
							{
								props.Add(xa.Name, xa.Value);
							}
							list.Add(props);
						}
					}
					foreach (Dictionary<string, string> ls in list)
					{
						VisualProgrammingDesigner r = VisualProgrammingDesigner.CreateDesigner(ls);
						if (r != null)
						{
							if (!ret.ContainsKey(r.Name))
							{
								ret.Add(r.Name, r);
							}
						}
					}
				}
			}
			foreach (Type t in builtinDesgners)
			{
				VisualProgrammingDesigner v;
				if (ret.ContainsKey(t.FullName))
				{
					v = ret[t.FullName];
				}
				else
				{
					string desc;
					object[] a = t.GetCustomAttributes(typeof(DescriptionAttribute), true);
					if (a != null && a.Length > 0)
					{
						desc = ((DescriptionAttribute)a[0]).Description;
					}
					else
					{
						desc = t.FullName;
					}
					v = new VisualProgrammingDesigner(t.FullName, t.Assembly.Location, true, desc);
					ret.Add(t.FullName, v);
				}
				v.CanRemove = false;
			}
			_designers = ret;
			return ret;
		}
		#endregion
	}
}
