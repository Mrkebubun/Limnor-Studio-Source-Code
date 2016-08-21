/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	ActiveX Import Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using VPL;

namespace PerformerImport
{
	public class ActiveXImporter
	{
		private List<Type> _types;
		public Type DotNetType = typeof(void);
		public bool ControlOnly = false;
		protected dlgOCXTypes objOCXTypes = null;
		protected TypeRec rec = null; //OCX types
		protected string sDllFile = "";
		protected string sOCXWrapperDllFile = "";
		[DllImport("kernel32.dll")]
		static extern uint GetSystemDirectory(System.Text.StringBuilder lpBuffer, uint uSize);
		[DllImport("kernel32.dll")]
		public static extern int GetWindowsDirectory(System.Text.StringBuilder lpBuffer, int nSize);
		public TypeRec GetOcxInfo()
		{
			return rec;
		}
		private static ActiveXImporter _default;
		public static ActiveXImporter ActiveXInfo
		{
			get
			{
				if (_default == null)
				{
					_default = new ActiveXImporter();
				}
				return _default;
			}
		}
		public void ClearSelectedTypes()
		{
			_types = null;
		}
		public void AddSelectedType(Type t)
		{
			if (_types == null)
			{
				_types = new List<Type>();
			}
			if (!_types.Contains(t))
			{
				foreach (Type t0 in _types)
				{
					if (string.Compare(t0.AssemblyQualifiedName, t.AssemblyQualifiedName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						_types.Remove(t0);
						break;
					}
				}
				_types.Add(t);
			}
		}
		public Type[] GetSelectedTypes()
		{
			Type[] types;
			if (_types == null)
			{
				types = new Type[] { };
			}
			else
			{
				types = new Type[_types.Count];
				_types.CopyTo(types);
			}
			return types;
		}
		public string DotNetDLL
		{
			get
			{
				return sDllFile;
			}
		}
		public bool DotNetDLLReady
		{
			get
			{
				if (sDllFile.Length > 0)
				{
					return System.IO.File.Exists(sDllFile);
				}
				return false;
			}
		}
		public string OcxFile
		{
			get
			{
				if (rec != null)
					return rec.File;
				return "";
			}
		}
		public string ComDll
		{
			get
			{
				if (rec != null)
				{
					return rec.File;
				}
				return "";
			}
		}
		public void SetDotNetDLL(string sFile)
		{
			sDllFile = sFile;
		}
		public bool ShowPublicTypes(System.Windows.Forms.ListBox lst, out string sMsg)
		{
			bool bRet = false;
			sMsg = "";
			lst.Items.Clear();
			string sCurDir = Directory.GetCurrentDirectory();
			try
			{
				string sDir = Path.GetDirectoryName(sDllFile);

				Directory.SetCurrentDirectory(sDir);
				System.Reflection.Assembly a = System.Reflection.Assembly.LoadFrom(sDllFile);
				System.Type[] tps = VPLUtil.GetExportedTypes(a);
				if (tps != null)
				{
					for (int i = 0; i < tps.Length; i++)
					{
						if (tps[i].IsPublic && !tps[i].IsAbstract && tps[i].IsClass)
						{
							if (tps[i].IsSubclassOf(typeof(System.Windows.Forms.Control)) || !ControlOnly)
							{
								lst.Items.Add(tps[i]);
							}
							else
							{
								System.Type tp0 = System.Type.GetType(tps[i].AssemblyQualifiedName);
								if (tp0 != null)
								{
									if (tp0.IsSubclassOf(typeof(System.Windows.Forms.Control)) || !ControlOnly)
									{
										lst.Items.Add(tp0);
									}
								}
							}
						}
					}
					bRet = true;
				}
			}
			catch (Exception er)
			{
				sMsg = er.Message;
			}
			Directory.SetCurrentDirectory(sCurDir);
			return bRet;
		}
		public bool GetOcxFile(System.Windows.Forms.Form caller)
		{
			if (objOCXTypes == null)
			{
				objOCXTypes = new dlgOCXTypes();
			}
			if (objOCXTypes.ShowDialog(caller) == System.Windows.Forms.DialogResult.OK)
			{
				rec = objOCXTypes.objRet;
				return true;
			}
			return false;
		}
		public bool GetOcxFileByBrowse(System.Windows.Forms.Form caller, out string sMsg)
		{
			sMsg = "";
			System.Text.StringBuilder sb = new System.Text.StringBuilder(200);
			System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
			dlg.Title = "Open Type Library";
			dlg.Filter = "Type Libraries (*.tlb;*.olb;*.dll;*.ocx)|*.tlb;*.olb;*.dll;*.ocx|All Files (*.*)|*.*";
			GetSystemDirectory(sb, 200);
			dlg.InitialDirectory = sb.ToString();
			if (dlg.ShowDialog(caller) == System.Windows.Forms.DialogResult.OK)
			{
				try
				{
					TLI.TypeLibInfoClass tliTypeLibInfo = new TLI.TypeLibInfoClass();
					tliTypeLibInfo.ContainingFile = dlg.FileName;
					//
					rec = new TypeRec();
					rec.GUID = tliTypeLibInfo.GUID;
					rec.LCID = tliTypeLibInfo.LCID;
					rec.MainVer = tliTypeLibInfo.MajorVersion;
					rec.MinVer = tliTypeLibInfo.MinorVersion;
					rec.LoadInfo();
					//
					return true;
				}
				catch (Exception er)
				{
					sMsg = er.Message;
				}
			}
			return false;
		}
		public static string FindDLL(string sDir)
		{
			string[] files = System.IO.Directory.GetFiles(sDir, "*.DLL");
			if (files != null)
			{
				if (files.Length > 0)
				{
					return files[0];
				}
			}
			files = System.IO.Directory.GetDirectories(sDir);
			if (files != null)
			{
				for (int i = 0; i < files.Length; i++)
				{
					string s = FindDLL(files[i]);
					if (s.Length > 0)
						return s;
				}
			}
			return "";
		}
		public bool CreateAxWrapper(out string sMsg)
		{
			bool bRet = false;
			sMsg = "";
			try
			{
				string sDirAX = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
				string sExe = Path.Combine(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "AxImp.exe");
				if (File.Exists(sExe))
				{
					string sFile = System.IO.Path.Combine(sDirAX, "AxInterop." + rec.Name + ".DLL");
					sOCXWrapperDllFile = System.IO.Path.Combine(sDirAX, rec.Name + ".DLL");
					bRet = System.IO.File.Exists(sFile);
					if (bRet)
					{
						//file exists, show warning and stop
						sDllFile = sFile;
						sMsg = "Warning:\r\nFile already exists: " + sFile;
						sFile = System.IO.Path.Combine(sDirAX, rec.Name + ".DLL");
						if (System.IO.File.Exists(sFile))
						{
							sMsg += "\r\nFile already exists: " + sFile;
						}
						else
						{
							sMsg += "\r\nFile does not exist: " + sFile;
						}
						sMsg += "\r\n\r\nExisting files will be used. If you want to re-generate files then delete the existing files";
					}
					else
					{
						Process p = new Process();
						p.StartInfo = new ProcessStartInfo(sExe);
						p.StartInfo.RedirectStandardError = true;
						p.StartInfo.RedirectStandardInput = true;
						p.StartInfo.RedirectStandardOutput = true;
						p.StartInfo.CreateNoWindow = true;
						p.StartInfo.ErrorDialog = false;
						p.StartInfo.UseShellExecute = false;
						//
						string s = "\"";
						s += rec.File;
						s += "\"";
						s += " /out:\"" + sFile + "\"";
						//s += " /source";
						p.StartInfo.Arguments = s;
						try
						{
							if (p.Start())
							{
								p.WaitForExit();
								if (p.ExitCode == 0)
								{
									if (System.IO.File.Exists(sFile))
									{
										sDllFile = sFile;
										bRet = true;
									}
									else
									{
										sMsg = "File not found: " + sFile;
									}
								}
								else
								{
									sMsg = "Failed to run " + sExe + ". Returned " + p.ExitCode.ToString() + ". Error:" + p.StandardOutput.ReadToEnd() + " " + p.StandardError.ReadToEnd();
								}
							}
							else
							{
								sMsg = "Failed to start " + sExe;
							}
						}
						catch (Exception er)
						{
							sMsg = er.Message;
						}
					}
				}
				else
				{
					sMsg = "File not found: " + sExe;
				}
			}
			catch (Exception er2)
			{
				sMsg = er2.Message;
			}
			return bRet;
		}
		public string OCXWrapperDllFile
		{
			get
			{
				if (!System.IO.File.Exists(sOCXWrapperDllFile))
				{
					System.Text.StringBuilder lpBuffer = new System.Text.StringBuilder(200);
					GetWindowsDirectory(lpBuffer, 200);
					string sGacDir = lpBuffer.ToString();
					string sType = DotNetType.Namespace;
					if (sType.IndexOf("Ax") == 0)
					{
						sType = sType.Substring(2);
					}
					sGacDir = System.IO.Path.Combine(sGacDir, "assembly\\GAC\\" + sType);
					if (System.IO.Directory.Exists(sGacDir))
					{
						string sFile = FindDLL(sGacDir);
						if (sFile.Length > 0)
							sOCXWrapperDllFile = sFile;
					}
				}
				return sOCXWrapperDllFile;
			}
		}
	}
}
