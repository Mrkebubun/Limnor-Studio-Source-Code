/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using VPL;

namespace Limnor.Drawing2D
{
	public interface IDrawDesignControl : IPostDeserializeProcess
	{
		[ReadOnly(true)]
		DrawingItem Item
		{
			get;
			set;
		}
		[ReadOnly(true)]
		[Browsable(false)]
		bool Destroying
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		Guid DrawingId
		{
			get;
		}
		[Description("Gets or sets an integer that specifies the order in which a series is rendered from front to back. For overlapping drawing objects, object with the highest ZOrder generates mouse events.")]
		int ZOrder
		{
			get;
			set;
		}
		[Browsable(false)]
		Guid LayerId
		{
			get;
			set;
		}
		string Name { get; }
		bool IsPropertyReadOnly(string propertyName);
		bool ExcludeFromInitialize(string propertyName);
		void BringToFront();
	}
	public interface IDrawDesignControlParent
	{
		void OnChildZOrderChanged(IDrawDesignControl itemControl);
	}
}
