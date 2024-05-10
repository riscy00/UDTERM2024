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
// Power Tool: insert QuickLaunch with "@tasks CollapseRegions" to close all region only. 
namespace UDT_Term_FFT
{
    public delegate void DMFP_Delegate(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage);

    public partial class DMFProtocol : Form
    {
        //#####################################################################################################
        //###################################################################################### Variable/Class
        //#####################################################################################################

        ITools Tools = new Tools();
        List<RegisterDMFP> myRegisterDMFP;
        public bool isDMFPBusy { get; set; }                    // True = awaiting command.
        private string m_sEntryTxt;

        //#####################################################################################################
        //###################################################################################### Reference
        //#####################################################################################################

        
        USB_FTDI_Comm myUSBComm;                    // FTDI device manager section (scan, open, close, read, write).
        USB_VCOM_Comm myUSBVCOMComm;
        GlobalBase myGlobalBase;
        //BG_ToolSetup myBGToolSetup;
        BG_Orientation myBGOrientation;
        BG_ToolSerial myBGToolSerial;
        MainProg myMainProg;
        EEpromTools myEEpromTools;
        ADIS16460 myADIS16460;
        BG_Battery myBGBattery;

        #region// ============================================================Reference Object
        public void MyBGBattery(BG_Battery myBGBatteryRef)
        {
            myBGBattery = myBGBatteryRef;
        }
        public void MyBGToolSerial(BG_ToolSerial myBGToolSerialRef)
        {
            myBGToolSerial = myBGToolSerialRef;
        }
        public void MyBGOrientation(BG_Orientation myBGOrientationRef)
        {
            myBGOrientation = myBGOrientationRef;
        }
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }
        public void MyUSBComm(USB_FTDI_Comm myUSBCommRef)
        {
            myUSBComm = myUSBCommRef;
        }
        public void MyUSBVCOMComm(USB_VCOM_Comm myUSBVCOMCommRef)
        {
            myUSBVCOMComm = myUSBVCOMCommRef;
        }
        //public void MyBGToolSetup(BG_ToolSetup myBGToolSetupRef)
        //{
        //    myBGToolSetup = myBGToolSetupRef;
        //}
        public void MyEEpromTools(EEpromTools myEEpromToolsRef)
        {
            myEEpromTools = myEEpromToolsRef;
        }
        public void MyMiniADIS1640(ADIS16460 myADIS16460REf)
        {
            myADIS16460 = myADIS16460REf;
        }
        public bool ValidateRef_MyADIS16460()
        {
            if (myADIS16460 == null)
                return (false);
            return (true);
        }

        #endregion

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public DMFProtocol()
        {
            InitializeComponent();
        }
        //#####################################################################################################
        //###################################################################################### Window
        //#####################################################################################################

        #region //==================================================DMFProtocol_FormClosing
        private void DMFProtocol_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        #endregion

        #region //==================================================DMFProtocol_Load
        private void DMFProtocol_Load(object sender, EventArgs e)
        {
            DMFProtocol_UpdateList();
        }
        #endregion

        #region //==================================================DMFProtocol_UpdateListFresh form
        public void DMFProtocol_UpdateList()
        {
            if (this.Visible == false)
                return;
            int index = 0;
            listBox1.Items.Clear();
            if (myRegisterDMFP != null)
            {
                try
                {
                    foreach (var item in myRegisterDMFP)
                    {
                        listBox1.Items.Add(" Index: " + index.ToString() + " | Message: " + item.sDuplexCommand + "(....)");
                        index++;
                    }
                }
                catch
                {
                    // ignore any exception affected by real time change in listing....
                }
            }
            this.Refresh();
        }
        #endregion

        #region //==================================================btnRefresh_Click
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            DMFProtocol_UpdateList();
        }
        #endregion

        #region //==================================================btnReset_Click
        private void btnReset_Click(object sender, EventArgs e)
        {
            DMFProtocol_Reset();
            this.Refresh();
        }
        #endregion

        #region //==================================================DMFProtocol_Reset
        public void DMFProtocol_Reset()
        {
            if (listBox1 !=null)
                listBox1.Items.Clear();
            if (myRegisterDMFP != null)
                myRegisterDMFP.Clear();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Send/Recieved/Async
        //#####################################################################################################
        #region //==================================================DMFP_Process_Command (all in one package)
        // CallBackFunction:    Delegate for function excution after recieving matching command with -
        // sCommandMessage:     Command to send out, must alway start with +
        // isEnableDebugText:   Display text, NB: the terminal is slow and can get out of sync, it good practice to set to false to disable
        // VCOMArray:           -1 = override and use non-array VCOM (may not work if VCOM not activated). >0 = selected Array VCOM that is activated. 
        // Return:              False, VCOM Array not enabled. True = Success. 
        public bool DMFP_Process_Command(DMFP_Delegate CallBackFunction, string sCommandMessage, bool isEnableDebugText, int VCOMArray)
        {
            if (VCOMArray != -1)    // This is generic VCOM (non-array type)
            {
                if (myGlobalBase.myUSBVCOMArray[VCOMArray].isDMFProtocolEnabled == false)
                    return false;
            }
            //---------------------------------------------------------------------- SNAD Address added to command message Mods. 
            if (myGlobalBase.mySNADNetwork != null)
            {
                sCommandMessage = myGlobalBase.mySNADNetwork.SNAD_Modify_UDT_Command(sCommandMessage);
            }
            //----------------------------------------------------------------------
            if (isEnableDebugText==true)
                myMainProg.myRtbTermMessageLF(sCommandMessage);
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form.
            Thread t1;
            if ((myGlobalBase.is_Serial_Server_Connected == true)|(VCOMArray==-1))
                t1 = new Thread(() => DMFP_Send_Command(CallBackFunction, sCommandMessage, 250));
            else
                t1 = new Thread(() => DMFP_VCOMArry_Send_Command(CallBackFunction, sCommandMessage, 250, VCOMArray));
            t1.Start();
            Thread.Sleep(10);               // Slow down batch commands. 
            DMFProtocol_UpdateList();       // Update list. 
            return true;
        }
        #endregion

        #region //==================================================DMFP_VCOMArry_Send_Command
        public void DMFP_VCOMArry_Send_Command(DMFP_Delegate functionPointer, string CmdMessage, int TimeOutPeriod, int eSelectType)
        {
            isDMFPBusy = true;
            //------------------------------------------------------------------Validate the entry. 
            if (CmdMessage.StartsWith("+", StringComparison.Ordinal) == false)    //Must have + on command message        
                return;
            //---------------------------------------------------------------------- SNAD Address added to command message Mods. 
            if (myGlobalBase.mySNADNetwork != null)
            {
                CmdMessage = myGlobalBase.mySNADNetwork.SNAD_Modify_UDT_Command(CmdMessage);
            }
            //----------------------------------------------------------------------
            int startParaIndex = CmdMessage.IndexOf("(");
            if (startParaIndex <= 0)                                              // Must have parameter start brace even it empty (). 
                return;
            //------------------------------------------------------------------Prepare message for sending 
            if (CmdMessage.Contains("\n") | CmdMessage.Contains("\r"))          // check for command end termination 
            {
                m_sEntryTxt = CmdMessage;
            }
            else
            {
                m_sEntryTxt = CmdMessage + '\n';
            }

            //------------------------------------------------------------------Register to the List (if not nulled)
            if (functionPointer != null)                                                                // null =no call back expected
            {
                CmdMessage = CmdMessage.Remove(startParaIndex, (CmdMessage.Length - startParaIndex));   // Delete Parameter section before List register
                CmdMessage = Tools.StrReplaceAtIndex(CmdMessage, 0, '-');                               // replace + to - before registration.
                if (myRegisterDMFP == null)
                {
                    myRegisterDMFP = new List<RegisterDMFP>();                                          // Create Object Instance.
                }
                myRegisterDMFP.Add(new RegisterDMFP(functionPointer, TimeOutPeriod, CmdMessage, myGlobalBase));
            }

            //------------------------------------------------------------------Place Message to rtbTerm Window.
            /*
            if (myGlobalBase.IsHexDisplayEnable == true)                                                // Add \r\n to get over hex data.
            {
                myMainProg.myRtbTermMessage("\r\n#DMFP: " + m_sEntryTxt);
            }
            else
            {
                myMainProg.myRtbTermMessage("#DMFP:" + m_sEntryTxt);
            }
            */
            if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_FTDI)
            {
                myMainProg.myRtbTermMessageLF("#DMFP: #ERR#: FTDI Type Serial is not supported, use VCOM");
            }
            else if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM)
            {
                myUSBVCOMComm.VCOMArray_Message_Send_DMFP(m_sEntryTxt, eSelectType);
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#DMFP: #ERR#: Serial Port is Not Activated");
            }
        }
        #endregion

        #region //==================================================DMFP_Send_Command
        public void DMFP_Send_Command(DMFP_Delegate functionPointer, string CmdMessage, int TimeOutPeriod)
        {
            isDMFPBusy = true;
            //------------------------------------------------------------------Validate the entry. 
            if (CmdMessage.StartsWith("+", StringComparison.Ordinal) == false)    //Must have + on command message        
                return;
            int startParaIndex = CmdMessage.IndexOf("(");
            if (startParaIndex == 0)                                              // Must have parameter start brace even it empty (). 
                return;
            //---------------------------------------------------------------------- SNAD Address added to command message Mods. 
            if (myGlobalBase.mySNADNetwork != null)
            {
                CmdMessage = myGlobalBase.mySNADNetwork.SNAD_Modify_UDT_Command(CmdMessage);
            }
            //----------------------------------------------------------------------
            //------------------------------------------------------------------Prepare message for sending 
            if (CmdMessage.Contains("\n") | CmdMessage.Contains("\r"))          // check for command end termination 
            {
                m_sEntryTxt = CmdMessage;
            }
            else
            {
                m_sEntryTxt = CmdMessage + '\n';
            }
            //------------------------------------------------------------------Register to the List (if not nulled)
            if (functionPointer != null)                                                                // null =no call back expected
            {
                CmdMessage = CmdMessage.Remove(startParaIndex, (CmdMessage.Length - startParaIndex));   // Delete Parameter section before List register
                CmdMessage = Tools.StrReplaceAtIndex(CmdMessage, 0, '-');                               // replace + to - before registration.
                if (myRegisterDMFP == null)
                {
                    myRegisterDMFP = new List<RegisterDMFP>();                                          // Create Object Instance.
                }
                myRegisterDMFP.Add(new RegisterDMFP(functionPointer, TimeOutPeriod, CmdMessage, myGlobalBase));
            }

            //------------------------------------------------------------------Place Message to rtbTerm Window.
            /*
            if (myGlobalBase.IsHexDisplayEnable == true)                                                // Add \r\n to get over hex data.
            {
                myMainProg.myRtbTermMessage("\r\n#DMFP: " + m_sEntryTxt);
            }
            else
            {
                myMainProg.myRtbTermMessage("#DMFP:" + m_sEntryTxt);
            }
            */
            myGlobalBase.EE_isDMFP_Mode_Enabled = true;
            if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_FTDI)
            {
                myUSBComm.FTDI_Message_Send_DMFP(m_sEntryTxt);
            }
            else if (myGlobalBase.USB_SelectDeviceMode == (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM)
            {
                myUSBVCOMComm.VCOM_Message_Send_DMFP(m_sEntryTxt);
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#DMFP: #ERR#: Serial Port is Not Activated");
            }
            myGlobalBase.EE_isDMFP_Mode_Enabled = false;
        }   
        #endregion

        #region //==================================================DMFP_Recieved_Command
        public void DMFP_Recieved_Command(string CmdMessage)
        {
            string MessageFull = CmdMessage;
            string CmdCommand = "";
            string CmdParameter = "";
            if (CmdMessage.StartsWith("-", StringComparison.Ordinal) == false)
            {
                //###TASK, check each item for timeout and deregister if expired.
                return;
            }
            //---------------------------------------------------------------------- SNAD Address added to command message Mods. 
            if (myGlobalBase.mySNADNetwork != null)
            {
                myGlobalBase.mySNADNetwork.SNAD_Read_SNAD_From_UDT_Command(MessageFull);
            }
            //----------------------------------------------------------------------
            Frame_Detokeniser myDetoken = new Frame_Detokeniser();
            int DeTokenErrorCode = myDetoken.DetokeniserMessage(MessageFull, ref CmdCommand, ref CmdParameter);
            if (DeTokenErrorCode != 0)
            {
                switch (DeTokenErrorCode)
                {
                    case -1:        // Internal processing error: never happen here so there no bug in code. 
                        {
                            myMainProg.myRtbTermMessageLF("#E: Internal Error in DetokeniserMessage() Class");
                            return;
                        }
                    case -2:        // Substandard Frame
                        {
                            myMainProg.myRtbTermMessageLF("+W: Substandard frame detected, ignored. Msg:" + CmdMessage);
                            return;
                        }
                    case -4:        // Comment Frame, no need to process further.
                        {
                            myMainProg.myRtbTermMessageLF("#E: No Command in the frame detected. Msg:" + CmdMessage);
                            return;
                        }
                    case -3:        // Comment Frame, no need to process further.
                        {
                            break;
                        }
                    default:
                        break;
                }
            }
            //---------------------------------------------------------------Scan the list and check for command match and then deregister
            if (myRegisterDMFP != null)
            {
                foreach (var dataRegisterDMFP in myRegisterDMFP)
                {
                    if (CmdCommand == dataRegisterDMFP.sDuplexCommand)
                    {
                        isDMFPBusy = false;
                        dataRegisterDMFP.dfunctionpointer(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull);    // Callback to function that send the command.  
                        myRegisterDMFP.Remove(dataRegisterDMFP);
                        break;
                    }
                }
            }
            //myMainProg.myRtbTermMessageLF("#DMFP:"+CmdMessageFull);
            DMFProtocol_UpdateList();
        }
        #endregion

        #region //==================================================DMFP_Recieved_Async_Command
        public void DMFP_Recieved_Async_Command(string CmdMessage)
        {
            bool BGCONNECTstate = false;
            string MessageFull = CmdMessage;        // Passed to detoken.
            string CmdCommand = "";
            string CmdParameter = "";
            //---------------------------------------------------------------------- SNAD Address added to command message Mods. 
            if (myGlobalBase.mySNADNetwork != null)
            {
                myGlobalBase.mySNADNetwork.SNAD_Read_SNAD_From_UDT_Command(CmdMessage);
            }
            //---------------------------------------------------------------------- 
            Frame_Detokeniser myDetoken = new Frame_Detokeniser();
            int DeTokenErrorCode = myDetoken.DetokeniserMessage(MessageFull, ref CmdCommand, ref CmdParameter);
            if (DeTokenErrorCode != 0)
            {
                switch (DeTokenErrorCode)
                {
                    case -1:        // Internal processing error: never happen here so there no bug in code. 
                        {
                            myMainProg.myRtbTermMessageLF("#E: Internal Error in DetokeniserMessage() Class");
                            return;
                        }
                    case -2:        // Substandard Frame
                        {
                            myMainProg.myRtbTermMessageLF("+W: Substandard frame detected, ignored. Msg:" + CmdMessage);
                            return;
                        }
                    case -4:        // Comment Frame, no need to process further.
                        {
                            myMainProg.myRtbTermMessageLF("#E: No Command in the frame detected. Msg:" + CmdMessage);
                            return;
                        }
                    case -3:        // Comment Frame, no need to process further.
                        {
                            break;
                        }

                    default:
                        break;
                }
            }
            //--------------------------------------------------------------Async call to routine, depending on received command. 
            isDMFPBusy = false;
            switch (CmdCommand)
            {
                //case ("~BATTERY"):
                //    {
                //        myBGToolSetup.zBG_Async_Battery(iPara, hPara, sPara, dPara, CmdMessage, FullMessage, Asynclistparameter);
                //        break;
                //    }
                //case ("~BGCHECK"):          // BGDrilling
                //    {
                //        myBGToolSetup.zBG_Async_BGCheck(iPara, hPara, sPara, dPara, CmdMessage, FullMessage, Asynclistparameter);
                //        break;
                //    }
                case ("~EE_ISEMPTY"):       // BGDrilling
                    {
                        myMainProg.zBG_Async_EE_ISEMPTY(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break; 
                    }
                case ("~RTC_CHECK"):        // BGDrilling
                    {
                        myMainProg.zBG_Async_RTC_CHECK(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                case ("~EE_ERASEBULK"):     // BGDrilling
                    {
                        myEEpromTools.zBG_Async_EE_ERASEBULK(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                case ("~ADISDATA"):         // TVB 
                    {
                        if (myADIS16460!=null)
                            myADIS16460.MiniAD9974_Async_ADISDATA_CallBack(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                case ("~ERR_REPORT"):       // BGDrilling
                    {
                        myMainProg.zBG_Async_ERR_REPORT(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                case ("~BGCONNECT"):       // BGDrilling
                    {
                        BGCONNECTstate = true;      // Defer to end of this routine. 
                        break;
                    }
                case ("~TOCOTP"):           // BGDrilling
                    {
                        myBGOrientation.Async_BGOrientation_RecievedData(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                //----------------------------------------------------------------------------------------------------------------Stealth and Session Logger. 
                case ("~L1"):               // TVB
                    {
                        myMainProg.AsyncSessionLogReciever_L1(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                case ("~L2"):               // TVB
                    {
                        myMainProg.AsyncSessionLogReciever_L2(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                case ("~LH"):               // TVB
                    {
                        myMainProg.AsyncSessionLogReciever_LH(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                //----------------------------------------------------------------------------------------------------------------Data Session Series (A to F)
                case ("~LA"):       // Hex Data save, in metadata statement, where filename is created with header.
                    {
                        myMainProg.AsyncSessionLogReciever_LA(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                case ("~LB"):       // Header Frame for data column.
                    {
                        myMainProg.AsyncSessionLogReciever_LB(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                case ("~LC"):       // Format Frame for data column.
                    {
                        myMainProg.AsyncSessionLogReciever_LC(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                case ("~LD"):       // Hex Data, separated by semicomma, save as INT32 data (from INT8, INT16, INT32) 
                    {
                        myMainProg.AsyncSessionLogReciever_LD(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                case ("~LE"):       // Hex Data, seperated by semicomma, save as string or mixed format
                    {
                        myMainProg.AsyncSessionLogReciever_LE(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                case ("~LF"):       // End of data stream, put collection into Logger Window, can contains post data collection statement, ie final number of collected data. 
                    {
                        myMainProg.AsyncSessionLogReciever_LF(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
                        break;
                    }
                //----------------------------------------------------------------------------------------------------------------
                case ("XXX6"):
                    {
                        break;
                    }
                default:
                    break;
            }
            DMFProtocol_UpdateList();
            //----------------------------------------------------------------------------------------------------------------
            if (BGCONNECTstate==true)
                myMainProg.zBG_Async_BGCONNECT(myDetoken.iPara, myDetoken.hPara, myDetoken.sPara, myDetoken.dPara, CmdParameter, MessageFull, myDetoken.listparameter);
        }
        #endregion

        #region //==================================================btnTest_Click
        public void btnTestButtonRun()
        {
            DMFP_Delegate FunctionPointerUPF = new DMFP_Delegate(DMFP_UPF_CallBack);
            DMFP_Send_Command(FunctionPointerUPF, "+UPF(0;0x11111111;0x22222222;0x33333333;0x44444444;0x55555555;0x66666666;0x77777777;0x88888888)\n", 10);
            DMFP_Delegate FunctionPointerUPG = new DMFP_Delegate(DMFP_UPG_CallBack);
            DMFP_Send_Command(FunctionPointerUPG, "+UPG(1;0x11111111;0x22222222;0x33333333;0x44444444;0x55555555;0x66666666;0x77777777;0x88888888)\n", 10);
            DMFP_Delegate FunctionPointerUPA = new DMFP_Delegate(DMFP_UPA_CallBack);
            DMFP_Send_Command(FunctionPointerUPA, "+UPA(1;0x11111111;0x22222222;0x33333333;0x44444444;0x55555555;0x66666666;0x77777777;0x88888888)\n", 10);

            DMFP_Recieved_Command("-UPG(0;0x11111111;testc;0x33333333,0x44444444;0x55555555;0x66666666;0x77777777;0x88888888)\n");
        }
        public static void DMFP_UPF_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {

        }
        public static void DMFP_UPG_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {

        }
        public static void DMFP_UPA_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {

        }

        #endregion

        //#####################################################################################################
        //###################################################################################### Feedback Status
        //#####################################################################################################

        #region //==================================================bTimeOutStatusOnMostRecentDFMPFrame
        public bool bTimeOutStatusOnMostRecentDFMPFrame()       // True = Busy/Active Task || False = Completed Task or timeout event (move on to next step)
        {
            if (myRegisterDMFP.Count == 0)
                return (false);                                 // DMFP Buffer is empty (job done)
            if (myRegisterDMFP[myRegisterDMFP.Count - 1].isTimeOut_Polling() == true)
                return (false);                                 // Time Out Happen
            return (true);                                      // Still Busy 
        }
        #endregion
    }
    //#####################################################################################################
    //###################################################################################### Class
    //#####################################################################################################

    #region //=======================================================RegisterDMFP Class
    public class RegisterDMFP
    {
        GlobalBase myGlobalBase;
        private Int64 DateTimeTickExpires;
        // ==================================================================Getter/Setter
        public int iTimeOutPeriod { get; set; }                 // Second, amount of time before de-registered if no reply occurred.
        public string sDuplexCommand { get; set; }              // Command that sent out to
        public bool bExpectAsyncReply { get; set; }             // true = allow async reply, ie do not de-register from the list. 
        public DMFP_Delegate dfunctionpointer { get; set; }     // Delegate is used as function pointer for call back purpose.
        // ==================================================================constructor
        public RegisterDMFP(DMFP_Delegate _dfunctionpointer, int _iTimeOutPeriod, string _sDuplexCommand, GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
            if (_iTimeOutPeriod == 0)
                _iTimeOutPeriod = 1;                    // Default 1 Second. 
            iTimeOutPeriod = _iTimeOutPeriod;
            sDuplexCommand = _sDuplexCommand;
            dfunctionpointer = _dfunctionpointer;
            DateTimeTickExpires = DateTime.Now.Ticks + (iTimeOutPeriod * 10000000);      // Registered time now plus expiry period (1 second = 10,000,000 tick). 
        }

        public bool isTimeOut_Polling()
        {
            myGlobalBase.bNoRFTermMessage = false;
            Int64 currentime = DateTime.Now.Ticks;
            if (currentime > DateTimeTickExpires)
                return true;
            return false;
        }

        //###TASK: Devise way for event method.
    }
    #endregion
}
