/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using VPL;

namespace LimnorDatabase
{
	public partial class DlgConnectionManager : Form
	{
		#region fields and constructors
		public ConnectionItem SelectedConnection;
		private List<ConnectionItem> _connectionList;
		private bool _loaded;
		private ConnectionItem _currentSelection;
		private List<Guid> _projectUsages;
		enum enumDlgUsage { All, Project, Partial };
		private enumDlgUsage _dlgUsage = enumDlgUsage.All;
		private object _listOwner;
		public DlgConnectionManager()
		{
			InitializeComponent();

			Text = string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"Database connections in {0}", System.Environment.MachineName);
		}
		#endregion

		#region Properties
		public bool UseProjectScope
		{
			get
			{
				if (_dlgUsage == enumDlgUsage.Project)
					return true;
				return false;
			}
			set
			{
				if (value)
				{
					_dlgUsage = enumDlgUsage.Project;
				}
				else
				{
					_dlgUsage = enumDlgUsage.All;
				}
				toolStripButtonSelect.Enabled = (_dlgUsage == enumDlgUsage.Project);
				toolStripButtonRemove.Enabled = (_dlgUsage == enumDlgUsage.Project);
			}
		}
		#endregion

		#region Protected methods
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			if (!_loaded)
			{
				_loaded = true;
				Label lb = new Label();
				lb.Height = 30;
				lb.AutoSize = false;
				lb.BackColor = Color.Wheat;
				lb.Text = "Loading connections, please wait ...";
				lb.TextAlign = ContentAlignment.TopLeft;
				lb.Top = (this.ClientSize.Height - lb.Height) / 2;
				lb.Width = this.ClientSize.Width - 10;
				lb.Left = 5;
				this.Controls.Add(lb);
				lb.Visible = true;
				lb.BringToFront();
				lb.Refresh();
				ProgressBar pb = new ProgressBar();
				pb.Width = lb.Width;
				pb.Height = 10;
				pb.Location = new Point(0, 20);
				lb.Controls.Add(pb);
				pb.Visible = true;
				loadData(pb);
				this.Controls.Remove(lb);
				if (_currentSelection != null)
				{
					AddConnection(_currentSelection);
				}
			}
		}
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			try
			{
				base.OnFormClosing(e);
				save();
				if (_dlgUsage == enumDlgUsage.All)
				{
				}
				else
				{
					//merge the list?
					//disable new button so that we do not need to do merge here
					if (_dlgUsage == enumDlgUsage.Project && _listOwner != null)
					{
						if (ConnectionItem.SetProjectDatabaseConnections != null && _connectionList != null)
						{
							List<Guid> gl = new List<Guid>();
							foreach (ConnectionItem ci in _connectionList)
							{
								gl.Add(ci.ConnectionGuid);
							}
							ConnectionItem.SetProjectDatabaseConnections(gl, _listOwner);
						}
					}
				}
			}
			catch (Exception err)
			{
				FormLog.NotifyException(true, err, "Error closing connection manager. {0}", this.ToString());
			}
		}
		#endregion

		#region Methods
		public void SetConnectionList(List<ConnectionItem> list)
		{
			_connectionList = list;
		}
		public void HideOKCancelButtons()
		{
			toolStripButtonOK.Visible = false;
			toolStripButtonCancel.Visible = false;
		}
		public void SetProjectDatabaseUsages(List<Guid> gs)
		{
			_projectUsages = gs;
		}
		public void AddConnection(ConnectionItem ci)
		{
			try
			{
				if (ci == null)
					throw new ExceptionLimnorDatabase("Cannot add a null connection");
				if (_connectionList == null)
				{
					_connectionList = new List<ConnectionItem>();
				}
				foreach (ConnectionItem c in _connectionList)
				{
					if (c.ConnectionObject.ConnectionGuid.Equals(ci.ConnectionObject.ConnectionGuid))
					{
						if (!string.IsNullOrEmpty(ci.Name))
						{
							if (string.IsNullOrEmpty(c.Name))
							{
								c.Name = ci.Name;
							}
						}
						SetSelection(ci);
						TreeNodeConnectionItem tnc = treeView1.SelectedNode as TreeNodeConnectionItem;
						if (tnc != null && tnc.OwnerItem != null)
						{
							if (tnc.OwnerItem.ConnectionObject.ConnectionGuid.Equals(ci.ConnectionObject.ConnectionGuid))
							{
								tnc.Text = c.ToString();
							}
						}
						return;
					}
				}
				_connectionList.Add(ci);
				TreeNodeConnectionItem tc = new TreeNodeConnectionItem(ci);
				treeView1.Nodes.Add(tc);
				treeView1.SelectedNode = tc;
			}
			catch (Exception err)
			{
				FormLog.NotifyException(true, err, "Error adding connection {0}", this.ToString());
			}
		}
		public void SetSelection(ConnectionItem ci)
		{
			_currentSelection = ci;
			if (ci != null)
			{
				for (int i = 0; i < treeView1.Nodes.Count; i++)
				{
					TreeNodeConnectionItem tnc = treeView1.Nodes[i] as TreeNodeConnectionItem;
					if (tnc != null)
					{
						ConnectionItem c = tnc.OwnerItem;
						if (c != null)
						{
							if (string.Compare(c.Filename, ci.Filename, StringComparison.OrdinalIgnoreCase) == 0)
							{
								treeView1.SelectedNode = tnc;
								break;
							}
						}
					}
				}
			}
		}
		public void EnableCancel(bool enable)
		{
			toolStripButtonCancel.Enabled = enable;
		}
		#endregion

		#region private methods
		private void loadData(ProgressBar pb)
		{
			try
			{
				pb.Value = 10;
				pb.Refresh();
				if (_connectionList == null)
				{
					if (UseProjectScope && ConnectionItem.GetProjectDatabaseConnections != null)
					{
						List<Guid> glst = new List<Guid>();
						_connectionList = new List<ConnectionItem>();
						if (_projectUsages != null && _projectUsages.Count > 0)
						{
							glst.AddRange(_projectUsages);
						}
						IList<Guid> gl = ConnectionItem.GetProjectDatabaseConnections(out _listOwner);
						if (gl != null && gl.Count > 0)
						{
							foreach (Guid g in gl)
							{
								if (g != Guid.Empty)
								{
									if (!glst.Contains(g))
									{
										glst.Add(g);
									}
								}
							}
						}
						if (glst.Count > 0)
						{
							foreach (Guid g in glst)
							{
								if (g != Guid.Empty)
								{
									ConnectionItem ci = ConnectionItem.LoadConnection(g, false, false);
									if (ci != null)
									{
										_connectionList.Add(ci);
									}
								}
							}
						}
						if (_listOwner != null)
						{
							this.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"Database connections for project {0}", _listOwner.ToString());
						}
					}
					else
					{
						_dlgUsage = enumDlgUsage.All;
						string dir = ConnectionItem.GetConnectionFileFolder();
						if (!System.IO.Directory.Exists(dir))
						{
							System.IO.Directory.CreateDirectory(dir);
						}
						_connectionList = new List<ConnectionItem>();

						string[] files = ConnectionItem.GetConnectionFiles();
						if (files != null && files.Length > 0)
						{
							double r = (double)85 / (double)(files.Length);
							for (int i = 0; i < files.Length; i++)
							{
								Guid g = new Guid(System.IO.Path.GetFileNameWithoutExtension(files[i]));
								ConnectionItem ci = ConnectionItem.LoadConnection(g, false, true);
								if (ci != null && !ci.EncryptConnectionString)
								{
									_connectionList.Add(ci);
									pb.Value = 10 + (int)((i + 1) * r);
									pb.Refresh();
								}
							}
						}
					}
				}
				else
				{
					_dlgUsage = enumDlgUsage.Partial;
					toolStripButtonCancel.Enabled = false;
				}
				pb.Value = 95;
				pb.Refresh();
				treeView1.Nodes.Clear();
				bool bSelected = false;
				foreach (ConnectionItem item in _connectionList)
				{
					if (item != null)
					{
						TreeNodeConnectionItem tn = new TreeNodeConnectionItem(item);
						treeView1.Nodes.Add(tn);
						if (!bSelected)
						{
							if (_currentSelection != null)
							{
								if (_currentSelection.ConnectionGuid == item.ConnectionGuid)
								{
									treeView1.SelectedNode = tn;
									bSelected = true;
								}
							}
						}
					}
				}
				if (!bSelected)
				{
					for (int i = 0; i < treeView1.Nodes.Count; i++)
					{
						TreeNodeConnectionItem tn = treeView1.Nodes[i] as TreeNodeConnectionItem;
						if (tn != null)
						{
							if (!tn.OwnerItem.IsValid)
							{
								treeView1.SelectedNode = tn;
								bSelected = true;
								break;
							}
						}
					}
				}
				if (!bSelected)
				{
					if (treeView1.Nodes.Count > 0)
					{
						for (int i = 0; i < treeView1.Nodes.Count; i++)
						{
							TreeNodeConnectionItem tn = treeView1.Nodes[i] as TreeNodeConnectionItem;
							if (tn != null)
							{
								treeView1.SelectedNode = tn;
								bSelected = true;
								break;
							}
						}
					}
				}
				pb.Value = 100;
				pb.Refresh();
			}
			catch (Exception err)
			{
				FormLog.NotifyException(true, err);
			}
		}
		private void save()
		{
			ConnectionItem cnn = propertyGrid1.SelectedObject as ConnectionItem;
			if (cnn != null)
			{
				cnn.Save();
			}
		}

		private void toolStripButtonHelp_Click(object sender, EventArgs e)
		{
			FormHelp fh = new FormHelp();
			fh.LoadData(Resource1.ConnectionManager);
			fh.ShowDialog(this);
		}

		private void toolStripButtonOK_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode != null)
			{
				TreeNodeConnectionItem tc = treeView1.SelectedNode as TreeNodeConnectionItem;
				if (tc == null)
				{
					tc = treeView1.SelectedNode.Parent as TreeNodeConnectionItem;
				}
				if (tc != null)
				{
					SelectedConnection = tc.OwnerItem;
					if (SelectedConnection != null)
					{
						this.DialogResult = DialogResult.OK;
					}
				}
			}
			if (_dlgUsage == enumDlgUsage.Project)
			{
				this.DialogResult = DialogResult.OK;
			}
		}

		private void toolStripButtonNew_Click(object sender, EventArgs e)
		{
			Connection cnn = new Connection();
			cnn.ConnectionGuid = Guid.NewGuid();
			ConnectionItem ci = new ConnectionItem(cnn);
			_connectionList.Add(ci);
			TreeNodeConnectionItem tc = new TreeNodeConnectionItem(ci);
			treeView1.Nodes.Add(tc);
			treeView1.SelectedNode = tc;
		}

		private void toolStripButtonDelete_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode != null)
			{
				TreeNodeConnectionItem tc = treeView1.SelectedNode as TreeNodeConnectionItem;
				if (tc == null)
				{
					tc = treeView1.SelectedNode.Parent as TreeNodeConnectionItem;
				}
				if (tc != null)
				{
					ConnectionItem ci = tc.OwnerItem;
					if (ci != null)
					{
						if (MessageBox.Show(this, string.Format(System.Globalization.CultureInfo.InvariantCulture, "Do you want to delete connection [{0}]?", ci.ToString()), "Database Connection Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
						{
							if (System.IO.File.Exists(ci.FullFilePath))
							{
								try
								{
									System.IO.File.Delete(ci.FullFilePath);
								}
								catch (Exception err)
								{
									MessageBox.Show(this, err.Message, "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
								}
							}
							propertyGrid1.SelectedObject = null;
							tc.Remove();
							_connectionList.Remove(ci);
							ConnectionConfig.RemoveConnection(ci.ConnectionObject.ConnectionGuid);
						}
					}
				}
			}
		}

		private void toolStripButtonCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void toolStripButtonTest_Click(object sender, EventArgs e)
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			if (treeView1.SelectedNode != null)
			{
				TreeNodeConnectionItem tc = treeView1.SelectedNode as TreeNodeConnectionItem;
				if (tc == null)
				{
					tc = treeView1.SelectedNode.Parent as TreeNodeConnectionItem;
				}
				if (tc != null)
				{
					ConnectionItem ci = tc.OwnerItem;
					if (ci != null)
					{
						try
						{
							if (ci.ConnectionObject.TestConnection(this, true))
							{
								tc.ImageIndex = TreeNodeConnectionItem.IMG_CNN_OK;
								tc.SelectedImageIndex = TreeNodeConnectionItem.IMG_CNN_OK;
							}
							else
							{
								tc.ImageIndex = TreeNodeConnectionItem.IMG_CNN_ERR;
								tc.SelectedImageIndex = TreeNodeConnectionItem.IMG_CNN_ERR;
							}
						}
						catch (Exception err)
						{
							MessageBox.Show(this, err.Message, "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
							tc.ImageIndex = TreeNodeConnectionItem.IMG_CNN_ERR;
							tc.SelectedImageIndex = TreeNodeConnectionItem.IMG_CNN_ERR;
						}
					}
				}
			}
			this.Cursor = System.Windows.Forms.Cursors.Default;
		}

		private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (e != null)
			{
				TreeNodeConnectionItem tc = e.Node as TreeNodeConnectionItem;
				if (tc != null)
				{
					tc.LoadNextLevel();
				}
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			save();
			if (e != null)
			{
				TreeNodeConnectionItem tc = e.Node as TreeNodeConnectionItem;
				if (tc == null && e.Node != null)
				{
					tc = e.Node.Parent as TreeNodeConnectionItem;
				}
				if (tc != null)
				{
					tc.OnSelected();
					propertyGrid1.SelectedObject = tc.OwnerItem;
				}
				else
				{
					propertyGrid1.SelectedObject = null;
				}
			}
		}

		private void toolStripButtonCleanup_Click(object sender, EventArgs e)
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			List<TreeNodeConnectionItem> delItems = new List<TreeNodeConnectionItem>();
			for (int i = 0; i < treeView1.Nodes.Count; i++)
			{
				TreeNodeConnectionItem tc = treeView1.Nodes[i] as TreeNodeConnectionItem;
				if (tc != null)
				{
					tc.OnSelected();
					ConnectionItem ci = tc.OwnerItem;
					if (ci != null)
					{
						if (string.IsNullOrEmpty(ci.DatabaseType) && string.IsNullOrEmpty(ci.ConnectionStringPlain))
						{
							delItems.Add(tc);
						}
					}
					else
					{
						delItems.Add(tc);
					}
				}
			}

			foreach (TreeNodeConnectionItem tn in delItems)
			{
				ConnectionItem ci = tn.OwnerItem;
				if (ci != null)
				{
					if (propertyGrid1.SelectedObject == ci)
					{
						propertyGrid1.SelectedObject = null;
					}
					if (System.IO.File.Exists(ci.FullFilePath))
					{
						try
						{
							System.IO.File.Delete(ci.FullFilePath);
						}
						catch (Exception err)
						{
							MessageBox.Show(this, err.Message, "Remove Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
					_connectionList.Remove(ci);
					ConnectionConfig.RemoveConnection(ci.ConnectionObject.ConnectionGuid);
				}
				tn.Remove();
			}
			this.Cursor = System.Windows.Forms.Cursors.Default;
		}
		private void toolStripButtonSelect_Click(object sender, EventArgs e)
		{
			if (_dlgUsage != enumDlgUsage.All)
			{
				DlgConnectionManager dlg = new DlgConnectionManager();
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					if (dlg.SelectedConnection != null)
					{
						bool bFound = false;
						foreach (ConnectionItem ci in _connectionList)
						{
							if (ci != null && ci.ConnectionGuid == dlg.SelectedConnection.ConnectionGuid)
							{
								bFound = true;
							}
						}
						if (!bFound)
						{
							_connectionList.Add(dlg.SelectedConnection);
						}
						bFound = false;
						for (int i = 0; i < treeView1.Nodes.Count; i++)
						{
							TreeNodeConnectionItem tn = treeView1.Nodes[i] as TreeNodeConnectionItem;
							if (tn != null)
							{
								if (tn.OwnerItem.ConnectionGuid == dlg.SelectedConnection.ConnectionGuid)
								{
									bFound = true;
									treeView1.SelectedNode = tn;
									break;
								}
							}
						}
						if (!bFound)
						{
							TreeNodeConnectionItem tnc = new TreeNodeConnectionItem(dlg.SelectedConnection);
							treeView1.Nodes.Add(tnc);
							treeView1.SelectedNode = tnc;
						}
					}
				}
			}
		}
		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (string.CompareOrdinal(e.ChangedItem.PropertyDescriptor.Name, "Name") == 0)
			{
				ConnectionItem ci = propertyGrid1.SelectedObject as ConnectionItem;
				if (ci != null)
				{
					TreeNodeConnectionItem tnc = treeView1.SelectedNode as TreeNodeConnectionItem;
					if (tnc != null)
					{
						if (tnc.OwnerItem.ConnectionGuid == ci.ConnectionGuid)
						{
							tnc.Text = ci.ToString();
						}
					}
				}
			}
		}
		private void toolStripButtonRemove_Click(object sender, EventArgs e)
		{
			TreeNodeConnectionItem tnc = treeView1.SelectedNode as TreeNodeConnectionItem;
			if (tnc != null)
			{
				if (MessageBox.Show(this, "Do you want to remove thid connection?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					if (_connectionList != null)
					{
						for (int i = 0; i < _connectionList.Count; i++)
						{
							if (_connectionList[i].ConnectionGuid == tnc.OwnerItem.ConnectionGuid)
							{
								_connectionList.Remove(_connectionList[i]);
								break;
							}
						}
					}
					tnc.Remove();
				}
			}
		}
		#endregion


	}
}
