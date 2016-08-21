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
using System.Windows.Forms;
using XmlUtility;
using VSPrj;
using System.Drawing;
using System.Collections;

namespace LimnorDesigner.ResourcesManager
{
	public class TypeSelectorLanguage : UITypeEditor
	{
		public TypeSelectorLanguage()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (service != null)
				{
					ILimnorProject p = null;
					IProjectAccessor pa = context.Instance as IProjectAccessor;
					if (pa != null)
					{
						p = pa.Project;
					}
					else
					{
						IWithProject wp = context.Instance as IWithProject;
						if (wp != null)
						{
							p = wp.Project;
						}
					}
					if (p != null)
					{
						LanguageList ctl = new LanguageList(service, p);
						service.DropDownControl(ctl);
						if (ctl.SelectedLanguage != null)
						{
							value = ctl.SelectedLanguage;
						}
					}
				}
			}
			return value;
		}
		class LanguageList : ListBox
		{
			IWindowsFormsEditorService _srv;
			public string SelectedLanguage;
			public LanguageList(IWindowsFormsEditorService service, ILimnorProject project)
			{
				_srv = service;
				this.DrawMode = DrawMode.OwnerDrawFixed;
				this.Items.Add(new languageItem(string.Empty));
				IList<string> names = XmlUtil.GetLanguages(project.ResourcesXmlNode);
				foreach (string s in names)
				{
					this.Items.Add(new languageItem(s));
				}
			}
			protected override void OnDrawItem(DrawItemEventArgs e)
			{
				e.DrawBackground();
				if (e.Index >= 0)
				{
					if ((e.State & DrawItemState.Selected) != 0)
					{
						e.DrawFocusRectangle();
					}
					languageItem iid = this.Items[e.Index] as languageItem;
					System.Drawing.Rectangle rc = new System.Drawing.Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
					e.Graphics.DrawImage(iid.Image, rc.X, rc.Y);
					rc.X += 16;
					if (rc.Width > 16)
					{
						rc.Width -= 16;
					}
					e.Graphics.DrawString(iid.Name, this.Font, Brushes.Black, rc);
				}
			}
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				if (SelectedIndex >= 0)
				{
					SelectedLanguage = ((languageItem)Items[SelectedIndex]).Name;
				}
				_srv.CloseDropDown();
			}
			protected override void OnKeyPress(KeyPressEventArgs e)
			{
				base.OnKeyPress(e);
				if (e.KeyChar == '\r')
				{
					if (SelectedIndex >= 0)
					{
						SelectedLanguage = ((languageItem)Items[SelectedIndex]).Name;
					}
					_srv.CloseDropDown();
				}
			}
			class languageItem
			{
				private string _name;
				private Image _img;
				public languageItem(string name)
				{
					_name = name;
					_img = TreeViewObjectExplorer.GetLangaugeBitmapByName(_name);
				}
				public string Name { get { return _name; } }
				public Image Image { get { return _img; } }
			}
		}
	}
}
