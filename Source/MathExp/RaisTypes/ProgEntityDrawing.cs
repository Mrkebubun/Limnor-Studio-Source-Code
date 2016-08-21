/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using VPL;

namespace MathExp.RaisTypes
{
	public class ProgEntityDrawing : ObjectDrawing
	{
		#region fields and constructors
		private const string XML_LABEL = "Label";
		private DrawingLabel _label;
		private Bitmap _entityImage;
		/// <summary>
		/// 
		/// </summary>
		public ProgEntityDrawing()
		{
			_messageReturn = false;
		}
		public ProgEntityDrawing(IProgramEntity pe)
			: base(pe)
		{
			_messageReturn = false;
			_entityImage = pe.CreateDrawIcon();
		}
		#endregion
		#region properties
		public override bool NeedSaveSize
		{
			get
			{
				return true;
			}
		}
		#endregion
		#region methods
		/// <summary>
		/// for clone
		/// </summary>
		/// <param name="label"></param>
		public void SetLabel(DrawingLabel label)
		{
			_label = label;
			_label.Move += new EventHandler(_label_Move);
		}
		public void SetLabelText(string text)
		{
			if (_label == null)
			{
				_label = new DrawingLabel(this);
				_label.RelativePosition = new RelativePosition(_label, this.Width, this.Height + 2, true, true);
				_label.Move += new EventHandler(_label_Move);
				if (this.Parent != null)
				{
					this.Parent.Controls.Add(_label);
				}
			}
			_label.Text = text;
		}

		void _label_Move(object sender, EventArgs e)
		{
			_label.SaveLocation();
			_label.SaveRelativePosition();
			if (XmlData != null)
			{
				XmlSerialization.WriteToUniqueChildXmlNode(_writer, XmlData, XML_LABEL, _label);
			}
			VplDesigner.Changed = true;
		}
		public override void OnDesirialize()
		{
			OnEntityChanged();
			SetLabelText(ObjectEntity.Name);
		}
		protected override void OnEntityChanged()
		{
			_entityImage = ObjectEntity.CreateDrawIcon();
		}
		/// <summary>
		/// it does not include link ports and line nodes
		/// </summary>
		/// <returns></returns>
		public override List<Control> GetAllControls()
		{
			List<Control> cs = base.GetAllControls();
			if (_label != null)
			{
				cs.Add(_label);
			}
			return cs;
		}
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			if (_entityImage != null)
			{
				e.Graphics.DrawImage(_entityImage, 0, 0, this.Width, this.Height);
			}
			else
			{
				base.OnPaint(e);
			}
		}

		#endregion
		#region ICloneable
		public override object Clone()
		{
			ProgEntityDrawing obj = (ProgEntityDrawing)base.Clone();
			if (_label != null)
			{
				obj.SetLabel((DrawingLabel)_label.Clone());
			}
			return obj;
		}
		#endregion
		#region IXmlNodeSerializable Members
		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			//it will also set the owner for _label
			_label = (DrawingLabel)XmlSerialization.ReadFromChildXmlNode(serializer, node, XML_LABEL, this);
			if (_label != null)
			{
				_label.Move += new EventHandler(_label_Move);
			}
		}
		IXmlCodeWriter _writer;
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			_writer = serializer;
			base.OnWriteToXmlNode(serializer, node);
			if (_label != null)
			{
				XmlSerialization.WriteToUniqueChildXmlNode(serializer, node, XML_LABEL, _label);
			}
		}
		#endregion
	}
}
