namespace UDT_Term_FFT
{
    partial class LoggerCSV
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
            System.Windows.Forms.MenuStrip menuStrip1;
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setWaitloopTo1000ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setWaitloopTo100ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCountdownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCountdown5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.enableAppendOneFilename = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.enableAsyncWithGDERToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.quickFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dDataFrameLoggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cADTDataFrameLoggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cADTDataFrameLoggerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cADTDataFrameLoggerToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cADTDataFrameLoggerToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.cADTDataFrameLoggerToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.cADTDataFrameLoggerToolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboveAlsoCreateADTSessionLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_OptionSync = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_OptionAsync = new System.Windows.Forms.ToolStripMenuItem();
            this.CbApplySetGetTime = new System.Windows.Forms.ToolStripMenuItem();
            this.rawDataignoreFormatFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmsi_FormattedCVStoFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_SplitDataToTwoColumn = new System.Windows.Forms.ToolStripMenuItem();
            this.rFDPPDHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instructionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tFormatFrameHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvMessage = new System.Windows.Forms.DataGridView();
            this.rtbConsole = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnLogCVSInstruction = new System.Windows.Forms.Button();
            this.tbCRCPassed = new System.Windows.Forms.TextBox();
            this.BtnOpenFolder = new System.Windows.Forms.Button();
            this.tbCRCFailed = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.txtFolderName = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.bgwk2 = new System.ComponentModel.BackgroundWorker();
            this.bgwk_DataWaitLoop = new System.ComponentModel.BackgroundWorker();
            this.bgwk_HeaderWaitLoop = new System.ComponentModel.BackgroundWorker();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rtbAppendBox = new System.Windows.Forms.RichTextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.tpLoggerCVS = new TransparentTabControl();
            this.tpLoggerSync = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txStopCounter = new System.Windows.Forms.TextBox();
            this.btnDataFrame = new System.Windows.Forms.Button();
            this.btnTestHeader = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnPause = new System.Windows.Forms.Button();
            this.txtCountDown = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCyclePeriod = new System.Windows.Forms.TextBox();
            this.tpLoggerAsync = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbSTCSTARTMode = new System.Windows.Forms.ComboBox();
            this.cbSendToLogMem = new System.Windows.Forms.CheckBox();
            this.cbCallBackMode = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tbAsyncPeroid = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbAlertTimer = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbSendtoClientCode = new System.Windows.Forms.CheckBox();
            this.btnAsyncHelp = new System.Windows.Forms.Button();
            this.btnSTCStart = new System.Windows.Forms.Button();
            this.btnAsyncSuspendUpdate = new System.Windows.Forms.Button();
            this.btnSTCStop = new System.Windows.Forms.Button();
            this.tpLoggerAsyncSnad = new System.Windows.Forms.TabPage();
            this.cbSendtoClientCodeSnad = new System.Windows.Forms.CheckBox();
            this.btnAsyncHelpSnad = new System.Windows.Forms.Button();
            this.btnAsyncSuspendUpdateSnad = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.tbAlertTimerSnad = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnSTCStartSnad = new System.Windows.Forms.Button();
            this.btnSTCStopSnad = new System.Windows.Forms.Button();
            this.tpCommand = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btnSTCERASE_Block = new System.Windows.Forms.Button();
            this.btnSTCERASE_Verify = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.btnSTCERASE_Bulk = new System.Windows.Forms.Button();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessage)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tpLoggerCVS.SuspendLayout();
            this.tpLoggerSync.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tpLoggerAsync.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tpLoggerAsyncSnad.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tpCommand.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = System.Drawing.Color.Transparent;
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem,
            this.quickFolderToolStripMenuItem,
            this.tsm_OptionSync,
            this.tsm_OptionAsync,
            this.aboutToolStripMenuItem});
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(1114, 24);
            menuStrip1.TabIndex = 16;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setWaitloopTo1000ToolStripMenuItem,
            this.setWaitloopTo100ToolStripMenuItem,
            this.resetCountdownToolStripMenuItem,
            this.resetCountdown5ToolStripMenuItem,
            this.toolStripSeparator3,
            this.enableAppendOneFilename,
            this.toolStripSeparator1,
            this.enableAsyncWithGDERToolStripMenuItem,
            this.toolStripSeparator2});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // setWaitloopTo1000ToolStripMenuItem
            // 
            this.setWaitloopTo1000ToolStripMenuItem.Checked = true;
            this.setWaitloopTo1000ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.setWaitloopTo1000ToolStripMenuItem.Name = "setWaitloopTo1000ToolStripMenuItem";
            this.setWaitloopTo1000ToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.setWaitloopTo1000ToolStripMenuItem.Text = "Set Waitloop to 100";
            this.setWaitloopTo1000ToolStripMenuItem.Click += new System.EventHandler(this.setWaitloopTo1000ToolStripMenuItem_Click);
            // 
            // setWaitloopTo100ToolStripMenuItem
            // 
            this.setWaitloopTo100ToolStripMenuItem.Name = "setWaitloopTo100ToolStripMenuItem";
            this.setWaitloopTo100ToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.setWaitloopTo100ToolStripMenuItem.Text = "Set Waitloop to 200";
            this.setWaitloopTo100ToolStripMenuItem.Click += new System.EventHandler(this.setWaitloopTo100ToolStripMenuItem_Click);
            // 
            // resetCountdownToolStripMenuItem
            // 
            this.resetCountdownToolStripMenuItem.Name = "resetCountdownToolStripMenuItem";
            this.resetCountdownToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.resetCountdownToolStripMenuItem.Text = "Reset Countdown";
            this.resetCountdownToolStripMenuItem.Click += new System.EventHandler(this.resetCountdownToolStripMenuItem_Click);
            // 
            // resetCountdown5ToolStripMenuItem
            // 
            this.resetCountdown5ToolStripMenuItem.Name = "resetCountdown5ToolStripMenuItem";
            this.resetCountdown5ToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.resetCountdown5ToolStripMenuItem.Text = "Reset Countdown 5 Sec";
            this.resetCountdown5ToolStripMenuItem.Click += new System.EventHandler(this.resetCountdown5ToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(232, 6);
            // 
            // enableAppendOneFilename
            // 
            this.enableAppendOneFilename.Name = "enableAppendOneFilename";
            this.enableAppendOneFilename.Size = new System.Drawing.Size(235, 22);
            this.enableAppendOneFilename.Text = "Enable Append One Filename";
            this.enableAppendOneFilename.Click += new System.EventHandler(this.enableAppendOneFilename_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(232, 6);
            // 
            // enableAsyncWithGDERToolStripMenuItem
            // 
            this.enableAsyncWithGDERToolStripMenuItem.Name = "enableAsyncWithGDERToolStripMenuItem";
            this.enableAsyncWithGDERToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.enableAsyncWithGDERToolStripMenuItem.Text = "Enable Async with $G,$D,$E,$R";
            this.enableAsyncWithGDERToolStripMenuItem.Click += new System.EventHandler(this.enableAsyncWithGDERToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(232, 6);
            // 
            // quickFolderToolStripMenuItem
            // 
            this.quickFolderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dDataFrameLoggerToolStripMenuItem,
            this.cADTDataFrameLoggerToolStripMenuItem,
            this.cADTDataFrameLoggerToolStripMenuItem1,
            this.cADTDataFrameLoggerToolStripMenuItem2,
            this.cADTDataFrameLoggerToolStripMenuItem3,
            this.cADTDataFrameLoggerToolStripMenuItem4,
            this.cADTDataFrameLoggerToolStripMenuItem5,
            this.aboveAlsoCreateADTSessionLogToolStripMenuItem});
            this.quickFolderToolStripMenuItem.Name = "quickFolderToolStripMenuItem";
            this.quickFolderToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.quickFolderToolStripMenuItem.Text = "Quick Folder";
            // 
            // dDataFrameLoggerToolStripMenuItem
            // 
            this.dDataFrameLoggerToolStripMenuItem.Name = "dDataFrameLoggerToolStripMenuItem";
            this.dDataFrameLoggerToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.dDataFrameLoggerToolStripMenuItem.Text = "D:\\\\ADTDataFrameLogger";
            this.dDataFrameLoggerToolStripMenuItem.Click += new System.EventHandler(this.dDataFrameLoggerToolStripMenuItem_Click);
            // 
            // cADTDataFrameLoggerToolStripMenuItem
            // 
            this.cADTDataFrameLoggerToolStripMenuItem.Name = "cADTDataFrameLoggerToolStripMenuItem";
            this.cADTDataFrameLoggerToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.cADTDataFrameLoggerToolStripMenuItem.Text = "E:\\\\ADTDataFrameLogger";
            this.cADTDataFrameLoggerToolStripMenuItem.Click += new System.EventHandler(this.cADTDataFrameLoggerToolStripMenuItem_Click);
            // 
            // cADTDataFrameLoggerToolStripMenuItem1
            // 
            this.cADTDataFrameLoggerToolStripMenuItem1.Name = "cADTDataFrameLoggerToolStripMenuItem1";
            this.cADTDataFrameLoggerToolStripMenuItem1.Size = new System.Drawing.Size(255, 22);
            this.cADTDataFrameLoggerToolStripMenuItem1.Text = "F:\\\\ADTDataFrameLogger";
            this.cADTDataFrameLoggerToolStripMenuItem1.Click += new System.EventHandler(this.cADTDataFrameLoggerToolStripMenuItem1_Click);
            // 
            // cADTDataFrameLoggerToolStripMenuItem2
            // 
            this.cADTDataFrameLoggerToolStripMenuItem2.Name = "cADTDataFrameLoggerToolStripMenuItem2";
            this.cADTDataFrameLoggerToolStripMenuItem2.Size = new System.Drawing.Size(255, 22);
            this.cADTDataFrameLoggerToolStripMenuItem2.Text = "G:\\\\ADTDataFrameLogger";
            this.cADTDataFrameLoggerToolStripMenuItem2.Click += new System.EventHandler(this.cADTDataFrameLoggerToolStripMenuItem2_Click);
            // 
            // cADTDataFrameLoggerToolStripMenuItem3
            // 
            this.cADTDataFrameLoggerToolStripMenuItem3.Name = "cADTDataFrameLoggerToolStripMenuItem3";
            this.cADTDataFrameLoggerToolStripMenuItem3.Size = new System.Drawing.Size(255, 22);
            this.cADTDataFrameLoggerToolStripMenuItem3.Text = "H:\\\\ADTDataFrameLogger";
            this.cADTDataFrameLoggerToolStripMenuItem3.Click += new System.EventHandler(this.cADTDataFrameLoggerToolStripMenuItem3_Click);
            // 
            // cADTDataFrameLoggerToolStripMenuItem4
            // 
            this.cADTDataFrameLoggerToolStripMenuItem4.Name = "cADTDataFrameLoggerToolStripMenuItem4";
            this.cADTDataFrameLoggerToolStripMenuItem4.Size = new System.Drawing.Size(255, 22);
            this.cADTDataFrameLoggerToolStripMenuItem4.Text = "I:\\\\ADTDataFrameLogger";
            this.cADTDataFrameLoggerToolStripMenuItem4.Click += new System.EventHandler(this.cADTDataFrameLoggerToolStripMenuItem4_Click);
            // 
            // cADTDataFrameLoggerToolStripMenuItem5
            // 
            this.cADTDataFrameLoggerToolStripMenuItem5.Name = "cADTDataFrameLoggerToolStripMenuItem5";
            this.cADTDataFrameLoggerToolStripMenuItem5.Size = new System.Drawing.Size(255, 22);
            this.cADTDataFrameLoggerToolStripMenuItem5.Text = "J:\\\\ADTDataFrameLogger";
            this.cADTDataFrameLoggerToolStripMenuItem5.Click += new System.EventHandler(this.cADTDataFrameLoggerToolStripMenuItem5_Click);
            // 
            // aboveAlsoCreateADTSessionLogToolStripMenuItem
            // 
            this.aboveAlsoCreateADTSessionLogToolStripMenuItem.Enabled = false;
            this.aboveAlsoCreateADTSessionLogToolStripMenuItem.Name = "aboveAlsoCreateADTSessionLogToolStripMenuItem";
            this.aboveAlsoCreateADTSessionLogToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.aboveAlsoCreateADTSessionLogToolStripMenuItem.Text = "Above also update ADTSessionLog";
            // 
            // tsm_OptionSync
            // 
            this.tsm_OptionSync.Name = "tsm_OptionSync";
            this.tsm_OptionSync.Size = new System.Drawing.Size(84, 20);
            this.tsm_OptionSync.Text = "Option Sync";
            // 
            // tsm_OptionAsync
            // 
            this.tsm_OptionAsync.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CbApplySetGetTime,
            this.rawDataignoreFormatFrameToolStripMenuItem,
            this.tmsi_FormattedCVStoFile,
            this.tsmi_SplitDataToTwoColumn,
            this.rFDPPDHelpToolStripMenuItem});
            this.tsm_OptionAsync.Name = "tsm_OptionAsync";
            this.tsm_OptionAsync.Size = new System.Drawing.Size(92, 20);
            this.tsm_OptionAsync.Text = "Option ASync";
            // 
            // CbApplySetGetTime
            // 
            this.CbApplySetGetTime.Name = "CbApplySetGetTime";
            this.CbApplySetGetTime.Size = new System.Drawing.Size(260, 22);
            this.CbApplySetGetTime.Text = "Apply GET/SET Time";
            // 
            // rawDataignoreFormatFrameToolStripMenuItem
            // 
            this.rawDataignoreFormatFrameToolStripMenuItem.Name = "rawDataignoreFormatFrameToolStripMenuItem";
            this.rawDataignoreFormatFrameToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.rawDataignoreFormatFrameToolStripMenuItem.Text = "Raw Data (ignore format frame)";
            this.rawDataignoreFormatFrameToolStripMenuItem.Click += new System.EventHandler(this.rawDataignoreFormatFrameToolStripMenuItem_Click);
            // 
            // tmsi_FormattedCVStoFile
            // 
            this.tmsi_FormattedCVStoFile.Checked = true;
            this.tmsi_FormattedCVStoFile.CheckOnClick = true;
            this.tmsi_FormattedCVStoFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tmsi_FormattedCVStoFile.Name = "tmsi_FormattedCVStoFile";
            this.tmsi_FormattedCVStoFile.Size = new System.Drawing.Size(260, 22);
            this.tmsi_FormattedCVStoFile.Text = "Formatted Data to CVS file";
            // 
            // tsmi_SplitDataToTwoColumn
            // 
            this.tsmi_SplitDataToTwoColumn.CheckOnClick = true;
            this.tsmi_SplitDataToTwoColumn.Name = "tsmi_SplitDataToTwoColumn";
            this.tsmi_SplitDataToTwoColumn.Size = new System.Drawing.Size(260, 22);
            this.tsmi_SplitDataToTwoColumn.Text = "RFDDAQ-CH3, Split to two Column";
            // 
            // rFDPPDHelpToolStripMenuItem
            // 
            this.rFDPPDHelpToolStripMenuItem.Name = "rFDPPDHelpToolStripMenuItem";
            this.rFDPPDHelpToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.rFDPPDHelpToolStripMenuItem.Text = "RFDPPD_Help";
            this.rFDPPDHelpToolStripMenuItem.Click += new System.EventHandler(this.rFDPPDHelpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.instructionToolStripMenuItem,
            this.tFormatFrameHelpToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // instructionToolStripMenuItem
            // 
            this.instructionToolStripMenuItem.Name = "instructionToolStripMenuItem";
            this.instructionToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.instructionToolStripMenuItem.Text = "Instruction";
            this.instructionToolStripMenuItem.Click += new System.EventHandler(this.instructionToolStripMenuItem_Click);
            // 
            // tFormatFrameHelpToolStripMenuItem
            // 
            this.tFormatFrameHelpToolStripMenuItem.Name = "tFormatFrameHelpToolStripMenuItem";
            this.tFormatFrameHelpToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.tFormatFrameHelpToolStripMenuItem.Text = "$T FormatFrame Help";
            this.tFormatFrameHelpToolStripMenuItem.Click += new System.EventHandler(this.tFormatFrameHelpToolStripMenuItem_Click);
            // 
            // dgvMessage
            // 
            this.dgvMessage.AllowUserToAddRows = false;
            this.dgvMessage.AllowUserToDeleteRows = false;
            this.dgvMessage.AllowUserToResizeColumns = false;
            this.dgvMessage.AllowUserToResizeRows = false;
            this.dgvMessage.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvMessage.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dgvMessage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMessage.EnableHeadersVisualStyles = false;
            this.dgvMessage.Location = new System.Drawing.Point(732, 30);
            this.dgvMessage.MaximumSize = new System.Drawing.Size(370, 690);
            this.dgvMessage.MinimumSize = new System.Drawing.Size(370, 720);
            this.dgvMessage.MultiSelect = false;
            this.dgvMessage.Name = "dgvMessage";
            this.dgvMessage.ReadOnly = true;
            this.dgvMessage.RowHeadersVisible = false;
            this.dgvMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvMessage.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvMessage.ShowCellErrors = false;
            this.dgvMessage.ShowCellToolTips = false;
            this.dgvMessage.ShowEditingIcon = false;
            this.dgvMessage.ShowRowErrors = false;
            this.dgvMessage.Size = new System.Drawing.Size(370, 720);
            this.dgvMessage.TabIndex = 5;
            // 
            // rtbConsole
            // 
            this.rtbConsole.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.rtbConsole.BackColor = System.Drawing.Color.DarkSlateGray;
            this.rtbConsole.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbConsole.ForeColor = System.Drawing.Color.Yellow;
            this.rtbConsole.Location = new System.Drawing.Point(12, 329);
            this.rtbConsole.MaximumSize = new System.Drawing.Size(714, 420);
            this.rtbConsole.MinimumSize = new System.Drawing.Size(714, 420);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.ReadOnly = true;
            this.rtbConsole.Size = new System.Drawing.Size(714, 420);
            this.rtbConsole.TabIndex = 12;
            this.rtbConsole.Text = "";
            this.rtbConsole.TextChanged += new System.EventHandler(this.rtbConsole_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.btnLogCVSInstruction);
            this.groupBox3.Controls.Add(this.tbCRCPassed);
            this.groupBox3.Controls.Add(this.BtnOpenFolder);
            this.groupBox3.Controls.Add(this.tbCRCFailed);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtFilename);
            this.groupBox3.Controls.Add(this.btnFolder);
            this.groupBox3.Controls.Add(this.txtFolderName);
            this.groupBox3.Location = new System.Drawing.Point(12, 30);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(714, 71);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Filename";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(577, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 13);
            this.label10.TabIndex = 42;
            this.label10.Text = "CRC Failed";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(571, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 41;
            this.label9.Text = "CRC Passed";
            // 
            // btnLogCVSInstruction
            // 
            this.btnLogCVSInstruction.Location = new System.Drawing.Point(9, 44);
            this.btnLogCVSInstruction.Name = "btnLogCVSInstruction";
            this.btnLogCVSInstruction.Size = new System.Drawing.Size(75, 23);
            this.btnLogCVSInstruction.TabIndex = 13;
            this.btnLogCVSInstruction.Text = "Instruction";
            this.btnLogCVSInstruction.UseVisualStyleBackColor = true;
            this.btnLogCVSInstruction.Click += new System.EventHandler(this.btnLogCVSInstruction_Click);
            // 
            // tbCRCPassed
            // 
            this.tbCRCPassed.BackColor = System.Drawing.SystemColors.Window;
            this.tbCRCPassed.Location = new System.Drawing.Point(641, 20);
            this.tbCRCPassed.Name = "tbCRCPassed";
            this.tbCRCPassed.Size = new System.Drawing.Size(62, 20);
            this.tbCRCPassed.TabIndex = 40;
            // 
            // BtnOpenFolder
            // 
            this.BtnOpenFolder.Location = new System.Drawing.Point(9, 17);
            this.BtnOpenFolder.Name = "BtnOpenFolder";
            this.BtnOpenFolder.Size = new System.Drawing.Size(75, 22);
            this.BtnOpenFolder.TabIndex = 4;
            this.BtnOpenFolder.Text = "Open";
            this.BtnOpenFolder.UseVisualStyleBackColor = true;
            this.BtnOpenFolder.Click += new System.EventHandler(this.BtnOpenFolder_Click);
            // 
            // tbCRCFailed
            // 
            this.tbCRCFailed.BackColor = System.Drawing.SystemColors.Window;
            this.tbCRCFailed.Location = new System.Drawing.Point(641, 43);
            this.tbCRCFailed.Name = "tbCRCFailed";
            this.tbCRCFailed.Size = new System.Drawing.Size(63, 20);
            this.tbCRCFailed.TabIndex = 39;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(97, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Current FileName";
            // 
            // txtFilename
            // 
            this.txtFilename.Location = new System.Drawing.Point(185, 44);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.ReadOnly = true;
            this.txtFilename.Size = new System.Drawing.Size(380, 20);
            this.txtFilename.TabIndex = 2;
            // 
            // btnFolder
            // 
            this.btnFolder.Location = new System.Drawing.Point(100, 17);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(75, 22);
            this.btnFolder.TabIndex = 1;
            this.btnFolder.Text = "Folder";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // txtFolderName
            // 
            this.txtFolderName.Location = new System.Drawing.Point(185, 20);
            this.txtFolderName.Name = "txtFolderName";
            this.txtFolderName.ReadOnly = true;
            this.txtFolderName.Size = new System.Drawing.Size(380, 20);
            this.txtFolderName.TabIndex = 0;
            // 
            // bgwk2
            // 
            this.bgwk2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwk2_DoWork);
            this.bgwk2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwk2_RunWorkerCompleted);
            // 
            // bgwk_HeaderWaitLoop
            // 
            this.bgwk_HeaderWaitLoop.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwk_HeaderWaitLoop_DoWork);
            this.bgwk_HeaderWaitLoop.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwk_HeaderWaitLoop_RunWorkerCompleted);
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.Transparent;
            this.groupBox5.Controls.Add(this.rtbAppendBox);
            this.groupBox5.Location = new System.Drawing.Point(12, 251);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(714, 78);
            this.groupBox5.TabIndex = 18;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Append Notes";
            // 
            // rtbAppendBox
            // 
            this.rtbAppendBox.BackColor = System.Drawing.Color.DarkSlateGray;
            this.rtbAppendBox.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbAppendBox.ForeColor = System.Drawing.SystemColors.Info;
            this.rtbAppendBox.Location = new System.Drawing.Point(9, 18);
            this.rtbAppendBox.Name = "rtbAppendBox";
            this.rtbAppendBox.Size = new System.Drawing.Size(695, 54);
            this.rtbAppendBox.TabIndex = 14;
            this.rtbAppendBox.Text = "Comment here and this append to your csv file. Insert Text and press enter to app" +
    "end.\nIt add $A along with date/time stamp at the start of the text, so it can be" +
    " sorted/filtered out within Excel.";
            this.rtbAppendBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.rtbAppendBox_KeyPress);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Transparent;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Location = new System.Drawing.Point(15, 125);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(710, 1);
            this.panel5.TabIndex = 38;
            // 
            // tpLoggerCVS
            // 
            this.tpLoggerCVS.Controls.Add(this.tpLoggerSync);
            this.tpLoggerCVS.Controls.Add(this.tpLoggerAsync);
            this.tpLoggerCVS.Controls.Add(this.tpLoggerAsyncSnad);
            this.tpLoggerCVS.Controls.Add(this.tpCommand);
            this.tpLoggerCVS.Location = new System.Drawing.Point(12, 103);
            this.tpLoggerCVS.Name = "tpLoggerCVS";
            this.tpLoggerCVS.SelectedIndex = 0;
            this.tpLoggerCVS.Size = new System.Drawing.Size(714, 142);
            this.tpLoggerCVS.TabIndex = 21;
            this.tpLoggerCVS.SelectedIndexChanged += new System.EventHandler(this.tpLoggerCVS_SelectedIndexChanged);
            // 
            // tpLoggerSync
            // 
            this.tpLoggerSync.BackColor = System.Drawing.SystemColors.Control;
            this.tpLoggerSync.Controls.Add(this.groupBox2);
            this.tpLoggerSync.Location = new System.Drawing.Point(4, 22);
            this.tpLoggerSync.Name = "tpLoggerSync";
            this.tpLoggerSync.Padding = new System.Windows.Forms.Padding(3);
            this.tpLoggerSync.Size = new System.Drawing.Size(706, 116);
            this.tpLoggerSync.TabIndex = 0;
            this.tpLoggerSync.Text = "Sync Mode (Timer)";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Snow;
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txStopCounter);
            this.groupBox2.Controls.Add(this.btnDataFrame);
            this.groupBox2.Controls.Add(this.btnTestHeader);
            this.groupBox2.Controls.Add(this.btnStart);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.btnPause);
            this.groupBox2.Controls.Add(this.txtCountDown);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtCyclePeriod);
            this.groupBox2.Location = new System.Drawing.Point(-3, -7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(714, 127);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(249, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "(-1 =Disabled)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(250, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Stop Counter";
            // 
            // txStopCounter
            // 
            this.txStopCounter.Location = new System.Drawing.Point(325, 13);
            this.txStopCounter.Name = "txStopCounter";
            this.txStopCounter.Size = new System.Drawing.Size(52, 20);
            this.txStopCounter.TabIndex = 13;
            this.txStopCounter.Text = "-1";
            // 
            // btnDataFrame
            // 
            this.btnDataFrame.Location = new System.Drawing.Point(625, 41);
            this.btnDataFrame.Name = "btnDataFrame";
            this.btnDataFrame.Size = new System.Drawing.Size(75, 21);
            this.btnDataFrame.TabIndex = 4;
            this.btnDataFrame.Text = "Data";
            this.btnDataFrame.UseVisualStyleBackColor = true;
            this.btnDataFrame.Click += new System.EventHandler(this.btnDataFrame_Click);
            // 
            // btnTestHeader
            // 
            this.btnTestHeader.Location = new System.Drawing.Point(625, 14);
            this.btnTestHeader.Name = "btnTestHeader";
            this.btnTestHeader.Size = new System.Drawing.Size(75, 21);
            this.btnTestHeader.TabIndex = 3;
            this.btnTestHeader.Text = "Header";
            this.btnTestHeader.UseVisualStyleBackColor = true;
            this.btnTestHeader.Click += new System.EventHandler(this.btnTestHeader_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(10, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 21);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(96, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Countdown";
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(10, 39);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 21);
            this.btnPause.TabIndex = 1;
            this.btnPause.Text = "Cease Loop";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // txtCountDown
            // 
            this.txtCountDown.Location = new System.Drawing.Point(163, 41);
            this.txtCountDown.Name = "txtCountDown";
            this.txtCountDown.ReadOnly = true;
            this.txtCountDown.Size = new System.Drawing.Size(81, 20);
            this.txtCountDown.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(95, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Period(Sec)";
            // 
            // txtCyclePeriod
            // 
            this.txtCyclePeriod.Location = new System.Drawing.Point(163, 13);
            this.txtCyclePeriod.Name = "txtCyclePeriod";
            this.txtCyclePeriod.Size = new System.Drawing.Size(81, 20);
            this.txtCyclePeriod.TabIndex = 11;
            // 
            // tpLoggerAsync
            // 
            this.tpLoggerAsync.BackColor = System.Drawing.SystemColors.Control;
            this.tpLoggerAsync.Controls.Add(this.groupBox1);
            this.tpLoggerAsync.Location = new System.Drawing.Point(4, 22);
            this.tpLoggerAsync.Name = "tpLoggerAsync";
            this.tpLoggerAsync.Padding = new System.Windows.Forms.Padding(3);
            this.tpLoggerAsync.Size = new System.Drawing.Size(706, 116);
            this.tpLoggerAsync.TabIndex = 1;
            this.tpLoggerAsync.Text = "ASync Mode";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Controls.Add(this.cbSTCSTARTMode);
            this.groupBox1.Controls.Add(this.cbSendToLogMem);
            this.groupBox1.Controls.Add(this.cbCallBackMode);
            this.groupBox1.Controls.Add(this.groupBox6);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.cbSendtoClientCode);
            this.groupBox1.Controls.Add(this.btnAsyncHelp);
            this.groupBox1.Controls.Add(this.btnSTCStart);
            this.groupBox1.Controls.Add(this.btnAsyncSuspendUpdate);
            this.groupBox1.Controls.Add(this.btnSTCStop);
            this.groupBox1.Location = new System.Drawing.Point(-4, -7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(714, 123);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // cbSTCSTARTMode
            // 
            this.cbSTCSTARTMode.FormattingEnabled = true;
            this.cbSTCSTARTMode.Items.AddRange(new object[] {
            "0 = Generic Field App",
            "1 = As Field App, no-LogMem.",
            "2 = LoggerCVS (Default)",
            "3 = Reserved",
            "4 = ESM Manual Start/Stop",
            "5 = ESM Debug#1. SeqTest.",
            "6 = ESM Debug#2.",
            "7 = ESM Debug#3."});
            this.cbSTCSTARTMode.Location = new System.Drawing.Point(449, 15);
            this.cbSTCSTARTMode.Name = "cbSTCSTARTMode";
            this.cbSTCSTARTMode.Size = new System.Drawing.Size(258, 21);
            this.cbSTCSTARTMode.TabIndex = 27;
            this.cbSTCSTARTMode.SelectedIndexChanged += new System.EventHandler(this.cbSTCSTARTMode_SelectedIndexChanged);
            // 
            // cbSendToLogMem
            // 
            this.cbSendToLogMem.AutoSize = true;
            this.cbSendToLogMem.BackColor = System.Drawing.Color.Transparent;
            this.cbSendToLogMem.Location = new System.Drawing.Point(96, 43);
            this.cbSendToLogMem.Name = "cbSendToLogMem";
            this.cbSendToLogMem.Size = new System.Drawing.Size(207, 17);
            this.cbSendToLogMem.TabIndex = 4;
            this.cbSendToLogMem.Text = "Send to LogMem Window [Advanced]";
            this.cbSendToLogMem.UseVisualStyleBackColor = false;
            // 
            // cbCallBackMode
            // 
            this.cbCallBackMode.AutoSize = true;
            this.cbCallBackMode.BackColor = System.Drawing.Color.Transparent;
            this.cbCallBackMode.Checked = true;
            this.cbCallBackMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCallBackMode.Location = new System.Drawing.Point(639, 42);
            this.cbCallBackMode.Name = "cbCallBackMode";
            this.cbCallBackMode.Size = new System.Drawing.Size(68, 17);
            this.cbCallBackMode.TabIndex = 26;
            this.cbCallBackMode.Text = "CallBack";
            this.cbCallBackMode.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.cbCallBackMode.UseVisualStyleBackColor = false;
            this.cbCallBackMode.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cbCallBackMode_MouseClick);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tbAsyncPeroid);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Location = new System.Drawing.Point(444, 65);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(86, 52);
            this.groupBox6.TabIndex = 25;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Async Period";
            // 
            // tbAsyncPeroid
            // 
            this.tbAsyncPeroid.BackColor = System.Drawing.SystemColors.Window;
            this.tbAsyncPeroid.Location = new System.Drawing.Point(6, 19);
            this.tbAsyncPeroid.Name = "tbAsyncPeroid";
            this.tbAsyncPeroid.Size = new System.Drawing.Size(42, 20);
            this.tbAsyncPeroid.TabIndex = 7;
            this.tbAsyncPeroid.Text = "10";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(54, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Sec";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbAlertTimer);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Location = new System.Drawing.Point(536, 65);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(167, 52);
            this.groupBox4.TabIndex = 24;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Timer Alert (No Data Expire)";
            // 
            // tbAlertTimer
            // 
            this.tbAlertTimer.BackColor = System.Drawing.SystemColors.Window;
            this.tbAlertTimer.Location = new System.Drawing.Point(6, 19);
            this.tbAlertTimer.Name = "tbAlertTimer";
            this.tbAlertTimer.Size = new System.Drawing.Size(63, 20);
            this.tbAlertTimer.TabIndex = 7;
            this.tbAlertTimer.Text = "-1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(77, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Sec (-1 Disabled)";
            // 
            // cbSendtoClientCode
            // 
            this.cbSendtoClientCode.AutoSize = true;
            this.cbSendtoClientCode.BackColor = System.Drawing.Color.Transparent;
            this.cbSendtoClientCode.Location = new System.Drawing.Point(96, 18);
            this.cbSendtoClientCode.Name = "cbSendtoClientCode";
            this.cbSendtoClientCode.Size = new System.Drawing.Size(202, 17);
            this.cbSendtoClientCode.TabIndex = 23;
            this.cbSendtoClientCode.Text = "Send to LogMem Viewer [ClientCode]";
            this.cbSendtoClientCode.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.cbSendtoClientCode.UseVisualStyleBackColor = false;
            // 
            // btnAsyncHelp
            // 
            this.btnAsyncHelp.Location = new System.Drawing.Point(10, 94);
            this.btnAsyncHelp.Name = "btnAsyncHelp";
            this.btnAsyncHelp.Size = new System.Drawing.Size(73, 23);
            this.btnAsyncHelp.TabIndex = 3;
            this.btnAsyncHelp.Text = "Help";
            this.btnAsyncHelp.UseVisualStyleBackColor = true;
            this.btnAsyncHelp.Click += new System.EventHandler(this.btnAsyncHelp_Click);
            // 
            // btnSTCStart
            // 
            this.btnSTCStart.Location = new System.Drawing.Point(10, 15);
            this.btnSTCStart.Name = "btnSTCStart";
            this.btnSTCStart.Size = new System.Drawing.Size(73, 21);
            this.btnSTCStart.TabIndex = 2;
            this.btnSTCStart.Text = "STC Start";
            this.btnSTCStart.UseVisualStyleBackColor = true;
            this.btnSTCStart.Click += new System.EventHandler(this.btnSTCStart_Click);
            // 
            // btnAsyncSuspendUpdate
            // 
            this.btnAsyncSuspendUpdate.Location = new System.Drawing.Point(10, 69);
            this.btnAsyncSuspendUpdate.Name = "btnAsyncSuspendUpdate";
            this.btnAsyncSuspendUpdate.Size = new System.Drawing.Size(73, 21);
            this.btnAsyncSuspendUpdate.TabIndex = 0;
            this.btnAsyncSuspendUpdate.Text = "Hold List";
            this.btnAsyncSuspendUpdate.UseVisualStyleBackColor = true;
            this.btnAsyncSuspendUpdate.Click += new System.EventHandler(this.btnAsyncSuspendUpdate_Click);
            // 
            // btnSTCStop
            // 
            this.btnSTCStop.Location = new System.Drawing.Point(10, 42);
            this.btnSTCStop.Name = "btnSTCStop";
            this.btnSTCStop.Size = new System.Drawing.Size(73, 21);
            this.btnSTCStop.TabIndex = 1;
            this.btnSTCStop.Text = "STC Stop";
            this.btnSTCStop.UseVisualStyleBackColor = true;
            this.btnSTCStop.Click += new System.EventHandler(this.btnSTCStop_Click);
            // 
            // tpLoggerAsyncSnad
            // 
            this.tpLoggerAsyncSnad.BackColor = System.Drawing.Color.GhostWhite;
            this.tpLoggerAsyncSnad.Controls.Add(this.cbSendtoClientCodeSnad);
            this.tpLoggerAsyncSnad.Controls.Add(this.btnAsyncHelpSnad);
            this.tpLoggerAsyncSnad.Controls.Add(this.btnAsyncSuspendUpdateSnad);
            this.tpLoggerAsyncSnad.Controls.Add(this.groupBox7);
            this.tpLoggerAsyncSnad.Controls.Add(this.btnSTCStartSnad);
            this.tpLoggerAsyncSnad.Controls.Add(this.btnSTCStopSnad);
            this.tpLoggerAsyncSnad.Location = new System.Drawing.Point(4, 22);
            this.tpLoggerAsyncSnad.Name = "tpLoggerAsyncSnad";
            this.tpLoggerAsyncSnad.Size = new System.Drawing.Size(706, 116);
            this.tpLoggerAsyncSnad.TabIndex = 2;
            this.tpLoggerAsyncSnad.Text = "Async SNAD Mode";
            // 
            // cbSendtoClientCodeSnad
            // 
            this.cbSendtoClientCodeSnad.AutoSize = true;
            this.cbSendtoClientCodeSnad.BackColor = System.Drawing.Color.Transparent;
            this.cbSendtoClientCodeSnad.Location = new System.Drawing.Point(96, 10);
            this.cbSendtoClientCodeSnad.Name = "cbSendtoClientCodeSnad";
            this.cbSendtoClientCodeSnad.Size = new System.Drawing.Size(119, 17);
            this.cbSendtoClientCodeSnad.TabIndex = 28;
            this.cbSendtoClientCodeSnad.Text = "Send to Log Viewer";
            this.cbSendtoClientCodeSnad.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.cbSendtoClientCodeSnad.UseVisualStyleBackColor = false;
            // 
            // btnAsyncHelpSnad
            // 
            this.btnAsyncHelpSnad.Location = new System.Drawing.Point(625, 34);
            this.btnAsyncHelpSnad.Name = "btnAsyncHelpSnad";
            this.btnAsyncHelpSnad.Size = new System.Drawing.Size(75, 23);
            this.btnAsyncHelpSnad.TabIndex = 27;
            this.btnAsyncHelpSnad.Text = "Help";
            this.btnAsyncHelpSnad.UseVisualStyleBackColor = true;
            this.btnAsyncHelpSnad.Click += new System.EventHandler(this.btnAsyncHelpSnad_Click);
            // 
            // btnAsyncSuspendUpdateSnad
            // 
            this.btnAsyncSuspendUpdateSnad.Location = new System.Drawing.Point(625, 7);
            this.btnAsyncSuspendUpdateSnad.Name = "btnAsyncSuspendUpdateSnad";
            this.btnAsyncSuspendUpdateSnad.Size = new System.Drawing.Size(75, 21);
            this.btnAsyncSuspendUpdateSnad.TabIndex = 26;
            this.btnAsyncSuspendUpdateSnad.Text = "Hold List";
            this.btnAsyncSuspendUpdateSnad.UseVisualStyleBackColor = true;
            this.btnAsyncSuspendUpdateSnad.Click += new System.EventHandler(this.btnAsyncSuspendUpdateSnad_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.tbAlertTimerSnad);
            this.groupBox7.Controls.Add(this.label8);
            this.groupBox7.Location = new System.Drawing.Point(437, 5);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(182, 52);
            this.groupBox7.TabIndex = 25;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Timer Alert (Expired with no data)";
            // 
            // tbAlertTimerSnad
            // 
            this.tbAlertTimerSnad.BackColor = System.Drawing.SystemColors.Window;
            this.tbAlertTimerSnad.Location = new System.Drawing.Point(6, 19);
            this.tbAlertTimerSnad.Name = "tbAlertTimerSnad";
            this.tbAlertTimerSnad.Size = new System.Drawing.Size(63, 20);
            this.tbAlertTimerSnad.TabIndex = 7;
            this.tbAlertTimerSnad.Text = "-1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(75, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Sec (-1 Disabled)";
            // 
            // btnSTCStartSnad
            // 
            this.btnSTCStartSnad.Location = new System.Drawing.Point(5, 7);
            this.btnSTCStartSnad.Name = "btnSTCStartSnad";
            this.btnSTCStartSnad.Size = new System.Drawing.Size(75, 21);
            this.btnSTCStartSnad.TabIndex = 4;
            this.btnSTCStartSnad.Text = "STC Start";
            this.btnSTCStartSnad.UseVisualStyleBackColor = true;
            this.btnSTCStartSnad.Click += new System.EventHandler(this.btnSTCStartSnad_Click);
            // 
            // btnSTCStopSnad
            // 
            this.btnSTCStopSnad.Location = new System.Drawing.Point(6, 34);
            this.btnSTCStopSnad.Name = "btnSTCStopSnad";
            this.btnSTCStopSnad.Size = new System.Drawing.Size(75, 21);
            this.btnSTCStopSnad.TabIndex = 3;
            this.btnSTCStopSnad.Text = "STC Stop";
            this.btnSTCStopSnad.UseVisualStyleBackColor = true;
            this.btnSTCStopSnad.Click += new System.EventHandler(this.btnSTCStopSnad_Click);
            // 
            // tpCommand
            // 
            this.tpCommand.BackColor = System.Drawing.Color.AliceBlue;
            this.tpCommand.Controls.Add(this.groupBox8);
            this.tpCommand.Location = new System.Drawing.Point(4, 22);
            this.tpCommand.Name = "tpCommand";
            this.tpCommand.Padding = new System.Windows.Forms.Padding(3);
            this.tpCommand.Size = new System.Drawing.Size(706, 116);
            this.tpCommand.TabIndex = 3;
            this.tpCommand.Text = "Util-Command";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.label13);
            this.groupBox8.Controls.Add(this.label11);
            this.groupBox8.Controls.Add(this.btnSTCERASE_Block);
            this.groupBox8.Controls.Add(this.btnSTCERASE_Verify);
            this.groupBox8.Controls.Add(this.label12);
            this.groupBox8.Controls.Add(this.btnSTCERASE_Bulk);
            this.groupBox8.Location = new System.Drawing.Point(6, 4);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(251, 54);
            this.groupBox8.TabIndex = 26;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "LogMem";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(168, 36);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(33, 13);
            this.label13.TabIndex = 19;
            this.label13.Text = "Verify";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(87, 36);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 13);
            this.label11.TabIndex = 17;
            this.label11.Text = "Bulk Erase";
            // 
            // btnSTCERASE_Block
            // 
            this.btnSTCERASE_Block.Location = new System.Drawing.Point(6, 14);
            this.btnSTCERASE_Block.Name = "btnSTCERASE_Block";
            this.btnSTCERASE_Block.Size = new System.Drawing.Size(75, 21);
            this.btnSTCERASE_Block.TabIndex = 5;
            this.btnSTCERASE_Block.Text = "STC Erase-1";
            this.btnSTCERASE_Block.UseVisualStyleBackColor = true;
            this.btnSTCERASE_Block.Click += new System.EventHandler(this.btnSTCERASE_Deep_Click);
            // 
            // btnSTCERASE_Verify
            // 
            this.btnSTCERASE_Verify.Location = new System.Drawing.Point(168, 14);
            this.btnSTCERASE_Verify.Name = "btnSTCERASE_Verify";
            this.btnSTCERASE_Verify.Size = new System.Drawing.Size(75, 21);
            this.btnSTCERASE_Verify.TabIndex = 7;
            this.btnSTCERASE_Verify.Text = "STC Erase-3";
            this.btnSTCERASE_Verify.UseVisualStyleBackColor = true;
            this.btnSTCERASE_Verify.Click += new System.EventHandler(this.btnSTCERASE_Verify_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 36);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "Block-Erase";
            // 
            // btnSTCERASE_Bulk
            // 
            this.btnSTCERASE_Bulk.Location = new System.Drawing.Point(87, 14);
            this.btnSTCERASE_Bulk.Name = "btnSTCERASE_Bulk";
            this.btnSTCERASE_Bulk.Size = new System.Drawing.Size(75, 21);
            this.btnSTCERASE_Bulk.TabIndex = 6;
            this.btnSTCERASE_Bulk.Text = "STC Erase-2";
            this.btnSTCERASE_Bulk.UseVisualStyleBackColor = true;
            this.btnSTCERASE_Bulk.Click += new System.EventHandler(this.btnSTCERASE_Fast_Click);
            // 
            // LoggerCSV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPageTVBBlank_1B;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1114, 761);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.tpLoggerCVS);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.rtbConsole);
            this.Controls.Add(this.dgvMessage);
            this.Controls.Add(menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = menuStrip1;
            this.MaximumSize = new System.Drawing.Size(1130, 800);
            this.MinimumSize = new System.Drawing.Size(1130, 800);
            this.Name = "LoggerCSV";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LoggerCSV";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoggerCSV_FormClosing);
            this.Load += new System.EventHandler(this.LoggerCSV_Load);
            this.Shown += new System.EventHandler(this.LoggerCSV_Shown);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessage)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.tpLoggerCVS.ResumeLayout(false);
            this.tpLoggerSync.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tpLoggerAsync.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tpLoggerAsyncSnad.ResumeLayout(false);
            this.tpLoggerAsyncSnad.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tpCommand.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMessage;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.TextBox txtFolderName;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCyclePeriod;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtCountDown;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.ComponentModel.BackgroundWorker bgwk2;
        private System.Windows.Forms.Button btnTestHeader;
        private System.Windows.Forms.Button btnDataFrame;
        private System.ComponentModel.BackgroundWorker bgwk_DataWaitLoop;
        private System.ComponentModel.BackgroundWorker bgwk_HeaderWaitLoop;
        private System.Windows.Forms.ToolStripMenuItem instructionToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RichTextBox rtbAppendBox;
        private System.Windows.Forms.Button BtnOpenFolder;
        private System.Windows.Forms.ToolStripMenuItem setWaitloopTo1000ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setWaitloopTo100ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetCountdownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetCountdown5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quickFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dDataFrameLoggerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cADTDataFrameLoggerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cADTDataFrameLoggerToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cADTDataFrameLoggerToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem cADTDataFrameLoggerToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem cADTDataFrameLoggerToolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem cADTDataFrameLoggerToolStripMenuItem5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem enableAppendOneFilename;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem enableAsyncWithGDERToolStripMenuItem;
        private System.Windows.Forms.Button btnSTCStart;
        private System.Windows.Forms.Button btnSTCStop;
        private System.Windows.Forms.Button btnAsyncSuspendUpdate;
        private TransparentTabControl tpLoggerCVS;
        private System.Windows.Forms.TabPage tpLoggerSync;
        private System.Windows.Forms.TabPage tpLoggerAsync;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAsyncHelp;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnLogCVSInstruction;
        private System.Windows.Forms.CheckBox cbSendToLogMem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txStopCounter;
        private System.Windows.Forms.ToolStripMenuItem tsm_OptionSync;
        private System.Windows.Forms.ToolStripMenuItem tsm_OptionAsync;
        private System.Windows.Forms.CheckBox cbCallBackMode;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox tbAsyncPeroid;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tbAlertTimer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbSendtoClientCode;
        private System.Windows.Forms.TabPage tpLoggerAsyncSnad;
        private System.Windows.Forms.Button btnAsyncHelpSnad;
        private System.Windows.Forms.Button btnAsyncSuspendUpdateSnad;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox tbAlertTimerSnad;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnSTCStartSnad;
        private System.Windows.Forms.Button btnSTCStopSnad;
        private System.Windows.Forms.CheckBox cbSendtoClientCodeSnad;
        private System.Windows.Forms.ToolStripMenuItem aboveAlsoCreateADTSessionLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbCRCPassed;
        private System.Windows.Forms.TextBox tbCRCFailed;
        private System.Windows.Forms.ComboBox cbSTCSTARTMode;
        private System.Windows.Forms.ToolStripMenuItem CbApplySetGetTime;
        private System.Windows.Forms.TabPage tpCommand;
        private System.Windows.Forms.Button btnSTCERASE_Verify;
        private System.Windows.Forms.Button btnSTCERASE_Bulk;
        private System.Windows.Forms.Button btnSTCERASE_Block;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripMenuItem rawDataignoreFormatFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tFormatFrameHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tmsi_FormattedCVStoFile;
        private System.Windows.Forms.ToolStripMenuItem tsmi_SplitDataToTwoColumn;
        private System.Windows.Forms.ToolStripMenuItem rFDPPDHelpToolStripMenuItem;
    }
}