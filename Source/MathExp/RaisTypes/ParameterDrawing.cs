/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;

namespace MathExp.RaisTypes
{
	public class ParameterDrawing : ObjectDrawing
	{
		#region fields and constructors
		public const int DefaultDotSize = 4;
		public ParameterDrawing()
		{
		}
		#endregion
		#region properties
		public Parameter Parameter
		{
			get
			{
				return (Parameter)this.ObjectEntity;
			}
			set
			{
				this.ObjectEntity = value;
				this.SetIniLocation(Parameter.Location);
			}
		}
		#endregion
		#region methods
		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);
			Parameter.Location = this.Location;
		}
		#endregion
		#region IMathExpViewer Members
		public override MathExpItem ExportMathItem(MathExpGroup group)
		{
			MathExpItem item = base.ExportMathItem(group);
			Parameter.Location = this.Location;
			item.MathExpression = this.Parameter;
			return item;
		}
		public override IVariable FindVariableById(UInt32 id)
		{
			LinkLineNodeOutPort[] ports = this.Parameter.OutPorts;
			for (int i = 0; i < ports.Length; i++)
			{
				if (ports[i].Variable.ID == id)
					return ports[i].Variable;
			}
			return null;
		}
		#endregion
		#region ICloneable
		public override object Clone()
		{
			ParameterDrawing clone = (ParameterDrawing)base.Clone();
			return clone;
		}
		#endregion
	}
}
