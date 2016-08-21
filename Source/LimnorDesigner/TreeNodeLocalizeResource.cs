/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using LimnorDesigner.ResourcesManager;
using System.Globalization;

namespace LimnorDesigner
{
	class TreeNodeLocalizeResource : TreeNode
	{
		private ResourcePointer _pointer;
		private CultureInfo _culture;
		private string _name;
		public TreeNodeLocalizeResource(ResourcePointer owner, CultureInfo culture)
		{
			_pointer = owner;
			_culture = culture;
			Text = string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0} [{1}]", _culture.NativeName, _culture.Name);
			ImageIndex = TreeViewObjectExplorer.GetLangaugeImageByName(_culture.Name);
			SelectedImageIndex = ImageIndex;
		}
		public TreeNodeLocalizeResource(ResourcePointer owner, string culture)
		{
			_pointer = owner;
			_name = culture;
			if (string.CompareOrdinal("zh", _name) == 0)
			{
				_culture = CultureInfo.GetCultureInfo("zh-CHT");
				Text = "中文 zh";
				ImageIndex = TreeViewObjectExplorer.GetLangaugeImageByName(_name);
				SelectedImageIndex = ImageIndex;
			}
		}
		public void OnSelected(TextBoxResEditor textBoxDefault, TextBoxResEditor textBoxLocal, PictureBoxResEditor pictureBoxDefault, PictureBoxResEditor pictureBoxLocal, CultureInfo c)
		{
			_pointer.OnSelected(textBoxDefault, textBoxLocal, pictureBoxDefault, pictureBoxLocal, c);
		}
		public string CultureName
		{
			get
			{
				return _name;
			}
		}
		public ResourcePointer Pointer
		{
			get
			{
				return _pointer;
			}
		}
		public CultureInfo Culture
		{
			get
			{
				return _culture;
			}
		}
	}
}
