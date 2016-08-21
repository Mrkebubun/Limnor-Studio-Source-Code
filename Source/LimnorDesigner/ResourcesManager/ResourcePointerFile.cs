/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing;
using XmlUtility;
using System.Xml;
using MathExp;
using System.IO;
using System.Xml.Serialization;
using WindowsUtility;

namespace LimnorDesigner.ResourcesManager
{
	public class ResourcePointerFile : ResourceFromFile
	{
		#region fields and constructors
		private Type _contentType;
		private Font _font;
		public ResourcePointerFile()
		{
			_font = new Font("Times New Roman", 12);
		}
		public ResourcePointerFile(ProjectResources owner)
			: base(owner)
		{
			_font = new Font("Times New Roman", 12);
		}
		private void mnu_setResType(object sender, EventArgs e)
		{
			MenuItem m = (MenuItem)sender;
			_contentType = (Type)(m.Tag);
			IsChanged = true;
		}
		#endregion

		#region ResourcePointer
		internal override TreeNodeResource CreateTreeNode()
		{
			return new TreeNodeResourceFile(this);
		}
		public override void AddFileContextMenus(List<MenuItem> l)
		{
			l.Add(new MenuItem("-"));
			MenuItemWithBitmap m = new MenuItemWithBitmap("Set resource type", Resources._type);
			MenuItem m1;

			if (typeof(string).Equals(_contentType))
			{
				m1 = new MenuItem("Text", mnu_setResType);
				m1.Checked = true;
			}
			else
			{
				m1 = new MenuItemWithBitmap("Text", mnu_setResType, Resources._string.ToBitmap());
			}
			m1.Tag = typeof(string);
			m.MenuItems.Add(m1);
			//
			if (typeof(byte[]).Equals(_contentType))
			{
				m1 = new MenuItem("Byte array", mnu_setResType);
				m1.Checked = true;
			}
			else
			{
				m1 = new MenuItemWithBitmap("Byte array", mnu_setResType, Resources.array);
			}
			m1.Tag = typeof(byte[]);
			m.MenuItems.Add(m1);
			//
			if (typeof(MemoryStream).Equals(_contentType))
			{
				m1 = new MenuItem("MemoryStream", mnu_setResType);
				m1.Checked = true;
			}
			else
			{
				m1 = new MenuItemWithBitmap("MemoryStream", mnu_setResType, Resources._dots.ToBitmap());
			}
			m1.Tag = typeof(MemoryStream);
			m.MenuItems.Add(m1);
			//
			if (typeof(UnmanagedMemoryStream).Equals(_contentType))
			{
				m1 = new MenuItem("UnmanagedMemoryStream", mnu_setResType);
				m1.Checked = true;
			}
			else
			{
				m1 = new MenuItemWithBitmap("UnmanagedMemoryStream", mnu_setResType, Resources._dots.ToBitmap());
			}
			m1.Tag = typeof(UnmanagedMemoryStream);
			m.MenuItems.Add(m1);
			//
			l.Add(m);
		}
		public override void OnPaintPictureBox(PaintEventArgs e, string languageName)
		{
			string s = GetResourceString(languageName);
			if (!string.IsNullOrEmpty(s))
			{
				e.Graphics.DrawString(s, _font, Brushes.Blue, (float)1, (float)1);
			}
		}
		protected override void OnSaveValue()
		{
			XmlNode nd = XmlData.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
			"{0}[@name='ObjectType']",
			 XmlTags.XML_PROPERTY));
			if (nd == null)
			{
				nd = XmlData.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
				XmlData.AppendChild(nd);
				XmlUtil.SetNameAttribute(nd, "ObjectType");
			}

			XmlNode tpNode = XmlUtil.CreateSingleNewElement(nd, XmlTags.XML_LIBTYPE);
			XmlUtil.SetLibTypeAttribute(tpNode, ObjectType);

		}
		protected override bool OnFileSelected(string selectedFile, string languageName)
		{
			string ext = System.IO.Path.GetExtension(selectedFile);
			if (string.Compare(ext, ".txt", StringComparison.OrdinalIgnoreCase) == 0
				|| string.Compare(ext, ".xml", StringComparison.OrdinalIgnoreCase) == 0
				|| string.Compare(ext, ".html", StringComparison.OrdinalIgnoreCase) == 0
				|| string.Compare(ext, ".htm", StringComparison.OrdinalIgnoreCase) == 0
				|| string.Compare(ext, ".xsd", StringComparison.OrdinalIgnoreCase) == 0
				|| string.Compare(ext, ".rtf", StringComparison.OrdinalIgnoreCase) == 0
				)
			{
				_contentType = typeof(string);
			}
			else
			{
				_contentType = typeof(byte[]);
			}
			return true;
		}
		public override string WebFolderName { get { return "media"; } }
		public override bool IsFile { get { return true; } }
		protected override string FileSelectionTitle { get { return "Select File"; } }
		protected override string FileSelectionFilter { get { return "Files|*.*"; } }

		public override object GetResourceValue(string languageName)
		{
			return GetResourceString(languageName);
		}
		public override int TreeNodeIconIndex { get { return TreeViewObjectExplorer.IMG_FILE; } }
		public override string XmlTag { get { return ProjectResources.XML_FILES; } }
		[ReadOnly(false)]
		public override Type ObjectType
		{
			get
			{
				if (_contentType != null)
				{
					return _contentType;
				}
				return typeof(byte[]);
			}
			set
			{
				_contentType = value;
			}
		}
		[XmlIgnore]
		[ReadOnly(true)]
		public override object ObjectInstance
		{
			get
			{
				return GetResourceString(Manager.DesignerLanguageName);
			}
			set
			{
				if (value is string)
				{
					string s = (string)value;
					SetResourceString(Manager.DesignerLanguageName, s);
				}
			}
		}
		#endregion
	}
}
