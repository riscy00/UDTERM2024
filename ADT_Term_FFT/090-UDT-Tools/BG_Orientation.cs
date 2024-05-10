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

namespace UDT_Term_FFT
{
    public partial class BG_Orientation : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        USB_VCOM_Comm myUSBVCOMComm;
        DMFProtocol myDMFProtocol;
        BG_ReportViewer myBGReportViewer;
        DialogSupport mbOperationBusy;
        MainProg myMainProg;

        BGAccelMath_MRC myTargetMath;
        BGAccelMath_MRC myActualMath;

        BGMasterCalData myBGMasterCalData;
        int SelectMethod;
        public string MathDebug = "";
        bool isActive;
        bool isFinishJobState;

        #region//================================================================Reference
        public void MyBGReportViewerSetup(BG_ReportViewer myBGReportViewerRef)
        {
            myBGReportViewer = myBGReportViewerRef;
        }
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }

        public void MyUSBVCOMComm(USB_VCOM_Comm myUSBVCOMCommRef)
        {
            myUSBVCOMComm = myUSBVCOMCommRef;
        }
        public void MyDMFProtocol(DMFProtocol myDMFProtocolRef)
        {
            myDMFProtocol = myDMFProtocolRef;
        }
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        #endregion

        public BG_Orientation()
        {
            InitializeComponent();
            myActualMath = new BGAccelMath_MRC();
            mbOperationBusy = new DialogSupport();
            isActive = false;
            isFinishJobState = false;
        }

        #region //================================================================BG_Orientation_Load
        private void BG_Orientation_Load(object sender, EventArgs e)
        {
            lbMatched.Visible = false;
            lbAntiClockwise.Visible = false;
            lbClockwise.Visible = false;
            tbActual_TP.BackColor = Color.WhiteSmoke;
            tbTarget_TP.BackColor = Color.WhiteSmoke;
            BG_Orientation_Button_NextTask_Disable();
        }
        #endregion

        #region //================================================================BG_Orientation_FormClosing
        private void BG_Orientation_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            //---------------------------Send Message to cease toolphase activity in case it form is closed. 
            if (isActive == true)
            {
                DMFP_Delegate fpCallBack_ToolStop = new DMFP_Delegate(DMFP_ToolStop);
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolStop, "+TLPHSTOP(0xFF)", false, (int)eUSBDeviceType.BGDRILLING);
            }
            myGlobalBase.BG_isToCoOrientationOpen = false;                 // Cease Survey CVS Terminal mode.
            BG_Orientation_Button_NextTask_Disable();
            if (isFinishJobState == true)
            {
                myMainProg.BGSurveyButtonSetting(11);
                myMainProg.BGSurveyButtonSetting(12);
                isFinishJobState = false;
            }
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================BG_Orientation_Show
        public void BG_Orientation_Show()
        {
            //-----------------------------Precaution Measure
            if (isActive == true)
            {
                DMFP_Delegate fpCallBack_ToolStop = new DMFP_Delegate(DMFP_ToolStop);
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolStop, "+TLPHSTOP(0xFF)", false, (int)eUSBDeviceType.BGDRILLING);
            }
            //-----------------------------------------------
            myGlobalBase.BG_isToCoOrientationOpen = true;
            isActive = false;
            lbMatched.Visible = false;
            lbAntiClockwise.Visible = false;
            lbClockwise.Visible = false;
            BG_Orientation_Button_NextTask_Disable();
            this.Visible = true;
            this.TopMost = true;
            this.BringToFront();
            this.Show();
            this.Refresh();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Update Target Section
        //#####################################################################################################

        #region //================================================================BGOrientation_Start_Orientation
        public void BGOrientation_Start_Orientation(BGMasterCalData myBGMasterCalDataRef, int SelectMethodRef)
        {
            myBGMasterCalData = myBGMasterCalDataRef;
            SelectMethod = SelectMethodRef;
            MathDebug = "";
            //---------------------------------------------------------------Step 0: Update the Tool Status
            UInt32 Battery = myBGReportViewer.ReportClass.EEBattState;      // Based on EEBattState
            UInt32 SysFlag = myBGReportViewer.ReportClass.EESysFlag;        // Based on zLoggingToolStatus
            //if (Tools.Bits_UInt32_Read(SysFlag, 11) == true)
            //{
            //    txToolStatus.Text += "Logger Memory was Full";
            //    txToolStatus.BackColor = Color.DarkOrange;
            //}
            //else 

            int batstate = (int)((Battery & (0x00000600)) >> 9);   // Bit 9 and 10
            switch (batstate)   // 0 = Normal, 1 = 75%, 2 = 50%, 3 = 25%
            {
                case (0):
                    {
                        txToolStatus.Text = "Battery >75% Capacity";
                        txToolStatus.BackColor = Color.Green;
                        break;
                    }
                case (1):   // 1 = 75%,
                    {
                        txToolStatus.Text = "Battery <75% Capacity";
                        txToolStatus.BackColor = Color.YellowGreen;
                        break;
                    }
                case (2):   // 2 = 50%,
                    {
                        txToolStatus.Text = "Battery <50% Capacity";
                        txToolStatus.BackColor = Color.Orange;
                        break;
                    }
                case (3):   // 3 = 25%
                    {
                        txToolStatus.Text = "Battery <25% Need Charging";
                        txToolStatus.BackColor = Color.OrangeRed;
                        break;
                    }
                default:
                    break;
            }
            myMainProg.zBG_Update_Battery_Button();
            //---------------------------------------------------------------Step 1: Process Math on Accel Data
            if (myTargetMath == null)
            {
                myTargetMath = null;
                myTargetMath = new BGAccelMath_MRC();
            }
            iVector3 iAccel = new iVector3();

            iAccel.X = myBGReportViewer.ReportClass.CaptureDataFrame[2].MRC_Gx;
            iAccel.Y = myBGReportViewer.ReportClass.CaptureDataFrame[2].MRC_Gy;
            iAccel.Z = myBGReportViewer.ReportClass.CaptureDataFrame[2].MRC_Gz;

            myTargetMath.AccelConvertINT32toDouble(iAccel);
            switch (SelectMethod)
            {
                case (0):   // Riscy. 
                    {
                        //myTargetMath.AccelProcessMathA();
                        myTargetMath.AccelProcessMathRiscy(myBGMasterCalData, out MathDebug);
                        MathVersion.Text = "RP1A";
                        break;
                    }
                case (3):   // New SL Method. Nov 18
                    {
                        myTargetMath.AccelProcessMath_SL_Method_3A(myBGMasterCalData, out MathDebug);
                        MathVersion.Text = "SL3A";
                        break;
                    }
                default:
                    MathVersion.Text = "ERROR";
                    break;
            }
            //---------------------------------------------------------------Step 2: Update Target Readout

            tbTarget_TP.Text = myTargetMath.dAToolFace.ToString("N1");
            tbTarget_Inc.Text = myTargetMath.dAInclination.ToString("N1");
            tbAccelMag.Text = myTargetMath.dAMagnitide.ToString("N3");

            tbTarget_Temp.Text = (Convert.ToDouble(myBGReportViewer.ReportClass.CaptureDataFrame[2].Temp) / 10).ToString("N1");
            isActive = true;
            //--------------------------------------------------------------Step 3: Start ASYNC reception.
            DMFP_Delegate fpCallBack_ToolFaceLoop = new DMFP_Delegate(DMFP_ToolFaceLoop);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolFaceLoop, "+TLPHSTART(0xFF)", false, (int)eUSBDeviceType.BGDRILLING);
        }

        #endregion

        #region //================================================================DMFP_ToolFaceData
        public void DMFP_ToolFaceLoop(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0]!=0xFF)
            {
                mbOperationBusy.PopUpMessageBox("ERROR: Orientation Cancelled due to ToCo Error", "ToCO Orientation Error", 30, 12F);
                return;
            }

            // In case of no Error, ASYNC (~TOCOTP(X,Y,Z,Temp)) take over until FINISH button is issued using +TLPHSTOP(0xFF)
        }
        #endregion

        #region //================================================================Async_BGOrientation_RecievedData
        public void Async_BGOrientation_RecievedData(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage, IList<string> Asynclistparameter)
        {
            iVector3 iAccel = new iVector3();
            if ((hPara.Count == 4) | (hPara.Count == 8))                      // Filter out corrupted data, updated for 4AA (dual Accel or single Accel)
            {
                MathDebug = "";
                iAccel.X = (Int16)(Convert.ToInt32(hPara[0]));
                iAccel.Y = (Int16)(Convert.ToInt32(hPara[1]));
                iAccel.Z = (Int16)(Convert.ToInt32(hPara[2]));

                myActualMath.AccelConvertINT32toDouble(iAccel);
                switch (SelectMethod)
                {
                    case (0):
                        {
                            //myActualMath.AccelProcessMathA();
                            myActualMath.AccelProcessMathRiscy(myBGMasterCalData, out MathDebug);
                            MathVersion.Text = "RP1A";
                        }
                        break;
                    case (3):   // New SL Method. Jan 19
                        {
                            myActualMath.AccelProcessMath_SL_Method_3A(myBGMasterCalData, out MathDebug);
                            MathVersion.Text = "SL3A";
                            break;
                        }
                    default:
                        MathVersion.Text = "ERROR";
                        break;
                }
                //---------------------------------------------------------------Step 2: Update Target Readout
                double diff = myActualMath.dAToolFace - myTargetMath.dAToolFace;
                //tbActual_TP.Text = myActualMath.dAToolFace.ToString("N1");
                tbActual_TP.Text = diff.ToString("N1");
                if ((diff > -3.0) & (diff < 3.0))
                {
                    lbMatched.Visible = true;
                    lbAntiClockwise.Visible = false;            //Color.Transparent
                    lbClockwise.Visible = false;
                    pAntiClockWise.BackColor = Color.GreenYellow;
                    pClockWise.BackColor = Color.GreenYellow;
                    //----------------------------------------------------
                    tbActual_TP.Text = (diff).ToString("N1");  //diff.ToString("N1");
                    tbActual_TP.BackColor = Color.PaleGreen;
                    tbTarget_TP.BackColor = Color.PaleGreen;
                }
                else if (myActualMath.dAToolFace > myTargetMath.dAToolFace)
                {
                    lbMatched.Visible = false;
                    lbAntiClockwise.Visible = true;
                    lbClockwise.Visible = false;
                    pAntiClockWise.BackColor = Color.GreenYellow;
                    pClockWise.BackColor = Color.Transparent;
                    //----------------------------------------------------
                    tbActual_TP.Text = (diff).ToString("N1");  //diff.ToString("N1");
                    tbActual_TP.BackColor = Color.WhiteSmoke;
                    tbTarget_TP.Text = myTargetMath.dAToolFace.ToString("N1");
                    tbTarget_TP.BackColor = Color.WhiteSmoke;
                }
                else
                {
                    lbMatched.Visible = false;
                    lbAntiClockwise.Visible = false;
                    lbClockwise.Visible = true;
                    pAntiClockWise.BackColor = Color.Transparent;
                    pClockWise.BackColor = Color.GreenYellow;
                    //----------------------------------------------------
                    tbActual_TP.Text = (diff).ToString("N1");  //diff.ToString("N1");
                    tbActual_TP.BackColor = Color.WhiteSmoke;
                    tbTarget_TP.Text = myTargetMath.dAToolFace.ToString("N1");
                    tbTarget_TP.BackColor = Color.WhiteSmoke;
                }
            }
        }
        #endregion

        #region //================================================================btnConfimCoreOrientation_Click
        private void btnConfimCoreOrientation_Click(object sender, EventArgs e)
        {
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ToolStop = new DMFP_Delegate(DMFP_ToolStop);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolStop, "+TLPHSTOP(0xFF)", false, (int)eUSBDeviceType.BGDRILLING);
        }
        #endregion

        #region //================================================================DMFP_ToolStop
        public void DMFP_ToolStop(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            BG_Orientation_Button_NextTask_Enable();

            myMainProg.BGSurveyButtonSetting(11);
            myMainProg.BGSurveyButtonSetting(12);
        }
        #endregion

        #region //================================================================BG_Orientation_Maximum
        private void BG_Orientation_Button_NextTask_Enable()    
        {

            txToolStatus.Visible = true;
            btnStartNextRun.Enabled = true;
            btnRechargeBattery.Enabled = true;
            btnFinishjob.Enabled = true;
            this.Refresh();
        }
        #endregion

        #region //================================================================BG_Orientation_Minimise
        private void BG_Orientation_Button_NextTask_Disable()
        {
            txToolStatus.Visible = false;
            btnStartNextRun.Enabled = false;
            btnRechargeBattery.Enabled = false;
            btnFinishjob.Enabled = false;
            this.Refresh();
        }
        #endregion

        #region //================================================================btnStartNextRun_Click
        private void btnStartNextRun_Click(object sender, EventArgs e)
        {
            myGlobalBase.BG_isToCoOrientationOpen = false;
            myMainProg.BGToCo_ToolA_Start_Survey();     // Restart Tools operation. 
            isActive = false;
            this.Close();
        }
        #endregion

        #region //================================================================btnRechargeBattery_Click
        private void btnRechargeBattery_Click(object sender, EventArgs e)
        {
            myGlobalBase.BG_isToCoOrientationOpen = false;
            myMainProg.BGToCo_ToolA_Start_Battery();    // Start Battery Window
            isActive = false;
            this.Close();
        }
        #endregion

        #region //================================================================btnFinishjob_Click
        private void btnFinishjob_Click(object sender, EventArgs e)
        {
            myGlobalBase.BG_isToCoOrientationOpen = false;
            myMainProg.BGToCo_ToolA_FinishJob();        // Close Connection.
            isActive = false;
            isFinishJobState = true;
            this.Close();
        }
        #endregion
    }
}
