/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Limnor.Drawing2D
{
	sealed class DefaultDrawings
	{
		Dictionary<Type, DrawingItem> _defaultObjects;
		public DefaultDrawings()
		{
			_defaultObjects = new Dictionary<Type, DrawingItem>();
			if (File.Exists(Filename))
			{
				try
				{
					List<Type> types = new List<Type>();
					XmlDocument doc = new XmlDocument();
					doc.Load(Filename);
					XmlNodeList typeNodes = doc.DocumentElement.SelectNodes("Types/Type");
					if (typeNodes != null)
					{
						string sVer = null;
						foreach (XmlNode nd in typeNodes)
						{
							Type t = Type.GetType(nd.InnerText);
							if (t == null)
							{
								//switch the version
								int p1 = nd.InnerText.IndexOf("Version=");
								if (p1 > 0)
								{
									int p2 = nd.InnerText.IndexOf(",", p1);
									if (p2 > 0)
									{
										if (string.IsNullOrEmpty(sVer))
										{
											sVer = this.GetType().Assembly.GetName().Version.ToString();
										}
										string sType = nd.InnerText.Substring(0, p1 + "Version=".Length) + sVer + nd.InnerText.Substring(p2);
										t = Type.GetType(sType);
									}
								}
							}
							if (t != null)
							{
								types.Add(t);
							}
						}
					}
					TextReader r = new StreamReader(Filename);
					XmlSerializer s = new XmlSerializer(typeof(List<DrawingItem>), types.ToArray());
					List<DrawingItem> drawings = (List<DrawingItem>)s.Deserialize(r);
					r.Close();
					foreach (DrawingItem item in drawings)
					{
						Type t = item.GetType();
						if (!_defaultObjects.ContainsKey(t))
						{
							_defaultObjects.Add(t, item);
						}
					}
				}
				catch (Exception err)
				{
					MessageBox.Show(err.Message);
				}
			}
		}
		public string Filename
		{
			get
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Limnor Studio\\DefaultDrawings.xml");
			}
		}
		public void SetDefaultDrawing(DrawingItem item)
		{
			Type t = item.GetType();
			DrawingItem d = (DrawingItem)item.Clone();
			if (_defaultObjects.ContainsKey(t))
			{
				_defaultObjects[t] = d;
			}
			else
			{
				_defaultObjects.Add(t, d);
			}
			updateFile();
		}
		public DrawingItem GetDefaultDrawing(Type t)
		{
			DrawingItem item;
			if (_defaultObjects.TryGetValue(t, out item))
			{
				return item;
			}
			return null;
		}
		private void updateFile()
		{
			try
			{
				List<Type> types = new List<Type>();
				List<DrawingItem> items = new List<DrawingItem>();
				foreach (KeyValuePair<Type, DrawingItem> kv in _defaultObjects)
				{
					types.Add(kv.Key);
					items.Add(kv.Value);
				}
				XmlSerializer s = new XmlSerializer(typeof(List<DrawingItem>), types.ToArray());
				TextWriter w = new StreamWriter(Filename);
				s.Serialize(w, items);
				w.Close();
				XmlDocument doc = new XmlDocument();
				doc.Load(Filename);
				XmlNode typesNode = doc.CreateElement("Types");
				doc.DocumentElement.AppendChild(typesNode);
				foreach (Type t in types)
				{
					XmlNode tn = doc.CreateElement("Type");
					typesNode.AppendChild(tn);
					tn.InnerText = t.AssemblyQualifiedName;
				}
				doc.Save(Filename);
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message);
			}
		}
	}
}
