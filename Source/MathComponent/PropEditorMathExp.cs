/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Expression Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;
using MathExp;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;

namespace MathComponent
{
	class PropEditorMathExp : UITypeEditor
	{
		private Rectangle rc = new Rectangle(0, 0, 100, 30);
		public PropEditorMathExp()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e)
		{
			if (e.Context != null && e.Context.Instance != null)
			{
				MathematicExpression me = e.Context.Instance as MathematicExpression;
				if (me != null)
				{
					Image img = me.CreateMathExpressionImage(e.Graphics);
					e.Graphics.DrawImage(img, e.Bounds);
					e.Graphics.DrawString(me.ToString(), new Font("Times New Roman", 8), Brushes.Black, new PointF(32, 0));
				}
			}
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null && context.Instance != null)
			{
				MathematicExpression me = context.Instance as MathematicExpression;
				if (me != null)
				{
					IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
					if (edSvc != null)
					{
						Point curPoint = System.Windows.Forms.Cursor.Position;
						rc.X = curPoint.X;
						rc.Y = curPoint.Y;
						MathExpEditor.ExcludeProjectItem = true;
						IMathEditor dlg = me.CreateEditor(rc);
						MathExpEditor.ExcludeProjectItem = false;
						MathNodeRoot r = new MathNodeRoot();
						r.SetWriter(me.GetWriter());
						r.SetReader(me.GetReader());
						try
						{
							XmlDocument doc = new XmlDocument();
							doc.LoadXml(me.Formula.Xml);
							if (doc.DocumentElement != null)
							{
								r.Load(doc.DocumentElement);
							}
						}
						catch
						{
						}
						dlg.MathExpression = r;
						dlg.MathExpression.EnableUndo = true;
						try
						{
							if (edSvc.ShowDialog((Form)dlg) == DialogResult.OK)
							{
								r = (MathNodeRoot)dlg.MathExpression;
								r.FindAllInputVariables();
								XmlDocument doc = new XmlDocument();
								XmlNode rootNode = doc.CreateElement(MathematicExpression.XML_Root);
								doc.AppendChild(rootNode);
								r.SetWriter(me.GetWriter());
								r.SetReader(me.GetReader());
								r.Save(rootNode);
								value = new FormulaProperty(doc.OuterXml);
								PropertyGrid pg = null;
								Type t = edSvc.GetType();
								PropertyInfo pif0 = t.GetProperty("OwnerGrid");
								if (pif0 != null)
								{
									object g = pif0.GetValue(edSvc, null);
									pg = g as PropertyGrid;
									if (pg != null)
									{
										me.SetPropertyGrid(pg);
									}
								}
							}
						}
						catch (Exception err2)
						{
							MessageBox.Show(MathException.FormExceptionText(null, err2), "Math Expression Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
			}
			return value;
		}
	}
}
