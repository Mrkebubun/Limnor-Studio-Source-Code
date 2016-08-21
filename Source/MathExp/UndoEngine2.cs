/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace MathExp
{
	public interface IUndoUnit
	{
		void Apply();
	}
	/// <summary>
	/// an undo unity is responsible for doing both undo and redo
	/// </summary>
	public class UndoEntity
	{
		private IUndoUnit _undoObject;
		private IUndoUnit _redoObject;
		public UndoEntity(IUndoUnit undo, IUndoUnit redo)
		{
			_undoObject = undo;
			_redoObject = redo;
		}
		public IUndoUnit UndoObject
		{
			get
			{
				return _undoObject;
			}
		}
		public IUndoUnit RedoObject
		{
			get
			{
				return _redoObject;
			}
		}
	}
	enum EnumUndoStackPosition { AtRedo = 0, AtUndo }
	public class UndoEngine2 : LinkedList<UndoEntity>
	{
		private LinkedListNode<UndoEntity> _currentItem;
		private EnumUndoStackPosition _position;
		public UndoEngine2()
		{
		}
		private void cutoff()
		{
			if (_currentItem == null)
			{
				this.Clear();
			}
			else
			{
				LinkedListNode<UndoEntity> item = First;
				while (item != _currentItem)
				{
					this.RemoveFirst();
					item = First;
					if (item == null)
					{
						throw new MathException("UndoEngin error: current location is not in the undo list");
					}
				}
			}
		}
		public void ClearStack()
		{
			_currentItem = null;
			this.Clear();
		}
		public void AddUndoEntity(UndoEntity entity)
		{
			cutoff();
			if (_currentItem != null)
			{
				if (_position == EnumUndoStackPosition.AtUndo)
				{
					RemoveFirst();
				}
				else
				{
					if (entity.UndoObject is SizeUndo && _currentItem.Value.UndoObject is SizeUndo)
					{
						if (((SizeUndo)entity.UndoObject).Key == ((SizeUndo)_currentItem.Value.UndoObject).Key)
						{
							RemoveFirst();
						}
					}
				}
			}
			this.AddFirst(entity);
			_currentItem = this.First;
			_position = EnumUndoStackPosition.AtRedo;
		}
		public bool HasUndo
		{
			get
			{
				if (_currentItem != null)
				{
					if (_position == EnumUndoStackPosition.AtRedo)
					{
						return (_currentItem.Value.UndoObject != null);
					}
					else
					{
						if (_currentItem.Next != null)
						{
							return (_currentItem.Next.Value.UndoObject != null);
						}
					}
				}
				return false;
			}
		}
		public bool HasRedo
		{
			get
			{
				if (_currentItem != null)
				{
					if (_position == EnumUndoStackPosition.AtRedo)
					{
						if (_currentItem.Previous != null)
						{
							return (_currentItem.Previous.Value.RedoObject != null);
						}
					}
					else
					{
						return (_currentItem.Value.RedoObject != null);
					}
				}
				return false;
			}
		}
		public IUndoUnit UseUndo()
		{
			IUndoUnit obj = null;
			if (_currentItem != null)
			{
				if (_position == EnumUndoStackPosition.AtRedo)
				{
					obj = _currentItem.Value.UndoObject;
					_position = EnumUndoStackPosition.AtUndo;
				}
				else
				{
					if (_currentItem.Next != null)
					{
						_currentItem = _currentItem.Next;
						obj = _currentItem.Value.UndoObject;
					}
				}
			}
			return obj;
		}
		public IUndoUnit UseRedo()
		{
			IUndoUnit obj = null;
			if (_currentItem != null)
			{
				if (_position == EnumUndoStackPosition.AtRedo)
				{
					if (_currentItem.Previous != null)
					{
						_currentItem = _currentItem.Previous;
						obj = _currentItem.Value.RedoObject;
					}
				}
				else
				{
					obj = _currentItem.Value.RedoObject;
					_position = EnumUndoStackPosition.AtRedo;
				}
			}
			return obj;
		}
	}
}
