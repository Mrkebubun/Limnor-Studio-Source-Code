/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Toolbox system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing;
using VOB;
using VPL;
using TraceLog;
using System.Drawing.Drawing2D;

namespace XToolbox2
{
	/// <summary>
	/// ToolboxTabs.
	/// </summary>
	public class ToolboxTab2 : Panel
	{
		private bool selected;
		private clsToolList itemList;
		private LabelToolboxTabTitle lblTitle;
		private int selectedIndex = 0;
		private int index;
		private ToolboxPane2 toolbox;
		private bool _readOnly;
		private bool _save = true;
		//
		public ToolboxTab2(ToolboxPane2 owner, string name, int idx, bool readOnly)
		{
			this.AutoScroll = false;
			this.AutoSize = false;
			this.HScroll = false;
			this.VScroll = false;

			_readOnly = readOnly;
			index = idx;
			toolbox = owner;
			itemList = new clsToolList();
			itemList.AllowDrop = true;
			itemList.ScrollAlwaysVisible = false;
			itemList.SelectionMode = SelectionMode.One;
			itemList.HorizontalScrollbar = false;
			itemList.IntegralHeight = false;
			itemList.BackColor = System.Drawing.Color.LightSlateGray;
			itemList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;

			itemList.Name = "tools_" + name;
			itemList.Size = new System.Drawing.Size(this.ClientSize.Width, 200);
			itemList.TabIndex = 0;
			itemList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.list_KeyDown);
			itemList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.list_MouseDown);
			itemList.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.list_MeasureItem);
			itemList.SelectedIndexChanged += new EventHandler(itemList_SelectedIndexChanged);
			itemList.MouseMove += new MouseEventHandler(itemList_MouseMove);
			itemList.Items.Add(new xToolboxItem((Type)null));
			itemList.SelectedIndex = 0;
			//
			if (_readOnly)
				lblTitle = new LabelToolboxTabTitle(this);
			else
				lblTitle = new LabelToolboxTabTitleCust(this);
			lblTitle.Name = "title_" + name;
			lblTitle.Text = name;
			lblTitle.Width = this.ClientSize.Width;
			lblTitle.Location = new System.Drawing.Point(0, 0);
			itemList.Location = new System.Drawing.Point(0, lblTitle.Height);
			itemList.Visible = true;
			lblTitle.Visible = true;
			//
			Controls.Add(lblTitle);
			Controls.Add(itemList);
			//
			this.Name = name;
		}
		public void SetTitle(string title)
		{
			lblTitle.Text = title;
		}
		public void SetSelectIndex(int i)
		{
			if (i > -2 && i < itemList.Items.Count)
			{
				itemList.SelectedIndex = i;
			}
		}
		void itemList_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left || itemList.SelectedIndex <= 0 || itemList.SelectedIndex != nSelectedIndex)
			{
				bPrepareDrag = false;
			}
			if (bPrepareDrag)
			{
				if (Math.Abs(e.X - x0) > 3 || Math.Abs(e.Y - y0) > 3)
				{
					ToolboxItem tbi = itemList.Items[itemList.SelectedIndex] as ToolboxItem;
					xToolboxItem xt = tbi as xToolboxItem;
					InterfaceVOB vob = ((IServiceContainer)toolbox.Host).GetService(typeof(InterfaceVOB)) as InterfaceVOB;
					IToolboxService tbs = ((IServiceContainer)toolbox.Host).GetService(typeof(IToolboxService)) as IToolboxService;
					if (tbs == null)
					{
						if (vob == null)
						{
							bool b = true;
							TraceLogClass log = new TraceLogClass();
							log.Log("service InterfaceVOB not available", ref b);
						}
						else
						{
							PassData data = new PassData();
							vob.SendNotice(enumVobNotice.GetToolbox, data);
							tbs = data.Data as IToolboxService;
						}
					}
					if (tbs != null)
					{
						bool bCanDrop = true;
						if (vob != null)
						{
							if (xt != null)
							{
								if (xt.Properties != null && xt.Properties.Contains("ClassId"))
								{
									ToolboxItemXType.SelectedToolboxClassId = (UInt32)(xt.Properties["ClassId"]);
									ToolboxItemXType.SelectedToolboxType = null;
									ToolboxItemXType.SelectedToolboxTypeKey = null;
									ToolboxItemXType.SelectedToolboxClassName = (string)(xt.Properties["DisplayName"]);
								}
								else
								{
									PassData pd = new PassData();
									pd.Key = xt.Type;
									pd.Data = true;
									pd.Attributes = xt.Properties;
									vob.SendNotice(enumVobNotice.ObjectCanCreate, pd);
									bCanDrop = (bool)pd.Data;
									if (bCanDrop && pd.Attributes != null)
									{
										ToolboxItem tx0 = pd.Attributes as ToolboxItem;
										if (tx0 != null)
										{
											tbi = tx0;
										}
									}
								}
							}
						}
						if (bCanDrop)
						{
							try
							{
								DataObject d = tbs.SerializeToolboxItem(tbi) as DataObject;
								toolbox.HideToolBox(this, null);
								itemList.DoDragDrop(d, DragDropEffects.Copy);
							}
							catch (Exception ex)
							{
								bool b = true;
								TraceLogClass log = new TraceLogClass();
								log.Log(this.FindForm(), ex, ref b);
							}
						}
					}
				}
			}
		}
		public virtual bool Persist
		{
			get
			{
				return _save;
			}
			set
			{
				_save = value;
			}
		}
		public bool IsCustom
		{
			get
			{
				return !_readOnly;
			}
		}
		public void RemoveTab()
		{
			if (toolbox != null)
			{
				toolbox.RemoveTab(this);
			}
		}
		void itemList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (toolbox != null)
			{
				toolbox.SetSelectedList(Index, itemList);
			}
		}
		/// <summary>
		/// it does not include the Pointer
		/// </summary>
		public int ToolboxItemCount
		{
			get
			{
				return itemList.Items.Count - 1;
			}
		}
		/// <summary>
		/// it includes the Pointer
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ToolboxItem this[int index]
		{
			get
			{
				return (ToolboxItem)itemList.Items[index];
			}
		}
		void miAdd_Click(object sender, EventArgs e)
		{
			if (toolbox != null && itemList != null)
			{
				string[] saTypes = TypeImporter.SelectTypes();
				if (saTypes != null && saTypes.Length > 0)
				{
					for (int i = 0; i < saTypes.Length; i++)
					{
						Type tp = Type.GetType(saTypes[i]);
						if (tp != null)
						{
							xToolboxItem x;
							x = new xToolboxItem(tp);
							itemList.Items.Add(x);
						}
						else
						{
							MessageBox.Show(FindForm(), string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"Cannot load {0}", saTypes[i]), "Load types", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
					AdjustSize();
					toolbox.Changed = true;
					toolbox.AdjustTabPos(this.Index - 1);
				}
			}
		}
		public void SelectedToolboxItemUsed()
		{
			if (selected && itemList != null && itemList.SelectedIndex >= 0)
			{
				itemList.Invalidate(itemList.GetItemRectangle(itemList.SelectedIndex));
				itemList.SelectedIndex = 0;
				itemList.Invalidate(itemList.GetItemRectangle(0));
			}
		}
		public void AdjustSize()
		{
			itemList.Top = lblTitle.Height;
			itemList.Height = itemList.Items.Count * itemList.ItemHeight;
			if (selected)
			{
				this.Height = lblTitle.Height + itemList.Height;
			}
			else
			{
				this.Height = lblTitle.Height;
			}
		}
		public void ToggleSelect()
		{
			selected = !selected;
			onSelectedChange();
		}
		private void onSelectedChange()
		{
			if (selected)
			{
				this.Height = lblTitle.Height + itemList.Height;
			}
			else
			{
				this.Height = lblTitle.Height;
			}
			lblTitle.Invalidate();
			if (toolbox != null)
			{
				toolbox.AdjustTabPos(this.index);
				if (selected)
				{
					toolbox.SetSelectedList(Index, itemList);
				}
				toolbox.RefreshTabs();
			}
		}
		public bool Selected
		{
			get
			{
				return selected;
			}
			set
			{
				selected = value;
				onSelectedChange();
			}
		}
		public Bitmap GetMinusImage()
		{
			if (toolbox == null)
				return null;
			return toolbox.GetMinusImage();
		}
		public Bitmap GetPlusImage()
		{
			if (toolbox == null)
				return null;
			return toolbox.GetPlusImage();
		}
		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);
			lblTitle.Width = this.ClientSize.Width;
			itemList.Width = this.ClientSize.Width;
		}
		public int Index
		{
			get
			{
				return index;
			}
			set
			{
				index = value;
				itemList.Name = "list" + index.ToString();
			}
		}
		public void ClearItems()
		{
			itemList.Items.Clear();
			itemList.Items.Add(new xToolboxItem((Type)null));
		}
		public void AddItem(ToolboxItem x)
		{
			itemList.Items.Add(x);
		}
		public void RemoveItems()
		{
			while (itemList.Items.Count > 1)
			{
				itemList.Items.RemoveAt(1);
			}
		}
		/// The toolbox can also be navigated using the keyboard commands Up, Down, and Enter.
		private void list_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			ListBox lbSender = sender as ListBox;
			Rectangle lastSelectedBounds = lbSender.GetItemRectangle(selectedIndex);
			switch (e.KeyCode)
			{
				case Keys.Up: if (selectedIndex > 0)
					{
						selectedIndex--; // change selection
						lbSender.SelectedIndex = selectedIndex;
						lbSender.Invalidate(lastSelectedBounds); // clear old highlight
						lbSender.Invalidate(lbSender.GetItemRectangle(selectedIndex)); // add new one
					}
					break;
				case Keys.Down: if (selectedIndex + 1 < lbSender.Items.Count)
					{
						selectedIndex++; // change selection
						lbSender.SelectedIndex = selectedIndex;
						lbSender.Invalidate(lastSelectedBounds); // clear old highlight
						lbSender.Invalidate(lbSender.GetItemRectangle(selectedIndex)); // add new one
					}
					break;
				case Keys.Enter:
					if (toolbox != null && toolbox.Host != null)
					{
						IToolboxUser tbu = toolbox.Host.GetDesigner(toolbox.Host.RootComponent) as IToolboxUser;
						if (tbu != null)
						{
							// Enter means place the tool with default location and default size.
							tbu.ToolPicked((ToolboxItem)(lbSender.Items[selectedIndex]));
						}
					}
					break;
			}
		}
		int x0 = 0, y0 = 0, nSelectedIndex = -1;
		bool bPrepareDrag = false;
		private void list_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Regardless of which kind of click this is, we need to change the selection.
			// First we grab the bounds of the old selected tool so that we can de-higlight it.
			//
			ListBox lbSender = sender as ListBox;
			if (selectedIndex < 0)
				selectedIndex = 0;
			bPrepareDrag = false;
			selectedIndex = lbSender.IndexFromPoint(e.X, e.Y); // change our selection
			if (selectedIndex < 0)
				selectedIndex = 0;
			lbSender.SelectedIndex = selectedIndex;
			if (selectedIndex >= 0)
			{
				if (e.Button == MouseButtons.Right)
				{
					ContextMenu mnu = new ContextMenu();
					MenuItem mi;
					if (!_readOnly)
					{
						mi = new MenuItem("Add Toolbox Item");
						mi.Click += new EventHandler(miAdd_Click);
						mi.Tag = selectedIndex;
						mnu.MenuItems.Add(mi);
						if (selectedIndex > 0)
						{
							ToolboxItem item = lbSender.Items[selectedIndex] as ToolboxItem;
							mi = new MenuItem("Delete " + item.DisplayName);
							mi.Click += new EventHandler(miDel_Click);
							mi.Tag = selectedIndex;
							mnu.MenuItems.Add(mi);
						}
						Form f = lbSender.FindForm();
						if (f != null)
						{
							Point p = lbSender.Parent.PointToScreen(new Point(e.X, e.Y));
							p = f.PointToClient(p);
							mnu.Show(f, p);
						}
					}
				}
				else
				{
					if (toolbox != null && toolbox.Host != null)
					{
						// If this is a double-click, then the user wants to add the selected component
						// to the default location on the designer, with the default size. We call
						// ToolPicked on the current designer (as a IToolboxUser) to place the tool.
						// The IToolboxService calls SelectedToolboxItemUsed(), which calls this control's
						// SelectPointer() method.
						//

						if (e.Clicks == 2)
						{
							IToolboxUser tbu = toolbox.Host.GetDesigner(toolbox.Host.RootComponent) as IToolboxUser;
							if (tbu != null)
							{
								ToolboxItemXType.SelectedToolboxTypeKey = string.Empty;
								ToolboxItem ti = (ToolboxItem)(lbSender.Items[selectedIndex]);
								xToolboxItem xti = ti as xToolboxItem;
								if (xti != null)
								{
									if (typeof(ToolboxItemXType).IsAssignableFrom(xti.Type))
									{
										ToolboxItemXType.SelectedToolboxTypeKey = ti.DisplayName;
										ToolboxItemXType.SelectedToolboxClassId = 0;
										ToolboxItemXType.SelectedToolboxType = null;
									}
									else
									{
										if (xti.Properties.Contains("ClassId"))
										{
											ToolboxItemXType.SelectedToolboxClassId = (UInt32)xti.Properties["ClassId"];
											ToolboxItemXType.SelectedToolboxTypeKey = string.Empty;
											ToolboxItemXType.SelectedToolboxType = null;
										}
										if (xti.Properties.Contains("DisplayName"))
										{
											ToolboxItemXType.SelectedToolboxClassName = (string)xti.Properties["DisplayName"];
										}
									}
								}

								tbu.ToolPicked(ti);
								toolbox.HideToolBox(this, null);
							}
						}
						// Otherwise this is either a single click or a drag. Either way, we do the same
						// thing: start a drag--if this is just a single click, then the drag will
						// abort as soon as there's a MouseUp event.
						//
						else if (e.Clicks < 2)
						{
							x0 = e.X;
							y0 = e.Y;
							nSelectedIndex = selectedIndex;
							bPrepareDrag = true;
						}
					}
				}
			}
		}
		private void list_MeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
		{
			ListBox lbSender = sender as ListBox;
			ToolboxItem tbi = lbSender.Items[e.Index] as ToolboxItem;
			Size textSize = e.Graphics.MeasureString(tbi.DisplayName, lbSender.Font).ToSize();
			e.ItemWidth = tbi.Bitmap.Width + textSize.Width;
			if (tbi.Bitmap.Height > textSize.Height)
			{
				e.ItemHeight = tbi.Bitmap.Height;
			}
			else
			{
				e.ItemHeight = textSize.Height;
			}
		}
		void miDel_Click(object sender, EventArgs e)
		{
			if (itemList.SelectedIndex > 0)
			{
				if (MessageBox.Show(this.FindForm(), "Do you want to remove this item?", "Toolbox", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					xToolboxItem x = itemList.Items[itemList.SelectedIndex] as xToolboxItem;
					if (x != null)
					{
						itemList.Items.RemoveAt(itemList.SelectedIndex);
						AdjustSize();
						toolbox.Changed = true;
						toolbox.AdjustTabPos(this.Index - 1);
					}
				}
			}
		}
	}
}
