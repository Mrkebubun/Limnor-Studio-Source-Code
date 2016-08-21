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
using MathExp;
using VSPrj;
using LimnorDesigner.Action;

namespace LimnorDesigner.MethodBuilder
{
	public partial class DlgDecisionTable : Form, IMethodDialog
	{
		private MethodDesignerHolder _viewer;
		public DlgDecisionTable()
		{
			InitializeComponent();
		}
		public void LoadData(DecisionTable data, MethodClass method, LimnorProject project, MethodDesignerHolder view)
		{
			_viewer = view;
			this.ClientSize = data.ControlSize;
			decisionTableControl1.LoadData(data, method, project);
		}
		public DecisionTable Result
		{
			get
			{
				return decisionTableControl1.Result;
			}
		}
		protected override void OnActivated(EventArgs e)
		{
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
