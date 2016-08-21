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
using System.Drawing;
using LimnorDesigner.Action;
using MathExp;
using VPL;

namespace LimnorDesigner.MethodBuilder
{
	public class MethodDiagramViewerActionGroup : MethodDiagramViewer
	{
		private AB_Group _group;

		public MethodDiagramViewerActionGroup()
		{

		}
		~MethodDiagramViewerActionGroup()
		{
		}
		public void LoadActionGroup(AB_Group group)
		{
			bool b = this.DesignerHolder.DisableUndo;
			DesignerHolder.DisableUndo = true;
			DesignerHolder.SetLoading(true);
			_group = group;
			this.Name = VPLUtil.FormCodeNameFromname(_group.Name);
			this.Site.Name = Name;
			Description = _group.Description;
			//
			LoadActions(_group.ActionList);
			//
			RemoveDisconnectedPorts();
			InitializeInputTypes();
			//
			DesignerHolder.DisableUndo = b;
			DesignerHolder.SetLoading(false);
		}
		public override bool Save()
		{
			try
			{
				if (!ReadOnly)
				{
					_group.ActionList = ExportActions();
				}
				return true;
			}
			catch (Exception e)
			{
				MathNode.Log(this.FindForm(),e);
				return false;
			}
		}
		protected override bool IsMain
		{
			get
			{
				return false;
			}
		}
	}
}
