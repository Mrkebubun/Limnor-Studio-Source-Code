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
using System.ComponentModel;
using VPL;
using System.Windows.Forms;
using MathExp;
using Limnor.WebBuilder;
using Limnor.WebServerBuilder;

namespace LimnorDesigner.Web
{
	class TreeNodeHtmlPropertyCollection : TreeNodeObjectCollection
	{
		public TreeNodeHtmlPropertyCollection(TreeViewObjectExplorer tv, TreeNodeObject parentNode, HtmlElement_Base objectPointer, UInt32 scopeMethodId)
			: base(tv, parentNode, false, objectPointer, scopeMethodId)
		{
			Text = "Properties inherited";
			Nodes.Add(new HtmlPropertyLoader());
		}

		protected override void ShowIconNoAction()
		{
			ImageIndex = TreeViewObjectExplorer.IMG_PROPERTIES;
			SelectedImageIndex = ImageIndex;
		}

		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Property; }
		}

		public override EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}
		public override TreeNodeObject FindObjectNode(IObjectIdentity pointer)
		{
			SetterPointer sp = pointer as SetterPointer;
			if (sp != null)
			{
				LoadNextLevel();
				for (int i = 0; i < this.Nodes.Count; i++)
				{
					TreeNodeObject ret = this.Nodes[i] as TreeNodeObject;
					if (ret != null)
					{
						if (sp.SetProperty.IsSameObjectRef(ret.OwnerPointer))
						{
							return ret;
						}
					}
				}
			}
			TreeNodeObject node = base.FindObjectNode(pointer);
			if (node != null)
			{
				return node;
			}
			return null;
		}
		public override bool CanContain(IObjectIdentity objectPointer)
		{
			return !(objectPointer is ClassPointer);
		}

		public override List<TreeNodeLoader> GetLoaderNodes()
		{
			List<TreeNodeLoader> lst = new List<TreeNodeLoader>();
			lst.Add(new HtmlPropertyLoader());
			return lst;
		}
		public override void OnNodeSelection(ILimnorDesignerLoader loader)
		{
			loader.NotifySelection(this.OwnerIdentity);
		}
		public override void OnShowActionIcon()
		{
			base.OnShowActionIcon();
			this.ImageIndex = TreeViewObjectExplorer.IMG_PROPERTIES_WITHACTIONS;
			SelectedImageIndex = this.ImageIndex;
		}
		public override bool IsPointerNode { get { return false; } }
	}
	class HtmlPropertyLoader : TreeNodeLoader
	{
		public HtmlPropertyLoader()
			: base(false)
		{
		}

		public override void LoadNextLevel(TreeViewObjectExplorer tv, TreeNodeObject parentNode)
		{
			UInt32 scopeId = parentNode.ScopeMethodId;
			SortedList<string, TreeNode> newNodes = new SortedList<string, TreeNode>();
			HtmlElement_BodyBase heb = (HtmlElement_BodyBase)parentNode.OwnerIdentity;
			PropertyDescriptorCollection pifs = TypeDescriptor.GetProperties(heb.GetType());
			TreeNodeClassRoot topClass = parentNode.TopLevelRootClassNode;
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
				if (tv != null)
				{
					if (tv.RootClassNode != null)
					{
						actions = tv.RootClassNode.GetActions();
					}
				}
				if (actions == null)
				{
					TreeNodeClassType rootType = parentNode.TopLevelNode as TreeNodeClassType;
					if (rootType != null)
					{
						actions = rootType.GetActions();
					}
				}
			}
			for (int i = 0; i < pifs.Count; i++)
			{
				if (NotForProgrammingAttribute.IsNotForProgramming(pifs[i]))
				{
					continue;
				}
				if (!WebClientMemberAttribute.IsClientProperty(pifs[i]) && !WebServerMemberAttribute.IsServerProperty(pifs[i]))
				{
					continue;
				}
				TreeNodeProperty nodeProperty;
				PropertyPointer pp;
				pp = new PropertyPointer();
				pp.Owner = new HtmlElementPointer(heb);
				pp.SetPropertyInfo(pifs[i]);
				if (!newNodes.ContainsKey(pp.Name))
				{
					nodeProperty = new TreeNodeProperty(ForStatic, pp);
					try
					{
						newNodes.Add(pp.Name, nodeProperty);
					}
					catch (Exception err)
					{
						MathNode.Log(tv != null ? tv.FindForm() : null, err);
					}
					//load actions
					bool bHasActions = false;
					if (string.CompareOrdinal(pp.Name, "Cursor") == 0)
					{
						bHasActions = false;
					}
					if (actions != null)
					{
						foreach (IAction a in actions.Values)
						{
							if (a != null && a.IsStatic == parentNode.IsStatic)
							{
								if (nodeProperty.IncludeAction(a, tv, scopeId, false))
								{
									bHasActions = true;
									break;
								}
							}
						}
						if (bHasActions)
						{
							nodeProperty.OnShowActionIcon();
						}
					}
				}
			}
			parentNode.AddSortedNodes(newNodes);
		}
	}
}
