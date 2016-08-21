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
using System.CodeDom;
using ProgElements;
using XmlSerializer;
using System.Xml;
using XmlUtility;

namespace LimnorDesigner.Interface
{
	public class MethodOverride : MethodClassInherited
	{
		#region fields and constructors
		private UInt32 _baseMethodId;
		private UInt32 _baseClassId;
		private bool _baseIsAbstract;
		public MethodOverride(ClassPointer owner)
			: base(owner)
		{
		}
		#endregion
		#region Methods
		public void CopyFromInherited(MethodClassInherited baseMethod)
		{
			Name = baseMethod.Name;
			BaseMethodId = baseMethod.MemberId;
			BaseClassId = baseMethod.Declarer.ClassId;
			_baseIsAbstract = baseMethod.IsAbstract;
			AccessControl = baseMethod.AccessControl;
			ReturnValue = baseMethod.ReturnValue;
			DataTypePointer[] dps = new DataTypePointer[baseMethod.ParameterCount];
			for (int i = 0; i < baseMethod.ParameterCount; i++)
			{
				dps[i] = baseMethod.Parameters[i];
			}
			Parameters = baseMethod.Parameters;
		}
		protected override void CopyFromThis(MethodClass obj)
		{
			base.CopyFromThis(obj);
			MethodOverride mo = (MethodOverride)obj;
			mo._baseIsAbstract = _baseIsAbstract;
			mo._baseMethodId = _baseMethodId;
			mo._baseClassId = _baseClassId;
		}
		protected override void InitializeNewMethod(XmlNode rootNode, ILimnorDesignerLoader loader)
		{
			MemberId = (UInt32)(Guid.NewGuid().GetHashCode());
		}
		#endregion
		#region Properties
		[Browsable(false)]
		[ReadOnly(true)]
		public override bool IsAbstract
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
		public override bool HasBaseImplementation
		{
			get
			{
				return !_baseIsAbstract;
			}
		}
		[Browsable(false)]
		public override bool Implemented
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		public override bool DoNotCompile
		{
			get
			{
				if (HasBaseImplementation)
				{
					bool bNeedCompile = true;
					//if there is only one action calling the base version with the same parameter then do not compile
					if (this.ActionList != null && ActionList.Count == 1)
					{
						ClassPointer cp = this.Holder as ClassPointer;
						if (cp == null)
						{
							ClassInstancePointer cip = this.Holder as ClassInstancePointer;
							if (cip != null)
							{
								if (cip.Definition != null)
								{
									ActionList.SetActions(cip.Definition.GetActions());
								}
							}
						}
						else
						{
							ActionList.SetActions(cp.GetActions());
						}
						List<IAction> acts = ActionList.GetActions();
						if (acts != null && acts.Count == 1)
						{
							BaseMethod bm = acts[0].ActionMethod as BaseMethod;
							if (bm != null)
							{
							}
						}
					}
					return !bNeedCompile;
				}
				return false;
			}
		}

		[Browsable(false)]
		public override MemberAttributes MethodAttributes
		{
			get
			{
				MemberAttributes a;
				if (this.AccessControl == EnumAccessControl.Public)
				{
					a = MemberAttributes.Public;
				}
				else if (this.AccessControl == EnumAccessControl.Private)
				{
					a = MemberAttributes.Private;
				}
				else
				{
					a = MemberAttributes.Family;
				}
				a |= MemberAttributes.Override;
				return a;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override bool IsStatic
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override UInt32 ClassId
		{
			get
			{
				ClassPointer root = this.RootPointer;
				if (root != null)
				{
					return root.ClassId;
				}
				return 0;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public UInt32 BaseMethodId
		{
			get
			{
				return _baseMethodId;
			}
			set
			{
				_baseMethodId = value;
			}
		}
		[Browsable(false)]
		public UInt32 BaseClassId
		{
			get
			{
				return _baseClassId;
			}
			set
			{
				_baseClassId = value;
			}
		}
		#endregion
		#region ISerializerProcessor Members

		public override void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			base.OnPostSerialize(objMap, objectNode, saved, serializer);
			MemberId = XmlUtility.XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_MethodID);
			//load attributes from base
			ClassPointer root = objMap.GetTypedData<ClassPointer>();
			MethodClassInherited p = root.GetBaseMethod(this.MethodSignature, BaseClassId, BaseMethodId);
			CopyFromInherited(p);
		}

		#endregion
	}
}
