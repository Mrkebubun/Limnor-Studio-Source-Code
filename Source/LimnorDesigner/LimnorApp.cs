/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.CodeDom;
using System.Windows.Forms;
using VPL;
using System.Xml.Serialization;
using VSPrj;

namespace LimnorDesigner
{
    public enum EnumThreadMode
    {
        None      = 0,
        STAThread = 1,
        MTAThread = 2
    }
	[Browsable(true)]//when showing properties, only show browsable ones
	[Serializable]
	public abstract class LimnorApp : IComponent, ISerializeAsObject, IWithProject2
	{
		public const string ExeMain = "Main";
		private string _name;
		private ISite _site;
		private LimnorProject _prj;
		private EnumThreadMode _threadMode = EnumThreadMode.STAThread;
		protected CodeEntryPointMethod mainMethod;
		protected CodeTypeDeclaration typeDeclaration;
		protected CodeNamespace typeNamespace;
		protected IList<ComponentID> rootClasses;
		protected string OutputFullPath;
		protected string OutputFilename;

		public LimnorApp()
		{
		}
		[NotForProgramming]
		[ReadOnly(true)]
		[XmlIgnore]
		[Browsable(false)]
		public EnumThreadMode ThreadMode
		{
			get
			{
				return _threadMode;
			}
			set
			{
				_threadMode = value;
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public CodeEntryPointMethod MainMethod
		{
			get
			{
				return mainMethod;
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public void SetProject(LimnorProject project)
		{
			_prj = project;
		}
		[NotForProgramming]
		[Browsable(false)]
		public virtual void ExportCode(CodeTypeDeclaration td, CodeNamespace ns, IList<ComponentID> cs, string outputPath, string assemblyName, bool forDebug)
		{
			typeDeclaration = td;
			typeNamespace = ns;
			rootClasses = cs;
			OutputFullPath = outputPath;
			OutputFilename = assemblyName;
			//no need to be static
			td.Attributes = MemberAttributes.Private;
			mainMethod = new CodeEntryPointMethod();
			mainMethod.Name = ExeMain;
			mainMethod.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			if (ThreadMode == EnumThreadMode.STAThread)
			{
				mainMethod.CustomAttributes.Add(new CodeAttributeDeclaration("STAThread"));
			}
			else if (ThreadMode == EnumThreadMode.MTAThread)
			{
				mainMethod.CustomAttributes.Add(new CodeAttributeDeclaration("MTAThread"));
			}
			td.Members.Add(mainMethod);
			//
			OnExportCode(forDebug);
		}
		private static string _staticName;
		[NotForProgramming]
		[Browsable(false)]
		public static void SetStaticName(string name)
		{
			_staticName = name;
		}
		[NotForProgramming]
		[Browsable(false)]
		public static string GetStaticName()
		{
			return _staticName;
		}
		[NotForProgramming]
		[Browsable(false)]
		protected abstract void OnExportCode(bool forDebug);

		[NotForProgramming]
		[Browsable(false)]
		public abstract void Run();
		[NotForProgramming]
		[Browsable(false)]
		public string Name
		{
			get
			{
				if (_site != null)
				{
					if (string.IsNullOrEmpty(_site.Name))
					{
						_site.Name = _name;
					}
					else if (string.IsNullOrEmpty(_name))
					{
						_name = _site.Name;
					}
				}
				return _name;
			}
			set
			{
				_name = value;
				if (_site != null)
				{
					_site.Name = _name;
				}
				_staticName = _name;
			}
		}

		[NotForProgramming]
		[Browsable(false)]
		[ReadOnly(true)]
		public string Namespace { get; set; }

		#region IComponent Members

		[NotForProgramming]
		[Browsable(false)]
		public event EventHandler Disposed;

		[NotForProgramming]
		[ReadOnly(true)]
		[Browsable(false)]
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members
		[NotForProgramming]
		[Browsable(false)]
		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region ISerializeAsObject Members
		[NotForProgramming]
		[Browsable(false)]
		public bool NeedSerializeAsObject
		{
			get { return false; }
		}

		#endregion

		#region IWithProject Members
		[NotForProgramming]
		[Browsable(false)]
		public LimnorProject Project
		{
			get { return _prj; }
		}

		#endregion
	}
}
