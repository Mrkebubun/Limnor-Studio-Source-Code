/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace VPL
{
	public class TypeEditorImage : UITypeEditor
	{
		public TypeEditorImage()
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
					try
					{
						FormImage dlg = new FormImage();
						if (edSvc.ShowDialog(dlg) == DialogResult.OK)
						{
							Image img = Image.FromFile(dlg.FileName);
							if (img != null)
							{
								return img;
							}
						}
					}
					catch (Exception err)
					{
						MessageBox.Show(err.Message);
					}
				}
			}
			return base.EditValue(context, provider, value);
		}
		public class TypeEditorImageFilename : UITypeEditor
		{
			public TypeEditorImageFilename()
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
						try
						{
							FormImage dlg = new FormImage();
							dlg.FileName = value as string;
							if (context.PropertyDescriptor.Attributes != null)
							{
								for (int i = 0; i < context.PropertyDescriptor.Attributes.Count; i++)
								{
									FileSelectionAttribute fsa = context.PropertyDescriptor.Attributes[i] as FileSelectionAttribute;
									if (fsa != null)
									{
										dlg.SetParams(fsa.Filter, fsa.Title);
										break;
									}
								}
							}
							if (edSvc.ShowDialog(dlg) == DialogResult.OK)
							{
								value = dlg.FileName;
							}
						}
						catch (Exception err)
						{
							MessageBox.Show(err.Message);
						}
					}
				}
				return value;
			}
		}
	}
	public class FormImage : Form
	{
		public string FileName;
		private bool _loaded;
		private string _filter;
		private string _title;
		public FormImage()
		{
			this.FormBorderStyle = FormBorderStyle.None;
			this.Size = new Size(1, 1);
		}
		public void SetParams(string filter, string title)
		{
			_filter = filter;
			_title = title;
		}
		protected override void OnActivated(EventArgs e)
		{
			if (!_loaded)
			{
				_loaded = true;
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.CheckFileExists = true;
				if (string.IsNullOrEmpty(_title))
					dlg.Title = "Select image file";
				else
					dlg.Title = _title;
				if (string.IsNullOrEmpty(_filter))
					dlg.Filter = "Image files|*.bmp;*.png;*.jpg;*.gif;*.tiff";
				else
					dlg.Filter = _filter;
				if (!string.IsNullOrEmpty(FileName))
				{
					try
					{
						dlg.FileName = FileName;
					}
					catch
					{
					}
				}
				System.Windows.Forms.DialogResult ret = dlg.ShowDialog(this);
				if (ret == DialogResult.OK)
				{
					FileName = dlg.FileName;
				}
				this.DialogResult = ret;
				Close();
			}
		}
	}
	public class FileSelectionAttribute : Attribute
	{
		public FileSelectionAttribute(string filter, string title)
		{
			Filter = filter;
			Title = title;
		}
		public string Filter
		{
			get;
			private set;
		}
		public string Title { get; private set; }
	}
}
