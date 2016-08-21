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
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ProgElements;
namespace MathExp
{
	public class MathPropertyGrid : PropertyGrid
	{
		private IMethod _scopeMethod;
		private EventHandler _onValueChanged;
		public MathPropertyGrid()
		{
			this.ToolStripRenderer = new ToolStripProfessionalRenderer(new GradientColorTable());
		}
		public IMethod ScopeMethod
		{
			get
			{
				return _scopeMethod;
			}
			set
			{
				_scopeMethod = value;
			}
		}
		public void SetOnValueChange(EventHandler onValueChanged)
		{
			_onValueChanged = onValueChanged;
		}
		public void OnValueChanged(object sender, EventArgs e)
		{
			if (_onValueChanged != null)
			{
				_onValueChanged(sender, e);
			}
		}
	}
}
