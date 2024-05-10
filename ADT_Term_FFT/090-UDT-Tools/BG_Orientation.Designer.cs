namespace UDT_Term_FFT
{
    partial class BG_Orientation
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
            this.btnConfirmCoreOrientation = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbAccelMag = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbTarget_Temp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbTarget_Inc = new System.Windows.Forms.TextBox();
            this.tbTarget_TP = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbActual_TP = new System.Windows.Forms.TextBox();
            this.lbMatched = new System.Windows.Forms.Label();
            this.lbClockwise = new System.Windows.Forms.Label();
            this.lbAntiClockwise = new System.Windows.Forms.Label();
            this.pClockWise = new System.Windows.Forms.Panel();
            this.pAntiClockWise = new System.Windows.Forms.Panel();
            this.btnStartNextRun = new System.Windows.Forms.Button();
            this.btnRechargeBattery = new System.Windows.Forms.Button();
            this.btnFinishjob = new System.Windows.Forms.Button();
            this.txToolStatus = new System.Windows.Forms.TextBox();
            this.MathVersion = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConfirmCoreOrientation
            // 
            this.btnConfirmCoreOrientation.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnConfirmCoreOrientation.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirmCoreOrientation.Location = new System.Drawing.Point(12, 218);
            this.btnConfirmCoreOrientation.Name = "btnConfirmCoreOrientation";
            this.btnConfirmCoreOrientation.Size = new System.Drawing.Size(340, 48);
            this.btnConfirmCoreOrientation.TabIndex = 0;
            this.btnConfirmCoreOrientation.Text = "Core Orientation Done!";
            this.btnConfirmCoreOrientation.UseVisualStyleBackColor = true;
            this.btnConfirmCoreOrientation.Click += new System.EventHandler(this.btnConfimCoreOrientation_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.MathVersion);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.tbAccelMag);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbTarget_Temp);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbTarget_Inc);
            this.groupBox1.Controls.Add(this.tbTarget_TP);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(340, 100);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Core Orientation ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(227, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 25);
            this.label8.TabIndex = 8;
            this.label8.Text = "Position";
            // 
            // tbAccelMag
            // 
            this.tbAccelMag.Location = new System.Drawing.Point(232, 26);
            this.tbAccelMag.Name = "tbAccelMag";
            this.tbAccelMag.Size = new System.Drawing.Size(49, 20);
            this.tbAccelMag.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(233, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Mag (G)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(284, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Temp (K)";
            // 
            // tbTarget_Temp
            // 
            this.tbTarget_Temp.Location = new System.Drawing.Point(285, 26);
            this.tbTarget_Temp.Name = "tbTarget_Temp";
            this.tbTarget_Temp.Size = new System.Drawing.Size(49, 20);
            this.tbTarget_Temp.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Inclination";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Core Mark";
            // 
            // tbTarget_Inc
            // 
            this.tbTarget_Inc.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTarget_Inc.Location = new System.Drawing.Point(124, 17);
            this.tbTarget_Inc.Name = "tbTarget_Inc";
            this.tbTarget_Inc.Size = new System.Drawing.Size(99, 31);
            this.tbTarget_Inc.TabIndex = 1;
            // 
            // tbTarget_TP
            // 
            this.tbTarget_TP.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTarget_TP.Location = new System.Drawing.Point(124, 58);
            this.tbTarget_TP.Name = "tbTarget_TP";
            this.tbTarget_TP.Size = new System.Drawing.Size(99, 31);
            this.tbTarget_TP.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.tbActual_TP);
            this.groupBox2.Controls.Add(this.lbMatched);
            this.groupBox2.Controls.Add(this.lbClockwise);
            this.groupBox2.Controls.Add(this.lbAntiClockwise);
            this.groupBox2.Controls.Add(this.pClockWise);
            this.groupBox2.Controls.Add(this.pAntiClockWise);
            this.groupBox2.Location = new System.Drawing.Point(12, 119);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(340, 92);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Surface Core Marking";
            // 
            // tbActual_TP
            // 
            this.tbActual_TP.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbActual_TP.Location = new System.Drawing.Point(124, 19);
            this.tbActual_TP.Name = "tbActual_TP";
            this.tbActual_TP.Size = new System.Drawing.Size(99, 31);
            this.tbActual_TP.TabIndex = 3;
            // 
            // lbMatched
            // 
            this.lbMatched.AutoSize = true;
            this.lbMatched.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMatched.ForeColor = System.Drawing.Color.Blue;
            this.lbMatched.Location = new System.Drawing.Point(128, 60);
            this.lbMatched.Name = "lbMatched";
            this.lbMatched.Size = new System.Drawing.Size(95, 20);
            this.lbMatched.TabIndex = 6;
            this.lbMatched.Text = "MATCHED";
            this.lbMatched.Visible = false;
            // 
            // lbClockwise
            // 
            this.lbClockwise.AutoSize = true;
            this.lbClockwise.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbClockwise.Location = new System.Drawing.Point(105, 62);
            this.lbClockwise.Name = "lbClockwise";
            this.lbClockwise.Size = new System.Drawing.Size(141, 16);
            this.lbClockwise.TabIndex = 4;
            this.lbClockwise.Text = "TURN CLOCKWISE";
            this.lbClockwise.Visible = false;
            // 
            // lbAntiClockwise
            // 
            this.lbAntiClockwise.AutoSize = true;
            this.lbAntiClockwise.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAntiClockwise.Location = new System.Drawing.Point(83, 61);
            this.lbAntiClockwise.Name = "lbAntiClockwise";
            this.lbAntiClockwise.Size = new System.Drawing.Size(181, 16);
            this.lbAntiClockwise.TabIndex = 5;
            this.lbAntiClockwise.Text = "TURN ANTI-CLOCKWISE";
            this.lbAntiClockwise.Visible = false;
            // 
            // pClockWise
            // 
            this.pClockWise.Location = new System.Drawing.Point(226, 19);
            this.pClockWise.Name = "pClockWise";
            this.pClockWise.Size = new System.Drawing.Size(108, 67);
            this.pClockWise.TabIndex = 8;
            // 
            // pAntiClockWise
            // 
            this.pAntiClockWise.Location = new System.Drawing.Point(6, 19);
            this.pAntiClockWise.Name = "pAntiClockWise";
            this.pAntiClockWise.Size = new System.Drawing.Size(116, 67);
            this.pAntiClockWise.TabIndex = 7;
            // 
            // btnStartNextRun
            // 
            this.btnStartNextRun.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStartNextRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartNextRun.Location = new System.Drawing.Point(12, 331);
            this.btnStartNextRun.Name = "btnStartNextRun";
            this.btnStartNextRun.Size = new System.Drawing.Size(340, 48);
            this.btnStartNextRun.TabIndex = 5;
            this.btnStartNextRun.Text = "Start Next Run Now";
            this.btnStartNextRun.UseVisualStyleBackColor = true;
            this.btnStartNextRun.Click += new System.EventHandler(this.btnStartNextRun_Click);
            // 
            // btnRechargeBattery
            // 
            this.btnRechargeBattery.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnRechargeBattery.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRechargeBattery.Location = new System.Drawing.Point(12, 385);
            this.btnRechargeBattery.Name = "btnRechargeBattery";
            this.btnRechargeBattery.Size = new System.Drawing.Size(161, 64);
            this.btnRechargeBattery.TabIndex = 6;
            this.btnRechargeBattery.Text = "Recharge Battery";
            this.btnRechargeBattery.UseVisualStyleBackColor = true;
            this.btnRechargeBattery.Click += new System.EventHandler(this.btnRechargeBattery_Click);
            // 
            // btnFinishjob
            // 
            this.btnFinishjob.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFinishjob.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFinishjob.Location = new System.Drawing.Point(191, 385);
            this.btnFinishjob.Name = "btnFinishjob";
            this.btnFinishjob.Size = new System.Drawing.Size(161, 64);
            this.btnFinishjob.TabIndex = 7;
            this.btnFinishjob.Text = "Finish Job";
            this.btnFinishjob.UseVisualStyleBackColor = true;
            this.btnFinishjob.Click += new System.EventHandler(this.btnFinishjob_Click);
            // 
            // txToolStatus
            // 
            this.txToolStatus.BackColor = System.Drawing.Color.PaleGreen;
            this.txToolStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txToolStatus.Location = new System.Drawing.Point(18, 285);
            this.txToolStatus.Name = "txToolStatus";
            this.txToolStatus.Size = new System.Drawing.Size(328, 29);
            this.txToolStatus.TabIndex = 8;
            this.txToolStatus.Text = "Tool is Ready For Next Run";
            this.txToolStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // MathVersion
            // 
            this.MathVersion.AutoSize = true;
            this.MathVersion.Location = new System.Drawing.Point(297, 49);
            this.MathVersion.Name = "MathVersion";
            this.MathVersion.Size = new System.Drawing.Size(33, 13);
            this.MathVersion.TabIndex = 9;
            this.MathVersion.Text = "SL3A";
            // 
            // BG_Orientation
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImage = global::UDT_Term.Properties.Resources.BG_MainPage_Blank_1B;
            this.ClientSize = new System.Drawing.Size(364, 461);
            this.Controls.Add(this.txToolStatus);
            this.Controls.Add(this.btnFinishjob);
            this.Controls.Add(this.btnRechargeBattery);
            this.Controls.Add(this.btnStartNextRun);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnConfirmCoreOrientation);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(380, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(380, 500);
            this.Name = "BG_Orientation";
            this.Text = "BG_Orientation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BG_Orientation_FormClosing);
            this.Load += new System.EventHandler(this.BG_Orientation_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConfirmCoreOrientation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbTarget_Inc;
        private System.Windows.Forms.TextBox tbTarget_TP;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbTarget_Temp;
        private System.Windows.Forms.Label lbMatched;
        private System.Windows.Forms.Label lbAntiClockwise;
        private System.Windows.Forms.Label lbClockwise;
        private System.Windows.Forms.TextBox tbActual_TP;
        private System.Windows.Forms.TextBox tbAccelMag;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel pClockWise;
        private System.Windows.Forms.Panel pAntiClockWise;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnStartNextRun;
        private System.Windows.Forms.Button btnRechargeBattery;
        private System.Windows.Forms.Button btnFinishjob;
        private System.Windows.Forms.TextBox txToolStatus;
        private System.Windows.Forms.Label MathVersion;
    }
}