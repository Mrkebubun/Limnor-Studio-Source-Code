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
using System.Windows.Forms.Design.Behavior;
using System.Collections;
using System.Windows.Forms;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// control the drag/drop of selected component, preventing from dropping to a different Form
	/// </summary>
	class DebugBehaviorService : Behavior
	{
		public DebugBehaviorService()
		{
		}
		public ICollection ComponentsDraged { get; set; }
		/// <summary>
		/// check dragged components are within their parent
		/// </summary>
		/// <param name="g">represent the Form</param>
		/// <param name="e"></param>
		/// <returns></returns>
		private bool candrop(Glyph g, System.Windows.Forms.DragEventArgs e)
		{
			bool bRet = false;
			if (ComponentsDraged != null && ComponentsDraged.Count > 0)
			{
				System.Windows.Forms.Design.Behavior.ControlBodyGlyph cbg = g as System.Windows.Forms.Design.Behavior.ControlBodyGlyph;
				if (cbg != null)
				{
					MethodDiagramViewer mv = cbg.RelatedComponent as MethodDiagramViewer;
					if (mv != null)
					{
						bRet = true;
						IEnumerator ie = ComponentsDraged.GetEnumerator();
						while (ie.MoveNext())
						{
							ActionViewer v = ie.Current as ActionViewer;
							if (v != null)
							{
								if (mv.GUID != v.ParentGuid)
								{
									bRet = false;
									break;
								}
							}
						}
					}
				}
			}
			if (!bRet)
			{
				e.Effect = System.Windows.Forms.DragDropEffects.None;
			}
			return bRet;
		}
		public override void OnDragOver(Glyph g, System.Windows.Forms.DragEventArgs e)
		{
			if (candrop(g, e))
			{
				base.OnDragOver(g, e);
			}
		}
		public override void OnDragDrop(Glyph g, System.Windows.Forms.DragEventArgs e)
		{
			if (candrop(g, e))
			{
				base.OnDragDrop(g, e);
			}
		}
	}
}
