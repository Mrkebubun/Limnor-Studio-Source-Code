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
using System.IO;
using System.Globalization;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace LimnorDesigner
{
	public partial class DialogCompileCode : Form
	{
		private bool _loading;
		private bool _changed;
		public DialogCompileCode()
		{
			InitializeComponent();
		}
		public static void StartSourceCompiler()
		{
			DialogCompileCode dlg = new DialogCompileCode();
			dlg.ShowDialog();
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			askSave();
		}
		private void askSave()
		{
			if (_changed)
			{
				if (textBoxSourceFile.Text.Length > 0)
				{
					if (MessageBox.Show(this, "Do you want to save the source code modifications?", "Source code edit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						save();
					}
				}
			}
		}
		private void save()
		{
			StreamWriter sw = null;
			try
			{
				sw = new StreamWriter(textBoxSourceFile.Text, false, Encoding.Unicode);
				sw.Write(textBoxContents.Text);
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, "Save source file", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				if (sw != null)
				{
					sw.Close();
				}
			}
			_changed = false;
			buttonSave.Enabled = false;
		}
		private void buttonSourceFile_Click(object sender, EventArgs e)
		{
			StreamReader sr = null;
			buttonSourceFile.Enabled = false;
			try
			{
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.CheckFileExists = true;
				if (rbCSharp.Checked)
				{
					dlg.Title = "Select C# source file";
					dlg.Filter = "C# file|*.cs";
				}
				else
				{
					dlg.Title = "Select VB.NET source file";
					dlg.Filter = "VB.NET file|*.vb;*.bas";
				}
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					askSave();
					_loading = true;
					sr = new StreamReader(dlg.FileName);
					textBoxContents.Text = sr.ReadToEnd();
					sr.Close();

					_changed = false;
					textBoxSourceFile.Text = dlg.FileName;
					string dir = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "tmp");
					if (!Directory.Exists(dir))
					{
						Directory.CreateDirectory(dir);
					}
					textBoxDll.Text = Path.Combine(dir,
						string.Format(CultureInfo.InvariantCulture, "{0}.dll", Path.GetFileNameWithoutExtension(dlg.FileName)));
					buttonSave.Enabled = false;
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, "Select source file", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				buttonSourceFile.Enabled = true;
				_loading = false;
				if (sr != null)
				{
					sr.Dispose();
				}

			}
		}

		private void textBoxContents_TextChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				_changed = true;
				buttonSave.Enabled = true;
			}
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			buttonSave.Enabled = false;
			save();
		}

		private void buttonCompile_Click(object sender, EventArgs e)
		{
			buttonCompile.Enabled = false;
			buttonSourceFile.Enabled = false;
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			try
			{
				askSave();
				resetMsg("Compiling ...");
				if (textBoxSourceFile.Text.Length == 0)
				{
					throw new ArgumentException("Source file not specified");
				}
				if (!File.Exists(textBoxSourceFile.Text))
				{
					throw new ArgumentException("Source file does not exist");
				}
				if (textBoxDll.Text.Length == 0)
				{
					throw new ArgumentException("DLL file not specified");
				}

				//prepare compilation parameters
				CompilerParameters cp = new CompilerParameters();
				//
				StringCollection asl = new StringCollection();
				string objLoc = typeof(object).Assembly.Location.ToLowerInvariant();
				asl.Add(objLoc);
				//
				string s = typeof(DllImportAttribute).Assembly.Location.ToLowerInvariant();
				if (!asl.Contains(s))
				{
					asl.Add(s);
				}
				s = typeof(System.Drawing.Bitmap).Assembly.Location.ToLowerInvariant();
				if (!asl.Contains(s))
				{
					asl.Add(s);
				}
				s = typeof(System.Text.ASCIIEncoding).Assembly.Location.ToLowerInvariant();
				if (!asl.Contains(s))
				{
					asl.Add(s);
				}
				s = typeof(System.IntPtr).Assembly.Location.ToLowerInvariant();
				if (!asl.Contains(s))
				{
					asl.Add(s);
				}
				s = typeof(System.IO.Directory).Assembly.Location.ToLowerInvariant();
				if (!asl.Contains(s))
				{
					asl.Add(s);
				}
				s = typeof(System.Net.Cookie).Assembly.Location.ToLowerInvariant();
				if (!asl.Contains(s))
				{
					asl.Add(s);
				}
				//
				foreach (string loc in asl)
				{
					cp.ReferencedAssemblies.Add(loc);
				}
				//
				cp.GenerateExecutable = false;
				StringBuilder sb = new StringBuilder();
				sb.Append(" /t:library ");
				//
				cp.CompilerOptions = sb.ToString();
				cp.OutputAssembly = textBoxDll.Text;
				//
				cp.GenerateInMemory = false;
				cp.IncludeDebugInformation = true;
				//
				string[] sourceFiles = new string[] { textBoxSourceFile.Text };
				//

#if DOTNET40
				string ver = "v4.0";
#else
				string ver = "v3.5";
#endif
				CodeDomProvider ccp;
				//use C# code provider
				if (rbCSharp.Checked)
				{
					ccp = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", ver } });
				}
				else
				{
					ccp = new VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", ver } });
				}
				CompilerResults cr = ccp.CompileAssemblyFromFile(cp, sourceFiles);
				//
				if (cr.Errors.HasErrors)
				{
					foreach (CompilerError error in cr.Errors)
					{
						appendMsg(error.ToString());
					}
				}
				else
				{
					appendMsg("Compilation done. Copying DLL");
					string target = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location),
						Path.GetFileName(textBoxDll.Text));
					File.Copy(textBoxDll.Text, target);
					appendMsg("Succeed");
				}
			}
			catch (Exception err)
			{
				appendMsg(err.Message);
				MessageBox.Show(this, err.Message, "Compile source file", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			buttonCompile.Enabled = true;
			buttonSourceFile.Enabled = true;
			this.Cursor = System.Windows.Forms.Cursors.Default;
		}
		private void resetMsg(string s)
		{
			textBoxMsg.Text = s;
		}
		private void appendMsg(string s)
		{
			textBoxMsg.Text = string.Format(CultureInfo.InvariantCulture, "{0}\r\n{1}", textBoxMsg.Text, s);
		}
	}
}
