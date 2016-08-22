using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Limnor.Windows
{
    class WindowInfo
    {
        public WindowInfo(IntPtr hWnd)
        {
            Handle = hWnd;
            int pid;
            int tid = NativeMethods.GetWindowThreadProcessId(hWnd, out pid);
            Marshal.ThrowExceptionForHR(tid);
            ProcessId = pid;
            ThreadId = tid;
        }
        public int ThreadId { get; private set; }
        public int ProcessId { get;private set; }
        public IntPtr Handle { get;private set; }
    }
    class ListenerControl : Form
    {
        static int HM_MESSAGE_RESULT;
        private SubClassManager _subClass;
        public ListenerControl(SubClassManager subClass)
        {
            _subClass = subClass;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new System.Drawing.Size(1, 1);
            HM_MESSAGE_RESULT = NativeMethods.RegisterWindowMessage("Limnor_MessageResult");
            //Parent = Instance;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)NativeMethods.Msgs.WM_COPYDATA)
            {
                // message was sended by ProcessViewer.Hooks.dll from the Hooked application when
                // a new message comes to that window

                // LPARAM of this message contains a COPYDATASTRUCT which has HOOK_MSG struct in it
                NativeMethods.COPYDATASTRUCT cdata =
                    (NativeMethods.COPYDATASTRUCT)m.GetLParam(typeof(NativeMethods.COPYDATASTRUCT));
                // This is the information of the message which is sended to the hooked window
                NativeMethods.HOOK_MSG msg = (NativeMethods.HOOK_MSG)Marshal.PtrToStructure(cdata.lpData,
                    typeof(NativeMethods.HOOK_MSG));

                // process message and set its result (0 ignore, 1 do nothing, other values replace parameters)
                m.Result = _subClass.ProcessMessage(m.WParam, ref msg);
                Marshal.DestroyStructure(m.LParam, typeof(NativeMethods.COPYDATASTRUCT));
            }
            else if (m.Msg == HM_MESSAGE_RESULT)
            //&& Properties.Settings.Default.HandleMessageResults)
            {
                // this message sended by hooked window to give information about the result of a message
                _subClass.ProcessMessageResult(m.WParam.ToInt32(), m.LParam);
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
    class SubClassManager : IDisposable
    {
        Dictionary<NativeMethods.Msgs, string> msgValues = null;
        ListenerControl listener = null;
        bool _Enabled = true;
        bool _ShowMessages = true;
        //Dictionary<NativeMethods.Msgs, FormMain.MessageBreakpoint> _Breakpoints = null;
        WindowInfo _Window = null;
        WindowsManager _wm;
        public SubClassManager(WindowsManager owner)
        {
            _wm = owner;
            Type enumType = typeof(NativeMethods.Msgs);
            Array msgArray = Enum.GetValues(enumType);
            msgValues = new Dictionary<NativeMethods.Msgs, string>(msgArray.Length);
            for (int i = 0; i < msgArray.Length; i++)
            {
                NativeMethods.Msgs value = (NativeMethods.Msgs)msgArray.GetValue(i);
                string name = Enum.GetName(enumType, value);

                if (msgValues.ContainsKey(value))
                    msgValues[value] = name;
                else
                    msgValues.Add(value, name);
            }
            // create Listener control which we will use to interact with the hooked window
            listener = new ListenerControl(this);
            listener.Show();
            //_Breakpoints = new Dictionary<NativeMethods.Msgs, MessageBreakpoint>();
        }

        internal void ProcessMessageResult(int message, IntPtr result)
        {
            if (ShowMessages)
            {
                // write result of a message
                //StringBuilder text = new StringBuilder();
                //text.Append(Properties.Resources.Hook_Result);
                //NativeMethods.Msgs msg = (NativeMethods.Msgs)message;
                //if (msgValues.ContainsKey(msg))
                //    text.Append(msgValues[msg]);
                //else
                //    text.Append(msg);
                //text.Append(" -> " + result.ToInt32());
                //text.AppendLine();

                //Instance.txtMessages.AppendText(text.ToString());
            }
        }

        /// <summary>
        ///  This is also a very important method for us..
        ///  This method is invoke when we are going to change the parameters of the message
        ///  By one way or another we have to pass these new parameters to the hooked window
        ///  which is generally belongs to a different process.
        ///  
        ///  To resolve this problem. We are going to write our new values to the memory of that process
        ///  which ProcessViewer.Hooks.dll will read these values from there...
        /// </summary>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        IntPtr WriteToTargetProcessMemory(IntPtr wParam, IntPtr lParam, NativeMethods.Msgs modifiedMsg)
        {
            // Open hooked window's process
            IntPtr hProcess = NativeMethods.OpenProcess(NativeMethods.PROCESS_VM_OPERATION
                                                        | NativeMethods.PROCESS_VM_READ
                                                        | NativeMethods.PROCESS_VM_WRITE,
                                                false, _Window.ProcessId);
            // allocate memory from target proces's memory block
            // 12 bytes : 4 modified msg + 4 wParam + 4 lParam
            IntPtr memAddress = NativeMethods.VirtualAllocEx(hProcess, IntPtr.Zero,
                12, NativeMethods.MEM_COMMIT,
                NativeMethods.PAGE_READWRITE);
            if (memAddress == IntPtr.Zero)
                return IntPtr.Zero;

            int written = 0;
            // write our new parameter values to the tarhet process memory
            bool hr = NativeMethods.WriteProcessMemory(hProcess, memAddress,
                                        new int[] {(int)modifiedMsg, wParam.ToInt32(), 
														lParam.ToInt32()},
                                        12, out written);
            // close handle
            NativeMethods.CloseHandle(hProcess);

            if (!hr)
                return IntPtr.Zero;

            //if (Properties.Settings.Default.ShowChangedParameterValues)
            //{
            //    StringBuilder text = new StringBuilder();
            //    text.Append(Properties.Resources.Hook_Parameters_Changed);
            //    text.Append("MSG: ");
            //    text.Append(modifiedMsg.ToString());
            //    text.Append(", WParam: ");
            //    text.Append(wParam.ToInt32());
            //    text.Append(", LParam: ");
            //    text.Append(lParam.ToInt32());
            //    text.AppendLine();

            //    Instance.txtMessages.AppendText(text.ToString());
            //}

            return memAddress;
        }

        /// <summary>
        ///  Process message which belongs to the hooked window
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hookedMessage"></param>
        /// <returns>
        ///		This return value will be sended to the Hooked window by our Listener Control
        ///		
        ///		Return value might be;
        ///		0 : which means ignore this message (Hooked window will ignore this message)
        ///		1 : Do nothing (Hooked window will do its original action for this message)
        ///		<Other Values> : We are going to change the wParam and lParam for this message
        ///		here the return value indicates an adress in the target process memory which contains
        ///		the new values of the wParam and lParam
        ///		By passing this address to the Hooked window, we make it read the new values
        ///		from this address and replace original wParam and lParam with the new values
        ///		and process message according to this new parameters
        /// </returns>
        internal IntPtr ProcessMessage(IntPtr hWnd, ref NativeMethods.HOOK_MSG hookedMessage)
        {
            lock (this)
            {
                IntPtr result = IntPtr.Zero;
                NativeMethods.Msgs msg = (NativeMethods.Msgs)hookedMessage.msg;
                switch (msg)
                {
                    case NativeMethods.Msgs.WM_MOVE:
                        _wm.OnMove();
                        break;
                    case NativeMethods.Msgs.WM_SIZE:
                        _wm.OnResize();
                        break;
                }
                //if (Enabled && Breakpoints.ContainsKey(msg))
                //{
                //    // if Breakpoints enabled and if we have a breakpoint for the
                //    // given message then do our action
                //    MessageBreakpoint watch = Breakpoints[msg];
                //    switch (watch.Action)
                //    {
                //        case BreakpointAction.IgnoreMessage:
                //            result = new IntPtr(1);
                //            break;
                //        case BreakpointAction.ManuelEditParameters:
                //            FormEditMessage em = new FormEditMessage(hWnd, msg, hookedMessage.wParam, hookedMessage.lParam);
                //            em.ShowDialog(FormMain.Instance);
                //            if (!em.Ignore)
                //                result = WriteToTargetProcessMemory(em.WParam, em.LParam, em.ModifiedMsg);
                //            else
                //                result = new IntPtr(1);
                //            break;
                //        case BreakpointAction.AutoChangeParameters:
                //            NativeMethods.Msgs modifiedMsg = (NativeMethods.Msgs)hookedMessage.msg;
                //            IntPtr wParam = hookedMessage.wParam;
                //            IntPtr lParam = hookedMessage.lParam;

                //            if ((watch.Modifications & ModifiyingSections.Message) == ModifiyingSections.Message
                //                && watch.ModifiedMsg != NativeMethods.Msgs.WM_NULL)
                //                modifiedMsg = watch.ModifiedMsg;

                //            if ((watch.Modifications & ModifiyingSections.WParam) == ModifiyingSections.WParam)
                //                wParam = new IntPtr(watch.WParam.Value);

                //            if ((watch.Modifications & ModifiyingSections.LParam) == ModifiyingSections.LParam)
                //                lParam = new IntPtr(watch.LParam.Value);


                //            result = WriteToTargetProcessMemory(wParam, lParam, modifiedMsg);
                //            break;
                //    }
                //}

                if (ShowMessages)
                {
                    //StringBuilder text = new StringBuilder();
                    //text.AppendFormat("0x{0:X8}", hWnd.ToInt32());
                    //text.Append("  ");
                    //if (msgValues.ContainsKey(msg))
                    //    text.Append(msgValues[msg]);
                    //else
                    //    text.Append(msg);

                    //text.Append("  ");
                    //text.AppendFormat("wParam:{0}", hookedMessage.wParam);
                    //text.AppendFormat(" lParam:{0}", hookedMessage.lParam);
                    //if (result == (IntPtr)1)
                    //    text.Append(Properties.Resources.Hook_Ignored);
                    //text.AppendLine();

                    //FormMain.Instance.txtMessages.AppendText(text.ToString());
                }

                return result;
            }
        }

        /// <summary>
        ///  Begin Hook operation
        /// </summary>
        /// <param name="window"></param>
        public void Begin(WindowInfo window)
        {
            // first try to end (if exist any hook)
            End();

            // do not allow hook on PV's log messages window (might cause problems)
            //if (window.Handle == Instance.txtMessages.Handle)
            //    return;

            _Window = window;
            // Call StartHook method from our ProcessViewer,Hooks.dll
            int hhk;
            if(IntPtr.Size == 4)
                hhk = NativeMethods.StartHook(window.Handle, listener.Handle);
            else
                hhk = NativeMethods.StartHook64(window.Handle, listener.Handle);
            if (hhk == 0)
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            //FormMain.Instance.pnlMessages.Visible = true;
            //Instance.SetVerbsHeight();
        }

        /// <summary>
        ///  Ends hook operation
        /// </summary>
        public void End()
        {
            // Call EndHook method from our ProcessViewer.Hooks.dll
            if (IntPtr.Size == 4)
                NativeMethods.EndHook();
            else
                NativeMethods.EndHook64();
            //if (Properties.Settings.Default.ResetBreakpointsOnChange)
            //    Clear();

            //FormMain.Instance.pnlMessages.Visible = false;
            //FormMain.Instance.txtMessages.Text = string.Empty;
            //_Window = null;
            //Instance.SetVerbsHeight();
        }

        /// <summary>
        ///  Clears all breakpoints
        /// </summary>
        public void Clear()
        {
            //_Breakpoints.Clear();
        }

        //public Dictionary<NativeMethods.Msgs, FormMain.MessageBreakpoint> Breakpoints
        //{
        //    get { return _Breakpoints; }
        //}

        /// <summary>
        ///  Show Edit Breakpoints dialog
        /// </summary>
        //public void ShowDialog()
        //{
        //    bool flag = Enabled;
        //    Enabled = false;

        //    FormBreakpoints form = new FormBreakpoints(_Breakpoints);
        //    form.ShowDialog(FormMain.Instance);

        //    Enabled = flag;
        //}

        public void Dispose()
        {
            End();
            if (listener != null)
            {
                if (!listener.IsDisposed && !listener.Disposing)
                {
                    listener.Close();
                }
                listener.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        ~SubClassManager()
        {
            Dispose();
        }

        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled == value)
                    return;
                _Enabled = value;
                //if (value)
                //{
                //    FormMain.Instance.sbarWatcher.Text = Properties.Resources.Breakpoints_On;
                //    FormMain.Instance.btnWatcherOff.Checked = false;
                //    FormMain.Instance.btnWatcherOn.Checked = true;
                //}
                //else
                //{
                //    FormMain.Instance.sbarWatcher.Text = Properties.Resources.Breakpoints_Off;
                //    FormMain.Instance.btnWatcherOff.Checked = true;
                //    FormMain.Instance.btnWatcherOn.Checked = false;
                //}
                //Application.DoEvents();
            }
        }

        public bool ShowMessages
        {
            get { return _ShowMessages; }
            set
            {
                if (_ShowMessages == value)
                    return;
                _ShowMessages = value;
                //Instance.btnShowMessages.Checked = value;
                //Instance.showMessagesLogToolStripMenuItem.Checked = value;
                //if (!value)
                //    Instance.txtMessages.Text = string.Empty;
            }
        }
    }

}
