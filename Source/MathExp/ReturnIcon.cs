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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using MathExp.RaisTypes;
using VPL;

namespace MathExp
{
	/// <summary>
	/// it has a var (MathNodeVariableDummy) to hold inport and outports
	/// </summary>
	[ToolboxBitmapAttribute(typeof(ReturnIcon), "Resources.ReturnIcon.bmp")]
	public partial class ReturnIcon : ActiveDrawing, IComponentWithID
	{
		#region fields and constructors
		const string XML_VAR = "Var";
		private Bitmap bmp;
		private MathNodeVariableDummy var;
		public ReturnIcon()
			: this(null)
		{
		}
		public ReturnIcon(RaisDataType dataType)
		{
			Location = new Point(300, 300);
			SaveLocation();
			this.Size = new Size(32, 32);
			System.Drawing.ToolboxBitmapAttribute tba = TypeDescriptor.GetAttributes(this.GetType())[typeof(System.Drawing.ToolboxBitmapAttribute)] as System.Drawing.ToolboxBitmapAttribute;
			if (tba != null)
			{
				bmp = (System.Drawing.Bitmap)tba.GetImage(this.GetType());
			}
			MathNodeRoot root = new MathNodeRoot();
			root.IsVariableHolder = true;
			var = new MathNodeVariableDummy(root);
			root[0] = var;
			if (dataType != null)
			{
				var.VariableType = dataType;
				var.TypeDefined = true;
			}
			LinkLineNodeInPort port = new LinkLineNodeInPort(var);
			port.ClearLine();
			port.SetPrevious(null);
			port.Owner = this;
			port.HideLabel();
			var.InPort = port;
			port.Owner = this;
			port.Label.Visible = false;
			port.Location = new Point(this.Left + this.Width / 2 - port.Width / 2, this.Top - port.Height);
			port.SaveLocation();
		}

		#endregion
		#region properties
		public bool TypeDefined
		{
			get
			{
				return var.TypeDefined;
			}
			set
			{
				var.TypeDefined = value;
			}
		}
		[Browsable(false)]
		public LinkLineNodeInPort InPort
		{
			get
			{
				if (var != null)
				{
					return var.InPort;
				}
				return null;
			}
		}
		[Browsable(false)]
		public LinkLineNodeOutPort[] OutPorts
		{
			get
			{
				if (var != null)
				{
					return var.OutPorts;
				}
				return null;
			}
		}
		[Browsable(false)]
		public IVariable ReturnVariable
		{
			get
			{
				return var;
			}
		}
		#endregion
		#region IXmlNodeSerializable Members
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			XmlNode nd = node.SelectSingleNode(XML_VAR);
			if (nd != null)
			{
				MathNodeRoot root = new MathNodeRoot();
				root.IsVariableHolder = true;
				root[0] = var;
				var.Load(nd);
				if (var.InPort != null)
				{
					var.InPort.Owner = this;
					var.InPort.Label.Visible = false;
				}
				if (var.OutPorts != null)
				{
					LinkLineNodeOutPort[] o = var.OutPorts;
					for (int i = 0; i < o.Length; i++)
					{
						o[i].Owner = this;
					}
				}
			}
			this.Size = new Size(32, 32);
		}
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlNode nd = node.SelectSingleNode(XML_VAR);
			if (nd == null)
			{
				nd = node.OwnerDocument.CreateElement(XML_VAR);
				node.AppendChild(nd);
			}
			else
			{
				nd.RemoveAll();
			}
			var.Save(nd);
		}
		#endregion
		#region Methods
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.DrawImage(bmp, new Point(0, 0));
		}
		public void SetBmp(Bitmap img)
		{
			bmp = img;
		}
		public void SetVar(MathNodeVariableDummy v)
		{
			var = v;
			if (var.InPort != null)
			{
				var.InPort.Owner = this;
			}
		}
		public void CheckReturnIconPos(object sender, EventArgs e)
		{
			Control parent = sender as Control;
			if (parent != null)
			{
				if (this.Left == 0 && this.Top == 0)
				{
					this.Left = (parent.ClientSize.Width - this.Width) / 2;
					this.Top = parent.ClientSize.Height - this.Height;
				}
			}
		}
		public void SetType(RaisDataType t)
		{
			var.VariableType = t;
		}
		public void SetPortId(UInt32 id)
		{
			var.MathExpression.Dummy.ResetID(id);
		}
		#endregion
		#region IComponentWithID Members

		public UInt32 ComponentID
		{
			get
			{
				return var.ID;
			}
		}
		public bool ContainsID(UInt32 id)
		{
			return (var.ID == id);
		}
		#endregion
		#region ICloneable Members

		public override object Clone()
		{
			ReturnIcon obj = (ReturnIcon)base.Clone();
			if (bmp != null)
			{
				obj.SetBmp((Bitmap)bmp.Clone());
			}
			MathNodeRoot root = (MathNodeRoot)var.root.Clone();
			obj.SetVar((MathNodeVariableDummy)root[0]);
			obj.TypeDefined = this.TypeDefined;
			return obj;
		}

		#endregion
	}
}
