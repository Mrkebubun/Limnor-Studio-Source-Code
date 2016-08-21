/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace LimnorDatabase
{
	[ReadOnly(true)]
	public class TextColumnStyle : DataGridTextBoxColumn
	{
		HorizontalAlignment _headerColumnAlignment;
		string headerText = "";
		private bool bAdjustingHeaderSize = false;
		public TextColumnStyle(HorizontalAlignment alig)
			: base()
		{
			_headerColumnAlignment = alig;
			this.WidthChanged += new EventHandler(TextColumnStyle_WidthChanged);
			this.HeaderTextChanged += new EventHandler(TextColumnStyle_HeaderTextChanged);
			this.FontChanged += new EventHandler(TextColumnStyle_FontChanged);
		}


		private void TextColumnStyle_WidthChanged(object sender, EventArgs e)
		{
			FormatHeaderText();
		}

		private void TextColumnStyle_HeaderTextChanged(object sender, EventArgs e)
		{
			if (!bAdjustingHeaderSize)
			{
				headerText = this.HeaderText;
				FormatHeaderText();
			}
		}

		private void TextColumnStyle_FontChanged(object sender, EventArgs e)
		{
			FormatHeaderText();
		}

		public void FormatHeaderText()
		{
			if (bAdjustingHeaderSize)
				return;
			bAdjustingHeaderSize = true;
			if (_headerColumnAlignment == this.Alignment)
			{
				this.HeaderText = headerText;
			}
			else
			{
				System.Drawing.Graphics g = this.TextBox.CreateGraphics();
				SizeF size = g.MeasureString(headerText, this.TextBox.Font);
				SizeF size0 = g.MeasureString(" ", this.TextBox.Font);
				//
				if (_headerColumnAlignment == HorizontalAlignment.Left)
				{
					float f = ((float)this.Width - size.Width);
					if (f <= 0)
					{
						this.HeaderText = headerText;
					}
					else
					{
						int n = (int)(f / size0.Width);
						string s = headerText + new string(' ', n);
						int k = 0;

						SizeF size1 = g.MeasureString(s + ".", this.TextBox.Font);
						while (size1.Width < (float)this.Width)
						{
							s = s + " ";
							size1 = g.MeasureString(s + ".", this.TextBox.Font);
							k++;
						}
						s = headerText + new string(' ', n + k - 1) + ".";
						this.HeaderText = s;
					}
				}
				else if (_headerColumnAlignment == HorizontalAlignment.Center)
				{
					float f = ((float)this.Width - size.Width) / (float)2;
					if (f <= 0)
					{
						this.HeaderText = headerText;
					}
					else
					{
						int n = (int)(f / size0.Width);

						if (this.Alignment == HorizontalAlignment.Left)
						{
							string s = new string(' ', n) + headerText;
							SizeF size1 = g.MeasureString(s, this.TextBox.Font);
							while (size1.Width + f < (float)this.Width)
							{
								s = " " + s;
								n++;
								size1 = g.MeasureString(s, this.TextBox.Font);
							}
							this.HeaderText = new string(' ', n) + headerText;
						}
						else if (this.Alignment == HorizontalAlignment.Right)
						{
							string s = headerText + new string(' ', n);
							SizeF size1 = g.MeasureString(s + ".", this.TextBox.Font);
							while (size1.Width + f < (float)this.Width)
							{
								s = s + " ";
								n++;
								size1 = g.MeasureString(s + ".", this.TextBox.Font);
							}
							s = s + ".";
							this.HeaderText = headerText + new string(' ', n) + ".";
						}
					}
				}
				else if (_headerColumnAlignment == HorizontalAlignment.Right)
				{
					if ((float)this.Width <= size.Width)
					{
						this.HeaderText = headerText;
					}
					else
					{
						int n = (int)(((float)this.Width - size.Width) / size0.Width);
						string s = new string(' ', n) + headerText;
						SizeF size1 = g.MeasureString(s, this.TextBox.Font);
						float f = (float)this.Width - size0.Width;
						while (size1.Width < f)
						{
							s = " " + s;
							n++;
							size1 = g.MeasureString(s, this.TextBox.Font);
						}
						this.HeaderText = new string(' ', n) + headerText;
					}
				}
			}
			bAdjustingHeaderSize = false;
		}
	}

}
