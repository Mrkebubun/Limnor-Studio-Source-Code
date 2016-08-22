/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Rich Text Editor
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace LimnorForms
{
	public partial class RichTextEditor : UserControl
	{
		private string _file;
		private bool _modified;
		private bool _cancel = true;
		public RichTextEditor()
		{
			InitializeComponent();
			rtb.KeyUp += rtb_KeyUp;
		}

		void rtb_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyValue >= 32 && e.KeyValue < 128)
			{
				_modified = true;
			}
		}

		public bool Modified
		{
			get
			{
				return _modified;
			}
		}
		public bool CancelEdit
		{
			get
			{
				return _cancel;
			}
		}
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (e.KeyValue >= 32 && e.KeyValue < 128)
			{
				_modified = true;
			}
		}

		#region Methods
		public void ResetModified()
		{
			_modified = false;
		}
		public void LoadFile(string file)
		{
			try
			{
				rtb.LoadFile(file);
				_file = file;
				_modified = false;
			}
			catch (Exception e)
			{
				MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Error loading file [{0}]. {1}", file, e.Message), "Load Document", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		public void SetSaveFile(string file)
		{
			_file = file;
		}
		public void SaveFile(string file)
		{
			try
			{
				if (string.IsNullOrEmpty(file))
					file = _file;
				rtb.SaveFile(file);
				_file = file;
				_modified = false;
			}
			catch (Exception e)
			{
				MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Error saving file [{0}]. {1}", file, e.Message), "Save Document", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region Toolbox
		private void richTextBox1_SelectionChanged(object sender, EventArgs e)
		{
			BoldToolStripButton.Checked = rtb.SelectionFont.Bold;
			UnderlineToolStripButton.Checked = rtb.SelectionFont.Underline;
			LeftToolStripButton.Checked = (rtb.SelectionAlignment == System.Windows.Forms.HorizontalAlignment.Left);
			CenterToolStripButton.Checked = (rtb.SelectionAlignment == System.Windows.Forms.HorizontalAlignment.Center);
			RightToolStripButton.Checked = (rtb.SelectionAlignment == System.Windows.Forms.HorizontalAlignment.Right);
			BulletsToolStripButton.Checked = rtb.SelectionBullet;
			PasteToolStripButton.Enabled = Clipboard.ContainsText();
			CopyToolStripButton.Enabled = !string.IsNullOrEmpty(rtb.SelectedRtf);
			CutToolStripButton.Enabled = !string.IsNullOrEmpty(rtb.SelectedRtf);
		}

		private void FontToolStripButton_Click(object sender, EventArgs e)
		{
			if (FontDlg.ShowDialog(this) != DialogResult.Cancel)
			{
				rtb.SelectionFont = FontDlg.Font;
				_modified = true;
			}
		}

		private void FontColorToolStripButton_Click(object sender, EventArgs e)
		{
			if (ColorDlg.ShowDialog(this) != DialogResult.Cancel)
			{
				rtb.SelectionColor = ColorDlg.Color;
				_modified = true;
			}
		}

		private void BoldToolStripButton_Click(object sender, EventArgs e)
		{
			Font currentFont = rtb.SelectionFont;
			FontStyle newFontStyle;
			if (rtb.SelectionFont.Bold)
				newFontStyle = currentFont.Style & ~FontStyle.Bold;
			else
				newFontStyle = currentFont.Style | FontStyle.Bold;

			rtb.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);

			BoldToolStripButton.Checked = (rtb.SelectionFont.Bold);
			_modified = true;
		}

		private void UnderlineToolStripButton_Click(object sender, EventArgs e)
		{
			Font currentFont = rtb.SelectionFont;
			FontStyle newFontStyle;
			if (rtb.SelectionFont.Underline)
				newFontStyle = currentFont.Style & ~FontStyle.Underline;
			else
				newFontStyle = currentFont.Style | FontStyle.Underline;

			rtb.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);

			UnderlineToolStripButton.Checked = (rtb.SelectionFont.Underline);
			_modified = true;
		}

		private void LeftToolStripButton_Click(object sender, EventArgs e)
		{
			rtb.SelectionAlignment = HorizontalAlignment.Left;
			_modified = true;
		}

		private void CenterToolStripButton_Click(object sender, EventArgs e)
		{
			rtb.SelectionAlignment = HorizontalAlignment.Center;
			_modified = true;
		}

		private void RightToolStripButton_Click(object sender, EventArgs e)
		{
			rtb.SelectionAlignment = HorizontalAlignment.Right;
			_modified = true;
		}

		private void BulletsToolStripButton_Click(object sender, EventArgs e)
		{
			rtb.SelectionBullet = !rtb.SelectionBullet;
			BulletsToolStripButton.Checked = rtb.SelectionBullet;
			_modified = true;
		}

		private void PasteToolStripButton_Click(object sender, EventArgs e)
		{
			if (Clipboard.ContainsText())
			{
				string txt = Clipboard.GetText();
				try
				{
					rtb.SelectedRtf = txt;
				}
				catch
				{
					try
					{
						rtb.SelectedText = txt;
					}
					catch
					{
					}
				}
			}
		}

		private void CopyToolStripButton_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(rtb.SelectedRtf))
			{
				Clipboard.SetText(rtb.SelectedRtf);
			}
		}

		private void CutToolStripButton_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(rtb.SelectedRtf))
			{
				Clipboard.SetText(rtb.SelectedRtf);
				rtb.SelectedRtf = string.Empty;
			}
		}

		private void toolStripCancel_Click(object sender, EventArgs e)
		{
			_cancel = true;
			Form f = this.FindForm();
			if (f != null)
			{
				f.Close();
			}
		}

		private void toolStripOK_Click(object sender, EventArgs e)
		{
			_cancel = false;
			Form f = this.FindForm();
			if (f != null)
			{
				f.Close();
			}
		}
	}
		#endregion
}
