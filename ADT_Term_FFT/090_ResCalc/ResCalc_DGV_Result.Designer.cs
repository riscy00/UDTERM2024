namespace UDT_Term_FFT
{
    partial class ResCalc_DGV_Result
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.dgvResCalc = new System.Windows.Forms.DataGridView();
            this.bsResCalcTable = new System.Windows.Forms.BindingSource(this.components);
            this.btnSaveCVS = new System.Windows.Forms.Button();
            this.sfdSaveResDataDialog = new System.Windows.Forms.SaveFileDialog();
            this.btnSaveClip = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResCalc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsResCalcTable)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(468, 41);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(466, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dgvResCalc
            // 
            this.dgvResCalc.AllowUserToAddRows = false;
            this.dgvResCalc.AllowUserToDeleteRows = false;
            this.dgvResCalc.AllowUserToResizeColumns = false;
            this.dgvResCalc.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.NullValue = null;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvResCalc.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.NullValue = null;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResCalc.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvResCalc.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvResCalc.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvResCalc.Location = new System.Drawing.Point(12, 12);
            this.dgvResCalc.Name = "dgvResCalc";
            this.dgvResCalc.ReadOnly = true;
            this.dgvResCalc.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvResCalc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvResCalc.Size = new System.Drawing.Size(448, 471);
            this.dgvResCalc.TabIndex = 4;
            this.dgvResCalc.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvResCalc_DataError);
            // 
            // btnSaveCVS
            // 
            this.btnSaveCVS.Location = new System.Drawing.Point(468, 86);
            this.btnSaveCVS.Name = "btnSaveCVS";
            this.btnSaveCVS.Size = new System.Drawing.Size(75, 23);
            this.btnSaveCVS.TabIndex = 7;
            this.btnSaveCVS.Text = "Save CVS";
            this.btnSaveCVS.UseVisualStyleBackColor = true;
            this.btnSaveCVS.Click += new System.EventHandler(this.btnSaveCVS_Click);
            // 
            // btnSaveClip
            // 
            this.btnSaveClip.Location = new System.Drawing.Point(468, 157);
            this.btnSaveClip.Name = "btnSaveClip";
            this.btnSaveClip.Size = new System.Drawing.Size(75, 23);
            this.btnSaveClip.TabIndex = 8;
            this.btnSaveClip.Text = "Copy to Clip";
            this.btnSaveClip.UseVisualStyleBackColor = true;
            this.btnSaveClip.Click += new System.EventHandler(this.btnSaveClip_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Location = new System.Drawing.Point(468, 115);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFolder.TabIndex = 9;
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // ResCalc_DGV_Result
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 491);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.btnSaveClip);
            this.Controls.Add(this.btnSaveCVS);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.dgvResCalc);
            this.Name = "ResCalc_DGV_Result";
            this.Text = "ResCalc_DGV_Result";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ResCalc_DGV_Result_FormClosing);
            this.Load += new System.EventHandler(this.ResCalc_DGV_Result_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResCalc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsResCalcTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvResCalc;
        private System.Windows.Forms.BindingSource bsResCalcTable;
        private System.Windows.Forms.Button btnSaveCVS;
        private System.Windows.Forms.SaveFileDialog sfdSaveResDataDialog;
        private System.Windows.Forms.Button btnSaveClip;
        private System.Windows.Forms.Button btnOpenFolder;
    }
}