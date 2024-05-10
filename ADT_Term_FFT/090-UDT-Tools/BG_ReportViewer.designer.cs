namespace UDT_Term_FFT
{
    partial class BG_ReportViewer
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
            this.dgvReportView = new System.Windows.Forms.DataGridView();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.BtnOpenFolder = new System.Windows.Forms.Button();
            this.txtFolderName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReportView)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvReportView
            // 
            this.dgvReportView.AllowUserToAddRows = false;
            this.dgvReportView.AllowUserToDeleteRows = false;
            this.dgvReportView.AllowUserToResizeColumns = false;
            this.dgvReportView.AllowUserToResizeRows = false;
            this.dgvReportView.EnableHeadersVisualStyles = false;
            this.dgvReportView.Location = new System.Drawing.Point(6, 39);
            this.dgvReportView.Name = "dgvReportView";
            this.dgvReportView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReportView.Size = new System.Drawing.Size(873, 715);
            this.dgvReportView.StandardTab = true;
            this.dgvReportView.TabIndex = 21;
            this.dgvReportView.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvReportView_CellMouseDown);
            this.dgvReportView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvReportView_KeyDown);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(550, 8);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 22;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Visible = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(631, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 23;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(469, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 24;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // BtnOpenFolder
            // 
            this.BtnOpenFolder.Location = new System.Drawing.Point(6, 7);
            this.BtnOpenFolder.Name = "BtnOpenFolder";
            this.BtnOpenFolder.Size = new System.Drawing.Size(78, 22);
            this.BtnOpenFolder.TabIndex = 26;
            this.BtnOpenFolder.Text = "Open Folder";
            this.BtnOpenFolder.UseVisualStyleBackColor = true;
            this.BtnOpenFolder.Click += new System.EventHandler(this.BtnOpenFolder_Click);
            // 
            // txtFolderName
            // 
            this.txtFolderName.Location = new System.Drawing.Point(90, 9);
            this.txtFolderName.Name = "txtFolderName";
            this.txtFolderName.ReadOnly = true;
            this.txtFolderName.Size = new System.Drawing.Size(373, 20);
            this.txtFolderName.TabIndex = 25;
            // 
            // BG_ReportViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 761);
            this.Controls.Add(this.BtnOpenFolder);
            this.Controls.Add(this.txtFolderName);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.dgvReportView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(900, 800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(900, 800);
            this.Name = "BG_ReportViewer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "BG Report Frame Viewer & Import/Export";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BG_ReportView_FormClosing);
            this.Load += new System.EventHandler(this.BG_ReportView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReportView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.DataGridView dgvReportView;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button BtnOpenFolder;
        private System.Windows.Forms.TextBox txtFolderName;
    }
}