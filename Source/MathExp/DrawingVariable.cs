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
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using VPL;

namespace MathExp
{
	public class DrawingVariable : RelativeDrawing
	{
		IVariable var;
		private bool _labelVisible = true;
		MathNodeRoot root;//holder for drawing the var
		public DrawingVariable()
		{
			root = new MathNodeRoot();
			var = (IVariable)root[0];
		}
		public DrawingVariable(Control owner)
			: base(owner)
		{
			root = new MathNodeRoot();
			var = (IVariable)root[0];
			this.Size = new Size(20, 20);
			RelativePosition = new RelativePosition(this, -20, -20, true, true);
		}
		public void SetDrawingAttributes(MathNodeRoot r)
		{
			root = r;
			var = (IVariable)root[0];
		}
		public IVariable Variable
		{
			get
			{
				return var;
			}
		}
		public override UInt32 OwnerID
		{
			get
			{
				if (var == null)
					return 0;
				return var.ID;
			}
		}
		public UInt32 RawId
		{
			get
			{
				if (var == null)
					return 0;
				return var.RawId;
			}
		}
		public bool LabelVisible
		{
			get
			{
				return _labelVisible;
			}
			set
			{
				_labelVisible = value;
				this.Visible = value;
			}
		}
		public bool ClonePorts
		{
			get
			{
				if (var != null) return var.ClonePorts;
				return false;
			}
			set
			{
				if (var != null)
					var.ClonePorts = value;
			}
		}
		public void SetVariable(IVariable variable)
		{
			if (variable == null)
			{
				throw new MathException("variable cannot be null");
			}
			var = variable;
			if (variable is MathNode)
			{
				root = ((MathNode)variable).root;
			}
			if (root == null)
			{
				//create a root for drawing the variable
				root = new MathNodeRoot();
				if (var is MathNode)
				{
					root[0] = (MathNode)var;
					((MathNode)var).IsFocused = false;
				}
				else
				{
				}
				root.SetFont(this.Font);
			}
		}
		public void SetTextFont(Font f)
		{
			root.SetFont(f);
		}
		public void SetTextColor(Color c)
		{
			root.SetPen(new Pen(c));
			root.SetBrush(new SolidBrush(c));
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			if (_labelVisible && root != null)
			{
				MathNode mv = var as MathNode;
				if (mv != null)
				{
					SizeF s = mv.CalculateDrawSize(e.Graphics);
					this.Size = new Size((int)s.Width, (int)s.Height);
					mv.Draw(e.Graphics);
				}
			}
		}
		#region IXmlNodeSerializable Members
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			object v;
			if (XmlSerialization.ReadValueFromChildNode(node, "LabelVisible", out v))
			{
				if (v != null)
				{
					_labelVisible = (bool)v;
				}
			}
			this.Visible = _labelVisible;
		}
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlSerialization.WriteValueToChildNode(node, "LabelVisible", _labelVisible);
		}
		#endregion
		public override object Clone()
		{
			DrawingVariable clone = (DrawingVariable)base.Clone();
			clone.LabelVisible = _labelVisible;
			if (root != null)
			{
				MathNodeRoot r = (MathNodeRoot)root.Clone();
				clone.SetDrawingAttributes(r);
			}
			return clone;
		}
	}
}
