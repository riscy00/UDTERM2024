namespace UDT_Term_FFT
{
    partial class BG_Driller
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnBGSurveyTimeStamp = new System.Windows.Forms.Button();
            this.btnBGRight = new System.Windows.Forms.Button();
            this.btnBGLeft = new System.Windows.Forms.Button();
            this.btnBGUp = new System.Windows.Forms.Button();
            this.btnBGDown = new System.Windows.Forms.Button();
            this.btnBGFinish = new System.Windows.Forms.Button();
            this.cbExtendView = new System.Windows.Forms.CheckBox();
            this.cbFeet = new System.Windows.Forms.CheckBox();
            this.cbMeter = new System.Windows.Forms.CheckBox();
            this.tbValue = new System.Windows.Forms.RichTextBox();
            this.rtbNote = new System.Windows.Forms.RichTextBox();
            this.dgvViewList = new System.Windows.Forms.CheckBox();
            this.dgvViewRecords = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvViewRecords)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBGSurveyTimeStamp
            // 
            this.btnBGSurveyTimeStamp.BackColor = System.Drawing.Color.Honeydew;
            this.btnBGSurveyTimeStamp.Font = new System.Drawing.Font("Arial Rounded MT Bold", 21.75F);
            this.btnBGSurveyTimeStamp.Location = new System.Drawing.Point(12, 12);
            this.btnBGSurveyTimeStamp.Name = "btnBGSurveyTimeStamp";
            this.btnBGSurveyTimeStamp.Size = new System.Drawing.Size(241, 207);
            this.btnBGSurveyTimeStamp.TabIndex = 0;
            this.btnBGSurveyTimeStamp.Text = "Survey";
            this.btnBGSurveyTimeStamp.UseVisualStyleBackColor = false;
            this.btnBGSurveyTimeStamp.Click += new System.EventHandler(this.btnBGSurveyTimeStamp_Click);
            // 
            // btnBGRight
            // 
            this.btnBGRight.BackColor = System.Drawing.Color.AntiqueWhite;
            this.btnBGRight.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F);
            this.btnBGRight.Location = new System.Drawing.Point(213, 225);
            this.btnBGRight.Name = "btnBGRight";
            this.btnBGRight.Size = new System.Drawing.Size(40, 135);
            this.btnBGRight.TabIndex = 1;
            this.btnBGRight.Text = "▶";
            this.btnBGRight.UseVisualStyleBackColor = true;
            this.btnBGRight.Click += new System.EventHandler(this.OnClick);
            // 
            // btnBGLeft
            // 
            this.btnBGLeft.BackColor = System.Drawing.Color.AntiqueWhite;
            this.btnBGLeft.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F);
            this.btnBGLeft.Location = new System.Drawing.Point(13, 225);
            this.btnBGLeft.Name = "btnBGLeft";
            this.btnBGLeft.Size = new System.Drawing.Size(40, 135);
            this.btnBGLeft.TabIndex = 3;
            this.btnBGLeft.Text = "◀";
            this.btnBGLeft.UseVisualStyleBackColor = true;
            this.btnBGLeft.Click += new System.EventHandler(this.OnClick);
            // 
            // btnBGUp
            // 
            this.btnBGUp.BackColor = System.Drawing.Color.AntiqueWhite;
            this.btnBGUp.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBGUp.Location = new System.Drawing.Point(59, 225);
            this.btnBGUp.Name = "btnBGUp";
            this.btnBGUp.Size = new System.Drawing.Size(148, 40);
            this.btnBGUp.TabIndex = 4;
            this.btnBGUp.Text = "▲";
            this.btnBGUp.UseVisualStyleBackColor = true;
            this.btnBGUp.Click += new System.EventHandler(this.OnClick);
            // 
            // btnBGDown
            // 
            this.btnBGDown.BackColor = System.Drawing.Color.AntiqueWhite;
            this.btnBGDown.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F);
            this.btnBGDown.Location = new System.Drawing.Point(59, 320);
            this.btnBGDown.Name = "btnBGDown";
            this.btnBGDown.Size = new System.Drawing.Size(148, 40);
            this.btnBGDown.TabIndex = 7;
            this.btnBGDown.Text = "▼";
            this.btnBGDown.UseVisualStyleBackColor = true;
            this.btnBGDown.Click += new System.EventHandler(this.OnClick);
            // 
            // btnBGFinish
            // 
            this.btnBGFinish.BackColor = System.Drawing.Color.MistyRose;
            this.btnBGFinish.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F);
            this.btnBGFinish.Location = new System.Drawing.Point(12, 366);
            this.btnBGFinish.Name = "btnBGFinish";
            this.btnBGFinish.Size = new System.Drawing.Size(119, 47);
            this.btnBGFinish.TabIndex = 8;
            this.btnBGFinish.Text = "FINISH";
            this.btnBGFinish.UseVisualStyleBackColor = false;
            this.btnBGFinish.Click += new System.EventHandler(this.btnBGFinish_Click);
            // 
            // cbExtendView
            // 
            this.cbExtendView.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbExtendView.AutoSize = true;
            this.cbExtendView.FlatAppearance.CheckedBackColor = System.Drawing.Color.Silver;
            this.cbExtendView.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbExtendView.Location = new System.Drawing.Point(171, 390);
            this.cbExtendView.Name = "cbExtendView";
            this.cbExtendView.Size = new System.Drawing.Size(82, 23);
            this.cbExtendView.TabIndex = 9;
            this.cbExtendView.Text = "Extend View  ";
            this.cbExtendView.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbExtendView.UseVisualStyleBackColor = true;
            this.cbExtendView.CheckedChanged += new System.EventHandler(this.cbExtendView_CheckedChanged);
            // 
            // cbFeet
            // 
            this.cbFeet.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbFeet.AutoSize = true;
            this.cbFeet.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbFeet.Location = new System.Drawing.Point(215, 366);
            this.cbFeet.Name = "cbFeet";
            this.cbFeet.Size = new System.Drawing.Size(38, 23);
            this.cbFeet.TabIndex = 10;
            this.cbFeet.Text = "Feet";
            this.cbFeet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbFeet.UseVisualStyleBackColor = true;
            this.cbFeet.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cbFeet_MouseClick);
            // 
            // cbMeter
            // 
            this.cbMeter.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbMeter.AutoSize = true;
            this.cbMeter.Checked = true;
            this.cbMeter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMeter.FlatAppearance.CheckedBackColor = System.Drawing.Color.Silver;
            this.cbMeter.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbMeter.Location = new System.Drawing.Point(171, 366);
            this.cbMeter.Name = "cbMeter";
            this.cbMeter.Size = new System.Drawing.Size(44, 23);
            this.cbMeter.TabIndex = 11;
            this.cbMeter.Text = "Meter";
            this.cbMeter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbMeter.UseVisualStyleBackColor = true;
            this.cbMeter.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cbMeter_MouseClick);
            // 
            // tbValue
            // 
            this.tbValue.BackColor = System.Drawing.Color.Ivory;
            this.tbValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbValue.Font = new System.Drawing.Font("Courier New", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbValue.Location = new System.Drawing.Point(59, 271);
            this.tbValue.Name = "tbValue";
            this.tbValue.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.tbValue.Size = new System.Drawing.Size(148, 43);
            this.tbValue.TabIndex = 12;
            this.tbValue.Text = "";
            // 
            // rtbNote
            // 
            this.rtbNote.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbNote.Location = new System.Drawing.Point(13, 420);
            this.rtbNote.Name = "rtbNote";
            this.rtbNote.Size = new System.Drawing.Size(238, 65);
            this.rtbNote.TabIndex = 13;
            this.rtbNote.Text = "Note Pad";
            // 
            // dgvViewList
            // 
            this.dgvViewList.Appearance = System.Windows.Forms.Appearance.Button;
            this.dgvViewList.AutoSize = true;
            this.dgvViewList.Location = new System.Drawing.Point(168, 491);
            this.dgvViewList.Name = "dgvViewList";
            this.dgvViewList.Size = new System.Drawing.Size(83, 23);
            this.dgvViewList.TabIndex = 14;
            this.dgvViewList.Text = "View Records";
            this.dgvViewList.UseVisualStyleBackColor = true;
            this.dgvViewList.CheckedChanged += new System.EventHandler(this.dgvViewList_CheckedChanged);
            // 
            // dgvViewRecords
            // 
            this.dgvViewRecords.AllowUserToAddRows = false;
            this.dgvViewRecords.AllowUserToDeleteRows = false;
            this.dgvViewRecords.AllowUserToResizeColumns = false;
            this.dgvViewRecords.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvViewRecords.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvViewRecords.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvViewRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvViewRecords.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvViewRecords.EnableHeadersVisualStyles = false;
            this.dgvViewRecords.Location = new System.Drawing.Point(272, 12);
            this.dgvViewRecords.Name = "dgvViewRecords";
            this.dgvViewRecords.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightGreen;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            this.dgvViewRecords.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvViewRecords.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvViewRecords.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvViewRecords.Size = new System.Drawing.Size(709, 502);
            this.dgvViewRecords.StandardTab = true;
            this.dgvViewRecords.TabIndex = 21;
            // 
            // BG_Driller
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 419);
            this.Controls.Add(this.dgvViewRecords);
            this.Controls.Add(this.dgvViewList);
            this.Controls.Add(this.rtbNote);
            this.Controls.Add(this.tbValue);
            this.Controls.Add(this.cbMeter);
            this.Controls.Add(this.cbFeet);
            this.Controls.Add(this.cbExtendView);
            this.Controls.Add(this.btnBGFinish);
            this.Controls.Add(this.btnBGDown);
            this.Controls.Add(this.btnBGUp);
            this.Controls.Add(this.btnBGLeft);
            this.Controls.Add(this.btnBGRight);
            this.Controls.Add(this.btnBGSurveyTimeStamp);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000, 558);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(281, 458);
            this.Name = "BG_Driller";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "BG Driller";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BG_Driller_FormClosing);
            this.Load += new System.EventHandler(this.BG_Driller_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvViewRecords)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button btnBGSurveyTimeStamp;
        private System.Windows.Forms.Button btnBGRight;
        private System.Windows.Forms.Button btnBGLeft;
        private System.Windows.Forms.Button btnBGUp;
        private System.Windows.Forms.Button btnBGDown;
        private System.Windows.Forms.Button btnBGFinish;
        private System.Windows.Forms.CheckBox cbExtendView;
        private System.Windows.Forms.CheckBox cbFeet;
        private System.Windows.Forms.CheckBox cbMeter;
        private System.Windows.Forms.RichTextBox tbValue;
        private System.Windows.Forms.RichTextBox rtbNote;
        private System.Windows.Forms.CheckBox dgvViewList;
        private System.Windows.Forms.DataGridView dgvViewRecords;
    }
}