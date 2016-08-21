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
using System.Drawing;
using Limnor.WebBuilder;

namespace LimnorDesigner
{
	/// <summary>
	/// represent an array variable T[].
	/// its type, typeof(T[]), is represented by an ArrayPointer
	/// </summary>
	public class ArrayVariable : LocalVariable, IClassWrapper
	{
		#region fields and constructors
		private string _desc;
		public ArrayVariable()
		{
		}
		public ArrayVariable(ArrayPointer type, string name, UInt32 classId, UInt32 memberId)
			: base(type, name, classId, memberId)
		{
		}
		#endregion
		#region Methods
		public override ComponentIconLocal CreateComponentIcon(ILimnorDesigner designer, MethodClass method)
		{
			return new ComponentIconArrayPointer(designer, this, method);
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		protected override Image IconImage
		{
			get
			{
				return Resources.array;
			}
		}
		[Browsable(false)]
		public Type WrappedType
		{
			get
			{
				string s = ArrayItemType.LibTypePointer.ClassType.AssemblyQualifiedName;
				int n = s.IndexOf(',');
				s = s.Substring(0, n) + "[]" + s.Substring(n);
				return Type.GetType(s);
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
		[Editor(typeof(PropEditorDataType), typeof(UITypeEditor))]
		[Description("Data type for each array item")]
		public DataTypePointer ArrayItemType
		{
			get
			{
				DataTypePointer t = null;
				ArrayPointer ap = ClassType as ArrayPointer;
				if (ap != null)
				{
					t = new DataTypePointer(new TypePointer(ap.ItemBaseType));
				}
				else
				{
					t = base.ClassType;
				}
				return t;
			}
			set
			{
				SetDataType(value);
			}
		}
		#endregion

		#region ICloneable Members

		public override object Clone()
		{
			ArrayVariable v = (ArrayVariable)base.Clone();
			v._desc = _desc;
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
		private MethodInfo searchForMethod(string name, Type[] types)
		{
			MethodInfo mi = null;
			if (_methods != null)
			{
				foreach (KeyValuePair<string, MethodInfo> kv in _methods)
				{
					if (string.CompareOrdinal(name, kv.Value.Name) == 0)
					{
						ParameterInfo[] ps = kv.Value.GetParameters();
						if (types == null || types.Length == 0)
						{
							if (ps == null || ps.Length == 0)
							{
								mi = kv.Value;
								break;
							}
						}
						else
						{
							if (types != null && types.Length == ps.Length)
							{
								mi = kv.Value;
								for (int i = 0; i < ps.Length; i++)
								{
									if (!ps[i].ParameterType.Equals(types[i]))
									{
										mi = null;
										break;
									}
								}
								if (mi != null)
								{
									break;
								}
							}
						}
					}
				}
			}
			return mi;
		}
		public MethodInfo GetMethod(string name, Type[] types)
		{
			MethodInfo mi = searchForMethod(name, types);
			if (mi == null)
			{
				_methods = GetMethods();
			}
			mi = searchForMethod(name, types);
			return mi;
		}
		private SortedDictionary<string, MethodInfo> _methods;
		public SortedDictionary<string, MethodInfo> GetMethods()
		{
			bool bClientOnly = false;
			bool isWebPage = false;
			if (this.RootPointer != null)
			{
				isWebPage = this.RootPointer.IsWebPage;
				if (this.RunAt == EnumWebRunAt.Client)
				{
					bClientOnly = true;
				}
			}
			SortedDictionary<string, MethodInfo> methods = new SortedDictionary<string, MethodInfo>();
			Type t = WrappedType;
			MethodInfo[] mifs = t.GetMethods();
			for (int i = 0; i < mifs.Length; i++)
			{
				if (!mifs[i].IsSpecialName)
				{
					if (bClientOnly)
					{
						bool include = false;
						if (!string.IsNullOrEmpty(mifs[i].Name))
						{
							if (string.CompareOrdinal(mifs[i].Name, "Get") == 0)
							{
								include = true;
							}
							else if (string.CompareOrdinal(mifs[i].Name, "Set") == 0)
							{
								include = true;
							}
						}
						if (!include)
						{
							continue;
						}
					}
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
							if (bClientOnly)
							{
								bool include = false;
								if (!string.IsNullOrEmpty(mifs[k].Name))
								{
									if (string.CompareOrdinal(mifs[k].Name, "Get") == 0)
									{
										include = true;
									}
									else if (string.CompareOrdinal(mifs[k].Name, "Set") == 0)
									{
										include = true;
									}
								}
								if (!include)
								{
									continue;
								}
							}
							string s = MethodPointer.GetMethodSignature(mifs[k]);
							if (!methods.ContainsKey(s))
							{
								methods.Add(s, mifs[k]);
							}
						}
					}
				}
			}
			ArrayForEachMethodInfo af = new ArrayForEachMethodInfo(ArrayItemType.LibTypePointer.ClassType, MemberId.ToString("x"));
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
			bool bClientOnly = false;
			bool isWebPage = false;
			if (this.RootPointer != null)
			{
				isWebPage = this.RootPointer.IsWebPage;
				if (this.RunAt == EnumWebRunAt.Client)
				{
					bClientOnly = true;
				}
			}
			SortedDictionary<string, PropertyDescriptor> props = new SortedDictionary<string, PropertyDescriptor>();
			Type t = WrappedType;
			PropertyInfo[] pifs = t.GetProperties();
			for (int i = 0; i < pifs.Length; i++)
			{
				if (!pifs[i].IsSpecialName)
				{
					if (bClientOnly)
					{
						if (string.CompareOrdinal(pifs[i].Name, "Length") != 0)
						{
							continue;
						}
					}
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
				Type type = ArrayItemType.BaseClassType;
				string dir = DesignUtil.GetApplicationDataFolder();
				string sFile = System.IO.Path.Combine(dir, type.Name) + "_array." + LimnorContextMenuCollection.FILE_EXT_ME;
				while (!System.IO.File.Exists(sFile))
				{
					if (type.Equals(typeof(object)))
						break;
					type = type.BaseType;
					if (type == null)
					{
						break;
					}
					sFile = System.IO.Path.Combine(dir, type.Name) + "_array." + LimnorContextMenuCollection.FILE_EXT_ME;
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
