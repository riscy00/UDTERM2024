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
using UDT_Term;
//using Microsoft.Expression.Utility.Extensions.Enumerable;

// Power Tool: insert QuickLaunch with "@tasks CollapseRegions" to close all region only. 

namespace UDT_Term_FFT
{
    public partial class EE_LogMemSurvey : Form
    {
        ITools Tools = new Tools();
        USB_FTDI_Comm myUSBComm;                    // FTDI device manager section (scan, open, close, read, write).
        USB_VCOM_Comm myUSBVCOMComm;

        DMFP_BulkFrame cDMFP_BulkFrame;
        DMFProtocol myDMFProtocol;
        GlobalBase myGlobalBase;
        //System.Windows.Forms.RichTextBox myRtbTerm;
        RiscyScope myRiscyScope;
       // BG_ToolSetup myBGToolSetup;
        //BG_ReportViewer myBGDrillerViewer;
        BGMasterSetup myBGMasterSetup;
        EEPROMExportImport myEEPROMExpImport;
        DialogSupport mbOperationBusy;
        MainProg myMainProg;
        BindingList<BGMasterJobSurvey> blBGMasterJobSurvey = null;                 // Binding List for basic setup.
        BindingList<BGMasterDriller> blBGMasterDrillerFile = null;
        #region 
        UDTmyFrameDataLog myUDTmyFrameDataLog;
        #endregion
        //--------------------------------------Filename
        //private string sFoldername;
        //private string sFilename;
        private string RecievedData;       
        private string m_sEntryTxt;
        public bool isMinimise { get; set; }
        public uint EEPROMTotalCapacity;
        List<string> sFrame;
        int ColumnNumber = 0;
        //private string RecievedDataForFiling;
        //---------------------------------------RiscyScope, buffer data for real time update and scrolling feature. New approach for charting. 
        int ChartSelectedColumn;
        int ChartSelectedStartRow;
        int ChartSelectedEndRow;
        int[] ChartSelectedCh = new int[4];     // ChartSelectedCh[0] = -1   ==> Not Selected for CH0. // ChartSelectedCh[0] = e.ColumnIndex  ==> Selected. 
        List<cPoint> ScopeCH0;
        List<cPoint> ScopeCH1;
        List<cPoint> ScopeCH2;
        List<cPoint> ScopeCH3;
        //-----------------------------------------
        CRC8CheckSum myCalc_LoggerCRC8;
        //-----------------------------------------
        ESM_STC_LogMem myESMSTCLogMem;

        #region //============================================================Reference Object
       
        public void MyDMFProtocol(DMFProtocol myDMFProtocolRef)
        {
            myDMFProtocol = myDMFProtocolRef;
        }
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        //public void MyRtbTerm(System.Windows.Forms.RichTextBox myrtbTermref)
        //{
        //    myRtbTerm = myrtbTermref;
        //}
        public void MyEEPROMExpImport(EEPROMExportImport myEEPROMExpImportRef)
        {
            myEEPROMExpImport = myEEPROMExpImportRef;
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
        public void MyBGMasterSetup(BGMasterSetup myBGMasterSetupRef)
        {
            myBGMasterSetup = myBGMasterSetupRef;
        }

        public void MyUSBComm(USB_FTDI_Comm myUSBCommRef)
        {
            myUSBComm = myUSBCommRef;
        }

        public void MyUSBVCOMComm(USB_VCOM_Comm myUSBVCOMCommRef)
        {
            myUSBVCOMComm = myUSBVCOMCommRef;
        }

        public void MyRiscyScope(RiscyScope myRiscyScopeRef)
        {
            myRiscyScope = myRiscyScopeRef;
        }
        public void MyBGMasterJobSurvey(BGMasterJobSurvey myMasterJobSurveyRef)
        {
            if (blBGMasterJobSurvey == null)
            {
                BindingList<BGMasterJobSurvey> bindingList = new BindingList<BGMasterJobSurvey>();
                blBGMasterJobSurvey = bindingList;
                blBGMasterJobSurvey.Add(new BGMasterJobSurvey());     // Default Setting if filename not exist.
                blBGMasterJobSurvey[0] = myMasterJobSurveyRef;
            }
            else
            {
                blBGMasterJobSurvey[0] = myMasterJobSurveyRef;
            }
        }
        #endregion

        public EE_LogMemSurvey()
        {
            InitializeComponent();
            dgvSurveyViewer.DoubleBufferedSurveyCVS(true);
            isMinimise = false;
            ColumnNumber = 0;
            RiscyScopeChart_Init();         // Init Chart Setup. 
            //BG_ProjectDriver 
            //BG_ProjectLocalFilename 
            myCalc_LoggerCRC8 = new CRC8CheckSum();
            myESMSTCLogMem = new ESM_STC_LogMem();
        }

        //#####################################################################################################
        //###################################################################################### Form Manager
        //##################################################################################################### 

        #region //================================================================EE_LogMemSurvey_FormClosing
        private void EE_LogMemSurvey_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (myGlobalBase.EE_isSurveyCVSRawLogDataActive == false & myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate == false)       // Active process, cannot close. 
            {
                myGlobalBase.LogMemWindowVisable = false;                 // Cease Survey CVS Terminal mode.
                this.Visible = false;
                this.Hide();
            }
        }
        #endregion
       
        #region //================================================================EE_LogMemSurvey_Show
        public void EE_LogMemSurvey_Show()
        { 
            this.Visible = true;
            this.Show();
            myGlobalBase.LogMemWindowVisable = true;
            //------------------------------  
            this.BringToFront();
            this.TopMost = false;
            this.WindowState = FormWindowState.Normal;
            //--------------------------------------------------Project Specific.
            EE_LogMemSurvey_UpdateTheme();
        }
        #endregion
        
        #region //================================================================EE_LogMemSurvey_UpdateTheme
        private void EE_LogMemSurvey_UpdateTheme()
        {
            switch (myGlobalBase.SelectTools)
            {
                case ((int)GlobalBase.eSelectTools.GENERIC):       //LoggerCVS, all tools, all debugs with LOG_DOWNLOAD() to be changed to +STCUPLOAD later. 
                    {
                        tstbSelectTool.Text = "Mode: Generic/LoggerCVS";
                        gbLogMemTransferSpec.Visible = false;
                        //---------------------------------
                        tbMetaData4.Visible = true;
                        tbMetaData5.Visible = true;
                        tbMetaData6.Visible = true;
                        tbMetaData7.Visible = true;
                        btnViewDrillerFile.Visible = false;
                        label5.Visible = true;
                        label6.Visible = true;
                        label7.Visible = true;
                        label8.Visible = true;
                        label1.Text = "$D Survey";
                        //---------------------------------
                        btnDownhole.Visible = true;
                        btnContinue.Visible = false;
                        BtnPause.Visible = false;
                        groupBox1.Visible = true;
                        //btnLoadSurveyFile.Visible = false;
                        btnViewDrillerFile.Visible = true;
                        cbHideD.Visible = false;
                        cbHideFR.Visible = false;
                        cbHideG.Visible = false;
                        break;
                    }
                case ((int)GlobalBase.eSelectTools.ADTESM):         // ESM with +STCUPLOAD(....)
                    {
                        tstbSelectTool.Text = "Mode: ADT ESM MK2";
                        gbLogMemTransferSpec.Visible = true;
                        //---------------------------------Hybrid Box.
                        tbMetaData5.Visible = true;
                        label5.Visible = true;
                        label5.Text = "$B Hybrid";
                        //---------------------------------No Of Byte
                        tbMetaData4.Visible = true;
                        label7.Visible = true;
                        //---------------------------------
                        tbMetaData6.Visible = false;
                        label6.Visible = false;
                        //---------------------------------
                        tbMetaData7.Visible = false;
                        label8.Visible = false;
                        //---------------------------------
                        btnViewDrillerFile.Visible = false;
                        label1.Text = "$D Data";
                        //---------------------------------
                        btnDownhole.Visible = true;
                        btnContinue.Visible = false;
                        BtnPause.Visible = true;
                        groupBox1.Visible = true;
                        //btnLoadSurveyFile.Visible = false;
                        btnViewDrillerFile.Visible = true;
                        cbHideD.Visible = true;
                        cbHideFR.Visible = true;
                        cbHideG.Visible = true;
                        break;
                    }
                case ((int)GlobalBase.eSelectTools.ADTRFFDAQ):       // RFDDAQ / DataLog
                    {
                        tstbSelectTool.Text = "Mode: RFDDAQ/DataLog";
                        //---------------------------------
                        DataLog_DownloadSurvey_Show();
                        break;
                    }
                case ((int)GlobalBase.eSelectTools.BGTOCO):       //BG TOCO
                    {
                        tstbSelectTool.Text = "Mode: TOCO/TVB";
                        gbLogMemTransferSpec.Visible = false;
                        //---------------------------------
                        tbMetaData4.Visible = false;
                        tbMetaData5.Visible = false;
                        tbMetaData6.Visible = false;
                        tbMetaData7.Visible = false;
                        label5.Visible = false;
                        label6.Visible = false;
                        label7.Visible = false;
                        label8.Visible = false;
                        btnViewDrillerFile.Visible = true;
                        label1.Text = "$D Survey";
                        //---------------------------------
                        btnDownhole.Visible = true;
                        btnContinue.Visible = false;
                        BtnPause.Visible = false;
                        groupBox1.Visible = true;
                        //btnLoadSurveyFile.Visible = false;
                        btnViewDrillerFile.Visible = true;
                        cbHideD.Visible = true;
                        cbHideFR.Visible = true;
                        cbHideG.Visible = true;
                        break;
                    }
                case ((int)GlobalBase.eSelectTools.BGTSTOCO):       //BG TS
                    {
                        tstbSelectTool.Text = "Mode: TS/TVB";
                        gbLogMemTransferSpec.Visible = false;
                        //---------------------------------
                        tbMetaData4.Visible = false;
                        tbMetaData5.Visible = false;
                        tbMetaData6.Visible = false;
                        tbMetaData7.Visible = false;
                        label5.Visible = false;
                        label6.Visible = false;
                        label7.Visible = false;
                        label8.Visible = false;
                        btnViewDrillerFile.Visible = true;
                        label1.Text = "$D Survey";
                        //---------------------------------
                        btnDownhole.Visible = true;
                        btnContinue.Visible = false;
                        BtnPause.Visible = false;
                        groupBox1.Visible = true;
                        //btnLoadSurveyFile.Visible = false;
                        btnViewDrillerFile.Visible = true;
                        cbHideD.Visible = true;
                        cbHideFR.Visible = true;
                        cbHideG.Visible = true;
                        break;
                    }
                case ((int)GlobalBase.eSelectTools.TVBADIS16460):       // Old Directional.
                    {
                        tstbSelectTool.Text = "Mode: ADIS16460/TVB";
                        gbLogMemTransferSpec.Visible = false;
                        //---------------------------------
                        tbMetaData4.Visible = true;
                        tbMetaData5.Visible = true;
                        tbMetaData6.Visible = true;
                        tbMetaData7.Visible = true;
                        btnViewDrillerFile.Visible = true;
                        label5.Visible = true;
                        label6.Visible = true;
                        label7.Visible = true;
                        label8.Visible = true;
                        label1.Text = "$D Survey";
                        //---------------------------------
                        btnDownhole.Visible = true;
                        btnContinue.Visible = false;
                        BtnPause.Visible = false;
                        groupBox1.Visible = true;
                        //btnLoadSurveyFile.Visible = false;
                        btnViewDrillerFile.Visible = true;
                        cbHideD.Visible = true;
                        cbHideFR.Visible = true;
                        cbHideG.Visible = true;
                        break;
                    }
                case ((int)GlobalBase.eSelectTools.OLDDIRECT):       // Old Directional.
                    {
                        tstbSelectTool.Text = "Mode: Old Dir/TVB";
                        gbLogMemTransferSpec.Visible = false;
                        //---------------------------------
                        tbMetaData4.Visible = true;
                        tbMetaData5.Visible = true;
                        tbMetaData6.Visible = true;
                        tbMetaData7.Visible = true;
                        btnViewDrillerFile.Visible = true;
                        label5.Visible = true;
                        label6.Visible = true;
                        label7.Visible = true;
                        label8.Visible = true;
                        label1.Text = "$D Survey";
                        //---------------------------------
                        btnDownhole.Visible = true;
                        btnContinue.Visible = false;
                        BtnPause.Visible = false;
                        groupBox1.Visible = true;
                        //btnLoadSurveyFile.Visible = false;
                        btnViewDrillerFile.Visible = true;
                        cbHideD.Visible = true;
                        cbHideFR.Visible = true;
                        cbHideG.Visible = true;
                        break;
                    }
                default:
                    break;
            }
        }
#endregion

        #region //================================================================DataLog_DownloadSurvey_Show
        public void DataLog_DownloadSurvey_Show()
        {
            myGlobalBase.DataLog_isSurveyCVSOpen = true;
            this.Visible = true;
            this.Show();
            myGlobalBase.LogMemWindowVisable = true;
            //------------------------------------------
            gbLogMemTransferSpec.Visible = false;
            //------------------------------------------
            tbMetaData4.Visible = false;
            tbMetaData5.Visible = false;
            tbMetaData6.Visible = false;
            tbMetaData7.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            btnViewDrillerFile.Visible = false;
            label1.Text = "$D Survey";
            //------------------------------------------
            btnDownhole.Visible = false;
            btnContinue.Visible = false;
            BtnPause.Visible = false;
            groupBox1.Visible = false;
            //btnLoadSurveyFile.Visible = false;
            cbHideD.Visible = false;
            cbHideFR.Visible = false;
            cbHideG.Visible = false;
            //------------------------------------------
            this.WindowState = FormWindowState.Normal;
            this.TopMost = false;
            this.BringToFront();
        }
        #endregion

        #region //================================================================EE_UpdateFolderName (Jun18)
        public void EE_UpdateFolderName(string foldername)
        {
            if (myBGMasterSetup == null)
                return;
            //--------------------------------------------------------------
            btnReset.Visible = false;
            
            if (Directory.Exists(foldername))
            {
                btnDownhole.Enabled = true;
                myBGMasterSetup.sFile_JobProjectAll = foldername;
                myBGMasterSetup.sFile_TopFolderProjectAll = foldername;
                txtFolderName.Text = myBGMasterSetup.sFile_JobProjectAll;
                myBGMasterSetup.sFile_DriveAll = "";
                string drivename = foldername.Substring(0, 2);               // J://SurveyProject//JobName
                myBGMasterSetup.sFile_DriveAll = (string)drivename;           // J:
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: FolderName Not Valid or Drive Do not exist, update this now!");
                txtFolderName.Text = "";
                btnDownhole.Enabled = false;
                myBGMasterSetup.sFile_JobProjectAll = "";
                myBGMasterSetup.sFile_TopFolderProjectAll = "";
                txtFolderName.Text = "" ;
                myBGMasterSetup.sFile_DriveAll = "";
            }

            //-------------------------------------------------------------
            cbChartXNone.Checked = true;
            cbChartXRIT.Checked = false;
            cbSuspendScroll.Checked = false;

            EE_LogMemSurvey_UpdateTheme();
        }
        #endregion

        #region //================================================================EE_LogMemSurvey_Load
        private void EE_LogMemSurvey_Load(object sender, EventArgs e)
        {
            //myBGDrillerViewer = new BG_ReportViewer();
            //myBGDrillerViewer.MyBGMasterSetup(myBGMasterSetup);
            //myBGDrillerViewer.MyGlobalBase(myGlobalBase);
            //myBGDrillerViewer.MyRtbTerm(myRtbTerm);

            btnDownhole.Enabled = false;

            this.dgvSurveyViewer.RowsDefaultCellStyle.BackColor = Color.Bisque;
            this.dgvSurveyViewer.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;           //Color.Bisque, Color.Beige

            btnReset.Visible = false;

            if (Directory.Exists(myBGMasterSetup.sFile_JobProjectAll))
            {
                btnDownhole.Enabled = true;
            }
            txtFolderName.Text = myBGMasterSetup.sFile_JobProjectAll;

            cbChartXNone.Checked = true;
            cbChartXRIT.Checked = false;
            cbSuspendScroll.Checked = false;

            tscSelectMethod.Items.AddRange(myESMSTCLogMem.sSelectMethod);
            tscSelectMethod.SelectedIndex = 2;

            EE_LogMemSurvey_UpdateTheme();
        }
        #endregion

        #region //================================================================EE_LogMemSurvey_Resize
        private void EE_LogMemSurvey_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                isMinimise = true;
            else
                isMinimise = false;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Received Data
        //#####################################################################################################

        #region //================================================================btnDownhole_Click
        private void btnDownhole_Click(object sender, EventArgs e)
        {
            mbOperationBusy = null;
            mbOperationBusy = new DialogSupport();
            if (!((myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == true) | (myGlobalBase.is_Serial_Server_Connected == true)))
            {
                mbOperationBusy.PopUpMessageBox("Error: Tool is not connected, Plug cable and connect!", "Serial Comm Error", (2));     // Close automatically after 5min
                return;
            }
            mbOperationBusy.isTopMostState = false;
            mbOperationBusy.PopUpMessageBox("Downloading Logger Data, Please Wait!", " Logger Memory Download", (60*5));     // Close automatically after 5min
            mbOperationBusy.isTopMostState = true;
            //--------------------------------------Clear DGV
            dgvSurveyViewer.DataSource = null;
            this.Refresh();
            myMainProg.myRtbTermMessageLF("-I: Please wait while being downloaded, click <Stop> to request cancellation");
            //--------------------------------------Reset Variables
            InitForSurveyCVS_RecievedData();
            //--------------------------------------
            switch (myGlobalBase.SelectTools)
            {
                case ((UInt32) GlobalBase.eSelectTools.ADTESM):     // Selected Tool for specific command
                    {
                        tsmbActual.Text = "Actual:+STCUPLOAD(...)";
                        ESMTools_STCUPLOAD();
                        break;
                    }
                case ((UInt32)GlobalBase.eSelectTools.BGTOCO):
                case ((UInt32)GlobalBase.eSelectTools.BGTSTOCO):    // Selected TSTOCO Rev 5X but not for TOCO Ref 4X for specific command
                    {
                        if (tscSelectMethod.SelectedIndex!=12)  // Rev 5X use newer command +STCUPLOAD(). 
                        {
                            tsmbActual.Text = "Actual:+STCUPLOAD(...)";
                            TSTOCOTools_STCUPLOAD();
                        }
                        else                                    // To Support Rev 4X FW.
                        {
                            tsmbActual.Text = "Actual:LOG_DOWNLOAD()";
                            if (myGlobalBase.is_Serial_Server_Connected == true)
                            {
                                m_sEntryTxt = "LOG_DOWNLOAD()";
                                Survey_Command_ASCII_Process();
                                Console.Beep(1000, 20);
                            }
                            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == true)
                            {
                                m_sEntryTxt = "LOG_DOWNLOAD()";
                                SurveyArray_Command_ASCII_Process((int)eUSBDeviceType.BGDRILLING);
                                Console.Beep(1000, 20);
                            }
                        }
                        break;
                    }
                default:
                    {
                        if (tscSelectMethod.SelectedIndex != 12)        // Use new command +STCUPLOAD()
                        {
                            tsmbActual.Text = "Actual:+STCUPLOAD(...)";
                            ESMTools_STCUPLOAD();                       // This is complete upload package based on ESM Project. 
                        }
                        else
                        { 
                            if (myGlobalBase.is_Serial_Server_Connected == true)
                            {
                                tsmbActual.Text = "Actual:LOG_DOWNLOAD()";
                                m_sEntryTxt = "LOG_DOWNLOAD()";
                                Survey_Command_ASCII_Process();
                                Console.Beep(1000, 20);
                            }
                        }
                        break;
                    }
            }
            //--------------------------------------
        }
        #endregion

        #region//================================================================ESMTools_CloseTransfer, run this when #@#E is detected from #@#P. 
        public void ESMTools_CloseTransfer()
        {
            this.SuspendLayout();
            myGlobalBase.EE_isSurveyCVSRawLogDataActive = false;
            myGlobalBase.EE_isLogMemFrameTransferMode = false;
            myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate = false;
            if (mbOperationBusy == null)
                mbOperationBusy = new DialogSupport();      // Required in case of download survey file.
            else
                mbOperationBusy.Close();
            btnDownhole.Enabled = true;
            this.ResumeLayout();
        }

        #endregion

        #region//================================================================ESMTools_STCUPLOAD
        ESM_LogMem_Report_Statistic myStatistic;
        public bool ESMTools_STCUPLOAD()            // 10/8/19 76P, updated coding style, using switch statement. 
        {
            //-----------------------------------Setup call-back delegate for +STCUPLOAD(Method;Para1;Para2)
            myESMSTCLogMem.Parameter1 = 0;
            myESMSTCLogMem.Parameter2 = 0;
            if (Tools.IsString_Numberic_UInt32(tstbPara1.Text))
            {
                myESMSTCLogMem.Parameter1 = Tools.ConversionStringtoUInt32(tstbPara1.Text);
            }
            if (Tools.IsString_Numberic_UInt32(tstbPara2.Text))
            {
                myESMSTCLogMem.Parameter2 = Tools.ConversionStringtoUInt32(tstbPara2.Text);
            }
            //-----------------------------------------------------------------------------------------------------
            switch (tscSelectMethod.SelectedIndex)      
            {
                case (0):
                case (1):       // Not Allowed.
                    {
                        FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        DialogResult response = FlexiMessageBox.Show("Method 0 and 1 is reserved for Pause/Continue scope. Cancelled Operation",
                        "STCUPLOAD Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                        return false;
                    }
                case (3):       // Selected Transfer via UDT Start and Stop. 
                    {
                        if (myESMSTCLogMem.Parameter1 >= myESMSTCLogMem.Parameter2)                      // STC_START must be less than STC_STOP
                        {
                            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            DialogResult response = FlexiMessageBox.Show("Parameter Error: STC_START must be less than STC_STOP. Cancelled Operation",
                            "STCUPLOAD Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                            return false;
                        }
                        if ((myESMSTCLogMem.Parameter1 < 1546300800) | (myESMSTCLogMem.Parameter2 < 1546300800))
                        {
                            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            DialogResult response = FlexiMessageBox.Show("Parameter Error: Cannot be less than 1546300800 (1st Jan 2019). Cancelled Operation",
                            "STCUPLOAD Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                            return false;
                        }
                        break;
                    }
                case (4):       // Selected Transfer via Start Address and number of frames/pages 
                    {
                        if (myESMSTCLogMem.Parameter1 <= 0x00000100)                                        // Page 0 is not allowed.
                        {
                            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            DialogResult response = FlexiMessageBox.Show("Start from Page 1 should be 0x00000100 or Parameter Error. Cancelled Operation",
                            "STCUPLOAD Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                            return false;
                        }
                        if (myESMSTCLogMem.Parameter1 >= 0x08000000 - 0x100)                                        // Page 0 is not allowed.
                        {
                            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            DialogResult response = FlexiMessageBox.Show("Outside LogMem Memory Range <0x0800000. Cancelled Operation",
                            "STCUPLOAD Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                            return false;
                        }
                        if (myESMSTCLogMem.Parameter2 == 0)                                                         // Page 0 is not allowed.
                        {
                            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            DialogResult response = FlexiMessageBox.Show("2nd Parameter should be more than 1 frame. Cancelled Operation",
                            "STCUPLOAD Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                            return false;
                        }
                        break;
                    }
                case (5):       // Selected Transfer via Start Address and number of frames/pages 
                    {
                        //                 if (tstbPara1.Text == "PARA1 N/A")                    // Select Bank1 as starting point 
                        //                 {
                        //                     tstbPara1.Text = "1";
                        //                     myESMSTCLogMem.Parameter1 = 1;
                        //                 }
                        break;
                    }
                case (6):       // Selected Transfer via Start UDT1970 and number of frames/pages 
                    {
                        if (myGlobalBase.TSTOCOParm1 < 1546300800)
                        {
                            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            DialogResult response = FlexiMessageBox.Show("Parameter Error: Cannot be less than 1546300800 (1st Jan 2019). Cancelled Operation",
                            "STCUPLOAD Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                            return false;
                        }
                        if (myGlobalBase.TSTOCOParm2 == 0)                                                         // Not allowed.
                        {
                            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            DialogResult response = FlexiMessageBox.Show("2nd Parameter should be more than 1 frame. Cancelled Operation",
                            "STCUPLOAD Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                            return false;
                        }
                        break;
                    }
                case (11):
                    {
                        myStatistic = null;     // dispose old object.
                        myStatistic = new ESM_LogMem_Report_Statistic();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            string command = myESMSTCLogMem.GenerateCommand_STCUPLOAD(tscSelectMethod.SelectedIndex);
            DMFP_Delegate fpCallBack_STCUPLOAD = new DMFP_Delegate(ESMTools_STCUPLOAD_Callback);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_STCUPLOAD, command, false, -1);
            return true;
        }
        #endregion

        #region//================================================================ESMTools_STCUPLOAD_Callback (ESM)
        public void ESMTools_STCUPLOAD_Callback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFF)
            {
                FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                DialogResult response = FlexiMessageBox.Show("ErrorCode 0x:" + hPara[0].ToString("X") + " | Check Parameter/Method. Operation Cancelled",
                "STCUPLOAD Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region//================================================================TSTOCOTools_STCUPLOAD
        public bool TSTOCOTools_STCUPLOAD()            // 12/04/20 78JD, updated coding style, using switch statement. 
        {
            //-----------------------------------Setup call-back delegate for +STCUPLOAD(Method;Para1;Para2)
            myGlobalBase.TSTOCOParm1 = 0;
            myGlobalBase.TSTOCOParm2 = 0;
            if (Tools.IsString_Numberic_UInt32(tstbPara1.Text))
            {
                myGlobalBase.TSTOCOParm1 = Tools.ConversionStringtoUInt32(tstbPara1.Text);
            }
            if (Tools.IsString_Numberic_UInt32(tstbPara2.Text))
            {
                myGlobalBase.TSTOCOParm2 = Tools.ConversionStringtoUInt32(tstbPara2.Text);
            }
            //-----------------------------------------------------------------------------------------------------
            switch (tscSelectMethod.SelectedIndex)
            {
                case (0):
                case (1):       // Not Allowed.
                    {
                        FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        DialogResult response = FlexiMessageBox.Show("Method 0 and 1 is reserved for Pause/Continue scope. Cancelled Operation",
                        "STCUPLOAD Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                        return false;
                    }
                case (2):       // All Data transfer.
                    {

                        break;
                    }
                case (3):       // Selected Transfer via UDT Start and Stop. 
                    {
                        if (myGlobalBase.TSTOCOParm1 >= myGlobalBase.TSTOCOParm2)                      // STC_START must be less than STC_STOP
                        {
                            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            DialogResult response = FlexiMessageBox.Show("Parameter Error: STC_START must be less than STC_STOP. Cancelled Operation",
                            "STCUPLOAD Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                            return false;
                        }
                        if ((myGlobalBase.TSTOCOParm1 < 1546300800) | (myGlobalBase.TSTOCOParm2 < 1546300800))
                        {
                            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            DialogResult response = FlexiMessageBox.Show("Parameter Error: Cannot be less than 1546300800 (1st Jan 2019). Cancelled Operation",
                            "STCUPLOAD Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                            return false;
                        }
                        break;
                    }
                case (6):       // Selected Transfer via Start Address and number of frames/pages 
                    {
                        if (myGlobalBase.TSTOCOParm1 < 1546300800)
                        {
                            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            DialogResult response = FlexiMessageBox.Show("Parameter Error: Cannot be less than 1546300800 (1st Jan 2019). Cancelled Operation",
                            "STCUPLOAD Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                            return false;
                        }
                        if (myGlobalBase.TSTOCOParm2 == 0)                                                         // Not allowed.
                        {
                            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            DialogResult response = FlexiMessageBox.Show("2nd Parameter should be more than 1 frame. Cancelled Operation",
                            "STCUPLOAD Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                            return false;
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            string command = TSTOCOGenerateCommand_STCUPLOAD(tscSelectMethod.SelectedIndex);
            DMFP_Delegate fpCallBack_TSTOCOSTCUPLOAD = new DMFP_Delegate(TSTOCO_STCUPLOAD_Callback);
            if (myGlobalBase.is_Serial_Server_Connected == true)
            {
                myDMFProtocol.DMFP_Process_Command(fpCallBack_TSTOCOSTCUPLOAD, command, false, -1);
                Console.Beep(1000, 20);
            }
            else
            {
                myDMFProtocol.DMFP_Process_Command(fpCallBack_TSTOCOSTCUPLOAD, command, false, (int)eUSBDeviceType.BGDRILLING);
                Console.Beep(1000, 20);
            }
            
            return true;
        }
        #endregion

        #region//================================================================TSTOCOGenerateCommand_STCUPLOAD
        public string TSTOCOGenerateCommand_STCUPLOAD(int MethodIndex)
        {
            string sParameter1 = "0";
            string sParameter2 = "0";
            string command = "";
            switch (MethodIndex)
            {
                case (2):
                    {
                        command = "+STCUPLOAD(0x2)";
                        break;
                    }
                case (3):
                    {
                        sParameter1 = myGlobalBase.TSTOCOParm1.ToString("X");
                        sParameter2 = myGlobalBase.TSTOCOParm2.ToString("X");
                        command = "+STCUPLOAD(0x" + MethodIndex.ToString("X") + ";0x" + sParameter1 + ";0x" + sParameter2 + ")";
                        break;
                    }
                case (4):
                    {
                        sParameter1 = myGlobalBase.TSTOCOParm1.ToString("X");
                        sParameter2 = myGlobalBase.TSTOCOParm2.ToString("X");
                        command = "+STCUPLOAD(0x" + MethodIndex.ToString("X") + ";0x" + sParameter1 + ";0x" + sParameter2 + ")";
                        break;
                    }
                case (5):
                    {
                        sParameter1 = myGlobalBase.TSTOCOParm1.ToString("X");
                        command = "+STCUPLOAD(0x5;0x" + sParameter1 + ")";
                        break;
                    }
                case (6):
                    {
                        sParameter1 = myGlobalBase.TSTOCOParm1.ToString("X");
                        sParameter2 = myGlobalBase.TSTOCOParm2.ToString("X");
                        command = "+STCUPLOAD(0x" + MethodIndex.ToString("X") + ";0x" + sParameter1 + ";0x" + sParameter2 + ")";
                        break;
                    }
                default:
                    break;
            }
            return command;
        }
        #endregion

        #region//================================================================TSTOCO_STCUPLOAD_Callback (TSTOCO)
        public void TSTOCO_STCUPLOAD_Callback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFF)
            {
                FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                DialogResult response = FlexiMessageBox.Show("ErrorCode 0x:" + hPara[0].ToString("X") + " | Check Parameter/Method. Operation Cancelled",
                "STCUPLOAD Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            }
        }
        #endregion


        #region//================================================================InitForSurveyCVS_RecievedData (ESM)
        private void InitForSurveyCVS_RecievedData()
        {
            btnDownhole.Enabled = false;
            myGlobalBase.IsImportRawDataEnable = false;     // Make sure the ImportCVS is disabled. 
            myGlobalBase.IsImportRawDataActivate = false;
            //-------------------------------------- Activate protocol within USB_FTDI_Comm for threaded reception of RX Data. 
            myGlobalBase.EE_isSurveyCVSRawLogDataActive = true;
            myGlobalBase.EE_isLogMemFrameTransferMode = false;
            myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate = false;
            //---------------------------------------Reset Counter
            RecievedData = "";
            tbMetaData5.Text = "";
            tbMetaData6.Text = "";
            tbMetaData5.Text = "";
            if (myUDTmyFrameDataLog == null)
                myUDTmyFrameDataLog = new UDTmyFrameDataLog();
            else
                myUDTmyFrameDataLog.ClearAllVariable();
            tbMetaData1.Text   = myUDTmyFrameDataLog.iCounterD.ToString();
            tbMetaData2.Text   = myUDTmyFrameDataLog.iCounterG.ToString();
            if (tscSelectMethod.SelectedIndex == 9)
            {
                tbMetaData4.Text   = myUDTmyFrameDataLog.iCounterS.ToString();
                tbMetaData5.Text   = myUDTmyFrameDataLog.iCounterP.ToString();
            }
            else
            {
                tbMetaData4.Text = myUDTmyFrameDataLog.CountByte.ToString();
                tbMetaData5.Text = myUDTmyFrameDataLog.iCounterB.ToString();
            }
            tbError.Text = myUDTmyFrameDataLog.iCounterE.ToString();
            tbReport.Text = myUDTmyFrameDataLog.iCounterD.ToString();
            //---------------------------------------------
            if (sFrame == null)                            // if Frame is null then init. 
                sFrame = new List<string>();
            else
                sFrame.Clear();
        }
        #endregion

        #region//================================================================LogMemRecievedPageFrameData (ESM)
        // This is part of #@#P and #@#E page transfer. 
        public delegate void ESM_SurveyCVS_RecievedDataDelegate(List<string> ListDataFrame);
        //Solution: http://www.dreamincode.net/forums/topic/188209-cross-thread-calls-made-easy/ 
        public void LogMemRecievedPageFrameData(List<string> ListDataFrame)
        {
            // Check if we need to call BeginInvoke.
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ESM_SurveyCVS_RecievedDataDelegate(LogMemRecievedPageFrameData), new object[] { ListDataFrame });
                return;
            }
            if (sFrame == null)
                sFrame = new List<string>();
            else
                sFrame.Clear();
            this.SuspendLayout();
            sFrame = ListDataFrame;                                 // Passing sframe reference to page list from ESM_LogMem_PageMode_Transfer class. 
            myGlobalBase.EE_isSurveyCVSRawLogDataActive = false;
            myGlobalBase.EE_isLogMemFrameTransferMode = false;
            myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate = false;
            RecievedData = "";
            //------------------------------------------------Generate message to save as raw data.
            for (int i = 0; i < sFrame.Count; i++)
            {
                RecievedData += sFrame[i] + '\n';
            }
            bool status = SurveyCVS_OpenSaveDataCloseFile(myBGMasterSetup.sFilenameDefaultDownloadedRaw);
            tbMetaData4.Text = "";
            tbMetaData1.Text = "";
            tbMetaData2.Text = "";
            tbMetaData4.Text = sFrame.Count.ToString();
            tbError.Text = "";
            tbReport.Text = "";
            myMainProg.myRtbTermMessageLF("-I: End of Survey Data Transfer");
            EndOfBulkFrame(false);                                  // Treat as Raw data so get converted. 
            this.ResumeLayout();
        }
        #endregion

        #region//================================================================LogMemRecievedDiscreteFrameData (ESM)
        public void LogMemRecievedDiscreteFrameData(UDTmyFrameDataLog myUDTmyFrameDataLog, List<string> sFrameIn)
        {
            if (sFrame == null)
                sFrame = new List<string>();
            else
                sFrame.Clear();
            sFrame = sFrameIn;
            this.SuspendLayout();
            myGlobalBase.EE_isSurveyCVSRawLogDataActive = false;
            myGlobalBase.EE_isLogMemFrameTransferMode = false;
            myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate = false;
            RecievedData = "";
            //------------------------------------------------Generate message to save as raw data.
            for (int i=0; i<sFrame.Count; i++)
            {
                RecievedData += sFrame[i]+'\n';
            }

            bool status = SurveyCVS_OpenSaveDataCloseFile(myBGMasterSetup.sFilenameDefaultDownloadedRaw);       // RecievedData as data to be saved into filename.
            tbMetaData4.Text =  myUDTmyFrameDataLog.CountByte.ToString();
            tbMetaData5.Text = myUDTmyFrameDataLog.iCounterB.ToString();
            tbMetaData1.Text =  myUDTmyFrameDataLog.iCounterD.ToString();
            tbMetaData2.Text =  myUDTmyFrameDataLog.iCounterG.ToString();
            tbError.Text     =  myUDTmyFrameDataLog.iCounterF.ToString();
            tbReport.Text    =  myUDTmyFrameDataLog.iCounterR.ToString();
            label7.Text      =  "Loaded Byte";
            myMainProg.myRtbTermMessageLF("-I: End of Survey Data Transfer");
            EndOfBulkFrame(false);
            if ((myGlobalBase.SelectTools == (int)GlobalBase.eSelectTools.BGTOCO) || (myGlobalBase.SelectTools == (int)GlobalBase.eSelectTools.OLDDIRECT))        // TOCO/OLD DIR 
            {
                //--------------------------------------------------Import CVS Data from LCP1549 Memory.
                myMainProg.myRtbTermMessageLF("-INFO: Now Importing calibration file (CVS)");
                string sFilename = System.IO.Path.Combine(myBGMasterSetup.sFile_JobProjectAll, "ImportedCalibrationFile" + DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + ".csv");
                myMainProg.myRtbTermMessageLF("-INFO: Saving Downloaded Filename: " + sFilename);
                myEEPROMExpImport.EEPROM_ImportDataNow(true, sFilename, 0);
            }
            this.ResumeLayout();
        }
        #endregion

        #region//================================================================EndOfBulkFrame
        // isLoggerCVS = true if sourced from LoggerCVS, other set to false if sourced from EEPROM.
        // isConverted = true if source from loaded file, other set to false if from EEPROM Raw Data download.
        List<string> partmi;
        List<string> partbi;
        private bool EndOfBulkFrame(bool isConverted)       // false = data not converted, true = data was converted. 
        {
            CRC8CheckSum myCalc_CRC8 = new CRC8CheckSum();
            Frame_Detokeniser myDetoken = new Frame_Detokeniser();
            bool isCRCActive = false;
            string DataFrameNoCRC = "";
            string DataFrame;
            

            if (mbOperationBusy == null)
                mbOperationBusy = new DialogSupport();      // Required in case of download survey file.
            else
                mbOperationBusy.Close();
            //---------------------------------------------
            //mbOperationBusy.PopUpMessageBox("Processing Logger Memory, Please Wait!", " Logger Memory Processing", 5);     // Close automatically after 1min
            string ss = "";
            btnDownhole.Enabled = true;
            char[] delimiterChars = { ',', ';' };
            //------------------------------------------------------ Update display

            if (myUDTmyFrameDataLog == null)
            {
                myUDTmyFrameDataLog = new UDTmyFrameDataLog();
            }
            myUDTmyFrameDataLog.ClearAllVariable();
            tbMetaData4.Text = myUDTmyFrameDataLog.CountByte.ToString();
            tbMetaData1.Text = myUDTmyFrameDataLog.iCounterD.ToString();
            tbMetaData2.Text = myUDTmyFrameDataLog.iCounterG.ToString();
            tbError.Text = myUDTmyFrameDataLog.iCounterF.ToString();
            tbReport.Text = myUDTmyFrameDataLog.iCounterR.ToString();
            //------------------------
            tbNoOfFrame.Text = "";
            tbLastAddress.Text = "";
            tbUDTStop.Text = "";
            tbUDTStart.Text = "";
            //==================================================================================Hybrid Frame, V,G,S only
            ESM_LogMem_Hyrbid_Frame myFrameData;
            string datatimefile = DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss");
            string sFilenameGyro = System.IO.Path.Combine(myBGMasterSetup.sFile_JobProjectAll, myBGMasterSetup.sFilenameDefaultDownloadedConverted + "_Gyro" + datatimefile + ".csv");
            SurveyCVS_HybridFrameNewFile(sFilenameGyro);
            string sFilenameVibration = System.IO.Path.Combine(myBGMasterSetup.sFile_JobProjectAll, myBGMasterSetup.sFilenameDefaultDownloadedConverted + "_Vibration" + datatimefile + ".csv");
            SurveyCVS_HybridFrameNewFile(sFilenameVibration);
            string sFilenameShock = System.IO.Path.Combine(myBGMasterSetup.sFile_JobProjectAll, myBGMasterSetup.sFilenameDefaultDownloadedConverted + "_Shock" + datatimefile + ".csv");
            SurveyCVS_HybridFrameNewFile(sFilenameShock);
            
            string filename = "";
            for (int i = 0; i < sFrame.Count; i++)
            {
                if (sFrame[i].Contains("$B"))
                {
                    myFrameData = null;
                    myFrameData = new ESM_LogMem_Hyrbid_Frame();
                    int DataType = myFrameData.HybridFrame_ProcessString(sFrame[i]);
                    RecievedData = myFrameData.HybridFrame_ConvertedDate(DataType) + Environment.NewLine;
                    //--------------------------When done, save into separate filename as formatted. 
                    switch (DataType)
                    {
                        case (0x1FF):   //Vibration
                            {
                                bool status = SurveyCVS_HybridFrameSaveFile(sFilenameVibration, RecievedData);
                                break;
                            }
                        case (0x2FF):   //Shock
                            {
                                bool status = SurveyCVS_HybridFrameSaveFile(sFilenameShock, RecievedData);
                                break;
                            }
                        case (0x3FF):   //Gyro
                            {
                                bool status = SurveyCVS_HybridFrameSaveFile(sFilenameGyro, RecievedData);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    myUDTmyFrameDataLog.UDTDataLogProcessFrame(sFrame[i]);
                    sFrame[i] = "";                                                     //Remove hybrid frame, no longer needed for rest of the code. 
                    RecievedData = "";
                    tbMetaData5.Text = (myUDTmyFrameDataLog.iCounterB).ToString();        //Update $B counter. 
                }
            }
            
            //============================================================================================================
            //-----------------------------------------------------Remove Nulls Elements
            for (int i = 0; i < sFrame.Count; i++)
            {
                sFrame[i] = sFrame[i].Replace("\0", string.Empty);
                sFrame[i] = sFrame[i].Replace("\n", string.Empty);
                sFrame[i] = sFrame[i].Replace("\r", string.Empty);
            }
            myUDTmyFrameDataLog.lsFrameIn = sFrame;
            //----------------------------------------------------------Added 76L
            if (partbi != null)
            {
                partbi = null;
                partbi = new List<string>();
            }
            if (partmi != null)
            {
                partmi = null;
                partmi = new List<string>();
            }
            //----------------------------------------------------------
            foreach (string Frame in sFrame)
            {
                if ((isConverted == false))
                {
                    tbMetaData4.Text = myUDTmyFrameDataLog.CountByte.ToString();

                    #region//-------$MD decoder to EEPROM informatics
                    if (Frame.Contains("$MI"))
                    {
                        string sss = Frame.Replace(" ", ""); // remove white space. 
                        partmi = sss.Split(delimiterChars).ToList();
                        // Do nothing here.
                    }
                    if (Frame.Contains("$MD"))
                    {
                        try
                        {
                            //printf("#MI; EEPROM INFO; DL_STARTADD; DL_ENDADD; MaxAddress; NoOfPages; PageSize; NoOfFittedDevice\n");
                            //printf("#MD; DATA; 0x00000100; 0x%08X; 0x%08X; 0x%04X; 0x%04; 0x%01\n",(PageAdd+i), EESelected.MaxAddress, EESelected.NoOfPages, EESelected.PageSize, zSERMEMStatus.NoOfFittedDevice);
                            string sss = Frame.Replace(" ", ""); // remove white space. 
                            string[] partmd = sss.Split(delimiterChars);
                            //------------------------------------------------Downloaded informatics :
                            for (int i = 0; i < partmi.Count; i++)
                            {
                                switch (partmi[i])
                                {
                                    case ("DL_STARTADD"): myGlobalBase.EE_StartAdd = (int)Tools.HexStringtoUInt32(partmd[i]); break;
                                    case ("DL_ENDADD"): myGlobalBase.EE_EndAdd = (int)Tools.HexStringtoUInt32(partmd[i]); break;
                                    case ("MaxAddress"): myGlobalBase.EE_MaxAddress = (uint)Tools.HexStringtoUInt32(partmd[i]); break;
                                    case ("NoOfPages"): myGlobalBase.EE_NoOfPages = (uint)Tools.HexStringtoUInt32(partmd[i]); break;
                                    case ("PageSize"): myGlobalBase.EE_PageSize = (uint)Tools.HexStringtoUInt32(partmd[i]); break;
                                    case ("NoOfFittedDevice"): myGlobalBase.EE_NoOfDevice = (uint)Tools.HexStringtoUInt32(partmd[i]); break;
                                }
                            }
                        }
                        catch
                        {
                            myMainProg.myRtbTermMessageLF("###ERR: Error in EEPROM informatics frame, check code under $MD, revert to default setting");
                            myGlobalBase.EE_NoOfDevice = 1;                 // 1 device fitted. 
                            myGlobalBase.EE_Select_DeviceIC = 1;            // 1 = Select 1st device for erase purpose. 
                            //------------------------------------------------Default informatics : 1 x M25S16, this get updated after download log session via $MD frame. 
                            myGlobalBase.EE_StartAdd = 0x00000100;       // Page 0 reserved. 
                            myGlobalBase.EE_EndAdd = 0x00000100;
                            myGlobalBase.EE_MaxAddress = 0x001FFFFF;       // 2MB
                            myGlobalBase.EE_NoOfPages = 8192;
                            myGlobalBase.EE_PageSize = 256;

                        }
                        #endregion
                    }
                    #region//-------$BD decoder to Battery informatics
                    if (Frame.Contains("$BI"))
                    {
                        string sss = Frame.Replace(" ", ""); // remove white space. 
                        partbi = sss.Split(delimiterChars).ToList();
                        // Do nothing here.
                    }
                    if (Frame.Contains("$BD"))
                    {
                        try
                        {
                            //zprintf("$BI; BATTERY INFO; Capacity(%); EEBattState\n");     // TOCO/ESM
                            //printf("$BI; BATTERY INFO; Capacity(%)\n");                   // Old Project. 
                            //printf("$BD; 0x2710;0xFF116FAB\n");		                    // 10.000% = 10000 = 0x2710. Test Only.
                            string sss = Frame.Replace(" ", ""); // remove white space. 
                            string[] partbd = sss.Split(delimiterChars);
                            //------------------------------------------------Downloaded informatics :
                            for (int i = 0; i < partbi.Count; i++)
                            {
                                switch (partbi[i])
                                {
                                    case ("Capacity"): myGlobalBase.EE_BatteryCapacity = (int)Tools.HexStringtoUInt32(partbd[i]); break;
                                    case ("EEBattState"): myGlobalBase.EE_BatteryState = (uint)Tools.HexStringtoUInt32(partbd[i]); break;
                                }
                            }
                        }
                        catch
                        {
                            myGlobalBase.EE_BatteryCapacity = -1;           // unknown. 
                            myGlobalBase.EE_BatteryState = 0xFFFFFFFF;       // unknown. 
                        }
                    }
                    #endregion
                }
                myUDTmyFrameDataLog.UDTDataLogProcessFrame(Frame);
                tbMetaData1.Text = myUDTmyFrameDataLog.iCounterD.ToString();
                tbMetaData2.Text = myUDTmyFrameDataLog.iCounterG.ToString();
                tbError.Text = myUDTmyFrameDataLog.iCounterF.ToString();
                tbReport.Text = myUDTmyFrameDataLog.iCounterR.ToString();

            }
            //------------------------------------------------------        // This form column for DataGrid View.
            myUDTmyFrameDataLog.UDTDataLogProcessFramePostCollection();
            //------------------------------------------------------        // Treat as RAW file (must have ';' separator to qualify). 
            if (isConverted == false)                               
            {
                for (int i = 0; i < sFrame.Count; i++)
                {
                    //-----------------------------------------CRC8 Section
                    if (myCalc_CRC8.isContainCRC(sFrame[i], ref DataFrameNoCRC) == true)
                    {
                        if (tbMetaData6.Visible == false)
                        {
                            tbMetaData6.Visible = true;
                            tbMetaData7.Visible = true;
                            label6.Visible = true;
                            label8.Visible = true;
                            label6.Text = "CRC8 Passed";
                            label8.Text = "CRC8 Failed";
                        }
                        isCRCActive = true;
                        //DataFrame = DataFrameNoCRC;
                        DataFrame = sFrame[i];
                        myCalc_CRC8.UDTMessageFrameIsPassedCRC8(sFrame[i]);
                        tbMetaData6.Text = myCalc_CRC8.CRC8PassCounter.ToString("D");
                        tbMetaData7.Text = myCalc_CRC8.CRC8FailCounter.ToString("D");

                    }
                    else
                    {
                        DataFrame = sFrame[i];
                        DataFrame = DataFrame.Replace(",$E", "");
                        DataFrame = DataFrame.Replace(";$E", "");
                        DataFrame = DataFrame.Replace("\n", "");
                    }
                    //==================================================================================================// Reserved for LoggerCVS
                    if ((sFrame[i].Contains("$D")) | (sFrame[i].Contains("$G")))
                    {
                        int y = 0;                       
                        myDetoken.ClearAllVariable();
                        int DeTokenErrorCode = myDetoken.DetokeniserMessageLogStyle(DataFrame, myUDTmyFrameDataLog.FormatColumn, ref y);
                        if (DeTokenErrorCode != 0)
                        {
                            switch (DeTokenErrorCode)
                            {
                                case -1:        // Internal processing error: never happen here so there no bug in code. 
                                    {
                                        myMainProg.myRtbTermMessageLF("#E: LogMem Window: Conversion error , EndOfBulkFrame()");
                                        try
                                        { 
                                            myMainProg.myRtbTermMessageLF("#E: ItemNo:" + y.ToString() + "| Format: " + myUDTmyFrameDataLog.FormatColumn[y] + "| sData: " + myDetoken.sData[y].ToString());
                                        }
                                        catch
                                        {
                                            myMainProg.myRtbTermMessageLF("$E: ItemNo:" + y.ToString() + "| Format: ERROR |sData: ERROR (Check format frame $T)");
                                        }
                                    break;
                                    }
                                default:
                                    break;
                            }
                        }
                        //------------------------------------------------------------------------
                        ss = string.Join(",", myDetoken.sData);
                        sFrame.RemoveAt(i);
                        sFrame.Insert(i, ss);
                    }
                    //==================================================================================================// Reserved for ESM Project.
                    else if ((sFrame[i].Contains("$S")) | (sFrame[i].Contains("$P")))       
                    {
                        if (tscSelectMethod.SelectedIndex == 9)
                        {
                            int y = 0;
                            if (tbMetaData4.Visible == false)
                            {
                                tbMetaData4.Visible = true;
                                tbMetaData5.Visible = true;
                                label7.Visible = true;
                                label5.Visible = true;
                                label7.Text = "$S Data (ESM)";
                                label5.Text = "$P Data (ESM)";
                                tbMetaData4.Text = myUDTmyFrameDataLog.iCounterS.ToString();
                                tbMetaData5.Text = myUDTmyFrameDataLog.iCounterP.ToString();
                            }
                            myDetoken.ClearAllVariable();
                            int DeTokenErrorCode = myDetoken.DetokeniserMessageLogStyle(DataFrame, myUDTmyFrameDataLog.FormatColumn, ref y);
                            if (DeTokenErrorCode != 0)
                            {
                                switch (DeTokenErrorCode)
                                {
                                    case -1:        // Internal processing error: never happen here so there no bug in code. 
                                        {
                                            myMainProg.myRtbTermMessageLF("#E: LogMem Window: Conversion error , EndOfBulkFrame()");
                                            try
                                            {
                                                myMainProg.myRtbTermMessageLF("#E: ItemNo:" + y.ToString() + "| Format: " + myUDTmyFrameDataLog.FormatColumn[y] + "| sData: " + myDetoken.sData[y].ToString());
                                            }
                                            catch
                                            {
                                                myMainProg.myRtbTermMessageLF("$E: ItemNo:" + y.ToString() + "| Format: ERROR |sData: ERROR (Check format frame $T)");
                                            }
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            //------------------------------------------------------------------------
                            ss = string.Join(",", myDetoken.sData);
                            sFrame.RemoveAt(i);
                            sFrame.Insert(i, ss);
                        }
                    }
                    //==================================================================================================// Reserved for All Project (Error and Report Frame)
                    else if ((sFrame[i].Contains("$F")) | (sFrame[i].Contains("$R")))
                    {
                        //----------------------------------------------------ESM Project Only
                        if (myStatistic != null)
                        {
                            if (myStatistic.isUploadfinish == false)      // it goes to TRUE when 0xFE 
                            {
                                myStatistic.DecodeReportFrame(sFrame[i]);
                            }
                            if ((myStatistic.isUploadfinish == true) & (myStatistic.isUDTUpdated == false))
                            {
                                myStatistic.isUDTUpdated = true;
                                for (int j=0xE0; j<=0xFE; j++ )
                                {
                                    if (j == 0xEE) j = 0xFB;
                                    myMainProg.myRtbTermMessage(myStatistic.ReadMessage(j));
                                }
                                tbUDTStart.Text = myStatistic.luDataA[0].ToString("D");
                                tbUDTStop.Text = myStatistic.luDataA[1].ToString("D");
                                tbLastAddress.Text = "0x" + myStatistic.luDataB[0].ToString("X8");
                                tbNoOfFrame.Text = myStatistic.luDataA[2].ToString("D");
                                tbMetaData4.Text = "";
                                tbMetaData5.Text = "";
                                tbMetaData6.Text = "";
                                tbMetaData7.Text = "";
                            }
                        }
                        //----------------------------------------------------
                        string[] sData = sFrame[i].Split(delimiterChars);
                        for (int y = 1; y < sData.Length; y++)
                        {
                            sData[y] = sData[y].Replace("0u", "");
                            sData[y] = sData[y].Replace("0i", "");
                        }
                        if ((myUDTmyFrameDataLog.iStartUDT == 0) & (sData[2] == "0x10"))
                        {
                            myUDTmyFrameDataLog.iStartUDT = Tools.HexStringtoUInt32X(sData[1]);
                            tbUDTStart.Text = myUDTmyFrameDataLog.iStartUDT.ToString();
                        }
                        if ((myUDTmyFrameDataLog.iStopUDT == 0) & (sData[2] == "0x11"))
                        {
                            myUDTmyFrameDataLog.iStopUDT = Tools.HexStringtoUInt32X(sData[1]);
                            tbUDTStop.Text = myUDTmyFrameDataLog.iStopUDT.ToString();
                        }
                        ss = string.Join(",", sData);
                        ss += ",UDT=>,"+ Tools.HexStringtoUInt32X(sData[1]);      // Converted Hex to uINT32 for readability purpose. 
                        sFrame.RemoveAt(i);
                        sFrame.Insert(i, ss);
                    }
                    else
                    {
                        sFrame[i] = sFrame[i].Replace(';', ',');
                    }
                    
                }
                //-----------------------------------------------------End Frame loops
                RecievedData = string.Join(System.Environment.NewLine, sFrame);
                //--------------------------When done, save into separate filename as formatted. 
                filename = myBGMasterSetup.sFilenameDefaultDownloadedConverted;
                bool status = SurveyCVS_OpenSaveDataCloseFile(filename);
            }
            else    //-------------------------------------------------Converted string section
            {
                // Nothing here needed. 
            }
            //---------------------------------------------------------Calculate Battery Capacity left and % of EEPROM space used.
            if (isCRCActive == false)
            {
                double cap = System.Convert.ToDouble(myGlobalBase.EE_MaxAddress * myGlobalBase.EE_NoOfDevice);
                cap = Math.Round((System.Convert.ToDouble(myUDTmyFrameDataLog.CountByte) / cap) * 100, 3);
                tbMetaData6.Text = (cap.ToString("N3"));
                label6.Text = "Filled (%)";
                double bat = System.Convert.ToDouble(myGlobalBase.EE_BatteryCapacity);
                bat = Math.Round(bat / 1000, 3);     // Second to hours
                tbMetaData7.Text = bat.ToString("N3");
                label8.Text = "Battery (%)";
            }
            tbNoOfFrame.Text = (sFrame.Count+1).ToString();
            //--------------------------------------------------------- ESM Only to format datagridview column correctly
            if (myGlobalBase.SelectTools == (int)GlobalBase.eSelectTools.ADTESM)
            {
                myUDTmyFrameDataLog.UDTDataLogProcessFramePostCollectionESM();
            }
            //---------------------------------------------------------DataGridView
            dgvSurveyViewer.DataSource = null;      // clear table 1st. 
            myUDTmyFrameDataLog.InitAndProcessColumnTableHeader();
            ClearAndUpdateDataGridView(sFrame);
            //---------------------------------------------------------Download the driller Survey
            //DrillerSyncTimeStampandSave();
            mbOperationBusy.Close();
            //mbOperationBusy.DoneThenCloseMessageBox(2);
            return (true);
        }
        #endregion

        #region //================================================================ClearAndUpdateDataGridView
        private void ClearAndUpdateDataGridView(List<string> sFrameIn)
        {
            //---------------------------------------------Transfer Table to DGV
            myUDTmyFrameDataLog.UDTDataLogDGVTable(sFrameIn, cbHideG.Checked, cbHideD.Checked, cbHideFR.Checked, cbHideG.Checked, cbHideD.Checked);
            if (myUDTmyFrameDataLog.DGVtable != null)
            {
                dgvSurveyViewer.DataSource = myUDTmyFrameDataLog.DGVtable;
                //--------------------------------------------Tidy Up DGV.
                dgvSurveyViewer.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);    // Resize the master DataGridView columns to fit the newly loaded data.
                dgvSurveyViewer.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvSurveyViewer.RowHeadersWidth = 60;
            }
            //---------------------------------------------Insert Row number on Row header for reference purpose.
            foreach (DataGridViewRow row in dgvSurveyViewer.Rows)
            {
                row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
            }
        }
        #endregion

        #region //================================================================btnLoadSurveyFile_Click
        private void btnLoadSurveyFile_Click(object sender, EventArgs e)
        {
            bool status = SurveyCVS_ReadRawFile("", true);
            tbMetaData5.Text = "";
            tbMetaData6.Text = "";
            //--------------------------------------------------Is this converted or raw file?
            string stest = "";
            foreach (string Frame in sFrame)
            {
                if (Frame.Contains("$D"))
                {
                    stest = Frame;
                    break;
                }
            }
            EndOfBulkFrame(stest.Contains(','));        // Get converted if it contains ';', the converted all have ',' for excel.  
        }
        #endregion

        #region//================================================================btnStopDownload_Click
        private void btnStopDownload_Click(object sender, EventArgs e)
        {

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Received Data (Logger CVS)
        //#####################################################################################################
        // This come from LoggerCVS which transfer one element of string at a time.
        // In case of no Header or Format frame (in performance of survey). It take up the $S/$G/$D/$P and take maximum column. 
        // When thus window open then it set myUDTmyFrameDataLog = null.
        #region//================================================================SurveyLoggerCVS_InitSetup
        public void SurveyLoggerCVS_InitSetup()
        {
            if (myUDTmyFrameDataLog==null)
                myUDTmyFrameDataLog = new UDTmyFrameDataLog(); // which include ClearAllVariable and DGVtable = null;
            myUDTmyFrameDataLog.ClearAllVariable();
            tbMetaData4.Visible = true;
            tbMetaData5.Visible = true;
            label7.Visible = true;
            label5.Visible = true;
            label7.Text = "$S Data (ESM)";
            label5.Text = "$P Data (ESM)";
            tbMetaData4.Text = myUDTmyFrameDataLog.iCounterS.ToString();
            tbMetaData5.Text = myUDTmyFrameDataLog.iCounterP.ToString();
            if (sFrame == null)
                sFrame = new List<string>();
            sFrame.Clear();
            //-----------------------------------// Modify the MetaData Box so it decide how to use it
            tbMetaData6.Visible = false;
            tbMetaData7.Visible = false;
            label6.Text = "";
            label8.Text = "";
            //-----------------------------------
            if (myCalc_LoggerCRC8 == null)
                myCalc_LoggerCRC8 = new CRC8CheckSum();
            else
                myCalc_LoggerCRC8.CRC8CounterReset();
        }
        #endregion

        #region//================================================================SurveyLoggerCVS_RecievedData
        public void SurveyLoggerCVS_RecievedData(string DataFrame)
        {
            if (DataFrame == "")
                return;
            DataFrame = DataFrame.Replace(" ", "");
            DataFrame = DataFrame.Replace("\0", "");        // Aug16
            int n = DataFrame.IndexOf('$');
            if (n == -1)                       // Not found.
                return;
            DataFrame = DataFrame.Substring(n);
            //RecievedData += DataFrame;
            myUDTmyFrameDataLog.UDTDataLogProcessFrame(DataFrame);            // Detect $H, $T and counts frame. 
            tbMetaData1.Text = myUDTmyFrameDataLog.iCounterD.ToString();
            tbMetaData2.Text = myUDTmyFrameDataLog.iCounterG.ToString();
            tbMetaData4.Text = myUDTmyFrameDataLog.iCounterS.ToString();
            tbMetaData5.Text = myUDTmyFrameDataLog.iCounterP.ToString();
            tbError.Text = myUDTmyFrameDataLog.iCounterF.ToString();
            tbReport.Text = myUDTmyFrameDataLog.iCounterR.ToString();
            //----------------------------------------------------------------Sort out the mess on 1st frame which contains unwanted text. 
            SurveyLoggerCVS_AppendDGV(DataFrame);
        }
        #endregion

        #region//================================================================SurveyLoggerCVS_AppendDGV
        // isLoggerCVS = true if sourced from LoggerCVS, other set to false if sourced from EEPROM.
        // isConverted = true if source from loaded file, other set to false if from EEPROM Raw Data download.
        private bool SurveyLoggerCVS_AppendDGV(string DataFrameIN)       // false = data not converted, true = data was converted. 
        {
            string DataFrameNoCRC = "";
            DataFrameIN = DataFrameIN.Replace("\n", "");
            DataFrameIN = DataFrameIN.Replace("\r", "");
            string DataFrame = DataFrameIN;
            try
            {
                btnDownhole.Enabled = true;
                //----------------------------------------Remove $E and /n end line
                if (myCalc_LoggerCRC8.isContainCRC(DataFrameIN, ref DataFrameNoCRC) == true)
                {
                    if (tbMetaData6.Visible == false)
                    {
                        tbMetaData6.Visible = true;
                        tbMetaData7.Visible = true;
                    }
                    myCalc_LoggerCRC8.UDTMessageFrameIsPassedCRC8(DataFrameIN);
                    tbMetaData6.Text = myCalc_LoggerCRC8.CRC8PassCounter.ToString("D");
                    tbMetaData7.Text = myCalc_LoggerCRC8.CRC8FailCounter.ToString("D");
                    label6.Text = "CRC8 Passed";
                    label8.Text = "CRC8 Failed";
                }
//                 else
//                 {
//                     DataFrame = DataFrameIN;
//                     DataFrame = DataFrame.Replace(",$E", "");
//                     DataFrame = DataFrame.Replace(";$E", "");
//                 }
                myUDTmyFrameDataLog.UDTDataLogLoggerCVSProcessFramePostCollection(DataFrame);
                myUDTmyFrameDataLog.InitAndProcessColumnTableHeader();
                //###############################################################################  $G and $D, detoken parameter
                //-----------------------------------------
                if ((DataFrame.Contains("$D")) | (DataFrame.Contains("$G")))        // Data and Data2 Frame.
                {
                    int y=0;
                    Frame_Detokeniser myDetoken = new Frame_Detokeniser();
                    int DeTokenErrorCode = myDetoken.DetokeniserMessageLogStyle(DataFrame, myUDTmyFrameDataLog.FormatColumn, ref y);
                    if (DeTokenErrorCode != 0)
                    {
                        switch (DeTokenErrorCode)
                        {
                            case -1:        // Internal processing error: never happen here so there no bug in code. 
                                {
                                    myMainProg.myRtbTermMessageLF("#E: LogMem Window: Conversion error , SurveyLoggerCVS_AppendDGV()");
                                    myMainProg.myRtbTermMessageLF("#E: ItemNo:" + y.ToString() + "| Format: " + myUDTmyFrameDataLog.FormatColumn[y] + "| sData: " + myDetoken.sData[y].ToString());
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    //------------------------------------------------------------------------
                    DataFrame = string.Join(",", myDetoken.sData);
                }
                else
                {
                    DataFrame = DataFrame.Replace(';', ',');
                }
                //###############################################################################  $S and $P, detoken parameter
                //-----------------------------------------
                if ((DataFrame.Contains("$S")) | (DataFrame.Contains("$P")))        // Data and Data2 Frame.
                {
                    int y = 0;
                    Frame_Detokeniser myDetoken = new Frame_Detokeniser();
                    int DeTokenErrorCode = myDetoken.DetokeniserMessageLogStyle(DataFrame, myUDTmyFrameDataLog.FormatColumn, ref y);
                    if (DeTokenErrorCode != 0)
                    {
                        switch (DeTokenErrorCode)
                        {
                            case -1:        // Internal processing error: never happen here so there no bug in code. 
                                {
                                    myMainProg.myRtbTermMessageLF("#E: LogMem Window: Conversion error , SurveyLoggerCVS_AppendDGV()");
                                    myMainProg.myRtbTermMessageLF("#E: ItemNo:" + y.ToString() + "| Format: " + myUDTmyFrameDataLog.FormatColumn[y] + "| sData: " + myDetoken.sData[y].ToString());
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    //------------------------------------------------------------------------
                    DataFrame = string.Join(",", myDetoken.sData);
                }
                else
                {
                    DataFrame = DataFrame.Replace(';', ',');
                }
                sFrame.Add(DataFrame);
                ClearAndUpdateDataGridView(sFrame);

                //###############################################################################  $G, transfer to column
                //---------------------------------------------------------DataGridView
                //                 if ((DataFrame.Contains("$G")) & (cbHideG.Checked == false))
                //                 {
                //                     string[] partdf = DataFrame.Split(delimiterChars);
                //                     DataRow dr = DGVtable.NewRow();
                //                     //---------------------------------------------Add header to Column.
                //                     for (int i = 0; i < ColumnNumber; i++)
                //                     {
                //                         if (i < partdf.Length)
                //                             dr[i] = partdf[i].ToString();
                //                         else
                //                             dr[i] = "";
                //                     }
                //                     DGVtable.Rows.Add(dr);
                //                     //DGVtable.Rows.Add(DataFrame.Split(','));
                //                 }
                //###############################################################################  $D, transfer to column.
                //                 if ((DataFrame.Contains("$D")) & (cbHideD.Checked == false))
                //                 {
                //                     string[] partdf = DataFrame.Split(delimiterChars);
                //                     DataRow dr = DGVtable.NewRow();
                //                     //---------------------------------------------Add header to Column.
                //                     for (int i = 0; i < ColumnNumber; i++)
                //                     {
                //                         if (i < partdf.Length)
                //                             dr[i] = partdf[i].ToString();
                //                         else
                //                             dr[i] = "";
                //                     }
                //                     DGVtable.Rows.Add(dr);
                //                     //DGVtable.Rows.Add(DataFrame.Split(','));
                //                 }
                //###############################################################################  $F/$R report frame
                //                 if ((DataFrame.Contains("$F")) | (DataFrame.Contains("$R")))            // Upgraded Jun 18 for ToCo Project. 
                //                 {
                //                     DGVtable.Rows.Add(DataFrame.Split(','));
                //                 }
                //---------------------------------------------Insert Row number on Row header for reference purpose.
                foreach (DataGridViewRow row in dgvSurveyViewer.Rows)
                {
                    row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
                }
                dgvSurveyViewer.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);    // Resize the master DataGridView columns to fit the newly loaded data.
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("LogMem Window: Message Frame Error in SurveyLoggerCVS_AppendDGV()");
            }
            //---------------------------------------------Auto scroll management. 
            if (cbSuspendScroll.Checked == false)
            {
                try  // Had to this to avoid crashes when RowCount=0 (negative number). 
                {
                    if (dgvSurveyViewer.RowCount != 0)
                        dgvSurveyViewer.FirstDisplayedScrollingRowIndex = dgvSurveyViewer.RowCount - 1;     // view to last row. 
                }
                catch { }
            }
            return (true);
        }
        #endregion

        #region//================================================================SurveyLoggerCVS_Start
        public delegate void SurveyLoggerCVS_StartDelegate();
        //Solution: http://www.dreamincode.net/forums/topic/188209-cross-thread-calls-made-easy/ 
        public void SurveyLoggerCVS_Start()
        {
            // Check if we need to call BeginInvoke.
            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false.
                this.BeginInvoke(new SurveyLoggerCVS_StartDelegate(SurveyLoggerCVS_Start));
                return;
            }
            InitForSurveyCVS_RecievedData();
            myUDTmyFrameDataLog.ClearAllVariable();
            //--------------------------------------Send Message to start download.
            Console.Beep(1000, 20);
        }
        #endregion

        #region//================================================================SurveyUpdateFolderName
        public void SurveyUpdateFolderName(string sFoldername)
        {
            if (myBGMasterSetup == null)
                return;
            if (Directory.Exists(sFoldername))
            {
                myBGMasterSetup.sFile_TopFolderProjectAll = sFoldername;
                myBGMasterSetup.sFile_JobProjectAll = sFoldername;
                txtFolderName.Text = myBGMasterSetup.sFile_JobProjectAll;
                myBGMasterSetup.sFile_DriveAll = "";
                string drivename = sFoldername.Substring(0, 2);               // J://SurveyProject//JobName
                myBGMasterSetup.sFile_DriveAll = (string)drivename;           // J:
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: FolderName Not Valid or Drive do not exist, update this now!");
                txtFolderName.Text = "";
                btnDownhole.Enabled = false;
                myBGMasterSetup.sFile_JobProjectAll = "";
                myBGMasterSetup.sFile_TopFolderProjectAll = "";
                txtFolderName.Text = "";
                myBGMasterSetup.sFile_DriveAll = "";
            }
        }
        #endregion

        #region//================================================================SurveyLoggerCVS_Stop
        public void SurveyLoggerCVS_Stop()
        {
            myGlobalBase.EE_isSurveyCVSRawLogDataActive = false;
            myGlobalBase.EE_isLogMemFrameTransferMode = false;
            myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate = false;
            myMainProg.myRtbTermMessageLF("---LogMem: End of Survey Data Transfer");
            bool status = SurveyCVS_OpenSaveDataCloseFile(myBGMasterSetup.sFilenameDefaultDownloadedRaw);
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### File Manager
        //#####################################################################################################

        #region //================================================================Load_DrillerFile
        private bool Load_DrillerFile()
        {
            try
            {
                blBGMasterDrillerFile = null;
                string filename = myBGMasterSetup.sFile_JobProjectAll + "\\" + myBGMasterSetup.sFilenameDefaultDrillerSurvey + ".xml";
                blBGMasterDrillerFile = myGlobalBase.DeserializeFromFile<BindingList<BGMasterDriller>>(filename);
                if (blBGMasterDrillerFile == null)
                    return (false);
            }
            catch
            {
                return (false);
            }
            return (true);
        }
        #endregion

        #region //================================================================btnViewDrillerFile_Click
        private void btnViewDrillerFile_Click(object sender, EventArgs e)
        {
            //myBGDrillerViewer.BG_DrillerView_Show();

        }
        #endregion

        #region //================================================================SurveyCVS_HybridFrameSaveFile
        bool SurveyCVS_HybridFrameNewFile(string sfilenametitle)
        {
            try
            {
                FileStream fsa = null;
                try
                {
                    fsa = new FileStream(sfilenametitle, FileMode.Append, FileAccess.Write, FileShare.Write);
                }
                finally
                {
                    if (fsa != null)
                    {
                        fsa.Dispose();
                    }
                    myMainProg.myRtbTermMessageLF("-INFO : Created new Dataframe Files : " + sfilenametitle);
                }
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("#ERR : Unable to create Dataframe File: " + sfilenametitle);
                myMainProg.myRtbTermMessageLF("+WARN: Do not save file into Drive C except user folder");
                return (false);
            }
            return (true);
        }
        #endregion 

        #region //================================================================SurveyCVS_HybridFrameSaveFile
        bool SurveyCVS_HybridFrameSaveFile(string sfilenametitle, string sdata)
        {
            StreamWriter sw = null;
            try
            {
                try
                {
                    sw = new StreamWriter(sfilenametitle, true, Encoding.ASCII);
                    sw.Write(sdata);                                     // Dataframe (assuming it has \n\0 terminator)
                    //myMainProg.myRtbTermMessageLF("-INFO : Dataframe Appended to File: " + sfilenametitle);
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
                myMainProg.myRtbTermMessageLF("#ERR: Unable to append file : " + sfilenametitle);
                return (false);
            }
            return (true);
        }
        #endregion

        #region //================================================================SurveyCVS_OpenSaveDataCloseFile
        bool SurveyCVS_OpenSaveDataCloseFile(string sfilenametitle)
        {
            string sFilename = System.IO.Path.Combine(myBGMasterSetup.sFile_JobProjectAll, sfilenametitle + DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + ".csv");
            myMainProg.myRtbTermMessageLF("-INFO: Saving Downloaded Filename: " + sFilename + ".csv");
            try
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(sFilename, true, Encoding.UTF8);          // 02/04/19: Used to be ASCII, but better use UTF8 instead
                    sw.Write(RecievedData);                                         // Dataframe (assuming it has \n\0 terminator)

                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Dispose();
                    }
                }
                return true;
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("###ERR: Unable to save file, try changing drive/folder and repeat download");
                MessageBox.Show("Unable to save file, try changing drive/folder and repeat download",
                       "Survey CVS filename Error",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Exclamation,
                       MessageBoxDefaultButton.Button1);
            }
            return false;

        }
        #endregion

        #region //================================================================SurveyCVS_ReadRawFile
        bool SurveyCVS_ReadRawFile(string filename, bool openDialogue)     // if filename is "", it open the load file dialogue for you.
        {
            string sFilename = ""; ;
            OpenFileDialog ofdReadSurvey = null;
            myMainProg.myRtbTermMessageLF("-INFO: Loading Raw/Converted Filename");
            //-----------------------------------------------
            if (sFrame == null)                            // if Frame is null then init. 
                sFrame = new List<string>();
            else
                sFrame.Clear();
            //-----------------------------------------------
            try
            {
                if (filename != "")                                 // filename
                {
                    sFilename = filename;
                }
                else                                                // No filename, allow user to select.
                {
                    if (openDialogue == false)                      // Open dialogue is not allowed.
                    {
                        return false;
                    }
                    ofdReadSurvey = null;
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    ofdReadSurvey = openFileDialog;
                    ofdReadSurvey.FileName = myBGMasterSetup.sFile_JobProjectAll;            //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    ofdReadSurvey.RestoreDirectory = true;
                    ofdReadSurvey.AutoUpgradeEnabled = false;
                    ofdReadSurvey.Filter = "csv files (*.csv)|*.csv";

                    ofdReadSurvey.Title = "Load Survey CVS File";
                    DialogResult dr = ofdReadSurvey.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        sFilename = ofdReadSurvey.FileName;

                    }
                    else
                    {
                        myMainProg.myRtbTermMessageLF("###ERR: Unable to load file");
                        return false;
                    }
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
                myMainProg.myRtbTermMessageLF("###ERR: Unable to load file");
                return false;
            }
            return true;
        }
        #endregion

        #region //================================================================btnProjectFolder_Click
        private void btnProjectFolder_Click(object sender, EventArgs e)
        {
            string sFoldername = myBGMasterSetup.sFile_JobProjectAll;
            try
            {
                //----------------------------------------------------------
                folderBrowserDialog1.SelectedPath = sFoldername;
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    myBGMasterSetup.sFile_JobProjectAll = folderBrowserDialog1.SelectedPath;
                    txtFolderName.Text = myBGMasterSetup.sFile_JobProjectAll;
                    myBGMasterSetup.sFile_TopFolderProjectAll = folderBrowserDialog1.SelectedPath;
                    myBGMasterSetup.sFile_DriveAll = "";
                    string drivename = folderBrowserDialog1.SelectedPath.Substring(0, 2);               // J://SurveyProject//JobName
                    myBGMasterSetup.sFile_DriveAll = (string)drivename;           // J:
                }
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("###ERR: Folder Dialog: Reported an Error");
            }
        }
        #endregion

        #region //================================================================BtnOpenFolder_Click
        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string myPath = myBGMasterSetup.sFile_JobProjectAll;
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

        //#####################################################################################################
        //###################################################################################### RiscyScope & Chart Interface
        //#####################################################################################################

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
                    //System.Windows.Forms.MenuItem menuItemx5 = new System.Windows.Forms.MenuItem("X Axis: RIT");
                    //menuItemx5.Click += new EventHandler(ContexSelectRIT);
                    System.Windows.Forms.MenuItem menuItemx6 = new System.Windows.Forms.MenuItem("NoOfRow:" + dgvSurveyViewer.RowCount.ToString());
                    contextMenuX.MenuItems.Add(menuItemx1);
                    contextMenuX.MenuItems.Add(menuItemx2);
                    contextMenuX.MenuItems.Add(menuItemx3);
                    contextMenuX.MenuItems.Add(menuItemx4);
                    //contextMenuX.MenuItems.Add(menuItemx5);
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

        #region //================================================================Open ScopeWindow Event: ContexMenuCH0
        private void ContexMenuCH0(object sender, EventArgs e)
        {
            DataGridViewColumn dgvColumnX;
            for (int i = 0; i < 4; i++)
            {
                if (i != 0)
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
            ChartSelectedStartRow = 0;
            ChartSelectedEndRow = dgvSurveyViewer.RowCount;
            if (ScopeCH0 == null)
            {
                ScopeCH0 = new List<cPoint>();
            }
            float fdata;
            float xx;
            for (int row = ChartSelectedStartRow; row < ChartSelectedEndRow; row++)
            {
                if (cbChartXRIT.Checked == false)
                {
                    xx = row;
                }
                else
                {
                    bool bx = float.TryParse(dgvSurveyViewer.Rows[row].Cells[2].Value.ToString(), out fdata);
                    xx = fdata;
                }
                bool by = float.TryParse(dgvSurveyViewer.Rows[row].Cells[ChartSelectedColumn].Value.ToString(), out fdata);
                ScopeCH0.Add(new cPoint(xx, fdata));
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
            ChartSelectedStartRow = 0;
            ChartSelectedEndRow = dgvSurveyViewer.RowCount;
            if (ScopeCH1 == null)
            {
                ScopeCH1 = new List<cPoint>();
            }
            float fdata;
            float xx;
            for (int row = ChartSelectedStartRow; row < ChartSelectedEndRow; row++)
            {
                if (cbChartXRIT.Checked == false)
                {
                    xx = row;
                }
                else
                {
                    bool bx = float.TryParse(dgvSurveyViewer.Rows[row].Cells[2].Value.ToString(), out fdata);
                    xx = fdata;
                }
                bool by = float.TryParse(dgvSurveyViewer.Rows[row].Cells[ChartSelectedColumn].Value.ToString(), out fdata);
                ScopeCH1.Add(new cPoint(xx, fdata));
            }
            //---------------------------------------------Update Chart
            Color linecolor = Color.MediumSeaGreen;
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
            ChartSelectedStartRow = 0;
            ChartSelectedEndRow = dgvSurveyViewer.RowCount;
            if (ScopeCH2 == null)
            {
                ScopeCH2 = new List<cPoint>();
            }
            float fdata;
            float xx;
            for (int row = ChartSelectedStartRow; row < ChartSelectedEndRow; row++)
            {
                if (cbChartXRIT.Checked == false)
                {
                    xx = row;
                }
                else
                {
                    bool bx = float.TryParse(dgvSurveyViewer.Rows[row].Cells[2].Value.ToString(), out fdata);
                    xx = fdata;
                }
                bool by = float.TryParse(dgvSurveyViewer.Rows[row].Cells[ChartSelectedColumn].Value.ToString(), out fdata);
                ScopeCH2.Add(new cPoint(xx, fdata));
            }
            //---------------------------------------------Update Chart
            Color linecolor = Color.Cyan; ;
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
            ChartSelectedStartRow = 0;
            ChartSelectedEndRow = dgvSurveyViewer.RowCount;
            if (ScopeCH3 == null)
            {
                ScopeCH3 = new List<cPoint>();
            }
            float fdata;
            float xx;
            for (int row = ChartSelectedStartRow; row < ChartSelectedEndRow; row++)
            {
                if (cbChartXRIT.Checked == false)
                {
                    xx = row;
                }
                else
                {
                    bool bx = float.TryParse(dgvSurveyViewer.Rows[row].Cells[2].Value.ToString(), out fdata);
                    xx = fdata;
                }
                bool by = float.TryParse(dgvSurveyViewer.Rows[row].Cells[ChartSelectedColumn].Value.ToString(), out fdata);
                ScopeCH3.Add(new cPoint(xx, fdata));
            }
            //---------------------------------------------Update Chart
            Color linecolor = Color.MediumPurple; ;
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
            tbx.AppendText("Format Frame: $T.\r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("When $T (Format Frame) is emitted from LoggerCVS test   \r\n");
            tbx.AppendText("code after the $H header frame. It decode and format    \r\n");
            tbx.AppendText("the raw data into representative number for each column.\r\n");
            tbx.AppendText("With Format Frame, the prefix on Data is not required  \r\n");
            tbx.AppendText(" ie 0xFF23;0x0234;0xFFFF12F4 => FF23;0234;FFFF12F4      \r\n");
            tbx.AppendText("Below is the format type data in UDTTerm Protocol       \r\n");
            tbx.AppendText("  0y      Date            Obsolete (BG), use UDT1970    \r\n");
            tbx.AppendText("  0z      Time            Obsolete (BG), use UDT1970    \r\n");
            tbx.AppendText("  0q      uINT16 Hex      iPara     \r\n");
            tbx.AppendText("  0w      INT16 Hex       iPara     \r\n");
            tbx.AppendText("  0i      INT32           iPara     \r\n");
            tbx.AppendText("  0u      uINT32 Hex      iPara     \r\n");
            tbx.AppendText("  0d      double          dPara     \r\n");
            tbx.AppendText("  0x      uINT32 Hex      hPara (display as hex)\r\n");
            tbx.AppendText("  0n      INT32 Hex       iPara (0FFF2A56=> neg num)   \r\n");
            tbx.AppendText("  0l      uINT32          iPara     \r\n");
            tbx.AppendText("  0s      string          sPara     \r\n");
            tbx.AppendText("  0c      char            sPara (future)    \r\n");
            tbx.AppendText("  undef   string          sPara     Undefined type      \r\n");
            tbx.AppendText("  Approved separator: ';' and ',' and ':'.              \r\n");
            tbx.AppendText("  (Alway use default ';' for all project)               \r\n");
            tbx.AppendText(" Example $T;0i;0i;0s;0i;0i;0i;0i;0x;0x;$E\\n            \r\n");
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
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("Command Note <+STCUPLOAD() & LOG_DOWNLOAD()>\r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText(" Old Command LOG_DOWNLOAD() is superseded by +STCUPLOAD()\r\n");
            tbx.AppendText(" For new project, use +STCUPLOAD(Mode;Para1;Para2)\r\n");
            tbx.AppendText(" Select 12 = LOG_DOWNLOAD() for older project (pre 2019)\r\n");
            frm.ShowDialog();

            // Drag the PDF file into PDFBin
            // For each PDF file properties, change to 
            //   Build Action       = Content 
            //   Copy to Output Dir = Alway Includes
            // modify the filename below. This should work. 
            /*
            string filename = Path.GetFullPath(Path.Combine(Application.StartupPath, ".\\PDFBin\\700137_BGDriller_QuickUserGuide020.pdf"));
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = filename,
                    UseShellExecute = true,
                    Verb = "open"
                });
                return;
            }
            catch { }
            try     // Failing opening the PDF filename or lack of PDF, then try again by opening the folder. 
            {
                filename = Path.GetFullPath(Path.Combine(Application.StartupPath, ".\\PDFBin\\"));
                Process.Start(filename);
            }
            catch
            {
                JR.Utils.GUI.Forms.FlexiMessageBox.Show("File Error for accessing PDF file/folder",
                    "PDF File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            */
        }
        #endregion

        #region //================================================================cbChartXNone_MouseClick
        private void cbChartXNone_MouseClick(object sender, MouseEventArgs e)
        {
            cbChartXNone.Checked = true;
            cbChartXRIT.Checked = false;
            //------------------------------Remove All from chart to start again. 
            try
            {
                myRiscyScope.RemoveCh0();
                myRiscyScope.RemoveCh1();
                myRiscyScope.RemoveCh2();
                myRiscyScope.RemoveCh3();
                for (int i = 0; i < 4; i++)
                {
                    DataGridViewColumn dgvColumnX;
                    if (ChartSelectedCh[i] != -1)
                    {
                        dgvColumnX = dgvSurveyViewer.Columns[ChartSelectedCh[i]];
                        dgvColumnX.HeaderCell.Style.BackColor = System.Drawing.SystemColors.Control;
                        dgvColumnX.HeaderCell.Style.ForeColor = Color.Black;
                        ChartSelectedCh[i] = -1;
                    }
                }
            }
            catch { }
        }
        #endregion

        #region //================================================================cbChartXRIT_MouseClick
        private void cbChartXRIT_MouseClick(object sender, MouseEventArgs e)
        {
            cbChartXNone.Checked = false;
            cbChartXRIT.Checked = true;
            //------------------------------Remove All from chart to start again. 
            try
            {
                myRiscyScope.RemoveCh0();
                myRiscyScope.RemoveCh1();
                myRiscyScope.RemoveCh2();
                myRiscyScope.RemoveCh3();
                for (int i = 0; i < 4; i++)
                {
                    DataGridViewColumn dgvColumnX;
                    if (ChartSelectedCh[i] != -1)
                    {
                        dgvColumnX = dgvSurveyViewer.Columns[ChartSelectedCh[i]];
                        dgvColumnX.HeaderCell.Style.BackColor = System.Drawing.SystemColors.Control;
                        dgvColumnX.HeaderCell.Style.ForeColor = Color.Black;
                        ChartSelectedCh[i] = -1;
                    }
                }
            }
            catch { }

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
                    DataGridViewCell clickedCell = (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex];

                    // Here you can do whatever you want with the cell
                    this.dgvSurveyViewer.CurrentCell = clickedCell;  // Select the clicked cell, for instance

                    //----------------------------------------------Add context menu
                    System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
                    System.Windows.Forms.MenuItem menuItem1 = new System.Windows.Forms.MenuItem("Save");
                    menuItem1.Click += new EventHandler(ContexMenuSaveAction);
                    System.Windows.Forms.MenuItem menuItem2 = new System.Windows.Forms.MenuItem("Copy");
                    menuItem2.Click += new EventHandler(ContexMenuCopyAction);
                    contextMenu.MenuItems.Add(menuItem1);
                    contextMenu.MenuItems.Add(menuItem2);
                    //----------------------------------------------
                    try
                    {
                        DataGridViewCell clickedStart = (sender as DataGridView).Rows[e.RowIndex].Cells[0];
                        DataGridViewCell clickedRTC = (sender as DataGridView).Rows[e.RowIndex].Cells[6];       // RTC column. 
                        DataGridViewCell clickedStatus = (sender as DataGridView).Rows[e.RowIndex].Cells[8];    // Status Information
                    }
                    catch { }

                    //if (clickedStart.Value.ToString().Contains("$D") == true)
                    //{
                    //    TimeSpan time = TimeSpan.FromSeconds(Convert.ToDouble(clickedRTC.Value.ToString()));

                    //    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).Add(time);
                    //    contextMenu.MenuItems.Add(new MenuItem(string.Format("UTC: " + clickedRTC.Value.ToString())));
                    //    contextMenu.MenuItems.Add(new MenuItem(string.Format("Time: {0:HH/mm/ss}", dateTime)));
                    //    contextMenu.MenuItems.Add(new MenuItem(string.Format("Date: {0:dd/MM/yy}", dateTime)));
                    //    if (clickedCell.ColumnIndex == 8)
                    //    {
                    //        contextMenu.MenuItems.Add(new MenuItem("------------StatusFlags"));
                    //        ToolStatusWord flagStatus = new ToolStatusWord();
                    //        flagStatus.zLoggingToolStatus = Tools.HexStringtoUInt32(clickedStatus.Value.ToString());
                    //        for (int i=0; i< flagStatus.maxarrayno;i++)
                    //        {
                    //            string ss = flagStatus.ContextMenuInsertString(i);
                    //            if (ss!="")
                    //                contextMenu.MenuItems.Add(new MenuItem(ss));
                    //        }
                    //    }
                    //}

                    var relativeMousePosition = dgvSurveyViewer.PointToClient(Cursor.Position);
                    contextMenu.Show(dgvSurveyViewer, relativeMousePosition);
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
            string Rdata = myUDTmyFrameDataLog.HeaderFrame + '\n';
            string sb = "";
            //-----------------------------------------------reverse or forward order
            if (dgvSurveyViewer.SelectedRows[0].Index > dgvSurveyViewer.SelectedRows[dgvSurveyViewer.SelectedRows.Count - 1].Index)
            {
                List<DataGridViewRow> rows = (from DataGridViewRow row in dgvSurveyViewer.SelectedRows where !row.IsNewRow orderby row.Index select row).ToList<DataGridViewRow>();
                for (int i = 0; i < rows.Count; i++)
                {
                    sb = "";
                    for (int col = 0; col < rows[i].Cells.Count; col++)
                    {
                        if (rows[i].Cells[col].Value.ToString() != "")
                        {
                            sb = sb + rows[i].Cells[col].Value.ToString() + ',';
                            sb = sb.Replace("\r", "");                               // remove certain char that causing spurious issue in excel. 
                            sb = sb.Replace("\n", "");
                            sb = sb.Replace("\0", "");
                        }
                    }
                    Rdata = Rdata + sb + '\n'; // that will give you the string you desire

                }
            }
            else
            {
                foreach (DataGridViewRow row in dgvSurveyViewer.SelectedRows)
                {
                    sb = "";
                    for (int col = 0; col < row.Cells.Count; col++)
                    {
                        if (row.Cells[col].Value.ToString() != "")
                        {
                            sb = sb + row.Cells[col].Value.ToString() + ',';
                            sb = sb.Replace("\r", "");                               // remove certain char that causing spurious issue in excel. 
                            sb = sb.Replace("\n", "");
                            sb = sb.Replace("\0", "");
                        }
                    }
                    Rdata += sb + '\n'; // that will give you the string you desire
                }

            }
            System.Windows.Forms.Clipboard.SetText(Rdata);
            myMainProg.myRtbTermMessageLF("-INFO: Selected Data in Clipboard Successfully");
        }

        #endregion

        #region //================================================================ContexMenuSaveAction
        // The code is what you see is what you capture, ie taken from Datagridview not from sframe[].
        // It also sort polarity to ensure correct timing sequence.
        // It filter out spurious \r and then add \n. It has two comma left but no serious impact on excel or notepad view. 
        // Fully tested and working fine. 
        private void ContexMenuSaveAction(object sender, EventArgs e)
        {
            RecievedData = myUDTmyFrameDataLog.HeaderFrame + '\n';
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
                            sb.Append(rows[i].Cells[col].Value.ToString() + ',');
                            sb.Replace("\r", "");                               // remove certain char that causing spurious issue in excel. 
                            sb.Replace("\n", "");
                            sb.Replace("\0", "");

                        }
                    }
                    RecievedData += sb.ToString() + '\n'; // that will give you the string you desire
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
                            sb.Append(row.Cells[col].Value.ToString() + ',');
                            sb.Replace("\r", "");                               // remove certain char that causing spurious issue in excel. 
                            sb.Replace("\n", "");
                            sb.Replace("\0", "");
                        }
                    }
                    RecievedData += sb.ToString() + '\n'; // that will give you the string you desire
                }

            }

            //-------------------------------------------------Save to filename under SelectSave_<timestamp>
            SurveyCVS_OpenSaveDataCloseFile(myBGMasterSetup.sFilenameDefaultDownloadedSelected);
            myMainProg.myRtbTermMessageLF("-INFO: Selected Data Saved Successfully");
            RecievedData = null;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### DGV
        //#####################################################################################################

        #region //================================================================btnReset_Click (Reset Sort)
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (myUDTmyFrameDataLog != null)
            {
                btnReset.Visible = false;
                myUDTmyFrameDataLog.DGVtable.DefaultView.Sort = string.Empty;
                this.Refresh();
            }
        }
        #endregion

        #region //================================================================Sort Reset Button Control via MouseLeave and CellMouseUp Event. 

        private void dgvSurveyViewer_MouseLeave(object sender, EventArgs e)
        {
            if (myUDTmyFrameDataLog != null)
            {
                if (myUDTmyFrameDataLog.DGVtable != null)
                {
                    if (myUDTmyFrameDataLog.DGVtable.DefaultView.Sort != string.Empty)
                    {
                        btnReset.Visible = true;
                    }
                }
            }
        }
        private void dgvSurveyViewer_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (myUDTmyFrameDataLog != null)
            {
                if (myUDTmyFrameDataLog.DGVtable != null)
                {
                    if (myUDTmyFrameDataLog.DGVtable.DefaultView.Sort != string.Empty)
                    {
                        btnReset.Visible = true;
                    }
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
            dgvSurveyViewer.DataSource = null;
            //----------------------------------------------------------Also reset the download mode.
            myGlobalBase.EE_isLogMemPageTransferMode = false;
            myGlobalBase.EE_isLogMemFrameTransferMode = false;
            myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate = false;
            EE_LogMemSurvey_UpdateTheme();
            //------------------------
            tbUDTStart.Text = "";
            tbUDTStop.Text = "";
            tbLastAddress.Text = "";
            tbNoOfFrame.Text = "";
            tbMetaData1.Text = "";
            tbMetaData2.Text = "";
            tbMetaData4.Text = "";
            tbMetaData5.Text = "";
            tbMetaData6.Text = "";
            tbMetaData7.Text = "";
            tbReport.Text = "";
            tbError.Text = "";
            //===========================
            btnDownhole.Enabled = true;
            this.Refresh();
        }
        #endregion

        #region //================================================================cbHideG_MouseClick
        private void cbHideG_MouseClick(object sender, MouseEventArgs e)
        {
            if (dgvSurveyViewer.DataSource != null)
            {
                ClearAndUpdateDataGridView(sFrame);
                dgvUpdateSelected();
            }
        }
        #endregion

        #region //================================================================cbHideD_MouseClick
        private void cbHideD_MouseClick(object sender, MouseEventArgs e)
        {
            if (dgvSurveyViewer.DataSource != null)
            {
                ClearAndUpdateDataGridView(sFrame);
                dgvUpdateSelected();
            }
        }
        #endregion

        #region //================================================================cbHideFR_MouseClick
        private void cbHideFR_MouseClick(object sender, MouseEventArgs e)
        {
            if (dgvSurveyViewer.DataSource != null)
            {
                ClearAndUpdateDataGridView(sFrame);
                dgvUpdateSelected();
            }
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

        #region //================================================================Array Command_ASCII_Process() without prefix // Also process CANUSB & USBUART
        //================================================================================================
        public void SurveyArray_Command_ASCII_Process(int SelectDevice)
        {
            myGlobalBase.LoggerOpertaionMode = true;
            if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM)
            {
                myUSBVCOMComm.VCOMArray_Message_Send(m_sEntryTxt, SelectDevice);
            }
        }
        #endregion
        
        //#####################################################################################################
        //###################################################################################### Syn Driller and Logged data. 
        //#####################################################################################################

        #region //================================================================DrillerSyncTimeStampandSave
        private void DrillerSyncTimeStampandSave()
        {
            if (myGlobalBase.SelectTools == (int)GlobalBase.eSelectTools.OLDDIRECT)              // limited to specfic projecttheme, in this case BG drilling (old project)
            {
                if (Load_DrillerFile() == false)       // Is there DrillerFile?
                {
                    myMainProg.myRtbTermMessageLF("AutoSelection: Driller Survey File is missing or incorrect folder, check folder and try again!!\n");
                    return;
                }
            }
            dgvUpdateSelected();
            //-------------------------------------------------------------------------------Save to filename.
            SurveyCVS_OpenSaveDataCloseFile(myBGMasterSetup.sFilenameDefaultSurveyResults);
        }
        #endregion

        #region //================================================================dgvUpdateSelected
        private void dgvUpdateSelected()
        {
            RecievedData = myUDTmyFrameDataLog.HeaderFrame + ",Actual_UTC:" + ",Note: " + '\n';
            StringBuilder sb = new StringBuilder();

            double SurveyUTC0 = 0;
            double SurveyUTC1 = 0;
            DataGridViewRow row1 = null;
            DataGridViewRow row2 = null;
            if (blBGMasterDrillerFile != null)
            {
                //-------------------------------------------Selected Data Message into RecievedData
                try
                {
                    foreach (BGMasterDriller DrillerData in blBGMasterDrillerFile)
                    {
                        row1 = null;
                        for (int i = 0; i < dgvSurveyViewer.RowCount; i++)
                        {
                            row2 = dgvSurveyViewer.Rows[i];
                            if ((string)row2.Cells[0].Value == "$D")
                            {
                                if (row1 == null)       // 1st row start with 0. 
                                    SurveyUTC0 = 0;
                                else
                                    SurveyUTC0 = Tools.ConversionStringtoDouble((string)row1.Cells[6].FormattedValue);
                                SurveyUTC1 = Tools.ConversionStringtoDouble((string)row2.Cells[6].FormattedValue);
                                if ((DrillerData.dtUTC > SurveyUTC0) && (DrillerData.dtUTC < SurveyUTC1)) // && (SurveyUTC0 != 0) && (SurveyUTC1 != 0))
                                {
                                    sb.Clear();
                                    dgvSurveyViewer.Rows[i].Cells[0].Style.BackColor = Color.Yellow;
                                    dgvSurveyViewer.Rows[i].Cells[6].Style.BackColor = Color.Yellow;
                                    for (int c = 0; c < dgvSurveyViewer.ColumnCount; c++)
                                    {
                                        sb.Append(row2.Cells[c].Value.ToString() + ',');
                                        sb.Replace("\r", "");                               // remove certain char that causing spurious issue in excel. 
                                        sb.Replace("\n", "");
                                    }
                                    RecievedData += sb.ToString() + DrillerData.dtUTC.ToString() + "," + DrillerData.sNote + '\n'; // that will give you the string you desire
                                    break;
                                }
                                row1 = row2;
                            }
                        }
                    }
                }
                catch
                {
                    myMainProg.myRtbTermMessageLF("AutoSelection: Error in auto selection and save, unable to process!!\n");
                }
            }
            //------------------------------------------Separator
            RecievedData += "---  ---\n";
            RecievedData += "# Job Survey Details\n";
            RecievedData += "---  ---\n";
            //-----------------------------------------Load Job Survey files
            try
            {
                blBGMasterJobSurvey = null;
                string filename = myBGMasterSetup.sFile_JobProjectAll + "\\" + myBGMasterSetup.sFilenameDefaultJobSurvey + ".xml";
                blBGMasterJobSurvey = myGlobalBase.DeserializeFromFile<BindingList<BGMasterJobSurvey>>(filename);
            }
            catch
            {
                RecievedData += "###ERR: Error in Job Survey Filename\n";
                blBGMasterJobSurvey = null;
                myMainProg.myRtbTermMessageLF("$E: Unable to load JobSurvey, missing file or incorrect folder\n");
            }
            RecievedData += "Filename:," + myBGMasterSetup.sFile_JobProjectAll + "\n";
            if (blBGMasterJobSurvey != null)
            {
                //-------------------------------------------Job Details
                RecievedData += "JobRef:," + blBGMasterJobSurvey[0].sJobRef + "\n";
                RecievedData += "TimeStamp:," + blBGMasterJobSurvey[0].sTimeStamp + "\n";
                RecievedData += "Note:," + blBGMasterJobSurvey[0].sNote + "\n";
                RecievedData += "Location:," + blBGMasterJobSurvey[0].sLocation + "\n";
                RecievedData += "Driller:," + blBGMasterJobSurvey[0].sDriller + "\n";
                RecievedData += "BoreHole Name:," + blBGMasterJobSurvey[0].sBoreholeName + "\n";
                RecievedData += "BoreHole Depth:," + blBGMasterJobSurvey[0].sBoreHoleDepth + "\n";
                if (blBGMasterJobSurvey[0].bisMeter == true)
                {
                    RecievedData += "Station Interval:," + (blBGMasterJobSurvey[0].iStationInterval / 10).ToString() + "M" + "\n";
                    RecievedData += "Units:, Meter\n";
                }
                else
                {
                    RecievedData += "Station Interval:," + (blBGMasterJobSurvey[0].iStationInterval / 10).ToString() + "Ft" + "\n";
                    RecievedData += "Units:, Feet\n";
                }
                RecievedData += "Gyro then Survey:," + blBGMasterJobSurvey[0].sGyroSurveyLoop + "\n";
                RecievedData += "Station Wait Period:," + blBGMasterJobSurvey[0].iWaitPeroidSec.ToString("N0") + "\n";
            }
            //------------------------------------------Separator
            RecievedData += "---  ---\n";
            RecievedData += "# Device Tools Config / EESys_Flags\n";
            RecievedData += "---  ---\n";
            //-------------------------------------------Report Frame Message into RecievedData
            if (sFrame == null)
                return;
            //-------------------------------------------
            for (int i = 0; i < sFrame.Count; i++)
            {

                if ((sFrame[i].Contains("$R")))
                {
                    RecievedData += sFrame[i] + '\n';
                }
            }
            //------------------------------------------Separator
            RecievedData += "---  ---\n";
            RecievedData += "# Error List\n";
            RecievedData += "---  ---\n";
            //-------------------------------------------Report Frame Message into RecievedData
            for (int i = 0; i < sFrame.Count; i++)
            {

                if ((sFrame[i].Contains("$F") == true) & (sFrame[i].Contains("$R") == false))
                {
                    RecievedData += sFrame[i] + '\n';
                }
            }

        }
        #endregion

        #region //================================================================btnViewSelected_Click
        private void btnViewSelected_Click(object sender, EventArgs e)
        {
            if (dgvSurveyViewer.DataSource == null)
                return;
            //ClearAndUpdateDataGridView();
            //dgvUpdateSelected();
        }
        #endregion

        #region //================================================================btnSTCStop_Click
        private void btnSTCStop_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.SelectTools == (int)GlobalBase.eSelectTools.ADTESM)
            {
                Thread.Sleep(100);
                myMainProg.VCOM_ProcessCommand("+STCUPLOAD(0x9)");
                Thread.Sleep(100);
                SurveyLoggerCVS_Stop();
            }
            else
            {
                myMainProg.VCOM_ProcessCommand(" ");
                Thread.Sleep(100);
                myMainProg.VCOM_ProcessCommand("STC_STOP()");
                Thread.Sleep(100);
                SurveyLoggerCVS_Stop();
            }
        }
        #endregion

        #region //================================================================BtnPause_Click
        private void BtnPause_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.SelectTools == (int)GlobalBase.eSelectTools.ADTESM)
            {
                Thread.Sleep(100);
                myMainProg.VCOM_ProcessCommand("+STCUPLOAD(0x0)");
                Thread.Sleep(100);
            }
        }
        #endregion

        #region //================================================================btnContinue_Click
        private void btnContinue_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.SelectTools == (int)GlobalBase.eSelectTools.ADTESM)
            {
                Thread.Sleep(100);
                myMainProg.VCOM_ProcessCommand("+STCUPLOAD(0x1)");
                Thread.Sleep(100);
            }
        }
        #endregion

        #region //================================================================tscSelectMethod_SelectedIndexChanged
        private void tscSelectMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tscSelectMethod.SelectedIndex == 2)
            {
                tstbPara1.Text = "PARA1 N/A";
                tstbPara2.Text = "PARA1 N/A";
            }
            if (tscSelectMethod.SelectedIndex == 5)
            {
                tstbPara1.Text = "1";
                tstbPara2.Text = "PARA1 N/A";
            }
            this.Refresh();
        }
        #endregion
    }
    public class ToolStatusWord
    {
        Tools Tools = new Tools();
        public UInt32 zLoggingToolStatus { get; set; }
        public int maxarrayno { get;}

        List<eToolStatusStruct> Flagx = new List<eToolStatusStruct>();
        // ==================================================================constructor
        public ToolStatusWord()
        {
            zLoggingToolStatus = 0;
            Flagx.Add(new eToolStatusStruct("0 : Gyro_Offset_Reset_Event",   0, 1));
            Flagx.Add(new eToolStatusStruct("1 : Sensor_Survey_Eventr",      1, 1));
            Flagx.Add(new eToolStatusStruct("2 :#",                         2, 1));       //#=blank.
            Flagx.Add(new eToolStatusStruct("3 :#",                         3, 2));
            Flagx.Add(new eToolStatusStruct("5 :#",                         5, 1));
            Flagx.Add(new eToolStatusStruct("6 :#",                         6, 1));
            Flagx.Add(new eToolStatusStruct("7 :#",                         7, 1));
            Flagx.Add(new eToolStatusStruct("8 :#",                         8, 1));
            Flagx.Add(new eToolStatusStruct("9 : ERR_FXAS_Unable_To_ReadData",9, 1));
            Flagx.Add(new eToolStatusStruct("10: ERR_IRAWMoreThan15mA",      10, 1));
            Flagx.Add(new eToolStatusStruct("11: ERR_EEPROM_is_Full",        11, 1));
            Flagx.Add(new eToolStatusStruct("12: ERR_POR_Occurred",          12, 1));
            Flagx.Add(new eToolStatusStruct("13: ERR_UARTRX_Chatter",        13, 1));
            Flagx.Add(new eToolStatusStruct("14: ERR_WDTResetEvent",         14, 1));
            Flagx.Add(new eToolStatusStruct("15: ERR_3V3PSU",                15, 1));
            Flagx.Add(new eToolStatusStruct("16: BatteryConfig",             16, 3));      // 3 BITS
            Flagx.Add(new eToolStatusStruct("19: BatteryLowVoltage",       19, 1));
            Flagx.Add(new eToolStatusStruct("20: BatteryStatus",             20, 2));      // 2 BITS
            Flagx.Add(new eToolStatusStruct("22:#",                         22, 1));
            maxarrayno = Flagx.Count;
        }
        [Flags]
        public enum eToolStatus
        {
            Gyro_Offset_Reset_Event             = 0,          //0
            Sensor_Survey_Event                 = 1,     
            Spare2                              = 2,
            Spare34                             = 3,
            AD7794_2nd_Sensor                   = 5,
            Spare6                              = 6,
            Spare7                              = 7,
            Spare8                              = 8,
            ERR_FXAS_Unable_To_ReadData         = 9,
            ERR_IRAWMoreThan15mA                = 10,
            ERR_EEPROM_is_Full                  = 11,
            ERR_POR_Occurred                    = 12,
            ERR_UARTRX_Chatter                  = 13,
            ERR_WDTResetEvent                   = 14,
            ERR_3V3PSU                          = 15,
            BatteryConfig                       = 16,   // 3 BITS
            isBatteryLowVoltage                 = 19,
            BatteryStatus                       = 20,   // 2 BITS
            Spare22                             = 22
        }

        public string ContextMenuInsertString(int arrayno)
        {
            if (arrayno >= Flagx.Count)               // out of range
                return "";
            if (Flagx[arrayno].bitNo == 5)
            {
                UInt32 i = (zLoggingToolStatus >> 5) & 0x1;
                if (i == 0)
                    return "5 : 2nd Sensor = MS9002";
                else
                    return "5 : 2nd_Sensor = KXBR";
            }
            if (Flagx[arrayno].bitNo == 16)        // Battery Config
            {
                // 0 = Not specified, 1 = 2 x SAFT LS12500 (3600mAHr), 2 = 4 x AAA Alkaline (1125mAHr)
                UInt32 i = (zLoggingToolStatus >> 16) & 0x7;
                string[] arr1 = new string[] { "16: BAT:????", "16: BAT:2xSAFT LS12500", "16: BAT:4xAAA Alkaline", "16:BAT ERR", "16:BAT ERR", "16:BAT ERR", "16:BAT ERR", "16:BAT ERR" };
                return arr1[i];
            }
            if (Flagx[arrayno].bitNo == 20)        // Battery Status, 2 bits
            {
                //0 = Normal, 1 = Half capacity(50 %), 2 = < 4.4V(Near End), 3 = < 4.0V(End)
                UInt32 i = (zLoggingToolStatus >> 20) & 0x3;
                string[] arr1 = new string[] { "20: BAT:Normal", "20: BAT:Half Capacity", "20: BAT:Near End", "20: BAT:End (<4V)" };
                return arr1[i];
            }
            if (Flagx[arrayno].sDesc.Contains("#"))   // Blank description
                return "";
            if (Tools.Bits_UInt32_intRead(zLoggingToolStatus, Flagx[arrayno].bitNo) == 0)  // if state is zero then no need to report text.
                return "";

            return Flagx[arrayno].sDesc;
        }

    }
    public class eToolStatusStruct
    {
        public string sDesc { get; set; }
        public int bitNo { get; set; }
        public int width { get; set; }
        public eToolStatusStruct(string sDesc, int bitNo, int width)
        {
            this.sDesc = sDesc;
            this.bitNo = bitNo;
            this.width = width;
        }
    }

    /*
        //================================================================
        // _zLoggingToolStatus Information about Tools and Error Flag reporting which is part of Logging. Some flag are copied from EESYSFLAG (EEPROM_INT)
        //================================================================
        typedef struct tagToolStatusMode
        {
            union  {
                uINT32 zSysWord;                                       // 32 BITS
            struct  {
                uINT32 Gyro_Offset_Reset_Event:1;                 //(0)	0 = No Event 1 = Event Occurred. See $R for Gryo Offset change reports.
                uINT32 Sensor_Survey_Event:1;                   //(1)	0 = No Event, 1 = Event Occurred, which as seen in Survey Reports.
                uINT32 :1;										//(2)
                uINT32 :2;										// (3-4)
                uINT32 AD7794_2nd_Sensor:1;                       //(5)	0 = MS9002, 1 = KXBR.
                uINT32 :1;  										//(6)
                uINT32 :1;  										//(7)
                uINT32 :1;                  						//(8)	.
                uINT32 ERR_FXAS_Unable_To_ReadData:1;             //(9)	0= Normal, 1 = Error.
                uINT32 ERR_IRAWMoreThan15mA:1;                                      //(10)
                uINT32 ERR_EEPROM_is_Full:1;                    //(11)	0 =Normal, 1 = Error, EEPROM is packed.
                uINT32 ERR_POR_Occurred:1;                          //(12)  0 =Normal, 1 = Error, POR occurred, possible battery chatter
                uINT32 ERR_UARTRX_Chatter:1;                        //(13)  0 =Normal, 1 = UART-RX Chatter O.
                uINT32 ERR_WDTResetEvent:1;                         //(14)	0 =Normal, 1 = WDT Reset Detected.
                uINT32 ERR_3V3PSU:1;                                        //(15)
                uINT32 BatteryConfig:3;                             //(16-18)	0 = Not specified, 1 = 2 x SAFT LS12500 (3600mAHr), 2 = 4 x AAA Alkaline (1125mAHr)
                uINT32 isBatteryLowVoltage:1;                       //(19)		0 = Bat is good, 1 = <4V threshold put device to DEADBATTERY Mode.
                uINT32 BatteryStatus:2;                             //(20-21)	(6-7) 0 = Normal, 1 = Half capacity (50%), 2 = <4.4V (Near End), 3 = <4.0V (End)
                uINT32 :9;  										//(22-31)
                };
    };
    _zLoggingToolStatus ;
extern volatile _zLoggingToolStatus zLoggingToolStatus;
    */


    #region//=============================================================DoubleBuffered property via reflection for datagridview, improve flickerless image.
    public static class ExtensionMethodsDownloadSurveyCVS
    {
        public static void DoubleBufferedSurveyCVS(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    #endregion
}


//#####################################################################################################
//###################################################################################### SandBox
//#####################################################################################################

//#region//================================================================SurveyCVS_RecievedData (Old Method)
// Important when '$E\n' i detected it goes to this routine for processing.
// When #@#E is detected then it finish !
// The ASCII can potentially goes up to 300 ASCII per message frame
// Frame is define as start with $X where X is any char and $E is end frame.
// Separator is ';' or ','.  
// 1st frame is header for column definition
// 2nd frame is format type which is used to decode hex column to readable data column.
// When completed, it save into two CVS filename, RAW (hex) and later Converted (int). 
// The conversion and DGV table take place after the saving RAW data. 
//public delegate void SurveyCVS_RecievedDataDelegate(string DataFrame);
////Solution: http://www.dreamincode.net/forums/topic/188209-cross-thread-calls-made-easy/ 
//public void SurveyCVS_RecievedData(string DataFrame)
//{
//    // Check if we need to call BeginInvoke.
//    if (this.InvokeRequired)
//    {
//        // Pass the same function to BeginInvoke,
//        // but the call would come on the correct
//        // thread and InvokeRequired will be false.
//        this.BeginInvoke(new SurveyCVS_RecievedDataDelegate(SurveyCVS_RecievedData), new object[] { DataFrame });
//        return;
//    }
//    DataFrame.Replace("/0", string.Empty);      // Remove null elements.
//    if (myUDTmyFrameDataLog == null)
//    {
//        myUDTmyFrameDataLog = new UDTmyFrameDataLog();
//    }
//    RecievedData += DataFrame;
//    myUDTmyFrameDataLog.UDTDataLogProcessFrame(DataFrame);
//    tbMetaData4.Text = myUDTmyFrameDataLog.CountByte.ToString();
//    tbMetaData1.Text = myUDTmyFrameDataLog.iCounterD.ToString();
//    tbMetaData2.Text = myUDTmyFrameDataLog.iCounterG.ToString();
//    tbError.Text = myUDTmyFrameDataLog.iCounterF.ToString();
//    tbReport.Text = myUDTmyFrameDataLog.iCounterR.ToString();

//    //------------------------------------------------End of bulk frame
//    if (RecievedData.Contains("#@#E") == true)
//    {
//        this.SuspendLayout();
//        myGlobalBase.EE_isSurveyCVSRawLogDataActive = false;
//        myGlobalBase.EE_isLogMemFrameTransferMode = false;
//        myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate = false;
//        myMainProg.myRtbTermMessageLF("-I: End of Survey Data Transfer");
//        bool status = SurveyCVS_OpenSaveDataCloseFile(myBGMasterSetup.sFilenameDefaultDownloadedRaw);
//        //--------------------------------------------------
//        if (sFrame == null)
//            sFrame = new List<string>();
//        else
//            sFrame.Clear();
//        //--------------------------------------------------Split end of frame and save into sFrame
//        sFrame.AddRange(RecievedData.Split('\n'));
//        EndOfBulkFrame(false);                                  // Treat as Raw data so get converted. 
//                                                                //--------------------------------------------------Calibration files, ESM has none. 
//        if ((myGlobalBase.SelectTools == (int)GlobalBase.eSelectTools.BGTOCO) || (myGlobalBase.SelectTools == (int)GlobalBase.eSelectTools.OLDDIRECT))        // TOCO/OLD DIR 
//        {
//            //--------------------------------------------------Import CVS Data from LCP1549 Memory.
//            myMainProg.myRtbTermMessageLF("-INFO: Now Importing calibration file (CVS)");
//            string sFilename = System.IO.Path.Combine(myBGMasterSetup.sFile_JobProjectAll, "ImportedCalibrationFile" + DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + ".csv");
//            myMainProg.myRtbTermMessageLF("-INFO: Saving Downloaded Filename: " + sFilename);
//            myEEPROMExpImport.EEPROM_ImportDataNow(true, sFilename, 0);
//        }
//        this.ResumeLayout();
//    }
//}
//#endregion