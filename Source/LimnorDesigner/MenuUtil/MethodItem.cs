/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Parser;
using VPL;
using System.Drawing;
using VSPrj;
using System.Xml;
using ProgElements;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Action;

namespace LimnorDesigner.MenuUtil
{
	/// <summary>
	/// for lib-type
	/// </summary>
	public class MethodItem : MenuItemDataMethod
	{
		private Type _type;
		private MethodInfo _val;
		private string _tooltips;
		public MethodItem(string key, IClass owner, MethodInfo mi)
			: base(key, owner)
		{
			if (owner.VariableCustomType != null)
			{
				_type = owner.VariableCustomType.BaseClassType;
			}
			else if (owner.VariableLibType != null)
			{
				_type = owner.VariableLibType;
			}
			else if (owner.VariableWrapperType != null)
			{
				_type = owner.VariableWrapperType.WrappedType;
			}
			else
			{
				throw new DesignerException("Invalid object type creating MethodItem");
			}
			_val = mi;
		}
		private MethodInfoPointer createPointer(MultiPanes viewersHolder, XmlNode rootNode)
		{
			MethodInfoPointer mp;
			if (_val is SubMethodInfo)
			{
				mp = new SubMethodInfoPointer();
			}
			else
			{
				mp = new MethodInfoPointer();
			}
			mp.Owner = Owner;
			mp.MemberName = _val.Name;
			ParameterInfo[] pifs = null;
			IDynamicMethodParameters dmp = Owner.ObjectInstance as IDynamicMethodParameters;
			if (dmp != null)
			{
				pifs = dmp.GetDynamicMethodParameters(_val.Name, null);
			}
			if (pifs == null)
			{
				pifs = _val.GetParameters();
			}
			if (pifs != null && pifs.Length > 0)
			{
				Type[] ts = new Type[pifs.Length];
				for (int i = 0; i < ts.Length; i++)
				{
					ts[i] = pifs[i].ParameterType;
				}
				mp.ParameterTypes = ts;
			}
			mp.SetMethodInfo(_val);
			return mp;
		}
		public override bool ExecuteMenuCommand(LimnorProject project, IClass holder, XmlNode node, MultiPanes pane, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			MethodInfoPointer mp = createPointer(pane, node);
			return DesignUtil.OnCreateAction(holder, mp, scopeMethod, actsHolder, pane, node) != null;
		}
		public override IAction CreateMethodAction(ILimnorDesignPane designPane, IClass holder, IMethod scopeMethod, IActionsHolder actsHolder)
		{
			MethodInfoPointer mp = createPointer(designPane.PaneHolder, designPane.RootXmlNode);
			return DesignUtil.OnCreateAction(holder, mp, scopeMethod, actsHolder, designPane.PaneHolder, designPane.RootXmlNode);
		}
		public Type ObjectType
		{
			get
			{
				return _type;
			}
		}
		public MethodInfo Value
		{
			get
			{
				return _val;
			}
			set
			{
				_val = value;
			}
		}
		public override string Tooltips
		{
			get
			{
				if (string.IsNullOrEmpty(_tooltips))
				{
					string desc, ret;
					Dictionary<string, string> paramsDesc = PMEXmlParser.GetMethodDescription(VPLUtil.GetObjectType(ObjectType), _val, out desc, out ret);
					if (paramsDesc.Count > 0)
					{
						StringBuilder sb = new StringBuilder();
						if (paramsDesc != null)
						{
							bool bFirst = true;
							foreach (KeyValuePair<string, string> kv in paramsDesc)
							{
								if (bFirst)
								{
									bFirst = false;
								}
								else
								{
									sb.Append(",");
								}
								sb.Append(kv.Key);
								sb.Append(":");
								sb.Append(kv.Value);
							}
						}
						_tooltips = string.Format(System.Globalization.CultureInfo.InvariantCulture,
							"{0}({1}) Returns:{2}", desc, sb.ToString(), ret);
					}
					else
					{
						if (_val.ReturnType.Equals(typeof(void)))
						{
							_tooltips = desc;
						}
						else
						{
							_tooltips = string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"{0}. Returns:{1}", desc, ret);
						}
					}
				}
				return _tooltips;
			}
		}
	}
}
