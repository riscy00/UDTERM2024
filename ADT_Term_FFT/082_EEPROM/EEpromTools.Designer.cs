namespace UDT_Term_FFT
{
    partial class EEpromTools
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
            this.label2 = new System.Windows.Forms.Label();
            this.tbEEPROMPassword = new System.Windows.Forms.TextBox();
            this.btnBulkErase = new System.Windows.Forms.Button();
            this.btnVerifyErase = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRTCGet = new System.Windows.Forms.Button();
            this.btnRTCSend = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDoneClose = new System.Windows.Forms.Button();
            this.cbLogMemOnly = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(15, 21);
            this.label2.MaximumSize = new System.Drawing.Size(56, 13);
            this.label2.MinimumSize = new System.Drawing.Size(56, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "PassWord";
            // 
            // tbEEPROMPassword
            // 
            this.tbEEPROMPassword.BackColor = System.Drawing.Color.Honeydew;
            this.tbEEPROMPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbEEPROMPassword.Location = new System.Drawing.Point(75, 19);
            this.tbEEPROMPassword.Name = "tbEEPROMPassword";
            this.tbEEPROMPassword.Size = new System.Drawing.Size(71, 18);
            this.tbEEPROMPassword.TabIndex = 8;
            this.tbEEPROMPassword.DoubleClick += new System.EventHandler(this.tbEEPROMPassword_DoubleClick);
            // 
            // btnBulkErase
            // 
            this.btnBulkErase.BackColor = System.Drawing.Color.SeaShell;
            this.btnBulkErase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBulkErase.Location = new System.Drawing.Point(10, 16);
            this.btnBulkErase.Name = "btnBulkErase";
            this.btnBulkErase.Size = new System.Drawing.Size(116, 43);
            this.btnBulkErase.TabIndex = 6;
            this.btnBulkErase.Text = "Logger Data Erase";
            this.btnBulkErase.UseVisualStyleBackColor = false;
            this.btnBulkErase.Click += new System.EventHandler(this.btnBulkErase_Click);
            // 
            // btnVerifyErase
            // 
            this.btnVerifyErase.BackColor = System.Drawing.Color.MintCream;
            this.btnVerifyErase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerifyErase.Location = new System.Drawing.Point(10, 65);
            this.btnVerifyErase.Name = "btnVerifyErase";
            this.btnVerifyErase.Size = new System.Drawing.Size(116, 43);
            this.btnVerifyErase.TabIndex = 10;
            this.btnVerifyErase.Text = "Blank Verify";
            this.btnVerifyErase.UseVisualStyleBackColor = false;
            this.btnVerifyErase.Click += new System.EventHandler(this.btnVerifyErase_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.btnVerifyErase);
            this.groupBox1.Controls.Add(this.btnBulkErase);
            this.groupBox1.Location = new System.Drawing.Point(12, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(134, 120);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            // 
            // btnRTCGet
            // 
            this.btnRTCGet.BackColor = System.Drawing.Color.MintCream;
            this.btnRTCGet.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRTCGet.Location = new System.Drawing.Point(9, 94);
            this.btnRTCGet.Name = "btnRTCGet";
            this.btnRTCGet.Size = new System.Drawing.Size(116, 43);
            this.btnRTCGet.TabIndex = 13;
            this.btnRTCGet.Text = "RTC Get TimeStamp";
            this.btnRTCGet.UseVisualStyleBackColor = false;
            this.btnRTCGet.Click += new System.EventHandler(this.btnRTCGet_Click);
            // 
            // btnRTCSend
            // 
            this.btnRTCSend.BackColor = System.Drawing.Color.MintCream;
            this.btnRTCSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRTCSend.Location = new System.Drawing.Point(9, 45);
            this.btnRTCSend.Name = "btnRTCSend";
            this.btnRTCSend.Size = new System.Drawing.Size(116, 43);
            this.btnRTCSend.TabIndex = 12;
            this.btnRTCSend.Text = "RTC Send TimeStamp";
            this.btnRTCSend.UseVisualStyleBackColor = false;
            this.btnRTCSend.Click += new System.EventHandler(this.btnRTCSend_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btnRTCSend);
            this.groupBox2.Controls.Add(this.btnRTCGet);
            this.groupBox2.Location = new System.Drawing.Point(152, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(133, 145);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "(TimeStamp  in UDT)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "RTC = Real Time Clock";
            // 
            // btnDoneClose
            // 
            this.btnDoneClose.Location = new System.Drawing.Point(210, 163);
            this.btnDoneClose.Name = "btnDoneClose";
            this.btnDoneClose.Size = new System.Drawing.Size(75, 23);
            this.btnDoneClose.TabIndex = 15;
            this.btnDoneClose.Text = "Done";
            this.btnDoneClose.UseVisualStyleBackColor = true;
            this.btnDoneClose.Click += new System.EventHandler(this.btnDoneClose_Click);
            // 
            // cbLogMemOnly
            // 
            this.cbLogMemOnly.AutoSize = true;
            this.cbLogMemOnly.Checked = true;
            this.cbLogMemOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLogMemOnly.Location = new System.Drawing.Point(18, 163);
            this.cbLogMemOnly.Name = "cbLogMemOnly";
            this.cbLogMemOnly.Size = new System.Drawing.Size(121, 17);
            this.cbLogMemOnly.TabIndex = 16;
            this.cbLogMemOnly.Text = "Erase LogMem Only";
            this.cbLogMemOnly.UseVisualStyleBackColor = true;
            // 
            // EEpromTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 189);
            this.Controls.Add(this.cbLogMemOnly);
            this.Controls.Add(this.btnDoneClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbEEPROMPassword);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(313, 228);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(313, 228);
            this.Name = "EEpromTools";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Flash & Real Time Clock Manager";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EEpromTools_FormClosing);
            this.Load += new System.EventHandler(this.EEpromTools_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbEEPROMPassword;
        private System.Windows.Forms.Button btnBulkErase;
        private System.Windows.Forms.Button btnVerifyErase;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRTCGet;
        private System.Windows.Forms.Button btnRTCSend;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDoneClose;
        private System.Windows.Forms.CheckBox cbLogMemOnly;
    }
}