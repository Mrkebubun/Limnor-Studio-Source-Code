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
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using Microsoft.CSharp;
using VPL;
using LimnorDesigner;
using System.Reflection;
using XmlUtility;

namespace LimnorDesigner.Remoting
{
	public partial class DlgAddWebService : Form
	{
		private WebServiceProxy _proxy;
		private ILimnorDesignPane _designPane;
		private string _folder;
		private string _namespace;
		private bool _filesFixed;
		public DlgAddWebService()
		{
			InitializeComponent();
		}
		public void SetData(ILimnorDesignPane data)
		{
			_designPane = data;
			_folder = _designPane.Loader.Project.ProjectFolder;
			_namespace = _designPane.Loader.GetRootId().Namespace;
			if (string.IsNullOrEmpty(_namespace))
			{
				_namespace = _designPane.Loader.Namespace;
			}
		}
		public void SetData(string folder, string defaultNamespace)
		{
			_folder = folder;
			_namespace = defaultNamespace;
			_designPane = null;
		}
		internal void SetFixedAsmxUrl(AsmxURL asmx)
		{
			textBoxAsmx.Text = asmx.AsmxUrl;
			textBoxAsmx.ReadOnly = true;
			textBoxDll.Text = asmx.DllFile;
			textBoxDll.ReadOnly = true;
			textBoxWsdl.Text = asmx.Wsdl;
			textBoxWsdl.ReadOnly = true;
			textBoxProxy.Text = asmx.Proxy;
			textBoxProxy.ReadOnly = true;
			rdbAsmx.Checked = true;
			rdbWsdl.Enabled = false;
			rdbAsmx.Enabled = false;
			buttonAdd.Text = "Update";
			_filesFixed = true;
		}
		private bool processAsm()
		{
			if (string.IsNullOrEmpty(textBoxAsmx.Text))
			{
				MessageBox.Show(this, "URL for the web service cannot be empty", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			if (!textBoxAsmx.Text.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
				&& textBoxAsmx.Text.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
			{
				MessageBox.Show(this, "URL for the web service must start with http:// or https://", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			string asmx = System.IO.Path.GetFileNameWithoutExtension(textBoxAsmx.Text);
			if (string.IsNullOrEmpty(asmx))
			{
				MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid URL for the web service {0}.", textBoxAsmx.Text), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			if (!textBoxAsmx.Text.EndsWith(".asmx", StringComparison.OrdinalIgnoreCase))
			{
				MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid URL for the web service {0}. It must be an *.asmx file.", textBoxAsmx.Text), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			if (!_filesFixed)
			{
				string wsdl = System.IO.Path.Combine(_folder, asmx) + ".wsdl";
				if (System.IO.File.Exists(wsdl))
				{
					DlgAskFileOverwrite dlg = new DlgAskFileOverwrite();
					dlg.Text = "Create web service contract file (WSDL)";
					dlg.SetFilePath(wsdl);
					DialogResult ret = dlg.ShowDialog(this);
					if (ret == DialogResult.OK)
					{
						wsdl = dlg.NewFilePath;
					}
					else if (ret != DialogResult.Ignore)
					{
						return false;
					}
				}
				textBoxWsdl.Text = wsdl;
			}
			FormWebUtil webUtil = null;
			try
			{
				bool created = false;
				webUtil = new FormWebUtil();
				webUtil.Show();
				Application.DoEvents();
				webUtil.SetAsmxUrl(textBoxAsmx.Text, textBoxWsdl.Text);
				while (!webUtil.Finished)
				{
					Application.DoEvents();
				}
				created = webUtil.WsdlCreated;
				webUtil.Close();
				webUtil = null;
				if (!created)
				{
					MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Error creating WSDL. Please open [{0}] for details. ", textBoxWsdl.Text), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					if (System.IO.File.Exists(textBoxWsdl.Text))
					{
						Process p = new Process();
						p.StartInfo.FileName = "Notepad.exe";
						p.StartInfo.Arguments = textBoxWsdl.Text;
						p.Start();
					}
				}
				return created;
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			finally
			{
				if (webUtil != null)
				{
					webUtil.Close();
					webUtil = null;
				}
			}
		}
		private bool processWsdl()
		{
			if (string.IsNullOrEmpty(textBoxWsdl.Text))
			{
				MessageBox.Show(this, "File path for WSDL cannot be empty", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			string wsdl = System.IO.Path.GetFileNameWithoutExtension(textBoxWsdl.Text);
			if (string.IsNullOrEmpty(wsdl))
			{
				MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid WSDL file path [{0}].", textBoxWsdl.Text), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			if (!textBoxWsdl.Text.EndsWith(".wsdl", StringComparison.OrdinalIgnoreCase))
			{
				MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid WSDL file path [{0}]. It must be an *.wsdl file.", textBoxWsdl.Text), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			if (!System.IO.File.Exists(textBoxWsdl.Text))
			{
				MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "WSDL file does not exist: [{0}].", textBoxWsdl.Text), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			if (!_filesFixed)
			{
				string proxy = System.IO.Path.Combine(_folder, wsdl) + ".cs";
				if (System.IO.File.Exists(proxy))
				{
					DlgAskFileOverwrite dlg = new DlgAskFileOverwrite();
					dlg.Text = "Create web service proxy source code";
					dlg.SetFilePath(proxy);
					DialogResult ret = dlg.ShowDialog(this);
					if (ret == DialogResult.OK)
					{
						proxy = dlg.NewFilePath;
					}
					else if (ret != DialogResult.Ignore)
					{
						return false;
					}
				}
				textBoxProxy.Text = proxy;
			}
			string wsdlUtil = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "wsdl.exe");
			if (!System.IO.File.Exists(wsdlUtil))
			{
				MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "WSDL utility not found: [{0}].", wsdlUtil), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			toolStripStatusLabel1.Text = "Generating proxy";
			Process proc = new Process();
			//
			ProcessStartInfo psI = new ProcessStartInfo("cmd");
			psI.UseShellExecute = false;
			psI.RedirectStandardInput = false;
			psI.RedirectStandardOutput = true;
			psI.RedirectStandardError = true;
			psI.CreateNoWindow = true;
			proc.StartInfo = psI;
			//
			proc.StartInfo.FileName = wsdlUtil;
			proc.StartInfo.Arguments = string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"/language:CS /namespace:{0} /out:\"{1}\" \"{2}\"", _namespace, textBoxProxy.Text, textBoxWsdl.Text);
			proc.Start();
			proc.WaitForExit();
			if (proc.ExitCode != 0)
			{
				string msg = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Error code {0}, output:{1}, error:{2} for calling {3} {4}", proc.ExitCode, proc.StandardOutput.ReadToEnd(), proc.StandardError.ReadToEnd(), proc.StartInfo.FileName, proc.StartInfo.Arguments);
				MessageBox.Show(this, msg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return false;
			}
			return true;
		}
		private bool processProxy()
		{
			if (!System.IO.File.Exists(textBoxProxy.Text))
			{
				MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Proxy file not found: [{0}].", textBoxProxy.Text), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			int n = textBoxProxy.Text.LastIndexOf(".", StringComparison.Ordinal);
			if (n < 1)
			{
				MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid proxy file: [{0}].", textBoxProxy.Text), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			if (!_filesFixed)
			{
				textBoxDll.Text = textBoxProxy.Text.Substring(0, n) + ".dll";
				if (System.IO.File.Exists(textBoxDll.Text))
				{
					DlgAskFileOverwrite dlg = new DlgAskFileOverwrite();
					dlg.Text = "Create web service proxy dll";
					dlg.SetFilePath(textBoxDll.Text);
					DialogResult ret = dlg.ShowDialog(this);
					if (ret == DialogResult.OK)
					{
						textBoxDll.Text = dlg.NewFilePath;
					}
					else if (ret != DialogResult.Ignore)
					{
						return false;
					}
				}
			}
			CompilerParameters cp = new CompilerParameters();
			/*
			using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
			*/
			StringCollection sc = new StringCollection();
			sc.Add(typeof(System.Web.Services.Protocols.SoapHttpClientProtocol).Assembly.Location);
			string sLoc = typeof(System.Threading.SendOrPostCallback).Assembly.Location;
			if (!sc.Contains(sLoc))
			{
				sc.Add(sLoc);
			}
			sLoc = typeof(Uri).Assembly.Location;
			if (!sc.Contains(sLoc))
			{
				sc.Add(sLoc);
			}
			sLoc = typeof(Process).Assembly.Location;
			if (!sc.Contains(sLoc))
			{
				sc.Add(sLoc);
			}
			sLoc = typeof(System.Xml.Serialization.IXmlSerializable).Assembly.Location;
			if (!sc.Contains(sLoc))
			{
				sc.Add(sLoc);
			}
			sLoc = typeof(System.ComponentModel.AsyncOperation).Assembly.Location;
			if (!sc.Contains(sLoc))
			{
				sc.Add(sLoc);
			}
			sLoc = typeof(System.Threading.SendOrPostCallback).Assembly.Location;
			if (!sc.Contains(sLoc))
			{
				sc.Add(sLoc);
			}
			sLoc = typeof(System.Web.Services.Protocols.InvokeCompletedEventArgs).Assembly.Location;
			if (!sc.Contains(sLoc))
			{
				sc.Add(sLoc);
			}
			sLoc = typeof(System.Data.DataSet).Assembly.Location;
			if (!sc.Contains(sLoc))
			{
				sc.Add(sLoc);
			}
			foreach (string loc in sc)
			{
				cp.ReferencedAssemblies.Add(loc);
			}
			cp.CompilerOptions = "/t:library";
			cp.GenerateExecutable = false;
			cp.GenerateInMemory = false;
			cp.IncludeDebugInformation = true;
			cp.OutputAssembly = textBoxDll.Text;
			//
			string pdbFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(textBoxDll.Text), System.IO.Path.GetFileNameWithoutExtension(textBoxDll.Text)) + ".pdb";
			if (System.IO.File.Exists(pdbFile))
			{
				try
				{
					System.IO.File.Delete(pdbFile);
				}
				catch (Exception err0)
				{
					MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}\r\nCannot overwrite {1}. Please close Limnor Studio and all programs that may use this file. Then manually delete this file", err0.Message, pdbFile),
						this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
					return false;
				}
			}
			//
			string[] sourceFiles = new string[1];
			sourceFiles[0] = textBoxProxy.Text;
			//
			//use C# code provider
			CSharpCodeProvider ccp = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });

			CompilerResults cr = ccp.CompileAssemblyFromFile(cp, sourceFiles);
			//
			if (cr.Errors.HasErrors)
			{
				FormStringList.ShowErrors("Error compiling the web service proxy", this, cr.Errors);
				return false;
			}
			if (_designPane != null)
			{
				List<WebServiceProxy> proxyList = _designPane.Loader.Project.GetTypedProjectData<List<WebServiceProxy>>();
				if (proxyList == null)
				{
					proxyList = new List<WebServiceProxy>();
					_designPane.Loader.Project.SetTypedProjectData<List<WebServiceProxy>>(proxyList);
				}
				Assembly a = Assembly.LoadFile(textBoxDll.Text);
				_proxy = new WebServiceProxy(textBoxDll.Text, a, textBoxAsmx.Text);
				proxyList.Add(_proxy);
			}
			return true;
		}
		private bool addProxyToProject()
		{
			if (_designPane != null)
			{
				_designPane.Loader.NotifyChanges();
				_designPane.RefreshWebServiceToolbox();
				Dictionary<string, string> web = new Dictionary<string, string>();
				web.Add(XmlTags.XMLATT_filename, System.IO.Path.GetFileName(_proxy.ProxyFile));
				web.Add(XmlTags.XMLATT_asmxUrl, _proxy.AsmxUrl);
				web.Add(XmlTags.XMLATT_wsdl, System.IO.Path.GetFileName(textBoxWsdl.Text));
				web.Add(XmlTags.XMLATT_proxy, System.IO.Path.GetFileName(textBoxProxy.Text));
				_designPane.Loader.Project.AddWebService(web);
			}
			return true;
		}
		private void buttonAdd_Click(object sender, EventArgs e)
		{
			buttonAdd.Enabled = false;
			buttonFinish.Enabled = false;
			Cursor = Cursors.WaitCursor;
			toolStripProgressBar1.Value = 10;
			if (rdbAsmx.Checked)
			{
				toolStripStatusLabel1.Text = "Process asmx";
				if (!processAsm())
				{
					buttonAdd.Enabled = true;
					buttonFinish.Enabled = true;
					Cursor = Cursors.Default;
					return;
				}
				toolStripProgressBar1.Value = 30;
			}
			else
			{
				textBoxAsmx.Text = "";
			}
			toolStripStatusLabel1.Text = "Process wsdl";
			if (!processWsdl())
			{
				buttonAdd.Enabled = true;
				buttonFinish.Enabled = true;
				Cursor = Cursors.Default;
				return;
			}
			toolStripProgressBar1.Value = 60;
			toolStripStatusLabel1.Text = "Process proxy";
			if (!processProxy())
			{
				buttonAdd.Enabled = true;
				buttonFinish.Enabled = true;
				Cursor = Cursors.Default;
				return;
			}
			toolStripProgressBar1.Value = 80;
			toolStripStatusLabel1.Text = "Add proxy to project";
			//
			if (!addProxyToProject())
			{
				buttonAdd.Enabled = true;
				buttonFinish.Enabled = true;
				Cursor = Cursors.Default;
				return;
			}
			//
			string msg;
			if (_filesFixed)
			{
				msg = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					 "Web Service proxy [{0}] updated. ", textBoxDll.Text);
			}
			else
			{
				msg = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					 "Web Service proxy [{0}] added to the toolbox. ", textBoxDll.Text);
			}
			toolStripStatusLabel1.Text = msg;
			toolStripProgressBar1.Value = 100;
			this.Refresh();
			//
			MessageBox.Show(this, msg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			buttonAdd.Enabled = true;
			buttonFinish.Enabled = true;
			Cursor = Cursors.Default;
			if (_filesFixed)
			{
				this.DialogResult = DialogResult.OK;
			}
		}

		private void btWsdl_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			try
			{
				dlg.FileName = textBoxWsdl.Text;
				dlg.Filter = "WSDL file|*.wsdl";
				dlg.Title = "Select WSDL file";
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					textBoxWsdl.Text = dlg.FileName;
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message);
			}
		}

		private void rdbWsdl_CheckedChanged(object sender, EventArgs e)
		{
			textBoxAsmx.ReadOnly = rdbWsdl.Checked;
			textBoxWsdl.ReadOnly = !rdbWsdl.Checked;
			btWsdl.Enabled = rdbWsdl.Checked;
		}
	}
}
