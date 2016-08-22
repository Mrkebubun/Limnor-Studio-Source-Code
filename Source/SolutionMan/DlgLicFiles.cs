/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
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

namespace SolutionMan
{
	public partial class DlgLicFiles : Form
	{
		public Dictionary<string, string[]> Ret = null;
		public DlgLicFiles()
		{
			InitializeComponent();
		}
		public void LoadData(Dictionary<string, string[]> files)
		{
			Ret = files;
			listBox1.Items.Clear();
			listBox2.Items.Clear();
			if (files != null)
			{
				foreach (KeyValuePair<string, string[]> kv in files)
				{
					listBox1.Items.Add(kv.Key);
				}
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				listBox2.Items.Clear();
				if (Ret != null)
				{
					if (n >= 0 && n < listBox1.Items.Count)
					{
						string dll = listBox1.Items[n] as string;
						foreach (KeyValuePair<string, string[]> kv in Ret)
						{
							if (string.Compare(kv.Key, dll, StringComparison.OrdinalIgnoreCase) == 0)
							{
								if (kv.Value != null)
								{
									for (int i = 0; i < kv.Value.Length; i++)
									{
										listBox2.Items.Add(kv.Value[i]);
									}
								}
								break;
							}
						}
					}
				}
			}
		}

		private void buttonAddDLL_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Select Licensed DLL File";
			dlg.CheckFileExists = true;
			dlg.Filter = "DLL files|*.DLL";
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				if (Ret == null)
				{
					Ret = new Dictionary<string, string[]>();
				}
				foreach (KeyValuePair<string, string[]> kv in Ret)
				{
					if (string.Compare(kv.Key, dlg.FileName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						for (int i = 0; i < listBox1.Items.Count; i++)
						{
							string f = listBox1.Items[i] as string;
							if (string.Compare(kv.Key, f, StringComparison.OrdinalIgnoreCase) == 0)
							{
								listBox1.SelectedIndex = i;
								return;
							}
						}
						listBox1.Items.Add(dlg.FileName);
						return;
					}
				}
				Ret.Add(dlg.FileName, new string[] { });
				int k = listBox1.Items.Add(dlg.FileName);
				listBox1.SelectedIndex = k;
			}
		}

		private void buttonDelDLL_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				if (MessageBox.Show(this, "Do you want to delete selected Library file and all its associated licese files from the lists?", "Remove License Files", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
				{
					listBox2.Items.Clear();
					if (Ret != null)
					{
						string s = listBox1.Items[n] as string;
						bool b = true;
						while (b)
						{
							b = false;
							foreach (KeyValuePair<string, string[]> kv in Ret)
							{
								if (string.Compare(kv.Key, s, StringComparison.OrdinalIgnoreCase) == 0)
								{
									b = true;
									Ret.Remove(kv.Key);
									break;
								}
							}
						}
					}
					listBox1.Items.RemoveAt(n);
					if (listBox1.Items.Count > n)
						listBox1.SelectedIndex = n;
					else
						listBox1.SelectedIndex = listBox1.Items.Count - 1;
				}
			}
			else
			{
				MessageBox.Show(this, "Please select a library file from the list", "Remove license file", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void buttonAddLic_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				string s = listBox1.Items[n] as string;
				if (!string.IsNullOrEmpty(s))
				{
					OpenFileDialog dlg = new OpenFileDialog();
					dlg.Title = "Select License File for selected library file";
					if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					{
						bool b = false;
						if (Ret == null)
						{
							Ret = new Dictionary<string, string[]>();
						}
						foreach (KeyValuePair<string, string[]> kv in Ret)
						{
							if (string.Compare(kv.Key, s, StringComparison.OrdinalIgnoreCase) == 0)
							{
								b = true;
								if (kv.Value == null || kv.Value.Length == 0)
								{
									Ret[kv.Key] = new string[] { dlg.FileName };
								}
								else
								{
									bool found = false;
									for (int i = 0; i < kv.Value.Length; i++)
									{
										if (string.Compare(kv.Value[i], dlg.FileName, StringComparison.OrdinalIgnoreCase) == 0)
										{
											found = true;
											break;
										}
									}
									if (!found)
									{
										string[] a = new string[kv.Value.Length+1];
										kv.Value.CopyTo(a,0);
										a[kv.Value.Length] = dlg.FileName;
										Ret[kv.Key] = a;
									}
								}
								break;
							}
						}
						if (!b)
						{
							Ret.Add(s, new string[] {dlg.FileName});
						}
						b = false;
						for (int i = 0; i < listBox2.Items.Count; i++)
						{
							string f = listBox2.Items[i] as string;
							if (string.Compare(f, dlg.FileName, StringComparison.OrdinalIgnoreCase) == 0)
							{
								b = true;
								listBox2.SelectedIndex = i;
								break;
							}
						}
						if (!b)
						{
							int m = listBox2.Items.Add(dlg.FileName);
							listBox2.SelectedIndex = m;
						}
					}
				}
			}
			else
			{
				MessageBox.Show(this, "Please select a library file from left", "Add license file", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void buttonDelLic_Click(object sender, EventArgs e)
		{
			int n0 = listBox1.SelectedIndex;
			if (n0 >= 0)
			{
				string s = listBox1.Items[n0] as string;
				if (!string.IsNullOrEmpty(s))
				{
					int n = listBox2.SelectedIndex;
					if (n >= 0)
					{
						if (MessageBox.Show(this, "Do you want to remove selected license file from the list?", "Remove license file", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
						{
							string f = listBox2.Items[n] as string;
							if (Ret != null)
							{
								foreach (KeyValuePair<string, string[]> kv in Ret)
								{
									if (string.Compare(kv.Key, s, StringComparison.OrdinalIgnoreCase) == 0)
									{
										if (kv.Value != null)
										{
											bool b = false;
											List<string> l = new List<string>();
											for (int i = 0; i < kv.Value.Length; i++)
											{
												if (string.Compare(kv.Value[i], f, StringComparison.OrdinalIgnoreCase) != 0)
												{
													l.Add(kv.Value[i]);
													b = true;
												}
											}
											if (b)
											{
												Ret[kv.Key] = l.ToArray();
											}
										}
										break;
									}
								}
							}
							listBox2.Items.RemoveAt(n);
							if (listBox2.Items.Count > n)
								listBox2.SelectedIndex = n;
							else
								listBox2.SelectedIndex = listBox2.Items.Count - 1;
						}
					}
					else
					{
						MessageBox.Show(this, "Please select a license file from the list", "Remove license file", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
			}
			else
			{
				MessageBox.Show(this, "Please select a library file from left", "Remove license file", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
	}
}
