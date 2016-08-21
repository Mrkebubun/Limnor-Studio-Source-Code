/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;

namespace LimnorDesigner
{
	public class TreeNodeGac : TreeNodeObject
	{
		public TreeNodeGac(bool isForStatic)
			: base(isForStatic, null)
		{
			init(isForStatic);
		}
		private void init(bool isForStatic)
		{
			Text = "Global Assembly Cache";
			ImageIndex = TreeViewObjectExplorer.IMG_FILES;
			SelectedImageIndex = ImageIndex;
			this.Nodes.Add(new CLoader(isForStatic));
		}
		class CLoader : TreeNodeLoader
		{
			public CLoader(bool isForStatic)
				: base(isForStatic)
			{
			}

			public override void LoadNextLevel(TreeViewObjectExplorer tv, TreeNodeObject parentNode)
			{
				List<string> ss = new List<string>();

				AssemblyCacheEnum ace = new AssemblyCacheEnum(null);
				string s;
				s = ace.GetNextAssembly();
				while (!string.IsNullOrEmpty(s))
				{
					ss.Add(s);
					s = ace.GetNextAssembly();
				}
				ss.Sort();
				for (int i = 0; i < ss.Count; i++)
				{
					parentNode.Nodes.Add(new TreeNodeAssembly(ss[i]));
				}
			}
		}

		public override ProgElements.EnumPointerType NodeType
		{
			get { return ProgElements.EnumPointerType.Unknown; }
		}

		public override ProgElements.EnumObjectDevelopType ObjectDevelopType
		{
			get { return ProgElements.EnumObjectDevelopType.Library; }
		}

		public override bool CanContain(ProgElements.IObjectIdentity objectPointer)
		{
			return false;
		}

		public override List<TreeNodeLoader> GetLoaderNodes()
		{
			List<TreeNodeLoader> l = new List<TreeNodeLoader>();
			l.Add(new CLoader(this.IsStatic));
			return l;
		}
	}
}
