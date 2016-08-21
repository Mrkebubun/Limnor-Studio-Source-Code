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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace LimnorDesigner.ResourcesManager
{
	public class ResourcePointerIcon : ResourceFromFile
	{
		#region fields and constructors
		private Icon _icon;
		private Dictionary<string, Icon> _iconValues;
		public ResourcePointerIcon()
		{
		}
		public ResourcePointerIcon(ProjectResources owner)
			: base(owner)
		{
		}
		#endregion

		#region ResourcePointer
		internal override TreeNodeResource CreateTreeNode()
		{
			return new TreeNodeResourceIcon(this);
		}
		public override void OnPaintPictureBox(PaintEventArgs e, string languageName)
		{
			Icon ic = (Icon)GetResourceValue(languageName);
			if (ic != null)
			{
				e.Graphics.DrawIcon(ic, 1, 1);
			}
		}
		protected override void OnSaveValue()
		{
		}
		public override bool IsFile { get { return true; } }
		public override string WebFolderName { get { return "images"; } }
		protected override string FileSelectionTitle { get { return "Select Icon File"; } }
		protected override string FileSelectionFilter { get { return "Icon|*.ico"; } }
		protected override bool OnFileSelected(string selectedFile, string languageName)
		{
			Icon bp = new Icon(selectedFile);
			if (bp != null)
			{
				if (string.IsNullOrEmpty(languageName))
				{
					_icon = bp;
				}
				else
				{
					if (_iconValues == null)
					{
						_iconValues = new Dictionary<string, Icon>();
					}
					if (_iconValues.ContainsKey(languageName))
					{
						_iconValues[languageName] = bp;
					}
					else
					{
						_iconValues.Add(languageName, bp);
					}
				}
				return true;
			}
			return false;
		}

		public override void ClearResource()
		{
			base.ClearResource();
			_icon = null;
		}
		public override object GetResourceValue(string languageName)
		{
			string f;
			if (string.IsNullOrEmpty(languageName))
			{
				if (_icon == null)
				{
					f = this.GetExistFilename(languageName);
					if (!string.IsNullOrEmpty(f))
					{
						_icon = new Icon(f);
					}
				}
				return _icon;
			}
			if (_iconValues != null)
			{
				if (_iconValues.ContainsKey(languageName))
				{
					return _iconValues[languageName];
				}
			}
			f = this.GetExistFilename(languageName);
			if (!string.IsNullOrEmpty(f))
			{
				Icon bp = new Icon(f);
				if (bp != null)
				{
					if (_iconValues == null)
					{
						_iconValues = new Dictionary<string, Icon>();
					}
					_iconValues.Add(languageName, bp);
					return bp;
				}
			}
			return null;
		}
		public override int TreeNodeIconIndex { get { return TreeViewObjectExplorer.IMG_ICON; } }
		public override string XmlTag { get { return ProjectResources.XML_ICONS; } }
		[ReadOnly(true)]
		public override Type ObjectType
		{
			get
			{
				return typeof(Icon);
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
				return GetResourceValue(Manager.DesignerLanguageName);
			}
			set
			{
				if (value is string)
				{
					string f = (string)value;
					if (string.IsNullOrEmpty(f) || System.IO.File.Exists(f))
					{
						//remove binary value
						SetCurrentResourceString(f);
						if (string.IsNullOrEmpty(Manager.DesignerLanguageName))
						{
							_icon = null;
						}
						else
						{
							if (_iconValues != null)
							{
								if (_iconValues.ContainsKey(Manager.DesignerLanguageName))
								{
									_iconValues.Remove(Manager.DesignerLanguageName);
								}
							}
						}
					}
				}
				else
				{
					if (value is Icon)
					{
						Icon bp = (Icon)value;
						SetCurrentResourceString(string.Empty);
						if (string.IsNullOrEmpty(Manager.DesignerLanguageName))
						{
							_icon = bp;
						}
						else
						{
							if (_iconValues == null)
							{
								_iconValues = new Dictionary<string, Icon>();
							}
							if (_iconValues.ContainsKey(Manager.DesignerLanguageName))
							{
								_iconValues[Manager.DesignerLanguageName] = bp;
							}
							else
							{
								_iconValues.Add(Manager.DesignerLanguageName, bp);
							}
						}
					}
				}
			}
		}
		#endregion
	}
}
