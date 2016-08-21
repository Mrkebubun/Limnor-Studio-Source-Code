/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	File Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace LFilePath
{
	public class OverrideReadOnlyAttribute : Attribute
	{
		public OverrideReadOnlyAttribute()
		{
		}
	}
	public class FilePathAttribute : Attribute
	{
		private string _pattern = "*.*";
		private string _title = "Select a file";
		private bool _saveFile;
		public FilePathAttribute()
		{
		}
		public FilePathAttribute(string pattern)
		{
			_pattern = pattern;
		}
		public FilePathAttribute(string pattern, string title)
			: this(pattern)
		{
			_title = title;
		}
		public FilePathAttribute(string pattern, string title, bool saveFile)
			: this(pattern, title)
		{
			_saveFile = saveFile;
		}
		public string Pattern
		{
			get
			{
				return _pattern;
			}
		}
		public string Title
		{
			get
			{
				return _title;
			}
		}
		public bool SaveFile
		{
			get
			{
				return _saveFile;
			}
		}
		public static bool IsFilePath(PropertyDescriptor p)
		{
			if (p.Attributes != null && p.Attributes.Count > 0)
			{

				for (int i = 0; i < p.Attributes.Count; i++)
				{
					if (p.Attributes[i] is FilePathAttribute)
					{
						return true;
					}
					EditorAttribute ea = p.Attributes[i] as EditorAttribute;
					if (ea != null)
					{
						if (ea.EditorTypeName != null && ea.EditorTypeName.Contains("LFilePath.PropEditorFilePath"))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
	public class PropEditorFilePath : UITypeEditor
	{
		public PropEditorFilePath()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				FilePathAttribute fa = null;
				if (context.PropertyDescriptor.Attributes != null)
				{
					foreach (Attribute a in context.PropertyDescriptor.Attributes)
					{
						fa = a as FilePathAttribute;
						if (fa != null)
						{
							break;
						}
					}
				}
				if (fa != null && fa.SaveFile)
				{
					SaveFileDialog dlg = new SaveFileDialog();
					if (value != null)
					{
						dlg.FileName = value.ToString();
					}
					dlg.Title = fa.Title;
					dlg.Filter = fa.Pattern;
					if (dlg.ShowDialog() == DialogResult.OK)
					{
						value = dlg.FileName;
						if (context.PropertyDescriptor.IsReadOnly)
						{
							if (context.PropertyDescriptor.Attributes.Contains(new OverrideReadOnlyAttribute()))
							{
								context.PropertyDescriptor.SetValue(context.Instance, dlg.FileName);
							}
						}
					}
				}
				else
				{
					OpenFileDialog dlg = new OpenFileDialog();
					if (value != null)
					{
						dlg.FileName = value.ToString();
					}
					if (fa != null)
					{
						dlg.Title = fa.Title;
						dlg.Filter = fa.Pattern;
					}
					if (dlg.ShowDialog() == DialogResult.OK)
					{
						value = dlg.FileName;
						if (context.PropertyDescriptor.IsReadOnly)
						{
							if (context.PropertyDescriptor.Attributes.Contains(new OverrideReadOnlyAttribute()))
							{
								context.PropertyDescriptor.SetValue(context.Instance, dlg.FileName);
							}
						}
					}
				}
			}
			return value;
		}
	}
}
