using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Diagnostics;

// Power Tool: insert QuickLaunch with "@tasks CollapseRegions" to close all region only. 
// 29-08-16: Added feature to Remove Zero to clean up chart (bad data event), it basically skip adding point to chart if data is zero. Default Enabled. Work for CH1,2,3 and 4. 
//

namespace UDT_Term_FFT
{
    public partial class RiscyScope : Form
    {
        public string sDefaultFolder { get; set; }
        public string sFilanameData { get; set; }
        public string sFilanameGainOffset { get; set; }
        public string sFilanameConfig { get; set; }

        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        TestWaveFormGen myTestWaveGen;
        int iCursorSelectChannel;
        BindingList<ScopeDataOp> blScopeDataOp;
        ScopeSetup cScopeSetup;
        private bool mDgvScopeDataOpSubscribed;     // Event subscribe or unsubscribe


        private DataSource DSCH0;
        private DataSource DSCH1;
        private DataSource DSCH2;
        private DataSource DSCH3;

        #region//================================================================Reference
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
            //--------------------------------------Themes
            switch (myGlobalBase.CompanyName)
            {
                case (0):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_Blank_1B;
                        break;
                    }
                case (20):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.BG_MainPage_Blank_1B;
                        break;
                    }
                case (30):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.BG_MainPage_Blank_1B;
                        break;
                    }
                case (50):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPageTVBBlank_1B;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            this.Refresh();
        }
        #endregion

        public RiscyScope()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            cScope.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            this.cScope.MouseWheel += new MouseEventHandler(this.cScope_MouseWheel);
            this.cScope.MouseEnter += new EventHandler(this.cScope_MouseEnter);
            this.cScope.MouseLeave += new EventHandler(this.cScope_MouseLeave);
            //----------------------------------------------------------------------------File name
            sDefaultFolder = "";
            sFilanameData = "ScopeData";
            sFilanameGainOffset = "ScopeGainOffset";
            sFilanameConfig = "ScopeConfig";

            ScopeDataOp_Init();     // Datagridview Box for Amp and Offset. 
            //----------------------------------------------------------------------------Axis Title
            cScope.ChartAreas["ScopeArea"].AxisX.TitleAlignment = StringAlignment.Near;
            cScope.ChartAreas["ScopeArea"].AxisX.TextOrientation = TextOrientation.Horizontal;
            cScope.ChartAreas["ScopeArea"].AxisX.TitleForeColor = Color.White;
            cScope.ChartAreas["ScopeArea"].CursorY.Interval = 0;                // Smoother Y axis movement for mouse cursor. 
            myTestWaveGen = new TestWaveFormGen();

            cScope.Series.Add("CH0w");
            cScope.Series["CH0w"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            cScope.Series["CH0w"].ChartArea = "ScopeArea";

            cScope.Series.Add("CH1w");
            cScope.Series["CH1w"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            cScope.Series["CH1w"].ChartArea = "ScopeArea";

            cScope.Series.Add("CH2w");
            cScope.Series["CH2w"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            cScope.Series["CH2w"].ChartArea = "ScopeArea";

            cScope.Series.Add("CH3w");
            cScope.Series["CH3w"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            cScope.Series["CH3w"].ChartArea = "ScopeArea";

            cScope.Series.Add("ROLL");      // Dummy channel to set fixed X axis. 
            cScope.Series["ROLL"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            cScope.Series["ROLL"].ChartArea = "ScopeArea";

            cScope.DoubleBufferedRiscyScope(true);
            iCursorSelectChannel = 0;
            cbCursorCH0.Checked = true;
            cbCursorCH1.Checked = false;
            cbCursorCH2.Checked = false;
            cbCursorCH3.Checked = false;
            cScope.ChartAreas[0].AxisX.IsMarginVisible = false;
            List<cPoint> CPointnull = null;
            DSCH0 = new DataSource(ref CPointnull, "CH0", "Null0", Color.Yellow);
            DSCH1 = new DataSource(ref CPointnull, "CH1", "Null1", Color.MediumSeaGreen);
            DSCH2 = new DataSource(ref CPointnull, "CH2", "Null2", Color.Cyan);
            DSCH3 = new DataSource(ref CPointnull, "CH3", "Null3", Color.LightPink);

            if (cScopeSetup == null)
                cScopeSetup = new ScopeSetup();
            //-------------------------------Initial Setting
            cScopeSetup.isSizeLarge = false;
            cScopeSetup.isRollingMode = true;       // Rolling mode, the xx must be point number (not mSec Tick). 
            cScopeSetup.isTimeBaseTick = false;
            cbRemoveZero.Checked = true;
            //-------------------------------Dual Y axis
            VertAxis_Init();
            VertAxisUpdate();
        }

        //#####################################################################################################
        //###################################################################################### Form Management
        //#####################################################################################################

        #region //================================================================RiscyScope_Load
        private void RiscyScope_Load(object sender, EventArgs e)
        {
            //Experiment();
            ((DataGridViewCheckBoxCell)dgvScopeDataOp.Rows[0].Cells[1]).Value = true;
            ((DataGridViewCheckBoxCell)dgvScopeDataOp.Rows[1].Cells[1]).Value = true;
            ((DataGridViewCheckBoxCell)dgvScopeDataOp.Rows[2].Cells[1]).Value = true;
            ((DataGridViewCheckBoxCell)dgvScopeDataOp.Rows[3].Cells[1]).Value = true;

            WindowSizeUpdate();
            TimeBaseUpdate(false);
            Event_dgvScopeDataOp_Subscribe(true);
            
        }
        #endregion

        #region //================================================================RiscyScope_FormClosing
        private void RiscyScope_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (myGlobalBase.EE_isSurveyCVSRawLogDataActive == false)       // Active process, cannot close. 
            {
                myGlobalBase.Scope_IsFormOpen = false;                 // Cease Survey CVS Terminal mode.
                this.Visible = false;
                this.Hide();
            }
        }
        #endregion

        #region //================================================================RiscyScope_Show
        public void RiscyScope_Show()
        {
            this.BringToFront();
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            myGlobalBase.Scope_IsFormOpen = true;
            this.Visible = true;
            this.Show();
            //---------------------------
            tbRollRange.Text = cScopeSetup.iMaxRollPointer.ToString();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Data Management
        //#####################################################################################################
        #region //================================================================cScopeUpdateAxisTitle
        private void cScopeUpdateAxisTitle()
        {
            int totalsample = 0;
            int displaysample = 0;
            if (DSCH0 != null)
            {
                totalsample = DSCH0.Length;
                displaysample = DSCH0.iRollPointer;
            }
            else if (DSCH1 != null)
            {
                totalsample = DSCH1.Length;
                displaysample = DSCH1.iRollPointer;
            }
            else if (DSCH2 != null)
            {
                totalsample = DSCH2.Length;
                displaysample = DSCH2.iRollPointer;
            }
            else if (DSCH3 != null)
            {
                totalsample = DSCH3.Length;
                displaysample = DSCH3.iRollPointer;
            }
            else  // No Data. 
            {
                cScope.ChartAreas["ScopeArea"].AxisX.Title = "No Available Data";
                return;
            }
            //---------------------------------------------
            if (cScopeSetup.isRollingMode == true)
            {
                cScope.ChartAreas["ScopeArea"].AxisX.Title = "Logger Mode: Total Sample:"
                    + totalsample.ToString()
                    + ". Displayed Range:"
                    + displaysample.ToString()
                    + ". (Sample Number)";
            }
            else
            {
                if (cScopeSetup.isTimeBaseTick == true)
                {
                    cScope.ChartAreas["ScopeArea"].AxisX.Title = "Full Chart Mode: Total Sample:"
                        + totalsample.ToString()
                        + ". (mSec)";
                }
                else
                {
                    cScope.ChartAreas["ScopeArea"].AxisX.Title = "Full Chart Mode: Total Sample :"
                        + totalsample.ToString()
                        + ". (Sample Number)";
                }
            }
        }
        #endregion

        #region //================================================================SetupRollingClass
        private void SetupRollingClass()
        {
            if (cScope.Series["ROLL"].Points.Count != cScopeSetup.iMaxRollPointer)        // In case of change in window size (compact = 800, extended = 1300)
            {
                cScope.Series["ROLL"].Points.Clear();
                cScope.Series["ROLL"].Color = Color.Transparent;
                cScope.Series["ROLL"].EmptyPointStyle.Color = Color.Transparent;
                //cScope.DataManipulator.InsertEmptyPoints(DSCH0.iMaxRollPointer,IntervalType.Days, cScope.Series["ROLL"]);
                for (int i = 0; i < cScopeSetup.iMaxRollPointer; i++)
                {
                    cScope.Series["ROLL"].Points.AddXY(i, 0);
                }
            }
        }
        #endregion

        #region //================================================================ReInitRollChart (allow switching between Chart and Rolling Mode)
        private void ReInitRollChart()
        {
            int length = 0;
            if (DSCH0 != null)
            {
                if (length < DSCH0.Length)
                    length = DSCH0.Length;
            }
            if (DSCH1 != null)
            {
                if (length < DSCH1.Length)
                    length = DSCH1.Length;
            }
            if (DSCH2 != null)
            {
                if (length < DSCH2.Length)
                    length = DSCH2.Length;
            }
            if (DSCH3 != null)
            {
                if (length < DSCH3.Length)
                    length = DSCH3.Length;
            }
            if (length < cScopeSetup.iMaxRollPointer)
            {
                SetupRollingClass();
            }
            //-------------------------CH0
            if (DSCH0 != null)
            {
                DSCH0.iRollPointer = 0;
                if (DSCH0.Length < cScopeSetup.iMaxRollPointer)
                {
                    DSCH0.iRoll_StartX = 0;
                    DSCH0.iRoll_EndX = DSCH0.Length;
                    DSCH0.iRollPointer = DSCH0.Length;
                }
                else
                {
                    DSCH0.iRollPointer = cScopeSetup.iMaxRollPointer;
                    DSCH0.iRoll_StartX = DSCH0.Length - cScopeSetup.iMaxRollPointer;
                    DSCH0.iRoll_EndX = DSCH0.Length;

                }
                RefreshRollingCh0Class();
            }
            //-------------------------CH1
            if (DSCH1 != null)
            {
                DSCH1.iRollPointer = 0;
                if (DSCH1.Length < cScopeSetup.iMaxRollPointer)
                {
                    DSCH1.iRoll_StartX = 0;
                    DSCH1.iRoll_EndX = DSCH1.Length;
                    DSCH1.iRollPointer = DSCH1.Length;
                }
                else
                {
                    DSCH1.iRollPointer = cScopeSetup.iMaxRollPointer;
                    DSCH1.iRoll_StartX = DSCH1.Length - cScopeSetup.iMaxRollPointer;
                    DSCH1.iRoll_EndX = DSCH1.Length;

                }
                RefreshRollingCh1Class();
            }
            //-------------------------CH2
            if (DSCH2 != null)
            {
                DSCH2.iRollPointer = 0;
                if (DSCH2.Length < cScopeSetup.iMaxRollPointer)
                {
                    DSCH2.iRoll_StartX = 0;
                    DSCH2.iRoll_EndX = DSCH2.Length;
                    DSCH2.iRollPointer = DSCH2.Length;
                }
                else
                {
                    DSCH2.iRollPointer = cScopeSetup.iMaxRollPointer;
                    DSCH2.iRoll_StartX = DSCH2.Length - cScopeSetup.iMaxRollPointer;
                    DSCH2.iRoll_EndX = DSCH2.Length;

                }
                RefreshRollingCh2Class();
            }
            //-------------------------CH3
            if (DSCH3 != null)
            {
                DSCH3.iRollPointer = 0;
                if (DSCH3.Length < cScopeSetup.iMaxRollPointer)
                {
                    DSCH3.iRoll_StartX = 0;
                    DSCH3.iRoll_EndX = DSCH3.Length;
                    DSCH3.iRollPointer = DSCH3.Length;
                }
                else
                {
                    DSCH3.iRollPointer = cScopeSetup.iMaxRollPointer;
                    DSCH3.iRoll_StartX = DSCH3.Length - cScopeSetup.iMaxRollPointer;
                    DSCH3.iRoll_EndX = DSCH3.Length;

                }
                RefreshRollingCh3Class();
            }

        }
        #endregion

        #region //================================================================ChangeRollingModetoChartMode
        private void ChangeRollingModetoChartMode()
        {
            cScope.Series["ROLL"].Points.Clear();
        }
        #endregion

        //==============================================================================================CH0
        #region //================================================================StartChartCh0Class (1st time start from here)
        public void StartChartCh0Class(int StartRow, int EndRow, string ColumnName, ref List<cPoint> fdata, Color linecolor)
        {
            DSCH0 = null;
            //----------------------------------------Rolling Mode (Initial Data View).
            if (cScopeSetup.isRollingMode == true)
            {
                List<cPoint> CPointnull = null;
                DSCH0 = new DataSource(ref CPointnull, "CH0", ColumnName, linecolor);
                DSCH0.StartRow = 0;
                DSCH0.EndRow = 0;
                SetupRollingClass();
                DSCH0.iRoll_StartX = 0;
                DSCH0.iRoll_EndX = 0;
                DSCH0.iRollPointer = 0;
                for (int i = StartRow; i < EndRow; i++)
                {
                    AppendCh0Class(fdata[i]);
                }
            }
            else  //------------------------------------Chart Mode
            {
                DSCH0 = new DataSource(ref fdata, "CH0", ColumnName, linecolor);
                DSCH0.StartRow = StartRow;
                DSCH0.EndRow = EndRow-1;
                RefreshChartCh0Class(true);
            }
            cScopeUpdateAxisTitle();
        }
        #endregion

        #region //================================================================RefreshChartCh0Class
        public void RefreshChartCh0Class(bool isMethod)     // True = Whole Screen, False = Append Data Only. 
        {
            if (DSCH0 == null)
                return;
            if ((cScopeSetup.isRollingMode == true) || (DSCH0 == null))
                return;
            if (cScope.Series["ROLL"].Points.Count != 0)        // Clear Rolling Series data, not needed in this mode. 
                ChangeRollingModetoChartMode();
            bool bb = Convert.ToBoolean(dgvScopeDataOp.Rows[0].Cells[1].Value);
            double yd;
            //----------------------------------Update scope display
            try
            {
                if (isMethod == true)  // Whole screens
                {
                    //----------------------------------Manage Endrow for selected and whole range to avoid exception occurance (minor bug fixed) 
                    int EndRow = DSCH0.EndRow;
                    if (DSCH0.EndRow == DSCH0.Length)
                        EndRow--;
                    cScope.Series["CH0w"].Points.Clear();       // Clear points
                    cScope.Series["CH0w"].Color = DSCH0.color;
                    for (int i = DSCH0.StartRow; i <= EndRow; i++)
                    {
                        yd = DSCH0.cSamples[i].y;
                        //----------------------------------------------Simple Gain/Offset calculation, if check activate this. 
                        if (bb == true)
                            yd = yd * blScopeDataOp[0].DataGain + blScopeDataOp[0].DataOffset;
                        if (cScopeSetup.isTimeBaseTick == true)          // Row Number or timebase(mSec)
                        {
                            if (!((cbRemoveZero.Checked==true) & (yd == 0.0)))
                                cScope.Series["CH0w"].Points.AddXY(DSCH0.cSamples[i].x, yd);
                        }
                        else
                        {
                            if (!((cbRemoveZero.Checked == true) & (yd == 0.0)))
                                cScope.Series["CH0w"].Points.AddXY(i, yd);
                        }
                    }
                }
                else  // Append Data Only
                {
                    yd = DSCH0.cSamples[DSCH0.Length - 1].y;
                    //----------------------------------------------Simple Gain/Offset calculation, if check activate this. 
                    if (bb == true)
                        yd = yd * blScopeDataOp[0].DataGain + blScopeDataOp[0].DataOffset;
                    if (cScopeSetup.isTimeBaseTick == true)          // Row Number or timebase(mSec)
                    {
                        if (!((cbRemoveZero.Checked == true) & (yd == 0.0)))
                            cScope.Series["CH0w"].Points.AddXY(DSCH0.cSamples[DSCH0.EndRow].x, yd);
                    }
                    else
                    {
                        if (!((cbRemoveZero.Checked == true) & (yd == 0.0)))
                            cScope.Series["CH0w"].Points.AddXY(DSCH0.EndRow, yd);
                    }
                }
            }
            catch { }
        }
        #endregion

        #region //================================================================RefreshRollingCh0Class
        private void RefreshRollingCh0Class()
        {
            if (DSCH0 == null)
                return;
            double yd;
            bool bb = Convert.ToBoolean(dgvScopeDataOp.Rows[0].Cells[1].Value);
            if ((DSCH0.iRollPointer == cScopeSetup.iMaxRollPointer) & (cScope.Series["ROLL"].Points.Count != 0))
            {
                cScope.Series["ROLL"].Points.Clear();       // Remove Roll series, no longer needed.  
            }
            cScope.Series["CH0w"].Points.Clear();           // Clear points
            cScope.Series["CH0w"].Color = DSCH0.color;
            for (int i = DSCH0.iRoll_StartX; i < DSCH0.iRoll_EndX; i++)
            {
                yd = DSCH0.cSamples[i].y;
                if (bb == true)
                    yd = yd * blScopeDataOp[0].DataGain + blScopeDataOp[0].DataOffset;
                if (!((cbRemoveZero.Checked == true) & (yd == 0.0)))
                    cScope.Series["CH0w"].Points.AddXY(i, yd);
            }

        }
        #endregion

        #region //================================================================AppendCh0Class
        // This work assuming it goes between 0 to end of data.
        // Do not use for selected range.
        // Next task is view up 1024 data element as in rolling mode, from the RHS edge of the screen.  
        public void AppendCh0Class(cPoint fdata)
        {
            if (DSCH0 == null)
                return;
            DSCH0.AppendDataSource(fdata, cScopeSetup.iMaxRollPointer);
            try
            {
                if (cScopeSetup.isRollingMode == true)
                {
                    RefreshRollingCh0Class();
                }
                else
                {
                    RefreshChartCh0Class(false);        // Append Only
                }
                DSCH0.EndRow = DSCH0.EndRow + 1;
            }
            catch { }
            cScopeUpdateAxisTitle();
        }
        #endregion

        #region //================================================================RemoveCh0
        public void RemoveCh0()
        {
            DSCH0 = null;
            cScope.Series["CH0w"].Points.Clear();       // Clear points
            this.Refresh();

            iCursorSelectChannel = 0;
            cbCursorCH0.Checked = true;
            cbCursorCH1.Checked = false;
            cbCursorCH2.Checked = false;
            cbCursorCH3.Checked = false;
            cScope.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            cScope.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            cScopeUpdateAxisTitle();
        }
        #endregion

        //==============================================================================================CH1
        #region //================================================================StartChartCh1Class (1st time start from here)
        public void StartChartCh1Class(int StartRow, int EndRow, string ColumnName, ref List<cPoint> fdata, Color linecolor)
        {
            DSCH1 = null;
            //----------------------------------------Rolling Mode (Initial Data View).
            if (cScopeSetup.isRollingMode == true)
            {
                List<cPoint> CPointnull = null;
                DSCH1 = new DataSource(ref CPointnull, "CH1", ColumnName, linecolor);
                DSCH1.StartRow = 0;
                DSCH1.EndRow = 0;
                SetupRollingClass();
                DSCH1.iRoll_StartX = 0;
                DSCH1.iRoll_EndX = 0;
                DSCH1.iRollPointer = 0;
                for (int i = StartRow; i < EndRow; i++)
                {
                    AppendCh1Class(fdata[i]);
                }
            }
            else  //------------------------------------Chart Mode
            {
                DSCH1 = new DataSource(ref fdata, "CH1", ColumnName, linecolor);
                DSCH1.StartRow = StartRow;
                DSCH1.EndRow = EndRow - 1;
                RefreshChartCh1Class(true);
            }
            cScopeUpdateAxisTitle();
        }
        #endregion

        #region //================================================================RefreshChartCh1Class
        public void RefreshChartCh1Class(bool isMethod)     // True = Whole Screen, False = Append Data Only. 
        {
            if (DSCH1 == null)
                return;
            if ((cScopeSetup.isRollingMode == true) || (DSCH1 == null))
                return;
            if (cScope.Series["ROLL"].Points.Count != 0)        // Clear Rolling Series data, not needed in this mode. 
                ChangeRollingModetoChartMode();
            bool bb = Convert.ToBoolean(dgvScopeDataOp.Rows[1].Cells[1].Value);
            double yd;
            //----------------------------------Update scope display
            try
            {
                if (isMethod == true)  // Whole screens
                {
                    //----------------------------------Manage Endrow for selected and whole range to avoid exception occurance (minor bug fixed) 
                    int EndRow = DSCH1.EndRow;
                    if (DSCH1.EndRow == DSCH1.Length)
                        EndRow--;
                    cScope.Series["CH1w"].Points.Clear();       // Clear points
                    cScope.Series["CH1w"].Color = DSCH1.color;
                    for (int i = DSCH1.StartRow; i <= EndRow; i++)
                    {
                        yd = DSCH1.cSamples[i].y;
                        //----------------------------------------------Simple Gain/Offset calculation, if check activate this. 
                        if (bb == true)
                            yd = yd * blScopeDataOp[1].DataGain + blScopeDataOp[1].DataOffset;
                        if (cScopeSetup.isTimeBaseTick == true)          // Row Number or timebase(mSec)
                        {
                            if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                                cScope.Series["CH1w"].Points.AddXY(DSCH1.cSamples[i].x, yd);
                        }
                        else
                        {
                            if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                                cScope.Series["CH1w"].Points.AddXY(i, yd);
                        }
                    }
                }
                else  // Append Data Only
                {
                    yd = DSCH1.cSamples[DSCH1.Length - 1].y;
                    //----------------------------------------------Simple Gain/Offset calculation, if check activate this. 
                    if (bb == true)
                        yd = yd * blScopeDataOp[1].DataGain + blScopeDataOp[1].DataOffset;
                    if (cScopeSetup.isTimeBaseTick == true)          // Row Number or timebase(mSec)
                    {
                        if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                            cScope.Series["CH1w"].Points.AddXY(DSCH1.cSamples[DSCH1.EndRow].x, yd);
                    }
                    else
                    {
                        if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                            cScope.Series["CH1w"].Points.AddXY(DSCH1.EndRow, yd);
                    }
                }
            }
            catch { }
        }
        #endregion

        #region //================================================================RefreshRollingCh1Class
        private void RefreshRollingCh1Class()
        {
            if (DSCH1 == null)
                return;
            double yd;
            bool bb = Convert.ToBoolean(dgvScopeDataOp.Rows[1].Cells[1].Value);
            if ((DSCH1.iRollPointer == cScopeSetup.iMaxRollPointer) & (cScope.Series["ROLL"].Points.Count != 0))
            {
                cScope.Series["ROLL"].Points.Clear();       // Remove Roll series, no longer needed.  
            }
            cScope.Series["CH1w"].Points.Clear();           // Clear points
            cScope.Series["CH1w"].Color = DSCH1.color;
            for (int i = DSCH1.iRoll_StartX; i < DSCH1.iRoll_EndX; i++)
            {
                yd = DSCH1.cSamples[i].y;
                if (bb == true)
                {
                    if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                        yd = yd * blScopeDataOp[1].DataGain + blScopeDataOp[1].DataOffset;
                }
                cScope.Series["CH1w"].Points.AddXY(i, yd);
            }
        }
        #endregion

        #region //================================================================AppendCh1Class
        // This work assuming it goes between 0 to end of data.
        // Do not use for selected range.
        // Next task is view up 1024 data element as in rolling mode, from the RHS edge of the screen.  
        public void AppendCh1Class(cPoint fdata)
        {
            if (DSCH1 == null)
                return;
            DSCH1.AppendDataSource(fdata, cScopeSetup.iMaxRollPointer);
            try
            {
                if (cScopeSetup.isRollingMode == true)
                {
                    RefreshRollingCh1Class();
                }
                else
                {
                    RefreshChartCh1Class(false);
                }
                DSCH1.EndRow = DSCH1.EndRow + 1;
            }
            catch { }
            cScopeUpdateAxisTitle();
        }
        #endregion

        #region //================================================================RemoveCh1
        public void RemoveCh1()
        {
            DSCH1 = null;
            cScope.Series["CH1w"].Points.Clear();       // Clear points
            this.Refresh();
            iCursorSelectChannel = 0;
            cbCursorCH0.Checked = true;
            cbCursorCH1.Checked = false;
            cbCursorCH2.Checked = false;
            cbCursorCH3.Checked = false;
            cScope.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            cScope.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            cScopeUpdateAxisTitle();
        }
        #endregion

        //==============================================================================================CH2
        #region //================================================================StartChartCh2Class (1st time start from here)
        public void StartChartCh2Class(int StartRow, int EndRow, string ColumnName, ref List<cPoint> fdata, Color linecolor)
        {
            DSCH2 = null;
            //----------------------------------------Rolling Mode (Initial Data View).
            if (cScopeSetup.isRollingMode == true)
            {
                List<cPoint> CPointnull = null;
                DSCH2 = new DataSource(ref CPointnull, "CH2", ColumnName, linecolor);
                DSCH2.StartRow = 0;
                DSCH2.EndRow = 0;
                SetupRollingClass();
                DSCH2.iRoll_StartX = 0;
                DSCH2.iRoll_EndX = 0;
                DSCH2.iRollPointer = 0;
                for (int i = StartRow; i < EndRow; i++)
                {
                    AppendCh2Class(fdata[i]);
                }
            }
            else  //------------------------------------Chart Mode
            {
                DSCH2 = new DataSource(ref fdata, "CH2", ColumnName, linecolor);
                DSCH2.StartRow = StartRow;
                DSCH2.EndRow = EndRow - 1;
                RefreshChartCh2Class(true);
            }
        }
        #endregion

        #region //================================================================RefreshChartCh2Class
        public void RefreshChartCh2Class(bool isMethod)     // True = Whole Screen, False = Append Data Only. 
        {
            if (DSCH2 == null)
                return;
            if ((cScopeSetup.isRollingMode == true) || (DSCH0 == null))
                return;
            if (cScope.Series["ROLL"].Points.Count != 0)        // Clear Rolling Series data, not needed in this mode. 
                ChangeRollingModetoChartMode();
            bool bb = Convert.ToBoolean(dgvScopeDataOp.Rows[2].Cells[1].Value);
            double yd;
            //----------------------------------Update scope display
            try
            {
                if (isMethod == true)  // Whole screens
                {
                    //----------------------------------Manage Endrow for selected and whole range to avoid exception occurance (minor bug fixed) 
                    int EndRow = DSCH2.EndRow;
                    if (DSCH2.EndRow == DSCH2.Length)
                        EndRow--;
                    cScope.Series["CH2w"].Points.Clear();       // Clear points
                    cScope.Series["CH2w"].Color = DSCH2.color;
                    for (int i = DSCH2.StartRow; i <= EndRow; i++)
                    {
                        yd = DSCH2.cSamples[i].y;
                        //----------------------------------------------Simple Gain/Offset calculation, if check activate this. 
                        if (bb == true)
                            yd = yd * blScopeDataOp[2].DataGain + blScopeDataOp[2].DataOffset;
                        if (cScopeSetup.isTimeBaseTick == true)          // Row Number or timebase(mSec)
                        {
                            if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                                cScope.Series["CH2w"].Points.AddXY(DSCH2.cSamples[i].x, yd);
                        }
                        else
                        {
                            if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                                cScope.Series["CH2w"].Points.AddXY(i, yd);
                        }
                    }
                }
                else  // Append Data Only
                {
                    yd = DSCH2.cSamples[DSCH2.Length - 1].y;
                    //----------------------------------------------Simple Gain/Offset calculation, if check activate this. 
                    if (bb == true)
                        yd = yd * blScopeDataOp[2].DataGain + blScopeDataOp[2].DataOffset;
                    if (cScopeSetup.isTimeBaseTick == true)          // Row Number or timebase(mSec)
                    {
                        if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                            cScope.Series["CH2w"].Points.AddXY(DSCH2.cSamples[DSCH2.EndRow].x, yd);
                    }
                    else
                    {
                        if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                            cScope.Series["CH2w"].Points.AddXY(DSCH2.EndRow, yd);
                    }
                }
            }
            catch { }
        }
        #endregion

        #region //================================================================RefreshRollingCh2Class
        private void RefreshRollingCh2Class()
        {
            if (DSCH2 == null)
                return;
            double yd;
            bool bb = Convert.ToBoolean(dgvScopeDataOp.Rows[2].Cells[1].Value);
            if ((DSCH2.iRollPointer == cScopeSetup.iMaxRollPointer) & (cScope.Series["ROLL"].Points.Count != 0))
            {
                cScope.Series["ROLL"].Points.Clear();       // Remove Roll series, no longer needed.  
            }
            cScope.Series["CH2w"].Points.Clear();           // Clear points
            cScope.Series["CH2w"].Color = DSCH2.color;
            for (int i = DSCH2.iRoll_StartX; i < DSCH2.iRoll_EndX; i++)
            {
                yd = DSCH2.cSamples[i].y;
                if (bb == true)
                    yd = yd * blScopeDataOp[2].DataGain + blScopeDataOp[2].DataOffset;
                if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                    cScope.Series["CH2w"].Points.AddXY(i, yd);
            }
        }
        #endregion

        #region //================================================================AppendCh2Class
        // This work assuming it goes between 0 to end of data.
        // Do not use for selected range.
        // Next task is view up 1024 data element as in rolling mode, from the RHS edge of the screen.  
        public void AppendCh2Class(cPoint fdata)
        {
            if (DSCH2 == null)
                return;
            DSCH2.AppendDataSource(fdata, cScopeSetup.iMaxRollPointer);
            try
            {
                if (cScopeSetup.isRollingMode == true)
                {
                    RefreshRollingCh2Class();
                }
                else
                {
                    RefreshChartCh2Class(false);
                }
                DSCH2.EndRow = DSCH2.EndRow + 1;
            }
            catch { }
            cScopeUpdateAxisTitle();
        }
        #endregion

        #region //================================================================RemoveCh2
        public void RemoveCh2()
        {
            DSCH2 = null;
            cScope.Series["CH2w"].Points.Clear();       // Clear points
            this.Refresh();
            iCursorSelectChannel = 0;
            cbCursorCH0.Checked = true;
            cbCursorCH1.Checked = false;
            cbCursorCH2.Checked = false;
            cbCursorCH3.Checked = false;
            cScope.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            cScope.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            cScopeUpdateAxisTitle();
        }
        #endregion

        //==============================================================================================CH3
        #region //================================================================StartChartCh3Class (1st time start from here)
        public void StartChartCh3Class(int StartRow, int EndRow, string ColumnName, ref List<cPoint> fdata, Color linecolor)
        {
            DSCH3 = null;
            //----------------------------------------Rolling Mode (Initial Data View).
            if (cScopeSetup.isRollingMode == true)
            {
                List<cPoint> CPointnull = null;
                DSCH3 = new DataSource(ref CPointnull, "CH3", ColumnName, linecolor);
                DSCH3.StartRow = 0;
                DSCH3.EndRow = 0;
                SetupRollingClass();
                DSCH3.iRoll_StartX = 0;
                DSCH3.iRoll_EndX = 0;
                DSCH3.iRollPointer = 0;
                for (int i = StartRow; i < EndRow; i++)
                {
                    AppendCh3Class(fdata[i]);
                }
            }
            else  //------------------------------------Chart Mode
            {
                DSCH3 = new DataSource(ref fdata, "CH3", ColumnName, linecolor);
                DSCH3.StartRow = StartRow;
                DSCH3.EndRow = EndRow - 1;
                RefreshChartCh3Class(true);
            }
            cScopeUpdateAxisTitle();
        }
        #endregion

        #region //================================================================RefreshChartCh3Class
        public void RefreshChartCh3Class(bool isMethod)     // True = Whole Screen, False = Append Data Only. 
        {
            if (DSCH3 == null)
                return;
            if ((cScopeSetup.isRollingMode == true) || (DSCH3 == null))
                return;
            if (cScope.Series["ROLL"].Points.Count != 0)        // Clear Rolling Series data, not needed in this mode. 
                ChangeRollingModetoChartMode();
            bool bb = Convert.ToBoolean(dgvScopeDataOp.Rows[3].Cells[1].Value);
            double yd;
            //----------------------------------Update scope display
            try
            {
                if (isMethod == true)  // Whole screens
                {
                    //----------------------------------Manage Endrow for selected and whole range to avoid exception occurance (minor bug fixed) 
                    int EndRow = DSCH3.EndRow;
                    if (DSCH3.EndRow == DSCH3.Length)
                        EndRow--;
                    cScope.Series["CH3w"].Points.Clear();       // Clear points
                    cScope.Series["CH3w"].Color = DSCH3.color;
                    for (int i = DSCH3.StartRow; i <= EndRow; i++)
                    {
                        yd = DSCH3.cSamples[i].y;
                        //----------------------------------------------Simple Gain/Offset calculation, if check activate this. 
                        if (bb == true)
                            yd = yd * blScopeDataOp[3].DataGain + blScopeDataOp[3].DataOffset;
                        if (cScopeSetup.isTimeBaseTick == true)          // Row Number or timebase(mSec)
                        {
                            if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                                cScope.Series["CH3w"].Points.AddXY(DSCH3.cSamples[i].x, yd);
                        }
                        else
                        {
                            if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                                cScope.Series["CH3w"].Points.AddXY(i, yd);
                        }
                    }
                }
                else  // Append Data Only
                {
                    yd = DSCH3.cSamples[DSCH3.Length - 1].y;
                    //----------------------------------------------Simple Gain/Offset calculation, if check activate this. 
                    if (bb == true)
                        yd = yd * blScopeDataOp[3].DataGain + blScopeDataOp[3].DataOffset;
                    if (cScopeSetup.isTimeBaseTick == true)          // Row Number or timebase(mSec)
                    {
                        if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                            cScope.Series["CH3w"].Points.AddXY(DSCH3.cSamples[DSCH3.EndRow].x, yd);
                    }
                    else
                    {
                        if ((cbRemoveZero.Checked == true) & (yd != 0.0))
                            cScope.Series["CH3w"].Points.AddXY(DSCH3.EndRow, yd);
                    }
                }
            }
            catch { }
        }
        #endregion

        #region //================================================================RefreshRollingCh3Class
        private void RefreshRollingCh3Class()
        {
            if (DSCH3 == null)
                return;
            double yd;
            bool bb = Convert.ToBoolean(dgvScopeDataOp.Rows[3].Cells[1].Value);
            if ((DSCH3.iRollPointer == cScopeSetup.iMaxRollPointer) & (cScope.Series["ROLL"].Points.Count != 0))
            {
                cScope.Series["ROLL"].Points.Clear();       // Remove Roll series, no longer needed.  
            }
            cScope.Series["CH3w"].Points.Clear();           // Clear points
            cScope.Series["CH3w"].Color = DSCH3.color;
            for (int i = DSCH3.iRoll_StartX; i < DSCH3.iRoll_EndX; i++)
            {
                yd = DSCH3.cSamples[i].y;
                if (bb == true)
                    yd = yd * blScopeDataOp[3].DataGain + blScopeDataOp[3].DataOffset;
                cScope.Series["CH3w"].Points.AddXY(i, yd);
            }
        }
        #endregion

        #region //================================================================AppendCh3Class
        // This work assuming it goes between 0 to end of data.
        // Do not use for selected range.
        // Next task is view up 1024 data element as in rolling mode, from the RHS edge of the screen.  
        public void AppendCh3Class(cPoint fdata)
        {
            if (DSCH3 == null)
                return;
            DSCH3.AppendDataSource(fdata, cScopeSetup.iMaxRollPointer);
            try
            {
                if (cScopeSetup.isRollingMode == true)
                {
                    RefreshRollingCh3Class();
                }
                else
                {
                    RefreshChartCh3Class(false);
                }
                DSCH3.EndRow = DSCH3.EndRow + 1;
            }
            catch { }
            cScopeUpdateAxisTitle();
        }
        #endregion

        #region //================================================================RemoveCh3
        public void RemoveCh3()
        {
            DSCH3 = null;
            cScope.Series["CH3w"].Points.Clear();       // Clear points
            this.Refresh();
            iCursorSelectChannel = 0;
            cbCursorCH0.Checked = true;
            cbCursorCH1.Checked = false;
            cbCursorCH2.Checked = false;
            cbCursorCH3.Checked = false;
            cScope.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            cScope.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            cScopeUpdateAxisTitle();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Mouse Control
        //#####################################################################################################

        #region //================================================================cScope_MouseLeave
        void cScope_MouseLeave(object sender, EventArgs e)
        {
            if (cScope.Focused) cScope.Parent.Focus();
        }
        #endregion

        #region //================================================================cScope_MouseEnter
        void cScope_MouseEnter(object sender, EventArgs e)
        {
            if (!cScope.Focused) cScope.Focus();
        }
        #endregion

        #region //================================================================cScope_MouseWheel

        // http://www.experts-exchange.com/questions/28927344/C-VS2015-what-the-best-way-to-zoom-professionally-with-mousewheel.html

        int zoomx = 0,      // zoom percentage along X axis
            zoomy = 0,      // zoom percentage along Y axis
            maxzoom = 50,   // max zoom level
            changezoom = 1; // you can change by more than 1 to make it faster
        private void cScope_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                int deltazoom = Math.Sign(e.Delta);
                if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
                {
                    if (deltazoom == 1 && zoomx < maxzoom)
                    {
                        zoomx += changezoom;
                    }
                    else if (deltazoom == -1 && zoomx > 0)
                    {
                        zoomx -= changezoom;
                    }
                    double posX = cScope.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X);
                    double sizeX = (cScope.ChartAreas[0].AxisX.Maximum - cScope.ChartAreas[0].AxisX.Minimum) * (maxzoom - zoomx) / maxzoom / 2;
                    double posXStart = posX - sizeX;
                    double posXFinish = posX + sizeX;
                    if (posXStart < cScope.ChartAreas[0].AxisX.Minimum)
                    {
                        posXFinish += cScope.ChartAreas[0].AxisX.Minimum - posXStart;
                        posXStart = cScope.ChartAreas[0].AxisX.Minimum;
                    }
                    if (posXFinish > cScope.ChartAreas[0].AxisX.Maximum)
                    {
                        posXStart -= posXFinish - cScope.ChartAreas[0].AxisX.Maximum;
                        posXFinish = cScope.ChartAreas[0].AxisX.Maximum;
                    }
                    cScope.ChartAreas[0].AxisX.ScaleView.Zoom(Math.Round(posXStart, 0), Math.Round(posXFinish, 0));
                }
                //-------------------------------------------------------------------Y Axis with 
                if ((Control.ModifierKeys & Keys.Control) != Keys.None)
                {
                    if (deltazoom == 1 && zoomy < maxzoom)
                    {
                        zoomy += changezoom;
                    }
                    else if (deltazoom == -1 && zoomy > 0)
                    {
                        zoomy -= changezoom;
                    }
                    double posY = cScope.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y);
                    double sizeY = (cScope.ChartAreas[0].AxisY.Maximum - cScope.ChartAreas[0].AxisY.Minimum) * (maxzoom - zoomy) / maxzoom / 2;
                    double posYStart = posY - sizeY;
                    double posYFinish = posY + sizeY;
                    if (posYStart < cScope.ChartAreas[0].AxisY.Minimum)
                    {
                        posYFinish += cScope.ChartAreas[0].AxisY.Minimum - posYStart;
                        posYStart = cScope.ChartAreas[0].AxisY.Minimum;
                    }
                    if (posYFinish > cScope.ChartAreas[0].AxisY.Maximum)
                    {
                        posYStart -= posYFinish - cScope.ChartAreas[0].AxisY.Maximum;
                        posYFinish = cScope.ChartAreas[0].AxisY.Maximum;
                    }
                    cScope.ChartAreas[0].AxisY.ScaleView.Zoom(Math.Round(posYStart, 0), Math.Round(posYFinish, 0));
                }
            }
            catch { }
        }

        #endregion

        //#####################################################################################################
        //###################################################################################### Misc Items
        //#####################################################################################################

        #region //================================================================btnZoom_Click
        private void btnZoom_Click(object sender, EventArgs e)
        {
            if (btnZoom.Text == "Y-Zoom")
            {
                btnZoom.Text = "X-Zoom";
                //-------------------------------------------------------------Turn off the zoom for X-axis.
                cScope.ChartAreas[0].CursorX.IsUserEnabled = false;
                cScope.ChartAreas[0].CursorX.IsUserSelectionEnabled = false;
                cScope.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
                //-------------------------------------------------------------Turn on the zoom on Y-Axis.
                cScope.ChartAreas[0].CursorY.IsUserEnabled = true;
                cScope.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
                cScope.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
                //cScope.ChartAreas[0].AxisY.ScrollBar.Enabled = false;                 //### Scale bug, fix this 1st before using Y zoom feature
                //FFTGraph.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;
                //FFTGraph.ChartAreas[0].CursorY.Interval = 1;
            }
            else
            {
                btnZoom.Text = "Y-Zoom";
                //-------------------------------------------------------------Turn on the zoom for X-axis.
                cScope.ChartAreas[0].CursorX.IsUserEnabled = true;
                cScope.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                cScope.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                //-------------------------------------------------------------Turn off the zoom on Y-Axis.
                cScope.ChartAreas[0].CursorY.IsUserEnabled = false;
                cScope.ChartAreas[0].CursorY.IsUserSelectionEnabled = false;
                cScope.ChartAreas[0].AxisY.ScaleView.Zoomable = false;
            }
        }
        #endregion

        #region //================================================================btnResetZoom_Click
        private void btnResetZoom_Click(object sender, EventArgs e)
        {
            cScope.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            cScope.ChartAreas[0].AxisY.ScaleView.ZoomReset();

        }
        #endregion

        #region //================================================================btnMarker_Click
        private void btnMarker_Click(object sender, EventArgs e)
        {
            foreach (var series in cScope.Series)
            {
                try
                {
                    if (series.Enabled == true)
                    {
                        if (series.Name != "ROLL")      // Does not apply to ROLL series. 
                        {
                            if (series.MarkerStyle == MarkerStyle.None)
                            {
                                series.MarkerStyle = MarkerStyle.Circle;
                                series.MarkerColor = System.Drawing.Color.LightGoldenrodYellow;
                                series.MarkerSize = 3;
                            }
                            else
                            {
                                series.MarkerStyle = MarkerStyle.None;
                            }
                        }
                    }
                }
                catch { }
            }
        }
        #endregion

        #region //================================================================Experiment
        public void Experiment()
        {
            List<DataSource> Channel;
            Channel = new List<DataSource>();
            List<cPoint> cPointnull = null;
            DataSource CH0 = new DataSource(ref cPointnull, "CH0", "1", Color.Yellow);
            DataSource CH1 = new DataSource(ref cPointnull, "CH1", "2", Color.MediumSeaGreen);
            DataSource CH2 = new DataSource(ref cPointnull, "CH2", "3", Color.Cyan);
            DataSource CH3 = new DataSource(ref cPointnull, "CH3", "4", Color.MediumPurple);

            Channel.Add(CH0);
            Channel.Add(CH1);
            Channel.Add(CH2);
            Channel.Add(CH3);

            myTestWaveGen.CalcSinusFunction_Riscy(Channel[0], 5, 0.5);
            myTestWaveGen.CalcSinusFunction_Riscy(Channel[1], 10, 0.5);
            myTestWaveGen.CalcSinusFunction_Riscy(Channel[2], 200, 0.3);
            myTestWaveGen.CalcSinusFunction_Riscy(Channel[3], 100, 0.7);

            for (int i = 0; i < 800; i++)
            {
                cScope.Series["CH0w"].Points.Add(Channel[0].cSamples[i].y);
                cScope.Series["CH1w"].Points.Add(Channel[1].cSamples[i].y);
                cScope.Series["CH2w"].Points.Add(Channel[2].cSamples[i].y);
                cScope.Series["CH3w"].Points.Add(Channel[3].cSamples[i].y);
            }
        }
        #endregion

        #region //================================================================cScope_DefaultFolder
        public void cScope_DefaultFolder(string sdefaultFolder)   // If default folder left blank, then use user document file location. 
        {
            try
            {
                if (sDefaultFolder == "")
                {
                    if (sdefaultFolder == "")       // If empty then use user myDocument location. 
                    {
                        sDefaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        sDefaultFolder = sDefaultFolder + @"\RiscyScope";
                    }
                    else                            // User defined folder. 
                    {
                        sDefaultFolder = sdefaultFolder;
                    }
                    System.IO.Directory.CreateDirectory(sDefaultFolder);
                }
                else  // sDefaultFolder was defined, so check folder for existence if not create it. 
                {
                    sDefaultFolder = sdefaultFolder + @"\RiscyScope";
                    System.IO.Directory.CreateDirectory(sDefaultFolder);
                }
            }
            catch
            {
                sDefaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                sDefaultFolder = sDefaultFolder + @"\RiscyScope";
                System.IO.Directory.CreateDirectory(sDefaultFolder);
            }
        }
        #endregion

        #region //================================================================Event_dgvScopeDataOp_Subscribe
        private void Event_dgvScopeDataOp_Subscribe(bool enabled)
        {
            if (!enabled)
            {
                this.dgvScopeDataOp.CellMouseClick -= new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvScopeDataOp_CellMouseClick);
                this.dgvScopeDataOp.CellValueChanged -= new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvScopeDataOp_CellValueChanged);
            }
            else if (!mDgvScopeDataOpSubscribed)
            {
                this.dgvScopeDataOp.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvScopeDataOp_CellMouseClick);
                this.dgvScopeDataOp.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvScopeDataOp_CellValueChanged);
            }

            mDgvScopeDataOpSubscribed = enabled;
        }
        #endregion

        #region //================================================================tbRollRange_KeyUp
        private void tbRollRange_KeyUp(object sender, KeyEventArgs e)
        {
           if (e.KeyCode == Keys.Enter)
            {
                string ss = tbRollRange.Text;
                if (Tools.IsString_Numberic_UInt32(ss))
                    cScopeSetup.iMaxRollPointer = Tools.ConversionStringtoInt32(ss);
                if (cScopeSetup.isRollingMode==true)
                {
                    SetupRollingClass();
                    ReInitRollChart();
                }
                ScopeDataOp_UpdateGainOffset(0);
                ScopeDataOp_UpdateGainOffset(1);
                ScopeDataOp_UpdateGainOffset(2);
                ScopeDataOp_UpdateGainOffset(3);
            }
        }
        #endregion

        #region //================================================================RefreshUpdateData
        private void RefreshUpdateData()
        {
            ScopeDataOp_UpdateGainOffset(0);
            ScopeDataOp_UpdateGainOffset(1);
            ScopeDataOp_UpdateGainOffset(2);
            ScopeDataOp_UpdateGainOffset(3);
        }
        #endregion

        #region //================================================================cbRemoveZero_CheckedChanged
        private void cbRemoveZero_CheckedChanged(object sender, EventArgs e)
        {
            RefreshUpdateData();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Measurement Cursor
        //#####################################################################################################

        #region //================================================================cbCursorCH0_MouseClick 
        private void cbCursorCH0_MouseClick(object sender, MouseEventArgs e)
        {
            cbCursorCH0.Checked = false;
            cbCursorCH1.Checked = false;
            cbCursorCH2.Checked = false;
            cbCursorCH3.Checked = false;
            bool isValid = true;
            if (DSCH0 == null)          // Check for null object
            {
                isValid = false;
            }
            else
            {
                if (DSCH0.Length == 0)  // Then check for data contents.
                    isValid = false;
            }
            if (isValid == false)
            {
                switch (iCursorSelectChannel)
                {
                    case 0:
                        {
                            cbCursorCH0.Checked = true;
                            break;
                        }
                    case 1:
                        {
                            cbCursorCH1.Checked = true;
                            break;
                        }
                    case 2:
                        {
                            cbCursorCH2.Checked = true;
                            break;
                        }
                    case 3:
                        {
                            cbCursorCH3.Checked = true;
                            break;
                        }
                    default:
                        break;
                }
                return;
            }
            try
            {
                cbCursorCH0.Checked = true;
                iCursorSelectChannel = 0;
                tbxAxisYAmp.BackColor = DSCH0.color;
            }
            catch { }

        }
        #endregion

        #region //================================================================cbCursorCH1_MouseClick 
        private void cbCursorCH1_MouseClick(object sender, MouseEventArgs e)
        {
            cbCursorCH0.Checked = false;
            cbCursorCH1.Checked = false;
            cbCursorCH2.Checked = false;
            cbCursorCH3.Checked = false;
            bool isValid = true;
            if (DSCH1 == null)          // Check for null object
            {
                isValid = false;
            }
            else
            {
                if (DSCH1.Length == 0)  // Then check for data contents.
                    isValid = false;
            }
            if (isValid == false)
            {
                switch (iCursorSelectChannel)
                {
                    case 0:
                        {
                            cbCursorCH0.Checked = true;
                            break;
                        }
                    case 1:
                        {
                            cbCursorCH1.Checked = true;
                            break;
                        }
                    case 2:
                        {
                            cbCursorCH2.Checked = true;
                            break;
                        }
                    case 3:
                        {
                            cbCursorCH3.Checked = true;
                            break;
                        }
                    default:
                        break;
                }
                return;
            }

            try
            {
                cbCursorCH1.Checked = true;
                iCursorSelectChannel = 1;
                tbxAxisYAmp.BackColor = DSCH1.color;
            }
            catch { }

        }
        #endregion

        #region //================================================================cbCursorCH2_MouseClick 
        private void cbCursorCH2_MouseClick(object sender, MouseEventArgs e)
        {
            cbCursorCH0.Checked = false;
            cbCursorCH1.Checked = false;
            cbCursorCH2.Checked = false;
            cbCursorCH3.Checked = false;
            bool isValid = true;
            if (DSCH2 == null)          // Check for null object
            {
                isValid = false;
            }
            else
            {
                if (DSCH2.Length == 0)  // Then check for data contents.
                    isValid = false;
            }
            if (isValid == false)
            {
                switch (iCursorSelectChannel)
                {
                    case 0:
                        {
                            cbCursorCH0.Checked = true;
                            break;
                        }
                    case 1:
                        {
                            cbCursorCH1.Checked = true;
                            break;
                        }
                    case 2:
                        {
                            cbCursorCH2.Checked = true;
                            break;
                        }
                    case 3:
                        {
                            cbCursorCH3.Checked = true;
                            break;
                        }
                    default:
                        break;
                }
                return;
            }
            try
            {
                cbCursorCH2.Checked = true;
                iCursorSelectChannel = 2;
                tbxAxisYAmp.BackColor = DSCH2.color;
            }
            catch { }
        }
        #endregion

        #region //================================================================cbCursorCH3_MouseClick 
        private void cbCursorCH3_MouseClick(object sender, MouseEventArgs e)
        {
            cbCursorCH0.Checked = false;
            cbCursorCH1.Checked = false;
            cbCursorCH2.Checked = false;
            cbCursorCH3.Checked = false;
            bool isValid = true;
            if (DSCH3 == null)          // Check for null object
            {
                isValid = false;
            }
            else
            {
                if (DSCH3.Length == 0)  // Then check for data contents.
                    isValid = false;
            }
            if (isValid == false)
            {
                switch (iCursorSelectChannel)
                {
                    case 0:
                        {
                            cbCursorCH0.Checked = true;
                            break;
                        }
                    case 1:
                        {
                            cbCursorCH1.Checked = true;
                            break;
                        }
                    case 2:
                        {
                            cbCursorCH2.Checked = true;
                            break;
                        }
                    case 3:
                        {
                            cbCursorCH3.Checked = true;
                            break;
                        }
                    default:
                        break;
                }
                return;
            }
            try
            {
                cbCursorCH3.Checked = true;
                iCursorSelectChannel = 3;
                tbxAxisYAmp.BackColor = DSCH3.color;
            }
            catch { }
        }
        #endregion

        #region //================================================================cScope_MouseMove 
        //http://stackoverflow.com/questions/32259371/c-sharp-chart-multiple-questions-about-chartareas-cusor-and-reading-x-y-values
        // http://stackoverflow.com/questions/33899354/get-y-value-of-series-from-the-x-cursor-position-where-a-chartarea-is-clicked-on/33900161
        Point prevPos;
        private void cScope_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < 0 || e.Y < 0 || e.Location == prevPos)
                return;
            prevPos = e.Location;
            PointF CursorPointer = new PointF(e.Location.X, e.Location.Y);
            cScope.ChartAreas[0].CursorY.SetCursorPixelPosition(CursorPointer, true);
            
            //cScope.ChartAreas[0].CursorX.SetCursorPixelPosition(CursorPointer, true);
            double searchVal;
            Point p = new Point(e.X, e.Y);
            try
            {
                searchVal = cScope.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
            }
            catch { return; }
            switch (iCursorSelectChannel)
            {
                case (0):
                    {
                        if (DSCH0 != null)
                        {
                            cScopeMouseMoveChannel(0, searchVal);
                            tbxAxisYAmp.BackColor = DSCH0.color;
                        }
                        break;
                    }
                case (1):
                    {
                        if (DSCH1 != null)
                        {
                            cScopeMouseMoveChannel(1, searchVal);
                            tbxAxisYAmp.BackColor = DSCH1.color;
                        }
                        break;
                    }
                case (2):
                    {
                        if (DSCH2 != null)
                        {
                            cScopeMouseMoveChannel(2, searchVal);
                            tbxAxisYAmp.BackColor = DSCH2.color;
                        }
                        break;
                    }
                case (3):
                    {
                        if (DSCH3 != null)
                        {
                            cScopeMouseMoveChannel(3, searchVal);
                            tbxAxisYAmp.BackColor = DSCH3.color;
                        }
                        break;
                    }
                default:
                    return;
            }
        }
        #endregion

        #region //================================================================cScopeMouseMoveChannel 
        private void cScopeMouseMoveChannel(int cc, double searchVal)
        {
            double xx = 0;
            //double[] yy;
            double x0 = 0, x1 = 0, x2 = 0;
            double xup = 0, xdown = 0;
            int i = 0;
            try
            {
                while (i <= cScope.Series[cc].Points.Count - 1)
                {
                    if (i == 0)
                    {
                        xup = (cScope.Series[cc].Points[i + 1].XValue / 2);
                        if ((searchVal >= 0) & (searchVal <= xup))
                        {
                            xx = cScope.Series[cc].Points[0].XValue;
                            cScope.ChartAreas[0].CursorX.SetCursorPosition(xx);
                            foreach (double yD in cScope.Series[cc].Points[0].YValues)
                            {
                                CScopeUpdateAxisYAmpReadout(yD);
                            }
                            break;
                        }

                    }
                    else if (i == cScope.Series[cc].Points.Count - 1)
                    {
                        x0 = cScope.Series[cc].Points[i - 1].XValue;
                        x1 = cScope.Series[cc].Points[i].XValue;
                        xdown = x1 - ((x1 - x0) / 2);
                        if ((searchVal >= xdown))
                        {
                            xx = cScope.Series[cc].Points[i].XValue;
                            cScope.ChartAreas[0].CursorX.SetCursorPosition(xx);
                            foreach (double yD in cScope.Series[cc].Points[i].YValues)
                            {
                                CScopeUpdateAxisYAmpReadout(yD);
                            }
                            break;
                        }

                    }
                    else
                    {
                        x0 = cScope.Series[cc].Points[i - 1].XValue;
                        x1 = cScope.Series[cc].Points[i].XValue;
                        x2 = cScope.Series[cc].Points[i + 1].XValue;
                        xdown = x1 - ((x1 - x0) / 2);
                        xup = x1 + ((x2 - x1) / 2);
                        if ((searchVal >= xdown) & (searchVal <= xup))
                        {
                            xx = cScope.Series[cc].Points[i].XValue;
                            cScope.ChartAreas[0].CursorX.SetCursorPosition(xx);
                            foreach (double yD in cScope.Series[cc].Points[i].YValues)
                            {
                                CScopeUpdateAxisYAmpReadout(yD);
                            }
                            break;
                        }
                    }
                    i++;
                }
            }
            catch
            {
                tbxAxisXTime.Text = "";
                tbxAxisYAmp.Text = "";
            }
            tbxAxisXTime.Text = string.Format("{0}", xx);
        }
        #endregion

        #region //================================================================CScopeUpdateAxisYAmpReadout 
        private void CScopeUpdateAxisYAmpReadout(double yD)
        {
            //tbxAxisYAmp.Text = string.Format("{0}", yD.ToString());
            tbxAxisYAmp.Text = string.Format("{0:#,0.#####}", yD);
            cScope.ChartAreas[0].CursorY.SetCursorPosition(yD);         //###TASK, fix ghosting lines, when snapped update this statement. 
            // string.Format("{0:#,0.########}", yourDouble);
        }
        #endregion
        
        #region //================================================================cScopexx_MouseMove 
        private void cScopexx_MouseMove(object sender, MouseEventArgs e)
        {

            try
            {
                Point p = new Point(e.X, e.Y);
                double searchVal = cScope.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                double xx = 0;
                foreach (DataPoint dp in cScope.Series[0].Points)
                {
                    if (dp.XValue >= searchVal)
                    {
                        xx = dp.XValue;
                        cScope.ChartAreas[0].CursorX.SetCursorPosition(dp.XValue);
                        foreach (double yD in dp.YValues)
                        {
                            tbxAxisYAmp.Text = Math.Round(yD, 4).ToString();
                        }
                        break;
                    }
                }
                tbxAxisXTime.Text = string.Format("{0:F2}", xx);
            }
            catch { }

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### TabPage: Data (ScopeData, Gain and Offset)
        //#####################################################################################################

        #region //------------------------------------------------------ScopeDataOp_Init
        private void ScopeDataOp_Init()
        {
            List<ScopeDataOp> myScopeDataOp;
            string[] name = new string[] { "CH0", "CH1", "CH2", "CH3" };
            myScopeDataOp = new List<ScopeDataOp>();
            for (int i = 0; i <= 3; i++)
            {
                myScopeDataOp.Add(new ScopeDataOp());
                myScopeDataOp[i].DataName = name[i];
            }

            //-----------------------------------------------Now we bind the above to datagridview
            //BindingList<zEVKIT_ADCOperation> blEVKITADCData;
            blScopeDataOp = new BindingList<ScopeDataOp>(myScopeDataOp);
            var source = new BindingSource(blScopeDataOp, null);
            dgvScopeDataOp.DataSource = source;
            dgvScopeDataOp.RowTemplate.Height = 20;
            //-----------------------------------------------Check Box State
        }
        #endregion

        #region //------------------------------------------------------dgvADCSetting_CellFormatting
        private void dgvScopeDataOp_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                switch (e.ColumnIndex)
                {
                    case (0):
                        {
                            string result = blScopeDataOp[e.RowIndex].DataName;
                            e.Value = result;
                            e.FormattingApplied = true;
                            if (e.RowIndex == 0)
                                e.CellStyle.BackColor = DSCH0.color;
                            if (e.RowIndex == 1)
                                e.CellStyle.BackColor = DSCH1.color;
                            if (e.RowIndex == 2)
                                e.CellStyle.BackColor = DSCH2.color;
                            if (e.RowIndex == 3)
                                e.CellStyle.BackColor = DSCH3.color;
                            break;
                        }
                    case (2):
                        {
                            double result = blScopeDataOp[e.RowIndex].DataOffset;
                            e.Value = result.ToString("N2");
                            e.FormattingApplied = true;
                            break;
                        }
                    case (3):
                        {
                            double result = blScopeDataOp[e.RowIndex].DataGain;
                            e.Value = result.ToString("N2");
                            e.FormattingApplied = true;
                            break;
                        }
                    default:
                        {
                            e.FormattingApplied = false;
                            break;
                        }
                }
            }
            catch { }

        }
        #endregion

        #region //------------------------------------------------------dgvScopeDataOp_CellValueChanged
        private void dgvScopeDataOp_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    DataGridViewRow row = dgvScopeDataOp.Rows[e.RowIndex];
                    bool bb = Convert.ToBoolean(row.Cells[1].Value);
                }
                else
                {
                    ScopeDataOp_UpdateGainOffset(e.RowIndex);
                }
            }
            catch { }
        }
        #endregion

        #region //------------------------------------------------------dgvScopeDataOp_CellMouseClick
        private void dgvScopeDataOp_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    DataGridViewRow row = dgvScopeDataOp.Rows[e.RowIndex];
                    // if (Convert.ToBoolean(row.Cells[1].Value) == true)
                    if (blScopeDataOp[e.RowIndex].DataCheckBox == true)
                    {
                        //row.Cells[1].Value = false;
                        blScopeDataOp[e.RowIndex].DataCheckBox = false;
                    }
                    else
                    {
                        //row.Cells[1].Value = true;

                        blScopeDataOp[e.RowIndex].DataCheckBox = true;
                    }
                    dgvScopeDataOp.InvalidateCell(e.RowIndex, e.ColumnIndex);
                    ScopeDataOp_UpdateGainOffset(e.RowIndex);
                }
            }
            catch { }
        }
        #endregion

        #region //------------------------------------------------------ScopeDataOp_UpdateGainOffset
        private void ScopeDataOp_UpdateGainOffset(int Channel)
        {
            switch (Channel)
            {
                case (0):
                    {
                        if (cScopeSetup.isRollingMode == true)
                        {
                            RefreshRollingCh0Class();
                        }
                        else
                        {
                            RefreshChartCh0Class(true);
                        }
                        break;
                    }
                case (1):
                    {
                        if (cScopeSetup.isRollingMode == true)
                        {
                            RefreshRollingCh1Class();
                        }
                        else
                        {
                            RefreshChartCh1Class(true);
                        }
                        break;
                    }
                case (2):
                    {
                        if (cScopeSetup.isRollingMode == true)
                        {
                            RefreshRollingCh2Class();
                        }
                        else
                        {
                            RefreshChartCh2Class(true);
                        }
                        break;
                    }
                case (3):
                    {
                        if (cScopeSetup.isRollingMode == true)
                        {
                            RefreshRollingCh3Class();
                        }
                        else
                        {
                            RefreshChartCh3Class(true);
                        }
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region //------------------------------------------------------btnDataResetChange_Click
        private void btnDataResetChange_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= 3; i++)
            {
                blScopeDataOp[i].DataGain = 1.0D;
                blScopeDataOp[i].DataOffset = 0.0;
            }
            VertAxisUpdate();
            dgvScopeDataOp.Invalidate();
            ScopeDataOp_UpdateGainOffset(0);
            ScopeDataOp_UpdateGainOffset(1);
            ScopeDataOp_UpdateGainOffset(2);
            ScopeDataOp_UpdateGainOffset(3);
        }
        #endregion

        #region //------------------------------------------------------btnNormaliseOffset_Click
        private void btnNormaliseOffset_Click(object sender, EventArgs e)
        {
            double[] iOffset = new double[4];
            int start=0;
            for (int i = 0; i < 4; i++)
                iOffset[i] = 0;
            //-----------------------CH0
            if (DSCH0!=null)
            {
                //---------------------Scan on whole data or limited to visual data.
                start = 0;
                if (cbNormaliseOffsetLimitData.Checked == true)
                    start = DSCH0.Length - cScopeSetup.iMaxRollPointer;
                if (start < 0)
                    start = 0;
                //----------------------------------------
                for (int i= start; i< DSCH0.Length;i++)
                {
                    iOffset[0] += DSCH0.cSamples[i].y;
                }
                iOffset[0] = iOffset[0] / DSCH0.Length;
                blScopeDataOp[0].DataOffset = -iOffset[0];
            }
            //-----------------------CH1
            if (DSCH1 != null)
            {
                //---------------------Scan on whole data or limited to visual data.
                start = 0;
                if (cbNormaliseOffsetLimitData.Checked == true)
                    start = DSCH1.Length - cScopeSetup.iMaxRollPointer;
                if (start < 0)
                    start = 0;
                //----------------------------------------
                for (int i = start; i < DSCH1.Length; i++)
                {
                    iOffset[1] += DSCH1.cSamples[i].y;
                }
                iOffset[1] = iOffset[1] / DSCH1.Length;
                blScopeDataOp[1].DataOffset = -iOffset[1];
            }
            //-----------------------CH2
            if (DSCH2 != null)
            {
                //---------------------Scan on whole data or limited to visual data.
                start = 0;
                if (cbNormaliseOffsetLimitData.Checked == true)
                    start = DSCH2.Length - cScopeSetup.iMaxRollPointer;
                if (start < 0)
                    start = 0;
                //----------------------------------------
                for (int i = start; i < DSCH2.Length; i++)
                {
                    iOffset[2] += DSCH2.cSamples[i].y;
                }
                iOffset[2] = iOffset[2] / DSCH2.Length;
                blScopeDataOp[2].DataOffset = -iOffset[2];
            }
            //-----------------------CH3
            if (DSCH3 != null)
            {
                //---------------------Scan on whole data or limited to visual data.
                start = 0;
                if (cbNormaliseOffsetLimitData.Checked == true)
                    start = DSCH3.Length - cScopeSetup.iMaxRollPointer;
                if (start < 0)
                    start = 0;
                //----------------------------------------
                for (int i = start; i < DSCH3.Length; i++)
                {
                    iOffset[3] += DSCH3.cSamples[i].y;
                }
                iOffset[3] = iOffset[3] / DSCH3.Length;
                blScopeDataOp[3].DataOffset = -iOffset[3];
            }
            dgvScopeDataOp.Invalidate();
            ScopeDataOp_UpdateGainOffset(0);
            ScopeDataOp_UpdateGainOffset(1);
            ScopeDataOp_UpdateGainOffset(2);
            ScopeDataOp_UpdateGainOffset(3);

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Timebase Mode Select
        //#####################################################################################################

        #region //------------------------------------------------------cbTimeBase_RowNo_CheckedChanged
        private void cbTimeBase_RowNo_MouseClick(object sender, MouseEventArgs e)
        {
            if (cScopeSetup.isRollingMode == false)
            {
                cScopeSetup.isTimeBaseTick = false;
                TimeBaseUpdate(true);
            }
            else
            {
                TimeBaseUpdate(false);
            }
            
        }
        #endregion

        #region //------------------------------------------------------cbTimeBase_mSec_CheckedChanged
        private void cbTimeBase_mSec_MouseClick(object sender, MouseEventArgs e)
        {
            if (cScopeSetup.isRollingMode==false)
            {
                cScopeSetup.isTimeBaseTick = true;
                TimeBaseUpdate(true);
            }
            else
            {
                TimeBaseUpdate(false);
            }
            
        }
        #endregion

        #region //------------------------------------------------------cbRolling_MouseClick
        private void cbRolling_MouseClick(object sender, MouseEventArgs e)
        {
            if (cScopeSetup.isRollingMode == true)
            {
                cScopeSetup.isRollingMode = false;
                TimeBaseUpdate(true);
            }
            else
            {
                cScopeSetup.isRollingMode = true;
                TimeBaseUpdate(false);
                ReInitRollChart();
                
            }
            
        }
        #endregion

        #region //------------------------------------------------------TimeBaseUpdate
        private void TimeBaseUpdate(bool isRefresh)
        {
            if (cScopeSetup.isRollingMode == true)
            {
                cScopeSetup.isTimeBaseTick = false;
                cbRolling.Checked = cScopeSetup.isRollingMode;
                cbTimeBase_mSec.Checked = cScopeSetup.isTimeBaseTick;
                cbTimeBase_RowNo.Checked = !cScopeSetup.isTimeBaseTick;
                
            }
            else
            {
                cbRolling.Checked = cScopeSetup.isRollingMode;      // which is False;
                cbTimeBase_mSec.Checked = cScopeSetup.isTimeBaseTick;
                cbTimeBase_RowNo.Checked = !cScopeSetup.isTimeBaseTick;
            }
            cScopeUpdateAxisTitle();
            if (isRefresh == true)
            {
                RefreshChartCh0Class(true);
                RefreshChartCh1Class(true);
                RefreshChartCh2Class(true);
                RefreshChartCh3Class(true);
            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Window Size Extend and Compact
        //#####################################################################################################

        #region //------------------------------------------------------btnLargeSize_Click
        private void btnLargeSize_Click(object sender, EventArgs e)
        {
            if (cScopeSetup.isSizeLarge == true)      // Go Small size
                cScopeSetup.isSizeLarge = false;
            else                                    // Go Large Size
                cScopeSetup.isSizeLarge = true;
            WindowSizeUpdate();
        }
        #endregion

        #region //------------------------------------------------------WindowSizeUpdate
        private void WindowSizeUpdate()
        {
            this.SuspendLayout();
            if (cScopeSetup.isSizeLarge==true)
            {
                btnLargeSize.Text = "Compact";
                this.Size = new Size(1400, 910);
                cScope.Size = new Size(1360, 700);
                cScope.Location = new Point(12, 29);
                groupBox3.Location = new Point(9, 730);
            }
            else
            {
                btnLargeSize.Text = "Extend";
                this.Size = new Size(900, 620);
                cScope.Size = new Size(860, 410);
                cScope.Location = new Point(12, 29);
                groupBox3.Location = new Point(9, 438);
            }
            this.ResumeLayout(false);
            //this.Refresh();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Filename for Gain/Offset Only
        //#####################################################################################################

        #region //------------------------------------------------------btnLoad_Click
        private void btnLoad_Click(object sender, EventArgs e)
        {
            //----------------------------------------------------------XML Load File Section. 
            BindingList<ScopeDataOp> blLoadData = null;
            ofdRiscyScope.Filter = "xml files (*.xml)|*.xml";
            ofdRiscyScope.InitialDirectory = sDefaultFolder;
            ofdRiscyScope.Title = "Load XML Scope Data/Gain Setting Files";
            DialogResult dr = ofdRiscyScope.ShowDialog();
            if (dr == DialogResult.OK)
            {
                blLoadData = myGlobalBase.DeserializeFromFile<BindingList<ScopeDataOp>>(ofdRiscyScope.FileName);
                if (blLoadData == null)
                {
                    MessageBox.Show("Unable to Load Gain/Offset Setting Configuration File (XML)",
                        "Load Config File Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else
                {
                    for (int i = 0; i <= 3; i++)
                    {
                        blScopeDataOp[i].DataName = blLoadData[i].DataName;
                        blScopeDataOp[i].DataGain = blLoadData[i].DataGain;
                        blScopeDataOp[i].DataOffset = blLoadData[i].DataOffset;
                    }
                }
            }
            else
            {
                MessageBox.Show("Load Gain/Offset Setting: Filename Error",
                    "Load Gain/Offset Setting File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            //----------------------------------------------------------Process captures data.
            dgvScopeDataOp.Invalidate();
            ScopeDataOp_UpdateGainOffset(0);
            ScopeDataOp_UpdateGainOffset(1);
            ScopeDataOp_UpdateGainOffset(2);
            ScopeDataOp_UpdateGainOffset(3);
        }
        #endregion

        #region //------------------------------------------------------btnDataSave_Click
        private void btnDataSave_Click(object sender, EventArgs e)
        {
            string sFilename;
            string sTimeStampNow = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
            sfdRiscyScope.Filter = "xml files (*.xml)|*.xml";
            sfdRiscyScope.InitialDirectory = sDefaultFolder;
            sFilename = System.IO.Path.Combine(sDefaultFolder, sFilanameGainOffset+"_"+sTimeStampNow);
            sfdRiscyScope.FileName = sFilename;
            sfdRiscyScope.Title = "Export to XML File for Scope Gain/Offset Setting";

            DialogResult dr = sfdRiscyScope.ShowDialog();
            if (dr == DialogResult.OK)
            {
                myGlobalBase.SerializeToFile<BindingList<ScopeDataOp>>(blScopeDataOp, sfdRiscyScope.FileName);
            }
            else
            {
                if ((dr == DialogResult.Cancel) || (dr == DialogResult.No))
                {
                    return;
                }
                MessageBox.Show("Save Configuration: Filename Error",
                    "Save Config File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Filename for Data in RiscyScope.
        //#####################################################################################################

        #region //------------------------------------------------------openFolderToolStripMenuItem_Click
        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string myPath = sDefaultFolder;
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = myPath;
            prc.Start();
        }
        #endregion

        #region //------------------------------------------------------saveDataToolStripMenuItem_Click
        private void saveDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sFN0, sFN1, sFN2, sFN3;
            string sTimeStampNow = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
            sfdRiscyScope.Filter = "xml files (*.xml)|*.xml";
            sfdRiscyScope.InitialDirectory = sDefaultFolder;
            sFN0 = System.IO.Path.Combine(sDefaultFolder, sFilanameData + "_CH0_" + sTimeStampNow);
            sfdRiscyScope.FileName = sFN0;
            sfdRiscyScope.Title = "Export to XML File of Data Collection, NB: Must have '_CH0_' in filename";
            DialogResult dr = sfdRiscyScope.ShowDialog();
            if (dr == DialogResult.OK)
            {
                try
                {
                    if (sfdRiscyScope.FileName.Contains("_CH"))
                    {
                        sFN0 = sfdRiscyScope.FileName;
                        sFN1 = sfdRiscyScope.FileName.Replace("_CH0_", "_CH1_");
                        sFN2 = sfdRiscyScope.FileName.Replace("_CH0_", "_CH2_");
                        sFN3 = sfdRiscyScope.FileName.Replace("_CH0_", "_CH3_");
                        if (DSCH0 != null)
                        {
                            if (DSCH0.Length != 0)
                                myGlobalBase.SerializeToFile<DataSource>(DSCH0, sFN0);
                        }
                        if (DSCH1 != null)
                        {
                            if (DSCH1.Length != 0)
                                myGlobalBase.SerializeToFile<DataSource>(DSCH1, sFN1);
                        }
                        if (DSCH2 != null)
                        {
                            if (DSCH2.Length != 0)
                                myGlobalBase.SerializeToFile<DataSource>(DSCH2, sFN2);
                        }
                        if (DSCH3 != null)
                        {
                            if (DSCH3.Length != 0)
                                myGlobalBase.SerializeToFile<DataSource>(DSCH3, sFN3);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Save XML Data: Filename Error, must contain '_CH0_' in filename",
                            "Save Data File Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                catch
                {
                    MessageBox.Show("Save XML Data: Internal Error, read issue, typo, etc",
                        "Save Data File Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                };

            }
            else
            {
                if ((dr == DialogResult.Cancel) || (dr == DialogResult.No))
                {
                    return;
                }
                MessageBox.Show("Save XML Data: Filename Error",
                    "Save Data File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        #endregion

        #region //------------------------------------------------------loadDataToolStripMenuItem_Click
        private void loadDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSource blLoadData = null;
            ofdRiscyScope.Filter = "xml files (*.xml)|*.xml";
            ofdRiscyScope.InitialDirectory = sDefaultFolder;
            ofdRiscyScope.Title = "Import from XML File of Data Collection, NB: Select '_CH0_' in filename";
            DialogResult dr = ofdRiscyScope.ShowDialog();
            try
            {
                if (dr == DialogResult.OK)
                {
                    blLoadData = myGlobalBase.DeserializeFromFile<DataSource>(sfdRiscyScope.FileName);
                    if (blLoadData == null)
                    {
                        MessageBox.Show("Unable to Data File (XML)",
                            "Load Data File Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (sfdRiscyScope.FileName.Contains("_CH0_"))
                        {
                            Color old;
                            if (DSCH0 != null)
                                old = DSCH0.color;
                            else
                                old = Color.Yellow;     // Default color. 
                            DSCH0 = blLoadData;         // Pass reference to DSCH0, the old data is now lost forever. 
                            DSCH0.color = old;
                            ScopeDataOp_UpdateGainOffset(0);
                        }
                        if (sfdRiscyScope.FileName.Contains("_CH1_"))
                        {
                            Color old;
                            if (DSCH1 != null)
                                old = DSCH1.color;
                            else
                                old = Color.MediumSeaGreen;     // Default color. 
                            DSCH1 = blLoadData;         // Pass reference to DSCH1, the old data is now lost forever. 
                            DSCH1.color = old;
                            ScopeDataOp_UpdateGainOffset(1);
                        }
                        if (sfdRiscyScope.FileName.Contains("_CH2_"))
                        {
                            Color old;
                            if (DSCH2 != null)
                                old = DSCH2.color;
                            else
                                old = Color.Cyan;     // Default color. 
                            DSCH2 = blLoadData;         // Pass reference to DSCH2, the old data is now lost forever. 
                            DSCH2.color = old;
                            ScopeDataOp_UpdateGainOffset(2);
                        }
                        if (sfdRiscyScope.FileName.Contains("_CH3_"))
                        {
                            Color old;
                            if (DSCH3 != null)
                                old = DSCH3.color;
                            else
                                old = Color.MediumPurple;     // Default color. 
                            DSCH3 = blLoadData;         // Pass reference to DSCH3, the old data is now lost forever. 
                            DSCH3.color = old;
                            ScopeDataOp_UpdateGainOffset(3);
                        }
                    }
                }
                else
                {
                    if ((dr == DialogResult.Cancel) || (dr == DialogResult.No))
                    {
                        return;
                    }
                    MessageBox.Show("Load Data: Filename Error",
                        "Load Gain/Offset Setting File Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch
            {
            }
            //----------------------------------------------------------Process captures data.

            ScopeDataOp_UpdateGainOffset(1);
            ScopeDataOp_UpdateGainOffset(2);
            ScopeDataOp_UpdateGainOffset(3);


        }
        #endregion

        #region //------------------------------------------------------saveDataCVSToolStripMenuItem_Click
        private void saveDataCVSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------Safety Check.
            if (DSCH0 == null)
                return;
            if (DSCH0.Length == 0)
                return;
            //------------------------------------------------------------
            SaveFileDialog sfdSaveCVSData = new SaveFileDialog();
            string sTimeStampNow = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
            sfdSaveCVSData.Filter = "csv files (*.csv)|*.csv";
            sfdSaveCVSData.InitialDirectory = sDefaultFolder;
            sfdSaveCVSData.FileName = System.IO.Path.Combine(sDefaultFolder, sFilanameData + "_CVS_" + sTimeStampNow); ;
            sfdSaveCVSData.Title = "Export to CVS File for Setup";
            //-----------------------------------------------------------
            StringBuilder sb = new StringBuilder();
            string ss;
            sb.AppendLine("This is saved from Universal Scope. All data are not adjusted by offset/gains");
            ss = "DataNumber, CH0_Time, CH0_Data, CH1_Time, CH1_Data,CH2_Time, CH2_Data,CH3_Time, CH3_Data";
            sb.AppendLine(ss);
            try
            {
                for (int i = 0; i < DSCH0.Length; i++)
                {
                    ss = i.ToString() + ",";
                    if (i < DSCH0.Length)
                        ss += DSCH0.cSamples[i].x.ToString() + "," + DSCH0.cSamples[i].y.ToString() + ",";
                    else
                        ss += "0,0,";
                    if (i < DSCH1.Length)
                        ss += DSCH1.cSamples[i].x.ToString() + "," + DSCH1.cSamples[i].y.ToString() + ",";
                    else
                        ss += "0,0,";
                    if (i < DSCH2.Length)
                        ss += DSCH2.cSamples[i].x.ToString() + "," + DSCH2.cSamples[i].y.ToString() + ",";
                    else
                        ss += "0,0,";
                    if (i < DSCH3.Length)
                        ss += DSCH3.cSamples[i].x.ToString() + "," + DSCH3.cSamples[i].y.ToString() + ",";
                    else
                        ss += "0,0,";
                    sb.AppendLine(ss);
                }
                DialogResult dr = sfdSaveCVSData.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(sfdSaveCVSData.FileName);
                    sw.Write(sb.ToString());
                    sw.Close();
                    Trace.WriteLine("-INFO: Command Table Saved!");
                }
                else
                {
                    Trace.WriteLine("+WARN: Filename Not Valid / Rejected Export Procedure");
                }
            }
            catch
            {
                Trace.WriteLine("###ERR: Problem in saving CVS data! (saveDataCVSToolStripMenuItem_Click)");
                MessageBox.Show("Error: Unable to save CVS data into filename", "File Error",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Error);

            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Vert Axis Controls
        //#####################################################################################################

        #region //------------------------------------------------------VertAxis_Init
        private void VertAxis_Init()
        {
            this.rbCH1LHS.BackColor = System.Drawing.Color.Yellow;
            this.rbCH1LHS.UseVisualStyleBackColor = false;
            this.rbCH2LHS.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.rbCH2LHS.UseVisualStyleBackColor = false;
            this.rbCH3LHS.BackColor = System.Drawing.Color.Cyan;
            this.rbCH3LHS.UseVisualStyleBackColor = false;
            this.rbCH4LHS.BackColor = System.Drawing.Color.LightPink;
            this.rbCH4LHS.UseVisualStyleBackColor = false;

            this.rbCH1RHS.BackColor = System.Drawing.Color.Transparent;
            this.rbCH1RHS.UseVisualStyleBackColor = false;
            this.rbCH2RHS.BackColor = System.Drawing.Color.Transparent;
            this.rbCH2RHS.UseVisualStyleBackColor = false;
            this.rbCH3RHS.BackColor = System.Drawing.Color.Transparent;
            this.rbCH3RHS.UseVisualStyleBackColor = false;
            this.rbCH4RHS.BackColor = System.Drawing.Color.Transparent;
            this.rbCH4RHS.UseVisualStyleBackColor = false;

            chLHSAutoScale.Checked = true;
            chRHSAutoScale.Checked = true;

            tbLHSMax.Text = cScopeSetup.VertLHSMax.ToString();
            tbLHSMin.Text = cScopeSetup.VertLHSMin.ToString();
            tbRHSMax.Text = cScopeSetup.VertRHSMax.ToString();
            tbRHSMin.Text = cScopeSetup.VertRHSMin.ToString();
            tbLHSMajor.Text = cScopeSetup.VertLHSMajorAxis.ToString();
            tbRHSMajor.Text = cScopeSetup.VertRHSMajorAxis.ToString();

            cScope.Series[0].YAxisType = AxisType.Primary;
            cScope.Series[1].YAxisType = AxisType.Primary;
            cScope.Series[2].YAxisType = AxisType.Primary;
            cScope.Series[3].YAxisType = AxisType.Primary;

            //Define RHS Vert axis (LHS)
            //cScope.ChartAreas["ScopeArea"].AxisY.LineColor = Color.DarkGoldenrod;
            cScope.ChartAreas["ScopeArea"].AxisY.MajorGrid.Enabled = true;
            cScope.ChartAreas["ScopeArea"].AxisY.Enabled = AxisEnabled.True;
            //cScope.ChartAreas["ScopeArea"].AxisY.IsStartedFromZero = cScope.ChartAreas["ScopeArea"].AxisY.IsStartedFromZero;
            //cScope.ChartAreas["ScopeArea"].AxisY.Title = "LHS Scale";
            cScope.ChartAreas["ScopeArea"].AxisY.LabelStyle.ForeColor = System.Drawing.Color.Yellow;
            cScope.ChartAreas["ScopeArea"].AxisY.MajorGrid.LineColor = System.Drawing.Color.Green;
            cScope.ChartAreas["ScopeArea"].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            cScope.ChartAreas["ScopeArea"].AxisY.MinorTickMark.LineColor = System.Drawing.Color.YellowGreen;
            cScope.ChartAreas["ScopeArea"].AxisY.MinorTickMark.LineDashStyle = ChartDashStyle.Dash;
            cScope.ChartAreas["ScopeArea"].AxisY.ScrollBar.ButtonColor = System.Drawing.SystemColors.ControlDark;
            cScope.ChartAreas["ScopeArea"].AxisY.ScrollBar.Size = 10D;
            cScope.ChartAreas["ScopeArea"].AxisY.TitleForeColor = System.Drawing.Color.Yellow;


            //Define RHS Vert axis (RHS)
            //cScope.ChartAreas["ScopeArea"].AxisY2.LineColor = Color.DarkGoldenrod;
            cScope.ChartAreas["ScopeArea"].AxisY2.MajorGrid.Enabled = true;
            cScope.ChartAreas["ScopeArea"].AxisY2.Enabled = AxisEnabled.False;
            //cScope.ChartAreas["ScopeArea"].AxisY2.IsStartedFromZero = cScope.ChartAreas["ScopeArea"].AxisY.IsStartedFromZero;
            //cScope.ChartAreas["ScopeArea"].AxisY2.Title = "RHS Scale";
            cScope.ChartAreas["ScopeArea"].AxisY2.LabelStyle.ForeColor = System.Drawing.Color.Yellow;
            cScope.ChartAreas["ScopeArea"].AxisY2.MajorGrid.LineColor = System.Drawing.Color.DarkCyan;
            cScope.ChartAreas["ScopeArea"].AxisY2.MajorGrid.LineDashStyle = ChartDashStyle.DashDot;
            cScope.ChartAreas["ScopeArea"].AxisY2.MinorTickMark.LineColor = System.Drawing.Color.DarkCyan;
            cScope.ChartAreas["ScopeArea"].AxisY2.MinorTickMark.LineDashStyle = ChartDashStyle.DashDot;
            cScope.ChartAreas["ScopeArea"].AxisY2.ScrollBar.ButtonColor = System.Drawing.SystemColors.ControlDark;
            cScope.ChartAreas["ScopeArea"].AxisY2.ScrollBar.Size = 10D;
            cScope.ChartAreas["ScopeArea"].AxisY2.TitleForeColor = System.Drawing.Color.Yellow;

            cScope.Update();

        }
        #endregion


        #region //------------------------------------------------------VertAxisUpdate
        private void VertAxisUpdate()
        {
            #region //---------------------------------------------------------Update channel to LHS or RHS axis.
            if (DSCH0 != null)
            {
                if (cScopeSetup.isVertCH0SelectLHS == true)
                {
                    cScope.Series[0].YAxisType = AxisType.Primary;
                    rbCH1LHS.Checked = true;
                    rbCH1RHS.Checked = false;
                    this.rbCH1LHS.BackColor = DSCH0.color;
                    this.rbCH1LHS.UseVisualStyleBackColor = false;
                    this.rbCH1RHS.BackColor = System.Drawing.Color.Transparent;
                    this.rbCH1RHS.UseVisualStyleBackColor = false;
                }
                else
                {
                    cScope.Series[0].YAxisType = AxisType.Secondary;
                    rbCH1LHS.Checked = false;
                    rbCH1RHS.Checked = true;
                    this.rbCH1LHS.BackColor = System.Drawing.Color.Transparent;
                    this.rbCH1LHS.UseVisualStyleBackColor = false;
                    this.rbCH1RHS.BackColor = DSCH0.color;
                    this.rbCH1RHS.UseVisualStyleBackColor = false;
                }
            }
            if (DSCH1 != null)
            {
                //-------------------------------------------------------------------------
                if (cScopeSetup.isVertCH1SelectLHS == true)
                {
                    cScope.Series[1].YAxisType = AxisType.Primary;
                    rbCH2LHS.Checked = true;
                    rbCH2RHS.Checked = false;
                    this.rbCH2LHS.BackColor = DSCH1.color;
                    this.rbCH2LHS.UseVisualStyleBackColor = false;
                    this.rbCH2RHS.BackColor = System.Drawing.Color.Transparent;
                    this.rbCH2RHS.UseVisualStyleBackColor = false;
                }
                else
                {
                    cScope.Series[1].YAxisType = AxisType.Secondary;
                    rbCH2LHS.Checked = false;
                    rbCH2RHS.Checked = true;
                    this.rbCH2LHS.BackColor = System.Drawing.Color.Transparent;
                    this.rbCH2LHS.UseVisualStyleBackColor = false;
                    this.rbCH2RHS.BackColor = DSCH1.color;
                    this.rbCH2RHS.UseVisualStyleBackColor = false;
                }
            }
            if (DSCH2 != null)
            {
                if (cScopeSetup.isVertCH2SelectLHS == true)
                {
                    cScope.Series[2].YAxisType = AxisType.Primary;
                    rbCH3LHS.Checked = true;
                    rbCH3RHS.Checked = false;
                    this.rbCH3LHS.BackColor = DSCH2.color;
                    this.rbCH3LHS.UseVisualStyleBackColor = false;
                    this.rbCH3RHS.BackColor = System.Drawing.Color.Transparent;
                    this.rbCH3RHS.UseVisualStyleBackColor = false;
                }
                else
                {
                    cScope.Series[2].YAxisType = AxisType.Secondary;
                    rbCH3LHS.Checked = false;
                    rbCH3RHS.Checked = true;
                    this.rbCH3LHS.BackColor = System.Drawing.Color.Transparent;
                    this.rbCH3LHS.UseVisualStyleBackColor = false;
                    this.rbCH3RHS.BackColor = DSCH2.color;
                    this.rbCH3RHS.UseVisualStyleBackColor = false;
                }
            }
            if (DSCH3 != null)
            {
                if (cScopeSetup.isVertCH3SelectLHS == true)
                {
                    cScope.Series[3].YAxisType = AxisType.Primary;
                    rbCH4LHS.Checked = true;
                    rbCH4RHS.Checked = false;
                    this.rbCH4LHS.BackColor = DSCH3.color;
                    this.rbCH4LHS.UseVisualStyleBackColor = false;
                    this.rbCH4RHS.BackColor = System.Drawing.Color.Transparent;
                    this.rbCH4RHS.UseVisualStyleBackColor = false;
                }
                else
                {
                    cScope.Series[3].YAxisType = AxisType.Secondary;
                    rbCH4LHS.Checked = false;
                    rbCH4RHS.Checked = true;
                    this.rbCH4LHS.BackColor = System.Drawing.Color.Transparent;
                    this.rbCH4LHS.UseVisualStyleBackColor = false;
                    this.rbCH4RHS.BackColor = DSCH3.color;
                    this.rbCH4RHS.UseVisualStyleBackColor = false;
                }
            }
            #endregion

            #region//--------------------------------------------------------- is there channel to use RHS axis, if not remove it. 
            if ((cScopeSetup.isVertCH0SelectLHS == false) | (cScopeSetup.isVertCH1SelectLHS == false) | (cScopeSetup.isVertCH2SelectLHS == false) | (cScopeSetup.isVertCH3SelectLHS == false))
                cScope.ChartAreas["ScopeArea"].AxisY2.Enabled = AxisEnabled.True;
            else
                cScope.ChartAreas["ScopeArea"].AxisY2.Enabled = AxisEnabled.False;
            #endregion

            #region//---------------------------------------------------------Auto and Manual Scale Control.
            cScope.ChartAreas["ScopeArea"].RecalculateAxesScale();
            if (cScopeSetup.isVertAutoScaleRHS == true)
            {
                cScope.ChartAreas["ScopeArea"].AxisY2.Minimum = Double.NaN;
                cScope.ChartAreas["ScopeArea"].AxisY2.Maximum = Double.NaN;
            }
            else
            {
                double dMax = 0;
                double dMin = 0;
                if (double.TryParse(tbRHSMax.Text, out dMax) == true)
                    cScope.ChartAreas["ScopeArea"].AxisY2.Maximum = dMax;
                if (double.TryParse(tbRHSMin.Text, out dMin) == true)
                    cScope.ChartAreas["ScopeArea"].AxisY2.Minimum = dMin;
            }

            if (cScopeSetup.isVertAutoScaleLHS == true)
            {
                cScope.ChartAreas["ScopeArea"].AxisY.Minimum = Double.NaN;
                cScope.ChartAreas["ScopeArea"].AxisY.Maximum = Double.NaN;
            }
            else
            {
                double dMax = 0;
                double dMin = 0;
                if (double.TryParse(tbLHSMax.Text, out dMax) == true)
                    cScope.ChartAreas["ScopeArea"].AxisY.Maximum = dMax;
                if (double.TryParse(tbLHSMin.Text, out dMin) == true)
                    cScope.ChartAreas["ScopeArea"].AxisY.Minimum = dMin;
            }
            #endregion

            #region//---------------------------------------------------------Start from Zero on Y axis
            cScope.ChartAreas["ScopeArea"].AxisY.IsStartedFromZero = cbLHSStartZero.Checked;
            cScope.ChartAreas["ScopeArea"].AxisY2.IsStartedFromZero = cBRHSMajorGrid.Checked;
            #endregion

            #region//---------------------------------------------------------Y axis grids ON/OFF
            cScope.ChartAreas["ScopeArea"].AxisY.MajorGrid.Enabled = cbLHSMajorGrid.Checked;
            cScope.ChartAreas["ScopeArea"].AxisY2.MajorGrid.Enabled = cBRHSMajorGrid.Checked;
            #endregion

            #region//---------------------------------------------------------Y axis grids Major Interval

            if (cScope.ChartAreas["ScopeArea"].AxisY.MajorGrid.Enabled == true)
            {
                double dMajor = 0;
                if (double.TryParse(tbLHSMajor.Text, out dMajor) == true)
                {
                    cScope.ChartAreas["ScopeArea"].AxisY.Interval = dMajor;
                    //cScope.ChartAreas["ScopeArea"].AxisY.MajorGrid.Interval = dMajor;
                    cScope.ChartAreas["ScopeArea"].AxisY.IntervalType = DateTimeIntervalType.Number;
                }
            }
            else
            {
                cScope.ChartAreas["ScopeArea"].AxisY.Interval = 0;
                //cScope.ChartAreas["ScopeArea"].AxisY.MajorGrid.Interval = dMajor;
                cScope.ChartAreas["ScopeArea"].AxisY.IntervalType = DateTimeIntervalType.Auto;
            }
            if (cScope.ChartAreas["ScopeArea"].AxisY2.MajorGrid.Enabled == true)
            {
                double dMajor = 0;
                if (double.TryParse(tbRHSMajor.Text, out dMajor) == true)
                {
                    cScope.ChartAreas["ScopeArea"].AxisY2.Interval = dMajor;
                    //cScope.ChartAreas["ScopeArea"].AxisY2.MajorGrid.Interval = dMajor;
                    cScope.ChartAreas["ScopeArea"].AxisY2.IntervalType = DateTimeIntervalType.Number;
                }
            }
            else
            {
                cScope.ChartAreas["ScopeArea"].AxisY2.Interval = 0;
                //cScope.ChartAreas["ScopeArea"].AxisY2.MajorGrid.Interval = dMajor;
                cScope.ChartAreas["ScopeArea"].AxisY2.IntervalType = DateTimeIntervalType.Auto;
            }
            #endregion

            ScopeDataOp_UpdateGainOffset(0);
            ScopeDataOp_UpdateGainOffset(1);
            ScopeDataOp_UpdateGainOffset(2);
            ScopeDataOp_UpdateGainOffset(3);
            cScope.Update();

        }
        #endregion

        //-------------------------------------------------------------------------------------------Y1 and Y2 Axis Scale. 
        #region //------------------------------------------------------rbCH4RHS_MouseClick
        private void rbCH4RHS_MouseClick(object sender, MouseEventArgs e)
        {
            cScopeSetup.isVertCH3SelectLHS = false;
            VertAxisUpdate();

        }
        #endregion
        #region //------------------------------------------------------rbCH4LHS_MouseClick
        private void rbCH4LHS_MouseClick(object sender, MouseEventArgs e)
        {
            cScopeSetup.isVertCH3SelectLHS = true;
            VertAxisUpdate();
        }
        #endregion
        #region //------------------------------------------------------rbCH3RHS_MouseClick
        private void rbCH3RHS_MouseClick(object sender, MouseEventArgs e)
        {
            cScopeSetup.isVertCH2SelectLHS = false;
            VertAxisUpdate();
        }
        #endregion
        #region //------------------------------------------------------rbCH3LHS_MouseClick
        private void rbCH3LHS_MouseClick(object sender, MouseEventArgs e)
        {
            cScopeSetup.isVertCH2SelectLHS = true;
            VertAxisUpdate();
        }
        #endregion
        #region //------------------------------------------------------rbCH2RHS_MouseClick
        private void rbCH2RHS_MouseClick(object sender, MouseEventArgs e)
        {
            cScopeSetup.isVertCH1SelectLHS = false;
            VertAxisUpdate();
        }
        #endregion
        #region //------------------------------------------------------rbCH2LHS_MouseClick
        private void rbCH2LHS_MouseClick(object sender, MouseEventArgs e)
        {
            cScopeSetup.isVertCH1SelectLHS = true;
            VertAxisUpdate();
        }
        #endregion
        #region //------------------------------------------------------rbCH1RHS_MouseClick
        private void rbCH1RHS_MouseClick(object sender, MouseEventArgs e)
        {
            cScopeSetup.isVertCH0SelectLHS = false;
            VertAxisUpdate();
        }
        #endregion
        #region //------------------------------------------------------rbCH1LHS_MouseClick
        private void rbCH1LHS_MouseClick(object sender, MouseEventArgs e)
        {
            cScopeSetup.isVertCH0SelectLHS = true;
            VertAxisUpdate();
        }
        #endregion

        //-------------------------------------------------------------------------------------------Y Auto Scale
        #region //------------------------------------------------------chLHSAutoScale_MouseClick
        private void chLHSAutoScale_MouseClick(object sender, MouseEventArgs e)
        {
            if (chLHSAutoScale.Checked)
                cScopeSetup.isVertAutoScaleLHS = true;
            else
                cScopeSetup.isVertAutoScaleLHS = false;
            VertAxisUpdate();
        }
        #endregion
        #region //------------------------------------------------------chRHSAutoScale_MouseClick
        private void chRHSAutoScale_MouseClick(object sender, MouseEventArgs e)
        {
            if (chRHSAutoScale.Checked)
                cScopeSetup.isVertAutoScaleRHS = true;
            else
                cScopeSetup.isVertAutoScaleRHS = false;
            VertAxisUpdate();
        }
        #endregion

        //-------------------------------------------------------------------------------------------Y Scale Min/Max
        #region //------------------------------------------------------tbLHSMax_KeyUp
        private void tbLHSMax_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string ss = tbLHSMax.Text;
                if (Tools.IsString_Numberic_Double(ss))
                {
                    VertAxisUpdate();
                }
            }
        }
        #endregion
        #region //------------------------------------------------------tbLHSMin_KeyUp
        private void tbLHSMin_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string ss = tbLHSMin.Text;
                if (Tools.IsString_Numberic_Double(ss))
                {
                    VertAxisUpdate();
                }
            }

        }
        #endregion
        #region //------------------------------------------------------tbRHSMax_KeyUp
        private void tbRHSMax_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string ss = tbRHSMax.Text;
                if (Tools.IsString_Numberic_Double(ss))
                {
                    VertAxisUpdate();
                }
            }

        }
        #endregion
        #region //------------------------------------------------------tbRHSMin_KeyUp
        private void tbRHSMin_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string ss = tbRHSMin.Text;
                if (Tools.IsString_Numberic_Double(ss))
                {
                    VertAxisUpdate();
                }
            }

        }
        #endregion
        //-------------------------------------------------------------------------------------------Y Scale Major Grid ON/OFF
        #region //------------------------------------------------------cbLHSMajorGrid_MouseClick
        private void cbLHSMajorGrid_MouseClick(object sender, MouseEventArgs e)
        {
            VertAxisUpdate();
        }
        #endregion
        #region //------------------------------------------------------cBRHSMajorGrid_MouseClick
        private void cBRHSMajorGrid_MouseClick(object sender, MouseEventArgs e)
        {
            VertAxisUpdate();
        }
        #endregion
        //-------------------------------------------------------------------------------------------Y Scale: Zero
        #region //------------------------------------------------------cbLHSStartZero_MouseClick
        private void cbLHSStartZero_MouseClick(object sender, MouseEventArgs e)
        {
            VertAxisUpdate();
        }
        #endregion
        #region //------------------------------------------------------cBRHSStartZero_MouseClick
        private void cBRHSStartZero_MouseClick(object sender, MouseEventArgs e)
        {
            VertAxisUpdate();
        }
        #endregion
        //-------------------------------------------------------------------------------------------Y Scale: Major Grid
        #region //------------------------------------------------------tbLHSMajor_KeyUp
        private void tbLHSMajor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string ss = tbLHSMajor.Text;
                if (Tools.IsString_Numberic_Double(ss))
                {
                    cbLHSMajorGrid.Checked = true;
                    VertAxisUpdate();
                }
            }
        }
        #endregion

        

        #region //------------------------------------------------------tbRHSMajor_KeyUp
        private void tbRHSMajor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string ss = tbRHSMajor.Text;
                if (Tools.IsString_Numberic_Double(ss))
                {
                    cBRHSMajorGrid.Checked = true;
                    VertAxisUpdate();
                }
            }
        }
        #endregion



        #region //------------------------------------------------------Experiment_changeYScala
        // Experiment this code for floor and ceiling, it easy to use, just changeYScala(chart1);
        // http://stackoverflow.com/questions/23981710/scale-y-axis-of-a-chart-depending-on-the-values-within-a-section-of-x-values-for
        private void Experiment_changeYScala(object chart)
        {
            double max = Double.MinValue;
            double min = Double.MaxValue;

            Chart tmpChart = (Chart)chart;

            double leftLimit = tmpChart.ChartAreas[0].AxisX.Minimum;
            double rightLimit = tmpChart.ChartAreas[0].AxisX.Maximum;

            for (int s = 0; s < tmpChart.Series.Count(); s++)
            {
                foreach (DataPoint dp in tmpChart.Series[s].Points)
                {
                    if (dp.XValue >= leftLimit && dp.XValue <= rightLimit)
                    {
                        min = Math.Min(min, dp.YValues[0]);
                        max = Math.Max(max, dp.YValues[0]);
                    }
                }
            }
            tmpChart.ChartAreas[0].AxisY.Maximum = (Math.Ceiling((max / 10)) * 10);
            tmpChart.ChartAreas[0].AxisY.Minimum = (Math.Floor((min / 10)) * 10);
        }
        #endregion

        //#######################################################################################################
        //######################################################################################Test Waveform Gen
        //#######################################################################################################

        #region --------------------------------------------------------Class TestWaveFormGen
        public class TestWaveFormGen
        {
            #region //================================================================CalcSinusFunction_Riscy
            public void CalcSinusFunction_Riscy(DataSource src, double Freq, double ampFactor)
            {
                int sampleRate = src.Length;
                double amplitude = ampFactor * UInt16.MaxValue;
                double frequency = Freq;

                for (int i = 0; i < src.Length; i++)
                {
                    src.cSamples[i] = new cPoint(0, 0)
                    {
                        x = i,
                        y = (float)(amplitude * Math.Sin((2 * Math.PI * (float)i * frequency) / sampleRate))
                    };

                }
            }
            #endregion

            #region //================================================================CalcSinusFunction_0

            public void CalcSinusFunction_0(DataSource src, int idx)
            {
                for (int i = 0; i < src.Length; i++)
                {
                    var obj = src.cSamples[i];
                    obj.x = i;
                    obj.y = (float)(((float)200 * Math.Sin((idx + 1) * (i + 1.0) * 48 / src.Length)));
                }
            }
            #endregion

            #region //================================================================CalcSinusFunction_1
            public void CalcSinusFunction_1(DataSource src, int idx)
            {
                for (int i = 0; i < src.Length; i++)
                {
                    var obj = src.cSamples[i];
                    obj.x = i;
                    obj.y = (float)(((float)20 *
                                                Math.Sin(20 * (idx + 1) * (i + 1) * Math.PI / src.Length)) *
                                                Math.Sin(40 * (idx + 1) * (i + 1) * Math.PI / src.Length)) +
                                                (float)(((float)200 *
                                                Math.Sin(200 * (idx + 1) * (i + 1) * Math.PI / src.Length)));
                }
            }
            #endregion

            #region //================================================================CalcSinusFunction_2
            private void CalcSinusFunction_2(DataSource src, int idx)
            {
                for (int i = 0; i < src.Length; i++)
                {
                    var obj = src.cSamples[i];
                    obj.x = i;
                    obj.y = (float)(((float)20 *
                                                Math.Sin(40 * (idx + 1) * (i + 1) * Math.PI / src.Length)) *
                                                Math.Sin(160 * (idx + 1) * (i + 1) * Math.PI / src.Length)) +
                                                (float)(((float)200 *
                                                Math.Sin(4 * (idx + 1) * (i + 1) * Math.PI / src.Length)));
                }
            }
            #endregion

            #region //================================================================CalcSinusFunction_3
            private void CalcSinusFunction_3(DataSource src, int idx, float time)
            {
                for (int i = 0; i < src.Length; i++)
                {
                    var obj = src.cSamples[i];
                    obj.x = i;
                    obj.y = 100 + (float)((200 * Math.Sin((idx + 1) * (time + i * 100) / 8000.0))) +
                                    +(float)((40 * Math.Sin((idx + 1) * (time + i * 200) / 2000.0)));
                    /**
                                (float)( 4* Math.Sin( ((time + (i+8) * 100) / 900.0)))+
                                (float)(28 * Math.Sin(((time + (i + 8) * 100) / 290.0))); */
                }

            }
            #endregion

        }
        
    }
    #endregion

    //#######################################################################################################
    //########################################################################################## DataSource
    //#######################################################################################################

    #region --------------------------------------------------------Class stcPoint (suitable for rolling and constant update), List<> can be used and faster
    //private stcPoint[] samples;                   // Data Buffer
    // public stcPoint[] Samples { get { return samples; } set { samples = value; length = samples.Length; } }  //samples
    [Serializable]                          // Need to include that for xmlserialize to work correctly. 
    public class cPoint
    {
        public float x { get; set; }
        public float y { get; set; }
        public cPoint() { }
        public cPoint(float xx, float yy)
        {
            x = xx;
            y = yy;
        }
    }
    #endregion

    #region --------------------------------------------------------Class DataSource (Updated for class type rather than struct type)
    [Serializable]
    [System.Xml.Serialization.XmlInclude(typeof(cPoint))]
    [System.Xml.Serialization.XmlInclude(typeof(Color))]        // This will not save color data to xml file. 
    public class DataSource
    {
        private List<cPoint> csamples;
        //---------------------------------------------------------Getter/Setter
        public string Name { get; set; }
        public int Length { get; set; }
        public string colname { get; set; }
        public Color color { get; set; }                // Color of the line.
        public bool AutoScaleY { get; set; }
        public bool AutoScaleX { get; set; }
        public double YDstart { get; set; }
        public double YDend { get; set; }
        public double Yoffset { get; set; }
        public double Ygain { get; set; }
        public Int32 StartRow { get; set; }
        public Int32 EndRow { get; set; }
        public Int32 iRollPointer { get; set; }
        public Int32 iRoll_StartX { get; set; }
        public Int32 iRoll_EndX { get; set; }
        public List<cPoint> cSamples { get { return csamples; } set { csamples = value; } }

        #region //================================================================GetXMin
        [XmlIgnore]
        public float GetXMin
        {
            get
            {
                float x_min = float.MaxValue;
                if (cSamples.Count > 0)
                {
                    foreach (cPoint p in cSamples)
                    {
                        if (p.x < x_min) x_min = p.x;
                    }
                }
                return x_min;
            }
        }
        #endregion

        #region //================================================================GetXMax
        [XmlIgnore]
        public float GetXMax
        {
            get
            {
                float x_max = float.MinValue;
                if (cSamples.Count > 0)
                {
                    foreach (cPoint p in cSamples)
                    {
                        if (p.x > x_max) x_max = p.x;
                    }
                }
                return x_max;
            }
        }
        #endregion

        #region //================================================================GetYMin
        [XmlIgnore]
        public float GetYMin
        {
            get
            {
                float y_min = float.MaxValue;
                if (cSamples.Count > 0)
                {
                    foreach (cPoint p in cSamples)
                    {
                        if (p.y < y_min) y_min = p.y;
                    }
                }
                return y_min;
            }
        }
        #endregion

        #region //================================================================GetYMax
        [XmlIgnore]
        public float GetYMax
        {
            get
            {
                float y_max = float.MinValue;
                if (cSamples.Count > 0)
                {
                    foreach (cPoint p in cSamples)
                    {
                        if (p.y > y_max) y_max = p.y;
                    }
                }
                return y_max;
            }
        }
        #endregion

        //------------------------------------------------------------------------------
        // =====================================================constructor
        public DataSource() { }

        public DataSource(ref List<cPoint> refdata, string sName, string sColName, Color cLine)
        {
            if (refdata == null)
            {
                cSamples = null;
                cSamples = new List<cPoint>();
                Length = 0;
            }
            else
            {
                cSamples = refdata;
                Length = refdata.Count;
            }
            //-------------------------Setup RollData
            iRollPointer = 0;       // Start Pointer
            //-------------------------Default Name/Color
            Name = sName;
            colname = sColName;
            color = cLine;
        }

        #region //================================================================AppendDataSource().
        public void AppendDataSource(cPoint fdata, int iMaxRollPointer)
        {
            //------------------------------------Chart Mode
            csamples.Add(fdata);
            Length = csamples.Count;
            //------------------------------------Rolling Mode
            if (iRollPointer == iMaxRollPointer)        // End of the screen where rolling is now in action. 
            {
                iRoll_EndX++;
                iRoll_StartX++;
            }
            else
            {
                iRollPointer++;
                iRoll_EndX = iRollPointer;
            }
            
        }
        #endregion

        #region //================================================================CreateList<T> Helper function for array init. 
        private static List<T> CreateList<T>(int capacity)
        {
            return Enumerable.Repeat(default(T), capacity).ToList();
        }
        #endregion

        #region //================================================================SetDisplayRangeY
        public void SetDisplayRangeY(float y_start, float y_end)
        {
            YDstart = y_start;
            YDend = y_end;
        }
        #endregion

        #region //================================================================Name (Old Code, not needed)
        /*
        [Category("Properties")] // Take this out, and you will soon have problems with serialization;
        [DefaultValue(typeof(string), "")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        */
        #endregion

        #region //================================================================Length (Old Code, not needed)
        /*
        [Category("Properties")] // Take this out, and you will soon have problems with serialization;
        [DefaultValue(typeof(int), "0")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Length
        {
            get { return length; }
            set
            {
                length = value;
                if (length != 0)
                {
                    if (csamples == null)
                        csamples = new List<cPoint>(length);

                    csamples.Clear();
                    csamples = new List<cPoint>(length);
                }
                else  // length is 0
                {
                    csamples.Clear();
                }
            }
        }
        */
        #endregion

    }
    #endregion

    //#######################################################################################################
    //########################################################################################## DataSourceOp (for Gain/Offset Adjustment)
    //#######################################################################################################

    #region --------------------------------------------------------Class ScopeData_Operation
    [Serializable]
    public class ScopeDataOp
    {
        public string DataName { get; set; }         // String Name 
        public double DataOffset { get; set; }       // 0 = No Offset, +/- data = offset
        public double DataGain { get; set; }         // 1 = 1V/V
        public bool DataCheckBox { get; set; }      // Resultant data
        // =====================================================Constructor
        public ScopeDataOp()
        {
            DataName = "";
            //----------------------ADC12 and ADC16 setting and results
            DataGain = 1.0D;         // 1V/V
            DataOffset = 0.0D;       // No Offset
            DataCheckBox = false;
        }
    }
    #endregion

    #region --------------------------------------------------------Class ScopeSetup (Configuration)
    [Serializable]
    public class ScopeSetup
    {
        public bool isSizeLarge         { get; set; }       // True = extend size, False = compact size
        public bool isRollingMode       { get; set; }       // True = Rolling mode, false = whole data and rely on zoom
        public bool isTimeBaseTick       { get; set; }      // True = use tick(mSec), False = Fixed Point on X (rolling mode)
        public Int32 iMaxRollPointer    { get; set; }       // X Axis Roll number range under rolling or logger mode. 
        //------------------------------------------------Vertical axis option list. 
        public bool isVertCH0SelectLHS   { get; set; }       // CH0 for vertical LHS (true), false = RHS.    
        public bool isVertCH1SelectLHS   { get; set; }       // CH1 for vertical LHS (true), false = RHS. 
        public bool isVertCH2SelectLHS   { get; set; }       // CH2 for vertical LHS (true), false = RHS. 
        public bool isVertCH3SelectLHS   { get; set; }       // CH3 for vertical LHS (true), false = RHS.  
        
        public bool isVertAutoScaleLHS   { get; set; }       // True enable auto scale, false = manual scale.
        public bool isVertAutoScaleRHS   { get; set; }       // True enable auto scale, false = manual scale.

        public double VertLHSMin        { get; set; }        // Vert Min LHS
        public double VertLHSMax        { get; set; }        // Vert Max LHS

        public double VertRHSMin        { get; set; }        // Vert Min RHS
        public double VertRHSMax        { get; set; }        // Vert Max RHS

        public double VertLHSMajorAxis  { get; set; }        // Major step axis LHS
        public double VertRHSMajorAxis  { get; set; }        // Major step axis RHS

        // =====================================================Constructor
        public ScopeSetup()
        {
            isSizeLarge = false;    // Compact size
            isRollingMode = true;   // Rolling charting mode. 
            isTimeBaseTick = false; // Must be false for rolling mode. 
            iMaxRollPointer = 400;           //20 or 1300
            isVertCH0SelectLHS = true;
            isVertCH1SelectLHS = true;
            isVertCH2SelectLHS = true;
            isVertCH3SelectLHS = true;
            isVertAutoScaleLHS = true;
            isVertAutoScaleRHS = true;
            VertLHSMin = -500;
            VertLHSMax = 500;
            VertRHSMin = -500;
            VertRHSMax = 500;
            VertLHSMajorAxis = 100;
            VertRHSMajorAxis = 100;
        }
    }
    #endregion

}
#region//======================================================DoubleBuffered property via reflection for dgvType, improve flickerless image.
public static class ExtensionMethodsRiscyScope
    {
        public static void DoubleBufferedRiscyScope(this Chart dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
#endregion


