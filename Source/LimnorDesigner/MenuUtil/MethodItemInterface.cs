using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using LimnorDesigner.Interface;
using ProgElements;
using LimnorDesigner.Action;
using VSPrj;
using System.Xml;

namespace LimnorDesigner.MenuUtil
{
	public class MethodItemInterface : MenuItemDataMethod
	{
		#region constructors and fields
		private ClassPointer _pointer;
		private InterfaceElementMethod _method;
		public MethodItemInterface(string key, Point location, IClass owner)
			: base(key, location, owner)
		{
		}
		public MethodItemInterface(string key, IClass owner, InterfaceElementMethod method)
			: base(key, owner)
		{
			_method = method;
			_pointer = owner.VariableCustomType;
			if (_pointer == null)
			{
				throw new DesignerException("VariableCustomType is null creating MethodItemInterface");
			}
		}
		#endregion

		public override IAction CreateMethodAction(ILimnorDesignPane designPane, IClass holder, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			ActionClass act = new ActionClass(designPane.RootClass);//holder.Host);
			act.ActionMethod = _method.CreatePointer(act);
			act.ActionName = act.ActionMethod.DefaultActionName;//.ReferenceName;
			act.ActionHolder = actsHolder;
			ClassPointer owner = holder.Host as ClassPointer;
			if (owner == null)
			{
				owner = Owner.Host as ClassPointer;
				if (owner == null)
				{
					owner = _pointer;
				}
			}
			if (owner.CreateNewAction(act, designPane.Loader.Writer, scopeMethod, designPane.PaneHolder.FindForm()))
			{
				return act;
			}
			return null;
		}

		public override string Tooltips
		{
			get { return string.Empty; }
		}

		public override bool ExecuteMenuCommand(LimnorProject project, IClass holder, XmlNode node, MultiPanes pane, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			ClassPointer cp = project.GetTypedData<ClassPointer>(pane.Loader.ClassID);
			ActionClass act = new ActionClass(cp);//holder.Host);
			act.ActionMethod = _method.CreatePointer(act);
			act.ActionName = act.ActionMethod.DefaultActionName;//.ReferenceName;
			act.ActionHolder = actsHolder;
			ClassPointer owner = holder.Host as ClassPointer;
			if (owner == null)
			{
				owner = Owner.Host as ClassPointer;
				if (owner == null)
				{
					owner = _pointer;
				}
			}
			return owner.CreateNewAction(act, pane.Loader.Writer, scopeMethod, pane.FindForm());
		}
	}
}
