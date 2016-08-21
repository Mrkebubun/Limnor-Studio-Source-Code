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
using LimnorDesigner.MethodBuilder;
using System.ComponentModel;
using System.Drawing.Design;
using LimnorDesigner.MenuUtil;
using System.Xml;
using System.Reflection;
using VPL;
using System.CodeDom;
using MathExp;
using System.Collections.Specialized;

namespace LimnorDesigner
{
	/// <summary>
	/// represent a StringCollection variable.
	/// </summary>
	public class StringCollectionVariable : LocalVariable, IClassWrapper
	{
		#region fields and constructors
		private string _desc;
		public StringCollectionVariable()
		{
		}
		public StringCollectionVariable(StringCollectionPointer typePoint, string name, UInt32 classId, UInt32 memberId)
			: base(typePoint, name, classId, memberId)
		{
		}
		#endregion
		#region Methods
		public override ComponentIconLocal CreateComponentIcon(ILimnorDesigner designer, MethodClass method)
		{
			return new ComponentIconLocal(designer, this, method);
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public Type WrappedType
		{
			get
			{
				return typeof(StringCollection);
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
		#endregion

		#region ICloneable Members

		public override object Clone()
		{
			StringCollectionVariable v = (StringCollectionVariable)base.Clone();
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
				return new ListForEachMethodInfo(typeof(String), MemberId.ToString("x"));
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
			ListForEachMethodInfo af = new ListForEachMethodInfo(typeof(String), MemberId.ToString("x"));
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
					props.Add(pifs[i].Name, new TypePropertyDescriptor(t, pifs[i].Name, new Attribute[] { }, pifs[i]));
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
			if (string.Compare(method.MethodName, "Get", StringComparison.Ordinal) == 0)
			{
				CodeExpression ce = new CodeArrayIndexerExpression(GetReferenceCode(methodCompile, statements, forValue), ps);
				return ce;
			}
			else if (string.Compare(method.MethodName, "Set", StringComparison.Ordinal) == 0)
			{
				CodeExpression ce = new CodeArrayIndexerExpression(GetReferenceCode(methodCompile, statements, forValue), ps[0]);
				return ce;
			}
			return null;
		}
		[Browsable(false)]
		public string MenuItemFilePath
		{
			get
			{
				Type type = typeof(StringCollection);
				string dir = DesignUtil.GetApplicationDataFolder();
				string sFile = System.IO.Path.Combine(dir, type.Name) + "." + LimnorContextMenuCollection.FILE_EXT_ME;
				while (!System.IO.File.Exists(sFile))
				{
					if (type.Equals(typeof(object)))
						break;
					type = type.BaseType;
					sFile = System.IO.Path.Combine(dir, type.Name) + "." + LimnorContextMenuCollection.FILE_EXT_ME;
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
