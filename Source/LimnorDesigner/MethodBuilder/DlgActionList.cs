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
using System.Text;
using System.Windows.Forms;
using LimnorDesigner.Action;
using VSPrj;
using MathExp;

namespace LimnorDesigner.MethodBuilder
{
	public partial class DlgActionList : Form, IMethodDialog
	{
		private MethodDesignerHolder _viewer;
		private ActionList _actions;
		private MethodClass _method;
		private LimnorProject _project;
		private bool _loaded;
		public DlgActionList()
		{
			InitializeComponent();
			actionListControl1.ActionNameChanged += new EventHandler(actionListControl1_ActionNameChanged);
		}

		void actionListControl1_ActionNameChanged(object sender, EventArgs e)
		{
			this.Text = string.Format("Actions - {0}", actionListControl1.ActionName);
		}
		public void LoadData(ActionList actions, MethodClass method, LimnorProject project, MethodDesignerHolder view)
		{
			_viewer = view;
			_actions = actions;
			_method = method;
			_project = project;
			_loaded = false;
			this.Text = string.Format("Actions - {0}", actions.Name);
#if DEBUG
			MathNode.Trace("End of DlgActionList.LoadData");
#endif
		}
		public ActionList Result
		{
			get
			{
				return actionListControl1.Result;
			}
		}
		protected override void OnActivated(EventArgs e)
		{
			if (!_loaded)
			{
				_loaded = true;
				actionListControl1.LoadData(_actions, _method, _project);
				actionListControl1.SetLoaded();
			}
			FormProgress.HideProgress();
			base.OnActivated(e);
		}
		#region IMethodDialog Members

		public MethodDesignerHolder GetEditor()
		{
			return _viewer;
		}

		#endregion

	}
}
