/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Limnor Studio Project
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace VSPrj
{
	public interface ILimnorBuilder
	{
		void AddService(Type serviceType, object service);
		void SetProject(LimnorProject project, string config);
		bool Execute();
	}
}
