/*
 * Author: Bob Limnor (info@limnor.com)
 * Product: Limnor Studio
 * 
 * Description: A Windows Form class supporting highly efficient 2-dimensional drawing elements.
 * A control in a Windows Form takes considerable resources. Therefore it is not efficient to use a control to represent a drawing element,
 * because a drawing might comprise a lot of drawing elements.
 * To solve the problem, the project builds a system supporting highly efficient drawing elements.
 * Such drawing elements draw quickly and take little resources, while they are easy to use as controls.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Collections;
using System.Xml;
using System.Drawing.Printing;
using VPL;
using System.Threading;
using XmlUtility;
using System.Security.Cryptography;
using System.Diagnostics;
using ITouch;

namespace Limnor.Drawing2D
{
	[UseDefaultInstance(DrawingPage.DEFAULTFORM)]
	[Description("This is a Form for making it easier to do 2D drawing")]
	public partial class DrawingPage : Form, IDrawingPage, IClassInstance, IComponentContainer, IForm, IRuntimeExecuter, IDrawingHolder, IDrawDesignControlParent
	{
		#region fields and constructors
		private PageAttrs _pagetAttrs;
		private bool _continuousPaint;
		private Cursor _cursor;
		private bool _drawingOrderSorted;
		private DrawingLayerCollection _layers;
		private Guid _activeLayerId = Guid.Empty;
		private DrawingLayerHeaderList _headers;
		private DrawingItem _currentSelectedDrawing;
		private DrawingItem _currentDrawingUnderMouse;
		public IDrawingOwnerPointer DrawingOwnerDesinTimePointer; //it is a ClassPointer
		public EventHandler NotifySelectionChange; //not an event because we do not want to maintain multiple notifications
		private Bitmap _memoryImage;
		private Label _bottomControl;
		private bool _disableRotation;
		private int _activityCount;
		private bool _watchActivity;
		private LogonUser _loggedonuser;
		private bool _suspending=true;
		//
		private Guid _loginGuid = Guid.Empty;
		//
		//fading
		private int _showFadeTime;
		private bool _loaded;
		private int _closeFadeTime;
		//
		public const string DEFAULTFORM = "DefaultForm";
		//
		private System.Threading.Timer _tm;
		//
		public DrawingPage()
		{
			InitializeComponent();
			_layers = new DrawingLayerCollection();
			_pagetAttrs = new PageAttrs();
			AutoNextEntry = false;
			ReadOnly = false;
			//
			_cursor = Cursor;
			//
			
		}
		static DrawingPage()
		{
			initStatic();
		}
		#endregion
		#region Protected and Override Methods
		protected virtual void OnComponentsInitialized()
		{
			if (BeforeLoad != null)
			{
				BeforeLoad(this, EventArgs.Empty);
			}
		}
		protected override void SetVisibleCore(bool value)
		{
			if (this.Site != null && this.Site.DesignMode)
			{
				base.SetVisibleCore(value);
			}
			else
			{
				base.SetVisibleCore(InitiallyHidden ? false : value);
			}
		}
		protected override void OnControlRemoved(ControlEventArgs e)
		{
			base.OnControlRemoved(e);
			IDrawDesignControl ddc = e.Control as IDrawDesignControl;
			if (ddc != null)
			{
				if (_layers != null)
				{
					_layers.RemoveDrawing(ddc.Item);
				}
			}
		}
		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);
			IDrawDesignControl ddc = e.Control as IDrawDesignControl;
			if (ddc != null)
			{
				if (_layers != null)
				{
					_layers.AddDrawing(ddc.Item);
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		protected void SetLoginManagerID(Guid id)
		{
			_loginGuid = id;
		}
		
		[Description("It is called on successfully calling LoadDrawingsFromFile")]
		protected virtual void OnLoadedFromFile(string filename)
		{
		}
		[Description("It is called on successfully calling SaveDrawingsToFile")]
		protected virtual void OnSavedToFile(string filename)
		{
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (_layers != null)
			{
				if (_layers.OnParentSizeChanged())
				{
					this.Invalidate();
				}
			}
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (_inputBox != null && _inputBox.Visible)
			{
				_inputBox.Finish();
			}
			base.OnMouseDown(e);
			if (_watchActivity)
			{
				_activityCount = 0;
				if (_loggedonuser != null)
				{
					_loggedonuser.ResetCount();
				}
			}
			DrawingItem selectedDrawing = HitTest(new Point(e.X, e.Y));
			if (selectedDrawing != null)
			{
				if (selectedDrawing.Container != null)
				{
					selectedDrawing.Container.CurrentItem = selectedDrawing;
				}
				selectedDrawing._OnMouseDown(e);
				if (_currentSelectedDrawing != selectedDrawing)
				{
					_currentSelectedDrawing = selectedDrawing;
					if (NotifySelectionChange != null)
					{
						NotifySelectionChange(_currentSelectedDrawing, EventArgs.Empty);
					}
				}
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (_watchActivity)
			{
				_activityCount = 0;
				if (_loggedonuser != null)
				{
					_loggedonuser.ResetCount();
				}
			}
			DrawingItem selectedDrawing = HitTest(new Point(e.X, e.Y));
			if (selectedDrawing != null)
			{
				if (selectedDrawing.Container != null)
				{
					selectedDrawing.Container.CurrentItem = selectedDrawing;
				}
				selectedDrawing._OnMouseUp(e);
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (_watchActivity)
			{
				_activityCount++;
				if (_activityCount > 3)
				{
					_activityCount = 0;
					if (_loggedonuser != null)
					{
						_loggedonuser.ResetCount();
					}
				}
			}
			DrawingItem selectedDrawing = HitTest(new Point(e.X, e.Y));
			if (selectedDrawing != null)
			{
				if (selectedDrawing.Container != null)
				{
					selectedDrawing.Container.CurrentItem = selectedDrawing;
				}
				if (_currentDrawingUnderMouse != null)
				{
					if (_currentDrawingUnderMouse != selectedDrawing)
					{
						_currentDrawingUnderMouse._OnMouseLeave(EventArgs.Empty);
					}
				}
				_currentDrawingUnderMouse = selectedDrawing;
				Cursor = selectedDrawing.Cursor;
				selectedDrawing._OnMouseMove(e);
			}
			else
			{
				Cursor = _cursor;
				if (_currentDrawingUnderMouse != null)
				{
					_currentDrawingUnderMouse._OnMouseLeave(EventArgs.Empty);
					_currentDrawingUnderMouse = null;
				}
			}
		}
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (_watchActivity)
			{
				_activityCount = 0;
				if (_loggedonuser != null)
				{
					_loggedonuser.ResetCount();
				}
			}
		}
		protected override void OnClosed(EventArgs e)
		{
			stopContinuousPaint();
			base.OnClosed(e);
		}
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
			if (e.Cancel == false)
			{
				if (_loginGuid != Guid.Empty)
				{
					LogonUser.RemoveForm(_loginGuid, this);
				}
				if (_closeFadeTime > 0)
				{
					NativeWIN32.ClodeFading(this.Handle, _closeFadeTime);
				}
			}
		}
		protected virtual void OnShowUserLogonDialog()
		{
		}
		protected override void OnLoad(EventArgs e)
		{
			if (_loginGuid != Guid.Empty)
			{
				if (!this.UserLoggedOn)
				{
					OnShowUserLogonDialog();
				}
				LogonUser u = LogonUser.GetLogonUser(_loginGuid);
				if (u != null)
				{
					if (this.UserLevel >= 0 && u.UserLevel >= 0)
					{
						if (u.UserLevel > this.UserLevel)
						{
							if (UserPermissionDenied != null)
							{
								UserPermissionDenied(this, EventArgs.Empty);
							}
							else
							{
								MessageBox.Show(this, "Permission denied", "Log in", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
							Close();
							return;
						}
					}
				}
				else
				{
					if (UserLogonFailed != null)
					{
						UserLogonFailed(this, EventArgs.Empty);
					}
					else
					{
						MessageBox.Show(this, "Invalid user name or password", "Log in", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					Close();
					return;
				}
				LogonUser.AddOpenedForm(_loginGuid, this);
				if (u.InactivityMinutes > 0)
				{
					_activityCount = 0;
					_watchActivity = true;
					_loggedonuser = u;
				}
			}
			if (_showFadeTime > 0)
			{
				NativeWIN32.ShowFading(this.Handle, _showFadeTime);
			}
			if (_pagetAttrs == null)
			{
				_pagetAttrs = new PageAttrs();
				Graphics g = this.CreateGraphics();
				_pagetAttrs.SetDPI(g.DpiX, g.DpiY);
			}
			base.OnLoad(e);
			_loaded = true;
		}
		protected virtual void OnPrepareNextPaint(PaintEventArgs e)
		{
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			bool inDesign;
			if (_continuousPaint)
			{
				OnPrepareNextPaint(e);
			}
			GraphicsState gs = null;
			if (this.AutoScrollPosition.X != 0 || this.AutoScrollPosition.Y != 0)
			{
				gs = e.Graphics.Save();
				e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
			}
			inDesign = (Site != null && Site.DesignMode);
			if (inDesign)
			{
				VerifyDrawingOrder();
			}
			int nBottom = _layers.Draw(e.Graphics);
			if (gs != null)
			{
				e.Graphics.Restore(gs);
			}
			base.OnPaint(e);
			if (_continuousPaint)
			{
				if (_drawingStep != null)
				{
					_drawingStep.Set();
				}
			}
			if (!inDesign)
			{
				if (this.AutoScroll)
				{
					if (_bottomControl == null)
					{
						_bottomControl = new Label();
						_bottomControl.Parent = this;
						_bottomControl.Text = "";
					}
					_bottomControl.Location = new Point(0, nBottom);
				}
			}
		}
		[Description("Add types to be serialized")]
		protected virtual void OnGetSerializationTypes(List<Type> types)
		{
		}
		#endregion
		#region Methods
		public void ClearDrawings()
		{
			_layers = new DrawingLayerCollection();
			_layers.AddLayer();
			this.Refresh();
		}
		public void RefreshDrawingOrder()
		{
			_drawingOrderSorted = false;
			VerifyDrawingOrder();
		}
		public new void SuspendLayout()
		{
			_suspending = true;
			base.SuspendLayout();
		}
		public new void ResumeLayout(bool resume)
		{
			_suspending = false;
			base.ResumeLayout(resume);
			VerifyDrawingOrder();
		}
		public bool IsSuspended()
		{
			return _suspending;
		}
		public void VerifyDrawingOrder()
		{
			if (!_drawingOrderSorted)
			{
				_drawingOrderSorted = true;
				if (this.InDesignMode)
				{
					SortedDictionary<int, List<IDrawDesignControl>> sortedItems = new SortedDictionary<int, List<IDrawDesignControl>>();
					for (int i = 0; i < Controls.Count; i++)
					{
						IDrawDesignControl dc = Controls[i] as IDrawDesignControl;
						if (dc != null)
						{
							List<IDrawDesignControl> lst;
							if (!sortedItems.TryGetValue(dc.ZOrder, out lst))
							{
								lst = new List<IDrawDesignControl>();
								sortedItems.Add(dc.ZOrder, lst);
							}
							lst.Add(dc);
						}
					}
					_layers.ClearDrawings();
					SortedDictionary<int, List<IDrawDesignControl>>.Enumerator en = sortedItems.GetEnumerator();
					while (en.MoveNext())
					{
						foreach (IDrawDesignControl dc in en.Current.Value)
						{
							_layers.AddDrawing(dc.Item);
							IDrawingHolder h = dc.Item as IDrawingHolder;
							if (h != null)
							{
								h.RefreshDrawingOrder();
							}
							Control c = dc as Control;
							if (c != null)
							{
								c.BringToFront();
							}
						}
					}
				}
				else
				{
					_layers.VerifyItemOrders();
				}
				this.Invalidate();
			}
		}
		public List<DrawingItem> ExportDrawingItems()
		{
			List<DrawingItem> lst = new List<DrawingItem>();
			if (_layers != null)
			{
				foreach (DrawingLayer l in _layers)
				{
					lst.AddRange(l);
				}
			}
			return lst;
		}
		[Description("Make drawings in a drawing layer visible or hidden")]
		public void ShowLayerByName(string name, bool show)
		{
			DrawingLayer l = _layers.GetLayerByName(name);
			if (l != null)
			{
				l.Visible = show;
				this.Refresh();
			}
		}
		[Description("Add a new drawing to the drawing layer specified by the drawing's LayerId property. If the LayerId property does not match an existing layer in the page then the drawing is added to the first layer")]
		public void AddDrawing(DrawingItem draw)
		{
			draw.Page = this;
			_layers.AddDrawing(draw);
			draw.Initialize();
			this.Refresh();
		}
		[Description("Remove a drawing")]
		public void RemoveDrawing(DrawingItem draw)
		{
			_layers.RemoveDrawing(draw);
		}
		[Description("Find a drawing layer by its guid")]
		public DrawingLayer GetLayerById(Guid id)
		{
			return _layers.GetLayerById(id);
		}
		[Description("Find a drawing layer by its name")]
		public DrawingLayer GetLayerByName(string name)
		{
			return _layers.GetLayerByName(name);
		}
		[Description("Find a drawing by its guid")]
		public DrawingItem GetDrawingItemById(Guid id)
		{
			foreach (DrawingLayer l in _layers)
			{
				foreach (DrawingItem item in l)
				{
					if (item.DrawingId == id)
					{
						return item;
					}
				}
			}
			return null;
		}


		[Description("Save the drawings contained in this page to an XML file. LoadDrawingsFromFile method can be used to load the drawings from the file")]
		public void SaveDrawingsToFile(string filename)
		{
			saveDrawingsToXmlFile(filename, false);
		}
		[Description("Save the drawings contained in this page to an XML file. LoadDrawingsFromFile method can be used to load the drawings from the file")]
		public void SaveDrawingsToFileWithPageSize(string filename)
		{
			saveDrawingsToXmlFile(filename, true);

		}
		[Description("Load drawings from an XML file. Usually the file is generated by SaveDrawingsToFile/SaveDrawingsToFileWithPageSize methods")]
		public void LoadDrawingsFromFile(string filename)
		{
			DrawingLayers.ClearDrawings();
			try
			{
				List<Type> types = new List<Type>();
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);
				XmlNodeList typeNodes = doc.DocumentElement.SelectNodes("Types/Type");
				if (typeNodes != null)
				{
					foreach (XmlNode nd in typeNodes)
					{
						types.Add(Type.GetType(nd.InnerText));
					}
				}
				XmlNode psnode = doc.DocumentElement.SelectSingleNode("PageSize");
				if (psnode != null)
				{
					string sv = GetAttribute(psnode, "Size");
					if (!string.IsNullOrEmpty(sv))
					{
						_pagetAttrs.PageSize = (PaperKind)Enum.Parse(typeof(PaperKind), sv);
					}
					sv = GetAttribute(psnode, "width");
					if (!string.IsNullOrEmpty(sv))
					{
						double w;
						if (double.TryParse(sv, out w))
						{
							sv = GetAttribute(psnode, "height");
							if (!string.IsNullOrEmpty(sv))
							{
								double h;
								if (double.TryParse(sv, out h))
								{
									_pagetAttrs.SetPageSizeMM(w, h);
								}
							}
						}
					}
				}
				OnGetSerializationTypes(types);
				TextReader r = new StreamReader(filename);
				XmlSerializer s = new XmlSerializer(XmlSerializarionObject.GetType(), types.ToArray());
				XmlSerializarionObject = s.Deserialize(r);
				r.Close();
				_layers.SetDrawingPage(this);
				this.Refresh();
				OnLoadedFromFile(filename);
			}
			catch (Exception e)
			{
				DlgMessage.ShowException(this, e);
			}
		}
		[Description("Launch drawing editor to modify the drawings. It returns false if the editing is canceled.")]
		public bool EditDrawings()
		{
			DrawingLayerCollection layers = ShowDrawingsEditor();
			if (layers != null)
			{
				_layers = layers;
				_layers.SetDrawingPage(this);
				this.Refresh();


				return true;
			}
			return false;
		}
		[Description("Print this form as it appears on the screen")]
		public void PrintFormSnapshot(string documentName, bool preview)
		{
			PrintDocument pdoc = new PrintDocument();
			pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);
			pdoc.DocumentName = documentName;

			captureScreen();
			if (preview)
			{
				PrintPreviewDialog dlg = new PrintPreviewDialogEx();
				dlg.Document = pdoc;
				dlg.ShowDialog(this);
			}
			else
			{
				pdoc.Print();
			}
		}
		[Description("Returns a bitmap image created by all drawings on the page. If a filename is provided then the image is saved to a file specified by filename and format. If xDpi and yDpi are greater than 0 then the image is created using xDpi and yDpi as x-resolution and y-resolution in Dot-Per-Inch. Screen resolution usually is 96 DPI. Printer resolutions usually are higher, for example, 300 DPI.")]
		public Bitmap CreateImageFileFromDrawings(string filename,float xDpi, float yDpi, EnumImageFormat format)
		{
			if (_layers != null)
			{
				Size sz = _layers.GetSize();
				if (sz.Width > 0 && sz.Height > 0)
				{
					Bitmap bmp = new Bitmap(sz.Width, sz.Height);
					if (xDpi > 0 && yDpi > 0)
					{
						bmp.SetResolution(xDpi, yDpi);
					}
					Graphics g = Graphics.FromImage(bmp);
					Brush bk = new SolidBrush(this.BackColor);
					g.FillRectangle(bk, new Rectangle(0, 0, sz.Width, sz.Height));
					_layers.Draw(g);
					if (!string.IsNullOrEmpty(filename))
					{
						bmp.Save(filename, VPLUtil.GetImageFormat(format));
					}
					return bmp;
				}
			}
			return null;
		}
		[Description("Print drawing layers. Specify the drawing layers using semi-column seperated layer names")]
		public void PrintDrawingLayers(string layerNames, string documentName, bool preview, bool landscape)
		{
			PrintDocumentDraw printDrawingLayer = new PrintDocumentDraw();
			printDrawingLayer.PrintPage += new PrintPageEventHandler(printDrawingLayer_PrintPage);
			printDrawingLayer.DocumentName = documentName;
			printDrawingLayer.DefaultPageSettings.Landscape = landscape;
			if (layerNames != null)
			{
				layerNames = layerNames.Trim();
			}
			if (string.IsNullOrEmpty(layerNames))
			{
				StringBuilder sb = new StringBuilder();
				if (_layers.Count > 0)
				{
					sb.Append(_layers[0].LayerName);
					if (_layers.Count > 1)
					{
						for (int i = 1; i < _layers.Count; i++)
						{
							sb.Append(";");
							sb.Append(_layers[i].LayerName);
						}
					}
				}
				layerNames = sb.ToString();
			}
			printDrawingLayer.DrawingLayerName = layerNames;
			_pagetAttrs.SetDocumentPageAttributes(printDrawingLayer);
			if (preview)
			{
				PrintPreviewDialog dlg = new PrintPreviewDialogEx();
				dlg.Document = printDrawingLayer;
				dlg.ShowDialog(this);
			}
			else
			{
				printDrawingLayer.Print();
			}
		}
		[Description("Print all drawings")]
		public void PrintDrawings(string documentName, bool preview)
		{
			PrintDocument pdocDrawings = new PrintDocument();
			pdocDrawings.PrintPage += new PrintPageEventHandler(pdocDrawings_PrintPage);
			pdocDrawings.DocumentName = documentName;
			_pagetAttrs.SetDocumentPageAttributes(pdocDrawings);
			if (preview)
			{
				PrintPreviewDialog dlg = new PrintPreviewDialogEx();
				dlg.Document = pdocDrawings;
				dlg.ShowDialog(this);
			}
			else
			{
				pdocDrawings.Print();
			}
		}
		[Description("If this form was displayed using ShowPreviousForm from another form then calling this method will display that form. ")]
		public Form ShowNextForm()
		{
			return FormNavigator.NextForm(this);
		}
		[Description("If this form was displayed by a Show action from another form then calling this method will display that form. ")]
		public Form ShowPreviousForm()
		{
			return FormNavigator.PreviousForm(this);
		}
		[Description("Get image of a rectangle of this form.")]
		public Bitmap GetFormImage(Rectangle range)
		{
			Bitmap img = new Bitmap(range.Width, range.Height);
			Bitmap img0 = new Bitmap(this.Width, this.Height);
			this.DrawToBitmap(img0, new Rectangle(0, 0, img0.Width, img0.Height));
			Graphics g = Graphics.FromImage(img);
			g.DrawImage(img0, new Rectangle(0, 0, img.Width, img.Height), range, GraphicsUnit.Pixel);
			g.Dispose();
			return img;
		}
		public new void Show(IWin32Window owner)
		{
			ShowOwned(owner);
		}
		[Description("This method replaces the original method, Show(owner), from the .Net Framework. It will not generate exception if the form is already visible.")]
		public void ShowOwned(IWin32Window owner)
		{
			try
			{
				if (this.Visible)
				{
					this.BringToFront();
				}
				base.Show(owner);
			}
			catch (InvalidOperationException)
			{
			}
		}
		[Description("Show the form with a fade-in effect. showFadeTime is the fading time, in milliseconds. Try use 500 for showFadeTime to see if it works for you. closeFadeTime is the fade time for fade-out effect.")]
		public void ShowWithFading(int showFadeTime, int closeFadeTime)
		{
			_showFadeTime = showFadeTime;
			_closeFadeTime = closeFadeTime;
			if (_loaded)
			{
				NativeWIN32.ShowFading(this.Handle, _showFadeTime);
			}
			Show();
		}
		[Description("Show the form with a fade-in effect. showFadeTime is the fading time, in milliseconds. Try use 500 for showFadeTime to see if it works for you. closeFadeTime is the fade time for fade-out effect.")]
		public void ShowWithFading(IWin32Window owner, int showFadeTime, int closeFadeTime)
		{
			_showFadeTime = showFadeTime;
			_closeFadeTime = closeFadeTime;
			if (_loaded)
			{
				NativeWIN32.ShowFading(this.Handle, _showFadeTime);
			}
			ShowOwned(owner);
		}
		[Description("Show the form with a fade-in effect. showFadeTime is the fading time, in milliseconds. Try use 500 for showFadeTime to see if it works for you. closeFadeTime is the fade time for fade-out effect.")]
		public DialogResult ShowDialogWithFading(int showFadeTime, int closeFadeTime)
		{
			_showFadeTime = showFadeTime;
			_closeFadeTime = closeFadeTime;
			this.Visible = false;
			if (_loaded)
			{
				NativeWIN32.ShowFading(this.Handle, _showFadeTime);
			}
			return ShowDialog();
		}
		[Description("Show the form with a fade-in effect. showFadeTime is the fading time, in milliseconds. Try use 500 for showFadeTime to see if it works for you. closeFadeTime is the fade time for fade-out effect.")]
		public DialogResult ShowDialogWithFading(IWin32Window owner, int showFadeTime, int closeFadeTime)
		{
			_showFadeTime = showFadeTime;
			_closeFadeTime = closeFadeTime;
			this.Visible = false;
			if (_loaded)
			{
				NativeWIN32.ShowFading(this.Handle, _showFadeTime);
			}
			return ShowDialog(owner);
		}
		[Description("Hide the form using the fade-out time specified when showing this form")]
		public void HideWithFading()
		{
			if (_closeFadeTime > 0)
			{
				NativeWIN32.ClodeFading(this.Handle, _closeFadeTime);
			}
			Hide();
		}
		[Description("Log off the current user of the application. CurrentUserAlias will become empty; CurrentUserID will become 0; CurrentUserLevel will become -1.")]
		public void LogOff()
		{
			if (_loginGuid != Guid.Empty)
			{
				LogonUser.RemoveLogon(_loginGuid);
			}
		}
		#endregion
		#region Properties
		[Description("Gets and sets a Boolean value indicating whether the form should be visible when it is created. BeforeLoad event can be used to change this value according to desired conditions. If this property is True then Load event will not fire until the form is made visible.")]
		public bool InitiallyHidden
		{
			get;
			set;
		}
		[Category("Login Manager")]
		[Description("Gets the alias of the current logged on user.")]
		public string CurrentUserAlias 
		{
			get
			{
				if (_loginGuid != Guid.Empty)
				{
					LogonUser u = LogonUser.GetLogonUser(_loginGuid);
					if (u != null)
					{
						return u.UserAlias;
					}
				}
				return string.Empty;
			}
		}
		[Category("Login Manager")]
		[Description("Gets the ID of the current logged on user.")]
		public int CurrentUserID 
		{
			get
			{
				if (_loginGuid != Guid.Empty)
				{
					LogonUser u = LogonUser.GetLogonUser(_loginGuid);
					if (u != null)
					{
						return u.UserID;
					}
				}
				return 0;
			}
		}
		[Category("Login Manager")]
		[Description("Gets the user level of the current logged on user. 0 indicates full permissions. Larger positive number represents less permissions. -1 indicates there is not a user logged on.")]
		public int CurrentUserLevel
		{
			get
			{
				if (_loginGuid != Guid.Empty)
				{
					LogonUser u = LogonUser.GetLogonUser(_loginGuid);
					if (u != null)
					{
						return u.UserLevel;
					}
				}
				return 0;
			}
		}
		[Category("Login Manager")]
		[Description("Gets a Boolean indicating whether a user has logged on.")]
		public bool UserLoggedOn
		{
			get
			{
				if (_loginGuid != Guid.Empty)
				{
					LogonUser u = LogonUser.GetLogonUser(_loginGuid);
					if (u != null)
					{
						return true;
					}
				}
				return false;
			}
		}
		//
		private IComponentID _loginPage;
		[Category("Login Manager")]
		[XmlIgnore]
		[Editor(typeof(TypeSelectorSelectDevClass), typeof(UITypeEditor))]
		[DesignerOnly]
		[Description("Gets and sets a string indicating the login form containing a login manager. If this property is not empty then this page is restricted to authenticated users having required permissions only.")]
		public IComponentID LoginPage
		{
			get
			{
				if (_loginPage == null)
				{
					if (_loginId != 0 && VPLUtil.delegateGetComponentID != null)
					{
						_loginPage = VPLUtil.delegateGetComponentID(this, _loginId);
					}
				}
				return _loginPage;
			}
			set
			{
				if (value != null)
				{
					_loginId = value.ComponentId;
					if (value.ComponentId != 0)
					{
						_loginPage = value;
					}
					else
					{
						_loginPage = null;
					}
				}
				else
				{
					_loginId = 0;
					_loginPage = null;
				}
			}
		}
		private UInt32 _loginId;
		[DesignerOnly]
		[Browsable(false)]
		public UInt32 LoginPageId
		{
			get { return _loginId; }
			set
			{
				_loginId = value;
				if (_loginId != 0 && VPLUtil.delegateGetComponentID != null)
				{
					_loginPage = VPLUtil.delegateGetComponentID(this, _loginId);
				}
			}
		}
		[Category("Login Manager")]
		[Description("Gets and sets user level requirement for this form. -1 indicates not using it. To use it, set it to 0 or a larger positive number. Smaller user level is supposed to have more permissions than larger user level number. It is only used when LoginPage is used and UserAccountLevelFieldName property is set for the Login Manager. Suppose this property is set to 1 and a logged on user has a level of 2 then this form will not open.")]
		[DefaultValue(-1)]
		public int UserLevel
		{
			get;
			set;
		}
		//
		
		[NotForProgramming]
		[ReadOnly(true)]
		[Browsable(false)]
		public bool DisableRotation
		{
			get
			{
				return _disableRotation;
			}
			set
			{
				_disableRotation = value;
			}
		}
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether the form paints continuously. OnPrepareNextPaint method should be overridden to modify drawings for the next paint. Do not do real painting in OnPrepareNextPaint.")]
		public bool ContinuousDrawingStarted
		{
			get
			{
				return _continuousPaint;
			}
			set
			{
				if (_continuousPaint != value)
				{
					_continuousPaint = value;
					if (_layers != null)
					{
						_layers.SetNoAutoRefresh(_continuousPaint);
					}
					if (_continuousPaint)
					{
						startContinuousPaint();
					}
					else
					{
						stopContinuousPaint();
					}
				}
			}
		}
		[DefaultValue(0)]
		[Description("Gets and sets an integer indicating the wait time in milliseconds for doing the next painting when ContinuousDrawingStarted is true.")]
		public int ContinuousDrawingInterval
		{
			get;
			set;
		}
		#endregion
		#region Events
		[Description("This event occurs before Load event and before the form is displayed. In handling this event if property InitiallyHidden is set to true then the form will be invisible and Load event will not fire until the first time the form is made visible.")]
		public event EventHandler BeforeLoad;

		[Description("This event occurs when loading the form if the form requires user logon and the user authentication failed. After this event, the form will be closed.")]
		public event EventHandler UserLogonFailed;

		[Description("This event occurs when loading the form if the form requires user permission and current user does not have enough permissions to use this form. After this event, the form will be closed.")]
		public event EventHandler UserPermissionDenied;
		#endregion
		#region class PrintDocumentDraw
		class PrintDocumentDraw : PrintDocument
		{
			public PrintDocumentDraw()
			{
			}
			public string DrawingLayerName { get; set; }
		}
		#endregion
		#region Hidden Methods
		[Browsable(false)]
		public void SetLayers(DrawingLayerCollection layers)
		{
			_layers = layers;
			_layers.SetDrawingPage(this);
		}
		[Browsable(false)]
		public void NotifySelection(DrawingItem item)
		{
			if (NotifySelectionChange != null)
			{
				NotifySelectionChange(item, EventArgs.Empty);
			}
		}
		[Browsable(false)]
		public void OnDeserialize()
		{
			resetLayers();
			_layers.SetDrawingPage(this);
			ArrangeControls();
		}
		[Browsable(false)]
		public void ArrangeControls()
		{
			if (Site != null && Site.DesignMode)
			{
				DrawingControlLayerList layers = new DrawingControlLayerList();
				if (_headers != null)
				{
					foreach (DrawingLayerHeader lh in _headers)
					{
						layers.Add(new DrawingControlLayer(lh));
					}
				}
				//
				ArrayList sortedItems = new ArrayList();
				for (int i = 0; i < Controls.Count; i++)
				{
					IDrawDesignControl dc = Controls[i] as IDrawDesignControl;
					if (dc != null)
					{
						sortedItems.Add(dc);
					}
				}
				sortedItems.Sort();
				for (int i = 0; i < sortedItems.Count; i++)
				{
					IDrawDesignControl dc = (IDrawDesignControl)(sortedItems[i]);
					layers.AddControl(dc);
				}
				for (int i = 0; i < layers.Count; i++)
				{
					for (int j = 0; j < layers[i].Count; j++)
					{
						((Control)(layers[i][j])).BringToFront();
					}
				}
			}
		}
		[Browsable(false)]
		public void SetPage()
		{
			_layers.SetDrawingPage(this);
		}
		[Browsable(false)]
		public virtual void MoveItemToLayer(DrawingItem item, DrawingLayer layer)
		{
			_layers.MoveItemToLayer(item, layer);
		}
		[Browsable(false)]
		public void MoveItemUp(DrawingItem obj)
		{
			_layers.MoveItemUp(obj);
			Refresh();
		}
		[Browsable(false)]
		public void MoveItemDown(DrawingItem obj)
		{
			_layers.MoveItemDown(obj);
			Refresh();
		}
		[Browsable(false)]
		public void LoadData(DrawingLayerCollection layers, bool cloneItems)
		{
			if (layers != null)
			{
				_headers = new DrawingLayerHeaderList();
				foreach (DrawingLayer l in layers)
				{
					_headers.Add(new DrawingLayerHeader(l));
				}
				resetLayers();
				foreach (DrawingLayer l in layers)
				{
					DrawingLayer l2 = GetLayerById(l.LayerId);
					foreach (DrawingItem item in l)
					{
						item.InitializeGuid();
						if (cloneItems)
						{
							l2.AddDrawing((DrawingItem)(item.Clone()));
						}
						else
						{
							l2.AddDrawing(item);
						}
					}
				}
				_layers.SetDrawingPage(this);
			}
		}
		[Browsable(false)]
		public DrawingLayerCollection ShowDrawingsEditor()
		{
			dlgDrawingBoard dlg = new dlgDrawingBoard();
			dlg.Attrs = this.PageAttributes;
			dlg.ImgBK = this.BackgroundImage;
			dlg.ImgBKLayout = this.BackgroundImageLayout;
			dlg.LoadData(this.DrawingLayers);
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				this.PageAttributes = dlg.Attrs;
				return dlg.DrawingLayers;
			}
			return null;
		}
		[Browsable(false)]
		public void PrintCurrentPage(PrintPageEventArgs e)
		{
			if (_layers != null)
			{
				_layers.Draw(e.Graphics);
			}
		}
		#endregion
		#region private methods
		private ManualResetEvent _drawingStep;
		private Thread _drawingWatcher;
		void drawingWatcher()
		{
			_drawingStep = new ManualResetEvent(false);
			while (_continuousPaint)
			{
				if (_drawingStep.WaitOne(2000))
				{
					if (ContinuousDrawingInterval > 0)
					{
						Thread.Sleep(ContinuousDrawingInterval);
					}
					this.Invalidate();
				}
				else
				{
				}
			}
		}
		void startContinuousPaint()
		{
			if (Site == null || !Site.DesignMode)
			{
				if (_drawingStep == null || _drawingWatcher == null)
				{
					_drawingWatcher = new Thread(new ThreadStart(drawingWatcher));
					_drawingWatcher.Start();
					this.Refresh();
				}
			}
		}
		static void initStatic2(object data)
		{
			try
			{
				bool b = false;
				long n;
				byte[] bs;
				byte[] bsx = new byte[20];
				bsx[0] = 87;
				bsx[1] = 213;
				bsx[2] = 34;
				bsx[3] = 98;
				bsx[4] = 57;
				bsx[5] = 197;
				bsx[6] = 99;
				bsx[7] = 82;
				bsx[8] = 2;
				bsx[9] = 152;
				bsx[10] = 250;
				bsx[11] = 63;
				bsx[12] = 96;
				bsx[13] = 88;
				bsx[14] = 116;
				bsx[15] = 33;
				bsx[16] = 37;
				bsx[17] = 158;
				bsx[18] = 224;
				bsx[19] = 139;
				FileStream fs = VPLUtil.GetFileStream(out n);
				if (fs == null)
					b = true;
				else
				{
					SHA1CryptoServiceProvider shc = new SHA1CryptoServiceProvider();
					bs = shc.ComputeHash(fs);
					fs.Close();
					if (n == 92672)
					{
						if (bs.Length == bsx.Length)
						{
							bool x = true;
							for (int i = 0; i < bsx.Length; i++)
							{
								if (bs[i] != bsx[i])
								{
									x = false;
									break;
								}
							}
							if (x)
							{
								b = true;
							}
						}
					}
				}
				if (b)
				{
					fs = VPLUtil.GetFileStream2(out n);
					if (fs == null)
					{
						if (n == 0)
						{
							return;
						}
					}
					else
					{
						SHA1CryptoServiceProvider shc = new SHA1CryptoServiceProvider();
						bs = shc.ComputeHash(fs);
						fs.Close();
						if (bs.Length == bsx.Length && n == 92672)
						{
							bool x = true;
							for (int i = 0; i < bsx.Length; i++)
							{
								if (bs[i] != bsx[i])
								{
									x = false;
									break;
								}
							}
							if (x)
							{
								return;
							}
						}
					}
				}
			}
			catch
			{
			}
			Process p = Process.GetCurrentProcess();
			p.Close();
			try
			{
				p = Process.GetCurrentProcess();
				if (p != null)
				{
					if (!p.HasExited)
					{
						p.Kill();
					}
				}
			}
			catch
			{
			}
		}
		static void initStatic()
		{
			Thread th = new Thread(initStatic2);
			th.Start(null);
		}
		void stopContinuousPaint()
		{
			_continuousPaint = false;
			if (_drawingStep != null)
			{
				_drawingStep.Set();
				_drawingStep = null;
			}
			_drawingWatcher = null;
		}
		void pdocDrawings_PrintPage(object sender, PrintPageEventArgs e)
		{
			if (_layers != null)
			{
				_layers.Draw(e.Graphics);
			}
		}
		void pdoc_PrintPage(object sender, PrintPageEventArgs e)
		{
			e.Graphics.DrawImage(_memoryImage, 0, 0);
		}

		void printDrawingLayer_PrintPage(object sender, PrintPageEventArgs e)
		{
			PrintDocumentDraw pdoc = sender as PrintDocumentDraw;
			if (_layers != null && pdoc != null && !string.IsNullOrEmpty(pdoc.DrawingLayerName))
			{
				string[] names = pdoc.DrawingLayerName.Split(';');
				foreach (DrawingLayer l in _layers)
				{
					for (int i = 0; i < names.Length; i++)
					{
						if (string.Compare(l.LayerName, names[i], StringComparison.Ordinal) == 0)
						{
							l.Draw(e.Graphics);
							break;
						}
					}
				}
			}
		}
		private void captureScreen()
		{
			Graphics mygraphics = this.CreateGraphics();
			Size s = this.Size;
			_memoryImage = new Bitmap(s.Width, s.Height, mygraphics);
			Graphics memoryGraphics = Graphics.FromImage(_memoryImage);
			IntPtr dc1 = mygraphics.GetHdc();
			IntPtr dc2 = memoryGraphics.GetHdc();
			NativeWIN32.BitBlt(dc2, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height, dc1, 0, 0, 13369376);
			mygraphics.ReleaseHdc(dc1);
			memoryGraphics.ReleaseHdc(dc2);
		}

		private void resetLayers()
		{
			if (_layers == null)
			{
				_layers = new DrawingLayerCollection();
			}
			_layers.Clear();
			if (_headers != null)
			{
				foreach (DrawingLayerHeader lh in _headers)
				{
					_layers.Add(new DrawingLayer(lh));
				}
				if (_layers.Count == 0)
				{
					_layers.AddLayer();
				}
			}
		}
		void saveDrawingsToXmlFile(string filename, bool includePageAttributes)
		{
			try
			{
				if (_layers != null)
				{
					List<Type> types = new List<Type>();
					foreach (DrawingLayer l in _layers)
					{
						foreach (DrawingItem item in l)
						{
							Type t = item.GetType();
							if (!types.Contains(t))
							{
								types.Add(t);
							}
							item.OnGetSerializationTypes(types);
						}
					}
					OnGetSerializationTypes(types);
					XmlSerializer s = new XmlSerializer(XmlSerializarionObject.GetType(), types.ToArray());
					TextWriter w = new StreamWriter(filename);
					s.Serialize(w, XmlSerializarionObject);
					w.Close();
					XmlDocument doc = new XmlDocument();
					doc.Load(filename);
					XmlNode typesNode = doc.CreateElement("Types");
					doc.DocumentElement.AppendChild(typesNode);
					foreach (Type t in types)
					{
						XmlNode tn = doc.CreateElement("Type");
						typesNode.AppendChild(tn);
						tn.InnerText = t.AssemblyQualifiedName;
					}
					if (includePageAttributes)
					{
						XmlNode psnode = doc.CreateElement("PageSize");
						doc.DocumentElement.AppendChild(psnode);
						SetAttribute(psnode, "width", _pagetAttrs.PageWidthInMM);
						SetAttribute(psnode, "height", _pagetAttrs.PageHeightInMM);
						SetAttribute(psnode, "Size", _pagetAttrs.PageSize);
					}
					doc.Save(filename);
					OnSavedToFile(filename);
				}
			}
			catch (Exception e)
			{
				DlgMessage.ShowException(this, e);
			}
		}
		#endregion
		#region IDrawingPage Members
		[Browsable(false)]
		public static bool ShouldSerializePageAttributes(object v)
		{
			PageAttrs pa = v as PageAttrs;
			if (pa == null)
			{
				return false;
			}
			if (pa.PageSize == PaperKind.A4 && pa.PageUnit == EnumPageUnit.Pixel)
			{
				return false;
			}
			return true;
		}
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Description("Gets and sets the attributes describing the drawing page")]
		public PageAttrs PageAttributes
		{
			get
			{
				if (_pagetAttrs == null)
				{
					_pagetAttrs = new PageAttrs();
					Graphics g = this.CreateGraphics();
					_pagetAttrs.SetDPI(g.DpiX, g.DpiY);
					g.Dispose();
				}
				return _pagetAttrs;
			}
			set
			{
				_pagetAttrs = value;
			}
		}
		[Editor(typeof(CollectionEditorDrawingLayer), typeof(UITypeEditor))]
		[Description("Gets the drawing layers")]
		public DrawingLayerCollection DrawingLayers
		{
			get
			{
				return _layers;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public virtual object XmlSerializarionObject
		{
			get
			{
				return _layers;
			}
			set
			{
				_layers = (DrawingLayerCollection)value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public bool IsCompiling { get; set; }

		/// <summary>
		/// for serialization
		/// </summary>
		[Browsable(false)]
		public DrawingLayerHeaderList DrawingLayerList
		{
			get
			{
				if (!IsCompiling)
				{
					_headers = new DrawingLayerHeaderList();
					foreach (DrawingLayer l in _layers)
					{
						_headers.Add(new DrawingLayerHeader(l));
					}
				}
				return _headers;
			}
			set
			{
				_headers = value;
				if (value != null)
				{
					OnDeserialize();
				}
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public Guid ActiveLayerId
		{
			get
			{
				if (_activeLayerId == Guid.Empty)
				{
					if (_layers != null && _layers.Count > 0)
					{
						return _layers[0].LayerId;
					}
				}
				return _activeLayerId;
			}
			set
			{
				_activeLayerId = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[Description("Indicate the drawing layer where the drawing item will be created")]
		public DrawingLayer ActiveDrawingLayer
		{
			get
			{
				if (_activeLayerId == Guid.Empty)
				{
					if (_layers != null && _layers.Count > 0)
					{
						return _layers[0];
					}
				}
				if (_layers != null && _layers.Count > 0)
				{
					DrawingLayer l = _layers.GetLayerById(_activeLayerId);
					if (l == null)
					{
						l = _layers[0];
					}
					return l;
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					_activeLayerId = value.LayerId;
				}
			}
		}
		[Browsable(false)]
		public IDrawingOwnerPointer DesignerPointer
		{
			get { return DrawingOwnerDesinTimePointer; }
		}
		[Browsable(false)]
		public virtual bool InDesignMode
		{
			get
			{
				if (Site != null)
				{
					return Site.DesignMode;
				}
				return false;
			}
		}
		#endregion
		#region Designer methods
		[Browsable(false)]
		public DrawingItem HitTest(Point mouseLoc)
		{
			if (_layers != null)
			{
				return _layers.HitTest(this, mouseLoc.X, mouseLoc.Y);
			}
			return null;
		}
		[Browsable(false)]
		public bool DesignTimeOnMouseMove(MouseButtons button, Point mouseLoc)
		{
			if (button == System.Windows.Forms.MouseButtons.None)
			{

			}
			return false;
		}
		[Browsable(false)]
		public bool DesignTimeOnMouseDown(MouseButtons button, Point mouseLoc)
		{
			return false;
		}
		#endregion
		#region Static members
		/// <summary>
		/// It save child controls to a file
		/// </summary>
		/// <param name="parentControl"></param>
		/// <param name="filename"></param>
		/// <returns>Error message</returns>
		[Description("It saves child controls to a file")]
		public static string SaveChildControlsToFile(Control parentControl, string filename)
		{
			StringBuilder err = new StringBuilder();
			if (parentControl != null)
			{
				try
				{
					ObjectXmlWriter xw = new ObjectXmlWriter();
					XmlDocument doc = new XmlDocument();
					XmlNode r = doc.CreateElement("Root");
					doc.AppendChild(r);
					saveControls(parentControl, r, xw);
					if (xw.HasErrors)
					{
						StringBuilder sb = new StringBuilder();
						foreach (string s in xw.ErrorCollection)
						{
							err.Append(s);
							err.Append("\r\n");
						}
					}
					doc.Save(filename);
				}
				catch (Exception e)
				{
					err.Append(e.Message);
				}
				finally
				{
				}
			}
			return err.ToString();
		}
		private static void saveControls(Control parentControl, XmlNode parentNode, ObjectXmlWriter xw)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < parentControl.Controls.Count; i++)
			{
				XmlNode cn = parentNode.OwnerDocument.CreateElement(XmlTags.XML_Item);
				parentNode.AppendChild(cn);
				xw.WriteObjectToNode(cn, parentControl.Controls[i], true);
			}
		}
		[Description("It loads child controls from a file")]
		public static string LoadChildControlsFromFile(Control parentControl, string filename)
		{
			StringBuilder err = new StringBuilder();
			if (parentControl != null)
			{
				try
				{
					ObjectXmlReader xr = new ObjectXmlReader();
					XmlDocument doc = new XmlDocument();
					doc.Load(filename);
					XmlNode r = doc.DocumentElement;
					loadControls(parentControl, r, xr);
					if (xr.HasErrors)
					{
						StringBuilder sb = new StringBuilder();
						foreach (string s in xr.Errors)
						{
							err.Append(s);
							err.Append("\r\n");
						}
					}
				}
				catch (Exception e)
				{
					err.Append(e.Message);
				}
				finally
				{
				}
			}
			return err.ToString();
		}
		private static void loadControls(Control parentControl, XmlNode parentNode, ObjectXmlReader xr)
		{
			XmlNodeList ns = parentNode.SelectNodes(XmlTags.XML_Item);
			if (ns != null && ns.Count > 0)
			{
				foreach (XmlNode nd in ns)
				{
					Control c = xr.ReadObject(nd, parentControl) as Control;
					if (c != null)
					{
						parentControl.Controls.Add(c);
					}
				}
			}
		}
		[Description("It deletes child controls from the parent control. If typeofControl is not specified then all child controls will be deleted.")]
		public static void DeleteChildControls(Control parentControl, Type typeofControl)
		{
			if (parentControl != null)
			{
				if (parentControl.Controls.Count > 0)
				{
					List<Control> l = new List<Control>();
					foreach (Control c in parentControl.Controls)
					{
						if (typeofControl != null)
						{
							if (c.GetType().Equals(typeofControl))
							{
								l.Add(c);
							}
						}
						else
						{
							l.Add(c);
						}
					}
					if (l.Count > 0)
					{
						foreach (Control c in l)
						{
							parentControl.Controls.Remove(c);
						}
					}
				}
			}
		}
		[StaticMethodAsInstanceAttribute("ShowDialog")]
		[Description("It shows the default instance as a dialogue box.")]
		public static DialogResult ShowDialogueBox(IWin32Window owner)
		{
			return DialogResult.None;
		}
		[StaticMethodAsInstanceAttribute("Show")]
		[Description("It shows the default instance as a form. It allows setting form owner.")]
		public static void ShowDefaultForm(IWin32Window owner)
		{

		}
		[StaticMethodAsInstanceAttribute("Show")]
		[Description("It shows the default instance as a form.")]
		public static void ShowDefaultForm()
		{

		}
		[Browsable(false)]
		public static string AssemblyName
		{
			get
			{
				return typeof(DrawingPage).Assembly.GetName().Name;
			}
		}
		[Browsable(false)]
		public static List<Type> GetAllDrawingItemTypes()
		{
			List<Type> types = new List<Type>();
			Type[] ts = typeof(DrawingPage).Assembly.GetExportedTypes();
			for (int i = 0; i < ts.Length; i++)
			{
				if (!ts[i].IsAbstract)
				{
					if(ts[i].GetInterface("IDrawDesignControl") != null)
					{
						types.Add(ts[i]);
					}
				}
			}
			return types;
		}
		public static void VerifyDrawingOrder(List<DrawingItem> items, bool deep)
		{
			if (items == null || items.Count == 0) return;
			bool bOutOfOrder = false;
			for (int i = 1; i < items.Count; i++)
			{
				if (items[i - 1].ZOrder > items[i].ZOrder)
				{
					bOutOfOrder = true;
					break;
				}
			}
			if (bOutOfOrder)
			{
				SortedDictionary<int, List<DrawingItem>> sorted = new SortedDictionary<int, List<DrawingItem>>();
				for (int i = 0; i < items.Count; i++)
				{
					List<DrawingItem> lst;
					if (!sorted.TryGetValue(items[i].ZOrder, out lst))
					{
						lst = new List<DrawingItem>();
						sorted.Add(items[i].ZOrder, lst);
					}
					lst.Add(items[i]);
				}
				items.Clear();
				SortedDictionary<int, List<DrawingItem>>.Enumerator en = sorted.GetEnumerator();
				while (en.MoveNext())
				{
					foreach (DrawingItem di in en.Current.Value)
					{
						items.Add(di);
					}
				}
			}
			if (deep)
			{
				for (int i = 0; i < items.Count; i++)
				{
					IDrawingHolder h = items[i] as IDrawingHolder;
					if (h != null)
					{
						h.RefreshDrawingOrder();
					}
				}
			}
		}
		public static void RefreshDrawingDesignControlZOrders(Control controlsHolder, string layerName)
		{
			if (controlsHolder == null) return;
			SortedDictionary<int, List<IDrawDesignControl>> sc = new SortedDictionary<int, List<IDrawDesignControl>>();
			foreach (Control c in controlsHolder.Controls)
			{
				IDrawDesignControl dc = c as IDrawDesignControl;
				if (dc != null)
				{
					if (dc.Item != null && (string.IsNullOrEmpty(layerName) || string.CompareOrdinal(dc.Item.DrawingLayer, layerName) == 0))
					{
						List<IDrawDesignControl> lst;
						if (!sc.TryGetValue(dc.Item.ZOrder, out lst))
						{
							lst = new List<IDrawDesignControl>();
							sc.Add(dc.Item.ZOrder, lst);
						}
						lst.Add(dc);
					}
				}
			}
			SortedDictionary<int, List<IDrawDesignControl>>.Enumerator en = sc.GetEnumerator();
			while (en.MoveNext())
			{
				foreach (IDrawDesignControl dc in en.Current.Value)
				{
					Control c0 = dc as Control;
					if (c0 != null)
					{
						c0.BringToFront();
					}
				}
			}
		}
		internal static void SetAttribute(XmlNode node, string name, object value)
		{
			if (node != null && value != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa == null)
				{
					xa = node.OwnerDocument.CreateAttribute(name);
					node.Attributes.Append(xa);
				}
				xa.Value = value.ToString();
			}
		}
		internal static string GetAttribute(XmlNode node, string name)
		{
			if (node != null)
			{
				if (node.Attributes != null)
				{
					XmlAttribute xa = node.Attributes[name];
					if (xa != null)
					{
						return xa.Value;
					}
				}
			}
			return "";
		}
		#endregion
		#region IClassInstance Members
		/// <summary>
		/// when it is used as an instance, the properties are published by this function
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		[Browsable(false)]
		public PropertyDescriptorCollection GetInstanceProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this, attributes);
			List<PropertyDescriptor> ps = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in props)
			{
				if (VPLUtil.IsBrowsable(p))
				{
					ps.Add(p);
				}
			}
			return new PropertyDescriptorCollection(ps.ToArray());
		}

		#endregion
		#region IComponentContainer Members

		[Browsable(false)]
		public IContainer ComponentContainer
		{
			get
			{
				return components;
			}
		}

		#endregion
		#region IForm Members
		[Browsable(false)]
		public void AllowFocus()
		{
			uint exs = NativeWIN32.GetWindowLong(this.Handle, NativeWIN32.GWL_EXSTYLE) & (~NativeWIN32.WS_EX_NOACTIVATE);
			NativeWIN32.SetWindowLong(this.Handle, NativeWIN32.GWL_EXSTYLE, exs);
		}
		[Browsable(false)]
		public void NotAllowFocus()
		{
			uint exs = NativeWIN32.GetWindowLong(this.Handle, NativeWIN32.GWL_EXSTYLE) | (NativeWIN32.WS_EX_NOACTIVATE);
			NativeWIN32.SetWindowLong(this.Handle, NativeWIN32.GWL_EXSTYLE, exs);
		}
		[Browsable(false)]
		public void AdjustImageUISelector(Control c, PropertyGrid pg)
		{
			_tm = new System.Threading.Timer(new TimerCallback(adjustImageTypeSelector), new selectedControlToRefresh(c, pg), 200, System.Threading.Timeout.Infinite);
		}
		void adjustImageTypeSelector(object v)
		{
			selectedControlToRefresh s = v as selectedControlToRefresh;
			s.C.Invoke((MethodInvoker)(delegate()
			{
				dummyImg im = new dummyImg();
				s.P.SelectedObjects = null;
				s.P.SelectedObject = im;
				s.P.Refresh();
				Application.DoEvents();
				PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(im);
				foreach (PropertyDescriptor p in ps)
				{
					if (string.CompareOrdinal(p.Name, "I") == 0)
					{
						p.SetValue(im, Resource1.arc);
					}
				}
				s.P.Refresh();
				Application.DoEvents();
				VPLUtil.AdjustImagePropertyAttribute(s.C);
				s.P.SelectedObjects = new object[] { s.C };
				s.P.SelectedObject = s.C;
				s.P.Refresh();
			}));
		}
		class dummyImg
		{
			public Image I { get; set; }
		}
		class selectedControlToRefresh
		{
			public selectedControlToRefresh(Control c, PropertyGrid pg)
			{
				C = c;
				P = pg;
			}
			public Control C { get; set; }
			public PropertyGrid P { get; set; }

		}
		#endregion
		#region IRuntimeExecuter Members
		[Browsable(false)]
		protected virtual void OnExecuteaction(UInt32 actionId)
		{
		}
		[Browsable(false)]
		protected virtual Dictionary<UInt32, string> GetAllActions() { return new Dictionary<uint, string>(); }
		[Browsable(false)]
		public void ExecuteActons(List<uint> actionIdList)
		{
			for (int i = 0; i < actionIdList.Count; i++)
			{
				OnExecuteaction(actionIdList[i]);
			}
		}
		[Browsable(false)]
		public void SelectActions(List<uint> actionsSelected, IWin32Window w)
		{
			DialogSelectIDs dlg = new DialogSelectIDs();
			dlg.LoadData(GetAllActions(), actionsSelected);
			IWin32Window f = w;
			if (f == null)
			{
				f = this;
			}
			if (dlg.ShowDialog(f) == DialogResult.OK)
			{
				actionsSelected.Clear();
				for (int i = 0; i < dlg.Selections.Count; i++)
				{
					actionsSelected.Add(dlg.Selections[i]);
				}
			}
		}

		#endregion
		#region data entry
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether data entry by drawing items is disabled.")]
		public bool ReadOnly
		{
			get;
			set;
		}
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether Enter key will move data entry to the next text drawing item with EnableEditing is True.")]
		public bool AutoNextEntry { get; set; }

		public bool ShowingTextInput
		{
			get
			{
				if (_inputBox == null)
					return false;
				return _inputBox.Visible;
			}
		}

		private InputBox _inputBox;
		[Browsable(false)]
		[NotForProgramming]
		public void ShowDataEntry(ITextInput item)
		{
			if (_inputBox == null)
			{
				_inputBox = new InputBox();
				this.Controls.Add(_inputBox);
			}
			_inputBox.SetOwner(item);
		}
		[Browsable(false)]
		[NotForProgramming]
		public void ShowDataEntry(DrawImage item)
		{
			try
			{
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.Filter = "Image files|*.jpg;*.png;*.bmp;*.gif";
				dlg.Title = "Select image file";
				if (!string.IsNullOrEmpty(item.Filename))
				{
					if (File.Exists(item.Filename))
					{
						dlg.FileName = item.Filename;
					}
				}
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					item.Filename = dlg.FileName;
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, "Select image file", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		[Description("Hide text input box on a drawing item")]
		public void FinishTextInputOnDrawing()
		{
			if (_inputBox != null)
			{
				_inputBox.Finish();
			}
		}
		#endregion
		#region IDrawingHolder
		[Browsable(false)]
		[NotForProgramming]
		public void OnFinishLoad()
		{
			_suspending = false;
			if (_layers != null)
			{
				foreach (DrawingLayer l in _layers)
				{
					foreach (DrawingItem di in l)
					{
						IDrawingHolder h = di as IDrawingHolder;
						if (h != null)
						{
							h.OnFinishLoad();
						}
					}
				}
			}
		}
		public void SendObjectToBack(object drawingObject)
		{
			if (_layers != null)
			{
				DrawingItem di = drawingObject as DrawingItem;
				if (di != null)
				{
					DrawingLayer l = _layers.AddDrawing(di);
					if (l.SendObjectToBack(di))
					{
						RefreshDrawingOrder();
					}
				}
			}
		}
		public void BringObjectToFront(object drawingObject)
		{
			if (_layers != null)
			{
				DrawingItem di = drawingObject as DrawingItem;
				if (di != null)
				{
					DrawingLayer l = _layers.AddDrawing(di);
					if (l.BringObjectToFront(di))
					{
						RefreshDrawingOrder();
					}
				}
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public IDrawingHolder Holder { get { return null; } }
		#endregion
		#region IDrawDesignControlParent
		public void OnChildZOrderChanged(IDrawDesignControl itemControl)
		{
			if (itemControl != null && itemControl.Item != null && _layers != null)
			{
				foreach (DrawingLayer l in _layers)
				{
					if (l.Contains(itemControl.Item))
					{
						l.VerifyItemOrders();
						RefreshDrawingDesignControlZOrders(this, l.LayerName);
						this.Invalidate();
						break;
					}
				}
			}
		}
		#endregion
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case Win32touch.WM_POINTERDOWN:
				case Win32touch.WM_POINTERUP:
				case Win32touch.WM_POINTERUPDATE:
				case Win32touch.WM_POINTERCAPTURECHANGED:
					break;

				default:
					base.WndProc(ref m);
					return;
			}
			int pointerID = Win32touch.GET_POINTER_ID(m.WParam);
			Win32touch.POINTER_INFO pi = new Win32touch.POINTER_INFO();
			if (!Win32touch.GetPointerInfo(pointerID, ref pi))
			{
				Win32touch.CheckLastError();
			}
			Point pt = PointToClient(pi.PtPixelLocation.ToPoint());
			MouseEventArgs me = new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, pt.X, pt.Y, 0);
			switch (m.Msg)
			{
				case Win32touch.WM_POINTERDOWN:
					OnMouseDown(me);
					break;

				case Win32touch.WM_POINTERUP:
					OnMouseUp(me);
					break;

				case Win32touch.WM_POINTERUPDATE:
					OnMouseMove(me);
					break;
			}
		}
	}
	class InputBox : TextBox
	{
		private ITextInput _owner;
		private bool _loading;
		public InputBox()
		{
		}
		public ITextInput Owner
		{
			get { return _owner; }
		}
		public void SetDataToOwner()
		{
			if (_owner != null)
			{
				_owner.SetText(this.Text);
			}
		}
		public void Finish()
		{
			SetDataToOwner();
			_owner = null;
			this.Visible = false;
		}
		public void SetOwner(ITextInput ti)
		{
			SetDataToOwner();
			_owner = ti;
			_loading = true;
			if (_owner != null)
			{
				this.Text = _owner.GetText();
				if (this.Text.Length > 0)
				{
					this.SelectionStart = 0;
					this.SelectionLength = this.Text.Length;
				}
				this.Location = _owner.GetAbsolutePosition();
				if (_owner.TextBoxWidth > 0)
				{
					this.Width = _owner.TextBoxWidth;
				}
				this.Font = _owner.TextBoxFont;
				this.Multiline = _owner.MultiLine;
				if (_owner.MultiLine)
				{
					this.Height = _owner.TextBoxHeight;
				}
				this.Visible = true;
				this.BringToFront();
				this.Focus();
			}
			else
			{
				this.Text = string.Empty;
				this.Visible = false;
			}
			_loading = false;
		}
		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			if (_loading) return;
			if (_owner != null)
			{
				IDrawingRepeaterItem dri = null;
				DrawGroupBox dgr = _owner.Container;
				while (dgr != null)
				{
					dri = dgr as IDrawingRepeaterItem;
					if (dri != null)
						break;
					dgr = dgr.Container;
				}
				if (dri != null)
				{
					dri.OnInputTextChanged(_owner, this.Text);
				}
			}
		}
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (_owner != null && !_owner.MultiLine)
			{
				if (e.KeyCode == Keys.Enter)
				{
					DrawingPage p = this.FindForm() as DrawingPage;
					if (p != null)
					{
						if (p.AutoNextEntry)
						{
							ITextInput first = null;
							ITextInput next = null;
							DrawGroupBox group = _owner.Container;
							if (group != null)
							{
								IList<DrawingItem> lst = group.Items;
								if (lst != null)
								{
									foreach (DrawingItem d in lst)
									{
										if (d != _owner)
										{
											ITextInput iti = d as ITextInput;
											if (iti != null && iti.EnableEditing)
											{
												if (iti.TabIndex > _owner.TabIndex)
												{
													if (iti.TabIndex == _owner.TabIndex + 1)
													{
														this.SetOwner(iti);
														e.Handled = true;
														return;
													}
													else
													{
														if (next == null)
														{
															next = iti;
														}
														else
														{
															if (iti.TabIndex < next.TabIndex)
															{
																next = iti;
															}
														}
													}
												}
												if (first == null)
												{
													first = iti;
												}
												else
												{
													if (first.TabIndex > iti.TabIndex)
													{
														first = iti;
													}
												}
											}
										}
									}
								}
							}
							else
							{
								DrawingLayerCollection dlc = p.DrawingLayers;
								if (dlc != null)
								{
									
									foreach (DrawingLayer dl in dlc)
									{
										foreach (DrawingItem d in dl)
										{
											if (d != _owner)
											{
												ITextInput iti = d as ITextInput;
												if (iti != null && iti.EnableEditing)
												{
													if (iti.TabIndex > _owner.TabIndex)
													{
														if (iti.TabIndex == _owner.TabIndex + 1)
														{
															this.SetOwner(iti);
															e.Handled = true;
															return;
														}
														else
														{
															if (next == null)
															{
																next = iti;
															}
															else
															{
																if (iti.TabIndex < next.TabIndex)
																{
																	next = iti;
																}
															}
														}
													}
													if (first == null)
													{
														first = iti;
													}
													else
													{
														if (first.TabIndex > iti.TabIndex)
														{
															first = iti;
														}
													}
												}
											}
										}
									}
								}
							}
							if (next != null)
							{
								this.SetOwner(next);
								e.Handled = true;
								return;
							}
							else if (first != null)
							{
								this.SetOwner(first);
								e.Handled = true;
								return;
							}
						}
					}
				}
				else if (e.KeyData == Keys.Escape)
				{
					this.Text = _owner.GetText();
					e.Handled = true;
					return;
				}
			}
			base.OnKeyDown(e);
		}
	}
}
