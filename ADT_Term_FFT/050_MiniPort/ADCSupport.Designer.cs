namespace UDT_Term_FFT
{
    partial class ADCSupport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ADCSupport));
            this.gbADCPost = new System.Windows.Forms.GroupBox();
            this.btnADCExportSetup = new System.Windows.Forms.Button();
            this.btnADC24ImportSetup = new System.Windows.Forms.Button();
            this.gbUserGuide = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.lbAD7794Disable = new System.Windows.Forms.Label();
            this.gbUserGuide.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbADCPost
            // 
            this.gbADCPost.Location = new System.Drawing.Point(13, 8);
            this.gbADCPost.Name = "gbADCPost";
            this.gbADCPost.Size = new System.Drawing.Size(370, 224);
            this.gbADCPost.TabIndex = 1;
            this.gbADCPost.TabStop = false;
            this.gbADCPost.Text = "PostData";
            // 
            // btnADCExportSetup
            // 
            this.btnADCExportSetup.Location = new System.Drawing.Point(651, 7);
            this.btnADCExportSetup.Name = "btnADCExportSetup";
            this.btnADCExportSetup.Size = new System.Drawing.Size(100, 23);
            this.btnADCExportSetup.TabIndex = 72;
            this.btnADCExportSetup.Text = "Export Setup";
            this.btnADCExportSetup.UseVisualStyleBackColor = true;
            this.btnADCExportSetup.Click += new System.EventHandler(this.btnADCExportSetup_Click);
            // 
            // btnADC24ImportSetup
            // 
            this.btnADC24ImportSetup.Location = new System.Drawing.Point(545, 7);
            this.btnADC24ImportSetup.Name = "btnADC24ImportSetup";
            this.btnADC24ImportSetup.Size = new System.Drawing.Size(100, 23);
            this.btnADC24ImportSetup.TabIndex = 73;
            this.btnADC24ImportSetup.Text = "Import Setup";
            this.btnADC24ImportSetup.UseVisualStyleBackColor = true;
            this.btnADC24ImportSetup.Click += new System.EventHandler(this.btnADC24ImportSetup_Click);
            // 
            // gbUserGuide
            // 
            this.gbUserGuide.Controls.Add(this.richTextBox1);
            this.gbUserGuide.Location = new System.Drawing.Point(13, 238);
            this.gbUserGuide.Name = "gbUserGuide";
            this.gbUserGuide.Size = new System.Drawing.Size(370, 119);
            this.gbUserGuide.TabIndex = 74;
            this.gbUserGuide.TabStop = false;
            this.gbUserGuide.Text = "PostData: Offset/Gain User Guide";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(6, 15);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(358, 97);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // lbAD7794Disable
            // 
            this.lbAD7794Disable.AutoSize = true;
            this.lbAD7794Disable.BackColor = System.Drawing.Color.White;
            this.lbAD7794Disable.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAD7794Disable.Location = new System.Drawing.Point(558, 7);
            this.lbAD7794Disable.Name = "lbAD7794Disable";
            this.lbAD7794Disable.Size = new System.Drawing.Size(190, 24);
            this.lbAD7794Disable.TabIndex = 91;
            this.lbAD7794Disable.Text = "AD7794 is Disabled";
            // 
            // ADCSupport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.gbUserGuide);
            this.Controls.Add(this.btnADCExportSetup);
            this.Controls.Add(this.btnADC24ImportSetup);
            this.Controls.Add(this.gbADCPost);
            this.Controls.Add(this.lbAD7794Disable);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(758, 370);
            this.MinimumSize = new System.Drawing.Size(758, 370);
            this.Name = "ADCSupport";
            this.Size = new System.Drawing.Size(758, 370);
            this.Load += new System.EventHandler(this.ADCSupport_Load);
            this.gbUserGuide.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbADCPost;
        private System.Windows.Forms.Button btnADCExportSetup;
        private System.Windows.Forms.Button btnADC24ImportSetup;
        private System.Windows.Forms.GroupBox gbUserGuide;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label lbAD7794Disable;
    }
}
