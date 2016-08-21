/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using MathExp;
using WindowsUtility;

namespace LimnorDesigner.MethodBuilder
{
	public class ActionViewerConstructor : ActionViewerSingleAction
	{
		private string _displayData;
		public ActionViewerConstructor()
		{
		}
		[Browsable(false)]
		public override bool NameReadOnly
		{
			get
			{
				return true;
			}
		}
		[Browsable(false)]
		protected override bool CanReplaceAction { get { return false; } }
		[Browsable(false)]
		protected override bool CanEditAction { get { return false; } }
		[Browsable(false)]
		public override string ActionDisplay
		{
			get
			{
				if (string.IsNullOrEmpty(_displayData))
				{
					OnActionNameChanged();
				}
				return _displayData;
			}
		}
		protected override void OnActionNameChanged()
		{
			AB_Constructor ab = this.ActionObject as AB_Constructor;
			if (ab != null && ab.ActionData != null)
			{
				ConstructorPointer cp = ab.ActionData.ActionMethod as ConstructorPointer;
				if (cp.ReturnReceiver != null)
				{
					_displayData = "Create " + cp.ReturnReceiver.Name;
				}
				else
				{
					_displayData = "Create " + cp.TypeString;
				}
			}
		}
		protected override void OnPaintAction(PaintEventArgs e)
		{
			SizeF sz = e.Graphics.MeasureString(ActionDisplay, TextFont);
			float x = (Size.Width - sz.Width - Resources.constructor.Width) / 2 + Resources.constructor.Width;
			float y = (Size.Height - sz.Height) / 2;
			if (x < 0)
				x = 0;
			if (y < 0)
				y = 0;
			e.Graphics.DrawImageUnscaled(Resources.constructor, 2, 2);
			e.Graphics.DrawString(ActionDisplay, TextFont, TextBrush, x, y);
		}
		protected override void OnCreateContextMenu(ContextMenu cm)
		{
			AB_Constructor av = this.ActionObject as AB_Constructor;
			ConstructorPointer cp = av.ActionData.ActionMethod as ConstructorPointer;
			LocalVariable lv = cp.Owner as LocalVariable;
			ConstructorInfo[] cis = lv.ObjectType.GetConstructors();
			if (cis.Length > 1)
			{
				MenuItem mi;
				//
				mi = new MenuItemWithBitmap("Select Constructor", miSelectConstructor_Click, Resources._dialog.ToBitmap());
				cm.MenuItems.Add(mi);
				//
				mi = new MenuItem("-");
				cm.MenuItems.Add(mi);
			}
		}

		void miSelectConstructor_Click(object sender, EventArgs e)
		{
			MethodDiagramViewer mv = this.Parent as MethodDiagramViewer;
			if (mv != null)
			{
				AB_Constructor av = this.ActionObject as AB_Constructor;
				IConstructor cp = av.ActionData.ActionMethod as IConstructor;
				LocalVariable lv = cp.Owner as LocalVariable;
				dlgConstructorParameters dlg = new dlgConstructorParameters();
				dlg.SetMethod(mv.DesignerHolder.Method);
				dlg.LoadData(lv);
				dlg.LocateConstructor(cp);
				if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
				{
					cp.CopyFrom(dlg.Ret);
					mv.Changed = true;
				}
			}
		}
	}
}
