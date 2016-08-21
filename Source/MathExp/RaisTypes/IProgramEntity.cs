/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.CodeDom;
using System.Drawing;
using VPLDrawing;
using VPL;
using System.Collections.Specialized;

namespace MathExp.RaisTypes
{
	public enum EnumDataFlowType
	{
		Push = 0,
		Pull = 1
	}
	/// <summary>
	/// used by third party library
	/// </summary>
	public interface IVisualEntity
	{
		/// <summary>
		/// a third party library may use its own drawing type.
		/// </summary>
		/// <param name="pe"></param>
		/// <returns>it must be a type derived from ObjectDrawing</returns>
		Type GetVisualEntityType(IProgramEntity pe);
	}
	public interface IProgramEntity : ICloneable, IXmlNodeSerializable
	{
		ProgramEntity ProgEntity { get; set; }
		string Name { get; }
		bool CanAddInputs { get; }
		bool CanAddOutputs { get; }
		UInt32 ID { get; }
		/// <summary>
		/// use the port selection to adjust the code for referring the property
		/// </summary>
		/// <param name="portSelection">property reference</param>
		/// <param name="method">method the code is used for</param>
		/// <returns>code adjusted</returns>
		CodeExpression ReturnCodeExpression(object portSelection, IMethodCompile method);
		string CreateJavaScript(object portSelection, StringCollection code);
		string CreatePhpScript(object portSelection, StringCollection code);
		Bitmap CreateDrawIcon();
	}
	public interface IProgramPort
	{
		ObjectRef PortProperty { get; set; }
	}
	/// <summary>
	/// program entity used in methods.
	/// it may have inports to represnt the setting of the properties
	/// and outports to represent the getting of the properties.
	/// </summary>
	public class ProgramEntity : IPortOwner, IXmlNodeSerializable, ICloneable
	{
		#region fields and constructors
		public const string XML_PortProperty = "PortProperty";
		const string XML_Owner = "Owner";
		const string XML_Inports = "Inports";
		const string XML_Outports = "Outports";
		const string XML_NewInport = "NewInport";
		const string XML_NewOutport = "NewOutport";
		/// <summary>
		/// object providing the properties, methods and access to XML
		/// </summary>
		private ObjectRef _owner;
		private ProgramInPort[] _inports;
		private ProgramOutPort[] _outports;
		private LinkLineNodeInPort _newInport;
		private LinkLineNodeOutPort _newOutport;
		/// <summary>
		/// it should be only used for deserialization so that OnReadFromXmlNode()
		/// can be called to retreive _owner
		/// </summary>
		public ProgramEntity()
		{
		}
		public ProgramEntity(ObjectRef owner)
		{
			_owner = owner;
		}
		#endregion
		#region properties
		public LinkLineNodeInPort NewInport
		{
			get
			{
				return _newInport;
			}
			set
			{
				_newInport = value;
			}
		}
		public LinkLineNodeOutPort NewOutport
		{
			get
			{
				return _newOutport;
			}
			set
			{
				_newOutport = value;
			}
		}
		public string Name
		{
			get
			{
				return _owner.Name;
			}
		}
		public UInt32 ID
		{
			get
			{
				return _owner.ID;
			}
		}
		public ObjectRef OwnerObject
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = value;
			}
		}
		/// <summary>
		/// corresponding to reading of properties from EntityType (ObjectRef)
		/// </summary>
		public ProgramInPort[] Inports
		{
			get
			{
				return _inports;
			}
			set
			{
				_inports = value;
			}
		}
		public int InPortCount
		{
			get
			{
				if (_inports == null)
				{
					return 0;
				}
				return _inports.Length;
			}
		}
		/// <summary>
		/// corresponding to writing of properties from EntityType (ObjectRef)
		/// </summary>
		public ProgramOutPort[] Outports
		{
			get
			{
				return _outports;
			}
			set
			{
				_outports = value;
			}
		}
		public ObjectRef EntityType
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = value;
			}
		}
		#endregion
		#region methods
		public virtual PortCollection GetAllPorts()
		{
			PortCollection ports = new PortCollection();
			if (_newInport == null)
			{
				_newInport = new ProgramNewInPort(this);
			}
			if (_newOutport == null)
			{
				_newOutport = new ProgramNewOutPort(this);
			}
			ports.Add(_newInport);
			ports.Add(_newOutport);
			if (_inports != null)
			{
				ports.AddRange(_inports);
			}
			if (_outports != null)
			{
				ports.AddRange(_outports);
			}
			return ports;
		}
		public void AddInport(ProgramInPort p)
		{
			int n;
			if (_inports == null)
			{
				_inports = new ProgramInPort[1];
				n = 0;
			}
			else
			{
				n = _inports.Length;
				ProgramInPort[] a = new ProgramInPort[n + 1];
				for (int i = 0; i < n; i++)
				{
					a[i] = _inports[i];
				}
				_inports = a;
			}
			_inports[n] = p;
		}
		public void RemoveInport(int id)
		{
			if (_inports != null)
			{
				for (int i = 0; i < _inports.Length; i++)
				{
					if (_inports[i].PortID == id)
					{
						if (_inports.Length == 1)
						{
							_inports = null;
						}
						else
						{
							ProgramInPort[] a = new ProgramInPort[_inports.Length - 1];
							for (int k = 0; k < _inports.Length; k++)
							{
								if (k < i)
									a[k] = _inports[k];
								else if (k > i)
								{
									a[k - 1] = _inports[k];
								}
							}
							_inports = a;
						}
						break;
					}
				}
			}
		}
		public void AddOutport(ProgramOutPort p)
		{
			int n;
			if (_outports == null)
			{
				_outports = new ProgramOutPort[1];
				n = 0;
			}
			else
			{
				n = _outports.Length;
				ProgramOutPort[] a = new ProgramOutPort[n + 1];
				for (int i = 0; i < n; i++)
				{
					a[i] = _outports[i];
				}
				_outports = a;
			}
			_outports[n] = p;
		}
		public void RemoveOutport(UInt32 id)
		{
			if (_outports != null)
			{
				for (int i = 0; i < _outports.Length; i++)
				{
					if (_outports[i].PortID == id)
					{
						if (_outports.Length == 1)
						{
							_outports = null;
						}
						else
						{
							ProgramOutPort[] a = new ProgramOutPort[_outports.Length - 1];
							for (int k = 0; k < _outports.Length; k++)
							{
								if (k < i)
									a[k] = _outports[k];
								else if (k > i)
								{
									a[k - 1] = _outports[k];
								}
							}
							_outports = a;
						}
						break;
					}
				}
			}
		}
		public override string ToString()
		{
			return _owner.ToString();
		}
		#endregion
		#region IPortOwner Members
		public UInt32 PortOwnerID
		{
			get
			{
				return this.ID;
			}
		}
		public bool IsDummyPort
		{
			get { return false; }
		}

		public void AddOutPort(LinkLineNodeOutPort port)
		{
			AddOutport((ProgramOutPort)port);
		}

		public void AddInPort(LinkLineNodeInPort port)
		{
			AddInport((ProgramInPort)port);
		}

		public void RemovePort(LinkLineNodePort port)
		{
		}

		public string TraceInfo
		{
			get
			{
				return "Program Entuty '" + _owner.Name + "'";
			}
		}

		#endregion
		#region ICloneable Members

		public object Clone()
		{
			ProgramEntity obj = new ProgramEntity(
				(ObjectRef)_owner.Clone());
			if (_inports != null)
			{
				ProgramInPort[] a = new ProgramInPort[_inports.Length];
				for (int i = 0; i < _inports.Length; i++)
				{
					_inports[i].ConstructorParameters = new object[] { obj };
					a[i] = (ProgramInPort)_inports[i].Clone();
				}
				obj.Inports = a;
			}
			if (_outports != null)
			{
				ProgramOutPort[] a = new ProgramOutPort[_outports.Length];
				for (int i = 0; i < _outports.Length; i++)
				{
					_outports[i].ConstructorParameters = new object[] { obj };
					a[i] = (ProgramOutPort)_outports[i].Clone();
				}
				obj.Outports = a;
			}
			if (_newInport != null)
			{
				_newInport.ConstructorParameters = new object[] { obj };
				obj.NewInport = (LinkLineNodeInPort)_newInport.Clone();
			}
			if (_newOutport != null)
			{
				_newOutport.ConstructorParameters = new object[] { obj };
				obj.NewOutport = (LinkLineNodeOutPort)_newOutport.Clone();
			}
			return obj;
		}

		#endregion
		#region IXmlNodeSerializable Members

		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_owner = (ObjectRef)XmlSerialization.ReadFromChildXmlNode(serializer, node, XML_Owner);
			XmlNodeList nodes = node.SelectNodes(XmlSerialization.FormatString("{0}/{1}",
				XML_Inports, XmlSerialization.XML_PORT));
			_inports = new ProgramInPort[nodes.Count];
			for (int i = 0; i < nodes.Count; i++)
			{
				_inports[i] = (ProgramInPort)XmlSerialization.ReadFromXmlNode(serializer, nodes[i], this);
			}
			nodes = node.SelectNodes(XmlSerialization.FormatString("{0}/{1}",
				XML_Outports, XmlSerialization.XML_PORT));
			_outports = new ProgramOutPort[nodes.Count];
			for (int i = 0; i < nodes.Count; i++)
			{
				_outports[i] = (ProgramOutPort)XmlSerialization.ReadFromXmlNode(serializer, nodes[i], this);
			}
			_newInport = (LinkLineNodeInPort)XmlSerialization.ReadFromChildXmlNode(serializer, node, XML_NewInport, this);
			_newOutport = (LinkLineNodeOutPort)XmlSerialization.ReadFromChildXmlNode(serializer, node, XML_NewOutport, this);
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlSerialization.WriteToChildXmlNode(serializer, node, XML_Owner, _owner);
			if (_inports != null)
			{
				XmlNode portsNode = node.SelectSingleNode(XML_Inports);
				if (portsNode == null)
				{
					portsNode = node.OwnerDocument.CreateElement(XML_Inports);
					node.AppendChild(portsNode);
				}
				else
				{
					portsNode.RemoveAll();
				}
				for (int i = 0; i < _inports.Length; i++)
				{
					XmlSerialization.WriteToChildXmlNode(serializer, portsNode, XmlSerialization.XML_PORT, _inports[i]);
				}
			}
			if (_outports != null)
			{
				XmlNode portsNode = node.SelectSingleNode(XML_Outports);
				if (portsNode == null)
				{
					portsNode = node.OwnerDocument.CreateElement(XML_Outports);
					node.AppendChild(portsNode);
				}
				else
				{
					portsNode.RemoveAll();
				}
				for (int i = 0; i < _outports.Length; i++)
				{
					XmlSerialization.WriteToChildXmlNode(serializer, portsNode, XmlSerialization.XML_PORT, _outports[i]);
				}
			}
			XmlSerialization.WriteToUniqueChildXmlNode(serializer, node, XML_NewInport, _newInport);
			XmlSerialization.WriteToUniqueChildXmlNode(serializer, node, XML_NewOutport, _newOutport);
		}

		#endregion
	}
	public class ProgramInPort : LinkLineNodeInPort, IProgramPort
	{
		#region fields and constructors
		//private int _id;
		private EnumDataFlowType _linkType = EnumDataFlowType.Push;
		private ObjectRef _property;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner">it should be a ProgramEntity</param>
		public ProgramInPort(IPortOwner owner)
			: base(owner)
		{
		}
		#endregion
		#region properties
		/// <summary>
		/// property reference for the port.
		/// </summary>
		public ObjectRef PortProperty
		{
			get
			{
				return _property;
			}
			set
			{
				_property = value;
			}
		}
		public EnumDataFlowType DataFlowType
		{
			get
			{
				return _linkType;
			}
			set
			{
				_linkType = value;
			}
		}
		public override bool CanAssignValue
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region methods
		#endregion
		#region ICloneable Members
		public override object Clone()
		{
			ProgramInPort obj = (ProgramInPort)base.Clone();
			if (_property != null)
			{
				obj.PortProperty = (ObjectRef)_property.Clone();
			}
			obj.DataFlowType = this.DataFlowType;
			return obj;
		}
		#endregion
		#region IXmlNodeSerializable Members
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			_linkType = (EnumDataFlowType)XmlSerialization.GetAttributeEnum(node, XmlSerialization.XMLATT_DataLinkType, typeof(EnumDataFlowType));
			XmlNode nd = node.SelectSingleNode(ProgramEntity.XML_PortProperty);
			if (nd != null)
			{
				_property = (ObjectRef)XmlSerialization.ReadFromXmlNode(serializer, nd, this);
				((MathNodeVariable)(this._label.Variable.MathExpression[0])).VariableName = _property.localName;
			}
		}
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_ID, PortID.ToString());
			XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_DataLinkType, _linkType);
			if (_property != null)
			{
				XmlSerialization.WriteToChildXmlNode(serializer, node, ProgramEntity.XML_PortProperty, _property);
			}
		}
		#endregion
	}
	public class ProgramOutPort : LinkLineNodeOutPort, IProgramPort
	{
		#region fields and constructors
		private ObjectRef _property;
		/// <summary>
		/// property reference for the port
		/// </summary>
		/// <param name="owner">it should be a ProgramEntity</param>
		public ProgramOutPort(IPortOwner owner)
			: base(owner)
		{
		}
		#endregion
		#region properties
		/// <summary>
		/// property reference for the port
		/// </summary>
		public ObjectRef PortProperty
		{
			get
			{
				return _property;
			}
			set
			{
				_property = value;
			}
		}
		#endregion
		#region methods
		protected override void OnCreateContextMenu(ContextMenu mnu)
		{
			base.OnCreateContextMenu(mnu);
			if (CanRemovePort && this.LinkedPortID == 0)
			{
				MenuItem mi = new MenuItem("Remove port");
				mi.Click += new EventHandler(mnuRemovePort_Click);
				mnu.MenuItems.Add(mi);
			}
		}

		void mnuRemovePort_Click(object sender, EventArgs e)
		{
			Control p = this.Parent;
			if (p != null)
			{
				ObjectDrawing ov = this.Owner as ObjectDrawing;
				if (ov != null && ov.ObjectEntity != null && ov.ObjectEntity.ProgEntity != null)
				{
					ov.ObjectEntity.ProgEntity.RemoveOutport(this.PortID);
					this.ClearLine();
					p.Controls.Remove(this.Label);
					p.Controls.Remove(this);
					ILinkLineNode l = this.NextNode;
					while (l != null)
					{
						l.ClearLine();
						p.Controls.Remove((Control)l);
						l = l.NextNode;
					}
					p.Refresh();
					IMathDesigner im = p as IMathDesigner;
					if (im != null)
					{
						im.Changed = true;
					}
				}
			}
		}
		#endregion
		#region ICloneable Members
		public override object Clone()
		{
			ProgramOutPort obj = (ProgramOutPort)base.Clone();
			if (_property != null)
			{
				obj.PortProperty = (ObjectRef)_property.Clone();
			}
			return obj;
		}
		#endregion
		#region IXmlNodeSerializable Members
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			XmlNode nd = node.SelectSingleNode(ProgramEntity.XML_PortProperty);
			if (nd != null)
			{
				_property = (ObjectRef)XmlSerialization.ReadFromXmlNode(serializer, nd, this);
				((MathNodeVariable)(this._label.Variable.MathExpression[0])).VariableName = _property.localName;
			}
		}
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlSerialization.SetAttribute(node, XmlSerialization.XMLATT_ID, PortID.ToString());
			XmlSerialization.WriteToUniqueChildXmlNode(serializer, node, ProgramEntity.XML_PortProperty, _property);
		}
		#endregion
	}
	public class ProgramNewInPort : ProgramInPort
	{
		#region fields and constructors
		private Brush _drawBrush = Brushes.SteelBlue;
		public ProgramNewInPort(IPortOwner owner)
			: base(owner)
		{
			this.LabelVisible = false;
		}
		#endregion
		#region properties
		public override bool CanStartLink
		{
			get
			{
				return false;
			}
		}
		public override bool CanRemovePort
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region methods
		private void mnuNewPort_Click(object sender, EventArgs e)
		{
			Control p = this.Parent;
			if (p != null)
			{
				ObjectDrawing ov = this.Owner as ObjectDrawing;
				if (ov != null && ov.ObjectEntity != null && ov.ObjectEntity.ProgEntity != null)
				{
					ov.CreateInport();
				}
			}
		}
		protected override void OnCreateContextMenu(ContextMenu mnu)
		{
			base.OnCreateContextMenu(mnu);
			MenuItem mi = new MenuItem("Create new port");
			mi.Click += new EventHandler(mnuNewPort_Click);
			mnu.MenuItems.Add(mi);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			VplDrawing.DrawInArrow(e.Graphics, _drawBrush, this.Size, this.PositionType);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			this.LabelVisible = false;
		}
		protected override LinkLineNode CreateTargetLinkNode()
		{
			Control p = this.Parent;
			if (p != null)
			{
				ObjectDrawing ov = this.Owner as ObjectDrawing;
				if (ov != null && ov.ObjectEntity != null && ov.ObjectEntity.ProgEntity != null)
				{
					LinkLineNode ll = ov.CreateInport();
					if (ll != null)
					{
						return ll.Start;
					}
				}
			}
			return null;
		}
		protected override void Init()
		{
			base.Init();
			this.Cursor = Cursors.Default;
		}
		#endregion
	}
	public class ProgramNewOutPort : ProgramOutPort
	{
		#region fields and constructors
		private Brush _drawBrush = Brushes.Violet;
		public ProgramNewOutPort(IPortOwner owner)
			: base(owner)
		{
			this.LabelVisible = false;
		}
		#endregion
		#region properties
		public override bool CanStartLink
		{
			get
			{
				return false;
			}
		}
		public override bool CanRemovePort
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region methods
		private void mnuNewPort_Click(object sender, EventArgs e)
		{
			Control p = this.Parent;
			if (p != null)
			{
				ObjectDrawing ov = this.Owner as ObjectDrawing;
				if (ov != null && ov.ObjectEntity != null && ov.ObjectEntity.ProgEntity != null)
				{
					ov.CreateOutport();
				}
			}
		}
		protected override void OnCreateContextMenu(ContextMenu mnu)
		{
			base.OnCreateContextMenu(mnu);
			MenuItem mi = new MenuItem("Create new port");
			mi.Click += new EventHandler(mnuNewPort_Click);
			mnu.MenuItems.Add(mi);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			VplDrawing.DrawOutArrow(e.Graphics, _drawBrush, this.Size, this.PositionType);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			this.LabelVisible = false;
		}
		protected override LinkLineNode CreateTargetLinkNode()
		{
			Control p = this.Parent;
			if (p != null)
			{
				ObjectDrawing ov = this.Owner as ObjectDrawing;
				if (ov != null && ov.ObjectEntity != null && ov.ObjectEntity.ProgEntity != null)
				{
					LinkLineNode ll = ov.CreateOutport();
					if (ll != null)
					{
						return ll.End;
					}
				}
			}
			return null;
		}
		protected override void Init()
		{
			base.Init();
			this.Cursor = Cursors.Default;
		}
		#endregion
	}
}
