/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using ProgElements;

namespace LimnorDesigner.Action
{
	public class ParameterValueCollection : List<ParameterValue>
	{
		public ParameterValueCollection()
		{
		}
		public void SetParameterValueChangeEvent(EventHandler h)
		{
			foreach (ParameterValue pv in this)
			{
				if (pv != null)
				{
					pv.SetParameterValueChangeEvent(h);
				}
			}
		}
		static public void ValidateParameterValues(ParameterValueCollection parameters, List<ParameterClass> ps, IActionMethodPointer owner)
		{
			List<ParameterValue> pvs = new List<ParameterValue>();
			for (int i = 0; i < ps.Count; i++)
			{
				bool bFound = false;
				foreach (ParameterValue pv in parameters)
				{
					if (string.Compare(ps[i].Name, pv.Name, StringComparison.Ordinal) == 0)
					{
						bFound = true;
						pv.SetDataType(ps[i]);
						pvs.Add(pv);
						break;
					}
				}
				if (!bFound)
				{
					ParameterValue p = owner.CreateDefaultParameterValue(i);
					pvs.Add(p);
				}
			}
			parameters.Clear();
			parameters.AddRange(pvs);
		}
		static public void ValidateParameterValues(ParameterValueCollection parameters, Type[] ps, IActionMethodPointer owner)
		{
			List<ParameterValue> pvs = new List<ParameterValue>();
			if (ps != null)
			{
				for (int i = 0; i < ps.Length; i++)
				{
					bool bFound = false;
					if (i < parameters.Count && parameters[i] != null)
					{
						if (parameters[i].ParameterLibType != null)
						{
							if (parameters[i].ValueType == EnumValueType.ConstantValue && ps[i].IsAssignableFrom(parameters[i].ParameterLibType))
							{
								bFound = true;
								parameters[i].SetDataType(ps[i]);
								pvs.Add(parameters[i]);
							}
							else
							{
								bFound = true;
								pvs.Add(parameters[i]);
							}
						}
					}
					if (!bFound)
					{
						ParameterValue p = owner.CreateDefaultParameterValue(i);
						if (p != null)
						{
							pvs.Add(p);
						}
					}
				}
			}
			parameters.Clear();
			parameters.AddRange(pvs);
		}
		static public void ValidateParameterValues(ParameterValueCollection parameters, IList<IParameter> ps, IActionMethodPointer owner)
		{
			List<ParameterValue> pvs = new List<ParameterValue>();
			for (int i = 0; i < ps.Count; i++)
			{
				bool bFound = false;
				foreach (ParameterValue pv in parameters)
				{
					if (string.Compare(ps[i].Name, pv.Name, StringComparison.Ordinal) == 0)
					{
						bFound = true;
						DataTypePointer dp = ps[i] as DataTypePointer;
						if (dp != null)
						{
							pv.SetDataType(dp);
						}
						else
						{
							pv.SetDataType(ps[i].ParameterLibType);
						}
						pvs.Add(pv);
						break;
					}
				}
				if (!bFound)
				{
					ParameterValue p = owner.CreateDefaultParameterValue(i);
					pvs.Add(p);
				}
			}
			parameters.Clear();
			parameters.AddRange(pvs);
		}
		public ParameterValue GetParameterValue(string name)
		{
			foreach (ParameterValue pv in this)
			{
				if (pv != null)
				{
					if (string.Compare(pv.Name, name, StringComparison.Ordinal) == 0)
					{
						return pv;
					}
				}
			}
			return null;
		}
		public void SetParameterValue(string name, object value)
		{
			foreach (ParameterValue pv in this)
			{
				if (pv != null)
				{
					if (string.Compare(pv.Name, name, StringComparison.Ordinal) == 0)
					{
						pv.SetValue(value);
						break;
					}
				}
			}
		}
		#region ICloneable Members

		public object Clone(IActionContext act)
		{
			ParameterValueCollection obj = new ParameterValueCollection();
			foreach (ParameterValue pv in this)
			{
				obj.Add(pv.Clone(act));
			}
			return obj;
		}

		#endregion
	}
}
