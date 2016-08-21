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
	public class ResourcePointerImage : ResourceFromFile
	{
		#region fields and constructors
		private Bitmap _bmp;
		private Dictionary<string, Bitmap> _bitmapValues;
		public ResourcePointerImage()
		{
		}
		public ResourcePointerImage(ProjectResources owner)
			: base(owner)
		{
		}
		#endregion

		#region ResourcePointer
		internal override TreeNodeResource CreateTreeNode()
		{
			return new TreeNodeResourceImage(this);
		}
		public override bool UsePictureBox { get { return true; } }
		public override bool IsFile { get { return true; } }
		public override string WebFolderName { get { return "images"; } }
		public override void OnPaintPictureBox(PaintEventArgs e, string languageName)
		{
		}
		protected override void OnSaveValue()
		{
		}
		protected override string FileSelectionTitle { get { return "Select Image File"; } }
		protected override string FileSelectionFilter { get { return "Bitmap|*.bmp|Jpeg|*.jpg|PNG|*.png|GIF|*.gif"; } }
		protected override bool OnFileSelected(string selectedFile, string languageName)
		{
			Bitmap bp = (Bitmap)Bitmap.FromFile(selectedFile);
			if (bp != null)
			{
				if (string.IsNullOrEmpty(languageName))
				{
					_bmp = bp;
				}
				else
				{
					if (_bitmapValues == null)
					{
						_bitmapValues = new Dictionary<string, Bitmap>();
					}
					if (_bitmapValues.ContainsKey(languageName))
					{
						_bitmapValues[languageName] = bp;
					}
					else
					{
						_bitmapValues.Add(languageName, bp);
					}
				}
				return true;
			}
			return false;
		}

		public override void ClearResource()
		{
			base.ClearResource();
			_bmp = null;
		}

		public override object GetResourceValue(string languageName)
		{
			string f;
			if (string.IsNullOrEmpty(languageName))
			{
				if (_bmp == null)
				{
					f = this.GetExistFilename(languageName);
					if (!string.IsNullOrEmpty(f))
					{
						_bmp = (Bitmap)Bitmap.FromFile(f);
					}
				}
				return _bmp;
			}
			if (_bitmapValues != null)
			{
				if (_bitmapValues.ContainsKey(languageName))
				{
					return _bitmapValues[languageName];
				}
			}
			f = this.GetExistFilename(languageName);
			if (!string.IsNullOrEmpty(f))
			{
				Bitmap bp = (Bitmap)Bitmap.FromFile(f);
				if (bp != null)
				{
					if (_bitmapValues == null)
					{
						_bitmapValues = new Dictionary<string, Bitmap>();
					}
					_bitmapValues.Add(languageName, bp);
					return bp;
				}
			}
			return null;
		}
		public override int TreeNodeIconIndex { get { return TreeViewObjectExplorer.IMG_IMAGE; } }
		public override string XmlTag { get { return ProjectResources.XML_IMAGES; } }

		[ReadOnly(true)]
		public override Type ObjectType
		{
			get
			{
				return typeof(Bitmap);
			}
			set
			{

			}
		}
		/// <summary>
		/// do not save it. value saving is done manually in Save function
		/// </summary>
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
						SetCurrentResourceString(f);
						if (string.IsNullOrEmpty(Manager.DesignerLanguageName))
						{
							_bmp = null;
						}
						else
						{
							if (_bitmapValues != null)
							{
								if (_bitmapValues.ContainsKey(Manager.DesignerLanguageName))
								{
									_bitmapValues.Remove(Manager.DesignerLanguageName);
								}
							}
						}
					}
				}
				else
				{
					if (value is Bitmap)
					{
						Bitmap bp = (Bitmap)value;
						SetCurrentResourceString(string.Empty);
						if (string.IsNullOrEmpty(Manager.DesignerLanguageName))
						{
							_bmp = bp;
						}
						else
						{
							if (_bitmapValues == null)
							{
								_bitmapValues = new Dictionary<string, Bitmap>();
							}
							if (_bitmapValues.ContainsKey(Manager.DesignerLanguageName))
							{
								_bitmapValues[Manager.DesignerLanguageName] = bp;
							}
							else
							{
								_bitmapValues.Add(Manager.DesignerLanguageName, bp);
							}
						}
					}
				}
			}
		}
		#endregion
	}
}
