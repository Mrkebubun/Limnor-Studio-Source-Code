/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Action;
using VPL;
using XmlUtility;
using System.ComponentModel;
using System.Drawing.Design;
using VSPrj;
using System.Globalization;
using System.Xml;
using System.Collections.Specialized;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// handle one exception
	/// </summary>
	[UseParentObject]
	public class ExceptionHandler : IWithProject, IXmlNodeHolder, ISkipWrite, IActionsHolder, ITransferBeforeWrite
	{
		#region fields and constructors
		private DataTypePointer _exceptionType;
		private BranchList _branchList;
		private MethodClass _ownerMethod;
		private XmlNode _node;
		private ComponentIconException _exceptionObject;
		private List<ComponentIconSubscopeVariable> _componentIconList;
		private Dictionary<UInt32, IAction> _acts;
		private EventHandler PropertyChanged;
		private static readonly DataTypePointer defaultExceptionType = new DataTypePointer(new TypePointer(typeof(SelectExceptionToHandle)));
		public ExceptionHandler(MethodClass owner)
		{
			_ownerMethod = owner;
			_exceptionType = defaultExceptionType;
			PropertyChanged = new EventHandler(_ownerMethod.NotifyPropertyChange);
		}
		#endregion
		#region Methods
		public bool IsSameException(ExceptionHandler eh)
		{
			if (eh != null)
			{
				if (eh.ExceptionType != null)
				{
					if (eh.ExceptionType.IsSameObjectRef(this.ExceptionType))
					{
						return true;
					}
				}
			}
			return false;
		}
		public bool IsSubExceptionOf(ExceptionHandler eh)
		{
			if (eh != null)
			{
				if (eh.ExceptionType != null)
				{
					//narrow scope can be assigned to wider scope
					//wider scope is assignable from narrow scope
					if (eh.ExceptionType.IsAssignableFrom(this.ExceptionType))
					{
						return true;
					}
				}
			}
			return false;
		}
		public void EstablishObjectOwnership(MethodClass m)
		{
			if (_exceptionObject != null)
			{
				_exceptionObject.EstablishObjectOwnership(_ownerMethod);
			}
			if (_componentIconList != null)
			{
				foreach (ComponentIconSubscopeVariable sv in _componentIconList)
				{
					sv.EstablishObjectOwnership(_ownerMethod);
				}
			}
			if (_branchList != null)
			{
				_branchList.EstablishObjectOwnership();
			}
			if (_acts != null)
			{
				foreach (IAction act in _acts.Values)
				{
					act.EstablishObjectOwnership(this);
				}
			}
		}
		public override string ToString()
		{
			return _exceptionType.DisplayName;
		}
		#endregion
		#region Properties

		[Browsable(false)]
		public bool IsDefaultExceptionType
		{
			get
			{
				return defaultExceptionType.IsSameObjectRef(_exceptionType);
			}
		}
		[PropertyReadOrder(100, true)]
		[Browsable(false)]
		public BranchList ActionList
		{
			get
			{
				return _branchList;
			}
			set
			{
				_branchList = value;
				if (_branchList != null)
				{
					_branchList.SetOwnerMethod(_ownerMethod);
				}
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		[TypeScope(typeof(Exception))]
		[Editor(typeof(PropEditorDataType), typeof(UITypeEditor))]
		[Description("The event handler type of the event. It specifies the parameters of the event. Usually two parameters are used; the first parameter is an object for the event sender; the second parameter is EventArgs or a class derived from EventArgs")]
		public DataTypePointer ExceptionType
		{
			get
			{
				return _exceptionType;
			}
			set
			{
				if (value != null)
				{
					_exceptionType = value;

					_exceptionType.TypeChanged += new EventHandler(_exceptionType_OnTypeChange);
					if (PropertyChanged != null)
					{
						PropertyChanged(this, new PropertyChangeEventArg("ExceptionHandlers", this));
					}
				}
			}
		}
		[Browsable(false)]
		public ComponentIconException ExceptionObject
		{
			get
			{
				if (_exceptionObject == null)
				{
					_exceptionObject = new ComponentIconException(_ownerMethod);
					LocalVariable lv = new LocalVariable(_exceptionType, _exceptionType.Name, _ownerMethod.RootPointer.ClassId, _exceptionObject.MemberId);
					lv.Owner = _ownerMethod;
					_exceptionObject.ClassPointer = lv;
					_exceptionObject.Location = new System.Drawing.Point(30, 30);
				}
				return _exceptionObject;
			}
			set
			{
				_exceptionObject = value;
				if (_exceptionObject != null)
				{
					_exceptionObject.SetOwnerMethod(_ownerMethod);
					if (_exceptionObject.ClassPointer == null)
					{
						LocalVariable lv = new LocalVariable(_exceptionType, _exceptionType.Name, _ownerMethod.RootPointer.ClassId, _exceptionObject.MemberId);
						lv.Owner = _ownerMethod;

						_exceptionObject.ClassPointer = lv;
					}
					else
					{
						LocalVariable lv = (LocalVariable)(_exceptionObject.ClassPointer);
						lv.Owner = _ownerMethod;
						lv.SetMemberId(_exceptionObject.MemberId);
						lv.SetDataType(_exceptionType);
					}
				}
			}
		}
		[Browsable(false)]
		public List<ComponentIconSubscopeVariable> ComponentIconList
		{
			get
			{
				if (_componentIconList == null)
					_componentIconList = new List<ComponentIconSubscopeVariable>();
				return _componentIconList;
			}
			set
			{
				_componentIconList = value;
			}
		}
		#endregion
		#region private methods
		void _exceptionType_OnTypeChange(object sender, EventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangeEventArg("ExceptionType"));
			}
		}
		#endregion
		#region IWithProject Members
		[Browsable(false)]
		public LimnorProject Project
		{
			get { return _ownerMethod.Project; }
		}

		#endregion

		#region IXmlNodeHolder Members
		[Browsable(false)]
		[ReadOnly(true)]
		public System.Xml.XmlNode DataXmlNode
		{
			get
			{
				return _node;
			}
			set
			{
				_node = value;
			}
		}

		#endregion

		#region ISkipWrite Members
		[Browsable(false)]
		public bool SkipSerialize
		{
			get
			{
				if (IsDefaultExceptionType)
				{
					return true;
				}
				return false;
			}
		}

		#endregion

		#region IActionsHolder Members

		public XmlNode ActionsNode
		{
			get
			{
				XmlNode node = this.DataXmlNode.SelectSingleNode(XmlTags.XML_ACTIONS);
				if (node == null)
				{
					node = this.DataXmlNode.OwnerDocument.CreateElement(XmlTags.XML_ACTIONS);
					this.DataXmlNode.AppendChild(node);
				}
				return node;
			}
		}
		[Browsable(false)]
		public UInt32 SubScopeId
		{
			get
			{
				return this.ExceptionType.MemberId;
			}
		}

		[Browsable(false)]
		public uint ScopeId
		{
			get { return _ownerMethod.MemberId; }
		}

		[Browsable(false)]
		public MethodClass OwnerMethod
		{
			get
			{
				return _ownerMethod;
			}
		}

		[Browsable(false)]
		public Dictionary<uint, IAction> ActionInstances
		{
			get
			{
				return _acts;
			}
		}
		public Dictionary<UInt32, IAction> GetVisibleActionInstances()
		{
			Dictionary<UInt32, IAction> lst = new Dictionary<uint, IAction>();
			if (_acts == null)
			{
				LoadActionInstances();
			}
			if (_acts != null)
			{
				foreach (KeyValuePair<UInt32, IAction> kv in _acts)
				{
					if (!lst.ContainsKey(kv.Key))
					{
						lst.Add(kv.Key, kv.Value);
					}
				}
			}
			Dictionary<uint, IAction> acts = OwnerMethod.RootPointer.ActionInstances;
			if (acts == null)
			{
				OwnerMethod.RootPointer.LoadActionInstances();
			}
			if (acts != null)
			{
				foreach (KeyValuePair<UInt32, IAction> kv in acts)
				{
					if (!lst.ContainsKey(kv.Key))
					{
						lst.Add(kv.Key, kv.Value);
					}
				}
			}
			return lst;
		}
		public void SetActionInstances(Dictionary<uint, IAction> actions)
		{
			_acts = actions;
		}
		public void LoadActionInstances()
		{
			this._ownerMethod.RootPointer.LoadActions(this);
		}
		[Browsable(false)]
		public IAction GetActionInstance(UInt32 actId)
		{
			return ClassPointer.GetActionObject(actId, this, this.OwnerMethod.RootPointer);
		}
		public ActionBranch FindActionBranchById(UInt32 branchId)
		{
			if (_branchList != null)
			{
				return _branchList.SearchBranchById(branchId);
			}
			return null;
		}
		public void GetActionNames(StringCollection sc)
		{
			if (_branchList != null)
			{
				_branchList.GetActionNames(sc);
			}
			if (_acts == null)
			{
				LoadActionInstances();
			}
			if (_acts != null)
			{
				foreach (IAction a in _acts.Values)
				{
					if (a != null)
					{
						if (!string.IsNullOrEmpty(a.ActionName))
						{
							if (!sc.Contains(a.ActionName))
							{
								sc.Add(a.ActionName);
							}
						}
					}
				}
			}
		}
		[Browsable(false)]
		public void AddActionInstance(IAction action)
		{
			if (_acts == null)
			{
				_acts = new Dictionary<uint, IAction>();
			}
			if (_acts != null)
			{
				bool found = _acts.ContainsKey(action.ActionId);
				if (!found)
				{
					_acts.Add(action.ActionId, action);
				}
			}
		}
		[Browsable(false)]
		public IAction TryGetActionInstance(UInt32 actId)
		{
			if (_acts != null)
			{
				IAction a;
				if (_acts.TryGetValue(actId, out a))
				{
					return a;
				}
			}
			return null;
		}
		[Browsable(false)]
		public void DeleteActions(List<UInt32> actionIds)
		{

			if (_acts != null)
			{
				foreach (UInt32 id in actionIds)
				{
					IAction a;
					if (_acts.TryGetValue(id, out a))
					{
						if (a != null)
						{
							ClassPointer.DeleteAction(_node, id);
						}
						_acts.Remove(id);
					}
				}
			}
		}
		#endregion
	}

	[UseParentObject]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ExceptionHandlerList : List<ExceptionHandler>, ICustomTypeDescriptor, IWithProject
	{
		#region fields and constructors
		private MethodClass _ownerMethod;
		public ExceptionHandlerList(MethodClass owner)
		{
			_ownerMethod = owner;
		}
		#endregion

		#region Methods
		public void SetOwner(MethodClass owner)
		{
			_ownerMethod = owner;
		}
		
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Handles {0} type of exceptions", this.Count - 1);
		}
		#endregion

		#region Properties
		public MethodClass OwnerMethod
		{
			get
			{
				return _ownerMethod;
			}
		}
		public bool IsEmpty
		{
			get
			{
				foreach (ExceptionHandler eh in this)
				{
					if (!eh.IsDefaultExceptionType)
					{
						if (eh.ActionList != null && eh.ActionList.Count > 0)
						{
							return false;
						}
					}
				}
				return true;
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

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<PropertyDescriptor> pdl = new List<PropertyDescriptor>();
			int n = 0;
			if (attributes != null)
			{
				n = attributes.Length;
			}
			foreach (ExceptionHandler eh in this)
			{
				Attribute[] attrs;
				if (eh.IsDefaultExceptionType)
				{
					attrs = new Attribute[n + 4];
				}
				else
				{
					attrs = new Attribute[n + 3];
				}
				if (n > 0)
				{
					attributes.CopyTo(attrs, 0);
				}
				if (eh.IsDefaultExceptionType)
				{
					attrs[n] = new DescriptionAttribute("Select a type of exceptions to be handled. Choose type Exception under System namespace for generic exceptions");
					attrs[n + 1] = new EditorAttribute(typeof(PropEditorDataType), typeof(UITypeEditor));
					attrs[n + 2] = new TypeScopeAttribute(typeof(Exception));
					attrs[n + 3] = new RefreshPropertiesAttribute(RefreshProperties.All);
				}
				else
				{
					attrs[n] = new DescriptionAttribute(string.Format(CultureInfo.InvariantCulture, "Specify actions for handling exceptions of type {0}", eh.ExceptionType.DisplayName));
					attrs[n + 1] = new EditorAttribute(typeof(TypeEditorExceptionHandler), typeof(UITypeEditor));
					attrs[n + 2] = new RefreshPropertiesAttribute(RefreshProperties.All);
				}
				pdl.Add(new PropertyDescriptorExceptionHandler(eh, attrs));
			}
			return new PropertyDescriptorCollection(pdl.ToArray());
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

		#region PropertyDescriptorExceptionHandler
		internal class PropertyDescriptorExceptionHandler : PropertyDescriptor
		{
			private ExceptionHandler _eh;
			public PropertyDescriptorExceptionHandler(ExceptionHandler eh, Attribute[] attrs)
				: base(eh.ExceptionType.Name, attrs)
			{
				_eh = eh;
			}

			public ExceptionHandler Handler
			{
				get
				{
					return _eh;
				}
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(ExceptionHandlerList); }
			}

			public override object GetValue(object component)
			{
				if (_eh.ActionList == null)
				{
					return string.Empty;
				}
				return string.Format(CultureInfo.InvariantCulture, "Action count:{0}", _eh.ActionList.GetActionCount());
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get { return typeof(string); }
			}

			public override void ResetValue(object component)
			{
				_eh.ActionList = null;
			}

			public override void SetValue(object component, object value)
			{
				_eh.ActionList = (BranchList)value;
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}
		}
		#endregion

		#region IWithProject Members

		public LimnorProject Project
		{
			get { return _ownerMethod.Project; }
		}

		#endregion
	}
}
