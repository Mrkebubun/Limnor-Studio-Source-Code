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
using ProgElements;
using Parser;
using VPL;

namespace LimnorDesigner.MethodBuilder
{
	public class TreeNodeConstructor : TreeNodeObject
	{
		private Dictionary<string, string> _parameterToolTips;
		public TreeNodeConstructor(ConstructorPointer objectPointer)
			: base(false, objectPointer)
		{
			Text = objectPointer.MethodSignature;
			ImageIndex = TreeViewObjectExplorer.IMG_Constructor;
			SelectedImageIndex = ImageIndex;
			//
			if (objectPointer.ConstructInfo != null)
			{
				ParameterInfo[] pifs = objectPointer.ConstructInfo.GetParameters();
				if (pifs != null && pifs.Length > 0)
				{
					for (int i = 0; i < pifs.Length; i++)
					{
						MethodParamPointer p = new MethodParamPointer();
						p.Owner = objectPointer;
						p.MemberName = pifs[i].Name;
						TreeNodeMethodParameter tmp = new TreeNodeMethodParameter(false, p);
						Nodes.Add(tmp);
					}
				}
			}
		}
		public override EnumActionMethodType ActionMethodType { get { return EnumActionMethodType.Instance; } }
		public override string Tooltips
		{
			get
			{
				string methodDesc = "Declare the variable but not create it at this time. The variable is not instantiated";
				ConstructorPointer methodPointer = (ConstructorPointer)this.OwnerPointer;
				if (methodPointer.MethodDef != null)
				{
					_parameterToolTips = PMEXmlParser.GetConstructorDescription(methodPointer.MethodDef.DeclaringType, methodPointer.ConstructInfo, out methodDesc);
					if (_parameterToolTips.Count > 0)
					{
						for (int i = 0; i < this.Nodes.Count; i++)
						{
							TreeNodeMethodParameter tmp = this.Nodes[i] as TreeNodeMethodParameter;
							if (tmp != null)
							{
								MethodParamPointer mpp = tmp.OwnerPointer as MethodParamPointer;
								if (!string.IsNullOrEmpty(mpp.MemberName))
								{
									if (_parameterToolTips.ContainsKey(mpp.MemberName))
									{
										tmp.SetToolTips(_parameterToolTips[mpp.MemberName]);
									}
								}
							}
						}
					}
				}
				return methodDesc;
			}
		}
		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Unknown; }
		}

		public override EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}

		public override bool CanContain(IObjectIdentity objectPointer)
		{
			return (objectPointer is ConstructorPointer);
		}

		public override List<TreeNodeLoader> GetLoaderNodes()
		{
			return null;
		}
		public override TreeNodeObject LocateObjectNode(Stack<IObjectPointer> ownerStack)
		{
			if (ownerStack.Count > 0)
			{
				IObjectPointer o = ownerStack.Pop();
				MethodParamPointer mpp = o as MethodParamPointer;
				if (mpp == null)
				{
					throw new DesignerException("TreeNodeConstructor.LocateObjectNode:{0} is not a MethodParamPointer", o.GetType());
				}
				for (int i = 0; i < this.Nodes.Count; i++)
				{
					TreeNodeMethodParameter tmp = Nodes[i] as TreeNodeMethodParameter;
					if (tmp != null)
					{
						if (tmp.OwnerPointer.IsSameObjectRef(mpp))
						{
							if (ownerStack.Count == 0)
							{
								return tmp;
							}
							else
							{
								return tmp.LocateObjectNode(ownerStack);
							}
						}
					}
				}
			}
			return null;
		}
	}
}
