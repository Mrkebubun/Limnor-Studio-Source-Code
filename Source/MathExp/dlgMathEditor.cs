/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
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
using LimnorUI;

namespace MathExp
{
	/// <summary>
	/// edit a single math expression
	/// </summary>
	public partial class dlgMathEditor : Form, IMathEditor
	{
		private bool _createCompound;
		private IActionContext _actionContext;
		public dlgMathEditor(Rectangle rcStart)
			: base(/*rcStart*/)
		{
			InitializeComponent();
			mathExpEditor1.OnCreateCompound += new EventHandler(mathExpEditor1_OnCreateCompound);
		}
		public void NoCompoundCreation()
		{
			mathExpEditor1.NoCompoundCreation();
			mathExpEditor1.OnCreateCompound -= new EventHandler(mathExpEditor1_OnCreateCompound);
		}
		public void SetScopeMethod(IMethod method)
		{
			mathExpEditor1.SetScopeMethod(method);
		}
		public bool CreateCompound
		{
			get
			{
				return _createCompound;
			}
		}
		/// <summary>
		/// when this math expression is used in an action, this is the action.
		/// if it is a property-setting action then the method is a set-property action;
		/// if it is a method execution action then this is the method-execution action.
		/// </summary>
		public IActionContext ActionContext
		{
			get
			{
				return _actionContext;
			}
			set
			{
				_actionContext = value;
				mathExpEditor1.ActionContext = _actionContext;
			}
		}
		private Type _vtt;
		/// <summary>
		/// the type for creating mapped instance
		/// currently it is typeof(ParameterValue)
		/// </summary>
		public Type VariableMapTargetType { get { return _vtt; } set { _vtt = value; } }
		void mathExpEditor1_OnCreateCompound(object sender, EventArgs e)
		{
			_createCompound = true;
			this.DialogResult = DialogResult.OK;
		}
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			mathExpEditor1.SetEditorFocus();
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			if (this.DialogResult != DialogResult.OK)
			{
				if (mathExpEditor1.Changed)
				{
					e.Cancel = UIUtil.AskCancelCloseDialog(this);
				}
			}
			if (!e.Cancel)
			{
				mathExpEditor1.AbortTest();
				mathExpEditor1.MathExpression.EitorBounds = this.Bounds;
				base.OnClosing(e);
			}
		}

		public XmlNode MathExpNode
		{
			get
			{
				return mathExpEditor1.MathExpNode;
			}
			set
			{
				mathExpEditor1.LoadFromXmlNode(value);
			}
		}

		private void mathExpEditor1_OnCancel(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void mathExpEditor1_OnOK(object sender, EventArgs e)
		{
			_createCompound = false;
			this.DialogResult = DialogResult.OK;
		}

		#region IMathEditor Members
		public IMathExpression MathExpression
		{
			get
			{
				return mathExpEditor1.MathExpression;
			}
			set
			{
				mathExpEditor1.MathExpression = (MathNodeRoot)value;
			}
		}
		public void AddService(Type t, object service)
		{
			mathExpEditor1.AddService(t, service);
		}
		public void DisableTest()
		{
			mathExpEditor1.DisableTest();
		}
		public void AddMathNode(Type type)
		{
			mathExpEditor1.AddMathNode(type);
		}
		#endregion
	}
}