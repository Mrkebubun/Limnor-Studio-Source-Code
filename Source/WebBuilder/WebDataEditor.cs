/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using System.Drawing;
using System.Collections.Specialized;

namespace Limnor.WebBuilder
{
	public class WebDataEditor : DataEditor
	{
		private IWebDataEditorsHolder _holder;
		public WebDataEditor()
		{
		}
		public IWebDataEditorsHolder Hold
		{
			get
			{
				return _holder;
			}
		}
		public string FieldName
		{
			get
			{
				return ValueField;
			}
			set
			{
				ValueField = value;
			}
		}
		public void SetHolder(IWebDataEditorsHolder hold)
		{
			_holder = hold;
		}
		protected override void OnClone(DataEditor cloned)
		{
			base.OnClone(cloned);
			WebDataEditor wd = cloned as WebDataEditor;
			wd.SetHolder(_holder);
		}
		public override DlgSetEditorAttributes GetDataDialog(DataEditor current)
		{
			return new DlgSelectWebFieldEditor(current);
		}
		public virtual string CreateJavascriptEditor(StringCollection code)
		{
			return "null";
		}
	}
	public class DlgSelectWebFieldEditor : DlgSelectFieldEditor
	{
		public DlgSelectWebFieldEditor()
		{
		}
		public DlgSelectWebFieldEditor(DataEditor editor)
			: base(editor)
		{
		}
		protected override Type EditorType
		{
			get
			{
				return typeof(WebDataEditor);
			}
		}
	}
	[ToolboxBitmapAttribute(typeof(DataEditorNone), "Resources.empty.bmp")]
	public class WebDataEditorNone : WebDataEditor
	{
		public WebDataEditorNone()
		{
		}
		public override string ToString()
		{
			return "None";
		}
	}
	[ToolboxBitmapAttribute(typeof(WebDataEditorLookup), "Resources.dropdown.bmp")]
	public class WebDataEditorLookup : WebDataEditor
	{
		#region fields and constructors
		public Dictionary<string, string> values = null;
		private string[] _values;
		private string[] _keys;
		public WebDataEditorLookup()
		{
		}
		#endregion
		#region private methods
		private void makeKeyValues()
		{
			values = new Dictionary<string, string>();
			if (_keys != null && _values != null)
			{
				if (_keys.Length == _values.Length)
				{
					for (int i = 0; i < _keys.Length; i++)
					{
						values.Add(_keys[i], _values[i]);
					}
				}
			}
		}
		#endregion
		#region Properties
		public string[] Names
		{
			get
			{
				if (values == null)
					return new string[] { };
				string[] ss = new string[values.Count];
				Dictionary<string, string>.KeyCollection.Enumerator en = values.Keys.GetEnumerator();
				int i = 0;
				while (en.MoveNext())
				{
					ss[i++] = en.Current;
				}
				return ss;
			}
			set
			{
				_keys = value;
				makeKeyValues();
			}
		}

		public string[] Values
		{
			get
			{
				if (values == null)
					return new string[] { };
				string[] ss = new string[values.Count];
				Dictionary<string, string>.ValueCollection.Enumerator en = values.Values.GetEnumerator();
				int i = 0;
				while (en.MoveNext())
				{
					ss[i++] = en.Current;
				}
				return ss;
			}
			set
			{
				_values = value;
				makeKeyValues();
			}
		}
		#endregion
		#region Methods
		public override DlgSetEditorAttributes GetDataDialog(DataEditor current)
		{
			return new DlgEditNameValueList(current);
		}
		public override string CreateJavascriptEditor(StringCollection code)
		{
			StringBuilder sc = new StringBuilder();
			sc.Append("{Editor:1,\r\n");
			sc.Append("Values:[");
			if (values != null)
			{
				bool first = true;
				foreach (KeyValuePair<string, string> kv in values)
				{
					if (first)
					{
						first = false;
					}
					else
					{
						sc.Append(",\r\n");
					}
					sc.Append("[\"");
					sc.Append(kv.Key);
					sc.Append("\",\"");
					sc.Append(kv.Value);
					sc.Append("\"]");
				}
			}
			sc.Append("]");
			sc.Append("\r\n}");
			return sc.ToString();
		}
		public override string ToString()
		{
			return "Options";
		}
		#endregion
		#region ICloneable Members
		protected override void OnClone(DataEditor cloned)
		{
			base.OnClone(cloned);
			WebDataEditorLookup obj = cloned as WebDataEditorLookup;
			if (values != null)
			{
				obj.values = new Dictionary<string, string>();
				foreach (KeyValuePair<string, string> kv in values)
				{
					obj.values.Add(kv.Key, kv.Value);
				}
			}
		}

		#endregion


	}
	[ToolboxBitmapAttribute(typeof(WebDataEditorLookup), "Resources.calendar.bmp")]
	public class WebDataEditorDatetime : WebDataEditor
	{
		public WebDataEditorDatetime()
		{
		}
		#region Methods
		public override DlgSetEditorAttributes GetDataDialog(DataEditor current)
		{
			return null;
		}
		public override string CreateJavascriptEditor(StringCollection code)
		{
			StringBuilder sc = new StringBuilder();
			sc.Append("{Editor:2\r\n");
			sc.Append("\r\n}");
			return sc.ToString();
		}
		public override string ToString()
		{
			return "Datetime";
		}
		#endregion
	}
}
