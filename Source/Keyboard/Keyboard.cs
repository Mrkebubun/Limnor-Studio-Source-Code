/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Keyboard Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using VPL;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Design;

namespace Limnor.InputDevice
{

	[ToolboxBitmapAttribute(typeof(Keyboard), "Resources.kb.bmp")]
	[Description("This object provides keyboard operations.")]
	public class Keyboard : Control, ICustomTypeDescriptor, ICustomEventMethodDescriptor
	{
		#region fields and constructors
		static class NativeMethods
		{
			[DllImport("Kernel32.dll")]
			public static extern int GetLastError();
			[DllImport("user32.dll", SetLastError = true)]
			public static extern bool RegisterHotKey(IntPtr hWnd, // handle to window    
				int id,            // hot key identifier    
				KeyModifiers fsModifiers,  // key-modifier options    
				Keys vk            // virtual-key code    
				);

			[DllImport("user32.dll", SetLastError = true)]
			public static extern bool UnregisterHotKey(IntPtr hWnd,  // handle to window    
				int id      // hot key identifier    
				);



		}
		private HotKeyList _hotKeys;
		private bool _enabled = true;
		private EventHandler _eventChanged;
		static StringCollection _s_baseProperties;
		static Keyboard()
		{
			_s_baseProperties = new StringCollection();
			_s_baseProperties.Add("Name");
			_s_baseProperties.Add("Location");
			_s_baseProperties.Add("Left");
			_s_baseProperties.Add("Top");
			_s_baseProperties.Add("HotKeys");
		}
		public Keyboard()
		{
			this.Size = new Size(16, 16);
		}
		#endregion
		#region Control
		[Browsable(false)]
		protected override void OnResize(EventArgs e)
		{
			if (this.Size.Width != 16 || this.Size.Height != 16)
			{
				this.Size = new Size(16, 16);
			}
		}
		[Browsable(false)]
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 
				return cp;

			}
		}
		[Browsable(false)]
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//do nothing 
		}
		[Browsable(false)]
		protected override void OnPaint(PaintEventArgs e)
		{
			if (Site != null && Site.DesignMode)
			{
				e.Graphics.DrawImage(Resource1.kb, 0, 0);
			}
		}
		[Browsable(false)]
		protected override void WndProc(ref Message m)
		{
			const int WM_HOTKEY = 0x0312;
			if (_enabled)
			{
				if (m.Msg == WM_HOTKEY)
				{
					int keyId = m.WParam.ToInt32();
					Key key = HotKeys.GetKeyById(keyId);
					if (key != null)
					{
						key.FireKeyPress(this);
					}
					m.Result = IntPtr.Zero;
					return;
				}
			}
			base.WndProc(ref m);
		}
		#endregion
		#region Methods
		[Description("Remove the specified key from the hot key list")]
		public void RemoveHotKey(Key key)
		{
			if (_hotKeys != null)
			{
				Key k;
				if (_hotKeys.TryGetKey(key.KeyName, out k))
				{
					if (k.HotKeySet)
					{
						NativeMethods.UnregisterHotKey(this.Handle, k.KeyId);
						k.HotKeySet = false;
					}
					_hotKeys.Remove(key.KeyName);
				}
			}
		}
		protected override void Dispose(bool disposing)
		{

			if (_hotKeys != null)
			{
				for (int i = 0; i < _hotKeys.Count; i++)
				{
					Key k = _hotKeys[i];
					if (k != null && k.HotKeySet)
					{
						NativeMethods.UnregisterHotKey(this.Handle, k.KeyId);
						k.HotKeySet = false;
					}
				}
			}
			base.Dispose(disposing);
		}
		#endregion
		#region Properties
		[Description("Indicate whether the hotkey events will occur. If this property is false then the actions assigned to hotkeys will not be executed.")]
		public bool EnableHotKeys
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}
		[Editor(typeof(TypeEditorHotKeys), typeof(UITypeEditor))]
		[Description("Hotkeys for assigning actions")]
		public HotKeyList HotKeys
		{
			get
			{
				if (_hotKeys == null)
				{
					_hotKeys = new HotKeyList();
				}
				return _hotKeys;
			}
			set
			{
				_hotKeys = value;
				if (_eventChanged != null)
				{
					_eventChanged(this, EventArgs.Empty);
				}
			}
		}
		#endregion
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
		#region class EventDescriptorKeyboard
		/// <summary>
		/// it is not used for now. TBD: append it to the Keyboard event descriptor collection
		/// </summary>
		class EventDescriptorKeyboard : EventDescriptor
		{
			private Key _key;
			private EventDescriptor _ed;
			public EventDescriptorKeyboard(string name, Attribute[] attrs, Key k)
				: base(name, attrs)
			{
				_key = k;
				EventDescriptorCollection ev = TypeDescriptor.GetEvents(_key);
				foreach (EventDescriptor e in ev)
				{
					if (string.CompareOrdinal(e.Name, "Event") == 0)
					{
						_ed = e;
						break;
					}
				}
			}
			public override void AddEventHandler(object component, Delegate value)
			{
				_ed.AddEventHandler(_key, value);
			}

			public override Type ComponentType
			{
				get { return typeof(Keyboard); }
			}

			public override Type EventType
			{
				get { return typeof(EventHandler); }
			}

			public override bool IsMulticast
			{
				get { return true; }
			}

			public override void RemoveEventHandler(object component, Delegate value)
			{
				_ed.RemoveEventHandler(component, value);
			}
		}
		#endregion
		#region ICustomEventMethodDescriptor Members
		class HotKeyEvent : EventInfo
		{
			private Key _key;
			private EventInfo _info;
			public HotKeyEvent(Key k)
			{
				_key = k;
				_info = typeof(Key).GetEvent("Event");
			}
			private EventInfo info
			{
				get
				{
					if (_info == null)
					{
						_info = typeof(Key).GetEvent("Event");
					}
					return _info;
				}
			}
			public override EventAttributes Attributes
			{
				get { return info.Attributes; }
			}

			public override MethodInfo GetAddMethod(bool nonPublic)
			{
				return info.GetAddMethod(nonPublic);
			}

			public override MethodInfo GetRaiseMethod(bool nonPublic)
			{
				return info.GetRaiseMethod(nonPublic);
			}

			public override MethodInfo GetRemoveMethod(bool nonPublic)
			{
				return info.GetRemoveMethod(nonPublic);
			}

			public override Type DeclaringType
			{
				get
				{
					return typeof(Keyboard);
				}
			}

			public override object[] GetCustomAttributes(Type attributeType, bool inherit)
			{
				return info.GetCustomAttributes(attributeType, inherit);
			}

			public override object[] GetCustomAttributes(bool inherit)
			{
				return info.GetCustomAttributes(inherit);
			}

			public override bool IsDefined(Type attributeType, bool inherit)
			{
				return info.IsDefined(attributeType, inherit);
			}

			public override string Name
			{
				get
				{
					return _key.KeyName;
				}
			}

			public override Type ReflectedType
			{
				get
				{
					return typeof(Keyboard);
				}
			}
		}
		EventInfo[] ICustomEventDescriptor.GetEvents()
		{
			if (HotKeys.Count == 0)
			{
				return new EventInfo[] { };
			}
			else
			{
				EventInfo[] evs = new EventInfo[HotKeys.Count];
				for (int i = 0; i < HotKeys.Count; i++)
				{
					evs[i] = new HotKeyEvent(HotKeys[i]);
				}
				return evs;
			}
		}
		public int GetEventId(string eventName)
		{
			Key k;
			if (HotKeys.TryGetKey(eventName, out k))
			{
				return k.KeyId;
			}
			return 0;
		}
		public bool IsCustomEvent(string eventName)
		{
			return HotKeys.ContainsKey(eventName);
		}
		[Browsable(false)]
		public Type GetEventArgumentType(string eventName)
		{
			return null;
		}
		public string GetEventNameById(int eventId)
		{
			for (int i = 0; i < HotKeys.Count; i++)
			{
				if (HotKeys[i].KeyId == eventId)
				{
					return HotKeys[i].KeyName;
				}
			}
			return null;
		}
		public EventInfo GetEvent(string eventName)
		{
			EventInfo[] ifs = ((ICustomEventMethodDescriptor)this).GetEvents();
			for (int i = 0; i < ifs.Length; i++)
			{
				if (string.CompareOrdinal(ifs[i].Name, eventName) == 0)
				{
					return ifs[i];
				}
			}
			return null;
		}
		public EventInfo GetEventById(int eventId)
		{
			return GetEvent(GetEventNameById(eventId));
		}
		/// <summary>
		/// this is called at runtime for attaching event handlers
		/// </summary>
		/// <param name="eventName"></param>
		/// <returns></returns>
		public IEventHolder GetEventHolder(string eventName)
		{
			Key key;
			if (HotKeys.TryGetKey(eventName, out key))
			{
				if (!key.HotKeySet)
				{
					key.HotKeySet = NativeMethods.RegisterHotKey(this.Handle, key.KeyId, key.Modifiers, key.VirtuakKeyCode);
					if (!key.HotKeySet)
					{
						MessageBox.Show(this.FindForm(), "Cannot set hot key " + key.ToString(), "HotKey", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				return key;
			}
			return null;
		}
		public MethodInfo[] GetMethods()
		{
			Type t = this.GetType();
			MethodInfo[] methods = new MethodInfo[1];
			methods[0] = t.GetMethod("RemoveHotKey");
			return methods;
		}
		public void SetEventChangeMonitor(EventHandler monitor)
		{
			_eventChanged = monitor;
		}
		#endregion
	}
}
