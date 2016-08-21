/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.ComponentModel.Design;
using System.CodeDom;
using MathExp.RaisTypes;

namespace MathExp
{
	/// <summary>
	/// viewing a math expression or a compound math expression as an image and input/output ports.
	/// 
	/// </summary>
	public partial class MathExpViewer : UserControl, IMessageReceiver, IImageItem, ICustomTypeDescriptor, IComponentWithID, IMathExpViewer
	{
		#region Fields and constructor
		/// <summary>
		/// the math expression or compound math expression
		/// </summary>
		private IMathExpression mathExp;
		/// <summary>
		/// image representation
		/// </summary>
		private Bitmap img;
		/// <summary>
		/// visible properties
		/// </summary>
		private StringCollection propertyNames;
		private UInt32 _id;
		private bool bLoading; //surpress set-changed signal when loading
		private Size _size0;

		public MathExpViewer()
		{
			InitializeComponent();
			propertyNames = new StringCollection();
			propertyNames.Add("Name");
			propertyNames.Add("Location");
			propertyNames.Add("Size");
			propertyNames.Add("MathExp");
			propertyNames.Add("ShortDescription");
			//
			_size0 = this.Size;
		}
		#endregion
		#region private methods
		private void showEditor(IMathExpression exp)
		{
			try
			{
				Rectangle rc = this.Parent.RectangleToScreen(this.Bounds);
				IMathEditor dlg = exp.CreateEditor(rc);
				if (((Form)dlg).ShowDialog(this.FindForm()) == DialogResult.OK)
				{
					LoadData(dlg.MathExpression);
					//
					Refresh();
					if (this.Parent != null)
					{
						ActiveDrawing.RefreshControl(this.Parent);
					}
					setChanged();
				}
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(), err);
			}
		}
		private void setChanged()
		{
			if (!bLoading)
			{
				IMathDesigner dv = this.Parent as IMathDesigner;
				if (dv != null)
				{
					dv.Changed = true;
				}
			}
		}
		private void createImage()
		{
			Graphics g = this.CreateGraphics();
			img = mathExp.CreateIcon(g);
			g.Dispose();
		}
		#endregion
		#region IMathExpViewer members
		public void ResetActiveDrawingID()
		{
			_id = (UInt32)Guid.NewGuid().GetHashCode();
		}
		public UInt32 ActiveDrawingID
		{
			get
			{
				if (_id == 0)
				{
					ResetActiveDrawingID();
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		public MathExpItem ExportMathItem(MathExpGroup group)
		{
			MathExpItem item = new MathExpItem(group);
			if (Site != null)
				item.Name = Site.Name;
			else
				item.Name = this.Name;
			item.Location = this.Location;
			item.Size = this.Size;
			item.MathExpression = (IMathExpression)this.mathExp.Clone();
			return item;
		}
		#endregion
		#region public methods
		public void Edit()
		{
			showEditor(mathExp);
		}
		public void RefreshImage()
		{
			createImage();
			this.Refresh();
		}
		public LinkLineNodeOutPort[] OutputPorts
		{
			get
			{
				if (mathExp != null)
				{
					if (mathExp.OutputVariable != null)
					{
						return mathExp.OutputVariable.OutPorts;
					}
				}
				return null;
			}
		}
		/// <summary>
		/// called when loading the group
		/// </summary>
		/// <param name="item"></param>
		public void ImportMathItem(MathExpItem item)
		{
			bLoading = true;
			this.Name = item.Name;
			this.Location = item.Location;
			this.Size = item.Size;
			this.mathExp = item.MathExpression;
			if (Site != null)
			{
				Site.Name = item.Name;
			}
			mathExp.GenerateInputVariables();

			mathExp.PrepareDrawInDiagram();
			if (OutputPorts != null)
			{
				for (int i = 0; i < OutputPorts.Length; i++)
				{
					OutputPorts[i].Owner = this;
					OutputPorts[i].Label.Visible = false;
					OutputPorts[i].RestoreLocation();
				}
			}
			createImage();
			bLoading = false;
		}
		public void SetInportOwners()
		{
			List<LinkLineNodePort> ports = this.GetPorts();
			for (int i = 0; i < ports.Count; i++)
			{
				ports[i].Owner = this;
				((DrawingVariable)ports[i].Label).OnMouseSelect += new EventHandler(var_OnMouseSelect);
				ports[i].RestoreLocation();
			}
		}
		public IVariable FindVariableById(UInt32 id)
		{
			if (mathExp != null)
			{
				return mathExp.GetVariableByID(id);
			}
			return null;
		}
		/// <summary>
		/// new load or re-load.
		/// this control must already be added to the viewer (DiagramViewer).
		/// It generates ports for all variables
		/// </summary>
		/// <param name="data">the result of lower level editing</param>
		public void LoadData(IMathExpression data)
		{
			bLoading = true;
			DiagramViewer dv = this.Parent as DiagramViewer;
			VariableList currentVariables = null;
			IVariable currOutVar = null;
			if (mathExp != null)
			{
				currOutVar = mathExp.OutputVariable;
				currentVariables = mathExp.InputVariables;
			}
			mathExp = data;
			if (mathExp == null)
			{
				mathExp = new MathNodeRoot();
			}
			mathExp.ClearFocus();
			//1. find out all unique and non-local variables
			VariableList _portVariables = mathExp.InputVariables;
			//2. remove removed ports
			if (currentVariables != null)
			{
				foreach (IVariable v in currentVariables)
				{
					if (_portVariables.GetVariableById(v.ID) == null)
					{
						//remove port and nodes
						List<Control> cs = new List<Control>();
						foreach (Control c in dv.Controls)
						{
							LinkLineNodeInPort p = c as LinkLineNodeInPort;
							if (p != null)
							{
								if (p.PortID == v.ID)
								{
									cs.Add(p);
									cs.Add(p.Label);
									ILinkLineNode prev = p.PrevNode;
									while (prev != null)
									{
										if (prev is LinkLineNodePort)
											break;
										cs.Add((Control)prev);
										prev = prev.PrevNode;
									}
									break;
								}
							}
						}
						foreach (Control c in cs)
						{
							dv.Controls.Remove(c);
						}
					}
				}
			}
			List<Control> newControls = new List<Control>();
			List<LinkLineNodeInPort> newInPorts = new List<LinkLineNodeInPort>();
			//3.create new ports
			foreach (IVariable v in _portVariables)
			{
				if (currentVariables == null || currentVariables.GetVariableById(v.ID) == null)
				{
					if (data.IsContainer)
					{
						//create a new variable instance
						MathNodeRoot r = new MathNodeRoot();
						((MathNode)v).root.CopyAttributesToTarget(r);
						r[1] = new MathNodeVariable(r);
						IVariable vi = (IVariable)r[1];
						vi.VariableType = (RaisDataType)v.VariableType.Clone();
						vi.VariableName = v.VariableName;
						vi.SubscriptName = v.SubscriptName;
						vi.ResetID(v.ID);
						vi.InPort = new LinkLineNodeInPort(vi);
						vi.InPort.SetPortOwner(vi);
						vi.InPort.Owner = this;
						newControls.Add(vi.InPort);
						newControls.Add(vi.InPort.Label);
						vi.InPort.CheckCreatePreviousNode();
						newControls.Add((Control)vi.InPort.PrevNode);
						newInPorts.Add(vi.InPort);
					}
					else
					{
						v.InPort = new LinkLineNodeInPort(v);
						v.InPort.Owner = this;
						newControls.Add(v.InPort);
						newControls.Add(v.InPort.Label);
						v.InPort.CheckCreatePreviousNode();
						newControls.Add((Control)v.InPort.PrevNode);
						newInPorts.Add(v.InPort);
					}
				}
			}
			if (newInPorts.Count > 0)
			{
				int dn = this.Width / (newInPorts.Count + 1);
				for (int i = 0; i < newInPorts.Count; i++)
				{
					newInPorts[i].Position = i * dn + dn;
					newInPorts[i].Left = this.Left + newInPorts[i].Position;
					newInPorts[i].SaveLocation();
					newInPorts[i].PrevNode.Left = newInPorts[i].Left;
				}
			}
			//3. re-map to existing ports
			if (currOutVar != null && currOutVar.OutPorts != null)
			{
				//if port exists then re-use it
				mathExp.OutputVariable.OutPorts = currOutVar.OutPorts;
				for (int i = 0; i < mathExp.OutputVariable.OutPorts.Length; i++)
				{
					mathExp.OutputVariable.OutPorts[i].SetPortOwner(mathExp.OutputVariable);
					//if linking exists then re-establish it
					if (currOutVar.OutPorts[i].LinkedPortID != 0)
					{
						//find the variable
						if (dv != null)
						{
							IVariable v = dv.FindVariableById(currOutVar.OutPorts[i].LinkedPortID);
							if (v != null)
							{
								v.InPort.LinkedPortID = mathExp.OutputVariable.ID;
							}
						}
					}
				}
			}
			else
			{
				//for a new load, create the output port
				mathExp.OutputVariable.OutPorts = new LinkLineNodeOutPort[] { new LinkLineNodeOutPort(mathExp.OutputVariable) };
				mathExp.OutputVariable.OutPorts[0].CheckCreateNextNode();
				////use the default position
				mathExp.OutputVariable.OutPorts[0].Position = this.Width / 2;

			}
			for (int i = 0; i < mathExp.OutputVariable.OutPorts.Length; i++)
			{
				mathExp.OutputVariable.OutPorts[i].Owner = this;
				mathExp.OutputVariable.OutPorts[i].Label.Visible = false;
				mathExp.OutputVariable.OutPorts[i].SaveLocation();
			}
			if (!(currOutVar != null && currOutVar.OutPorts != null))
			{
				for (int i = 0; i < mathExp.OutputVariable.OutPorts.Length; i++)
				{
					//for a new load, set default postion for the empty linking node
					((Control)(mathExp.OutputVariable.OutPorts[i].NextNode)).Location = mathExp.OutputVariable.OutPorts[i].DefaultNextNodePosition();
					//add new controls
					newControls.Add(mathExp.OutputVariable.OutPorts[i]);
					newControls.Add((Control)mathExp.OutputVariable.OutPorts[i].NextNode);
					newControls.Add(mathExp.OutputVariable.OutPorts[i].Label);
				}
			}
			//add the new controls
			if (this.Parent != null && newControls.Count > 0)
			{
				Control[] a = new Control[newControls.Count];
				newControls.CopyTo(a);
				this.Parent.Controls.AddRange(a);
			}
			mathExp.PrepareDrawInDiagram();
			//
			createImage();
			bLoading = false;
		}
		public void LoadMathExpression(IMathExpression data)
		{
			mathExp = data;
			this.Name = data.Name;
			this.Site.Name = data.Name;
			mathExp.PrepareDrawInDiagram();
			//
			createImage();
		}
		void var_OnMouseSelect(object sender, EventArgs e)
		{
			DrawingVariable dv = sender as DrawingVariable;
			if (dv != null)
			{
				IMathDesigner dr = this.Parent as IMathDesigner;
				if (dr != null)
				{
					dr.ShowProperties(dv.Variable);
				}
			}
		}
		#endregion
		#region Properties
		public IMathExpression MathExp
		{
			get
			{
				return mathExp;
			}
		}
		#endregion
		#region Hidden properties
		public List<LinkLineNodePort> GetPorts()
		{
			List<LinkLineNodePort> ports = new List<LinkLineNodePort>();
			DiagramViewer dv = this.Parent as DiagramViewer;
			List<LinkLineNodePort> allports = dv.GetAllPorts();
			foreach (LinkLineNodePort p in allports)
			{
				if (mathExp.GetVariableByID(p.PortID) != null)
				{
					ports.Add(p);
				}
			}
			return ports;
		}

		bool _selected = true;
		[Browsable(false)]
		public bool Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				_selected = value;
			}
		}
		#endregion
		#region Design time events
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (img != null)
			{
				e.Graphics.DrawImage(img, 1, 1, this.Size.Width - 4, this.Size.Height - 4);
			}
			e.Graphics.DrawRectangle(Pens.Blue, 0, 0, this.Size.Width - 3, this.Size.Height - 3);
			e.Graphics.DrawLine(Pens.LightGray, 3, this.Height - 1, this.Width, this.Height - 1);
			e.Graphics.DrawLine(Pens.LightGray, 3, this.Height - 2, this.Width, this.Height - 2);
			e.Graphics.DrawLine(Pens.LightGray, this.Width - 1, 3, this.Width - 1, this.Height);
			e.Graphics.DrawLine(Pens.LightGray, this.Width - 2, 3, this.Width - 2, this.Height);
		}
		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);
			setChanged();
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			IMathDesigner md = this.Parent as IMathDesigner;
			if (md != null)
			{
				md.ReSelectComponent(this);
				if (!md.DisableUndo)
				{
					UndoEntity entity = new UndoEntity(new SizeUndo(md, this.ActiveDrawingID, _size0), new SizeUndo(md, this.ActiveDrawingID, this.Size));
					md.AddUndoEntity(entity);
				}
			}
			setChanged();
		}
		#endregion
		#region IMessageReceiver Members
		public bool FireMouseDown(MouseButtons button, int x, int y, Keys modifiers)
		{
			_size0 = this.Size;
			return false;
		}

		public bool FireMouseMove(MouseButtons button, int x, int y, Keys modifiers)
		{
			return false;
		}

		public bool FireMouseUp(MouseButtons button, int x, int y, Keys modifiers)
		{
			if (button == MouseButtons.Right)
			{
				DiagramViewer dv = this.Parent as DiagramViewer;
				if (dv != null)
				{
					dv.OnMathViewRightClick(this, dv.PointToClient(this.PointToScreen(new Point(x, y))));
				}
			}
			return false;
		}

		public bool FireMouseDblClick(MouseButtons button, int x, int y, Keys modifiers)
		{
			showEditor(mathExp);
			return false;
		}
		public bool FireKeyDown(KeyEventArgs e)
		{
			OnKeyDown(e);
			return false;
		}
		public bool FireKeyUp(KeyEventArgs e)
		{
			OnKeyUp(e);
			return false;
		}
		#endregion
		#region IImageItem Members

		public ImageID ImageItem
		{
			get
			{
				ImageID imgid = new ImageID();
				imgid.ID = this.OutputPorts[0].PortID;
				imgid.SelectedImage = img;
				return imgid;
			}
		}

		#endregion
		#region ICustomTypeDescriptor Members

		public virtual AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public virtual string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public virtual string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public virtual TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public virtual EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public virtual PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public virtual object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public virtual EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, true);
			PropertyDescriptorCollection newProps = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor oProp in baseProps)
			{
				if (propertyNames.Contains(oProp.Name))
				{
					newProps.Add(oProp);
				}
			}
			return newProps;
		}

		public virtual PropertyDescriptorCollection GetProperties()
		{
			PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, true);
			PropertyDescriptorCollection newProps = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor oProp in baseProps)
			{
				if (propertyNames.Contains(oProp.Name))
				{
					newProps.Add(oProp);
				}
			}
			return newProps;
		}

		public virtual object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;

		}

		#endregion
		#region IComponentWithID Members

		public UInt32 ComponentID
		{
			get
			{
				return mathExp.OutputVariable.ID;
			}
		}
		public bool ContainsID(UInt32 id)
		{
			if (mathExp != null)
			{
				VariableList vs = mathExp.FindAllInputVariables();
				for (int i = 0; i < vs.Count; i++)
				{
					if (vs[i] != null)
					{
						if (vs[i].ID == id)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		#endregion
	}
}
