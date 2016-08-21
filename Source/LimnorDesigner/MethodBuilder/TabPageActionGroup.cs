/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using LimnorDesigner.Action;

namespace LimnorDesigner.MethodBuilder
{
	public class TabPageActionGroup : TabPage
	{
		private AB_Group _group;
		private MethodDiagramViewerActionGroup _view;
		public TabPageActionGroup(AB_Group group, MethodDiagramViewerActionGroup mview, Control frame)
		{
			_group = group;
			_view = mview;
			this.Text = group.Name;
			frame.Dock = DockStyle.Fill;
			mview.Dock = DockStyle.Fill;
			Controls.Add(frame);
		}
		public AB_Group ActionGroup
		{
			get
			{
				return _group;
			}
		}
		public MethodDiagramViewerActionGroup Viewer
		{
			get
			{
				return _view;
			}
		}
	}
}
