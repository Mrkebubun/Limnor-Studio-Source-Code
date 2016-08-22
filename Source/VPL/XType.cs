/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Xml;

namespace VPL
{
	/// <summary>
	/// a wrapper for a type in a DLL located in the project folder, not IDE folder.
	/// 1. IDE's Toolbox cannot use types in DLL's not in IDE folder
	/// 2. Some types, such as WCF service proxy, cannot be used to created instances in design time by IDE.
	///     WCF service proxy needs app.config file in the application 
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RuntimeInstance : IComponent, ICustomTypeDescriptor, ICustomMethodDescriptor, IXmlNodeSerializable
	{
		#region fields and constructors
		private Type _instanceType;
		private string _projectFolder;
		private string _filename;
		private string _typeName;
		private Dictionary<string, object> _propertyValues;
		public RuntimeInstance()
		{
		}
		public RuntimeInstance(IContainer c)
		{
			if (c != null)
			{
				c.Add(this);
			}
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public object this[string propertyName]
		{
			get
			{
				if (_propertyValues == null)
				{
					return null;
				}
				object v;
				if (_propertyValues.TryGetValue(propertyName, out v))
				{
					return v;
				}
				return null;
			}
			set
			{
				if (_propertyValues == null)
				{
					_propertyValues = new Dictionary<string, object>();
				}
				if (_propertyValues.ContainsKey(propertyName))
				{
					_propertyValues[propertyName] = value;
				}
				else
				{
					_propertyValues.Add(propertyName, value);
				}
			}
		}
		[Browsable(false)]
		public Type InstanceType
		{
			get
			{
				return _instanceType;
			}
		}
		[Browsable(false)]
		public string Filename
		{
			get
			{
				return _filename;
			}
			set
			{
				_filename = value;
			}
		}
		[Browsable(false)]
		public string Typename
		{
			get
			{
				return _typeName;
			}
			set
			{
				_typeName = value;
			}
		}
		#endregion
		#region private methods
		private void onSetType()
		{
			if (_instanceType != null)
			{
				MethodInfo mif = _instanceType.GetMethod("SetConfigFilePath", BindingFlags.Static | BindingFlags.Public);
				if (mif != null)
				{
					string p = Path.Combine(_projectFolder, string.Format(CultureInfo.InvariantCulture, "{0}.config", Path.GetFileNameWithoutExtension(_instanceType.Assembly.Location)));
					mif.Invoke(null, new object[] { p });
				}
			}
		}
		#endregion
		#region Methods
		[Browsable(false)]
		public void SetType(Type type, string projectFolder)
		{
			_projectFolder = projectFolder;
			_instanceType = type;
			_filename = Path.GetFileName(_instanceType.Assembly.Location);
			_typeName = _instanceType.AssemblyQualifiedName;
			onSetType();
		}
		[Browsable(false)]
		public void LoadType(string projectFolder)
		{
			_projectFolder = projectFolder;
			if (_instanceType == null)
			{
				if (!string.IsNullOrEmpty(_typeName))
				{
					_instanceType = Type.GetType(_typeName);
					if (_instanceType == null)
					{
						if (!string.IsNullOrEmpty(_filename))
						{
							bool bFound = File.Exists(_filename);
							if (!bFound)
							{
								if (!string.IsNullOrEmpty(projectFolder))
								{
									if (Directory.Exists(projectFolder))
									{
										string f = Path.Combine(projectFolder, Path.GetFileName(_filename));
										if (File.Exists(f))
										{
											_filename = f;
											bFound = true;
										}
									}
								}

							}
							if (bFound)
							{
								Assembly a = Assembly.LoadFile(_filename);
								if (a != null)
								{
									_instanceType = a.GetType(_typeName);
								}
							}
						}
					}
				}
			}
			onSetType();
		}
		[Browsable(false)]
		public bool IsDesignOnlyProperty(string name)
		{
			if (string.CompareOrdinal(name, "Filename") == 0)
			{
				return true;
			}
			if (string.CompareOrdinal(name, "Typename") == 0)
			{
				return true;
			}
			return false;
		}
		#endregion
		#region IComponent Members
		[Browsable(false)]
		public event EventHandler Disposed;

		ISite _site;
		[ReadOnly(true)]
		[Browsable(false)]
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
		[Browsable(false)]
		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion
		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public AttributeCollection GetAttributes()
		{
			if (_instanceType != null)
			{
				return TypeDescriptor.GetAttributes(_instanceType);
			}
			return null;
		}
		[Browsable(false)]
		public string GetClassName()
		{
			if (_instanceType != null)
			{
				return TypeDescriptor.GetClassName(_instanceType);
			}
			return null;
		}
		[Browsable(false)]
		public string GetComponentName()
		{
			if (_instanceType != null)
			{
				return TypeDescriptor.GetClassName(_instanceType);
			}
			return null;
		}
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			if (_instanceType != null)
			{
				return TypeDescriptor.GetConverter(_instanceType);
			}
			return null;
		}
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			if (_instanceType != null)
			{
				return TypeDescriptor.GetDefaultEvent(_instanceType);
			}
			return null;
		}
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			if (_instanceType != null)
			{
				return TypeDescriptor.GetDefaultProperty(_instanceType);
			}
			return null;
		}
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			if (_instanceType != null)
			{
				return TypeDescriptor.GetEditor(_instanceType, editorBaseType);
			}
			return null;
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			if (_instanceType != null)
			{
				return TypeDescriptor.GetEvents(_instanceType, attributes);
			}
			return null;
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			if (_instanceType != null)
			{
				return TypeDescriptor.GetEvents(_instanceType);
			}
			return null;
		}

		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<PropertyDescriptor> ps2 = new List<PropertyDescriptor>();
			PropertyDescriptorCollection ps1 = TypeDescriptor.GetProperties(this, attributes, true);
			foreach (PropertyDescriptor p in ps1)
			{
				if (IsDesignOnlyProperty(p.Name))
				{
					ps2.Add(p);
				}
				if (string.CompareOrdinal(p.Name, "Name") == 0)
				{
					ps2.Add(p);
				}
			}
			if (_instanceType != null)
			{
				PropertyDescriptorCollection ps0 = TypeDescriptor.GetProperties(_instanceType, attributes);

				foreach (PropertyDescriptor p in ps0)
				{
					if (!p.IsReadOnly)
					{
						ps2.Add(new PropertyDescriptorInstanceValue(p.Name, attributes, p.PropertyType, p.Attributes, this));
					}
				}

			}
			return new PropertyDescriptorCollection(ps2.ToArray());
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			if (_instanceType != null)
			{
				return GetProperties(new Attribute[] { });
			}
			return null;
		}
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
		#region property descriptor
		class PropertyDescriptorInstanceValue : PropertyDescriptor
		{
			private RuntimeInstance _owner;
			private Type _type;
			public PropertyDescriptorInstanceValue(string name, Attribute[] attrs, Type propertyType, AttributeCollection propertyAttributes, RuntimeInstance owner)
				: base(name, attrs)
			{
				_owner = owner;
				_type = propertyType;
				int n1 = 0;
				if (attrs != null)
				{
					n1 = attrs.Length;
				}
				int n2 = 0;
				if (propertyAttributes != null)
				{
					n2 = propertyAttributes.Count;
				}
				Attribute[] a = new Attribute[n1 + n2];
				for (int i = 0; i < n1; i++)
				{
					a[i] = attrs[i];
				}
				for (int i = 0; i < n2; i++)
				{
					a[i + n1] = propertyAttributes[i];
				}
				this.AttributeArray = a;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(RuntimeInstance); }
			}

			public override object GetValue(object component)
			{
				return _owner[Name];
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return _type; }
			}

			public override void ResetValue(object component)
			{
				_owner[Name] = VPLUtil.GetDefaultValue(_type);
			}

			public override void SetValue(object component, object value)
			{
				_owner[Name] = value;
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}
		}
		#endregion
		#region ICustomMethodDescriptor Members
		[Browsable(false)]
		public MethodInfo GetMethod(string name, Type[] parameterTypes, Type returnType)
		{
			if (_instanceType != null)
			{
				MethodInfo mif = null;
				try
				{
					mif = _instanceType.GetMethod(name, parameterTypes);
					if (mif != null)
					{
						return mif;
					}
				}
				catch
				{
				}

				MethodInfo[] mifs = _instanceType.GetMethods();
				if (mifs != null)
				{
					for (int i = 0; i < mifs.Length; i++)
					{
						if (string.CompareOrdinal(mifs[i].Name, name) == 0)
						{
							mif = mifs[i];
							if (parameterTypes == null)
							{
								return mif;
							}
							else
							{
								ParameterInfo[] ps = mif.GetParameters();
								if (ps != null && ps.Length == parameterTypes.Length)
								{
									bool bMatch = true;
									for (int j = 0; j < ps.Length; j++)
									{
										if (!ps[j].ParameterType.Equals(parameterTypes[j]))
										{
											bMatch = false;
											break;
										}
									}
									if (bMatch)
									{
										if (returnType == null)
										{
											return mif;
										}
										else
										{
											if (returnType.Equals(mif.ReturnType))
											{
												return mif;
											}
										}
									}
								}
							}
						}
					}
				}
				return mif;
			}
			return null;
		}
		[Browsable(false)]
		public MethodInfo[] GetMethods(EnumReflectionMemberInfoSelectScope scope)
		{
			if (_instanceType == null)
			{
				return new MethodInfo[] { };
			}
			MethodInfo[] ret;
			if (scope == EnumReflectionMemberInfoSelectScope.Both)
			{
				ret = _instanceType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
			}
			else if (scope == EnumReflectionMemberInfoSelectScope.InstanceOnly)
			{
				ret = _instanceType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
			}
			else if (scope == EnumReflectionMemberInfoSelectScope.StaticOnly)
			{
				ret = _instanceType.GetMethods(BindingFlags.Static | BindingFlags.Public);
			}
			else
			{
				ret = _instanceType.GetMethods();
			}
			if (ret == null)
			{
				return new MethodInfo[] { };
			}
			else
			{
				List<MethodInfo> l = new List<MethodInfo>();
				for (int i = 0; i < ret.Length; i++)
				{
					if (!ret[i].IsSpecialName)
					{
						l.Add(ret[i]);
					}
				}
				return l.ToArray();
			}
		}

		#endregion
		#region IXmlNodeSerializable Members
		const string XML_Data = "Data";
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_instanceType = serializer.ReadValue<Type>(node, null);
			if (_instanceType == null)
			{
				string msg = node.InnerXml;
				throw new VPLException("Loading type failed from xml [{0}]. If this project was developed in another Limnor Studio IDE then it could be that some support files or dependent assemblies are not copied from the original IDE", msg);
			}
			SetType(_instanceType, serializer.ProjectFolder);
			XmlNode dataNode = node.SelectSingleNode(XML_Data);
			if (dataNode != null)
			{
				PropertyDescriptorCollection ps = GetProperties();
				foreach (XmlNode nd in dataNode.ChildNodes)
				{
					PropertyDescriptor p = ps[nd.Name];
					if (p != null)
					{
						object obj = serializer.ReadObject(nd, this);//.ReadObjectFromXmlNode(nd, obj, p.PropertyType, this);
						p.SetValue(this, obj);
					}
				}
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			serializer.WriteValue(node, _instanceType, null);
			XmlNode dataNode = node.OwnerDocument.CreateElement(XML_Data);
			node.AppendChild(dataNode);
			PropertyDescriptorCollection ps = GetProperties();
			foreach (PropertyDescriptor p in ps)
			{
				if (string.CompareOrdinal(p.Name, "InstanceType") != 0)
				{
					if (!p.IsReadOnly)
					{
						XmlNode nd = node.OwnerDocument.CreateElement(p.Name);
						dataNode.AppendChild(nd);
						serializer.WriteObjectToNode(nd, p.GetValue(this));
					}
				}
			}
		}

		#endregion
	}
}
