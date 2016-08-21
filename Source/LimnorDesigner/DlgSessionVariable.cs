/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Limnor.WebBuilder;
using VPL;
using LimnorDesigner.Web;

namespace LimnorDesigner
{
	public partial class DlgSessionVariable : Form
	{
		private SessionVariableCollection _owner;
		private SessionVariable _defVal;
		private Dictionary<string, Type> _types;
		private Dictionary<Type, Image> imgs = new Dictionary<Type, Image>();
		public SessionVariable Return;
		public string ReturnName;
		public IJavascriptType ReturnType;
		public DlgSessionVariable()
		{
			InitializeComponent();
			_types = WebClientData.GetJavascriptTypes();
			foreach (string s in _types.Keys)
			{
				listBox1.Items.Add(s);
			}
		}
		public void LoadData(SessionVariableCollection owner, SessionVariable defValue)
		{
			_owner = owner;
			_defVal = defValue;
			if (_defVal != null)
			{
				textBox1.Text = _defVal.Name;
			}
		}
		private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();
			if (e.Index >= 0)
			{
				if ((e.State & DrawItemState.Selected) != 0)
				{
					e.DrawFocusRectangle();
				}
				string sName = listBox1.Items[e.Index] as string;
				Type t = _types[sName];
				Image img;
				if (!imgs.TryGetValue(t, out img))
				{
					img = VPLUtil.GetTypeIcon(t);
					imgs.Add(t, img);
				}
				System.Drawing.Rectangle rc = new System.Drawing.Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
				e.Graphics.DrawImage(img, rc.X, rc.Y);
				rc.X += 16;
				if (rc.Width > 16)
				{
					rc.Width -= 16;
				}
				e.Graphics.DrawString(sName, this.Font, Brushes.Black, rc);
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			string nm = textBox1.Text.Trim();
			if (string.IsNullOrEmpty(nm))
			{
				MessageBox.Show(this, "Name cannot be empty", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				bool bExists = false;
				if (_owner != null)
				{
					if (!(_defVal != null && string.Compare(_defVal.Name, nm, StringComparison.OrdinalIgnoreCase) == 0))
					{
						bExists = (_owner[nm] != null);
					}
				}
				if (bExists)
				{
					MessageBox.Show(this, "Name is in use", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else
				{
					Return = new SessionVariable();
					Return.Name = nm;
					Type t;
					if (listBox1.SelectedIndex >= 0)
					{
						t = _types[listBox1.Items[listBox1.SelectedIndex] as string];
					}
					else
					{
						t = typeof(JsString);
					}
					Return.Value = Activator.CreateInstance(t) as IJavascriptType;
					ReturnName = nm;
					ReturnType = Return.Value;
					this.DialogResult = DialogResult.OK;
				}
			}
		}
	}
}
