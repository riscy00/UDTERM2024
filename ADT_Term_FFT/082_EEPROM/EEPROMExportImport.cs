using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//-------------------------------Window Support
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Timers;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using JR.Utils.GUI.Forms;
using Timer = System.Timers.Timer;
using UDT_Term;

namespace UDT_Term_FFT
{
    public class EEPROMExportImport
    {
        //#####################################################################################################
        //###################################################################################### Variable/Classes
        //#####################################################################################################
        ITools Tools = new Tools();
        //---------------------------------------Filename
        string sDefaultFolder;              // Initial folder. 
        string sFilename;                   // Both Load and Save
        string DownloadSurveyFilename;      // From downloadsurvey (BG), actual filename to be used. 
        //---------------------------------------
        private List<string> sFrame;
        private string sLoadData;
        public string sSaveData;
        private int iFrameIndex;
        private int checksumerrorloop;
        private int checksumfailedcount;

        //#####################################################################################################
        //###################################################################################### Reference 
        //#####################################################################################################

        #region //==================================================Reference
        GlobalBase myGlobalBase;
        MainProg myMainProg;
        DMFProtocol myDMFProtocol;
        USB_VCOM_Comm myUSBVCOMComm;
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        public void MyDMFProtocol(DMFProtocol myDMFProtocolRef)
        {
            myDMFProtocol = myDMFProtocolRef;
        }
        public void MyUSBVCOMComm(USB_VCOM_Comm myUSBVCOMCommRef)
        {
            myUSBVCOMComm = myUSBVCOMCommRef;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Public Variable 
        //#####################################################################################################

        #region //==================================================Getter/Setter
        public UInt32 LPC1549_Start_Address { get; set; }
        public UInt32 LPC1549_MaxCapacity { get; set; }
        public UInt32 LPC1549_Start_Offset { get; set; }
        public UInt32 LPC1549_ByteFrame { get; set; }

        #endregion


        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public EEPROMExportImport()
        {
            LPC1549_Start_Address = 0x3200000;
            LPC1549_MaxCapacity = 0x0E00;            // 0x0200 to 0x0FFF = 3584 Byte (896 Words)  
            LPC1549_Start_Offset = 0x0200;           // 0x0000 to 0x01FF = reserved for internal use.
            LPC1549_ByteFrame = 0;

            sDefaultFolder = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                sDefaultFolder = Directory.GetParent(sDefaultFolder).ToString();
            }
            sFilename = sDefaultFolder;
        }
        #region //==================================================EEPROMExportImport_Init
        public void EEPROMExportImport_Init()
        {


        }

        #endregion

        //#################################################################################################################
        //################################################################################################################# Import Data Section
        //#################################################################################################################

        #region //==================================================EEPROM_ButtonClicked_ImportDataNow
        public void EEPROM_ButtonClicked_ImportDataNow()
        {
            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            DialogResult response = FlexiMessageBox.Show("Import Calibration Data from LPC1549's EEPROM into UDT and save file"
            + Environment.NewLine + "Yes = Proceed to import Calibration Data."
            + Environment.NewLine + "No = Cancel Procedure.",
            "Operation Mode",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);
            if (response == DialogResult.No)
            {
                myMainProg.myRtbTermMessageLF("#E: Import to UDT; Operation Cancelled due to user response");
                return;
            }
            EEPROM_ImportDataNow(false,"",0);
        }

        #endregion

        #region //==================================================EEPROM_ImportDataNow
        int EEPROM_ImportCalDataReturnMethod=0;
        public void EEPROM_ImportDataNow(bool NoRFTermMessage, string CalFilefilename, int returnmethod)      // NoRFTermMessage= true, do not enable term message.
        {
            myGlobalBase.bNoRFTermMessage = NoRFTermMessage;
            DownloadSurveyFilename = CalFilefilename;
            EEPROM_ImportCalDataReturnMethod = returnmethod;        // 0 = no return method, 1 = return to Orientation section code
            iFrameIndex = 0;
            checksumerrorloop = 0;
            checksumfailedcount = 0;

            myGlobalBase.isBGEEIMPORTActivate = true;
            //------------------------------------Issue Command to Device. We use callback to keep data transfer in sync. 
            iFrameIndex = 0;
            sSaveData = "";
            //------------------------------------Init sFrame
            if (sFrame == null)                            // if Frame is null then init. 
                sFrame = new List<string>();
            else
                sFrame.Clear();
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_EEPROM_IMPORT_STAGE1 = new DMFP_Delegate(DMFP_EEPROM_IMPORT_STAGE1_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+EEIENIMPORT()";
            if (myGlobalBase.bNoRFTermMessage == false)
                myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t1;
            if (myGlobalBase.is_Serial_Server_Connected == true)
                t1 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_IMPORT_STAGE1, sMessage, 250));
            else
                t1 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_EEPROM_IMPORT_STAGE1, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t1.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
        }

        #endregion


        #region //==================================================DMFP_EEPROM_IMPORT_STAGE1_CallBack  // Activate the Import Mode. 
        public void DMFP_EEPROM_IMPORT_STAGE1_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Status = hPara[0];
            if (EEDataStatus_isCmdSuccess(Status) == false)
            {
                myMainProg.myRtbTermMessageLF("#E: Cancelled, due to Export Process Error: Registered Command Error or corrupt data");
                EEPROM_Cancel_ExportImportMode();
                if (EEPROM_ImportCalDataReturnMethod == 1)
                    myMainProg.ToolFace_PreStart_AfterCalDataLoad();
                return;
            }
            if (EEDataStatus_isImportMode(Status) == false)
            {
                myMainProg.myRtbTermMessageLF("#E: Cancelled, due to Export Process Error: Import Mode not Activated. ");
                EEPROM_Cancel_ExportImportMode();
                if (EEPROM_ImportCalDataReturnMethod == 1)
                    myMainProg.ToolFace_PreStart_AfterCalDataLoad();
                return;
            }
            myGlobalBase.EE_isSurveyCVSRawLogDataActive = false;
            myGlobalBase.EE_isLogMemFrameTransferMode = false;
            myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate = true;
            DMFP_Delegate fpCallBack_EEPROM_IMPORT_STAGE2 = new DMFP_Delegate(DMFP_EEPROM_IMPORT_STAGE2_CallBack);  //Import Data
                                                                                                                    //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+EEIIMPORT(0x" + iFrameIndex.ToString("X") + ")";
            if (myGlobalBase.bNoRFTermMessage == false)
                myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
            else
                myMainProg.myRtbTermMessageLF("\n-I: Downloading Cal File:");
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t1;
            if (myGlobalBase.is_Serial_Server_Connected == true)
                t1 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_IMPORT_STAGE2, sMessage, 250));
            else
                t1 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_EEPROM_IMPORT_STAGE2, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t1.Start();
            myDMFProtocol.DMFProtocol_UpdateList();


        }
        #endregion

        #region //==================================================DMFP_EEPROM_IMPORT_STAGE2_CallBack      // Loop Transfer
        public void DMFP_EEPROM_IMPORT_STAGE2_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UDT_Term.DialogSupport mbOperationBusy = new DialogSupport();
            if (sPara.Count !=0)
                sSaveData += sPara[0];
            if ((sSaveData.Contains("&E&")==true) | (sPara.Count == 0))
            {
                EEPROM_Cancel_ExportImportMode();
                myMainProg.myRtbTermMessageLF("-I: Import from Tool: Completed Data. No of Transfer:"+ iFrameIndex.ToString());
                sSaveData = sSaveData.Replace("&E&", "");
                sSaveData = sSaveData.Replace((char)0xE, '\n');     //
                sSaveData = sSaveData.Replace((char)0xF, '\r');     // 
                sSaveData = sSaveData.Replace((char)0x10, ',');     //
                sSaveData = sSaveData.Replace((char)0x11, '(');     // 
                sSaveData = sSaveData.Replace((char)0x12, ')');     // 
                sSaveData = sSaveData.Replace((char)0x13, ';');     //
                EEPROM_SaveCVSFile();
                //-------------------------------------------------------------------------------
                switch (EEPROM_ImportCalDataReturnMethod)
                {
                    case (1):       //CO Only
                        {
                            myMainProg.ToolFace_LoadCalData_SaveToFile();
                            myMainProg.ToolFace_PreStart_AfterCalDataLoad();
                            break;
                        }
                    case (2):       //TO Only
                        {
                            myMainProg.ToolFace_LoadCalData_SaveToFile();
                            break;
                        }
                    default:
                        {
                            mbOperationBusy.DoneThenCloseMessageBox(2);
                            break;
                        }
                }
                return;
            }
            //---------------------------------------------------------------------------
            iFrameIndex++;
            DMFP_Delegate fpCallBack_EEPROM_IMPORT_STAGE2 = new DMFP_Delegate(DMFP_EEPROM_IMPORT_STAGE2_CallBack);  //Import Data
            string sMessage = "+EEIIMPORT(0x" + iFrameIndex.ToString("X") + ")";
            if (myGlobalBase.bNoRFTermMessage == false)
                myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
            else
                myMainProg.myRtbTermMessage(".");
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t1;
            if (myGlobalBase.is_Serial_Server_Connected == true)
                t1 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_IMPORT_STAGE2, sMessage, 250));
            else
                t1 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_EEPROM_IMPORT_STAGE2, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t1.Start();
            myDMFProtocol.DMFProtocol_UpdateList();

        }
        #endregion

        //#################################################################################################################
        //#################################################################################################################Export Section
        //#################################################################################################################

        #region //==================================================EEPROM_ButtonClicked_ExportDataNow
        public void EEPROM_ButtonClicked_ExportDataNow()
        {
            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            DialogResult response = FlexiMessageBox.Show("Export Calibration data (in CVS format) to LPC1549's EEPROM"
            + Environment.NewLine + "Yes = Proceed to erase old data and then export new data into EEPROM."
            + Environment.NewLine + "No = Cancel Procedure."
            + Environment.NewLine + "Warning: 'Yes' may lead to lost existing calibration data!",
            "Operation Mode",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
            if (response == DialogResult.No)
            {
                myMainProg.myRtbTermMessageLF("#E: Export to Tool; Operation Cancelled due to user response");
                return;
            }
            //------------------------------------Load filename
            if (EEPROM_LoadCVSFile()==false)
            {
                myMainProg.myRtbTermMessageLF("#E: Export to Device Operation Cancelled due to file error");
                return;
            }
            //------------------------------------Check number of byte to be less than 3568 byte (0x800 x 4 byte words). 
            if (sLoadData.Length>= LPC1549_MaxCapacity)
            {
                myMainProg.myRtbTermMessageLF("#E: Loaded CVS data exceed 2048 Bytes capacity, unable to export.");
                return;
            }
            //------------------------------------Now set UDT to EEPROM Export Mode so that VCOM/FTDI run EEPROM Export Mode messages. 
            myGlobalBase.isBGEEEXPORTActivate = true;
            //------------------------------------Init sFrame
            if (sFrame == null)                            // if Frame is null then init. 
                sFrame = new List<string>();
            else
                sFrame.Clear();
            //------------------------------------Formulating Command, Index, Frame and checksum
            //--- Command: EEIEXPORT(Index, String data, CheckSum);
            string sStartCmd = "+EEIEXPORT(";
            string sEndCmd = ")";
            char[] cc = sLoadData.ToCharArray();        // break up string into char array
            string ss="";
            UInt32 index = 0;
            UInt32 checksum = 0;
            for (int i=0; i< cc.Length; i++)
            {
                if ((i!=0) & (i % 32 == 0))                 // 32 byte threshold, then add to frame to message string with index reference and checksum. 
                {
                    checksum = (checksum + index) & 0x000000FF;   // filter down to 8 bits only. 
                    ss = sStartCmd + "0x" + index.ToString("X") + ";" + ss + ";0x" + checksum.ToString("X") + sEndCmd;
                    sFrame.Add(ss);
                    ss = "";
                    index++;
                    checksum = 0;
                }
                //----------------------Swap two LF and NL to other control ascii (shift in/out) to avoid command decoder issue in LPC1549 where it depend on \n as command terminator. 
                if (cc[i] == '\n')
                    cc[i] = (char) 0xE;        // 14    Shift Out
                if (cc[i] == '\r')
                    cc[i] = (char) 0xF;        // 16    Shift in
                if (cc[i] == ',')
                    cc[i] = (char) 0x10;       // 16    Data Link Escape
                if (cc[i] == '(')
                    cc[i] = (char) 0x11;       // 17    Device Control 1
                if (cc[i] == ')')
                    cc[i] = (char) 0x12;       // 18    Device Control 2
                if (cc[i] == ';')
                    cc[i] = (char) 0x13;       // 19    Device Control 3
                ss += cc[i].ToString();
                checksum += (UInt32) cc[i];
            }

            checksum = (checksum + index) & 0x000000FF;   // filter down to 8 bits only. 
            ss = sStartCmd + "0x" + index.ToString("X") + ";" + ss + ";0x" + checksum.ToString("X") + sEndCmd;       
            sFrame.Add(ss);
            checksum = '&'+'E'+'&';
            index++;
            checksum = (checksum + index) & 0x000000FF;   // filter down to 8 bits only. 
            ss = sStartCmd + "0x" + index.ToString("X") + ";&E&;0x" + checksum.ToString("X") + sEndCmd;             //&E& end of line terminator
            sFrame.Add(ss);
            //------------------------------------Issue Command to Device. We use callback to keep data transfer in sync. 
            iFrameIndex = 0;
            checksumerrorloop = 0;
            checksumfailedcount = 0;
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_EEPROM_EXPORT_STAGE1 = new DMFP_Delegate(DMFP_EEPROM_EXPORT_STAGE1_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+EEIENEXPORT()";
            myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t1;
            if (myGlobalBase.is_Serial_Server_Connected == true)
                t1 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_EXPORT_STAGE1, sMessage, 250));
            else
                t1 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_EEPROM_EXPORT_STAGE1, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t1.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
            
        }
        #endregion

        #region //==================================================DMFP_EEPROM_EXPORT_STAGE1_CallBack  // Activate the Export Mode. 
        public void DMFP_EEPROM_EXPORT_STAGE1_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            string sMessage;
            UInt32 Status = hPara[0];
            if (EEDataStatus_isCmdSuccess(Status) == false)
            {
                myMainProg.myRtbTermMessageLF("#E: Cancelled, due to Export Process Error: Registered Command Error or corrupt data");
                EEPROM_Cancel_ExportImportMode();
            }
            else if (EEDataStatus_isExportMode(Status)==false)
            {
                myMainProg.myRtbTermMessageLF("#E: Cancelled, due to Export Process Error: Export Mode not Activated. ");
                EEPROM_Cancel_ExportImportMode();
            }
            else if (EEDataStatus_isDataBankEmpty(Status) == false)
            { 
                myMainProg.myRtbTermMessageLF("#W: Data Bank is not empty, erasing content ,please wait for few seconds");
                DMFP_Delegate fpCallBack_EEPROM_EXPORT_STAGE2 = new DMFP_Delegate(DMFP_EEPROM_EXPORT_STAGE2_CallBack);  //Erase Content
                //-----------------------------------Place Message to rtbTerm Window.
                sMessage = "+EEIERASE(0x6F)";
                myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
                Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
                //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
                Thread t2;
                if (myGlobalBase.is_Serial_Server_Connected == true)
                    t2 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_EXPORT_STAGE2, sMessage, 250));
                else
                    t2 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_EEPROM_EXPORT_STAGE2, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
                t2.Start();

                myDMFProtocol.DMFProtocol_UpdateList();
            }
            else
            {
                //------Start Data Transfer Loop Process
                DMFP_Delegate fpCallBack_EEPROM_EXPORT_STAGE3 = new DMFP_Delegate(DMFP_EEPROM_EXPORT_STAGE3_CallBack);  //Erase Content
                                                                                                                        //-----------------------------------Place Message to rtbTerm Window.
                sMessage = sFrame[iFrameIndex];
                myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
                Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
                //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
                Thread t3;
                if (myGlobalBase.is_Serial_Server_Connected == true)
                    t3 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_EXPORT_STAGE3, sMessage, 250));
                else
                    t3 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_EEPROM_EXPORT_STAGE3, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
                t3.Start();
                myDMFProtocol.DMFProtocol_UpdateList();
            }

        }
        #endregion

        #region //==================================================DMFP_EEPROM_EXPORT_STAGE2_CallBack  // Erase Content
        public void DMFP_EEPROM_EXPORT_STAGE2_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Status = hPara[0];
            string sMessage;
            if (EEDataStatus_isCmdSuccess(Status) == false)
            {
                myMainProg.myRtbTermMessageLF("#E: Cancelled, due to Export Process Error: Registered Command Error or corrupt data");
                EEPROM_Cancel_ExportImportMode();
            }
            else if (EEDataStatus_isDataBankEmpty(Status) == false)
            {
                myMainProg.myRtbTermMessageLF("#E: Cancelled, due to Export Process Error: Unable to erase EEPROM content ");
                EEPROM_Cancel_ExportImportMode();
            }
            else
            {
                //------Start Data Transfer Loop Process
                DMFP_Delegate fpCallBack_EEPROM_EXPORT_STAGE3 = new DMFP_Delegate(DMFP_EEPROM_EXPORT_STAGE3_CallBack);  //Erase Content
                if (sFrame.Count != 0)
                {//-----------------------------------Place Message to rtbTerm Window.
                    sMessage = sFrame[iFrameIndex];
                    myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
                }
                else
                {
                    myMainProg.myRtbTermMessageLF("#ERR: No CVS data in sFrame[], End Procedure!");
                    return;
                }
                Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
                //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
                Thread t4;
                if (myGlobalBase.is_Serial_Server_Connected == true)
                    t4 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_EXPORT_STAGE3, sMessage, 250));
                else
                    t4 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_EEPROM_EXPORT_STAGE3, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
                t4.Start();
                myDMFProtocol.DMFProtocol_UpdateList();
            }
        }
        #endregion

        #region //==================================================DMFP_EEPROM_EXPORT_STAGE3_CallBack  // Transfer sFrame (loop)
        public void DMFP_EEPROM_EXPORT_STAGE3_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Status = hPara[0];
            if (EEDataStatus_isCheckSumError(Status) == true)
            {
                checksumerrorloop++;
                if (checksumerrorloop >= 3)
                {
                    myMainProg.myRtbTermMessageLF("#E:Too many checksum error loop (3 attempt), Skipped Retry on Frame Index No"+ iFrameIndex.ToString());
                    iFrameIndex++;
                    checksumfailedcount++;
                    checksumerrorloop = 0;
                    if (iFrameIndex == sFrame.Count)
                    {
                        myMainProg.myRtbTermMessageLF("-I: ### Export Process Transfer is Completed ###");
                        EEPROM_Cancel_ExportImportMode();
                        return;
                    }
                }
                else
                {
                    myMainProg.myRtbTermMessageLF("+W: Checksum Error Occurred. Resend and retry. Loop Counter:"+checksumerrorloop.ToString());
                }
            }
            else if (EEDataStatus_isCmdSuccess(Status) == false)
            {
                myMainProg.myRtbTermMessageLF("#E: Cancelled, due to Export Process Error: Registered Command Error or corrupt data");
                EEPROM_Cancel_ExportImportMode();
                return;
            }
            else
            {
                iFrameIndex++;
                checksumerrorloop = 0;
                if (iFrameIndex== sFrame.Count)
                {
                    myMainProg.myRtbTermMessageLF("-I: ### Export Process Transfer Completed ###");
                    EEPROM_Cancel_ExportImportMode();
                    if (checksumfailedcount == 0)
                    {
                        FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        DialogResult response = FlexiMessageBox.Show("Export CSV Successfully Completed!",
                        "Export CSV Procedure Success!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    }
                    else
                    {
                        FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        DialogResult response = FlexiMessageBox.Show("### Export CSV Failure ###"
                            +Environment.NewLine + "###cChecksum Error Occurred ###",
                        "Export CSV Procedure Success!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    }
                    return;
                }
            }
            //------Start Data Transfer Loop Process
            DMFP_Delegate fpCallBack_EEPROM_EXPORT_STAGE3 = new DMFP_Delegate(DMFP_EEPROM_EXPORT_STAGE3_CallBack);  //Erase Content
                                                                                                                    //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = sFrame[iFrameIndex];
            myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t5;
            if (myGlobalBase.is_Serial_Server_Connected == true)
                t5 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_EXPORT_STAGE3, sMessage, 250));
            else
                t5 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_EEPROM_EXPORT_STAGE3, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t5.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
        }
        #endregion

        //#################################################################################################################
        //################################################################################################################# Erase Only
        //#################################################################################################################
        #region //==================================================EEPROM_ButtonClicked_EraseOnly
        public void EEPROM_ButtonClicked_EraseOnly()
        {
            iFrameIndex = 0;
            checksumerrorloop = 0;
            checksumfailedcount = 0;

            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            DialogResult response = FlexiMessageBox.Show("Erase Only (Debug) within LPC1549's EEPROM"
            + Environment.NewLine + "Yes = Proceed to erase within LPC1549's EEPROM (Debug Only)."
            + Environment.NewLine + "No = Cancel Procedure."
            + Environment.NewLine + "Warning: 'Yes' may lead to lost existing calibration data!",
            "Operation Mode",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
            if (response == DialogResult.No)
            {
                myMainProg.myRtbTermMessageLF("#E: Debug Erase; Operation Cancelled due to user response");
                return;
            }
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_EEPROM_ERASE_STAGE1 = new DMFP_Delegate(EEPROM_Action_EraseOnly_CallBack);
            //-----------------------------------
            string sMessage = "+EEIENEXPORT()";
            myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t1 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_ERASE_STAGE1, sMessage, 250));
            t1.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
        }
        #endregion

        #region //==================================================EEPROM_Action_EraseOnly_CallBack
        public void EEPROM_Action_EraseOnly_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        { 
            myMainProg.myRtbTermMessageLF("#W: Erasing contents,please wait for few seconds");
            DMFP_Delegate fpCallBack_EEPROM_ERASE_STAGE2 = new DMFP_Delegate(EEPROM_Post_EraseOnly_CallBack);  //Erase Content
            //-----------------------------------
            string sMessage = "+EEIERASE(0x6F)";
            myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t2 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_ERASE_STAGE2, sMessage, 100));
            t2.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
        }
        #endregion

        #region //==================================================EEPROM_Post_EraseOnly_CallBack
        public void EEPROM_Post_EraseOnly_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Status = hPara[0];
            if (EEDataStatus_isCmdSuccess(Status) == false)
            {
                myMainProg.myRtbTermMessageLF("#E: Cancelled, due to Export Process Error: Registered Command Error or corrupt data");
                EEPROM_Cancel_ExportImportMode();
            }
            else if (EEDataStatus_isDataBankEmpty(Status) == false)
            {
                myMainProg.myRtbTermMessageLF("#E: Cancelled, due to Export Process Error: Unable to erase EEPROM content ");
                EEPROM_Cancel_ExportImportMode();
            }
            else
            {
                myMainProg.myRtbTermMessageLF("-I: Erase procedure completed!");
                EEPROM_Cancel_ExportImportMode();
            }
        }
        #endregion

        //#################################################################################################################
        //################################################################################################################# EEDataStatus
        //#################################################################################################################

        #region //==================================================EEDataStatus_isCmdSuccess
        private bool EEDataStatus_isCmdSuccess(UInt32 Status)     //Return 0 = Command Error, rejected. 1 = Command Successful
        {
            Status = Status & 0x00000001;   //0b0000 0001
            if (Status != 0)
                return true;       // State = 1, return true
            return false;          // State = 0, return false. 
        }
        #endregion

        #region //==================================================EEDataStatus_isExportMode
        private bool EEDataStatus_isExportMode(UInt32 Status)     //Return 1 = ExportMode Enabled.
        {
            Status = Status & 0x00000002;   //0b0000 0010
            if (Status != 0)
                return true;       // State = 1, return true as Export Mode.
            return false;          // State = 0, return false. 
        }
        #endregion

        #region //==================================================EEDataStatus_isImportMode
        private bool EEDataStatus_isImportMode(UInt32 Status)     //Return 1 = ImportMode Enabled.
        {
            Status = Status & 0x00000004;   //0b0000 0100
            if (Status != 0)
                return true;       // State = 1, return true as Import Mode.
            return false;          // State = 0, return false. 
        }
        #endregion

        #region //==================================================EEDataStatus_isDataBankEmpty
        private bool EEDataStatus_isDataBankEmpty(UInt32 Status)     //Return 0 = No, 1 = Yes, bank is empty.
        {
            Status = Status & 0x00000008;   //0b0000 1000
            if (Status != 0)
                return true;       // State = 1, return true bank is empty
            return false;          // State = 0, return false, bank is not empty. 
        }
        #endregion

        #region //==================================================EEDataStatus_isCheckSumError
        private bool EEDataStatus_isCheckSumError(UInt32 Status)     //Return 0 = No Error, 1 = Error, sFrame disposed, need repeat transfer process. 
        {
            Status = Status & 0x00000010;   //0b0001 0000
            if (Status != 0)
                return true;       // State = 1, return true, checksum error occurred. Need resend. 
            return false;          // State = 0, return false, no error. 
        }
        #endregion

        #region //==================================================EEDataStatus_IndexNumber
        private int EEDataStatus_IndexNumber(UInt32 Status)     //Return index number from 0 to 0xFFFE. (0xFFFF = Not relevant). 
        {
            Status = Status & 0xFFFF0000;
            Status = Status >> 16;
            return (int)Status;  
        }
        #endregion

        #region //==================================================EEPROM_Cancel_ExportImportMode
        private void EEPROM_Cancel_ExportImportMode()
        {
            myGlobalBase.bNoRFTermMessage = false;
            myGlobalBase.isBGEEIMPORTActivate = false;
            myGlobalBase.isBGEEEXPORTActivate = false;
            myGlobalBase.EE_isSurveyCVSRawLogDataActive = false;
            myGlobalBase.EE_isLogMemFrameTransferMode = false;
            myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate = false;
            string sMessage = "EEISTATE(0x0)\n";                // Cancel operation (No call back needed). 
            myMainProg.myRtbTermMessageLF("\n"+sMessage);
            if (myGlobalBase.is_Serial_Server_Connected == true)
                myMainProg.Command_ASCII_Process_UARTs(sMessage);
            else
                myUSBVCOMComm.VCOMArray_Message_Send(sMessage, (int)eUSBDeviceType.BGDRILLING);
            if (sFrame!=null)
                sFrame.Clear();                                 // Clear sFrame, data no longer needed.
        }
        #endregion
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        #region //==================================================EEPROM_LoadCVSFile
        public bool EEPROM_LoadCVSFile()
        {
            OpenFileDialog ofdReadSurvey = null;
            myMainProg.myRtbTermMessageLF("-I: Loading Calibration CVS File");
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
                    sDefaultFolder = Path.GetDirectoryName(sFilename);
                }
                else
                {
                    myMainProg.myRtbTermMessageLF("#E: Unable to load file");
                    return false;
                }
                using (StreamReader reader = File.OpenText(ofdReadSurvey.FileName))
                {
                    Encoding sourceEndocing = reader.CurrentEncoding;
                    sLoadData = reader.ReadToEnd();
                }
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("#E: Unable to load file");
                return false;
            }
            return true;
        }
        #endregion

        #region //==================================================BtnOpenFolder_Click
        public void EEPROM_ExploreFolder()
        {
            string myPath = sDefaultFolder;
            try
            {
                
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = myPath;
                prc.Start();

            }
            catch
            {
                myMainProg.myRtbTermMessageLF("#E: Unable to Open Folder:"+myPath);
            }

        }
        #endregion

        #region //==================================================EEPROM_ProjectFolder
        private bool EEPROM_ProjectFolder()
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            try
            {
                //----------------------------------------------------------
                folderBrowserDialog1.SelectedPath = sDefaultFolder;
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    sDefaultFolder = folderBrowserDialog1.SelectedPath;
                    return true;
                }
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("#E: Folder Dialogue Reported an Error");
            }
            return false;

        }
        #endregion

        #region //==================================================EEPROM_SaveCVSFile
        public bool EEPROM_SaveCVSFile()
        {
            if (sSaveData=="")
            {
                myMainProg.myRtbTermMessageLF("#E: Import EEPROM Data: Data is Empty!, Nothing to Save!");
                return false;
            }
            if (DownloadSurveyFilename == "")
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.InitialDirectory = sDefaultFolder;
                saveFileDialog1.Filter = "csv files (*.csv)|*.csv";
                saveFileDialog1.Title = "Save an CSV Import Data into File";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    sFilename = saveFileDialog1.FileName;       // System.IO.Path.Combine(sDefaultFolder, saveFileDialog1.FileName);
                }
            }
            else
            {
                sFilename = DownloadSurveyFilename;
            }
            //-----------------Save Filename now.
            if (sFilename != "")
            {
                try
                {
                    File.WriteAllText(sFilename, sSaveData);                         // Save to filename
                }
                catch
                {
                    myMainProg.myRtbTermMessageLF("#E: File Error: Unable to Save Data, but imported data are copied to clipboard");
                }
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: Filename Save Error, but imported data are copied to clipboard");
            }
            Clipboard.SetDataObject(sSaveData);                                 // Copy (set clipboard)
            return true;
        }

        #endregion

    }
}





