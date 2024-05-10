namespace UDT_Term_FFT
{
    partial class ADIS16460
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ADIS16460));
            this.cbADISExportSetup = new System.Windows.Forms.CheckBox();
            this.btnADISCaptureNow = new System.Windows.Forms.Button();
            this.gbADISData = new System.Windows.Forms.GroupBox();
            this.pB_ADIS_Image = new System.Windows.Forms.PictureBox();
            this.btnADISExportSetting = new System.Windows.Forms.Button();
            this.btnADISImportSetting = new System.Windows.Forms.Button();
            this.chADISEnable = new System.Windows.Forms.CheckBox();
            this.gpBasicMath = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbZAxis = new System.Windows.Forms.TextBox();
            this.tbYAxis = new System.Windows.Forms.TextBox();
            this.tbXAxis = new System.Windows.Forms.TextBox();
            this.tboToolFace = new System.Windows.Forms.TextBox();
            this.tboInc = new System.Windows.Forms.TextBox();
            this.tboMag = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbEnableText = new System.Windows.Forms.Label();
            this.clbAccelFormat = new System.Windows.Forms.CheckedListBox();
            this.clbAxisPolicy = new System.Windows.Forms.CheckedListBox();
            this.clbGyroFormat = new System.Windows.Forms.CheckedListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cbAccelMath = new System.Windows.Forms.CheckBox();
            this.cbAdvanceFrame = new System.Windows.Forms.CheckBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.gbSetup = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.gbConnectError = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pB_ADIS_Image)).BeginInit();
            this.gpBasicMath.SuspendLayout();
            this.gbSetup.SuspendLayout();
            this.gbConnectError.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbADISExportSetup
            // 
            this.cbADISExportSetup.AutoSize = true;
            this.cbADISExportSetup.Location = new System.Drawing.Point(485, 341);
            this.cbADISExportSetup.Name = "cbADISExportSetup";
            this.cbADISExportSetup.Size = new System.Drawing.Size(105, 17);
            this.cbADISExportSetup.TabIndex = 88;
            this.cbADISExportSetup.Text = "Inc.Export Setup";
            this.cbADISExportSetup.UseVisualStyleBackColor = true;
            // 
            // btnADISCaptureNow
            // 
            this.btnADISCaptureNow.Location = new System.Drawing.Point(399, 337);
            this.btnADISCaptureNow.Name = "btnADISCaptureNow";
            this.btnADISCaptureNow.Size = new System.Drawing.Size(80, 23);
            this.btnADISCaptureNow.TabIndex = 87;
            this.btnADISCaptureNow.Text = "Capture Now";
            this.btnADISCaptureNow.UseVisualStyleBackColor = true;
            this.btnADISCaptureNow.Click += new System.EventHandler(this.btnADISCaptureNow_Click);
            // 
            // gbADISData
            // 
            this.gbADISData.BackColor = System.Drawing.Color.White;
            this.gbADISData.Location = new System.Drawing.Point(399, 145);
            this.gbADISData.MaximumSize = new System.Drawing.Size(355, 180);
            this.gbADISData.MinimumSize = new System.Drawing.Size(355, 180);
            this.gbADISData.Name = "gbADISData";
            this.gbADISData.Size = new System.Drawing.Size(355, 180);
            this.gbADISData.TabIndex = 86;
            this.gbADISData.TabStop = false;
            this.gbADISData.Text = "Readout";
            // 
            // pB_ADIS_Image
            // 
            this.pB_ADIS_Image.Image = ((System.Drawing.Image)(resources.GetObject("pB_ADIS_Image.Image")));
            this.pB_ADIS_Image.Location = new System.Drawing.Point(5, 127);
            this.pB_ADIS_Image.Name = "pB_ADIS_Image";
            this.pB_ADIS_Image.Size = new System.Drawing.Size(388, 239);
            this.pB_ADIS_Image.TabIndex = 84;
            this.pB_ADIS_Image.TabStop = false;
            // 
            // btnADISExportSetting
            // 
            this.btnADISExportSetting.Location = new System.Drawing.Point(654, 5);
            this.btnADISExportSetting.Name = "btnADISExportSetting";
            this.btnADISExportSetting.Size = new System.Drawing.Size(100, 23);
            this.btnADISExportSetting.TabIndex = 82;
            this.btnADISExportSetting.Text = "Export Setting";
            this.btnADISExportSetting.UseVisualStyleBackColor = true;
            this.btnADISExportSetting.Click += new System.EventHandler(this.btnADISExportSetting_Click);
            // 
            // btnADISImportSetting
            // 
            this.btnADISImportSetting.Location = new System.Drawing.Point(548, 5);
            this.btnADISImportSetting.Name = "btnADISImportSetting";
            this.btnADISImportSetting.Size = new System.Drawing.Size(100, 23);
            this.btnADISImportSetting.TabIndex = 83;
            this.btnADISImportSetting.Text = "Import Setting";
            this.btnADISImportSetting.UseVisualStyleBackColor = true;
            this.btnADISImportSetting.Click += new System.EventHandler(this.btnADISImportSetting_Click);
            // 
            // chADISEnable
            // 
            this.chADISEnable.AutoSize = true;
            this.chADISEnable.Checked = true;
            this.chADISEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chADISEnable.Location = new System.Drawing.Point(483, 9);
            this.chADISEnable.Name = "chADISEnable";
            this.chADISEnable.Size = new System.Drawing.Size(59, 17);
            this.chADISEnable.TabIndex = 81;
            this.chADISEnable.Text = "Enable";
            this.chADISEnable.UseVisualStyleBackColor = true;
            this.chADISEnable.MouseClick += new System.Windows.Forms.MouseEventHandler(this.chADISEnable_MouseClick);
            // 
            // gpBasicMath
            // 
            this.gpBasicMath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.gpBasicMath.Controls.Add(this.label2);
            this.gpBasicMath.Controls.Add(this.label3);
            this.gpBasicMath.Controls.Add(this.label6);
            this.gpBasicMath.Controls.Add(this.tbZAxis);
            this.gpBasicMath.Controls.Add(this.tbYAxis);
            this.gpBasicMath.Controls.Add(this.tbXAxis);
            this.gpBasicMath.Controls.Add(this.tboToolFace);
            this.gpBasicMath.Controls.Add(this.tboInc);
            this.gpBasicMath.Controls.Add(this.tboMag);
            this.gpBasicMath.Controls.Add(this.label8);
            this.gpBasicMath.Controls.Add(this.label7);
            this.gpBasicMath.Controls.Add(this.label1);
            this.gpBasicMath.Location = new System.Drawing.Point(400, 37);
            this.gpBasicMath.Name = "gpBasicMath";
            this.gpBasicMath.Size = new System.Drawing.Size(355, 111);
            this.gpBasicMath.TabIndex = 92;
            this.gpBasicMath.TabStop = false;
            this.gpBasicMath.Text = "Gravity Data";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(247, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 21);
            this.label2.TabIndex = 117;
            this.label2.Text = "Z";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(247, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 21);
            this.label3.TabIndex = 116;
            this.label3.Text = "Y";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(247, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 21);
            this.label6.TabIndex = 115;
            this.label6.Text = "X";
            // 
            // tbZAxis
            // 
            this.tbZAxis.BackColor = System.Drawing.SystemColors.Info;
            this.tbZAxis.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbZAxis.Location = new System.Drawing.Point(269, 75);
            this.tbZAxis.Name = "tbZAxis";
            this.tbZAxis.Size = new System.Drawing.Size(80, 29);
            this.tbZAxis.TabIndex = 114;
            // 
            // tbYAxis
            // 
            this.tbYAxis.BackColor = System.Drawing.SystemColors.Info;
            this.tbYAxis.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbYAxis.Location = new System.Drawing.Point(269, 43);
            this.tbYAxis.Name = "tbYAxis";
            this.tbYAxis.Size = new System.Drawing.Size(80, 29);
            this.tbYAxis.TabIndex = 113;
            // 
            // tbXAxis
            // 
            this.tbXAxis.BackColor = System.Drawing.SystemColors.Info;
            this.tbXAxis.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbXAxis.Location = new System.Drawing.Point(269, 10);
            this.tbXAxis.Name = "tbXAxis";
            this.tbXAxis.Size = new System.Drawing.Size(80, 29);
            this.tbXAxis.TabIndex = 112;
            // 
            // tboToolFace
            // 
            this.tboToolFace.BackColor = System.Drawing.SystemColors.Info;
            this.tboToolFace.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tboToolFace.Location = new System.Drawing.Point(153, 75);
            this.tboToolFace.Name = "tboToolFace";
            this.tboToolFace.Size = new System.Drawing.Size(80, 29);
            this.tboToolFace.TabIndex = 111;
            // 
            // tboInc
            // 
            this.tboInc.BackColor = System.Drawing.SystemColors.Info;
            this.tboInc.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tboInc.Location = new System.Drawing.Point(153, 43);
            this.tboInc.Name = "tboInc";
            this.tboInc.Size = new System.Drawing.Size(80, 29);
            this.tboInc.TabIndex = 110;
            // 
            // tboMag
            // 
            this.tboMag.BackColor = System.Drawing.SystemColors.Info;
            this.tboMag.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tboMag.Location = new System.Drawing.Point(153, 10);
            this.tboMag.Name = "tboMag";
            this.tboMag.Size = new System.Drawing.Size(80, 29);
            this.tboMag.TabIndex = 109;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(70, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 21);
            this.label8.TabIndex = 103;
            this.label8.Text = "ToolFace";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(54, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 21);
            this.label7.TabIndex = 102;
            this.label7.Text = "Inclination";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(53, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 21);
            this.label1.TabIndex = 101;
            this.label1.Text = "Magnitude";
            // 
            // lbEnableText
            // 
            this.lbEnableText.AutoSize = true;
            this.lbEnableText.BackColor = System.Drawing.Color.White;
            this.lbEnableText.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbEnableText.Location = new System.Drawing.Point(539, 5);
            this.lbEnableText.Name = "lbEnableText";
            this.lbEnableText.Size = new System.Drawing.Size(219, 24);
            this.lbEnableText.TabIndex = 107;
            this.lbEnableText.Text = "ADIS16460 is Disabled";
            // 
            // clbAccelFormat
            // 
            this.clbAccelFormat.CheckOnClick = true;
            this.clbAccelFormat.FormattingEnabled = true;
            this.clbAccelFormat.Items.AddRange(new object[] {
            "Raw",
            "G",
            "uG"});
            this.clbAccelFormat.Location = new System.Drawing.Point(94, 32);
            this.clbAccelFormat.Name = "clbAccelFormat";
            this.clbAccelFormat.Size = new System.Drawing.Size(60, 49);
            this.clbAccelFormat.TabIndex = 96;
            this.clbAccelFormat.ThreeDCheckBoxes = true;
            this.clbAccelFormat.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbAccelFormat_ItemCheck);
            // 
            // clbAxisPolicy
            // 
            this.clbAxisPolicy.CheckOnClick = true;
            this.clbAxisPolicy.FormattingEnabled = true;
            this.clbAxisPolicy.Items.AddRange(new object[] {
            "OCG",
            "XYZ",
            "MRC"});
            this.clbAxisPolicy.Location = new System.Drawing.Point(160, 32);
            this.clbAxisPolicy.Name = "clbAxisPolicy";
            this.clbAxisPolicy.Size = new System.Drawing.Size(60, 49);
            this.clbAxisPolicy.TabIndex = 97;
            this.clbAxisPolicy.ThreeDCheckBoxes = true;
            this.clbAxisPolicy.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbAxisPolicy_ItemCheck);
            // 
            // clbGyroFormat
            // 
            this.clbGyroFormat.CheckOnClick = true;
            this.clbGyroFormat.FormattingEnabled = true;
            this.clbGyroFormat.Items.AddRange(new object[] {
            "RAW",
            "Deg/Sec",
            "uDeg/Sec"});
            this.clbGyroFormat.Location = new System.Drawing.Point(8, 32);
            this.clbGyroFormat.Name = "clbGyroFormat";
            this.clbGyroFormat.Size = new System.Drawing.Size(80, 49);
            this.clbGyroFormat.TabIndex = 98;
            this.clbGyroFormat.ThreeDCheckBoxes = true;
            this.clbGyroFormat.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbGyroFormat_ItemCheck);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(163, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 99;
            this.label4.Text = "Axis";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 100;
            this.label5.Text = "Gyro Format";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(91, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(69, 13);
            this.label9.TabIndex = 101;
            this.label9.Text = "Accel Format";
            // 
            // cbAccelMath
            // 
            this.cbAccelMath.AutoSize = true;
            this.cbAccelMath.Location = new System.Drawing.Point(226, 32);
            this.cbAccelMath.Name = "cbAccelMath";
            this.cbAccelMath.Size = new System.Drawing.Size(80, 17);
            this.cbAccelMath.TabIndex = 102;
            this.cbAccelMath.Text = "Accel Math";
            this.cbAccelMath.UseVisualStyleBackColor = true;
            this.cbAccelMath.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cbAccelMath_MouseClick);
            // 
            // cbAdvanceFrame
            // 
            this.cbAdvanceFrame.AutoSize = true;
            this.cbAdvanceFrame.Location = new System.Drawing.Point(226, 51);
            this.cbAdvanceFrame.Name = "cbAdvanceFrame";
            this.cbAdvanceFrame.Size = new System.Drawing.Size(101, 17);
            this.cbAdvanceFrame.TabIndex = 103;
            this.cbAdvanceFrame.Text = "Advance Frame";
            this.cbAdvanceFrame.UseVisualStyleBackColor = true;
            this.cbAdvanceFrame.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cbAdvanceFrame_MouseClick);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(712, 337);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(42, 23);
            this.btnStop.TabIndex = 104;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(663, 337);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(43, 23);
            this.btnRun.TabIndex = 105;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // gbSetup
            // 
            this.gbSetup.Controls.Add(this.label11);
            this.gbSetup.Controls.Add(this.gbConnectError);
            this.gbSetup.Controls.Add(this.cbAdvanceFrame);
            this.gbSetup.Controls.Add(this.cbAccelMath);
            this.gbSetup.Controls.Add(this.label5);
            this.gbSetup.Controls.Add(this.clbAccelFormat);
            this.gbSetup.Controls.Add(this.clbAxisPolicy);
            this.gbSetup.Controls.Add(this.clbGyroFormat);
            this.gbSetup.Controls.Add(this.label4);
            this.gbSetup.Controls.Add(this.label9);
            this.gbSetup.Location = new System.Drawing.Point(5, 5);
            this.gbSetup.Name = "gbSetup";
            this.gbSetup.Size = new System.Drawing.Size(388, 121);
            this.gbSetup.TabIndex = 106;
            this.gbSetup.TabStop = false;
            this.gbSetup.Text = "Setup";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(205, 10);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(177, 13);
            this.label11.TabIndex = 105;
            this.label11.Text = "Select MRC for correct Maths (WIP)";
            // 
            // gbConnectError
            // 
            this.gbConnectError.BackColor = System.Drawing.Color.MistyRose;
            this.gbConnectError.Controls.Add(this.label10);
            this.gbConnectError.Location = new System.Drawing.Point(226, 87);
            this.gbConnectError.Name = "gbConnectError";
            this.gbConnectError.Size = new System.Drawing.Size(156, 29);
            this.gbConnectError.TabIndex = 104;
            this.gbConnectError.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(6, 4);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(144, 21);
            this.label10.TabIndex = 0;
            this.label10.Text = "NOT CONNECTED";
            // 
            // ADIS16460
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.gbSetup);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.gpBasicMath);
            this.Controls.Add(this.cbADISExportSetup);
            this.Controls.Add(this.btnADISCaptureNow);
            this.Controls.Add(this.gbADISData);
            this.Controls.Add(this.pB_ADIS_Image);
            this.Controls.Add(this.btnADISExportSetting);
            this.Controls.Add(this.btnADISImportSetting);
            this.Controls.Add(this.chADISEnable);
            this.Controls.Add(this.lbEnableText);
            this.MaximumSize = new System.Drawing.Size(758, 370);
            this.MinimumSize = new System.Drawing.Size(758, 370);
            this.Name = "ADIS16460";
            this.Size = new System.Drawing.Size(758, 370);
            this.Load += new System.EventHandler(this.ADIS16460_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pB_ADIS_Image)).EndInit();
            this.gpBasicMath.ResumeLayout(false);
            this.gpBasicMath.PerformLayout();
            this.gbSetup.ResumeLayout(false);
            this.gbSetup.PerformLayout();
            this.gbConnectError.ResumeLayout(false);
            this.gbConnectError.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox cbADISExportSetup;
        private System.Windows.Forms.Button btnADISCaptureNow;
        private System.Windows.Forms.GroupBox gbADISData;
        private System.Windows.Forms.PictureBox pB_ADIS_Image;
        private System.Windows.Forms.Button btnADISExportSetting;
        private System.Windows.Forms.Button btnADISImportSetting;
        private System.Windows.Forms.CheckBox chADISEnable;
        private System.Windows.Forms.GroupBox gpBasicMath;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tboToolFace;
        private System.Windows.Forms.TextBox tboInc;
        private System.Windows.Forms.TextBox tboMag;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbZAxis;
        private System.Windows.Forms.TextBox tbYAxis;
        private System.Windows.Forms.TextBox tbXAxis;
        private System.Windows.Forms.CheckedListBox clbAccelFormat;
        private System.Windows.Forms.CheckedListBox clbAxisPolicy;
        private System.Windows.Forms.CheckedListBox clbGyroFormat;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox cbAccelMath;
        private System.Windows.Forms.CheckBox cbAdvanceFrame;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.GroupBox gbSetup;
        private System.Windows.Forms.Label lbEnableText;
        private System.Windows.Forms.GroupBox gbConnectError;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
    }
}
