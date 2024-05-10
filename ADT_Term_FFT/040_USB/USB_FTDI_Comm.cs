//====================================================Propietary Software.
// FTDI FT232R Driver
// (c) Richard Payne, Varna, Bulgaria 10/12/2011
// riscy00@gmail.com
// Its use, redistribution or modification is prohibited. 
// Note: Advanced code for thread safe reciver
//       Use latest implementation solution. 
//====================================================Propietary Software.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTD2XX_NET;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Management;
// Power Tool: insert QuickLaunch with "@tasks CollapseRegions" to close all region only
namespace UDT_Term_FFT
{
    public class USB_FTDI_Comm
    {
        //#####################################################################################################
        //###################################################################################### Variable/Classes
        //#####################################################################################################
        ITools Tools = new Tools();
        FTDI FTDI_Device;
        FTDI.FT_STATUS FTDI_Ftstatus = FTDI.FT_STATUS.FT_DEVICE_NOT_FOUND;
        FTDI.FT_DEVICE_INFO_NODE[] FTDI_DevList;

        private string[,] FTDIList = new string[5, 3];          // USB reports

        //#####################################################################################################
        //###################################################################################### Reference 
        //#####################################################################################################

        #region //==================================================Reference
        GlobalBase myGlobalBase;
        MainProg myMainProg;
        USB_Message_Manager myUSB_Message_Manager;
        public void MyUSB_Message_Manager(USB_Message_Manager myUSB_Message_ManagerRef)
        {
            myUSB_Message_Manager = myUSB_Message_ManagerRef;
        }
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Public Variable 
        //#####################################################################################################

        #region //==================================================Getter/Setter

        private uint m_FTDI_Device_Count;
        private uint m_FTDI_Index;
        private string m_FTDI_Description;
        private string m_FTDI_Device_Type;
        private int m_USBLIN_Device_ID;        // See USBUARTDeviceType enum below
        private bool m_RX_IsValid_Message;

        public uint FTDI_Device_Count       { get { return m_FTDI_Device_Count; } set { m_FTDI_Device_Count = value; } }
        public uint FTDI_Index              { get { return m_FTDI_Index; } set { m_FTDI_Index = value; } }
        public string FTDI_Description      { get { return m_FTDI_Description; } set { m_FTDI_Description = value; } }
        public string FTDI_Device_Type      { get { return m_FTDI_Device_Type; } set { m_FTDI_Device_Type = value; } }
        public int USBLIN_Device_ID         { get { return m_USBLIN_Device_ID; } set { m_USBLIN_Device_ID = value; } }
        public bool RX_IsValid_Message      { get { return m_RX_IsValid_Message; } set { m_RX_IsValid_Message = value; } }

        #endregion

        //#####################################################################################################
        //###################################################################################### Special Code for textbox repaint. 
        //#####################################################################################################

        #region //==================================================Special Code for textbox repaint.
        private const int WM_SETREDRAW = 0x000B;
        private const int WM_USER = 0x400;
        private const int EM_GETEVENTMASK = (WM_USER + 59);
        private const int EM_SETEVENTMASK = (WM_USER + 69);
        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        IntPtr eventMask = IntPtr.Zero;
        #endregion

        //#####################################################################################################
        //###################################################################################### Public Reference
        //#####################################################################################################
        public FTDI FTDI_DeviceRef { get { return this.FTDI_Device; } } // Passing object reference to other class.

        //public FTDI.FT_DEVICE_INFO_NODE FTDI_DeviceInfoRef { get { return this.FTDI_DevList[m_FTDI_Index]; } }

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################

        public USB_FTDI_Comm()
        {
            FTDI_Device = new FTDI();
            m_FTDI_Device_Count = 0;
            m_USBLIN_Device_ID = (byte) GlobalBase.USBUARTDeviceType.NO_DEVICE;
            m_RX_IsValid_Message = false;
            //====================================Set up separate thread for USB receives monitoring.
            var readThread = new Thread(FTDI_Thread_ReadData);
            readThread.Start();
        }
        //#####################################################################################################
        //###################################################################################### FTDI_Thread_ReadData
        //#####################################################################################################

        #region //==================================================FTDI_Thread_ReadData
        //-----Technology based on http://zoidbergsolutions.nl/blog/2011/09/30/connecting-to-ftdi-devices-using-silverlight-5-rc/
        //-----Modified by Riscy for handling BeginInvoke since above link use asp (web), where here use C#, 
        //-----Further work optionally to remove thread sleep completely. But in general, the routine work, need performance testing later.
        //-----10/12/201.
        private void FTDI_Thread_ReadData()
        {
            UInt32 nrOfBytesAvailable = 0;
            while (myGlobalBase.IsApplicationQuit==false)           //while (true), modified which make it possible to quit the FTDI constant loop monitoring. 
            {
                //### TODO, improve code to removes thread dependency. 
                var readStatus = FTDI_Device.GetRxBytesAvailable(ref nrOfBytesAvailable);
                if (readStatus != FTDI.FT_STATUS.FT_OK)
                {
                    Thread.Sleep(1);
                    continue;
                }
                if (nrOfBytesAvailable > 0)
                {
                    byte[] readData = new byte[nrOfBytesAvailable];
                    UInt32 numBytesRead = 0;
                    readStatus = FTDI_Device.Read(readData, nrOfBytesAvailable, ref numBytesRead);
                    string data = Encoding.UTF8.GetString(readData, 0, readData.Length);
                    myUSB_Message_Manager.ComMSG_Designate_ReadData_To_App(data, readData);
                }
                Thread.Sleep(10);
            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### FTDI Send
        //#####################################################################################################

        #region //==================================================FTDI_Message_Send_DMFP
        //==========================================================
        // Purpose  : Send Message under DMFP Protocol
        // Input    : 
        // Output   : 
        //==========================================================
        public void FTDI_Message_Send_DMFP(string sTX)
        {
            if (myGlobalBase.EE_isDMFP_Mode_Enabled==false)
            {
                myMainProg.myRtbTermMessageLF("-W: <EE_isDMFP_Mode_Enabled flag> is false in FTDI section, check code!");
            }
            uint numBytes = 0;
            myUSB_Message_Manager.MessageTX = sTX;                                          // For reference only.
            //sTX = sTX + "\r\n";                                       // Do not send "\r\n" as this screw up LPC1549 ring buffer (need debugging there). 
            if (FTDI_Device.IsOpen == true)
            {
                //------------------------------------------------------Transmits
                FTDI_Device.SetTimeouts(500, 500);                     // Set timeout to RX=500mS, TX=500mS force quit if expired, this avoid hanging.
                FTDI_Ftstatus = FTDI_Device.Write(sTX, sTX.Length, ref numBytes);
                if (FTDI_Ftstatus != FTDI.FT_STATUS.FT_OK)
                {
                    myMainProg.myRtbTermMessageLF("#E: (EXPORT MODE): FTDI Device Transmits Failures");
                    return;
                }
                //Trace.WriteLine("-INFO (EXPORT MODE): FTDI Device Transmits Success :'" + sTX + "'");
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: (EXPORT MODE): FTDI Device is not established");
            }
            
        }
        #endregion

        #region //==================================================FTDI_Message_Send
        //==========================================================
        // Purpose  : Send Message.
        // Input    : String message, please avoid the use of '\r' ASCII.
        // Output   : 
        //          : Important Note, use string to send data or hex <> method. 
        //          : In case of using '\r', ';', ',', '(', ')' in string message move it to other ASCII map ie 0x10 onward. See EEPROMExportImport section.
        //          : within EEPROM_ButtonClicked_ExportDataNow(). 
        //          : The  "sTX = sTX.Replace(" ", "");" will impact the space in message, so need to move space to other ASCII map as well. 
        //==========================================================
        public void FTDI_Message_Send(string sTX)
        {                                                               // This routine take place after the scan
            uint numBytes = 0;
            myUSB_Message_Manager.MessageTX = sTX;                                          // For reference only.
            //-----------------------------------------------Hex Send Message Mode <32><45><453F><89AF>
            if (sTX.Contains('<') & sTX.Contains('>'))                  // Check for Hex Display Mode
            {

                sTX = sTX.Replace("<", "");                             // remove unwanted typo
                sTX = sTX.Replace(">", "");
                sTX = sTX.Replace(",", "");
                sTX = sTX.Replace(";", "");
                sTX = sTX.Replace(" ", "");
                
                if (Tools.IsString_Hex(sTX) == true)
                {
                    byte[] bHex;
                    //------------------------------------------Convert Hex ASCII into byte and then back to string in hex data ie "3F" => 0x3F
                    if (myGlobalBase.IsHexSendEnableWithLF==true)
                    {
                        bHex = Tools.StrToHexBytesNonASCIILF(sTX);
                    }
                    else
                    {
                        bHex = Tools.StrToHexBytesNonASCII(sTX);
                    }
                    if (FTDI_Device.IsOpen == true)
                    {
                        //------------------------------------------------------Transmits
                        FTDI_Device.SetTimeouts(500, 500);                     // Set timeout to RX=500mS, TX=500mS force quit if expired, this avoid hanging.

                        FTDI_Ftstatus = FTDI_Device.Write(bHex, bHex.Length, ref numBytes);
                        if (FTDI_Ftstatus != FTDI.FT_STATUS.FT_OK)
                        {
                            myMainProg.myRtbTermMessageLF("#E : FTDI Device Transmits Failures");
                            return;
                        }
                        Trace.WriteLine("-INFO : FTDI Device Transmits Success :'" + sTX + "'");
                    }
                    else
                    {
                        myMainProg.myRtbTermMessageLF("#ERR : FTDI Device is not established");
                    }
                }
                //-----------------------------------------------ASCII Send Message Mode. 
                else
                {
                    myMainProg.myRtbTermMessageLF("#ERR: HEX frame via '<' & '>' failed, check typo, ie '<03><1A><6AB2>' (8 bit or 16 bits) and enter");
                }
            }
            else
            {
                sTX = sTX.Replace("\r", "");                                    // 5B: Remove '\r' as it cause problem in certain MCU operation (bug fixes) 
                if (sTX.Contains("\n") == false)
                    sTX = sTX + "\n";                                           // 5B: 5/2/17 RGP: Change from \r\n to \n only (no need for \r).  
                if (sTX != " ")                                                 // One certain Space char to cease certain activity for BG Directional. 
                    sTX = sTX.Replace(" ", "");                                 // remove white space in LIN message (easier to read).
                if (FTDI_Device.IsOpen == true)
                {
                    //------------------------------------------------------Transmits
                    FTDI_Device.SetTimeouts(500, 500);                     // Set timeout to RX=500mS, TX=500mS force quit if expired, this avoid hanging.
                    FTDI_Ftstatus = FTDI_Device.Write(sTX, sTX.Length, ref numBytes);
                    if (FTDI_Ftstatus != FTDI.FT_STATUS.FT_OK)
                    {
                        myMainProg.myRtbTermMessageLF("#ERR : FTDI Device Transmits Failures");
                        return;
                    }
                    Trace.WriteLine("-INFO : FTDI Device Transmits Success :'" + sTX + "'");
                }
                else
                {
                    myMainProg.myRtbTermMessageLF("#ERR : FTDI Device is not established");
                }
            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### FTDI Misc
        //#####################################################################################################

        #region //==================================================FTDI_Scan_GetData
        public void FTDI_Scan_GetData(int channel, ref string s0, ref string s1, ref string s2)
        {
            if (FTDIList[channel, 0] == "")
            {
                s0 = "Empty";
                s1 = "";
                s2 = "";
            }
            else
            {
                s0 = FTDIList[channel, 0];
                s1 = FTDIList[channel, 1];
                s2 = FTDIList[channel, 2];
            }
        }
        #endregion

        #region //==================================================FTDI_Scan_Device

        //==========================================================
        public void FTDI_Scan_Device()
        {
            int i;
            for (i = 0; i < 3; i++)
            {
                FTDIList[0, i] = "";
                FTDIList[1, i] = "";
                FTDIList[2, i] = "";
                FTDIList[3, i] = "";
                FTDIList[4, i] = "";
            }
            FTDI_Ftstatus = FTDI_Device.GetNumberOfDevices(ref m_FTDI_Device_Count);
            if (m_FTDI_Device_Count != 0)
            {
                FTDI_DevList = new FTDI.FT_DEVICE_INFO_NODE[m_FTDI_Device_Count];
                FTDI_Ftstatus = FTDI_Device.GetDeviceList(FTDI_DevList);

                //Trace.WriteLine("---REPORT : FTDI SCAN RESULTS ---");
                for (i = 0; i < m_FTDI_Device_Count; i++)
                {
                    FTDIList[i, 0] = "NO::" + i.ToString() + " ==> DES::" + FTDI_DevList[i].Description.ToString();
                    FTDIList[i, 1] = "SN::" + FTDI_DevList[i].SerialNumber.ToString() + " TYPE::" + FTDI_DevList[i].Type.ToString();
                    FTDIList[i, 2] = "LOCID::" + FTDI_DevList[i].LocId.ToString() + " ID::" + FTDI_DevList[i].ID.ToString() + " FLAG::" + FTDI_DevList[i].Flags.ToString() + " FTHANDLE::" + FTDI_DevList[i].ftHandle.ToString();
                    //Trace.WriteLine("======================================================");
                    //Trace.WriteLine("NO::" + i.ToString() + " ==> DES::" + FTDI_DevList[i].Description.ToString());
                    //Trace.WriteLine("      " + "SN::" + FTDI_DevList[i].SerialNumber.ToString() + " TYPE::" + FTDI_DevList[i].Type.ToString());
                    //Trace.WriteLine("      " + "LOCID::" + FTDI_DevList[i].LocId.ToString() + " ID::" + FTDI_DevList[i].ID.ToString() + " FLAG::" + FTDI_DevList[i].Flags.ToString() + " FTHANDLE::" + FTDI_DevList[i].ftHandle.ToString());
                }
                //Trace.WriteLine("---REPORT : END SCAN RESULTS ---");
            }
            else
            {
                //Trace.WriteLine("#ERR : NO FTDI DEVICE FOUND ---");
                FTDI_Index = 999;
            }
        }
        #endregion

        #region //==================================================FTDI_Seek_Open_FT232R_USB_UART_Module
        //==========================================================
        // Purpose  : Identify USB-UART FT232R device Module.
        // Input    : selected, if false, use loop to seek, if true select in accordance to m_FTDI_Device_Count
        // Output   : False, no device exist, otherwise TRUE, 
        //          : properties FTDI_Index is the channel number where is found, along with FTDI_Description and FTDI_Device_Type.
        //==========================================================

        public bool FTDI_Seek_Open_FT232R_USB_UART_Module(bool selected)
        {                                                               // This routine take place after the scan!
            uint i, j = 0;
            bool isfound = false;
            FTDI_Index = 999;                                           // Set 999 as device not found.

            if (m_FTDI_Device_Count == 0) return false;                // No FTDI device exist!
            if (selected == true)
            {
                j = m_FTDI_Device_Count - 1;                                // One loop only!
            }
            for (i = j; i < m_FTDI_Device_Count; i++)
            {
                FTDI_Description = FTDI_DevList[i].Description.ToString();  // Read description from the existing FTDI in the USB network.
                FTDI_Device_Type = FTDI_DevList[i].Type.ToString();         // Read device type, ie FT232R for support.
                if (selected == true)
                {
                    isfound = true;
                }
                else
                {
                    isfound = FTDI_Description.StartsWith("FT232R", System.StringComparison.OrdinalIgnoreCase);
                    if (isfound == false)  // Try again with different FT232R device (white/blue case and white cable, nice looking one)
                    {
                        isfound = FTDI_Description.StartsWith("US232R", System.StringComparison.OrdinalIgnoreCase);
                    }
                    if (isfound == false)  // Try again with different FT232R device (white/blue case and white cable, nice looking one)
                    {
                        isfound = FTDI_Description.StartsWith("UB232R", System.StringComparison.OrdinalIgnoreCase);     //UB232R device from FTDI. 
                    }
                }
                if (isfound == true)
                {
                    myMainProg.myRtbTermMessage("-I : Found USB_UART (FT232R)Channel No:" + i.ToString() + " DES:" + FTDI_DevList[i].Description.ToString() + Environment.NewLine);
                    m_FTDI_Index = i;
                    FTDI_Ftstatus = FTDI_Device.OpenByIndex(m_FTDI_Index);
                    if (FTDI_Ftstatus == FTDI.FT_STATUS.FT_OK)
                    {
                        myMainProg.myRtbTermMessage("-I : USB_UART (FT232R) Open Successfully:" + " HANDLE:" + FTDI_DevList[FTDI_Index].ftHandle.ToString() + Environment.NewLine);
                        m_USBLIN_Device_ID = (byte)GlobalBase.USBUARTDeviceType.USB_UART_FT232R;
                        //Setup Baud Rate and serial config
                        if (myGlobalBase.isUser_Baudrate == false)
                            FTDI_Device.SetBaudRate(myGlobalBase.iBaudRateList[myGlobalBase.Select_Baudrate]);
                        else
                            FTDI_Device.SetBaudRate(myGlobalBase.Select_Baudrate);
                          //0 = 1200, 6 = 115200, see above table. 
                        FTDI_Device.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
                        FTDI_Device.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_NONE, 0, 0);       //Added 18/02/15
                        return true;
                    }
                    else
                    {
                        myMainProg.myRtbTermMessage("#ERR: USB_UART (FT232R) Unable to Open!" + Environment.NewLine);
                        m_USBLIN_Device_ID = (byte) GlobalBase.USBUARTDeviceType.NO_DEVICE;
                        return false;
                    }
                }
            }
            myMainProg.myRtbTermMessage("#ERR : USB-UART Not Found" + Environment.NewLine);
            return false;
        }
        #endregion

        #region //==================================================FTDI_Message_ClosePort
        //==========================================================
        // Purpose  : Send and Receives Message.
        // Input    : String message as per excel instruction, permits white space or comma as this is later removed 
        // Output   : Response message as per excel instruction. 
        //          : properties FTDI_Index is the channel number where  is found, along with FTDI_Description and FTDI_Device_Type.
        //==========================================================
        public void FTDI_Message_ClosePort()
        {
            UInt32 counter = 0;
            if (FTDI_Device.IsOpen == true)
            {
                while (counter < 100)
                {
                    FTDI_Ftstatus = FTDI_Device.Close();
                    if (FTDI_Ftstatus == FTDI.FT_STATUS.FT_OK) break;
                    Thread.Sleep(10);
                    counter++;
                }
                myMainProg.myRtbTermMessage("-I: FTDI Device is now Closed\n");
            }
        }
        #endregion

    }
}
