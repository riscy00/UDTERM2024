namespace UDT_Term_FFT
{
    partial class CanBus
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
            this.tbCanID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbDLC = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.D0 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tbDelay = new System.Windows.Forms.TextBox();
            this.tbCycle = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbCounts = new System.Windows.Forms.TextBox();
            this.tbComment = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tbD0D7 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chCycle = new System.Windows.Forms.CheckBox();
            this.lbHexError = new System.Windows.Forms.Label();
            this.gBSDO = new System.Windows.Forms.GroupBox();
            this.btnSDOHelp = new System.Windows.Forms.Button();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.tbBSpecS = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.tbBSpecExp = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.tbSDOSpec = new System.Windows.Forms.TextBox();
            this.tbBSpecNofB = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.tbBSpecCMD = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.tbSDOSub = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.tbSDOIndex = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.gBSDO.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbCanID
            // 
            this.tbCanID.AcceptsReturn = true;
            this.tbCanID.AcceptsTab = true;
            this.tbCanID.Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCanID.Location = new System.Drawing.Point(15, 27);
            this.tbCanID.MaxLength = 3;
            this.tbCanID.Name = "tbCanID";
            this.tbCanID.Size = new System.Drawing.Size(52, 22);
            this.tbCanID.TabIndex = 0;
            this.tbCanID.Text = "000";
            this.tbCanID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbCanID.WordWrap = false;
            this.tbCanID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TbCanID_KeyDown);
            this.tbCanID.Leave += new System.EventHandler(this.TbCanID_Leave);
            this.tbCanID.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TbCanID_PreviewKeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 15);
            this.label1.TabIndex = 200;
            this.label1.Text = "ID: (Hex)";
            // 
            // tbDLC
            // 
            this.tbDLC.AcceptsReturn = true;
            this.tbDLC.AcceptsTab = true;
            this.tbDLC.Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDLC.Location = new System.Drawing.Point(81, 27);
            this.tbDLC.MaxLength = 1;
            this.tbDLC.Name = "tbDLC";
            this.tbDLC.Size = new System.Drawing.Size(28, 22);
            this.tbDLC.TabIndex = 2;
            this.tbDLC.Text = "8";
            this.tbDLC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbDLC.WordWrap = false;
            this.tbDLC.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TbDLC_KeyDown);
            this.tbDLC.Leave += new System.EventHandler(this.TbDLC_Leave);
            this.tbDLC.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TbDLC_PreviewKeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(79, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 15);
            this.label2.TabIndex = 201;
            this.label2.Text = "DLC";
            // 
            // D0
            // 
            this.D0.AutoSize = true;
            this.D0.Location = new System.Drawing.Point(12, 52);
            this.D0.Name = "D0";
            this.D0.Size = new System.Drawing.Size(63, 15);
            this.D0.TabIndex = 202;
            this.D0.Text = "Data(Hex)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 15);
            this.label3.TabIndex = 203;
            this.label3.Text = "D0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(47, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 15);
            this.label4.TabIndex = 204;
            this.label4.Text = "D1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(80, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 15);
            this.label5.TabIndex = 205;
            this.label5.Text = "D2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(110, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 15);
            this.label6.TabIndex = 206;
            this.label6.Text = "D3";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(142, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 15);
            this.label7.TabIndex = 207;
            this.label7.Text = "D4";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(174, 95);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 15);
            this.label8.TabIndex = 208;
            this.label8.Text = "D5";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(206, 95);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(23, 15);
            this.label9.TabIndex = 209;
            this.label9.Text = "D6";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(238, 95);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(23, 15);
            this.label10.TabIndex = 210;
            this.label10.Text = "D7";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(223, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 15);
            this.label11.TabIndex = 211;
            this.label11.Text = "Delay(mS)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(288, 9);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(63, 15);
            this.label12.TabIndex = 212;
            this.label12.Text = "Cycle(mS)";
            // 
            // tbDelay
            // 
            this.tbDelay.AcceptsReturn = true;
            this.tbDelay.AcceptsTab = true;
            this.tbDelay.Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDelay.Location = new System.Drawing.Point(224, 27);
            this.tbDelay.MaxLength = 6;
            this.tbDelay.Name = "tbDelay";
            this.tbDelay.Size = new System.Drawing.Size(60, 22);
            this.tbDelay.TabIndex = 20;
            this.tbDelay.Text = "0";
            this.tbDelay.WordWrap = false;
            this.tbDelay.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TbDelay_KeyDown);
            this.tbDelay.Leave += new System.EventHandler(this.TbDelay_Leave);
            this.tbDelay.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TbDelay_PreviewKeyDown);
            // 
            // tbCycle
            // 
            this.tbCycle.AcceptsReturn = true;
            this.tbCycle.AcceptsTab = true;
            this.tbCycle.Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCycle.Location = new System.Drawing.Point(290, 27);
            this.tbCycle.MaxLength = 21;
            this.tbCycle.Name = "tbCycle";
            this.tbCycle.Size = new System.Drawing.Size(60, 22);
            this.tbCycle.TabIndex = 24;
            this.tbCycle.Text = "1000";
            this.tbCycle.WordWrap = false;
            this.tbCycle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TbCycle_KeyDown);
            this.tbCycle.Leave += new System.EventHandler(this.TbCycle_Leave);
            this.tbCycle.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TbCycle_PreviewKeyDown);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(351, 9);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(45, 15);
            this.label13.TabIndex = 213;
            this.label13.Text = "Counts";
            // 
            // tbCounts
            // 
            this.tbCounts.AcceptsReturn = true;
            this.tbCounts.AcceptsTab = true;
            this.tbCounts.Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCounts.Location = new System.Drawing.Point(357, 27);
            this.tbCounts.MaxLength = 4;
            this.tbCounts.Name = "tbCounts";
            this.tbCounts.Size = new System.Drawing.Size(39, 22);
            this.tbCounts.TabIndex = 25;
            this.tbCounts.Text = "0";
            this.tbCounts.WordWrap = false;
            this.tbCounts.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TbCounts_KeyDown);
            this.tbCounts.Leave += new System.EventHandler(this.TbCounts_Leave);
            this.tbCounts.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TbCounts_PreviewKeyDown);
            // 
            // tbComment
            // 
            this.tbComment.AcceptsReturn = true;
            this.tbComment.AcceptsTab = true;
            this.tbComment.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbComment.Location = new System.Drawing.Point(15, 130);
            this.tbComment.MaxLength = 6;
            this.tbComment.Name = "tbComment";
            this.tbComment.Size = new System.Drawing.Size(245, 21);
            this.tbComment.TabIndex = 26;
            this.tbComment.Text = "This is STD Frame";
            this.tbComment.WordWrap = false;
            this.tbComment.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TbComment_KeyDown);
            this.tbComment.Leave += new System.EventHandler(this.TbComment_Leave);
            this.tbComment.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TbComment_PreviewKeyDown);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 113);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(61, 15);
            this.label14.TabIndex = 214;
            this.label14.Text = "Comment";
            // 
            // tbD0D7
            // 
            this.tbD0D7.AcceptsReturn = true;
            this.tbD0D7.AcceptsTab = true;
            this.tbD0D7.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbD0D7.Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbD0D7.Location = new System.Drawing.Point(271, 70);
            this.tbD0D7.MaxLength = 17;
            this.tbD0D7.Name = "tbD0D7";
            this.tbD0D7.Size = new System.Drawing.Size(126, 22);
            this.tbD0D7.TabIndex = 26;
            this.tbD0D7.Text = "0000000000000000";
            this.tbD0D7.WordWrap = false;
            this.tbD0D7.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TbD0D7_KeyDown);
            this.tbD0D7.Leave += new System.EventHandler(this.TbD0D7_Leave);
            this.tbD0D7.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TbD0D7_PreviewKeyDown);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(293, 52);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(101, 15);
            this.label15.TabIndex = 215;
            this.label15.Text = "Counts=0: Infinite";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(273, 95);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(112, 15);
            this.label16.TabIndex = 216;
            this.label16.Text = "D0-D7 (Hex,64bits)";
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.LightGreen;
            this.btnOK.Location = new System.Drawing.Point(266, 130);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(62, 23);
            this.btnOK.TabIndex = 30;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.LightGreen;
            this.btnCancel.Location = new System.Drawing.Point(334, 130);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 23);
            this.btnCancel.TabIndex = 31;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // chCycle
            // 
            this.chCycle.AutoSize = true;
            this.chCycle.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chCycle.Location = new System.Drawing.Point(117, 28);
            this.chCycle.Name = "chCycle";
            this.chCycle.Size = new System.Drawing.Size(104, 19);
            this.chCycle.TabIndex = 23;
            this.chCycle.Text = "Cycle Enable?";
            this.chCycle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chCycle.UseVisualStyleBackColor = true;
            this.chCycle.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChCycle_MouseClick);
            // 
            // lbHexError
            // 
            this.lbHexError.AutoSize = true;
            this.lbHexError.BackColor = System.Drawing.Color.Yellow;
            this.lbHexError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHexError.ForeColor = System.Drawing.Color.Red;
            this.lbHexError.Location = new System.Drawing.Point(305, 113);
            this.lbHexError.Name = "lbHexError";
            this.lbHexError.Size = new System.Drawing.Size(88, 15);
            this.lbHexError.TabIndex = 217;
            this.lbHexError.Text = "HEX ERROR";
            this.lbHexError.Visible = false;
            // 
            // gBSDO
            // 
            this.gBSDO.Controls.Add(this.btnSDOHelp);
            this.gBSDO.Controls.Add(this.label30);
            this.gBSDO.Controls.Add(this.label29);
            this.gBSDO.Controls.Add(this.label28);
            this.gBSDO.Controls.Add(this.tbBSpecS);
            this.gBSDO.Controls.Add(this.label27);
            this.gBSDO.Controls.Add(this.tbBSpecExp);
            this.gBSDO.Controls.Add(this.label26);
            this.gBSDO.Controls.Add(this.tbSDOSpec);
            this.gBSDO.Controls.Add(this.tbBSpecNofB);
            this.gBSDO.Controls.Add(this.label25);
            this.gBSDO.Controls.Add(this.label24);
            this.gBSDO.Controls.Add(this.tbBSpecCMD);
            this.gBSDO.Controls.Add(this.label20);
            this.gBSDO.Controls.Add(this.label21);
            this.gBSDO.Controls.Add(this.label22);
            this.gBSDO.Controls.Add(this.label23);
            this.gBSDO.Controls.Add(this.label19);
            this.gBSDO.Controls.Add(this.tbSDOSub);
            this.gBSDO.Controls.Add(this.label18);
            this.gBSDO.Controls.Add(this.tbSDOIndex);
            this.gBSDO.Controls.Add(this.label17);
            this.gBSDO.Location = new System.Drawing.Point(12, 157);
            this.gBSDO.Name = "gBSDO";
            this.gBSDO.Size = new System.Drawing.Size(385, 101);
            this.gBSDO.TabIndex = 218;
            this.gBSDO.TabStop = false;
            this.gBSDO.Text = "SDO (All Entry in Hex)";
            // 
            // btnSDOHelp
            // 
            this.btnSDOHelp.Location = new System.Drawing.Point(278, 34);
            this.btnSDOHelp.Name = "btnSDOHelp";
            this.btnSDOHelp.Size = new System.Drawing.Size(100, 23);
            this.btnSDOHelp.TabIndex = 230;
            this.btnSDOHelp.Text = "SDO Manual";
            this.btnSDOHelp.UseVisualStyleBackColor = true;
            this.btnSDOHelp.Click += new System.EventHandler(this.BtnSDOHelp_Click);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(129, 37);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(13, 15);
            this.label30.TabIndex = 229;
            this.label30.Text = "||";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(276, 79);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(110, 15);
            this.label29.TabIndex = 228;
            this.label29.Text = "580h+ID / 600h+ID";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(276, 60);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(102, 15);
            this.label28.TabIndex = 227;
            this.label28.Text = "SDO is Limited to";
            // 
            // tbBSpecS
            // 
            this.tbBSpecS.AcceptsReturn = true;
            this.tbBSpecS.AcceptsTab = true;
            this.tbBSpecS.Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbBSpecS.Location = new System.Drawing.Point(242, 63);
            this.tbBSpecS.MaxLength = 1;
            this.tbBSpecS.Name = "tbBSpecS";
            this.tbBSpecS.ReadOnly = true;
            this.tbBSpecS.Size = new System.Drawing.Size(24, 22);
            this.tbBSpecS.TabIndex = 313;
            this.tbBSpecS.Text = "0";
            this.tbBSpecS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbBSpecS.WordWrap = false;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(216, 66);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(25, 15);
            this.label27.TabIndex = 225;
            this.label27.Text = "S:1";
            // 
            // tbBSpecExp
            // 
            this.tbBSpecExp.AcceptsReturn = true;
            this.tbBSpecExp.AcceptsTab = true;
            this.tbBSpecExp.Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbBSpecExp.Location = new System.Drawing.Point(189, 63);
            this.tbBSpecExp.MaxLength = 1;
            this.tbBSpecExp.Name = "tbBSpecExp";
            this.tbBSpecExp.ReadOnly = true;
            this.tbBSpecExp.Size = new System.Drawing.Size(24, 22);
            this.tbBSpecExp.TabIndex = 312;
            this.tbBSpecExp.Text = "0";
            this.tbBSpecExp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbBSpecExp.WordWrap = false;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(148, 66);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(38, 15);
            this.label26.TabIndex = 223;
            this.label26.Text = "Exp:1";
            // 
            // tbSDOSpec
            // 
            this.tbSDOSpec.AcceptsReturn = true;
            this.tbSDOSpec.AcceptsTab = true;
            this.tbSDOSpec.Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSDOSpec.Location = new System.Drawing.Point(6, 35);
            this.tbSDOSpec.MaxLength = 2;
            this.tbSDOSpec.Name = "tbSDOSpec";
            this.tbSDOSpec.Size = new System.Drawing.Size(23, 22);
            this.tbSDOSpec.TabIndex = 300;
            this.tbSDOSpec.Text = "2B";
            this.tbSDOSpec.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSDOSpec.WordWrap = false;
            this.tbSDOSpec.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TbBSDOSpec_KeyDown);
            this.tbSDOSpec.Leave += new System.EventHandler(this.TbSDOSpec_Leave);
            this.tbSDOSpec.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TbBSDOSpec_PreviewKeyDown);
            // 
            // tbBSpecNofB
            // 
            this.tbBSpecNofB.AcceptsReturn = true;
            this.tbBSpecNofB.AcceptsTab = true;
            this.tbBSpecNofB.Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbBSpecNofB.Location = new System.Drawing.Point(121, 63);
            this.tbBSpecNofB.MaxLength = 1;
            this.tbBSpecNofB.Name = "tbBSpecNofB";
            this.tbBSpecNofB.ReadOnly = true;
            this.tbBSpecNofB.Size = new System.Drawing.Size(24, 22);
            this.tbBSpecNofB.TabIndex = 311;
            this.tbBSpecNofB.Text = "0";
            this.tbBSpecNofB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbBSpecNofB.WordWrap = false;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(78, 66);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(41, 15);
            this.label25.TabIndex = 220;
            this.label25.Text = "NoB:2";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(4, 66);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(45, 15);
            this.label24.TabIndex = 219;
            this.label24.Text = "CMD:3";
            // 
            // tbBSpecCMD
            // 
            this.tbBSpecCMD.AcceptsReturn = true;
            this.tbBSpecCMD.AcceptsTab = true;
            this.tbBSpecCMD.Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbBSpecCMD.Location = new System.Drawing.Point(51, 63);
            this.tbBSpecCMD.MaxLength = 1;
            this.tbBSpecCMD.Name = "tbBSpecCMD";
            this.tbBSpecCMD.ReadOnly = true;
            this.tbBSpecCMD.Size = new System.Drawing.Size(24, 22);
            this.tbBSpecCMD.TabIndex = 310;
            this.tbBSpecCMD.Text = "0";
            this.tbBSpecCMD.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbBSpecCMD.WordWrap = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(243, 17);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(23, 15);
            this.label20.TabIndex = 214;
            this.label20.Text = "D7";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(211, 17);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(23, 15);
            this.label21.TabIndex = 213;
            this.label21.Text = "D6";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(179, 17);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(23, 15);
            this.label22.TabIndex = 212;
            this.label22.Text = "D5";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(147, 17);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(23, 15);
            this.label23.TabIndex = 211;
            this.label23.Text = "D4";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(91, 17);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(29, 15);
            this.label19.TabIndex = 205;
            this.label19.Text = "Sub";
            // 
            // tbSDOSub
            // 
            this.tbSDOSub.AcceptsReturn = true;
            this.tbSDOSub.AcceptsTab = true;
            this.tbSDOSub.Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSDOSub.Location = new System.Drawing.Point(95, 35);
            this.tbSDOSub.MaxLength = 2;
            this.tbSDOSub.Name = "tbSDOSub";
            this.tbSDOSub.Size = new System.Drawing.Size(25, 22);
            this.tbSDOSub.TabIndex = 302;
            this.tbSDOSub.Text = "05";
            this.tbSDOSub.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSDOSub.WordWrap = false;
            this.tbSDOSub.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbSDOSub_KeyDown);
            this.tbSDOSub.Leave += new System.EventHandler(this.TbSDOSub_Leave);
            this.tbSDOSub.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.tbSDOSub_PreviewKeyDown);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(38, 17);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(37, 15);
            this.label18.TabIndex = 203;
            this.label18.Text = "Index";
            // 
            // tbSDOIndex
            // 
            this.tbSDOIndex.AcceptsReturn = true;
            this.tbSDOIndex.Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSDOIndex.Location = new System.Drawing.Point(36, 35);
            this.tbSDOIndex.MaxLength = 4;
            this.tbSDOIndex.Name = "tbSDOIndex";
            this.tbSDOIndex.Size = new System.Drawing.Size(52, 22);
            this.tbSDOIndex.TabIndex = 301;
            this.tbSDOIndex.Text = "4020";
            this.tbSDOIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSDOIndex.WordWrap = false;
            this.tbSDOIndex.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TbBSDOIndex_KeyDown);
            this.tbSDOIndex.Leave += new System.EventHandler(this.TbSDOIndex_Leave);
            this.tbSDOIndex.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TbBSDOIndex_PreviewKeyDown);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(2, 17);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(35, 15);
            this.label17.TabIndex = 201;
            this.label17.Text = "Spec";
            // 
            // CanBus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 263);
            this.Controls.Add(this.gBSDO);
            this.Controls.Add(this.lbHexError);
            this.Controls.Add(this.chCycle);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.tbD0D7);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.tbComment);
            this.Controls.Add(this.tbCounts);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.tbCycle);
            this.Controls.Add(this.tbDelay);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.D0);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbDLC);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbCanID);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(423, 304);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(423, 200);
            this.Name = "CanBus";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Can Frame Editor (Press Tab or Enter to update)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CanBus_FormClosing);
            this.Load += new System.EventHandler(this.CanBus_Load);
            this.gBSDO.ResumeLayout(false);
            this.gBSDO.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbCanID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbDLC;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label D0;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbDelay;
        private System.Windows.Forms.TextBox tbCycle;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbCounts;
        private System.Windows.Forms.TextBox tbComment;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tbD0D7;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chCycle;
        private System.Windows.Forms.Label lbHexError;
        private System.Windows.Forms.GroupBox gBSDO;
        private System.Windows.Forms.Button btnSDOHelp;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox tbBSpecS;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox tbBSpecExp;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox tbSDOSpec;
        private System.Windows.Forms.TextBox tbBSpecNofB;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox tbBSpecCMD;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox tbSDOSub;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox tbSDOIndex;
        private System.Windows.Forms.Label label17;
    }
}