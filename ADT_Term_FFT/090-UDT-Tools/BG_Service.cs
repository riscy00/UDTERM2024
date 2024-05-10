using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UDT_Term;

namespace UDT_Term_FFT
{
    public partial class BG_Service : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        USB_VCOM_Comm myUSBVCOMComm;
        DMFProtocol myDMFProtocol;
        DialogSupport mbOperationBusy;
        BG_ReportViewer myBGReportViewer;
        MainProg myMainProg;
        EEPROMExportImport myEEPROMExpImport;
        EEpromTools myEEpromTools;
        EE_LogMemSurvey myEEDownloadSurvey;
        //BG_ToCo_Setup myBGToCoSetup;
        BG_ToolSerial myBGToCoToolSerial;
        BGToCoProjectFileManager myBGToCoProjectFileManager;
        //UInt32 ToolType;

        #region//================================================================Reference
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        public void MyUSBVCOMComm(USB_VCOM_Comm myUSBVCOMCommRef)
        {
            myUSBVCOMComm = myUSBVCOMCommRef;
        }
        public void MyDMFProtocol(DMFProtocol myDMFProtocolRef)
        {
            myDMFProtocol = myDMFProtocolRef;
        }
        //public void MyBG_ToCo_Setup(BG_ToCo_Setup myBGToCoSetupRef)
        //{
        //    myBGToCoSetup = myBGToCoSetupRef;
        //}
        public void MyEEpromTools(EEpromTools myEEpromToolsRef)
        {
            myEEpromTools = myEEpromToolsRef;
        }
        public void MyEEPROMExportImport(EEPROMExportImport myEEPROMExpImportRef)
        {
            myEEPROMExpImport = myEEPROMExpImportRef;
        }
        public void MyEEDownloadSurvey(EE_LogMemSurvey myEEDownloadSurveyRef)
        {
            myEEDownloadSurvey = myEEDownloadSurveyRef;
        }
        public void MyBGReportViewerSetup(BG_ReportViewer myBGReportViewerRef)
        {
            myBGReportViewer = myBGReportViewerRef;
        }
        public void MyBGToCoProjectFileManager(BGToCoProjectFileManager myBGToCoProjectFileManagerRef)
        {
            myBGToCoProjectFileManager = myBGToCoProjectFileManagerRef;
        }
        public void MyBGToolSerial(BG_ToolSerial myBGToCoToolSerialRef)
        {
            myBGToCoToolSerial = myBGToCoToolSerialRef;
        }
        #endregion

        public BG_Service()
        {
            InitializeComponent();
            mbOperationBusy = new DialogSupport();
        }

        #region //================================================================BG_Service_Load
        private void BG_Service_Load(object sender, EventArgs e)
        {
            UpdateRentalSection();
        }
        #endregion

        #region //================================================================BG_Service_FormClosing
        private void BG_Service_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            myGlobalBase.BG_isToCoBatteryOpen = false;                 // Cease Survey CVS Terminal mode.
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================BG_Service_Show
        public void BG_Service_Show(string FolderName)
        {
            //this.cbToolType.SelectedIndexChanged -= new System.EventHandler(this.cbToolType_SelectedIndexChanged);
            myGlobalBase.BG_TSToco_FolderName = FolderName;
            this.BringToFront();
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            myGlobalBase.BG_isToCoBatteryOpen = true;
            this.Visible = true;
            this.Show();
            //this.cbToolType.SelectedIndexChanged += new System.EventHandler(this.cbToolType_SelectedIndexChanged);
            RefreshEESYSFLAGConfig();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### 
        //#####################################################################################################

        #region //==================================================btnReporView_Click
        private void btnReporView_Click(object sender, EventArgs e)
        {
            RunReportView();
        }
        #endregion

        #region //==================================================RunReportView
        public void RunReportView()
        {
            mbOperationBusy = null;
            mbOperationBusy = new DialogSupport();
            btnReporView.Enabled = false;
            int SettingActual = 0;
            if (myGlobalBase.is_BGDRILLING_Device_Activated == true)
            {
                myBGReportViewer.isEnableShowWindow = true;
                if (myBGReportViewer.Report_Download_WithAttempts() == false)      // 76J: New design with repeat report frame loop, in case TOCO is busy.
                {
                    mbOperationBusy.PopUpMessageBox("ERROR: Report Frame Upload failure or Rental Data Expired/Error", "Report Frame Error", 30, 12F);
                }
                myBGReportViewer.isEnableShowWindow = false;
                int found = 0;
                for (int i = 0; i < 20; i++)
                {
                    Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
                    if (myBGReportViewer.SeekReportFrame_STCSTART() != "")
                    {
                        myMainProg.myRtbTermMessageLF("-INFO: <STC_START> Report frame found");
                        found = 1;
                        break;
                    }
                }
                if (found == 0) // Not found then cannot START or STOP survey.
                {
                    myMainProg.myRtbTermMessageLF("#ERR: <STC_START> Report frame NOT found!");
                }
                UpdateRentalSection();
                SettingActual = RefreshEESYSFLAGConfig();
            }
            if (SettingActual == 3)
            {

            }
            btnReporView.Enabled = true;
        }
        #endregion

        #region //==================================================btnReporViewNow_Click
        private void btnReporViewNow_Click(object sender, EventArgs e)
        {
            myBGReportViewer.isEnableShowWindow = true;
            myBGReportViewer.Report_Show();
            UpdateRentalSection();
            RefreshEESYSFLAGConfig();
            myBGReportViewer.isEnableShowWindow = false;
        }
        #endregion

        #region //==================================================UpdateRentalSection
        private void UpdateRentalSection()
        {
            for (int i = 0; i < myBGReportViewer.ReportClass.ReportData.Count; i++)
            {
                if (myBGReportViewer.ReportClass.ReportData[i].sType.Contains("RD0") == true)
                {
                    tbRD0.Text = myBGReportViewer.ReportClass.ReportData[i].sNote;
                }
                if (myBGReportViewer.ReportClass.ReportData[i].sType.Contains("RD1") == true)
                {
                    tbRD1.Text = myBGReportViewer.ReportClass.ReportData[i].dtUTC.ToString();
                }
                if (myBGReportViewer.ReportClass.ReportData[i].sType.Contains("RD2") == true)
                {
                    tbRD2.Text = myBGReportViewer.ReportClass.ReportData[i].dtUTC.ToString();
                }
                if (myBGReportViewer.ReportClass.ReportData[i].sType.Contains("RD3") == true)
                {
                    string sRD3 = myBGReportViewer.ReportClass.ReportData[i].sNote;
                    UInt32 uRD3 = Tools.ConversionStringtoUInt32(sRD3);
                    //ToolType = (uRD3) >> 28;         // Read only 4 MSB for tooltype. 
                    tbRD3.Text = (uRD3 & 0x0FFFFFFFU).ToString();
                }
                if (myBGReportViewer.ReportClass.ReportData[i].sType.Contains("RD4") == true)
                {
                    string tt = myBGReportViewer.ReportClass.ReportData[i].sNote;       // Convert 0x41414141 to AAAA
                    int ii = Tools.HexStringtoInt32X(tt);
                    byte[] bytes = BitConverter.GetBytes(ii);
                    tbRD4.Text = Tools.ByteArrayToStrASCII(bytes);
                }
            }
        }
        #endregion

        #region //==================================================btn_BGSurvey_ExportCVS_Click
        private void btn_BGSurvey_ExportCVS_Click(object sender, EventArgs e)
        {
            myMainProg.myRtbTermMessageLF("-I: EEPROM Export Mode Procedure Start");
            myEEPROMExpImport.EEPROM_ButtonClicked_ExportDataNow();
        }
        #endregion

        #region //==================================================btnEEDownLoadSurvey_Click
        private void btnEEDownLoadSurvey_Click(object sender, EventArgs e)
        {
            //myEEDownloadSurvey.Show();
            myEEDownloadSurvey.EE_UpdateFolderName(myGlobalBase.BG_TSToco_FolderName);
            myEEDownloadSurvey.EE_LogMemSurvey_Show();

        }
        #endregion

        #region //==================================================BtnOpenFolder_Click
        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            string myPath = myBGToCoProjectFileManager.ToCo_FolderName;
            if (myPath == "")
            {
                myMainProg.myRtbTermMessageLF("#E: FolderName Path Not Defined, applying default location" + myPath);
                string sFoldername = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                try
                {
                    if (!Directory.Exists(sFoldername))                             // Default folder name for given drive. 
                    {
                        DirectoryInfo di = Directory.CreateDirectory(sFoldername);  // Create folder if not exist. 
                        sFoldername = Directory.GetParent(sFoldername).ToString();
                    }
                }
                catch
                {
                    return;
                }
                myBGToCoProjectFileManager.ToCo_FolderName = sFoldername;
                myPath = sFoldername;
            }
            try
            {

                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = myPath;
                prc.Start();

            }
            catch
            {
                myMainProg.myRtbTermMessageLF("#E: Unable to Open Folder:" + myPath);
            }
        }
        #endregion

        #region //==================================================btnEepromTools_Click
        private void btnEepromTools_Click(object sender, EventArgs e)
        {
            myEEpromTools.EEPROMTools_Show();
        }
        #endregion

        #region //==================================================linkLabel1_LinkClicked
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://www.epochconverter.com/");
        }
        #endregion

        #region //==================================================btnRentalUpdateTool_Click
        private void btnRentalUpdateTool_Click(object sender, EventArgs e)
        {
            string RentalUpdate = "+RCWR(";
            double dtUTC1 = 0.0;
            double dtUTC2 = 0.0;
            //UInt32 uToolStypeIndex = (UInt32)cbToolType.SelectedIndex;
            //-----------------------------------------------------------------------------

            for (int i = 0; i < myBGReportViewer.ReportClass.ReportData.Count; i++)
            {
                if (myBGReportViewer.ReportClass.ReportData[i].sType.Contains("RD0") == true)
                {
                    //UInt32 dd = Tools.ConversionStringtoUInt32(tbRD0.Text);
                    //RentalUpdate += dd.ToString("X") + "|";

                    RentalUpdate += "0i" + tbRD0.Text + ";";
                }
                if (myBGReportViewer.ReportClass.ReportData[i].sType.Contains("RD1") == true)
                {
                    if (Tools.IsString_Double(tbRD1.Text))
                    {
                        dtUTC1 = Tools.ConversionStringtoDouble(tbRD1.Text);
                        RentalUpdate += "0i" + tbRD1.Text + ";";
                    }
                    else
                    {
                        mbOperationBusy.PopUpMessageBox("ERROR: RD1 text is not valid UDT format", "ToCo Rental Update", 30, 12F);
                        return;
                    }
                    if (dtUTC1 < 1523801721)
                    {
                        mbOperationBusy.PopUpMessageBox("ERROR: RD1 text is not valid UDT format (must be >1523801721)", "ToCo Rental Update", 30, 12F);
                        return;
                    }
                }
                if (myBGReportViewer.ReportClass.ReportData[i].sType.Contains("RD2") == true)
                {
                    if (Tools.IsString_Double(tbRD2.Text))
                    {
                        dtUTC2 = Tools.ConversionStringtoDouble(tbRD2.Text);
                        RentalUpdate += "0i" + tbRD2.Text + ";";
                    }
                    else
                    {
                        mbOperationBusy.PopUpMessageBox("ERROR: RD2 text is not valid UDT format", "ToCo Rental Update", 30, 12F);
                        return;
                    }
                    if (dtUTC2 < 1523801721)
                    {
                        mbOperationBusy.PopUpMessageBox("ERROR: RD2 text is not valid UDT format (must be >1523801721)", "ToCo Rental Update", 30, 12F);
                        return;
                    }
                    if (dtUTC1 > dtUTC2)
                    {
                        mbOperationBusy.PopUpMessageBox("ERROR: RD1 & RD2 text is not valid due to reverse Start/Expiry UDT\n, RD2 must be more than RD1", "ToCo Rental Update", 30, 12F);
                        return;
                    }
                }
                if (myBGReportViewer.ReportClass.ReportData[i].sType.Contains("RD3") == true)
                {
                    string RD3Text = tbRD3.Text;
                    UInt32 uRD3;
                    if (Tools.IsString_Numberic_UInt32(tbRD3.Text))
                    {
                        uRD3 = Tools.ConversionStringtoUInt32(RD3Text);
                    }
                    else
                    {
                        mbOperationBusy.PopUpMessageBox("ERROR: RD3 must be numeric and less than 268435455), try again", "ToCo Rental Update", 30, 12F);
                        return;
                    }
                    if (uRD3 > 268435455)                         // policy number limit to Bit 0 to Bit 27 range. 
                    {
                        mbOperationBusy.PopUpMessageBox("ERROR: RD3 must be numeric and less than 268435455), try again", "ToCo Rental Update", 30, 12F);
                        return;
                    }
                    //uRD3 = uRD3 + (uToolStypeIndex << 28);      // Bit 31,30,29,28 is reserved for tooltype. 
                    RentalUpdate += "0i" + uRD3.ToString() + ";";
                }
                if (myBGReportViewer.ReportClass.ReportData[i].sType.Contains("RD4") == true)
                {
                    string ss = tbRD4.Text;
                    if (ss.Length != 4)
                    {
                        mbOperationBusy.PopUpMessageBox("ERROR: Password must be 4 ASCII (ie 'PXC5'), try again", "ToCo Rental Update", 30, 12F);
                        return;
                    }
                    // Convert AAAA back to 0x41414141
                    int val = 0;
                    for (int j = 0; j < 4; j++)
                    {
                        int b = (int)ss[j] * (int)Math.Pow(256, j);
                        val += b;
                    }
                    RentalUpdate += "0x" + val.ToString("X") + ")";
                }
            }

            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_RentalDataUpdate = new DMFP_Delegate(DMFP_RentalDataUpdate);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_RentalDataUpdate,
                RentalUpdate,
                false, (int)eUSBDeviceType.BGDRILLING);
        }
        #endregion

        #region //==================================================DMFP_RentalDataUpdate
        public void DMFP_RentalDataUpdate(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFF)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Rental Data Update Error: Code 0x" + hPara[0].ToString("X"), "ToCo Rental Update", 30, 12F);
            }
            else
            {
                mbOperationBusy.PopUpMessageBox("INFO: Rental Data Updated!, Please click <Reload Report View> to verify RD0 to RD4.", "ToCo Rental Update", 5, 12F);
            }
        }
        #endregion

        #region //==================================================btnTestToCoTool_Click
        private void btnTestToCoTool_Click(object sender, EventArgs e)
        {
            mbOperationBusy.PopUpMessageBox("INFO: Still in Development (WIP)", "ToCo Service", 3, 12F);
        }
        #endregion

        #region //==================================================btnResetToolNow_Click
        private void btnResetToolNow_Click(object sender, EventArgs e)
        {
            // Issue tool reset command
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isSerial_Server_Connected == false)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Please performs Connect first with plugged cable (Click <Scan and Connect> Button).", " BGDRILLING USB Connection", 2);     // Close automatically after 2 Sec
                return;
            }
            //---------------------------------------------------------Direct Telegraph Style Command (no response expected)
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            myUSBVCOMComm.VCOMArray_Message_Send("+STCRESET(0x1;0x4224)", (int)eUSBDeviceType.BGDRILLING);
            mbOperationBusy.PopUpMessageBox("INFO: Config has been reset, you may need to disconnect battery/power and reboot", "ToCo Service", 3, 12F);
        }
        #endregion

        #region //==================================================btnSleepMode_Click
        private void btnSleepMode_Click(object sender, EventArgs e)
        {
            // Step 1: Erase logger Memory
            // Step 2: Sleep Mode after disconnection
            // The ToCo LED will cease flashing after 5min, only blink every 5 sec until next connect session or wake up my magnet or light sensor.
            mbOperationBusy.PopUpMessageBox("INFO: Still in Development (WIP)", "ToCo Service", 3, 12F);
        }
        #endregion

        #region //==================================================btnRentalHelp_Click
        private void btnRentalHelp_Click(object sender, EventArgs e)
        {
            // Open PDF document within the file kit, details how to implement rental data, how to apply extension and so on.
            mbOperationBusy.PopUpMessageBox("INFO: Still in Development (WIP), PDF document being generated", "ToCo Service", 3, 12F);

        }
        #endregion

        #region //==================================================btnServiceHelp_Click
        private void btnServiceHelp_Click(object sender, EventArgs e)
        {
            // Open PDF document within the file kit, details about Tool failure, issue, action. 
            mbOperationBusy.PopUpMessageBox("INFO: Still in Development (WIP), PDF document being generated. Support TS/TOCO Firmware 5C onward", "ToCo Service", 3, 12F);
        }
        #endregion

        #region //==================================================btnCalibrationProc_Click
        private void btnCalibrationProc_Click(object sender, EventArgs e)
        {
            mbOperationBusy.PopUpMessageBox("INFO: Still in Development (WIP)", "ToCo Service", 3, 12F);
        }
        #endregion

        #region //==================================================btnOneMonthRental_Click
        private void btnOneMonthRental_Click(object sender, EventArgs e)
        {
            tbRD0.Text = "12345";
            UInt32 datetime = Tools.uDateTimeToUnixTimestampNowLocal();
            tbRD1.Text = datetime.ToString();
            datetime += 2592000U; // 30 Day
            tbRD2.Text = datetime.ToString();
            //tbRD3.Text = "12345";
            string sRD3 = tbRD3.Text;
            UInt32 uRD3 = Tools.AnyStringtoUInt32(sRD3);
            //ToolType = (uRD3 & 0x0FFFFFFF) >> 28;         // Read only 4 MSB for tooltype. 
            //tbRD3.Text = sRD3;
            //cbToolType.SelectedIndex = (int)ToolType;
        }
        #endregion

        #region //==================================================btnToolSerial_Click
        private void btnToolSerial_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.BG_isToCoOrientationOpen == true)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Please close the BG Orientation Window first and try again", "ToCo Battery Mode", 30, 12F);
                return;
            }
            if (myGlobalBase.BG_isToCoToolSerialOpen == false)
                myBGToCoToolSerial.BG_ToolSerial_Show();
        }
        #endregion

        #region //==================================================RefreshEESYSFLAGConfig
        // After report frame (0x13) upload, run below.
        private int RefreshEESYSFLAGConfig()
        {
            UInt32 EESysFlag = myBGReportViewer.ReportClass.EESysFlag;
            UInt32 SettingReq = (EESysFlag >> 16) & 0x00000007;             //Bit 16,17,18. 
            cbSettingRequest.SelectedIndex = (int)SettingReq;
            UInt32 SettingActual = (EESysFlag >> 19) & 0x00000007;             //Bit 19,20,21. 
            cbSettingActual.SelectedIndex = (int)SettingActual;
            UInt32 SettingGyro = (EESysFlag >> 22) & 0x00000003;             //Bit 22,23
            cbSettingGyro.SelectedIndex = (int)SettingGyro;
            UInt32 FGMathMode = (EESysFlag >> 13) & 0x00000007;             //Bit 13,14,15
            cbFGFormat.SelectedIndex = (int)FGMathMode;
            cbSettingRequest.Refresh();
            cbSettingActual.Refresh();
            cbSettingGyro.Refresh();
            cbFGFormat.Refresh();
            return (int)SettingActual;
        }
        #endregion
        #region //==================================================UpdateEESYSFLAGConfig
        // This update the EESYSFLAG prior to write via write command
        private void UpdateEESYSFLAGConfig()
        {
            UInt32 EESysFlag = myBGReportViewer.ReportClass.EESysFlag & 0xFF001FFF; // clear old setting       
            UInt32 SettingReq = (UInt32)cbSettingRequest.SelectedIndex;
            UInt32 SettingGyro = (UInt32)cbSettingGyro.SelectedIndex;
            UInt32 FGMathMode = (UInt32)cbFGFormat.SelectedIndex;
            // Refresh setting. 
            EESysFlag |= (SettingReq << 16);
            EESysFlag |= (SettingGyro << 22);
            EESysFlag |= (FGMathMode << 13);
            myBGReportViewer.ReportClass.EESysFlag = EESysFlag;
        }
        #endregion
        #region //================================================================BtnEESYSFLAGVerify_Click
        private void BtnEESYSFLAGVerify_Click(object sender, EventArgs e)
        {
            if (cbSettingRequest.SelectedIndex == -1)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Do <Read Only> First or update parameter", "TS/TOCO Service", 3, 12F);
                return;
            }
            UpdateEESYSFLAGConfig();
            //Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_DMFP_EESYSFLAGVerify = new DMFP_Delegate(DMFP_EESYSFLAGVerify);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_DMFP_EESYSFLAGVerify, "+EESYSFLAG(0x1;0x" + myBGReportViewer.ReportClass.EESysFlag.ToString("X") + ")", false, (int)eUSBDeviceType.BGDRILLING);
        }
        #endregion
        #region //================================================================DMFP_EESYSFLAGVerify
        // hPara[0] = 0x00 to 0x0F related to Error Code
        //          0x01: Actual Error
        //          0x02: Command Parameter Error
        //          0xFF = Success. 
        // hPara[1] = EESYSFLAG Data.

        public void DMFP_EESYSFLAGVerify(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if ((hPara[0] == 0xFF) | (hPara[0] == 0x01))
            {
                myBGReportViewer.ReportClass.EESysFlag = hPara[1];
                if (RefreshEESYSFLAGConfig() == 0)
                {
                    mbOperationBusy.PopUpMessageBox("ERROR: Actual Config in EESYSFLAG Reported Error, Check your board/setting", "TS/TOCO Service", 3, 12F);
                    cbSettingRequest.SelectedIndex = -1;
                    cbSettingActual.SelectedIndex = -1;
                    cbSettingGyro.SelectedIndex = -1;
                    cbFGFormat.SelectedIndex = -1;
                }
            }
        }
        #endregion
        #region //================================================================BtnEESYSFLAGUpdate_Click
        private void BtnEESYSFLAGUpdate_Click(object sender, EventArgs e)
        {
            if (cbSettingRequest.SelectedIndex == -1)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Do <Read Only> First or update parameter", "TS/TOCO Service", 3, 12F);
                return;
            }
            UpdateEESYSFLAGConfig();
            //Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_DMFP_EESYSFLAGUpdate = new DMFP_Delegate(DMFP_EESYSFLAGUpdate);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_DMFP_EESYSFLAGUpdate, "+EESYSFLAG(0x2;0x" + myBGReportViewer.ReportClass.EESysFlag.ToString("X") + ")", false, (int)eUSBDeviceType.BGDRILLING);

        }
        #endregion
        #region //================================================================DMFP_EESYSFLAGUpdate
        // hPara[0] = 0x00 to 0x0F related to Error Code
        //          0x01: Actual Error
        //          0x02: Command Parameter Error
        //          0xFF = Success. 
        // hPara[1] = EESYSFLAG Data.

        public void DMFP_EESYSFLAGUpdate(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if ((hPara[0] == 0xFF)| (hPara[0] == 0x01))
            {
                myBGReportViewer.ReportClass.EESysFlag = hPara[1];
                if (RefreshEESYSFLAGConfig()==0)
                {
                    mbOperationBusy.PopUpMessageBox("ERROR: Actual Config in EESYSFLAG Reported Error, Check your board/setting", "TS/TOCO Service", 3, 12F);
                    cbSettingRequest.SelectedIndex = -1;
                    cbSettingActual.SelectedIndex = -1;
                    cbSettingGyro.SelectedIndex = -1;
                    cbFGFormat.SelectedIndex = -1;
                }
                else
                {
                    if (hPara[0] == 0xFF)
                        mbOperationBusy.PopUpMessageBox("INFO: Config EEPROM Update Success, check again via Report Frame Reload", "TS/TOCO Service", 3, 12F);
                }
            }
        }
        #endregion
        #region //================================================================BtnEESYSFLAGRead_Click
        private void BtnEESYSFLAGRead_Click(object sender, EventArgs e)
        {
            //Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_DMFP_EESYSFLAGRead = new DMFP_Delegate(DMFP_EESYSFLAGRead);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_DMFP_EESYSFLAGRead, "+EESYSFLAG(0x0;0x0)", false, (int)eUSBDeviceType.BGDRILLING);
        }
        #endregion

        #region //================================================================DMFP_EESYSFLAGRead
        // hPara[0] = 0x00 to 0x0F related to Error Code
        //          0x01: Actual Error
        //          0x02: Command Parameter Error
        //          0xFF = Success. 
        // hPara[1] = EESYSFLAG Data.

        public void DMFP_EESYSFLAGRead(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            //------------------------------Error Reporting Section
            if ((hPara[0] == 0xFF) | (hPara[0] == 0x01))
            {
                myBGReportViewer.ReportClass.EESysFlag = hPara[1];
                RefreshEESYSFLAGConfig();
            }
        }
        #endregion
    }
}
