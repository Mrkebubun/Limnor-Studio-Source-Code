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
	/// <summary>
	/// Object using the undo engine
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IUndoable<T> where T : ICloneable
	{
		bool HasUndo { get; }
		bool HasRedo { get; }
		void Undo();
		void Redo();
		void ResetUndo();
		void SaveCurrentStateForUndo();
		void Apply(T obj);
		void FireStateChange();
	}
	/// <summary>
	/// undo engine using ICloneable as the mean of serialization
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MathUndoEngine<T> where T : ICloneable
	{
		#region fields and constructor
		/// <summary>
		/// all states are saved in this list sorted by time, newer states are on the top
		/// </summary>
		private List<T> undoStatck;
		/// <summary>
		/// the owner using this engine
		/// </summary>
		private IUndoable<T> _owner;
		/// <summary>
		/// undoStatck[undoPointer] = current snapshot on the screen.
		/// it is like a time indicator.
		/// undo: undoPointer++
		/// redo: undoPointer--
		/// if undoPointer is not a valid index for undoStatck then the current state has not been saved for undo
		/// </summary>
		private int undoPointer = -1;
		/// <summary>
		/// when undoPointer is a valid index for undoStatck, use this flag to indicate whether the current state is saved or not
		/// </summary>
		private bool newState;
		public MathUndoEngine(IUndoable<T> owner)
		{
			_owner = owner;
		}
		#endregion

		#region public interface
		public void ResetUndo()
		{
			undoStatck = null;
		}
		public bool HasUndo
		{
			get
			{
				return (undoStatck != null && undoStatck.Count > 0 && undoPointer < undoStatck.Count - 1);
			}
		}
		public bool HasRedo
		{
			get
			{
				return (undoStatck != null && undoStatck.Count > 0 && undoPointer > 0);
			}
		}
		/// <summary>
		/// save surrent state for undo
		/// </summary>
		/// <param name="state"></param>
		public void SaveCurrentState(T state)
		{
			if (undoStatck == null)
			{
				undoStatck = new List<T>();
			}
			if (undoPointer < 0 || undoStatck.Count == 0)
			{
				undoPointer = -1;
				undoStatck.Insert(0, (T)state.Clone());
				newState = false;
			}
			else
			{
				if (newState)
				{
					newState = false;
					undoStatck.Insert(undoPointer, (T)state.Clone());
				}
				else
				{
					newState = true;
				}
			}
			_owner.FireStateChange();
		}
		public void Undo(T current)
		{
			if (HasUndo)
			{
				T un = GetUndo(current);
				_owner.Apply(un);
				_owner.FireStateChange();
			}
		}
		public void Redo(T current)
		{
			if (HasRedo)
			{
				T un = GetRedo(current);
				_owner.Apply(un);
				_owner.FireStateChange();
			}
		}
		#endregion

		#region protected members
		/// <summary>
		/// perform undo
		/// </summary>
		/// <returns></returns>
		protected T GetUndo(T current)
		{
			if (undoStatck != null)
			{
				if (undoPointer < 0)
				{
					undoPointer = -1;
				}
				int n = undoPointer + 1;
				if (n < undoStatck.Count)
				{
					if (undoPointer < 0 || newState)
					{
						SaveCurrentState(current);
						if (undoPointer < 0)
							undoPointer = 1;
						else
						{
							undoPointer = n;
						}
					}
					else
					{
						undoPointer = n;
					}
					T state = undoStatck[undoPointer];
					return (T)state.Clone();
				}
			}
			return default(T);
		}

		/// <summary>
		/// perform redo
		/// </summary>
		/// <returns></returns>
		protected T GetRedo(T current)
		{
			if (undoStatck != null && undoPointer > 0 && undoStatck.Count > 0)
			{
				if (undoPointer > undoStatck.Count)
				{
					undoPointer = undoStatck.Count;
				}
				int n = undoPointer - 1;
				if (n >= 0)
				{
					if (undoPointer == undoStatck.Count || newState)
					{
						SaveCurrentState(current);
					}
					undoPointer = n;
					T state = undoStatck[undoPointer];
					return (T)state.Clone();
				}
			}
			return default(T);
		}
		#endregion
	}
}
