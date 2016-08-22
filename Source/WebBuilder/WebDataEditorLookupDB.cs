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
using VPL;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Globalization;
using Limnor.WebServerBuilder;
using MathExp;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(WebDataEditorLookupDB), "Resources.qry.bmp")]
	public class WebDataEditorLookupDB : WebDataEditor, IPhpUser
	{
		private Guid _id;
		public WebDataEditorLookupDB()
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
		public StringMapList FieldsMap { get; set; }
		public bool IsValid
		{
			get
			{
				if (ConnectionID == Guid.Empty)
				{
					MathNode.LastValidationError = "web lookup: connection string is empty";
					return false;
				}
				if (string.IsNullOrEmpty(SqlString))
				{
					MathNode.LastValidationError = "web lookup: connection string is empty";
					return false;
				}
				if (FieldsMap == null)
				{
					MathNode.LastValidationError = "web lookup: fields map is null";
					return false;
				}
				if (FieldsMap.StringMaps == null)
				{
					MathNode.LastValidationError = "web lookup: map strings is null";
					return false;
				}
				if (FieldsMap.StringMaps.Length == 0)
				{
					MathNode.LastValidationError = "web lookup: field map strings is empty";
					return false;
				}
				bool hasMap = false;
				for (int i = 0; i < FieldsMap.StringMaps.Length; i++)
				{
					if (!string.IsNullOrEmpty(FieldsMap.StringMaps[i].Target) && !string.IsNullOrEmpty(FieldsMap.StringMaps[i].Source))
					{
						hasMap = true;
						break;
					}
				}
				if (!hasMap)
				{
					MathNode.LastValidationError = "web lookup: field mapping not found";
				}
				return hasMap;
			}
		}
		public void OnGeneratePhpCode(StringCollection pageCode, StringCollection methods, StringCollection requestExecutes)
		{
			if (IsValid)
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
		public override DlgSetEditorAttributes GetDataDialog(DataEditor current)
		{
			if (DataEditor.DelegateGetDatabaseLookupDataDialog != null)
			{
				return DataEditor.DelegateGetDatabaseLookupDataDialog(current, this);
			}
			return null;
		}
		public override string CreateJavascriptEditor(StringCollection code)
		{
			StringBuilder sc = new StringBuilder();
			if (IsValid)
			{
				sc.Append("{Editor:3,\r\n");
				sc.Append("TableName:'");
				sc.Append(TableName);
				sc.Append("',\r\n");
				sc.Append("Map:[\r\n");
				bool first = true;
				for (int i = 0; i < FieldsMap.StringMaps.Length; i++)
				{
					if (!string.IsNullOrEmpty(FieldsMap.StringMaps[i].Target) && !string.IsNullOrEmpty(FieldsMap.StringMaps[i].Source))
					{
						if (first)
						{
							first = false;
						}
						else
						{
							sc.Append(",");
						}
						sc.Append("['");

						sc.Append(FieldsMap.StringMaps[i].Target);
						sc.Append("','");
						sc.Append(FieldsMap.StringMaps[i].Source);
						sc.Append("']");
					}
				}
				sc.Append("\r\n]}");
			}
			else
			{
				sc.Append("{Editor:3}\r\n");
			}
			return sc.ToString();
		}
		protected override void OnGetTypeForXmlSerializarion(List<Type> types)
		{
			if (!types.Contains(typeof(StringMapList)))
				types.Add(typeof(StringMapList));
			if (!types.Contains(typeof(StringMap)))
				types.Add(typeof(StringMap));
		}
		public override string ToString()
		{
			return "Database lookup";
		}
		protected override void OnClone(DataEditor cloned)
		{
			base.OnClone(cloned);
			WebDataEditorLookupDB obj = (WebDataEditorLookupDB)cloned;
			obj.SqlString = SqlString;
			if (FieldsMap != null)
			{
				obj.FieldsMap = (StringMapList)FieldsMap.Clone();
			}
			obj.ConnectionID = ConnectionID;
		}
	}
}
