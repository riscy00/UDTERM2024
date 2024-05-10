using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Threading;
using JR.Utils.GUI.Forms;
using UDT_Term;

namespace UDT_Term_FFT
{
    public partial class EEpromTools : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        MainProg myMainProg;
        DMFProtocol myDMFProtocol;
        System.Windows.Forms.RichTextBox myRtbTerm;
        wfSectorErasePara mywfSectorErasePara;

        DialogSupport mbOperationBusy;

        private string m_sEntryTxt;
        Timer EEtimer;
        int EECounter;

        #region//================================================================Reference
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
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
        #endregion
        public void MyRtbTerm(System.Windows.Forms.RichTextBox myrtbTermref)
        {
            myRtbTerm = myrtbTermref;
        }
        public EEpromTools()
        {
            InitializeComponent();
        }

        #region //================================================================EEpromTools_Load
        private void EEpromTools_Load(object sender, EventArgs e)
        {
            EEPROM_Setup_Init();
        }
        #endregion

        #region //================================================================EEPROMTools_Show
        public void EEPROMTools_Show()
        {
            this.BringToFront();
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            myGlobalBase.EEPROM_IsFormOpen = true;
            this.Visible = true;
            this.Show();
        }
        #endregion

        #region //================================================================EEpromTools_FormClosing
        private void EEpromTools_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            myGlobalBase.EEPROM_IsFormOpen = false;                 // Cease Survey CVS Terminal mode.
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //==================================================EEPROM_Setup_Init
        public void EEPROM_Setup_Init()
        {

            myGlobalBase.EE_NoOfDevice = 1;                 // 1 device fitted. 
            myGlobalBase.EE_Select_DeviceIC = 1;            // 1 = Select 1st device for erase purpose. 
            //------------------------------------------------Default informatics : 1 x M25S16, this get updated after download log session via $MD frame. 
            myGlobalBase.EE_StartAdd = 0x00000100;       // Page 0 reserved. 
            myGlobalBase.EE_EndAdd = 0x00000100;
            myGlobalBase.EE_MaxAddress = 0x001FFFFF;       // 2MB
            myGlobalBase.EE_NoOfPages = 8192;
            myGlobalBase.EE_PageSize = 256;
            //-----------------------------------------------

        }
        #endregion

        #region //================================================================btnBulkErase_Click 
        public void btnBulkErase_Click(object sender, EventArgs e)
        {
            mbOperationBusy = null;
            mbOperationBusy = new DialogSupport();
            if (tbEEPROMPassword.Text != "EEPROM")
            {
                mbOperationBusy.PopUpMessageBox("Type 'EEPROM' within text box below to enable Bulk Erase Procedure", "Log Data Tool Operation", 2);
                return;
            }
            
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_EEPROM_EraseLogData = new DMFP_Delegate(DMFP_EEPROM_EraseLogData_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "";
            if (cbLogMemOnly.Checked == false)
                sMessage = "+EEERASELOGDATA(0x1)";      // Complete erase including client data and rental data.
            else
                sMessage = "+EEERASELOGDATA(0x5)";      // LogMem only, fast erase.
            if (myGlobalBase.bNoRFTermMessage == false)
                myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
            Thread t1;
            if (myGlobalBase.is_Serial_Server_Connected == true)
                 t1 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_EraseLogData, sMessage, 250));
            else
                 t1 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_EEPROM_EraseLogData, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t1.Start();
            myDMFProtocol.DMFProtocol_UpdateList();

            mbOperationBusy.PopUpMessageBox("Please Wait, Erasing and verifying LogData Memory Bank", "Log Data Tool Operation", 7);
            tbEEPROMPassword.Text = "Busy/Wait";
            //-------------------------------------------------Time out event. 
            Tools.InteractivePause(TimeSpan.FromMilliseconds(5000));
            if (tbEEPROMPassword.Text == "Busy/Wait")       
            {
                mbOperationBusy.Close();
                tbEEPROMPassword.Text = "";
            }
        }
        #endregion


        #region //================================================================DMFP_EEPROM_EraseLogData_CallBack
        public void DMFP_EEPROM_EraseLogData_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            tbEEPROMPassword.Text = "";
            mbOperationBusy.Close();
            if (hPara[0] == 0xFF)
            {
                mbOperationBusy.PopUpMessageBox("Logger Data Memory Erased Done!", "Log Data Tool Operation", 2);
            }
            else
            {
                mbOperationBusy.PopUpMessageBox("Error!: Logger Data Memory Erase Fails!", "Log Data Tool Operation", 2);
            }
            
        }
        #endregion

        #region //================================================================btnBulkErase_Click 
        public void zBG_Async_EE_ERASEBULK(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage, IList<string> Asynclistparameter)
        {
            if (mbOperationBusy != null)
            {
                mbOperationBusy.Close();
                Console.Beep(1000, 20);
            }
            DialogSupport mbOperationFeedback = new DialogSupport();
            if (hPara[0] == 1)
                mbOperationFeedback.DoneThenCloseMessageBox(2);
            else
                mbOperationFeedback.ErrorThenCloseMessageBox(2);
        }
        #endregion

        #region //================================================================BtnSectorErase_Click
        private void BtnSectorErase_Click(object sender, EventArgs e)
        {
            if (tbEEPROMPassword.Text != "EEPROM")
            {
                MessageBox.Show("Type 'EEPROM' within textbox below to enable Sector Erase Procedure", "EEPROM Password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (mywfSectorErasePara == null)
            {
                mywfSectorErasePara = new wfSectorErasePara();
                mywfSectorErasePara.MyGlobalBase(myGlobalBase);
            }
            mywfSectorErasePara.ShowDialog();
            if (myGlobalBase.EE_isEraseOkay == false)       // Cancelled or incorrect parameter, cannot do erase.
            {
                tbEEPROMPassword.Text = "Error";
                return;
            }
            //-------------------------------------------------
            tbEEPROMPassword.Text = "";
            EECounter = myGlobalBase.EE_Sector_Start;
            myMainProg.Terminal_Append_Response("Initiating Erase process, please wait\n");
            m_sEntryTxt = "EE_ERASESEC(";
            m_sEntryTxt += (myGlobalBase.EE_Select_DeviceIC - 1).ToString("D") + ";";
            m_sEntryTxt += EECounter.ToString("D") + ")";
            //Terminal_Append_Response(m_sEntryTxt);        // Do not use, it tend to hang for some reason. 
            myMainProg.Command_ASCII_Process_UARTs(m_sEntryTxt);
            if (EEtimer == null)
            {
                EEtimer = new Timer();
                EEtimer.Elapsed += new System.Timers.ElapsedEventHandler(EE_SectorEraseLoop);
            }
            EEtimer.Interval = 2000;
            EEtimer.AutoReset = true;
            EEtimer.Start();

        }
        #endregion

        #region //================================================================EE_SectorEraseLoop
        void EE_SectorEraseLoop(object sender, System.Timers.ElapsedEventArgs e)
        {
            // ###TASK, how to monitor for end of process?
            if (EECounter >= myGlobalBase.EE_Sector_Stop)
            {
                EEtimer.AutoReset = false;
                EEtimer.Elapsed -= EE_SectorEraseLoop;  // Unsubscribe
                EEtimer = null;
                tbEEPROMPassword.Text = "";
                Console.Beep(1000, 20);
                return;
            }
            EECounter++;
            m_sEntryTxt = "EE_ERASESEC(";
            m_sEntryTxt += (myGlobalBase.EE_Select_DeviceIC - 1).ToString("D") + ";";
            m_sEntryTxt += EECounter.ToString("D") + ")";
            //Terminal_Append_Response(m_sEntryTxt);        // Do not use, it tend to hang for some reason. 
            myMainProg.Command_ASCII_Process_UARTs(m_sEntryTxt);
        }
        #endregion

        #region //================================================================btnVerifyErase_Click
        private void btnVerifyErase_Click(object sender, EventArgs e)
        {
            myMainProg.Terminal_Append_Response("---INFO: Verifying for empty Logger Memory\n");
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_EEPROM_isEmptyLogData = new DMFP_Delegate(DMFP_EEPROM_isEmptyLogData_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+EEISEMPTY(0x1)";
            if (myGlobalBase.bNoRFTermMessage == false)
                myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
            Thread t1;
            if (myGlobalBase.is_Serial_Server_Connected == true)
                t1 = new Thread(() => myDMFProtocol.DMFP_Send_Command(fpCallBack_EEPROM_isEmptyLogData, sMessage, 250));
            else
                t1 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_EEPROM_isEmptyLogData, sMessage, 250, (int)eUSBDeviceType.BGDRILLING));
            t1.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
            tbEEPROMPassword.Text = "Busy/Wait";
            //-------------------------------------------------
        }
        #endregion

        #region //================================================================DMFP_EEPROM_isEmptyLogData_CallBack 
        public void DMFP_EEPROM_isEmptyLogData_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            mbOperationBusy = new DialogSupport();
            tbEEPROMPassword.Text = "";
            if (hPara[0] == 0xFF)
            {
                mbOperationBusy.PopUpMessageBox("Logger Data Memory is Empty!", "Log Data Tool Operation", 2);
            }
            else
            {
                mbOperationBusy.PopUpMessageBox("Error!: Logger Data is Not Empty!", "Log Data Tool Operation", 2);
            }

        }
        #endregion

        #region //================================================================btnRTCSend_Click 
        private void btnRTCSend_Click(object sender, EventArgs e)
        {
            myMainProg.RTC_Setup_SendNow();

        }
        #endregion

        #region //================================================================btnRTCGet_Click
        private void btnRTCGet_Click(object sender, EventArgs e)
        {
            myMainProg.RTC_Setup_GetNow();

        }
        #endregion

        #region //================================================================btnDoneClose_Click
        private void btnDoneClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region //================================================================tbEEPROMPassword_DoubleClick
        private void tbEEPROMPassword_DoubleClick(object sender, EventArgs e)
        {
            tbEEPROMPassword.Text = "EEPROM";
        }
        #endregion
    }
}

