using System;
using System.Collections.Generic;
using System.Text;

namespace DirectShowLib
{
//class ATL_NO_VTABLE CErrorLog : public CComObjectRootEx<...>, public IErrorLog { ... }
    public class CErrorLog : IErrorLog
    {
        private static CErrorLog _log;
        private List<ErrorItem> _errors;
        public CErrorLog()
        {
        }
        static CErrorLog()
        {
            _log = new CErrorLog();
        }
        public void ClearErrorLog()
        {
            _errors = null;
        }
        public void AddError(string propertyName, string error)
        {
            System.Runtime.InteropServices.ComTypes.EXCEPINFO exp = new System.Runtime.InteropServices.ComTypes.EXCEPINFO();
            exp.bstrDescription = error;
            AddError(propertyName, exp);
        }
        public IList<ErrorItem> Errors
        {
            get
            {
                return _errors;
            }
        }
        
        public static CErrorLog ErrorLog
        {
            get
            {
                return _log;
            }
        }
        #region IErrorLog Members

        public int AddError(string pszPropName, System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo)
        {
            if (_errors == null)
            {
                _errors = new List<ErrorItem>();
            }
            _errors.Add(new ErrorItem(pszPropName, pExcepInfo));
            return _errors.Count;
        }

        #endregion
    }
    public class ErrorItem
    {
        private string _propertyName;
        private System.Runtime.InteropServices.ComTypes.EXCEPINFO _exception;
        private DateTime _time;
        public ErrorItem(string propertyname,System.Runtime.InteropServices.ComTypes.EXCEPINFO error)
        {
            _propertyName = propertyname;
            _exception = error;
            _time = DateTime.Now;
        }
        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
        }
        public System.Runtime.InteropServices.ComTypes.EXCEPINFO ExceptionInfo
        {
            get
            {
                return _exception;
            }
        }
        public DateTime Time
        {
            get
            {
                return _time;
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_time.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(" ");
            sb.Append(_propertyName);
            sb.Append(":");
            sb.Append(_exception.bstrDescription);
            sb.Append(". Help file:");
            if (!string.IsNullOrEmpty(_exception.bstrHelpFile))
            {
                sb.Append(_exception.bstrHelpFile);
            }
            sb.Append(". Source:");
            if (!string.IsNullOrEmpty(_exception.bstrSource))
            {
                sb.Append(_exception.bstrSource);
            }
            sb.Append(". Code:");
            sb.Append(_exception.scode.ToString(System.Globalization.CultureInfo.InvariantCulture));
            return sb.ToString();
        }
    }
}
