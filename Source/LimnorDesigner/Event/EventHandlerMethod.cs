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
using System.ComponentModel;
using MathExp;
using LimnorDesigner.MethodBuilder;
using System.Windows.Forms;
using System.Xml;
using XmlUtility;
using XmlSerializer;
using System.Drawing;
using LimnorDesigner.Action;
using System.Globalization;
using VPL;
using TraceLog;

namespace LimnorDesigner.Event
{
	//public enum EnumEditWebHandler { EditServer = 0, EditClient, EditDownload }
	/// <summary>
	/// a method attached to an event as a handler.
	/// it is saved inside Item element for TaskID as one item of the TaskIDList.
	/// when read it back, its Parameters should be fetched from the event.
	/// </summary>
	public class EventHandlerMethod : MethodClass
	{
		#region fields and constructors
		private IEvent _event;
		private UInt32 _actionBranchId;
		private EventAction _handlerOwner;
		public EventHandlerMethod(ClassPointer owner)
			: base(owner)
		{
		}
		public EventHandlerMethod(ClassPointer owner, IEvent eventOwner)
			: base(owner)
		{
			SetEvent(eventOwner);
		}
		public EventHandlerMethod(HandlerMethodID taskId)
			: base(DesignUtil.LoadComponentClass(XmlObjectReader.CurrentProject, taskId.DataXmlNode.OwnerDocument.DocumentElement))
		{
		}
		#endregion
		#region Methods
		
		[Browsable(false)]
		public UInt32 GetDynamicActionId()
		{
			XmlNode node = XmlData;
			if (node != null)
			{
				while (node != null && string.CompareOrdinal(XmlTags.XML_HANDLER, node.Name) != 0)
				{
					node = node.ParentNode;
				}
				if (node != null && string.CompareOrdinal(XmlTags.XML_HANDLER, node.Name) == 0)
				{
					return XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ActionID);
				}
			}
			return 0;
		}
		[Browsable(false)]
		public EventAction GetHandlerOwner()
		{
			return _handlerOwner;
		}
		const string XMLATT_forallTypes = "forAllTypes";
		protected override void OnBeforeRead()
		{
			ForAllTypes = false;
			base.OnBeforeRead();
		}
		protected override void OnAfterRead()
		{
			ForAllTypes = XmlUtil.GetAttributeBoolDefFalse(XmlData, XMLATT_forallTypes);
			base.OnAfterRead();
		}
		public override void OnBeforeWrite(XmlObjectWriter writer, XmlNode node)
		{
			if (XmlData != null)
			{
				XmlNode actsNode = node.SelectSingleNode(XmlTags.XML_ACTIONS);
				if (actsNode == null)
				{
					XmlNode a2 = XmlData.SelectSingleNode(XmlTags.XML_ACTIONS);
					if (a2 != null)
					{
						actsNode = node.OwnerDocument.ImportNode(a2, true);
						node.AppendChild(actsNode);
					}
				}
			}
			base.OnBeforeWrite(writer, node);
			if (ForAllTypes)
			{
				XmlUtil.SetAttribute(node, XMLATT_forallTypes, ForAllTypes);
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XMLATT_forallTypes);
			}
		}
		protected override PropertyDescriptor OnGettingPropertyDescriptor(PropertyDescriptor p, AttributeCollection attrs)
		{
			if (string.CompareOrdinal(p.Name, "ForAllTypes") == 0)
			{
				Attribute[] attrs0;
				if (attrs == null)
				{
					attrs0 = new Attribute[] { };
				}
				else
				{
					attrs0 = new Attribute[attrs.Count];
					attrs.CopyTo(attrs0, 0);
				}
				return new PropertyDescriptorForAll(string.Format(CultureInfo.InvariantCulture,
					"ForAll{0}s", this.Event.Owner.ObjectType.Name), attrs0, this);
			}
			return p;
		}
		public override List<ConstObjectPointer> GetCustomAttributeList()
		{
			return new List<ConstObjectPointer>();
		}
		protected override void InitializeNewMethod(XmlNode rootNode, ILimnorDesignerLoader loader)
		{
			MemberId = (UInt32)Guid.NewGuid().GetHashCode();
			if (string.IsNullOrEmpty(Name))
				Name = loader.CreateMethodName("h", null);
			else
				Name = loader.CreateMethodName(Name, null);
		}
		public override void RemoveMethodXmlNode(XmlNode rootNode)
		{
			string path = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}/{1}/{2}[@{3}='TaskIDList']/{4}/{2}[@{3}='HandlerMethod']/{5}/{2}[@{3}='MemberId' and text()='{6}']",
					XmlTags.XML_HANDLERLISTS, XmlTags.XML_HANDLER, XmlTags.XML_PROPERTY,
					XmlTags.XMLATT_NAME, XmlTags.XML_Item, XmlTags.XML_ObjProperty, this.MethodID
					);
			XmlNode nodeMethod = rootNode.SelectSingleNode(path);
			if (nodeMethod != null)
			{
				XmlNode itemNode = nodeMethod.ParentNode;
				while (itemNode != null && itemNode.Name != XmlTags.XML_Item)
				{
					itemNode = itemNode.ParentNode;
				}
				if (itemNode == null)
				{
					throw new DesignerException("Invalid Method path {0}", path);
				}
				XmlNode p = itemNode.ParentNode;
				p.RemoveChild(itemNode);
				if (!p.HasChildNodes)
				{
					itemNode = p.ParentNode;
					while (itemNode != null && itemNode.Name != XmlTags.XML_HANDLER)
					{
						itemNode = itemNode.ParentNode;
					}
					p = itemNode.ParentNode;
					p.RemoveChild(itemNode);
				}
			}
		}
		public void SetEvent(IEvent e)
		{
			_event = e;
			Parameters = _event.GetParameters(this);
		}
		public DlgMethod GetEditDialog(Rectangle rcStart, ILimnorDesignerLoader loader)
		{
			if (Owner == null)
			{
				MathNode.Log(loader.DesignPane.Window as Form,new DesignerException("Calling EventHandlerMethod.Edit with null method owner"));
			}
			if (this.Owner == null)
			{
				this.Owner = loader.GetRootId();
			}
			DlgMethod dlg = this.CreateMethodEditor(rcStart);
			if (Parameters != null && Parameters.Count > 0)
			{
				if (typeof(object).Equals(Parameters[0].ObjectType) && string.CompareOrdinal(Parameters[0].Name, "sender") == 0)
				{
					Parameters[0] = new ParameterClass(this.Event.Owner.ObjectType, "sender", this);
					Parameters[0].ReadOnly = true;
				}
				if (Parameters.Count > 1)
				{
					if (typeof(EventArgs).Equals(Parameters[1].ObjectType) && string.CompareOrdinal(Parameters[1].Name, "e") == 0)
					{
						ICustomEventMethodDescriptor ce = this.Event.Owner.ObjectInstance as ICustomEventMethodDescriptor;
						if (ce != null)
						{
							Type pType = ce.GetEventArgumentType(this.Event.Name);
							if (pType != null)
							{
								Parameters[1] = new ParameterClass(pType, "e", this);
								Parameters[1].ReadOnly = true;
							}
						}
					}
				}
			}
			dlg.LoadMethod(this, EnumParameterEditType.ReadOnly);
			return dlg;
		}
		public void OnFinishEdit(UInt32 actionBranchId, ILimnorDesignerLoader loader)
		{
			try
			{
				ILimnorDesignPane dp = loader.DesignPane;
				if (dp == null)
				{
					throw new DesignerException("ILimnorDesignPane not found for class {0}", loader.GetRootId());
				}
				IEvent ep = _event;
				EventAction ea = null; //event-actions mapping
				ClassPointer a = loader.GetRootId();
				if (a != null)
				{
					List<EventAction> lst = a.EventHandlers;
					if (lst != null)
					{
						foreach (EventAction e in lst)
						{
							if (e.GetAssignActionId() == actionBranchId && e.Event.IsSameObjectRef(ep))
							{
								ea = e;
								break;
							}
						}
					}
				}
				if (ea == null)
				{
					ea = new EventAction();
					ea.Event = ep;
					if (actionBranchId != 0)
					{
						ea.SetAssignActionId(actionBranchId);
					}
				}
				_handlerOwner = ea;
				ea.AddAction(this);
				a.SaveEventHandler(ea);
				dp.OnNotifyChanges();
				dp.OnAssignAction(ea);
			}
			catch (Exception err)
			{
				MathNode.Log(TraceLogClass.MainForm,err);
			}
			finally
			{
				this.ExitEditor();
			}
		}
		private EnumRunContext _origiContext = EnumRunContext.Server;
		public override bool Edit(UInt32 actionBranchId, Rectangle rcStart, ILimnorDesignerLoader loader, Form caller)
		{
			try
			{
				_origiContext = VPLUtil.CurrentRunContext;
				if (loader.Project.IsWebApplication)
				{
					if (this.RunAt == EnumWebRunAt.Client)
					{
						VPLUtil.CurrentRunContext = EnumRunContext.Client;
					}
					else
					{
						VPLUtil.CurrentRunContext = EnumRunContext.Server;
					}
				}
				else
				{
					VPLUtil.CurrentRunContext = EnumRunContext.Server;
				}
				DlgMethod dlg = GetEditDialog(rcStart, loader);
				if (dlg.ShowDialog(caller) == DialogResult.OK)
				{
					OnFinishEdit(actionBranchId, loader);
					return true;
				}
			}
			catch (Exception err)
			{
				MathNode.Log(caller,err);
			}
			finally
			{
				VPLUtil.CurrentRunContext = _origiContext;
			}
			return false;
		}
		#endregion
		#region PropertyDescriptorForAll
		class PropertyDescriptorForAll : PropertyDescriptor
		{
			EventHandlerMethod _owner;
			public PropertyDescriptorForAll(string name, Attribute[] attrs, EventHandlerMethod owner)
				: base(name, attrs)
			{
				_owner = owner;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return _owner.GetType(); }
			}

			public override object GetValue(object component)
			{
				return _owner.ForAllTypes;
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return typeof(bool); }
			}

			public override void ResetValue(object component)
			{
				_owner.ForAllTypes = false;
			}

			public override void SetValue(object component, object value)
			{
				_owner.ForAllTypes = Convert.ToBoolean(value);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return _owner.ForAllTypes;
			}
		}
		#endregion
		#region Properties
		[ReadOnly(true)]
		[Browsable(false)]
		public override List<ParameterClass> Parameters
		{
			get
			{
				return base.Parameters;
			}
			set
			{
				base.Parameters = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IEvent Event
		{
			get
			{
				return _event;
			}
		}
		[Browsable(false)]
		public override bool NoReturn
		{
			get
			{
				return true;
			}
		}
		public override EnumWebRunAt RunAt
		{
			get
			{
				if (_event != null)
				{
					return _event.RunAt;
				}
				return base.RunAt;
			}
		}
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether the event handler will be applied to all the same types.")]
		public bool ForAllTypes
		{
			get;
			set;
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public UInt32 ActionBranchId
		{
			get
			{
				if (_actionBranchId == 0)
				{
					XmlNode nd = this.XmlData;
					while (nd != null && string.CompareOrdinal(nd.Name, XmlTags.XML_HANDLER) != 0)
					{
						nd = nd.ParentNode;
					}
					if (nd != null)
					{
						_actionBranchId = XmlUtil.GetAttributeUInt(nd, XmlTags.XMLATT_ActionID);
					}
				}
				return _actionBranchId;
			}
			set
			{
				_actionBranchId = value;
			}
		}
		#endregion
	}
}
