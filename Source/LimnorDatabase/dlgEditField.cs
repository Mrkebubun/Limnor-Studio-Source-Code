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
using System.Text;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for dlgEditField.
	/// </summary>
	public class dlgEditField : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblTable;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label3;
		private DbTypeComboBox cbxType;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox txtSize;
		//
		public EPField field = null;
		bool bNew = true;
		Connection _connection;
		//
		public dlgEditField()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
			this.lblTable = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.txtSize = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.cbxType = new DbTypeComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 24);
			this.label1.Name = "label1";
			this.label1.TabIndex = 0;
			this.label1.Tag = "1";
			this.label1.Text = "Table name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblTable
			// 
			this.lblTable.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblTable.Location = new System.Drawing.Point(136, 24);
			this.lblTable.Name = "lblTable";
			this.lblTable.Size = new System.Drawing.Size(128, 24);
			this.lblTable.TabIndex = 1;
			this.lblTable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 72);
			this.label2.Name = "label2";
			this.label2.TabIndex = 2;
			this.label2.Tag = "2";
			this.label2.Text = "Field name:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(136, 72);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(128, 20);
			this.txtName.TabIndex = 0;
			this.txtName.Text = "";
			// 
			// txtSize
			// 
			this.txtSize.Location = new System.Drawing.Point(136, 136);
			this.txtSize.Name = "txtSize";
			this.txtSize.Size = new System.Drawing.Size(128, 20);
			this.txtSize.TabIndex = 2;
			this.txtSize.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 104);
			this.label3.Name = "label3";
			this.label3.TabIndex = 4;
			this.label3.Tag = "3";
			this.label3.Text = "Field type:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cbxType
			// 
			//this.cbxType.Items.AddRange(new object[] {
			//                                             "String", //FLD_String
			//                                             "Integer", //FLD_Integer
			//                                             "Long integer", //FLD_Long_integer
			//                                             "Decimal", //FLD_Decimal
			//                                             "Currency", //FLD_Currency
			//                                             "Date", //FLD_Date
			//                                             "Date and time", //FLD_Date_time
			//                                             "Yes/No", //FLD_Bool
			//                                             "Large text", //FLD_Text
			//                                             "Large binary" //FLD_Binary

			//});
			this.cbxType.Location = new System.Drawing.Point(136, 104);
			this.cbxType.Name = "cbxType";
			this.cbxType.Size = new System.Drawing.Size(128, 21);
			this.cbxType.TabIndex = 1;
			this.cbxType.SelectedIndexChanged += new System.EventHandler(this.cbxType_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(24, 136);
			this.label4.Name = "label4";
			this.label4.TabIndex = 7;
			this.label4.Tag = "4";
			this.label4.Text = "Field size:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(192, 176);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 4;
			this.btCancel.Text = "Cancel";
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(104, 176);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 3;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// dlgEditField
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 224);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cbxType);
			this.Controls.Add(this.txtSize);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblTable);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgEditField";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Field";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(string table, EPField fld, Connection cn)
		{
			_connection = cn;
			lblTable.Text = table;
			if (fld == null)
			{
				bNew = true;
				field = new EPField();
			}
			else
			{
				bNew = false;
				field = fld;
				txtName.ReadOnly = true;
			}
			txtName.Text = field.Name;
			cbxType.SetSelectionByOleDbType(field.OleDbType);

			txtSize.Text = field.DataSize.ToString();
		}
		private void cbxType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			txtSize.ReadOnly = !cbxType.CurrentSelectAllowChangeSize();
			int n = cbxType.CurrentSelectDataSize();
			if (n == 0)
			{
				txtSize.Text = "";
			}
			else
			{
				txtSize.Text = n.ToString();
			}
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			string newName = txtName.Text.Trim();
			newName = System.Text.RegularExpressions.Regex.Replace(newName, "~[a-zA-Z~_]", "");
			if (newName.Length == 0)
			{
				MessageBox.Show(this, Resource1.fieldNameMissing, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
			}
			else
			{
				try
				{
					string sep1 = _connection.NameDelimiterBegin;
					string sep2 = _connection.NameDelimiterEnd;
					StringBuilder sSQL = new StringBuilder("ALTER TABLE ");
					sSQL.Append(sep1);
					sSQL.Append(lblTable.Text);
					sSQL.Append(sep2);
					if (bNew)
					{
						sSQL.Append(" ADD ");
						sSQL.Append(sep1);
						sSQL.Append(newName);
						sSQL.Append(sep2);
						sSQL.Append(" ");
					}
					else
					{
						if (_connection.IsOdbc)
						{
							if (field.Name != newName)
							{
								sSQL.Append(" CHANGE COLUMN ");
								sSQL.Append(sep1);
								sSQL.Append(field.Name);
								sSQL.Append(sep2);
								sSQL.Append(" ");
								sSQL.Append(sep1);
								sSQL.Append(newName);
								sSQL.Append(sep2);
								sSQL.Append(" ");
							}
							else
							{
								sSQL.Append(" MODIFY COLUMN ");
								sSQL.Append(sep1);
								sSQL.Append(field.Name);
								sSQL.Append(sep2);
								sSQL.Append(" ");
							}
						}
						else
						{
							sSQL.Append(" ALTER COLUMN ");
							sSQL.Append(sep1);
							sSQL.Append(field.Name);
							sSQL.Append(sep2);
							sSQL.Append(" ");
						}
					}

					switch (cbxType.SelectedIndex)
					{
						case DbTypeComboBox.FLD_String:
							try
							{
								field.DataSize = Convert.ToInt32(txtSize.Text);
								txtSize.Text = field.DataSize.ToString();
							}
							catch
							{
							}
							if (field.DataSize <= 0)
								field.DataSize = 255;
							field.OleDbType = System.Data.OleDb.OleDbType.VarWChar;
							if (_connection.IsOdbc)
							{
								sSQL.Append(" VARCHAR(");
								sSQL.Append(field.DataSize.ToString());
								sSQL.Append(") CHARACTER SET utf8 COLLATE utf8_bin NULL;");
							}
							else
							{
								sSQL.Append(" NVARCHAR(");
								sSQL.Append(field.DataSize.ToString());
								sSQL.Append(") NULL ");
							}
							break;
						case DbTypeComboBox.FLD_Integer:
							field.OleDbType = System.Data.OleDb.OleDbType.Integer;
							field.DataSize = 4;
							sSQL.Append(" INT NULL ");
							break;
						case DbTypeComboBox.FLD_Long_integer:
							field.OleDbType = System.Data.OleDb.OleDbType.BigInt;
							field.DataSize = 8;
							if (_connection.IsOdbc)
							{
								sSQL.Append(" BIGINT NULL; ");
							}
							else
							{
								sSQL.Append(" LONG NULL ");
							}
							break;
						case DbTypeComboBox.FLD_Decimal:
							field.OleDbType = System.Data.OleDb.OleDbType.Double;
							field.DataSize = 8;
							sSQL.Append(" DOUBLE NULL ");
							break;
						case DbTypeComboBox.FLD_Currency:
							field.OleDbType = System.Data.OleDb.OleDbType.Currency;
							field.DataSize = 8;
							if (_connection.IsOdbc)
							{
								sSQL.Append(" DECIMAL NULL; ");
							}
							else
							{
								sSQL.Append(" CURRENCY NULL ");
							}
							break;
						case DbTypeComboBox.FLD_Date:
							field.OleDbType = System.Data.OleDb.OleDbType.Date;
							field.DataSize = 8;
							sSQL.Append(" DATETIME NULL ");
							break;
						case DbTypeComboBox.FLD_Date_time:
							field.OleDbType = System.Data.OleDb.OleDbType.DBTimeStamp;
							field.DataSize = 8;
							sSQL.Append(" DATETIME NULL ");
							break;
						case DbTypeComboBox.FLD_Bool:
							field.OleDbType = System.Data.OleDb.OleDbType.Boolean;
							field.DataSize = 1;
							if (_connection.IsOdbc)
							{
								sSQL.Append(" BOOLEAN NULL; ");
							}
							else
							{
								if (_connection.IsMySql)
								{
									sSQL.Append(" BIT(1) NULL ");
								}
								else
								{
									sSQL.Append(" YESNO NULL ");
								}
							}
							break;
						case DbTypeComboBox.FLD_Text:
							field.OleDbType = System.Data.OleDb.OleDbType.LongVarWChar;
							field.DataSize = -1;
							sSQL.Append(" TEXT NULL ");
							break;
						case DbTypeComboBox.FLD_Binary:
							field.OleDbType = System.Data.OleDb.OleDbType.LongVarBinary;
							field.DataSize = -1;
							if (_connection.IsOdbc)
							{
								sSQL.Append(" SQL_LONGVARBINARY NULL; ");
							}
							else
							{
								sSQL.Append(" BIGBINARY ");
							}
							break;
					}
					_connection.ExecuteNonQuery(sSQL.ToString());
					//
					field.Name = newName;
					this.DialogResult = System.Windows.Forms.DialogResult.OK;
				}
				catch (Exception er)
				{
					MessageBox.Show(this, er.Message, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
				}
			}
		}
	}
}
