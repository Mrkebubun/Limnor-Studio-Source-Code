/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
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
using System.Collections.Specialized;

namespace VPL
{
	public partial class DlgNewTypedNamedValue : Form
	{
		public Type DataType;
		public string DataName;
		private StringCollection _names;
		private string _oldName;
		private bool _validateName;
		private bool _useDefaultTypes;
		private Dictionary<string, Type> _types;
		private Dictionary<Type, Image> imgs;
		public DlgNewTypedNamedValue()
		{
			InitializeComponent();
			listBox1.DrawMode = DrawMode.OwnerDrawVariable;
			listBox1.SelectedIndex = 0;
		}
		public void LoadData(StringCollection names, string title, TypedNamedValue defaultValue, Dictionary<string, Type> types, bool validateName)
		{
			_validateName = validateName;
			_useDefaultTypes = false;
			_names = names;
			imgs = new Dictionary<Type, Image>();
			listBox1.Items.Clear();
			_types = types;
			foreach (string s in types.Keys)
			{
				listBox1.Items.Add(s);
			}
			if (!string.IsNullOrEmpty(title))
			{
				this.Text = title;
			}
			if (defaultValue != null)
			{
				_oldName = defaultValue.Name;
				textBox1.Text = defaultValue.Name;
				for (int i = 0; i < listBox1.Items.Count; i++)
				{
					Type t = _types[listBox1.Items[i] as string];
					if (t.Equals(defaultValue.Value.ValueType))
					{
						listBox1.SelectedIndex = i;
						break;
					}
				}

			}
			if (listBox1.SelectedIndex < 0)
			{
				listBox1.SelectedIndex = 0;
			}
		}
		public void LoadData(StringCollection names, string title, TypedNamedValue defaultValue)
		{
			_validateName = true;
			_useDefaultTypes = true;
			_names = names;
			imgs = null;
			if (!string.IsNullOrEmpty(title))
			{
				this.Text = title;
			}
			if (defaultValue != null)
			{
				_oldName = defaultValue.Name;
				textBox1.Text = defaultValue.Name;
				if (typeof(string).Equals(defaultValue.Value.ValueType))
				{
					listBox1.SelectedIndex = 0;
				}
				else if (typeof(Int32).Equals(defaultValue.Value.ValueType))
				{
					listBox1.SelectedIndex = 1;
				}
				else if (typeof(double).Equals(defaultValue.Value.ValueType))
				{
					listBox1.SelectedIndex = 2;
				}
				else if (typeof(bool).Equals(defaultValue.Value.ValueType))
				{
					listBox1.SelectedIndex = 3;
				}
				else if (typeof(DateTime).Equals(defaultValue.Value.ValueType))
				{
					listBox1.SelectedIndex = 4;
				}
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
				System.Drawing.Rectangle rc = new System.Drawing.Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
				Image img;
				string txt = listBox1.Items[e.Index] as string;
				if (_useDefaultTypes)
				{
					img = imageList1.Images[e.Index];
				}
				else
				{
					Type t = _types[txt];
					if (!imgs.TryGetValue(t, out img))
					{
						img = VPLUtil.GetTypeIcon(t);
						imgs.Add(t, img);
					}
				}
				e.Graphics.DrawImage(img, rc.X, rc.Y);
				rc.X += 16;
				if (rc.Width > 16)
				{
					rc.Width -= 16;
				}
				e.Graphics.DrawString(txt, this.Font, Brushes.Black, rc);
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			DataName = textBox1.Text.Trim();
			if (string.IsNullOrEmpty(DataName))
			{
				MessageBox.Show(this, "Value name cannot be empty", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				if (_validateName && !NameChangeEventArgs.IsValidName(DataName))
				{
					MessageBox.Show(this, Resource1.InvalidName, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else
				{
					if (string.Compare(_oldName, DataName, StringComparison.OrdinalIgnoreCase) != 0)
					{
						if (_names != null)
						{
							for (int i = 0; i < _names.Count; i++)
							{
								if (string.Compare(_names[i], DataName, StringComparison.OrdinalIgnoreCase) == 0)
								{
									MessageBox.Show(this, "Value name is in use", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
									return;
								}
							}
						}
					}
					int n = listBox1.SelectedIndex;
					if (n >= 0)
					{
						if (_useDefaultTypes)
						{
							switch (n)
							{
								case 0:
									DataType = typeof(string);
									break;
								case 1:
									DataType = typeof(Int32);
									break;
								case 2:
									DataType = typeof(double);
									break;
								case 3:
									DataType = typeof(bool);
									break;
								case 4:
									DataType = typeof(DateTime);
									break;
								default:
									DataType = typeof(string);
									break;
							}
						}
						else
						{
							DataType = _types[listBox1.Items[n] as string];
						}
						this.DialogResult = DialogResult.OK;
					}
				}
			}
		}

		private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			if (e.Index >= 0 && e.Index < listBox1.Items.Count)
			{
				string txt = listBox1.Items[e.Index] as string;
				SizeF size = e.Graphics.MeasureString(txt, listBox1.Font);
				e.ItemHeight = (int)size.Height + 2;
			}
		}
	}
}
