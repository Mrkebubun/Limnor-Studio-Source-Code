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
using System.Diagnostics;
using VSPrj;
using System.IO;
using System.Globalization;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using Microsoft.CSharp;
using VPL;
using System.Reflection;
using XmlUtility;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace LimnorDesigner.Remoting
{
	public partial class DlgAddWcfService : Form
	{
		private LimnorProject _prj;
		private WcfServiceProxy _proxy;
		private ILimnorDesignPane _designPane;
		public DlgAddWcfService()
		{
			InitializeComponent();
		}
		public void SetData(ILimnorDesignPane designer)
		{
			_designPane = designer;
			_prj = designer.Loader.Project;
			labelFolder.Text = _prj.ProjectFolder;
			labelIDE.Text = Path.GetDirectoryName(Application.ExecutablePath);
		}
		private void showStatus(string status)
		{
			labelStatus.Text = status;
			labelStatus.Refresh();
		}
		private void buttonGenerate_Click(object sender, EventArgs e)
		{
			try
			{
				buttonCancel.Enabled = false;
				buttonGenerate.Enabled = false;
				Cursor = Cursors.WaitCursor;
				showStatus("validating...");
				if (string.IsNullOrEmpty(textBoxServiceUrl.Text))
				{
					throw new DesignerException("Service URL is missing");
				}
				if (string.IsNullOrEmpty(textBoxSourceFilename.Text))
				{
					throw new DesignerException("DLL file name is missing");
				}
				string svcdlUtil = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "svcutil.exe");
				if (!System.IO.File.Exists(svcdlUtil))
				{
					throw new DesignerException("Service utility not found: [{0}].", svcdlUtil);
				}
				string appTargetDll = string.Format(CultureInfo.InvariantCulture, "{0}.dll", Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), textBoxSourceFilename.Text));
				if (File.Exists(appTargetDll))
				{
					if (MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture,
						"The file [{0}] exists. Do you want to overwrite it?", appTargetDll), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					{
						throw new DesignerException("User canceled the operation");
					}
				}
				//
				string targetFilename = Path.Combine(_prj.ProjectFolder, textBoxSourceFilename.Text);
				string configFilename = targetFilename;
				//
				showStatus("Generate source code and configuration...");
				//
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
				proc.StartInfo.FileName = svcdlUtil;
				//svcutil.exe /language:cs /out:generatedProxy.cs /config:app.config http://localhost:8000/ServiceModelSamples/service
				proc.StartInfo.Arguments = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"/language:cs /out:\"{0}.cs\" /config:\"{1}.config\" \"{2}\"",
					targetFilename, configFilename, textBoxServiceUrl.Text);
				proc.Start();
				proc.WaitForExit();
				if (proc.ExitCode != 0)
				{
					throw new DesignerException("Error code {0}, output:{1}, error:{2} for calling {3} {4}", proc.ExitCode, proc.StandardOutput.ReadToEnd(), proc.StandardError.ReadToEnd(), proc.StartInfo.FileName, proc.StartInfo.Arguments);
				}
				//
				showStatus("Generate proxy DLL...");
				if (processProxy())
				{
					if (addProxyToProject())
					{
						this.DialogResult = DialogResult.OK;
						return;
					}
				}

				//
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			buttonCancel.Enabled = true;
			buttonGenerate.Enabled = true;
			Cursor = Cursors.Default;
		}
		private bool processProxy()
		{
			string targetFilename = string.Format(CultureInfo.InvariantCulture, "{0}.cs", Path.Combine(_prj.ProjectFolder, textBoxSourceFilename.Text));
			string configFilename = string.Format(CultureInfo.InvariantCulture, "{0}.config", Path.Combine(_prj.ProjectFolder, textBoxSourceFilename.Text));

			if (!System.IO.File.Exists(targetFilename))
			{
				MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Proxy file not generated: [{0}].", targetFilename), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			string dllFilename = string.Format(CultureInfo.InvariantCulture, "{0}.dll", Path.Combine(_prj.ProjectFolder, textBoxSourceFilename.Text));

			if (System.IO.File.Exists(dllFilename))
			{
				File.Delete(dllFilename);
			}
			string targetFilename2 = string.Format(CultureInfo.InvariantCulture, "{0}2.cs", Path.Combine(_prj.ProjectFolder, textBoxSourceFilename.Text));
			if (File.Exists(targetFilename2))
			{
				File.Delete(targetFilename2);
			}
			string className = null;
			StreamReader sr = new StreamReader(targetFilename);
			while (!sr.EndOfStream && string.IsNullOrEmpty(className))
			{
				string line = sr.ReadLine();
				if (!string.IsNullOrEmpty(line))
				{
					if (line.StartsWith("public partial class ", StringComparison.Ordinal))
					{
						int n = line.IndexOf(':');
						if (n > 0)
						{
							string s = line.Substring(0, n).Trim();
							n = s.LastIndexOf(' ');
							className = s.Substring(n).Trim();
						}
					}
				}
			}
			sr.Close();
			if (string.IsNullOrEmpty(className))
			{
				throw new DesignerException("WCF proxy class not found in {0}", targetFilename);
			}
			StreamWriter sw = new StreamWriter(targetFilename2);
			string code2 = Resources.CodeFileWcfProxy;
			code2 = code2.Replace("proxyName", className);
			sw.Write(code2);
			sw.Close();
			CompilerParameters cp = new CompilerParameters();
			StringCollection sc = new StringCollection();
			string sLoc;
			sLoc = typeof(System.ServiceModel.ServiceHost).Assembly.Location;
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
			sLoc = typeof(System.Windows.Forms.Form).Assembly.Location;
			if (!sc.Contains(sLoc))
			{
				sc.Add(sLoc);
			}
			sLoc = typeof(UITypeEditor).Assembly.Location;
			if (!sc.Contains(sLoc))
			{
				sc.Add(sLoc);
			}
			sLoc = typeof(IWindowsFormsEditorService).Assembly.Location;
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
			cp.OutputAssembly = dllFilename;
			//
			string pdbFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(dllFilename), System.IO.Path.GetFileNameWithoutExtension(dllFilename)) + ".pdb";
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
			string[] sourceFiles = new string[] { targetFilename, targetFilename2 };
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
			if (_prj != null)
			{
				string appDllFilename = string.Format(CultureInfo.InvariantCulture, "{0}.dll",
					Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), textBoxSourceFilename.Text));
				List<WcfServiceProxy> proxyList = _prj.GetTypedProjectData<List<WcfServiceProxy>>();
				if (proxyList == null)
				{
					proxyList = new List<WcfServiceProxy>();
					_prj.SetTypedProjectData<List<WcfServiceProxy>>(proxyList);
				}
				try
				{
					File.Copy(dllFilename, appDllFilename, true);
				}
				catch (Exception err)
				{
					MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Error copying [{0}] to [{1}]. You may close Limnor Studio and manually copy the file. \r\n{2}",
						dllFilename, appDllFilename, err.Message), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				Assembly a = Assembly.LoadFile(appDllFilename);
				_proxy = new WcfServiceProxy(Path.GetFileName(dllFilename), a, textBoxServiceUrl.Text,
					string.Format(CultureInfo.InvariantCulture, "{0}.config", textBoxSourceFilename.Text));
				proxyList.Add(_proxy);
				MessageBox.Show(this, "The WCF Remoting Service proxy has been added to the Toolbox", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
				web.Add(XmlTags.XMLATT_filename, System.IO.Path.GetFileName(_proxy.ProxyDllFile));
				web.Add(XmlTags.XMLATT_url, _proxy.ServerUrl);
				web.Add(XmlTags.XMLATT_config, _proxy.ConfigFile);
				_designPane.Loader.Project.AddWcfService(web);
			}
			return true;
		}
		private void textBoxSourceFilename_TextChanged(object sender, EventArgs e)
		{
			if (textBoxSourceFilename.Text.Contains(':') || textBoxSourceFilename.Text.Contains('\\') || textBoxSourceFilename.Text.Contains(' '))
			{
				textBoxSourceFilename.Text = textBoxSourceFilename.Text.Replace(':', '_');
				textBoxSourceFilename.Text = textBoxSourceFilename.Text.Replace('\\', '_');
				textBoxSourceFilename.Text = textBoxSourceFilename.Text.Replace(' ', '_');
			}
		}
	}
}
