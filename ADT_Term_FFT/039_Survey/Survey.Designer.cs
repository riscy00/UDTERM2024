namespace UDT_Term_FFT
{
    partial class Survey
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnClear = new System.Windows.Forms.Button();
            this.BtnOpenFolder = new System.Windows.Forms.Button();
            this.btnProjectFolder = new System.Windows.Forms.Button();
            this.txtFolderName = new System.Windows.Forms.TextBox();
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.dgvSurveyViewer = new System.Windows.Forms.DataGridView();
            this.cbExtendView = new System.Windows.Forms.CheckBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.cbSuspendScroll = new System.Windows.Forms.CheckBox();
            this.btnPause = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbHMRLoop = new System.Windows.Forms.TextBox();
            this.cbMFormat = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbAutoLoop = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbAppendFile = new System.Windows.Forms.CheckBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCalWindow = new System.Windows.Forms.Button();
            this.cbStayTop = new System.Windows.Forms.CheckBox();
            this.btnSaveAllData = new System.Windows.Forms.Button();
            this.dgvDeviceStatus = new System.Windows.Forms.DataGridView();
            this.btnLoadAllData = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSurveyViewer)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeviceStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(921, 97);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // BtnOpenFolder
            // 
            this.BtnOpenFolder.Location = new System.Drawing.Point(661, 68);
            this.BtnOpenFolder.Name = "BtnOpenFolder";
            this.BtnOpenFolder.Size = new System.Drawing.Size(98, 22);
            this.BtnOpenFolder.TabIndex = 8;
            this.BtnOpenFolder.Text = "Explore Folder";
            this.BtnOpenFolder.UseVisualStyleBackColor = true;
            this.BtnOpenFolder.Click += new System.EventHandler(this.BtnOpenFolder_Click);
            // 
            // btnProjectFolder
            // 
            this.btnProjectFolder.Location = new System.Drawing.Point(661, 40);
            this.btnProjectFolder.Name = "btnProjectFolder";
            this.btnProjectFolder.Size = new System.Drawing.Size(98, 22);
            this.btnProjectFolder.TabIndex = 6;
            this.btnProjectFolder.Text = "Project Folder";
            this.btnProjectFolder.UseVisualStyleBackColor = true;
            this.btnProjectFolder.Click += new System.EventHandler(this.btnProjectFolder_Click);
            // 
            // txtFolderName
            // 
            this.txtFolderName.BackColor = System.Drawing.Color.Beige;
            this.txtFolderName.Location = new System.Drawing.Point(661, 15);
            this.txtFolderName.Name = "txtFolderName";
            this.txtFolderName.ReadOnly = true;
            this.txtFolderName.Size = new System.Drawing.Size(416, 20);
            this.txtFolderName.TabIndex = 5;
            // 
            // btnQuit
            // 
            this.btnQuit.Location = new System.Drawing.Point(1002, 98);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(75, 22);
            this.btnQuit.TabIndex = 13;
            this.btnQuit.Text = "Close";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(921, 39);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 15;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // dgvSurveyViewer
            // 
            this.dgvSurveyViewer.AllowUserToAddRows = false;
            this.dgvSurveyViewer.AllowUserToDeleteRows = false;
            this.dgvSurveyViewer.AllowUserToResizeColumns = false;
            this.dgvSurveyViewer.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSurveyViewer.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSurveyViewer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSurveyViewer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSurveyViewer.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSurveyViewer.EnableHeadersVisualStyles = false;
            this.dgvSurveyViewer.Location = new System.Drawing.Point(12, 127);
            this.dgvSurveyViewer.MaximumSize = new System.Drawing.Size(1065, 320);
            this.dgvSurveyViewer.MinimumSize = new System.Drawing.Size(1065, 320);
            this.dgvSurveyViewer.Name = "dgvSurveyViewer";
            this.dgvSurveyViewer.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightGreen;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            this.dgvSurveyViewer.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvSurveyViewer.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvSurveyViewer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSurveyViewer.Size = new System.Drawing.Size(1065, 320);
            this.dgvSurveyViewer.StandardTab = true;
            this.dgvSurveyViewer.TabIndex = 20;
            this.dgvSurveyViewer.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvSurveyViewer_CellMouseClick);
            this.dgvSurveyViewer.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvSurveyViewer_CellMouseUp);
            this.dgvSurveyViewer.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvSurveyViewer_ColumnHeaderMouseClick);
            this.dgvSurveyViewer.MouseLeave += new System.EventHandler(this.dgvSurveyViewer_MouseLeave);
            // 
            // cbExtendView
            // 
            this.cbExtendView.AutoSize = true;
            this.cbExtendView.BackColor = System.Drawing.Color.Transparent;
            this.cbExtendView.Location = new System.Drawing.Point(1002, 80);
            this.cbExtendView.Name = "cbExtendView";
            this.cbExtendView.Size = new System.Drawing.Size(82, 17);
            this.cbExtendView.TabIndex = 22;
            this.cbExtendView.Text = "ExtendView";
            this.cbExtendView.UseVisualStyleBackColor = false;
            this.cbExtendView.CheckedChanged += new System.EventHandler(this.cbExtendView_CheckedChanged);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(921, 68);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 23;
            this.btnReset.Text = "Sort Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // cbSuspendScroll
            // 
            this.cbSuspendScroll.AutoSize = true;
            this.cbSuspendScroll.BackColor = System.Drawing.Color.Transparent;
            this.cbSuspendScroll.Location = new System.Drawing.Point(798, 102);
            this.cbSuspendScroll.Name = "cbSuspendScroll";
            this.cbSuspendScroll.Size = new System.Drawing.Size(97, 17);
            this.cbSuspendScroll.TabIndex = 2;
            this.cbSuspendScroll.Text = "Suspend Scroll";
            this.cbSuspendScroll.UseVisualStyleBackColor = false;
            // 
            // btnPause
            // 
            this.btnPause.Enabled = false;
            this.btnPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPause.Location = new System.Drawing.Point(19, 95);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 24);
            this.btnPause.TabIndex = 28;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(87, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 46;
            this.label2.Text = "mSec/Loop";
            // 
            // tbHMRLoop
            // 
            this.tbHMRLoop.Location = new System.Drawing.Point(6, 49);
            this.tbHMRLoop.Name = "tbHMRLoop";
            this.tbHMRLoop.Size = new System.Drawing.Size(75, 20);
            this.tbHMRLoop.TabIndex = 45;
            this.tbHMRLoop.Text = "1000";
            this.tbHMRLoop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cbMFormat
            // 
            this.cbMFormat.FormattingEnabled = true;
            this.cbMFormat.Items.AddRange(new object[] {
            "Raw",
            "uT",
            "mG"});
            this.cbMFormat.Location = new System.Drawing.Point(66, 18);
            this.cbMFormat.Name = "cbMFormat";
            this.cbMFormat.Size = new System.Drawing.Size(67, 21);
            this.cbMFormat.TabIndex = 47;
            this.cbMFormat.SelectedIndexChanged += new System.EventHandler(this.cbMFormat_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(9, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 48;
            this.label1.Text = "M-Format";
            // 
            // cbAutoLoop
            // 
            this.cbAutoLoop.AutoSize = true;
            this.cbAutoLoop.BackColor = System.Drawing.Color.Transparent;
            this.cbAutoLoop.Location = new System.Drawing.Point(87, 15);
            this.cbAutoLoop.Name = "cbAutoLoop";
            this.cbAutoLoop.Size = new System.Drawing.Size(75, 17);
            this.cbAutoLoop.TabIndex = 49;
            this.cbAutoLoop.Text = "Auto-Loop";
            this.cbAutoLoop.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.cbAppendFile);
            this.groupBox1.Controls.Add(this.tbHMRLoop);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.cbAutoLoop);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 78);
            this.groupBox1.TabIndex = 50;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sensor Operation";
            // 
            // cbAppendFile
            // 
            this.cbAppendFile.AutoSize = true;
            this.cbAppendFile.BackColor = System.Drawing.Color.Transparent;
            this.cbAppendFile.Location = new System.Drawing.Point(87, 33);
            this.cbAppendFile.Name = "cbAppendFile";
            this.cbAppendFile.Size = new System.Drawing.Size(82, 17);
            this.cbAppendFile.TabIndex = 55;
            this.cbAppendFile.Text = "Append File";
            this.cbAppendFile.UseVisualStyleBackColor = false;
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(6, 18);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 54;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(110, 95);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 24);
            this.btnStop.TabIndex = 51;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.btnCalWindow);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.cbMFormat);
            this.groupBox3.Location = new System.Drawing.Point(201, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(139, 109);
            this.groupBox3.TabIndex = 52;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sensor Setup";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 81);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(127, 22);
            this.button1.TabIndex = 56;
            this.button1.Text = "Directional Window";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCalWindow
            // 
            this.btnCalWindow.Location = new System.Drawing.Point(6, 53);
            this.btnCalWindow.Name = "btnCalWindow";
            this.btnCalWindow.Size = new System.Drawing.Size(127, 22);
            this.btnCalWindow.TabIndex = 55;
            this.btnCalWindow.Text = "Calibration Window";
            this.btnCalWindow.UseVisualStyleBackColor = true;
            this.btnCalWindow.Click += new System.EventHandler(this.btnCalWindow_Click);
            // 
            // cbStayTop
            // 
            this.cbStayTop.AutoSize = true;
            this.cbStayTop.BackColor = System.Drawing.Color.Transparent;
            this.cbStayTop.Location = new System.Drawing.Point(1002, 58);
            this.cbStayTop.Name = "cbStayTop";
            this.cbStayTop.Size = new System.Drawing.Size(69, 17);
            this.cbStayTop.TabIndex = 53;
            this.cbStayTop.Text = "Stay Top";
            this.cbStayTop.UseVisualStyleBackColor = false;
            this.cbStayTop.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cbStayTop_MouseClick);
            // 
            // btnSaveAllData
            // 
            this.btnSaveAllData.Location = new System.Drawing.Point(765, 67);
            this.btnSaveAllData.Name = "btnSaveAllData";
            this.btnSaveAllData.Size = new System.Drawing.Size(98, 22);
            this.btnSaveAllData.TabIndex = 54;
            this.btnSaveAllData.Text = "Save Data CVS";
            this.btnSaveAllData.UseVisualStyleBackColor = true;
            this.btnSaveAllData.Click += new System.EventHandler(this.btnSaveAllData_Click);
            // 
            // dgvDeviceStatus
            // 
            this.dgvDeviceStatus.AllowUserToAddRows = false;
            this.dgvDeviceStatus.AllowUserToDeleteRows = false;
            this.dgvDeviceStatus.AllowUserToResizeColumns = false;
            this.dgvDeviceStatus.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.WhiteSmoke;
            this.dgvDeviceStatus.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDeviceStatus.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvDeviceStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDeviceStatus.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvDeviceStatus.Location = new System.Drawing.Point(346, 12);
            this.dgvDeviceStatus.Name = "dgvDeviceStatus";
            this.dgvDeviceStatus.ReadOnly = true;
            this.dgvDeviceStatus.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDeviceStatus.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvDeviceStatus.RowHeadersVisible = false;
            this.dgvDeviceStatus.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold);
            this.dgvDeviceStatus.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvDeviceStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvDeviceStatus.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvDeviceStatus.Size = new System.Drawing.Size(169, 109);
            this.dgvDeviceStatus.StandardTab = true;
            this.dgvDeviceStatus.TabIndex = 55;
            // 
            // btnLoadAllData
            // 
            this.btnLoadAllData.Location = new System.Drawing.Point(765, 39);
            this.btnLoadAllData.Name = "btnLoadAllData";
            this.btnLoadAllData.Size = new System.Drawing.Size(98, 22);
            this.btnLoadAllData.TabIndex = 56;
            this.btnLoadAllData.Text = "Load Data CVS";
            this.btnLoadAllData.UseVisualStyleBackColor = true;
            this.btnLoadAllData.Click += new System.EventHandler(this.btnLoadAllData_Click);
            // 
            // Survey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1089, 459);
            this.Controls.Add(this.btnLoadAllData);
            this.Controls.Add(this.dgvDeviceStatus);
            this.Controls.Add(this.btnSaveAllData);
            this.Controls.Add(this.cbStayTop);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbSuspendScroll);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.cbExtendView);
            this.Controls.Add(this.dgvSurveyViewer);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.BtnOpenFolder);
            this.Controls.Add(this.btnProjectFolder);
            this.Controls.Add(this.txtFolderName);
            this.Controls.Add(this.btnClear);
            this.MaximumSize = new System.Drawing.Size(1105, 498);
            this.MinimumSize = new System.Drawing.Size(1105, 498);
            this.Name = "Survey";
            this.Text = "Real Time Survey Tools";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Survey_FormClosing);
            this.Load += new System.EventHandler(this.Survey_Load);
            this.Resize += new System.EventHandler(this.Survey_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSurveyViewer)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeviceStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button BtnOpenFolder;
        private System.Windows.Forms.Button btnProjectFolder;
        private System.Windows.Forms.TextBox txtFolderName;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.DataGridView dgvSurveyViewer;
        private System.Windows.Forms.CheckBox cbExtendView;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.CheckBox cbSuspendScroll;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbHMRLoop;
        private System.Windows.Forms.ComboBox cbMFormat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbAutoLoop;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbStayTop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.CheckBox cbAppendFile;
        private System.Windows.Forms.Button btnSaveAllData;
        private System.Windows.Forms.Button btnCalWindow;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dgvDeviceStatus;
        private System.Windows.Forms.Button btnLoadAllData;
    }
}