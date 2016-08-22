using System;
using System.Runtime.InteropServices;
using System.Collections;

namespace mshtml
{
    [Guid("3050F3EE-98B5-11CF-BB82-00AA00BDCE0B")]
    [TypeLibType(4160)]
    public interface IHTMLFiltersCollection : IEnumerable
    {
        [DispId(1001)]
        int length { get; }

        [DispId(-4)]
        [TypeLibFunc(65)]
        IEnumerator GetEnumerator();
        [DispId(0)]
        object item(ref object pvarIndex);
    }
    [Guid("3050F25E-98B5-11CF-BB82-00AA00BDCE0B")]
    [TypeLibType(4160)]
    public interface IHTMLStyle
    {
        [DispId(-2147413080)]
        string background { get; set; }
        [DispId(-2147413067)]
        string backgroundAttachment { get; set; }
        [DispId(-501)]
        object backgroundColor { get; set; }
        [DispId(-2147413111)]
        string backgroundImage { get; set; }
        [DispId(-2147413066)]
        string backgroundPosition { get; set; }
        [DispId(-2147413079)]
        object backgroundPositionX { get; set; }
        [DispId(-2147413078)]
        object backgroundPositionY { get; set; }
        [DispId(-2147413068)]
        string backgroundRepeat { get; set; }
        [DispId(-2147413063)]
        string border { get; set; }
        [DispId(-2147413060)]
        string borderBottom { get; set; }
        [DispId(-2147413055)]
        object borderBottomColor { get; set; }
        [DispId(-2147413045)]
        string borderBottomStyle { get; set; }
        [DispId(-2147413050)]
        object borderBottomWidth { get; set; }
        [DispId(-2147413058)]
        string borderColor { get; set; }
        [DispId(-2147413059)]
        string borderLeft { get; set; }
        [DispId(-2147413054)]
        object borderLeftColor { get; set; }
        [DispId(-2147413044)]
        string borderLeftStyle { get; set; }
        [DispId(-2147413049)]
        object borderLeftWidth { get; set; }
        [DispId(-2147413061)]
        string borderRight { get; set; }
        [DispId(-2147413056)]
        object borderRightColor { get; set; }
        [DispId(-2147413046)]
        string borderRightStyle { get; set; }
        [DispId(-2147413051)]
        object borderRightWidth { get; set; }
        [DispId(-2147413048)]
        string borderStyle { get; set; }
        [DispId(-2147413062)]
        string borderTop { get; set; }
        [DispId(-2147413057)]
        object borderTopColor { get; set; }
        [DispId(-2147413047)]
        string borderTopStyle { get; set; }
        [DispId(-2147413052)]
        object borderTopWidth { get; set; }
        [DispId(-2147413053)]
        string borderWidth { get; set; }
        [DispId(-2147413096)]
        string clear { get; set; }
        [DispId(-2147413020)]
        string clip { get; set; }
        [DispId(-2147413110)]
        object color { get; set; }
        [DispId(-2147413013)]
        string cssText { get; set; }
        [DispId(-2147413010)]
        string cursor { get; set; }
        [DispId(-2147413041)]
        string display { get; set; }
        [DispId(-2147413030)]
        string filter { get; set; }
        [DispId(-2147413071)]
        string font { get; set; }
        [DispId(-2147413094)]
        string fontFamily { get; set; }
        [DispId(-2147413093)]
        object fontSize { get; set; }
        [DispId(-2147413088)]
        string fontStyle { get; set; }
        [DispId(-2147413087)]
        string fontVariant { get; set; }
        [DispId(-2147413085)]
        string fontWeight { get; set; }
        [DispId(-2147418106)]
        object height { get; set; }
        [DispId(-2147418109)]
        object left { get; set; }
        [DispId(-2147413104)]
        object letterSpacing { get; set; }
        [DispId(-2147413106)]
        object lineHeight { get; set; }
        [DispId(-2147413037)]
        string listStyle { get; set; }
        [DispId(-2147413038)]
        string listStyleImage { get; set; }
        [DispId(-2147413039)]
        string listStylePosition { get; set; }
        [DispId(-2147413040)]
        string listStyleType { get; set; }
        [DispId(-2147413076)]
        string margin { get; set; }
        [DispId(-2147413073)]
        object marginBottom { get; set; }
        [DispId(-2147413072)]
        object marginLeft { get; set; }
        [DispId(-2147413074)]
        object marginRight { get; set; }
        [DispId(-2147413075)]
        object marginTop { get; set; }
        [DispId(-2147413102)]
        string overflow { get; set; }
        [DispId(-2147413101)]
        string padding { get; set; }
        [DispId(-2147413098)]
        object paddingBottom { get; set; }
        [DispId(-2147413097)]
        object paddingLeft { get; set; }
        [DispId(-2147413099)]
        object paddingRight { get; set; }
        [DispId(-2147413100)]
        object paddingTop { get; set; }
        [DispId(-2147413034)]
        string pageBreakAfter { get; set; }
        [DispId(-2147413035)]
        string pageBreakBefore { get; set; }
        [DispId(-2147414109)]
        int pixelHeight { get; set; }
        [DispId(-2147414111)]
        int pixelLeft { get; set; }
        [DispId(-2147414112)]
        int pixelTop { get; set; }
        [DispId(-2147414110)]
        int pixelWidth { get; set; }
        [DispId(-2147414105)]
        float posHeight { get; set; }
        [DispId(-2147413022)]
        string position { get; }
        [DispId(-2147414107)]
        float posLeft { get; set; }
        [DispId(-2147414108)]
        float posTop { get; set; }
        [DispId(-2147414106)]
        float posWidth { get; set; }
        [DispId(-2147413042)]
        string styleFloat { get; set; }
        [DispId(-2147418040)]
        string textAlign { get; set; }
        [DispId(-2147413077)]
        string textDecoration { get; set; }
        [DispId(-2147413090)]
        bool textDecorationBlink { get; set; }
        [DispId(-2147413092)]
        bool textDecorationLineThrough { get; set; }
        [DispId(-2147413089)]
        bool textDecorationNone { get; set; }
        [DispId(-2147413043)]
        bool textDecorationOverline { get; set; }
        [DispId(-2147413091)]
        bool textDecorationUnderline { get; set; }
        [DispId(-2147413105)]
        object textIndent { get; set; }
        [DispId(-2147413108)]
        string textTransform { get; set; }
        [DispId(-2147418108)]
        object top { get; set; }
        [DispId(-2147413064)]
        object verticalAlign { get; set; }
        [DispId(-2147413032)]
        string visibility { get; set; }
        [DispId(-2147413036)]
        string whiteSpace { get; set; }
        [DispId(-2147418107)]
        object width { get; set; }
        [DispId(-2147413065)]
        object wordSpacing { get; set; }
        [DispId(-2147413021)]
        object zIndex { get; set; }

        [DispId(-2147417610)]
        object getAttribute(string strAttributeName, int lFlags);
        [DispId(-2147417609)]
        bool removeAttribute(string strAttributeName, int lFlags);
        [DispId(-2147417611)]
        void setAttribute(string strAttributeName, object AttributeValue, int lFlags);
        [DispId(-2147414104)]
        string toString();
    }
    [TypeLibType(4160)]
    [Guid("3050F1FF-98B5-11CF-BB82-00AA00BDCE0B")]
    public interface IHTMLElement
    {
        [DispId(-2147417074)]
        object all { get; }
        [DispId(-2147417075)]
        object children { get; }
        [DispId(-2147417111)]
        string className { get; set; }
        [DispId(-2147417094)]
        object document { get; }
        [DispId(-2147417077)]
        IHTMLFiltersCollection filters { get; }
        [DispId(-2147417110)]
        string id { get; set; }
        [DispId(-2147417086)]
        string innerHTML { get; set; }
        [DispId(-2147417085)]
        string innerText { get; set; }
        [DispId(-2147417078)]
        bool isTextEdit { get; }
        [DispId(-2147413103)]
        string lang { get; set; }
        [DispId(-2147413012)]
        string language { get; set; }
        [DispId(-2147417101)]
        int offsetHeight { get; }
        [DispId(-2147417104)]
        int offsetLeft { get; }
        [DispId(-2147417100)]
        IHTMLElement offsetParent { get; }
        [DispId(-2147417103)]
        int offsetTop { get; }
        [DispId(-2147417102)]
        int offsetWidth { get; }
        [DispId(-2147412090)]
        object onafterupdate { get; set; }
        [DispId(-2147412091)]
        object onbeforeupdate { get; set; }
        [DispId(-2147412104)]
        object onclick { get; set; }
        [DispId(-2147412071)]
        object ondataavailable { get; set; }
        [DispId(-2147412072)]
        object ondatasetchanged { get; set; }
        [DispId(-2147412070)]
        object ondatasetcomplete { get; set; }
        [DispId(-2147412103)]
        object ondblclick { get; set; }
        [DispId(-2147412077)]
        object ondragstart { get; set; }
        [DispId(-2147412074)]
        object onerrorupdate { get; set; }
        [DispId(-2147412069)]
        object onfilterchange { get; set; }
        [DispId(-2147412099)]
        object onhelp { get; set; }
        [DispId(-2147412107)]
        object onkeydown { get; set; }
        [DispId(-2147412105)]
        object onkeypress { get; set; }
        [DispId(-2147412106)]
        object onkeyup { get; set; }
        [DispId(-2147412110)]
        object onmousedown { get; set; }
        [DispId(-2147412108)]
        object onmousemove { get; set; }
        [DispId(-2147412111)]
        object onmouseout { get; set; }
        [DispId(-2147412112)]
        object onmouseover { get; set; }
        [DispId(-2147412109)]
        object onmouseup { get; set; }
        [DispId(-2147412093)]
        object onrowenter { get; set; }
        [DispId(-2147412094)]
        object onrowexit { get; set; }
        [DispId(-2147412075)]
        object onselectstart { get; set; }
        [DispId(-2147417084)]
        string outerHTML { get; set; }
        [DispId(-2147417083)]
        string outerText { get; set; }
        [DispId(-2147418104)]
        IHTMLElement parentElement { get; }
        [DispId(-2147417080)]
        IHTMLElement parentTextEdit { get; }
        [DispId(-2147417087)]
        object recordNumber { get; }
        [DispId(-2147417088)]
        int sourceIndex { get; }
        [DispId(-2147418038)]
        IHTMLStyle style { get; }
        [DispId(-2147417108)]
        string tagName { get; }
        [DispId(-2147418043)]
        string title { get; set; }

        [DispId(-2147417079)]
        void click();
        [DispId(-2147417092)]
        bool contains(IHTMLElement pChild);
        [DispId(-2147417610)]
        object getAttribute(string strAttributeName, int lFlags);
        [DispId(-2147417082)]
        void insertAdjacentHTML(string where, string html);
        [DispId(-2147417081)]
        void insertAdjacentText(string where, string text);
        [DispId(-2147417609)]
        bool removeAttribute(string strAttributeName, int lFlags);
        [DispId(-2147417093)]
        void scrollIntoView(object varargStart);
        [DispId(-2147417611)]
        void setAttribute(string strAttributeName, object AttributeValue, int lFlags);
        [DispId(-2147417076)]
        string toString();
    }

    [Guid("3050F485-98B5-11CF-BB82-00AA00BDCE0B")]
    [TypeLibType(4160)]
    public interface IHTMLDocument3
    {
        [DispId(1080)]
        string baseUrl { get; set; }
        [DispId(-2147417063)]
        object childNodes { get; }
        [DispId(-2147412995)]
        string dir { get; set; }
        [DispId(1075)]
        IHTMLElement documentElement { get; }
        [DispId(1079)]
        bool enableDownload { get; set; }
        [DispId(1082)]
        bool inheritStyleSheets { get; set; }
        [DispId(-2147412043)]
        object onbeforeeditfocus { get; set; }
        [DispId(-2147412048)]
        object oncellchange { get; set; }
        [DispId(-2147412047)]
        object oncontextmenu { get; set; }
        [DispId(-2147412071)]
        object ondataavailable { get; set; }
        [DispId(-2147412072)]
        object ondatasetchanged { get; set; }
        [DispId(-2147412070)]
        object ondatasetcomplete { get; set; }
        [DispId(-2147412065)]
        object onpropertychange { get; set; }
        [DispId(-2147412050)]
        object onrowsdelete { get; set; }
        [DispId(-2147412049)]
        object onrowsinserted { get; set; }
        [DispId(-2147412044)]
        object onstop { get; set; }
        [DispId(1078)]
        System.Design.NativeMethods.IHTMLDocument2 parentDocument { get; }
        [DispId(1077)]
        string uniqueID { get; }

        [DispId(-2147417605)]
        bool attachEvent(string @event, object pdisp);
        [DispId(1076)]
        System.Design.NativeMethods.IHTMLDocument2 createDocumentFragment();
        [DispId(1074)]
        System.Design.NativeMethods.IHTMLDOMNode createTextNode(string text);
        [DispId(-2147417604)]
        void detachEvent(string @event, object pdisp);
        [DispId(1088)]
        IHTMLElement getElementById(string v);
        [DispId(1086)]
        System.Design.NativeMethods.IHTMLElementCollection getElementsByName(string v);
        [DispId(1087)]
        System.Design.NativeMethods.IHTMLElementCollection getElementsByTagName(string v);
        [DispId(1073)]
        void recalc(bool fForce);
        [DispId(1072)]
        void releaseCapture();
    }
}
