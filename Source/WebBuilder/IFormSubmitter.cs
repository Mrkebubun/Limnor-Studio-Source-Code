/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.WebBuilder
{
	public interface IFormSubmitter
	{
		string FormName { get; }
		bool IsSubmissionMethod(string method);
		bool RequireSubmissionMethod(string method);
	}
}
