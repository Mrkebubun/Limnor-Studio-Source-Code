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
using LimnorDesigner.MethodBuilder;
using VSPrj;
using System.Xml;
using ProgElements;
using LimnorDesigner.Action;

namespace LimnorDesigner.MenuUtil
{
	public class MethodItemWrapper : MenuItemDataMethod
	{
		IClassWrapper _type;
		public MethodItemWrapper(string key, IClass owner, IMethodWrapper method)
			: base(key, owner)
		{
			_type = owner.VariableWrapperType;
			if (_type == null)
			{
				throw new DesignerException("Cannot create MethodItemWrapper: VariableWrapperType is null");
			}
		}
		public override bool ExecuteMenuCommand(LimnorProject project, IClass holder, XmlNode node, MultiPanes pane, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			throw new NotImplementedException("MethodItemWrapper.ExecuteMenuCommand");
		}
		public override IAction CreateMethodAction(ILimnorDesignPane designPane, IClass holder, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			throw new NotImplementedException("MethodItemWrapper.CreateMethodAction");
		}
		public override string Tooltips
		{
			get
			{
				return _type.Description;
			}
		}
	}
}
