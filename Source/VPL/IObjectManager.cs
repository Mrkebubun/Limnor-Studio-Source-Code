/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;
using System.Reflection;

namespace VPL
{
	/// <summary>
	/// object manager to be implemented by abstract types.
	/// it manages instances as a one level collection (no hierarchy).
	/// </summary>
	public interface IObjectManager
	{
		/// <summary>
		/// for initialization
		/// </summary>
		/// <param name="node">initialization data</param>
		/// <param name="typeProvider">current assembly</param>
		void LoadMap(XmlNode node, Assembly typeProvider);
		/// <summary>
		/// instance count
		/// </summary>
		int ItemCount(string xtPath);
		/// <summary>
		/// indexer data type
		/// </summary>
		Type IndexerType { get; }
		/// <summary>
		/// instance identified by indexer
		/// </summary>
		/// <param name="indexer"></param>
		/// <returns></returns>
		object Item(string xtPath, object indexer);
		/// <summary>
		/// instance identified by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		object ItemByName(string xtPath, string name);
		/// <summary>
		/// current indexer for identifying the current instance
		/// </summary>
		object CurrentIndexer(string xtPath);
		/// <summary>
		/// set the current indexer
		/// </summary>
		/// <param name="xtPath"></param>
		/// <param name="indexer"></param>
		void SetCurrebtIndexer(string xtPath, object indexer);
		/// <summary>
		/// current instance
		/// </summary>
		object CurrentItem(string xtPath);
		/// <summary>
		/// newly created instance by calling CreateItem
		/// </summary>
		object NewItem(string xtPath);
		/// <summary>
		/// remove the instance identified by the indexer
		/// </summary>
		/// <param name="indexer"></param>
		void RemoveItem(string xtPath, object indexer);
		/// <summary>
		/// remove the instance identified by the name
		/// </summary>
		/// <param name="name"></param>
		void RemoveItemByName(string xtPath, string name);
		/// <summary>
		/// remove all instances
		/// </summary>
		void RemoveAll(string xtPath);
		/// <summary>
		/// create a new instance
		/// </summary>
		/// <param name="name"></param>
		void CreateItem(string xtPath, string name);
		/// <summary>
		/// find the item by the path and enumerate all instance
		/// </summary>
		/// <param name="xtPath"></param>
		/// <param name="handler"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		bool WorkOnAllInstances(string xtPath, delegateOnGetInstance handler, object data);
		/// <summary>
		/// find the element by path
		/// </summary>
		/// <param name="xtPath"></param>
		/// <returns></returns>
		IObjectManager FindElement(string xtPath);
		/// <summary>
		/// make the next item the current item
		/// </summary>
		/// <param name="xtPath"></param>
		void MoveNext(string xtPath);
		/// <summary>
		/// make the previous item the current item
		/// </summary>
		/// <param name="xtPath"></param>
		void MoveBack(string xtPath);
	}
}
