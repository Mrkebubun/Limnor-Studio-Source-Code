using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace LimnorWebBrowser
{
    public class EditContents
    {
        private WebBrowserControl _htmlEditor;
        public EditContents(WebBrowserControl editor)
        {
            _htmlEditor = editor;
        }
        public Color BackColor
        {
            get
            {
                return _htmlEditor.BackColor;
            }
        }
        public string HtmlContents
        {
            get
            {
                return _htmlEditor.BodyHtml;
            }
            set
            {
                PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(_htmlEditor, true);
                foreach (PropertyDescriptor p in ps)
                {
                    if (string.CompareOrdinal("BodyHtml", p.Name) == 0)
                    {
                        p.SetValue(_htmlEditor, value);
                        break;
                    }
                }
            }
        }
        public override string ToString()
        {
            return "...";
        }
    }
}
