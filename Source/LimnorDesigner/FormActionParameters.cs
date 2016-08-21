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
using System.Text;
using System.Windows.Forms;
using System.Xml;
using ProgElements;
using MathExp;
using VPL;
using LimnorDesigner.Action;

namespace LimnorDesigner
{
	public partial class FormActionParameters : Form
	{
		XmlNode _rootNode;
		public FormActionParameters()
		{
			InitializeComponent();
			propertyGrid1.PropertySort = PropertySort.NoSort;
			propertyGrid1.ToolbarVisible = false;
			btOK.ImageList = TreeViewObjectExplorer.ObjectImageList;
			btOK.ImageIndex = TreeViewObjectExplorer.IMG_OK;
			btCancel.ImageList = TreeViewObjectExplorer.ObjectImageList;
			btCancel.ImageIndex = TreeViewObjectExplorer.IMG_CANCEL;
			btBack.ImageList = TreeViewObjectExplorer.ObjectImageList;
			btBack.ImageIndex = TreeViewObjectExplorer.IMG_ARROWLT;
			propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);
			this.Activated += FormActionParameters_Activated;
		}

		void FormActionParameters_Activated(object sender, EventArgs e)
		{
			FormProgress.HideProgress();
		}
		public void SetScopeMethod(IMethod m)
		{
			propertyGrid1.ScopeMethod = m;
		}
		void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			ActionAttachEvent aae = propertyGrid1.SelectedObject as ActionAttachEvent;
			if (aae != null)
			{
				if (string.CompareOrdinal(e.ChangedItem.PropertyDescriptor.Name, "ActionName") == 0)
				{
					if (DesignUtil.IsActionNameInUse(_rootNode, e.ChangedItem.Value.ToString(), aae.ActionId))
					{
						MessageBox.Show(this, "The action name is in use", "Set action name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						aae.ActionName = e.OldValue.ToString();
					}
				}
				else
				{
					propertyGrid1.Refresh();
				}
			}
			else
			{
				ActionClass act = propertyGrid1.SelectedObject as ActionClass;
				if (act != null)
				{
					if (string.CompareOrdinal(e.ChangedItem.PropertyDescriptor.Name, "ActionName") == 0)
					{
						if (DesignUtil.IsActionNameInUse(_rootNode, e.ChangedItem.Value.ToString(), act.ActionId))
						{
							MessageBox.Show(this, "The action name is in use", "Set action name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							act.ActionName = e.OldValue.ToString();
						}
					}
					else
					{
						if (act.ActionMethod != null && act.ActionMethod.Owner != null)
						{
							IMethodParameterAttributesProvider dmp = act.ActionMethod.Owner.ObjectInstance as IMethodParameterAttributesProvider;
							if (dmp != null)
							{
								Dictionary<string, Attribute[]> pap = dmp.GetParameterAttributes(act.ActionMethod.MethodName);
								if (pap != null && pap.Count > 0)
								{
									propertyGrid1.Refresh();
								}
							}
						}
					}
				}
			}
		}
		public void LoadAction(IAction act, XmlNode root)
		{
			_rootNode = root;
			propertyGrid1.SelectedObject = act;
		}
		public bool LoadObject(IDynamicProperties obj)
		{
			try
			{
				obj.SetPropertyGrid(propertyGrid1);
				propertyGrid1.SelectedObject = obj;
				return true;
			}
			catch (Exception e)
			{
				MathNode.Log(this,e);
			}
			return false;
		}
		public void DisableBack()
		{
			btBack.Enabled = false;
		}

		private void btCancel_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "Do you want to cancel the action creation?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				this.DialogResult = DialogResult.OK;
			}
		}
	}
}
