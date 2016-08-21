/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.Drawing2D
{
	public interface IDrawingPage
	{
		PageAttrs PageAttributes { get; set; }
		IDrawingOwnerPointer DesignerPointer { get; }
		DrawingLayer GetLayerById(Guid id);
		void Refresh();
		DrawingLayerCollection DrawingLayers { get; }
		bool InDesignMode { get; }
	}

	public interface IDrawingPageHolder
	{
		IDrawingPage GetDrawingPage();
	}
	public interface IDrawingRepeaterItem
	{
		void OnInputTextChanged(ITextInput txt, string newText);
	}
}
