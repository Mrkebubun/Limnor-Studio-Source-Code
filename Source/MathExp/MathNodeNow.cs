/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp.RaisTypes;
using System.Drawing;
using System.Globalization;
using System.CodeDom;
using System.Collections.Specialized;
using System.ComponentModel;
using XmlUtility;
using System.Xml;
using VPL;

namespace MathExp
{
	[MathNodeCategoryAttribute(enumOperatorCategory.System)]
	[Description("Represents current date and time")]
	[ToolboxBitmapAttribute(typeof(MathNodeNow), "Resources.MathNodeNow2.bmp")]
	public class MathNodeNow : MathNode
	{
		private static RaisDataType _datatype;
		private static RaisDataType _datatypeStr;
		public MathNodeNow(MathNode parent)
			: base(parent)
		{
			IsUTC = false;
			ToIsoString = false;
		}
		private string display
		{
			get
			{
				return "Now";
			}
		}
		public override object CloneExp(MathNode parent)
		{
			MathNodeNow obj = base.CloneExp(parent) as MathNodeNow;
			obj.IsUTC = this.IsUTC;
			obj.ToIsoString = this.ToIsoString;
			return obj;
		}
		public override string ToString()
		{
			return "Now";
		}
		[DefaultValue(false)]
		public bool IsUTC { get; set; }

		[Description("If it is True then the value of this expression is a string formatted as yyyy-mm-dd hh:mm:ss. If it is False then the value is a date time.")]
		[DefaultValue(false)]
		public bool ToIsoString { get; set; }

		public override RaisDataType DataType
		{
			get
			{
				if (ToIsoString)
				{
					if (_datatypeStr == null)
					{
						_datatypeStr = new RaisDataType(typeof(string));
					}
					return _datatypeStr;
				}
				else
				{
					if (_datatype == null)
					{
						_datatype = new RaisDataType(typeof(DateTime));
					}
					return _datatype;
				}
			}
		}

		public override string TraceInfo
		{
			get { return display; }
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}

		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			return g.MeasureString(display, TextFont);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, DrawSize.Width, DrawSize.Height);
				g.DrawString(display, TextFont, TextBrushFocus, (float)0, (float)0);
			}
			else
			{
				g.DrawString(display, TextFont, TextBrush, (float)0, (float)0);
			}
		}
		const string XMLATT_IsUTC = "isutc";
		const string XMLATT_IsSTR = "istext";
		protected override void OnSave(XmlNode node)
		{
			base.OnSave(node);
			XmlUtil.SetAttribute(node, XMLATT_IsUTC, IsUTC);
			XmlUtil.SetAttribute(node, XMLATT_IsSTR, this.ToIsoString);
		}
		protected override void OnLoad(XmlNode node)
		{
			base.OnLoad(node);
			IsUTC = XmlUtil.GetAttributeBoolDefFalse(node, XMLATT_IsUTC);
			ToIsoString = XmlUtil.GetAttributeBoolDefFalse(node, XMLATT_IsSTR);
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			CodeExpression ce;
			if (IsUTC)
			{
				ce = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(DateTime)), "UtcNow");
			}
			else
			{
				ce = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(DateTime)), "Now");
			}
			if (ToIsoString)
			{
				CodeMethodInvokeExpression cmi = new CodeMethodInvokeExpression();
				cmi.Method = new CodeMethodReferenceExpression(ce, "ToString");
				cmi.Parameters.Add(new CodePrimitiveExpression("yyyy-MM-dd hh:mm:ss"));
				cmi.Parameters.Add(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(CultureInfo)), "InvariantCulture"));
				return cmi;
			}
			return ce;
		}

		public override string CreateJavaScript(StringCollection method)
		{
			if (ToIsoString)
			{
				VPLUtil.AddJsFile("dateformat_min.js");
				if (IsUTC)
				{
					return "((new Date((new Date()).getUTCFullYear(),(new Date()).getUTCMonth(),(new Date()).getUTCDate(),(new Date()).getUTCHours(),(new Date()).getUTCMinutes(),(new Date()).getUTCSeconds(),(new Date()).getUTCMilliseconds())).format(\"yyyy-mm-dd HH:MM:ss\"))";
				}
				else
				{
					return "((new Date()).format(\"yyyy-mm-dd HH:MM:ss\"))";
				}
			}
			if (IsUTC)
			{
				return "(new Date((new Date()).getUTCFullYear(),(new Date()).getUTCMonth(),(new Date()).getUTCDate(),(new Date()).getUTCHours(),(new Date()).getUTCMinutes(),(new Date()).getUTCSeconds(),(new Date()).getUTCMilliseconds()))";
			}
			else
			{
				return "(new Date())";
			}
		}

		public override string CreatePhpScript(StringCollection method)
		{
			if (IsUTC)
			{
				return "gmdate(\"Y-m-d H:i:s\")";
			}
			return "date(\"Y-m-d H:i:s\")";
		}

		public override void OnReplaceNode(MathNode replaced)
		{

		}
	}
}
