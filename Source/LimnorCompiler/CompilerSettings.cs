/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using LimnorDesigner;
using System.Globalization;

namespace LimnorCompiler
{
	public class CompilerFolders
	{
		static bool init;
		public static bool NeedClearFolders
		{
			get
			{
				return init;
			}
			set
			{
				init = value;
			}
		}
		public CompilerFolders(string folder)
		{
			Folder = folder;
		}
		public void Init()
		{
			if (!System.IO.Directory.Exists(_folder))
			{
				System.IO.Directory.CreateDirectory(_folder);
			}
			string s = SourceFolder;
			if (!System.IO.Directory.Exists(s))
			{
				System.IO.Directory.CreateDirectory(s);
			}
			else
			{
				if (NeedClearFolders)
				{
					FileUtil.FileUtilities.ClearFolders(s);
				}
			}
			s = ObjectFolder;
			if (!System.IO.Directory.Exists(s))
			{
				System.IO.Directory.CreateDirectory(s);
			}
			s = BinFolder;
			if (!System.IO.Directory.Exists(s))
			{
				System.IO.Directory.CreateDirectory(s);
			}
			s = EmbedFileFolder;
			if (!System.IO.Directory.Exists(s))
			{
				System.IO.Directory.CreateDirectory(s);
			}
			else
			{
				if (NeedClearFolders)
				{
					FileUtil.FileUtilities.ClearFolders(s);
				}
			}
			NeedClearFolders = false;
		}
		public void Cleanup()
		{
			string s = EmbedFileFolder;
			FileUtil.FileUtilities.ClearFolders(s);
		}
		private string _folder = "c:\\limnorVob";
		public string Folder
		{
			get
			{
				return _folder;
			}
			set
			{
				_folder = value;
				Init();
			}
		}
		public string SourceFolder
		{
			get
			{
				return System.IO.Path.Combine(_folder, LimnorCompiler.LimnorXmlCompiler.SOURCE);
			}
		}
		public string ObjectFolder
		{
			get
			{
				return System.IO.Path.Combine(_folder, "object");
			}
		}
		public string BinFolder
		{
			get
			{
				return System.IO.Path.Combine(_folder, "bin");
			}
		}
		public string EmbedFileFolder
		{
			get
			{
				return System.IO.Path.Combine(ObjectFolder, "embed");
			}
		}
	}

	public class CompilerSettings : CompilerFolders
	{
		private string _namespace = "LimnorVOB";
		private string _typeName;
		public CompilerSettings(string componentNamespace, string componentName, string folder)
			: base(folder)
		{
			_namespace = componentNamespace;
			if (!string.IsNullOrEmpty(componentName))
			{
				int n = componentName.IndexOf('.');
				if (n > 0)
				{
					componentName = componentName.Substring(0, n);
				}
			}
			_typeName = componentName;
		}
		public string Namespace
		{
			get
			{
				return _namespace;
			}
		}
		public string SourceFilename
		{
			get
			{
				return System.IO.Path.Combine(SourceFolder, TypeName);
			}
		}
		public string SourceFilenameX
		{
			get
			{
				return System.IO.Path.Combine(SourceFolder, string.Format(CultureInfo.InvariantCulture, "{0}.Designer", NameX));
			}
		}
		public string ObjectFilename
		{
			get
			{
				return System.IO.Path.Combine(ObjectFolder, TypeName);
			}
		}
		public string ResourceFilename
		{
			get
			{
				return System.IO.Path.Combine(SourceFolder, string.Format(CultureInfo.InvariantCulture, "{0}.resx", TypeName));
			}
		}
		public string ResourceFilenameX
		{
			get
			{
				return System.IO.Path.Combine(SourceFolder, string.Format(CultureInfo.InvariantCulture, "{0}.resx", NameX));
			}
		}
		public string Resources
		{
			get
			{
				return System.IO.Path.Combine(SourceFolder, string.Format(CultureInfo.InvariantCulture, "{0}.resources", TypeName));
			}
		}
		public string ResourcesX
		{
			get
			{
				return System.IO.Path.Combine(SourceFolder, string.Format(CultureInfo.InvariantCulture, "{0}.resources", TypeNameX));
			}
		}
		/// <summary>
		/// XML map for "programming on types" to be embeded as a resource
		/// </summary>
		public string MapFilename
		{
			get
			{
				return System.IO.Path.Combine(SourceFolder, string.Format(CultureInfo.InvariantCulture, "{0}.map", TypeName));
			}
		}
		public string TypeName
		{
			get
			{
				if (string.IsNullOrEmpty(Namespace))
					return _typeName;
				return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Namespace, _typeName);
			}
		}
		public string TypeNameX
		{
			get
			{
				if (string.IsNullOrEmpty(Namespace))
					return NameX;
				return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Namespace, NameX);
			}
		}
		public string Name
		{
			get
			{
				return _typeName;
			}
		}
		public string NameX
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "X{0}", _typeName);
			}
		}
	}
}
