/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Video/Audio Capture component
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
using DirectX.Capture;

namespace Limnor.DirectXCapturer
{
	public partial class DialogSelectInputDevices : Form
	{
		public string SelectedVideoSource;
		public string SelectedAudioSource;
		private Capturer _capture;
		public DialogSelectInputDevices()
		{
			InitializeComponent();

		}
		public void LoadData(Capturer cap)
		{
			_capture = cap;
			listBoxVideo.Items.Clear();
			listBoxVideo.Items.Add("");
			IList<string> ss = cap.VideoDeviceList;
			foreach (string s in ss)
			{
				int n = listBoxVideo.Items.Add(s);
				if (listBoxVideo.SelectedIndex < 0)
				{
					if (string.Compare(s, cap.VideoDeviceName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						listBoxVideo.SelectedIndex = n;
					}
				}
			}
			if (listBoxVideo.SelectedIndex < 0 && listBoxVideo.Items.Count > 0)
			{
				listBoxVideo.SelectedIndex = 0;
			}
			listBoxAudio.Items.Clear();
			listBoxAudio.Items.Add("");
			ss = cap.AudioDeviceList;
			foreach (string s in ss)
			{
				int n = listBoxAudio.Items.Add(s);
				if (listBoxAudio.SelectedIndex < 0)
				{
					if (string.Compare(s, cap.AudioDeviceName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						listBoxAudio.SelectedIndex = n;
					}
				}
			}
			if (listBoxAudio.SelectedIndex < 0 && listBoxAudio.Items.Count > 0)
			{
				listBoxAudio.SelectedIndex = 0;
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			SelectedVideoSource = "";
			if (listBoxVideo.SelectedIndex >= 0)
			{
				SelectedVideoSource = listBoxVideo.Text;
			}
			SelectedAudioSource = "";
			if (listBoxAudio.SelectedIndex >= 0)
			{
				SelectedAudioSource = listBoxAudio.Text;
			}
			if (!string.IsNullOrEmpty(SelectedAudioSource) || !string.IsNullOrEmpty(SelectedVideoSource))
			{
				_capture.SetInputDevices(SelectedVideoSource, SelectedAudioSource);
				this.DialogResult = DialogResult.OK;
			}
		}
	}
}
