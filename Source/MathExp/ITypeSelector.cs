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
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace MathExp
{
	/// <summary>
	/// for dropdown control or modal dialogue box
	/// </summary>
	public interface IDataSelectionControl
	{
		object UITypeEditorSelectedValue { get; }
		void SetCaller(IWindowsFormsEditorService wfe);
	}
	public interface IMethodNode
	{
		void SetFunction(object func);
		object GetFunction();
		string GetFunctionName();
	}
	/// <summary>
	/// it is a service for selecting data type
	/// </summary>
	public interface ITypeSelector
	{
		bool GetPaintValueSupported(ITypeDescriptorContext context);
		void PaintValue(System.Drawing.Design.PaintValueEventArgs e);
		IDataSelectionControl GetUIEditorDropdown(ITypeDescriptorContext context, IServiceProvider provider, object value);
		IDataSelectionControl GetUIEditorModal(ITypeDescriptorContext context, IServiceProvider provider, object value);
		UITypeEditorEditStyle GetUIEditorStyle(ITypeDescriptorContext context);
	}
	/// <summary>
	/// it is a service for selecting data value
	/// </summary>
	public interface IValueSelector
	{
		bool GetPaintValueSupported(ITypeDescriptorContext context);
		void PaintValue(System.Drawing.Design.PaintValueEventArgs e);
		IDataSelectionControl GetUIEditorDropdown(ITypeDescriptorContext context, IServiceProvider provider, object value);
		IDataSelectionControl GetUIEditorModal(ITypeDescriptorContext context, IServiceProvider provider, object value);
		UITypeEditorEditStyle GetUIEditorStyle(ITypeDescriptorContext context);
	}
	/// <summary>
	/// it is a service for selecting method
	/// </summary>
	public interface IMethodSelector
	{
		bool GetPaintValueSupported(ITypeDescriptorContext context);
		void PaintValue(System.Drawing.Design.PaintValueEventArgs e);
		IDataSelectionControl GetUIEditorDropdown(ITypeDescriptorContext context, IServiceProvider provider, object value);
		IDataSelectionControl GetUIEditorModal(ITypeDescriptorContext context, IServiceProvider provider, object value);
		UITypeEditorEditStyle GetUIEditorStyle(ITypeDescriptorContext context);
	}

	public interface IMethodPointerNode
	{
		UInt32 ID { get; }
		string MethodName { get; }
		object MethodExecuter { get; }
		object MethodObject { get; } //a IMethodPointer
		Type ReturnBaseType { get; }
	}
}
