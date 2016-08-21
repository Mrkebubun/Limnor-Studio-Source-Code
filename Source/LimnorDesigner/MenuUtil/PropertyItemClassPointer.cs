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
using LimnorDesigner.Property;
using System.Drawing;
using VSPrj;
using System.Xml;
using ProgElements;
using LimnorDesigner.Action;

namespace LimnorDesigner.MenuUtil
{
	public class PropertyItemClassPointer : MenuItemDataProperty
	{
		#region constructors and fields
		private ClassPointer _pointer;
		private PropertyClass _property;
		public PropertyItemClassPointer(string key, Point location, IClass owner)
			: base(key, location, owner)
		{
			_pointer = owner.VariableCustomType;
			if (_pointer == null)
			{
				throw new DesignerException("null VariableCustomType creating PropertyItemClassPointer");
			}
		}
		public PropertyItemClassPointer(string key, IClass owner, PropertyClass property)
			: base(key, owner)
		{
			_property = (PropertyClass)property.Clone();
			_property.SetHolder(owner);
			_pointer = owner.VariableCustomType;
			if (_pointer == null)
			{
				throw new DesignerException("null VariableCustomType creating PropertyItemClassPointer");
			}
		}
		#endregion
		public override bool ExecuteMenuCommand(LimnorProject project, IClass holder, XmlNode node, MultiPanes pane, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			return MenuItemDataPropertySelector.createAction(holder, _property, pane.Loader.Writer, node, scopeMethod, actsHolder, pane.FindForm()) != null;
		}
		public override ActionClass CreateSetPropertyAction(ILimnorDesignPane designPane, IClass holder, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			return MenuItemDataPropertySelector.createAction(holder, _property, designPane.Loader.Writer, designPane.Loader.Node, scopeMethod, actsHolder, designPane.PaneHolder.FindForm());
		}
		public PropertyClass Property
		{
			get
			{
				return _property;
			}
		}
		public override string Tooltips
		{
			get { return _property.Description; }
		}
	}
}
