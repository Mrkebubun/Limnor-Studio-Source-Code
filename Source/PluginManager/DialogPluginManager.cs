/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Plugin Manager
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
using System.Globalization;
using System.Reflection;
using XmlUtility;

namespace LimnorVisualProgramming
{
	public partial class DialogPluginManager : Form
	{
		private List<PluginItem> _pluginList;
		private Type _pluginType;
		private string _configFile;
		public DialogPluginManager()
		{
			InitializeComponent();
			_pluginList = new List<PluginItem>();
		}
		public bool LoadData<PluginType>(PluginManager<PluginType> manager, Form caller)
		{
			_pluginType = manager.PlginType;
			if (string.IsNullOrEmpty(manager.PluginConfigurationFilename))
			{
				MessageBox.Show(caller, "PluginConfigurationFilename is empty", "Plugin Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			_configFile = manager.PluginConfigurationFileFullpath;
			lblConfig.Text = _configFile;
			lblPluginType.Text = _pluginType.AssemblyQualifiedName;
			IList<PluginItem> lst = manager.GetAllPluginItems();
			foreach (PluginItem pt in lst)
			{
				listBox1.Items.Add(pt, pt.Enabled);
				_pluginList.Add(pt);
				pt.Modified = false;
			}
			return true;
		}
		internal IList<PluginItem> GetResult()
		{
			return _pluginList;
		}
		private void btOK_Click(object sender, EventArgs e)
		{
			_pluginList = new List<PluginItem>();
			for (int i = 0; i < listBox1.Items.Count; i++)
			{
				_pluginList.Add(listBox1.Items[i] as PluginItem);
			}
			this.DialogResult = DialogResult.OK;
		}

		private void btNew_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = string.Format(CultureInfo.InvariantCulture, "Select DLL file containing {0}", _pluginType.Name);
			dlg.Filter = "Dll files|*.dll";
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				try
				{
					Assembly a = XmlUtil.LoadAssembly(dlg.FileName, false);
					if (a != null)
					{
						bool bFound = false;
						Type[] tps = a.GetExportedTypes();
						if (tps != null)
						{
							for (int i = 0; i < tps.Length; i++)
							{
								if (tps[i].IsInterface)
									continue;
								if (tps[i].IsValueType)
									continue;
								Type t = Type.GetType(tps[i].AssemblyQualifiedName);
								if (_pluginType.IsAssignableFrom(t))
								{
									bFound = true;
									bool bExists = false;
									for (int j = 0; j < listBox1.Items.Count; j++)
									{
										PluginItem pi = listBox1.Items[j] as PluginItem;
										if (pi != null)
										{
											if (pi.PluginItemType.Equals(t))
											{
												bExists = true;
												break;
											}
										}
									}
									if (!bExists)
									{
										PluginItem pi = new PluginItem();
										pi.PluginItemType = t;
										pi.Name = t.FullName;
										listBox1.Items.Add(pi, true);
									}
								}
							}
						}
						if (!bFound)
						{
							MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Plugin type [{0}] does not exist in assembly [{1}]", _pluginType.Name, dlg.FileName), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
					else
					{
						MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Cannot load assembly from [{0}]", dlg.FileName), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				catch (Exception err)
				{
					MessageBox.Show(this, err.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void btDel_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				if (MessageBox.Show(this, "Do you want to remove selected plugin?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					listBox1.Items.RemoveAt(n);
				}
			}
		}
		private void updateList()
		{
			for (int i = 0; i < listBox1.Items.Count; i++)
			{
				PluginItem pt = listBox1.Items[i] as PluginItem;
				if (pt != null)
				{
					if (pt.Modified)
					{
						pt.Modified = false;
						listBox1.Items.RemoveAt(i);
						listBox1.Items.Insert(i, pt);
						listBox1.SetItemChecked(i, pt.Enabled);
					}
				}
			}
		}
		bool _loading;
		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			_loading = true;
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				PluginItem pt = listBox1.Items[n] as PluginItem;
				if (pt != null)
				{
					textBoxName.Text = pt.Name;
				}
			}
			_loading = false;
			timer1.Enabled = true;
		}

		private void textBoxName_TextChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				int n = listBox1.SelectedIndex;
				if (n >= 0)
				{
					PluginItem pt = listBox1.Items[n] as PluginItem;
					if (pt != null)
					{
						pt.Name = textBoxName.Text;
						pt.Modified = true;
					}
				}
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Enabled = false;
			updateList();
		}
	}
}
