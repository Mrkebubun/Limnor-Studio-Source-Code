/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlSerializer;
using VSPrj;
using XmlUtility;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Property;
using System.Windows.Forms;
using LimnorDesigner.Action;
using MathExp;
using System.Drawing;
using MathExp.RaisTypes;
using VPLDrawing;
using LimnorDesigner.Event;
using LimnorDesigner.Interface;
using VPL;
using System.Web.Services;
using LimnorDesigner.EventMap;
using System.ComponentModel;
using LimnorDatabase;
using LimnorDesigner.ResourcesManager;
using Limnor.WebServerBuilder;
using System.CodeDom;
using Limnor.WebBuilder;
using LimnorDesigner.Web;
using LimnorWebBrowser;
using LimnorVisualProgramming;
using LimnorDesigner.DesignTimeType;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using Limnor.CopyProtection;
using LimnorWeb;

namespace LimnorDesigner
{
	/// <summary>
	/// IDesignService is defined in module XmlSerializer.
	/// This class extend the design service to any module referencing XmlSerializer
	/// </summary>
	public class DesignService : IDesignService
	{
		#region fields and constructors
		static DesignService()
		{
			MathNode.GetTypeConversion = _getTypeConversion;
			ObjectIDmap.DesignService = new DesignService();
		}
		static CodeExpression _getTypeConversion(Type targetType, CodeExpression data, Type dataType, CodeStatementCollection statements)
		{
			return CompilerUtil.GetTypeConversion(new DataTypePointer(targetType), data, new DataTypePointer(dataType), statements);
		}
		public DesignService()
		{
		}
		#endregion
		#region Known types
		static private bool _knowTypesAdded;
		public static void Init()
		{
			if (!_knowTypesAdded)
			{
				XmlUtil.AddKnownType("WebPage", typeof(WebPage));
				VPLUtil.DelegateLogIdeProfiling = DesignUtil.LogIdeProfile;
				_knowTypesAdded = true;
				//
				VirtualWebDir.OnValidationError = new EventHandler(MathNode.OnSetLastValidationError);
				//
				XmlUtil.CreateDesignTimeType = new fnCreateDesignTimeType(CreateClassType);
				//
				VPLUtil.GetClassTypeFromDynamicType = new fnGetClassTypeFromDynamicType(ClassPointerX.GetClassTypeFromDynamicType);
				VPLUtil.CollectLanguageIcons = ProjectResources.CollectLanguageIcons;
				VPLUtil.GetLanguageImageByName = TreeViewObjectExplorer.GetLangaugeBitmapByName;
				VPLUtil.SetServiceByName(VPLUtil.SERVICE_ComponentSelector, typeof(ComponentPointerSelector<EasyDataSet>));
				VPLUtil.PropertyValueLinkEditor = ParameterValue.GetValueSelector();
				VPLUtil.PropertyValueLinkType = typeof(PropertyValue);
				VPLUtil.delegateGetComponentID = GetComponentID;
				VPLUtil.delegateGetComponentList = GetProjectComponents;
				VPLUtil.RemoveDialogCaches = FrmObjectExplorer.RemoveDialogueCaches;
				VPLUtil.VariableMapTargetType = typeof(ParameterValue);
				//
				VariableMap.ValueTypeSelectorType = typeof(SelectorEnumValueType);
				//
				XmlSerializerUtility.OnCreateWriterFromReader = new fnCreateWriterFromReader(CreateWriterFromReader);
				//
				MathNode.Init();
				MathNode.AddPlugin(typeof(MathNodeActionInput));
				//
				XmlUtil.AddKnownType("ParameterValue", typeof(ParameterValue));
				XmlUtil.AddKnownType("ActionClass", typeof(ActionClass));
				XmlUtil.AddKnownType("SetterPointer", typeof(SetterPointer));
				XmlUtil.AddKnownType("PropertyPointer", typeof(PropertyPointer));
				XmlUtil.AddKnownType("ClassPointer", typeof(ClassPointer));
				//
				XmlUtil.AddKnownType("MethodClass", typeof(MethodClass));
				XmlUtil.AddKnownType("CustomMethodPointer", typeof(CustomMethodPointer));
				XmlUtil.AddKnownType("ConstructorClass", typeof(ConstructorClass));
				XmlUtil.AddKnownType("GetterClass", typeof(GetterClass));
				XmlUtil.AddKnownType("PropertyClass", typeof(PropertyClass));
				//
				XmlUtil.AddKnownType("SetterClass", typeof(SetterClass));
				XmlUtil.AddKnownType("CustomPropertyPointer", typeof(CustomPropertyPointer));
				XmlUtil.AddKnownType("UserControl", typeof(UserControl));
				XmlUtil.AddKnownType("ConstObjectPointer", typeof(ConstObjectPointer));
				XmlUtil.AddKnownType("DataTypePointer", typeof(DataTypePointer));
				//
				XmlUtil.AddKnownType("Form", typeof(Form));
				XmlUtil.AddKnownType("LimnorWinApp", typeof(LimnorWinApp));
				XmlUtil.AddKnownType("ComponentPointer", typeof(ComponentPointer));
				XmlUtil.AddKnownType("TypePointerCollection", typeof(TypePointerCollection));
				XmlUtil.AddKnownType("MessageBox", typeof(MessageBox));
				//
				XmlUtil.AddKnownType("WebService", typeof(WebService));
				XmlUtil.AddKnownType("WebServiceAttribute", typeof(WebServiceAttribute));
				XmlUtil.AddKnownType("WebMethodAttribute", typeof(WebMethodAttribute));
				XmlUtil.AddKnownType("WebServiceBindingAttribute", typeof(WebServiceBindingAttribute));
				//
				XmlUtil.AddKnownType("LimnorKioskApp", typeof(LimnorKioskApp));
				XmlUtil.AddKnownType("LimnorConsole", typeof(LimnorConsole));
				XmlUtil.AddKnownType("Console", typeof(Console));
				XmlUtil.AddKnownType("Object", typeof(Object));
				XmlUtil.AddKnownType("MethodInfoPointer", typeof(MethodInfoPointer));
				//
				XmlUtil.AddKnownType("Control", typeof(Control));
				XmlUtil.AddKnownType("EventAction", typeof(EventAction));
				XmlUtil.AddKnownType("EventPointer", typeof(EventPointer));
				XmlUtil.AddKnownType("TaskID", typeof(TaskID));
				XmlUtil.AddKnownType("Button", typeof(Button));
				//
				XmlUtil.AddKnownType("ActionMethodReturn", typeof(ActionMethodReturn));
				XmlUtil.AddKnownType("MathNodeRoot", typeof(MathNodeRoot));
				XmlUtil.AddKnownType("EnumIconType", typeof(EnumIconType));
				XmlUtil.AddKnownType("Size", typeof(Size));
				XmlUtil.AddKnownType("Font", typeof(Font));
				//
				XmlUtil.AddKnownType("Color", typeof(Color));
				XmlUtil.AddKnownType("Point", typeof(Point));
				XmlUtil.AddKnownType("MathNodeVariable", typeof(MathNodeVariable));
				XmlUtil.AddKnownType("RaisDataType", typeof(RaisDataType));
				XmlUtil.AddKnownType("MathNodePropertyField", typeof(MathNodePropertyField));
				//
				XmlUtil.AddKnownType("ActionAssignment", typeof(ActionAssignment));
				XmlUtil.AddKnownType("MathNodePropertySetValue", typeof(MathNodePropertySetValue));
				XmlUtil.AddKnownType("TypePointer", typeof(TypePointer));
				XmlUtil.AddKnownType("AB_SingleAction", typeof(AB_SingleAction));
				XmlUtil.AddKnownType("ActionPortOut", typeof(ActionPortOut));
				//
				XmlUtil.AddKnownType("enumPositionType", typeof(enumPositionType));
				XmlUtil.AddKnownType("DrawingVariable", typeof(DrawingVariable));
				XmlUtil.AddKnownType("ActionPortIn", typeof(ActionPortIn));
				XmlUtil.AddKnownType("PropertyReturnAction", typeof(PropertyReturnAction));
				XmlUtil.AddKnownType("ParameterClass", typeof(ParameterClass));
				//
				XmlUtil.AddKnownType("MemberComponentId", typeof(MemberComponentId));
				XmlUtil.AddKnownType("MathNodeIntegral", typeof(MathNodeIntegral));
				XmlUtil.AddKnownType("MathExpItem", typeof(MathExpItem));
				XmlUtil.AddKnownType("MathExpGroup", typeof(MathExpGroup));
				XmlUtil.AddKnownType("EnumIncludeReturnPorts", typeof(EnumIncludeReturnPorts));
				//
				XmlUtil.AddKnownType("MathNodeArgument", typeof(MathNodeArgument));
				XmlUtil.AddKnownType("MathNodeAssign", typeof(MathNodeAssign));
				XmlUtil.AddKnownType("MathNodeCondition", typeof(MathNodeCondition));
				XmlUtil.AddKnownType("MathNodeConditions", typeof(MathNodeConditions));
				XmlUtil.AddKnownType("MathNodeDefaultValue", typeof(MathNodeDefaultValue));
				//
				XmlUtil.AddKnownType("MathNodeFunction", typeof(MathNodeFunction));
				XmlUtil.AddKnownType("MathNodeInc", typeof(MathNodeInc));
				XmlUtil.AddKnownType("IntegerVariable", typeof(IntegerVariable));
				XmlUtil.AddKnownType("Modulus", typeof(Modulus));
				XmlUtil.AddKnownType("MathNodeBitAnd", typeof(MathNodeBitAnd));
				//
				XmlUtil.AddKnownType("MathNodeBitOr", typeof(MathNodeBitOr));
				XmlUtil.AddKnownType("LogicVariable", typeof(LogicVariable));
				XmlUtil.AddKnownType("LogicFalse", typeof(LogicFalse));
				XmlUtil.AddKnownType("LogicTrue", typeof(LogicTrue));
				XmlUtil.AddKnownType("LogicNot", typeof(LogicNot));
				//
				XmlUtil.AddKnownType("LogicAnd", typeof(LogicAnd));
				XmlUtil.AddKnownType("LogicOr", typeof(LogicOr));
				XmlUtil.AddKnownType("LogicGreaterThan", typeof(LogicGreaterThan));
				XmlUtil.AddKnownType("LogicGreaterThanOrEqual", typeof(LogicGreaterThanOrEqual));
				XmlUtil.AddKnownType("LogicValueEquality", typeof(LogicValueEquality));
				//
				XmlUtil.AddKnownType("LogicValueInEquality", typeof(LogicValueInEquality));
				XmlUtil.AddKnownType("LogicLessThan", typeof(LogicLessThan));
				XmlUtil.AddKnownType("LogicLessThanOrEqual", typeof(LogicLessThanOrEqual));
				XmlUtil.AddKnownType("MathNodeMethodInvoke", typeof(MathNodeMethodInvoke));
				XmlUtil.AddKnownType("MathNodeParameter", typeof(MathNodeParameter));
				//
				XmlUtil.AddKnownType("MathNodeStringContains", typeof(MathNodeStringContains));
				XmlUtil.AddKnownType("MathNodeStringBegins", typeof(MathNodeStringBegins));
				XmlUtil.AddKnownType("MathNodeStringEnds", typeof(MathNodeStringEnds));
				XmlUtil.AddKnownType("MathNodeStringGT", typeof(MathNodeStringGT));
				XmlUtil.AddKnownType("MathNodeStringGET", typeof(MathNodeStringGET));
				//
				XmlUtil.AddKnownType("MathNodeStringLT", typeof(MathNodeStringLT));
				XmlUtil.AddKnownType("MathNodeStringLET", typeof(MathNodeStringLET));
				XmlUtil.AddKnownType("MathNodeStringEQ", typeof(MathNodeStringEQ));
				XmlUtil.AddKnownType("MathNodeStringValue", typeof(MathNodeStringValue));
				XmlUtil.AddKnownType("StringVariable", typeof(StringVariable));
				//
				XmlUtil.AddKnownType("MathNodeStringAdd", typeof(MathNodeStringAdd));
				XmlUtil.AddKnownType("MathNodeSum", typeof(MathNodeSum));
				XmlUtil.AddKnownType("MathNodeValue", typeof(MathNodeValue));
				XmlUtil.AddKnownType("MathNodeNumber", typeof(MathNodeNumber));
				XmlUtil.AddKnownType("MathNodeVariable", typeof(MathNodeVariable));
				//
				XmlUtil.AddKnownType("MathNodeVariableDummy", typeof(MathNodeVariableDummy));
				XmlUtil.AddKnownType("MathNodeSqrt", typeof(MathNodeSqrt));
				XmlUtil.AddKnownType("MathNodeAbs", typeof(MathNodeAbs));
				XmlUtil.AddKnownType("MathNodeAcos", typeof(MathNodeAcos));
				XmlUtil.AddKnownType("MathNodeAsin", typeof(MathNodeAsin));
				//
				XmlUtil.AddKnownType("MathNodeAtan", typeof(MathNodeAtan));
				XmlUtil.AddKnownType("MathNodeAtan2", typeof(MathNodeAtan2));
				XmlUtil.AddKnownType("MathNodeCeiling", typeof(MathNodeCeiling));
				XmlUtil.AddKnownType("MathNodeCos", typeof(MathNodeCos));
				XmlUtil.AddKnownType("MathNodeCos2", typeof(MathNodeCos2));
				//
				XmlUtil.AddKnownType("MathNodeCosh", typeof(MathNodeCosh));
				XmlUtil.AddKnownType("MathNodeFloor", typeof(MathNodeFloor));
				XmlUtil.AddKnownType("MathNodeIEEERemainder", typeof(MathNodeIEEERemainder));
				XmlUtil.AddKnownType("MathNodeConstE", typeof(MathNodeConstE));
				XmlUtil.AddKnownType("MathNodeConstPI", typeof(MathNodeConstPI));
				//
				XmlUtil.AddKnownType("MathNodeExp", typeof(MathNodeExp));
				XmlUtil.AddKnownType("MathNodeLog", typeof(MathNodeLog));
				XmlUtil.AddKnownType("MathNodeLog10", typeof(MathNodeLog10));
				XmlUtil.AddKnownType("MathNodeLogX", typeof(MathNodeLogX));
				XmlUtil.AddKnownType("MathNodeMax", typeof(MathNodeMax));
				//
				XmlUtil.AddKnownType("MathNodeMin", typeof(MathNodeMin));
				XmlUtil.AddKnownType("MathNodeRound", typeof(MathNodeRound));
				XmlUtil.AddKnownType("MathNodeRound2", typeof(MathNodeRound2));
				XmlUtil.AddKnownType("MathNodeSign", typeof(MathNodeSign));
				XmlUtil.AddKnownType("MathNodeSin", typeof(MathNodeSin));
				//
				XmlUtil.AddKnownType("MathNodeSinh", typeof(MathNodeSinh));
				XmlUtil.AddKnownType("MathNodeTan", typeof(MathNodeTan));
				XmlUtil.AddKnownType("MathNodeTanh", typeof(MathNodeTanh));
				XmlUtil.AddKnownType("MathNodeTruncate", typeof(MathNodeTruncate));
				XmlUtil.AddKnownType("MathNodePower", typeof(MathNodePower));
				//
				XmlUtil.AddKnownType("LinkLineNodeInPort", typeof(LinkLineNodeInPort));
				XmlUtil.AddKnownType("EventHandlerMethod", typeof(EventHandlerMethod));
				XmlUtil.AddKnownType("ComponentIconPublic", typeof(ComponentIconPublic));
				XmlUtil.AddKnownType("ComponentIconLocal", typeof(ComponentIconLocal));
				XmlUtil.AddKnownType("ComponentIconMethodReturnPointer", typeof(ComponentIconMethodReturnPointer));
				XmlUtil.AddKnownType("HandlerMathodID", typeof(HandlerMethodID));
				//
				XmlUtil.AddKnownType("AB_ActionString", typeof(AB_ActionString));
				XmlUtil.AddKnownType("LinkLineNode", typeof(LinkLineNode));
				XmlUtil.AddKnownType("PlusNode", typeof(PlusNode));
				XmlUtil.AddKnownType("DivNode", typeof(DivNode));
				XmlUtil.AddKnownType("MinusNode", typeof(MinusNode));
				//
				XmlUtil.AddKnownType("MultiplyNode", typeof(MultiplyNode));
				XmlUtil.AddKnownType("MultiplyNodeBig", typeof(MultiplyNodeBig));
				XmlUtil.AddKnownType("ArrayVariable", typeof(ArrayVariable));
				XmlUtil.AddKnownType("ComponentIconArrayPointer", typeof(ComponentIconArrayPointer));
				XmlUtil.AddKnownType("ArrayPointer", typeof(ArrayPointer));
				//
				XmlUtil.AddKnownType("ConstructorPointer", typeof(ConstructorPointer));
				XmlUtil.AddKnownType("LocalVariable", typeof(LocalVariable));
				XmlUtil.AddKnownType("CustomMethodReturnPointer", typeof(CustomMethodReturnPointer));
				XmlUtil.AddKnownType("AB_SubMethodAction", typeof(AB_SubMethodAction));
				XmlUtil.AddKnownType("ActionSubMethod", typeof(ActionSubMethod));
				XmlUtil.AddKnownType("SubMethodInfoPointer", typeof(SubMethodInfoPointer));
				//
				XmlUtil.AddKnownType("ComponentIconParameter", typeof(ComponentIconParameter));
				XmlUtil.AddKnownType("ParameterClassArrayIndex", typeof(ParameterClassArrayIndex));
				XmlUtil.AddKnownType("ParameterClassArrayItem", typeof(ParameterClassArrayItem));
				XmlUtil.AddKnownType("AB_ConditionBranch", typeof(AB_ConditionBranch));
				XmlUtil.AddKnownType("Label", typeof(System.Windows.Forms.Label));
				//
				XmlUtil.AddKnownType("PropertyValueClass", typeof(PropertyValueClass));
				XmlUtil.AddKnownType("MethodReturnMethod", typeof(MethodReturnMethod));
				XmlUtil.AddKnownType("AB_MethodReturn", typeof(AB_MethodReturn));
				XmlUtil.AddKnownType("MathNodeActionInput", typeof(MathNodeActionInput));
				XmlUtil.AddKnownType("InterfaceClass", typeof(InterfaceClass));
				//
				XmlUtil.AddKnownType("InterfaceElementMethod", typeof(InterfaceElementMethod));
				XmlUtil.AddKnownType("InterfacePointer", typeof(InterfacePointer));
				XmlUtil.AddKnownType("InterfaceElementEvent", typeof(InterfaceElementEvent));
				XmlUtil.AddKnownType("InterfaceElementProperty", typeof(InterfaceElementProperty));
				XmlUtil.AddKnownType("NamedDataType", typeof(NamedDataType));
				//
				XmlUtil.AddKnownType("InterfaceElementMethodParameter", typeof(InterfaceElementMethodParameter));
				XmlUtil.AddKnownType("PropertyOverride", typeof(PropertyOverride));
				XmlUtil.AddKnownType("CustomPropertyOverridePointer", typeof(CustomPropertyOverridePointer));
				XmlUtil.AddKnownType("ParameterClassBaseProperty", typeof(ParameterClassBaseProperty));
				XmlUtil.AddKnownType("BaseMethod", typeof(BaseMethod));
				//
				XmlUtil.AddKnownType("MethodOverride", typeof(MethodOverride));
				XmlUtil.AddKnownType("EventClass", typeof(EventClass));
				XmlUtil.AddKnownType("ActionInput", typeof(ActionInput));
				XmlUtil.AddKnownType("CustomEventPointer", typeof(CustomEventPointer));
				XmlUtil.AddKnownType("CustomMethodParameterPointer", typeof(CustomMethodParameterPointer));
				//
				XmlUtil.AddKnownType("InterfaceMethodPointer", typeof(InterfaceMethodPointer));
				XmlUtil.AddKnownType("InterfacePropertyPointer", typeof(InterfacePropertyPointer));
				XmlUtil.AddKnownType("InterfaceMethodPointer", typeof(InterfaceMethodPointer));
				XmlUtil.AddKnownType("InterfaceEventPointer", typeof(InterfaceEventPointer));
				XmlUtil.AddKnownType("InterfaceCustomProperty", typeof(InterfaceCustomProperty));
				//
				XmlUtil.AddKnownType("AB_DecisionTableActions", typeof(AB_DecisionTableActions));
				XmlUtil.AddKnownType("DecisionTable", typeof(DecisionTable));
				XmlUtil.AddKnownType("MathNodeRandom", typeof(MathNodeRandom));
				XmlUtil.AddKnownType("AB_ActionList", typeof(AB_ActionList));
				XmlUtil.AddKnownType("ActionItem", typeof(ActionItem));
				//
				XmlUtil.AddKnownType("ClassInstancePointer", typeof(ClassInstancePointer));
				XmlUtil.AddKnownType("AB_LoopActions", typeof(AB_LoopActions));
				XmlUtil.AddKnownType("ActionAssignInstance", typeof(ActionAssignInstance));
				XmlUtil.AddKnownType("AB_ForLoop", typeof(AB_ForLoop));
				XmlUtil.AddKnownType("ActionBranchParameter", typeof(ActionBranchParameter));
				//
				XmlUtil.AddKnownType("ComponentIconActionBranchParameter", typeof(ComponentIconActionBranchParameter));
				XmlUtil.AddKnownType("ActionBranchParameterPointer", typeof(ActionBranchParameterPointer));
				XmlUtil.AddKnownType("EnumImageFormat", typeof(EnumImageFormat));
				XmlUtil.AddKnownType("AB_Constructor", typeof(AB_Constructor));
				XmlUtil.AddKnownType("ExpressionValue", typeof(ExpressionValue));
				//
				XmlUtil.AddKnownType("CustomEventHandlerType", typeof(CustomEventHandlerType));
				XmlUtil.AddKnownType("MemberComponentIdCustom", typeof(MemberComponentIdCustom));
				XmlUtil.AddKnownType("Environment", typeof(Environment));
				XmlUtil.AddKnownType("String[]", typeof(String[]));
				XmlUtil.AddKnownType("StringCollectionVariable", typeof(StringCollectionVariable));
				//
				XmlUtil.AddKnownType("StringCollectionPointer", typeof(StringCollectionPointer));
				XmlUtil.AddKnownType("FireEventMethod", typeof(FireEventMethod));
				XmlUtil.AddKnownType("FieldPointer", typeof(FieldPointer));
				XmlUtil.AddKnownType("NullObjectPointer", typeof(NullObjectPointer));
				XmlUtil.AddKnownType("BreakActionMethod", typeof(BreakActionMethod));
				//
				XmlUtil.AddKnownType("AB_Break", typeof(AB_Break));
				XmlUtil.AddKnownType("AttributeConstructor", typeof(AttributeConstructor));
				XmlUtil.AddKnownType("IXDesignerViewer", typeof(IXDesignerViewer));
				XmlUtil.AddKnownType("EnumMaxButtonLocation", typeof(EnumMaxButtonLocation));
				XmlUtil.AddKnownType("ICustomEventMethodDescriptor", typeof(ICustomEventMethodDescriptor));
				//
				XmlUtil.AddKnownType("MultiPanes", typeof(MultiPanes));
				XmlUtil.AddKnownType("IAction", typeof(IAction));
				XmlUtil.AddKnownType("INonHostedObject", typeof(INonHostedObject));
				XmlUtil.AddKnownType("LimnorService", typeof(LimnorService));
				XmlUtil.AddKnownType("TextBox", typeof(TextBox));
				//
				XmlUtil.AddKnownType("GroupBox", typeof(GroupBox));
				XmlUtil.AddKnownType("EventPortOutExecuteMethod", typeof(EventPortOutExecuteMethod));
				XmlUtil.AddKnownType("EventPortOutExecuter", typeof(EventPortOutExecuter));
				XmlUtil.AddKnownType("ComponentIconMethod", typeof(ComponentIconMethod));
				XmlUtil.AddKnownType("EventPathData", typeof(EventPathData));
				//
				XmlUtil.AddKnownType("ComponentIconEvent", typeof(ComponentIconEvent));
				XmlUtil.AddKnownType("ComponentIconEventhandle", typeof(ComponentIconEventhandle));
				XmlUtil.AddKnownType("ActionAssignComponent", typeof(ActionAssignComponent));
				XmlUtil.AddKnownType("ComponentIconCollectionPointer", typeof(ComponentIconCollectionPointer));
				XmlUtil.AddKnownType("CollectionTypePointer", typeof(CollectionTypePointer));
				//
				XmlUtil.AddKnownType("CollectionVariable", typeof(CollectionVariable));
				XmlUtil.AddKnownType("ParameterClassCollectionItem", typeof(ParameterClassCollectionItem));
				XmlUtil.AddKnownType("CustomConstructorPointer", typeof(CustomConstructorPointer));
				XmlUtil.AddKnownType("ComponentIconProperty", typeof(ComponentIconProperty));
				XmlUtil.AddKnownType("EventPortOutSetProperty", typeof(EventPortOutSetProperty));
				//
				XmlUtil.AddKnownType("ComponentIconFireEvent", typeof(ComponentIconFireEvent));
				XmlUtil.AddKnownType("EventPortOutFirer", typeof(EventPortOutFirer));
				XmlUtil.AddKnownType("EventPortInFireEvent", typeof(EventPortInFireEvent));
				XmlUtil.AddKnownType("EventPortIn", typeof(EventPortIn));
				XmlUtil.AddKnownType("EventHandler", typeof(EventHandler));
				//
				XmlUtil.AddKnownType("EventArgs", typeof(EventArgs));
				XmlUtil.AddKnownType("MemberComponentIdInstance", typeof(MemberComponentIdInstance));
				XmlUtil.AddKnownType("MemberComponentIdCustomInstance", typeof(MemberComponentIdCustomInstance));
				XmlUtil.AddKnownType("LimnorScreenSaverApp", typeof(LimnorScreenSaverApp));
				XmlUtil.AddKnownType("CollectionPointer", typeof(CollectionPointer));
				//
				XmlUtil.AddKnownType("AB_CastAs", typeof(AB_CastAs));
				XmlUtil.AddKnownType("Component", typeof(Component));
				XmlUtil.AddKnownType("ComponentIconClass", typeof(ComponentIconClass));
				XmlUtil.AddKnownType("ComponentIconClassType", typeof(ComponentIconClassType));
				XmlUtil.AddKnownType("EventPortOutTypeAction", typeof(EventPortOutTypeAction));
				//
				XmlUtil.AddKnownType("EventPortOutClassTypeAction", typeof(EventPortOutClassTypeAction));
				XmlUtil.AddKnownType("MemberComponentIdDefaultInstance", typeof(MemberComponentIdDefaultInstance));
				XmlUtil.AddKnownType("ExceptionHandler", typeof(ExceptionHandler));
				XmlUtil.AddKnownType("ExceptionHandlerList", typeof(ExceptionHandlerList));
				XmlUtil.AddKnownType("SelectExceptionToHandle", typeof(SelectExceptionToHandle));
				//
				XmlUtil.AddKnownType("ComponentIconLocalSubScope", typeof(ComponentIconException));
				XmlUtil.AddKnownType("SubscopeActions", typeof(SubscopeActions));
				XmlUtil.AddKnownType("ComponentIconSubscopeVariable", typeof(ComponentIconSubscopeVariable));
				XmlUtil.AddKnownType("ComponentID", typeof(ComponentID));
				XmlUtil.AddKnownType("ListVariable", typeof(ListVariable));
				//
				XmlUtil.AddKnownType("ComponentIconListPointer", typeof(ComponentIconListPointer));
				XmlUtil.AddKnownType("ListTypePointer", typeof(ListTypePointer));
				XmlUtil.AddKnownType("RuntimeInstance", typeof(RuntimeInstance));
				XmlUtil.AddKnownType("ActionSubMethodGlobal", typeof(ActionSubMethodGlobal));
				XmlUtil.AddKnownType("SubMethodInfoPointerGlobal", typeof(SubMethodInfoPointerGlobal));
				//
				XmlUtil.AddKnownType("MethodActionForeachAtServer", typeof(MethodActionForeachAtServer));
				XmlUtil.AddKnownType("MethodActionForeachAtClient", typeof(MethodActionForeachAtClient));
				XmlUtil.AddKnownType("MethodDataTransfer", typeof(MethodDataTransfer));
				XmlUtil.AddKnownType("StringMapList", typeof(StringMapList));
				XmlUtil.AddKnownType("StringMap", typeof(StringMap));
				//
				XmlUtil.AddKnownType("ParameterValueArrayItem", typeof(ParameterValueArrayItem));
				XmlUtil.AddKnownType("NameTypePair", typeof(NameTypePair));
				XmlUtil.AddKnownType("InlineAction", typeof(InlineAction));
				XmlUtil.AddKnownType("AB_Group", typeof(AB_Group));
				XmlUtil.AddKnownType("FileDownloadEventArgs", typeof(FileDownloadEventArgs));
				//
				XmlUtil.AddKnownType("BrowserNavigationEventArgs", typeof(BrowserNavigationEventArgs));
				XmlUtil.AddKnownType("ConstValueSelector", typeof(ConstValueSelector));
				XmlUtil.AddKnownType("EnumBorderStyle", typeof(EnumBorderStyle));
				XmlUtil.AddKnownType("EnumBorderWidthStyle", typeof(EnumBorderWidthStyle));
				//
				XmlUtil.AddKnownType("WebClientEventHandlerMethodDownloadActions", typeof(WebClientEventHandlerMethodDownloadActions));
				XmlUtil.AddKnownType("WebMouseEventArgs", typeof(WebMouseEventArgs));
				XmlUtil.AddKnownType("WebKeyEventArgs", typeof(WebKeyEventArgs));
				XmlUtil.AddKnownType("FileDownloadEventHandler", typeof(FileDownloadEventHandler));
				//
				XmlUtil.AddKnownType("HtmlElementPointer", typeof(HtmlElementPointer));
				XmlUtil.AddKnownType("ComponentIconHtmlElement", typeof(ComponentIconHtmlElement));
				XmlUtil.AddKnownType("ComponentIconHtmlElementCurrent", typeof(ComponentIconHtmlElementCurrent));
				XmlUtil.AddKnownType("WebClientEventHandlerMethodClientActions", typeof(WebClientEventHandlerMethodClientActions));
				XmlUtil.AddKnownType("WebClientEventHandlerMethodServerActions", typeof(WebClientEventHandlerMethodServerActions));
				//
				XmlUtil.AddKnownType("IWebClientControl", typeof(IWebClientControl));
				XmlUtil.AddKnownType("WebMouseButton", typeof(WebMouseButton));
				XmlUtil.AddKnownType("IWebClientComponent", typeof(IWebClientComponent));
				XmlUtil.AddKnownType("CollectionComponents", typeof(CollectionComponentNames));
				//
				XmlUtil.AddKnownType("LogonUser", typeof(LogonUser));
				XmlUtil.AddKnownType("CustomInterfaceMethodPointer", typeof(CustomInterfaceMethodPointer));
				//
				Type[] ts = typeof(HtmlElement_body).Assembly.GetExportedTypes();
				for (int i = 0; i < ts.Length; i++)
				{
					if (typeof(HtmlElement_Base).IsAssignableFrom(ts[i]))
					{
						XmlUtil.AddKnownType(ts[i].Name, ts[i]);
					}
				}
				XmlUtil.AddKnownType("WebClientValueCollection", typeof(WebClientValueCollection));
				//
				XmlUtil.AddKnownType("OLECMDF", typeof(OLECMDF));
				XmlUtil.AddKnownType("OLECMDID", typeof(OLECMDID));
				XmlUtil.AddKnownType("OLECMDEXECOPT", typeof(OLECMDEXECOPT));
				XmlUtil.AddKnownType("EnumPopupLevel", typeof(EnumPopupLevel));
				XmlUtil.AddKnownType("IWebBrowser2", typeof(IWebBrowser2));
				//
				XmlUtil.AddKnownType("ConnectionItem", typeof(ConnectionItem));
				XmlUtil.AddKnownType("PluginManager`1", typeof(PluginManager<>));
				XmlUtil.AddKnownType("MethodAssignActions", typeof(MethodAssignActions));
				XmlUtil.AddKnownType("AB_AssignActions", typeof(AB_AssignActions));
				XmlUtil.AddKnownType("ActionAttachEvent", typeof(ActionAttachEvent));
				XmlUtil.AddKnownType("ActionDetachEvent", typeof(ActionDetachEvent));
				//
				XmlUtil.AddKnownType("EventHandlerDataChanged", typeof(EventHandlerDataChanged));
				XmlUtil.AddKnownType("EventArgsDataName", typeof(EventArgsDataName));
				XmlUtil.AddKnownType("IPluginManager", typeof(IPluginManager));
				XmlUtil.AddKnownType("IPlugin", typeof(IPlugin));
				//
				XmlUtil.AddKnownType("ILicenseRequestHandler", typeof(ILicenseRequestHandler));
				XmlUtil.AddKnownType("EventArgsRegister", typeof(EventArgsRegister));
				XmlUtil.AddKnownType("CopyProtector", typeof(CopyProtector));
				XmlUtil.AddKnownType("EnumHideDialogButtons", typeof(EnumHideDialogButtons));
				XmlUtil.AddKnownType("VplMethodPointer", typeof(VplMethodPointer));
			}
		}
		#endregion
		#region static methods
		public static bool IsUsingHtmlEditor()
		{
#if USEHTMLEDITOR
			return true;
#else
			return false;
#endif
		}
		public static IXmlCodeWriter CreateWriterFromReader(IXmlCodeReader reader)
		{
			XmlObjectReader r = reader as XmlObjectReader;
			if (r != null && r.ObjectList != null)
			{
				return new XmlObjectWriter(r.ObjectList);
			}
			return XmlSerializerUtility.ActiveWriter;
		}
		public static fnGetMessage GetJsonMin;
		public static fnGetMessage GetModalMin;
		public static IComponentID GetComponentID(IComponent caller, UInt32 cid)
		{
			if (LimnorProject.ActiveProject != null)
			{
				return LimnorProject.ActiveProject.GetComponentByID(cid);
			}
			return null;
		}
		public static IList<IComponentID> GetProjectComponents(IComponent caller)
		{
			if (LimnorProject.ActiveProject != null)
			{
				IList<ComponentID> lst = LimnorProject.ActiveProject.GetAllComponents();
				if (lst != null)
				{
					List<IComponentID> ret = new List<IComponentID>();
					foreach (ComponentID c in lst)
					{
						ret.Add(c);
					}
					return ret;
				}
			}
			return null;
		}
		public static ObjectIDmap GetObjectMap(LimnorProject proj, UInt32 classId)
		{
			return proj.GetTypedData<ObjectIDmap>(classId);
		}

		public static Type CreateClassType(UInt32 classId, Guid projectGuid)
		{
			Type t = VPLUtil.GetClassType(classId, projectGuid);
			if (t == null)
			{
				ClassPointer root = null;
				LimnorProject prj = LimnorSolution.GetLimnorProjectByGuid(projectGuid);
				if (prj != null)
				{
					root = ClassPointer.CreateClassPointer(classId, prj);
				}
				string typename = null;
				if (root != null)
				{
					typename = root.Name;
				}
				if (string.IsNullOrEmpty(typename))
				{
					typename = string.Format(CultureInfo.InvariantCulture,
						"Class_{0}_{1}", classId, projectGuid.ToString("N", CultureInfo.InvariantCulture));
				}
				string name = string.Format(CultureInfo.InvariantCulture,
					"{0}{1}", VPLUtil.ASSEMBLYNAME_PRE, classId.ToString("x", CultureInfo.InvariantCulture));
				AssemblyName aName = new AssemblyName(name);
				AssemblyBuilder ab =
					AppDomain.CurrentDomain.DefineDynamicAssembly(
						aName,
						AssemblyBuilderAccess.RunAndSave);

				// For a single-module assembly, the module name is usually
				// the assembly name plus an extension.
				ModuleBuilder mb =
					ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");
				TypeBuilder tb = mb.DefineType(
				typename,
				 TypeAttributes.Public, typeof(ClassPointerX));
				ConstructorInfo cif = typeof(ClassIdAttribute).GetConstructor(new Type[] { typeof(UInt32), typeof(string) });
				CustomAttributeBuilder cab = new CustomAttributeBuilder(cif, new object[] { (UInt32)classId, projectGuid.ToString("N", CultureInfo.InvariantCulture) });
				tb.SetCustomAttribute(cab);
				t = tb.CreateType();
				VPLUtil.SetClassType(classId, projectGuid, t);
			}
			return t;
		}
		#endregion
		#region IDesignService Members

		public IClassPointer CreateClassPointer(ObjectIDmap map)
		{
			ClassPointer cp = ClassPointer.CreateClassPointer(map);
			return cp;
		}
		public IClassPointer GetClassPointerFromCache(ObjectIDmap map)
		{
			return map.GetTypedData<ClassPointer>();
		}
		public IClassPointer GetClassPointerFromCache(LimnorProject proj, UInt32 classId)
		{
			ClassPointer cp = proj.GetTypedData<ClassPointer>(classId);
			if (cp == null)
			{
				cp = ClassPointer.CreateClassPointer(classId, proj);
			}
			return cp;
		}
		#endregion
	}
}
