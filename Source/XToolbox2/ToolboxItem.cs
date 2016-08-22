/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Toolbox system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.ComponentModel;
using System.Xml;
using VPL;
using System.Drawing.Design;

namespace XToolbox2
{
	/// <summary>
	/// xToolboxItem.
	/// </summary>
	[ToolboxBitmap(typeof(xToolboxItem), "arrow.bmp")]
	public class xToolboxItem : ToolboxItem
	{
		private Type m_type = null;
		private Type m_xcomponent = null;
		public xToolboxItem(Type t, Type innerType)
			: base(t)
		{
			m_xcomponent = innerType;
		}
		public xToolboxItem(Type t)
			: base(t)
		{
			m_type = t;
			if (t != null)
			{
				System.Drawing.ToolboxBitmapAttribute tba = TypeDescriptor.GetAttributes(t)[typeof(System.Drawing.ToolboxBitmapAttribute)] as System.Drawing.ToolboxBitmapAttribute;
				if (tba != null)
				{
					Bitmap = (System.Drawing.Bitmap)tba.GetImage(t);
				}
			}
			else
			{
				DisplayName = " Pointer";
				System.Drawing.ToolboxBitmapAttribute tba = TypeDescriptor.GetAttributes(typeof(xToolboxItem))[typeof(System.Drawing.ToolboxBitmapAttribute)] as System.Drawing.ToolboxBitmapAttribute;
				if (tba != null)
				{
					Bitmap = (System.Drawing.Bitmap)tba.GetImage(typeof(xToolboxItem));
				}
				else
				{
					Bitmap = new System.Drawing.Bitmap(16, 16);
				}
			}
		}
		public xToolboxItem(Type t, Bitmap bmp)
			: base(t)
		{
			m_type = t;
			if (bmp != null)
			{
				Bitmap = bmp;
			}
			else
			{
				System.Drawing.ToolboxBitmapAttribute tba = TypeDescriptor.GetAttributes(t)[typeof(System.Drawing.ToolboxBitmapAttribute)] as System.Drawing.ToolboxBitmapAttribute;
				if (tba != null)
				{
					Bitmap = (System.Drawing.Bitmap)tba.GetImage(t);
				}
			}
		}
		public xToolboxItem(Type t, Bitmap bmp, string text)
			: this(t, bmp)
		{
			DisplayName = text;
		}
		public Type Type
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type = value;
			}
		}
		public Type InnerType
		{
			get
			{
				return m_xcomponent;
			}
		}
		public virtual XmlNode CreateNode(XmlDocument xml)
		{
			XmlNode node = xml.CreateElement("Item");
			node.InnerText = m_type.AssemblyQualifiedName;
			if (m_xcomponent != null)
			{
				XmlNode inner = xml.CreateElement("InnerType");
				inner.InnerText = m_xcomponent.AssemblyQualifiedName;
				node.AppendChild(inner);
			}
			return node;
		}
		public virtual void ReadNode(XmlNode node)
		{
			m_type = Type.GetType(node.InnerText);
			XmlNode inner = node.SelectSingleNode("InnerType");
			if (inner != null)
			{
				m_xcomponent = Type.GetType(inner.InnerText);
			}
		}
	}
	public class ToolboxItemCom : xToolboxItem
	{
		private string guid = ""; //for getting icon
		public string[] Files = new string[0]; //com files
		public string regName = "";
		public ToolboxItemCom(Type t)
			: base(t)
		{
		}
		public string Guid
		{
			get
			{
				return guid;
			}
			set
			{
				guid = value;
				findImage();
			}
		}
		protected void findImage()
		{
			System.Drawing.Image img = null;
			Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("CLSID\\" + guid + "\\ToolboxBitmap32");
			string s = "";
			if (reg != null)
			{
				object v = reg.GetValue(null);
				if (v != null)
				{
					s = v.ToString();
				}
				reg.Close();
			}
			if (s.Length > 0)
			{
				int n = s.LastIndexOf(',');
				if (n > 0)
				{
					try
					{
						string sModule = s.Substring(0, n);
						string sRes = s.Substring(n + 1);
						sModule = sModule.Trim();
						sRes = sRes.Trim();
						if (sRes.Length > 0)
						{
							int nRes = Convert.ToInt32(sRes);
							IntPtr hBitmap = CPP.DLLLoadRes_Bitmap(sModule, nRes);
							if (hBitmap != IntPtr.Zero)
							{
								img = System.Drawing.Image.FromHbitmap(hBitmap);
								CPP.DeleteObject(hBitmap);
							}
							else
							{
								uint errCode = CPP.GetLastError();
								if (errCode != 0)
								{
								}
								System.Text.StringBuilder sb = new System.Text.StringBuilder(300);
								CPP.DLLGetLastError(sb, 300);
								string sw = sb.ToString();
								if (sw != null)
								{
								}
							}
						}
					}
					catch
					{
					}
				}
			}
			if (img != null)
			{
				this.Bitmap = (Bitmap)img;
			}
		}
		public override XmlNode CreateNode(XmlDocument xml)
		{
			XmlNode node = xml.CreateElement("Item");
			node.InnerText = Type.AssemblyQualifiedName;
			XmlAttribute xa = xml.CreateAttribute("Guid");
			xa.Value = Guid;
			node.Attributes.Append(xa);
			xa = xml.CreateAttribute("Regname");
			xa.Value = regName;
			node.Attributes.Append(xa);
			xa = xml.CreateAttribute("Type");
			xa.Value = this.GetType().AssemblyQualifiedName;
			node.Attributes.Append(xa);
			for (int i = 0; i < Files.Length; i++)
			{
				XmlNode nd = xml.CreateElement("File");
				nd.InnerText = Files[i];
				node.AppendChild(nd);
			}
			return node;
		}
		public override void ReadNode(XmlNode node)
		{
			XmlAttribute xa = node.Attributes["Guid"];
			if (xa != null)
			{
				Guid = xa.Value;
			}
			xa = node.Attributes["Regname"];
			if (xa != null)
			{
				regName = xa.Value;
			}
			Files = new string[node.ChildNodes.Count];
			for (int i = 0; i < node.ChildNodes.Count; i++)
			{
				Files[i] = node.ChildNodes[i].InnerText;
			}
		}
	}
}
