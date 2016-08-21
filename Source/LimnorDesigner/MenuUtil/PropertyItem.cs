/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Parser;
using VPL;
using System.Drawing;
using ProgElements;
using VSPrj;
using System.Xml;
using System.ComponentModel;
using LimnorDesigner.Action;

namespace LimnorDesigner.MenuUtil
{
	/// <summary>
	/// for lib-type
	/// </summary>
	public class PropertyItem : MenuItemDataProperty
	{
		private Type _type;
		private PropertyDescriptor _val;
		private string _tooltips;
		public PropertyItem(string key, IClass owner, PropertyDescriptor ei)
			: base(key, owner)
		{
			if (owner.VariableCustomType != null)
			{
				_type = owner.VariableCustomType.BaseClassType;
			}
			else if (owner.VariableLibType != null)
			{
				_type = owner.VariableLibType;
			}
			else if (owner.VariableWrapperType != null)
			{
				_type = owner.VariableWrapperType.WrappedType;
			}
			else
			{
				throw new DesignerException("Invalid object type creating PropertyItem");
			}
			_val = ei;
		}
		public Type ObjectType
		{
			get
			{
				return _type;
			}
		}
		public PropertyDescriptor Value
		{
			get
			{
				return _val;
			}
			set
			{
				_val = value;
			}
		}
		public override bool ExecuteMenuCommand(LimnorProject project, IClass holder, XmlNode node, MultiPanes pane, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			PropertyPointer pp = new PropertyPointer();
			pp.SetPropertyInfo(_val);
			pp.Owner = Owner;
			return DesignUtil.CreateSetPropertyAction(pp, pane.Loader.DesignPane, scopeMethod, actsHolder, pane.FindForm()) != null;
		}
		public override ActionClass CreateSetPropertyAction(ILimnorDesignPane designPane, IClass holder, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			PropertyPointer pp = new PropertyPointer();
			pp.SetPropertyInfo(_val);
			pp.Owner = Owner;
			return DesignUtil.CreateSetPropertyAction(pp, designPane, scopeMethod, actsHolder, designPane.PaneHolder.FindForm());
		}
		#region IHoverListItem Members

		public override string Tooltips
		{
			get
			{
				if (string.IsNullOrEmpty(_tooltips))
				{
					Type t = VPLUtil.GetObjectType(ObjectType);
					PropertyInfo pi = t.GetProperty(_val.Name);
					_tooltips = PMEXmlParser.GetPropertyDescription(t, pi);
				}
				return _tooltips;
			}
		}

		#endregion
	}
}
