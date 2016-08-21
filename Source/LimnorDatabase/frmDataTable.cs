using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for frmDataTable.
	/// </summary>
	public class frmDataTable : System.Windows.Forms.Form
	{
		public event System.EventHandler OnCellIndexChange = null;
		private EPDataGrid dataGrid1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		public EasyDataGrid tblOwner = null;
		public bool bCanClose = false;
		public frmDataTable()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			//
			dataGrid1.CurrentCellChanged += new EventHandler(cellIndexChange);
			//
		}
		void cellIndexChange(object sender,System.EventArgs e)
		{
			if( OnCellIndexChange != null )
				OnCellIndexChange(sender,e);
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dataGrid1 = new EPDataGrid();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGrid1
			// 
			this.dataGrid1.AllowSorting = false;
			this.dataGrid1.DataMember = "";
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(0, 0);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.Size = new System.Drawing.Size(344, 112);
			this.dataGrid1.TabIndex = 0;
			// 
			// frmDataTable
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(344, 110);
			this.ControlBox = false;
			this.Controls.Add(this.dataGrid1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmDataTable";
			this.ShowInTaskbar = false;
			this.Text = "Data table";
			this.Resize += new System.EventHandler(this.frmDataTable_Resize);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmDataTable_Closing);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void frmDataTable_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if( !bCanClose )
			{
				e.Cancel = true;
			}
		}

		private void frmDataTable_Resize(object sender, System.EventArgs e)
		{
			dataGrid1.SetBounds(0,0,this.ClientSize.Width,this.ClientSize.Height);
		}
        //public void SetLinks(FieldList flds,string tblName,DataSet ds, object d)
        //{
        //    dataGrid1.SetLinks(flds,tblName,ds, d);
        //    dataGrid1.CaptionText = tblName;
        //}
		public void ApplyStyle()
		{
			if( tblOwner != null )
				dataGrid1.ReadOnly = tblOwner.ReadOnly;
			dataGrid1.ApplyStyle();
		}
		public void BindData()
		{
			//      dataGrid1.ReadOnly = true;
			dataGrid1.BindData();
		}
		public int CurrentRowIndex
		{
			get
			{
				return dataGrid1.CurrentRowIndex;
			}
			set
			{
				try
				{
					dataGrid1.CurrentRowIndex = value;
				}
				catch
				{
				}
			}
		}
		public int CurrentColumnIndex
		{
			get
			{
				return dataGrid1.CurrentCell.ColumnNumber;
			}
			set
			{
				try
				{
                    dataGrid1.CurrentColumnIndex = value;
				}
				catch
				{
				}
			}
		}
	}
}
