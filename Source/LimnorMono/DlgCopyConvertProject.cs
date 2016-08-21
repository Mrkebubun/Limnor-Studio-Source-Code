/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */
using LimnorDesigner;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using VSPrj;
using XmlUtility;
using System.Reflection;

namespace LimnorVOB
{
	public partial class DlgCopyConvertProject : Form
	{
		private bool _cancel = false;
		private Thread _thread = null;
		private DataTable _tbl;
		public DlgCopyConvertProject()
		{
			InitializeComponent();
			_tbl = new DataTable("Classes");
			_tbl.Columns.Add(new DataColumn("StatusIcon", typeof(Image)));
			_tbl.Columns.Add(new DataColumn("ClassFile"));
			_tbl.Columns.Add(new DataColumn("StatusText"));
			_tbl.Columns.Add(new DataColumn("ConversionMessage"));
			dataGridView1.Columns.Clear();
			dataGridView1.AutoGenerateColumns = true;
			dataGridView1.DataSource = _tbl;
			dataGridView1.Columns[0].HeaderText = " ";
			dataGridView1.Columns[1].HeaderText = "Class file";
			dataGridView1.Columns[2].HeaderText = "Status";
			dataGridView1.Columns[3].HeaderText = "Message";
			dataGridView1.Columns[0].Width = 20;
			dataGridView1.Columns[1].Width = 180;
			dataGridView1.Columns[2].Width = 80;
			dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		}

		private void btBrowse_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Select Limnor Studio Project";
			dlg.CheckFileExists = true;
			dlg.Filter = "Project files|*.lrproj";
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				lblSourceProject.Text = dlg.FileName;
				btStart.Enabled = (lblTargetFolder.Text.Length > 0);
			}
		}

		private void btTargetFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.Description = "Select a folder to copy the project to";
			dlg.ShowNewFolderButton = true;
			while (true)
			{
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					string[] ss = Directory.GetFiles(dlg.SelectedPath);
					if (ss != null && ss.Length > 0)
					{
						if (MessageBox.Show(this, "The 'Copy to folder' is not empty. Existing files will be overwritten. Do you want to continue?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
						{
							continue;
						}
					}
					lblTargetFolder.Text = dlg.SelectedPath;
					btStart.Enabled = (lblSourceProject.Text.Length > 0);
				}
				break;
			}
		}
		private void showErrMsgBox(string err)
		{
			if (this.InvokeRequired)
			{
				this.Invoke((MethodInvoker)(delegate()
				{
					MessageBox.Show(this, err, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}));
			}
			else
			{
				MessageBox.Show(this, err, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void btStart_Click(object sender, EventArgs e)
		{
			btStart.Enabled = false;
			btTargetFolder.Enabled = false;
			btBrowse.Enabled = false;
			if (lblSourceProject.Text.Length == 0)
			{
				showErrMsgBox("Source project is not selected");
			}
			else if (lblTargetFolder.Text.Length == 0)
			{
				showErrMsgBox("'Copy to folder' is not selected");
			}
			else if (!File.Exists(lblSourceProject.Text))
			{
				showErrMsgBox("Source project file does not exist");
			}
			else if (!Directory.Exists(lblTargetFolder.Text))
			{
				showErrMsgBox("'Copy to folder' does not exist");
			}
			else
			{
				bool bOK = false;
				string[] ss = Directory.GetFiles(lblTargetFolder.Text);
				if (ss != null && ss.Length > 0)
				{
					if (MessageBox.Show(this, "The 'Copy to folder' is not empty. Existing files will be overwritten. Do you want to continue?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
					{
						bOK = true;
					}
				}
				else
				{
					bOK = true;
				}
				if (bOK)
				{
					_cancel = false;
					btCancel.Enabled = true;
					lblInfo.Text = "Starting...";
					lblInfo.Refresh();
					_tbl.Rows.Clear();
					dataGridView1.Refresh();
					_thread = new Thread(copyConvert);
					_thread.Start();
					return;
				}
			}
			btStart.Enabled = true;
			btTargetFolder.Enabled = true;
			btBrowse.Enabled = true;
			btCancel.Enabled = false;
		}
		private void showinfo(string info)
		{
			this.Invoke((MethodInvoker)(delegate()
			{
				lblInfo.Text = info;
				lblInfo.Refresh();
			}));
		}
		private void showclass(ComponentID c)
		{
			this.Invoke((MethodInvoker)(delegate()
			{
				_tbl.Rows.Add(Resource1._file.ToBitmap(), Path.GetFileName(c.ComponentFile), "Not verified", ""); 
			}));
		}
		private void processclass(ComponentID c, int idx)
		{
			bool bOK = false;
			string msg = string.Empty;
			if (File.Exists(c.ComponentFile))
			{
				XmlDocument xml = new XmlDocument();
				xml.Load(c.ComponentFile);
				if (xml.DocumentElement != null)
				{
					bOK = true;
					XmlNodeList typeNodes = xml.DocumentElement.SelectNodes("Types/Item");
					if (typeNodes != null && typeNodes.Count > 0)
					{
						bool bModified = false;
						StringBuilder sb = new StringBuilder();
						foreach (XmlNode nd in typeNodes)
						{
							string stype = XmlUtil.GetAttribute(nd, "fullTypeName");
							if (!string.IsNullOrEmpty(stype))
							{
								bool b = false;
								Type t;
								try
								{
									t = Type.GetType(stype);
									if (t != null)
									{
										b = true;
									}
								}
								catch
								{
								}
								if (!b)
								{
									int k = stype.IndexOf("Version=", StringComparison.Ordinal);
									if (k > 0)
									{
										string stype2 = stype.Substring(0, k).Trim();
										if (stype2.EndsWith(","))
										{
											stype2 = stype2.Substring(0, stype2.Length - 1);
										}
										if (!string.IsNullOrEmpty(stype2))
										{
											try
											{
												t = Type.GetType(stype2);
												if (t != null)
												{
													b = true;
													bModified = true;
													XmlUtil.SetAttribute(nd, "fullTypeName", t.AssemblyQualifiedName);
												}
											}
											catch
											{
											}
											if (!b)
											{
												k = stype2.IndexOf(',');
												if (k > 0)
												{
													stype2 = stype2.Substring(0, k).Trim();
													try
													{
														t = Type.GetType(stype2);
														if (t != null)
														{
															b = true;
															bModified = true;
															XmlUtil.SetAttribute(nd, "fullTypeName", t.AssemblyQualifiedName);
														}
													}
													catch
													{
													
													}
												}
											}
										}
									}
								}
								if (!b)
								{
									string so;
									XmlUtil.DisableTypePathAdjust = true;
									t = XmlUtil.GetLibTypeAttribute(nd, out so);
									if (t != null)
									{
										b = true;
									}
									XmlUtil.DisableTypePathAdjust = false;
								}
								if (!b)
								{
									int k = stype.IndexOf("Version=", StringComparison.Ordinal);
									if (k > 0)
									{
										if (stype.IndexOf(',', k) > 0)
										{
											this.Invoke((MethodInvoker)(delegate()
											{
											t = DlgChangeTypeVersion.SelectTypeVersion(this, stype);
											if (t != null)
											{
												b = true;
												bModified = true;
												XmlUtil.SetAttribute(nd, "fullTypeName", t.AssemblyQualifiedName);
											}
											}));
										}
									}
								}
								if (!b)
								{
									bOK = false;
									sb.Append(stype);
									sb.Append("\r\n");
								}
							}
						}
						if (!bOK)
						{
							msg = string.Format(CultureInfo.InvariantCulture, "Following classes cannot be loaded:\r\n{0}", sb.ToString());
						}
						else if (bModified)
						{
							xml.Save(c.ComponentFile);
						}
					}
				}
				else
				{
					msg = string.Format(CultureInfo.InvariantCulture, "Invalid class file:{0}", c.ComponentFile);
				}
			}
			else
			{
				msg = string.Format(CultureInfo.InvariantCulture, "File not found:{0}", c.ComponentFile);
			}
			if (bOK)
			{
				this.Invoke((MethodInvoker)(delegate()
				{
					_tbl.Rows[idx][0] = Resource1._ok.ToBitmap();
					_tbl.Rows[idx][2] = "OK";
					_tbl.Rows[idx][3] = "";
				}));
			}
			else
			{
				this.Invoke((MethodInvoker)(delegate()
				{
					_tbl.Rows[idx][0] = Resource1._cancel.ToBitmap();
					_tbl.Rows[idx][2] = "Error";
					_tbl.Rows[idx][3] = msg;
				}));
			}
		}
		private void copyConvert()
		{
			try
			{
				string sprj = lblSourceProject.Text;
				string sdir = lblTargetFolder.Text;
				string srcDir = Path.GetDirectoryName(sprj);
				string[] files = Directory.GetFiles(srcDir);
				if (files != null && files.Length > 0)
				{
					for (int i = 0; i < files.Length; i++)
					{
						string f = Path.GetFileName(files[i]);
						showinfo(string.Format(CultureInfo.InvariantCulture, "Copying {0}", f));
						File.Copy(files[i], Path.Combine(sdir, f), true);
					}
				}
				LimnorProject prj = new LimnorProject(Path.Combine(sdir, Path.GetFileName(sprj)));
				IList<ComponentID> cs = prj.GetAllComponents();
				if (cs == null)
				{
					showinfo("Invalid project file");
				}
				else
				{
					if (cs.Count == 0)
					{
						showinfo("The project is empty");
					}
					else
					{
						foreach (ComponentID cid in cs)
						{
							showclass(cid);
						}
						if (_cancel)
						{
							showinfo("Canceled");
						}
						else
						{
							int idx = 0;
							foreach (ComponentID cid in cs)
							{
								if (_cancel)
								{
									showinfo("Canceled");
									break;
								}
								else
								{
									showinfo(string.Format(CultureInfo.InvariantCulture, "Verifying {0}", cid.ComponentName));
									processclass(cid, idx);
									idx++;
								}
							}
							if (!_cancel)
							{
								showinfo("Completed");
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				showinfo(e.Message);
				showErrMsgBox(e.Message);
			}
			finally
			{
				this.Invoke((MethodInvoker)(delegate()
				{
					btStart.Enabled = true;
					btTargetFolder.Enabled = true;
					btBrowse.Enabled = true;
					btCancel.Enabled = false;
				}));
			}
		}

		private void btCancel_Click(object sender, EventArgs e)
		{
			_cancel = true;
		}

		private void btShowMsg_Click(object sender, EventArgs e)
		{
			if (dataGridView1.CurrentRow != null)
			{
				object v = dataGridView1.CurrentRow.Cells[3].Value;
				if (v != null)
				{
					string s = v.ToString();
					FormWarning.ShowMessageDialog(this, s);
				}
			}
		}
	}
}
