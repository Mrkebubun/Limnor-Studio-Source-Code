/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlSerializer;
using System.Xml;
using XmlUtility;
using System.ComponentModel;
using VPL;
using System.Globalization;
using LimnorDesigner.Interface;

namespace LimnorDesigner.Event
{
	/// <summary>
	/// a custom delegate type.
	/// derive from ClassPointer so that it can be held by a DataTypePointer.
	/// </summary>
	[InternalType]
	public class CustomEventHandlerType : ClassPointer, IComponent
	{
		#region fields and constructors
		private List<NamedDataType> _parameters;
		public CustomEventHandlerType()
		{

		}
		public CustomEventHandlerType(UInt32 classId, UInt32 memberId, ObjectIDmap objList)
			: base(classId, memberId, objList, null)
		{
		}
		public CustomEventHandlerType(ObjectIDmap objList)
			: this(
			objList.ClassId,
			objList.MemberId,
			objList
			)
		{
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public override InterfaceClass Interface
		{
			get
			{
				return null;
			}
		}
		public override bool IsInterface
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override object ObjectInstance
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public override UInt32 ClassId
		{
			get
			{
				UInt32 cid = base.ClassId;
				if (cid == 0)
				{
					cid = (UInt32)(Guid.NewGuid().GetHashCode());
					base.ClassId = cid;
				}
				return cid;
			}
			set
			{
				base.ClassId = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override UInt32 MemberId
		{
			get
			{
				base.MemberId = 1;
				return 1;
			}
			set
			{
				base.MemberId = 1;
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get
			{
				if (_parameters == null)
					return 0;
				return _parameters.Count;
			}
		}
		[Browsable(false)]
		public virtual List<NamedDataType> Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				_parameters = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override Type ObjectType
		{
			get
			{
				return typeof(CustomEventHandlerType);
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[ParenthesizePropertyName(true)]
		[Description("Class or member name")]
		public override string Name
		{
			get
			{
				return "Handler";
			}
			set
			{
			}
		}
		#endregion
		#region IComponent Members

		public event EventHandler Disposed;
		private ISite _site;
		[Browsable(false)]
		[ReadOnly(true)]
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion
		#region IDisposable Members

		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion
		#region IXmlNodeSerializable Members

		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_ClassID, ClassId);
			if (_parameters != null)
			{
				XmlNode pns = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_PARAMLIST);
				foreach (NamedDataType ndt in _parameters)
				{
					XmlNode itemNode = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
					pns.AppendChild(itemNode);
					ndt.OnWriteToXmlNode(writer, itemNode);
				}
			}
		}

		public override void OnReadFromXmlNode(IXmlCodeReader reader0, XmlNode node)
		{
			_parameters = null;
			XmlObjectReader reader = (XmlObjectReader)reader0;
			ClassId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ClassID);
			XmlNodeList nds = node.SelectNodes(string.Format(CultureInfo.InvariantCulture,
				"{0}/{1}", XmlTags.XML_PARAMLIST, XmlTags.XML_Item));
			if (nds != null)
			{
				_parameters = new List<NamedDataType>();
				foreach (XmlNode itemNode in nds)
				{
					NamedDataType ndt = new NamedDataType();
					ndt.OnReadFromXmlNode(reader, itemNode);
					_parameters.Add(ndt);
				}
			}
		}
		[Browsable(false)]
		public override void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
		}
		#endregion
	}
}
