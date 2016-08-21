/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Property;
using LimnorDesigner.Action;
using System.Drawing;
using System.Windows.Forms;
using MathExp;
using ProgElements;
using System.ComponentModel;
using VPL;

namespace LimnorDesigner.MethodBuilder
{
	public abstract class MethodActionForeach : MethodAction
	{
		#region fields and constructors
		public MethodActionForeach(ClassPointer owner)
			: base(owner)
		{
			ActionSubMethodGlobal act = new ActionSubMethodGlobal(owner);
			act.ActionId = (UInt32)Guid.NewGuid().GetHashCode();
			act.ActionType = this.RunAtServer ? EnumWebActionType.Server : EnumWebActionType.Client;
			act.ActionHolder = this;
			AB_SubMethodAction ab = new AB_SubMethodAction(act);
			this.ActionList.Add(ab);
		}
		public MethodActionForeach(ClassPointer owner, ActionSubMethodGlobal act)
			: base(owner)
		{
			act.ActionHolder = this;
			if (act.ActionId == 0)
			{
				act.ActionId = (UInt32)Guid.NewGuid().GetHashCode();
			}
			act.ActionType = this.RunAtServer ? EnumWebActionType.Server : EnumWebActionType.Client;
			this.ActionList.Add(new AB_SubMethodAction(act));
		}
		#endregion

		#region Properties
		[Browsable(false)]
		public override IObjectPointer MethodOwner
		{
			get { return ActionOwner; }
		}
		[Browsable(false)]
		public override uint ExecuterMemberId
		{
			get
			{
				IObjectPointer op = ActionOwner;
				IClass ic = op as IClass;
				while (ic == null && op != null)
				{
					ic = op.Owner as IClass;
					op = op.Owner;
				}
				if (ic != null)
				{
					return ic.MemberId;
				}
				return 0;
			}
		}
		[Browsable(false)]
		public IObjectPointer ActionOwner
		{
			get
			{
				if (this.ActionInstances != null)
				{
					IAction a;
					if (this.ActionInstances.TryGetValue(this.MemberId, out a))
					{
						if (a != null)
						{
							if (a == this)
							{
								return this.RootPointer;
							}
							IObjectPointer ao = a.MethodOwner;
							if (ao != null)
							{
								return ao.Owner;
							}
						}
					}
				}
				return null;
			}
		}
		[Browsable(false)]
		public override EnumObjectDevelopType ObjectDevelopType
		{
			get
			{
				IObjectPointer ao = ActionOwner;
				if (ao != null)
				{
					return ao.ObjectDevelopType;
				}
				return EnumObjectDevelopType.Library;
			}
		}
		public override bool IsStaticAction
		{
			get
			{
				if (this.ActionList != null && this.ActionList.Count > 0)
				{
					AB_SubMethodAction ab = this.ActionList[0] as AB_SubMethodAction;
					if (ab != null)
					{
						ActionSubMethodGlobal act = ab.ActionData as ActionSubMethodGlobal;
						if (act != null)
						{
							return act.IsStatic;
						}
					}
				}
				return false;
			}
		}
		#endregion
		protected override void OnGetSourceValuePointers(UInt32 taskId, EnumWebValueSources scope, List<ISourceValuePointer> list)
		{
			ISourceValuePointer p = this.ActionOwner as ISourceValuePointer;
			if (p != null)
			{
				if (scope == EnumWebValueSources.HasClientValues)
				{
					if (p.IsWebClientValue())
					{
						list.Add(p);
					}
				}
				else if (scope == EnumWebValueSources.HasServerValues)
				{
					if (p.IsWebServerValue())
					{
						list.Add(p);
					}
				}
				else
				{
					list.Add(p);
				}
			}
		}
		private EnumRunContext _origiContext = EnumRunContext.Server;
		public override bool Edit(UInt32 actionBranchId, Rectangle rcStart, ILimnorDesignerLoader loader, Form caller)
		{
			if (Owner == null)
			{
				Owner = loader.GetRootId();
			}
			try
			{
				_origiContext = VPLUtil.CurrentRunContext;
				if (loader.Project.IsWebApplication)
				{
					if (this.RunAt == EnumWebRunAt.Client)
					{
						VPLUtil.CurrentRunContext = EnumRunContext.Client;
					}
					else
					{
						VPLUtil.CurrentRunContext = EnumRunContext.Server;
					}
				}
				else
				{
					VPLUtil.CurrentRunContext = EnumRunContext.Server;
				}
				DlgMethod dlg = this.CreateMethodEditor(rcStart);
				dlg.LoadMethod(this, EnumParameterEditType.ReadOnly);
				if (dlg.EditSubAction(caller))
				{
					IsNewMethod = false;
					loader.GetRootId().SaveAction(this, loader.Writer);
					loader.NotifyChanges();
					return true;
				}
			}
			catch (Exception err)
			{
				MathNode.Log(caller,err);
			}
			finally
			{
				ExitEditor();
				VPLUtil.CurrentRunContext = _origiContext;
			}
			return false;
		}
	}
	public class MethodActionForeachAtServer : MethodActionForeach
	{
		#region fields and constructors
		public MethodActionForeachAtServer(ClassPointer owner)
			: base(owner)
		{
		}
		public MethodActionForeachAtServer(ClassPointer owner, ActionSubMethodGlobal act)
			: base(owner, act)
		{
		}
		#endregion
		public override void SetEditContext()
		{
			if (this.ActionOwner != null && this.ActionOwner.RootPointer != null)
			{
				MethodEditContext.IsWebPage = this.ActionOwner.RootPointer.IsWebPage;
			}
			else
			{
				if (this.Project != null && this.Project.IsWebApplication)
				{
					MethodEditContext.IsWebPage = true;
				}
				else
				{
					MethodEditContext.IsWebPage = false;
				}
			}
			MethodEditContext.UseClientExecuterOnly = false;
			MethodEditContext.UseClientPropertyOnly = false;
			MethodEditContext.UseServerExecuterOnly = true;
		}

		#region Properties
		public override bool RunAtServer
		{
			get { return true; }
		}
		#endregion
		public override EnumWebRunAt RunAt
		{
			get
			{
				return EnumWebRunAt.Server;
			}
		}

		[ReadOnly(true)]
		public override EnumWebRunAt ScopeRunAt
		{
			get
			{
				return EnumWebRunAt.Server;
			}
			set
			{
			}
		}
	}
	public class MethodActionForeachAtClient : MethodActionForeach
	{
		#region fields and constructors
		public MethodActionForeachAtClient(ClassPointer owner)
			: base(owner)
		{
		}
		public MethodActionForeachAtClient(ClassPointer owner, ActionSubMethodGlobal act)
			: base(owner, act)
		{
		}
		#endregion
		public override void SetEditContext()
		{
			MethodEditContext.IsWebPage = true;
			MethodEditContext.UseClientExecuterOnly = true;
			MethodEditContext.UseClientPropertyOnly = true;
			MethodEditContext.UseServerExecuterOnly = false;
			MethodEditContext.UseServerPropertyOnly = false;
		}
		#region Properties
		public override bool RunAtServer
		{
			get { return false; }
		}
		#endregion
		public override EnumWebRunAt RunAt
		{
			get
			{
				return EnumWebRunAt.Client;
			}
		}
		[ReadOnly(true)]
		public override EnumWebRunAt ScopeRunAt
		{
			get
			{
				return EnumWebRunAt.Client;
			}
			set
			{
			}
		}
	}
}
