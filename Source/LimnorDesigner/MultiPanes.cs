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
using System.Xml;
using XmlSerializer;
using System.Collections;
using VSPrj;
using MathExp;
using XmlUtility;
using LimnorDesigner.Property;
using LimnorDesigner.Event;
using LimnorDesigner.Interface;
using VPL;
using LimnorDesigner.MenuUtil;
using LimnorDesigner.Web;
using Limnor.WebBuilder;
using System.Reflection;
using Limnor.WebServerBuilder;
using System.IO;

namespace LimnorDesigner
{
	public partial class MultiPanes : UserControl, IConfig
	{
		#region fields and constructors
		private const string XMLATT_Max = "max";
		private bool _reading;
		private bool _loaded;
		/// <summary>
		/// for restoring widthes after maximization
		/// </summary>
		private List<int> _viewerWidth;
		private Dictionary<Type, IXDesignerViewer> _viewers;
		public MultiPanes()
		{
			InitializeComponent();
			this.Width = Screen.PrimaryScreen.Bounds.Width;
		}
		#endregion

		#region Programming Utilities
		public void RemoveEventHandlers(IEvent e)
		{
			ClassPointer root = Loader.GetRootId();
			root.RemoveEventHandlers(e);
			Loader.NotifyChanges();
			OnRemoveAllEventHandlers(e);
		}
		public void RemoveEventHandlers(IEvent e, List<TaskID> handlers)
		{
			ClassPointer root = Loader.GetRootId();
			EventAction ea = root.RemoveEventHandlers(e, handlers);
			if (ea != null)
			{
				Loader.NotifyChanges();
				foreach (TaskID tid in handlers)
				{
					HandlerMethodID hid = tid as HandlerMethodID;
					if (hid != null)
					{
						OnDeleteEventMethod(hid.HandlerMethod);
					}
					else
					{
						OnRemoveEventHandler(ea, tid);
					}
				}
			}
		}

		public bool AssignActions(IEvent ep, Form caller)
		{
			DesignUtil.LogIdeProfile("Assign actions to event {0}", ep);
			bool append = true;
			try
			{
				ILimnorDesignerLoader loader = this.Loader;
				ILimnorDesignPane dp = loader.DesignPane;
				if (dp == null)
				{
					throw new DesignerException("ILimnorDesignPane not found for class {0}", loader.GetRootId());// rootId.ClassId);
				}
				DesignUtil.LogIdeProfile("Save all");
				dp.SaveAll();
				ClassPointer a = loader.GetRootId();
				MethodEditContext.IsWebPage = a.IsWebPage;
				MethodEditContext.UseClientExecuterOnly = false;
				MethodEditContext.UseClientPropertyOnly = false;
				if (a.Project.IsWebApplication)
				{
					if (DesignUtil.IsWebClientObject(ep.Owner))
					{
						MethodEditContext.UseServerExecuterOnly = false;
						MethodEditContext.UseServerPropertyOnly = false;
					}
					else
					{
						MethodEditContext.UseServerExecuterOnly = true;
						MethodEditContext.UseServerPropertyOnly = false;
					}
				}
				else
				{
					MethodEditContext.UseServerExecuterOnly = false;
					MethodEditContext.UseServerPropertyOnly = false;
				}
				EventPointer ept = ep as EventPointer;
				if (ept != null)
				{
					EventInfo eif = ept.Info;
					if (eif != null)
					{
						if (WebClientMemberAttribute.IsClientEvent(eif))
						{
							MethodEditContext.UseServerExecuterOnly = false;
							MethodEditContext.UseServerPropertyOnly = false;
						}
						else if (WebServerMemberAttribute.IsServerEvent(eif))
						{
							MethodEditContext.UseServerExecuterOnly = true;
							MethodEditContext.UseServerPropertyOnly = false;
						}
					}
				}
				DesignUtil.LogIdeProfile("Select actions");
				//select action
				List<IAction> actionList = DesignUtil.SelectAction(loader, null, ep, true, null, null, caller);
				MethodEditContext.IsWebPage = false;
				if (actionList != null && actionList.Count > 0)
				{
					if (MethodEditContext.UseServerExecuterOnly)
					{
						foreach (IAction action in actionList)
						{
							if (action.ActionMethod != null && action.ActionMethod.Owner != null)
							{
								if (!DesignUtil.CanBeWebServerObject(action.ActionMethod.Owner))
								{
									MessageBox.Show(caller, "Only server action can be used", "Assign action", MessageBoxButtons.OK, MessageBoxIcon.Error);
									return false;
								}
							}
						}
					}
					EventAction ea = null; //event-actions mapping
					if (append)
					{
						if (a != null)
						{
							List<EventAction> lst = a.EventHandlers;
							if (lst != null)
							{
								foreach (EventAction e in lst)
								{
									if (e.Event.IsSameObjectRef(ep))
									{
										ea = e;
										break;
									}
								}
							}
						}
					}
					if (ea == null)
					{
						ea = new EventAction();
						ea.Event = ep;
					}
					DesignUtil.LogIdeProfile("assign actions in memory");
					foreach (IAction action in actionList)
					{
							ea.AddAction(action);
					}
					DesignUtil.LogIdeProfile("save action assignments");
					loader.GetRootId().SaveEventHandler(ea);
					DesignUtil.LogIdeProfile("Notify designer");
					dp.OnNotifyChanges();
					DesignUtil.LogIdeProfile("finishing assignment");
					dp.OnAssignAction(ea);
					DesignUtil.LogIdeProfile("Finish event-action assignment");
					return true;
				}
			}
			catch (Exception er)
			{
				MathNode.Log(this.FindForm(),er);
			}
			return false;
		}
		#endregion

		#region class ViewerContainer
		class ViewerContainer : SplitContainer
		{
			public event EventHandler MaximizePanel;
			public event EventHandler RestorePanel;
			public event EventHandler SplitterDistanceChange;
			private int _index = 0;
			private int _currentSplitDistance = 0;
			private IXDesignerViewer _viewer;
			private Label lblMax;
			private MultiPanes _panes;
			public ViewerContainer(MultiPanes panes)
			{
				_panes = panes;
				lblMax = new Label();
				lblMax.Text = "";
				lblMax.Name = XMLATT_Max;
				lblMax.Size = new Size(13, 13);
				lblMax.BorderStyle = BorderStyle.None;
				lblMax.Cursor = Cursors.Hand;
				lblMax.Image = Resources.maxbuttonNormal;
				lblMax.Visible = true;
				lblMax.Click += new EventHandler(lblMax_Click);
				lblMax.MouseEnter += new EventHandler(lblMax_MouseEnter);
				lblMax.MouseLeave += new EventHandler(lblMax_MouseLeave);
				Panel1.Resize += new EventHandler(Panel1_Resize);
				Panel1.Controls.Add(lblMax);
				lblMax.Left = 0;
			}
			private bool _maxmized;
			public bool MaxiMized
			{
				get
				{
					return _maxmized;
				}
				set
				{
					_maxmized = value;
					if (_maxmized)
					{
						lblMax.Image = Resources.maxRestoreNormal;
					}
					else
					{
						lblMax.Image = Resources.maxbuttonNormal;
					}
				}
			}
			public virtual bool IsInHtmlEditor { get { return false; } }
			public Control ViewerParent
			{
				get
				{
					if (_viewer != null)
					{
						Control viewerUI = _viewer.GetDesignSurfaceUI();
						if (viewerUI != null)
						{
							Form f = viewerUI.FindForm();
							if (f != null)
							{
								return f;
							}
						}
						return viewerUI;
					}
					return null;
				}
			}
			public MultiPanes MPane
			{
				get
				{
					return _panes;
				}
			}
			public IXDesignerViewer Viewer
			{
				get
				{
					return _viewer;
				}
				set
				{
					_viewer = value;
					Control viewerUI = _viewer.GetDesignSurfaceUI();
					Panel1.Controls.Add(viewerUI);
					viewerUI.Dock = DockStyle.Fill;
					if (_viewer.MaxButtonLocation == EnumMaxButtonLocation.Left)
						lblMax.Left = 0;
					else
						lblMax.Left = Panel1.ClientRectangle.Width - lblMax.Width;
					OnSetViewer();
				}
			}
			public int ViewIndex
			{
				get
				{
					return _index;
				}
				set
				{
					_index = value;
				}
			}
			void Panel1_Resize(object sender, EventArgs e)
			{
				if (_panes.IsReading)
				{
					_currentSplitDistance = this.SplitterDistance;
				}
				else
				{
					if (_currentSplitDistance != this.SplitterDistance)
					{
						_currentSplitDistance = this.SplitterDistance;
						if (!MaxiMized)
						{
							if (SplitterDistanceChange != null)
							{
								SplitterDistanceChange(this, e);
							}
						}
						_panes.AdjustButtonPositions();
					}
				}
			}
			void lblMax_MouseLeave(object sender, EventArgs e)
			{
				if (MaxiMized)
				{
					lblMax.Image = Resources.maxRestoreNormal;
				}
				else
				{
					lblMax.Image = Resources.maxbuttonNormal;
				}
			}

			void lblMax_MouseEnter(object sender, EventArgs e)
			{
				if (MaxiMized)
				{
					lblMax.Image = Resources.maxRestoreHighlight;
				}
				else
				{
					lblMax.Image = Resources.maxbuttonHighlight;
				}
			}
			/// <summary>
			/// maximize/restore the panel
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void lblMax_Click(object sender, EventArgs e)
			{
				if (MaxiMized)
				{
					lblMax.Image = Resources.maxbuttonNormal;
					MaxiMized = false;
					if (RestorePanel != null)
					{
						RestorePanel(this, e);
					}
				}
				else
				{
					lblMax.Image = Resources.maxRestoreNormal;
					MaxiMized = true;
					if (MaximizePanel != null)
					{
						MaximizePanel(this, e);
					}
				}
			}
			public virtual void ToggleHtmlEditing()
			{
			}
			public virtual void OnSetViewer()
			{
			}
			public bool ContainsType(Type t)
			{
				return t.Equals(Viewer.GetType());
			}
			public void HideButton()
			{
				lblMax.Hide();
			}
			public void ShowButton()
			{
				lblMax.Show();
				lblMax.BringToFront();
			}
			public void AdjustButtonPosition()
			{
				if (_viewer != null)
				{
					if (_viewer.MaxButtonLocation == EnumMaxButtonLocation.Left)
						lblMax.Left = 0;
					else
						lblMax.Left = Panel1.ClientRectangle.Width - lblMax.Width;
				}
			}
		}
		#endregion

		#region class ViewerContainerWebPage
		class ViewerContainerWebPage : ViewerContainer
		{
			private Label _lblHtml;
			private IWebHost _htmlEditor;
			private bool _showingHtmlEditor;
			public ViewerContainerWebPage(MultiPanes panes)
				: base(panes)
			{
				_lblHtml = new Label();
				_lblHtml.Text = "";
				_lblHtml.Name = "toggleWebEditor";
				_lblHtml.Size = new Size(32, 13);
				_lblHtml.BorderStyle = BorderStyle.None;
				_lblHtml.Image = Resources.htmlEditor;
				_lblHtml.Cursor = Cursors.Hand;
				_lblHtml.Visible = false;
				_lblHtml.Click += new EventHandler(_lblHtml_Click);
				Panel1.Controls.Add(_lblHtml);
				_lblHtml.Left = 15;
				_lblHtml.Top = 0;
			}
			/// <summary>
			/// toggle html edit mode
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void _lblHtml_Click(object sender, EventArgs e)
			{
				ToggleHtmlEditing();
			}
			public override void ToggleHtmlEditing()
			{
				WebPage wpage = this.MPane.Loader.RootObject as WebPage;
				if (wpage != null)
				{
					Control viewerUI = Viewer.GetDesignSurfaceUI();
					if (_showingHtmlEditor)
					{
						//finish HTML edit mode
						_showingHtmlEditor = false;
						_lblHtml.Image = Resources.htmlEditor;
						wpage.UpdateHtmlFile();
						if (wpage.HtmlChanged)
						{
							wpage.RefreshWebDisplay();
						}
						viewerUI.Visible = true;
						if (wpage.HtmlChangedNoCheck)
						{
							MPane.Loader.DataModified = true;
						}
						if (_htmlEditor != null)
						{
							_htmlEditor.Visible = false;
						}
					}
					else
					{
						//start HTML edit mode
						_showingHtmlEditor = true;
						_lblHtml.Image = Resources.webPageEditor;
						viewerUI.Visible = false;
						if (_htmlEditor != null)
						{
							_htmlEditor.StartEditor();
							_htmlEditor.Visible = true;
						}
					}
				}
				_lblHtml.BringToFront();
				ShowButton();
			}
			public override bool IsInHtmlEditor 
			{ 
				get
				{
					return _showingHtmlEditor;
				}
			}
			public void ShowToggleButton()
			{
#if USEHTMLEDITOR
				_lblHtml.Visible = true;
#endif
			}
			public override void OnSetViewer()
			{
#if USEHTMLEDITOR
				IDesignerFormView df = Viewer as IDesignerFormView;
				if (df != null)
				{
					WebPage wpage = MPane.Loader.RootObject as WebPage;
					if (wpage != null)
					{
						IWebHost whost = WebHost.CreateHtmlHost(MPane.Loader, this.FindForm());
						if (whost != null)
						{
							_htmlEditor = whost;
							Panel1.Controls.Add(_htmlEditor as Control);
							whost.LoadIntoControl();
						}
					}
				}
#else
				_htmlEditor = null;
#endif
			}
		}
		#endregion

		#region Notification
		public IList<IXDesignerViewer> GetDettachedViewers()
		{
			if (_owner != null)
			{
				return _owner.GetDettachedViewers();
			}
			return null;
		}
		public void OnClosing()
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnClosing();
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnClosing();
				}
			}
		}
		public void OnAddExternalType(UInt32 classId, Type t)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnAddExternalType(classId, t);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnAddExternalType(classId, t);
				}
			}
		}
		public void OnRemoveExternalType(UInt32 classId, Type t)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnRemoveExternalType(classId, t);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnRemoveExternalType(classId, t);
				}
			}
		}
		public void AdjustButtonPositions()
		{
			ViewerContainer sc = splitContainer1;
			while (sc != null)
			{
				sc.AdjustButtonPosition();
				sc = getNextContainer(sc);
			}
		}
		public void OnIconChanged(UInt32 classId)
		{
			if (classId == Loader.ClassID)
			{
				//refresh toolbox
				Loader.DesignPane.OnIconChanged();
			}
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnIconChanged(classId);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnIconChanged(classId);
				}
			}
		}
		public void SetClassRefIcon(UInt32 classId, System.Drawing.Image img)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.SetClassRefIcon(classId, img);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.SetClassRefIcon(classId, img);
				}
			}
		}
		public void OnActionSelected(IAction action)
		{
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnActionSelected(action);
				}
				FireEventMethod fe = action.ActionMethod as FireEventMethod;
				if (fe != null)
				{
					OnFireEventActionSelected(fe);
				}
			}
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnActionSelected(action);
				}
			}
		}
		public void OnAssignAction(EventAction ea)
		{
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnAssignAction(ea);
				}
			}
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnAssignAction(ea);
				}
			}
		}
		public void OnRemoveEventHandler(EventAction ea, TaskID task)
		{
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnRemoveEventHandler(ea, task);
				}
			}
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnRemoveEventHandler(ea, task);
				}
			}
		}
		public void OnRemoveAllEventHandlers(IEvent e)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnRemoveAllEventHandlers(e);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnRemoveAllEventHandlers(e);
				}
			}
		}
		public void OnActionListOrderChanged(object sender, EventAction ea)
		{
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnActionListOrderChanged(sender, ea);
				}
			}
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnActionListOrderChanged(sender, ea);
				}
			}
		}
		public void OnActionCreated(UInt32 classId, IAction act, bool isNewAction)
		{
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnActionChanged(classId, act, isNewAction);
				}
			}
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnActionChanged(classId, act, isNewAction);
				}
			}
		}
		public void OnRemoveInterface(InterfacePointer interfacePointer)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnRemoveInterface(interfacePointer);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnRemoveInterface(interfacePointer);
				}
			}
		}
		public void OnInterfaceAdded(InterfacePointer interfacePointer)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnInterfaceAdded(interfacePointer);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnInterfaceAdded(interfacePointer);
				}
			}
		}
		public void OnBaseInterfaceAdded(InterfaceClass owner, InterfacePointer baseInterface)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnBaseInterfaceAdded(owner, baseInterface);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnBaseInterfaceAdded(owner, baseInterface);
				}
			}
		}
		public void OnInterfaceMethodDeleted(InterfaceElementMethod method)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnInterfaceMethodDeleted(method);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnInterfaceMethodDeleted(method);
				}
			}
		}
		public void OnInterfaceMethodChanged(InterfaceElementMethod method)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnInterfaceMethodChanged(method);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnInterfaceMethodChanged(method);
				}
			}
		}
		public void OnInterfaceMethodCreated(InterfaceElementMethod method)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnInterfaceMethodCreated(method);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnInterfaceMethodCreated(method);
				}
			}
		}
		public void OnInterfaceEventAdded(InterfaceElementEvent eventType)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnInterfaceEventAdded(eventType);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnInterfaceEventAdded(eventType);
				}
			}
		}
		public void OnInterfaceEventDeleted(InterfaceElementEvent eventType)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnInterfaceEventDeleted(eventType);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnInterfaceEventDeleted(eventType);
				}
			}
		}
		public void OnInterfaceEventChanged(InterfaceElementEvent eventType)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnInterfaceEventChanged(eventType);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnInterfaceEventChanged(eventType);
				}
			}
		}
		public void OnInterfacePropertyDeleted(InterfaceElementProperty property)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnInterfacePropertyDeleted(property);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnInterfacePropertyDeleted(property);
				}
			}
		}
		public void OnInterfacePropertyAdded(InterfaceElementProperty property)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnInterfacePropertyAdded(property);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnInterfacePropertyAdded(property);
				}
			}
		}
		public void OnInterfacePropertyChanged(InterfaceElementProperty property)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnInterfacePropertyChanged(property);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnInterfacePropertyChanged(property);
				}
			}
		}
		public void OnResetDesigner(object obj)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnResetDesigner(obj);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnResetDesigner(obj);
				}
			}
			Loader.DesignPane.ResetContextMenu();
		}
		public void OnMethodChanged(MethodClass method, bool isNewAction)
		{
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnMethodChanged(method, isNewAction);
				}
			}
			LimnorContextMenuCollection.RemoveMenuCollection(Loader.GetRootId());
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnMethodChanged(method, isNewAction);
				}
			}
		}
		public void OnMethodSelected(MethodClass method)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnMethodSelected(method);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnMethodSelected(method);
				}
			}
		}

		public void OnDeleteMethod(MethodClass method)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnDeleteMethod(method);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnDeleteMethod(method);
				}
			}
			LimnorContextMenuCollection.RemoveMenuCollection(Loader.GetRootId());
		}
		public void OnActionDeleted(IAction action)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnActionDeleted(action);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnActionDeleted(action);
				}
			}
		}
		public void OnDeleteEventMethod(EventHandlerMethod method)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnDeleteEventMethod(method);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnDeleteEventMethod(method);
				}
			}
		}
		public void OnEventSelected(EventClass eventObject)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnEventSelected(eventObject);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnEventSelected(eventObject);
				}
			}
		}
		public void OnFireEventActionSelected(FireEventMethod method)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnFireEventActionSelected(method);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnFireEventActionSelected(method);
				}
			}
		}
		public void OnDeleteProperty(PropertyClass property)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnDeleteProperty(property);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnDeleteProperty(property);
				}
			}
			LimnorContextMenuCollection.RemoveMenuCollection(Loader.GetRootId());
		}
		public void OnResetMap()
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnResetMap();
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnResetMap();
				}
			}
		}
		public void OnDatabaseListChanged()
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnDatabaseListChanged();
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnDatabaseListChanged();
				}
			}
		}
		public void OnDatabaseConnectionNameChanged(Guid connectionid, string newName)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnDatabaseConnectionNameChanged(connectionid, newName);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnDatabaseConnectionNameChanged(connectionid, newName);
				}
			}
		}
		public void OnAddProperty(PropertyClass property)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnAddProperty(property);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnAddProperty(property);
				}
			}
			LimnorContextMenuCollection.RemoveMenuCollection(Loader.GetRootId());
		}

		public void OnPropertySelected(PropertyClass property)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnPropertySelected(property);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnPropertySelected(property);
				}
			}
		}

		public void OnPropertyChanged(INonHostedObject property, string name)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					if (string.CompareOrdinal(name, "Name") == 0 || string.CompareOrdinal(name, "ActionName") == 0)
					{
						v.OnObjectNameChanged(property);
					}
					else
					{
						v.OnPropertyChanged(property, name);
					}
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					if (string.CompareOrdinal(name, "Name") == 0 || string.CompareOrdinal(name, "ActionName") == 0)
					{
						ix.Value.OnObjectNameChanged(property);
					}
					else
					{
						ix.Value.OnPropertyChanged(property, name);
					}
				}
			}
		}
		public void OnDeleteEvent(EventClass eventObject)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnDeleteEvent(eventObject);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnDeleteEvent(eventObject);
				}
			}
			LimnorContextMenuCollection.RemoveMenuCollection(Loader.GetRootId());
		}
		public void OnAddEvent(EventClass eventObject)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnAddEvent(eventObject);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnAddEvent(eventObject);
				}
			}
			LimnorContextMenuCollection.RemoveMenuCollection(Loader.GetRootId());
		}
		public void OnEventListChanged(ICustomEventMethodDescriptor owner, UInt32 objectId)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnEventListChanged(owner, objectId);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnEventListChanged(owner, objectId);
				}
			}
		}

		public PropertyPointer CreatePropertyPointer(object v, string propertyName)
		{
			return Loader.CreatePropertyPointer(v, propertyName);
		}
		public void OnClassLoadedIntoDesigner(UInt32 classId)
		{
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnClassLoadedIntoDesigner(classId);
				}
			}
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnClassLoadedIntoDesigner(classId);
				}
			}
		}
		public void OnDefinitionChanged(UInt32 classId, object relatedObject, EnumClassChangeType changeMade)
		{
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnDefinitionChanged(classId, relatedObject, changeMade);
				}
			}
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnDefinitionChanged(classId, relatedObject, changeMade);
				}
			}
		}
		/// <summary>
		/// selected via HTML editor
		/// </summary>
		/// <param name="element"></param>
		public void OnHtmlElementSelected(HtmlElement_Base element)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnHtmlElementSelected(element);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnHtmlElementSelected(element);
				}
			}
		}
		/// <summary>
		/// selected via IDE
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="selector">designer making the selection</param>
		public void OnSelectedHtmlElement(Guid guid, object selector)
		{
			WebPage wpage = _owner.RootClass.ObjectInstance as WebPage;
			if (wpage != null)
			{
				wpage.SelectHtmlElementByGuid(guid);
			}
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnSelectedHtmlElement(guid, selector);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnSelectedHtmlElement(guid, selector);
				}
			}
		}
		//an html element is used. guid and id should have been created
		public void OnUseHtmlElement(HtmlElement_BodyBase element)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnUseHtmlElement(element);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnUseHtmlElement(element);
				}
			}
		}
		public void OnComponentAdded(object obj)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnComponentAdded(obj);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnComponentAdded(obj);
				}
			}
		}
		public void OnComponentSelected(object obj)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnComponentSelected(obj);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnComponentSelected(obj);
				}
			}
		}
		public void OnComponentRename(object obj, string newName)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnComponentRename(obj, newName);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnComponentRename(obj, newName);
				}
			}
		}
		public void OnComponentRemoved(object obj)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnComponentRemoved(obj);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnComponentRemoved(obj);
				}
			}
			ClassPointer root = Loader.GetRootId();
			root.OnComponentRemoved(obj);
		}
		public void OnObjectNameChanged(INonHostedObject obj)
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnObjectNameChanged(obj);
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> ix in _viewers)
				{
					ix.Value.OnObjectNameChanged(obj);
				}
			}
		}
		public void OnDataLoaded()
		{
			IList<IXDesignerViewer> dvs = GetDettachedViewers();
			if (dvs != null)
			{
				foreach (IXDesignerViewer v in dvs)
				{
					v.OnDataLoaded();
				}
			}
			if (_viewers != null)
			{
				foreach (KeyValuePair<Type, IXDesignerViewer> de in _viewers)
				{
					de.Value.OnDataLoaded();
				}
			}
		}
		#endregion

		#region Properties
		private ILimnorDesignPane _owner;
		public ILimnorDesignPane Owner
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
		public Control ViewerParent
		{
			get
			{
				ViewerContainer vc = getViewer(0);
				if (vc != null)
				{
					return vc.ViewerParent;
				}
				return null;
			}
		}
		public bool IsReSizing
		{
			get
			{
				return _resizing;
			}
		}
		public ILimnorDesignerLoader Loader { get; set; }

		public ObjectIDmap ObjectIDList
		{
			get
			{
				return Loader.ObjectMap;
			}
		}
		public bool IsReading
		{
			get
			{
				return _reading;
			}
		}
		public bool Loaded
		{
			get
			{
				return _loaded;
			}
		}
		#endregion

		#region Methods
		public void ToggleHtmlEditing()
		{
			ViewerContainer start = splitContainer1;
			while (start != null)
			{
				start.ToggleHtmlEditing();
				start = getNextContainer(start);
			}
		}
		public bool IsInHtmlEditor()
		{
			ViewerContainer start = splitContainer1;
			while (start != null)
			{
				if (start.IsInHtmlEditor)
				{
					return true;
				}
				start = getNextContainer(start);
			}
			return false;
		}
		private bool _resizing;
		protected override void OnResize(EventArgs e)
		{
			_resizing = true;
			base.OnResize(e);
			_resizing = false;
		}
		public SplitContainer GetContainerByType(Type t)
		{
			ViewerContainer start = splitContainer1;
			while (start != null)
			{
				if (start.ContainsType(t))
				{
					return start;
				}
				start = getNextContainer(start);
			}
			return null;
		}
		public IXDesignerViewer GetDesignerByType(Type t)
		{
			ViewerContainer vc = GetContainerByType(t) as ViewerContainer;
			if (vc != null)
			{
				return vc.Viewer;
			}
			return null;
		}
		public T GetDesigner<T>()
		{
			ViewerContainer vc = GetContainerByType(typeof(T)) as ViewerContainer;
			if (vc != null)
			{
				return (T)(vc.Viewer);
			}
			return default(T);
		}
		public void HideContainerByType(Type t)
		{
			ViewerContainer start = splitContainer1;
			while (start != null)
			{
				if (start.ContainsType(t))
				{
					start.Panel1Collapsed = true;
					if (_viewers != null)
					{
						if (_viewers.Count == 2)
						{
							start = splitContainer1;
							while (start != null)
							{
								start.HideButton();
								start = getNextContainer(start);
							}
						}
					}
					break;
				}
				start = getNextContainer(start);
			}
		}
		public ClassPointer GetRootId()
		{
			return Loader.GetRootId();
		}

		public void ApplyConfig()
		{
			if (Loader != null && Loader.Node != null)
			{
				XmlAttribute xa = Loader.Node.Attributes["width0"];
				if (xa == null)
				{
					MaximizeLastPane();
				}
				else
				{
					ReadConfig(Loader.Node);
				}
				ViewerContainer sc = splitContainer1;
				while (sc != null)
				{
					sc.AdjustButtonPosition();
					sc = getNextContainer(sc);
				}
				SetLoaded();
			}
		}
		public void SetLoaded()
		{
			ViewerContainer sc = splitContainer1;
			while (sc != null)
			{
				sc.Viewer.OnUIConfigLoaded();
				sc = getNextContainer(sc);
			}
			_loaded = true;
		}
		public void RemoveViewer(IXDesignerViewer viewer)
		{
			Type t = viewer.GetType();
			if (_viewers.ContainsKey(t))
			{
				ViewerContainer sc = splitContainer1;
				while (sc != null)
				{
					if (t.Equals(sc.Viewer.GetType()))
					{
						//remove viewer from sc.Panel1
						sc.Panel1.Controls.Remove(viewer.GetDesignSurfaceUI());
						//move other viewers forward
						ViewerContainer nextV = null;
						foreach (Control c in sc.Panel2.Controls)
						{
							if (c is ViewerContainer)
							{
								nextV = c as ViewerContainer;
								break;
							}
						}
						if (nextV != null)
						{
							Control p = sc.Parent;
							p.Controls.Remove(sc);
							p.Controls.Add(nextV);
						}
						break;
					}
					sc = getNextContainer(sc);
				}
				_viewers.Remove(t);
			}
		}
		public void AddViewer(IXDesignerViewer viewer)
		{
			Type t = viewer.GetType();
			bool bFirst = false;
			if (_viewers == null)
			{
				bFirst = true;
				_viewers = new Dictionary<Type, IXDesignerViewer>();
			}
			if (!_viewers.ContainsKey(t))
			{
				viewer.SetDesigner(this);//it must be called before GetDesignSurfaceUI
				_viewers.Add(t, viewer);
				ViewerContainer sc = getLastContainer(null);
				ViewerContainer scNew = null;
				if (bFirst)
				{
					scNew = sc;
					if (_owner.Loader.RootObject is WebPage && viewer is IDesignerFormView)
					{
						ViewerContainerWebPage vcw = scNew as ViewerContainerWebPage;
						vcw.ShowToggleButton();
					}
				}
				else
				{
					if (_owner.Loader.RootObject is WebPage && viewer is IDesignerFormView)
					{
						scNew = new ViewerContainerWebPage(this);
					}
					else
					{
						scNew = new ViewerContainer(this);
					}
					scNew.Dock = DockStyle.Fill;
					sc.Panel2.Controls.Add(scNew);
					scNew.ViewIndex = sc.ViewIndex + 1;
					sc.Panel2Collapsed = false;
					scNew.Panel2Collapsed = true;
				}
				scNew.Viewer = viewer;//viewer.GetDesignSurfaceUI is added to scNew.Panel1
				scNew.MaximizePanel += new EventHandler(lblMax_Maximize);
				scNew.RestorePanel += new EventHandler(lblMax_Restore);
				scNew.SplitterDistanceChange += new EventHandler(scNew_SplitterDistanceChange);
				scNew.SizeChanged += new EventHandler(scNew_SizeChanged);
			}
		}
		public void SaveWidths()
		{
			ViewerContainer sc = splitContainer1;
			while (sc != null)
			{
				saveWidth(sc.ViewIndex, sc.SplitterDistance);
				sc = getNextContainer(sc);
			}
		}

		public void MaximizeLastPane()
		{
			ViewerContainer sc = getLastContainer(null);
			sc.Panel2MinSize = 1;
			int nNewSize = sc.ClientRectangle.Width - 1;
			if (nNewSize < sc.Panel1MinSize)
				nNewSize = sc.Panel1MinSize;
			else if (nNewSize > sc.ClientRectangle.Width - sc.Panel2MinSize)
				nNewSize = sc.ClientRectangle.Width - sc.Panel2MinSize;
			if (nNewSize >= sc.Panel1MinSize && nNewSize <= sc.ClientRectangle.Width - sc.Panel2MinSize)
			{
				sc.SplitterDistance = nNewSize;
			}
		}
		public void WriteConfig(XmlNode node)
		{
			ViewerContainer scMax = getMaximizedContainer();
			if (scMax != null)
			{
				XmlUtil.SetAttribute(node, XMLATT_Max, scMax.ViewIndex);
			}
			else
			{
				XmlAttribute xa = node.Attributes[XMLATT_Max];
				if (xa != null)
				{
					node.Attributes.Remove(xa);
				}
			}
			ViewerContainer sc = splitContainer1;
			while (sc != null)
			{
				XmlUtil.SetAttribute(node, "width" + sc.ViewIndex.ToString(), getWidth(sc.ViewIndex));
				sc = getNextContainer(sc);
			}
		}
		public void ReadConfig(XmlNode node)
		{
			_reading = true;
			if (XmlUtil.HasAttribute(node, "width0"))
			{
				ViewerContainer sc = splitContainer1;
				while (sc != null)
				{
					int n = XmlUtil.GetAttributeInt(node, "width" + sc.ViewIndex.ToString());
					if (n == 0)
						break;
					try
					{
						sc.SplitterDistance = n;
						saveWidth(sc.ViewIndex, n);
					}
					catch
					{
					}
					sc = getNextContainer(sc);
				}
				XmlAttribute xaMax = node.Attributes[XMLATT_Max];
				if (xaMax != null && !string.IsNullOrEmpty(xaMax.Value))
				{
					try
					{
						int nMax = int.Parse(xaMax.Value);
						maximizeContainer(nMax);
					}
					catch
					{
					}
				}
			}
			_configChanged = false;
			_reading = false;
		}
		#endregion

		#region private methods
		void scNew_SizeChanged(object sender, EventArgs e)
		{
			AdjustButtonPositions();
		}

		void scNew_SplitterDistanceChange(object sender, EventArgs e)
		{
			if (!_reading && _loaded && !_resizing)
			{
				if (Loader.IsInDesign)
				{
					ViewerContainer sc = sender as ViewerContainer;
					saveWidth(sc.ViewIndex, sc.SplitterDistance);
					WriteConfig(Loader.Node);
					Loader.NotifyChanges();
					_configChanged = true;
				}
			}
		}

		/// <summary>
		/// maximize/restore the panel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void lblMax_Restore(object sender, EventArgs e)
		{
			restoreContainerWidths();
		}
		void lblMax_Maximize(object sender, EventArgs e)
		{
			ViewerContainer sc = sender as ViewerContainer;
			maximizeContainer(sc.ViewIndex);
		}

		private ViewerContainer getNextContainer(ViewerContainer currentContainer)
		{
			if (currentContainer == null)
				return splitContainer1;
			for (int i = 0; i < currentContainer.Panel2.Controls.Count; i++)
			{
				if (currentContainer.Panel2.Controls[i] is ViewerContainer)
					return (ViewerContainer)currentContainer.Panel2.Controls[i];
			}
			return null;
		}
		private ViewerContainer getLastContainer(ViewerContainer start)
		{
			if (start == null)
				start = splitContainer1;
			for (int i = 0; i < start.Panel2.Controls.Count; i++)
			{
				if (start.Panel2.Controls[i] is ViewerContainer)
				{
					return getLastContainer((ViewerContainer)start.Panel2.Controls[i]);
				}
			}
			return start;
		}
		private void saveWidth(int i, int width)
		{
			if (_viewerWidth == null)
			{
				_viewerWidth = new List<int>();
			}
			while (_viewerWidth.Count <= i)
			{
				_viewerWidth.Add(30);
			}
			_viewerWidth[i] = width;
		}
		private int getWidth(int i)
		{
			if (_viewerWidth != null)
			{
				if (_viewerWidth.Count > i)
				{
					return _viewerWidth[i];
				}
			}
			ViewerContainer sc = getViewer(i);
			if (sc != null)
			{
				return sc.SplitterDistance;
			}
			return 60;
		}

		private void maximizeContainer(int index)
		{
			ViewerContainer sc = splitContainer1;
			while (sc != null)
			{
				if (sc.ViewIndex == index)
				{
					sc.SplitterDistance = this.ClientSize.Width;
					sc.MaxiMized = true;
				}
				else
				{
					int p2 = sc.Width - sc.Panel2MinSize;
					if (sc.Panel1MinSize <= p2)
					{
						sc.SplitterDistance = sc.Panel1MinSize;
					}
					sc.MaxiMized = false;
				}
				sc = getNextContainer(sc);
			}
			//AdjustButtonPositions();
		}

		private void restoreContainerWidths()
		{
			int i = 0;
			ViewerContainer scLast = splitContainer1;
			ViewerContainer sc = splitContainer1;
			while (sc != null)
			{
				scLast = sc;
				sc.Panel2Collapsed = false;
				int n = getWidth(i);
				if (n >= sc.Panel1MinSize && n <= sc.Width - sc.Panel2MinSize)
				{
					sc.SplitterDistance = n;
				}
				sc = getNextContainer(sc);
				i++;
			}
			scLast.Panel2Collapsed = true;
		}
		private ViewerContainer getViewer(int i)
		{
			ViewerContainer sc = splitContainer1;
			while (sc != null)
			{
				if (sc.ViewIndex == i)
					return sc;
				sc = getNextContainer(sc);
			}
			return null;
		}
		private ViewerContainer getMaximizedContainer()
		{
			ViewerContainer sc = splitContainer1;
			while (sc != null)
			{
				if (sc.MaxiMized)
				{
					return sc;
				}
				sc = getNextContainer(sc);
			}
			return null;
		}
		#endregion

		#region IConfig Members

		private bool _configChanged;
		public bool ConfigChanged
		{
			get
			{
				return _configChanged;
			}
			set
			{
				_configChanged = value;
			}
		}

		#endregion
	}
}
