/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using VSPrj;
using LimnorKiosk;

namespace LimnorDesigner
{
	/// <summary>
	/// debug UI
	/// </summary>
	public partial class FormDebugger : Form
	{
		#region fields and constructors
		const int IMG_Run = 0;
		const int IMG_Run2 = 1;
		const int IMG_Pause = 2;
		const int IMG_Pause2 = 3;
		const int IMG_Stop = 4;
		const int IMG_Stop2 = 5;
		const int IMG_StepInto = 6;
		const int IMG_StepInto2 = 7;
		const int IMG_StepOver = 8;
		const int IMG_StepOver2 = 9;
		const int IMG_StepOut = 10;
		const int IMG_StepOut2 = 11;
		//
		private XmlDocument _docPrj;
		private string _projectFile;
		private LimnorProject _prj;
		private bool _stopping;
		private int _threadId;
		//
		private fnOnInteger miSetTabIndex;
		private fnVoid miSetButtonImages;
		private fnOnDebug miAddNewTab;
		private fnVoid miClose;
		//
		public FormDebugger(string projectFile)
			: base()
		{
			InitializeComponent();
			//
			miSetTabIndex = new fnOnInteger(setTabIndex);
			miSetButtonImages = new fnVoid(setButtonImages0);
			miAddNewTab = new fnOnDebug(addNewTabPage);
			miClose = new fnVoid(exitDebug);
			//
			_projectFile = projectFile;
			_docPrj = new XmlDocument();
			_docPrj.Load(_projectFile);
			_prj = new LimnorProject(_projectFile);
			_prj.Debugging = true;
		}
		#endregion
		#region private methods
		//
		private void exitDebug()
		{
			DebugCommandStatus = EnumRunStatus.Stop;
			btStop.Enabled = false;
			_stopping = true;
			setButtonImages();
			for (int i = 0; i < tabControl1.TabPages.Count; i++)
			{
				for (int j = 0; j < tabControl1.TabPages[i].Controls.Count; j++)
				{
					UserControlDebugger c = tabControl1.TabPages[i].Controls[j] as UserControlDebugger;
					if (c != null)
					{
						c.Stop();
					}
				}
			}
			if (_prj.ProjectType == EnumProjectType.Kiosk)
			{
				LKiosk.Exitkiosk();
			}
			tmStop.Enabled = true;
		}
		private void addNewTabPage(LimnorDebugger debugger)
		{
			UserControlDebugger c = new UserControlDebugger(debugger);
			TabPage p = new TabPage();
			p.Text = debugger.ComponentName;
			p.Name = debugger.Key;
			c.Dock = DockStyle.Fill;
			p.Controls.Add(c);
			tabControl1.TabPages.Add(p);
		}
		private void setTabIndex(int i)
		{
			tabControl1.SelectedIndex = i;
		}
		private void disableAll()
		{
			btRun.Image = imageList1.Images[IMG_Run2];
			btRun.Enabled = false;
			btPause.Image = imageList1.Images[IMG_Pause2];
			btPause.Enabled = false;
			btStepInto.Image = imageList1.Images[IMG_StepInto2];
			btStepInto.Enabled = false;
			btStepOver.Image = imageList1.Images[IMG_StepOver2];
			btStepOver.Enabled = false;
			btStepOut.Image = imageList1.Images[IMG_StepOut2];
			btStepOut.Enabled = false;
			btStop.Image = imageList1.Images[IMG_Stop2];
			btStop.Enabled = false;
		}
		private void setButtonImages0()
		{
			if (_stopping)
			{
				disableAll();
			}
			else
			{
				bool atBreak;
				int level;
				EnumRunStatus status;
				UserControlDebugger c = CurrentComponentUI;
				if (c != null)
				{
					if (c.CurrentViewerFinished)
					{
						status = EnumRunStatus.Finished;
						atBreak = false;
						level = 0;
					}
					else
					{
						atBreak = c.GetAtBreak(c.CurrentThreadId);
						level = c.GetCallStackLevel(c.CurrentThreadId);
						status = c.GetRunStatus(c.CurrentThreadId);
					}
				}
				else
				{
					atBreak = false;
					level = 0;
					status = EnumRunStatus.Run;
				}
				if (status == EnumRunStatus.Finished)
				{
					disableAll();
				}
				else
				{
					if (atBreak)
					{
						btRun.Image = imageList1.Images[IMG_Run];
						btRun.Enabled = true;
						btPause.Image = imageList1.Images[IMG_Pause2];
						btPause.Enabled = false;
						btStepInto.Image = imageList1.Images[IMG_StepInto];
						btStepInto.Enabled = true;
						btStepOver.Image = imageList1.Images[IMG_StepOver];
						btStepOver.Enabled = true;
						if (level > 0)
						{
							btStepOut.Image = imageList1.Images[IMG_StepOut];
							btStepOut.Enabled = true;
						}
						else
						{
							btStepOut.Image = imageList1.Images[IMG_StepOut2];
							btStepOut.Enabled = false;
						}
					}
					else
					{
						btRun.Image = imageList1.Images[IMG_Run2];
						btRun.Enabled = false;
						btPause.Image = imageList1.Images[IMG_Pause];
						btPause.Enabled = true;
						btStepInto.Image = imageList1.Images[IMG_StepInto2];
						btStepInto.Enabled = false;
						btStepOver.Image = imageList1.Images[IMG_StepOver2];
						btStepOver.Enabled = false;
						btStepOut.Image = imageList1.Images[IMG_StepOut2];
						btStepOut.Enabled = false;
					}
				}
				btStop.Image = imageList1.Images[IMG_Stop];
				btStop.Enabled = true;
			}
		}
		#endregion
		#region properties
		public int MainThreadId
		{
			get
			{
				return _threadId;
			}
			set
			{
				_threadId = value;
			}
		}
		public bool FormReady { get; set; }
		/// <summary>
		/// the last button used
		/// </summary>
		public EnumRunStatus DebugCommandStatus { get; set; }

		public UserControlDebugger CurrentComponentUI
		{
			get
			{
				if (tabControl1.TabPages.Count > 0 && tabControl1.SelectedIndex >= 0)
				{
					for (int i = 0; i < tabControl1.TabPages[tabControl1.SelectedIndex].Controls.Count; i++)
					{
						UserControlDebugger c = tabControl1.TabPages[tabControl1.SelectedIndex].Controls[i] as UserControlDebugger;
						if (c != null)
						{
							return c;
						}
					}
				}
				return null;
			}
		}
		public EnumRunStatus RunStatus
		{
			get
			{
				UserControlDebugger c = CurrentComponentUI;
				if (c != null)
				{
					return c.GetRunStatus(c.CurrentThreadId);
				}
				return EnumRunStatus.Run;
			}
		}
		public LimnorProject Project
		{
			get
			{
				return _prj;
			}
		}
		public XmlDocument ProjectDoc
		{
			get
			{
				return _docPrj;
			}
		}
		public string ProjectFile
		{
			get
			{
				return _projectFile;
			}
		}
		#endregion
		#region public methods
		public void ExitDebug()
		{
			this.Invoke(miClose);
		}
		public void SetMainThraedId(int id)
		{
			//tabControl1.TabPages[0].Controls 
		}
		public void setButtonImages()
		{
			this.Invoke(miSetButtonImages);
		}
		public void ShowBreakPoint(UserControlDebugger uc)
		{
			for (int i = 0; i < tabControl1.TabPages.Count; i++)
			{
				for (int j = 0; j < tabControl1.TabPages[i].Controls.Count; j++)
				{
					UserControlDebugger c = tabControl1.TabPages[i].Controls[j] as UserControlDebugger;
					if (c != null)
					{
						if (c.ComponentFile == uc.ComponentFile)
						{
							this.Invoke(miSetTabIndex, i);
							return;
						}
					}
				}
			}
		}
		private UserControlDebugger GetComponentDebugger(LimnorDebugger debugger)
		{
			for (int i = 0; i < tabControl1.TabPages.Count; i++)
			{
				for (int j = 0; j < tabControl1.TabPages[i].Controls.Count; j++)
				{
					UserControlDebugger c = tabControl1.TabPages[i].Controls[j] as UserControlDebugger;
					if (c != null)
					{
						if (c.ComponentFile == debugger.ComponentFile)
						{
							return c;
						}
					}
				}
			}
			return null;
		}
		public UserControlDebugger AddTab(LimnorDebugger debugger)
		{
			UserControlDebugger c = GetComponentDebugger(debugger);
			if (c == null)
			{
				this.Invoke(miAddNewTab, debugger);
				TabPage p = tabControl1.TabPages[debugger.Key];
				c = p.Controls[0] as UserControlDebugger;
			}
			return c;
		}
		public void RefreshUI()
		{
			setButtonImages();
		}
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			setButtonImages();
			FormReady = true;
		}
		#endregion
		#region Event handlers including buttons
		private void btRun_Click(object sender, EventArgs e)
		{
			DebugCommandStatus = EnumRunStatus.Run;
			UserControlDebugger c = CurrentComponentUI;
			if (c != null)
			{
				c.SetRunStatus(c.CurrentThreadId, EnumRunStatus.Run);
				c.SetAtBreak(c.CurrentThreadId, false);
			}
		}
		/// <summary>
		/// break at the first event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btPause_Click(object sender, EventArgs e)
		{
			DebugCommandStatus = EnumRunStatus.Pause;
			UserControlDebugger c = CurrentComponentUI;
			if (c != null)
			{
				c.SetRunStatus(c.CurrentThreadId, EnumRunStatus.Pause);
			}
		}

		private void btStop_Click(object sender, EventArgs e)
		{
			ExitDebug();

		}

		private void btStepInto_Click(object sender, EventArgs e)
		{
			DebugCommandStatus = EnumRunStatus.StepInto;
			UserControlDebugger c = CurrentComponentUI;
			if (c != null)
			{
				c.SetRunStatus(c.CurrentThreadId, EnumRunStatus.StepInto);
				c.SetAtBreak(c.CurrentThreadId, false);
			}
		}

		private void btStepOver_Click(object sender, EventArgs e)
		{
			DebugCommandStatus = EnumRunStatus.StepOver;
			UserControlDebugger c = CurrentComponentUI;
			if (c != null)
			{
				c.SetRunStatus(c.CurrentThreadId, EnumRunStatus.StepOver);
				c.SetAtBreak(c.CurrentThreadId, false);
			}
		}

		private void btStepOut_Click(object sender, EventArgs e)
		{
			DebugCommandStatus = EnumRunStatus.StepOut;
			UserControlDebugger c = CurrentComponentUI;
			if (c != null)
			{
				c.SetRunStatus(c.CurrentThreadId, EnumRunStatus.StepOut);
				c.SetAtBreak(c.CurrentThreadId, false);
			}
		}
		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			setButtonImages();
		}

		private void tmStop_Tick(object sender, EventArgs e)
		{
			tmStop.Enabled = false;
			bool b = false;
			Application.DoEvents();
			System.Threading.Thread.Sleep(0);
			Application.DoEvents();
			for (int i = 0; i < tabControl1.TabPages.Count; i++)
			{
				for (int j = 0; j < tabControl1.TabPages[i].Controls.Count; j++)
				{
					UserControlDebugger c = tabControl1.TabPages[i].Controls[j] as UserControlDebugger;
					if (c != null)
					{
						if (c.IsWaitingAtBreakPoint)
						{
							b = true;
							break;
						}
					}
				}
				if (b)
				{
					break;
				}
			}
			if (b)
			{
				tmStop.Enabled = true;
			}
			else
			{
				Application.Exit();
			}
		}
		#endregion
	}
}
