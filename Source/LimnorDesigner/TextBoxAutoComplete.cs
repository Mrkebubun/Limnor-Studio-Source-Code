/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LimnorDesigner
{
	public class TextBoxAutoComplete : TextBox
	{
		private bool bReseting;
		private Dictionary<char, EventHandler> _searchDelimiter;
		public TextBoxAutoComplete()
		{
		}
		public bool Resetting
		{
			get
			{
				return bReseting;
			}
		}
		public void SetDelimiterSearch(char k, EventHandler h)
		{
			if (_searchDelimiter == null)
			{
				_searchDelimiter = new Dictionary<char, EventHandler>();
			}
			if (_searchDelimiter.ContainsKey(k))
			{
				if (h == null)
				{
					_searchDelimiter.Remove(k);
				}
				else
				{
					_searchDelimiter[k] = h;
				}
			}
			else
			{
				if (h != null)
				{
					_searchDelimiter.Add(k, h);
				}
			}
		}
		public void SetText(string text)
		{
			if (bReseting)
				return;
			bReseting = true;
			int n = Text.Length;
			Text = text;
			SelectionStart = n;
			SelectionLength = text.Length - n;
			bReseting = false;
		}
		public void SetText(string text, int selectStart)
		{
			if (bReseting)
				return;
			bReseting = true;
			selectStart++;
			Text = text;
			SelectionStart = selectStart;
			SelectionLength = text.Length - selectStart;
			bReseting = false;
		}
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
			{
				bReseting = true;
			}
			else
			{

			}
			base.OnKeyDown(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (_searchDelimiter != null)
			{
				EventHandler h;
				if (_searchDelimiter.TryGetValue(e.KeyChar, out h))
				{
					e.Handled = true;
					h(this, e);
					return;
				}
			}
			base.OnKeyPress(e);
		}
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			bReseting = false;
		}
	}
}
