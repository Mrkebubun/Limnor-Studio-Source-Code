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
using VSPrj;
using System.Reflection;

namespace LimnorDesigner
{
	public partial class DlgDesigners : Form
	{
		const int IMG_Unchecked = 5;
		const int IMG_Checked = 6;
		Dictionary<string, VisualProgrammingDesigner> _designers;
		public DlgDesigners()
		{
			InitializeComponent();
		}
		public void LoadData(List<Type> builtinDesgners)
		{
			_designers = VisualProgrammingDesigner.LoadDesigners(builtinDesgners);
			listView1.Items.Clear();
			foreach (VisualProgrammingDesigner p in _designers.Values)
			{
				addItem(p);
			}

		}
		private void addItem(VisualProgrammingDesigner p)
		{
			ListViewItem lv = listView1.Items.Add(p.Name);
			lv.Name = p.Name;
			if (p.Enabled)
			{
				lv.ImageIndex = IMG_Checked;
			}
			else
			{
				lv.ImageIndex = IMG_Unchecked;
			}
			lv.SubItems.Add(p.Enabled.ToString());
			lv.SubItems.Add(p.Description);
			lv.SubItems.Add(p.File);
		}
		private bool findInterface(Type t, object v)
		{
			return typeof(IXDesignerViewer).Equals(t);
		}
		private void buttonAdd_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.Title = "Select Visual Programming System";
				dlg.Filter = "*.DLL|*.DLL";
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					string file = dlg.FileName;
					Assembly a = Assembly.LoadFile(file);
					Type[] tps = a.GetExportedTypes();
					if (tps != null && tps.Length > 0)
					{
						for (int i = 0; i < tps.Length; i++)
						{
							Type[] tis = tps[i].FindInterfaces(findInterface, null);
							if (tis != null && tis.Length > 0)
							{
								object[] ats = tps[i].GetCustomAttributes(typeof(DescriptionAttribute), true);
								string desc = "";
								if (ats != null && ats.Length > 0)
								{
									DescriptionAttribute da = (DescriptionAttribute)ats[0];
									desc = da.Description;
								}
								VisualProgrammingDesigner vp = new VisualProgrammingDesigner(tps[i].AssemblyQualifiedName, file, true, desc);
								if (_designers == null)
								{
									_designers = new Dictionary<string, VisualProgrammingDesigner>();
								}
								if (!_designers.ContainsKey(vp.Name))
								{
									_designers.Add(vp.Name, vp);
									addItem(vp);
								}
							}
						}
					}
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message);
			}
		}

		private void buttonEnable_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				if (_designers != null)
				{
					if (_designers.ContainsKey(listView1.SelectedItems[0].Name))
					{
						VisualProgrammingDesigner p = _designers[listView1.SelectedItems[0].Name];
						p.Enabled = !p.Enabled;
						if (p.Enabled)
						{
							listView1.SelectedItems[0].ImageIndex = IMG_Checked;
						}
						else
						{
							listView1.SelectedItems[0].ImageIndex = IMG_Unchecked;
						}
					}
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (_designers != null)
			{
				VisualProgrammingDesigner.SaveDesigners(_designers);
				this.DialogResult = DialogResult.OK;
			}
		}

		private void buttonDel_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				if (_designers != null)
				{
					if (_designers.ContainsKey(listView1.SelectedItems[0].Name))
					{
						if (_designers[listView1.SelectedItems[0].Name].CanRemove)
						{
							_designers.Remove(listView1.SelectedItems[0].Name);
							listView1.Items.Remove(listView1.SelectedItems[0]);
						}
					}
				}
			}
		}
	}
}
