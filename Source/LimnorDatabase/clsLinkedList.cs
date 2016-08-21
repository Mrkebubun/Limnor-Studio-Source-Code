using System;

namespace LimnorDatabase
{
	public class clsListItem
	{
		public object obj=null;
		public clsListItem next=null;
		public clsListItem prev=null;
		public clsListItem(object v)
		{
			obj = v;
		}
	}
	/// <summary>
	/// Simple linked list. Not thread-safe
	/// </summary>
	public class clsLinkedList
	{
		private clsListItem first=null;
		private clsListItem last=null;
		private clsListItem current=null;
		public clsLinkedList()
		{
			//
			//
		}
		public int count
		{
			get
			{
				lock(this)
				{
					int n = 0;
					clsListItem d = first;
					while(d != null )
					{
						n++;
						d = d.next;
					}
					return n;
				}
			}
		}
		public clsListItem FirstItem
		{
			get
			{
				return first;
			}
		}
		public clsListItem LastItem
		{
			get
			{
				return last;
			}
		}
		public virtual void Add(object obj)
		{
			lock(this)
			{
				if( first == null || last == null)
				{
					first = new clsListItem(obj);
					last = first;
					current = first;
				}
				else
				{
					current = new clsListItem(obj);
					current.prev = last;
					last.next = current;
					last = current;
				}
			}
		}
		public virtual object PopFirst()
		{
			lock(this)
			{
				if( first != null)
				{
					object v = first.obj;
					first = first.next;
					if(first != null)
					{
						first.prev = null;
					}
					current = first;
					return v;
				}
				return null;
			}
		}
		/// <summary>
		/// calling function should place lock to ensure thread-safe
		/// </summary>
		public void DeleteCurrent()
		{
			if( current != null )
			{
				if( current.prev == null )
				{
					//it is the first
					first = current.next;
					if( first == null )
					{
						last = null;
					}
					else
						first.prev = null;
					current = first;
				}
				else
				{
					if( current.next == null )
					{
						//it is the last
						last = current.prev;
						current = last;
						current.next = null;
					}
					else
					{
						//first and last will not change
						//connect prev and next together.
						//current.prev ->current ->current.next
						current.next.prev = current.prev;
						current.prev.next = current.next;
						current = current.next;
					}
				}
			}
		}
		public void Clear()
		{
			lock(this)
			{
				first = null;
				last = null;
				current = null;
			}
		}
		public object Current
		{
			get
			{
				if(current != null)
					return current.obj;
				return null;
			}
		}
		public object First
		{
			get
			{
				current = first;
				if( current != null )
					return current.obj;
				return null;
			}
		}
		public object Last
		{
			get
			{
				current = last;
				if( current != null )
					return current.obj;
				return null;
			}
		}
		public object Next
		{
			get
			{
				if( current != null )
				{
					current = current.next;
				}
				if( current != null )
					return current.obj;
				return null;
			}
		}
		public object Previous
		{
			get
			{
				if( current != null )
				{
					current = current.prev;
				}
				if( current != null )
					return current.obj;
				return null;
			}
		}
		public bool setCurrent(object v)
		{
			bool bRet = false;
			clsListItem obj = first;
			while(obj != null )
			{
				if( obj.obj == v )
				{
					current = obj;
					return true;
				}
				obj = obj.next;
			}
			return bRet;
		}
		public bool MoveUp(object v)
		{
			bool bRet = false;
			clsListItem obj = first;
			clsListItem prev,next;
			while(obj != null )
			{
				if( obj.obj == v )
				{
					if( obj == first )
						break;
					current = obj;
					prev = obj.prev;
					next = obj.next;
					//
					if( prev.prev == null )
					{
						//move to the first
						first = obj;
						obj.prev = null;
						obj.next = prev;
						prev.prev = obj;
					}
					else
					{
						prev.prev.next = obj;
						obj.prev = prev.prev;
						obj.next = prev;
						prev.prev = obj;
					}
					prev.next = next;
					if( next == null )
					{
						last = prev;
					}
					else
					{
						next.prev = prev;
					}
					return true;
				}
				obj = obj.next;
			}
			return bRet;
		}
		public bool MoveDown(object v)
		{
			bool bRet = false;
			clsListItem obj = first;
			clsListItem prev,next;
			while(obj != null )
			{
				if( obj.obj == v )
				{
					if( obj == last )
						break;
					current = obj;
					prev = obj.prev;
					next = obj.next;
					//
					if( next.next == null )
					{
						//move to the last
						last = obj;
						obj.next = null;
						obj.prev = next;
						next.next = obj;
					}
					else
					{
						next.next.prev = obj;
						obj.next = next.next;
						obj.prev = next;
						next.next = obj;
					}
					next.prev = prev;
					if( prev == null )
					{
						first = next;
					}
					else
					{
						prev.next = next;
					}
					return true;
				}
				obj = obj.next;
			}
			return bRet;
		}
	}
}
