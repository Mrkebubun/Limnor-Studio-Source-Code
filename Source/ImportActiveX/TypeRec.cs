/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	ActiveX Import Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Globalization;

namespace PerformerImport
{
	/// <summary>
	/// Summary description for TypeRec.
	/// </summary>
	public class TypeRec
	{
		//from reg
		public string GUID = "";
		public short MainVer = 0;
		public short MinVer = 0;
		public int LCID = 0;
		protected TLI.TypeLibInfoClass typeInfo = null;
		public TypeRec()
		{
		}
		public override string ToString()
		{
			if (typeInfo == null)
				return GUID;
			return typeInfo.Name + " Version " + typeInfo.MajorVersion.ToString() + "." + typeInfo.MinorVersion.ToString() + " " + typeInfo.get_HelpString(LCID);
		}
		public void LoadInfo()
		{
			try
			{
				typeInfo = new TLI.TypeLibInfoClass();
			}
			catch (Exception e)
			{
				throw new ExceptionTLI(e);
			}
			try
			{

				typeInfo.LoadRegTypeLib(GUID, MainVer, MinVer, LCID);
			}
			catch (Exception e)
			{
				throw new LoadActiveXInfoException(string.Format(CultureInfo.InvariantCulture, "Error calling TypeLibInfoClass.LoadRegTypeLib to get ActiveX information [{0}, version:{1}.{2}, LCID:{3}]", GUID, MainVer, MinVer, LCID), e);
			}
		}
		public string File
		{
			get
			{
				if (typeInfo == null)
					return "";
				return typeInfo.ContainingFile;
			}
		}
		public string Name
		{
			get
			{
				if (typeInfo == null)
					return "";
				return typeInfo.Name;
			}
		}
		public int ClassCount
		{
			get
			{
				if (typeInfo == null)
					return 0;
				if (typeInfo.CoClasses == null)
					return 0;
				return typeInfo.CoClasses.Count;
			}
		}
		public TLI.CoClassInfo ClassInfo(short i)
		{
			return typeInfo.CoClasses[i];
		}
		public ClassRec ClassRecord(short i)
		{
			return new ClassRec(typeInfo.CoClasses[(short)(i + 1)]);
		}
	}
	public class ExceptionTLI : Exception
	{
		public ExceptionTLI(Exception e)
			: base(string.Format(CultureInfo.InvariantCulture, "Cannot execute TLI:{0}", (e == null) ? "Unknown" : e.Message), e)
		{

		}
	}
	public class ClassRec
	{
		protected TLI.CoClassInfo clsInfo = null;
		public ClassRec(TLI.CoClassInfo info)
		{
			clsInfo = info;
		}
		public override string ToString()
		{
			if (clsInfo == null)
				return "";
			return clsInfo.Name;
		}
		public int MemberCount
		{
			get
			{
				if (clsInfo == null)
					return 0;
				return clsInfo.Interfaces.Count;
			}
		}
		public InterfaceRecord GetInfo(short i)
		{
			if (clsInfo == null)
				return null;
			return new InterfaceRecord(clsInfo.Interfaces[i]);
		}
		public static string ValueTypeString(string name, System.Type tp, ref System.Type[] typesUsed)
		{
			string s = "";
			if (tp.IsValueType)
				s = name + ":" + tp.ToString() + "\r\n";
			else
			{
				int n;
				bool b = false;
				if (typesUsed == null)
				{
					n = 0;
					typesUsed = new System.Type[1];
				}
				else
				{
					n = typesUsed.Length;
					System.Type[] a = new Type[n + 1];
					for (int i = 0; i < n; i++)
					{
						if (typesUsed[i].Equals(tp))
							b = true;
						else if (tp.IsSubclassOf(typesUsed[i]))
							b = true;
						if (b)
							break;
						a[i] = typesUsed[i];
					}
					if (!b)
						typesUsed = a;
				}
				if (!b)
				{
					typesUsed[n] = tp;
					System.Reflection.MemberInfo[] pifs = tp.GetMembers();
					if (pifs == null)
					{
						s = name + ":" + tp.ToString() + "\r\n";
					}
					else
					{
						if (pifs.Length > 0)
						{

							for (int i = 0; i < pifs.Length; i++)
							{
								b = false;
								if (pifs[i].MemberType == System.Reflection.MemberTypes.Field)
								{
									System.Reflection.FieldInfo fif = pifs[i] as System.Reflection.FieldInfo;
									if (fif != null)
									{
										s += ValueTypeString(name + "." + pifs[i].Name, fif.FieldType, ref typesUsed) + "\r\n";
										b = true;
									}
								}
								else if (pifs[i].MemberType == System.Reflection.MemberTypes.Property)
								{
									System.Reflection.PropertyInfo pif = pifs[i] as System.Reflection.PropertyInfo;
									if (pif != null)
									{
										s += ValueTypeString(name + "." + pifs[i].Name, pif.PropertyType, ref typesUsed) + "\r\n";
										b = true;
									}
								}
							}
						}
						else
						{
							s = name + ":" + tp.ToString() + "\r\n";
						}
					}
				}
			}
			return s;
		}
	}
	public class InterfaceRecord
	{
		TLI.InterfaceInfo interafceInfo = null;
		public InterfaceRecord(TLI.InterfaceInfo info)
		{
			interafceInfo = info;
		}
		public override string ToString()
		{
			if (interafceInfo == null)
				return "";
			return interafceInfo.Name;
		}
	}
}
