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
using UDT_Term;
using System.Linq.Expressions;
// Power Tool: insert QuickLaunch with "@tasks CollapseRegions" to close all region only
namespace UDT_Term_FFT
{
    public class USB_Message_Manager
    {
        DMFP_BulkFrame cDMFP_BulkFrame;
        ESM_LogMem_PageMode_Transfer myESM_LogMem_PageMode;
        ESM_LogMem_FrameMode_Transfer myESM_LogMem_FrameMode;
        //---------------------------------Temp Test Code for USB Speed test.
        List<string> testlist;
        Stopwatch stopWatch;
        UInt32 testcounter;
        IntPtr rtbOEMMsg;

        //#####################################################################################################
        //###################################################################################### Message Handling
        //#####################################################################################################

        #region //==================================================Message Getter/Setter

        public string MessageTX                 { get; set; }
        public string LoggerMessageRX           { get; set; }
        public bool endoflinedetected           { get; set; }
        private string RXMessageTemp            { get; set; }
        private string m_MessageRXtempII        { get; set; }

        private string m_LoggerMessageRXtemp    { get; set; }
        public string m_LoggerMessageRXtempII  { get; set; }

        #endregion

        //#####################################################################################################
        //###################################################################################### Reference 
        //#####################################################################################################

        #region //==================================================Reference
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        FFTWindow myFFWindow;
        ImportDataCVS myImportDataCVS;
        EE_LogMemSurvey myEESurveyCVS;
        Survey mySurvey;
        LoggerCSV myLoggerCVS;
        DMFProtocol myDMFProtocol;
        EEPROMExportImport myEEPROMExpImport;           // Added Rev 53 (14 Jan 2017)
        MainProg myMainProg;

        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        public void MyLoggerCVS(LoggerCSV myLoggerCVSRef)
        {
            myLoggerCVS = myLoggerCVSRef;
        }
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }
        public void MyFFWindowBase(FFTWindow myFFWindowRef)
        {
            myFFWindow = myFFWindowRef;
        }
        public void MyImportDataCVS(ImportDataCVS myImportDataCVSRef)
        {
            myImportDataCVS = myImportDataCVSRef;
        }
        public void MyDMFProtocol(DMFProtocol myDMFProtocolRef)
        {
            myDMFProtocol = myDMFProtocolRef;
        }
        public void MySurveyCVS(EE_LogMemSurvey myEEDownloadSurveyRef)
        {
            myEESurveyCVS = myEEDownloadSurveyRef;
        }
        public void MySurveyXX(Survey mySurveyRef)
        {
            mySurvey = mySurveyRef;
        }
        public void MyEEPROMExportImport(EEPROMExportImport myEEPROMExpImportRef)
        {
            myEEPROMExpImport = myEEPROMExpImportRef;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public USB_Message_Manager()
        {
            rtbOEMMsg = new IntPtr();
            AddInsetUSBHandler();
            AddRemoveUSBHandler();
            //-------------------------USB timing test
            testcounter = 0;
            stopWatch = new Stopwatch();
            testlist  = new List<string>();
        }

        //#####################################################################################################
        //###################################################################################### Read Data Manager
        //#####################################################################################################

        #region //==================================================ComMSG_Designate_ReadData_To_App
        public void ComMSG_Designate_ReadData_To_App(string data, byte[] readData)
        {
            if (myGlobalBase.LINUX_isLinuxModeEnabled == true)
            {
                ComMSG_AddDataLinux(data, ref readData);
            }
            else
            { 
                if ((myGlobalBase.LoggerOpertaionMode == true) & (myGlobalBase.LoggerWindowVisable == true))
                {
                    ComMSG_AddDataLogger(data);
                }
                else
                {
                    ComMSG_AddData(data, ref readData);        // Both string and byte[] are transferred to avoid Unicode issue 
                }
            }
            myGlobalBase.ETH_messageReceived = false;
        }
        #endregion

        #region //==================================================BeginInvoke: ComMSG_AddData
        // History Note: 
        //--------------------------------------------------------------------------------------
        // 27/02/19|76N     Major revision and tidy up. no longer use lastindexof('\n'), use while loop with indexof for each '\n' statement.
        //                  Add page frame handling with #@#P
        //                  Upgrade discrete frame handling with #@#S, there one outstanding task to sort out with RFFDAQ: mySurvey.SurveyCVS_DataLog_RecievedData(parts[i]); 
        //                  Transfer some non window code to UDTProtocol for easier mobility.
        //                  The slave device must requires 100-500mSec delay after #@#P/#@#S for this to work correctly
        //                  General tidy up, work better with LogMem Window. 
        //                  Suitable for ESM project and older project (to be tested out). 
        // -------------------------------------------------------------------------------------
        // 04/08/18|65      Update in Rev 76, copied from UDTLITE 65, moderate upgrade which include
        //                  Handling many message with EOF (\n) which is split and processed each one.
        //                  Support Session logs
        //                  Scrapped '\r' forever, only accept '\n' from this point onward, all code to be updated.
        //                  This improve ASYNC rapid message handling without losing partially incomplete message. 
        //                  Enable and disable message to terminal depends on AsyncSessionDoNotDisplay Start string. 
        //--------------------------------------------------------------------------------------
        //string[] AsyncSessionDoNotDisplay = { "~L1(", "~L2(", "~LH(" };
        bool isMyRtbTermMessageEnabled;

        //----- Special trick by taking richtextbox reference from the other form into non-window code so that text can written directly! 

        public delegate void AddDataDelegate(string RXdata, ref byte[] ByteData);
        public void ComMSG_AddData(string RXdata, ref byte[] ByteData)
        {
            if (myMainProg.rtbTermfRichTextBox_Ref().InvokeRequired)
            {
                myMainProg.rtbTermfRichTextBox_Ref().BeginInvoke(new AddDataDelegate(ComMSG_AddData), new object[] { RXdata, ByteData });
                return;
            }

            #region //==================================================================Bulk Page Transfer Protocol #@#P and #@#E
            //-----------------------------------------------------------------------------------------------------------------------Survey Data in Page Mode (#@#P) 
            if ((myGlobalBase.EE_isLogMemPageTransferMode == true) & (myESM_LogMem_PageMode != null))
            {
                DialogSupport mbOperationError;
                if ((myESM_LogMem_PageMode.PageNumber % 50)==0)
                {
                    myMainProg.myRtbTermMessageLF("-I: Page No :" + myESM_LogMem_PageMode.PageNumber.ToString());
                }
                if (myESM_LogMem_PageMode.ProcessPageFrame(RXdata) == true)
                {
                    stopWatch.Stop();
                    myGlobalBase.EE_isLogMemPageTransferMode = false;
                    myMainProg.myRtbTermMessageLF("-I: #@#E: End Page Mode Bulk Frame.StopWatch(mSec):" + (myESM_LogMem_PageMode.StopWatchmSec).ToString() + " | NoOfPage:" + myESM_LogMem_PageMode.PageNumber.ToString());
                    myMainProg.myRtbTermMessageLF("-I:       (NB: Deducts 100mSec Pause between Page Bulk data and terminator '#@#E')");
                    myMainProg.myRtbTermMessageLF("-I:       Estimate Data Rate (bit/Sec): " + (1000 * (myESM_LogMem_PageMode.PageNumber * (256 + 17) * 8) / (myESM_LogMem_PageMode.StopWatchmSec - 100)).ToString());
                    //                                                                          ^mSec to Sec                                ^ 17 for UDT frame wrapper and 256 for page bulk transfer.
                    myMainProg.myRtbTermMessageLF("-I:       No of Page:" + myESM_LogMem_PageMode.PageNumber);

                    if (myESM_LogMem_PageMode.isBankPageNumberMissing() == true)
                    {
                        List<int> missingpage = myESM_LogMem_PageMode.GetListBankMissingPage();
                        myMainProg.myRtbTermMessage("#E:       Page is missing from the transfer:");
                        foreach (int page in missingpage)
                            myMainProg.myRtbTermMessage("0x" + page.ToString("X") + " , ");
                        myMainProg.myRtbTermMessageLF(".");
                    }
                    myMainProg.myRtbTermMessageLF(">>>Please wait while it processing page into hybrid frame<<<");
                    string status = myESM_LogMem_PageMode.DecodePageToFrame(false);
                    myMainProg.myRtbTermMessageLF(">>>Completed, now saving to file kits<<<");
                    if (status != "")
                    {
                        myMainProg.myRtbTermMessageLF("#E#E#E:" + status);
                        if (status.Contains("message exceed"))                      // Attempt to save data to file
                        {
                            mbOperationError = null;
                            mbOperationError = new DialogSupport();
                            myEESurveyCVS.LogMemRecievedPageFrameData(myESM_LogMem_PageMode.myFrameData);
                            mbOperationError.PopUpMessageBox("DecodePageToFrame() report error, possibly due to Bulk Erase not being applied. (Block Erase apply only up to 0x3FFFF)", "ESM Operation", 5, 12F);
                        }
                        myEESurveyCVS.ESMTools_CloseTransfer();
                        return;
                    }
                    myEESurveyCVS.LogMemRecievedPageFrameData(myESM_LogMem_PageMode.myFrameData);
                    myEESurveyCVS.ESMTools_CloseTransfer();
                    myMainProg.myRtbTermMessageLF(">>>File saving Completed<<<");
                }
                return;
            }
            #endregion

            #region //==================================================================Discrete Frame Transfer Protocol #@#S and #@#E
            //-----------------------------------------------------------------------------------------------------------------------Survey Data CVS Mode
            if (myGlobalBase.EE_isLogMemFrameTransferMode == true)
            {
                //mySurvey.SurveyCVS_DataLog_RecievedData(RXdata);
                if (myESM_LogMem_FrameMode.DecodeSerialToFrame(RXdata) == true)
                {
                    stopWatch.Stop();
                    myGlobalBase.EE_isLogMemFrameTransferMode = false;
                    myMainProg.myRtbTermMessageLF("-I: #@#E: End Page Mode Bulk Frame.StopWatch(mSec):" + (myESM_LogMem_FrameMode.StopWatchmSec).ToString() + " | NoOfFrame:" + myESM_LogMem_FrameMode.myFrameData.Count.ToString());
                    myMainProg.myRtbTermMessageLF("-I:       (NB: Deducts 100mSec Pause between Page Bulk data and terminator '#@#E')");
                    myEESurveyCVS.LogMemRecievedDiscreteFrameData(myESM_LogMem_FrameMode.myUDTmyFrameDataLog, myESM_LogMem_FrameMode.myFrameData);
                    myEESurveyCVS.ESMTools_CloseTransfer();
                }
                //myEESurveyCVS.SurveyCVS_RecievedData(RXdata);
                return;
            }
            #endregion

            #region //==================================================================ImportCVS_RecievedData
            //-----------------------------------------------------------------------------------------------------------------------Import Data CVS Mode
            if (myGlobalBase.IsImportRawDataActivate == true)
            {
                myImportDataCVS.ImportCVS_RecievedData(RXdata);
                return;
            }
            #endregion

            #region //==================================================================Hex & MODBUS Display Mode
            if (myGlobalBase.IsHexDisplayEnable == true)        // New code Version 1D......Hex Display Mode ie 00 F0 23 46 21 23 etc
            {
                string sData;
                if (myGlobalBase.IsHexDisplayEnableLF == false)
                {
                    if (myGlobalBase.bHexFormatModBusCRC == true)   // if enabled, add two byte CRC based on Modbus RTU CRC16
                    {
                        sData = Tools.ModBus_ByteArrayToHexStrSpace(ByteData);
                        myMainProg.myRtbTermMessage(sData);
                        sData = string.Empty;
                        if (ByteData[1]==0x3)       // Function 3 MODBUS Only. 
                        {
                            UInt16 D16;
                            int ipos = 9;
                            for (int i=0; i< ((ByteData[2]-6) / 2); i++)   //position to start of 16 bit data. 
                            {
                                sData += " | ";
                                D16 = Tools.HexBytetoUInterger16( ByteData[ipos+1], ByteData[ipos]);
                                sData += D16.ToString();
                                ipos += 2;
                            }
                            //myMainProg.myRtbTermMessageLF("-I: Hex to UINT16:"+sData);
                            myMainProg.myRtbTermMessageLF(sData);
                        }
                    }
                    else
                    {
                        sData = Tools.ByteArrayToHexStrSpace(ByteData);
                        myMainProg.myRtbTermMessage(sData);
                        myMainProg.myRtbTermMessageLF("");
                    }
                    //-------------------------------------------Clear array garbage.
                    Array.Clear(ByteData, 0, ByteData.Length);
                    //myRtbTermMessage(sData);
                }
                else
                {
                    if (myGlobalBase.bHexFormatModBusCRC == true)   // if enabled, add two byte CRC based on Modbus RTU CRC16
                    {
                        sData = Tools.ModBus_ByteArrayToHexStrSpaceLF(ByteData);
                        myMainProg.myRtbTermMessage(sData);
                        sData = string.Empty;
                        if (ByteData[1] == 0x3)       // Function 3 MODBUS Only. 
                        {
                            UInt16 D16;
                            int ipos = 9;
                            for (int i = 0; i < ((ByteData[2]-6) / 2); i++)   //position 9 is start of 16 bit data
                            {
                                sData += " | ";
                                D16 = Tools.HexBytetoUInterger16(ByteData[ipos + 1], ByteData[ipos]);
                                sData += D16.ToString();
                                ipos += 2;
                            }
                            //myMainProg.myRtbTermMessageLF("-I: Hex to UINT16:"+sData);
                            myMainProg.myRtbTermMessageLF(sData);
                        }
                    }
                    else
                    {
                        sData = Tools.ByteArrayToHexStrSpaceLF(ByteData);
                        myMainProg.myRtbTermMessage(sData);
                        myMainProg.myRtbTermMessageLF("");
                    }
                    //-------------------------------------------Clear array garbage.
                    Array.Clear(ByteData, 0, ByteData.Length);
                    //myRtbTermMessage(sData);
                }
                ConsoleAppendMessageB(sData);
                RXMessageTemp = "";
                return;
            }
            #endregion

            RXdata = RXdata.Replace("\0", string.Empty);        // Remove '\0' that causing bug issue. 
            RXdata = RXdata.Replace("\r", string.Empty);        // Remove '\r' obsolete use. 
            RXMessageTemp += RXdata;                            // Add message (like rtbTerm)
                                                                // Old method: int pos = RXMessageTemp.LastIndexOf("\n")+1;      // +1 include '\n', seek last index of '\n', not ideal for multiple response. 
            int pos = 1;
            while (true)                                        // UDTerm 76N: New method to process discrete \n statement (especially when fast). 
            {
                #region //==================================================================Check for End of Line \n.
                pos = RXMessageTemp.IndexOf('\n') + 1;          // +1 include '\n', seek 1st index of '\n'. 0 = No string                                     
                if (pos == 0)                                   // Empty string. IndexOf: The zero-based index position of value if that string is found, or -1 if it is not. If value is Empty, the return value is 0.
                {
                    endoflinedetected = false;
                    return;
                }
                if (pos == -1)                                  // '\n' Not found.
                {
                    endoflinedetected = false;
                    if (RXMessageTemp.Contains("$F") && (myGlobalBase.FFTOpertaionMode == true))
                    {
                        myMainProg.myRtbTermMessageLF(".");
                    }
                    else
                    {
                        myMainProg.myRtbTermMessage(RXdata);       //fixed bug should not be LF type!.
                    }
                    return;
                }
                #endregion
                // If not detected, quit.

                #region //==================================================================End of Line detected
                m_MessageRXtempII = RXMessageTemp.Substring(0, pos);                            // Take out 1st message to late \n. 
                RXMessageTemp = RXMessageTemp.Substring(pos, (RXMessageTemp.Length - pos));     // Remaining is goes back to RX reception.
                isMyRtbTermMessageEnabled = true;
                string[] parts = Tools.SplitAndKeepDelimiters(m_MessageRXtempII, '\n');
                #endregion

                #region //==================================================================EE Survey Page Mode Section, "#@#P"
                if (myGlobalBase.LogMemWindowVisable == true)
                {
                    string EESurveyPageMode = "#@#P"; // Pattern for Download Survey
                    //                         if (parts != null)
                    //                         {
                    //                             for (int i = 0; i < parts.Length; i++)
                    //                             {
                    isMyRtbTermMessageEnabled = true;
                    if (myGlobalBase.EE_isSurveyCVSRawLogDataActive == true                      //true
                        & myGlobalBase.EE_isLogMemFrameTransferMode == false                //false
                        & myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate == false
                        & myGlobalBase.EE_isLogMemPageTransferMode == false
                        & m_MessageRXtempII.Contains(EESurveyPageMode))
                    {
                        myMainProg.myRtbTermMessageLF("-I: #@#P: Start Page Mode Bulk Frame.......");
                        CloseOtherWindow();
                        //--------------------------------------
                        myGlobalBase.EE_isLogMemPageTransferMode = true;
                        //--------------------------------------
                        if (myESM_LogMem_PageMode != null)
                            myESM_LogMem_PageMode = null;
                        myESM_LogMem_PageMode = new ESM_LogMem_PageMode_Transfer();
                        myESM_LogMem_PageMode.StartResetPageFrameTransfer();
                        m_MessageRXtempII = "";                                     // Done message.
                    }
                    //                             }
                    //                         }

                }
                #endregion

                #region //==================================================================EE Survey CVS Section, "#@#S"
                if (myGlobalBase.LogMemWindowVisable == true)
                {
                    string EESurveyFrameMode = "#@#S"; // Pattern for Download Survey
                    //                         if (parts != null)           // Not required due to change in way '\n' is handled (switch LastIndexof to Indexof.
                    //                         {
                    //                             for (int i = 0; i < parts.Length; i++)
                    //                             {
                    isMyRtbTermMessageEnabled = true;
                    if (myGlobalBase.EE_isSurveyCVSRawLogDataActive == true              //true
                        & myGlobalBase.EE_isLogMemFrameTransferMode == false                //false
                        & myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate == false
                        & myGlobalBase.EE_isLogMemPageTransferMode == false
                        & m_MessageRXtempII.Contains(EESurveyFrameMode))
                    {
                        myMainProg.myRtbTermMessageLF("#@#S: Start Bulk Frame.......");
                        CloseOtherWindow();
                        //-------------------------
                        myGlobalBase.EE_isLogMemFrameTransferMode = true;
                        //--------------------------------------
                        if (myESM_LogMem_FrameMode != null)
                            myESM_LogMem_FrameMode = null;
                        myESM_LogMem_FrameMode = new ESM_LogMem_FrameMode_Transfer();
                        myESM_LogMem_FrameMode.StartResetFrameTransfer();
                        //--------------------------------------
                        //mySurvey.SurveyCVS_DataLog_RecievedData(parts[i]);          //Datalog transfer. <###TASK: Check if this is right??
                        //myEESurveyCVS.SurveyCVS_RecievedData(parts[i);            // should it be this?
                        m_MessageRXtempII = "";                                     // Done message.
                    }
                    //                             }
                    //                         }

                }
                #endregion

                #region //==================================================================USB Speed test code/End of line detector (disabled)
//                 if (parts != null)
//                 {
//                     for (int i = 0; i < parts.Length; i++)
//                     {
//                         if ((parts[i].Contains("#@#XX")) & (testcounter == 0))
//                         {
//                             testlist.Clear();
//                             testcounter = 1;
//                             stopWatch.Reset();
//                             stopWatch.Start();
//                             m_MessageRXtempII = "";
//                             myMainProg.myRtbTermMessage("S:" + testcounter.ToString());
//                         }
//                         if ((parts[i].Contains("$XX")) & (testcounter != 0))
//                         {
//                             testlist.Add(m_MessageRXtempII);
//                             m_MessageRXtempII = "";
//                             testcounter++;
//                             myMainProg.myRtbTermMessage(":" + testcounter.ToString());
//                         }
//                         if ((parts[i].Contains("#@#EE")) & (testcounter != 0))
//                         {
//                             stopWatch.Stop();
//                             myMainProg.myRtbTermMessage(":E");
//                             testcounter++;
//                             TimeSpan ts = stopWatch.Elapsed;
//                             myMainProg.myRtbTermMessageLF("\nCollected Frame : " + testcounter.ToString());
//                             myMainProg.myRtbTermMessageLF("StopWtach Lapsed (mSec): " + ts.TotalMilliseconds.ToString());
//                             myMainProg.myRtbTermMessageLF("StopWtach Lapsed (tick(100nSec)): " + ts.Ticks.ToString());
//                             testcounter = 0;
//                             m_MessageRXtempII = "";
//                             return;
//                         }
//                     }
//                     if (testcounter != 0)
//                         return;
//                 }
                #endregion

                #region //==================================================================Copy received message into clipboard (if enabled). 
                //if (myGlobalBase.configb[0].bOptionAutoCopyRXMessageToClipBoard == true)      //unreliable to use. 
                //    myMainProg.zClipBoard_AddString(m_MessageRXtempII);
                #endregion

                #region //==================================================================DMFP and callback Support, '-' and '~' prefix (Universal, Work for IDT/BG/ADT/TVB,etc)
                if (parts != null)
                {
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (parts[i].Contains(")"))          // Rev 65, It would be nice to detect '\n' instead of ')'
                        {
                            //-------------------------------------------
                            //-------------DMFP Received Command Section                 
                            //-------------------------------------------
                            if ((parts[i].StartsWith("-", StringComparison.Ordinal) == true))            // Detect at the start, not within (which may contain negative data)
                            {
                                isMyRtbTermMessageEnabled = true;
                                if ((parts[i].Contains("-EEIIMPORT(") == true) & (myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate == true))   //Survey Calibration CVS Mode, alternative method Rev76 3/9/18
                                {
                                    isMyRtbTermMessageEnabled = false;
                                    endoflinedetected = true;
                                }
                                else
                                {
                                    endoflinedetected = false;
                                }
                                myDMFProtocol.DMFP_Recieved_Command(parts[i]);
                                m_MessageRXtempII = "";                                     // Done message.
                                if ((isMyRtbTermMessageEnabled == true) & (myGlobalBase.bisHideCallBackChecked==false))
                                {
                                    myMainProg.myRtbTermMessage(parts[i]);
                                }
                            }
                            //--------------------------Async Command now work for all project including BG, IDT, ADT and so on. 
                            if (parts[i].StartsWith("~", StringComparison.Ordinal) == true)     // Async Message
                            {
                                myDMFProtocol.DMFP_Recieved_Async_Command(parts[i]);
                                isMyRtbTermMessageEnabled = true;
                                //-------------------------------------------------------------Session Log enable or disable terminal message. 
                                if (myGlobalBase.bSessionHideStealthTerm == true)
                                {
                                    if ((parts[i].StartsWith("~L1(", StringComparison.Ordinal) == true) || (parts[i].StartsWith("~LH(", StringComparison.Ordinal) == true)) // Stealth Log
                                    {
                                        isMyRtbTermMessageEnabled = false;
                                        //RXdata = RXdata.Replace((m_MessageRXtemp), "");
                                        m_MessageRXtempII = "";                                     // Done message.
                                    }
                                }
                                if (myGlobalBase.bSessionHideSessionTerm == true)
                                {
                                    if ((parts[i].StartsWith("~L2(", StringComparison.Ordinal) == true)) // Session Log
                                    {
                                        isMyRtbTermMessageEnabled = false;
                                        //RXdata = RXdata.Replace((m_MessageRXtemp), "");
                                        m_MessageRXtempII = "";                                     // Done message.
                                    }
                                }

                                if (isMyRtbTermMessageEnabled == true)
                                {
                                    myMainProg.myRtbTermMessage(parts[i]);
                                    //myMainProg.myRtbTermMessage(parts[i] + "\n");
                                    m_MessageRXtempII = "";
                                    endoflinedetected = true;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region //==================================================================(OLD) DMFP and callback Support, '-' and '~' prefix (Universal, Work for IDT/BG/ADT/TVB,etc)
                ////-------------------------------------------
                ////-------------DMFP Received Command Section                 
                ////-------------------------------------------
                //if ((RXMessageTemp.StartsWith("-", StringComparison.Ordinal) == true))            // Detect at the start, not within (which may contain negative data)
                //{
                //    myDMFProtocol.DMFP_Recieved_Command(RXMessageTemp);
                //}
                ////--------------------------Async Command now work for all project including BG, IDT, ADT and so on. 
                //if ((myGlobalBase.IsImportRawDataActivate == false))
                //{
                //    if (RXMessageTemp.StartsWith("~", StringComparison.Ordinal) == true)     // Async Message
                //    {
                //        //------------------------Close FFT and other unneeded window
                //        if (myFFWindow.Visible == true)
                //        {
                //            myFFWindow.Visible = false;
                //            myFFWindow.Hide();
                //        }
                //        myDMFProtocol.DMFP_Recieved_Async_Command(RXMessageTemp);
                //    }
                //}

                #endregion

                #region //==================================================================DataLog Survey CVS Section, ~LB(), ~LC(), ~LD() and ~LE()
                //-------------------------------------------
                //------------------------DataLog Survey CVS Section, "~LB(), ~LC(), ~LD() and ~LE()"
                //-------------------------------------------
                if (myGlobalBase.DataLog_isSurveyCVSOpen)
                {
                    isMyRtbTermMessageEnabled = true;
                    //if (myGlobalBase.EE_isSurveyCVSRawLogDataActive == true              //true
                    //    & myGlobalBase.EE_isLogMemFrameTransferMode == false                //false
                    //    & myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate == false)
                    {
                        //-------------------------Close FFT window to reduce number of forms in layout.   
                        if (myFFWindow.Visible == true)
                        {
                            myFFWindow.Visible = false;
                            myFFWindow.Hide();
                        }
                        //-------------------------Close ImportCVS window to reduce number of forms in layout.
                        if (myImportDataCVS.Visible == true)
                        {
                            myImportDataCVS.Visible = false;
                            myImportDataCVS.Hide();
                        }
                        //-------------------------
                        //myGlobalBase.EE_isLogMemFrameTransferMode = true;
                        //--------------------------------------
                        if (parts != null)
                        {
                            for (int i = 0; i < parts.Length; i++)
                            {
                                mySurvey.SurveyCVS_DataLog_RecievedData(parts[i]);
                            }
                        }
                    }
                    m_MessageRXtempII = "";
                }
                #endregion

                #region //==================================================================Import CVS Section, "#@#I"
                if (myGlobalBase.IsImportRawDataEnable == true & myGlobalBase.IsImportRawDataActivate == false & m_MessageRXtempII.Contains("#@#I"))
                {
                    if (parts != null)
                    {
                        for (int i = 0; i < parts.Length; i++)
                        {
                            isMyRtbTermMessageEnabled = true;
                            myMainProg.myRtbTermMessageLF("#@#I: Import CVS Mode Activated");
                            //-------------------------Close FFT window to reduce number of forms in layout.   
                            if (myFFWindow.Visible == true)
                            {
                                myFFWindow.Visible = false;
                                myFFWindow.Hide();
                            }
                            //-------------------------Close SurveyCVS window to reduce number of forms in layout.
                            if (myEESurveyCVS.Visible == true)
                            {
                                myEESurveyCVS.Visible = false;
                                myEESurveyCVS.Hide();
                            }
                            //-------------------------Open Import CVS window. 
                            myImportDataCVS.ImportCVS_StartFrameDetected();
                            if (parts[i].Length >= 6)                                 // fix array error due to #@#I\n output which should be #@#I\n\r. 
                                parts[i].Remove(0, 6);                                // Remove #@#I\r\n in case there is header/data frame inside.
                            else
                                parts[i].Remove(0, RXdata.Length);
                            myImportDataCVS.ImportCVS_RecievedData(parts[i]);
                        }
                    }
                    m_MessageRXtempII = "";                                             // Done message.
                }
                #endregion

                #region //==================================================================FFT Section, "$F"
                //-------------------------------------------
                //-------------------------FFT Section
                //-------------------------------------------
                if (myGlobalBase.IsImportRawDataActivate == false & myGlobalBase.FFTOpertaionMode == true & m_MessageRXtempII.Contains("$F"))      // BUG: Check for FFT frame, make sure not to send <ACK> after commands!!, use 0xFE to skip this.
                {
                    isMyRtbTermMessageEnabled = false;
                    if (parts != null)
                    {
                        for (int i = 0; i < parts.Length; i++)
                        {
                            //-------------------------------------------Strip away the 1st text before $F
                            string[] result = new string[5];
                            try
                            {
                                string[] stringSeparators = new string[] { "$F," };
                                result = parts[i].Split(stringSeparators, StringSplitOptions.None);
                                if (result.Length == 0)
                                {
                                    ConsoleAppendMessageB("#ERR : Bad data, raw data has missing start terminator $E");
                                    return;
                                }
                            }
                            catch (System.Exception ex)
                            {
                                ConsoleAppendMessageB("#ERR: Bad Received Data, unable to detect $F or split string. Exception:" + ex.ToString());
                            }
                            //----------------------------------1st split goes to terminal window.
                            myMainProg.myRtbTermMessage(result[0]);
                            //----------------------------------Send to FFT Window
                            myFFWindow.FFTRecievedData("$F," + result[1]);        // Update window. 
                                                                                  //m_MessageRX = m_MessageRXtemp;
                            myMainProg.myRtbTermMessage("-I:(" + DateTime.Now.ToString("hh_mm_ss") + ") Updated FFT Chart" + Environment.NewLine);
                        }
                    }
                    m_MessageRXtempII = "";
                }
                #endregion

                if ((isMyRtbTermMessageEnabled == true) & (m_MessageRXtempII != ""))
                {
                    //ConsoleAppendMessageB(RXMessageTemp);                 // Alway log message if enabled to keep timedate difference small as possible.
                    //m_MessageRX = m_MessageRXtemp;
                    myMainProg.myRtbTermMessage(m_MessageRXtempII);
                    m_MessageRXtempII = "";
                    endoflinedetected = true;                               // This invoke ASYNC Logger code to accomodate survey data here. 
                }
            }
        }
        //=====================End of this section. 
        #endregion

        #region //==================================================CloseOtherWindow
        private void CloseOtherWindow()
        {
            //-------------------------Close FFT window to reduce number of forms in layout.   
            if (myFFWindow.Visible == true)
            {
                myFFWindow.Visible = false;
                myFFWindow.Hide();
            }
            //-------------------------Close ImportCVS window to reduce number of forms in layout.
            if (myImportDataCVS.Visible == true)
            {
                myImportDataCVS.Visible = false;
                myImportDataCVS.Hide();
            }
        }
        #endregion

        #region //==================================================ConsoleAppendMessageB
        private void ConsoleAppendMessageB(string message)
        {
            if (myGlobalBase.IsLogEnable == false)
                return;
            if (message == ".")                 // No need to log "." used as timing tick.
                return;
            if (myGlobalBase.sFilename == "")  // Avoid error.
                return;
            message.Replace("\n", Environment.NewLine);
            try
            {
                string sdatetime = DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss");
                using (StreamWriter sw = File.AppendText(myGlobalBase.sFilename))           //using statement ensure to dispose and close sw.
                {
                    sw.Write(sdatetime + " ::: " + message);
                }
            }
            catch (AmbiguousMatchException)
            {
                myMainProg.myRtbTermMessage("#ERR : Unable to append message into Filename: " + myGlobalBase.sFilename + Environment.NewLine);
                myGlobalBase.IsLogEnable = false;
            }
        }
        #endregion

        #region //==================================================BeginInvoke: AddDataLinux
        //----- Special trick by taking richtextbox reference from the other form into non-window code so that text can written directly! 
        public delegate void AddDataLinuxDelegate(string RXdata, ref byte[] ByteData);
        public void ComMSG_AddDataLinux(string RXdata, ref byte[] ByteData)
        {
            System.Windows.Forms.RichTextBox rtbTerm = myMainProg.rtbTermPanelRichTextBox_Ref();
            if (myMainProg.rtbTermfRichTextBox_Ref().InvokeRequired)
            {
                myMainProg.rtbTermfRichTextBox_Ref().BeginInvoke(new AddDataLinuxDelegate(ComMSG_AddDataLinux), new object[] { RXdata, ByteData });
                return;
            }
            
            //----------------------------------------------------------------------
            try
            {
                if (myGlobalBase.isTermScreenHalted == false)
                {
                    if (rtbTerm != null)
                    {
                        Tools.rtb_StopRepaint(rtbTerm, rtbOEMMsg);
                        rtbTerm.SelectionFont = myGlobalBase.FontResponse;
                        rtbTerm.SelectionColor = myGlobalBase.ColorResponse;
                        rtbTerm.SelectionStart = rtbTerm.TextLength;
                        rtbTerm.ScrollToCaret();
                        rtbTerm.Select();
                        rtbTerm.AppendText(myGlobalBase.TermHaltMessageBuf + RXdata);
                        Tools.rtb_StartRepaint(rtbTerm, rtbOEMMsg);
                    }
                    myGlobalBase.TermHaltMessageBuf = "";
                }
                else
                {
                    myGlobalBase.TermHaltMessageBuf += RXdata;
                }
            }
            catch { }
            endoflinedetected = false;
            //----------------------------------------------------------------------
            // Below no service needed for now, something for future related to Linux terminal mode.
            // This help to speed up response. 
            //----------------------------------------------------------------------
        }
        #endregion

        #region //==================================================BeginInvoke: AddDataLogger
        //----- Special trick by taking richtextbox reference from the other form into non-window code so that text can written directly! 
        public delegate void AddDataLoggerDelegate(string RXdataLogger);
        public void ComMSG_AddDataLogger(string RXdataLogger)
        {
            System.Windows.Forms.RichTextBox myLoggerCVStrbTerm = myLoggerCVS.rtbConsoleRichTextBox_Ref();
            if (myLoggerCVStrbTerm.InvokeRequired)
            {
                myLoggerCVStrbTerm.BeginInvoke(new AddDataLoggerDelegate(ComMSG_AddDataLogger),new object[] { RXdataLogger });
                return;
            }
            //------------------------------------------------------------------------
            try
            {
                //----- Now write text to the terminal box. 
                Tools.rtb_StopRepaint(myLoggerCVStrbTerm, rtbOEMMsg);
                myLoggerCVStrbTerm.SelectionFont = myGlobalBase.FontResponse;
                myLoggerCVStrbTerm.SelectionColor = myGlobalBase.ColorResponse;
                myLoggerCVStrbTerm.SelectionStart = myLoggerCVS.rtbConsoleRichTextBox_Ref().TextLength;
                myLoggerCVStrbTerm.ScrollToCaret();
                myLoggerCVStrbTerm.Select();
                myLoggerCVStrbTerm.AppendText(RXdataLogger);
                m_LoggerMessageRXtemp += RXdataLogger;              // Add message (like rtbTerm)
                Tools.rtb_StartRepaint(myLoggerCVStrbTerm, rtbOEMMsg);
            }
            catch { }

            endoflinedetected = false;

            int pos = 1;
            while (true)                                        // UDTerm 76N: New method to process discrete \n statement (especially when fast). 
            {
                #region //==================================================================Check for End of Line \n.
                pos = m_LoggerMessageRXtemp.IndexOf('\n') + 1;          // +1 include '\n', seek 1st index of '\n'. 0 = No string                                     
                if (pos == 0)                                   // Empty string. IndexOf: The zero-based index position of value if that string is found, or -1 if it is not. If value is Empty, the return value is 0.
                {
                    endoflinedetected = false;
                    return;
                }
                if (pos == -1)                                  // '\n' Not found.
                {
                    endoflinedetected = false;
                    return;
                }
                #endregion
                // If not detected, quit.

                #region //==================================================================End of Line detected
                m_LoggerMessageRXtempII = m_LoggerMessageRXtemp.Substring(0, pos);                            // Take out 1st message to late \n. 
                m_LoggerMessageRXtemp = m_LoggerMessageRXtemp.Substring(pos, (m_LoggerMessageRXtemp.Length - pos));     // Remaining is goes back to RX reception.
                string[] parts = Tools.SplitAndKeepDelimiters(m_LoggerMessageRXtempII, '\n');
                #endregion

                if (m_LoggerMessageRXtempII.Contains("$E"))          // End of line terminator for logger message.
                {
                    if ((m_LoggerMessageRXtempII.Contains("<LOGON>") == true) | (m_LoggerMessageRXtempII.Contains("<LOGOFF>") == true))
                    {
                        Tools.rtb_StopRepaint(myLoggerCVStrbTerm, rtbOEMMsg);
                        myLoggerCVStrbTerm.AppendText("\n");
                        Tools.rtb_StartRepaint(myLoggerCVStrbTerm, rtbOEMMsg);
                        m_LoggerMessageRXtemp = "";
                    }
                    else
                    {
                        LoggerMessageRX = m_LoggerMessageRXtempII;
                        endoflinedetected = true;
                        myLoggerCVS.RecievedMessage_Async_Logger_Processing();
                    }
                }
                //============================================================ Handle DMFP Message Protocol for both SYNC and ASYNC response while in Logger Mode. 
                else
                {
                    //-------------------------------------------
                    //-------------DMFP Received Command Section                 
                    //-------------------------------------------
                    if ((m_LoggerMessageRXtempII.StartsWith("-", StringComparison.Ordinal) == true))            // Detect at the start, not within (which may contain negative data)
                    {
                        myDMFProtocol.DMFP_Recieved_Command(m_LoggerMessageRXtempII);
                    }
                    //--------------------------Async Command now work for all project including BG, IDT, ADT and so on. 
                    if (m_LoggerMessageRXtempII.StartsWith("~", StringComparison.Ordinal) == true)     // Async Message
                    {
                        myDMFProtocol.DMFP_Recieved_Async_Command(m_LoggerMessageRXtempII);
                    }
                    endoflinedetected = true;
                    myLoggerCVS.RecievedMessage_Async_Logger_Processing();
                    // LoggerMessageRX = "";
                    // m_LoggerMessageRXtemp = "";
                    
                        // RXdataLogger = "";
                }
            }
        }

        #endregion

        //#####################################################################################################
        //###################################################################################### USB Insertion Detection
        //#####################################################################################################
        // http://www.c-sharpcorner.com/blogs/detect-insertion-and-removal-of-usb-drive-c-sharp1

        #region //==================================================AddRemoveUSBHandler
        //==========================================================
        // Purpose  : 
        // Input    :  
        // Output   : This work for Window 10 and VS2017. Older code will not work
        // Status   : https://social.msdn.microsoft.com/Forums/windows/en-US/4955a6ce-2eb8-4553-bab9-6e346ce8e17e/usb-plugin-unplug-events-and-to-get-detatils-of-connected-device?forum=winforms
        //==========================================================
        public ManagementEventWatcher wUSB = null;
        public void AddRemoveUSBHandler()
        {
            WqlEventQuery q;
            ManagementScope scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true;

            try
            {
                q = new WqlEventQuery
                {
                    EventClassName = "__InstanceDeletionEvent",
                    WithinInterval = new TimeSpan(0, 0, 3),
                    Condition = "TargetInstance ISA 'Win32_USBControllerdevice'"
                };
                w = new ManagementEventWatcher(scope, q);
                w.EventArrived += USBRemoved;
                w.Start();
            }
            catch (Exception e)
            {


                Console.WriteLine(e.Message);
                if (w != null)
                {
                    w.Stop();
                }
            }
        }
        #endregion

        #region //==================================================AddInsetUSBHandler
        //==========================================================
        // Purpose  : 
        // Input    :  
        // Output   : This work for Window 10 and VS2017. Older code will not work
        // Status   : https://social.msdn.microsoft.com/Forums/windows/en-US/4955a6ce-2eb8-4553-bab9-6e346ce8e17e/usb-plugin-unplug-events-and-to-get-detatils-of-connected-device?forum=winforms
        //==========================================================
        static ManagementEventWatcher w = null;
        public void AddInsetUSBHandler()
        {
            WqlEventQuery q;
            ManagementScope scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true;

            try
            {
                q = new WqlEventQuery
                {
                    EventClassName = "__InstanceCreationEvent",
                    WithinInterval = new TimeSpan(0, 0, 3),
                    Condition = "TargetInstance ISA 'Win32_USBControllerdevice'"
                };
                w = new ManagementEventWatcher(scope, q);
                w.EventArrived += USBAdded;
                w.Start();
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                if (w != null)
                {
                    w.Stop();
                }
            }
        }
        #endregion

        #region //==================================================USBAdded
        //==========================================================
        // Purpose  : 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        public void USBAdded(object sender, EventArrivedEventArgs e)
        {
            myMainProg.myRtbTermMessageLF("\n-I: Detected USB Device Plugged in.");
            myMainProg.USB_Device_PlugIn_Detected();
        }
        #endregion

        #region //==================================================USBRemoved
        //==========================================================
        // Purpose  : 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        public void USBRemoved(object sender, EventArgs e)
        {
            myMainProg.myRtbTermMessageLF("\n-I: Detected USB Device Removed.");
            myMainProg.USB_Device_Removed_Detected();
        }
        #endregion

        //##################################################################################################### New Method



        //#####################################################################################################
        //###################################################################################### DHMF Bulk Message Transfer Protocol
        //#####################################################################################################

        #region //=============================================================================================DMFProtocol_BulkMessage   
        public void DMFProtocol_BulkMessage(DMFP_BulkFrame cDMFP_BulkFrameRef)
        {
            //---------------------------------------------------------------------------------------------------------Reference. 
            if (cDMFP_BulkFrameRef != null)
            {
                cDMFP_BulkFrame = null;
                cDMFP_BulkFrame = cDMFP_BulkFrameRef;
            }
            cDMFP_BulkFrame.isBusy = true;
            //---------------------------------------------------------------------------------------------------------
            myGlobalBase.myUSBVCOMArray[cDMFP_BulkFrame.DMFP_USBDeviceChannel].isDMFProtocolEnabled = true;
            if (cDMFP_BulkFrame == null)
            {
                myMainProg.myRtbTermMessageLF("#E: Internal Error: DMFProtocol_BulkMessage(): cDMFP_BulkFrame is empty");
                return;
            }
            if (myGlobalBase.myUSBVCOMArray[cDMFP_BulkFrame.DMFP_USBDeviceChannel].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: Selected VCOM Port is not open. Try again after connection (ie Connect USB device)");
                return;
            }
            //-----------------------------------Clear Logger Message. 
            LoggerMessageRX = "";
             m_LoggerMessageRXtemp = "";
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_DMFPBulkMessage = new DMFP_Delegate(DMFP_DMFPBulkMessage_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = cDMFP_BulkFrame.DMFPStreamTX[cDMFP_BulkFrame.DMFPBulkMessageCounter];

            if (myGlobalBase.bNoRFTermMessage == false)
                myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(cDMFP_BulkFrame.DMFP_Delay));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t3 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_DMFPBulkMessage, sMessage, 250, cDMFP_BulkFrame.DMFP_USBDeviceChannel));
            t3.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
        }
        #endregion

        #region //=============================================================================================DMFP_DMFPBulkMessage_CallBack  
        public void DMFP_DMFPBulkMessage_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Data = 0;
            UInt32 Status = hPara[0];   // Error Code
            if (hPara.Count >= 2)
                Data = hPara[1];        // Data Code (ie sent parameter causing error)
            if (cDMFP_BulkFrame.isNoErrorReport == false)       // false = enable error report, otherwise skipped to avoid duplicate error report. 
            {
                if (Status != 0xFFFF)       // 0xFFFF mean no error.
                {
                    myMainProg.myRtbTermMessageLF("#E:Rejected Message Stream<" + cDMFP_BulkFrame.DMFPStreamTX[cDMFP_BulkFrame.DMFPBulkMessageCounter] + ">. Error Code:" + Status.ToString("X") + ". Data Code:" + Data.ToString("X"));
                }
            }
            if (cDMFP_BulkFrame.DMFPBulkMessageCounter < cDMFP_BulkFrame.DMFP_Delegate_CallBack.Count)
                cDMFP_BulkFrame.DMFP_Delegate_CallBack[cDMFP_BulkFrame.DMFPBulkMessageCounter]?.Invoke(iPara, hPara, sPara, dPara, CmdMessage, FullMessage);        // If not Null then Callback to function that send the command. 
            else
                myMainProg.myRtbTermMessageLF("#E:Internal Error in DMFP_DMFPBulkMessage_CallBack(): DMFPBulkMessageCounter is more than limits cDMFP_BulkFrame.DMFP_Delegate_CallBack.Count");
            cDMFP_BulkFrame.DMFPBulkMessageCounter++;
            if (cDMFP_BulkFrame.DMFPBulkMessageCounter < cDMFP_BulkFrame.DMFPStreamTX.Count)
                DMFProtocol_BulkMessage(null);  // Repeat for next message.
            else
                cDMFP_BulkFrame.isBusy = false;
        }
        #endregion

    }
}

//#####################################################################################################
//###################################################################################### DSandBox
//#####################################################################################################

#region //==================================================================Old Code
//-----------------------------------------------------------------------------------------------------------------------Survey Calibration CVS Mode
//if (myGlobalBase.EE_isSurveyCVSRawDataCalibrationActivate == true)  // This only related to -EEIIMPORT for calibration data download, which hide the message from terminal.
//{
//    //RXdata = RXdata.Replace("\0", string.Empty);        // Remove '/0' that causing bug issue. 
//    //RXMessageTemp += RXdata;                          // Add message (like rtbTerm)
//    //if ((RXMessageTemp.Contains("\n")) || (RXMessageTemp.Contains("\r")))
//    //{
//    //    testtesttest = RXMessageTemp;
//    //    //if (RXMessageTemp.Contains("-EEIIMPORT(") ==true)
//    //    if ((RXMessageTemp.StartsWith("-", StringComparison.Ordinal) == true))            // Detect at the start, not within (which may contain negative data)
//    //    {
//    //        myDMFProtocol.DMFP_Recieved_Command(RXMessageTemp);
//    //        RXMessageTemp = "";
//    //        endoflinedetected = true;
//    //    }
//    //}
//    //else
//    //{
//    //    endoflinedetected = false;
//    //}
//    //return;
//}
//-----------------------------------------------------------------------------------------------------------------------Command, ASCII, Hex, etc.
#endregion