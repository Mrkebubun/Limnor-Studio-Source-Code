/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VPL
{
	public partial class DlgSelectPen : Form
	{
		public PenWrapper _pen = new PenWrapper();
		public DlgSelectPen()
		{
			InitializeComponent();
			comboBox1.Items.Add("");
			comboBox1.Items.Add(new PenWrapper(Pens.AliceBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.AntiqueWhite));
			comboBox1.Items.Add(new PenWrapper(Pens.Aqua));
			comboBox1.Items.Add(new PenWrapper(Pens.Aquamarine));
			comboBox1.Items.Add(new PenWrapper(Pens.Azure));
			comboBox1.Items.Add(new PenWrapper(Pens.Beige));
			comboBox1.Items.Add(new PenWrapper(Pens.Bisque));
			comboBox1.Items.Add(new PenWrapper(Pens.Black));
			comboBox1.Items.Add(new PenWrapper(Pens.BlanchedAlmond));
			comboBox1.Items.Add(new PenWrapper(Pens.Blue));
			comboBox1.Items.Add(new PenWrapper(Pens.BlueViolet));
			comboBox1.Items.Add(new PenWrapper(Pens.Brown));
			comboBox1.Items.Add(new PenWrapper(Pens.BurlyWood));
			comboBox1.Items.Add(new PenWrapper(Pens.CadetBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.Chartreuse));
			comboBox1.Items.Add(new PenWrapper(Pens.Chocolate));
			comboBox1.Items.Add(new PenWrapper(Pens.Coral));
			comboBox1.Items.Add(new PenWrapper(Pens.CornflowerBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.Cornsilk));
			comboBox1.Items.Add(new PenWrapper(Pens.Crimson));
			comboBox1.Items.Add(new PenWrapper(Pens.Cyan));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkCyan));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkGoldenrod));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkGray));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkGreen));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkKhaki));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkMagenta));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkOliveGreen));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkOrange));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkOrchid));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkRed));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkSalmon));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkSeaGreen));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkSlateBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkSlateGray));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkTurquoise));
			comboBox1.Items.Add(new PenWrapper(Pens.DarkViolet));
			comboBox1.Items.Add(new PenWrapper(Pens.DeepPink));
			comboBox1.Items.Add(new PenWrapper(Pens.DeepSkyBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.DimGray));
			comboBox1.Items.Add(new PenWrapper(Pens.DodgerBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.Firebrick));
			comboBox1.Items.Add(new PenWrapper(Pens.FloralWhite));
			comboBox1.Items.Add(new PenWrapper(Pens.ForestGreen));
			comboBox1.Items.Add(new PenWrapper(Pens.Fuchsia));
			comboBox1.Items.Add(new PenWrapper(Pens.Gainsboro));
			comboBox1.Items.Add(new PenWrapper(Pens.GhostWhite));
			comboBox1.Items.Add(new PenWrapper(Pens.Gold));
			comboBox1.Items.Add(new PenWrapper(Pens.Goldenrod));
			comboBox1.Items.Add(new PenWrapper(Pens.Gray));
			comboBox1.Items.Add(new PenWrapper(Pens.Green));
			comboBox1.Items.Add(new PenWrapper(Pens.GreenYellow));
			comboBox1.Items.Add(new PenWrapper(Pens.Honeydew));
			comboBox1.Items.Add(new PenWrapper(Pens.HotPink));
			comboBox1.Items.Add(new PenWrapper(Pens.IndianRed));
			comboBox1.Items.Add(new PenWrapper(Pens.Indigo));
			comboBox1.Items.Add(new PenWrapper(Pens.Ivory));
			comboBox1.Items.Add(new PenWrapper(Pens.Khaki));
			comboBox1.Items.Add(new PenWrapper(Pens.Lavender));
			comboBox1.Items.Add(new PenWrapper(Pens.LavenderBlush));
			comboBox1.Items.Add(new PenWrapper(Pens.LawnGreen));
			comboBox1.Items.Add(new PenWrapper(Pens.LemonChiffon));
			comboBox1.Items.Add(new PenWrapper(Pens.LightBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.LightCoral));
			comboBox1.Items.Add(new PenWrapper(Pens.LightCyan));
			comboBox1.Items.Add(new PenWrapper(Pens.LightGoldenrodYellow));
			comboBox1.Items.Add(new PenWrapper(Pens.LightGray));
			comboBox1.Items.Add(new PenWrapper(Pens.LightGreen));
			comboBox1.Items.Add(new PenWrapper(Pens.LightPink));
			comboBox1.Items.Add(new PenWrapper(Pens.LightSalmon));
			comboBox1.Items.Add(new PenWrapper(Pens.LightSeaGreen));
			comboBox1.Items.Add(new PenWrapper(Pens.LightSkyBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.LightSlateGray));
			comboBox1.Items.Add(new PenWrapper(Pens.LightSteelBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.LightYellow));
			comboBox1.Items.Add(new PenWrapper(Pens.Lime));
			comboBox1.Items.Add(new PenWrapper(Pens.LimeGreen));
			comboBox1.Items.Add(new PenWrapper(Pens.Linen));
			comboBox1.Items.Add(new PenWrapper(Pens.Magenta));
			comboBox1.Items.Add(new PenWrapper(Pens.Maroon));
			comboBox1.Items.Add(new PenWrapper(Pens.MediumAquamarine));
			comboBox1.Items.Add(new PenWrapper(Pens.MediumBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.MediumOrchid));
			comboBox1.Items.Add(new PenWrapper(Pens.MediumPurple));
			comboBox1.Items.Add(new PenWrapper(Pens.MediumSeaGreen));
			comboBox1.Items.Add(new PenWrapper(Pens.MediumSlateBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.MediumSpringGreen));
			comboBox1.Items.Add(new PenWrapper(Pens.MediumTurquoise));
			comboBox1.Items.Add(new PenWrapper(Pens.MediumVioletRed));
			comboBox1.Items.Add(new PenWrapper(Pens.MidnightBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.MintCream));
			comboBox1.Items.Add(new PenWrapper(Pens.MistyRose));
			comboBox1.Items.Add(new PenWrapper(Pens.Moccasin));
			comboBox1.Items.Add(new PenWrapper(Pens.NavajoWhite));
			comboBox1.Items.Add(new PenWrapper(Pens.Navy));
			comboBox1.Items.Add(new PenWrapper(Pens.OldLace));
			comboBox1.Items.Add(new PenWrapper(Pens.Olive));
			comboBox1.Items.Add(new PenWrapper(Pens.OliveDrab));
			comboBox1.Items.Add(new PenWrapper(Pens.Orange));
			comboBox1.Items.Add(new PenWrapper(Pens.OrangeRed));
			comboBox1.Items.Add(new PenWrapper(Pens.Orchid));
			comboBox1.Items.Add(new PenWrapper(Pens.PaleGoldenrod));
			comboBox1.Items.Add(new PenWrapper(Pens.PaleGreen));
			comboBox1.Items.Add(new PenWrapper(Pens.PaleTurquoise));
			comboBox1.Items.Add(new PenWrapper(Pens.PaleVioletRed));
			comboBox1.Items.Add(new PenWrapper(Pens.PapayaWhip));
			comboBox1.Items.Add(new PenWrapper(Pens.PeachPuff));
			comboBox1.Items.Add(new PenWrapper(Pens.Peru));
			comboBox1.Items.Add(new PenWrapper(Pens.Pink));
			comboBox1.Items.Add(new PenWrapper(Pens.Plum));
			comboBox1.Items.Add(new PenWrapper(Pens.PowderBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.Purple));
			comboBox1.Items.Add(new PenWrapper(Pens.Red));
			comboBox1.Items.Add(new PenWrapper(Pens.RosyBrown));
			comboBox1.Items.Add(new PenWrapper(Pens.RoyalBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.SaddleBrown));
			comboBox1.Items.Add(new PenWrapper(Pens.Salmon));
			comboBox1.Items.Add(new PenWrapper(Pens.SandyBrown));
			comboBox1.Items.Add(new PenWrapper(Pens.SeaGreen));
			comboBox1.Items.Add(new PenWrapper(Pens.SeaShell));
			comboBox1.Items.Add(new PenWrapper(Pens.Sienna));
			comboBox1.Items.Add(new PenWrapper(Pens.Silver));
			comboBox1.Items.Add(new PenWrapper(Pens.SkyBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.SlateBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.SlateGray));
			comboBox1.Items.Add(new PenWrapper(Pens.Snow));
			comboBox1.Items.Add(new PenWrapper(Pens.SpringGreen));
			comboBox1.Items.Add(new PenWrapper(Pens.SteelBlue));
			comboBox1.Items.Add(new PenWrapper(Pens.Tan));
			comboBox1.Items.Add(new PenWrapper(Pens.Teal));
			comboBox1.Items.Add(new PenWrapper(Pens.Thistle));
			comboBox1.Items.Add(new PenWrapper(Pens.Tomato));
			comboBox1.Items.Add(new PenWrapper(Pens.Transparent));
			comboBox1.Items.Add(new PenWrapper(Pens.Turquoise));
			comboBox1.Items.Add(new PenWrapper(Pens.Violet));
			comboBox1.Items.Add(new PenWrapper(Pens.Wheat));
			comboBox1.Items.Add(new PenWrapper(Pens.White));
			comboBox1.Items.Add(new PenWrapper(Pens.WhiteSmoke));
			comboBox1.Items.Add(new PenWrapper(Pens.Yellow));
			comboBox1.Items.Add(new PenWrapper(Pens.YellowGreen));
		}

		public void SetData(Pen pen)
		{
			_pen = new PenWrapper(pen);
			textBoxWidth.Text = _pen.Width.ToString();
			lblColor.BackColor = _pen.Color;
			updateSample();
		}
		private void updateSample()
		{
			lblSample.Height = (int)_pen.Width;
			lblSample.BackColor = _pen.Color;
		}
		private void textBoxWidth_TextChanged(object sender, EventArgs e)
		{
			float n = _pen.Width;
			try
			{
				n = Convert.ToSingle(textBoxWidth.Text);
				if (n > 0 && n < 30)
				{
					_pen = new PenWrapper(_pen.Color, n);
					updateSample();
				}
			}
			catch
			{
			}

		}

		private void btColor_Click(object sender, EventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			dlg.Color = _pen.Color;
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				_pen = new PenWrapper(dlg.Color, _pen.Width);
				lblColor.BackColor = dlg.Color;
				updateSample();
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			int n = comboBox1.SelectedIndex;
			if (n > 0)
			{
				_pen = comboBox1.Items[n] as PenWrapper;
				textBoxWidth.Text = "1";
				lblColor.BackColor = _pen.Color;
				updateSample();
			}
		}

		private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index > 0)
			{
				PenWrapper pw = comboBox1.Items[e.Index] as PenWrapper;
				bool selected = ((e.State & DrawItemState.Selected) != DrawItemState.None);
				if (selected)
				{
					e.Graphics.FillRectangle(Brushes.Blue, e.Bounds);
				}
				else
				{
					e.Graphics.FillRectangle(Brushes.White, e.Bounds);
				}
				e.Graphics.FillRectangle(new SolidBrush(pw.Pen.Color), e.Bounds.Left + 2, e.Bounds.Top + 2, 16, 16);
				if (selected)
				{
					e.Graphics.DrawString(pw.ToString(), this.Font, Brushes.White, e.Bounds.Left + (float)20, e.Bounds.Top + (float)2);
				}
				else
				{
					e.Graphics.DrawString(pw.ToString(), this.Font, Brushes.Black, e.Bounds.Left + (float)20, e.Bounds.Top + (float)2);
				}
			}
		}
	}
}
