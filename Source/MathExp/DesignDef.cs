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
using System.ComponentModel;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using System.CodeDom;
using MathExp.RaisTypes;
using System.Windows.Forms.Design;
using System.Reflection;
using ProgElements;
using VPL;
using System.Collections.Specialized;

namespace MathExp
{
	public interface IMethodCompile
	{
		CodeTypeDeclaration TypeDeclaration { get; set; }
		CodeMemberMethod MethodCode { get; set; }
		UInt32 MethodID { get; set; }
		string MethodName { get; set; }
		bool IsStatic { get; }
		bool HasReturn { get; }
		string GetParameterCodeNameById(UInt32 id);
		object CompilerData(string key);
		/// <summary>
		/// when compiling a sub method, set the sub-method here to pass it into compiling elements
		/// </summary>
		Stack<IMethod0> SubMethod { get; }
		IMethod GetSubMethodByParameterId(UInt32 parameterId);
		bool MakeUIThreadSafe { get; set; }
		object ModuleProject { get; }
	}
	/// <summary>
	/// implemented by ParameterValue
	/// </summary>
	public interface ICompileableItem : ICloneable
	{
		CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue);
		string CreateJavaScript(StringCollection method);
		string CreatePhpScript(StringCollection method);
		PropertyDescriptorCollection GetProperties(Attribute[] attributes);
		void SetParameterValueChangeEvent(EventHandler h);
		void SetValue(object value);
		void SetDataType(Type tp);
		void SetCloneOwner(IActionContext o);
		void SetCustomMethod(IMethod m);
		void GetValueSources(List<ISourceValuePointer> list);
		IList<ISourceValuePointer> GetValueSources();
		//
		string Name { get; set; }
		UInt32 ParameterID { get; set; }
		IMethod OwnerMethod { get; }
	}
	/// <summary>
	/// implemented by ParameterValue
	/// </summary>
	public interface IXmlSerializeItem
	{
		/// <summary>
		/// let the item save/load itself
		/// </summary>
		/// <param name="serializer">XmlObjectWriter/XmlObjectReader</param>
		/// <param name="node">XmlNode holding the item</param>
		/// <param name="saving">true:save to node; false:load from node</param>
		void ItemSerialize(object serializer, XmlNode node, bool saving);
		/// <summary>
		/// when the instance is an item in variable map, set the owner of the map
		/// so that variable type can be retrieved
		/// </summary>
		void SetMapOwner(MathNodeRoot owner);
	}
	public enum EnumPropAccessType
	{
		All = 0,
		CanRead,
		CanWrite,
		CanReadWrite
	}
	/// <summary>
	/// provides original values to expression
	/// it is a simplified IObjectPointer.
	/// IObjectPointer inherits it
	/// </summary>
	public interface IPropertyPointer : IVplObjectPointer
	{
		string CodeName { get; }
		Type ObjectType { get; set; }
		IPropertyPointer PropertyOwner { get; }
	}
	public interface ISourceValuePointer
	{
		object ValueOwner { get; } //it is an IObjectPointer 
		string DataPassingCodeName { get; } //value being passed between client/server using this variable name
		UInt32 TaskId { get; } //value-pointer can be used in different actions and value may change
		void SetTaskId(UInt32 taskId);
		void SetValueOwner(object o);
		bool IsSameProperty(ISourceValuePointer p);
		bool IsWebClientValue();
		bool IsWebServerValue();
		string CreateJavaScript(StringCollection method);
		string CreatePhpScript(StringCollection method);
		CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue);
		bool CanWrite { get; }
		bool IsMethodReturn { get; }
	}
	public interface ISourceValuePointersHolder
	{
		IList<ISourceValuePointer> GetValueSources();
	}
	public interface INameCreator
	{
		string CreateNewName(string baseName);
	}
	public interface IDesignServiceProvider
	{
		object GetDesignerService(Type serviceType);
		void AddDesignerService(Type serviceType, object service);
	}
	public interface ILocalServiceProvider
	{
		object GetLocalService(Type serviceType);
		void AddLocalService(Type serviceType, object service);
	}
	public interface IUndoHost
	{
		void AddUndoEntity(UndoEntity entity);
		bool DisableUndo { get; set; }
		Control GetUndoControl(UInt32 key);
	}
	public interface IChangeControl2
	{
		bool Changed { get; set; }
	}
	/// <summary>
	/// currently implemented by DiagramViewer and MethodDiagramViewer
	/// </summary>
	public interface IMathDesigner : IDesignServiceProvider, IUndoHost, IChangeControl2
	{
		void DeleteComponent(IComponent c);
		void DeleteSelectedComponents();
		void ReSelectComponent(IComponent c);
		bool NoCompoundCreation { get; set; }
		void ExecuteOnShowHandlers();
		void ShowProperties(object v);
		void CreateValue(RaisDataType type, LinkLineNodeInPort port, Point pos);
		bool TestDisabled { get; }
		void CreateCompound(Point pos, Size size);
	}
	/// <summary>
	/// controls on design surface but not a design component
	/// </summary>
	public interface INonHostedItem
	{
		Control[] GetControls();
		void InitLinkLine();
		void InitLocation();
	}
	/// <summary>
	/// represent one math expression.
	/// currently implemented by MathNodeRoot (a single math expression) and MathExpGroup (a group of math expressions linked together as a single math expression).
	/// ???it doesn't allow mixing client and server values. RunAt indicates what kind of values it is using.
	/// </summary>
	public interface IMathExpression : IComponent, ICloneable, IXmlNodeSerializable, ILocalServiceProvider, ICustomSerialization, IMethod, IMethodElement
	{
		string Name { get; set; }
		/// <summary>
		/// unique ID. 
		/// For MathNodeRoot, the ID for the varDummy is used.
		/// For MathExpGroup, the ID for the ReturnIcon variable is used.
		/// For Parameter, the ID for variable of its out-port is used.
		/// </summary>
		UInt32 ID { get; }
		bool IsContainer { get; }
		string Description { get; set; }
		void ReplaceVariableID(UInt32 currentID, UInt32 newID);
		IVariable GetVariableByID(UInt32 id);
		IVariable GetVariableByKeyName(string keyname);
		IVariable OutputVariable { get; }
		VariableList InputVariables { get; }
		VariableList InportVariables { get; }
		VariableList DummyInputs(bool create);
		VariableList FindAllInputVariables();
		void GetPointers(Dictionary<string, IPropertyPointer> pointers);
		Bitmap CreateIcon(Graphics g);
		IMathEditor CreateEditor(Rectangle rcStart);
		CompileResult CreateMethodCompilerUnit(string classNamespace, string className, string methodName);
		void ClearFocus();
		/// <summary>
		/// return data type
		/// </summary>
		RaisDataType DataType { get; }
		/// <summary>
		/// return data type for compile overriden by the compiler
		/// </summary>
		RaisDataType CompileDataType { get; set; }
		/// <summary>
		/// compiler sets it. true: use CompileDataType as the return type; false: use DataType as the return type
		/// </summary>
		bool UseCompileDataType { get; set; }
		void SetDataType(RaisDataType type);
		/// <summary>
		/// it can be DataType or CompileDataType, depending on the implementations.
		/// the value may depends on the call of ReturnCodeExpression.
		/// </summary>
		RaisDataType ActualCompileDataType { get; }
		/// <summary>
		/// find out the scopes of all variables
		/// </summary>
		void GenerateInputVariables();
		/// <summary>
		/// assign the code expression to all the variables with the same CodeVariableName
		/// </summary>
		/// <param name="code">code to be used fpr compiling</param>
		/// <param name="codeVarName">CodeVariableName of the variables to search for</param>
		void AssignCodeExp(CodeExpression code, string codeVarName);
		void AssignJavaScriptCodeExp(string code, string codeVarName);
		void AssignPhpScriptCodeExp(string code, string codeVarName);

		/// <summary>
		/// find out all assemblies imported
		/// </summary>
		/// <param name="imports"></param>
		void GetAllImports(AssemblyRefList imports);
		/// <summary>
		/// generate code
		/// </summary>
		/// <param name="supprtStatements"></param>
		/// <returns></returns>
		CodeExpression ReturnCodeExpression(IMethodCompile method);//CodeStatementCollection supprtStatements);
		string ReturnJavaScriptCodeExpression(StringCollection methodCode);
		string ReturnPhpScriptCodeExpression(StringCollection methodCode);
		string CreateJavaScript(StringCollection code);
		string CreatePhpScript(StringCollection code);
		void PrepareDrawInDiagram();
		/// <summary>
		/// searches children to find the item
		/// </summary>
		/// <param name="portId"></param>
		/// <returns></returns>
		MathExpItem GetItemByID(UInt32 portId);
		MathExpItem RemoveItemByID(UInt32 portId);
		void AddItem(IMathExpression item);
		MathExpItem ContainerMathItem { get; set; }
		IMathExpression RootContainer { get; }
		void PrepareForCompile(IMethodCompile method);
		void SetServiceProvider(IDesignServiceProvider designServiceProvider);
		/// <summary>
		/// find the XML document of the RAIS schema
		/// </summary>
		/// <returns></returns>
		XmlDocument GetXmlDocument();
		bool ContainLibraryTypesOnly { get; }
		/// <summary>
		/// if it is true then it cannot be edited
		/// </summary>
		bool Fixed { get; set; }
		string TraceInfo { get; }
		void SetParameterMethodID(UInt32 methodID);
		/// <summary>
		/// set the code for referencing parameter to array element
		/// </summary>
		/// <param name="ps"></param>
		void SetParameterReferences(Parameter[] ps);
		void SetParameterValueChangeEvent(EventHandler h);
		/// <summary>
		/// when this object is used as an action parameter value, a static method is created
		/// using ReturnCodeExpression as if creating a test method. This function returns the
		/// method created.
		/// </summary>
		/// <param name="propertyOwner">object for accessing the properties</param>
		/// <param name="pointers">property pointers. every one corresponds to a method parameter</param>
		/// <returns></returns>
		MethodInfo GetCalculationMethod(object propertyOwner, List<IPropertyPointer> pointers);
		/// <summary>
		/// when it is used within an action definition, this is the action.
		/// It is needed before loading from XML if variable map is used.
		/// It is needed for creating other objects in action-context, such as ParameterValue
		/// </summary>
		IActionContext ActionContext { get; set; }
		/// <summary>
		/// the type for the mapped instance.
		/// currently it is ParameterValue.
		/// It is needed before loading from XML if variable map is used
		/// </summary>
		Type VariableMapTargetType { get; set; }
		bool EnableUndo { get; set; }
		IList<ISourceValuePointer> GetValueSources();
		IList<ISourceValuePointer> GetUploadProperties(UInt32 taskId);
		EnumWebRunAt RunAt { get; }
		bool IsValid { get; }
	}
	/// <summary>
	/// viewer for MathExpGroup
	/// </summary>
	public interface IMathExpViewer : IActiveDrawing
	{
		MathExpItem ExportMathItem(MathExpGroup group);
		IVariable FindVariableById(UInt32 id);
	}
	/// <summary>
	/// animated dialogue box for math expression editing.
	/// </summary>
	public interface IMathEditor
	{
		IMathExpression MathExpression { get; set; }
		IActionContext ActionContext { get; set; }
		Type VariableMapTargetType { get; set; }
		void AddService(Type t, object service);
		void DisableTest();
		void SetScopeMethod(IMethod method);
		void AddMathNode(Type t);
	}
	public interface IImageItem
	{
		ImageID ImageItem { get; }
	}
	public interface IImageList
	{
		List<ImageID> ImageList { get; }
	}
	public interface IComponentWithID
	{
		UInt32 ComponentID { get; }
		bool ContainsID(UInt32 id);
	}
	/// <summary>
	/// only Control should implement it
	/// </summary>
	public interface IControlWithID
	{
		UInt32 ControlID { get; }
	}
	/// <summary>
	/// control with id
	/// </summary>
	public interface IActiveDrawing
	{
		UInt32 ActiveDrawingID { get; }
		/// <summary>
		/// after pasting a control, id needs to be reset
		/// </summary>
		void ResetActiveDrawingID();
	}
	/// <summary>
	/// </summary>
	public interface IRaisTypeResolver
	{
		/// <summary>
		/// resolve a type under development.
		/// if the type has been compiled then return the compiled type.
		/// if the type has not been compiled then return the closet parent type from existing library.
		/// </summary>
		/// <param name="objRef"></param>
		/// <returns></returns>
		Type ResolveRaisType(ObjectRef objRef);
	}
	/// <summary>
	/// implemented by MethodDesignerHolder
	/// </summary>
	public interface IObjectRefSelector
	{
		IDataSelectionControl GetSelector(MathNodeObjRef mo);
		IDataSelectionControl GetPropertySelector(ObjectRef owner, EnumPropAccessType access);
	}
	/// <summary>
	/// implemented by IXDesigner
	/// </summary>
	public interface IXDesignerHost
	{
		/// <summary>
		/// select a property from the owner
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="access"></param>
		/// <returns></returns>
		ObjectRef SelectProperty(ObjectRef owner, EnumPropAccessType access);
		IDataSelectionControl CreatePropertySelector(ObjectRef owner, EnumPropAccessType access);
	}
	public interface IRaisCodeCompiler
	{
		string CurrentXPath { get; }
	}
	#region Unit Test
	/// <summary>
	/// unit test object
	/// </summary>
	public interface ITestData
	{
		bool Finished { get; set; }
	}
	/// <summary>
	/// dialog for unit test
	/// </summary>
	public interface ITestDialog
	{
		bool LoadData(ITestData data);
	}
	/// <summary>
	/// the object initiate the unit test.Implemented by DiagramDesignerHolder and MathExpEditor
	/// </summary>
	public interface IUnitTestOwner
	{
		void AddMethod(CodeTypeDeclaration t, CodeNamespace ns, CodeMemberMethod testMethod, AssemblyRefList imports, VariableList parameters, List<IPropertyPointer> pointers);
		IMathExpression MathExpression { get; }
	}
	public interface IObjectTypeUnitTester
	{
		/// <summary>
		/// if members are used then a XType compilation is doen and a test dialogue is displayed. 
		/// if no members are used then no test dialogue is created
		/// </summary>
		/// <param name="mathExp">the math expression to test</param>
		/// <returns>null: no test dialogue is created</returns>
		ITestData UseMemberTest(IUnitTestOwner mathExp);
	}
	#endregion
}
