using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UDT_Term;

//typedef struct tagEEBattState
//{
//    union  {
//        uINT32 zWord;                                     // 32 BITS
//    struct  {
//    uINT32 ADP5063_ILIM :3;                           //(0-2) 0 = No Charge, 1 = 100mA, 2 = 500mA, 3 = 750mA, (I2C: 4 = 1500mA)
//    uINT32 ADP5063_I2C:1;                             //(3)   0 = Digital (R020 Board), 1 = I2C Control (R030 Board)
//    uINT32 ILIMReset:1;                               //(4)   0 = Self Reset ILIM=0 after charging. 1 = Do not self reset.
//    uINT32 ChargeMode:2;                              //(5-6) 0 = Charging is OFF, 1 = Window Charging, 2 = Adaptor Charging Mode.
//    uINT32 isBatteryNotConnected:1;                   //(7)   0 = Battery connected, 1 = Battery Not connected (test), check this during POR.
//    uINT32 isAdaptorPassedDisconnect:1;               //(8)   0 = Not passed disconnection, 1 = Disconnection, Ready for connection. (for Adaptor flag control)
//    uINT32 BatteryStatus:2;                           //(9-10) 0 = Normal, 1 = 75%, 2 = 50%, 3 = 25%
//    uINT32 BatteryConfig:2;                           //(11-12) 0 = Unspecified, 1 = LIFEPOD
//    uINT32 isBatteryNearCharged:1;                    //(13)  0 = No, 1 = Yes, it passed 3500mV during charging, now monitor for turn off charge mode
//    uINT32 POR_CharegeMode:2;                         //(14)  0 = Do not charge after POR, 1 = Permits POR Charging, 2 = detected VRAW/VUSB, 3 = Expired No Command Time out, proceed to charging.
//    uINT32 :8;                                        //(16-23) Dummy Bits
//    //---------------------------------------------------------------------------------------------------Error Section, 8 bit reserved.
//    uINT32 Err_InterruptCharging:1;                   //(24)  1 = Interrupted charging or low VRAW <4.5V
//    uINT32 Err_Overheating:1;                         //(25)  1 = Overheating occurred during charging.
//    uINT32 Err_TooCold:1;                             //(26)  1 = Too Cold To charge <0C
//    uINT32 Err_TooHot:1;                              //(27)  1 = Too Hot To charge >55C
//    uINT32 Err_DefectiveBattery:1;                    //(28)  1 = <2.0V, unable to recharge.
//    uINT32 Err_FailedChargeEvent:1;                   //(29)  1 = System error (ie still in survey/logger mode) prevent charging.
//    uINT32 :1;                                        //(30)
//    uINT32 :1;                                        //(31)
//                };
//           };
//        } _EEBattState ;
//extern volatile _EEBattState EEBattState;

namespace UDT_Term_FFT
{
    public partial class BG_Battery : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        USB_VCOM_Comm myUSBVCOMComm;
        DMFProtocol myDMFProtocol;
        DialogSupport mbOperationBusy;
        MainProg myMainProg;
        BG_ReportViewer myBGReportViewer;

        private int flowcontrol;

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
        public void MyBGReportViewerSetup(BG_ReportViewer myBGReportViewerRef)
        {
            myBGReportViewer = myBGReportViewerRef;
        }
        #endregion


        public BG_Battery()
        {
            InitializeComponent();
            mbOperationBusy = new DialogSupport();
            flowcontrol = 0;
        }

        #region //================================================================BG_Battery_Load
        private void BG_Battery_Load(object sender, EventArgs e)
        {

        }
        #endregion

        #region //================================================================BG_Battery_FormClosing
        private void BG_Battery_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            myGlobalBase.BG_isToCoBatteryOpen = false;                 // Cease Survey CVS Terminal mode.
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================BG_Battery_Show
        public void BG_Battery_Show()
        {
            this.BringToFront();
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            myGlobalBase.BG_isToCoBatteryOpen = true;
            this.Visible = true;
            RefreshBatteryInformation();
            gbBatteryState.BackColor = UpdateBatteryStateColor();       // Update button color for battery status. 
            this.Show();
        }
        #endregion

        #region //================================================================btnRechargeFromLaptop_Click
        private void btnRechargeFromLaptop_Click(object sender, EventArgs e)        //// +BATTSETUP(2;1;1)
        {
            gbBatteryState.BackColor = UpdateBatteryStateColor();       // Update button color for battery status. 
            DMFP_Delegate fpCallBack_ReportDownloadLoopI = new DMFP_Delegate(DMFP_RechargeFromLaptopCallback);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ReportDownloadLoopI, "+BATTSETUP(2;1;1)", false, (int)eUSBDeviceType.BGDRILLING);
        }
        #endregion

        #region //================================================================DMFP_RechargeFromLaptopCallback
        public void DMFP_RechargeFromLaptopCallback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFF)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: +BATTSETUP() call back has error :0x" + hPara[0].ToString("X"), "ToCo Battery Charging Error", 30, 12F);
                return;
            }

            RefreshBatteryInformation();
            mbOperationBusy.PopUpMessageBox("Battery is now charging. Do not disconnects, until advised", "ToCo Battery Charging", 3, 12F);

        }
        #endregion

        #region //================================================================btnUpdateVBAT_Click
        private void btnUpdateVBAT_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.is_BGDRILLING_Device_Activated == true)        
            {
                //bool backup = myBGReportViewer.isEnableShowWindow;
                flowcontrol = 0;                                        // This deactivate report download and update. 
                //myBGReportViewer.isEnableShowWindow = false;            // 
                RefreshBatteryInformation();
                gbBatteryState.BackColor = UpdateBatteryStateColor();       // Update button color for battery status. 
                //myBGReportViewer.isEnableShowWindow = backup;
            }
            else
            {
                mbOperationBusy.PopUpMessageBox("ERROR: ToCo Tool is not connected", "ToCo Battery Update", 30, 12F);
                return; 
            }
        }
        #endregion

        #region //================================================================btnRechargeFromAdaptor_Click
        private void btnRechargeFromAdaptor_Click(object sender, EventArgs e)
        {
            // Step 1: // +BATTSETUP(3;0;2) 750mA       (or +BATTSETUP(2;0;2)  500mA)
            // Step 2: // Open PDF to explain how to connect to adaptor to start charging. Tip how to avoid adnroid charing the ToCo tool.
            // Step 3: // Close Battery Window Application. 
            gbBatteryState.BackColor = UpdateBatteryStateColor();       // Update button color for battery status. 
            DMFP_Delegate fpCallBack_ReportDownloadLoopI = new DMFP_Delegate(DMFP_RechargeFromAdaptorCallback);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ReportDownloadLoopI, "+BATTSETUP(3;0;2)", false, (int)eUSBDeviceType.BGDRILLING);
        }
        #endregion

        #region //================================================================DMFP_RechargeFromAdaptorCallback
        public void DMFP_RechargeFromAdaptorCallback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFF)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: +BATTSETUP() call back has error :0x" + hPara[0].ToString("X"), "ToCo Battery Charging Error", 30, 12F);
                return;
            }
            mbOperationBusy.PopUpMessageBox("ToCo Tools is now Adapter Battery Charging Mode\n (2) Disconnect USB cable from Laptop\n (3) Connect to 1Amp USB Adapter\n (4) Check LED for Green/Red flashing\n", "ToCo Adapter Battery Charging ", 60, 12F);
        }
        #endregion

        #region //================================================================btnBatHelp_Click
        private void btnBatHelp_Click(object sender, EventArgs e)
        {
            mbOperationBusy.PopUpMessageBox("INFO: Still in Development (WIP), PDF document being generated", "ToCo Battery", 3, 12F);
        }
        #endregion

        #region //================================================================btnClose_Click
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region //================================================================btnStopCharging_Click
        private void btnStopCharging_Click(object sender, EventArgs e)
        {
            gbBatteryState.BackColor = UpdateBatteryStateColor();       // Update button color for battery status. 
            DMFP_Delegate fpCallBack_ReportDownloadLoopI = new DMFP_Delegate(DMFP_StopChargingCallback);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ReportDownloadLoopI, "+BATTSETUP(0;0;0)", false, (int)eUSBDeviceType.BGDRILLING);

        }
        #endregion

        #region //================================================================DMFP_StopChargingCallback
        public void DMFP_StopChargingCallback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFF)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: +BATTSETUP() call back has error :0x" + hPara[0].ToString("X"), "ToCo Battery Charging Error", 30, 12F);
                return;
            }

            RefreshBatteryInformation();
            mbOperationBusy.PopUpMessageBox("Battery Now Ceased Charging.", "ToCo Battery Charging ", 3, 12F);

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Update Target Section
        //#####################################################################################################

        #region //================================================================btnRechargeFromAdaptor_Click
        private void RefreshBatteryInformation()
        {
            //--------------------------------------------------------------Step 3: Start ASYNC reception.
            DMFP_Delegate fpCallBack_ToolFaceLoop = new DMFP_Delegate(DMFP_ADC12Update);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolFaceLoop,
                "+ADC12()",
                false, (int)eUSBDeviceType.BGDRILLING);
        }
        #endregion

        #region //================================================================DMFP_ADC12Update

        //---------------------------------------------------------------------------------------MK2 ToCo Rev 3D
        // Within the ToCo Code Version 3E: OBSLETE STRUCTURE DO NOT USE!!!!!
        //TOCO_ADC12_ConvertNow();
        //zprintf("-ADC12(0xFF");                           0 = 0xFF
        //zprintf(";0x%X", ADC12Result.aPIN_ADC0_VBAT);     1 = VBAT
        //zprintf(";0x%X", ADC12Result.aPIN_ADC1_VUSB);     2 = VUSB
        //zprintf(";0x%X", ADC12Result.aPIN_ADC2_VDDR);     3 = VDDR
        //zprintf(";0x%X", EESysFlag.zWord);                4 = EESysFlag
        //zprintf(";0x%X", EEBattState.zWord);              5 = EEBattState
        //zprintf(";0x%X)\n", ADC12Result.aInternal_VDDS);  6 = aInternal_VDDS
        //---------------------------------------------------------------------------------------MK2 ToCo / TS Rev 5C onward.
        // New structure for TS and TOCO from Rev 5C. 
        //zprintf("-ADC12(0xFF");
        //zprintf(";0x%X", ADC12Result.aPIN_ADC0_VBAT);
        //zprintf(";0x%X", ADC12Result.aPIN_ADC1_VUSB);
        //zprintf(";0x%X", ADC12Result.aPIN_ADC2_VDDR);
        //zprintf(";0x%X", ADC12Result.aPIN_ADC2_V5VPSU);   // Added ADC12 channel.
        //zprintf(";0x%4X", EESysFlag.zWord);
        //zprintf(";0x%4X", EEBattState.zWord);
        //zprintf(";0x%X)\n", ADC12Result.aInternal_VDDS);

        //---------------------------------------------------------------------------------------MK2 ToCo Rev 3D also unchanged for 4X and 5C (TOCO/TS)
        //        typedef struct tagEEBattState
        //        {
        //            union  {
        //        uINT32 zWord;                                     // 32 BITS
        //            struct  {
        //          uINT32 ADP5063_ILIM :3;                           //(0-2) 0 = No Charge, 1 = 100mA, 2 = 500mA, 3 = 750mA, (I2C: 4 = 1500mA)
        //            uINT32 ADP5063_I2C:1;                             //(3)   0 = Digital (R020 Board), 1 = I2C Control (R030 Board)
        //            uINT32 ILIMReset:1;                               //(4)   0 = Self Reset ILIM=0 after charging. 1 = Do not self reset.
        //            uINT32 ChargeMode:2;                              //(5-6) 0 = Charging is OFF, 1 = Window Charging, 2 = Adaptor Charging Mode.
        //            uINT32 isBatteryNotConnected:1;                   //(7)   0 = Battery connected, 1 = Battery Not connected (test), check this during POR.
        //            uINT32 isAdaptorPassedDisconnect:1;               //(8)   0 = Not passed disconnection, 1 = Disconnection, Ready for connection. (for Adaptor flag control)
        //            uINT32 BatteryStatus:2;                           //(9-10) 0 = Normal, 1 = 75%, 2 = 50%, 3 = 25%
        //            uINT32 BatteryConfig:2;                           //(11-12) 0 = Unspecified, 1 = LIFEPOD
        //            uINT32 isBatteryNearCharged:1;                    //(13)  0 = No, 1 = Yes, it passed 3500mV during charging, now monitor for turn off charge mode
        //            uINT32 POR_CharegeMode:2;                         //(14-15)  0 = Do not charge after POR, 1 = Permits POR Charging, 2 = detected VRAW/VUSB, 3 = Expired No Command Time out, proceed to charging.
        //            uINT32 isConnected:1;                             //(16)
        //            uINT32 :7;                                        //(17-23) Dummy Bits
        //          //---------------------------------------------------------------------------------------------------Error Section, 8 bit reserved.
        //          uINT32 Err_InterruptCharging:1;                   //(24)  1 = Interrupted charging or low VRAW <4.5V
        //            uINT32 Err_Overheating:1;                         //(25)  1 = Overheating occurred during charging.
        //            uINT32 Err_TooCold:1;                             //(26)  1 = Too Cold To charge <0C
        //            uINT32 Err_TooHot:1;                              //(27)  1 = Too Hot To charge >55C
        //            uINT32 Err_DefectiveBattery:1;                    //(28)  1 = <2.0V, unable to recharge.
        //            uINT32 Err_FailedChargeEvent:1;                   //(29)  1 = System error (ie still in survey/logger mode) prevent charging.
        //            uINT32 :1;                                        //(30)
        //          uINT32 :1;                                        //(31)
        //                };
        //    };
        //}
        //_EEBattState ;
        //extern volatile _EEBattState EEBattState;

        public void DMFP_ADC12Update(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFF)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: +ADC12() call back has error :0x"+ hPara[0].ToString("X"), "ToCo ADC12 Readout Error", 30, 12F);
                return;
            }
            //-----------------------------------5X onward.
            if (hPara.Count == 8)
            {
                lVBAT.Text = "VBAT =" + hPara[1].ToString("D") + "mV";

                myMainProg.myRtbTermMessageLF("-I: VBAT        =  " + hPara[1].ToString("D") + "mV");
                myMainProg.myRtbTermMessageLF("-I: VUSB        =  " + hPara[2].ToString("D") + "mV");
                myMainProg.myRtbTermMessageLF("-I: VDDR        =  " + hPara[3].ToString("D") + "mV");
                myMainProg.myRtbTermMessageLF("-I: V5V0        =  " + hPara[4].ToString("D") + "mV");
                myMainProg.myRtbTermMessageLF("-I: VMCU        =  " + hPara[7].ToString("D") + "mV");
                myMainProg.myRtbTermMessageLF("-I: EESysFlag   =0x" + hPara[5].ToString("X"));
                myMainProg.myRtbTermMessageLF("-I: EEBattState =0x" + hPara[6].ToString("X"));

                myBGReportViewer.ReportClass.EESysFlag = hPara[5];      // Update report class.
                myBGReportViewer.ReportClass.EEBattState = hPara[6];     // Update report class.
                                                                         //-------------------------------------------------------------Error Bits Section
                if (myBGReportViewer.ReportClass.EESysFlag == 0)
                {
                    rbErr1.Visible = false;
                    rbErr2.Visible = false;
                    rbErr3.Visible = false;
                    rbErr4.Visible = false;
                    rbErr5.Visible = false;
                    rbErr6.Visible = false;
                }
                else
                {
                    rbErr1.Visible = true;
                    rbErr2.Visible = true;
                    rbErr3.Visible = true;
                    rbErr4.Visible = true;
                    rbErr5.Visible = true;
                    rbErr6.Visible = true;
                    rbErr1.Checked = Tools.Bits_UInt32_Read(myBGReportViewer.ReportClass.EEBattState, 24);
                    rbErr2.Checked = Tools.Bits_UInt32_Read(myBGReportViewer.ReportClass.EEBattState, 25);
                    rbErr3.Checked = Tools.Bits_UInt32_Read(myBGReportViewer.ReportClass.EEBattState, 26);
                    rbErr4.Checked = Tools.Bits_UInt32_Read(myBGReportViewer.ReportClass.EEBattState, 27);
                    rbErr5.Checked = Tools.Bits_UInt32_Read(myBGReportViewer.ReportClass.EEBattState, 28);
                    rbErr6.Checked = Tools.Bits_UInt32_Read(myBGReportViewer.ReportClass.EEBattState, 29);
                }
                //-------------------------------------------------------------
                gbBatteryState.BackColor = UpdateBatteryStateColor();
                //-------------------------------------------------------------
                BatteryChargingModeState();
                //-------------------------------------------------------------
            }
            else if (hPara.Count==7)         // For 3X to 4X ony
            {
                lVBAT.Text = "VBAT =" + hPara[1].ToString("D") + "mV";
                myMainProg.myRtbTermMessageLF("-I: VBAT        =  " + hPara[1].ToString("D") + "mV");
                myMainProg.myRtbTermMessageLF("-I: VUSB        =  " + hPara[2].ToString("D") + "mV");
                myMainProg.myRtbTermMessageLF("-I: VDDR        =  " + hPara[3].ToString("D") + "mV");
                myMainProg.myRtbTermMessageLF("-I: VMCU        =  " + hPara[6].ToString("D") + "mV");
                myMainProg.myRtbTermMessageLF("-I: EESysFlag   =0x" + hPara[4].ToString("X"));
                myMainProg.myRtbTermMessageLF("-I: EEBattState =0x" + hPara[5].ToString("X"));

                myBGReportViewer.ReportClass.EESysFlag = hPara[4];      // Update report class.
                myBGReportViewer.ReportClass.EEBattState = hPara[5];     // Update report class.
                                                                         //-------------------------------------------------------------Error Bits Section
                if (myBGReportViewer.ReportClass.EESysFlag == 0)
                {
                    rbErr1.Visible = false;
                    rbErr2.Visible = false;
                    rbErr3.Visible = false;
                    rbErr4.Visible = false;
                    rbErr5.Visible = false;
                    rbErr6.Visible = false;
                }
                else
                {
                    rbErr1.Visible = true;
                    rbErr2.Visible = true;
                    rbErr3.Visible = true;
                    rbErr4.Visible = true;
                    rbErr5.Visible = true;
                    rbErr6.Visible = true;
                    rbErr1.Checked = Tools.Bits_UInt32_Read(myBGReportViewer.ReportClass.EEBattState, 24);
                    rbErr2.Checked = Tools.Bits_UInt32_Read(myBGReportViewer.ReportClass.EEBattState, 25);
                    rbErr3.Checked = Tools.Bits_UInt32_Read(myBGReportViewer.ReportClass.EEBattState, 26);
                    rbErr4.Checked = Tools.Bits_UInt32_Read(myBGReportViewer.ReportClass.EEBattState, 27);
                    rbErr5.Checked = Tools.Bits_UInt32_Read(myBGReportViewer.ReportClass.EEBattState, 28);
                    rbErr6.Checked = Tools.Bits_UInt32_Read(myBGReportViewer.ReportClass.EEBattState, 29);
                }
                //-------------------------------------------------------------

                gbBatteryState.BackColor = UpdateBatteryStateColor();
                //-------------------------------------------------------------
                BatteryChargingModeState();
                //-------------------------------------------------------------
            }
            else
            {
                rbErr1.Visible = false;
                rbErr2.Visible = false;
                rbErr3.Visible = false;
                rbErr4.Visible = false;
                rbErr5.Visible = false;
                rbErr6.Visible = false;
            }
            //if (flowcontrol == 1)
            //{
            //    flowcontrol = 0;
            //    if (myBGReportViewer.Report_Download_WithAttempts()==false)     // 76J: New design with repeat report frame loop, in case TOCO is busy.
            //    {
            //        mbOperationBusy.PopUpMessageBox("ERROR: Report Frame Upload failure or Rental Data Expired/Error", "Report Frame Error", 30, 12F);
            //    }
            //}
        }
        #endregion

        #region //================================================================UpdateBatteryStateColor
        //#define LIFEPOD_5PC     2750    //Threshold <2750mV, cease survey mode, enter to sleep mode.
        //#define LIFEPOD_25PC    3200    //Threshold <3200mV                           0
        //#define LIFEPOD_50PC    3300    //Threshold <3300mV                           1
        //#define LIFEPOD_75PC    3400    //Threshold <3400mV                           2
        //#define LIFEPOD_100PC   3500    //>3500mV fresh battery after charge up       3
        public Color UpdateBatteryStateColor()
        {
            int batstate = (int)((myBGReportViewer.ReportClass.EEBattState & (0x00000600)) >> 9);   // Bit 9 and 10
            //int DefBattery = (int)((myBGReportViewer.ReportClass.EEBattState & (0x10000000)) >> 28);   // Bit 28
            bool DefBattery = Tools.Bits_UInt32_Read(myBGReportViewer.ReportClass.EEBattState, 28);
            Color statecolor = Color.Green;

            if (DefBattery == true)
            {
                lBatState.Text = "BAT is Dead/Missing";
                statecolor = Color.Red;
            }
            else
            {
                switch (batstate)       //0 = Normal, 1 = 75 %, 2 = 50 %, 3 = 25 %
                {
                    case (0):
                        {
                            lBatState.Text = "Battery State: >75%";
                            statecolor = Color.Green;
                            break;
                        }
                    case (1):
                        {
                            lBatState.Text = "Battery State: <75%";
                            statecolor = Color.YellowGreen;
                            break;
                        }
                    case (2):
                        {
                            lBatState.Text = "Battery State: <50%";
                            statecolor = Color.Orange;
                            break;
                        }
                    default:
                        {
                            lBatState.Text = "Battery State: Low (<25%)";
                            statecolor = Color.OrangeRed;
                            break;
                        }
                }
            }
            gbBatteryState.BackColor = statecolor;
            return (statecolor);
        }
        #endregion

        #region //================================================================BatteryChargingModeState
        public int BatteryChargingModeState()
        {
            //EEBattState.ChargeMode //(5-6) 0 = Charging is OFF, 1 = Window Charging, 2 = Adapter Charging Mode.
            int batstate = (int)((myBGReportViewer.ReportClass.EEBattState & (0x0000060)) >> 5);   // Bit 5-6, Bug Fixd 22/3/20. 
            switch (batstate)       //0 = Charging is OFF, 1 = Window Charging, 2 = Adapter Charging Mode.
            {
                case (0):
                    {
                        lBatCharging.Text = "STANDBY";
                        break;
                    }
                case (1):
                    {
                        lBatCharging.Text = "CHARGING (Window)";
                        break;
                    }
                case (2):
                    {
                        lBatCharging.Text = "CHARGING (Adapter)";
                        break;
                    }
                default:
                    {
                        lBatCharging.Text = "STANDBY";
                        break;
                    }
            }
            return batstate;
        }
        #endregion


    }
}
