using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CefSharp;
using CefSharp.LimnorStudio;
using CefSharp.WinForms;
using XmlUtility;
using Limnor.WebBuilder;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using MathExp;

namespace LimnorDesigner.Web
{
    class WebHostX86 : UserControl, ISkipWrite, ILimnorStudioView, IWebHost
    {
        #region fields and constructors
        private string _loadedUrl;
        private string _webPhysicalFolder;
        private string _projectFolder;
        private string _htmlUrl;
        private string _htmlFile;
        //private string _htmlString; //for detecting modifications
        private string _docType;
        private Encoding _encoding = Encoding.Unicode;
        //private bool _htmlChanged;
        private bool _editorStarted;
        private bool _editorLoaded;
        private string _selectedElement = string.Empty;
        private WebPage _page;
        private readonly WebView web_view;
        private Timer _timerX;
        public WebHostX86()
        {
            BrowserSettings bs = new BrowserSettings();
            bs.ApplicationCacheDisabled = true;
            bs.HistoryDisabled = true;
            bs.JavaScriptAccessClipboardDisallowed = false;
            bs.JavaScriptDisabled = false;
            bs.JavaScriptOpenWindowsDisallowed = false;
            bs.PageCacheDisabled = true;
            bs.UniversalAccessFromFileUrlsAllowed = true;
            bs.WebSecurityDisabled = true;
            bs.TabToLinksDisabled = true;
            web_view = new WebView("about:blank", bs);
            web_view.Dock = DockStyle.Fill;
            web_view.RegisterJsObject("limnorStudio", new limnorStudioJs(this));
            this.Controls.Add(web_view);
            var presenter = new LimnorStudioPresenter(web_view, this,
                invoke => Invoke(invoke));
            this.Dock = DockStyle.Fill;
            this.AutoScroll = false;
            
            
        }
        #endregion
        #region Methods
        public void LoadWebHost(string prjFolder, string webFolder, string webName, string htmlFile, Encoding encode, string docType, WebPage wpage)
        {
            _projectFolder = prjFolder;
            _webPhysicalFolder = webFolder;
            _encoding = encode;
            _docType = docType;
            _page = wpage;
            _page.SetHtmlEditor(this);
            _htmlFile = htmlFile;
            _htmlUrl = string.Format(CultureInfo.InvariantCulture,
                "http://localhost/{0}/{1}", webName, Path.GetFileName(htmlFile));
            //LoadUrl(string.Format(CultureInfo.InvariantCulture, "http://localhost/ideloader.html?{0}", _htmlUrl));
			
        }
        public void LoadIntoControl()
        {
            LoadUrl(string.Format(CultureInfo.InvariantCulture, "http://localhost/ideloader.html?{0}", _htmlUrl));
        }
        public void SetEncode(Encoding encode)
        {
            _encoding = encode;
        }
        public void SetDocType(string docType)
        {
            _docType = docType;
        }
        public void StartEditor()
        {
            if (_editorStarted)
            {
                if (_page != null)
                {
                    _page.RefreshHtmlElementSelection();
                }
                //if (_tmHtml != null)
                //{
                //    _tmHtml.Enabled = true;
                //}
            }
            else
            {
                //if (webBrowser1.Document != null)
                {
                    _editorStarted = true;
                    LoadIntoControl();
                    /*
                    HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0];
                    HtmlElement scriptEl = webBrowser1.Document.CreateElement("script");
                    IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
                    element.text = Resources.jscolor;
                    head.AppendChild(scriptEl);
                    //
                    scriptEl = webBrowser1.Document.CreateElement("script");
                    element = (IHTMLScriptElement)scriptEl.DomElement;
                    element.text = DesignService.GetJsonMin();
                    head.AppendChild(scriptEl);
                    //
                    scriptEl = webBrowser1.Document.CreateElement("script");
                    element = (IHTMLScriptElement)scriptEl.DomElement;
                    element.text = DesignService.GetModalMin();
                    head.AppendChild(scriptEl);
                    //
                    scriptEl = webBrowser1.Document.CreateElement("script");
                    element = (IHTMLScriptElement)scriptEl.DomElement;
                    element.text = Resources.htmleditor1;
                    head.AppendChild(scriptEl);
                    //
                    scriptEl = webBrowser1.Document.CreateElement("script");
                    element = (IHTMLScriptElement)scriptEl.DomElement;
                    element.text = Resources.DEBUG_htmlEditorStarter;
                    head.AppendChild(scriptEl);
                    //
                    //problem: permission denied
                    webBrowser1.Document.InvokeScript("limnorHtmlEditoreditHtmlFile", new object[] { _htmlUrl });
                    */
                    //
                    //TBD2
                    //if (_page != null)
                    //{
                    //    if (!string.IsNullOrEmpty(_page.BackgroundImageFile))
                    //    {
                    //        if (File.Exists(_page.BackgroundImageFile))
                    //        {
                    //            StringBuilder sb = new StringBuilder();
                    //            sb.Append("document.body.style.backgroundImage='url(");
                    //            sb.Append("Images/");
                    //            sb.Append(Path.GetFileName(_page.BackgroundImageFile));
                    //            sb.Append(")';");
                    //            ExecuteJs(sb.ToString());
                    //            _page.BackgroundImageFile = string.Empty;
                    //        }
                    //    }
                    //}
                    //webBrowser1.Navigate("javascript:function foo(){alert('hello');}foo();");
                    //TBD2
                    //                    HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0];
                    //                    HtmlElement scriptEl = webBrowser1.Document.CreateElement("script");
                    //                    IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
                    //#if DEBUG
                    //                    element.text = Resources.startHtmlEditor;
                    //#else
                    //                    element.text = Resources.startHtmlEditor_release;
                    //#endif
                    //                    head.AppendChild(scriptEl);
                    //                    if (string.IsNullOrEmpty(_docType))
                    //                    {
                    //                        webBrowser1.Document.InvokeScript("startHtmlEditor");
                    //                    }
                    //                    else
                    //                    {
                    //                        webBrowser1.Document.InvokeScript("startHtmlEditor", new object[] { _docType });
                    //                    }
                    //ENDo of TBD2
                    //
                    //_tmHtml = new Timer();
                    //_tmHtml.Interval = 1000;
                    //_tmHtml.Tick += new EventHandler(_tmHtml_Tick);
                    //_tmHtml.Enabled = true;
                    //
                    //TBD2
                    //HTMLDocumentClass doc = webBrowser1.Document.DomDocument as HTMLDocumentClass;
                    //IHTMLDOMNode node = element as IHTMLDOMNode;
                    //if (node != null)
                    //{
                    //    node.parentNode.removeChild(node);
                    //}
                    //_editorLoaded = true;
                }
            }
        }
        public void ExecuteJs(string jsCms, params object[] values)
        {
            if (_editorLoaded)
            {
                if (values != null && values.Length > 0)
                {
                    jsCms = string.Format(CultureInfo.InvariantCulture, jsCms, values);
                }
                web_view.ExecuteScript(jsCms);
            }
        }
        public object EvaluateJs(string jsCms, params object[] values)
        {
            if (_editorLoaded)
            {
                if (values != null && values.Length > 0)
                {
                    jsCms = string.Format(CultureInfo.InvariantCulture, jsCms, values);
                }
                return web_view.EvaluateScript(jsCms);
            }
            return null;
        }
        public bool checkHtmlChange()
        {
            if (_editorLoaded)
            {
                object v = EvaluateJs("pageModified();");
                if (v != null)
                {
                    return Convert.ToBoolean(v);
                }
            }
            return false;
        }
        public void Save()
        {
            if (_editorLoaded && _timerX == null)
            {
                ExecuteJs("saveAndFinish();");
                _timerX = new Timer();
                _timerX.Enabled = false;
                _timerX.Interval = 300;
                _timerX.Tick += _timerX_Tick;
                _timerX.Enabled = true;
            }
        }

        void _timerX_Tick(object sender, EventArgs e)
        {
            _timerX.Enabled = false;
            _timerX = null;
            string src = string.Empty;
            string tgt = string.Empty;
            try
            {
                src = _htmlFile;
                tgt = Path.Combine(_projectFolder, Path.GetFileName(_htmlFile));
                File.Copy(src, tgt, true);
            }
            catch (Exception err)
            {
                MathNode.LogError(string.Format(CultureInfo.InvariantCulture, "Error copying from [{0}] to [{1}]. {2}", src, tgt, err.Message));
            }
            int p = _htmlFile.LastIndexOf('.');
            if (p > 0)
            {
                src = string.Format(CultureInfo.InvariantCulture, "{0}.css", _htmlFile.Substring(0, p));
                if (File.Exists(src))
                {
                    tgt = Path.Combine(_projectFolder, Path.GetFileName(src));
                    try
                    {
                        File.Copy(src, tgt, true);
                    }
                    catch (Exception err)
                    {
                        MathNode.LogError(string.Format(CultureInfo.InvariantCulture, "Error copying from [{0}] to [{1}]. {2}", src, tgt, err.Message));
                    }
                }
            }
        }
        public void SelectHtmlElementByGuid(Guid guid)
        {
            ExecuteJs("setSelectedObject('{0}');",  HtmlElement_Base.GetGuidString(guid));
        }
        //public bool CheckContentsChanged()
        //{
        //    object v = EvaluateJs("pageModified();");
        //    if (v != null)
        //    {
        //        _htmlChanged = Convert.ToBoolean(v);
        //        return _htmlChanged;
        //    }
        //    return false;
        //}

        //public string ExportHtmlString()
        //{
        //    if (_editorStarted)
        //    {
        //        object v = EvaluateJs("getHtmlString();");
        //        string html = v as string;
        //        if (string.IsNullOrEmpty(_htmlString))
        //        {
        //            _htmlString = html;
        //        }
        //        else
        //        {
        //            if (string.CompareOrdinal(html, _htmlString) != 0)
        //            {
        //                _htmlString = html;
        //                _htmlChanged = true;
        //                if (!string.IsNullOrEmpty(_htmlFile))
        //                {
        //                    StreamWriter sw = new StreamWriter(_htmlFile, false, _encoding);
        //                    sw.Write(html);
        //                    sw.Flush();
        //                    sw.Close();
        //                }
        //            }
        //        }
        //        return html;
        //    }
        //    return null;
        //}
        #endregion
        #region Properties
        public bool EditorLoaded
        {
            get
            {
                return _editorLoaded;
            }
        }
        public string HtmlUrl
        {
            get
            {
                return _htmlUrl;
            }
        }
        public bool EditorStarted
        {
            get
            {
                return _editorStarted;
            }
        }
        //[ReadOnly(true)]
        //[Browsable(false)]
        //public bool HtmlChanged
        //{
        //    get
        //    {
        //        return _htmlChanged;
        //    }
        //    //set
        //    //{
        //    //    _htmlChanged = value;
        //    //}
        //}
        #endregion
        #region IWebHost
        public string CreateOrGetIdForCurrentElement(string newGuid)
        {
            if (_editorStarted)
            {
                return EvaluateJs("createOrGetId('{0}');", newGuid) as string;
            }
            return string.Empty;
        }
        public string getTagNameById(string id)
        {
            if (_editorStarted)
            {
                object v = EvaluateJs("getTagNameById('{0}');", id);
                if (v != null)
                {
                    string tag = v.ToString();
                    return tag.ToLower();
                }
            }
            return string.Empty;
        }
        public string getIdByGuid(string guid)
        {
            if (_editorStarted)
            {
                object v = EvaluateJs("getIdByGuid('{0}');", guid);
                string id = v as string;
                if (id == null)
                    return string.Empty;
                return id;
            }
            return string.Empty;
        }
        public Dictionary<string, string> getIdList()
        {
            Dictionary<string, string> l = new Dictionary<string, string>();
            if (_editorStarted)
            {
                object v = EvaluateJs("getIdList();");
                string ids = v as string;
                if (!string.IsNullOrEmpty(ids))
                {
                    string[] ss = ids.Split(',');
                    for (int i = 0; i < ss.Length; i++)
                    {
                        string[] kv = ss[i].Split(':');
                        l.Add(kv[0], kv[1]);
                    }
                }
            }
            return l;
        }
        public string GetMaps()
        {
            string ret = null;
            if (_editorStarted)
            {
                object v = EvaluateJs("getMaps();");
                ret = v as string;
            }
            return ret;
        }
        public string GetAreas(string mapId)
        {
            string ret = null;
            if (_editorStarted)
            {
                object v = EvaluateJs("getAreas('{0}');",mapId);
                ret = v as string;
            }
            return ret;
        }
        public string CreateNewMap(string guid)
        {
            string ret = null;
            if (_editorStarted)
            {
                object v = EvaluateJs("createNewMap('{0}');", guid);
                ret = v as string;
            }
            return ret;
        }
        public void UpdateMapAreas(string mapId, string areas)
        {
            if (_editorStarted)
            {
                ExecuteJs("setMapAreas('{0}','{1}');", mapId, areas);
            }
        }
        public void SetUseMap(string imgId, string mapId)
        {
            if (_editorStarted)
            {
                ExecuteJs("setUseMap('{0}','{1}');", imgId, mapId);
            }
        }
        public void SetWebProperty(string name, string value)
        {
            if (_editorStarted)
            {
                ExecuteJs("setPropertyValue('{0}','{1}');", name, value);
            }
        }
        public string AppendArchiveFile(string name, string archiveFilePath)
        {
            if (_editorStarted)
            {
                object v = EvaluateJs("appendArchiveFile('{0}','{1}');", name, archiveFilePath);
                return v as string;
            }
            return string.Empty;
        }
        #endregion
        #region Webb Properties
        private string _address;
        public string Address { get { return _address; } }
        private bool _cangoback;
        public bool CanGoBack { get { return _cangoback; } }
        private bool _cangoforward;
        public bool CanGoForward { get { return _cangoforward; } }
        private bool _isLoading;
        public bool IsLoading { get { return _isLoading; } }
        #endregion
        #region Webb Methods
        protected virtual void OnDisplayOutput(string output)
        {
        }
        public void LoadUrl(string url)
        {
            _loadedUrl = url;
            var handler = this.UrlActivated;
            if (handler != null)
            {
                handler(this, url);
            }
        }
        #endregion
        #region IExampleView Remembers
        public event EventHandler ShowDevToolsActivated { add { } remove { } }

        public event EventHandler CloseDevToolsActivated { add { } remove { } }

        public event EventHandler ExitActivated { add { } remove { } }

        public event EventHandler UndoActivated { add { } remove { } }

        public event EventHandler RedoActivated { add { } remove { } }

        public event EventHandler CutActivated { add { } remove { } }

        public event EventHandler CopyActivated { add { } remove { } }

        public event EventHandler PasteActivated { add { } remove { } }

        public event EventHandler DeleteActivated { add { } remove { } }

        public event EventHandler SelectAllActivated { add { } remove { } }

        public event EventHandler TestResourceLoadActivated { add { } remove { } }

        public event EventHandler TestSchemeLoadActivated { add { } remove { } }

        public event EventHandler TestExecuteScriptActivated { add { } remove { } }

        public event EventHandler TestEvaluateScriptActivated { add { } remove { } }

        public event EventHandler TestBindActivated { add { } remove { } }

        public event EventHandler TestConsoleMessageActivated { add { } remove { } }

        public event EventHandler TestTooltipActivated { add { } remove { } }

        public event EventHandler TestPopupActivated { add { } remove { } }

        public event EventHandler TestLoadStringActivated { add { } remove { } }

        public event EventHandler TestCookieVisitorActivated { add { } remove { } }

        public event Action<object, string> UrlActivated;

        public event EventHandler BackActivated { add { } remove { } }

        public event EventHandler ForwardActivated { add { } remove { } }

        public void SetTitle(string title)
        {
            this.Text = title;
        }

        public void SetAddress(string address)
        {
            _address = address;
        }

        public void SetCanGoBack(bool can_go_back)
        {
            _cangoback = can_go_back;
        }

        public void SetCanGoForward(bool can_go_forward)
        {
            _cangoforward = can_go_forward;
        }

        public void SetIsLoading(bool is_loading)
        {
            _isLoading = is_loading;
        }

        public void ExecuteScript(string script)
        {
            web_view.ExecuteScript(script);
        }

        public object EvaluateScript(string script)
        {
            return web_view.EvaluateScript(script);
        }

        public void DisplayOutput(string output)
        {
            OnDisplayOutput(output);
        }
        #endregion
        #region ISkipWrite Members

        public bool SkipSerialize
        {
            get { return true; }
        }

        #endregion
        #region limnor studio js IDE
        private Timer _timer = null;
        public virtual void onLoadEditorFailed()
        {
            _editorLoaded = false;
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(delegate()
                {
                    if (_timer == null)
                    {
                        _timer = new Timer();
                        _timer.Interval = 300;
                        _timer.Enabled = false;
                        _timer.Tick += _timer_Tick;
                    }
                    _timer.Enabled = true;
                }
               ));
            }
            else
            {
                if (_timer == null)
                {
                    _timer = new Timer();
                    _timer.Interval = 300;
                    _timer.Enabled = false;
                    _timer.Tick += _timer_Tick;
                }
                _timer.Enabled = true;
            }
        }
        private int _nVheRetryCount = 0;
        void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Enabled = false;
            _timer = null;
            if (_nVheRetryCount == 0)
            {
                _nVheRetryCount++;
                this.LoadUrl(_loadedUrl);
            }
            else if (MessageBox.Show(this.FindForm(), "Do you want to re-try the loading of the Visual HTML Designer?", "Visual Html Designer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.LoadUrl(_loadedUrl);
            }
        }
        public virtual void onEditorStarted()
        {
            _editorLoaded = true;
            ExecuteJs("setTargetFolder('{0}');",_webPhysicalFolder.Replace('\\','/'));// Path.GetDirectoryName(_htmlFile).Replace('\\', '/'));
        }
        delegate void fnJsApiP2(Guid guid, string id);
        public virtual void onElementIdChanged(string guid, string id)
        {
            if (_page != null)
            {
                if (!string.IsNullOrEmpty(guid))
                {
                    Guid g = new Guid(guid);
                    if (g != Guid.Empty)
                    {
                        if (_page.InvokeRequired)
                        {
                            _page.Invoke(new fnJsApiP2(_page.OnElementIdChanged), g, id);
                        }
                        else
                        {
                            _page.OnElementIdChanged(new Guid(guid), id);
                        }
                    }
                }
            }
        }
        delegate void fnJsApiP3(string objStr, int x, int y);
        public virtual void onRightClickElement(string objStr, int x, int y)
        {
            if (_page != null)
            {
                if (_page.InvokeRequired)
                {
                    _page.Invoke(new fnJsApiP3(_page.OnRightClickElement), objStr, x, y);
                }
                else
                {
                    _page.OnRightClickElement(objStr, x, y);
                }
            }
        }
        delegate void fnJsApiP1(string objStr);
        public virtual void onElementSelected(string e)
        {
            if (_page != null)
            {
                if (_page.InvokeRequired)
                {
                    _page.Invoke(new fnJsApiP1(_page.OnSelectHtmlElement), e);
                }
                else
                {
                    _page.OnSelectHtmlElement(e);
                }
            }
        }
        #endregion


    }
    class limnorStudioJs
    {
        private readonly ILimnorStudioView _owner;
        public limnorStudioJs(ILimnorStudioView owner)
        {
            _owner = owner;
        }
        public void onLoadEditorFailed()
        {
            if (_owner != null)
            {
                _owner.onLoadEditorFailed();
            }
        }
        public void onEditorStarted()
        {
            if (_owner != null)
            {
                _owner.onEditorStarted();
            }
        }
        public void onElementIdChanged(string guid, string id)
        {
            if (_owner != null)
            {
                _owner.onElementIdChanged(guid, id);
            }
        }
        public void onRightClickElement(string objStr, int x, int y)
        {
            if (_owner != null)
            {
                _owner.onRightClickElement(objStr, x, y);
            }
        }
        public void onElementSelected(string e)
        {
            if (_owner != null)
            {
                _owner.onElementSelected(e);
            }
        }
    }

}
