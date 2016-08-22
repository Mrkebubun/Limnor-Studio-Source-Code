/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;

namespace VPL
{
	public delegate DlgSetEditorAttributes fnGetDataDialog(DataEditor current, DataEditor caller);
	public delegate void fnNotifyException(Exception e, string message, params object[] values);
	public delegate void fnOnPickValue(object Sender, object Value);
	public interface IFieldList
	{
		int FindFieldIndex(string name);
		int Count { get; }
		string GetFieldname(int i);
	}
	[ToolboxBitmapAttribute(typeof(DataEditor), "Resources.question.bmp")]
	public class DataEditor : ICloneable
	{
		public static fnGetDataDialog DelegateGetDatabaseLookupDataDialog;
		public static fnGetDataDialog DelegateGetCheckedListDialog;
		public event fnOnPickValue OnPickValue = null;
		public static fnNotifyException NotifyException = null;
		public object currentValue = null;
		public string ValueField = ""; //field name to receive the value. empty = associated field
		protected int nUpdatePos = -1;
		//
		private Form form = null;
		private Image _img;
		public DataEditor()
		{
		}
		protected Form OwnerForm
		{
			get
			{
				return form;
			}
		}
		public Image Icon
		{
			get
			{
				if (_img == null)
				{
					_img = VPLUtil.GetTypeIcon(this.GetType());
				}
				return _img;
			}
		}
		protected virtual void OnGetTypeForXmlSerializarion(List<Type> types)
		{
		}
		public virtual void OnLoad()
		{
		}
		/// <summary>
		/// use XML Serialization to convert it to an XML string
		/// </summary>
		/// <returns></returns>
		public string ConvertToString()
		{
			List<Type> types = new List<Type>();
			OnGetTypeForXmlSerializarion(types);
			XmlDocument doc = VPL.XmlSerializerUtility.Save(this, types);
			return doc.OuterXml;
		}
		public static DataEditor ConvertFromString(string xml)
		{
			try
			{
				object obj = XmlSerializerUtility.LoadFromXmlString(xml);
				if (obj != null)
				{
					DataEditor ret = obj as DataEditor;
					if (ret == null)
					{
						if (NotifyException != null)
						{
							NotifyException(null, "Error reading DataEditor. the type is not a DataEditor:{0}, {1}", obj.GetType(), xml);
						}
					}
					return ret;
				}
				return null;
			}
			catch (Exception err)
			{
				if (NotifyException != null)
				{
					NotifyException(err, "Error reading DataEditor:{0}", xml);
				}
			}
			return null;
		}
		public override string ToString()
		{
			return "Choose selector";
		}
		public virtual DlgSetEditorAttributes GetDataDialog(DataEditor current)
		{
			return new DlgSelectFieldEditor(current);
		}
		protected void SetOwner(Form owner)
		{
			form = owner;
		}

		protected void FirePickValue(object Value)
		{
			if (this.OnPickValue != null)
				this.OnPickValue(this, Value);
		}
		/// <summary>
		/// find index on the field list for the field name of this editor
		/// </summary>
		/// <param name="flds"></param>
		public virtual void SetFieldsAttribute(IFieldList flds)
		{
			nUpdatePos = -1;
			if (ValueField != null && flds != null)
			{
				if (ValueField.Length > 0)
				{
					nUpdatePos = flds.FindFieldIndex(ValueField);
				}
			}
		}
		public virtual void UpdateComboBox(ComboBox cbb)
		{
		}
		#region ICloneable Members
		protected virtual void OnClone(DataEditor cloned)
		{
		}
		public object Clone()
		{
			DataEditor obj = (DataEditor)Activator.CreateInstance(this.GetType());
			obj.ValueField = ValueField;
			obj.form = form;
			obj.nUpdatePos = nUpdatePos;
			OnClone(obj);
			return obj;
		}

		#endregion

	}
	/// <summary>
	/// 
	/// </summary>
	[ToolboxBitmapAttribute(typeof(DataEditorNone), "Resources.empty.bmp")]
	public class DataEditorNone : DataEditor
	{
		public DataEditorNone()
		{
		}
		public override string ToString()
		{
			return "None";
		}
	}
	public class DataEditorButton : DataEditor
	{
		public DataEditorButton()
		{
		}
		public virtual Button MakeButton(Form owner)
		{
			SetOwner(owner);
			Button bt = new Button();
			bt.BackColor = System.Drawing.Color.FromArgb(255, 236, 233, 216);
			bt.Text = "...";
			return bt;
		}
	}
}
