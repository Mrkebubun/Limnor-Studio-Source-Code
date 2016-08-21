using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System;
using System.Globalization;
public partial class proxyName
{
    public static proxyName CreateProxy(string endpointName)
    {
        return new proxyName(endpointName);
    }
    internal static string ConfigFilePath = string.Empty;
    public static void SetConfigFilePath(string path)
    {
        ConfigFilePath = path;
    }
    private string _endpointName;
    [Editor(typeof(EndPointSelector), typeof(UITypeEditor))]
    [Description("Gets and sets the endpoint name")]
    public string EndPointName
    {
        get
        {
            return _endpointName;
        }
        set
        {
            _endpointName = value;
        }
    }
    class EndPointSelector : UITypeEditor
    {
        public EndPointSelector()
        {
        }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            if (context != null && context.Instance != null && provider != null)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    ValueList list = new ValueList(edSvc);
                    edSvc.DropDownControl(list);
                    if (list.MadeSelection)
                    {
                        value = list.Selection;
                    }
                }
            }
            return value;
        }
        class ValueList : ListBox
        {
            public bool MadeSelection;
            public object Selection;
            private IWindowsFormsEditorService _service;
            public ValueList(IWindowsFormsEditorService service)
            {
                _service = service;
                string cfgFile = proxyName.ConfigFilePath; // Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), string.Format(CultureInfo.InvariantCulture,"{0}.config",this.GetType().Name));
                if (string.IsNullOrEmpty(cfgFile))
                {
                    throw new Exception("Configuration file not set");
                }
                if (!File.Exists(cfgFile))
                {
                    throw new Exception(string.Format(CultureInfo.InvariantCulture, "Configuration file not found at {0}", cfgFile));
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(cfgFile);
                XmlNodeList nodes = doc.SelectNodes("//endpoint");
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes != null)
                    {
                        XmlAttribute xa = node.Attributes["name"];
                        if (xa != null)
                        {
                            if (!string.IsNullOrEmpty(xa.Value))
                            {
                                Items.Add(xa.Value);
                            }
                        }
                    }
                }
            }
            private void finishSelection()
            {
                if (SelectedIndex >= 0)
                {
                    MadeSelection = true;
                    Selection = Items[SelectedIndex];
                }
                _service.CloseDropDown();
            }
            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                finishSelection();

            }
            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                base.OnKeyPress(e);
                if (e.KeyChar == '\r')
                {
                    finishSelection();
                }
            }
        }
    }
}