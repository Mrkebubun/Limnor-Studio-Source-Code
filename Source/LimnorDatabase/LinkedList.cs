/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorDatabase
{
	public class LinkedListItem
	{
		protected object Val;
		public LinkedListItem Next = null;
		public LinkedListItem Previous = null;
		public LinkedListItem(object v)
		{
			Val = v;
		}
		public object Value
		{
			get
			{
				return Val;
			}
			set
			{
				Val = value;
			}
		}
	}
	public class LinkedList
	{
		protected LinkedListItem first = null;
		protected LinkedListItem last = null;
		protected LinkedListItem current = null;
		public LinkedList()
		{
		}
		public bool IsEmpty
		{
			get
			{
				return (first == null);
			}
		}
		public void PushToStack(object v)
		{
			this.AddFirst(v);
		}
		public object PopFromStack()
		{
			object v = null;
			if (this.first != null)
			{
				v = this.first.Value;
				this.RemoveFirst();
			}
			return v;
		}
		public void InsertList(int n, LinkedList list)
		{
			int i = 0;
			LinkedListItem theFirst = list.First;
			LinkedListItem theLast = list.Last;
			if (first == null)
			{
				first = theFirst;
			}
			else
			{
				LinkedListItem c = first;
				while (c != null)
				{
					if (i >= n)
						break;
					i++;
					c = c.Next;
				}
				if (c == null)
				{
					last.Next = theFirst;
					theFirst.Previous = last;
					last = theLast;
				}
				else if (c.Next == null)
				{
					last.Next = theFirst;
					theFirst.Previous = last;
					last = theLast;
				}
				else if (c.Previous == null)
				{
					first.Previous = theLast;
					theLast.Next = first;
					first = theFirst;
				}
				else
				{
					theFirst.Previous = c;
					c.Next.Previous = theLast;
					theLast.Next = c.Next;
					c.Next = theFirst;
				}
			}
		}
		public void Append(LinkedList lst)
		{
			LinkedListItem f = lst.First;
			LinkedListItem l = lst.Last;
			if (f != null && l != null)
			{
				if (first == null)
				{
					first = f;
					last = l;
				}
				else
				{
					last.Next = f;
					f.Previous = last;
					last = l;
				}
			}
		}
		public LinkedListItem First
		{
			get
			{
				current = first;
				return first;
			}
		}
		public LinkedListItem Last
		{
			get
			{
				current = last;
				return last;
			}
		}
		public LinkedListItem CutOff(object v)
		{
			LinkedListItem item = first;
			while (item != null)
			{
				if (item.Value == v)
				{
					if (item.Previous == null)
					{
						first = null;
						last = null;
						current = null;
					}
					else
					{
						item.Previous.Next = null;
						last = item.Previous;
					}
					return item;
				}
				else
				{
					item = item.Next;
				}
			}
			return null;
		}
		public void Clear()
		{
			current = null;
			first = null;
			last = null;
		}
		public void RemoveLast()
		{
			if (last != null)
			{
				bool bSetCur = (current == last);
				if (last.Previous == null)
				{
					last = null;
					first = null;
					current = null;
				}
				else
				{
					last.Previous.Next = null;
					last = last.Previous;
					if (bSetCur)
					{
						current = null;
					}
				}
			}
		}
		public void RemoveFirst()
		{
			if (first != null)
			{
				bool bSetCur = (current == first);
				if (first.Next == null)
				{
					last = null;
					first = null;
					current = null;
				}
				else
				{
					first.Next.Previous = null;
					first = first.Next;
					if (bSetCur)
					{
						current = null;
					}
				}
			}
		}
		public void Remove(object v)
		{
			if (first != null)
			{
				LinkedListItem item = first;
				while (item != null)
				{
					if (v == item.Value)
					{
						LinkedListItem prev = item.Previous;
						if (prev == null)
						{
							//item is the first
							RemoveFirst();
						}
						else
						{
							LinkedListItem next = item.Next;
							if (next == null)
							{
								RemoveLast();
							}
							else
							{
								prev.Next = next;
								next.Previous = prev;
							}
						}
						break;
					}
					else
					{
						item = item.Next;
					}
				}
			}
		}
		public LinkedListItem Current
		{
			get
			{
				return current;
			}
		}
		public LinkedListItem Add(object v)
		{
			LinkedListItem item = new LinkedListItem(v);
			if (first == null)
			{
				first = item;
				last = item;
			}
			else
			{
				last.Next = item;
				item.Previous = last;
				last = item;
			}
			current = item;
			return item;
		}
		public LinkedListItem AddFirst(object v)
		{
			LinkedListItem item = new LinkedListItem(v);
			if (first == null)
			{
				first = item;
				last = item;
			}
			else
			{
				first.Previous = item;
				item.Next = first;
				first = item;
			}
			current = item;
			return item;
		}
		public LinkedListItem AddLast(object v)
		{
			LinkedListItem item = new LinkedListItem(v);
			if (first == null)
			{
				first = item;
				last = item;
			}
			else
			{
				last.Next = item;
				item.Previous = last;
				last = item;
			}
			current = item;
			return item;
		}
		public LinkedListItem Next
		{
			get
			{
				if (current == null)
				{
					current = first;
				}
				else
				{
					current = current.Next;
				}
				return current;
			}
		}
		public LinkedListItem Previous
		{
			get
			{
				if (current == null)
				{
					current = last;
				}
				else
				{
					current = current.Previous;
				}
				return current;
			}
		}
		public int Count
		{
			get
			{
				int n = 0;
				LinkedListItem item = first;
				while (item != null)
				{
					n++;
					item = item.Next;
				}
				return n;
			}
		}
	}

}
