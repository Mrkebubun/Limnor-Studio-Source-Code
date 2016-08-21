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
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Specialized;
using System.Xml;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Collections;
using MathExp.RaisTypes;
using System.CodeDom;
using LimnorUI;

namespace MathExp
{
	/// <summary>
	/// root designer to holder a group of linked math expressions.
	/// the group contains math expressions and the link nodes linking them togather.
	/// it is like the Form class for Windows UI.
	/// LoadGroup(MathExpGroup mathGroup) loads a MathExpGroup for editing.
	/// MathExpGroup Export() output the editing result.
	/// During the editing the design process does not maintain a MathExpGroup object.
	/// </summary>
	[Designer(typeof(IRootDesigner))]
	[Designer(typeof(ComponentDesigner))]
	public class DiagramViewer : Diagram, IMessageReceiver, ICustomTypeDescriptor, IMathDesigner, IImageList, INameCreator
	{
		#region Fields and Constructor
		StringCollection propertyNames;
		DiagramDesignerHolder holder;
		private bool _changed;
		List<EventHandler> onShowHandlers;
		private Hashtable _services;
		bool _noCompound;
		public DiagramViewer()
		{
			InitializeComponent();
			gridImage = this.BackgroundImage;
			this.BackColor = Color.White;
			propertyNames = new StringCollection();
			propertyNames.Add("Name");
			propertyNames.Add("ResultValueType");
			propertyNames.Add("ShowGrid");
			propertyNames.Add("ShowArrowAtLineBreaks");
			propertyNames.Add("IconImage");
			propertyNames.Add("Description");
			propertyNames.Add("IconSize");
			propertyNames.Add("IconType");
			propertyNames.Add("ShortDescription");
			//
			AddOnShowHandler(new EventHandler(refreshActiveDrawings));
		}
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiagramViewer));
			this.SuspendLayout();
			// 
			// DiagramViewer
			// 
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.Name = "DiagramViewer";
			this.Size = new System.Drawing.Size(900, 541);
			this.ResumeLayout(false);

		}
		#endregion
		#region INameCreator Members
		public string CreateNewName(string baseName)
		{
			if (holder == null)
				return baseName + "1";
			return holder.CreateName(baseName);
		}
		#endregion
		#region private Methods
		private void refreshActiveDrawings(object sender, EventArgs e)
		{
			for (int i = 0; i < Controls.Count; i++)
			{
				Controls[i].Refresh();
			}
		}
		#endregion
		#region Methods
		protected override IUndoHost UndoHost
		{
			get
			{
				return holder;
			}
		}
		public List<ParameterDrawing> GetParameters()
		{
			List<ParameterDrawing> ps = new List<ParameterDrawing>();
			foreach (Control c in Controls)
			{
				ParameterDrawing p = c as ParameterDrawing;
				if (p != null)
				{
					ps.Add(p);
				}
			}
			return ps;
		}
		public void RemoveParameter(ParameterDrawing p)
		{
			if (p.Parameter.OutPorts[0].LinkedPortID != 0)
			{
				foreach (Control c in Controls)
				{
					LinkLineNodePort x = c as LinkLineNodePort;
					if (x != null)
					{
						if (x.PortID == p.Parameter.OutPorts[0].LinkedPortID)
						{
							x.LinkedPortID = 0;
						}
					}
				}
			}
			LinkLineNode l = (LinkLineNode)p.Parameter.OutPorts[0].NextNode;
			while (l != null && !(l is LinkLineNodePort))
			{
				if (l.NextNode != null && l.NextNode is LinkLineNodePort)
				{
					if (l.Line != null)
					{
						if (l.Line.EndPoint != l.NextNode)
						{
							l.ClearLine();
						}
					}
					l.SetPrevious(null);
					l.Visible = true;
					break;
				}
				l.ClearLine();
				l.SetPrevious(null);
				Controls.Remove(l);
				l = (LinkLineNode)l.NextNode;
			}
			p.Parameter.OutPorts[0].ClearLine();
			Controls.Remove(p.Parameter.OutPorts[0].Label);
			Controls.Remove(p.Parameter.OutPorts[0]);
			Controls.Remove(p);
		}
		public List<LinkLineNodePort> GetAllPorts()
		{
			List<LinkLineNodePort> ports = new List<LinkLineNodePort>();
			foreach (Control c in Controls)
			{
				LinkLineNodePort p = c as LinkLineNodePort;
				if (p != null)
				{
					ports.Add(p);
				}
			}
			return ports;
		}
		public void AddNewMathExp(int x, int y)
		{
			MathNodeRoot newRoot = new MathNodeRoot();
			PlusNode plus = new PlusNode(newRoot);
			newRoot[1] = plus;
			MathNodeVariable nx = new MathNodeVariable(plus);
			plus[0] = nx;
			nx.VariableName = "x";
			nx = new MathNodeVariable(plus);
			plus[1] = nx;
			nx.VariableName = "y";
			IDesignServiceProvider sp = MathNode.GetGlobalServiceProvider(this);
			if (sp != null)
			{
				MathNode.RegisterGetGlobalServiceProvider(newRoot, sp);
			}
			holder.AddMathViewer(newRoot, x, y);
		}
		public void AssignHolder(DiagramDesignerHolder h)
		{
			holder = h;
		}
		/// <summary>
		/// generate result
		/// </summary>
		/// <returns></returns>
		public MathExpGroup Export()
		{
			MathExpGroup mg = new MathExpGroup();
			mg.Name = this.Site.Name;
			mg.Description = this.Description;
			mg.ShortDescription = this.ShortDescription;
			mg.IconImage = this.IconImage;
			Form f = this.FindForm();
			if (f != null)
			{
				mg.EitorBounds = f.Bounds;
			}
			//record positions
			for (int i = 0; i < this.Controls.Count; i++)
			{
				ActiveDrawing av = this.Controls[i] as ActiveDrawing;
				if (av != null)
				{
					av.SaveLocation();
				}
			}
			//
			VariableList inportVariables = new VariableList();
			for (int i = 0; i < this.Controls.Count; i++)
			{
				if (this.Controls[i] is IMathExpViewer)
				{
					mg.Expressions.Add(((IMathExpViewer)this.Controls[i]).ExportMathItem(mg));
				}
				else if (this.Controls[i] is ReturnIcon)
				{
					mg.ReturnIcon = this.Controls[i] as ReturnIcon;
				}
			}
			//generate inports
			for (int i = 0; i < this.Controls.Count; i++)
			{
				if (this.Controls[i] is LinkLineNodeInPort)
				{
					LinkLineNodeInPort ip = (LinkLineNodeInPort)this.Controls[i];
					IVariable v = ip.PortOwner as IVariable;
					if (v != null && !v.IsReturn)
					{
						if (v.ID != mg.ReturnIcon.ReturnVariable.ID)
						{
							MathExpItem mi = mg.GetItemByID(v.ID);
							if ((!mi.MathExpression.IsContainer))
							{
								//use the variable directly
								inportVariables.Add(v);
							}
							else
							{
								//generate a duplication
								MathNodeRoot r = new MathNodeRoot();
								((MathNode)v).root.CopyAttributesToTarget(r);
								r[1] = (MathNode)v.CloneExp(r);
								r.AdjustVariableMap();
								inportVariables.Add((IVariable)v);
							}
						}
					}
				}
			}
			mg.SetInportVariables(inportVariables);
			//find all input variables
			mg.GenerateInputVariables();
			return mg;
		}
		public IVariable FindVariableById(UInt32 id)
		{
			foreach (Control c in Controls)
			{
				LinkLineNodePort p = c as LinkLineNodePort;
				if (p != null)
				{
					if (p.PortID == id)
					{
						return p.PortOwner as IVariable;
					}
				}
				else
				{
					ReturnIcon r = c as ReturnIcon;
					if (r != null)
					{
						if (r.ReturnVariable.ID == id)
						{
							return r.ReturnVariable;
						}
					}
				}
			}
			return null;
		}
		public MathExpViewer FindMathExpViewerById(UInt32 id, ref LinkLineNodePort linkedPort)
		{
			for (int i = 0; i < this.Controls.Count; i++)
			{
				MathExpViewer mv = this.Controls[i] as MathExpViewer;
				if (mv != null)
				{
					List<LinkLineNodePort> ports = mv.GetPorts();
					for (int j = 0; j < ports.Count; j++)
					{
						if (ports[j].PortID == id)
						{
							linkedPort = ports[j];
							return mv;
						}
					}
				}
			}
			return null;
		}
		private void loadgroup(MathExpGroup mathGroup, bool includeReturnIcon)
		{
			List<MathExpViewer> mvs = new List<MathExpViewer>();
			List<MathExpItem> items = mathGroup.Expressions;
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].MathExpression is INonHostedItem)
				{
					//Method parameters (Parameter class)
					INonHostedItem nv = items[i].MathExpression as INonHostedItem;
					Controls.AddRange(nv.GetControls());
					nv.InitLocation();
					nv.InitLinkLine();
				}
				else
				{
					//create MathExpViewer control as a design component
					//let MathExpViewer to import the item
					mvs.Add(holder.LoadMathItem(items[i]));
				}
			}
			Control[] ctrls = mathGroup.GetAllLinkNodes(includeReturnIcon);
			Controls.AddRange(ctrls);
			mathGroup.CreateLinkLines();
			//
			for (int i = 0; i < ctrls.Length; i++)
			{
				ActiveDrawing ad = ctrls[i] as ActiveDrawing;
				if (ad != null)
				{
					ad.OnDesirialize();
				}
			}
			foreach (MathExpViewer mv in mvs)
			{
				mv.SetInportOwners();
			}
			mathGroup.PrepareDrawInDiagram();
			//
			for (int i = 0; i < ctrls.Length; i++)
			{
				LinkLineNodePort p = ctrls[i] as LinkLineNodePort;
				if (p != null)
				{
					p.adjustPosition();
				}
			}
		}
		private void clearAll()
		{
			foreach (Control c in Controls)
			{
				LinkLineNodePort p = c as LinkLineNodePort;
				if (p != null)
				{
					p.ClearLine();
				}
			}
			Controls.Clear();
		}
		public void LoadGroup(MathExpGroup mathGroup)
		{
			Name = mathGroup.Name;
			this.Site.Name = mathGroup.Name;
			Description = mathGroup.Description;
			ShortDescription = mathGroup.ShortDescription;
			IconImage = mathGroup.IconImage;
			//
			_returnType = mathGroup.DataType;
			clearAll();
			//
			loadgroup(mathGroup, true);
			//
			if (string.IsNullOrEmpty(mathGroup.Name))
			{
				mathGroup.Name = CreateNewName("Group");
				this.Name = mathGroup.Name;
				this.Site.Name = mathGroup.Name;
			}
		}
		protected LinkLineNodeOutPort GetOutPortByID(UInt32 portId, UInt32 portInstanceId)
		{
			foreach (Control c in Controls)
			{
				LinkLineNodeOutPort p = c as LinkLineNodeOutPort;
				if (p != null)
				{
					if (p.PortID == portId && p.PortInstanceID == portInstanceId)
					{
						return p;
					}
				}
			}
			return null;
		}
		protected LinkLineNodeInPort GetInPortByID(UInt32 portId)
		{
			LinkLineNodeInPort p = GetPortByID(portId) as LinkLineNodeInPort;
			return p;
		}
		protected LinkLineNodePort GetPortByID(UInt32 portId)
		{
			foreach (Control c in Controls)
			{
				LinkLineNodePort p = c as LinkLineNodePort;
				if (p != null)
				{
					if (p.PortID == portId)
					{
						return p;
					}
				}
			}
			return null;
		}
		/// <summary>
		/// group selected items into a new MathExpGroup
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="size"></param>
		public void CreateCompound(Point pos, Size size)
		{
			holder.CreateUndoTransaction("CreateCompound");
			try
			{
				foreach (Control c in Controls)
				{
					ActiveDrawing a = c as ActiveDrawing;
					if (a != null)
					{
						a.SaveLocation();
					}
				}
				int nOutputCount = 0;
				//find boundary
				LinkLineNodeInPort linkedInportForOut = null; //the inport linked to the ourport
				//find the return icon
				ReturnIcon returnIcon = null;
				for (int i = 0; i < this.Controls.Count; i++)
				{
					if (this.Controls[i] is ReturnIcon)
					{
						returnIcon = this.Controls[i] as ReturnIcon;
						break;
					}
				}
				if (returnIcon == null)
				{
					throw new MathException("ReturnIcon not found");
				}
				//find selected items
				List<MathExpViewer> items = new List<MathExpViewer>();
				ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
				if (selectionService != null)
				{
					ICollection ic = selectionService.GetSelectedComponents();
					foreach (object v in ic)
					{
						MathExpViewer mv = v as MathExpViewer;
						if (mv != null)
						{
							if (!items.Contains(mv))
							{
								items.Add(mv);
							}
						}
					}
				}
				if (items.Count > 0)
				{
					//find out the output id; also make sure there is not more than one output
					UInt32 outputId = 0;
					foreach (MathExpViewer mv in items)
					{
						//the item linked to the return icon is certainly the outport
						bool linkedToReturn = false;
						for (int i = 0; i < mv.OutputPorts.Length; i++)
						{
							if (mv.OutputPorts[i].LinkedPortID == returnIcon.InPort.PortID)
							{
								if (outputId != mv.OutputPorts[i].PortID)//prevent counting it more than once because going through link-lines might hit an item more than once
								{
									nOutputCount++;
									outputId = mv.OutputPorts[i].PortID;
									linkedInportForOut = (LinkLineNodeInPort)returnIcon.InPort;
								}
								if (mv.OutputPorts.Length > 1)
								{
									nOutputCount++;
								}
								linkedToReturn = true;
								break;
							}
						}
						if (!linkedToReturn)
						{
							//locate the boundary item
							MathExpViewer linked = mv; //start go through the link line
							for (int i = 0; i < linked.OutputPorts.Length; i++)
							{
								while (linked.OutputPorts[i].LinkedPortID != 0)
								{
									LinkLineNodePort port = null;
									//get the linked item
									MathExpViewer linked2 = FindMathExpViewerById(linked.OutputPorts[i].LinkedPortID, ref port);
									if (linked2 == null)
									{
										throw new MathException("Item not found for port {0} linked from {1} in {2}", linked.OutputPorts[i].LinkedInPort.PortID, linked.OutputPorts[i].PortID, linked.Name);
									}
									if (port != linked.OutputPorts[i].LinkedInPort)
									{
										throw new MathException("FindMathExpViewerById returns mismatching port");
									}
									//if the linked item is not a selected item then the item is at the edge
									if (!items.Contains(linked2))
									{
										//linked2 is outside of the group
										if (outputId != linked.OutputPorts[i].PortID)//prevent counting it more than once because going through link-lines might hit an item more than once
										{
											nOutputCount++;
											outputId = linked.OutputPorts[i].PortID;
											linkedInportForOut = linked.OutputPorts[i].LinkedInPort;
										}
										break;
									}
									//linked2 is still inside the group
									//if it is linked to the return icon then the end of linking is reached
									if (linked2.OutputPorts[i].LinkedPortID == returnIcon.InPort.PortID)
									{
										if (outputId != linked2.OutputPorts[i].PortID)
										{
											nOutputCount++;
											outputId = linked2.OutputPorts[i].PortID;
											linkedInportForOut = (LinkLineNodeInPort)returnIcon.InPort;
										}
										break;
									}
									linked = linked2;//check the next link
								}
							}
						}
					}
					if (nOutputCount > 1)
					{
						UIUtil.ShowWarning(this.FindForm(), "Cannot create a formula group because the selected items link to two or more unselected items. Select the items which link to less than 2 unselected items.");
					}
					else
					{
						//create a new math group as the compound
						MathExpGroup g = new MathExpGroup();
						g.Name = CreateNewName("Item");
						//create out port
						g.ReturnIcon.Location = new Point(100, 350);
						g.ReturnIcon.SaveLocation();
						g.ReturnIcon.InPort.Position = g.ReturnIcon.Size.Width / 2 - LinkLineNode.dotSize / 2;
						g.ReturnIcon.InPort.Location = new Point(g.ReturnIcon.Location.X + g.ReturnIcon.InPort.Position, g.ReturnIcon.Location.Y - LinkLineNode.dotSize);
						g.ReturnIcon.InPort.SaveLocation();
						g.ReturnIcon.ReturnVariable.OutPorts = new LinkLineNodeOutPort[] { new LinkLineNodeOutPort(g.ReturnIcon.ReturnVariable) };
						g.ReturnIcon.ReturnVariable.OutPorts[0].Position = size.Width / 2 - LinkLineNode.dotSize / 2;
						g.ReturnIcon.ReturnVariable.OutPorts[0].Location = new Point(pos.X + g.ReturnIcon.ReturnVariable.OutPorts[0].Position, pos.Y + size.Height);
						g.ReturnIcon.ReturnVariable.OutPorts[0].SaveLocation();
						g.ReturnIcon.ReturnVariable.OutPorts[0].CheckCreateNextNode();
						((ActiveDrawing)g.ReturnIcon.ReturnVariable.OutPorts[0].NextNode).SaveLocation();
						//add all selected items
						foreach (MathExpViewer mv in items)
						{
							g.Expressions.Add(mv.ExportMathItem(g));
						}
						//make output link
						/*Every variable has an Inport and an array of OutPorts
						 * MathExpGroup uses its ReturnIcon's variable to pass the result to outside: 
						 * Inport links to the last item's outport, Outports links to the Inports of outside items.
						 */
						//
						if (outputId != 0)
						{
							MathExpItem outputItem = g.GetItemByID(outputId); //internal item of g as the end item for output
							//link outputItem to g:
							//because we only allow single output, we know there is just one outport
							LinkLineNodeOutPort outPort = outputItem.MathExpression.OutputVariable.OutPorts[0];
							outPort.LinkedPortID = g.ReturnIcon.InPort.PortID;
							g.ReturnIcon.InPort.LinkedPortID = outPort.PortID;
						}
						else
						{
							//create default link line node for the return inport
							g.ReturnIcon.InPort.CheckCreatePreviousNode();
							((ActiveDrawing)g.ReturnIcon.InPort.PrevNode).SaveLocation();
						}
						//link g to outside will be done after removing the selected items
						VariableList allInputVariables = g.AllInputVariables;
						VariableList inputVariables = new VariableList();
						VariableList inportVariables = new VariableList();
						//collect inport variables
						foreach (IVariable v in allInputVariables)
						{
							bool bIsInput = (!v.IsConst && !v.IsDummyPort);
							IVariable vx = FindVariableById(v.ID);
							if (vx == null)
							{
								throw new MathException("variable {0}({1}) not found", v.VariableName, v.ID);
							}
							if (vx.InPort.LinkedPortID != 0)
							{
								if (g.GetVariableByID(vx.InPort.LinkedPortID) != null)
								{
									bIsInput = false;
								}
							}
							if (bIsInput)
							{
								inputVariables.Add(v);
							}
							vx.InPort.SaveLocation();
							vx.InPort.Label.SaveLocation();
							inportVariables.Add(vx);//keep the current version
						}
						g.SetInputVariables(inputVariables);
						g.SetInportVariables(inportVariables);
						//create new inports for this level========================================
						VariableList newInportVars = new VariableList();
						int dx = size.Width / (inputVariables.Count + 1);
						for (int i = 0; i < inputVariables.Count; i++)
						{
							IVariable v = inputVariables[i];
							IVariable vx = FindVariableById(v.ID);
							if (vx == null)
							{
								throw new MathException("error creating inport: variable {0}({1}) not found", v.VariableName, v.ID);
							}
							MathNodeRoot r = new MathNodeRoot();
							r[1] = (MathNode)Activator.CreateInstance(v.GetType(), r);
							IVariable vi = (IVariable)r[1];
							vi.VariableType = (RaisDataType)v.VariableType.Clone();
							vi.VariableName = v.VariableName;
							vi.SubscriptName = v.SubscriptName;
							vi.ResetID(v.ID);
							vi.InPort = new LinkLineNodeInPort(vi);
							vi.InPort.LinkedPortID = vx.InPort.LinkedPortID;
							vi.InPort.LinkedPortInstanceID = vx.InPort.LinkedPortInstanceID;
							vi.InPort.Position = dx + dx * i - LinkLineNode.dotSize / 2;
							vi.InPort.Location = new Point(pos.X + vi.InPort.Position, pos.Y - LinkLineNode.dotSize);
							vi.InPort.SaveLocation();
							vi.InPort.CheckCreatePreviousNode();
							LinkLineNode n = vi.InPort.PrevNode as LinkLineNode;
							int top = vi.InPort.Top - 30;
							if (top < 0)
								top = 0;
							n.Location = new Point(vi.InPort.Left, top);
							n.SaveLocation();
							newInportVars.Add(vi);
						}
						//
						//remove the components===================
						foreach (MathExpViewer mv in items)
						{
							this.DeleteComponent(mv);
						}
						//========================================
						//link g to outside
						if (linkedInportForOut != null)
						{
							//we know there is just one output link
							g.ReturnIcon.ReturnVariable.OutPorts[0].LinkedPortID = linkedInportForOut.PortID;
							linkedInportForOut.LinkedPortID = g.ReturnIcon.ReturnVariable.OutPorts[0].PortID;
						}
						//
						//create the component
						MathExpViewer mvNew = holder.AddEmptyMathExpViewer();
						mvNew.Location = pos;
						mvNew.Size = size;
						mvNew.LoadMathExpression(g);
						//
						//load new controls
						List<Control> newControls = new List<Control>();
						//show inports
						foreach (IVariable v in newInportVars)
						{
							newControls.Add(v.InPort);
							newControls.Add(v.InPort.Label);
							newControls.Add((Control)v.InPort.PrevNode); //line already create by CheckCreatePreviousNode()
						}
						//show outport
						g.ReturnIcon.ReturnVariable.OutPorts[0].LabelVisible = false;
						newControls.Add(g.ReturnIcon.ReturnVariable.OutPorts[0]);
						newControls.Add((Control)g.ReturnIcon.ReturnVariable.OutPorts[0].NextNode);

						//re-connect inports
						foreach (IVariable v in newInportVars)
						{
							if (v.InPort.LinkedPortID != 0)
							{
								LinkLineNodeOutPort p = GetOutPortByID(v.InPort.LinkedPortID, v.InPort.LinkedPortInstanceID);
								if (p == null)
								{
									throw new MathException("Port [{0},{1}] is linked to [{2},{3}] but port [{2},{3}] is not found", v.InPort.PortID, v.InPort.PortInstanceID, v.InPort.LinkedPortID, v.InPort.LinkedPortInstanceID);
								}
								joinPorts(v.InPort, p, true);
							}
						}
						//re-connect outport
						if (g.ReturnIcon.ReturnVariable.OutPorts[0].LinkedPortID != 0)
						{
							LinkLineNodeInPort p = GetInPortByID(g.ReturnIcon.ReturnVariable.OutPorts[0].LinkedPortID);
							if (p == null)
							{
								throw new MathException("error connecting the outport {0} to inport {1}. inport not found", g.ReturnIcon.ReturnVariable.OutPorts[0].PortID, g.ReturnIcon.ReturnVariable.OutPorts[0].LinkedPortID);
							}
							joinPorts(p, g.ReturnIcon.ReturnVariable.OutPorts[0], false);
						}
						//add all controls
						Control[] a = new Control[newControls.Count];
						newControls.CopyTo(a);
						Controls.AddRange(a);
						//
						mvNew.SetInportOwners();
						//clear link id from inputs of the new lower level
						foreach (IVariable v in inputVariables)
						{
							v.InPort.LinkedPortID = 0;
							if (v.InPort.PrevNode is LinkLineNodeOutPort)
							{
								v.InPort.SetPrevious(null);
							}
							if (v.InPort.PrevNode == null)
							{
								v.InPort.CheckCreatePreviousNode();
							}
						}
						this.Refresh();
					}
				}
				holder.CommitUndoTransaction("CreateCompound");
			}
			catch (Exception err)
			{
				UIUtil.ShowError(this.FindForm(), err);
				holder.RollbackUndoTransaction("CreateCompound");
			}
		}
		private void joinPorts(LinkLineNodeInPort pi, LinkLineNodeOutPort po, bool fromInport)
		{
			pi.LinkedPortID = po.PortID;
			pi.LinkedPortInstanceID = po.PortInstanceID;
			po.LinkedPortID = pi.PortID;
			po.LinkedPortInstanceID = pi.PortInstanceID;
			//remove nodes from port
			if (fromInport)
			{
				LinkLineNode l = (LinkLineNode)po.NextNode;
				while (l != null)
				{
					Controls.Remove(l);
					l = (LinkLineNode)l.NextNode;
				}
				po.ClearLine();
				po.SetNext(null);
			}
			else
			{
				LinkLineNode l = (LinkLineNode)pi.PrevNode;
				while (l != null)
				{
					Controls.Remove(l);
					l = (LinkLineNode)l.PrevNode;
				}
				pi.ClearLine();
				pi.SetPrevious(null);
			}
			//make the link
			LinkLineNode start = po.End;
			LinkLineNode end = pi.Start;
			if (start is LinkLineNodeInPort)
			{
				throw new MathException("error connecting ports: Outport already connected with an Inport");
			}
			if (end is LinkLineNodeOutPort)
			{
				throw new MathException("error connecting ports: Inport already connected with an Outport");
			}
			start.SetNext(end);
			end.SetPrevious(start);
			if (start.Line == null)
			{
				start.CreateForwardLine();
			}
			else
			{
				if (end.Line != null)
				{
					throw new MathException("error connecting the ports. Both ends are used.");
				}
				end.CreateBackwardLine();
			}
			OnPortsLinked(pi, po);
		}
		/// <summary>
		/// break apart of a group
		/// 1. delete MV
		/// 2. load g
		/// 3. if g's OutPort is linked to an outside item
		///     *.1. if g's InPort is linked to an item inside g
		///         Link the item to the outside item
		///     *.2. if g's InPort is not linked to an item inside g
		///         Find an end item to use it as if it is linked 
		///4. If g's InPorts are linked to items in the parent level
		///     Make the links
		/// </summary>
		/// <param name="mv">the item to break, its MathExp must be a group</param>
		public void DissolveCompound(MathExpViewer mv)
		{
			holder.CreateUndoTransaction("DissolveCompound");
			try
			{
				MathExpGroup g = mv.MathExp as MathExpGroup;
				if (g == null)
				{
					throw new MathException("The item '{0}' is not a group", mv.MathExp.Name);
				}
				VariableList inputs = g.InputVariables;
				//outside links
				Dictionary<UInt32, KeyValuePair<UInt32, UInt32>> linkedOutPorts = new Dictionary<UInt32, KeyValuePair<UInt32, UInt32>>();
				List<UInt32> returnPortLinkedId = new List<UInt32>();
				for (int i = 0; i < g.ReturnIcon.OutPorts.Length; i++)
				{
					if (g.ReturnIcon.OutPorts[i].LinkedPortID != 0)
					{
						returnPortLinkedId.Add(g.ReturnIcon.OutPorts[i].LinkedPortID);
					}
				}
				UInt32 outPortId = 0; //internal item linked to outside item
				if (returnPortLinkedId.Count > 0)
				{
					////g's OutPort is linked to outside items
					//returnPortLinkedId.Count outports will be created for item.MathExpression.OutputVariable
					MathExpItem item = g.GetOutputItem();
					if (item != null)
					{
						outPortId = item.MathExpression.OutputVariable.ID;
					}
				}
				//inports linked to outside's outports
				foreach (Control c in Controls)
				{
					LinkLineNodeOutPort p = c as LinkLineNodeOutPort;
					if (p != null)
					{
						IVariable v = inputs.GetVariableById(p.LinkedPortID);
						if (v != null)
						{
							linkedOutPorts.Add(v.ID, new KeyValuePair<UInt32, UInt32>(p.PortID, p.PortInstanceID));
						}
					}
				}
				g = (MathExpGroup)g.Clone();
				holder.DeleteComponent(mv);
				loadgroup(g, false);
				//link inports
				foreach (KeyValuePair<UInt32, KeyValuePair<UInt32, UInt32>> kv in linkedOutPorts)
				{
					LinkLineNodeInPort pi = GetInPortByID(kv.Key);
					LinkLineNodeOutPort po = GetOutPortByID(kv.Value.Key, kv.Value.Value);
					joinPorts(pi, po, true);

				}
				//link out ports
				if (outPortId != 0)
				{
					LinkLineNodeOutPort po = GetOutPortByID(outPortId, 0); //new item linked to the outside item
					for (int i = 0; i < returnPortLinkedId.Count; i++)
					{
						if (i > 0)
						{
							po.ConstructorParameters = new object[] { po.PortOwner };
							LinkLineNodeOutPort newOut = (LinkLineNodeOutPort)po.Clone();
							this.Controls.Add(newOut);
							newOut.Owner = po.Owner;
							newOut.LabelVisible = false;
							newOut.SetLabelOwner();
							po.PortOwner.AddOutPort(newOut);
							po = newOut;
						}
						LinkLineNodeInPort pi = GetInPortByID(returnPortLinkedId[i]); //original outside item
						joinPorts(pi, po, true); //lines on po will be cleared
					}
				}
				//
				this.Refresh();
				holder.CommitUndoTransaction("DissolveCompound");
			}
			catch (Exception err)
			{
				UIUtil.ShowError(this.FindForm(), err);
				holder.RollbackUndoTransaction("DissolveCompound");
			}
		}
		public void OnMathViewRightClick(MathExpViewer mv, Point p)
		{
			int nSelectionCount = 0;
			ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
			if (selectionService != null)
			{
				bool selected = false;
				ICollection ic = selectionService.GetSelectedComponents();
				if (ic != null)
				{
					nSelectionCount = ic.Count;
					foreach (object v in ic)
					{
						if (v == mv)
						{
							selected = true;
							break;
						}
					}
				}
				if (selected)
				{
				}
				else
				{
					selectionService.SetSelectedComponents(new IComponent[] { mv });
					nSelectionCount = 1;
				}
			}
			if (nSelectionCount > 0 && !(mv.MathExp.Fixed))
			{
				MenuItem mi;
				ContextMenu cm = new ContextMenu();
				//
				if (nSelectionCount > 1)
				{
				}
				else
				{
					mi = new MenuItem("Edit");
					mi.Tag = mv;
					mi.Click += new EventHandler(miEdit_Click);
					cm.MenuItems.Add(mi);
					//
					if (mv.MathExp is MathExpGroup)
					{
						mi = new MenuItem("Dissolve group");
						mi.Tag = mv;
						mi.Click += new EventHandler(miDegroup_Click);
						cm.MenuItems.Add(mi);
					}
				}
				//
				mi = new MenuItem("Delete");
				mi.Click += new EventHandler(miDelete_Click);
				cm.MenuItems.Add(mi);
				//
				cm.Show(this, p);
			}

		}
		MathExpViewer getMenuSender(object sender)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				return mi.Tag as MathExpViewer;
			}
			return null;
		}
		void miCreateCompound_Click(object sender, EventArgs e)
		{
			try
			{
				MathExpViewer mv = getMenuSender(sender);
				if (mv != null)
				{
					CreateCompound(mv.Location, mv.Size);
				}
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(), err);
			}
		}
		void miEdit_Click(object sender, EventArgs e)
		{
			try
			{
				MathExpViewer mv = getMenuSender(sender);
				if (mv != null)
				{
					mv.Edit();
				}
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(), err);
			}
		}
		void miDegroup_Click(object sender, EventArgs e)
		{
			try
			{
				MathExpViewer mv = getMenuSender(sender);
				if (mv != null)
				{
					DissolveCompound(mv);
				}
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(), err);
			}
		}
		void miDelete_Click(object sender, EventArgs e)
		{
			try
			{
				DeleteSelectedComponents();
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(), err);
			}
		}
		#endregion

		#region Properties
		private RaisDataType _returnType;
		[Editor(typeof(UITypeEditorTypeSelector), typeof(UITypeEditor))]
		public virtual RaisDataType ResultValueType
		{
			get
			{
				if (_returnType == null)
				{
					_returnType = new RaisDataType();
					_returnType.LibType = typeof(double);
				}
				return _returnType;
			}
			set
			{
				_returnType = value;
			}
		}
		public bool ShowArrowAtLineBreaks
		{
			get
			{
				return LinkLine.ShowArrows;
			}
			set
			{
				if (LinkLine.ShowArrows != value)
				{
					LinkLine.ShowArrows = value;
					Refresh();
				}
			}
		}

		private string _desc;
		public string Description
		{
			get
			{
				return _desc;
			}
			set
			{
				_desc = value;
			}
		}
		private string _shortDesc;
		public string ShortDescription
		{
			get
			{
				return _shortDesc;
			}
			set
			{
				_shortDesc = value;
			}
		}
		Image gridImage;
		public bool ShowGrid
		{
			get
			{
				return (this.BackgroundImage != null);
			}
			set
			{
				if (value)
				{
					this.BackgroundImage = gridImage;
				}
				else
				{
					this.BackgroundImage = null;
				}
			}
		}
		private ObjectIconData _iconData;
		public ObjectIconData IconImage
		{
			get
			{
				if (_iconData == null)
					_iconData = new ObjectIconData();
				return _iconData;
			}
			set
			{
				_iconData = value;
			}
		}
		#endregion

		#region Design events
		protected override void OnCreateContextMenu(ContextMenu mnu, Point pt)
		{
			base.OnCreateContextMenu(mnu, pt);
			MenuItem mi = new MenuItem("Add Math Expression");
			mi.Click += new EventHandler(miNewMathExp_Click);
			mi.Tag = pt;
			mnu.MenuItems.Add(mi);
			//
			if (holder != null && holder.HasClipboardData())
			{
				mi = new MenuItem("Paste");
				mi.Click += new EventHandler(miPaste_Click);
				mi.Tag = pt;
				mnu.MenuItems.Add(mi);
			}
		}
		protected virtual void OnPortsLinked(LinkLineNodeInPort inport, LinkLineNodeOutPort outPort)
		{
		}
		void miNewMathExp_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				Point pt = (Point)mi.Tag;
				if (holder != null)
				{
					holder.CreateUndoTransaction("miNewMathExp_Click");
					AddNewMathExp(pt.X, pt.Y);
					holder.CommitUndoTransaction("miNewMathExp_Click");
				}
			}
		}
		void miPaste_Click(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				if (holder != null)
				{
					holder.PasteFromClipboard();
				}
			}
		}

		#endregion

		#region IMessageReceiver members
		public bool FireMouseDown(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseDown(e);
			return false;
		}


		public bool FireMouseMove(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseMove(e);
			return false;
		}
		public bool FireMouseUp(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseUp(e);
			return false;
		}
		public bool FireMouseDblClick(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseDoubleClick(e);
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

		#region IMathDesigner Members
		public Control GetUndoControl(UInt32 key)
		{
			foreach (Control c in Controls)
			{
				IActiveDrawing ia = c as IActiveDrawing;
				if (ia != null)
				{
					if (ia.ActiveDrawingID == key)
					{
						return c;
					}
				}
			}
			return null;
		}
		public void AddUndoEntity(UndoEntity entity)
		{
			holder.AddUndoEntity(entity);
		}
		public bool DisableUndo
		{
			get
			{
				return holder.DisableUndo;
			}
			set
			{
				holder.DisableUndo = value;
			}
		}
		public bool NoCompoundCreation
		{
			get
			{
				return _noCompound;
			}
			set
			{
				_noCompound = value;
			}
		}
		public void CreateValue(RaisDataType type, LinkLineNodeInPort inPort, Point pos)
		{
			dlgValue dlg = new dlgValue();
			dlg.Location = pos;
			dlg.SetProperty(new PropertySpec(inPort.Variable.VariableName, inPort.Variable.VariableType.Type, "", "Create a value for the input"));
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				MathNodeRoot newRoot = new MathNodeRoot();
				MathNodeValue mv = new MathNodeValue(newRoot);
				mv.Value = dlg.ReturnValue;
				newRoot[1] = mv;
				((MathNodeVariable)newRoot[0]).VariableType = mv.ValueType;
				Point p = this.PointToClient(pos);
				MathExpViewer mev = holder.AddMathViewer(newRoot, p.X, p.Y);
				mev.Size = new Size(32, 32);
				mev.OutputPorts[0].Label.Visible = false;
				//make the link to the in-port
				LinkLineNode l = mev.OutputPorts[0] as LinkLineNode;
				while (l.NextNode != null)
				{
					l = (LinkLineNode)l.NextNode;
				}
				LinkLineNode m = inPort;
				while (m.PrevNode != null)
				{
					m = (LinkLineNode)m.PrevNode;
				}
				LinkLineNode.JoinToEnd(l, m);
				//
				this.Refresh();
			}
		}
		public void ShowProperties(object v)
		{
			PropertyGrid pr = (PropertyGrid)this.GetService(typeof(PropertyGrid));
			if (pr != null)
			{
				ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
				selectionService.SetSelectedComponents(new Component[] { });
				pr.SelectedObject = v;
			}
		}

		public void DeleteComponent(IComponent c)
		{
			holder.DeleteComponent(c);
		}
		public void DeleteSelectedComponents()
		{
			holder.DeleteSelectedComponents();
		}
		public void ReSelectComponent(IComponent c)
		{
			ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
			if (selectionService != null)
			{
				selectionService.SetSelectedComponents(null);
				Application.DoEvents();
				selectionService.SetSelectedComponents(new IComponent[] { c });
			}
		}
		[Browsable(false)]
		public bool Changed
		{
			get
			{
				return _changed;
			}
			set
			{
				_changed = value;
			}
		}

		public void AddOnShowHandler(EventHandler handler)
		{
			if (onShowHandlers == null)
			{
				onShowHandlers = new List<EventHandler>();
			}
			onShowHandlers.Add(handler);
		}
		/// <summary>
		/// called by a Form holding this control
		/// </summary>
		public void ExecuteOnShowHandlers()
		{
			foreach (Control c in Controls)
			{
				ReturnIcon r = c as ReturnIcon;
				if (r != null)
				{
					r.CheckReturnIconPos(this, null);
					break;
				}
			}
			if (onShowHandlers != null)
			{
				bool b = Changed;
				foreach (EventHandler handler in onShowHandlers)
				{
					handler(this, null);
				}
				onShowHandlers = null;
				Changed = b;
			}
		}
		public object GetDesignerService(Type serviceType)
		{
			if (_services != null)
				if (_services.ContainsKey(serviceType))
					return _services[serviceType];
			return this.GetService(serviceType);
		}

		public void AddDesignerService(Type serviceType, object service)
		{
			if (_services == null)
				_services = new Hashtable();
			if (_services.ContainsKey(serviceType))
				_services[serviceType] = service;
			else
				_services.Add(serviceType, service);
		}
		public bool TestDisabled
		{
			get { return holder.TestDisabled; }
		}
		#endregion

		#region IImageList Members

		public List<ImageID> ImageList
		{
			get
			{
				List<ImageID> list = new List<ImageID>();
				for (int i = 0; i < Controls.Count; i++)
				{
					if (Controls[i] is IImageItem)
					{
						list.Add(((IImageItem)Controls[i]).ImageItem);
					}
				}
				return list;
			}
		}

		#endregion
	}
}
