/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace VPL
{
	public class LogonUser
	{
		private Guid _gid;
		private System.Threading.Timer _timer;
		private int _count = 0;
		private int _interval = 5;
		private int _maxCount;
		public LogonUser()
		{
		}
		public LogonUser(string alias, int uid, int level, int minutes)
		{
			this.UserAlias = alias;
			this.UserID = uid;
			this.UserLevel = level;
			this.InactivityMinutes = minutes;
		}
		private void ontimer(object state)
		{
			_count++;
			if (_count >= _maxCount)
			{
				_timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
				_timer = null;
				RemoveLogon(_gid);
			}
		}
		public string UserAlias { get; set; }
		public int UserID { get; set; }
		public int UserLevel { get; set; }
		public int InactivityMinutes { get; set; }
		public void ResetCount()
		{
			_count = 0;
		}
		public void SetID(Guid id)
		{
			_gid = id;
			if (InactivityMinutes > 0)
			{
				_maxCount = (int)((double)InactivityMinutes * 60 / (double)(_interval));
				_timer = new System.Threading.Timer(ontimer, null, 1000, _interval * 1000); //5 seconds
			}
		}
		#region login manager
		private static Dictionary<Guid, LogonUser> _loggedOnUsers;
		private static Dictionary<Guid, List<Form>> _openedForms;
		public static void SetLogon(Guid g, LogonUser user)
		{
			if (_loggedOnUsers == null)
			{
				_loggedOnUsers = new Dictionary<Guid, LogonUser>();
			}
			if (_loggedOnUsers.ContainsKey(g))
			{
				_loggedOnUsers[g] = user;
			}
			else
			{
				_loggedOnUsers.Add(g, user);
			}
			user.SetID(g);
		}
		public static void RemoveLogon(Guid g)
		{
			if (_loggedOnUsers != null)
			{
				if (_loggedOnUsers.ContainsKey(g))
				{
					_loggedOnUsers.Remove(g);
				}
			}
			if (_openedForms != null)
			{
				List<Form> fs;
				if (_openedForms.TryGetValue(g, out fs))
				{
					Form[] fsa = new Form[fs.Count];
					fs.CopyTo(fsa);
					for (int i = 0; i < fsa.Length; i++)
					{
						try
						{
							if (fsa[i] != null && !fsa[i].IsDisposed && !fsa[i].Disposing)
							{
								fsa[i].Close();
							}
						}
						catch
						{
						}
					}
					_openedForms.Remove(g);
				}
			}
		}
		public static void RemoveForm(Guid id, Form f)
		{
			if (_openedForms != null)
			{
				List<Form> fs;
				if (_openedForms.TryGetValue(id, out fs))
				{
					for (int i = 0; i < fs.Count; i++)
					{
						if (f == fs[i])
						{
							fs.RemoveAt(i);
							break;
						}
					}
				}
			}
		}
		public static void AddOpenedForm(Guid id, Form f)
		{
			if (_openedForms == null)
			{
				_openedForms = new Dictionary<Guid, List<Form>>();
			}
			List<Form> fs;
			if (!_openedForms.TryGetValue(id, out fs))
			{
				fs = new List<Form>();
				fs.Add(f);
				_openedForms.Add(id, fs);
			}
			else
			{
				foreach (Form f0 in fs)
				{
					if (f0 == f)
					{
						return;
					}
				}
				fs.Add(f);
			}
		}
		public static LogonUser GetLogonUser(Guid g)
		{
			if (_loggedOnUsers != null)
			{
				if (_loggedOnUsers.ContainsKey(g))
				{
					return _loggedOnUsers[g];
				}
			}
			return null;
		}
		public static void ResetCount(Guid g)
		{
			if (_loggedOnUsers != null)
			{
				LogonUser u;
				if (_loggedOnUsers.TryGetValue(g, out u))
				{
					u.ResetCount();
				}
			}
		}
		#endregion
	}
}
