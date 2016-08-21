/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Components for Windows Form
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Specialized;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FormComponents
{
	[ToolboxBitmapAttribute(typeof(PrinterManager), "Resources.printer.bmp")]
	[Description("This object provides printer management.")]
	public class PrinterManager : IComponent
	{
		#region Win API
		class WinAPI
		{
			[DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
			public static extern bool SetDefaultPrinter(string Name);
		}
		#endregion
		#region fields and constructors
		private StringCollection _printers;
		private ISite _site;

		public PrinterManager()
		{
		}
		public PrinterManager(IContainer container)
		{
			container.Add(this);
		}
		#endregion
		#region Properties
		[Description("Gets a StringCollection containing all printer names")]
		public StringCollection Printers
		{
			get
			{
				if (_printers == null)
				{
					_printers = new StringCollection();
					for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
					{
						_printers.Add(PrinterSettings.InstalledPrinters[i]);
					}
				}
				return _printers;
			}
		}
		#endregion
		#region Methods
		[Description("Set the default printer to the one specified by printerName. To get all printer names, use property Printers")]
		public void SetDefaultPrinter(string printerName)
		{
			WinAPI.SetDefaultPrinter(printerName);
		}
		[Description("Display a dialogue box showing all printers. Select a printer from the list as the default printer")]
		public void SetDefaultPrinterByUI(Form caller)
		{
			DlgSetDefaultPrinter dlg = new DlgSetDefaultPrinter();
			if (dlg.ShowDialog(caller) == DialogResult.OK)
			{
				SetDefaultPrinter(dlg.SelectedPrinter);
			}
		}
		#endregion

		#region IComponent Members
		[Description("Occurs when this object is disposed")]
		public event EventHandler Disposed;

		[Browsable(false)]
		[ReadOnly(true)]
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
		[Browsable(false)]
		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
