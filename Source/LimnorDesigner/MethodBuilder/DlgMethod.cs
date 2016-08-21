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
using MathExp;
using LimnorDesigner.Event;
using LimnorUI;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// the constructors will setup the diagram viewer
	/// </summary>
	public partial class DlgMethod : Form, IMethodDialog
	{
		MethodDesignerHolder holder;
		ILimnorDesigner _designer;
		public DlgMethod(ILimnorDesigner designer, Rectangle rcStart, UInt32 scopeId)
			: base()
		{
			setMembers(designer, typeof(MethodDesignerHolder), null, scopeId);
		}
		/// <summary>
		/// sub-editor
		/// </summary>
		/// <param name="designer"></param>
		/// <param name="designerType"></param>
		/// <param name="rcStart"></param>
		/// <param name="parentEditor"></param>
		public DlgMethod(ILimnorDesigner designer, Type designerType, Rectangle rcStart, MethodDiagramViewer parentEditor, UInt32 scopeId)
			: base(/*rcStart*/)
		{
			setMembers(designer, designerType, parentEditor, scopeId);
		}
		private void setMembers(ILimnorDesigner designer, Type designerType, MethodDiagramViewer parentEditor, UInt32 scopeId)
		{
			_designer = designer;
			InitializeComponent();
			holder = (MethodDesignerHolder)(Activator.CreateInstance(designerType, _designer, scopeId));
			holder.Dock = DockStyle.Fill;
			holder.SetForSubMethod(parentEditor);
			if (parentEditor != null)
			{
				parentEditor.Method.CurrentSubEditor = holder;
			}
			this.Controls.Add(holder);
			this.Load += DlgMethod_Load;
		}

		void DlgMethod_Load(object sender, EventArgs e)
		{
			holder.SetRootSelection();
		}
		protected override void OnActivated(EventArgs e)
		{
			FormProgress.HideProgress();
			base.OnActivated(e);
		}
		public bool EditSubAction(Form caller)
		{
			ActionViewerSubMethod av = null;
			foreach (Control c in holder.CurrentDiagramViewer.Controls)
			{
				av = c as ActionViewerSubMethod;
				if (av != null)
				{
					break;
				}
			}
			if (av != null)
			{
				return av.EditAction(caller);
			}
			return false;
		}
		public void SetNameReadOnly()
		{
			holder.SetNameReadOnly();
		}
		public void SetSubScopeId(UInt32 id)
		{
			holder.SetSubScopeId(id);
		}
		public void SetAttributesReadOnly(bool readOnly)
		{
			holder.SetAttributesReadOnly(readOnly);
		}
		public void SetForSubScope()
		{
			holder.SetForSubScope();
		}
		public ActionBranch SearchBranchById(UInt32 id)
		{
			BranchList bl = holder.ExportActions();
			if (bl != null)
			{
				return bl.SearchBranchById(id);
			}
			return null;
		}
		public void LoadMethod(MethodClass method, EnumParameterEditType parameterEditType)
		{
			if (holder.MainDiagramViewer.ParentEditor == null)
			{
				method.SetEditContext();
			}
			holder.LoadMethod(method, parameterEditType);
			EventHandlerMethod ehm = method as EventHandlerMethod;
			if (ehm != null)
			{
				this.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					Resources.TitleEditEventMethod, ehm.Event.Name, method.MethodName);
			}
			else
			{
				this.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					Resources.TitleEditMethod, method.MethodName);
			}
		}
		public void LoadActions(AB_Squential actions)
		{
			holder.LoadActions(actions);
			this.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture,
				Resources.TitleEditMethod, actions.Name);
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			if (holder.Changed && this.DialogResult != DialogResult.OK)
			{
				e.Cancel = UIUtil.AskCancelCloseDialog(this);
				if (!e.Cancel)
				{
					holder.CancelEdit();
				}
			}
			if (!e.Cancel)
			{
				MethodEditContext.IsWebPage = false;
				holder.AbortTest();
				holder.OnClosing();
				base.OnClosing(e);
			}
		}
		/// <summary>
		/// when editing the action string, this is the result
		/// </summary>
		public AB_Squential ActionResult
		{
			get
			{
				return holder.Actions;
			}
		}
		public List<ComponentIcon> ComponentIcons
		{
			get
			{
				return holder.ComponentIcons;
			}
		}

		#region IMethodDialog Members

		public MethodDesignerHolder GetEditor()
		{
			return holder;
		}

		#endregion
	}
}
