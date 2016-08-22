/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
/*
 * interfaces in this file are created for decoupling modules
 */
namespace VOB
{
	public enum enumVobNotice
	{
		NewObject = 0,
		HideToolbox,
		GetToolbox,
		ResetToolbox,
		GetTypeImage,
		ComponentAdded,
		ComponentDeleted,
		ComponentRenamed,
		ComponentSelected,
		HtmlElementSelected,
		HtmlElementRenamed,
		HtmlElementUsed,
		ObjectOpen,
		ObjectCreated,
		ObjectActivate,
		ObjectActivate2,
		ObjectChanged,
		ObjectSaved,
		ObjectClose,
		ObjectDeleted,
		ObjectNameChanged,
		ObjectSelected,
		ObjectCanCreate,
		ProjectNodeSelected,
		ProjectBuild,
		SolutionFilenameChange,
		SolutionFileCreated,
		RootMenu,
		BeforeRootComponentNameChange,
		AddFrequentType,
		RemoveFrequentType
	}
	public class PassData
	{
		public object Key;
		public object Data;
		public object Attributes;
		public PassData()
		{
		}
	}
	/// <summary>
	/// implemented by the main IDE form
	/// </summary>
	public interface InterfaceVOB
	{
		void SendNotice(enumVobNotice notice, object data);
	}
	/// <summary>
	/// implemented by classes in IDE for saving the configurations
	/// </summary>
	public interface IChangeControl
	{
		bool Dirty { get; set; }
		void ResetModified();
	}
}
