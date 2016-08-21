using XHost;
using LimnorDatabase;
using VSPrj;
using VPL;
using LimnorDesigner;
namespace LimnorVOB
{
	partial class LimnorVOBMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
				//if (host != null)
				//{
				//    host.Dispose();
				//}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LimnorVOBMain));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonNewProject = new System.Windows.Forms.ToolStripButton();
			this.opentoolStripButton = new System.Windows.Forms.ToolStripButton();
			this.closetoolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSaveProject = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSaveSolution = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonRun = new System.Windows.Forms.ToolStripButton();
			this.toolStripComboBoxConfig = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripComboBoxTargetPlatform = new System.Windows.Forms.ToolStripComboBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveProjectAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.MruMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.logFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.errorLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.databaseLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.compilerErrorLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addWindowsFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addUserControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addComponentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripSeparator();
			this.manageDatabaseConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addToolboxItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resourceManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
			this.addWebServiceProxyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeWebServiceProxyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.updateWebServiceProxyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createServiceProxyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.licenseManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
			this.buildConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.projectBuildOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
			this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildModifiedWebPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startDebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startWithoutDebuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.stepIntoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stepOverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.manageVisualProgrammingSystemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
			this.manageMySQLDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.manageMicrosoftAccessDatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
			this.addMathLibToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sourceCompilerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.updateDynamicLinkLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			//this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
			this.projectConvertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripSeparator();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.usersGuideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tutorialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.referencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panelToolbox = new System.Windows.Forms.Panel();
			this.picHide = new System.Windows.Forms.PictureBox();
			this.picToolbox = new System.Windows.Forms.PictureBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.solutionTree1 = new SolutionMan.SolutionTree();
			this.comboBoxObjects = new LimnorDesigner.ComboBoxClassPointer();
			this.propertyGrid1 = new VPL.XPropertyGrid();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.outputWindow1 = new VOB.OutputWindow();
			this.imgEdit = new System.Windows.Forms.ImageList(this.components);
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.xToolbox1 = new XToolbox2.ToolboxPane2();
			this.makeHTMLReadableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removePrgCfgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.panelToolbox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picHide)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picToolbox)).BeginInit();
#if DOTNET40
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
#endif
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
#if DOTNET40
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
#endif
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
#if DOTNET40
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
#endif
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripButtonNewProject,
			this.opentoolStripButton,
			this.closetoolStripButton,
			this.toolStripButtonSaveProject,
			this.toolStripButtonSaveSolution,
			this.toolStripSeparator1,
			this.toolStripButtonRun,
			this.toolStripComboBoxConfig,
			this.toolStripComboBoxTargetPlatform});
			this.toolStrip1.Location = new System.Drawing.Point(0, 24);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(402, 25);
			this.toolStrip1.TabIndex = 1;
			// 
			// toolStripButtonNewProject
			// 
			this.toolStripButtonNewProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonNewProject.Image = global::LimnorVOB.Properties.Resources.newPrj;
			this.toolStripButtonNewProject.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonNewProject.Name = "toolStripButtonNewProject";
			this.toolStripButtonNewProject.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonNewProject.Text = "New Project";
			this.toolStripButtonNewProject.ToolTipText = "New Project";
			this.toolStripButtonNewProject.Click += new System.EventHandler(this.toolStripButtonNewProject_Click);
			// 
			// opentoolStripButton
			// 
			this.opentoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.opentoolStripButton.Image = global::LimnorVOB.Properties.Resources.openPrj1;
			this.opentoolStripButton.ImageTransparentColor = System.Drawing.Color.White;
			this.opentoolStripButton.Name = "opentoolStripButton";
			this.opentoolStripButton.Size = new System.Drawing.Size(23, 22);
			this.opentoolStripButton.Text = "Open solution";
			this.opentoolStripButton.Click += new System.EventHandler(this.openSolutionToolStripMenuItem_Click);
			// 
			// closetoolStripButton
			// 
			this.closetoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.closetoolStripButton.Image = global::LimnorVOB.Properties.Resources.closePrj;
			this.closetoolStripButton.ImageTransparentColor = System.Drawing.Color.White;
			this.closetoolStripButton.Name = "closetoolStripButton";
			this.closetoolStripButton.Size = new System.Drawing.Size(23, 22);
			this.closetoolStripButton.Text = "Close solution";
			this.closetoolStripButton.Click += new System.EventHandler(this.closeSolutionToolStripMenuItem_Click);
			// 
			// toolStripButtonSaveProject
			// 
			this.toolStripButtonSaveProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonSaveProject.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveProject.Image")));
			this.toolStripButtonSaveProject.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonSaveProject.Name = "toolStripButtonSaveProject";
			this.toolStripButtonSaveProject.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonSaveProject.Text = "toolStripButtonSaveProject";
			this.toolStripButtonSaveProject.ToolTipText = "save current project";
			this.toolStripButtonSaveProject.Click += new System.EventHandler(this.toolStripButtonSaveProject_Click);
			// 
			// toolStripButtonSaveSolution
			// 
			this.toolStripButtonSaveSolution.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonSaveSolution.Image = global::LimnorVOB.Properties.Resources.savePrj1;
			this.toolStripButtonSaveSolution.ImageTransparentColor = System.Drawing.Color.White;
			this.toolStripButtonSaveSolution.Name = "toolStripButtonSaveSolution";
			this.toolStripButtonSaveSolution.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonSaveSolution.Text = "Save project";
			this.toolStripButtonSaveSolution.ToolTipText = "Save solution";
			this.toolStripButtonSaveSolution.Click += new System.EventHandler(this.toolStripButtonSaveSolution_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonRun
			// 
			this.toolStripButtonRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonRun.Image = global::LimnorVOB.Properties.Resources._run;
			this.toolStripButtonRun.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonRun.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonRun.Name = "toolStripButtonRun";
			this.toolStripButtonRun.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonRun.Text = "Run";
			this.toolStripButtonRun.Click += new System.EventHandler(this.toolStripButtonRun_Click);
			// 
			// toolStripComboBoxConfig
			// 
			this.toolStripComboBoxConfig.Items.AddRange(new object[] {
				"Debug",
				"Release"});
			this.toolStripComboBoxConfig.Name = "toolStripComboBoxConfig";
			this.toolStripComboBoxConfig.Size = new System.Drawing.Size(121, 25);
			this.toolStripComboBoxConfig.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxConfig_SelectedIndexChanged);
			// 
			// toolStripComboBoxTargetPlatform
			// 
			this.toolStripComboBoxTargetPlatform.Items.AddRange(new object[] {
				"Any CPU",
				"x86 (32-bit)",
				"x64 (64-bit)"});
			this.toolStripComboBoxTargetPlatform.Name = "toolStripComboBoxTargetPlatform";
			this.toolStripComboBoxTargetPlatform.Size = new System.Drawing.Size(121, 25);
			this.toolStripComboBoxTargetPlatform.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxTargetPlatform_SelectedIndexChanged);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.fileToolStripMenuItem,
			this.editToolStripMenuItem,
			this.viewToolStripMenuItem,
			this.projectToolStripMenuItem,
			this.buildToolStripMenuItem,
			this.debugToolStripMenuItem,
			this.toolsToolStripMenuItem,
			this.windowToolStripMenuItem,
			this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(827, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			this.menuStrip1.MouseEnter += new System.EventHandler(this.menuStrip1_MouseEnter);
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.newSolutionToolStripMenuItem,
			this.openToolStripMenuItem,
			this.saveProjectToolStripMenuItem,
			this.saveProjectAsToolStripMenuItem,
			this.openSolutionToolStripMenuItem,
			this.saveSolutionToolStripMenuItem,
			this.closeSolutionToolStripMenuItem,
			this.toolStripMenuItem1,
			this.MruMenuItem,
			this.toolStripMenuItem6,
			this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// newSolutionToolStripMenuItem
			// 
			this.newSolutionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newSolutionToolStripMenuItem.Image")));
			this.newSolutionToolStripMenuItem.Name = "newSolutionToolStripMenuItem";
			this.newSolutionToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			this.newSolutionToolStripMenuItem.Text = "New Project";
			this.newSolutionToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Image = global::LimnorVOB.Properties.Resources.openPrj1;
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			this.openToolStripMenuItem.Text = "Open Project";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// saveProjectToolStripMenuItem
			// 
			this.saveProjectToolStripMenuItem.Image = global::LimnorVOB.Properties.Resources.save;
			this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
			this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			this.saveProjectToolStripMenuItem.Text = "Save Project";
			this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click_1);
			this.saveProjectToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+S";
			this.saveProjectToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
			// 
			// saveProjectAsToolStripMenuItem
			// 
			this.saveProjectAsToolStripMenuItem.Image = global::LimnorVOB.Properties.Resources.save;
			this.saveProjectAsToolStripMenuItem.Name = "saveProjectAsToolStripMenuItem";
			this.saveProjectAsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			this.saveProjectAsToolStripMenuItem.Text = "Copy Project to ...";
			this.saveProjectAsToolStripMenuItem.Click += new System.EventHandler(this.saveProjectAsToolStripMenuItem_Click);
			// 
			// openSolutionToolStripMenuItem
			// 
			this.openSolutionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openSolutionToolStripMenuItem.Image")));
			this.openSolutionToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.White;
			this.openSolutionToolStripMenuItem.Name = "openSolutionToolStripMenuItem";
			this.openSolutionToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			this.openSolutionToolStripMenuItem.Text = "Open Solution";
			this.openSolutionToolStripMenuItem.Click += new System.EventHandler(this.openSolutionToolStripMenuItem_Click);
			// 
			// saveSolutionToolStripMenuItem
			// 
			this.saveSolutionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveSolutionToolStripMenuItem.Image")));
			this.saveSolutionToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.White;
			this.saveSolutionToolStripMenuItem.Name = "saveSolutionToolStripMenuItem";
			this.saveSolutionToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			this.saveSolutionToolStripMenuItem.Text = "Save Solution";
			this.saveSolutionToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
			this.saveSolutionToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+S";
			this.saveSolutionToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.S;
			// 
			// closeSolutionToolStripMenuItem
			// 
			this.closeSolutionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeSolutionToolStripMenuItem.Image")));
			this.closeSolutionToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.White;
			this.closeSolutionToolStripMenuItem.Name = "closeSolutionToolStripMenuItem";
			this.closeSolutionToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			this.closeSolutionToolStripMenuItem.Text = "Close Solution";
			this.closeSolutionToolStripMenuItem.Click += new System.EventHandler(this.closeSolutionToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(165, 6);
			// 
			// MruMenuItem
			// 
			this.MruMenuItem.Name = "MruMenuItem";
			this.MruMenuItem.Size = new System.Drawing.Size(168, 22);
			this.MruMenuItem.Text = "Recent Solutions";
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(165, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.undoToolStripMenuItem,
			this.redoToolStripMenuItem,
			this.toolStripMenuItem2,
			this.cutToolStripMenuItem,
			this.copyToolStripMenuItem,
			this.pasteToolStripMenuItem,
			this.deleteToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "Edit";
			this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.undoToolStripMenuItem.Text = "Undo";
			this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
			// 
			// redoToolStripMenuItem
			// 
			this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
			this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
			this.redoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.redoToolStripMenuItem.Text = "Redo";
			this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(141, 6);
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.cutToolStripMenuItem.Text = "Cut";
			this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.pasteToolStripMenuItem.Text = "Paste";
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.logFileToolStripMenuItem,
			this.errorLogToolStripMenuItem,
			this.buildLogToolStripMenuItem,
			this.compilerErrorLogToolStripMenuItem,
			this.databaseLogToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// logFileToolStripMenuItem
			// 
			this.logFileToolStripMenuItem.Name = "logFileToolStripMenuItem";
			this.logFileToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.logFileToolStripMenuItem.Text = "IDE Log";
			this.logFileToolStripMenuItem.Click += new System.EventHandler(this.logFileToolStripMenuItem_Click);
			// 
			// errorLogToolStripMenuItem
			// 
			this.errorLogToolStripMenuItem.Name = "errorLogToolStripMenuItem";
			this.errorLogToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.errorLogToolStripMenuItem.Text = "IDE Error Log";
			this.errorLogToolStripMenuItem.Click += new System.EventHandler(this.errorLogToolStripMenuItem_Click);
			// 
			// buildLogToolStripMenuItem
			// 
			this.buildLogToolStripMenuItem.Name = "buildLogToolStripMenuItem";
			this.buildLogToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.buildLogToolStripMenuItem.Text = "Compiler Log";
			this.buildLogToolStripMenuItem.Click += new System.EventHandler(this.buildLogToolStripMenuItem_Click);
			// 
			// compilerErrorLogToolStripMenuItem
			// 
			this.compilerErrorLogToolStripMenuItem.Name = "compilerErrorLogToolStripMenuItem";
			this.compilerErrorLogToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.compilerErrorLogToolStripMenuItem.Text = "Compiler Error Log";
			this.compilerErrorLogToolStripMenuItem.Click += new System.EventHandler(this.compilerErrorLogToolStripMenuItem_Click);
			// 
			// databaseLogToolStripMenuItem
			// 
			this.databaseLogToolStripMenuItem.Name = "databaseLogToolStripMenuItem";
			this.databaseLogToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.databaseLogToolStripMenuItem.Text = "Database Log";
			this.databaseLogToolStripMenuItem.Visible = false;
			this.databaseLogToolStripMenuItem.Click += new System.EventHandler(this.databaseLogToolStripMenuItem_Click);
			// 
			// projectToolStripMenuItem
			// 
			this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.addWindowsFormToolStripMenuItem,
			this.addUserControlToolStripMenuItem,
			this.addComponentToolStripMenuItem,
			this.addClassToolStripMenuItem,
			this.toolStripMenuItem12,
			this.manageDatabaseConnectionsToolStripMenuItem,
			this.addToolboxItemToolStripMenuItem,
			this.resourceManagerToolStripMenuItem,
			this.toolStripMenuItem13,
			this.addWebServiceProxyToolStripMenuItem,
			this.removeWebServiceProxyToolStripMenuItem,
			this.updateWebServiceProxyToolStripMenuItem,
			this.createServiceProxyToolStripMenuItem,
			this.toolStripMenuItem3,
			this.licenseManagerToolStripMenuItem,
			this.toolStripMenuItem11,
			this.buildConfigurationToolStripMenuItem,
			this.projectBuildOrderToolStripMenuItem,
			this.toolStripMenuItem7,
			this.propertiesToolStripMenuItem});
			this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
			this.projectToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
			this.projectToolStripMenuItem.Text = "Project";
			// 
			// addWindowsFormToolStripMenuItem
			// 
			this.addWindowsFormToolStripMenuItem.Name = "addWindowsFormToolStripMenuItem";
			this.addWindowsFormToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.addWindowsFormToolStripMenuItem.Text = "Add Windows Form ...";
			this.addWindowsFormToolStripMenuItem.Click += new System.EventHandler(this.addWindowsFormToolStripMenuItem_Click);
			// 
			// addUserControlToolStripMenuItem
			// 
			this.addUserControlToolStripMenuItem.Name = "addUserControlToolStripMenuItem";
			this.addUserControlToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.addUserControlToolStripMenuItem.Text = "Add UserControl...";
			this.addUserControlToolStripMenuItem.Click += new System.EventHandler(this.addUserControlToolStripMenuItem_Click);
			// 
			// addComponentToolStripMenuItem
			// 
			this.addComponentToolStripMenuItem.Name = "addComponentToolStripMenuItem";
			this.addComponentToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.addComponentToolStripMenuItem.Text = "Add Component...";
			this.addComponentToolStripMenuItem.Click += new System.EventHandler(this.addComponentToolStripMenuItem_Click);
			// 
			// addClassToolStripMenuItem
			// 
			this.addClassToolStripMenuItem.Name = "addClassToolStripMenuItem";
			this.addClassToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.addClassToolStripMenuItem.Text = "Add Class...";
			this.addClassToolStripMenuItem.Click += new System.EventHandler(this.addClassToolStripMenuItem_Click);
			// 
			// toolStripMenuItem12
			// 
			this.toolStripMenuItem12.Name = "toolStripMenuItem12";
			this.toolStripMenuItem12.Size = new System.Drawing.Size(235, 6);
			// 
			// manageDatabaseConnectionsToolStripMenuItem
			// 
			this.manageDatabaseConnectionsToolStripMenuItem.Name = "manageDatabaseConnectionsToolStripMenuItem";
			this.manageDatabaseConnectionsToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.manageDatabaseConnectionsToolStripMenuItem.Text = "Manage Database Connections";
			this.manageDatabaseConnectionsToolStripMenuItem.Click += new System.EventHandler(this.manageDatabaseConnectionsToolStripMenuItem_Click);
			// 
			// addToolboxItemToolStripMenuItem
			// 
			this.addToolboxItemToolStripMenuItem.Enabled = false;
			this.addToolboxItemToolStripMenuItem.Name = "addToolboxItemToolStripMenuItem";
			this.addToolboxItemToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.addToolboxItemToolStripMenuItem.Text = "Add Toolbox Item...";
			this.addToolboxItemToolStripMenuItem.Click += new System.EventHandler(this.addToolboxItemToolStripMenuItem_Click);
			// 
			// resourceManagerToolStripMenuItem
			// 
			this.resourceManagerToolStripMenuItem.Name = "resourceManagerToolStripMenuItem";
			this.resourceManagerToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.resourceManagerToolStripMenuItem.Text = "Resource Manager";
			this.resourceManagerToolStripMenuItem.Click += new System.EventHandler(this.resourceManagerToolStripMenuItem_Click);
			// 
			// toolStripMenuItem13
			// 
			this.toolStripMenuItem13.Name = "toolStripMenuItem13";
			this.toolStripMenuItem13.Size = new System.Drawing.Size(235, 6);
			// 
			// addWebServiceProxyToolStripMenuItem
			// 
			this.addWebServiceProxyToolStripMenuItem.Name = "addWebServiceProxyToolStripMenuItem";
			this.addWebServiceProxyToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.addWebServiceProxyToolStripMenuItem.Text = "Add Web Service Proxy";
			this.addWebServiceProxyToolStripMenuItem.Click += new System.EventHandler(this.addWebServiceProxyToolStripMenuItem_Click);
			// 
			// removeWebServiceProxyToolStripMenuItem
			// 
			this.removeWebServiceProxyToolStripMenuItem.Name = "removeWebServiceProxyToolStripMenuItem";
			this.removeWebServiceProxyToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.removeWebServiceProxyToolStripMenuItem.Text = "Remove Web Service Proxy";
			this.removeWebServiceProxyToolStripMenuItem.Click += new System.EventHandler(this.removeWebServiceProxyToolStripMenuItem_Click);
			// 
			// updateWebServiceProxyToolStripMenuItem
			// 
			this.updateWebServiceProxyToolStripMenuItem.Name = "updateWebServiceProxyToolStripMenuItem";
			this.updateWebServiceProxyToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.updateWebServiceProxyToolStripMenuItem.Text = "Update Web Service Proxy";
			this.updateWebServiceProxyToolStripMenuItem.Click += new System.EventHandler(this.updateWebServiceProxyToolStripMenuItem_Click);
			// 
			// createServiceProxyToolStripMenuItem
			// 
			this.createServiceProxyToolStripMenuItem.Name = "createServiceProxyToolStripMenuItem";
			this.createServiceProxyToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.createServiceProxyToolStripMenuItem.Text = "Create Service Proxy";
			this.createServiceProxyToolStripMenuItem.Click += new System.EventHandler(this.createServiceProxyToolStripMenuItem_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(235, 6);
			// 
			// licenseManagerToolStripMenuItem
			// 
			this.licenseManagerToolStripMenuItem.Name = "licenseManagerToolStripMenuItem";
			this.licenseManagerToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.licenseManagerToolStripMenuItem.Text = "License Manager";
			this.licenseManagerToolStripMenuItem.Click += new System.EventHandler(this.licenseManagerToolStripMenuItem_Click);
			// 
			// toolStripMenuItem11
			// 
			this.toolStripMenuItem11.Name = "toolStripMenuItem11";
			this.toolStripMenuItem11.Size = new System.Drawing.Size(235, 6);
			// 
			// buildConfigurationToolStripMenuItem
			// 
			this.buildConfigurationToolStripMenuItem.Name = "buildConfigurationToolStripMenuItem";
			this.buildConfigurationToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.buildConfigurationToolStripMenuItem.Text = "Build Configuration";
			this.buildConfigurationToolStripMenuItem.Click += new System.EventHandler(this.buildConfigurationToolStripMenuItem_Click);
			// 
			// projectBuildOrderToolStripMenuItem
			// 
			this.projectBuildOrderToolStripMenuItem.Name = "projectBuildOrderToolStripMenuItem";
			this.projectBuildOrderToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.projectBuildOrderToolStripMenuItem.Text = "Project Build Order ...";
			this.projectBuildOrderToolStripMenuItem.Click += new System.EventHandler(this.projectBuildOrderToolStripMenuItem_Click);
			// 
			// toolStripMenuItem7
			// 
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new System.Drawing.Size(235, 6);
			// 
			// propertiesToolStripMenuItem
			// 
			this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
			this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
			this.propertiesToolStripMenuItem.Text = "Properties";
			this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
			// 
			// buildToolStripMenuItem
			// 
			this.buildToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.buildModifiedWebPagesToolStripMenuItem,
			this.buildProjectToolStripMenuItem,
			this.buildSolutionToolStripMenuItem,
			this.configurationToolStripMenuItem});
			this.buildToolStripMenuItem.Name = "buildToolStripMenuItem";
			this.buildToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
			this.buildToolStripMenuItem.Text = "Build";
			// 
			// buildModifiedWebPagesToolStripMenuItem
			// 
			this.buildModifiedWebPagesToolStripMenuItem.Name = "buildModifiedWebPagesToolStripMenuItem";
			this.buildModifiedWebPagesToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
			this.buildModifiedWebPagesToolStripMenuItem.Text = "Build Modified Web Pages";
			this.buildModifiedWebPagesToolStripMenuItem.Click += new System.EventHandler(this.buildModifiedWebPagesToolStripMenuItem_Click);
			this.buildModifiedWebPagesToolStripMenuItem.ShortcutKeyDisplayString = "F6";
			this.buildModifiedWebPagesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
			// 
			// buildProjectToolStripMenuItem
			// 
			this.buildProjectToolStripMenuItem.Name = "buildProjectToolStripMenuItem";
			this.buildProjectToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
			this.buildProjectToolStripMenuItem.Text = "Build Project";
			this.buildProjectToolStripMenuItem.Click += new System.EventHandler(this.buildProjectToolStripMenuItem_Click);
			this.buildProjectToolStripMenuItem.ShortcutKeyDisplayString = "Shift+F6";
			this.buildProjectToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F6;
			// 
			// buildSolutionToolStripMenuItem
			// 
			this.buildSolutionToolStripMenuItem.Name = "buildSolutionToolStripMenuItem";
			this.buildSolutionToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
			this.buildSolutionToolStripMenuItem.Text = "Build Solution";
			this.buildSolutionToolStripMenuItem.Click += new System.EventHandler(this.buildSolutionToolStripMenuItem_Click);
			this.buildSolutionToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+F6";
			this.buildSolutionToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F6;
			// 
			// configurationToolStripMenuItem
			// 
			this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
			this.configurationToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
			this.configurationToolStripMenuItem.Text = "Configuration";
			// 
			// debugToolStripMenuItem
			// 
			this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.startDebugToolStripMenuItem,
			this.startWithoutDebuggingToolStripMenuItem,
			this.toolStripMenuItem4,
			this.stepIntoToolStripMenuItem,
			this.stepOverToolStripMenuItem});
			this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
			this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this.debugToolStripMenuItem.Text = "Debug";
			// 
			// startDebugToolStripMenuItem
			// 
			this.startDebugToolStripMenuItem.Name = "startDebugToolStripMenuItem";
			this.startDebugToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.startDebugToolStripMenuItem.Text = "Start Debugging";
			this.startDebugToolStripMenuItem.Click += new System.EventHandler(this.startDebugToolStripMenuItem_Click);
			this.startDebugToolStripMenuItem.ShortcutKeyDisplayString = "F5";
			this.startDebugToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			// 
			// startWithoutDebuggingToolStripMenuItem
			// 
			this.startWithoutDebuggingToolStripMenuItem.Name = "startWithoutDebuggingToolStripMenuItem";
			this.startWithoutDebuggingToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.startWithoutDebuggingToolStripMenuItem.Text = "Start without Debugging";
			this.startWithoutDebuggingToolStripMenuItem.Click += new System.EventHandler(this.startWithoutDebuggingToolStripMenuItem_Click);
			this.startWithoutDebuggingToolStripMenuItem.Visible = false;
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(201, 6);
			// 
			// stepIntoToolStripMenuItem
			// 
			this.stepIntoToolStripMenuItem.Name = "stepIntoToolStripMenuItem";
			this.stepIntoToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.stepIntoToolStripMenuItem.Text = "Step Into";
			// 
			// stepOverToolStripMenuItem
			// 
			this.stepOverToolStripMenuItem.Name = "stepOverToolStripMenuItem";
			this.stepOverToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.stepOverToolStripMenuItem.Text = "Step Over";
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.manageVisualProgrammingSystemsToolStripMenuItem,
			this.toolStripMenuItem10,
			this.manageMySQLDatabaseToolStripMenuItem,
			this.manageMicrosoftAccessDatToolStripMenuItem,
			this.toolStripMenuItem9,
			this.addMathLibToolStripMenuItem,
			this.sourceCompilerToolStripMenuItem,
			this.updateDynamicLinkLibraryToolStripMenuItem,
			this.makeHTMLReadableToolStripMenuItem,
			this.removePrgCfgToolStripMenuItem,
			//this.toolStripMenuItem8,
			this.projectConvertToolStripMenuItem,
			this.insertIconsToolStripMenuItem,
			this.toolStripMenuItem14,
			this.optionsToolStripMenuItem,
			this.customizeToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.toolsToolStripMenuItem.Text = "Tools";
			// 
			// manageVisualProgrammingSystemsToolStripMenuItem
			// 
			this.manageVisualProgrammingSystemsToolStripMenuItem.Name = "manageVisualProgrammingSystemsToolStripMenuItem";
			this.manageVisualProgrammingSystemsToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.manageVisualProgrammingSystemsToolStripMenuItem.Text = "Manage Visual Programming Systems";
			this.manageVisualProgrammingSystemsToolStripMenuItem.Click += new System.EventHandler(this.manageVisualProgrammingSystemsToolStripMenuItem_Click);
			// 
			// toolStripMenuItem10
			// 
			this.toolStripMenuItem10.Name = "toolStripMenuItem10";
			this.toolStripMenuItem10.Size = new System.Drawing.Size(271, 6);
			// 
			// manageMySQLDatabaseToolStripMenuItem
			// 
			this.manageMySQLDatabaseToolStripMenuItem.Name = "manageMySQLDatabaseToolStripMenuItem";
			this.manageMySQLDatabaseToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.manageMySQLDatabaseToolStripMenuItem.Text = "Manage My SQL Database...";
			this.manageMySQLDatabaseToolStripMenuItem.Click += new System.EventHandler(this.manageMySQLDatabaseToolStripMenuItem_Click);
			// 
			// manageMicrosoftAccessDatToolStripMenuItem
			// 
			this.manageMicrosoftAccessDatToolStripMenuItem.Name = "manageMicrosoftAccessDatToolStripMenuItem";
			this.manageMicrosoftAccessDatToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.manageMicrosoftAccessDatToolStripMenuItem.Text = "Manage Microsoft Access Daabaset...";
			this.manageMicrosoftAccessDatToolStripMenuItem.Click += new System.EventHandler(this.manageMicrosoftAccessDatToolStripMenuItem_Click);
			// 
			// toolStripMenuItem9
			// 
			this.toolStripMenuItem9.Name = "toolStripMenuItem9";
			this.toolStripMenuItem9.Size = new System.Drawing.Size(271, 6);
			// 
			// addMathLibToolStripMenuItem
			// 
			this.addMathLibToolStripMenuItem.Name = "addMathLibToolStripMenuItem";
			this.addMathLibToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.addMathLibToolStripMenuItem.Text = "Add Math Library";
			this.addMathLibToolStripMenuItem.Click += new System.EventHandler(this.addMathLibToolStripMenuItem_Click);
			// 
			// sourceCompilerToolStripMenuItem
			// 
			this.sourceCompilerToolStripMenuItem.Name = "sourceCompilerToolStripMenuItem";
			this.sourceCompilerToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.sourceCompilerToolStripMenuItem.Text = "Source Compiler";
			this.sourceCompilerToolStripMenuItem.Click += new System.EventHandler(this.sourceCompilerToolStripMenuItem_Click);
			// 
			// updateDynamicLinkLibraryToolStripMenuItem
			// 
			this.updateDynamicLinkLibraryToolStripMenuItem.Name = "updateDynamicLinkLibraryToolStripMenuItem";
			this.updateDynamicLinkLibraryToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.updateDynamicLinkLibraryToolStripMenuItem.Text = "Update Dynamic Link Library";
			this.updateDynamicLinkLibraryToolStripMenuItem.Click += new System.EventHandler(this.updateDynamicLinkLibraryToolStripMenuItem_Click);
			// 
			// toolStripMenuItem8
			// 
			//this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			//this.toolStripMenuItem8.Size = new System.Drawing.Size(271, 6);
			//
			// projectConvertToolStripMenuItem
			//
			this.projectConvertToolStripMenuItem.Name = "projectConvertToolStripMenuItem";
			this.projectConvertToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.projectConvertToolStripMenuItem.Text = "Copy and convert project...";
			this.projectConvertToolStripMenuItem.Click += new System.EventHandler(this.projectConvertToolStripMenuItem_Click);
			//
			// insertIconsToolStripMenuItem
			//
			this.insertIconsToolStripMenuItem.Name = "insertIconsToolStripMenuItem";
			this.insertIconsToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.insertIconsToolStripMenuItem.Text = "Insert Win32 icons to executable...";
			this.insertIconsToolStripMenuItem.Click += new System.EventHandler(this.insertIconsToolStripMenuItem_Click);
			//
			// toolStripMenuItem14
			//
			this.toolStripMenuItem14.Name = "toolStripMenuItem14";
			this.toolStripMenuItem14.Size = new System.Drawing.Size(271, 6);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.optionsToolStripMenuItem.Text = "Options...";
			this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
			//
			// customizeToolStripMenuItem
			// 
			this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
			this.customizeToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.customizeToolStripMenuItem.Text = "Customize...";
			this.customizeToolStripMenuItem.Click += new System.EventHandler(this.customizeToolStripMenuItem_Click);
			this.customizeToolStripMenuItem.Visible = false;
			// 
			// windowToolStripMenuItem
			// 
			this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.windowsToolStripMenuItem});
			this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
			this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
			this.windowToolStripMenuItem.Text = "Window";
			// 
			// windowsToolStripMenuItem
			// 
			this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
			this.windowsToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
			this.windowsToolStripMenuItem.Text = "Windows...";
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.contentsToolStripMenuItem,
			this.toolStripMenuItem5,
			this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// contentsToolStripMenuItem
			// 
			this.contentsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.usersGuideToolStripMenuItem,
			this.tutorialsToolStripMenuItem,
			this.referencesToolStripMenuItem});
			this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
			this.contentsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
			this.contentsToolStripMenuItem.Text = "Contents";
			// 
			// usersGuideToolStripMenuItem
			// 
			this.usersGuideToolStripMenuItem.Name = "usersGuideToolStripMenuItem";
			this.usersGuideToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.usersGuideToolStripMenuItem.Text = "Users\' Guide";
			this.usersGuideToolStripMenuItem.Click += new System.EventHandler(this.usersGuideToolStripMenuItem_Click);
			// 
			// tutorialsToolStripMenuItem
			// 
			this.tutorialsToolStripMenuItem.Name = "tutorialsToolStripMenuItem";
			this.tutorialsToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.tutorialsToolStripMenuItem.Text = "Tutorials";
			this.tutorialsToolStripMenuItem.Click += new System.EventHandler(this.tutorialsToolStripMenuItem_Click);
			// 
			// referencesToolStripMenuItem
			// 
			this.referencesToolStripMenuItem.Name = "referencesToolStripMenuItem";
			this.referencesToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.referencesToolStripMenuItem.Text = "References";
			this.referencesToolStripMenuItem.Click += new System.EventHandler(this.referencesToolStripMenuItem_Click);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(119, 6);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// panelToolbox
			// 
			this.panelToolbox.BackColor = System.Drawing.SystemColors.ControlLight;
			this.panelToolbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelToolbox.Controls.Add(this.picHide);
			this.panelToolbox.Controls.Add(this.picToolbox);
			this.panelToolbox.Location = new System.Drawing.Point(0, 52);
			this.panelToolbox.Name = "panelToolbox";
			this.panelToolbox.Size = new System.Drawing.Size(24, 243);
			this.panelToolbox.TabIndex = 3;
			this.panelToolbox.MouseHover += new System.EventHandler(this.panelToolbox_MouseHover);
			// 
			// picHide
			// 
			this.picHide.Image = ((System.Drawing.Image)(resources.GetObject("picHide.Image")));
			this.picHide.Location = new System.Drawing.Point(0, 70);
			this.picHide.Name = "picHide";
			this.picHide.Size = new System.Drawing.Size(20, 95);
			this.picHide.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.picHide.TabIndex = 3;
			this.picHide.TabStop = false;
			this.picHide.MouseHover += new System.EventHandler(this.picHide_MouseHover);
			// 
			// picToolbox
			// 
			this.picToolbox.Image = global::LimnorVOB.Properties.Resources.toolbox2;
			this.picToolbox.Location = new System.Drawing.Point(0, 0);
			this.picToolbox.Name = "picToolbox";
			this.picToolbox.Size = new System.Drawing.Size(20, 63);
			this.picToolbox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.picToolbox.TabIndex = 2;
			this.picToolbox.TabStop = false;
			this.picToolbox.MouseEnter += new System.EventHandler(this.picToolbox_MouseEnter);
			// 
			// splitContainer1
			// 
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.solutionTree1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.comboBoxObjects);
			this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
			this.splitContainer1.Size = new System.Drawing.Size(153, 280);
			this.splitContainer1.SplitterDistance = 103;
			this.splitContainer1.TabIndex = 6;
			// 
			// solutionTree1
			// 
			this.solutionTree1.Dirty = false;
			this.solutionTree1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.solutionTree1.Location = new System.Drawing.Point(0, 0);
			this.solutionTree1.Name = "solutionTree1";
			this.solutionTree1.SelectedNode = null;
			this.solutionTree1.ServiceProvider = null;
			this.solutionTree1.Size = new System.Drawing.Size(149, 99);
			this.solutionTree1.TabIndex = 0;
			this.solutionTree1.VobService = null;
			// 
			// comboBoxObjects
			// 
			this.comboBoxObjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxObjects.FormattingEnabled = true;
			this.comboBoxObjects.Location = new System.Drawing.Point(-2, -2);
			this.comboBoxObjects.Name = "comboBoxObjects";
			this.comboBoxObjects.Size = new System.Drawing.Size(153, 21);
			this.comboBoxObjects.TabIndex = 9;
			this.comboBoxObjects.SelectedObject += new System.EventHandler(this.comboBoxObjects_SelectedObject);
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid1.DrawFlat = false;
			this.propertyGrid1.IsInEditing = false;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 17);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(149, 151);
			this.propertyGrid1.TabIndex = 8;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.tabControl1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
			this.splitContainer2.Size = new System.Drawing.Size(520, 280);
			this.splitContainer2.SplitterDistance = 363;
			this.splitContainer2.TabIndex = 7;
			// 
			// tabControl1
			// 
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(363, 280);
			this.tabControl1.TabIndex = 5;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// splitContainer3
			// 
			this.splitContainer3.Location = new System.Drawing.Point(27, 52);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.outputWindow1);
			this.splitContainer3.Size = new System.Drawing.Size(520, 351);
			this.splitContainer3.SplitterDistance = 280;
			this.splitContainer3.TabIndex = 8;
			this.splitContainer3.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer3_SplitterMoved);
			// 
			// outputWindow1
			// 
			this.outputWindow1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.outputWindow1.IsShowException = true;
			this.outputWindow1.Location = new System.Drawing.Point(0, 0);
			this.outputWindow1.Name = "outputWindow1";
			this.outputWindow1.Size = new System.Drawing.Size(520, 67);
			this.outputWindow1.TabIndex = 0;
			// 
			// imgEdit
			// 
			this.imgEdit.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgEdit.ImageStream")));
			this.imgEdit.TransparentColor = System.Drawing.Color.White;
			this.imgEdit.Images.SetKeyName(0, "undo_enable.bmp");
			this.imgEdit.Images.SetKeyName(1, "undo_disable.bmp");
			this.imgEdit.Images.SetKeyName(2, "Redo_enable.bmp");
			this.imgEdit.Images.SetKeyName(3, "redo_disable.bmp");
			this.imgEdit.Images.SetKeyName(4, "cut.bmp");
			this.imgEdit.Images.SetKeyName(5, "copy.bmp");
			this.imgEdit.Images.SetKeyName(6, "paste.bmp");
			this.imgEdit.Images.SetKeyName(7, "delete.bmp");
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::LimnorVOB.Properties.Resources.toolbox2;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(20, 63);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// xToolbox1
			// 
			this.xToolbox1.AutoScroll = true;
			this.xToolbox1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.xToolbox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.xToolbox1.Changed = false;
			this.xToolbox1.GenericServices = null;
			this.xToolbox1.Host = null;
			this.xToolbox1.IsInDisplay = false;
			this.xToolbox1.Location = new System.Drawing.Point(26, 52);
			this.xToolbox1.Name = "xToolbox1";
			this.xToolbox1.SelectedCategory = "";
			this.xToolbox1.Size = new System.Drawing.Size(183, 243);
			this.xToolbox1.TabIndex = 4;
			this.xToolbox1.ToolXml = null;
			// 
			// makeHTMLReadableToolStripMenuItem
			// 
			this.makeHTMLReadableToolStripMenuItem.Name = "makeHTMLReadableToolStripMenuItem";
			this.makeHTMLReadableToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.makeHTMLReadableToolStripMenuItem.Text = "Make HTML Readable";
			this.makeHTMLReadableToolStripMenuItem.Click += new System.EventHandler(this.makeHTMLReadableToolStripMenuItem_Click);
			// 
			// removePrgCfgToolStripMenuItem
			// 
			this.removePrgCfgToolStripMenuItem.Name = "removePrgCfgToolStripMenuItem";
			this.removePrgCfgToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
			this.removePrgCfgToolStripMenuItem.Text = "Application Configuration Utility";
			this.removePrgCfgToolStripMenuItem.Click += new System.EventHandler(this.removePrgCfgToolStripMenuItem_Click);

			// 
			// LimnorVOBMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.ClientSize = new System.Drawing.Size(827, 432);
			this.Controls.Add(this.splitContainer3);
			this.Controls.Add(this.xToolbox1);
			this.Controls.Add(this.panelToolbox);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.toolStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "LimnorVOBMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Limnor Studio for Mono";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.panelToolbox.ResumeLayout(false);
			this.panelToolbox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picHide)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picToolbox)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
#if DOTNET40
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
#endif
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
#if DOTNET40
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
#endif
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
#if DOTNET40
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
#endif
			this.splitContainer3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonNewProject;
		private System.Windows.Forms.ToolStripButton opentoolStripButton;
		private System.Windows.Forms.ToolStripButton closetoolStripButton;
		private System.Windows.Forms.ToolStripButton toolStripButtonSaveSolution;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newSolutionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openSolutionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveSolutionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeSolutionToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addWindowsFormToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addUserControlToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addComponentToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem buildToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem buildProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startDebugToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startWithoutDebuggingToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem stepIntoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stepOverToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem projectConvertToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertIconsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem customizeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.PictureBox picToolbox;
		private System.Windows.Forms.Panel panelToolbox;
		private System.Windows.Forms.PictureBox pictureBox1;
		private XToolbox2.ToolboxPane2 xToolbox1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private XPropertyGrid propertyGrid1;
		//private HostTab tabPage1;
		private SolutionMan.SolutionTree solutionTree1;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private VOB.OutputWindow outputWindow1;
		private System.Windows.Forms.ImageList imgEdit;
		private System.Windows.Forms.ToolStripMenuItem MruMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem logFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem buildLogToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem databaseLogToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addMathLibToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem buildSolutionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBoxConfig;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton toolStripButtonRun;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBoxTargetPlatform;
		private System.Windows.Forms.ToolStripMenuItem projectBuildOrderToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
		private System.Windows.Forms.ToolStripButton toolStripButtonSaveProject;
		private System.Windows.Forms.ToolStripMenuItem addClassToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem usersGuideToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tutorialsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem referencesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem manageMySQLDatabaseToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem manageMicrosoftAccessDatToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem manageDatabaseConnectionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem11;
		private System.Windows.Forms.ToolStripMenuItem manageVisualProgrammingSystemsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem10;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem9;
		//private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem12;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem14;
		private System.Windows.Forms.ToolStripMenuItem addToolboxItemToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem resourceManagerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem licenseManagerToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem13;
		private System.Windows.Forms.ToolStripMenuItem addWebServiceProxyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeWebServiceProxyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem updateWebServiceProxyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem buildConfigurationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sourceCompilerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem updateDynamicLinkLibraryToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem createServiceProxyToolStripMenuItem;
		private System.Windows.Forms.PictureBox picHide;
		private ComboBoxClassPointer comboBoxObjects;
		private System.Windows.Forms.ToolStripMenuItem buildModifiedWebPagesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem errorLogToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem compilerErrorLogToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveProjectAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem makeHTMLReadableToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removePrgCfgToolStripMenuItem;
	}
}

