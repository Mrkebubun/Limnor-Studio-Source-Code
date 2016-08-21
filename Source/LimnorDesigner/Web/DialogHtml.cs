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

namespace LimnorDesigner.Web
{
	public partial class DialogHtml : Form
	{
		public DialogHtml()
		{
			InitializeComponent();
		}
		public void LoadWebPage(string page)
		{
			webBrowser1.Url = new Uri("http://localhost/WebApplication14/WebPage1_design.html");
		}
	}
}
