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
using System.Xml.Serialization;

namespace LimnorDesigner.ResourcesManager
{
	public class ResourcePointerString : ResourcePointer
	{
		#region fields and constructors

		public ResourcePointerString()
		{
		}
		public ResourcePointerString(ProjectResources owner)
			: base(owner)
		{
		}
		#endregion

		#region ResourcePointer
		internal override TreeNodeResource CreateTreeNode()
		{
			return new TreeNodeResourceString(this);
		}
		public override void OnPaintPictureBox(PaintEventArgs e, string languageName)
		{
		}
		protected override void OnSaveValue()
		{
		}
		public override bool SelectResourceFile(Form caller, string languageName)
		{
			return false;
		}
		public override void OnSelected(TextBoxResEditor textBoxDefault, TextBoxResEditor textBoxLocal, PictureBoxResEditor pictureBoxDefault, PictureBoxResEditor pictureBoxLocal, CultureInfo c)
		{
			textBoxDefault.SetResourceOwner(this, null);
			textBoxDefault.ReadOnly = (c != null);
			textBoxDefault.Visible = true;
			if (c == null)
			{
				textBoxLocal.SetResourceOwner(null, c);
			}
			else
			{
				textBoxLocal.SetResourceOwner(this, c);
			}
			textBoxLocal.ReadOnly = (c == null);
			textBoxLocal.Visible = true;
			pictureBoxDefault.Visible = false;
			pictureBoxLocal.Visible = false;

		}
		public override object GetResourceValue(string languageName)
		{
			return GetResourceString(languageName);
		}
		public override bool IsFile { get { return false; } }
		public override string WebFolderName { get { return string.Empty; } }
		public override int TreeNodeIconIndex { get { return TreeViewObjectExplorer.IMG_STRING; } }
		public override string XmlTag { get { return ProjectResources.XML_STRINGS; } }
		public override string ValueDisplay
		{
			get
			{
				string s = GetResourceString(string.Empty);
				if (string.IsNullOrEmpty(s))
				{
					return string.Empty;
				}
				if (s.Length < VALUE_DISPLAY_LEN)
				{
					return s;
				}
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}...", s.Substring(0, VALUE_DISPLAY_LEN));
			}
		}
		[ReadOnly(true)]
		public override Type ObjectType
		{
			get
			{
				return typeof(string);
			}
			set
			{

			}
		}
		[XmlIgnore]
		[ReadOnly(true)]
		public override object ObjectInstance
		{
			get
			{
				return GetResourceString(string.Empty);
			}
			set
			{
				if (value == null || value == DBNull.Value)
				{
					SetResourceString(string.Empty, string.Empty);
				}
				SetResourceString(string.Empty, Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture));
			}
		}
		#endregion
	}
}
