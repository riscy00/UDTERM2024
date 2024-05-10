namespace UDT_Term_FFT
{
    partial class FCL3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FCL3));
            this.rbFCL30Raw = new System.Windows.Forms.RadioButton();
            this.rbFCL30XYZ = new System.Windows.Forms.RadioButton();
            this.rbFCL30MRC = new System.Windows.Forms.RadioButton();
            this.rbFCL30OGC = new System.Windows.Forms.RadioButton();
            this.rbFCL30uG = new System.Windows.Forms.RadioButton();
            this.rbFCL30nT = new System.Windows.Forms.RadioButton();
            this.rbFCL30mG = new System.Windows.Forms.RadioButton();
            this.rbFCL30pT = new System.Windows.Forms.RadioButton();
            this.cbFCL30ExportSetup = new System.Windows.Forms.CheckBox();
            this.btnFCL30CaptureNow = new System.Windows.Forms.Button();
            this.gbFCL30Data = new System.Windows.Forms.GroupBox();
            this.labFCL3_PictureGuide = new System.Windows.Forms.Label();
            this.pB_FCL3_Image = new System.Windows.Forms.PictureBox();
            this.btnFCL30ExportSetting = new System.Windows.Forms.Button();
            this.btnFCL30ImportSetting = new System.Windows.Forms.Button();
            this.chFCL30Enable = new System.Windows.Forms.CheckBox();
            this.lbFCL30Disable = new System.Windows.Forms.Label();
            this.gbADCPost = new System.Windows.Forms.GroupBox();
            this.gpBasicMath = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbZAxis = new System.Windows.Forms.TextBox();
            this.tbYAxis = new System.Windows.Forms.TextBox();
            this.tbXAxis = new System.Windows.Forms.TextBox();
            this.tboIntensity = new System.Windows.Forms.TextBox();
            this.tboWestY = new System.Windows.Forms.TextBox();
            this.tboNorthX = new System.Windows.Forms.TextBox();
            this.tboDeclination = new System.Windows.Forms.TextBox();
            this.tboInclination = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.gbFormat = new System.Windows.Forms.GroupBox();
            this.gbAxisPolicy = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pB_FCL3_Image)).BeginInit();
            this.gpBasicMath.SuspendLayout();
            this.gbFormat.SuspendLayout();
            this.gbAxisPolicy.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbFCL30Raw
            // 
            this.rbFCL30Raw.AutoSize = true;
            this.rbFCL30Raw.Location = new System.Drawing.Point(15, 49);
            this.rbFCL30Raw.Name = "rbFCL30Raw";
            this.rbFCL30Raw.Size = new System.Drawing.Size(47, 17);
            this.rbFCL30Raw.TabIndex = 77;
            this.rbFCL30Raw.TabStop = true;
            this.rbFCL30Raw.Text = "Raw";
            this.rbFCL30Raw.UseVisualStyleBackColor = true;
            this.rbFCL30Raw.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rbFCL30Format_MouseClick);
            // 
            // rbFCL30XYZ
            // 
            this.rbFCL30XYZ.AutoSize = true;
            this.rbFCL30XYZ.Location = new System.Drawing.Point(6, 14);
            this.rbFCL30XYZ.Name = "rbFCL30XYZ";
            this.rbFCL30XYZ.Size = new System.Drawing.Size(46, 17);
            this.rbFCL30XYZ.TabIndex = 45;
            this.rbFCL30XYZ.TabStop = true;
            this.rbFCL30XYZ.Text = "XYZ";
            this.rbFCL30XYZ.UseVisualStyleBackColor = true;
            this.rbFCL30XYZ.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rbFCL30Axis_MouseClick);
            // 
            // rbFCL30MRC
            // 
            this.rbFCL30MRC.AutoSize = true;
            this.rbFCL30MRC.Location = new System.Drawing.Point(6, 49);
            this.rbFCL30MRC.Name = "rbFCL30MRC";
            this.rbFCL30MRC.Size = new System.Drawing.Size(49, 17);
            this.rbFCL30MRC.TabIndex = 44;
            this.rbFCL30MRC.TabStop = true;
            this.rbFCL30MRC.Text = "MRC";
            this.rbFCL30MRC.UseVisualStyleBackColor = true;
            this.rbFCL30MRC.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rbFCL30Axis_MouseClick);
            // 
            // rbFCL30OGC
            // 
            this.rbFCL30OGC.AutoSize = true;
            this.rbFCL30OGC.Location = new System.Drawing.Point(6, 32);
            this.rbFCL30OGC.Name = "rbFCL30OGC";
            this.rbFCL30OGC.Size = new System.Drawing.Size(48, 17);
            this.rbFCL30OGC.TabIndex = 43;
            this.rbFCL30OGC.TabStop = true;
            this.rbFCL30OGC.Text = "OGC";
            this.rbFCL30OGC.UseVisualStyleBackColor = true;
            this.rbFCL30OGC.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rbFCL30Axis_MouseClick);
            // 
            // rbFCL30uG
            // 
            this.rbFCL30uG.AutoSize = true;
            this.rbFCL30uG.Location = new System.Drawing.Point(59, 32);
            this.rbFCL30uG.Name = "rbFCL30uG";
            this.rbFCL30uG.Size = new System.Drawing.Size(39, 17);
            this.rbFCL30uG.TabIndex = 7;
            this.rbFCL30uG.TabStop = true;
            this.rbFCL30uG.Text = "uG";
            this.rbFCL30uG.UseVisualStyleBackColor = true;
            this.rbFCL30uG.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rbFCL30Format_MouseClick);
            // 
            // rbFCL30nT
            // 
            this.rbFCL30nT.AutoSize = true;
            this.rbFCL30nT.Location = new System.Drawing.Point(15, 14);
            this.rbFCL30nT.Name = "rbFCL30nT";
            this.rbFCL30nT.Size = new System.Drawing.Size(38, 17);
            this.rbFCL30nT.TabIndex = 6;
            this.rbFCL30nT.TabStop = true;
            this.rbFCL30nT.Text = "nT";
            this.rbFCL30nT.UseVisualStyleBackColor = true;
            this.rbFCL30nT.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rbFCL30Format_MouseClick);
            // 
            // rbFCL30mG
            // 
            this.rbFCL30mG.AutoSize = true;
            this.rbFCL30mG.Location = new System.Drawing.Point(59, 14);
            this.rbFCL30mG.Name = "rbFCL30mG";
            this.rbFCL30mG.Size = new System.Drawing.Size(41, 17);
            this.rbFCL30mG.TabIndex = 5;
            this.rbFCL30mG.TabStop = true;
            this.rbFCL30mG.Text = "mG";
            this.rbFCL30mG.UseVisualStyleBackColor = true;
            this.rbFCL30mG.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rbFCL30Format_MouseClick);
            // 
            // rbFCL30pT
            // 
            this.rbFCL30pT.AutoSize = true;
            this.rbFCL30pT.Location = new System.Drawing.Point(15, 32);
            this.rbFCL30pT.Name = "rbFCL30pT";
            this.rbFCL30pT.Size = new System.Drawing.Size(38, 17);
            this.rbFCL30pT.TabIndex = 4;
            this.rbFCL30pT.TabStop = true;
            this.rbFCL30pT.Text = "pT";
            this.rbFCL30pT.UseVisualStyleBackColor = true;
            this.rbFCL30pT.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rbFCL30Format_MouseClick);
            // 
            // cbFCL30ExportSetup
            // 
            this.cbFCL30ExportSetup.AutoSize = true;
            this.cbFCL30ExportSetup.Location = new System.Drawing.Point(485, 341);
            this.cbFCL30ExportSetup.Name = "cbFCL30ExportSetup";
            this.cbFCL30ExportSetup.Size = new System.Drawing.Size(105, 17);
            this.cbFCL30ExportSetup.TabIndex = 88;
            this.cbFCL30ExportSetup.Text = "Inc.Export Setup";
            this.cbFCL30ExportSetup.UseVisualStyleBackColor = true;
            // 
            // btnFCL30CaptureNow
            // 
            this.btnFCL30CaptureNow.Location = new System.Drawing.Point(399, 337);
            this.btnFCL30CaptureNow.Name = "btnFCL30CaptureNow";
            this.btnFCL30CaptureNow.Size = new System.Drawing.Size(80, 23);
            this.btnFCL30CaptureNow.TabIndex = 87;
            this.btnFCL30CaptureNow.Text = "Capture Now";
            this.btnFCL30CaptureNow.UseVisualStyleBackColor = true;
            this.btnFCL30CaptureNow.Click += new System.EventHandler(this.btnFCL30CaptureNow_Click);
            // 
            // gbFCL30Data
            // 
            this.gbFCL30Data.BackColor = System.Drawing.Color.White;
            this.gbFCL30Data.Location = new System.Drawing.Point(399, 224);
            this.gbFCL30Data.Name = "gbFCL30Data";
            this.gbFCL30Data.Size = new System.Drawing.Size(355, 101);
            this.gbFCL30Data.TabIndex = 86;
            this.gbFCL30Data.TabStop = false;
            this.gbFCL30Data.Text = "Readout";
            // 
            // labFCL3_PictureGuide
            // 
            this.labFCL3_PictureGuide.AutoSize = true;
            this.labFCL3_PictureGuide.Location = new System.Drawing.Point(5, 111);
            this.labFCL3_PictureGuide.Name = "labFCL3_PictureGuide";
            this.labFCL3_PictureGuide.Size = new System.Drawing.Size(148, 13);
            this.labFCL3_PictureGuide.TabIndex = 85;
            this.labFCL3_PictureGuide.Text = "For more Image click below....";
            // 
            // pB_FCL3_Image
            // 
            this.pB_FCL3_Image.Image = ((System.Drawing.Image)(resources.GetObject("pB_FCL3_Image.Image")));
            this.pB_FCL3_Image.Location = new System.Drawing.Point(5, 127);
            this.pB_FCL3_Image.Name = "pB_FCL3_Image";
            this.pB_FCL3_Image.Size = new System.Drawing.Size(388, 239);
            this.pB_FCL3_Image.TabIndex = 84;
            this.pB_FCL3_Image.TabStop = false;
            this.pB_FCL3_Image.Click += new System.EventHandler(this.pB_FCL3_Image_Click);
            // 
            // btnFCL30ExportSetting
            // 
            this.btnFCL30ExportSetting.Location = new System.Drawing.Point(654, 5);
            this.btnFCL30ExportSetting.Name = "btnFCL30ExportSetting";
            this.btnFCL30ExportSetting.Size = new System.Drawing.Size(100, 23);
            this.btnFCL30ExportSetting.TabIndex = 82;
            this.btnFCL30ExportSetting.Text = "Export Setting";
            this.btnFCL30ExportSetting.UseVisualStyleBackColor = true;
            this.btnFCL30ExportSetting.Click += new System.EventHandler(this.btnFCL30ExportSetting_Click);
            // 
            // btnFCL30ImportSetting
            // 
            this.btnFCL30ImportSetting.Location = new System.Drawing.Point(548, 5);
            this.btnFCL30ImportSetting.Name = "btnFCL30ImportSetting";
            this.btnFCL30ImportSetting.Size = new System.Drawing.Size(100, 23);
            this.btnFCL30ImportSetting.TabIndex = 83;
            this.btnFCL30ImportSetting.Text = "Import Setting";
            this.btnFCL30ImportSetting.UseVisualStyleBackColor = true;
            this.btnFCL30ImportSetting.Click += new System.EventHandler(this.btnFCL30ImportSetting_Click);
            // 
            // chFCL30Enable
            // 
            this.chFCL30Enable.AutoSize = true;
            this.chFCL30Enable.Checked = true;
            this.chFCL30Enable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chFCL30Enable.Location = new System.Drawing.Point(483, 9);
            this.chFCL30Enable.Name = "chFCL30Enable";
            this.chFCL30Enable.Size = new System.Drawing.Size(59, 17);
            this.chFCL30Enable.TabIndex = 81;
            this.chFCL30Enable.Text = "Enable";
            this.chFCL30Enable.UseVisualStyleBackColor = true;
            this.chFCL30Enable.MouseClick += new System.Windows.Forms.MouseEventHandler(this.chFCL30Enable_MouseClick);
            // 
            // lbFCL30Disable
            // 
            this.lbFCL30Disable.AutoSize = true;
            this.lbFCL30Disable.BackColor = System.Drawing.Color.White;
            this.lbFCL30Disable.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFCL30Disable.Location = new System.Drawing.Point(559, 5);
            this.lbFCL30Disable.Name = "lbFCL30Disable";
            this.lbFCL30Disable.Size = new System.Drawing.Size(196, 24);
            this.lbFCL30Disable.TabIndex = 90;
            this.lbFCL30Disable.Text = "FCL30-7 is Disabled";
            // 
            // gbADCPost
            // 
            this.gbADCPost.Location = new System.Drawing.Point(399, 141);
            this.gbADCPost.Name = "gbADCPost";
            this.gbADCPost.Size = new System.Drawing.Size(355, 83);
            this.gbADCPost.TabIndex = 91;
            this.gbADCPost.TabStop = false;
            this.gbADCPost.Text = "PostData";
            // 
            // gpBasicMath
            // 
            this.gpBasicMath.Controls.Add(this.label9);
            this.gpBasicMath.Controls.Add(this.label2);
            this.gpBasicMath.Controls.Add(this.label3);
            this.gpBasicMath.Controls.Add(this.label6);
            this.gpBasicMath.Controls.Add(this.tbZAxis);
            this.gpBasicMath.Controls.Add(this.tbYAxis);
            this.gpBasicMath.Controls.Add(this.tbXAxis);
            this.gpBasicMath.Controls.Add(this.tboIntensity);
            this.gpBasicMath.Controls.Add(this.tboWestY);
            this.gpBasicMath.Controls.Add(this.tboNorthX);
            this.gpBasicMath.Controls.Add(this.tboDeclination);
            this.gpBasicMath.Controls.Add(this.tboInclination);
            this.gpBasicMath.Controls.Add(this.label8);
            this.gpBasicMath.Controls.Add(this.label7);
            this.gpBasicMath.Controls.Add(this.label1);
            this.gpBasicMath.Controls.Add(this.label5);
            this.gpBasicMath.Controls.Add(this.label4);
            this.gpBasicMath.Location = new System.Drawing.Point(399, 32);
            this.gpBasicMath.Name = "gpBasicMath";
            this.gpBasicMath.Size = new System.Drawing.Size(355, 111);
            this.gpBasicMath.TabIndex = 92;
            this.gpBasicMath.TabStop = false;
            this.gpBasicMath.Text = "Compass Data (mG)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(98, 13);
            this.label9.TabIndex = 118;
            this.label9.Text = "XYZ via Axis Policy";
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
            // tboIntensity
            // 
            this.tboIntensity.BackColor = System.Drawing.SystemColors.Info;
            this.tboIntensity.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tboIntensity.Location = new System.Drawing.Point(153, 75);
            this.tboIntensity.Name = "tboIntensity";
            this.tboIntensity.Size = new System.Drawing.Size(80, 29);
            this.tboIntensity.TabIndex = 111;
            // 
            // tboWestY
            // 
            this.tboWestY.BackColor = System.Drawing.SystemColors.Info;
            this.tboWestY.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tboWestY.Location = new System.Drawing.Point(153, 43);
            this.tboWestY.Name = "tboWestY";
            this.tboWestY.Size = new System.Drawing.Size(80, 29);
            this.tboWestY.TabIndex = 110;
            // 
            // tboNorthX
            // 
            this.tboNorthX.BackColor = System.Drawing.SystemColors.Info;
            this.tboNorthX.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tboNorthX.Location = new System.Drawing.Point(153, 10);
            this.tboNorthX.Name = "tboNorthX";
            this.tboNorthX.Size = new System.Drawing.Size(80, 29);
            this.tboNorthX.TabIndex = 109;
            // 
            // tboDeclination
            // 
            this.tboDeclination.BackColor = System.Drawing.SystemColors.Info;
            this.tboDeclination.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tboDeclination.Location = new System.Drawing.Point(46, 75);
            this.tboDeclination.Name = "tboDeclination";
            this.tboDeclination.Size = new System.Drawing.Size(66, 29);
            this.tboDeclination.TabIndex = 108;
            // 
            // tboInclination
            // 
            this.tboInclination.BackColor = System.Drawing.SystemColors.Info;
            this.tboInclination.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tboInclination.Location = new System.Drawing.Point(46, 42);
            this.tboInclination.Name = "tboInclination";
            this.tboInclination.Size = new System.Drawing.Size(66, 29);
            this.tboInclination.TabIndex = 107;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(122, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 21);
            this.label8.TabIndex = 103;
            this.label8.Text = "Int";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(113, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 21);
            this.label7.TabIndex = 102;
            this.label7.Text = "W/Y";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(113, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 21);
            this.label1.TabIndex = 101;
            this.label1.Text = "N/X";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(2, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 21);
            this.label5.TabIndex = 100;
            this.label5.Text = "Decl";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(13, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 21);
            this.label4.TabIndex = 99;
            this.label4.Text = "Inc";
            // 
            // gbFormat
            // 
            this.gbFormat.Controls.Add(this.rbFCL30Raw);
            this.gbFormat.Controls.Add(this.rbFCL30pT);
            this.gbFormat.Controls.Add(this.rbFCL30mG);
            this.gbFormat.Controls.Add(this.rbFCL30nT);
            this.gbFormat.Controls.Add(this.rbFCL30uG);
            this.gbFormat.Location = new System.Drawing.Point(8, 5);
            this.gbFormat.Name = "gbFormat";
            this.gbFormat.Size = new System.Drawing.Size(106, 71);
            this.gbFormat.TabIndex = 93;
            this.gbFormat.TabStop = false;
            this.gbFormat.Text = "Format Type";
            // 
            // gbAxisPolicy
            // 
            this.gbAxisPolicy.Controls.Add(this.rbFCL30MRC);
            this.gbAxisPolicy.Controls.Add(this.rbFCL30OGC);
            this.gbAxisPolicy.Controls.Add(this.rbFCL30XYZ);
            this.gbAxisPolicy.Location = new System.Drawing.Point(118, 5);
            this.gbAxisPolicy.Name = "gbAxisPolicy";
            this.gbAxisPolicy.Size = new System.Drawing.Size(58, 71);
            this.gbAxisPolicy.TabIndex = 94;
            this.gbAxisPolicy.TabStop = false;
            this.gbAxisPolicy.Text = "Axis";
            // 
            // FCL3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.gbAxisPolicy);
            this.Controls.Add(this.gbFormat);
            this.Controls.Add(this.gpBasicMath);
            this.Controls.Add(this.gbADCPost);
            this.Controls.Add(this.cbFCL30ExportSetup);
            this.Controls.Add(this.btnFCL30CaptureNow);
            this.Controls.Add(this.gbFCL30Data);
            this.Controls.Add(this.labFCL3_PictureGuide);
            this.Controls.Add(this.pB_FCL3_Image);
            this.Controls.Add(this.btnFCL30ExportSetting);
            this.Controls.Add(this.btnFCL30ImportSetting);
            this.Controls.Add(this.chFCL30Enable);
            this.Controls.Add(this.lbFCL30Disable);
            this.MaximumSize = new System.Drawing.Size(758, 370);
            this.MinimumSize = new System.Drawing.Size(758, 370);
            this.Name = "FCL3";
            this.Size = new System.Drawing.Size(758, 370);
            this.Load += new System.EventHandler(this.FCL30_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pB_FCL3_Image)).EndInit();
            this.gpBasicMath.ResumeLayout(false);
            this.gpBasicMath.PerformLayout();
            this.gbFormat.ResumeLayout(false);
            this.gbFormat.PerformLayout();
            this.gbAxisPolicy.ResumeLayout(false);
            this.gbAxisPolicy.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RadioButton rbFCL30Raw;
        private System.Windows.Forms.RadioButton rbFCL30XYZ;
        private System.Windows.Forms.RadioButton rbFCL30MRC;
        private System.Windows.Forms.RadioButton rbFCL30OGC;
        private System.Windows.Forms.RadioButton rbFCL30uG;
        private System.Windows.Forms.RadioButton rbFCL30nT;
        private System.Windows.Forms.RadioButton rbFCL30mG;
        private System.Windows.Forms.RadioButton rbFCL30pT;
        private System.Windows.Forms.CheckBox cbFCL30ExportSetup;
        private System.Windows.Forms.Button btnFCL30CaptureNow;
        private System.Windows.Forms.GroupBox gbFCL30Data;
        private System.Windows.Forms.Label labFCL3_PictureGuide;
        private System.Windows.Forms.PictureBox pB_FCL3_Image;
        private System.Windows.Forms.Button btnFCL30ExportSetting;
        private System.Windows.Forms.Button btnFCL30ImportSetting;
        private System.Windows.Forms.CheckBox chFCL30Enable;
        private System.Windows.Forms.Label lbFCL30Disable;
        private System.Windows.Forms.GroupBox gbADCPost;
        private System.Windows.Forms.GroupBox gpBasicMath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tboIntensity;
        private System.Windows.Forms.TextBox tboWestY;
        private System.Windows.Forms.TextBox tboNorthX;
        private System.Windows.Forms.TextBox tboDeclination;
        private System.Windows.Forms.TextBox tboInclination;
        private System.Windows.Forms.GroupBox gbFormat;
        private System.Windows.Forms.GroupBox gbAxisPolicy;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbZAxis;
        private System.Windows.Forms.TextBox tbYAxis;
        private System.Windows.Forms.TextBox tbXAxis;
    }
}
