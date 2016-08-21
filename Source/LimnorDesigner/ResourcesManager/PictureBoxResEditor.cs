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
using System.Drawing;
using System.Globalization;
using MathExp;
using WindowsUtility;

namespace LimnorDesigner.ResourcesManager
{
	public class PictureBoxResEditor : PictureBox
	{
		private ResourcePointer _pointer;
		private CultureInfo _culture;
		private bool _loading;
		private bool _readOnly;
		public PictureBoxResEditor()
		{
		}

		private void mnu_selectFile(object sender, EventArgs e)
		{
			if (_pointer.SelectResourceFile(this.FindForm(), CultureName))
			{
				if (_pointer.UsePictureBox)
				{
					this.Image = (Image)_pointer.GetResourceValue(CultureName);
				}
				_pointer.IsChanged = true;
				this.Invalidate();
			}
		}
		private void mnu_clear(object sender, EventArgs e)
		{
			_pointer.ClearResource(CultureName);

			this.Image = null;
			_pointer.IsChanged = true;
			this.Invalidate();
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (!_readOnly && _pointer != null && e.Button == MouseButtons.Right)
			{
				ContextMenu mnu = new ContextMenu();
				List<MenuItem> l = new List<MenuItem>();
				l.Add(new MenuItemWithBitmap("Select file", mnu_selectFile, Resources._dialog.ToBitmap()));
				l.Add(new MenuItemWithBitmap("Clear", mnu_clear, Resources._cancel.ToBitmap()));
				_pointer.AddFileContextMenus(l);
				mnu.MenuItems.AddRange(l.ToArray());
				mnu.Show(this, new Point(e.X, e.Y));
			}
		}
		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			if (_pointer != null)
			{
				_pointer.OnPaintPictureBox(pe, CultureName);
			}
		}
		public string CultureName
		{
			get
			{
				if (_culture != null)
					return _culture.Name;
				return string.Empty;
			}
		}
		public void SetReadOnly(bool readOnly)
		{
			_readOnly = readOnly;
			if (_readOnly)
			{
				this.BackColor = Color.LightGray;
			}
			else
			{
				this.BackColor = Color.White;
			}
		}
		public void SetResourceOwner(ResourcePointer pointer, CultureInfo culture)
		{
			if (!_loading)
			{
				_loading = true;
				_pointer = pointer;
				_culture = culture;

				this.Image = null;
				if (_pointer != null)
				{
					try
					{
						if (_pointer.UsePictureBox)
						{
							object v = _pointer.GetResourceValue(CultureName);
							this.Image = (Image)v;
						}
						else
						{
						}
					}
					catch (Exception err)
					{
						MessageBox.Show(this.FindForm(), err.Message, "Show image", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				_loading = false;
				this.Invalidate();
			}
		}
	}
}
