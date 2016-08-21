/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for WMPasser.
	/// </summary>
	public class WMPasser
	{
		protected Hashtable register = null;
		public WMPasser()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public void RegisterMsg(int msg,IWMReceiver rcvr)
		{
			if( register == null )
			{
				register = new Hashtable();
			}
			ArrayList vs = register[msg] as ArrayList;
			if( vs == null )
			{
				vs = new ArrayList();
				register.Add(msg,vs);
			}
			if( !vs.Contains(rcvr) )
			{
				vs.Add(rcvr);
			}
		}
		public ArrayList GetReceivers(int msg)
		{
			if( register != null )
			{
				return register[msg] as ArrayList;
			}
			return null;
		}
	}
	public interface IWMReceiver
	{
		void Notify(System.Windows.Forms.Message m);
	}
}
