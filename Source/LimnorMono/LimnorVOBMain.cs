/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using VOB;
using XHost;
using System.Drawing.Design;
using SolutionMan;
using System.ComponentModel.Design.Serialization;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using VOBMsg;
using System.Runtime.InteropServices;
using VPL;
using MathExp;
using TraceLog;
using XToolbox2;
using LimnorUI;
using LimnorDesigner;
using System.Globalization;
using System.CodeDom;
using LimnorDesigner.ResourcesManager;
using Limnor.Drawing2D;
using Limnor.Reporting;
using XmlUtility;
using LimnorDatabase;
using System.Data.OleDb;
using Limnor.TreeViewExt;
using System.ServiceProcess;
using System.IO;
using LimnorDesigner.EventMap;
using VSPrj;
using MathItem;
using LimnorCompiler;
using System.Collections.Specialized;
using Limnor.CopyProtection;
using System.Data.Odbc;
using XmlSerializer;
using Limnor.WebBuilder;
using Limnor.PhpComponents;
using LimnorDesigner.Remoting;
using LimnorDesigner.Web;
using LimnorWeb;
using System.Diagnostics;
using FileUtil;
using Limnor.Application;
using WindowsUtility;

namespace LimnorVOB
{
	delegate void fnSimpleCall();
	public partial class LimnorVOBMain : Form, InterfaceVOB, ICompilerLog
	{
		#region fields and constructors
		private Label lblCloseTab;
		private ServiceMan genericServices;
		private XEventService eventService;
		private HostSurfaceManager _hostSurfaceManager = null;
		private Hashtable _allTools;
		private bool _solutionLoaded;
		private bool _projectLoaded;
		private bool _fuTypeLoaded;
		[Flags()]
		public enum KeyModifiers
		{
			None = 0,
			Alt = 1,
			Control = 2,
			Shift = 4,
			Windows = 8
		}
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool RegisterHotKey(IntPtr hWnd, // handle to window    
			int id,            // hot key identifier    
			KeyModifiers fsModifiers,  // key-modifier options    
			Keys vk            // virtual-key code    
			);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool UnregisterHotKey(IntPtr hWnd,  // handle to window    
			int id      // hot key identifier    
			);
		private bool _openingComponent;
		private Process _debugProcess;
		public LimnorVOBMain()
		{
			lblCloseTab = new Label();
			InitializeComponent();
			try
			{
				init();
				if (DesignUtil.BatchBuildEnabled())
				{
					buildToolStripMenuItem.DropDownItems.Add("Batch build", Resource1._run.ToBitmap(), batchBuild);
				}
			}
			catch (Exception e)
			{
				FormSplash.CloseSplash();
				MessageBox.Show(Program.FormExceptionText(e), "Exception in Main Form");
			}
			TraceLogClass.MainForm = this;
		}
		
		
		private void updateMru(string file)
		{
			MRU mru = new MRU();
			mru.SaveMRU(file);
			refreshMruMenus();
		}
		private void refreshMruMenus()
		{
			try
			{
				MruMenuItem.DropDownItems.Clear();
				MRU mru = new MRU();
				string[] ss = mru.GetMruFiles();
				EventHandler eh = new EventHandler(openMru);
				for (int i = 0; i < ss.Length; i++)
				{
					ToolStripMenuItem tm = new ToolStripMenuItem();
					tm.Text = string.Format("{0}. {1}", (i + 1), ss[i]);
					tm.Click += eh;
					tm.Tag = ss[i];
					MruMenuItem.DropDownItems.Add(tm);
				}
			}
			catch (Exception err)
			{
				UIUtil.ShowError(this, err);
			}
		}
		private void batchBuild(object sender, EventArgs e)
		{
			FormBatchBuilder dlg = new FormBatchBuilder();
			dlg.SetVOBMain(this);
			dlg.ShowDialog(this);
		}
		private void init()
		{
			DlgMsHtml.CheckMsHtml(this);
			//
			int nbit = IntPtr.Size * 8;
#if DOTNET40
			Text = String.Format(CultureInfo.InvariantCulture, "{0} ({1}-bit for .Net 4)", AboutBox1.AssemblyTitle, nbit);
#else
			Text = String.Format(CultureInfo.InvariantCulture, "{0} ({1}-bit for .Net 3.5)", AboutBox1.AssemblyTitle, nbit);
#endif
			//
			DesignService.GetJsonMin = LimnorXmlCompiler.GetJsonJs;
			DesignService.GetModalMin = LimnorXmlCompiler.GetModalJs;
			DesignService.Init();
			initTypes();
			//
			VisualProgrammingDesigner.AddExcludedDesigner("Longflow.VSMain.FormViewer");
			VisualProgrammingDesigner.LoadDesigners(BuiltInDesigners);
			//
			ConnectionItem.GetProjectDatabaseConnections = LimnorProject.GetObjectGuidList;
			ConnectionItem.SetProjectDatabaseConnections = LimnorProject.SetObjectGuidList;
			//
			DesignUtil.WriteToOutputWindowDelegate = new DesignUtil.fnWriteToOutputWindow(outputWindow1.AppendMessage);
			//
			tabControl1.Resize += new EventHandler(tabControl1_Resize);
			//
			solutionTree1.VobService = this;
			refreshMruMenus();

			this.Controls.Add(lblCloseTab);
			lblCloseTab.Visible = false;
			lblCloseTab.Left = 100;
			lblCloseTab.Width = 18;
			lblCloseTab.Height = 18;
			lblCloseTab.Font = new Font(lblCloseTab.Font.FontFamily, lblCloseTab.Font.Size, lblCloseTab.Font.Style | FontStyle.Bold);
			lblCloseTab.Text = "X";
			lblCloseTab.TextAlign = ContentAlignment.MiddleCenter;

			lblCloseTab.Top = splitContainer3.Top;
			lblCloseTab.Left = splitContainer3.Left + tabControl1.Width - lblCloseTab.Width;
			lblCloseTab.BringToFront();
			lblCloseTab.MouseEnter += new EventHandler(lblCloseTab_MouseEnter);
			lblCloseTab.MouseLeave += new EventHandler(lblCloseTab_MouseLeave);
			lblCloseTab.Click += new EventHandler(lblCloseTab_Click);
			//
			string ers;
			xToolbox1.ReadFromXmlFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "toolbox.xml"), out ers);
			if (ers.Length > 0)
			{
				MessageBox.Show(ers);
			}
			xToolbox1.ShowToolbox();
			xToolbox1.HideToolbox();
			genericServices = new ServiceMan();
			genericServices.AddService(typeof(VOB.OutputWindow), outputWindow1);
			genericServices.AddService(typeof(System.Windows.Forms.PropertyGrid), this.propertyGrid1);
			xToolbox1.GenericServices = genericServices;
			solutionTree1.ServiceProvider = genericServices;
			//=============================================================

			OnResize(null);
			//ensure the animation will work
			this.xToolbox1.AutoScroll = true;
			this.xToolbox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.xToolbox1.ForecScroll();
			//
			_allTools = xToolbox1.GetAllToolboxItems();
			//
			eventService = new XEventService();
			//
			CustomInitialize();
			//
			solutionTree1.OnSaved += new EventHandler(solutionTree1_OnSaved);
			//
			//===Home=============================================================================== 
			HostControl2 hc = _hostSurfaceManager.GetNewHost(typeof(VOB.NewComponentPrompt));
			this.xToolbox1.Host = hc.DesignerHost;
			TabPage tabpage = new TabPage();
			tabControl1.TabPages.Add(tabpage);
			tabpage.Text = "Home";
			hc.Parent = tabpage;
			hc.Dock = DockStyle.Fill;
			//=======================================================================================
			//restore last configurations
			int w = VobState.MainWinowWidth;
			int h = VobState.MainWinowHeight;
			if (w > 0 && h > 0)
			{
				FormWindowState stt = FormWindowState.Normal;
				try
				{
					stt = VobState.MainWindowState;
				}
				catch
				{
					stt = FormWindowState.Normal;
				}
				if (stt == FormWindowState.Normal)
				{
					int t = VobState.MainWinowTop;
					int l = VobState.MainWinowLeft;
					this.Bounds = new Rectangle(l, t, w, h);
				}
			}
			//
			onSolutionLoaded();
			//
			//add custom shortcuts to this.toolStrip1.Items
			ToolStripSeparator tss = new ToolStripSeparator();
			this.toolStrip1.Items.Add(tss);
		}
		#endregion

		#region Frequent Used Type
		const string XMLATT_Desc = "Desc";
		const string XML_Type = "Type";
		private string mrutypefile()
		{
			return System.IO.Path.Combine(VobUtil.AppDataFolder, "mostusedtypes.xml");
		}
		private void removeMruType(Type t)
		{
			try
			{
				string file = mrutypefile();
				if (File.Exists(file))
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(file);
					if (doc.DocumentElement != null)
					{
						foreach (XmlNode nd in doc.DocumentElement.ChildNodes)
						{
							if (string.Compare(nd.Name, XML_Type, StringComparison.OrdinalIgnoreCase) == 0)
							{
								Type t0 = XmlUtil.GetLibTypeAttribute(nd);
								if (t.Equals(t0))
								{
									doc.DocumentElement.RemoveChild(nd);
									doc.Save(file);
									break;
								}
							}
						}
					}
				}
				for (int i = 0; i < this.toolStrip1.Items.Count; i++)
				{
					ToolStripButton tsb = this.toolStrip1.Items[i] as ToolStripButton;
					if (tsb != null)
					{
						FuTypeData data = tsb.Tag as FuTypeData;
						if (data != null)
						{
							if (t.Equals(data.Type))
							{
								this.toolStrip1.Items.Remove(tsb);
								break;
							}
						}
					}
				}

			}
			catch (Exception e)
			{
				MessageBox.Show(this, e.Message, "Remove frequent used type", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void addMruType(Type t)
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				string file = mrutypefile();
				if (File.Exists(file))
				{
					doc.Load(file);
					if (doc.DocumentElement != null)
					{
						foreach (XmlNode nd in doc.DocumentElement.ChildNodes)
						{
							if (string.Compare(nd.Name, XML_Type, StringComparison.OrdinalIgnoreCase) == 0)
							{
								Type t0 = XmlUtil.GetLibTypeAttribute(nd);
								if (t.Equals(t0))
								{
									return;
								}
							}
						}
					}
				}
				string desc0 = t.FullName;
				object[] vs = t.GetCustomAttributes(typeof(DescriptionAttribute), true);
				if (vs != null && vs.Length > 0)
				{
					for (int i = 0; i < vs.Length; i++)
					{
						DescriptionAttribute da = vs[i] as DescriptionAttribute;
						if (da != null)
						{
							desc0 = da.Description;
							break;
						}
					}
				}
				string desc = VPL.DlgText.GetTextInput(this, desc0, "Description");
				if (desc != null)
				{
					if (doc.DocumentElement == null)
					{
						XmlNode xmlNode = doc.CreateElement("Types");
						doc.AppendChild(xmlNode);
					}
					XmlNode typeNode = doc.CreateElement(XML_Type);
					doc.DocumentElement.AppendChild(typeNode);
					XmlUtil.SetLibTypeAttribute(typeNode, t);
					XmlUtil.SetAttribute(typeNode, XMLATT_Desc, desc);
					doc.Save(file);
					FuTypeData data = new FuTypeData(t, desc);
					ToolStripButton tsb = addFuButton(data);
					tsb.Visible = true;
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(this, e.Message, "Add frequent used types", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private void getMruTypes(bool web)
		{
			if (!_fuTypeLoaded)
			{
				_fuTypeLoaded = true;
				List<FuTypeData> types = new List<FuTypeData>();
				try
				{
					string file = mrutypefile();
					if (File.Exists(file))
					{
						XmlDocument doc = new XmlDocument();
						doc.Load(file);
						if (doc.DocumentElement != null)
						{
							foreach (XmlNode nd in doc.DocumentElement.ChildNodes)
							{
								if (string.Compare(nd.Name, XML_Type, StringComparison.OrdinalIgnoreCase) == 0)
								{
									Type t = XmlUtil.GetLibTypeAttribute(nd);
									types.Add(new FuTypeData(t, XmlUtil.GetAttribute(nd, XMLATT_Desc)));
								}
							}
						}
					}
				}
				catch (Exception e)
				{
					MessageBox.Show(this, e.Message, "Read most used types", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				for (int i = 0; i < types.Count; i++)
				{
					ToolStripButton tsb = addFuButton(types[i]);
					bool isWeb = types[i].Type.GetInterface("IWebClientControl") != null;
					bool include = false;
					if (web)
					{
						if (isWeb)
						{
							include = true;
						}
						else if (!typeof(Control).IsAssignableFrom(types[i].Type))
						{
							include = true;
						}
					}
					else
					{
						if (!isWeb)
						{
							include = true;
						}
					}
					if (!include)
					{
						tsb.Visible = false;
					}
				}
			}
		}
		private ToolStripButton addFuButton(FuTypeData data)
		{
			Image shortcutImg = data.Image;
			ToolStripButton tsb = new ToolStripButton(string.Empty, shortcutImg, customshortcutclick);
			tsb.ToolTipText = data.Description;
			tsb.Tag = data;
			tsb.Visible = true;
			tsb.MouseDown += tsb_MouseDown;
			this.toolStrip1.Items.Add(tsb);
			return tsb;
		}

		void tsb_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				ToolStripButton tsb = sender as ToolStripButton;
				if (tsb != null)
				{
					FuTypeData data = tsb.Tag as FuTypeData;
					if (data != null)
					{
						ContextMenu mnu = new ContextMenu();
						MenuItem mi = new MenuItemWithBitmap("Remove", removeMruTypeClick, Resource1._cancel.ToBitmap());
						mnu.MenuItems.Add(mi);
						mi.Tag = data;
						Rectangle rc = tsb.Bounds;
						mnu.Show(tsb.Owner, new Point(rc.X + e.X, rc.Y + e.Y));
					}
				}
			}
		}
		private void removeMruTypeClick(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				FuTypeData data = mi.Tag as FuTypeData;
				if (data != null)
				{
					if (MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Do you want to remove [{0}] from the toolbar?", data.Type.Name), "Remove frequently used component", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
					{
						removeMruType(data.Type);
					}
				}
			}
		}
		private void customshortcutclick(object sender, EventArgs e)
		{
			int n = tabControl1.SelectedIndex;
			if (n >= 0)
			{
				HostTab tabpage = tabControl1.TabPages[n] as HostTab;
				if (tabpage != null)
				{
					NodeObjectComponent componentData = tabpage.NodeData;
					object c = tabpage.GetHost().CurrentObject;
					if (c != null)
					{
						this.propertyGrid1.CheckItemsSelection(c);
					}
					ILimnorDesignPane xdh = tabpage.GetHost();
					if (xdh != null)
					{
						ToolStripButton tsb = sender as ToolStripButton;
						if (tsb != null)
						{
							FuTypeData shortcuttarget = tsb.Tag as FuTypeData;
							if (shortcuttarget != null)
							{
								IComponent ic = xdh.CreateComponent(shortcuttarget.Type, null);
								xdh.SelectComponent(ic);
							}
						}
						return;
					}
				}
			}
			MessageBox.Show(this, "A visual designer is not active", "Add component", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		#endregion

		#region static members
		static List<Type> BuiltInDesigners
		{
			get
			{
				List<Type> ts = new List<Type>();
				ts.Add(typeof(FormViewer));
				ts.Add(typeof(ObjectExplorerView));
				ts.Add(typeof(EventPathHolder));
				return ts;
			}
		}
		#endregion

		#region Methods
		public SolutionNode LoadSolution(string file, bool updateMRU)
		{
			return openSolution(file, updateMRU);
		}
		public Bitmap GetMaxbuttonNormal()
		{
			return Resource1.maxbuttonNormal;
		}
		public Bitmap GetMaxbuttonHighlight()
		{
			return Resource1.maxbuttonHighlight;
		}
		public Bitmap GetMaxbuttonRestoreNormal()
		{
			return Resource1.maxRestoreNormal;
		}
		public Bitmap GetMaxbuttonRestoreHighlight()
		{
			return Resource1.maxRestoreHighlight;
		}
		#endregion

		#region register known types
		private void initTypes()
		{
			InitKnownTypes.Init();
		}
		#endregion

		#region private methods
		private HostTab getTabPageById(UInt32 classId, Guid projectId)
		{
			for (int i = 0; i < tabControl1.TabPages.Count; i++)
			{
				HostTab tab = tabControl1.TabPages[i] as HostTab;
				if (tab != null)
				{
					ILimnorDesignPane lp = tab.GetHost();
					if (lp.Loader.ClassID == classId && lp.Loader.Project.ProjectGuid == projectId)
					{
						return tab;
					}
				}
			}
			return null;
		}
		void tabControl1_Resize(object sender, EventArgs e)
		{
			lblCloseTab.Top = splitContainer3.Top;
			lblCloseTab.Left = splitContainer3.Left + tabControl1.Width - lblCloseTab.Width;
			lblCloseTab.BringToFront();
		}
		private void closeTabPage(int n)
		{
			HostTab tabpage = tabControl1.TabPages[n] as HostTab;
			if (tabpage != null)
			{
				NodeObjectComponent componentData = tabpage.NodeData;
				object c = tabpage.GetHost().CurrentObject;
				if (c != null)
				{
					this.propertyGrid1.CheckItemsSelection(c);
				}
				ILimnorDesignPane xdh = tabpage.GetHost();
				if (xdh != null)
				{
					xdh.OnClosing();
				}
				if (componentData.Dirty)
				{
					if (MessageBox.Show("Do you want to save the modifications to this object?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						if (xdh != null)
						{
							SetupContentsHolder sch = xdh.Window as SetupContentsHolder;
							if (sch != null)
							{
								sch.OnBeforeSave();
							}
						}
						saveClassToFile(componentData);
					}
				}
				if (xdh != null)
				{
					xdh.OnClosed();
				}
				componentData.Holder = null;
				VobUtil.RootComponentSelected = false;
				VobUtil.CurrentComponentName = "";
				VobUtil.CurrentComponent = null;
			}
			//
			tabControl1.TabPages.RemoveAt(n);
			if (tabControl1.SelectedIndex <= 0)
			{
				if (tabControl1.TabPages.Count > 0)
				{
					tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
				}
			}
		}
		void lblCloseTab_Click(object sender, EventArgs e)
		{
			int n = tabControl1.SelectedIndex;
			if (n >= 0)
			{
				closeTabPage(n);
			}
		}

		void lblCloseTab_MouseLeave(object sender, EventArgs e)
		{
			lblCloseTab.BackColor = this.BackColor;
		}

		void lblCloseTab_MouseEnter(object sender, EventArgs e)
		{
			lblCloseTab.BackColor = System.Drawing.Color.LightBlue;
		}
		private void comboBoxObjects_SelectedObject(object sender, EventArgs e)
		{
			EventArgsObjectSelection ev = e as EventArgsObjectSelection;
			if (ev != null)
			{
				LimnorXmlDesignerLoader2 l2 = LimnorProject.ActiveDesignerLoader as LimnorXmlDesignerLoader2;
				if (l2 != null)
				{
					l2.ViewerHolder.SetComponentSelection(ev.ObjectDisplay.Component);
				}
			}
			else
			{
				EventArgsHtmlElementSelection eh = e as EventArgsHtmlElementSelection;
				if (eh != null)
				{
					LimnorXmlDesignerLoader2 l2 = LimnorProject.ActiveDesignerLoader as LimnorXmlDesignerLoader2;
					if (l2 != null)
					{
						l2.ViewerHolder.OnSelectedHtmlElement(eh.ObjectDisplay.ElementGuid, null);
					}
				}
			}
		}
		/// <summary>
		/// create a new root object
		/// </summary>
		/// <param name="prjData"></param>
		/// <param name="type"></param>
		ILimnorDesignPane createNewObject(ProjectNodeData prjData, Type type, params object[] values)
		{
			try
			{
			}
			catch (Exception err)
			{
				outputWindow1.ShowException(err);
			}
			return null;
		}
		void solutionTree1_OnSaved(object sender, EventArgs e)
		{
			NodeDataSolution nd = sender as NodeDataSolution;
			if (nd != null)
			{
				try
				{
					updateMru(nd.FilePath);
				}
				catch
				{
				}
			}
		}

		private void openMru(object sender, EventArgs e)
		{
			ToolStripMenuItem tm = sender as ToolStripMenuItem;
			if (tm != null && tm.Tag != null)
			{
				string file = tm.Tag.ToString();
				openSolution(file, true);
			}
		}
		SolutionNode openSolution(string file, bool updateMRU)
		{
			try
			{
				if (System.IO.File.Exists(file))
				{
					closeSolutionToolStripMenuItem_Click(null, null);
					SolutionNode slnNode = solutionTree1.LoadFromFile(file);
					if (slnNode != null)
					{
						solutionTree1.ResetModified();
						slnNode.Expand();
						if (slnNode.Nodes.Count > 0)
						{
							for (int i = 0; i < slnNode.Nodes.Count; i++)
							{
								ProjectNode pn = slnNode.Nodes[i] as ProjectNode;
								if (pn != null)
								{
									solutionTree1.SelectedNode = pn;
									break;
								}
							}
						}
						if (updateMRU)
						{
							updateMru(file);
						}
						_solutionLoaded = true;
						onSolutionLoaded();
						return slnNode;
					}
				}
				else
				{
					UIUtil.ShowError(this, "Solution file not found: {0}", file);
				}
			}
			catch (Exception e)
			{
				UIUtil.ShowError(this, e);
			}
			return null;
		}
		/// <summary>
		/// find solution by project in the same folder and open the solution
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		private string openProject(string file)
		{
			closeSolutionToolStripMenuItem_Click(null, null);
			return solutionTree1.OpenProject(file);
		}
		private void updateMenuDisplay()
		{
			const int Undo_Enable = 0;
			const int Undo_Disable = 1;
			const int Redo_Enable = 2;
			const int Redo_Disable = 3;
			const int Edit_Cut = 4;
			const int Edit_Copy = 5;
			const int Edit_Paste = 6;
			const int Edit_Delete = 7;
			IMenuCommandService mnu = activeMenu;
			if (mnu != null)
			{
				bool b;
				MenuCommand undoMenuCommand = mnu.FindCommand(StandardCommands.Undo);
				MenuCommand redoMenuCommand = mnu.FindCommand(StandardCommands.Redo);
				if (undoMenuCommand != null)
				{
					b = undoMenuCommand.Enabled;
				}
				else
				{
					b = false;
				}
				if (b)
				{
					if (Undo_Enable < imgEdit.Images.Count)
						undoToolStripMenuItem.Image = imgEdit.Images[Undo_Enable];
				}
				else
				{
					if (Undo_Disable < imgEdit.Images.Count)
						undoToolStripMenuItem.Image = imgEdit.Images[Undo_Disable];
				}
				//
				undoToolStripMenuItem.Enabled = b;
				if (redoMenuCommand != null)
				{
					b = redoMenuCommand.Enabled;
				}
				else
				{
					b = false;
				}
				if (b)
				{
					if (Redo_Enable < imgEdit.Images.Count)
						redoToolStripMenuItem.Image = imgEdit.Images[Redo_Enable];
				}
				else
				{
					if (Undo_Disable < imgEdit.Images.Count)
						redoToolStripMenuItem.Image = imgEdit.Images[Undo_Disable];
				}
				redoToolStripMenuItem.Enabled = b;
				//
				MenuCommand mc = mnu.FindCommand(StandardCommands.Copy);
				if (mc == null)
				{
					b = false;
				}
				else
				{
					b = mc.Enabled;
				}
				copyToolStripMenuItem.Enabled = b;
				mc = mnu.FindCommand(StandardCommands.Cut);
				if (mc == null)
				{
					b = false;
				}
				else
				{
					b = mc.Enabled;
				}
				cutToolStripMenuItem.Enabled = b;
				mc = mnu.FindCommand(StandardCommands.Delete);
				if (mc == null)
				{
					b = false;
				}
				else
				{
					b = mc.Enabled;
				}
				deleteToolStripMenuItem.Enabled = b;
				mc = mnu.FindCommand(StandardCommands.Paste);
				if (mc == null)
				{
					b = false;
				}
				else
				{
					b = mc.Enabled;
				}
				pasteToolStripMenuItem.Enabled = b;
			}
			else
			{
				if (Undo_Disable < imgEdit.Images.Count)
					undoToolStripMenuItem.Image = imgEdit.Images[Undo_Disable];
				if (Redo_Disable < imgEdit.Images.Count)
					redoToolStripMenuItem.Image = imgEdit.Images[Redo_Disable];
				undoToolStripMenuItem.Enabled = false;
				redoToolStripMenuItem.Enabled = false;
				cutToolStripMenuItem.Enabled = false;
				copyToolStripMenuItem.Enabled = false;
				pasteToolStripMenuItem.Enabled = false;
				deleteToolStripMenuItem.Enabled = false;
			}
			//
			if (imgEdit.Images.Count > Edit_Delete)
			{
				cutToolStripMenuItem.Image = imgEdit.Images[Edit_Cut];
				copyToolStripMenuItem.Image = imgEdit.Images[Edit_Copy];
				pasteToolStripMenuItem.Image = imgEdit.Images[Edit_Paste];
				deleteToolStripMenuItem.Image = imgEdit.Images[Edit_Delete];
			}
			bool br = (_timerBuild == null);
			buildProjectToolStripMenuItem.Enabled = br;
			buildSolutionToolStripMenuItem.Enabled = br;
			buildModifiedWebPagesToolStripMenuItem.Enabled = br;
		}
		private bool getPropertyGridFocus()
		{
			return propertyGrid1.IsInEditing;
		}
		private void sendDeleteToPropertyGridTextEditor(object sender, EventArgs e)
		{
			propertyGrid1.TextEditor_DeleteOne();
		}
		private void CustomInitialize()
		{
			propertyGrid1.AddTextEditorFocusWatcher();
			//
			_hostSurfaceManager = new HostSurfaceManager();//
			_hostSurfaceManager.AddService(typeof(IToolboxService), this.xToolbox1);
			_hostSurfaceManager.AddService(typeof(ILimnorToolbox), this.xToolbox1);
			_hostSurfaceManager.AddService(typeof(SolutionMan.SolutionTree), this.solutionTree1);
			_hostSurfaceManager.AddService(typeof(VOB.OutputWindow), this.outputWindow1);
			_hostSurfaceManager.AddService(typeof(System.Windows.Forms.PropertyGrid), this.propertyGrid1);
			_hostSurfaceManager.AddService(typeof(IEventBindingService), eventService);
			_hostSurfaceManager.AddService(typeof(VOB.InterfaceVOB), this);
			//
			VobState.GetMainPropertyGridFocus = getPropertyGridFocus;
			VobState.SendDeleteToMainPropertyGridTextEditor = sendDeleteToPropertyGridTextEditor;
			//
			_hostSurfaceManager.DesignSurfaceCreated += new DesignSurfaceEventHandler(_hostSurfaceManager_DesignSurfaceCreated);
			_hostSurfaceManager.ActiveDesignSurfaceChanged += new ActiveDesignSurfaceChangedEventHandler(_hostSurfaceManager_ActiveDesignSurfaceChanged);
			//
		}

		void _hostSurfaceManager_ActiveDesignSurfaceChanged(object sender, ActiveDesignSurfaceChangedEventArgs e)
		{
			if (e.NewSurface != e.OldSurface)
			{
				_hostSurfaceManager.ActiveDesignSurface = e.NewSurface;
			}
		}

		void _hostSurfaceManager_DesignSurfaceCreated(object sender, DesignSurfaceEventArgs e)
		{
			_hostSurfaceManager.ActiveDesignSurface = e.Surface;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}
		private void picToolbox_MouseEnter(object sender, EventArgs e)
		{
			xToolbox1.BringToFront();
			xToolbox1.ShowToolBox(sender, e);
		}

		private void panelToolbox_MouseHover(object sender, EventArgs e)
		{
			xToolbox1.HideToolBox(sender, e);
		}
		private void picHide_MouseHover(object sender, EventArgs e)
		{
			xToolbox1.HideToolBox(sender, e);
		}
		private void menuStrip1_MouseEnter(object sender, EventArgs e)
		{
			xToolbox1.HideToolBox(sender, e);
		}
		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (tabControl1.SelectedIndex > 0 && tabControl1.SelectedIndex < tabControl1.TabCount)
			{
				HostTab page = tabControl1.TabPages[tabControl1.SelectedIndex] as HostTab;
				if (page != null)
				{
					ILimnorDesignPane hc = page.GetHost();
					if (hc != null && hc.Loader != null && hc.RootClass != null)
					{
						_hostSurfaceManager.ActiveDesignSurface = hc.Surface2;
						LimnorXmlDesignerLoader2.SetActiveDesigner(hc.Loader);
						LimnorXmlDesignerLoader2.ActiveItemID = hc.Loader.ClassID;
						VPLUtil.CurrentProject = hc.Loader.Project;
						HostSurface hs = hc.Surface2 as HostSurface;
						if (hc.RootClass.Project.IsWebApplication)
							VPLUtil.CurrentRunContext = EnumRunContext.Client;
						else
							VPLUtil.CurrentRunContext = EnumRunContext.Server;
						if (hs != null)
						{
							xToolbox1.Host = hs.DesignerHost;
							hs.Loader.SetRunContext();
							hs.OnSelectionChange();
							if (!hs.Loader.Project.ToolboxLoaded)
							{
								hc.InitToolbox();
							}
							else
							{
								xToolbox1.ShowProjectTab(hs.Loader.Project.ProjectGuid);
							}
						}
						if (hc.RootClass.IsWebPage)
						{
							showWebComponents();
						}
						else if (!hc.RootClass.IsApp)
						{
							showFormComponents();
						}
						else
						{
							hideShortcutComponents(0);
						}
					}
				}
			}
			if (tabControl1.SelectedIndex > 0)
			{
				lblCloseTab.Top = splitContainer3.Top;
				lblCloseTab.Left = splitContainer3.Left + tabControl1.Width - lblCloseTab.Width;
				lblCloseTab.Visible = true;
				lblCloseTab.BringToFront();
			}
			else
			{
				lblCloseTab.Visible = false;
			}
		}
		private void showWebComponents()
		{
			hideShortcutComponents(1);
			if (!_fuTypeLoaded)
			{
				getMruTypes(true);
			}
		}
		private void showFormComponents()
		{
			hideShortcutComponents(2);
			if(!_fuTypeLoaded)
			{
				getMruTypes(false);
			}
		}
		/// <summary>
		/// 0: none
		/// 1: web
		/// 2: form
		/// </summary>
		/// <param name="showType"></param>
		private void hideShortcutComponents(int showType)
		{
			for (int i = 0; i < this.toolStrip1.Items.Count; i++)
			{
				ToolStripButton tsb = this.toolStrip1.Items[i] as ToolStripButton;
				if (tsb != null)
				{
					FuTypeData td = tsb.Tag as FuTypeData;
					if (td != null)
					{
						if (showType == 0)
						{
							tsb.Visible = false;
						}
						else
						{
							bool isWeb = td.Type.GetInterface("IWebClientControl") != null;
							if (showType == 1)
							{
								if (isWeb)
								{
									tsb.Visible = true;
								}
								else if (typeof(Control).IsAssignableFrom(td.Type))
								{
									tsb.Visible = false;
								}
								else
								{
									tsb.Visible = true;
								}
							}
							else if (showType == 2)
							{
								if (isWeb)
								{
									tsb.Visible = false;
								}
								else
								{
									tsb.Visible = true;
								}
							}
							else
							{
								tsb.Visible = false;
							}
						}
					}
				}
			}
		}
		private void splitContainer3_SplitterMoved(object sender, System.Windows.Forms.SplitterEventArgs e)
		{
			tabControl1.Invalidate();
		}
		private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProjectNode pn = getActiveProject();
			if (pn != null)
			{
				propertyGrid1.SelectedObject = pn.PropertyObject;
			}
		}
		string[] getViewers()
		{
			string[] ss = new string[0];
			string file = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "vob.xml");
			if (System.IO.File.Exists(file))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(file);
				XmlNodeList list = doc.DocumentElement.SelectNodes("Viewer");
				ss = new string[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					XmlAttribute xa = list[i].Attributes["file"];
					ss[i] = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), xa.Value);
				}
			}
			return ss;
		}
		private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			updateMenuDisplay();
		}
		private void saveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProjectNode pnode = getActiveProject();
			if (pnode != null)
			{
				ProjectNodeData pdata = pnode.PropertyObject as ProjectNodeData;
				FolderBrowserDialog dlg = new FolderBrowserDialog();
				dlg.Description = "Select a folder to copy the current project to";
				dlg.ShowNewFolderButton = true;
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					string prjFolder = Path.GetDirectoryName(pdata.ProjectFile);
					if (string.Compare(prjFolder, dlg.SelectedPath, StringComparison.OrdinalIgnoreCase) != 0)
					{
						if (MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Do you want to copy project files from [{0}] to [{1}]?", prjFolder, dlg.SelectedPath), "Copy Project", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
						{
							return;
						}
						string webAppFile = null;
						bool appCopied = false;
						if (pdata.Project.IsWebApplication)
						{
							if (pdata.Project.MainClassId > 0)
							{
								webAppFile = Path.GetFileName(pdata.Project.MainComponent.MainComponentFile);
							}
						}
						EnumFileOverwrite o = EnumFileOverwrite.SkipOne;
						try
						{
							Dictionary<string, Guid[]> prjfiles = new Dictionary<string, Guid[]>();
							string[] ss = Directory.GetFiles(prjFolder);
							for (int i = 0; i < ss.Length; i++)
							{
								string tgt = Path.Combine(dlg.SelectedPath, Path.GetFileName(ss[i]));
								if (File.Exists(tgt))
								{
									if (o != EnumFileOverwrite.OverwriteAll)
									{
										if (o == EnumFileOverwrite.SkipAll)
										{
											continue;
										}
										DlgAskFileOverwrite dlgAsk = new DlgAskFileOverwrite();
										dlgAsk.LoadData(tgt);
										dlgAsk.ShowDialog(this);
										o = dlgAsk.Result;
										if (o == EnumFileOverwrite.SkipAll || o == EnumFileOverwrite.SkipOne)
										{
											continue;
										}
									}
								}
								File.Copy(ss[i], tgt, true);
								if (!string.IsNullOrEmpty(webAppFile))
								{
									if (!appCopied)
									{
										if (string.Compare(webAppFile, Path.GetFileName(tgt), StringComparison.OrdinalIgnoreCase) == 0)
										{
											appCopied = true;
										}
									}
								}
								string ext = Path.GetExtension(tgt);
								if (string.Compare(ext, ".lrproj", StringComparison.OrdinalIgnoreCase) == 0)
								{
									XmlDocument doc = new XmlDocument();
									doc.Load(tgt);
									if (doc.DocumentElement != null)
									{
										Guid newGuid = Guid.Empty;
										Guid prjGuid = Guid.Empty;
										foreach (XmlNode nd in doc.DocumentElement.ChildNodes)
										{
											if (string.Compare(nd.Name, "PropertyGroup", StringComparison.OrdinalIgnoreCase) == 0)
											{
												foreach (XmlNode nd2 in nd.ChildNodes)
												{
													if (string.Compare(nd2.Name, "ProjectGuid", StringComparison.OrdinalIgnoreCase) == 0)
													{
														prjGuid = new Guid(nd2.InnerText);
														newGuid = Guid.NewGuid();
														nd2.InnerText = newGuid.ToString("B", CultureInfo.InvariantCulture);
														break;
													}
												}
											}
											if (prjGuid != Guid.Empty)
											{
												break;
											}
										}
										if (prjGuid != Guid.Empty)
										{
											doc.Save(tgt);
											prjfiles.Add(tgt.ToLowerInvariant(), new Guid[] { prjGuid, newGuid });
										}
									}
								}
							}
							foreach (KeyValuePair<string, Guid[]> kv in prjfiles)
							{
								string vob = string.Format(CultureInfo.InvariantCulture, "{0}.vob", kv.Key);
								if (File.Exists(vob))
								{
									XmlDocument doc = new XmlDocument();
									doc.Load(vob);
									if (doc.DocumentElement != null)
									{
										bool found = false;
										foreach (XmlNode nd in doc.DocumentElement.ChildNodes)
										{
											if (string.Compare(nd.Name, "VobGroup", StringComparison.OrdinalIgnoreCase) == 0)
											{
												foreach (XmlNode nd2 in nd.ChildNodes)
												{
													if (string.Compare(nd2.Name, "ProjectGuid", StringComparison.OrdinalIgnoreCase) == 0)
													{
														nd2.InnerText = kv.Value[1].ToString("D", CultureInfo.InvariantCulture);
														found = true;
														break;
													}
												}
											}
											if (found)
											{
												break;
											}
										}
										XmlNode websNode = doc.DocumentElement.SelectSingleNode("Webs");
										if (websNode != null)
										{
											doc.DocumentElement.RemoveChild(websNode);
											found = true;
										}
										if (found)
										{
											doc.Save(vob);
										}
									}
								}
								string[] sols = Directory.GetFiles(Path.GetDirectoryName(kv.Key), "*.LimnorMain_sln");
								if (sols != null && sols.Length > 0)
								{
									string g = kv.Value[0].ToString("B", CultureInfo.InvariantCulture);
									for (int i = 0; i < sols.Length; i++)
									{
										StreamReader sr = new StreamReader(sols[i]);
										Encoding sn = sr.CurrentEncoding;
										string s = sr.ReadToEnd();
										sr.Close();
										if (!string.IsNullOrEmpty(s))
										{
											int p = s.IndexOf(g, StringComparison.OrdinalIgnoreCase);
											if (p > 0)
											{
												s = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", s.Substring(0, p), kv.Value[1].ToString("B", CultureInfo.InvariantCulture).ToUpper(CultureInfo.InvariantCulture), s.Substring(p + g.Length));
												StreamWriter sw = new StreamWriter(sols[i], false, sn);
												sw.Write(s);
												sw.Close();
												break;
											}
										}
									}
								}
							}
							if (!string.IsNullOrEmpty(webAppFile) && appCopied)
							{
								string tgt = Path.Combine(dlg.SelectedPath, webAppFile);
								if (File.Exists(tgt))
								{
									XmlDocument doc = new XmlDocument();
									doc.Load(tgt);
									if (doc.DocumentElement != null)
									{
										XmlNode nd = doc.DocumentElement.SelectSingleNode("Property[@name=\"WebSiteName\"]");
										if (nd != null)
										{
											doc.DocumentElement.RemoveChild(nd);
											doc.Save(tgt);
										}
									}
								}
							}
							MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Project files copied from [{0}] to [{1}].", prjFolder, dlg.SelectedPath), "Copy Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
						catch (Exception err)
						{
							MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Error copying project. {0}", err.Message), "Copy Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
			}
		}
		private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			executeBuild(saveProjectToolStripMenuItemHandler, this, EventArgs.Empty);
		}
		private void saveProjectToolStripMenuItemHandler(object sender, EventArgs e)
		{
			askSaveSolution(false);
		}
		private ProjectNode getActiveProject()
		{
			HostTab tp = tabControl1.SelectedTab as HostTab;
			if (tp != null)
			{
				return solutionTree1.GetProjectNodeByGuid(tp.NodeData.Class.Project.ProjectGuid);
			}
			else
			{
				if (LimnorProject.ActiveProject != null)
				{
					SolutionNode slnNode = solutionTree1.GetCurrentSolution();
					ProjectNode[] projects = slnNode.GetProjectBuilderOrder();
					for (int i = 0; i < projects.Length; i++)
					{
						if (string.Compare(LimnorProject.ActiveProject.ProjectFile, projects[i].Project.ProjectFile, StringComparison.OrdinalIgnoreCase) == 0)
						{
							return projects[i];
						}
					}
				}
				return solutionTree1.ActiveProjectNode;
			}
		}
		
		private void saveActiveProject()
		{
			ProjectNode pnode = getActiveProject();
			saveProject(pnode);
		}
		private void saveProject(ProjectNode pnode)
		{
			if (pnode != null)
			{
				this.propertyGrid1.CheckItemsSelection();
				for (int i = 0; i < tabControl1.TabCount; i++)
				{
					HostTab page = tabControl1.TabPages[i] as HostTab;
					if (page != null)
					{
						NodeObjectComponent componentData = page.NodeData;
						if (componentData.Class.Project.ProjectGuid == pnode.Project.ProjectGuid)
						{
							if (componentData.Dirty)
							{
								ILimnorDesignPane xdh = page.GetHost();
								if (xdh != null)
								{
									SetupContentsHolder sch = xdh.Window as SetupContentsHolder;
									if (sch != null)
									{
										sch.OnBeforeSave();
									}
								}
								saveClassToFile(componentData);
								page.ResetTitle();
							}
						}
					}
				}
			}
		}
		private CompileResults buildProject(ProjectNode pn, string cfg, bool increment)
		{
			CompileResults cs = new CompileResults();
			LimnorXmlCompiler builder = null;
			ProjectNodeData pnd = pn.PropertyObject as ProjectNodeData;
			try
			{
				builder = new LimnorXmlCompiler(pnd.ProjectFile, this);
			}
			catch (Exception err2)
			{
				MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Error creating ILimnorBuilder from [LimnorXmlCompiler]. {0} ", err2.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				builder = null;
				cs.HasErrors = true;
			}
			if (builder != null)
			{
				if (increment)
				{
					if (!pnd.Project.IsNewProject)
					{
						builder.SetIncrement();
					}
				}
				builder.AddService(typeof(IOutputWindow), outputWindow1);
				builder.SetProject(pnd.Project, cfg);
				if (builder.Execute())
				{
					cs.SkippedComponents = builder.SkippedClassCount;
					cs.CompiledComponents = builder.CompiledClassCount;
					outputWindow1.AppendMessage("Finished building [{0}]. Succeed ===============", pnd.Project.ProjectFile);
					if (!builder.CopyBinFileFailed)
					{
						pnd.Project.SetProjectNamedData<bool>("JustCompiled", true);
					}
				}
				else
				{
					cs.HasErrors = true;
					outputWindow1.AppendMessage("Finished building [{0}]. Failed ===============", pnd.Project.ProjectFile);
				}
				cs.CopyBinFileFailed = builder.CopyBinFileFailed;
			}
			return cs;
		}
		private Timer _timerBuild;
		private EventHandler _buildHandler;
		private object _buildSender;
		private EventArgs _buildArgs;
		private int _buildWaitCount;
		private void executeBuild(EventHandler eh, object sender, EventArgs e)
		{
			buildProjectToolStripMenuItem.Enabled = false;
			buildSolutionToolStripMenuItem.Enabled = false;
			buildModifiedWebPagesToolStripMenuItem.Enabled = false;
			for (int i = 0; i < tabControl1.TabCount; i++)
			{
				HostTab ht = tabControl1.TabPages[i] as HostTab;
				if (ht != null)
				{
					ILimnorDesignPane pane = ht.GetHost();
					if (pane != null)
					{
						if (pane.PaneHolder != null && pane.PaneHolder.IsInHtmlEditor())
						{
							WebPage wp = pane.RootClass.ObjectInstance as WebPage;
							if (wp != null)
							{
								wp.OnBeforeIDErun();
							}
						}
					}
				}
			}
			if (_timerBuild == null)
			{
				_timerBuild = new Timer();
				_timerBuild.Interval = 10;
				_timerBuild.Enabled = false;
				_timerBuild.Tick += _timerBuild_Tick;
				_buildArgs = e;
				_buildHandler = eh;
				_buildSender = sender;
				_buildWaitCount = 0;
				_timerBuild.Enabled = true;
			}
		}

		void _timerBuild_Tick(object sender, EventArgs e)
		{
			_timerBuild.Enabled = false;
			bool canBuild = (tabControl1.TabPages.Count<=1);
			if (!canBuild)
			{
				canBuild = true;
				for (int i = 0; i < tabControl1.TabPages.Count; i++)
				{
					HostTab page = tabControl1.TabPages[i] as HostTab;
					if (page != null)
					{
						ILimnorDesignPane hc = page.GetHost();
						if (hc != null)
						{
							if (!hc.CanBuild)
							{
								canBuild = false;
								break;
							}
						}
					}
				}
			}
			if (canBuild)
			{
				try
				{
					_buildHandler(_buildSender, _buildArgs);
				}
				finally
				{
					_timerBuild = null;
					buildProjectToolStripMenuItem.Enabled = true;
					buildSolutionToolStripMenuItem.Enabled = true;
					buildModifiedWebPagesToolStripMenuItem.Enabled = true;
				}
			}
			else
			{
				_buildWaitCount++;
				if (_buildWaitCount > 10)
				{
					MessageBox.Show(this, "Timeout waiting for the designer to finish saving changes. You may close all designers and try it again.", "Build", MessageBoxButtons.OK, MessageBoxIcon.Error);
					_timerBuild = null;
					buildProjectToolStripMenuItem.Enabled = true;
					buildSolutionToolStripMenuItem.Enabled = true;
					buildModifiedWebPagesToolStripMenuItem.Enabled = true;
				}
				else
				{
					_timerBuild.Interval = 300;
					_timerBuild.Enabled = true;
				}
			}
		}
		private void buildProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			executeBuild(buildProjectToolStripMenuItemHandler, sender, e);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buildProjectToolStripMenuItemHandler(object sender, EventArgs e)
		{
			askSaveSolution(false);
			ProjectNode pnode = getActiveProject();
			if (pnode != null)
			{
				if (VobState.ClearBuildLog)
				{
					clearBuildLog();
				}
				bool increment = false;
				EventArgCancel evc = e as EventArgCancel;
				if (evc != null)
				{
					increment = true;
				}
				CompileResults compiled = compileProject(pnode, increment);
				if (evc != null)
				{
					if (compiled.CompiledComponents > 1 && compiled.SkippedComponents > 0)
					{
						evc.Message = string.Format(CultureInfo.InvariantCulture, "Compilation finished successfully.\r\n {0} component(s) compiled.\r\n {1} component(s) skipped.\r\nIf the application does not run as you expected then make a full build and try again.", compiled.CompiledComponents, compiled.SkippedComponents);
					}
					else
					{
						evc.Message = string.Empty;
					}
					if (compiled.CopyBinFileFailed || compiled.HasErrors)
					{
						evc.Cancel = true;
					}
				}
			}
		}
		void compileNext(ProjectNode pnode)
		{
			compileProject(pnode, false);
		}
		void ad2_DomainUnload(object sender, EventArgs e)
		{
		}
		public string BuildConfig
		{
			get
			{
				string cfg = toolStripComboBoxConfig.Text;
				if (string.IsNullOrEmpty(cfg))
				{
					cfg = "Release";
				}
				return cfg;
			}
		}
		public bool DebugMode
		{
			get
			{
				return string.Compare(BuildConfig, "Debug", StringComparison.OrdinalIgnoreCase) == 0;
			}
		}
		CompileResults compileProject(ProjectNode pnode, bool increment)
		{
			ProjectNodeData pdata = pnode.PropertyObject as ProjectNodeData;
			if (pdata.Dirty)
			{
				pdata.Save();
			}
			object curObj = propertyGrid1.SelectedObject;
			propertyGrid1.SelectedObject = null;
			string cfg = BuildConfig;
			_compilerErrors = new StringCollection();
			CompileResults cs = buildProject(pnode, cfg, increment);
			if (_compilerErrors.Count > 0)
			{
				MathNode.Log(_compilerErrors);
			}
			propertyGrid1.SelectedObject = curObj;
			return cs;
		}
		private void clearBuildLog()
		{
			ITraceLog log = (ITraceLog)MathNode.GetService(typeof(ITraceLog));
			if (System.IO.File.Exists(log.LogFile))
			{
				try
				{
					System.IO.File.Delete(log.LogFile);
				}
				catch (Exception er)
				{
					VOB.OutputWindow.ShowException2(er);
				}
			}
		}
		/// <summary>
		/// show build log
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void logFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				ITraceLog log = (ITraceLog)MathNode.GetService(typeof(ITraceLog));
				if (File.Exists(log.LogFile))
				{
					showLogfile(log.LogFile);
				}
				else
				{
					MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Log file does not exist:{0}", log.LogFile), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception er)
			{
				VOB.OutputWindow.ShowException2(er);
			}
		}
		private void setSelectedClassId(UInt32 id, string name)
		{
			ToolboxItemXType.SelectedToolboxClassId = id;
			ToolboxItemXType.SelectedToolboxClassName = name;
			ToolboxItemXType.SelectedToolboxType = null;
			ToolboxItemXType.SelectedToolboxTypeKey = null;
			ToolboxItemXType.SelectedToolboxTypeKey = string.Empty;
		}
		private void setSelectedType(Type t)
		{
			ToolboxItemXType.SelectedToolboxTypeKey = string.Empty;
			ToolboxItemXType.SelectedToolboxClassId = 0;
			ToolboxItemXType.SelectedToolboxClassName = string.Empty;
			ToolboxItemXType.SelectedToolboxType = t;
		}
		private void showLogfile(string logfile)
		{
			try
			{
				System.Diagnostics.Process p = new System.Diagnostics.Process();
				p.StartInfo.FileName = "notepad.exe";
				if (System.IO.File.Exists(logfile))
				{
					p.StartInfo.Arguments = logfile;
				}
				p.Start();
			}
			catch (Exception er)
			{
				VOB.OutputWindow.ShowException2(er);
			}
		}
		private void buildLogToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				ITraceLog log = (ITraceLog)MathNode.GetService(typeof(ITraceLog));
				string f = Path.Combine(Path.GetDirectoryName(log.ErrorFile), MathNode.FILE_DEFAULT_COMPILERLOG);
				if (File.Exists(f))
				{
					showLogfile(f);
				}
				else
				{
					MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Build Log file does not exist:{0}", f), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception er)
			{
				VOB.OutputWindow.ShowException2(er);
			}
		}
		private void errorLogToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				ITraceLog log = (ITraceLog)MathNode.GetService(typeof(ITraceLog));
				if (File.Exists(log.ErrorFile))
				{
					showLogfile(log.ErrorFile);
				}
				else
				{
					MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Error log file does not exist:{0}", log.ErrorFile), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception er)
			{
				VOB.OutputWindow.ShowException2(er);
			}
		}

		private void compilerErrorLogToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				ITraceLog log = (ITraceLog)MathNode.GetService(typeof(ITraceLog));
				string f = Path.Combine(Path.GetDirectoryName(log.ErrorFile), string.Format(CultureInfo.InvariantCulture, "{0}.err", MathNode.FILE_DEFAULT_COMPILERLOG));
				if (File.Exists(f))
				{
					showLogfile(f);
				}
				else
				{
					MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Build Error file does not exist:{0}", f), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception er)
			{
				VOB.OutputWindow.ShowException2(er);
			}
		}

		private void databaseLogToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				string f = EasyQuery.LogFile;
				if (File.Exists(f))
				{
					showLogfile(f);
				}
				else
				{
					MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Database log file does not exist:{0}", f), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception er)
			{
				VOB.OutputWindow.ShowException2(er);
			}
		}

		private void addMathLibToolStripMenuItem_Click(object sender, EventArgs e)
		{
			dlgEditLibrary dlg = new dlgEditLibrary();
			dlg.LoadData("Math Library Editor", "MathTypes.xml", typeof(MathNode));
			dlg.ShowDialog(this);
		}

		private void openSolutionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Select Limnor solution file";
			dlg.Filter = string.Format(CultureInfo.InvariantCulture, "Limnor solution|*.{0}", NodeDataSolution.FILE_EXT_SOLUTION);
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				openSolution(dlg.FileName, true);
				updateMru(dlg.FileName);
			}
		}

		private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			closeSolutionToolStripMenuItem_Click(null, null);
			if (solutionTree1.CreateNewProject(null))
			{
				if (solutionTree1.SolutionNodeObject != null)
				{
					if (!string.IsNullOrEmpty(solutionTree1.SolutionNodeObject.Filename.Filename))
					{
						updateMru(solutionTree1.SolutionNodeObject.Filename.Filename);

					}
				}
				_solutionLoaded = true;
				_projectLoaded = true;
				onSolutionLoaded();
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Select Limnor project file";
			dlg.Filter = "Limnor project|*." + NodeDataSolution.FILE_EXT_PROJECT;
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				string sSolution = openProject(dlg.FileName);
				if (!string.IsNullOrEmpty(sSolution))
				{
					updateMru(sSolution);
					_projectLoaded = true;
					_solutionLoaded = true;
					onSolutionLoaded();
				}
			}
		}
		private bool askSaveSolution(bool ask, params bool[] vs)
		{
			bool closing = false;
			if (vs != null && vs.Length > 0 && vs[0])
			{
				closing = true;
			}
			bool bChanged = xToolbox1.Changed;
			this.propertyGrid1.CheckItemsSelection();
			SolutionNode sn = solutionTree1.GetCurrentSolution();
			if (sn != null)
			{
				if (sn.Dirty)
				{
					bChanged = true;
				}
				if (!bChanged)
				{
					for (int i = 0; i < sn.Nodes.Count; i++)
					{
						ProjectNode pn = sn.Nodes[i] as ProjectNode;
						if (pn != null)
						{
							if (pn.Dirty)
							{
								bChanged = true;
								break;
							}
						}
					}
				}
			}
			//export all html pages
			for (int i = 0; i < tabControl1.TabCount; i++)
			{
				HostTab ht = tabControl1.TabPages[i] as HostTab;
				if (ht != null)
				{
					ILimnorDesignPane pane = ht.GetHost();
					if (pane != null)
					{
						pane.OnClosing();
						if (pane.Loader.DataModified)
						{
							bChanged = true;
						}
					}
				}
			}
			if (!bChanged)
			{
				for (int i = 0; i < tabControl1.TabCount; i++)
				{
					HostTab page = tabControl1.TabPages[i] as HostTab;
					if (page != null)
					{
						if (page.Changed)
						{
							bChanged = true;
							break;
						}
					}
				}
			}
			if (bChanged)
			{
				DialogResult ret = System.Windows.Forms.DialogResult.Yes;
				if (closing || ask)
				{
					ret = MessageBox.Show(this, "Do you want to save modifications?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
					if (ret == System.Windows.Forms.DialogResult.Cancel)
					{
						return false;
					}
				}
				if (ret  == DialogResult.Yes)
				{
					if (xToolbox1.Changed)
					{
						xToolbox1.SaveCustomToolboxTabs();
					}
					if (sn != null)
					{
						if (sn.Dirty)
						{
							for (int i = 0; i < sn.Nodes.Count; i++)
							{
								ProjectNode pn = sn.Nodes[i] as ProjectNode;
								if (pn != null)
								{
									saveProject(pn);
								}
							}
							NodeDataSolution nds = sn.PropertyObject as NodeDataSolution;
							updateMru(nds.Filename.Filename);
							solutionTree1.Save();
						}
						for (int i = 0; i < sn.Nodes.Count; i++)
						{
							ProjectNode pn = sn.Nodes[i] as ProjectNode;
							if (pn != null)
							{
								if (pn.Dirty)
								{
									saveProject(pn);
								}
							}
						}
					}
					for (int i = 0; i < tabControl1.TabCount; i++)
					{
						HostTab page = tabControl1.TabPages[i] as HostTab;
						if (page != null)
						{
							NodeObjectComponent componentData = page.NodeData;
							if (componentData.Dirty)
							{
								saveClassToFile(componentData);
								page.ResetTitle();
							}
						}
					}
				}
			}
			return true;
		}
		private void saveClassToFile(NodeObjectComponent componentData)
		{
			componentData.Holder.Loader.SaveHtmlFile();
			componentData.Class.Save();
			componentData.Dirty = false;
		}
		private void closeSolution()
		{
			FormWarning.CloseMessageForm();
			askSaveSolution(true);
			while (tabControl1.TabCount > 0)
			{
				bool bFound = false;
				for (int i = 0; i < tabControl1.TabCount; i++)
				{
					HostTab page = tabControl1.TabPages[i] as HostTab;
					if (page != null)
					{
						closeTabPage(i);
						bFound = true;
						break;
					}
				}
				if (!bFound)
				{
					break;
				}
			}
			solutionTree1.CloseSolution();
			propertyGrid1.SelectedObject = null;
			_projectLoaded = false;
			_solutionLoaded = false;
			onSolutionLoaded();
			xToolbox1.RemoveProjectTabs();
			LimnorProject.ClearTypeProjectData();
		}
		private ILimnorDesignPane openDesigner(NodeObjectComponent dataComponent)
		{
			_openingComponent = true;
			//load XML file into host, return the Control hosting all the views
			DesignUtil.LogIdeProfile("Reload component:{0}", dataComponent);
			dataComponent.Class.ReloadXmlFile();
			DesignUtil.LogIdeProfile("Create designer component:{0}", dataComponent);
			ILimnorDesignPane xdh = _hostSurfaceManager.CreateDesigner(dataComponent.Class);
			if (xdh != null)
			{
				DesignUtil.LogIdeProfile("Activate component designer");
				dataComponent.Holder = xdh;
				//create a new tab
				HostTab tabpage = new HostTab(xdh, dataComponent);
				this.tabControl1.TabPages.Add(tabpage);
				this.tabControl1.SelectedIndex = this.tabControl1.TabPages.Count - 1;
				this.outputWindow1.AppendMessage("Opened " + dataComponent.Name);
				_solutionLoaded = true;
				_projectLoaded = true;
				onSolutionLoaded();
				onActiveProjectChanged();
				addToolboxItemToolStripMenuItem.Enabled = true;
				LimnorXmlDesignerLoader2.SetActiveDesigner(xdh.Loader);
			}
			DesignUtil.LogIdeProfile("Finish opening component");
			_openingComponent = false;
			return xdh;
		}
		private void closeSolutionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			closeSolution();
		}
		private void buildSolutionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			executeBuild(buildSolutionToolStripMenuItemHandler, sender, e);
		}
		private void buildSolutionToolStripMenuItemHandler(object sender, EventArgs e)
		{
			askSaveSolution(false);
			object curObj = propertyGrid1.SelectedObject;
			propertyGrid1.SelectedObject = null;
			SolutionNode slnNode = solutionTree1.GetCurrentSolution();
			ProjectNode[] projects = slnNode.GetProjectBuilderOrder();
			if (projects.Length > 0)
			{
				if (VobState.ClearBuildLog)
				{
					clearBuildLog();
				}
				string cfg = toolStripComboBoxConfig.Text;
				if (string.IsNullOrEmpty(cfg))
				{
					cfg = "Release";
				}
				outputWindow1.AppendMessage("Build solution starts ===");
				_compilerErrors = new StringCollection();
				for (int i = 0; i < projects.Length; i++)
				{
					if (slnNode.IsProjectBuildIncluded(projects[i]))
					{
						buildProject(projects[i], cfg, false);
					}
				}
				outputWindow1.AppendMessage("Build solution ends ===");
				if (_compilerErrors.Count > 0)
				{
					MathNode.Log(_compilerErrors);
				}
			}
			if (propertyGrid1 != null)
			{
				propertyGrid1.SelectedObject = curObj;
			}
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutBox1 dlg = new AboutBox1();
			dlg.ShowDialog(this);
		}
		private void toolStripButtonRun_Click(object sender, EventArgs e)
		{
			ProjectNode prjNode = getActiveProject();
			if (prjNode != null)
			{
				ProjectNodeData prjData = prjNode.PropertyObject as ProjectNodeData;
				if (prjData != null)
				{
					LimnorProject prj = prjData.Project;
					if (prj != null)
					{
						executeBuild(startDebugToolStripMenuItem_Click, sender, e);
					}
				}
			}
		}
		private void startDebugToolStripMenuItem_Click(object sender, EventArgs e)
		{
			startWithoutDebuggingToolStripMenuItem_Click(sender, e);
		}
		private static void DeleteDirectory(string target_dir)
		{
			string[] files = Directory.GetFiles(target_dir);
			string[] dirs = Directory.GetDirectories(target_dir);
			foreach (string file in files)
			{
				File.SetAttributes(file, FileAttributes.Normal);
				File.Delete(file);
			}
			foreach (string dir in dirs)
			{
				DeleteDirectory(dir);
			}
			Directory.Delete(target_dir, false);
		}
		private bool _startingDebug;
		private void startWithoutDebuggingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			incrementBuild(true);
		}
		private void incrementBuild(bool run)
		{
			if (_startingDebug)
			{
				MessageBox.Show(this, "The project is already started.", "Limnor Studio", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			_startingDebug = true;
			string msg = null;
			try
			{
				ProjectNode prjNode = getActiveProject();
				ProjectNodeData prjData = prjNode.PropertyObject as ProjectNodeData;
				if (_debugProcess != null && prjData.Project.ProjectType != EnumProjectType.WebAppAspx && prjData.Project.ProjectType != EnumProjectType.WebAppPhp)
				{
					bool finished = false;
					try
					{
						if (_debugProcess.HasExited)
						{
							finished = true;
						}
					}
					catch (InvalidOperationException)
					{
						finished = true;
					}
					catch (Exception err)
					{
						MessageBox.Show(this, err.Message, "Debug", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
					if (finished)
					{
						try
						{
							_debugProcess.Dispose();
						}
						catch
						{
						}
						_debugProcess = null;
					}
				}
				else
				{
					_debugProcess = null;
				}
				if (_debugProcess == null)
				{
					EventArgCancel evc = new EventArgCancel(true);
					if (prjData.Project.ProjectType != EnumProjectType.WebAppPhp && run && prjData.Project.GetProjectNamedData<bool>("JustCompiled"))
					{
					}
					else
					{
						buildProjectToolStripMenuItemHandler(this, evc);
					}
					if (run && !evc.Cancel)
					{
						_debugProcess = new System.Diagnostics.Process();
						_debugProcess.EnableRaisingEvents = true;
						_debugProcess.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(_debugProcess_ErrorDataReceived);
						_debugProcess.Exited += new EventHandler(_debugProcess_Exited);
						
						if (prjNode != null)
						{
							if (prjData.Project.ProjectType == EnumProjectType.WebAppAspx || prjData.Project.ProjectType == EnumProjectType.WebAppPhp)
							{
								string file = prjData.Project.StartFile;
								if (string.IsNullOrEmpty(file))
								{
									msg = "Start web page not given";
								}
								else
								{
									if (System.IO.File.Exists(file))
									{
										_debugProcess.StartInfo.FileName = prjData.Project.GetTestUrl(this, Path.GetFileName(file));
										if (prjData.Project.ProjectType == EnumProjectType.WebAppAspx)
										{
											string wn = prjData.Project.GetWebSiteName(this);
											if (!string.IsNullOrEmpty(wn))
											{
												string sDir = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.System));
												if (IntPtr.Size == 4)
												{
													sDir = Path.Combine(sDir, @"Microsoft.NET\Framework\v2.0.50727\Temporary ASP.NET Files");
												}
												else
												{
													sDir = Path.Combine(sDir, @"Microsoft.NET\Framework64\v2.0.50727\Temporary ASP.NET Files");
												}
												sDir = Path.Combine(sDir, wn);
												if (Directory.Exists(sDir))
												{
													try
													{
														DeleteDirectory(sDir);
													}
													catch
													{
													}
												}
											}
											Process cmd = new Process();
											cmd.StartInfo = new ProcessStartInfo("iisreset");
											cmd.Start();
											cmd.WaitForExit();
										}
										else
										{
											if (!string.IsNullOrEmpty(evc.Message))
											{
												MessageBox.Show(this, evc.Message, "Limnor Studio", MessageBoxButtons.OK, MessageBoxIcon.Information);
											}
										}
										_debugProcess.Start();
									}
									else
									{
										msg = XmlSerialization.FormatString("Start web page [{0}] does not exist", file);
									}
								}
							}
							else if (prjData.Project.ProjectType == EnumProjectType.ClassDLL)
							{
								msg = "A DLL Project cannot be executed directly";
							}
							else
							{
								string file = prjData.GetProgramFullpath(DebugMode);
								if (!string.IsNullOrEmpty(file))
								{
									if (System.IO.File.Exists(file))
									{
										string ext = System.IO.Path.GetExtension(file);
										if (string.Compare(ext, ".exe", StringComparison.OrdinalIgnoreCase) == 0)
										{
											_debugProcess.StartInfo.FileName = file;
											_debugProcess.Start();
										}
										else
										{
											msg = XmlSerialization.FormatString("Project file is not an executeable: {0}. Only an EXE file can be started.", file);
										}
									}
									else
									{
										msg = XmlSerialization.FormatString("Project file not created: {0}", file);
									}
								}
								else
								{
									msg = "Project file not created";
								}
							}
						}
						else
						{
							msg = "No project is selected";
						}
					}
				}
				else
				{
					MessageBox.Show(this, "The last debugging process has not fully finished", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
			catch (Exception err)
			{
				msg = DesignerException.FormExceptionText(err);
			}
			finally
			{
				_startingDebug = false;
			}
			if (!string.IsNullOrEmpty(msg))
			{
				UIUtil.ShowError(this, msg);
			}
		}

		void _debugProcess_Exited(object sender, EventArgs e)
		{
			if (_debugProcess != null)
			{
				_debugProcess.Dispose();
			}
			_debugProcess = null;
		}

		void _debugProcess_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
		{
			MessageBox.Show(this, e.Data, "Debug error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void saveProjectToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			executeBuild(saveProjectToolStripMenuItemHandler, this, EventArgs.Empty);
		}

		private void toolStripButtonSaveSolution_Click(object sender, EventArgs e)
		{
			executeBuild(toolStripButtonSaveSolutionHandler, this, EventArgs.Empty);
		}
		private void toolStripButtonSaveSolutionHandler(object sender, EventArgs e)
		{
			askSaveSolution(false);
		}

		private void toolStripButtonNewProject_Click(object sender, EventArgs e)
		{
			newProjectToolStripMenuItem_Click(sender, e);
		}
		private void projectConvertToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DlgCopyConvertProject dlg = new DlgCopyConvertProject();
			dlg.ShowDialog(this);
		}
		private void insertIconsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DlgInsertIcons dlg = new DlgInsertIcons();
			dlg.ShowDialog(this);
		}
		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			dlgOptions dlg = new dlgOptions();
			dlg.ShowDialog(this);
			DesignUtil.SetEnableIdeProfiling(VobOptions.EnableIdeProfiling());
		}
		private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}
		private void projectBuildOrderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SolutionNode sl = solutionTree1.GetCurrentSolution();
			if (sl != null)
			{
				sl.ChangeProjectBuildOrder();
			}
		}
		private void toolStripButtonSaveProject_Click(object sender, EventArgs e)
		{
			executeBuild(toolStripButtonSaveProjectHandler, this, EventArgs.Empty);
		}
		private void toolStripButtonSaveProjectHandler(object sender, EventArgs e)
		{
			saveActiveProject();
		}

		private void createProjectItem(string item)
		{
			ProjectNode pn = getActiveProject();
			if (pn != null)
			{
				solutionTree1.CreateNewProjectItem(pn, item);
			}
		}
		private void addWindowsFormToolStripMenuItem_Click(object sender, EventArgs e)
		{
			createProjectItem("Form");
		}

		private void addUserControlToolStripMenuItem_Click(object sender, EventArgs e)
		{
			createProjectItem("UserControl");
		}

		private void addComponentToolStripMenuItem_Click(object sender, EventArgs e)
		{
			createProjectItem("Component");
		}

		private void addClassToolStripMenuItem_Click(object sender, EventArgs e)
		{
			createProjectItem("Class");
		}
		#endregion

		#region Main Menu
		/// <summary>
		/// open/close solution
		/// </summary>
		/// <param name="loaded"></param>
		private void onSolutionLoaded()
		{
			configurationToolStripMenuItem.Visible = false;
			stepIntoToolStripMenuItem.Visible = false;
			stepOverToolStripMenuItem.Visible = false;
			windowToolStripMenuItem.Visible = false;
			//
			toolStripButtonRun.Enabled = _projectLoaded;
			projectToolStripMenuItem.Enabled = _projectLoaded;
			startDebugToolStripMenuItem.Enabled = _projectLoaded;
			startWithoutDebuggingToolStripMenuItem.Enabled = _projectLoaded;
			addWindowsFormToolStripMenuItem.Enabled = _projectLoaded;
			addUserControlToolStripMenuItem.Enabled = _projectLoaded;
			addComponentToolStripMenuItem.Enabled = _projectLoaded;
			addClassToolStripMenuItem.Enabled = _projectLoaded;
			propertiesToolStripMenuItem.Enabled = _projectLoaded;
			saveProjectToolStripMenuItem.Enabled = _projectLoaded;
			saveProjectAsToolStripMenuItem.Enabled = _projectLoaded;
			buildProjectToolStripMenuItem.Enabled = _projectLoaded;
			toolStripButtonSaveProject.Enabled = _projectLoaded;
			//
			buildConfigurationToolStripMenuItem.Enabled = _solutionLoaded;
			closeSolutionToolStripMenuItem.Enabled = _solutionLoaded;
			buildSolutionToolStripMenuItem.Enabled = _solutionLoaded;
			toolStripButtonSaveSolution.Enabled = _solutionLoaded;
			saveSolutionToolStripMenuItem.Enabled = _solutionLoaded;
			closetoolStripButton.Enabled = _solutionLoaded;
			projectBuildOrderToolStripMenuItem.Enabled = _solutionLoaded;
			//
			toolStripButtonNewProject.Enabled = !_solutionLoaded;
			newSolutionToolStripMenuItem.Enabled = !_solutionLoaded; //this is for project
			openToolStripMenuItem.Enabled = !_solutionLoaded;
			//
			SolutionNode sn = solutionTree1.GetCurrentSolution();
			if (sn != null)
			{
				NodeDataSolution snd = sn.PropertyObject as NodeDataSolution;
				string cfg = VobState.GetSolutionConfig(snd.Filename.Filename);
				if (string.Compare(cfg, "Release", StringComparison.OrdinalIgnoreCase) == 0)
				{
					toolStripComboBoxConfig.SelectedIndex = 1;
				}
				else
				{
					toolStripComboBoxConfig.SelectedIndex = 0;
				}
			}
			bool b = _projectLoaded;
			if (b)
			{
				b = false;
				ProjectNode pnode = getActiveProject();
				if (pnode != null)
				{
					if (pnode.Project != null)
					{
						b = (pnode.Project.ProjectType == EnumProjectType.WebAppPhp);
					}
				}
			}
			buildModifiedWebPagesToolStripMenuItem.Visible = b;
		}
		private void onActiveProjectChanged()
		{
			ProjectNode pnode = getActiveProject();

			if (pnode != null)
			{
				ProjectNodeData pnd = pnode.PropertyObject as ProjectNodeData;
				string cfg = pnd.Project.ActiveBuildConfig;
				if (string.Compare(cfg, "Release", StringComparison.OrdinalIgnoreCase) == 0)
				{
					toolStripComboBoxConfig.SelectedIndex = 1;
				}
				else
				{
					toolStripComboBoxConfig.SelectedIndex = 0;
				}
				switch (pnd.Project.TargetPlatform)
				{
					case AssemblyTargetPlatform.AnyCPU:
						toolStripComboBoxTargetPlatform.SelectedIndex = 0;
						break;
					case AssemblyTargetPlatform.X86:
						toolStripComboBoxTargetPlatform.SelectedIndex = 1;
						break;
					case AssemblyTargetPlatform.X64:
						toolStripComboBoxTargetPlatform.SelectedIndex = 2;
						break;
					default:
						toolStripComboBoxTargetPlatform.SelectedIndex = 0;
						break;
				}
			}
		}
		private void toolStripComboBoxConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			ProjectNode pnode = getActiveProject();
			if (pnode != null)
			{
				ProjectNodeData pnd = pnode.PropertyObject as ProjectNodeData;
				pnd.Project.SetActiveConfig(toolStripComboBoxConfig.Text);
			}
			SolutionNode ns = solutionTree1.GetCurrentSolution();
			if (ns != null)
			{
				NodeDataSolution nsd = ns.PropertyObject as NodeDataSolution;
				VobState.SetSolutionConfig(nsd.Filename.Filename, toolStripComboBoxConfig.Text);
				VobState.SaveSettings();
			}
		}

		private void toolStripComboBoxTargetPlatform_SelectedIndexChanged(object sender, EventArgs e)
		{
			ProjectNode pnode = getActiveProject();
			if (pnode != null)
			{
				ProjectNodeData pnd = pnode.PropertyObject as ProjectNodeData;
				switch (toolStripComboBoxTargetPlatform.SelectedIndex)
				{
					case 0:
						pnd.Project.SetTargetPlatform(AssemblyTargetPlatform.AnyCPU);
						break;
					case 1:
						pnd.Project.SetTargetPlatform(AssemblyTargetPlatform.X86);
						break;
					case 2:
						pnd.Project.SetTargetPlatform(AssemblyTargetPlatform.X64);
						break;

				}
			}
		}
		private void buildConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SolutionNode sl = solutionTree1.GetCurrentSolution();
			if (sl != null)
			{
				sl.ProjectConfig();
			}
		}
		#endregion

		#region Property Grid
		protected void OnPropertyGridSelectedObjectChanged(object sender, EventArgs e)
		{
			UpdatePropertyGridSite();
			propertyGrid1.ShowEvents(true);
		}
		protected void UpdatePropertyGridSite()
		{
		}
		protected IServiceProvider GetPropertyGridServiceProvider(ref IContainer cr)
		{
			object selObject = null;
			if (propertyGrid1.SelectedObjects != null && propertyGrid1.SelectedObjects.Length > 0)
				selObject = propertyGrid1.SelectedObjects[0];
			else
				selObject = propertyGrid1.SelectedObject;

			if (selObject is Component)
			{
				if ((selObject as Component).Site != null)
				{
					cr = (selObject as Component).Site.Container;
				}
				return (selObject as Component).Site;
			}
			return null;
		}
		//======================================================
		#endregion

		#region Form overrides
		protected override void WndProc(ref Message m)
		{
			const int WM_HOTKEY = 0x0312;

			switch (m.Msg)
			{
				case WM_HOTKEY:
					break;
			}
			base.WndProc(ref m);
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			FormWarning.CloseMessageForm();
			if (!askSaveSolution(true, true))
			{
				e.Cancel = true;
				return;
			}
			while (tabControl1.TabCount > 0)
			{
				bool bFound = false;
				for (int i = 0; i < tabControl1.TabCount; i++)
				{
					HostTab page = tabControl1.TabPages[i] as HostTab;
					if (page != null)
					{
						closeTabPage(i);
						bFound = true;
						break;
					}
				}
				if (!bFound)
				{
					break;
				}
			}
			base.OnClosing(e);
			VobState.MainWindowState = this.WindowState;
			VobState.MainWinowTop = Top;
			VobState.MainWinowLeft = Left;
			VobState.MainWinowWidth = Width;
			VobState.MainWinowHeight = Height;
			//
			VobState.SaveSettings();
		}
		protected override void OnResize(EventArgs e)
		{
			if (this.ClientSize.Height > panelToolbox.Top)
			{
				panelToolbox.Height = this.ClientSize.Height - panelToolbox.Top;
				xToolbox1.Top = panelToolbox.Top;
				xToolbox1.Height = this.ClientSize.Height - xToolbox1.Top;
			}
			if (this.ClientSize.Width > panelToolbox.Width + 10)
			{
				splitContainer3.Left = panelToolbox.Left + panelToolbox.Width;
				splitContainer3.Width = this.ClientSize.Width - splitContainer3.Left;
			}
			splitContainer3.Top = menuStrip1.Height + toolStrip1.Height;
			if (this.ClientSize.Height > splitContainer3.Top + 30)
			{
				splitContainer3.Height = this.ClientSize.Height - splitContainer3.Top;
			}
		}
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			updateMenuDisplay();
			xToolbox1.HideToolbox();
			FormSplash.CloseSplash();
		}
		//protected override 
		#endregion

		#region Edit Menu

		private IMenuCommandService activeMenu
		{
			get
			{
				if (_hostSurfaceManager != null && _hostSurfaceManager.ActiveDesignSurface != null)
				{
					IDesignerHost dh = (IDesignerHost)_hostSurfaceManager.ActiveDesignSurface.GetService(typeof(IDesignerHost));
					if (dh != null)
					{
						return dh.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
					}
				}
				return null;
			}
		}
		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			IMenuCommandService mnu = activeMenu;
			if (mnu != null)
			{
				mnu.GlobalInvoke(StandardCommands.Undo);
			}
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			IMenuCommandService mnu = activeMenu;
			if (mnu != null)
			{
				mnu.GlobalInvoke(StandardCommands.Cut);
			}
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			IMenuCommandService mnu = activeMenu;
			if (mnu != null)
			{
				mnu.GlobalInvoke(StandardCommands.Redo);
			}
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			IMenuCommandService mnu = activeMenu;
			if (mnu != null)
			{
				mnu.GlobalInvoke(StandardCommands.Copy);
			}
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			IMenuCommandService mnu = activeMenu;
			if (mnu != null)
			{
				mnu.GlobalInvoke(StandardCommands.Paste);
			}
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			IMenuCommandService mnu = activeMenu;
			if (mnu != null)
			{
				mnu.GlobalInvoke(StandardCommands.Delete);
			}
		}
		#endregion

		#region InterfaceVOB Members
		bool _openingPrj;
		public void SendNotice(enumVobNotice notice, object data)
		{
			EventArgNameChange ar;
			NodeObjectComponent dataComponent;
			SolutionNode slnNode;
			ClassData classData;
			HostTab tabpage;
			XToolbox2.xToolboxItem item;
			PassData pd;
			Type type;
			IComponent ic;
			HtmlElement_BodyBase heb;
			switch (notice)
			{
				case enumVobNotice.NewObject:
					//
					//check project folder
					ProjectNode prj = solutionTree1.ActiveProjectNode;
					if (prj == null)
					{
						MessageBox.Show(this, "A project is not selected to create the object", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
					}
					else
					{
						ProjectNodeData prjData = prj.PropertyObject as ProjectNodeData;
						buildModifiedWebPagesToolStripMenuItem.Enabled = (prjData.Project.ProjectType == EnumProjectType.WebAppPhp);
						bool b = prjData.HasFile;
						if (!b)
						{
							if (prjData.SelectFile(this))
							{
								b = true;
							}
						}
						if (b)
						{
							item = data as XToolbox2.xToolboxItem;
							if (item != null)
							{
								outputWindow1.AppendMessage(string.Format("Creating component: {0}", item));
								createNewObject(prjData, item.Type);
								this.xToolbox1.ClearItemSelection();
							}
						}
					}
					break;
				case enumVobNotice.HideToolbox:
					if ((bool)data)
					{
						xToolbox1.HideToolBox(this, null);
					}
					else
					{
						xToolbox1.ShowToolBox(this, null);
					}
					break;
				case enumVobNotice.ObjectOpen: //converted
					if (!_openingPrj)
					{
						_openingPrj = true;
						DesignUtil.LogIdeProfile("Open object");
						dataComponent = (NodeObjectComponent)data;
						openDesigner(dataComponent);
						_openingPrj = false;
					}
					break;
				case enumVobNotice.ObjectCreated:
					dataComponent = (NodeObjectComponent)data;
					ILimnorDesignPane xdh = openDesigner(dataComponent);
					if (xdh != null)
					{
						//add to toolbox
						xdh.Loader.Project.ToolboxLoaded = false;
						xdh.InitToolbox();
					}
					break;
				case enumVobNotice.ObjectActivate:
					dataComponent = data as NodeObjectComponent;
					if (dataComponent == null)
					{
						if (_hostSurfaceManager.ActiveDesignSurface != null)
						{
							IDesignerHost g = (IDesignerHost)_hostSurfaceManager.ActiveDesignSurface.GetService(typeof(IDesignerHost));
							propertyGrid1.SelectedObject = g.RootComponent;
						}
					}
					else
					{
						tabpage = getTabPageById(dataComponent.Class.ComponentId, dataComponent.Class.Project.ProjectGuid);
						if (tabpage != null)
						{
							tabControl1.SelectedTab = tabpage;
						}
					}
					xToolbox1.HideToolBox(this, null);
					break;
				case enumVobNotice.ObjectActivate2:
					ClassPointer root = data as ClassPointer;
					if (root != null)
					{
						comboBoxObjects.LoadClassPointer(root);
					}
					break;
				case enumVobNotice.ComponentSelected:
					ic = data as IComponent;
					if (ic != null && ic.Site != null && !string.IsNullOrEmpty(ic.Site.Name))
					{
						comboBoxObjects.SelectByName(ic.Site.Name);
					}
					break;
				case enumVobNotice.HtmlElementSelected:
					heb = data as HtmlElement_BodyBase;
					if (heb != null)
					{
						comboBoxObjects.SelectHtmlElement(heb);
						comboBoxObjects.Text = heb.toString();
					}
					break;
				case enumVobNotice.ComponentRenamed:
					comboBoxObjects.ReloadClassPointer();
					ic = data as IComponent;
					if (ic != null && ic.Site != null && !string.IsNullOrEmpty(ic.Site.Name))
					{
						comboBoxObjects.SelectByName(ic.Site.Name);
					}
					break;
				case enumVobNotice.HtmlElementRenamed:
					comboBoxObjects.ReloadClassPointer();
					heb = data as HtmlElement_BodyBase;
					if (heb != null)
					{
						comboBoxObjects.SelectHtmlElement(heb);
						propertyGrid1.Refresh();
					}
					break;
				case enumVobNotice.HtmlElementUsed:
					comboBoxObjects.ReloadClassPointer();
					heb = data as HtmlElement_BodyBase;
					if (heb != null)
					{
						comboBoxObjects.SelectHtmlElement(heb);
					}
					break;
				case enumVobNotice.ObjectChanged:
					ClassPointer cp = data as ClassPointer;
					if (cp != null)
					{
						tabpage = getTabPageById(cp.ClassId, cp.Project.ProjectGuid);
						if (tabpage != null)
						{
							tabpage.NodeData.Dirty = true;
							tabpage.Text = string.Format(CultureInfo.InvariantCulture, "{0}*", XmlUtil.GetNameAttribute(cp.XmlData));
						}
						cp.Project.SetProjectNamedData<bool>("JustCompiled", false);
					}
					break;
				case enumVobNotice.ResetToolbox:
					xToolbox1.ClearItemSelection();
					xToolbox1.HideToolBox(this, null);
					break;
				case enumVobNotice.GetTypeImage:
					PassData obj = data as PassData;
					if (obj != null)
					{
						XToolbox2.xToolboxItem ti = _allTools[obj.Key] as XToolbox2.xToolboxItem;
						if (ti != null)
						{
							obj.Data = ti.Bitmap;
						}
					}
					break;
				case enumVobNotice.ObjectClose:
					dataComponent = data as NodeObjectComponent;
					List<HostTab> pl = new List<HostTab>();
					for (int i = 0; i < tabControl1.TabPages.Count; i++)
					{
						HostTab page = tabControl1.TabPages[i] as HostTab;
						if (page != null)
						{
							pl.Add(page);
						}
					}
					if (dataComponent == null)
					{
						foreach (HostTab p in pl)
						{
							tabControl1.TabPages.Remove(p);
						}
					}
					else
					{
						foreach (HostTab p in pl)
						{
							if (p.NodeData == dataComponent)
							{
								tabControl1.TabPages.Remove(p);
								break;
							}
						}
					}
					addToolboxItemToolStripMenuItem.Enabled = false;
					for (int i = 0; i < tabControl1.TabPages.Count; i++)
					{
						HostTab page = tabControl1.TabPages[i] as HostTab;
						if (page != null)
						{
							addToolboxItemToolStripMenuItem.Enabled = true;
							break;

						}
					}
					break;
				case enumVobNotice.ObjectDeleted:
					dataComponent = data as NodeObjectComponent;
					dataComponent.Class.Project.ToolboxLoaded = false;
					tabpage = tabControl1.SelectedTab as HostTab;
					if (tabpage != null)
					{
						ILimnorDesignPane dp = tabpage.GetHost();
						if (dp.Loader.Project.ProjectGuid == dataComponent.Class.Project.ProjectGuid)
						{
							dp.InitToolbox();
						}
					}
					break;
				case enumVobNotice.ObjectSelected:
					propertyGrid1.SelectedObject = data;
					break;
				case enumVobNotice.ObjectSaved:
					dataComponent = data as NodeObjectComponent;
					tabpage = getTabPageById(dataComponent.Class.ComponentId, dataComponent.Class.Project.ProjectGuid);
					if (tabpage != null)
					{
						tabpage.ResetTitle();
					}
					break;
				case enumVobNotice.ObjectNameChanged:
					classData = data as ClassData;
					if (classData != null)
					{
						slnNode = solutionTree1.GetCurrentSolution();
						if (slnNode != null)
						{
							ProjectNode prjNode = slnNode.GetProjectByGuid(classData.Project.ProjectGuid);
							if (prjNode != null)
							{
								ComponentNode cNode = prjNode.GetComponentNodeById(classData.ComponentId);
								if (cNode != null)
								{
									((NodeObjectComponent)cNode.PropertyObject).Name = classData.ComponentName;
									tabpage = getTabPageById(classData.ComponentId, classData.Project.ProjectGuid);
									tabpage.ResetTitle();
								}
							}
						}
					}
					break;
				case enumVobNotice.ObjectCanCreate:
					setSelectedClassId(0, string.Empty);
					pd = data as PassData;
					if (pd != null)
					{
						type = pd.Key as Type;
						if (type != null)
						{
							LimnorXmlPane2 curPane = GetActiveXmlPane();
							if (curPane == null)
							{
								pd.Data = false;
							}
							else
							{
								if (curPane.IsWebApp)
								{
									pd.Data = false;
									xToolbox1.DataUsed();
								}
								else
								{
									if (curPane.IsWebPage)
									{
									}
									else
									{
										if (typeof(Form).IsAssignableFrom(type))
										{
											if (tabControl1.SelectedTab == null)
												pd.Data = false;
											else
											{
												HostTab tab = tabControl1.SelectedTab as HostTab;
												if (tab == null)
													pd.Data = false;
												else
												{
													if (tab.NodeData != null)
													{
														pd.Data = false;
													}
												}
											}
										}
										else
										{
											IDictionary dic = pd.Attributes as IDictionary;
											if (dic != null && dic.Contains(LimnorXmlPane2.WrappedType))
											{
												Type t0 = dic[LimnorXmlPane2.WrappedType] as Type;
												if (t0 != null)
												{
													setSelectedType(t0);
													//
													xToolboxItem tbi = new xToolboxItem(typeof(RuntimeInstance));
													pd.Attributes = tbi;
												}
											}
											else if (type.Equals(typeof(ClassInstancePointer)))
											{
												IDictionary properties = pd.Attributes as IDictionary;
												if (properties != null)
												{
													UInt32 classId = (UInt32)properties["ClassId"];
													string name = (string)properties["Name"];
													Type t = (Type)properties["Type"];
													setSelectedClassId(classId, name);
													//
													xToolboxItem tbi = new xToolboxItem(t);
													pd.Attributes = tbi;
												}
											}
											else
											{
												if (typeof(ToolboxItemXType).IsAssignableFrom(type))
												{
													IDictionary properties = pd.Attributes as IDictionary;
													if (properties != null)
													{
														ToolboxItemXType.SelectedToolboxTypeKey = (string)properties["DisplayName"];
													}
												}
												else
												{
													bool isComponent = (type.GetInterface("IComponent") != null);
													if (!isComponent)
													{
														xToolboxItem tbi = new xToolboxItem(VPLUtil.GetXClassType(type), type);
														pd.Attributes = tbi;
													}
												}
											}
										}
									}
								}
							}
						}
					}
					break;
				case enumVobNotice.ComponentAdded:
					item = data as XToolbox2.xToolboxItem;
					if (item != null && item.Type != null)
					{
						outputWindow1.AppendMessage(string.Format("Created component from toolbox: {0}", item));
					}
					else
					{
						outputWindow1.AppendMessage(string.Format("Created component: {0}", VPLUtil.GetObjectDisplayString(data)));
					}
					xToolbox1.DataUsed();
					comboBoxObjects.ReloadClassPointer();
					ic = data as IComponent;
					if (ic != null && ic.Site != null && !string.IsNullOrEmpty(ic.Site.Name))
					{
						comboBoxObjects.SelectByName(ic.Site.Name);
					}
					break;
				case enumVobNotice.ComponentDeleted:
					outputWindow1.AppendMessage(string.Format("Deleted component: {0}", VPLUtil.GetObjectDisplayString(data)));
					comboBoxObjects.ReloadClassPointer();
					break;
				case enumVobNotice.ProjectNodeSelected:
					_projectLoaded = true;
					_solutionLoaded = true;
					onSolutionLoaded();
					break;
				case enumVobNotice.ProjectBuild:
					ProjectNode pnode = data as ProjectNode;
					if (pnode != null)
					{
						askSaveSolution(false);
						if (VobState.ClearBuildLog)
						{
							clearBuildLog();
						}
						compileProject(pnode, false);
					}
					break;
				case enumVobNotice.SolutionFilenameChange:
					EventArgvFilenameChange ef = data as EventArgvFilenameChange;
					if (ef != null)
					{
						try
						{
							MRU mru = new MRU();
							mru.ReplaceMRU(ef.OldFilename, ef.NewFilename);
						}
						catch (Exception err)
						{
							UIUtil.ShowError(this, err);
						}
					}
					break;
				case enumVobNotice.SolutionFileCreated:
					try
					{
						updateMru((string)data);
					}
					catch (Exception err)
					{
						UIUtil.ShowError(this, err);
					}
					break;
				case enumVobNotice.RootMenu:
					if (_hostSurfaceManager.ActiveDesignSurface != null)
					{
						IDesignerHost g = (IDesignerHost)_hostSurfaceManager.ActiveDesignSurface.GetService(typeof(IDesignerHost));
						propertyGrid1.SelectedObject = g.RootComponent;
						Control c = g.RootComponent as Control;
						if (c != null)
						{
							System.Drawing.Point pt = (System.Drawing.Point)data;
							System.Windows.Forms.ContextMenu cm = new ContextMenu();
							System.Windows.Forms.MenuItem mi = new MenuItem("Undo");
							mi.Click += new EventHandler(undoToolStripMenuItem_Click);
							cm.MenuItems.Add(mi);
							IMenuCommandService mnu = activeMenu;
							if (mnu == null)
							{
								mi.Enabled = false;
							}
							else
							{
								MenuCommand undoMenuCommand = mnu.FindCommand(StandardCommands.Undo);
								if (undoMenuCommand != null)
								{
									mi.Enabled = undoMenuCommand.Enabled;
								}
								else
								{
									mi.Enabled = false;
								}
							}
							mi = new MenuItem("Redo");
							mi.Click += new EventHandler(redoToolStripMenuItem_Click);
							cm.MenuItems.Add(mi);
							if (mnu == null)
							{
								mi.Enabled = false;
							}
							else
							{
								MenuCommand redoMenuCommand = mnu.FindCommand(StandardCommands.Redo);
								if (redoMenuCommand != null)
								{
									mi.Enabled = redoMenuCommand.Enabled;
								}
								else
								{
									mi.Enabled = false;
								}
							}
							mi = new MenuItem("Paste");
							mi.Click += new EventHandler(pasteToolStripMenuItem_Click);
							cm.MenuItems.Add(mi);
							cm.Show(c, pt);
						}
					}
					xToolbox1.HideToolBox(this, null);
					break;
				case enumVobNotice.GetToolbox:
					pd = (PassData)data;
					pd.Data = xToolbox1;
					break;
				case enumVobNotice.BeforeRootComponentNameChange:
					if (_openingComponent)
						break;
					ar = (EventArgNameChange)data;
					if (string.CompareOrdinal(ar.NewName, XmlSerialization.STARTERCLASS) == 0 && (ar.ComponentType == null || ar.ComponentType.GetInterface("IAppType") == null))
					{
						ar.Cancel = true;
						ar.Message = XmlSerialization.FormatString("The name '{0}' is reserved", ar.NewName);
					}
					else
					{
						if (ar.Owner == propertyGrid1.SelectedObject)
						{
							slnNode = solutionTree1.GetCurrentSolution();
							if (slnNode != null)
							{
								ProjectNode prjNode = slnNode.GetProjectByName((string)ar.Attributes);
								if (prjNode != null)
								{
									if (prjNode.GetComponentNodeByName(ar.NewName) != null)
									{
										ar.Cancel = true;
										ar.Message = XmlSerialization.FormatString("The name {0} is in use", ar.NewName);
									}
								}
							}
						}
					}
					break;
				case enumVobNotice.AddFrequentType:
					type = data as Type;
					if (type != null)
					{
						this.addMruType(type);
					}
					break;
				case enumVobNotice.RemoveFrequentType:
					type = data as Type;
					if (type != null)
					{
						removeMruType(type);
					}
					break;
			}
		}
		public LimnorProject GetActiveProject()
		{
			ProjectNode pn = getActiveProject();
			if (pn != null)
			{
				ProjectNodeData pnd = pn.PropertyObject as ProjectNodeData;
				return pnd.Project;
			}
			return null;
		}
		public LimnorXmlPane2 GetActiveXmlPane()
		{
			HostTab tab = tabControl1.SelectedTab as HostTab;
			if (tab != null)
			{
				return tab.GetHost() as LimnorXmlPane2;
			}
			return null;
		}
		#endregion

		#region ICompilerLog Members
		StringCollection _compilerErrors;
		public void LogError(string msg)
		{
			outputWindow1.AppendMessage(msg);
			_compilerErrors.Add(msg);
		}

		public void LogWarning(string msg, params object[] values)
		{
			if (values != null && values.Length > 0)
			{
				msg = string.Format(CultureInfo.InvariantCulture, msg, values);
			}
			msg = string.Format(CultureInfo.InvariantCulture, "Warning:{0}", msg);
			outputWindow1.AppendMessage(msg);
		}

		public bool HasLoggedErrors
		{
			get { return _compilerErrors.Count > 0; }
		}

		public void LogErrorFromException(Exception e)
		{
			string msg = DesignerException.FormExceptionText(e);
			LogError(msg);
		}

		#endregion

		#region Help
		const string HPKEY_USERGUIDE = "UserGuide";
		const string HPKEY_TUTORIAL = "Tutorial";
		const string HPKEY_REFERENCE = "Reference";
		private void addHelps(Control c, string file)
		{
			StreamReader sr = null;
			try
			{
				int y = 30;
				int dy = 3;
				int x = 30;
				int i = 0;
				sr = new StreamReader(file);
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();
					if (!string.IsNullOrEmpty(line))
					{
						string[] ss = line.Split(';');

						HelpLink hl = new HelpLink(ss[0]);
						if (ss.Length > 1)
						{
							hl.Text = ss[1];
						}
						else
						{
							hl.Text = ss[0];
						}
						hl.Top = y;
						hl.Left = x;
						hl.Width = c.ClientSize.Width;
						hl.Height = 30;
						hl.Parent = c;
						hl.Visible = true;
						i++;
						if (i > 10 && ss.Length < 2)
						{
							i = 0;
							y = 30;
							x = 300;
						}
						else
						{
							y += hl.Height;
							y += dy;
						}
					}
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Error loading configuration file [{0}]. {1}", file, err.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			if (sr != null)
			{
				sr.Close();
			}
		}
		private void usersGuideToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (TabPage p in tabControl1.TabPages)
			{
				if (p.Tag != null)
				{
					string sk = p.Tag as string;
					if (!string.IsNullOrEmpty(sk))
					{
						if (string.CompareOrdinal(sk, HPKEY_USERGUIDE) == 0)
						{
							tabControl1.SelectedTab = p;
							return;
						}
					}
				}
			}
			string s = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Help\\UserGuide.cfg");
			if (File.Exists(s))
			{
				HostControl2 hc = _hostSurfaceManager.GetNewHost(typeof(NewComponentPrompt));
				this.xToolbox1.Host = hc.DesignerHost;
				TabPage tabpage = new TabPage();
				tabControl1.TabPages.Add(tabpage);
				tabpage.Text = "Users' Guide";
				hc.Parent = tabpage;
				hc.Dock = DockStyle.Fill;
				//
				DesignSurface ds = hc.DesignSurface;
				VOB.NewComponentPromptDesigner.NewComponentPromptView pv = ds.View as VOB.NewComponentPromptDesigner.NewComponentPromptView;
				pv.ShowBackgroundText = false;
				pv.HelpFolder = VobState.UserGuide;

				//
				addHelps(pv, s);
				//
				tabControl1.SelectedTab = tabpage;
				tabpage.Tag = HPKEY_USERGUIDE;
			}
			else
			{
				MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Configuration file not found [{0}]", s), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void tutorialsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (TabPage p in tabControl1.TabPages)
			{
				if (p.Tag != null)
				{
					string sk = p.Tag as string;
					if (!string.IsNullOrEmpty(sk))
					{
						if (string.CompareOrdinal(sk, HPKEY_TUTORIAL) == 0)
						{
							tabControl1.SelectedTab = p;
							return;
						}
					}
				}
			}
			string s = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Help\\Tutorial.cfg");
			if (File.Exists(s))
			{
				string f = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TutorialViewer.exe");
				if (File.Exists(f))
				{
					HostControl2 hc = _hostSurfaceManager.GetNewHost(typeof(NewComponentPrompt));
					this.xToolbox1.Host = hc.DesignerHost;
					TabPage tabpage = new TabPage();
					tabControl1.TabPages.Add(tabpage);
					tabpage.Text = "Tutorials";
					hc.Parent = tabpage;
					hc.Dock = DockStyle.Fill;
					//
					DesignSurface ds = hc.DesignSurface;
					VOB.NewComponentPromptDesigner.NewComponentPromptView pv = ds.View as VOB.NewComponentPromptDesigner.NewComponentPromptView;
					pv.ShowBackgroundText = false;
					pv.HelpFolder = VobState.TutorialFolder;
					if (string.IsNullOrEmpty(pv.HelpFolder))
					{
						MessageBox.Show(this, "To reduce download size, the Tutorials have been moved from the Limnor Studio installation into a separate installation. It can be downloaded from http://www.limnor.com/studio/setuptutorials.msi", "Tutorials", MessageBoxButtons.OK, MessageBoxIcon.Information);
						pv.HelpFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tutorials");
					}
					pv.Exe = f;
					//
					addHelps(pv, s);
					//
					tabControl1.SelectedTab = tabpage;
					tabpage.Tag = HPKEY_TUTORIAL;
					//
					if (!Directory.Exists(pv.HelpFolder))
					{
						MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Tutorial files are not found in folder {0}. You may downloaded and install http://www.limnor.com/studio/setuptutorials.msi", pv.HelpFolder), "Tutorials", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
				else
				{
					MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Slide viewer file not found [{0}]", f), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Configuration file not found [{0}]", s), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void referencesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (TabPage p in tabControl1.TabPages)
			{
				if (p.Tag != null)
				{
					string sk = p.Tag as string;
					if (!string.IsNullOrEmpty(sk))
					{
						if (string.CompareOrdinal(sk, HPKEY_REFERENCE) == 0)
						{
							tabControl1.SelectedTab = p;
							return;
						}
					}
				}
			}
			string s = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Help\\Reference.cfg");
			if (File.Exists(s))
			{
				HostControl2 hc = _hostSurfaceManager.GetNewHost(typeof(NewComponentPrompt));
				this.xToolbox1.Host = hc.DesignerHost;
				TabPage tabpage = new TabPage();
				tabControl1.TabPages.Add(tabpage);
				tabpage.Text = "References";
				hc.Parent = tabpage;
				hc.Dock = DockStyle.Fill;
				//
				DesignSurface ds = hc.DesignSurface;
				VOB.NewComponentPromptDesigner.NewComponentPromptView pv = ds.View as VOB.NewComponentPromptDesigner.NewComponentPromptView;
				pv.ShowBackgroundText = false;
				pv.HelpFolder = VobState.Reference;
				//
				addHelps(pv, s);
				//
				tabControl1.SelectedTab = tabpage;
				tabpage.Tag = HPKEY_REFERENCE;
			}
			else
			{
				MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Configuration file not found [{0}]", s), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#endregion

		#region Project menu
		private void manageDatabaseConnectionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProjectNode pn = getActiveProject();
			if (pn != null)
			{
				pn.Expand();
				List<Guid> gs = new List<Guid>();
				for (int i = 0; i < pn.Nodes.Count; i++)
				{
					ComponentNode cn = pn.Nodes[i] as ComponentNode;
					if (cn != null)
					{
						NodeObjectComponent cnd = cn.PropertyObject as NodeObjectComponent;
						ClassPointer cp = ClassPointer.CreateClassPointer(cnd.Class.ComponentId, cnd.Class.Project);
						if (cp.ObjectList.Count == 0)
						{
							cp.ObjectList.LoadObjects();
							if (cp.ObjectList.Reader.HasErrors)
							{
								MathNode.Log(cp.ObjectList.Reader.Errors);
								cp.ObjectList.Reader.ResetErrors();
							}
						}
						IList<Guid> g0 = cp.GetDatabaseConnectionsUsed();
						foreach (Guid g in g0)
						{
							if (!gs.Contains(g))
							{
								gs.Add(g);
							}
						}
					}
				}
				DlgConnectionManager dlg = new DlgConnectionManager();
				dlg.UseProjectScope = true;
				dlg.EnableCancel(false);
				dlg.SetProjectDatabaseUsages(gs);
				dlg.ShowDialog();
			}
		}

		private void addToolboxItemToolStripMenuItem_Click(object sender, EventArgs e)
		{
			HostTab page = tabControl1.SelectedTab as HostTab;
			if (page != null)
			{
				LimnorXmlPane2 pane = page.GetHost() as LimnorXmlPane2;
				if (pane != null)
				{
					pane.AddToolboxItem();
				}
			}

		}

		private void resourceManagerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProjectNode pn = getActiveProject();
			if (pn != null)
			{
				ProjectNodeData pnd = pn.PropertyObject as ProjectNodeData;
				if (DlgResMan.EditResources(null, pnd.Project) == DialogResult.OK)
				{
				}
			}
		}

		private void addWebServiceProxyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LimnorXmlPane2 p = GetActiveXmlPane();
			if (p != null)
			{
				p.AddWebService();
			}
		}

		private void removeWebServiceProxyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LimnorXmlPane2 p = GetActiveXmlPane();
			if (p != null)
			{
				p.ManageWebService();
			}
		}

		private void updateWebServiceProxyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LimnorXmlPane2 p = GetActiveXmlPane();
			if (p == null)
			{
				DlgUpdateWebServices dlg = new DlgUpdateWebServices();
				dlg.ShowDialog();
			}
		}
		private void createServiceProxyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LimnorXmlPane2 pane = GetActiveXmlPane();
			if (pane != null)
			{
				string svcUtil = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "svcutil.exe");
				if (!System.IO.File.Exists(svcUtil))
				{
					MessageBox.Show(pane.FindForm(), string.Format(System.Globalization.CultureInfo.InvariantCulture, "Service utility not found: [{0}].", svcUtil), "Add WCF Service Proxy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else
				{
					DlgAddWcfService dlg = new DlgAddWcfService();
					dlg.SetData(pane);
					if (dlg.ShowDialog(pane.FindForm()) == DialogResult.OK)
					{
					}
				}
			}
		}
		private void licenseManagerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LimnorProject prj = GetActiveProject();
			if (prj != null)
			{
				CopyProtector cp = new CopyProtector();
				cp.SetApplicationGuid(prj.ProjectGuid);
				cp.SetApplicationName(prj.ProjectName);
				cp.IssueLicense(null);
			}
		}
		private void buildModifiedWebPagesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			executeBuild(buildModifiedWebPagesToolStripMenuItemHandler, sender, e);
		}
		private void buildModifiedWebPagesToolStripMenuItemHandler(object sender, EventArgs e)
		{
			incrementBuild(false);
		}
		#endregion

		#region Tools menu
		private void manageVisualProgrammingSystemsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DlgDesigners dlg = new DlgDesigners();
			dlg.LoadData(BuiltInDesigners);
			dlg.ShowDialog();
		}

		private void manageMySQLDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DlgMySQL dlg = new DlgMySQL();
			Connection cnn = new Connection();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				dlgDatabase dlgDB = new dlgDatabase();
				cnn.ConnectionString = dlg.ConnectionString;
				cnn.DatabaseType = dlg.ConnectionType;
				dlgDB.LoadData(cnn);
				dlgDB.ShowDialog();
			}
		}

		private void manageMicrosoftAccessDatToolStripMenuItem_Click(object sender, EventArgs e)
		{
			dlgDBAccess dlg = new dlgDBAccess();
			dlg.SetForDatabaseEdit();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				dlgDatabase dlgDB = new dlgDatabase();
				dlgDB.LoadAccess(dlg.sRet, dlg.sDBPass, dlg.sUser, dlg.sPass);
				dlgDB.ShowDialog();
			}
		}


		private void sourceCompilerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogCompileCode dlg = new DialogCompileCode();
			dlg.ShowDialog(this);
		}

		private void updateDynamicLinkLibraryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogCopyDLL dlg = new DialogCopyDLL();
			dlg.ShowDialog(this);
		}

		private void makeHTMLReadableToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LimnorVOB.DlgHtmlIndent dlg = new LimnorVOB.DlgHtmlIndent();
			dlg.SetInitFile(LimnorXmlCompiler.LastCompiledHtmlFile);
			dlg.ShowDialog(this);
		}

		private void removePrgCfgToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DlgCfgUtil dlg = new DlgCfgUtil();
			dlg.ShowDialog(this);

		}
		#endregion

	}
	public enum EnumFileOverwrite
	{
		SkipOne,
		SkipAll,
		OverwriteOne,
		OverwriteAll
	}
}