/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace LimnorDatabase
{
	public enum enumDroptype { NONE = 0, STRING = 1, FILE, FOLDER, FILES, FOLDERS, OBJECT };
	public class DropType : ICloneable
	{
		public enumDroptype droptype = enumDroptype.NONE;
		public DropType()
		{
		}
		public override string ToString()
		{
			if (droptype == enumDroptype.STRING)
				return "Text";
			if (droptype == enumDroptype.FILES)
				return "Files";
			if (droptype == enumDroptype.FOLDERS)
				return "Folders";
			return droptype.ToString();
		}

		#region ICloneable Members

		public virtual object Clone()
		{
			DropType obj = new DropType();
			obj.droptype = droptype;
			return obj;
		}

		#endregion
	}
	public class DropTypeObject : DropType, ICloneable
	{
		protected System.Type _type = null;
		protected object _obj = null;
		public DropTypeObject()
		{
			droptype = enumDroptype.OBJECT;
		}
		public System.Type ObjectType
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
				_obj = null;
			}
		}
		public object Instance
		{
			get
			{
				if (_obj == null)
				{
					if (_type != null)
					{
						try
						{
							_obj = System.Activator.CreateInstance(_type, true);
						}
						catch
						{
						}
					}
				}
				return _obj;
			}
		}
		public override string ToString()
		{
			IComponent p = _obj as IComponent;
			if (p != null && p.Site != null)
				return p.Site.Name;
			if (_type != null)
				return _type.Name;
			return "{null}";
		}
		#region ICloneable Members

		public override object Clone()
		{
			DropTypeObject obj = new DropTypeObject();
			obj.droptype = droptype;
			obj._type = _type;
			obj._obj = _obj;
			return obj;
		}

		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class ObjectTypes : ICloneable
	{
		protected Type[] pTypes = null;
		public ObjectTypes()
		{
		}
		public int Count
		{
			get
			{
				if (pTypes == null)
					return 0;
				return pTypes.Length;
			}
		}
		public Type this[int Index]
		{
			get
			{
				if (Index >= 0 && Index < Count)
				{
					return pTypes[Index];
				}
				return null;
			}
		}
		public bool AddType(Type ty)
		{
			if (pTypes == null)
			{
				pTypes = new Type[1];
				pTypes[0] = ty;
				return true;
			}
			else
			{
				int i;
				for (i = 0; i < pTypes.Length; i++)
				{
					if (pTypes[i].Equals(ty))
						return false;
				}
				Type[] a = new Type[pTypes.Length + 1];
				for (i = 0; i < pTypes.Length; i++)
				{
					a[i] = pTypes[i];
				}
				a[pTypes.Length] = ty;
				pTypes = a;
				return true;
			}
		}
		public void RemoveType(Type ty)
		{
			if (pTypes != null)
			{
				int i;
				for (i = 0; i < pTypes.Length; i++)
				{
					if (pTypes[i].Equals(ty))
					{
						if (pTypes.Length == 1)
						{
							pTypes = null;
						}
						else
						{
							Type[] a = new Type[pTypes.Length - 1];
							for (int j = 0; j < pTypes.Length; j++)
							{
								if (j < i)
								{
									a[j] = pTypes[j];
								}
								else if (j > i)
								{
									a[j - 1] = pTypes[j];
								}
							}
							pTypes = a;
						}
						return;
					}
				}
			}
		}

		#region ICloneable Members

		public object Clone()
		{
			ObjectTypes obj = new ObjectTypes();
			if (pTypes != null)
			{
				for (int i = 0; i < pTypes.Length; i++)
				{
					obj.AddType(pTypes[i]);
				}
			}
			return obj;
		}

		#endregion
		public override string ToString()
		{
			string s = "";
			if (pTypes != null)
			{
				for (int i = 0; i < pTypes.Length; i++)
				{
					s += pTypes[i].ToString() + ";";
				}
			}
			return s;
		}
	}
	/// <summary>
	/// 
	/// </summary>
	public class DropTypes : ICloneable
	{
		protected enumDroptype[] dropType = null;
		protected ObjectTypes performerTypes = null;
		public DropTypes()
		{
		}
		public bool AddPerformerType(System.Type ty)
		{
			if (performerTypes == null)
				performerTypes = new ObjectTypes();
			return performerTypes.AddType(ty);
		}
		public void RemovePerformerType(System.Type ty)
		{
			if (ty == null)
			{
				performerTypes = null;
				RemoveType(enumDroptype.OBJECT);
			}
			else
			{
				if (performerTypes != null)
					performerTypes.RemoveType(ty);
			}
		}
		public int PerformerCount
		{
			get
			{
				if (performerTypes != null)
					return performerTypes.Count;
				return 0;
			}
		}
		public System.Type PerformerType(int Index)
		{
			if (Index >= 0 && Index < PerformerCount)
				return performerTypes[Index];
			return null;
		}
		public int Count
		{
			get
			{
				if (dropType == null)
					return 0;
				return dropType.Length;
			}
		}
		public enumDroptype[] CloneTypes()
		{
			int n = Count;
			if (n > 0)
			{
				enumDroptype[] ret = new enumDroptype[n];
				for (int i = 0; i < n; i++)
				{
					ret[i] = dropType[i];
				}
				return ret;
			}
			return null;
		}
		public void SetTypes(enumDroptype[] types)
		{
			dropType = types;
		}
		public enumDroptype this[int Index]
		{
			get
			{
				if (Index >= 0 && Index < Count)
				{
					return dropType[Index];
				}
				return enumDroptype.NONE;
			}
		}
		public bool AddType(DropType ty)
		{
			if (dropType == null)
			{
				dropType = new enumDroptype[1];
				dropType[0] = ty.droptype;
				if (ty.droptype == enumDroptype.OBJECT)
				{
					DropTypeObject p = ty as DropTypeObject;
					if (p != null)
					{
						if (performerTypes == null)
							performerTypes = new ObjectTypes();
						performerTypes.AddType(p.ObjectType);
					}
				}
				return true;
			}
			else
			{
				int i;
				for (i = 0; i < dropType.Length; i++)
				{
					if (dropType[i] == ty.droptype)
					{
						if (ty.droptype == enumDroptype.OBJECT)
						{
							DropTypeObject p = ty as DropTypeObject;
							if (p != null)
							{
								return performerTypes.AddType(p.ObjectType);
							}
							return false;
						}
						else
							return false;
					}
				}
				enumDroptype[] a = new enumDroptype[dropType.Length + 1];
				for (i = 0; i < dropType.Length; i++)
				{
					a[i] = dropType[i];
				}
				a[dropType.Length] = ty.droptype;
				dropType = a;
				if (ty.droptype == enumDroptype.OBJECT)
				{
					DropTypeObject p = ty as DropTypeObject;
					if (p != null)
					{
						if (performerTypes == null)
							performerTypes = new ObjectTypes();
						performerTypes.AddType(p.ObjectType);
					}
				}
				return true;
			}
		}
		public void RemoveType(enumDroptype ty)
		{
			if (dropType != null)
			{
				int i;
				for (i = 0; i < dropType.Length; i++)
				{
					if (dropType[i] == ty)
					{
						if (ty == enumDroptype.OBJECT)
						{
							performerTypes = null;
						}
						if (dropType.Length == 1)
						{
							dropType = null;
						}
						else
						{
							enumDroptype[] a = new enumDroptype[dropType.Length - 1];
							for (int j = 0; j < dropType.Length; j++)
							{
								if (j < i)
								{
									a[j] = dropType[j];
								}
								else if (j > i)
								{
									a[j - 1] = dropType[j];
								}
							}
							dropType = a;
						}
						return;
					}
				}
			}
		}

		#region ICloneable Members

		public object Clone()
		{
			DropTypes obj = new DropTypes();
			obj.SetTypes(CloneTypes());
			if (performerTypes != null)
			{
				for (int i = 0; i < performerTypes.Count; i++)
				{
					obj.AddPerformerType(performerTypes[i]);
				}
			}
			return obj;
		}

		#endregion
		public override string ToString()
		{
			string s = "";
			if (dropType != null)
			{
				for (int i = 0; i < dropType.Length; i++)
				{
					if (dropType[i] != enumDroptype.NONE)
					{
						s += dropType[i].ToString() + ";";
					}
				}
			}
			return s;
		}
	}
}
