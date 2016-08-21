/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Globalization;
using LFilePath;
using System.Drawing.Design;

namespace LimnorDesigner
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FilePathList : ICustomTypeDescriptor
	{
		private StringCollection _files;
		public FilePathList()
		{
			_files = new StringCollection();
		}
		protected virtual bool UseAddnew
		{
			get
			{
				return true;
			}
		}
		public int Count
		{
			get
			{
				return _files.Count;
			}
		}
		public string this[int index]
		{
			get
			{
				if (index >= 0 && index < _files.Count)
					return _files[index];
				return string.Empty;
			}
			set
			{
				if (index >= 0 && index < _files.Count)
				{
					_files[index] = value;
					OnFileListChanged();
				}
			}
		}
		public void AddFile(string file)
		{
			if (!string.IsNullOrEmpty(file))
			{
				foreach (string s in _files)
				{
					if (string.Compare(s, file, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return;
					}
				}
				_files.Add(file);
				OnFileListChanged();
			}
		}
		public void DeleteFile(int index)
		{
			if (index >= 0 && index < _files.Count)
			{
				_files.RemoveAt(index);
				OnFileListChanged();
			}
		}
		protected virtual void OnFileListChanged()
		{
		}
		public override string ToString()
		{
			return "File list";
		}
		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			if (UseAddnew)
			{
				list.Add(new PropertyDescriptorNewFile());
			}
			for (int i = 0; i < _files.Count; i++)
			{
				list.Add(new PropertyDescriptorFile(i));
			}
			return new PropertyDescriptorCollection(list.ToArray());
		}
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
		#region class PropertyDescriptorNewFile
		class PropertyDescriptorNewFile : PropertyDescriptor
		{
			public PropertyDescriptorNewFile()
				: base("New File", new Attribute[]{
                    new RefreshPropertiesAttribute(RefreshProperties.All),
                    new ParenthesizePropertyNameAttribute(true),
                    new OverrideReadOnlyAttribute(),
                    new DescriptionAttribute("Select a new file into the list"),
                    new EditorAttribute(typeof(PropEditorFilePath),typeof(UITypeEditor))
            })
			{
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(FilePathList); }
			}

			public override object GetValue(object component)
			{
				return "Select a new file";
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get { return typeof(string); }
			}

			public override void ResetValue(object component)
			{

			}

			public override void SetValue(object component, object value)
			{
				FilePathList fl = (FilePathList)component;
				fl.AddFile(value as string);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion
		#region class PropertyDescriptorFile
		class PropertyDescriptorFile : PropertyDescriptor
		{
			private int _index;
			public PropertyDescriptorFile(int idx)
				: base(string.Format(CultureInfo.InvariantCulture, "File{0}", idx + 1),
					new Attribute[] {
                    new RefreshPropertiesAttribute(RefreshProperties.All),
                    new EditorAttribute(typeof(PropEditorFilePath),typeof(UITypeEditor))
                    })
			{
				_index = idx;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(FilePathList); }
			}

			public override object GetValue(object component)
			{
				FilePathList fl = (FilePathList)component;
				return fl[_index];
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return typeof(string); }
			}

			public override void ResetValue(object component)
			{

			}

			public override void SetValue(object component, object value)
			{
				FilePathList fl = (FilePathList)component;
				fl[_index] = value as string;
				if (string.IsNullOrEmpty(fl[_index]))
				{
					fl.DeleteFile(_index);
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}
		}
		#endregion
	}
}
