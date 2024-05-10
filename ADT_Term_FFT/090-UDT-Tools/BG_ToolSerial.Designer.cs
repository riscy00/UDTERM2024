namespace UDT_Term_FFT
{
    partial class BG_ToolSerial
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
            this.btnLoadData = new System.Windows.Forms.Button();
            this.btnValidate = new System.Windows.Forms.Button();
            this.btnWriteAll = new System.Windows.Forms.Button();
            this.btnErase = new System.Windows.Forms.Button();
            this.cbEraseSerialNo = new System.Windows.Forms.CheckBox();
            this.txbSerialNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txbBoardNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txbBoardRev = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txbConfig1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txbConfig2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txbConfig3 = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnDefault = new System.Windows.Forms.Button();
            this.cbSelectTOCO = new System.Windows.Forms.CheckBox();
            this.cbSelectTS = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnLoadData
            // 
            this.btnLoadData.Location = new System.Drawing.Point(277, 11);
            this.btnLoadData.Name = "btnLoadData";
            this.btnLoadData.Size = new System.Drawing.Size(75, 23);
            this.btnLoadData.TabIndex = 0;
            this.btnLoadData.Text = "LoadData";
            this.btnLoadData.UseVisualStyleBackColor = true;
            this.btnLoadData.Click += new System.EventHandler(this.btnLoadData_Click);
            // 
            // btnValidate
            // 
            this.btnValidate.Location = new System.Drawing.Point(277, 40);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(75, 23);
            this.btnValidate.TabIndex = 1;
            this.btnValidate.Text = "Validate";
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // btnWriteAll
            // 
            this.btnWriteAll.Location = new System.Drawing.Point(277, 146);
            this.btnWriteAll.Name = "btnWriteAll";
            this.btnWriteAll.Size = new System.Drawing.Size(75, 23);
            this.btnWriteAll.TabIndex = 2;
            this.btnWriteAll.Text = "Write All";
            this.btnWriteAll.UseVisualStyleBackColor = true;
            this.btnWriteAll.Click += new System.EventHandler(this.btnWriteAll_Click);
            // 
            // btnErase
            // 
            this.btnErase.Location = new System.Drawing.Point(277, 117);
            this.btnErase.Name = "btnErase";
            this.btnErase.Size = new System.Drawing.Size(75, 23);
            this.btnErase.TabIndex = 3;
            this.btnErase.Text = "Erase/Reset";
            this.btnErase.UseVisualStyleBackColor = true;
            this.btnErase.Click += new System.EventHandler(this.btnErase_Click);
            // 
            // cbEraseSerialNo
            // 
            this.cbEraseSerialNo.AutoSize = true;
            this.cbEraseSerialNo.BackColor = System.Drawing.Color.Transparent;
            this.cbEraseSerialNo.Location = new System.Drawing.Point(180, 12);
            this.cbEraseSerialNo.Name = "cbEraseSerialNo";
            this.cbEraseSerialNo.Size = new System.Drawing.Size(99, 17);
            this.cbEraseSerialNo.TabIndex = 4;
            this.cbEraseSerialNo.Text = "Erase Serial No";
            this.cbEraseSerialNo.UseVisualStyleBackColor = false;
            // 
            // txbSerialNo
            // 
            this.txbSerialNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbSerialNo.Location = new System.Drawing.Point(99, 8);
            this.txbSerialNo.Name = "txbSerialNo";
            this.txbSerialNo.Size = new System.Drawing.Size(75, 22);
            this.txbSerialNo.TabIndex = 5;
            this.txbSerialNo.Text = "2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "Serial No";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Board PN";
            // 
            // txbBoardNo
            // 
            this.txbBoardNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbBoardNo.Location = new System.Drawing.Point(99, 37);
            this.txbBoardNo.Name = "txbBoardNo";
            this.txbBoardNo.ReadOnly = true;
            this.txbBoardNo.Size = new System.Drawing.Size(75, 22);
            this.txbBoardNo.TabIndex = 7;
            this.txbBoardNo.Text = "0x700141";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "Board Rev";
            // 
            // txbBoardRev
            // 
            this.txbBoardRev.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbBoardRev.Location = new System.Drawing.Point(99, 84);
            this.txbBoardRev.Name = "txbBoardRev";
            this.txbBoardRev.Size = new System.Drawing.Size(75, 22);
            this.txbBoardRev.TabIndex = 9;
            this.txbBoardRev.Text = "0x040";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 16);
            this.label4.TabIndex = 12;
            this.label4.Text = "Config1";
            // 
            // txbConfig1
            // 
            this.txbConfig1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbConfig1.Location = new System.Drawing.Point(99, 113);
            this.txbConfig1.Name = "txbConfig1";
            this.txbConfig1.Size = new System.Drawing.Size(136, 22);
            this.txbConfig1.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 146);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 16);
            this.label5.TabIndex = 14;
            this.label5.Text = "Config2";
            // 
            // txbConfig2
            // 
            this.txbConfig2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbConfig2.Location = new System.Drawing.Point(99, 143);
            this.txbConfig2.Name = "txbConfig2";
            this.txbConfig2.Size = new System.Drawing.Size(136, 22);
            this.txbConfig2.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 174);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 16);
            this.label6.TabIndex = 16;
            this.label6.Text = "Config3";
            // 
            // txbConfig3
            // 
            this.txbConfig3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbConfig3.Location = new System.Drawing.Point(99, 171);
            this.txbConfig3.Name = "txbConfig3";
            this.txbConfig3.Size = new System.Drawing.Size(136, 22);
            this.txbConfig3.TabIndex = 15;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(277, 212);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 17;
            this.btnClose.Text = "Close/Quit";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(12, 201);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(218, 16);
            this.label7.TabIndex = 18;
            this.label7.Text = "IMPORTANT: All Data must be";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(12, 220);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(214, 16);
            this.label8.TabIndex = 19;
            this.label8.Text = "Hex Format, except Serial No.";
            // 
            // btnDefault
            // 
            this.btnDefault.Location = new System.Drawing.Point(277, 67);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(75, 23);
            this.btnDefault.TabIndex = 20;
            this.btnDefault.Text = "Default";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // cbSelectTOCO
            // 
            this.cbSelectTOCO.AutoSize = true;
            this.cbSelectTOCO.BackColor = System.Drawing.Color.Transparent;
            this.cbSelectTOCO.Location = new System.Drawing.Point(180, 39);
            this.cbSelectTOCO.Name = "cbSelectTOCO";
            this.cbSelectTOCO.Size = new System.Drawing.Size(95, 17);
            this.cbSelectTOCO.TabIndex = 21;
            this.cbSelectTOCO.Text = "TOCO 700141";
            this.cbSelectTOCO.UseVisualStyleBackColor = false;
            this.cbSelectTOCO.CheckedChanged += new System.EventHandler(this.cbSelectTOCO_CheckedChanged);
            // 
            // cbSelectTS
            // 
            this.cbSelectTS.AutoSize = true;
            this.cbSelectTS.BackColor = System.Drawing.Color.Transparent;
            this.cbSelectTS.Location = new System.Drawing.Point(180, 57);
            this.cbSelectTS.Name = "cbSelectTS";
            this.cbSelectTS.Size = new System.Drawing.Size(94, 17);
            this.cbSelectTS.TabIndex = 22;
            this.cbSelectTS.Text = "TS      700149";
            this.cbSelectTS.UseVisualStyleBackColor = false;
            this.cbSelectTS.CheckedChanged += new System.EventHandler(this.cbSelectTS_CheckedChanged);
            // 
            // BG_ToolSerial
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImage = global::UDT_Term.Properties.Resources.BG_MainPage_Blank_1B;
            this.ClientSize = new System.Drawing.Size(364, 241);
            this.Controls.Add(this.cbSelectTS);
            this.Controls.Add(this.cbSelectTOCO);
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txbConfig3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txbConfig2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txbConfig1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txbBoardRev);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txbBoardNo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txbSerialNo);
            this.Controls.Add(this.cbEraseSerialNo);
            this.Controls.Add(this.btnErase);
            this.Controls.Add(this.btnWriteAll);
            this.Controls.Add(this.btnValidate);
            this.Controls.Add(this.btnLoadData);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(380, 280);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(380, 280);
            this.Name = "BG_ToolSerial";
            this.Text = "BG_Tool Serial DataSet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BG_ToolSerial_FormClosing);
            this.Load += new System.EventHandler(this.BG_ToolSerial_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoadData;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.Button btnWriteAll;
        private System.Windows.Forms.Button btnErase;
        private System.Windows.Forms.CheckBox cbEraseSerialNo;
        private System.Windows.Forms.TextBox txbSerialNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txbBoardNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txbBoardRev;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txbConfig1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txbConfig2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txbConfig3;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.CheckBox cbSelectTOCO;
        private System.Windows.Forms.CheckBox cbSelectTS;
    }
}