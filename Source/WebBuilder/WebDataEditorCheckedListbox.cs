/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.ComponentModel;
using VPL;
using Limnor.WebServerBuilder;
using System.Collections.Specialized;
using MathExp;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(WebDataEditorCheckedListbox), "Resources.chklist.bmp")]
	public class WebDataEditorCheckedListbox : WebDataEditor, IDataQueryUser, IPhpUser
	{
		private Guid _id;
		public WebDataEditorCheckedListbox()
		{
		}
		public Guid ID
		{
			get
			{
				if (_id == Guid.Empty)
				{
					_id = Guid.NewGuid();
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		public string TableName
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "t_{0}", ID.ToString("N", CultureInfo.InvariantCulture));
			}
		}
		public Guid ConnectionID { get; set; }
		public string SqlString { get; set; }

		[Description("Gets and sets a Boolean indicating wether the list box should be filled with data from a database.")]
		public bool UseDataFromDatabase { get; set; }

		public string[] ListBoxItems { get; set; }

		public bool IsValid
		{
			get
			{
				if (UseDataFromDatabase)
				{
					if (ConnectionID == Guid.Empty)
					{
						MathNode.LastValidationError = "web checked list: connection ID is empty";
						return false;
					}
					if (string.IsNullOrEmpty(SqlString))
					{
						MathNode.LastValidationError = "web checked list: connection string is empty";
						return false;
					}
				}
				else
				{
					if (ListBoxItems == null)
					{
						MathNode.LastValidationError = "web checked list: items is null";
						return false;
					}
					if (ListBoxItems.Length == 0)
					{
						MathNode.LastValidationError = "web checked list: items count is 0";
						return false;
					}
				}
				return true;
			}
		}
		public string GetListItems()
		{
			StringBuilder sb = new StringBuilder();
			if (ListBoxItems != null)
			{
				for (int i = 0; i < ListBoxItems.Length; i++)
				{
					if (i > 0)
					{
						sb.Append("\r\n");
					}
					sb.Append(ListBoxItems[i]);
				}
			}
			return sb.ToString();
		}
		public void SetListItems(string[] lines)
		{
			if (lines == null)
			{
				ListBoxItems = new string[] { };
			}
			else
			{
				ListBoxItems = lines;
			}
		}
		public void OnGeneratePhpCode(StringCollection pageCode, StringCollection methods, StringCollection requestExecutes)
		{
			if (IsValid && UseDataFromDatabase)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("function ");
				sb.Append(TableName);
				sb.Append("()\r\n");
				sb.Append("{\r\n");
				sb.Append("  $sql = '");
				sb.Append(SqlString.Replace("'", "\\'"));
				sb.Append("';\r\n");
				sb.Append("  $tbl = $this->AddDataTable('");
				sb.Append(TableName);
				sb.Append("');\r\n");
				sb.Append("  $ps = array();\r\n");
				sb.Append("  $msql = new JsonSourceMySql();\r\n");
				sb.Append("  $msql->SetCredential($this->");
				sb.Append(ServerCodeutility.GetPhpMySqlConnectionName(this.ConnectionID));
				sb.Append(");\r\n");
				sb.Append("  $msql->SetDebug($this->DEBUG);\r\n");
				sb.Append("  $msql->GetData($tbl,$sql,$ps);\r\n");
				//
				sb.Append("}\r\n");
				//
				methods.Add(sb.ToString());
				//
				sb = new StringBuilder();
				sb.Append("if($method == '");
				sb.Append(TableName);
				sb.Append("') $this->");
				sb.Append(TableName);
				sb.Append("();\r\n");
				requestExecutes.Add(sb.ToString());
			}
		}
		public override string CreateJavascriptEditor(StringCollection code)
		{
			StringBuilder sc = new StringBuilder();
			if (IsValid)
			{
				sc.Append("{Editor:4,\r\n");
				sc.Append("UseDb:");
				if (UseDataFromDatabase)
				{
					sc.Append("true");
				}
				else
				{
					sc.Append("false");
				}
				sc.Append(",\r\n");
				if (UseDataFromDatabase)
				{
				}
				else
				{
					sc.Append("List:[");
					for (int i = 0; i < ListBoxItems.Length; i++)
					{
						if (i > 0)
						{
							sc.Append(",");
						}
						sc.Append("\r\n\"");
						sc.Append(ListBoxItems[i]);
						sc.Append("\"");
					}
					sc.Append("],\r\n");
				}
				sc.Append("TableName:'");
				sc.Append(TableName);
				sc.Append("'");

				sc.Append("\r\n}");
			}
			else
			{
				sc.Append("{Editor:4}\r\n");
			}
			return sc.ToString();
		}
		public override DlgSetEditorAttributes GetDataDialog(DataEditor current)
		{
			if (DataEditor.DelegateGetCheckedListDialog != null)
			{
				return DataEditor.DelegateGetCheckedListDialog(current, this);
			}
			return null;
		}
		public override string ToString()
		{
			return "Checked Listbox";
		}
		protected override void OnClone(DataEditor cloned)
		{
			base.OnClone(cloned);
			WebDataEditorCheckedListbox obj = (WebDataEditorCheckedListbox)cloned;
			obj.SqlString = SqlString;
			obj.ConnectionID = ConnectionID;
			obj.UseDataFromDatabase = UseDataFromDatabase;
			obj.ID = ID;
			if (ListBoxItems != null)
			{
				obj.ListBoxItems = new string[ListBoxItems.Length];
				for (int i = 0; i < ListBoxItems.Length; i++)
				{
					obj.ListBoxItems[i] = ListBoxItems[i];
				}
			}
		}
	}
}
