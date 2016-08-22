using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using XmlUtility;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Collections.Specialized;
using System.IO;

namespace Limnor.WebBuilder
{
    public static class WebPageCompilerUtility
    {
        public static string GetMethodParameterValueString(string p, StringCollection code)
        {
            string x = null;
            if (p == null)
            {
                x = "null";
            }
            else if (string.IsNullOrEmpty(p))
            {
                x = "''";
            }
            else
            {
                if (p.StartsWith("'", StringComparison.Ordinal))
                {
                    x = p;
                }
                else
                {
                    x = string.Format(CultureInfo.InvariantCulture, "v{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
                    code.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = {1};\r\n", x, p));
                }
            }
            return x;
        }
        public static string GetMethodParameterValueInt(string p, StringCollection code)
        {
            string x = null;
            int n;
            if (int.TryParse(p, out n))
            {
                x = p;
            }
            else
            {
                x = string.Format(CultureInfo.InvariantCulture, "v{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
                code.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = {1};\r\n", x, p));
            }
            return x;
        }
        public static string GetMethodParameterValueBool(string p, StringCollection code)
        {
            string x = null;
            if (string.IsNullOrEmpty(p))
            {
                x = "false";
            }
            else
            {
                if (string.Compare(p, "true", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    x = "true";
                }
                else if (string.Compare(p, "fals", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    x = "false";
                }
                else
                {
                    int n;
                    if (int.TryParse(p, out n))
                    {
                        if (n == 0)
                        {
                            x = "false";
                        }
                        else
                        {
                            x = "true";
                        }
                    }
                    else
                    {
                        x = string.Format(CultureInfo.InvariantCulture, "v{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
                        code.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = {1};\r\n", x, p));
                    }
                }
            }
            return x;
        }
        public static bool CreateActionJavaScript(string element, string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
        {
            if (string.CompareOrdinal(methodName, "focus") == 0)
            {
                code.Add(string.Format(CultureInfo.InvariantCulture,
                    "document.getElementById('{0}').focus();\r\n", element));
                return true;
            }
            return false;
        }
        public static void AddWebControlProperties(StringCollection _propertyNames)
        {
            _propertyNames.Add("Location");
            _propertyNames.Add("Size");
            _propertyNames.Add("Left");
            _propertyNames.Add("Top");
            _propertyNames.Add("Width");
            _propertyNames.Add("Height");
            _propertyNames.Add("ClientSize");
            _propertyNames.Add("ClientRectangle");
            _propertyNames.Add("Bounds");
            _propertyNames.Add("PositionAnchor");
            _propertyNames.Add("PositionAlignment");
            _propertyNames.Add("RightToLeft");
            _propertyNames.Add("zOrder");
            _propertyNames.Add("textAlign");
            _propertyNames.Add("Overflow");
        }
        public static string MapJavaScriptCodeName(string name)
        {
            if (string.CompareOrdinal(name, "BackgroundImageFile") == 0)
            {
                return "style.backgroundImage";
            }
            else if (string.CompareOrdinal(name, "BackColor") == 0)
            {
                return "style.backgroundColor";
            }
            else if (string.CompareOrdinal(name, "Visible") == 0)
            {
                return "style.display";
            }
            else if (string.CompareOrdinal(name, "Text") == 0)
            {
                return "value";
            }
            if (string.CompareOrdinal(name, "BackColor") == 0)
            {
                return "style.backgroundColor";
            }
            if (string.CompareOrdinal(name, "ForeColor") == 0)
            {
                return "style.color";
            }
            else if (string.CompareOrdinal(name, "Left") == 0)
            {
                return "style.x";
            }
            else if (string.CompareOrdinal(name, "Top") == 0)
            {
                return "style.y";
            }
            else if (string.CompareOrdinal(name, "Width") == 0)
            {
                return "style.width";
            }
            else if (string.CompareOrdinal(name, "Height") == 0)
            {
                return "style.height";
            }
            if (string.CompareOrdinal(name, "Cursor") == 0)
            {
                return "style.cursor";
            }
            if (string.CompareOrdinal(name, "Float") == 0)
            {
                return "style.cssFloat";
            }
            if (string.CompareOrdinal(name, "RightToLeft") == 0)
            {
                return "dir";
            }
            if (string.CompareOrdinal(name, "zOrder") == 0)
            {
                return "style.zIndex";
            }
            if (string.CompareOrdinal(name, "textAlign") == 0)
            {
                return "style.textAlign";
            }
            if (string.CompareOrdinal(name, "Overflow") == 0)
            {
                return "style.overflow";
            }
            return null;
        }
        public static string MapJavaScriptVallue(string name, string value, List<WebResourceFile> resourceFiles)
        {
            if (string.CompareOrdinal(name, "BackgroundImageFile") == 0)
            {
                StringBuilder sb;
                if (string.IsNullOrEmpty(value))
                {
                    return "'none'";
                }
                if (value.StartsWith("'", StringComparison.Ordinal))
                {
                    string sf = value.Substring(1);
                    if (sf.EndsWith("'", StringComparison.Ordinal))
                    {
                        if (sf.Length == 1)
                        {
                            sf = "";
                        }
                        else
                        {
                            sf = sf.Substring(0, sf.Length - 1);
                        }
                    }
                    if (string.IsNullOrEmpty(sf))
                    {
                        return "'none'";
                    }
                    if (File.Exists(sf))
                    {
                        resourceFiles.Add(new WebResourceFile(sf, WebResourceFile.WEBFOLDER_Images));
                        sb = new StringBuilder();
                        sb.Append("\"url('");
                        sb.Append(WebResourceFile.WEBFOLDER_Images);
                        sb.Append("/");
                        sb.Append(Path.GetFileName(sf));
                        sb.Append("')\"");
                        return sb.ToString();
                    }
                }
                sb = new StringBuilder();
                sb.Append("\"url('\" + ");
                sb.Append(value);
                sb.Append(" + \"')\"");
                return sb.ToString();
                //return string.Format(CultureInfo.InvariantCulture, "\"url({0})\"", value);
            }
            else if (string.CompareOrdinal(name, "Visible") == 0)
            {
                if (string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return "'none'";
                }
                if (string.Compare(value, "true", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return "'block'";
                }
                if (string.IsNullOrEmpty(value))
                    return "''";
                return string.Format(CultureInfo.InvariantCulture,"(({0})?\"block\":\"none\")", value);
            }
            if (string.CompareOrdinal(name, "RightToLeft") == 0)
            {
                if (string.Compare(value, "Yes", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return "'RTL'";
                }
                if (string.Compare(value, "No", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return "'LTR'";
                }
                if (string.IsNullOrEmpty(value))
                    return "''";
                return string.Format(CultureInfo.InvariantCulture, "(({0})?\"RTL\":\"LTR\")", value);
            }
            if (string.CompareOrdinal(name, "disabled") == 0)
            {
                return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.isValueTrue({0})", value);
            }
            return null;
        }
        public static MethodInfo[] GetWebClientMethods(Type t, bool isStatic)
        {
            List<MethodInfo> lst = new List<MethodInfo>();
            BindingFlags flags;
            if (isStatic)
            {
                flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static;
            }
            else
            {
                flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
            }
            MethodInfo[] ret = t.GetMethods(flags);
            if (ret != null && ret.Length > 0)
            {
                for (int i = 0; i < ret.Length; i++)
                {
                    if (!ret[i].IsSpecialName)
                    {
                        object[] objs = ret[i].GetCustomAttributes(typeof(WebClientMemberAttribute), true);
                        if (objs != null && objs.Length > 0)
                        {
                            lst.Add(ret[i]);
                        }
                    }
                }
            }
            ret = lst.ToArray();
            return ret;
        }
        public static void WriteDataBindings(XmlNode node, ControlBindingsCollection DataBindings, Dictionary<string,string> nameMap)
        {
            if (DataBindings != null && DataBindings.Count > 0)
            {
                bool first = true;
                StringBuilder dbs = new StringBuilder();
                for (int i = 0; i < DataBindings.Count; i++)
                {
                    if (DataBindings[i].DataSource != null)
                    {
                        if (nameMap != null && nameMap.ContainsKey(DataBindings[i].PropertyName))
                        {
                            if (string.IsNullOrEmpty(nameMap[DataBindings[i].PropertyName]))
                            {
                                continue;
                            }
                        }
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            dbs.Append(";");
                        }
                        dbs.Append(DataBindings[i].BindingMemberInfo.BindingPath);
                        dbs.Append(":");
                        dbs.Append(DataBindings[i].BindingMemberInfo.BindingField);
                        dbs.Append(":");
                        if (nameMap != null && nameMap.ContainsKey(DataBindings[i].PropertyName))
                        {
                            dbs.Append(nameMap[DataBindings[i].PropertyName]);
                        }
                        else
                        {
                            dbs.Append(DataBindings[i].PropertyName);
                        }
                    }
                }
                if (!first) //has data binding
                {
                    XmlUtil.SetAttribute(node, "jsdb", dbs.ToString());
                }
            }
        }

        public static string GetFontStyleString(Font f)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("font-family:");
            sb.Append(f.FontFamily.Name);
            sb.Append("; ");
            //
            sb.Append("font-size:");
            sb.Append(f.SizeInPoints.ToString(CultureInfo.InvariantCulture));
            sb.Append("pt; ");
            //
            if (f.Italic)
            {
                sb.Append("font-style:italic; ");
            }
            if (f.Bold)
            {
                sb.Append("font-weight:bold; ");
            }
            if (f.Underline || f.Strikeout)
            {
                sb.Append("text-decoration:");
                if (f.Underline)
                {
                    sb.Append("underline ");
                }
                if (f.Strikeout)
                {
                    sb.Append("line-through ");
                }
                sb.Append("; ");
            }
            //
            return sb.ToString();
        }
        public static void SetTextContentsToNode(XmlNode node, string text)
        {
            string txt = text.Replace("\r", "");
            string[] ss = txt.Split('\n');
            for (int i = 0; i < ss.Length; i++)
            {
                XmlNode ntxt = node.OwnerDocument.CreateTextNode(ss[i]);
                node.AppendChild(ntxt);
                if (i < ss.Length - 1)
                {
                    XmlNode nbr = node.OwnerDocument.CreateElement("br");
                    node.AppendChild(nbr);
                }
            }
        }
        public static void CreateWebElementZOrder(int zorder, StringBuilder sb)
        {
            if (zorder > 0)
            {
                sb.Append(string.Format(CultureInfo.InvariantCulture, "z-index:{0}; ", zorder));
            }
        }
        public static void CreateWebElementCursor(Cursor c, StringBuilder sb, bool defaultCursor)
        {
            string cr = GetJavascriptCursorValue(c, defaultCursor);
            if (!string.IsNullOrEmpty(cr))
            {
                sb.Append(string.Format(CultureInfo.InvariantCulture, "cursor:{0}; ", cr));
            }
            //if (c == Cursors.Default)
            //{
            //    if (defaultCursor)
            //    {
            //        sb.Append("cursor:default; ");
            //    }
            //}
            //else if (c == Cursors.AppStarting)
            //{
            //    sb.Append("cursor:wait; ");
            //}
            //else if (c == Cursors.Cross)
            //{
            //    sb.Append("cursor:crosshair; ");
            //}
            //else if (c == Cursors.Hand)
            //{
            //    sb.Append("cursor:pointer; ");
            //}
            //else if (c == Cursors.Help)
            //{
            //    sb.Append("cursor:help; ");
            //}
            //else if (c == Cursors.SizeAll)
            //{
            //    sb.Append("cursor:move; ");
            //}
            //else if (c == Cursors.IBeam)
            //{
            //    sb.Append("cursor:text; ");
            //}
            //else if (c == Cursors.WaitCursor)
            //{
            //    sb.Append("cursor:wait; ");
            //}
            //else if (c == Cursors.SizeWE)
            //{
            //    sb.Append("cursor:e-resize; ");
            //}
            //else if (c == Cursors.SizeNESW)
            //{
            //    sb.Append("cursor:ne-resize; ");
            //}
            //else if (c == Cursors.SizeNWSE)
            //{
            //    sb.Append("cursor:nw-resize; ");
            //}
            //else if (c == Cursors.SizeNS)
            //{
            //    sb.Append("cursor:n-resize; ");
            //}
            //else if (c == Cursors.PanSE)
            //{
            //    sb.Append("cursor:se-resize; ");
            //}
            //else if (c == Cursors.PanSW)
            //{
            //    sb.Append("cursor:sw-resize; ");
            //}
            //else if (c == Cursors.PanSouth)
            //{
            //    sb.Append("cursor:s-resize; ");
            //}
            //else if (c == Cursors.PanWest)
            //{
            //    sb.Append("cursor:w-resize; ");
            //}
        }
        public static string GetJavascriptCursorValue(Cursor c, bool defaultCursor)
        {
            if (c == Cursors.Default)
            {
                if (defaultCursor)
                {
                    return "default";
                }
            }
            else if (c == Cursors.AppStarting)
            {
                return "wait";
            }
            else if (c == Cursors.Cross)
            {
                return "crosshair";
            }
            else if (c == Cursors.Hand)
            {
                return "pointer";
            }
            else if (c == Cursors.Help)
            {
                return "help";
            }
            else if (c == Cursors.SizeAll)
            {
                return "move";
            }
            else if (c == Cursors.IBeam)
            {
                return "text";
            }
            else if (c == Cursors.WaitCursor)
            {
                return "wait";
            }
            else if (c == Cursors.SizeWE)
            {
                return "e-resize";
            }
            else if (c == Cursors.SizeNESW)
            {
                return "ne-resize";
            }
            else if (c == Cursors.SizeNWSE)
            {
                return "nw-resize";
            }
            else if (c == Cursors.SizeNS)
            {
                return "n-resize";
            }
            else if (c == Cursors.PanSE)
            {
                return "se-resize";
            }
            else if (c == Cursors.PanSW)
            {
                return "sw-resize";
            }
            else if (c == Cursors.PanSouth)
            {
                return "s-resize";
            }
            else if (c == Cursors.PanWest)
            {
                return "w-resize";
            }
            return string.Empty;
        }
        public static void CreateElementAnchor(IWebClientControl c, XmlNode node)
        {
            bool bUseAnchor = false;
            StringBuilder sb = new StringBuilder();
            if ((c.PositionAnchor & AnchorStyles.Left) == AnchorStyles.Left)
            {
                sb.Append("left,");
            }
            if ((c.PositionAnchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
            {
                sb.Append("bottom,");
                bUseAnchor = true;
            }
            if ((c.PositionAnchor & AnchorStyles.Right) == AnchorStyles.Right)
            {
                sb.Append("right,");
                bUseAnchor = true;
            }
            if ((c.PositionAnchor & AnchorStyles.Top) == AnchorStyles.Top)
            {
                sb.Append("top,");
            }
            if (!bUseAnchor)
            {
                switch (c.PositionAlignment)
                {
                    case ContentAlignment.BottomCenter:
                        sb.Append("bottomcenter");
                        break;
                    case ContentAlignment.BottomLeft:
                        sb.Append("leftbottom");
                        break;
                    case ContentAlignment.BottomRight:
                        sb.Append("bottomright");
                        break;
                    case ContentAlignment.MiddleCenter:
                        sb.Append("center");
                        break;
                    case ContentAlignment.MiddleLeft:
                        sb.Append("leftcenter");
                        break;
                    case ContentAlignment.MiddleRight:
                        sb.Append("centerright");
                        break;
                    case ContentAlignment.TopCenter:
                        sb.Append("topcenter");
                        break;
                    case ContentAlignment.TopRight:
                        sb.Append("topright");
                        break;
                }
            }
            if (sb.Length > 0)
            {
                string anchor = sb.ToString();
                if (string.CompareOrdinal(anchor, "left,top,") != 0)
                {
                    XmlUtil.SetAttribute(node, "anchor", anchor);
                }
            }
        }
        public static void CreateElementPosition(Control c, StringBuilder sb, EnumWebElementPositionType positionType)
        {
            IWebClientControl wcc = c as IWebClientControl;
            if (wcc != null)
            {
                if (wcc.textAlign != EnumTextAlign.left)
                {
                    sb.Append(string.Format(CultureInfo.InvariantCulture,"text-align:{0};",wcc.textAlign));
                }
            }
            IScrollableWebControl sw = c as IScrollableWebControl;
            if (sw != null)
            {
                if (sw.Overflow != EnumOverflow.visible)
                {
                    sb.Append(string.Format(CultureInfo.InvariantCulture, "overflow:{0};", sw.Overflow));
                }
            }
            if (c.RightToLeft == RightToLeft.Yes)
            {
                sb.Append("direction:rtl;");
            }
            if (positionType != EnumWebElementPositionType.Auto)
            {
                if (positionType == EnumWebElementPositionType.Absolute)
                {
                    sb.Append("position: absolute; ");
                    //Form f = c.FindForm();
                    //if (f != null)
                    //{
                    //Point pointScreen = c.PointToScreen(c.Location);
                    //Point pointForm = f.PointToClient(pointScreen);
                    Point pointForm = c.Location;
                    sb.Append("left:");
                    sb.Append(pointForm.X.ToString(CultureInfo.InvariantCulture));
                    sb.Append("px; ");
                    //
                    sb.Append("top:");
                    sb.Append(pointForm.Y.ToString(CultureInfo.InvariantCulture));
                    sb.Append("px; ");
                    //}
                }
                else if (positionType == EnumWebElementPositionType.Relative)
                {
                    sb.Append("position: relative; ");
                    sb.Append("left:");
                    sb.Append(c.Left.ToString(CultureInfo.InvariantCulture));
                    sb.Append("px; ");
                    //
                    sb.Append("top:");
                    sb.Append(c.Top.ToString(CultureInfo.InvariantCulture));
                    sb.Append("px; ");
                }
                else if (positionType == EnumWebElementPositionType.FloatLeft)
                {
                    sb.Append("float:left; ");
                }
                else if (positionType == EnumWebElementPositionType.FloatRight)
                {
                    sb.Append("float:right; ");
                }
                else if (positionType == EnumWebElementPositionType.FloatCenter)
                {
                    sb.Append("float:center; ");
                }
            }
            IWebClientControl wc = c as IWebClientControl;
            if (wc != null)
            {
                if (wc.Opacity < 100)
                {
                    sb.Append(string.Format(CultureInfo.InvariantCulture, "opacity:{0};filter:alpha(opacity={1}); ",((double)wc.Opacity/(double)100).ToString("#.##",CultureInfo.InvariantCulture),wc.Opacity));
                }
            }
        }

        public static string GetColorString(Color c)
        {
            if (c == Color.Transparent)
            {
                return "transparent";
            }
            string s1 = c.R.ToString("x2", CultureInfo.InvariantCulture);
            string s2 = c.G.ToString("x2", CultureInfo.InvariantCulture);
            string s3 = c.B.ToString("x2", CultureInfo.InvariantCulture);
            return string.Format(CultureInfo.InvariantCulture, "#{0}{1}{2}", s1, s2, s3);
        }

    }
}
