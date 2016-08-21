/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Action;
using LimnorDesigner.MenuUtil;
using System.Windows.Forms;
using System.Drawing;
using MathExp;
using System.ComponentModel;
using VPL;
using System.Xml;
using WindowsUtility;
using XmlSerializer;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// its _componentPointer is an ActionBranchParameterPointer
	/// </summary>
	public class ComponentIconActionBranchParameter : ComponentIconParameter
	{
		private ActionBranch _actionBranch;
		/// <summary>
		/// for clone
		/// </summary>
		public ComponentIconActionBranchParameter()
			: base()
		{
		}
		public ComponentIconActionBranchParameter(ActionBranch branch)
			: base(branch)
		{
			_actionBranch = branch;

		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override bool ReadOnly
		{
			get
			{
				return true;
			}
			set
			{
			}
		}
		public ActionBranch ActionBranch
		{
			get
			{
				return _actionBranch;
			}
		}
		protected override void OnEstablishObjectOwnership(MethodClass owner)
		{
			base.OnEstablishObjectOwnership(owner);
			if (MemberId != _actionBranch.BranchId)
			{
				//search for the branch
				ActionBranch ab = _actionBranch.ActionsHolder.FindActionBranchById(MemberId);
				if (ab == null)
				{
				}
				else
				{
					_actionBranch = ab;
					ActionBranchParameter abp = this.ClassPointer as ActionBranchParameter;
					if (abp != null)
					{
						abp.SetActionBranch(ab);
					}
					RefreshLabelText();
				}
			}
		}
		private void mi_useAs(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if (mi != null)
			{
				ComponentIconLocal cil = mi.Tag as ComponentIconLocal;
				if (cil != null)
				{
					MethodDesignerHolder mh = null;
					Control c = this.Parent;
					while (c != null)
					{
						mh = c as MethodDesignerHolder;
						if (mh != null)
						{
							break;
						}
						c = c.Parent;
					}
					if (mh != null)
					{
						ParameterClassSubMethod pc = this.ClassPointer as ParameterClassSubMethod;
						mh.LoadNewCastAs(pc, cil.LocalPointer);
					}
				}
			}
		}
		public override void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			if (saved)
			{
			}
			else
			{
				if (this.ClassPointer == null)
				{
					this.ClassPointer = new ActionBranchParameter(_actionBranch);
				}
			}
		}
		protected override void OnCreateContextMenu(ContextMenu mnu, Point location)
		{
			base.OnCreateContextMenu(mnu, location);
			Type t = null;
			ParameterClassArrayItem ap = this.ClassPointer as ParameterClassArrayItem;
			ParameterClassCollectionItem ci = this.ClassPointer as ParameterClassCollectionItem;
			if (ap != null)
			{
				t = ap.BaseClassType;
			}
			else if (ci != null)
			{
				t = ci.BaseClassType;
			}
			if (t != null)
			{
				if (this.Parent != null)
				{
					List<ComponentIconLocal> list = new List<ComponentIconLocal>();
					foreach (Control c in this.Parent.Controls)
					{
						ComponentIconLocal cil = c as ComponentIconLocal;
						if (cil != null)
						{
							if (t.IsAssignableFrom(cil.LocalPointer.BaseClassType))
							{
								list.Add(cil);
							}
						}
					}
					if (list.Count > 0)
					{
						if (mnu.MenuItems.Count > 0)
						{
							mnu.MenuItems.Add("-");
						}
						MenuItemWithBitmap mi = new MenuItemWithBitmap("Use it as", Resources._as.ToBitmap());
						mnu.MenuItems.Add(mi);
						foreach (ComponentIconLocal cil in list)
						{
							MenuItemWithBitmap mi0 = new MenuItemWithBitmap(cil.Label.Text, mi_useAs, VPL.VPLUtil.GetTypeIcon(cil.ClassPointer.ObjectType));
							mi0.Tag = cil;
							mi.MenuItems.Add(mi0);
						}

					}
				}
			}
		}
		protected override IAction OnCreateAction(MenuItemDataMethod data, ILimnorDesignPane designPane)
		{
			ActionBranchParameter ab = (ActionBranchParameter)ClassPointer;
			ActionBranchParameterPointer abp = new ActionBranchParameterPointer(ab, designPane.RootClass);
			abp.MethodId = MethodViewer.Method.MethodID;
			abp.BranchId = ActionBranch.BranchId;
			IClass op = data.Owner;
			data.ResetOwner(abp);
			try
			{
				IAction act = data.CreateMethodAction(designPane, abp, MethodViewer.Method, MethodViewer.ActionsHolder);
				if (act != null)
				{
					act.ScopeMethod = MethodViewer.Method;
					act.ActionHolder = MethodViewer.ActionsHolder;
				}

				data.ResetOwner(op);
				return act;
			}
			catch
			{
				throw;
			}
			finally
			{
				data.ResetOwner(op);
			}
		}
		#region ICloneable Members
		public override object Clone()
		{
			ComponentIconActionBranchParameter obj = (ComponentIconActionBranchParameter)base.Clone();
			obj._actionBranch = _actionBranch;
			return obj;
		}
		#endregion
	}
}
