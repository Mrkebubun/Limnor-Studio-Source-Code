/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Component Importer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Security.Policy;
using System.Reflection;
using VPL;

namespace PerformerImport
{
	public enum enumSourceType { DotNetClass = 0, ActiveX = 1, Compile = 2 }
	/// <summary>
	/// Saves all information collected via the wizard 
	/// </summary>
	public class PerformerImporter
	{
		public enumSourceType SourceType = enumSourceType.DotNetClass;
		public bool ControlOnly = false;
		public System.Type DotNetType = typeof(void);
		protected System.Windows.Forms.Form[] openedForms = null;

		protected string sDllFile = "";
		protected string sOCXWrapperDllFile = "";
		protected string sSourceFile = "";
		protected string[] supportDLLs = null;
		//
		private List<Type> _types;
		//

		//
		public PerformerImporter()
		{
			//
			// TODO: Add constructor logic here
			//

		}
		public string OCXWrapperDllFile
		{
			get
			{
				return ActiveXImporter.ActiveXInfo.OCXWrapperDllFile;
			}
		}

		public int SupportDllCount
		{
			get
			{
				if (supportDLLs == null)
					return 0;
				return supportDLLs.Length;
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
		public void ClearSupportDLLs()
		{
			supportDLLs = null;
		}
		public string GetSupportFile(int i)
		{
			return supportDLLs[i];
		}
		public string[] GetSupportFiles()
		{
			int n;
			string[] rets = null;
			if (System.IO.File.Exists(DotNetDLL))
			{
				rets = new string[1];
				rets[0] = DotNetDLL;
			}
			if (SourceType == enumSourceType.ActiveX)
			{
				if (System.IO.File.Exists(OCXWrapperDllFile))
				{
					if (rets == null)
					{
						n = 0;
						rets = new string[1];
					}
					else
					{
						n = rets.Length;
						string[] a = new string[n + 1];
						a[0] = rets[0];
						rets = a;
					}
					rets[n] = OCXWrapperDllFile;
				}
			}
			int nCount = SupportDllCount;
			if (nCount > 0)
			{
				if (rets == null)
				{
					n = 0;
					rets = new string[nCount];
				}
				else
				{
					n = rets.Length;
					string[] a = new string[nCount + n];
					for (int i = 0; i < n; i++)
						a[i] = rets[i];
					rets = a;
				}
				for (int i = 0; i < nCount; i++)
				{
					rets[n + i] = GetSupportFile(i);
				}
			}
			return rets;
		}
		public void DelSupportDLL(string sFile)
		{
			int n = SupportDllCount;
			for (int i = 0; i < n; i++)
			{
				if (string.Compare(supportDLLs[i], sFile, StringComparison.OrdinalIgnoreCase) == 0)
				{
					if (n == 1)
						supportDLLs = null;
					else
					{
						string[] a = new string[n - 1];
						for (int k = 0; k < n; k++)
						{
							if (k < i)
								a[k] = supportDLLs[k];
							else if (k > i)
								a[k - 1] = supportDLLs[k];
						}
						supportDLLs = a;
					}
					break;
				}
			}
		}
		public void AddSupportDLL(string sFile)
		{
			string sWrapper = OCXWrapperDllFile;
			string sDotNet = DotNetDLL;
			if (string.Compare(sFile, sWrapper, StringComparison.OrdinalIgnoreCase) != 0)
			{
				if (string.Compare(sFile, sDotNet, StringComparison.OrdinalIgnoreCase) != 0)
				{
					int n = SupportDllCount;
					for (int i = 0; i < n; i++)
					{
						if (string.Compare(supportDLLs[i], sFile, StringComparison.OrdinalIgnoreCase) == 0)
						{
							return;
						}
					}

					string[] a = new string[n + 1];
					for (int i = 0; i < n; i++)
					{
						a[i] = supportDLLs[i];
					}
					supportDLLs = a;
					supportDLLs[n] = sFile;
				}
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
		public string DotNetDLL
		{
			get
			{
				return sDllFile;
			}
		}


		public string SourceFile
		{
			get
			{
				if (SourceType == enumSourceType.Compile)
					return sSourceFile;
				return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), DotNetType.ToString().Replace(".", "")) + ".cs";
			}
		}
		public void SetSourceFile(string sFile)
		{
			sSourceFile = sFile;
		}
		public string CompiledFile
		{
			get
			{
				if (SourceType == enumSourceType.Compile)
				{
					if (sSourceFile.Length > 0)
						return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(sSourceFile), System.IO.Path.GetFileNameWithoutExtension(sSourceFile) + ".DLL");
					return "";
				}
				return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), DotNetType.ToString().Replace(".", "")) + ".DLL";
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
	}
}
