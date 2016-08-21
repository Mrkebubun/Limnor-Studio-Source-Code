/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using VSPrj;
using System.Globalization;
using LimnorDesigner.Interface;
using LimnorDesigner.Property;
using LimnorDesigner.Event;
using VPL;

namespace LimnorDesigner.DesignTimeType
{
	/// <summary>
	/// design time type of a ClassPointer
	/// </summary>
	public class TypeX : Type
	{
		private UInt32 _classId;
		private ClassPointer _classPointer;
		public TypeX(UInt32 classId, LimnorProject project)
		{
			_classId = classId;
			_classPointer = ClassPointer.CreateClassPointer(classId, project);
		}
		public TypeX(ClassPointer p)
		{
			_classPointer = p;
			_classId = p.ClassId;
		}
		public TypeX(UInt32 classId, ILimnorStudioProject project)
			: this(classId, (LimnorProject)project)
		{
		}
		public UInt32 ClassId
		{
			get
			{
				return _classId;
			}
		}
		public ClassPointer Owner
		{
			get
			{
				return _classPointer;
			}
		}
		public override Assembly Assembly
		{
			get { return null; }
		}

		public override string AssemblyQualifiedName
		{
			get
			{
				if (_classPointer != null)
					return _classPointer.TypeString;
				return null;
			}
		}

		public override Type BaseType
		{
			get
			{
				if (_classPointer != null)
					return _classPointer.BaseClassType;
				return null;
			}
		}

		public override string FullName
		{
			get
			{
				if (_classPointer != null)
					return _classPointer.TypeString;
				return null;
			}
		}

		public override Guid GUID
		{
			get
			{
				if (_classPointer != null)
					return _classPointer.ClassGuid;
				return Guid.Empty;
			}
		}

		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			if (_classPointer != null)
				if (_classPointer.IsInterface)
					return TypeAttributes.Interface;
			return TypeAttributes.Class;
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return new ConstructorInfoX(this);
		}

		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			return new ConstructorInfo[] { new ConstructorInfoX(this) };
		}

		public override Type GetElementType()
		{
			return null;
		}

		public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
		{
			if (_classPointer != null)
			{
				return _classPointer.GetEventInfo(name, bindingAttr);
			}
			return null;
		}

		public override EventInfo[] GetEvents(BindingFlags bindingAttr)
		{
			if (_classPointer != null)
			{
				return _classPointer.GetAllEvents(bindingAttr);
			}
			return new EventInfo[] { };
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			return null;
		}

		public override FieldInfo[] GetFields(BindingFlags bindingAttr)
		{
			return new FieldInfo[] { };
		}

		public override Type GetInterface(string name, bool ignoreCase)
		{
			if (_classPointer != null)
			{
				List<InterfacePointer> ifps = _classPointer.GetInterfaces();
				if (ifps != null)
				{
					foreach (InterfacePointer ifp in ifps)
					{
						if (ignoreCase)
						{
							if (string.Compare(ifp.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
							{
								return ifp.DataTypeEx;
							}
						}
						else
						{
							if (string.CompareOrdinal(ifp.Name, name) == 0)
							{
								return ifp.DataTypeEx;
							}
						}
					}
				}
			}
			return null;
		}

		public override Type[] GetInterfaces()
		{
			List<Type> tps = new List<Type>();
			if (_classPointer != null)
			{
				List<InterfacePointer> ifps = _classPointer.GetInterfaces();
				if (ifps != null)
				{
					foreach (InterfacePointer ifp in ifps)
					{
						tps.Add(ifp.DataTypeEx);
					}
				}
			}
			return tps.ToArray();
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			List<MemberInfo> lst = new List<MemberInfo>();
			PropertyInfo[] pfs = GetProperties(bindingAttr);
			if (pfs != null)
			{
				lst.AddRange(pfs);
			}
			MethodInfo[] mfs = GetMethods(bindingAttr);
			if (mfs != null)
			{
				lst.AddRange(mfs);
			}
			EventInfo[] efs = GetEvents(bindingAttr);
			if (efs != null)
			{
				lst.AddRange(efs);
			}
			FieldInfo[] ffs = GetFields(bindingAttr);
			if (ffs != null)
			{
				lst.AddRange(ffs);
			}
			return lst.ToArray();
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (_classPointer != null)
			{
				return _classPointer.GetMethodInfo(name, bindingAttr);
			}
			return null;
		}
		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			if (_classPointer != null)
			{
				return _classPointer.GetAllMethods(bindingAttr);
			}
			return new MethodInfo[] { };
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			return null;
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			return null;
		}

		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
		{
			if (_classPointer != null)
			{
				return _classPointer.GetAllProperties(bindingAttr);
			}
			return new PropertyInfo[] { };
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			if (_classPointer != null)
			{
				return _classPointer.GetPropertyInfo(name, bindingAttr);
			}
			return null;
		}

		protected override bool HasElementTypeImpl()
		{
			return false;
		}

		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			return null;
		}

		protected override bool IsArrayImpl()
		{
			return false;
		}

		protected override bool IsByRefImpl()
		{
			return true;
		}

		protected override bool IsCOMObjectImpl()
		{
			return false;
		}

		protected override bool IsPointerImpl()
		{
			return true;
		}

		protected override bool IsPrimitiveImpl()
		{
			return false;
		}

		public override Module Module
		{
			get { return null; }
		}

		public override string Namespace
		{
			get
			{
				if (_classPointer != null)
					return _classPointer.Namespace;
				return null;
			}
		}

		public override Type UnderlyingSystemType
		{
			get
			{
				if (_classPointer != null)
					return _classPointer.BaseClassType;
				return null;
			}
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			List<object> vs = new List<object>();
			if (attributeType != null)
			{
				if (_classPointer != null)
				{
					IList<ConstObjectPointer> lst = _classPointer.GetCustomAttributeList();
					if (lst != null && lst.Count > 0)
					{
						foreach (ConstObjectPointer o in lst)
						{
							if (attributeType == null || attributeType.IsAssignableFrom(o.BaseClassType))
							{
								vs.Add(o.Value);
							}
						}
					}
					if (inherit)
					{
						object[] vsh = _classPointer.BaseClassType.GetCustomAttributes(inherit);
						if (vsh != null)
						{
							for (int i = 0; i < vsh.Length; i++)
							{
								if (vsh[i] != null)
								{
									if (attributeType == null || attributeType.IsAssignableFrom(vsh[i].GetType()))
									{
										vs.Add(vsh[i]);
									}
								}
							}
						}
					}
				}
			}
			return vs.ToArray();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return GetCustomAttributes(null, inherit);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			object[] vs = GetCustomAttributes(attributeType, inherit);
			if (vs != null)
			{
				for (int i = 0; i < vs.Length; i++)
				{
					if (vs[i] != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override string Name
		{
			get
			{
				if (_classPointer != null)
					return _classPointer.Name;
				return null;
			}
		}

	}
	#region ConstructorInfoX
	public class ConstructorInfoX : ConstructorInfo
	{
		private TypeX _type;
		public ConstructorInfoX(TypeX type)
		{
			_type = type;
		}

		public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			return null;
		}

		public override MethodAttributes Attributes
		{
			get { return MethodAttributes.Public; }
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MethodImplAttributes.Managed;
		}

		public override ParameterInfo[] GetParameters()
		{
			return new ParameterInfo[] { };
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			return null;
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get { return new RuntimeMethodHandle(); }
		}

		public override Type DeclaringType
		{
			get { return _type; }
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return null;
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return null;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return false;
		}

		public override string Name
		{
			get { return "_ctr"; }
		}

		public override Type ReflectedType
		{
			get { return _type; }
		}
	}
	#endregion
}
