
namespace UDT_Term_FFT
{
    partial class BG_ClientData
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
            this.BtnOpenFolder = new System.Windows.Forms.Button();
            this.txtBGProjectFilename = new System.Windows.Forms.TextBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvClientData = new System.Windows.Forms.DataGridView();
            this.IDNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.tbDiagCommCheck = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbSensorCheck = new System.Windows.Forms.TextBox();
            this.btnBatteryReset = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbBatteryStatus = new System.Windows.Forms.TextBox();
            this.btnTestNow = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.tbPSUCheck = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCopyReport = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbSamplePeriod = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fbdToolSetup = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientData)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnOpenFolder
            // 
            this.BtnOpenFolder.Location = new System.Drawing.Point(595, 19);
            this.BtnOpenFolder.Name = "BtnOpenFolder";
            this.BtnOpenFolder.Size = new System.Drawing.Size(75, 22);
            this.BtnOpenFolder.TabIndex = 8;
            this.BtnOpenFolder.Text = "Open";
            this.BtnOpenFolder.UseVisualStyleBackColor = true;
            this.BtnOpenFolder.Click += new System.EventHandler(this.BtnOpenFolder_Click);
            // 
            // txtBGProjectFilename
            // 
            this.txtBGProjectFilename.Location = new System.Drawing.Point(107, 19);
            this.txtBGProjectFilename.Name = "txtBGProjectFilename";
            this.txtBGProjectFilename.ReadOnly = true;
            this.txtBGProjectFilename.Size = new System.Drawing.Size(482, 20);
            this.txtBGProjectFilename.TabIndex = 6;
            // 
            // btnFolder
            // 
            this.btnFolder.Location = new System.Drawing.Point(9, 19);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(92, 22);
            this.btnFolder.TabIndex = 5;
            this.btnFolder.Text = "Select Folder";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.BtnOpenFolder);
            this.groupBox1.Controls.Add(this.txtBGProjectFilename);
            this.groupBox1.Controls.Add(this.btnFolder);
            this.groupBox1.Location = new System.Drawing.Point(2, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(676, 56);
            this.groupBox1.TabIndex = 49;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IMPORTANT: SETUP PROJECT/JOB FOLDER FIRST!";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.dgvClientData);
            this.groupBox2.Location = new System.Drawing.Point(2, 59);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(676, 294);
            this.groupBox2.TabIndex = 50;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Client Data";
            // 
            // dgvClientData
            // 
            this.dgvClientData.AllowUserToAddRows = false;
            this.dgvClientData.AllowUserToDeleteRows = false;
            this.dgvClientData.AllowUserToResizeColumns = false;
            this.dgvClientData.AllowUserToResizeRows = false;
            this.dgvClientData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvClientData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IDNo,
            this.CD});
            this.dgvClientData.Location = new System.Drawing.Point(6, 19);
            this.dgvClientData.MultiSelect = false;
            this.dgvClientData.Name = "dgvClientData";
            this.dgvClientData.RowHeadersVisible = false;
            this.dgvClientData.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvClientData.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.dgvClientData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvClientData.Size = new System.Drawing.Size(664, 269);
            this.dgvClientData.StandardTab = true;
            this.dgvClientData.TabIndex = 0;
            // 
            // IDNo
            // 
            this.IDNo.FillWeight = 50F;
            this.IDNo.HeaderText = "CD No";
            this.IDNo.MinimumWidth = 50;
            this.IDNo.Name = "IDNo";
            this.IDNo.ReadOnly = true;
            this.IDNo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.IDNo.ToolTipText = "Client Data Number";
            this.IDNo.Width = 50;
            // 
            // CD
            // 
            this.CD.HeaderText = "Client Data Entry";
            this.CD.MinimumWidth = 700;
            this.CD.Name = "CD";
            this.CD.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.CD.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.CD.ToolTipText = "Text Entry";
            this.CD.Width = 700;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(593, 452);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 23);
            this.btnClose.TabIndex = 63;
            this.btnClose.Text = "Save && Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.BackColor = System.Drawing.Color.Transparent;
            this.groupBox6.Controls.Add(this.groupBox10);
            this.groupBox6.Controls.Add(this.groupBox9);
            this.groupBox6.Controls.Add(this.btnBatteryReset);
            this.groupBox6.Controls.Add(this.groupBox7);
            this.groupBox6.Controls.Add(this.btnTestNow);
            this.groupBox6.Controls.Add(this.groupBox8);
            this.groupBox6.Location = new System.Drawing.Point(3, 352);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(675, 69);
            this.groupBox6.TabIndex = 62;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Basic Tool Diagnostic";
            this.groupBox6.Visible = false;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.tbDiagCommCheck);
            this.groupBox10.Controls.Add(this.label13);
            this.groupBox10.Location = new System.Drawing.Point(7, 15);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(145, 49);
            this.groupBox10.TabIndex = 45;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Comm Check";
            // 
            // tbDiagCommCheck
            // 
            this.tbDiagCommCheck.BackColor = System.Drawing.Color.Honeydew;
            this.tbDiagCommCheck.Location = new System.Drawing.Point(68, 17);
            this.tbDiagCommCheck.Name = "tbDiagCommCheck";
            this.tbDiagCommCheck.Size = new System.Drawing.Size(52, 20);
            this.tbDiagCommCheck.TabIndex = 44;
            this.tbDiagCommCheck.Text = "..";
            this.tbDiagCommCheck.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.Location = new System.Drawing.Point(6, 22);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 13);
            this.label13.TabIndex = 43;
            this.label13.Text = "Pass/Fails";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.label10);
            this.groupBox9.Controls.Add(this.tbSensorCheck);
            this.groupBox9.Location = new System.Drawing.Point(303, 15);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(140, 49);
            this.groupBox9.TabIndex = 45;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Sensor Check";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Location = new System.Drawing.Point(6, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(56, 13);
            this.label10.TabIndex = 42;
            this.label10.Text = "Pass/Fails";
            // 
            // tbSensorCheck
            // 
            this.tbSensorCheck.BackColor = System.Drawing.Color.Honeydew;
            this.tbSensorCheck.Location = new System.Drawing.Point(63, 18);
            this.tbSensorCheck.Name = "tbSensorCheck";
            this.tbSensorCheck.Size = new System.Drawing.Size(52, 20);
            this.tbSensorCheck.TabIndex = 41;
            this.tbSensorCheck.Text = "..";
            this.tbSensorCheck.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnBatteryReset
            // 
            this.btnBatteryReset.Location = new System.Drawing.Point(594, 41);
            this.btnBatteryReset.Name = "btnBatteryReset";
            this.btnBatteryReset.Size = new System.Drawing.Size(75, 23);
            this.btnBatteryReset.TabIndex = 44;
            this.btnBatteryReset.Text = "Bat Reset";
            this.btnBatteryReset.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label11);
            this.groupBox7.Controls.Add(this.tbBatteryStatus);
            this.groupBox7.Location = new System.Drawing.Point(448, 15);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(140, 49);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Battery Status (estimate)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Location = new System.Drawing.Point(6, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(64, 13);
            this.label11.TabIndex = 42;
            this.label11.Text = "Cap Left (%)";
            // 
            // tbBatteryStatus
            // 
            this.tbBatteryStatus.BackColor = System.Drawing.Color.Honeydew;
            this.tbBatteryStatus.Location = new System.Drawing.Point(79, 19);
            this.tbBatteryStatus.Name = "tbBatteryStatus";
            this.tbBatteryStatus.Size = new System.Drawing.Size(52, 20);
            this.tbBatteryStatus.TabIndex = 41;
            this.tbBatteryStatus.Text = "%";
            this.tbBatteryStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnTestNow
            // 
            this.btnTestNow.Location = new System.Drawing.Point(594, 15);
            this.btnTestNow.Name = "btnTestNow";
            this.btnTestNow.Size = new System.Drawing.Size(75, 23);
            this.btnTestNow.TabIndex = 1;
            this.btnTestNow.Text = "Test Now";
            this.btnTestNow.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.tbPSUCheck);
            this.groupBox8.Controls.Add(this.label12);
            this.groupBox8.Location = new System.Drawing.Point(157, 15);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(140, 49);
            this.groupBox8.TabIndex = 43;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "PSU Check";
            // 
            // tbPSUCheck
            // 
            this.tbPSUCheck.BackColor = System.Drawing.Color.Honeydew;
            this.tbPSUCheck.Location = new System.Drawing.Point(68, 17);
            this.tbPSUCheck.Name = "tbPSUCheck";
            this.tbPSUCheck.Size = new System.Drawing.Size(52, 20);
            this.tbPSUCheck.TabIndex = 44;
            this.tbPSUCheck.Text = "..";
            this.tbPSUCheck.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Location = new System.Drawing.Point(6, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 13);
            this.label12.TabIndex = 43;
            this.label12.Text = "Pass/Fails";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(512, 422);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 60;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(593, 423);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 23);
            this.btnSave.TabIndex = 61;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCopyReport
            // 
            this.btnCopyReport.Location = new System.Drawing.Point(512, 452);
            this.btnCopyReport.Name = "btnCopyReport";
            this.btnCopyReport.Size = new System.Drawing.Size(75, 23);
            this.btnCopyReport.TabIndex = 64;
            this.btnCopyReport.Text = "Copy Report";
            this.btnCopyReport.UseVisualStyleBackColor = true;
            this.btnCopyReport.Click += new System.EventHandler(this.btnCopyReport_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.tbSamplePeriod);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(3, 422);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(172, 53);
            this.groupBox3.TabIndex = 65;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Setup";
            // 
            // tbSamplePeriod
            // 
            this.tbSamplePeriod.BackColor = System.Drawing.Color.Honeydew;
            this.tbSamplePeriod.Location = new System.Drawing.Point(115, 24);
            this.tbSamplePeriod.Name = "tbSamplePeriod";
            this.tbSamplePeriod.Size = new System.Drawing.Size(52, 20);
            this.tbSamplePeriod.TabIndex = 46;
            this.tbSamplePeriod.Text = "..";
            this.tbSamplePeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(13, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 45;
            this.label1.Text = "Sample Peroid (Sec)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(176, 428);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(336, 13);
            this.label2.TabIndex = 66;
            this.label2.Text = "IMPORTANT: Client Data/Row entry limited to 125 letters (inc space).";
            // 
            // BG_ToCo_Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 480);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnCopyReport);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "BG_ToCo_Setup";
            this.Text = "BG_ToCo_Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BG_ToCo_Setup_FormClosing);
            this.Load += new System.EventHandler(this.BG_ToCo_Setup_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientData)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnOpenFolder;
        private System.Windows.Forms.TextBox txtBGProjectFilename;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvClientData;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.TextBox tbDiagCommCheck;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbSensorCheck;
        private System.Windows.Forms.Button btnBatteryReset;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbBatteryStatus;
        private System.Windows.Forms.Button btnTestNow;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.TextBox tbPSUCheck;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCopyReport;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbSamplePeriod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FolderBrowserDialog fbdToolSetup;
        private System.Windows.Forms.DataGridViewTextBoxColumn IDNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn CD;
    }
}