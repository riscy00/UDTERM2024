using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Timers;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using JR.Utils.GUI.Forms;
using Timer = System.Timers.Timer;


//using Microsoft.Expression.Utility.Extensions.Enumerable;

// Power Tool: insert QuickLaunch with "@tasks CollapseRegions" to close all region only. 

namespace UDT_Term_FFT
{
    public partial class Survey : Form
    {
        ITools Tools = new Tools();
        USB_FTDI_Comm myUSBComm;                    // FTDI device manager section (scan, open, close, read, write).
        USB_VCOM_Comm myUSBVCOMComm;
        GlobalBase myGlobalBase;
        RiscyScope myRiscyScope;
        HRM2300 myHRM2300;
        FluxGateII myFGIIA;
        DataTable DGVtable;
        Calibration myCalibration;
        MainProg myMainProg;
        BindingList<DeviceStatusComm> blyDeviceStatusComm;
        //--------------------------------------Public Getter/Setter
        public bool isMinimise { get; set; }
        private bool isDebugNoSensorConfirmedOkay { get; set; }
        //--------------------------------------COM channel names.
        string[] DeviceName = new string[] { "Generic", "HMR2300", "FGII_A", "FGII_B", "ADIS16460", "Spare1","Spare2","Spare3","Spare4" };
        //--------------------------------------Message Frame
        private string RecievedDataForFiling;
        private string m_sEntryTxt = "";
        private List<string> sFrame;
        string HeaderFrameTool = "Date;Time;mSecTick;UTC;HR_MagX;HR_MagY;HR_MagZ;FG_MagX;FG_MagY;FG_MagZ;FG_Temp;AD_AccelX;AD_AccelY;AD_AccelZ;AD_GryoX;AD_GryoY;AD_GryoZ;AD_Temp";
        //--------------------------------------DGV
        private List<string> HeaderColumn;
        private List<string> FormatColumn;
        //---------------------------------------Update Survey Readout 
        Timer Surveytimer;
        Regex SurveyHMRrg;
        Regex SurveyFGIIrg;
        bool isSurveyTimerPauseEnabled;
        private object[] myRow;
        private long lTimeStampTick;
        private int LoopProcessCounter;
        //---------------------------------------Filename
        string sDefaultFolder;
        string sFilename;
        string sAppendFilename;
        //---------------------------------------RiscyScope, buffer data for real time update and scrolling feature. New approach for charting. 
        DateTime dtNow;
        int ChartSelectedColumn;
        int ChartSelectedStartRow;              // highlight selected only Start
        int ChartSelectedEndRow;                // highlight selected only End
        int ChartStartRow;                      // Normally 0, beginning of the row
        int ChartEndRow;                        // Normally max row count, end of the row

        int[] ChartSelectedCh = new int[4];     // ChartSelectedCh[0] = -1   ==> Not Selected for CH0. // ChartSelectedCh[0] = e.ColumnIndex  ==> Selected. 
        private List<cPoint> ScopeCH0;
        private List<cPoint> ScopeCH1;
        private List<cPoint> ScopeCH2;
        private List<cPoint> ScopeCH3;
        //---------------------------------------

        #region //============================================================Reference Object
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
            //--------------------------------------Themes
            switch (myGlobalBase.CompanyName)
            {
                case (int)GlobalBase.eCompanyName.BGDS:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.BG_MainPage_Blank_1B;
                        break;
                    }
                case (int)GlobalBase.eCompanyName.ADT:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_Blank_1B;
                        break;
                    }
                case ((int)GlobalBase.eCompanyName.TVB):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPageTVBBlank_1B;
                        break;
                    }
                default:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_Blank_1B;
                        break;
                    }
            }
            this.Refresh();
        }

        public void MyUSBComm(USB_FTDI_Comm myUSBCommRef)
        {
            myUSBComm = myUSBCommRef;
        }
        public void MyHMR2300(HRM2300 myHRM2300Ref)
        {
            myHRM2300 = myHRM2300Ref;
        }

        public void MyFGII(FluxGateII myFGIIAREf)
        {
            myFGIIA = myFGIIAREf;
        }

        public void MyUSBVCOMComm(USB_VCOM_Comm myUSBVCOMCommRef)
        {
            myUSBVCOMComm = myUSBVCOMCommRef;
        }

        public void MyRiscyScope(RiscyScope myRiscyScopeRef)
        {
            myRiscyScope = myRiscyScopeRef;
        }

        #endregion

        public Survey()
        {
            InitializeComponent();
            dgvSurveyViewer.DoubleBufferedMessage1(true);
            isMinimise = false;
            SurveyHMRrg = new Regex(@"^[0-9\s,+-]*$");
            SurveyFGIIrg = new Regex(@"^[0-9\s,+-]*$E");
            isSurveyTimerPauseEnabled = false;
            sDefaultFolder = @"F:\UDT_Survey";
            sFilename = "";
            RiscyScopeChart_Init();         // Init Chart Setup. 
            isDebugNoSensorConfirmedOkay = false;
        }

        //#####################################################################################################
        //###################################################################################### Form Manager
        //#####################################################################################################

        #region //================================================================Survey_FormClosing
        private void Survey_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (myGlobalBase.Svy_isSurveyCVSRawDataEnable == false)       // Active process, cannot close. 
            {
                myGlobalBase.Svy_isSurveyCVSOpen = false;                 // Cease Survey CVS Terminal mode.
                this.Visible = false;
                this.Hide();
            }
        }
        #endregion

        #region //================================================================Svy_Show
        public void Svy_Show(bool isDataLog)
        {
            //-------------------------------------------------Define Default Folder.
            myRiscyScope.cScope_DefaultFolder(sDefaultFolder);
            if (myCalibration != null)
                myCalibration.cCal_DefaultFolder(sDefaultFolder);
            //-------------------------------------------------Manage window. 
            this.BringToFront();
            this.TopMost = false;
            this.WindowState = FormWindowState.Normal;
            if (isDataLog == false)
            {
                myGlobalBase.Svy_isSurveyCVSOpen = true;
                groupBox1.Visible = true;
                groupBox3.Visible = true;
            }
            else
            {
                myGlobalBase.Svy_isSurveyCVSOpen = false;
                groupBox1.Visible = false;
                groupBox3.Visible = false;
            }
            this.Visible = true;
            this.Show();
            // Update COM Array Box
            dgv_DeviceStatus_Update();
        }
        #endregion

        #region //================================================================Svy_Close
        public void Svy_Close(bool isDataLog)
        {
            if (myGlobalBase.Svy_isSurveyCVSRawDataEnable == false)       // Active process, cannot close. 
            {
                if (isDataLog == false)
                    myGlobalBase.Svy_isSurveyCVSOpen = false;                 // Cease Survey CVS Terminal mode.
                this.Visible = false;
                this.Hide();
            }
        }
        #endregion

        #region //================================================================Survey_Load
        private void Survey_Load(object sender, EventArgs e)
        {
            this.dgvSurveyViewer.RowsDefaultCellStyle.BackColor = Color.Bisque;
            this.dgvSurveyViewer.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;           //Color.Bisque, Color.Beige

            btnReset.Visible = false;
            cbSuspendScroll.Checked = false;
            if (myGlobalBase.Svy_isSurveyCVSOpen==true)
                Survey_ClearAndUpdateDataGridView(HeaderFrameTool, false);
            else
                Survey_ClearAndUpdateDataGridView("", false);
            cbMFormat.SelectedIndex = 2;
            btnPause.Enabled = false;
            btnStop.Enabled = false;
            cbAutoLoop.Enabled = true;
            txtFolderName.Text = sDefaultFolder;
            dtNow = DateTime.Now;
            //--------------------------------------Setup Calibration Window.
            if (myCalibration == null)
            {
                myCalibration = new Calibration();
                myCalibration.MySurveyRef(this);
                myCalibration.MyMainProg(myMainProg);
                myCalibration.MyGlobalBase(myGlobalBase);
                myCalibration.MydgvSurveyViewer(dgvSurveyViewer);
                myCalibration.cCal_DefaultFolder(sDefaultFolder);
                myCalibration.Calibration_Init();
            }
            dgv_DeviceStatus_Init();
        }
        #endregion

        #region //================================================================Survey_Resize
        private void Survey_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                isMinimise = true;
            else
                isMinimise = false;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Comm Port Information Panel.
        //#####################################################################################################

        #region //================================================================ScopeDataOp_Init
        private void dgv_DeviceStatus_Init()
        {
            if (dgvDeviceStatus.RowCount >= 3)
                return;                                 // Already Initialized, no need to repeat. 

            List<DeviceStatusComm> myDeviceStatusComm;

            myDeviceStatusComm = new List<DeviceStatusComm>();
            for (int i = 0; i < myGlobalBase.myUSBVCOMArray.Count; i++)
            {
                myDeviceStatusComm.Add(new DeviceStatusComm());
                myDeviceStatusComm[i].Device = DeviceName[i];
                myDeviceStatusComm[i].Com = "---";
                myDeviceStatusComm[i].Status = "--";
            }
            //-----------------------------------------------Now we bind the above to datagridview
            blyDeviceStatusComm = new BindingList<DeviceStatusComm>(myDeviceStatusComm);
            var source = new BindingSource(blyDeviceStatusComm, null);
            dgvDeviceStatus.DataSource = source;
            //-----------------------------------------------Now Tweaks Layout for best viewing. 
            for (int i = 0; i < dgvDeviceStatus.RowCount; i++)
            {
                dgvDeviceStatus.Rows[i].Frozen = false;
                dgvDeviceStatus.Rows[i].Height = 17;
                dgvDeviceStatus.Rows[i].MinimumHeight = 17;
            }
            dgvDeviceStatus.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDeviceStatus.Columns[0].Width = 40;
            dgvDeviceStatus.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDeviceStatus.Columns[1].Width = 60;
            dgvDeviceStatus.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvDeviceStatus.Columns[2].Width = 50;
        }
        #endregion

        #region //================================================================dgv_DeviceStatus_Update
        public void dgv_DeviceStatus_Update()
        {
            if (blyDeviceStatusComm == null)        // Not initialized  
                return;
            try
            {
                if (myGlobalBase.is_Serial_Server_Connected==true)
                {
                    dgvDeviceStatus.Visible = false;        // Hide Array COM Box due to global VCOM connection
                    return;
                }
                else
                {
                    for (int i = 0; i < myGlobalBase.myUSBVCOMArray.Count; i++)
                    {
                        blyDeviceStatusComm[i].Com = "---";
                        blyDeviceStatusComm[i].Status = "--";
                    }
                    if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == true)
                    {
                        blyDeviceStatusComm[(int)eUSBDeviceType.ADIS].Com = (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].ComPort.ToString()).Replace("COM", "");
                        blyDeviceStatusComm[(int)eUSBDeviceType.ADIS].Status = "ON";
                    }
                    if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isSerial_Server_Connected == true)
                    {
                        blyDeviceStatusComm[(int)eUSBDeviceType.HMR2300].Com = (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].ComPort.ToString()).Replace("COM", "");
                        blyDeviceStatusComm[(int)eUSBDeviceType.HMR2300].Status = "ON";
                    }
                    if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isSerial_Server_Connected == true)
                    {
                        blyDeviceStatusComm[(int)eUSBDeviceType.FGII_A].Com = (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].ComPort.ToString()).Replace("COM", "");
                        blyDeviceStatusComm[(int)eUSBDeviceType.FGII_A].Status = "ON";
                    }
                    if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_B].isSerial_Server_Connected == true)
                    {
                        blyDeviceStatusComm[(int)eUSBDeviceType.FGII_B].Com = (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_B].ComPort.ToString()).Replace("COM", "");
                        blyDeviceStatusComm[(int)eUSBDeviceType.FGII_B].Status = "ON";
                    }
                    if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.Generic].isSerial_Server_Connected == true)
                    {
                        blyDeviceStatusComm[(int)eUSBDeviceType.Generic].Com = (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.Generic].ComPort.ToString()).Replace("COM", "");
                        blyDeviceStatusComm[(int)eUSBDeviceType.Generic].Status = "ON";
                    }
                    if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == true)
                    {
                        blyDeviceStatusComm[(int)eUSBDeviceType.BGDRILLING].Com = (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].ComPort.ToString()).Replace("COM", "");
                        blyDeviceStatusComm[(int)eUSBDeviceType.BGDRILLING].Status = "ON";
                    }
                }
            }
            catch { }

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Start/Stop/Pause Section
        //#####################################################################################################

        #region //================================================================btnStartProcess_Click
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (isDebugNoSensorConfirmedOkay == false)      // Become Debug Mode after clicking Start twice. 
            {
                isDebugNoSensorConfirmedOkay = true;        // Treat as debug purpose for testing only.
                if ((myGlobalBase.is_SvyADISSerial_Server_Connected == false) & (myGlobalBase.is_SvyHMRSerial_Server_Connected == false) & (myGlobalBase.is_SvyFGIISerial_Server_Connected == false))
                {
                    FlexiMessageBox.Show("#ERR: ADIS and HMR2300 and FluxGateII Serial Port is not connected, please connect first (Repeat Start to skip this)", "Serial Comm Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                    return;
                }
            }

            if (cbAppendFile.Checked == true)
            {
                if (!Directory.Exists(sDefaultFolder))
                {
                    if (Survey_ProjectFolder() == false)
                    {
                        myMainProg.myRtbTermMessageLF("#ERR: Unable to append file due to folder location issue. Unchecked 'Append File' if not used.");
                        return;
                    }
                }
            }
            //--------------------------------------
            Svy_InitForSurveyCVS_RecievedData();
            //--------------------------------------Start Survey Now HMR2300
            Survey_ProcessRowNow();
            Console.Beep(1000, 20);
            //--------------------------------------Activate Loop if checked.
            if (cbAutoLoop.Checked == true)
            {
                //--------------------------------------Setup Control. 
                isSurveyTimerPauseEnabled = false;
                btnPause.Text = "Pause";
                btnStart.Enabled = false;
                btnPause.Enabled = true;
                btnStop.Enabled = true;
                tbHMRLoop.Enabled = false;
                cbAutoLoop.Enabled = false;
                Survey_Timer_Start();
            }
            else
            {
                isSurveyTimerPauseEnabled = false;
                btnPause.Text = "Pause";
                btnStop.Enabled = false;
                btnPause.Enabled = false;
                btnStart.Enabled = true;
                tbHMRLoop.Enabled = true;
                cbAutoLoop.Enabled = true;
            }
            if (cbAppendFile.Checked == true)
            {
                Survey_AppendHeaderToFile();
            }
        }
        #endregion

        #region//================================================================btnStop_Click
        private void btnStop_Click(object sender, EventArgs e)
        {
            Survey_Timer_Stop();
            dgvSurveyViewer.ResumeLayout(false);

        }
        #endregion

        #region//================================================================btnPause_Click
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (isSurveyTimerPauseEnabled == false)
            {
                isSurveyTimerPauseEnabled = true;
                btnPause.Text = "Cont";
            }
            else
            {
                isSurveyTimerPauseEnabled = false;
                btnPause.Text = "Pause";
            }

        }
        #endregion

        #region//================================================================HMRClosePort (cease loop)
        public void HMRClosePort()
        {
            Survey_Timer_Stop();
            dgvSurveyViewer.ResumeLayout(false);
        }
        #endregion

        #region//================================================================ADISClosePort (cease loop)
        public void ADISClosePort()
        {
            Survey_Timer_Stop();
            dgvSurveyViewer.ResumeLayout(false);
        }
        #endregion

        #region//================================================================FGIIClosePort (cease loop)
        public void FGIIClosePort()
        {
            Survey_Timer_Stop();
            dgvSurveyViewer.ResumeLayout(false);
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Timer Loops
        //#####################################################################################################

        #region //================================================================Survey_Timer_Start
        //==========================================================
        // Purpose  : This code monitor the received HMR2300 message 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void Survey_Timer_Start()
        {
            if (Surveytimer == null)
            {
                Surveytimer = new Timer();
                Surveytimer.Elapsed += new System.Timers.ElapsedEventHandler(Survey_Timer_Event);
            }
            double mSecPeriod = Tools.ConversionStringtoDouble(tbHMRLoop.Text);
            if (mSecPeriod == Double.NaN)
            {
                mSecPeriod = 1000;
                return;
            }
            if (mSecPeriod <= 100)
            {
                mSecPeriod = 100;
                tbHMRLoop.Text = "100";
            }
            Surveytimer.Interval = mSecPeriod;
            Surveytimer.AutoReset = true;
            LoopProcessCounter = 0;
            Surveytimer.Start();
            myMainProg.myRtbTermMessageLF("-INFO: Survey Sample Timer Loop is now active");
            myGlobalBase.HMR_HideTextDisplay = true;
        }
        #endregion

        #region //================================================================Survey_Timer_Stop
        //==========================================================
        // Purpose  : This code monitor the received HMR2300 message 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void Survey_Timer_Stop()
        {
            isSurveyTimerPauseEnabled = false;
            btnPause.Text = "Pause";
            btnStop.Enabled = false;
            btnPause.Enabled = false;
            btnStart.Enabled = true;
            tbHMRLoop.Enabled = true;
            cbAutoLoop.Enabled = true;

            //------------------------------------
            if (Surveytimer == null)
                return;
            Surveytimer.AutoReset = false;
            Surveytimer.Elapsed -= Survey_Timer_Event;  // Unsubscribe
            Surveytimer = null;
            //------------------------------------
            dgvSurveyViewer.ResumeLayout(false);
        }
        #endregion

        #region //================================================================Survey_Timer_Event
        //==========================================================
        // Purpose  : This code monitor the received device message 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void Survey_Timer_Event(object sender, System.Timers.ElapsedEventArgs e)
        {
            int DebugDevice = 0;
            if (isSurveyTimerPauseEnabled == true)
                return;
            dgvSurveyViewer.ResumeLayout(false);
            //----Check if any is still busy collecting data or stuck waiting for received data
            if (Survey_DGV_isAllDataCollected(ref DebugDevice) == false)
            {
                myMainProg.myRtbTermMessageLF("+WARN: Data collection is not completed, increase sample period or check device and laptop.");
                myMainProg.myRtbTermMessageLF("+WARN: Device currently busy or hanging: " + DeviceName[DebugDevice]);
            }
            else
            {
                Survey_AllDataCollected_Update_DGV();
            }
            if ((cbAppendFile.Checked == true) & (LoopProcessCounter >= 1))     // Append file to data before next capture, skipped 1st loop since there no data at the start. 
            {
                Survey_AppendDataToFile();
            }
            if (cbAutoLoop.Checked == false)
            {
                Survey_Timer_Stop();
                return;
            }
            dgvSurveyViewer.SuspendLayout();        // DGV is suspended to speed up operation. 
            Survey_ProcessRowNow();
            LoopProcessCounter++;
        }
        #endregion

        #region //================================================================Survey_ProcessRowNow
        //==========================================================
        // Purpose  : This code monitor the received HMR2300 message 
        // Input    :  
        // Output   : 
        // Status   : Added invoke as necessary to fix bug. 
        //==========================================================
        public delegate void Survey_ProcessRowNowDelegate();
        private void Survey_ProcessRowNow()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Survey_ProcessRowNowDelegate(Survey_ProcessRowNow));
                return;
            }

            Svy_AddNewRow();
            //-------------------------------------Implement Start Data
            //--------------------------------------------------------------BGDRILLING
            if (myGlobalBase.is_BGDRILLING_Device_Activated == true)
            {
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].SurveyProcessState = 1;
                myUSBVCOMComm.VCOMArray_Message_Send("*99P\r", (int)eUSBDeviceType.BGDRILLING);
            }
            //--------------------------------------------------------------HMR2300
            if (myGlobalBase.is_SvyHMRSerial_Server_Connected == true)
            {
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].SurveyProcessState = 1;
                myUSBVCOMComm.VCOMArray_Message_Send("*99P\r", (int)eUSBDeviceType.HMR2300);
            }
            //--------------------------------------------------------------ADIS
            if (myGlobalBase.is_SvyADISSerial_Server_Connected == true)
            {
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].SurveyProcessState = 1;
            }
            //--------------------------------------------------------------FGIIA
            if (myGlobalBase.is_SvyFGIISerial_Server_Connected == true)
            {
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].SurveyProcessState = 1;
                //myUSBVCOMComm.VCOMArray_Message_Send("FG_POLLREAD()", (int)eUSBDeviceType.FGII_A);
                myUSBVCOMComm.VCOMArray_Message_Send("FGPR()", (int)eUSBDeviceType.FGII_A);     // Modified command 29-Aug-2016 for better response. 
            }
            if (isDebugNoSensorConfirmedOkay == false)      // No need to send repeat message under debug mode. 
            {
                if ((myGlobalBase.is_SvyHMRSerial_Server_Connected == false) &
                    (myGlobalBase.is_SvyADISSerial_Server_Connected == false) &
                    (myGlobalBase.is_SvyFGIISerial_Server_Connected == false))
                {
                    myMainProg.myRtbTermMessageLF("#ERR: No Data due to all inactive VCOM Ports, The survey operation is ceased");
                    Survey_Timer_Stop();
                    dgvSurveyViewer.ResumeLayout(false);
                }
            }
            //Thread.Sleep(10);
        }
        #endregion

        #region //================================================================Svy_AddNewRow
        //==========================================================
        // Purpose  : Add new row within survey report. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        public void Svy_AddNewRow()
        {
            //--------------------------------------TimeStamp pLacement.
            DateTime dt = DateTime.Now;
            long milliseconds = (DateTime.Now.Ticks - dtNow.Ticks) / TimeSpan.TicksPerMillisecond;
            string sTimeStampDate = string.Format("{0:dd|MM|yy}", dt);
            string sTimeStampTime = string.Format("{0:HH:mm:ss}", dt);
            lTimeStampTick = milliseconds;
            double dTimeStampTickUCT = Tools.DateTimeToUnixTimestamp(dt);
            //--------------------------------------Format new Row
            myRow = null;
            myRow = new object[DGVtable.Columns.Count];
            myRow[0] = sTimeStampDate;
            myRow[1] = sTimeStampTime;
            myRow[2] = lTimeStampTick.ToString();
            myRow[3] = dTimeStampTickUCT;
            DGVtable.Rows.Add(myRow);
            //-------------------------------------------Insert Row Number on Row Header. 
            DataGridViewRow row = dgvSurveyViewer.Rows[DGVtable.Rows.Count - 1];
            row.HeaderCell.Value = (row.Index + 1).ToString();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Setup for CVS file.
        //#####################################################################################################

        #region//================================================================Svy_InitForSurveyCVS_RecievedData
        private void Svy_InitForSurveyCVS_RecievedData()
        {
            //----------------------------------------
            myGlobalBase.IsImportRawDataEnable = false;     // Make sure the ImportCVS is disabled. 
            myGlobalBase.IsImportRawDataActivate = false;
            //-------------------------------------- Activate protocol within USB_FTDI_Comm for threaded reception of RX Data. 
            RecievedDataForFiling = "";
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### HMR2300 Support
        //#####################################################################################################

        #region//================================================================Svy_SurveyCVS_HMR_RecievedData
        public delegate void Svy_SurveyCVS_HMR_RecievedDataDelegate(string DataFrame);
        //Solution: http://www.dreamincode.net/forums/topic/188209-cross-thread-calls-made-easy/ 
        public void Svy_SurveyCVS_HMR_RecievedData(string DataFrame)
        {
            // Check if we need to call BeginInvoke.
            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false.
                this.BeginInvoke(new Svy_SurveyCVS_HMR_RecievedDataDelegate(Svy_SurveyCVS_HMR_RecievedData),
                                             new object[] { DataFrame });
                return;
            }
            //int DebugDevice = 0;
            if (dgvSurveyViewer.ColumnCount == 0)           // Error in Datagridview, this happen when clearing the DGV. 
            {
                return;
            }
            //----------------------------------------Break up string 
            HRM2300.Mag_Data myMagData = myHRM2300.HRM2300_MagDataRef();
            myHRM2300.HMR_FrametoMagData(DataFrame);
            myHRM2300.HMR2300_Math_CalculateAll();
            //---------------------------------------Calibration (assumed Simple method, for now, (need smarter solution for this for Method1/2)
            List<double> dHMRmag = new List<double>();
            dHMRmag.Add(myMagData.MagCal.X);
            dHMRmag = myCalibration.Cal_ProcessRow(0, dHMRmag);
            DGVtable.Rows[DGVtable.Rows.Count - 1].SetField(4, dHMRmag[0]);
            dHMRmag.Clear();
            dHMRmag.Add(myMagData.MagCal.Y);
            dHMRmag = myCalibration.Cal_ProcessRow(1, dHMRmag);
            DGVtable.Rows[DGVtable.Rows.Count - 1].SetField(5, dHMRmag[0]);
            dHMRmag.Clear();
            dHMRmag.Add(myMagData.MagCal.Z);
            dHMRmag = myCalibration.Cal_ProcessRow(2, dHMRmag);
            DGVtable.Rows[DGVtable.Rows.Count - 1].SetField(6, dHMRmag[0]);
            dHMRmag.Clear();

            //----------------------------------------------Update Scope Chart. ### Need to sort timestamp tick (common). 
            int[] selectRow = new int[] { 4, 5, 6 };
            Survey_UpdateScopeChart(selectRow);
            //----------------------------------------------Update datagridview
            switch (myMagData.DataFormat)
            {
                case (0):   // Raw Data, no decimal points
                    {
                        dgvSurveyViewer.Columns[3].DefaultCellStyle.Format = "N0";
                        dgvSurveyViewer.Columns[4].DefaultCellStyle.Format = "N0";
                        dgvSurveyViewer.Columns[5].DefaultCellStyle.Format = "N0";
                        break;
                    }
                case (1):   //uT
                    {
                        dgvSurveyViewer.Columns[3].DefaultCellStyle.Format = "N3";
                        dgvSurveyViewer.Columns[4].DefaultCellStyle.Format = "N3";
                        dgvSurveyViewer.Columns[5].DefaultCellStyle.Format = "N3";
                        break;
                    }
                case (2):   //mG
                    {
                        dgvSurveyViewer.Columns[3].DefaultCellStyle.Format = "N2";
                        dgvSurveyViewer.Columns[4].DefaultCellStyle.Format = "N2";
                        dgvSurveyViewer.Columns[5].DefaultCellStyle.Format = "N2";
                        break;
                    }
            }
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].SurveyProcessState = 2;
            //if (Survey_DGV_isAllDataCollected(ref DebugDevice) ==true)
            //{
            //    Survey_AllDataCollected_Update_DGV();
            //}

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### ADIS Support
        //#####################################################################################################

        //#####################################################################################################
        //###################################################################################### DataLog Support
        //#####################################################################################################

        #region//================================================================SurveyCVS_DataLog_RecievedData
        // Important when '$E\n' i detected it goes to this routine for processing.
        // When #@#E is detected then it finish !
        // The ASCII can potentially goes up to 300 ASCII per message frame
        // Frame is define as start with $X where X is any char and $E is end frame.
        // Separator is ';' or ','.  
        // 1st frame is header for column definition
        // 2nd frame is format type which is used to decode hex column to readable data column.
        // When completed, it save into two CVS filename, RAW (hex) and later Converted (int). 
        // The conversion and DGV table take place after the saving RAW data. 
        public delegate void SurveyCVS_DataLog_RecievedDataDelegate(string RecievedData);
        //Solution: http://www.dreamincode.net/forums/topic/188209-cross-thread-calls-made-easy/ 
        public void SurveyCVS_DataLog_RecievedData(string RecievedData)
        {
            char[] delimiterChars = { ',', ';' };
            // Check if we need to call BeginInvoke.
            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false.
                this.BeginInvoke(new SurveyCVS_DataLog_RecievedDataDelegate(SurveyCVS_DataLog_RecievedData), new object[] { RecievedData });
                return;
            }
            RecievedData.Replace("/0", string.Empty);      // Remove null elements. 
            //-----------------------------------------------------------------------------------------------------------------------------Header
            if ((RecievedData.StartsWith("~LB(", StringComparison.Ordinal) == true))     //Header Section
            {
                RecievedData = RecievedData.Replace("~LB(", "");
                RecievedData = RecievedData.Replace(")", "");
                //-------------------------------------Clear Header Frame
                Survey_ClearAndUpdateDataGridView(RecievedData,true);
                //-------------------------------------Clear Format Frame
                if (FormatColumn == null)
                    FormatColumn = new List<string>();
                else
                    FormatColumn.Clear();
            }
            //-----------------------------------------------------------------------------------------------------------------------------Format
            if ((RecievedData.StartsWith("~LC(", StringComparison.Ordinal) == true))     //Format Section
            {
                RecievedData = RecievedData.Replace("~LC(", "");
                RecievedData = RecievedData.Replace(")", "");
                string[] parth = RecievedData.Split(delimiterChars);
                FormatColumn = new List<string>(parth);
            }
            //-----------------------------------------------------------------------------------------------------------------------------Data1
            if ((RecievedData.StartsWith("~LD(", StringComparison.Ordinal) == true))     //Data1 Section
            {
                RecievedData = RecievedData.Replace("~LD(", "");
                RecievedData = RecievedData.Replace(")", "");
                string[] sData = RecievedData.Split(delimiterChars);
                for (int y = 1; y < sData.Length; y++)
                {
                    switch (FormatColumn[y])
                    {
                        case ("0w"):        // INT16 only
                            {
                                //sData[y] = Tools.HexStringtoInt16(sData[y]).ToString();
                                break;
                            }
                        case ("0q"):        // uINT16 only
                            {
                                //sData[y] = Tools.HexStringtoUInt16(sData[y]).ToString();
                                break;
                            }
                        case ("0u"):        // convert hex to unsigned integer. 
                            {
                                //sData[y] = Tools.HexStringtoUInt32X(sData[y]).ToString();
                                break;
                            }
                        case ("0i"):        // convert hex to  integer
                            {
                                //sData[y] = Tools.HexStringtoInt32X(sData[y]).ToString();
                                break;
                            }
                        case ("0x"):        // Hex (leave unchanged, except to add 0x if missing)
                            {
                                //if (sData[y].Contains("0x") == false)
                                //{
                                //    sData[y] = sData[y].Insert(0, "0x");
                                //}
                                break;
                            }
                        case ("0y"):        // date
                            {
                                break;
                            }
                        case ("0z"):        // time
                            {
                                break;
                            }
                        default:
                            break;

                    }
                }
                //------------------------------------------------------------------------
                RecievedData = string.Join(",", sData);
                DGVtable.Rows.Add(RecievedData.Split(delimiterChars));
            }
            //-----------------------------------------------------------------------------------------------------------------------------Data2
            if ((RecievedData.StartsWith("~LE(", StringComparison.Ordinal) == true))     //Data2 Section
            {
                RecievedData = RecievedData.Replace("~LE(", "");
                RecievedData = RecievedData.Replace(")", "");
                string[] sData = RecievedData.Split(delimiterChars);
                for (int y = 1; y < sData.Length; y++)
                {
                    switch (FormatColumn[y])
                    {
                        case ("0w"):        // INT16 only
                            {
                                //sData[y] = Tools.HexStringtoInt16(sData[y]).ToString();
                                break;
                            }
                        case ("0q"):        // uINT16 only
                            {
                                //sData[y] = Tools.HexStringtoUInt16(sData[y]).ToString();
                                break;
                            }
                        case ("0u"):        // convert hex to unsigned integer. 
                            {
                                //sData[y] = Tools.HexStringtoUInt32X(sData[y]).ToString();
                                break;
                            }
                        case ("0i"):        // convert hex to  integer
                            {
                                //sData[y] = Tools.HexStringtoInt32X(sData[y]).ToString();
                                break;
                            }
                        case ("0x"):        // Hex (leave unchanged, except to add 0x if missing)
                            {
                                //if (sData[y].Contains("0x") == false)
                                //{
                                //    sData[y] = sData[y].Insert(0, "0x");
                                //}
                                break;
                            }
                        case ("0y"):        // date
                            {
                                break;
                            }
                        case ("0z"):        // time
                            {
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                //------------------------------------------------------------------------
                RecievedData = string.Join(",", sData);
                DGVtable.Rows.Add(RecievedData.Split(delimiterChars));
            }
            //dgvSurveyViewer.DataSource = null;      // clear table 1st. 
            //dgvSurveyViewer.DataSource = DGVtable;

        }
        #endregion



        //#####################################################################################################
        //###################################################################################### FluxGateII Support
        //#####################################################################################################

        #region//================================================================Svy_SurveyCVS_FGII_RecievedData
        public delegate void Svy_SurveyCVS_FGII_RecievedDataDelegate(string DataFrame);
        //Solution: http://www.dreamincode.net/forums/topic/188209-cross-thread-calls-made-easy/ 
        public void Svy_SurveyCVS_FGII_RecievedData(string DataFrame)
        {
            // Check if we need to call BeginInvoke.
            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false.
                this.BeginInvoke(new Svy_SurveyCVS_FGII_RecievedDataDelegate(Svy_SurveyCVS_FGII_RecievedData),
                                             new object[] { DataFrame });
                return;
            }
            //int DebugDevice = 0;
            //----------------------------------------Break up string 
            FluxGateII.Mag_DataFG myMagData = myFGIIA.FGII_MagDataRef();
            myFGIIA.FGII_FrametoMagData(DataFrame);
            myFGIIA.FGII_Math_CalculateAll();
            //---------------------------------------Calibration (assumed Simple method, for now, (need smarter solution for this for Method1/2)
            List<double> dFGIImag = new List<double>();
            dFGIImag.Add(myMagData.temp);                                       // Temp Readout, add this first. 
            dFGIImag = myCalibration.Cal_ProcessRow(6, dFGIImag);
            DGVtable.Rows[DGVtable.Rows.Count - 1].SetField(10, dFGIImag[0]);
            dFGIImag.Clear();
            dFGIImag.Add(myMagData.MagCal.X);                                   // Mag X
            dFGIImag = myCalibration.Cal_ProcessRow(3, dFGIImag);
            DGVtable.Rows[DGVtable.Rows.Count - 1].SetField(7, dFGIImag[0]);
            dFGIImag.Clear();
            dFGIImag.Add(myMagData.MagCal.Y);                                   // Mag Y
            dFGIImag = myCalibration.Cal_ProcessRow(4, dFGIImag);
            DGVtable.Rows[DGVtable.Rows.Count - 1].SetField(8, dFGIImag[0]);
            dFGIImag.Clear();
            dFGIImag.Add(myMagData.MagCal.Z);                                   // Mag Z
            dFGIImag = myCalibration.Cal_ProcessRow(5, dFGIImag);
            DGVtable.Rows[DGVtable.Rows.Count - 1].SetField(9, dFGIImag[0]);
            dFGIImag.Clear();

            int[] selectRow = new int[] { 7, 8, 9, 10 };
            //----------------------------------------------Update Scope Chart. 
            Survey_UpdateScopeChart(selectRow);
            //----------------------------------------------Update datagridview
            dgvSurveyViewer.Columns[10].DefaultCellStyle.Format = "N1";     // Alway 24.4 format for degC temp. 
            switch (myMagData.DataFormat)
            {
                case (0):   // Raw Data, no decimal points
                    {
                        dgvSurveyViewer.Columns[7].DefaultCellStyle.Format = "N0";
                        dgvSurveyViewer.Columns[8].DefaultCellStyle.Format = "N0";
                        dgvSurveyViewer.Columns[9].DefaultCellStyle.Format = "N0";
                        break;
                    }
                case (1):   //uT
                    {
                        dgvSurveyViewer.Columns[7].DefaultCellStyle.Format = "N4";
                        dgvSurveyViewer.Columns[8].DefaultCellStyle.Format = "N4";
                        dgvSurveyViewer.Columns[9].DefaultCellStyle.Format = "N4";
                        break;
                    }
                case (2):   //mG
                    {
                        dgvSurveyViewer.Columns[7].DefaultCellStyle.Format = "N3";
                        dgvSurveyViewer.Columns[8].DefaultCellStyle.Format = "N3";
                        dgvSurveyViewer.Columns[9].DefaultCellStyle.Format = "N3";
                        break;
                    }
            }
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].SurveyProcessState = 2;
            //if (Survey_DGV_isAllDataCollected(ref DebugDevice) == true)
            //{
            //    Survey_AllDataCollected_Update_DGV();
            //}
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Post Data Collection Manager
        //#####################################################################################################

        #region//================================================================Survey_DGV_isAllDataCollected
        //==========================================================
        // Purpose  : Check all device for completed data collection service. 
        // Input    :  
        // Output   : True = Data collection is completed, else not finish collection, (until next update) 
        //          : Ref DetectedDeviceBusy specified which channel is busy for debug purpose. 
        // Status   :
        //==========================================================
        // 
        bool Survey_DGV_isAllDataCollected(ref int DetectedDeviceBusy)
        {
            for (int i = 0; i < myUSBVCOMComm.VCOMserialPortArray.Count; i++)
            {
                if (myGlobalBase.myUSBVCOMArray[i].SurveyProcessState != 0)           // Is channel enabled for data collection, 0 = not enabled. 
                {
                    if (myGlobalBase.myUSBVCOMArray[i].SurveyProcessState == 1)       // Detected that Data collection not finish yet. 
                    {
                        DetectedDeviceBusy = i;                                     // Specified which channel is busy
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region//================================================================Survey_AllDataCollected_Update_DGV
        public delegate void Survey_AllDataCollected_Update_DGVDelegate();
        public void Survey_AllDataCollected_Update_DGV()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Survey_AllDataCollected_Update_DGVDelegate(Survey_AllDataCollected_Update_DGV));
                return;
            }
            if (dgvSurveyViewer==null)
            {
                Survey_AppendFile_with_Exception_Error("+WARN: Survey_AllDataCollected_Update_DGV() has null object detected, may ignore this message");
                return;
            }
            try     // This avoid JIT pop-up message when occurred due to null object exception, possibly in dgvSurveyViewer
            {
                dgvSurveyViewer.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                dgvSurveyViewer.PerformLayout();
                //---------------------------------------------Auto scroll management.
                if (cbSuspendScroll.Checked == false)
                {
                    if (dgvSurveyViewer.RowCount >= 1)
                    {
                        dgvSurveyViewer.FirstDisplayedScrollingRowIndex = dgvSurveyViewer.RowCount - 1;     // view to last row. 
                    }
                }
                dgvSurveyViewer.Invalidate();
            }
            catch
            {
                Survey_AppendFile_with_Exception_Error("+WARN: Survey_AllDataCollected_Update_DGV() has exception occurance, may ignore this message");
            }
        }
        #endregion



        //#####################################################################################################
        //###################################################################################### Setup DataGridView
        //#####################################################################################################

        #region //================================================================Survey_ClearAndUpdateDataGridView
        // 27/8/16 Added Temp from FGII. 
        private void Survey_ClearAndUpdateDataGridView(string sHeaderFrame, bool isDataLog)
        {
            // This include HMR2300 and ADIS device for now.
            DGVtable = null;
            DGVtable = new DataTable();
            //-------------------------------------
            if (HeaderColumn == null)                            // if HeaderColumn is null then init. 
                HeaderColumn = new List<string>();
            else
                HeaderColumn.Clear();
            if (sHeaderFrame == "")
                return;
            char[] delimiterChars = { ',', ';' };
            string[] parth = sHeaderFrame.Split(delimiterChars);
            HeaderColumn = new List<string>(parth);
            //---------------------------------------------Add header to Column.

            for (int i = 0; i < parth.Length; i++)
            {
                if (i >= 3)
                {
                    if (isDataLog==true)
                        DGVtable.Columns.Add(parth[i]);
                    else
                        DGVtable.Columns.Add(parth[i], typeof(double));
                }
                else
                {
                    switch (i)
                    {
                        case 2:  //Window ticks
                            {
                                if (isDataLog == true)
                                    DGVtable.Columns.Add(parth[i]);
                                else
                                    DGVtable.Columns.Add(parth[i], typeof(UInt64));
                                break;
                            }
                        case 3:  //UDT (unix Date Time)
                            {
                                if (isDataLog == true)
                                    DGVtable.Columns.Add(parth[i], typeof(int));
                                else
                                    DGVtable.Columns.Add(parth[i], typeof(double));
                                break;
                            }
                        default:
                            {
                                DGVtable.Columns.Add(parth[i]);
                                break;
                            }
                    }
                }
            }
            //--------------------------------------------Add format frame (Cannot be done since the column are double format, not string. 
            // DGVtable.Rows.Add(FormatFrame.Split(';'));
            //---------------------------------------------Link Table to DGV
            dgvSurveyViewer.DataSource = DGVtable;
            //---------------------------------------------Insert Row number on Row header for reference purpose.
            foreach (DataGridViewRow row in dgvSurveyViewer.Rows)
            {
                row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
            }
            //-----------------------------------------------------------Speed up process, various measure to try out when data row get more than 1000+
            foreach (DataGridViewColumn c in dgvSurveyViewer.Columns)
            {
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            dgvSurveyViewer.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            dgvSurveyViewer.VirtualMode = true;
            //--------------------------------------------Tidy Up DGV.
            //dgvSurveyViewer.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);    // Resize the master DataGridView columns to fit the newly loaded data.
            dgvSurveyViewer.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvSurveyViewer.RowHeadersWidth = 60;
        }
        #endregion



        //#####################################################################################################
        //###################################################################################### Received Data (Logger CVS)
        //#####################################################################################################

        #region//================================================================SurveyLoggerCVS_Start
        public delegate void SurveyLoggerCVS_StartDelegate();
        //Solution: http://www.dreamincode.net/forums/topic/188209-cross-thread-calls-made-easy/ 
        public void SurveyLoggerCVS_Start()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new SurveyLoggerCVS_StartDelegate(SurveyLoggerCVS_Start));
                return;
            }
            Svy_InitForSurveyCVS_RecievedData();
            //-------------------------------------Data Table and DataGrid setup
            dgvSurveyViewer.DataSource = null;                  // clear table 1st. 
            HeaderFrameTool = null;                                  // Reset to null.
            DGVtable = null;
            DGVtable = new DataTable();
            dgvSurveyViewer.DataSource = DGVtable;
            //-------------------------------------
            if (HeaderColumn == null)                            // if HeaderColumn is null then init. 
                HeaderColumn = new List<string>();
            else
                HeaderColumn.Clear();
            //--------------------------------------
            if (FormatColumn == null)                            // if FormatColumn is null then init. 
                FormatColumn = new List<string>();
            else
                FormatColumn.Clear();
            //--------------------------------------Send Message to start download.
            Console.Beep(1000, 20);
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### File Manager
        //#####################################################################################################

        #region //================================================================Survey_AppendDataToFile
        private void Survey_AppendDataToFile()
        {
            try
            {
                dgvSurveyViewer.MultiSelect = false;
                dgvSurveyViewer.Rows[dgvSurveyViewer.RowCount - 1].Selected = true;
                dgvSurveyViewer.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
                string text = dgvSurveyViewer.GetClipboardContent().GetText(TextDataFormat.CommaSeparatedValue);
                text = text + "\r\n";
                System.IO.File.AppendAllText(sAppendFilename, text);
                dgvSurveyViewer.MultiSelect = true;
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("+WARN: Survey_AppendDataToFile() has exception occurance, may ignore this message");
            }

        }
        #endregion

        #region //================================================================Survey_AppendHeaderToFile
        private void Survey_AppendHeaderToFile()
        {
            try
            {
                string sHF = HeaderFrameTool.Replace(';', ',') + "\r\n";
                string sTimeStampNow = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
                sAppendFilename = "_AppendData_" + sTimeStampNow + ".csv";
                sAppendFilename = System.IO.Path.Combine(sDefaultFolder, sAppendFilename);
                System.IO.File.WriteAllText(sAppendFilename, sHF);
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("+WARN: Survey_AppendHeaderToFile() has exception occurance, may ignore this message");
            }
        }
        #endregion

        #region//================================================================Survey_AppendFile_with_Exception_Error
        void Survey_AppendFile_with_Exception_Error(string text)
        {
            try
            {
                myMainProg.myRtbTermMessageLF(text);
                if (sAppendFilename != "")
                {
                    text = text + "\r\n";
                    System.IO.File.AppendAllText(sAppendFilename, text);
                }
            }
            catch { }
        }
        #endregion

        #region //================================================================btnProjectFolder_Click
        private void btnProjectFolder_Click(object sender, EventArgs e)
        {
            Survey_ProjectFolder();
        }
        #endregion

        #region //================================================================Survey_ProjectFolder
        private bool Survey_ProjectFolder()
        {
            try
            {
                //----------------------------------------------------------
                folderBrowserDialog1.SelectedPath = sDefaultFolder;
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    sDefaultFolder = folderBrowserDialog1.SelectedPath;
                    txtFolderName.Text = folderBrowserDialog1.SelectedPath;
                    return true;
                }
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("###ERR: Folder Dialogue Reported an Error");
            }
            return false;

        }
        #endregion

        #region //================================================================BtnOpenFolder_Click
        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string myPath = sDefaultFolder;
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = myPath;
                prc.Start();


            }
            catch
            {
                myMainProg.myRtbTermMessageLF("#ERR: Unable to Open Folder: myPath");
                txtFolderName.Text = "";
            }


        }
        #endregion

        #region //================================================================Survey_SaveAllData
        private void btnSaveAllData_Click(object sender, EventArgs e)
        {
            Survey_SaveAllData();
        }
        #endregion

        #region //================================================================btnLoadAllData_Click
        private void btnLoadAllData_Click(object sender, EventArgs e)
        {
            if (SurveyClearAllData() == false)                       // Check if they really want to clear data 1st before loading file.
                return;
            if (Survey_LoadDataFile() == false)                      // Load file
                return;
            //====================================1st frame is header frame which reset the datatable and add columns. 
            sFrame[0] = sFrame[0].Replace("\n", string.Empty);
            sFrame[0] = sFrame[0].Replace("\r", string.Empty);
            sFrame[0] = sFrame[0].Replace(";", ",");
            Survey_ClearAndUpdateDataGridView(sFrame[0],false);
            //====================================Rest of frame goes into datatable which is linked to datagridview via datasource. 

            int ss1 = 0;
            int ss2 = 0;
            for (int i = 1; i < sFrame.Count; i++)
            {
                string sFramex = sFrame[i];
                if (sFramex != "")
                {
                    // UTC data is string with /" and /" pre and post fix, this code convert , and " to space so it be removed via replace. 
                    for (int j = 0; j < sFramex.Length; j++)
                    {
                        if ((ss1 == 0) & (sFramex[j] == '"'))
                        {
                            ss1 = j;
                        }
                        if ((ss1 != 0) & (sFramex[j] == '"'))
                        {
                            ss2 = j;
                        }
                    }
                    for (int j = ss1; j <= ss2; j++)
                    {
                        if (sFramex[j] == '"')
                            sFramex = Tools.StringReplaceAt(sFramex, j, 1, " ");
                        if (sFramex[j] == ',')
                            sFramex = Tools.StringReplaceAt(sFramex, j, 1, " ");
                    }
                    // Remove unneeded text before split take place. 
                    sFramex = sFramex.Replace("\n", string.Empty);
                    sFramex = sFramex.Replace("\r", string.Empty);
                    sFramex = sFramex.Replace(" ", string.Empty);
                    sFramex = sFramex.Replace("\"", string.Empty);
                    sFramex = sFramex.Replace(";", ",");
                    string[] field = sFramex.Split(',');
                    // Add data field to row, in this way. 
                    DataRow row = DGVtable.NewRow();
                    for (int j = 0; j < field.Length; j++)
                    {
                        try
                        {
                            if ((field[j] == "0.0") || (field[j] == "0.00") || (field[j] == "0.000") || (field[j] == "0"))       // This may help the charting. 
                                field[j] = string.Empty;
                            else if (field[j] != "") // Skip blank data. 
                                row[j] = field[j];
                        }
                        catch { }
                    }
                    DGVtable.Rows.Add(row);
                    //--------------Add number to row header. 
                    DataGridViewRow rowh = dgvSurveyViewer.Rows[DGVtable.Rows.Count - 1];
                    rowh.HeaderCell.Value = (i).ToString();
                }
            }
        }
        #endregion

        #region //================================================================SurveyCVS_ReadRawFile
        bool Survey_LoadDataFile()
        {
            string sFilename;
            OpenFileDialog ofdReadSurvey = null;
            myMainProg.myRtbTermMessageLF("-INFO: Loading Survey Data File");
            if (sFrame == null)                            // if Frame is null then init. 
            {
                sFrame = new List<string>();
            }
            else
            {
                sFrame.Clear();
            }

            try
            {
                ofdReadSurvey = new OpenFileDialog();
                ofdReadSurvey.Filter = "csv files (*.csv)|*.csv";
                ofdReadSurvey.FileName = sDefaultFolder;
                ofdReadSurvey.Title = "Load Survey CVS File";
                DialogResult dr = ofdReadSurvey.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    sFilename = ofdReadSurvey.FileName;

                }
                else
                {
                    myMainProg.myRtbTermMessageLF("#ERR: Unable to load file");
                    return false;
                }
                using (StreamReader reader = File.OpenText(ofdReadSurvey.FileName))
                {
                    Encoding sourceEndocing = reader.CurrentEncoding;
                    string file = reader.ReadToEnd();
                    string[] part = file.Split('\n');
                    sFrame = new List<string>(part);
                }
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("#ERR: Unable to load file");
                return false;
            }
            return true;
        }
        #endregion

        #region //================================================================Survey_SaveAllData
        private void Survey_SaveAllData()
        {
            if (dgvSurveyViewer.RowCount == 0)
            {
                myMainProg.myRtbTermMessageLF("#E: Saving Survey Data to CVS files: Rejected Operation, No Data to save!");
                return;
            }
            string sTimeStampNow = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
            sFilename = "_Data_" + sTimeStampNow + ".csv";
            sFilename = System.IO.Path.Combine(sDefaultFolder, sFilename);

            IDataObject objectSave = Clipboard.GetDataObject();                 // Save the current state of the clipboard so we can restore it after we are done
            dgvSurveyViewer.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;        // Choose whether to write header. Use EnableWithoutHeaderText instead to omit header.

            dgvSurveyViewer.SelectAll();                                        // Select all the cells

            Clipboard.SetDataObject(dgvSurveyViewer.GetClipboardContent());     // Copy (set clipboard)

            File.WriteAllText(sFilename, Clipboard.GetText(TextDataFormat.CommaSeparatedValue));     // Paste (get the clipboard and serialize it to a file)

            // Restore the current state of the clipboard so the effect is seamless
            if (objectSave != null) // If we try to set the Clipboard to an object that is null, it will throw...
            {
                Clipboard.SetDataObject(objectSave);
            }
        }
        #endregion

        #region //================================================================cbSelectDrive_SelectedIndexChanged
        private void cbSelectDrive_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            if (myBGMasterSetup == null)
                return;
            string sFoldername = myBGMasterSetup.sFile_JobProjectAll;               // J://SurveyProject//JobName
            try
            {
                sFoldername = sFoldername.Substring(2);         // Remove 2 char at the start. 
                sFoldername = cbSelectDrive.SelectedItem + sFoldername;
                //---------------------------------------------------------------------Update Global. 
                myBGMasterSetup.sFile_DriveAll = (string)cbSelectDrive.SelectedItem;                        // J:
                myBGMasterSetup.sFile_TopFolderProjectAll = myBGMasterSetup.sFile_DriveAll + "\\" + myBGMasterSetup.sFilenameDefaultTopProject;
                myBGMasterSetup.sFile_JobProjectAll = sFoldername;
                txtFolderName.Text = myBGMasterSetup.sFile_JobProjectAll;
            }
            catch  { }
            */
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### RiscyScope & Chart Interface
        //#####################################################################################################

        #region//================================================================Survey_UpdateScopeChart
        // selectRow, list of selected row related to the translated data to be sent to scope channel(s)
        private void Survey_UpdateScopeChart(int[] selectRow)
        {
            //--------------------------------------- CH0 Chart Update
            if (ChartSelectedCh[0] != -1)
            {
                for (int i = 0; i < selectRow.Length; i++)
                {
                    if (ChartSelectedCh[0] == selectRow[i])
                    {
                        float newValue = Convert.ToSingle(DGVtable.Rows[DGVtable.Rows.Count - 1][ChartSelectedCh[0]]);
                        myRiscyScope.AppendCh0Class(new cPoint(lTimeStampTick, newValue));
                        break;
                    }
                }
            }
            //--------------------------------------- CH1 Chart Update
            if (ChartSelectedCh[1] != -1)
            {
                for (int i = 0; i < selectRow.Length; i++)
                {
                    if (ChartSelectedCh[1] == selectRow[i])
                    {
                        float newValue = Convert.ToSingle(DGVtable.Rows[DGVtable.Rows.Count - 1][ChartSelectedCh[1]]);
                        myRiscyScope.AppendCh1Class(new cPoint(lTimeStampTick, newValue));
                        break;
                    }
                }
            }
            //--------------------------------------- CH2 Chart Update
            if (ChartSelectedCh[2] != -1)
            {
                for (int i = 0; i < selectRow.Length; i++)
                {
                    if (ChartSelectedCh[2] == selectRow[i])
                    {
                        float newValue = Convert.ToSingle(DGVtable.Rows[DGVtable.Rows.Count - 1][ChartSelectedCh[2]]);
                        myRiscyScope.AppendCh2Class(new cPoint(lTimeStampTick, newValue));
                        break;
                    }
                }
            }
            //--------------------------------------- CH3 Chart Update
            if (ChartSelectedCh[3] != -1)
            {
                for (int i = 0; i < selectRow.Length; i++)
                {
                    if (ChartSelectedCh[3] == selectRow[i])
                    {
                        float newValue = Convert.ToSingle(DGVtable.Rows[DGVtable.Rows.Count - 1][ChartSelectedCh[3]]);
                        myRiscyScope.AppendCh3Class(new cPoint(lTimeStampTick, newValue));
                        break;
                    }
                }
            }
        }

        #endregion

        #region //================================================================RiscyScopeChart_Init

        private void RiscyScopeChart_Init()
        {

            ChartSelectedCh[0] = -1;
            ChartSelectedCh[1] = -1;
            ChartSelectedCh[2] = -1;
            ChartSelectedCh[3] = -1;

            ChartSelectedColumn = -1;
            ChartSelectedStartRow = -1;
            ChartSelectedEndRow = -1;
        }
        #endregion

        #region //================================================================btnScopeTest_Click
        private void btnScopeTest_Click(object sender, EventArgs e)
        {
            myRiscyScope.Show();
            myRiscyScope.RiscyScope_Show();
        }
        #endregion

        #region //================================================================dgvSurveyViewer_ColumnHeaderMouseClick
        private void dgvSurveyViewer_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    ChartSelectedColumn = e.ColumnIndex;
                    //----------------------------------------------Add context menu
                    System.Windows.Forms.ContextMenu contextMenuX = new System.Windows.Forms.ContextMenu();
                    System.Windows.Forms.MenuItem menuItemx1 = new System.Windows.Forms.MenuItem("Chart CH0");
                    menuItemx1.Click += new EventHandler(ContexMenuCH0);
                    System.Windows.Forms.MenuItem menuItemx2 = new System.Windows.Forms.MenuItem("Chart CH1");
                    menuItemx2.Click += new EventHandler(ContexMenuCH1);
                    System.Windows.Forms.MenuItem menuItemx3 = new System.Windows.Forms.MenuItem("Chart CH2");
                    menuItemx3.Click += new EventHandler(ContexMenuCH2);
                    System.Windows.Forms.MenuItem menuItemx4 = new System.Windows.Forms.MenuItem("Chart CH3");
                    menuItemx4.Click += new EventHandler(ContexMenuCH3);
                    System.Windows.Forms.MenuItem menuItemx5 = new System.Windows.Forms.MenuItem("Clear All");
                    menuItemx5.Click += new EventHandler(ContexMenuClearAllScopeChannel);
                    //System.Windows.Forms.MenuItem menuItemx5 = new System.Windows.Forms.MenuItem("X Axis: RIT");
                    //menuItemx5.Click += new EventHandler(ContexSelectRIT);
                    System.Windows.Forms.MenuItem menuItemx6 = new System.Windows.Forms.MenuItem("NoOfRow:" + dgvSurveyViewer.RowCount.ToString());
                    contextMenuX.MenuItems.Add(menuItemx1);
                    contextMenuX.MenuItems.Add(menuItemx2);
                    contextMenuX.MenuItems.Add(menuItemx3);
                    contextMenuX.MenuItems.Add(menuItemx4);
                    contextMenuX.MenuItems.Add(menuItemx5);
                    contextMenuX.MenuItems.Add(menuItemx6);
                    //----------------------------------------------
                    var relativeMousePosition = dgvSurveyViewer.PointToClient(Cursor.Position);
                    relativeMousePosition.X += 10;
                    relativeMousePosition.Y += 10;
                    contextMenuX.Show(dgvSurveyViewer, relativeMousePosition);
                }
            }
        }
        #endregion

        #region //================================================================ContextMenuSelectedRow()
        private void ContextMenuSelectedRow()
        {
            //----------------------------------------------Row and Selected Row
            ChartStartRow = 0;
            ChartEndRow = dgvSurveyViewer.RowCount;
            if (dgvSurveyViewer.SelectedRows.Count <= 1)        // Not selected region, so treat as full row list. 
            {
                ChartSelectedStartRow = ChartStartRow;
                ChartSelectedEndRow = ChartEndRow;
            }
            else                                                // Selected row region
            {
                int SelectedStart = dgvSurveyViewer.SelectedRows[0].Index;
                int SelectedEnd = dgvSurveyViewer.SelectedRows[dgvSurveyViewer.SelectedRows.Count - 1].Index;
                if (SelectedStart > SelectedEnd)        // backward select, need to flips so timebase work from left to right
                {
                    ChartSelectedStartRow = SelectedEnd;
                    ChartSelectedEndRow = SelectedStart;
                }
                else
                {
                    ChartSelectedStartRow = SelectedStart;
                    ChartSelectedEndRow = SelectedEnd;
                }
            }
        }
        #endregion

        #region //================================================================ContexMenuClearAllScopeChannel()
        private void ContexMenuClearAllScopeChannel(object sender, EventArgs e)
        {
            DataGridViewColumn dgvColumnX;
            for (int i=0; i<4; i++)
            {
                if (ChartSelectedCh[i]>=0)
                {
                    dgvColumnX = dgvSurveyViewer.Columns[ChartSelectedCh[i]];
                    dgvColumnX.HeaderCell.Style.BackColor = System.Drawing.SystemColors.Control;
                    dgvColumnX.HeaderCell.Style.ForeColor = Color.Black;
                    ChartSelectedCh[i] = -1;
                }
            }
            myRiscyScope.RemoveCh0();
            myRiscyScope.RemoveCh1();
            myRiscyScope.RemoveCh2();
            myRiscyScope.RemoveCh3();
        }
        #endregion

        #region //================================================================Open ScopeWindow Event: ContexMenuCH0
        private void ContexMenuCH0(object sender, EventArgs e)
        {
            DataGridViewColumn dgvColumnX;

            if ((ChartSelectedColumn <= 3) & (myGlobalBase.DataLog_isSurveyCVSOpen==false))                                // Sensor Range Only, not clock column. 
                return;
            for (int i = 0; i < 4; i++)
            {
                if (i!=0)
                {
                    if (ChartSelectedCh[i] == ChartSelectedColumn)        // Selected channel, avoid overwrite. 
                        return;
                }
            }
            //-----------------------------------------------Selected Different Column, remove old and add new. 
            if (ChartSelectedCh[0] != ChartSelectedColumn)
            {
                dgvColumnX = dgvSurveyViewer.Columns[ChartSelectedColumn];
                dgvColumnX.HeaderCell.Style.BackColor = System.Drawing.SystemColors.Control;
                dgvColumnX.HeaderCell.Style.ForeColor = Color.Black;
                myRiscyScope.RemoveCh0();
            }
            else
            {
                //-------------------------------------------Selected Column as before, remove it. 
                if (ChartSelectedCh[0] != -1)
                {
                    dgvColumnX = dgvSurveyViewer.Columns[ChartSelectedColumn];
                    dgvColumnX.HeaderCell.Style.BackColor = System.Drawing.SystemColors.Control;
                    dgvColumnX.HeaderCell.Style.ForeColor = Color.Black;
                    ChartSelectedCh[0] = -1;
                    ScopeCH0.Clear();               // Clear Data
                    myRiscyScope.RemoveCh0();
                    return;
                }
            }
            //----------------------------------------------Selected Column. 
            ChartSelectedCh[0] = ChartSelectedColumn;
            //----------------------------------------------Row and Selected Row
            ContextMenuSelectedRow();
            //----------------------------------------------Init Data Points
            if (ScopeCH0==null)
            {
                ScopeCH0 = new List<cPoint>();
            }
            float fdata;
            float xxmSec;
            for (int row = ChartStartRow; row < ChartEndRow; row++)
            {
                float.TryParse(dgvSurveyViewer.Rows[row].Cells[2].Value.ToString(), out xxmSec);
                float.TryParse(dgvSurveyViewer.Rows[row].Cells[ChartSelectedColumn].Value.ToString(), out fdata);
                ScopeCH0.Add(new cPoint(xxmSec, fdata));
            }
            //---------------------------------------------Update Chart
            Color linecolor = Color.Yellow;
            myRiscyScope.StartChartCh0Class(ChartSelectedStartRow, ChartSelectedEndRow, dgvSurveyViewer.Columns[ChartSelectedColumn].HeaderText, ref ScopeCH0, linecolor);
            //---------------------------------------------Update header DGV
            DataGridViewColumn dataGridViewColumn = dgvSurveyViewer.Columns[ChartSelectedColumn];
            dataGridViewColumn.HeaderCell.Style.BackColor = linecolor;
            dataGridViewColumn.HeaderCell.Style.ForeColor = Color.Black;

            if (myRiscyScope.Visible == false)
            {
                myRiscyScope.RiscyScope_Show();
            }
        }
        #endregion

        #region //================================================================Open ScopeWindow Event:ContexMenuCH1
        private void ContexMenuCH1(object sender, EventArgs e)
        {
            DataGridViewColumn dgvColumnX;
            if ((ChartSelectedColumn <= 3) & (myGlobalBase.DataLog_isSurveyCVSOpen == false))                              // Sensor Range Only, not clock column. 
                return;
            for (int i = 0; i < 4; i++)
            {
                if (i != 1)
                {
                    if (ChartSelectedCh[i] == ChartSelectedColumn)        // Selected channel, avoid overwrite. 
                        return;
                }
            }
            //-----------------------------------------------Selected Different Column, remove old and add new. 
            if (ChartSelectedCh[1] != ChartSelectedColumn)
            {
                dgvColumnX = dgvSurveyViewer.Columns[ChartSelectedColumn];
                dgvColumnX.HeaderCell.Style.BackColor = System.Drawing.SystemColors.Control;
                dgvColumnX.HeaderCell.Style.ForeColor = Color.Black;
                myRiscyScope.RemoveCh1();
            }
            else
            {
                //-------------------------------------------Selected Column as before, remove it. 
                if (ChartSelectedCh[1] != -1)
                {
                    dgvColumnX = dgvSurveyViewer.Columns[ChartSelectedColumn];
                    dgvColumnX.HeaderCell.Style.BackColor = System.Drawing.SystemColors.Control;
                    dgvColumnX.HeaderCell.Style.ForeColor = Color.Black;
                    ChartSelectedCh[1] = -1;
                    myRiscyScope.RemoveCh1();
                    return;
                }
            }
            //----------------------------------------------Selected Column. 
            ChartSelectedCh[1] = ChartSelectedColumn;
            //----------------------------------------------Row and Selected Row
            ContextMenuSelectedRow();
            //----------------------------------------------Init Data Points
            if (ScopeCH1 == null)
            {
                ScopeCH1 = new List<cPoint>();
            }
            float fdata;
            float xxmSec;
            for (int row = ChartStartRow; row < ChartEndRow; row++)
            {
                float.TryParse(dgvSurveyViewer.Rows[row].Cells[2].Value.ToString(), out xxmSec);
                float.TryParse(dgvSurveyViewer.Rows[row].Cells[ChartSelectedColumn].Value.ToString(), out fdata);
                ScopeCH1.Add(new cPoint(xxmSec, fdata));
            }
            //---------------------------------------------Update Chart
            Color linecolor= Color.MediumSeaGreen;
            myRiscyScope.StartChartCh1Class(ChartSelectedStartRow, ChartSelectedEndRow, dgvSurveyViewer.Columns[ChartSelectedColumn].HeaderText, ref ScopeCH1, linecolor);
            //---------------------------------------------Update header DGV
            DataGridViewColumn dataGridViewColumn = dgvSurveyViewer.Columns[ChartSelectedColumn];
            dataGridViewColumn.HeaderCell.Style.BackColor = linecolor;
            dataGridViewColumn.HeaderCell.Style.ForeColor = Color.Black;

            if (myRiscyScope.Visible == false)
            {
                myRiscyScope.RiscyScope_Show();
            }

        }
        #endregion

        #region //================================================================Open ScopeWindow Event:ContexMenuCH2
        private void ContexMenuCH2(object sender, EventArgs e)
        {
            DataGridViewColumn dgvColumnX;
            if ((ChartSelectedColumn <= 3) & (myGlobalBase.DataLog_isSurveyCVSOpen == false))                              // Sensor Range Only, not clock column. 
                return;
            for (int i = 0; i < 4; i++)
            {
                if (i != 2)
                {
                    if (ChartSelectedCh[i] == ChartSelectedColumn)        // Selected channel, avoid overwrite. 
                        return;
                }
            }
            //-----------------------------------------------Selected Different Column, remove old and add new. 
            if (ChartSelectedCh[2] != ChartSelectedColumn)
            {
                dgvColumnX = dgvSurveyViewer.Columns[ChartSelectedColumn];
                dgvColumnX.HeaderCell.Style.BackColor = System.Drawing.SystemColors.Control;
                dgvColumnX.HeaderCell.Style.ForeColor = Color.Black;
                myRiscyScope.RemoveCh2();
            }
            else
            {
                //-------------------------------------------Selected Column as before, remove it. 
                if (ChartSelectedCh[2] != -1)
                {
                    dgvColumnX = dgvSurveyViewer.Columns[ChartSelectedColumn];
                    dgvColumnX.HeaderCell.Style.BackColor = System.Drawing.SystemColors.Control;
                    dgvColumnX.HeaderCell.Style.ForeColor = Color.Black;
                    ChartSelectedCh[2] = -1;
                    myRiscyScope.RemoveCh2();
                    return;
                }
            }
            //----------------------------------------------Selected Column. 
            ChartSelectedCh[2] = ChartSelectedColumn;
            //----------------------------------------------Row and Selected Row
            ContextMenuSelectedRow();
            //----------------------------------------------Init Data Points
            if (ScopeCH2 == null)
            {
                ScopeCH2 = new List<cPoint>();
            }
            float fdata;
            float xxmSec;
            for (int row = ChartStartRow; row < ChartEndRow; row++)
            {
                float.TryParse(dgvSurveyViewer.Rows[row].Cells[2].Value.ToString(), out xxmSec);
                float.TryParse(dgvSurveyViewer.Rows[row].Cells[ChartSelectedColumn].Value.ToString(), out fdata);
                ScopeCH2.Add(new cPoint(xxmSec, fdata));
            }
            //---------------------------------------------Update Chart
            Color linecolor= Color.Cyan;
            myRiscyScope.StartChartCh2Class(ChartSelectedStartRow, ChartSelectedEndRow, dgvSurveyViewer.Columns[ChartSelectedColumn].HeaderText, ref ScopeCH2, linecolor);
            //---------------------------------------------Update header DGV
            DataGridViewColumn dataGridViewColumn = dgvSurveyViewer.Columns[ChartSelectedColumn];
            dataGridViewColumn.HeaderCell.Style.BackColor = linecolor;
            dataGridViewColumn.HeaderCell.Style.ForeColor = Color.Black;

            if (myRiscyScope.Visible == false)
            {
                myRiscyScope.RiscyScope_Show();
            }


        }
        #endregion

        #region //================================================================Open ScopeWindow Event:ContexMenuCH3
        private void ContexMenuCH3(object sender, EventArgs e)
        {
            DataGridViewColumn dgvColumnX;
            if ((ChartSelectedColumn <= 3) & (myGlobalBase.DataLog_isSurveyCVSOpen == false))                               // Sensor Range Only, not clock column. 
                return;
            for (int i = 0; i < 4; i++)
            {
                if (i != 3)
                {
                    if (ChartSelectedCh[i] == ChartSelectedColumn)        // Selected channel, avoid overwrite. 
                        return;
                }
            }
            //-----------------------------------------------Selected Different Column, remove old and add new. 
            if (ChartSelectedCh[3] != ChartSelectedColumn)
            {
                dgvColumnX = dgvSurveyViewer.Columns[ChartSelectedColumn];
                dgvColumnX.HeaderCell.Style.BackColor = System.Drawing.SystemColors.Control;
                dgvColumnX.HeaderCell.Style.ForeColor = Color.Black;
                myRiscyScope.RemoveCh3();
            }
            else
            {
                //-------------------------------------------Selected Column as before, remove it. 
                if (ChartSelectedCh[3] != -1)
                {
                    dgvColumnX = dgvSurveyViewer.Columns[ChartSelectedColumn];
                    dgvColumnX.HeaderCell.Style.BackColor = System.Drawing.SystemColors.Control;
                    dgvColumnX.HeaderCell.Style.ForeColor = Color.Black;
                    ChartSelectedCh[3] = -1;
                    myRiscyScope.RemoveCh3();
                    return;
                }
            }
            //----------------------------------------------Selected Column. 
            ChartSelectedCh[3] = ChartSelectedColumn;
            //----------------------------------------------Row and Selected Row
            ContextMenuSelectedRow();
            //----------------------------------------------Init Data Points
            if (ScopeCH3 == null)
            {
                ScopeCH3 = new List<cPoint>();
            }
            float fdata;
            float xxmSec;
            for (int row = ChartStartRow; row < ChartEndRow; row++)
            {
                float.TryParse(dgvSurveyViewer.Rows[row].Cells[2].Value.ToString(), out xxmSec);
                float.TryParse(dgvSurveyViewer.Rows[row].Cells[ChartSelectedColumn].Value.ToString(), out fdata);
                ScopeCH3.Add(new cPoint(xxmSec, fdata));
            }
            //---------------------------------------------Update Chart
            Color linecolor= Color.MediumPurple;
            myRiscyScope.StartChartCh3Class(ChartSelectedStartRow, ChartSelectedEndRow, dgvSurveyViewer.Columns[ChartSelectedColumn].HeaderText, ref ScopeCH3, linecolor);
            //---------------------------------------------Update header DGV
            DataGridViewColumn dataGridViewColumn = dgvSurveyViewer.Columns[ChartSelectedColumn];
            dataGridViewColumn.HeaderCell.Style.BackColor = linecolor;
            dataGridViewColumn.HeaderCell.Style.ForeColor = Color.Black;

            if (myRiscyScope.Visible == false)
            {
                myRiscyScope.RiscyScope_Show();
            }


        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Misc
        //#####################################################################################################

        #region //================================================================btnQuit_Click
        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region //================================================================btnHelp_Click
        private void btnHelp_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region //================================================================cbMFormat_SelectedIndexChanged (Readout format)
        private void cbMFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            HRM2300.Mag_Data myMagDataHMR = myHRM2300.HRM2300_MagDataRef();
            FluxGateII.Mag_DataFG myMagDataFGII = myFGIIA.FGII_MagDataRef();
            if ((cbMFormat.SelectedIndex == myMagDataHMR.DataFormat) || (cbMFormat.SelectedIndex == myMagDataFGII.DataFormat))       // No Change in index. 
                return;
            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            DialogResult response = FlexiMessageBox.Show("Warning, you about to lose all data due to format change"
            + Environment.NewLine + "OK     = Proceed format change and erase all recorded data."
            + Environment.NewLine + "Cancel = Opps, should I save data into file? or just cancel?",
            "Magnetometer Readout Format Change",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Warning);
            if (response != DialogResult.OK)
                return;
            myMagDataHMR.DataFormat = cbMFormat.SelectedIndex;
            myMagDataFGII.DataFormat = cbMFormat.SelectedIndex;
            Survey_ClearAndUpdateDataGridView(HeaderFrameTool, false);
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### DGV Right Click Mouse
        //#####################################################################################################

        #region //================================================================dgvSurveyViewer_CellMouseClick
        private void dgvSurveyViewer_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    //----------------------------------------------Add context menu
                    System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
                    System.Windows.Forms.MenuItem menuItem1 = new System.Windows.Forms.MenuItem("Save");
                    menuItem1.Click += new EventHandler(ContexMenuSaveAction);
                    System.Windows.Forms.MenuItem menuItem2 = new System.Windows.Forms.MenuItem("Copy");
                    menuItem2.Click += new EventHandler(ContexMenuCopyAction);
                    contextMenu.MenuItems.Add(menuItem1);
                    contextMenu.MenuItems.Add(menuItem2);
                    //----------------------------------------------
                    DataGridViewCell clickedStart = (sender as DataGridView).Rows[e.RowIndex].Cells[0];
                    DataGridViewCell clickedRTC = (sender as DataGridView).Rows[e.RowIndex].Cells[6];      // RTC column. 

                    if (clickedStart.Value.ToString().Contains("$D") == true)
                    {
                        TimeSpan time = TimeSpan.FromSeconds(Convert.ToDouble(clickedRTC.Value.ToString()));

                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).Add(time);
                        contextMenu.MenuItems.Add(new MenuItem(string.Format("Time: {0:HH/mm/ss}", dateTime)));
                        contextMenu.MenuItems.Add(new MenuItem(string.Format("Date: {0:dd/MM/yy}", dateTime)));
                    }

                    //this.dgvSurveyViewer.CurrentCell = this.dgvSurveyViewer.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    //DataGridViewCell clickedCell = (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex];      // Here you can do whatever you want with the cell
                    //this.dgvSurveyViewer.CurrentCell = clickedCell;                                                 // Select the clicked cell, for instance

                    // Get mouse position relative to the vehicles grid
                    var relativeMousePosition = dgvSurveyViewer.PointToClient(Cursor.Position);
                    relativeMousePosition.X += 10;
                    relativeMousePosition.Y += 10;
                    contextMenu.Show(dgvSurveyViewer, relativeMousePosition);
                    //contextMenu.Show(dgvSurveyViewer, new Point(e.X, e.Y));
                }
            }
        }
        #endregion

        #region //================================================================ContexMenuCopyAction
        //http://stackoverflow.com/questions/899350/how-to-copy-the-contents-of-a-string-to-the-clipboard-in-c 
        // The code is what you see is what you capture, ie taken from Datagridview not from sframe[].
        // It also sort polarity to ensure correct timing sequence.
        // It filter out spurious \r and then add \n. It has two comma left but no serious impact on excel or notepad view. 
        // Fully tested and working fine.
        // The outstanding issue is thread related to STA.....so far it not really needed. 
        private void ContexMenuCopyAction(object sender, EventArgs e)
        {
            RecievedDataForFiling = HeaderFrameTool + '\n';
            StringBuilder sb = new StringBuilder();
            //-----------------------------------------------reverse or forward order
            if (dgvSurveyViewer.SelectedRows[0].Index > dgvSurveyViewer.SelectedRows[dgvSurveyViewer.SelectedRows.Count - 1].Index)
            {
                List<DataGridViewRow> rows = (from DataGridViewRow row in dgvSurveyViewer.SelectedRows where !row.IsNewRow orderby row.Index select row).ToList<DataGridViewRow>();
                for (int i = 0; i < rows.Count; i++)
                {
                    sb.Clear();
                    for (int col = 0; col < rows[i].Cells.Count; col++)
                    {
                        if (rows[i].Cells[col].Value.ToString() != "")
                        {
                            sb.Append(rows[i].Cells[col].Value.ToString() + ';');
                            sb.Replace("\r", "");                               // remove certain char that causing spurious issue in excel. 
                            sb.Replace("\n", "");
                        }
                    }
                    RecievedDataForFiling += sb.ToString() + '\n'; // that will give you the string you desire

                }
            }
            else
            {
                foreach (DataGridViewRow row in dgvSurveyViewer.SelectedRows)
                {
                    sb.Clear();
                    for (int col = 0; col < row.Cells.Count; col++)
                    {
                        if (row.Cells[col].Value.ToString() != "")
                        {
                            sb.Append(row.Cells[col].Value.ToString() + ';');
                            sb.Replace("\r", "");                               // remove certain char that causing spurious issue in excel. 
                            sb.Replace("\n", "");
                        }
                    }
                    RecievedDataForFiling += sb.ToString() + '\n'; // that will give you the string you desire
                }

            }
            System.Windows.Forms.Clipboard.SetText(RecievedDataForFiling);
        }

        #endregion

        #region //================================================================ContexMenuSaveAction
        // The code is what you see is what you capture, ie taken from Datagridview not from sframe[].
        // It also sort polarity to ensure correct timing sequence.
        // It filter out spurious \r and then add \n. It has two comma left but no serious impact on excel or notepad view. 
        // Fully tested and working fine. 
        private void ContexMenuSaveAction(object sender, EventArgs e)
        {
            RecievedDataForFiling = HeaderFrameTool + '\n';
            StringBuilder sb = new StringBuilder();
            //-----------------------------------------------reverse or forward order
            if (dgvSurveyViewer.SelectedRows[0].Index > dgvSurveyViewer.SelectedRows[dgvSurveyViewer.SelectedRows.Count - 1].Index)
            {
                List<DataGridViewRow> rows = (from DataGridViewRow row in dgvSurveyViewer.SelectedRows where !row.IsNewRow orderby row.Index select row).ToList<DataGridViewRow>();
                for (int i = 0; i < rows.Count; i++)
                {
                    sb.Clear();
                    for (int col = 0; col < rows[i].Cells.Count; col++)
                    {
                        if (rows[i].Cells[col].Value.ToString() != "")
                        {
                            sb.Append(rows[i].Cells[col].Value.ToString() + ';');
                            sb.Replace("\r", "");                               // remove certain char that causing spurious issue in excel. 
                            sb.Replace("\n", "");
                        }
                    }
                    RecievedDataForFiling += sb.ToString() + '\n'; // that will give you the string you desire
                }
            }
            else
            {
                foreach (DataGridViewRow row in dgvSurveyViewer.SelectedRows)
                {
                    sb.Clear();
                    for (int col = 0; col < row.Cells.Count; col++)
                    {
                        if (row.Cells[col].Value.ToString() != "")
                        {
                            sb.Append(row.Cells[col].Value.ToString() + ';');
                            sb.Replace("\r", "");                               // remove certain char that causing spurious issue in excel. 
                            sb.Replace("\n", "");
                        }
                    }
                    RecievedDataForFiling += sb.ToString() + '\n'; // that will give you the string you desire
                }

            }

            //-------------------------------------------------Save to filename under SelectSave_<timestamp>
            SurveyCVS_OpenSaveDataCloseFile(sDefaultFolder);
            RecievedDataForFiling = null;
        }
        #endregion

        #region //================================================================SurveyCVS_OpenSaveDataCloseFile
        bool SurveyCVS_OpenSaveDataCloseFile(string sFoldername)
        {
            if (sFoldername == null)
                return false;
            txtFolderName.Text = sFoldername;
            sFilename = System.IO.Path.Combine(sFoldername, "Selected_Imported_CVS" + DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + ".csv");   // Default filename without SNAD, this is added automatically on demand.
            try
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(sFilename, true, Encoding.ASCII);
                    sw.Write(RecievedDataForFiling);                                         // Dataframe (assuming it has \n\0 terminator)
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Dispose();
                    }
                }
                myMainProg.myRtbTermMessageLF("+INFO: Selected rows Data successfully Saved!");
                return true;
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("#ERR: Unable to append file : " + sFilename);
                MessageBox.Show("Problem in saving imported data into filename, try again",
                       "Import Data CVS",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Exclamation,
                       MessageBoxDefaultButton.Button1);
            }
            return false;

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### DGV
        //#####################################################################################################

        #region //================================================================btnReset_Click (Reset Sort)
        private void btnReset_Click(object sender, EventArgs e)
        {
            btnReset.Visible = false;
            DGVtable.DefaultView.Sort = string.Empty;
            this.Refresh();
        }
        #endregion

        #region //================================================================Sort Reset Button Control via MouseLeave and CellMouseUp Event. 

        private void dgvSurveyViewer_MouseLeave(object sender, EventArgs e)
        {
            if (DGVtable != null)
            {
                if (DGVtable.DefaultView.Sort != string.Empty)
                {
                    btnReset.Visible = true;
                }
            }
        }
        private void dgvSurveyViewer_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (DGVtable != null)
            {
                if (DGVtable.DefaultView.Sort != string.Empty)
                {
                    btnReset.Visible = true;
                }
            }
        }

        #endregion

        #region //================================================================dgvSurveyViewer_Sorted
        private void dgvSurveyViewer_Sorted(object sender, EventArgs e) // This does not work anymore so we use mouseleave and cellmouseclickup event
        {
            btnReset.Visible = true;
        }
        #endregion

        #region //================================================================btnClear_Click (Clear DGV Data)
        private void btnClear_Click(object sender, EventArgs e)
        {
            SurveyClearAllData();
        }
        #endregion                              

        #region //================================================================SurveyClearAllData
        private bool SurveyClearAllData()
        {
            Survey_Timer_Stop();                    // Cease Loop Timer. 
            dgvSurveyViewer.ResumeLayout(false);    // Re-Enable DGV.

            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            DialogResult response = FlexiMessageBox.Show("Warning: This will clear all Data!, 'OK' to proceed?",
                "Warning: This will clear all Data!, 'OK' to proceed?",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);
            if (response != DialogResult.OK)
            {
                return false;
            }
            //-------------------------------------------------------
            DataTable DT = (DataTable)dgvSurveyViewer.DataSource;
            if (DT != null)
                DT.Rows.Clear();
            //dgvSurveyViewer.Rows.Clear();
            this.Refresh();
            return true;
        }
        #endregion

        #region //================================================================cbExtendView_CheckedChanged

        private void cbExtendView_CheckedChanged(object sender, EventArgs e)
        {
            int extend = 300;
            if (cbExtendView.Checked == false)
            {
                this.dgvSurveyViewer.Size = new System.Drawing.Size(1065, 320);
                this.dgvSurveyViewer.MinimumSize = new System.Drawing.Size(1065, 320);
                this.dgvSurveyViewer.MaximumSize = new System.Drawing.Size(1065, 320);
                this.ClientSize = new System.Drawing.Size(1089, 459);
                this.MaximumSize = new System.Drawing.Size(1105, 498);
            }
            else
            {
                this.MaximumSize = new System.Drawing.Size(1105, 498 + extend);
                this.ClientSize = new System.Drawing.Size(1089, 459 + extend);
                this.dgvSurveyViewer.MaximumSize = new System.Drawing.Size(1065, 320 + extend);
                this.dgvSurveyViewer.MinimumSize = new System.Drawing.Size(1065, 320 + extend);
                this.dgvSurveyViewer.Size = new System.Drawing.Size(1065, 320 + extend);

            }
            this.Invalidate();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Commands
        //#####################################################################################################
        //================================================================================================
        #region //================================================================Command_ASCII_Process() without prefix // Also process CANUSB & USBUART
        //================================================================================================
        public void Survey_Command_ASCII_Process()
        {
            myGlobalBase.LoggerOpertaionMode = true;
            if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_FTDI)
            {
                myUSBComm.FTDI_Message_Send(m_sEntryTxt);
            }
            if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM)
            {
                myUSBVCOMComm.VCOM_Message_Send(m_sEntryTxt);
            }

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Misc
        //#####################################################################################################

        #region //================================================================cbStayTop_MouseClick
        private void cbStayTop_MouseClick(object sender, MouseEventArgs e)
        {
            this.TopMost = cbStayTop.Checked;

        }
        #endregion

        #region //================================================================btnCalWindow_Click
        private void btnCalWindow_Click(object sender, EventArgs e)
        {
            myCalibration.Calibration_Show();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }

    //#####################################################################################################
    //###################################################################################### Class 
    //#####################################################################################################
    #region --------------------------------------------------------Class ScopeData_Operation
    [Serializable]
    public class DeviceStatusComm
    {
        public string Com { get; set; }         // String Name 
        public string Device { get; set; }       // 0 = No Offset, +/- data = offset
        public string Status { get; set; }         // 1 = 1V/V
        // =====================================================Constructor
        public DeviceStatusComm()
        {
            Com = "";
            Device = "";
            Status = "OFF";
        }
    }
    #endregion

    #region//=============================================================DoubleBuffered property via reflection for datagridview, improve flickerless image.
    public static class ExtensionMethodsMessage1
    {
        public static void DoubleBufferedMessage1(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    #endregion
}
