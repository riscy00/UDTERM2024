/******************************************************************************
 *   Copyright (C) 2017 : Total Vision Bulgaria, Sofia, Bulgaria,
 *
 *   All Right Reserved.
 *****************************************************************************
 *
 *   FILENAME  	: LoggerCVS
 *   PURPOSE   	: Generic Data Transfer Protocol
 *   AUTHOR    	: Richard Payne, Total Vision Bulgaria, Sofia.
 *   REVISION  	: UDT LITE 76 (copied from LITE 64)
 *   History   	:
 *   NOTE	   	: VS 2017 and .NET4.7.2 
 ******************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using UDT_Term;
using Timer = System.Timers.Timer;

// Power Tool: insert QuickLaunch with "@tasks CollapseRegions" to close all region only. 

// Design Note
// ===========
// UART send "LOGGER_$H" command to invokes the header formatting and stream download until 'n0' end terminator is detected 

///////////////////////////////////////////////////////////////////////////
// Outstanding Task
//==================
// (a) Python Add on to validate data (major task), this generate abnormal reading alerts
// (b) Handle Null command (not urgent and small task). This just for completeness.
// (c) Handling malfunctioned data. (medium task, already included try/catch method to reject defective data stream).
// (d) Flag monitoring on message. (under consideration, but not essential as this is viewed on excel). 
// (e) Optimizing timeout/wait for message period.
// (f) Study XAML feature later.   
// (g) Interface with CAN bus with ID 0x0F message protocol. Receives text on 0x0D and interfaced with this code, similar to the FTDI method.
// (h) Permits discrete command on main terminal (need to sort out the console select issue with myGlobalBase.LoggerOpertaionMode = false (async racing issue with <LOGON>$E and <LOGOFF>$E response. 
// (i) Update document.
// (j) Click button to open folder (short cut). 
//////////////////////////////////////////////////////////////////////////

namespace UDT_Term_FFT
{
    public partial class LoggerCSV : Form
    {
        private System.Windows.Forms.RichTextBox rtbConsole;        //taken from LoggerCSV.Designer, do not delete!
        ITools Tools = new Tools();
        Timer LCVStimer1;
        ClientCode myClientCode;
        USB_FTDI_Comm myUSBComm;                    // FTDI device manager section (scan, open, close, read, write).
        USB_VCOM_Comm myUSBVCOMComm;
        EE_LogMemSurvey myEEDownloadSurvey;
        GlobalBase myGlobalBase;
        USB_Message_Manager myUSB_Message_Manager;
        DMFProtocol myDMFProtocol;
        MiniAD7794Setup myMiniSetup;
        PDFViewer myPDFViewerBGDebug;
        DialogSupport mbOperationBusy;
        DialogSupport mbOperationError;
        MainProg myMainProg;
        CRC8CheckSum myCalc_CRC8;
        Frame_Detokeniser myDetoken;
        UDTmyFrameDataLog myUDTmyFrameDataLog;

        #region//----------------------------------------Reference
        public void MyUSB_Message_Manager(USB_Message_Manager myUSB_Message_ManagerRef)
        {
            myUSB_Message_Manager = myUSB_Message_ManagerRef;
        }
        public void MyMiniAD7794Setup(MiniAD7794Setup myMiniSetupRef)
        {
            myMiniSetup = myMiniSetupRef;
        }
        public void MyDMFProtocol(DMFProtocol myDMFProtocolRef)
        {
            myDMFProtocol = myDMFProtocolRef;
        }
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
                case ((int)GlobalBase.eCompanyName.TVB):
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
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        public void MyUSBComm(USB_FTDI_Comm myUSBCommRef)
        {
            myUSBComm = myUSBCommRef;
        }
        public void MyUSBVCOMComm(USB_VCOM_Comm myUSBVCOMCommRef)
        {
            myUSBVCOMComm = myUSBVCOMCommRef;
        }

        public void MyEEDownloadSurvey(EE_LogMemSurvey myEEDownloadSurveyRef)
        {
            myEEDownloadSurvey = myEEDownloadSurveyRef;
        }
        public void MyClientCode(ClientCode myClientCodeRef)
        {
            myClientCode = myClientCodeRef;
        }
        public System.Windows.Forms.RichTextBox Ref_rtbConsole()
        {
            return rtbConsole;                                              // Passing object reference of the 
        }

        #endregion

        //---------------------------------------Message
        private string sMessageFormat;                                 // Format message, provided by the device, this also trigger new filename.
        private string sMessageHeader;                                 // Header message, provided by the device, this also trigger new filename.
        private string sMessageData;                                   // Data message, provided by the device,
        private string sMessageNote;                                   // Append Note message, provided by the user, it append note with $A and time stamp. 
        private char[] cMessage = new char[256];                       // intermediate transfer from LIN dataframe download. 
        //private UInt32      cMessagePointer;                                // point for the array.
        //private bool        bDataFrameError;                                // Dataframe failure transfer including SNAD mismatch. 
        private bool bPauseButtonClicked;                            // true = pause button clicked. 
        private bool bHoldConsoleScreen;                             // true = Hold console screen, false = Normal operation.
#pragma warning disable 0169
        private Char uDataFrameType;                                 // 'D' = Dataframe, 'H' Header, 'N' Null command, detected from 1st LIN frame of the dataframe message
        private UInt32 iMessageNoOfToken;                              // For datagridview setup.(dgvMessage)  
#pragma warning restore 0169
#pragma warning disable 414
        private Byte bMessageModeSelected;
#pragma warning restore 414
        private bool bAppendedText;                                  // true = Text has been previously appended to file, ready to edit (to white) when next key is pressed.
        private bool bDataFrameTestOnly;                             // true = prevent saving data into files.
        private bool bAppendOneFileEnable;                           // true = append console text into single filename, help to reduce sitching discrete console save data.
        //--------------------------------------Filename
        private string sFoldername;
        private string sFoldernameDefault;
        private string sFilename;
        private string sFilenameConsole;
        private string sAppendFileName;
        private string sErrorFileName;
        private string sAppendConsoleOneFileName;

        //--------------------------------------Async Alert TimeOut in case of no message event.
        private int iASyncAlertTimerSetting;                        // -1 Do not use
        private int iASyncAlertTimerCurrent;                        // current clock, which pop up error message if exceeded time limit. 
        private int iAsyncAlertTimerCount;                          // Count Event.
        private bool isASYNC_STCSTOP_occurred;                       // true if STC_Stop is pushed
        //-------------------------------------CAN response data
        //private Byte        Response_Mode;                  // D4 from invoke command.
        //private UInt32      Response_Systick;               // D4-D7 alway
        //private Byte        Response_OperationStatus;       // D3 alway
        //private Byte        Response_MessageStatus;         // D2 alway Response Message Status
        //private Byte        Response_DeinvokeState;         // D2 alway Response Message Status
        //-------------------------------------Timer/loop
        private UInt32 BatchWaitPeriod;
        private UInt32 iFrameWaitLoop;
        private Int32 iStopCounter;                     // -1 = Not used, otherwise count backward and cease logging operation when 0.
        private Int32 iStopCounterBackup;               // Backup of iStopCounter
        //-------------------------------------UART/CAN Control: NB: Console is read only.  
        private string m_sLoggerEntryTxt;
        private string m_sLoggerResponseTxt;
        //private bool isAsynctoSync;
        private bool isTurnOffSyncTab;
        private ToolStripMenuItem SyncMenuItemtemp;
        uint[] SyncMenuItem_mSec = new uint[] { 100, 1000, 10000, 30000, 60000, 0 };
        IntPtr rtbOEMConsole;
        //===============================================================================================================================Getter/Setter

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public LoggerCSV()
        {
            rtbOEMConsole = new IntPtr();
            InitializeComponent();
            dgvMessage.DoubleBufferedLoggerMessage1(true);
            myCalc_CRC8 = new CRC8CheckSum();
            sFoldernameDefault = Tools.GetPathUserFolder();
            sFoldername = sFoldernameDefault;
        }
        //#####################################################################################################
        //###################################################################################### Form Manager
        //#####################################################################################################

        #region //================================================================LoggerCSV_Load
        private void LoggerCSV_Load(object sender, EventArgs e)
        {
            this.BringToFront();
            this.TopMost = false;
            this.WindowState = FormWindowState.Normal;
            //---------------------------------Default Config
            iStopCounter = -1;                              // Disable logging counter. 
            bMessageModeSelected = (Byte)'H';
            bPauseButtonClicked = false;
            iMessageNoOfToken = 60;
            bAppendedText = false;
            bDataFrameTestOnly = false;
            bAppendOneFileEnable = false;
            enableAppendOneFilename.Checked = false;        // make sure append console text to one file name as disabled. 
            //---------------------------------Filename
            if (sFoldername == "")
            {
                sFoldername = sFoldernameDefault;
                txtFolderName.Text = sFoldername;
                sFilename = System.IO.Path.Combine(sFoldername, DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss"));   // Default filename without SNAD, this is added automatically on demand.
                txtFilename.Text = "(Default)" + sFilename;

                sAppendFileName = System.IO.Path.Combine(sFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_AppendNote.csv"));
                sErrorFileName = System.IO.Path.Combine(sFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_ExceptionReport.txt"));
            }
            //---------------------------------Timer/Loop
            BatchWaitPeriod = 5;        //60
            txtCyclePeriod.Text = BatchWaitPeriod.ToString();
            //--------------------------------- Async GroupBox

            #region//--------------------------------------------dgvMessage

            #region //==============================Column Definition
            DataGridViewTextBoxColumn[] txtMessage = new DataGridViewTextBoxColumn[4];
            int[] width = new int[] { 30, 125, 80, 120 };
            string[] mheader = new string[] { "No", "Header", "Data", "Data2" };
            for (int i = 0; i < 0x4; i++)
            {
                txtMessage[i] = new DataGridViewTextBoxColumn();
                //----Name
                txtMessage[i].Name = "txtCol" + i.ToString();
                //----Header Text
                txtMessage[i].HeaderText = mheader[i];
                txtMessage[i].DefaultCellStyle.Font = new System.Drawing.Font("Arial", 10.5F, GraphicsUnit.Pixel);
                //----Width
                txtMessage[i].Width = width[i];
                //----Resizable
                txtMessage[i].Resizable = DataGridViewTriState.False;
                //---Disable sort
                txtMessage[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                //---Other
                this.dgvMessage.Columns.Add(txtMessage[i]);
            }
            #endregion

            #region //==============================Row Definition
            dgvMessage.Visible = false;
            dgvMessage.RowTemplate.Height = 15;
            for (int y = 0; y < iMessageNoOfToken; y++)
            {
                this.dgvMessage.Rows.Add(new string[] { y.ToString() });
            }
            dgvMessage.Visible = true;

            #endregion

            #endregion

            // This is derived class to implement transparency for tabcontrol, see http://stackoverflow.com/questions/30063061/how-can-i-make-a-transparent-tabpage 
            tpLoggerCVS.MakeTransparent();
            tpLoggerAsync.BackColor = Color.Transparent;
            tpLoggerSync.BackColor = Color.Transparent;
            //------------------------------------------------Select SNAD Mode if activated. 
            if (myGlobalBase.mySNADNetwork != null)     // Has SNAD network setup?
            {
                if (myGlobalBase.mySNADNetwork.isNetworkEnabled == true)
                {
                    tpLoggerCVS.SelectTab(2);
                    tbAsyncPeroid.Text = "255";
                    tbAsyncPeroid.Enabled = false;
                    rtbAppendBox.ResetText();
                    rtbAppendBox.AppendText("IMPORTANT: The SNAD Network is enabled, DO NOT MODIFY THE ASYNC PERIOD=255 !!.\nThe period is determined by firmware.");
                }
            }
            if (tbAsyncPeroid.Enabled != false)
                tpLoggerCVS.SelectTab(1);
            //------------------------------------------------
            //isAsynctoSync = false;
            cbSendToLogMem.Checked = false;
            //------------------------------------------------------------------------Sync Option Menu Item

            string[] SyncMenuItemText = new string[] {
                "TimeOut 100 mSec",
                "TimeOut 1 Sec",
                "TimeOut 10 Sec",
                "TimeOut 30 Sec",
                "TimeOut 60 Sec",
                "TimeOut OFF"
            };
            int j = 0;
            SyncMenuItemtemp = this.tsm_OptionSync;
            foreach (var SyncMenuItemTextx in SyncMenuItemText)
            {
                ToolStripItem subItem = new ToolStripMenuItem(SyncMenuItemTextx);
                subItem.Click += new EventHandler(tsm_OptionSync_Click);
                subItem.Tag = j;
                tsm_OptionSync.DropDownItems.Add(subItem);
                if (j == 3) subItem.PerformClick();         // Default 30 Second.
                j++;
            }
            //------------------------------------------------------------------------SNAD
            if (myGlobalBase.mySNADNetwork != null)
            {
                if (myGlobalBase.mySNADNetwork.isNetworkEnabled == true)
                {
                    tbAsyncPeroid.Text = "255";
                    tbAsyncPeroid.Enabled = false;
                    rtbAppendBox.ResetText();
                    rtbAppendBox.AppendText("IMPORTANT: The SNAD Network is enabled, DO NOT MODIFY THE ASYNC PERIOD=255 !!.\nThe period is determined by firmware.");
                    this.BringToFront();
                    return;
                }
            }
            rtbAppendBox.ResetText();
            tbAsyncPeroid.Enabled = true;
            rtbAppendBox.AppendText("Comment here and this append to your csv file.Insert Text and press enter to append.\nIt add $A along with date/ time stamp at the start of the text, so it can be sorted / filtered out within Excel.");
            this.BringToFront();
            //-----------------------------------------------------------------------STCSTART
            cbCallBackMode.Checked = myGlobalBase.LoggerCVS_CallBackMode;
            cbSTCSTARTMode.SelectedIndex = myGlobalBase.LoggerCVS_STCSTARTMode;
            //-----------------------------------------------------------------------Added 78F
            if (myUDTmyFrameDataLog == null)
                myUDTmyFrameDataLog = new UDTmyFrameDataLog();
            else
                myUDTmyFrameDataLog.ClearAllVariable();
        }
        #endregion

        #region //================================================================tsm_OptionSync_Click
        private void tsm_OptionSync_Click(object sender, EventArgs e)
        {
            SyncMenuItemtemp.CheckState = CheckState.Unchecked;
            SyncMenuItemtemp = (ToolStripMenuItem)sender;
            SyncMenuItemtemp.CheckState = CheckState.Checked;
            int SyncMenuItemSelect = (int)SyncMenuItemtemp.Tag;
            if (SyncMenuItemSelect < SyncMenuItem_mSec.Length)
                myGlobalBase.WaitCyclePeriodSetting = SyncMenuItem_mSec[SyncMenuItemSelect];
        }
        #endregion

        #region //================================================================LoggerCSV_FormClosing
        private void LoggerCSV_FormClosing(object sender, FormClosingEventArgs e)
        {
            myGlobalBase.LoggerOpertaionMode = false;       // Cease Logger Terminal mode.
            myGlobalBase.LoggerWindowVisable = false;
            myGlobalBase.LoggerCVSMiniPortEnable = false;   // Added 22/10/17
            e.Cancel = true;
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================LoggerCSV_Shown (###BUG: Fired only once, will not refired when shown). 
        private void LoggerCSV_Shown(object sender, EventArgs e)
        {
        }
        #endregion

        #region //================================================================LoggerCVS_SetupShow_Window
        public void LoggerCVS_SetupShow_Window(int iSourceCall, string sFolderNameRef)  //SourceCall: 0=Misc, 1 = MiniAD7794 Project, 2 = etc.
        {
            this.BringToFront();
            this.TopMost = false;
            this.WindowState = FormWindowState.Normal;
            //---------------------------------------------
            myGlobalBase.LoggerOpertaionMode = true;
            myGlobalBase.LoggerWindowVisable = true;
            string sTimeStampNow = DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss");
            sAppendConsoleOneFileName = "_ConsoleAppendOneFN_" + sTimeStampNow + ".txt";
            switch (iSourceCall)
            {
                case (0):       // Default/Generic
                    {
                        isTurnOffSyncTab = false;
                        sFoldername = sFoldernameDefault;
                        if (!tpLoggerCVS.TabPages.Contains(tpLoggerSync))
                        {
                            tpLoggerCVS.TabPages.Insert(0, tpLoggerSync);
                        }
                        toolsToolStripMenuItem.Enabled = true;
                        toolsToolStripMenuItem.Visible = true;
                        myGlobalBase.LoggerCVSMiniPortEnable = false;
                        break;
                    }
                case (1):       // MiniAD7794 Call.
                    {
                        isTurnOffSyncTab = true;
                        sFoldername = sFolderNameRef;
                        tpLoggerSync.Hide();
                        tpLoggerCVS.SelectTab(tpLoggerAsync);
                        tpLoggerCVS.TabPages.Remove(tpLoggerSync);
                        toolsToolStripMenuItem.Enabled = false;
                        toolsToolStripMenuItem.Visible = false;
                        myGlobalBase.LoggerCVSMiniPortEnable = true;
                        break;
                    }
                default:
                    break;
            }
            txtFolderName.Text = sFoldername;
            sFilename = System.IO.Path.Combine(sFoldername, DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss"));   // Default filename without SNAD, this is added automatically on demand.
            txtFilename.Text = "(Default)" + sFilename;

            sAppendFileName = System.IO.Path.Combine(sFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_AppendNote.csv"));
            sErrorFileName = System.IO.Path.Combine(sFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_ExceptionReport.txt"));
            btnFolder.Enabled = true;
            this.BringToFront();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### STC Control
        //#####################################################################################################

        #region //================================================================STC_Logger_OpenWindow
        public void STC_Logger_OpenWindow(string sFolderNameRef)
        {
            sFoldername = sFolderNameRef;
            sFilename = System.IO.Path.Combine(sFoldername, DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss"));   // Default filename without SNAD, this is added automatically on demand.
            txtFilename.Text = "(Default)" + sFilename;
            sAppendFileName = System.IO.Path.Combine(sFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_AppendNote.csv"));
            sErrorFileName = System.IO.Path.Combine(sFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_ExceptionReport.txt"));

            if (Logger_OpenNewFile(true) == false)                      // Validate filename for CVS logging. 
            {
                rtbConsoleAppend("\n#ERR: !!! ASYNC MODE IS NOT ACTIVATED DUE TO INCORREDCT DRIVE/FOLDER !!!");
                btnSTCStart.Enabled = false;
                btnSTCStartSnad.Enabled = false;
                return;
            }
            btnSTCStart.Enabled = true;
            btnSTCStartSnad.Enabled = true;
            enableAsyncWithGDERToolStripMenuItem.Checked = true;
            //             myGlobalBase.LoggerAsyncMode = true;
            //             myGlobalBase.LoggerAsyncSnadMode = true;
            myGlobalBase.LoggerCVSMiniPortEnable = true;
            //----------------------------------------Enable Append into one filename. 
            //enableAppendOneFilename.Checked = true;
            //bAppendOneFileEnable = true;
            //-----------------------------------
            btnFolder.Enabled = false;
            btnTestHeader.Enabled = false;
            btnDataFrame.Enabled = false;
            bDataFrameTestOnly = false;
            //-------------------------------------Open Downhole Survey Window and update default folder location to save data into. 
            rtbConsoleAppend("Transfer to BG Survey Enabled!, Opening the BG Downhole Survey Window");
            myEEDownloadSurvey.SurveyUpdateFolderName(sFoldername);
            myEEDownloadSurvey.EE_LogMemSurvey_Show();
            myEEDownloadSurvey.SurveyLoggerCVS_Start();
            cbSendToLogMem.Enabled = false;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Command Processor
        //#####################################################################################################

        //================================================================================================
        #region //================================================================Command_Processor
        //================================================================================================
        public void Command_Processor(String CommandEntry)
        {
            switch (CommandEntry)
            {
                case ("$D"):            // Download Data
                    {
                        rtbConsoleAppend("-INFO : Command Entry: Request Data Download Frame");
                        Download_Data_Processor();
                        break;
                    }
                case ("$H"):            // Download Header
                    {
                        rtbConsoleAppend("-INFO : Command Entry: Request Header Download Frame");
                        Download_Header_Processor();
                        break;
                    }
                case ("$T"):            // Format Header
                    {
                        rtbConsoleAppend("-INFO : Command Entry: Request Header Download Frame");
                        Download_Format_Processor();
                        break;
                    }
                case ("$G"):            // Download Header (Added 19 March 2016)
                    {
                        rtbConsoleAppend("-INFO : Command Entry: Request Data2 (Gyro) Download Frame");
                        Download_Data2_Processor();
                        break;
                    }
                case ("$N"):            // Null Command only
                    {
                        rtbConsoleAppend("-INFO : Command Entry: Request Null Download Frame");
                        Download_Null_Processor();
                        break;
                    }
                case ("$S"):            // Start auto loop / logging operation
                    {
                        rtbConsoleAppend("-INFO : Command Entry: Enable logger operation");
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        //================================================================================================
        #region //================================================================Download_Data_Processor ($D,SYSTICK,SNAD)
        //================================================================================================
        public void Download_Data_Processor()
        {
            //-----------------------------------------------################# Sync Logger Section Original Code (unchanged).
            // This download the data frame from the device
            // It is essential to have 1st format before rest of the data in sync with the header. 
            // $D,SYSTICK,SNAD, and then user data stream with comma separator (",") 
            // Must have '\n' end terminator.
            // on reception, it append data frame into selected timestamped file (previous configured by header)
            // There is two way to implement this
            //      Send request command "LOGGER_$D(FF)"      (NB: CAN ESD/OD under 0x0F, details TBA)
            //      Optionally send $H command at any time! 
            try
            {
                if (myGlobalBase.LoggerAsyncMode == true)
                {
                    //-----------------------------------------------################# ASync Logger Section Added: 31/1/16
                    myUSB_Message_Manager.LoggerMessageRX = "";
                    m_sLoggerResponseTxt = "";
                    myGlobalBase.LoggerOpertaionMode = true;
                    myUSB_Message_Manager.endoflinedetected = false;
                    iFrameWaitLoop = 0;
                    if (bgwk_DataWaitLoop.IsBusy == false)      // This fix thread issue.
                        bgwk_DataWaitLoop.RunWorkerAsync();     // Now we wait for response, using background worker to keep window form responsive to user. 
                    return;
                }
                if (myGlobalBase.LoggerAsyncSnadMode == true)
                {
                    //-----------------------------------------------################# ASync SNAD Logger Section Added: 7/1/19
                    myUSB_Message_Manager.LoggerMessageRX = "";
                    m_sLoggerResponseTxt = "";
                    myGlobalBase.LoggerOpertaionMode = true;
                    myUSB_Message_Manager.endoflinedetected = false;
                    iFrameWaitLoop = 0;
                    if (bgwk_DataWaitLoop.IsBusy == false)      // This fix thread issue.
                        bgwk_DataWaitLoop.RunWorkerAsync();     // Now we wait for response, using background worker to keep window form responsive to user. 
                    return;
                }
                //-----------------------------------------------SYNC Mode. 
                if (Logger_ON_Command() == false) return;
                myUSB_Message_Manager.LoggerMessageRX = "";
                m_sLoggerEntryTxt = "LOGGER_$D(0xFF)";      // Remove \n
                m_sLoggerResponseTxt = "";
                myGlobalBase.LoggerOpertaionMode = true;
                myUSB_Message_Manager.endoflinedetected = false;
                Logger_Command_ASCII_Process();
                // UART only!            
                iFrameWaitLoop = 0;
                if (bgwk_DataWaitLoop.IsBusy == false)      // This fix thread issue.
                    bgwk_DataWaitLoop.RunWorkerAsync();     // Now we wait for response, using background worker to keep window form responsive to user.  
            }
            catch
            {
                rtbConsoleAppend("##ERR: RunWorkerAsync thread is busy or crashed, reboot UDT and please restrict single active operation under ASYNC or SYNC but do not alternate!");
            }
        }
        #endregion

        //================================================================================================

        #region //================================================================Download_Data2_Processor ($G,SYSTICK,SNAD, ....Prevously Gyro)
        //================================================================================================
        private void Download_Data2_Processor()
        {
            //-----------------------------------------------################# Sync Logger Section Original Code (unchanged).
            // This download the data2 (gyro) frame from the device
            // It is essential to have 1st format before rest of the data in sync with the header. 
            // $D,SYSTICK,SNAD, and then user data stream with comma separator (",") 
            // Must have '\n' end terminator.
            // on reception, it append data frame into selected timestamped file (previous configured by header)
            // There is two way to implement this
            //      Send request command "LOGGER_$G(FF)"      (NB: CAN ESD/OD under 0x0F, details TBA)
            //      Optionally send $H command at any time! (not supported yet). 
            try
            {
                if (myGlobalBase.LoggerAsyncMode == true)
                {
                    //-----------------------------------------------################# ASync Logger Section Added: 31/1/16
                    myUSB_Message_Manager.LoggerMessageRX = "";
                    m_sLoggerResponseTxt = "";
                    myGlobalBase.LoggerOpertaionMode = true;
                    myUSB_Message_Manager.endoflinedetected = false;
                    iFrameWaitLoop = 0;
                    if (bgwk_DataWaitLoop.IsBusy == false)      // This fix thread issue.
                        bgwk_DataWaitLoop.RunWorkerAsync();     // Now we wait for response, using background worker to keep window form responsive to user. 
                    return;
                }
                if (myGlobalBase.LoggerAsyncSnadMode == true)
                {
                    //-----------------------------------------------################# ASync SNAD Logger Section Added: 07/01/19
                    myUSB_Message_Manager.LoggerMessageRX = "";
                    m_sLoggerResponseTxt = "";
                    myGlobalBase.LoggerOpertaionMode = true;
                    myUSB_Message_Manager.endoflinedetected = false;
                    iFrameWaitLoop = 0;
                    if (bgwk_DataWaitLoop.IsBusy == false)      // This fix thread issue.
                        bgwk_DataWaitLoop.RunWorkerAsync();     // Now we wait for response, using background worker to keep window form responsive to user. 
                    return;
                }
                //-----------------------------------------------SYNC Mode. 
                if (Logger_ON_Command() == false) return;
                myUSB_Message_Manager.LoggerMessageRX = "";
                m_sLoggerEntryTxt = "LOGGER_$G(0xFF)";      // Remove \n
                m_sLoggerResponseTxt = "";
                myGlobalBase.LoggerOpertaionMode = true;
                myUSB_Message_Manager.endoflinedetected = false;
                Logger_Command_ASCII_Process();
                // UART only!            
                iFrameWaitLoop = 0;
                //                 if (bgwk_DataWaitLoop.IsBusy==false)        // This fix thread issue.
                //                     bgwk_DataWaitLoop.RunWorkerAsync();     // Now we wait for response, using background worker to keep window form responsive to user.  
            }
            catch
            {
                rtbConsoleAppend("##ERR: RunWorkerAsync thread is busy or crashed, reboot UDT and please restrict single active operation under ASYNC or SYNC but do not alternate!");
            }
        }
        #endregion

        #region //================================================================bgwk_DataWaitLoop_DoWork//bgwk_DataWaitLoop_RunWorkerCompleted
        //         private void bgwk_DataWaitLoop_DoWork(object sender, DoWorkEventArgs e)
        //         {
        //             //Tools.InteractivePause(TimeSpan.FromMilliseconds(10));
        //             Thread.Sleep(10);        //10mSec sleep while waiting for RX response. 
        //         }

        //        private void bgwk_DataWaitLoop_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //        {
        //             myGlobalBase.LoggerOpertaionMode = true;
        //             if (myUSB_Message_Manager.endoflinedetected == false)
        //             {
        //                 iFrameWaitLoop++;
        //                 if (iFrameWaitLoop >= myGlobalBase.WaitCyclePeriodSetting)
        //                 {
        //                     myUSB_Message_Manager.endoflinedetected = true;
        //                 }
        //                 else
        //                 {
        //                     bgwk_DataWaitLoop.RunWorkerAsync();         // Continue waiting for UART response 
        //                 }
        //             }
        //             if (myUSB_Message_Manager.endoflinedetected == true)            // End of Line detection. Do not ever change this to else otherwise it will not work!!!.
        //             {
        //                 myUSB_Message_Manager.endoflinedetected = false;
        //                 sMessageData = myUSB_Message_Manager.LoggerMessageRX;
        //                 myUSB_Message_Manager.LoggerMessageRX = "";
        //                 RecievedMessage_Async_Logger_Processing();
        //             }
        //       }
        #endregion

        #region //================================================================RecievedMessage_Async_Logger_Processing (inc SNAD)
        public delegate void RecievedMessage_Async_Logger_ProcessingDelegate();
        public void RecievedMessage_Async_Logger_Processing()      // Also apply to SNAD. 
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new RecievedMessage_Async_Logger_ProcessingDelegate(RecievedMessage_Async_Logger_Processing));
                return;
            }
            int y = 0;
            sMessageData = myUSB_Message_Manager.m_LoggerMessageRXtempII;
            string refdummy = null;
            if (myGlobalBase.LoggerAsyncMode == true)
            {
                if (iASyncAlertTimerSetting != -1)
                    iASyncAlertTimerCurrent = iASyncAlertTimerSetting;
                tbAlertTimer.Text = iASyncAlertTimerCurrent.ToString();
            }
            //--------------------------------------------------------------------------------------78F Added to support 0w/0q type
            if (myDetoken == null)
                myDetoken = new Frame_Detokeniser();
            else
                myDetoken.ClearAllVariable();
            //------------------------------------------------------------------------------------
            sMessageData = sMessageData.Replace(" ", "");               // remove ' '
            sMessageData = sMessageData.Replace("\r", "");              // remove \r
            //------------------------------------------------------------------------------------SNAD
            if (myGlobalBase.mySNADNetwork != null)
                myGlobalBase.mySNADNetwork.SNAD_Read_SNAD_From_LoggerCVS(sMessageData);         // Extract SNAD for $D/$H/$G/$T only. 
            //------------------------------------------------------------------------------------78F Added UDT protocol with 0w/0d token and decoder.
            if (rawDataignoreFormatFrameToolStripMenuItem.Checked == false)
            {
                if (myUDTmyFrameDataLog != null)
                    myUDTmyFrameDataLog.UDTDataLogProcessFrame(sMessageData);                       // Handle $T/$H/$D/$G/$T/$R/$S/$B/$E/$F with $T/$H detoken
            }
            else
            {
                if (myUDTmyFrameDataLog != null)
                    myUDTmyFrameDataLog.ClearAllVariable();
            }
            //------------------------------------------------------------------------------------LogMem Window. 
            if (myGlobalBase.ClientCodeWindowVisable == true)
                myClientCode.myClientCodeRecievedData(sMessageData);
            if (myGlobalBase.LogMemWindowVisable == true)
                myEEDownloadSurvey.SurveyLoggerCVS_RecievedData(sMessageData);
            //------------------------------------------------------------------------------------
            if (sMessageData.Contains("$T"))                        // Format Type
            {
                int start = sMessageData.IndexOf("$T");
                int end = sMessageData.IndexOf("$E") + 3;           // $E\n = +3. For CRC, it goes up to start of CC$E\n, so we need to deduct by 2. 
                int length = end - start;
                sMessageFormat = sMessageData.Substring(start, length);
                if (myCalc_CRC8.isContainCRC(sMessageFormat, ref refdummy) == true)
                {
                    myCalc_CRC8.UDTMessageFrameIsPassedCRC8(sMessageFormat);
                    tbCRCPassed.Enabled = false;
                    tbCRCFailed.Enabled = false;
                    tbCRCPassed.Text = myCalc_CRC8.CRC8PassCounter.ToString();
                    tbCRCFailed.Text = myCalc_CRC8.CRC8FailCounter.ToString();
                }
                Logger_OpenAppendCloseFile(true);                           // Append data into the file
                sMessageFormat = "";
            }
            if (sMessageData.Contains("$H"))                        // Header Type
            {
                int start = sMessageData.IndexOf("$H");
                int end = sMessageData.IndexOf("$E") + 3;
                sMessageHeader = sMessageData.Substring(start, end - start);
                if (myCalc_CRC8.isContainCRC(sMessageHeader, ref refdummy) == true)
                {
                    tbCRCPassed.Enabled = false;
                    tbCRCFailed.Enabled = false;
                    myCalc_CRC8.UDTMessageFrameIsPassedCRC8(sMessageHeader);
                    tbCRCPassed.Text = myCalc_CRC8.CRC8PassCounter.ToString();
                    tbCRCFailed.Text = myCalc_CRC8.CRC8FailCounter.ToString();
                }
                Logger_Update_Table_Header();
                Logger_OpenAppendCloseFile(true);                           // Append data into the file
                sMessageHeader = "";
                sMessageData = "";
            }
            if (sMessageData.Contains("$D"))       // Data Type
            {
                sMessageData = sMessageData.Replace("\n", "");          // remove \n
                if (myCalc_CRC8.isContainCRC(sMessageData, ref refdummy) == true)
                {
                    myCalc_CRC8.UDTMessageFrameIsPassedCRC8(sMessageData);
                    tbCRCPassed.Text = myCalc_CRC8.CRC8PassCounter.ToString();
                    tbCRCFailed.Text = myCalc_CRC8.CRC8FailCounter.ToString();
                }
                if (btnAsyncSuspendUpdate.Text == "Hold List")
                {
                    int DeTokenErrorCode;
                    if (myUDTmyFrameDataLog.isFormatFrameFound==true)
                    {
                        DeTokenErrorCode = myDetoken.DetokeniserMessageLogStyle(sMessageData, myUDTmyFrameDataLog.FormatColumn, ref y);
                        Logger_Async_Update_Table_Data(0, true);
                    }
                    else
                    {
                        Logger_Async_Update_Table_Data(0,false);                  // Update the table Column 2  
                    }
                }
                //---------------------------------------------------------------------------------------------78JD
                if (tmsi_FormattedCVStoFile.Checked==true)
                    sMessageData = myDetoken.sDataFileOut;
                if (tsmi_SplitDataToTwoColumn.Checked==true)
                {
                    sMessageData = myDetoken.RFDPPD_FormatDataIntoTwoColumn();
                }
                //---------------------------------------------------------------------------------------------
                Logger_OpenAppendCloseFile(true);                       // Append data into the file  
                //---------------------------------------------------------------------------------------------
                sMessageData = "";
            }
            if (sMessageData.Contains("$G"))       // Data Type
            {
                sMessageData = sMessageData.Replace("\n", "");          // remove \n
                if (myCalc_CRC8.isContainCRC(sMessageData, ref refdummy) == true)
                {
                    myCalc_CRC8.UDTMessageFrameIsPassedCRC8(sMessageData);
                    tbCRCPassed.Text = myCalc_CRC8.CRC8PassCounter.ToString();
                    tbCRCFailed.Text = myCalc_CRC8.CRC8FailCounter.ToString();
                }
                if (btnAsyncSuspendUpdate.Text == "Hold List")
                {
                    int DeTokenErrorCode;
                    if (myUDTmyFrameDataLog.isFormatFrameFound == true)
                    {

                        DeTokenErrorCode = myDetoken.DetokeniserMessageLogStyle(sMessageData, myUDTmyFrameDataLog.FormatColumn, ref y);
                        Logger_Async_Update_Table_Data(1, true);
                    }
                    else
                    {
                        Logger_Async_Update_Table_Data(1, false);                  // Update the table Column 2  
                    }
                }
                Logger_OpenAppendCloseFile(true);                       // Append data into the file
                sMessageData = "";
            }
            bDataFrameTestOnly = true;
            Download_Data_Processor();                                  // Keep in Logging for more data.
        }
        #endregion

        #region //================================================================RecievedMessage_Sync_Logger_Processing

        private void RecievedMessage_Sync_Logger_Processing()      // Updated 19th March 2016, accept $D and $G as un Async. 
        {
            //----------------------------------------------------------Clean up Message 
            sMessageData = sMessageData.Replace("<ACK>", "");      // remove -<ACK> message.
            sMessageData = sMessageData.Replace("\r", "");         // remove \r
            sMessageData = sMessageData.Replace("\n", "");         // remove \n 
            sMessageData = sMessageData.Replace(" ", "");          // remove ' '
            sMessageData = sMessageData.Replace("\u0006", "");     // remove \<ACK>
            sMessageData = sMessageData.Replace("<LOGON>$E", "");  // remove \<ACK>

            if ((myGlobalBase.WaitCyclePeriodSetting == 0) || (iFrameWaitLoop < myGlobalBase.WaitCyclePeriodSetting))    // Change from 100 11/Dec/17
            {
                rtbConsoleAppend("-INFO : Data Responded : WaitLoop:" + iFrameWaitLoop.ToString());
                Logger_OFF_Command();
                if (sMessageData.Contains("$D"))                            // Data Type
                {
                    Logger_Async_Update_Table_Data(0,false);                      // Update the table Column 2    
                }
                if (sMessageData.Contains("$G"))                            // Data2 Type
                {
                    Logger_Async_Update_Table_Data(1,false);                      // Update the table Column 3     
                }
                Logger_OpenAppendCloseFile(false);                          // Append data into the file
                rtbConsole.Refresh();
            }
            else
            {
                rtbConsoleAppend("\n#ERR : No Data Response Message, exceeded WaitLoop:" + iFrameWaitLoop.ToString());
                rtbConsole.Refresh();
                sMessageData = "Error No Data: " + sMessageData;
                Logger_OFF_Command();
                if (sMessageData.Contains("$D"))                            // Data Type
                {
                    Logger_Async_Update_Table_Data(0,false);                      // Update the table Column 2    
                }
                if (sMessageData.Contains("$G"))                            // Data2 Type
                {
                    Logger_Async_Update_Table_Data(1,false);                      // Update the table Column 3     
                }         // Update the table
                Logger_OpenAppendCloseFile(false);                          // Append data into the file
                rtbConsole.Refresh();
            }
        }
        #endregion

        //================================================================================================
        #region //================================================================Download_Format_Processor  Added: 18/Jan/19 UDT76L
        //================================================================================================
        private void Download_Format_Processor()
        {
            try
            {
                if (myGlobalBase.LoggerAsyncMode == true)
                {
                    //-----------------------------------------------################# ASync Logger Section Added: 31/1/16
                    myUSB_Message_Manager.LoggerMessageRX = "";
                    m_sLoggerResponseTxt = "";
                    myGlobalBase.LoggerOpertaionMode = true;
                    myUSB_Message_Manager.endoflinedetected = false;
                    iFrameWaitLoop = 0;
                    if (bgwk_DataWaitLoop.IsBusy == false)      // This fix thread issue.
                        bgwk_DataWaitLoop.RunWorkerAsync();     // Now we wait for response, using background worker to keep window form responsive to user. 
                    return;
                }
                if (myGlobalBase.LoggerAsyncSnadMode == true)
                {
                    //-----------------------------------------------################# ASync SNAD Logger Section Added: 07/01/19
                    myUSB_Message_Manager.LoggerMessageRX = "";
                    m_sLoggerResponseTxt = "";
                    myGlobalBase.LoggerOpertaionMode = true;
                    myUSB_Message_Manager.endoflinedetected = false;
                    iFrameWaitLoop = 0;
                    if (bgwk_DataWaitLoop.IsBusy == false)      // This fix thread issue.
                        bgwk_DataWaitLoop.RunWorkerAsync();     // Now we wait for response, using background worker to keep window form responsive to user. 
                    return;
                }
            }
            catch
            {
                rtbConsoleAppend("##ERR: RunWorkerAsync thread is busy or crashed, reboot UDT and please restrict single active operation under ASYNC or SYNC but do not alternate!");
            }
        }
        #endregion

        //================================================================================================
        #region //================================================================Download_Header_Processor
        //================================================================================================
        private void Download_Header_Processor()
        {
            // This download the header file frame from the device where new CSV file is created
            // It is essential to have 1st format before rest of the abitary header
            // $H,SYSTICK,SNAD, and then user header details with comma separator (",") , ie FADC,ADC01, ADC02, ADC03,.....etc
            // Must have '\n' end terminator.
            // on reception, it create and open new time-stamped file along with header as exact received message including $H. 
            // it then configure the code to use this new file.
            // There is two way to implement this
            //      Send request command "LOGGER_$H(FF)"      (NB: CAN ESD/OD under 0x0F, details TBA)
            //      Optionally send $H command at any time! (not supported yet).
            if (Logger_ON_Command() == false) return;
            myUSB_Message_Manager.LoggerMessageRX = "";
            m_sLoggerEntryTxt = "LOGGER_$H(0xFF)";          // Changed 13/2/16 remove \n statement. Need to check for PIC later
            m_sLoggerResponseTxt = "";
            myGlobalBase.LoggerOpertaionMode = true;
            myUSB_Message_Manager.endoflinedetected = false;
            Logger_Command_ASCII_Process();                // UART only!            
            iFrameWaitLoop = 0;
            try
            {
                bgwk_HeaderWaitLoop.RunWorkerAsync();     // Now we wait for response, using background worker to keep window form responsive to user.  
            }
            catch { }
        }

        private void bgwk_HeaderWaitLoop_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(10);        //10mSec sleep while waiting for RX response. 
        }

        private void bgwk_HeaderWaitLoop_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            myGlobalBase.LoggerOpertaionMode = true;
            if (myUSB_Message_Manager.endoflinedetected == false)
            {
                iFrameWaitLoop++;
                if (iFrameWaitLoop >= 100)
                {
                    myUSB_Message_Manager.endoflinedetected = true;     // Set for timeout...
                    //TODO stop loop for response failure.
                }
                bgwk_HeaderWaitLoop.RunWorkerAsync();         // Continue waiting for UART response 
            }
            else
            {
                myUSB_Message_Manager.endoflinedetected = false;
                if (iFrameWaitLoop >= 50)
                {
                    rtbConsoleAppend("\n#ERR : No Header Response Message, exceeded WaitLoop:" + iFrameWaitLoop.ToString());
                    return;                // No data arrived, no point to continue.
                }
                rtbConsoleAppend("-INFO : Header Responded: WaitLoop:" + iFrameWaitLoop.ToString());
                sMessageHeader = myUSB_Message_Manager.LoggerMessageRX;
                myUSB_Message_Manager.LoggerMessageRX = "";
                //----------------------------------------------------------Clean up Message 
                sMessageHeader = sMessageHeader.Replace("<ACK>", "");      // remove -<ACK> message.
                sMessageHeader = sMessageHeader.Replace("\r", "");         // remove \r
                sMessageHeader = sMessageHeader.Replace("\n", "");         // remove \n 
                sMessageHeader = sMessageHeader.Replace(" ", "");          // remove \n 
                sMessageHeader = sMessageHeader.Replace("\u0006", "");     // remove \<ACK>
                Logger_OFF_Command();
                Logger_Update_Table_Header();
                rtbConsole.Refresh();
                //Logger_OpenNewFile();
            }
        }

        #endregion

        //================================================================================================
        #region //================================================================Download_Null_Processor
        //================================================================================================
        private void Download_Null_Processor()
        {
            // This command configure device for logger operation as well as null command.
            // This command need to be implemented 1st and expect response based on systick and ACK, before using header or data stream. 
            // Request command "LOGGER_$N(FF)"      (NB: CAN ESD/OD under 0x0F, details TBA)
            // Response goes like this "$N,SYSTICK,SNAD ACKGO" where 
            //          SNAD is Slave Node Address Device, were FF is declared not supported (default). This is for network approach of many board testing.   
            //          SYSTICK is UINT32 in ASCII HEX and 
            //          ACKGO is ASC message to confirm Logger operation
            // Must have '\n' end terminator. 
            // if the response is "$N,00112345,FF,ACKGO" then is ready for header or data frame next
            // if the response is "$N,00112345,FF,NACK" then it is not ready for header or data. 
            // NB: SNAD response is generally ignored but recommend to use FF (which imply all network request/response). 
            if (Logger_ON_Command() == false) return;
            m_sLoggerEntryTxt = "LOGGER_$N(0xFF)";          // Removed \n
            m_sLoggerResponseTxt = "";
            myGlobalBase.LoggerOpertaionMode = true;
            myUSB_Message_Manager.endoflinedetected = false;
            Logger_Command_ASCII_Process();         // UART only!
            int waitloop = 0;
            while (waitloop <= 100)
            {
                if (myUSB_Message_Manager.endoflinedetected == true) break;
                waitloop++;
                Thread.Sleep(1);        //1mSec sleep while waiting for RX response. 
            }
            m_sLoggerResponseTxt = myUSB_Message_Manager.LoggerMessageRX;       //1F/1G change from discontinue 'MessageRX'. Hope this work....
            myUSB_Message_Manager.endoflinedetected = false;
            if (waitloop == 100)
            {
                rtbConsoleAppend("#ERR : No Response Message, exceeded WaitLoop:" + iFrameWaitLoop.ToString());
                return;                // No data arrived, no point to continue.
            }
            rtbConsoleAppend("-INFO : Null Responded: WaitLoop:" + waitloop.ToString() + ". MSG:" + m_sLoggerResponseTxt);
            Logger_OFF_Command();
            rtbConsole.Refresh();
            if (m_sLoggerResponseTxt == "")
            {
                return; // no message response.
            }

            // Validate message here.
            // expect systick with ACK or NACK. 

        }
        #endregion

        //================================================================================================
        #region //================================================================Message_Loop_Master
        //================================================================================================
        // This routine is responsible for doing operation on each channel and pause for a while.
        public void Logger_Loop_Master()
        {
            bPauseButtonClicked = false;
            //Download_Null_Processor();          // validate connection (UART)
            Download_Data_Processor();
            //string sTimeStampNow = DateTime.Now.ToString("dd_MM_yy_hh_mm_ss");
            //rtbConsoleAppend("-INFO : Started Logger Scan, Time/Date:" + sTimeStampNow);
            bgwk2.RunWorkerAsync();         // Use background worker thread for background process operation (it worked very well, so far so good, let hold up promise!)
        }
        #endregion

        //================================================================================================
        #region //================================================================bgwk2_RunWorkerCompleted (wait period between SNAD scan operation)
        //================================================================================================
        private void bgwk2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((myGlobalBase.LoggerAsyncMode == true) & (iASyncAlertTimerSetting != -1))
            {
                if (isASYNC_STCSTOP_occurred == true)
                {
                    isASYNC_STCSTOP_occurred = false;
                    return;
                }
                iASyncAlertTimerCurrent--;
                if (iASyncAlertTimerCurrent == 0)
                {
                    //-------------------------Alert Signal.
                    for (int i = 0; i < 3; i++)
                    {
                        Console.Beep(1000, 200);
                        Thread.Sleep(20);
                        Console.Beep(2500, 100);
                        Thread.Sleep(20);
                        Console.Beep(750, 200);
                        Thread.Sleep(20);
                        Console.Beep(2000, 100);
                        Thread.Sleep(20);
                    }
                    //-------------------------
                    iAsyncAlertTimerCount++;
                    iASyncAlertTimerCurrent = iASyncAlertTimerSetting;      // Reset Count
                    //Alert Message goes here.
                    string sTimeStampNow = DateTime.Now.ToString("dd_MM_yy_hh_mm_ss");
                    rtbConsoleAppend("#ERR: Expected ASYNC Data did not arrived, Count:" + iAsyncAlertTimerCount.ToString() + " || TimeStamp:" + sTimeStampNow);
                }
                tbAlertTimer.Text = iASyncAlertTimerCurrent.ToString();
                tbAlertTimer.Refresh();
                bgwk2.RunWorkerAsync();
                return;
            }
            //-----------------------------------------------------------ASYNC Only
            if (myGlobalBase.LoggerAsyncMode == true)
            {
                BatchWaitPeriod--;
                txtCountDown.Text = BatchWaitPeriod.ToString();
                txtCountDown.Refresh();
                if (BatchWaitPeriod == 0)
                {
                    //string sTimeStampNow = DateTime.Now.ToString("dd_MM_yy_hh_mm_ss");
                    //rtbConsoleAppend("-INFO : Started Logger Scan, Time/Date:" + sTimeStampNow);
                    // Process Logger transfer operation 
                    BatchWaitPeriod = Convert.ToUInt32(txtCyclePeriod.Text, 10);
                    txtCountDown.Text = BatchWaitPeriod.ToString();
                    Download_Data_Processor();
                    //-------------------------------Stop Counter
                    if (iStopCounter != -1)
                    {
                        iStopCounter--;
                        txStopCounter.Text = iStopCounter.ToString();
                        if (iStopCounter == 0)
                        {
                            txStopCounter.Text = iStopCounterBackup.ToString();
                            bPauseButtonClicked = true;
                        }
                    }

                    if (bPauseButtonClicked == true)
                    {
                        rtbConsoleAppend("-INFO : Clicked Paused/Stopped");
                    }
                    else
                    {
                        bgwk2.RunWorkerAsync();
                    }
                }
                else
                {
                    if (bPauseButtonClicked == true)
                    {
                        rtbConsoleAppend("-INFO : Clicked Paused/Stopped");
                    }
                    else
                    {
                        bgwk2.RunWorkerAsync();         // Sleep for 1 second on separate thread. 
                    }
                }
            }
            //-----------------------------------------------------------ASYNC SNAD Only, Timeout is not supported or developed later. 

        }
        #endregion

        //================================================================================================
        #region //================================================================bgwk2_DoWork (wait period)
        //================================================================================================
        private void bgwk2_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }
        #endregion

        //================================================================================================
        #region //================================================================Logger_Command_ASCII_Process() without prefix // Also process CANUSB & USBUART
        //================================================================================================
        private void Logger_Command_ASCII_Process()
        {
            m_sLoggerResponseTxt = "";
            myGlobalBase.LoggerOpertaionMode = true;
            //---------------------------------------------------------------------- SNAD Address added to command message Mods. 
            if (myGlobalBase.mySNADNetwork != null)
            {
                m_sLoggerEntryTxt = myGlobalBase.mySNADNetwork.SNAD_Modify_UDT_Command(m_sLoggerEntryTxt);
            }
            //-----------------------------------------------------------------------
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == true)
            {
                myUSBVCOMComm.VCOMArray_Message_Send(m_sLoggerEntryTxt, (int)eUSBDeviceType.ADIS);
            }
            else if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == true)
            {
                myUSBVCOMComm.VCOMArray_Message_Send(m_sLoggerEntryTxt, (int)eUSBDeviceType.BGDRILLING);
            }
            else if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == true)     // Rev 72. 
            {
                myUSBVCOMComm.VCOMArray_Message_Send(m_sLoggerEntryTxt, (int)eUSBDeviceType.MiniAD7794);
            }
            else if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM)
            {
                myUSBVCOMComm.VCOM_Message_Send(m_sLoggerEntryTxt);
            }
            else if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_FTDI)
            {
                //### Check if dsPIC33 is actually connected, if not reject command....this need review/optional.
                myUSBComm.FTDI_Message_Send(m_sLoggerEntryTxt);
            }
        }
        #endregion

        //================================================================================================
        #region //================================================================MiniAD7794_Logger_Command_ASCII_Process() without prefix // Also process CANUSB & USBUART
        //================================================================================================
        public bool MiniAD7794_Logger_Command_ASCII_Process(string LoggerEntryTxt)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == true)
            {
                m_sLoggerEntryTxt = LoggerEntryTxt;
                m_sLoggerResponseTxt = "";
                myGlobalBase.LoggerOpertaionMode = true;
                myUSBVCOMComm.VCOMArray_Message_Send(m_sLoggerEntryTxt, (int)eUSBDeviceType.MiniAD7794);
                return (true);
            }
            return (false);
        }
        #endregion

        //================================================================================================
        #region //================================================================Logger_ON_Command
        //================================================================================================
        private bool Logger_ON_Command()
        {
            myGlobalBase.LoggerOpertaionMode = true;
            myUSB_Message_Manager.endoflinedetected = false;
            m_sLoggerEntryTxt = "LOGGER_ON(0xFF)";
            Logger_Command_ASCII_Process();
            Thread.Sleep(50);                      // Check for response (later task)
            return true;
        }
        #endregion

        //================================================================================================
        #region //================================================================Logger_OFF_Command
        //================================================================================================
        private void Logger_OFF_Command()
        {
            myUSB_Message_Manager.endoflinedetected = false;
            m_sLoggerEntryTxt = "LOGGER_OFF(0xFF)";
            Logger_Command_ASCII_Process();
            Thread.Sleep(20);                      // Check for response (later task)
            //myGlobalBase.LoggerOpertaionMode = false;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### DGV
        //#####################################################################################################

        #region //================================================================Logger_Update_Table_Header
        void Logger_Update_Table_Format()
        {
            try
            {
                char[] delimiterChars = { ',', ';' };                    // Modified 31/1/16. 
                string[] sTokenHeaders = sMessageHeader.Split(delimiterChars);
                int count = sTokenHeaders.Length;
                for (int row = 0; row < count; row++)
                {
                    if (row < this.dgvMessage.RowCount)         // avoid error!
                    {
                        this.dgvMessage[1, row].Value = sTokenHeaders[row];
                    }
                    else
                    {
                        rtbConsoleAppend("#ERR : Corrupted FormatFrame, too many element in row (Logger_Update_Table_Format())");
                    }
                }
            }
            catch (Exception ex)
            {
                rtbConsoleAppend("#ERR : Logger_Update_Table_Header() Exception:" + ex.ToString());
            }
        }
        #endregion

        #region //================================================================Logger_Update_Table_Header
        void Logger_Update_Table_Header()
        {
            try
            {
                for (int i = 0; i < this.dgvMessage.RowCount; i++)      //Clean up the table before use. 
                {
                    this.dgvMessage[1, i].Value = "";
                    this.dgvMessage[2, i].Value = "";
                    this.dgvMessage[3, i].Value = "";
                }
                char[] delimiterChars = { ',', ';' };                    // Modified 31/1/16. 
                string[] sTokenHeaders = sMessageHeader.Split(delimiterChars);
                int count = sTokenHeaders.Length;
                for (int row = 0; row < count; row++)
                {
                    if (row < this.dgvMessage.RowCount)         // avoid error!
                    {
                        this.dgvMessage[1, row].Value = sTokenHeaders[row];
                    }
                    else
                    {
                        rtbConsoleAppend("#ERR : Corrupted HeaderFrame, too many element in row (Logger_Update_Table_Header())");
                    }
                }
            }
            catch (Exception ex)
            {
                rtbConsoleAppend("#ERR : Logger_Update_Table_Header() Exception:" + ex.ToString());
            }
        }
        #endregion

        #region //================================================================Logger_Update_Table_Data (Old Code)
        void Logger_Update_Table_Data()
        {
            if (sMessageHeader == null)
                return;
            try
            {
                for (int i = 0; i < this.dgvMessage.RowCount; i++)      //Clean up the table before use. 
                {
                    this.dgvMessage[2, i].Value = "";
                    this.dgvMessage[3, i].Value = "";
                }
                char[] delimiterChars = { ',', ';' };                    // Modified 31/1/16. 
                string[] sTokenDatas = sMessageData.Split(delimiterChars);
                string[] sTokenHeaders = sMessageHeader.Split(delimiterChars);
                int count = sTokenDatas.Length;
                for (int row = 0; row < count; row++)
                {
                    if (row < this.dgvMessage.RowCount)         // avoid error!
                    {
                        this.dgvMessage[2, row].Value = sTokenDatas[row];
                        sTokenHeaders[row] = sTokenHeaders[row].Trim();
                    }
                    else
                    {
                        rtbConsoleAppend("#ERR : Corrupted DataFrame, too many element in row (Logger_Update_Table_Data())");
                    }
                }
            }
            catch (System.Exception ex2)
            {
                rtbConsoleAppend("#ERR : Issue with response message during token process " + ex2.ToString());
            }

        }
        #endregion

        #region //================================================================Logger_Async_Update_Table_Data
        void Logger_Async_Update_Table_Data(int mode, bool isUDTProtocolFormatter)
        {
            //-----------------------------------------------------Added 78F.
            try
            {
                for (int i = 0; i < this.dgvMessage.RowCount; i++)      //Clean up the table before use. 
                {
                    if (mode == 0) this.dgvMessage[2, i].Value = "";
                    if (mode == 1) this.dgvMessage[3, i].Value = "";
                }
                //-----------------------------------------------------------------------
                if (isUDTProtocolFormatter == true)     // Already converted proto data (with format). 
                {
                    for (int row = 0; row < myDetoken.sData.Length; row++)
                    {
                        if (mode == 0)      // $D goes here.
                        {
                            if (row < this.dgvMessage.RowCount)         // avoid error!
                            {
                                this.dgvMessage[2, row].Value = myDetoken.sData[row];
                            }
                            else
                            {
                                rtbConsoleAppend("#ERR : Corrupted DataFrame, too many element in row (Logger_Update_Table_Data())");
                            }
                        }
                        if (mode == 1)      // $G goes here
                        {
                            if (row < this.dgvMessage.RowCount)         // avoid error!
                            {
                                this.dgvMessage[3, row].Value = myDetoken.sData[row];
                            }
                            else
                            {
                                rtbConsoleAppend("#ERR : Corrupted DataFrame, too many element in row (Logger_Update_Table_Data())");
                            }
                        }
                    }
                }
                else
                {
                    char[] delimiterChars = { ',', ';' };                    // Modified 31/1/16. 
                    string[] sTokenDatas = sMessageData.Split(delimiterChars);
                    int count = sTokenDatas.Length;
                    for (int row = 0; row < count; row++)
                    {
                        if (mode == 0)      // $D goes here.
                        {
                            if (row < this.dgvMessage.RowCount)         // avoid error!
                            {
                                this.dgvMessage[2, row].Value = sTokenDatas[row];
                            }
                            else
                            {
                                rtbConsoleAppend("#ERR : Corrupted DataFrame, too many element in row (Logger_Update_Table_Data())");
                            }
                        }
                        if (mode == 1)      // $G goes here
                        {
                            if (row < this.dgvMessage.RowCount)         // avoid error!
                            {
                                this.dgvMessage[3, row].Value = sTokenDatas[row];
                            }
                            else
                            {
                                rtbConsoleAppend("#ERR : Corrupted DataFrame, too many element in row (Logger_Update_Table_Data())");
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex2)
            {
                rtbConsoleAppend("#ERR : Issue with response message during token process " + ex2.ToString());
            }

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Files
        //#####################################################################################################

        #region //================================================================Logger_SNAD_OpenNewFile
        private bool Logger_SNAD_OpenNewFile()
        {
            string sFilenameII;
            string sTimeStampNow = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
            string sTimeStampHeader = String.Empty;
            sFilename = "_Data_" + sTimeStampNow + ".csv";
            sFilenameII = System.IO.Path.Combine(sFoldername, sFilename);       // Create new filename with new timestamp on it.
            txtFilename.Text = sFilenameII;
            txtFilename.Refresh();
            sTimeStampHeader = "[MM_dd_yyyy],[hh_mm_ss],[DateStampTick]," + sMessageHeader + "\n";
            //==========================================Create new AppendNoteFileName
            try
            {
                FileStream fsa = null;
                try
                {
                    fsa = new FileStream(sAppendFileName, FileMode.Append, FileAccess.Write, FileShare.Write);
                }
                finally
                {
                    if (fsa != null)
                    {
                        fsa.Dispose();
                    }
                    rtbConsoleAppend("-INFO : Created new File+AppendNote: " + sAppendFileName);
                }
            }
            catch
            {
                rtbConsoleAppend("#ERR : Unable to create AppendNote File: " + sAppendFileName);
                rtbConsoleAppend("+WARN: Do not save file into Drive C except user folder");
                return false;
            }
            //-------------------------------Create new set of file, each one for SNAD. 
            try
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(sFilenameII, FileMode.Append, FileAccess.Write, FileShare.Write);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                }
            }
            catch
            {
                rtbConsoleAppend("#ERR : Unable to create File: " + sFilenameII);
                rtbConsoleAppend("+WARN: Do not save file into Drive C except user folder");
                return false;
            }
            //-------------------------------Write Header Message to the existing file
            //             if (isAsync == false)
            //             {
            try
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(sFilenameII, true, Encoding.ASCII);
                    sw.Write(sTimeStampHeader);                   // Header Message
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Dispose();
                    }
                    rtbConsoleAppend("-INFO : Created new File+Header: " + sFilenameII);
                }
            }
            catch (Exception)
            {
                rtbConsoleAppend("#ERR : Unable to append header File: " + sFilenameII);
                return false;
            }
            //            }
            return true;
        }
        #endregion

        #region //================================================================Logger_OpenNewFile
        private bool Logger_OpenNewFile(bool isAsync)
        {
            if (isAsync == false)
            {
                if (bDataFrameTestOnly == true)
                    return false;
            }
            string sFilenameII;
            string sTimeStampNow = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
            string sTimeStampHeader = String.Empty;
            sFilename = "_Data_" + sTimeStampNow + ".csv";
            sFilenameII = System.IO.Path.Combine(sFoldername, sFilename);       // Create new filename with new timestamp on it.
            txtFilename.Text = sFilenameII;
            txtFilename.Refresh();
            sTimeStampHeader = "[MM_dd_yyyy],[hh_mm_ss],[DateStampTick]," + sMessageHeader + "\n";
            //==========================================Create new AppendNoteFileName
            try
            {
                FileStream fsa = null;
                try
                {
                    fsa = new FileStream(sAppendFileName, FileMode.Append, FileAccess.Write, FileShare.Write);
                }
                finally
                {
                    if (fsa != null)
                    {
                        fsa.Dispose();
                    }
                    rtbConsoleAppend("-INFO : Created new File+AppendNote: " + sAppendFileName);
                }
            }
            catch
            {
                rtbConsoleAppend("#ERR : Unable to create AppendNote File: " + sAppendFileName);
                rtbConsoleAppend("+WARN: Do not save file into Drive C except user folder");
                return false;
            }
            //-------------------------------Create new set of file, each one for SNAD. 
            try
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(sFilenameII, FileMode.Append, FileAccess.Write, FileShare.Write);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                }
            }
            catch
            {
                rtbConsoleAppend("#ERR : Unable to create File: " + sFilenameII);
                rtbConsoleAppend("+WARN: Do not save file into Drive C except user folder");
                return false;
            }
            //-------------------------------Write Header Message to the existing file
            if (isAsync == false)
            {
                try
                {
                    StreamWriter sw = null;
                    try
                    {
                        sw = new StreamWriter(sFilenameII, true, Encoding.ASCII);
                        sw.Write(sTimeStampHeader);                   // Header Message
                    }
                    finally
                    {
                        if (sw != null)
                        {
                            sw.Dispose();
                        }
                        rtbConsoleAppend("-INFO : Created new File+Header: " + sFilenameII);
                    }
                }
                catch (Exception)
                {
                    rtbConsoleAppend("#ERR : Unable to append header File: " + sFilenameII);
                    return false;
                }
            }
            return true;

        }
        #endregion

        #region //================================================================Message_AppendFile
        void Logger_OpenAppendCloseFile(bool Async)
        {
            if (Async == false)
            {
                if (bDataFrameTestOnly == true)
                    return;
            }
            // The sfilename already have timestamp within.
            string sFilenameII;
            string sTimeStampHeader = DateTime.Now.ToString("dd/MM/yyyy,HH:mm:ss,");
            string sTimeStampTick = DateTime.Now.Ticks.ToString();

            sFilenameII = System.IO.Path.Combine(sFoldername, sFilename);
            txtFilename.Text = sFilenameII;
            txtFilename.Refresh();
            try
            {
                StreamWriter sw = null;
                try
                {
                    if (tsmi_SplitDataToTwoColumn.Checked == true)
                    {
                        sTimeStampHeader = ",,,"+sMessageData;
                    }
                    else
                    {
                        sTimeStampHeader = sTimeStampHeader + sTimeStampTick + "," + sMessageData + "\n";
                    }
                    sw = new StreamWriter(sFilenameII, true, Encoding.ASCII);
                    sw.Write(sTimeStampHeader);                                     // Dataframe (assuming it has \n\0 terminator)
                    rtbConsoleAppend("-INFO : Appended Data to File: " + sFilenameII);
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Dispose();
                    }
                }
            }
            catch (Exception X)
            {
                rtbConsole.Text += "#ERR: Unable to append file : " + sFilenameII + " Error Msg:" + X.ToString();
            }
        }
        #endregion

        #region //================================================================Message_AppendNoteFile
        void Logger_OpenAppendNoteCloseFile()
        {
            if (bDataFrameTestOnly == true) return;
            // The sfilename already have timestamp within.
            string sFilenameII;
            string sTimeStampHeader = DateTime.Now.ToString("dd/MM/yyyy,HH:mm:ss,");      //m_Endtime = DateTime.Now.Ticks
            string sTimeStampTick = DateTime.Now.Ticks.ToString();
            sFilenameII = System.IO.Path.Combine(sFoldername, sFilename);
            txtFilename.Text = sFilenameII;
            txtFilename.Refresh();
            try
            {
                StreamWriter sw = null;
                try
                {
                    sTimeStampHeader = sTimeStampHeader + sTimeStampTick + ",$A," + sMessageNote + "\n";
                    sw = new StreamWriter(sAppendFileName, true, Encoding.ASCII);
                    sw.Write(sTimeStampHeader);                                     // Dataframe (assuming it has \n\0 terminator)
                    rtbConsoleAppend("-INFO : Appended Data to File: " + sFilenameII);
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Dispose();
                    }
                }
            }
            catch (Exception X)
            {
                rtbConsole.Text += "#ERR: Unable to append file : " + sFilenameII + " Error Msg:" + X.ToString();
            }
        }
        #endregion

        #region //================================================================rtbAppendBox_KeyPress
        private void rtbAppendBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (bAppendedText == true)
            {
                rtbAppendBox.ForeColor = Color.White;
                rtbAppendBox.Refresh();
                bAppendedText = false;
                sMessageNote = "";
            }
            if (e.KeyChar == 13)
            {
                rtbAppendBox.ForeColor = Color.Yellow;
                rtbAppendBox.Text = rtbAppendBox.Text.Replace("\n", "");
                rtbAppendBox.Text = rtbAppendBox.Text.Replace("\r", "");
                rtbAppendBox.Refresh();
                sMessageNote = rtbAppendBox.Text;
                Logger_OpenAppendNoteCloseFile();
                e.Handled = true;
                bAppendedText = true;
            }
        }
        #endregion

        #region //================================================================Logger_Console_OpenNewFile
        private void Logger_Console_OpenNewFile()
        {
            string sTimeStampNow = DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss");
            string sFilenameConsole = "_Console_" + sTimeStampNow + ".txt";
            string sFilenameII = System.IO.Path.Combine(sFoldername, sFilenameConsole);       // Create new filename with new timestamp on it.
            //-------------------------------Create new set of file, each one for SNAD. 
            try
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(sFilenameII, FileMode.Append, FileAccess.Write, FileShare.Write);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                }
            }
            catch (Exception x)
            {
                rtbConsoleAppend("#ERR : Unable to create console File: " + sFilenameII + " Exception" + x.ToString());
            }
            //-------------------------------Write Header Message to the existing file
            try
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(sFilenameII, true, Encoding.ASCII);
                    sw.Write(rtbConsole.Text + Environment.NewLine);                                      // Console Message
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                rtbConsoleAppend("#ERR : Unable to append console File: " + sFilenameII);
            }

        }
        #endregion

        #region //================================================================Logger_Console_OpenOneFilename
        private void Logger_Console_OpenOneFilename()
        {
            string sFilenameII = System.IO.Path.Combine(sFoldername, sAppendConsoleOneFileName);
            //==========================================Create new AppendNoteFileName
            try
            {
                FileStream fsa = null;
                try
                {
                    fsa = new FileStream(sFilenameII, FileMode.Append, FileAccess.Write, FileShare.Write);
                }
                finally
                {
                    if (fsa != null)
                    {
                        fsa.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                rtbConsoleAppend("#ERR : Unable to create AppendNote File: " + sFilenameII + ",Exception" + ex.ToString());
            }
        }
        #endregion

        #region //================================================================Logger_Console_AppendOneFilename
        private void Logger_Console_AppendOneFilename()
        {
            string sFilenameII = System.IO.Path.Combine(sFoldername, sAppendConsoleOneFileName);       // Create new filename with new timestamp on it.
            //-------------------------------Write Header Message to the existing file
            try
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(sFilenameII, true, Encoding.ASCII);
                    sw.Write(rtbConsole.Text + Environment.NewLine);                                      // Console Message
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                rtbConsoleAppend("#ERR : Unable to append console File: " + sFilenameII);
            }

        }
        #endregion

        #region //================================================================BtnSaveConsole_Click
        private void BtnSaveConsole_Click(object sender, EventArgs e)
        {
            try
            {
                string archiveFileName = DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + "__ConsoleLog.txt";
                sFilenameConsole = System.IO.Path.Combine(sFoldername, archiveFileName);
                rtbConsole.SaveFile(sFilenameConsole, RichTextBoxStreamType.PlainText);
                rtbConsole.Text = "~TASK: Console Text saved into Time Stamped Filename ::" + sFilenameConsole;
            }
            catch (System.Exception ex)
            {
                rtbConsole.Text += "#ERR: Unable to save console text into file : " + sFilenameConsole + " Error Msg:" + ex.ToString() + " : Log file is lost, continue operation";
            }

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Controls
        //#####################################################################################################

        #region //================================================================btnFolder_Click
        private void btnFolder_Click(object sender, EventArgs e)
        {
            //
            // This event handler was created by double-clicking the window in the designer.
            // It runs on the program's startup routine.
            //
            bDataFrameTestOnly = false;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                //
                // The user selected a folder and pressed the OK button.
                // We print the number of files found.
                //
                sFoldername = folderBrowserDialog1.SelectedPath;
                if (!Directory.Exists(sFoldername))
                {
                    DirectoryInfo di = Directory.CreateDirectory(sFoldername);  // Create folder if not exist. 
                }
            }
            txtFolderName.Text = sFoldername;
            myGlobalBase.sFoldername = sFoldername;
            myGlobalBase.sSessionFoldername = sFoldername;
            txtFilename.Text = "To be Generated";

            sAppendFileName = System.IO.Path.Combine(sFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_AppendNote.csv"));
            sErrorFileName = System.IO.Path.Combine(sFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_ExceptionReport.txt"));
        }
        #endregion

        #region //================================================================btnStart_Click
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.LoggerAsyncSnadMode == true)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("This is ASYNC SNAD Logging Mode, Cancel request", "Operation ERROR", 10);
                return;
            }
            if (myGlobalBase.LoggerAsyncMode == true)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("This is ASYNC Logging Mode, Cancel request", "Operation ERROR", 10);
                return;
            }
            bPauseButtonClicked = false;
            bDataFrameTestOnly = false;

            txtCyclePeriod.ReadOnly = false;
            btnFolder.Enabled = false;
            btnTestHeader.Enabled = false;
            btnDataFrame.Enabled = false;
            btnPause.Enabled = true;
            //--------------------------------------------------------------StopCounter. 
            if (Tools.IsString_Numberic_Int32(txStopCounter.Text) == true)
            {
                iStopCounter = Tools.ConversionStringtoInt32(txStopCounter.Text);
            }
            else
            {
                txStopCounter.Text = "-1";
                iStopCounter = -1;
            }
            iStopCounterBackup = iStopCounter;
            txStopCounter.Enabled = false;

            //Step1: Obtains header file from the device and user to confirm

            bMessageModeSelected = (Byte)'H';
            Download_Header_Processor();

            DialogResult dr = MessageBox.Show("Check the header layout as shown, click 'ok' to proceed?", "", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.Cancel)
            {
                txtCyclePeriod.ReadOnly = false;
                return;
            }
            //Step2: Create new batch of filename with header file if filename and folder is validated. 
            if (Logger_OpenNewFile(false) == false)     // Modified 4/2/16
                return;
            //Step3: Start loop timer process
            bMessageModeSelected = (Byte)'D';
            Logger_Loop_Master();
            //The loop timer process now control the download operation//
        }
        #endregion

        #region //================================================================btnPause_Click
        private void btnPause_Click(object sender, EventArgs e)
        {
            bPauseButtonClicked = true;

            txtCyclePeriod.ReadOnly = false;
            btnFolder.Enabled = true;
            btnTestHeader.Enabled = true;
            btnDataFrame.Enabled = true;
            btnPause.Enabled = true;
            txStopCounter.Enabled = true;

            myGlobalBase.LoggerAsyncMode = false;
            myGlobalBase.LoggerAsyncSnadMode = false;
        }
        #endregion

        #region //================================================================btnNullTest_Click
        private void btnNullTest_Click(object sender, EventArgs e)
        {
            Download_Null_Processor();
        }
        #endregion

        #region //================================================================btnTestHeader_Click
        private void btnTestHeader_Click(object sender, EventArgs e)
        {
            bMessageModeSelected = (Byte)'H';
            Download_Header_Processor();
        }
        #endregion

        #region //================================================================btnDataFrame_Click
        private void btnDataFrame_Click(object sender, EventArgs e)
        {
            bDataFrameTestOnly = true;
            bMessageModeSelected = (Byte)'H';
            Download_Data_Processor();
        }
        #endregion

        #region //================================================================instructionToolStripMenuItem_Click
        private void instructionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Step: \n (a) Select Folder you wish to store, by default c:\\DataFrameLogger\n (b) Click Header/Data to test stream, make sure it is aligned\n (c) Choose period\n (d) Click Start to create filename and start process\n (e) Click Cease Loop to discontinue loop\n (f) Click Start to create new timestamped filename again.\n\n Append Note insert note manually any time, ie external meter readout, etc\n Suggest '$T1:145' for temp1, '$T2:156', $V1:145mV, $I1:2.1A, etc");
        }
        #endregion

        #region //================================================================BtnOpenFolder_Click
        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                bDataFrameTestOnly = false;
                string myPath = sFoldername;
                if (myPath == null)             // No filename path, quit. 
                    return;
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = myPath;
                prc.Start();
            }
            catch
            {
                rtbConsole.Text += "#ERR: Open Folder has no folder/path or attempt to use restricted domains of Drive C";
            }
        }
        #endregion

        #region //================================================================setWaitloopTo1000ToolStripMenuItem_Click
        private void setWaitloopTo1000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myGlobalBase.WaitCyclePeriodSetting = 100;
            setWaitloopTo1000ToolStripMenuItem.Checked = true;
            setWaitloopTo100ToolStripMenuItem.Checked = false;
        }
        #endregion

        #region //================================================================setWaitloopTo100ToolStripMenuItem_Click
        private void setWaitloopTo100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myGlobalBase.WaitCyclePeriodSetting = 200;
            setWaitloopTo1000ToolStripMenuItem.Checked = false;
            setWaitloopTo100ToolStripMenuItem.Checked = true;
        }
        #endregion

        #region //================================================================resetCountdownToolStripMenuItem_Click
        private void resetCountdownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BatchWaitPeriod = Convert.ToUInt32(txtCyclePeriod.Text, 10);
            txtCyclePeriod.Refresh();
            txtCountDown.Text = BatchWaitPeriod.ToString();
            txtCountDown.Refresh();
        }
        #endregion

        #region //================================================================resetCountdown5ToolStripMenuItem_Click
        private void resetCountdown5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BatchWaitPeriod = 5;
            txtCyclePeriod.Refresh();
            txtCountDown.Text = BatchWaitPeriod.ToString();
            txtCountDown.Refresh();
        }
        #endregion

        #region //================================================================cADTDataFrameLoggerToolStripMenuItem2_Click
        private void cADTDataFrameLoggerToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            bDataFrameTestOnly = false;
            sFoldername = "G:\\ADTDataFrameLogger";
            myGlobalBase.sSessionFoldername = "G:\\ADTSessionLog";
            dDataFrameLoggerCommonFolderNameUpdate();
        }
        private void cADTDataFrameLoggerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bDataFrameTestOnly = false;
            sFoldername = "F:\\ADTDataFrameLogger";
            myGlobalBase.sSessionFoldername = "F:\\ADTSessionLog";
            dDataFrameLoggerCommonFolderNameUpdate();
        }

        private void cADTDataFrameLoggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bDataFrameTestOnly = false;
            sFoldername = "E:\\ADTDataFrameLogger";
            myGlobalBase.sSessionFoldername = "E:\\ADTSessionLog";
            dDataFrameLoggerCommonFolderNameUpdate();
        }

        private void cADTDataFrameLoggerToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            bDataFrameTestOnly = false;
            sFoldername = "H:\\ADTDataFrameLogger";
            myGlobalBase.sSessionFoldername = "H:\\ADTSessionLog";
            dDataFrameLoggerCommonFolderNameUpdate();
        }

        private void cADTDataFrameLoggerToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            bDataFrameTestOnly = false;
            sFoldername = "I:\\ADTDataFrameLogger";
            myGlobalBase.sSessionFoldername = "I:\\ADTSessionLog";
            dDataFrameLoggerCommonFolderNameUpdate();
        }

        private void cADTDataFrameLoggerToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            bDataFrameTestOnly = false;
            sFoldername = "J:\\ADTDataFrameLogger";
            myGlobalBase.sSessionFoldername = "J:\\ADTSessionLog";
            dDataFrameLoggerCommonFolderNameUpdate();
        }

        private void dDataFrameLoggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bDataFrameTestOnly = false;
            sFoldername = "D:\\ADTDataFrameLogger";
            myGlobalBase.sSessionFoldername = "D:\\ADTSessionLog";
            dDataFrameLoggerCommonFolderNameUpdate();
        }
        private void dDataFrameLoggerCommonFolderNameUpdate()
        {
            try
            {
                txtFolderName.Text = sFoldername;
                if (!Directory.Exists(sFoldername))                                 // If folder does not exisit, create new one 
                {
                    DirectoryInfo di = Directory.CreateDirectory(sFoldername);  // Create folder if not exist. 
                }
                txtFolderName.Text = sFoldername;
                txtFilename.Text = "To be Generated";

                sAppendFileName = System.IO.Path.Combine(sFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_AppendNote.csv"));
                sErrorFileName = System.IO.Path.Combine(sFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_ExceptionReport.txt"));
            }
            catch
            {
                rtbConsoleAppend("#ERR: Folder/Drive Not Exist\n");
                btnSTCStart.Enabled = false;
                btnStart.Enabled = false;
                btnSTCStartSnad.Enabled = false;
            }
            btnSTCStart.Enabled = true;
            btnStart.Enabled = true;
            btnSTCStartSnad.Enabled = true;
        }
        #endregion

        #region //================================================================enableAppendOneFilenameToolStripMenuItem_Click
        private void enableAppendOneFilename_Click(object sender, EventArgs e)
        {
            if (enableAppendOneFilename.Checked == true)
            {
                enableAppendOneFilename.Checked = false;
                bAppendOneFileEnable = false;
            }
            else
            {
                enableAppendOneFilename.Checked = true;
                bAppendOneFileEnable = true;
            }
        }
        #endregion

        #region //================================================================enableAsyncWithGDERToolStripMenuItem_Click
        private void enableAsyncWithGDERToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (enableAsyncWithGDERToolStripMenuItem.Checked == false)      // For Async message starting with $D, $G, $R, $F with $E ending. 
            {
                if (Logger_OpenNewFile(true) == false)                      // Validate filename for CVS logging. 
                {
                    rtbConsoleAppend("!!! ASYNC MODE IS NOT ACTIVATED DUE TO INCORREDCT DRIVE/FOLDER !!!");
                    btnSTCStart.Enabled = false;
                    btnSTCStartSnad.Enabled = false;
                    return;
                }
                enableAsyncWithGDERToolStripMenuItem.Checked = true;
                myGlobalBase.LoggerAsyncMode = true;
                myGlobalBase.LoggerAsyncSnadMode = false;
                //----------------------------------------Enable Append into one filename. 
                //enableAppendOneFilename.Checked = true;
                //bAppendOneFileEnable = true;
                //-----------------------------------
                btnFolder.Enabled = false;
                btnTestHeader.Enabled = false;
                btnDataFrame.Enabled = false;
                bDataFrameTestOnly = false;
                Download_Data_Processor();
            }
            else
            {
                enableAsyncWithGDERToolStripMenuItem.Checked = false;
                myGlobalBase.LoggerAsyncMode = false;
                myGlobalBase.LoggerAsyncSnadMode = false;
                //-----------------------------------
                enableAppendOneFilename.Checked = false;
                bAppendOneFileEnable = false;
                //-----------------------------------
                bDataFrameTestOnly = false;
                btnFolder.Enabled = true;
                btnTestHeader.Enabled = true;
                btnDataFrame.Enabled = true;
            }
        }
        #endregion

        #region //================================================================tpLoggerCVS_SelectedIndexChanged
        private void tpLoggerCVS_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (isTurnOffSyncTab == true)
            {
                tpLoggerCVS.SelectedIndex = 1;
            }
            if (tpLoggerCVS.SelectedIndex == 0)     // Sync
            {
                tpLoggerCVS_Select_Sync();
            }
            if (tpLoggerCVS.SelectedIndex == 1)     // ASync
            {
                tpLoggerCVS_Select_Async();
            }
            if (tpLoggerCVS.SelectedIndex == 2)     // ASync Snad
            {
                tpLoggerCVS_Select_AsyncSnad();
            }
        }
        #endregion

        #region //================================================================tpLoggerCVS_Select_AsyncSnad
        private void tpLoggerCVS_Select_AsyncSnad()
        {
            if (myGlobalBase.LoggerAsyncSnadMode == true)                 // Stay on ASYNC SNAD.
            {
                tpLoggerCVS.SelectTab(2);
                return;
            }
            if (myGlobalBase.LoggerAsyncMode == true)                   // Stay on ASYNC .
            {
                tpLoggerCVS.SelectTab(1);
                return;
            }
            btnSTCStartSnad.Enabled = false;
            btnSTCStart.Enabled = false;
            enableAsyncWithGDERToolStripMenuItem.Checked = true;
            //myGlobalBase.LoggerAsyncMode = true;
            //myGlobalBase.LoggerAsyncSnadMode = false;
            //----------------------------------------Enable Append into one filename. 
            //enableAppendOneFilename.Checked = true;
            //bAppendOneFileEnable = true;
            //-----------------------------------
            btnFolder.Enabled = false;
            btnTestHeader.Enabled = false;
            btnDataFrame.Enabled = false;
            bDataFrameTestOnly = false;
            /*
            Download_Data_Processor();
            */
        }
        #endregion

        #region //================================================================tpLoggerCVS_Select_Async
        private void tpLoggerCVS_Select_Async()
        {
            if (myGlobalBase.LoggerAsyncSnadMode == true)                 // Stay on ASYNC SNAD.
            {
                tpLoggerCVS.SelectTab(2);
                return;
            }
            if (myGlobalBase.LoggerAsyncMode == true)                   // Stay on ASYNC .
            {
                tpLoggerCVS.SelectTab(1);
                return;
            }
            btnSTCStartSnad.Enabled = false;
            btnSTCStart.Enabled = true;
            enableAsyncWithGDERToolStripMenuItem.Checked = true;
            //myGlobalBase.LoggerAsyncMode = true;
            //myGlobalBase.LoggerAsyncSnadMode = false;
            //----------------------------------------Enable Append into one filename. 
            //enableAppendOneFilename.Checked = true;
            //bAppendOneFileEnable = true;
            //-----------------------------------
            btnFolder.Enabled = false;
            btnTestHeader.Enabled = false;
            btnDataFrame.Enabled = false;
            bDataFrameTestOnly = false;
            /*
            Download_Data_Processor();
            */
        }
        #endregion

        #region //================================================================tpLoggerCVS_Select_Sync
        private void tpLoggerCVS_Select_Sync()
        {
            if (myGlobalBase.LoggerAsyncSnadMode == true)                 // Stay on ASYNC SNAD.
            {
                tpLoggerCVS.SelectTab(2);
                return;
            }
            if (myGlobalBase.LoggerAsyncMode == true)                   // Stay on ASYNC .
            {
                tpLoggerCVS.SelectTab(1);
                return;
            }

            //             if (myGlobalBase.LoggerAsyncMode == true)
            //                 isAsynctoSync = true;                                   // block error.
            enableAsyncWithGDERToolStripMenuItem.Checked = false;
            //myGlobalBase.LoggerAsyncMode = false;
            //-----------------------------------
            enableAppendOneFilename.Checked = false;
            bAppendOneFileEnable = false;
            //-----------------------------------
            bDataFrameTestOnly = false;
            btnFolder.Enabled = true;
            btnTestHeader.Enabled = true;
            btnDataFrame.Enabled = true;
            sMessageData = "";
        }
        #endregion


        //#####################################################################################################
        //###################################################################################### ASYNC Control
        //#####################################################################################################

        #region //================================================================cbSTCSTARTMode_MouseClick
        private void cbSTCSTARTMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            myGlobalBase.LoggerCVS_STCSTARTMode = cbSTCSTARTMode.SelectedIndex;
        }
        #endregion

        #region //================================================================cbSTCSTARTMode_MouseClick
        private void cbCallBackMode_MouseClick(object sender, MouseEventArgs e)
        {
            myGlobalBase.LoggerCVS_CallBackMode = cbCallBackMode.Checked;
            //------------------------------------------------Important!!!!!
            if (cbCallBackMode.Checked == true)               // When callback is enable, the old style GET/SET time is unchecked as it not relevant.  
                CbApplySetGetTime.Checked = false;
        }
        #endregion

        #region //================================================================btnSTCStart_Click
        private void btnSTCStart_Click(object sender, EventArgs e)
        {
            myCalc_CRC8.CRC8CounterReset();
            rawDataignoreFormatFrameToolStripMenuItem.Enabled = false;
            //-----------------------------------------------------Added 78F.
            if (myDetoken != null)
                myDetoken.ClearAllVariable();
            if (myUDTmyFrameDataLog != null)
                myUDTmyFrameDataLog.ClearAllVariable();
            //---------------------------------------
            if (myGlobalBase.LoggerAsyncSnadMode == true)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("Async SNAD Mode is active, you need to stop first. ignore request", "Operation ERROR", 10);
                return;
            }
            myGlobalBase.LoggerAsyncMode = true;
            myGlobalBase.LoggerAsyncSnadMode = false;
            if (Logger_OpenNewFile(true) == false)                      // Validate filename for CVS logging. 
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("Async mode not activated due to Open Filename issue, check folder", "Operation ERROR", 10);
                btnSTCStart.Enabled = false;
                btnSTCStartSnad.Enabled = false;
                myGlobalBase.LoggerAsyncMode = false;
                myGlobalBase.LoggerAsyncSnadMode = false;
                return;
            }
            //--------------------------------- ASYNC timeout and counter. 
            if (Tools.IsString_Numberic_Int32(tbAlertTimer.Text) == false)
            {
                tbAlertTimer.Text = "#ERROR#";
                return;
            }
            iASyncAlertTimerSetting = Tools.ConversionStringtoInt32(tbAlertTimer.Text);
            if (iASyncAlertTimerSetting < -1)
            {
                iASyncAlertTimerSetting = -1;
                tbAlertTimer.Text = "-1";
            }
            if (iASyncAlertTimerSetting != -1)
            {
                iAsyncAlertTimerCount = 0;
                iASyncAlertTimerCurrent = iASyncAlertTimerSetting;
            }
            //==================================================================================
            //==================================================================================Survey: LogMem Window
            //==================================================================================
            if (cbSendToLogMem.Checked == true)
            {
                rtbConsoleAppend("Transfer to LogMem Survey Window Enabled (Advanced)");
                myEEDownloadSurvey.SurveyUpdateFolderName(sFoldername);     //myGlobalBase.sSessionFoldername
                myEEDownloadSurvey.EE_LogMemSurvey_Show();
                myEEDownloadSurvey.SurveyLoggerCVS_InitSetup();
                //-----------------------------------------------
                myEEDownloadSurvey.SurveyLoggerCVS_Start();
            }
            //==================================================================================
            //==================================================================================Survey: Client Code 
            //==================================================================================
            if (cbSendtoClientCode.Checked == true)
            {
                rtbConsoleAppend("Transfer to Client Survey Window Enabled (Basic)");
                myClientCode.myClientCode_PreStartSurveySetup();
            }
            //=================================================================================
            btnSTCStart.Enabled = true;
            btnSTCStartSnad.Enabled = true;
            enableAsyncWithGDERToolStripMenuItem.Checked = true;
            isASYNC_STCSTOP_occurred = false;
            //----------------------------------------Enable Append into one filename. 
            //enableAppendOneFilename.Checked = true;
            //bAppendOneFileEnable = true;
            //-----------------------------------
            btnFolder.Enabled = false;
            btnTestHeader.Enabled = false;
            btnDataFrame.Enabled = false;
            bDataFrameTestOnly = false;

            //==================================================================================
            cbSendtoClientCode.Enabled = false;
            cbSendToLogMem.Enabled = false;
            //-------------------------------------------------------Added 22/10/17 for MiniAD7794 Supports
            if (myGlobalBase.LoggerCVSMiniPortEnable == true)
            {
                myMiniSetup.LoggerCVS_MiniAD7794_ASYNC_Send_RTC_StartLog();
            }
            else
            {
                if (cbCallBackMode.Checked == false)        // Skipped this, not relevant to callback STC protocol. 
                {
                    //------------------------------------- Check for empty EEPROM, since it save there.
                    m_sLoggerEntryTxt = "EE_ISEMPTY()";
                    rtbConsoleAppend(m_sLoggerEntryTxt);
                    Logger_Command_ASCII_Process();
                    System.Threading.Thread.Sleep(200);     //200mSec pause
                    //---------------------------------
                }
                Download_Data_Processor();
                if (LCVStimer1 == null)
                {
                    LCVStimer1 = new Timer();
                    LCVStimer1.Elapsed += new System.Timers.ElapsedEventHandler(logggerCVS_Timer1_Sqeuencer_Event);
                }
                LCVStimer1.Interval = 1000;                 // 100mSec sequencer
                LCVStimer1.AutoReset = false;
                LCVStimer1.Start();
            }
            //---------------------------------
            if (iASyncAlertTimerSetting != -1)
            {
                bgwk2.RunWorkerAsync();
            }
        }
        #endregion

        #region //================================================================logggerCVS_Timer1_Sqeuencer_Event
        //---------------------------------------------------------------------------------------
        private void logggerCVS_Timer1_Sqeuencer_Event(object sender, System.Timers.ElapsedEventArgs e)
        {
            LCVStimer1.AutoReset = false;
            LCVStimer1.Elapsed -= logggerCVS_Timer1_Sqeuencer_Event;            // un-subscribe
            LCVStimer1 = null;
            //----------------------------------tbAsyncPeroid
            if (Tools.IsString_Numberic_UInt32(tbAsyncPeroid.Text) == false)
            {
                tbAsyncPeroid.Text = "ERROR";
                return;
            }
            Int32 AsyncPeroid = Tools.ConversionStringtoInt32(tbAsyncPeroid.Text);
            if (AsyncPeroid <= 0)
            {
                tbAsyncPeroid.Text = "10";
                AsyncPeroid = 10;
            }

            //----------------------------------Update Date/Time
            DateTime dt2 = DateTime.Now;
            DateTime dt = dt2.ToLocalTime();
            if ((CbApplySetGetTime.Checked == true) & (cbCallBackMode.Checked == false))                 // Old Style Commands which involve three command. ";
            {
                m_sLoggerEntryTxt = "SET_TIME(";
                m_sLoggerEntryTxt += String.Format("{0:HH;mm;ss}", dt);
                m_sLoggerEntryTxt += ")";
                rtbConsoleAppend(m_sLoggerEntryTxt);
                Logger_Command_ASCII_Process();
                System.Threading.Thread.Sleep(100);                  //ex 100 //100mSec pause
                m_sLoggerEntryTxt = "SET_DATE(";
                m_sLoggerEntryTxt += String.Format("{0:yyyy;MM;dd}", dt);
                m_sLoggerEntryTxt += ")";
                rtbConsoleAppend(m_sLoggerEntryTxt);
                Logger_Command_ASCII_Process();
                System.Threading.Thread.Sleep(100);                   //ex 100 //100mSec pause
                //----------------------------------
                myUSB_Message_Manager.LoggerMessageRX = "";
                //-----------------------------------------------------STC Start Generator (early version)
                //                 if (myGlobalBase.LoggerCVS_CallBackMode == true)
                //                     logCVS_STCSTART_Callback(AsyncPeroid,false);     // callback approach for old style command is not supported.
                //                 else
                logCVS_STCSTART_Standard(AsyncPeroid, false);
            }
            else  // New Style Command "STC_START(2;UDT;AsyncPeroid)";      // One command replace the old style method. 
            {
                //-----------------------------------------------------STC Start Generator (new version)
                if (myGlobalBase.LoggerCVS_CallBackMode == true)
                    logCVS_STCSTART_Callback(AsyncPeroid, true);
                else
                    logCVS_STCSTART_Standard(AsyncPeroid, true);
            }
        }
        #endregion

        #region //=================================================================logCVS_STCSTART_Standard
        private void logCVS_STCSTART_Standard(Int32 AsyncPeroid, bool isNewStyle)
        {
            if (isNewStyle == true)
            {
                DateTime dt2 = DateTime.Now;
                DateTime dt = dt2.ToLocalTime();
                string UDT = Tools.sDateTimeToUnixTimestamp(dt);
                UDT = UDT.Substring(0, UDT.IndexOf("."));
                m_sLoggerEntryTxt = "STC_START(";
                m_sLoggerEntryTxt += myGlobalBase.LoggerCVS_STCSTARTMode.ToString();
                m_sLoggerEntryTxt += ";" + UDT;
                m_sLoggerEntryTxt += ";" + AsyncPeroid.ToString() + ")";         // Enable Export CVS (as EEPROM store), Added AsyncPeroid.
            }
            else
            {
                m_sLoggerEntryTxt = "STC_START(";
                m_sLoggerEntryTxt += myGlobalBase.LoggerCVS_STCSTARTMode.ToString();
                m_sLoggerEntryTxt += ";" + AsyncPeroid.ToString() + ")";         // Enable Export CVS (as EEPROM store), Added AsyncPeroid.
            }                                                                            //-----------------------------------------------------
            rtbConsoleAppend(m_sLoggerEntryTxt);
            myGlobalBase.LoggerOpertaionMode = true;
            myUSB_Message_Manager.endoflinedetected = false;
            Logger_Command_ASCII_Process();
        }
        #endregion

        #region //=================================================================logCVS_STCSTART_Callback, callback
        private void logCVS_STCSTART_Callback(Int32 AsyncPeroid, bool isNewStyle)
        {
            string sMessage = "";
            //-----------------------------------Place Message to rtbTerm Window.
            if (isNewStyle == true)
            {
                DateTime dt2 = DateTime.Now;
                DateTime dt = dt2.ToLocalTime();
                string UDT = Tools.sDateTimeToUnixTimestampHex(dt);
                //UDT = UDT.Substring(0, UDT.IndexOf("."));
                sMessage = "+STCSTART(";
                sMessage += myGlobalBase.LoggerCVS_STCSTARTMode.ToString();
                sMessage += ";" + UDT;
                sMessage += ";" + AsyncPeroid.ToString() + ")";         // Enable Export CVS (as EEPROM store), Added AsyncPeroid.
            }
            else
            {
                sMessage = "+STCSTART(";
                sMessage += myGlobalBase.LoggerCVS_STCSTARTMode.ToString();
                sMessage += ";" + AsyncPeroid.ToString() + ")";                             // Enable Export CVS (as EEPROM store), Added AsyncPeroid.
            }
            //-----------------------------------------------------------------------------Modernized version of callback.
            DMFP_Delegate fpSTCStart_CALLBACK = new DMFP_Delegate(DMFP_STCSTART_CallBack);
            if (myDMFProtocol.DMFP_Process_Command(fpSTCStart_CALLBACK, sMessage, false, -1) == false)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("ERROR within logCVS_STCSTART_Callback(): This callback does not support Array VCOM ", "ERROR in VCOM Protocol", 30, 12F);
            }
            else
            {
                myUSB_Message_Manager.endoflinedetected = false;
                rtbConsoleAppend(sMessage);
            }
        }
        #endregion

        #region //===============================================================DMFP_STCSTART_CallBack  
        public void DMFP_STCSTART_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFF)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("ERROR within DMFP_STCSTART_CallBack(): +STCSTART() return error code: 0x" + hPara[0].ToString("X"), "ERROR in +STCTART() Protocol", 30, 12F);
                return;
            }
            mbOperationBusy = null;
            mbOperationBusy = new DialogSupport();
            mbOperationBusy.PopUpMessageBox("Processing Logging ", "LoggerCVS", 1, 12F);
            myGlobalBase.LoggerOpertaionMode = true;
        }
        #endregion

        #region //================================================================btnSTCStop_Click
        private void btnSTCStop_Click(object sender, EventArgs e)
        {
            rawDataignoreFormatFrameToolStripMenuItem.Enabled = true;
            if ((cbSendToLogMem.Checked == true) & (myGlobalBase.LoggerAsyncMode == true))
            {
                rtbConsoleAppend("-I: Transfer to LogMem Window has stopped");
                myEEDownloadSurvey.SurveyLoggerCVS_Stop();
            }
            if (cbSendToLogMem.Checked == true)
            {
                rtbConsoleAppend("-I: Transfer to LogMem Table (ClientCode) has stopped");
                myClientCode.myClientCodeStop();
            }
            //-----------------------------------------------------
            cbSendToLogMem.Enabled = true;
            cbSendtoClientCode.Enabled = true;
            //----------------------------------------------------
            tbAlertTimer.Text = iASyncAlertTimerSetting.ToString();
            iAsyncAlertTimerCount = 0;
            iASyncAlertTimerCurrent = iASyncAlertTimerSetting;
            //-----------------------------------------------------
            isASYNC_STCSTOP_occurred = true;
            myUSB_Message_Manager.LoggerMessageRX = "";
            m_sLoggerEntryTxt = " ";
            myUSB_Message_Manager.endoflinedetected = false;
            Logger_Command_ASCII_Process();
            Thread.Sleep(100);                      // Wait 100mSec, then
            myUSB_Message_Manager.LoggerMessageRX = "";
            //----------------------------------------------------
            if (myGlobalBase.LoggerCVS_CallBackMode == true)
            {
                //-----------------------------------------------------------------------------Modernized version of callback.
                DMFP_Delegate fpSTCStop_CALLBACK = new DMFP_Delegate(DMFP_STCSTOP_CallBack);
                string sMessage = "+STCSTOP()";
                rtbConsoleAppend(sMessage);
                if (myDMFProtocol.DMFP_Process_Command(fpSTCStop_CALLBACK, sMessage, false, -1) == false)
                {
                    mbOperationError.PopUpMessageBox("ERROR within btnSTCStop_Click(): This callback does not support Array VCOM ", "ERROR in VCOM Protocol", 30, 12F);
                }
                else
                {
                    myGlobalBase.LoggerOpertaionMode = true;
                    myUSB_Message_Manager.endoflinedetected = false;
                }
            }
            else
            {
                m_sLoggerEntryTxt = "STC_STOP()";
                myGlobalBase.LoggerOpertaionMode = true;
                myUSB_Message_Manager.endoflinedetected = false;
                Logger_Command_ASCII_Process();
            }

        }
        #endregion

        #region //===============================================================DMFP_STCSTOP_CallBack  
        public void DMFP_STCSTOP_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFF)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("ERROR within DMFP_STCSTOP_CallBack(): +STCSTOP() return error code: " + hPara[0].ToString("X"), "ERROR in +STCSTOP() Protocol", 30, 12F);
            }
            else
            {
                mbOperationBusy = null;
                mbOperationBusy = new DialogSupport();
                mbOperationBusy.PopUpMessageBox("Finish Logging", "LoggerCVS", 1, 12F);
            }
            btnSTCStart.Enabled = true;
            btnSTCStartSnad.Enabled = true;
            myGlobalBase.LoggerAsyncMode = false;
            myGlobalBase.LoggerAsyncSnadMode = false;
        }
        #endregion

        #region //================================================================btnAsyncSuspendUpdate_Click
        private void btnAsyncSuspendUpdate_Click(object sender, EventArgs e)
        {
            if (btnAsyncSuspendUpdate.Text == "Hold List")
            {
                btnAsyncSuspendUpdate.Text = "Cont. List";
                bHoldConsoleScreen = true;
            }
            else
            {
                btnAsyncSuspendUpdate.Text = "Hold List";
                bHoldConsoleScreen = false;
            }
        }
        #endregion

        #region //================================================================btnAsyncHelp_Click
        private void btnAsyncHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Async Frame Example <#D;324;ENABLED;0xFF34;-45678;-90;0x12;$E\n> $H = Header (Column-1), $D = Data (Column-2), $G = Data/Des (Column-3). $E\\n = End Frame. Separator with ';'. Each frame is appended to timestamp filename. NB: Must select folder first.", "Async Help",
             MessageBoxButtons.OK,
             MessageBoxIcon.Information);
        }
        #endregion

        #region //================================================================btnLogCVSInstruction_Click
        private void btnLogCVSInstruction_Click(object sender, EventArgs e)
        {
            string filename = Application.StartupPath;
            filename = Path.GetFullPath(Path.Combine(filename, ".\\PDFBin\\700138_BGDebug_QuickUserGuideII.pdf"));
            if (myPDFViewerBGDebug == null)
            {
                myPDFViewerBGDebug = new PDFViewer();
                myPDFViewerBGDebug.filename = filename;
            }
            myPDFViewerBGDebug.Show();

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### ASYNC SNAD Control
        //#####################################################################################################

        #region //================================================================btnSTCStartSnad_Click
        private void btnSTCStartSnad_Click(object sender, EventArgs e)
        {
            //-----------------------------------------------------Added 78F.
            if (myDetoken != null)
                myDetoken.ClearAllVariable();
            if (myUDTmyFrameDataLog != null)
                myUDTmyFrameDataLog.ClearAllVariable();
            //-----------------------------------------------------
            if (myGlobalBase.LoggerAsyncMode == true)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("Async Mode is active, you need to stop first. ignore request", "Operation ERROR", 10);
                return;
            }
            myGlobalBase.LoggerAsyncMode = false;
            myGlobalBase.LoggerAsyncSnadMode = true;
            if (Logger_SNAD_OpenNewFile() == false)                      // Validate filename for CVS logging. 
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("Async mode not activated due to Open Filename issue, check folder", "Operation ERROR", 10);
                btnSTCStart.Enabled = false;
                btnSTCStartSnad.Enabled = false;
                myGlobalBase.LoggerAsyncMode = false;
                myGlobalBase.LoggerAsyncSnadMode = false;
                return;
            }
            btnSTCStart.Enabled = true;
            btnSTCStartSnad.Enabled = true;
            //--------------------------------- ASYNC timeout and counter. 
            iASyncAlertTimerSetting = -1;                               // Feature disabled.
            iASyncAlertTimerCurrent = 0;
            //---------------------------------------------------Client Code, reset DGV and variables before starting. 
            if (cbSendtoClientCodeSnad.Checked == true)
            {
                myClientCode.myClientCode_PreStartSurveySetup();
            }
            enableAsyncWithGDERToolStripMenuItem.Checked = true;

            isASYNC_STCSTOP_occurred = false;
            //----------------------------------------Enable Append into one filename. 
            //enableAppendOneFilename.Checked = true;
            //bAppendOneFileEnable = true;
            //-----------------------------------
            btnFolder.Enabled = false;
            btnTestHeader.Enabled = false;
            btnDataFrame.Enabled = false;
            bDataFrameTestOnly = false;
            cbSendToLogMem.Enabled = false;
            //---------------------------------         // Activate Serial Comms.
            Download_Data_Processor();
            //---------------------------------         // Timer.
            //             if (LCVStimer1 == null)
            //             {
            //                 LCVStimer1 = new Timer();
            //                 LCVStimer1.Elapsed += new System.Timers.ElapsedEventHandler(logggerCVS_Timer1_Sqeuencer_Event);
            //             }
            //             LCVStimer1.Interval = 1000;                 // 100mSec sequencer
            //             LCVStimer1.AutoReset = false;
            //             LCVStimer1.Start();
            //---------------------------------
            //             if (iASyncAlertTimerSetting != -1)
            //             {
            //                 bgwk2.RunWorkerAsync();
            //             }
        }
        #endregion

        #region //================================================================btnSTCStopSnad_Click
        private void btnSTCStopSnad_Click(object sender, EventArgs e)
        {
            btnSTCStop.PerformClick();
        }
        #endregion

        #region //================================================================btnAsyncSuspendUpdateSnad_Click
        private void btnAsyncSuspendUpdateSnad_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region //================================================================btnAsyncHelpSnad_Click
        private void btnAsyncHelpSnad_Click(object sender, EventArgs e)
        {

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### App Terminal (rtbConsole)
        //#####################################################################################################

        #region //================================================================rtbTermfRichTextBox_Ref
        //==========================================================
        // Purpose  : Append message in terminal window,. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        public System.Windows.Forms.RichTextBox rtbConsoleRichTextBox_Ref()
        {
            return (this.rtbConsole);
        }
        #endregion

        #region //================================================================rtbConsoleAppend
        private string messagebuffer;
        public delegate void rtbConsoleDelegate(string message);
        //Solution: http://www.dreamincode.net/forums/topic/188209-cross-thread-calls-made-easy/ 
        public void rtbConsoleAppend(string message)
        {
            
            // Check if we need to call BeginInvoke.
            if (rtbConsole.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false.
                rtbConsole.BeginInvoke(new rtbConsoleDelegate(rtbConsoleAppend),
                                             new object[] { message });
                return;
            }
            Tools.rtb_StopRepaint(rtbConsole, rtbOEMConsole);
            if (rtbConsole.TextLength >= 300000)        //300000
            {
                Logger_Console_OpenNewFile();           // Save console text into file before clean up
                if (bAppendOneFileEnable == true)
                {
                    Logger_Console_AppendOneFilename();
                }
                rtbConsole.Text = String.Empty;
                rtbConsole.AppendText("+INFO: Saved console text into the timestamp file (.txt) with the folder and cleared text\n");
            }
            if (bHoldConsoleScreen == true)
            {
                messagebuffer += message + Environment.NewLine;
            }
            else
            {
                messagebuffer += message;
                rtbConsole.SelectionStart = rtbConsole.TextLength;
                rtbConsole.ScrollToCaret();
                rtbConsole.Select();
                if (messagebuffer=="")
                {
                    rtbConsole.AppendText(message + Environment.NewLine);
                }
                else
                {
                    messagebuffer += Environment.NewLine;
                    rtbConsole.AppendText(messagebuffer);
                    messagebuffer = "";
                }
                //rtbConsole.Refresh();
                Tools.rtb_StartRepaint(rtbConsole, rtbOEMConsole);
            }
        }
        #endregion

        #region //================================================================rtbConsole_TextChanged
        private void rtbConsole_TextChanged(object sender, EventArgs e)
        {
            if (rtbConsole.Lines.Length >= 1000)          // trim off the excessive length to avoid memory issue. 
            {
                BtnSaveConsole_Click(this, new EventArgs());
            }
            rtbConsole.ScrollToCaret();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### LogMem Erase
        //#####################################################################################################

        #region //================================================================btnSTCERASE_Deep_Click
        private void btnSTCERASE_Deep_Click(object sender, EventArgs e)
        {
            //-----------------------------------------------------------------------------Modernized version of callback.
            DMFP_Delegate fpSTCEraseBlock_CALLBACK = new DMFP_Delegate(DMFP_STCERASEBlock_CallBack);
            string sMessage = "+STCERASE(1)";
            rtbConsoleAppend(sMessage);
            if (myDMFProtocol.DMFP_Process_Command(fpSTCEraseBlock_CALLBACK, sMessage, false, -1) == false)
            {
                mbOperationError.PopUpMessageBox("ERROR within btnSTCERASE_Deep_Click(): This callback does not support Array VCOM ", "ERROR in VCOM Protocol", 30, 12F);
            }
        }
        #endregion

        #region //===============================================================DMFP_STCERASEBlock_CallBack  
        public void DMFP_STCERASEBlock_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFF)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("ERROR within DMFP_STCERASEBlock_CallBack(): +STCERASE(1) return error code: " + hPara[0].ToString("X"), "ERROR in +STCSTOP() Protocol", 30, 12F);
            }
            else
            {
                mbOperationBusy = null;
                mbOperationBusy = new DialogSupport();
                mbOperationBusy.PopUpMessageBox("LogMem Erase Done", "LoggerCVS", 1, 12F);
            }
        }
        #endregion

        #region //================================================================btnSTCERASE_Fast_Click
        private void btnSTCERASE_Fast_Click(object sender, EventArgs e)
        {
            //-----------------------------------------------------------------------------Modernized version of callback.
            DMFP_Delegate fpSTCErase_Bulk_CALLBACK = new DMFP_Delegate(DMFP_STCERASEBulk_CallBack);
            string sMessage = "+STCERASE(2)";
            rtbConsoleAppend(sMessage);
            if (myDMFProtocol.DMFP_Process_Command(fpSTCErase_Bulk_CALLBACK, sMessage, false, -1) == false)
            {
                mbOperationError.PopUpMessageBox("ERROR within btnSTCStop_Click(): This callback does not support Array VCOM ", "ERROR in VCOM Protocol", 30, 12F);
            }
        }
        #endregion

        #region //===============================================================DMFP_STCERASEBulk_CallBack  
        public void DMFP_STCERASEBulk_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFF)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("ERROR within DMFP_STCERASEBulk_CallBack(): +STCERASE(1) return error code: " + hPara[0].ToString("X"), "ERROR in +STCSTOP() Protocol", 30, 12F);
            }
            else
            {
                mbOperationBusy = null;
                mbOperationBusy = new DialogSupport();
                mbOperationBusy.PopUpMessageBox("LogMem Erase Busy, wait until busy inductor has expired", "LoggerCVS", 5, 12F);
            }
        }
        #endregion

        #region //================================================================btnSTCERASE_Verify_Click
        private void btnSTCERASE_Verify_Click(object sender, EventArgs e)
        {
            //-----------------------------------------------------------------------------Modernized version of callback.
            DMFP_Delegate fpSTCErase_verify_CALLBACK = new DMFP_Delegate(DMFP_STCERASEVerify_CallBack);
            string sMessage = "+STCERASE(3)";
            rtbConsoleAppend(sMessage);
            if (myDMFProtocol.DMFP_Process_Command(fpSTCErase_verify_CALLBACK, sMessage, false, -1) == false)
            {
                mbOperationError.PopUpMessageBox("ERROR within btnSTCERASE_Verify_Click(): This callback does not support Array VCOM ", "ERROR in VCOM Protocol", 30, 12F);
                return;
            }
            mbOperationBusy = null;
            mbOperationBusy = new DialogSupport();
            mbOperationBusy.PopUpMessageBox("LogMem Verify is busy, please wait", "LoggerCVS", 2, 12F);
        }
        #endregion

        #region //===============================================================DMFP_STCERASEVerify_CallBack  
        public void DMFP_STCERASEVerify_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFF)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("ERROR within DMFP_STCERASEBulk_CallBack(): +STCERASE(1) return error code: " + hPara[0].ToString("X"), "ERROR in +STCSTOP() Protocol", 30, 12F);
            }
            else
            {
                mbOperationBusy = null;
                mbOperationBusy = new DialogSupport();
                mbOperationBusy.PopUpMessageBox("LogMem Verify Done", "LoggerCVS", 1, 12F);
            }
        }
        #endregion

        #region//=============================================================rawDataignoreFormatFrameToolStripMenuItem_Click
        private void rawDataignoreFormatFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rawDataignoreFormatFrameToolStripMenuItem.Checked == false)
                rawDataignoreFormatFrameToolStripMenuItem.Checked = true;
            else
                rawDataignoreFormatFrameToolStripMenuItem.Checked = false;
        }
        #endregion

        #region//=============================================================tFormatFrameHelpToolStripMenuItem_Click
        private void tFormatFrameHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            frm.Height = 600;
            frm.Width = 650;
            TextBox tbx = new TextBox();
            frm.Controls.Add(tbx);
            tbx.Multiline = true;
            tbx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            tbx.Font = new Font("Consolas", 12.0f);
            tbx.Height = frm.Height;
            tbx.Width = frm.Width;
            tbx.AppendText("Help Section for LoggerCVS Data transfer\r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("78F: This LoggerCVS support the following frame type:   \r\n");
            tbx.AppendText("$D/$G/$H/$T: This is general use. $T is optional        \r\n");
            tbx.AppendText("$E/$S/$R, etc are not accommodated to this window but   \r\n");
            tbx.AppendText("passed to other window (especially LogMem Window).      \r\n");
            tbx.AppendText("$H header frame must be sent first followed by optional \r\n");
            tbx.AppendText("$T format frame (which define data format type) and     \r\n");
            tbx.AppendText("$D Data frame (primary) which goes to 2nd column.       \r\n");
            tbx.AppendText("$G Data2 frame (secondary), optional goes to 3rd column.\r\n");
            tbx.AppendText("$D and $G can be left raw string data or converted via  \r\n");
            tbx.AppendText("format frame (as detail below).                         \r\n");
            tbx.AppendText("Top Menu=>Optional_Async/Raw Data:                      \r\n");
            tbx.AppendText("         checked= ignore $T if provided.                \r\n");
            tbx.AppendText("       unchecked= ignore $T if provided.                \r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("Format Frame: $T.\r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("When $T (Format Frame) is emitted from LoggerCVS test   \r\n");
            tbx.AppendText("code after the $H header frame. It decode and format    \r\n");
            tbx.AppendText("the raw data into representative number for each column \r\n");
            tbx.AppendText("Below is the format type data in UDTTerm Protocol       \r\n");
            tbx.AppendText("  0y      Date            Obsolete (BG), use UDT1970    \r\n");
            tbx.AppendText("  0z      Time            Obsolete (BG), use UDT1970    \r\n");
            tbx.AppendText("  0q      uINT16 Hex      iPara     \r\n");
            tbx.AppendText("  0w      INT16 Hex       iPara     \r\n");
            tbx.AppendText("  0i      INT32           iPara     <<<Alway use this!!!\r\n");
            tbx.AppendText("  0u      uINT32          hPara     \r\n");
            tbx.AppendText("  0d      double          dPara     \r\n");
            tbx.AppendText("  0x      uINT32          hPara     \r\n");
            tbx.AppendText("  0s      string          sPara     \r\n");
            tbx.AppendText("  undef   string          sPara     Undefined type     \r\n");
            tbx.AppendText("  Approved separator: ';' and ',' and ':'.             \r\n");
            tbx.AppendText("  (Alway use default ';' for all project)              \r\n");
            tbx.AppendText(" Example $T;0i;0i;0s;0i;0i;0i;0i;0x;0x;$E\\n           \r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("Header Frame. $H\r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText(" Description of column aligned with $T, $D and $G frame \r\n");
            tbx.AppendText(" Example $H;string;string;string;string; etc#E\\n       \r\n");
            tbx.AppendText(" NB: All project must have $H;SNAD;SYSTICK to start with\r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("Data Frame. $D \r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText(" This frame contains data set based on Format Frame     \r\n");
            tbx.AppendText(" It may contains 0xFF34 hex data or 0sHello string, etc \r\n");
            tbx.AppendText(" Example $D;0;1529345612;0xFF32;0x32;0sHello;0i3421;$E\\n\r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("2nd Data Frame. $G \r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText(" This is 2nd Data Frame to handle two survey format.    \r\n");
            tbx.AppendText(" It assumed aligned with $H and $T for $D Frame         \r\n");
            tbx.AppendText(" for example $D for vibration and $G for Gyro data set  \r\n");
            tbx.AppendText(" This was used in BG project but already obsolete use.  \r\n");
            frm.ShowDialog();
        }
        #endregion

        #region//=============================================================tFormatFrameHelpToolStripMenuItem_Click
        private void rFDPPDHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            frm.Height = 600;
            frm.Width = 650;
            TextBox tbx = new TextBox();
            frm.Controls.Add(tbx);
            tbx.Multiline = true;
            tbx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            tbx.Font = new Font("Consolas", 12.0f);
            tbx.Height = frm.Height;
            tbx.Width = frm.Width;
            tbx.AppendText("Help Section for RFDPPD MKII for ADC24-CH3 Transfer     \r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("Revision 78JD onward support this feature in ASYNC Tab  \r\n");
            tbx.AppendText("(a) Formatted Data to CVS file: Raw or Formatted type   \r\n");
            tbx.AppendText("(b) RFDDAQ-CH3, Split to two Column: Custom CVS file    \r\n");
            tbx.AppendText("(c) Bug fixes in detoken code (use 'D' within toString) \r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("RFDPPD has following details                            \r\n");
            tbx.AppendText("Sample Rate is 50Hz: $DData transfer every 1024mSec/3 = \r\n");
            tbx.AppendText("341mSec (can run 100Hz sample rate).                    \r\n");
            tbx.AppendText("$H;SNAD;UDT1970;1mSecTick;CH3A;CH3B;CH3A;CH3B...$E      \r\n");
            tbx.AppendText("$T;0u;0l;0l;0q;0q;0q;0q.....$E                          \r\n");
            tbx.AppendText("$D;0;UDT1970;1mSec;CH3A;CH3B;CH3A;CH3B;CH3A;CH3B...$E   \r\n");
            tbx.AppendText("FW: Rev 3K included in this feature.Rely on SIR flags   \r\n");
            tbx.AppendText("every 10mSec, when set it run $D frame to UDTTERM       \r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("Make Sure STCSTART(1;UDT1970;0) with option 1 (not 2)   \r\n");
            tbx.AppendText("Make Sure to check two box (a) and (b) in ASYNC tab     \r\n");
            tbx.AppendText("Do not use callback feature.                            \r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("### HIGHLY RECOMMENDED TO RUN ON SOLID STATE DRIVE! ### \r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            frm.ShowDialog();
        }
        #endregion
    }
    #region//=============================================================DoubleBuffered property via reflection for datagridview, improve flickerless image.
    public static class ExtensionMethodsLoggerMessage1
    {
        public static void DoubleBufferedLoggerMessage1(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    #endregion
}