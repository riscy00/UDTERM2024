namespace UDT_Term_FFT
{
    partial class USBComPortManager
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
            this.label1 = new System.Windows.Forms.Label();
            this.dgvComList = new System.Windows.Forms.DataGridView();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnRescan = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComList)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(246, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Below are idenified Active COM or FT232RL Ports,";
            // 
            // dgvComList
            // 
            this.dgvComList.AllowUserToAddRows = false;
            this.dgvComList.AllowUserToDeleteRows = false;
            this.dgvComList.AllowUserToResizeColumns = false;
            this.dgvComList.AllowUserToResizeRows = false;
            this.dgvComList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvComList.Location = new System.Drawing.Point(12, 42);
            this.dgvComList.MaximumSize = new System.Drawing.Size(531, 332);
            this.dgvComList.MinimumSize = new System.Drawing.Size(531, 332);
            this.dgvComList.Name = "dgvComList";
            this.dgvComList.ReadOnly = true;
            this.dgvComList.RowHeadersVisible = false;
            this.dgvComList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvComList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvComList.ShowCellErrors = false;
            this.dgvComList.ShowCellToolTips = false;
            this.dgvComList.ShowEditingIcon = false;
            this.dgvComList.ShowRowErrors = false;
            this.dgvComList.Size = new System.Drawing.Size(531, 332);
            this.dgvComList.TabIndex = 2;
            this.dgvComList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvComList_CellClick);
            this.dgvComList.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvComList_CellMouseEnter);
            this.dgvComList.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvComList_CellMouseLeave);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(306, 12);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 3;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnRescan
            // 
            this.btnRescan.Location = new System.Drawing.Point(387, 12);
            this.btnRescan.Name = "btnRescan";
            this.btnRescan.Size = new System.Drawing.Size(75, 23);
            this.btnRescan.TabIndex = 4;
            this.btnRescan.Text = "Rescan";
            this.btnRescan.UseVisualStyleBackColor = true;
            this.btnRescan.Click += new System.EventHandler(this.btnRescan_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(468, 12);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(12, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(213, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Twinkle idenify which device is connected.,";
            // 
            // USBComPortManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 386);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRescan);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.dgvComList);
            this.Controls.Add(this.label1);
            this.Name = "USBComPortManager";
            this.Text = "USBComPortManager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.USBComPortManager_FormClosing);
            this.Load += new System.EventHandler(this.USBComPortManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvComList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvComList;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnRescan;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label2;
    }
}