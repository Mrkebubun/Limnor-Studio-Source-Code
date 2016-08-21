/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LimnorVOB
{
	public partial class DlgInsertIcons : Form
	{
		public DlgInsertIcons()
		{
			InitializeComponent();
		}

		private void btExe_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Select Executable File";
			dlg.CheckFileExists = true;
			dlg.Filter = "Executable file|*.exe";
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				txtExe.Text = dlg.FileName;
			}
		}

		private void btSelIcon_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Select Icon File";
			dlg.CheckFileExists = true;
			dlg.Filter = "Icon file|*.ico";
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				listBox1.Items.Add(new IconFile(dlg.FileName));
			}
		}

		private void btDelIcon_Click(object sender, EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				listBox1.Items.RemoveAt(n);
				if (n < listBox1.Items.Count)
				{
					listBox1.SelectedIndex = n;
				}
			}
		}

		private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();
			if (e.Index >= 0)
			{
				IconFile iid = listBox1.Items[e.Index] as IconFile;
				if (iid.IconImage != null)
				{
					e.Graphics.DrawImage(iid.IconImage, new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, 16, 16));
				}
				e.Graphics.DrawString(iid.FileName, this.Font, Brushes.Black, (float)(e.Bounds.X + 20), (float)(e.Bounds.Y + 2));
				if ((e.State & DrawItemState.Selected) != 0)
				{
					e.DrawFocusRectangle();
				}
			}
		}

		private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemHeight = 30;
			e.ItemWidth = listBox1.Width;
		}
		class IconFile
		{
			private string _file;
			private Bitmap _bmp;
			private Icon _ico;
			public IconFile(string file)
			{
				_file = file;
			}
			public string FileName
			{
				get
				{
					return _file;
				}
			}
			public Bitmap IconImage
			{
				get
				{
					if (_bmp == null)
					{
						if (_ico == null)
						{
							if (!string.IsNullOrEmpty(_file))
							{
								try
								{
									_ico = Icon.ExtractAssociatedIcon(_file);
								}
								catch
								{
								}
							}
						}
						if (_ico != null)
						{
							_bmp = _ico.ToBitmap();
						}
					}
					return _bmp;
				}
			}
			public override string ToString()
			{
				return _file;
			}
		}

		private void btInsertIcons_Click(object sender, EventArgs e)
		{
			btInsertIcons.Enabled = false;
			string sExe = txtExe.Text.Trim();
			if (sExe.Length == 0)
			{
				MessageBox.Show(this, "Executable file not selected", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				if (File.Exists(sExe))
				{
					if (listBox1.Items.Count > 0)
					{
						bool bOK = true;
						StringBuilder sb = new StringBuilder();
						sb.Append("\"");
						sb.Append(sExe);
						sb.Append("\" \"");
						for (int i = 0; i < listBox1.Items.Count; i++)
						{
							IconFile f = listBox1.Items[i] as IconFile;
							if (f != null)
							{
								if (File.Exists(f.FileName))
								{
									if (i > 0)
										sb.Append(";");
									sb.Append(f.FileName);
								}
								else
								{
									bOK = false;
									MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Icon file does not exist:{0}", f.FileName), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
									break;
								}
							}
						}
						if (bOK)
						{
							sb.Append("\"");
							string uexe = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "InsertIcons.exe");
							if (File.Exists(uexe))
							{
								try
								{
									//
									Process p = new Process();
									//
									ProcessStartInfo psI = new ProcessStartInfo("cmd");
									psI.UseShellExecute = false;
									psI.RedirectStandardInput = false;
									psI.RedirectStandardOutput = true;
									psI.RedirectStandardError = true;
									psI.CreateNoWindow = true;
									p.StartInfo = psI;
									p.StartInfo.FileName = uexe;
									p.StartInfo.Arguments = sb.ToString();
									//
									string stdout;
									string errout;
									p.Start();
									stdout = p.StandardOutput.ReadToEnd();
									errout = p.StandardError.ReadToEnd();
									p.WaitForExit();
									if (p.ExitCode != 0)
									{
										string errmsg = string.Format(CultureInfo.InvariantCulture, "Error code {0}, output:{1}, error:{2} for calling {3} {4}", p.ExitCode, stdout, errout, p.StartInfo.FileName, p.StartInfo.Arguments);
										MessageBox.Show(this, errmsg, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
									}
									else
									{
										MessageBox.Show(this, "Icon(s) inserted.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
									}
								}
								catch (Exception err)
								{
									MessageBox.Show(this, err.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
								}
							}
							else
							{
								MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Utility file does not exist:{0}", uexe), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
						}
					}
					else
					{
						MessageBox.Show(this, "Icon file list is empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				else
				{
					MessageBox.Show(this, "Executable file does not exist", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			btInsertIcons.Enabled = true;
		}
	}
}
