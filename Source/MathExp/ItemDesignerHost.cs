/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace MathExp
{
	/// <summary>
	/// designer host to make use of .Net Form Designer Framework
	/// </summary>
	public partial class ItemDesignerHost : UserControl, IUndoHost
	{
		#region fields and constructors
		private DesignSurface dsf;
		private Diagram root;
		private bool _disableUndo;
		private UndoEngine2 _undoEngine;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="designer"></param>
		/// <param name="rootComponentType">it can be null. if not null then it must be a type derived from Diagram</param>
		public ItemDesignerHost(IXDesignerHost designer, Type rootComponentType)
		{
			InitializeComponent();
			_undoEngine = new UndoEngine2();
			if (rootComponentType == null)
				dsf = new DesignSurface(typeof(Diagram));
			else
				dsf = new DesignSurface(rootComponentType);
			Control control = dsf.View as Control;
			this.Controls.Add(control);
			control.Dock = DockStyle.Fill;
			control.Visible = true;
			//
			IDesignerHost host = (IDesignerHost)dsf.GetService(typeof(IDesignerHost));
			root = (Diagram)host.RootComponent;
			root.Dock = DockStyle.Fill;
			//
			DesignMessageFilter filter = new DesignMessageFilter(dsf);
			Application.AddMessageFilter(filter);
		}
		public ItemDesignerHost(IXDesignerHost designer)
			: this(designer, typeof(Diagram))
		{
		}
		#endregion
		#region properties
		public Component RootComponent
		{
			get
			{
				return root;
			}
		}
		public IDesignerHost DesignerHost
		{
			get
			{
				return (IDesignerHost)dsf.GetService(typeof(IDesignerHost));
			}
		}
		#endregion
		#region IUndoHost Members
		public void ResetUndo()
		{
			_undoEngine.ClearStack();
		}
		[Browsable(false)]
		public bool HasUndo
		{
			get
			{
				return _undoEngine.HasUndo;
			}
		}
		[Browsable(false)]
		public bool HasRedo
		{
			get
			{
				return _undoEngine.HasRedo;
			}
		}
		public void Undo()
		{
			IUndoUnit obj = _undoEngine.UseUndo();
			if (obj != null)
			{
				obj.Apply();
				OnHasUndoChange();
			}
		}
		public void Redo()
		{
			IUndoUnit obj = _undoEngine.UseRedo();
			if (obj != null)
			{
				obj.Apply();
				OnHasUndoChange();
			}
		}
		public void AddUndoEntity(UndoEntity entity)
		{
			if (!_disableUndo)
			{
				_undoEngine.AddUndoEntity(entity);
				OnHasUndoChange();
			}
		}
		public Control GetUndoControl(UInt32 key)
		{
			return root.GetActiveDrawingById(key);
		}
		/// <summary>
		/// programmically disable undo for some operations
		/// </summary>
		[Browsable(false)]
		public bool DisableUndo
		{
			get
			{
				return _disableUndo;
			}
			set
			{
				_disableUndo = value;
			}
		}
		/// <summary>
		/// event for change UI
		/// </summary>
		public virtual void OnHasUndoChange()
		{
		}
		#endregion
	}
}
