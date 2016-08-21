/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Kiosk Support
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace LimnorKiosk
{
	public partial class FormExitCode : Form
	{
		public string ExitCode = "";
		private Button[] _letters;
		private Button[] _symbols;
		public FormExitCode()
		{
			InitializeComponent();
			_letters = new Button[26];
			_letters[0] = buttonA;
			_letters[1] = buttonB;
			_letters[2] = buttonC;
			_letters[3] = buttonD;
			_letters[4] = buttonE;
			_letters[5] = buttonF;
			_letters[6] = buttonG;
			_letters[7] = buttonH;
			_letters[8] = buttonI;
			_letters[9] = buttonJ;
			_letters[10] = buttonK;
			_letters[11] = buttonL;
			_letters[12] = buttonM;
			_letters[13] = buttonN;
			_letters[14] = buttonO;
			_letters[15] = buttonP;
			_letters[16] = buttonQ;
			_letters[17] = buttonR;
			_letters[18] = buttonS;
			_letters[19] = buttonT;
			_letters[20] = buttonU;
			_letters[21] = buttonV;
			_letters[22] = buttonW;
			_letters[23] = buttonX;
			_letters[24] = buttonY;
			_letters[25] = buttonZ;
			_symbols = new Button[16];
			_symbols[0] = buttonAT;
			_symbols[1] = buttonCl;
			_symbols[2] = buttonDollar;
			_symbols[3] = buttonPer;
			_symbols[4] = buttonSharp;
			_symbols[5] = buttonStar;
			_symbols[6] = button0;
			_symbols[7] = button1;
			_symbols[8] = button2;
			_symbols[9] = button3;
			_symbols[10] = button4;
			_symbols[11] = button5;
			_symbols[12] = button6;
			_symbols[13] = button7;
			_symbols[14] = button8;
			_symbols[15] = button9;
			for (int i = 0; i < _letters.Length; i++)
			{
				_letters[i].Click += codeClick;
			}
			for (int i = 0; i < _symbols.Length; i++)
			{
				_symbols[i].Click += codeClick;
			}
		}
		private void codeClick(object sender, EventArgs e)
		{
			Button bt = sender as Button;
			if (bt != null)
			{
				textBox1.Text = string.Format(CultureInfo.InvariantCulture, "{0}{1}", textBox1.Text, bt.Text);
			}
		}
		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			ExitCode = textBox1.Text;
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)13)
			{
				this.DialogResult = DialogResult.OK;
			}
		}

		private void buttonCap_Click(object sender, EventArgs e)
		{
			bool isCap = (string.CompareOrdinal("A", buttonA.Text)==0);
			for (int i = 0; i < _letters.Length; i++)
			{
				if (isCap)
					_letters[i].Text = _letters[i].Text.ToLowerInvariant();
				else
					_letters[i].Text = _letters[i].Text.ToUpperInvariant();
			}
		}

		private void buttonDel_Click(object sender, EventArgs e)
		{
			if (textBox1.Text.Length > 0)
			{
				textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
			}
		}
	}
}
