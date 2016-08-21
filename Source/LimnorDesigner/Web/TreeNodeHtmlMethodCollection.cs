/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VSPrj;
using System.Windows.Forms;
using VPL;
using Limnor.WebBuilder;
using System.Reflection;
using Limnor.WebServerBuilder;
using LimnorDesigner.MethodBuilder;
using ProgElements;

namespace LimnorDesigner.Web
{
	class TreeNodeHtmlMethodCollection : TreeNodeObjectCollection
	{
		public TreeNodeHtmlMethodCollection(TreeViewObjectExplorer tv, TreeNodeObject parentNode, HtmlElement_Base objectPointer, UInt32 scopeMethodId)
			: base(tv, parentNode, false, objectPointer, scopeMethodId)
		{
			Text = "Methods inherited";
			Nodes.Add(new HtmlMethodLoader());
		}
		public override void OnShowActionIcon()
		{
			base.OnShowActionIcon();
			this.ImageIndex = TreeViewObjectExplorer.IMG_METHODS_WITHACTS;
			SelectedImageIndex = this.ImageIndex;
		}
		protected override void ShowIconNoAction()
		{
			base.OnShowActionIcon();
			this.ImageIndex = TreeViewObjectExplorer.IMG_METHODS;
			SelectedImageIndex = this.ImageIndex;
		}

		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Method; }
		}

		public override EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}

		public override bool CanContain(IObjectIdentity objectPointer)
		{
			return (objectPointer is MethodPointer || objectPointer is MethodParamPointer);
		}

		public override List<TreeNodeLoader> GetLoaderNodes()
		{
			List<TreeNodeLoader> lst = new List<TreeNodeLoader>();
			lst.Add(new HtmlMethodLoader());
			return lst;
		}
	}
	class HtmlMethodLoader : TreeNodeLoader
	{
		public HtmlMethodLoader()
			: base(false)
		{
		}
		public override void LoadNextLevel(TreeViewObjectExplorer tv, TreeNodeObject parentNode)
		{
			if (tv != null && tv.RootId != null && parentNode != null && parentNode.OwnerPointer != null && parentNode.OwnerPointer.RootPointer != null)
			{
				if (tv.Project != null && (tv.Project.ProjectType == EnumProjectType.WebAppPhp || tv.Project.ProjectType == EnumProjectType.WebAppAspx))
				{
					if (tv.RootId.ClassId != parentNode.OwnerPointer.RootPointer.ClassId)
					{
						return;
					}
				}
			}
			IObjectPointer objRef = parentNode.OwnerPointer;
			TreeNodeClassRoot topClass = parentNode.TopLevelRootClassNode;
			bool forPhp = false;
			if (tv.Project != null)
			{
				forPhp = (tv.Project.ProjectType == EnumProjectType.WebAppPhp);
			}
			Dictionary<UInt32, IAction> actions = null;
			if (topClass != null)
			{
				if (!topClass.StaticScope)
				{
					actions = topClass.GetActions();
				}
			}
			else
			{
				TreeNodeClassType rootType = parentNode.TopLevelNode as TreeNodeClassType;
				if (rootType != null)
				{
					actions = rootType.GetActions();
				}
			}
			SortedList<string, TreeNode> newNodes = new SortedList<string, TreeNode>();
			HtmlElement_BodyBase heb = (HtmlElement_BodyBase)parentNode.OwnerIdentity;
			MethodInfo[] mifs = heb.GetType().GetMethods();
			if (mifs != null)
			{
				for (int i = 0; i < mifs.Length; i++)
				{
					if (parentNode.SelectionTarget == EnumObjectSelectType.Object)
					{
						if (mifs[i].ReturnType.Equals(typeof(void)))
						{
							continue;
						}
						ParameterInfo[] ps = mifs[i].GetParameters();
						if (ps != null && ps.Length > 0)
						{
							continue;
						}
					}
					if (VPLUtil.IsNotForProgramming(mifs[i]))
					{
						continue;
					}
					if (!WebClientMemberAttribute.IsClientMethod(mifs[i]) && !WebServerMemberAttribute.IsServerMethod(mifs[i]))
					{
						continue;
					}
					MethodInfoPointer mp = new MethodInfoPointer();
					mp.Owner = objRef;
					mp.SetMethodInfo(mifs[i]);
					int c;
					TreeNodeMethod nodeMethod = new TreeNodeMethod(ForStatic, mp);
					string key = mp.GetMethodSignature(out c);
					TreeNode nodeExist;
					if (newNodes.TryGetValue(key, out nodeExist))
					{
						TreeNodeMethod mnd = (TreeNodeMethod)nodeExist;
						if (mnd.MethodInformation.DeclaringType.Equals(mnd.MethodInformation.ReflectedType))
						{
							key = key + " - " + mifs[i].DeclaringType.Name;
							newNodes.Add(key, nodeMethod);
						}
						else
						{
							if (mifs[i].DeclaringType.Equals(mifs[i].ReflectedType))
							{
								newNodes[key] = nodeMethod;
								key = key + " - " + mnd.MethodInformation.DeclaringType.Name;
								newNodes.Add(key, mnd);
							}
							else
							{
								key = key + " - " + mifs[i].DeclaringType.Name;
								newNodes.Add(key, nodeMethod);
							}
						}
					}
					else
					{
						newNodes.Add(key, nodeMethod);
					}
					//load actions
					if (actions != null)
					{
						bool bHasActions = false;
						foreach (IAction a in actions.Values)
						{
							ActionClass ac = a as ActionClass;
							if (ac != null)
							{
								MethodPointer mp0 = ac.ActionMethod as MethodPointer;
								if (mp0 != null)
								{
									if (mp0.IsSameObjectRef(mp))
									{
										bHasActions = true;
										break;
									}
								}
							}
						}
						if (bHasActions)
						{
							nodeMethod.ShowActionIcon();
						}
					}
				}
				parentNode.AddSortedNodes(newNodes);
			}
		}
	}
}
