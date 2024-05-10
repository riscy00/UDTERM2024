namespace UDT_Term_FFT
{
    partial class Serial_Monitor
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
            System.Windows.Forms.SaveFileDialog sfd_SaveLinData;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OutputViewTop = new System.Windows.Forms.ListView();
            this.OutputView = new System.Windows.Forms.DataGridView();
            sfd_SaveLinData = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OutputView)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.utilsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(889, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.saveToolStripMenuItem.Text = "Export to CVS";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // utilsToolStripMenuItem
            // 
            this.utilsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearScreenToolStripMenuItem,
            this.hideViewToolStripMenuItem});
            this.utilsToolStripMenuItem.Name = "utilsToolStripMenuItem";
            this.utilsToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.utilsToolStripMenuItem.Text = "Utils";
            // 
            // clearScreenToolStripMenuItem
            // 
            this.clearScreenToolStripMenuItem.Name = "clearScreenToolStripMenuItem";
            this.clearScreenToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.clearScreenToolStripMenuItem.Text = "Reset Screen";
            this.clearScreenToolStripMenuItem.Click += new System.EventHandler(this.clearScreenToolStripMenuItem_Click);
            // 
            // hideViewToolStripMenuItem
            // 
            this.hideViewToolStripMenuItem.Name = "hideViewToolStripMenuItem";
            this.hideViewToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.hideViewToolStripMenuItem.Text = "Hide View";
            this.hideViewToolStripMenuItem.Click += new System.EventHandler(this.hideViewToolStripMenuItem_Click);
            // 
            // OutputViewTop
            // 
            this.OutputViewTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.OutputViewTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.OutputViewTop.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputViewTop.ForeColor = System.Drawing.Color.Black;
            this.OutputViewTop.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.OutputViewTop.Location = new System.Drawing.Point(0, 24);
            this.OutputViewTop.Name = "OutputViewTop";
            this.OutputViewTop.Size = new System.Drawing.Size(889, 22);
            this.OutputViewTop.TabIndex = 9;
            this.OutputViewTop.UseCompatibleStateImageBehavior = false;
            this.OutputViewTop.View = System.Windows.Forms.View.Details;
            // 
            // OutputView
            // 
            this.OutputView.AllowUserToAddRows = false;
            this.OutputView.AllowUserToDeleteRows = false;
            this.OutputView.AllowUserToResizeColumns = false;
            this.OutputView.AllowUserToResizeRows = false;
            this.OutputView.BackgroundColor = System.Drawing.Color.DarkSlateGray;
            this.OutputView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Teal;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Yellow;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Teal;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.OutputView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.OutputView.ColumnHeadersHeight = 25;
            this.OutputView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkSlateGray;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Yellow;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.OutputView.DefaultCellStyle = dataGridViewCellStyle2;
            this.OutputView.GridColor = System.Drawing.Color.DarkCyan;
            this.OutputView.Location = new System.Drawing.Point(0, 45);
            this.OutputView.MultiSelect = false;
            this.OutputView.Name = "OutputView";
            this.OutputView.ReadOnly = true;
            this.OutputView.RowHeadersVisible = false;
            this.OutputView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.OutputView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.OutputView.ShowCellErrors = false;
            this.OutputView.ShowCellToolTips = false;
            this.OutputView.ShowEditingIcon = false;
            this.OutputView.ShowRowErrors = false;
            this.OutputView.Size = new System.Drawing.Size(889, 313);
            this.OutputView.TabIndex = 10;
            // 
            // Serial_Monitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 358);
            this.Controls.Add(this.OutputView);
            this.Controls.Add(this.OutputViewTop);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Serial_Monitor";
            this.Text = "CAN Bus Monitor";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Serial_Monitor_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OutputView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem utilsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearScreenToolStripMenuItem;
        private System.Windows.Forms.ListView OutputViewTop;
        private System.Windows.Forms.ToolStripMenuItem hideViewToolStripMenuItem;
        private System.Windows.Forms.DataGridView OutputView;
    }
}