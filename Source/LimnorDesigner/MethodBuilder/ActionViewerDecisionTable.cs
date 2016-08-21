/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using LimnorDesigner.Action;
using System.Windows.Forms;
using VSPrj;
using System.Drawing;

namespace LimnorDesigner.MethodBuilder
{
	class ActionViewerDecisionTable : ActionViewer
	{
		#region fields and constructors
		public ActionViewerDecisionTable()
		{
		}
		#endregion
		#region Properties
		[Browsable(false)]
		protected override bool CanEditAction
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		protected override bool CanReplaceAction
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public override bool NameReadOnly
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public virtual string ActionDisplay
		{
			get
			{
				AB_DecisionTableActions av = ActionObject as AB_DecisionTableActions;
				if (av == null)
				{
					return ActionName;
				}
				return av.Name;
			}
		}
		[ParenthesizePropertyName(true)]
		[Description("Name of the action")]
		public override string ActionName
		{
			get
			{
				return base.ActionName;
			}
			set
			{
				AB_DecisionTableActions av = ActionObject as AB_DecisionTableActions;
				if (av == null)
				{
					base.ActionName = value;
				}
				else
				{
					IMethodDiagram p = this.Parent as IMethodDiagram;
					if (!NameReadOnly && !IsLoading)
					{
						if (p != null)
						{
							if (DesignUtil.IsActionNameInUse(p.RootXmlNode, value, 0))
							{
								MessageBox.Show(this, "The action name is in use", "Set action name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
								return;
							}
						}
					}
					av.Name = value;
					if (base.ActionName != value)
					{
						base.ActionName = value;
						OnActionNameChanged();
					}
				}
				this.Refresh();
			}
		}
		[Description("Indicate whether this action may generate a value which can be passed to the next action.")]
		public bool HasOutput
		{
			get
			{
				return ActionObject.HasOutput;
			}
		}
		[Description("Indicate the type of the data this action may generate and pass to the next action")]
		public string OutputType
		{
			get
			{
				return ActionObject.OutputType.DataTypeName;
			}
		}
		public EnumIconDrawType IconLayout
		{
			get
			{
				AB_DecisionTableActions av = ActionObject as AB_DecisionTableActions;
				if (av != null)
				{
					return av.IconLayout;
				}
				return EnumIconDrawType.Left;
			}
			set
			{
				AB_DecisionTableActions av = ActionObject as AB_DecisionTableActions;
				if (av != null)
				{
					av.IconLayout = value;
				}
			}
		}
		#endregion
		#region Methods
		protected virtual void OnActionNameChanged()
		{
		}
		protected override void OnEditAction()
		{
			MethodDiagramViewer mv = this.DiagramViewer;
			if (mv != null)
			{
				LimnorProject project = mv.Project;
				AB_DecisionTableActions av = this.ActionObject as AB_DecisionTableActions;
				//string name = av.Name;
				DlgDecisionTable dlg = new DlgDecisionTable();
				dlg.LoadData(av.DecisionTable, mv.Method, project, mv.DesignerHolder);
				if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
				{
					av.DecisionTable = dlg.Result;
					mv.Changed = true;
				}
			}
		}
		protected override void OnImportAction()
		{
			base.OnImportAction();
			AB_DecisionTableActions av = this.ActionObject as AB_DecisionTableActions;
			if (av != null)
			{
				ActionName = av.Name;
				Description = av.Description;
			}
			else
			{
				DesignUtil.WriteToOutputWindowAndLog("Decision table not found");
			}
		}
		protected virtual void OnPaintAction(PaintEventArgs e)
		{
			int nLeft = 0;
			//draw action
			AB_DecisionTableActions av = this.ActionObject as AB_DecisionTableActions;
			if (ActionImage == null)
			{
				SetActionImage(Resources.decisionTable);
			}
			Bitmap img = ActionImage;
			switch (av.IconLayout)
			{
				case EnumIconDrawType.Fill:
					if (this.Size.Width > 4 && this.Size.Height > 4)
					{
						e.Graphics.DrawImage(img, 1, 1, this.Size.Width - 4, this.Size.Height - 4);
						nLeft = 1;
					}
					break;
				case EnumIconDrawType.Left:
					e.Graphics.DrawImage(img, 1, (this.Size.Height - img.Height) / 2, img.Size.Width, img.Size.Height);
					nLeft = 1 + img.Width;
					break;
				case EnumIconDrawType.Center:
					e.Graphics.DrawImage(img, (this.Size.Width - img.Width) / 2, (this.Size.Height - img.Height) / 2, img.Size.Width, img.Size.Height);
					nLeft = 1;
					break;
			}
			SizeF sz = e.Graphics.MeasureString(this.ActionDisplay, TextFont);
			float x = (Size.Width - sz.Width - nLeft) / 2 + nLeft;
			float y = (Size.Height - sz.Height) / 2;
			if (x < 0)
				x = 0;
			if (y < 0)
				y = 0;
			e.Graphics.DrawString(this.ActionDisplay, TextFont, TextBrush, x, y);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			OnPaintAction(e);
		}
		#endregion
	}
}
