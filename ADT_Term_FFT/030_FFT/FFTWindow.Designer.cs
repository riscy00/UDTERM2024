namespace UDT_Term_FFT
{
    partial class FFTWindow
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.FFTGraph = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.rtbFFTConsole = new System.Windows.Forms.RichTextBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.tbxAxisXFreq = new System.Windows.Forms.TextBox();
            this.tbxAxisYAmp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stayTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yAxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmYScaleLinear = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmAutoScale = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmHoldAutoScale = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm2K = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm20K = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm70K = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm2M = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm10M = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm20M = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm2G5 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm5G = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmYLogSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmYLinearSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmYScaleUpStep = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmYScaleDownStep = new System.Windows.Forms.ToolStripMenuItem();
            this.chartsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yAsixInADCVoltToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vScaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vScaleToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.testDataAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testDataALoopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testDataBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.whenLogIsUsedThereIsABugInnetChartWhichCrashesNeedToCheckNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dsPIC3325V0To16384ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bgwTestDataA = new System.ComponentModel.BackgroundWorker();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxFFTAverager = new System.Windows.Forms.TextBox();
            this.cBxYZOOM = new System.Windows.Forms.CheckBox();
            this.cBox_SQRTY = new System.Windows.Forms.CheckBox();
            this.cBox_Marker = new System.Windows.Forms.CheckBox();
            this.btn_YScaleUp = new System.Windows.Forms.Button();
            this.btn_YScaleDown = new System.Windows.Forms.Button();
            this.btn_YScaleAuto = new System.Windows.Forms.Button();
            this.btnLogLinYAxis = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.BtnExtendDisplay = new System.Windows.Forms.Button();
            this.btnSuspend = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbYScale_SelectRawData = new System.Windows.Forms.CheckBox();
            this.tbYScaleUser = new System.Windows.Forms.TextBox();
            this.tbYScaleMetaData = new System.Windows.Forms.TextBox();
            this.cbYScaleSelectUser = new System.Windows.Forms.CheckBox();
            this.cbYScaleSelectMetaData = new System.Windows.Forms.CheckBox();
            this.btnEditData = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.FFTGraph)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // FFTGraph
            // 
            this.FFTGraph.BackColor = System.Drawing.Color.DarkSlateGray;
            this.FFTGraph.BackSecondaryColor = System.Drawing.Color.White;
            this.FFTGraph.BorderlineWidth = 3;
            this.FFTGraph.BorderSkin.BorderColor = System.Drawing.Color.GreenYellow;
            this.FFTGraph.BorderSkin.BorderWidth = 3;
            chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.Yellow;
            chartArea1.AxisX.LineColor = System.Drawing.Color.Green;
            chartArea1.AxisX.MajorGrid.Interval = 0D;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Green;
            chartArea1.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisX.MinorTickMark.Enabled = true;
            chartArea1.AxisX.MinorTickMark.LineColor = System.Drawing.Color.YellowGreen;
            chartArea1.AxisX.ScrollBar.Size = 10D;
            chartArea1.AxisX.TitleForeColor = System.Drawing.Color.Green;
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.Yellow;
            chartArea1.AxisY.LineColor = System.Drawing.Color.Green;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.Green;
            chartArea1.AxisY.MinorTickMark.LineColor = System.Drawing.Color.YellowGreen;
            chartArea1.AxisY.ScrollBar.Size = 10D;
            chartArea1.AxisY.TitleForeColor = System.Drawing.Color.Yellow;
            chartArea1.BackColor = System.Drawing.Color.DarkSlateGray;
            chartArea1.BorderColor = System.Drawing.Color.Beige;
            chartArea1.BorderWidth = 2;
            chartArea1.CursorX.IsUserEnabled = true;
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.Name = "FFTArea";
            this.FFTGraph.ChartAreas.Add(chartArea1);
            this.FFTGraph.Location = new System.Drawing.Point(12, 118);
            this.FFTGraph.MaximumSize = new System.Drawing.Size(660, 434);
            this.FFTGraph.MinimumSize = new System.Drawing.Size(660, 434);
            this.FFTGraph.Name = "FFTGraph";
            this.FFTGraph.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.FFTGraph.Size = new System.Drawing.Size(660, 434);
            this.FFTGraph.TabIndex = 0;
            this.FFTGraph.Text = "FFTWindow";
            this.FFTGraph.GetToolTipText += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs>(this.FFTGraph_GetToolTipText);
            this.FFTGraph.FormatNumber += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.FormatNumberEventArgs>(this.FFTGraph_FormatNumber);
            this.FFTGraph.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FFTGraph_MouseMove);
            // 
            // rtbFFTConsole
            // 
            this.rtbFFTConsole.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.rtbFFTConsole.BackColor = System.Drawing.Color.DarkSlateGray;
            this.rtbFFTConsole.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbFFTConsole.ForeColor = System.Drawing.Color.Yellow;
            this.rtbFFTConsole.Location = new System.Drawing.Point(13, 556);
            this.rtbFFTConsole.MaximumSize = new System.Drawing.Size(659, 143);
            this.rtbFFTConsole.MaxLength = 10000;
            this.rtbFFTConsole.MinimumSize = new System.Drawing.Size(659, 143);
            this.rtbFFTConsole.Name = "rtbFFTConsole";
            this.rtbFFTConsole.ReadOnly = true;
            this.rtbFFTConsole.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rtbFFTConsole.Size = new System.Drawing.Size(659, 143);
            this.rtbFFTConsole.TabIndex = 1;
            this.rtbFFTConsole.Text = "";
            this.rtbFFTConsole.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbFFTConsole_KeyDown);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(173, 55);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(59, 23);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // tbxAxisXFreq
            // 
            this.tbxAxisXFreq.BackColor = System.Drawing.SystemColors.Control;
            this.tbxAxisXFreq.Location = new System.Drawing.Point(46, 12);
            this.tbxAxisXFreq.MaxLength = 16;
            this.tbxAxisXFreq.Name = "tbxAxisXFreq";
            this.tbxAxisXFreq.ReadOnly = true;
            this.tbxAxisXFreq.Size = new System.Drawing.Size(125, 20);
            this.tbxAxisXFreq.TabIndex = 5;
            this.tbxAxisXFreq.Text = "X";
            this.tbxAxisXFreq.WordWrap = false;
            this.tbxAxisXFreq.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbxAxisXFreq_MouseClick);
            // 
            // tbxAxisYAmp
            // 
            this.tbxAxisYAmp.Location = new System.Drawing.Point(46, 35);
            this.tbxAxisYAmp.MaxLength = 16;
            this.tbxAxisYAmp.Name = "tbxAxisYAmp";
            this.tbxAxisYAmp.ReadOnly = true;
            this.tbxAxisYAmp.Size = new System.Drawing.Size(125, 20);
            this.tbxAxisYAmp.TabIndex = 6;
            this.tbxAxisYAmp.Text = "Y";
            this.tbxAxisYAmp.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Freq=";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Amp=";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.tbxAxisYAmp);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbxAxisXFreq);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(494, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(179, 58);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cursor";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem,
            this.yAxisToolStripMenuItem,
            this.chartsToolStripMenuItem,
            this.dataToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(240, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stayTopToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.toolsToolStripMenuItem.Text = "Utils";
            // 
            // stayTopToolStripMenuItem
            // 
            this.stayTopToolStripMenuItem.Name = "stayTopToolStripMenuItem";
            this.stayTopToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.stayTopToolStripMenuItem.Text = "Stay Top";
            this.stayTopToolStripMenuItem.Click += new System.EventHandler(this.stayTopToolStripMenuItem_Click);
            // 
            // yAxisToolStripMenuItem
            // 
            this.yAxisToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmYScaleLinear,
            this.tsmYLogSelect,
            this.tsmYLinearSelect,
            this.tsmYScaleUpStep,
            this.tsmYScaleDownStep});
            this.yAxisToolStripMenuItem.Name = "yAxisToolStripMenuItem";
            this.yAxisToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.yAxisToolStripMenuItem.Text = "Y-Axis";
            // 
            // tsmYScaleLinear
            // 
            this.tsmYScaleLinear.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmAutoScale,
            this.tsmHoldAutoScale,
            this.tsm2K,
            this.tsm20K,
            this.tsm70K,
            this.tsm2M,
            this.tsm10M,
            this.tsm20M,
            this.tsm2G5,
            this.tsm5G});
            this.tsmYScaleLinear.Name = "tsmYScaleLinear";
            this.tsmYScaleLinear.Size = new System.Drawing.Size(173, 22);
            this.tsmYScaleLinear.Text = "Y-Scale";
            // 
            // tsmAutoScale
            // 
            this.tsmAutoScale.Checked = true;
            this.tsmAutoScale.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmAutoScale.Name = "tsmAutoScale";
            this.tsmAutoScale.Size = new System.Drawing.Size(162, 22);
            this.tsmAutoScale.Text = "Auto (Self-Scale)";
            this.tsmAutoScale.Click += new System.EventHandler(this.tsmAutoScale_Click);
            // 
            // tsmHoldAutoScale
            // 
            this.tsmHoldAutoScale.Name = "tsmHoldAutoScale";
            this.tsmHoldAutoScale.Size = new System.Drawing.Size(162, 22);
            this.tsmHoldAutoScale.Text = "Hold Auto Scale";
            this.tsmHoldAutoScale.Click += new System.EventHandler(this.tsmHoldAutoScale_Click);
            // 
            // tsm2K
            // 
            this.tsm2K.Name = "tsm2K";
            this.tsm2K.Size = new System.Drawing.Size(162, 22);
            this.tsm2K.Text = "2K (~10 Bit)";
            this.tsm2K.Click += new System.EventHandler(this.tsm2K_Click);
            // 
            // tsm20K
            // 
            this.tsm20K.Name = "tsm20K";
            this.tsm20K.Size = new System.Drawing.Size(162, 22);
            this.tsm20K.Text = "20 K (~14 Bits)";
            this.tsm20K.Click += new System.EventHandler(this.tsm20K_Click);
            // 
            // tsm70K
            // 
            this.tsm70K.Name = "tsm70K";
            this.tsm70K.Size = new System.Drawing.Size(162, 22);
            this.tsm70K.Text = "70 K (~16 Bit)";
            this.tsm70K.Click += new System.EventHandler(this.tsm70K_Click);
            // 
            // tsm2M
            // 
            this.tsm2M.Name = "tsm2M";
            this.tsm2M.Size = new System.Drawing.Size(162, 22);
            this.tsm2M.Text = "2 M ( ~20 Bit)";
            this.tsm2M.Click += new System.EventHandler(this.tsm2M_Click);
            // 
            // tsm10M
            // 
            this.tsm10M.Name = "tsm10M";
            this.tsm10M.Size = new System.Drawing.Size(162, 22);
            this.tsm10M.Text = "10 M (~23 Bit)";
            this.tsm10M.Click += new System.EventHandler(this.tsm10M_Click);
            // 
            // tsm20M
            // 
            this.tsm20M.Name = "tsm20M";
            this.tsm20M.Size = new System.Drawing.Size(162, 22);
            this.tsm20M.Text = "20 M (~24 Bit)";
            this.tsm20M.Click += new System.EventHandler(this.tsm20M_Click);
            // 
            // tsm2G5
            // 
            this.tsm2G5.Name = "tsm2G5";
            this.tsm2G5.Size = new System.Drawing.Size(162, 22);
            this.tsm2G5.Text = "2.5 G (~ 31 Bit)";
            this.tsm2G5.Click += new System.EventHandler(this.tsm2G5_Click);
            // 
            // tsm5G
            // 
            this.tsm5G.Name = "tsm5G";
            this.tsm5G.Size = new System.Drawing.Size(162, 22);
            this.tsm5G.Text = "5.0 G (~32 Bit)";
            this.tsm5G.Click += new System.EventHandler(this.tsm5G_Click);
            // 
            // tsmYLogSelect
            // 
            this.tsmYLogSelect.Name = "tsmYLogSelect";
            this.tsmYLogSelect.Size = new System.Drawing.Size(173, 22);
            this.tsmYLogSelect.Text = "Y-Log";
            this.tsmYLogSelect.Click += new System.EventHandler(this.tsmYLogSelect_Click);
            // 
            // tsmYLinearSelect
            // 
            this.tsmYLinearSelect.Name = "tsmYLinearSelect";
            this.tsmYLinearSelect.Size = new System.Drawing.Size(173, 22);
            this.tsmYLinearSelect.Text = "Y-Linear";
            this.tsmYLinearSelect.Click += new System.EventHandler(this.tsmYLinearSelect_Click);
            // 
            // tsmYScaleUpStep
            // 
            this.tsmYScaleUpStep.Name = "tsmYScaleUpStep";
            this.tsmYScaleUpStep.Size = new System.Drawing.Size(173, 22);
            this.tsmYScaleUpStep.Text = "Y-Scale Up Step";
            this.tsmYScaleUpStep.Click += new System.EventHandler(this.tsmYScaleUpStep_Click);
            // 
            // tsmYScaleDownStep
            // 
            this.tsmYScaleDownStep.Name = "tsmYScaleDownStep";
            this.tsmYScaleDownStep.Size = new System.Drawing.Size(173, 22);
            this.tsmYScaleDownStep.Text = "Y-Scale Down Step";
            this.tsmYScaleDownStep.Click += new System.EventHandler(this.tsmYScaleDownStep_Click);
            // 
            // chartsToolStripMenuItem
            // 
            this.chartsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.yAsixInADCVoltToolStripMenuItem});
            this.chartsToolStripMenuItem.Name = "chartsToolStripMenuItem";
            this.chartsToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.chartsToolStripMenuItem.Text = "Charts";
            // 
            // yAsixInADCVoltToolStripMenuItem
            // 
            this.yAsixInADCVoltToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vScaleToolStripMenuItem,
            this.vScaleToolStripMenuItem1});
            this.yAsixInADCVoltToolStripMenuItem.Name = "yAsixInADCVoltToolStripMenuItem";
            this.yAsixInADCVoltToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.yAsixInADCVoltToolStripMenuItem.Text = "Y-Asix in ADC Volt";
            this.yAsixInADCVoltToolStripMenuItem.Click += new System.EventHandler(this.yAsixInADCVoltToolStripMenuItem_Click);
            // 
            // vScaleToolStripMenuItem
            // 
            this.vScaleToolStripMenuItem.Checked = true;
            this.vScaleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.vScaleToolStripMenuItem.Name = "vScaleToolStripMenuItem";
            this.vScaleToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.vScaleToolStripMenuItem.Text = "2.5V Scale";
            // 
            // vScaleToolStripMenuItem1
            // 
            this.vScaleToolStripMenuItem1.Enabled = false;
            this.vScaleToolStripMenuItem1.Name = "vScaleToolStripMenuItem1";
            this.vScaleToolStripMenuItem1.Size = new System.Drawing.Size(126, 22);
            this.vScaleToolStripMenuItem1.Text = "0.5V Scale";
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.testDataAToolStripMenuItem,
            this.testDataALoopToolStripMenuItem,
            this.testDataBToolStripMenuItem,
            this.toolStripSeparator2});
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.dataToolStripMenuItem.Text = "Test";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(167, 6);
            // 
            // testDataAToolStripMenuItem
            // 
            this.testDataAToolStripMenuItem.Name = "testDataAToolStripMenuItem";
            this.testDataAToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.testDataAToolStripMenuItem.Text = "Test-Data-A (128)";
            this.testDataAToolStripMenuItem.Click += new System.EventHandler(this.testDataAToolStripMenuItem_Click);
            // 
            // testDataALoopToolStripMenuItem
            // 
            this.testDataALoopToolStripMenuItem.Name = "testDataALoopToolStripMenuItem";
            this.testDataALoopToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.testDataALoopToolStripMenuItem.Text = "Test-Data-A-Loop";
            this.testDataALoopToolStripMenuItem.Click += new System.EventHandler(this.testDataALoopToolStripMenuItem_Click);
            // 
            // testDataBToolStripMenuItem
            // 
            this.testDataBToolStripMenuItem.Name = "testDataBToolStripMenuItem";
            this.testDataBToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.testDataBToolStripMenuItem.Text = "Test-Data-B (256)";
            this.testDataBToolStripMenuItem.Click += new System.EventHandler(this.testDataBToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(167, 6);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.whenLogIsUsedThereIsABugInnetChartWhichCrashesNeedToCheckNumberToolStripMenuItem,
            this.dsPIC3325V0To16384ToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(572, 22);
            this.aboutToolStripMenuItem.Text = "About/Help";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // whenLogIsUsedThereIsABugInnetChartWhichCrashesNeedToCheckNumberToolStripMenuItem
            // 
            this.whenLogIsUsedThereIsABugInnetChartWhichCrashesNeedToCheckNumberToolStripMenuItem.Name = "whenLogIsUsedThereIsABugInnetChartWhichCrashesNeedToCheckNumberToolStripMenuItem";
            this.whenLogIsUsedThereIsABugInnetChartWhichCrashesNeedToCheckNumberToolStripMenuItem.Size = new System.Drawing.Size(572, 22);
            this.whenLogIsUsedThereIsABugInnetChartWhichCrashesNeedToCheckNumberToolStripMenuItem.Text = "BUG Issue: When Log is used, there is a bug in .net chart which crashes. Need to " +
    "check number";
            // 
            // dsPIC3325V0To16384ToolStripMenuItem
            // 
            this.dsPIC3325V0To16384ToolStripMenuItem.Name = "dsPIC3325V0To16384ToolStripMenuItem";
            this.dsPIC3325V0To16384ToolStripMenuItem.Size = new System.Drawing.Size(572, 22);
            this.dsPIC3325V0To16384ToolStripMenuItem.Text = "dsPIC33, 2.5V => 0 to 16384";
            // 
            // bgwTestDataA
            // 
            this.bgwTestDataA.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwTestDataA_DoWork);
            this.bgwTestDataA.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwTestDataA_RunWorkerCompleted);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tbxFFTAverager);
            this.groupBox2.Location = new System.Drawing.Point(494, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(121, 48);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "FFT Averager";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(64, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "x32= Max";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(78, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "0 = Off";
            // 
            // tbxFFTAverager
            // 
            this.tbxFFTAverager.Location = new System.Drawing.Point(9, 21);
            this.tbxFFTAverager.Name = "tbxFFTAverager";
            this.tbxFFTAverager.Size = new System.Drawing.Size(48, 20);
            this.tbxFFTAverager.TabIndex = 0;
            this.tbxFFTAverager.Text = "0";
            this.tbxFFTAverager.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbxFFTAverager.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbxFFTAverager_KeyDown);
            // 
            // cBxYZOOM
            // 
            this.cBxYZOOM.Appearance = System.Windows.Forms.Appearance.Button;
            this.cBxYZOOM.Location = new System.Drawing.Point(173, 84);
            this.cBxYZOOM.Name = "cBxYZOOM";
            this.cBxYZOOM.Size = new System.Drawing.Size(59, 23);
            this.cBxYZOOM.TabIndex = 20;
            this.cBxYZOOM.Text = "Y-Zoom";
            this.cBxYZOOM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cBxYZOOM.UseVisualStyleBackColor = true;
            this.cBxYZOOM.CheckedChanged += new System.EventHandler(this.cBxYZOOM_CheckedChanged);
            // 
            // cBox_SQRTY
            // 
            this.cBox_SQRTY.AutoSize = true;
            this.cBox_SQRTY.Location = new System.Drawing.Point(6, 39);
            this.cBox_SQRTY.Name = "cBox_SQRTY";
            this.cBox_SQRTY.Size = new System.Drawing.Size(69, 17);
            this.cBox_SQRTY.TabIndex = 26;
            this.cBox_SQRTY.Text = "SQRT(Y)";
            this.cBox_SQRTY.UseVisualStyleBackColor = true;
            this.cBox_SQRTY.CheckedChanged += new System.EventHandler(this.cBox_SQRTY_CheckedChanged);
            // 
            // cBox_Marker
            // 
            this.cBox_Marker.AutoSize = true;
            this.cBox_Marker.Location = new System.Drawing.Point(6, 20);
            this.cBox_Marker.Name = "cBox_Marker";
            this.cBox_Marker.Size = new System.Drawing.Size(59, 17);
            this.cBox_Marker.TabIndex = 25;
            this.cBox_Marker.Text = "Marker";
            this.cBox_Marker.UseVisualStyleBackColor = true;
            this.cBox_Marker.CheckedChanged += new System.EventHandler(this.cBox_Marker_CheckedChanged);
            // 
            // btn_YScaleUp
            // 
            this.btn_YScaleUp.Location = new System.Drawing.Point(13, 55);
            this.btn_YScaleUp.Name = "btn_YScaleUp";
            this.btn_YScaleUp.Size = new System.Drawing.Size(62, 23);
            this.btn_YScaleUp.TabIndex = 0;
            this.btn_YScaleUp.Text = "Y-Up";
            this.btn_YScaleUp.UseVisualStyleBackColor = true;
            this.btn_YScaleUp.Click += new System.EventHandler(this.btn_YScaleUp_Click);
            // 
            // btn_YScaleDown
            // 
            this.btn_YScaleDown.Location = new System.Drawing.Point(12, 84);
            this.btn_YScaleDown.Name = "btn_YScaleDown";
            this.btn_YScaleDown.Size = new System.Drawing.Size(62, 23);
            this.btn_YScaleDown.TabIndex = 1;
            this.btn_YScaleDown.Text = "Y-Down";
            this.btn_YScaleDown.UseVisualStyleBackColor = true;
            this.btn_YScaleDown.Click += new System.EventHandler(this.btn_YScaleDown_Click);
            // 
            // btn_YScaleAuto
            // 
            this.btn_YScaleAuto.Location = new System.Drawing.Point(13, 26);
            this.btn_YScaleAuto.Name = "btn_YScaleAuto";
            this.btn_YScaleAuto.Size = new System.Drawing.Size(62, 23);
            this.btn_YScaleAuto.TabIndex = 2;
            this.btn_YScaleAuto.Text = "Y-Auto";
            this.btn_YScaleAuto.UseVisualStyleBackColor = true;
            this.btn_YScaleAuto.Click += new System.EventHandler(this.btn_YScaleAuto_Click);
            // 
            // btnLogLinYAxis
            // 
            this.btnLogLinYAxis.Location = new System.Drawing.Point(92, 84);
            this.btnLogLinYAxis.Name = "btnLogLinYAxis";
            this.btnLogLinYAxis.Size = new System.Drawing.Size(62, 23);
            this.btnLogLinYAxis.TabIndex = 21;
            this.btnLogLinYAxis.Text = "Y-Log";
            this.btnLogLinYAxis.UseVisualStyleBackColor = true;
            this.btnLogLinYAxis.Click += new System.EventHandler(this.btnLogLinYAxis_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.Transparent;
            this.groupBox4.Controls.Add(this.cBox_SQRTY);
            this.groupBox4.Controls.Add(this.cBox_Marker);
            this.groupBox4.Location = new System.Drawing.Point(251, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(84, 74);
            this.groupBox4.TabIndex = 27;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Option";
            // 
            // BtnExtendDisplay
            // 
            this.BtnExtendDisplay.Location = new System.Drawing.Point(173, 26);
            this.BtnExtendDisplay.Name = "BtnExtendDisplay";
            this.BtnExtendDisplay.Size = new System.Drawing.Size(59, 23);
            this.BtnExtendDisplay.TabIndex = 28;
            this.BtnExtendDisplay.Text = "Extend";
            this.BtnExtendDisplay.UseVisualStyleBackColor = true;
            this.BtnExtendDisplay.Click += new System.EventHandler(this.BtnExtendDisplay_Click);
            // 
            // btnSuspend
            // 
            this.btnSuspend.Location = new System.Drawing.Point(95, 26);
            this.btnSuspend.Name = "btnSuspend";
            this.btnSuspend.Size = new System.Drawing.Size(59, 23);
            this.btnSuspend.TabIndex = 29;
            this.btnSuspend.Text = "Suspend";
            this.btnSuspend.UseVisualStyleBackColor = true;
            this.btnSuspend.Click += new System.EventHandler(this.btnSuspend_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.Transparent;
            this.groupBox5.Controls.Add(this.cbYScale_SelectRawData);
            this.groupBox5.Controls.Add(this.tbYScaleUser);
            this.groupBox5.Controls.Add(this.tbYScaleMetaData);
            this.groupBox5.Controls.Add(this.cbYScaleSelectUser);
            this.groupBox5.Controls.Add(this.cbYScaleSelectMetaData);
            this.groupBox5.Location = new System.Drawing.Point(341, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(147, 106);
            this.groupBox5.TabIndex = 30;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Y-Scale Reading";
            // 
            // cbYScale_SelectRawData
            // 
            this.cbYScale_SelectRawData.AutoSize = true;
            this.cbYScale_SelectRawData.Location = new System.Drawing.Point(6, 69);
            this.cbYScale_SelectRawData.Name = "cbYScale_SelectRawData";
            this.cbYScale_SelectRawData.Size = new System.Drawing.Size(74, 17);
            this.cbYScale_SelectRawData.TabIndex = 28;
            this.cbYScale_SelectRawData.Text = "Raw Data";
            this.cbYScale_SelectRawData.UseVisualStyleBackColor = true;
            this.cbYScale_SelectRawData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cbYScale_SelectRawData_MouseClick);
            // 
            // tbYScaleUser
            // 
            this.tbYScaleUser.Location = new System.Drawing.Point(82, 46);
            this.tbYScaleUser.Name = "tbYScaleUser";
            this.tbYScaleUser.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbYScaleUser.Size = new System.Drawing.Size(59, 20);
            this.tbYScaleUser.TabIndex = 27;
            this.tbYScaleUser.Text = "0";
            this.tbYScaleUser.WordWrap = false;
            // 
            // tbYScaleMetaData
            // 
            this.tbYScaleMetaData.Location = new System.Drawing.Point(82, 20);
            this.tbYScaleMetaData.Name = "tbYScaleMetaData";
            this.tbYScaleMetaData.ReadOnly = true;
            this.tbYScaleMetaData.Size = new System.Drawing.Size(59, 20);
            this.tbYScaleMetaData.TabIndex = 3;
            this.tbYScaleMetaData.Text = "0";
            this.tbYScaleMetaData.WordWrap = false;
            // 
            // cbYScaleSelectUser
            // 
            this.cbYScaleSelectUser.AutoSize = true;
            this.cbYScaleSelectUser.Location = new System.Drawing.Point(6, 46);
            this.cbYScaleSelectUser.Name = "cbYScaleSelectUser";
            this.cbYScaleSelectUser.Size = new System.Drawing.Size(66, 17);
            this.cbYScaleSelectUser.TabIndex = 26;
            this.cbYScaleSelectUser.Text = "Override";
            this.cbYScaleSelectUser.UseVisualStyleBackColor = true;
            this.cbYScaleSelectUser.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cbYScaleSelectUser_MouseClick);
            // 
            // cbYScaleSelectMetaData
            // 
            this.cbYScaleSelectMetaData.AutoSize = true;
            this.cbYScaleSelectMetaData.Location = new System.Drawing.Point(6, 23);
            this.cbYScaleSelectMetaData.Name = "cbYScaleSelectMetaData";
            this.cbYScaleSelectMetaData.Size = new System.Drawing.Size(76, 17);
            this.cbYScaleSelectMetaData.TabIndex = 25;
            this.cbYScaleSelectMetaData.Text = "Meta Data";
            this.cbYScaleSelectMetaData.UseVisualStyleBackColor = true;
            this.cbYScaleSelectMetaData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cbYScaleSelectMetaData_MouseClick);
            // 
            // btnEditData
            // 
            this.btnEditData.Location = new System.Drawing.Point(251, 84);
            this.btnEditData.Name = "btnEditData";
            this.btnEditData.Size = new System.Drawing.Size(84, 23);
            this.btnEditData.TabIndex = 31;
            this.btnEditData.Text = "Edit Data";
            this.btnEditData.UseVisualStyleBackColor = true;
            this.btnEditData.Click += new System.EventHandler(this.btnEditData_Click);
            // 
            // FFTWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(684, 711);
            this.Controls.Add(this.btnEditData);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnSuspend);
            this.Controls.Add(this.BtnExtendDisplay);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.cBxYZOOM);
            this.Controls.Add(this.btnLogLinYAxis);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btn_YScaleUp);
            this.Controls.Add(this.btn_YScaleDown);
            this.Controls.Add(this.btn_YScaleAuto);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rtbFFTConsole);
            this.Controls.Add(this.FFTGraph);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximumSize = new System.Drawing.Size(700, 750);
            this.MinimumSize = new System.Drawing.Size(700, 750);
            this.Name = "FFTWindow";
            this.Text = "FFTWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FFTWindow_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.FFTGraph)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.DataVisualization.Charting.Chart FFTGraph;
        private System.Windows.Forms.DataVisualization.Charting.Series seriesCh1;
        private System.Windows.Forms.RichTextBox rtbFFTConsole;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TextBox tbxAxisXFreq;
        private System.Windows.Forms.TextBox tbxAxisYAmp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chartsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testDataAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testDataBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yAsixInADCVoltToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vScaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vScaleToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem stayTopToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bgwTestDataA;
        private System.Windows.Forms.ToolStripMenuItem testDataALoopToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxFFTAverager;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.CheckBox cBxYZOOM;
        private System.Windows.Forms.ToolStripMenuItem whenLogIsUsedThereIsABugInnetChartWhichCrashesNeedToCheckNumberToolStripMenuItem;
        private System.Windows.Forms.CheckBox cBox_SQRTY;
        private System.Windows.Forms.CheckBox cBox_Marker;
        private System.Windows.Forms.ToolStripMenuItem yAxisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmYScaleLinear;
        private System.Windows.Forms.ToolStripMenuItem tsmAutoScale;
        private System.Windows.Forms.ToolStripMenuItem tsmHoldAutoScale;
        private System.Windows.Forms.ToolStripMenuItem tsm2K;
        private System.Windows.Forms.ToolStripMenuItem tsm20K;
        private System.Windows.Forms.ToolStripMenuItem tsm70K;
        private System.Windows.Forms.ToolStripMenuItem tsm2M;
        private System.Windows.Forms.ToolStripMenuItem tsm10M;
        private System.Windows.Forms.ToolStripMenuItem tsm20M;
        private System.Windows.Forms.ToolStripMenuItem tsm5G;
        private System.Windows.Forms.ToolStripMenuItem tsm2G5;
        private System.Windows.Forms.ToolStripMenuItem tsmYLogSelect;
        private System.Windows.Forms.ToolStripMenuItem tsmYScaleUpStep;
        private System.Windows.Forms.ToolStripMenuItem tsmYScaleDownStep;
        private System.Windows.Forms.Button btn_YScaleUp;
        private System.Windows.Forms.Button btn_YScaleDown;
        private System.Windows.Forms.Button btn_YScaleAuto;
        private System.Windows.Forms.Button btnLogLinYAxis;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ToolStripMenuItem tsmYLinearSelect;
        private System.Windows.Forms.Button BtnExtendDisplay;
        private System.Windows.Forms.ToolStripMenuItem dsPIC3325V0To16384ToolStripMenuItem;
        private System.Windows.Forms.Button btnSuspend;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox tbYScaleUser;
        private System.Windows.Forms.TextBox tbYScaleMetaData;
        private System.Windows.Forms.CheckBox cbYScaleSelectUser;
        private System.Windows.Forms.CheckBox cbYScaleSelectMetaData;
        private System.Windows.Forms.CheckBox cbYScale_SelectRawData;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button btnEditData;
    }
}