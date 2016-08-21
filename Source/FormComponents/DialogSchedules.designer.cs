namespace FormComponents
{
    partial class DialogSchedules
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btDelete = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.buttonAssignActions = new System.Windows.Forms.Button();
            this.groupBoxSpecific = new System.Windows.Forms.GroupBox();
            this.textBoxSecondSpec = new FormComponents.TextBoxNumber();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxMinuteSpec = new FormComponents.TextBoxNumber();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxHourSpec = new FormComponents.TextBoxNumber();
            this.label12 = new System.Windows.Forms.Label();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBoxMinutes = new System.Windows.Forms.GroupBox();
            this.textBoxHour = new FormComponents.TextBoxNumber();
            this.labelHour = new System.Windows.Forms.Label();
            this.textBoxMinute = new FormComponents.TextBoxNumber();
            this.labelMinute = new System.Windows.Forms.Label();
            this.textBoxSecond = new FormComponents.TextBoxNumber();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBoxWeekLy = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cbWeekly = new System.Windows.Forms.ComboBox();
            this.groupBoxYear = new System.Windows.Forms.GroupBox();
            this.textBoxDay = new FormComponents.TextBoxNumber();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxMonth = new FormComponents.TextBoxNumber();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBoxInterval = new System.Windows.Forms.GroupBox();
            this.labelIntervalUnit = new System.Windows.Forms.Label();
            this.textBoxInterval = new FormComponents.TextBoxNumber();
            this.labelInterval = new System.Windows.Forms.Label();
            this.chkEnable = new System.Windows.Forms.CheckBox();
            this.textBoxMax = new FormComponents.TextBoxNumber();
            this.label3 = new System.Windows.Forms.Label();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxName1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBoxSpecific.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxMinutes.SuspendLayout();
            this.groupBoxWeekLy.SuspendLayout();
            this.groupBoxYear.SuspendLayout();
            this.groupBoxInterval.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btDelete);
            this.splitContainer1.Panel1.Controls.Add(this.btAdd);
            this.splitContainer1.Panel1.Controls.Add(this.btCancel);
            this.splitContainer1.Panel1.Controls.Add(this.btOK);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(707, 442);
            this.splitContainer1.SplitterDistance = 34;
            this.splitContainer1.TabIndex = 0;
            // 
            // btDelete
            // 
            this.btDelete.Location = new System.Drawing.Point(267, 3);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(75, 23);
            this.btDelete.TabIndex = 3;
            this.btDelete.Text = "Delete";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btAdd
            // 
            this.btAdd.Location = new System.Drawing.Point(186, 3);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(75, 23);
            this.btAdd.TabIndex = 2;
            this.btAdd.Text = "Add";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(93, 3);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(12, 3);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 0;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.buttonAssignActions);
            this.splitContainer2.Panel2.Controls.Add(this.groupBoxSpecific);
            this.splitContainer2.Panel2.Controls.Add(this.label4);
            this.splitContainer2.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer2.Panel2.Controls.Add(this.chkEnable);
            this.splitContainer2.Panel2.Controls.Add(this.textBoxMax);
            this.splitContainer2.Panel2.Controls.Add(this.label3);
            this.splitContainer2.Panel2.Controls.Add(this.cbType);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Panel2.Controls.Add(this.textBoxName1);
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Size = new System.Drawing.Size(707, 404);
            this.splitContainer2.SplitterDistance = 199;
            this.splitContainer2.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.ShowLines = false;
            this.treeView1.ShowPlusMinus = false;
            this.treeView1.ShowRootLines = false;
            this.treeView1.Size = new System.Drawing.Size(199, 404);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // buttonAssignActions
            // 
            this.buttonAssignActions.Enabled = false;
            this.buttonAssignActions.Location = new System.Drawing.Point(277, 69);
            this.buttonAssignActions.Name = "buttonAssignActions";
            this.buttonAssignActions.Size = new System.Drawing.Size(82, 23);
            this.buttonAssignActions.TabIndex = 12;
            this.buttonAssignActions.Text = "Actions";
            this.buttonAssignActions.UseVisualStyleBackColor = true;
            this.buttonAssignActions.Click += new System.EventHandler(this.buttonAssignActions_Click);
            // 
            // groupBoxSpecific
            // 
            this.groupBoxSpecific.Controls.Add(this.textBoxSecondSpec);
            this.groupBoxSpecific.Controls.Add(this.label14);
            this.groupBoxSpecific.Controls.Add(this.textBoxMinuteSpec);
            this.groupBoxSpecific.Controls.Add(this.label13);
            this.groupBoxSpecific.Controls.Add(this.textBoxHourSpec);
            this.groupBoxSpecific.Controls.Add(this.label12);
            this.groupBoxSpecific.Controls.Add(this.monthCalendar1);
            this.groupBoxSpecific.Location = new System.Drawing.Point(25, 109);
            this.groupBoxSpecific.Name = "groupBoxSpecific";
            this.groupBoxSpecific.Size = new System.Drawing.Size(334, 181);
            this.groupBoxSpecific.TabIndex = 11;
            this.groupBoxSpecific.TabStop = false;
            // 
            // textBoxSecondSpec
            // 
            this.textBoxSecondSpec._RawText = "0";
            this.textBoxSecondSpec.Location = new System.Drawing.Point(288, 94);
            this.textBoxSecondSpec.MaximumValue = ((long)(59));
            this.textBoxSecondSpec.MinimumValue = ((long)(0));
            this.textBoxSecondSpec.Name = "textBoxSecondSpec";
            this.textBoxSecondSpec.NumericValue = 0;
            this.textBoxSecondSpec.Size = new System.Drawing.Size(25, 20);
            this.textBoxSecondSpec.TabIndex = 6;
            this.textBoxSecondSpec.Text = "0";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(244, 97);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 13);
            this.label14.TabIndex = 5;
            this.label14.Text = "Second:";
            // 
            // textBoxMinuteSpec
            // 
            this.textBoxMinuteSpec._RawText = "0";
            this.textBoxMinuteSpec.Location = new System.Drawing.Point(288, 60);
            this.textBoxMinuteSpec.MaximumValue = ((long)(59));
            this.textBoxMinuteSpec.MinimumValue = ((long)(0));
            this.textBoxMinuteSpec.Name = "textBoxMinuteSpec";
            this.textBoxMinuteSpec.NumericValue = 0;
            this.textBoxMinuteSpec.Size = new System.Drawing.Size(25, 20);
            this.textBoxMinuteSpec.TabIndex = 4;
            this.textBoxMinuteSpec.Text = "0";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(244, 63);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(42, 13);
            this.label13.TabIndex = 3;
            this.label13.Text = "Minute:";
            // 
            // textBoxHourSpec
            // 
            this.textBoxHourSpec._RawText = "0";
            this.textBoxHourSpec.Location = new System.Drawing.Point(288, 28);
            this.textBoxHourSpec.MaximumValue = ((long)(23));
            this.textBoxHourSpec.MinimumValue = ((long)(0));
            this.textBoxHourSpec.Name = "textBoxHourSpec";
            this.textBoxHourSpec.NumericValue = 0;
            this.textBoxHourSpec.Size = new System.Drawing.Size(25, 20);
            this.textBoxHourSpec.TabIndex = 2;
            this.textBoxHourSpec.Text = "0";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(249, 31);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(33, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Hour:";
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(15, 13);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "0: unlimited occurrences";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBoxMinutes);
            this.groupBox1.Controls.Add(this.groupBoxWeekLy);
            this.groupBox1.Controls.Add(this.groupBoxYear);
            this.groupBox1.Controls.Add(this.groupBoxInterval);
            this.groupBox1.Location = new System.Drawing.Point(25, 109);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(334, 181);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // groupBoxMinutes
            // 
            this.groupBoxMinutes.Controls.Add(this.textBoxHour);
            this.groupBoxMinutes.Controls.Add(this.labelHour);
            this.groupBoxMinutes.Controls.Add(this.textBoxMinute);
            this.groupBoxMinutes.Controls.Add(this.labelMinute);
            this.groupBoxMinutes.Controls.Add(this.textBoxSecond);
            this.groupBoxMinutes.Controls.Add(this.label6);
            this.groupBoxMinutes.Location = new System.Drawing.Point(8, 66);
            this.groupBoxMinutes.Name = "groupBoxMinutes";
            this.groupBoxMinutes.Size = new System.Drawing.Size(250, 106);
            this.groupBoxMinutes.TabIndex = 11;
            this.groupBoxMinutes.TabStop = false;
            // 
            // textBoxHour
            // 
            this.textBoxHour._RawText = "0";
            this.textBoxHour.Location = new System.Drawing.Point(80, 80);
            this.textBoxHour.MaximumValue = ((long)(23));
            this.textBoxHour.MinimumValue = ((long)(0));
            this.textBoxHour.Name = "textBoxHour";
            this.textBoxHour.NumericValue = 0;
            this.textBoxHour.Size = new System.Drawing.Size(82, 20);
            this.textBoxHour.TabIndex = 5;
            this.textBoxHour.Text = "0";
            // 
            // labelHour
            // 
            this.labelHour.AutoSize = true;
            this.labelHour.Location = new System.Drawing.Point(19, 87);
            this.labelHour.Name = "labelHour";
            this.labelHour.Size = new System.Drawing.Size(44, 13);
            this.labelHour.TabIndex = 4;
            this.labelHour.Text = "At hour:";
            // 
            // textBoxMinute
            // 
            this.textBoxMinute._RawText = "0";
            this.textBoxMinute.Location = new System.Drawing.Point(80, 50);
            this.textBoxMinute.MaximumValue = ((long)(59));
            this.textBoxMinute.MinimumValue = ((long)(0));
            this.textBoxMinute.Name = "textBoxMinute";
            this.textBoxMinute.NumericValue = 0;
            this.textBoxMinute.Size = new System.Drawing.Size(82, 20);
            this.textBoxMinute.TabIndex = 3;
            this.textBoxMinute.Text = "0";
            // 
            // labelMinute
            // 
            this.labelMinute.AutoSize = true;
            this.labelMinute.Location = new System.Drawing.Point(19, 53);
            this.labelMinute.Name = "labelMinute";
            this.labelMinute.Size = new System.Drawing.Size(54, 13);
            this.labelMinute.TabIndex = 2;
            this.labelMinute.Text = "At minute:";
            // 
            // textBoxSecond
            // 
            this.textBoxSecond._RawText = "0";
            this.textBoxSecond.Location = new System.Drawing.Point(80, 13);
            this.textBoxSecond.MaximumValue = ((long)(59));
            this.textBoxSecond.MinimumValue = ((long)(0));
            this.textBoxSecond.Name = "textBoxSecond";
            this.textBoxSecond.NumericValue = 0;
            this.textBoxSecond.Size = new System.Drawing.Size(82, 20);
            this.textBoxSecond.TabIndex = 1;
            this.textBoxSecond.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "At second:";
            // 
            // groupBoxWeekLy
            // 
            this.groupBoxWeekLy.Controls.Add(this.label9);
            this.groupBoxWeekLy.Controls.Add(this.cbWeekly);
            this.groupBoxWeekLy.Location = new System.Drawing.Point(8, 8);
            this.groupBoxWeekLy.Name = "groupBoxWeekLy";
            this.groupBoxWeekLy.Size = new System.Drawing.Size(249, 54);
            this.groupBoxWeekLy.TabIndex = 13;
            this.groupBoxWeekLy.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(21, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Week day:";
            // 
            // cbWeekly
            // 
            this.cbWeekly.FormattingEnabled = true;
            this.cbWeekly.Items.AddRange(new object[] {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"});
            this.cbWeekly.Location = new System.Drawing.Point(80, 19);
            this.cbWeekly.Name = "cbWeekly";
            this.cbWeekly.Size = new System.Drawing.Size(121, 21);
            this.cbWeekly.TabIndex = 0;
            // 
            // groupBoxYear
            // 
            this.groupBoxYear.Controls.Add(this.textBoxDay);
            this.groupBoxYear.Controls.Add(this.label11);
            this.groupBoxYear.Controls.Add(this.textBoxMonth);
            this.groupBoxYear.Controls.Add(this.label10);
            this.groupBoxYear.Location = new System.Drawing.Point(8, 8);
            this.groupBoxYear.Name = "groupBoxYear";
            this.groupBoxYear.Size = new System.Drawing.Size(249, 54);
            this.groupBoxYear.TabIndex = 13;
            this.groupBoxYear.TabStop = false;
            // 
            // textBoxDay
            // 
            this.textBoxDay._RawText = "1";
            this.textBoxDay.Location = new System.Drawing.Point(155, 18);
            this.textBoxDay.MaximumValue = ((long)(31));
            this.textBoxDay.MinimumValue = ((long)(1));
            this.textBoxDay.Name = "textBoxDay";
            this.textBoxDay.NumericValue = 1;
            this.textBoxDay.Size = new System.Drawing.Size(61, 20);
            this.textBoxDay.TabIndex = 3;
            this.textBoxDay.Text = "1";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(120, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Day:";
            // 
            // textBoxMonth
            // 
            this.textBoxMonth._RawText = "1";
            this.textBoxMonth.Location = new System.Drawing.Point(62, 17);
            this.textBoxMonth.MaximumValue = ((long)(12));
            this.textBoxMonth.MinimumValue = ((long)(1));
            this.textBoxMonth.Name = "textBoxMonth";
            this.textBoxMonth.NumericValue = 1;
            this.textBoxMonth.Size = new System.Drawing.Size(32, 20);
            this.textBoxMonth.TabIndex = 1;
            this.textBoxMonth.Text = "1";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Month:";
            // 
            // groupBoxInterval
            // 
            this.groupBoxInterval.Controls.Add(this.labelIntervalUnit);
            this.groupBoxInterval.Controls.Add(this.textBoxInterval);
            this.groupBoxInterval.Controls.Add(this.labelInterval);
            this.groupBoxInterval.Location = new System.Drawing.Point(8, 8);
            this.groupBoxInterval.Name = "groupBoxInterval";
            this.groupBoxInterval.Size = new System.Drawing.Size(249, 54);
            this.groupBoxInterval.TabIndex = 12;
            this.groupBoxInterval.TabStop = false;
            // 
            // labelIntervalUnit
            // 
            this.labelIntervalUnit.AutoSize = true;
            this.labelIntervalUnit.Location = new System.Drawing.Point(170, 25);
            this.labelIntervalUnit.Name = "labelIntervalUnit";
            this.labelIntervalUnit.Size = new System.Drawing.Size(63, 13);
            this.labelIntervalUnit.TabIndex = 2;
            this.labelIntervalUnit.Text = "milliseconds";
            // 
            // textBoxInterval
            // 
            this.textBoxInterval._RawText = "";
            this.textBoxInterval.Location = new System.Drawing.Point(80, 19);
            this.textBoxInterval.MaximumValue = ((long)(0));
            this.textBoxInterval.MinimumValue = ((long)(0));
            this.textBoxInterval.Name = "textBoxInterval";
            this.textBoxInterval.NumericValue = 0;
            this.textBoxInterval.Size = new System.Drawing.Size(82, 20);
            this.textBoxInterval.TabIndex = 1;
            this.textBoxInterval.TargetType = FormComponents.EnumNumber.UInt32;
            this.textBoxInterval.Text = "0";
            // 
            // labelInterval
            // 
            this.labelInterval.AutoSize = true;
            this.labelInterval.Location = new System.Drawing.Point(19, 25);
            this.labelInterval.Name = "labelInterval";
            this.labelInterval.Size = new System.Drawing.Size(45, 13);
            this.labelInterval.TabIndex = 0;
            this.labelInterval.Text = "Interval:";
            // 
            // chkEnable
            // 
            this.chkEnable.AutoSize = true;
            this.chkEnable.Location = new System.Drawing.Point(201, 75);
            this.chkEnable.Name = "chkEnable";
            this.chkEnable.Size = new System.Drawing.Size(65, 17);
            this.chkEnable.TabIndex = 10;
            this.chkEnable.Text = "Enabled";
            this.chkEnable.UseVisualStyleBackColor = true;
            // 
            // textBoxMax
            // 
            this.textBoxMax._RawText = "";
            this.textBoxMax.Location = new System.Drawing.Point(140, 67);
            this.textBoxMax.MaximumValue = ((long)(0));
            this.textBoxMax.MinimumValue = ((long)(0));
            this.textBoxMax.Name = "textBoxMax";
            this.textBoxMax.NumericValue = 0;
            this.textBoxMax.Size = new System.Drawing.Size(32, 20);
            this.textBoxMax.TabIndex = 9;
            this.textBoxMax.TargetType = FormComponents.EnumNumber.UInt32;
            this.textBoxMax.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Maximum occurrence:";
            // 
            // cbType
            // 
            this.cbType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbType.FormattingEnabled = true;
            this.cbType.Items.AddRange(new object[] {
            "InMilliseconds",
            "InSeconds",
            "InMinutes",
            "InHours",
            "Daily",
            "Weekly",
            "Monthly",
            "Yearly",
            "SpecificTime"});
            this.cbType.Location = new System.Drawing.Point(110, 39);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(382, 21);
            this.cbType.TabIndex = 7;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Schedule type:";
            // 
            // textBoxName1
            // 
            this.textBoxName1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName1.Location = new System.Drawing.Point(109, 13);
            this.textBoxName1.Name = "textBoxName1";
            this.textBoxName1.Size = new System.Drawing.Size(383, 20);
            this.textBoxName1.TabIndex = 5;
            this.textBoxName1.Text = "schedule";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Schedule name:";
            // 
            // DialogSchedules
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 442);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.MinimizeBox = false;
            this.Name = "DialogSchedules";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Schedules";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.groupBoxSpecific.ResumeLayout(false);
            this.groupBoxSpecific.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBoxMinutes.ResumeLayout(false);
            this.groupBoxMinutes.PerformLayout();
            this.groupBoxWeekLy.ResumeLayout(false);
            this.groupBoxWeekLy.PerformLayout();
            this.groupBoxYear.ResumeLayout(false);
            this.groupBoxYear.PerformLayout();
            this.groupBoxInterval.ResumeLayout(false);
            this.groupBoxInterval.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxName1;
        private System.Windows.Forms.Label label1;
        private TextBoxNumber textBoxMax;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkEnable;
        private TextBoxNumber textBoxInterval;
        private System.Windows.Forms.Label labelInterval;
        private System.Windows.Forms.Label labelIntervalUnit;
        private System.Windows.Forms.GroupBox groupBoxMinutes;
        private TextBoxNumber textBoxSecond;
        private System.Windows.Forms.Label label6;
        private TextBoxNumber textBoxHour;
        private System.Windows.Forms.Label labelHour;
        private TextBoxNumber textBoxMinute;
        private System.Windows.Forms.Label labelMinute;
        private System.Windows.Forms.GroupBox groupBoxInterval;
        private System.Windows.Forms.GroupBox groupBoxWeekLy;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbWeekly;
        private System.Windows.Forms.GroupBox groupBoxYear;
        private TextBoxNumber textBoxDay;
        private System.Windows.Forms.Label label11;
        private TextBoxNumber textBoxMonth;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBoxSpecific;
        private TextBoxNumber textBoxSecondSpec;
        private System.Windows.Forms.Label label14;
        private TextBoxNumber textBoxMinuteSpec;
        private System.Windows.Forms.Label label13;
        private TextBoxNumber textBoxHourSpec;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonAssignActions;
        private System.Windows.Forms.TreeView treeView1;
    }
}