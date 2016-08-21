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
using System.Windows.Forms;
using System.Drawing;
using LimnorDesigner.Action;
using System.Xml;
using XmlSerializer;
using VSPrj;
using VPL;
using System.Drawing.Design;
using LimnorDesigner.Property;

namespace LimnorDesigner.MethodBuilder
{
	public class ActionViewerSingleAction : ActionViewer
	{
		#region fields and constructors
		public ActionViewerSingleAction()
		{
			AddPropertyName("HasOutput");
			AddPropertyName("OutputType");
			AddPropertyName("IconLayout");
			RemovePropertyName("Description");
		}
		#endregion
		#region Properties
		[Browsable(false)]
		protected override bool CanEditAction
		{
			get
			{
				ISingleAction av = ActionObject as ISingleAction;
				if (av == null)
				{
					return false;
				}
				else
				{
					return av.CanEditAction;
				}
			}
		}
		[Browsable(false)]
		protected override bool CanReplaceAction
		{
			get
			{
				ISingleAction av = ActionObject as ISingleAction;
				if (av == null)
				{
					return false;
				}
				if (av.ActionData == null)
				{
					return false;
				}
				return av.ActionData.IsPublic;
			}
		}
		[Browsable(false)]
		public override bool NameReadOnly
		{
			get
			{
				ISingleAction av = ActionObject as ISingleAction;
				if (av == null)
				{
					return true;
				}
				if (av.ActionData == null)
				{
					return true;
				}
				return av.ActionData.IsPublic;
			}
		}
		[Browsable(false)]
		public virtual string ActionDisplay
		{
			get
			{
				ISingleAction av = ActionObject as ISingleAction;
				if (av == null)
				{
					return ActionName;
				}
				if (av.ActionData == null)
				{
					return ActionName;
				}
				return av.ActionData.Display;
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
				ISingleAction av = ActionObject as ISingleAction;
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
							if (DesignUtil.IsActionNameInUse(p.RootXmlNode, value, av.ActionId.ActionId))
							{
								MessageBox.Show(this, "The action name is in use", "Set action name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
								return;
							}
						}
					}
					av.ActionData.ActionName = value;
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
				ISingleAction av = ActionObject as ISingleAction;
				if (av != null)
				{
					return av.IconLayout;
				}
				return EnumIconDrawType.Left;
			}
			set
			{
				ISingleAction av = ActionObject as ISingleAction;
				if (av != null)
				{
					av.IconLayout = value;
				}
			}
		}
		#endregion
		#region Methods
		public void OnLocalVariableRename(LocalVariable lv)
		{
			ISingleAction av = ActionObject as ISingleAction;
			if (av.ActionData != null)
			{
				if (av.ActionData.ReturnReceiver != null)
				{
					if (lv.IsSameObjectRef(av.ActionData.ReturnReceiver))
					{
						LocalVariable lv2 = av.ActionData.ReturnReceiver as LocalVariable;
						lv2.Name = lv.Name;
						av.ActionData.ResetDisplay();
						this.Refresh();
					}
				}
			}
		}
		public override void ResetDisplay()
		{
			ISingleAction av = ActionObject as ISingleAction;
			if (av.ActionData != null)
			{
				av.ActionData.ResetDisplay();
				this.Refresh();
			}
		}
		protected virtual void OnActionNameChanged()
		{
		}
		protected override PropertyDescriptor OnProcessProperty(PropertyDescriptor p)
		{
			if (string.CompareOrdinal(p.Name, "OutputType") == 0)
			{
				ISingleAction av = this.ActionObject as ISingleAction;
				if (av != null && av.ActionData != null && av.ActionData.ActionMethod != null)
				{
					GetterClass gc = av.ActionData.ActionMethod as GetterClass;
					if (gc != null)
					{
						PropertyDescriptor p0 = new ReadOnlyPropertyDesc(p);
						return p0;
					}
					else
					{
						SetterClass sc = av.ActionData.ActionMethod as SetterClass;
						if (sc != null)
						{
							PropertyDescriptor p0 = new ReadOnlyPropertyDesc(p);
							return p0;
						}
					}
				}
			}
			return p;
		}
		protected override void OnReplaceAction()
		{
			MethodDiagramViewer mv = this.Parent as MethodDiagramViewer;
			if (mv != null)
			{
				ISingleAction av = this.ActionObject as ISingleAction;
				List<IAction> actReplaceList = DesignUtil.SelectAction(mv.DesignerHolder.Loader, av.ActionData, null, false, mv.DesignerHolder.Method, mv.ActionsHolder, this.FindForm());
				if (actReplaceList != null && actReplaceList.Count > 0)
				{
					av.ActionId = new TaskID(actReplaceList[0].WholeActionId);
					av.ActionData = actReplaceList[0];
					IMethodDiagram p = this.Parent as IMethodDiagram;
					if (p != null)
					{
						p.OnActionNameChanged(av.ActionData.ActionName, av.ActionId.WholeTaskId);
					}
				}
			}
		}
		protected override void OnEditAction()
		{
			MethodDiagramViewer mv = this.DiagramViewer;
			if (mv != null)
			{
				ILimnorDesignerLoader loader = mv.DesignerHolder.Designer as ILimnorDesignerLoader;
				ISingleAction av = this.ActionObject as ISingleAction;
				string name = av.ActionData.ActionName;
				FormProgress.HideProgress();
				if (av.ActionData.Edit(loader.Writer, mv.Method, this.FindForm(), false))
				{
					if (name != av.ActionData.ActionName)
					{
						IMethodDiagram p = this.Parent as IMethodDiagram;
						if (p != null)
						{
							p.OnActionNameChanged(av.ActionData.ActionName, av.ActionId.WholeTaskId);
						}
					}
					mv.Changed = true;
				}
			}
		}
		protected override void OnImportAction()
		{
			base.OnImportAction();
			ISingleAction av = this.ActionObject as ISingleAction;
			if (av.ActionData != null)
			{
				ActionName = av.ActionData.ActionName;
				Description = av.ActionData.Description;
				MethodDiagramViewer mv = this.Parent as MethodDiagramViewer;
				List<ParameterValue> ps = av.ActionData.ParameterValues;
				if (ps != null && ps.Count > 0)
				{
					foreach (ParameterValue p in ps)
					{
						if (p != null)
						{
							p.SetCustomMethod(mv.DesignerHolder.Method);
						}
					}
				}
			}
			else
			{
				DesignUtil.WriteToOutputWindowAndLog("Action data for {0} not found for [{1}]. You may delete the action from the method and re-create it.", av.ActionId, this.ActionName);
			}
		}

		protected virtual void OnPaintAction(PaintEventArgs e)
		{
			int nLeft = 0;
			//draw action
			ISingleAction av = this.ActionObject as ISingleAction;
			Bitmap img = ActionImage;
			if (img != null)
			{
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
			}
			if (av.ShowText)
			{
				SizeF sz = e.Graphics.MeasureString(this.ActionDisplay, TextFont);
				float x = (Size.Width - sz.Width - nLeft) / 2 + nLeft;
				float y = (Size.Height - sz.Height) / 2;
				if (x < 0)
					x = 0;
				if (y < 0)
					y = 0;
				e.Graphics.DrawString(this.ActionDisplay, TextFont, TextBrush, x, y);
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			OnPaintAction(e);
		}
		#endregion
	}
}
