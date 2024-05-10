namespace UDT_Term_FFT
{
    partial class MiniAD7794Setup
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExportAll = new System.Windows.Forms.Button();
            this.btnImportAll = new System.Windows.Forms.Button();
            this.tcMainFeature = new TransparentTabControl();
            this.tpMiniPort = new System.Windows.Forms.TabPage();
            this.tpLoggerCVS = new System.Windows.Forms.TabPage();
            this.tpADC = new System.Windows.Forms.TabPage();
            this.tpAD7794 = new System.Windows.Forms.TabPage();
            this.tpADC12 = new System.Windows.Forms.TabPage();
            this.tpI2C = new System.Windows.Forms.TabPage();
            this.tpSPI = new System.Windows.Forms.TabPage();
            this.tpUART = new System.Windows.Forms.TabPage();
            this.tpADIS16460 = new System.Windows.Forms.TabPage();
            this.tpFLC370 = new System.Windows.Forms.TabPage();
            this.tpLogMemory = new System.Windows.Forms.TabPage();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.ofLoadSetup = new System.Windows.Forms.OpenFileDialog();
            this.btnLoadExportAll = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.tcMainFeature.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.helpToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 34;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectFolderToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.helpToolStripMenuItem.Text = "File";
            // 
            // selectFolderToolStripMenuItem
            // 
            this.selectFolderToolStripMenuItem.Name = "selectFolderToolStripMenuItem";
            this.selectFolderToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.selectFolderToolStripMenuItem.Text = "Select Folder";
            this.selectFolderToolStripMenuItem.Click += new System.EventHandler(this.selectFolderToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem1.Text = "Help";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(692, 429);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 23);
            this.btnClose.TabIndex = 62;
            this.btnClose.Text = "Save && Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(530, 429);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 60;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(611, 429);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 61;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExportAll
            // 
            this.btnExportAll.Location = new System.Drawing.Point(449, 429);
            this.btnExportAll.Name = "btnExportAll";
            this.btnExportAll.Size = new System.Drawing.Size(75, 23);
            this.btnExportAll.TabIndex = 63;
            this.btnExportAll.Text = "Export All";
            this.btnExportAll.UseVisualStyleBackColor = true;
            this.btnExportAll.Click += new System.EventHandler(this.btnExportAll_Click);
            // 
            // btnImportAll
            // 
            this.btnImportAll.Enabled = false;
            this.btnImportAll.Location = new System.Drawing.Point(13, 429);
            this.btnImportAll.Name = "btnImportAll";
            this.btnImportAll.Size = new System.Drawing.Size(75, 23);
            this.btnImportAll.TabIndex = 64;
            this.btnImportAll.Text = "Import All";
            this.btnImportAll.UseVisualStyleBackColor = true;
            this.btnImportAll.Click += new System.EventHandler(this.btnImportAll_Click);
            // 
            // tcMainFeature
            // 
            this.tcMainFeature.Controls.Add(this.tpMiniPort);
            this.tcMainFeature.Controls.Add(this.tpLoggerCVS);
            this.tcMainFeature.Controls.Add(this.tpADC);
            this.tcMainFeature.Controls.Add(this.tpAD7794);
            this.tcMainFeature.Controls.Add(this.tpADC12);
            this.tcMainFeature.Controls.Add(this.tpI2C);
            this.tcMainFeature.Controls.Add(this.tpSPI);
            this.tcMainFeature.Controls.Add(this.tpUART);
            this.tcMainFeature.Controls.Add(this.tpADIS16460);
            this.tcMainFeature.Controls.Add(this.tpFLC370);
            this.tcMainFeature.Controls.Add(this.tpLogMemory);
            this.tcMainFeature.Location = new System.Drawing.Point(9, 27);
            this.tcMainFeature.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.tcMainFeature.Name = "tcMainFeature";
            this.tcMainFeature.SelectedIndex = 0;
            this.tcMainFeature.Size = new System.Drawing.Size(766, 396);
            this.tcMainFeature.TabIndex = 33;
            this.tcMainFeature.SelectedIndexChanged += new System.EventHandler(this.tcMainFeature_SelectedIndexChanged);
            // 
            // tpMiniPort
            // 
            this.tpMiniPort.Location = new System.Drawing.Point(4, 22);
            this.tpMiniPort.Name = "tpMiniPort";
            this.tpMiniPort.Size = new System.Drawing.Size(758, 370);
            this.tpMiniPort.TabIndex = 1;
            this.tpMiniPort.Text = "MiniPort";
            this.tpMiniPort.UseVisualStyleBackColor = true;
            // 
            // tpLoggerCVS
            // 
            this.tpLoggerCVS.Location = new System.Drawing.Point(4, 22);
            this.tpLoggerCVS.Name = "tpLoggerCVS";
            this.tpLoggerCVS.Padding = new System.Windows.Forms.Padding(3);
            this.tpLoggerCVS.Size = new System.Drawing.Size(758, 370);
            this.tpLoggerCVS.TabIndex = 4;
            this.tpLoggerCVS.Text = "LoggerCVS";
            this.tpLoggerCVS.UseVisualStyleBackColor = true;
            // 
            // tpADC
            // 
            this.tpADC.Location = new System.Drawing.Point(4, 22);
            this.tpADC.Name = "tpADC";
            this.tpADC.Size = new System.Drawing.Size(758, 370);
            this.tpADC.TabIndex = 10;
            this.tpADC.Text = "ADC Support";
            this.tpADC.UseVisualStyleBackColor = true;
            // 
            // tpAD7794
            // 
            this.tpAD7794.Location = new System.Drawing.Point(4, 22);
            this.tpAD7794.Name = "tpAD7794";
            this.tpAD7794.Padding = new System.Windows.Forms.Padding(3);
            this.tpAD7794.Size = new System.Drawing.Size(758, 370);
            this.tpAD7794.TabIndex = 0;
            this.tpAD7794.Text = "ADC24/AD7794";
            this.tpAD7794.UseVisualStyleBackColor = true;
            // 
            // tpADC12
            // 
            this.tpADC12.Location = new System.Drawing.Point(4, 22);
            this.tpADC12.Name = "tpADC12";
            this.tpADC12.Size = new System.Drawing.Size(758, 370);
            this.tpADC12.TabIndex = 5;
            this.tpADC12.Text = "ADC12";
            this.tpADC12.UseVisualStyleBackColor = true;
            // 
            // tpI2C
            // 
            this.tpI2C.Location = new System.Drawing.Point(4, 22);
            this.tpI2C.Name = "tpI2C";
            this.tpI2C.Size = new System.Drawing.Size(758, 370);
            this.tpI2C.TabIndex = 6;
            this.tpI2C.Text = "I2C";
            this.tpI2C.UseVisualStyleBackColor = true;
            // 
            // tpSPI
            // 
            this.tpSPI.Location = new System.Drawing.Point(4, 22);
            this.tpSPI.Name = "tpSPI";
            this.tpSPI.Size = new System.Drawing.Size(758, 370);
            this.tpSPI.TabIndex = 7;
            this.tpSPI.Text = "SPI";
            this.tpSPI.UseVisualStyleBackColor = true;
            // 
            // tpUART
            // 
            this.tpUART.Location = new System.Drawing.Point(4, 22);
            this.tpUART.Name = "tpUART";
            this.tpUART.Size = new System.Drawing.Size(758, 370);
            this.tpUART.TabIndex = 8;
            this.tpUART.Text = "UART";
            this.tpUART.UseVisualStyleBackColor = true;
            // 
            // tpADIS16460
            // 
            this.tpADIS16460.Location = new System.Drawing.Point(4, 22);
            this.tpADIS16460.Name = "tpADIS16460";
            this.tpADIS16460.Size = new System.Drawing.Size(758, 370);
            this.tpADIS16460.TabIndex = 2;
            this.tpADIS16460.Text = "ADIS16460";
            this.tpADIS16460.UseVisualStyleBackColor = true;
            // 
            // tpFLC370
            // 
            this.tpFLC370.Location = new System.Drawing.Point(4, 22);
            this.tpFLC370.Name = "tpFLC370";
            this.tpFLC370.Size = new System.Drawing.Size(758, 370);
            this.tpFLC370.TabIndex = 3;
            this.tpFLC370.Text = "FLC3-70";
            this.tpFLC370.UseVisualStyleBackColor = true;
            // 
            // tpLogMemory
            // 
            this.tpLogMemory.Location = new System.Drawing.Point(4, 22);
            this.tpLogMemory.Name = "tpLogMemory";
            this.tpLogMemory.Size = new System.Drawing.Size(758, 370);
            this.tpLogMemory.TabIndex = 9;
            this.tpLogMemory.Text = "Log-Memory";
            this.tpLogMemory.UseVisualStyleBackColor = true;
            // 
            // ofLoadSetup
            // 
            this.ofLoadSetup.FileName = "ofSaveSetup";
            // 
            // btnLoadExportAll
            // 
            this.btnLoadExportAll.Location = new System.Drawing.Point(352, 429);
            this.btnLoadExportAll.Name = "btnLoadExportAll";
            this.btnLoadExportAll.Size = new System.Drawing.Size(91, 23);
            this.btnLoadExportAll.TabIndex = 65;
            this.btnLoadExportAll.Text = "Load+Export All";
            this.btnLoadExportAll.UseVisualStyleBackColor = true;
            this.btnLoadExportAll.Click += new System.EventHandler(this.btnLoadExportAll_Click);
            // 
            // MiniAD7794Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPageTVBBlank_1B;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.btnLoadExportAll);
            this.Controls.Add(this.btnImportAll);
            this.Controls.Add(this.btnExportAll);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tcMainFeature);
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "MiniAD7794Setup";
            this.Text = "MiniAD7794Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MiniAD7794Setup_FormClosing);
            this.Load += new System.EventHandler(this.MiniAD7794Setup_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tcMainFeature.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TransparentTabControl tcMainFeature;
        private System.Windows.Forms.TabPage tpAD7794;
        private System.Windows.Forms.TabPage tpMiniPort;
        private System.Windows.Forms.TabPage tpADIS16460;
        private System.Windows.Forms.TabPage tpFLC370;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnExportAll;
        private System.Windows.Forms.TabPage tpADC12;
        private System.Windows.Forms.TabPage tpI2C;
        private System.Windows.Forms.TabPage tpSPI;
        private System.Windows.Forms.TabPage tpUART;
        private System.Windows.Forms.TabPage tpLoggerCVS;
        private System.Windows.Forms.TabPage tpLogMemory;
        private System.Windows.Forms.TabPage tpADC;
        private System.Windows.Forms.Button btnImportAll;
        private System.Windows.Forms.ToolStripMenuItem selectFolderToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog ofLoadSetup;
        private System.Windows.Forms.Button btnLoadExportAll;
    }
}