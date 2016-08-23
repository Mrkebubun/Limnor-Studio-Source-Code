/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Mouse Pointer control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VPL;
using System.Collections.Specialized;
using System.Reflection;

namespace Limnor.InputDevice
{
	[ToolboxBitmapAttribute(typeof(MousePointer), "Resources.mousePointer.bmp")]
	[Description("This object provides mouse operations.")]
	public class MousePointer : Control, ICustomTypeDescriptor, ICustomEventMethodDescriptor
	{
		struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}
		static class NativeMethods
		{
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			public static extern bool ClipCursor(ref RECT rcClip);
			[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "ClipCursor")]
			public static extern void ClipCursorClear(IntPtr rc);
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			public static extern bool SetCursorPos(int x, int y);
			[DllImport("Kernel32.dll")]
			public static extern int GetLastError();
		}
		static StringCollection _s_baseProperties;
		static MousePointer()
		{
			_s_baseProperties = new StringCollection();
			_s_baseProperties.Add("Name");
			_s_baseProperties.Add("Location");
			_s_baseProperties.Add("Left");
			_s_baseProperties.Add("Top");
		}
		public MousePointer()
		{
			this.Size = new Size(16, 16);
		}
		protected override void OnResize(EventArgs e)
		{
			if (this.Size.Width != 16 || this.Size.Height != 16)
			{
				this.Size = new Size(16, 16);
			}
		}
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 
				return cp;

			}
		}
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//do nothing 
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			if (Site != null && Site.DesignMode)
			{
				e.Graphics.DrawImage(Resource1.mousePointer, 0, 0);
			}
		}
		[Description("It confines the cursor to a rectangular area on the window. If a subsequent cursor position (set by the SetCursorPos function or the mouse) lies outside the rectangle, the system automatically adjusts the position to keep the cursor inside the rectangular area. ")]
		public void ClipCursor(Rectangle rc)
		{
			Control p = this.Parent;
			if (p != null)
			{
				rc = p.RectangleToScreen(rc);
			}
			RECT r = new RECT();
			r.Left = rc.Left;
			r.Top = rc.Top;
			r.Right = rc.Right;
			r.Bottom = rc.Bottom;
			NativeMethods.ClipCursor(ref r);
		}
		[Description("Remove cursor clipping so that the cursor is free to move anywhere on the screen")]
		public void ClearCursorClip()
		{
			NativeMethods.ClipCursorClear(IntPtr.Zero);
		}
		[Description("Moves the cursor to the specified window coordinates. If the new coordinates are not within the screen rectangle set by the most recent ClipCursor function call, the system automatically adjusts the coordinates so that the cursor stays within the rectangle. Returns 0 if successful or error code otherwise.")]
		public int SetCursorPosition(int x, int y)
		{
			Control p = this.Parent;
			if (p != null)
			{
				Point po = p.PointToScreen(new Point(x, y));
				x = po.X;
				y = po.Y;
			}
			if (NativeMethods.SetCursorPos(x, y))
			{
				return 0;
			}
			return NativeMethods.GetLastError();
		}
		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		static public bool GetBrowseableProperties(Attribute[] attributes)
		{
			if (attributes != null && attributes.Length > 0)
			{
				for (int i = 0; i < attributes.Length; i++)
				{
					if (attributes[i] is BrowsableAttribute)
					{
						return true;
					}
				}
			}
			return false;
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (_s_baseProperties.Contains(p.Name))
				{
					list.Add(p);
					continue;
				}
			}
			return new PropertyDescriptorCollection(list.ToArray());
		}
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region ICustomEventMethodDescriptor Members

		EventInfo[] ICustomEventDescriptor.GetEvents()
		{
			return new EventInfo[] { };
		}
		EventInfo ICustomEventDescriptor.GetEvent(string eventName)
		{
			return null;
		}
		public EventInfo GetEventById(int eventId)
		{
			return null;
		}
		public int GetEventId(string eventName)
		{
			return 0;
		}
		public bool IsCustomEvent(string eventName)
		{
			return false;
		}
		[Browsable(false)]
		public Type GetEventArgumentType(string eventName)
		{
			return null;
		}
		public string GetEventNameById(int eventId)
		{
			return null;
		}
		public IEventHolder GetEventHolder(string eventName)
		{
			return null;
		}
		public void SetEventChangeMonitor(EventHandler monitor)
		{
		}
		public MethodInfo[] GetMethods()
		{
			Type t = this.GetType();
			MethodInfo[] methods = new MethodInfo[3];
			methods[0] = t.GetMethod("ClipCursor");
			methods[1] = t.GetMethod("ClearCursorClip");
			methods[2] = t.GetMethod("SetCursorPosition");
			return methods;
		}

		#endregion
	}
}
