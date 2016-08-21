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
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing;

namespace LimnorDesigner.ResourcesManager
{
	public class ResourcePointerAudio : ResourceFromFile
	{
		#region fields and constructors
		Font _font;

		public ResourcePointerAudio()
		{
			_font = new Font("Times New Roman", 12);
		}
		public ResourcePointerAudio(ProjectResources owner)
			: base(owner)
		{
			_font = new Font("Times New Roman", 12);
		}
		#endregion

		#region ResourcePointer
		internal override TreeNodeResource CreateTreeNode()
		{
			return new TreeNodeResourceAudio(this);
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
		public override string WebFolderName { get { return "media"; } }
		protected override string FileSelectionTitle { get { return "Select Audio File"; } }
		protected override string FileSelectionFilter { get { return "Wave|*.wav|MP3|*.mp3"; } }
		protected override bool OnFileSelected(string selectedFile, string languageName)
		{
			return true;
		}

		public override object GetResourceValue(string languageName)
		{
			return null;
		}
		public override int TreeNodeIconIndex { get { return TreeViewObjectExplorer.IMG_AUDIO; } }
		public override string XmlTag { get { return ProjectResources.XML_AUDIOS; } }
		[ReadOnly(true)]
		public override Type ObjectType
		{
			get
			{
				return typeof(MemoryStream);
			}
			set
			{

			}
		}
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
