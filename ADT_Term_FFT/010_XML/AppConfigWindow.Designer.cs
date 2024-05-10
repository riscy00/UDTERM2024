namespace UDT_Term_FFT
{
    partial class AppConfigWindow
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnReLoad = new System.Windows.Forms.Button();
            this.cbCompanyThemes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbBaudRateSelect = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbAutoDetectEnable = new System.Windows.Forms.CheckBox();
            this.cbSkipFTDIScan = new System.Windows.Forms.CheckBox();
            this.cbAutoCopyClip = new System.Windows.Forms.CheckBox();
            this.cbSelectTool = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbUserBaudRate = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.btnSetFolder = new System.Windows.Forms.Button();
            this.tbFolderName = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.cbTabPageList = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(353, 283);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(269, 283);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnReLoad
            // 
            this.btnReLoad.Location = new System.Drawing.Point(188, 283);
            this.btnReLoad.Name = "btnReLoad";
            this.btnReLoad.Size = new System.Drawing.Size(75, 23);
            this.btnReLoad.TabIndex = 2;
            this.btnReLoad.Text = "ReLoad";
            this.btnReLoad.UseVisualStyleBackColor = true;
            this.btnReLoad.Click += new System.EventHandler(this.btnReLoad_Click);
            // 
            // cbCompanyThemes
            // 
            this.cbCompanyThemes.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbCompanyThemes.FormattingEnabled = true;
            this.cbCompanyThemes.Location = new System.Drawing.Point(73, 12);
            this.cbCompanyThemes.Name = "cbCompanyThemes";
            this.cbCompanyThemes.Size = new System.Drawing.Size(134, 21);
            this.cbCompanyThemes.TabIndex = 5;
            this.cbCompanyThemes.SelectedIndexChanged += new System.EventHandler(this.cbCompanyThemes_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Set Themes";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(225, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Set BAUD";
            // 
            // cbBaudRateSelect
            // 
            this.cbBaudRateSelect.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbBaudRateSelect.FormattingEnabled = true;
            this.cbBaudRateSelect.Location = new System.Drawing.Point(281, 12);
            this.cbBaudRateSelect.Name = "cbBaudRateSelect";
            this.cbBaudRateSelect.Size = new System.Drawing.Size(134, 21);
            this.cbBaudRateSelect.TabIndex = 33;
            this.cbBaudRateSelect.SelectedIndexChanged += new System.EventHandler(this.cbBaudRateSelect_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbAutoDetectEnable);
            this.groupBox1.Controls.Add(this.cbSkipFTDIScan);
            this.groupBox1.Controls.Add(this.cbAutoCopyClip);
            this.groupBox1.Location = new System.Drawing.Point(12, 189);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(414, 88);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Option";
            // 
            // cbAutoDetectEnable
            // 
            this.cbAutoDetectEnable.AutoSize = true;
            this.cbAutoDetectEnable.Location = new System.Drawing.Point(7, 65);
            this.cbAutoDetectEnable.Name = "cbAutoDetectEnable";
            this.cbAutoDetectEnable.Size = new System.Drawing.Size(147, 17);
            this.cbAutoDetectEnable.TabIndex = 2;
            this.cbAutoDetectEnable.Text = "USB AutoDetect Enabled";
            this.cbAutoDetectEnable.UseVisualStyleBackColor = true;
            this.cbAutoDetectEnable.CheckedChanged += new System.EventHandler(this.cbAutoDetectEnable_CheckedChanged);
            // 
            // cbSkipFTDIScan
            // 
            this.cbSkipFTDIScan.AutoSize = true;
            this.cbSkipFTDIScan.Location = new System.Drawing.Point(7, 43);
            this.cbSkipFTDIScan.Name = "cbSkipFTDIScan";
            this.cbSkipFTDIScan.Size = new System.Drawing.Size(175, 17);
            this.cbSkipFTDIScan.TabIndex = 1;
            this.cbSkipFTDIScan.Text = "Skip FTDI scan (ie VCOM Only)";
            this.cbSkipFTDIScan.UseVisualStyleBackColor = true;
            this.cbSkipFTDIScan.CheckedChanged += new System.EventHandler(this.cbSkipFTDIScan_CheckedChanged);
            // 
            // cbAutoCopyClip
            // 
            this.cbAutoCopyClip.AutoSize = true;
            this.cbAutoCopyClip.Location = new System.Drawing.Point(7, 20);
            this.cbAutoCopyClip.Name = "cbAutoCopyClip";
            this.cbAutoCopyClip.Size = new System.Drawing.Size(197, 17);
            this.cbAutoCopyClip.TabIndex = 0;
            this.cbAutoCopyClip.Text = "Auto copy RX Message to Clipboard";
            this.cbAutoCopyClip.UseVisualStyleBackColor = true;
            this.cbAutoCopyClip.CheckedChanged += new System.EventHandler(this.cbAutoCopyClip_CheckedChanged);
            // 
            // cbSelectTool
            // 
            this.cbSelectTool.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbSelectTool.FormattingEnabled = true;
            this.cbSelectTool.Items.AddRange(new object[] {
            "Generic (All/Debug)",
            "ESM (Tool/App)",
            "RFDDAQ (Tool/App)",
            "TOCO (Tool/App)",
            "TS (Tool/App)",
            "ADIS (Sensor)",
            "OldDir (Tool/App)"});
            this.cbSelectTool.Location = new System.Drawing.Point(73, 39);
            this.cbSelectTool.Name = "cbSelectTool";
            this.cbSelectTool.Size = new System.Drawing.Size(134, 21);
            this.cbSelectTool.TabIndex = 38;
            this.cbSelectTool.SelectedIndexChanged += new System.EventHandler(this.cbSelectTool_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 39;
            this.label5.Text = "Select Tool";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(219, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 40;
            this.label4.Text = "User BAUD";
            // 
            // tbUserBaudRate
            // 
            this.tbUserBaudRate.Location = new System.Drawing.Point(281, 39);
            this.tbUserBaudRate.Name = "tbUserBaudRate";
            this.tbUserBaudRate.Size = new System.Drawing.Size(134, 20);
            this.tbUserBaudRate.TabIndex = 41;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(280, 62);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(145, 13);
            this.label6.TabIndex = 42;
            this.label6.Text = "0 = Not Defined, use indexed";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.btnOpenFolder);
            this.groupBox2.Controls.Add(this.btnSetFolder);
            this.groupBox2.Controls.Add(this.tbFolderName);
            this.groupBox2.Location = new System.Drawing.Point(12, 124);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(414, 65);
            this.groupBox2.TabIndex = 43;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Default FolderName";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(209, 13);
            this.label7.TabIndex = 46;
            this.label7.Text = "This where all data/config/project file goes";
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Location = new System.Drawing.Point(332, 12);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFolder.TabIndex = 45;
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // btnSetFolder
            // 
            this.btnSetFolder.Location = new System.Drawing.Point(251, 12);
            this.btnSetFolder.Name = "btnSetFolder";
            this.btnSetFolder.Size = new System.Drawing.Size(75, 23);
            this.btnSetFolder.TabIndex = 44;
            this.btnSetFolder.Text = "Set Folder";
            this.btnSetFolder.UseVisualStyleBackColor = true;
            this.btnSetFolder.Click += new System.EventHandler(this.btnSetFolder_Click);
            // 
            // tbFolderName
            // 
            this.tbFolderName.Location = new System.Drawing.Point(6, 39);
            this.tbFolderName.Name = "tbFolderName";
            this.tbFolderName.ReadOnly = true;
            this.tbFolderName.Size = new System.Drawing.Size(401, 20);
            this.tbFolderName.TabIndex = 42;
            // 
            // cbTabPageList
            // 
            this.cbTabPageList.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbTabPageList.FormattingEnabled = true;
            this.cbTabPageList.Location = new System.Drawing.Point(73, 66);
            this.cbTabPageList.Name = "cbTabPageList";
            this.cbTabPageList.Size = new System.Drawing.Size(134, 21);
            this.cbTabPageList.TabIndex = 3;
            this.cbTabPageList.SelectedIndexChanged += new System.EventHandler(this.cbTabPageList_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 47;
            this.label3.Text = "TabPage";
            // 
            // AppConfigWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 311);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbTabPageList);
            this.Controls.Add(this.tbUserBaudRate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbSelectTool);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbBaudRateSelect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbCompanyThemes);
            this.Controls.Add(this.btnReLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(560, 350);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 350);
            this.Name = "AppConfigWindow";
            this.ShowIcon = false;
            this.Text = "UDT Setup/Option";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AppConfigWindow_FormClosing);
            this.Load += new System.EventHandler(this.AppConfigWindow_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnReLoad;
        private System.Windows.Forms.ComboBox cbCompanyThemes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbBaudRateSelect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbAutoCopyClip;
        private System.Windows.Forms.CheckBox cbSkipFTDIScan;
        private System.Windows.Forms.CheckBox cbAutoDetectEnable;
        private System.Windows.Forms.ComboBox cbSelectTool;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbUserBaudRate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Button btnSetFolder;
        private System.Windows.Forms.TextBox tbFolderName;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ComboBox cbTabPageList;
        private System.Windows.Forms.Label label3;
    }
}