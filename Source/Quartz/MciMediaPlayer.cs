/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Play audio and video using Media Control Interface
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.IO;

namespace Limnor.Quartz
{
	/// <summary>
	/// Summary description for MediaPlayer.
	/// </summary>
	[ToolboxBitmapAttribute(typeof(MciMediaPlayer), "Resources.mci.bmp")]
	[Description("Simple video and audio player using Media Control Interface. It allows you to play video on the surface of any controls. It does not need DirectX or third party shoftware libraries.")]
	public class MciMediaPlayer : IComponent
	{
		#region fields and constructors
		private QuartzTypeLib.FilgraphManagerClass graphManager = null;
		private QuartzTypeLib.IMediaControl mc = null;
		private QuartzTypeLib.IVideoWindow wm = null;
		private QuartzTypeLib.IMediaEvent me = null;
		private QuartzTypeLib.IMediaPosition mp = null;
		private System.Windows.Forms.Control ctrlOwner = null;
		//
		internal bool bIsVideo = false;
		//
		internal bool bLoaded = false;
		internal bool bClosing = false;
		private clsQZRun objRun = null;
		private bool _initialized;
		public MciMediaPlayer()
		{
			initMCI();
		}
		public MciMediaPlayer(IContainer c)
		{
			if (c != null)
			{
				c.Add(this);
			}
			initMCI();
		}
		private void initMCI()
		{
			graphManager =
				new QuartzTypeLib.FilgraphManagerClass();

			VideoPosition = new System.Drawing.Point(0, 0);
			ManualSize = new System.Drawing.Size(100, 100);
			VideoAllign = System.Drawing.ContentAlignment.MiddleCenter;

			// QueryInterface for the IMediaControl interface:
			mc = (QuartzTypeLib.IMediaControl)graphManager;

			wm = (QuartzTypeLib.IVideoWindow)graphManager;

			me = (QuartzTypeLib.IMediaEvent)graphManager;
			mp = (QuartzTypeLib.IMediaPosition)graphManager;
			if (objRun != null)
			{
				objRun.Stop();
				objRun = null;
			}
			bLoaded = false;
		}
		private bool isRuntime
		{
			get
			{
				if (_site == null)
					return true;
				if (!_site.DesignMode)
					return true;
				return false;
			}
		}
		#endregion
		//
		#region Properties
		[Description("Media play finishes")]
		public event System.EventHandler PlayFinished;

		[Description("Media play generates an error")]
		public event System.EventHandler ErrorOccurred;
		//
		[Browsable(false)]
		internal string sErrMsg = "";
		//Properties

		[DefaultValue(false)]
		[Description("If it is set to true, at runtime, when Filename is set to a media file, or Open method is used, media play will start automatically; if it is set to false, Play method must be used to let the media start to play.")]
		public bool AutoPlay { get; set; }

		[DefaultValue(ContentAlignment.MiddleCenter)]
		[Description("Video allignment position")]
		public ContentAlignment VideoAllign { get; set; }

		[Description("Video position")]
		public Point VideoPosition { get; set; }

		[DefaultValue(false)]
		[Description("Set it to true to ignore video allignment and use video position; set it to false to use video allignment position.")]
		public bool UseVideoPosition { get; set; }

		[Description("Manually set the size of the video")]
		public Size ManualSize { get; set; }

		[DefaultValue(false)]
		[Description("Set it to true to use manual video size; set it to false to use original vedio size")]
		public bool UseManualSize { get; set; }

		[DefaultValue(false)]
		[Description("If it is set to true, then once it finishes playing the media file, it will re-play the file from the beginning again and again; if it is set to false, once it finishes playing the media file, a Finish event will be fired and the playing is stopped at the end position.")]
		public bool Loop { get; set; }
		//
		//
		[Browsable(false)]
		public bool Loaded
		{
			get
			{
				return bLoaded;
			}
		}
		[Description("Specify the window to show video")]
		public Control VideoWindow
		{
			get
			{
				return ctrlOwner;
			}
			set
			{
				ctrlOwner = value;
			}
		}
		[Description("The length of the video or audio, in seconds.")]
		public double Duration
		{
			get
			{
				if (bLoaded)
					return graphManager.Duration;
				return 0;
			}
		}
		[Description("Current position of the video or audio play, in seconds.")]
		public double CurrentPosition
		{
			get
			{
				if (bLoaded)
					return graphManager.CurrentPosition;
				return 0;
			}
		}

		private string _filename;
		[Bindable(BindableSupport.Yes)]
		[Editor(typeof(TypeEditorFilename), typeof(UITypeEditor))]
		[Description("Video or audio file name")]
		public string Filename
		{
			get
			{
				return _filename;
			}
			set
			{
				_filename = value;
				if (isRuntime && _initialized && AutoPlay)
				{
					if (!string.IsNullOrEmpty(_filename))
					{
						if (File.Exists(_filename))
						{
							Open(_filename);
						}
					}
				}
			}
		}
		[Description("Gets a value indicating whether the playing has finished")]
		public bool Finished
		{
			get
			{
				if (graphManager != null)
				{
					if (graphManager.CurrentPosition < graphManager.Duration)
					{
						System.Threading.Thread.Sleep(100);
						return false;
					}
				}
				return true;
			}
		}
		private int _zorder;
		[Browsable(false)]
		public int ZOrder
		{
			get
			{
				return _zorder;
			}
			set
			{
				_zorder = value;
				_initialized = true;
				Filename = _filename;
			}
		}
		#endregion

		#region Methods
		[Browsable(false)]
		internal void _Run()
		{
			mc.Run();
		}
		protected void _WaitForComplete(out int nCode)
		{
			me.WaitForCompletion((int)(graphManager.Duration * 1000) + 1000, out nCode);
		}

		[Browsable(false)]
		internal void _OnFinish()
		{
			ToBeginPos();
			if (PlayFinished != null)
				PlayFinished(this, EventArgs.Empty);
		}
		[Browsable(false)]
		internal void _OnError()
		{
			if (ErrorOccurred != null)
				ErrorOccurred(this, EventArgs.Empty);
		}
		internal void BeginInvoke(MethodInvoker mi)
		{
			if (ctrlOwner != null)
			{
				ctrlOwner.BeginInvoke(mi);
			}
			else
			{
				mi();
			}
		}
		protected void SetOwner(System.Windows.Forms.Control c)
		{
			ctrlOwner = c;
		}
		[Description("Set the current playing position")]
		public void SetCurrentPlayPosition(double position)
		{
			if (bLoaded)
				graphManager.CurrentPosition = position;
		}
		/// <summary>
		/// Open a video or audio file.
		/// </summary>
		/// <param name="sFile">The video or audio file name.</param>
		[Description("Open a video or audio file.")]
		public void Open(string sFile)
		{
			if (bLoaded)
				Stop();
			_filename = sFile;
			_play(false);
		}
		private void _play(bool start)
		{
			mc.RenderFile(_filename);
			try
			{
				int n = wm.WindowStyle;
				if (n == 0)
				{
				}
				bIsVideo = true;
			}
			catch
			{
				bIsVideo = false;
			}
			if (bIsVideo)
			{
				if (ctrlOwner != null)
				{
					wm.WindowStyle = 0;
					if (UseVideoPosition)
					{
						wm.Left = VideoPosition.X;
						wm.Top = VideoPosition.Y;
					}
					else
					{
						switch (VideoAllign)
						{
							case System.Drawing.ContentAlignment.MiddleCenter:
								if (ctrlOwner.ClientSize.Width > wm.Width)
									wm.Left = (ctrlOwner.ClientSize.Width - wm.Width) / 2;
								else
									wm.Left = 0;
								if (ctrlOwner.ClientSize.Height > wm.Height)
									wm.Top = (ctrlOwner.ClientSize.Height - wm.Height) / 2;
								else
									wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.BottomCenter:
								if (ctrlOwner.ClientSize.Width > wm.Width)
									wm.Left = (ctrlOwner.ClientSize.Width - wm.Width) / 2;
								else
									wm.Left = 0;
								if (ctrlOwner.ClientSize.Height > wm.Height)
									wm.Top = ctrlOwner.ClientSize.Height - wm.Height;
								else
									wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.BottomLeft:
								wm.Left = 0;
								if (ctrlOwner.ClientSize.Height > wm.Height)
									wm.Top = ctrlOwner.ClientSize.Height - wm.Height;
								else
									wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.BottomRight:
								if (ctrlOwner.ClientSize.Width > wm.Width)
									wm.Left = ctrlOwner.ClientSize.Width - wm.Width;
								else
									wm.Left = 0;
								if (ctrlOwner.ClientSize.Height > wm.Height)
									wm.Top = ctrlOwner.ClientSize.Height - wm.Height;
								else
									wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.MiddleLeft:
								wm.Left = 0;
								if (ctrlOwner.ClientSize.Height > wm.Height)
									wm.Top = (ctrlOwner.ClientSize.Height - wm.Height) / 2;
								else
									wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.MiddleRight:
								if (ctrlOwner.ClientSize.Width > wm.Width)
									wm.Left = ctrlOwner.ClientSize.Width - wm.Width;
								else
									wm.Left = 0;
								if (ctrlOwner.ClientSize.Height > wm.Height)
									wm.Top = (ctrlOwner.ClientSize.Height - wm.Height) / 2;
								else
									wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.TopCenter:
								if (ctrlOwner.ClientSize.Width > wm.Width)
									wm.Left = (ctrlOwner.ClientSize.Width - wm.Width) / 2;
								else
									wm.Left = 0;
								wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.TopLeft:
								wm.Left = 0;
								wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.TopRight:
								if (ctrlOwner.ClientSize.Width > wm.Width)
									wm.Left = ctrlOwner.ClientSize.Width - wm.Width;
								else
									wm.Left = 0;
								wm.Top = 0;
								break;
						}
					}
					wm.Owner = (int)ctrlOwner.Handle;
					wm.MessageDrain = (int)ctrlOwner.Handle;
				}
				else
				{
					if (UseVideoPosition)
					{
						wm.Left = VideoPosition.X;
						wm.Top = VideoPosition.Y;
					}
					else
					{
						System.Windows.Forms.Screen sr = System.Windows.Forms.Screen.PrimaryScreen;
						switch (VideoAllign)
						{
							case System.Drawing.ContentAlignment.MiddleCenter:
								if (sr.Bounds.Width > wm.Width)
									wm.Left = (sr.Bounds.Width - wm.Width) / 2;
								else
									wm.Left = 0;
								if (sr.Bounds.Height > wm.Height)
									wm.Top = (sr.Bounds.Height - wm.Height) / 2;
								else
									wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.BottomCenter:
								if (sr.Bounds.Width > wm.Width)
									wm.Left = (sr.Bounds.Width - wm.Width) / 2;
								else
									wm.Left = 0;
								if (sr.Bounds.Height > wm.Height)
									wm.Top = sr.Bounds.Height - wm.Height;
								else
									wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.BottomLeft:
								wm.Left = 0;
								if (sr.Bounds.Height > wm.Height)
									wm.Top = sr.Bounds.Height - wm.Height;
								else
									wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.BottomRight:
								if (sr.Bounds.Width > wm.Width)
									wm.Left = sr.Bounds.Width - wm.Width;
								else
									wm.Left = 0;
								if (sr.Bounds.Height > wm.Height)
									wm.Top = sr.Bounds.Height - wm.Height;
								else
									wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.MiddleLeft:
								wm.Left = 0;
								if (sr.Bounds.Height > wm.Height)
									wm.Top = (sr.Bounds.Height - wm.Height) / 2;
								else
									wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.MiddleRight:
								if (sr.Bounds.Width > wm.Width)
									wm.Left = sr.Bounds.Width - wm.Width;
								else
									wm.Left = 0;
								if (sr.Bounds.Height > wm.Height)
									wm.Top = (sr.Bounds.Height - wm.Height) / 2;
								else
									wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.TopCenter:
								if (sr.Bounds.Width > wm.Width)
									wm.Left = (sr.Bounds.Width - wm.Width) / 2;
								else
									wm.Left = 0;
								wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.TopLeft:
								wm.Left = 0;
								wm.Top = 0;
								break;
							case System.Drawing.ContentAlignment.TopRight:
								if (sr.Bounds.Width > wm.Width)
									wm.Left = sr.Bounds.Width - wm.Width;
								else
									wm.Left = 0;
								wm.Top = 0;
								break;
						}
					}
					wm.Owner = 0;
				}
				if (UseManualSize)
				{
					wm.Width = ManualSize.Width;
					wm.Height = ManualSize.Height;
				}
			}
			bLoaded = true;
			if (AutoPlay || start)
				Play();
			else
			{
				mc.Run();
				mc.Pause();
				mp.CurrentPosition = 0;
			}
		}
		[Description("Start playing the video or audio file from the current position after a Pause.")]
		public void Play()
		{
			if (!bLoaded)
			{
				_play(true);
			}
			else
			{
				//use clsQZRun is hard to return to this thread
				if (objRun == null)
				{
					objRun = new clsQZRun(this);
				}
				objRun.Play();
			}
		}
		[Description("Stop playing the video or audio file and close the media file.")]
		public void Stop()
		{
			if (bLoaded)
			{
				if (objRun != null)
				{
					objRun.Stop();
					objRun = null;
				}
				if (mc != null)
				{
					mc.Stop();
				}
				if (this.bIsVideo)
				{
					if (wm != null)
					{
						wm.Visible = 0;
					}
				}
				if (!bClosing)
				{
					initMCI();
				}
			}
		}

		public void Close()
		{
			if (objRun != null)
			{
				objRun.Cancel();
			}
			if (mc != null)
			{
				mc.Stop();
				mc = null;
			}

			if (graphManager != null)
			{
				graphManager.Stop();
				graphManager = null;
			}
			wm = null;
			me = null;
			mp = null;
			if (objRun != null)
			{
				objRun.Cancel();
				objRun.Stop();
				objRun = null;
			}
			ctrlOwner = null;
		}
		[Description("Pause the playing of the media file.")]
		public void Pause()
		{
			if (bLoaded)
			{
				mc.Pause();
			}
		}
		[Description("Move to the beginning position of the media file.")]
		public void ToBeginPos()
		{
			if (bLoaded)
				mp.CurrentPosition = 0;
		}
		[Description("Move to the end position of the media file.")]
		public void ToEndPos()
		{
			if (bLoaded)
				mp.CurrentPosition = mp.Duration;
		}
		[Description("Move to the next position of the media file.")]
		public void Next()
		{
			if (bLoaded)
			{
				//
				double cur = mp.CurrentPosition;
				double r = graphManager.AvgTimePerFrame;
				cur += r;
				if (cur <= mp.Duration)
					mp.CurrentPosition = cur;
			}
		}
		[Description("Move to the previous position of the media file.")]
		public void Back()
		{
			if (bLoaded)
			{
				//
				double cur = mp.CurrentPosition;
				double r = graphManager.AvgTimePerFrame;
				cur -= r;
				if (cur >= 0)
					mp.CurrentPosition = cur;
			}
		}
		#endregion

		#region IComponent Members

		public event EventHandler Disposed;
		private ISite _site;
		[ReadOnly(true)]
		[Browsable(false)]
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			bClosing = true;
			this.Stop();

			this.Close();
			bLoaded = false;
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
