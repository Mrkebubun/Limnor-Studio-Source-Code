/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Limnor Studio Project
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using XmlUtility;

namespace VSPrj
{
	public sealed class LimnorSolution : List<LimnorProject>
	{
		private static object _vsDTE; //DTE
		private static LimnorSolution solution;
		public static LimnorSolution Solution
		{
			get
			{
				if (solution == null)
				{
					solution = new LimnorSolution();
				}
				return solution;
			}
		}
		public static void ResetSolution()
		{
			if (solution != null)
			{
				foreach (LimnorProject p in solution)
				{
					if (p != null)
					{
						p.OnClose();
					}
				}
			}
			solution = null;
			XmlUtil.RemoveDynamicTypes();
		}
		public static void SetVsDTE(object dte)
		{
			_vsDTE = dte;
		}
		public static object VsDTE
		{
			get
			{
				return _vsDTE;
			}
		}

		public static void AddProject(LimnorProject project)
		{
			if (solution == null)
			{
				solution = new LimnorSolution();
			}
			foreach (LimnorProject p in solution)
			{
				if (string.Compare(p.ProjectFile, project.ProjectFile, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return;
				}
			}

			solution.Add(project);
		}
		public static LimnorProject AddProject(string prjFile)
		{
			if (solution == null)
			{
				solution = new LimnorSolution();
			}
			foreach (LimnorProject p in solution)
			{
				if (string.Compare(p.ProjectFile, prjFile, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return p;
				}
			}
			LimnorProject prj = new LimnorProject(prjFile);
			LimnorProject.SetActiveProject(prj);
			return prj;
		}
		public static LimnorProject GetProjectByComponentFile(string componentFile)
		{
			string prjFile = LimnorProject.GetProjectFileByComponentFile(componentFile);
			if (!string.IsNullOrEmpty(prjFile))
			{
				Guid guid = LimnorProject.GetProjectGuid(prjFile);
				foreach (LimnorProject p in solution)
				{
					if (p.ProjectGuid == guid)
					{
						return p;
					}
				}
			}
			return null;
		}
		public static LimnorProject GetProjectByProjectFile(string projectFile)
		{
			foreach (LimnorProject p in solution)
			{
				if (string.Compare(p.ProjectFile, projectFile, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return p;
				}
			}
			return null;
		}
		public static LimnorProject CreateProject(string projectFile)
		{
			LimnorProject p = GetProjectByProjectFile(projectFile);
			if (p == null)
			{
				p = new LimnorProject(projectFile);
			}
			return p;
		}
		public static LimnorProject GetLimnorProjectByGuid(Guid guid)
		{
			if (solution != null)
			{
				return solution.GetProjectByGuid(guid);
			}
			return null;
		}

		public LimnorProject GetProjectByGuid(Guid id)
		{
			foreach (LimnorProject p in this)
			{
				if (p.ProjectGuid == id)
				{
					return p;
				}
			}
			return null;
		}
	}
}
