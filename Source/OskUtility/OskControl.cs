using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OskUtility
{
    [ToolboxBitmapAttribute(typeof(OskControl), "Resources.osk.bmp")]
    [Description("This object manages on screen keyboard.")]
    public partial class OskControl : UserControl
    {
        #region fields and constructors
        //private bool _oskLoaded;
        private string _errStr;
        private IntPtr _oskHandle;
        private Process _oskProcess;
        public OskControl()
        {
            InitializeComponent();
        }
        #endregion

        #region protected overrides
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //loadOsk();
        }
        #endregion

        #region private methods
        private bool findWindwo(IntPtr hWnd, IntPtr lParam)
        {
            int processID;
            int threadID = WindowsNative.GetWindowThreadProcessId(hWnd, out processID);
            if (_oskProcess.Id == processID)
            {
                _oskHandle = hWnd;
                return false;
            }
            return true;
        }
        private void loadOsk()
        {
            _errStr = string.Empty;
            if (_oskHandle == IntPtr.Zero)
            {
                string sys32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string oskFile = System.IO.Path.Combine(sys32, "osk.exe");
                if (!System.IO.File.Exists(oskFile))
                {
                    _errStr = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "On-screen-keyboad not found at {0}", oskFile);
                }
                else
                {
                    _oskProcess = new Process();
                    try
                    {
                        _oskProcess.StartInfo.FileName = oskFile;
                        _oskProcess.Exited += new EventHandler(_oskProcess_Exited);
                        _oskProcess.Start();
                        if (_oskProcess.WaitForInputIdle(3000))
                        {
                            if (_oskProcess.MainWindowHandle != IntPtr.Zero)
                            {
                                _oskHandle = _oskProcess.MainWindowHandle;
                            }
                            else
                            {
                                WindowsNative.EnumWindows(new EnumWindowsProc(findWindwo), 0);
                                if (_oskHandle == IntPtr.Zero)
                                {
                                    _errStr = "Handle for on-screen-keyboard not found";
                                }
                            }
                            if (_oskHandle != IntPtr.Zero)
                            {
                                WindowsNative.MoveWindow(_oskHandle, 0, 0);
                                WindowsNative.DisableCloseButton(_oskHandle);
                            }
                        }
                    }
                    catch (Exception er)
                    {
                        _errStr = er.Message;
                    }
                }
            }
        }

        void _oskProcess_Exited(object sender, EventArgs e)
        {
            _oskHandle = IntPtr.Zero;
        }
        #endregion

        #region Methods
        public void StartOsk()
        {
            loadOsk();
            if (_oskHandle == IntPtr.Zero)
            {
                MessageBox.Show(this.FindForm(), _errStr, "Load OSK", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void CloseOsk()
        {
            if (_oskHandle != IntPtr.Zero)
            {
                WindowsNative.CloseWindow2(_oskHandle);
                _oskHandle = IntPtr.Zero;
            }
        }
        #endregion

        #region Properties
        [Description("Gets a Boolean value indicating whether the on screen keyboard is loaded successfully")]
        public bool OskLoaded
        {
            get
            {
                return (_oskHandle != IntPtr.Zero && WindowsNative.IsWindow(_oskHandle));
            }
        }
        [Description("Gets a string value representing the error message for the last action")]
        public string ErrorMessage
        {
            get
            {
                return _errStr;
            }
        }
        #endregion
    }
}
