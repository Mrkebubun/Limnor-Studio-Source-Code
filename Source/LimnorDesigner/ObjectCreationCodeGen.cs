using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Globalization;
using Limnor.WebBuilder;
using VPL;

namespace LimnorDesigner
{
    /// <summary>
    /// for code generator plug-ins
    /// </summary>
    public interface IGetObjectCreationCode
    {
        /// <summary>
        /// create a CodeExpression for an object
        /// </summary>
        /// <param name="v">the object to be generated in code</param>
        /// <returns>a CodeExpression for the object</returns>
        CodeExpression ObjectCreationCode(object v);
        /// <summary>
        /// create a C# code for an object
        /// </summary>
        /// <param name="v">the object to be generated in C# code</param>
        /// <returns>C# code snippet for the object</returns>
        string ObjectCreationCodeSnippetCSharp(object v);
        /// <summary>
        /// the Guid to uniquely identify this plug-in
        /// </summary>
        Guid Guid { get; }
    }
    /// <summary>
    /// code generator executer
    /// </summary>
    public static class ObjectCreationCodeGen
    {
        /// <summary>
        /// holder for the code generator plug-ins
        /// </summary>
        static Dictionary<Guid, IGetObjectCreationCode> creators;
        public static void AddCodeGenerator(IGetObjectCreationCode co)
        {
            if (creators == null)
            {
                creators = new Dictionary<Guid, IGetObjectCreationCode>();
            }
            if (creators.ContainsKey(co.Guid))
            {
                creators[co.Guid] = co;
            }
            else
            {
                creators.Add(co.Guid, co);
            }
        }
        public static void ClearCodeGenerator()
        {
            creators = null;
        }
        public static string ObjectCreateJavaScriptCode(object v)
        {
            if (v == null || v == System.DBNull.Value)
            {
                return "null";
            }
            Type t = v.GetType();
            if (typeof(bool).Equals(t))
            {
                bool c = Convert.ToBoolean(v);
                if (c)
                    return "true";
                else
                    return "false";
            }
            if (typeof(Color).Equals(t))
            {
                Color c = (Color)v;
                return string.Format(CultureInfo.InvariantCulture, "'{0}'", WebPageCompilerUtility.GetColorString(c));
            }
            if (typeof(Font).Equals(t))
            {
                Font ft = (Font)v;
                return string.Format(CultureInfo.InvariantCulture, "'{0}'", WebPageCompilerUtility.GetFontStyleString(ft));
            }
            if (typeof(Cursor).Equals(t))
            {
                return string.Format(CultureInfo.InvariantCulture, "'{0}'", WebPageCompilerUtility.GetJavascriptCursorValue((Cursor)v, true));
            }
            if (VPLUtil.IsNumber(t))
            {
                return v.ToString();
            }
            if (t.IsArray)
            {
                Array a = v as Array;
                StringBuilder sb = new StringBuilder("[");
                if (a.Length > 0)
                {
                    sb.Append(ObjectCreateJavaScriptCode(a.GetValue(0)));
                    for (int i = 1; i < a.Length; i++)
                    {
                        sb.Append(",");
                        sb.Append(ObjectCreateJavaScriptCode(a.GetValue(i)));
                    }
                }
                sb.Append("]");
                return sb.ToString();
            }
            return string.Format(CultureInfo.InvariantCulture, "'{0}'", v);
        }
        public static string ObjectCreatePhpScriptCode(object v)
        {
            if (v == null || v == System.DBNull.Value)
            {
                return "NULL";
            }
            Type t = v.GetType();
            //if (typeof(Color).Equals(t))
            //{
            //    Color c = (Color)v;
            //    return WebPage.GetColorString(c);
            //}
            //if (typeof(Font).Equals(t))
            //{
            //    Font ft = (Font)v;
            //    return WebPage.GetFontStyleString(ft);
            //}
            if (VPLUtil.IsNumber(t))
            {
                return v.ToString();
            }
            TypeConverter tc = TypeDescriptor.GetConverter(t);
            if (tc != null && tc.CanConvertTo(typeof(string)))
            {
                string vs = tc.ConvertToInvariantString(v);
                return string.Format(CultureInfo.InvariantCulture, "'{0}'", vs);
            }
            return string.Format(CultureInfo.InvariantCulture, "'{0}'", v);
        }
        /// <summary>
        /// create a CodeExpression to generate an object
        /// </summary>
        /// <param name="v">the object to be generated</param>
        /// <returns>the CodeExpression generating the object</returns>
        public static CodeExpression ObjectCreationCode(object v)
        {
            //===simple values===
            if (v == null)
                return new CodePrimitiveExpression(null);
            if (v == System.DBNull.Value)
                return new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(System.DBNull)), "Value");

            Type t = v.GetType();
            if (t.Equals(typeof(EventArgs)))
                return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EventArgs)), "Empty");
            if (t.IsPrimitive)
            {
                CodePrimitiveExpression cpi = new CodePrimitiveExpression(v);
                TypeCode tc = Type.GetTypeCode(t);
                return new CodeCastExpression(t, cpi);
            }
            if (t.Equals(typeof(string)))
            {
                return new CodePrimitiveExpression(v);
            }
            if (t.IsEnum)
            {
                return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(t), v.ToString());
            }
            if (t.IsArray)
            {
                Type te = t.GetElementType();
                Array a = (Array)v;
                if (a.Rank == 1)
                {
                    CodeExpression[] inits = new CodeExpression[a.Length];
                    for (int i = 0; i < inits.Length; i++)
                    {
                        object val = a.GetValue(i);
                        if (val == null)
                        {
                            inits[i] = CompilerUtil.GetDefaultValueExpression(te);
                        }
                        else
                        {
                            inits[i] = ObjectCreationCodeGen.ObjectCreationCode(val);
                        }
                    }
                    CodeArrayCreateExpression ac = new CodeArrayCreateExpression(t, inits);
                    return ac;
                }
                else
                {
                    StringBuilder sb = new StringBuilder(CompilerUtil.CreateArrayCreationCodeString(a.Rank, te.Name));
                    sb.Append("{");
                    for (int i = 0; i < a.Rank; i++)
                    {
                        if (i > 0)
                            sb.Append(",");
                        sb.Append("{");
                        for (int j = a.GetLowerBound(i); j <= a.GetUpperBound(i); j++)
                        {
                            if (j > 0)
                                sb.Append(",");
                            sb.Append(ObjectCreationCodeGen.GetObjectCreationCodeSnippet(a.GetValue(i, j)));
                        }
                        sb.Append("}");
                    }
                    sb.Append("}");
                    return new CodeSnippetExpression(sb.ToString());
                }
            }
            //===use plug-ins to handle it===
            if(creators != null)
            {
                foreach(IGetObjectCreationCode occ in creators.Values)
                {
                    CodeExpression ce = occ.ObjectCreationCode(v);
                    if(ce != null)
                    {
                        return ce;
                    }
                }
            }
            //===use default handlers===
            if (t.Equals(typeof(Size)))
            {
                Size v1 = (Size)v;
                return new CodeObjectCreateExpression(t,
                    new CodePrimitiveExpression(v1.Width),
                    new CodePrimitiveExpression(v1.Height));
            }
            if (t.Equals(typeof(Point)))
            {
                Point v1 = (Point)v;
                return new CodeObjectCreateExpression(t,
                    new CodePrimitiveExpression(v1.X),
                    new CodePrimitiveExpression(v1.Y));
            }
            if (t.Equals(typeof(Color)))
            {
                Color v1 = (Color)v;
                return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(Color)),"FromArgb"),
                    new CodeCastExpression(typeof(int), new CodePrimitiveExpression(v1.A)),
                    new CodeCastExpression(typeof(int), new CodePrimitiveExpression(v1.R)),
                    new CodeCastExpression(typeof(int), new CodePrimitiveExpression(v1.G)),
                    new CodeCastExpression(typeof(int), new CodePrimitiveExpression(v1.B))
                    );
            }
            if (t.Equals(typeof(Cursor)))
            {
                //System.Windows.Forms.Cursors.
                Cursor v1 = (Cursor)v;
                if (v1 == Cursors.AppStarting)
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "AppStarting");
                }
                else if (v1 == Cursors.Arrow)
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "Arrow");
                }
                else if (v1 == Cursors.Cross )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "Cross");
                }
                else if (v1 == Cursors.Default)
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "Default");
                }
                else if (v1 == Cursors.Hand)
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "Hand");
                }
                else if (v1 == Cursors.Help )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "Help");
                }
                else if (v1 == Cursors.HSplit )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "HSplit");
                }
                else if (v1 == Cursors.IBeam )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "IBeam");
                }
                else if (v1 == Cursors.No )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "No");
                }
                else if (v1 == Cursors.NoMove2D )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "NoMove2D");
                }
                else if (v1 == Cursors.NoMoveHoriz )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "NoMoveHoriz");
                }
                else if (v1 == Cursors.NoMoveVert )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "NoMoveVert");
                }
                else if (v1 == Cursors.PanEast )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "PanEast");
                }
                else if (v1 == Cursors.PanNE )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "PanNE");
                }
                else if (v1 == Cursors.PanNorth )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "PanNorth");
                }
                else if (v1 == Cursors.PanNW )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "PanNW");
                }
                else if (v1 == Cursors.PanSE )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "PanSE");
                }
                else if (v1 == Cursors.PanSouth )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "PanSouth");
                }
                else if (v1 == Cursors.PanSW )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "PanSW");
                }
                else if (v1 == Cursors.PanWest )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "PanWest");
                }
                else if (v1 == Cursors.SizeAll )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "SizeAll");
                }
                else if (v1 == Cursors.SizeNESW )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "SizeNESW");
                }
                else if (v1 == Cursors.SizeNS )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "SizeNS");
                }
                else if (v1 == Cursors.SizeNWSE )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "SizeNWSE");
                }
                else if (v1 == Cursors.SizeWE )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "SizeWE");
                }
                else if (v1 == Cursors.UpArrow )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "UpArrow");
                }
                else if (v1 == Cursors.VSplit )
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "VSplit");
                }
                else if (v1 == Cursors.WaitCursor)
                {
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Cursors)), "WaitCursor");
                }
                else
                {
                    CursorConverter cc = new CursorConverter();
                    string s = cc.ConvertToString(v);
                    return new CodeMethodInvokeExpression(
                        new CodeObjectCreateExpression(typeof(CursorConverter), null),
                        "ConvertFromString",
                        new CodePrimitiveExpression(s)
                        );
                    //new CursorConverter().ConvertFromString(s)
                }
            }
            if (t.Equals(typeof(Font)))
            {
                Font f = (Font)v;
                return new CodeObjectCreateExpression(typeof(Font), new CodePrimitiveExpression(f.FontFamily.ToString()), new CodePrimitiveExpression(f.Size));
            }
            //use string converter ===================================
            TypeConverter sc = TypeDescriptor.GetConverter(t);
            if (sc != null)
            {
                if (sc.CanConvertTo(typeof(string)) && sc.CanConvertFrom(typeof(string)))
                {
                    string svalue = sc.ConvertToInvariantString(v);
                    //TypeDescriptor.GetConverter(t).ConvertFromInvariantString(svalue)
                    CodeMethodInvokeExpression mgc = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(TypeDescriptor)),"GetConverter",new CodeTypeOfExpression(t));
                    CodeMethodInvokeExpression fs = new CodeMethodInvokeExpression(mgc, "ConvertFromInvariantString", new CodePrimitiveExpression(svalue));
                    //
                    return new CodeCastExpression(t, fs);
                }
            }
            //===use TypeConverter as the last resort===
            PropertyDescriptorCollection pifs = TypeDescriptor.GetProperties(v);
            if (pifs == null || pifs.Count == 0)
            {
                return new CodeObjectCreateExpression(t);
            }
            int nCount = 0;
            for (int i = 0; i < pifs.Count; i++)
            {
                if (pifs[i].IsBrowsable && !pifs[i].IsReadOnly)
                {
                    nCount++;
                }
            }
            if (nCount == 0)
            {
                return new CodeObjectCreateExpression(t);
            }
            string sCodeSnippet = GetObjectCreationCodeSnippet(v);
            return new CodeSnippetExpression(sCodeSnippet);
        }
        public static string GetObjectCreationCodeSnippet(object v)
        {
            //===simple values===
            if (v == null)
                return "null";
            if (v == System.DBNull.Value)
                return "System.DBNull.Value";

            Type t = v.GetType();
            if (t.Equals(typeof(EventArgs)))
                return "EventArgs.Empty";
            if (t.IsPrimitive)
            {
                return v.ToString();
            }
            if (t.Equals(typeof(string)))
            {
                return "\"" + (string)v + "\"";
            }
            //===use plug-ins to handle it===
            if (creators != null)
            {
                foreach (IGetObjectCreationCode occ in creators.Values)
                {
                    string s = occ.ObjectCreationCodeSnippetCSharp(v);
                    if (!string.IsNullOrEmpty(s))
                    {
                        return s;
                    }
                }
            }
            //use default handlings=====================
            if (t.Equals(typeof(Size)))
            {
                Size vx = (Size)v;
                return "new System.Drawing.Size(" + vx.Width.ToString() + "," + vx.Height.ToString() + ")";
            }
            if (t.Equals(typeof(Point)))
            {
                Point vx = (Point)v;
                return "new System.Drawing.Point(" + vx.X.ToString() + "," + vx.Y.ToString() + ")";
            }
            if (t.Equals(typeof(Color)))
            {
                Color v1 = (Color)v;
                return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "System.Drawing.Color.FromArgb(((int)(((byte)({0})))), ((int)(((byte)({1})))), ((int)(((byte)({2})))), ((int)(((byte)({3})))))",
                    v1.A,v1.R,v1.G,v1.B);
            }
            if (t.Equals(typeof(Font)))
            {
                Font f = (Font)v;
                return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "new Font({0},{1})",
                    f.FontFamily.ToString(), f.Size);
            }
            //use string converter ===================================
            TypeConverter sc = TypeDescriptor.GetConverter(t);
            if (sc != null)
            {
                if (sc.CanConvertTo(typeof(string)) && sc.CanConvertFrom(typeof(string)))
                {
                    string svalue = sc.ConvertToInvariantString(v);
                    //TypeDescriptor.GetConverter(t).ConvertFromInvariantString(svalue)
                    //CodeMethodInvokeExpression mgc = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(TypeDescriptor)), "GetConverter", new CodeTypeOfExpression(t));
                    //CodeMethodInvokeExpression fs = new CodeMethodInvokeExpression(mgc, "ConvertFromInvariantString", new CodePrimitiveExpression(svalue));
                    return "TypeDescriptor.GetConverter(Type.GetType(\"" + t.AssemblyQualifiedName + "\")).ConvertFromInvariantString(\"" + svalue + "\")";
                }
            }
            //===use TypeConverter as the last resort===
            PropertyDescriptorCollection pifs = TypeDescriptor.GetProperties(v);
            if (pifs == null || pifs.Count == 0)
            {
                return "new " + t.FullName + "()";
            }
            int nCount = 0;
            for (int i = 0; i < pifs.Count; i++)
            {
                if (pifs[i].IsBrowsable && !pifs[i].IsReadOnly)
                {
                    nCount++;
                }
            }
            if (nCount == 0)
            {
                return "new " + t.FullName + "()"; ;
            }
            StringBuilder sCodeSnippet = new StringBuilder(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "({0})System.ComponentModel.TypeDescriptor.GetConverter(typeof({0})).CreateInstance(new Dictionary<string, object>() ", t.FullName));
            sCodeSnippet.Append("{");
            bool bFirst = true;
            for (int i = 0; i < pifs.Count; i++)
            {
                if (pifs[i].IsBrowsable && !pifs[i].IsReadOnly)
                {
                    if (bFirst)
                    {
                        bFirst = false;
                    }
                    else
                    {
                        sCodeSnippet.Append(",");
                    }
                    sCodeSnippet.Append("{");
                    sCodeSnippet.Append("\""+pifs[i].Name+"\",");
                    sCodeSnippet.Append(GetObjectCreationCodeSnippet(pifs[i].GetValue(v)));
                    sCodeSnippet.Append("}");
                }
            }
            sCodeSnippet.Append("})");
            return sCodeSnippet.ToString();
        }
    }
}
