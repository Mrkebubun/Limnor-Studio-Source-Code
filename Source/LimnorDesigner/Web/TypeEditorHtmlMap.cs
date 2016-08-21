/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Limnor.WebBuilder;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Globalization;

namespace LimnorDesigner.Web
{
	class TypeEditorHtmlMap : UITypeEditor
	{
		public TypeEditorHtmlMap()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					HtmlElement_img himg = context.Instance as HtmlElement_img;
					if (himg != null)
					{
						ClassPointer root = himg.RootPointer;
						if (root != null)
						{
							WebPage wpage = root.ObjectInstance as WebPage;
							if (wpage != null)
							{
								if (wpage.EditorStarted)
								{
									string f = himg.src;
									if (!string.IsNullOrEmpty(f))
									{
										if (!File.Exists(f))
										{
											f = Path.Combine(root.Project.WebPhysicalFolder(wpage), f);
										}
									}
									if (string.IsNullOrEmpty(f) || !File.Exists(f))
									{
										MessageBox.Show("Image file is not specified. Please set src property.", "Map");
									}
									else
									{
										Image bkImg = null;
										try
										{
											bkImg = Image.FromFile(f);
										}
										catch (Exception err)
										{
											MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Error load image file [{0}]. {1}", f, err.Message), "Map", MessageBoxButtons.OK, MessageBoxIcon.Error);
										}
										if (bkImg != null)
										{
											DialogSelectHtmlMap dlg = new DialogSelectHtmlMap();
											string mapId = himg.usemap;
											if (!string.IsNullOrEmpty(mapId) && mapId.StartsWith("#", StringComparison.Ordinal))
											{
												mapId = mapId.Substring(1);
											}
											dlg.LoadData(bkImg, wpage, mapId);
											if (edSvc.ShowDialog(dlg) == DialogResult.OK)
											{
												string mid = string.Format(CultureInfo.InvariantCulture, "#{0}", dlg.MapID);
												value = mid;
												if (string.IsNullOrEmpty(himg.id))
												{
													root.OnUseHtmlElement(himg);
												}
												wpage.SetUseMap(himg.id, mid);
											}
										}
									}
								}
								else
								{
									MessageBox.Show("Html editor not started. Please go into Html Editing mode", "Map");
								}
							}
						}
					}
				}
			}
			return value;
		}
	}
}
