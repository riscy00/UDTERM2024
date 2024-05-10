namespace UDT_Term_FFT
{
    partial class FFT_DataEditor
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rtbDataBox = new System.Windows.Forms.RichTextBox();
            this.cbx_UpdateFromFFTWindow = new System.Windows.Forms.CheckBox();
            this.btnSendFFTWindow = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbxYScale = new System.Windows.Forms.TextBox();
            this.tbxFFTElement = new System.Windows.Forms.TextBox();
            this.tbxSampleRate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPasteClips = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.rtbDataBox);
            this.groupBox2.Location = new System.Drawing.Point(17, 90);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(548, 284);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "FFT Data Frame ";
            // 
            // rtbDataBox
            // 
            this.rtbDataBox.BackColor = System.Drawing.Color.DarkSlateGray;
            this.rtbDataBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbDataBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbDataBox.Location = new System.Drawing.Point(3, 16);
            this.rtbDataBox.Name = "rtbDataBox";
            this.rtbDataBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rtbDataBox.Size = new System.Drawing.Size(542, 265);
            this.rtbDataBox.TabIndex = 0;
            this.rtbDataBox.Text = "";
            this.rtbDataBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbDataBox_KeyDown);
            // 
            // cbx_UpdateFromFFTWindow
            // 
            this.cbx_UpdateFromFFTWindow.AutoSize = true;
            this.cbx_UpdateFromFFTWindow.BackColor = System.Drawing.Color.Transparent;
            this.cbx_UpdateFromFFTWindow.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbx_UpdateFromFFTWindow.Location = new System.Drawing.Point(20, 22);
            this.cbx_UpdateFromFFTWindow.Name = "cbx_UpdateFromFFTWindow";
            this.cbx_UpdateFromFFTWindow.Size = new System.Drawing.Size(164, 19);
            this.cbx_UpdateFromFFTWindow.TabIndex = 5;
            this.cbx_UpdateFromFFTWindow.Text = "Update From FFT Window";
            this.cbx_UpdateFromFFTWindow.UseVisualStyleBackColor = false;
            this.cbx_UpdateFromFFTWindow.CheckedChanged += new System.EventHandler(this.cbx_UpdateFromFFTWindow_CheckedChanged);
            // 
            // btnSendFFTWindow
            // 
            this.btnSendFFTWindow.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendFFTWindow.Location = new System.Drawing.Point(20, 57);
            this.btnSendFFTWindow.Name = "btnSendFFTWindow";
            this.btnSendFFTWindow.Size = new System.Drawing.Size(82, 23);
            this.btnSendFFTWindow.TabIndex = 6;
            this.btnSendFFTWindow.Text = "Send to FFT";
            this.btnSendFFTWindow.UseVisualStyleBackColor = true;
            this.btnSendFFTWindow.Click += new System.EventHandler(this.btnSendFFTWindow_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.tbxYScale);
            this.groupBox1.Controls.Add(this.tbxFFTElement);
            this.groupBox1.Controls.Add(this.tbxSampleRate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(260, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 82);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // tbxYScale
            // 
            this.tbxYScale.Location = new System.Drawing.Point(86, 57);
            this.tbxYScale.Name = "tbxYScale";
            this.tbxYScale.ReadOnly = true;
            this.tbxYScale.Size = new System.Drawing.Size(100, 20);
            this.tbxYScale.TabIndex = 5;
            // 
            // tbxFFTElement
            // 
            this.tbxFFTElement.Location = new System.Drawing.Point(86, 34);
            this.tbxFFTElement.Name = "tbxFFTElement";
            this.tbxFFTElement.ReadOnly = true;
            this.tbxFFTElement.Size = new System.Drawing.Size(100, 20);
            this.tbxFFTElement.TabIndex = 4;
            // 
            // tbxSampleRate
            // 
            this.tbxSampleRate.Location = new System.Drawing.Point(86, 11);
            this.tbxSampleRate.Name = "tbxSampleRate";
            this.tbxSampleRate.ReadOnly = true;
            this.tbxSampleRate.Size = new System.Drawing.Size(100, 20);
            this.tbxSampleRate.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(7, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Y-Scale";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "FFT Element";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sample Rate";
            // 
            // btnPasteClips
            // 
            this.btnPasteClips.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPasteClips.Location = new System.Drawing.Point(118, 57);
            this.btnPasteClips.Name = "btnPasteClips";
            this.btnPasteClips.Size = new System.Drawing.Size(76, 23);
            this.btnPasteClips.TabIndex = 8;
            this.btnPasteClips.Text = "Paste Clips";
            this.btnPasteClips.UseVisualStyleBackColor = true;
            this.btnPasteClips.Click += new System.EventHandler(this.btnPasteClips_Click);
            // 
            // FFT_DataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_1A;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(582, 386);
            this.Controls.Add(this.btnPasteClips);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSendFFTWindow);
            this.Controls.Add(this.cbx_UpdateFromFFTWindow);
            this.Controls.Add(this.groupBox2);
            this.DoubleBuffered = true;
            this.Name = "FFT_DataEditor";
            this.Text = "FFT Data Viewer & Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FFT_DataEditor_FormClosing);
            this.Load += new System.EventHandler(this.FFT_DataEditor_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox rtbDataBox;
        private System.Windows.Forms.CheckBox cbx_UpdateFromFFTWindow;
        private System.Windows.Forms.Button btnSendFFTWindow;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbxYScale;
        private System.Windows.Forms.TextBox tbxFFTElement;
        private System.Windows.Forms.TextBox tbxSampleRate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPasteClips;
    }
}