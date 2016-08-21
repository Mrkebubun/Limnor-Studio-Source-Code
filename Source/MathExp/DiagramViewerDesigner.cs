using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Drawing;

namespace MathExp
{
    public class DiagramViewerDesigner : ComponentDesigner, IRootDesigner
    {
        public DiagramViewerDesigner()
        {
        }
        #region Implementation of IRootDesigner
        DiagramViewer _rootView;
        public object GetView(System.ComponentModel.Design.ViewTechnology technology)
        {
            if (_rootView == null)
                _rootView = new DiagramViewer(this);
            return _rootView;
        }
        public System.ComponentModel.Design.ViewTechnology[] SupportedTechnologies
        {
            get
            {
                return new ViewTechnology[] { ViewTechnology.Default };
            }
        }
        #endregion

        #region DiagramViewer
        /// <summary>
        /// This is the View of the RootDesigner.
        /// </summary>
        public class DiagramViewer : Control
        {
            private DiagramViewerDesigner _rootDesigner;
            public DiagramViewer(DiagramViewerDesigner rootDesigner)
            {
                _rootDesigner = rootDesigner;
                //this.AllowDrop = true;
                this.BackColor = Color.White;
                Invalidate();
            }
            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
            }
            protected override void OnPaint(PaintEventArgs pe)
            {
                
            } // OnPaint


            public void InvokeToolboxItem(System.Drawing.Design.ToolboxItem tool)
            {
                IDesignerHost dh = DesignerHost;
                if (dh != null)
                {
                    
                }
                Invalidate();
            }
            protected override void OnDragDrop(DragEventArgs e)
            {
               
            }
            protected override void OnDragEnter(DragEventArgs e)
            {
                base.OnDragEnter(e);
                //
                
            }

            /// <summary>
            /// Clear the drag object we may have stored in OnDragEnter.
            /// </summary>
            protected override void OnDragLeave(EventArgs e)
            {
                base.OnDragLeave(e);
            }

            /// <summary>
            /// Overridden so we can give feedback about the drag.
            /// </summary>
            protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
            {
                base.OnGiveFeedback(e);
            }
            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                
            }
            public IDesignerHost DesignerHost
            {
                get
                {
                    return (IDesignerHost)_rootDesigner.GetService(typeof(IDesignerHost));
                }
            }

            public IToolboxService ToolboxService
            {
                get
                {
                    return (IToolboxService)_rootDesigner.GetService(typeof(IToolboxService));
                }
            }
            protected override void OnResize(EventArgs e)
            {
                Invalidate();
            }
        } // class NewComponentPromptView
        #endregion
    }
}
