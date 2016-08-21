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
	/// Summary description for dlgPropFields.
	/// </summary>
	public class dlgPropFields : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cbFile;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox cbVisible;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbxReadOnly;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtWidth;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		ListViewItem item = null;
		public FieldList fields = null;
		public EasyQuery query = null;
		public IDataBinder ownerPerformer = null;
		//
		//
		const int IDX_DATATYPE = 1;
		const int IDX_IDENTITY = 2;
		const int IDX_ISFILE = 3;
		const int IDX_WIDTH = 4;
		const int IDX_VISIBLE = 5;
		private System.Windows.Forms.Button btML;
		private System.Windows.Forms.Button btMLLcap;
		private System.Windows.Forms.Label lblDataType;
		private System.Windows.Forms.ComboBox cbDataType;
		private System.Windows.Forms.Button btLookup;
		private System.Windows.Forms.TextBox txtLookup;
		private System.Windows.Forms.CheckBox chkIndexed;
		private System.Windows.Forms.Button btHelpLookup;
		private System.Windows.Forms.Label lblHelpLookup;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cbxAlignment;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox cbxHdrAlign;
		private System.Windows.Forms.TextBox txtFormat;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button btHelpFormat;
		const int IDX_READONLY = 6;
		public dlgPropFields()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			System.Windows.Forms.Button bt = new Button();
			bt.Width = 30;
			bt.Height = 30;
			bt.Text = "X";
			lblHelpLookup.Controls.Add(bt);
			bt.Left = lblHelpLookup.Width - bt.Width;
			bt.Top = lblHelpLookup.Height - bt.Height;
			bt.Visible = true;
			bt.Click += new EventHandler(lblHelpLookup_Click);
			//
			//
			cbxAlignment.Items.Add(System.Windows.Forms.HorizontalAlignment.Center);
			cbxAlignment.Items.Add(System.Windows.Forms.HorizontalAlignment.Left);
			cbxAlignment.Items.Add(System.Windows.Forms.HorizontalAlignment.Right);
			//
			cbxHdrAlign.Items.Add(System.Windows.Forms.HorizontalAlignment.Center);
			cbxHdrAlign.Items.Add(System.Windows.Forms.HorizontalAlignment.Left);
			cbxHdrAlign.Items.Add(System.Windows.Forms.HorizontalAlignment.Right);

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
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.lblDataType = new System.Windows.Forms.Label();
			this.cbDataType = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.cbFile = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.cbVisible = new System.Windows.Forms.ComboBox();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.cbxReadOnly = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtWidth = new System.Windows.Forms.TextBox();
			this.btML = new System.Windows.Forms.Button();
			this.btMLLcap = new System.Windows.Forms.Button();
			this.btLookup = new System.Windows.Forms.Button();
			this.txtLookup = new System.Windows.Forms.TextBox();
			this.chkIndexed = new System.Windows.Forms.CheckBox();
			this.btHelpLookup = new System.Windows.Forms.Button();
			this.lblHelpLookup = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.cbxAlignment = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.cbxHdrAlign = new System.Windows.Forms.ComboBox();
			this.txtFormat = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.btHelpFormat = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader3,
																						this.columnHeader4,
																						this.columnHeader5,
																						this.columnHeader6,
																						this.columnHeader7});
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(8, 40);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(464, 432);
			this.listView1.TabIndex = 0;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseMove);
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "FieldName";
			this.columnHeader1.Width = 80;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "DataType";
			this.columnHeader2.Width = 67;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "IsIdentity";
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "IsFile";
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Width";
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Visible";
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "ReadOnly";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(464, 23);
			this.label1.TabIndex = 1;
			this.label1.Tag = "1";
			this.label1.Text = "Set field attributes";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblDataType
			// 
			this.lblDataType.Location = new System.Drawing.Point(480, 40);
			this.lblDataType.Name = "lblDataType";
			this.lblDataType.Size = new System.Drawing.Size(100, 16);
			this.lblDataType.TabIndex = 5;
			this.lblDataType.Tag = "2";
			this.lblDataType.Text = "Data type";
			this.lblDataType.Visible = false;
			// 
			// cbDataType
			// 
			this.cbDataType.Items.AddRange(new object[] {
															"Date/Time",
															"Date",
															"Timestamp",
                                                            "Time"});
			this.cbDataType.Location = new System.Drawing.Point(480, 56);
			this.cbDataType.Name = "cbDataType";
			this.cbDataType.Size = new System.Drawing.Size(96, 21);
			this.cbDataType.TabIndex = 4;
			this.cbDataType.Visible = false;
			this.cbDataType.SelectedIndexChanged += new System.EventHandler(this.cbDataType_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(480, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 7;
			this.label4.Tag = "3";
			this.label4.Text = "Is file name";
			// 
			// cbFile
			// 
			this.cbFile.Items.AddRange(new object[] {
														"True",
														"False"});
			this.cbFile.Location = new System.Drawing.Point(480, 96);
			this.cbFile.Name = "cbFile";
			this.cbFile.Size = new System.Drawing.Size(96, 21);
			this.cbFile.TabIndex = 6;
			this.cbFile.SelectedIndexChanged += new System.EventHandler(this.cbFile_SelectedIndexChanged);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(480, 120);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 16);
			this.label6.TabIndex = 11;
			this.label6.Tag = "4";
			this.label6.Text = "Visible";
			// 
			// cbVisible
			// 
			this.cbVisible.Items.AddRange(new object[] {
														   "True",
														   "False"});
			this.cbVisible.Location = new System.Drawing.Point(480, 136);
			this.cbVisible.Name = "cbVisible";
			this.cbVisible.Size = new System.Drawing.Size(96, 21);
			this.cbVisible.TabIndex = 10;
			this.cbVisible.SelectedIndexChanged += new System.EventHandler(this.cbVisible_SelectedIndexChanged);
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(8, 480);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 12;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(104, 480);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 13;
			this.btCancel.Text = "Cancel";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(480, 160);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 15;
			this.label2.Tag = "5";
			this.label2.Text = "Read only";
			// 
			// cbxReadOnly
			// 
			this.cbxReadOnly.Items.AddRange(new object[] {
															 "True",
															 "False"});
			this.cbxReadOnly.Location = new System.Drawing.Point(480, 176);
			this.cbxReadOnly.Name = "cbxReadOnly";
			this.cbxReadOnly.Size = new System.Drawing.Size(96, 21);
			this.cbxReadOnly.TabIndex = 14;
			this.cbxReadOnly.SelectedIndexChanged += new System.EventHandler(this.cbxReadOnly_SelectedIndexChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(480, 200);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 16;
			this.label5.Tag = "6";
			this.label5.Text = "Width";
			// 
			// txtWidth
			// 
			this.txtWidth.Location = new System.Drawing.Point(480, 216);
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Size = new System.Drawing.Size(96, 20);
			this.txtWidth.TabIndex = 17;
			this.txtWidth.Text = "80";
			this.txtWidth.TextChanged += new System.EventHandler(this.txtWidth_TextChanged);
			// 
			// btML
			// 
			this.btML.Location = new System.Drawing.Point(480, 480);
			this.btML.Name = "btML";
			this.btML.Size = new System.Drawing.Size(96, 23);
			this.btML.TabIndex = 18;
			this.btML.Text = "Multi-language";
			//this.btML.Click += new System.EventHandler(this.btML_Click);
			// 
			// btMLLcap
			// 
			this.btMLLcap.Location = new System.Drawing.Point(280, 480);
			this.btMLLcap.Name = "btMLLcap";
			this.btMLLcap.Size = new System.Drawing.Size(192, 23);
			this.btMLLcap.TabIndex = 19;
			this.btMLLcap.Text = "Multi-language caption";
			//this.btMLLcap.Click += new System.EventHandler(this.btMLLcap_Click);
			// 
			// btLookup
			// 
			this.btLookup.Location = new System.Drawing.Point(480, 240);
			this.btLookup.Name = "btLookup";
			this.btLookup.Size = new System.Drawing.Size(72, 23);
			this.btLookup.TabIndex = 22;
			this.btLookup.Text = "Lookup";
			this.btLookup.Click += new System.EventHandler(this.btLookup_Click);
			// 
			// txtLookup
			// 
			this.txtLookup.Location = new System.Drawing.Point(480, 264);
			this.txtLookup.Name = "txtLookup";
			this.txtLookup.Size = new System.Drawing.Size(96, 20);
			this.txtLookup.TabIndex = 23;
			this.txtLookup.Text = "";
			this.txtLookup.TextChanged += new System.EventHandler(this.txtLookup_TextChanged);
			// 
			// chkIndexed
			// 
			this.chkIndexed.Location = new System.Drawing.Point(480, 288);
			this.chkIndexed.Name = "chkIndexed";
			this.chkIndexed.TabIndex = 24;
			this.chkIndexed.Text = "Indexed";
			this.chkIndexed.CheckedChanged += new System.EventHandler(this.chkIndexed_CheckedChanged);
			// 
			// btHelpLookup
			// 
			this.btHelpLookup.Location = new System.Drawing.Point(552, 240);
			this.btHelpLookup.Name = "btHelpLookup";
			this.btHelpLookup.Size = new System.Drawing.Size(24, 23);
			this.btHelpLookup.TabIndex = 25;
			this.btHelpLookup.Text = "?";
			this.btHelpLookup.Click += new System.EventHandler(this.btHelpLookup_Click);
			// 
			// lblHelpLookup
			// 
			this.lblHelpLookup.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblHelpLookup.Location = new System.Drawing.Point(64, 160);
			this.lblHelpLookup.Name = "lblHelpLookup";
			this.lblHelpLookup.Size = new System.Drawing.Size(368, 160);
			this.lblHelpLookup.TabIndex = 26;
			this.lblHelpLookup.Tag = "7";
			this.lblHelpLookup.Text = @"Lookup is a query returning a record set with one or more fields. The first field is for displaying in a dropdown list, for example, Customer Name. All fields can be used for updating the values in the selected row, for example, Customer ID. When editing the fields, a drop-down list box will display the first field for selection. When an item is selected the values of the fields will be used to update the field values of the current row, using a field mapping you specified.";
			this.lblHelpLookup.Visible = false;
			this.lblHelpLookup.Click += new System.EventHandler(this.lblHelpLookup_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(480, 312);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 28;
			this.label3.Tag = "";
			this.label3.Text = "Alignment";
			// 
			// cbxAlignment
			// 
			this.cbxAlignment.Location = new System.Drawing.Point(480, 328);
			this.cbxAlignment.Name = "cbxAlignment";
			this.cbxAlignment.Size = new System.Drawing.Size(96, 21);
			this.cbxAlignment.TabIndex = 27;
			this.cbxAlignment.SelectedIndexChanged += new System.EventHandler(this.cbxAlignment_SelectedIndexChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(480, 352);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(100, 16);
			this.label7.TabIndex = 30;
			this.label7.Tag = "";
			this.label7.Text = "Header Alignment";
			// 
			// cbxHdrAlign
			// 
			this.cbxHdrAlign.Location = new System.Drawing.Point(480, 368);
			this.cbxHdrAlign.Name = "cbxHdrAlign";
			this.cbxHdrAlign.Size = new System.Drawing.Size(96, 21);
			this.cbxHdrAlign.TabIndex = 29;
			this.cbxHdrAlign.SelectedIndexChanged += new System.EventHandler(this.cbxHdrAlign_SelectedIndexChanged);
			// 
			// txtFormat
			// 
			this.txtFormat.Location = new System.Drawing.Point(480, 408);
			this.txtFormat.Name = "txtFormat";
			this.txtFormat.Size = new System.Drawing.Size(96, 20);
			this.txtFormat.TabIndex = 32;
			this.txtFormat.Text = "";
			this.txtFormat.TextChanged += new System.EventHandler(this.txtFormat_TextChanged);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(480, 392);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(64, 16);
			this.label8.TabIndex = 31;
			this.label8.Tag = "";
			this.label8.Text = "Format";
			// 
			// btHelpFormat
			// 
			this.btHelpFormat.Location = new System.Drawing.Point(552, 390);
			this.btHelpFormat.Name = "btHelpFormat";
			this.btHelpFormat.Size = new System.Drawing.Size(24, 18);
			this.btHelpFormat.TabIndex = 33;
			this.btHelpFormat.Text = "?";
			this.btHelpFormat.Click += new System.EventHandler(this.btHelpFormat_Click);
			// 
			// dlgPropFields
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(586, 512);
			this.Controls.Add(this.btHelpFormat);
			this.Controls.Add(this.txtFormat);
			this.Controls.Add(this.txtLookup);
			this.Controls.Add(this.txtWidth);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.cbxHdrAlign);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.cbxAlignment);
			this.Controls.Add(this.lblHelpLookup);
			this.Controls.Add(this.btHelpLookup);
			this.Controls.Add(this.chkIndexed);
			this.Controls.Add(this.btLookup);
			this.Controls.Add(this.btMLLcap);
			this.Controls.Add(this.btML);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cbxReadOnly);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.cbVisible);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cbFile);
			this.Controls.Add(this.lblDataType);
			this.Controls.Add(this.cbDataType);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listView1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgPropFields";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Fields attributes";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(EasyQuery qry)
		{
			query = (EasyQuery)qry.Clone();
			fields = query.Fields;
			ListViewItem v;
			for (int i = 0; i < fields.Count; i++)
			{
				v = new ListViewItem(fields[i].Name);
				v.SubItems.Add(EPField.TypeString(fields[i].OleDbType));
				v.SubItems.Add(fields[i].IsIdentity.ToString());
				v.SubItems.Add(fields[i].IsFile.ToString());
				v.SubItems.Add(fields[i].ColumnWidth.ToString());
				v.SubItems.Add(fields[i].Visible.ToString());
				v.SubItems.Add(fields[i].ReadOnly.ToString());
				listView1.Items.Add(v);
			}
			if (listView1.Items.Count > 0)
				listView1.Items[0].Selected = true;
		}
		private void listView1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			txtLookup.Enabled = false;
			if (listView1.SelectedItems.Count > 0)
			{
				item = listView1.SelectedItems[0];
				if (item != null)
				{
					if (fields[item.Index].OleDbType == System.Data.OleDb.OleDbType.Date)
					{
						cbDataType.SelectedIndex = 0;
						lblDataType.Visible = true;
						cbDataType.Visible = true;
					}
					else if (fields[item.Index].OleDbType == System.Data.OleDb.OleDbType.DBDate)
					{
						cbDataType.SelectedIndex = 1;
						lblDataType.Visible = true;
						cbDataType.Visible = true;
					}
					else if (fields[item.Index].OleDbType == System.Data.OleDb.OleDbType.DBTimeStamp)
					{
						cbDataType.SelectedIndex = 2;
						lblDataType.Visible = true;
						cbDataType.Visible = true;
					}
					else if (fields[item.Index].OleDbType == System.Data.OleDb.OleDbType.DBTime)
					{
						cbDataType.SelectedIndex = 3;
						lblDataType.Visible = true;
						cbDataType.Visible = true;
					}
					else
					{
						lblDataType.Visible = false;
						cbDataType.Visible = false;
						if (fields[item.Index].OleDbType != System.Data.OleDb.OleDbType.Boolean &&
							!fields[item.Index].IsFile &&
							!fields[item.Index].IsIdentity &&
							!fields[item.Index].ReadOnly)
						{
							btLookup.Enabled = true;
							if (fields[item.Index].editor != null)
							{
								txtLookup.Text = fields[item.Index].editor.ToString();
							}
							else
							{
								txtLookup.Text = "";
							}
							txtLookup.Enabled = true;
						}
					}
					cbFile.SelectedIndex = (fields[item.Index].IsFile ? 0 : 1);
					cbVisible.SelectedIndex = (fields[item.Index].Visible ? 0 : 1);
					cbxReadOnly.SelectedIndex = (fields[item.Index].ReadOnly ? 0 : 1);
					txtWidth.Text = fields[item.Index].ColumnWidth.ToString();
					txtFormat.Text = fields[item.Index].Format;
					chkIndexed.Checked = fields[item.Index].Indexed;
					btML.Enabled = (EPField.IsBinary(fields[item.Index].OleDbType) || EPField.IsString(fields[item.Index].OleDbType));
					for (int i = 0; i < cbxAlignment.Items.Count; i++)
					{
						if ((System.Windows.Forms.HorizontalAlignment)cbxAlignment.Items[i] == fields[item.Index].TxtAlignment)
						{
							cbxAlignment.SelectedIndex = i;
							break;
						}
					}
					for (int i = 0; i < cbxHdrAlign.Items.Count; i++)
					{
						if ((System.Windows.Forms.HorizontalAlignment)cbxHdrAlign.Items[i] == fields[item.Index].HeaderAlignment)
						{
							cbxHdrAlign.SelectedIndex = i;
							break;
						}
					}
				}
			}
		}

		private void cbDataType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (item != null)
			{
				if (cbDataType.SelectedIndex == 0)
				{
					fields[item.Index].OleDbType = System.Data.OleDb.OleDbType.Date;
					fields[item.Index].DataSize = 16;
					item.SubItems[IDX_DATATYPE].Text = "Date";
				}
				else if (cbDataType.SelectedIndex == 1)
				{
					fields[item.Index].OleDbType = System.Data.OleDb.OleDbType.DBDate;
					item.SubItems[IDX_DATATYPE].Text = "DBDate";
					fields[item.Index].DataSize = 16;
				}
				else if (cbDataType.SelectedIndex == 2)
				{
					fields[item.Index].OleDbType = System.Data.OleDb.OleDbType.DBTimeStamp;
					item.SubItems[IDX_DATATYPE].Text = "Timestamp";
					fields[item.Index].DataSize = 16;
				}
				else if (cbDataType.SelectedIndex == 3)
				{
					fields[item.Index].OleDbType = System.Data.OleDb.OleDbType.DBTime;
					item.SubItems[IDX_DATATYPE].Text = "Time";
					fields[item.Index].DataSize = 16;
				}
			}
		}

		private void cbFile_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (item != null)
			{
				fields[item.Index].IsFile = (cbFile.SelectedIndex == 0);
				item.SubItems[IDX_ISFILE].Text = fields[item.Index].IsFile.ToString();
			}
		}

		private void cbPic_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		}

		private void cbVisible_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (item != null)
			{
				fields[item.Index].Visible = (cbVisible.SelectedIndex == 0);
				item.SubItems[IDX_VISIBLE].Text = fields[item.Index].Visible.ToString();
			}
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			int n = 0, m = 0;
			for (int i = 0; i < fields.Count; i++)
			{
				if (fields[i].IsIdentity)
					n++;
				if (fields[i].OleDbType == System.Data.OleDb.OleDbType.DBTimeStamp)
					m++;
				if (fields[i].IsFile)
				{
					if (fields[i].editor == null)
					{
						fields[i].editor = new DataEditorFile();
					}
				}
			}
			if (n > 1)
			{
				MessageBox.Show(this, Resource1.flds1, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
			}
			else if (m > 1)
			{
				MessageBox.Show(this, Resource1.flds4, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
			}
			else
			{
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}

		private void cbxReadOnly_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (item != null)
			{
				fields[item.Index].ReadOnly = (cbxReadOnly.SelectedIndex == 0);
				item.SubItems[IDX_READONLY].Text = fields[item.Index].ReadOnly.ToString();
			}
		}

		private void txtWidth_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				if (item != null)
				{
					int n = Convert.ToInt32(txtWidth.Text);
					if (n >= 0)
					{
						fields[item.Index].ColumnWidth = n;
						item.SubItems[IDX_WIDTH].Text = n.ToString();
					}
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// setup a database lookup for the field
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btLookup_Click(object sender, System.EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				item = listView1.SelectedItems[0];
				if (item != null)
				{
					//build the query for the database lookup
					EasyQuery qry = null;
					DataEditorLookupDB lk = fields[item.Index].editor as DataEditorLookupDB;
					if (lk == null)
					{
						lk = new DataEditorLookupDB();
					}
					if (lk.Query != null)
					{
						qry = lk.Query;
					}
					if (qry == null)
					{
						qry = new EasyQuery();
					}
					//edit the query
					EasyQuery q = EasyQuery.Edit(qry, this);
					if (q != null)
					{
						lk.Query = q;
						fields[item.Index].editor = lk;
						txtLookup.Text = lk.ToString();
						FieldList srcfields = lk.Query.Fields; //lookup fields provides the source fields
						if (srcfields != null && srcfields.Count > 0)
						{
							if (ownerPerformer == null || srcfields.Count == 1)
							{
								//choose the field name that the value of this source field will map to
								dlgSelectString dlg = new dlgSelectString();
								for (int i = 0; i < fields.Count; i++)
								{
									dlg.LoadData(fields[i].Name);
								}
								dlg.Text = "Select field to update";
								dlg.SetSel(lk.ValueField);
								if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
								{
									lk.ValueField = dlg.sRet;
								}
							}
							else
							{
								if (lk.valuesMaps == null)
								{
									lk.valuesMaps = new DataBind();
								}
								lk.valuesMaps.SourceFields = new string[srcfields.Count];
								for (int i = 0; i < srcfields.Count; i++)
								{
									lk.valuesMaps.SourceFields[i] = srcfields[i].Name;
								}
								dlgPropDataLink dlg2 = new dlgPropDataLink();
								dlg2.LoadData(lk.valuesMaps);
							}
						}
					}
				}
			}
		}

		private void txtLookup_TextChanged(object sender, System.EventArgs e)
		{
			if (txtLookup.Enabled)
			{
				if (txtLookup.Text.Length == 0)
				{
					if (listView1.SelectedItems.Count > 0)
					{
						item = listView1.SelectedItems[0];
						if (item != null)
						{
							fields[item.Index].editor = null;
						}
					}
				}
			}
		}

		private void chkIndexed_CheckedChanged(object sender, System.EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				item = listView1.SelectedItems[0];
				if (item != null)
				{
					fields[item.Index].Indexed = chkIndexed.Checked;
				}
			}
		}

		private void btHelpLookup_Click(object sender, System.EventArgs e)
		{
			lblHelpLookup.Visible = true;
			lblHelpLookup.BringToFront();
		}

		private void lblHelpLookup_Click(object sender, System.EventArgs e)
		{
			lblHelpLookup.Visible = false;
		}

		private void cbxAlignment_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (item != null)
			{
				if (cbxAlignment.SelectedIndex >= 0)
				{
					fields[item.Index].TxtAlignment = (System.Windows.Forms.HorizontalAlignment)cbxAlignment.Items[cbxAlignment.SelectedIndex];
				}
			}
		}

		private void cbxHdrAlign_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (item != null)
			{
				if (cbxHdrAlign.SelectedIndex >= 0)
				{
					fields[item.Index].HeaderAlignment = (System.Windows.Forms.HorizontalAlignment)cbxHdrAlign.Items[cbxHdrAlign.SelectedIndex];
				}
			}

		}

		private void btHelpFormat_Click(object sender, System.EventArgs e)
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo.FileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "textFT.htm");
			p.Start();
		}

		private void txtFormat_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				if (item != null)
				{
					fields[item.Index].Format = txtFormat.Text;
				}
			}
			catch
			{
			}
		}
	}
}
