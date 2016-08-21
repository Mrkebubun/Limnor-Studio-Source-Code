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
using System.Collections.Specialized;
using VPL;

namespace LimnorDesigner.ResourcesManager
{
	public partial class DlgResMan : Form
	{
		private ProjectResources _resman;
		private TreeNodeResourceManager _rootTreeNode;
		private string _curLanguageName;
		public DlgResMan()
		{
			InitializeComponent();
			treeView1.NodeSelected += new EventHandler(onNodeSelection);
			treeView1.ShowLines = false;
		}
		public static DialogResult EditResources(Form caller, LimnorProject project)
		{
			DlgResMan dlg = new DlgResMan();
			dlg.LoadData(project);
			DialogResult ret = dlg.ShowDialog(caller);
			FrmObjectExplorer.RemoveDialogCaches(project.ProjectGuid, 0);
			return ret;
		}
		private void onNodeSelection(object sender, EventArgs e)
		{
			bool bHandled = false;
			EventArgObjectSelected eos = e as EventArgObjectSelected;
			if (eos != null)
			{
				TreeNodeResource tr = eos.SelectedObject as TreeNodeResource;
				if (tr != null)
				{
					bHandled = true;
					tr.OnSelected(textBoxDefault, textBoxLocal, pictureBoxDefault, pictureBoxLocal, null);
				}
				else
				{
					TreeNodeLocalizeResource tnl = eos.SelectedObject as TreeNodeLocalizeResource;
					if (tnl != null)
					{
						bHandled = true;
						tnl.OnSelected(textBoxDefault, textBoxLocal, pictureBoxDefault, pictureBoxLocal, tnl.Culture);
					}
				}
			}
			if (!bHandled)
			{
				textBoxDefault.Visible = false;
				textBoxLocal.Visible = false;
				pictureBoxDefault.Visible = false;
				pictureBoxLocal.Visible = false;
			}
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			if (string.CompareOrdinal(_curLanguageName, _resman.DesignerLanguageName) != 0)
			{
				_resman.NotifyLanguageChange();
			}
			if (_rootTreeNode != null)
			{
				if (_rootTreeNode.ResourceManager.Save())
				{
					this.DialogResult = DialogResult.OK;
				}
			}
			base.OnClosing(e);

		}
		public void LoadData(LimnorProject proj)
		{
			_resman = proj.GetProjectSingleData<ProjectResources>();
			_curLanguageName = _resman.DesignerLanguageName;
			_rootTreeNode = new TreeNodeResourceManager(_resman, false);
			treeView1.Nodes.Add(_rootTreeNode);
			_rootTreeNode.Expand();
			Text = Text + " - " + proj.ProjectName;
		}
		private void addResource<T>() where T : TreeNodeResourceCollection
		{
			_rootTreeNode.Expand();
			T nodes = _rootTreeNode.GetCollectionNode<T>();
			if (nodes != null)
			{
				nodes.AddResource();
			}
		}
		private void buttonLanguage_Click(object sender, EventArgs e)
		{
			_rootTreeNode.SelectLanguages();
		}

		private void buttonString_Click(object sender, EventArgs e)
		{
			addResource<TreeNodeResourceStringCollection>();
		}

		private void buttonImage_Click(object sender, EventArgs e)
		{
			addResource<TreeNodeResourceImageCollection>();
		}

		private void buttonIcon_Click(object sender, EventArgs e)
		{
			addResource<TreeNodeResourceIconCollection>();
		}

		private void buttonAudio_Click(object sender, EventArgs e)
		{
			addResource<TreeNodeResourceAudioCollection>();
		}

		private void buttonFile_Click(object sender, EventArgs e)
		{
			addResource<TreeNodeResourceFileCollection>();
		}
	}
}
