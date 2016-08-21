/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace LimnorDatabase
{
	public class DropData : ICloneable
	{
		public DropType dropType = null;
		public object data = null; //string or object
		public DropData()
		{
		}
		public bool DataReady()
		{
			if (dropType != null)
			{
				if (dropType.droptype == enumDroptype.OBJECT)
				{
					bool bTypeOK = false;
					if (data != null)
					{
						DropTypeObject dpp = dropType as DropTypeObject;
						if (dpp != null)
						{
							if (dpp.ObjectType.IsAssignableFrom(data.GetType()))
								bTypeOK = true;
						}
					}
					return bTypeOK;
				}
				else
				{
					int i;
					string[] ss = data as string[];
					if (ss != null)
					{
						if (ss.Length > 0)
						{
							if (dropType.droptype == enumDroptype.FILE || dropType.droptype == enumDroptype.FILES)
							{
								for (i = 0; i < ss.Length; i++)
								{
									if (System.IO.File.Exists(ss[i]))
										return true;
								}
								return false;
							}
							else if (dropType.droptype == enumDroptype.FOLDER || dropType.droptype == enumDroptype.FOLDERS)
							{
								for (i = 0; i < ss.Length; i++)
								{
									if (System.IO.Directory.Exists(ss[i]))
										return true;
								}
								return false;
							}
						}
						return true;
					}
				}
				return false;
			}
			return false;
		}
		public object GetValue()
		{
			if (dropType != null)
			{
				if (dropType.droptype == enumDroptype.OBJECT)
				{
					bool bTypeOK = false;
					if (data != null)
					{
						DropTypeObject dpp = dropType as DropTypeObject;
						if (dpp != null)
						{
							if (dpp.ObjectType.IsAssignableFrom(data.GetType()))
								bTypeOK = true;
						}
					}
					if (bTypeOK)
					{
						return data;
					}
				}
				else
				{
					int i;
					string[] ss = data as string[];
					if (ss != null)
					{
						if (ss.Length > 0)
						{
							if (dropType.droptype == enumDroptype.FILE)
							{
								for (i = 0; i < ss.Length; i++)
								{
									if (System.IO.File.Exists(ss[i]))
										return ss[i];
								}
							}
							else if (dropType.droptype == enumDroptype.FOLDER)
							{
								for (i = 0; i < ss.Length; i++)
								{
									if (System.IO.Directory.Exists(ss[i]))
										return ss[i];
								}
							}
							else if (dropType.droptype == enumDroptype.FILES)
							{
								int nCount = 0;
								bool[] include = new bool[ss.Length];
								for (i = 0; i < ss.Length; i++)
								{
									include[i] = System.IO.File.Exists(ss[i]);
									if (include[i])
										nCount++;
								}
								string[] a = new string[nCount];
								int j = 0;
								for (i = 0; i < ss.Length; i++)
								{
									if (include[i])
									{
										a[j] = ss[i];
										j++;
									}
								}
								return a;
							}
							else if (dropType.droptype == enumDroptype.FOLDERS)
							{
								int nCount = 0;
								bool[] include = new bool[ss.Length];
								for (i = 0; i < ss.Length; i++)
								{
									include[i] = System.IO.Directory.Exists(ss[i]);
									if (include[i])
										nCount++;
								}
								string[] a = new string[nCount];
								int j = 0;
								for (i = 0; i < ss.Length; i++)
								{
									if (include[i])
									{
										a[j] = ss[i];
										j++;
									}
								}
								return a;
							}
						}
						return ss;
					}
				}
				return data;
			}
			return null;
		}
		#region ICloneable Members
		public object Clone()
		{
			DropData obj = new DropData();
			if (dropType != null)
				obj.dropType = (DropType)dropType.Clone();
			ICloneable v = data as ICloneable;
			if (v != null)
				obj.data = ((ICloneable)data).Clone();
			else
				obj.data = data;
			return obj;
		}
		#endregion
		public override string ToString()
		{
			if (data != null)
				return data.ToString();
			if (dropType != null)
				return dropType.ToString();
			return "";
		}
	}
}
