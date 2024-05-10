namespace UDT_Term_FFT
{
    partial class CanPCAN
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
            this.components = new System.ComponentModel.Container();
            this.chbShowPeriod = new System.Windows.Forms.CheckBox();
            this.rdbManual = new System.Windows.Forms.RadioButton();
            this.rdbEvent = new System.Windows.Forms.RadioButton();
            this.lstMessages = new System.Windows.Forms.ListView();
            this.clhType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhRcvTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhData = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnMsgClear = new System.Windows.Forms.Button();
            this.rdbTimer = new System.Windows.Forms.RadioButton();
            this.btnRead = new System.Windows.Forms.Button();
            this.tmrRead = new System.Windows.Forms.Timer(this.components);
            this.tmrDisplay = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbFolderName = new System.Windows.Forms.ToolStripTextBox();
            this.createFolderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.SaveLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportLogToMainUDTTermtsmi = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.aPIVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.heatBeatEmuTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.terminalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateRealTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.update1SecondToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.update2SecondToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.update5SecondToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.wordWrapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.dataASCIIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataHexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataDecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pCANDriverInstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pCANUDTCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pCANGeneralUseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pCAN1AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fdbPCANConfig = new System.Windows.Forms.FolderBrowserDialog();
            this.tcPCANViewer = new System.Windows.Forms.TabControl();
            this.tpPCANMessage = new System.Windows.Forms.TabPage();
            this.tbTabCycleTXWindow = new System.Windows.Forms.TabPage();
            this.cbCycleClearAll = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbCyclePause = new System.Windows.Forms.CheckBox();
            this.cbCycleRun = new System.Windows.Forms.CheckBox();
            this.cbCycleReset = new System.Windows.Forms.CheckBox();
            this.tpPCANLogs = new System.Windows.Forms.TabPage();
            this.cbAutoClearGreaterThan30K = new System.Windows.Forms.CheckBox();
            this.cbLogExcludeHeartBeat = new System.Windows.Forms.CheckBox();
            this.cbLogAutoScroll = new System.Windows.Forms.CheckBox();
            this.rtPCanTerm = new System.Windows.Forms.RichTextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnHalt = new System.Windows.Forms.Button();
            this.btnBusHeavy = new System.Windows.Forms.Button();
            this.btnBusLight = new System.Windows.Forms.Button();
            this.btnBusOFF = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPassive = new System.Windows.Forms.Button();
            this.bgwPCANTermUpdate = new System.ComponentModel.BackgroundWorker();
            this.TabMasterPCAN = new System.Windows.Forms.TabControl();
            this.TabTool1 = new System.Windows.Forms.TabPage();
            this.TabTool2 = new System.Windows.Forms.TabPage();
            this.TabTool3 = new System.Windows.Forms.TabPage();
            this.TabTool4 = new System.Windows.Forms.TabPage();
            this.TabTool5 = new System.Windows.Forms.TabPage();
            this.TabTool6 = new System.Windows.Forms.TabPage();
            this.TabTool7 = new System.Windows.Forms.TabPage();
            this.TabTool8 = new System.Windows.Forms.TabPage();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.tcHeadMaster = new TransparentTabControl();
            this.tp1HeartBeat = new System.Windows.Forms.TabPage();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.cbHBSuspend = new System.Windows.Forms.CheckBox();
            this.cbHBResetAll = new System.Windows.Forms.CheckBox();
            this.cbHBSpare1 = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnGEN_LoadCONFIGDump = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.tcPCANViewer.SuspendLayout();
            this.tpPCANMessage.SuspendLayout();
            this.tbTabCycleTXWindow.SuspendLayout();
            this.tpPCANLogs.SuspendLayout();
            this.TabMasterPCAN.SuspendLayout();
            this.tcHeadMaster.SuspendLayout();
            this.tp1HeartBeat.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chbShowPeriod
            // 
            this.chbShowPeriod.AutoSize = true;
            this.chbShowPeriod.Checked = true;
            this.chbShowPeriod.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbShowPeriod.Location = new System.Drawing.Point(395, 7);
            this.chbShowPeriod.Name = "chbShowPeriod";
            this.chbShowPeriod.Size = new System.Drawing.Size(123, 17);
            this.chbShowPeriod.TabIndex = 75;
            this.chbShowPeriod.Text = "Timestamp as period";
            this.chbShowPeriod.UseVisualStyleBackColor = true;
            this.chbShowPeriod.CheckedChanged += new System.EventHandler(this.chbShowPeriod_CheckedChanged);
            // 
            // rdbManual
            // 
            this.rdbManual.AutoSize = true;
            this.rdbManual.Location = new System.Drawing.Point(303, 6);
            this.rdbManual.Name = "rdbManual";
            this.rdbManual.Size = new System.Drawing.Size(89, 17);
            this.rdbManual.TabIndex = 74;
            this.rdbManual.Text = "Manual Read";
            this.rdbManual.UseVisualStyleBackColor = true;
            this.rdbManual.CheckedChanged += new System.EventHandler(this.rdbTimer_CheckedChanged);
            // 
            // rdbEvent
            // 
            this.rdbEvent.AutoSize = true;
            this.rdbEvent.Location = new System.Drawing.Point(187, 7);
            this.rdbEvent.Name = "rdbEvent";
            this.rdbEvent.Size = new System.Drawing.Size(113, 17);
            this.rdbEvent.TabIndex = 73;
            this.rdbEvent.Text = "Reading via Event";
            this.rdbEvent.UseVisualStyleBackColor = true;
            this.rdbEvent.CheckedChanged += new System.EventHandler(this.rdbTimer_CheckedChanged);
            // 
            // lstMessages
            // 
            this.lstMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhType,
            this.clhID,
            this.clhLength,
            this.clhCount,
            this.clhRcvTime,
            this.clhData});
            this.lstMessages.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstMessages.FullRowSelect = true;
            this.lstMessages.HideSelection = false;
            this.lstMessages.Location = new System.Drawing.Point(0, 28);
            this.lstMessages.MultiSelect = false;
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.Size = new System.Drawing.Size(657, 446);
            this.lstMessages.TabIndex = 28;
            this.lstMessages.UseCompatibleStateImageBehavior = false;
            this.lstMessages.View = System.Windows.Forms.View.Details;
            this.lstMessages.DoubleClick += new System.EventHandler(this.lstMessages_DoubleClick);
            // 
            // clhType
            // 
            this.clhType.Text = "Type";
            this.clhType.Width = 110;
            // 
            // clhID
            // 
            this.clhID.Text = "ID";
            this.clhID.Width = 50;
            // 
            // clhLength
            // 
            this.clhLength.Text = "Length";
            this.clhLength.Width = 50;
            // 
            // clhCount
            // 
            this.clhCount.Text = "Count";
            this.clhCount.Width = 49;
            // 
            // clhRcvTime
            // 
            this.clhRcvTime.Text = "Rcv Time";
            this.clhRcvTime.Width = 150;
            // 
            // clhData
            // 
            this.clhData.Text = "Data";
            this.clhData.Width = 300;
            // 
            // btnMsgClear
            // 
            this.btnMsgClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMsgClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnMsgClear.Location = new System.Drawing.Point(519, 3);
            this.btnMsgClear.Name = "btnMsgClear";
            this.btnMsgClear.Size = new System.Drawing.Size(65, 23);
            this.btnMsgClear.TabIndex = 50;
            this.btnMsgClear.Text = "Clear";
            this.btnMsgClear.UseVisualStyleBackColor = true;
            this.btnMsgClear.Click += new System.EventHandler(this.BtnMsgClear_Click);
            // 
            // rdbTimer
            // 
            this.rdbTimer.AutoSize = true;
            this.rdbTimer.Checked = true;
            this.rdbTimer.Location = new System.Drawing.Point(87, 6);
            this.rdbTimer.Name = "rdbTimer";
            this.rdbTimer.Size = new System.Drawing.Size(97, 17);
            this.rdbTimer.TabIndex = 72;
            this.rdbTimer.TabStop = true;
            this.rdbTimer.Text = "Read via Timer";
            this.rdbTimer.UseVisualStyleBackColor = true;
            this.rdbTimer.CheckedChanged += new System.EventHandler(this.rdbTimer_CheckedChanged);
            // 
            // btnRead
            // 
            this.btnRead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRead.Enabled = false;
            this.btnRead.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRead.Location = new System.Drawing.Point(590, 3);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(65, 23);
            this.btnRead.TabIndex = 49;
            this.btnRead.Text = "Read";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // tmrRead
            // 
            this.tmrRead.Interval = 50;
            this.tmrRead.Tick += new System.EventHandler(this.tmrRead_Tick);
            // 
            // tmrDisplay
            // 
            this.tmrDisplay.Tick += new System.EventHandler(this.tmrDisplay_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.terminalToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.pCAN1AToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1084, 24);
            this.menuStrip1.TabIndex = 52;
            this.menuStrip1.Text = "PCANViewer";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFolderToolStripMenuItem,
            this.saveConfigToolStripMenuItem,
            this.loadConfigToolStripMenuItem,
            this.toolStripSeparator5,
            this.SaveLogToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbFolderName,
            this.createFolderToolStripMenuItem1,
            this.openFolderToolStripMenuItem1});
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openFolderToolStripMenuItem.Text = "Manage Folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.OpenFolderToolStripMenuItem_Click);
            // 
            // tsbFolderName
            // 
            this.tsbFolderName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tsbFolderName.Name = "tsbFolderName";
            this.tsbFolderName.ReadOnly = true;
            this.tsbFolderName.Size = new System.Drawing.Size(250, 23);
            // 
            // createFolderToolStripMenuItem1
            // 
            this.createFolderToolStripMenuItem1.Name = "createFolderToolStripMenuItem1";
            this.createFolderToolStripMenuItem1.Size = new System.Drawing.Size(310, 22);
            this.createFolderToolStripMenuItem1.Text = "Create Folder";
            this.createFolderToolStripMenuItem1.Click += new System.EventHandler(this.CreateFolderToolStripMenuItem1_Click);
            // 
            // openFolderToolStripMenuItem1
            // 
            this.openFolderToolStripMenuItem1.Name = "openFolderToolStripMenuItem1";
            this.openFolderToolStripMenuItem1.Size = new System.Drawing.Size(310, 22);
            this.openFolderToolStripMenuItem1.Text = "Open Folder";
            this.openFolderToolStripMenuItem1.Click += new System.EventHandler(this.OpenFolderToolStripMenuItem1_Click);
            // 
            // saveConfigToolStripMenuItem
            // 
            this.saveConfigToolStripMenuItem.Name = "saveConfigToolStripMenuItem";
            this.saveConfigToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveConfigToolStripMenuItem.Text = "Save Config";
            this.saveConfigToolStripMenuItem.Click += new System.EventHandler(this.SaveConfigToolStripMenuItem_Click);
            // 
            // loadConfigToolStripMenuItem
            // 
            this.loadConfigToolStripMenuItem.Name = "loadConfigToolStripMenuItem";
            this.loadConfigToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.loadConfigToolStripMenuItem.Text = "Load Config";
            this.loadConfigToolStripMenuItem.Click += new System.EventHandler(this.LoadConfigToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(163, 6);
            // 
            // SaveLogToolStripMenuItem
            // 
            this.SaveLogToolStripMenuItem.Name = "SaveLogToolStripMenuItem";
            this.SaveLogToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.SaveLogToolStripMenuItem.Text = "Export Logs (TXT)";
            this.SaveLogToolStripMenuItem.Click += new System.EventHandler(this.SaveLogToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiExportLogToMainUDTTermtsmi,
            this.toolStripSeparator6,
            this.aPIVersionToolStripMenuItem,
            this.statusToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.toolStripSeparator1,
            this.heatBeatEmuTestToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // tsmiExportLogToMainUDTTermtsmi
            // 
            this.tsmiExportLogToMainUDTTermtsmi.Name = "tsmiExportLogToMainUDTTermtsmi";
            this.tsmiExportLogToMainUDTTermtsmi.Size = new System.Drawing.Size(226, 22);
            this.tsmiExportLogToMainUDTTermtsmi.Text = "Export Log To Main UDTTerm";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(223, 6);
            // 
            // aPIVersionToolStripMenuItem
            // 
            this.aPIVersionToolStripMenuItem.Name = "aPIVersionToolStripMenuItem";
            this.aPIVersionToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.aPIVersionToolStripMenuItem.Text = "Device/API Version";
            this.aPIVersionToolStripMenuItem.Click += new System.EventHandler(this.APIVersionToolStripMenuItem_Click);
            // 
            // statusToolStripMenuItem
            // 
            this.statusToolStripMenuItem.Name = "statusToolStripMenuItem";
            this.statusToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.statusToolStripMenuItem.Text = "Device Status";
            this.statusToolStripMenuItem.Click += new System.EventHandler(this.StatusToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.resetToolStripMenuItem.Text = "Device Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.ResetToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(223, 6);
            // 
            // heatBeatEmuTestToolStripMenuItem
            // 
            this.heatBeatEmuTestToolStripMenuItem.Name = "heatBeatEmuTestToolStripMenuItem";
            this.heatBeatEmuTestToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.heatBeatEmuTestToolStripMenuItem.Text = "HeatBeat_Emulation_Test";
            this.heatBeatEmuTestToolStripMenuItem.Click += new System.EventHandler(this.heatBeatEmuTestToolStripMenuItem_Click);
            // 
            // terminalToolStripMenuItem
            // 
            this.terminalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateRealTimeToolStripMenuItem,
            this.update1SecondToolStripMenuItem,
            this.update2SecondToolStripMenuItem,
            this.update5SecondToolStripMenuItem,
            this.toolStripSeparator2,
            this.wordWrapToolStripMenuItem,
            this.toolStripSeparator3,
            this.dataASCIIToolStripMenuItem,
            this.dataHexToolStripMenuItem,
            this.dataDecToolStripMenuItem,
            this.toolStripSeparator4});
            this.terminalToolStripMenuItem.Name = "terminalToolStripMenuItem";
            this.terminalToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.terminalToolStripMenuItem.Text = "Logs Term";
            // 
            // updateRealTimeToolStripMenuItem
            // 
            this.updateRealTimeToolStripMenuItem.Checked = true;
            this.updateRealTimeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateRealTimeToolStripMenuItem.Name = "updateRealTimeToolStripMenuItem";
            this.updateRealTimeToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.updateRealTimeToolStripMenuItem.Text = "Update: Real Time";
            this.updateRealTimeToolStripMenuItem.Click += new System.EventHandler(this.UpdateRealTimeToolStripMenuItem_Click);
            // 
            // update1SecondToolStripMenuItem
            // 
            this.update1SecondToolStripMenuItem.Name = "update1SecondToolStripMenuItem";
            this.update1SecondToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.update1SecondToolStripMenuItem.Text = "Update: 1 second";
            this.update1SecondToolStripMenuItem.Click += new System.EventHandler(this.Update1SecondToolStripMenuItem_Click);
            // 
            // update2SecondToolStripMenuItem
            // 
            this.update2SecondToolStripMenuItem.Name = "update2SecondToolStripMenuItem";
            this.update2SecondToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.update2SecondToolStripMenuItem.Text = "Update: 2 second";
            this.update2SecondToolStripMenuItem.Click += new System.EventHandler(this.Update2SecondToolStripMenuItem_Click);
            // 
            // update5SecondToolStripMenuItem
            // 
            this.update5SecondToolStripMenuItem.Name = "update5SecondToolStripMenuItem";
            this.update5SecondToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.update5SecondToolStripMenuItem.Text = "Update: 5 second";
            this.update5SecondToolStripMenuItem.Click += new System.EventHandler(this.Update5SecondToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(166, 6);
            // 
            // wordWrapToolStripMenuItem
            // 
            this.wordWrapToolStripMenuItem.Name = "wordWrapToolStripMenuItem";
            this.wordWrapToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.wordWrapToolStripMenuItem.Text = "Word Wrap";
            this.wordWrapToolStripMenuItem.Click += new System.EventHandler(this.WordWrapToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(166, 6);
            // 
            // dataASCIIToolStripMenuItem
            // 
            this.dataASCIIToolStripMenuItem.Name = "dataASCIIToolStripMenuItem";
            this.dataASCIIToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.dataASCIIToolStripMenuItem.Text = "Data:ASCII";
            // 
            // dataHexToolStripMenuItem
            // 
            this.dataHexToolStripMenuItem.Name = "dataHexToolStripMenuItem";
            this.dataHexToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.dataHexToolStripMenuItem.Text = "Data:Hex";
            // 
            // dataDecToolStripMenuItem
            // 
            this.dataDecToolStripMenuItem.Name = "dataDecToolStripMenuItem";
            this.dataDecToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.dataDecToolStripMenuItem.Text = "Data:Dec";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(166, 6);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pCANDriverInstallToolStripMenuItem,
            this.pCANUDTCommandToolStripMenuItem,
            this.pCANGeneralUseToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // pCANDriverInstallToolStripMenuItem
            // 
            this.pCANDriverInstallToolStripMenuItem.Name = "pCANDriverInstallToolStripMenuItem";
            this.pCANDriverInstallToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.pCANDriverInstallToolStripMenuItem.Text = "PCAN Driver Install";
            this.pCANDriverInstallToolStripMenuItem.Click += new System.EventHandler(this.PCANDriverInstallToolStripMenuItem_Click);
            // 
            // pCANUDTCommandToolStripMenuItem
            // 
            this.pCANUDTCommandToolStripMenuItem.Name = "pCANUDTCommandToolStripMenuItem";
            this.pCANUDTCommandToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.pCANUDTCommandToolStripMenuItem.Text = "PCAN UDT Command";
            this.pCANUDTCommandToolStripMenuItem.Click += new System.EventHandler(this.PCANUDTCommandToolStripMenuItem_Click);
            // 
            // pCANGeneralUseToolStripMenuItem
            // 
            this.pCANGeneralUseToolStripMenuItem.Name = "pCANGeneralUseToolStripMenuItem";
            this.pCANGeneralUseToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.pCANGeneralUseToolStripMenuItem.Text = "PCAN_GeneralUse";
            this.pCANGeneralUseToolStripMenuItem.Click += new System.EventHandler(this.pCANGeneralUseToolStripMenuItem_Click);
            // 
            // pCAN1AToolStripMenuItem
            // 
            this.pCAN1AToolStripMenuItem.Name = "pCAN1AToolStripMenuItem";
            this.pCAN1AToolStripMenuItem.Size = new System.Drawing.Size(132, 20);
            this.pCAN1AToolStripMenuItem.Text = "Rev: PCAN 1A 200819";
            // 
            // tcPCANViewer
            // 
            this.tcPCANViewer.Controls.Add(this.tpPCANMessage);
            this.tcPCANViewer.Controls.Add(this.tbTabCycleTXWindow);
            this.tcPCANViewer.Controls.Add(this.tpPCANLogs);
            this.tcPCANViewer.Location = new System.Drawing.Point(12, 126);
            this.tcPCANViewer.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.tcPCANViewer.Name = "tcPCANViewer";
            this.tcPCANViewer.SelectedIndex = 0;
            this.tcPCANViewer.Size = new System.Drawing.Size(669, 510);
            this.tcPCANViewer.TabIndex = 53;
            // 
            // tpPCANMessage
            // 
            this.tpPCANMessage.Controls.Add(this.chbShowPeriod);
            this.tpPCANMessage.Controls.Add(this.lstMessages);
            this.tpPCANMessage.Controls.Add(this.rdbManual);
            this.tpPCANMessage.Controls.Add(this.rdbTimer);
            this.tpPCANMessage.Controls.Add(this.btnRead);
            this.tpPCANMessage.Controls.Add(this.btnMsgClear);
            this.tpPCANMessage.Controls.Add(this.rdbEvent);
            this.tpPCANMessage.Location = new System.Drawing.Point(4, 22);
            this.tpPCANMessage.Name = "tpPCANMessage";
            this.tpPCANMessage.Padding = new System.Windows.Forms.Padding(3);
            this.tpPCANMessage.Size = new System.Drawing.Size(661, 484);
            this.tpPCANMessage.TabIndex = 0;
            this.tpPCANMessage.Text = "RX/RCV";
            this.tpPCANMessage.UseVisualStyleBackColor = true;
            // 
            // tbTabCycleTXWindow
            // 
            this.tbTabCycleTXWindow.Controls.Add(this.cbCycleClearAll);
            this.tbTabCycleTXWindow.Controls.Add(this.label2);
            this.tbTabCycleTXWindow.Controls.Add(this.cbCyclePause);
            this.tbTabCycleTXWindow.Controls.Add(this.cbCycleRun);
            this.tbTabCycleTXWindow.Controls.Add(this.cbCycleReset);
            this.tbTabCycleTXWindow.Font = new System.Drawing.Font("Monospac821 BT", 8.25F);
            this.tbTabCycleTXWindow.Location = new System.Drawing.Point(4, 22);
            this.tbTabCycleTXWindow.Name = "tbTabCycleTXWindow";
            this.tbTabCycleTXWindow.Size = new System.Drawing.Size(661, 484);
            this.tbTabCycleTXWindow.TabIndex = 2;
            this.tbTabCycleTXWindow.Text = "TX/Cycle";
            this.tbTabCycleTXWindow.UseVisualStyleBackColor = true;
            // 
            // cbCycleClearAll
            // 
            this.cbCycleClearAll.AutoSize = true;
            this.cbCycleClearAll.Location = new System.Drawing.Point(3, 4);
            this.cbCycleClearAll.Name = "cbCycleClearAll";
            this.cbCycleClearAll.Size = new System.Drawing.Size(89, 18);
            this.cbCycleClearAll.TabIndex = 69;
            this.cbCycleClearAll.Text = "Clear All";
            this.cbCycleClearAll.UseVisualStyleBackColor = true;
            this.cbCycleClearAll.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CbCycleClearAll_MouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(389, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 14);
            this.label2.TabIndex = 68;
            this.label2.Text = "Global Cycle:";
            // 
            // cbCyclePause
            // 
            this.cbCyclePause.AutoSize = true;
            this.cbCyclePause.Location = new System.Drawing.Point(543, 3);
            this.cbCyclePause.Name = "cbCyclePause";
            this.cbCyclePause.Size = new System.Drawing.Size(61, 18);
            this.cbCyclePause.TabIndex = 0;
            this.cbCyclePause.Text = "Pause";
            this.cbCyclePause.UseVisualStyleBackColor = true;
            this.cbCyclePause.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CbCycle_MouseClick);
            // 
            // cbCycleRun
            // 
            this.cbCycleRun.AutoSize = true;
            this.cbCycleRun.Location = new System.Drawing.Point(493, 3);
            this.cbCycleRun.Name = "cbCycleRun";
            this.cbCycleRun.Size = new System.Drawing.Size(47, 18);
            this.cbCycleRun.TabIndex = 66;
            this.cbCycleRun.Text = "Run";
            this.cbCycleRun.UseVisualStyleBackColor = true;
            this.cbCycleRun.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CbCycle_MouseClick);
            // 
            // cbCycleReset
            // 
            this.cbCycleReset.AutoSize = true;
            this.cbCycleReset.Location = new System.Drawing.Point(604, 3);
            this.cbCycleReset.Name = "cbCycleReset";
            this.cbCycleReset.Size = new System.Drawing.Size(61, 18);
            this.cbCycleReset.TabIndex = 67;
            this.cbCycleReset.Text = "Reset";
            this.cbCycleReset.UseVisualStyleBackColor = true;
            this.cbCycleReset.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CbCycle_MouseClick);
            // 
            // tpPCANLogs
            // 
            this.tpPCANLogs.Controls.Add(this.cbAutoClearGreaterThan30K);
            this.tpPCANLogs.Controls.Add(this.cbLogExcludeHeartBeat);
            this.tpPCANLogs.Controls.Add(this.cbLogAutoScroll);
            this.tpPCANLogs.Controls.Add(this.rtPCanTerm);
            this.tpPCANLogs.Controls.Add(this.btnClear);
            this.tpPCANLogs.Controls.Add(this.btnHalt);
            this.tpPCANLogs.Font = new System.Drawing.Font("Monospac821 BT", 8.25F);
            this.tpPCANLogs.Location = new System.Drawing.Point(4, 22);
            this.tpPCANLogs.Name = "tpPCANLogs";
            this.tpPCANLogs.Padding = new System.Windows.Forms.Padding(3);
            this.tpPCANLogs.Size = new System.Drawing.Size(661, 484);
            this.tpPCANLogs.TabIndex = 1;
            this.tpPCANLogs.Text = "RX/TX Log";
            this.tpPCANLogs.UseVisualStyleBackColor = true;
            // 
            // cbAutoClearGreaterThan30K
            // 
            this.cbAutoClearGreaterThan30K.AutoSize = true;
            this.cbAutoClearGreaterThan30K.Checked = true;
            this.cbAutoClearGreaterThan30K.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoClearGreaterThan30K.Location = new System.Drawing.Point(250, 6);
            this.cbAutoClearGreaterThan30K.Name = "cbAutoClearGreaterThan30K";
            this.cbAutoClearGreaterThan30K.Size = new System.Drawing.Size(152, 18);
            this.cbAutoClearGreaterThan30K.TabIndex = 58;
            this.cbAutoClearGreaterThan30K.Text = "Auto Clear if >30K";
            this.cbAutoClearGreaterThan30K.UseVisualStyleBackColor = true;
            // 
            // cbLogExcludeHeartBeat
            // 
            this.cbLogExcludeHeartBeat.AutoSize = true;
            this.cbLogExcludeHeartBeat.Location = new System.Drawing.Point(108, 6);
            this.cbLogExcludeHeartBeat.Name = "cbLogExcludeHeartBeat";
            this.cbLogExcludeHeartBeat.Size = new System.Drawing.Size(145, 18);
            this.cbLogExcludeHeartBeat.TabIndex = 57;
            this.cbLogExcludeHeartBeat.Text = "Exclude HeartBeat";
            this.cbLogExcludeHeartBeat.UseVisualStyleBackColor = true;
            // 
            // cbLogAutoScroll
            // 
            this.cbLogAutoScroll.AutoSize = true;
            this.cbLogAutoScroll.Checked = true;
            this.cbLogAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLogAutoScroll.Location = new System.Drawing.Point(6, 6);
            this.cbLogAutoScroll.Name = "cbLogAutoScroll";
            this.cbLogAutoScroll.Size = new System.Drawing.Size(96, 18);
            this.cbLogAutoScroll.TabIndex = 56;
            this.cbLogAutoScroll.Text = "AutoScroll";
            this.cbLogAutoScroll.UseVisualStyleBackColor = true;
            // 
            // rtPCanTerm
            // 
            this.rtPCanTerm.BackColor = System.Drawing.Color.DarkSlateGray;
            this.rtPCanTerm.DetectUrls = false;
            this.rtPCanTerm.Font = new System.Drawing.Font("Monospac821 BT", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtPCanTerm.ForeColor = System.Drawing.Color.LightYellow;
            this.rtPCanTerm.Location = new System.Drawing.Point(0, 27);
            this.rtPCanTerm.MaximumSize = new System.Drawing.Size(660, 460);
            this.rtPCanTerm.MinimumSize = new System.Drawing.Size(660, 460);
            this.rtPCanTerm.Name = "rtPCanTerm";
            this.rtPCanTerm.ReadOnly = true;
            this.rtPCanTerm.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rtPCanTerm.Size = new System.Drawing.Size(660, 460);
            this.rtPCanTerm.TabIndex = 16;
            this.rtPCanTerm.Text = " ";
            this.rtPCanTerm.WordWrap = false;
            this.rtPCanTerm.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RtPCanTerm_KeyDown);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(536, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(57, 21);
            this.btnClear.TabIndex = 54;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // btnHalt
            // 
            this.btnHalt.Location = new System.Drawing.Point(599, 3);
            this.btnHalt.Name = "btnHalt";
            this.btnHalt.Size = new System.Drawing.Size(56, 21);
            this.btnHalt.TabIndex = 55;
            this.btnHalt.Text = "Halt";
            this.btnHalt.UseVisualStyleBackColor = true;
            this.btnHalt.Click += new System.EventHandler(this.BtnHalt_Click);
            // 
            // btnBusHeavy
            // 
            this.btnBusHeavy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBusHeavy.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBusHeavy.Location = new System.Drawing.Point(548, 122);
            this.btnBusHeavy.Name = "btnBusHeavy";
            this.btnBusHeavy.Size = new System.Drawing.Size(65, 23);
            this.btnBusHeavy.TabIndex = 56;
            this.btnBusHeavy.Text = "Bus Heavy";
            this.btnBusHeavy.UseVisualStyleBackColor = true;
            // 
            // btnBusLight
            // 
            this.btnBusLight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBusLight.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBusLight.Location = new System.Drawing.Point(482, 122);
            this.btnBusLight.Name = "btnBusLight";
            this.btnBusLight.Size = new System.Drawing.Size(65, 23);
            this.btnBusLight.TabIndex = 57;
            this.btnBusLight.Text = "Bus Light";
            this.btnBusLight.UseVisualStyleBackColor = true;
            // 
            // btnBusOFF
            // 
            this.btnBusOFF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBusOFF.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBusOFF.Location = new System.Drawing.Point(613, 122);
            this.btnBusOFF.Name = "btnBusOFF";
            this.btnBusOFF.Size = new System.Drawing.Size(65, 23);
            this.btnBusOFF.TabIndex = 58;
            this.btnBusOFF.Text = "Bus OFF";
            this.btnBusOFF.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
            this.label1.Location = new System.Drawing.Point(340, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 16);
            this.label1.TabIndex = 59;
            this.label1.Text = "Bus Status";
            // 
            // btnPassive
            // 
            this.btnPassive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPassive.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnPassive.Location = new System.Drawing.Point(415, 122);
            this.btnPassive.Name = "btnPassive";
            this.btnPassive.Size = new System.Drawing.Size(65, 23);
            this.btnPassive.TabIndex = 60;
            this.btnPassive.Text = "Bus Passive";
            this.btnPassive.UseVisualStyleBackColor = true;
            // 
            // bgwPCANTermUpdate
            // 
            this.bgwPCANTermUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPCANTermUpdate_DoWork);
            this.bgwPCANTermUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPCANTermUpdate_RunWorkerCompleted);
            // 
            // TabMasterPCAN
            // 
            this.TabMasterPCAN.Controls.Add(this.TabTool1);
            this.TabMasterPCAN.Controls.Add(this.TabTool2);
            this.TabMasterPCAN.Controls.Add(this.TabTool3);
            this.TabMasterPCAN.Controls.Add(this.TabTool4);
            this.TabMasterPCAN.Controls.Add(this.TabTool5);
            this.TabMasterPCAN.Controls.Add(this.TabTool6);
            this.TabMasterPCAN.Controls.Add(this.TabTool7);
            this.TabMasterPCAN.Controls.Add(this.TabTool8);
            this.TabMasterPCAN.Location = new System.Drawing.Point(687, 122);
            this.TabMasterPCAN.MaximumSize = new System.Drawing.Size(386, 470);
            this.TabMasterPCAN.MinimumSize = new System.Drawing.Size(286, 470);
            this.TabMasterPCAN.Name = "TabMasterPCAN";
            this.TabMasterPCAN.SelectedIndex = 0;
            this.TabMasterPCAN.Size = new System.Drawing.Size(386, 470);
            this.TabMasterPCAN.TabIndex = 61;
            // 
            // TabTool1
            // 
            this.TabTool1.Location = new System.Drawing.Point(4, 22);
            this.TabTool1.Name = "TabTool1";
            this.TabTool1.Padding = new System.Windows.Forms.Padding(3);
            this.TabTool1.Size = new System.Drawing.Size(378, 444);
            this.TabTool1.TabIndex = 0;
            this.TabTool1.Text = "Tool1";
            this.TabTool1.UseVisualStyleBackColor = true;
            // 
            // TabTool2
            // 
            this.TabTool2.Location = new System.Drawing.Point(4, 22);
            this.TabTool2.Name = "TabTool2";
            this.TabTool2.Padding = new System.Windows.Forms.Padding(3);
            this.TabTool2.Size = new System.Drawing.Size(378, 444);
            this.TabTool2.TabIndex = 1;
            this.TabTool2.Text = "Tool2";
            this.TabTool2.UseVisualStyleBackColor = true;
            // 
            // TabTool3
            // 
            this.TabTool3.Location = new System.Drawing.Point(4, 22);
            this.TabTool3.Name = "TabTool3";
            this.TabTool3.Padding = new System.Windows.Forms.Padding(3);
            this.TabTool3.Size = new System.Drawing.Size(378, 444);
            this.TabTool3.TabIndex = 2;
            this.TabTool3.Text = "Tool3";
            this.TabTool3.UseVisualStyleBackColor = true;
            // 
            // TabTool4
            // 
            this.TabTool4.Location = new System.Drawing.Point(4, 22);
            this.TabTool4.Name = "TabTool4";
            this.TabTool4.Padding = new System.Windows.Forms.Padding(3);
            this.TabTool4.Size = new System.Drawing.Size(378, 444);
            this.TabTool4.TabIndex = 3;
            this.TabTool4.Text = "Tool4";
            this.TabTool4.UseVisualStyleBackColor = true;
            // 
            // TabTool5
            // 
            this.TabTool5.Location = new System.Drawing.Point(4, 22);
            this.TabTool5.Name = "TabTool5";
            this.TabTool5.Size = new System.Drawing.Size(378, 444);
            this.TabTool5.TabIndex = 4;
            this.TabTool5.Text = "Tool5";
            this.TabTool5.UseVisualStyleBackColor = true;
            // 
            // TabTool6
            // 
            this.TabTool6.Location = new System.Drawing.Point(4, 22);
            this.TabTool6.Name = "TabTool6";
            this.TabTool6.Size = new System.Drawing.Size(378, 444);
            this.TabTool6.TabIndex = 5;
            this.TabTool6.Text = "Tool6";
            this.TabTool6.UseVisualStyleBackColor = true;
            // 
            // TabTool7
            // 
            this.TabTool7.Location = new System.Drawing.Point(4, 22);
            this.TabTool7.Name = "TabTool7";
            this.TabTool7.Size = new System.Drawing.Size(378, 444);
            this.TabTool7.TabIndex = 6;
            this.TabTool7.Text = "Tool7";
            this.TabTool7.UseVisualStyleBackColor = true;
            // 
            // TabTool8
            // 
            this.TabTool8.Location = new System.Drawing.Point(4, 22);
            this.TabTool8.Name = "TabTool8";
            this.TabTool8.Size = new System.Drawing.Size(378, 444);
            this.TabTool8.TabIndex = 7;
            this.TabTool8.Text = "Tool8";
            this.TabTool8.UseVisualStyleBackColor = true;
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(994, 605);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 21);
            this.btnImport.TabIndex = 63;
            this.btnImport.Text = "Load List";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(913, 605);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 21);
            this.btnExport.TabIndex = 62;
            this.btnExport.Text = "Save List";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // tcHeadMaster
            // 
            this.tcHeadMaster.Controls.Add(this.tp1HeartBeat);
            this.tcHeadMaster.Controls.Add(this.tabPage2);
            this.tcHeadMaster.Controls.Add(this.tabPage3);
            this.tcHeadMaster.Controls.Add(this.tabPage4);
            this.tcHeadMaster.Location = new System.Drawing.Point(12, 22);
            this.tcHeadMaster.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.tcHeadMaster.Name = "tcHeadMaster";
            this.tcHeadMaster.SelectedIndex = 0;
            this.tcHeadMaster.Size = new System.Drawing.Size(835, 95);
            this.tcHeadMaster.TabIndex = 64;
            // 
            // tp1HeartBeat
            // 
            this.tp1HeartBeat.Controls.Add(this.checkBox4);
            this.tp1HeartBeat.Controls.Add(this.cbHBSuspend);
            this.tp1HeartBeat.Controls.Add(this.cbHBResetAll);
            this.tp1HeartBeat.Controls.Add(this.cbHBSpare1);
            this.tp1HeartBeat.Location = new System.Drawing.Point(4, 22);
            this.tp1HeartBeat.Name = "tp1HeartBeat";
            this.tp1HeartBeat.Padding = new System.Windows.Forms.Padding(3);
            this.tp1HeartBeat.Size = new System.Drawing.Size(827, 69);
            this.tp1HeartBeat.TabIndex = 0;
            this.tp1HeartBeat.Text = "HeartBeat";
            this.tp1HeartBeat.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(751, 52);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(60, 17);
            this.checkBox4.TabIndex = 3;
            this.checkBox4.Text = "Spare2";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // cbHBSuspend
            // 
            this.cbHBSuspend.AutoSize = true;
            this.cbHBSuspend.Location = new System.Drawing.Point(751, 20);
            this.cbHBSuspend.Name = "cbHBSuspend";
            this.cbHBSuspend.Size = new System.Drawing.Size(68, 17);
            this.cbHBSuspend.TabIndex = 2;
            this.cbHBSuspend.Text = "Suspend";
            this.cbHBSuspend.UseVisualStyleBackColor = true;
            this.cbHBSuspend.CheckedChanged += new System.EventHandler(this.CbHBSuspend_CheckedChanged);
            // 
            // cbHBResetAll
            // 
            this.cbHBResetAll.AutoSize = true;
            this.cbHBResetAll.Location = new System.Drawing.Point(751, 4);
            this.cbHBResetAll.Name = "cbHBResetAll";
            this.cbHBResetAll.Size = new System.Drawing.Size(68, 17);
            this.cbHBResetAll.TabIndex = 1;
            this.cbHBResetAll.Text = "Reset All";
            this.cbHBResetAll.UseVisualStyleBackColor = true;
            this.cbHBResetAll.CheckedChanged += new System.EventHandler(this.CbHBResetAll_CheckedChanged);
            // 
            // cbHBSpare1
            // 
            this.cbHBSpare1.AutoSize = true;
            this.cbHBSpare1.Location = new System.Drawing.Point(751, 36);
            this.cbHBSpare1.Name = "cbHBSpare1";
            this.cbHBSpare1.Size = new System.Drawing.Size(60, 17);
            this.cbHBSpare1.TabIndex = 0;
            this.cbHBSpare1.Text = "Spare1";
            this.cbHBSpare1.UseVisualStyleBackColor = true;
            this.cbHBSpare1.CheckedChanged += new System.EventHandler(this.CbHBSpare1_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnGEN_LoadCONFIGDump);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(827, 69);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Generator";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(827, 69);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(827, 69);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btnGEN_LoadCONFIGDump
            // 
            this.btnGEN_LoadCONFIGDump.Location = new System.Drawing.Point(6, 6);
            this.btnGEN_LoadCONFIGDump.Name = "btnGEN_LoadCONFIGDump";
            this.btnGEN_LoadCONFIGDump.Size = new System.Drawing.Size(75, 57);
            this.btnGEN_LoadCONFIGDump.TabIndex = 5;
            this.btnGEN_LoadCONFIGDump.Text = "Read/View CONFIG";
            this.btnGEN_LoadCONFIGDump.UseVisualStyleBackColor = true;
            this.btnGEN_LoadCONFIGDump.Click += new System.EventHandler(this.btnGEN_LoadCONFIGDump_Click_1);
            // 
            // CanPCAN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1084, 638);
            this.Controls.Add(this.tcHeadMaster);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.TabMasterPCAN);
            this.Controls.Add(this.btnPassive);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBusOFF);
            this.Controls.Add(this.btnBusLight);
            this.Controls.Add(this.btnBusHeavy);
            this.Controls.Add(this.tcPCANViewer);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1100, 677);
            this.MinimumSize = new System.Drawing.Size(1100, 677);
            this.Name = "CanPCAN";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UDT PCAN Terminal";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CanPCAN_FormClosing);
            this.Load += new System.EventHandler(this.CanPCAN_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tcPCANViewer.ResumeLayout(false);
            this.tpPCANMessage.ResumeLayout(false);
            this.tpPCANMessage.PerformLayout();
            this.tbTabCycleTXWindow.ResumeLayout(false);
            this.tbTabCycleTXWindow.PerformLayout();
            this.tpPCANLogs.ResumeLayout(false);
            this.tpPCANLogs.PerformLayout();
            this.TabMasterPCAN.ResumeLayout(false);
            this.tcHeadMaster.ResumeLayout(false);
            this.tp1HeartBeat.ResumeLayout(false);
            this.tp1HeartBeat.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RadioButton rdbEvent;
        private System.Windows.Forms.ListView lstMessages;
        private System.Windows.Forms.ColumnHeader clhType;
        private System.Windows.Forms.ColumnHeader clhID;
        private System.Windows.Forms.ColumnHeader clhLength;
        private System.Windows.Forms.ColumnHeader clhData;
        private System.Windows.Forms.ColumnHeader clhCount;
        private System.Windows.Forms.ColumnHeader clhRcvTime;
        private System.Windows.Forms.RadioButton rdbTimer;
        private System.Windows.Forms.RadioButton rdbManual;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.Button btnMsgClear;
        private System.Windows.Forms.CheckBox chbShowPeriod;
        private System.Windows.Forms.Timer tmrRead;
        private System.Windows.Forms.Timer tmrDisplay;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pCAN1AToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox tsbFolderName;
        private System.Windows.Forms.ToolStripMenuItem createFolderToolStripMenuItem1;
        private System.Windows.Forms.FolderBrowserDialog fdbPCANConfig;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem1;
        private System.Windows.Forms.TabControl tcPCANViewer;
        private System.Windows.Forms.TabPage tpPCANMessage;
        private System.Windows.Forms.TabPage tpPCANLogs;
        public System.Windows.Forms.RichTextBox rtPCanTerm;
        private System.Windows.Forms.Button btnHalt;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnBusHeavy;
        private System.Windows.Forms.Button btnBusLight;
        private System.Windows.Forms.Button btnBusOFF;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPassive;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportLogToMainUDTTermtsmi;
        private System.ComponentModel.BackgroundWorker bgwPCANTermUpdate;
        private System.Windows.Forms.ToolStripMenuItem terminalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateRealTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem update1SecondToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem update2SecondToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem update5SecondToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem wordWrapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aPIVersionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem dataASCIIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataHexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataDecToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem SaveLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.TabControl TabMasterPCAN;
        private System.Windows.Forms.TabPage TabTool1;
        private System.Windows.Forms.TabPage TabTool2;
        private System.Windows.Forms.TabPage TabTool3;
        private System.Windows.Forms.TabPage TabTool4;
        private System.Windows.Forms.TabPage TabTool5;
        private System.Windows.Forms.TabPage TabTool6;
        private System.Windows.Forms.TabPage TabTool7;
        private System.Windows.Forms.TabPage TabTool8;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ToolStripMenuItem pCANDriverInstallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pCANUDTCommandToolStripMenuItem;
        private TransparentTabControl tcHeadMaster;
        private System.Windows.Forms.TabPage tp1HeartBeat;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tbTabCycleTXWindow;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbCyclePause;
        private System.Windows.Forms.CheckBox cbCycleRun;
        private System.Windows.Forms.CheckBox cbCycleReset;
        private System.Windows.Forms.CheckBox cbCycleClearAll;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox cbHBSuspend;
        private System.Windows.Forms.CheckBox cbHBResetAll;
        private System.Windows.Forms.CheckBox cbHBSpare1;
        private System.Windows.Forms.CheckBox cbLogAutoScroll;
        private System.Windows.Forms.CheckBox cbLogExcludeHeartBeat;
        private System.Windows.Forms.CheckBox cbAutoClearGreaterThan30K;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem heatBeatEmuTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pCANGeneralUseToolStripMenuItem;
        private System.Windows.Forms.Button btnGEN_LoadCONFIGDump;
    }
}

