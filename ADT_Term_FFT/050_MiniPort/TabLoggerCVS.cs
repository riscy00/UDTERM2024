using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Documents;
using System.IO;

namespace UDT_Term_FFT
{
    public partial class TabLoggerCVS : UserControl
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        USB_Message_Manager myUSB_Message_Manager;
        MainProg myMainProg;
        DMFProtocol myDMFProtocol;
        TVMiniPort_LoggerCVS TabLoggerCVSSetup;
        TVMiniPort_TabEnable TabWindowEnable;
        LoggerCSV myLoggerCSV;

        #region//================================================================Reference
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }
        public void MyTabWindowEnable(TVMiniPort_TabEnable TabWindowEnableRef)
        {
            TabWindowEnable = TabWindowEnableRef;
        }
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        public void MyUSB_Message_Manager(USB_Message_Manager myUSB_Message_ManagerRef)
        {
            myUSB_Message_Manager = myUSB_Message_ManagerRef;
        }
        public void MyLoggerCVSSetup(TVMiniPort_LoggerCVS TabLoggerCVSSetupRef)
        {
            TabLoggerCVSSetup = TabLoggerCVSSetupRef;
        }
        public void MyDMFProtocol(DMFProtocol myDMFProtocolRef)
        {
            myDMFProtocol = myDMFProtocolRef;
        }
        public void MyLoggerCVS(LoggerCSV myLoggerCSVRef)
        {
            myLoggerCSV = myLoggerCSVRef;
        }
        #endregion

        //------------------------------------------------------------------------Local Variable
        public TabLoggerCVS()
        {
            InitializeComponent();
        }

        #region//================================================================ADIS16460_Load
        private void TabLoggerCVS_Load(object sender, EventArgs e)
        {
            if (this.cbSelectTrig.Items.Count == 0)
            {
                cbSelectTrig.Items.AddRange(TabLoggerCVSSetup.lSelectTrigger().ToArray<object>());
            }
            LoggerCVS_Refresh_Window_Control();
            LoggerCVS_Refresh_Window_Write();
            chTabEnable.Enabled = false;        // Avoid clicking this (make no sense to disable this feature anyway).
            this.Invalidate();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Tab Enable Manager
        //#####################################################################################################

        #region //=============================================================================================TabWindowEnableImplement 
        public bool TabWindowEnableImplement()  
        {
            if (TabWindowEnable.bLoggerCVS == true)
            {
                chTabEnable.Checked = true;
                LoggerCVS_Refresh_Window_Control();
                return true;
            }
            else
            {
                chTabEnable.Checked = false;
                LoggerCVS_Refresh_Window_Control();
            }
            return false;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Window
        //#####################################################################################################

        #region //=============================================================================================LoggerCVS_Refresh_Window_Control  
        public void LoggerCVS_Refresh_Window_Control()
        {
            if (TabWindowEnable.bLoggerCVS == false)            // Hide Elements
            {
                btnImportSetting.Visible = false;
                btnExportSetting.Visible = false;
                btnStop.Visible = false;
                btnRun.Visible = false;
                gbSelectTrigger.Visible = false;
                gbTimer.Visible = false;
                gbLoggerCVS.Visible = false;
                chTabEnable.Checked = false;
                lbEnableText.Visible = true;
                gbLoggerWindow.Visible = false;
            }
            else                                                // Display Elements
            {
                btnImportSetting.Visible = true;
                btnExportSetting.Visible = true;
                btnStop.Visible = true;
                btnRun.Visible = true;
                gbSelectTrigger.Visible = true;
                gbTimer.Visible = true;
                gbLoggerCVS.Visible = true;
                chTabEnable.Checked = true;
                lbEnableText.Visible = false;
                gbLoggerWindow.Visible = true;
            }
        }
        #endregion

        #region //=============================================================================================LoggerCVS_Refresh_Window_Read  
        public void LoggerCVS_Refresh_Window_Read()
        {
            btnExportSetting.Visible = true;
            tbFolderName.Text = myGlobalBase.sMiniAD7794_Foldername;
            //----------------------------------------------------iCounterToCease
            if (tbCounter.Text == "0=Infinity")
                TabLoggerCVSSetup.iCounterToCease = 0;
            else
            {
                if (Tools.IsString_Numberic_UInt32(tbCounter.Text)==true)
                {
                    TabLoggerCVSSetup.iCounterToCease = Tools.AnyStringtoInt32(tbCounter.Text);
                }
                else
                {
                    TabLoggerCVSSetup.iCounterToCease = 0;
                    tbCounter.Text = "*Error*";
                    btnExportSetting.Visible = false;
                }
            }
            //----------------------------------------------------iSelectTrigger
            TabLoggerCVSSetup.iSelectTrigger = cbSelectTrig.SelectedIndex;
            //----------------------------------------------------iTimerPeroid
            if (Tools.IsString_Numberic_UInt32(tbTimerPeroid.Text) == true)
            {
                TabLoggerCVSSetup.iTimerPeroid = Tools.ConversionStringtoInt32(tbTimerPeroid.Text);
            }
            else
            {
                TabLoggerCVSSetup.iTimerPeroid = 1000;
                tbTimerPeroid.Text = "*Error*";
                btnExportSetting.Visible = false;
            }
            //----------------------------------------------------bAsyncUDTEnabled
            TabLoggerCVSSetup.bAsyncUDTEnabled = cbExporttoUDT.Checked;
            //----------------------------------------------------bAsyncLogEnabled
            TabLoggerCVSSetup.bAsyncLogEnabled = cbLoggerFlash.Checked;
            //----------------------------------------------------sProjectName
            TabLoggerCVSSetup.sProjectName = tbsProjectName.Text;
            if (TabLoggerCVSSetup.sProjectName.Length >= 127)
            {
                TabLoggerCVSSetup.sProjectName = TabLoggerCVSSetup.sProjectName.Substring(0, 127);
            }
            //----------------------------------------------------bIsEnable
            //chTabEnable.Checked = TabLoggerCVSSetup.bIsEnable;

        }
        #endregion

        #region //=============================================================================================LoggerCVS_Refresh_Window_Write  
        public void LoggerCVS_Refresh_Window_Write()
        {
            tbFolderName.Text = myGlobalBase.sMiniAD7794_Foldername;
            //----------------------------------------------------iCounterToCease
            if (TabLoggerCVSSetup.iCounterToCease == 0)
            {
                tbCounter.Text = "0=Infinity";
            }
            else
            {
                tbCounter.Text = TabLoggerCVSSetup.iCounterToCease.ToString();
            }
            //----------------------------------------------------iSelectTrigger
            if (cbSelectTrig.SelectedIndex >= 2)
                TabLoggerCVSSetup.iSelectTrigger = 1;
            cbSelectTrig.SelectedIndex = TabLoggerCVSSetup.iSelectTrigger;
            //----------------------------------------------------iTimerPeroid
            tbTimerPeroid.Text = TabLoggerCVSSetup.iTimerPeroid.ToString();
            //----------------------------------------------------bAsyncUDTEnabled
            cbExporttoUDT.Checked = TabLoggerCVSSetup.bAsyncUDTEnabled;
            //----------------------------------------------------bAsyncLogEnabled
            cbLoggerFlash.Checked = TabLoggerCVSSetup.bAsyncLogEnabled;
            //----------------------------------------------------sProjectName
            if (TabLoggerCVSSetup.sProjectName.Length>=127)
            {
                TabLoggerCVSSetup.sProjectName = TabLoggerCVSSetup.sProjectName.Substring(0, 127);
            }
            tbsProjectName.Text = TabLoggerCVSSetup.sProjectName;
            //----------------------------------------------------bIsEnable
            //chTabEnable.Checked = TabLoggerCVSSetup.bIsEnable;
        }
        #endregion

        #region //=============================================================================================LoggerCVS_InitDefaultSetupFirstTime  
        public void LoggerCVS_InitDefaultSetupFirstTime()
        {
            TabLoggerCVSSetup.LoggerCVS_SetupInit();
            tbFolderName.Text = myGlobalBase.sMiniAD7794_Foldername;
        }
        #endregion

        #region //=============================================================================================cbSelectTrig_SelectedIndexChanged/Mouse Up/Mouse Down
        private void cbSelectTrig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSelectTrig.SelectedIndex <= 1)
                TabLoggerCVSSetup.iSelectTrigger = cbSelectTrig.SelectedIndex;
            else
                cbSelectTrig.SelectedIndex = TabLoggerCVSSetup.iSelectTrigger;
        }
        private void cbSelectTrig_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void cbSelectTrig_MouseDown(object sender, MouseEventArgs e)
        {
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### LoggerCVS Setup
        //#####################################################################################################

        #region //=============================================================================================DMFP_ADISRUNSTOP_CallBack
        public void DMFP_ADISRUNSTOP_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFFFF)
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_LOGCVSRUNSTOP_CallBack()...has Error code: 0x" + hPara[0].ToString("X4") + "| DataCode 0x" + hPara[1].ToString("X4"));
            }
        }

        #endregion

        //#####################################################################################################
        //###################################################################################### Export Setup
        //#####################################################################################################

        #region //=============================================================================================btnExportSetting_Click 
        private void btnExportSetting_Click(object sender, EventArgs e)
        {
            LoggerCVS_Update_Setup_Data();
        }
        #endregion

        #region //=============================================================================================LoggerCVS_Update_Setup_Data & CallBacks
        // Note: ErrorCode must be Hex 0x, all other data in integer (0i)
        //
        //+LOGCVSSETUP(isEnable; AsyncLogFlashEnabled; AsyncUDTEnabled;isSleepEnabled; iSelectTrigger; iTimerPeroid; iCounterToCease)\n
        //-LOGCVSSETUP(ErrorCode;Data)\n
        //+LOGCVSSETUPR()\n
        //-LOGCVSSETUPR(ErrorCode; isEnable; AsyncLogFlashEnabled; AsyncUDTEnabled; isSleepEnabled; iSelectTrigger; iTimerPeroid; iCounterToCease)\n
        //
        //+LOGCVSPROJNAME(<Project Name in String, 128 char max>)\n
        //-LOGCVSPROJNAME(ErrorCode; Data)\n        
        //+LOGCVSPROJNAMER()\n
        //-LOGCVSPROJNAMER(ErrorCode; <String> )\n    
        public DMFP_BulkFrame LoggerCVS_Update_Setup_Data()
        {
            if (TabLoggerCVSSetup == null)
                return (null);
            if (TabLoggerCVSSetup.bIsEnable == true)        //
            {
                myMainProg.myRtbTermMessageLF("#E: On-going real time data collection click <STOP> button first");
                return (null);
            }
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return (null);
            }
            LoggerCVS_Refresh_Window_Read();
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();

            //---------------------------------------------------------------------------------------------------------FCL30 Initial Setup 
            //+ADISSETUP (AccelFormat; GyroFormat; AxiPolicy; AccelMath; isAdvFrame; isEnabled)\n
            string command = "+LOGCVSSETUP(";
            command += (Tools.BinaryFalseTrue(TabLoggerCVSSetup.bIsEnable)).ToString() + ";";
            command += (Tools.BinaryFalseTrue(TabLoggerCVSSetup.bAsyncLogEnabled)).ToString() + ";"; 
           command += (Tools.BinaryFalseTrue(TabLoggerCVSSetup.bAsyncUDTEnabled)).ToString() + ";";
            command += (Tools.BinaryFalseTrue(TabLoggerCVSSetup.isSleepEnabled)).ToString() + ";";
            command += (TabLoggerCVSSetup.iSelectTrigger).ToString() + ";";
            command += (TabLoggerCVSSetup.iTimerPeroid).ToString() + ";";
            command += (TabLoggerCVSSetup.iCounterToCease).ToString() + ")";
            cDMFP_BulkFrame.AddItem(command, new DMFP_Delegate(DMFP_LOGCVSSETUP_CallBack));
            //---------------------------------------------------------------------Project Name Part
            command = "+LOGCVSPROJNAME(0s" + TabLoggerCVSSetup.sProjectName + ")";
            cDMFP_BulkFrame.AddItem(command, new DMFP_Delegate(DMFP_LOGCVSPROJNAME_CallBack));
            //---------------------------------------------------------------------Send Bulk Frame
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 100;
            cDMFP_BulkFrame.isNoErrorReport = true;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);
            //------------------------------------------------------------------------------------
            return cDMFP_BulkFrame;
        }
        #endregion

        #region //=============================================================================================DMFP_LOGCVSSETUP_CallBack and +LOGCVSPROJNAME(....)
        public void DMFP_LOGCVSSETUP_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFFFF)
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_LOGCVSSETUP_CallBack()...has Error code: 0x" + hPara[0].ToString("X4") + "| DataCode 0x" + hPara[1].ToString("X4"));
            }
        }
        #endregion

        #region //=============================================================================================DMFP_LOGCVSPROJNAME_CallBack
        public void DMFP_LOGCVSPROJNAME_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFFFF)
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_LOGCVSPROJNAME_CallBack()...has Error code: 0x" + hPara[0].ToString("X4") + "| DataCode 0x" + hPara[1].ToString("X4"));
            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Import Setup
        //#####################################################################################################

        #region //=============================================================================================btnImportSetting_Click
        private void btnImportSetting_Click(object sender, EventArgs e)
        {
            if (TabLoggerCVSSetup == null)
                return;
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();
            //---------------------------------------------------------------------------------------------------------  
            cDMFP_BulkFrame.AddItem("+LOGCVSSETUPR()", new DMFP_Delegate(DMFP_LOGCVSSETUPR_CallBack));
            cDMFP_BulkFrame.AddItem("+LOGCVSPROJNAMER()", new DMFP_Delegate(DMFP_LOGCVSPROJNAMER_CallBack));
            //---------------------------------------------------------------------------------------------------------
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 10;
            cDMFP_BulkFrame.isNoErrorReport = true;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);
        }
        #endregion

        #region //=============================================================================================DMFP_LOGCVSSETUPR_CallBack and LOGCVSPROJNAMER()
        public void DMFP_LOGCVSSETUPR_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (TabLoggerCVSSetup == null)
                return;
            //-ADISSETUPR(Errorcode; AccelFormat; GyroFormat; AxiPolicy; AccelMath; isAdvFarme, isEnabled ,isPlugged)\n
            if (hPara[0] == 0xFFFF)
            {
                TabLoggerCVSSetup.bIsEnable = Convert.ToBoolean(iPara[0]);
                TabLoggerCVSSetup.bAsyncLogEnabled = Convert.ToBoolean(iPara[1]);
                TabLoggerCVSSetup.bAsyncUDTEnabled = Convert.ToBoolean(iPara[2]);
                TabLoggerCVSSetup.isSleepEnabled = Convert.ToBoolean(iPara[3]);
                TabLoggerCVSSetup.iSelectTrigger = iPara[4];
                TabLoggerCVSSetup.iTimerPeroid = iPara[5];
                TabLoggerCVSSetup.iCounterToCease = iPara[6];
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_LOGCVSSETUPR_CallBack()...has Error code: 0x" + hPara[0].ToString("X4") + "| DataCode 0x" + hPara[1].ToString("X4"));
                return;
            }
        }
        #endregion

        #region //=============================================================================================DMFP_LOGCVSPROJNAMER_CallBack
        public void DMFP_LOGCVSPROJNAMER_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (TabLoggerCVSSetup == null)
                return;
            if (hPara[0] == 0xFFFF)
            {
                TabLoggerCVSSetup.sProjectName = sPara[0];
                LoggerCVS_Refresh_Window_Write();
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_LOGCVSPROJNAMER_CallBack()...has Error code: 0x" + hPara[0].ToString("X4") + "| DataCode 0x" + hPara[1].ToString("X4"));
                return;
            }
        }
        #endregion


        //#####################################################################################################
        //###################################################################################### Run/Stop
        //#####################################################################################################


        #region //=============================================================================================btnRun_Click
        private void btnRun_Click(object sender, EventArgs e)
        {
            if (LoggerCVS_RunMode(1,false) == false)
                return;
            btnRun.Visible = false;
            btnStop.Visible = true;
        }
        #endregion

        #region //=============================================================================================btnStop_Click
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (LoggerCVS_RunMode(0, false) == false)
                return;
            btnRun.Visible = true;
            btnStop.Visible = false;
        }
        #endregion

        #region //=============================================================================================LoggerCVS_RunMode
        private Boolean LoggerCVS_RunMode(int RunStop, bool bWaitLoop)
        {
            /*
            int loop = 0;
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();
            //---------------------------------------------------------------------------------------------------------
            //+ADISRUNSTOP (x)\n	x=0: Stop, x=1: Run
            if (RunStop == 1)
            {
                cDMFP_BulkFrame.AddItem("+ADISRUNSTOP(1)", new DMFP_Delegate(DMFP_ADISRUNSTOP_CallBack));
            }
            else
            {
                cDMFP_BulkFrame.AddItem("+ADISRUNSTOP(0)", new DMFP_Delegate(DMFP_ADISRUNSTOP_CallBack));
            }
            //---------------------------------------------------------------------------------------------------------     
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 10;
            cDMFP_BulkFrame.isNoErrorReport = true;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);

            if (bWaitLoop == true)
            {
                while (cDMFP_BulkFrame.isBusy == true)
                {
                    Tools.InteractivePause(TimeSpan.FromMilliseconds(25));
                    loop++;
                    if (loop > 40)      // Second too long to wait. 
                        break;
                }
            }
            return;
            */
            return true;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Tab Enable
        //#####################################################################################################

        #region //=============================================================================================chTabEnable_MouseClick    
        private void chTabEnable_MouseClick(object sender, MouseEventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            BtnOpenSurveyWindow.Show();
            btnOpenLoggerWindow.Show();
            TabWindowEnable.bLoggerCVS = chTabEnable.Checked;
            myGlobalBase.bTabWindowEnableUpdate = true;
            LoggerCVS_Refresh_Window_Control();
        }

        #endregion

        //#####################################################################################################
        //###################################################################################### Logger Window
        //#####################################################################################################

        #region //=============================================================================================btnSetupFolder_Click
        private void btnSetupFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                myGlobalBase.sMiniAD7794_Foldername = folderBrowserDialog1.SelectedPath;
                if (!Directory.Exists(myGlobalBase.sMiniAD7794_Foldername))
                {
                    DirectoryInfo di = Directory.CreateDirectory(myGlobalBase.sMiniAD7794_Foldername);  // Create folder if not exist. 
                }
            }
            tbFolderName.Text = myGlobalBase.sMiniAD7794_Foldername;
        }
        #endregion

        #region //=============================================================================================btnOpenFolder_Click
        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (myGlobalBase.sMiniAD7794_Foldername == null)             // No filename path, quit. 
                    return;
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = myGlobalBase.sMiniAD7794_Foldername;
                prc.Start();
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("#E: Open Folder has no folder/path or attempt to use restricted domains of Drive C");
            }

        }
        #endregion

        #region //=============================================================================================btnOpenLoggerWindow_Click
        private void btnOpenLoggerWindow_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            BtnOpenSurveyWindow.Hide();
            btnOpenLoggerWindow.Show();
            myMainProg.myRtbTermMessageLF("===================================Logger Mode\n");
            myMainProg.myRtbTermMessageLF("====== DO NOT USE MAIN TERMINAL ! ============\n");
            myMainProg.myRtbTermMessageLF("==============================================\n");
            myGlobalBase.LoggerOpertaionMode = true;
            myGlobalBase.LoggerWindowVisable = true;
            myLoggerCSV.TopMost = true;
            myLoggerCSV.Show();
            Thread.Sleep(1);
            myLoggerCSV.LoggerCVS_SetupShow_Window(1, myGlobalBase.sMiniAD7794_Foldername);
            myLoggerCSV.TopMost = false;
        }
        #endregion

        #region //=============================================================================================BtnOpenSurveyWindow_Click
        private void BtnOpenSurveyWindow_Click(object sender, EventArgs e)
        {
            BtnOpenSurveyWindow.Show();
            btnOpenLoggerWindow.Hide();
            myMainProg.myRtbTermMessageLF("===================================Logger Mode\n");
            myMainProg.myRtbTermMessageLF("====== DO NOT USE MAIN TERMINAL ! ============\n");
            myMainProg.myRtbTermMessageLF("==============================================\n");
            //myGlobalBase.LoggerOpertaionMode = true;
            //myGlobalBase.LoggerWindowVisable = true;
            //myLoggerCSV.isTurnOffSyncTab = true;
            myLoggerCSV.STC_Logger_OpenWindow(myGlobalBase.sMiniAD7794_Foldername);

        }
        #endregion
    }
}