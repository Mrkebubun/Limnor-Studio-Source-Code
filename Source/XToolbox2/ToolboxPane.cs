/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Toolbox system
 * License: GNU General Public License v3.0
 
 */
//------------------------------------------------------------------------------
/// <copyright from='2006' to='2020' company='Longflow Enterprises Ltd'>
///    Copyright (c) Longflow Enterprises Ltd. All Rights Reserved.
/// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.IO;
using System.Reflection;
using System.Xml;
using VOB;
using VPL;
using System.Collections.Generic;
using System.Globalization;

namespace XToolbox2
{
	/// Our implementation of a toolbox. We kept the actual toolbox control
	/// separate from the IToolboxService, but the toolbox could easily
	/// implement the service itself. Note that IToolboxUser does not pertain
	/// to the toolbox, but instead is implemented by the designers that receive
	/// ToolboxItems.
	public class ToolboxPane2 : UserControl, ILimnorToolbox
	{
		#region fields and constructors
		private IDesignerHost host;
		private IServiceProvider genericServices;
		private ListBox selectedList = null; // the index of the currently selected list
		private int selectedTabIndex = -1; // the index of the currently selected tool
		private ToolboxTab2[] toolTabs = null;
		private Bitmap bmpMinus = null;
		private Bitmap bmpPlus = null;
		private LabelToolboxTitle titleBar;
		private bool dirty;
		private string _toolboxXml;
		private TabsPanel panelTabs;
		private bool bFirstShow = true;
		private System.ComponentModel.Container components = null;
		public ToolboxPane2()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			titleBar.toolbox = this;
			panelTabs.OnVScroll += new EventHandler(panelTabs_OnVScroll);
		}
		#endregion

		#region IDispose
		/// Clean up any resources being used.
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#endregion

		#region Properties
		public bool IsInDisplay { get; set; }
		public IServiceProvider GenericServices
		{
			get
			{
				return genericServices;
			}
			set
			{
				genericServices = value;
			}
		}
		/// We need access to the designers.
		public IDesignerHost Host
		{
			get
			{
				return host;
			}
			set
			{
				host = value;
			}
		}
		public int TabCount
		{
			get
			{
				if (toolTabs == null)
					return 0;
				return toolTabs.Length;
			}
		}


		public string ToolXml
		{
			get
			{
				return _toolboxXml;
			}
			set
			{
				_toolboxXml = value;
			}
		}


		public ToolboxItem SelectedItem
		{
			get
			{
				return SelectedTool;
			}
		}
		/// The currently selected tool is defined by our currently selected
		/// category (ListBox) and our selectedIndex member.
		public ToolboxItem SelectedTool
		{
			get
			{
				if (selectedList != null)
				{
					if (selectedList.SelectedIndex > 0)
					{
						return selectedList.Items[selectedList.SelectedIndex] as ToolboxItem;
					}
				}
				return null;
			}
		}

		/// The name of our selected category (Windows Forms, Components, etc.)
		/// This property (and the next few) are all in place to support
		/// methods of the IToolboxService.
		public string SelectedCategory
		{
			get
			{
				if (toolTabs != null)
				{
					if (selectedTabIndex >= 0 && selectedTabIndex < toolTabs.Length)
					{
						return toolTabs[selectedTabIndex].Name;
					}
				}
				return "";
			}
			set
			{
				if (toolTabs != null)
				{
					for (int i = 0; i < toolTabs.Length; i++)
					{
						if (toolTabs[i].Name == value)
						{
							selectedTabIndex = i;
							toolTabs[i].Selected = true;
							break;
						}
					}
				}
			}
		}
		public bool Changed
		{
			get
			{
				return dirty;
			}
			set
			{
				dirty = value;
			}
		}
		/// The names of all our categories.
		public CategoryNameCollection CategoryNames
		{
			get
			{
				if (toolTabs != null)
				{
					string[] categories = new string[toolTabs.Length];
					for (int i = 0; i < toolTabs.Length; i++)
					{
						categories[i] = toolTabs[i].Name;
					}
					return new CategoryNameCollection(categories);
				}
				return new CategoryNameCollection(new string[0]);
			}
		}
		#endregion

		#region private methods
		void panelTabs_OnVScroll(object sender, EventArgs e)
		{
			RefreshTabs();
		}


		private void refreshForm(object data)
		{
			Form frm = this.FindForm();
			if (frm != null && frm.Handle != IntPtr.Zero && !frm.IsDisposed && !frm.Disposing)
			{
				if (frm.InvokeRequired)
				{
					System.Windows.Forms.MethodInvoker mi = new System.Windows.Forms.MethodInvoker(frm.Refresh);
					frm.BeginInvoke(mi);
				}
				else
				{
					frm.Refresh();
				}
			}
		}
		private xToolboxItem createItem(Type tp, string name, Bitmap img, Dictionary<string, object> properties)
		{
			xToolboxItem ti = new xToolboxItem(tp, img);
			ti.IsTransient = true;
			if (properties != null)
			{
				foreach (KeyValuePair<string, object> kv in properties)
				{
					ti.Properties.Add(kv.Key, kv.Value);
				}
			}
			ti.DisplayName = name;
			return ti;
		}
		private void adjustTabWith()
		{
			if (toolTabs != null)
			{
				int n = panelTabs.ClientWidth;
				for (int i = 0; i < toolTabs.Length; i++)
				{
					toolTabs[i].Width = n;
				}
			}
			panelTabs.Init();
		}
		private void removeTab(int i)
		{
			int n = toolTabs.Length - 1;
			ToolboxTab2 tab = toolTabs[i];
			ToolboxTab2[] a = new ToolboxTab2[n];
			for (int j = 0; j < toolTabs.Length; j++)
			{
				if (j < i)
					a[j] = toolTabs[j];
				else if (j > i)
				{
					a[j - 1] = toolTabs[j];
					a[j - 1].Index = j - 1;
				}
			}
			toolTabs = a;
			this.Controls.Remove(tab);
			AdjustTabPos(i - 1);
			Changed = true;
		}
		#endregion

		#region Methods
		/// <summary>
		/// call this function to avoid repeated hiding
		/// </summary>
		public void ReqestHideToolbox()
		{
			if (IsInDisplay)
			{
				HideToolbox();
			}
		}
		public void RefreshTabs()
		{
			if (toolTabs != null)
			{
				if (toolTabs.Length > 0)
				{
					toolTabs[0].Top = -panelTabs.ScrollTop;
					for (int i = 1; i < toolTabs.Length; i++)
					{
						toolTabs[i].Top = toolTabs[i - 1].Top + toolTabs[i - 1].Height;
					}
				}
			}
		}
		public Bitmap GetMinusImage()
		{
			if (bmpMinus == null)
			{
				bmpMinus = new Bitmap(GetType(), "minus.bmp");
			}
			return bmpMinus;
		}
		public Bitmap GetPlusImage()
		{
			if (bmpPlus == null)
			{
				bmpPlus = new Bitmap(GetType(), "plus.bmp");
			}
			return bmpPlus;
		}
		public void AdjustTabPos(int idx)
		{
			if (toolTabs != null && idx < toolTabs.Length && toolTabs.Length > 0)
			{
				if (idx < 1)
					idx = 1;

				int h = 0;
				for (int i = 0; i < toolTabs.Length; i++)
				{
					h += toolTabs[i].Height;
				}
				panelTabs.SetContentHeight(h);
				if (h > this.panelTabs.Height)
				{
					this.panelTabs.ShowVScroll();
				}
				else
				{
					this.panelTabs.HideVScroll();
				}
				toolTabs[0].Top = panelTabs.ScrollTop;
				for (int i = idx; i < toolTabs.Length; i++)
				{
					toolTabs[i].Top = toolTabs[i - 1].Top + toolTabs[i - 1].Height;
				}
			}
			adjustTabWith();
		}
		public void SetSelectedList(int selTab, ListBox l)
		{
			selectedTabIndex = selTab;
			selectedList = l;
		}
		public void ShowProjectTab(Guid projectGuid)
		{
			int n = TabCount;
			for (int i = 0; i < n; i++)
			{
				ToolboxTabProject ptab = toolTabs[i] as ToolboxTabProject;
				if (ptab != null)
				{
					if (ptab.ProjectGuid == projectGuid)
					{
						ptab.Visible = true;
					}
					else
					{
						ptab.Visible = false;
					}
				}
			}
		}
		public void RemoveProjectTabs()
		{
			List<ToolboxTabProject> ps = new List<ToolboxTabProject>();
			List<ToolboxTab2> ts = new List<ToolboxTab2>();
			int n = TabCount;
			for (int i = 0; i < n; i++)
			{
				ToolboxTabProject ptab = toolTabs[i] as ToolboxTabProject;
				if (ptab != null)
				{
					ps.Add(ptab);
				}
				else
				{
					ts.Add(toolTabs[i]);
				}
			}
			toolTabs = new ToolboxTab2[ts.Count];
			ts.CopyTo(toolTabs);
			foreach (ToolboxTabProject p in ps)
			{
				Control pr = p.Parent;
				if (pr != null)
				{
					pr.Controls.Remove(p);
				}
			}
			AdjustTabSizes();
		}
		public void ResetProjectTab(Guid projectGuid)
		{
			int n = TabCount;
			for (int i = 0; i < n; i++)
			{
				ToolboxTabProject ptab = toolTabs[i] as ToolboxTabProject;
				if (ptab != null)
				{
					if (ptab.ProjectGuid == projectGuid)
					{
						ptab.ClearItems();
						break;
					}
				}
			}
		}
		public ToolboxTabProject CreateProjectTab(string name, Guid guid)
		{
			ToolboxTabProject retTab = null;
			int n = TabCount;
			for (int i = 0; i < n; i++)
			{
				ToolboxTabProject ptab = toolTabs[i] as ToolboxTabProject;
				if (ptab != null)
				{
					if (ptab.ProjectGuid == guid)
					{
						ptab.Visible = true;
						retTab = ptab;
					}
					else
					{
						ptab.Visible = false;
					}
				}
			}
			if (retTab == null)
			{
				ToolboxTab2[] a = new ToolboxTab2[n + 1];
				toolTabs.CopyTo(a, 1);
				toolTabs = a;
				retTab = new ToolboxTabProject(this, name, guid);
				toolTabs[0] = retTab;
				retTab.Visible = true;
				retTab.Location = new Point(0, 0);
				retTab.Size = new Size(this.ClientSize.Width - 1, LabelToolboxTabTitle.TitleHeight - 1);
				panelTabs.Controls.Add(retTab);
			}
			return retTab;
		}
		public ToolboxTab2 CreateTab(string name, bool readOnly, int idx)
		{
			int n = TabCount;
			for (int i = 0; i < n; i++)
			{
				if (string.Compare(name, toolTabs[i].Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					toolTabs[i].Visible = true;
					return toolTabs[i];
				}
			}
			ToolboxTab2[] a = new ToolboxTab2[n + 1];
			if (idx >= 0 && idx < n)
			{
				for (int i = 0, j = 0; i <= n; i++)
				{
					if (i < idx)
					{
						a[i] = toolTabs[j];
						j++;
					}
					else if (i > idx)
					{
						a[i] = toolTabs[j];
						j++;
					}
				}
			}
			else
			{
				if (n > 0)
				{
					toolTabs.CopyTo(a, 0);
				}
			}
			toolTabs = a;
			if (idx >= 0 && idx < n)
			{
				n = idx;
			}
			toolTabs[n] = new ToolboxTab2(this, name, n, readOnly);
			if (n == 0)
			{
				toolTabs[n].Location = new Point(0, 0);
			}
			else
			{
				toolTabs[n].Location = new Point(0, toolTabs[n - 1].Height + toolTabs[n - 1].Top);
			}
			toolTabs[n].Size = new Size(this.ClientSize.Width - 1, LabelToolboxTabTitle.TitleHeight - 1);
			this.Visible = true;
			panelTabs.Visible = true;
			panelTabs.Controls.Add(toolTabs[n]);
			return toolTabs[n];
		}
		public void ForecScroll()
		{
			panelTabs.Width = this.Width + 8;
		}
		public void ReadFromXmlFile(string xmlFile, out string error)
		{
			_toolboxXml = xmlFile;
			ReadFromXmlFile(out error);
		}
		public void ReadFromXmlFile(out string error)
		{
			if (_toolboxXml == null || _toolboxXml.Length == 0)
			{
				error = "ToolboxXml is empty";
				return;
			}
			if (!System.IO.File.Exists(_toolboxXml))
			{
				error = "ToolboxXml does not exist at " + _toolboxXml;
				return;
			}
			error = "";
			try
			{
				StreamReader sr = new StreamReader(_toolboxXml);
				XmlDocument xml = new XmlDocument();
				xml.LoadXml(sr.ReadToEnd());
				sr.Close();
				foreach (XmlNode nodeDoc in xml.ChildNodes)
				{
					if (string.CompareOrdinal(nodeDoc.Name, "Toolbox") == 0)
					{
						foreach (XmlNode node in nodeDoc.ChildNodes)
						{
							if (string.CompareOrdinal(node.Name, "ToolboxTab") == 0)
							{
								XmlAttribute xa = node.Attributes["title"];
								if (xa != null && xa.Value != null && xa.Value.Length > 0)
								{
									string name = xa.Value;
									bool bReadOnly = true;
									xa = node.Attributes["readonly"];
									if (xa != null && xa.Value != null && xa.Value.Length > 0)
									{
										bReadOnly = (string.Compare(xa.Value, "false", StringComparison.OrdinalIgnoreCase) != 0);
									}
									ToolboxTab2 tab = CreateTab(name, bReadOnly, -1);
									foreach (XmlNode nd in node.ChildNodes)
									{
										if (string.CompareOrdinal(nd.Name, "Item") == 0)
										{
											try
											{
												string s = nd.InnerText.Replace("\r", "").Replace("\n", "").Trim();
												Type t = Type.GetType(s);
												if (t != null)
												{
													xToolboxItem xi;
													xa = nd.Attributes["Type"];
													if (xa != null)
													{
														Type xt = Type.GetType(xa.Value);
														xi = (xToolboxItem)Activator.CreateInstance(xt, new object[] { t });
													}
													else
													{
														xi = new xToolboxItem(t);
													}
													xi.ReadNode(nd);
													tab.AddItem(xi);
												}
											}
											catch (Exception ee)
											{
												if (error.Length == 0)
													error += ee.Message;
												else
													error += "\r\n" + ee.Message;
											}
										}
									}
									tab.AdjustSize();
								}
							}
						}

					}
				}
			}
			catch (Exception e)
			{
				error = string.Format(CultureInfo.InvariantCulture, "{0}. \r\n{1}", e.Message, e.StackTrace);
			}
			OnResize(null);
			AdjustTabPos(0);
		}
		public void SaveCustomToolboxTabs()
		{
			XmlDocument xml = new XmlDocument();
			XmlNode root = xml.CreateElement("Toolbox");
			xml.AppendChild(root);
			if (toolTabs != null)
			{
				for (int i = 0; i < toolTabs.Length; i++)
				{
					ToolboxTab2 tab = toolTabs[i];
					if (tab != null && tab.Persist)
					{
						XmlNode node = xml.CreateElement("ToolboxTab");
						XmlAttribute xa = xml.CreateAttribute("title");
						xa.Value = tab.Name;
						node.Attributes.Append(xa);
						//
						if (tab.IsCustom)
						{
							xa = xml.CreateAttribute("readonly");
							xa.Value = "false";
							node.Attributes.Append(xa);
						}
						root.AppendChild(node);
						for (int j = 0; j < tab.ToolboxItemCount; j++)
						{
							xToolboxItem x = tab[j + 1] as xToolboxItem;
							if (x != null && x.Type != null)
							{
								node.AppendChild(x.CreateNode(xml));
							}
						}
					}
				}
			}
			xml.Save(_toolboxXml);
			Changed = false;
		}
		public void ShowToolbox()
		{
			BringToFront();
			if (bFirstShow)
			{
				bFirstShow = false;
				panelTabs.Width = this.Width - 1;
			}
			adjustTabWith();
			//exclude Windows 2003, Windows XP, and WIndows 2000
			if (System.Environment.OSVersion.Platform == PlatformID.Win32NT && System.Environment.OSVersion.Version.Major == 5)
			{
				this.Show();
				this.Refresh();
				refreshForm(null);
			}
			else
			{
				CPP.AnimateWindowShow(Handle);
			}
			IsInDisplay = true;
		}
		public void HideToolbox()
		{
			//exclude Windows 2003, Windows XP, and WIndows 2000
			if (System.Environment.OSVersion.Platform == PlatformID.Win32NT && System.Environment.OSVersion.Version.Major == 5)
			{
				this.Hide();
				refreshForm(null);
			}
			else
			{
				CPP.AnimateWindowHide(Handle);
			}
			IsInDisplay = false;
		}
		public void HideToolBox(object sender, EventArgs e)
		{
			HideToolbox();
		}
		public void ShowToolBox(object sender, EventArgs e)
		{
			ShowToolbox();
		}

		/// The IToolboxService has methods for getting tool collections using
		/// an optional category parameter. We support that request here.
		public ToolboxItemCollection GetToolsFromCategory(string category)
		{
			if (toolTabs != null)
			{
				for (int i = 0; i < toolTabs.Length; i++)
				{
					if (toolTabs[i].Name == category)
					{
						ToolboxItem[] tools = new ToolboxItem[toolTabs[i].ToolboxItemCount];
						for (int j = 0; j < toolTabs[i].ToolboxItemCount; j++)
						{
							tools[j] = toolTabs[i][j + 1];
						}
						return new ToolboxItemCollection(tools);
					}
				}
			}
			return new ToolboxItemCollection(new ToolboxItem[] { });
		}

		public Hashtable GetAllToolboxItems()
		{
			Hashtable ht = new Hashtable();
			if (toolTabs != null)
			{
				for (int i = 0; i < toolTabs.Length; i++)
				{
					for (int j = 0; j < toolTabs[i].ToolboxItemCount; j++)
					{
						xToolboxItem xt = toolTabs[i][j + 1] as xToolboxItem;
						if (xt != null)
						{
							ht[xt.Type] = xt;
						}
					}
				}
			}
			return ht;
		}
		/// Get all of our tools.
		public ToolboxItemCollection GetAllTools()
		{
			ToolboxItem[] tools;
			if (toolTabs != null)
			{
				int n = 0;
				for (int i = 0; i < toolTabs.Length; i++)
				{
					n += toolTabs[i].ToolboxItemCount;
				}
				//
				tools = new ToolboxItem[n];
				n = 0;
				for (int i = 0; i < toolTabs.Length; i++)
				{
					for (int j = 0; j < toolTabs[i].ToolboxItemCount; j++)
					{
						tools[j + n] = toolTabs[i][j + 1];
					}
					n += toolTabs[i].ToolboxItemCount;
				}
			}
			else
			{
				tools = new ToolboxItem[0];
			}
			return new ToolboxItemCollection(tools);
		}
		public void ClearItemSelection()
		{
			SelectPointer();
		}
		/// Resets the selection to the pointer. Note that this is the only method
		/// which allows our IToolboxService to set our selection. It calls this method
		/// after a tool has been used.
		public void SelectPointer()
		{
			if (this.selectedList != null)
			{
				if (this.selectedList.SelectedIndex > 0)
				{
					selectedList.Invalidate(selectedList.GetItemRectangle(selectedList.SelectedIndex));
					selectedList.SelectedIndex = 0;
					selectedList.Invalidate(selectedList.GetItemRectangle(0));
				}
			}
		}
		public void CreateCommonObjTab()
		{
		}
		public void CreatePrimaryObjTab()
		{
			Bitmap bmpInt = new Bitmap(GetType(), "int.bmp");
			Bitmap bmpDec = new Bitmap(GetType(), "decimal.bmp");
			Bitmap bmpAbc = new Bitmap(GetType(), "abc.bmp");
			Bitmap bmpObj = new Bitmap(GetType(), "obj.bmp");
			Bitmap bmpBool = new Bitmap(GetType(), "bool.bmp");
			Bitmap bmpDate = new Bitmap(GetType(), "date.bmp");
			Bitmap bmpByte = new Bitmap(GetType(), "byte.bmp");
			Bitmap bmpEarg = new Bitmap(GetType(), "eargv.bmp");
			Bitmap bmpSByte = new Bitmap(GetType(), "sbyte.bmp");
			Bitmap bmpChar = new Bitmap(GetType(), "char.bmp");
			ToolboxTab2 tab = CreateTab("Primary Objects", true, -1);
			tab.Persist = false;
			tab.AddItem(new xToolboxItem(typeof(object), bmpObj));
			tab.AddItem(new xToolboxItem(typeof(char), bmpChar));
			tab.AddItem(new xToolboxItem(typeof(sbyte), bmpSByte));
			tab.AddItem(new xToolboxItem(typeof(Int16), bmpInt));
			tab.AddItem(new xToolboxItem(typeof(Int32), bmpInt));
			tab.AddItem(new xToolboxItem(typeof(Int64), bmpInt));
			tab.AddItem(new xToolboxItem(typeof(byte), bmpByte));
			tab.AddItem(new xToolboxItem(typeof(UInt16), bmpInt));
			tab.AddItem(new xToolboxItem(typeof(UInt32), bmpInt));
			tab.AddItem(new xToolboxItem(typeof(UInt64), bmpInt));
			tab.AddItem(new xToolboxItem(typeof(string), bmpAbc));
			tab.AddItem(new xToolboxItem(typeof(float), bmpDec));
			tab.AddItem(new xToolboxItem(typeof(double), bmpDec));
			tab.AddItem(new xToolboxItem(typeof(bool), bmpBool));
			tab.AddItem(new xToolboxItem(typeof(DateTime), bmpDate));
			tab.AddItem(new xToolboxItem(typeof(EventArgs), bmpEarg));
			tab.AddItem(new xToolboxItem(typeof(void), bmpObj, "Object"));
			tab.AdjustSize();
		}
		public void RemoveTab(ToolboxTab2 tab)
		{
			if (toolTabs != null)
			{
				for (int i = 0; i < toolTabs.Length; i++)
				{
					if (toolTabs[i] == tab)
					{
						removeTab(i);
						break;
					}
				}
			}
		}
		public bool TabNameExists(string name)
		{
			if (toolTabs != null)
			{
				for (int i = 0; i < toolTabs.Length; i++)
				{
					if (toolTabs[i].Name == name)
					{
						return true;
					}
				}
			}
			return false;
		}
		#endregion

		#region Component Designer generated code

		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		private void InitializeComponent()
		{
			this.titleBar = new LabelToolboxTitle();
			this.panelTabs = new TabsPanel();
			this.SuspendLayout();
			//
			// titleBar
			//
			this.titleBar.Location = new System.Drawing.Point(0, 0);
			//
			// panelTabs
			//
			this.panelTabs.Location = new System.Drawing.Point(0, 18);
			this.panelTabs.BorderStyle = BorderStyle.None;
			this.panelTabs.AutoScroll = false;
			// 
			// ToolboxPane
			// 
			this.BackColor = System.Drawing.Color.Black;
			this.Controls.AddRange(new System.Windows.Forms.Control[] { titleBar,panelTabs
																		  });
			this.Name = "ToolboxPane";
			this.Size = new System.Drawing.Size(288, 552);
			this.BorderStyle = BorderStyle.Fixed3D;
			this.AutoScroll = false;
			this.ResumeLayout(false);

		}
		#endregion

		#region override
		protected override void OnResize(EventArgs e)
		{
			if (this.ClientSize.Width < 2)
				this.Width = 10;
			titleBar.Width = this.ClientSize.Width - 1;
			panelTabs.Width = this.ClientSize.Width - 1;
			if (this.ClientSize.Height > panelTabs.Top + 1)
			{
				panelTabs.Height = this.ClientSize.Height - panelTabs.Top - 1;
			}
			adjustTabWith();
		}
		#endregion

		#region IToolboxService Members

		public void AddCreator(ToolboxItemCreatorCallback creator, string format, IDesignerHost host)
		{

		}

		public void AddCreator(ToolboxItemCreatorCallback creator, string format)
		{

		}
		/// <summary>
		/// IDesignerHost is a service provided by DesignSurface
		/// </summary>
		/// <param name="toolboxItem"></param>
		/// <param name="category"></param>
		/// <param name="host"></param>
		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, IDesignerHost host)
		{
			ToolboxTab2 tab = CreateTab(category, true, -1);
			tab.AddItem(toolboxItem);
		}

		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, IDesignerHost host)
		{
			if (toolTabs != null)
			{
				if (selectedTabIndex >= 0 && selectedTabIndex < toolTabs.Length)
				{
					toolTabs[selectedTabIndex].AddItem(toolboxItem);
				}
			}
		}
		public void AddToolboxItem(string tab, Type tp, string name, Bitmap img, Dictionary<string, object> properties)
		{
			xToolboxItem ti = createItem(tp, name, img, properties);
			AddToolboxItem(ti, tab);
		}
		public void AddToolboxItem(string tab, Guid projectGuid, Type tp, string name, Bitmap img, Dictionary<string, object> properties)
		{
			xToolboxItem ti = createItem(tp, name, img, properties);
			ToolboxTabProject ptab = CreateProjectTab(tab, projectGuid);
			ptab.AddItem(ti);
		}
		public void AddToolboxItem(ToolboxItem toolboxItem, string category)
		{
			ToolboxTab2 tab = CreateTab(category, true, -1);
			tab.AddItem(toolboxItem);
		}

		public void AddToolboxItem(ToolboxItem toolboxItem)
		{
			if (toolTabs != null)
			{
				if (selectedTabIndex >= 0 && selectedTabIndex < toolTabs.Length)
				{
					toolTabs[selectedTabIndex].AddItem(toolboxItem);
				}
			}
		}

		public ToolboxItem DeserializeToolboxItem(object serializedObject, IDesignerHost host)
		{
			return (ToolboxItem)((DataObject)serializedObject).GetData(typeof(ToolboxItem));
		}

		public ToolboxItem DeserializeToolboxItem(object serializedObject)
		{
			return DeserializeToolboxItem(serializedObject, host);
		}

		public ToolboxItem GetSelectedToolboxItem(IDesignerHost host)
		{
			if (SelectedItem != null && SelectedItem.TypeName != null)
			{
				return SelectedItem;
			}
			return null;
		}

		public ToolboxItem GetSelectedToolboxItem()
		{
			return this.GetSelectedToolboxItem(this.host);
		}

		public ToolboxItemCollection GetToolboxItems(string category, IDesignerHost host)
		{
			return GetToolsFromCategory(category);
		}

		public ToolboxItemCollection GetToolboxItems(string category)
		{
			return this.GetToolboxItems(category, this.host);
		}

		public ToolboxItemCollection GetToolboxItems(IDesignerHost host)
		{
			return GetAllTools();
		}

		public ToolboxItemCollection GetToolboxItems()
		{
			return GetAllTools();
		}

		public bool IsSupported(object serializedObject, ICollection filterAttributes)
		{
			return true;
		}

		public bool IsSupported(object serializedObject, IDesignerHost host)
		{
			return true;
		}

		public bool IsToolboxItem(object serializedObject, IDesignerHost host)
		{
			if (this.DeserializeToolboxItem(serializedObject, host) != null)
			{
				return true;
			}
			return false;
		}

		public bool IsToolboxItem(object serializedObject)
		{
			return IsToolboxItem(serializedObject, this.host);
		}

		public void RemoveCreator(string format, IDesignerHost host)
		{

		}

		public void RemoveCreator(string format)
		{

		}

		public void RemoveToolboxItem(ToolboxItem toolboxItem, string category)
		{

		}

		public void RemoveToolboxItem(ToolboxItem toolboxItem)
		{

		}

		public void SelectedToolboxItemUsed()
		{
			ClearItemSelection();
		}

		public object SerializeToolboxItem(ToolboxItem toolboxItem)
		{
			return new DataObject2(toolboxItem);
		}

		public bool SetCursor()
		{
			if (SelectedItem != null && SelectedItem.TypeName != null)
			{
				Cursor.Current = Cursors.Cross;
				return true;
			}
			return false;
		}

		public void SetSelectedToolboxItem(ToolboxItem toolboxItem)
		{
			if (selectedList != null)
			{
				xToolboxItem x = toolboxItem as xToolboxItem;
				if (x != null)
				{
					Type t = x.Type;
					for (int i = 0; i < selectedList.Items.Count; i++)
					{
						xToolboxItem item = selectedList.Items[i] as xToolboxItem;
						if (item.Type.Equals(t))
						{
							selectedList.SelectedIndex = i;
							break;
						}
					}
				}
			}
		}

		#endregion

		#region ILimnorToolbox Members
		public void SetChanged(bool hasChanged)
		{
			this.dirty = hasChanged;
		}
		public void RemoveTab(string tab)
		{
			if (toolTabs != null)
			{
				for (int i = 0; i < toolTabs.Length; i++)
				{
					if (string.Compare(toolTabs[i].Name, tab, StringComparison.OrdinalIgnoreCase) == 0)
					{
						removeTab(i);
					}
				}
			}
		}
		public void AdjustTabSize(string tab)
		{
			int n = TabCount;
			for (int i = 0; i < n; i++)
			{
				if (string.Compare(tab, toolTabs[i].Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					toolTabs[i].AdjustSize();
					break;
				}
			}
		}
		public void AdjustTabSize(Guid projectGuid)
		{
			int n = TabCount;
			for (int i = 0; i < n; i++)
			{
				ToolboxTabProject ptab = toolTabs[i] as ToolboxTabProject;

				if (ptab != null && ptab.ProjectGuid == projectGuid)
				{
					toolTabs[i].AdjustSize();
					break;
				}
			}
		}
		public void AdjustTabSizes()
		{
			int n = TabCount;
			if (n > 0)
			{
				OnResize(null);
				AdjustTabPos(0);
			}
		}
		public void AddTab(string tab)
		{
			CreateTab(tab, true, -1);
		}
		public void AddTab(string tab, bool readOnly, bool persist, int idx, bool clearItems)
		{
			ToolboxTab2 t = CreateTab(tab, readOnly, idx);
			t.Persist = persist;
			t.Visible = true;
			if (clearItems)
			{
				t.RemoveItems();
			}
		}
		public void DataUsed()
		{
			if (selectedList != null)
			{
				selectedList.SelectedIndex = -1;
			}
			int n = TabCount;
			for (int i = 0; i < n; i++)
			{
				toolTabs[i].SetSelectIndex(-1);
			}
		}

		#endregion
	}
	public class DataObject2 : DataObject
	{
		ToolboxItem ti;
		public DataObject2(object data)
			: base(data)
		{
			if (data is ToolboxItem)
			{
				ti = (ToolboxItem)data;
			}
		}
		public override object GetData(Type format)
		{
			if (format.Equals(typeof(ToolboxItem)) || format.IsSubclassOf(typeof(ToolboxItem)))
			{
				return ti;
			}
			return base.GetData(format);
		}
	}
}
