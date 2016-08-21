/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using Limnor.WebBuilder;
using LimnorDesigner.MenuUtil;
using MathExp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using VPL;
using WindowsUtility;
using XmlUtility;

namespace LimnorDesigner.Web
{
	public interface IHtmlMenubarParent
	{
		HtmlElement_menuItemCollection Children { get; }
	}
	public class HtmlElement_menubar : HtmlElement_ItemBase, IHtmlMenubarParent
	{
		private HtmlElement_menuItemCollection _children;
		public HtmlElement_menubar(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_menubar(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "nav"; }
		}
		public HtmlElement_menuItemCollection Children
		{
			get
			{
				if (_children == null)
					_children = new HtmlElement_menuItemCollection(this.RootPointer, this);
				return _children;
			}
		}
		public static void AddMenuItemHandler(HtmlElement_menuItem e, Menu.MenuItemCollection mnu, EventHandler handler, Form caller)
		{
			MenuItem m0 = new MenuItemWithBitmap(e.Text, Resources._event1.ToBitmap());
			mnu.Add(m0);
			EventItem ei = new EventItem(e.Text, e, new EventInfoHtmlMenu(e));
			m0.Tag = ei;
			ei.MenuOwner = e;
			ei.MenuInvoker = caller;
			m0.Click += handler;
			e.CreateContextMenu(m0.MenuItems, handler);
		}
		[Browsable(false)]
		[NotForProgramming]
		public override void CreateContextMenu(Menu.MenuItemCollection mnu, EventHandler handler)
		{
			if (_children != null && _children.Count > 0)
			{
				Form caller = null;
				WebPage wpage = this.RootPointer.ObjectInstance as WebPage;
				if (wpage != null)
				{
					if (wpage.Parent != null)
						caller = wpage.Parent.FindForm();
					else
						caller = wpage;
				}
				MenuItem mi = new MenuItemWithBitmap("Menu click events", Resources._eventHandlers.ToBitmap());
				for (int i = 0; i < _children.Count; i++)
				{
					HtmlElement_menubar.AddMenuItemHandler(_children[i], mi.MenuItems, handler, caller);
				}
				mnu.Add(mi);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public void LoadMenuItems(string items)
		{
			_children = new HtmlElement_menuItemCollection(this.RootPointer, this);
			if (!string.IsNullOrEmpty(items))
			{
				items = items.Trim();
				
				while (items.Length > 0)
				{
					HtmlElement_menuItem mi = new HtmlElement_menuItem(this.RootPointer);
					if (mi.LoadMenuItems(ref items))
					{
						_children.Add(mi);
					}
				}
			}
		}
	}
	public class HtmlElement_menuItem : HtmlElement_ItemBase, IHtmlMenubarParent
	{
		private HtmlElement_menuItemCollection _children;
		private string _text;
		public HtmlElement_menuItem(ClassPointer owner)
			: base(owner)
		{
		}
		public HtmlElement_menuItem(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
		}
		public override string tagName
		{
			get { return "li"; }
		}
		public string Text
		{
			get
			{
				return _text;
			}
		}
		public HtmlElement_menuItemCollection Children
		{
			get
			{
				if (_children == null)
					_children = new HtmlElement_menuItemCollection(this.RootPointer, this);
				return _children;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string ExpressionDisplay
		{
			get
			{
				return _text;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string Name
		{
			get { return _text; }
		}
		#region IXmlNodeSerializable Members
		[Browsable(false)]
		[NotForProgramming]
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			_text = XmlUtil.GetNameAttribute(node);
		}
		[Browsable(false)]
		[NotForProgramming]
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlUtil.SetNameAttribute(node, _text);
		}

		#endregion
		[Browsable(false)]
		[NotForProgramming]
		public override void CreateContextMenu(Menu.MenuItemCollection mnu, EventHandler handler)
		{
			if (_children != null && _children.Count > 0)
			{
				Form caller = null;
				WebPage wpage = this.RootPointer.ObjectInstance as WebPage;
				if (wpage != null)
				{
					if (wpage.Parent != null)
						caller = wpage.Parent.FindForm();
					else
						caller = wpage;
				}
				for (int i = 0; i < _children.Count; i++)
				{
					HtmlElement_menubar.AddMenuItemHandler(_children[i], mnu, handler, caller);
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool LoadMenuItems(ref string items)
		{
			_children = new HtmlElement_menuItemCollection(this.RootPointer, this);
			if (items.Length > 0)
			{
				if (items.StartsWith("[", StringComparison.Ordinal))
				{
					items = items.Substring(1);
					int pos = items.IndexOf(';');
					if (pos > 0)
					{
						this.SetId(items.Substring(0, pos));
						items = items.Substring(pos + 1);
					}
					pos = items.IndexOf(']');
					int pos2 = items.IndexOf('[');
					if (pos2 >= 0 && pos2 < pos)
					{
						pos = pos2;
					}
					if (pos < 0)
					{
						items = string.Empty;
						return false;
					}
					_text = items.Substring(0, pos);
					if (items[pos] == ']')
					{
						items = items.Substring(pos + 1);
						items = items.Trim();
						return true;
					}
					items = items.Substring(pos);
					items = items.Trim();
					while (items.Length > 0 && !items.StartsWith("]", StringComparison.Ordinal))
					{
						HtmlElement_menuItem mi = new HtmlElement_menuItem(this.RootPointer);
						if (mi.LoadMenuItems(ref items))
						{
							_children.Add(mi);
						}
						else
						{
							break;
						}
					}
					if (items.Length > 0 && items.StartsWith("]", StringComparison.Ordinal))
					{
						items = items.Substring(1);
						items = items.Trim();
						return true;
					}
				}
			}
			items = string.Empty;
			return false;
		}
	}
	public class HtmlElement_menuItemCollection : List<HtmlElement_menuItem>
	{
		private ClassPointer _owner;
		private IHtmlMenubarParent _parent;
		public HtmlElement_menuItemCollection(ClassPointer owner, IHtmlMenubarParent parent)
		{
			_owner = owner;
			_parent = parent;
		}
	}
	public class EventInfoHtmlMenu : EventInfo
	{
		private HtmlElement_menuItem _item;
		public EventInfoHtmlMenu(HtmlElement_menuItem item)
		{
			_item = item;
		}

		public override EventAttributes Attributes
		{
			get { return EventAttributes.None; }
		}

		public override MethodInfo GetAddMethod(bool nonPublic)
		{
			return null;
		}

		public override MethodInfo GetRaiseMethod(bool nonPublic)
		{
			return null;
		}

		public override MethodInfo GetRemoveMethod(bool nonPublic)
		{
			return null;
		}

		public override Type DeclaringType
		{
			get { return typeof(HtmlElement_menuItem); }
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return null;
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return null;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return false;
		}

		public override string Name
		{
			get { return "onclick"; }
		}

		public override Type ReflectedType
		{
			get { return typeof(HtmlElement_menuItem); }
		}
#if DOTNET40
		public override Type EventHandlerType
		{
			get
			{
				return typeof(SimpleCall);
			}
		}
#else
		public new Type EventHandlerType
		{
			get
			{
				return typeof(SimpleCall);
			}
		}
#endif
	}
}
