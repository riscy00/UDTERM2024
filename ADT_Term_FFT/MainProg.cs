using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;       // for Struct tools
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Reflection;
using System.Text.RegularExpressions;
using JR.Utils.GUI.Forms;
using UDT_Term;
using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using UDT_Term_FFT;
using System.Net.NetworkInformation;
using Amazon.SimpleNotificationService.Util;
using System.Xml.Linq;
using System.Windows.Markup;
using Amazon;

// Power Tool: insert QuickLaunch with "@tasks CollapseRegions" to close all region only. 

// TASK Plan
// (3) SPI and I2C Protocol support
// (4) DMTL Batch Command in sequencer, jumping to specific function (Have I done this in ZMDI, check again)
// (5) Speed up terminal box, change to textbox rather than richtext box or consider 3rd party alternative. 
//      (a) Consider buffering the text input in real time and when no further message for say 100mSec update display.
//      (d) Reduce number of repaint in text box.
//      (c) Review for 3rd party alternative text box (faster). 
// (6) Fix end of text cursor, it look messy (old method).  
// (7) Document difference between pauses.
// (8) Document different pop up message and have best solution. 
// (9) BLE support if NET provide it (neater solution than crappy one in the past).

// Version 77A, 13/8/19: Added CANBUS support for PCAN Device. Based on demo sample package. 
// Version 76Q, 08/8/19: Continue debug fix and support for ESM Project. Change in default filename.

// Version 76OA, continue works. 

// Version 76O 03/04/19  ESM project Frame and Page transfer, also encoding and filename save. 100% works for both USB and UART of the ESM project.
// (1) Frame debug finish.
// (2) Page debug finish.
// (3) Had to use encoding ISO-8859-1 to solve many problem, this is default so take care. when open the LogMem, it check current encoding and warn you of possible issue.
// (4) Save filename in RAW, Gyro, Vibration, Shock and Other ($R/$F), working good.
// (5) Support hybrid frame including upload, page to frame, frame to filename. Does not display data in Log Mem, but this can be worked on in the evening. 

// Version 76N  25/03/19 
// (1) Fixed minor bug for TOCO project

// Version 76L Jan 2019:
// (1) Moderate upgrade to support SNAD network and have 3rd ASYNC Panel with SNAD features.
// (2) Upgraded on filename for SNAD filename generation based on SNAD number. 
// (3) transfer message to main window and make use of pop up boxes.
// (4) new SNADNetwork.cs class goes to global variable so it can be shared between Main and LoggerCVS easily.

// Version 75/76 Jan-May 2018: 
// (1) Work on TOCO Project with some various minor enhancement within code.
// (2) Directional code no longer work, use earlier version UDT for directional project (2016/17)
// (3) Improved 16 channel VCOM array, adopted in ToCo project (channel 6). 
// (4) Supported MiniAD7794 project. 

// Version 71: Skipped from 57 to 70 then to 71
// (1) Major work for MiniAD7794 support, similar to EVKIT window application development.
// (2) Focus on AD7794 to start with.
// (3) Unify BAUD rate in setting box, so it get automatically listed from Global variable.
// (4) BUG: ScopeChart do not update every time data frame come in, this need fixing. 

// Version 57: 11-Jun-17
// (1) Deleted/Scraped EVKIT code and library
// (2) HMR2300 tested fine. Need to use '\r'. Update 
// (3) Include TVB template background
// (4) Update Option Window, delete old EVKIT option. Deleted I2C option class. 
// (5) New code for copy/cut/paste
// (6) Added new ResCalc code as test bed, to be promoted to Android App.
// (7) Tidy up codes
// (8) Include option to automatically copy received message to clips. 
// (9) Added context menu for TabMaster section for save and load.
// (10) Check to ensure it save right filename after changes. 
// (11) Move option class to GlobalBase. 
// --------------------------------------------
// Version 54: 5/Feb/17
// (1) Remove CAN firmware and deleted anything that is CAN, this need new CAN redevelopment if required (see 53 for existing solution). 
// (2) Moderised Globalbase with (get;set)
// (3) Debugged and Test EEPROM Import and Export code for BG Survey, all tested fine. Task completed. Incude Erase button for testing. 
//     Work in conjunction with F:\009-EFT-MyGIT\114_DIRECTIONAL_BG_5B\LPC1549_DIR_BG_5B (final release for EEPROM function). 
// (4) VCOM/FTDI: added code to delete '\r' to address issue with LPC devices (as perhaps for DSP device).
//                added code to check for '\n' and if missing insert '\n' before sending. Updated comment. 
// (5) DMFP: + and - now working correctly in UDT section (not ZMDI section). Can be used for various application. 
// (6) Included global to mask TX message into terminal display via myGlobalBase.bNoRFTermMessage (when true) this included in DMFP time out which reset to false. 
// (7) Release as 54. To SH and desktop.  
// Version 53: 1/Feb/17
// (1) Major work on EEPROM Import and Export for BG calibration data. 
// (2) Retained for Reference as 53 and move on to 54. Not for release. 



namespace UDT_Term_FFT
{
    public partial class MainProg : Form
    {
        DialogSupport mbOperationBusy;
        DialogSupport mbOperationError;
        ITools Tools = new Tools();
        USB_FTDI_Comm myUSBComm;                    // FTDI device manager section (scan, open, close, read, write).                                         // USB_USBCAN_Comm myUSBCANComm;
        USB_VCOM_Comm myUSB_VCOM_Comm;
        FTDI_DataView myFDTI_DataView;              // FTDI properties information
        LoggerCSV myLoggerCSV;
        ImportDataCVS myImportDataCVS;
        FFTWindow myFFWindow;
        RiscyScope myRiscyScope;
        EEPROMExportImport myEEPROMExpImport;       // Added Rev 53 (14 Jan 2017)
                                                    //-----------------------------------------MiniAD7794 Project
        MiniAD7794Setup myMiniSetup;
        ADIS16460 myADIS16460;
        //-----------------------------------------Survey
        Survey mySurvey;
        Timer StartSurveyTimer1;     // Start Survey Precheck Timer.
        private bool isSurveyRTCOkay = false;
        private bool isSurveyEEEmpty = false;
        private int DMFPCounter;
        //-----------------------------------------HMR2300
        HRM2300 myHRM2300;
        Timer HRMtimer;             // HMR2300 Update Readout
        Timer HRMtimerLoop;         // HMR2300 Trigger loop sample
        Regex HMRrg;
        //-----------------------------------------FluxGate
        FluxGateII myFGII;
        Timer FGIItimer;             // HMR2300 Update Readout
        Timer FGIItimerLoop;         // HMR2300 Trigger loop sample
        Regex FGIIrg;
        //-----------------------------------------ADIS16140
        //-----------------------------------------ErrReport
        ERR_Report Err_Report;
        //--------------------------------------------
        AppConfigWindow myAppConfigWindow;         // Added RGP 11/10/15, this used to configure this application. XML is used.
                                                   //AppDGVExample myAppDGVExample;           // XML Experiment Code, include datagridview and pivot/flip code. 
        System.Windows.Forms.RichTextBox myrtbTerm;
        GlobalBase myGlobalBase;
        DMFProtocol myDMFProtocol;
        ClientCode myClientCode;
        USBComPortManager myUSBComPortManager;
        USB_Message_Manager myUSB_Message_Manager;
        EE_LogMemSurvey myEEDownloadSurvey;
        EEpromTools myEEpromTools;
        //---------------------------------------------BG Drilling Section

        BG_ClientData myBGClientData;
        BG_Driller myBGDriller;
        BGMasterSetup myBGMasterSetup;
        BGMasterJobSurvey myMasterJobSurvey;
        BGMasterDriller myMasterDriller;
        //--------------------------------
        BG_ReportViewer myBGReportViewer;       //2018  for report and orientation only
        //BG_ToCo_Setup myBGToCoSetup;          //2018  for setup window only. NB: Filename copied to myBGReportViewer at Orientation.
        BG_Orientation myBGOrientation;
        BG_ToolSerial myBGToolSerial;

        BG_Battery myBGBattery;
        BG_Service myBGService;
        BGMasterCalData myBGMasterCalData;
        BGToCoProjectFileManager myBGToCoProjectFileManager;
        //----------------------------------------------------------CRC8
        CRC8CheckSum myCalc_CRC8;
        //----------------------------------------------------------ESM
        ESM_NVM_SensorConfig myESM_NVM_SensorConfig;
        ESMConfigNVM myESMConfigNVM;
        ESMBlockSeqNVM myESMBlockSeqNVM;
        //----------------------------------------------------------CANBus 77A
        CanConfig myCanConfig;
        CanPCAN myCanPCAN;
        //----------------------------------------------------------RiscyTimerII
        RiscyTimerII myRiscyTimerII;

        //########################################################################################## Main Program
        private System.Windows.Forms.DataGridView[] dataGridArray;
        DataGridViewTextBoxColumn[] CmdColumn;
        //-------------------------------------Command List Buffer for recall
        private string[] sCommandList;
        private const int iCommandListMax = 20;
        private int iCommandListPointer;
        //-------------------------------------Define number of tab in the UDT
        const int NoOfTabs = 8;
        //-------------------------------------
        private string m_sEntryTxt;
        private string m_sResponseTxt;
        private bool m_bIsCommandEntered;
        private bool m_isNewLine;                   // TRUE, insert ':' at the start of the new line. It assumed \r is executed prior to setting this bool to true.
                                                    //--------------------------------------Filename
        private string sFoldernameDefault;
        private string sFoldername;
        private string sProgramFolderDirectory;

#pragma warning disable 0169
        private string sFilenameConsole;
#pragma warning restore 0169
        private string sCommandTableFileName;
        IntPtr rtbOEMx;
        IntPtr rtbOEMy;
        //#####################################################################################################
        //###################################################################################### Setter/Getter
        //#####################################################################################################
        public string EntryTxt { get { return m_sEntryTxt; } set { m_sEntryTxt = value; } }
        public bool bIsCommandEntered { get { return m_bIsCommandEntered; } set { m_bIsCommandEntered = value; } }

        //#####################################################################################################
        //###################################################################################### SessionLog
        //#####################################################################################################

        List<string> sSessionDataLog;
        //--------------------------------------Session Filename
        private string sSessionFoldername;          // = "F:\\ADTSessionLog"; //"NULL";
        private string sSessionAppendFileName;
        private string sStealthAppendFileName;
        private string sDataAppendFileName;
        //---------------------------------------
        private bool isSessionDataAppendEnabled;    // Requires ~LA(...) to enable Data session mode. To disable requires ~LF(....) as end of data session mode. 

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public MainProg()
        {
            rtbOEMx = new IntPtr();
            rtbOEMy = new IntPtr();
            //---------------------------------------
            myCalc_CRC8 = new CRC8CheckSum();
            sFoldernameDefault = Tools.GetPathUserFolder();  // Static folder name default, do not modify by code, use sFoldername
            //Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            sFoldername = sFoldernameDefault;               // use default filename
            sSessionAppendFileName = "";
            sStealthAppendFileName = "";
            sDataAppendFileName = "";

            //---------------------------------------Fetch Local Directory where app filename is located.  
            //sProgramFolderDirectory = System.IO.Directory.GetCurrentDirectory();
            sProgramFolderDirectory = AppDomain.CurrentDomain.BaseDirectory;                //http://stackoverflow.com/questions/6041332/best-way-to-get-application-folder-path

            #region//---------------------------------------SDO Module

            sCommandTableFileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            #endregion

            InitializeComponent();
            this.BringToFront();
            this.TopMost = false;
            this.WindowState = FormWindowState.Normal;

            #region //---------------------------------------TabPage Context Menu
            //=============================================Context Menu for TabPage1-8
            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Rename Tab", new EventHandler(TabTools_RenameTabPageText));
            cm.MenuItems.Add("Save", new EventHandler(TabTools_SaveTabMaster));
            cm.MenuItems.Add("Load", new EventHandler(TabTools_LoadTabMaster));
            TabTool1.ContextMenu = cm;
            TabTool2.ContextMenu = cm;
            TabTool3.ContextMenu = cm;
            TabTool4.ContextMenu = cm;
            TabTool5.ContextMenu = cm;
            TabTool6.ContextMenu = cm;
            TabTool7.ContextMenu = cm;
            TabTool8.ContextMenu = cm;
            #endregion

            //-------------------------------------rtbTerm
            myrtbTerm = rtbTerm;
            //-------------------------------------ErrReport
            Err_Report = new ERR_Report();
            //--------------------------------------Global Base
            myGlobalBase = new GlobalBase();
            //---------------------------------------Application Setting and setup themes
            myAppConfigWindow = new AppConfigWindow();
            myAppConfigWindow.MyGlobalBase(myGlobalBase);                           //Just in case.
            myAppConfigWindow.MyMainProg(this);
            myAppConfigWindow.LoadConfigNow();
            CompanyName_Theme_Update();
            //---------------------------------------ADT Session Folder for ESM, Survey, Session Log, etc
            myGlobalBase.sFilename = sFoldernameDefault;
            //---------------------------------------FFTWindow
            myFFWindow = new FFTWindow();
            myFFWindow.MyGlobalBase(myGlobalBase);

            //---------------------------------------RiscyScope
            myRiscyScope = new RiscyScope();
            myRiscyScope.MyGlobalBase(myGlobalBase);

            //---------------------------------------HMR2300
            HMRrg = new Regex(@"^[0-9\s,+-]*$");

            //---------------------------------------Fluxgate
            FGIIrg = new Regex(@"^[0-9E$\s,+-]*$");

            //#####################################################################################################
            //###################################################################################### BG Section Code
            //#####################################################################################################
            //---------------------------------------BG Setup Tool.
            Survey_Setup_Init();

            myBGMasterSetup = new BGMasterSetup();
            myMasterJobSurvey = new BGMasterJobSurvey();
            myMasterDriller = new BGMasterDriller();

            //---------------------------------------Download Survey CVS. (work for any project BG, IDT, ADT or EFT)
            myEEDownloadSurvey = new EE_LogMemSurvey();
            myEEDownloadSurvey.MyGlobalBase(myGlobalBase);
            myEEDownloadSurvey.MyMainProg(this);
            myEEDownloadSurvey.MyRiscyScope(myRiscyScope);
            myEEDownloadSurvey.MyBGMasterSetup(myBGMasterSetup);
            myEEDownloadSurvey.MyBGMasterJobSurvey(myMasterJobSurvey);
            //-------------------------------------BG Driller 
            myBGDriller = new BG_Driller();
            myBGDriller.MyDownloadSurvey(myEEDownloadSurvey);
            myBGDriller.MyGlobalBase(myGlobalBase);
            myBGDriller.MyRtbTerm(myrtbTerm);
            myBGDriller.MyBGMasterSetup(myBGMasterSetup);
            //------------------------------------- Report Viewer
            myBGReportViewer = new BG_ReportViewer();                   //BGToCoReportClass ReportClassRef
            myBGReportViewer.MyBGMasterSetup(myBGMasterSetup);
            myBGReportViewer.MyGlobalBase(myGlobalBase);
            myBGReportViewer.MyMainProg(this);
            //------------------------------------- Project File Manager (one object only!), linked to reportviewer.
            myBGToCoProjectFileManager = new BGToCoProjectFileManager(myBGReportViewer.ReportClass);
            //--------------------------------------BG Tool Setup.
            myBGClientData = new BG_ClientData(myBGReportViewer.ReportClass);
            myBGClientData.MyMainProg(this);
            myBGClientData.MyGlobalBase(myGlobalBase);
            myBGClientData.MyBGMasterSetup(myBGMasterSetup);
            myBGClientData.MyBGToCoProjectFileManager(myBGToCoProjectFileManager);

            myESMConfigNVM = new ESMConfigNVM();
            myESMConfigNVM.MyMainProg(this);
            myESMConfigNVM.MyGlobalBase(myGlobalBase);

            myESMBlockSeqNVM = new ESMBlockSeqNVM();
            myESMBlockSeqNVM.MyMainProg(this);
            myESMBlockSeqNVM.MyGlobalBase(myGlobalBase);
            //-------------------------------------BG Orientation Window
            myBGOrientation = new BG_Orientation();
            myBGOrientation.MyGlobalBase(myGlobalBase);
            myBGOrientation.MyBGReportViewerSetup(myBGReportViewer);
            myBGOrientation.MyMainProg(this);
            //-------------------------------------BG ToolSerial Window
            myBGToolSerial = new BG_ToolSerial();
            myBGToolSerial.MyGlobalBase(myGlobalBase);
            myBGToolSerial.MyBGReportViewerSetup(myBGReportViewer);
            myBGToolSerial.MyMainProg(this);
            //-------------------------------------BG Battery Window
            myBGBattery = new BG_Battery();
            myBGBattery.MyGlobalBase(myGlobalBase);
            myBGBattery.MyMainProg(this);
            myBGBattery.MyBGReportViewerSetup(myBGReportViewer);
            //-------------------------------------BG Service Window
            myBGService = new BG_Service();
            myBGService.MyGlobalBase(myGlobalBase);
            myBGService.MyMainProg(this);
            myBGService.MyBGReportViewerSetup(myBGReportViewer);
            myBGService.MyEEDownloadSurvey(myEEDownloadSurvey);
            myBGService.MyBGToCoProjectFileManager(myBGToCoProjectFileManager);
            //myBGService.MyBG_ToCo_Setup(myBGToCoSetup);
            myBGService.MyBGToolSerial(myBGToolSerial);
            //-------------------------------------BG Select Mode 78JE. 
            cbSelectTS.Checked = false;
            cbSelectTOCO.Checked = true;
            myBGMasterSetup.BGToCoToolSelectMode = 0;
            BGSurveyButtonSetting(0xFFFF);
            //-------------------------------------Calibration Data (class only frim BGMasterSetup)
            myBGMasterCalData = new BGMasterCalData();
            //-------------------------------------File Manager


            //#####################################################################################################
            //###################################################################################### Export/Import Internal EEPROM Protocol (LPC1549) (Added in Rev 53)
            //#####################################################################################################
            myEEPROMExpImport = new EEPROMExportImport();
            myEEPROMExpImport.EEPROMExportImport_Init();
            myEEPROMExpImport.MyGlobalBase(myGlobalBase);
            myEEPROMExpImport.MyMainProg(this);
            myEEDownloadSurvey.MyEEPROMExpImport(myEEPROMExpImport);
            myBGService.MyEEPROMExportImport(myEEPROMExpImport);

            //#####################################################################################################
            //###################################################################################### Support & Arrival
            //#####################################################################################################

            //---------------------------------------ImportDataCVS, 1D release
            myImportDataCVS = new ImportDataCVS();
            myImportDataCVS.MyGlobalBase(myGlobalBase);
            myImportDataCVS.MyUSBComm(myUSBComm);

            //---------------------------------------DMFP (Duplex message protocol) // Do not separate!!
            myDMFProtocol = new DMFProtocol();
            myDMFProtocol.MyGlobalBase(myGlobalBase);
            myDMFProtocol.MyMainProg(this);
            myDMFProtocol.MyBGOrientation(myBGOrientation);
            myDMFProtocol.MyBGToolSerial(myBGToolSerial);
            myDMFProtocol.MyBGBattery(myBGBattery);
            //---------------------------------------
            myEEDownloadSurvey.MyDMFProtocol(myDMFProtocol);
            myEEPROMExpImport.MyDMFProtocol(myDMFProtocol);
            myBGClientData.MyDMFProtocol(myDMFProtocol);
            myESMConfigNVM.MyDMFProtocol(myDMFProtocol);
            myESMBlockSeqNVM.MyDMFProtocol(myDMFProtocol);
            myBGReportViewer.MyDMFProtocol(myDMFProtocol);
            myBGOrientation.MyDMFProtocol(myDMFProtocol);
            myBGToolSerial.MyDMFProtocol(myDMFProtocol);
            myBGBattery.MyDMFProtocol(myDMFProtocol);
            myBGService.MyDMFProtocol(myDMFProtocol);
            //--------------------------------------FTDI USB Comm (Top Level USB Interface Code). 
            myUSBComm = new USB_FTDI_Comm();
            myUSBComm.MyGlobalBase(myGlobalBase);
            myUSBComm.MyMainProg(this);
            myEEDownloadSurvey.MyUSBComm(myUSBComm);
            myGlobalBase.MyUSBComm(myUSBComm);                          // Pass reference pointer into global so other can use it.

            //---------------------------------------DMPF
            myDMFProtocol.MyUSBComm(myUSBComm);
            //myDMFProtocol.MyBGToolSetup(myBGToolSetup);                 // Required for Async Battery Call. 
            myFDTI_DataView = new FTDI_DataView();
            //---------------------------------------VCOM Interface
            myUSB_VCOM_Comm = new USB_VCOM_Comm();
            myUSB_VCOM_Comm.MyGlobalBase(myGlobalBase);
            myUSB_VCOM_Comm.MyMainProg(this);
            myDMFProtocol.MyUSBVCOMComm(myUSB_VCOM_Comm);
            myEEPROMExpImport.MyUSBVCOMComm(myUSB_VCOM_Comm);
            myEEDownloadSurvey.MyUSBVCOMComm(myUSB_VCOM_Comm);
            myBGClientData.MyUSBVCOMComm(myUSB_VCOM_Comm);
            myESMConfigNVM.MyUSBVCOMComm(myUSB_VCOM_Comm);
            myESMBlockSeqNVM.MyUSBVCOMComm(myUSB_VCOM_Comm);
            myBGReportViewer.MyUSBVCOMComm(myUSB_VCOM_Comm);
            myBGOrientation.MyUSBVCOMComm(myUSB_VCOM_Comm);
            myBGToolSerial.MyUSBVCOMComm(myUSB_VCOM_Comm);
            myBGBattery.MyUSBVCOMComm(myUSB_VCOM_Comm);
            myBGService.MyUSBVCOMComm(myUSB_VCOM_Comm);
            //-------------------------------------USB_Message_Manager
            myUSB_Message_Manager = new USB_Message_Manager();
            myUSB_Message_Manager.MyGlobalBase(myGlobalBase);
            myUSB_Message_Manager.MyDMFProtocol(myDMFProtocol);
            myUSB_Message_Manager.MyImportDataCVS(myImportDataCVS);
            myUSB_Message_Manager.MyMainProg(this);
            myUSB_Message_Manager.MySurveyCVS(myEEDownloadSurvey);
            myUSB_Message_Manager.MyFFWindowBase(myFFWindow);           // For FFT Window Chart (New addition 24/03/13)
            myUSB_Message_Manager.MyEEPROMExportImport(myEEPROMExpImport);
            myUSB_VCOM_Comm.MyUSB_Message_Manager(myUSB_Message_Manager);
            myUSBComm.MyUSB_Message_Manager(myUSB_Message_Manager);

            //--------------------------------------USBComPortManager (Both FTDI and VCOM)
            myUSBComPortManager = new USBComPortManager();
            myUSBComPortManager.MyGlobalBase(myGlobalBase);
            myUSBComPortManager.MyRtbTerm(myrtbTerm);
            myUSBComPortManager.MyUSBFTDIComm(myUSBComm);
            myUSBComPortManager.MyUSBVCOMComm(myUSB_VCOM_Comm);
            //---------------------------------------LoggerCSV
            myLoggerCSV = new LoggerCSV();
            myLoggerCSV.MyGlobalBase(myGlobalBase);
            myLoggerCSV.MyUSBComm(myUSBComm);
            myLoggerCSV.MyUSBVCOMComm(myUSB_VCOM_Comm);
            myLoggerCSV.MyEEDownloadSurvey(myEEDownloadSurvey);
            myLoggerCSV.MyUSB_Message_Manager(myUSB_Message_Manager);
            myLoggerCSV.MyDMFProtocol(myDMFProtocol);
            myLoggerCSV.MyMainProg(this);                       // Added 76L

            myUSB_Message_Manager.MyLoggerCVS(myLoggerCSV);
            //--------------------------------------Client Code
            myClientCode = new ClientCode();
            myClientCode.MyGlobalBase(myGlobalBase);
            myClientCode.MyMainProg(this);
            myClientCode.MyUSBVCOMComm(myUSB_VCOM_Comm);
            myLoggerCSV.MyClientCode(myClientCode);
            //--------------------------------------EEPROM Tools
            myEEpromTools = new EEpromTools();
            myEEpromTools.MyGlobalBase(myGlobalBase);
            myEEpromTools.MyRtbTerm(myrtbTerm);
            myEEpromTools.MyMainProg(this);
            myEEpromTools.MyDMFProtocol(myDMFProtocol);
            myDMFProtocol.MyEEpromTools(myEEpromTools);
            myBGService.MyEEpromTools(myEEpromTools);

            //#####################################################################################################
            //###################################################################################### HMR2300/ADIS Sensor Survey
            //#####################################################################################################

            //------------------------------------- HRM2300 Device
            myHRM2300 = new HRM2300();
            myHRM2300.MyGlobalBase(myGlobalBase);
            HMR2300_Init();

            //------------------------------------- FGII Device
            this.myFGII = new FluxGateII();
            myFGII.MyGlobalBase(myGlobalBase);
            FGIIA_Init();

            //------------------------------------- Suvery
            mySurvey = new Survey();
            mySurvey.MyGlobalBase(myGlobalBase);
            mySurvey.MyMainProg(this);
            mySurvey.MyRiscyScope(myRiscyScope);
            mySurvey.MyHMR2300(myHRM2300);
            mySurvey.MyFGII(myFGII);
            mySurvey.MyUSBVCOMComm(myUSB_VCOM_Comm);
            myUSB_Message_Manager.MySurveyXX(mySurvey);

            //#####################################################################################################
            //###################################################################################### MiniAD7794
            //#####################################################################################################

            MiniAD7794_Init();

            //#####################################################################################################
            //###################################################################################### PCAD
            //#####################################################################################################
            //-------------------------------------CAN Serial Monitor
            //---------------------------------------Check if dll is pre-installed
            if (File.Exists(@"C:\Windows\System32\PCANBasic.dll"))
            {
                myGlobalBase.isPCANBasicInstalled = true;
            }
            else
            {
                myGlobalBase.isPCANBasicInstalled = false;
                btnCanRelease.Enabled = false;
                btnCanConnect.Enabled = false;
                btnCanWindow.Enabled = false;
                cbbBaudrates.Enabled = false;
                cbCanExtended.Enabled = false;
                myRtbTermMessageLF("\n**********************************************************************************");
                myRtbTermMessageLF("PCANBasic.dll is not installed in default location: C:\\Windows\\System32\\");
                myRtbTermMessageLF("This can be obtained from (https://www.peak-system.com/Firmware.548.0.html?&L=1");
                myRtbTermMessageLF("and download PCAN Basic API or Package. Otherwise do not use CAN features in UDT");
                myRtbTermMessageLF("**********************************************************************************");
            }
            myCanConfig = new CanConfig();    // For Connection.
            myCanConfig.MyGlobalBase(myGlobalBase);
            myCanConfig.MyMainProg(this);

            myCanPCAN = new CanPCAN(myGlobalBase);
            myCanPCAN.MyGlobalBase(myGlobalBase);
            myCanPCAN.MyMainProg(this);
            PCAN_Update_TabPanel();

            //#####################################################################################################
            //###################################################################################### TimeStep
            //#####################################################################################################
            TimeStamp_Init();

            //#####################################################################################################
            //###################################################################################### Riscy Timer II
            //#####################################################################################################
            myRiscyTimerII = new RiscyTimerII();
            myRiscyTimerII.MyGlobalBase(myGlobalBase);
            myRiscyTimerII.MyMainProg(this);

            //#####################################################################################################
            //###################################################################################### Context Menu Setup
            //#####################################################################################################
            //--------------------------------------Cursor key
            sCommandList = new string[iCommandListMax];
            iCommandListPointer = 0;

            #region//------------------------------------------------ Context Menu (New Method UDT-57 Onward)
            //https://stackoverflow.com/questions/18966407/enable-copy-cut-past-window-in-a-rich-text-box
            if (rtbTerm.ContextMenuStrip == null)
            {
                ContextMenuStrip cms = new ContextMenuStrip { ShowImageMargin = true };
                ToolStripMenuItem tsmiCut = new ToolStripMenuItem("Cut");
                tsmiCut.Click += (sender, e) => rtbTerm.Cut();
                cms.Items.Add(tsmiCut);
                ToolStripMenuItem tsmiCopy = new ToolStripMenuItem("Copy");
                tsmiCopy.Click += (sender, e) => rtbTerm.Copy();
                cms.Items.Add(tsmiCopy);
                ToolStripMenuItem tsmiPaste = new ToolStripMenuItem("Paste");
                tsmiPaste.Click += (sender, e) => rtbTerm.Paste();
                cms.Items.Add(tsmiPaste);
                rtbTerm.ContextMenuStrip = cms;
            }
            #endregion

            //#####################################################################################################
            //###################################################################################### UART and VCOM
            //#####################################################################################################

            #region//------------------------------------------------ Insert List in cbCOMList
            //---------------------------------------Add COM Port list to cbCOMList and set to default checkbox. 
            cbCOMList.Items.Add("SCAN");
            for (int i = 1; i <= 64; i++)       // Technically Window 6.0+ support up to 256 COM ports. 
            {
                cbCOMList.Items.Add("COM" + i.ToString());
            }
            cbCOMList.SelectedIndex = 0;
            cbCOMList.Refresh();
            #endregion

            #region//------------------------------------------------ Configure VCOM and USB AutoDetect.
            if (myGlobalBase.configb[0].bOptionSkipFTDIScan == true)
            {
                myGlobalBase.VCOMMode = 1;
                cbVCOM.Checked = true;
                cbFT232RL.Checked = false;
            }
            else
            {
                myGlobalBase.VCOMMode = 0;
                cbVCOM.Checked = true;
                cbFT232RL.Checked = false;
            }
            //cbUSB_AutoDetect.Checked = myGlobalBase.configb[0].bOptionAutoDetectEnable;
            #endregion

            #region//------------------------------------------------ DMFP USB Array Selection List.
            if (this.cbUSBOption_SelectArray.Items.Count == 0)
            {
                for (int i = 0; i < myGlobalBase.sUSBDeviceName.Count; i++)
                {
                    this.cbUSBOption_SelectArray.Items.Add(myGlobalBase.sUSBDeviceName[i]);
                }
            }
            #endregion

            #region//------------------------------------------------ USB Option (Default)
            myGlobalBase.USBArray_RedirectDebugTerm = -1;
            myGlobalBase.UDTerm_DMFP_ShowSyncCallBack = false;
            //cbUSB_AutoDetect.Checked = false;       // This also update  myGlobalBase.isUSBOption_AutoDetect_Enable 
            #endregion

            //#####################################################################################################
            //###################################################################################### MISC
            //#####################################################################################################
            //--------------------------------------Double Buffering/smoother display. 
            TabMaster.DoubleBuffered3(true);
            rtbTerm.DoubleBuffered4(true);

            //##############################################################################################################
            //=============================================================================================MainFeatureSelectedIndex
            //##############################################################################################################

            #region//==================================================Short cuts to select TAB for specific project works

            //--------------------------------------Feature Tab Control: Updated 04/11/19
            tcMainFeature.MakeTransparent();
            tpLoggerCVS.BackColor = Color.Transparent;
            tpFFTWindow.BackColor = Color.Transparent;
            tpHexDisplay.BackColor = Color.Transparent;
            tpImportCVS.BackColor = Color.Transparent;
            tpRTClock.BackColor = Color.Transparent;
            tpMiniAD7794.BackColor = Color.Transparent;
            tpHMR2300.BackColor = Color.Transparent;
            tpResCalcX.BackColor = Color.Transparent;
            tpADIS16460.BackColor = Color.Transparent;
            tpBGSurvey.BackColor = Color.Transparent;
            tpBGSurvey2.BackColor = Color.Transparent;
            tgSurvey.BackColor = Color.Transparent;
            tpFluxGateII.BackColor = Color.Transparent;
            tpRTClock.BackColor = Color.Transparent;
            tpUSBOption.BackColor = Color.Transparent;
            tbSessionLog.BackColor = Color.Transparent;
            tpSNAD.BackColor = Color.Transparent;
            tpTimeStamp.BackColor = Color.Transparent;
            tpCanBus1.BackColor = Color.Transparent;
            tpESMConfigII.BackColor = Color.Transparent;
            tpSpare.BackColor = Color.Transparent;

            switch (myGlobalBase.CompanyName)
            {
                case (int)GlobalBase.eCompanyName.BGDS:
                    {
                        //myGlobalBase.MainFeatureSelectedIndex = 3;      //BGSurvey      ADIS=5
                        myGlobalBase.USBArray_RedirectDebugTerm = (int)eUSBDeviceType.BGDRILLING;
                        myGlobalBase.UDTerm_DMFP_ShowSyncCallBack = true;
                        break;
                    }
                case (int)GlobalBase.eCompanyName.ADT:
                    {
                        //myGlobalBase.MainFeatureSelectedIndex = 0;

                        break;
                    }
                case (int)GlobalBase.eCompanyName.TVB:
                    {
                        //myGlobalBase.MainFeatureSelectedIndex = 6;      //MiniAD7794
                        myGlobalBase.USBArray_RedirectDebugTerm = (int)eUSBDeviceType.MiniAD7794;
                        myGlobalBase.UDTerm_DMFP_ShowSyncCallBack = true;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            //------------------------------------USB Option Section
            tcMainFeature.SelectTab(myGlobalBase.MainFeatureSelectedIndex);
            USBOption_UpdateTabPageSection();
            #endregion

            #region //############################################################################################TABMASTER

            this.dataGridArray = new System.Windows.Forms.DataGridView[NoOfTabs];

            byte[] test = new byte[NoOfTabs];


            for (int i = 0; i < NoOfTabs; i++)
            {
                this.dataGridArray[i] = new System.Windows.Forms.DataGridView();
            }

            this.TabMaster.SuspendLayout();
            this.TabTool1.SuspendLayout();
            for (int i = 0; i < NoOfTabs; i++)
            {
                ((System.ComponentModel.ISupportInitialize)(this.dataGridArray[i])).BeginInit();
            }


            this.TabTool1.Controls.Add(this.dataGridArray[0]);
            this.TabTool2.Controls.Add(this.dataGridArray[1]);
            this.TabTool3.Controls.Add(this.dataGridArray[2]);
            this.TabTool4.Controls.Add(this.dataGridArray[3]);
            this.TabTool5.Controls.Add(this.dataGridArray[4]);
            this.TabTool6.Controls.Add(this.dataGridArray[5]);
            this.TabTool7.Controls.Add(this.dataGridArray[6]);
            this.TabTool8.Controls.Add(this.dataGridArray[7]);

            #region //================================================Baseline DataGridView
            // 
            // dataGridView1
            // 
            for (int i = 0; i < NoOfTabs; i++)
            {
                // Added RGP 06/10/15------------------------------------------------ This fix row height. 
                this.dataGridArray[i].AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                this.dataGridArray[i].RowTemplate.Height = 21;
                this.dataGridArray[i].RowTemplate.MinimumHeight = 21;
                //this.dataGridArray[i].ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                //--------------------------------------------------------------------
                this.dataGridArray[i].Location = new System.Drawing.Point(0, 0);
                this.dataGridArray[i].Size = new System.Drawing.Size(378, 487);
                this.dataGridArray[i].TabIndex = 0;
                this.dataGridArray[i].AllowUserToAddRows = false;
                this.dataGridArray[i].AllowUserToDeleteRows = false;
                this.dataGridArray[i].AllowUserToResizeColumns = false;
                this.dataGridArray[i].AllowUserToResizeRows = false;
                this.dataGridArray[i].BackgroundColor = System.Drawing.Color.Gainsboro;
                this.dataGridArray[i].MultiSelect = false;
                this.dataGridArray[i].Name = "CmdBox" + i.ToString();
                this.dataGridArray[i].RowHeadersVisible = false;
                this.dataGridArray[i].ScrollBars = System.Windows.Forms.ScrollBars.None;
                this.dataGridArray[i].CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.CmdBox_CellContentClick);

            }
            #endregion

            //==============================================Command Box (DataGridView Section)

            CmdColumn = new DataGridViewTextBoxColumn[3];

            #region //=========================================DataGridView1 (Request) Column Definition


            for (int j = 0; j < NoOfTabs; j++)
            {
                DataGridViewButtonColumn btnRequest = new DataGridViewButtonColumn();
                btnRequest.Name = "btnCommand";
                btnRequest.HeaderText = "Process";
                btnRequest.Width = 50;
                btnRequest.Resizable = DataGridViewTriState.False;
                btnRequest.SortMode = DataGridViewColumnSortMode.NotSortable;
                btnRequest.ReadOnly = true;
                this.dataGridArray[j].Columns.Add(btnRequest);
                for (int i = 0; i < 2; i++)
                {
                    CmdColumn[i] = new DataGridViewTextBoxColumn();
                    CmdColumn[i].Name = "txt" + myCommndColumnFormat[i].Label;
                    CmdColumn[i].HeaderText = myCommndColumnFormat[i].Label;
                    CmdColumn[i].Width = myCommndColumnFormat[i].Width;
                    CmdColumn[i].Resizable = DataGridViewTriState.False;
                    CmdColumn[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dataGridArray[j].Columns.Add(CmdColumn[i]);
                }
            }
            #endregion

            #region //=========================================Add Row
            for (int j = 0; j < NoOfTabs; j++)
            {
                for (int i = 0; i < 20; i++)
                {
                    this.dataGridArray[j].Rows.Add(new string[] { (i + (j * 20)).ToString("D2") });
                }
            }
            #endregion

            this.TabMaster.ResumeLayout(false);
            this.TabTool1.ResumeLayout(false);

            for (int i = 0; i < NoOfTabs; i++)
            {
                ((System.ComponentModel.ISupportInitialize)(this.dataGridArray[i])).EndInit();
            }
            this.TabMaster.SelectedTab = TabTool1;
            //############################################################################################
            #endregion

            //-------------------------------------BG Survey Button Default, this override previous setting, assumed CO configuration. 
            cbSelectTS.Checked = false;                                 // Set to TOCO to begin with. 
            cbSelectTOCO.Checked = true;
            myBGMasterSetup.BGToCoToolSelectMode = 0;
            BGSurveyButtonSetting(0);
            //-------------------------------------Reset Checkbox and hide since it not connected. 
            SNAD_UpdateLayout(0);
            //-------------------------------------ESM 
            ESMConfigInit();
            //-------------------------------------###TASK: Temp Code, remove when finish with CAN works
            //tcMainFeature.SelectedTab = tpESMConfigII; //tpCanBus1; 
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        //##############################################################################################################
        //=============================================================================================Folder Name
        //##############################################################################################################

        #region //====================================================myFolder_UpdateSync
        public void myFolder_UpdateSync(string Foldername)  // This come from config window. 
        {
            sSessionAppendFileName = "";
            sStealthAppendFileName = "";
            sDataAppendFileName = "";
            sFoldernameDefault = Foldername;
            sFoldername = Foldername;
            sSessionFoldername = Foldername;
            sFoldername = Foldername;
            //--------------------------------------------------------Master for all projects.
            myGlobalBase.zCommon_DefaultFolder = Foldername;
            //--------------------------------------------------------Update projects folder/filename definition. 
            myGlobalBase.sSessionFoldername = Foldername;
            myGlobalBase.sMiniAD7794_Foldername = Foldername;
            myGlobalBase.sMiniAD7794_Filename = "";
            myGlobalBase.ESM_Config_FolderName = Foldername;
            myGlobalBase.ESM_Config_FilenameName = "";
            myGlobalBase.CanPCAN_Config_FolderName = Foldername;
            myGlobalBase.CanPCAN_Config_FilenameName = "";
            myGlobalBase.CanPCAN_TXCycleList_FolderName = Foldername;
            myGlobalBase.CanPCAN_TXCycleList_FilenameName = "";
            myGlobalBase.CanPCAN_LogMsg_FolderName = Foldername;
            myGlobalBase.CanPCAN_LogMsg_FilenameName = "";
            myGlobalBase.TimeStamp_FolderName = Foldername;
            myGlobalBase.TimeStamp_FilenameName = "";
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================TabPage Sync
        //##############################################################################################################

        #region //====================================================myTabPage_UpdateSync
        public void myTabPage_UpdateSync(int TabPage)
        {
            myGlobalBase.MainFeatureSelectedIndex = TabPage;
            tcMainFeature.SelectTab(myGlobalBase.MainFeatureSelectedIndex);
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Baud Rate
        //##############################################################################################################

        #region //====================================================myBaudRate_UpdateSync
        public void myBaudRate_UpdateSync(UInt32 BaudRate)  // Must alway use myGlobalBase.configb[0].User_Baudrate, or user entry
        {
            if (this.cbBaudRateSelect.Items.Count == 0)
            {
                for (int i = 0; i < myGlobalBase.iBaudRateList.Length; i++)
                {
                    this.cbBaudRateSelect.Items.Add(myGlobalBase.iBaudRateList[i]);
                }
            }
            if (BaudRate == 0)                  // Select Index mode.
            {
                myGlobalBase.configb[0].User_Baudrate = 0;
                cbBaudRateSelect.Show();
                txUserBaudRateSelect.Hide();
                myGlobalBase.isUser_Baudrate = false;
                cbBaudRateSelect.SelectedIndex = (int)myGlobalBase.configb[0].Select_Baudrate;
                myGlobalBase.Select_Baudrate = (uint)cbBaudRateSelect.SelectedIndex;       // Update BAUD Rate
            }
            else                                // Select User Entry mode.
            {
                myGlobalBase.configb[0].User_Baudrate = BaudRate;
                cbBaudRateSelect.Hide();
                txUserBaudRateSelect.Show();
                myGlobalBase.isUser_Baudrate = true;
                txUserBaudRateSelect.Text = myGlobalBase.configb[0].User_Baudrate.ToString();
                myGlobalBase.Select_Baudrate = myGlobalBase.configb[0].User_Baudrate;
            }
        }
        #endregion

        #region //====================================================myBaudRate_UpdateSetting
        private void myBaudRate_UpdateSetting()  // Must alway use myGlobalBase.configb[0].User_Baudrate, or user entry
        {
            if (myGlobalBase.isUser_Baudrate == true)
            {
                cbBaudRateSelect.Hide();
                txUserBaudRateSelect.Show();
                txUserBaudRateSelect.Text = myGlobalBase.Select_Baudrate.ToString();
            }
            else
            {
                cbBaudRateSelect.Show();
                txUserBaudRateSelect.Hide();
                cbBaudRateSelect.SelectedIndex = (int)myGlobalBase.Select_Baudrate;
            }
            cbBaudRateSelect.Refresh();
            txUserBaudRateSelect.Refresh();
        }
        #endregion

        //#########################################################################################################
        //=============================================================================================Clip Board Thread
        //#########################################################################################################

        #region //=============================================================================================zClipBoard_AddString
        //==========================================================
        // Purpose  : THis create thread for clipboard copy of message.   
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        public delegate void zClipBoard_AddString_StartDelegate(string Message);
        public void zClipBoard_AddString(string Message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new zClipBoard_AddString_StartDelegate(zClipBoard_AddString), new object[] { Message });
                return;
            }
            try
            {
                Clipboard.SetText(Message);
            }
            catch { };
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Win Form
        //##############################################################################################################

        #region //=============================================================================================MainProg_FormClosed
        private void MainProg_FormClosed(object sender, FormClosedEventArgs e)
        {
            myUSBComm.FTDI_Message_ClosePort();         // Close USB/UART port, to avoid risk of hanging.
            Application.Exit();                         // Exit app.
        }

        private void MainProg_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (myUSB_Message_Manager.wUSB != null)
                {
                    myUSB_Message_Manager.wUSB.EventArrived -= myUSB_Message_Manager.USBRemoved;
                    myUSB_Message_Manager.wUSB.EventArrived -= myUSB_Message_Manager.USBAdded;
                    myUSB_Message_Manager.wUSB.Stop();
                    myUSB_Message_Manager.wUSB.Dispose();
                }
            }
            catch { }
            myGlobalBase.IsApplicationQuit = true;
            Thread.Sleep(50);
        }
        #endregion

        #region //=============================================================================================MainProg_Load
        private void MainProg_Load(object sender, EventArgs e)
        {
            myEEPROMExpImport.EEPROMExportImport_Init();

        }
        #endregion

        #region //=============================================================================================hexDisplayModeToolStripMenuItem_Click
        private void tsmStayTopWindow_Click(object sender, EventArgs e)
        {
            if (tsmStayTopWindow.CheckState == CheckState.Checked)
            {
                tsmStayTopWindow.CheckState = CheckState.Unchecked;
                this.BringToFront();
                this.TopMost = false;
                this.WindowState = FormWindowState.Normal;
                this.Invalidate();

            }
            else
            {
                tsmStayTopWindow.CheckState = CheckState.Checked;
                this.BringToFront();
                this.TopMost = true;
                this.WindowState = FormWindowState.Normal;
                this.Invalidate();
            }
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================DFMP Async Command ('+' and '-')
        //##############################################################################################################

        #region //==================================================viewDMToolStripMenuItem_Click
        private void viewDMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myDMFProtocol.Visible == false)
            {
                viewDMToolStripMenuItem.Checked = false;
                myDMFProtocol.Show();
            }
            else
            {
                viewDMToolStripMenuItem.Checked = false;
                myDMFProtocol.Hide();
            }
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================This section deal with command entry (FT232RL and CAN and USB) within Window Terminal
        //##############################################################################################################

        #region//==================================================MsgBox_KeyPress Control ( This is for command entry by user)
        private void rtbTerm_KeyDown(object sender, KeyEventArgs e)  // claret = blinking on line, cursor = mouse cursor.
        {
            rtbTerm.SelectionFont = myGlobalBase.FontMessage;
            rtbTerm.SelectionColor = myGlobalBase.ColorMessage;
            //-----------------------------------------Escape
            if (e.KeyData == Keys.Escape)
            {
                Trace.Write("-INFO: <ESC>");
                if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_FTDI)
                {
                    //### Check if dsPIC33 is actually connected, if not reject command....this need review/optional.
                    myUSBComm.FTDI_Message_Send("\x1b");
                }
                if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM)
                {
                    myUSB_VCOM_Comm.VCOM_Message_Send("\x1b");
                }
                Terminal_Append_Request("\r\n:");
                return;
            }
            //-----------------------------------------Up or Down cursor key
            if (e.KeyData == Keys.Up || e.KeyData == Keys.Down)     // Ignore up or down key when pressed
            {
                e.SuppressKeyPress = true;
                if ((e.KeyData == Keys.Up) & (iCommandListPointer < iCommandListMax))
                {
                    List<string> myList = rtbTerm.Lines.ToList();
                    if (myList.Count > 0)
                    {
                        Tools.rtb_StopRepaint(rtbTerm, rtbOEMx);
                        iCommandListPointer++;
                        myList.RemoveAt(myList.Count - 1);
                        rtbTerm.Lines = myList.ToArray();
                        rtbTerm.AppendText("\r\n");
                        if (sCommandList[iCommandListPointer] == null)    // if null, go back to previous.
                            iCommandListPointer = 0;
                        if (sCommandList[iCommandListPointer] != null)        // NB: Does not records DMFP or non-user command.
                        {
                            rtbTerm.AppendText(sCommandList[iCommandListPointer]);
                        }
                        //rtbTerm.Refresh();
                        Tools.rtb_StartRepaint(rtbTerm, rtbOEMx);
                    }
                }
                if ((e.KeyData == Keys.Down))
                {
                    List<string> myList = rtbTerm.Lines.ToList();
                    if (myList.Count > 0)
                    {
                        Tools.rtb_StopRepaint(rtbTerm, rtbOEMx);
                        iCommandListPointer--;
                        if (iCommandListPointer <= 0)
                            iCommandListPointer = 0;
                        myList.RemoveAt(myList.Count - 1);
                        rtbTerm.Lines = myList.ToArray();
                        rtbTerm.AppendText("\r\n");
                        if (sCommandList[iCommandListPointer] != null)
                        {
                            rtbTerm.AppendText(sCommandList[iCommandListPointer]);
                        }
                        //rtbTerm.Refresh();
                        Tools.rtb_StartRepaint(rtbTerm, rtbOEMx);
                    }
                    return;
                }
            }
            //-----------------------------------------Enter
            if (e.KeyData == Keys.Enter)
            {
                // Break down message into discrete text.
                e.SuppressKeyPress = true;
                //------------------------------------------Add CMD text into buffer
                int i = 19;
                while (i > 0)
                {
                    sCommandList[i] = sCommandList[i - 1];
                    i--;
                }
                //----------------------------------------
                string[] sEntryTxtCapture = rtbTerm.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
                int sNumberOfLines = sEntryTxtCapture.Length;
                m_sEntryTxt = sEntryTxtCapture[sNumberOfLines - 1];     //TODO###: handle many split commands.
                sCommandList[0] = m_sEntryTxt;                          // Save command into recall buffer. 
                iCommandListPointer = 0;                                // Reset recall pointer back to start
                rtbTerm.AppendText("\r\n");
                bIsCommandEntered = true;
                rtbTerm.ReadOnly = true;                                // Disable MsgBox to avoid further command entry until process is completed.

                //if (myLoggerCSV.Visible== false)
                //    myGlobalBase.LoggerWindowVisable = false;

                m_sResponseTxt = string.Empty;
                MsgBoxCommandEntry();
                m_sEntryTxt = string.Empty;

                bIsCommandEntered = false;
                rtbTerm.ReadOnly = false;
            }
            //-----------------------------------------Left Cursor key
            if (e.KeyData == Keys.Left)
            {
                if ((rtbTerm.SelectionStart - rtbTerm.GetFirstCharIndexOfCurrentLine()) == 0)
                {
                    e.SuppressKeyPress = true;                      // This suppress any claret moving up 
                }
                return;
            }
            if (m_isNewLine == true)
            {
                Terminal_Append_Request(":");
                m_isNewLine = false;
            }
            //----------------------------------------Copy Only by Key
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                if (rtbTerm.SelectedText != "")
                    Clipboard.SetText(rtbTerm.SelectedText);
            }
            base.OnKeyDown(e);

        }
        #endregion

        #region //==================================================MsgBoxCommandEntry() // Check for prefix and make decision.
        //==========================================================High Level Command Processor goes here
        private void MsgBoxCommandEntry()
        {
            try
            {
                m_sEntryTxt.ToUpper();
                m_sEntryTxt = m_sEntryTxt.Replace(":", "");     // Remove ':', not needed.
                if (m_sEntryTxt.Length == 0)                    // No text entry, quit process.
                {
                    m_isNewLine = true;
                    return;
                }
            }
            catch (System.Exception ex)
            {
                Trace.Write("#ERR: Window terminal, abnormal text entry: " + ex.ToString());
                m_isNewLine = true;
                return;
            }
            // With UDTTERM command with ! (standard) or +! (callback), it treated as Ethernet command entry. 
            if (m_sEntryTxt.Length<=2)
            {
                myRtbTermMessageLF("#E:Command entry length is too short, must be more than 3 chars.");
                return;
            }
            //      !Test(para) or !+Test(Para)             +!Test(Para)                                          
            if (((m_sEntryTxt.Substring(0, 1) == "!") || (m_sEntryTxt.Substring(0, 2) == "!")) && (myGlobalBase.ETH_isUDTClientOpen == true))
            {
                Command_ASCII_Process_Ethernet();
            }
            else
            {
                m_sEntryTxt = m_sEntryTxt.Replace("!", "");     // remove ! so that it goes to UART based channel if enabled.
                Command_ASCII_Process_UART();
            }
        }
        #endregion

        #region //==================================================Command_ASCII_Process_Ethernet()
        private void Command_ASCII_Process_Ethernet()
        {
            m_sEntryTxt = m_sEntryTxt.Replace("!", "");

            string prefix = string.Empty;
            prefix = m_sEntryTxt.Substring(0, 1);

            ETH_UDT_SendMessage(m_sEntryTxt);
        }
        #endregion

        #region //==================================================Command_ASCII_Process_UART()
        private void Command_ASCII_Process_UART()
        {
            string prefix = string.Empty;
            prefix = m_sEntryTxt.Substring(0, 1);

            if (myGlobalBase.USBArray_RedirectDebugTerm == -1)        // Non Array Type Reception.
            {
                if (myGlobalBase.is_Serial_Server_Connected == false)
                {
                    myRtbTermMessageLF("#E:USB port has no connected device (Generic). Connect device and try again.");
                    return;
                }
                btn_ConnectSerial.Enabled = false;
                btnClosePort.Enabled = true;
                #region--------------------FTDI Section
                if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_FTDI)
                {
                    switch (prefix)
                    {
                        case ("+"):                         // Duplex Message Frame Protocol (New Nov-2015)
                            {
                                //---------------------------------------------------------------------- SNAD Address added to command message Mods. 
                                if (myGlobalBase.mySNADNetwork != null)
                                {
                                    m_sEntryTxt = myGlobalBase.mySNADNetwork.SNAD_Modify_UDT_Command(m_sEntryTxt);
                                }
                                //-----------------------------------------------------------------------
                                myUSBComm.FTDI_Message_Send(m_sEntryTxt);
                                break;
                            }
                        case ("$"):                         // Logger CSV command, if 'H', 'D' or 'N' then open up the LoggerCSV 
                            {
                                if (m_sEntryTxt.Contains("$N") | m_sEntryTxt.Contains("$H") | m_sEntryTxt.Contains("$D"))
                                {
                                    myLoggerCSV.Show();
                                    Thread.Sleep(1);
                                    myLoggerCSV.LoggerCVS_SetupShow_Window(0, "");
                                    myLoggerCSV.Command_Processor(m_sEntryTxt.Substring(0, 2));
                                }
                                break;
                            }
                        default:                            // Terminal based command (generic)
                            {
                                //---------------------------------------------------------------------- SNAD Address added to command message Mods. 
                                if (myGlobalBase.mySNADNetwork != null)
                                {
                                    m_sEntryTxt = myGlobalBase.mySNADNetwork.SNAD_Modify_UDT_Command(m_sEntryTxt);
                                }
                                //-----------------------------------------------------------------------
                                myUSBComm.FTDI_Message_Send(m_sEntryTxt);
                                break;
                            }
                    }

                }
                #endregion

                #region--------------------VCOM Section

                if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM)
                {
                    switch (prefix)
                    {
                        case ("+"):                         // Duplex Message Frame Protocol (New Nov-2015)
                            {
                                //---------------------------------------------------------------------- SNAD Address added to command message Mods. 
                                if (myGlobalBase.mySNADNetwork != null)
                                {
                                    m_sEntryTxt = myGlobalBase.mySNADNetwork.SNAD_Modify_UDT_Command(m_sEntryTxt);
                                }
                                //-----------------------------------------------------------------------
                                myUSB_VCOM_Comm.VCOM_Message_Send(m_sEntryTxt);
                                break;
                            }
                        case ("$"):                         // Logger CSV command, if 'H', 'D' or 'N' then open up the LoggerCSV 
                            {
                                if (m_sEntryTxt.Contains("$N") | m_sEntryTxt.Contains("$H") | m_sEntryTxt.Contains("$D") | m_sEntryTxt.Contains("$T"))
                                {
                                    myLoggerCSV.Show();
                                    Thread.Sleep(1);
                                    myLoggerCSV.LoggerCVS_SetupShow_Window(0, "");
                                    myLoggerCSV.Command_Processor(m_sEntryTxt.Substring(0, 2));
                                }
                                break;
                            }
                        default:                            // Terminal based command (generic)
                            {
                                //---------------------------------------------------------------------- SNAD Address added to command message Mods. 
                                if (myGlobalBase.mySNADNetwork != null)
                                {
                                    m_sEntryTxt = myGlobalBase.mySNADNetwork.SNAD_Modify_UDT_Command(m_sEntryTxt);
                                }
                                //-----------------------------------------------------------------------
                                {
                                    myUSB_VCOM_Comm.VCOM_Message_Send(m_sEntryTxt);
                                    break;
                                }
                            }
                    }
                }
                #endregion
            }
            else                                                       // Array Type Reception.
            {
                if (myGlobalBase.myUSBVCOMArray[myGlobalBase.USBArray_RedirectDebugTerm].isDMFProtocolEnabled == true)
                {
                    if ((m_sEntryTxt[0] == '+') & (myGlobalBase.UDTerm_DMFP_ShowSyncCallBack == true))
                    {
                        if (myGlobalBase.myUSBVCOMArray[myGlobalBase.USBArray_RedirectDebugTerm].isSerial_Server_Connected == true)
                        {
                            //-----------------------------------Setup call-back delegate. 
                            DMFP_Delegate fpCallBack_GenericCallBack = new DMFP_Delegate(DMFP_Generic_CallBack);
                            //-----------------------------------Place Message to rtbTerm Window.
                            if (myGlobalBase.bNoRFTermMessage == false)
                                myRtbTermMessageLF("#DMFP:" + m_sEntryTxt);
                            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
                            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
                            Thread t1 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_GenericCallBack, m_sEntryTxt, 250, myGlobalBase.USBArray_RedirectDebugTerm));
                            t1.Start();
                            myDMFProtocol.DMFProtocol_UpdateList();
                            return;
                        }
                    }
                }
                //------------Array type ordinary command from UDT Debug
                if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM)
                {
                    #region--------------------VCOM Section
                    switch (prefix)
                    {
                        case ("+"):                         // Duplex Message Frame Protocol (New Nov-2015)
                            {
                                myUSB_VCOM_Comm.VCOMArray_Message_Send(m_sEntryTxt, myGlobalBase.USBArray_RedirectDebugTerm);
                                break;
                            }
                        case ("$"):                         // Logger CSV command, if 'H', 'D' or 'N' then open up the LoggerCSV 
                            {
                                /*
								if (m_sEntryTxt.Contains("$N") | m_sEntryTxt.Contains("$H") | m_sEntryTxt.Contains("$D"))
								{
									myLoggerCSV.Show();
									Thread.Sleep(1);
									myLoggerCSV.LoggerCVS_SetupShow_Window(0,"");
									myLoggerCSV.Command_Processor(m_sEntryTxt.Substring(0, 2));
								}
								*/
                                break;
                            }
                        default:                            // Terminal based command (generic)
                            {
                                myUSB_VCOM_Comm.VCOMArray_Message_Send(m_sEntryTxt, myGlobalBase.USBArray_RedirectDebugTerm);
                                break;
                            }
                    }
                    #endregion
                }
            }
        }
        #endregion

        #region //==================================================DMFP_MiniAD7794_MINITWIK_CallBack  
        public void DMFP_Generic_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            // In case of special response to jump to specific routine, goes here for UDT Debug Command base.

        }
        #endregion

        #region //==================================================Command_ASCII_Process_UART(string sCMD)
        public void Command_ASCII_Process_UARTs(string sCMD)
        {
            //---------------------------------------------------------------------- SNAD Address added to command message Mods. 
            if (myGlobalBase.mySNADNetwork != null)
            {
                sCMD = myGlobalBase.mySNADNetwork.SNAD_Modify_UDT_Command(sCMD);
                myRtbTermMessage("-INFO: SNAD Message Out:" + sCMD);
            }
            //----------------------------------------------------------------------
            m_sEntryTxt = sCMD;
            if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_FTDI)
            {
                //### Check if dsPIC33 is actually connected, if not reject command....this need review/optional.
                myUSBComm.FTDI_Message_Send(m_sEntryTxt);
            }
            if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM)
            {
                myUSB_VCOM_Comm.VCOM_Message_Send(m_sEntryTxt);
            }
        }
        #endregion

        #region //==================================================Command_ASCII_Process_CAN()
        private void Command_ASCII_Process_CAN()
        {
            /*
			if (myUSBCANComm.isProtocol_0x0F_Active == false)
			{
				Trace.WriteLine("#ERR: CAN Terminal Protocol Mode not activated, rejected command entry");
				return;
			}
			// ###TODO: This routine decode the message and send 
			// myUSBCANComm.CANBUS_Send_Message()
			Trace.WriteLine("#ERR: Command Not Supported....yet");
			*/
        }

        #endregion

        //##############################################################################################################
        //=============================================================================================FTDI Configuration Section
        //##############################################################################################################

        #region//==================================================uSBUARTFT232RToolStripMenuItem_Click
        private void uSBUARTFT232RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uSBUARTFT232RToolStripMenuItem.Checked = true;
            btn_ConnectSerial.Enabled = true;
            //### TODO CLOSE DOWN USB-UART SERIAL INTERFACE MODULE. 
            //myUSBCANComm.USBCAN_Close();
            myGlobalBase.USB_SelectDeviceMode = (int)GlobalBase.eSerialDeviceSelect.USB_UART_FTDI;
        }
        #endregion

        #region//==================================================uSBUARTFT232RToolStripMenuItem_Click
        private void uSBRS232FT232R56KToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uSBUARTFT232RToolStripMenuItem.Checked = false;
            btn_ConnectSerial.Enabled = true;
            //### TODO CLOSE DOWN USB-UART SERIAL INTERFACE MODULE. 
            //myUSBCANComm.USBCAN_Close();
            myGlobalBase.USB_SelectDeviceMode = (int)GlobalBase.eSerialDeviceSelect.USB_UART_FTDI;
            myGlobalBase.Select_Baudrate = 1;
        }
        #endregion

        #region//==================================================btnClosePort_Click
        private void btnClosePort_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.is_Serial_Server_Connected == false)
            {
                rtbTerm.AppendText("###ERR: What COM/FTDI Port?, there nothing to close!\n");
            }
            else
            {
                rtbTerm.AppendText("-I: Closing USB/UART Port, please wait few seconds until finally closed\n");
                btnClosePort.Enabled = false;
                myUSBComm.FTDI_Message_ClosePort();
                myUSB_VCOM_Comm.VCOM_Message_ClosePort();
                myGlobalBase.is_Serial_Server_Connected = false;
            }
            btnClosePort.Enabled = true;
            cbVCOM.Enabled = true;
            cbFT232RL.Enabled = true;
            cbCOMList.Enabled = true;
            cbBaudRateSelect.Enabled = true;
            btn_ConnectSerial.Enabled = true;
            btnScanAndConnectTool.Text = "Scan and Connect Tools";
            btnScanAndConnectTool.Enabled = true;
        }
        #endregion

        #region//==================================================btn_ConnectSerial_Click()
        private void btn_ConnectSerial_Click(object sender, EventArgs e)
        {
            Serial_ConnectSerial_Scan();
            this.Invalidate();
            this.BringToFront();
            //this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            //this.WindowState = FormWindowState.Maximized;
            rtbTerm.Focus();
            this.Focus();
        }
        #endregion

        #region//================================================== btn_ConnectSerial_Scan()
        private void Serial_ConnectSerial_Scan()
        {
            //------------------------------------------Scan Mode
            if (myGlobalBase.configb[0].bOptionSkipFTDIScan == true)
                myGlobalBase.VCOMMode = 1;
            //---------------------------------------------------
            myGlobalBase.VCOM_IsChange = false;
            myBaudRate_UpdateSetting();
            //-------------------------Disable various control.
            cbBaudRateSelect.Enabled = false;
            btn_ConnectSerial.Enabled = false;      // Avoid repeat clicking.
            cbVCOM.Enabled = false;
            cbFT232RL.Enabled = false;
            cbCOMList.Enabled = false;              // Keep it disabled until Com Port is stopped.
                                                    //-----------------------------------
            if (myGlobalBase.VCOMPort == "SCAN")    // Initial Name, if found it become COMxx
            {
                myUSBComPortManager.ScanPortNow();
                if (myGlobalBase.is_Serial_Server_Connected == false)
                    myUSBComPortManager.ShowDialog();       // Better to use ShowDialogue in case not able to select device.
                if (myGlobalBase.is_Serial_Server_Connected == false)
                {
                    btnClosePort.Enabled = true;
                    cbVCOM.Enabled = true;
                    cbFT232RL.Enabled = true;
                    cbCOMList.Enabled = true;
                    cbBaudRateSelect.Enabled = true;
                    btn_ConnectSerial.Enabled = true;
                }
            }
            else   //In this case where COM number is selected, the FTDI is not COM related so it not processed therein.  
            {
                myUSB_VCOM_Comm.VCOM_SerialInit(myGlobalBase.VCOMPort);
                myUSB_VCOM_Comm.VCOM_Start_RX_Operation();
                myGlobalBase.USB_SelectDeviceMode = (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM;
                myGlobalBase.is_Serial_Server_Connected = true;
                btnScanAndConnectTool.Text = "Close Port";
                btnScanAndConnectTool.Enabled = true;
            }
            //-----------------------------------
            if (myGlobalBase.is_Serial_Server_Connected == true)      // Added 12/Dec/17to fix Array and Non-Array interface. 
            {
                //myGlobalBase.MainFeatureSelectedIndex = 0;          // Generic for future work to phase out non-Array VCOM code.
                myGlobalBase.USBArray_RedirectDebugTerm = -1;       // None Array COM Interface Mode
                myGlobalBase.UDTerm_DMFP_ShowSyncCallBack = true;
                USBOption_UpdateTabPageSection();                   // Updated USB Option Tab Page.                  
                tcMainFeature.SelectTab(myGlobalBase.MainFeatureSelectedIndex); // Refresh Tab Page.
            }
            myGlobalBase.USBArray_RedirectDebugTerm = -1;
            btnClosePort.Enabled = true;
        }
        #endregion

        #region//==================================================USB_UART_FTDI_ImplementInterface
        private void USB_UART_FTDI_ImplementInterface()
        {
            DialogResult dr;
            int i = 0;
            bool selected = false;
            myUSBComm.FTDI_Scan_Device();
            if (myUSBComm.FTDI_Device_Count > 1)
            {
                while (true)
                {
                    string s0 = "";
                    string s1 = "";
                    string s2 = "";
                    myUSBComm.FTDI_Scan_GetData(i, ref s0, ref s1, ref s2);
                    dr = MessageBox.Show(s0 + "\n" + s1 + "\n" + s2 + "\n", "Do you wish select this FTDI device?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        myUSBComm.FTDI_Device_Count = (uint)(i + 1);        //selected device
                        selected = true;
                        break;
                    }
                    if (dr == DialogResult.Cancel)
                    {
                        MessageBox.Show("FT232R USB_UART Comm Module Not Found / Unable to Initialize!",
                            "Important Note",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1);
                        cbBaudRateSelect.Enabled = true;
                        return;
                    }
                    i++;
                    if (i == myUSBComm.FTDI_Device_Count)
                        i = 0;
                }
                //TASK: Pop up window to display many FTDI device, so user have choice.
            }
            bool isfound = myUSBComm.FTDI_Seek_Open_FT232R_USB_UART_Module(selected);

            if (isfound == false)
            {
                MessageBox.Show("FT232R USB_UART Comm Module Not Found / Unable to Initialize!",
                    "Important Note",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                cbBaudRateSelect.Enabled = true;
                return;
            }
            myGlobalBase.is_Serial_Server_Connected = true;         // UART alway connected to dsPIC33.
        }
        #endregion

        #region//==================================================FTDI_Summary_ToolStripMenuItem_Click
        private void FTDI_Summary_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (uSBUARTFT232RToolStripMenuItem.Checked == true)
            {
                myFDTI_DataView.UpdateFTDIDevice(myUSBComm.FTDI_DeviceRef);
                myFDTI_DataView.ShowDialog();
                myFDTI_DataView.Hide();
            }
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Tab Master Tools: DataGridView Setup
        //##############################################################################################################

        #region//==================================================struct CommandStruct
        struct CommandStruct
        {
            private readonly string label;
            private readonly int width;

            public CommandStruct(string label, int width)
            {
                this.label = label;
                this.width = width;
            }

            public string Label { get { return label; } }
            public int Width { get { return width; } }

        }
        #endregion

        #region//==================================================Setup Column Format
        static CommandStruct[] myCommndColumnFormat =
            new CommandStruct[]{
                new CommandStruct("Command",120),
                new CommandStruct("Description",220)
            };
        #endregion

        #region//==================================================CmdBox_CellContentClick
        private void CmdBox_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int column = e.ColumnIndex;
            if (column != 0) return;       // Only button is clickable!
            if (row < 0) return;
            //-----------------------
            string CommandString;
            int selectedtab = 0;
            int btn;
            if (this.TabMaster.SelectedTab == TabTool2) selectedtab = 1;
            if (this.TabMaster.SelectedTab == TabTool3) selectedtab = 2;
            if (this.TabMaster.SelectedTab == TabTool4) selectedtab = 3;
            if (this.TabMaster.SelectedTab == TabTool5) selectedtab = 4;
            if (this.TabMaster.SelectedTab == TabTool6) selectedtab = 5;
            if (this.TabMaster.SelectedTab == TabTool7) selectedtab = 6;
            if (this.TabMaster.SelectedTab == TabTool8) selectedtab = 7;

            if (dataGridArray[selectedtab].Rows[row].Cells[1].Value != null)        // Filter out null commands.
            {
                CommandString = dataGridArray[selectedtab].Rows[row].Cells[1].Value.ToString();
                btn = row + (selectedtab * 20);
                if (m_isNewLine == true)
                {
                    Terminal_Append_Request(":");
                    m_isNewLine = false;
                }
                if (cbImmExcuteCmdOption.Checked == true)
                {
                    int i = 19;
                    //-------------------------------------------Put command string into recall buffer
                    while (i > 0)
                    {
                        sCommandList[i] = sCommandList[i - 1];
                        i--;
                    }
                    sCommandList[0] = CommandString;
                    //-------------------------------------------
                    if (myGlobalBase.IsHexDisplayEnable == true)     // Add \r\n to get over hex data. Rev 1D.
                    {
                        Terminal_Append_Request("\r\n#BTN-" + btn.ToString() + ":" + CommandString + " :: ");
                    }
                    else
                    {
                        Terminal_Append_Request("#BTN-" + btn.ToString() + ":" + CommandString + "\r\n");
                    }
                    m_sEntryTxt = CommandString;
                    MsgBoxCommandEntry();
                }
                else   // Enter command but not excuted so user may edit details before excution.
                {
                    Terminal_Append_Request(CommandString + " ");

                }
            }
            TabMaster.Refresh();
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Tab Master Utils
        //##############################################################################################################

        #region//==================================================TabTools_LoadTabMaster
        private void TabTools_LoadTabMaster(object sender, EventArgs e)
        {
            exportCommandCVSToolStripMenuItem_Click(null, e);
        }
        #endregion

        #region//==================================================TabTools_SaveTabMaster
        private void TabTools_SaveTabMaster(object sender, EventArgs e)
        {
            importCommandCVSToolStripMenuItem_Click(null, e);
        }
        #endregion

        #region//==================================================TabTools_RenameTabPageText
        private void TabTools_RenameTabPageText(object sender, EventArgs e)
        {
            var stabshowdialog = this.TabShowDialog(TabMaster.SelectedTab.Text);
            TabMaster.SelectedTab.Text = stabshowdialog;
        }

        public string TabShowDialog(string TabText)
        {
            string backup = TabText;
            bool confirmchange = false;
#pragma warning disable IDE0017 // Simplify object initialization
            Form prompt = new Form();
#pragma warning restore IDE0017 // Simplify object initialization
            prompt.MinimizeBox = false;
            prompt.MaximizeBox = false;
            prompt.StartPosition = FormStartPosition.CenterParent;

            prompt.Width = 100;
            prompt.Height = 100;
            prompt.MaximumSize = new System.Drawing.Size(prompt.Width, prompt.Height);
            prompt.MinimumSize = new System.Drawing.Size(prompt.Width, prompt.Height);

            prompt.Text = "Rename Tab";
            Label textLabel = new Label() { Left = 10, Top = 10, Text = "Rename" };
            TextBox textBox = new TextBox() { Left = 10, Top = 35, Width = 100, Text = TabText, MaxLength = 6 };
            textBox.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    confirmchange = true;
                    prompt.Close();
                }
            };
            Button confirmation = new Button() { Text = "Ok", Left = 70, Width = 40, Top = 5 };
            confirmation.Click += (sender, e) =>
            {
                confirmchange = true;
                prompt.Close();
            };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.ShowDialog();
            if (confirmchange == false)
                return backup;
            return textBox.Text;
        }

        #endregion

        #region//==================================================exportCommandCVSToolStripMenuItem_Click
        private void exportCommandCVSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabMaster.Refresh();

            string strRowValue;

#pragma warning disable IDE0017 // Simplify object initialization
            SaveFileDialog sfd_SaveLinData = new SaveFileDialog();
#pragma warning restore IDE0017 // Simplify object initialization
            sfd_SaveLinData.Filter = "csv files (*.csv)|*.csv";
            sfd_SaveLinData.FileName = sCommandTableFileName;
            sfd_SaveLinData.Title = "Export to CVS File for Setup";

            StringBuilder sb = new StringBuilder();
            for (int v = 0; v < NoOfTabs; v++)
            {
                if (v == 0) sb.AppendLine(TabTool1.Text);
                if (v == 1) sb.AppendLine(TabTool2.Text);
                if (v == 2) sb.AppendLine(TabTool3.Text);
                if (v == 3) sb.AppendLine(TabTool4.Text);
                if (v == 4) sb.AppendLine(TabTool5.Text);
                if (v == 5) sb.AppendLine(TabTool6.Text);
                if (v == 6) sb.AppendLine(TabTool7.Text);
                if (v == 7) sb.AppendLine(TabTool8.Text);

                for (int m = 0; m < 20; m++)
                {
                    strRowValue = (m + (v * 20)).ToString("D3");
                    strRowValue += "," + dataGridArray[v].Rows[m].Cells[1].Value;
                    strRowValue += "," + dataGridArray[v].Rows[m].Cells[2].Value;
                    sb.AppendLine(strRowValue);
                }

            }
            Trace.WriteLine("-INFO : Saving Command List Table");
            DialogResult dr = sfd_SaveLinData.ShowDialog();
            if (dr == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(sfd_SaveLinData.FileName);
                sw.Write(sb.ToString());
                sw.Close();
                Trace.WriteLine("-INFO: Command Table Saved!");
            }
            else
            {
                Trace.WriteLine("+WARN: Filename Not Valid / Rejected Export Procedure");
            }
        }
        #endregion

        #region//==================================================btnExport_Click
        private void btnExport_Click(object sender, EventArgs e)
        {
            exportCommandCVSToolStripMenuItem_Click(null, e);

        }
        #endregion

        #region//==================================================importCommandCVSToolStripMenuItem_Click
        // Updated 03-Aug-2013: change text name on button and set reference to MyDocument in C drive so it repeatable for all PC.
        private void importCommandCVSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string rowValue;
            string[] cellValue;
            //-------------------------Clear Array
            for (int v = 0; v < NoOfTabs; v++)
            {
                if (v == 0) TabTool1.Text = "Tool1";
                if (v == 1) TabTool2.Text = "Tool2";
                if (v == 2) TabTool3.Text = "Tool3";
                if (v == 3) TabTool4.Text = "Tool4";
                if (v == 4) TabTool5.Text = "Tool5";
                if (v == 5) TabTool6.Text = "Tool6";
                if (v == 6) TabTool7.Text = "Tool7";
                if (v == 7) TabTool8.Text = "Tool8";

                for (int m = 0; m < 20; m++)
                {
                    dataGridArray[v].Rows[m].Cells[1].Value = "";
                    dataGridArray[v].Rows[m].Cells[2].Value = "";
                }
            }

            OpenFileDialog ofd_ImportLinData = new OpenFileDialog();
            ofd_ImportLinData.Filter = "csv files (*.csv)|*.csv";
            ofd_ImportLinData.FileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ofd_ImportLinData.Title = "Load CVS File for Command Table";
            DialogResult dr = ofd_ImportLinData.ShowDialog();
            if (dr == DialogResult.OK)
            {
                Trace.WriteLine("-INFO : Loading data file into Command Table");
                sCommandTableFileName = ofd_ImportLinData.FileName;
                StreamReader sr = new StreamReader(ofd_ImportLinData.FileName);
                try
                {
                    while (sr.Peek() != -1)
                    {
                        for (int v = 0; v < NoOfTabs; v++)
                        {
                            if (v == 0) TabTool1.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 1) TabTool2.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 2) TabTool3.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 3) TabTool4.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 4) TabTool5.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 5) TabTool6.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 6) TabTool7.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 7) TabTool8.Text = sr.ReadLine().Replace(",", ""); ;

                            for (int m = 0; m < 20; m++)
                            {
                                rowValue = sr.ReadLine();
                                cellValue = rowValue.Split(',');
                                dataGridArray[v].Rows[m].Cells[1].Value = cellValue[1];
                                dataGridArray[v].Rows[m].Cells[2].Value = cellValue[2];
                            }
                        }
                        break;
                    }
                    Trace.WriteLine("-INFO : Command Data Loaded");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("###INFO:Problem with Import File: " + ex.Message);
                }
                sr.Close();
            }
            else
            {
                Trace.WriteLine("+WARN: Filename Not Valid / Rejected Import Procedure");
            }
            TabMaster.Refresh();
        }
        #endregion

        #region//==================================================btnImport_Click
        private void btnImport_Click(object sender, EventArgs e)
        {
            importCommandCVSToolStripMenuItem_Click(null, e);

        }
        #endregion

        //##############################################################################################################
        //=============================================================================================rtbTerm Terminal Section
        //##############################################################################################################

        #region //==================================================btnHalt_Click (Halt screen activity to user control edit command and stay focussed on that). 
        private void btnHalt_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.isTermScreenHalted == false)
            {
                rtbTerm.BackColor = System.Drawing.Color.DarkSlateBlue;
                rtbTerm.Refresh();
                myGlobalBase.isTermScreenHalted = true;

            }
            else
            {
                rtbTerm.BackColor = System.Drawing.Color.DarkSlateGray;
                rtbTerm.Refresh();
                myGlobalBase.isTermScreenHalted = false;
            }
        }
        #endregion

        #region //==================================================myRtbTermMessage
        //==========================================================
        // Purpose  : Append message in terminal window,. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        public delegate void myRtbTermMessage_StartDelegate(string Message);
        public void myRtbTermMessage(string Message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new myRtbTermMessage_StartDelegate(myRtbTermMessage), new object[] { Message });
                return;
            }
            //----------------------------------------------------------------------------------
            if (myGlobalBase.bSessionDataLogTerm == true)                                         // Do not include ASYNC LD and LE datalog stream into terminal. 
            {
                if (Message.StartsWith("~LD(", StringComparison.Ordinal) || Message.StartsWith("~LE(", StringComparison.Ordinal))      // Check for Datalog data stream. 
                {
                    return;
                }
            }
            //----------------------------------------------------------------------------------
            if ((myGlobalBase.isTermUpdateModeTimerActivated == false) & (myGlobalBase.iTerminalUpdateMode != (int)GlobalBase.eTerminalUpdate.TermREAL))
            {
                myGlobalBase.TermHaltMessageBuf += Message;
                myGlobalBase.isTermUpdateModeTimerActivated = true;
                bgwTermUpdate.RunWorkerAsync();
                return;
            }
            //----------------------------------------------------------------------------------
            if (myGlobalBase.iTerminalUpdateMode != (int)GlobalBase.eTerminalUpdate.TermREAL)   // Delay Time Terminal refresh. 
            {
                myGlobalBase.TermHaltMessageBuf += Message;
                return;
            }
            //----------------------------------------------------------------------------------
            if (myGlobalBase.isTermScreenHalted == false)
            {
                if (rtbTerm != null)
                {
                    Tools.rtb_StopRepaint(rtbTerm, rtbOEMx);
                    rtbTerm.SelectionFont = myGlobalBase.FontResponse;
                    rtbTerm.SelectionColor = myGlobalBase.ColorResponse;
                    rtbTerm.SelectionStart = rtbTerm.TextLength;
                    rtbTerm.ScrollToCaret();
                    rtbTerm.Select();
                    rtbTerm.AppendText(myGlobalBase.TermHaltMessageBuf + Message);
                    Tools.rtb_StartRepaint(rtbTerm, rtbOEMx);
                }
                myGlobalBase.TermHaltMessageBuf = "";
            }
            else
            {
                myGlobalBase.TermHaltMessageBuf += Message;
            }
        }
        #endregion

        #region //==================================================myRtbTermMessageLF
        //==========================================================
        // Purpose  : Append message in terminal window,. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================

        public delegate void myRtbTermMessageLF_StartDelegate(string Message);
        public void myRtbTermMessageLF(string Message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new myRtbTermMessageLF_StartDelegate(myRtbTermMessageLF), new object[] { Message });
                return;
            }
            //----------------------------------------------------------------------------------
            if (myGlobalBase.bSessionDataLogTerm == true)                                       // Do not include ASYNC LD and LE datalog stream into terminal. 
            {
                if (Message.Contains("~LD(") || Message.Contains("~LE("))                       // Check for Datalog data stream. 
                {
                    return;
                }
            }
            //----------------------------------------------------------------------------------
            if ((myGlobalBase.isTermUpdateModeTimerActivated == false) & (myGlobalBase.iTerminalUpdateMode != (int)GlobalBase.eTerminalUpdate.TermREAL))
            {
                myGlobalBase.TermHaltMessageBuf += Message + "\r\n";
                myGlobalBase.isTermUpdateModeTimerActivated = true;
                bgwTermUpdate.RunWorkerAsync();
                return;
            }
            //----------------------------------------------------------------------------------
            if (myGlobalBase.iTerminalUpdateMode != (int)GlobalBase.eTerminalUpdate.TermREAL)   // Delay Time Terminal refresh. 
            {
                myGlobalBase.TermHaltMessageBuf += Message + "\r\n";
                return;
            }
            //----------------------------------------------------------------------------------
            if (myGlobalBase.isTermScreenHalted == false)
            {
                if (rtbTerm != null)
                {
                    Tools.rtb_StopRepaint(rtbTerm, rtbOEMx);
                    rtbTerm.SelectionFont = myGlobalBase.FontResponse;
                    rtbTerm.SelectionColor = myGlobalBase.ColorResponse;
                    rtbTerm.SelectionStart = rtbTerm.TextLength;
                    rtbTerm.ScrollToCaret();
                    rtbTerm.Select();
                    rtbTerm.AppendText(myGlobalBase.TermHaltMessageBuf + Message + "\r\n");
                    Tools.rtb_StartRepaint(rtbTerm, rtbOEMx);
                }
                myGlobalBase.TermHaltMessageBuf = "";
            }
            else
            {
                myGlobalBase.TermHaltMessageBuf += Message + "\r\n";
            }
        }
        #endregion

        #region//==================================================Terminal_Append_Request
        public void Terminal_Append_Request(string text)
        {
            if (myGlobalBase.isTermScreenHalted == false)
            {
                Tools.rtb_StopRepaint(rtbTerm, rtbOEMx);
                rtbTerm.SelectionStart = rtbTerm.TextLength;
                rtbTerm.ScrollToCaret();
                rtbTerm.Select();
                rtbTerm.Focus();
                rtbTerm.SelectionFont = myGlobalBase.FontMessage;
                rtbTerm.SelectionColor = myGlobalBase.ColorMessage;
                rtbTerm.AppendText(myGlobalBase.TermHaltMessageBuf + text);
                Tools.rtb_StartRepaint(rtbTerm, rtbOEMx);
                //ConsoleAppendMessage(text);
                myGlobalBase.TermHaltMessageBuf = "";
            }
            else
            {
                myGlobalBase.TermHaltMessageBuf += text;
            }
        }
        #endregion

        #region //==================================================Append Response Message Goes Here
        public delegate void Terminal_Append_Response_StartDelegate(string Message);
        public void Terminal_Append_Response(string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Terminal_Append_Response_StartDelegate(Terminal_Append_Response), new object[] { message });
                return;
            }
            if (myGlobalBase.isTermScreenHalted == false)
            {
                Tools.rtb_StopRepaint(rtbTerm, rtbOEMx);
                rtbTerm.SelectionFont = myGlobalBase.FontResponse;
                rtbTerm.SelectionColor = myGlobalBase.ColorResponse;
                message.TrimEnd(new char[] { '\r', '\n' });
                rtbTerm.AppendText(myGlobalBase.TermHaltMessageBuf + message);
                ConsoleAppendMessage(myGlobalBase.TermHaltMessageBuf + message);
                rtbTerm.SelectionStart = rtbTerm.TextLength;
                rtbTerm.ScrollToCaret();
                rtbTerm.Select();
                Tools.rtb_StartRepaint(rtbTerm, rtbOEMx);
                //rtbTerm.Focus();
                myGlobalBase.TermHaltMessageBuf = "";
            }
            else
            {
                myGlobalBase.TermHaltMessageBuf += message + "\r\n";
            }

        }
        #endregion

        #region //==================================================Insert Command Error or Warning Message
        private void CommandError(string errormessage)
        {
            rtbTerm.SelectionFont = myGlobalBase.FontError;
            rtbTerm.SelectionColor = myGlobalBase.ColorError;
            rtbTerm.AppendText("##ERR  : " + errormessage + "\r\n");
            ConsoleAppendMessage(errormessage);
            rtbTerm.SelectionStart = rtbTerm.TextLength;
            rtbTerm.ScrollToCaret();
            myGlobalBase.TermHaltMessageBuf = "";
        }
        private void CommandWarning(string warnmessage)
        {
            rtbTerm.SelectionFont = myGlobalBase.FontWarning;
            rtbTerm.SelectionColor = myGlobalBase.ColorWarning;
            rtbTerm.AppendText("++WARNING: " + warnmessage + "\r\n");
            ConsoleAppendMessage(warnmessage);
            rtbTerm.SelectionStart = rtbTerm.TextLength;
            rtbTerm.ScrollToCaret();
        }
        #endregion

        #region//==================================================btnClear_Click
        private void btnClear_Click(object sender, EventArgs e)
        {
            //DialogResult dr = MessageBox.Show("Are you sure to clear all list?", "Clearing the List", MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);
            //if (dr == DialogResult.OK)
            //{
            rtbTerm.Text = "";
            rtbTerm.Clear();
            rtbTerm.Refresh();
            //}
        }
        #endregion

        #region //==================================================rtbTermfRichTextBox_Ref
        //==========================================================
        // Purpose  : Append message in terminal window,. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        public System.Windows.Forms.RichTextBox rtbTermfRichTextBox_Ref()
        {
            return (this.myrtbTerm);
        }
        #endregion

        #region//=============================================================================================updateRealTimeToolStripMenuItem_Click
        private void updateRealTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateRealTimeToolStripMenuItem.CheckState = CheckState.Checked;
            update1SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            update2SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            update5SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            myGlobalBase.iTerminalUpdateMode = (int)GlobalBase.eTerminalUpdate.TermREAL;
        }
        #endregion

        #region//=============================================================================================update1SecondToolStripMenuItem_Click
        private void update1SecondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateRealTimeToolStripMenuItem.CheckState = CheckState.Unchecked;
            update1SecondToolStripMenuItem.CheckState = CheckState.Checked;
            update2SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            update5SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            myGlobalBase.iTerminalUpdateMode = (int)GlobalBase.eTerminalUpdate.TermSEC1;
        }
        #endregion

        #region//=============================================================================================update2SecondToolStripMenuItem_Click
        private void update2SecondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateRealTimeToolStripMenuItem.CheckState = CheckState.Unchecked;
            update1SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            update2SecondToolStripMenuItem.CheckState = CheckState.Checked;
            update5SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            myGlobalBase.iTerminalUpdateMode = (int)GlobalBase.eTerminalUpdate.TermSEC2;
        }
        #endregion

        #region//=============================================================================================update5SecondToolStripMenuItem_Click
        private void update5SecondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateRealTimeToolStripMenuItem.CheckState = CheckState.Unchecked;
            update1SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            update2SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            update5SecondToolStripMenuItem.CheckState = CheckState.Checked;
            myGlobalBase.iTerminalUpdateMode = (int)GlobalBase.eTerminalUpdate.TermSEC5;
        }
        #endregion

        #region//=============================================================================================excludingDataLogStreamToolStripMenuItem_Click
        private void excludingDataLogStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (excludingDataLogStreamToolStripMenuItem.CheckState == CheckState.Checked)
            {
                myGlobalBase.bSessionDataLogTerm = false;
                excludingDataLogStreamToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            else
            {
                myGlobalBase.bSessionDataLogTerm = true;
                excludingDataLogStreamToolStripMenuItem.CheckState = CheckState.Checked;
            }
        }
        #endregion

        #region//=============================================================================================excludeL1StealthToolStripMenuItem_Click
        private void excludeL1StealthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (excludeL1StealthToolStripMenuItem.CheckState == CheckState.Checked)
            {
                myGlobalBase.bSessionHideStealthTerm = false;
                excludeL1StealthToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            else
            {
                myGlobalBase.bSessionHideStealthTerm = true;
                excludeL1StealthToolStripMenuItem.CheckState = CheckState.Checked;
            }

        }
        #endregion

        #region//=============================================================================================excludeL2SessionLogToolStripMenuItem_Click
        private void excludeL2SessionLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (excludeL2SessionLogToolStripMenuItem.CheckState == CheckState.Checked)
            {
                myGlobalBase.bSessionHideSessionTerm = false;
                excludeL2SessionLogToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            else
            {
                myGlobalBase.bSessionHideSessionTerm = true;
                excludeL2SessionLogToolStripMenuItem.CheckState = CheckState.Checked;
            }
        }
        #endregion

        #region//=============================================================================================wordWrapToolStripMenuItem_CheckStateChanged
        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (wordWrapToolStripMenuItem.CheckState == CheckState.Checked)
            {
                rtbTerm.WordWrap = false;
                rtbTerm.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
                wordWrapToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            else
            {
                rtbTerm.WordWrap = true;
                rtbTerm.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
                wordWrapToolStripMenuItem.CheckState = CheckState.Checked;
            }
            rtbTerm.SelectionStart = rtbTerm.TextLength;
            rtbTerm.ScrollToCaret();
            rtbTerm.Select();
            rtbTerm.Refresh();
        }
        #endregion

        #region//=============================================================================================bgwTermUpdate_DoWork
        private void bgwTermUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            if (bgwTermUpdate.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            switch (myGlobalBase.iTerminalUpdateMode)
            {
                case ((int)GlobalBase.eTerminalUpdate.TermSEC1):
                    {
                        Thread.Sleep(1000);
                        break;
                    }
                case ((int)GlobalBase.eTerminalUpdate.TermSEC2):
                    {
                        Thread.Sleep(2000);
                        break;
                    }
                case ((int)GlobalBase.eTerminalUpdate.TermSEC5):
                    {
                        Thread.Sleep(5000);
                        break;
                    }
                default:
                    break;

            }
        }
        #endregion

        #region//=============================================================================================bgwTermUpdate_RunWorkerCompleted
        private void bgwTermUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //----------------------------------------------------------------------------------
            if (myGlobalBase.bSessionDataLogTerm == true)                                         // Do not include ASYNC LD and LE datalog stream into terminal. 
            {
                if (myGlobalBase.TermHaltMessageBuf.StartsWith("~LD(", StringComparison.Ordinal) || myGlobalBase.TermHaltMessageBuf.StartsWith("~LE(", StringComparison.Ordinal))      // Check for Datalog data stream. 
                {
                    if (myGlobalBase.iTerminalUpdateMode != (int)GlobalBase.eTerminalUpdate.TermREAL)       // Keep looping. 
                        bgwTermUpdate.RunWorkerAsync();
                    else
                        myGlobalBase.isTermUpdateModeTimerActivated = false;
                    return;
                }
            }
            Tools.rtb_StopRepaint(rtbTerm, rtbOEMx);
            rtbTerm.SelectionFont = myGlobalBase.FontResponse;
            rtbTerm.SelectionColor = myGlobalBase.ColorResponse;
            rtbTerm.SelectionStart = rtbTerm.TextLength;
            rtbTerm.ScrollToCaret();
            rtbTerm.Select();
            rtbTerm.AppendText(myGlobalBase.TermHaltMessageBuf);
            Tools.rtb_StartRepaint(rtbTerm, rtbOEMx);
            myGlobalBase.TermHaltMessageBuf = "";
            if (myGlobalBase.iTerminalUpdateMode != (int)GlobalBase.eTerminalUpdate.TermREAL)       // Keep looping. 
                bgwTermUpdate.RunWorkerAsync();
            else
                myGlobalBase.isTermUpdateModeTimerActivated = false;
        }
        #endregion

        #region //=============================================================================================tsmiHideCallBack_Click
        private void tsmiHideCallBack_Click(object sender, EventArgs e)
        {
            myGlobalBase.bisHideCallBackChecked = tsmiHideCallBack.Checked;
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Terminal Protocol enable/disable
        //##############################################################################################################

        #region//==================================================BtnEnableTerminalProtocol_Click
        private void BtnEnableTerminalProtocol_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Under Development, assumed deactivated");
            myGlobalBase.is_Terminal_Protocol_Activated = false;
        }
        #endregion

        #region//==================================================BtnDisableTerminalProtocol_Click
        private void BtnDisableTerminalProtocol_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Under Development, assumed deactivated");
            myGlobalBase.is_Terminal_Protocol_Activated = false;
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Log Console Message into File
        //##############################################################################################################

        #region //==================================================setupLogFolderToolStripMenuItem_Click
        private void setupLogFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (sFoldername == "")
                {
                    sFoldername = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    sFoldername = sFoldernameDefault + @"\MainTerm";
                }
                else  // sDefaultFolder was defined, so check folder for existence if not create it. 
                {
                    System.IO.Directory.CreateDirectory(sFoldername);
                }
            }
            catch
            {
                sFoldername = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                sFoldername = sFoldernameDefault + @"\MainTerm";
            }
            folderBrowserDialog1.SelectedPath = sFoldername;
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
        }
        #endregion

        #region //==================================================ConsoleAppendMessage  Append Text
        private void ConsoleAppendMessage(string message)
        {
            if (myGlobalBase.IsLogEnable == false) return;
            if (message == ".") return;                           // No need to log "." used as timing tick.
            if (myGlobalBase.sFilename == "") return;           // Avoid error.
            try
            {
                string sdatetime = DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss");
                using (StreamWriter sw = File.AppendText(myGlobalBase.sFilename))           //using statement ensure to dispose and close sw.
                {
                    sw.Write(sdatetime + " ::: " + message + Environment.NewLine);
                }
            }
            catch (AmbiguousMatchException)
            {
                Terminal_Append_Request("#ERR : Unable to append message into Filename: " + myGlobalBase.sFilename + Environment.NewLine);
                enableLoggingToolStripMenuItem.Checked = false;
                myGlobalBase.IsLogEnable = false;
            }
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Logger Window Section
        //##############################################################################################################

        #region //==================================================btnLoggerCSV_Click
        private void btnLoggerCSV_Click(object sender, EventArgs e)
        {
            Terminal_Append_Response("===================================Logger Mode\n");
            Terminal_Append_Response("====== DO NOT USE MAIN TERMINAL ! ============\n");
            Terminal_Append_Response("==============================================\n");
            myGlobalBase.LoggerOpertaionMode = true;
            myGlobalBase.LoggerWindowVisable = true;
            myLoggerCSV.Show();
            Thread.Sleep(1);
            myLoggerCSV.LoggerCVS_SetupShow_Window(0, "");
        }
        #endregion

        #region //==================================================enableLoggingToolStripMenuItem_Click
        private void enableLoggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sFoldername == "")
                return;                        // rejected request for safety
            myGlobalBase.IsLogEnable = true;
            enableLoggingToolStripMenuItem.Checked = true;      // Set to checked state as shown activate logging.
            string archiveFileName = DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "__Log.txt";
            myGlobalBase.sFilename = System.IO.Path.Combine(sFoldername, archiveFileName);
            try
            {
                FileStream fsa = null;
                try
                {
                    fsa = new FileStream(myGlobalBase.sFilename, FileMode.Append, FileAccess.Write, FileShare.Write);
                }
                finally
                {
                    if (fsa != null)
                    {
                        fsa.Dispose();
                    }
                    Terminal_Append_Request("-INFO : Created new File+AppendNote: " + myGlobalBase.sFilename + Environment.NewLine);
                }
            }
            catch (AmbiguousMatchException)
            {
                Terminal_Append_Request("#ERR : Unable to create AppendNote File: " + myGlobalBase.sFilename + Environment.NewLine);
                enableLoggingToolStripMenuItem.Checked = false;
                myGlobalBase.IsLogEnable = false;
            }
        }
        #endregion

        #region //==================================================ceaseLoggingToolStripMenuItem_Click
        private void ceaseLoggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enableLoggingToolStripMenuItem.Checked = false;
            myGlobalBase.IsLogEnable = false;
        }
        #endregion

        #region //==================================================openLogFolderToolStripMenuItem_Click
        private void openLogFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sFoldername == "")
                return;
            string myPath = sFoldername;
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = myPath;
            prc.Start();
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================FFT Window
        //##############################################################################################################

        #region //==================================================btnFFTWindow_Click
        private void btnFFTWindow_Click(object sender, EventArgs e)
        {
            myGlobalBase.FFTOpertaionMode = true;
            myFFWindow.Show();
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Import CVS Window
        //##############################################################################################################

        #region //==================================================butImportCVS_OpenWindow_Click
        private void butImportCVS_OpenWindow_Click(object sender, EventArgs e)
        {
            myGlobalBase.ImportCVSOpertaionMode = true;
            myImportDataCVS.Show();

        }
        #endregion

        #region //==================================================cB_EnableAutoDetect_MouseClick
        private void cB_EnableAutoDetect_MouseClick_1(object sender, MouseEventArgs e)
        {
            myGlobalBase.IsImportRawDataEnable = cB_EnableAutoDetect.Checked;
            myrtbTerm.ScrollToCaret();
            myrtbTerm.Select();
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Control S save TabMaster (CVS)
        //##############################################################################################################

        #region //==================================================MainProg_KeyDown
        private void MainProg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)           // CNTR+S, save command table (shortcut key)
            {
                exportCommandCVSToolStripMenuItem_Click(null, e);
            }
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Cut/Copy/Paste action 
        //##############################################################################################################

        #region//==================================================rtbTerm_MouseUp
        private void rtbTerm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {   //click event
                //MessageBox.Show("you got it!");
            }
        }
        #endregion

        /* // Old Clip Code
		//--------------------------------------Context Menu
			ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
			MenuItem menuItem = new MenuItem("Cut");
			menuItem.Click += new EventHandler(CutAction);
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Copy");
			menuItem.Click += new EventHandler(CopyAction);
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Paste");
			menuItem.Click += new EventHandler(PasteAction);
			contextMenu.MenuItems.Add(menuItem);
			rtbTerm.ContextMenu = contextMenu;
		void CutAction(object sender, EventArgs e)
		{
			rtbTerm.Cut();
		}

		void CopyAction(object sender, EventArgs e)
		{
			if (rtbTerm.SelectedText != "")
				Clipboard.SetText(rtbTerm.SelectedText);
		}

		void PasteAction(object sender, EventArgs e)
		{
			if (Clipboard.ContainsText(TextDataFormat.Rtf))
			{
				rtbTerm.Text += Clipboard.GetText(TextDataFormat.Text).ToString();
			}
		}
		*/

        //##############################################################################################################
        //=============================================================================================Hex Display Section
        //##############################################################################################################

        #region //==================================================cBox_HexDisplay_Enable_MouseClick
        private void cBox_HexDisplay_Enable_MouseClick(object sender, MouseEventArgs e)
        {
            myGlobalBase.IsHexDisplayEnable = cBox_HexDisplay_Enable.Checked;
            myrtbTerm.ScrollToCaret();
            myrtbTerm.Select();
        }
        #endregion

        #region //==================================================cB_HexDisplay_EnableLF_MouseClick
        private void cB_HexDisplay_EnableLF_MouseClick(object sender, MouseEventArgs e)
        {
            myGlobalBase.IsHexDisplayEnableLF = cB_HexDisplay_EnableLF.Checked;
            myrtbTerm.ScrollToCaret();
            myrtbTerm.Select();
        }
        #endregion

        #region //==================================================cB_HexDisplay_TXAdd_LF_MouseClick
        private void cB_HexDisplay_TXAdd_LF_MouseClick(object sender, MouseEventArgs e)
        {
            myGlobalBase.IsHexSendEnableWithLF = cB_HexDisplay_TXAdd_LF.Checked;
            myrtbTerm.ScrollToCaret();
            myrtbTerm.Select();

        }
        #endregion

        #region //==================================================but_HexDisplay_NewLine_Click
        private void but_HexDisplay_NewLine_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.isTermScreenHalted == false)
            {
                Tools.rtb_StopRepaint(myrtbTerm, rtbOEMx);
                myrtbTerm.SelectionFont = myGlobalBase.FontResponse;
                myrtbTerm.SelectionColor = myGlobalBase.ColorResponse;
                myrtbTerm.SelectionStart = myrtbTerm.TextLength;
                myrtbTerm.ScrollToCaret();
                myrtbTerm.Select();
                myrtbTerm.AppendText(myGlobalBase.TermHaltMessageBuf + "\r\n");
                myGlobalBase.TermHaltMessageBuf = "";
                Tools.rtb_StartRepaint(myrtbTerm, rtbOEMy);
            }
            else
            {
                myGlobalBase.TermHaltMessageBuf += "\r\n";
            }
        }
        #endregion

        #region //==================================================hexDisplayModeToolStripMenuItem_Click
        // Note     :This is help Menu using frm and textbox. This need further work to size it and avoid form adjustment. 
        //          :This is simpler style compared to messagebox.
        private void hexDisplayModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            TextBox tbx = new TextBox();
            frm.Controls.Add(tbx);
            tbx.Multiline = true;
            tbx.Height = frm.Height;
            tbx.Width = frm.Width;

            tbx.AppendText("Help Section for Hex based Display \r\n");
            tbx.AppendText("----------------------------------------------------------\r\n");
            tbx.AppendText("This feature allow display of receives data in hex form.\r\n");
            tbx.AppendText("So that non-ASCII data can be inspected. This is relevant\r\n");
            tbx.AppendText("for Short Hop Project where UART protocol (non-ASCII)\r\n");
            tbx.AppendText("can be validated or debugged\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText("(1) To enable this feature, check 'Display Hex'\r\n");
            tbx.AppendText("(2) 'Receives \n as LF': This detect 0x0A which add LF\r\n");
            tbx.AppendText("     NB: '~' is added after the 0A to signify this event\r\n");
            tbx.AppendText("(3) 'Send with \\r\\n': This added when enter is pressed\r\n");
            tbx.AppendText("(4) Tip: To avoid duplicate issue, use 'New Line' button\r\n");
            tbx.AppendText("(5) To enter hex byte, type <00><2F><A4><5F> and enter\r\n");
            tbx.AppendText("(6) To enter hex word, type <002F><A45F> and enter\r\n");
            tbx.AppendText("     NB: Only 8 or 16 bits is supported.\r\n");
            tbx.AppendText("Note: The codes is crafted to excludes unicode ASCII\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText("Revision 1D, RPayne (18-Feb-15)\r\n");
            frm.ShowDialog();
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Tab Master Mouse Control (width adjustment)
        //##############################################################################################################

        #region //==================================================CbCompactWidth_MouseClick
        private void CbCompactWidth_MouseClick(object sender, MouseEventArgs e)
        {
            if (CbCompactWidth.Checked == true)
            {
                myGlobalBase.IsCompactViewEnable = true;
            }
            else
            {
                myGlobalBase.IsCompactViewEnable = false;
            }
            this.SuspendLayout();
            this.TabMaster.SuspendLayout();
            this.TabTool1.SuspendLayout();
            if (myGlobalBase.IsCompactViewEnable == true)
            {
                this.Size = new System.Drawing.Size(1000, 657);    // Original size

                this.TabTool1.Size = new System.Drawing.Size(278, 431);
                this.TabMaster.Size = new System.Drawing.Size(286, 457);
                for (int i = 0; i < NoOfTabs; i++)
                {
                    this.dataGridArray[i].Size = new System.Drawing.Size(278, 487);
                }
            }
            else
            {
                this.Size = new System.Drawing.Size(1100, 657);    // Original size
                this.TabTool1.Size = new System.Drawing.Size(378, 431);
                this.TabMaster.Size = new System.Drawing.Size(386, 457);
                for (int i = 0; i < NoOfTabs; i++)
                {
                    this.dataGridArray[i].Size = new System.Drawing.Size(378, 487);
                }
                this.TabTool1.Size = new System.Drawing.Size(378, 431);
                this.TabTool2.Size = new System.Drawing.Size(378, 431);
                this.TabTool3.Size = new System.Drawing.Size(378, 431);
                this.TabTool4.Size = new System.Drawing.Size(378, 431);
                this.TabTool5.Size = new System.Drawing.Size(378, 431);
                this.TabTool6.Size = new System.Drawing.Size(378, 431);
                this.TabTool7.Size = new System.Drawing.Size(378, 431);
                this.TabTool8.Size = new System.Drawing.Size(378, 431);

            }
            this.TabMaster.ResumeLayout(false);
            this.TabTool1.ResumeLayout(false);
            this.ResumeLayout();
            this.Refresh();
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Main Feature Tab Control 
        //##############################################################################################################

        #region//=============================================================================================tcMainFeature_SelectedIndexChanged
        private void tcMainFeature_SelectedIndexChanged(object sender, EventArgs e)
        {
            myGlobalBase.MainFeatureSelectedIndex = tcMainFeature.SelectedIndex;
            cbUSB_AutoDetect.Checked = myGlobalBase.configb[0].bOptionAutoDetectEnable;
            if (myGlobalBase.CanPCAN_Refresh == true)
            {
                PCAN_Update_TabPanel();
                myGlobalBase.CanPCAN_Refresh = false;
                this.Refresh();
            }
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================About 
        //##############################################################################################################

        #region //==================================================aboutToolStripMenuItem_Click
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.CompanyName == (int)GlobalBase.eCompanyName.ADT)
            {
                ADT_About_Box box = new ADT_About_Box();
                box.ShowDialog();
            }
            if (myGlobalBase.CompanyName == (int)GlobalBase.eCompanyName.BGDS)
            {
                BG_About_Box box = new BG_About_Box();
                box.ShowDialog();
            }
            if (myGlobalBase.CompanyName == (int)GlobalBase.eCompanyName.TVB)           //Rev 71
            {
                TVB_About_Box box = new TVB_About_Box();
                box.ShowDialog();
            }
            if (myGlobalBase.CompanyName == (int)GlobalBase.eCompanyName.NONE)          //Rev 71
            {
                TVB_About_Box box = new TVB_About_Box();
                box.ShowDialog();
            }

        }
        #endregion

        //##############################################################################################################
        //=============================================================================================VCOM Section
        //##############################################################################################################

        #region //=============================================================================================cbBaudRateSelect_SelectedIndexChanged
        private void cbBaudRateSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            myGlobalBase.Select_Baudrate = (uint)cbBaudRateSelect.SelectedIndex;
            Console.Beep(1000, 100);
        }
        #endregion

        #region//=============================================================================================cbFT232RL_CheckedChanged
        private void cbFT232RL_CheckedChanged(object sender, EventArgs e)
        {
            if (cbVCOM.Checked == true)
                myGlobalBase.VCOMMode = 0;
            else
                myGlobalBase.VCOMMode = 2;

        }
        #endregion

        #region//=============================================================================================cbVCOM_CheckedChanged
        private void cbVCOM_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFT232RL.Checked == true)
                myGlobalBase.VCOMMode = 2;
            else
                myGlobalBase.VCOMMode = 1;
        }
        #endregion

        #region//=============================================================================================VCOM_ProcessCommand (pulic function for quick command insertion)
        public void VCOM_ProcessCommand(string Message)
        {
            m_sResponseTxt = Message;
            myGlobalBase.LoggerOpertaionMode = true;
            //---------------------------------------------------------------------- SNAD Address added to command message Mods. 
            if (myGlobalBase.mySNADNetwork != null)
            {
                m_sEntryTxt = myGlobalBase.mySNADNetwork.SNAD_Modify_UDT_Command(m_sResponseTxt);
            }
            //-----------------------------------------------------------------------
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == true)
            {
                myUSB_VCOM_Comm.VCOMArray_Message_Send(m_sResponseTxt, (int)eUSBDeviceType.ADIS);
            }
            else if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == true)
            {
                myUSB_VCOM_Comm.VCOMArray_Message_Send(m_sResponseTxt, (int)eUSBDeviceType.BGDRILLING);
            }
            else if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == true)     // Rev 72. 
            {
                myUSB_VCOM_Comm.VCOMArray_Message_Send(m_sResponseTxt, (int)eUSBDeviceType.MiniAD7794);
            }
            else if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM)
            {
                myUSB_VCOM_Comm.VCOM_Message_Send(m_sResponseTxt);
            }
            else if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_FTDI)
            {
                //### Check if dsPIC33 is actually connected, if not reject command....this need review/optional.
                myUSBComm.FTDI_Message_Send(m_sResponseTxt);
            }
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Company Themes
        //##############################################################################################################

        #region//======================================================CompanyName_Theme_Update
        private void CompanyName_Theme_Update()
        {
            switch (myGlobalBase.CompanyName)
            {
                case ((int)GlobalBase.eCompanyName.BGDS):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPageTVB1D;
                        break;
                    }
                case ((int)GlobalBase.eCompanyName.ADT):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_1A;
                        break;
                    }
                case ((int)GlobalBase.eCompanyName.TVB):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPageTVB_1B;
                        break;
                    }
                default:
                    {
                        this.BackgroundImage = null;       // Default
                        break;
                    }
            }
            this.Refresh();
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================XML and Setup File 
        //##############################################################################################################

        #region //==================================================optionConfigToolStripMenuItem_Click (App Configuration)
        private void optionConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] tabname = new string[tcMainFeature.TabCount];
            int i = 0;
            foreach (TabPage tab in tcMainFeature.TabPages)
            {
                tabname[i] = tab.Text;
                i++;
            }
            myAppConfigWindow.SetupTabList(tabname);
            myAppConfigWindow.ShowDialog();
            CompanyName_Theme_Update();
            myBaudRate_UpdateSetting();
            tcMainFeature.SelectTab(myGlobalBase.MainFeatureSelectedIndex);
        }
        #endregion

        #region //==================================================OpenOptionWindow (App Configuration)
        public void OpenOptionWindow()
        {
            myAppConfigWindow.ShowDialog();
            CompanyName_Theme_Update();
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================-Date/Time (Universal Command)
        //##############################################################################################################

        #region //==================================================getTimeDateToolStripMenuItem_Click (Set Date/Time)
        private void btnRTCSend_Click(object sender, EventArgs e)
        {
            RTC_Setup_SendNow();
        }
        #endregion

        #region //==================================================RTC_Setup_SendNow (Set Date/Time)
        public void RTC_Setup_SendNow()
        {
            DateTime dt2 = DateTime.Now;
            DateTime dt = dt2.ToLocalTime();
            m_sEntryTxt = "SET_TIME(";
            m_sEntryTxt += String.Format("{0:HH;mm;ss}", dt);
            m_sEntryTxt += ")";
            Terminal_Append_Response(m_sEntryTxt + '\n');
            Command_ASCII_Process_UART();
            System.Threading.Thread.Sleep(100);     //100mSec pause
            m_sEntryTxt = "SET_DATE(";
            m_sEntryTxt += String.Format("{0:yyyy;MM;dd}", dt);
            m_sEntryTxt += ")";
            Terminal_Append_Response(m_sEntryTxt + '\n');
            Command_ASCII_Process_UART();
        }
        #endregion

        #region //==================================================sendTimeDateToolStripMenuItem_Click  (Get Date/Time)
        private void btnRTCGet_Click(object sender, EventArgs e)
        {
            RTC_Setup_GetNow();
        }
        #endregion

        #region //==================================================RTC_Setup_GetNow() (Get Date/Time)
        public void RTC_Setup_GetNow()
        {
            m_sEntryTxt = "GET_TIME()";
            Terminal_Append_Response(m_sEntryTxt + '\n');
            Command_ASCII_Process_UART();
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================HMR2300
        //##############################################################################################################

        #region //==================================================HMR2300_Init
        private void HMR2300_Init()
        {
            cbHMR_9600.Enabled = true;
            cbHMR_19200.Enabled = true;
            btnHMRScanPort.Enabled = true;
            btnHMRClosePort.Enabled = false;
            btnHMRStatus.Enabled = false;
            btnHMRStop.Enabled = false;
            btnHMRContinue.Enabled = false;
            btnHMROpenWindow.Enabled = false;
            cbHMR_NoText.Checked = true;
            myGlobalBase.HMR_HideTextDisplay = false;
            tbHMRLoop.Enabled = false;
            btnHMRSampleLoop.Enabled = false;
            btnHMR_SampleOnce.Enabled = false;
        }
        #endregion

        #region //==================================================cbHMR_NoText_MouseClick
        private void cbHMR_NoText_MouseClick(object sender, MouseEventArgs e)
        {
            //cbHMR_NoText.Checked = !cbHMR_NoText.Checked;
            myGlobalBase.HMR_HideTextDisplay = cbHMR_NoText.Checked;
        }
        #endregion

        #region //==================================================HMR_Timer_Start
        //==========================================================
        // Purpose  : This code run after Timer1 expired where expected to have board PN, Rev, Serial No and so on. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void HMR_Timer_Start()
        {
            if (HRMtimer == null)
            {
                HRMtimer = new Timer();
                HRMtimer.Elapsed += new System.Timers.ElapsedEventHandler(HMR_Timer_Event);
            }
            HRMtimer.Interval = 5;                 // 5mSec sequencer
            HRMtimer.AutoReset = true;
            HRMtimer.Start();
        }
        #endregion

        #region //==================================================HMR_Timer_Stop
        //==========================================================
        // Purpose  : This code run after Timer1 expired where expected to have board PN, Rev, Serial No and so on. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void HMR_Timer_Stop()
        {
            if (HRMtimer != null)
            {
                HRMtimer.AutoReset = false;
                HRMtimer.Elapsed -= HMR_Timer_Event;  // Unsubscribe
                HRMtimer = null;
            }
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].MessTempRX = "";
        }
        #endregion

        #region //==================================================HMR_Timer_Event
        //==========================================================
        // Purpose  : This code run after Timer1 expired where expected to have board PN, Rev, Serial No and so on. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        //_eConfigPortMode eConfigPortMode;
        private void HMR_Timer_Event(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isSerial_Server_Connected == false)
                return;
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isEndOfLine)
            {
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isEndOfLine = false;
                if (HMRrg.IsMatch(myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].MessTempRX))        // Is this data contain no ASCII, just number, comma, sign (this is magnetometer data stream)
                {
                    if (myGlobalBase.TV_isHMR2300Open == true)
                    {
                        myHRM2300.HMR_Update_Readout(myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].MessTempRX);    // Transfer data to display window 
                    }
                    if (myGlobalBase.Svy_isSurveyMode == true)
                    {
                        HMR_Cease_Operation();
                        //myRtbTermMessageLF("-INFO: HMR2300 Received Message, halting HMR2300 operation");
                        mySurvey.Svy_SurveyCVS_HMR_RecievedData(myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].MessTempRX);  // Jump to survey Window for data readout.
                    }
                }
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].MessTempRX = "";
            }
        }
        #endregion

        //----------------------------Timer for Loop Read data. 

        #region //==================================================btnHMRSampleLoop_Click
        private void btnHMRSampleLoop_Click(object sender, EventArgs e)
        {
            HMR_TimerLoop_Start();
            tbHMRLoop.Enabled = false;
            btnHMRScanPort.Enabled = false;
            btnHMRClosePort.Enabled = true;
            btnHMRStatus.Enabled = false;
            btnHMRStop.Enabled = true;
            btnHMRContinue.Enabled = false;
            btnHMROpenWindow.Enabled = true;               // ex false.
            btnHMRSampleLoop.Enabled = false;
            btnHMR_SampleOnce.Enabled = false;
        }
        #endregion

        #region //==================================================HMR_TimerLoop_Start
        //==========================================================
        // Purpose  : This code run after Timer1 expired where expected to have board PN, Rev, Serial No and so on. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void HMR_TimerLoop_Start()
        {
            if (HRMtimerLoop == null)
            {
                HRMtimerLoop = new Timer();
                HRMtimerLoop.Elapsed += new System.Timers.ElapsedEventHandler(HMR_TimerLoop_Event);
            }
            double mSecPeriod = Tools.ConversionStringtoDouble(tbHMRLoop.Text);
            if (mSecPeriod <= 100)
            {
                mSecPeriod = 100;
                tbHMRLoop.Text = "100";
            }
            HRMtimerLoop.Interval = mSecPeriod;
            HRMtimerLoop.AutoReset = true;
            HRMtimerLoop.Start();
            myRtbTermMessageLF("HMR2300 Sample Timer Loop");
        }
        #endregion

        #region //==================================================HMR_TimerLoop_Stop
        //==========================================================
        // Purpose  : This code run after Timer1 expired where expected to have board PN, Rev, Serial No and so on. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void HMR_TimerLoop_Stop()
        {
            if (HRMtimerLoop == null)
                return;
            HRMtimerLoop.AutoReset = false;
            HRMtimerLoop.Elapsed -= HMR_TimerLoop_Event;  // Unsubscribe
            HRMtimerLoop = null;
        }
        #endregion

        #region //==================================================HMR_TimerLoop_Event
        //==========================================================
        // Purpose  : 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        //_eConfigPortMode eConfigPortMode;
        private void HMR_TimerLoop_Event(object sender, System.Timers.ElapsedEventArgs e)
        {
            myGlobalBase.HMR_HideTextDisplay = cbHMR_NoText.Checked;
            myUSB_VCOM_Comm.VCOMArray_Message_Send("*99P\r", (int)eUSBDeviceType.HMR2300);
            Thread.Sleep(10);
        }
        #endregion

        //----------------------------

        #region //==================================================btnHMRScanPort_Click
        private void btnHMRScanPort_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isSerial_Server_Connected == true)
            {
                rtbTerm.AppendText("#E: The port was left open, please close!\n");
                return;
            }
            myGlobalBase.Svy_isSurveyMode = false;
            //-----------------------------------SCAN for PORT
            myUSBComPortManager.ScanPortNowArray((int)eUSBDeviceType.HMR2300, false);
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isSerial_Server_Connected == false)
                myUSBComPortManager.ShowDialog();       // Better to use ShowDialogue in case not able to select device.
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isSerial_Server_Connected == true)
            {
                cbHMR_9600.Enabled = false;
                cbHMR_19200.Enabled = false;
                btnHMRScanPort.Enabled = false;
                btnHMRClosePort.Enabled = true;
                btnHMRStatus.Enabled = true;
                btnHMRStop.Enabled = true;
                btnHMRContinue.Enabled = true;
                btnHMROpenWindow.Enabled = true;
                tbHMRLoop.Enabled = true;
                btnHMRSampleLoop.Enabled = true;
                btnHMR_SampleOnce.Enabled = true;
                btnHMRGetID.Enabled = true;
                btnHMREscape.Enabled = true;
                HMR_Timer_Start();                  // Used for receiving message.
            }
            rtbTerm.AppendText("-I: The USB Array Select Manager set to HMR2300.\n-I: You can use *99P<CR>, etc command, it take care of \\r\n");
            //---------------------- Must override other array based connection (depends on manufcature code select). 
            myGlobalBase.USBArray_RedirectDebugTerm = (int)eUSBDeviceType.HMR2300;
            mySurvey.dgv_DeviceStatus_Update();
            //             char[] chars = new char[4];
            //             chars[0] = (char)27;    // 
            //             chars[1] = '\r';        // 

            string sTX = "\x1B\r";
            myUSB_VCOM_Comm.VCOMserialPortArray[(int)eUSBDeviceType.HMR2300].Write(sTX);
        }
        #endregion

        #region //==================================================btnHMRClosePort_Click
        private void btnHMRClosePort_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isSerial_Server_Connected == false)
            {
                rtbTerm.AppendText("###ERR: What HMR2300 COM/FTDI Port?, there nothing to close!\n");
            }
            else
            {
                HMR_Timer_Stop();
                rtbTerm.AppendText("-INFO: Closing HMR2300 USB/UART Port, please wait few seconds until finally closed\n");
                myUSB_VCOM_Comm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.HMR2300);
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isSerial_Server_Connected = false;
                myGlobalBase.USBArray_RedirectDebugTerm = (int)eUSBDeviceType.Generic;
                cbHMR_9600.Enabled = true;
                cbHMR_19200.Enabled = true;
                btnHMRScanPort.Enabled = true;
                btnHMRClosePort.Enabled = false;
                btnHMRStatus.Enabled = false;
                btnHMRStop.Enabled = false;
                btnHMRContinue.Enabled = false;
                btnHMROpenWindow.Enabled = false;
                btnHMR_SampleOnce.Enabled = false;
                btnHMRSampleLoop.Enabled = false;
                btnHMRGetID.Enabled = false;
                btnHMREscape.Enabled = false;

            }
        }
        #endregion

        #region //==================================================btnHMRStatus_Click
        private void btnHMRStatus_Click(object sender, EventArgs e)
        {
            myGlobalBase.HMR_HideTextDisplay = false;
            myUSB_VCOM_Comm.VCOMArray_Message_Send("*99Q\r", (int)eUSBDeviceType.HMR2300);
            myRtbTermMessageLF("HMR2300 Query Setup");
            myGlobalBase.HMR_HideTextDisplay = cbHMR_NoText.Checked;
        }
        #endregion

        #region //==================================================btnHMRStop_Click
        private void btnHMRStop_Click(object sender, EventArgs e)
        {
            HMR_Cease_Operation();
        }
        #endregion

        #region //==================================================HMR_Cease_Operation
        private void HMR_Cease_Operation()
        {
            myGlobalBase.HMR_HideTextDisplay = false;
            myUSB_VCOM_Comm.VCOMArray_Message_Send("\x1B\r", (int)eUSBDeviceType.HMR2300);
            //myRtbTermMessageLF("HMR2300 Halted");
            HMR_TimerLoop_Stop();
            myGlobalBase.HMR_HideTextDisplay = cbHMR_NoText.Checked;
            tbHMRLoop.Enabled = true;
            btnHMRScanPort.Enabled = false;
            btnHMRClosePort.Enabled = true;
            btnHMRStatus.Enabled = true;
            btnHMRStop.Enabled = true;
            btnHMRContinue.Enabled = true;
            btnHMROpenWindow.Enabled = true;
            btnHMRSampleLoop.Enabled = true;
            btnHMR_SampleOnce.Enabled = true;
            btnHMRGetID.Enabled = true;
            btnHMREscape.Enabled = true;
        }
        #endregion

        #region //==================================================BtnHMRGetID_Click
        private void BtnHMRGetID_Click(object sender, EventArgs e)
        {
            // Below is the low level approach to insert command to read ID. .
            string sTX = "*99ID\r";
            myUSB_VCOM_Comm.VCOMserialPortArray[(int)eUSBDeviceType.HMR2300].Write(sTX);
            myRtbTermMessageLF("HMR2300: ID command");
        }
        #endregion

        #region //==================================================BtnHMREscape_Click
        private void BtnHMREscape_Click(object sender, EventArgs e)
        {
            // Below is the low level approach to insert escape which cease looping if run by continous mode.
            string sTX = "\x1B\r";
            myUSB_VCOM_Comm.VCOMserialPortArray[(int)eUSBDeviceType.HMR2300].Write(sTX);
            myRtbTermMessageLF("HMR2300: Inserted Escape to cease looping");
        }
        #endregion

        #region //==================================================btnSampleRate_Click (Not used, keep for reference)
        /*
		private void btnSampleRate_Click(object sender, EventArgs e)
		{
			myGlobalBase.HMR_HideTextDisplay = false;
			if (myHRM2300.SampleRate==20)
			{
				myHRM2300.SampleRate = 10;
				myUSB_VCOM_Comm.VCOMArray_Message_Send("*99WE *99R=10\r", (int)eUSBDeviceType.HMR2300);
				myRtbTermMessageLF("HMR2300 Sample Rate = 10 Sample/Sec");
			}
			else
			{
				myHRM2300.SampleRate = 20;
				myUSB_VCOM_Comm.VCOMArray_Message_Send("*99WE *99R=20\r", (int)eUSBDeviceType.HMR2300);
				myRtbTermMessageLF("HMR2300 Sample Rate=20 Sample/Sec");
			}
			myGlobalBase.HMR_HideTextDisplay = cbHMR_NoText.Checked;

		}
		*/
        #endregion

        #region //==================================================btnHMRContinue_Click
        private void btnHMRContinue_Click(object sender, EventArgs e)
        {
            myGlobalBase.HMR_HideTextDisplay = false;
            myUSB_VCOM_Comm.VCOMArray_Message_Send("*99C\r", (int)eUSBDeviceType.HMR2300);
            myRtbTermMessageLF("HMR2300 Continued at 10/20 sps");
            myGlobalBase.HMR_HideTextDisplay = cbHMR_NoText.Checked;
        }
        #endregion

        #region //==================================================btnHMR_SampleOnce_Click
        private void btnHMR_SampleOnce_Click(object sender, EventArgs e)
        {
            myGlobalBase.HMR_HideTextDisplay = false;
            myUSB_VCOM_Comm.VCOMArray_Message_Send("*99P\r", (int)eUSBDeviceType.HMR2300);
            myRtbTermMessageLF("HMR2300 Sample Once.");
            myGlobalBase.HMR_HideTextDisplay = cbHMR_NoText.Checked;
        }
        #endregion

        #region //==================================================btnHMROpenWindow_Click
        private void btnHMROpenWindow_Click(object sender, EventArgs e)
        {
            myHRM2300.HMR2300_Show();
            btnHMROpenWindow.Enabled = false;
        }
        #endregion

        #region //==================================================cbHMR_9600_MouseClick
        private void cbHMR_9600_MouseClick(object sender, MouseEventArgs e)
        {
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].BaudRate = 9600;
            cbHMR_19200.Checked = false;
            cbHMR_9600.Checked = true;
        }
        #endregion

        #region //==================================================cbHMR_19200_MouseClick
        private void cbHMR_19200_MouseClick(object sender, MouseEventArgs e)
        {
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].BaudRate = 19200;
            cbHMR_19200.Checked = true;
            cbHMR_9600.Checked = false;

        }
        #endregion

        #region //==================================================btnHMRTestSample_Click
        private void btnHMRTestSample_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.Svy_isSurveyCVSOpen == false)
            {
                mySurvey.Svy_Show(false);

            }
            myGlobalBase.Svy_isSurveyMode = true;
            if (myHRM2300.HMR2300Test_Pointer >= myHRM2300.HMR2300Test.Length)
                myHRM2300.HMR2300Test_Pointer = 0;
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].MessTempRX = myHRM2300.HMR2300Test[myHRM2300.HMR2300Test_Pointer];
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].MessTempRX = myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].MessTempRX.Replace("\n", "");
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].MessTempRX = myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].MessTempRX.Replace("\r", "");
            myHRM2300.HMR2300Test_Pointer++;
            if (myGlobalBase.TV_isHMR2300Open == true)
            {

                myHRM2300.HMR_Update_Readout(myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].MessTempRX);    // Transfer data to display window 
            }
            if (myGlobalBase.Svy_isSurveyMode == true)
            {
                myRtbTermMessageLF("-INFO: Test Data Mode, sent test data");
                mySurvey.Svy_AddNewRow();
                mySurvey.Svy_SurveyCVS_HMR_RecievedData(myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].MessTempRX);  // Jump to survey Window for data readout.
            }
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].MessTempRX = "";
        }
        #endregion



        //##############################################################################################################
        //=============================================================================================Survey (HMR2300)
        //##############################################################################################################

        #region //==================================================btnSurvey_HMRConnect_Click
        private void btnSurvey_HMRConnect_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isSerial_Server_Connected == true)
            {
                rtbTerm.AppendText("###ERR: The port was left open, please close!\n");
                return;
            }
            myGlobalBase.Svy_isSurveyMode = true;
            //-----------------------------------SCAN for PORT
            myUSBComPortManager.ScanPortNowArray((int)eUSBDeviceType.HMR2300, false);
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isSerial_Server_Connected == false)
                myUSBComPortManager.ShowDialog();       // Better to use ShowDialogue in case not able to select device.
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isSerial_Server_Connected == true)
            {
                cbHMR_9600.Enabled = false;
                cbHMR_19200.Enabled = false;
                btnHMRScanPort.Enabled = false;
                btnHMRClosePort.Enabled = true;
                btnHMRStatus.Enabled = true;
                btnHMRStop.Enabled = true;
                btnHMRContinue.Enabled = true;
                btnHMROpenWindow.Enabled = true;
                tbHMRLoop.Enabled = true;
                btnHMRSampleLoop.Enabled = true;
                btnHMR_SampleOnce.Enabled = true;
                btnHMRGetID.Enabled = true;
                btnHMREscape.Enabled = true;
                myGlobalBase.is_SvyHMRSerial_Server_Connected = true;
                HMR_Timer_Start();              // Used for receiving message.
            }
            mySurvey.dgv_DeviceStatus_Update();
        }
        #endregion

        #region //==================================================btnSurvey_HMRClose_Click
        private void btnSurvey_HMRClose_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isSerial_Server_Connected == false)
            {
                rtbTerm.AppendText("###ERR: What HMR2300 COM/FTDI Port?, there nothing to close!\n");
            }
            else
            {
                HMR_Timer_Stop();
                rtbTerm.AppendText("-INFO: Closing HMR2300 USB/UART Port, please wait few seconds until finally closed\n");
                myUSB_VCOM_Comm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.HMR2300);
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.HMR2300].isSerial_Server_Connected = false;
                cbHMR_9600.Enabled = true;
                cbHMR_19200.Enabled = true;
                btnHMRScanPort.Enabled = true;
                btnHMRClosePort.Enabled = false;
                btnHMRStatus.Enabled = false;
                btnHMRStop.Enabled = false;
                btnHMRContinue.Enabled = false;
                btnHMROpenWindow.Enabled = false;
                btnHMR_SampleOnce.Enabled = false;
                btnHMRSampleLoop.Enabled = false;
                btnHMRGetID.Enabled = false;
                btnHMREscape.Enabled = false;
                myGlobalBase.is_SvyHMRSerial_Server_Connected = false;
                mySurvey.HMRClosePort();
            }
            mySurvey.dgv_DeviceStatus_Update();
        }
        #endregion

        #region //==================================================btnSurvey_Open_Click
        private void btnSurvey_Open_Click(object sender, EventArgs e)
        {
            mySurvey.Svy_Show(false);

        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Survey/Directional (FGIIA)
        //##############################################################################################################

        #region //==================================================btnSurvey_FGIIAConnect_Click
        private void btnSurvey_FGIIAConnect_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isSerial_Server_Connected == true)
            {
                rtbTerm.AppendText("###ERR: The port was left open, please close!\n");
                return;
            }
            myGlobalBase.Svy_isSurveyMode = true;
            //-----------------------------------SCAN for PORT
            myUSBComPortManager.ScanPortNowArray((int)eUSBDeviceType.FGII_A, false);
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isSerial_Server_Connected == false)
                myUSBComPortManager.ShowDialog();       // Better to use ShowDialogue in case not able to select device.
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isSerial_Server_Connected == true)
            {
                btnFGIIScanPort.Enabled = false;
                btnFGIIClosePort.Enabled = true;
                btnFGIIStop.Enabled = true;
                btnFGIIOpenWindow.Enabled = true;
                tbFGIILoop.Enabled = true;
                btnFGIISampleLoop.Enabled = true;
                btnHMR_SampleOnce.Enabled = true;
                myGlobalBase.is_SvyFGIISerial_Server_Connected = true;
                FGII_Timer_Start();
            }
            mySurvey.dgv_DeviceStatus_Update();
        }
        #endregion

        #region //==================================================btnSurvey_FGIIAClose_Click
        private void btnSurvey_FGIIAClose_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isSerial_Server_Connected == false)
            {
                rtbTerm.AppendText("###ERR: What FGIIA COM/FTDI Port?, there nothing to close!\n");
            }
            else
            {
                FGII_Timer_Stop();
                rtbTerm.AppendText("-INFO: Closing FGIIA USB/UART Port, please wait few seconds until finally closed\n");
                myUSB_VCOM_Comm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.FGII_A);
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isSerial_Server_Connected = false;
                btnFGIIScanPort.Enabled = true;
                btnFGIIClosePort.Enabled = false;
                btnFGIIStop.Enabled = false;
                btnFGIIOpenWindow.Enabled = false;
                btnFGII_SampleOnce.Enabled = false;
                btnFGIISampleLoop.Enabled = false;
                myGlobalBase.is_SvyFGIISerial_Server_Connected = false;
                mySurvey.FGIIClosePort();
            }
            mySurvey.dgv_DeviceStatus_Update();
        }
        #endregion

        #region //==================================================FGIIA_Init
        private void FGIIA_Init()
        {
            btnFGIIScanPort.Enabled = true;
            btnFGIIClosePort.Enabled = false;
            btnFGIIStop.Enabled = false;
            btnFGIIOpenWindow.Enabled = false;
            cbFGII_NoText.Checked = true;
            tbFGIILoop.Enabled = false;
            btnFGIISampleLoop.Enabled = false;
            btnFGII_SampleOnce.Enabled = false;
            myGlobalBase.FGII_HideTextDisplay = false;
        }
        #endregion

        #region //==================================================cbFGII_NoText_MouseClick
        private void cbFGII_NoText_MouseClick(object sender, MouseEventArgs e)
        {
            myGlobalBase.FGII_HideTextDisplay = cbHMR_NoText.Checked;
        }
        #endregion

        //--------------------------- Timer for Read FGII message.

        #region //==================================================FGII_Timer_Start
        //==========================================================
        // Purpose  : This code run after Timer1 expired where expected to have board PN, Rev, Serial No and so on. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void FGII_Timer_Start()
        {
            if (FGIItimer == null)
            {
                FGIItimer = new Timer();
                FGIItimer.Elapsed += new System.Timers.ElapsedEventHandler(FGII_Timer_Event);
            }
            FGIItimer.Interval = 5;                 // 5mSec sequencer
            FGIItimer.AutoReset = true;
            FGIItimer.Start();
        }
        #endregion

        #region //==================================================FGII_Timer_Stop
        //==========================================================
        // Purpose  : This code run after Timer1 expired where expected to have board PN, Rev, Serial No and so on. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void FGII_Timer_Stop()
        {
            if (FGIItimer != null)
            {
                FGIItimer.AutoReset = false;
                FGIItimer.Elapsed -= FGII_Timer_Event;  // Unsubscribe
                FGIItimer = null;
            }
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].MessTempRX = "";
        }
        #endregion

        #region //==================================================FGII_Timer_Event
        //==========================================================
        // Purpose  : This code run after Timer1 expired where expected to have board PN, Rev, Serial No and so on. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        //_eConfigPortMode eConfigPortMode;
        private void FGII_Timer_Event(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isSerial_Server_Connected == false)
                return;
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isEndOfLine)
            {
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isEndOfLine = false;
                if (FGIIrg.IsMatch(myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].MessTempRX))         // Is this data contain no ASCII, just number, comma, sign (this is magnetometer data stream)
                {
                    if (myGlobalBase.TV_isFGIIOpen == true)
                    {
                        myFGII.FGII_Update_Readout(myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].MessTempRX);    // Transfer data to display window 
                    }
                    if ((myGlobalBase.Svy_isSurveyMode == true) & (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isSerial_Server_Connected == true))
                    {
                        FGII_Cease_Operation();
                        //myRtbTermMessageLF("-INFO: FGIIA Received Message, halting FGIIA operation");
                        mySurvey.Svy_SurveyCVS_FGII_RecievedData(myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].MessTempRX);  // Jump to survey Window for data readout.
                    }
                }
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].MessTempRX = "";
            }
        }
        #endregion

        //----------------------------Timer for Loop Read data. 

        #region //==================================================btnFGIISampleLoop_Click
        private void btnFGIISampleLoop_Click(object sender, EventArgs e)
        {
            FGII_TimerLoop_Start();
            tbFGIILoop.Enabled = false;
            btnFGIIScanPort.Enabled = false;
            btnFGIIClosePort.Enabled = true;
            btnFGIIStop.Enabled = true;
            btnFGIIOpenWindow.Enabled = true;               // ex false.
            btnFGIISampleLoop.Enabled = false;
            btnFGII_SampleOnce.Enabled = false;
        }
        #endregion

        #region //==================================================FGII_TimerLoop_Start
        //==========================================================
        // Purpose  : This code run after Timer1 expired where expected to have board PN, Rev, Serial No and so on. 
        // Input    :  
        // Output   : 
        // Status   : Timer FGIItimerLoop;         
        //==========================================================
        private void FGII_TimerLoop_Start()
        {
            if (FGIItimerLoop == null)
            {
                FGIItimerLoop = new Timer();
                FGIItimerLoop.Elapsed += new System.Timers.ElapsedEventHandler(FGII_TimerLoop_Event);
            }
            double mSecPeriod = Tools.ConversionStringtoDouble(tbFGIILoop.Text);
            if (mSecPeriod <= 100)
            {
                mSecPeriod = 100;
                tbFGIILoop.Text = "100";
            }
            FGIItimerLoop.Interval = mSecPeriod;
            FGIItimerLoop.AutoReset = true;
            FGIItimerLoop.Start();
            myRtbTermMessageLF("FluxGateII Sample Timer Loop");
        }
        #endregion

        #region //==================================================FGII_TimerLoop_Stop
        //==========================================================
        // Purpose  : This code run after Timer1 expired where expected to have board PN, Rev, Serial No and so on. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void FGII_TimerLoop_Stop()
        {
            if (FGIItimerLoop == null)
                return;
            FGIItimerLoop.AutoReset = false;
            FGIItimerLoop.Elapsed -= FGII_TimerLoop_Event;  // Unsubscribe
            FGIItimerLoop = null;
        }
        #endregion

        #region //==================================================FGII_TimerLoop_Event
        //==========================================================
        // Purpose  : 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void FGII_TimerLoop_Event(object sender, System.Timers.ElapsedEventArgs e)
        {
            myGlobalBase.FGII_HideTextDisplay = cbFGII_NoText.Checked;
            myUSB_VCOM_Comm.VCOMArray_Message_Send("*99P\r", (int)eUSBDeviceType.FGII_A);         // Check Code in Fluxgate Module. 
            Thread.Sleep(10);
        }
        #endregion

        #region //==================================================btnFGIIScanPort_Click
        private void btnFGIIScanPort_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isSerial_Server_Connected == true)
            {
                rtbTerm.AppendText("###ERR: The port was left open, please close!\n");
                return;
            }
            myGlobalBase.Svy_isSurveyMode = false;
            //-----------------------------------SCAN for PORT
            myUSBComPortManager.ScanPortNowArray((int)eUSBDeviceType.FGII_A, false);
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isSerial_Server_Connected == false)
                myUSBComPortManager.ShowDialog();       // Better to use ShowDialogue in case not able to select device.
            else
            {
                btnFGIIScanPort.Enabled = false;
                btnFGIIClosePort.Enabled = true;
                btnFGIIStop.Enabled = true;
                btnFGIIOpenWindow.Enabled = true;
                tbFGIILoop.Enabled = true;
                btnFGIISampleLoop.Enabled = true;
                btnFGII_SampleOnce.Enabled = true;
                FGII_Timer_Start();
            }
        }
        #endregion

        #region //==================================================btnFGIIClosePort_Click
        private void btnFGIIClosePort_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isSerial_Server_Connected == false)
            {
                rtbTerm.AppendText("###ERR: What FGIIA COM/FTDI Port?, there nothing to close!\n");
            }
            else
            {
                FGII_Timer_Stop();
                rtbTerm.AppendText("-INFO: Closing FGIIA USB/UART Port, please wait few seconds until finally closed\n");
                myUSB_VCOM_Comm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.FGII_A);
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].isSerial_Server_Connected = false;
                btnFGIIScanPort.Enabled = true;
                btnFGIIClosePort.Enabled = false;
                btnFGIIStop.Enabled = false;
                btnFGIIOpenWindow.Enabled = false;
                btnFGII_SampleOnce.Enabled = false;
                btnFGIISampleLoop.Enabled = false;
            }
        }
        #endregion

        #region //==================================================btnFGII_SampleOnce_Click
        private void btnFGII_SampleOnce_Click(object sender, EventArgs e)
        {
            myGlobalBase.FGII_HideTextDisplay = false;
            myUSB_VCOM_Comm.VCOMArray_Message_Send("*99P\r", (int)eUSBDeviceType.FGII_A);
            myRtbTermMessageLF("FGIIA Sample Once.");
            myGlobalBase.FGII_HideTextDisplay = cbHMR_NoText.Checked;
        }
        #endregion

        #region //==================================================btnFGIIOpenWindow_Click
        private void btnFGIIOpenWindow_Click(object sender, EventArgs e)
        {
            myFGII.FGII_Show();
            btnFGIIOpenWindow.Enabled = false;
        }
        #endregion

        #region //==================================================FGII_Cease_Operation
        private void FGII_Cease_Operation()
        {
            myGlobalBase.FGII_HideTextDisplay = false;
            //myRtbTermMessageLF("FluxGate-II Halted");
            FGII_TimerLoop_Stop();
            myGlobalBase.FGII_HideTextDisplay = cbFGII_NoText.Checked;
            tbFGIILoop.Enabled = true;
            btnFGIIScanPort.Enabled = false;
            btnFGIIClosePort.Enabled = true;
            btnFGIIStop.Enabled = true;
            btnFGIIOpenWindow.Enabled = true;
            btnFGIISampleLoop.Enabled = true;
            btnFGII_SampleOnce.Enabled = true;
        }
        #endregion

        #region //==================================================btnFGIIOpenWindow_Click
        private void btnFGIIStop_Click(object sender, EventArgs e)
        {
            FGII_Cease_Operation();
        }
        #endregion

        #region //==================================================btnFGIITestSample_Click
        private void btnFGIITestSample_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.Svy_isSurveyCVSOpen == false)
            {
                mySurvey.Svy_Show(false);
            }
            myGlobalBase.Svy_isSurveyMode = true;
            if (myFGII.FGIITest_Pointer >= myFGII.FGIITest.Length)
                myFGII.FGIITest_Pointer = 0;
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].MessTempRX = myHRM2300.HMR2300Test[myFGII.FGIITest_Pointer];
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].MessTempRX = myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].MessTempRX.Replace("\n", "");
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].MessTempRX = myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].MessTempRX.Replace("\r", "");
            myFGII.FGIITest_Pointer++;
            if (myGlobalBase.TV_isFGIIOpen == true)
            {
                myFGII.FGII_Update_Readout(myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].MessTempRX);    // Transfer data to display window 
            }
            if (myGlobalBase.Svy_isSurveyMode == true)
            {
                myRtbTermMessageLF("-INFO: FGII Test Data Mode, sent test data");
                mySurvey.Svy_SurveyCVS_FGII_RecievedData(myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].MessTempRX);  // Jump to survey Window for data readout.
            }
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.FGII_A].MessTempRX = "";

        }
        #endregion

        //##############################################################################################################
        //=============================================================================================ADIS16460
        //##############################################################################################################

        #region //==================================================btn_ADIS_USBControl_SetMode
        private void btn_ADIS_USBControl_SetMode(int Mode)
        {
            switch (Mode)
            {
                case (1):       // All Generic and ADIS16460 USB control enabled.
                    {
                        btnADIS_Connect.Enabled = true;
                        btnADIS_Close_Port.Enabled = true;
                        btnClosePort.Enabled = true;
                        btn_ConnectSerial.Enabled = true;
                        cbBaudRateSelect.Enabled = true;
                        cbVCOM.Enabled = true;
                        cbFT232RL.Enabled = true;
                        cbCOMList.Enabled = true;
                        break;
                    }
                case (2):       // USB connected, disable connects
                    {
                        btnADIS_Connect.Enabled = false;
                        btnADIS_Close_Port.Enabled = true;
                        btnClosePort.Enabled = false;
                        btn_ConnectSerial.Enabled = false;
                        cbBaudRateSelect.Enabled = false;
                        cbVCOM.Enabled = false;
                        cbFT232RL.Enabled = false;
                        cbCOMList.Enabled = false;
                        break;
                    }
                case (3):       // USB disconnected, enable connects
                    {
                        btnADIS_Connect.Enabled = true;
                        btnADIS_Close_Port.Enabled = false;
                        btnClosePort.Enabled = false;
                        btn_ConnectSerial.Enabled = true;
                        cbBaudRateSelect.Enabled = true;
                        cbVCOM.Enabled = true;
                        cbFT232RL.Enabled = true;
                        cbCOMList.Enabled = true;
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region //==================================================btnADIS_Connect_Click
        private void btnADIS_Connect_Click(object sender, EventArgs e)
        {
            // Add code to make connecton to IDT MINI device on selected USB channel.
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == true)
            {
                rtbTerm.AppendText("###################################################################################\n");
                rtbTerm.AppendText("#E: The Direct VCOM port for Mini ADIS16460 was left open. Auto Close and Try again\n");
                rtbTerm.AppendText("###################################################################################\n");
                myUSB_VCOM_Comm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.ADIS);
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected = false;
                btn_ADIS_USBControl_SetMode(1); //enable all connect control.
                return;
            }
            btn_ADIS_USBControl_SetMode(2);     // Preasume connected.
            myGlobalBase.Svy_isSurveyMode = false;
            myGlobalBase.VCOM_IsChange = false;
            //-----------------------------------SCAN for PORT
            myUSBComPortManager.ScanPortNowArray((int)eUSBDeviceType.ADIS, false);
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == false)
            {
                try
                {
                    myUSBComPortManager.ShowDialog();       // Better to use ShowDialogue in case not able to select device.
                }
                catch (System.Exception ex)
                {
                    rtbTerm.AppendText("#E: Internal Code Catch Bug: myUSBComPortManager.ShowDialog() within btnADIS_Connect_Click(), try fix this\n" + ex.ToString());
                    btn_ADIS_USBControl_SetMode(1);
                }
            }
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == false)
            {
                btn_ADIS_USBControl_SetMode(1); //enable all connect control.
                return;     // Failed connection. 
            }
            rtbTerm.Focus();
            this.Focus();
            rtbTerm.AppendText("-I: VCOM Connected!\n");
            //-----------------------------------
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isDMFProtocolEnabled = true;      // Enable DMFP Protocol operation. 

            Tools.InteractivePause(TimeSpan.FromSeconds(2));
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ADIS_Connect = new DMFP_Delegate(DMFP_ADIS_Connect_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+ADISCON()";
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t1 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_ADIS_Connect, sMessage, 250, (int)eUSBDeviceType.ADIS));
            t1.Start();
            myDMFProtocol.DMFProtocol_UpdateList();

        }
        #endregion

        #region //==================================================DMFP_ADIS_Connect_CallBack  
        public void DMFP_ADIS_Connect_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Status = hPara[0];
            if (Status == 0xF)
            {
                myRtbTermMessageLF("-I:ADIS16460 Connection is Activated !!");
                myGlobalBase.is_ADIS16460_Device_Activated = true;
            }
            else
            {
                myRtbTermMessageLF("#E:ADIS16460 Connection Failure!!");
                myGlobalBase.is_ADIS16460_Device_Activated = false;
                btn_ADIS_USBControl_SetMode(1);
            }
        }
        #endregion

        #region //==================================================btnADIS_TurnOFF_Click
        private void btnADIS_TurnOFF_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == false)
            {
                rtbTerm.AppendText("#E: The ADIS VCOM Channel is not open. Try again with Connect Button\n");
                btn_ADIS_USBControl_SetMode(1);
                return;
            }
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ADIS_POFFS = new DMFP_Delegate(DMFP_ADIS_POFFS_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+ADISOFF()";
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t2 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_ADIS_POFFS, sMessage, 250, (int)eUSBDeviceType.ADIS));
            t2.Start();
            myDMFProtocol.DMFProtocol_UpdateList();

        }
        #endregion

        #region //==================================================tnADIS_TurnON_Click
        private void tnADIS_TurnON_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == false)
            {
                rtbTerm.AppendText("#E: The ADIS VCOM Channel is not open. Try again with Connect Button\n");
                btn_ADIS_USBControl_SetMode(1);
                return;
            }
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ADIS_PON = new DMFP_Delegate(DMFP_ADIS_PON_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+ADISON()";
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t2 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_ADIS_PON, sMessage, 250, (int)eUSBDeviceType.ADIS));
            t2.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
        }
        #endregion

        #region //==================================================btnADIS_Close_Port_Click 
        private void btnADIS_Close_Port_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == false)
            {
                rtbTerm.AppendText("#ERR: What ADIS16460 COM/FTDI Port?, there nothing to close!\n");
            }
            else
            {
                rtbTerm.AppendText("-INFO: Closing ADIS16460 USB/UART Port, please wait few seconds until finally closed\n");
                myUSB_VCOM_Comm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.ADIS);
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected = false;
            }
            btn_ADIS_USBControl_SetMode(1);
            cbADIS_LoggerCVS.Enabled = true;                   // Also turn off LoggerCVS Mode
            myGlobalBase.LoggerCVSADIS16460Enable = false;
            btnADIS_CaptureLoop.Enabled = true;
            btnADIS_ReadID.Enabled = true;
            tnADIS_TurnON.Enabled = true;
            btnADISOpenLogger.Enabled = true;
            btnADIS_BurstRead.Enabled = true;
        }
        #endregion

        #region //==================================================DMFP_ADIS_POFFS_CallBack  
        public void DMFP_ADIS_POFFS_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Status = hPara[0];
            if (Status == 0xF)
            {
                myRtbTermMessageLF("-I:ADIS16460 is powered down but VCOM remains connected!!");
                myGlobalBase.is_ADIS16460_Device_Activated = false;
                cbADIS_LoggerCVS.Enabled = true;                   // Also turn off LoggerCVS Mode
                myGlobalBase.LoggerCVSADIS16460Enable = false;
                btnADIS_CaptureLoop.Enabled = true;
                btnADIS_ReadID.Enabled = true;
                tnADIS_TurnON.Enabled = true;
                btnADISOpenLogger.Enabled = true;
                btnADIS_BurstRead.Enabled = true;
            }
            else
            {
                myRtbTermMessageLF("#E:ADIS16460 power down failure, check cable and MINI device!!");
            }
        }
        #endregion

        #region //==================================================DMFP_ADIS_PON_CallBack  
        public void DMFP_ADIS_PON_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Status = hPara[0];
            if (Status == 0xF)
            {
                myRtbTermMessageLF("-I:ADIS16460 is powered up");
                myGlobalBase.is_ADIS16460_Device_Activated = true;
            }
            else
            {
                myRtbTermMessageLF("#E:ADIS16460 power up failure, check cable and MINI device!!");
            }
        }
        #endregion

        #region //==================================================btnADIS_BurstRead_Click
        private void btnADIS_BurstRead_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == false)
            {
                rtbTerm.AppendText("#E: The ADIS VCOM Channel is not open. Try again with Connect Button\n");
                return;
            }
            if (myGlobalBase.is_ADIS16460_Device_Activated == false)
            {
                rtbTerm.AppendText("#E: ADIS is powered down down (VCOM is active). Try again with Connect Button\n");
                return;
            }
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ADIS_BurstRead = new DMFP_Delegate(DMFP_ADIS_BurstRead_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+ADISBR()";
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t3 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_ADIS_BurstRead, sMessage, 250, (int)eUSBDeviceType.ADIS));
            t3.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
        }
        #endregion

        #region //==================================================DMFP_ADIS_BurstRead_CallBack  
        public void DMFP_ADIS_BurstRead_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Status = hPara[0];
            if (Status == 0xFF)
            {
                if (cbADIS_TranslateData.Checked == true)
                {
                    // Translate all data into readouts


                }
            }
            else
            {
                myRtbTermMessageLF("#E:ADIS16460 Burst Read Failure. Check Device and firmware");
            }
        }
        #endregion

        #region //==================================================btnADIS_ReadID_Click
        private void btnADIS_ReadID_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == false)
            {
                rtbTerm.AppendText("#E: The ADIS VCOM Channel is not open. Try again with Connect Button\n");
                return;
            }
            if (myGlobalBase.is_ADIS16460_Device_Activated == false)
            {
                rtbTerm.AppendText("#E: ADIS is powered down down (VCOM is active). Try again with Connect Button\n");
                return;
            }
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ADIS_ReadID = new DMFP_Delegate(DMFP_ADIS_ReadID_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+ADISR(2x5600;2x0000)";
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t3 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_ADIS_ReadID, sMessage, 250, (int)eUSBDeviceType.ADIS));
            t3.Start();
            myDMFProtocol.DMFProtocol_UpdateList();

        }
        #endregion

        #region //==================================================DMFP_ADIS_ReadID_CallBack  
        public void DMFP_ADIS_ReadID_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Status = hPara[0];
            if (Status == 0xFF)
            {
                myRtbTermMessageLF("-I:ADIS16460 Read ID : 0x" + hPara[1].ToString("X"));        // Expect 0x404C
            }
            else
            {
                myRtbTermMessageLF("#E:ADIS16460 power down failure, check cable and MINI device!!");
            }
        }
        #endregion

        #region //==================================================btnADISOpenLogger_Click  
        private void btnADISOpenLogger_Click(object sender, EventArgs e)
        {
            myGlobalBase.LoggerCVSADIS16460Enable = cbADIS_LoggerCVS.Checked;
            if (myGlobalBase.LoggerCVSADIS16460Enable == false)
            {
                MessageBox.Show("LoggerCVS for ADIS16460 is not checked, check this and rerun again",
                "Logger CVS Configuration Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                return;
            }
            //-------------------------------------------
            //+ADISLOG()        // This command activate the LoggerCVS Mode. It assumed the MiniPort and ADIS has been initiaised via connection. 
            //-------------------------------------------

            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == false)
            {
                rtbTerm.AppendText("#E: The ADIS VCOM Channel is not open. Try again with Connect Button\n");
                btn_ADIS_USBControl_SetMode(1);
                return;
            }
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ADIS_ADISLOG = new DMFP_Delegate(DMFP_ADIS_ADISLOG_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+ADISLOG(1)";
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t2 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_ADIS_ADISLOG, sMessage, 250, (int)eUSBDeviceType.ADIS));
            t2.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
        }
        #endregion

        #region //==================================================DMFP_ADIS_ADISLOG_CallBack  
        public void DMFP_ADIS_ADISLOG_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Status = hPara[0];
            if (Status == 0xF)
            {
                cbADIS_LoggerCVS.Enabled = false;
                btnADIS_CaptureLoop.Enabled = false;
                btnADIS_ReadID.Enabled = false;
                tnADIS_TurnON.Enabled = false;
                btnADISOpenLogger.Enabled = false;
                btnADIS_BurstRead.Enabled = false;
                Terminal_Append_Response("===================================Logger Mode\n");
                Terminal_Append_Response("====== DO NOT USE MAIN TERMINAL ! ============\n");
                Terminal_Append_Response("==============================================\n");
                myGlobalBase.LoggerOpertaionMode = true;
                myGlobalBase.LoggerWindowVisable = true;
                myLoggerCSV.Show();
                Thread.Sleep(1);
                myLoggerCSV.LoggerCVS_SetupShow_Window(0, "");
            }
            else
            {
                myRtbTermMessageLF("#E:ADIS16460 ADISLOG() Command Failure, check cable and MINI device!!");
            }
        }
        #endregion

        #region //==================================================btnADIS_CaptureLoop_Click  
        private void btnADIS_CaptureLoop_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == false)
            {
                rtbTerm.AppendText("#E: The ADIS VCOM Channel is not open. Try again with Connect Button\n");
                return;
            }
            if (myGlobalBase.is_ADIS16460_Device_Activated == false)
            {
                rtbTerm.AppendText("#E: ADIS is powered down down (VCOM is active). Try again with Connect Button\n");
                return;
            }
            rtbTerm.AppendText("-I: DEC-RATE set to 1023. Press Space Bar and Return to cease loop (quickly!)\n");
            string sMessage = "ADISLOOP()";
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            myUSB_VCOM_Comm.VCOMArray_Message_Send(sMessage, (int)eUSBDeviceType.ADIS);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Development/Misc
        //##############################################################################################################

        #region //==================================================cbFT232RL_MouseClick
        private void cbFT232RL_MouseClick(object sender, MouseEventArgs e)
        {
            if (cbFT232RL.Checked == true)
            {
                if (cbVCOM.Checked == true)
                    myGlobalBase.VCOMMode = 0;      // Both checked
                else
                    myGlobalBase.VCOMMode = 2;      // VCOM Only
            }
            else
            {
                cbVCOM.Checked = true;           // FT232RL Only since cbVCOOM is unchecked.
                myGlobalBase.VCOMMode = 1;
            }
        }
        #endregion

        #region //==================================================cbVCOM_MouseClick
        private void cbVCOM_MouseClick(object sender, MouseEventArgs e)
        {
            if (cbVCOM.Checked == true)
            {
                if (cbFT232RL.Checked == true)
                    myGlobalBase.VCOMMode = 0;      // Both checked
                else
                    myGlobalBase.VCOMMode = 1;      // VCOM Only
            }
            else
            {
                cbFT232RL.Checked = true;           // FT232RL Only since cbVCOOM is unchecked.
                myGlobalBase.VCOMMode = 2;
            }
        }
        #endregion

        #region //==================================================cbCOMList_MouseCaptureChanged
        private void cbCOMList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = this.cbCOMList.GetItemText(this.cbCOMList.SelectedItem);
            myGlobalBase.VCOMPort = item;
            myGlobalBase.VCOM_IsChange = true;
            if (myGlobalBase.VCOMPort != "SCAN")      // Was is COM1, etc
            {
                cbBaudRateSelect.Enabled = true;
                btn_ConnectSerial.Enabled = true;      // Avoid repeat clicking.
                cbVCOM.Checked = true;
                cbVCOM.Enabled = false;
                cbFT232RL.Checked = false;
                cbFT232RL.Enabled = false;
            }
            else
            {
                cbBaudRateSelect.Enabled = true;
                btn_ConnectSerial.Enabled = true;      // Avoid repeat clicking.
                cbVCOM.Enabled = true;
                cbFT232RL.Enabled = true;
            }
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================EEPROM Section
        //##############################################################################################################


        //##############################################################################################################
        //=============================================================================================USB Plug In and Removed Management 
        //##############################################################################################################
        bool BGSurveyantilooprepeat = false;
        #region //==================================================USB_Device_PlugIn_Detected
        public void USB_Device_PlugIn_Detected()
        {
            if (myGlobalBase.configb[0].bOptionAutoDetectEnable == false)
                return;

            switch (myGlobalBase.MainFeatureSelectedIndex)
            {
                case (0):       // LoggerCVS
                    {
                        break;
                    }
                case (1):       // Survey
                    {
                        break;
                    }
                case (2):       // BG Survey Tab (BG Drilling Project)
                    {
                        if (BGSurveyantilooprepeat == true)                    // Anti loop event by certain USB/UART adapter. 
                            break;
                        BGSurveyantilooprepeat = true;
                        USB_Device_PlugIn_Detected_BGSurveyArray(true);        // BGDRILLING Only
                        BGSurveyButtonSetting(2);
                        break;
                    }
                case (3):       // BG Survey 2nd Tab (BG Drilling Project)
                    {
                        break;
                    }
                case (4):       // HMR2300 Device
                    {
                        break;
                    }
                case (5):       // ADIS16460 Board with Mini ZMDI Device
                    {
                        USB_Device_PlugIn_Detected_ADIS16460Board(true);     // Also Applied to ZMDI Board
                        break;
                    }
                case (6):       // MiniAD7794 Board 
                    {
                        USB_Device_PlugIn_Detected_MiniAD7794(true);        // MiniAD7794 Only
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region //==================================================USB_Device_Removed_Detected
        public void USB_Device_Removed_Detected()
        {
            switch (myGlobalBase.MainFeatureSelectedIndex)
            {
                case (0):
                    {
                        break;
                    }
                case (1):       // Tool Survey Tab
                    {
                        break;
                    }
                case (2):       // BG Survey Tab (BG Drilling Project)
                    {
                        BGSurveyantilooprepeat = false;
                        USB_Device_Removed_Detected_BGSurveyArray();
                        BGSurveyButtonSetting(1);
                        break;
                    }
                case (4):       // HMR2300 Device
                    {
                        break;
                    }
                case (5):       // ADIS16460 Board with Mini ZMDI Device
                    {
                        USB_Device_Removed_Detected_ADIS16460(); // Also Applied to Mini Board
                        break;
                    }
                case (6):       // MiniAD7794 Board 
                    {
                        USB_Device_Removed_Detected_MiniAD7794();        // MiniAD7794 Only
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region //==================================================USB_Device_PlugIn_Detected_BGSurvey
        public delegate void USB_Device_PlugIn_Detected_BGSurvey_StartDelegate(bool ReportErrorIfNotFound);
        private void USB_Device_PlugIn_Detected_BGSurvey(bool ReportErrorIfNotFound)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new USB_Device_PlugIn_Detected_BGSurvey_StartDelegate(USB_Device_PlugIn_Detected_BGSurvey), new object[] { ReportErrorIfNotFound });
                return;
            }

            btnScanAndConnectTool.Enabled = false;
            string VCOMPort = "";

            myUSBComPortManager.ScanPortNow();

            if (myGlobalBase.is_Serial_Server_Connected == true)   // Existing port, so cannot connect unless closed.     
            {
                FlexiMessageBox.COLOR = Color.Red;
                FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                FlexiMessageBox.Show("There is active COM port!. Unable to connects!"
                + Environment.NewLine + "Please close port (ie remove USB/UART device) and try again",
                "Active Port Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
                btnScanAndConnectTool.Enabled = true;
                btnScanAndConnectTool.Text = "Scan and Connect Tools";
                return;
            }
            //-----------------------------------------------------------------
            myGlobalBase.Select_Baudrate = 6;       // Select 115200 Baud Rate. 
            myGlobalBase.VCOM_IsChange = false;
            myGlobalBase.VCOMMode = 1;              //VCOM Only. 
                                                    //-------------------------Disable various control.
            btnScanAndConnectTool.Enabled = false;
            cbBaudRateSelect.Enabled = false;
            btn_ConnectSerial.Enabled = false;      // Avoid repeat clicking.
            cbVCOM.Enabled = false;
            cbFT232RL.Enabled = false;
            cbCOMList.Enabled = false;              // Keep it disabled until Com Port is stopped.
                                                    //-----------------------------------
            Thread.Sleep(250);                      // Short Pause to allow Device to complete power on process. 
            VCOMPort = myUSBComPortManager.ScanVcomNowBGPING();      // Do inital scan

            if (VCOMPort == "")       // Not found
            {
                if (ReportErrorIfNotFound == true)
                {
                    FlexiMessageBox.COLOR = Color.Red;
                    FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    FlexiMessageBox.Show("Unable to Detect BG Tools!"
                    + Environment.NewLine + "(1) Check/Replug Lemo Serial Cable."
                    + Environment.NewLine + "(2) Reset Power and Perhaps reboot Computer."
                    + Environment.NewLine + "(3) Try manual connection (refer to manual)",
                    "Unable to auto connect BG Survey Tool",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                }
                else
                {
                    myRtbTermMessageLF("#E:BG Tool Not Detected <Failed response from BGPING()>");
                }
                cbBaudRateSelect.Enabled = true;
                btn_ConnectSerial.Enabled = true;      // Avoid repeat clicking.
                cbVCOM.Enabled = true;
                cbFT232RL.Enabled = true;
                cbCOMList.Enabled = true;              // Keep it disabled until Com Port is stopped.
                btnScanAndConnectTool.Enabled = true;
                btnScanAndConnectTool.Text = "Scan and Connect Tools";
            }
            else
            {
                btnScanAndConnectTool.Text = "Close Port";
                btnScanAndConnectTool.Enabled = true;
            }
        }
        #endregion

        #region //==================================================USB_Device_Removed_Detected_BGSurvey
        public delegate void USB_Device_Removed_Detected_BGSurvey_StartDelegate();
        private void USB_Device_Removed_Detected_BGSurvey()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new USB_Device_Removed_Detected_BGSurvey_StartDelegate(USB_Device_Removed_Detected_BGSurvey), null);
                return;
            }

            myUSB_VCOM_Comm.VCOM_Message_ClosePort();
            cbBaudRateSelect.Enabled = true;
            btn_ConnectSerial.Enabled = true;      // Avoid repeat clicking.;
            if (myGlobalBase.is_Serial_Server_Connected == false)
            {
                myRtbTermMessageLF("#E: What COM/FTDI Port?, there nothing to close!");
            }
            else
            {
                myRtbTermMessageLF("-I: Closing USB/UART Port, please wait few seconds until finally closed");
                btnClosePort.Enabled = false;
                myUSB_VCOM_Comm.VCOM_Message_ClosePort();
                myGlobalBase.is_Serial_Server_Connected = false;
            }
            btnScanAndConnectTool.Text = "Scan and Connect Tools";
            btnScanAndConnectTool.Enabled = true;
            btnClosePort.Enabled = true;
            cbVCOM.Enabled = true;
            cbFT232RL.Enabled = true;
            cbCOMList.Enabled = true;
            cbBaudRateSelect.Enabled = true;
            btn_ConnectSerial.Enabled = true;
        }
        #endregion

        #region //==================================================USB_Device_PlugIn_Detected_BGSurveyArray
        public delegate void USB_Device_PlugIn_Detected_BGSurveyArray_StartDelegate(bool ReportErrorIfNotFound);
        private void USB_Device_PlugIn_Detected_BGSurveyArray(bool ReportErrorIfNotFound)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new USB_Device_PlugIn_Detected_BGSurveyArray_StartDelegate(USB_Device_PlugIn_Detected_BGSurveyArray), new object[] { ReportErrorIfNotFound });
                return;
            }
            mbOperationBusy = null;
            mbOperationBusy = new DialogSupport();
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == true)
            {
                //rtbTerm.AppendText("#ERR: The port was left open, optionally you may close!\n");
                mbOperationBusy.PopUpMessageBox("BGDRILLING: USB was left Open, Auto Close and Try Again.", " BGDRILLING USB Connection", (2));
                myUSB_VCOM_Comm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.BGDRILLING);
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected = false;
                //return;
            }
            myGlobalBase.is_Serial_Server_Connected = false;            // Non Array VCOM discontinued. .
            btnScanAndConnectTool.Enabled = true;
            Thread.Sleep(500);                      // 1M=500, 1L=250 Short Pause to allow Device to complete power on process. 
            //-----------------------------------SCAN for PORT
            myGlobalBase.USB_SelectDeviceMode = (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM;
            btnScanAndConnectTool.Enabled = false;
            string VCOMPort = "";
            for (int i = 0; i < 2; i++)     // Two Loop Attempt
            {
                myUSBComPortManager.ScanPortNowArray((int)eUSBDeviceType.BGDRILLING, false);    // Do initial scan
                VCOMPort = myUSBComPortManager.ScanArrayVcomNowBGDRILLING_BGPING();             // Then do BGPING
                if ((myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == true) & (VCOMPort != ""))
                    break;
                Thread.Sleep(500);
            }
            //-------------------------------------------------------------------------------------------------------------------------------
            if ((myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == true) & (VCOMPort != ""))
            {
                myGlobalBase.Svy_isSurveyMode = false;
                myGlobalBase.is_BGDRILLING_Device_Activated = true;
                mbOperationBusy.PopUpMessageBox("BGDRILLING's USB is now connected.", " BGDRILLING USB Connection", (2));     // Close automatically after 2 Sec
                btnScanAndConnectTool.Text = "Close Port";
                btnScanAndConnectTool.Enabled = true;
                myGlobalBase.USBArray_RedirectDebugTerm = (int)eUSBDeviceType.BGDRILLING;
                //--------------------------------------------------------DMFPortocolsetup and implements. 
                myDMFProtocol.DMFProtocol_Reset();                                                            // clear any spurious message in DMF
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isDMFProtocolEnabled = true;      // Enable DMFP Protocol operation. 
                //--------------------------------------------------------Download Tools Data and save to file
                //Thread.Sleep(2000);                     
                //Tools.InteractivePause(TimeSpan.FromMilliseconds(500));
                //myBGReportViewer.isEnableShowWindow = BGSurveyToolStripMenuItem.Checked;        // Set false 
                //myBGReportViewer.Report_Show();                                                 // 76K: fixed SL bug, disable report show.
                //myBGReportViewer.Report_ClearData();
                if (myBGReportViewer.Report_Download_WithAttempts() == false)      // 76J: New design with repeat report frame loop, in case TOCO is busy.
                {
                    mbOperationBusy.PopUpMessageBox("ERROR: Report Frame Upload failure or Rental Data Expired/Error", "Report Frame Error", 30, 12F);
                }
                btnToCoBattery.BackColor = myBGBattery.UpdateBatteryStateColor();       // Update button color for battery status. 
            }
            else
            {
                if (ReportErrorIfNotFound == true)
                {
                    FlexiMessageBox.COLOR = Color.Red;
                    FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    FlexiMessageBox.Show("Unable to Detect BG Tools!"
                    + Environment.NewLine + "(1) Check/Replug Lemo Serial Cable."
                    + Environment.NewLine + "(2) Reset Power and Perhaps reboot Computer."
                    + Environment.NewLine + "(3) Try manual connection (refer to manual)",
                    "Unable to auto connect BG Survey Tool",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                }
                else
                {
                    myRtbTermMessageLF("#E:BG Tool Not Detected <Failed response from BGPING()>");
                }
                cbBaudRateSelect.Enabled = true;
                btn_ConnectSerial.Enabled = true;      // Avoid repeat clicking.
                cbVCOM.Enabled = true;
                cbFT232RL.Enabled = true;
                cbCOMList.Enabled = true;              // Keep it disabled until Com Port is stopped.
                btnScanAndConnectTool.Enabled = true;
                btnScanAndConnectTool.Text = "Scan and Connect Tools";
            }
        }
        #endregion

        #region //==================================================USB_Device_Removed_Detected_BGSurveyArray
        public delegate void USB_Device_Removed_Detected_BGSurveyArray_StartDelegate();
        private void USB_Device_Removed_Detected_BGSurveyArray()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new USB_Device_Removed_Detected_BGSurveyArray_StartDelegate(USB_Device_Removed_Detected_BGSurveyArray), null);
                return;
            }
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == false)
            {
                rtbTerm.AppendText("#E: What BGDRILLING COM Port?, there nothing to close!\n");
                return;
            }
            else
            {
                rtbTerm.AppendText("-I: Closing BGDRILLING COM Port, please wait few seconds until finally closed\n");
                myUSB_VCOM_Comm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.BGDRILLING);
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected = false;

            }
            cbBaudRateSelect.Enabled = true;
            btn_ConnectSerial.Enabled = true;      // Avoid repeat clicking.;
            btnScanAndConnectTool.Text = "Scan and Connect Tools";
            btnScanAndConnectTool.Enabled = true;
            btnClosePort.Enabled = true;
            cbVCOM.Enabled = true;
            cbFT232RL.Enabled = true;
            cbCOMList.Enabled = true;
            cbBaudRateSelect.Enabled = true;
            btn_ConnectSerial.Enabled = true;
            myGlobalBase.USBArray_RedirectDebugTerm = -1;
            myGlobalBase.is_BGDRILLING_Device_Activated = false;

        }
        #endregion

        #region //==================================================USB_Device_PlugIn_Detected_ADIS16460Board
        public delegate void USB_Device_PlugIn_Detected_ADIS16460Board_StartDelegate(bool ReportErrorIfNotFound);
        private void USB_Device_PlugIn_Detected_ADIS16460Board(bool ReportErrorIfNotFound)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new USB_Device_PlugIn_Detected_ADIS16460Board_StartDelegate(USB_Device_PlugIn_Detected_ADIS16460Board), new object[] { ReportErrorIfNotFound });
                return;
            }
            btn_ADIS_USBControl_SetMode(2);              //-------------------------Disable various control.
            Tools.InteractivePause(TimeSpan.FromSeconds(2));                // Short Pause to allow Device to complete power on process. 
            string VCOMPort = "";
            btnScanAndConnectTool.Enabled = false;
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isSerial_Server_Connected == true)   // Existing port, so cannot connect unless closed.     
            {
                rtbTerm.AppendText("#################################################################\n");
                rtbTerm.AppendText("#E: The Direct VCOM port (Non-Array) was left open. Please close!\n");
                rtbTerm.AppendText("#################################################################\n");
                btn_ADIS_USBControl_SetMode(1);
                return;
            }
            //-------------------------Perform Connect Operation
            //myGlobalBase.Svy_isSurveyMode = false;
            // myGlobalBase.VCOM_IsChange = false;
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isDMFProtocolEnabled = false;
            VCOMPort = myUSBComPortManager.ScanArrayVcomNowADISPING();      // Do inital scan on Array VCOM Arrange,emt

            if (VCOMPort == "")       // Not found
            {
                if (ReportErrorIfNotFound == true)
                {
                    myRtbTermMessageLF("#################################################################");
                    myRtbTermMessageLF("#E: The VCOM port (Array) Not Found, Try Again\n");
                    myRtbTermMessageLF("#################################################################");
                }
                btn_ADIS_USBControl_SetMode(1);
                return;
            }
            rtbTerm.Focus();
            this.Focus();
            rtbTerm.AppendText("-I: VCOM Connected!\n");
            //-----------------------------------DMFPortocolsetup and implments. 
            myDMFProtocol.DMFProtocol_Reset();                                                      // clear any spurious message in DMF
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isDMFProtocolEnabled = true;      // Enable DMFP Protocol operation. 

            Tools.InteractivePause(TimeSpan.FromSeconds(1));
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ADIS_Connect = new DMFP_Delegate(DMFP_ADIS_Connect_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+ADISCON()";
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t1 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_ADIS_Connect, sMessage, 250, (int)eUSBDeviceType.ADIS));
            t1.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
        }
        #endregion

        #region //==================================================USB_Device_Removed_Detected_ADIS16460
        public delegate void USB_Device_Removed_Detected_ADIS16460_StartDelegate();
        private void USB_Device_Removed_Detected_ADIS16460()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new USB_Device_Removed_Detected_ADIS16460_StartDelegate(USB_Device_Removed_Detected_ADIS16460), null);
                return;
            }
            btnADIS_Close_Port.PerformClick();      // Short cut solution
            btn_ADIS_USBControl_SetMode(3);
        }
        #endregion

        #region //==================================================USB_Device_PlugIn_Detected_MiniAD7794  (Rev 71)
        public delegate void USB_Device_PlugIn_Detected_MiniAD7794_StartDelegate(bool ReportErrorIfNotFound);
        private void USB_Device_PlugIn_Detected_MiniAD7794(bool ReportErrorIfNotFound)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new USB_Device_PlugIn_Detected_MiniAD7794_StartDelegate(USB_Device_PlugIn_Detected_MiniAD7794), new object[] { ReportErrorIfNotFound });
                return;
            }

            btnScanAndConnectTool.Enabled = false;
            string VCOMPort = "";
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == true)            // Existing port, so cannot connect unless closed.     
            {
                btnMiniConnect.Text = "Close Port";
                btnMiniSetup.Enabled = true;
                return;
            }
            //-------------------------Perform Connect Operation
            //myGlobalBase.Svy_isSurveyMode = false;
            // myGlobalBase.VCOM_IsChange = false;
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isDMFProtocolEnabled = false;
            int i = 0;
            myRtbTermMessage("-I: No of retry (500mSec): ");
            while (i < 8)        // repeat 5 x 500mSec loop 
            {
                Tools.InteractivePause(TimeSpan.FromMilliseconds(500));                    // Short Pause to allow Device to complete power on process. 
                VCOMPort = myUSBComPortManager.ScanArrayVcomNowMiniAD7794PING();    // Do inital scan on Array VCOM Arrange,emt
                if (VCOMPort != "")
                    break;
                i++;
                myRtbTermMessage("|");
            }

            if (VCOMPort == "")       // Not found
            {
                if (ReportErrorIfNotFound == true)
                {
                    myRtbTermMessageLF("#################################################################");
                    myRtbTermMessageLF("#E: The MiniAD7794 unable to connect, Try Again\n");
                    myRtbTermMessageLF("#################################################################");
                }
                btnMiniConnect.Text = "Connect";
                return;
            }
            rtbTerm.Focus();
            this.Focus();
            rtbTerm.AppendText("-I: VCOM Connected!\n");
            //-----------------------------------DMFPortocolsetup and implements. 
            myDMFProtocol.DMFProtocol_Reset();                                                      // clear any spurious message in DMF
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isDMFProtocolEnabled = true;      // Enable DMFP Protocol operation. 
            btnMiniConnect.Text = "Close Port";
            btnMiniSetup.Enabled = true;
            myGlobalBase.USB_SelectDeviceMode = (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM;
            myGlobalBase.USBArray_RedirectDebugTerm = (int)eUSBDeviceType.MiniAD7794;
        }
        #endregion

        #region //==================================================USB_Device_Removed_Detected_MiniAD7794  (Rev 71)
        public delegate void USB_Device_Removed_Detected_MiniAD7794_StartDelegate();
        private void USB_Device_Removed_Detected_MiniAD7794()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new USB_Device_Removed_Detected_MiniAD7794_StartDelegate(USB_Device_Removed_Detected_MiniAD7794), null);
                return;
            }
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                rtbTerm.AppendText("#E: What Mini7794 COM Port?, there nothing to close!\n");

            }
            else
            {
                rtbTerm.AppendText("-I: Closing Mini7794 COM Port, please wait few seconds until finally closed\n");
                myUSB_VCOM_Comm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.MiniAD7794);
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected = false;
            }
            btnMiniSetup.Enabled = false;
            btnMiniConnect.Text = "Connect";
            myGlobalBase.USBArray_RedirectDebugTerm = -1;
        }
        #endregion


        //##############################################################################################################
        #region//=============================================================================================BG Tool Section. 
        //##############################################################################################################
        RiscyTimer riscyTimerCOToolA;
        RiscyTimer riscyTimerCOCoreMarkA;
        RiscyTimer riscyTimerTOToolA;
        RiscyTimer riscyTimerTOToolMarkA;

        #region //==================================================cbSelectTS_MouseClick 
        private void cbSelectTS_MouseClick(object sender, MouseEventArgs e)
        {
            cbSelectTS.Checked = true;
            cbSelectTOCO.Checked = false;
            myBGMasterSetup.BGToCoToolSelectMode = 1;
            BGSurveyButtonSetting(0xFFFF);
        }
        #endregion

        #region //==================================================cbSelectTOCO_MouseClick 
        private void cbSelectTOCO_MouseClick(object sender, MouseEventArgs e)
        {
            cbSelectTS.Checked = false;
            cbSelectTOCO.Checked = true;
            myBGMasterSetup.BGToCoToolSelectMode = 0;
            BGSurveyButtonSetting(0xFFFF);
        }
        #endregion

        #region //==================================================btnTimeStamp_Click 
        private void btnTimeStamp_Click(object sender, EventArgs e)
        {
            if (riscyTimerCOToolA != null)
            {
                riscyTimerCOToolA.forceclose = true;
                riscyTimerCOToolA.Close();
            }
            if (myBGMasterSetup.BGToCoToolSelectMode == 0)
            {
                BGSurveyButtonSetting(5);           //Core Mark A
                if (myBGToCoProjectFileManager.sToCo_Master_Insert_TimeStamp() == false)
                {
                    btnTimeStamp.Visible = true;
                    mbOperationBusy.PopUpMessageBox("Unable to Record TimeStamp into this file\n" + myBGToCoProjectFileManager.ToCo_Master_FileName, "ToCo TimeStamp Record", 10, 12F);
                    BGSurveyButtonSetting(9);       // Tool A Error.
                }
                mbOperationBusy.PopUpMessageBox("TimeStamp Recorded into Master File\n" + myBGToCoProjectFileManager.ToCo_Master_FileName, "ToCo TimeStamp Record", 2, 12F);
                //---------------------------------------------------------------------------
                if (riscyTimerCOCoreMarkA != null)
                {
                    riscyTimerCOCoreMarkA.ForceClose();
                    riscyTimerCOCoreMarkA.Dispose();
                    riscyTimerCOCoreMarkA = null;
                }
                riscyTimerCOCoreMarkA = new RiscyTimer();
                riscyTimerCOCoreMarkA.RunRisyTimer(60, "Core-A Marked");
                //---------------------------------------------------------------------------
            }
            if (myBGMasterSetup.BGToCoToolSelectMode == 1)      // Get start and end timetstamp via report frame. 
            {
                btnTimeStamp.Enabled = false;
                myBGService.RunReportView();
                btnTimeStamp.Enabled = true;
            }
        }
        #endregion


        #region //==================================================btnToCoOrientation_Click        // Tool A
        private void btnToCoOrientation_Click(object sender, EventArgs e)               // Tool A
        {
            if (myBGMasterSetup.BGToCoToolSelectMode == 0)
            {
                if (riscyTimerCOCoreMarkA != null)
                {
                    riscyTimerCOCoreMarkA.forceclose = true;
                    riscyTimerCOCoreMarkA.Close();
                }
                ToCo_TO_Orientation_Run(false);
            }
            if (myBGMasterSetup.BGToCoToolSelectMode == 1)
            {
                myEEDownloadSurvey.EE_UpdateFolderName(myGlobalBase.BG_TSToco_FolderName);
                myEEDownloadSurvey.EE_LogMemSurvey_Show();
            }
        }
        #endregion

        #region //==================================================ToCo_TO_Orientation_Run        // Tool A
        private int ToolFaceDataCounter;
        private int ToolFaceTOBatchCounter;
        private void ToCo_TO_Orientation_Run(bool isAutoRunMode)               // Tool A
        {
            //int counter = 0;
            ToolFaceDataCounter = 0;
            mbOperationBusy = null;
            mbOperationBusy = new DialogSupport();
            if (isAutoRunMode == true)
                BGSurveyButtonSetting(9);       // Tool A Error.
            else
                BGSurveyButtonSetting(9);       // Tool A Error.
            //-------------------------------------------------------------------------------Copy TOCO Setup to Master file 
            //if (myBGToCoSetup.ToCoMasterReportClass.ToCo_Master_FileName != "")
            //{
            //    myBGToCoProjectFileManager.ToCo_Master_FileName = myBGToCoSetup.ToCoMasterReportClass.ToCo_Master_FileName;
            //}
            //if (myBGToCoSetup.ToCoMasterReportClass.ToCo_FolderName != "")
            //{
            //    myBGToCoProjectFileManager.ToCo_FolderName = myBGToCoSetup.ToCoMasterReportClass.ToCo_FolderName;
            //}
            //-------------------------------------------------------------------------------
            if (myGlobalBase.is_Serial_Server_Connected == true)        // Incorrect Port
            {
                if (debugModeToolStripMenuItem.Checked == false)
                    mbOperationBusy.PopUpMessageBox("USB Connect Error: Use the BGSurvey TabWindow Connects", "ToCo Orientation Mode", 7, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == false)
            {
                if (debugModeToolStripMenuItem.Checked == false)
                    mbOperationBusy.PopUpMessageBox("The Tool and PC has no serial interface, please check cable", "ToCo Orientation Mode", 7, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }

            //-------------------------------------------------------------------------------- Record STC_START Button click as reference to start survey for filename sync. 
            //-------------------------------------------------------------------- Step (1) Check if ReportSetup exist.
            if (myBGReportViewer.ReportClass == null)        // This should never happen!
            {
                mbOperationBusy.PopUpMessageBox("ERROR:UDT Software BUG! this should exist when created, report Bug to Service", "ToCo Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }
            //-------------------------------------------------------------------- Step (2) Check if Stop Survey Time has been recorded, if not then we need to implement report update and check again. 
            if (myBGReportViewer.ReportClass.Survey_EndTimeStamp == 0)
            {
                //----------------------------------Report Viewer
                if (myBGReportViewer.Report_Download_WithAttempts() == false)     // 76J: New design with repeat report frame loop, in case TOCO is busy.
                {
                    mbOperationBusy.PopUpMessageBox("ERROR: Report Frame Upload failure or Rental Data Expired/Error", "Report Frame Error", 30, 12F); mbOperationBusy.PopUpMessageBox("ERROR:No Logging Data Recorded, It Empty! or Timeout exceeded during Report Download", "ToCo Orientation Mode", 30, 12F);
                    BGSurveyButtonSetting(9);       // Tool A Error.
                    return;
                }
                myBGReportViewer.isEnableShowWindow = false;                    // 
                if (myBGReportViewer.ReportClass.Survey_EndTimeStamp == 0)
                {
                    myRtbTermMessageLF("+W: ToCo Tool does not have recorded Last Log Survey TimeStamp\n *We put in current UTC-Local timestamp on it*\n");
                    myBGReportViewer.ReportClass.Survey_EndTimeStamp = Tools.uDateTimeToUnixTimestampNowLocal();
                }

            }
            //-------------------------------------------------------------------- Step (3): Check default foldername which should be initialized by ToCoMasterReportClass in ReportClass (since connect or orientation button)
            if (myBGToCoProjectFileManager.ToCo_FolderName == "")        // No folder name, then use user document location.
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Foldername Name should exist but found none, goto SETUP button and correct foldername", "ToCo Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }
            //-------------------------------------------------------------------- Step (4) The ReportClass in ToCoMasterReportClass, has STCSTART timestamp from ToCo, then update STCSTART_Button_TimeStamp
            if (myBGReportViewer.ReportClass.bSTCSTART_Button_TimeStampFound == false)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: STCSTART timestamp was not found in report frame, is this correct version?", "ToCo Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }
            //-------------------------------------------------------------------- Step (5) Obtains the Capture timestamp filename. Updated 22/9/18 for SerialNo. 
            myBGReportViewer.ReportClass.STC_CoreCapture_Survey_TimeStamp = 0u;
            myBGReportViewer.ReportClass.STC_Filename_Survey_SerialNo = 0u;
            for (int i = 0; i < 4; i++)
            {
                string data = Tools.File_GetLine(myBGToCoProjectFileManager.sToCo_Master_FileName(), i);
                if (data != null)
                {
                    if (data.Contains("CoreCapture") == true)
                    {
                        string sTimeStamp = new String(data.Where(Char.IsDigit).ToArray());
                        myBGReportViewer.ReportClass.STC_CoreCapture_Survey_TimeStamp = Tools.ConversionStringtoUInt32(sTimeStamp);
                    }
                    if (data.Contains("SERIAL_NO") == true)
                    {
                        string sSerialNo = new String(data.Where(Char.IsDigit).ToArray());
                        myBGReportViewer.ReportClass.STC_Filename_Survey_SerialNo = Tools.ConversionStringtoUInt32(sSerialNo);
                    }
                }
            }
            if (myBGReportViewer.ReportClass.STC_CoreCapture_Survey_TimeStamp == 0u)
            {
                mbOperationBusy.PopUpMessageBox("Warning: Cannot find Core Capture Timestamp in filename, you can enter manually", "ToCo Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }
            if (myBGReportViewer.ReportClass.STC_Filename_Survey_SerialNo == 0u)
            {
                mbOperationBusy.PopUpMessageBox("Warning: Cannot find SerialNo in filename", "ToCo Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }
            if (myBGReportViewer.ReportClass.STC_Filename_Survey_SerialNo != myGlobalBase.ToCoSerialNumber)
            {
                mbOperationBusy.PopUpMessageBox("Warning: Serial Number Does not match between Report and Filename (but STC Start Timestamp in Tool matched with filename in Window)", "ToCo Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }
            if (!((myBGReportViewer.ReportClass.STC_CoreCapture_Survey_TimeStamp > myBGReportViewer.ReportClass.Survey_StartTimeStamp) &
                (myBGReportViewer.ReportClass.STC_CoreCapture_Survey_TimeStamp < myBGReportViewer.ReportClass.Survey_EndTimeStamp)))
            {
                mbOperationBusy.PopUpMessageBox("ERROR: The Core Capture Timestamp is outside the Start and End Survey Timestamp", "ToCo Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }
            myRtbTermMessageLF("-I: Target Capture TimeStamp. Dec:" + myBGReportViewer.ReportClass.STC_CoreCapture_Survey_TimeStamp.ToString() + " Hex: 0x" + myBGReportViewer.ReportClass.STC_CoreCapture_Survey_TimeStamp.ToString("X"));
            //myGlobalBase.bNoRFTermMessage = true;
            //--------------------------------------------------------------------------------This conclude the ToCo section for STCStart timestamp and folder creation.
            myBGReportViewer.ReportClass.CaptureDataFrameReset();
            //-------------------------------------------------------------------------------
            Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ToolFaceData = new DMFP_Delegate(DMFP_ToolFaceData);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolFaceData, "+TLPHDATA(0xF0;0x" + myBGReportViewer.ReportClass.STC_CoreCapture_Survey_TimeStamp.ToString("X") + ")", false, (int)eUSBDeviceType.BGDRILLING);
        }
        #endregion

        #region //================================================================DMFP_ToolFaceData
        // hPara[0] = 0x00 to 0x0F related to Error Code
        //          0x01: Capture TimeStamp is less than Logger Start TimeStamp.
        //          0x02: Command Parameter Error
        //          0x03: Data Processing error within Log_Survey_GetCaptureData(), sample too small or could not find logged data with Capture TimeStamp.

        public void DMFP_ToolFaceData(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            bool isDone = false;
            //------------------------------Error Reporting Section
            if (hPara[0] <= 0x0F)
            {
                switch (hPara[0])
                {
                    case (0x01):
                        {
                            mbOperationBusy.PopUpMessageBox("ERROR within TLPHDATA() in TOCO Tool: Capture TimeStamp is less than Logger Start TimeStamp", "ToCo Orientation Mode", 30, 12F);
                            BGSurveyButtonSetting(9);       // Tool A Error.
                            return;
                        }
                    case (0x02):
                        {
                            mbOperationBusy.PopUpMessageBox("ERROR within TLPHDATA() in TOCO Tool: Command Parameter Error", "ToCo Orientation Mode", 30, 12F);
                            BGSurveyButtonSetting(9);       // Tool A Error.
                            return;
                        }
                    case (0x03):
                        {
                            mbOperationBusy.PopUpMessageBox("ERROR within TLPHDATA() in TOCO Tool: Data Processing error within Log_Survey_GetCaptureData(), sample too small or could not find logged data with Capture TimeStamp.", "ToCo Orientation Mode", 30, 12F);
                            BGSurveyButtonSetting(9);       // Tool A Error.
                            return;
                        }
                    default:
                        {
                            mbOperationBusy.PopUpMessageBox("ERROR within TLPHDATA() in TOCO Tool: Other Error: Code: 0x" + hPara[0].ToString("X"), "ToCo Orientation Mode", 30, 12F);
                            BGSurveyButtonSetting(9);       // Tool A Error.
                            return;
                        }
                }
            }
            if (!((hPara.Count == 6) | (hPara.Count == 10)))
            {
                mbOperationBusy.PopUpMessageBox("ERROR within DMFP_ToolFaceData(): Expecting 6 or 10 element but recieved: " + hPara.Count.ToString(), "ToCo Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }
            //Int32 bigInt2 = Convert.ToInt32(hPara[3]);
            //Int16 smallInt2 = (Int16)bigInt2;
            //---------------------------- Received Data, hpara[1] = Timestamp, hpara[2]=MRC_Gx, hpara[3]=MRC_Gy, hpara[4]=MRC_Gz, hpara[5]=Temp : 6 element.
            switch (hPara[0])
            {
                case (0xF0):        // Actual timestamp capture.
                    {
                        myBGReportViewer.ReportClass.CaptureDataFrame[2].TimeStamp = hPara[1];
                        myBGReportViewer.ReportClass.CaptureDataFrame[2].MRC_Gx = (Int16)(Convert.ToInt32(hPara[2]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[2].MRC_Gy = (Int16)(Convert.ToInt32(hPara[3]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[2].MRC_Gz = (Int16)(Convert.ToInt32(hPara[4]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[2].Temp = hPara[5];
                        break;
                    }
                case (0xF1):        // Sample which is -2 data set
                    {
                        myBGReportViewer.ReportClass.CaptureDataFrame[0].TimeStamp = hPara[1];
                        myBGReportViewer.ReportClass.CaptureDataFrame[0].MRC_Gx = (Int16)(Convert.ToInt32(hPara[2]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[0].MRC_Gy = (Int16)(Convert.ToInt32(hPara[3]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[0].MRC_Gz = (Int16)(Convert.ToInt32(hPara[4]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[0].Temp = hPara[5];
                        break;
                    }
                case (0xF2):        // Sample which is -1 data set
                    {
                        myBGReportViewer.ReportClass.CaptureDataFrame[1].TimeStamp = hPara[1];
                        myBGReportViewer.ReportClass.CaptureDataFrame[1].MRC_Gx = (Int16)(Convert.ToInt32(hPara[2]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[1].MRC_Gy = (Int16)(Convert.ToInt32(hPara[3]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[1].MRC_Gz = (Int16)(Convert.ToInt32(hPara[4]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[1].Temp = hPara[5];
                        break;
                    }
                case (0xF3):        // Sample which is +1 data set
                    {
                        myBGReportViewer.ReportClass.CaptureDataFrame[3].TimeStamp = hPara[1];
                        myBGReportViewer.ReportClass.CaptureDataFrame[3].MRC_Gx = (Int16)(Convert.ToInt32(hPara[2]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[3].MRC_Gy = (Int16)(Convert.ToInt32(hPara[3]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[3].MRC_Gz = (Int16)(Convert.ToInt32(hPara[4]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[3].Temp = hPara[5];
                        break;
                    }
                case (0xF4):        // Last Sample which is +2 data set
                    {
                        myBGReportViewer.ReportClass.CaptureDataFrame[4].TimeStamp = hPara[1];
                        myBGReportViewer.ReportClass.CaptureDataFrame[4].MRC_Gx = (Int16)(Convert.ToInt32(hPara[2]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[4].MRC_Gy = (Int16)(Convert.ToInt32(hPara[3]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[4].MRC_Gz = (Int16)(Convert.ToInt32(hPara[4]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[4].Temp = hPara[5];
                        isDone = true;
                        break;
                    }
                default:
                    {
                        mbOperationBusy.PopUpMessageBox("ERROR within DMFP_ToolFaceData(): Reply parameter outside 0xF0-0xF4 scope. hPara[0]= " + hPara[0], "ToCo Orientation Mode", 30, 12F);
                        BGSurveyButtonSetting(9);       // Tool A Error.
                        return;
                    }

            }
            ToolFaceDataCounter++;
            //if (ToolFaceDataCounter>=5)
            //{
            //    mbOperationBusy.PopUpMessageBox("ERROR within DMFP_ToolFaceData(): Too many data transfer or stuck in loop, possible bug in TOCO firmwre (ie 0xF0-0xF4)", "ToCo Orientation Mode", 30, 12F);
            //    isDone = true;
            //}
            if (isDone == false)
            {
                UInt32 x = (hPara[0] - 0xF0) + 1;
                //-----------------------------------Setup call-back delegate. 
                DMFP_Delegate fpCallBack_ToolFaceData = new DMFP_Delegate(DMFP_ToolFaceData);
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolFaceData, "+TLPHDATA(0xF" + x.ToString() + ")", false, (int)eUSBDeviceType.BGDRILLING);
            }
            else
            {
                //-------------------------------------------------------------------- Append collected data and survey information to master filename.
                myBGToCoProjectFileManager.sToCo_Master_Insert_CaptureDataSection();      // Append list data to the master txt file for reference.
                //-------------------------------------------------------------------- Now we do cal data file download. 
                ToolFace_LoadCalData();
            }

        }
        #endregion

        #region //================================================================ToolFace_LoadCalData
        public void ToolFace_LoadCalData()
        {
            //--------------------------------------------------------------------Import CVS Data from LCP1549 Memory.
            myRtbTermMessageLF("-INFO: Now Importing calibration file (CVS)");
            string sFilename = System.IO.Path.Combine(myBGToCoProjectFileManager.ToCo_FolderName, "ImportedCalibrationFile" + myBGReportViewer.ReportClass.Survey_StartTimeStamp.ToString() + ".csv");
            myRtbTermMessageLF("-INFO: Saving Downloaded Filename: " + sFilename);
            myEEPROMExpImport.EEPROM_ImportDataNow(true, sFilename, 1);
        }
        #endregion

        #region //================================================================ToolFace_LoadCalData
        public void ToolFace_LoadCalData_SaveToFile()
        {
            //-----------------------------------------------------------------------
            while (myEEPROMExpImport.sSaveData == "")
            {
                //if (sw.ElapsedMilliseconds > 5000)
                //{
                mbOperationBusy.PopUpMessageBox("ERROR within ToolFace_PreStart_AfterCalDataLoad(): Calibration Data Issue during download, quit operation", "ToCo Orientation Mode", 30, 12F);
                return;
                //}
            }
            //-------------------------------------------------------------------- Decode Data into List Array between $S and $E. 
            if (myBGMasterCalData.CalDataConvert(myEEPROMExpImport.sSaveData) == false)
            {
                mbOperationBusy.PopUpMessageBox("ERROR within ToolFace_PreStart_AfterCalDataLoad(): detokenisation of caldata failure", "ToCo Orientation Mode", 30, 12F);
                return;
            }
            //--------------------------------------------------------------------- Append this Cal Data into file
            try
            {
                File.AppendAllText(myBGToCoProjectFileManager.ToCo_Master_FileName, "\n----------------------Cal Data Section\n");
                for (int i = 0; i < myBGMasterCalData.CalInfo.Count; i++)
                {
                    string s = myBGMasterCalData.CalInfo[i].Name + " = " + myBGMasterCalData.CalInfo[i].data;
                    File.AppendAllText(myBGToCoProjectFileManager.ToCo_Master_FileName, s + "\n");
                }
                for (int i = 0; i < myBGMasterCalData.CalData.Count; i++)
                {
                    string s = myBGMasterCalData.CalData[i].Name + " = " + myBGMasterCalData.CalData[i].dData.ToString();
                    File.AppendAllText(myBGToCoProjectFileManager.ToCo_Master_FileName, s + "\n");
                }
                File.AppendAllText(myBGToCoProjectFileManager.ToCo_Master_FileName, "----------------------\n");
            }
            catch { }
        }
        #endregion

        #region //================================================================ToolFace_PreStart_AfterCalDataLoad
        public void ToolFace_PreStart_AfterCalDataLoad()
        {
            //--------------------------------------------------------------------- Open Orientation Window.
            myBGOrientation.BG_Orientation_Show();
            if (cbSelectMathSL.Checked)       // Select SL-3A Math.
            {
                cbSelectMathSL.Checked = true;
                cbSelectMathRGP.Checked = false;
                myBGOrientation.BGOrientation_Start_Orientation(myBGMasterCalData, 3);
                if (myBGOrientation.MathDebug != "")
                {
                    File.AppendAllText(myBGToCoProjectFileManager.ToCo_Master_FileName, "----------------------\n");
                    File.AppendAllText(myBGToCoProjectFileManager.ToCo_Master_FileName, myBGOrientation.MathDebug);
                    File.AppendAllText(myBGToCoProjectFileManager.ToCo_Master_FileName, "----------------------\n");
                }
            }
            else                                // Select RGP Math.
            {
                cbSelectMathSL.Checked = false;
                cbSelectMathRGP.Checked = true;
                myBGOrientation.BGOrientation_Start_Orientation(myBGMasterCalData, 0);
                if (myBGOrientation.MathDebug != "")
                {
                    File.AppendAllText(myBGToCoProjectFileManager.ToCo_Master_FileName, "----------------------\n");
                    File.AppendAllText(myBGToCoProjectFileManager.ToCo_Master_FileName, myBGOrientation.MathDebug);
                    File.AppendAllText(myBGToCoProjectFileManager.ToCo_Master_FileName, "----------------------\n");
                }
            }

        }
        #endregion

        #region //==================================================BGToCo_ToolA_Start_Battery
        public delegate void BGToCo_ToolA_Start_BatteryDelegate();
        public void BGToCo_ToolA_Start_Battery()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new BGToCo_ToolA_Start_BatteryDelegate(BGToCo_ToolA_Start_Battery), null);
                return;
            }
            myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.CONNECT;
            BGSurveyButtonSetting(2);
            btnToCoBattery.PerformClick();
        }
        #endregion

        #region //==================================================btnToCoBattery_Click
        private void btnToCoBattery_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.BG_isToCoOrientationOpen == true)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Please close the BG Orientation Window first and try again", "ToCo Battery Mode", 30, 12F);
                return;
            }
            myBGReportViewer.Report_CloseNow();                  // Make sure the report view is close, use the service report viewer. 
            if (myGlobalBase.BG_isToCoBatteryOpen == false)
                myBGBattery.BG_Battery_Show();
        }
        #endregion

        #region //==================================================btnClientSetup_Click
        private void btnClientSetup_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.BG_isToCoOrientationOpen == true)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Please close the BG Orientation Window first and try again", "ToCo Battery Mode", 30, 12F);
                return;
            }
            myBGReportViewer.Report_CloseNow();                  // Make sure the report view is close, use the service report viewer. 
            btnClientSetup.Enabled = false;
            myBGClientData.ToCoSetup_Show();
            btnClientSetup.Enabled = true;

        }
        #endregion

        #region //==================================================btnBGService_Click
        private void btnBGService_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.BG_isToCoOrientationOpen == true)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Please close the BG Orientation Window first and try again", "ToCo Battery Mode", 30, 12F);
                return;
            }
            myBGReportViewer.Report_CloseNow();                  // Make sure the report view is close, use the service report viewer. 
            myBGService.BG_Service_Show(myBGToCoProjectFileManager.ToCo_FolderName);       //This linked to myBGMasterSetup.ToCo_FolderName
        }

        #endregion

        #region //==================================================BGToCo_ToolA_FinishJob
        public delegate void BGToCo_ToolA_FinishJobDelegate();
        public void BGToCo_ToolA_FinishJob()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new BGToCo_ToolA_FinishJobDelegate(BGToCo_ToolA_FinishJob), null);
                return;
            }
            myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.CONNECT;
            BGSurveyButtonSetting(2);
            btnScanAndConnectTool.PerformClick();
        }
        #endregion

        #region //==================================================btnScanAndConnectTool_Click (BGPING)
        private void btnScanAndConnectTool_Click(object sender, EventArgs e)
        {
            btnScanAndConnectTool.Enabled = false;
            //-------------------------Disable various control.
            cbBaudRateSelect.Enabled = false;
            btn_ConnectSerial.Enabled = false;      // Avoid repeat clicking.
            cbVCOM.Enabled = false;
            cbFT232RL.Enabled = false;
            cbCOMList.Enabled = false;
            //--------------------------------
            if (btnScanAndConnectTool.Text == "Scan and Connect Tools")     // This is open 
            {
                if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == true)
                {
                    mbOperationBusy.PopUpMessageBox("BGDRILLING: USB was left Open, Auto Close and Try Again.", " BGDRILLING USB Connection", (2));
                    myUSB_VCOM_Comm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.BGDRILLING);
                    myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected = false;
                }
                USB_Device_PlugIn_Detected_BGSurveyArray(true);
                BGSurveyButtonSetting(2);

                //USB_Device_PlugIn_Detected_BGSurvey(true);
            }
            else  // Close Port Button,
            {
                myUSB_Message_Manager.LoggerMessageRX = "";
                m_sEntryTxt = "STC_COMMEND()";
                myGlobalBase.LoggerOpertaionMode = true;
                myUSB_Message_Manager.endoflinedetected = false;
                Command_ASCII_Process_UART();
                Tools.InteractivePause(TimeSpan.FromMilliseconds(250)); // Allow pause for UART to put to standby, then. 
                USB_Device_Removed_Detected_BGSurveyArray();
                BGSurveyButtonSetting(1);
            }
        }
        #endregion

        #region//================================================================zBG_Async_BGCONNECT (Rev 75)
        public void BGToCo_Connected_UpdateToolType()
        {
            BGSurveyButtonSetting(2);
        }
        #endregion

        #region //==================================================Survey_Setup_Init
        private void Survey_Setup_Init()
        {

        }
        #endregion

        #region//================================================================zBG_Async_BGCONNECT (Rev 75)

        //public delegate void Async_BGCONNECT_Delegate(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage, IList<string> Asynclistparameter);
        public void zBG_Async_BGCONNECT(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage, IList<string> Asynclistparameter)
        {
            ////--------------------------------------------------------------------------76J for TOCO 3E: Bug Fix for inititing Report Frame process correctly. 
            //if (this.InvokeRequired)        // Check if we need to call BeginInvoke.
            //{
            //    this.BeginInvoke(new Async_BGCONNECT_Delegate(zBG_Async_BGCONNECT),
            //        new object[] { iPara, hPara, sPara, dPara, CmdMessage, FullMessage, Asynclistparameter});
            //    return;
            //}
            //--------------------------------------------------------------------------
            myBGReportViewer.Report_CloseNow();                                // Make sure the report view is close, use the service report viewer. 
            myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.ORIENTATION;
            BGSurveyButtonSetting(2);
            //Thread.Sleep(500);
            //------------------------------------------------------------------------------23/9/18: Added Report Viewer after connects. 
            bool status = myBGReportViewer.Report_Download_WithAttempts();     // 76J: New design with repeat report frame loop, in case TOCO is busy.
            myBGReportViewer.isEnableShowWindow = false;                       // Change to FALSE later. 
            if (status == false)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Report Frame Upload failure or Rental Data Expired/Error", "Report Frame Error", 30, 12F); BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }
            if (status == true)
            {
                if (BGSurvey_COAutoRunOrientationEnabledToolStripMenuItem.Checked == true)
                {
                    mbOperationBusy.PopUpMessageBox("This is Core Orientation Tool, AutoRun Orientation Mode.", "ToCo Orientation Mode", 2, 12F);
                    ToCo_TO_Orientation_Run(true);
                }
                else
                {
                    mbOperationBusy.PopUpMessageBox("This is Core Orientation Tool. You need to click Orient-A Button", "ToCo Orientation Mode", 2, 12F);
                }
            }
            if (myBGReportViewer.ReportClass.Survey_EndTimeStamp == 0)
            {
                myRtbTermMessageLF("+W: ToCo Tool does not have recorded Last Log Survey TimeStamp\n *We put in current UTC-Local timestamp on it*\n");
                myBGReportViewer.ReportClass.Survey_EndTimeStamp = Tools.uDateTimeToUnixTimestampNowLocal();
            }
        }
        #endregion

        #region//================================================================zBG_Update_Battery_Button
        public void zBG_Update_Battery_Button()
        {
            bool backup = btnToCoBattery.Enabled;
            btnToCoBattery.Enabled = true;
            btnToCoBattery.BackColor = myBGBattery.UpdateBatteryStateColor();       // Update button color for battery status. 
            btnToCoBattery.Refresh();
            btnToCoBattery.Enabled = backup;
        }

        #endregion

        //######################################################################################################################## Dual TOCO Section (TOOLA and TOOLB)

        #region //==================================================btnStartSurvey_Click  (START A)
        private void BGToCo_UpdateFilenameScan()
        {

        }
        #endregion

        //######################################################################################################################## CO Tool A

        #region //==================================================btnSetupSurvey_Click
        private void btnSetupSurvey_Click(object sender, EventArgs e)
        {
            if (debugModeToolStripMenuItem.Checked == false)
            {
                if (myGlobalBase.is_Serial_Server_Connected == false)
                {
                    FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    FlexiMessageBox.COLOR = Color.Red;
                    DialogResult response = FlexiMessageBox.Show("The Tool and PC has no serial interface, please check cable."
                    + Environment.NewLine + "OK     = Attempt to connect to tool."
                    + Environment.NewLine + "Cancel = Just open Survey Setup Box",
                    "Tool/PC Serial Connection",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);
                    if (response == DialogResult.OK)
                        btnScanAndConnectTool.PerformClick();
                    //Serial_ConnectSerial_Scan();
                }
            }
            Tools.InteractivePause(TimeSpan.FromMilliseconds(1000));        // Pause 1 Second.
                                                                            // in case it still false, open the window box anyway 
                                                                            // myBGToolSetup.BG_ToolSetup_Show();
        }
        #endregion

        #region //==================================================BGToCo_ToolA_Start_Survey
        public delegate void BGToCo_ToolA_Start_SurveyDelegate();
        public void BGToCo_ToolA_Start_Survey()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new BGToCo_ToolA_Start_SurveyDelegate(BGToCo_ToolA_Start_Survey), null);
                return;
            }
            myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.CONNECT;
            btnStartSurvey.PerformClick();
        }
        #endregion

        #region //==================================================btnStartSurvey_Click  (START A)
        private void btnStartSurvey_Click(object sender, EventArgs e)
        {
            if (riscyTimerCOToolA != null)
            {
                riscyTimerCOToolA.ForceClose();
                riscyTimerCOToolA.Dispose();
                riscyTimerCOToolA = null;
            }
            if (riscyTimerCOToolA == null)
                riscyTimerCOToolA = new RiscyTimer();
            riscyTimerCOToolA.RunRisyTimer(60, "ToolA Started");
            BGToCo_StartSurveyA_Now();
        }
        #endregion

        #region //==================================================BGToCo_StartSurveyA_Now  (START A)
        private void BGToCo_StartSurveyA_Now()
        {
            isSurveyRTCOkay = false;
            isSurveyEEEmpty = false;
            mbOperationBusy = null;
            mbOperationBusy = new DialogSupport();
            BGSurveyButtonSetting(3);
            //-------------------------------------------------------------------------------
            if (myGlobalBase.is_Serial_Server_Connected == true)        // Incorrect Port
            {
                if (debugModeToolStripMenuItem.Checked == false)
                    mbOperationBusy.PopUpMessageBox("USB Connect Error: Use the BGSurvey TabWindow Connects", "BGDrilling USB Interface", 7, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.

                return;
            }
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == false)
            {
                if (debugModeToolStripMenuItem.Checked == false)
                    mbOperationBusy.PopUpMessageBox("The Tool and PC has no serial interface, please check cable", "BGDrilling USB Interface", 7, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
            }
            //--------------------------------------------------------------------------------
            //RTC_Setup_SendNow();      22/9/18 comment out, not needed anymore, used STCStart(), firmware 3B updated to reflect this

            //-------------------------------------------------------------------------------- Record STC_START Button click as reference to start survey for filename sync. 
            // Step (1) Check if ReportSetup exist.
            //if (myBGToCoSetup.ToCoMasterReportClass == null)                    // This should never happen!
            //{
            //    mbOperationBusy.PopUpMessageBox("ERROR:UDT Software BUG! this should exist when created, report Bug to Service", "ToCo UDT Bug Error", 30, 12F);
            //    if (myBGReportViewer.ReportClass.ToCo_ToolType == 0)
            //        BGSurveyButtonSetting(9);       // Tool A Error.
            //    else
            //        BGSurveyButtonSetting(23);      // TO Tool Error
            //    return;
            //}
            // Step (2) Check if folder exist
            if (myBGToCoProjectFileManager.ToCo_FolderName == "")        // No folder name, then use user document location.
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Foldername Name should exist but found none, goto SETUP button and correct foldername", "ToCo FolderName", 30, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.

            }
            // Step (2) Record Button Timestamp
            if (myBGToCoProjectFileManager.ToCo_Master_STCSTART_Operation(myGlobalBase.ToCoSerialNumber) == false)    // Record timestamp, create ToCoMaster and save timestamp
            {
                mbOperationBusy.PopUpMessageBox("ERROR:Unable to create filename. Possible restricted folder region?\n:" + myBGToCoProjectFileManager.ToCo_Master_FileName, "ToCo Filename Failure", 30, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.

                return;
            }
            //--------------------------------------------------------------------------------This conclude the ToCo section for STCStart timestamp and folder creation.

            //-------------------------------------------------------------------------------
            Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_EEPROM_EEIsEmptyII = new DMFP_Delegate(DMFP_EEPROM_EEIsEmptyII_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+EEISEMPTY(0x1)";
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
            Thread t1;
            if (myGlobalBase.is_Serial_Server_Connected == true)
                t1 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_EEIsEmptyII, sMessage, 250));
            else
                t1 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_EEPROM_EEIsEmptyII, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t1.Start();
            Thread.Sleep(10);
        }
        #endregion

        //######################################################################################################################## CO Tool B



        //######################################################################################################################## TO Tool
        #region //================================================================btnTO_Start_Click
        private void btnTO_Start_Click(object sender, EventArgs e)
        {
            if (riscyTimerTOToolA != null)
            {
                riscyTimerTOToolA.ForceClose();
                riscyTimerTOToolA.Dispose();
                riscyTimerTOToolA = null;
            }
            if (riscyTimerTOToolA == null)
                riscyTimerTOToolA = new RiscyTimer();
            riscyTimerTOToolA.RunRisyTimer(40, "To-Tool Started");

            BGToCo_StartSurveyA_Now();

        }
        #endregion

        #region //================================================================btnTO_MarkTime_Click
        private void btnTO_MarkTime_Click(object sender, EventArgs e)
        {
            if (riscyTimerTOToolA != null)
            {
                riscyTimerTOToolA.forceclose = true;
                riscyTimerTOToolA.Close();
            }
            if (myBGMasterSetup.BGToCoToolSelectMode == 0)
            {
                if (myBGToCoProjectFileManager.sToCo_Master_Insert_TimeStamp() == false)
                {
                    btnTimeStamp.Visible = true;
                    mbOperationBusy.PopUpMessageBox("TO Mode:Unable to Record TimeStamp into this file\n" + myBGToCoProjectFileManager.ToCo_Master_FileName, "ToCo TimeStamp Record", 10, 12F);
                    BGSurveyButtonSetting(23);       // Tool A Error.
                }
                if (riscyTimerTOToolMarkA != null)
                {
                    riscyTimerTOToolMarkA.ForceClose();
                    riscyTimerTOToolMarkA.Dispose();
                    riscyTimerTOToolMarkA = null;
                }
                if (riscyTimerTOToolMarkA == null)
                    riscyTimerTOToolMarkA = new RiscyTimer();
                riscyTimerTOToolMarkA.RunRisyTimer(40, "To-MarkedTime");
                BGSurveyButtonSetting(21);
                mbOperationBusy.PopUpMessageBox("TO Mode: TimeStamp Recorded into Master File\n" + myBGToCoProjectFileManager.ToCo_Master_FileName, "ToCo TimeStamp Record", 2, 12F);
            }
            if (myBGMasterSetup.BGToCoToolSelectMode == 1)
            {
            }

        }
        #endregion

        #region //================================================================TOModeUpLoadDataToolStripMenuItem_Click
        private void TOModeUpLoadDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BGToCo_ToMode_UploadData();
        }
        #endregion

        #region //================================================================BGSurvey_COAutoRunOrientationEnabledToolStripMenuItem_Click
        private void BGSurvey_COAutoRunOrientationEnabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuItem temp = sender as MenuItem;
            if (temp != null)
            {
                temp.Checked = !temp.Checked;
            }
        }
        #endregion

        #region //================================================================BGToCo_ToMode_UploadData
        private void BGToCo_ToMode_UploadData()
        {
            if (riscyTimerTOToolMarkA != null)
            {
                riscyTimerTOToolMarkA.forceclose = true;
                riscyTimerTOToolMarkA.Close();
            }
            int counter = 0;
            ToolFaceDataCounter = 0;
            mbOperationBusy = null;
            mbOperationBusy = new DialogSupport();
            BGSurveyButtonSetting(22);       // Finish TO_Mode Operation
            //-------------------------------------------------------------------------------Copy TOCO Setup to Master file 
            //if (myBGToCoProjectFileManager.ToCo_Master_FileName != "")
            //{
            //    myBGToCoProjectFileManager.ToCo_Master_FileName = myBGToCoSetup.ToCoMasterReportClass.ToCo_Master_FileName;
            //}
            //if (myBGToCoSetup.ToCoMasterReportClass.ToCo_FolderName != "")
            //{
            //    myBGToCoProjectFileManager.ToCo_FolderName = myBGToCoSetup.ToCoMasterReportClass.ToCo_FolderName;
            //}
            //-------------------------------------------------------------------------------
            if (myGlobalBase.is_Serial_Server_Connected == true)        // Incorrect Port
            {
                if (debugModeToolStripMenuItem.Checked == false)
                    mbOperationBusy.PopUpMessageBox("#E: USB Connect Error: Use the BGSurvey TabWindow Connects", "ToCo Orientation Mode", 7, 12F);
                BGSurveyButtonSetting(23);       // To Tool Error.
                return;
            }
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == false)
            {
                if (debugModeToolStripMenuItem.Checked == false)
                    mbOperationBusy.PopUpMessageBox("#E: The Tool and PC has no serial interface, please check cable", "ToCo Orientation Mode", 7, 12F);
                BGSurveyButtonSetting(23);       // To Tool Error.
                return;
            }
            //-------------------------------------------------------------------- Step (1) Check if ReportSetup exist.
            if (myBGReportViewer.ReportClass == null)        // This should never happen!
            {
                mbOperationBusy.PopUpMessageBox("ERROR:UDT Software BUG! this should exist when created, report Bug to Service", "ToCo Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(23);       // To Tool Error.
                return;
            }
            //-------------------------------------------------------------------- Step (2) Check if Stop Survey Time has been recorded, if not then we need to implement report update and check again. 
            if (myBGReportViewer.ReportClass.Survey_EndTimeStamp == 0)
            {
                //----------------------------------Report Viewer
                //myBGReportViewer.Report_Show();               // No need to pop up. 
                bool status = myBGReportViewer.Report_Download_WithAttempts();     // 76J: New design with repeat report frame loop, in case TOCO is busy.
                //--------------------------------------------------
                BGSurveyButtonSetting(22);                      // Finish TO_Mode Operation
                myBGReportViewer.isEnableShowWindow = false;    // Change to FALSE later. 
                if (status == false)
                {
                    mbOperationBusy.PopUpMessageBox("ERROR: Report Frame Upload failure or Rental Data Expired/Error", "Report Frame Error", 30, 12F); BGSurveyButtonSetting(23);       // To Tool Error.
                    return;
                }
                if (myBGReportViewer.ReportClass.Survey_EndTimeStamp == 0)
                {
                    myRtbTermMessageLF("+W: ToCo Tool does not have recorded Last Log Survey TimeStamp\n *We put in current UTC-Local timestamp on it*\n");
                    myBGReportViewer.ReportClass.Survey_EndTimeStamp = Tools.uDateTimeToUnixTimestampNowLocal();
                }
            }
            //-------------------------------------------------------------------- Step (3): Check default foldername which should be initialized by ToCoMasterReportClass in ReportClass (since connect or orientation button)
            if (myBGToCoProjectFileManager.ToCo_FolderName == "")        // No folder name, then use user document location.
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Foldername Name should exist but found none, goto SETUP button and correct foldername", "ToCo Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(23);       // To Tool Error.
                return;
            }
            //-------------------------------------------------------------------- Step (4) The ReportClass in ToCoMasterReportClass, has STCSTART timestamp from ToCo, then update STCSTART_Button_TimeStamp
            if (myBGReportViewer.ReportClass.bSTCSTART_Button_TimeStampFound == false)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: STCSTART timestamp was not found in report frame, is this correct version?", "ToCo Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(23);       // To Tool Error.
                return;
            }
            //-------------------------------------------------------------------- Step (5) Obtains the Capture timestamp filename. Updated 22/9/18 for SerialNo. 
            if (myBGReportViewer.ReportClass.STC_TO_Survey_TimeStamp != null)       // Not empty, so we reset. 
            {
                myBGReportViewer.ReportClass.STC_TO_Survey_TimeStamp = null;
                myBGReportViewer.ReportClass.STC_TO_Survey_TimeStamp = new List<UInt32>();
            }
            myBGReportViewer.ReportClass.STC_Filename_Survey_SerialNo = 0u;
            counter = Tools.File_NoOfLines(myBGToCoProjectFileManager.sToCo_Master_FileName());   // 0 = No files found.
            if (counter >= 1)
            {
                for (int i = 0; i <= counter; i++)
                {
                    string data = Tools.File_GetLine(myBGToCoProjectFileManager.sToCo_Master_FileName(), i);
                    if (data != null)
                    {
                        if (data.Contains("SERIAL_NO") == true)
                        {
                            string sSerialNo = new String(data.Where(Char.IsDigit).ToArray());
                            myBGReportViewer.ReportClass.STC_Filename_Survey_SerialNo = Tools.ConversionStringtoUInt32(sSerialNo);
                        }
                        if (data.Contains("CoreCapture") == true)
                        {
                            string sTimeStamp = new String(data.Where(Char.IsDigit).ToArray());
                            myBGReportViewer.ReportClass.STC_TO_Survey_TimeStamp.Add(Tools.ConversionStringtoUInt32(sTimeStamp));
                        }
                    }
                }
            }
            else
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Unable to process filename or empty text, check ToCoMaster", "TO Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(23);       // To Tool Error.
                return;
            }
            if (myBGReportViewer.ReportClass.STC_TO_Survey_TimeStamp.Count == 0u)   // No corecapture found in there.
            {
                mbOperationBusy.PopUpMessageBox("Warning: Cannot find Core Capture Timestamp in filename, you can enter manually", "TO Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(23);       // To Tool Error.
                return;
            }
            if (myBGReportViewer.ReportClass.STC_Filename_Survey_SerialNo == 0u)
            {
                mbOperationBusy.PopUpMessageBox("Warning: Cannot find SerialNo in filename", "ToCo Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(23);       // To Tool Error.
                return;
            }
            if (myBGReportViewer.ReportClass.STC_Filename_Survey_SerialNo != myGlobalBase.ToCoSerialNumber)
            {
                mbOperationBusy.PopUpMessageBox("Warning: Serial Number Does not match between Report and Filename (but STC Start Timestamp in Tool matched with filename in Window)", "ToCo Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(23);       // To Tool Error.
                return;
            }
            bool isfails = false;
            myRtbTermMessageLF("-I: >>> Start TimeStamp.      Dec:0u" + myBGReportViewer.ReportClass.Survey_StartTimeStamp.ToString() + " Hex: 0x" + myBGReportViewer.ReportClass.Survey_StartTimeStamp.ToString("X"));
            for (int i = 0; i < myBGReportViewer.ReportClass.STC_TO_Survey_TimeStamp.Count; i++)
            {
                if ((myBGReportViewer.ReportClass.STC_TO_Survey_TimeStamp[i] < myBGReportViewer.ReportClass.Survey_StartTimeStamp) ||
                    (myBGReportViewer.ReportClass.STC_TO_Survey_TimeStamp[i] > myBGReportViewer.ReportClass.Survey_EndTimeStamp))
                    isfails = true;
                myRtbTermMessageLF("-I: Target Capture TimeStamp. Dec:0u" + myBGReportViewer.ReportClass.STC_TO_Survey_TimeStamp[i].ToString() + " Hex: 0x" + myBGReportViewer.ReportClass.STC_TO_Survey_TimeStamp[i].ToString("X"));
            }
            myRtbTermMessageLF("-I: >>> End TimeStamp.        Dec:0u" + myBGReportViewer.ReportClass.Survey_EndTimeStamp.ToString() + " Hex: 0x" + myBGReportViewer.ReportClass.Survey_EndTimeStamp.ToString("X"));
            if (isfails == true)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: The Core Mark Timestamp is outside the Start and End Survey Timestamp", "TO Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(24);       // Tool Timing Error.
                myBGToCoProjectFileManager.sToCo_Master_Filename_Append_String("###ERR: Timing conflict issue, unable to complete task, refer to service for recovery");
                return;
            }
            myBGToCoProjectFileManager.sToCo_Master_Filename_Append_String("This is Tool Orientation Tool Mode.");
            myBGToCoProjectFileManager.sToCo_Master_Filename_Append_String("Index = 2 is the closest to time-marked log data.");
            myBGToCoProjectFileManager.sToCo_Master_Filename_Append_String("Index = 0 and 1 is negative time-marker log data by logger capture period (ie -5, -10 sec).");
            myBGToCoProjectFileManager.sToCo_Master_Filename_Append_String("Index = 3 and 4 is positive time-marker log data by logger capture period (ie +5, +10 sec).");
            myBGToCoProjectFileManager.sToCo_Master_Filename_Append_String("At the time of writing, all data is not calibrated. Accel in mG Readout.");
            myBGToCoProjectFileManager.sToCo_Master_Filename_Append_String("Temp in mKelvin / 10 Readout. NB: All TimeStamp is based on Unix Time Epoch (1970).");
            ToolFaceTOBatchCounter = myBGReportViewer.ReportClass.STC_TO_Survey_TimeStamp.Count - 1;      // Number of upload element to download.
            myBGReportViewer.ReportClass.CaptureDataFrameReset();
            Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ToolFaceDataTO = new DMFP_Delegate(DMFP_ToolFaceDataTO);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolFaceDataTO, "+TLPHDATA(0xF0;0x" + myBGReportViewer.ReportClass.STC_TO_Survey_TimeStamp[ToolFaceTOBatchCounter].ToString("X") + ")", false, (int)eUSBDeviceType.BGDRILLING);
            //-------------------------------------------------------------------------------
        }
        #endregion

        #region //================================================================DMFP_ToolFaceData
        // hPara[0] = 0x00 to 0x0F related to Error Code
        //          0x01: Capture TimeStamp is less than Logger Start TimeStamp.
        //          0x02: Command Parameter Error
        //          0x03: Data Processing error within Log_Survey_GetCaptureData(), sample too small or could not find logged data with Capture TimeStamp.

        public void DMFP_ToolFaceDataTO(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            bool isDone = false;
            //------------------------------Error Reporting Section
            if (hPara[0] <= 0x0F)
            {
                switch (hPara[0])
                {
                    case (0x01):
                        {
                            mbOperationBusy.PopUpMessageBox("ERROR within TLPHDATA() in TOCO Tool: Capture TimeStamp is less than Logger Start TimeStamp", "To Orientation Mode", 30, 12F);
                            BGSurveyButtonSetting(24);       // Tool Error.
                            return;
                        }
                    case (0x02):
                        {
                            mbOperationBusy.PopUpMessageBox("ERROR within TLPHDATA() in TOCO Tool: Command Parameter Error", "To Orientation Mode", 30, 12F);
                            BGSurveyButtonSetting(24);       // Tool Error.
                            return;
                        }
                    case (0x03):
                        {
                            mbOperationBusy.PopUpMessageBox("ERROR within TLPHDATA() in TOCO Tool: Data Processing error within Log_Survey_GetCaptureData(), sample too small or could not find logged data with Capture TimeStamp.", "To Orientation Mode", 30, 12F);
                            BGSurveyButtonSetting(24);       // Tool Error.
                            return;
                        }
                    default:
                        {
                            mbOperationBusy.PopUpMessageBox("ERROR within TLPHDATA() in TOCO Tool: Other Error: Code: 0x" + hPara[0].ToString("X"), "To Orientation Mode", 30, 12F);
                            BGSurveyButtonSetting(24);       // Tool Error.
                            return;
                        }
                }
            }
            if (!((hPara.Count == 6) | (hPara.Count == 10)))
            {
                mbOperationBusy.PopUpMessageBox("ERROR within DMFP_ToolFaceData(): Expecting 6 or 10 element but recieved: " + hPara.Count.ToString(), "To Orientation Mode", 30, 12F);
                BGSurveyButtonSetting(24);       // Tool A Error.
                return;
            }
            //Int32 bigInt2 = Convert.ToInt32(hPara[3]);
            //Int16 smallInt2 = (Int16)bigInt2;
            //---------------------------- Received Data, hpara[1] = Timestamp, hpara[2]=MRC_Gx, hpara[3]=MRC_Gy, hpara[4]=MRC_Gz, hpara[5]=Temp : 6 element.
            switch (hPara[0])
            {
                case (0xF0):        // Actual timestamp capture.
                    {
                        myBGReportViewer.ReportClass.CaptureDataFrame[2].TimeStamp = hPara[1];
                        myBGReportViewer.ReportClass.CaptureDataFrame[2].MRC_Gx = (Int16)(Convert.ToInt32(hPara[2]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[2].MRC_Gy = (Int16)(Convert.ToInt32(hPara[3]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[2].MRC_Gz = (Int16)(Convert.ToInt32(hPara[4]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[2].Temp = hPara[5];
                        break;
                    }
                case (0xF1):        // Sample which is -2 data set
                    {
                        myBGReportViewer.ReportClass.CaptureDataFrame[0].TimeStamp = hPara[1];
                        myBGReportViewer.ReportClass.CaptureDataFrame[0].MRC_Gx = (Int16)(Convert.ToInt32(hPara[2]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[0].MRC_Gy = (Int16)(Convert.ToInt32(hPara[3]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[0].MRC_Gz = (Int16)(Convert.ToInt32(hPara[4]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[0].Temp = hPara[5];
                        break;
                    }
                case (0xF2):        // Sample which is -1 data set
                    {
                        myBGReportViewer.ReportClass.CaptureDataFrame[1].TimeStamp = hPara[1];
                        myBGReportViewer.ReportClass.CaptureDataFrame[1].MRC_Gx = (Int16)(Convert.ToInt32(hPara[2]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[1].MRC_Gy = (Int16)(Convert.ToInt32(hPara[3]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[1].MRC_Gz = (Int16)(Convert.ToInt32(hPara[4]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[1].Temp = hPara[5];
                        break;
                    }
                case (0xF3):        // Sample which is +1 data set
                    {
                        myBGReportViewer.ReportClass.CaptureDataFrame[3].TimeStamp = hPara[1];
                        myBGReportViewer.ReportClass.CaptureDataFrame[3].MRC_Gx = (Int16)(Convert.ToInt32(hPara[2]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[3].MRC_Gy = (Int16)(Convert.ToInt32(hPara[3]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[3].MRC_Gz = (Int16)(Convert.ToInt32(hPara[4]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[3].Temp = hPara[5];
                        break;
                    }
                case (0xF4):        // Last Sample which is +2 data set
                    {
                        myBGReportViewer.ReportClass.CaptureDataFrame[4].TimeStamp = hPara[1];
                        myBGReportViewer.ReportClass.CaptureDataFrame[4].MRC_Gx = (Int16)(Convert.ToInt32(hPara[2]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[4].MRC_Gy = (Int16)(Convert.ToInt32(hPara[3]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[4].MRC_Gz = (Int16)(Convert.ToInt32(hPara[4]));
                        myBGReportViewer.ReportClass.CaptureDataFrame[4].Temp = hPara[5];
                        isDone = true;
                        break;
                    }
                default:
                    {
                        mbOperationBusy.PopUpMessageBox("ERROR within DMFP_ToolFaceData(): Reply parameter outside 0xF0-0xF4 scope. hPara[0]= " + hPara[0], "ToCo Orientation Mode", 30, 12F);
                        BGSurveyButtonSetting(24);       // Tool A Error.
                        return;
                    }

            }
            ToolFaceDataCounter++;
            if (isDone == false)                        // Do more, not yet finish. 
            {
                UInt32 x = (hPara[0] - 0xF0) + 1;
                //-----------------------------------Setup call-back delegate. 
                DMFP_Delegate fpCallBack_ToolFaceDataTO = new DMFP_Delegate(DMFP_ToolFaceDataTO);
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolFaceDataTO, "+TLPHDATA(0xF" + x.ToString() + ")", false, (int)eUSBDeviceType.BGDRILLING);
                return;
            }

            //-------------------------------------------------------------------- Append collected data and survey information to master filename.
            myBGToCoProjectFileManager.sToCo_Master_Insert_CaptureDataSection();      // Append list data to the master txt file for reference.
            //-------------------------------------------------------------------- Any more timestamp to append?
            if (ToolFaceTOBatchCounter != 0)
            {
                ToolFaceTOBatchCounter--;
                //---------------------------------------------------------------------We now process next ToCo Upload data
                myBGReportViewer.ReportClass.CaptureDataFrameReset();
                Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
                //-----------------------------------Setup call-back delegate. 
                DMFP_Delegate fpCallBack_ToolFaceDataTO = new DMFP_Delegate(DMFP_ToolFaceDataTO);
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolFaceDataTO, "+TLPHDATA(0xF0;0x" + myBGReportViewer.ReportClass.STC_TO_Survey_TimeStamp[ToolFaceTOBatchCounter].ToString("X") + ")", false, (int)eUSBDeviceType.BGDRILLING);
            }
            else
            {
                //-------------------------------------------------------------------- Now we do cal data file download. 
                BGToCo_ToMode_LoadCalData();
                BGSurveyButtonSetting(25);
            }

        }
        #endregion

        #region //================================================================ToolFace_LoadCalData
        public void BGToCo_ToMode_LoadCalData()
        {
            //--------------------------------------------------------------------Import CVS Data from LCP1549 Memory.
            myRtbTermMessageLF("-INFO: Now Importing calibration file (CVS)");
            string sFilename = System.IO.Path.Combine(myBGToCoProjectFileManager.ToCo_FolderName, "ImportedCalibrationFile" + myBGReportViewer.ReportClass.Survey_StartTimeStamp.ToString() + ".csv");
            myRtbTermMessageLF("-INFO: Saving Downloaded Filename: " + sFilename);
            myEEPROMExpImport.EEPROM_ImportDataNow(true, sFilename, 2);
        }
        #endregion

        #region //================================================================btnTO_DoMath_Click
        // The purpose of this function is to process the TOCO_MASTER filename where sensor readout is collected and then make new filename which include maths result factoring the calibration file.
        // ###TASK: Later coding.
        private void btnTO_DoMath_Click(object sender, EventArgs e)
        {

        }
        #endregion

        //########################################################################################################################

        #region //================================================================DMFP_EEPROM_EEIsEmptyII_CallBack
        public void DMFP_EEPROM_EEIsEmptyII_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            DMFPCounter++;
            if (hPara[0] == 0xFF)
                isSurveyEEEmpty = true;
            else
            {
                isSurveyEEEmpty = false;        // Do quick erase
                EEPROM_EEFastEraseProcedureII_Start();
                return;
            }
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_RTCCheckII = new DMFP_Delegate(DMFP_CONFIGUpdate_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+RTCCHECK(0x3)";
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
            Thread t2;
            if (myGlobalBase.is_Serial_Server_Connected == true)
                t2 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_RTCCheckII, sMessage, 250));
            else
                t2 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_RTCCheckII, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t2.Start();
            Thread.Sleep(10);
        }
        #endregion

        #region //================================================================EEPROM_EEFastEraseProcedureII_Start
        private void EEPROM_EEFastEraseProcedureII_Start()
        {
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_EEPROM_EEFastEraseProcedureII = new DMFP_Delegate(DMFP_EEPROM_EEFastEraseProcedureII_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+EEERASELOGDATA(0x5)";       // Fast Erase Procedure.
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
            Thread t11;
            if (myGlobalBase.is_Serial_Server_Connected == true)
                t11 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_EEFastEraseProcedureII, sMessage, 250));
            else
                t11 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_EEPROM_EEFastEraseProcedureII, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t11.Start();
            Thread.Sleep(10);
        }
        #endregion

        #region //================================================================DMFP_EEPROM_EEFastEraseProcedureII_CallBack
        public void DMFP_EEPROM_EEFastEraseProcedureII_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] == 0xFF)
                isSurveyEEEmpty = true;
            else
            {
                isSurveyEEEmpty = false;        // Do quick erase
                mbOperationBusy.PopUpMessageBox("ERR: Logger Memory Fast Erase failure. Try again with lower level erase command or seek Service", "Logger Memory Error", 60, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_RTCCheckII = new DMFP_Delegate(DMFP_CONFIGUpdate_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+RTCCHECK(0x3)";
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
            Thread t12;

            if (myGlobalBase.is_Serial_Server_Connected == true)
                t12 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_RTCCheckII, sMessage, 250));
            else
                t12 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_RTCCheckII, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t12.Start();
            Thread.Sleep(10);
        }
        #endregion

        #region //================================================================DMFP_CONFIGUpdate_CallBack
        public void DMFP_CONFIGUpdate_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            mbOperationBusy = null;
            mbOperationBusy = new DialogSupport();

            if (hPara[0] == 0xFF)
                isSurveyRTCOkay = true;
            else
                isSurveyRTCOkay = false;

            if (isSurveyRTCOkay == false)
            {
                mbOperationBusy.PopUpMessageBox("TOOL ERROR: RTC Failure!", "Log Data Tool Operation", 5, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }

            if (myBGToCoProjectFileManager.myToCoReportClass.isSamplePeroidChanged == false)
            {
                myBGToCoProjectFileManager.myToCoReportClass.isClientDataTransferDone = false;
                Client_Data_Report_Download();
                return;
            }

            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ConfigUpdate = new DMFP_Delegate(DMFP_CONFIGClientData_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+CFGWRITE(0x001A;0x" + myBGToCoProjectFileManager.myToCoReportClass.iSampleperoid.ToString("X") + " )";      // Update Sample Peroid. 
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
            Thread t20;

            if (myGlobalBase.is_Serial_Server_Connected == true)
                t20 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_ConfigUpdate, sMessage, 250));
            else
                t20 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_ConfigUpdate, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t20.Start();
            Thread.Sleep(10);
        }
        #endregion

        #region //================================================================DMFP_CONFIGClientData_CallBack
        public void DMFP_CONFIGClientData_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            myBGToCoProjectFileManager.myToCoReportClass.isClientDataTransferDone = false;
            Client_Data_Report_Download();
        }
        #endregion

        #region //================================================================Client_Data_Report_Download
        int ClientDataRefNo;
        //#####################################################################################################
        //###################################################################################### Client Data Transfer
        //#####################################################################################################
        public void Client_Data_Report_Download()
        {
            ClientDataRefNo = 0;
            if (myBGToCoProjectFileManager.myToCoReportClass.ClientDataFrame.Count == 0)
            {
                //Application_STCSTART_Start();
                DMFP_Application_STCSTART_StartCallBack();
            }
            else
            {
                while (ClientDataRefNo < myBGToCoProjectFileManager.myToCoReportClass.ClientDataFrame.Count)
                {
                    if (myBGToCoProjectFileManager.myToCoReportClass.ClientDataFrame[ClientDataRefNo].sNote != "")
                    {
                        break;
                    }
                    ClientDataRefNo++;
                }
                if (ClientDataRefNo >= myBGToCoProjectFileManager.myToCoReportClass.ClientDataFrame.Count)        // No Client Data to transfer, Task Completed
                {
                    //Application_STCSTART_Start();
                    DMFP_Application_STCSTART_StartCallBack();
                    return;
                }
                Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
                //-----------------------------------Setup call-back delegate. 
                DMFP_Delegate fpCallBack_ClientDataLoopI = new DMFP_Delegate(DMFP_ClientData_CallBack);
                //-----------------------------------Place Message to rtbTerm Window.
                string sMessage = "+CDWRITE(0x003" + ClientDataRefNo.ToString() + ";0s" + myBGToCoProjectFileManager.myToCoReportClass.ClientDataFrame[ClientDataRefNo].sNote + ")\n";

                //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
                Thread t30;
                if (myGlobalBase.is_Serial_Server_Connected == true)
                    t30 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_ClientDataLoopI, sMessage, 250));
                else
                    t30 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_ClientDataLoopI, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
                t30.Start();
                Thread.Sleep(10);
            }
        }
        #endregion

        #region //================================================================DMFP_ClientData_CallBack  
        public void DMFP_ClientData_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFF)
            {
                mbOperationBusy.PopUpMessageBox("TOOL ERROR: Client Data +CDWRITE(....) return with error", "Tool Operation Error", 5, 12F);
                BGSurveyButtonSetting(9);       // Tool A Error.
                return;
            }
            ClientDataRefNo++;
            while (ClientDataRefNo < myBGToCoProjectFileManager.myToCoReportClass.ClientDataFrame.Count)
            {
                if (myBGToCoProjectFileManager.myToCoReportClass.ClientDataFrame[ClientDataRefNo].sNote != "")
                {
                    break;
                }
                ClientDataRefNo++;
            }
            if (ClientDataRefNo >= myBGToCoProjectFileManager.myToCoReportClass.ClientDataFrame.Count)        // No Client Data to transfer, Task Completed.
            {
                //Application_STCSTART_Start();
                DMFP_Application_STCSTART_StartCallBack();
                return;
            }
            //-----------------------------------------------------------------Next Data to collects
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ClientDataLoopII = new DMFP_Delegate(DMFP_ClientData_CallBack);        // Loop Back
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+CDWRITE(0x003" + ClientDataRefNo.ToString() + ";0s" + myBGToCoProjectFileManager.myToCoReportClass.ClientDataFrame[ClientDataRefNo].sNote + ")\n";
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
            Thread t31;
            if (myGlobalBase.is_Serial_Server_Connected == true)
                t31 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_ClientDataLoopII, sMessage, 250));
            else
                t31 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_ClientDataLoopII, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t31.Start();
            Thread.Sleep(10);
        }
        #endregion

        #region //================================================================Application_STCSTART_Start()

        private void Application_STCSTART_Start()
        {
            myUSB_Message_Manager.LoggerMessageRX = "";
            m_sEntryTxt = "STC_START(0;" + myBGToCoProjectFileManager.myToCoReportClass.sSTCSTART_Button_TimeStamp + ")";                            // Start Survey in application context (not debug)
            myGlobalBase.LoggerOpertaionMode = true;
            myUSB_Message_Manager.endoflinedetected = false;
            myBGReportViewer.ReportClass.Survey_EndTimeStamp = 0;
            Terminal_Append_Response("The tool is now in Survey/Logging Mode\n");
            Terminal_Append_Response("STEP 1: Check ':' response to confirm Survey Mode (wait 10 second).\n");
            Terminal_Append_Response("STEP 2: Disconnect Serial Lemo cable\n");
            Terminal_Append_Response(m_sEntryTxt + '\n');
            Command_ASCII_Process_UART();
            //BGDrilling_Start_Survey();
        }
        #endregion

        #region //================================================================Application_STCSTART_StartCallBack()                  // New command 76G and ToCo Rev 3D

        private void DMFP_Application_STCSTART_StartCallBack()
        {
            myUSB_Message_Manager.LoggerMessageRX = "";
            myGlobalBase.LoggerOpertaionMode = true;
            myUSB_Message_Manager.endoflinedetected = false;
            myBGReportViewer.ReportClass.Survey_EndTimeStamp = 0;
            Terminal_Append_Response("The tool is now in Survey/Logging Mode\n");
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_STCSTART = new DMFP_Delegate(DMFP_STCSTART_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+STCSTART(0;" + myBGToCoProjectFileManager.myToCoReportClass.sSTCSTART_Button_TimeStamp + ")";
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
            Thread t12 = null;
            if (myGlobalBase.is_Serial_Server_Connected == false)
                t12 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_STCSTART, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t12.Start();
            Thread.Sleep(10);
        }
        #endregion

        #region //================================================================DMFP_STCSTART_CallBack
        public void DMFP_STCSTART_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            switch (hPara[0])
            {
                case (0x01):
                    {
                        Terminal_Append_Response("#ERR:Start Survey Success!\n");
                        break;
                    }
                case (0x02):
                    {
                        Terminal_Append_Response("#ERR:Start Survey Success!\n");
                        break;
                    }
                case (0x03):
                    {
                        Terminal_Append_Response("#ERR:Start Survey Success!\n");
                        break;
                    }
                case (0x04):
                    {
                        Terminal_Append_Response("#ERR:Start Survey Success!\n");
                        break;
                    }
                case (0xFF):
                    {
                        Terminal_Append_Response("-INFO: Start Survey Success!\n");
                        Terminal_Append_Response("-INFO: Disconnect Serial Lemo cable, Now!\n");
                        break;
                    }
            }
        }
        #endregion

        //########################################################################################################################

        #region //================================================================btnSurveyReset_Click()
        private void btnSurveyReset_Click(object sender, EventArgs e)
        {
            BGSurveyButtonSetting(0xFF);
        }
        #endregion

        #region //================================================================BGSurveyButtonSetting()
        public delegate void BGSurveyButtonSetting_StartDelegate(int Mode);

        public void BGSurveyButtonSetting(int Mode)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new BGSurveyButtonSetting_StartDelegate(BGSurveyButtonSetting), new object[] { Mode });
                return;
            }
            try
            {
                //------------------------------------------------------ TS Mode.
                if (myBGMasterSetup.BGToCoToolSelectMode == 1)
                {
                    btnTimeStamp.Text = "Report";
                    btnTimeStamp.Enabled = true;
                    btnToCoOrientation.Text = "LogMem";
                    btnToCoOrientation.Enabled = true;
                    btnStartSurvey.Enabled = true;

                    switch (Mode)
                    {
                        //--------------------------------------------------------------------------------------
                        case (0):       //Default (Run this once after UDT is run), assumed CO configuration (default)
                            {
                                //--------------------------------------
                                myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.DISCONNECT;
                                //-------------------------------------Misc
                                btnClientSetup.Enabled = true; btnClientSetup.Visible = true;
                                btnToCoBattery.Enabled = true; btnToCoBattery.Visible = true;
                                btnBGService.Enabled = true; btnBGService.Visible = true;
                                //--------------------------------------Misc
                                btnStopSurvey.Enabled = true; btnStopSurvey.Visible = true;
                                //myBGReportViewer.ReportClass.ToCo_ToolType = 0;                 // Default 0.
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (1):       //Disconnect Tool
                            {
                                btnClientSetup.Enabled = false;
                                btnToCoBattery.Enabled = false;
                                btnBGService.Enabled = false;
                                //--------------------------------------Misc
                                btn_ConnectSerial.Enabled = false;
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (2):       //Connect Tool
                            {
                                btnClientSetup.Enabled = true;
                                btnToCoBattery.Enabled = true;
                                btnBGService.Enabled = true;
                                this.Refresh();
                                //--------------------------------------
                                btn_ConnectSerial.Enabled = false;
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (0xFE):  // Disable this feature. 
                            {
                                myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.OFF;
                                //-------Tool A
                                btnTimeStamp.Visible = false;
                                //-------Misc
                                btnClientSetup.Visible = true; btnClientSetup.Enabled = true;
                                btnToCoBattery.Visible = true; btnToCoBattery.Enabled = true;
                                btnBGService.Visible = true; btnBGService.Enabled = true;

                                btnStopSurvey.Visible = false;
                                btn_ConnectSerial.Enabled = true;
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (0xFF):
                            {
                                //if (btnScanAndConnectTool.Text != "Scan and Connect Tools")         // Need to closd Port as part of reset. 
                                //{
                                //    myUSB_Message_Manager.LoggerMessageRX = "";
                                //    m_sEntryTxt = "STC_COMMEND()";
                                //    myGlobalBase.LoggerOpertaionMode = true;
                                //    myUSB_Message_Manager.endoflinedetected = false;
                                //    Command_ASCII_Process_UART();
                                //    Tools.InteractivePause(TimeSpan.FromMilliseconds(250));         // Allow pause for UART to put to standby, then. 
                                //    USB_Device_Removed_Detected_BGSurveyArray();
                                //    //BGSurveyButtonSetting(1);
                                //}
                                //myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.DISCONNECT;

                                btnTimeStamp.Visible = true;
                                //-------Misc
                                btnClientSetup.Visible = true; btnClientSetup.Enabled = true;
                                btnToCoBattery.Visible = true; btnToCoBattery.Enabled = true;
                                btnBGService.Visible = true; btnBGService.Enabled = true;

                                btnStopSurvey.Visible = false;
                                btn_ConnectSerial.Enabled = true;
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        default:        //Reset (0xFF)
                            {
                                break;
                            }

                    }
                }
                //------------------------------------------------------ TOCO Mode.
                if (myBGMasterSetup.BGToCoToolSelectMode == 0)
                {
                    btnTimeStamp.Text = "Core-Mark";
                    btnToCoOrientation.Text = "Orient";
                    switch (Mode)
                    {
                        //--------------------------------------------------------------------------------------
                        case (0):       //Default (Run this once after UDT is run), assumed CO configuration (default)
                            {
                                //--------------------------------------
                                myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.DISCONNECT;
                                //--------------------------------------Tool A
                                btnStartSurvey.Visible = true; btnStartSurvey.Enabled = true;
                                btnTimeStamp.Visible = true; btnTimeStamp.Enabled = false;
                                btnToCoOrientation.Visible = true; btnToCoOrientation.Enabled = false;
                                //-------------------------------------Misc
                                btnClientSetup.Enabled = false; btnClientSetup.Visible = true;
                                btnToCoBattery.Enabled = false; btnToCoBattery.Visible = true;
                                btnBGService.Enabled = false; btnBGService.Visible = true;
                                //--------------------------------------Misc
                                btnStopSurvey.Enabled = false; btnStopSurvey.Visible = false;
                                //myBGReportViewer.ReportClass.ToCo_ToolType = 0;                 // Default 0.
                                //--------------------------------------TO Buttons

                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (1):       //Disconnect Tool
                            {
                                btnClientSetup.Enabled = false;
                                btnToCoBattery.Enabled = false;
                                btnBGService.Enabled = false;
                                //------------------------------------------------------------
                                btnStartSurvey.Show();
                                btnTimeStamp.Show();
                                btnToCoOrientation.Show();
                                switch (myBGMasterSetup.BGToCoToolAState)   // Tool A
                                {
                                    case ((int)BGToCoToolA.STARTMODE):      // Do nothing, the tool is busy
                                        {
                                            btnStartSurvey.Enabled = false;
                                            btnTimeStamp.Enabled = true;
                                            btnToCoOrientation.Enabled = false;
                                            break;
                                        }
                                    case ((int)BGToCoToolA.COREMARKED):     // Do nothing, the tool is busy
                                        {
                                            btnStartSurvey.Enabled = false;
                                            btnTimeStamp.Enabled = false;
                                            btnToCoOrientation.Enabled = false;
                                            break;
                                        }
                                    case ((int)BGToCoToolA.ORIENTATION):    // Close Orientation window?
                                        {
                                            break;
                                        }
                                    default:    // All goes to Disconnect state. 
                                        {
                                            myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.DISCONNECT;
                                            btnToCoOrientation.Visible = true;
                                            break;
                                        }
                                }

                                //--------------------------------------Misc
                                btn_ConnectSerial.Enabled = false;
                                break;

                            }
                        //--------------------------------------------------------------------------------------
                        case (2):       //Connect Tool
                            {
                                btnClientSetup.Enabled = true;
                                btnToCoBattery.Enabled = true;
                                btnBGService.Enabled = true;
                                this.Refresh();
                                //------------------------------------------------------------
                                btnStartSurvey.Show();
                                btnTimeStamp.Show();
                                btnToCoOrientation.Show();
                                switch (myBGMasterSetup.BGToCoToolAState)   // Tool A
                                {
                                    case ((int)BGToCoToolA.STARTMODE):      // Do nothing, the tool is busy
                                        {
                                            btnStartSurvey.Enabled = false;
                                            btnTimeStamp.Enabled = true;
                                            btnToCoOrientation.Enabled = false;
                                            break;
                                        }
                                    case ((int)BGToCoToolA.COREMARKED):     // Do nothing, the tool is busy
                                        {
                                            btnStartSurvey.Enabled = false;
                                            btnTimeStamp.Enabled = false;
                                            btnToCoOrientation.Enabled = true;
                                            break;
                                        }
                                    case ((int)BGToCoToolA.ORIENTATION):    //ORIENTATION to CONNECT state. #TASK: Orientation window to close
                                        {
                                            btnStartSurvey.Enabled = true;
                                            btnTimeStamp.Enabled = false;
                                            btnToCoOrientation.Enabled = true;
                                            break;
                                        }
                                    default:    // All goes to CONNECT state. 
                                        {
                                            myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.CONNECT;
                                            btnStartSurvey.Enabled = true;
                                            btnTimeStamp.Enabled = false;
                                            btnToCoOrientation.Enabled = true;
                                            break;

                                        }
                                }

                                //--------------------------------------
                                btn_ConnectSerial.Enabled = false;
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (3):       //Clicked Start A
                            {
                                myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.STARTMODE;
                                btnStartSurvey.Enabled = false;
                                btnTimeStamp.Enabled = true;
                                btnToCoOrientation.Enabled = false;
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (4):       //Clicked Start B
                            {
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (5):       //Clicked Core Mark A
                            {
                                myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.COREMARKED;
                                btnTimeStamp.Enabled = false;
                                btnStartSurvey.Enabled = false;
                                btnToCoOrientation.Enabled = false;
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (6):       //Clicked Core Mark B
                            {
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (7):       //Clicked Orientation A
                            {
                                myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.ORIENTATION;
                                btnToCoOrientation.Enabled = false;
                                btnStartSurvey.Enabled = false;
                                btnTimeStamp.Enabled = false;
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (8):       //Clicked Orientation B
                            {
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (9):       //Error Mode A
                            {
                                btnToCoOrientation.Enabled = true;
                                btnStartSurvey.Enabled = true;
                                btnTimeStamp.Enabled = true;
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (10):       //Error Mode B
                            {
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (11):       //Clicked End (close Orientation A)
                            {
                                myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.END;
                                btnToCoOrientation.Enabled = true;
                                btnStartSurvey.Enabled = true;
                                btnTimeStamp.Enabled = false;
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (12):       //Clicked End (close Orientation B)
                            {
                                break;
                            }
                        //--------------------------------------------------------------------------------------TO Only
                        case (20):       //TO Start Button Clicked
                            {
                                break;
                            }
                        //--------------------------------------------------------------------------------------TO Only
                        case (21):       //TO Mark Time Button Clicked.
                            {
                                break;
                            }
                        //--------------------------------------------------------------------------------------TO Only
                        case (22):       //Finish TO Operation.
                            {
                                break;
                            }
                        //--------------------------------------------------------------------------------------TO Only
                        case (23):       //TO Error
                            {
                                break;
                            }
                        //--------------------------------------------------------------------------------------TO Only
                        case (24):       //Finish to restart
                            {
                                break;
                            }
                        //--------------------------------------------------------------------------------------TO Only
                        case (25):       //Do Math and Start 
                            {
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (0xFE):  // Disable this feature. 
                            {
                                myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.OFF;
                                //-------Tool A
                                btnStartSurvey.Visible = true; btnStartSurvey.Enabled = true;
                                btnTimeStamp.Visible = true; btnTimeStamp.Enabled = true;
                                btnToCoOrientation.Visible = true; btnToCoOrientation.Enabled = true;
                                //-------Misc
                                btnClientSetup.Visible = true; btnClientSetup.Enabled = true;
                                btnToCoBattery.Visible = true; btnToCoBattery.Enabled = true;
                                btnBGService.Visible = true; btnBGService.Enabled = true;

                                btnStopSurvey.Visible = false;
                                btn_ConnectSerial.Enabled = true;
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        case (0xFF):
                            {
                                if (btnScanAndConnectTool.Text != "Scan and Connect Tools")         // Need to closd Port as part of reset. 
                                {
                                    myUSB_Message_Manager.LoggerMessageRX = "";
                                    m_sEntryTxt = "STC_COMMEND()";
                                    myGlobalBase.LoggerOpertaionMode = true;
                                    myUSB_Message_Manager.endoflinedetected = false;
                                    Command_ASCII_Process_UART();
                                    Tools.InteractivePause(TimeSpan.FromMilliseconds(250)); // Allow pause for UART to put to standby, then. 
                                    USB_Device_Removed_Detected_BGSurveyArray();
                                    //BGSurveyButtonSetting(1);
                                }
                                myBGMasterSetup.BGToCoToolAState = (int)BGToCoToolA.DISCONNECT;
                                //-------Tool A
                                btnStartSurvey.Visible = true; btnStartSurvey.Enabled = true;
                                btnTimeStamp.Visible = true; btnTimeStamp.Enabled = true;
                                btnToCoOrientation.Visible = true; btnToCoOrientation.Enabled = true;
                                //-------Misc
                                btnClientSetup.Visible = true; btnClientSetup.Enabled = true;
                                btnToCoBattery.Visible = true; btnToCoBattery.Enabled = true;
                                btnBGService.Visible = true; btnBGService.Enabled = true;

                                btnStopSurvey.Visible = false;
                                btn_ConnectSerial.Enabled = true;
                                break;
                            }
                        //--------------------------------------------------------------------------------------
                        default:        //Reset (0xFF)
                            {
                                break;
                            }
                    }
                }
            }
            catch
            {
            }

        }
        #endregion

        #region //================================================================BGSurvey_ToolA_Enable_Click
        private void BGSurvey_ToolA_Enable_Click(object sender, EventArgs e)
        {
            mbOperationBusy.PopUpMessageBox("ERROR: Cannot be unchecked!, Tool A alway enabled", "Single or Dual Tools", 5, 12F);
            BGSurvey_ToolA_Enable.Checked = true;
        }
        #endregion

        #region //==================================================BGDrilling_Start_Survey
        public delegate void BGDrilling_Start_Survey_StartDelegate();
        private void BGDrilling_Start_Survey()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new BGDrilling_Start_Survey_StartDelegate(BGDrilling_Start_Survey), null);
                return;
            }
            try
            {
                FlexiMessageBox.FONT = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                FlexiMessageBox.Show("The tool is now in Survey/Logging Mode"
                + Environment.NewLine + "STEP 1: Check ':' response to confirm Survey Mode (wait 20 second)."
                + Environment.NewLine + "STEP 2: Disconnect Serial Lemo cable"
                + Environment.NewLine + "STEP 3: Click OK to close this window",
                "Survey Mode",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
            catch { }

        }
        #endregion

        #region //==================================================zBG_Async_EE_ISEMPTY
        //Async: ~BATTERY(ToolStatus; VRAWReadout; IRAWReadout; V3V3Readout; CapacityLeft; SpecCapacity; SpecVoltage)\n
        public void zBG_Async_EE_ISEMPTY(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage, IList<string> Asynclistparameter)
        {
            Thread.Sleep(200);      // Short pause for comm to settle down. 
            try
            {
                if (hPara[0] == 0)
                    isSurveyEEEmpty = false;
                else
                    isSurveyEEEmpty = true;
            }
            catch { }
        }
        #endregion

        #region //==================================================zBG_Async_RTC_CHECK
        //Async: ~BATTERY(ToolStatus; VRAWReadout; IRAWReadout; V3V3Readout; CapacityLeft; SpecCapacity; SpecVoltage)\n
        public void zBG_Async_RTC_CHECK(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage, IList<string> Asynclistparameter)
        {
            Thread.Sleep(200);      // Short pause for comm to settle down. 
            try
            {
                if (hPara[0] == 0)
                    isSurveyRTCOkay = false;
                else
                    isSurveyRTCOkay = true;
            }
            catch { }
        }
        #endregion

        #region //==================================================btnBGDriller_Click
        private void btnBGDriller_Click(object sender, EventArgs e)
        {
            myBGDriller.BG_Driller_Show();
        }

        #endregion

        #region //==================================================btnBGSurveyResults_Click
        private void btnBGSurveyResults_Click(object sender, EventArgs e)
        {


        }

        #endregion

        #region //==================================================btnEepromTools_Click_1
        private void btnEepromTools_Click_1(object sender, EventArgs e)
        {
            if (debugModeToolStripMenuItem.Checked == false)
            {
                //if (myGlobalBase.is_Serial_Server_Connected == false)
                if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == false)
                {
                    FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    FlexiMessageBox.COLOR = Color.Red;
                    FlexiMessageBox.Show("The Tool and PC has no serial interface, please check cable.",
                    "Tool/PC Serial Connection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                    return;
                }
            }
            myEEpromTools.EEPROMTools_Show();
        }
        #endregion

        #region //==================================================cbSelectMathSL_Click
        private void cbSelectMathSL_Click(object sender, EventArgs e)
        {
            cbSelectMathSL.Checked = true;
            cbSelectMathRGP.Checked = false;
        }
        #endregion

        #region //==================================================cbSelectMathRGP_Click
        private void cbSelectMathRGP_Click(object sender, EventArgs e)
        {
            cbSelectMathSL.Checked = false;
            cbSelectMathRGP.Checked = true;
        }
        #endregion

        #region //==================================================BGSurvey_ShowReportViewer_Click
        private void BGSurvey_ShowReportViewer_Click(object sender, EventArgs e)
        {
            myBGReportViewer.isEnableShowWindow = true;
            myBGReportViewer.Report_Show();
            myBGReportViewer.isEnableShowWindow = false;
        }
        #endregion

        #region //==================================================testCodeSLCalToolStripMenuItem_Click
        BGAccelMath_MRC myTestMath;
        private void testCodeSLCalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myRtbTermMessageLF("#I: Running the Test Code, use debug console for results readout");
            //---------------------------------------------------------------
            if (myTestMath == null)
            {
                myTestMath = new BGAccelMath_MRC();
            }
            //---------------------------------------------------------------
            //myTestMath.AccelProcessMath_SL_Method_Test();
            //---------------------------------------------------------------
        }
        #endregion

        #endregion
        //##############################################################################################################
        #region//=============================================================================================EEPROM Export/Import CVS (Rev 53)
        //##############################################################################################################

        #region //==================================================btnEEPROMImportCVS_Click
        private void btnEEPROMImportCVS_Click(object sender, EventArgs e)
        {
            myEEPROMExpImport.EEPROM_ButtonClicked_ImportDataNow();
        }
        #endregion

        #region //==================================================btnEEPROMExportCVS_Click
        private void btnEEPROMExportCVS_Click(object sender, EventArgs e)
        {
            myRtbTermMessageLF("-I: EEPROM Export Mode Procedure Start");
            myEEPROMExpImport.EEPROM_ButtonClicked_ExportDataNow();
        }
        #endregion

        #region //==================================================BtnOpenFolder_Click
        private void BtnOpenFolder_Click(object sender, EventArgs e)        // Open Folder and Explore
        {
            myRtbTermMessageLF("-I: EEPROM Import Mode Procedure Start");
            myEEPROMExpImport.EEPROM_ExploreFolder();
        }
        #endregion

        #region //==================================================btnEEIDebugExport_Click
        private void btnEEIDebugExport_Click(object sender, EventArgs e)
        {
            myGlobalBase.isBGEEEXPORTActivate = true;
            myGlobalBase.isBGEEIMPORTActivate = false;
            myRtbTermMessageLF("-I: EEPROM EXPORT MODE Activated for DEBUG only");
        }
        #endregion

        #region //==================================================btnEEIDebugImport_Click
        private void btnEEIDebugImport_Click(object sender, EventArgs e)
        {
            myGlobalBase.isBGEEEXPORTActivate = false;
            myGlobalBase.isBGEEIMPORTActivate = true;
            myRtbTermMessageLF("-I: EEPROM IMPORT MODE Activated for DEBUG Only");
        }
        #endregion

        #region //==================================================btnBG2_Erase_Click
        private void btnBG2_Erase_Click(object sender, EventArgs e)
        {
            myGlobalBase.isBGEEEXPORTActivate = true;
            myGlobalBase.isBGEEIMPORTActivate = false;
            myEEPROMExpImport.EEPROM_ButtonClicked_EraseOnly();
        }
        #endregion

        #endregion

        //##############################################################################################################
        #region//=============================================================================================Res Calculator Tab (Rev 57)
        //##############################################################################################################

        #region //=============================================================================================btnRecCalc_ProcessNow_Click
        private void btnRecCalc_ProcessNow_Click(object sender, EventArgs e)
        {
            //--------------------------------------------------------- Text to double data

            double dResCalcVIN = Tools.ConversionStringtoDouble(tbVIN.Text);
            double dResCalcVOUT = Tools.ConversionStringtoDouble(tbVOUT.Text);
            double dResCalcTol = Tools.ConversionStringtoDouble(tbTol.Text);

            if (dResCalcVIN == Double.NaN)
            {
                myRtbTermMessageLF("#ERR: ResCal VIN is not a number, check entry and try again");
                return;
            }
            if (dResCalcVOUT == Double.NaN)
            {
                myRtbTermMessageLF("#ERR: ResCal VIN is not a number, check entry and try again");
                return;
            }
            if (dResCalcTol == Double.NaN)
            {
                myRtbTermMessageLF("#ERR: ResCal Tol is not a number, check entry and try again");
                return;
            }
            //--------------------------------------------------------- 
            int Rtop, Rbot;
            double dR1, dR2, dVout, dTol;
            double[] ArrayRes;

            if (cbE12.Checked == true)
                ArrayRes = new double[] { 1.0, 1.5, 2.2, 3.3, 4.7, 6.8 };
            else if (cbE24.Checked == true)
                ArrayRes = new double[] { 1.0, 1.1, 1.2, 1.3, 1.5, 1.8, 2.0, 2.2, 2.4, 2.7, 3.0, 3.3, 3.6, 3.9, 4.3, 4.7, 5.1, 5.6, 6.2, 6.8, 7.5, 8.2, 9.1 };
            else if (cbE48.Checked == true)
                ArrayRes = new double[] { 1, 1.05, 1.1, 1.15, 1.21, 1.27, 1.33, 1.4, 1.47, 1.54, 1.62, 1.69, 1.78, 1.87, 1.96, 2.05, 2.15, 2.26, 2.37, 2.49, 2.61, 2.74, 2.87, 3.01, 3.16, 3.32, 3.48, 3.65, 3.83, 4.02, 4.22, 4.42, 4.64, 4.87, 5.11, 5.36, 5.62, 5.9, 6.19, 6.49, 6.81, 7.15, 7.5, 7.87, 8.25, 8.66, 9.09, 9.53 };
            else
                ArrayRes = new double[] { 1, 1.02, 1.05, 1.07, 1.1, 1.13, 1.15, 1.18, 1.21, 1.24, 1.27, 1.3, 1.33, 1.37, 1.4, 1.43, 1.47, 1.5, 1.54, 1.58, 1.62, 1.65, 1.69, 1.74, 1.78, 1.82, 1.87, 1.91, 1.96, 2, 2.05, 2.1, 2.16, 2.21, 2.26, 2.32, 2.37, 2.43, 2.49, 2.55, 2.61, 2.67, 2.74, 2.8, 2.87, 2.94, 3.01, 3.09, 3.16, 3.24, 3.32, 3.4, 3.48, 3.57, 3.65, 3.74, 3.83, 3.92, 4.02, 4.12, 4.22, 4.32, 4.42, 4.53, 4.64, 4.75, 4.87, 4.99, 5.11, 5.23, 5.36, 5.49, 5.62, 5.76, 5.9, 6.04, 6.19, 6.34, 6.49, 6.65, 6.81, 6.98, 7.15, 7.32, 7.5, 7.68, 7.87, 8.06, 8.25, 8.45, 8.66, 8.87, 9.09, 9.31, 9.53, 9.76 };

            List<ResCalc_ResultTable> ResTable = new List<ResCalc_ResultTable>();
            //--------------------------------------------------------- Populate Resistor table based on E selection and decades
            List<double> ResList = new List<double>();

            if (cbResCalc1R.Checked == true)
            {
                for (int i = 0; i < ArrayRes.Length; i++)
                {
                    ResList.Add(ArrayRes[i]);
                }
            }
            if (cbResCalc10R.Checked == true)
            {
                for (int i = 0; i < ArrayRes.Length; i++)
                {
                    ResList.Add(ArrayRes[i] * 10);
                }
            }
            if (cbResCalc100R.Checked == true)
            {
                for (int i = 0; i < ArrayRes.Length; i++)
                {
                    ResList.Add(ArrayRes[i] * 100);
                }
            }
            if (cbResCalc1K.Checked == true)
            {
                for (int i = 0; i < ArrayRes.Length; i++)
                {
                    ResList.Add(ArrayRes[i] * 1000);
                }
            }
            if (cbResCalc10K.Checked == true)
            {
                for (int i = 0; i < ArrayRes.Length; i++)
                {
                    ResList.Add(ArrayRes[i] * 10000);
                }
            }
            if (cbResCalc100K.Checked == true)
            {
                for (int i = 0; i < ArrayRes.Length; i++)
                {
                    ResList.Add(ArrayRes[i] * 100000);
                }
            }
            if (cbResCalc1M.Checked == true)
            {
                for (int i = 0; i < ArrayRes.Length; i++)
                {
                    ResList.Add(ArrayRes[i] * 1000000);
                }
            }
            if (cbResCalc10M.Checked == true)
            {
                for (int i = 0; i < ArrayRes.Length; i++)
                {
                    ResList.Add(ArrayRes[i] * 10000000);
                }
            }
            //----------------------------------------------------Do calculation and if within tolerance spec, add list table.
            for (Rtop = 0; Rtop < ResList.Count; Rtop++)       //R1 Loop
            {
                for (Rbot = 0; Rbot < ResList.Count; Rbot++)   //R2 Loop
                {
                    dR1 = ResList[Rtop];
                    dR2 = ResList[Rbot];
                    dVout = dResCalcVIN * dR2 / (dR1 + dR2);
                    dTol = dResCalcVOUT - dVout;
                    if ((dTol < (-dResCalcTol)) | (dTol > dResCalcTol))
                    {
                        // ResTable.Add(new ResCalc_ResultTable(dR1, dR2, dTol, dVout, false));
                    }
                    else
                    {
                        ResTable.Add(new ResCalc_ResultTable(dR1, dR2, dTol, dVout));
                    }
                }
            }
            List<ResCalc_ResultTable> SortedList = ResTable.OrderBy(o => o.dTolerance).ToList();

            BindingList<ResCalc_ResultTable> blResCalcTable;

            blResCalcTable = new BindingList<ResCalc_ResultTable>(SortedList);
            ResCalc_DGV_Result myResCalc = new ResCalc_DGV_Result();
            myResCalc.MyGlobalBase(myGlobalBase);
            myResCalc.MyResCalcResultTable(blResCalcTable);
            myResCalc.ShowDialog();     // Do Modal
        }
        #endregion

        #region//=============================================================================================ResCalc: Select E12, E24, E48, E96
        private void cbE12_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.CheckState == CheckState.Checked)
            {
                checkboxSelect(cb.Name);
            }

        }

        private void cbE24_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.CheckState == CheckState.Checked)
            {
                checkboxSelect(cb.Name);
            }
        }

        private void cbE48_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.CheckState == CheckState.Checked)
            {
                checkboxSelect(cb.Name);
            }
        }

        private void cbE96_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.CheckState == CheckState.Checked)
            {
                checkboxSelect(cb.Name);
            }
        }

        //Function that will check the state of all checkbox inside the groupbox
        private void checkboxSelect(string selectedCB)
        {
            foreach (Control ctrl in gbResCalcEGroup.Controls)
            {
                if (ctrl.Name != selectedCB)
                {
                    CheckBox cb = (CheckBox)ctrl;
                    cb.Checked = false;
                }
            }
        }

        #endregion

        #region//=============================================================================================ResCalc_ResultTable Class
        //###################################################################################### Class Mag_Data
        [Serializable]
        public class ResCalc_ResultTable
        {
            //======================================================Getter/Setter f===         
            public double dR1 { get; set; }
            public double dR2 { get; set; }
            public double dTolerance { get; set; }
            public double dVout { get; set; }
            // =====================================================constructor
            public ResCalc_ResultTable(double R1, double R2, double Tol, double Vout)
            {
                dR1 = R1;
                dR2 = R2;
                dTolerance = Tol;
                dVout = Vout;
            }
        }

        #endregion

        #endregion

        //##############################################################################################################
        #region//=============================================================================================USB Array Option (Rev 71)
        //##############################################################################################################
        #region //=============================================================================================btnUSBArrayList_Click
        private void btnUSBArrayList_Click(object sender, EventArgs e)
        {
            myRtbTermMessageLF("#E: This feature not yet supported, WIP, it display list of active ports");
        }
        #endregion

        #region //=============================================================================================cbUSBOption_EnableMode_CheckedChanged
        private void cbUSBOption_EnableMode_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUSBOption_EnableMode.Checked == true)
            {
                myGlobalBase.USBArray_RedirectDebugTerm = cbUSBOption_SelectArray.SelectedIndex;
                //if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == true)
                //   myRtbTermMessageLF("-I: All UDT Debug Message now connected to generic USB channel (Active COM Port)");
                //else
                //    myRtbTermMessageLF("+W: All UDT Debug Message now connected to generic USB channel but has no COM Port");
            }
            else
            {
                myGlobalBase.USBArray_RedirectDebugTerm = -1;

            }
            USBOption_UpdateTabPageSection();
        }
        #endregion

        #region //=============================================================================================cbUSBOption_ShowSync_CheckedChanged
        private void cbUSBOption_SelectArray_SelectedIndexChanged(object sender, EventArgs e)
        {
            myGlobalBase.USBArray_RedirectDebugTerm = cbUSBOption_SelectArray.SelectedIndex;
        }
        #endregion

        #region //=============================================================================================cbUSBOption_ShowSync_CheckedChanged
        private void cbUSBOption_ShowSync_CheckedChanged(object sender, EventArgs e)
        {
            myGlobalBase.UDTerm_DMFP_ShowSyncCallBack = cbUSBOption_ShowSYNC.Checked;
        }
        #endregion

        #region //=============================================================================================USBOption_UpdateTabPageSection
        private void USBOption_UpdateTabPageSection()
        {
            if (myGlobalBase.USBArray_RedirectDebugTerm == -1)
            {
                cbUSBOption_SelectArray.SelectedIndex = 0;
                cbUSBOption_EnableMode.Checked = false;
                cbUSBOption_ShowSYNC.Enabled = false;
                cbUSBOption_ShowSYNC.Checked = false;
            }
            else
            {
                cbUSBOption_SelectArray.SelectedIndex = myGlobalBase.USBArray_RedirectDebugTerm;
                cbUSBOption_EnableMode.Checked = true;
                cbUSBOption_ShowSYNC.Enabled = true;
                cbUSBOption_ShowSYNC.Checked = myGlobalBase.UDTerm_DMFP_ShowSyncCallBack;
            }
        }
        #endregion

        #region//=============================================================================================cbUSB_AutoDetect_CheckedChanged
        private void cbUSB_AutoDetect_CheckedChanged(object sender, EventArgs e)
        {
            myGlobalBase.configb[0].bOptionAutoDetectEnable = cbUSB_AutoDetect.Checked;
        }
        #endregion
        #endregion

        //##############################################################################################################
        #region//=============================================================================================BG Survey Result
        //##############################################################################################################

        #endregion

        //##############################################################################################################
        #region//=============================================================================================MiniAD7794 (Rev 71)
        //##############################################################################################################

        #region//=============================================================================================MiniAD7794_Init
        private void MiniAD7794_Init()      // Run only once.
        {
            if (myMiniSetup == null)
            {
                myMiniSetup = new MiniAD7794Setup();
                myMiniSetup.MyGlobalBase(myGlobalBase);
                myMiniSetup.MyUSBVCOMComm(myUSB_VCOM_Comm);
                myMiniSetup.MyMainProg(this);
                myMiniSetup.MyLoggerCVS(myLoggerCSV);
                myMiniSetup.MyDMFProtocol(myDMFProtocol);
                myMiniSetup.MyUSB_Message_Manager(myUSB_Message_Manager);
                myADIS16460 = myMiniSetup.MyADIS16460();
                myDMFProtocol.MyMiniADIS1640(myADIS16460);
                myLoggerCSV.MyMiniAD7794Setup(myMiniSetup);
            }
            MiniAD7794_WindowControlState(false);
            btnAD7794TestOnly.Visible = true;
        }
        #endregion

        #region //=============================================================================================btnMiniSetup_Click
        private void btnMiniSetup_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                FlexiMessageBox.COLOR = Color.Red;
                DialogResult response = FlexiMessageBox.Show("MiniAD7794 is not plugged in or connected."
                + Environment.NewLine + "OK     = Attempt to connect to MiniAD7794 Module."
                + Environment.NewLine + "Cancel = Just open MiniAD7794 Setup Window",
                "MiniAD7794 USB Connection",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);
                if (response == DialogResult.OK)
                    btnMiniConnect.PerformClick();
            }
            myMiniSetup.MiniSetup_Show();
        }
        #endregion

        #region//=============================================================================================btnMiniConnect_Click
        private void btnMiniConnect_Click(object sender, EventArgs e)
        {
            mbOperationBusy = null;
            mbOperationBusy = new DialogSupport();
            myGlobalBase.USB_SelectDeviceMode = (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM;
            myGlobalBase.is_MiniPort_Device_Activated = false;
            if (btnMiniConnect.Text == "Connect")
            {
                if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == true)
                {
                    mbOperationBusy.PopUpMessageBox("MiniAD7794's USB was left Open, Auto Close and Try Again.", " MiniAD7794 USB Connection", (2));
                    myUSB_VCOM_Comm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.MiniAD7794);
                    myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected = false;
                    MiniAD7794_WindowControlState(false);
                    return;
                }
                //-----------------------------------SCAN for PORT
                myUSBComPortManager.ScanPortNowArray((int)eUSBDeviceType.MiniAD7794, false);
                if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
                {
                    try
                    {
                        myUSBComPortManager.ShowDialog();       // Better to use ShowDialogue in case not able to select device.
                    }
                    catch (System.Exception ex)
                    {
                        myRtbTermMessageLF("#E: Internal Code Catch Bug: myUSBComPortManager.ShowDialog() within btnMiniConnect_Click(), try fix this\n" + ex.ToString());
                        MiniAD7794_WindowControlState(false);
                        return;
                    }
                    if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == true)
                    {
                        mbOperationBusy.PopUpMessageBox("MiniAD7794's USB is now connected.", " MiniAD7794 USB Connection", (2));     // Close automatically after 2 Sec
                        MiniAD7794_WindowControlState(true);
                        myGlobalBase.Svy_isSurveyMode = false;
                        myGlobalBase.is_MiniPort_Device_Activated = true;
                    }
                    else
                    {
                        mbOperationBusy.PopUpMessageBox("Error!: MiniAD7794's USB unable to open, disconnect/connect device and try again", " MiniAD7794 USB Connection", (2));
                        MiniAD7794_WindowControlState(false);
                        return;
                    }
                }
            }
            else // This section where the port is closed. 
            {
                myGlobalBase.LoggerCVSMiniPortEnable = false;
                if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
                {
                    myRtbTermMessageLF("#ERR: What MiniAD7794 COM/FTDI Port?, there nothing to close!\n");
                    MiniAD7794_WindowControlState(false);
                }
                else
                {
                    myRtbTermMessageLF("-INFO: Closing MiniAD7794 USB/UART Port, please wait few seconds until finally closed\n");
                    myUSB_VCOM_Comm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.MiniAD7794);
                    myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected = false;
                    MiniAD7794_WindowControlState(false);
                }
            }
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == true)
            {
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isDMFProtocolEnabled = true;
                Tools.InteractivePause(TimeSpan.FromSeconds(2));
                //-----------------------------------Setup call-back delegate. 
                DMFP_Delegate fpCallBack_MINIPINGB = new DMFP_Delegate(DMFP_MiniAD7794_MINIPINGB_CallBack);
                myDMFProtocol.DMFP_Process_Command(fpCallBack_MINIPINGB,
                    "+MINIPING()",
                    true, (int)eUSBDeviceType.MiniAD7794);
            }
        }
        #endregion

        #region //=============================================================================================DMFP_MiniAD7794_MINITWIK_CallBack  
        public void DMFP_MiniAD7794_MINIPINGB_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xF)       // 0xFFFF mean no error.
            {
                myRtbTermMessageLF("#E: +MINIPING() Connection Issue. Auto-Close and please Connect. Code: 0x" + hPara[0].ToString("X"));
                myUSB_VCOM_Comm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.MiniAD7794);
                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected = false;
                MiniAD7794_WindowControlState(false);
            }
            else
            {
                myRtbTermMessageLF("-I: Passed +MINIPING() Test");
            }
        }
        #endregion

        #region//=============================================================================================MiniAD7794_WindowControlState
        private void MiniAD7794_WindowControlState(bool ConnectMode)        // false = Not connected, true = connected.
        {
            if (ConnectMode == false)
            {
                btnMiniSetup.Enabled = false;
                btnMiniConnect.Enabled = true;
                btnAD7794TestOnly.Visible = false;
                btnMiniConnect.Text = "Connect";
            }
            else
            {
                btnMiniSetup.Enabled = true;
                btnMiniConnect.Enabled = true;
                btnAD7794TestOnly.Visible = true;
                btnMiniConnect.Text = "Close Port";
            }
        }
        #endregion

        #region//=============================================================================================btnAD7794TestOnly_Click
        private void btnAD7794TestOnly_Click(object sender, EventArgs e)        // Only for development purpose, not for actual connection to device. Delete this later.
        {
            MiniAD7794_WindowControlState(true);
            btnMiniConnect.Enabled = false;
            myGlobalBase.Svy_isSurveyMode = false;
            myGlobalBase.is_MiniPort_Device_Activated = true;
            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected = true;
            myMiniSetup.MiniSetup_Show();

        }


        #endregion
        #endregion

        //##############################################################################################################

        #region//=============================================================================================ERR_REPORT (Rev 75)
        //##############################################################################################################
        //Async: ~BATTERY(ToolStatus; VRAWReadout; IRAWReadout; V3V3Readout; CapacityLeft; SpecCapacity; SpecVoltage)\n
        public void zBG_Async_ERR_REPORT(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage, IList<string> Asynclistparameter)
        {
            int i;
            for (i = 0; i < Err_Report.iErrReport.Count; i++)
            {
                if (Err_Report.iErrReport[i].ErrNumber == hPara[0])
                {
                    mbOperationBusy.PopUpMessageBox("No:" + Err_Report.iErrReport[i].ErrNumber + " : " + Err_Report.iErrReport[i].ErrString, " ASYNC Tool Report", (10));     // Close automatically after 2 Sec
                    break;
                }
            }
        }
        #endregion

        #region//=============================================================================================btnSTCSTART_SendDateTime_Click (Rev 76)
        private void btnSTCSTART_SendDateTime_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.is_Serial_Server_Connected == true)
            {
                DateTime dt2 = DateTime.Now;
                DateTime dt = dt2.ToLocalTime();
                string UDT = Tools.sDateTimeToUnixTimestamp(dt);
                UDT = UDT.Substring(0, UDT.IndexOf("."));
                string m_sEntryTxt = "STC_START(0xF;" + UDT + ";10)";     // 0xF applied only for date time update protocol via unix date time format. 
                Terminal_Append_Response(m_sEntryTxt + '\n');
                //myUSB_Message_Manager.endoflinedetected = false;
                Command_ASCII_Process_UARTs(m_sEntryTxt);
            }
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Session Log Section
        //##############################################################################################################

        #region//=============================================================================================btnSessionHelp_Click (Rev 65)
        private void btnSessionHelp_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            TextBox tbx = new TextBox();
            Font font = new Font("Consolas", 10.0f, FontStyle.Regular);
            tbx.Font = font;
            tbx.Height = 530;
            tbx.Width = 600;
            frm.Height = 540;
            frm.Width = 610;
            frm.Controls.Add(tbx);
            tbx.Multiline = true;
            tbx.ScrollBars = ScrollBars.Vertical;
            tbx.AppendText(" Help Section for Session Log (Stealth and Session Frame) \r\n");
            tbx.AppendText("---------------------------------------------------------------------\r\n");
            tbx.AppendText(" Session log (Debug) has two element: Session and Stealth\r\n");
            tbx.AppendText(" Stealth is used on MODBUS, Address, Function, Data[] and CRC[2]\r\n");
            tbx.AppendText(" Session is text based reporting,debug, detailing activity/error\r\n");
            tbx.AppendText(" Both is enabled by <Session On> Btn [SESSION(1;UDT)]\r\n");
            tbx.AppendText(" Both is disable by <Session Off> Btn [SESSION(0)]...\r\n");
            tbx.AppendText("                ..which free up MCU resource by skipping this feature.\r\n");
            tbx.AppendText(" Within MCU: By POR default the session is OFF\r\n");
            tbx.AppendText(" NB: Both Session/Stealth is ASCII Based which is displayed and\r\n");
            tbx.AppendText(" optionally save into designated folder name (two filename).\r\n");
            tbx.AppendText("---------------------------------------------------------------------\r\n");
            tbx.AppendText(" Note: Not to be confused with UDT as Universal Debug Terminal.r\n");
            tbx.AppendText(" UDT: is a Unix Date Time Epoch (1/1/1970). Info via search/website\r\n");
            tbx.AppendText("      When SESSION(1,UDT) is sent, The UDT override datetime within\r\n");
            tbx.AppendText("      MCU and assumed have working clock solution (ideally with XTAL). r\n");
            tbx.AppendText("      This UDT is inserted for each Stealth/Session frame out. r\n");
            tbx.AppendText("---------------------------------------------------------------------\r\n");
            tbx.AppendText(" If the folder name exist, it automatically save to filename in csv\r\n");
            tbx.AppendText(" ...otherwise it skip filename save, leaving to display only\r\n");
            tbx.AppendText("---------------------------------------------------------------------\r\n");
            tbx.AppendText(" Do the follow step: \r\n");
            tbx.AppendText(" (1) Run UDTermLite 65 and Select Session Tab in UDT term. \r\n");
            tbx.AppendText(" (2) Click <Folder> and chose folder to save session/stealth filename. \r\n");
            tbx.AppendText("     Or use Quick Folder at the top menu list, select folder and click\r\n");
            tbx.AppendText(" (3) Click <Session ON> which put slave MCU to Session log mode\r\n");
            tbx.AppendText("     and that it!\r\n");
            tbx.AppendText(" In case of poor performance, you can turn off via <Session Off> btn \r\n");
            tbx.AppendText("---------------------------------------------------------------------\r\n");
            tbx.AppendText(" Technical detail for Slave Embedded MCU\r\n");
            tbx.AppendText(" At the MCU side, there is protocol for this to work correctly\r\n");
            tbx.AppendText(" ~ prefix is a UDT's ASYNC protocol, UDT accept slave message any time.\r\n");
            tbx.AppendText(" (- and +) is a UDT's SYNC callback protocol with timeout (Not used here)\r\n");
            tbx.AppendText(" ~LH(.....) is a Stealth header type every 30 rows. It define columns. \r\n");
            tbx.AppendText(" ~L1(.....) is a stealth type steam which goes to Stealth filename\r\n");
            tbx.AppendText("            They carefully formatted to fit UDT width and have multiple\r\n");
            tbx.AppendText("            They carefully formatted to fit UDT width and have multiple\r\n");
            tbx.AppendText("            They carefully formatted to fit UDT width and have multiple\r\n");
            tbx.AppendText("            They carefully formatted to fit UDT width and have multiple\r\n");
            tbx.AppendText("            rows for longer msg to avoid wrap around by UDT terminal.\r\n");
            tbx.AppendText(" ~L2(.....) is a session type stream which goes to Session filename\r\n");
            tbx.AppendText("            NB: Keep Session message less than 230 Byte (limited buffer)\r\n");
            tbx.AppendText(" End of line format is '\\n' as per UDT Protocol standard\r\n");
            tbx.AppendText(" Within the (.....) can be any text msg in ASCII (since it Debug).\r\n");
            tbx.AppendText(" MCU must accept SESSION(...) to turn ON/OFF and Update DateTime\r\n");
            tbx.AppendText(" The ASCII msg can forms column style via ';' or '|' separator in excel. \r\n");
            tbx.AppendText(" (Do not use ',' comma, as this is reserved ASCII for UDT protocol)\r\n");
            tbx.AppendText(" The Session code depends on sprintf(...) to form msg buffer and posted \r\n");
            tbx.AppendText(" into selected serial port: USB/Ethernet/UART/RS422, etc. \r\n");
            tbx.AppendText(" In TM4C, use System_sprintf(..) but does not support %X, only %x.\r\n");
            tbx.AppendText("---------------------------------------------------------------------\r\n");
            tbx.AppendText(" IMPORTANT: Use UDTermLite 65 which has updated ASYNC protocol\r\n");
            tbx.AppendText("            which include multi-command stream buffering (~/+/- only)\r\n");
            tbx.AppendText("---------------------------------------------------------------------\r\n");
            tbx.AppendText(" Application: RFD/MODBUS \r\n");
            tbx.AppendText(" USB Port is MODBUS interface to RFDMB device. It RTU/Binary based\r\n");
            tbx.AppendText(" UART0 Port is debug comm interface to laptop (115K2). It ASCII based\r\n");
            tbx.AppendText(" Important: Consider using USB/UART isolator (1KV) on UART0\r\n");
            tbx.AppendText(" NB: USB Port is 10MHz and thus timing is non-standard to MODBUS-RTU\r\n");
            tbx.AppendText(" (End of message depend on t3.5 timeout (430uS). t1.5 is not supported)\r\n");
            tbx.AppendText(" NB: To send message use Hex Mode <04><12><F4A>, etc\r\n");
            tbx.AppendText("---------------------------------------------------------------------\r\n");
            tbx.AppendText(" Application: Generic, ie Accel Readout, ADC12, ADC16, etc \r\n");
            tbx.AppendText(" The Session is shared with UDT Command protocol via same UART/USB port\r\n");
            tbx.AppendText(" Take care the session can consume MCU resource, event spintf() is used\r\n");
            tbx.AppendText(" to pack message in array and then ported to USB/UART debug port\r\n");
            tbx.AppendText("---------------------------------------------------------------------\r\n");
            tbx.AppendText(" Session is best applied for 16 and 32 bit MCU with moderate resource\r\n");
            tbx.AppendText(" to process Session/Stealth message quickly and have sufficient RAM \r\n");
            tbx.AppendText(" due to buffer use, it can take up min 512 to 768 byte buffer \r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText("Revision UDTTermLite 65, (C) RPayne@TotalVision.pro (05-July-18).\r\n");
            frm.ShowDialog();

        }
        #endregion

        #region//=============================================================================================btnDataLogHelp_Click (Rev 65)
        private void btnDataLogHelp_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            TextBox tbx = new TextBox();
            Font font = new Font("Consolas", 10.0f, FontStyle.Regular);
            tbx.Font = font;
            tbx.Height = 530;
            tbx.Width = 620;
            frm.Height = 540;
            frm.Width = 630;
            frm.Controls.Add(tbx);
            tbx.Multiline = true;
            tbx.ScrollBars = ScrollBars.Vertical;
            tbx.AppendText("----------------------------------------------------------------------------\r\n");
            tbx.AppendText(" Help Section for Async Data Log Session\r\n");
            tbx.AppendText("----------------------------------------------------------------------------\r\n");
            tbx.AppendText(" Async DataLog Protocol: Programmer Guide\r\n");
            tbx.AppendText(" DataLog contains 5 elements: NB: All separator is semi-colon ';' only\r\n");
            tbx.AppendText(" ~LA(...)\\n : Start Frame create new timestamp filename and insert metadata\r\n");
            tbx.AppendText(" ~LB(...)\\n : Header name for column  saved under $H frame\r\n");
            tbx.AppendText(" ~LC(...)\\n : Format type for column, saved under $F frame\r\n");
            tbx.AppendText(" ~LD(...)\\n : 1st data frame          saved under $D frame\r\n");
            tbx.AppendText(" ~LE(...)\\n : 2nd data frame          saved under $G frame\r\n");
            tbx.AppendText(" ~LF(...)\\n : End Frame with metadata, ie counts. (see below)\r\n");
            tbx.AppendText(" NB: ~LA(...) and ~LF(...) include timestamp and is saved under $R (report)\r\n");
            tbx.AppendText("----------------------------------------------------------------------------\r\n");
            tbx.AppendText(" Purpose: DataLog is session data stream between MCU module and UDT terminal \r\n");
            tbx.AppendText("          When activated by <Session ON> and ~LA(...), it accept slave module \r\n");
            tbx.AppendText("          that send frames at any time (ASYNC operation) without UDT (master)\r\n");
            tbx.AppendText("          calling Slave to invoke data stream (SYNC). This ASYNC approach is\r\n");
            tbx.AppendText("          designed to keep Slave MCU busy and send stream at suitable timing.\r\n");
            tbx.AppendText("          In a way, the MCU module acts as master to UDT in a controlled way.\r\n");
            tbx.AppendText("          It support flexible data format type, column name and meta data. \r\n");
            tbx.AppendText("          It automatically save to timestamp filename in session folder in csv\r\n");
            tbx.AppendText("          The end frame has optional command to open FFT or Data Table, etc.\r\n");
            tbx.AppendText("          DataLog is more suited for bulk data transfer and does not replace\r\n");
            tbx.AppendText("          the LoggerCVS in ASYNC Mode, which has diag/test function role.\r\n");
            tbx.AppendText("          It can be used for FLASH data transfer into csv file.\r\n");
            tbx.AppendText("----------------------------------------------------------------------------\r\n");
            tbx.AppendText(" Start and End Frame  <~LA(...) and ~LF(...)>\r\n");
            tbx.AppendText(" To activate DataLog Session Mode, must alway start ~LA(...). It can be empty\r\n");
            tbx.AppendText(" or filled with meta data, useful to know where it come from, sample rate,\r\n");
            tbx.AppendText(" memory map, etc, etc. The End Frame ~LF(...) terminate session can include\r\n");
            tbx.AppendText(" post capture information, number of counts, end timestamp, etc\r\n");
            tbx.AppendText(" These meta data is essentially string, separated by ';' for excel splits.\r\n");
            tbx.AppendText(" The saved frames contains end terminator $E\\n inserted by UDT (Master).\r\n");
            tbx.AppendText("----------------------------------------------------------------------------\r\n");
            tbx.AppendText(" Data1 and Data2 Frame <~LD(..) and ~LE(..)> \r\n");
            tbx.AppendText(" The data frame is essentially string, separated by ';' allowing splits for \r\n");
            tbx.AppendText(" excel and UDT terminal to column structure. The data column is aligned to the \r\n");
            tbx.AppendText(" top format and header column. Each data frame with \\n terminator create\r\n");
            tbx.AppendText(" new row. Each data element is required to have prefix syntax in accordance to\r\n");
            tbx.AppendText(" format policy (see below) in sync with ~LC(..) format frame. This is required\r\n");
            tbx.AppendText(" for OPENFFT/OPENLOG to function correctly, otherwise leave out prefixes for \r\n");
            tbx.AppendText(" excel. UDT Term include provision for two data frame ~LD(..) and ~LE(..) which\r\n");
            tbx.AppendText(" is treated as separate data entity, ie it can be separated into two excel sheets.\r\n");
            tbx.AppendText(" Example: \r\n");
            tbx.AppendText("           Sent out ~LD(12/32/12;0sTestme;12657;0x3267;-1267)\\n from MCU\r\n");
            tbx.AppendText("           Save as  $D;14/32/12;0sTestme;12657;0x3267;-1267;$E\\n to filename\r\n");
            tbx.AppendText("----------------------------------------------------------------------------\r\n");
            tbx.AppendText(" Format Type Policy (~LC(...))\r\n");
            tbx.AppendText(" There is only one format frame which is aligned primary to ~LD(...). The \r\n");
            tbx.AppendText(" 2nd data frame may optionally aligned with ~LC(..) but free to do so otherwise\r\n");
            tbx.AppendText(" The following format supported: example \r\n");
            tbx.AppendText("           Sent out ~LC(0d;0s;0i;0i;0x;0x;0i;0i)\\n from MCU\r\n");
            tbx.AppendText("           Save as  $T;0d;0s;0i;0i;0x;0x;0i;0i;$E\\n to filename in UDT\r\n");
            tbx.AppendText("    0s = string, max 256 char\r\n");
            tbx.AppendText("    0u = unsigned int (uINT32)\r\n");
            tbx.AppendText("    0i = signed int   (INT32)\r\n");
            tbx.AppendText("    0x = hex data     (uINT32), if 0x missing, UDT insert it for you.\r\n");
            tbx.AppendText("    0y = date         (string)\r\n");
            tbx.AppendText("    0z = time         (string)\r\n");
            tbx.AppendText("    0d = double       (64 bits double, In UDT: future Work)\r\n");
            tbx.AppendText("    0f = float        (32 bits float,  In UDT: future Work)\r\n");
            tbx.AppendText("----------------------------------------------------------------------------\r\n");
            tbx.AppendText(" Header Frame Policy (~LB(...))\r\n");
            tbx.AppendText(" It is used as a title of the data column which is placed at the top in excel\r\n");
            tbx.AppendText(" The Header and Format Frame is required for proper detokenisation operation\r\n");
            tbx.AppendText(" Unlike Data Frame, they do not required format type prefixes\r\n");
            tbx.AppendText(" IMPORTANT: ~LB(...), ~LC(...) and ~LD(...) must be aligned each other!!\r\n");
            tbx.AppendText("----------------------------------------------------------------------------\r\n");
            tbx.AppendText(" Special Feature: End Frame invoke FFT or Data Table Window\r\n");
            tbx.AppendText(" if the ~LF(...) contains ..;0sOPENFFT;.. will open FFT window\r\n");
            tbx.AppendText(" if the ~LF(...) contains ..;0sOPENLOG;.. will open Data Table window (not LITE)\r\n");
            tbx.AppendText("----------------------------------------------------------------------------\r\n");
            tbx.AppendText(" Example with start frame \r\n");
            tbx.AppendText(" ~LA(0sThis is example message on RCV channel:; 1;0sADC16 Rate:;100;0sHz)\\n \r\n");
            tbx.AppendText(" ~LB(TimeStamp;Counter;ADC1;ADC2;isGood;Error)\\n            This is header type\r\n");
            tbx.AppendText(" ~LC(0s;0i;0x;0x;0i;0s)\\n                                   This is format type\r\n");
            tbx.AppendText(" ~LD(0s14/22/12;10023;0x34FE;0x6EE1;0;0sNone)\\n             This is data frame\r\n");
            tbx.AppendText(" ~LD(0s14/23/12;10024;0x3EEE;0x6EE0;0;0sNone)\\n             This is data frame\r\n");
            tbx.AppendText(" ~LD(0s14/24/12;10025;0x68E7;0x6EE3;0;0sGainErr)\\n          This is data frame\r\n");
            tbx.AppendText(" ~LD(0s14/25/12;10026;0x542A;0x6EE5;0;0sNone)\\n             This is data frame\r\n");
            tbx.AppendText(" ....... \r\n");
            tbx.AppendText(" ~LF(0sEnd of Operation Status;0x223F;0sOPENLOG)\\n          This is end frame\r\n");
            tbx.AppendText(" The above is saved into following filename for UDT and Excel application.\r\n");
            tbx.AppendText("  $R;0sThis is example message on RCV channel:; 1;0sADC16 Rate:;100;0sHz;$E\\n \r\n");
            tbx.AppendText("  $H;TimeStamp;0sCounter;ADC1;ADC2;isGood;Error;$E\\n \r\n");
            tbx.AppendText("  $T;0s;0i;0x;0x;0i;0s;$E\\n \r\n");
            tbx.AppendText("  $D;0s14/22/12;10023;0x34FE;0x6EE1;0;0sNone;$E\\n \r\n");
            tbx.AppendText("  $D;0s14/23/12;10024;0x3EEE;0x6EE0;0;0sNone;$E\\n \r\n");
            tbx.AppendText("  $D;0s14/24/12;10025;0x68E7;0x6EE3;0;0sGainErr;$E\\n \r\n");
            tbx.AppendText("  $D;0s14/25/12;10026;0x542A;0x6EE5;0;0sNone;4E\\n \r\n");
            tbx.AppendText(" ....... \r\n");
            tbx.AppendText("  $R;0sEnd of Operation Status;0x223F;0sOPENLOG;$E\\n \r\n");
            tbx.AppendText("----------------------------------------------------------------------------\r\n");
            frm.ShowDialog();

        }
        #endregion

        #region//=============================================================================================btnSessionSTCStartUDT_Click (Rev 65)
        private void btnSessionSTCStartUDT_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.is_Serial_Server_Connected == true)
            {
                DateTime dt2 = DateTime.Now;
                DateTime dt = dt2.ToLocalTime();
                string UDT = Tools.sDateTimeToUnixTimestamp(dt);
                UDT = UDT.Substring(0, UDT.IndexOf("."));
                string m_sEntryTxt = "STC_START(0xF;" + UDT + ";10)";     // 0xF applied only for date time update protocol via unix date time format. 
                myRtbTermMessageLF(m_sEntryTxt + '\n');
                //myUSB_Message_Manager.endoflinedetected = false;
                Command_ASCII_Process_UARTs(m_sEntryTxt);
            }
        }
        #endregion

        #region//=============================================================================================btnSessionON_Click (Rev 65)
        private void btnSessionON_Click(object sender, EventArgs e)
        {
            if ((txtSessionFolderName.Text == "") || (sSessionFoldername == ""))
            {
                MessageBox.Show("Error: Please setup Session Folder Name (or type NULL) and try again", "Folder Name Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            if ((myGlobalBase.is_Serial_Server_Connected == true) || (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == true))
            {
                DateTime dt2 = DateTime.Now;
                DateTime dt = dt2.ToLocalTime();
                string UDT = Tools.sDateTimeToUnixTimestamp(dt);
                UDT = UDT.Substring(0, UDT.IndexOf("."));
                m_sEntryTxt = "SESSION(0x1;" + UDT + ")";     //     Turn ON Session with UDT. 
                myRtbTermMessageLF(m_sEntryTxt + '\n');
                //myUSB_Message_Manager.endoflinedetected = false;
                Command_ASCII_Process_UART();
            }
            //----------------------------------------------------Survey Window
            if (cbDataLogToBGSurvey.Checked == true)
            {
                if (myGlobalBase.DataLog_isSurveyCVSOpen == false)
                {
                    mySurvey.Svy_Show(true);
                }
                myGlobalBase.DataLog_isSurveyCVSOpen = true;
                cbDataLogToBGSurvey.Enabled = false;
            }
            else
            {
                if (myGlobalBase.DataLog_isSurveyCVSOpen == true)
                {
                    mySurvey.Svy_Close(true);
                }
                myGlobalBase.DataLog_isSurveyCVSOpen = false;
                cbDataLogToBGSurvey.Enabled = true;
            }

        }
        #endregion

        #region//=============================================================================================btnSessionOFF_Click (Rev 65)
        private void btnSessionOFF_Click(object sender, EventArgs e)
        {
            if ((myGlobalBase.is_Serial_Server_Connected == true) || (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == true))
            {
                m_sEntryTxt = "SESSION(0x0)";     //     Turn OFF Session, No UDT needed.
                myRtbTermMessageLF(m_sEntryTxt + '\n');
                //myUSB_Message_Manager.endoflinedetected = false;
                Command_ASCII_Process_UART();
            }
            cbDataLogToBGSurvey.Enabled = true;
        }
        #endregion

        #region//=============================================================================================btnSessionOpenFolder_Click (Rev 65)
        private void btnSessionOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string myPath = sSessionFoldername;
                if (myPath == null)             // No filename path, quit. 
                    return;
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = myPath;
                prc.Start();
            }
            catch
            {
                myRtbTermMessageLF("#ERR: Open Folder has no folder/path or attempt to use restricted domains of Drive C");
            }
        }
        #endregion

        #region//=============================================================================================btnSessionFolder_Click (Rev 65)
        private void btnSessionFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                //
                // The user selected a folder and pressed the OK button.
                // We print the number of files found.
                //
                sSessionFoldername = folderBrowserDialog1.SelectedPath;
                if (!Directory.Exists(sSessionFoldername))
                {
                    DirectoryInfo di = Directory.CreateDirectory(sSessionFoldername);  // Create folder if not exist. 
                }
            }
            txtSessionFolderName.Text = sSessionFoldername;
            myGlobalBase.sSessionFoldername = sSessionFoldername;
            sSessionAppendFileName = System.IO.Path.Combine(sSessionFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_Session.csv"));
            sStealthAppendFileName = System.IO.Path.Combine(sSessionFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_Stealth.csv"));
            sDataAppendFileName = System.IO.Path.Combine(sSessionFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_Data.csv"));
        }
        #endregion

        #region//=============================================================================================Quick Folder Section (Rev 76P)
        private void tsmiFolder1_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in tsmiQuickFolder.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            sSessionFoldername = "C:\\ADTSessionLog";
            dSessionLogCommonFolderNameUpdate();
        }

        private void tsmiFolder2_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in tsmiQuickFolder.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            sSessionFoldername = "D:\\ADTSessionLog";
            dSessionLogCommonFolderNameUpdate();
        }

        private void tsmiFolder3_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in tsmiQuickFolder.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            sSessionFoldername = "E:\\ADTSessionLog";
            dSessionLogCommonFolderNameUpdate();
        }

        private void tsmiFolder4_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in tsmiQuickFolder.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            sSessionFoldername = "F:\\ADTSessionLog";
            dSessionLogCommonFolderNameUpdate();
        }

        private void tsmiFolder5_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in tsmiQuickFolder.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            sSessionFoldername = "G:\\ADTSessionLog";
            dSessionLogCommonFolderNameUpdate();
        }

        private void tsmiFolder6_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in tsmiQuickFolder.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            sSessionFoldername = "H:\\ADTSessionLog";
            dSessionLogCommonFolderNameUpdate();
        }

        private void tsmiFolder7_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in tsmiQuickFolder.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            sSessionFoldername = "I:\\ADTSessionLog";
            dSessionLogCommonFolderNameUpdate();
        }

        private void nULLNoLogSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in tsmiQuickFolder.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            sSessionFoldername = "NULL";
            dSessionLogCommonFolderNameUpdate();
        }

        private void TsmiFolder8_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in tsmiQuickFolder.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            sSessionFoldername = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\ADTSessionLog";      //C:\Users\Public\Documents
            dSessionLogCommonFolderNameUpdate();
        }

        #endregion

        #region//=============================================================================================btnSessionHelp_Click (Rev 76P)
        private void dSessionLogCommonFolderNameUpdate()
        {
            if (sSessionFoldername == "NULL")
            {
                myGlobalBase.sSessionFoldername = "NULL";
                sSessionAppendFileName = "NULL";
                sStealthAppendFileName = "NULL";
                return;
            }
            try
            {
                txtSessionFolderName.Text = sSessionFoldername;
                if (!Directory.Exists(sSessionFoldername))                                 // If folder does not exisit, create new one 
                {
                    DirectoryInfo di = Directory.CreateDirectory(sSessionFoldername);  // Create folder if not exist. 
                }
                txtSessionFolderName.Text = sSessionFoldername;
                myGlobalBase.sSessionFoldername = sSessionFoldername;
                sSessionAppendFileName = System.IO.Path.Combine(sSessionFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_Session.csv"));
                sStealthAppendFileName = System.IO.Path.Combine(sSessionFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_Stealth.csv"));
            }
            catch
            {
                myRtbTermMessageLF("#ERR: Folder/Drive Not Exist\n");
            }
        }
        #endregion

        #region//=============================================================================================AsyncSessionLogReciever_L1 (Rev 65)
        public void AsyncSessionLogReciever_L1(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdParameter, string FullMessage, IList<string> Asynclistparameter)
        {
            if ((sStealthAppendFileName == "") || (sStealthAppendFileName == "NULL"))
                return;
            string sTimeStampHeader = DateTime.Now.ToString("dd/MM/yyyy;HH:mm:ss");
            try
            {
                StreamWriter sw = null;
                try
                {
                    sTimeStampHeader = sTimeStampHeader + ";" + CmdParameter + "\n";
                    sw = new StreamWriter(sStealthAppendFileName, true, Encoding.ASCII);
                    sw.Write(sTimeStampHeader);                                     // Dataframe (assuming it has \n\0 terminator)
                    //myRtbTermMessageLF("-INFO : Appended Data to File: " + sFilenameII);
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
                myRtbTermMessageLF("#ERR: Unable to append file : " + sStealthAppendFileName + " Error Msg:" + X.ToString());
            }
        }
        #endregion

        #region//=============================================================================================AsyncSessionLogReciever_L2 (Rev 65)
        public void AsyncSessionLogReciever_L2(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdParameter, string FullMessage, IList<string> Asynclistparameter)
        {
            if ((sSessionAppendFileName == "") || (sSessionAppendFileName == "NULL"))
                return;
            //-------------------------------------------------------------------------------------
            string sTimeStampHeader = DateTime.Now.ToString("dd/MM/yyyy;HH:mm:ss");
            try
            {
                StreamWriter sw = null;
                try
                {
                    sTimeStampHeader = sTimeStampHeader + ";" + CmdParameter + "\n";
                    sw = new StreamWriter(sSessionAppendFileName, true, Encoding.ASCII);
                    sw.Write(sTimeStampHeader);                                     // Dataframe (assuming it has \n\0 terminator)
                    //myRtbTermMessageLF("-INFO : Appended Data to File: " + sFilenameII);
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
                myRtbTermMessageLF("#ERR: Unable to append file : " + sSessionAppendFileName + " Error Msg:" + X.ToString());
            }
        }
        #endregion

        #region//=============================================================================================AsyncSessionLogReciever_LH (Rev 65)
        public void AsyncSessionLogReciever_LH(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdParameter, string FullMessage, IList<string> Asynclistparameter)
        {
            if ((sStealthAppendFileName == "") || (sStealthAppendFileName == "NULL"))
                return;
            string sTimeStampHeader = DateTime.Now.ToString("          ;        ");
            try
            {
                StreamWriter sw = null;
                try
                {
                    sTimeStampHeader = sTimeStampHeader + ";" + CmdParameter + "\n";
                    sw = new StreamWriter(sStealthAppendFileName, true, Encoding.ASCII);
                    sw.Write(sTimeStampHeader);                                     // Dataframe (assuming it has \n\0 terminator)
                    //myRtbTermMessageLF("-INFO : Appended Data to File: " + sFilenameII);
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
                myRtbTermMessageLF("#ERR: Unable to append file : " + sStealthAppendFileName + " Error Msg:" + X.ToString());
            }
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Session Data Section
        //##############################################################################################################

        #region//=============================================================================================AsyncSessionLogReciever_LA (Rev 65)
        // LA format is Meta Data supporting LA to LF session. Alway run LA 
        // $R = Start Meta Data. (Report Frame)
        public void AsyncSessionLogReciever_LA(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdParameter, string FullMessage, IList<string> Asynclistparameter)
        {
            if ((sDataAppendFileName == "") || (sDataAppendFileName == "NULL"))
                return;
            //-------------------------------------------Clear buffer data.
            if (sSessionDataLog == null)
                sSessionDataLog = new List<string>();
            else
                sSessionDataLog.Clear();
            //-------------------------------------------Create new timestamped filename.
            sDataAppendFileName = System.IO.Path.Combine(sSessionFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_Data.csv"));
            //-------------------------------------------
            string sStartMetaDataFrame = "$R;" + DateTime.Now.ToString("dd/MM/yyyy;HH:mm:ss") + ";" + CmdParameter + ";$E\n";
            try
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(sDataAppendFileName, true, Encoding.ASCII);
                    sw.Write(sStartMetaDataFrame);                                     // Dataframe (assuming it has \n\0 terminator)
                    sSessionDataLog.Add(sStartMetaDataFrame);
                    //myRtbTermMessageLF("-INFO : Appended Data to File: " + sFilenameII);
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Dispose();
                    }
                }
                isSessionDataAppendEnabled = true;
                myRtbTermMessageLF("-INFO : Appended Data to File: " + sDataAppendFileName);
            }
            catch (Exception X)
            {
                myRtbTermMessageLF("#ERR: Unable to append file : " + sDataAppendFileName + " Error Msg:" + X.ToString());
                isSessionDataAppendEnabled = false;
            }

        }
        #endregion

        #region//=============================================================================================AsyncSessionLogReciever_LB (Rev 65)
        // // Header Frame for data column, same as UDT Logger Style. 
        //$H;DateTime;Temp;GryoX;GryoY;GryoZ;AccelX;AccelY;AccelZ;VelX;VelY;VelZ;DelAngX;DelAngY;DelAngZ;$E
        public void AsyncSessionLogReciever_LB(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdParameter, string FullMessage, IList<string> Asynclistparameter)
        {
            if ((sDataAppendFileName == "") || (sDataAppendFileName == "NULL"))
                return;
            if (isSessionDataAppendEnabled == false)
                return;
            string sHeaderFrame = "$H;" + CmdParameter + ";$E\n";
            try
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(sDataAppendFileName, true, Encoding.ASCII);
                    sw.Write(sHeaderFrame);                                     // Dataframe (assuming it has \n\0 terminator)
                    sSessionDataLog.Add(sHeaderFrame);
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
                myRtbTermMessageLF("#ERR: Unable to append file : " + sDataAppendFileName + " Error Msg:" + X.ToString());
            }

        }
        #endregion

        #region//=============================================================================================AsyncSessionLogReciever_LC (Rev 65)
        // Format Frame for data column, same as UDT Logger Style. This is optional. 
        // $T;0d;0i;0i;0i;0i;0i;0i;0i;0i;0i;0i;0i;0i;0i;$E
        public void AsyncSessionLogReciever_LC(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdParameter, string FullMessage, IList<string> Asynclistparameter)
        {
            if ((sDataAppendFileName == "") || (sDataAppendFileName == "NULL"))
                return;
            if (isSessionDataAppendEnabled == false)
                return;
            string sFormatFrame = "$T;" + CmdParameter + ";$E\n";
            try
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(sDataAppendFileName, true, Encoding.ASCII);
                    sw.Write(sFormatFrame);                                     // Dataframe (assuming it has \n\0 terminator)
                    sSessionDataLog.Add(sFormatFrame);
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
                myRtbTermMessageLF("#ERR: Unable to append file : " + sDataAppendFileName + " Error Msg:" + X.ToString());
            }
        }
        #endregion

        #region//=============================================================================================AsyncSessionLogReciever_LD (Rev 65)
        // Hex Data which is classified as $D stream. 
        // $D;-23789;23232;\n. 
        public void AsyncSessionLogReciever_LD(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdParameter, string FullMessage, IList<string> Asynclistparameter)
        {
            if ((sDataAppendFileName == "") || (sDataAppendFileName == "NULL"))
                return;
            if (isSessionDataAppendEnabled == false)
                return;
            string sDataFrameD = "$D;" + CmdParameter + ";$E\n";
            try
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(sDataAppendFileName, true, Encoding.ASCII);
                    sw.Write(sDataFrameD);                                     // Dataframe (assuming it has \n\0 terminator)
                    sSessionDataLog.Add(sDataFrameD);
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
                myRtbTermMessageLF("#ERR: Unable to append file : " + sDataAppendFileName + " Error Msg:" + X.ToString());
            }
        }
        #endregion

        #region//=============================================================================================AsyncSessionLogReciever_LE (Rev 65)
        // Hex Data which is classified as $G stream (secondary data) 
        // $G;-23789;23232;\n.
        public void AsyncSessionLogReciever_LE(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdParameter, string FullMessage, IList<string> Asynclistparameter)
        {
            if ((sDataAppendFileName == "") || (sDataAppendFileName == "NULL"))
                return;
            if (isSessionDataAppendEnabled == false)
                return;
            string sDataFrameG = "$G;" + CmdParameter + ";$E\n";
            try
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(sDataAppendFileName, true, Encoding.ASCII);
                    sw.Write(sDataFrameG);                                     // Dataframe (assuming it has \n\0 terminator)
                    sSessionDataLog.Add(sDataFrameG);
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
                myRtbTermMessageLF("#ERR: Unable to append file : " + sDataAppendFileName + " Error Msg:" + X.ToString());
            }

        }
        #endregion

        #region//=============================================================================================AsyncSessionLogReciever_LF (Rev 65)
        // LF is the closing session after all data is connected, it provide additional information, such as number of collected data. 
        // $R = Start Meta Data. (Report Frame)
        public void AsyncSessionLogReciever_LF(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdParameter, string FullMessage, IList<string> Asynclistparameter)
        {
            if ((sDataAppendFileName == "") || (sDataAppendFileName == "NULL"))
                return;
            string sEndMetaDataFrame = "$R;" + DateTime.Now.ToString("dd/MM/yyyy;HH:mm:ss") + ";" + CmdParameter + ";$E\n";
            try
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(sDataAppendFileName, true, Encoding.ASCII);
                    sw.Write(sEndMetaDataFrame);                                     // Dataframe (assuming it has \n\0 terminator)
                    sSessionDataLog.Add(sEndMetaDataFrame);
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
                myRtbTermMessageLF("#ERR: Unable to append file : " + sDataAppendFileName + " Error Msg:" + X.ToString());
            }
            //-------------------------------------------------------------------Now Open up Window. 
            if (CmdParameter.Contains("OPENFFT") != true)
            {
                myClientCode.myClientCode_PreStartSurveySetup();
                foreach (var sData in sSessionDataLog)
                {
                    myClientCode.myClientCodeRecievedData(sData);
                }
            }
            if (CmdParameter.Contains("OPENLOG") != true)
            {
                myClientCode.myClientCode_PreStartSurveySetup();
                foreach (var sData in sSessionDataLog)
                {
                    myClientCode.myClientCodeRecievedData(sData);
                }
            }
            isSessionDataAppendEnabled = false;
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================MODBUS CRC Add.
        //##############################################################################################################

        #region//=============================================================================================cbMODBUSCRC_CheckStateChanged (Rev 65)
        private void cbMODBUSCRC_CheckStateChanged(object sender, EventArgs e)
        {
            if (cbMODBUSCRC.Checked == true)
                myGlobalBase.bHexFormatModBusCRC = true;
            else
                myGlobalBase.bHexFormatModBusCRC = false;
        }
        #endregion

        #region //=============================================================================================hexDisplayModeToolStripMenuItem_Click
        // Note     :This is help Menu using frm and textbox. This need further work to size it and avoid form adjustment. 
        //          :This is simpler style compared to messagebox.
        private void btnHelpHexMode_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            TextBox tbx = new TextBox();
            frm.Controls.Add(tbx);
            tbx.Multiline = true;
            tbx.Height = frm.Height;
            tbx.Width = frm.Width;

            tbx.AppendText("Help Section for Hex based Display \r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("This feature allow display of receives data in hex form.\r\n");
            tbx.AppendText("So that non-ASCII data can be inspected. This is relevant\r\n");
            tbx.AppendText("for Short Hop Project where UART protocol (non-ASCII)\r\n");
            tbx.AppendText("can be validated or debugged\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText("(1) To enable this feature, check 'Display Hex'\r\n");
            tbx.AppendText("(2) 'Receives \n as LF': This detect 0x0A which add LF\r\n");
            tbx.AppendText("     NB: '~' is added after the 0A to signify this event\r\n");
            tbx.AppendText("(3) 'Send with \\r\\n': This added when enter is pressed\r\n");
            tbx.AppendText("(4) Tip: To avoid duplicate issue, use 'New Line' button\r\n");
            tbx.AppendText("(5) To enter hex byte, type <00><2F><A4><5F> and enter\r\n");
            tbx.AppendText("(6) To enter hex word, type <002F><A45F> and enter\r\n");
            tbx.AppendText("     NB: Only 8 or 16 bits is supported.\r\n");
            tbx.AppendText("Note: The codes is crafted to excludes unicode ASCII\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText("Revision 1D, RPayne (18-Feb-15)\r\n");
            frm.ShowDialog();
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================CRC8 Add.
        //##############################################################################################################
        // This is not for MODBUS Application. Test for ESM project.
        // This is genetic CRC8 check for any project. See ESM. 
        #region //=============================================================================================cRCTESTToolStripMenuItem_Click
        private void cRCTESTToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_CRCTEST = new DMFP_Delegate(DMFP_CRCTEST_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+CRCTEST()";
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t1 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_CRCTEST, sMessage, 250));
            t1.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
        }
        #endregion

        #region //=============================================================================================DMFP_CRCTEST_CallBack  
        public void DMFP_CRCTEST_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            int start = FullMessage.IndexOf('(') + 1; // Remove (
            int end = FullMessage.IndexOf(')');

            string sFrame = FullMessage.Substring(start, end - start);
            myCalc_CRC8.UDTMessageFrameIsPassedCRC8(sFrame);
            myRtbTermMessageLF("CRC8 Passed:" + myCalc_CRC8.CRC8PassCounter.ToString());
            myRtbTermMessageLF("CRC8 Failed:" + myCalc_CRC8.CRC8FailCounter.ToString());
        }
        #endregion

        #region //=============================================================================================cALLBACKToolStripMenuItem_Click  
        private void cALLBACKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_CALLBACK = new DMFP_Delegate(DMFP_CALLBACK_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+CALLBACK()";
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t1 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_CALLBACK, sMessage, 250));
            t1.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
        }
        #endregion

        #region //=============================================================================================DMFP_CALLBACK_CallBack  
        public void DMFP_CALLBACK_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            myRtbTermMessageLF("CALLBACK:" + FullMessage.ToString());

        }
        #endregion

        //##############################################################################################################
        //============================================================================================= SNAD Network
        //##############################################################################################################

        #region //=============================================================================================btnSNAD_Init_Click
        private void btnSNAD_Init_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.mySNADNetwork == null)
            {
                myGlobalBase.mySNADNetwork = new SNADNetwork();
                SNAD_UpdateLayout(0);
            }
            if (btnSNAD_Init.Text == "Init SNAD Network")
            {
                btnSNAD_Init.Text = "Cancel SNAD Network";
                //-----------------------------------Setup call-back delegate. 
                DMFP_Delegate fpCallBack_SNADInit = new DMFP_Delegate(DMFP_SNADInit_CallBack);
                //-----------------------------------Place Message to rtbTerm Window.
                string sMessage = "+SSETNODE(1)";
                if (myGlobalBase.bNoRFTermMessage == false)
                    myRtbTermMessageLF("#DMFP:" + sMessage);
                Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
                //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
                Thread t2 = null;
                if (myGlobalBase.is_Serial_Server_Connected == true)
                {
                    t2 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_SNADInit, sMessage, 250));
                }
                else
                {
                    myRtbTermMessageLF("#ERR: VCOM is not established: Do Scan and Connect first (NB:Array VCOM not supported)");
                    return;
                }
                t2.Start();
                Thread.Sleep(10);
                SNAD_UpdateLayout(1);
            }
            else
            {
                btnSNAD_Init.Text = "Init SNAD Network";
                //-----------------------------------Setup call-back delegate. 
                DMFP_Delegate fpCallBack_SNADCancel = new DMFP_Delegate(DMFP_SNADCancel_CallBack);
                //-----------------------------------Place Message to rtbTerm Window.
                string sMessage = "+SSCANCEL()1";
                if (myGlobalBase.bNoRFTermMessage == false)
                    myRtbTermMessageLF("#DMFP:" + sMessage);
                Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
                //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
                Thread t2 = null;
                if (myGlobalBase.is_Serial_Server_Connected == true)
                {
                    t2 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_SNADCancel, sMessage, 250));
                }
                else
                {
                    myRtbTermMessageLF("#ERR: VCOM is not established: Do Scan and Connect first (NB:Array VCOM not supported)");
                    return;
                }
                t2.Start();
                Thread.Sleep(10);
                myGlobalBase.mySNADNetwork.chMinRange = 1;
                myGlobalBase.mySNADNetwork.chMaxRange = 1;
                myGlobalBase.mySNADNetwork.isInitSuccess = false;
                myGlobalBase.mySNADNetwork.isNetworkEnabled = false;
                myGlobalBase.mySNADNetwork.ChannelSelect = -1;
                SNAD_UpdateLayout(0);
            }
        }
        #endregion

        #region //=============================================================================================DMFP_SNADCancel_CallBack
        public void DMFP_SNADCancel_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            myRtbTermMessageLF("---------------------------------------------------------------------------------");
            myRtbTermMessageLF("-INFO: SNAD Network is now canceled, UDT no longer issue SNAD number.");
            myRtbTermMessageLF("       The chain network will no longer function correctly, since ECHO is disabled");
            myRtbTermMessageLF("       Recommend to change chain network to one master- one slave basis.");
            myRtbTermMessageLF("---------------------------------------------------------------------------------");
        }
        #endregion

        #region //=============================================================================================DMFP_SNADInit_CallBack
        public void DMFP_SNADInit_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 SNADAdd = 0;
            //int comma = CmdMessage.IndexOf(')');       // Step 1 is to find ')'
            //char SNADAddHex = CmdMessage[comma + 1];
            if (hPara[0] <= 0xF)
            {
                SNADAdd = hPara[0] - 1;
                while (true)
                {
                    if (SNADAdd >= 1)
                        cbSNAD1.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 2)
                        cbSNAD2.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 3)
                        cbSNAD3.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 4)
                        cbSNAD4.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 5)
                        cbSNAD5.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 6)
                        cbSNAD6.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 7)
                        cbSNAD7.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 8)
                        cbSNAD8.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 9)
                        cbSNAD9.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 10)
                        cbSNADA.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 11)
                        cbSNADB.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 12)
                        cbSNADC.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 13)
                        cbSNADD.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 14)
                        cbSNADE.Visible = true;
                    else
                        break;
                    if (SNADAdd >= 15)
                        cbSNADF.Visible = true;
                    //--------------------------------------
                    break;
                }
                cbSNAD1.Checked = true;
                myGlobalBase.mySNADNetwork.chMinRange = 1;
                myGlobalBase.mySNADNetwork.chMaxRange = (int)SNADAdd;
                myGlobalBase.mySNADNetwork.isInitSuccess = true;
                myGlobalBase.mySNADNetwork.isNetworkEnabled = true;
                myGlobalBase.mySNADNetwork.ChannelSelect = 1;
                myRtbTermMessageLF("-------------------------------------------------------------------------------");
                myRtbTermMessageLF("-INFO: SNAD Network Activated with " + SNADAdd + " Detected Device");
                myRtbTermMessageLF("       UDT will now append SNAD number after ')' for any UDT commands protocol");
                myRtbTermMessageLF("       NB: without SNAD number, device will not accept UDT commands");
                myRtbTermMessageLF("-------------------------------------------------------------------------------");
            }
            else
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("ERROR!: SNAD Number missing or not within 1-F protocol policy", "SNAD ERROR", 10);
            }
        }
        #endregion

        #region //=============================================================================================SNAD_UpdateLayout
        private void SNAD_UpdateLayout(int Layout)
        {
            switch (Layout)
            {
                case (0):   // Power Up Default
                    {
                        lbSNADSelect.Text = "SEL:NULL";
                        btnSNAD_Disable.Enabled = false;
                        btnSNAD_Enable.Enabled = false;
                        btnSNAD_WhoID.Enabled = false;
                        cbSNAD1.Enabled = true; cbSNAD2.Enabled = true; cbSNAD3.Enabled = true; cbSNAD4.Enabled = true;
                        cbSNAD5.Enabled = true; cbSNAD6.Enabled = true; cbSNAD7.Enabled = true; cbSNAD8.Enabled = true;
                        cbSNAD9.Enabled = true; cbSNADA.Enabled = true; cbSNADB.Enabled = true; cbSNADC.Enabled = true;
                        cbSNADD.Enabled = true; cbSNADE.Enabled = true; cbSNADF.Enabled = true;
                        //--------------------------------------------------------------------------------------------------
                        cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                        cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                        cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                        cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        //--------------------------------------------------------------------------------------------------
                        cbSNAD1.Visible = false; cbSNAD2.Visible = false; cbSNAD3.Visible = false; cbSNAD4.Visible = false;
                        cbSNAD5.Visible = false; cbSNAD6.Visible = false; cbSNAD7.Visible = false; cbSNAD8.Visible = false;
                        cbSNAD9.Visible = false; cbSNADA.Visible = false; cbSNADB.Visible = false; cbSNADC.Visible = false;
                        cbSNADD.Visible = false; cbSNADE.Visible = false; cbSNADF.Visible = false;
                        break;
                    }
                case (1):   // SNAD Init and activate. 
                    {
                        btnSNAD_Disable.Enabled = false;
                        btnSNAD_Enable.Enabled = false;
                        btnSNAD_WhoID.Enabled = true;
                        cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                        cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                        cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                        cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        //--------------------------------------------------------------------------------------------------
                        cbSNAD1.Visible = false; cbSNAD2.Visible = false; cbSNAD3.Visible = false; cbSNAD4.Visible = false;
                        cbSNAD5.Visible = false; cbSNAD6.Visible = false; cbSNAD7.Visible = false; cbSNAD8.Visible = false;
                        cbSNAD9.Visible = false; cbSNADA.Visible = false; cbSNADB.Visible = false; cbSNADC.Visible = false;
                        cbSNADD.Visible = false; cbSNADE.Visible = false; cbSNADF.Visible = false;
                        // The code later set Visable true for connected device.
                        break;
                    }
                case (2):   // Select Check Box. 
                case (3):   // Select Check Box. 
                    {
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 1)
                        {
                            cbSNAD1.Checked = true; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                            cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 2)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = true; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                            cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 3)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = true; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                            cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 4)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = true;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                            cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 5)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = true; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                            cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 6)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = true; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                            cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 7)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = true; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                            cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 8)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = true;
                            cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                            cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 9)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = true; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                            cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 10)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = false; cbSNADA.Checked = true; cbSNADB.Checked = false; cbSNADC.Checked = false;
                            cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 11)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = true; cbSNADC.Checked = false;
                            cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 12)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = true;
                            cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 13)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                            cbSNADD.Checked = true; cbSNADE.Checked = false; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 14)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                            cbSNADD.Checked = false; cbSNADE.Checked = true; cbSNADF.Checked = false;
                        }
                        if (myGlobalBase.mySNADNetwork.ChannelSelect == 15)
                        {
                            cbSNAD1.Checked = false; cbSNAD2.Checked = false; cbSNAD3.Checked = false; cbSNAD4.Checked = false;
                            cbSNAD5.Checked = false; cbSNAD6.Checked = false; cbSNAD7.Checked = false; cbSNAD8.Checked = false;
                            cbSNAD9.Checked = false; cbSNADA.Checked = false; cbSNADB.Checked = false; cbSNADC.Checked = false;
                            cbSNADD.Checked = false; cbSNADE.Checked = false; cbSNADF.Checked = true;
                        }
                        myRtbTermMessageLF("-INFO: SNAD Selected Channel: " + myGlobalBase.mySNADNetwork.ChannelSelect.ToString("X"));
                        lbSNADSelect.Text = "SEL:0x" + myGlobalBase.mySNADNetwork.ChannelSelect.ToString("X");
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region //=============================================================================================cbSNAD 1-F_CheckedChanged
        private void cbSNAD1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNAD1.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 1;
                SNAD_UpdateLayout(3);
            }
        }

        private void cbSNAD2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNAD2.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 2;
                SNAD_UpdateLayout(3);
            }
        }

        private void cbSNAD3_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNAD3.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 3;
                SNAD_UpdateLayout(3);
            }
        }

        private void cbSNAD4_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNAD4.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 4;
                SNAD_UpdateLayout(3);
            }
        }
        private void cbSNAD5_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNAD5.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 5;
                SNAD_UpdateLayout(3);
            }
        }

        private void cbSNAD6_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNAD6.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 6;
                SNAD_UpdateLayout(3);
            }

        }

        private void cbSNAD7_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNAD7.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 7;
                SNAD_UpdateLayout(3);
            }
        }

        private void cbSNAD8_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNAD8.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 8;
                SNAD_UpdateLayout(3);
            }
        }

        private void cbSNAD9_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNAD9.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 9;
                SNAD_UpdateLayout(3);
            }
        }

        private void cbSNADA_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNADA.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 10;
                SNAD_UpdateLayout(3);
            }
        }

        private void cbSNADB_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNADB.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 11;
                SNAD_UpdateLayout(3);
            }
        }

        private void cbSNADC_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNADC.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 12;
                SNAD_UpdateLayout(3);
            }
        }

        private void cbSNADD_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNADD.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 13;
                SNAD_UpdateLayout(3);
            }
        }

        private void cbSNADE_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNADE.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 14;
                SNAD_UpdateLayout(3);
            }
        }

        private void cbSNADF_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSNADF.Checked == true)
            {
                myGlobalBase.mySNADNetwork.ChannelSelect = 15;
                SNAD_UpdateLayout(3);
            }
        }
        //----------------------------------------------------------------Reserved 0 for global message purpose (all SNAD pickup message). 
        private void cbSNAD0_CheckedChanged(object sender, EventArgs e)
        {
            //             if (cbSNAD0.Checked == true)
            //             {
            //                 myGlobalBase.mySNADNetwork.ChannelSelect = 0;
            //                 SNAD_UpdateLayout(3);
            //             }
        }

        #endregion

        #region //=============================================================================================btnSNAD_WhoID_Click
        private void btnSNAD_WhoID_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.mySNADNetwork == null)
                myGlobalBase.mySNADNetwork = new SNADNetwork();
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_SNADWhoID = new DMFP_Delegate(DMFP_SNADWhoID_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+WHOID()" + myGlobalBase.mySNADNetwork.ChannelSelect.ToString("X");
            if (myGlobalBase.bNoRFTermMessage == false)
                myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
            Thread t2 = null;
            if (myGlobalBase.is_Serial_Server_Connected == true)
            {
                t2 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_SNADWhoID, sMessage, 250));
            }
            else
            {
                myRtbTermMessageLF("#ERR: VCOM is not established: Do Scan and Connect first (NB:Array VCOM not supported)");
                return;
            }
            t2.Start();
            Thread.Sleep(10);
        }
        public void DMFP_SNADWhoID_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            myRtbTermMessage("-INFO: FullMessage:" + FullMessage);
            //             if (sPara[0]!=null)
            //                 myRtbTermMessageLF("-WHOID: String: " + sPara[0]);
        }
        #endregion

        #region //=============================================================================================btnSNAD_Disable_Click
        private void btnSNAD_Disable_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.mySNADNetwork == null)
                return;

        }
        #endregion

        #region //=============================================================================================btnSNAD_Enable_Click
        private void btnSNAD_Enable_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.mySNADNetwork == null)
                return;

        }
        #endregion

        #region //=============================================================================================btnSNAD_Help_Click
        private void btnSNAD_Help_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            TextBox tbx = new TextBox();
            frm.Controls.Add(tbx);
            frm.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            frm.Size = new System.Drawing.Size(600, 500);
            tbx.Multiline = true;
            tbx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            tbx.Height = frm.Height - 10;
            tbx.Width = frm.Width - 20;
            tbx.ReadOnly = true;
            tbx.AppendText("Help Section for Slave Node Address Device (SNAD) Protocol Network\r\n");
            tbx.AppendText("-----------------------------------------------------------------------------\r\n");
            tbx.AppendText(" > UDT Command(Parameter)X\\n is the SNAD Frame, where X = 1 to F\r\n");
            tbx.AppendText(" > Maximum of 15 SNAD device is supported on UART or similar chainable bus\r\n");
            tbx.AppendText(" > Array COM bus is not supported. Use Common COM connection. \r\n");
            tbx.AppendText("----------------------------------------------------Hardware:\r\n");
            tbx.AppendText(" > UART shall be connected as chain fashion\r\n");
            tbx.AppendText(" > Master-UART-TX connect to Slave UART-RX (SNAD=1)\r\n");
            tbx.AppendText(" > SNAD1-UART-TX connect to SNAD2-UART-RX \r\n");
            tbx.AppendText(" > SNAD2-UART-TX connect to SNAD3-UART-RX \r\n");
            tbx.AppendText("   and so on until last SNAD to maximum 15 devices\r\n");
            tbx.AppendText(" > SNADE-UART-TX connect to SNADF-UART-RX \r\n");
            tbx.AppendText(" > SNADF-UART-TX to Master-UART-RX\r\n");
            tbx.AppendText(" > Any last number (<0xF) of SNAD Device can terminate to Master\r\n");
            tbx.AppendText(" > All UART must be same BAUD and voltage (3V3 or 5V0 but not mixed\r\n");
            tbx.AppendText("----------------------------------------------------Init:\r\n");
            tbx.AppendText(" > After powering up and connect, click <Init SNAD> \r\n");
            tbx.AppendText(" > For 1st device in chain assign itself to SNAD1, 2nd as SNAD2, etc\r\n");
            tbx.AppendText(" > The master recieves last SNAD number and hide unused SNAD\r\n");
            tbx.AppendText("----------------------------------------------------Slave Protocol:\r\n");
            tbx.AppendText(" > Once Init is completed, all device conforms to SNAD Protocol\r\n");
            tbx.AppendText(" > It only accept UDT command protocol via SNAD Frame.\r\n");
            tbx.AppendText("   Example: <ADXL355RUN(0x23;0x5643)2\\n> goes to SNAD=2 Device\r\n");
            tbx.AppendText(" > If SNAD number matched, it remove the SNAD Frame so it does not echo\r\n");
            tbx.AppendText("   it to the next device. The Slave may response message back in chain\r\n");
            tbx.AppendText("   as long it does not contains ')X\\n' ASCII\r\n");
            tbx.AppendText(" > If not matched: it echo the SNAD Frame to next SNAD device\r\n");
            tbx.AppendText("   until match is found and thus remove SNAD Frame. \r\n");
            tbx.AppendText(" > If SNAD number after ')' is missing it echo back to master\r\n");
            tbx.AppendText("----------------------------------------------------UDT Tab Page:\r\n");
            tbx.AppendText(" > The SNAD Tabpage will insert SNAD number for you. just checked visable box\r\n");
            tbx.AppendText(" > before sending command to network. \r\n");
            tbx.AppendText("----------------------------------------------------ASYNC and Callback:\r\n");
            tbx.AppendText(" > The following is supported\r\n");
            tbx.AppendText("   + Callback protocol with + prefix sent and - prefix response\r\n");
            tbx.AppendText("   ~ ASYNC response, however may not specify which device (WIP to fix)\r\n");
            tbx.AppendText("   Ordinary command without prefixes. \r\n");
            tbx.AppendText("TVB: RGPayne (16-Dec-18)\r\n");
            frm.ShowDialog();
        }
        #endregion

        #region //=============================================================================================btnDownloadLogSurvey_Click
        private void btnDownloadLogSurvey_Click(object sender, EventArgs e)
        {
            //myEEDownloadSurvey.Show();
            if (sSessionFoldername == "")
            {
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("ERROR: No foldername, check UDT config setup and try again", "File System Error", 5, 12F);
                myRtbTermMessageLF("#E: No foldername, check UDT config setup and try again");
                return;
            }

            //sSessionFoldername = "H:\\ADTSessionLog"; //"F:\\ADTSessionLog";    // C:\Users\User\Documents
            string drive = Path.GetPathRoot(sSessionFoldername);
            if (!Directory.Exists(drive))
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Create folder name in selected drive: ADTSessionLog";
                    fbd.RootFolder = Environment.SpecialFolder.CommonDocuments;
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        sSessionFoldername = fbd.SelectedPath;
                    }
                    else
                    {
                        mbOperationError = new DialogSupport();
                        mbOperationError.PopUpMessageBox("ERROR: Drive/folder not exist", "File System Error", 5, 12F);
                        myRtbTermMessageLF("#E: Drive/folder not exist");
                        return;
                    }
                }
            }
            myGlobalBase.sSessionFoldername = sSessionFoldername;
            myEEDownloadSurvey.EE_UpdateFolderName(myGlobalBase.sSessionFoldername);
            if (myUSB_VCOM_Comm.VCOM_Validate_Encoding() == false)                  // Check encoding. 
                return;
            myEEDownloadSurvey.EE_LogMemSurvey_Show();
        }
        #endregion

        #region //=============================================================================================btnTestCode_Click
        private void btnTestCode_Click(object sender, EventArgs e)
        {
            string sData = "$B;5C7E92E2;V;<";
            byte[] data = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0, 0x7E, 0x7F, 0x80, 0x81, 0x82, 0x83, 0xFF, 0x00, 0xAA, 0x77, 0x00, 0xFF };
            string s = System.Text.Encoding.Unicode.GetString(data, 0, data.Length);
            //byte[] bufferAll = System.Text.Encoding.Unicode.GetBytes(s);

            sData = sData + s + ">;1F67892;F327812;89341;$E\n";
            ESM_LogMem_Hyrbid_Frame myFrame = new ESM_LogMem_Hyrbid_Frame();
            myFrame.HybridFrame_ProcessString(sData);
            myRtbTermMessageLF("Done!");


        }
        #endregion

        //##############################################################################################################
        //============================================================================================= ESM Project
        //##############################################################################################################

        #region //=============================================================================================ESMConfigInit
        private void ESMConfigInit()
        {
            myESM_NVM_SensorConfig = new ESM_NVM_SensorConfig();
        }
        #endregion

        #region //=============================================================================================btnOpenNVMWindow_Click
        private void btnOpenNVMWindow_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.ESMConfig_SetupOpen == true)
            {
                DialogSupport mbOperationBusy = new DialogSupport();
                mbOperationBusy.PopUpMessageBox("ERROR: Please close the ESM NVM Config Window first and try again", "ESM NVM Config", 30, 12F);
                return;
            }
            myESMConfigNVM.ESMConfig_Show();
        }
        #endregion

        #region //=============================================================================================btnOpenNVMWindowBlockSeqNo_Click
        private void btnOpenNVMWindowBlockSeqNo_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.ESMBlockSeqNo_SetupOpen == true)
            {
                DialogSupport mbOperationBusy = new DialogSupport();
                mbOperationBusy.PopUpMessageBox("ERROR: Please close the ESM NVM Config Window first and try again", "ESM NVM Config", 30, 12F);
                return;
            }
            myESMBlockSeqNVM.ESMConfig_Show();
        }
        #endregion
        #region //=============================================================================================btnESMBlockErase_Click
        private void btnESMBlockErase_Click(object sender, EventArgs e)
        {
            DMFP_Delegate fpCallBack = new DMFP_Delegate(DMFP_ESMBlockEraseCallback);
            myDMFProtocol.DMFP_Process_Command(fpCallBack, "+STCERASE(1)", true, -1);

        }

        #region //================================================================DMFP_ESMBlockEraseCallback
        public void DMFP_ESMBlockEraseCallback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            string ErrorMessage = "ERROR: Block Erase Callback Error";
            if (hPara[0] != 0xFF)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox(ErrorMessage, "ESM Operation", 5, 12F);
                myRtbTermMessageLF(ErrorMessage);
                return;
            }
        }
        #endregion


        #endregion
        #region //=============================================================================================btnESMBlukErase_Click
        private void btnESMBlukErase_Click(object sender, EventArgs e)
        {
            DMFP_Delegate fpCallBack = new DMFP_Delegate(DMFP_ESMBlukEraseCallback);
            myDMFProtocol.DMFP_Process_Command(fpCallBack, "+STCERASE(2)", true, -1);

        }
        #endregion

        #region //================================================================DMFP_ESMBlukEraseCallback
        public void DMFP_ESMBlukEraseCallback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            string ErrorMessage = "ERROR: Bulk Erase Callback Error";
            if (hPara[0] != 0xFF)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox(ErrorMessage, "ESM Operation", 5, 12F);
                myRtbTermMessageLF(ErrorMessage);
                return;
            }
        }
        #endregion
        #region //=============================================================================================btnESMStartSurvey_Click
        private void btnESMStartSurvey_Click(object sender, EventArgs e)
        {
            string sSTCStart = "";
            DMFP_Delegate fpCallBack = new DMFP_Delegate(DMFP_ESMStartSurveyCallback);
            DateTime dt2 = DateTime.Now;
            DateTime dt = dt2.ToLocalTime();
            string UDT = Tools.sDateTimeToUnixTimestampHex(dt);
            //UDT = UDT.Substring(0, UDT.IndexOf("."));
            sSTCStart = "+STCSTART(0;" + UDT + ")";
            myDMFProtocol.DMFP_Process_Command(fpCallBack, sSTCStart, true, -1);
        }
        #endregion
        #region //================================================================DMFP_ESMBlukEraseCallback
        public void DMFP_ESMStartSurveyCallback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            string ErrorMessage = "ERROR: Start Survey Reported Error, Have you Erase/Update timestamp?";
            if (hPara[0] != 0xFF)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox(ErrorMessage, "ESM Operation", 5, 12F);
                myRtbTermMessageLF(ErrorMessage);
                return;
            }
        }
        #endregion
        #region //=============================================================================================btnESMStopSurvey_Click
        private void btnESMStopSurvey_Click(object sender, EventArgs e)
        {
            DMFP_Delegate fpCallBack = new DMFP_Delegate(DMFP_ESMStopSurveyCallback);
            myDMFProtocol.DMFP_Process_Command(fpCallBack, "+STCSTOP()", true, -1);
        }
        #endregion
        #region //================================================================DMFP_ESMBlukEraseCallback
        public void DMFP_ESMStopSurveyCallback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            string ErrorMessage = "ERROR: Stop Survey Reported Error";
            if (hPara[0] != 0xFF)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox(ErrorMessage, "ESM Operation", 5, 12F);
                myRtbTermMessageLF(ErrorMessage);
                return;
            }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= CAN Bus Section
        //##############################################################################################################

        #region //=============================================================================================CanBusInit
        private void CanBusInit()
        {


        }
        #endregion

        #region //=============================================================================================PCAN_Update_TabPanel
        public void PCAN_Update_TabPanel()
        {
            cbbBaudrates.SelectedIndex = myCanPCAN.myPCAN_Configuration.BaudrateIndex;
            cbCanExtended.Checked = myCanPCAN.PCAN_FilterExt_State();
            nudCanIdFrom.Value = myCanPCAN.myPCAN_Configuration.IndexFrom;
            nudCanIdTo.Value = myCanPCAN.myPCAN_Configuration.IndexTo;
        }
        #endregion

        #region //=============================================================================================PCAN_PanelLayout_Mode
        public void PCAN_PanelLayout_Mode(bool isConnected)  // True = block control (connected)  False = access control (disconnected),
        {
            cbbBaudrates.Enabled = !isConnected;
            cbCanExtended.Enabled = !isConnected;
            nudCanIdFrom.Enabled = !isConnected;
            nudCanIdTo.Enabled = !isConnected;
            btnCanRelease.Enabled = isConnected;
            btnCanConnect.Enabled = !isConnected;
        }
        #endregion

        #region //=============================================================================================CbbBaudrates_SelectedIndexChanged
        private void CbbBaudrates_SelectedIndexChanged(object sender, EventArgs e)
        {
            myCanPCAN.PCAN_Baudrates_IndexChanged(cbbBaudrates.SelectedIndex);
        }
        #endregion

        #region //=============================================================================================CbCanExtended_CheckedChanged
        private void CbCanExtended_CheckedChanged(object sender, EventArgs e)
        {
            decimal myFrom = nudCanIdFrom.Value;
            decimal myTo = nudCanIdTo.Value;
            myCanPCAN.PCAN_FilterExt(cbCanExtended.Checked, ref myFrom, ref myTo);
            nudCanIdFrom.Maximum = myFrom;
            nudCanIdTo.Maximum = myTo;
        }
        #endregion

        #region //=============================================================================================BtnCanWindow_Click
        private void BtnCanWindow_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.CanPCAN_OpenForm == true)
            {
                DialogSupport mbOperationBusy = new DialogSupport();
                mbOperationBusy.PopUpMessageBox("ERROR: Please close the Can-PCAN Window first and try again", "Can-PCAN Window", 30, 12F);
                return;
            }
            myCanPCAN.CanPCAN_Show();
        }
        #endregion

        #region //=============================================================================================BtnCanConnect_Click
        private void BtnCanConnect_Click(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------------------------Check if running on battery.
            Boolean isRunningOnBattery = (System.Windows.Forms.SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Offline);
            if (isRunningOnBattery == true)
            {
                string ErrorMessage = "+WARN: This laptop is running on battery (with reduce MCU performance setting), which may impact PCAN real time operation in UDT";
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox(ErrorMessage, "PCAN Operation", 10, 12F);
                myRtbTermMessageLF(ErrorMessage);
            }
            //-------------------------------------------------------------------------------------------------
            myCanPCAN.myPCAN_Configuration.ReleaseEnable = true;
            PCAN_Update_TabPanel();
            PCAN_PanelLayout_Mode(true);
            if (myCanPCAN.PCAN_Connect_With_Device() == true)
            {
                myGlobalBase.CanPCAN_isConnected = true;
            }
            else
            {
                myGlobalBase.CanPCAN_isConnected = false;
            }
            myCanPCAN.CanPCAN_Show();
        }
        #endregion

        #region //=============================================================================================BtnCanRelease_Click
        private void BtnCanRelease_Click(object sender, EventArgs e)
        {
            myCanPCAN.myPCAN_Configuration.ReleaseEnable = false;
            PCAN_PanelLayout_Mode(false);
            myCanPCAN.PCAN_Release_With_Device();
        }
        #endregion

        #region //=============================================================================================btnPCANRefresh_Click
        private void btnPCANRefresh_Click(object sender, EventArgs e)
        {
            if (myCanPCAN.PCAN_isPCANBasicDriverFitted() == false)
            {
                myRtbTermMessageLF("**************************************************************");
                myRtbTermMessageLF("Unable to find the library: PCANBasic.dll. Read 'Connect Help'");
                myRtbTermMessageLF("**************************************************************");
            }
            else
            {
                myRtbTermMessageLF("PCANBasic.dll is fitted. Read 'Connect Help' if unable to connect!");
            }
        }
        #endregion

        #region //=============================================================================================btnPCAN_ConnectHelp_Click
        private void btnPCAN_ConnectHelp_Click(object sender, EventArgs e)
        {
            myCanPCAN.BtnCANHelpA();
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= TimeStamp
        //##############################################################################################################

        #region //=============================================================================================TimeStamp_Init
        private void TimeStamp_Init()
        {
            //myGlobalBase.TimeStamp_FolderName = Tools.GetPathUserFolder();
            txTSFileName.Text = myGlobalBase.TimeStamp_FolderName;
            myGlobalBase.TimeStamp_FilenameName = "";
        }
        #endregion
        #region //=============================================================================================btnTSOpen_Click
        private void btnTSOpen_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.TimeStamp_FolderName == null)             // No filename path, quit. 
                return;
            try
            {
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = myGlobalBase.TimeStamp_FolderName;
                prc.Start();
            }
            catch
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("#ERR: Open Folder has no folder/path or attempt to use restricted domains of Drive C", "TimeStamp ERROR", 10);
            }
        }
        #endregion
        #region //=============================================================================================btnTSSetFileNme_Click
        //         Path.GetDirectoryName(saveFileDialog1.FileName);
        //         Path.GetFileName(saveFileDialog1.FileName);      // Just a filename, no folder path. 
        private void btnTSSetFileNme_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfdSaveTSData = new SaveFileDialog();
            sfdSaveTSData.Filter = "csv files (*.csv)|*.csv";
            sfdSaveTSData.InitialDirectory = myGlobalBase.TimeStamp_FolderName;
            sfdSaveTSData.FileName = "TimeStampEventRecords";
            sfdSaveTSData.Title = "Export to CVS File for TimeStamp";
            DialogResult dr = sfdSaveTSData.ShowDialog();

            if (dr == DialogResult.OK)
            {

                UInt32 TimeStampNow = Tools.uDateTimeToUnixTimestampNowLocal();
                string sTimeStampNow = TimeStampNow.ToString("D");
                myGlobalBase.TimeStamp_FilenameName = sfdSaveTSData.FileName;
                myGlobalBase.TimeStamp_FolderName = Path.GetDirectoryName(sfdSaveTSData.FileName);
                txTSFileName.Text = myGlobalBase.TimeStamp_FilenameName;
                StringBuilder sb = new StringBuilder();
                try
                {
                    using (FileStream fs = new FileStream(myGlobalBase.TimeStamp_FilenameName, FileMode.Append, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sb.AppendLine(sTimeStampNow + ",Created Filename Success!");
                        sw.Write(sb.ToString());
                        sw.Close();
                        mbOperationBusy = null;
                        mbOperationBusy = new DialogSupport();
                        mbOperationBusy.PopUpMessageBox("-I: Filename Successfully Created!", "TimeStamp", 2);
                    }
                }
                catch
                {
                    mbOperationError = null;
                    mbOperationError = new DialogSupport();
                    mbOperationError.PopUpMessageBox("+E:Filename Write Issue? / Open file? / Protected Region?", "TimeStamp Filename ERROR", 10);
                }
            }
            else
            {
                txTSFileName.Text = myGlobalBase.TimeStamp_FolderName;
                myGlobalBase.TimeStamp_FilenameName = "";
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("+E:Filename Not Valid / User Cancel. Try Again", "TimeStamp Filename ERROR", 5);
            }
        }
        #endregion
        #region //=============================================================================================TimeStampFileNameNotSetError
        private bool TimeStampFileNameNotSetError()
        {
            if (myGlobalBase.TimeStamp_FilenameName == "")
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("Click <Set File> to setup Filename First", "TimeStamp ERROR", 10);
                return false;
            }
            return true;
        }
        #endregion
        #region //=============================================================================================TimeStampWriteNow
        private void TimeStampWriteNow(string Comment)
        {
            UInt32 TimeStampNow = Tools.uDateTimeToUnixTimestampNowLocal();
            string sTimeStampNow = TimeStampNow.ToString("D");

            StringBuilder sb = new StringBuilder();
            try
            {
                using (FileStream fs = new FileStream(myGlobalBase.TimeStamp_FilenameName, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sb.AppendLine(sTimeStampNow + "," + Comment);
                    sw.Write(sb.ToString());
                    sw.Close();
                    mbOperationBusy = null;
                    mbOperationBusy = new DialogSupport();
                    mbOperationBusy.PopUpMessageBox("-I: Append Success! UDT1970:" + sTimeStampNow, "TimeStamp", 1);
                }
            }
            catch
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("+E:Filename Write Issue? / Open file? / Protected Region?", "TimeStamp Filename ERROR", 10);
            }
        }
        #endregion
        #region //=============================================================================================btnTSSaveTimeStamp_Click
        private void btnTSSaveTimeStamp_Click(object sender, EventArgs e)
        {
            if (TimeStampFileNameNotSetError() == false)
                return;
            TimeStampWriteNow(txTSComment.Text);
        }
        #endregion
        #region //=============================================================================================btnTSPumpON_Click
        private void btnTSPumpON_Click(object sender, EventArgs e)
        {
            if (TimeStampFileNameNotSetError() == false)
                return;
            TimeStampWriteNow("Pump ON");
        }
        #endregion
        #region //=============================================================================================btnTSPumpOFF_Click
        private void btnTSPumpOFF_Click(object sender, EventArgs e)
        {
            if (TimeStampFileNameNotSetError() == false)
                return;
            TimeStampWriteNow("Pump OFF");
        }
        #endregion
        #region //=============================================================================================btnTSMotorON_Click
        private void btnTSMotorON_Click(object sender, EventArgs e)
        {
            if (TimeStampFileNameNotSetError() == false)
                return;
            TimeStampWriteNow("Motor ON");
        }
        #endregion
        #region //=============================================================================================btnTSMotorOFF_Click
        private void btnTSMotorOFF_Click(object sender, EventArgs e)
        {
            if (TimeStampFileNameNotSetError() == false)
                return;
            TimeStampWriteNow("Motor OFF");
        }
        #endregion
        #region //=============================================================================================btnTSStartTool_Click
        private void btnTSStartTool_Click(object sender, EventArgs e)
        {
            if (TimeStampFileNameNotSetError() == false)
                return;
            TimeStampWriteNow("Tool Start Operation");
        }
        #endregion
        #region //=============================================================================================btnTSStopTool_Click
        private void btnTSStopTool_Click(object sender, EventArgs e)
        {
            if (TimeStampFileNameNotSetError() == false)
                return;
            TimeStampWriteNow("Tool Stop Operation");
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= Riscy Timer II
        //##############################################################################################################

        #region //=============================================================================================timerIIWithSMSToolStripMenuItem_Click
        private void timerIIWithSMSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //             if (myGlobalBase.RiscyTimerII_OpenForm == true)
            //             {
            //                 myRiscyTimerII.RiscyTimerII_Show();
            //                 return;
            //             }
            myRiscyTimerII.RiscyTimerII_Show();
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Debug Helper
        //##############################################################################################################

        #region //=============================================================================================debugModeToolStripMenuItem_Click
        private void debugModeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        #endregion

        //##############################################################################################################
        //============================================================================================= Hex Duplicate (30/Nov/22)
        //##############################################################################################################

        List<byte> lstHexDuplicateBufferOut;
        byte[] aHexDuplicateBufferIn;
        string sHexDuplicateFilename;

        #region //=============================================================================================btnDuplicateSeekFile_Click
        private void btnDuplicateSeekFile_Click(object sender, EventArgs e)
        {
            aHexDuplicateBufferIn = null;
            lbHexDuplicateStatus.Text = "Select Filename in Hex/Bin/Txt";
            OpenFileDialog ofd_ImportData = new OpenFileDialog();
            ofd_ImportData.Filter = "(*.txt)|*.txt|(*.bin)|*.bin|(*.hex)|*.hex|(*.*)|*.*";
            ofd_ImportData.FileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ofd_ImportData.Title = "Browse Hex Files to duplicate";
            DialogResult dr = ofd_ImportData.ShowDialog();
            if (dr == DialogResult.OK)
            {
                tbHexDuplicateFilename.Text = ofd_ImportData.FileName;
            }
            else
            {
                tbHexDuplicateFilename.Text = "Filename Error";
                lbHexDuplicateStatus.Text = "Try again, check filename type, etc";
                return;
            }
            try
            {
                lbHexDuplicateStatus.Text = "Loading File";

                FileStream archivoImg = new FileStream(ofd_ImportData.FileName, FileMode.Open, FileAccess.Read);
                BinaryReader bReaderImg = new BinaryReader(archivoImg);
                aHexDuplicateBufferIn = bReaderImg.ReadBytes(Convert.ToInt32(archivoImg.Length));
                archivoImg.Close();
                lbHexDuplicateStatus.Text = "Buffer Length (Byte): " + aHexDuplicateBufferIn.Length.ToString() + " || 0x" + aHexDuplicateBufferIn.Length.ToString("X");
                sHexDuplicateFilename = ofd_ImportData.FileName;
                if (lstHexDuplicateBufferOut == null)
                {
                    lstHexDuplicateBufferOut = new List<byte>();
                }
                else
                {
                    lstHexDuplicateBufferOut.Clear();
                }
                lstHexDuplicateBufferOut.AddRange(aHexDuplicateBufferIn);
            }
            catch (Exception ex)
            {
                lbHexDuplicateStatus.Text = "###ERR:Load Failure, check permits, etc";
                return;
            }
        }
        #endregion

        #region //=============================================================================================btnDuplicateProcess_Click
        private void btnDuplicateProcess_Click(object sender, EventArgs e)
        {
            if (lstHexDuplicateBufferOut == null)
            {
                lbHexDuplicateStatus.Text = "###ERR: Load filename first";
                return;
            }
            int offset = Tools.AnyStringtoInt32(tbHexDuplicateOffset.Text);

            if (offset == -975579)
            {
                lbHexDuplicateStatus.Text = "###ERR: Check Offset Syntax (0x and 0X for hex entry)";
                return;
            }
            int length = aHexDuplicateBufferIn.Length;
            int LengthZero = offset - length;
            if (LengthZero<=0)
            {
                lbHexDuplicateStatus.Text = "###ERR: Check your Offset Value (it should be more than filename size)";
                return;
            }
            for (int i = 0; i < LengthZero; i++)
            {
                lstHexDuplicateBufferOut.Add(0x00);
            }
            lstHexDuplicateBufferOut.AddRange(aHexDuplicateBufferIn);
            lbHexDuplicateStatus.Text = "Final Buffer Length (Byte):" + lstHexDuplicateBufferOut.Count.ToString() + " || 0x" + lstHexDuplicateBufferOut.Count.ToString("X");

            //----------------------------------------------------------Save to filename
            byte[] bArrarySave = lstHexDuplicateBufferOut.ToArray();
            //----------------------Check existing filename to save.
            int retryloop = 10;
            while (retryloop!=0)
            {
                try
                {
                    if (File.Exists(sHexDuplicateFilename))
                    {
                        sHexDuplicateFilename = sHexDuplicateFilename.Insert(sHexDuplicateFilename.Length - 4, "X"); // Insert 'X' before .hex
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    sHexDuplicateFilename = sHexDuplicateFilename.Insert(sHexDuplicateFilename.Length - 4, "X");
                }
                retryloop--;
            }
            if (retryloop==0)
            {
                lbHexDuplicateStatus.Text = "###ERR:Write Failure, try different filename or delete old filename, etc";
                return;
            }
            //---------------------Write filename
            try
            {
                var bw = new BinaryWriter(File.OpenWrite(sHexDuplicateFilename));
                bw.Write(bArrarySave);
                bw.Close();
                lbHexDuplicateStatus.Text = "File Saved with 'X' postfix || Length :" + lstHexDuplicateBufferOut.Count.ToString() + " || 0x" + lstHexDuplicateBufferOut.Count.ToString("X"); ;

            }
            catch (Exception ex)
            {
                lbHexDuplicateStatus.Text = "###ERR:Write Failure, not sure why.";
            }
            
        }
        #endregion

        #region //=============================================================================================btnHDOpenFolder_Click
        private void btnHDOpenFolder_Click(object sender, EventArgs e)
        {
            if (sHexDuplicateFilename == "")
                return;
            string myPath = Path.GetDirectoryName(sHexDuplicateFilename); 
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = myPath;
            prc.Start();
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= Ethernet 10/Dec/2022
        //##############################################################################################################

        // VS2022: Try extension <Hot Commands> for modified short cut. 
        // http://tkramar.blogspot.com/2007/10/effective-eclipse-ii-shortcut-keys_16.html
        // https://marketplace.visualstudio.com/items?itemName=JustinClareburtMSFT.VSShortcutsManager
        // https://bytescout.com/blog/visual-studio-hot-keys.html
        // avoid older VS extension, they do not work. 
        // https://gist.github.com/zmilojko/5055246

        private bool ETH_isDebugViewEnable = false;         // Test message in block code method. 
        private UdpClient ETH_udpClient;

        #region //=============================================================================================ETH_UdpState
        private struct ETH_UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
        }
        #endregion

        #region //============================================================================================= class ETH_Config
        private class ETH_Config
        {
            public IPAddress IpAddress;
            public string sIPAddress;
            public int DSNPort;
            public int SRCPort;
            //---------------------------------------------Constructor
            public ETH_Config(string _sIPAddress, int _DSNPort, int _SRCPORT)
            {
                sIPAddress = _sIPAddress;
                DSNPort = _DSNPort;
                SRCPort = _SRCPORT;
                IpAddress = IPAddress.Parse(sIPAddress);
            }
        }
        ETH_Config ETH_ConfigActive;
        #endregion

        #region //=============================================================================================btnUDP_Connect_Click
        private void btnUDP_Connect_Click(object sender, EventArgs e)
        {
            //----------------------------------------- Open socket and enabled UDP. 
            //https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.udpclient?view=net-7.0
            if (myGlobalBase.ETH_isUDTClientOpen == false)
            {
                if (ETH_Validate_ConfigEntry() == false)
                    return;
                //-------------------------------------------Update Window Panel.
                myRtbTermMessageLF("-I:Open UDP Client IPV4/Port:" + ETH_ConfigActive.sIPAddress + ":" + ETH_ConfigActive.DSNPort.ToString());
                //-------------------------------------------Setup UDT Client.
                try
                {
                    ETH_udpClient = new UdpClient(ETH_ConfigActive.SRCPort);
                    ETH_udpClient.Connect(ETH_ConfigActive.IpAddress, ETH_ConfigActive.DSNPort);
                    //------------------------------------------- Setup UDPClient
                    ETH_UDT_Start_Listening();
                    //-------------------------------------------Update window state
                    myGlobalBase.ETH_isUDTClientOpen = true;
                    tbUDPDesIP.Enabled = false;
                    tbUDPDesPort.Enabled = false;
                    tbUDPSourcePort.Enabled = false;
                    btnUDP_Connect.Text = "Disconnect";
                    myGlobalBase.ETH_messageReceived = false;
                    ////------------------------------------------- Debug test only
                    //if (ETH_isDebugViewEnable)
                    //{
                    //    string testmessage = "Is anybody there?";
                    //    myRtbTermMessageLF("Sent:" + testmessage);
                    //    Byte[] sendBytes = Encoding.ASCII.GetBytes(testmessage);
                    //    myGlobalBase.ETH_udpClient.Send(sendBytes, sendBytes.Length);
                    //}
                }
                catch (Exception eeee)
                {
                    mbOperationError = null;
                    mbOperationError = new DialogSupport();
                    mbOperationError.PopUpMessageBox("ERROR: Open Port Exception Event:"+eeee.Message, "IP Port Number", 30, 12F);
                }
            }
            else
            {
                ETH_Close_UDPClientPort();
                myGlobalBase.ETH_isUDTClientOpen = false;
                myGlobalBase.ETH_messageReceived = false;
            }
        }
        #endregion

        #region //=============================================================================================ETH_UDT_Start_Listening
        public void ETH_UDT_Start_Listening()
        {
            //------------------------------------------- Setup Non Blocking Listening Port
            IPEndPoint eee = new IPEndPoint(IPAddress.Any, 0);
            UdpClient uuu = ETH_udpClient;      //new UdpClient(eee);

            ETH_UdpState ETH_objectstate = new ETH_UdpState();
            ETH_objectstate.e = eee;
            ETH_objectstate.u = uuu;
            uuu.BeginReceive(new AsyncCallback(ETH_ReceiveCallback), ETH_objectstate);
        }
        #endregion

        #region //=============================================================================================ETH_ReceiveCallback
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.udpclient.beginreceive?view=net-7.0
        public void ETH_ReceiveCallback(IAsyncResult ar)
        {
            byte[] receiveBytes;
            string receiveString;
            try
            {
                UdpClient uuu = ((ETH_UdpState)(ar.AsyncState)).u;
                IPEndPoint eee = ((ETH_UdpState)(ar.AsyncState)).e;

                receiveBytes = uuu.EndReceive(ar, ref eee);
                receiveString = Encoding.ASCII.GetString(receiveBytes);
                myGlobalBase.ETH_messageReceived = true;
            }
            catch
            {
                if (myGlobalBase.ETH_isUDTClientOpen == true)               // Close Port if left open
                {
                    ETH_Close_UDPClientPort();
                }
                return;
            }
            //----------------------------------------restart
            ETH_UDT_Start_Listening();
            //----------------------------------------
            if (ETH_isDebugViewEnable)
            {
                myRtbTermMessageLF("-I:Recieved:" + receiveString);
                //myRtbTermMessageLF($"-I:Received: {receiveString}");     // interesting alternative.
            }
            else
            {
                myUSB_Message_Manager.ComMSG_Designate_ReadData_To_App(receiveString, receiveBytes);
            }
        }
        #endregion

        #region //=============================================================================================ETH_UDT_SendMessage
        public bool ETH_UDT_SendMessage(string msg)
        {
            if (myGlobalBase.ETH_isUDTClientOpen == false)      // Not connected
                return false;
            if (msg == "")                                      // No message to send.
                return false;

            //myGlobalBase.ETH_udpClient = null;
            //myGlobalBase.ETH_udpClient = new UdpClient(ETH_ConfigActive.SRCPort);
            //myGlobalBase.ETH_udpClient.Connect(ETH_ConfigActive.IpAddress, ETH_ConfigActive.DSNPort);
   
            Byte[] sendBytes = Encoding.ASCII.GetBytes(msg);
            ETH_udpClient.Send(sendBytes, sendBytes.Length);

            return true;
        }
        #endregion

        #region //=============================================================================================ETH_Close_UDPClientPort
        private void ETH_Close_UDPClientPort()
        {
            myGlobalBase.ETH_isUDTClientOpen = false;
            tbUDPDesIP.Enabled = true;
            tbUDPDesPort.Enabled = true;
            tbUDPSourcePort.Enabled = true;
            btnUDP_Connect.Text = "Connect";
            myRtbTermMessageLF("-I:Closed UDP Client IPV4/Port:" + tbUDPDesIP.Text + ":" + tbUDPDesPort.Text);
            try
            {

                ETH_udpClient.Close();
                //Tools.InteractivePause(TimeSpan.FromMilliseconds(500));
            }
            catch (Exception eeee)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("ERROR:  Close Port Exception Event:" + eeee.Message, "IP Port Number", 30, 12F);
            }
            
            //ETH_udpClient = null;
        }
        #endregion

        #region //=============================================================================================ethernetToolStripMenuItem_Click

        private void ethernetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            TextBox tbx = new TextBox();
            frm.Controls.Add(tbx);
            tbx.Multiline = true;
            tbx.Height = frm.Height;
            tbx.Width = frm.Width;

            tbx.AppendText("Help Section for Ethernet Connection \r\n");
            tbx.AppendText("----------------------------------------------------------\r\n");
            tbx.AppendText("Support the following protocol\r\n");
            tbx.AppendText("UDP Client (Implemented:         Other end must be setup as UDT Server)\r\n");
            tbx.AppendText("UDP Server (Not implemented yet, Other end must be setup as UDT Client)\r\n");
            tbx.AppendText("TCP        (Not implemented yet)\r\n");
            tbx.AppendText("Other Variants: TBA\r\n");
            tbx.AppendText("----------------------------------------------------------\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText("NB: In case of firewall block, set Port/IP channel to be allowed\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText("Revision 1A, (C) RPayne (10-Dec-22)\r\n");
            frm.ShowDialog();
            
        }
        #endregion

        #region //=============================================================================================CbETH_Ping_Click
        private void CbETH_Ping_Click(object sender, EventArgs e)
        {
            cbETH_Ping.Checked = false;
            if (myGlobalBase.ETH_isUDTClientOpen == false)
            {
                if (ETH_Validate_ConfigEntry() == false)
                    return;
                //-----------------------------------------------Check network address with port, both must match for pings to work!
                try
                {
                    Ping myPing = new Ping();
                    
                    PingReply reply = myPing.Send(ETH_ConfigActive.sIPAddress, ETH_ConfigActive.DSNPort);
                    if (reply != null)
                    {
                        myRtbTermMessageLF("-I: Ping Status: " + reply.Status + " Time (mSec): " + reply.RoundtripTime.ToString() + " Address: " + reply.Address + ":" + ETH_ConfigActive.DSNPort.ToString());
                    }
                }
                catch
                {
                    myRtbTermMessageLF("#E: Ping Timeout!");
                    return;
                }
            }
            else
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("ERROR: Disconnect Port before ping test", "Ping test", 30, 12F);
                return;
            }
            //--------------------------------Fetch MAC number (physical address) if passed ping test. 
            string sMacNumber = ETH_GetMacByIp(ETH_ConfigActive.sIPAddress);
            myRtbTermMessageLF("-I: Ext Device's MAC Number  :"+ sMacNumber);
        }
        #endregion

        #region //=============================================================================================ETH_Validate_ConfigEntry
        private bool ETH_Validate_ConfigEntry()
        {
            //-------------------------------------------------------------text correction for IP port.
            if (tbUDPDesIP.Text.Contains(','))
                tbUDPDesIP.Text = tbUDPDesIP.Text.Replace(',', '.');
            if (tbUDPDesIP.Text.Contains(':'))
                tbUDPDesIP.Text = tbUDPDesIP.Text.Replace(':', '.');
            if (tbUDPDesIP.Text.Contains('/'))
                tbUDPDesIP.Text = tbUDPDesIP.Text.Replace('/', '.');
            if (tbUDPDesIP.Text.Contains('\\'))
                tbUDPDesIP.Text = tbUDPDesIP.Text.Replace('\\', '.');
            tbUDPDesIP.Invalidate();
            //-------------------------------------------------------------Validate Des IP 
            if (Tools.Ethernet_ValidateIPv4(tbUDPDesIP.Text) == false)
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("ERROR: Check IP address, it must be 4 x decimal (0 to 255) with dot", "IP Address", 30, 12F);
                return false;
            }
            //-------------------------------------------------------------Validate port Number (0 to 0xFFFF) 
            int iPortNoSrc = 0;
            int iPortNoSDsn = 0;
            iPortNoSDsn = Tools.Ethernet_ValidatePortNumber(tbUDPDesPort.Text, 0, 0xFFFF);
            iPortNoSrc = Tools.Ethernet_ValidatePortNumber(tbUDPSourcePort.Text, 0, 0xFFFF);
            if ((iPortNoSDsn <= 0) || (iPortNoSrc <= 0))
            {
                mbOperationError = null;
                mbOperationError = new DialogSupport();
                mbOperationError.PopUpMessageBox("ERROR: Incorrect Port Number Entry: 0 to 65535", "IP Port Number", 30, 12F);
                return false;
            }
            //-------------------------------------------Backup Config details
            ETH_ConfigActive = null;
            ETH_ConfigActive = new ETH_Config(tbUDPDesIP.Text, iPortNoSDsn, iPortNoSrc);
            return true;
        }
        #endregion

        #region //=============================================================================================ETH_GetMacByIp
        //https://stackoverflow.com/questions/12802888/get-a-machines-mac-address-on-the-local-network-from-its-ip-in-c-sharp 
        public struct MacIpPair
        {
            public string MacAddress;
            public string IpAddress;
        }

        public string ETH_GetMacByIp(string ip)
        {
            var pairs = this.GetMacIpPairs();

            foreach (var pair in pairs)
            {
                if (pair.IpAddress == ip)
                    return pair.MacAddress;
            }

            throw new Exception($"Can't retrieve mac address from ip: {ip}");
        }

        public IEnumerable<MacIpPair> GetMacIpPairs()
        {
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a ";
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();

            string cmdOutput = pProcess.StandardOutput.ReadToEnd();
            string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})";

            foreach (Match m in Regex.Matches(cmdOutput, pattern, RegexOptions.IgnoreCase))
            {
                yield return new MacIpPair()
                {
                    MacAddress = m.Groups["mac"].Value,
                    IpAddress = m.Groups["ip"].Value
                };
            }
        }


        #endregion

        //##############################################################################################################
        //=============================================================================================
        //##############################################################################################################
    }

    #region//======================================================DoubleBuffered property via reflection for RichTextBox, improve flickerless image.
    public static class ExtensionMethods4
	{
		public static void DoubleBuffered4(this RichTextBox dgv, bool setting)
		{
			Type dgvType = dgv.GetType();
			PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
				BindingFlags.Instance | BindingFlags.NonPublic);
			pi.SetValue(dgv, setting, null);
		}
	}
	#endregion

	#region//======================================================DoubleBuffered property via reflection for TabControl, improve flickerless image.
	public static class ExtensionMethods3
	{
		public static void DoubleBuffered3(this TabControl dgv, bool setting)
		{
			Type dgvType = dgv.GetType();
			PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
				BindingFlags.Instance | BindingFlags.NonPublic);
			pi.SetValue(dgv, setting, null);
		}
	}
    #endregion


    }




//if (isBlockingEnabled)
//{   // Blocking Method.
//    //IPEndPoint object will allow us to read datagrams sent from any source.
//    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

//    // Blocks until a message returns on this socket from a remote host.
//    Byte[] receiveBytes = myGlobalBase.ETH_udpClient.Receive(ref RemoteIpEndPoint);
//    string returnData = Encoding.ASCII.GetString(receiveBytes);

//    // Uses the IPEndPoint object to determine which of these two hosts responded.
//    myRtbTermMessageLF("Recieved:" + returnData.ToString());
//    myRtbTermMessageLF("This message was sent from " +
//                                RemoteIpEndPoint.Address.ToString() +
//                                " on their port number " +
//                                RemoteIpEndPoint.Port.ToString());
//}
//else  // Non Blocking method
//{
    