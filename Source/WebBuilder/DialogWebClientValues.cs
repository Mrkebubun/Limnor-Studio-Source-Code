/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
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
using System.Globalization;
using System.Collections.Specialized;

namespace Limnor.WebBuilder
{
	public partial class DialogWebClientValues : Form
	{
		public WebClientValueCollection Return;
		private Dictionary<Type, Image> imgs = new Dictionary<Type, Image>();
		public DialogWebClientValues()
		{
			InitializeComponent();
		}
		public void LoadData(WebClientValueCollection data, string name)
		{
			Text = string.Format(CultureInfo.InvariantCulture, "Manage cutom values for {0}", name);
			if (data != null)
			{
				foreach (KeyValuePair<string, IJavascriptType> kv in data)
				{
					this.listBox1.Items.Add(new WebClientValue(kv));
				}
				Return = data;
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
				WebClientValue t = listBox1.Items[e.Index] as WebClientValue;
				Image img;
				if (!imgs.TryGetValue(t.ValueType, out img))
				{
					img = VPLUtil.GetTypeIcon(t.ValueType);
					imgs.Add(t.ValueType, img);
				}
				System.Drawing.Rectangle rc = new System.Drawing.Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
				e.Graphics.DrawImage(img, rc.X, rc.Y);
				rc.X += 16;
				if (rc.Width > 16)
				{
					rc.Width -= 16;
				}
				e.Graphics.DrawString(t.ToString(), this.Font, Brushes.Black, rc);
			}
		}

		private void buttonNewMap_Click(object sender, EventArgs e)
		{
			DlgWebClientVariable dlg = new DlgWebClientVariable();
			StringCollection wcs = new StringCollection();
			for (int i = 0; i < listBox1.Items.Count; i++)
			{
				WebClientValue t = listBox1.Items[i] as WebClientValue;
				if (t != null)
				{
					wcs.Add(t.Name);
				}
			}
			dlg.LoadData(wcs, null);
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				listBox1.Items.Add(new WebClientValue(dlg.ReturnName, dlg.ReturnType));
			}
		}

		private void btDel_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				listBox1.Items.RemoveAt(n);
				if (n >= listBox1.Items.Count)
				{
					listBox1.SelectedIndex = listBox1.Items.Count - 1;
				}
				else
				{
					listBox1.SelectedIndex = n;
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (Return == null)
			{
				Return = new WebClientValueCollection(null);
			}
			for (int i = 0; i < listBox1.Items.Count; i++)
			{
				WebClientValue t = listBox1.Items[i] as WebClientValue;
				if (t != null)
				{
					if (!Return.ContainsKey(t.Name))
					{
						Return.Add(t.Name, t.Value);
					}
				}
			}
			this.DialogResult = DialogResult.OK;
		}
	}
	class WebClientValue
	{
		private string _name;
		private IJavascriptType _value;
		public WebClientValue(KeyValuePair<string, IJavascriptType> v)
		{
			_name = v.Key;
			_value = v.Value;
		}
		public WebClientValue(string name, IJavascriptType v)
		{
			_name = name;
			_value = v;
		}
		public string Name
		{
			get
			{
				return _name;
			}
		}
		public IJavascriptType Value
		{
			get
			{
				return _value;
			}
		}
		public Type ValueType
		{
			get
			{
				return _value.GetValueType();
			}
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}({1})", _name, _value.GetValueType().Name);
		}
	}
}
