/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using VPL;

namespace MathExp.RaisTypes
{
	/// <summary>
	/// drawing for a program entity
	/// </summary>
	public abstract class ObjectDrawing : ActiveDrawing, IMathExpViewer
	{
		#region fields and constructors
		private const string XML_ENTITY = "Entity";
		private int _dotSize = 4;
		private int _dotSizeHalf = 2;
		private IProgramEntity _entity; //its ProgEntity (ProgramEntity) contains ports
		private XmlNode _xmlNode;
		private IVplDesigner _ownerViewer;
		public ObjectDrawing()
		{
			this.Size = new System.Drawing.Size(_dotSize * _dotSize, _dotSize * _dotSize);
		}
		public ObjectDrawing(IProgramEntity pe)
			: this()
		{
			_entity = pe;
		}
		#endregion
		#region properties
		public int DotSize
		{
			get
			{
				return _dotSize;
			}
			set
			{
				_dotSize = value;
				_dotSizeHalf = _dotSize / 2;
				this.Size = new System.Drawing.Size(_dotSize * _dotSize, _dotSize * _dotSize);
			}
		}
		/// <summary>
		/// its ProgEntity member is a ProgramEntity which has object reference and ports:
		///     ObjectRef OwnerObject
		///     ProgramInPort[] Inports
		///     ProgramOutPort[] Outports
		/// ProgramEntity implements IPortOwner to own and manage the ports
		/// </summary>
		public IProgramEntity ObjectEntity
		{
			get
			{
				return _entity;
			}
			set
			{
				_entity = value;
				OnEntityChanged();
			}
		}
		public int DotSizeHalf
		{
			get
			{
				return _dotSizeHalf;
			}
		}
		public UInt32 ID
		{
			get
			{
				return _entity.ID;
			}
		}
		public XmlNode XmlData
		{
			get
			{
				return _xmlNode;
			}
			set
			{
				_xmlNode = value;
			}
		}
		public IVplDesigner VplDesigner
		{
			get
			{
				return _ownerViewer;
			}
			set
			{
				_ownerViewer = value;
			}
		}
		#endregion
		#region methods
		/// <summary>
		/// it does not include itself
		/// it does not include link ports and line nodes
		/// </summary>
		/// <returns></returns>
		public virtual List<Control> GetAllControls()
		{
			List<Control> cs = new List<Control>();
			return cs;
		}
		protected virtual void OnEntityChanged()
		{
		}
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.FillEllipse(Brushes.LightGray, new Rectangle(DotSize, DotSize, this.ClientSize.Width - DotSizeHalf, this.ClientSize.Height - DotSizeHalf));
			e.Graphics.FillEllipse(Brushes.Blue, new Rectangle(DotSizeHalf, DotSizeHalf, this.ClientSize.Width - DotSize, this.ClientSize.Height - DotSize));

		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			IMathDesigner dr = this.Parent as IMathDesigner;
			if (dr != null)
			{
				dr.ShowProperties(_entity);
			}

			base.OnMouseDown(e);
		}
		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);
			if (this.VplDesigner != null)
			{
				this.VplDesigner.Changed = true;
			}
			if (_xmlNode != null)
			{
				this.SaveLocation();
				base.OnWriteToXmlNode(_writer, _xmlNode);
			}
		}
		public override string ToString()
		{
			if (_entity == null)
				return "Unknown";
			return _entity.Name;
		}
		public ProgramOutPort CreateOutport()
		{
			if (_entity != null && _entity.ProgEntity != null && _entity.CanAddOutputs)
			{
				IDataSelectionControl dlg = MathNode.GetPropertySelector(
					XmlSerialization.GetProjectGuid(_xmlNode.OwnerDocument),
					XmlSerialization.GetRootComponentId(_xmlNode),
					_entity.ProgEntity.OwnerObject,
					EnumPropAccessType.CanRead);
				if (((Form)dlg).ShowDialog(this.FindForm()) == DialogResult.OK)
				{
					ObjectRef v = dlg.UITypeEditorSelectedValue as ObjectRef;
					if (v != null)
					{
						ProgramOutPort pp = new ProgramOutPort(_entity.ProgEntity);
						pp.PortProperty = v;
						//add to collection
						_entity.ProgEntity.AddOutport(pp);
						//setup
						this.Parent.Controls.Add(pp);
						pp.Owner = this;
						pp.CheckCreateNextNode();
						this.Parent.Controls.Add((Control)pp.NextNode);
						pp.CreateForwardLine();
						pp.Label.Text = v.localName;
						((MathNodeVariable)(((DrawingVariable)(pp.Label)).Variable.MathExpression[0])).VariableName = v.localName;
						return pp;
					}
				}
				//}
			}
			return null;
		}
		public ProgramInPort CreateInport()
		{
			if (_entity != null && _entity.ProgEntity != null && _entity.CanAddInputs)
			{
				IDataSelectionControl dlg = MathNode.GetPropertySelector(
					XmlSerialization.GetProjectGuid(_xmlNode.OwnerDocument),
					XmlSerialization.GetRootComponentId(_xmlNode),
					_entity.ProgEntity.OwnerObject,
					EnumPropAccessType.CanWrite);
				if (((Form)dlg).ShowDialog(this.FindForm()) == DialogResult.OK)
				{
					ObjectRef v = dlg.UITypeEditorSelectedValue as ObjectRef;
					if (v != null)
					{
						ProgramInPort pp = new ProgramInPort(_entity.ProgEntity);
						pp.PortProperty = v;
						//add to collection
						_entity.ProgEntity.AddInport(pp);
						//setup
						this.Parent.Controls.Add(pp);
						pp.Owner = this;
						pp.CheckCreatePreviousNode();
						this.Parent.Controls.Add((Control)pp.PrevNode);
						pp.CreateBackwardLine();
						pp.Label.Text = v.localName;
						((MathNodeVariable)(((DrawingVariable)(pp.Label)).Variable.MathExpression[0])).VariableName = v.localName;
						return pp;
					}
				}
			}
			return null;
		}
		protected virtual void mnu_CreateInput(object sender, EventArgs e)
		{
		}
		protected virtual void mnu_CreateOutput(object sender, EventArgs e)
		{
			CreateOutport();
		}
		#endregion
		#region IMathExpViewer Members
		public virtual MathExpItem ExportMathItem(MathExpGroup group)
		{
			MathExpItem item = new MathExpItem(group);
			if (Site != null)
				item.Name = Site.Name;
			else
				item.Name = this.Name;
			item.Location = this.Location;
			item.Size = this.Size;
			return item;
		}
		public virtual IVariable FindVariableById(UInt32 id)
		{
			return null;
		}
		#endregion
		#region ICloneable
		public override object Clone()
		{
			ObjectDrawing clone = (ObjectDrawing)base.Clone();
			clone.DotSize = _dotSize;
			if (_entity != null)
			{
				clone.ObjectEntity = (IProgramEntity)_entity.Clone();
			}
			return clone;
		}
		#endregion
		#region IXmlNodeSerializable Members
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_xmlNode = node;
			base.OnReadFromXmlNode(serializer, node);
			_entity = (IProgramEntity)XmlSerialization.ReadFromChildXmlNode(serializer, node, XML_ENTITY);
		}
		IXmlCodeWriter _writer;
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			_writer = serializer;
			base.OnWriteToXmlNode(serializer, node);
			if (_entity != null)
			{
				XmlSerialization.WriteToUniqueChildXmlNode(serializer, node, XML_ENTITY, _entity);
			}
		}
		#endregion
	}
	public class ObjectDrawingList : List<ObjectDrawing>
	{
		public ObjectDrawingList()
		{
		}
		public ObjectDrawing GetObjectByID(int id)
		{
			foreach (ObjectDrawing obj in this)
			{
				if (obj.ID == id)
					return obj;
			}
			return null;
		}
	}
}
