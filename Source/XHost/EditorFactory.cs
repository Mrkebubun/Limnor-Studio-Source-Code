using System;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Runtime.InteropServices;

//using Microsoft.VisualStudio;
//using Microsoft.VisualStudio.Designer.Interfaces;
//using Microsoft.VisualStudio.OLE.Interop;
//using Microsoft.VisualStudio.Shell;
//using Microsoft.VisualStudio.Shell.Interop;
//using Microsoft.VisualStudio.TextManager.Interop;

//using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using MathExp;

namespace XHost
{
    /// <summary>
    /// Factory for creating our editor object. Extends from the IVsEditoryFactory interface
    /// </summary>
    /// 
    //[ComVisible(true)]
    //[Guid(GuidList.guidVSMainEditorFactoryString)]
    public class EditorFactory// : IVsEditorFactory
    {
        //private LimnorStudioPackage editorPackage;
        //private ServiceProvider serviceProvider;
        //private WinControPME winControlPME;
        
        public EditorFactory(LimnorStudioPackage package)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering {0} constructor", this.ToString()));

            this.editorPackage = package;
            string pmePath = Path.Combine(Application.StartupPath, @"PackagesToLoad\winControlPME.XML");
            //winControlPME = new WinControPME(pmePath);
        }

        #region IVsEditorFactory Members

        public virtual int SetSite(IOleServiceProvider serviceProvider)
        {
            this.serviceProvider = new ServiceProvider(serviceProvider);
            return VSConstants.S_OK;
        }

        public virtual object GetService(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }

        // This method is called by the Environment (inside IVsUIShellOpenDocument::
        // OpenStandardEditor and OpenSpecificEditor) to map a LOGICAL view to a 
        // PHYSICAL view. A LOGICAL view identifies the purpose of the view that is
        // desired (e.g. a view appropriate for Debugging [LOGVIEWID_Debugging], or a 
        // view appropriate for text view manipulation as by navigating to a find
        // result [LOGVIEWID_TextView]). A PHYSICAL view identifies an actual type 
        // of view implementation that an IVsEditorFactory can create. 
        //
        // NOTE: Physical views are identified by a string of your choice with the 
        // one constraint that the default/primary physical view for an editor  
        // *MUST* use a NULL string as its physical view name (*pbstrPhysicalView = NULL).
        //
        // NOTE: It is essential that the implementation of MapLogicalView properly
        // validates that the LogicalView desired is actually supported by the editor.
        // If an unsupported LogicalView is requested then E_NOTIMPL must be returned.
        //
        // NOTE: The special Logical Views supported by an Editor Factory must also 
        // be registered in the local registry hive. LOGVIEWID_Primary is implicitly 
        // supported by all editor types and does not need to be registered.
        // For example, an editor that supports a ViewCode/ViewDesigner scenario
        // might register something like the following:
        //        HKLM\Software\Microsoft\VisualStudio\9.0\Editors\
        //            {...guidEditor...}\
        //                LogicalViews\
        //                    {...LOGVIEWID_TextView...} = s ''
        //                    {...LOGVIEWID_Code...} = s ''
        //                    {...LOGVIEWID_Debugging...} = s ''
        //                    {...LOGVIEWID_Designer...} = s 'Form'
        //
        //public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        //{
        //    pbstrPhysicalView = null;    // initialize out parameter

        //    // we support only a single physical view
        //    if (VSConstants.LOGVIEWID_Primary == rguidLogicalView
        //        || VSConstants.LOGVIEWID_Designer == rguidLogicalView)
        //    {
        //        pbstrPhysicalView = "Design";
        //        return VSConstants.S_OK;        // primary view uses NULL as pbstrPhysicalView
        //    }
        //    else
        //        return VSConstants.E_NOTIMPL;   // you must return E_NOTIMPL for any unrecognized rguidLogicalView values
        //}
        public virtual int MapLogicalView(ref Guid logicalView, out string physicalView)
        {
            // initialize out parameter
            physicalView = null;

            bool isSupportedView = false;
            // Determine the physical view
            if (VSConstants.LOGVIEWID_Primary == logicalView)
            {
                //physicalView = "Design";
                isSupportedView = true;
            }
            else if (VSConstants.LOGVIEWID_Designer == logicalView)
            {
                //physicalView = "Design";
                isSupportedView = true;
            }
            else if (VSConstants.LOGVIEWID_TextView == logicalView)
            {
                isSupportedView = false;
            }
            else if (VSConstants.LOGVIEWID_Code == logicalView)
            {
                isSupportedView = false;
            }

            if (isSupportedView)
                return VSConstants.S_OK;
            else
            {
                // E_NOTIMPL must be returned for any unrecognized rguidLogicalView values
                return VSConstants.E_NOTIMPL;
            }
        }

        public int Close()
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Used by the editor factory to create an editor instance. the environment first determines the 
        /// editor factory with the highest priority for opening the file and then calls 
        /// IVsEditorFactory.CreateEditorInstance. If the environment is unable to instantiate the document data 
        /// in that editor, it will find the editor with the next highest priority and attempt to so that same 
        /// thing. 
        /// NOTE: The priority of our editor is 32 as mentioned in the attributes on the package class.
        /// 
        /// Since our editor supports opening only a single view for an instance of the document data, if we 
        /// are requested to open document data that is already instantiated in another editor, or even our 
        /// editor, we return a value VS_E_INCOMPATIBLEDOCDATA.
        /// </summary>
        /// <param name="grfCreateDoc">Flags determining when to create the editor. Only open and silent flags 
        /// are valid
        /// </param>
        /// <param name="pszMkDocument">path to the file to be opened</param>
        /// <param name="pszPhysicalView">name of the physical view</param>
        /// <param name="pvHier">pointer to the IVsHierarchy interface</param>
        /// <param name="itemid">Item identifier of this editor instance</param>
        /// <param name="punkDocDataExisting">This parameter is used to determine if a document buffer 
        /// (DocData object) has already been created
        /// </param>
        /// <param name="ppunkDocView">Pointer to the IUnknown interface for the DocView object</param>
        /// <param name="ppunkDocData">Pointer to the IUnknown interface for the DocData object</param>
        /// <param name="pbstrEditorCaption">Caption mentioned by the editor for the doc window</param>
        /// <param name="pguidCmdUI">the Command UI Guid. Any UI element that is visible in the editor has 
        /// to use this GUID. This is specified in the .vsct file
        /// </param>
        /// <param name="pgrfCDW">Flags for CreateDocumentWindow</param>
        /// <returns></returns>
        public virtual int CreateEditorInstance(
                        uint createEditorFlags,
                        string documentMoniker,
                        string physicalView,
                        IVsHierarchy hierarchy,
                        uint itemid,
                        IntPtr docDataExisting,
                        out IntPtr docView,
                        out IntPtr docData,
                        out string editorCaption,
                        out Guid commandUIGuid,
                        out int createDocumentWindowFlags)
        {
            // Initialize out parameters
            docView = IntPtr.Zero;
            docData = IntPtr.Zero;
            commandUIGuid = GuidList.guidVSMainEditorFactory;
            createDocumentWindowFlags = 0;
            editorCaption = null;

            // Validate inputs
            if ((createEditorFlags & (VSConstants.CEF_OPENFILE | VSConstants.CEF_SILENT)) == 0)
                return VSConstants.E_INVALIDARG;

            // Get the text buffer
            IVsTextLines textLines = GetTextBuffer(docDataExisting, documentMoniker);

            // Assign docData IntPtr to either existing docData or the new text buffer
            if (docDataExisting != IntPtr.Zero)
            {
                docData = docDataExisting;
                Marshal.AddRef(docData);
            }
            else
            {
                docData = Marshal.GetIUnknownForObject(textLines);
            }

            try
            {
                docView = CreateDocumentView(physicalView, hierarchy, itemid, textLines, out editorCaption, out commandUIGuid, documentMoniker);
            }
            finally
            {
                if (docView == IntPtr.Zero)
                {
                    if (docDataExisting != docData && docData != IntPtr.Zero)
                    {
                        // Cleanup the instance of the docData that we have addref'ed
                        Marshal.Release(docData);

                        docData = IntPtr.Zero;
                    }
                }
            }

            return VSConstants.S_OK;
        }

        private IVsTextLines GetTextBuffer(IntPtr docDataExisting, string documentMoniker)
        {
            IVsTextLines textLines;

            if (docDataExisting == IntPtr.Zero)
            {
                //// Create a new IVsTextLines buffer
                textLines = this.CreateInstance<IVsTextLines, VsTextBufferClass>();
                //textLines = new TextBuffer();

                // set the buffer's site
                ((IObjectWithSite)textLines).SetSite(serviceProvider.GetService(typeof(IOleServiceProvider)));

                //// Fcuk COM
                //Guid GUID_VsBufferMoniker = typeof(IVsUserData).GUID;

                // Explicitly load the data through IVsPersistDocData
                ((IVsPersistDocData)textLines).LoadDocData(documentMoniker);

                
            }
            else
            {
                // Use the existing text buffer
                object dataObject = Marshal.GetObjectForIUnknown(docDataExisting);

                textLines = dataObject as IVsTextLines;

                if (textLines == null)
                {
                    // Try get the text buffer from textbuffer provider
                    IVsTextBufferProvider textBufferProvider = dataObject as IVsTextBufferProvider;

                    if (textBufferProvider != null)
                        textBufferProvider.GetTextBuffer(out textLines);
                }

                if (textLines == null)
                {
                    // Unknown docData type then, so we have to force VS to close the other editor.
                    ErrorHandler.ThrowOnFailure(VSConstants.VS_E_INCOMPATIBLEDOCDATA);
                }
            }

            return textLines;
        }

        private IntPtr CreateDocumentView(string physicalView, IVsHierarchy hierarchy, uint itemid, IVsTextLines textLines, out string editorCaption, out Guid cmdUI, string documentPath)
        {
            //Init out params
            editorCaption = String.Empty;
            cmdUI = Guid.Empty;

            try
            {
                // Create Form view
                return this.CreateDesignerView(hierarchy, itemid, textLines, ref editorCaption, ref cmdUI, documentPath);
            }
            catch //(InvalidOperationException ex)
            {
                throw;
            }

            //if (String.IsNullOrEmpty(physicalView))
            //{
            //    // create code window as default physical view
            //    return this.CreateCodeView(textLines, ref editorCaption, ref cmdUI);
            //}
            //else
            //{
            //    // Create Form view
            //    return this.CreateDesignerView(hierarchy, itemid, textLines, ref editorCaption, ref cmdUI, documentPath);
            //}

        }

        private IntPtr CreateDesignerView(IVsHierarchy hierarchy, uint itemid, IVsTextLines textLines, ref string editorCaption, ref Guid cmdUI, string documentMoniker)
        {
            // Request the Designer Service
            IVSMDDesignerService designerService = (IVSMDDesignerService)GetService(typeof(IVSMDDesignerService));

            try
            {
                // Get the service provider
                IOleServiceProvider provider = serviceProvider.GetService(typeof(IOleServiceProvider)) as IOleServiceProvider;
                IObjectWithSite ows = (IObjectWithSite)textLines;
                ows.SetSite(provider);
                // Create loader for the designer
                LimnorXmlDesignerLoader2 designerLoader = new LimnorXmlDesignerLoader2(documentMoniker, itemid);

                // Create the designer using the provider and the loader
                IVSMDDesigner designer = designerService.CreateDesigner(provider, designerLoader);

                designerLoader.AddComponentChangeEventHandler();

                // Retrieve the design surface
                DesignSurface designSurface = (DesignSurface)designer;

                IDesignerHost dh = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
                dh.AddService(typeof(INameCreationService), new NameCreationService());

                IServiceContainer serviceContainer = dh.GetService(typeof(ServiceContainer)) as IServiceContainer;
                dh.AddService(typeof(IDesignerSerializationService), new DesignerSerializationService(serviceContainer));
                //DesignerSerializationService uses CodeDomComponentSerializationService
                CodeDomComponentSerializationService codeDomComponentSerializationService = new CodeDomComponentSerializationService(serviceContainer);
                dh.AddService(typeof(ComponentSerializationService), codeDomComponentSerializationService);

                //LimnorUndoEngine undoEngine = new LimnorUndoEngine(serviceContainer);
                //undoEngine.Enabled = false;//not use undo during loading
                //dh.AddService(typeof(UndoEngine), undoEngine);
                //object v = dh.GetService(typeof(IOleUndoManager));
                //if (v != null)
                //{
                //    object v2 = dh.GetService(typeof(UndoEngine));
                //    if (v2 == null)
                //    {
                //        dh.AddService(typeof(UndoEngine), v);
                //    }
                //}
                // Create pane with this surface
                LimnorXmlPane2 limnorXmlPane = new LimnorXmlPane2(designSurface);//, winControlPME);

                // Get command guid from designer
                cmdUI = limnorXmlPane.CommandGuid;
                editorCaption = " [Design]";
                if (designerLoader.IsSetup)
                {
                }
                else
                {
                    if (limnorXmlPane.Loader.ObjectMap != null)
                    {
                        limnorXmlPane.BeginApplyConfig();
                    }
                }
                // Return LimnorXmlPane
                return Marshal.GetIUnknownForObject(limnorXmlPane);

            }
            catch (Exception ex)
            {
                MathNode.Log(ex);
                //Trace.WriteLine("Exception: " + ex.Message);
                // Just rethrow for now
                throw;
            }
        }

        private IntPtr CreateCodeView(IVsTextLines textLines, ref string editorCaption, ref Guid cmdUI)
        {
            IVsCodeWindow window = this.CreateInstance<IVsCodeWindow, VsCodeWindowClass>();

            ErrorHandler.ThrowOnFailure(window.SetBuffer(textLines));
            ErrorHandler.ThrowOnFailure(window.SetBaseEditorCaption(null));
            ErrorHandler.ThrowOnFailure(window.GetEditorCaption(READONLYSTATUS.ROSTATUS_Unknown, out editorCaption));

            cmdUI = VSConstants.GUID_TextEditorFactory;

            return Marshal.GetIUnknownForObject(window);
        }

        private TInterface CreateInstance<TInterface, TClass>()
            where TInterface : class
            where TClass : class
        {
            Guid clsid = typeof(TClass).GUID;
            Guid riid = typeof(TInterface).GUID;

            return (TInterface)editorPackage.CreateInstance(ref clsid, ref riid, typeof(TInterface));
        }

        #endregion
    }
}
