/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Components for Windows Form
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace FormComponents
{
	[ToolboxBitmapAttribute(typeof(ButtonKey), "Resources.checkedList.bmp")]
	[Description("A list box with a checkbox on each item. It allows making Indeterminate not changeable. It allows displaying Indeterminate as checked or unchecked")]
	public class CheckedListBoxEx : CheckedListBox
	{
		private bool _indeterminateChangeable;
		private bool _showIndeterminateAsChecked;
		public CheckedListBoxEx()
		{
			this.DrawMode = DrawMode.OwnerDrawFixed;
		}
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether the user may change the Indeterminate status of items to other status")]
		public bool IsIndeterminateChangeable
		{
			get
			{
				return _indeterminateChangeable;
			}
			set
			{
				_indeterminateChangeable = value;
			}
		}
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether an item in Indeterminate status should be displayed as Checked")]
		public bool ShowIndeterminateAsChecked
		{
			get
			{
				return _showIndeterminateAsChecked;
			}
			set
			{
				_showIndeterminateAsChecked = value;
			}
		}
		protected override void OnItemCheck(ItemCheckEventArgs ice)
		{
			if (!_indeterminateChangeable)
			{
				if (ice.CurrentValue == CheckState.Indeterminate)
				{
					ice.NewValue = CheckState.Indeterminate;
				}
			}
			base.OnItemCheck(ice);
			this.Invalidate();
		}
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			int boxSize = 13;
			int textStart = 16;
			Rectangle contentRect = e.Bounds;
			Rectangle textRect = e.Bounds;
			CheckState chk = CheckState.Unchecked;
			string msg;
			if (e.Index >= 0 && e.Index < Items.Count)
			{
				chk = this.GetItemCheckState(e.Index);
				if (Items[e.Index] != null)
				{
					msg = Items[e.Index].ToString();
				}
				else
				{
					msg = string.Empty;
				}
			}
			else
			{
				msg = string.Empty;
			}
			//draw checkbox===================================
			Rectangle box = contentRect;
			box.Width = boxSize;
			box.Height = boxSize;
			int n;
			if (contentRect.Height > boxSize)
			{
				n = (contentRect.Height - boxSize) / 2;
			}
			else
			{
				n = 0;
			}
			box.Y += n;
			box.X = 2;


			if (chk == CheckState.Checked)
			{
				e.Graphics.DrawImage(Resource1._checked, box);
			}
			else if (chk == CheckState.Indeterminate)
			{
				if (_showIndeterminateAsChecked)
				{
					e.Graphics.DrawImage(Resource1._checkedShaded, box);
				}
				else
				{
					e.Graphics.DrawImage(Resource1._uncheckedShaded, box);
				}
			}
			else
			{
				e.Graphics.DrawImage(Resource1.chkbox, box);
			}
			//==draw text==========================================
			textRect.X = textRect.X + textStart;
			if (textRect.Width > textStart)
			{
				textRect.Width = textRect.Width - textStart;
			}
			contentRect.X = contentRect.X + textStart + 1;
			if (contentRect.Width > textStart + 1)
			{
				contentRect.Width = contentRect.Width - textStart - 1;
			}
			contentRect.Y = contentRect.Y + 1;
			if (contentRect.Height > 2)
			{
				contentRect.Height = contentRect.Height - 2;
			}
			e.Graphics.FillRectangle(new SolidBrush(this.BackColor), textRect);
			if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
			{
				e.Graphics.DrawRectangle(Pens.LightBlue, contentRect);
				e.Graphics.FillRectangle(new SolidBrush(Color.LightBlue), contentRect);
			}
			e.Graphics.DrawString(msg,
				e.Font,
				new SolidBrush(this.ForeColor),
				contentRect);

		}
	}
}
