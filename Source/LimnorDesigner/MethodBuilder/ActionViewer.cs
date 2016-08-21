/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MathExp;
using System.Xml;
using System.ComponentModel.Design;
using System.Collections.Specialized;
using MathExp.RaisTypes;
using System.Collections;
using System.Drawing.Drawing2D;
using LimnorDesigner.Action;
using VSPrj;
using XmlSerializer;
using VPL;
using System.Drawing.Design;
using System.Xml.Serialization;
using System.Globalization;
using WindowsUtility;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// this viewer is for Action even though its member _act is not of ActionClass.
	/// TBD: more typed arrangement to declare Action for _act
	/// </summary>
	public abstract partial class ActionViewer : UserControl, IMessageReceiver, ICustomTypeDescriptor, IActiveDrawing
	{
		#region Fields and constructors
		private Bitmap img;
		ActionBranch _act;
		private bool bLoading;
		Pen penShade;
		protected bool _messageReturn = false; //allow message to go through
		private Font _textFont;
		private Color _textColor = Color.Black;
		private Brush _textBrush;
		private Guid _parentGuid;
		/// <summary>
		/// visible properties
		/// </summary>
		private StringCollection propertyNames;
		public ActionViewer()
		{
			InitializeComponent();
			//_location = this.Location;
			penShade = new Pen(Brushes.Gray, 2);
			propertyNames = new StringCollection();
			propertyNames.Add("ActionName");
			propertyNames.Add("Location");
			propertyNames.Add("Size");
			propertyNames.Add("Description");
			propertyNames.Add("TextFont");
			propertyNames.Add("TextColor");
			propertyNames.Add("IsMainThread");
		}
		#endregion
		#region Properties
		[Editor(typeof(TypeEditorHide), typeof(UITypeEditor))]
		[Description("Parameter values for this action")]
		public virtual ListProperty<ParameterValue> ActionParameters
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public virtual bool NameReadOnly
		{
			get
			{
				return _act.IsNameReadOnly;
			}
		}
		public bool IsValid
		{
			get
			{
				if (_act != null)
				{
					return _act.IsValid;
				}
				else
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_act is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				}
				return false;
			}
		}
		public Guid ParentGuid
		{
			get
			{
				return _parentGuid;
			}
			set
			{
				_parentGuid = value;
			}
		}
		public MethodDiagramViewer DiagramViewer
		{
			get
			{
				Control p = this.Parent;
				while (p != null)
				{
					MethodDiagramViewer mv = p as MethodDiagramViewer;
					if (mv != null)
					{
						return mv;
					}
					p = p.Parent;
				}
				return null;
			}
		}
		[Description("Font for drawing action name")]
		public Font TextFont
		{
			get
			{
				if (_textFont == null)
				{
					_textFont = new Font("Times New Roman", 12);
				}
				return _textFont;
			}
			set
			{
				if (value != null)
				{
					_textFont = value;
					this.Refresh();
				}
			}
		}
		[Description("Color for drawing action name")]
		public Color TextColor
		{
			get
			{
				if (_textColor == Color.Empty)
					_textColor = Color.Black;
				return _textColor;
			}
			set
			{
				_textColor = value;
				if (_textColor == Color.Empty)
					_textColor = Color.Black;
				_textBrush = new SolidBrush(_textColor);
				this.Refresh();
			}
		}
		public Brush TextBrush
		{
			get
			{
				if (_textBrush == null)
				{
					_textBrush = new SolidBrush(TextColor);
				}
				return _textBrush;
			}
		}
		public ActionBranch ActionObject
		{
			get
			{
				return _act;
			}
		}
		[ParenthesizePropertyName(true)]
		[Description("Name of the action")]
		public virtual string ActionName
		{
			get
			{
				return _act.Name;
			}
			set
			{
				_act.Name = value;
			}
		}
		[Description("When this action is the first action in a thread, this value indicates that whether this thread goes with the main execution thread of the running application.")]
		public bool IsMainThread
		{
			get
			{
				return _act.IsMainThread;
			}
			set
			{
				_act.IsMainThread = value;
			}
		}

		public string Description
		{
			get
			{
				return _act.Description;
			}
			set
			{
				_act.Description = value;
			}
		}
		public PortCollection Ports
		{
			get
			{
				if (_act == null)
					return new PortCollection();
				return _act.GetAllPorts();
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		[XmlIgnore]
		public bool KeepRemovedAction
		{
			get;
			set;
		}
		#endregion
		#region Protected Properties
		protected Bitmap ActionImage
		{
			get
			{
				return img;
			}
		}
		protected bool IsLoading
		{
			get
			{
				return bLoading;
			}
		}
		protected Pen ShapePen
		{
			get
			{
				return penShade;
			}
		}
		protected Bitmap BreakPointImage
		{
			get
			{
				if (_act != null)
				{
					return _act.BreakPointImage;
				}
				return null;
			}
		}
		#endregion
		#region public methods
		public void SetActionImage(Bitmap im)
		{
			img = im;
		}
		public virtual void ResetDisplay()
		{
		}
		/// <summary>
		/// called when it is linked to a previous action
		/// </summary>
		public void CreateDefaultInputUsage()
		{
			ActionObject.CreateDefaultInputUsage();
			CreateImage();
			Refresh();
		}
		public void InitializeInput()
		{
			//find out the Input data type
			if (_act != null)
			{
				if (_act.InPortList != null && _act.InPortList.Count > 0)
				{
					for (int i = 0; i < _act.InPortList.Count; i++)
					{
						if (_act.InPortList[i].LinkedOutPort != null)
						{
							ActionViewer av = _act.InPortList[i].LinkedOutPort.Owner as ActionViewer;
							if (av != null)
							{
								_act.InputName = av.ActionObject.OutputCodeName;
								_act.InputType = av.ActionObject.OutputType;
								_act.SetInputName(av.ActionObject.OutputCodeName, av.ActionObject.OutputType);
								break;
							}
						}
					}
				}
			}
		}
		public void ClearBreakpointStatus()
		{
			_act.AtBreak = EnumActionBreakStatus.None;
		}
		public void SetBreakpointStatus(EnumActionBreakStatus status)
		{
			_act.AtBreak = status;
		}
		public void SetActionName(string name)
		{
			_act.Name = name;
			this.Refresh();
		}
		protected void AddPropertyName(string name)
		{
			propertyNames.Add(name);
		}
		protected void RemovePropertyName(string name)
		{
			if (propertyNames.Contains(name))
				propertyNames.Remove(name);
		}
		protected virtual void OnPreInitializeComponent()
		{
		}
		protected virtual void OnInitializeComponent()
		{
		}
		protected virtual void OnImportAction()
		{
			TextFont = this.ActionObject.TextFont;
			TextColor = this.ActionObject.TextColor;
		}
		protected virtual void OnInitNewPorts()
		{
		}
		protected virtual void OnInitExistingPorts()
		{
		}
		public virtual void OnDeleteAction(ClassPointer root)
		{
		}
		public void UpdateAction(ActionBranch item)
		{
			_act = item;
			OnActonChanged();
		}
		protected virtual void OnActonChanged()
		{
			List<ActionPortIn> inPorts = new List<ActionPortIn>();
			List<ActionPortOut> outPorts = new List<ActionPortOut>();
			foreach (Control c in Parent.Controls)
			{
				ActionPortIn ai = c as ActionPortIn;
				if (ai != null)
				{
					if (ai.Owner == this)
					{
						inPorts.Add(ai);
					}
				}
				else
				{
					ActionPortOut ao = c as ActionPortOut;
					if (ao != null)
					{
						if (ao.Owner == this)
						{
							outPorts.Add(ao);
						}
					}
				}
			}
			this.ActionObject.InPortList = inPorts;
			this.ActionObject.OutPortList = outPorts;
		}
		public void ImportAction(ActionBranch item, bool isNewAction)
		{
			bLoading = true;
			this._act = item;
			Point pos = new Point(item.Location.X < 0 ? 0 : item.Location.X, item.Location.Y < 0 ? 0 : item.Location.Y);
			this.Location = pos;
			this.Size = item.Size;
			this.Description = item.Description;
			this.TextFont = item.TextFont;
			this.TextColor = item.TextColor;
			OnImportAction();
			bLoading = false;
			setupPorts(isNewAction);
			CreateImage();
			MethodDiagramViewer dv = this.Parent as MethodDiagramViewer;
			if (dv != null)
			{
				_parentGuid = dv.GUID;
			}
		}
		public ActionBranch ExportAction()
		{
			_act.Location = this.Location;
			_act.Size = this.Size;
			_act.TextColor = this.TextColor;
			_act.TextFont = this.TextFont;
			return _act;
		}
		#endregion
		#region private methods
		private void setupPorts(bool isNewAction)
		{
			if (isNewAction)
			{
				_act.InitializePorts(this);
			}
			PortCollection pc = _act.GetAllPorts();
			if (!isNewAction)
			{
				foreach (LinkLineNodePort p in pc)
				{
					p.Owner = this;
				}
			}
			List<Control> ctrls = pc.GetAllControls(false);
			Control[] a = new Control[ctrls.Count];
			ctrls.CopyTo(a);
			Parent.Controls.AddRange(a);

			if (isNewAction)
			{
				OnInitNewPorts();
				Parent.Refresh();
			}
			else
			{
				for (int i = 0; i < a.Length; i++)
				{
					ActiveDrawing ad = a[i] as ActiveDrawing;
					if (ad != null)
					{
						ad.RestoreLocation();
					}
				}
				OnInitExistingPorts();
			}
		}
		/// <summary>
		/// edit this action
		/// </summary>
		/// <param name="exp"></param>
		private void showEditor(IMathExpression exp)
		{
			try
			{
				Rectangle rc = this.Parent.RectangleToScreen(this.Bounds);
				IMathEditor dlg = exp.CreateEditor(rc);
				IMathDesigner md = this.Parent as IMathDesigner;
				if (md.TestDisabled)
				{
					dlg.DisableTest();
				}
				if(((Form)dlg).ShowDialog(this.FindForm()) == DialogResult.OK)
				{
					setChanged();
				}
			}
			catch (Exception err)
			{
				MathNode.Log(this.FindForm(),err);
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
		protected void CreateImage()
		{
			Graphics g = this.CreateGraphics();
			img = _act.CreateIcon(g);
			g.Dispose();
		}
		#endregion
		#region overrides
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x20;
				return cp;
			}
		}
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			VPLDrawing.VplDrawing.FillShadeRoundRectangle(e.Graphics, this.ClientSize, 100, Color.White, Color.LightGray, Pens.White, Pens.LightGray);
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			IMathDesigner md = this.Parent as IMathDesigner;
			if (md != null)
			{
				md.ReSelectComponent(this);
			}
			setChanged();
		}
		protected override void OnMove(EventArgs e)
		{
			RecreateHandle();
			base.OnMove(e);
			setChanged();
		}
		protected virtual void OnPaintValidIcon(PaintEventArgs e)
		{
			if (!this.IsValid)
			{
				e.Graphics.DrawIcon(Resources._cancel, 3, 3);
			}
		}
		protected virtual void OnPaintActionView(PaintEventArgs e)
		{
			OnPaintValidIcon(e);
			VPLDrawing.VplDrawing.DrawRoundRectangleShaded(e.Graphics, this.ClientSize, 100, 4, Pens.Blue, penShade, 3);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			if (bLoading)
				return;
			OnPaintActionView(e);
			ShowBreakPoint(e.Graphics);
		}
		protected void ShowBreakPoint(Graphics g)
		{
			Bitmap bmp = BreakPointImage;
			if (bmp != null)
			{
				g.DrawImage(bmp, 2, 2);
			}
		}
		#endregion
		#region IMessageReceiver members
		public bool FireMouseDown(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseDown(e);
			return _messageReturn;
		}
		public bool FireMouseMove(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseMove(e);
			return _messageReturn;
		}
		public bool FireMouseUp(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseUp(e);
			return _messageReturn;
		}
		public bool FireMouseDblClick(MouseButtons button, int x, int y, Keys modifiers)
		{
			MouseEventArgs e = new MouseEventArgs(button, 1, x, y, 0);
			OnMouseDoubleClick(e);
			return _messageReturn;
		}
		public bool FireKeyDown(KeyEventArgs e)
		{
			OnKeyDown(e);
			return _messageReturn;
		}
		public bool FireKeyUp(KeyEventArgs e)
		{
			OnKeyUp(e);
			return _messageReturn;
		}
		#endregion
		#region Event handlers

		void miCreateCompound_Click(object sender, EventArgs e)
		{
			IMathDesigner md = this.Parent as IMathDesigner;
			if (md != null)
			{
				//create A new Actions object an its viewer
				IMathDesigner dv = this.Parent as IMathDesigner;
				if (dv != null)
				{
					dv.CreateCompound(this.Location, this.Size);
				}
			}
		}
		void miCreateMathExpGroup_Click(object sender, EventArgs e)
		{
		}
		void miDelete_Click(object sender, EventArgs e)
		{
			IMathDesigner md = this.Parent as IMathDesigner;
			if (md != null)
			{
				md.DeleteSelectedComponents();
			}
		}
		void miMakeGroup_Click(object sender, EventArgs e)
		{
			MethodDiagramViewer mv = this.Parent as MethodDiagramViewer;
			if (mv != null)
			{
				ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
				if (selectionService != null)
				{
					ICollection ic = selectionService.GetSelectedComponents();
					if (ic != null)
					{
						List<ActionViewer> avs = new List<ActionViewer>();
						foreach (object v in ic)
						{
							ActionViewer av = v as ActionViewer;
							if (av != null)
							{
								avs.Add(av);
							}
						}
						if (avs.Count > 0)
						{
							mv.CreateActionGroup(avs);
						}
					}
				}
			}
		}
		void miMakeList_Click(object sender, EventArgs e)
		{
			MethodDiagramViewer mv = this.Parent as MethodDiagramViewer;
			if (mv != null)
			{
				ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
				if (selectionService != null)
				{
					ICollection ic = selectionService.GetSelectedComponents();
					if (ic != null)
					{
						List<ActionViewer> avs = new List<ActionViewer>();
						foreach (object v in ic)
						{
							ActionViewer av = v as ActionViewer;
							if (av != null)
							{
								avs.Add(av);
							}
						}
						if (avs.Count > 0)
						{
							mv.CreateActionList(avs);
						}
					}
				}
			}
		}
		void miEdit_Click(object sender, EventArgs e)
		{
			FormProgress.ShowProgress("Loading action editor, please wait ...");
#if DEBUG
			MathNode.Trace("Loading action editor");
#endif
			InitializeInput();
#if DEBUG
			MathNode.Trace("End of InitializeInput");
#endif
			//
			OnEditAction();
			FormProgress.HideProgress();
		}
		void miReplace_Click(object sender, EventArgs e)
		{
			OnReplaceAction();
		}
		void mnu_removeBefore(object sender, EventArgs e)
		{
			if (_act != null)
			{
				_act.BreakBeforeExecute = false;
				this.Refresh();
			}
		}
		void mnu_addBefore(object sender, EventArgs e)
		{
			if (_act != null)
			{
				_act.BreakBeforeExecute = true;
				this.Refresh();
			}
		}
		void mnu_removeAfter(object sender, EventArgs e)
		{
			if (_act != null)
			{
				_act.BreakAfterExecute = false;
				this.Refresh();
			}
		}
		void mnu_addAfter(object sender, EventArgs e)
		{
			if (_act != null)
			{
				_act.BreakAfterExecute = true;
				this.Refresh();
			}
		}
		[Browsable(false)]
		protected bool ReadOnly
		{
			get
			{
				MethodDiagramViewer v = this.Parent as MethodDiagramViewer;
				if (v != null)
				{
					if (v.ReadOnly)
					{
						return true;
					}
					if (_act != null)
					{
						return _act.IsReadOnly;
					}
				}
				return true;
			}
		}
		[Browsable(false)]
		protected virtual bool CanReplaceAction { get { return false; } }
		[Browsable(false)]
		protected virtual bool CanEditAction { get { return true; } }
		[Browsable(false)]
		protected virtual bool CanDeleteAction { get { return true; } }
		protected virtual void OnEditAction()
		{
		}
		protected virtual void OnReplaceAction()
		{
		}
		protected virtual void OnCreateContextMenu(ContextMenu cm)
		{
		}
		bool bMouseDown;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			bMouseDown = true;
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (bMouseDown)
			{
				if (e.Button == MouseButtons.Left)
				{
				}
			}
			bMouseDown = false;
			if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
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
							if (v == this)
							{
								selected = true;
								break;
							}
						}
					}
					if (!selected)
					{
						selectionService.SetSelectedComponents(new IComponent[] { this });
						nSelectionCount = 1;
					}
				}
				ContextMenu cm = new ContextMenu();
				MenuItem mi;
				if (!ReadOnly)
				{
					if (nSelectionCount == 1)
					{
						if (CanEditAction)
						{
							mi = new MenuItemWithBitmap("Edit", miEdit_Click, Resources._method.ToBitmap());
							cm.MenuItems.Add(mi);
						}
						////
						if (this.CanReplaceAction)
						{
							mi = new MenuItemWithBitmap("Replace", miReplace_Click, Resources._replace.ToBitmap());
							cm.MenuItems.Add(mi);
						}
						OnCreateContextMenu(cm);
					}

					if (nSelectionCount > 1 && CanDeleteAction)
					{
						mi = new MenuItemWithBitmap("Make action list", miMakeList_Click, Resources._actLst.ToBitmap());
						cm.MenuItems.Add(mi);
						mi = new MenuItemWithBitmap("Make action group", miMakeGroup_Click, Resources._method.ToBitmap());
						cm.MenuItems.Add(mi);
					}
				}
				if (nSelectionCount == 1)
				{
					if (_act != null)
					{
						if (_act.BreakBeforeExecute)
						{
							cm.MenuItems.Add(new MenuItemWithBitmap("Remove break point before action", mnu_removeBefore, Resources._del_breakPoint.ToBitmap()));
						}
						else
						{
							cm.MenuItems.Add(new MenuItemWithBitmap("Add break point before action", mnu_addBefore, Resources._breakpoint.ToBitmap()));
						}
						if (_act.BreakAfterExecute)
						{
							cm.MenuItems.Add(new MenuItemWithBitmap("Remove break point after action", mnu_removeAfter, Resources._del_breakPoint.ToBitmap()));
						}
						else
						{
							cm.MenuItems.Add(new MenuItemWithBitmap("Add break point after action", mnu_addAfter, Resources._breakpoint.ToBitmap()));
						}
					}
				}
				if (!ReadOnly)
				{
					if (nSelectionCount > 0 && CanDeleteAction)
					{
						cm.MenuItems.Add("-");
						//
						mi = new MenuItemWithBitmap("Delete", miDelete_Click, Resources._cancel.ToBitmap());
						cm.MenuItems.Add(mi);
						////
					}
				}
				if (cm.MenuItems.Count > 0)
				{
					cm.Show(this, new Point(e.X, e.Y));
				}
			}
		}
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.KeyData == Keys.Delete)
			{
				miDelete_Click(this, e);
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
		public virtual void OnAddProperties(PropertyDescriptorCollection props)
		{
		}
		protected virtual PropertyDescriptor OnProcessProperty(PropertyDescriptor p)
		{
			return p;
		}
		public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, true);
			PropertyDescriptorCollection newProps = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor oProp in baseProps)
			{
				if (propertyNames.Contains(oProp.Name))
				{
					if (NameReadOnly && string.CompareOrdinal(oProp.Name, "ActionName") == 0)
					{
					}
					else
					{
						if (string.CompareOrdinal(oProp.Name, "ActionParameters") == 0)
						{
							if (ActionParameters != null && ActionParameters.Count > 0)
							{
								newProps.Add(oProp);
							}
						}
						else
						{
							PropertyDescriptor p = OnProcessProperty(oProp);
							if (p != null)
							{
								newProps.Add(p);
							}
						}
					}
				}
			}
			OnAddProperties(newProps);
			return newProps;
		}

		public virtual PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}

		public virtual object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
		#region IActiveDrawing Members
		public UInt32 ActiveDrawingID
		{
			get
			{
				return _act.BranchId;
			}
		}
		public void ResetActiveDrawingID()
		{
			_act.BranchId = (UInt32)Guid.NewGuid().GetHashCode();
		}
		#endregion
	}
}
