/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using MathExp;
using LimnorDesigner.Action;
using VPL;
using LimnorDesigner.MethodBuilder;

namespace LimnorDesigner
{
	public sealed class MethodEditContext
	{
		public static bool IsWebPage;
		public static bool UseClientExecuterOnly;
		public static bool UseClientPropertyOnly;
		public static bool UseServerExecuterOnly;
		public static bool UseServerPropertyOnly;
		public static bool CheckAction(IAction act, Form caller)
		{
			if (act == null)
			{
				return true;
			}
			if (!act.IsValid || act.ActionMethod == null)
			{
				MessageBox.Show(caller, string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"The action is invalid. [{0}]", act.ActionName), "Select action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			if (IsWebPage)
			{
				DataTypePointer dtp = act.MethodOwner as DataTypePointer;
				if (dtp != null)
				{
					return true;
				}
				TypePointer tp = act.MethodOwner as TypePointer;
				if (tp != null)
				{
					return true;
				}
				MethodInfoPointer mip = act.ActionMethod as MethodInfoPointer;
				if (mip != null)
				{
					if (mip.MethodDef != null)
					{
						object[] vs = mip.MethodDef.GetCustomAttributes(typeof(StandaloneOnlyActionAttribute), true);
						if (vs != null && vs.Length > 0)
						{
							MessageBox.Show(caller, string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"Action [{0}] cannot be used inside a method. Consider assigning it directly to an event.", act.ActionName), "Select action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							return false;
						}
					}
				}
				if (UseClientExecuterOnly)
				{
					if (act.ActionMethod.RunAt == EnumWebRunAt.Server)
					{
						MessageBox.Show(caller, string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"Cannot use action executed by server object. Action:[{0}]", act.ActionName), "Select action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return false;
					}
				}
				if (UseServerExecuterOnly)
				{
					if (act.ActionMethod.RunAt == EnumWebRunAt.Client)
					{
						MessageBox.Show(caller, string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"Cannot use an action executed by client object. [{0}]", act.ActionName), "Select action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return false;
					}
				}
				if (UseClientPropertyOnly)
				{
					IList<ISourceValuePointer> lst0 = act.GetServerProperties(0);
					if (lst0 != null && lst0.Count > 0)
					{
						List<ISourceValuePointer> lst = new List<ISourceValuePointer>();
						foreach (ISourceValuePointer v in lst0)
						{
							if (v.IsWebServerValue())
							{
								lst.Add(v);
							}
						}
						if (lst.Count > 0)
						{
							StringBuilder sb = new StringBuilder(lst[0].ToString());
							for (int i = 1; i < lst.Count; i++)
							{
								sb.Append(",");
								sb.Append(lst[i].ToString());
							}
							MessageBox.Show(caller, string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"Cannot use an action involving server values. Action:[{0}] Server value(s):{1}", act.ActionName, sb.ToString()), "Select action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							return false;
						}
					}
				}
				if (UseServerPropertyOnly)
				{
					IList<ISourceValuePointer> lst0 = act.GetClientProperties(0);
					if (lst0 != null && lst0.Count > 0)
					{
						List<ISourceValuePointer> lst = new List<ISourceValuePointer>();
						foreach (ISourceValuePointer v in lst0)
						{
							if (v.IsWebClientValue())
							{
								lst.Add(v);
							}
						}
						if (lst.Count > 0)
						{
							StringBuilder sb = new StringBuilder(lst[0].ToString());
							for (int i = 1; i < lst.Count; i++)
							{
								sb.Append(",");
								sb.Append(lst[i].ToString());
							}
							MessageBox.Show(caller, string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"Cannot use an action involving client values. Action:[{0}] Client value(s):{1}", act.ActionName, sb.ToString()), "Select action", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							return false;
						}
					}
				}
			}
			return true;
		}
	}
}
