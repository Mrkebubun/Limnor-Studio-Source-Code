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
	public class FormNavigator
	{
		private static Dictionary<Form, LinkedForms> _openForms;
		public static Form NextForm(Form current)
		{
			if (_openForms != null && current != null && !current.Modal)
			{
				LinkedForms fl;
				if (_openForms.TryGetValue(current, out fl))
				{
					if (fl.Next != null)
					{
						if (_openForms.ContainsKey(fl.Next))
						{
							fl.Next.Show();
							fl.Next.BringToFront();
						}
						else
						{
							fl.Next = null;
						}
					}
					return fl.Next;
				}
			}
			return null;
		}
		public static Form PreviousForm(Form current)
		{
			if (_openForms != null && current != null && !current.Modal)
			{
				LinkedForms fl;
				if (_openForms.TryGetValue(current, out fl))
				{
					if (fl.Previous != null)
					{
						if (_openForms.ContainsKey(fl.Previous))
						{
							fl.Previous.Show();
							fl.Previous.BringToFront();
						}
						else
						{
							fl.Previous = null;
						}
					}
					return fl.Previous;
				}
			}
			return null;
		}
		public static void AddForm(Form current, Form next)
		{
			if (current != null && !current.IsDisposed && !current.Modal)
			{
				if (next != null && !next.IsDisposed && !next.Modal)
				{
					if (current != next)
					{
						if (_openForms == null)
						{
							_openForms = new Dictionary<Form, LinkedForms>();
						}
						LinkedForms flCurrent = null;
						LinkedForms flNext = null;
						_openForms.TryGetValue(current, out flCurrent);
						_openForms.TryGetValue(next, out flNext);
						if (flCurrent == null)
						{
							flCurrent = new LinkedForms();
							_openForms.Add(current, flCurrent);
							current.FormClosed += new FormClosedEventHandler(f_FormClosed);
						}
						flCurrent.Next = next;
						if (flNext == null)
						{
							flNext = new LinkedForms();
							_openForms.Add(next, flNext);
							next.FormClosed += new FormClosedEventHandler(f_FormClosed);
						}
						flNext.Previous = current;
					}
				}
			}
		}
		static void f_FormClosed(object sender, FormClosedEventArgs e)
		{
			Form f = sender as Form;
			if (_openForms != null && f != null)
			{
				if (_openForms.ContainsKey(f))
				{
					_openForms.Remove(f);
				}
			}
		}
	}
	class LinkedForms
	{
		public LinkedForms()
		{
		}
		private Form _prev;
		public Form Previous { get { return _prev; } set { _prev = value; } }
		private Form _next;
		public Form Next { get { return _next; } set { _next = value; } }
	}
}