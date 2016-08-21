/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.Windows.Forms;
using LimnorDesigner.Action;
using System.Drawing;
using ProgElements;
using System.ComponentModel;
using VPL;
using LimnorDesigner.Property;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// load a single thread of actions into the designer
	/// </summary>
	class ActionGroupDiagramViewer : MethodDiagramViewer
	{
		public ActionGroupDiagramViewer()
		{
		}
		[Browsable(false)]
		public override IActionsHolder ActionsHolder
		{
			get
			{
				TopActionGroupDiagramViewer top = ParentEditor as TopActionGroupDiagramViewer;
				if (top != null)
				{
					return top.ActionsHolder;
				}
				return base.ActionsHolder;
			}
		}
		public override void LoadMethod()
		{
			ActionGroupDesignerHolder holder = DesignerHolder as ActionGroupDesignerHolder;
			bool b = holder.DisableUndo;
			holder.DisableUndo = true;
			//
			this.Name = VPLUtil.NameToCodeName(VPLUtil.FormCodeNameFromname(holder.Actions.Name));
			this.Site.Name = Name;
			Description = holder.Actions.Description;
			//
			ReloadActions();
			//
			RemoveDisconnectedPorts();
			//
			holder.DisableUndo = b;
			//
		}
		public override List<ComponentIcon> IconList
		{
			get
			{
				ActionGroupDesignerHolder holder = DesignerHolder as ActionGroupDesignerHolder;
				ISubMethod sa = holder.Actions as ISubMethod;
				if (sa != null)
				{
					return sa.ComponentIconList;
				}
				return null;
			}
		}
		public void ReloadActions()
		{
			ActionGroupDesignerHolder holder = DesignerHolder as ActionGroupDesignerHolder;
			AB_Squential actions = holder.Actions;
			bool b = holder.DisableUndo;
			holder.DisableUndo = true;
			holder.ClearAllComponent();
			Controls.Clear();

			BranchList bl = actions.ActionList;
			LoadActions(bl);

			//load component icons
			List<ComponentIcon> iconList = IconList;
			if (iconList == null)
			{
				iconList = new List<ComponentIcon>();
			}
			List<ComponentIcon> invalids = new List<ComponentIcon>();
			foreach (ComponentIcon ci in iconList)
			{
				if (ci.ClassPointer == null)
				{
					invalids.Add(ci);
				}
			}
			foreach (ComponentIcon ci in invalids)
			{
				iconList.Remove(ci);
			}
			//find root
			ClassPointer root = holder.Designer.GetRootId();
			List<IClass> objList;
			if (Method.IsStatic)
			{
				objList = new List<IClass>();
			}
			else
			{
				objList = root.GetClassList();
			}
			SubMethodInfoPointer smi = null;
			if (Method.SubMethod.Count > 0)
			{
				smi = Method.SubMethod.Peek() as SubMethodInfoPointer;
			}
			//initialize existing icons, creating ComponentIcon.ClassPointer
			foreach (ComponentIcon ic in iconList)
			{
				ic.SetDesigner(holder.Designer);
				ComponentIconPublic cip = ic as ComponentIconPublic;
				if (cip == null)
				{
					ComponentIconLocal lv = ic as ComponentIconLocal;
					if (lv != null)
					{
						lv.ReadOnly = true;
						if (!ParentEditor.LocalVariableDeclared(lv))
						{
							lv.ScopeGroupId = actions.BranchId;
						}
					}
					else
					{
					}
					if (smi != null)
					{
						ParameterClassSubMethod sm = ic.ClassPointer as ParameterClassSubMethod;
						if (sm != null)
						{
							if (sm.ActionId == 0)
							{
								sm.ActionId = smi.ActionOwner.ActionId;
							}
							ParameterClass pc = ic.ClassPointer as ParameterClass;
							if (pc != null && pc.ObjectType != null)
							{
								if (pc.ObjectType.IsGenericParameter)
								{
									if (pc.ConcreteType == null)
									{
										if (smi.ActionOwner.MethodOwner != null)
										{
											CustomPropertyPointer cpp = smi.ActionOwner.MethodOwner.Owner as CustomPropertyPointer;
											if (cpp != null)
											{
												pc.ConcreteType = cpp.GetConcreteType(pc.ObjectType);
											}
										}
									}
								}
							}
						}
					}
				}
				else
				{
				}
			}
			//add new public component icons
			int x0 = 10;
			int y0 = 30;
			int x = x0;
			int y = y0;
			int dx = 10;
			int dy = 10;
			foreach (IClass c in objList)
			{
				bool bFound = false;
				foreach (ComponentIcon ic in iconList)
				{
					if (ic.ClassPointer == null)
					{
					}
					else
					{
						if (ic.ClassPointer.IsSameObjectRef(c))
						{
							bFound = true;
							break;
						}
					}
				}
				if (!bFound)
				{
					ComponentIconPublic cip = new ComponentIconPublic(holder.Designer, c, Method);
					cip.Location = new Point(x, y);
					y += dy;
					y += cip.Height;
					if (y >= this.Height)
					{
						y = y0;
						x += dx;
						x += cip.Width;
					}
					iconList.Add(cip);
				}
			}
			//add new local component icons
			List<ComponentIcon> picons = ParentEditor.IconList;
			foreach (ComponentIcon lv in picons)
			{
				ComponentIconPublic cip0 = lv as ComponentIconPublic;
				if (cip0 == null)
				{
					bool bFound = false;
					foreach (ComponentIcon ic in iconList)
					{
						if (ic.ClassPointer.IsSameObjectRef(lv.ClassPointer))
						{
							bFound = true;
							break;
						}
					}
					if (!bFound)
					{
						ComponentIcon cip = (ComponentIcon)lv.Clone();
						cip.SetDesigner(holder.Designer);
						cip.ReadOnly = true;
						cip.Location = new Point(x, y);
						y += dy;
						y += cip.Height;
						if (y >= this.Height)
						{
							y = y0;
							x += dx;
							x += cip.Width;
						}
						iconList.Add(cip);
					}
				}
			}
			//add parameters
			if (Method.ParameterCount > 0)
			{
				foreach (ParameterClass c in Method.Parameters)
				{
					bool bFound = false;
					foreach (ComponentIcon ic in iconList)
					{
						if (ic.ClassPointer.IsSameObjectRef(c))
						{
							bFound = true;
							break;
						}
					}
					if (!bFound)
					{
						ComponentIconParameter cip = new ComponentIconParameter(holder.Designer, c, Method);
						cip.Location = new Point(x, y);
						y += dy;
						y += cip.Height;
						if (y >= this.Height)
						{
							y = y0;
							x += dx;
							x += cip.Width;
						}
						iconList.Add(cip);
					}
				}
			}
			//add action parameters
			ISingleAction sa = actions as ISingleAction;
			if (sa != null && sa.ActionData != null && sa.ActionData.ActionMethod != null && sa.ActionData.ActionMethod.MethodPointed.MethodParameterTypes != null)
			{
				List<ParameterClass> plist = new List<ParameterClass>();
				SubMethodInfoPointer smp = sa.ActionData.ActionMethod.MethodPointed as SubMethodInfoPointer;
				if (smp != null)
				{
					foreach (ParameterClassSubMethod p in smp.Parameters)
					{
						plist.Add(p);
					}
				}
				else
				{
					foreach (IParameter p in sa.ActionData.ActionMethod.MethodPointed.MethodParameterTypes)
					{
						ParameterClass c = p as ParameterClass;
						if (c == null)
						{
							try
							{
								c = sa.ActionData.ActionMethod.MethodPointed.GetParameterType(p.ParameterID) as ParameterClass;
								if (c == null)
								{
									DesignUtil.WriteToOutputWindowAndLog("Cannot get ParameterClass {0} for method {1} of {2}.", p.Name, sa.ActionData.ActionMethod.MethodName, sa.ActionData.ActionMethod.MethodPointed.GetType());
									continue;
								}
							}
							catch (Exception errp)
							{
								DesignUtil.WriteToOutputWindowAndLog(errp, "Cannot get ParameterClass {0} for method {1} of {2}", p.Name, sa.ActionData.ActionMethod.MethodName, sa.ActionData.ActionMethod.MethodPointed.GetType());
								continue;
							}
						}
						if (c != null)
						{
							plist.Add(c);
						}
					}
				}
				foreach (ParameterClass c in plist)
				{
					bool bFound = false;
					foreach (ComponentIcon ic in iconList)
					{
						if (ic.ClassPointer.IsSameObjectRef(c))
						{
							ParameterClass c0 = ic.ClassPointer as ParameterClass;
							c0.ReadOnly = true;
							c0.Description = c.Description;
							bFound = true;
							break;
						}
					}
					if (!bFound)
					{
						ComponentIcon cip;
						ActionBranchParameter abp = c as ActionBranchParameter;
						if (abp != null)
						{
							cip = new ComponentIconActionBranchParameter(actions);
							cip.ClassPointer = abp;
							cip.SetDesigner(holder.Designer);
						}
						else
						{
							cip = new ComponentIconParameter(holder.Designer, c, Method);
						}
						cip.Location = new Point(x, y);
						y += dy;
						y += cip.Height;
						if (y >= this.Height)
						{
							y = y0;
							x += dx;
							x += cip.Width;
						}
						iconList.Add(cip);
					}
				}
			}
			//add icons
			holder.AddControlsToIconsHolder(iconList.ToArray());
			foreach (ComponentIcon ic in iconList)
			{
				ComponentIconPublic cip = ic as ComponentIconPublic;
				if (cip == null)
				{
					if (ic.Left < 0)
						ic.Left = 2;
					if (ic.Top < 0)
						ic.Top = 2;
					ic.BringToFront();
					ic.RefreshLabelPosition();
					ic.RefreshLabelText();
					ComponentIconLocal cil = ic as ComponentIconLocal;
					if (cil != null)
					{
						cil.HookNameChecker();
					}
				}
			}
			//}
			InitializeInputTypes();
			ValidateControlPositions();
			holder.DisableUndo = b;
		}
		/// <summary>
		/// load a single threaded actions into the designer
		/// </summary>
		/// <param name="actions">actions to be loaded</param>
		protected override void OnLoadActionList(BranchList actions)
		{
			if (actions != null)
			{
				actions.LoadToDesignerAsSingleThread(this);
			}
		}
		public override void CancelEdit()
		{
			foreach (Control c in Controls)
			{
				ComponentIconLocal cil = c as ComponentIconLocal;
				if (cil != null)
				{
					LocalVariable.RemoveLocalVariable(cil.LocalPointer);
				}
			}
		}
		public override bool Save()
		{
			ActionGroupDesignerHolder holder = DesignerHolder as ActionGroupDesignerHolder;
			try
			{
				holder.Actions.Name = this.Site.Name;
				holder.Actions.Description = this.Description;
				//
				holder.Actions.ActionList = ExportActions();
				//
				///
				Form f = this.FindForm();
				if (f != null)
				{
					holder.Actions.EditorBounds = f.Bounds;
				}
				return true;
			}
			catch (Exception e)
			{
				MathNode.Log(this.FindForm(),e);
				return false;
			}
		}
	}
}
