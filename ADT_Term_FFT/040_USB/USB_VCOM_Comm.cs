//====================================================Propietary Software.
// USB - VCOM
// (c) Richard Payne, Varna, Bulgaria 25/11/2015
// riscy00@gmail.com
// Its use, redistribution or modification is prohibited. 
// Note: Advanced code for thread safe reciver
//       Use latest implementation solution. 
//====================================================Propietary Software.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Diagnostics;
using System.Windows.Controls;
using UDT_Term.Static;
using System.Management;

// Power Tool: insert QuickLaunch with "@tasks CollapseRegions" to close all region only. 

namespace UDT_Term_FFT
{
    public class USB_VCOM_Comm
    {
        //#####################################################################################################
        //###################################################################################### Variable/Class
        //#####################################################################################################
        ITools Tools = new Tools();
        //------------------------------------------------------Serial Port 
        private Thread VCOMreadThread;
        
        
        public SerialPort serialPort1 = null;
        
        public List<SerialPort> VCOMserialPortArray;
        private List<Thread> VCOMreadThreadArray;
        private List<bool> VCOMreadThreadArrayisStarted;

        private string VCOM_PortNumber;


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
        //###################################################################################### Constructor
        //#####################################################################################################
        public USB_VCOM_Comm()
        {
           // myGlobalBase.HMRMessage_EndOfLine = false;
            //-------------------------------------USB Array Ports (with 6 channel indexable VCOM). 
            VCOMserialPortArray = new List<SerialPort>(9)
            {
                new SerialPort(),                                     // Any tools devices
                new SerialPort(),                                     // HMR2300
                new SerialPort(),                                     // FGII_A
                new SerialPort(),                                     // FGII_B
                new SerialPort(),                                     // ADIS
                new SerialPort(),                                     // MiniAD7794
                new SerialPort(),                                     // BGDrilling (TOCO)
                new SerialPort(),                                     // Spare (None)
                new SerialPort(),                                     // Spare (None)
            };

            VCOMreadThreadArray = new List<Thread>(9)           //###TASK improve thread with backgroundworker or similar. 
            {
                new Thread(VCOMArry_Thread_ReadData0),                   // Any tools devices
                new Thread(VCOMArry_Thread_ReadData1),                   // HMR2300
                new Thread(VCOMArry_Thread_ReadData2),                   // FGII_A  
                new Thread(VCOMArry_Thread_ReadData3),                   // FGII_B
                new Thread(VCOMArry_Thread_ReadData4),                   // ADIS
                new Thread(VCOMArry_Thread_ReadData5),                   // MiniAD7794 (None)
                new Thread(VCOMArry_Thread_ReadData6),                   // BGDrilling (TOCO)
                new Thread(VCOMArry_Thread_ReadData7),                   // 
                new Thread(VCOMArry_Thread_ReadData8)                    // Spare (None)                
            };
            VCOMreadThreadArrayisStarted = new List<bool>(9)
            {
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false
            };

            /* The above is tied in to Globalbase list,make sure it sync!
                new USBVCOMArray((int)eUSBDeviceType.Generic,57600),        // Any tools devices
                new USBVCOMArray((int)eUSBDeviceType.HMR2300,19200),
                new USBVCOMArray((int)eUSBDeviceType.FGII_A,57600),         // Indian Plate DYI fluxgate. 
                new USBVCOMArray((int)eUSBDeviceType.FGII_B,57600),         // Coilcraft fluxgate 
                new USBVCOMArray((int)eUSBDeviceType.ADIS,115200),          // MiniPort + ADIS16460, (###TASK: move to MiniAD7794)
                new USBVCOMArray((int)eUSBDeviceType.MiniAD7794,3456000),   // MiniAD7794
                new USBVCOMArray((int)eUSBDeviceType.Spare6,3456000),
                new USBVCOMArray((int)eUSBDeviceType.Spare7,3456000),
                new USBVCOMArray((int)eUSBDeviceType.Spare8,3456000)
                */
        }
        //#####################################################################################################
        //###################################################################################### VCOM Terminal (Non Array)
        //#####################################################################################################

        #region //==================================================VCOM_Message_ClosePort
        //==========================================================
        // Purpose  : Close Port, this is working solution. It also dispose port object.
        // Input    :  
        // Output   : 
        //          : 
        //==========================================================
        public void VCOM_Message_ClosePort()
        {
            if (serialPort1 == null)
                return;
            if (serialPort1.IsOpen)
            {
                //
                try
                {
                    serialPort1.DiscardInBuffer();
                    serialPort1.DiscardOutBuffer();
                    serialPort1.Close();
                    VCOMreadThread.Join();      // This stop and dispose thread. (concern with reliability). 
                }
                #pragma warning disable 0168
                catch (Exception ex)
                #pragma warning restore 0168
                {
                    //throw new Exception("Failed to close serial port", ex);
                }
                if (serialPort1 !=null)
                {
                    serialPort1.Dispose();
                }
                myMainProg.myRtbTermMessageLF("-INFO: VCOM Port:"+ VCOM_PortNumber+" is now Closed");
            }
        }
        #endregion

        #region //==================================================VCOM_Start_RX_Operation
        public bool VCOM_Start_RX_Operation()
        {
            try
            {
                serialPort1.ReadTimeout = SerialPort.InfiniteTimeout;
                serialPort1.Open();
                
            }
            #pragma warning disable 0168
            catch (Exception ex)
            #pragma warning restore 0168
            {
                myMainProg.myRtbTermMessageLF("#E: Unable to open Port Name " + VCOM_PortNumber + " :- It does not exist or in use by other app");
                myMainProg.myRtbTermMessageLF("#E: VCOM Port Request is Cancelled");
                return false;
            }

            //====================================Set up separate thread for USB receives monitoring.
            VCOMreadThread = new Thread(VCOM_Thread_ReadData);
            VCOMreadThread.Start();
            myMainProg.myRtbTermMessageLF("-I: Port Name:" + VCOM_PortNumber + " is now Open");
            return true;
        }
        #endregion

        #region //==================================================VCOM_StartThread_RX_Operation
        public void VCOM_StartThread_RX_Operation()
        {
            VCOMreadThread = new Thread(VCOM_Thread_ReadData);
            VCOMreadThread.Start();
        }
        #endregion

        #region //==================================================VCOM_Validate_Encoding
        public bool VCOM_Validate_Encoding()
        {
            var encode = System.Text.Encoding.Default;
            if (!encode.BodyName.Contains("8859"))
            {
                myMainProg.myRtbTermMessageLF("#W: >>>>>> The default encoding must be <iso-8859-1> or <Window 1252> type, please check your PC setting <<<<<<");
                myMainProg.myRtbTermMessageLF("#W: >>>>>> This will impact hybrid data transfer in ESM project. Current Encoding: " + encode.BodyName.ToString()+" <<<<<<");
                return false;
            }
            else
            {
                myMainProg.myRtbTermMessageLF("-I: Default Encoding is :"+ encode.BodyName.ToString());
                return true;
            }

        }
        #endregion

        #region //==================================================VCOM_Thread_ReadData
        // Note: Frame and Page extend to hybrid frame so we adopt the encoding to iso-889-1 default which also Window-1252. It is 8 bit which is why it work well for ESM project and TOCO as well. 
        // UTF-8 and ANSII did not work well and need more study time to make proper use of this, but it is not pure 8 bit byte rep. 
        // Make sure the PC encoding is set to iso-889-1 default for proper operation.
        private void VCOM_Thread_ReadData()
        {
            int nrOfBytesAvailable = 0;
            try
            {
                while (myGlobalBase.IsApplicationQuit == false)           // While (true), modified which make it possible to quit the VCOM constant loop monitoring. 
                {
                    if (serialPort1.BytesToRead > 0)                     // Any data in buffer?
                    {
                        //----------------------------------------------Method 1, using readline, but it remove newline (not ideal). 
                        /*
                        nrOfBytesAvailable = serialPort1.BytesToRead;
                        byte[] readData = new byte[nrOfBytesAvailable];
                        string rxdata = serialPort1.ReadLine();         // However the '\n' is not transmitted.
                        rxdata = rxdata + "\n";
                        readData = Tools.StrToByteArray(rxdata);        // build in UTF8Encoding, so that okay. 
                        */
                        //----------------------------------------------Method 2, using read so that it does not remove newline, so it passed newline to internal code for proper operation, similar to FTDI method.
                        string rxdata;
                        //------------------------------------------------------------------------------------Page by Page Mode, no filtering!
                        if (myGlobalBase.EE_isLogMemPageTransferMode == true)
                        {
                            nrOfBytesAvailable = serialPort1.BytesToRead;
                            byte[] readDatax = new byte[nrOfBytesAvailable];
                            nrOfBytesAvailable = serialPort1.Read(readDatax, 0, nrOfBytesAvailable);
                            rxdata = System.Text.Encoding.Default.GetString(readDatax);                     //<= Working solution, do not change!, all must use default encoding
                            //rxdata = Tools.ByteArrayToStrUTF8(readDatax);
                            //rxdata = Tools.ByteArrayToStr(readDatax);         //Do not use ASCII, it seem to provide 16 bit encoding (UTF16) which is not ideal.
                            myUSB_Message_Manager.ComMSG_Designate_ReadData_To_App(rxdata, readDatax);
                        }
                        //------------------------------------------------------------------------------------Frame by Frame Mode, no filtering!
                        else if (myGlobalBase.EE_isLogMemFrameTransferMode == true)
                        {
                            var encode = System.Text.Encoding.Default;                                      //This encoding is iso-889-1 default which also Window-1252. IT is 8 bit which is why it work well. 
                            nrOfBytesAvailable = serialPort1.BytesToRead;
                            byte[] readDatax = new byte[nrOfBytesAvailable];
                            nrOfBytesAvailable = serialPort1.Read(readDatax, 0, nrOfBytesAvailable);
                            rxdata = System.Text.Encoding.Default.GetString(readDatax);                     //<= Working solution, do not change!, all must use default encoding
                            //rxdata = Tools.ByteArrayToStrUTF8(readDatax);
                            //rxdata = Tools.ByteArrayToStrASCII(readDatax);        //Do not use ASCII, it seem to provide 16 bit encoding (UTF16) which is not ideal. 
                            myUSB_Message_Manager.ComMSG_Designate_ReadData_To_App(rxdata, readDatax);
                        }
                        else
                        {
                            byte[] readData = new byte[Constants.twelvebitAddress];   // 2^12=4096
                            byte[] readDatax = new byte[Constants.twelvebitAddress];
                            nrOfBytesAvailable = serialPort1.BytesToRead;
                            nrOfBytesAvailable = serialPort1.Read(readDatax, 0, nrOfBytesAvailable);
                            //----------------------------Remove null bytes that creep in. 
                            int j = 0;
                            for (int i = 0; i < 4096; i++)
                            {
                                if (myGlobalBase.IsHexDisplayEnable == false)
                                {
                                    if (readDatax[i] != '\0')
                                    {
                                        readData[j] = readDatax[i];
                                        j++;
                                    }
                                }
                                else  // Under hex mode, should not delete the 00 byte!!. 
                                {
                                    readData[j] = readDatax[i];
                                    j++;
                                }
                            }
                            //---------------------------byte to string. 
                            rxdata = Tools.ByteArrayToStrASCII(readData);       // Under hex mode, should not delete the 00 byte!!. 
                            if (myGlobalBase.IsHexDisplayEnable==false)
                                rxdata = rxdata.Replace("\0", string.Empty);    //78JC should not be deleted!
                            myUSB_Message_Manager.ComMSG_Designate_ReadData_To_App(rxdata, readData);
                        }
                    }
                    Thread.Sleep(10);                                   // Sleep background.
                }
            }
            catch { }
        }

        #endregion

        #region //==================================================VCOM_SerialInit
        // Purpose: Init VCOM setting (default)
        // Input:   VCom_PortName = "COM10" etc.
        // Output:  
        // Note:    
        // ==================================================
        public void VCOM_SerialInit(string VCom_PortName)
        {
 
            if (serialPort1 == null)            // Create new SerialPort object
            {
                serialPort1 = new SerialPort();
            }
            if (serialPort1.IsOpen == true)     // Close Port if open 
            {
                VCOM_Message_ClosePort();       
                serialPort1 = new SerialPort(); // Create new SerialPort object
            }
            //this.serialPort1.RtsEnable = true;
            //-----------------------------------------Now Setup up Default Port Setting. 
            VCOM_PortNumber = VCom_PortName;
            serialPort1.PortName = VCom_PortName;
            if (myGlobalBase.isUser_Baudrate==false)
                serialPort1.BaudRate = (Int32)myGlobalBase.iBaudRateList[myGlobalBase.Select_Baudrate];
            else
                serialPort1.BaudRate = (Int32)myGlobalBase.Select_Baudrate;
            serialPort1.DataBits = 8;
            serialPort1.Parity = Parity.None;
            serialPort1.Handshake = Handshake.None;
            serialPort1.ReadTimeout = 2000;
            serialPort1.NewLine = "\n";             // only for Readline() function which not used.
            serialPort1.WriteTimeout = 500;
            serialPort1.ReadTimeout = 500;
        }
        #endregion

        #region //==================================================VCOM_Message_Send_DMFP
        //==========================================================
        // Purpose  : Send Message under DMFP protocol
        // Input    : 
        // Output   : 
        //==========================================================

        public void VCOM_Message_Send_DMFP(string sTX)
        {
            if (myGlobalBase.EE_isDMFP_Mode_Enabled == false)
            {
                myMainProg.myRtbTermMessageLF("-W: <EE_isDMFP_Mode_Enabled flag> is false in VCOM section, check code!");
            }
            //----------------------------------------------- EEPROM Export Mode to transfer data into LPC1549 EEPROM or similar device. The space is preserved.
            //----------------------------------------------------------------------------<UDT1970>
            if (sTX.Contains("{UDT1970}"))
            {
                UInt32 TimeStampNow = Tools.uDateTimeToUnixTimestampNowLocal();
                string sTimeStampNow = TimeStampNow.ToString("X");

                sTimeStampNow = "0x" + sTimeStampNow;
                sTX=sTX.Replace("{UDT1970}", sTimeStampNow);
            }
            try
            {

                //sTX = sTX + "\n";                                   // Do not send "\r\n" as this screw up LPC1549 ring buffer (need debugging there). 
                //------------------------------------------------------Transmits
                serialPort1.Write(sTX);
                Trace.WriteLine("-I: (EXPORT MODE): VCOM (String) Transmits Success :'" + sTX + "'");
            }
            catch
            {
                Trace.WriteLine("#E: (EXPORT MODE): VCOM (String) Transmits Failed, have you change COM or disconnect? :'" + sTX + "'");
            }

        }
        #endregion

        #region //==================================================VCOM_Message_Send
        //==========================================================
        // Purpose  : Send Message.
        // Input    : String message, please avoid the use of '\r' ASCII.
        // Output   : 
        //          : Important Note, use string to send data or hex <> method. 
        //          : In case of using '\r', ';', ',', '(', ')' in string message move it to other ASCII map ie 0x10 onward. See EEPROMExportImport section.
        //          : within EEPROM_ButtonClicked_ExportDataNow().
        //          : The  "sTX = sTX.Replace(" ", "");" will impact the space in message, so need to move space to other ASCII map as well. 
        //==========================================================
        public void VCOM_Message_Send(string sTX)
        {
            //---------------------------------------------------Standard VCOM 

            if (serialPort1 == null)
            {
                myMainProg.myRtbTermMessageLF("#E: VCOM Device is not established");
                return;
            }
            if (serialPort1.IsOpen == false)
            {
                myMainProg.myRtbTermMessageLF("#E: VCOM Device is not established");
                return;
            }
            /*
            if (sTX == null)
            {
                myRtbTermMessage("#E: Command Message (sTX) is Null\n");
                return;
            }
            */
            //----------------------------------------------------------------------------<UDT1970>
            if (sTX.Contains("{UDT1970}"))
            {
                UInt32 TimeStampNow = Tools.uDateTimeToUnixTimestampNowLocal();
                string sTimeStampNow = TimeStampNow.ToString("X");

                sTimeStampNow = "0x" + sTimeStampNow;
                sTX = sTX.Replace("{UDT1970}", sTimeStampNow);
            }
            //-----------------------------------------------Hex Send Message Mode <32><45><453F><89AF>
            if (sTX.Contains('<') & sTX.Contains('>'))                  // Check for Hex Display Mode
            {
                sTX = sTX.Replace("<<", "");
                sTX = sTX.Replace(">>", "");
                sTX = sTX.Replace("<", "");                             // remove unwanted typo
                sTX = sTX.Replace(">", "");
                sTX = sTX.Replace(",", "");
                sTX = sTX.Replace(";", "");
                sTX = sTX.Replace(" ", "");

                if (Tools.IsString_Hex(sTX) == true)
                {
                    try
                    {
                        byte[] bHex;
                        //------------------------------------------Convert Hex ASCII into byte and then back to string in hex data ie "3F" => 0x3F
                        if (myGlobalBase.IsHexSendEnableWithLF == true)
                            bHex = Tools.StrToHexBytesNonASCIILF(sTX);
                        else
                            bHex = Tools.StrToHexBytesNonASCII(sTX);
                        if (myGlobalBase.bHexFormatModBusCRC == true)   // if enabled, add two byte CRC based on Modbus RTU CRC16
                        {
                            UInt16 crc = Tools.ModBus_RTU_CRC_calculate(bHex, bHex.Length);
                            byte[] crcb = BitConverter.GetBytes(crc);
                            byte[] bHex1 = Tools.ByteAddByteToArray(bHex, crcb[0]);
                            byte[] bHex2 = Tools.ByteAddByteToArray(bHex1, crcb[1]);
                            serialPort1.Write(bHex2, 0, bHex2.Length);
                            Trace.WriteLine("-INFO : Serial <HEX> Transmits Success :'" + BitConverter.ToString(bHex2) + "'");
                        }
                        else
                        {
                            //------------------------------------------------------Transmits
                            serialPort1.Write(bHex, 0, bHex.Length);
                            Trace.WriteLine("-INFO : VCOM <HEX> Transmits Success :'" + sTX + "'");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        while (true) ;
                    }
                }
                //-----------------------------------------------ASCII Send Message Mode. 
                else
                {
                    myMainProg.myRtbTermMessageLF("#E: VCOM: HEX frame via '<' & '>' failed, check typo, ie '<03><1A><6AB2>' (8 bit or 16 bits) and enter");
                }
            }
            else
            {
                try
                {
                    if (sTX.Contains("\\r"))         // Special postfix to signify \r 
                    {
                        sTX = sTX.Replace("\\r", "\r");
                        serialPort1.Write(sTX);
                        Trace.WriteLine("-I: VCOM (String) Transmits Success :'" + sTX + "'");
                    }
                    else
                    {
                        sTX = sTX.Replace("\r", "");              // 5B: Remove '\r' as it cause problem in MCU operation. 
                        if (sTX.Contains("\n") == false)
                            sTX = sTX + "\n";                     // 5B: 5/2/17 RGP: Change from \r\n to \n only (no need for \r). 
                        if (sTX != " ")                           // One certain Space char to cease certain activity for BG Directional.  
                            sTX = sTX.Replace(" ", "");           // remove white space, not needed                                               
                        serialPort1.Write(sTX);
                        Trace.WriteLine("-I: VCOM (String) Transmits Success :'" + sTX + "'");
                    }
                }
                catch
                {
                    Trace.WriteLine("#E: VCOM (String) Transmits Failed, have you change COM or disconnect? :'" + sTX + "'");
                }

            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### VCOM Array Device. 
        //#####################################################################################################

        #region //==================================================VCOMArray_Message_ClosePort
        //==========================================================
        // Purpose  : Close Port, this is working solution. It also dispose port object.
        // Input    : Select type to close using  enum eUSBDeviceType from Global.
        // Output   : true = success, false = failure or not active port to close. 
        //          : 
        //==========================================================
        public bool VCOMArray_Message_ClosePort(int eSelectType)
        {
            if (VCOMserialPortArray[eSelectType] == null)                           // Already closed or not open. 
                return false;
            if (myGlobalBase.myUSBVCOMArray[eSelectType].isSerial_Server_Connected == false)     // Disabled feature, assumed closed by previous code. 
                return false;

            if (VCOMserialPortArray[eSelectType].IsOpen)
            {
                //
                try
                {
                    VCOMserialPortArray[eSelectType].DiscardInBuffer();
                    VCOMserialPortArray[eSelectType].DiscardOutBuffer();
                    VCOMserialPortArray[eSelectType].Close();
                    //-------------------------------------------
                    // We cannot use System.Timers.Timer since it slow down quite a lot, not suitbale for <100mSec.
                    // We cannot use System.Windows.Timer since it hang up the UI for some reason. 
                    // The best course of action is keep this thread going once started and have flag control to excutes data collection. 
                    // VCOMreadThreadArray[eSelectType].Join();      // This stop and dispose thread. (concern with reliability). ###TASK : need rework for better reliability. 
                }
                #pragma warning disable 0168
                catch (Exception ex)
                #pragma warning restore 0168
                {
                    //throw new Exception("Failed to close serial port", ex);
                }
                if (VCOMserialPortArray[eSelectType] != null)
                {
                    VCOMserialPortArray[eSelectType].Dispose();
                }
                myMainProg.myRtbTermMessageLF("-I: Array Channel Number: "+eSelectType.ToString() + " | VCOM Port: " + myGlobalBase.myUSBVCOMArray[eSelectType].ComPort + " is now Closed");
            }
            return true;
        }
        #endregion

        #region //==================================================VCOMArray_Start_RX_Operation
        //==========================================================
        // Purpose  : 
        // Input    : 
        // Output   : 
        //          : 
        //==========================================================
        public bool VCOMArray_Start_RX_Operation(int eSelectType)
        {
            if (VCOMserialPortArray[eSelectType] == null)
            {
                myMainProg.myRtbTermMessageLF("ER: Serial Port detected a null where not supose to!, (VCOMArray_Start_RX_Operation())");
                return false;
            }
            try
            {
                if (VCOMserialPortArray[eSelectType].IsOpen == false)
                {
                    VCOMserialPortArray[eSelectType].ReadTimeout = SerialPort.InfiniteTimeout;
                    VCOMserialPortArray[eSelectType].Open();
                    
                }
            }
            #pragma warning disable 0168
            catch (Exception ex)
            #pragma warning restore 0168
            {
                myMainProg.myRtbTermMessageLF("#ERR: Unable to open SerialArray Port Name " + myGlobalBase.myUSBVCOMArray[eSelectType].ComPort + " :- It does not exist or in use by other app");
                myMainProg.myRtbTermMessageLF("#ERR: VCOM Port Request is Cancelled");
                return false;
            }

            //====================================Set up separate thread for USB receives monitoring.
            myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp = "";
            myGlobalBase.myUSBVCOMArray[eSelectType].isEndOfLine = false;
            myGlobalBase.myUSBVCOMArray[eSelectType].isSerial_Server_Connected = true;
            myMainProg.myRtbTermMessageLF("-INFO: SerialArray Port Name:" + myGlobalBase.myUSBVCOMArray[eSelectType].ComPort + " is now Open");
            if (VCOMreadThreadArrayisStarted[eSelectType] == false)
            {
                VCOMreadThreadArray[eSelectType].Start();
                VCOMreadThreadArrayisStarted[eSelectType] = true;
            }

            return true;
        }
        #endregion

        #region //==================================================VCOMArry_Thread_ReadData0
        private void VCOMArry_Thread_ReadData0()
        {
            int nrOfBytesAvailable = 0;
            try
            {
                while (myGlobalBase.myUSBVCOMArray[0].IsApplicationQuit == false)           // While (true), modified which make it possible to quit the VCOM constant loop monitoring. 
                {
                    if (VCOMserialPortArray[0].IsOpen == true)
                    {
                        if (VCOMserialPortArray[0].BytesToRead > 0) // Any data in buffer?
                        {
                            //----------------------------------------------Method 2, using read so that it does not remove newline, so it passed newline to internal code for proper operation, similar to FTDI method.
                            string rxdata;
                            byte[] readData = new byte[256];
                            byte[] readDatax = new byte[256];
                            nrOfBytesAvailable = VCOMserialPortArray[0].BytesToRead;
                            VCOMserialPortArray[0].Read(readDatax, 0, nrOfBytesAvailable);
                            //----------------------------Remove null bytes that creep in. 
                            int j = 0;
                            for (int i = 0; i < 256; i++)
                            {
                                if (readDatax[i] != '\0')
                                {
                                    readData[j] = readDatax[i];
                                    j++;
                                }
                            }
                            //---------------------------byte to string. 
                            rxdata = Tools.ByteArrayToStrASCII(readData);
                            rxdata = rxdata.Replace("\0", string.Empty);
                            VCOMArray_AddData(rxdata, ref readData, 0);
                            // Both string and byte[] are transferred to avoid Unicode issue 
                        }
                        
                    }
                    Thread.Sleep(10); // Sleep background.
                }
            }
            catch { }
        }
        #endregion

        #region //==================================================VCOMArry_Thread_ReadData1: HMR2300. 
        private void VCOMArry_Thread_ReadData1()
        {
            int nrOfBytesAvailable = 0;
            try
            {
                while (myGlobalBase.myUSBVCOMArray[1].IsApplicationQuit == false)           // While (true), modified which make it possible to quit the VCOM constant loop monitoring. 
                {
                    if (VCOMserialPortArray[1].IsOpen == true)
                    {
                        if (VCOMserialPortArray[1].BytesToRead > 0) // Any data in buffer?
                        {
                            //----------------------------------------------Method 2, using read so that it does not remove newline, so it passed newline to internal code for proper operation, similar to FTDI method.
                            string rxdata;
                            byte[] readData = new byte[256];
                            byte[] readDatax = new byte[256];
                            nrOfBytesAvailable = VCOMserialPortArray[1].BytesToRead;
                            VCOMserialPortArray[1].Read(readDatax, 0, nrOfBytesAvailable);
                            //----------------------------Remove null bytes that creep in. 
                            int j = 0;
                            for (int i = 0; i < 256; i++)
                            {
                                if (readDatax[i] != '\0')
                                {
                                    readData[j] = readDatax[i];
                                    j++;
                                }
                            }
                            //---------------------------byte to string. 
                            rxdata = Tools.ByteArrayToStrASCII(readData);
                            rxdata = rxdata.Replace("\0", string.Empty);
                            VCOMArray_AddData(rxdata, ref readData, 1);
                                // Both string and byte[] are transferred to avoid Unicode issue 
                        }
                    }
                    
                }
                Thread.Sleep(10);                                   // Sleep background.
            }
            catch { }
        }

        #endregion

        #region //==================================================VCOMArry_Thread_ReadData2
        private void VCOMArry_Thread_ReadData2()
        {
            int nrOfBytesAvailable = 0;
            try
            {
                while (myGlobalBase.myUSBVCOMArray[2].IsApplicationQuit == false)   // While (true), modified which make it possible to quit the VCOM constant loop monitoring. 
                {
                    if (VCOMserialPortArray[2].IsOpen == true)
                    {
                        if (VCOMserialPortArray[2].BytesToRead > 0) // Any data in buffer?
                        {
                            //----------------------------------------------Method 2, using read so that it does not remove newline, so it passed newline to internal code for proper operation, similar to FTDI method.
                            string rxdata;
                            byte[] readData = new byte[256];
                            byte[] readDatax = new byte[256];
                            nrOfBytesAvailable = VCOMserialPortArray[2].BytesToRead;
                            VCOMserialPortArray[2].Read(readDatax, 0, nrOfBytesAvailable);
                            //----------------------------Remove null bytes that creep in. 
                            int j = 0;
                            for (int i = 0; i < 256; i++)
                            {
                                if (readDatax[i] != '\0')
                                {
                                    readData[j] = readDatax[i];
                                    j++;
                                }
                            }
                            //---------------------------byte to string. 
                            rxdata = Tools.ByteArrayToStrASCII(readData);
                            rxdata = rxdata.Replace("\0", string.Empty);
                            VCOMArray_AddData(rxdata, ref readData, 2);
                                    // Both string and byte[] are transferred to avoid Unicode issue 
                        }
                    }
                    Thread.Sleep(10); // Sleep background.
                }
            }
            catch { }
        }

        #endregion

        #region //==================================================VCOMArry_Thread_ReadData3
        private void VCOMArry_Thread_ReadData3()
        {
            int nrOfBytesAvailable = 0;
            try
            {
                while (myGlobalBase.myUSBVCOMArray[3].IsApplicationQuit == false)   // While (true), modified which make it possible to quit the VCOM constant loop monitoring. 
                {
                    if (VCOMserialPortArray[3].IsOpen == true)
                    {
                        if (VCOMserialPortArray[3].BytesToRead > 0) // Any data in buffer?
                        {
                            //----------------------------------------------Method 2, using read so that it does not remove newline, so it passed newline to internal code for proper operation, similar to FTDI method.
                            string rxdata;
                            byte[] readData = new byte[4096];
                            byte[] readDatax = new byte[4096];
                            nrOfBytesAvailable = VCOMserialPortArray[3].BytesToRead;
                            VCOMserialPortArray[3].Read(readDatax, 0, nrOfBytesAvailable);
                            //----------------------------Remove null bytes that creep in. 
                            int j = 0;
                            for (int i = 0; i < 4096; i++)
                            {
                                if (readDatax[i] != '\0')
                                {
                                    readData[j] = readDatax[i];
                                    j++;
                                }
                            }
                            //---------------------------byte to string. 
                            rxdata = Tools.ByteArrayToStrASCII(readData);
                            rxdata = rxdata.Replace("\0", string.Empty);
                            VCOMArray_AddData(rxdata, ref readData, 3);
                                    // Both string and byte[] are transferred to avoid Unicode issue 
                        }
                    }
                    Thread.Sleep(10); // Sleep background.
                }
            }
            catch { }
        }

        #endregion

        #region //==================================================VCOMArry_Thread_ReadData4   (ADIS Only)
        private void VCOMArry_Thread_ReadData4()
        {
            int nrOfBytesAvailable = 0;
            try
            {
                while (myGlobalBase.myUSBVCOMArray[4].IsApplicationQuit == false)   // While (true), modified which make it possible to quit the VCOM constant loop monitoring. 
                {
                    if (VCOMserialPortArray[4].IsOpen == true)
                    {
                        if (VCOMserialPortArray[4].BytesToRead > 0) // Any data in buffer?
                        {
                            //----------------------------------------------Method 2, using read so that it does not remove newline, so it passed newline to internal code for proper operation, similar to FTDI method.

                            string rxdata;
                            nrOfBytesAvailable = VCOMserialPortArray[4].BytesToRead;
                            byte[] readData = new byte[nrOfBytesAvailable+8];
                            byte[] readDatax = new byte[nrOfBytesAvailable+8];
                            
                            VCOMserialPortArray[4].Read(readDatax, 0, nrOfBytesAvailable);
                            //----------------------------Remove null bytes that creep in. 
                            int j = 0;
                            for (int i = 0; i < nrOfBytesAvailable + 8; i++)
                            {
                                if (readDatax[i] != '\0')
                                {
                                    readData[j] = readDatax[i];
                                    j++;
                                }
                            }
                            //---------------------------byte to string. 
                            rxdata = Tools.ByteArrayToStrASCII(readData);
                            rxdata = rxdata.Replace("\0", string.Empty);

                            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].isDMFProtocolEnabled == true)
                                VCOMArray_AddData_DMFPMode(rxdata, ref readData, (int)eUSBDeviceType.ADIS);
                            else
                                VCOMArray_AddData(rxdata, ref readData, (int)eUSBDeviceType.ADIS);
                            // Both string and byte[] are transferred to avoid Unicode issue 
                        }
                    }
                    Thread.Sleep(10); // Sleep background.
                }
            }
            catch { }
        }

        #endregion

        #region //==================================================VCOMArry_Thread_ReadData5
        private void VCOMArry_Thread_ReadData5()
        {
            int nrOfBytesAvailable = 0;
            try
            {
                while (myGlobalBase.myUSBVCOMArray[5].IsApplicationQuit == false)           // While (true), modified which make it possible to quit the VCOM constant loop monitoring. 
                {
                    if (VCOMserialPortArray[5].IsOpen == true)
                    {
                        if (VCOMserialPortArray[5].BytesToRead > 0) // Any data in buffer?
                        {
                            //----------------------------------------------Method 2, using read so that it does not remove newline, so it passed newline to internal code for proper operation, similar to FTDI method.
                            string rxdata;
                            byte[] readData = new byte[Constants.twelvebitAddress]; // 2^12=4096
                            byte[] readDatax = new byte[Constants.twelvebitAddress];
                            nrOfBytesAvailable = VCOMserialPortArray[5].BytesToRead;
                            nrOfBytesAvailable = VCOMserialPortArray[5].Read(readDatax, 0, nrOfBytesAvailable);
                            //----------------------------Remove null bytes that creep in. 
                            int j = 0;
                            for (int i = 0; i < Constants.twelvebitAddress; i++)
                            {
                                if (readDatax[i] != '\0')
                                {
                                    readData[j] = readDatax[i];
                                    j++;
                                }
                            }
                            //---------------------------byte to string. 
                            rxdata = Tools.ByteArrayToStrASCII(readData);
                            rxdata = rxdata.Replace("\0", string.Empty);
                            if (myGlobalBase.myUSBVCOMArray[5].isDMFProtocolEnabled == true)
                                VCOMArray_AddData_DMFPMode(rxdata, ref readData, 5);
                            else
                                VCOMArray_AddData(rxdata, ref readData, 5);
                                    // Both string and byte[] are transferred to avoid Unicode issue 
                        }
                    }
                    Thread.Sleep(10); // Sleep background.
                }
            }
            catch { }
        }

        #endregion

        #region //==================================================VCOMArry_Thread_ReadData6 (BGDRILLING Only)
        private void VCOMArry_Thread_ReadData6()
        {
            int nrOfBytesAvailable = 0;
            try
            {
                while (myGlobalBase.myUSBVCOMArray[6].IsApplicationQuit == false)           // While (true), modified which make it possible to quit the VCOM constant loop monitoring. 
                {
                    if (VCOMserialPortArray[6].IsOpen == true)
                    {
                        if (VCOMserialPortArray[6].BytesToRead > 0) // Any data in buffer?
                        {
                            //----------------------------------------------Method 2, using read so that it does not remove newline, so it passed newline to internal code for proper operation, similar to FTDI method.
                            string rxdata;
                            byte[] readData = new byte[256];
                            byte[] readDatax = new byte[256];
                            nrOfBytesAvailable = VCOMserialPortArray[(int)eUSBDeviceType.BGDRILLING].BytesToRead;
                            VCOMserialPortArray[(int)eUSBDeviceType.BGDRILLING].Read(readDatax, 0, nrOfBytesAvailable);
                            //----------------------------Remove null bytes that creep in. 
                            int j = 0;
                            for (int i = 0; i < 256; i++)
                            {
                                if (readDatax[i] != '\0')
                                {
                                    readData[j] = readDatax[i];
                                    j++;
                                }
                            }
                            //---------------------------byte to string. 
                            rxdata = Tools.ByteArrayToStrASCII(readData);
                            if (myGlobalBase.IsHexDisplayEnable == false)           // In Hex Mode, do not delete byte!
                            {
                                rxdata = rxdata.Replace("\0", string.Empty);        // Remove '\0' that causing bug issue. 
                            }
                            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].isDMFProtocolEnabled == true)
                                VCOMArray_AddData_DMFPMode(rxdata, ref readData, (int)eUSBDeviceType.BGDRILLING);
                            else
                                VCOMArray_AddData(rxdata, ref readData, (int)eUSBDeviceType.BGDRILLING);
                        }
                    }
                    Thread.Sleep(10); // Sleep background.
                }
            }
            catch { }
        }
        #endregion

        #region //==================================================VCOMArry_Thread_ReadData7
        private void VCOMArry_Thread_ReadData7()
        {
            int nrOfBytesAvailable = 0;
            try
            {
                while (myGlobalBase.myUSBVCOMArray[7].IsApplicationQuit == false)           // While (true), modified which make it possible to quit the VCOM constant loop monitoring. 
                {
                    if (VCOMserialPortArray[7].IsOpen == true)
                    {
                        if (VCOMserialPortArray[7].BytesToRead > 0) // Any data in buffer?
                        {
                            //----------------------------------------------Method 2, using read so that it does not remove newline, so it passed newline to internal code for proper operation, similar to FTDI method.
                            string rxdata;
                            byte[] readData = new byte[256];
                            byte[] readDatax = new byte[256];
                            nrOfBytesAvailable = VCOMserialPortArray[7].BytesToRead;
                            VCOMserialPortArray[7].Read(readDatax, 0, nrOfBytesAvailable);
                            //----------------------------Remove null bytes that creep in. 
                            int j = 0;
                            for (int i = 0; i < 256; i++)
                            {
                                if (readDatax[i] != '\0')
                                {
                                    readData[j] = readDatax[i];
                                    j++;
                                }
                            }
                            //---------------------------byte to string. 
                            rxdata = Tools.ByteArrayToStrASCII(readData);
                            rxdata = rxdata.Replace("\0", string.Empty);
                            VCOMArray_AddData(rxdata, ref readData, 7);
                            // Both string and byte[] are transferred to avoid Unicode issue 
                        }
                    }
                    Thread.Sleep(10); // Sleep background.
                }
            }
            catch { }
        }
        #endregion

        #region //==================================================VCOMArry_Thread_ReadData8
        private void VCOMArry_Thread_ReadData8()
        {
            int nrOfBytesAvailable = 0;
            try
            {
                while (myGlobalBase.myUSBVCOMArray[8].IsApplicationQuit == false)           // While (true), modified which make it possible to quit the VCOM constant loop monitoring. 
                {
                    if (VCOMserialPortArray[8].IsOpen == true)
                    {
                        if (VCOMserialPortArray[8].BytesToRead > 0) // Any data in buffer?
                        {
                            //----------------------------------------------Method 2, using read so that it does not remove newline, so it passed newline to internal code for proper operation, similar to FTDI method.
                            string rxdata;
                            byte[] readData = new byte[256];
                            byte[] readDatax = new byte[256];
                            nrOfBytesAvailable = VCOMserialPortArray[8].BytesToRead;
                            VCOMserialPortArray[8].Read(readDatax, 0, nrOfBytesAvailable);
                            //----------------------------Remove null bytes that creep in. 
                            int j = 0;
                            for (int i = 0; i < 256; i++)
                            {
                                if (readDatax[i] != '\0')
                                {
                                    readData[j] = readDatax[i];
                                    j++;
                                }
                            }
                            //---------------------------byte to string. 
                            rxdata = Tools.ByteArrayToStrASCII(readData);
                            rxdata = rxdata.Replace("\0", string.Empty);
                            VCOMArray_AddData(rxdata, ref readData, 8);
                            // Both string and byte[] are transferred to avoid Unicode issue 
                        }
                    }
                    Thread.Sleep(10); // Sleep background.
                }
            }
            catch { }
        }
        #endregion

        #region //==================================================BeginInvoke: VCOMArray_AddData
        //----- Special trick by taking richtextbox reference from the other form into non-window code so that text can written directly! 
        public delegate void VCOMArrayAddDataDelegate(string RXdata, ref byte[] ByteData, int eSelectType);
        public void VCOMArray_AddData(string RXdata, ref byte[] ByteData, int eSelectType)
        {
            if (myMainProg.rtbTermfRichTextBox_Ref().InvokeRequired)
            {
                myMainProg.rtbTermfRichTextBox_Ref().BeginInvoke(new VCOMArrayAddDataDelegate(VCOMArray_AddData), new object[] { RXdata, ByteData, eSelectType });
                return;
            }
            //----- Now write text to the terminal box. 
            //-------------------------------------------
            if (eSelectType== (int)eUSBDeviceType.HMR2300)          // Special for HMR2300 since it work on '\r' not '\n'
            {
                RXdata = RXdata.Replace("\0", string.Empty);        // Remove '/0' that causing bug issue. 
                RXdata = RXdata.Replace("\r", "\n");                // replace \r with \n
            }
            else
            {
                RXdata = RXdata.Replace("\0", string.Empty);        // Remove '/0' that causing bug issue. 
                RXdata = RXdata.Replace("\r", string.Empty);        // '\r' not needed, avoid spurious message. 
            }
            myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp += RXdata;                          // Add message (like rtbTerm)
            if ((myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp.Contains("\n")))
            {
                if (myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp.Length<=2)        // less than 2 char is not message response, reject. 
                {                                                                       // Hopefully this fixed spurious \n\r string. 
                    myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp = "";
                    return;
                }
                // User option to display test or not             // Survey mode should not display text. 
                //if ((myGlobalBase.HMR_HideTextDisplay == false) & (myGlobalBase.Svy_isSurveyMode==false))
                //{
                myMainProg.myRtbTermMessage(myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp);
                //}
                myGlobalBase.myUSBVCOMArray[eSelectType].MessTempRX = myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp;
                myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp = "";
                myGlobalBase.myUSBVCOMArray[eSelectType].isEndOfLine = true;
            }
        }
        #endregion

        #region //==================================================BeginInvoke: VCOMArray_AddData_DMFPMode
        //----- Special trick by taking richtextbox reference from the other form into non-window code so that text can written directly! 
        public delegate void VCOMArrayAddDataDMFPDelegate(string RXdata, ref byte[] ByteData, int eSelectType);
        public void VCOMArray_AddData_DMFPMode(string RXdata, ref byte[] ByteData, int eSelectType)
        {
            if (myMainProg.rtbTermfRichTextBox_Ref().InvokeRequired)
            {
                myMainProg.rtbTermfRichTextBox_Ref().BeginInvoke(new VCOMArrayAddDataDMFPDelegate(VCOMArray_AddData_DMFPMode), new object[] { RXdata, ByteData, eSelectType });
                return;
            }
            if (myGlobalBase.IsHexDisplayEnable == false)           // In Hex Mode, do not delete byte!
            {
                RXdata = RXdata.Replace("\0", string.Empty);        // Remove '\0' that causing bug issue. 
                RXdata = RXdata.Replace("\r", string.Empty);        // '\r' not needed, avoid spurious message.
            }
            //-------------------Invoke message into application including DMFP protocol. 
            myUSB_Message_Manager.ComMSG_Designate_ReadData_To_App(RXdata, ByteData);
            //-------------------For Array based message support, in case it may be handy. 
            myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp += RXdata;                          // Add message (like rtbTerm)
            if ((myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp.Contains("\n")))
            {
                if (myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp.Length <= 2)          // less than 2 char is not message response, reject. 
                {                                                                           // Hopefully this fixed spurious \n\r string. 
                    myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp = "";
                    return;
                }
                // User option to display test or not             // Survey mode should not display text. 
                //if ((myGlobalBase.HMR_HideTextDisplay == false) & (myGlobalBase.Svy_isSurveyMode==false))
                //{
                //}
                myGlobalBase.myUSBVCOMArray[eSelectType].MessTempRX = myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp;
                myGlobalBase.myUSBVCOMArray[eSelectType].MessTemp = "";
                myGlobalBase.myUSBVCOMArray[eSelectType].isEndOfLine = true;
            }
        }
        #endregion

        #region //==================================================VCOMArray_SerialInit
        // Purpose: Init VCOM setting (default)
        // Input:   VCom_PortName = "COM10" etc.
        // Output:  
        // Note:    
        // ==================================================
        public void VCOMArray_SerialInit(string VCom_PortName, int eSelectType)
        {
            if (VCOMserialPortArray[eSelectType] == null)            // Create new SerialPort object
            {
                VCOMserialPortArray[eSelectType] = new SerialPort();
            }
            if (VCOMserialPortArray[eSelectType].IsOpen == true)     // Close Port if open, then close it and make new serial. 
            {
                VCOMArray_Message_ClosePort(eSelectType);
                VCOMserialPortArray[eSelectType] = new SerialPort(); // Create new SerialPort object
            }
            //this.serialPort1.RtsEnable = true;
            //-----------------------------------------Now Setup up Default Port Setting. 
            myGlobalBase.myUSBVCOMArray[eSelectType].ComPort = VCom_PortName;
            VCOMserialPortArray[eSelectType].PortName = VCom_PortName;
            VCOMserialPortArray[eSelectType].BaudRate = myGlobalBase.myUSBVCOMArray[eSelectType].BaudRate;
            VCOMserialPortArray[eSelectType].DataBits = 8;
            VCOMserialPortArray[eSelectType].Parity = Parity.None;
            VCOMserialPortArray[eSelectType].Handshake = Handshake.None;
            VCOMserialPortArray[eSelectType].ReadTimeout = 2000;
            VCOMserialPortArray[eSelectType].NewLine = "\n";             // only for Readline() function which not used.
            VCOMserialPortArray[eSelectType].WriteTimeout = 500;
            VCOMserialPortArray[eSelectType].ReadTimeout = 500;
        }
        #endregion

        #region //==================================================VCOMArray_Message_Send_DMFP
        //==========================================================
        // Purpose  : Send  Message.
        // Input    : String message as per excel instruction, permits white space or comma as this is later removed 
        // Output   : Response message as per excel instruction. 
        //          : properties FTDI_Index is the channel number where  is found, along with FTDI_Description and FTDI_Device_Type.
        //==========================================================
        public void VCOMArray_Message_Send_DMFP(string sTX, int eSelectType)
        {
            if (VCOMreadThreadArray[eSelectType].ThreadState == System.Threading.ThreadState.Stopped)
            {
                myMainProg.myRtbTermMessageLF("#E: USB: VCOMreadThreadArray[eSelectType].ThreadState has been Stopped or Error, unable to receives RX message");
                return;
            }
            if (myGlobalBase.myUSBVCOMArray[eSelectType].isDMFProtocolEnabled == false)
            {
                myMainProg.myRtbTermMessageLF("-W: <myUSBVCOMArray[eSelectType].isDMFProtocolEnabled> is false in VCOM section, check code!");
            }
            if (VCOMserialPortArray[eSelectType] == null)
            {
                myMainProg.myRtbTermMessageLF("#E: Selected SerialArray VCOM Device is not established");
                return;
            }
            if (VCOMserialPortArray[eSelectType].IsOpen == false)
            {
                myMainProg.myRtbTermMessageLF("#E: Selected SerialArray VCOM Device is not established");
                return;
            }
            //----------------------------------------------------------------------------<UDT1970>
            if (sTX.Contains("{UDT1970}"))
            {
                UInt32 TimeStampNow = Tools.uDateTimeToUnixTimestampNowLocal();
                string sTimeStampNow = TimeStampNow.ToString("X");

                sTimeStampNow = "0x" + sTimeStampNow;
                sTX = sTX.Replace("{UDT1970}", sTimeStampNow);
            }
            try
            {
                myUSB_Message_Manager.MessageTX = sTX;                                          // For reference only.                                                  
                VCOMserialPortArray[eSelectType].Write(sTX);            //------------------------------------------------------Transmits
                Trace.WriteLine("-I: Selected SerialArray VCOM (String) Transmits Success :'" + sTX + "'");
            }
            catch
            {
                Trace.WriteLine("#E: Selected SerialArray VCOM (String) Transmits Failed, have you change COM or disconnect? :'" + sTX + "'");
            }
        }
        #endregion

        #region //==================================================VCOMArray_Is_Any_Open
        //==========================================================
        // Purpose  : Send  Message.
        // Input    : String message as per excel instruction, permits white space or comma as this is later removed 
        // Output   : Response message as per excel instruction. 
        //          : properties FTDI_Index is the channel number where  is found, along with FTDI_Description and FTDI_Device_Type.
        //==========================================================
        public int VCOMArray_Is_Any_Open()
        {
            int eSelectType;

            for (eSelectType = 0; eSelectType < VCOMserialPortArray.Count; eSelectType++)
            {
                if (VCOMserialPortArray[eSelectType].IsOpen == true)
                {
                    return (eSelectType);
                }
            }
            return (-1);        // Not Found.
        }
        #endregion

        #region //==================================================VCOMArray_Message_Send(string sTX, int eSelectType)....Preferred Method.
        //==========================================================
        // Purpose  : Send  Message. Perferred method, use this instead of VCOM_Message_Array_Send(string sTX, int eSelectType)
        // Input    : String message as per excel instruction, permits white space or comma as this is later removed 
        // Output   : Response message as per excel instruction. 
        //          : properties FTDI_Index is the channel number where  is found, along with FTDI_Description and FTDI_Device_Type.
        //==========================================================
        public void VCOMArray_Message_Send(string sTX, int eSelectType)
        {
            if (VCOMreadThreadArray[eSelectType].ThreadState== System.Threading.ThreadState.Stopped)
            {
                myMainProg.myRtbTermMessageLF("#E: USB: VCOMreadThreadArray[eSelectType].ThreadState has been Stopped or Error, unable to receives RX message");
            }
            if (VCOMserialPortArray[eSelectType] == null)
            {
                myMainProg.myRtbTermMessageLF("#E: Selected SerialArray VCOM Device is not established");
                return;
            }
            if (VCOMserialPortArray[eSelectType].IsOpen == false)
            {
                myMainProg.myRtbTermMessageLF("#E: Selected SerialArray VCOM Device is not established");
                return;
            }
            //----------------------------------------------------------------------------<UDT1970>
            if (sTX.Contains("{UDT1970}"))
            {
                UInt32 TimeStampNow = Tools.uDateTimeToUnixTimestampNowLocal();
                string sTimeStampNow = TimeStampNow.ToString("X");

                sTimeStampNow = "0x" + sTimeStampNow;
                sTX = sTX.Replace("{UDT1970}", sTimeStampNow);
            }
            try
            {
                //----------------------------------------------------------------------------Special protocol for HMR2300 transmits commands.
                if ((eSelectType == (int)eUSBDeviceType.HMR2300) & (sTX.Contains('*')))        // HMR2300 must use '\r'!
                {
                    sTX = sTX.Replace(" ", "");                             // remove white space in LIN message (easier to read). 
                    sTX = sTX.Replace("\n", "\r");
                    if (sTX.Contains('\r')==false)
                        sTX = sTX + "\r";
                }
                else
                {
                    sTX = sTX + "\n";                                       // remove '\r' 8/3/17
                    sTX = sTX.Replace(" ", "");                             // remove white space in LIN message (easier to read). 
                }
                //----------------------------------------------------------------------------Transmits
                VCOMserialPortArray[eSelectType].Write(sTX);            
                Trace.WriteLine("-I: Selected SerialArray VCOM (String) Transmits Success :'" + sTX + "'");
            }
            catch
            {
                Trace.WriteLine("#E: Selected SerialArray VCOM (String) Transmits Failed, have you change COM or disconnect? :'" + sTX + "'");
            }
        }
        #endregion

        #region //==================================================VCOM_Message_Array_Send(string sTX, int eSelectType)
        //==========================================================
        // Purpose  : Send Message for Array VCOM (Not used much)
        // Input    : String message, please avoid the use of '\r' ASCII.
        // Output   : 
        //          : Important Note, use string to send data or hex <> method. 
        //          : In case of using '\r', ';', ',', '(', ')' in string message move it to other ASCII map ie 0x10 onward. See EEPROMExportImport section.
        //          : within EEPROM_ButtonClicked_ExportDataNow().
        //          : The  "sTX = sTX.Replace(" ", "");" will impact the space in message, so need to move space to other ASCII map as well. 
        //==========================================================
        public void VCOM_Message_Array_Send(string sTX, int eSelectType)
        {
            //---------------------------------------------------Array VCOM 
            if (VCOMserialPortArray[eSelectType].IsOpen == true)
            {
                //----------------------------------------------------------------------------78E: {UDT1970} replace with UDT1970. 
                if (sTX.Contains("{UDT1970}"))
                {
                    UInt32 TimeStampNow = Tools.uDateTimeToUnixTimestampNowLocal();
                    string sTimeStampNow = TimeStampNow.ToString("X");

                    sTimeStampNow = "0x" + sTimeStampNow;
                    sTX = sTX.Replace("{UDT1970}", sTimeStampNow);
                }
                //-----------------------------------------------Hex Send Message Mode <32><45><453F><89AF>
                if (sTX.Contains('<') & sTX.Contains('>'))                  // Check for Hex Display Mode
                {
                    sTX = sTX.Replace("<", "");                             // remove unwanted typo
                    sTX = sTX.Replace(">", "");
                    sTX = sTX.Replace(",", "");
                    sTX = sTX.Replace(";", "");
                    sTX = sTX.Replace(" ", "");
                }
                else
                {
                    try
                    {
                        //--------------------------------------------------------------------------------------
                        sTX = sTX.Replace("\r", "");              // 5B: Remove '\r' as it cause problem in MCU operation. 
                        if (sTX.Contains("\n") == false)
                            sTX = sTX + "\n";                     // 5B: 5/2/17 RGP: Change from \r\n to \n only (no need for \r). 
                        if (sTX != " ")                           // One certain Space char to cease certain activity for BG Directional.  
                            sTX = sTX.Replace(" ", "");           // remove white space, not needed    
                        //Tools.InteractivePause(TimeSpan.FromMilliseconds(10));
                        myUSB_Message_Manager.MessageTX = sTX;                                          // For reference only.
                        VCOMserialPortArray[eSelectType].Write(sTX);
                        //Tools.InteractivePause(TimeSpan.FromMilliseconds(1000));
                        Trace.WriteLine("-I: VCOM (String) on Channel:" + eSelectType.ToString() + " | Transmits Success :'" + sTX + "'");

                    }
                    catch
                    {
                        Trace.WriteLine("#E: VCOM (String) on Channel:" + eSelectType.ToString() + " | Transmits Failed, have you change COM or disconnect? :'" + sTX + "'");
                    }
                }
                return;
            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Misc
        //#####################################################################################################
        /*
        #region //==================================================myRtbTermMessage
        //==========================================================
        // Purpose  : Append message in terminal window,. 
        // Input    :  
        // Output   : 
        // Status   : 10/9/16: Added BeginInvoke to avoid thread block issue. 
        //==========================================================
        public delegate void myRtbTermMessageVCOM_StartDelegate(string Message);
        private void myRtbTermMessage(string Message)
        {
            if (myRtbTerm.InvokeRequired)
            {
                myRtbTerm.BeginInvoke(new myRtbTermMessageVCOM_StartDelegate(myRtbTermMessage), new object[] { Message });
                return;
            }
            try
            {
                if (myGlobalBase.isTermScreenHalted == false)
                {
                    myRtbTerm.SelectionFont = myGlobalBase.FontResponse;
                    myRtbTerm.SelectionColor = myGlobalBase.ColorResponse;
                    myRtbTerm.SelectionStart = myRtbTerm.TextLength;
                    myRtbTerm.ScrollToCaret();
                    myRtbTerm.Select();
                    myRtbTerm.AppendText(myGlobalBase.TermHaltMessageBuf + Message);
                    myGlobalBase.TermHaltMessageBuf = "";
                }
                else
                {
                    myGlobalBase.TermHaltMessageBuf += Message;
                }
            }
            catch { }
        }
        #endregion
        */
    }

    #region // Below is the Async Method which is new implementation in .NET 4.5, this replace the old version unreliable readline within serialport class. This is future works. 
    /*
    http://stackoverflow.com/questions/23230375/sample-serial-port-comms-code-using-async-api-in-net-4-5 
    http://w3stack.org/question/sample-serial-port-comms-code-using-async-api-in-net-4-5/
    http://codereview.stackexchange.com/questions/84319/async-serialport-wrapper
    http://stackoverflow.com/questions/24041378/c-sharp-async-serial-port-read
    https://ms-iot.github.io/content/en-US/win10/samples/SerialSample.htm
    https://social.msdn.microsoft.com/Forums/en-US/b22ed8e7-4504-460a-b489-d99c81d51866/how-would-i-go-about-making-an-async-implementation-for-extension-methods-to-the-serialport-class?forum=async
    http://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport
                    byte[] buffer = new byte[blockLimit];
                    Action kickoffRead = null;
                    kickoffRead = delegate {
                port.BaseStream.BeginRead(buffer, 0, buffer.Length, delegate (IAsyncResult ar) {
                    try {
                        int actualLength = port.BaseStream.EndRead(ar);
                    byte[] received = new byte[actualLength];
                    Buffer.BlockCopy(buffer, 0, received, 0, actualLength);
                        raiseAppSerialDataEvent(received);
                }
                    catch (IOException exc) {
                        handleAppSerialError(exc);
            }
                    kickoffRead();
                }, null);
            };
            kickoffRead();
    */
    #endregion
}
