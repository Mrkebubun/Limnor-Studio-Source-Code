using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace LimnorWebBrowser
{
    //class DWebBrowserEvents2Class : DWebBrowserEvents2
    //{
    //    WebBrowserControl _webBrowser;
    //    public DWebBrowserEvents2Class()
    //    {
    //    }


    //    public DWebBrowserEvents2Class(WebBrowserControl browser)
    //    {
    //        _webBrowser = browser;
    //    }

    //    #region DWebBrowserEvents2 Members

    //    //Implement whichever events you wish
    //    public void BeforeNavigate2(object pDisp, ref object URL, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel)
    //    {
    //        Uri urlUri = new Uri(URL.ToString());

    //        string tFrame = null;
    //        if (targetFrameName != null)
    //        {
    //            tFrame = targetFrameName.ToString();
    //        }
    //        if (_webBrowser != null)
    //        {
    //            BrowserNavigationEventArgs args = new BrowserNavigationEventArgs(pDisp, urlUri, tFrame, EnumUrlContext.None);
    //            _webBrowser.OnStartNavigate(args);

    //            cancel = args.Cancel;
    //            pDisp = args.AutomationObject;
    //        }
    //    }
    //    //The NewWindow2 event, used on Windows XP SP1 and below
    //    public void NewWindow2(ref object pDisp, ref bool cancel)
    //    {
    //        BrowserNavigationEventArgs args = new BrowserNavigationEventArgs(pDisp, null, null, EnumUrlContext.None);
    //        _webBrowser.OnStartNewWindow(args);
    //        cancel = args.Cancel;
    //        pDisp = args.AutomationObject;
    //    }

    //    // NewWindow3 event, used on Windows XP SP2 and higher
    //    public void NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
    //    {
    //        BrowserNavigationEventArgs args = new BrowserNavigationEventArgs(ppDisp, new Uri(bstrUrl), null, (EnumUrlContext)dwFlags);
    //        _webBrowser.OnStartNewWindow(args);
    //        Cancel = args.Cancel;
    //        ppDisp = args.AutomationObject;
    //    }

    //    // Fired when downloading begins
    //    public void DownloadBegin()
    //    {
    //        _webBrowser.OnDownloading(EventArgs.Empty);
    //    }

    //    // Fired when downloading is completed
    //    public void DownloadComplete()
    //    {
    //        _webBrowser.OnDownloadComplete(EventArgs.Empty);
    //    }

    //    #region Unused events

    //    // This event doesn't fire. 
    //    [DispId(0x00000107)]
    //    public void WindowClosing(bool isChildWindow, ref bool cancel)
    //    {
    //    }

    //    public void OnQuit()
    //    {

    //    }

    //    public void StatusTextChange(string text)
    //    {
    //    }

    //    public void ProgressChange(int progress, int progressMax)
    //    {
    //    }

    //    public void TitleChange(string text)
    //    {
    //    }

    //    public void PropertyChange(string szProperty)
    //    {
    //    }

    //    public void NavigateComplete2(object pDisp, ref object URL)
    //    {
    //    }

    //    public void DocumentComplete(object pDisp, ref object URL)
    //    {
    //    }

    //    public void OnVisible(bool visible)
    //    {
    //    }

    //    public void OnToolBar(bool toolBar)
    //    {
    //    }

    //    public void OnMenuBar(bool menuBar)
    //    {
    //    }

    //    public void OnStatusBar(bool statusBar)
    //    {
    //    }

    //    public void OnFullScreen(bool fullScreen)
    //    {
    //    }

    //    public void OnTheaterMode(bool theaterMode)
    //    {
    //    }

    //    public void WindowSetResizable(bool resizable)
    //    {
    //    }

    //    public void WindowSetLeft(int left)
    //    {
    //    }

    //    public void WindowSetTop(int top)
    //    {
    //    }

    //    public void WindowSetWidth(int width)
    //    {
    //    }

    //    public void WindowSetHeight(int height)
    //    {
    //    }

    //    public void SetSecureLockIcon(int secureLockIcon)
    //    {
    //    }

    //    public void FileDownload(ref bool cancel)
    //    {
    //    }

    //    public void NavigateError(object pDisp, ref object URL, ref object frame, ref object statusCode, ref bool cancel)
    //    {
    //    }

    //    public void PrintTemplateInstantiation(object pDisp)
    //    {
    //    }

    //    public void PrintTemplateTeardown(object pDisp)
    //    {
    //    }

    //    public void UpdatePageStatus(object pDisp, ref object nPage, ref object fDone)
    //    {
    //    }

    //    public void PrivacyImpactedStateChange(bool bImpacted)
    //    {
    //    }

    //    public void CommandStateChange(int Command, bool Enable)
    //    {
    //    }

    //    public void ClientToHostWindow(ref int CX, ref int CY)
    //    {
    //    }
    //    #endregion

    //    #endregion
    //}
}
