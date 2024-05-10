using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.Reflection;

// Useful resource
// http://blogs.msdn.com/b/alexgor/
// 

namespace UDT_Term_FFT
{
    public partial class FFTWindow : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        FFT_DataEditor myFFT_DataEditor;
        
        //====================================================================Frame Information
        //1st data      "$F" header.
        //2nd data	    16 bit unsigned integer		Number of data within the frame
        //3rd data		16 bit unsigned integer 	Sample rate, ie 1000 = 1Ksps
        //4th data		16 bit unsigned integer		Y-Scale translation.
        //5th data		16 bit unsigned integer		FFTStatus flag-bits, refer to dsPIC33 code for more 							information. It include error flags
        //6th data to 6+1024 = 1030th data, max 1024 element (NB: mirror 1025-2048 not transferred)
        // ==================================================================Private Variable

        private UInt32 m_iFFTElementCount;
        private UInt32 m_iFFTSampleRate;
        private UInt32 m_iFFTYScale;
        private UInt32 m_iFFTStatusWord;
        private double m_dFFTIntervel_Hz;
        //-------------------------------------Chart variables.
        private bool m_YAxisScaleInVolt;
        private bool m_YAxisScaleInLog;
        private UInt32 m_YAxisScaleMode;
        private bool m_YAxisSQRTY;
        private UInt32 m_XAxis_intervelStep;
        private bool m_YAxisisYZoomMode;
        private UInt32 m_Yaxis_PeakValue;       // seek maximum peak value for YScale purpose.
        //--------------------------------------Averager Variables
        private bool m_TestDataCeaseLoop;       // Part of test data for evaluating averager
        private UInt32 m_TestDataBackgroundCounter;
        private bool m_FFTAvergerIsEnable;      // if true enable averager operation.
        private UInt32 m_FFTAvergerLoop;        // Number of loop, max 32
        private UInt32 m_FFTAvergerAddress;     // Array Pointer (based on FIFO data)
        private UInt32 m_FFTAvergerFill;        // Fill number for average calculation process until equal to m_FFTAvergerLoop, use for start process. 
        //--------------------------------------Misc
        private bool m_SuspendMode;             // When 1, cease data collection 
        private ulong[] Step;
        private uint StepPointer;               // Pointer to Step array
        private bool m_isExtendDisplayEnabled;  // true extend display (wider), suitable for 1920 x 1080 screen.  
        private bool m_isCursorFreqAdvancedMode;// true include cursor position data on Freq.
        IntPtr rtbOEMfft;
        // ==================================================================Getter/Setter

        public UInt32 iFFTElementCount { get { return m_iFFTElementCount; } }
        public UInt32 iFFTSampleRate { get { return m_iFFTSampleRate; } }
        public UInt32 iFFTYScale { get { return m_iFFTYScale; } }
        public UInt32 iFFTStatusWord { get { return m_iFFTStatusWord; } }
        public double dFFTIntervel_Hz { get { return m_dFFTIntervel_Hz; } }

        //===================================================================Data Array

        private double[] m_dFFTDisplayData;
        private double[,] m_dFFTAvgData;
        //==================================================================Cursor
        PointF CursorPointer = new PointF(2, 2);

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
                case (50):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPageTVBBlank_1B;
                        break;
                    }
                case (100):
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


        #region//================================================================Constructor
        public FFTWindow()
        {
            InitializeComponent();
            //----------------------------------------Default Config
            m_dFFTDisplayData = new double[1024];
            m_dFFTAvgData = new double[32, 1024];           // up to x32 average is supported.
            FFTAvgDataClear();
            rtbOEMfft = new IntPtr();
            //-----------------Step Init
            Step = new ulong[] 
                                    {1,2,5,
                                    10,20,50,
                                    100,200,500,
                                    1000,2000,5000,
                                    10000,20000,50000,
                                    100000,200000,500000,
                                    1000000,2000000,5000000,
                                    10000000,20000000,50000000,
                                    100000000,200000000,500000000,
                                    1000000000,2000000000,5000000000
                                    };
            StepPointer = 15;        // Start
            //-----------------test chart
            seriesCh1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "FFTData",
                Color = System.Drawing.Color.YellowGreen,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Line,
                //ChartType = SeriesChartType.FastLine,
            };

            this.FFTGraph.Series.Clear();
            this.FFTGraph.Series.Add(seriesCh1);
            //---------------------------------------test chart
            for (int i = 0; i < 128; i++)
            {
                seriesCh1.Points.AddXY(i, 0);
                m_dFFTDisplayData[i] = (double) i;
            }
            m_iFFTElementCount = 128;
            m_XAxis_intervelStep = 5;
            //---------------------------------------Set Y Axis to Linear Mode
            btnLogLinYAxis.Text = "Y-Log";
            m_YAxisScaleInLog = false;
            tsmYLogSelect.Checked = false;
            tsmYLinearSelect.Checked = true;
            //--------------------------------------Update Chart
            m_YAxisScaleInVolt = false;
            m_YAxisisYZoomMode = false;
            m_YAxisSQRTY = false;
            this.cBox_SQRTY.Checked = false;
            this.cBox_Marker.Checked = false;
            UpdateFFTChart(true);
            //--------------------------------------Other
            this.TopMost = false;
            m_TestDataCeaseLoop = false;
            m_FFTAvergerIsEnable = false;
            rtbFFTConsole.AppendText("Welcome to ADT's FFT Window\n");
            m_isExtendDisplayEnabled = false;
            m_SuspendMode = false;                      // Continue Mode.
            m_isCursorFreqAdvancedMode = false;
            //--------------------------------------Minor grid
            FFTGraph.ChartAreas[0].AxisY.MinorGrid.Interval = 1;
            FFTGraph.ChartAreas[0].AxisY.MinorGrid.LineColor = System.Drawing.Color.DarkGreen;   //.Green;
            FFTGraph.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //-------------------------------------FFTGraph use Doublebuffer for smoother chart operation, reduce flickring.
            FFTGraph.DoubleBufferedFFT(true);
            //-------------------------------------Y Scale Mode
            UpdateYScaleBox(0);
            tbYScaleMetaData.Text = "1";
            tbYScaleUser.Text = "1";

            //-----------------------------------Context Menu for Copy and Paste
            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem menuItem = new System.Windows.Forms.MenuItem("Copy");
            menuItem.Click += new EventHandler(CopyAction);
            contextMenu.MenuItems.Add(menuItem);
            //menuItem = new System.Windows.Forms.MenuItem("Paste");
            //menuItem.Click += new EventHandler(PasteAction);
            //contextMenu.MenuItems.Add(menuItem);
            rtbFFTConsole.ContextMenu = contextMenu;
        }
        #endregion

        #region//================================================================rtbFFTConsole_KeyDown (CNTR-V and C)
        private void rtbFFTConsole_KeyDown(object sender, KeyEventArgs e)
        {
            /* No Paste allowed.
            if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                if (Clipboard.ContainsText(TextDataFormat.Rtf))
                {
                    rtbFFTConsole.SelectedRtf
                        = Clipboard.GetData(DataFormats.Rtf).ToString();
                    e.SuppressKeyPress = true;
                }
            }
            */
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                if (rtbFFTConsole.SelectedText != "")
                    Clipboard.SetText(rtbFFTConsole.SelectedText);
                e.SuppressKeyPress = true;
            }
            base.OnKeyDown(e);
        }
        #endregion

        #region//================================================================rtbTerm_MouseUp
        void CopyAction(object sender, EventArgs e)
        {
            if (rtbFFTConsole.SelectedText != "")
                Clipboard.SetText(rtbFFTConsole.SelectedText);
        }

        void PasteAction(object sender, EventArgs e)
        {
            /*
            if (Clipboard.ContainsText(TextDataFormat.Rtf))
            {
                rtbFFTConsole.SelectedRtf
                    = Clipboard.GetData(DataFormats.Rtf).ToString();
            }
            */
        }
        #endregion

        #region//================================================================UpdateFFTChart
        private void UpdateFFTChart(bool Reset)
        {
            try
            {
                if (Reset == true)
                {
                    //==========================================================Y Axis Linear (fixed scale for 2.5V for now). 
                    if (m_YAxisScaleInLog == false)
                    {
                        #region // -------------------Linear code for chart
                        //-------------------------------------------------------------Added March 2015,
                        FFTGraph.ChartAreas[0].AxisY.IsLogarithmic = false;
                        if (m_YAxisScaleInVolt == true)
                        {//---------------------Voltage Data

                        }
                        else
                        {//---------------------Raw Data
                            if (tsmAutoScale.Checked == true)
                            {//-------------------------------Auto Y-Scale
                                StepPointer = 6;                    // Start 500 as minimum Y scale 
                                ulong data = (ulong)m_Yaxis_PeakValue;  // Has to be 64 bit format for upper value. 
                                while (StepPointer < Step.Length)
                                {
                                    if (Step[StepPointer] > data)
                                        break;
                                    StepPointer++;
                                }
                                data = Step[StepPointer];
                                FFTGraph.ChartAreas[0].AxisY.Maximum = data;
                                FFTGraph.ChartAreas[0].AxisY.Interval = data / 20;
                            }
                        }
                        FFTGraph.ChartAreas[0].AxisY.Minimum = 0;                                   
                        FFTGraph.ChartAreas[0].AxisY.IsStartedFromZero = true;
                        FFTGraph.ChartAreas[0].AxisY.Crossing = 0;
                        FFTGraph.ChartAreas[0].AxisY.MinorGrid.Enabled = false;  // no need for minor grid.
                        #endregion
                    }
                    else
                    //==========================================================Y Axis Log
                    {
                        #region // -------------------Log code for chart
                        double data;
                        FFTGraph.ChartAreas[0].AxisY.IsLogarithmic = true;
                        seriesCh1.Points.Clear();
                        for (int i = 0; i < m_iFFTElementCount; i++)    // This is required to convert zero data into fractional to avoid crashing the log(0) event. 
                        {
                            data = m_dFFTDisplayData[i];
                            if (data == 0) data = 0.1;
                            if (m_YAxisSQRTY==true)
                            {
                                data = Math.Sqrt(data);
                            }
                            seriesCh1.Points.AddXY((i * m_dFFTIntervel_Hz), data);
                        }
                        //-------------------------------------------------------------Added March 2015,
                        if (m_YAxisScaleInVolt == true)
                        {
                            //FFTGraph.ChartAreas[0].AxisY.Maximum = 65536; // 10V
                        }
                        else
                        {
                            if (tsmAutoScale.Checked == true)
                            {//-------------------------------Auto Scale
                                StepPointer = 6;                    // Start 500 as minimum Y scale 
                                ulong dataa = (ulong)m_Yaxis_PeakValue;  // Has to be 64 bit format for upper value. 
                                while (StepPointer < Step.Length)
                                {
                                    if (Step[StepPointer] > dataa)
                                        break;
                                    StepPointer++;
                                }
                                dataa = Step[StepPointer];
                                FFTGraph.ChartAreas[0].AxisY.Maximum = dataa;
                            }
                        }
                        FFTGraph.ChartAreas[0].AxisY.LogarithmBase = 10;
                        FFTGraph.ChartAreas[0].AxisY.Minimum = 1;       // double.NaN;
                        FFTGraph.ChartAreas[0].AxisY.Crossing = 0;      // double.NaN; Auto
                        FFTGraph.ChartAreas[0].AxisY.Interval = 1;
                        FFTGraph.ChartAreas[0].AxisY.IsStartedFromZero = false;
                        FFTGraph.ChartAreas[0].AxisY.MinorGrid.Enabled = true;


                        //FFTGraph.ChartAreas[0].AxisY.ScaleBreakStyle
                        //Axis.ScaleBreakStyle

                        #endregion
                    }

                    //---------X Axis
                    FFTGraph.ChartAreas[0].AxisX.Minimum = 0;
                    FFTGraph.ChartAreas[0].AxisX.IsStartedFromZero = true;
                    FFTGraph.ChartAreas[0].AxisX.Interval = m_XAxis_intervelStep;//10;         500Hz/5 =100 X-Axis point (if fits). Cannot make it 20, 40, 60 HZ etc. Does not like fractional number.
                    FFTGraph.ChartAreas[0].AxisX.Maximum = (double)m_iFFTElementCount;
                    FFTGraph.ChartAreas[0].AxisX.LabelStyle.Format = "{0:F1}";  // make easier to read x-axis.Use cursor for higher resolution. 
                }
                FFTGraph.Series.ResumeUpdates();            // 30/4/15...Faster?
                FFTGraph.Series.Invalidate();               // Ref http://stackoverflow.com/questions/26642980/c-sharp-ms-chart-slow-on-large-amount-of-real-time-data
                FFTGraph.Series.SuspendUpdates();
                //FFTGraph.Invalidate();
            }
            catch (System.Exception ex)
            {
                rtbFFTConsoleAppend("#ERR: Exception Event-A UpdateFFTChart() Exception:" + ex.ToString());
            }
        }
        #endregion 

        #region//================================================================FFTRecievedData

        public void FFTRecievedData(string FFTFrame)
        {
            m_XAxis_intervelStep = 5;       // Default setting.
            int i = 0;
            UInt32 datatt;
            string[] sFFTToken;
            UInt32 count;
            if (m_SuspendMode == true)                  // Skip Chart Update to study.
                return;
            //-------------------------------Strip away unwanted text before $F and after $E

            //-------------------------------
            try
            {
                sFFTToken = FFTFrame.Split(',');
                count = (UInt32) sFFTToken.Length;
                uint data;
                int offset = 5;
                string endtest = "$E";
                int badcount = 0;             
                
                #region //------------------------------Validate meta/header data
                //1st data      "$F" header.
                //2nd data	    16 bit unsigned integer		Number of data within the frame
                //3rd data		16 bit unsigned integer 	Sample rate, ie 1000 = 1Ksps
                //4th data		16 bit unsigned integer		Y-Scale translation.
                //5th data		16 bit unsigned integer		FFTStatus flag-bits, refer to dsPIC33 code for more 							information. It include error flags
                //6th data to 6+1024 = 1030th data, max 1024 element (NB: mirror 1025-2048 not transferred).
                if (count > 1030)
                {
                    rtbFFTConsoleAppend("###Error: Too many Elements: "+count.ToString()+" (<1024 max). Frame Disposed.\n");
                    return;
                }
                //------2nd of the frame: m_iFFTElementCount
                if (!UInt32.TryParse(sFFTToken[1], out m_iFFTElementCount))
                {
                    rtbFFTConsoleAppend("###Error: iFFTElementCount, the string is not numeric\n");
                    return;
                }
                if (m_iFFTElementCount > 1024)
                {
                    rtbFFTConsoleAppend("###Error: iFFTElementCount, too many elements (<1024), check your embedded code\n");
                    return;
                }
                //------3rd of the frame: m_iFFTSampleRate
                if (!UInt32.TryParse(sFFTToken[2], out m_iFFTSampleRate))
                {
                    rtbFFTConsoleAppend("###Error: m_iFFTSampleRate, the string is not numeric\n");
                    return;
                }
                //------4th of the frame: m_iFFTYScale
                if (!UInt32.TryParse(sFFTToken[3], out m_iFFTYScale))
                {
                    rtbFFTConsoleAppend("###Error: m_iFFTYScale, the string is not numeric\n");
                    return;
                }
                //------5th of the frame: m_iFFTStatusWord
                if (!UInt32.TryParse(sFFTToken[4], out m_iFFTStatusWord))
                {
                    rtbFFTConsoleAppend("###Error: m_iFFTYStatusWord, the string is not numeric\n");
                    return;
                }
                else
                {
                    //string binaryText;
                    //binaryText = Convert.ToString(m_iFFTStatusWord, 2);
                   // binaryText = Int32.Parse(Convert.ToString(m_iFFTStatusWord, 2)).ToString("0000 0000 0000 0000");
                    //rtbFFTConsoleAppend("-INFO: FFT Status Flag: 0b" + binaryText);
                }
                //-------The rest are data.
                #endregion
                seriesCh1.Points.Clear();
                //--------------------------------------Frequency Resolution Bin

                m_dFFTIntervel_Hz = (double)m_iFFTSampleRate / (double)(m_iFFTElementCount * 2);

                //--------------------------------------Convert FFT string element into UINT32 (actually UINT16) data (with error trapping)
                m_Yaxis_PeakValue=16384;
                while (i < m_iFFTElementCount)
                {
                    if (sFFTToken[i + offset].Contains(endtest)) break;      // end of data termination
                    if (!UInt32.TryParse(sFFTToken[i + offset], out data))
                    {
                        m_dFFTDisplayData[i] = 0;
                        rtbFFTConsoleAppend("#ERR : Bad data, check your embedded code and data frame: Element No:"+i.ToString());
                        badcount++;
                        if (badcount == 10)
                        {
                            rtbFFTConsoleAppend("#ERR : Too many bad data (>10), Quit FFT Frame decodes");
                            break;
                        }
                    }
                    else
                    {
                        m_dFFTDisplayData[i] = (double)data;                      // save data into array for display and log process.
                        if (data > m_Yaxis_PeakValue)                             // Seek peak value for Y Scale purpose. 
                            m_Yaxis_PeakValue = data;
                    }
                    i++;
                }
                if (myGlobalBase.FFTOpertaionMode == true)
                {
                    myFFT_DataEditor.FFT_UpdateData(FFTFrame, m_iFFTElementCount, m_iFFTSampleRate, m_iFFTYScale, m_iFFTStatusWord);
                }
            }
            catch (System.Exception ex)
            {
                rtbFFTConsoleAppend("#ERR: Exception Event-A FFTRecievedData() Exception:" + ex.ToString());
            }
            rtbFFTConsoleAppend("-INFO :(" + DateTime.Now.ToString("hh_mm_ss")
                + ") Elements:" + i.ToString()
                + " | Rate:" + m_iFFTSampleRate.ToString()
                + " | YScale:" + m_iFFTYScale.ToString()
                + " | XIntervel:" + m_dFFTIntervel_Hz.ToString()+"Hz");
            //---------------------------------------------------------------------Y Scale User Mode. 
            if (m_YAxisScaleMode==1)
            {
                if (!UInt32.TryParse(tbYScaleUser.Text, out datatt))
                {
                    rtbFFTConsoleAppend("###Error: Incorrect Entry, must be numeric whole number (no decimal)");
                    tbYScaleUser.Text = "1";
                    datatt = 1;
                }
                if (datatt == 0)
                {
                    rtbFFTConsoleAppend("###Error: Incorrect Entry, must be non zero from 1");
                    tbYScaleUser.Text = "1";
                    datatt = 1;
                }
                m_iFFTYScale = datatt;
            }

            //----------------------------------------------------------------------Y Scale, MetaData Mode.
            tbYScaleMetaData.Text = m_iFFTYScale.ToString();
            try
            {
                //-------------------------------------Update Display
                if (m_FFTAvergerIsEnable == false)
                {
                    FFTLinearProcess();     // real time single shot display
                }
                else
                {
                    FFTAvgProcess();        // Averager display
                }
                //--------------------------------------Update chart
                switch (m_iFFTElementCount)         //------------Optimize the X-Axis label spacing (to avoid small text font if too many). 
                {
                    case 128:
                        m_XAxis_intervelStep = 5;
                        break;
                    case 256:
                        m_XAxis_intervelStep = 10;
                        break;
                    case 512:
                        m_XAxis_intervelStep = 20;
                        break;
                    case 1024:
                        m_XAxis_intervelStep = 50;
                        break;
                    default:
                        m_XAxis_intervelStep = 5;
                        break;
                }
                UpdateFFTChart(true);
                //---------------------------Update cursor.
                FFTGraoh_UpdateCursor();
            }
            catch (System.Exception ex)
            {
                rtbFFTConsoleAppend("#ERR: Exception Event-B: FFTRecievedData(): ex:"+ex.Message);
            }
        }
        #endregion

        #region //================================================================rtbConsoleAppend
        
        private void rtbFFTConsoleAppend(string message)
        {
            try
            {
                Tools.rtb_StopRepaint(rtbFFTConsole, rtbOEMfft);
                if (rtbFFTConsole.TextLength >= 10000)
                {
                    rtbFFTConsole.Text = String.Empty;
                    //rtbFFTConsole.Refresh();
                }
                rtbFFTConsole.SelectionStart = rtbFFTConsole.TextLength;
                rtbFFTConsole.ScrollToCaret();
                rtbFFTConsole.Select();
                rtbFFTConsole.AppendText(message + Environment.NewLine);
                //rtbFFTConsole.Refresh();
                Tools.rtb_StartRepaint(rtbFFTConsole, rtbOEMfft);
            }
            catch (AmbiguousMatchException)
            {
                rtbFFTConsole.Text = String.Empty;
                rtbFFTConsole.Refresh();           	
            }

        }
        #endregion

        #region //================================================================FFTGraph_GetToolTipText
        private void FFTGraph_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            try
            {
                // Check selected chart element and set tooltip text
                if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
                {
                    int i = e.HitTestResult.PointIndex;
                    DataPoint dp = e.HitTestResult.Series.Points[i];
                    if (m_YAxisScaleInVolt == true)
                    {
                        e.Text = string.Format("{0:F1} || {1:F0}mV", (dp.XValue), (dp.YValues[0] / m_iFFTYScale) * 1000);
                    }
                    else
                    {
                        e.Text = string.Format("{0:F1} || {1:F1}", (dp.XValue), dp.YValues[0]);
                    }
                    //tbxAxisXFreq.Text = string.Format("{0:F1}",(dp.XValue));
                    //tbxAxisYAmp.Text = string.Format("{0:F1}", (dp.YValues[0]));
                }
            }
            catch (AmbiguousMatchException)
            {
                rtbFFTConsoleAppend("#ERR: Exception Event FFTGraph_GetToolTipText()");
            }
        }
        #endregion

        #region //================================================================FFTGraph_CursorPositionChanged
        private void FFTGraph_CursorPositionChanged(object sender, CursorEventArgs e)
        {
            FFTGraph.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
        }
        #endregion

        #region //================================================================btnReset_Click
        private void btnReset_Click(object sender, EventArgs e)
        {
            //--------------------------------------------------Uncheck the Y-ZOOM and disable.
            cBxYZOOM.Checked = false;
            m_YAxisisYZoomMode = false;
            cBxYZOOM.BackColor = System.Drawing.SystemColors.Control;
            cBxYZOOM.FlatStyle = FlatStyle.Standard;
            //---------------------------------------------------Reset Zoom
            FFTGraph.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
            FFTGraph.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
            //----------------------------------------------------Clear Zoom features. 
            FFTGraph.ChartAreas[0].CursorX.IsUserEnabled = true;
            FFTGraph.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            FFTGraph.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            FFTGraph.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = false;
            FFTGraph.ChartAreas[0].CursorY.IsUserEnabled = false;
            FFTGraph.ChartAreas[0].CursorY.IsUserSelectionEnabled = false;
            FFTGraph.ChartAreas[0].AxisY.ScaleView.Zoomable = false;
            FFTGraph.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = false;

            UpdateFFTChart(true);
        }
        #endregion

        #region //================================================================FFTGraph_MouseMove 
        private void FFTGraph_MouseMove(object sender, MouseEventArgs e)
        {
            CursorPointer.X = e.Location.X;
            CursorPointer.Y = e.Location.Y;
            if (m_YAxisScaleInLog == true)
            {
                FFTGraph.ChartAreas[0].CursorY.Interval = 0.1;
            }
            else
            {
                FFTGraph.ChartAreas[0].CursorY.Interval = 1;
            }
            
            FFTGraoh_UpdateCursor();
        }
        #endregion

        #region //================================================================FFTGraoh_UpdateCursor
        private void FFTGraoh_UpdateCursor()
        {
            FFTGraph.ChartAreas[0].CursorY.SetCursorPixelPosition(CursorPointer, true);
            FFTGraph.ChartAreas[0].CursorX.SetCursorPixelPosition(CursorPointer, true);

            int x = (int)FFTGraph.ChartAreas[0].CursorX.Position;
            if (x >= (m_iFFTElementCount-1))
            {
                x = ((int)m_iFFTElementCount-1);
            }
            if (x != 0)
            {
                DataPoint dp = seriesCh1.Points[x - 1];
                if (m_isCursorFreqAdvancedMode==false)
                    tbxAxisXFreq.Text = string.Format("{0:F2}", dp.XValue);
                else
                    tbxAxisXFreq.Text = string.Format("{0} || {1:F2}", (x-1), dp.XValue);
                tbxAxisYAmp.Text = string.Format("{0:F2}", dp.YValues[0]);
            }
        }
            #endregion

        #region //================================================================FFTGraph_FormatNumber
        private void FFTGraph_FormatNumber(object sender, FormatNumberEventArgs e)
        {
            Axis axis = sender as Axis;
            try
            {
                if (axis != null &&
                    e.ElementType == ChartElementType.AxisLabels &&
                    e.ValueType != ChartValueType.String)
                {
                    if (axis.AxisName == AxisName.Y)
                    {
                        if (m_YAxisScaleInVolt == true)
                        //-----------------------------------------------This is for FFT to Voltage transulation data section.
                        {
                            if (e.Value == 0)
                            {
                                e.LocalizedValue = "0.000";
                            }
                            else
                            {
                                //------LOG scale
                                if (m_YAxisScaleInLog == true)
                                {
                                    //e.LocalizedValue = string.Format("{0:F1}"+"mV", ((e.Value / m_iFFTYScale)*1000));
                                    e.LocalizedValue = string.Format("{0:0.0##e+00}", ((e.Value / m_iFFTYScale)));
                                }
                                //------Linear Scale (mV)
                                else
                                {
                                    e.LocalizedValue = string.Format("{0:F0}", ((e.Value / m_iFFTYScale) * 1000));
                                }
                            }
                        }
                        //-----------------------------------------------This is for raw FFT data section (Non translation). 
                        else
                        {
                            if (e.Value == 0)
                            {
                                e.LocalizedValue = "0.0";
                            }
                            else
                            {
                                //------LOG scale (FFT Raw Data)
                                if (m_YAxisScaleInLog == true)
                                {
                                    e.LocalizedValue = string.Format("{0:F0}", e.Value);
                                }
                                //------Linear Scale (FFT Raw Data)
                                else
                                {
                                    e.LocalizedValue = string.Format("{0:F0}", e.Value);
                                }
                            }
                        }
                    }
                }
            }
            catch (AmbiguousMatchException)
            {
                e.LocalizedValue = string.Format("#NaN#");
            }
        }
        #endregion

        #region //================================================================testDataAToolStripMenuItem_Click (128 element)
        private void testDataAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int rdata;
            Random r = new Random();
            string FFTFrame;
            //FFTFrame = "$F,128,1000,6553,37,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,317,413,27,0,0,0,0,0,0,0,0,0,0,0,0,2,8,5,2,10,300,16384,1045,30,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,$E\r\n";
            FFTFrame = "$F,128,1000,6553,37,";
            for (int i = 0; i < 128; i++)
            {
                rdata = 0;
                if (i == 18) rdata = 50;
                else if (i == 19) rdata = 150;
                else if (i == 20) rdata = 200;
                else if (i == 21) rdata = 35;
                else if (i == 49) rdata = 70;
                else if (i == 50) rdata = 300;
                else if (i == 51) rdata = 16384;
                else if (i == 52) rdata = 1045;
                else if (i == 53) rdata = 60;
                rdata += r.Next(0, 100);                // Add random noise floor. 
                FFTFrame += rdata.ToString() + ",";

            }
            FFTFrame += "$E\r\n";
            FFTRecievedData(FFTFrame);
        }
        #endregion

        #region //================================================================testDataBToolStripMenuItem_Click (256 element)
        private void testDataBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string FFTFrame = "$F,256,1000,6553,37,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,317,413,27,0,0,0,0,0,0,0,0,0,0,0,0,2,8,5,2,10,300,16384,1045,30,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,317,413,27,0,0,0,0,0,0,0,0,0,0,0,0,2,8,5,2,10,300,16384,1045,30,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,$E\r\n";
            FFTRecievedData(FFTFrame);
        }
        #endregion

        #region //================================================================yAsixInADCVoltToolStripMenuItem_Click
        private void yAsixInADCVoltToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_YAxisScaleInVolt == false)
            {
                m_YAxisScaleInVolt = true;
                yAsixInADCVoltToolStripMenuItem.Checked = true;
                //labYAxis.Text = "Y-Axis:: FFT mVpeak";
                //FFTGraph.ChartAreas[0].AxisX
                // ###TASK: Insert code to copy data into array, do amplitude modification and reinsert back in
                // Try using point type as experiment. 
            }
            else
            {
                m_YAxisScaleInVolt = false;
                yAsixInADCVoltToolStripMenuItem.Checked = false;
                //labYAxis.Text = "Y-Axis:: FFT Raw (Bin)";
                // ###TASK: Insert code to copy data into array, do amplitude modification and reinsert back in
                // Try using point type as experiment. 
                
            }
            UpdateFFTChart(true);
        }
        #endregion

        #region //================================================================stayTopToolStripMenuItem_Click
        private void stayTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stayTopToolStripMenuItem.Checked == false)
            {
                this.TopMost = true;
                stayTopToolStripMenuItem.Checked = true;

            }
            else
            {
                this.TopMost = false;
                stayTopToolStripMenuItem.Checked = false;
            }
        }
        #endregion

        #region //================================================================Random Data Generator
        private void testDataALoopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (testDataALoopToolStripMenuItem.Checked == false)
            {
                testDataALoopToolStripMenuItem.Checked = true;
                m_TestDataCeaseLoop = false;
                if (bgwTestDataA.IsBusy == false)  // avoid double threads.
                {
                    m_TestDataBackgroundCounter = 0;
                    bgwTestDataA.RunWorkerAsync();
                }
            }
            else
            {
                testDataALoopToolStripMenuItem.Checked = false;
                m_TestDataCeaseLoop = true;
            }
        }

        private void bgwTestDataA_DoWork(object sender, DoWorkEventArgs e)
        {
            m_TestDataBackgroundCounter++;
            Thread.Sleep(1);
        }

        private void bgwTestDataA_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (m_TestDataBackgroundCounter >= 100)
            {
                int rdata;
                Random r = new Random();
                string FFTFrame;
                FFTFrame = "$F,128,1000,6553,37,";
                for (int i = 0; i < 128; i++)
                {
                    rdata = 0;
                    if (i == 18) rdata = 50;
                    else if (i == 19) rdata = 150;
                    else if (i == 20) rdata = 200;
                    else if (i == 21) rdata = 35;
                    else if (i == 49) rdata = 70;
                    else if (i == 50) rdata = 300;
                    else if (i == 51) rdata = 16384;
                    else if (i == 52) rdata = 1045;
                    else if (i == 53) rdata = 60;
                    rdata += r.Next(0, 100);                // Add random noise floor. 
                    FFTFrame += rdata.ToString() + ",";
                }
                FFTFrame += "$E\r\n";
                FFTRecievedData(FFTFrame);
                m_TestDataBackgroundCounter = 0;
            }
            if (m_TestDataCeaseLoop == false)
            {
                bgwTestDataA.RunWorkerAsync();
            }
        }
        #endregion

        #region //================================================================tbxFFTAverager_KeyDown
        private void tbxFFTAverager_KeyDown(object sender, KeyEventArgs e)
        {
            UInt32 data;
            if (e.KeyCode == Keys.Enter)
            {

                if (!UInt32.TryParse(tbxFFTAverager.Text, out data))
                {
                    rtbFFTConsoleAppend("###Error: Incorrect Entry, must be numeric");
                    tbxFFTAverager.Text = "0";
                    return;
                }
                if (data == 0)
                {
                    m_FFTAvergerIsEnable = false;
                    m_FFTAvergerLoop = 0;
                    rtbFFTConsoleAppend("-INFO: Averager Mode Disabled");
                    return;
                }
                if (data <= 32)
                {
                    rtbFFTConsoleAppend("-INFO: Averager Mode Enabled : Loop=x" + data.ToString());
                    m_FFTAvergerIsEnable = true;
                    m_FFTAvergerLoop = data;
                    FFTAvgDataClear();
                }
                else
                {
                    m_FFTAvergerIsEnable = false;
                    m_FFTAvergerLoop = 0;
                    rtbFFTConsoleAppend("###Error: Incorrect Entry, must be less than 32");
                    FFTAvgDataClear();
                }

            }
        }
        #endregion

        #region //================================================================FFTAvgDataClear, clear all data
        private void FFTAvgDataClear()
        {
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 1024; j++)
                {
                    m_dFFTAvgData[i, j] = 0;
                }
            }
            m_FFTAvergerAddress = 0;
            m_FFTAvergerFill = 0;
        }
        #endregion

        #region //================================================================FFTLinearProcess, Process linear (non-average) data
        private void FFTLinearProcess()
        {
            for (int i = 0; i < m_iFFTElementCount; i++)
            {
                seriesCh1.Points.AddXY((i * m_dFFTIntervel_Hz), m_dFFTDisplayData[i]);
            }
        }
        #endregion

        #region //================================================================FFTAvgProcess, Process average data
        private void FFTAvgProcess()
        {
            // m_FFTAvergerLoop;        // Number of loop, max 32
            // m_FFTAvergerAddress;     // Array Pointer (based on FIFO data)
            // m_FFTAvergerFill;        // Fill number for average calculation process until equal to m_FFTAvergerLoop, use for start process. 
            double data;
            try
            {
                //-------Copy captured data into averager array
                for (int i = 0; i < m_iFFTElementCount; i++)
                {
                    m_dFFTAvgData[m_FFTAvergerAddress, i] = m_dFFTDisplayData[i];
                }
                //-------Manage averager pointer to array
                m_FFTAvergerAddress++;
                if (m_FFTAvergerFill != m_FFTAvergerLoop) m_FFTAvergerFill++;
                if (m_FFTAvergerAddress == m_FFTAvergerLoop)
                {
                    m_FFTAvergerAddress = 0;                    // Reset avergaer array pointer
                    m_FFTAvergerFill = (m_FFTAvergerLoop);      // Array is full with data. 
                }
                //------Do average process and put results to chart
                for (int i = 0; i < m_iFFTElementCount; i++)
                {
                    data = 0;
                    for (int j = 0; j < m_FFTAvergerFill; j++)
                    {
                        data += m_dFFTAvgData[j, i];

                    }
                    data = data / m_FFTAvergerFill;
                    m_dFFTDisplayData[i] = data;
                    seriesCh1.Points.AddXY((i * m_dFFTIntervel_Hz), data);
                }
            }
            catch (System.Exception ex)
            {
                rtbFFTConsoleAppend("#ERR: Exception Event-A FFTAvgProcess() Exception:" + ex.ToString());
            }
        }
        #endregion

        #region //================================================================cBxYZOOM_CheckedChanged
        private void cBxYZOOM_CheckedChanged(object sender, EventArgs e)
        {            
            if (m_YAxisisYZoomMode == false)
            {
                //cBxYZOOM.Checked = true;
                m_YAxisisYZoomMode = true;
                cBxYZOOM.Text = "X-Zoom";
                cBxYZOOM.BackColor = System.Drawing.Color.Gainsboro;
                cBxYZOOM.FlatStyle = FlatStyle.Popup;
                //-------------------------------------------------------------Turn off the zoom for X-axis.
                FFTGraph.ChartAreas[0].CursorX.IsUserEnabled = false;
                FFTGraph.ChartAreas[0].CursorX.IsUserSelectionEnabled = false;
                FFTGraph.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
                //-------------------------------------------------------------Turn on the zoom on Y-Axis.
                FFTGraph.ChartAreas[0].CursorY.IsUserEnabled = true;
                FFTGraph.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
                FFTGraph.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
                FFTGraph.ChartAreas[0].AxisY.ScrollBar.Enabled = false;                 //### Scale bug, fix this 1st before using Y zoom feature
                //FFTGraph.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;
                //FFTGraph.ChartAreas[0].CursorY.Interval = 1;
            }
            else
            {
                cBxYZOOM.Checked = false;
                m_YAxisisYZoomMode = false;
                cBxYZOOM.Text = "Y-Zoom";
                cBxYZOOM.BackColor = System.Drawing.SystemColors.Control;
                cBxYZOOM.FlatStyle = FlatStyle.Standard;
                //-------------------------------------------------------------Turn on the zoom for X-axis.
                FFTGraph.ChartAreas[0].CursorX.IsUserEnabled = true;
                FFTGraph.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                FFTGraph.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                //-------------------------------------------------------------Turn off the zoom on Y-Axis.
                FFTGraph.ChartAreas[0].CursorY.IsUserEnabled = false;
                FFTGraph.ChartAreas[0].CursorY.IsUserSelectionEnabled = false;
                FFTGraph.ChartAreas[0].AxisY.ScaleView.Zoomable = false;
            }
        }
        #endregion

        #region //================================================================FFTWindow_FormClosing
        private void FFTWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true; // this cancels the close event.
            myGlobalBase.FFTOpertaionMode = false;
        }
        #endregion

        #region //================================================================btnLogLinYAxis_Click
        private void btnLogLinYAxis_Click(object sender, EventArgs e)
        {
            if (btnLogLinYAxis.Text == "Y-Log")
            {
                btnLogLinYAxis.Text = "Y-Linear";
                m_YAxisScaleInLog = true;
                tsmYLogSelect.Checked = true;
                tsmYLinearSelect.Checked = false;
            }
            else
            {
                btnLogLinYAxis.Text = "Y-Log";
                m_YAxisScaleInLog = false;
                tsmYLogSelect.Checked = false;
                tsmYLinearSelect.Checked = true;
            }
            UpdateFFTChart(true);

        }
        #endregion

        #region //================================================================cBox_Marker_CheckedChanged
        private void cBox_Marker_CheckedChanged(object sender, EventArgs e)
        {
            if (this.FFTGraph.Series[0].MarkerStyle == MarkerStyle.None)
            {
                this.cBox_Marker.Checked = true;
                this.FFTGraph.Series[0].MarkerStyle = MarkerStyle.Circle;
                this.FFTGraph.Series[0].MarkerColor = System.Drawing.Color.LightGoldenrodYellow;
                this.FFTGraph.Series[0].MarkerSize = 4;
            }
            else
            {
                this.cBox_Marker.Checked = false;
                this.FFTGraph.Series[0].MarkerStyle = MarkerStyle.None;
            }
            UpdateFFTChart(false);
        }
        #endregion

        #region //================================================================cBox_SQRTY_CheckedChanged
        private void cBox_SQRTY_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cBox_SQRTY.Checked==true)
            {
                m_YAxisSQRTY = true;
            }
            else
            {
                m_YAxisSQRTY = false;
            }
            UpdateFFTChart(true);
        }
        #endregion

        #region //================================================================YScale Menu Items
        private void tsmAutoScale_Click(object sender, EventArgs e)
        {
            YScaleMenuItemUnchecked();
            tsmAutoScale.Checked = true;
            UpdateFFTChart(true);
        }

        private void tsmHoldAutoScale_Click(object sender, EventArgs e)
        {
            YScaleMenuItemUnchecked();
            tsmHoldAutoScale.Checked = true;
            UpdateFFTChart(true);
        }

        private void tsm2K_Click(object sender, EventArgs e)
        {
            YScaleMenuItemUnchecked();
            tsm2K.Checked = true;
            FFTGraph.ChartAreas[0].AxisY.Maximum = 2000;
            FFTGraph.ChartAreas[0].AxisY.Interval = 2000 / 20;
            UpdateFFTChart(true);
        }

        private void tsm20K_Click(object sender, EventArgs e)
        {
            YScaleMenuItemUnchecked();
            tsm20K.Checked = true;
            FFTGraph.ChartAreas[0].AxisY.Maximum = 20000;
            FFTGraph.ChartAreas[0].AxisY.Interval = 20000 / 20;
            //UpdateFFTChart(true);
        }

        private void tsm70K_Click(object sender, EventArgs e)
        {
            YScaleMenuItemUnchecked();
            tsm70K.Checked = true;
            FFTGraph.ChartAreas[0].AxisY.Maximum = 70000;
            FFTGraph.ChartAreas[0].AxisY.Interval = 70000 / 20;
            //UpdateFFTChart(true);
        }

        private void tsm2M_Click(object sender, EventArgs e)
        {
            YScaleMenuItemUnchecked();
            tsm2M.Checked = true;
            FFTGraph.ChartAreas[0].AxisY.Maximum = 2000000;
            FFTGraph.ChartAreas[0].AxisY.Interval = 2000000 / 20;
            //UpdateFFTChart(true);
        }

        private void tsm10M_Click(object sender, EventArgs e)
        {
            YScaleMenuItemUnchecked();
            tsm10M.Checked = true;
            FFTGraph.ChartAreas[0].AxisY.Maximum = 10000000;
            FFTGraph.ChartAreas[0].AxisY.Interval = 10000000 / 20;
            //UpdateFFTChart(true);
        }

        private void tsm20M_Click(object sender, EventArgs e)
        {
            YScaleMenuItemUnchecked();
            tsm20M.Checked = true;
            FFTGraph.ChartAreas[0].AxisY.Maximum = 20000000;
            FFTGraph.ChartAreas[0].AxisY.Interval = 20000000 / 20;
            //UpdateFFTChart(true);
        }

        private void tsm2G5_Click(object sender, EventArgs e)
        {
            YScaleMenuItemUnchecked();
            tsm2G5.Checked = true;
            FFTGraph.ChartAreas[0].AxisY.Maximum = 2500000000;
            FFTGraph.ChartAreas[0].AxisY.Interval = 2500000000 / 20;
            //UpdateFFTChart(true);
        }

        private void tsm5G_Click(object sender, EventArgs e)
        {
            YScaleMenuItemUnchecked();
            tsm5G.Checked = true;
            FFTGraph.ChartAreas[0].AxisY.Maximum = 5000000000;
            FFTGraph.ChartAreas[0].AxisY.Interval = 5000000000 / 20;
            //UpdateFFTChart(true);
        }
        #endregion

        #region //================================================================YScaleMenuItemUnchecked
        private void YScaleMenuItemUnchecked()
        {
            tsm5G.Checked = false;
            tsm2G5.Checked = false;
            tsm20M.Checked = false;
            tsm10M.Checked = false;
            tsm2M.Checked = false;
            tsm70K.Checked = false;
            tsm20K.Checked = false;
            tsm2K.Checked = false;
            tsmHoldAutoScale.Checked = false;
            tsmAutoScale.Checked = false;
        }
        #endregion

        #region //================================================================tsmYScaleUpStep_Click/tsmYScaleUpDown_Click
        private void tsmYScaleUpStep_Click(object sender, EventArgs e)
        {
            tsmYScaleDownStep.Checked = false;
            tsmYScaleUpStep.Checked = false;
            tsmAutoScale.Checked = false;
            YScaleMenuItemUnchecked();
            if (StepPointer < Step.Length-1)
            {
                StepPointer++;
                FFTGraph.ChartAreas[0].AxisY.Maximum = Step[StepPointer];
                FFTGraph.ChartAreas[0].AxisY.Interval = Step[StepPointer] / 20;
            }
            UpdateFFTChart(true);
        }

        private void tsmYScaleDownStep_Click(object sender, EventArgs e)
        {
            tsmYScaleDownStep.Checked = false;
            tsmYScaleUpStep.Checked = false;
            tsmAutoScale.Checked = false;
            YScaleMenuItemUnchecked();
            if (StepPointer > 6)        // Limit to 100 minimum. 
            {
                StepPointer--;
                FFTGraph.ChartAreas[0].AxisY.Maximum = Step[StepPointer];
                FFTGraph.ChartAreas[0].AxisY.Interval = Step[StepPointer] / 20;
            }
            UpdateFFTChart(true);
        }
        #endregion

        #region //================================================================btn_YScaleDown_Click/btn_YScaleUp_Click/btn_YScaleAuto_Click
        private void btn_YScaleDown_Click(object sender, EventArgs e)
        {
            tsmYScaleDownStep.Checked = false;
            tsmYScaleUpStep.Checked = false;
            tsmAutoScale.Checked = false;
            YScaleMenuItemUnchecked();
            if (StepPointer > 6)        // Limit to 100 minimum. 
            {
                StepPointer--;
                FFTGraph.ChartAreas[0].AxisY.Maximum = Step[StepPointer];
                FFTGraph.ChartAreas[0].AxisY.Interval = Step[StepPointer] / 20;
            }
            UpdateFFTChart(true);
        }

        private void btn_YScaleUp_Click(object sender, EventArgs e)
        {
            tsmYScaleDownStep.Checked = false;
            tsmYScaleUpStep.Checked = false;
            tsmAutoScale.Checked = false;
            YScaleMenuItemUnchecked();
            if (StepPointer < Step.Length-1)
            {
                StepPointer++;
                FFTGraph.ChartAreas[0].AxisY.Maximum = Step[StepPointer];
                FFTGraph.ChartAreas[0].AxisY.Interval = Step[StepPointer] / 20;
            }
            UpdateFFTChart(true);
        }

        private void btn_YScaleAuto_Click(object sender, EventArgs e)
        {
            YScaleMenuItemUnchecked();
            tsmAutoScale.Checked = true;
            UpdateFFTChart(true);
        }
        #endregion

        #region //================================================================tsmYLogSelect_Click/tsmYLinearSelect_Click
        private void tsmYLogSelect_Click(object sender, EventArgs e)
        {
            btnLogLinYAxis.Text = "Y-Linear";
            m_YAxisScaleInLog = true;
            tsmYLogSelect.Checked = true;
            tsmYLinearSelect.Checked = false;
            UpdateFFTChart(true);
        }

        private void tsmYLinearSelect_Click(object sender, EventArgs e)
        {
                btnLogLinYAxis.Text = "Y-Log";
                m_YAxisScaleInLog = false;
                tsmYLogSelect.Checked = false;
                tsmYLinearSelect.Checked = true;
                UpdateFFTChart(true);
        }
        #endregion

        #region //================================================================BtnExtendDisplay_Click/ExtendDisplayUpdate
        private void BtnExtendDisplay_Click(object sender, EventArgs e)
        {
            m_isExtendDisplayEnabled = !m_isExtendDisplayEnabled;
            if (m_isExtendDisplayEnabled==true)
                BtnExtendDisplay.Text = "Compact";
            else
                BtnExtendDisplay.Text = "Extend";
            ExtendDisplayUpdate();
        }
        private void ExtendDisplayUpdate()
        {
            if (m_isExtendDisplayEnabled==true)
            {
                int xwidth = 1820;      //Extend Parameter
                int ywidth = 980;
                //-----------------------------------------------------------------FFT Window.
                this.MaximumSize = new System.Drawing.Size(xwidth, ywidth);
                this.MinimumSize = new System.Drawing.Size(xwidth, ywidth);
                this.ClientSize = new System.Drawing.Size((xwidth-16), (ywidth-39));
                this.StartPosition = FormStartPosition.CenterScreen;
                //----------------------------------------------------------------FFTGraph
                FFTGraph.MaximumSize = new System.Drawing.Size(xwidth - 40, ywidth - 316);
                FFTGraph.MinimumSize = new System.Drawing.Size(xwidth - 40, ywidth - 316);
                FFTGraph.Size = new System.Drawing.Size((xwidth - 40), (ywidth - 316));
                //----------------------------------------------------------------rtbFFTConsole 
                rtbFFTConsole.MaximumSize = new System.Drawing.Size(xwidth - 40, 143);
                rtbFFTConsole.MinimumSize = new System.Drawing.Size(xwidth - 40, 143);
                rtbFFTConsole.Size = new System.Drawing.Size(xwidth - 40, 143);
                rtbFFTConsole.Location = new System.Drawing.Point(13, 556 + (ywidth - 750));
            }
            else
            {
                int xwidtho = 700;      //Compact parameter
                int ywidtho = 750;
                //-----------------------------------------------------------------FFT Window.
                this.MaximumSize = new System.Drawing.Size(xwidtho, ywidtho);
                this.MinimumSize = new System.Drawing.Size(xwidtho, ywidtho);
                this.ClientSize = new System.Drawing.Size((xwidtho - 16), (ywidtho - 39));
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;
                //----------------------------------------------------------------FFTGraph
                FFTGraph.MaximumSize = new System.Drawing.Size(xwidtho - 40, ywidtho-316);
                FFTGraph.MinimumSize = new System.Drawing.Size(xwidtho - 40, ywidtho-316);
                FFTGraph.Size = new System.Drawing.Size((xwidtho-40), (ywidtho - 316));
                //----------------------------------------------------------------rtbFFTConsole Position = 13, 556
                rtbFFTConsole.MaximumSize = new System.Drawing.Size(659,143);
                rtbFFTConsole.MinimumSize = new System.Drawing.Size(659,143);
                rtbFFTConsole.Location= new System.Drawing.Point(13, 556);
            }

        }
        #endregion

        #region //================================================================btnSuspend_Click
        private void btnSuspend_Click(object sender, EventArgs e)
        {
            if (btnSuspend.Text=="Suspend")
            {
                btnSuspend.Text ="Continue";
                m_SuspendMode = true;

            }
            else
            {
                btnSuspend.Text = "Suspend";
                m_SuspendMode = false;
            }
        }
        #endregion

        #region//================================================================UpdateYScaleBox & checkbox related to this. 
        // 0 = Raw Data
        // 1 = User Data (override the metadata, supplied by MCU)
        // 2 = MetaData as part of data string from MCU/DSP device. 
        private void UpdateYScaleBox(UInt32 ScaleMode)
        {
            m_YAxisScaleMode = ScaleMode;
            switch (ScaleMode)
            {
                case 0:
                    {
                        cbYScaleSelectMetaData.Checked = false;
                        cbYScaleSelectUser.Checked = false;
                        cbYScale_SelectRawData.Checked = true;
                        tbYScaleUser.ReadOnly = true;
                        tbYScaleMetaData.ReadOnly = true;
                        break;
                    }
                case 1:
                    {
                        cbYScaleSelectMetaData.Checked = false;
                        cbYScaleSelectUser.Checked = true;
                        cbYScale_SelectRawData.Checked = false;
                        tbYScaleUser.ReadOnly = false;
                        tbYScaleMetaData.ReadOnly = true;
                        break;
                    }
                case 2:
                    {
                        cbYScaleSelectMetaData.Checked = true;
                        cbYScaleSelectUser.Checked = false;
                        cbYScale_SelectRawData.Checked = false;
                        tbYScaleMetaData.Text = m_iFFTYScale.ToString();
                        tbYScaleUser.ReadOnly = true;
                        tbYScaleMetaData.ReadOnly = true;
                        break;
                    }
                default:
                    break;
            }

        }

        private void cbYScale_SelectRawData_MouseClick(object sender, MouseEventArgs e)
        {
            UpdateYScaleBox(0);
            m_YAxisScaleInVolt = false;
            yAsixInADCVoltToolStripMenuItem.Checked = false;
            UpdateFFTChart(true);
        }

        private void cbYScaleSelectUser_MouseClick(object sender, MouseEventArgs e)
        {
            UInt32 data;

            if (!UInt32.TryParse(tbYScaleUser.Text, out data))
            {
                rtbFFTConsoleAppend("###Error: Incorrect Entry, must be numeric whole number (no decimal)");
                tbYScaleUser.Text = "1";
                return;
            }
            if (data == 0)
            {
                rtbFFTConsoleAppend("###Error: Incorrect Entry, must be non zero from 1");
                tbYScaleUser.Text = "1";
                return;
            }
            m_iFFTYScale = data;
            UpdateYScaleBox(1);
            m_YAxisScaleInVolt = true;
            yAsixInADCVoltToolStripMenuItem.Checked = true;
            UpdateFFTChart(true);
        }

        private void cbYScaleSelectMetaData_MouseClick(object sender, MouseEventArgs e)
        {
            tbYScaleMetaData.Text = m_iFFTYScale.ToString();
            UpdateYScaleBox(2);
            m_YAxisScaleInVolt = true;
            yAsixInADCVoltToolStripMenuItem.Checked = true;
            UpdateFFTChart(true);
        }
        #endregion

        #region//================================================================tbxAxisXFreq_TextChanged for m_isCursorFreqAdvancedMode
        private void tbxAxisXFreq_MouseClick(object sender, MouseEventArgs e)
        {
            m_isCursorFreqAdvancedMode = !m_isCursorFreqAdvancedMode;       //Toggle state
            if (m_isCursorFreqAdvancedMode==false)
                tbxAxisXFreq.BackColor = SystemColors.Control; 
            else
                tbxAxisXFreq.BackColor = System.Drawing.Color.Bisque;
            FFTGraoh_UpdateCursor();
        }
        #endregion

        #region//================================================================aboutToolStripMenuItem_Click
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FFT_HelpAbout box = new FFT_HelpAbout();
            box.ShowDialog();
        }
        #endregion

        #region//================================================================tbxAxisXFreq_TextChanged
        private void btnEditData_Click(object sender, EventArgs e)
        {
            if (myFFT_DataEditor == null)
            {
                //------------------------------------FFT Data Editor
                myFFT_DataEditor = new FFT_DataEditor();
                myFFT_DataEditor.MyGlobalBase(myGlobalBase);
                myFFT_DataEditor.MyFFTWindow(this);
            }
            myGlobalBase.FFTOpertaionMode = true;
            myFFT_DataEditor.Show();
         
        }
        #endregion

        #region//================================================================FFTDataEditor_UpdateFFTChart
        public void FFTDataEditor_UpdateFFTChart(string FFTFrame)
        {
            m_SuspendMode = false;
            FFTRecievedData(FFTFrame);
            m_SuspendMode = true;
        }
        #endregion

    }

    #region//======================================================DoubleBuffered property via reflection for RichTextBox, improve flickerless image.
    public static class ExtensionMethodsFFT
    {
        public static void DoubleBufferedFFT(this Chart dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    #endregion
}

            