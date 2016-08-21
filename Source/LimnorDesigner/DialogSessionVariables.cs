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
using System.Text;
using System.Windows.Forms;
using VPL;

namespace LimnorDesigner
{
	public partial class DialogSessionVariables : Form
	{
		public SessionVariable[] SessionVariables;
		private Dictionary<Type, Image> imgs = new Dictionary<Type, Image>();
		private SessionVariableCollection _owner;
		public DialogSessionVariables()
		{
			InitializeComponent();
		}
		public void LoadData(SessionVariableCollection owner)
		{
			_owner = owner;
			for (int i = 0; i < _owner.Count; i++)
			{
				listBox1.Items.Add(_owner[i]);
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
				SessionVariable sv = listBox1.Items[e.Index] as SessionVariable;
				Type t = sv.ValueType;
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
				e.Graphics.DrawString(sv.ToString(), this.Font, Brushes.Black, rc);
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			_owner.Clear();
			SessionVariables = new SessionVariable[listBox1.Items.Count];
			for (int i = 0; i < listBox1.Items.Count; i++)
			{
				SessionVariables[i] = listBox1.Items[i] as SessionVariable;
				_owner.Add(SessionVariables[i]);
			}
			this.DialogResult = DialogResult.OK;
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			DlgSessionVariable dlg = new DlgSessionVariable();
			SessionVariableCollection ow = new SessionVariableCollection(_owner.Owner);
			for (int i = 0; i < listBox1.Items.Count; i++)
			{
				ow.Add(listBox1.Items[i] as SessionVariable);
			}
			dlg.LoadData(ow, null);
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				listBox1.Items.Add(dlg.Return);
			}
		}
		private void buttonDel_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0 && n < listBox1.Items.Count)
			{
				listBox1.Items.RemoveAt(n);
				if (n < listBox1.Items.Count)
				{
					listBox1.SelectedIndex = n;
				}
				else
				{
					listBox1.SelectedIndex = listBox1.Items.Count - 1;
				}
			}
		}
	}
}
