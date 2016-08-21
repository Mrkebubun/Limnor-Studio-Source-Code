/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using VPL;
using System.Xml;
using XmlUtility;
using XmlSerializer;
using VSPrj;
using MathExp;
using System.Globalization;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// it is a variable receiving a custom method return value for an action
	/// </summary>
	public class CustomMethodReturnPointer : LocalVariable, IDelayedInitialize
	{
		private UInt32 _methodId;
		private LimnorProject _prj;
		public CustomMethodReturnPointer()
		{
		}
		public CustomMethodReturnPointer(DataTypePointer type, string name, UInt32 classId, UInt32 memberId)
			: base(type, name, classId, memberId)
		{
		}
		[Browsable(false)]
		public UInt32 MethodId
		{
			get
			{
				return _methodId;
			}
			set
			{
				_methodId = value;
			}
		}
		[Browsable(false)]
		public override Type ObjectType
		{
			get
			{
				if (base.ObjectType == null)
				{
					resolveDataType();
				}
				return base.ObjectType;
			}
			set
			{
				base.ObjectType = value;
			}
		}
		[Browsable(false)]
		public override bool IsValid
		{
			get
			{
				if (ObjectType != null && !string.IsNullOrEmpty(this.Name))
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "(ObjectType={2},Name={3}) for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name, ObjectType, Name);
				return false;
			}
		}
		public override object Clone()
		{
			CustomMethodReturnPointer v = base.Clone() as CustomMethodReturnPointer;
			v._methodId = _methodId;
			v._prj = _prj;
			return v;
		}
		public override ComponentIconLocal CreateComponentIcon(ILimnorDesigner designer, MethodClass method)
		{
			return new ComponentIconMethodReturnPointer(designer, this, method);
		}
		protected override void OnWrite(IXmlCodeWriter writer, XmlNode node)
		{
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_MethodID, _methodId);
			string s = this.Name;
			if (!string.IsNullOrEmpty(s))
			{
				XmlUtil.SetNameAttribute(node, s);
			}
			if (_prj == null)
			{
				XmlObjectWriter w = writer as XmlObjectWriter;
				if (w != null && w.ObjectList != null)
				{
					_prj = w.ObjectList.Project;
				}
			}
		}
		protected override void OnRead(IXmlCodeReader reader, XmlNode node)
		{
			_methodId = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_MethodID);
			string s = XmlUtil.GetNameAttribute(node);
			if (!string.IsNullOrEmpty(s))
			{
				this.SetName(s);
			}
			if (_prj == null)
			{
				XmlObjectReader r = reader as XmlObjectReader;
				if (r != null && r.ObjectList != null)
				{
					_prj = r.ObjectList.Project;
				}
			}
		}
		public override void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			if (_prj == null)
			{
				_prj = objMap.Project;
			}
			resolveDataType();
		}
		private void resolveDataType()
		{
			if (this.ClassType == null)
			{
				if (ClassId != 0 && _methodId != 0 && _prj != null)
				{
					ClassPointer root = _prj.GetTypedData<ClassPointer>(ClassId);
					if (root != null)
					{
						if (root.IsLoadingMethods())
						{
							//try read type from XML?
						}
						else
						{
							MethodClass mc = root.GetCustomMethodById(_methodId);
							if (mc != null)
							{
								this.SetDataType(mc.ReturnValue);
							}
						}
					}
				}
			}
		}

		#region IDelayedInitialize Members

		public void OnDelayedPostSerialize(ObjectIDmap objMap, XmlNode objectNode, XmlObjectReader reader)
		{
			if (_prj == null)
			{
				_prj = objMap.Project;
			}
			resolveDataType();
		}

		public void SetReader(ObjectIDmap objMap, XmlNode objectNode, XmlObjectReader reader)
		{

		}

		#endregion
	}
}