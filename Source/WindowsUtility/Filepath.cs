/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace WindowsUtility
{
	public class TypeConverterFilepath : TypeConverter
	{
		public TypeConverterFilepath()
		{
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (typeof(string).Equals(sourceType))
			{
				return true;
			}
			if (context != null)
			{
				if (context.PropertyDescriptor.PropertyType.IsAssignableFrom(sourceType))
					return true;
				TypeConverter converter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);
				return converter.CanConvertFrom(context, sourceType);
			}

			return base.CanConvertFrom(context, sourceType);
		}
		public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
		{
			string s = value as string;
			if (s != null)
			{
				return new Filepath(s);
			}
			if (context != null)
			{
				if (value != null)
				{
					if (context.PropertyDescriptor.PropertyType.IsAssignableFrom(value.GetType()))
						return value;
				}
				TypeConverter converter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);
				return converter.ConvertFrom(context, culture, value);
			}
			return value;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(object).Equals(destinationType))
			{
				return true;
			}
			if (typeof(string).Equals(destinationType))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				if (value == null)
					return string.Empty;
				Filepath f = value as Filepath;
				if (f != null)
				{
					return f.Strings;
				}
				if (context != null)
				{
					TypeConverter converter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);
					if (converter.CanConvertTo(destinationType))
					{
						return (string)converter.ConvertTo(context, CultureInfo.InvariantCulture, value, typeof(string));
					}
				}
				return value.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	[Serializable]
	[TypeConverter(typeof(TypeConverterFilepath))]
	public class Filepath : ICloneable
	{
		public const string FOLD_CPMMPMAPPDATA = "%CommonProgramData%";
		public const string FOLD_APPLICATION = "%Application%";
		public const string FOLD_APPLICATIONDATA = "%ApplicationData%";
		public const string FOLD_SYSTEM = "%System%";
		private string[] _paths;
		private static string[][] _maps;
		private const int _mapCount = 4;
		static Filepath()
		{
			_maps = new string[_mapCount][];
			for (int i = 0; i < _mapCount; i++)
			{
				_maps[i] = new string[2];
			}
			_maps[0][0] = FOLD_CPMMPMAPPDATA; _maps[0][1] = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			_maps[1][0] = FOLD_APPLICATION; _maps[1][1] = AppDomain.CurrentDomain.BaseDirectory;
			_maps[2][0] = FOLD_APPLICATIONDATA; _maps[2][1] = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			_maps[3][0] = FOLD_SYSTEM; _maps[3][1] = Environment.GetFolderPath(Environment.SpecialFolder.System);
		}
		public Filepath()
		{
		}
		public Filepath(string paths)
		{
			if (!string.IsNullOrEmpty(paths))
			{
				_paths = paths.Split(';');
			}
		}
		public override string ToString()
		{
			return FilePath;
		}
		public string Strings
		{
			get
			{
				if (_paths != null && _paths.Length > 0)
				{
					StringBuilder sb = new StringBuilder(_paths[0]);
					for (int i = 1; i < _paths.Length; i++)
					{
						sb.Append(";");
						sb.Append(_paths[i]);
					}
					return sb.ToString();
				}
				return string.Empty;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_paths = null;
				}
				else
				{
					_paths = value.Split(';');
				}
			}
		}
		public string FilePath
		{
			get
			{
				string file = null;
				if (_paths != null && _paths.Length > 0)
				{
					for (int i = 0; i < _paths.Length; i++)
					{
						if (!string.IsNullOrEmpty(_paths[i]))
						{
							bool b = false;
							for (int k = 0; k < _mapCount; k++)
							{
								if (_paths[i].StartsWith(_maps[k][0], StringComparison.OrdinalIgnoreCase))
								{
									string s = _paths[i].Substring(_maps[k][0].Length);
									while (s.StartsWith("\\"))
									{
										s = s.Substring(1);
									}
									s = Path.Combine(_maps[k][1], s);
									if (File.Exists(s))
									{
										return s;
									}
									file = s;
									b = true;
									break;
								}
							}
							if (!b)
							{
								if (File.Exists(_paths[i]))
								{
									return _paths[i];
								}
							}
						}
					}
					if (file == null)
					{
						for (int i = 0; i < _paths.Length; i++)
						{
							if (!string.IsNullOrEmpty(_paths[i]))
							{
								file = _paths[i];
								break;
							}
						}
					}
				}
				if (!string.IsNullOrEmpty(file))
				{
					return file;
				}
				return string.Empty;
			}
		}

		#region ICloneable Members

		public object Clone()
		{
			Filepath f = new Filepath();
			if (_paths != null)
			{
				f._paths = new string[_paths.Length];
				for (int i = 0; i < _paths.Length; i++)
				{
					f._paths[i] = _paths[i];
				}
			}
			return f;
		}

		#endregion
	}
}
