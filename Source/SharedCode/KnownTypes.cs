/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */
using XmlUtility;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using LimnorDatabase;
using Limnor.Reporting;
using Limnor.Drawing2D;
using System.CodeDom;
using LimnorDesigner.ResourcesManager;
using Limnor.InputDevice;
using Limnor.Application;
using MathComponent;
using System.ServiceProcess;
using System;
using Limnor.Net;
using FormComponents;
using ProgUtility;
using Limnor.DirectXCapturer;
using Limnor.CopyProtection;
using OskUtility;
using Limnor.TreeViewExt;
using LimnorWebBrowser;
using LimnorForms;
using LimnorDesigner;
using System.IO;
using Limnor.WebBuilder;
using Limnor.PhpComponents;
using MathItem;
using Limnor.Windows;
using WindowsUtility;
using Limnor.Quartz;
using Limnor.Remoting.Host;
using VPL;
using LimnorDesigner.Property;
using System.Collections.Generic;
using LimnorDatabase.DataTransfer;
static class InitKnownTypes
{
	public static void Init()
	{
		addDatabaseTypes();
		addDrawingTypes();
		addOtherTypes();
		addWebBuilderTypes();
		MathNodePointer.InitType();
		loadTypes();
	}
	static void loadTypes()
	{
		string sDir = Path.GetDirectoryName(typeof(InitKnownTypes).Assembly.Location);
		string toolboxCfg = Path.Combine(sDir, "LimnorToolbox.xml");
		XmlUtil.AddKnownTypes(toolboxCfg);
		Type[] lst = typeof(WebMessageBox).Assembly.GetExportedTypes();
		for (int i = 0; i < lst.Length; i++)
		{
			if (lst[i].GetInterface("IWebClientControl") != null)
			{
				XmlUtil.AddKnownType(lst[i].Name, lst[i]);
			}
			else
			{
				object[] vs = lst[i].GetCustomAttributes(typeof(WebClientClassAttribute), true);
				if (vs != null && vs.Length > 0)
				{
					XmlUtil.AddKnownType(lst[i].Name, lst[i]);
				}
				else
				{
					if (lst[i].GetInterface("IWebServerComponentCreator") != null)
					{
						XmlUtil.AddKnownType(lst[i].Name, lst[i]);
					}
				}
			}
		}
	}
	private static void addDatabaseTypes()
	{
		DataEditor.DelegateGetDatabaseLookupDataDialog = DataEditorLookupDB.GetDatabaseLookupDataDialog;
		DataEditor.DelegateGetCheckedListDialog = DlgDataEditorCheckedListbox.GetCheckedListDataDialog;
		XmlUtil.AddKnownType("OleDbType", typeof(OleDbType));
		XmlUtil.AddKnownType("DataTable", typeof(DataTable));
		XmlUtil.AddKnownType("DataColumn", typeof(DataColumn));
		XmlUtil.AddKnownType("Binding", typeof(Binding));

		XmlUtil.AddKnownType("SQLNoneQuery", typeof(SQLNoneQuery));
		XmlUtil.AddKnownType("EasyGrid", typeof(EasyGrid));
		XmlUtil.AddKnownType("EasyDetailsGrid", typeof(EasyGridDetail));
		XmlUtil.AddKnownType("EasyDataSet", typeof(EasyDataSet));
		XmlUtil.AddKnownType("EasyQuery", typeof(EasyQuery));
		XmlUtil.AddKnownType("Connection", typeof(Connection));
		XmlUtil.AddKnownType("EPField", typeof(EPField));
		XmlUtil.AddKnownType("DataEditorFile", typeof(DataEditorFile));
		XmlUtil.AddKnownType("DataEditorDatetime", typeof(DataEditorDatetime));
		XmlUtil.AddKnownType("FieldList", typeof(FieldList));
		XmlUtil.AddKnownType("ParameterList", typeof(ParameterList));
		XmlUtil.AddKnownType("EasyUpdator", typeof(EasyUpdator));
		XmlUtil.AddKnownType("DatabaseExecuter", typeof(DatabaseExecuter));
		XmlUtil.AddKnownType("EasyTransactor", typeof(EasyTransactor));
		XmlUtil.AddKnownType("EasyTransfer", typeof(EasyTransfer));
		XmlUtil.AddKnownType("WebDataRow", typeof(WebDataRow));
		XmlUtil.AddKnownType("DataGridViewEx", typeof(DataGridViewEx));
		//
		XmlUtil.AddKnownType("ColumnAttributes", typeof(ColumnAttributes));
		XmlUtil.AddKnownType("ColorCondition", typeof(ColorCondition));
		XmlUtil.AddKnownType("DrawTextAttrs", typeof(DrawTextAttrs));
		XmlUtil.AddKnownType("WebLoginManager", typeof(WebLoginManager));
		XmlUtil.AddKnownType("FieldCollection", typeof(FieldCollection));
		//
		XmlUtil.AddKnownType("TransMethod", typeof(TransMethod));
		XmlUtil.AddKnownType("clsFTPParams", typeof(clsFTPParams));
		XmlUtil.AddKnownType("DTDataSource", typeof(DTDataSource));
		XmlUtil.AddKnownType("DTSSourceText", typeof(DTSSourceText));
		XmlUtil.AddKnownType("DTDataDestination", typeof(DTDataDestination));
		//
		XmlUtil.AddKnownType("DataQuery", typeof(DataQuery));
		XmlUtil.AddKnownType("DTDest", typeof(DTDest));
		XmlUtil.AddKnownType("DTDestTextFile", typeof(DTDestTextFile));
		XmlUtil.AddKnownType("DTSQuery", typeof(DTSQuery));
		XmlUtil.AddKnownType("PropertyValue", typeof(PropertyValue));
		XmlUtil.AddKnownType("DTDataDestination", typeof(DTDataDestination));
		//
		XmlUtil.AddKnownType("DbParameterListExt", typeof(DbParameterListExt));
	}
	private static void addDrawingTypes()
	{
		XmlUtil.AddKnownType("ScreenSaverBackgroundForm", typeof(ScreenSaverBackgroundForm));
		XmlUtil.AddKnownType("DrawingPage", typeof(DrawingPage));
		XmlUtil.AddKnownType("DrawingItem", typeof(DrawingItem));
		XmlUtil.AddKnownType("DrawingLayerHeader", typeof(DrawingLayerHeader));
		XmlUtil.AddKnownType("DrawingLayerCollection", typeof(DrawingLayerCollection));
		XmlUtil.AddKnownType("CPoint", typeof(CPoint));
		XmlUtil.AddKnownType("DrawingControl", typeof(DrawingControl));
		XmlUtil.AddKnownType("DrawingGroupBox", typeof(DrawingGroupBox));
		XmlUtil.AddKnownType("DrawingLayer", typeof(DrawingLayer));
		XmlUtil.AddKnownType("DrawCoordinates", typeof(DrawCoordinates));
		XmlUtil.AddKnownType("PageAttrs", typeof(PageAttrs));

		XmlUtil.AddKnownType("DrawLine", typeof(DrawLine));
		XmlUtil.AddKnownType("Draw2DLine", typeof(Draw2DLine));

		XmlUtil.AddKnownType("DrawLineArrow", typeof(DrawLineArrow));
		XmlUtil.AddKnownType("Draw2DLineArrow", typeof(Draw2DLineArrow));

		XmlUtil.AddKnownType("DrawLineArrow2", typeof(DrawLineArrow2));
		XmlUtil.AddKnownType("Draw2DLineArrow2", typeof(Draw2DLineArrow2));

		XmlUtil.AddKnownType("DrawCircle", typeof(DrawCircle));
		XmlUtil.AddKnownType("Draw2DCircle", typeof(Draw2DCircle));

		XmlUtil.AddKnownType("DrawText", typeof(DrawText));
		XmlUtil.AddKnownType("Draw2DText", typeof(Draw2DText));

		XmlUtil.AddKnownType("DrawArc", typeof(DrawArc));
		XmlUtil.AddKnownType("Draw2DArc", typeof(Draw2DArc));

		XmlUtil.AddKnownType("DrawEllips", typeof(DrawEllipse));
		XmlUtil.AddKnownType("Draw2DEllipse", typeof(Draw2DEllipse));

		XmlUtil.AddKnownType("DrawBezier", typeof(DrawBezier));
		XmlUtil.AddKnownType("Draw2DBezier", typeof(Draw2DBezier));

		XmlUtil.AddKnownType("DrawRect", typeof(DrawRect));
		XmlUtil.AddKnownType("Draw2DRect", typeof(Draw2DRect));

		XmlUtil.AddKnownType("DrawClosedCurve", typeof(DrawClosedCurve));
		XmlUtil.AddKnownType("Draw2DClosedCurve", typeof(Draw2DClosedCurve));

		XmlUtil.AddKnownType("DrawPolygon", typeof(DrawPolygon));
		XmlUtil.AddKnownType("Draw2DPolygon", typeof(Draw2DPolygon));

		XmlUtil.AddKnownType("DrawImage", typeof(DrawImage));
		XmlUtil.AddKnownType("Draw2DImage", typeof(Draw2DImage));

		XmlUtil.AddKnownType("DrawRoundRectangle", typeof(DrawRoundRectangle));
		XmlUtil.AddKnownType("Draw2DRoundRectangle", typeof(Draw2DRoundRectangle));

		XmlUtil.AddKnownType("DrawTextRect", typeof(DrawTextRect));
		XmlUtil.AddKnownType("Draw2DTextRect", typeof(Draw2DTextRect));

		XmlUtil.AddKnownType("DrawRectText", typeof(DrawRectText));
		XmlUtil.AddKnownType("Draw2DRectText", typeof(Draw2DRectText));
		//
		XmlUtil.AddKnownType("Draw2DGroupBox", typeof(Draw2DGroupBox));
		XmlUtil.AddKnownType("DrawGroupBox", typeof(DrawGroupBox));
		//
		XmlUtil.AddKnownType("Draw2DDataRepeater", typeof(Draw2DDataRepeater));
		XmlUtil.AddKnownType("DrawDataRepeater", typeof(DrawDataRepeater));
		//
		XmlUtil.AddKnownType("Draw2DTable", typeof(Draw2DTable));
		XmlUtil.AddKnownType("DrawTable", typeof(DrawTable));
	}
	private static void addOtherTypes()
	{
		XmlUtil.AddKnownType("CodeArgumentReferenceExpression", typeof(CodeArgumentReferenceExpression));
		XmlUtil.AddKnownType("CodeArrayCreateExpression", typeof(CodeArrayCreateExpression));
		XmlUtil.AddKnownType("CodeArrayIndexerExpression", typeof(CodeArrayIndexerExpression));
		XmlUtil.AddKnownType("CodeBaseReferenceExpression", typeof(CodeBaseReferenceExpression));
		XmlUtil.AddKnownType("CodeBinaryOperatorExpression", typeof(CodeBinaryOperatorExpression));
		XmlUtil.AddKnownType("CodeCastExpression", typeof(CodeCastExpression));
		XmlUtil.AddKnownType("CodeDefaultValueExpression", typeof(CodeDefaultValueExpression));
		XmlUtil.AddKnownType("CodeDelegateCreateExpression", typeof(CodeDelegateCreateExpression));
		XmlUtil.AddKnownType("CodeDelegateInvokeExpression", typeof(CodeDelegateInvokeExpression));
		XmlUtil.AddKnownType("CodeDirectionExpression", typeof(CodeDirectionExpression));
		XmlUtil.AddKnownType("CodeEventReferenceExpression", typeof(CodeEventReferenceExpression));
		XmlUtil.AddKnownType("CodeFieldReferenceExpression", typeof(CodeFieldReferenceExpression));
		XmlUtil.AddKnownType("CodeIndexerExpression", typeof(CodeIndexerExpression));
		XmlUtil.AddKnownType("CodeMethodInvokeExpression", typeof(CodeMethodInvokeExpression));
		XmlUtil.AddKnownType("CodeMethodReferenceExpression", typeof(CodeMethodReferenceExpression));
		XmlUtil.AddKnownType("CodeObjectCreateExpression", typeof(CodeObjectCreateExpression));
		XmlUtil.AddKnownType("CodeParameterDeclarationExpression", typeof(CodeParameterDeclarationExpression));
		XmlUtil.AddKnownType("CodePrimitiveExpression", typeof(CodePrimitiveExpression));
		XmlUtil.AddKnownType("CodePropertyReferenceExpression", typeof(CodePropertyReferenceExpression));
		XmlUtil.AddKnownType("CodePropertySetValueReferenceExpression", typeof(CodePropertySetValueReferenceExpression));
		XmlUtil.AddKnownType("CodeSnippetExpression", typeof(CodeSnippetExpression));
		XmlUtil.AddKnownType("CodeThisReferenceExpression", typeof(CodeThisReferenceExpression));
		XmlUtil.AddKnownType("CodeTypeOfExpression", typeof(CodeTypeOfExpression));
		XmlUtil.AddKnownType("CodeTypeReferenceExpression", typeof(CodeTypeReferenceExpression));
		XmlUtil.AddKnownType("CodeVariableReferenceExpression", typeof(CodeVariableReferenceExpression));
		//
		XmlUtil.AddKnownType("CodeAssignStatement", typeof(CodeAssignStatement));
		XmlUtil.AddKnownType("CodeAttachEventStatement", typeof(CodeAttachEventStatement));
		XmlUtil.AddKnownType("CodeCommentStatement", typeof(CodeCommentStatement));
		XmlUtil.AddKnownType("CodeConditionStatement", typeof(CodeConditionStatement));
		XmlUtil.AddKnownType("CodeExpressionStatement", typeof(CodeExpressionStatement));
		XmlUtil.AddKnownType("CodeGotoStatement", typeof(CodeGotoStatement));
		XmlUtil.AddKnownType("CodeIterationStatement", typeof(CodeIterationStatement));
		XmlUtil.AddKnownType("CodeLabeledStatement", typeof(CodeLabeledStatement));
		XmlUtil.AddKnownType("CodeMethodReturnStatement", typeof(CodeMethodReturnStatement));
		XmlUtil.AddKnownType("CodeRemoveEventStatement", typeof(CodeRemoveEventStatement));
		XmlUtil.AddKnownType("CodeSnippetStatement", typeof(CodeSnippetStatement));
		XmlUtil.AddKnownType("CodeThrowExceptionStatement", typeof(CodeThrowExceptionStatement));
		XmlUtil.AddKnownType("CodeTryCatchFinallyStatement", typeof(CodeTryCatchFinallyStatement));
		XmlUtil.AddKnownType("CodeVariableDeclarationStatement", typeof(CodeVariableDeclarationStatement));

		//
		XmlUtil.AddKnownType("ProjectResources", typeof(ProjectResources));
		XmlUtil.AddKnownType("ResourceCodePointer", typeof(ResourceCodePointer));
		XmlUtil.AddKnownType("ResourcePointerString", typeof(ResourcePointerString));
		XmlUtil.AddKnownType("ResourcePointerImage", typeof(ResourcePointerImage));
		XmlUtil.AddKnownType("ResourcePointerIcon", typeof(ResourcePointerIcon));
		XmlUtil.AddKnownType("ResourcePointerAudio", typeof(ResourcePointerAudio));
		XmlUtil.AddKnownType("ResourcePointerFile", typeof(ResourcePointerFile));
		XmlUtil.AddKnownType("ResourcePointerFilePath", typeof(ResourcePointerFilePath));
		//
		XmlUtil.AddKnownType("MousePointer", typeof(MousePointer));
		XmlUtil.AddKnownType("Keyboard", typeof(Keyboard));
		XmlUtil.AddKnownType("HotKeyList", typeof(HotKeyList));
		XmlUtil.AddKnownType("WindowsManager", typeof(WindowsManager));
		XmlUtil.AddKnownType("WindowsRegistry", typeof(WindowsRegistry));
		XmlUtil.AddKnownType("ExecuteFile", typeof(ExecuteFile));
		XmlUtil.AddKnownType("ApplicationConfiguration", typeof(ApplicationConfiguration));
		XmlUtil.AddKnownType("CategoryList", typeof(CategoryList));
		XmlUtil.AddKnownType("MathematicExpression", typeof(MathematicExpression));
		XmlUtil.AddKnownType("ServiceBase", typeof(ServiceBase));
		XmlUtil.AddKnownType("Environment", typeof(Environment));
		XmlUtil.AddKnownType("MailSender", typeof(MailSender));
		XmlUtil.AddKnownType("OperationFailEventArgs", typeof(OperationFailEventArgs));
		XmlUtil.AddKnownType("OperationFailHandler", typeof(OperationFailHandler));
		XmlUtil.AddKnownType("EnumCharEncode", typeof(EnumCharEncode));
		XmlUtil.AddKnownType("FtpClient", typeof(FtpClient));
		XmlUtil.AddKnownType("FtpTransferEventArgs", typeof(FtpTransferEventArgs));
		XmlUtil.AddKnownType("FtpFileInfo", typeof(FtpFileInfo));
		XmlUtil.AddKnownType("TextBoxNumber", typeof(TextBoxNumber));
		XmlUtil.AddKnownType("LabelNumber", typeof(LabelNumber));
		XmlUtil.AddKnownType("ButtonKey", typeof(ButtonKey));
		XmlUtil.AddKnownType("KeyPairList", typeof(KeyPairList));
		XmlUtil.AddKnownType("KeyPair", typeof(KeyPair));
		XmlUtil.AddKnownType("StringTool", typeof(StringTool));
		XmlUtil.AddKnownType("Capturer", typeof(Capturer));
		XmlUtil.AddKnownType("CopyProtector", typeof(CopyProtector));
		XmlUtil.AddKnownType("OskWindow", typeof(OskWindow));
		XmlUtil.AddKnownType("PrinterManager", typeof(PrinterManager));
		XmlUtil.AddKnownType("RS232", typeof(RS232));
		XmlUtil.AddKnownType("ScheduleTimer", typeof(ScheduleTimer));
		XmlUtil.AddKnownType("Scheduler", typeof(Scheduler));
		XmlUtil.AddKnownType("EnumScheduleType", typeof(EnumScheduleType));
		XmlUtil.AddKnownType("DialogStringCollection", typeof(DialogStringCollection));
		XmlUtil.AddKnownType("StringCollectionEditor", typeof(StringCollectionEditor));
		XmlUtil.AddKnownType("MciMediaPlayer", typeof(MciMediaPlayer));
		XmlUtil.AddKnownType("RemotingHost", typeof(RemotingHost));
		XmlUtil.AddKnownType("EventArgsSchedule", typeof(EventArgsSchedule));
		XmlUtil.AddKnownType("MonthCalendar", typeof(MonthCalendar));
		XmlUtil.AddKnownType("PropertyClassWebClient", typeof(PropertyClassWebClient));
		XmlUtil.AddKnownType("PropertyClassWebServer", typeof(PropertyClassWebServer));
		XmlUtil.AddKnownType("SessionVariableCollection", typeof(SessionVariableCollection));
		XmlUtil.AddKnownType("SessionVariable", typeof(SessionVariable));
		//activate TreeViewX.AddKnownTypes()
		///////
		XmlUtil.AddKnownType("PointFX", typeof(PointFX));
		TreeViewX.Init();
		//========
		XmlUtil.AddKnownType("WebBrowserControl", typeof(WebBrowserControl));
		XmlUtil.AddKnownType("RichTextBoxEx", typeof(RichTextBoxEx));
		//
		string sDir = Path.GetDirectoryName(typeof(DesignService).Assembly.Location);
		string toolboxCfg = Path.Combine(sDir, "LimnorToolbox.xml");
		toolboxCfg = Path.Combine(sDir, "LimnorWebToolbox.xml");
		XmlUtil.AddKnownTypes(toolboxCfg);
	}
	private static void addWebBuilderTypes()
	{
		XmlUtil.AddKnownType("LimnorWebApp", typeof(LimnorWebApp));
		XmlUtil.AddKnownType("LimnorWebAppPhp", typeof(LimnorWebAppPhp));
		XmlUtil.AddKnownType("SendMail", typeof(SendMail));
		XmlUtil.AddKnownType("ServerFile", typeof(ServerFile));
		XmlUtil.AddKnownType("PhpComponent", typeof(PhpComponent));
		//
		XmlUtil.AddKnownType("PhpString", typeof(PhpString));
		XmlUtil.AddKnownType("PhpArray", typeof(PhpArray));
		//
		XmlUtil.AddKnownType("WebDataEditor", typeof(WebDataEditor));
		XmlUtil.AddKnownType("WebDataEditorDatetime", typeof(WebDataEditorDatetime));
		XmlUtil.AddKnownType("WebDataEditorLookup", typeof(WebDataEditorLookup));
		XmlUtil.AddKnownType("WebDataEditorNone", typeof(WebDataEditorNone));
		//
		Dictionary<string, Type> types = WebClientData.GetJavascriptTypes();
		foreach (Type t in types.Values)
		{
			XmlUtil.AddKnownType(t.Name, t);
		}
	}
}