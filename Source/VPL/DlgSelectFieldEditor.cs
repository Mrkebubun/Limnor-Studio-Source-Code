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
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace VPL
{
	public partial class DlgSelectFieldEditor : DlgSetEditorAttributes
	{
		public DlgSelectFieldEditor()
		{
			InitializeComponent();
		}
		public DlgSelectFieldEditor(DataEditor editor)
			: base(editor)
		{
			InitializeComponent();
		}
		protected virtual Type EditorType { get { return typeof(DataEditor); } }
		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonOK.Enabled = (listBox1.SelectedIndex >= 0);
		}

		private void buttonFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Assembly|*.DLL";
			dlg.Title = "Select Field Editor";
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				listBox1.Items.Clear();
				buttonOK.Enabled = false;
				textBox1.Text = dlg.FileName;
				try
				{
					Assembly a = Assembly.LoadFile(dlg.FileName);
					Type[] tps = a.GetExportedTypes();
					if (tps != null && tps.Length > 0)
					{
						for (int i = 0; i < tps.Length; i++)
						{
							if (!tps[i].IsAbstract && tps[i].IsSubclassOf(EditorType))
							{
								listBox1.Items.Add(tps[i]);
							}
						}
					}
					if (listBox1.Items.Count > 0)
					{
						listBox1.SelectedIndex = 0;
					}
				}
				catch (Exception err)
				{
					MessageBox.Show(err.Message);
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			try
			{
				if (listBox1.SelectedIndex >= 0)
				{
					DataEditor de = (DataEditor)Activator.CreateInstance((Type)(listBox1.Items[listBox1.SelectedIndex]));
					DlgSetEditorAttributes dlg = de.GetDataDialog(SelectedEditor);
					if (dlg != null)
					{
						dlg.SetEditorAttributes(SelectedEditor);
						if (dlg.ShowDialog(this) == DialogResult.OK)
						{
							SetSelection(de);
							this.DialogResult = DialogResult.OK;
						}
					}
					else
					{
						this.DialogResult = DialogResult.OK;
					}
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message);
			}
		}

		public override void SetEditorAttributes(DataEditor current)
		{

		}
	}
}
