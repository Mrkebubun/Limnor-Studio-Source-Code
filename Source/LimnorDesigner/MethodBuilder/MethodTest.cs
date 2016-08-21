/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MathExp;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// it is supposed to be created in another domain
	/// </summary>
	public class MethodTest : MarshalByRefObject
	{
		public MethodTest()
		{
		}
		public void Start(MethodTestData data)
		{
			dlgMethodTest dlg = new dlgMethodTest();
			dlg.LoadData(data);
			dlg.TopMost = true;
			dlg.ShowDialog(null);
		}
	}
}
