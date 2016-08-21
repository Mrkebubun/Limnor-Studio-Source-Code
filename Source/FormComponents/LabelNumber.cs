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
using System.Drawing;
using System.ComponentModel;
using VPL;
using System.Xml.Serialization;
using System.Globalization;

namespace FormComponents
{
	[ToolboxBitmapAttribute(typeof(LabelNumber), "Resources.labelNum.bmp")]
	[Description("Label for showing numbers")]
	public class LabelNumber : Label, ISerializeNotify
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
		public LabelNumber()
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
		[Description("Gets or sets a value indicating the display format. The value can be C, c, D, d, E, e, F, f, G, g, N, n, P, p, R, r, X, x, or other format string, for example: quanty:#.00. See http://msdn.microsoft.com/en-us/library/kfsatb94.aspx and other Microsoft .Net references for details. If the value is not empty then this text box is read-only.")]
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
		[Description("The numeric value represented by the Text property")]
		public object NumericValue
		{
			get
			{
				return _numbervalue;
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
						_adjusting = false;
						base.OnTextChanged(e);
					}
					else
					{
						try
						{
							switch (_typeCode)
							{
								case EnumNumber.Byte:
									switch (_culture)
									{
										case EnumCulture.InvariantCulture:
											_numbervalue = Convert.ToByte(Text, System.Globalization.CultureInfo.InvariantCulture);
											break;
										case EnumCulture.CurrentCulture:
											_numbervalue = Convert.ToByte(Text, System.Globalization.CultureInfo.CurrentCulture);
											break;
										case EnumCulture.InstalledUICulture:
											_numbervalue = Convert.ToByte(Text, System.Globalization.CultureInfo.InstalledUICulture);
											break;
										case EnumCulture.CurrentUICulture:
											_numbervalue = Convert.ToByte(Text, System.Globalization.CultureInfo.CurrentUICulture);
											break;
									}
									break;
								case EnumNumber.Double:
									if (string.CompareOrdinal(Text, "-") != 0)
									{
										switch (_culture)
										{
											case EnumCulture.InvariantCulture:
												_numbervalue = Convert.ToDouble(Text, System.Globalization.CultureInfo.InvariantCulture);
												break;
											case EnumCulture.CurrentCulture:
												_numbervalue = Convert.ToDouble(Text, System.Globalization.CultureInfo.CurrentCulture);
												break;
											case EnumCulture.InstalledUICulture:
												_numbervalue = Convert.ToDouble(Text, System.Globalization.CultureInfo.InstalledUICulture);
												break;
											case EnumCulture.CurrentUICulture:
												_numbervalue = Convert.ToDouble(Text, System.Globalization.CultureInfo.CurrentUICulture);
												break;
										}
									}
									break;
								case EnumNumber.Int16:
									if (string.CompareOrdinal(Text, "-") != 0)
									{
										switch (_culture)
										{
											case EnumCulture.InvariantCulture:
												_numbervalue = Convert.ToInt16(Text, System.Globalization.CultureInfo.InvariantCulture);
												break;
											case EnumCulture.CurrentCulture:
												_numbervalue = Convert.ToInt16(Text, System.Globalization.CultureInfo.CurrentCulture);
												break;
											case EnumCulture.InstalledUICulture:
												_numbervalue = Convert.ToInt16(Text, System.Globalization.CultureInfo.InstalledUICulture);
												break;
											case EnumCulture.CurrentUICulture:
												_numbervalue = Convert.ToInt16(Text, System.Globalization.CultureInfo.CurrentUICulture);
												break;
										}
									}
									break;
								case EnumNumber.Int32:
									if (string.CompareOrdinal(Text, "-") != 0)
									{
										switch (_culture)
										{
											case EnumCulture.InvariantCulture:
												_numbervalue = Convert.ToInt32(Text, System.Globalization.CultureInfo.InvariantCulture);
												break;
											case EnumCulture.CurrentCulture:
												_numbervalue = Convert.ToInt32(Text, System.Globalization.CultureInfo.CurrentCulture);
												break;
											case EnumCulture.InstalledUICulture:
												_numbervalue = Convert.ToInt32(Text, System.Globalization.CultureInfo.InstalledUICulture);
												break;
											case EnumCulture.CurrentUICulture:
												_numbervalue = Convert.ToInt32(Text, System.Globalization.CultureInfo.CurrentUICulture);
												break;
										}
									}
									break;
								case EnumNumber.Int64:
									if (string.CompareOrdinal(Text, "-") != 0)
									{
										switch (_culture)
										{
											case EnumCulture.InvariantCulture:
												_numbervalue = Convert.ToInt64(Text, System.Globalization.CultureInfo.InvariantCulture);
												break;
											case EnumCulture.CurrentCulture:
												_numbervalue = Convert.ToInt64(Text, System.Globalization.CultureInfo.CurrentCulture);
												break;
											case EnumCulture.InstalledUICulture:
												_numbervalue = Convert.ToInt64(Text, System.Globalization.CultureInfo.InstalledUICulture);
												break;
											case EnumCulture.CurrentUICulture:
												_numbervalue = Convert.ToInt64(Text, System.Globalization.CultureInfo.CurrentUICulture);
												break;
										}
									}
									break;
								case EnumNumber.SByte:
									if (string.CompareOrdinal(Text, "-") != 0)
									{
										switch (_culture)
										{
											case EnumCulture.InvariantCulture:
												_numbervalue = Convert.ToSByte(Text, System.Globalization.CultureInfo.InvariantCulture);
												break;
											case EnumCulture.CurrentCulture:
												_numbervalue = Convert.ToSByte(Text, System.Globalization.CultureInfo.CurrentCulture);
												break;
											case EnumCulture.InstalledUICulture:
												_numbervalue = Convert.ToSByte(Text, System.Globalization.CultureInfo.InstalledUICulture);
												break;
											case EnumCulture.CurrentUICulture:
												_numbervalue = Convert.ToSByte(Text, System.Globalization.CultureInfo.CurrentUICulture);
												break;
										}
									}
									break;
								case EnumNumber.Single:
									if (string.CompareOrdinal(Text, "-") != 0)
									{
										switch (_culture)
										{
											case EnumCulture.InvariantCulture:
												_numbervalue = Convert.ToSingle(Text, System.Globalization.CultureInfo.InvariantCulture);
												break;
											case EnumCulture.CurrentCulture:
												_numbervalue = Convert.ToSingle(Text, System.Globalization.CultureInfo.CurrentCulture);
												break;
											case EnumCulture.InstalledUICulture:
												_numbervalue = Convert.ToSingle(Text, System.Globalization.CultureInfo.InstalledUICulture);
												break;
											case EnumCulture.CurrentUICulture:
												_numbervalue = Convert.ToSingle(Text, System.Globalization.CultureInfo.CurrentUICulture);
												break;
										}
									}
									break;
								case EnumNumber.UInt16:
									switch (_culture)
									{
										case EnumCulture.InvariantCulture:
											_numbervalue = Convert.ToUInt16(Text, System.Globalization.CultureInfo.InvariantCulture);
											break;
										case EnumCulture.CurrentCulture:
											_numbervalue = Convert.ToUInt16(Text, System.Globalization.CultureInfo.CurrentCulture);
											break;
										case EnumCulture.InstalledUICulture:
											_numbervalue = Convert.ToUInt16(Text, System.Globalization.CultureInfo.InstalledUICulture);
											break;
										case EnumCulture.CurrentUICulture:
											_numbervalue = Convert.ToUInt16(Text, System.Globalization.CultureInfo.CurrentUICulture);
											break;
									}
									break;
								case EnumNumber.UInt32:
									switch (_culture)
									{
										case EnumCulture.InvariantCulture:
											_numbervalue = Convert.ToUInt32(Text, System.Globalization.CultureInfo.InvariantCulture);
											break;
										case EnumCulture.CurrentCulture:
											_numbervalue = Convert.ToUInt32(Text, System.Globalization.CultureInfo.CurrentCulture);
											break;
										case EnumCulture.InstalledUICulture:
											_numbervalue = Convert.ToUInt32(Text, System.Globalization.CultureInfo.InstalledUICulture);
											break;
										case EnumCulture.CurrentUICulture:
											_numbervalue = Convert.ToUInt32(Text, System.Globalization.CultureInfo.CurrentUICulture);
											break;
									}
									break;
								case EnumNumber.UInt64:
									switch (_culture)
									{
										case EnumCulture.InvariantCulture:
											_numbervalue = Convert.ToUInt64(Text, System.Globalization.CultureInfo.InvariantCulture);
											break;
										case EnumCulture.CurrentCulture:
											_numbervalue = Convert.ToUInt64(Text, System.Globalization.CultureInfo.CurrentCulture);
											break;
										case EnumCulture.InstalledUICulture:
											_numbervalue = Convert.ToUInt64(Text, System.Globalization.CultureInfo.InstalledUICulture);
											break;
										case EnumCulture.CurrentUICulture:
											_numbervalue = Convert.ToUInt64(Text, System.Globalization.CultureInfo.CurrentUICulture);
											break;
									}
									break;
							}
							base.OnTextChanged(e);
						}
						catch
						{
							_restoring = true;
							Text = _text;
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
		private bool formatDisplay()
		{
			if (!string.IsNullOrEmpty(DisplayFormat))
			{
				_formatting = true;
				try
				{
					Text = string.Format(ValidateCultureInfo, "{0:" + DisplayFormat + "}", _numbervalue);
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
