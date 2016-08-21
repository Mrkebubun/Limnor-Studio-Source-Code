/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.Drawing;
using ProgElements;
using System.Windows.Forms;
using MathExp;
using Limnor.WebBuilder;
using WindowsUtility;

namespace LimnorDesigner.Web
{
	class TreeNodeHtmlElement : TreeNodeObject, ITreeNodeObjectSelection
	{
		#region fields and constructors
		static Dictionary<string, int> imgList;
		public TreeNodeHtmlElement(HtmlElement_BodyBase element)
			: base(element)
		{
			ShowText();
			ImageIndex = getTreeIcon();
			SelectedImageIndex = ImageIndex;
			if (!(element is HtmlElementUnknown))
			{
				this.Nodes.Add(new TreeNodeHtmlMemberLoader());
				this.Nodes.Add(new ActionLoader(false));
			}
		}
		#endregion
		#region Properties
		public HtmlElement_Base HtmlElement
		{
			get
			{
				return (HtmlElement_Base)(this.OwnerIdentity);
			}
		}
		#endregion
		#region methods
		private int getTreeIcon()
		{
			HtmlElement_Base heb = HtmlElement;
			return GetHtmlElementIcon(heb);
		}
		public override void OnNodeSelection(ILimnorDesignerLoader loader)
		{
			loader.NotifySelection(HtmlElement);
		}
		public override void ShowText()
		{
			Text = HtmlElement.ToString();
		}
		public void SwitchHtmlElement(HtmlElement_Base element)
		{
			this.ResetObjectPointer(element);
			this.ResetNextLevel(this.TreeView as TreeViewObjectExplorer);
		}
		public override void ResetNextLevel(TreeViewObjectExplorer tv)
		{
			NextLevelLoaded = true;
			base.ResetNextLevel(tv);
		}
		public static int GetHtmlElementIconbyKey(string imgKey)
		{
			int n;
			if (imgList == null)
			{
				imgList = new Dictionary<string, int>();
			}
			if (imgList.TryGetValue(imgKey, out n))
			{
				return n;
			}
			Image img = null;
			if (string.CompareOrdinal(imgKey, "html") == 0)
			{
				img = Resources._html.ToBitmap();
			}
			if (img == null)
			{
				img = Resources._html.ToBitmap();
			}
			n = TreeViewObjectExplorer.ObjectImageList.Images.Add(img, Color.White);
			imgList.Add(imgKey, n);
			return n;
		}
		public static int GetHtmlElementIcon(HtmlElement_Base he)
		{
			string webFolder = he.WebPhysicalFolder;
			int n;
			string imgKey = he.ImageKey;
			if (imgList == null)
			{
				imgList = new Dictionary<string, int>();
			}
			if (imgList.TryGetValue(imgKey, out n))
			{
				return n;
			}
			Image img = he.ImageIcon;
			n = TreeViewObjectExplorer.ObjectImageList.Images.Add(img, Color.White);
			imgList.Add(imgKey, n);
			return n;
		}
		#endregion
		#region TreeNodeObject
		public override EnumPointerType NodeType
		{
			get { return EnumPointerType.Class; }
		}

		public override EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}

		public override bool CanContain(IObjectIdentity objectPointer)
		{
			return false;
		}

		public override List<TreeNodeLoader> GetLoaderNodes()
		{
			List<TreeNodeLoader> l = new List<TreeNodeLoader>();
			if (!(this.HtmlElement is HtmlElementUnknown))
			{
				l.Add(new TreeNodeHtmlMemberLoader());
				l.Add(new ActionLoader(false));
			}
			return l;
		}
		#endregion

		#region ITreeNodeObjectSelection Members

		public virtual void OnNodeAfterSelection(TreeViewEventArgs e)
		{
			HtmlElement_Base he = HtmlElement;
			Guid guid = Guid.Empty;
			if (string.CompareOrdinal(he.tagName, "body") != 0)
			{
				if (he.ElementGuid != Guid.Empty)
				{
					guid = he.ElementGuid;
				}
			}
			if (guid != Guid.Empty)
			{
				TreeViewObjectExplorer tv = this.TreeView as TreeViewObjectExplorer;
				if (tv != null)
				{
					TreeNodeClassRoot r = tv.RootClassNode;
					if (r != null && r.ClassData.DesignerHolder != null)
					{
						r.ClassData.DesignerHolder.OnSelectedHtmlElement(guid, tv);
					}
				}
			}
		}

		#endregion
	}
	class TreeNodeHtmlElementCurrent : TreeNodeHtmlElement
	{
		#region fields and constructors
		public TreeNodeHtmlElementCurrent(HtmlElement_BodyBase element)
			: base(element)
		{
		}
		public override void ShowText()
		{
			Text = string.Format(CultureInfo.InvariantCulture, "Current element - {0}", HtmlElement.ToString());
		}
		#endregion
		private void mi_useIt(object sender, EventArgs e)
		{
			if (!(this.HtmlElement is HtmlElementUnknown) && this.HtmlElement.ElementGuid == Guid.Empty)
			{
				HtmlElement_BodyBase hbb = this.HtmlElement as HtmlElement_BodyBase;
				if (hbb != null)
				{
					TreeViewObjectExplorer tv = this.TreeView as TreeViewObjectExplorer;
					if (tv != null)
					{
						TreeNodeClassRoot ownerNode = tv.DesignerRootNode;
						if (ownerNode != null)
						{
							ClassPointer root = ownerNode.ClassData.RootClassID;
							root.UseHtmlElement(hbb, tv.FindForm());
						}
					}
				}
			}
		}
		public override MenuItem[] GetContextMenuItems(bool readOnly)
		{
			if (!(this.HtmlElement is HtmlElementUnknown) && this.HtmlElement.ElementGuid == Guid.Empty)
			{
				MenuItem[] mis = new MenuItem[1];
				mis[0] = new MenuItemWithBitmap("Use it in programming", mi_useIt, Resources._createEventFireAction.ToBitmap());
				return mis;
			}
			return null;
		}
	}
}
