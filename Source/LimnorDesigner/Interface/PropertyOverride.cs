/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Property;
using System.ComponentModel;
using System.CodeDom;
using XmlSerializer;
using System.Xml;
using LimnorDesigner.MethodBuilder;
using ProgElements;
using MathExp;
using XmlUtility;

namespace LimnorDesigner.Interface
{
	/// <summary>
	/// override a property with an implementation
	/// </summary>
	public class PropertyOverride : PropertyClassInherited
	{
		#region fields and constructors
		private UInt32 _basePropertyId;
		private UInt32 _baseClassId;
		private bool _baseIsAbstract;
		public PropertyOverride(ClassPointer owner)
			: base(owner)
		{
		}
		#endregion
		#region Methods
		public void CopyFromInherited(PropertyClassInherited baseProperty)
		{
			SetName(baseProperty.Name);
			BasePropertyId = baseProperty.MemberId;
			BaseClassId = baseProperty.Declarer.ClassId;
			PropertyType = baseProperty.PropertyType;
			CanRead = baseProperty.CanRead;
			CanWrite = baseProperty.CanWrite;
			_baseIsAbstract = baseProperty.IsAbstract;
			AccessControl = baseProperty.AccessControl;
		}
		protected override void OnPropertyTypeChanged()
		{
			base.OnPropertyTypeChanged();
			DataTypePointer type = PropertyType;
			//update special values in action parameter-values
			List<PropertyValueClass> values = new List<PropertyValueClass>();
			List<CustomPropertyOverridePointer> items = new List<CustomPropertyOverridePointer>();
			if (Getter != null)
			{
				Getter.Parameters[0].SetDataType(type);
				if (Getter.ActionList != null)
				{
					Dictionary<UInt32, IAction> acts = this.RootPointer.GetActions();
					Getter.ActionList.SetActions(acts);
					Getter.ActionList.FindItemByType<CustomPropertyOverridePointer>(items);
				}
			}
			if (Setter != null)
			{
				if (Setter.ActionList != null)
				{
					Dictionary<UInt32, IAction> acts = this.RootPointer.GetActions();
					Setter.ActionList.SetActions(acts);
					Setter.ActionList.FindItemByType<CustomPropertyOverridePointer>(items);
					Setter.ActionList.FindItemByType<PropertyValueClass>(values);
				}
			}
			foreach (CustomPropertyOverridePointer a in items)
			{
				PropertyOverride po = (PropertyOverride)(a.Property);
				if (po.BaseClassId == this.BaseClassId && po.BasePropertyId == this.BasePropertyId)
				{
					po.SetMembers(CanRead, CanWrite, AccessControl, type);
				}
			}
			foreach (PropertyValueClass a in values)
			{
				a.SetDataType(type);
			}
		}
		public override void SetMembers(bool canRead, bool canWrite, EnumAccessControl access, DataTypePointer type)
		{
			bool bTypeChanged = !type.IsSameObjectRef(PropertyType);
			base.SetMembers(canRead, canWrite, access, type);
			if (bTypeChanged)
			{
				OnPropertyTypeChanged();
			}
		}
		public override CustomPropertyPointer CreatePointer()
		{
			if (HasBaseImplementation)
			{
				CustomPropertyOverridePointer p = new CustomPropertyOverridePointer(this, Holder);
				p.UseBaseValue = true;
				return p;
			}
			else
			{
				return new CustomPropertyPointer(this, Holder);
			}
		}
		protected override void OnNameSet()
		{
			ClassPointer decl = (ClassPointer)Owner;
			PropertyClassInherited p = decl.GetBaseProperty(Name, BaseClassId, BasePropertyId);
			if (p == null)
			{
				throw new DesignerException("Base property [{0}] not found for class [{1},{2}]", Name, decl.ClassId, decl.Name);
			}
			CopyFromInherited(p);
		}
		#endregion
		#region Properties
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
		[ReadOnly(true)]
		[Browsable(false)]
		public override UInt32 MemberId
		{
			get
			{
				return base.MemberId;
			}
			set
			{
				base.MemberId = value;
			}
		}
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
					bool bNeedCompile = false;
					GetterClass g = Getter;
					if (CanRead && g != null && g.ActionList != null && g.ActionList.Count == 1)
					{
						bNeedCompile = true;
						//if the getter contains only the default action them do not need compile
						ClassPointer cp = this.Holder as ClassPointer;
						g.ActionList.SetActions(cp.GetActions());
						List<IAction> acts = g.ActionList.GetActions();
						if (acts != null && acts.Count == 1)
						{
							MethodReturnMethod rm = acts[0].ActionMethod as MethodReturnMethod;
							if (rm != null && acts[0].ParameterValues.Count == 1)
							{
								ParameterValue pv = acts[0].ParameterValues[0];
								if (pv != null && pv.ValueType == EnumValueType.Property)
								{
									CustomPropertyOverridePointer pop = pv.Property as CustomPropertyOverridePointer;
									if (pop != null)
									{
										if (pop.UseBaseValue)
										{
											bNeedCompile = false;
										}
									}
								}
							}
						}
					}
					if (!bNeedCompile)
					{
						SetterClass sc = Setter;
						if (CanWrite && sc != null && sc.ActionList != null && sc.ActionList.Count == 1)
						{
							bNeedCompile = true;
							ClassPointer cp = this.Holder as ClassPointer;
							sc.ActionList.SetActions(cp.GetActions());
							List<IAction> acts = sc.ActionList.GetActions();
							if (acts != null && acts.Count == 1)
							{
								SetterPointer sp = acts[0].ActionMethod as SetterPointer;
								if (sp != null && acts[0].ParameterValues.Count == 1)
								{
									CustomPropertyOverridePointer pop = sp.SetProperty as CustomPropertyOverridePointer;
									if (pop != null && pop.UseBaseValue)
									{
										ParameterValue pv = acts[0].ParameterValues[0];//sp.GetParameterValue("value") as ParameterValue;
										if (pv != null && pv.ValueType == EnumValueType.Property)
										{
											PropertyValueClass pvc = pv.Property as PropertyValueClass;
											if (pvc != null)
											{
												bNeedCompile = false;
											}
										}
									}
								}
							}
						}
					}
					return !bNeedCompile;
				}
				return false;
			}
		}

		[Browsable(false)]
		public override MemberAttributes PropertyAttributes
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


		[Browsable(false)]
		public UInt32 BasePropertyId
		{
			get
			{
				return _basePropertyId;
			}
			set
			{
				_basePropertyId = value;
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
		[ReadOnly(true)]
		[Browsable(false)]
		public override UInt64 WholeId
		{
			get
			{
				return DesignUtil.MakeDDWord(MemberId, ClassId);
			}
			set
			{
			}
		}
		#endregion
		#region ISerializerProcessor Members

		public override void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			base.OnPostSerialize(objMap, objectNode, saved, serializer);
			MemberId = XmlUtility.XmlUtil.GetAttributeUInt(objectNode, XmlTags.XMLATT_memberId);
			//load attributes from base
			ClassPointer root = objMap.GetTypedData<ClassPointer>();
			PropertyClassInherited p = root.GetBaseProperty(this.Name, BaseClassId, BasePropertyId);
			CopyFromInherited(p);
		}

		#endregion
		#region ICloneable Members

		public override object Clone()
		{
			PropertyOverride po = (PropertyOverride)base.Clone();
			po._baseClassId = _baseClassId;
			po._baseIsAbstract = _baseIsAbstract;
			po._basePropertyId = _basePropertyId;
			return po;
		}
		#endregion
	}
}
