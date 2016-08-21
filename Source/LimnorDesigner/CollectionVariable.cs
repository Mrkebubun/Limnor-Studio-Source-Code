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
using LimnorDesigner.MenuUtil;
using LimnorDesigner.MethodBuilder;
using System.ComponentModel;
using System.Xml;
using System.Drawing.Design;
using System.Reflection;
using VPL;
using System.CodeDom;
using MathExp;

namespace LimnorDesigner
{
	public class CollectionVariable : LocalVariable, IClassWrapper
	{
		#region fields and constructors
		private string _desc;
		public CollectionVariable()
		{
		}
		public CollectionVariable(CollectionTypePointer type, string name, UInt32 classId, UInt32 memberId)
			: base(type, name, classId, memberId)
		{
		}
		#endregion
		#region Methods
		public override ComponentIconLocal CreateComponentIcon(ILimnorDesigner designer, MethodClass method)
		{
			return new ComponentIconCollectionPointer(designer, this, method);
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public Type WrappedType
		{
			get
			{
				CollectionTypePointer ap = ClassType as CollectionTypePointer;
				return ap.BaseClassType;
			}
		}
		[Browsable(false)]
		public override DataTypePointer ClassType
		{
			get
			{
				return base.ClassType;
			}
		}
		/// <summary>
		/// it is read-only for generic type ICollection<T> or IEnumerable<T>
		/// </summary>
		[Editor(typeof(PropEditorDataType), typeof(UITypeEditor))]
		[Description("Data type for each collection item")]
		public DataTypePointer CollectionItemType
		{
			get
			{
				CollectionTypePointer ap = ClassType as CollectionTypePointer;
				if (ap != null)
				{
					return new DataTypePointer(new TypePointer(ap.ItemBaseType));
				}
				return base.ClassType;
			}
			set
			{
				SetDataType(value);
			}
		}
		public Type CollectionType
		{
			get
			{
				CollectionTypePointer ap = ClassType as CollectionTypePointer;
				if (ap != null)
				{
					return ap.CollectionType;
				}
				return null;
			}
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		#endregion

		#region ICloneable Members

		public override object Clone()
		{
			CollectionVariable v = (CollectionVariable)base.Clone();// (Owner, (CollectionTypePointer)ClassType, Name, ClassId, MemberId);
			v.Description = Description;
			return v;
		}

		#endregion

		#region ISerializerProcessor Members

		public override void OnPostSerialize(XmlSerializer.ObjectIDmap objMap, System.Xml.XmlNode objectNode, bool saved, object serializer)
		{
		}

		#endregion

		#region IClass Members
		[Browsable(false)]
		public override Type VariableLibType
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public override ClassPointer VariableCustomType
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		public override IClassWrapper VariableWrapperType
		{
			get
			{
				return this;
			}
		}
		#endregion

		#region IClassWrapper Members

		public string Description
		{
			get { return _desc; }
			set { _desc = value; }
		}
		public MethodInfo GetMethod(string name, Type[] types)
		{
			if (name == SubMethodInfo.ExecuteForEachItem)
			{
				DataTypePointer ti = new DataTypePointer(CollectionItemType.LibTypePointer.ClassType);
				if (ti.IsGenericParameter)
				{
					DataTypePointer dp = GetConcreteType(ti.BaseClassType);
					if (dp != null)
					{
						ti.SetConcreteType(dp);
					}
				}
				return new CollectionForEachMethodInfo(ti, CollectionType, MemberId.ToString("x"));
			}
			else
			{
				Type t = WrappedType;
				MethodInfo mif = t.GetMethod(name, types);
				if (mif == null)
				{
					Type[] tps = t.GetInterfaces();
					if (tps != null && tps.Length > 0)
					{
						for (int i = 0; i < tps.Length; i++)
						{
							mif = tps[i].GetMethod(name, types);
							if (mif != null)
							{
								break;
							}
						}
					}
				}
				return mif;
			}
		}
		public SortedDictionary<string, MethodInfo> GetMethods()
		{
			SortedDictionary<string, MethodInfo> methods = new SortedDictionary<string, MethodInfo>();
			Type t = WrappedType;
			MethodInfo[] mifs = t.GetMethods();
			for (int i = 0; i < mifs.Length; i++)
			{
				if (!mifs[i].IsSpecialName)
				{
					string s = MethodPointer.GetMethodSignature(mifs[i]);
					methods.Add(s, mifs[i]);
				}
			}
			Type[] tps = t.GetInterfaces();
			if (tps != null && tps.Length > 0)
			{
				for (int i = 0; i < tps.Length; i++)
				{
					mifs = tps[i].GetMethods();
					for (int k = 0; k < mifs.Length; k++)
					{
						if (!mifs[k].IsSpecialName)
						{
							string s = MethodPointer.GetMethodSignature(mifs[k]);
							if (!methods.ContainsKey(s))
							{
								methods.Add(s, mifs[k]);
							}
						}
					}
				}
			}
			DataTypePointer ti = new DataTypePointer(CollectionItemType.LibTypePointer.ClassType);
			if (ti.IsGenericParameter)
			{
				DataTypePointer dp = GetConcreteType(ti.BaseClassType);
				if (dp != null)
				{
					ti.SetConcreteType(dp);
				}
			}
			CollectionForEachMethodInfo af = new CollectionForEachMethodInfo(ti, CollectionType, MemberId.ToString("x"));
			methods.Add(af.Name, af);
			return methods;
		}
		public SortedDictionary<string, EventInfo> GetEvents()
		{
			SortedDictionary<string, EventInfo> events = new SortedDictionary<string, EventInfo>();
			Type t = WrappedType;
			EventInfo[] eifs = t.GetEvents();
			for (int i = 0; i < eifs.Length; i++)
			{
				events.Add(eifs[i].Name, eifs[i]);
			}
			return events;
		}
		public SortedDictionary<string, PropertyDescriptor> GetProperties()
		{
			SortedDictionary<string, PropertyDescriptor> props = new SortedDictionary<string, PropertyDescriptor>();
			Type t = WrappedType;
			PropertyInfo[] pifs = t.GetProperties();
			for (int i = 0; i < pifs.Length; i++)
			{
				if (!pifs[i].IsSpecialName)
				{
					int n = 0;
					string key = pifs[i].Name;
					while (props.ContainsKey(key))
					{
						n++;
						key = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}{1}", pifs[i].Name, n);
					}
					props.Add(key, new TypePropertyDescriptor(t, pifs[i].Name, new Attribute[] { }, pifs[i]));
				}
			}
			return props;
		}
		/// <summary>
		/// get a CodeExpression representing T[i]?
		/// </summary>
		/// <param name="methodCompile"></param>
		/// <param name="statements"></param>
		/// <param name="method"></param>
		/// <param name="forValue"></param>
		/// <returns></returns>
		public CodeExpression GetReferenceCode(IMethodCompile methodCompile, CodeStatementCollection statements, MethodPointer method, CodeExpression[] ps, bool forValue)
		{
			return new CodeVariableReferenceExpression(this.CodeName);
		}

		[Browsable(false)]
		public string MenuItemFilePath
		{
			get
			{
				Type type = CollectionItemType.BaseClassType;
				string dir = DesignUtil.GetApplicationDataFolder();
				string sFile = System.IO.Path.Combine(dir, type.Name) + "_collect." + LimnorContextMenuCollection.FILE_EXT_ME;
				while (!System.IO.File.Exists(sFile))
				{
					if (type.Equals(typeof(object)))
						break;
					type = type.BaseType;
					sFile = System.IO.Path.Combine(dir, type.Name) + "_collect." + LimnorContextMenuCollection.FILE_EXT_ME;
				}
				return sFile;
			}
		}
		[Browsable(false)]
		public XmlNode MenuItemNode
		{
			get
			{
				string s = MenuItemFilePath;
				if (System.IO.File.Exists(s))
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(s);
					return doc.DocumentElement;
				}
				return null;
			}
		}
		#endregion
	}
}
