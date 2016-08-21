/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VSPrj;
using LimnorDesigner.Action;
using System.Windows.Forms;
using MathExp;
using System.Drawing;

namespace LimnorDesigner.MethodBuilder
{
	public class ActionViewerSubMethod : ActionViewerSingleAction
	{
		public ActionViewerSubMethod()
		{

		}
		private Form _caller;
		public bool EditAction(Form caller)
		{
			_caller = caller;
			this.DiagramViewer.Changed = false;
			OnEditAction();
			if (this.DiagramViewer.Changed)
			{
				this.DiagramViewer.Save();
				return true;
			}
			return false;
		}
		protected override void OnEditAction()
		{
			MethodDiagramViewer mv = this.DiagramViewer;
			if (mv != null)
			{
				AB_Squential av = this.ActionObject as AB_Squential;

				MethodDesignerHolder holder = mv.DesignerHolder;
				MethodClass mc = av.Method;
				DlgMethod dlg = mc.CreateSubMethodEditor(typeof(ActionGroupDesignerHolder), this.Parent.RectangleToScreen(this.Bounds), mv, av.BranchId);
				SubMethodInfoPointer smi = null;
				try
				{
					ISingleAction sa = av as ISingleAction;
					if (sa != null)
					{
						smi = sa.ActionData.ActionMethod as SubMethodInfoPointer;
						if (smi != null)
						{
							AB_SubMethodAction smb = this.ActionObject as AB_SubMethodAction;
							smi.CreateParameters(smb);
							av.Method.SubMethod.Push(smi);
						}
					}
					//
					dlg.LoadActions(av);
					if (_caller == null)
					{
						_caller = this.FindForm();
					}
					if (dlg.ShowDialog(_caller) == DialogResult.OK)
					{
						AB_SubMethodAction abs = this.ActionObject as AB_SubMethodAction;
						abs.CopyActionsFrom(dlg.ActionResult, dlg.ComponentIcons);
						mv.Changed = true;
					}
				}
				catch (Exception err)
				{
					MathNode.Log(this.FindForm(),err);
				}
				finally
				{
					if (smi != null)
					{
						if (av.Method.SubMethod.Count > 0)
						{
							av.Method.SubMethod.Pop();
						}
					}
					mc.CurrentSubEditor = null;
				}
			}
		}
	}
}
