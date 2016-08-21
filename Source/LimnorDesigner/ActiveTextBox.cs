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
using MathExp;
using System.Drawing;
using System.Xml;
using XmlUtility;

namespace LimnorDesigner
{
	public class ActiveTextBox : Control
	{
		#region fields and constructors
		private TextBox _txtBox;
		public event EventHandler CancelEdit;
		public event EventHandler FinishEdit;
		public ActiveTextBox()
		{

		}
		#endregion
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			_txtBox.Size = this.Size;
		}
		public void Initialize()
		{
			_txtBox = new TextBox();
			Controls.Add(_txtBox);
			_txtBox.Location = new Point(0, 0);
			_txtBox.Size = this.Size;
			_txtBox.KeyUp += new KeyEventHandler(_txtBox_KeyUp);
			_txtBox.LostFocus += new EventHandler(_txtBox_LostFocus);
		}
		public void SetTextFocus()
		{
			_txtBox.Focus();
		}
		void _txtBox_LostFocus(object sender, EventArgs e)
		{
			if (FinishEdit != null)
			{
				FinishEdit(this, e);
			}
		}

		void _txtBox_KeyUp(object sender, KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (e.KeyCode == Keys.Escape)
			{
				if (CancelEdit != null)
				{
					CancelEdit(this, e);
				}
			}
			else if (e.KeyCode == Keys.Enter)
			{
				if (FinishEdit != null)
				{
					FinishEdit(this, e);
				}
			}
		}
		public string TextBoxText
		{
			get
			{
				return _txtBox.Text;
			}
			set
			{
				_txtBox.Text = value;
			}
		}
	}
}
