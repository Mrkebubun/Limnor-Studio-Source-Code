/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using MathExp.RaisTypes;
using System.ComponentModel;
using VPL;

namespace MathExp
{
	[MathNodeCategory(enumOperatorCategory.Internal)]
	public class MathNodeDefaultValue : MathNodeVariable
	{
		string dispaly = "default";
		public MathNodeDefaultValue(MathNode parent)
			: base(parent)
		{
		}

		public override string ToString()
		{
			Type t = this.DataType.Type;
			object v = VPLUtil.GetDefaultValue(t);
			return v.ToString();
		}
		protected override void OnInPortCreated()
		{
			((DrawingVariable)(InPort.Label)).LabelVisible = false;
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}

		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			dispaly = ToString();
			return g.MeasureString(dispaly, TextFont);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			dispaly = ToString();
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, DrawSize.Width, DrawSize.Height);
				g.DrawString(dispaly, TextFont, TextBrushFocus, (float)0, (float)0);
			}
			else
			{
				g.DrawString(dispaly, TextFont, TextBrush, (float)0, (float)0);
			}
		}
		/// <summary>
		/// true: a variable will be generated; false: no variable will be generated
		/// </summary>
		[Browsable(false)]
		public override bool IsCodeVariable
		{
			get { return false; }
		}
		/// <summary>
		/// find linked node and use that node's code.
		/// if not found then use default value
		/// </summary>
		/// <returns></returns>
		public override System.CodeDom.CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			if (this.InPort != null)
			{
				if (this.InPort.LinkedPortID != 0)
				{
					MathExpItem mathParent = root.ContainerMathItem;
					if (mathParent == null)
					{
						throw new MathException(XmlSerialization.FormatString("ExportCode for Default value: Linked port ID {0}. MathNodeRoot missing container.", this.InPort.LinkedPortID));
					}
					if (mathParent.Parent == null)
					{
						throw new MathException(XmlSerialization.FormatString("ExportCode for Default value: Linked port ID {0}. MathExpItem missing container.", this.InPort.LinkedPortID));
					}
					MathExpItem item = mathParent.Parent.GetItemByID(this.InPort.LinkedPortID);
					if (item == null)
					{
						throw new MathException(XmlSerialization.FormatString("ExportCode for Default value: Linked port ID {0} does not point to a valid port", this.InPort.LinkedPortID));
					}
					return item.ReturnCodeExpression(method);
				}
			}
			return ValueTypeUtil.GetDefaultValueByType(this.DataType.Type);
		}
	}
}
