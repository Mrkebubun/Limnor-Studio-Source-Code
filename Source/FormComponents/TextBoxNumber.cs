/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Components for Windows Form
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using VPL;
using System.Xml.Serialization;
using System.Globalization;
#if DOTNET40
using System.Numerics;
#endif

namespace FormComponents
{
	[ToolboxBitmapAttribute(typeof(TextBoxNumber), "Resources.textbox.bmp")]
	[Description("TextBox for showing/entering numbers")]
	public class TextBoxNumber : TextBox, ISerializeNotify
	{
		#region fields and constructors
		private EnumNumber _typeCode = EnumNumber.Int32;
		private EnumCulture _culture = EnumCulture.InvariantCulture;
		private string _text;
		private bool _restoring;
		private bool _readingProperties;
		private bool _formatting;
		private bool _adjusting;
		private object _numbervalue = 0;
		private string _displayFormat = "";
		public TextBoxNumber()
		{
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public string _RawText
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
				try
				{
					convertNumericVaalue(_text);
				}
				catch
				{
				}
			}
		}
		[DefaultValue(false)]
		[Description("Gets or sets a value indicating whether the data validation is disabled. When the data validation is disabled this component acts as a normal TextBox")]
		public bool DisableValidation
		{
			get;
			set;
		}
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue("")]
		[Description("Gets or sets a value indicating the display format. The value can be C, c, D, d, E, e, F, f, G, g, N, n, P, p, R, r, X, x, or other format string, for example: quantity:#.00. See http://msdn.microsoft.com/en-us/library/kfsatb94.aspx and other Microsoft .Net references for details. If the value is not empty then this text box is read-only.")]
		public string DisplayFormat
		{
			get
			{
				return _displayFormat;
			}
			set
			{
				if (string.CompareOrdinal(_displayFormat, value) != 0)
				{
					_displayFormat = value;
					if (!_readingProperties)
					{
						onChangeDisplayFormat();
					}
				}
			}
		}
		[DefaultValue(EnumCulture.InvariantCulture)]
		[Description("Gets or sets a value indicating the culture used for data validation.")]
		public EnumCulture ValidateCulture
		{
			get
			{
				return _culture;
			}
			set
			{
				_culture = value;
			}
		}
		[DefaultValue(EnumNumber.Int32)]
		[Description("Gets or sets a value indicating the data type allowed for the Text")]
		public EnumNumber TargetType
		{
			get
			{
				return _typeCode;
			}
			set
			{
				if (_typeCode != value)
				{
					_typeCode = value;
					OnTextChanged(EventArgs.Empty);
				}
			}
		}
		[Bindable(true, BindingDirection.TwoWay)]
		[XmlIgnore]
		[Description("The numeric value represented by the Text property")]
		public object NumericValue
		{
			get
			{
				return _numbervalue;
			}
			set
			{
				if (setNumericValue(value))
				{
					formatDisplay();
				}
			}
		}
		public byte ValueByte
		{
			get
			{
				return Convert.ToByte(_numbervalue, CultureInfo.InvariantCulture);
			}
		}
		public Int16 ValueInt16
		{
			get
			{
				return Convert.ToInt16(_numbervalue, CultureInfo.InvariantCulture);
			}
		}
		public Int32 ValueInt32
		{
			get
			{
				return Convert.ToInt32(_numbervalue, CultureInfo.InvariantCulture);
			}
		}
		public Int64 ValueInt64
		{
			get
			{
				return Convert.ToInt64(_numbervalue, CultureInfo.InvariantCulture);
			}
		}
		public UInt16 ValueUInt16
		{
			get
			{
				return Convert.ToUInt16(_numbervalue, CultureInfo.InvariantCulture);
			}
		}
		public UInt32 ValueUInt32
		{
			get
			{
				return Convert.ToUInt32(_numbervalue, CultureInfo.InvariantCulture);
			}
		}
		public UInt64 ValueUInt64
		{
			get
			{
				return Convert.ToUInt64(_numbervalue, CultureInfo.InvariantCulture);
			}
		}
		public SByte ValueSByte
		{
			get
			{
				return Convert.ToSByte(_numbervalue, CultureInfo.InvariantCulture);
			}
		}
		public float ValueFloat
		{
			get
			{
				return Convert.ToSingle(_numbervalue, CultureInfo.InvariantCulture);
			}
		}
		public double ValueDouble
		{
			get
			{
				return Convert.ToDouble(_numbervalue, CultureInfo.InvariantCulture);
			}
		}
#if DOTNET40
		public BigInteger ValueBigInteger
		{
			get
			{
				if (_numbervalue is BigInteger)
				{
					return (BigInteger)_numbervalue;
				}
				double d = Convert.ToDouble(_numbervalue, CultureInfo.InvariantCulture);
				return new BigInteger(d);
			}
		}
#endif
		[Description("Gets a CultureInfo object corresponding to the ValidateCulture value.")]
		public CultureInfo ValidateCultureInfo
		{
			get
			{
				switch (_culture)
				{
					case EnumCulture.InvariantCulture:
						return System.Globalization.CultureInfo.InvariantCulture;
					case EnumCulture.CurrentCulture:
						return System.Globalization.CultureInfo.CurrentCulture;
					case EnumCulture.InstalledUICulture:
						return System.Globalization.CultureInfo.InstalledUICulture;
					case EnumCulture.CurrentUICulture:
						return System.Globalization.CultureInfo.CurrentUICulture;
				}
				return System.Globalization.CultureInfo.InvariantCulture;
			}
		}
		private double _mini = 0;
		[DefaultValue(0)]
		[Description("Gets and sets the minimum value for input validation. The validation is disabled if MinimumValue=MinimumValue.")]
		public double MinimumValue
		{
			get
			{
				return _mini;
			}
			set
			{
				_mini = value;
			}
		}

		private double _maxi = 0;
		[DefaultValue(0)]
		[Description("Gets and sets the maximum value for input validation. The validation is disabled if MinimumValue=MinimumValue.")]
		public double MaximumValue
		{
			get
			{
				return _maxi;
			}
			set
			{
				_maxi = value;
			}
		}
		#endregion
		#region Methods
		protected override void OnTextChanged(EventArgs e)
		{
			if (_readingProperties || _formatting || _adjusting)
			{
				return;
			}
			if (DisableValidation)
			{
				base.OnTextChanged(e);
			}
			else
			{
				if (!_restoring)
				{
					if (string.IsNullOrEmpty(Text))
					{
						_text = "";
						_numbervalue = 0;
						_adjusting = true;
						Text = "0";
						this.SelectionStart = 1;
						_adjusting = false;
						base.OnTextChanged(e);
					}
					else
					{
						try
						{
							convertNumericVaalue(Text);
							base.OnTextChanged(e);
						}
						catch
						{
							_restoring = true;
							int n = this.SelectionStart;
							Text = _text;
							if (!string.IsNullOrEmpty(_text))
							{
								if (n > 0 && n <= _text.Length)
								{
									this.SelectionStart = n - 1;
								}
								else
								{
									this.SelectionStart = _text.Length;
								}
							}
						}
						finally
						{
							_restoring = false;
						}
						_text = Text;
						formatDisplay();
					}
				}
			}
		}
		#endregion
		#region private methods
		private void onChangeDisplayFormat()
		{
			if (!formatDisplay())
			{
				_formatting = true;
				try
				{
					Text = string.Format(ValidateCultureInfo, "{0}", _numbervalue);
				}
				finally
				{
					_formatting = false;
				}
			}
		}
		private bool setNumericValue(object value)
		{
			if (value == null || value == DBNull.Value)
			{
				_numbervalue = 0;
				return true;
			}
			else
			{
				try
				{
					switch (_typeCode)
					{
						case EnumNumber.Byte:
							_numbervalue = Convert.ToByte(value, CultureInfo.InvariantCulture);
							return true;
						case EnumNumber.Double:
							_numbervalue = Convert.ToDouble(value, CultureInfo.InvariantCulture);
							return true;
						case EnumNumber.Int16:
							_numbervalue = Convert.ToInt16(value, CultureInfo.InvariantCulture);
							return true;
						case EnumNumber.Int32:
							_numbervalue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
							return true;
						case EnumNumber.Int64:
							_numbervalue = Convert.ToInt64(value, CultureInfo.InvariantCulture);
							return true;
						case EnumNumber.SByte:
							_numbervalue = Convert.ToSByte(value, CultureInfo.InvariantCulture);
							return true;
						case EnumNumber.Single:
							_numbervalue = Convert.ToSingle(value, CultureInfo.InvariantCulture);
							return true;
						case EnumNumber.UInt16:
							_numbervalue = Convert.ToUInt16(value, CultureInfo.InvariantCulture);
							return true;
						case EnumNumber.UInt32:
							_numbervalue = Convert.ToUInt32(value, CultureInfo.InvariantCulture);
							return true;
						case EnumNumber.UInt64:
							_numbervalue = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
							return true;
#if DOTNET40
							case EnumNumber.BigInteger:
							double d = Convert.ToDouble(_numbervalue, CultureInfo.InvariantCulture);
							_numbervalue = new BigInteger(d);
							return true;
#endif
					}
				}
				catch
				{
				}
			}
			return false;
		}
		private void convertNumericVaalue(string txt)
		{
			switch (_typeCode)
			{
				case EnumNumber.Byte:
					byte by = 0;
					switch (_culture)
					{
						case EnumCulture.InvariantCulture:
							by = Convert.ToByte(txt, System.Globalization.CultureInfo.InvariantCulture);
							break;
						case EnumCulture.CurrentCulture:
							by = Convert.ToByte(txt, System.Globalization.CultureInfo.CurrentCulture);
							break;
						case EnumCulture.InstalledUICulture:
							by = Convert.ToByte(txt, System.Globalization.CultureInfo.InstalledUICulture);
							break;
						case EnumCulture.CurrentUICulture:
							by = Convert.ToByte(txt, System.Globalization.CultureInfo.CurrentUICulture);
							break;
					}
					if (MinimumValue != MaximumValue)
					{
						if (by >= _mini && by <= _maxi)
						{
							_numbervalue = by;
						}
						else
						{
							throw new Exception("data out of range");
						}
					}
					else
					{
						_numbervalue = by;
					}
					break;
				case EnumNumber.Double:
					if (string.CompareOrdinal(txt, "-") != 0)
					{
						double dv = 0;
						switch (_culture)
						{
							case EnumCulture.InvariantCulture:
								dv = Convert.ToDouble(txt, System.Globalization.CultureInfo.InvariantCulture);
								break;
							case EnumCulture.CurrentCulture:
								dv = Convert.ToDouble(txt, System.Globalization.CultureInfo.CurrentCulture);
								break;
							case EnumCulture.InstalledUICulture:
								dv = Convert.ToDouble(txt, System.Globalization.CultureInfo.InstalledUICulture);
								break;
							case EnumCulture.CurrentUICulture:
								dv = Convert.ToDouble(txt, System.Globalization.CultureInfo.CurrentUICulture);
								break;
						}
						if (MinimumValue != MaximumValue)
						{
							if (dv >= _mini && dv <= _maxi)
							{
								_numbervalue = dv;
							}
							else
							{
								throw new Exception("data out of range");
							}
						}
						else
						{
							_numbervalue = dv;
						}
					}
					break;
				case EnumNumber.Int16:
					if (string.CompareOrdinal(txt, "-") != 0)
					{
						Int16 i16 = 0;
						switch (_culture)
						{
							case EnumCulture.InvariantCulture:
								i16 = Convert.ToInt16(txt, System.Globalization.CultureInfo.InvariantCulture);
								break;
							case EnumCulture.CurrentCulture:
								i16 = Convert.ToInt16(txt, System.Globalization.CultureInfo.CurrentCulture);
								break;
							case EnumCulture.InstalledUICulture:
								i16 = Convert.ToInt16(txt, System.Globalization.CultureInfo.InstalledUICulture);
								break;
							case EnumCulture.CurrentUICulture:
								i16 = Convert.ToInt16(txt, System.Globalization.CultureInfo.CurrentUICulture);
								break;
						}
						if (MinimumValue != MaximumValue)
						{
							if (i16 >= _mini && i16 <= _maxi)
							{
								_numbervalue = i16;
							}
							else
							{
								throw new Exception("data out of range");
							}
						}
						else
						{
							_numbervalue = i16;
						}
					}
					break;
				case EnumNumber.Int32:
					if (string.CompareOrdinal(txt, "-") != 0)
					{
						Int32 i32 = 0;
						switch (_culture)
						{
							case EnumCulture.InvariantCulture:
								i32 = Convert.ToInt32(txt, System.Globalization.CultureInfo.InvariantCulture);
								break;
							case EnumCulture.CurrentCulture:
								i32 = Convert.ToInt32(txt, System.Globalization.CultureInfo.CurrentCulture);
								break;
							case EnumCulture.InstalledUICulture:
								i32 = Convert.ToInt32(txt, System.Globalization.CultureInfo.InstalledUICulture);
								break;
							case EnumCulture.CurrentUICulture:
								i32 = Convert.ToInt32(txt, System.Globalization.CultureInfo.CurrentUICulture);
								break;
						}
						if (MinimumValue != MaximumValue)
						{
							if (i32 >= _mini && i32 <= _maxi)
							{
								_numbervalue = i32;
							}
							else
							{
								throw new Exception("data out of range");
							}
						}
						else
						{
							_numbervalue = i32;
						}
					}
					break;
				case EnumNumber.Int64:
					if (string.CompareOrdinal(txt, "-") != 0)
					{
						Int64 i64 = 0;
						switch (_culture)
						{
							case EnumCulture.InvariantCulture:
								i64 = Convert.ToInt64(txt, System.Globalization.CultureInfo.InvariantCulture);
								break;
							case EnumCulture.CurrentCulture:
								i64 = Convert.ToInt64(txt, System.Globalization.CultureInfo.CurrentCulture);
								break;
							case EnumCulture.InstalledUICulture:
								i64 = Convert.ToInt64(txt, System.Globalization.CultureInfo.InstalledUICulture);
								break;
							case EnumCulture.CurrentUICulture:
								i64 = Convert.ToInt64(txt, System.Globalization.CultureInfo.CurrentUICulture);
								break;
						}
						if (MinimumValue != MaximumValue)
						{
							if (i64 >= _mini && i64 <= _maxi)
							{
								_numbervalue = i64;
							}
							else
							{
								throw new Exception("data out of range");
							}
						}
						else
						{
							_numbervalue = i64;
						}
					}
					break;
				case EnumNumber.SByte:
					if (string.CompareOrdinal(txt, "-") != 0)
					{
						sbyte sb = 0;
						switch (_culture)
						{
							case EnumCulture.InvariantCulture:
								sb = Convert.ToSByte(txt, System.Globalization.CultureInfo.InvariantCulture);
								break;
							case EnumCulture.CurrentCulture:
								sb = Convert.ToSByte(txt, System.Globalization.CultureInfo.CurrentCulture);
								break;
							case EnumCulture.InstalledUICulture:
								sb = Convert.ToSByte(txt, System.Globalization.CultureInfo.InstalledUICulture);
								break;
							case EnumCulture.CurrentUICulture:
								sb = Convert.ToSByte(txt, System.Globalization.CultureInfo.CurrentUICulture);
								break;
						}
						if (MinimumValue != MaximumValue)
						{
							if (sb >= _mini && sb <= _maxi)
							{
								_numbervalue = sb;
							}
							else
							{
								throw new Exception("data out of range");
							}
						}
						else
						{
							_numbervalue = sb;
						}
					}
					break;
				case EnumNumber.Single:
					if (string.CompareOrdinal(txt, "-") != 0)
					{
						float f = 0;
						switch (_culture)
						{
							case EnumCulture.InvariantCulture:
								f = Convert.ToSingle(txt, System.Globalization.CultureInfo.InvariantCulture);
								break;
							case EnumCulture.CurrentCulture:
								f = Convert.ToSingle(txt, System.Globalization.CultureInfo.CurrentCulture);
								break;
							case EnumCulture.InstalledUICulture:
								f = Convert.ToSingle(txt, System.Globalization.CultureInfo.InstalledUICulture);
								break;
							case EnumCulture.CurrentUICulture:
								f = Convert.ToSingle(txt, System.Globalization.CultureInfo.CurrentUICulture);
								break;
						}
						if (MinimumValue != MaximumValue)
						{
							if (f >= _mini && f <= _maxi)
							{
								_numbervalue = f;
							}
							else
							{
								throw new Exception("data out of range");
							}
						}
						else
						{
							_numbervalue = f;
						}
					}
					break;
				case EnumNumber.UInt16:
					UInt16 u16 = 0;
					switch (_culture)
					{
						case EnumCulture.InvariantCulture:
							u16 = Convert.ToUInt16(txt, System.Globalization.CultureInfo.InvariantCulture);
							break;
						case EnumCulture.CurrentCulture:
							u16 = Convert.ToUInt16(txt, System.Globalization.CultureInfo.CurrentCulture);
							break;
						case EnumCulture.InstalledUICulture:
							u16 = Convert.ToUInt16(txt, System.Globalization.CultureInfo.InstalledUICulture);
							break;
						case EnumCulture.CurrentUICulture:
							u16 = Convert.ToUInt16(txt, System.Globalization.CultureInfo.CurrentUICulture);
							break;
					}
					if (MinimumValue != MaximumValue)
					{
						if (u16 >= _mini && u16 <= _maxi)
						{
							_numbervalue = u16;
						}
						else
						{
							throw new Exception("data out of range");
						}
					}
					else
					{
						_numbervalue = u16;
					}
					break;
				case EnumNumber.UInt32:
					UInt32 u32 = 0;
					switch (_culture)
					{
						case EnumCulture.InvariantCulture:
							u32 = Convert.ToUInt32(txt, System.Globalization.CultureInfo.InvariantCulture);
							break;
						case EnumCulture.CurrentCulture:
							u32 = Convert.ToUInt32(txt, System.Globalization.CultureInfo.CurrentCulture);
							break;
						case EnumCulture.InstalledUICulture:
							u32 = Convert.ToUInt32(txt, System.Globalization.CultureInfo.InstalledUICulture);
							break;
						case EnumCulture.CurrentUICulture:
							u32 = Convert.ToUInt32(txt, System.Globalization.CultureInfo.CurrentUICulture);
							break;
					}
					if (MinimumValue != MaximumValue)
					{
						if (u32 >= _mini && u32 <= _maxi)
						{
							_numbervalue = u32;
						}
						else
						{
							throw new Exception("data out of range");
						}
					}
					else
					{
						_numbervalue = u32;
					}
					break;
				case EnumNumber.UInt64:
					UInt64 u64 = 0;
					switch (_culture)
					{
						case EnumCulture.InvariantCulture:
							u64 = Convert.ToUInt64(txt, System.Globalization.CultureInfo.InvariantCulture);
							break;
						case EnumCulture.CurrentCulture:
							u64 = Convert.ToUInt64(txt, System.Globalization.CultureInfo.CurrentCulture);
							break;
						case EnumCulture.InstalledUICulture:
							u64 = Convert.ToUInt64(txt, System.Globalization.CultureInfo.InstalledUICulture);
							break;
						case EnumCulture.CurrentUICulture:
							u64 = Convert.ToUInt64(txt, System.Globalization.CultureInfo.CurrentUICulture);
							break;
					}
					if (MinimumValue != MaximumValue)
					{
						if (u64 >= (ulong)_mini && u64 <= (ulong)_maxi)
						{
							_numbervalue = u64;
						}
						else
						{
							throw new Exception("data out of range");
						}
					}
					else
					{
						_numbervalue = u64;
					}
					break;
#if DOTNET40
				case EnumNumber.BigInteger:
					BigInteger bigi;
					bool b = false;
					switch (_culture)
					{
						case EnumCulture.InvariantCulture:
							b = BigInteger.TryParse(txt, NumberStyles.Number, CultureInfo.InvariantCulture, out bigi);
							break;
						case EnumCulture.CurrentCulture:
							b = BigInteger.TryParse(txt, NumberStyles.Number, CultureInfo.CurrentCulture, out bigi);
							break;
						case EnumCulture.InstalledUICulture:
							b = BigInteger.TryParse(txt, NumberStyles.Number, CultureInfo.InstalledUICulture, out bigi);
							break;
						case EnumCulture.CurrentUICulture:
							b = BigInteger.TryParse(txt, NumberStyles.Number, CultureInfo.CurrentUICulture, out bigi);
							break;
						default:
							b = BigInteger.TryParse(txt, out bigi);
							break;
					}
					if (b)
					{
						if (MinimumValue != MaximumValue)
						{
							if (bigi >= new BigInteger(_mini) && bigi <= new BigInteger(_maxi))
							{
								_numbervalue = bigi;
							}
							else
							{
								throw new Exception("data out of range");
							}
						}
						else
						{
							_numbervalue = bigi;
						}
					}
					break;
#endif
			}
		}
		private bool formatDisplay()
		{
			if (!string.IsNullOrEmpty(DisplayFormat))
			{
				_formatting = true;
				int n = this.SelectionStart;
				try
				{
					string s = string.Format(ValidateCultureInfo, "{0:" + DisplayFormat + "}", _numbervalue);
					Text = s;
					if (n < Text.Length)
					{
						this.SelectionStart = n;
					}
					else
					{
						this.SelectionStart = Text.Length;
					}
				}
				finally
				{
					_formatting = false;
				}
				return true;
			}
			return false;
		}
		#endregion
		#region ISerializeNotify Members
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public bool ReadingProperties
		{
			get
			{
				return _readingProperties;
			}
			set
			{
				if (_readingProperties != value)
				{
					_readingProperties = value;
					if (!_readingProperties)
					{
						Text = _text;
					}
				}
			}
		}

		#endregion
	}
}
