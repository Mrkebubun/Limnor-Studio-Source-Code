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
using System.Drawing;
using VPL;
using VSPrj;
using System.Xml;
using ProgElements;
using System.Windows.Forms;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Property;
using LimnorDesigner.Event;
using XmlSerializer;
using Limnor.Drawing2D;
using LimnorDesigner.Action;

namespace LimnorDesigner.MenuUtil
{
	/// <summary>
	/// context menu for Form Designer and Method Designer
	/// </summary>
	public abstract class MenuItemData : IWithTooltips
	{
		#region constructors and fields
		string _key;
		private Point _location;
		private IClass _owner;
		public MenuItemData(string key, Point location, IClass owner)
		{
			_key = key;
			_location = location;
			_owner = owner;
		}
		public MenuItemData(string key, IClass owner)
		{
			_key = key;
			_location = new Point(0, 0);
			_owner = owner;
		}
		#endregion
		#region properties
		public IClass MenuOwner { get; set; }
		public Form MenuInvoker { get; set; }
		public string Key
		{
			get
			{
				return _key;
			}
		}
		public Point Location
		{
			get
			{
				return _location;
			}
			set
			{
				_location = value;
			}
		}
		public IClass Owner
		{
			get
			{
				return _owner;
			}
		}
		#endregion
		#region IWithTooltips Members

		public abstract string Tooltips { get; }
		#endregion
		/// <summary>
		/// 
		/// </summary>
		/// <param name="project"></param>
		/// <param name="holder">action executer</param>
		/// <param name="node"></param>
		/// <param name="pane"></param>
		/// <returns></returns>
		public abstract bool ExecuteMenuCommand(LimnorProject project, IClass holder, XmlNode node, MultiPanes pane, IMethod scopeMethod, IActionsHolder actsHolder);
		public override string ToString()
		{
			return Key;
		}
		public void ResetOwner(IClass o)
		{
			_owner = o;
		}
	}

	public abstract class MenuItemDataMethod : MenuItemData
	{
		#region constructors and fields
		public MenuItemDataMethod(string key, Point location, IClass owner)
			: base(key, location, owner)
		{
		}
		public MenuItemDataMethod(string key, IClass owner)
			: base(key, owner)
		{
		}
		#endregion
		public abstract IAction CreateMethodAction(ILimnorDesignPane designPane, IClass holder, IMethod scopeMethod, IActionsHolder actsHolder);
		public static MenuItemDataMethod CreateMenuItem(IMethod p, LimnorContextMenuCollection menus)
		{
			MethodInfoPointer ip = p as MethodInfoPointer;
			if (ip != null)
			{
				return new MethodItem(p.MethodSignature, menus.Owner, ip.MethodInformation);
			}
			MethodClass mc = p as MethodClass;
			if (mc != null)
			{
				return new MethodItemClassPointer(p.MethodSignature, menus.Owner, mc);
			}
			IMethodWrapper w = p as IMethodWrapper;
			if (w != null)
			{
				return new MethodItemWrapper(p.MethodSignature, menus.Owner, w);
			}
			throw new DesignerException("Unsupported method type {0} for CreateMenuItem", p.GetType());
		}
	}
	public class MenuItemDataMethodSelector : MenuItemDataMethod
	{
		#region constructors and fields
		private LimnorContextMenuCollection _menuData;
		public MenuItemDataMethodSelector(string key, Point location, LimnorContextMenuCollection menuData)
			: base(key, location, menuData.Owner)
		{
			_menuData = menuData;
		}
		public MenuItemDataMethodSelector(string key, LimnorContextMenuCollection menuData)
			: base(key, menuData.Owner)
		{
			_menuData = menuData;
		}
		#endregion
		public override bool ExecuteMenuCommand(LimnorProject project, IClass holder, XmlNode node, MultiPanes pane, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			IMethodPointer mi = DesignUtil.EditFrequentlyUsedMethodList(project, node, _menuData, pane.Loader.DesignPane, pane.FindForm());
			if (mi != null)
			{
				return DesignUtil.OnCreateAction(holder, mi, scopeMethod, actsHolder, pane, node) != null;
			}
			return false;
		}
		public override IAction CreateMethodAction(ILimnorDesignPane designPane, IClass holder, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			IMethodPointer mi = DesignUtil.EditFrequentlyUsedMethodList(designPane.Loader.Project, designPane.RootXmlNode, _menuData, designPane, designPane.PaneHolder.FindForm());
			if (mi != null)
			{
				return DesignUtil.OnCreateAction(holder, mi, scopeMethod, actsHolder, designPane.PaneHolder, designPane.RootXmlNode);
			}
			return null;
		}
		public override string Tooltips
		{
			get
			{
				return "Select a method from all methods";
			}
		}
	}
	public abstract class MenuItemDataProperty : MenuItemData
	{
		#region constructors and fields
		public MenuItemDataProperty(string key, Point location, IClass owner)
			: base(key, location, owner)
		{
		}
		public MenuItemDataProperty(string key, IClass owner)
			: base(key, owner)
		{
		}
		#endregion
		public abstract ActionClass CreateSetPropertyAction(ILimnorDesignPane designPane, IClass holder, IMethod scopeMethod, IActionsHolder actsHolder);
		public static MenuItemDataProperty CreateMenuItem(IProperty p, LimnorContextMenuCollection menus)
		{
			PropertyPointer pp = p as PropertyPointer;
			if (pp != null)
			{
				return new PropertyItem(p.Name, menus.Owner, pp.Info);
			}
			else
			{
				PropertyClass pc = p as PropertyClass;
				if (pc != null)
				{
					return new PropertyItemClassPointer(p.Name, menus.Owner, pc);
				}
				else
				{
					IPropertyWrapper w = p as IPropertyWrapper;
					if (w != null)
					{
					}
				}
			}
			throw new DesignerException("Unsupported property type {0} for CreateMenuItem", p.GetType());
		}
	}
	public class MenuItemDataPropertySelector : MenuItemDataProperty
	{
		#region constructors and fields
		private LimnorContextMenuCollection _menuData;
		public MenuItemDataPropertySelector(string key, Point location, LimnorContextMenuCollection menuData)
			: base(key, location, menuData.Owner)
		{
			_menuData = menuData;
		}
		public MenuItemDataPropertySelector(string key, LimnorContextMenuCollection menuData)
			: base(key, menuData.Owner)
		{
			_menuData = menuData;
		}
		#endregion
		public override bool ExecuteMenuCommand(LimnorProject project, IClass holder, XmlNode node, MultiPanes pane, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			IProperty ei = DesignUtil.EditFrequentlyUsedPropertyList(project, node, _menuData, pane.Loader.DesignPane, pane.FindForm());
			return createAction(holder, ei, pane.Loader.Writer, node, scopeMethod, actsHolder, pane.FindForm()) != null;
		}
		public override ActionClass CreateSetPropertyAction(ILimnorDesignPane designPane, IClass holder, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			IProperty ei = DesignUtil.EditFrequentlyUsedPropertyList(designPane.Loader.Project, designPane.RootXmlNode, _menuData, designPane, designPane.PaneHolder.FindForm());
			return createAction(holder, ei, designPane.Loader.Writer, designPane.RootXmlNode, scopeMethod, actsHolder, designPane.PaneHolder.FindForm());
		}
		public static ActionClass createAction(IClass holder, IProperty ei, XmlObjectWriter writer, XmlNode node, IMethod scopeMethod, IActionsHolder actsHolder, Form caller)
		{
			if (ei != null)
			{
				IPropertyEx pp = ei as IPropertyEx;
				if (pp != null)
				{
					ActionClass act = new ActionClass(pp.RootPointer);
					act.ActionMethod = pp.CreateSetterMethodPointer(act);
					act.ActionName = act.ActionMethod.DefaultActionName;
					act.ActionHolder = actsHolder;
					if (pp.RootPointer.CreateNewAction(act, writer, scopeMethod, caller))
					{
						return act;
					}
				}
				else
				{
					PropertyClass pc = ei as PropertyClass;
					if (pc != null)
					{
						PropertyClass pc2 = (PropertyClass)pc.Clone();
						pc2.SetHolder(holder);
						ActionClass act = new ActionClass(holder.RootPointer);
						act.ActionMethod = pc2.CreateSetterMethodPointer(act);
						act.ActionName = act.ActionMethod.DefaultActionName;// pc.Holder.CodeName + ".Set" + pc.Name;
						act.ActionHolder = actsHolder;
						if (holder.RootPointer.CreateNewAction(act, writer, scopeMethod, caller))
						{
							return act;
						}
					}
					else
					{
					}
				}
			}
			return null;
		}
		public override string Tooltips
		{
			get
			{
				return "Select a property from all properties";
			}
		}
	}
	public abstract class MenuItemDataEvent : MenuItemData
	{
		#region constructors and fields
		public MenuItemDataEvent(string key, Point location, IClass owner)
			: base(key, location, owner)
		{
		}
		public MenuItemDataEvent(string key, IClass owner)
			: base(key, owner)
		{
		}
		#endregion
		public abstract IEvent CreateEventPointer(IClass holder);
		public static MenuItemDataEvent CreateMenuItem(IEvent p, LimnorContextMenuCollection menus)
		{
			EventPointer pp = p as EventPointer;
			if (pp != null)
			{
				return new EventItem(pp.ObjectKey, menus.Owner, pp.Info);
			}
			else
			{
				EventClass pc = p as EventClass;
				if (pc != null)
				{
					return new EventItemClassPointer(pc.DisplayName, menus.Owner, pc);
				}
				else
				{
					IPropertyWrapper w = p as IPropertyWrapper;
					if (w != null)
					{
					}
				}
			}
			throw new DesignerException("Unsupported property type {0} for CreateMenuItem", p.GetType());
		}
	}
	public class MenuItemDataEventSelector : MenuItemDataEvent
	{
		#region constructors and fields
		private LimnorContextMenuCollection _menuData;
		public MenuItemDataEventSelector(string key, Point location, LimnorContextMenuCollection menuData)
			: base(key, location, menuData.Owner)
		{
			_menuData = menuData;
		}
		public MenuItemDataEventSelector(string key, LimnorContextMenuCollection menuData)
			: base(key, menuData.Owner)
		{
			_menuData = menuData;
		}
		#endregion
		public override IEvent CreateEventPointer(IClass holder)
		{
			return null;
		}
		public override bool ExecuteMenuCommand(LimnorProject project, IClass holder, XmlNode node, MultiPanes pane, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			DlgSelectEvent dlg = new DlgSelectEvent();
			dlg.LoadData(_menuData);
			DialogResult ret = dlg.ShowDialog(pane.FindForm());
			if (dlg.FrequentlyUsedMethodsChanged)
			{
				_menuData.RemoveMenuCollection();
				pane.Loader.DesignPane.ResetContextMenu();
			}
			if (ret == DialogResult.OK)
			{
				IEvent ei = dlg.ReturnEventInfo;
				if (ei != null)
				{
					EventPointer ep = ei as EventPointer;
					if (ep != null)
					{
						return pane.AssignActions(ep, pane.FindForm());
					}
					else
					{
						EventClass ec = ei as EventClass;
						if (ec != null)
						{
							EventClass ec2 = (EventClass)ec.Clone();
							ec2.SetHolder(holder);
							CustomEventPointer cep = new CustomEventPointer(ec2, this.Owner);
							return pane.AssignActions(cep, pane.FindForm());
						}
					}
				}
			}
			return false;
		}
		public override string Tooltips
		{
			get
			{
				return "Select an event from all events";
			}
		}
	}
}
