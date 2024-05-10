namespace UDT_Term_FFT
{
    partial class ImportDataCVS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportDataCVS));
            this.rtbImportConsole = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.BtnOpenFolder = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.txtFolderName = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableDataViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fillWithTestDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dDataFrameLoggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cADTDataFrameLoggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cADTDataFrameLoggerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cADTDataFrameLoggerToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cADTDataFrameLoggerToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.cADTDataFrameLoggerToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.cADTDataFrameLoggerToolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instructionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbImportConsole
            // 
            this.rtbImportConsole.BackColor = System.Drawing.Color.DarkSlateGray;
            this.rtbImportConsole.DetectUrls = false;
            this.rtbImportConsole.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbImportConsole.ForeColor = System.Drawing.Color.Yellow;
            this.rtbImportConsole.Location = new System.Drawing.Point(12, 104);
            this.rtbImportConsole.MaximumSize = new System.Drawing.Size(714, 110);
            this.rtbImportConsole.MinimumSize = new System.Drawing.Size(714, 110);
            this.rtbImportConsole.Name = "rtbImportConsole";
            this.rtbImportConsole.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbImportConsole.Size = new System.Drawing.Size(714, 110);
            this.rtbImportConsole.TabIndex = 16;
            this.rtbImportConsole.Text = resources.GetString("rtbImportConsole.Text");
            this.rtbImportConsole.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rtbImportConsole_MouseUp);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.BtnOpenFolder);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtFilename);
            this.groupBox3.Controls.Add(this.btnFolder);
            this.groupBox3.Controls.Add(this.txtFolderName);
            this.groupBox3.Location = new System.Drawing.Point(12, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(714, 71);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Filename";
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Current FileName";
            // 
            // txtFilename
            // 
            this.txtFilename.Location = new System.Drawing.Point(100, 45);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.ReadOnly = true;
            this.txtFilename.Size = new System.Drawing.Size(604, 20);
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
            this.txtFolderName.Size = new System.Drawing.Size(519, 20);
            this.txtFolderName.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem,
            this.optionToolStripMenuItem,
            this.quickFolderToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(738, 24);
            this.menuStrip1.TabIndex = 18;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveDataToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.toolsToolStripMenuItem.Text = "File";
            // 
            // saveDataToolStripMenuItem
            // 
            this.saveDataToolStripMenuItem.Name = "saveDataToolStripMenuItem";
            this.saveDataToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.saveDataToolStripMenuItem.Text = "Save Data";
            this.saveDataToolStripMenuItem.Click += new System.EventHandler(this.saveDataToolStripMenuItem_Click);
            // 
            // optionToolStripMenuItem
            // 
            this.optionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableDataViewToolStripMenuItem,
            this.fillWithTestDataToolStripMenuItem});
            this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
            this.optionToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.optionToolStripMenuItem.Text = "Option";
            // 
            // enableDataViewToolStripMenuItem
            // 
            this.enableDataViewToolStripMenuItem.Name = "enableDataViewToolStripMenuItem";
            this.enableDataViewToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.enableDataViewToolStripMenuItem.Text = "Enable Data View";
            this.enableDataViewToolStripMenuItem.Click += new System.EventHandler(this.enableDataViewToolStripMenuItem_Click);
            // 
            // fillWithTestDataToolStripMenuItem
            // 
            this.fillWithTestDataToolStripMenuItem.Name = "fillWithTestDataToolStripMenuItem";
            this.fillWithTestDataToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.fillWithTestDataToolStripMenuItem.Text = "Fill with test Data";
            this.fillWithTestDataToolStripMenuItem.Click += new System.EventHandler(this.fillWithTestDataToolStripMenuItem_Click);
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
            this.cADTDataFrameLoggerToolStripMenuItem5});
            this.quickFolderToolStripMenuItem.Name = "quickFolderToolStripMenuItem";
            this.quickFolderToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.quickFolderToolStripMenuItem.Text = "Quick Folder";
            // 
            // dDataFrameLoggerToolStripMenuItem
            // 
            this.dDataFrameLoggerToolStripMenuItem.Name = "dDataFrameLoggerToolStripMenuItem";
            this.dDataFrameLoggerToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.dDataFrameLoggerToolStripMenuItem.Text = "D:\\\\ADTDataFrameLogger";
            this.dDataFrameLoggerToolStripMenuItem.Click += new System.EventHandler(this.dDataFrameLoggerToolStripMenuItem_Click);
            // 
            // cADTDataFrameLoggerToolStripMenuItem
            // 
            this.cADTDataFrameLoggerToolStripMenuItem.Name = "cADTDataFrameLoggerToolStripMenuItem";
            this.cADTDataFrameLoggerToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.cADTDataFrameLoggerToolStripMenuItem.Text = "E:\\\\ADTDataFrameLogger";
            this.cADTDataFrameLoggerToolStripMenuItem.Click += new System.EventHandler(this.cADTDataFrameLoggerToolStripMenuItem_Click);
            // 
            // cADTDataFrameLoggerToolStripMenuItem1
            // 
            this.cADTDataFrameLoggerToolStripMenuItem1.Name = "cADTDataFrameLoggerToolStripMenuItem1";
            this.cADTDataFrameLoggerToolStripMenuItem1.Size = new System.Drawing.Size(213, 22);
            this.cADTDataFrameLoggerToolStripMenuItem1.Text = "F:\\\\ADTDataFrameLogger";
            this.cADTDataFrameLoggerToolStripMenuItem1.Click += new System.EventHandler(this.cADTDataFrameLoggerToolStripMenuItem1_Click);
            // 
            // cADTDataFrameLoggerToolStripMenuItem2
            // 
            this.cADTDataFrameLoggerToolStripMenuItem2.Name = "cADTDataFrameLoggerToolStripMenuItem2";
            this.cADTDataFrameLoggerToolStripMenuItem2.Size = new System.Drawing.Size(213, 22);
            this.cADTDataFrameLoggerToolStripMenuItem2.Text = "G:\\\\ADTDataFrameLogger";
            this.cADTDataFrameLoggerToolStripMenuItem2.Click += new System.EventHandler(this.cADTDataFrameLoggerToolStripMenuItem2_Click);
            // 
            // cADTDataFrameLoggerToolStripMenuItem3
            // 
            this.cADTDataFrameLoggerToolStripMenuItem3.Name = "cADTDataFrameLoggerToolStripMenuItem3";
            this.cADTDataFrameLoggerToolStripMenuItem3.Size = new System.Drawing.Size(213, 22);
            this.cADTDataFrameLoggerToolStripMenuItem3.Text = "H:\\\\ADTDataFrameLogger";
            this.cADTDataFrameLoggerToolStripMenuItem3.Click += new System.EventHandler(this.cADTDataFrameLoggerToolStripMenuItem3_Click);
            // 
            // cADTDataFrameLoggerToolStripMenuItem4
            // 
            this.cADTDataFrameLoggerToolStripMenuItem4.Name = "cADTDataFrameLoggerToolStripMenuItem4";
            this.cADTDataFrameLoggerToolStripMenuItem4.Size = new System.Drawing.Size(213, 22);
            this.cADTDataFrameLoggerToolStripMenuItem4.Text = "I:\\\\ADTDataFrameLogger";
            this.cADTDataFrameLoggerToolStripMenuItem4.Click += new System.EventHandler(this.cADTDataFrameLoggerToolStripMenuItem4_Click);
            // 
            // cADTDataFrameLoggerToolStripMenuItem5
            // 
            this.cADTDataFrameLoggerToolStripMenuItem5.Name = "cADTDataFrameLoggerToolStripMenuItem5";
            this.cADTDataFrameLoggerToolStripMenuItem5.Size = new System.Drawing.Size(213, 22);
            this.cADTDataFrameLoggerToolStripMenuItem5.Text = "J:\\\\ADTDataFrameLogger";
            this.cADTDataFrameLoggerToolStripMenuItem5.Click += new System.EventHandler(this.cADTDataFrameLoggerToolStripMenuItem5_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.instructionToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // instructionToolStripMenuItem
            // 
            this.instructionToolStripMenuItem.Name = "instructionToolStripMenuItem";
            this.instructionToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.instructionToolStripMenuItem.Text = "Instruction";
            this.instructionToolStripMenuItem.Click += new System.EventHandler(this.instructionToolStripMenuItem_Click);
            // 
            // ImportDataCVS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_1A;
            this.ClientSize = new System.Drawing.Size(738, 222);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.rtbImportConsole);
            this.MaximumSize = new System.Drawing.Size(754, 261);
            this.MinimumSize = new System.Drawing.Size(754, 261);
            this.Name = "ImportDataCVS";
            this.Text = "Import Data CVS";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImportDataCVS_FormClosing);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        //public System.Windows.Forms.RichTextBox rtbImportConsole;     // transferred to cs code.
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button BtnOpenFolder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.TextBox txtFolderName;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem quickFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dDataFrameLoggerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cADTDataFrameLoggerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cADTDataFrameLoggerToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cADTDataFrameLoggerToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem cADTDataFrameLoggerToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem cADTDataFrameLoggerToolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem cADTDataFrameLoggerToolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem instructionToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableDataViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fillWithTestDataToolStripMenuItem;

    }
}