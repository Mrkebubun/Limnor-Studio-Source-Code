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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.Xml;
using System.CodeDom;
using System.Reflection;
using MathExp.RaisTypes;
using VPL;

namespace MathExp
{
	/// <summary>
	/// the control holding the DesignerFrame for the root component (DigramViewer).
	/// propery grid is used to assist the design.
	/// it is supposed to be a self-contained designer for a group of linked math expressions.
	/// </summary>
	public partial class DiagramDesignerHolder : UserControl, IUnitTestOwner, IUndoHost
	{
		#region fields and constructors
		private DesignSurface dsf;
		private DiagramViewer root;
		private DesignMessageFilter _msgFilter;
		private ISelectionService selectionService;
		private bool bLoading = false;
		private ITestData _testData;
		private System.Threading.Thread thTest;
		private MathExpGroup result;
		private bool _disableUndo;
		private UndoEngine2 _undoEngine;
		private IXmlCodeReader _reader;
		private IXmlCodeWriter _writer;
		//
		const int IMG_Delete_Enable = 8;
		const int IMG_Delete_Disable = 7;
		const int IMG_Copy_Enable = 4;
		const int IMG_Copy_Disable = 3;
		const int IMG_Cut_Enable = 6;
		const int IMG_Cut_Disable = 5;
		const int IMG_Paste_Enable = 10;
		const int IMG_Paste_Disable = 9;
		const int IMG_UndoEnable = 13;
		const int IMG_UndoDisable = 2;
		const int IMG_RedoEnable = 11;
		const int IMG_RedoDisable = 12;
		//
		public DiagramDesignerHolder()
		{
			InitializeComponent();
			btOK.BringToFront();
			btCancel.BringToFront();
			_undoEngine = new UndoEngine2();
			//
			dsf = new DesignSurface(typeof(DiagramViewer));
			Control control = dsf.View as Control; //DesignerFrame
			splitContainer1.Panel2.Controls.Add(control);
			control.Dock = DockStyle.Fill;
			control.Visible = true;
			splitContainer1.SplitterMoved += new SplitterEventHandler(splitContainer1_SplitterMoved);
			this.Resize += new EventHandler(designview_Resize);
			splitContainer1.SplitterWidth = 3;
			//
			IDesignerHost host = (IDesignerHost)dsf.GetService(typeof(IDesignerHost));
			root = (DiagramViewer)host.RootComponent;
			root.Dock = DockStyle.Fill;
			root.AssignHolder(this);
			//
			host.AddService(typeof(PropertyGrid), propertyGrid1);
			host.AddService(typeof(INameCreationService), new NameCreation());
			//
			selectionService = (ISelectionService)dsf.GetService(typeof(ISelectionService));
			if (selectionService != null)
			{
				selectionService.SelectionChanged += new EventHandler(selectionService_SelectionChanged);
			}
			IComponentChangeService componentChangeService = (IComponentChangeService)dsf.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentAdded += new ComponentEventHandler(componentChangeService_ComponentAdded);
			componentChangeService.ComponentRemoving += new ComponentEventHandler(componentChangeService_ComponentRemoving);
			componentChangeService.ComponentRemoved += new ComponentEventHandler(componentChangeService_ComponentRemoved);
			componentChangeService.ComponentChanged += new ComponentChangedEventHandler(componentChangeService_ComponentChanged);
			//
			root.ControlRemoved += new ControlEventHandler(root_ControlRemoved);
			//
			_msgFilter = new DesignMessageFilter(dsf);
			Application.AddMessageFilter(_msgFilter);
			//
			propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);
			propertyGrid1.HelpVisible = false;
			propertyGrid1.PropertySort = PropertySort.Alphabetical;
			propertyGrid1.SelectedObject = root;
			//
		}

		void componentChangeService_ComponentChanged(object sender, ComponentChangedEventArgs e)
		{
			if (!_disableUndo)
			{

				if (e.OldValue != null)
				{
					Point pOld = (Point)e.OldValue;

					if (e.NewValue != null)
					{
						Point pNew = (Point)e.NewValue;
						IActiveDrawing c = e.Component as IActiveDrawing;
						if (c != null)
						{
							UInt32 key = c.ActiveDrawingID;
							UndoEntity entity = new UndoEntity(new PositionUndo(this, key, pOld), new PositionUndo(this, key, pNew));
							AddUndoEntity(entity);
						}
					}
				}
			}
		}

		#endregion
		#region form event handlers
		void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
		{
			timerAdjustFrame.Enabled = false;
			timerAdjustFrame.Enabled = true;
		}
		void designview_Resize(object sender, EventArgs e)
		{
			timerAdjustFrame.Enabled = false;
			timerAdjustFrame.Enabled = true;
		}
		void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			root.Changed = true;
			bool iconChanged = false;
			bool nameChanged = (e.ChangedItem.PropertyDescriptor.Name == RaisDataType.PROP_NAME);
			bool isRoot = (propertyGrid1.SelectedObject is DiagramViewer);
			bool isMathViewer = (propertyGrid1.SelectedObject is MathExpViewer);
			GridItem gi = e.ChangedItem;
			while (gi != null)
			{
				if (gi.Value is Image)
				{
				}
				else if (gi.Value is EnumIconType)
				{
				}
				else if (gi.Value is ObjectIconData)
				{
					iconChanged = true;
					break;
				}
				gi = gi.Parent;
			}
			if (isRoot && nameChanged)
			{
				root.Name = (string)e.ChangedItem.Value;
			}
			if (iconChanged || (isRoot && nameChanged))
			{
				loadIconImage();
			}
			else if (isMathViewer)
			{
				if (e.ChangedItem.Value is Point || e.ChangedItem.Value is Font)
				{
					((MathExpViewer)propertyGrid1.SelectedObject).RefreshImage();
				}
			}
			if (nameChanged)
			{
				root.Refresh();
			}
		}

		void selectionService_SelectionChanged(object sender, EventArgs e)
		{
			if (selectionService != null)
			{
				ICollection selectedComponents = selectionService.GetSelectedComponents();
				object[] comps = new object[selectedComponents.Count];
				int i = 0;

				foreach (Object o in selectedComponents)
				{
					comps[i] = o;
					i++;
				}
				propertyGrid1.SelectedObjects = comps;
				if (propertyGrid1.SelectedObject == null || propertyGrid1.SelectedObject == root)
				{
					btCopy.Enabled = false;
					btCopy.ImageIndex = IMG_Copy_Disable;
					btCut.Enabled = false;
					btCut.ImageIndex = IMG_Cut_Disable;
					btDelete.Enabled = false;
					btDelete.ImageIndex = IMG_Delete_Disable;
				}
				else
				{
					btCopy.Enabled = true;
					btCopy.ImageIndex = IMG_Copy_Enable;
					btCut.Enabled = true;
					btCut.ImageIndex = IMG_Cut_Enable;
					btDelete.Enabled = true;
					btDelete.ImageIndex = IMG_Delete_Enable;
				}
			}
		}
		public void OnClosing()
		{
			Application.RemoveMessageFilter(_msgFilter);
		}
		public void OnOK()
		{
			result = root.Export();
			root.Changed = false;
			Form f = FindForm();
			f.DialogResult = DialogResult.OK;
		}
		private void btOK_Click(object sender, EventArgs e)
		{
			OnOK();
		}
		private void timerAdjustFrame_Tick(object sender, EventArgs e)
		{
			timerAdjustFrame.Enabled = false;
			adjustFrame();
		}
		#endregion
		#region public methods
		public void SetSerializers(IXmlCodeReader reader, IXmlCodeWriter writer)
		{
			_reader = reader;
			_writer = writer;
		}
		public void DisableTest()
		{
			btTest.Enabled = false;
		}
		public void AddService(Type t, object service)
		{
			IDesignerHost host = (IDesignerHost)dsf.GetService(typeof(IDesignerHost));
			host.AddService(t, service);
		}
		public void adjustFrame()
		{
			bool b = (selectionService.PrimarySelection == root);
			if (b || selectionService.PrimarySelection == null)
			{
				IDesignerHost host = (IDesignerHost)dsf.GetService(typeof(IDesignerHost));
				for (int i = 0; i < host.Container.Components.Count; i++)
				{
					if (host.Container.Components[i] != root)
					{
						selectionService.SetSelectedComponents(new IComponent[] { host.Container.Components[i] });
						Application.DoEvents();
						Application.DoEvents();
						break;
					}
				}
				if (b)
				{
					selectionService.SetSelectedComponents(new IComponent[] { root });
				}
			}
		}
		public MathExpViewer AddEmptyMathExpViewer()
		{
			IDesignerHost host = (IDesignerHost)dsf.GetService(typeof(IDesignerHost));
			MathExpViewer v = (MathExpViewer)host.CreateComponent(typeof(MathExpViewer));
			root.Controls.Add(v);
			return v;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public MathExpViewer LoadMathItem(MathExpItem item)
		{
			if (string.IsNullOrEmpty(item.Name))
			{
				item.Name = this.CreateName("item");
			}
			MathExpViewer v = AddEmptyMathExpViewer();
			//
			v.ImportMathItem(item);
			return v;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="MathExpression"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public MathExpViewer AddMathViewer(MathNodeRoot MathExpression, int x, int y)
		{
			string name = CreateName(XmlSerialization.XML_Math);
			IDesignerHost host = (IDesignerHost)dsf.GetService(typeof(IDesignerHost));
			MathExpViewer v = (MathExpViewer)host.CreateComponent(typeof(MathExpViewer));
			root.Controls.Add(v);
			v.Visible = true;
			v.Location = new Point(x, y);
			//
			v.LoadData((MathNodeRoot)MathExpression.Clone());
			//
			v.Name = name;
			v.Site.Name = v.Name;
			propertyGrid1.SelectedObject = v;
			root.Refresh();
			root.Changed = true;
			return v;

		}
		public void SetMathName(string name)
		{
			root.Name = name;
			root.Site.Name = name;
		}

		public ReturnIcon CreateReturnIcon(RaisDataType t)
		{
			root.ResultValueType = t;
			ReturnIcon ri = new ReturnIcon(t);
			root.Controls.Add(ri);
			root.Controls.Add(ri.InPort);
			return ri;
		}
		public void AddControls(Control[] ctrls)
		{
			root.Controls.AddRange(ctrls);
		}
		public void ClearComponents()
		{
			bLoading = true;
			IDesignerHost host = (IDesignerHost)dsf.GetService(typeof(IDesignerHost));
			if (host != null)
			{
				ComponentCollection cc = host.Container.Components;
				foreach (IComponent c in cc)
				{
					if (c != root)
					{
						host.DestroyComponent(c);
					}
				}
			}
		}
		public void LoadGroup(MathExpGroup mathGroup)
		{
			bool b = _disableUndo;
			_disableUndo = true;
			ClearComponents();
			//
			IDesignServiceProvider sp = MathNode.GetGlobalServiceProvider(mathGroup);
			if (sp != null)
			{
				MathNode.RegisterGetGlobalServiceProvider(root, sp);
			}
			root.LoadGroup(mathGroup);
			designview_Resize(null, null);
			//
			picIcomImage.Size = mathGroup.IconImage.IconSize;
			picIcomImage.Image = mathGroup.CreateIcon(root.CreateGraphics());
			enablePaste();
			root.Changed = false;
			bLoading = false;
			root.Refresh();
			_disableUndo = b;
			result = mathGroup;
		}
		public List<ParameterDrawing> GetParameters()
		{
			return root.GetParameters();
		}
		public void RemoveParameter(ParameterDrawing p)
		{
			root.RemoveParameter(p);
		}
		public string CreateName(string baseName)
		{
			IDesignerHost host = (IDesignerHost)dsf.GetService(typeof(IDesignerHost));
			NameCreation nc = (NameCreation)host.GetService(typeof(INameCreationService));
			IContainer c = (IContainer)host.GetService(typeof(IContainer));
			return nc.CreateName(c, baseName);
		}
		public bool HasClipboardData()
		{
			return Clipboard.ContainsData("MathItem");
		}
		public void PasteFromClipboard()
		{
			if (Clipboard.ContainsData("MathItem"))
			{
				object data = Clipboard.GetData("MathItem");
				if (data != null)
				{
					string s = data.ToString();
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(s);
					if (doc.DocumentElement != null)
					{
						try
						{
							CreateUndoTransaction("PasteFromClipboard");
							MathExpGroup mg = new MathExpGroup();
							MathExpItem mi = new MathExpItem(mg);
							mi.ReadFromXmlNode(_reader, doc.DocumentElement);
							mi.Name = CreateName(XmlSerialization.XML_Math);
							mi.Location = new Point(mi.Location.X + 5, mi.Location.Y + 5);
							//re-arrange the ports
							mi.ReCreateDefaultPorts();
							this.LoadMathItem(mi);
							//create controls, same as DiagramViewer.LoadGroup
							root.Controls.AddRange(mi.GetPortControls());
							mi.CreateLinkLines();
							CommitUndoTransaction("PasteFromClipboard");
						}
						catch (Exception err)
						{
							MessageBox.Show(err.Message);
							RollbackUndoTransaction("PasteFromClipboard");
						}
					}
				}
			}
		}
		/// <summary>
		/// called by a Form holding this control
		/// </summary>
		public void ExecuteOnShowHandlers()
		{
			if (root != null)
			{
				root.ExecuteOnShowHandlers();
			}
		}
		public void DeleteComponent(IComponent c)
		{
			IDesignerHost host = (IDesignerHost)dsf.GetService(typeof(IDesignerHost));
			if (host != null)
			{
				host.DestroyComponent(c);
				root.Changed = true;
			}
		}
		public void DeleteSelectedComponents()
		{
			try
			{
				bool bSaved = false;
				ISelectionService ss = (ISelectionService)dsf.GetService(typeof(ISelectionService));
				if (ss != null)
				{
					ICollection cc = ss.GetSelectedComponents();
					if (cc != null)
					{
						IDesignerHost host = (IDesignerHost)dsf.GetService(typeof(IDesignerHost));
						if (host != null)
						{
							CreateUndoTransaction("deleteSelectedComponent");
							foreach (object v in cc)
							{
								if (v is System.ComponentModel.IComponent)
								{
									if (v != host.RootComponent)
									{
										if (!bSaved)
										{
											bSaved = true;
											bLoading = true;
											bLoading = false;
										}
										host.DestroyComponent((System.ComponentModel.IComponent)v);
									}
								}
							}
							root.Changed = true;
							CommitUndoTransaction("deleteSelectedComponent");
						}
					}
				}
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(), err);
				RollbackUndoTransaction("deleteSelectedComponent");
			}
		}
		#endregion
		#region component event handlers

		void componentChangeService_ComponentRemoving(object sender, ComponentEventArgs e)
		{
			if (bLoading)
				return;

			MathExpViewer mv = e.Component as MathExpViewer;
			if (mv != null)
			{
				List<LinkLineNodePort> ports = mv.GetPorts();
				for (int i = 0; i < ports.Count; i++)
				{
					LinkLineNode l = ports[i];
					l.ClearLine();
					root.Controls.Remove(ports[i]);
					root.Controls.Remove(ports[i].Label);
					if (l is LinkLineNodeInPort)
					{
						if (l.LinkedOutPort == null)
						{
							//it is not linked to an output, remove all nodes
							l = (LinkLineNode)l.PrevNode;
							while (l != null)
							{
								root.Controls.Remove(l);
								l = (LinkLineNode)l.PrevNode;
							}
						}
						else
						{
							l.LinkedOutPort.LinkedPortID = 0;
							//replace this port with a node
							LinkLineNode prev = (LinkLineNode)l.PrevNode;
							LinkLineNode end = new LinkLineNode(null, prev);
							end.Location = l.Location;
							root.Controls.Add(end);
							prev.SetNext(end);
							if (prev.Line == null)
							{
								prev.CreateForwardLine();
							}
							else
							{
								if (prev.Line.EndPoint == l)
								{
									prev.Line.SetEnd(end);
								}
								else
								{
									end.CreateBackwardLine();
								}
							}
						}
					}
					else if (l is LinkLineNodeOutPort)
					{
						if (l.LinkedInPort == null)
						{
							//it is not linked to an input, remove all nodes
							l = (LinkLineNode)l.NextNode;
							while (l != null)
							{
								root.Controls.Remove(l);
								l = (LinkLineNode)l.NextNode;
							}
						}
						else
						{
							l.LinkedInPort.LinkedPortID = 0;
							//replace this port with a node
							LinkLineNode next = (LinkLineNode)l.NextNode;
							if (next.Parent != null)
							{
								LinkLineNode end = new LinkLineNode(next, null);
								end.Location = l.Location;
								next.Parent.Controls.Add(end);
								next.SetPrevious(end);
								if (next.Line == null)
								{
									next.CreateBackwardLine();
								}
								else
								{
									if (next.Line.StartPoint == l)
									{
										next.Line.SetStart(end);
									}
									else
									{
										end.CreateForwardLine();
									}
								}
							}
						}
					}
				}
			}
		}

		void componentChangeService_ComponentRemoved(object sender, ComponentEventArgs e)
		{
			root.Refresh();
		}

		void componentChangeService_ComponentAdded(object sender, ComponentEventArgs e)
		{
			if (bLoading)
				return;
			if (!_disableUndo)
			{
				//SaveCurrentStateForUndo();
			}
		}
		void root_ControlRemoved(object sender, ControlEventArgs e)
		{


		}
		#endregion
		#region private methods
		private void loadIconImage()
		{
			MathExpGroup mathExpGroup = root.Export();
			picIcomImage.Size = mathExpGroup.IconImage.IconSize;
			picIcomImage.Image = mathExpGroup.CreateIcon(root.CreateGraphics());
		}
		private void enablePaste()
		{
			if (HasClipboardData())
			{
				btPaste.Enabled = true;
				btPaste.ImageIndex = IMG_Paste_Enable;
			}
			else
			{
				btPaste.Enabled = false;
				btPaste.ImageIndex = IMG_Paste_Disable;
			}
		}
		private void copyToClipboard()
		{
			if (selectionService.PrimarySelection != null)
			{
				MathExpViewer mv = selectionService.PrimarySelection as MathExpViewer;
				if (mv != null)
				{
					MathExpGroup mg = new MathExpGroup();
					MathExpItem mi = mv.ExportMathItem(mg);
					XmlDocument doc = new XmlDocument();
					XmlNode node = doc.CreateElement("MathExpression");
					doc.AppendChild(node);
					mi.SaveToXmlNode(_writer, node);
					Clipboard.SetData("MathItem", doc.OuterXml);
					enablePaste();
				}
			}
		}
		private void cutToClipboard()
		{
			if (selectionService.PrimarySelection != null)
			{
				MathExpViewer mv = selectionService.PrimarySelection as MathExpViewer;
				if (mv != null)
				{
					CreateUndoTransaction("cutToClipboard");
					MathExpGroup mg = new MathExpGroup();
					MathExpItem mi = mv.ExportMathItem(mg);
					XmlDocument doc = new XmlDocument();
					XmlNode node = doc.CreateElement("MathExpression");
					doc.AppendChild(node);
					mi.SaveToXmlNode(_writer, node);
					Clipboard.SetData("MathItem", doc.OuterXml);
					enablePaste();
					DeleteComponent(mv);
					CommitUndoTransaction("cutToClipboard");
				}
			}
		}
		private void enableUndo()
		{
			if (HasUndo)
			{
				btUndo.Enabled = true;
				btUndo.ImageIndex = IMG_UndoEnable;
			}
			else
			{
				btUndo.Enabled = false;
				btUndo.ImageIndex = IMG_UndoDisable;
			}
			if (HasRedo)
			{
				btRedo.Enabled = true;
				btRedo.ImageIndex = IMG_RedoEnable;
			}
			else
			{
				btRedo.Enabled = false;
				btRedo.ImageIndex = IMG_RedoDisable;
			}
		}

		#endregion
		#region properties
		public bool IsLoading
		{
			get
			{
				return bLoading;
			}
			set
			{
				bLoading = value;
			}
		}
		public bool DisableUndo
		{
			get
			{
				return _disableUndo;
			}
			set
			{
				_disableUndo = value;
			}
		}
		public bool NoCompound
		{
			get
			{
				return root.NoCompoundCreation;
			}
			set
			{
				root.NoCompoundCreation = value;
			}
		}
		public bool TestDisabled
		{
			get
			{
				return !btTest.Enabled;
			}
		}
		public MathExpGroup Result
		{
			get
			{
				return result;
			}
		}

		public bool Changed
		{
			get
			{
				if (root == null)
					return false;
				return root.Changed;
			}
		}
		#endregion
		#region button click event handlers
		private void btInsert_Click(object sender, EventArgs e)
		{
			root.AddNewMathExp(30, 30);
		}

		private void btDelete_Click(object sender, EventArgs e)
		{
			DeleteSelectedComponents();
		}

		private void btUndo_Click(object sender, EventArgs e)
		{
			try
			{
				Undo();
				root.Changed = true;
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
			}
		}

		private void btRedo_Click(object sender, EventArgs e)
		{
			try
			{
				Redo();
				root.Changed = true;
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
			}
		}

		private void btCopy_Click(object sender, EventArgs e)
		{
			try
			{
				copyToClipboard();
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
			}
		}

		private void btPaste_Click(object sender, EventArgs e)
		{
			try
			{
				PasteFromClipboard();
				root.Changed = true;
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
			}
		}

		private void btCut_Click(object sender, EventArgs e)
		{
			try
			{
				cutToClipboard();
				root.Changed = true;
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
			}
		}
		#endregion
		#region Undo Engine
		public void ResetUndo()
		{
			_undoEngine.ClearStack();
		}
		[Browsable(false)]
		public bool HasUndo
		{
			get
			{
				return _undoEngine.HasUndo;
			}
		}
		[Browsable(false)]
		public bool HasRedo
		{
			get
			{
				return _undoEngine.HasRedo;
			}
		}
		public void Undo()
		{
			IUndoUnit obj = _undoEngine.UseUndo();
			if (obj != null)
			{
				obj.Apply();
				enableUndo();
			}
		}
		public void Redo()
		{
			IUndoUnit obj = _undoEngine.UseRedo();
			if (obj != null)
			{
				obj.Apply();
				enableUndo();
			}
		}
		Dictionary<string, MathExpGroupUndo> _undoTransaction;
		public void CreateUndoTransaction(string transactionName)
		{
			if (!_disableUndo)
			{
				_disableUndo = true;
				if (_undoTransaction == null)
				{
					_undoTransaction = new Dictionary<string, MathExpGroupUndo>();
				}
				if (!_undoTransaction.ContainsKey(transactionName))
				{
					_undoTransaction.Add(transactionName, new MathExpGroupUndo(this, (MathExpGroup)root.Export().Clone()));
				}
				else
				{
					_undoTransaction[transactionName] = new MathExpGroupUndo(this, (MathExpGroup)root.Export().Clone());
				}
			}
		}
		public void CommitUndoTransaction(string transactionName)
		{
			if (_undoTransaction != null && _undoTransaction.ContainsKey(transactionName))
			{
				MathExpGroupUndo _currStates = _undoTransaction[transactionName];
				MathExpGroupUndo objRedo = new MathExpGroupUndo(this, (MathExpGroup)root.Export().Clone());
				UndoEntity undo = new UndoEntity(_currStates, objRedo);
				_undoTransaction.Remove(transactionName);
				_undoEngine.AddUndoEntity(undo);
				enableUndo();
				_disableUndo = false;
			}
		}
		public void RollbackUndoTransaction(string transactionName)
		{
			if (_undoTransaction != null && _undoTransaction.ContainsKey(transactionName))
			{
				_undoTransaction.Remove(transactionName);
				_disableUndo = false;
			}
		}
		public void AddUndoEntity(UndoEntity entity)
		{
			if (!_disableUndo)
			{
				_undoEngine.AddUndoEntity(entity);
				enableUndo();
			}
		}
		public Control GetUndoControl(UInt32 key)
		{
			return root.GetUndoControl(key);
		}
		public void Apply(MathExpGroup item)
		{
			bLoading = true;
			this.LoadGroup(item);
			bLoading = false;
		}

		public void FireStateChange()
		{
			enableUndo();
		}

		#endregion
		#region unit test
		private void btTest_Click(object sender, EventArgs e)
		{
			btTest.Enabled = false;
			try
			{
				_testData = null;
				//generate the result
				result = root.Export();
				//
				//check if compilation is needed
				IObjectTypeUnitTester uc = (IObjectTypeUnitTester)MathNode.GetService(typeof(IObjectTypeUnitTester));
				if (uc != null)
				{
					_testData = uc.UseMemberTest(this);
				}
				if (_testData == null)
				{
					CompileResult compiled = result.CreateMethodCompilerUnit("TestMathExpression", "Test", "TestMathExpGroup");
					//
					thTest = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(test));
					thTest.SetApartmentState(System.Threading.ApartmentState.STA);
					_testData = new TestData(compiled);
					thTest.Start(_testData);
				}
				//
				timer1.Enabled = true;
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
			}
		}
		private void test(object data)
		{
			dlgTest dlg = new dlgTest();
			dlg.LoadData((ITestData)data);
			dlg.TopMost = true;
			dlg.ShowDialog(null);
		}
		/// <summary>
		/// checking for the exiting of thread 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer1_Tick(object sender, EventArgs e)
		{
			if (_testData == null)
				timer1.Enabled = false;
			else
			{
				if (_testData.Finished)
				{
					thTest = null;
					timer1.Enabled = false;
					btTest.Enabled = true;
					//MathNode.RestoreTrace();
				}
			}
		}
		public void AbortTest()
		{
			if (thTest != null)
			{
				if (thTest.ThreadState != System.Threading.ThreadState.Unstarted && thTest.ThreadState != System.Threading.ThreadState.Aborted && thTest.ThreadState != System.Threading.ThreadState.AbortRequested && thTest.ThreadState != System.Threading.ThreadState.Stopped && thTest.ThreadState != System.Threading.ThreadState.StopRequested)
				{
					thTest.Abort();
				}
			}
		}
		public static void CreateTestMethod(IMathExpression result, CodeTypeDeclaration t, CodeNamespace ns, CodeMemberMethod m, AssemblyRefList imports, VariableList parameters, List<IPropertyPointer> pointerList)
		{
			result.GetAllImports(imports);
			//
			m.ReturnType = new CodeTypeReference(result.DataType.Type);
			//
			m.Comments.Add(new CodeCommentStatement("Variable mapping:"));
			m.Comments.Add(new CodeCommentStatement("In formula:  method parameter"));
			//
			MethodType mt = new MethodType();
			mt.MethodCode = m;
			result.GenerateInputVariables();
			VariableList variables = result.InputVariables;
			Dictionary<string, IPropertyPointer> pointers = new Dictionary<string, IPropertyPointer>();
			result.GetPointers(pointers);
			int n = variables.Count;
			MathNode.Trace("Generate arguments from {0} input variables", n);
			MathNode.IndentIncrement();
			for (int k = 0; k < n; k++)
			{
				IVariable var = variables[k];
				MathNode.Trace(k, var);
				if (!(var is MathNodeVariableDummy) && !var.IsParam && !var.IsConst)
				{
					string paramName = "";
					parameters.Add(var);
					paramName = var.CodeVariableName;
					CodeParameterDeclarationExpression p = new CodeParameterDeclarationExpression(new CodeTypeReference(var.VariableType.Type), paramName);
					m.Parameters.Add(p);
					//add comment
					string sub = var.SubscriptName;
					if (string.IsNullOrEmpty(sub))
						sub = " ";
					m.Comments.Add(new CodeCommentStatement(string.Format("{0}{1}:\t {2}", var.VariableName, sub, var.CodeVariableName)));
					MathNode.IndentIncrement();
					MathNode.Trace("Argument {0} {1} for {2}, {3}", var.VariableType.Type, paramName, var.TraceInfo, var.GetType());
					MathNode.IndentDecrement();
					//
					result.AssignCodeExp(new CodeArgumentReferenceExpression(paramName), var.CodeVariableName);
				}
			}
			MathNode.Trace("Generate arguments from {0} pointers", pointers.Count);
			foreach (KeyValuePair<string, IPropertyPointer> kv in pointers)
			{
				string paramName = "";
				pointerList.Add(kv.Value);
				paramName = kv.Value.CodeName;
				CodeParameterDeclarationExpression p = new CodeParameterDeclarationExpression(new CodeTypeReference(kv.Value.ObjectType), paramName);
				m.Parameters.Add(p);
				//add comment
				m.Comments.Add(new CodeCommentStatement(string.Format("{0}:\t {1}", kv.Value.ToString(), paramName)));
				MathNode.IndentIncrement();
				MathNode.Trace("Argument {0} {1} for {2}", kv.Value.ObjectType, paramName, kv.Value.ToString());
				MathNode.IndentDecrement();
			}
			MathNode.IndentDecrement();
			//do the compiling
			CodeExpression ce = result.ReturnCodeExpression(mt);
			//
			MathNode.Trace("Test method returns {0}, compiled type: {1}", result.DataType.Type, result.ActualCompileDataType.Type);
			if (result.ActualCompileDataType.Type.Equals(result.DataType.Type))
			{
				CodeMethodReturnStatement mr = new CodeMethodReturnStatement(ce);
				m.Statements.Add(mr);
			}
			else
			{
				if (result.ActualCompileDataType.IsVoid)
				{
					m.Statements.Add(new CodeExpressionStatement(ce));
					CodeMethodReturnStatement mr = new CodeMethodReturnStatement(ValueTypeUtil.GetDefaultValueByType(result.DataType.Type));
					m.Statements.Add(mr);
				}
				else
				{
					if (result.DataType.IsVoid)
					{
						m.Statements.Add(new CodeExpressionStatement(ce));
					}
					else
					{
						if (result.DataType.Type.Equals(typeof(string)))
						{
							CodeMethodReturnStatement mr = new CodeMethodReturnStatement(new CodeMethodInvokeExpression(ce, "ToString", new CodeExpression[] { }));
							m.Statements.Add(mr);
						}
						else
						{
							CodeExpression mie = RaisDataType.GetConversionCode(result.ActualCompileDataType, ce, result.DataType, m.Statements);
							if (mie != null)
							{
								CodeMethodReturnStatement mr = new CodeMethodReturnStatement(mie);
								m.Statements.Add(mr);
							}
						}
					}
				}
			}
		}
		#endregion
		#region IUnitTestOwner Members
		public void AddMethod(CodeTypeDeclaration t, CodeNamespace ns, CodeMemberMethod m, AssemblyRefList imports, VariableList parameters, List<IPropertyPointer> pointerList)
		{
			CreateTestMethod(result, t, ns, m, imports, parameters, pointerList);
		}
		public IMathExpression MathExpression
		{
			get { return result; }
		}

		#endregion
	}
}
