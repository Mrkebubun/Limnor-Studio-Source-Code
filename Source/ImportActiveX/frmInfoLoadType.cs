/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	ActiveX Import Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Diagnostics;

namespace PerformerImport
{
	/// <summary>
	/// Summary description for frmInfoLoadType.
	/// </summary>
	public class frmInfoLoadType : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.Button btCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		bool bCancel = false;
		public frmInfoLoadType()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.lblInfo = new System.Windows.Forms.Label();
			this.btCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this.label1.Location = new System.Drawing.Point(40, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(328, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Loading COM objects, please wait...";
			// 
			// lblInfo
			// 
			this.lblInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblInfo.Location = new System.Drawing.Point(40, 56);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(344, 56);
			this.lblInfo.TabIndex = 1;
			// 
			// btCancel
			// 
			this.btCancel.Location = new System.Drawing.Point(312, 128);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 2;
			this.btCancel.Text = "&Stop";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// frmInfoLoadType
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(424, 168);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "frmInfoLoadType";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "frmInfoLoadType";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadTypes(System.Windows.Forms.ListBox listBox1)
		{
			bool bAbort = false;
			bool bSkip = false;
			int n;
			string sExt;
			listBox1.Visible = false;
			Microsoft.Win32.RegistryKey keyTypeLib = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("TypeLib", false);
			bCancel = false;
			if (keyTypeLib == null)
			{
				MessageBox.Show(this, "Cannot find registry TypeLib", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				bool bTryRegsvr = false;
				bSkip = true; //not to confuse the amerturers
				//LoadActiveXInfoException
				List<Type> skipTypes = new List<Type>();
				string[] types = keyTypeLib.GetSubKeyNames();
				if (types != null)
				{
					for (int i = 0; i < types.Length; i++)
					{
						Application.DoEvents();
						if (bAbort)
							break;
						Microsoft.Win32.RegistryKey keyType = keyTypeLib.OpenSubKey(types[i], false);
						if (bCancel)
							break;
						if (keyType != null)
						{
							string[] vers = keyType.GetSubKeyNames();
							if (vers != null)
							{
								for (int j = 0; j < vers.Length; j++)
								{
									Application.DoEvents();
									if (bAbort)
										break;
									short MainVer = 0;
									short MinVer = 0;
									try
									{
										n = vers[j].IndexOf('.');
										if (n == 0)
										{
											string s = vers[j].Substring(1);
											if (!string.IsNullOrEmpty(s))
											{
												if (!short.TryParse(s, out MinVer))
												{
													if (!short.TryParse(s, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out MinVer))
													{
														throw new LoadActiveXInfoException("Invalid MinVer:" + s);
													}
												}
											}
										}
										else if (n < 0)
										{
											if (!string.IsNullOrEmpty(vers[j]))
											{
												if (!short.TryParse(vers[j], out MainVer))
												{
													if (!short.TryParse(vers[j], System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out MainVer))
													{
														throw new LoadActiveXInfoException("Invalid MinVer:" + vers[j]);
													}
												}
											}
										}
										else
										{
											string s = vers[j].Substring(0, n);
											if (!short.TryParse(s, out MainVer))
											{
												if (!short.TryParse(s, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out MainVer))
												{
													throw new LoadActiveXInfoException("Invalid MinVer:" + s);
												}
											}
											s = vers[j].Substring(n + 1);
											if (!short.TryParse(s, out MinVer))
											{
												if (!short.TryParse(s, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out MinVer))
												{
													throw new LoadActiveXInfoException("Invalid MinVer:" + s);
												}
											}
										}
									}
									catch (Exception err)
									{
										if (!bSkip)
										{
											if (!skipTypes.Contains(err.GetType()))
											{
												FormError dlg = new FormError();
												dlg.SetMessage(LoadActiveXInfoException.FormExceptionText(err));
												DialogResult ret = dlg.ShowDialog(this);
												if (ret == DialogResult.Ignore)
												{
													bSkip = true;
												}
												else if (ret == DialogResult.Abort)
												{
													bAbort = true;
												}
												else if (ret == DialogResult.Cancel)
												{
													skipTypes.Add(err.GetType());
												}
											}
										}
									}
									Microsoft.Win32.RegistryKey keyVer = keyType.OpenSubKey(vers[j], false);
									if (bCancel)
										break;
									if (keyVer != null)
									{
										string[] LCIDs = keyVer.GetSubKeyNames();
										if (LCIDs != null)
										{
											for (int k = 0; k < LCIDs.Length; k++)
											{
												if (bAbort)
													break;
												TypeRec rec = null;
												try
												{
													rec = new TypeRec();
												}
												catch (Exception eRec)
												{
													bAbort = true;
													bTryRegsvr = true;
													MessageBox.Show(this, eRec.Message, "Search ActiveX Libraries", MessageBoxButtons.OK, MessageBoxIcon.Error);
												}
												if (rec != null && !bAbort)
												{
													try
													{
														rec.GUID = types[i];
														rec.MainVer = MainVer;
														rec.MinVer = MinVer;
														if (!string.IsNullOrEmpty(LCIDs[k]))
														{
															int cid;
															if (int.TryParse(LCIDs[k], out cid))
															{
																rec.LCID = cid;
																rec.LoadInfo();
																sExt = System.IO.Path.GetExtension(rec.File);
																sExt = sExt.ToLowerInvariant();
																if (string.Compare(sExt, ".dll", StringComparison.OrdinalIgnoreCase) == 0)
																{
																	listBox1.Items.Add(rec);
																}
																lblInfo.Text = rec.ToString();
																Application.DoEvents();
																Application.DoEvents();
															}
														}
													}
													catch (ExceptionTLI etli)
													{
														MessageBox.Show(this, etli.Message, "Search ActiveX Librarues", MessageBoxButtons.OK, MessageBoxIcon.Error);
														bAbort = true;
														bTryRegsvr = true;
													}
													catch (Exception err)
													{
														if (!bSkip)
														{
															if (!skipTypes.Contains(err.GetType()))
															{
																FormError dlg = new FormError();
																dlg.SetMessage(LoadActiveXInfoException.FormExceptionText(err));
																DialogResult ret = dlg.ShowDialog(this);
																if (ret == DialogResult.Ignore)
																{
																	bSkip = true;
																}
																else if (ret == DialogResult.Abort)
																{
																	bAbort = true;
																}
																else if (ret == DialogResult.Cancel)
																{
																	skipTypes.Add(err.GetType());
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
							keyType.Close();
						}
					}
				}
				keyTypeLib.Close();
				if (bTryRegsvr)
				{

					string tliFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "TLBINF32.DLL");
					if (File.Exists(tliFile))
					{
						if (MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Limnor Studio is trying to fix the problem by execyting regsvr32 \"{0}\"\r\n After executing the command, you may try importing ActiveX again.\r\n\r\nDo you want to continue?", tliFile), "Search ActiveX Libraries", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
						{
							string stdout;
							string errout;
							Process p = new Process();
							ProcessStartInfo psI = new ProcessStartInfo("cmd");
							psI.UseShellExecute = false;
							psI.RedirectStandardInput = false;
							psI.RedirectStandardOutput = true;
							psI.RedirectStandardError = true;
							psI.CreateNoWindow = true;
							p.StartInfo = psI;
							p.StartInfo.FileName = "regsvr32";
							p.StartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", tliFile);
							p.Start();

							stdout = p.StandardOutput.ReadToEnd();
							errout = p.StandardError.ReadToEnd();

							p.WaitForExit();
							if (p.ExitCode != 0)
							{
								MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "regsvr32 failed. Error code {0}, output:{1}, error:{2}", p.ExitCode, stdout, errout), "Search ActiveX Libraries", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
							else
							{
								MessageBox.Show(this, "TLBINF32.DLL is registered. You may try to import ActiveX libraries again", "Search ActiveX Libraries", MessageBoxButtons.OK, MessageBoxIcon.Information);
							}
						}
					}
					else
					{
						MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "File not found:{0}", tliFile), "Search ActiveX Libraries", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			listBox1.Visible = true;
		}
		private void btCancel_Click(object sender, System.EventArgs e)
		{
			bCancel = true;
		}
	}
}
