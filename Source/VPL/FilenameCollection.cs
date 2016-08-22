/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace VPL
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FilenameCollection : ICustomTypeDescriptor
	{
		#region fields and constructors
		private StringCollection _names;
		public FilenameCollection()
		{
		}
		#endregion
		#region Properties
		public string FilePatern { get; set; }
		public string FileDialogTitle { get; set; }
		public string this[int idx]
		{
			get
			{
				return _names[idx];
			}
			set
			{
				_names[idx] = value;
			}
		}
		public int Count
		{
			get
			{
				if (_names == null)
				{
					return 0;
				}
				return _names.Count;
			}
		}
		public StringCollection Filenames { get { return _names; } }
		#endregion
		#region Methods
		public void SetNames(string[] names)
		{
			_names = new StringCollection();
			if (names != null && names.Length > 0)
			{
				for (int i = 0; i < names.Length; i++)
				{
					_names.Add(names[i]);
				}
			}
		}
		public void SetNames(StringCollection names)
		{
			_names = names;
		}
		public override string ToString()
		{
			int n = 0;
			for (int i = 0; i < Count; i++)
			{
				if (!string.IsNullOrEmpty(this[i]))
				{
					n++;
				}
			}
			return string.Format(CultureInfo.InvariantCulture, "Count:{0}", n);
		}
		#endregion
		#region class FileNameEditorWithFilter
		class FileNameEditorWithFilter : FileNameEditor
		{
			public static string Title;
			public static string Filter;
			protected override void InitializeDialog(System.Windows.Forms.OpenFileDialog openFileDialog)
			{
				base.InitializeDialog(openFileDialog);
				if (!string.IsNullOrEmpty(Title))
				{
					openFileDialog.Title = Title;
				}
				if (!string.IsNullOrEmpty(Filter))
				{
					openFileDialog.Filter = Filter;
				}
			}
		}
		#endregion
		#region PropertyDescriptorFilename
		class PropertyDescriptorFilename : PropertyDescriptor
		{
			private int idx;
			public PropertyDescriptorFilename(int i)
				: base(string.Format(CultureInfo.InvariantCulture, "File{0}", i),
				new Attribute[] { new EditorAttribute(typeof(FileNameEditorWithFilter), typeof(UITypeEditor)),
                new RefreshPropertiesAttribute(RefreshProperties.All)})
			{
				idx = i;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(FilenameCollection); }
			}

			public override object GetValue(object component)
			{
				FilenameCollection f = (FilenameCollection)component;
				return f[idx];
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
				FilenameCollection f = (FilenameCollection)component;
				f[idx] = string.Empty;
			}

			public override void SetValue(object component, object value)
			{
				FilenameCollection f = (FilenameCollection)component;
				f[idx] = value as string;
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion
		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public System.ComponentModel.AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		[Browsable(false)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<PropertyDescriptor> l = new List<PropertyDescriptor>();
			if (_names == null)
			{
				_names = new StringCollection();
				_names.Add(string.Empty);
			}
			else
			{
				bool hasEmpty = false;
				for (int i = 0; i < _names.Count; i++)
				{
					if (string.IsNullOrEmpty(_names[i]))
					{
						hasEmpty = true;
						break;
					}
				}
				if (!hasEmpty)
				{
					_names.Add(string.Empty);
				}
			}
			FileNameEditorWithFilter.Title = this.FileDialogTitle;
			FileNameEditorWithFilter.Filter = this.FilePatern;
			for (int i = 0; i < _names.Count; i++)
			{
				PropertyDescriptorFilename p = new PropertyDescriptorFilename(i);
				l.Add(p);
			}

			return new PropertyDescriptorCollection(l.ToArray());
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
	}
}
