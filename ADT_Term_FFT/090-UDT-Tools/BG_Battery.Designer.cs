namespace UDT_Term_FFT
{
    partial class BG_Battery
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
            this.lBatState = new System.Windows.Forms.Label();
            this.btnRechargeFromLaptop = new System.Windows.Forms.Button();
            this.btnRechargeFromAdaptor = new System.Windows.Forms.Button();
            this.gbBatteryState = new System.Windows.Forms.GroupBox();
            this.lBatCharging = new System.Windows.Forms.Label();
            this.lVBAT = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbErr6 = new System.Windows.Forms.RadioButton();
            this.rbErr5 = new System.Windows.Forms.RadioButton();
            this.rbErr4 = new System.Windows.Forms.RadioButton();
            this.rbErr3 = new System.Windows.Forms.RadioButton();
            this.rbErr2 = new System.Windows.Forms.RadioButton();
            this.rbErr1 = new System.Windows.Forms.RadioButton();
            this.btnUpdateVBAT = new System.Windows.Forms.Button();
            this.btnBatHelp = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnStopCharging = new System.Windows.Forms.Button();
            this.gbBatteryState.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lBatState
            // 
            this.lBatState.AutoSize = true;
            this.lBatState.BackColor = System.Drawing.Color.Transparent;
            this.lBatState.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lBatState.Location = new System.Drawing.Point(4, 6);
            this.lBatState.Name = "lBatState";
            this.lBatState.Size = new System.Drawing.Size(175, 25);
            this.lBatState.TabIndex = 51;
            this.lBatState.Text = "Battery State: 50%";
            // 
            // btnRechargeFromLaptop
            // 
            this.btnRechargeFromLaptop.Location = new System.Drawing.Point(12, 252);
            this.btnRechargeFromLaptop.Name = "btnRechargeFromLaptop";
            this.btnRechargeFromLaptop.Size = new System.Drawing.Size(75, 34);
            this.btnRechargeFromLaptop.TabIndex = 50;
            this.btnRechargeFromLaptop.Text = "Recharge Now Laptop";
            this.btnRechargeFromLaptop.UseVisualStyleBackColor = true;
            this.btnRechargeFromLaptop.Click += new System.EventHandler(this.btnRechargeFromLaptop_Click);
            // 
            // btnRechargeFromAdaptor
            // 
            this.btnRechargeFromAdaptor.Location = new System.Drawing.Point(181, 252);
            this.btnRechargeFromAdaptor.Name = "btnRechargeFromAdaptor";
            this.btnRechargeFromAdaptor.Size = new System.Drawing.Size(75, 34);
            this.btnRechargeFromAdaptor.TabIndex = 52;
            this.btnRechargeFromAdaptor.Text = "Recharge Via Adaptor";
            this.btnRechargeFromAdaptor.UseVisualStyleBackColor = true;
            this.btnRechargeFromAdaptor.Click += new System.EventHandler(this.btnRechargeFromAdaptor_Click);
            // 
            // gbBatteryState
            // 
            this.gbBatteryState.BackColor = System.Drawing.Color.YellowGreen;
            this.gbBatteryState.Controls.Add(this.lBatCharging);
            this.gbBatteryState.Controls.Add(this.lVBAT);
            this.gbBatteryState.Controls.Add(this.lBatState);
            this.gbBatteryState.Location = new System.Drawing.Point(12, 12);
            this.gbBatteryState.Name = "gbBatteryState";
            this.gbBatteryState.Size = new System.Drawing.Size(244, 88);
            this.gbBatteryState.TabIndex = 53;
            this.gbBatteryState.TabStop = false;
            // 
            // lBatCharging
            // 
            this.lBatCharging.AutoSize = true;
            this.lBatCharging.BackColor = System.Drawing.Color.Transparent;
            this.lBatCharging.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lBatCharging.Location = new System.Drawing.Point(7, 65);
            this.lBatCharging.Name = "lBatCharging";
            this.lBatCharging.Size = new System.Drawing.Size(75, 17);
            this.lBatCharging.TabIndex = 56;
            this.lBatCharging.Text = "CHARGING";
            // 
            // lVBAT
            // 
            this.lVBAT.AutoSize = true;
            this.lVBAT.BackColor = System.Drawing.Color.Transparent;
            this.lVBAT.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lVBAT.Location = new System.Drawing.Point(6, 38);
            this.lVBAT.Name = "lVBAT";
            this.lVBAT.Size = new System.Drawing.Size(131, 21);
            this.lVBAT.TabIndex = 52;
            this.lVBAT.Text = "VBAT = 0000mV";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.rbErr6);
            this.groupBox2.Controls.Add(this.rbErr5);
            this.groupBox2.Controls.Add(this.rbErr4);
            this.groupBox2.Controls.Add(this.rbErr3);
            this.groupBox2.Controls.Add(this.rbErr2);
            this.groupBox2.Controls.Add(this.rbErr1);
            this.groupBox2.Location = new System.Drawing.Point(12, 106);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(244, 140);
            this.groupBox2.TabIndex = 54;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Battery Flag";
            // 
            // rbErr6
            // 
            this.rbErr6.AutoSize = true;
            this.rbErr6.Enabled = false;
            this.rbErr6.Location = new System.Drawing.Point(6, 114);
            this.rbErr6.Name = "rbErr6";
            this.rbErr6.Size = new System.Drawing.Size(181, 17);
            this.rbErr6.TabIndex = 5;
            this.rbErr6.TabStop = true;
            this.rbErr6.Text = "Err: ToCo Busy/Rejected Charge";
            this.rbErr6.UseVisualStyleBackColor = true;
            // 
            // rbErr5
            // 
            this.rbErr5.AutoSize = true;
            this.rbErr5.Enabled = false;
            this.rbErr5.Location = new System.Drawing.Point(6, 95);
            this.rbErr5.Name = "rbErr5";
            this.rbErr5.Size = new System.Drawing.Size(126, 17);
            this.rbErr5.TabIndex = 4;
            this.rbErr5.TabStop = true;
            this.rbErr5.Text = "Err: Defective Battery";
            this.rbErr5.UseVisualStyleBackColor = true;
            // 
            // rbErr4
            // 
            this.rbErr4.AutoSize = true;
            this.rbErr4.Enabled = false;
            this.rbErr4.Location = new System.Drawing.Point(6, 76);
            this.rbErr4.Name = "rbErr4";
            this.rbErr4.Size = new System.Drawing.Size(136, 17);
            this.rbErr4.TabIndex = 3;
            this.rbErr4.TabStop = true;
            this.rbErr4.Text = "Err: Too Hot To Charge";
            this.rbErr4.UseVisualStyleBackColor = true;
            // 
            // rbErr3
            // 
            this.rbErr3.AutoSize = true;
            this.rbErr3.Enabled = false;
            this.rbErr3.Location = new System.Drawing.Point(6, 57);
            this.rbErr3.Name = "rbErr3";
            this.rbErr3.Size = new System.Drawing.Size(140, 17);
            this.rbErr3.TabIndex = 2;
            this.rbErr3.TabStop = true;
            this.rbErr3.Text = "Err: Too Cold To Charge";
            this.rbErr3.UseVisualStyleBackColor = true;
            // 
            // rbErr2
            // 
            this.rbErr2.AutoSize = true;
            this.rbErr2.Enabled = false;
            this.rbErr2.Location = new System.Drawing.Point(6, 38);
            this.rbErr2.Name = "rbErr2";
            this.rbErr2.Size = new System.Drawing.Size(104, 17);
            this.rbErr2.TabIndex = 1;
            this.rbErr2.TabStop = true;
            this.rbErr2.Text = "Err: OverHeating";
            this.rbErr2.UseVisualStyleBackColor = true;
            // 
            // rbErr1
            // 
            this.rbErr1.AutoSize = true;
            this.rbErr1.Enabled = false;
            this.rbErr1.Location = new System.Drawing.Point(6, 19);
            this.rbErr1.Name = "rbErr1";
            this.rbErr1.Size = new System.Drawing.Size(140, 17);
            this.rbErr1.TabIndex = 0;
            this.rbErr1.TabStop = true;
            this.rbErr1.Text = "Err: Interrupted Charging";
            this.rbErr1.UseVisualStyleBackColor = true;
            // 
            // btnUpdateVBAT
            // 
            this.btnUpdateVBAT.Location = new System.Drawing.Point(97, 252);
            this.btnUpdateVBAT.Name = "btnUpdateVBAT";
            this.btnUpdateVBAT.Size = new System.Drawing.Size(75, 34);
            this.btnUpdateVBAT.TabIndex = 55;
            this.btnUpdateVBAT.Text = "Refresh / Update";
            this.btnUpdateVBAT.UseVisualStyleBackColor = true;
            this.btnUpdateVBAT.Click += new System.EventHandler(this.btnUpdateVBAT_Click);
            // 
            // btnBatHelp
            // 
            this.btnBatHelp.Location = new System.Drawing.Point(97, 292);
            this.btnBatHelp.Name = "btnBatHelp";
            this.btnBatHelp.Size = new System.Drawing.Size(75, 27);
            this.btnBatHelp.TabIndex = 56;
            this.btnBatHelp.Text = "Bat Help";
            this.btnBatHelp.UseVisualStyleBackColor = true;
            this.btnBatHelp.Click += new System.EventHandler(this.btnBatHelp_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(181, 292);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 27);
            this.btnClose.TabIndex = 57;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnStopCharging
            // 
            this.btnStopCharging.Location = new System.Drawing.Point(12, 292);
            this.btnStopCharging.Name = "btnStopCharging";
            this.btnStopCharging.Size = new System.Drawing.Size(75, 27);
            this.btnStopCharging.TabIndex = 58;
            this.btnStopCharging.Text = "Stop Charge";
            this.btnStopCharging.UseVisualStyleBackColor = true;
            this.btnStopCharging.Click += new System.EventHandler(this.btnStopCharging_Click);
            // 
            // BG_Battery
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImage = global::UDT_Term.Properties.Resources.BG_MainPage_Blank_1B;
            this.ClientSize = new System.Drawing.Size(268, 324);
            this.Controls.Add(this.btnStopCharging);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnBatHelp);
            this.Controls.Add(this.btnUpdateVBAT);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gbBatteryState);
            this.Controls.Add(this.btnRechargeFromAdaptor);
            this.Controls.Add(this.btnRechargeFromLaptop);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(284, 363);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(284, 363);
            this.Name = "BG_Battery";
            this.Text = "BG Battery Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BG_Battery_FormClosing);
            this.Load += new System.EventHandler(this.BG_Battery_Load);
            this.gbBatteryState.ResumeLayout(false);
            this.gbBatteryState.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lBatState;
        private System.Windows.Forms.Button btnRechargeFromLaptop;
        private System.Windows.Forms.Button btnRechargeFromAdaptor;
        private System.Windows.Forms.GroupBox gbBatteryState;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbErr6;
        private System.Windows.Forms.RadioButton rbErr5;
        private System.Windows.Forms.RadioButton rbErr4;
        private System.Windows.Forms.RadioButton rbErr3;
        private System.Windows.Forms.RadioButton rbErr2;
        private System.Windows.Forms.RadioButton rbErr1;
        private System.Windows.Forms.Button btnUpdateVBAT;
        private System.Windows.Forms.Label lVBAT;
        private System.Windows.Forms.Label lBatCharging;
        private System.Windows.Forms.Button btnBatHelp;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnStopCharging;
    }
}