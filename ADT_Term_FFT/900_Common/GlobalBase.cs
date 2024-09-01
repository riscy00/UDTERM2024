using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;
using System.ComponentModel;
using UDT_Term;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;

//=======================================================================================WARNING
//=============CAN BUS PROTOCOL OR SUCH (ie SDO, OD, EDS) IS NOT DEFINED IN THIS CODE====
//=============HOLD REQUEST and RESPONSE DATA BUFFER AND SERVICE ONLY====================
//=======================================================================================WARNING

namespace UDT_Term_FFT
{   
    //===================================================================3D Vector
    public struct Vector3           // Double
    {
        public double X;
        public double Y;
        public double Z;
    }
    public struct fVector3           // Float
    {
        public float X;
        public float Y;
        public float Z;
    }
    public struct sVector3          // String 
    {
        public string X;
        public string Y;
        public string Z;
    }
    public struct iVector3          // Integer 
    {
        public Int32 X;
        public Int32 Y;
        public Int32 Z;
    }

    public struct sByteVector3          // Integer 
    {
        public sbyte X;
        public sbyte Y;
        public sbyte Z;
    }

    //================================Scope: General purpose database files and records, including device status and setting and configuration.
    public class GlobalBase
    {
        public BindingList<Configuration> configb;                 // Binding List genetic type.;
        ITools Tools = new Tools();
        USB_FTDI_Comm myUSBComm;                                // FTDI device manager section (scan, open, close, read, write).
        public SNADNetwork mySNADNetwork;
        public void MyUSBComm(USB_FTDI_Comm myUSBCommRef)       // This to retains reference pointer in Global base (not used in actual code here)
        {
            myUSBComm = myUSBCommRef;
        }
        public USB_FTDI_Comm MyUSBCommRef()
        {
            return myUSBComm;
        }
        //###################################################
        public enum eCompanyName : int
        {
            ADT=0,      
            BGDS=20,
            TVB=50,
            NONE=100
        };
        //###################################################
        public enum eTerminalUpdate : int
        {
            TermREAL = 0,       // Real time update.
            TermSEC1 = 1,       // Update term every 1 second.
            TermSEC2 = 2,       // 2 second.
            TermSEC5 = 5        // 5 second.
        };
        //###################################################
        public enum eSerialDeviceSelect :int
        {
            USB_SPARE=0,
            USB_UART_FTDI=1,              
            USB_UART_VCOM=2               // Added Dec 2015 to support VCOM
        };
        //###################################################
        public enum USBUARTDeviceType : int      // Moved from FTDI code.
        {
            NO_DEVICE = 0,
            USB_UART_FT232R = 10,
            USB_CAN_FT232R = 30
        }
        //################################################### ToolSelect Feb19
        public enum eSelectTools : int      
        {
            GENERIC         = 0,        // LoggerCVS
            ADTESM          = 1,        // EMS Project          (int) GlobalBase.eSelectTools.ADTESM
            ADTRFFDAQ       = 2,        // DataLog
            BGTOCO          = 3,        // Rev 4X Firmaware for TOCO board only. 
            BGTSTOCO        = 4,        // Rev 5X Firmware fpr TOCO and TS board.
            TVBADIS16460    = 5,    
            OLDDIRECT       = 6         // Old Directional Tools.
        }   
        //------------------------------------------ Baud Rate
        public uint[] iBaudRateList = new uint[10] { 1200, 2400, 4800, 9600, 19600, 57600, 115200, 576000, 1152000, 3456000};
        
        // ==================================================================Getter/Setter
        //-----------------------Company Config
        public UInt32 CompanyName                   { get; set; }
        //-----------------------Can Related
        //----------------------------Comm
        public List<string> sUSBDeviceName;
        public bool UDTerm_DMFP_ShowSyncCallBack    { get; set; }           // show or hide '-' type message on terminal box
        public int USB_SelectDeviceMode             { get; set; }           // Select VCOM or FTDI
        public int USBArray_RedirectDebugTerm       { get; set; }           // This channel UDT terminal to generic USB (-1) or specific USB Array Port.
        public bool is_Serial_Server_Connected      { get; set; }
        public bool is_Terminal_Protocol_Activated  { get; set; }
        //public bool is_Baudrate_56K            { get { return m_is_Baudrate_56K; } set { m_is_Baudrate_56K = value; } }
        public UInt32 Select_Baudrate               { get; set; }           // 0 = 1200,2400,4800,9600,19600,57600,6 = 115200
        public bool isUser_Baudrate                 { get; set; }           // 1 = User Baud Rate, 0 = List Baud Rate
        public UInt32 SelectTools                   { get; set; }           // See enum eSelectTools
        public string sFilename                     { get; set; }
        public string sFoldername                   { get; set; }
        public bool bNoRFTermMessage                { get; set; }           // False = enable RFTerm Display, True = Do not display. 
        public UInt32 WaitCyclePeriodSetting         { get; set; }

        public bool bisHideCallBackChecked          { get; set; }

        //---------------------------Logger
        public bool IsLogEnable                     { get; set; }
        //----------------------------Hex Display Management
        public bool IsHexDisplayEnable              { get; set; }
        public bool IsHexDisplayEnableLF            { get; set; }
        public bool IsHexSendEnableWithLF           { get; set; }
        //---------------------------Import Raw Data CVS
        public bool IsImportRawDataEnable           { get; set; }
        public bool IsImportRawDataActivate         { get; set; }

        //---------------------------Session
        public string sSessionFoldername            { get; set; }
        public bool bSessionHideSessionTerm         { get; set; }          // true, do not display session log, false display session log
        public bool bSessionHideStealthTerm         { get; set; }          // true, do not display session log, false display session log
        public bool bHexFormatModBusCRC             { get; set; }          // true, add two byte CRC to message, false = do not add. 
        public bool bSessionDataLogTerm             { get; set; }          // true: hide LD/LE datalog stream, do not add to display into term.
        public bool DataLog_isSurveyCVSOpen         { get; set; }           // Open and close window only

        //----------------------------Window Management
        public bool IsApplicationQuit               { get; set; }
        public bool IsCompactViewEnable             { get; set; }
        //-----------------------------Logger Window
        public bool LoggerOpertaionMode             { get; set; }
        public bool LoggerMiniOpertaionMode         { get; set; }       // Added 22/10/17
        public bool LoggerWindowVisable             { get; set; }       // Added 6/2/16
        public bool LoggerAsyncMode                 { get; set; }
        public bool LoggerAsyncSnadMode             { get; set; }       // Added 76L 7/1/19, new feature for SNAD Networking. 
        public bool LoggerCVSADIS16460Enable        { get; set; }       // False, ADIS16460 not used for LoggerCVS. True, channel LoggerCVS to this device.
        public bool LoggerCVSMiniPortEnable         { get; set; }       // False, MiniPort not used for LoggerCVS. True, channel LoggerCVS to this device.
        public int  LoggerCVS_STCSTARTMode          { get; set; }       // STCSTART(MODE; UDT; PERIOD), default = 2 for LoggerCVS generic.
        public bool LoggerCVS_CallBackMode          { get; set; }       // Include + for callback or exclude. default = false, no callback.
        //-----------------------------Client Window
        public bool ClientCodeWindowVisable { get; set; }
        public bool EE_isSurveyCVSRawDataEnable { get; set; }       // Operation control
        public bool EE_isSurveyCVSRawDataActivate { get; set; }

        //-----------------------------FFT Window
        public bool FFTOpertaionMode                { get; set; }
        public bool FFTDataEditorMode               { get; set; }
        public bool ImportCVSOpertaionMode          { get; set; }
        public bool FFTWindowLogScale               { get; set; }

        //-----------------------------User or debug Mode
        public bool UserDebugMode                   { get; set; }

        //------------------------------Application Config Window
        public bool AppConfigOpertaionMode          { get; set; }
        //------------------------------rtbTerm Terminal screen halt update
        public bool isTermScreenHalted              { get; set; }       // false = normal, true = halted.
        public string TermHaltMessageBuf            { get; set; }       // buffer message while halted.
        public int iTerminalUpdateMode              { get; set; }       // Refresh terminal policy. 
        public bool isTermUpdateModeTimerActivated { get; set; }
        //----------------------------- VCOM Setup (FT232RL or VCOM)..Add Dec 2015
        public int VCOMMode                         { get; set; }       // 0 = Use both FTDI and VCOM, 1 = VCOM only , 2 = FTDI only. 
        public bool VCOM_IsChange                   { get; set; }       // COM port change is requested
        public string VCOMPort                      { get; set; }       // String Port Name
        //----------------------------- EEPROM Erase
        public int EE_Sector_Start                  { get; set; }
        public int EE_Sector_Stop                   { get; set; }
        public bool  EE_isEraseOkay                 { get; set; }
        //----------------------------- EEPROM Informatics.
        public int EE_Select_DeviceIC               { get; set; }                 // 1 = Select IC1, 2 = Select IC2 and so on
        public int EE_StartAdd                      { get; set; }                        // Alway start on page 1 (0x0000 0100)
        public int EE_EndAdd                        { get; set; }                          // End address over download log
        public uint EE_MaxAddress                   { get; set; }                      // Max address 0x001F FFFF (2M)
        public uint EE_NoOfPages                    { get; set; }                       // 8192 page for 2M device
        public uint EE_PageSize                     { get; set; }                        // 256 byte per page
        public uint EE_NoOfDevice                   { get; set; }                      // 0 = No Device, 1 = 1 IC, 2 = 2 IC and so on. 

        //----------------------------- Tool Download Survey 
        public bool EE_isSurveyCVSRawLogDataActive  { get; set; }                       // Operation control for Survey (LOG_DOWNLOAD) CVS file. This set the EE_isLogMemFrameTransferMode in code. 
        public bool EE_isLogMemFrameTransferMode { get; set; }                     // When true, the Survey is busy transferring raw data from tools. This redirect message to a specific routine. 
        public bool EE_isSurveyCVSRawDataCalibrationActivate { get; set; }              // Operation control for calibration files
        public int EE_BatteryCapacity               { get; set; }                       // Battery Capacity Left 0x2710 = 10000 = 10.000 %
        public uint EE_BatteryState                 { get; set; }                       // Battery State Flags
        public bool EE_isLogMemPageTransferMode     { get; set; }                       // ESM, Page Transfer from Device's LogMem.

        //############################################################# Survey HMR2300 / ADIS16140
        //----------------------------- Survey Tool HMR2300 
        public string Svy_sFilenameDefaultDownloadedSelected { get; set; }
        public bool Svy_isSurveyCVSRawDataEnable { get; set; }          // Operation control
        public bool Svy_isSurveyCVSRawDataActivate { get; set; }        // Operation control
        //----------------------------- Svy_isSurveyCVSOpen
        public bool Svy_isSurveyCVSOpen { get; set; }                   // Open and close window only
        //-----------------------------Svy_isToolSetupOpen
        public bool Svy_isToolSetupOpen { get; set; }                   // Open and Close window control.
        public bool Svy_isSurveyMode { get; set; }                      // Set true when either connect button is clicked.
        //-----------------------------HMR2300 device
        public bool is_SvyHMRSerial_Server_Connected { get; set; }      // true = connected otherwise false. 
        //-----------------------------BGDrilling device
        public bool is_BGDRILLING_Device_Activated { get; set; }         // true = device is ON and connected otherwise false.
        //-----------------------------ADIS16140 device
        public bool is_SvyADISSerial_Server_Connected { get; set; }     // true = connected otherwise false. 
        //-----------------------------FluxGate Device
        public bool is_SvyFGIISerial_Server_Connected { get; set; }     // true = connected otherwise false. 
        //----------------------------- ADIS16460 Connection 
        public bool is_ADIS16460_Device_Activated { get; set; }         // true = device is ON and connected otherwise false.
        //----------------------------- MiniPort Connection 
        public bool is_MiniPort_Device_Activated { get; set; }         // true = device is ON and connected otherwise false.

        //############################################################# TV: Calibration Window
        //------------------------------- Calibration Window
        public bool Svy_isCalibrationOpen       { get; set; }             // Open and Close window control.
        //############################################################# TV: BGDRILLING
        public bool TV_isBGDRILLINGOpen         { get; set; }
        public bool BGDRILLING_HideTextDisplay  { get; set; }               // Hide Text in Terminal
        public UInt32 TSTOCOParm1               { get; set; }
        public UInt32 TSTOCOParm2               { get; set; }
        //############################################################# TV: HMR2300
        public bool TV_isHMR2300Open            { get; set; }
        public bool HMR_HideTextDisplay         { get; set; }               // Hide Text in Terminal

        //############################################################# TV: FluxGateII
        public bool TV_isFGIIOpen               { get; set; }
        public bool FGII_HideTextDisplay        { get; set; }               // Hide Text in Terminal
        //############################################################# TV: ADIS16140
        public bool TV_isADISOpen               { get; set; }
        public bool ADIS_HideTextDisplay        { get; set; }               // Hide Text in Terminal
        //############################################################# TV: MiniAD7794 / MiniPort Device
        public bool TV_isMiniPortOpen           { get; set; }
        public bool MiniPort_HideTextDisplay    { get; set; }               // Hide Text in Terminal
        public string sMiniAD7794_Filename      { get; set; }             
        public string sMiniAD7794_Foldername    { get; set; }  
        
        public bool bTabWindowEnableUpdate      { get; set; }               // When set, implement global update for all Tab Window and then clear it.             

        //############################################################# TV: Index Serial Port (VCOM/USB)
        public int iSelectedTypeArrayIndex       { get; set; }
        public List<USBVCOMArray> myUSBVCOMArray;          // This is passed to RX/TX side, supported by enum eUSBDeviceType selection.

        //############################################################# Form Management
        //------------------------------ RiscyScope
        public bool Scope_IsFormOpen            { get; set; }              // Open and Close window control.
        //------------------------------ EEPROM Form
        public bool EEPROM_IsFormOpen           { get; set; }             // Open and Close window control.

        //------------------------------ Tab Control Main Feature
        public int MainFeatureSelectedIndex     { get; set; }       // Select Tab: Logger, Survey, BGSurvey, etc. (Logger is default)     

        //############################################################# BG Drilling
        //----------------------------- Tool Download Survey 
        public bool LogMemWindowVisable          { get; set; }            // Open and close window only
        //-----------------------------BG Tool Setup
        public bool BG_isToolSetupOpen          { get; set; }            // Open and Close window control.
        //-----------------------------BG Driller
        public bool BG_isDrillerOpen            { get; set; }              // Open and Close window control.
        //-----------------------------BG Driller Viewer
        public bool BG_isDrillerViewerOpen      { get; set; }        // Open and Close window control.
        //-----------------------------BG Report Viewer
        public bool BG_isReportViewerOpen       { get; set; }        // Open and Close window control.
        //-----------------------------BG ToCo Setup Viewer
        public bool BG_isToCoSetupOpen          { get; set; }            // Open and Close window control.
        //-----------------------------BG ToCo Orientation Viewer
        public bool BG_isToCoOrientationOpen    { get; set; }      // Open and Close window control
        //-----------------------------BG ToCo Orientation Viewer
        public bool BG_isToCoToolSerialOpen     { get; set; }      // Open and Close window control
        //-----------------------------BG ToCo Service Viewer
        public bool BG_isToCoServiceOpen        { get; set; }      // Open and Close window control
        //-----------------------------BG ToCo Battery Viewer
        public bool BG_isToCoBatteryOpen        { get; set; }       // Open and Close window control
        //-----------------------------
        public bool isBGEEEXPORTActivate        { get; set; }       // This related to EEPROM EXPORT Operation. 
        public bool isBGEEIMPORTActivate        { get; set; }       // This related to EEPROM IMPORT Operation. 
        //-----------------------------
        //public string sToCo_FolderName { get; set; }              // Used for passing foldername to EE_Downholesurvey() window.
        public UInt32 ToCoSerialNumber          { get; set; }       // Tool Serial Number from Report Frame download. 

        public string BG_TSToco_FolderName      { get; set; }       // Moved from myBGToCoProjectFileManager in BG_Service.


        //############################################################# DMF Protocol
        public bool EE_isDMFP_Mode_Enabled      { get; set; }            // When TRUE, DMPF Protocol in use, which affect VCOM and FTDI COM exchange

        //############################################################# ZMDI/IDT   
        public string zCommon_DefaultFolder     { get; set; }           // Common folder location for various files

        //############################################################# ESM
        public bool ESMConfig_SetupOpen         { get; set; }                // Open and Close window control.
        public bool ESMBlockSeqNo_SetupOpen     { get; set; }                // Open and Close window control.
        public string ESM_Config_FolderName     { get; set; }                // Folder Name
        public string ESM_Config_FilenameName   { get; set; }                // Filename Name

        //############################################################# PopUpForm
        public bool PopUpForm_OpenForm          { get; set; }                // Open and Close window control.
        public string PopUpForm_StringValue     { get; set; }

        //############################################################# CAN-PCAN
        public bool isPCANBasicInstalled        { get; set; }                // Check for dll install or not.
        public bool CanPCAN_OpenForm            { get; set; }                // Open and Close window control.
        public bool CanPCAN_isConnected         { get; set; }                // PCAN connect Status.
        public bool CanPCAN_isHeatBeatEmulationMode { get; set; }            // PCAN in emulation test mode
        public bool CanPCAN_Refresh             { get; set; }                // Open and Close window control.
        public string CanPCAN_Config_FolderName { get; set; }                // Folder Name
        public string CanPCAN_Config_FilenameName { get; set; }              // Filename Name
        public string CanPCAN_TXCycleList_FolderName { get; set; }           // Folder Name
        public string CanPCAN_TXCycleList_FilenameName { get; set; }         // Filename Name
        public string CanPCAN_LogMsg_FolderName { get; set; }                // Folder Name
        public string CanPCAN_LogMsg_FilenameName { get; set; }              // Filename Name

        //############################################################# TimeStamp
        public string TimeStamp_FolderName { get; set; }                    // Folder Name
        public string TimeStamp_FilenameName { get; set; }                  // Filename Name

        //############################################################# 
        //--------------------------------------------------------Terminal manager
        public bool isCanPCANTermScreenHalted   { get; set; }               // false = normal, true = halted.
        public string CanPCANTermHaltMessageBuf { get; set; }               // buffer message while halted.
        public int iCanPCANTerminalUpdateMode    { get; set; }              // Refresh terminal policy. 
        public bool isCanPCANTermUpdateModeTimerActivated { get; set; }

        //############################################################# RiscyTimerII 
        //--------------------------------------------------------
        public bool RiscyTimerII_OpenForm       { get; set; }                // Open and Close window control.
        public string RiscyTimerII_PhoneNoSMS   { get; set; }                // SMS Phone Number.
        public bool RiscyTimerII_SMSEnabled     { get; set; }                // SMS enable state
        public bool RiscyTimerII_Repeat         { get; set; }                // Repeat Period enable state
        public int RiscyTimerII_Period          { get; set; }                // Timer Period.

        #region//==================================================Text Font and Color
        public Font FontWarning;
        public Color ColorWarning;
        public Font FontError;
        public Color ColorError;
        public Font FontResponse;
        public Color ColorResponse;
        public Font FontMessage;
        public Color ColorMessage;
        #endregion

        //############################################################# Ethernet
        public bool ETH_isUDTClientOpen { get; set; }                // Open and Close UDT Client Port
        public bool ETH_messageReceived { get; set; }

        //############################################################# Linux

        public bool LINUX_isLinuxModeEnabled     { get; set; }
        public bool LINUX_isLargeScreenEnabled  { get; set; }
       
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        // ==================================================================Constructor
        public GlobalBase()
        {
            sUSBDeviceName = new List<string>(new string[] { "Generic", "HMR2300", "FGIIA", "FGIIB", "Spare", "MiniAD7794", "BGDRILLING", "Spare7", "Spare8" });
            //----------------------------------------USB Array Ports (with 6 channel indexable VCOM). 
            myUSBVCOMArray = new List<USBVCOMArray>(9)
            {
                new USBVCOMArray((int)eUSBDeviceType.Generic,57600),        // Any tools devices
                new USBVCOMArray((int)eUSBDeviceType.HMR2300,19200),        // 1 = HoneyWell Device.
                new USBVCOMArray((int)eUSBDeviceType.FGII_A,57600),         // 2 = Indian Plate DYI fluxgate. 
                new USBVCOMArray((int)eUSBDeviceType.FGII_B,57600),         // 3 = Coilcraft fluxgate 
                new USBVCOMArray((int)eUSBDeviceType.ADIS,115200),          // 4 = ZMDI MiniPort + ADIS16460, (###TASK: move to MiniAD7794)
                new USBVCOMArray((int)eUSBDeviceType.MiniAD7794,3456000),   // 5 = MiniAD7794, USB direct. 
                new USBVCOMArray((int)eUSBDeviceType.BGDRILLING,115200),
                new USBVCOMArray((int)eUSBDeviceType.Spare7,3456000),
                new USBVCOMArray((int)eUSBDeviceType.Spare8,3456000)

            };
            iSelectedTypeArrayIndex = -1;                           // Low Level DMPF Only selected. Not for UDT debug message, use  USBArray_RedirectDebugTerm below.
            USBArray_RedirectDebugTerm = -1;                        // This direct UDT to generic USB interface (non-Array). Use eUSBDeviceType number to channel UDT debug to there.

            //----------------------------------------USB Option Section
            UDTerm_DMFP_ShowSyncCallBack = false;
            bisHideCallBackChecked = false;
            //----------------------------------------Configuration
            configb = null;
            configb = new BindingList<Configuration>();
            configb.Add(new Configuration());                       // Default Setting if filename not exist.

            //----------------------------------------Session
            bSessionHideSessionTerm = false;
            bSessionHideStealthTerm = false;
            bSessionDataLogTerm = false;
            //----------------------------------------MODBUS
            bHexFormatModBusCRC = true;

            //----------------------------------------VCOM
            VCOMMode = 0;
            VCOM_IsChange = false;
            VCOMPort = "SCAN";
            //----------------------------------------Company Themes
            CompanyName = 0;                         // EFT Image (Default)
            sFoldername = "";
            sFilename = "";
            //---------------------------------------- UDT Stuff
            is_Serial_Server_Connected = false;
            WaitCyclePeriodSetting = 100;             // Default setting to wait for device to send UART message. This is required for MWD Directional where SERMEM test is involved.
            FFTWindowLogScale = false;
            FFTOpertaionMode = false;
            ImportCVSOpertaionMode = false;
            IsApplicationQuit = false;
            IsHexDisplayEnable = false;
            IsHexDisplayEnableLF = false;
            IsHexSendEnableWithLF = false;
            IsImportRawDataEnable = false;
            IsCompactViewEnable = false;              // Wide View for 8 tab 
            UserDebugMode = true;                        // Debug Mode.
            //---------------------------------------- LoggerCVS
            LoggerAsyncMode = false;
            LoggerCVS_STCSTARTMode = 2;             // Alway LoggerCVS. 
            LoggerCVS_CallBackMode = false;
            //---------------------------------------- EEPROM
            EE_Sector_Start = 0;
            EE_Sector_Stop = 0;        // No text entry. 
            EE_isEraseOkay = false;
            EE_isSurveyCVSRawLogDataActive = false;
            EE_isLogMemFrameTransferMode = false;
            EE_isSurveyCVSRawDataCalibrationActivate = false;
            EE_isLogMemPageTransferMode = false;
            isBGEEEXPORTActivate = false;
            isBGEEIMPORTActivate = false;
            //---------------------------------------- EEPROM
            EE_isSurveyCVSRawDataEnable = false;
            EE_isSurveyCVSRawDataActivate = false;
            //----------------------------------------Form Control
            LogMemWindowVisable = false;
            EEPROM_IsFormOpen = false;
            Scope_IsFormOpen = false;
            BG_isDrillerOpen = false;
            BG_isToolSetupOpen = false;
            MainFeatureSelectedIndex = 0;
            //----------------------------------------BGDRILLING
            is_BGDRILLING_Device_Activated = false;
            TV_isBGDRILLINGOpen = false;
            BGDRILLING_HideTextDisplay = false;
            BG_isToCoSetupOpen = false;
            BG_isReportViewerOpen = false;
            //sToCo_FolderName = "";
            //----------------------------------------HMR2300
            TV_isHMR2300Open = false;
            HMR_HideTextDisplay = false;
            //----------------------------------------ADIS16460
            is_ADIS16460_Device_Activated = false;
            LoggerCVSADIS16460Enable = false;
            //----------------------------------------FluxGateII
            TV_isFGIIOpen = false;
            FGII_HideTextDisplay = false;
            //----------------------------------------ADIS IMU
            TV_isADISOpen = false;
            ADIS_HideTextDisplay = false;
            //----------------------------------------Calibration Window
            Svy_isCalibrationOpen = false;
            //----------------------------------------DMFP
            EE_isDMFP_Mode_Enabled = false;
            //----------------------------------------ESM
            ESMConfig_SetupOpen = false;
            ESM_Config_FilenameName = "";
            ESMBlockSeqNo_SetupOpen = false;
            //----------------------------------------CanPCAN
            CanPCAN_OpenForm = false;
            CanPCAN_Config_FilenameName = "";
            CanPCAN_LogMsg_FilenameName = "";
            CanPCAN_Refresh = false;
            CanPCAN_isConnected = false;
            CanPCAN_isHeatBeatEmulationMode = false;
            //----------------------------------------MiniPort
            is_MiniPort_Device_Activated = false;
            LoggerCVSMiniPortEnable = false;
            bTabWindowEnableUpdate = false;
            //---------------------------------------RiscyTimerII
            RiscyTimerII_OpenForm = false;
            RiscyTimerII_PhoneNoSMS = "+15877780425";
            RiscyTimerII_SMSEnabled = true;
            RiscyTimerII_Repeat = false;
            RiscyTimerII_Period = 10;
            //--------------------------------------Ethernet
            ETH_isUDTClientOpen = false;
            ETH_messageReceived = false;
            //--------------------------------------Linux
            LINUX_isLinuxModeEnabled = false;
            LINUX_isLargeScreenEnabled = false;

            //----------------------------Common Folder.
            //EVKItApp_DefaultFolder = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents");

            #region//==================================================path
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            #endregion

            #region//==================================================OS Version
            if (Environment.OSVersion.Version.Major >= 6)       //Window 7/8/10 onward.
            {
                zCommon_DefaultFolder = Directory.GetParent(path).ToString();
                zCommon_DefaultFolder = Path.Combine(zCommon_DefaultFolder, "Documents");
                //ESM_Config_FolderName = zCommon_DefaultFolder;
            }
            else
            {

            }
            #endregion

            ESM_Config_FolderName = "";     // Tools.GetPathUserFolder() + "\\ESMProject";
            CanPCAN_Config_FolderName = "";     // Tools.GetPathUserFolder() +"\\ESMProject";
            CanPCAN_LogMsg_FolderName = "";     // Tools.GetPathUserFolder()+"\\ESMProject";
            CanPCAN_TXCycleList_FolderName = "";     // Tools.GetPathUserFolder()+"\\ESMProject";
            // 78D: Use config option now 

            #region//==================================================Text Font and Color
            FontWarning = new Font("Consolas", 10, FontStyle.Bold);
            ColorWarning = Color.Red;
            FontError = new Font("Consolas", 10, FontStyle.Bold);
            ColorError = Color.Red;
            FontResponse = new Font("Consolas", 10, FontStyle.Regular);
            ColorResponse = Color.Lime;
            FontMessage = new Font("Consolas", 10, FontStyle.Regular);
            ColorMessage = Color.Yellow;
            #endregion
        }


        //#########################################################################################################
        //##########################################################################################Serialization (XML)
        //#########################################################################################################
        // Note: XMLSerialization do not work with Arraylist and List<>.
        //       Except BinarySerialization (faster and more compact). 


        #region //------------------------------------------------------SerializeToFile
        public void SerializeToFile<T>(T item, string xmlFileName)
        {
            if (typeof(T).IsSerializable)       // Check before applying, in case class has missing [Serializable]
            {
                using (FileStream stream = new FileStream(xmlFileName, FileMode.Create, FileAccess.Write))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stream, item);
                }
            }
        }
        #endregion

        #region //------------------------------------------------------DeserializeFromFile
        // This is better solution since there is no IO filename exception error. 
        public T DeserializeFromFile<T>(string filenme) where T : class
        {
            T result = null;
            if (typeof(T).IsSerializable)            // Check before applying, in case class has missing [Serializable]
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (XmlTextReader reader = new XmlTextReader(filenme))
                {
                    try
                    {
                        result = (T)serializer.Deserialize(reader);
                        //Console.WriteLine("-INFO: Deserialization successful! Got string: \n{0}", result);
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("###ERR: Failed to deserialize object!!");
                    }
                }
            }
            return result;
            //return default(T); or this method, not sure which one best....! (see expert exchange)
        }
        #endregion

        //-----------------------------------------------------------------------------------Below is improved version from above.
        //-------------https://codereview.stackexchange.com/questions/78191/simple-generic-output-for-deserializer 
        #region //------------------------------------------------------MiniSerializeToFile
        public void MiniSerializeToFile<T>(T obj, string fullPath) where T : class, new()
        {
            if (Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                using (var stream = new FileStream(fullPath, FileMode.Create))
                using (var writer = new XmlTextWriter(stream, Encoding.Unicode))
                {
                    var xmlSerializer = new XmlSerializer(obj.GetType());
                    xmlSerializer.Serialize(writer, obj);
                }
            }
        }
        #endregion

        #region //------------------------------------------------------MiniDeserializeFromFile
        public T MiniDeserializeFromFile<T>(string fullPath) where T : class, new()
        {
            if (File.Exists(fullPath))
            {
                using (var stream = new FileStream(fullPath, FileMode.Open))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(stream);
                }
            }
            return default(T);
        }
        #endregion

        //-----------------------------------------------------------------------------------BinaryType Serialization which work for List object
        #region //------------------------------------------------------SerializeToFileBinary
        public void SerializeToFileBinary<T>(T item, string xmlFileName)
        {
            if (typeof(T).IsSerializable)       // Check before applying, in case class has missing [Serializable]
            {
                using (FileStream stream = new FileStream(xmlFileName, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, item);
                }
            }
        }
        #endregion

        #region //------------------------------------------------------DeserializeFromFileBinary
        // This is better solution since there is no IO filename exception error. 
        public T DeserializeFromFileBinary<T>(string filenme) where T : class
        {
            T result = null;
            if (typeof(T).IsSerializable)            // Check before applying, in case class has missing [Serializable]
            {
                using (Stream stream = File.Open(filenme, FileMode.Open))
                {
                    try
                    {
                        BinaryFormatter bin = new BinaryFormatter();
                        result = (T)bin.Deserialize(stream);
                        //Console.WriteLine("-INFO: Deserialization successful! Got string: \n{0}", result);
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("###ERR: Failed to deserialize object!!");
                    }
                }
            }
            return result;
            //return default(T); or this method, not sure which one best....! (see expert exchange)
        }
        #endregion


        //#########################################################################################################
        //##########################################################################################Bit Access tools
        //#########################################################################################################
        //===================================================================Bit Access tools
        //--------------------UInt32, overload.
        public UInt32 Set(UInt32 ba, int bit)
        {
            ba |= 1U << bit;
            return ba;
        }
        public UInt32 Clear(UInt32 ba, int bit)
        {
            UInt32 mask = 1U << bit;
            mask = ~mask;
            ba &= mask;
            return ba;
        }
        public bool IsSet(UInt32 ba, int bit)
        {
            UInt32 mask = 1U << bit;
            return (ba & mask) != 0;
        }

    }

    //#########################################################################################################
    //########################################################################################## Class USBVCOMArray (multi index serial port)
    //#########################################################################################################

    #region//==================================================Class USBVCOMArray
    public class USBVCOMArray
    {
        public Int32 ePortType { get; set; }                    // Type of port for given device application, in sync with eNum eUSBDeviceType
        //public bool isEnable { get; set; }                      // To be deleted, obslete use, use isSerial_Server_Connected instead.
        public string ComPort { get; set; }                     // ComPort Number to sync with device
        public bool isEndOfLine { get; set; }                   // End of Line indicator.
        public bool isDMFProtocolEnabled { get; set; }          // True where DMFP protocol is activated and goes to ComMSG_Designate_ReadData_To_App() routine, otherwise stay in Array style protocol. 
        public string MessTemp { get; set; }                    // Message Buffer, accumulator.
        public string MessTempRX { get; set; }                  // Message Buffer, received data with \r\n
        public bool IsApplicationQuit { get; set; }             // Set flag to call for quits/ close VCOM port
        public int BaudRate { get; set; }                       // BaudRate setting
        public bool isSerial_Server_Connected { get; set; }     // VCOM server is activated and in use.
        public int SurveyProcessState { get; set; }             // 0 = deactivated channel, not used, 1 = Command sent out, 2 = Recieved data. 

        //---------------------------------------------Constructor
        public USBVCOMArray(int PortType, int iBaudRate)
        {
            ePortType = PortType;
            //isEnable = false;
            ComPort = "";
            isEndOfLine = false;
            MessTemp = "";
            MessTempRX = "";
            IsApplicationQuit = false;
            BaudRate = iBaudRate;
            isSerial_Server_Connected = false;
            SurveyProcessState = 0;
            isDMFProtocolEnabled = false;                       // DMFP Protocol assumed not actvated (will not enter into ComMSG_Designate_ReadData_To_App() routine). 
        }
    }
    // List<string> sUSBDeviceName, don't forget to update this name list!!
    public enum eUSBDeviceType : int
    {
        Generic = 0,
        HMR2300 = 1,    // AMR type magnetometer. 
        FGII_A = 2,     // Fluxgate chassis II with india plate (home made)
        FGII_B = 3,     // Fluxgate chassis II with coilcraft 
        ADIS = 4,       // IMU Module
        MiniAD7794 = 5, // MiniAD7794 Project.
        BGDRILLING = 6, // BGDrilling Tools, TOCO, COCO, etc. 
        Spare7 = 7,
        Spare8 = 8
    };
    #endregion

    //#########################################################################################################
    //########################################################################################## DMFP Bulk Frame Class 
    //#########################################################################################################

    #region -- DMFP_Bulk_Message_Frame --
    public class DMFP_BulkFrame
    {
        // This is new style Getter and Setter for modern .NET typestyle. 
        // ==================================================================Variable as part of XML Configuration Data


        // ==================================================================Getter/Setter
        public int DMFPBulkMessageCounter { get; set; }         // Bulk Message Stream Counter.
        public int DMFP_USBDeviceChannel { get; set; }          // Selected USB device
        public int DMFP_Delay { get; set; }                     // Delay between frame
        public bool isBusy { get; set; }                        // TRUE, message processing is busy. FALSE = Completed.
        public bool isNoErrorReport { get; set; }               // TRUE, disable Error Report.

        public List<string> DMFPStreamTX;                       // Bulk Message String

        public List<DMFP_Delegate> DMFP_Delegate_CallBack;

        // ==================================================================constructor
        public DMFP_BulkFrame()
        {
            DMFP_Delay = 50;                // Default.
            DMFPStreamTX = new List<string>();
            DMFP_Delegate_CallBack = new List<DMFP_Delegate>();
            isNoErrorReport = false;
            isBusy = false;
        }
        public void AddItem(string DMFPStreamTXRef, DMFP_Delegate DMFP_Delegate_CallBackRef)
        {
            DMFPStreamTX.Add(DMFPStreamTXRef);
            DMFP_Delegate_CallBack.Add(DMFP_Delegate_CallBackRef);
        }
    }

    #endregion

    //#########################################################################################################
    //########################################################################################## UDT Configuration Class 
    //#########################################################################################################

    #region -- Configuration Class --
    // Set of class object's of member being transformed into element in XML.
    // When change take place, ie added element (member), must delete old XMLConfig file. 
    [Serializable]
    public class Configuration
    {
        // This is new style Getter and Setter for modern .NET typestyle. 
        // ==================================================================Variable as part of XML Configuration Data


        // ==================================================================Getter/Setter
        public int      CompanyID { get; set; }
        public string   CompanyString { get; set; }
        public int      Test1 { get; set; }
        public string[] ListCompanyName { get; set; }
        public int[]    ListCompanyID { get; set; }
        public UInt32   Select_Baudrate { get; set; }
        public UInt32   SelectTool { get; set; }
        //------------------------------------------78D 
        public string   DefaultFolder { get; set; }
        public UInt32   User_Baudrate { get; set; }
        public int      Select_TabPage { get; set; }
        //------------------------------------------
        public bool bOptionAutoCopyRXMessageToClipBoard { get; set; }

        public bool bOptionSkipFTDIScan { get; set; }

        public bool bOptionAutoDetectEnable { get; set; }

        // ==================================================================constructor
        public Configuration()
        {
            ListCompanyName = new string[] { "ADT", "BGDS", "TVB", "NONE" };
            ListCompanyID = new int[] { 0, 20, 50, 100 };
            bOptionSkipFTDIScan = true;
            bOptionAutoDetectEnable = true;
            bOptionAutoCopyRXMessageToClipBoard = true;
            SelectTool = (int) GlobalBase.eSelectTools.GENERIC;
            DefaultFolder = "";     // Use default folder via myDocument. 
            User_Baudrate = 0;      // Not configured.
        }
    }
    #endregion

    //#########################################################################################################
    //########################################################################################## AutoClose Message Box 
    //#########################################################################################################

    #region////==================================================Auto Close Message Box 
    //
    //http://stackoverflow.com/questions/14522540/close-a-messagebox-after-several-seconds
    //var userResult = AutoClosingMessageBox.Show("Yes or No?", "Caption", 1000, MessageBoxButtons.YesNo);
    //  if(userResult == System.Windows.Forms.DialogResult.Yes) 
    //{ 
    // do something
    //}
    public class AutoClosingMessageBox
    {
        System.Threading.Timer _timeoutTimer;
        string _caption;
        DialogResult _result;
        DialogResult _timerResult;
        AutoClosingMessageBox(bool noButton, string text, string caption, int timeout, MessageBoxButtons buttons = MessageBoxButtons.OK, DialogResult timerResult = DialogResult.None)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            _timerResult = timerResult;
            if (noButton == true)
                _result = MessageBox.Show(text, caption, buttons);
            else
                MessageBox.Show(text, caption);
        }
        public static DialogResult Show(string text, string caption, int timeout, MessageBoxButtons buttons = MessageBoxButtons.OK, DialogResult timerResult = DialogResult.None)
        {
            return new AutoClosingMessageBox(true, text, caption, timeout, buttons, timerResult)._result;
        }
        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox(false, text, caption, timeout);
        }

        void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
            _result = _timerResult;
        }
        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }
    #endregion

}
