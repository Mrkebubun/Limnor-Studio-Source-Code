/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using ProgElements;
using System.Runtime.InteropServices;
using VPL;

namespace LimnorDesigner.MethodBuilder
{
	public partial class dlgConstructorParameters : Form
	{
		public IConstructor Ret = null;
		public dlgConstructorParameters()
		{
			InitializeComponent();
			splitContainer1.Panel2Collapsed = true;
			treeViewConstructors.NodeSelected += new EventHandler(treeViewConstructors_TypeSelected);
		}
		public void SetMethod(IMethod m)
		{
			propertyGrid1.ScopeMethod = m;
		}
		void treeViewConstructors_TypeSelected(object sender, EventArgs e)
		{
			TreeNodeObject tn = treeViewConstructors.SelectedNode as TreeNodeObject;
			if (tn != null)
			{
				Ret = tn.OwnerPointer as IConstructor;
				if (Ret != null)
				{
					propertyGrid1.SelectedObject = Ret;
					btOK.Enabled = true;
				}
				else
				{
					btOK.Enabled = false;
				}
			}
			else
			{
				btOK.Enabled = false;
			}
		}
		public bool LoadData(LocalVariable lv)
		{
			ConstructorInfo[] cifs;
			Type t = lv.BaseClassType;
			if (t.IsEnum)
			{
				cifs = null;
			}
			else
			{
				try
				{
					Type tCo = VPLUtil.GetCoClassType(t);
					if (tCo != null)
					{
						cifs = tCo.GetConstructors();
					}
					else
					{
						cifs = t.GetConstructors();
					}
				}
				catch
				{
					cifs = null;
				}
			}
			if (cifs != null && cifs.Length > 0)
			{
				Text = string.Format(Resources.TitleSelectConstructor, t.Name);
				List<TreeNodeConstructor> list = new List<TreeNodeConstructor>();
				for (int i = 0; i < cifs.Length; i++)
				{
					TreeNodeConstructor tn = new TreeNodeConstructor(new ConstructorPointer(cifs[i], lv));
					list.Add(tn);
				}
				if (!t.IsValueType)
				{
					treeViewConstructors.Nodes.Add(new TreeNodeConstructor(new ConstructorPointerNull(lv)));
				}
				IOrderedEnumerable<TreeNodeConstructor> sorted = list.OrderBy<TreeNodeConstructor, string>(r => r.Text);
				foreach (TreeNodeConstructor nc in sorted)
				{
					treeViewConstructors.Nodes.Add(nc);
				}
				if (lv.VariableCustomType != null)
				{
					List<ConstructorClass> clst = lv.VariableCustomType.GetCustomConstructors();
					if (clst != null && clst.Count > 0)
					{
						foreach (ConstructorClass cc in clst)
						{
							TreeNodeCustomConstructorPointer tnc = new TreeNodeCustomConstructorPointer(new CustomConstructorPointer(cc, lv.RootPointer));
							treeViewConstructors.Nodes.Add(tnc);
						}
					}
				}
				return true;
			}
			return false;
		}
		public void LocateConstructor(IConstructor cp)
		{
			for (int i = 0; i < treeViewConstructors.Nodes.Count; i++)
			{
				TreeNodeConstructor tn = treeViewConstructors.Nodes[i] as TreeNodeConstructor;
				if (tn != null)
				{
					ConstructorPointer cp0 = tn.OwnerPointer as ConstructorPointer;
					if (cp0.IsSameMethod((IMethod)cp))
					{
						cp0.CopyFrom(cp);
						treeViewConstructors.SelectedNode = tn;
						break;
					}
				}
				else
				{
					TreeNodeCustomConstructorPointer tn2 = treeViewConstructors.Nodes[i] as TreeNodeCustomConstructorPointer;
					if (tn2 != null)
					{
						CustomConstructorPointer cp2 = tn2.OwnerPointer as CustomConstructorPointer;
						if (cp2.IsSameMethod((IMethod)cp))
						{
							treeViewConstructors.SelectedNode = tn2;
							break;
						}
					}
				}
			}
		}
	}
}
