/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using VSPrj;
using System.Xml;
using ProgElements;
using LimnorDesigner.Action;

namespace LimnorDesigner.MenuUtil
{
	public class MethodItemClassPointer : MenuItemDataMethod
	{
		#region constructors and fields
		private ClassPointer _pointer;
		private MethodClass _method;
		public MethodItemClassPointer(string key, Point location, IClass owner)
			: base(key, location, owner)
		{
		}
		public MethodItemClassPointer(string key, IClass owner, MethodClass method)
			: base(key, owner)
		{
			_method = method;
			_pointer = owner.VariableCustomType;
			if (_pointer == null)
			{
				throw new DesignerException("VariableCustomType is null creating MethodItemClassPointer");
			}
		}
		#endregion
		public override bool ExecuteMenuCommand(LimnorProject project, IClass holder, XmlNode node, MultiPanes pane, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			ClassPointer cp = project.GetTypedData<ClassPointer>(pane.Loader.ClassID);
			ActionClass act = new ActionClass(cp);//holder.Host);
			act.ActionMethod = _method.CreatePointer(holder);
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
		public override IAction CreateMethodAction(ILimnorDesignPane designPane, IClass holder, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			ActionClass act = new ActionClass(designPane.RootClass);//holder.Host);
			act.ActionMethod = _method.CreatePointer(holder);
			act.ActionName = act.ActionMethod.DefaultActionName;
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
		public MethodClass Method
		{
			get
			{
				return _method;
			}
		}
		public override string Tooltips
		{
			get { return _method.Description; }
		}
	}
}
