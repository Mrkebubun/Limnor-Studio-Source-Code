/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for dlgPropDataLink.
	/// </summary>
	public class dlgPropDataLink : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		//		bool bLoading = false;
		System.Windows.Forms.ComboBox cbx2;
		public DataBind objRet = null;
		System.Data.DataSet ds = new System.Data.DataSet("Relation");
		public dlgPropDataLink()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(192)));
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(376, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "Data Field Mapping";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// dataGrid1
			// 
			this.dataGrid1.AllowNavigation = false;
			this.dataGrid1.AllowSorting = false;
			this.dataGrid1.CaptionVisible = false;
			this.dataGrid1.DataMember = "";
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(8, 48);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.Size = new System.Drawing.Size(376, 240);
			this.dataGrid1.TabIndex = 1;
			this.dataGrid1.CurrentCellChanged += new System.EventHandler(this.dataGrid1_CurrentCellChanged);
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(120, 296);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 2;
			this.btOK.Text = "&OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(200, 296);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 3;
			this.btCancel.Text = "&Cancel";
			// 
			// dlgPropDataLink
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(394, 336);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.dataGrid1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgPropDataLink";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Data Link";
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(DataBind dbd)
		{
			objRet = dbd;
			//
			ds.Tables.Clear();
			ds.Tables.Add("Links");
			//
			ds.Tables[0].Columns.Add();
			ds.Tables[0].Columns.Add();
			//
			ds.Tables[0].Columns[0].Caption = "Fields to be Updated";
			ds.Tables[0].Columns[0].ColumnName = "Destination";
			ds.Tables[0].Columns[0].DataType = typeof(string);
			ds.Tables[0].Columns[0].MaxLength = 120;
			ds.Tables[0].Columns[0].ReadOnly = true;
			//
			ds.Tables[0].Columns[1].Caption = "Source Fields";
			ds.Tables[0].Columns[1].ColumnName = "Source";
			ds.Tables[0].Columns[1].DataType = typeof(string);
			ds.Tables[0].Columns[1].MaxLength = 120;
			//			ds.Tables[0].Columns[1].ReadOnly = true;
			//
			DataGridColumnStyle TextCol;
			DataGridTableStyle myGridTableStyle = new DataGridTableStyle();
			myGridTableStyle.MappingName = "Links";
			//
			TextCol = new DataGridTextBoxColumn();
			TextCol.MappingName = ds.Tables[0].Columns[0].ColumnName;
			TextCol.HeaderText = ds.Tables[0].Columns[0].Caption;
			TextCol.Width = 160;
			myGridTableStyle.GridColumnStyles.Add(TextCol);
			//
			TextCol = new DataGridTextBoxColumn();
			TextCol.MappingName = ds.Tables[0].Columns[1].ColumnName;
			TextCol.HeaderText = ds.Tables[0].Columns[1].Caption;
			TextCol.Width = 160;
			myGridTableStyle.GridColumnStyles.Add(TextCol);
			//
			dataGrid1.TableStyles.Clear();
			dataGrid1.TableStyles.Add(myGridTableStyle);
			//
			dataGrid1.SetDataBinding(ds, "Links");
			dataGrid1.ReadOnly = true;
			//
			int i;
			int n = objRet.SourceFieldCount;
			if (n == 0)
			{
				MessageBox.Show("Linked data source not found (SourceFieldCount = 0)");
				return;
			}
			cbx2 = new System.Windows.Forms.ComboBox();
			cbx2.Parent = dataGrid1;
			cbx2.Visible = false;
			cbx2.Left = 0;
			cbx2.SelectedIndexChanged += new EventHandler(cbx2_SelectedIndexChanged);
			cbx2.Items.Add("");
			for (i = 0; i < n; i++)
			{
				cbx2.Items.Add(objRet.SourceFields[i]);
			}
			object[] vs;
			for (i = 0; i < n; i++)
			{
				vs = new object[2];
				vs[0] = objRet.SourceFields[i];// flds[i].Name; //field to be updated
				vs[1] = objRet.GetMappedField(objRet.SourceFields[i]); //source field
				ds.Tables[0].Rows.Add(vs);
			}
		}

		private void dataGrid1_CurrentCellChanged(object sender, System.EventArgs e)
		{
			if (dataGrid1.CurrentCell.RowNumber >= 0)
			{
				if (dataGrid1.CurrentCell.ColumnNumber == 1)
				{
					System.Drawing.Rectangle rc = dataGrid1.GetCurrentCellBounds();
					cbx2.SetBounds(rc.Left, rc.Top, rc.Width, rc.Height);
					cbx2.SelectedIndex = -1;
					if (dataGrid1.CurrentCell.RowNumber < ds.Tables[0].Rows.Count)
					{
						string s = ds.Tables[0].Rows[dataGrid1.CurrentCell.RowNumber][dataGrid1.CurrentCell.ColumnNumber].ToString();
						for (int i = 0; i < cbx2.Items.Count; i++)
						{
							if (cbx2.Items[i].ToString() == s)
							{
								cbx2.SelectedIndex = i;
								break;
							}
						}
					}
					cbx2.Visible = true;
					cbx2.BringToFront();
				}
			}
		}
		private void cbx2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = cbx2.SelectedIndex;
			if (n < 0)
				return;
			if (dataGrid1.CurrentCell.RowNumber < ds.Tables[0].Rows.Count)
			{
				ds.Tables[0].Rows[dataGrid1.CurrentRowIndex].BeginEdit();
				ds.Tables[0].Rows[dataGrid1.CurrentRowIndex][1] = cbx2.Items[n].ToString();
				ds.Tables[0].Rows[dataGrid1.CurrentRowIndex].EndEdit();
			}
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			string s1, s2;
			objRet.ClearFieldMaps();
			for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
			{
				s2 = ValueConvertor.ToString(ds.Tables[0].Rows[i][1]);
				if (s2.Length > 0)
				{
					s1 = ValueConvertor.ToString(ds.Tables[0].Rows[i][0]);
					objRet.AddFieldMap(s1, s2);
				}
			}
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}
	}
}
