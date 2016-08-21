/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;

namespace LimnorDesigner.EventMap
{
	public interface IEventMapSource : IPortOwner
	{
		ClassPointer RootPointer { get; }
	}
}
