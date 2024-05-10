namespace UDT_Term_FFT
{
    partial class TabLoggerCVS
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnExportSetting = new System.Windows.Forms.Button();
            this.btnImportSetting = new System.Windows.Forms.Button();
            this.chTabEnable = new System.Windows.Forms.CheckBox();
            this.lbEnableText = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.pB_ADIS_Image = new System.Windows.Forms.PictureBox();
            this.gbSelectTrigger = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbSelectTrig = new System.Windows.Forms.ComboBox();
            this.gbLoggerCVS = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbsProjectName = new System.Windows.Forms.TextBox();
            this.cbLoggerFlash = new System.Windows.Forms.CheckBox();
            this.cbExporttoUDT = new System.Windows.Forms.CheckBox();
            this.gbTimer = new System.Windows.Forms.GroupBox();
            this.tbCounter = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbTimerPeroid = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOpenLoggerWindow = new System.Windows.Forms.Button();
            this.BtnOpenSurveyWindow = new System.Windows.Forms.Button();
            this.gbLoggerWindow = new System.Windows.Forms.GroupBox();
            this.tbFolderName = new System.Windows.Forms.TextBox();
            this.btnSetupFolder = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pB_ADIS_Image)).BeginInit();
            this.gbSelectTrigger.SuspendLayout();
            this.gbLoggerCVS.SuspendLayout();
            this.gbTimer.SuspendLayout();
            this.gbLoggerWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExportSetting
            // 
            this.btnExportSetting.Location = new System.Drawing.Point(654, 5);
            this.btnExportSetting.Name = "btnExportSetting";
            this.btnExportSetting.Size = new System.Drawing.Size(100, 23);
            this.btnExportSetting.TabIndex = 82;
            this.btnExportSetting.Text = "Export Setting";
            this.btnExportSetting.UseVisualStyleBackColor = true;
            this.btnExportSetting.Click += new System.EventHandler(this.btnExportSetting_Click);
            // 
            // btnImportSetting
            // 
            this.btnImportSetting.Location = new System.Drawing.Point(548, 5);
            this.btnImportSetting.Name = "btnImportSetting";
            this.btnImportSetting.Size = new System.Drawing.Size(100, 23);
            this.btnImportSetting.TabIndex = 83;
            this.btnImportSetting.Text = "Import Setting";
            this.btnImportSetting.UseVisualStyleBackColor = true;
            this.btnImportSetting.Click += new System.EventHandler(this.btnImportSetting_Click);
            // 
            // chTabEnable
            // 
            this.chTabEnable.AutoSize = true;
            this.chTabEnable.Checked = true;
            this.chTabEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chTabEnable.Location = new System.Drawing.Point(483, 9);
            this.chTabEnable.Name = "chTabEnable";
            this.chTabEnable.Size = new System.Drawing.Size(59, 17);
            this.chTabEnable.TabIndex = 81;
            this.chTabEnable.Text = "Enable";
            this.chTabEnable.UseVisualStyleBackColor = true;
            this.chTabEnable.MouseClick += new System.Windows.Forms.MouseEventHandler(this.chTabEnable_MouseClick);
            // 
            // lbEnableText
            // 
            this.lbEnableText.AutoSize = true;
            this.lbEnableText.BackColor = System.Drawing.Color.White;
            this.lbEnableText.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbEnableText.Location = new System.Drawing.Point(539, 5);
            this.lbEnableText.Name = "lbEnableText";
            this.lbEnableText.Size = new System.Drawing.Size(225, 24);
            this.lbEnableText.TabIndex = 107;
            this.lbEnableText.Text = "LoggerCVS is Disabled";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(476, 337);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(69, 23);
            this.btnStop.TabIndex = 104;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(399, 337);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(71, 23);
            this.btnRun.TabIndex = 105;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // pB_ADIS_Image
            // 
            this.pB_ADIS_Image.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pB_ADIS_Image.Image = global::UDT_Term.Properties.Resources.TabLoggerCVS_Diagram1;
            this.pB_ADIS_Image.Location = new System.Drawing.Point(5, 127);
            this.pB_ADIS_Image.Name = "pB_ADIS_Image";
            this.pB_ADIS_Image.Size = new System.Drawing.Size(388, 239);
            this.pB_ADIS_Image.TabIndex = 84;
            this.pB_ADIS_Image.TabStop = false;
            // 
            // gbSelectTrigger
            // 
            this.gbSelectTrigger.Controls.Add(this.label2);
            this.gbSelectTrigger.Controls.Add(this.cbSelectTrig);
            this.gbSelectTrigger.Location = new System.Drawing.Point(399, 127);
            this.gbSelectTrigger.Name = "gbSelectTrigger";
            this.gbSelectTrigger.Size = new System.Drawing.Size(355, 52);
            this.gbSelectTrigger.TabIndex = 108;
            this.gbSelectTrigger.TabStop = false;
            this.gbSelectTrigger.Text = "Capture Trigger";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Select Trigger";
            // 
            // cbSelectTrig
            // 
            this.cbSelectTrig.FormattingEnabled = true;
            this.cbSelectTrig.Location = new System.Drawing.Point(88, 19);
            this.cbSelectTrig.Name = "cbSelectTrig";
            this.cbSelectTrig.Size = new System.Drawing.Size(126, 21);
            this.cbSelectTrig.TabIndex = 6;
            this.cbSelectTrig.SelectedIndexChanged += new System.EventHandler(this.cbSelectTrig_SelectedIndexChanged);
            this.cbSelectTrig.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cbSelectTrig_MouseDown);
            this.cbSelectTrig.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cbSelectTrig_MouseUp);
            // 
            // gbLoggerCVS
            // 
            this.gbLoggerCVS.Controls.Add(this.label1);
            this.gbLoggerCVS.Controls.Add(this.tbsProjectName);
            this.gbLoggerCVS.Controls.Add(this.cbLoggerFlash);
            this.gbLoggerCVS.Controls.Add(this.cbExporttoUDT);
            this.gbLoggerCVS.Location = new System.Drawing.Point(399, 243);
            this.gbLoggerCVS.Name = "gbLoggerCVS";
            this.gbLoggerCVS.Size = new System.Drawing.Size(355, 88);
            this.gbLoggerCVS.TabIndex = 109;
            this.gbLoggerCVS.TabStop = false;
            this.gbLoggerCVS.Text = "LoggerCVS";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Project Name";
            // 
            // tbsProjectName
            // 
            this.tbsProjectName.Location = new System.Drawing.Point(80, 61);
            this.tbsProjectName.Name = "tbsProjectName";
            this.tbsProjectName.Size = new System.Drawing.Size(269, 20);
            this.tbsProjectName.TabIndex = 2;
            // 
            // cbLoggerFlash
            // 
            this.cbLoggerFlash.AutoSize = true;
            this.cbLoggerFlash.Location = new System.Drawing.Point(9, 40);
            this.cbLoggerFlash.Name = "cbLoggerFlash";
            this.cbLoggerFlash.Size = new System.Drawing.Size(133, 17);
            this.cbLoggerFlash.TabIndex = 1;
            this.cbLoggerFlash.Text = "Logger Memory (Flash)";
            this.cbLoggerFlash.UseVisualStyleBackColor = true;
            // 
            // cbExporttoUDT
            // 
            this.cbExporttoUDT.AutoSize = true;
            this.cbExporttoUDT.Location = new System.Drawing.Point(9, 20);
            this.cbExporttoUDT.Name = "cbExporttoUDT";
            this.cbExporttoUDT.Size = new System.Drawing.Size(94, 17);
            this.cbExporttoUDT.TabIndex = 0;
            this.cbExporttoUDT.Text = "Export to UDT";
            this.cbExporttoUDT.UseVisualStyleBackColor = true;
            // 
            // gbTimer
            // 
            this.gbTimer.Controls.Add(this.tbCounter);
            this.gbTimer.Controls.Add(this.label4);
            this.gbTimer.Controls.Add(this.tbTimerPeroid);
            this.gbTimer.Controls.Add(this.label3);
            this.gbTimer.Location = new System.Drawing.Point(399, 185);
            this.gbTimer.Name = "gbTimer";
            this.gbTimer.Size = new System.Drawing.Size(355, 52);
            this.gbTimer.TabIndex = 109;
            this.gbTimer.TabStop = false;
            this.gbTimer.Text = "Timer";
            // 
            // tbCounter
            // 
            this.tbCounter.Location = new System.Drawing.Point(261, 19);
            this.tbCounter.Name = "tbCounter";
            this.tbCounter.Size = new System.Drawing.Size(85, 20);
            this.tbCounter.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(211, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Counter";
            // 
            // tbTimerPeroid
            // 
            this.tbTimerPeroid.Location = new System.Drawing.Point(80, 19);
            this.tbTimerPeroid.Name = "tbTimerPeroid";
            this.tbTimerPeroid.Size = new System.Drawing.Size(125, 20);
            this.tbTimerPeroid.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Peroid(mSec)";
            // 
            // btnOpenLoggerWindow
            // 
            this.btnOpenLoggerWindow.Location = new System.Drawing.Point(67, 14);
            this.btnOpenLoggerWindow.Name = "btnOpenLoggerWindow";
            this.btnOpenLoggerWindow.Size = new System.Drawing.Size(132, 36);
            this.btnOpenLoggerWindow.TabIndex = 110;
            this.btnOpenLoggerWindow.Text = "Open Logger Window";
            this.btnOpenLoggerWindow.UseVisualStyleBackColor = true;
            this.btnOpenLoggerWindow.Click += new System.EventHandler(this.btnOpenLoggerWindow_Click);
            // 
            // BtnOpenSurveyWindow
            // 
            this.BtnOpenSurveyWindow.Location = new System.Drawing.Point(205, 14);
            this.BtnOpenSurveyWindow.Name = "BtnOpenSurveyWindow";
            this.BtnOpenSurveyWindow.Size = new System.Drawing.Size(140, 36);
            this.BtnOpenSurveyWindow.TabIndex = 111;
            this.BtnOpenSurveyWindow.Text = "Open Survey Window";
            this.BtnOpenSurveyWindow.UseVisualStyleBackColor = true;
            this.BtnOpenSurveyWindow.Click += new System.EventHandler(this.BtnOpenSurveyWindow_Click);
            // 
            // gbLoggerWindow
            // 
            this.gbLoggerWindow.Controls.Add(this.btnOpenFolder);
            this.gbLoggerWindow.Controls.Add(this.btnSetupFolder);
            this.gbLoggerWindow.Controls.Add(this.tbFolderName);
            this.gbLoggerWindow.Controls.Add(this.btnOpenLoggerWindow);
            this.gbLoggerWindow.Controls.Add(this.BtnOpenSurveyWindow);
            this.gbLoggerWindow.Location = new System.Drawing.Point(399, 34);
            this.gbLoggerWindow.Name = "gbLoggerWindow";
            this.gbLoggerWindow.Size = new System.Drawing.Size(355, 87);
            this.gbLoggerWindow.TabIndex = 112;
            this.gbLoggerWindow.TabStop = false;
            this.gbLoggerWindow.Text = "Logger Window";
            // 
            // tbFolderName
            // 
            this.tbFolderName.Location = new System.Drawing.Point(130, 56);
            this.tbFolderName.Name = "tbFolderName";
            this.tbFolderName.ReadOnly = true;
            this.tbFolderName.Size = new System.Drawing.Size(216, 20);
            this.tbFolderName.TabIndex = 112;
            // 
            // btnSetupFolder
            // 
            this.btnSetupFolder.Location = new System.Drawing.Point(68, 53);
            this.btnSetupFolder.Name = "btnSetupFolder";
            this.btnSetupFolder.Size = new System.Drawing.Size(56, 23);
            this.btnSetupFolder.TabIndex = 113;
            this.btnSetupFolder.Text = "Folder";
            this.btnSetupFolder.UseVisualStyleBackColor = true;
            this.btnSetupFolder.Click += new System.EventHandler(this.btnSetupFolder_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Location = new System.Drawing.Point(9, 53);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(53, 23);
            this.btnOpenFolder.TabIndex = 114;
            this.btnOpenFolder.Text = "Open";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // TabLoggerCVS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.gbLoggerWindow);
            this.Controls.Add(this.gbTimer);
            this.Controls.Add(this.gbLoggerCVS);
            this.Controls.Add(this.gbSelectTrigger);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.pB_ADIS_Image);
            this.Controls.Add(this.btnExportSetting);
            this.Controls.Add(this.btnImportSetting);
            this.Controls.Add(this.chTabEnable);
            this.Controls.Add(this.lbEnableText);
            this.MaximumSize = new System.Drawing.Size(758, 370);
            this.MinimumSize = new System.Drawing.Size(758, 370);
            this.Name = "TabLoggerCVS";
            this.Size = new System.Drawing.Size(758, 370);
            this.Load += new System.EventHandler(this.TabLoggerCVS_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pB_ADIS_Image)).EndInit();
            this.gbSelectTrigger.ResumeLayout(false);
            this.gbSelectTrigger.PerformLayout();
            this.gbLoggerCVS.ResumeLayout(false);
            this.gbLoggerCVS.PerformLayout();
            this.gbTimer.ResumeLayout(false);
            this.gbTimer.PerformLayout();
            this.gbLoggerWindow.ResumeLayout(false);
            this.gbLoggerWindow.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pB_ADIS_Image;
        private System.Windows.Forms.Button btnExportSetting;
        private System.Windows.Forms.Button btnImportSetting;
        private System.Windows.Forms.CheckBox chTabEnable;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lbEnableText;
        private System.Windows.Forms.GroupBox gbSelectTrigger;
        private System.Windows.Forms.GroupBox gbLoggerCVS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbsProjectName;
        private System.Windows.Forms.CheckBox cbLoggerFlash;
        private System.Windows.Forms.CheckBox cbExporttoUDT;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbSelectTrig;
        private System.Windows.Forms.GroupBox gbTimer;
        private System.Windows.Forms.TextBox tbCounter;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbTimerPeroid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOpenLoggerWindow;
        private System.Windows.Forms.Button BtnOpenSurveyWindow;
        private System.Windows.Forms.GroupBox gbLoggerWindow;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Button btnSetupFolder;
        private System.Windows.Forms.TextBox tbFolderName;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}
