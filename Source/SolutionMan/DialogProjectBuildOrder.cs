/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
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

namespace SolutionMan
{
	public partial class DialogProjectBuildOrder : Form
	{
		public List<ProjectDependencies> ProjectDependencies;
		private ProjectDependencies _current;
		private bool _loading;
		public DialogProjectBuildOrder()
		{
			InitializeComponent();
		}
		public void LoadData(SolutionNode sn)
		{
			comboBox1.Items.Clear();
			ProjectDependencies = sn.GetDependencies();
			foreach (ProjectDependencies pds in ProjectDependencies)
			{
				comboBox1.Items.Add(pds);
			}
			if (comboBox1.Items.Count > 0)
			{
				comboBox1.SelectedIndex = 0;
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_loading)
				return;
			_loading = true;
			_current = null;
			checkedListBox1.Items.Clear();
			ProjectDependencies pd0 = comboBox1.SelectedItem as ProjectDependencies;
			if (pd0 != null)
			{
				SortedList<string, ProjectDependencies> lst = new SortedList<string, ProjectDependencies>();
				foreach (ProjectDependencies pd in ProjectDependencies)
				{
					if (pd.Project.ProjectGuid != pd0.Project.ProjectGuid)
					{
						if (!lst.ContainsKey(pd.Project.ProjectName))
						{
							lst.Add(pd.Project.ProjectName, pd);
						}
					}
				}
				//add all projects
				IEnumerator<KeyValuePair<string, ProjectDependencies>> ie = lst.GetEnumerator();
				while (ie.MoveNext())
				{
					//add a project to listbox
					int n = checkedListBox1.Items.Add(ie.Current.Value);
					//the current project depends on this project?
					if (pd0.IsInDependencies(ie.Current.Value.Project))
					{
						checkedListBox1.SetItemChecked(n, true);
					}
					//this project depends on current project, directly/indirectly?
					List<Guid> usedProjects = new List<Guid>();
					if (ie.Current.Value.IsInDependenciesIndirect(pd0, lst, usedProjects))
					{
						checkedListBox1.SetItemChecked(n, false);
						checkedListBox1.SetItemCheckState(n, CheckState.Indeterminate);
					}
				}
				_current = pd0;
			}
			_loading = false;
		}

		private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (e.CurrentValue == CheckState.Indeterminate)
			{
				e.NewValue = CheckState.Indeterminate;
			}
			else
			{
				if (_current != null)
				{
					if (e.Index >= 0 && e.Index < checkedListBox1.Items.Count)
					{
						ProjectDependencies pd = checkedListBox1.Items[e.Index] as ProjectDependencies;
						if (pd != null)
						{
							if (e.NewValue == CheckState.Checked)
							{
								_current.AddDependency(pd.Project);
							}
							else if (e.NewValue == CheckState.Unchecked)
							{
								_current.RemoveDependency(pd.Project);
							}
						}
					}
				}
			}
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tabControl1.SelectedIndex == 1)
			{
				listBox1.Items.Clear();
				List<ProjectDependencies> orders = SolutionNode.GetBuildOrder(this.ProjectDependencies);
				for (int i = 0; i < orders.Count; i++)
				{
					listBox1.Items.Add(orders[i]);
				}
			}
		}

	}
	public class ProjectDependencies
	{
		private LimnorProject _prj;
		private List<LimnorProject> _dependencies;
		public ProjectDependencies(LimnorProject project, List<LimnorProject> dependencies)
		{
			_prj = project;
			_dependencies = dependencies;
		}
		public void AddDependency(LimnorProject project)
		{
			foreach (LimnorProject p in _dependencies)
			{
				if (p.ProjectGuid == project.ProjectGuid)
				{
					return;
				}
			}
			_dependencies.Add(project);
		}
		public void RemoveDependency(LimnorProject project)
		{
			foreach (LimnorProject p in _dependencies)
			{
				if (p.ProjectGuid == project.ProjectGuid)
				{
					_dependencies.Remove(p);
					break;
				}
			}
		}
		public bool IsInDependencies(LimnorProject p)
		{
			foreach (LimnorProject p0 in _dependencies)
			{
				if (p0.ProjectGuid == p.ProjectGuid)
				{
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="p">currently selected project</param>
		/// <param name="lst"></param>
		/// <returns></returns>
		public bool IsInDependenciesIndirect(ProjectDependencies p, SortedList<string, ProjectDependencies> lst, List<Guid> usedProjects)
		{
			if (IsInDependencies(p.Project))
			{
				return true;
			}
			if (usedProjects.Contains(_prj.ProjectGuid))
			{
				return false;
			}
			usedProjects.Add(_prj.ProjectGuid);
			foreach (LimnorProject p0 in _dependencies)
			{
				ProjectDependencies pdx = null;
				foreach (ProjectDependencies pd in lst.Values)
				{
					if (pd.Project.ProjectGuid == p0.ProjectGuid)
					{
						pdx = pd;
						break;
					}
				}
				if (pdx != null)
				{
					if (pdx.IsInDependenciesIndirect(p, lst, usedProjects))
					{
						return true;
					}
				}
			}
			return false;
		}
		public LimnorProject Project
		{
			get
			{
				return _prj;
			}
		}
		public List<LimnorProject> Dependencies
		{
			get
			{
				return _dependencies;
			}
		}
		public List<Guid> GetDependentProjectIDlist()
		{
			List<Guid> lst = new List<Guid>();
			foreach (LimnorProject p in _dependencies)
			{
				lst.Add(p.ProjectGuid);
			}
			return lst;
		}
		public override string ToString()
		{
			return _prj.ProjectName;
		}
	}
}
