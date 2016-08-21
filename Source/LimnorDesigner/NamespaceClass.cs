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
using System.Reflection;
using System.Collections.ObjectModel;
using ProgElements;
using System.ComponentModel;
using XmlUtility;
using System.Windows.Forms;
using System.IO;
using VPL;

namespace LimnorDesigner
{
	/// <summary>
	/// NamespaceClass
	///     Types
	/// </summary>
	public class NamespaceList : IObjectIdentity
	{
		private Dictionary<string, NamespaceClass> _namespaces = new Dictionary<string, NamespaceClass>();
		public NamespaceList()
		{
		}

		public IOrderedEnumerable<NamespaceClass> GetElementsSorted()
		{
			return _namespaces.Values.OrderBy<NamespaceClass, string>(s => s.ToString());
		}
		public Dictionary<string, NamespaceClass>.Enumerator GetEnumerator()
		{
			return _namespaces.GetEnumerator();
		}
		public void AddNamespace(NamespaceClass nc)
		{
			_namespaces.Add(nc.ToString(), nc);
		}
		public void AddAssembly(Assembly a)
		{
			Type[] tps = VPLUtil.GetExportedTypes(a);
			for (int i = 0; i < tps.Length; i++)
			{
				if (tps[i] != null && tps[i].IsPublic)
				{
					if (string.IsNullOrEmpty(tps[i].Namespace))
					{
						NamespaceClass nc;
						if (!_namespaces.TryGetValue(string.Empty, out nc))
						{
							nc = new NamespaceClass(string.Empty);
							_namespaces.Add(string.Empty, nc);
						}
						nc.AddType(tps[i]);
					}
					else
					{
						NamespaceClass nc;
						if (!_namespaces.TryGetValue(tps[i].Namespace, out nc))
						{
							nc = new NamespaceClass(tps[i].Namespace);
							_namespaces.Add(tps[i].Namespace, nc);
						}
						nc.AddType(tps[i]);
					}
				}
			}
		}

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			return false;
		}

		public IObjectIdentity IdentityOwner
		{
			get { return null; }
		}
		public bool IsStatic
		{
			get { return true; }
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Library; } }
		[Browsable(false)]
		public EnumPointerType PointerType { get { return EnumPointerType.Unknown; } }
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			NamespaceList obj = new NamespaceList();
			Dictionary<string, NamespaceClass>.Enumerator e = GetEnumerator();
			while (e.MoveNext())
			{
				obj.AddNamespace(e.Current.Value);
			}
			return obj;
		}

		#endregion
	}
	/// <summary>
	/// it is an IObjectIdentity so that it can be an owner of TreeNodeObject
	/// </summary>
	public class NamespaceClass : IObjectIdentity
	{
		private string _namespace;
		private List<Type> _typees = new List<Type>();
		public NamespaceClass(string ns)
		{
			_namespace = ns;
		}
		public IOrderedEnumerable<Type> GetTypesSorted()
		{
			return _typees.OrderBy<Type, string>(t => t.Name);
		}
		public ReadOnlyCollection<Type> Types
		{
			get
			{
				return _typees.AsReadOnly();
			}
		}
		public ReadOnlyCollection<string> AssemblyNames
		{
			get
			{
				List<string> lst = new List<string>();
				foreach (Type t in _typees)
				{
					string s = t.Assembly.GetName().Name;
					if (!lst.Contains(s))
					{
						lst.Add(s);
					}
				}
				return lst.AsReadOnly();
			}
		}
		public string Namespace
		{
			get
			{
				return _namespace;
			}
		}
		public void AddType(Type t)
		{
			if (!_typees.Contains(t))
			{
				_typees.Add(t);
			}
		}
		public void AddAssembly(Assembly a)
		{
			if (_typees == null)
			{
				_typees = new List<Type>();
			}
			Type[] tps = a.GetExportedTypes();
			for (int i = 0; i < tps.Length; i++)
			{
				if (tps[i].IsPublic)
				{
					if (tps[i].Namespace == _namespace)
					{
						_typees.Add(tps[i]);
					}
				}
			}
		}
		public override string ToString()
		{
			return _namespace;
		}

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			NamespaceClass nc = objectIdentity as NamespaceClass;
			if (nc != null)
			{
				return nc.ToString() == this.ToString();
			}
			return false;
		}

		public IObjectIdentity IdentityOwner
		{
			get { return null; }
		}
		public bool IsStatic
		{
			get { return true; }
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType { get { return EnumObjectDevelopType.Library; } }
		[Browsable(false)]
		public EnumPointerType PointerType { get { return EnumPointerType.Unknown; } }
		#endregion

		#region ICloneable Members
		public object Clone()
		{
			NamespaceClass obj = new NamespaceClass(this.ToString());
			ReadOnlyCollection<Type> tps = this.Types;
			foreach (Type t in tps)
			{
				obj.AddType(t);
			}
			return obj;
		}

		#endregion
	}
}
