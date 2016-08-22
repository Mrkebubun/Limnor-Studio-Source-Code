/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Xml;

namespace VPL
{
	/// <summary>
	/// for perform actions on an instance
	/// </summary>
	/// <param name="instance"></param>
	/// <param name="data"></param>
	public delegate void delegateOnGetInstance(RAIS_R r, object data);
	/// <summary>
	/// for assigning event handlers
	/// </summary>
	class OM_EventData
	{
		public string EventName;
		public Delegate Handler;
		public OM_EventData(string eventName, Delegate handler)
		{
			EventName = eventName;
			Handler = handler;
		}
	}
	class OM_Invoke
	{
		public string MethodName;
		public object[] Parameters;
		public OM_Invoke(string methodName, object[] ps)
		{
			MethodName = methodName;
			Parameters = ps;
		}
	}
	class OM_Assign
	{
		public string PropertyName;
		public object Value;
		public object[] Indexer;
		public OM_Assign(string propertyName, object value, object[] indexer)
		{
			PropertyName = propertyName;
			Value = value;
			Indexer = indexer;
		}
	}
	/// <summary>
	/// Implement RAIS model for "programming on types" - programming applies to all instances of the same type.
	/// It is for implementing IObjectManager as 1-dimensional array.
	/// 
	/// R: real type, A: abstract type, I: instance, S: sub-class
	/// 
	/// This type system is fading out from Limnor Studio. Currently only data types in Expression Editor are still using it merely as a wrapper on .Net Type
	/// </summary>
	public abstract class RAIS : IObjectManager
	{
		public const string XMLATT_NAME = "name";
		public const string TYPE = "type";
		public const string XML_R = "R";
		public const string XML_A = "A";
		public const string XML_I = "I";
		public const string XML_S = "S";
		private string _name;
		private Type _type;
		private List<RAIS_R> _r;
		private List<RAIS_A> _a;
		private RAIS_A _baseType;
		private RAIS _parentType;
		/// <summary>
		/// create the root
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		protected RAIS(string name, Type type)
		{
			_name = name;
			_type = type;
		}
		/// <summary>
		/// to create a member of I/S
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="baseType"></param>
		protected RAIS(string name, Type type, RAIS_A baseType)
		{
			_name = name;
			_baseType = baseType;
			_type = type;
		}
		/// <summary>
		/// for loading map. name and type will be read from XML
		/// </summary>
		/// <param name="baseType"></param>
		protected RAIS(RAIS_A baseType)
		{
			_name = null;
			_type = null;
			_baseType = baseType;
		}
		/// <summary>
		/// to create a member of R/T
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="parentType"></param>
		protected RAIS(string name, Type type, RAIS parentType)
		{
			_name = name;
			_type = type;
			_parentType = parentType;
		}
		/// <summary>
		/// for loading map. name and type will be read from XML
		/// </summary>
		/// <param name="parentType"></param>
		protected RAIS(RAIS parentType)
		{
			_name = null;
			_type = null;
			_parentType = parentType;
		}
		public RAIS Root
		{
			get
			{
				if (_baseType != null)
				{
					return _baseType.Root;
				}
				if (_parentType != null)
				{
					return _parentType.Root;
				}
				return this;
			}
		}
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		public Type Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}
		/// <summary>
		/// if BaseType is not null then it is a member of I or S of BaseType.
		/// if BaseType is null then it is the root or it is a member of R or T.
		/// </summary>
		public RAIS_A BaseType
		{
			get
			{
				return _baseType;
			}
		}
		/// <summary>
		/// if ParentType is not null then it is a member of R or T of ParentType.
		/// if ParentType is null then it is the root.
		/// </summary>
		public RAIS ParentType
		{
			get
			{
				return _parentType;
			}
		}
		/// <summary>
		/// if ContainerType is not null then it is a member object/type of ContainerType.
		/// if ContainerType is null the it is the root
		/// </summary>
		public RAIS_R ContainerType
		{
			get
			{
				if (BaseType == null)
				{
					//this type is not derived from another type (a library type)
					//this type is a member of R/T or it is the root
					if (this.ParentType is RAIS_R)
						return ((RAIS_R)this.ParentType);
					else
					{
						return null; //this type is a part of type definition
					}
				}
				else
				{
					//this type is a member of I/S, its container is the same as its base type.
					return BaseType.ContainerType;
				}
			}
		}
		public static string GetAttribute(XmlNode node, string name)
		{
			XmlAttribute xa = node.Attributes[name];
			if (xa != null)
			{
				return xa.Value;
			}
			return null;
		}
		/// <summary>
		/// populate the object map by loading XML.
		/// </summary>
		/// <param name="node"></param>
		public virtual void LoadMap(XmlNode node, Assembly typeProvider)
		{
			this.Name = GetAttribute(node, RAIS.XMLATT_NAME);
			string s = GetAttribute(node, RAIS.TYPE);
			this.Type = typeProvider.GetType(s, true);
			//load R/A/I/S
			foreach (XmlNode nd in node.ChildNodes)
			{
				if (nd.Name == XML_R) //R
				{
					RAIS_R r = new RAIS_R((RAIS)this);
					r.LoadMap(nd, typeProvider);
					AddR(r);
				}
				else if (nd.Name == XML_A) //A
				{
					RAIS_A t = new RAIS_A((RAIS)this);
					t.LoadMap(nd, typeProvider);
					AddA(t);
				}
			}
		}
		/// <summary>
		/// number of elements in R
		/// </summary>
		public int RCount
		{
			get
			{
				if (_r == null)
					return 0;
				return _r.Count;
			}
		}
		public RAIS_R RItem(int i)
		{
			if (i >= 0 && i < RCount)
				return _r[i];
			return null;
		}
		public RAIS_R RItem(string name)
		{
			if (_r != null)
			{
				for (int i = 0; i < _r.Count; i++)
				{
					if (_r[i].Name == name)
						return _r[i];
				}
			}
			return null;
		}
		/// <summary>
		/// number of elements in A
		/// </summary>
		public int ACount
		{
			get
			{
				if (_a == null)
					return 0;
				return _a.Count;
			}
		}
		public RAIS_A AItem(int i)
		{
			if (i >= 0 && i < RCount)
				return _a[i];
			return null;
		}
		public RAIS_A AItem(string name)
		{
			if (_a != null)
			{
				for (int i = 0; i < _a.Count; i++)
				{
					if (_a[i].Name == name)
						return _a[i];
				}
			}
			return null;
		}
		public RAIS_R AddR(object instance, string name)
		{
			RAIS_R r = new RAIS_R(name, instance, this);
			AddR(r);
			return r;
		}
		protected void AddR(RAIS_R r)
		{
			if (_r == null)
				_r = new List<RAIS_R>();
			_r.Add(r);
		}
		public RAIS_A AddA(Type type, string name)
		{
			RAIS_A t = new RAIS_A(name, type, this);
			AddA(t);
			return t;
		}
		protected void AddA(RAIS_A a)
		{
			if (_a == null)
				_a = new List<RAIS_A>();
			_a.Add(a);
		}
		public virtual bool NameExists(string name)
		{
			if (_r != null)
			{
				for (int i = 0; i < _r.Count; i++)
				{
					if (_r[i].Name == name)
						return true;
				}
			}
			if (_a != null)
			{
				for (int i = 0; i < _a.Count; i++)
				{
					if (_a[i].Name == name)
						return true;
					if (_a[i].NameExists(name))
						return true;
				}
			}
			return false;
		}
		public string CreateNewName(string nameBase)
		{
			int n = 1;
			string name = nameBase + n.ToString();
			while (this.NameExists(name))
			{
				n++;
				name = nameBase + n.ToString();
			}
			return name;
		}
		/// <summary>
		/// find the RAIS object by the specified path.
		/// </summary>
		/// <param name="xtPath">path format: ?name/?name/...
		/// ? is one of R, A, I, S
		/// </param>
		/// <returns></returns>
		public RAIS GetItemByXTPath(string xtPath)
		{
			RAIS rais = this.Root;
			string s = xtPath;
			while (!string.IsNullOrEmpty(s))
			{
				string name = VPLUtil.PopPath(ref s);
				//get one name in the format ?<name>
				rais = rais.GetSubClass(name);
			}
			return rais;
		}
		protected virtual RAIS GetSubClass(string name)
		{
			RAIS rais = null;
			string sType = name.Substring(0, 1);
			string s = name.Substring(1);
			if (sType == XML_A)
				rais = AItem(s);
			else if (sType == XML_R)
				rais = RItem(s);
			return rais;
		}
		public virtual void WorkOnAllInstances(delegateOnGetInstance handler, object data)
		{
			if (_r != null)
			{
				for (int i = 0; i < _r.Count; i++)
				{
					handler(_r[i], data);
				}
			}
			if (_a != null)
			{
				for (int i = 0; i < _a.Count; i++)
				{
					_a[i].WorkOnAllInstances(handler, data);
				}
			}
		}
		public virtual bool WorkOnAllInstances(string xtPath, delegateOnGetInstance handler, object data)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
			{
				rais.WorkOnAllInstances(handler, data);
				return true;
			}
			return false;
		}
		private void onAddEventHandler(RAIS_R r, object data)
		{
			OM_EventData e = (OM_EventData)data;
			EventInfo ei = r.Instance.GetType().GetEvent(e.EventName);
			ei.AddEventHandler(r.Instance, e.Handler);
		}
		private void onRemoveEventHandler(RAIS_R r, object data)
		{
			OM_EventData e = (OM_EventData)data;
			EventInfo ei = r.Instance.GetType().GetEvent(e.EventName);
			ei.RemoveEventHandler(r.Instance, e.Handler);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventName">for all instances</param>
		/// <param name="methodName">it belongs to the methodOwner</param>
		public void AddEventHandler(string eventName, object methodOwner, string methodName)
		{
			EventInfo ei = _type.GetEvent(eventName);
			object v = DotNetUtilEvent.CreateDelegate(ei.EventHandlerType, methodOwner, methodName);
			OM_EventData e = new OM_EventData(eventName, (Delegate)v);
			delegateOnGetInstance eh = new delegateOnGetInstance(onAddEventHandler);
			WorkOnAllInstances(eh, e);
		}

		public void RemoveEventHandler(string eventName, object methodOwner, string methodName)
		{
			EventInfo ei = _type.GetEvent(eventName);
			object v = DotNetUtilEvent.CreateDelegate(ei.EventHandlerType, methodOwner, methodName);
			OM_EventData e = new OM_EventData(eventName, (Delegate)v);
			delegateOnGetInstance eh = new delegateOnGetInstance(onRemoveEventHandler);
			WorkOnAllInstances(eh, e);
		}
		public virtual void AddEventHandler(string xtPath, string eventName, object methodOwner, string methodName)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
			{
				rais.AddEventHandler(eventName, methodOwner, methodName);
			}
		}
		public virtual void RemoveEventHandler(string xtPath, string eventName, object methodOwner, string methodName)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
			{
				rais.RemoveEventHandler(eventName, methodOwner, methodName);
			}
		}
		private void onInvoke(RAIS_R r, object data)
		{
			OM_Invoke e = (OM_Invoke)data;
			MethodInfo mi = r.Instance.GetType().GetMethod(e.MethodName);
			mi.Invoke(r.Instance, e.Parameters);
		}
		public void Invoke(string methodName, object[] ps)
		{
			MethodInfo mi = _type.GetMethod(methodName);
			if (mi.IsStatic)
			{
				mi.Invoke(null, ps);
			}
			else
			{
				OM_Invoke data = new OM_Invoke(methodName, ps);
				delegateOnGetInstance eh = new delegateOnGetInstance(onInvoke);
				WorkOnAllInstances(eh, data);
			}
		}
		public void Invoke(string xtPath, string methodName, object[] ps)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
			{
				rais.Invoke(methodName, ps);
			}
		}
		private void onAssign(RAIS_R r, object data)
		{
			OM_Assign e = (OM_Assign)data;
			PropertyInfo pi = r.Instance.GetType().GetProperty(e.PropertyName);
			pi.SetValue(r.Instance, e.Value, e.Indexer);
		}
		public void Assign(string propertyName, object value, object[] indexer)
		{
			OM_Assign data = new OM_Assign(propertyName, value, indexer);
			delegateOnGetInstance eh = new delegateOnGetInstance(onAssign);
			WorkOnAllInstances(eh, data);
		}
		public void Assign(string xtPath, string propertyName, object value, object[] indexer)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
			{
				rais.Assign(propertyName, value, indexer);
			}
		}


		#region IObjectManager Members
		protected virtual int itemCount
		{
			get
			{
				return 1;
			}
		}
		public int ItemCount(string xtPath)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
				return rais.itemCount;
			return 0;
		}

		public Type IndexerType
		{
			get { return typeof(int); }
		}
		protected virtual object Item(object indexer)
		{
			return this;
		}
		public object Item(string xtPath, object indexer)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
				return rais.Item(indexer);
			return null;
		}
		protected virtual object ItemByName(string name)
		{
			return this;
		}
		public object ItemByName(string xtPath, string name)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
				return rais.Item(name);
			return null;
		}
		protected virtual object CurrentIndexer()
		{
			return 0;
		}
		public object CurrentIndexer(string xtPath)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
				return rais.CurrentIndexer();
			return null;
		}
		protected virtual void SetCurrebtIndexer(object indexer)
		{
		}
		public void SetCurrebtIndexer(string xtPath, object indexer)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
				rais.SetCurrebtIndexer(indexer);
		}
		protected virtual object CurrentItem()
		{
			return this;
		}
		public object CurrentItem(string xtPath)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
				return rais.CurrentItem();
			return null;
		}
		protected virtual object NewItem()
		{
			return null;
		}
		public object NewItem(string xtPath)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
				return rais.NewItem();
			return null;
		}
		protected virtual void RemoveItem(object indexer)
		{
		}
		public void RemoveItem(string xtPath, object indexer)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
				rais.RemoveItem(indexer);
		}
		protected virtual void RemoveItemByName(string name)
		{
		}
		public void RemoveItemByName(string xtPath, string name)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
				rais.RemoveItemByName(name);
		}
		protected virtual void RemoveAll()
		{
		}
		public void RemoveAll(string xtPath)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
				rais.RemoveAll();
		}
		protected virtual void CreateItem(string name)
		{
		}
		public void CreateItem(string xtPath, string name)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
				rais.CreateItem(name);
		}
		public IObjectManager FindElement(string xtPath)
		{
			return this.GetItemByXTPath(xtPath);
		}
		protected virtual void MoveNext()
		{
		}
		public void MoveNext(string xtPath)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
				rais.MoveNext();
		}
		protected virtual void MoveBack()
		{
		}
		public void MoveBack(string xtPath)
		{
			RAIS rais = this.GetItemByXTPath(xtPath);
			if (rais != null)
				rais.MoveBack();
		}
		#endregion
	}
	public class RAIS_R : RAIS
	{
		private object _object;
		/// <summary>
		/// create the root
		/// </summary>
		/// <param name="name"></param>
		/// <param name="instance"></param>
		public RAIS_R(string name, object instance)
			: base(name, instance.GetType())
		{
			_object = instance;
		}
		/// <summary>
		/// create a member of I/S
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="baseType"></param>
		public RAIS_R(string name, object instance, RAIS_A baseType)
			: base(name, instance.GetType(), baseType)
		{
			_object = instance;
		}
		/// <summary>
		/// create a member of R/A
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="parentType"></param>
		public RAIS_R(string name, object instance, RAIS parentType)
			: base(name, instance.GetType(), parentType)
		{
			_object = instance;
		}
		/// <summary>
		/// for loading map
		/// </summary>
		/// <param name="parentType"></param>
		public RAIS_R(RAIS parentType)
			: base(parentType)
		{
		}
		public RAIS_R(RAIS_A baseType)
			: base(baseType)
		{
		}
		public object Instance
		{
			get
			{
				if (_object == null)
				{
					if (this.ContainerType == null)
					{
						throw new VPLException("Map Root not set");
					}
					else
					{
						object o = this.ContainerType.Instance;
						if (o == null)
						{
							throw new VPLException("Root instance not set");
						}
						else
						{
							FieldInfo fi = o.GetType().GetField(this.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
							if (fi == null)
							{
								throw new VPLException(string.Format("Variable {0} not found in {1}", this.Name, this.ContainerType.Name));
							}
							_object = fi.GetValue(o);
						}
					}
				}
				return _object;
			}
		}
	}
	public class RAIS_A : RAIS
	{
		private List<RAIS_R> _i;
		private List<RAIS_A> _s;
		public RAIS_A(string name, Type type)
			: base(name, type)
		{
		}
		public RAIS_A(string name, Type type, RAIS_A baseType)
			: base(name, type, baseType)
		{
		}
		public RAIS_A(string name, Type type, RAIS parentType)
			: base(name, type, parentType)
		{
		}
		public RAIS_A(RAIS_A baseType)
			: base(baseType)
		{
		}
		public RAIS_A(RAIS parentType)
			: base(parentType)
		{
		}
		public override void LoadMap(XmlNode node, Assembly typeProvider)
		{
			base.LoadMap(node, typeProvider);
			//load I/S
			foreach (XmlNode nd in node.ChildNodes)
			{
				if (nd.Name == RAIS.XML_I) //I
				{
					foreach (XmlNode nd1 in nd.ChildNodes)
					{
						RAIS_R r = new RAIS_R((RAIS_A)this);
						r.LoadMap(nd1, typeProvider);
						AddI(r);
					}
				}
				else if (nd.Name == XML_S) //S
				{
					foreach (XmlNode nd1 in nd.ChildNodes)
					{
						RAIS_A t = new RAIS_A((RAIS_A)this);
						t.LoadMap(nd1, typeProvider);
						AddS(t);
					}
				}
			}
		}
		public int ICount
		{
			get
			{
				if (_i == null)
					return 0;
				return _i.Count;
			}
		}
		public RAIS_R IItem(int i)
		{
			if (i >= 0 && i < ICount)
				return _i[i];
			return null;
		}
		public RAIS_R IItem(string name)
		{
			if (_i != null)
			{
				for (int i = 0; i < _i.Count; i++)
				{
					if (_i[i].Name == name)
						return _i[i];
				}
			}
			return null;
		}
		public int SCount
		{
			get
			{
				if (_s == null)
					return 0;
				return _s.Count;
			}
		}
		public RAIS_A SItem(int i)
		{
			if (i >= 0 && i < SCount)
				return _s[i];
			return null;
		}
		public RAIS_A SItem(string name)
		{
			if (_s != null)
			{
				for (int i = 0; i < _s.Count; i++)
				{
					if (_s[i].Name == name)
						return _s[i];
				}
			}
			return null;
		}
		public RAIS_R AddI(object instance, string name)
		{
			RAIS_R r = new RAIS_R(name, instance, this);
			AddI(r);
			return r;
		}
		protected void AddI(RAIS_R r)
		{
			if (_i == null)
				_i = new List<RAIS_R>();
			_i.Add(r);
		}
		public RAIS_A AddS(Type type, string name)
		{
			RAIS_A t = new RAIS_A(name, type, this);
			AddS(t);
			return t;
		}
		protected void AddS(RAIS_A t)
		{
			if (_s == null)
				_s = new List<RAIS_A>();
			_s.Add(t);
		}
		public override bool NameExists(string name)
		{
			if (_i != null)
			{
				for (int i = 0; i < _i.Count; i++)
				{
					if (_i[i].Name == name)
						return true;
				}
			}
			if (_s != null)
			{
				for (int i = 0; i < _s.Count; i++)
				{
					if (_s[i].Name == name)
						return true;
					if (_s[i].NameExists(name))
						return true;
				}
			}
			return false;
		}
		public RAIS_R CreateI(string name)
		{
			object instance = Activator.CreateInstance(this.Type);
			IComponent ic = instance as IComponent;
			if (ic != null)
			{
				if (ic.Site == null)
				{
					ic.Site = new XTypeSite(ic);
				}
				if (string.IsNullOrEmpty(name))
				{
					int n = 2;
					name = this.Type.Name + n.ToString();
					RAIS container = this.ContainerType;
					if (container != null)
					{
						name = container.CreateNewName(this.Type.Name);
					}
					else
					{
						name = this.CreateNewName(this.Type.Name);
					}
				}
				ic.Site.Name = name;
			}
			RAIS_R r = new RAIS_R(name, instance, this);
			if (_i == null)
				_i = new List<RAIS_R>();
			_i.Add(r);
			return r;
		}
		protected override RAIS GetSubClass(string name)
		{
			RAIS rais = base.GetSubClass(name);
			if (rais == null)
			{
				string sType = name.Substring(0, 1);
				string s = name.Substring(1);
				if (sType == RAIS.XML_I)
					rais = IItem(s);
				else if (sType == RAIS.XML_S)
					rais = SItem(s);
			}
			return rais;
		}

		public bool IsInstance(object v)
		{
			return v.GetType().IsSubclassOf(this.Type);
		}
		public override void WorkOnAllInstances(delegateOnGetInstance handler, object data)
		{
			if (_i != null)
			{
				for (int i = 0; i < _i.Count; i++)
				{
					handler(_i[i], data);
				}
			}
			if (_s != null)
			{
				for (int i = 0; i < _s.Count; i++)
				{
					_s[i].WorkOnAllInstances(handler, data);
				}
			}
		}
		#region Implementing IObjectManager Members
		protected override int itemCount
		{
			get
			{
				return this.ICount;
			}
		}
		protected override object Item(object indexer)
		{
			return this.IItem((int)indexer).Instance;
		}
		protected override object ItemByName(string name)
		{
			return this.IItem(name).Instance;
		}
		int _currentIndexer = 0;
		protected override object CurrentIndexer()
		{
			return _currentIndexer;
		}
		protected override void SetCurrebtIndexer(object indexer)
		{
			_currentIndexer = (int)indexer;
		}
		protected override object CurrentItem()
		{
			return this.Item(_currentIndexer);
		}
		object _newInstance = null;
		protected override object NewItem()
		{
			return _newInstance;
		}
		protected override void RemoveItem(object indexer)
		{
			int n = (int)indexer;
			if (_i != null && n >= 0 && n < _i.Count)
			{
				_i.RemoveAt(n);
			}
		}
		protected override void RemoveItemByName(string name)
		{
			if (_i != null)
			{
				for (int i = 0; i < _i.Count; i++)
				{
					if (_i[i].Name == name)
					{
						_i.RemoveAt(i);
						break;
					}
				}
			}
		}
		protected override void RemoveAll()
		{
			_i = null;
			_newInstance = null;
		}
		protected override void CreateItem(string name)
		{
			_newInstance = this.CreateI(name).Instance;
		}
		protected override void MoveNext()
		{
			if (_i != null)
			{
				if (_currentIndexer < _i.Count - 1)
					_currentIndexer++;
			}
		}
		protected override void MoveBack()
		{
			if (_currentIndexer > 0)
				_currentIndexer--;
		}
		#endregion
	}
}
