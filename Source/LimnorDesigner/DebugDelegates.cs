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
using LimnorDesigner.Action;
using System.Windows.Forms;
using System.Drawing;

namespace LimnorDesigner
{
	delegate void fnVoid();
	delegate void fnOnObject(object v);
	delegate void fnOnImage(Image v);
	delegate void fnOnDebug(LimnorDebugger v);
	delegate void fnOnControl(Control v);
	delegate void fnOnInteger(int v);
	delegate void fnOnBool(bool v);
	delegate void fnOnTreeNodeExplorer(TreeNodeExplorer v);
	delegate void fnOnSplitContainer(SplitContainer v);
	delegate void fnAddToSplitContainer(SplitContainer v, Control s);
	delegate void fnShowBreakPointInMethod(int threadId, MethodClass method, IActionGroup group, ActionBranch branch);
}
