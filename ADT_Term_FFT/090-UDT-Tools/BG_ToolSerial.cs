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
    public partial class BG_ToolSerial : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        USB_VCOM_Comm myUSBVCOMComm;
        DMFProtocol myDMFProtocol;
        BG_ReportViewer myBGReportViewer;
        DialogSupport myDialogReport;
        MainProg myMainProg;
       
        bool isActive;
        bool isEraseSerialNumber;
        bool isLoadDataDone;
        bool isEraseDone;
        int WriteIndexNo;

        DMFP_Delegate fpCallBack_ToolSerialWriteData;
        DMFP_Delegate fpCallBack_ToolSerialReadSerialNo;
        DMFP_Delegate fpCallBack_ToolSerialWriteSerialNo;

        UInt32[] ToolSerialData = new UInt32[8];        // maximum 8 element. 

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

        public BG_ToolSerial()
        {
            InitializeComponent();
            myDialogReport = new DialogSupport();
            isActive = false;
        }

        #region //================================================================BG_Orientation_Load
        private void BG_ToolSerial_Load(object sender, EventArgs e)
        {
            //lbMatched.Visible = false;
            //lbAntiClockwise.Visible = false;
            //lbClockwise.Visible = false;
            //tbActual_TP.BackColor = Color.WhiteSmoke;
            //tbTarget_TP.BackColor = Color.WhiteSmoke;
            //BG_Orientation_Minimise();
        }
        #endregion

        #region //================================================================BG_ToolSerial_FormClosing
        private void BG_ToolSerial_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            //---------------------------Send Message to cease toolphase activity in case it form is closed. 
            if (isActive == true)
            {
                DMFP_Delegate fpCallBack_ToolStop = new DMFP_Delegate(DMFP_ToolStop);
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolStop, "+TLPHSTOP(0xFF)", false, (int)eUSBDeviceType.BGDRILLING);
            }
            myGlobalBase.BG_isToCoToolSerialOpen = false;                 // Cease Survey CVS Terminal mode.
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================BG_ToolSerial_Show
        public void BG_ToolSerial_Show()
        {
            //-----------------------------Precaution Measure
            if (isActive == true)
            {
                DMFP_Delegate fpCallBack_ToolStop = new DMFP_Delegate(DMFP_ToolStop);
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolStop, "+TLPHSTOP(0xFF)", false, (int)eUSBDeviceType.BGDRILLING);
            }
            //-----------------------------------------------
            myGlobalBase.BG_isToCoToolSerialOpen = true;
            btnLoadData.PerformClick();                     // Download 
            isActive = false;
            isLoadDataDone = false;
            isEraseDone = false;
            isEraseSerialNumber = cbEraseSerialNo.Checked;
            WriteIndexNo = 0;
            this.Visible = true;
            this.TopMost = true;
            this.BringToFront();
            this.Show();
            this.Refresh();
        }
        #endregion

        #region //================================================================btnClose_Click
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        //#######################################################################################################################

        #region //================================================================DMFP_ToolStop
        public void DMFP_ToolStop(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            //###TASK, post cease operation....TBD. 
        }
        #endregion

        #region //================================================================btnLoadData_Click
        private void btnLoadData_Click(object sender, EventArgs e)
        {
            //+EETIMPORT:  iPara[0] = Index to read
            //-EETIMPORT: iPara[0] = Index, iPara[0] = Data.
//          //: Error if -EETIMPORT(0xEEEE,0xEEEE)
            int i;
            isLoadDataDone = false;
            DMFP_Delegate fpCallBack_ToolSerialLoadData = new DMFP_Delegate(DMFP_ToolSerialLoadData);
            for (i=0; i<8; i++)
            {
                System.Threading.Thread.Sleep(50);     //50mSec pause
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolSerialLoadData, "+EETIMPORT("+i+")", false, (int)eUSBDeviceType.BGDRILLING);
            }
        }
        #endregion

        #region //================================================================DMFP_ToolSerialLoadData
        public void DMFP_ToolSerialLoadData(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] == 0xEEEE)
            {
                myDialogReport = null;
                myDialogReport = new DialogSupport();
                myDialogReport.PopUpMessageBox("Load ToolSerial DataSet Report Error, Load data may be invalid", "ToolSerial Error", 2);
            }
            else
            {
                ToolSerialData[hPara[0]] = hPara[1];
            }
            if (hPara[0]==7)    // Last element.
            {
                txbSerialNo.Text = ToolSerialData[0].ToString();
                txbBoardNo.Text = "0x"+ToolSerialData[1].ToString("X");
                //---------------------------------------------
                cbSelectTS.Enabled = false;
                cbSelectTOCO.Enabled = false;
                //---------------------------------------------
                if (ToolSerialData[1]==0x700141)
                {
                    cbSelectTS.Checked = false;
                    cbSelectTOCO.Checked = true;
                }
                if (ToolSerialData[1] == 0x700149)
                {
                    cbSelectTS.Checked = true;
                    cbSelectTOCO.Checked = false;
                }
                //---------------------------------------------
                cbSelectTS.Enabled = true;
                cbSelectTOCO.Enabled = true;
                //---------------------------------------------
                txbBoardRev.Text = "0x"+ ToolSerialData[2].ToString("X");
                // Marker1 not appeared.
                txbConfig1.Text = "0x" + ToolSerialData[4].ToString("X");
                txbConfig2.Text = "0x" + ToolSerialData[5].ToString("X");
                txbConfig3.Text = "0x" + ToolSerialData[6].ToString("X");
                // Marker2 not appeared.
                isLoadDataDone = true;
            }
        }
        #endregion

        #region //================================================================btnValidate_Click
        private void btnValidate_Click(object sender, EventArgs e)
        {
            myDialogReport = null;
            myDialogReport = new DialogSupport();
            if (isLoadDataDone == false)
            { 
                myDialogReport.PopUpMessageBox("Please Load DataSet First and try again", "ToolSerial Error", 5);
                return;
            }
            if ((ToolSerialData[0]==0xFFFFFFFF)||(ToolSerialData[3]!=0x10)||(ToolSerialData[7]!=0x11))
            {
                myDialogReport.PopUpMessageBox("DataSet has failed the validation test. Must be fixed before use!", "ToolSerial Error", 5);
            }
            else
            {
                myDialogReport.PopUpMessageBox("DataSet has passed the validation test", "ToolSerial", 5);
            }

        }
        #endregion

        #region //================================================================btnWriteAll_Click
        private void btnWriteAll_Click(object sender, EventArgs e)
        {
            myDialogReport = null;
            myDialogReport = new DialogSupport();
            //------------------------------------------------------------------------Has it been erased before writing?
            if (isEraseDone==false)
            {
                myDialogReport.PopUpMessageBox("Please Erase DataSet first then try again", "ToolSerial Error", 5);
                return;
            }
            //------------------------------------------------------------------------Load TextBox Data into array
            ToolSerialData[0] = Tools.ConversionStringtoUInt32(txbSerialNo.Text);
            ToolSerialData[1] = Tools.HexStringtoUInt32X(txbBoardNo.Text);
            ToolSerialData[2] = Tools.HexStringtoUInt32X(txbBoardRev.Text);
            ToolSerialData[3] = 0x10;
            ToolSerialData[4] = Tools.HexStringtoUInt32X(txbConfig1.Text);
            ToolSerialData[5] = Tools.HexStringtoUInt32X(txbConfig2.Text);
            ToolSerialData[6] = Tools.HexStringtoUInt32X(txbConfig3.Text);
            ToolSerialData[7] = 0x11;
            //------------------------------------------------------------------------Check Serial Number first
            if (fpCallBack_ToolSerialReadSerialNo == null)
                fpCallBack_ToolSerialReadSerialNo = new DMFP_Delegate(DMFP_ToolSerialReadSerialNo);
            System.Threading.Thread.Sleep(50);     //50mSec pause
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolSerialReadSerialNo, "+EETIMPORT(0)", false, (int)eUSBDeviceType.BGDRILLING);
        }
        #endregion

        #region //================================================================DMFP_ToolSerialReadSerialNo
        public void DMFP_ToolSerialReadSerialNo(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {

            if ((hPara[0] == 0) & (hPara[1] == 0xFFFFFFFF))
            {
                if (fpCallBack_ToolSerialWriteSerialNo == null)
                    fpCallBack_ToolSerialWriteSerialNo = new DMFP_Delegate(DMFP_ToolSerialWriteSerialNo);
                WriteIndexNo = 0;       // Serial Number was blank.
                System.Threading.Thread.Sleep(50);     //50mSec pause
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolSerialWriteSerialNo, "+EETEXPORTSN(" + ToolSerialData[WriteIndexNo].ToString() + ")", false, (int)eUSBDeviceType.BGDRILLING);
            }
            else
            {
                if (fpCallBack_ToolSerialWriteData == null)
                    fpCallBack_ToolSerialWriteData = new DMFP_Delegate(DMFP_ToolSerialWriteData);
                txbSerialNo.Text = hPara[1].ToString();
                WriteIndexNo = 1;       // Serial Number is not blank and thus cannot write over. 
                System.Threading.Thread.Sleep(50);     //50mSec pause
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolSerialWriteData, "+EETEXPORT(" + WriteIndexNo.ToString() + ";0x" + ToolSerialData[WriteIndexNo].ToString("X8") + ")", false, (int)eUSBDeviceType.BGDRILLING);
            }
        }
        #endregion

        #region //================================================================DMFP_ToolSerialWriteSerialNo
        public void DMFP_ToolSerialWriteSerialNo(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            myDialogReport = null;
            myDialogReport = new DialogSupport();
            if (hPara[0]==0x02)     // Error. ToolSerial not erased!
            {
                myDialogReport.PopUpMessageBox("Serial No Not Erased, Erase all including serial no first", "ToolSerial Error", 5);
                return;
            }
            if (hPara[0]==0x01)     //Error, incorrect serial no. Cannot be Zero
            {
                myDialogReport.PopUpMessageBox("Serial No Cannot Be Zero, Try again", "ToolSerial Error", 5);
                return;
            }
            if (fpCallBack_ToolSerialWriteData == null)
                fpCallBack_ToolSerialWriteData = new DMFP_Delegate(DMFP_ToolSerialWriteData);
            txbSerialNo.Text = hPara[1].ToString();
            WriteIndexNo = 1;       // Serial Number is not blank and thus cannot write over. 
            System.Threading.Thread.Sleep(50);     //50mSec pause
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolSerialWriteData, "+EETEXPORT(" + WriteIndexNo.ToString() + ";0x" + ToolSerialData[WriteIndexNo].ToString("X8") + ")", false, (int)eUSBDeviceType.BGDRILLING);
        }
        #endregion

        #region //================================================================DMFP_ToolSerialWriteData
        public void DMFP_ToolSerialWriteData(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            myDialogReport = null;
            myDialogReport = new DialogSupport();
            if (hPara[1]== 0xEEEEEEEE)      // Index error
            {
                myDialogReport.PopUpMessageBox("Index Out of Range, Software failure", "ToolSerial Error", 5);
                return;
            }
            if (hPara[1] == 0xDDDDDDDD)     // Data not erased.
            {
                myDialogReport.PopUpMessageBox("ToolSerial DataSet Content is not erased, do that first", "ToolSerial Error", 5);
                return;
            }
            if (hPara[1] == 0xEEEEEEEF)     // Write Failure
            {
                myDialogReport.PopUpMessageBox("Firmware detected Write to Flash failure", "ToolSerial Error", 5);
                return;
            }
            WriteIndexNo++;
            if (WriteIndexNo == 3)  // Jump Marker1 for now.
                WriteIndexNo++;
            if (WriteIndexNo == 7)  // Marker2 
            {
                System.Threading.Thread.Sleep(50);     //50mSec pause
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolSerialWriteData, "+EETEXPORT(7;0x11)", false, (int)eUSBDeviceType.BGDRILLING);
                return;
            }
            if (WriteIndexNo == 8)
            {
                System.Threading.Thread.Sleep(50);     //50mSec pause
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolSerialWriteData, "+EETEXPORT(3;0x10)", false, (int)eUSBDeviceType.BGDRILLING);
                return;
            }
            if (WriteIndexNo == 9)
            {
                myDialogReport = null;
                myDialogReport = new DialogSupport();
                myDialogReport.PopUpMessageBox("ToolSerial Export/Write DataSet Done!", "ToolSerial", 2);
                isEraseDone = false;
                return;
            }
            System.Threading.Thread.Sleep(50);     //50mSec pause
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolSerialWriteData, "+EETEXPORT(" + WriteIndexNo.ToString() + ";0x" + ToolSerialData[WriteIndexNo].ToString("X8") + ")", false, (int)eUSBDeviceType.BGDRILLING);
        }
        #endregion

        #region //================================================================btnErase_Click
        private void btnErase_Click(object sender, EventArgs e)
        {
            isEraseSerialNumber = cbEraseSerialNo.Checked;
            DMFP_Delegate fpCallBack_ToolSerialEraseData = new DMFP_Delegate(DMFP_ToolSerialEraseData);
            if (isEraseSerialNumber==true)  // Erase all including Serial No.
            {
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolSerialEraseData, "+EETERASE(0x7F)", false, (int)eUSBDeviceType.BGDRILLING);
            }
            else                            // Erase all except the serial no. 
            {
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolSerialEraseData, "+EETERASE(0x6F)", false, (int)eUSBDeviceType.BGDRILLING);
            }
        }
        #endregion

        #region //================================================================DMFP_ToolSerialEraseData
        public void DMFP_ToolSerialEraseData(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            myDialogReport = new DialogSupport();
            if (hPara[0]==0xFF)
            {
                myDialogReport.PopUpMessageBox("ToolSerial Erasure Done!", "ToolSerial", 5);
                isEraseDone = true;
            }
            else
            {
                myDialogReport.PopUpMessageBox("ToolSerial Erasure Failure!", "ToolSerial Error", 5);
                isEraseDone = true;
            }
        }
        #endregion

        #region //================================================================btnDefault_Click
        private void btnDefault_Click(object sender, EventArgs e)
        {
            txbSerialNo.Text = "0";
            txbBoardNo.Text = "0x700141";
            txbBoardRev.Text = "0x040";

            ToolSerialData[0] = Tools.ConversionStringtoUInt32(txbSerialNo.Text);
            ToolSerialData[1] = Tools.HexStringtoUInt32X(txbBoardNo.Text);
            ToolSerialData[2] = Tools.HexStringtoUInt32X(txbBoardRev.Text);
        }
        #endregion

        #region //================================================================cbSelectTOCO_CheckedChanged
        private void cbSelectTOCO_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSelectTOCO.Checked==true)
            {
                cbSelectTS.Checked = false;
                txbBoardNo.Text = "0x700141";
            }
            else
            {
                cbSelectTS.Checked = true;
                txbBoardNo.Text = "0x700149";
            }
            ToolSerialData[1] = Tools.HexStringtoUInt32X(txbBoardNo.Text);

        }
        #endregion

        #region //================================================================cbSelectTS_CheckedChanged
        private void cbSelectTS_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSelectTS.Checked == true)
            {
                cbSelectTOCO.Checked = false;
                txbBoardNo.Text = "0x700149";
            }
            else
            {
                cbSelectTOCO.Checked = true;
                txbBoardNo.Text = "0x700141";
            }
            ToolSerialData[1] = Tools.HexStringtoUInt32X(txbBoardNo.Text);
        }
        #endregion
    }
}
