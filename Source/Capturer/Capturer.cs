/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Video/Audio Capture component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DirectX.Capture;
using System.Drawing;
using DirectShowLib;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Drawing.Design;

namespace Limnor.DirectXCapturer
{
	[ToolboxBitmapAttribute(typeof(Capturer), "Resources.webCam16.bmp")]
	[Description("This component can be used to capture video and autio from sources such as webcameras, TV, etc.")]
	public class Capturer : IComponent, ICloneable, ICustomTypeDescriptor
	{
		#region fields and constructors
		private Capture _capture;
		private Filters _filters;
		private FormErrors _errorDisplay;
		//
		private string _videoDeviceName;
		private string _audioDeviceName;
		private string _videoCompressor;
		private string _audioCompressor;
		private string _videoSourceName;
		private string _audioSourceName;
		//
		private int _frameRate = 24000;
		private Size _frameSize;
		private EnumAudioChannel _audioChannel = EnumAudioChannel.Mono;
		private int _audioSamplerate = 22050;
		private short _audioSampleSize = 16;
		private int _tvChannel = 1;
		private TunerInputType _tunerInputType = DirectShowLib.TunerInputType.Cable;
		//
		private string _filename;
		private Control _previewWindow;
		//
		private bool _noPreviewSelection;
		public Capturer()
		{
			initOnConstruct();
		}
		public Capturer(IContainer components)
		{
			initOnConstruct();
			if (components != null)
			{
				components.Add(this);
			}
		}
		private void initOnConstruct()
		{
			_frameSize = new Size(160, 120);
			_filters = new Filters();
			ShowMessagesOnError = true;
		}
		#endregion

		#region private properties
		private bool isDesignMode
		{
			get
			{
				return (Site != null && Site.DesignMode);
			}
		}
		#endregion

		#region private methods
		private void initCatureObject()
		{
			if (!string.IsNullOrEmpty(_videoDeviceName) || !string.IsNullOrEmpty(_audioDeviceName))
			{
				try
				{
					Filter v = getVideoFilter();
					Filter a = getAudioFilter();
					if (v != null || a != null)
					{
						if (_capture != null)
						{
							_capture.Stop();
							_capture.Dispose();
						}
						_capture = new Capture(v, a);

						setAudioCompressor();
						setVideoCompressor();
						setVideoSource();
						setAudioSource();
						setFrameRate();
						setframeSize();
						setAudioChannel();
						setAudioSampleRate();
						setAudioSampleSize();
						setTvTunerChannel();
						setTunerInputType();
						setPreviewWindow();
						_capture.CaptureComplete += new EventHandler(_capture_CaptureComplete);
					}
					else
					{
						processError("initialize", "No video device and audio device.");
					}
				}
				catch (Exception err)
				{
					processError("initialize", err);
				}
			}
		}

		private void _capture_CaptureComplete(object sender, EventArgs e)
		{
			if (CaptureCompleted != null)
			{
				CaptureCompleted(this, e);
			}
		}
		private Filter getVideoFilter()
		{
			FilterCollection fc = _filters.VideoInputDevices;
			if (fc != null && fc.Count > 0)
			{
				foreach (Filter f in fc)
				{
					if (string.Compare(_videoDeviceName, f.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return f;
					}
				}
			}
			return null;
		}
		private Filter getAudioFilter()
		{
			FilterCollection fc = _filters.AudioInputDevices;
			if (fc != null && fc.Count > 0)
			{
				foreach (Filter f in fc)
				{
					if (string.Compare(_audioDeviceName, f.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return f;
					}
				}
			}
			return null;
		}
		private void setVideoCompressor()
		{
			if (_capture != null)
			{
				FilterCollection fc = _filters.VideoCompressors;
				if (fc != null && fc.Count > 0)
				{
					foreach (Filter f in fc)
					{
						if (string.Compare(_videoCompressor, f.Name, StringComparison.OrdinalIgnoreCase) == 0)
						{
							_capture.VideoCompressor = f;
							break;
						}
					}
				}
			}
		}
		private void setAudioCompressor()
		{
			if (_capture != null)
			{
				FilterCollection fc = _filters.AudioCompressors;
				if (fc != null && fc.Count > 0)
				{
					foreach (Filter f in fc)
					{
						if (string.Compare(_audioCompressor, f.Name, StringComparison.OrdinalIgnoreCase) == 0)
						{
							_capture.AudioCompressor = f;
							break;
						}
					}
				}
			}
		}
		private void setVideoSource()
		{
			if (_capture != null)
			{
				SourceCollection sc = _capture.VideoSources;
				if (sc != null && sc.Count > 0)
				{
					foreach (Source s in sc)
					{
						if (string.Compare(_videoSourceName, s.Name, StringComparison.OrdinalIgnoreCase) == 0)
						{
							_capture.VideoSource = s;
							break;
						}
					}
				}
			}
		}
		private void setAudioSource()
		{
			if (_capture != null)
			{
				SourceCollection sc = _capture.AudioSources;
				if (sc != null && sc.Count > 0)
				{
					foreach (Source s in sc)
					{
						if (string.Compare(_audioSourceName, s.Name, StringComparison.OrdinalIgnoreCase) == 0)
						{
							_capture.AudioSource = s;
							break;
						}
					}
				}
			}
		}
		private void setFrameRate()
		{
			if (_capture != null)
			{
				try
				{
					_capture.FrameRate = _frameRate;
				}
				catch (Exception e)
				{
					processError("Set Frame rate", e);
				}
			}
		}
		private void setframeSize()
		{
			if (_capture != null)
			{
				try
				{
					_capture.FrameSize = _frameSize;
				}
				catch (Exception e)
				{
					processError("Set Frame size", e);
				}
			}
		}
		private void setAudioChannel()
		{
			if (_capture != null)
			{
				try
				{
					_capture.AudioChannels = (short)_audioChannel;
				}
				catch (Exception e)
				{
					processError("Set Frame rate", e);
				}
			}
		}
		private void setAudioSampleRate()
		{
			if (_capture != null)
			{
				try
				{
					_capture.AudioSamplingRate = _audioSamplerate;
				}
				catch (Exception e)
				{
					processError("Set audio sample rate", e);
				}
			}
		}
		private void setAudioSampleSize()
		{
			if (_capture != null)
			{
				try
				{
					_capture.AudioSampleSize = _audioSampleSize;
				}
				catch (Exception e)
				{
					processError("Set audio sample size", e);
				}
			}
		}
		private void setTvTunerChannel()
		{
			if (_capture != null && _capture.Tuner != null)
			{
				try
				{
					_capture.Tuner.Channel = _tvChannel;
				}
				catch (Exception e)
				{
					processError("Set TV Tuner Channel", e);
				}
			}
		}
		private void setTunerInputType()
		{
			if (_capture != null && _capture.Tuner != null)
			{
				try
				{
					_capture.Tuner.InputType = _tunerInputType;
				}
				catch (Exception e)
				{
					processError("Set TV Tuner Input Type", e);
				}
			}
		}
		private void setPreviewWindow()
		{
			if (isDesignMode)
			{
				if (_previewWindow != null)
				{
					Graphics g = _previewWindow.CreateGraphics();
					g.FillRectangle(Brushes.Black, 0, 0, _previewWindow.Width, _previewWindow.Height);
					g.DrawString("Web Camera Window", _previewWindow.Font, Brushes.White, (float)2, (float)2);
				}
			}
			else
			{
				if (_capture != null)
				{
					try
					{
						_capture.PreviewWindow = _previewWindow;
					}
					catch (Exception e)
					{
						processError("Set preview window", e);
					}
				}
			}
		}
		private void copyData(Capturer obj)
		{
			_videoDeviceName = obj._videoDeviceName;
			_audioDeviceName = obj._audioDeviceName;
			_videoCompressor = obj._videoCompressor;
			_audioCompressor = obj._audioCompressor;
			_videoSourceName = obj._videoSourceName;
			_audioSourceName = obj._audioSourceName;
			_frameRate = obj._frameRate;
			_audioChannel = obj._audioChannel;
			_audioSamplerate = obj._audioSamplerate;
			_audioSampleSize = obj._audioSampleSize;
			_tvChannel = obj._tvChannel;
			_tunerInputType = obj._tunerInputType;
			_filename = obj._filename;
			_filters = obj._filters;
			initCatureObject();
		}
		private void processError(string context, Exception error, params object[] values)
		{
			StringBuilder sb = new StringBuilder();
			while (error != null)
			{
				sb.Append(error.Message);
				sb.Append("\r\nSource:");
				if (error.Source != null)
				{
					sb.Append(error.Source);
				}
				sb.Append("\r\nStack trace:");
				if (error.StackTrace != null)
				{
					sb.Append(error.StackTrace);
				}
				error = error.InnerException;
				if (error != null)
				{
					sb.Append("\r\nInner exception:\r\n");
				}
			}
			processError(context, sb.ToString(), values);
		}
		private void processError(string context, string error, params object[] values)
		{
			if (values != null && values.Length > 0)
			{
				CErrorLog.ErrorLog.AddError(string.Format(System.Globalization.CultureInfo.InvariantCulture, context, values), error);
			}
			else
			{
				CErrorLog.ErrorLog.AddError(context, error);
			}
			if (Error != null)
			{
				Error(this, EventArgs.Empty);
				if (ShowMessagesOnError)
				{
					DisplayErrors();
				}
			}
		}
		#endregion

		#region internal methods
		[Browsable(false)]
		internal void PrepareForDialog()
		{
			_noPreviewSelection = true;
		}
		#endregion

		#region Properties
		[DefaultValue(true)]
		[Description("Gets and sets a value indicating whether error messages are displayed when an error occurs. Error messages may also be displayed by executing a DisplayErrors action.")]
		public bool ShowMessagesOnError
		{
			get;
			set;
		}

		[RefreshProperties(RefreshProperties.All)]
		[Editor(typeof(PropEditorDevices), typeof(UITypeEditor))]
		[DefaultValue(null)]
		[Description("Video device name")]
		public string VideoDeviceName
		{
			get
			{
				return _videoDeviceName;
			}
			set
			{
				_videoDeviceName = value;
				initCatureObject();
			}
		}
		[Editor(typeof(PropEditorDevices), typeof(UITypeEditor))]
		[DefaultValue(null)]
		[Description("Audio device name")]
		public string AudioDeviceName
		{
			get
			{
				return _audioDeviceName;
			}
			set
			{
				_audioDeviceName = value;
				initCatureObject();
			}
		}
		[Editor(typeof(PropEditorDevices), typeof(UITypeEditor))]
		[DefaultValue(null)]
		[Description("Video compressor name")]
		public string VideoCompressor
		{
			get
			{
				return _videoCompressor;
			}
			set
			{
				_videoCompressor = value;
				setVideoCompressor();
			}
		}
		[Editor(typeof(PropEditorDevices), typeof(UITypeEditor))]
		[DefaultValue(null)]
		[Description("Audio compressor name")]
		public string AudioCompressor
		{
			get
			{
				return _audioCompressor;
			}
			set
			{
				_audioCompressor = value;
				setAudioCompressor();
			}
		}
		[Editor(typeof(PropEditorDevices), typeof(UITypeEditor))]
		[DefaultValue(null)]
		[Description("Video source name")]
		public string VideoSourceName
		{
			get
			{
				return _videoSourceName;
			}
			set
			{
				_videoSourceName = value;
				setVideoSource();
			}
		}
		[Editor(typeof(PropEditorDevices), typeof(UITypeEditor))]
		[DefaultValue(null)]
		[Description("Audio source name")]
		public string AudioSourceName
		{
			get
			{
				return _audioSourceName;
			}
			set
			{
				_audioSourceName = value;
				setAudioSource();
			}
		}

		[TypeConverter(typeof(TypeConverterFrameRate))]
		[Editor(typeof(PropEditorFrameRate), typeof(UITypeEditor))]
		[DefaultValue(24000)]
		[Description("Gets and sets the frame rate for capturing video")]
		public int FrameRate
		{
			get
			{
				return _frameRate;
			}
			set
			{
				_frameRate = value;
				setFrameRate();
			}
		}

		[TypeConverter(typeof(TypeConverterVideoFrameSize))]
		[Editor(typeof(PropEditorFrameSize), typeof(UITypeEditor))]
		[Description("Gets and sets the frame size for capturing video")]
		public Size FrameSize
		{
			get
			{
				return _frameSize;
			}
			set
			{
				_frameSize = value;
				setframeSize();
			}
		}
		[DefaultValue(EnumAudioChannel.Mono)]
		[Description("Gets and sets the number of channels in the waveform-audio data.")]
		public EnumAudioChannel AudioChannel
		{
			get
			{
				return _audioChannel;
			}
			set
			{
				_audioChannel = value;
				setAudioChannel();
			}
		}

		[TypeConverter(typeof(TypeConverterAudioSampleRate))]
		[Editor(typeof(PropEditorAudioSampleRate), typeof(UITypeEditor))]
		[DefaultValue(22050)]
		[Description("Gets and sets the number of audio samples taken per second.")]
		public int AudioSamplingRate
		{
			get
			{
				return _audioSamplerate;
			}
			set
			{
				_audioSamplerate = value;
				setAudioSampleRate();
			}
		}

		[TypeConverter(typeof(TypeConverterAudioSampleSize))]
		[Editor(typeof(PropEditorAudioSampleSize), typeof(UITypeEditor))]
		[DefaultValue((short)16)]
		[Description("Gets and sets the number of bits recorded per sample. ")]
		public short AudioSampleSize
		{
			get
			{
				return _audioSampleSize;
			}
			set
			{
				_audioSampleSize = value;
				setAudioSampleSize();
			}
		}

		[DefaultValue(1)]
		[Description("Gets and sets the TV Tuner channel.")]
		public int TvTunerChannel
		{
			get
			{
				return _tvChannel;
			}
			set
			{
				_tvChannel = value;
				setTvTunerChannel();
			}
		}

		[DefaultValue(TunerInputType.Cable)]
		[Description("Gets and sets the tuner frequency (cable or antenna).")]
		public TunerInputType TunerInputType
		{
			get
			{
				return _tunerInputType;
			}
			set
			{
				_tunerInputType = value;
				setTunerInputType();
			}
		}
		[DefaultValue(null)]
		[Description("The name of the file for saving the capturing")]
		public string Filename
		{
			get
			{
				return _filename;
			}
			set
			{
				_filename = value;
			}
		}

		[DefaultValue(null)]
		[Description("This is the window showing the video preview")]
		public Control PreviewWindow
		{
			get
			{
				return _previewWindow;
			}
			set
			{
				if (_previewWindow != null)
				{
					_previewWindow.Invalidate();
				}
				_previewWindow = value;
				setPreviewWindow();
			}
		}
		[Description("A list of available video input devices")]
		public IList<string> VideoDeviceList
		{
			get
			{
				List<String> lst = new List<string>();
				FilterCollection fc = _filters.VideoInputDevices;
				if (fc != null && fc.Count > 0)
				{
					foreach (Filter f in fc)
					{
						lst.Add(f.Name);
					}
				}
				return lst;
			}
		}
		[Description("A list of available audio input devices")]
		public IList<string> AudioDeviceList
		{
			get
			{
				List<String> lst = new List<string>();
				FilterCollection fc = _filters.AudioInputDevices;
				if (fc != null && fc.Count > 0)
				{
					foreach (Filter f in fc)
					{
						lst.Add(f.Name);
					}
				}
				return lst;
			}
		}
		[Description("Available video compressors")]
		public IList<Filter> VideoCompressorList
		{
			get
			{
				List<Filter> lst = new List<Filter>();
				FilterCollection fc = _filters.VideoCompressors;
				if (fc != null && fc.Count > 0)
				{
					foreach (Filter f in fc)
					{
						lst.Add(f);
					}
				}
				return lst;
			}
		}
		[Description("Available audio compressors")]
		public IList<Filter> AudioCompressorList
		{
			get
			{
				List<Filter> lst = new List<Filter>();
				FilterCollection fc = _filters.AudioCompressors;
				if (fc != null && fc.Count > 0)
				{
					foreach (Filter f in fc)
					{
						lst.Add(f);
					}
				}
				return lst;
			}
		}
		[Description("Collection of available video sources/physical connectors on the current video device.")]
		public IList<Source> VideoSourceList
		{
			get
			{
				List<Source> lst = new List<Source>();
				if (_capture != null)
				{
					SourceCollection sc = _capture.VideoSources;
					if (sc != null && sc.Count > 0)
					{
						foreach (Source s in sc)
						{
							lst.Add(s);
						}
					}
				}
				return lst;
			}
		}
		[Description("Collection of available audio sources/physical connectors on the current audio device.")]
		public IList<Source> AudioSourceList
		{
			get
			{
				List<Source> lst = new List<Source>();
				if (_capture != null)
				{
					SourceCollection sc = _capture.AudioSources;
					if (sc != null && sc.Count > 0)
					{
						foreach (Source s in sc)
						{
							lst.Add(s);
						}
					}
				}
				return lst;
			}
		}
		[Description("Gets a list of property pages")]
		public IList<string> PropertyPages
		{
			get
			{
				List<string> lst = new List<string>();
				if (_capture != null && _capture.PropertyPages != null && _capture.PropertyPages.Count > 0)
				{
					for (int i = 0; i < _capture.PropertyPages.Count; i++)
					{
						lst.Add(_capture.PropertyPages[i].Name);
					}
				}
				return lst;
			}
		}
		[Description("Indicates whether the capturing is stopped")]
		public bool Stopped
		{
			get
			{
				if (_capture != null)
				{
					return _capture.Stopped;
				}
				return true;
			}
		}
		[Description("Indicates whether the capturing is started")]
		public bool Started
		{
			get
			{
				if (_capture != null)
				{
					return _capture.Capturing;
				}
				return false;
			}
		}
		[Description("Indicates whether it is cued for starting capturing")]
		public bool Cued
		{
			get
			{
				if (_capture != null)
				{
					return _capture.Cued;
				}
				return false;
			}
		}
		#endregion

		#region Methods
		[Description("Prepare for capturing. Use this method when capturing must begin as quickly as possible. ")]
		public void Cue()
		{
			if (_capture != null)
			{
				if (!_capture.Cued)
				{
					_capture.Filename = _filename;
				}
				try
				{
					_capture.Cue();
				}
				catch (Exception err)
				{
					processError("Cue", err);
				}
			}
		}
		[Description("Start capturing")]
		public void StartCapture()
		{
			if (_capture != null)
			{
				try
				{
					if (!_capture.Cued)
					{
						_capture.Filename = _filename;
					}
					_capture.Start();
				}
				catch (Exception err)
				{
					processError("StartCapture", err);
				}
			}
		}
		[Description("Stop capturing")]
		public void StopCapture()
		{
			if (_capture != null)
			{
				try
				{
					_capture.Stop();
				}
				catch (Exception err)
				{
					processError("StopCapture", err);
				}
			}
		}
		[Description("Show all errors occured since the starting of the application or since the last ClearErrorLog was executed")]
		public void DisplayErrors()
		{
			if (_errorDisplay == null || _errorDisplay.IsDisposed || _errorDisplay.Disposing)
			{
				_errorDisplay = new FormErrors();
			}
			_errorDisplay.ShowErrors();
			_errorDisplay.Show();
			_errorDisplay.TopMost = true;
		}
		[Description("Remove all errors logged in memory")]
		public void ClearErrorLog()
		{
			CErrorLog.ErrorLog.ClearErrorLog();
		}
		[Description("Use the video input device and audio input device specified by the parameters")]
		public void SetInputDevices(string videoDevice, string audioDevice)
		{
			if (string.IsNullOrEmpty(videoDevice) && string.IsNullOrEmpty(audioDevice))
			{
				processError("SetInputDevices", "Input devices cannot all be empty");
			}
			else
			{
				_videoDeviceName = videoDevice;
				_audioDeviceName = audioDevice;
				initCatureObject();
			}
		}
		[Description("Use a dialogue box to select video and audio input devices")]
		public bool SelectInputDevices()
		{
			try
			{
				DialogSelectInputDevices dlg = new DialogSelectInputDevices();
				dlg.LoadData(this);
				Form f = null;
				if (_previewWindow != null)
				{
					f = _previewWindow.FindForm();
				}
				DialogResult r = dlg.ShowDialog(f);
				return (r == DialogResult.OK);
			}
			catch (Exception err)
			{
				processError("SelectInputDevices", err);
			}
			return false;
		}
		[Description("Use a dialogue box to select video compressor")]
		public bool SelectVideoCompression()
		{
			try
			{
				DialogMakeSelection dlg = new DialogMakeSelection();
				dlg.LoadFilters(this.VideoCompressorList, _videoCompressor);
				Form f = null;
				if (_previewWindow != null)
				{
					f = _previewWindow.FindForm();
				}
				if (dlg.ShowDialog(f) == DialogResult.OK)
				{
					if (dlg.SelectedFilter == null)
					{
						_videoCompressor = string.Empty;
					}
					else
					{
						_videoCompressor = dlg.SelectedFilter.Name;
					}
					if (_capture != null)
					{
						_capture.VideoCompressor = dlg.SelectedFilter;
					}
					return true;
				}
			}
			catch (Exception err)
			{
				processError("SelectVideoCompression", err);
			}
			return false;
		}
		[Description("Use a dialogue box to select audio compressor")]
		public bool SelectAudioCompression()
		{
			try
			{
				DialogMakeSelection dlg = new DialogMakeSelection();
				dlg.LoadFilters(this.AudioCompressorList, _audioCompressor);
				Form f = null;
				if (_previewWindow != null)
				{
					f = _previewWindow.FindForm();
				}
				if (dlg.ShowDialog(f) == DialogResult.OK)
				{
					if (dlg.SelectedFilter == null)
					{
						_audioCompressor = string.Empty;
					}
					else
					{
						_audioCompressor = dlg.SelectedFilter.Name;
					}
					if (_capture != null)
					{
						_capture.AudioCompressor = dlg.SelectedFilter;
					}
					return true;
				}
			}
			catch (Exception err)
			{
				processError("SelectAudioCompression", err);
			}
			return false;
		}

		[Description("Use a dialogue box to select video source")]
		public bool SelectVideoSource()
		{
			if (_capture != null)
			{
				Form f = null;
				if (_previewWindow != null)
				{
					f = _previewWindow.FindForm();
				}
				try
				{
					DialogMakeSelection dlg = new DialogMakeSelection();
					dlg.LoadSources(_capture.VideoSources, _capture.VideoSource);
					if (dlg.ShowDialog(f) == DialogResult.OK)
					{
						_capture.VideoSource = dlg.SelectedSource;
						return true;
					}
				}
				catch (Exception err)
				{
					processError("SelectVideoSource", err);
				}
			}
			return false;
		}

		[Description("Use a dialogue box to select audio source")]
		public bool SelectAudioSource()
		{
			if (_capture != null)
			{
				Form f = null;
				if (_previewWindow != null)
				{
					f = _previewWindow.FindForm();
				}
				try
				{
					DialogMakeSelection dlg = new DialogMakeSelection();
					dlg.LoadSources(_capture.AudioSources, _capture.AudioSource);
					if (dlg.ShowDialog(f) == DialogResult.OK)
					{
						_capture.AudioSource = dlg.SelectedSource;
						return true;
					}
				}
				catch (Exception err)
				{
					processError("SelectAudioSource", err);
				}
			}
			return false;
		}
		[Description("Show property page identified by index. Some property pages cannot be displayed while previewing and/or capturing. This method will block until the property page is closed by the user. If the input device drivers are 32-bit then your application must be compiled to x86 for this method to work.")]
		public void ShowPropertyPageByIndex(int index)
		{
			if (_capture != null)
			{
				if (index >= 0 && index < _capture.PropertyPages.Count)
				{
					try
					{
						_capture.PropertyPages[index].Show(_previewWindow);
					}
					catch (Exception e)
					{
						processError("ShowPropertyPageByIndex({0})", e, index);
					}
				}
				else
				{
					processError("ShowPropertyPageByIndex({0})", "index is out of range", index);
				}
			}
			else
			{
				processError("ShowPropertyPageByIndex({0})", "capture device not initialized", index);
			}
		}
		[Description("Show property page identified by page name. Some property pages cannot be displayed while previewing and/or capturing. This method will block until the property page is closed by the user. If the input device drivers are 32-bit then your application must be compiled to x86 for this method to work.")]
		public void ShowPropertyPageByName(string pageName)
		{
			if (_capture != null)
			{
				PropertyPage p = null;
				if (_capture.PropertyPages != null && _capture.PropertyPages.Count > 0)
				{
					for (int i = 0; i < _capture.PropertyPages.Count; i++)
					{
						if (string.Compare(pageName, _capture.PropertyPages[i].Name, StringComparison.OrdinalIgnoreCase) == 0)
						{
							p = _capture.PropertyPages[i];
							break;
						}
					}
				}
				if (p != null)
				{
					try
					{
						p.Show(_previewWindow);
					}
					catch (Exception e)
					{
						processError("ShowPropertyPageByName({0})", e, pageName);
					}
				}
				else
				{
					processError("ShowPropertyPageByName({0})", "invalid page name", pageName);
				}
			}
			else
			{
				processError("ShowPropertyPageByName({0})", "capture device not initialized", pageName);
			}
		}
		[Description("Display a dialogue box showing all properties")]
		public bool ShowPropertiesDialogue()
		{
			DialogProperties dlg = new DialogProperties();
			Capturer obj = (Capturer)this.Clone();
			dlg.LoadData(obj);
			Form f = null;
			if (_previewWindow != null)
			{
				f = _previewWindow.FindForm();
			}
			if (dlg.ShowDialog(f) == DialogResult.OK)
			{
				copyData(obj);
				return true;
			}
			return false;
		}
		#endregion

		#region Events
		[Description("It occurs when the capture is completed")]
		public event EventHandler CaptureCompleted;
		[Description("It occurs when an operation failed.")]
		public event EventHandler Error;
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
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
			if (_capture != null)
			{
				_capture.Stop();
				_capture.Dispose();
				_capture = null;
			}
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			Capturer obj = new Capturer();
			obj._videoDeviceName = _videoDeviceName;
			obj._audioDeviceName = _audioDeviceName;
			obj._videoCompressor = _videoCompressor;
			obj._audioCompressor = _audioCompressor;
			obj._videoSourceName = _videoSourceName;
			obj._audioSourceName = _audioSourceName;
			obj._frameRate = _frameRate;
			obj._audioChannel = _audioChannel;
			obj._audioSamplerate = _audioSamplerate;
			obj._audioSampleSize = _audioSampleSize;
			obj._tvChannel = _tvChannel;
			obj._tunerInputType = _tunerInputType;
			obj._filename = _filename;
			obj.initCatureObject();
			return obj;
		}

		#endregion

		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (_noPreviewSelection)
				{
					if (string.CompareOrdinal("PreviewWindow", p.Name) != 0)
					{
						if (string.CompareOrdinal("PropertyPages", p.Name) != 0)
						{
							list.Add(p);
						}
					}
				}
				else
				{
					list.Add(p);
				}
			}
			if (_capture != null && _capture.PropertyPages != null && _capture.PropertyPages.Count > 0)
			{
				for (int i = 0; i < _capture.PropertyPages.Count; i++)
				{
					if (_capture.PropertyPages[i] != null)
					{
						list.Add(new PropertyDescriptorPropertyPage(
								_capture.PropertyPages[i],
								string.Format(System.Globalization.CultureInfo.InvariantCulture,
								"PropertyPage{0}", i + 1), attributes));
					}
				}
			}
			return new PropertyDescriptorCollection(list.ToArray());
		}
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region PropertyDescriptorPropertyPage
		class PropertyDescriptorPropertyPage : PropertyDescriptor
		{
			private PropertyPage _page;
			public PropertyDescriptorPropertyPage(PropertyPage page, string name, Attribute[] attrs)
				: base(name, createAttributes(attrs))
			{
				_page = page;
			}
			private static Attribute[] createAttributes(Attribute[] attrs)
			{
				int nEditor;
				int nDesc;
				Attribute[] ret;
				if (attrs == null)
				{
					ret = new Attribute[2];
					nEditor = 0;
					nDesc = 1;
				}
				else
				{
					nDesc = -1;
					nEditor = attrs.Length;
					for (int i = 0; i < nEditor; i++)
					{
						if (attrs[i] is DescriptionAttribute)
						{
							nDesc = i;
							break;
						}
					}
					if (nDesc < 0)
					{
						ret = new Attribute[attrs.Length + 2];
						nDesc = attrs.Length + 1;
					}
					else
					{
						ret = new Attribute[attrs.Length + 1];
					}
					if (attrs.Length > 0)
					{
						attrs.CopyTo(ret, 0);
					}
				}
				ret[nEditor] = new EditorAttribute(typeof(PropEditorPropertyPage), typeof(UITypeEditor));
				ret[nDesc] = new DescriptionAttribute("Click the edit button (...) to show the property page provided by the device driver. If the device driver is of 32-bit then the application has to be compiled to x86 for it to work.");
				return ret;
			}
			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(Capturer); }
			}

			public override object GetValue(object component)
			{
				return _page.Name;
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get { return typeof(string); }
			}

			public override void ResetValue(object component)
			{

			}

			public override void SetValue(object component, object value)
			{

			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion
	}
}
