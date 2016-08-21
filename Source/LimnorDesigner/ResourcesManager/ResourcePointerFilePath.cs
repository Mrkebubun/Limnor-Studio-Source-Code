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
using System.Windows.Forms;
using WindowsUtility;
using System.Globalization;
using MathExp;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;

namespace LimnorDesigner.ResourcesManager
{
	public class ResourcePointerFilePath : ResourcePointer
	{
		#region fields and constructors
		private Font _font;
		public ResourcePointerFilePath()
		{
			_font = new Font("Times New Roman", 12);
		}
		public ResourcePointerFilePath(ProjectResources owner)
			: base(owner)
		{
			_font = new Font("Times New Roman", 12);
		}
		#endregion

		#region ResourcePointer
		internal override TreeNodeResource CreateTreeNode()
		{
			return new TreeNodeResourceFilePath(this);
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
		}
		public override bool SelectResourceFile(Form caller, string languageName)
		{
			try
			{
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.Title = "Select file name";
				string s = GetResourceString(languageName);
				if (!string.IsNullOrEmpty(s))
				{
					if (File.Exists(s))
					{
						dlg.FileName = s;
					}
				}
				dlg.Filter = "All|*.*";
				if (dlg.ShowDialog(caller) == DialogResult.OK)
				{
					SetResourceString(languageName, dlg.FileName);
					IsChanged = true;
					return true;
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(caller, err.Message, "Select file", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return false;
		}
		public override void OnSelected(TextBoxResEditor textBoxDefault, TextBoxResEditor textBoxLocal, PictureBoxResEditor pictureBoxDefault, PictureBoxResEditor pictureBoxLocal, CultureInfo c)
		{
			pictureBoxDefault.SetResourceOwner(this, null);
			pictureBoxDefault.Visible = true;
			pictureBoxDefault.SetReadOnly(c != null);
			if (c == null)
			{
				//for default, disable local
				pictureBoxLocal.SetResourceOwner(null, null);
			}
			else
			{
				//for local
				pictureBoxLocal.SetResourceOwner(this, c);
			}
			pictureBoxLocal.Visible = true;
			pictureBoxLocal.SetReadOnly(c == null);

			textBoxDefault.Visible = false;
			textBoxLocal.Visible = false;

		}
		public override object GetResourceValue(string languageName)
		{
			return GetResourceString(languageName);
		}
		public override string WebFolderName { get { return "media"; } }
		public override bool IsFile { get { return true; } }
		public override int TreeNodeIconIndex { get { return TreeViewObjectExplorer.IMG_STRING; } }
		public override string XmlTag { get { return ProjectResources.XML_FILENAMES; } }
		public override string ValueDisplay
		{
			get
			{
				string s = GetResourceString(string.Empty);
				if (string.IsNullOrEmpty(s))
				{
					return string.Empty;
				}
				return s;
			}
		}
		[ReadOnly(true)]
		public override Type ObjectType
		{
			get
			{
				return typeof(Filepath);
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
