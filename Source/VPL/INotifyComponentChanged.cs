/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace VPL
{
	/// <summary>
	/// array item change cannot trig property change notification to the Form designer.
	/// use this interface to do notification manually
	/// </summary>
	public interface INotifyComponentChanged
	{
		/// <summary>
		/// handle property changes
		/// </summary>
		/// <param name="e"></param>
		/// <returns>true: processed</returns>
		bool OnComponentChanged(PropertyValueChangedEventArgs e);
	}
	public interface IVplDesignerService
	{
		void OnRequestService(object component, string serviceRequest);
	}
}
