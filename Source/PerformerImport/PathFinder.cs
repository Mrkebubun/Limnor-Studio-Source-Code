/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Component Importer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Security.Policy;
using System.IO;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace PerformerImport
{
	public class PathFinder : MarshalByRefObject
	{
		private string[] _paths;
		public PathFinder()
		{
		}
		public void Run(string path)
		{
			List<string> lst = new List<string>();
			Assembly mainAssem = Assembly.ReflectionOnlyLoadFrom(path);
			AssemblyName[] refNames = mainAssem.GetReferencedAssemblies();
			foreach (AssemblyName refName in refNames)
			{
				Assembly loadedAssem = Assembly.ReflectionOnlyLoad(refName.FullName);
				if (!loadedAssem.GlobalAssemblyCache)
				{
					lst.Add(loadedAssem.Location);
				}
			}
			_paths = new string[lst.Count];
			lst.CopyTo(_paths);
		}
		public string[] Paths { get { return _paths; } }
		//
		public static void GetAssemblyPaths(StringCollection paths, string assemblyPath)
		{
			string s = assemblyPath.ToLowerInvariant();
			if (!paths.Contains(s))
			{
				paths.Add(s);
				//
				try
				{
					AppDomainSetup domainInfo = new AppDomainSetup();
					domainInfo.ApplicationBase = "file:///" + Path.GetDirectoryName(s);
					domainInfo.DisallowBindingRedirects = false;
					domainInfo.DisallowCodeDownload = true;
					AppDomain domain = AppDomain.CreateDomain("PathFinder", null, domainInfo);

					PathFinder runObj;
					runObj = (PathFinder)domain.CreateInstanceFromAndUnwrap(
											typeof(PathFinder).Assembly.Location,
											typeof(PathFinder).FullName);
					runObj.Run(assemblyPath);

					if (runObj.Paths != null)
					{
						for (int i = 0; i < runObj.Paths.Length; i++)
						{
							s = runObj.Paths[i].ToLowerInvariant();
							if (!paths.Contains(s))
							{
								paths.Add(s);
							}
						}
					}
				}
				catch (Exception err)
				{
					MessageBox.Show(err.Message);
				}
			}
		}
	}
}
