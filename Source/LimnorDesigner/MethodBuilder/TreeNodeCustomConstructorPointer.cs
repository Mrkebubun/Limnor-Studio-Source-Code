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

namespace LimnorDesigner.MethodBuilder
{
	public class TreeNodeCustomConstructorPointer : TreeNodeObject
	{
		public TreeNodeCustomConstructorPointer(CustomConstructorPointer objectPointer)
			: base(false, objectPointer)
		{
			Text = objectPointer.DisplayName;
			ImageIndex = TreeViewObjectExplorer.IMG_Constructor;
			SelectedImageIndex = ImageIndex;
			//
			List<ParameterClass> plst = objectPointer.Parameters;
			if (plst != null && plst.Count > 0)
			{
				foreach (ParameterClass p in plst)
				{
					TreeNodeCustomMethodParameter tmp = new TreeNodeCustomMethodParameter(false, p);
					Nodes.Add(tmp);
				}
			}
		}
		public override EnumActionMethodType ActionMethodType { get { return EnumActionMethodType.Instance; } }
		public override ProgElements.EnumPointerType NodeType
		{
			get { return ProgElements.EnumPointerType.Method; }
		}

		public override ProgElements.EnumObjectDevelopType ObjectDevelopType
		{
			get { return ProgElements.EnumObjectDevelopType.Custom; }
		}

		public override bool CanContain(ProgElements.IObjectIdentity objectPointer)
		{
			return (objectPointer is CustomConstructorPointer);
		}

		public override List<TreeNodeLoader> GetLoaderNodes()
		{
			return null;
		}
	}
}
