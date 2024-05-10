using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;
using System.ComponentModel;
using UDT_Term;
using System.Data;
using System.Globalization;
using UDT_Term_FFT;

namespace UDT_Term_FFT
{
    //#########################################################################################################
    //########################################################################################## Frame_DataLog
    //#########################################################################################################

    #region//=================================================================================Frame_DataLog
    //-------------------------------------------------------- Supported type
    //  Type:       Project         Goes to        Description  
    //--------------------------------------------------------
    //  $E          All             Generic         End of Frame signifier.
    //  $H          LoggerCVS                       Header type frame
    //  $T          LoggerCVS                       Format type frame.
    //  $D          LoggerCVS                       Primary Data Frame
    //  $G          LoggerCVS                       Secondary Data Frame
    //  $F          All             Generic         Error Frame: 1st Para = Error Code, 2nd Para = Message from 0s.
    //  $R          All             Generic         Report Frame. 1st Para = Report Code, 2nd Para = Message from 0s.
    //  $P          ESM                             ESM Periodic Survey Data
    //  $S          ESM                             ESM Shock Survey Data
    //--------------------------------------------------------
    public class UDTmyFrameDataLog
    {
        public DataTable DGVtable;
        //------------------------------------------------
        public List<string> HeaderColumn;                       // List of detoken elements.
        public List<string> FormatColumn;                       // List of detoken elements.
        public List<string> lsFrameIn;
        public string HeaderFrame { get; set; }       // String frame.
        public string FormatFrame { get; set; }       // String frame.
        //------------------------------------------------
        public bool isHeaderFrameFound { get; set; }       // True if detected.
        public bool isFormatFrameFound { get; set; }       // True if detected.
        public bool isFormatFrameFoundFake { get; set; }        // Fake Format Frame is generated based on $H or other frame. 
        public bool isCount_TandH_Matched { get; set; }       // True if both $H and $T count is matched
        public bool isFakeHandTAdded { get; set; }       // True if Header and Format is created due to missing from from LogMem. 
        //------------------------------------------------
        public int ColumnSetup { get; set; }       // Number of Column for DGV table.
        private int ColumnSetupOld { get; set; }       // To detect change to add more Column, in LoggerCVS Section.
        //------------------------------------------------
        public int iCounterE { get; set; }
        public int iCounterH { get; set; }
        public int iCounterF { get; set; }
        public int iCounterD { get; set; }
        public int iCounterG { get; set; }
        public int iCounterT { get; set; }
        public int iCounterR { get; set; }
        public int iCounterP { get; set; }
        public int iCounterS { get; set; }
        public int iCounterB { get; set; }          // HybridFrame
        public int CountByte { get; set; }          // Number of Element Counts received. 

        //-------------------------------------------------LogMem MetaData
        public UInt32 iStartUDT { get; set; }          // Based on $R(UDT;0x10;CRC8) via +STCSTART()
        public UInt32 iStopUDT { get; set; }           // Based on $R(UDT;0x11;CRC8) via +STCSTOP()

        //===================================================Private

        private string sFrameMostElements;
        ITools Tools = new Tools();
        char[] delimiterChars = { ',', ';' };
        //-----------------------------------------------
        public UDTmyFrameDataLog()
        {
            ClearAllVariable();
        }

        //=====================================================================
        //=====================================================================UDTDataFrame:
        // Purpose:  This Class handle the UDT Data Frame or DataLog which include generic LoggerCVS and ESM Project.
        // Input  :  sFrame in UDT DataLog Frame
        // Return :  
        // Note   :
        //=====================================================================
        public void UDTDataLogProcessPage(string PageData)
        {

        }

        //=====================================================================
        //=====================================================================UDTDataFrame:
        // Purpose:  This Class handle the UDT Data Frame or DataLog which include generic LoggerCVS and ESM Project.
        // Input  :  sFrame in UDT DataLog Frame
        // Return :  
        // Note   :
        //=====================================================================
        public void UDTDataLogProcessFrame(string sFrame)
        {
            CountByte += sFrame.Length;  
            //------------------------------------------------------------------
            iCounterE += Tools.CountOccurrencesString(sFrame, "$E", false);
            iCounterH += Tools.CountOccurrencesString(sFrame, "$H", false);
            iCounterF += Tools.CountOccurrencesString(sFrame, "$F", false);
            iCounterD += Tools.CountOccurrencesString(sFrame, "$D", false);
            iCounterG += Tools.CountOccurrencesString(sFrame, "$G", false);
            iCounterT += Tools.CountOccurrencesString(sFrame, "$T", false);
            iCounterR += Tools.CountOccurrencesString(sFrame, "$R", false);
            iCounterP += Tools.CountOccurrencesString(sFrame, "$P", false);
            iCounterS += Tools.CountOccurrencesString(sFrame, "$S", false);
            iCounterB += Tools.CountOccurrencesString(sFrame, "$B", false);     //Hybrid Frame
            //------------------------------------------------------------------
            if (sFrame.Contains("$T"))       // Type Format Frame
            {
                if (isFormatFrameFound == false)
                {
                    FormatFrame = sFrame;
                    isFormatFrameFound = true;
                    string[] partt = FormatFrame.Split(delimiterChars);
                    FormatColumn = new List<string>(partt);
                    if (ColumnSetup < FormatColumn.Count)
                        ColumnSetup = FormatColumn.Count;
                    //---------------------------------------------
                    if (HeaderColumn.Count == FormatColumn.Count)
                        isCount_TandH_Matched = true;
                }
            }
            //------------------------------------------------------------------
            if (sFrame.Contains("$H"))       // Type Header Frame
            {
                if (isHeaderFrameFound == false)
                {
                    HeaderFrame = sFrame;
                    isHeaderFrameFound = true;
                    string[] partt = HeaderFrame.Split(delimiterChars);
                    HeaderColumn = new List<string>(partt);
                    if (ColumnSetup < HeaderColumn.Count)
                        ColumnSetup = HeaderColumn.Count;
                    //---------------------------------------------
                    if (HeaderColumn.Count == FormatColumn.Count)
                        isCount_TandH_Matched = true;
                }
            }
        }
        //=====================================================================
        //=====================================================================UDTDataLogProcessFramePostCollection:
        // Purpose: In case of no $H and $T frame, the code make up defaults
        // Input  : 
        // Return :  
        // Note   : Run this after the UDTDataLogProcessFrame().
        //=====================================================================
        public void UDTDataLogLoggerCVSProcessFramePostCollection(string sFrame)
        {
            //-------------------------------------------------------------------------------
            // In this case the header frame and format frame is missing, so we make it up. 
            // This is one off process, so we use isFormatFrameFoundFake to avoid repeat
            //--------------------------------------------------------------------------------
            if ((isHeaderFrameFound == false) && (isFormatFrameFound == false))
            {
                if ((sFrame.Contains("$R") == false) && (sFrame.Contains("$F") == false))   //$R and $F not allowed.
                {
                    string[] partt = sFrame.Split(delimiterChars);      //$D, $G, $P, $S is used to count frame
                    if (partt.Length > ColumnSetup)
                    {
                        ColumnSetup = partt.Length;                     // Update number of column incrementally
                        //-----------------------------------------------------
                        HeaderColumn.Clear();
                        FormatColumn.Clear();
                        HeaderFrame = "$H;";
                        FormatFrame = "$T;";
                        for (int i = 0; i < ColumnSetup; i++)
                        {
                            HeaderColumn.Add("Data" + i.ToString());
                            FormatColumn.Add(";");
                            HeaderFrame += "Data" + i.ToString() + ";";
                            FormatFrame += ";";
                        }
                        HeaderFrame += "$E\n";
                        FormatFrame += "$E\n";
                        isHeaderFrameFound = true;
                        isFormatFrameFound = true;
                        isFakeHandTAdded = true;
                        isFormatFrameFoundFake = true;
                        //----------------------------------------------------
                    }
                }
            }
            //-------------------------------------------------------------------------------
            // In this case Header Frame was found as well as Typeframe is found, the UDTDataLogProcessFrame() take care of that. 
            // This is one off process, so we use isFormatFrameFoundFake to avoid repeat
            //--------------------------------------------------------------------------------
            if ((isHeaderFrameFound == true) && (isFormatFrameFound == true))
            {
                isFormatFrameFoundFake = true;      // To avoid following code that overwrite formatframe. 
            }
            //-------------------------------------------------------------------------------
            // In this case Header Frame was found but no type frame so we create type frame.
            // This is one off process, so we use isFormatFrameFoundFake to avoid repeat
            //--------------------------------------------------------------------------------
            if ((isHeaderFrameFound == true) && (isFormatFrameFound == false) && (isFormatFrameFoundFake == false))
            {
                isFormatFrameFoundFake = true;
                {
                    FormatFrame = "$T;";
                    for (int i = 0; i < ColumnSetup; i++)
                    {
                        FormatColumn.Add(";");
                        FormatFrame += ";";
                    }
                    FormatFrame += ";$E\n";
                }
            }

        }

        //=====================================================================
        //=====================================================================UDTDataLogProcessFramePostCollection:
        // Purpose: In case of no $H and $T frame, the code make up defaults
        // Input  : 
        // Return :  
        // Note   : Run this after the UDTDataLogProcessFrame().
        //=====================================================================
        public void UDTDataLogProcessFramePostCollection()
        {
            isFakeHandTAdded = false;
            if ((isHeaderFrameFound == true) || (isFormatFrameFound == true))
            {
                //-------------------------------Populate Format Frame if not included.
                if (isFormatFrameFound == false)
                {
                    FormatFrame = "$T;";
                    for (int i = 0; i < ColumnSetup; i++)
                    {
                        FormatColumn.Add(";");
                        FormatFrame += ";";
                    }
                    FormatFrame += ";$E\n";
                }
                return;
            }
            ColumnSetup = 1;
            sFrameMostElements = "";
            foreach (string Frame in lsFrameIn)
            {
                string[] partt = Frame.Split(delimiterChars);
                int count = partt.Length;
                if (ColumnSetup < count)
                {
                    ColumnSetup = count;
                    sFrameMostElements = Frame;
                }
            }
            //-------------------------------------------------//This rarely happen.
            if (sFrameMostElements == "")
            {
                ColumnSetup = 1;
                HeaderColumn.Clear();
                FormatColumn.Clear();
                HeaderFrame = "$H;";
                FormatFrame = "$T;";
                for (int i = 0; i < 50; i++)            //Generate 50 element
                {
                    HeaderColumn.Add("Data" + i.ToString());
                    FormatColumn.Add("");
                    HeaderFrame += "Data" + i.ToString();
                    FormatFrame += "";
                }
                HeaderFrame += ";$E\n";
                FormatFrame += ";$E\n";
                isHeaderFrameFound = true;
                isFormatFrameFound = true;
                isFakeHandTAdded = true;
            }
        }
        //=====================================================================
        //=====================================================================UDTDataLogProcessFramePostCollectionESM:
        // Purpose: In case of no $H and $T frame, the code make up defaults for ESM only
        // Input  : 
        // Return :  
        // Note   : Run this after the UDTDataLogProcessFrame().
        //        : New code for 76P, 10/8/19 RGP to support $R and $F frame. 
        //=====================================================================
        public void UDTDataLogProcessFramePostCollectionESM()
        {
            //--------------------------------------------------------------Seek max number of column. 
            foreach (string Frame in lsFrameIn)
            {
                string[] partt = Frame.Split(delimiterChars);
                int count = partt.Length;
                if (ColumnSetup < count)
                {
                    ColumnSetup = count;
                    sFrameMostElements = Frame;
                }
            }
            //--------------------------------------------------------------
            HeaderColumn.Clear();
            FormatColumn.Clear();
            HeaderFrame = "$H;";
            FormatFrame = "$T;";
            for (int i = 0; i < ColumnSetup; i++)
            {
                HeaderColumn.Add("Data" + i.ToString());
                FormatColumn.Add(";");
                HeaderFrame += "Data" + i.ToString() + ";";
                FormatFrame += ";";
            }
            HeaderFrame += "$E\n";
            FormatFrame += "$E\n";
            isHeaderFrameFound = true;
            isFormatFrameFound = true;
            isFakeHandTAdded = true;
        }

        //=====================================================================
        //=====================================================================UDTDataFrame:
        // Purpose: This handle conversion sFrame in List to DGV Table. 
        // Input  : sFrame in UDT DataLog Frame (whole collection of data)
        // Return :  
        // Note   : Run this after the UDTDataLogProcessFrame().
        //=====================================================================
        public void UDTDataLogDGVTable(List<string> sFrame, bool isHideG, bool isHideD, bool isHideFR, bool isHideS, bool isHideP)
        {
            DGVtable.Rows.Clear();
            //----------------------------------------------Add row with vie select option
            if (DGVtable.Columns.Count != 0)                                       // avoid crashes here.
            {
                for (int i = 0; i < sFrame.Count; i++)
                {
                    if ((sFrame[i].Contains("$G")) & (isHideG == false))
                    {
                        DGVtable.Rows.Add(sFrame[i].Split(','));
                    }
                    if ((sFrame[i].Contains("$D")) & (isHideD == false))
                    {
                        DGVtable.Rows.Add(sFrame[i].Split(','));
                    }
                    if ((sFrame[i].Contains("$S")) & (isHideS == false))        // ESM Project
                    {
                        DGVtable.Rows.Add(sFrame[i].Split(','));
                    }
                    if ((sFrame[i].Contains("$P")) & (isHideP == false))        // ESM Project
                    {
                        DGVtable.Rows.Add(sFrame[i].Split(','));
                    }
                    if (((sFrame[i].Contains("$F")) | (sFrame[i].Contains("$R"))) && (isHideFR == false))
                    {
                        DGVtable.Rows.Add(sFrame[i].Split(','));
                    }
                }
            }
        }

        //=====================================================================
        //=====================================================================ProcessColumnTable:
        // Purpose: This handle conversion sFrame in List to DGV Table. 
        // Input  : sFrame in UDT DataLog Frame (whole collection of data)
        // Return :  
        // Note   : Run this after the UDTDataLogProcessFrame().
        //=====================================================================
        public void InitAndProcessColumnTableHeader()
        {
            if (DGVtable == null)
                DGVtable = new DataTable();
            if (ColumnSetupOld == 1)
            {
                DGVtable.Clear();
                //---------------------------------------------
                for (int i = 0; i < HeaderColumn.Count; i++)
                {
                    DGVtable.Columns.Add(HeaderColumn[i]);
                }
                ColumnSetupOld = ColumnSetup;
            }
            else
            {
                if (ColumnSetup > ColumnSetupOld)       // Add new column.
                {
                    //---------------------------------------------
                    for (int i = ColumnSetupOld; i < HeaderColumn.Count; i++)
                    {
                        DGVtable.Columns.Add(HeaderColumn[i]);
                    }
                    ColumnSetupOld = ColumnSetup;
                }
            }
        }

        //=====================================================================
        //=====================================================================ClearAllVariable:
        // Purpose:  Reset Counts
        // Input  :  
        // Return :  
        // Note   :
        //=====================================================================
        public void ClearAllVariable()
        {
            CountByte = 0;
            iCounterE = 0;
            iCounterH = 0;
            iCounterF = 0;
            iCounterD = 0;
            iCounterG = 0;
            iCounterT = 0;
            iCounterR = 0;
            iCounterP = 0;
            iCounterS = 0;
            iCounterB = 0;
            //----------------------------------------
            iStartUDT = 0;
            iStopUDT = 0;
            //----------------------------------------
            HeaderFrame = "";
            FormatFrame = "";
            //----------------------------------------
            if (HeaderColumn == null)
                HeaderColumn = new List<string>();
            else
                HeaderColumn.Clear();
            //----------------------------------------
            if (FormatColumn == null)
                FormatColumn = new List<string>();
            else
                FormatColumn.Clear();
            //----------------------------------------
            isHeaderFrameFound = false;
            isFormatFrameFound = false;
            isFormatFrameFoundFake = false;
            isCount_TandH_Matched = false;
            ColumnSetup = 1;
            ColumnSetupOld = 1;
            //----------------------------------------
            DGVtable = null;
        }


    }
    #endregion

    //#########################################################################################################
    //########################################################################################## Frame_Detokeniser
    //#########################################################################################################

    #region//=================================================================================Frame_Detokeniser
    //----------------------------------------------------------------- Supported type for Debug Transfer and LogMem
    //  Type:       Summary         Description
    //-----------------------------------------------------------------
    //      0y      Date            TBA         Old format from BG/IDT project. Replaced with UDT Epoch 1970s. Obsolete use.
    //      0z      Time            TBA         Old format from BG/IDT project. Replaced with UDT Epoch 1970s. Obsolete use.
    //      0q      uINT16 Hex      hPara,      no prefix. UDT convert to uINT32. For LogMem.
    //      0w      INT16 Hex       iPara,      no prefix. UDT convert to INT32.  For LogMem.
    //      0i      INT32           iPara
    //      0u      uINT32          hPara       no prefix. UDT convert to uINT32. For LogMem. 32 bits (UDT: unsigned Hex to UINT32 only), especially when prefix is not used.
    //      0n      INT32           iPara       no prefix. UDT convert to INT32.  For LogMem. 32 bits (UDT: signed Hex to IINT32 only),   especially when prefix is not used.
    //      0d      double          dPara
    //      0x      uINT32          hPara
    //      0s      string          sPara
    //      0l      uINT32          hPara       32 bit long, 21/3/20: New UDT REV 78JB. NB: This is not Hex number.  (UINT32 to UINT32 only). Only work on UDTTerm Rev 78HG:22/Feb/20 onward.
    //      undef   string          sPara       Undefined type
    //
    // Note, use UDTTERM 78JE as it support 0n and 0u correctly, this is hex number without prefixes.
    //-----------------------------------------------------------------
    //
    // Approved separator: ';' and ',' and ':'. Alway use default ';' for all project. 
    //--------------------------------------------------------
    public class Frame_Detokeniser
    {
        public List<UInt32> hPara = new List<UInt32>();
        public List<int> iPara = new List<int>();
        public List<string> sPara = new List<string>();
        public List<double> dPara = new List<double>();
        public IList<string> listparameter;
        public string[] sData;
        public string[] sDataConverted;
        public string sDataFileOut;
        ITools Tools = new Tools();
        public Frame_Detokeniser()
        {
            hPara = new List<UInt32>();
            iPara = new List<int>();
            sPara = new List<string>();
            dPara = new List<double>();
            listparameter = null;
            sData = null;
        }

        //=====================================================================
        //=====================================================================DetokeniserMessageLogStyle (LoggerCVS or LogMem format: $X;......;CRC$E\n)
        // Purpose:  Detokenise Survey Data Parameter aligned to FormatColumn[].
        // Input  :  DataFrame in UDT LogMem Style.
        //        :  list of FormatColumn aligned with DataFrame
        // Return :  Error Code: 0 = No Error, -1 to -3 Error Code
        //        :  sData in string array. 
        // Note   :
        //=====================================================================
        public int DetokeniserMessageLogStyle(string DataFrame, List<string> FormatColumn, ref int index)
        {
            char[] delimiterChars = { ',', ';' };
            sDataFileOut = string.Empty;
            sData = DataFrame.Split(delimiterChars);
            sDataConverted = DataFrame.Split(delimiterChars);
            sDataFileOut = "$D;";
            int y;
            for (y = 1; y < sData.Length; y++)
            {
                try
                {
                    switch (FormatColumn[y])
                    {
                        case ("0w"):        // INT16 only
                            {
                                sData[y] = sData[y].Replace("0w", "");
                                sData[y] = Tools.HexStringtoInt16(sData[y]).ToString("D");
                                break;
                            }
                        case ("0q"):        // uINT16 only
                            {
                                sData[y] = sData[y].Replace("0q", "");
                                sData[y] = Tools.HexStringtoUInt16(sData[y]).ToString("D");
                                break;
                            }
                        case ("0u"):        // convert 32 bit unsigned hex to unsigned integer. Do not use for unsigned number to unsigned number, does not work, use 0l instead.
                            {
                                sData[y] = Tools.HexStringtoUInt32(sData[y]).ToString("D");                     // Already number (not hex, nothing)
                                break;
                            }
                        case ("0n"):        // 87JE: convert 32 bit signed hex to signed integer. Do not use for unsigned number to unsigned number, does not work, use 0l instead.
                            {
                                sData[y] = Tools.HexStringtoInt32(sData[y]).ToString("D");                     // Already number (not hex, nothing)
                                break;
                            }
                        case ("0l"):        // 32 bit unsigned long number,                                  // Added 22/Feb/20 to support TSTOCO Rev 5B. NB: This is not Hex number. 
                            {
                                sData[y] = Tools.ConversionStringtoUInt32(sData[y]).ToString("D");              // Already number (not hex, nothing)
                                break;
                            }
                        case ("0i"):        // convert hex to  integer   
                            {
                                sData[y] = Tools.AnyStringtoInt32(sData[y]).ToString("D");
                                break;
                            }
                        case ("0x"):        // Hex (leave unchanged, except to add 0x if missing)
                            {
                                if (sData[y].Contains("0x") == false)
                                {
                                    sData[y] = sData[y].Insert(0, "0x");
                                }
                                break;
                            }
                        case ("0y"):        // date
                            {
                                break;
                            }
                        case ("0z"):        // time
                            {
                                break;
                            }
                        case ("0s"):        // string
                            {
                                sData[y] = sData[y].Replace("0s", "");
                                //sData[y].Add(sData[y]);
                                break;
                            }
                        case ("0d"):        // double
                            {
                                double d = Tools.ConversionStringtoDouble(sData[y]);
                                sData[y] = d.ToString();
                                break;
                            }
                        default:
                            break;
                    }
                    sDataConverted[y] = sData[y];
                    sDataFileOut += (sData[y] + ";");
                }
                catch
                {
                    index = y;
                    return -1;
                }
            }
            index = y;
            return 0;
        }
        //=====================================================================
        //=====================================================================RFDPPD_FormatDataIntoTwoColumn (78JD)
        // Purpose:  Translate formatted data of ADC24A-CH3 and ADC24B-CH3 to two column.
        // Input  :  
        // Return :   
        // Note   :  $D;SNAD;UDT1970;1mSecTick;CHA;CHB;CHA;CHB.......;$E  there 16 pairs
        //=====================================================================
        public string RFDPPD_FormatDataIntoTwoColumn()
        {
            string sRFD = string.Empty;
            int i = 6;

            sRFD = "$D,0,"+ sDataConverted[2] + "," + sDataConverted[3] +"," + sDataConverted[4] + "," + sDataConverted[5]+"\n";
            while (i < sDataConverted.Length)
            {
                if ((sDataConverted[i].Contains("$E") == true))
                    break;
                sRFD += ",,,$D,0,--,--," + sDataConverted[i] + ",";
                i++;
                if ((sDataConverted[i].Contains("$E") == true))
                    break;
                sRFD += sDataConverted[i] + "\n";
                i++;
            }
            return sRFD;
        }
        //=====================================================================
        //=====================================================================DetokeniserMessage: UDT COmmand/Parameter style. 
        // Purpose:  Calculate CRC8 byte array. 
        // Input  :  sFrame in UDT command protocol. See Above
        // Return :  CmdCommand: Extracted Command String
        //        :  CmdParameter: Extracted Parameter string
        //        :  Error Code: 0 = No Error, -1 to -3 Error Code
        // Note   :
        //=====================================================================

        public int DetokeniserMessage(string sFrame, ref string CmdCommand, ref string CmdParameter)
        {
            string MessageFull = sFrame;
            int startParaIndex;
            string sPx;
            //-----------------------------------------------------------------------
            if (sFrame.StartsWith("---", StringComparison.Ordinal) == true)
            {
                return (-3);                                                     // ###ERR : This is comment frame.
            }
            if (sFrame.StartsWith("+++", StringComparison.Ordinal) == true)
            {
                return (-3);
            }
            if (sFrame.StartsWith("###", StringComparison.Ordinal) == true)
            {
                return (-3);
            }
            //---------------------------------------------------------------------------
            try
            {
                //---------------------------------------------------------------Split up the Command and discrete parameter into list
                startParaIndex = sFrame.IndexOf("(");                           // Identify '('
                //EndParaIndex = sFrame.IndexOf(")");
                if (startParaIndex < 0)                                         // Does not exist then return
                    return (-2);                                                // ###ERR : Substandard frame
                CmdCommand = MessageFull.Substring(0, startParaIndex);          // Remove the command part.
                if (CmdCommand == "")                                           // Command is missing
                {
                    return (-4);
                }
                CmdParameter = MessageFull.Substring(startParaIndex, (MessageFull.Length - startParaIndex));   // Remove the parameter part
                CmdParameter = CmdParameter.Replace("(", "");
                CmdParameter = CmdParameter.Replace(")", "");
                CmdParameter = CmdParameter.Replace("\n", "");
                char[] delimiterChars = { ',', ';', ':' };
                listparameter = CmdParameter.Split(delimiterChars);

            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
                return (-1);                                                    // ###ERR : Internal Error (never happen) 
            }

            //---------------------------------------------------------------Format parameter into iPara and sPara (in order)
            foreach (string para in listparameter)
            {
                sPx = para;
                if (Tools.isStringContainxx(sPx))                     // 0x/1x/2x/3x/4x hex which is most common
                {
                    sPx = sPx.Replace("0x", "");
                    sPx = sPx.Replace("0X", "");
                    UInt32 i = Tools.HexStringtoUInt32(sPx);        // Took care of 0x,1x, 2x, 3x, 4x
                    hPara.Add((UInt32)i);
                }
                else if (Tools.isStringContain0q(sPx))                // 0q = uINT16
                {
                    sPx = sPx.Replace("0q", "");
                    sPx = sPx.Replace("0Q", "");
                    UInt16 i = Tools.HexStringtoUInt16(sPx);
                    iPara.Add((Int32)i);
                }
                else if (Tools.isStringContain0w(sPx))                // 0w = INT16
                {
                    sPx = sPx.Replace("0w", "");
                    sPx = sPx.Replace("0W", "");
                    Int16 i = Tools.HexStringtoInt16(sPx);
                    iPara.Add((int)i);
                }
                else if (Tools.isStringContain0i(sPx))                // 0i = integer
                {
                    sPx = sPx.Replace("0i", "");
                    sPx = sPx.Replace("0I", "");
                    Int32 i = Tools.ConversionStringtoInt32(sPx);
                    iPara.Add((int)i);
                }
                else if (Tools.isStringContain0l(sPx))                // 0l = long uINT32 to uINT32
                {
                    sPx = sPx.Replace("0l", "");
                    sPx = sPx.Replace("0L", "");
                    UInt32 i = Tools.ConversionStringtoUInt32(sPx);
                    hPara.Add((UInt32)i);
                }
                else if (Tools.isStringContain0u(sPx))                //0u treat as 0x since they both same UINT32
                {
                    sPx = sPx.Replace("0u", "");
                    sPx = sPx.Replace("0U", "");
                    UInt32 i = Tools.ConversionStringtoUInt32(sPx);
                    hPara.Add((UInt32)i);
                }
                else if (Tools.isStringContain0s(sPx))                // 0s = String 
                {
                    sPx = sPx.Replace("0s", "");
                    sPx = sPx.Replace("0S", "");
                    sPara.Add(sPx);
                }
                else if (Tools.isStringContain0d(sPx))                // 0d = double (64 bits)
                {
                    sPx = sPx.Replace("0d", "");
                    sPx = sPx.Replace("0D", "");
                    double d = Tools.ConversionStringtoDouble(sPx);
                    dPara.Add((double)d);
                }
                else if (Tools.IsString_Numberic_Int32(sPx))          // non-prefix as long it number style (legacy), not string
                {
                    Int32 i = Tools.ConversionStringtoInt32(sPx);
                    iPara.Add((int)i);
                }
                else
                {
                    sPara.Add((string)sPx);                                   // All else goes to string. 
                }
            }
            //             if (hPara.Count == 9)
            //             {
            //                 EndParaIndex = 2;
            //             }
            return 0;
        }

        public void ClearAllVariable()
        {
            hPara.Clear();
            iPara.Clear();
            sPara.Clear();
            dPara.Clear();
            listparameter = null;
            sData = null;
        }

        // EXAMPLE CODE (MINIMUM), texted in DMFP_Recieved_Command
        //         //----------------------------------------------------------------------
        //         Frame_Detokeniser myDetoken = new Frame_Detokeniser();
        //         int DeTokenErrorCode = myDetoken.DetokeniserMessage(MessageFull, ref CmdCommand, ref CmdParameter);
        //             if (DeTokenErrorCode != 0)
        //             {
        //                 switch (DeTokenErrorCode)
        //                 {
        //                     case -1:        // Internal processing error: never happen here so there no bug in code. 
        //                         {
        //                             myMainProg.myRtbTermMessageLF("#E: Internal Error in DetokeniserMessage() Class");
        //                             return;
        //                         }
        //                     case -2:        // Substandard Frame
        //                         {
        //                             myMainProg.myRtbTermMessageLF("+W: Substandard frame detected, ignored. Msg:"+ MessageFull);
        //                             return;
        //                         }
        //                     case -4:        // No Command Frame
        //                         {
        //                             myMainProg.myRtbTermMessageLF("#E: No Command frame detected. Msg:"+ MessageFull);
        //                             return;
        //                         }
        //                     case -3:        // Comment Frame, no need to process further.
        //                         {
        //                             break;
        //                         }
        //                     default:
        //                         break;
        //                 }
        //             }
        //             //---------------------------------------------------------------

    }
    #endregion

    //#########################################################################################################
    //########################################################################################## ESM Stuff: Frame Mode Transfer #@#S and #@#E
    //#########################################################################################################
    // This is modified version of previous protocol so it work better under hybrid frame 

    #region//=================================================================================ESM_LogMem_FrameMode_Transfer  #@#S and #@#E
    public class ESM_LogMem_FrameMode_Transfer
    {
        ITools Tools = new Tools();
        //------------------------------------------------Private variable for internal use. 
        Stopwatch stopWatch;
        string RecievedData;
        int index = 0;
        string sFrame = "";
        //------------------------------------------------Public getter/setter
        public Int64 StopWatchmSec { get; private set; }               // USB bulk transfer performance timing. 
        //------------------------------------------------Public variable
        public List<string> myFrameData;
        //public List<byte> myFrameByte;
        public UDTmyFrameDataLog myUDTmyFrameDataLog;

        public ESM_LogMem_FrameMode_Transfer()
        {
            stopWatch = new Stopwatch();
            myUDTmyFrameDataLog = new UDTmyFrameDataLog();
        }
        //=====================================================================

        #region//=====================================================================StartResetFrameTransfer
        // Purpose:  Process received VCOM reception under Page Transfer Mode
        // Input  :  
        // Return :  true, process completed or #@#E detected. 
        // Note   :
        //=====================================================================
        public void StartResetFrameTransfer()
        {
            if (myFrameData == null)
                myFrameData = new List<string>();
            //----------------------------------
            stopWatch.Stop();
            stopWatch.Reset();
            StopWatchmSec = 0;
        }
        //=====================================================================
        #endregion

        #region //=====================================================================DecodeSerialToFrame
        // Purpose:  Process received VCOM reception under Page Transfer Mode, adapted for ordinary and hybrid. 
        // Input  :  Serial data since #@#S is detected. 
        // Return :  false = #@#E not detected, true = #@#E detected and detoken serial data to string. 
        // Note   :  This code is better suited for ESM where hybrid frame may be used.
        //=====================================================================
        public bool DecodeSerialToFrame(string RXData)
        {
            //RXData.Replace("/0", string.Empty);      // Remove null elements.
            if (myUDTmyFrameDataLog == null)
            {
                myUDTmyFrameDataLog = new UDTmyFrameDataLog();
            }
            RecievedData += RXData;
            myUDTmyFrameDataLog.UDTDataLogProcessFrame(RXData);

            if (RecievedData.Contains("#@#E\n") == true)
            {
                RecievedData.Replace("#@#E\n\0", "");
                RecievedData.Replace("#@#E\n", "");
                myFrameData.Clear();
                index = 0;
                for (int i = 0; i < (RecievedData.Length - 2); i++)                                               // Seek end of frame
                {
                    if ((RecievedData[i] == '$') & (RecievedData[i + 1] == 'E') & (RecievedData[i + 2] == '\n'))
                    {
                        //sFrame = RecievedData.Substring(index, (i - 3 - index));
                        sFrame = RecievedData.Substring(index, (i + 3 - index));        // Include $E\n as well for completeness. 
                        myFrameData.Add(sFrame);
                        index = i + 3;
                        sFrame = "";
                    }
                }
                return (true);
            }
            return (false);
        }
        #endregion
    }

    #endregion

    //#########################################################################################################
    //########################################################################################## ESM Stuff: Page Mode Transfer
    //#########################################################################################################

    #region//=================================================================================ESM_LogMem_PageMode_Transfer #@#P and #@#E
    // This class is exclusive for the bulk page transfer method via USB/UART under VCOM, it should work for UDT hybrid frame as well as ordinary UDT frame.
    // Refer to ComMSG_AddData() in <USB_Message_Manager.cs> for code example from Serial end (with other protocol). 
    // It activated when #@#P it detected, where object instance of this class is formed. The serial code in ComMSG_AddData() become exclusive to this protocol, other UDT protocol is ignore/disable.
    // When #@#E is detected then the page transfer protocol is done and deactivate exclusive protocol. 
    // The end result that you have LIST object in myFrameData which is accessible from this class for software-op to process.
    // It include stopwatch for data transfer performance timing. 
    // It requires Tool.cs for this class to works correctly.
    // Refer to ESM firmware/software Technote. 
    // The code is writen to process 1 bank (65536 pages), for next bank should create new instance of below class or clear current object.
    // ###TASK: Handle multiple bank including contious message between two banks.
    // ###TASK: Testing Testing Testing.
    // ###TASK: Test with hybrid frame. 

    public class ESM_LogMem_PageMode_Transfer
    {
        ITools Tools = new Tools();
        //------------------------------------------------Private variable for internal use. 
        Stopwatch stopWatch;
        string RXMessageTemp;
        string RXMessageTempII;
        char[] delimiterChars = { ',', ';' };
        string EndPageMode = "#@#E\n";              // Pattern for End of Page Transfer

        //------------------------------------------------Public getter/setter
        public int PageNumber { get; private set; }               // Number of pages that transferred/Bank. NB: Page 0 of each bank should be skipped.
        public Int64 StopWatchmSec { get; private set; }               // USB bulk transfer performance timing. 
        public string stoken { get; set; }                      // End of page frame with unprocessed frame, to be transferred over to next bank. 
        //------------------------------------------------Public variable
        public List<string> myFrameData;
        public List<ESM_LogMem_PageObject> myBank;

        public ESM_LogMem_PageMode_Transfer()
        {
            stopWatch = new Stopwatch();
        }
        //=====================================================================

        #region//=====================================================================StartResetPageFrameTransfer
        // Purpose:  
        // Input  :  
        // Return :  
        // Note   :
        //=====================================================================
        public void StartResetPageFrameTransfer()
        {
            RXMessageTemp = "";
            RXMessageTempII = "";
            if (myFrameData == null)
                myFrameData = new List<string>();
            else
                myFrameData.Clear();

            if (myBank == null)
                myBank = new List<ESM_LogMem_PageObject>();
            else
                myBank.Clear();
            for (int i = 0; i <= 0x7FFFF; i++)              // 8 Bank => 0 to 0x7FFFF pages. 
                myBank.Add(new ESM_LogMem_PageObject());
            //----------------------------------
            stopWatch.Stop();
            stopWatch.Reset();
            StopWatchmSec = 0;
        }
        //=====================================================================
        #endregion

        #region//=====================================================================ProcessPageFrame
        // Purpose:  Process received VCOM reception under Page Transfer Mode
        // Input  :  
        // Return :  true, process completed or #@#E detected. 
        // Note   :
        //=====================================================================
        public bool ProcessPageFrame(string RXdata)
        {
            RXMessageTemp += RXdata;                                                            // Add message (like rtbTerm)
            int pos = 1;
            int posII = 1;
            if (stopWatch.IsRunning == false)
            {
                stopWatch.Start();
            }
            //-----------------------------------------------------------------------------------End of Page Mode process. This happen due to 100mSec delay in ESM firmware before sending out #@#E\n
            if ((RXMessageTemp.Length < 6) & RXdata.Contains(EndPageMode))
            {
                stopWatch.Stop();
                StopWatchmSec = stopWatch.ElapsedMilliseconds;
                return (true);
            }
            //----------------------------------------------------------------------------------Take out page from reception buffer.
            while (true)                                                                        // UDTerm 76N: New method to process discrete \n statement (especially when fast). 
            {
                int tempLen = RXMessageTemp.Length;
                pos = RXMessageTemp.IndexOf(";#E\n");                                           // 
                if (pos == 0)                                                                   // Empty string. IndexOf: The zero-based index position of value if that string is found, or -1 if it is not. If value is Empty, the return value is 0.
                    break;
                if (pos == -1)                                                                  // ';#E\n' Not found, incomplete transfer?, wait for next USB bulk. 
                    break;
                pos = pos + 4;          // include the ;#E\n element.
                //=============================================================================Process Page Frame
                if (pos >= 1)
                {
                    if (tempLen== pos)
                    {
                        while (true) ;
                    }
                    posII = RXMessageTemp.IndexOf("#P");                                          // Start Page Frame
                    RXMessageTempII = RXMessageTemp.Substring(posII, pos);                        // Take out 1st message to late \n. 
                    RXMessageTemp = RXMessageTemp.Substring(pos, (RXMessageTemp.Length - pos));   // Remaining is goes back to RX reception.
                    string[] parth = RXMessageTempII.Split(delimiterChars);                       // only use this to take out page number on parth[1]. 
                    PageNumber = Tools.HexStringtoInt32(parth[1]);
                    if (PageNumber <= 0x7FFFF)
                    {
                        myBank[PageNumber].PageNumber = PageNumber;
                        myBank[PageNumber].PageData = RXMessageTempII.Substring(14, pos - 14);
                        //myBank[PageNumber].PageData.Replace(";#E\n", "");
                    }
                }
            }
            //=============================================================================Process End of Page Transfer
            if (RXMessageTemp.Contains(EndPageMode))
            {
                stopWatch.Stop();
                StopWatchmSec = stopWatch.ElapsedMilliseconds;
                return (true);
            }
            return (false);
        }
        //=====================================================================
        #endregion

        #region //=====================================================================DecodePageToFrame
        // Purpose:  Process received VCOM reception under Page Transfer Mode
        // Input  :  isStarNextBank, set false for 1st bank and then true for next bank so message frame is continuous between banks.
        // Return :  Error Message in string, otherwise "" if goes well. 
        // Note   :
        //=====================================================================
        public string DecodePageToFrame(bool isStartNextBank)
        {
            int pageno = 1;
            int offset = 0;
            int index = 0;
            int pageloop = 256;
            char[] b = new char[2000];
            //--------------------------------------------------
            for (int i = 0; i < 2000; i++)                                                                        //Clear char array
            { b[i] = '\0'; }
            //---------------------------------------------------Next Bank handling. 
            if (isStartNextBank == true)
            {
                offset = stoken.Length;
                for (int i = 0; i < offset; i++)                                                                // Copy unprocessed string to char array for next frame scan via next page.
                {
                    b[i] = stoken[i];
                }
            }
            else
            {
                stoken = "";
            }
            //-----------------------------------------------WARNING, DO NOT MODIFY!!!!
            try
            {
                while (pageno <= PageNumber)                                                                    // Process Page loop.
                {
                    myBank[pageno].PageData = myBank[pageno].PageData.Replace(";#E\n\0", "");
                    myBank[pageno].PageData = myBank[pageno].PageData.Replace(";#E\n", "");                     // Remove end of page frame terminator.
                    //---------------------------------------------------
                    for (int i = 0; i < 256; i++)                                                               // copy string to char array
                    {
                        b[offset + i] = myBank[pageno].PageData[i];
                        if ((offset + i) >= (2000-1))
                        {
                            return ("Exception in DecodePageToFrame(), message exceed Array limit (2000) without $E\n terminator. Did Bulk Erase used?");
                        }
                    }
                    //---------------------------------------------------
                    for (int i = 0; i < (pageloop + offset); i++)                                               // Seek end of frame
                    {
                        if ((b[i] == '$') & (b[i + 1] == 'E') & (b[i + 2] == '\n'))
                        {
                            stoken += myBank[pageno].PageData.Substring(index, (i + 3 - index - offset));       // Found, copy to Frame list. 
                            myFrameData.Add(stoken);
                            index = i + 3 - offset;                                                             // Adjust index for next end of frame.
                            stoken = "";
                        }
                    }
                    //---------------------------------------------------
                    stoken += myBank[pageno].PageData.Substring(index, (pageloop - index));                      // take out unprocessed string, bug fixed, add +
                    offset = stoken.Length;
                    for (int i = 0; i < 2000; i++)                                                               // Clear char array
                    { b[i] = '\0'; }
                    for (int i = 0; i < offset; i++)                                                            // Copy unprocessed string to char array for next frame scan via next page.
                    {
                        b[i] = stoken[i];
                    }
                    index = 0;
                    //---------------------------------------------------
                    pageno++;                                                                                   // Next Page
                }
            }
            catch
            {
                return ("Exception in DecodePageToFrame()");
            }
            //----------------------------------------------
            return ("");
        }

        #endregion

        #region//=====================================================================isBankPageNumberMissing
        // Purpose:  isBankPageNumberMissing
        // Input  :  
        // Return :  true: all page (except Page 0) is continous from Page 1 to last loaded page. False if missing page within the Page 1 to last loaded page.    
        // Note   :
        //=====================================================================
        public bool isBankPageNumberMissing()
        {
            int pageno = 1;
            while (pageno <= PageNumber)
            {
                if (myBank[pageno].PageNumber == 0)
                {
                    return true;
                }
                pageno++;
            }
            return false;
        }
        //=====================================================================
        #endregion

        #region //=====================================================================GetListBankMissingPage
        // Purpose:  isBankPageNumberMissing
        // Input  :  
        // Return :  true: all page (except Page 0) is continous from Page 1 to last loaded page. False if missing page within the Page 1 to last loaded page.    
        // Note   :
        //=====================================================================
        public List<int> GetListBankMissingPage()
        {
            List<int> missagepage = new List<int>();
            int pageno = 1;
            while (pageno <= PageNumber)
            {
                if (myBank[pageno].PageNumber == 0)
                {
                    missagepage.Add(pageno);
                }
                pageno++;
            }
            return missagepage;
        }
        #endregion
    }

    #region//=================================================================================ESM_LogMem_PageObject
    public class ESM_LogMem_PageObject
    {
        public int PageNumber;
        public string PageData;

        public ESM_LogMem_PageObject()
        {
            PageNumber = 0;
            PageData = "";
        }
    }
    #endregion

    #endregion

    //#########################################################################################################
    //########################################################################################## ESM Stuff: Hybrid Frame $B
    //#########################################################################################################

    #region//=================================================================================Hybrid Frame $B
    // The role of the Hybrid frame is to convert binary stream into data that
    // The Datatype is defined 
    //      V = Vibration.  3 axis in OGC X/Y/Z in 8 bits, return INT8 data in LIST<i8XYZ> fashion. 
    //      S = Shock.      3 axis in OGC X/Y/Z in 8 bits, return INT8 data in LIST<i8XYZ> fashion.
    //      G = Gyro.       1 axis in OGC Z axis in 8 bits. return INT8 data in LIST<i8XYZ> fashion.
    // The 3 axis would have a class with contains Start UDT, LIST<i8XYZ>, Counts, Para2-16 in uINT32 number.
    // Syntax: $B;UDT(1970);DataType;<.....BinaryData.....>;Counts;Para2;....;Para16;CRC8$E\n.  
    public class ESM_LogMem_Hyrbid_Frame
    {
        public ESM_HFData_i8XYZ myVibration;
        public ESM_HFData_i8XYZ myShock;
        public ESM_HFData_i8XYZ myGyro;
        public int ErrorCode { get; private set; }
        public ESM_LogMem_Hyrbid_Frame()
        {
            myVibration = new ESM_HFData_i8XYZ();
            myShock = new ESM_HFData_i8XYZ();
            myGyro = new ESM_HFData_i8XYZ();
            ErrorCode = 0xFF;
        }

        #region//=====================================================================HybridFrame_ProcessString
        // Purpose:  This code translate the hybrid frame in string into meaningful data chucks.
        // Input  :  string data of Hybrid Frame from $B to $E\n. 
        // Return :  The resultant data goes to selected class object myVibration, myShock , myRPM.
        // Note   :  Syntax: $B;UDT(1970);DataType;<.....BinaryData.....>;Counts;Para2;....;Para16;CRC8$E\n.
        //        :  ErrorCode: 0x1FF (Vibration), 0x2FF(Shock) and 0x3FF(Gyro/RPM) = Success!
        //        :           : Other number than FF provide reason for error
        //                    : Bit 7 = cleared: Endterm issue (missing $E)
        //                    : Bit 6 = Issue with UpdateParameter
        //                    : Bit 5 = Issue with ProcessBinaryData
        //                    : Bit 4 = Issue with UDT1970 conversion. 
        //                    : 0x01 = This is not hybrid frame, $B.
        //                    : 0x02 = Corrupted Frame.
        //https://stackoverflow.com/questions/1003275/how-to-convert-utf-8-byte-to-string
        //=====================================================================
        public int HybridFrame_ProcessString(string sData)
        {
            if (sData.Contains("$B;") == false)
            {
                ErrorCode = 0x01;           // This is not hybrid frame
                return 0x01;
            }
            //--------------------------------------------------Test Code for conversion
            //             sbyte[] sbtest = { -127, -120, -110, -100, -90, -80, -70, -60, -50, -40, -30, -20, -10, 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 127 };
            //             byte[] btest = { 0x80, 0x90, 0xA0, 0xB0, 0xC0, 0xD0, 0xE0, 0xF0, 0xFF,0x0,0x10,0x20,0x30,0x40,0x50,0x60,0x70 };
            // 
            //             sData = "$B;5C7E92E2;G;<";
            //             sData += Encoding.Default.GetString(btest);
            //             sData +=">;1F67892;F327812;$E\n";
            //-------------------------------------------This is simple extraction but not idiot proof, we can find more rugged solution later
            int startpos = sData.IndexOf(";<") + 2;
            int endpos = sData.LastIndexOf(">;");
            if ((endpos == -1) || (startpos == -1))
            {
                //ErrorCode = 0x02;
            }
            else
            {
                //--------------------------------------------------------------------------------------------------------------------------------
                string binarydata = sData.Substring(startpos, endpos - startpos);
                string prestring = sData.Substring(0, startpos - 2);
                string poststring = sData.Substring(endpos + 2, sData.Length - (endpos + 2));
                string[] preparts = prestring.Split(';');
                string[] postparts = poststring.Split(';');
                char[] Datatype = preparts[2].ToCharArray();

                switch (Datatype[0])
                {
                    case ('V'):
                        {
                            if (myVibration.AddUDT1970(preparts[1]) == false)
                                ErrorCode &= 0xEF;       // Issue with UDT1970 conversion. 
                            if (myVibration.ProcessBinaryData_sByte(binarydata) == false)
                                ErrorCode &= 0xDF;       // Issue with ProcessBinaryData
                            for (int i = 0; i < postparts.Length - 1; i++)
                            {
                                if (myVibration.UpdateParameter(postparts[i], i) == false)
                                    ErrorCode &= 0xBF;       // Issue with UpdateParameter
                            }
                            if (myVibration.AddEndTerm(postparts[postparts.Length - 1]) == false)
                                ErrorCode &= 0x7F;          // Issue with EndTerm (Missing $E)
                            return (0x100 | ErrorCode);      // 0x1FF = Vibration type, no error. 0x1XX = Error
                        }
                    case ('S'):
                        {
                            if (myShock.AddUDT1970(preparts[1]) == false)
                                ErrorCode &= 0xEF;       // Issue with UDT1970 conversion. 
                            if (myShock.ProcessBinaryData_sByte(binarydata) == false)
                                ErrorCode &= 0xDF;       // Issue with ProcessBinaryData
                            for (int i = 0; i < postparts.Length - 1; i++)
                            {
                                if (myShock.UpdateParameter(postparts[i], i) == false)
                                    ErrorCode &= 0xBF;       // Issue with UpdateParameter
                            }
                            if (myShock.AddEndTerm(postparts[postparts.Length - 1]) == false)
                                ErrorCode &= 0x7F;          // Issue with EndTerm (Missing $E)
                            return (0x200 | ErrorCode);      // 0x2FF = Shock type, no error. 0x2XX = Error
                        }
                    case ('G'):
                        {
                            if (myGyro.AddUDT1970(preparts[1]) == false)
                                ErrorCode &= 0xEF;       // Issue with UDT1970 conversion. 
                            if (myGyro.ProcessBinaryData_sByte(binarydata) == false)
                                ErrorCode &= 0xDF;       // Issue with ProcessBinaryData
                            for (int i = 0; i < postparts.Length - 1; i++)
                            {
                                if (myGyro.UpdateParameter(postparts[i], i) == false)
                                    ErrorCode &= 0xBF;       // Issue with UpdateParameter
                            }
                            if (myGyro.AddEndTerm(postparts[postparts.Length - 1]) == false)
                                ErrorCode &= 0x7F;          // Issue with EndTerm (Missing $E)
                            return (0x300 | ErrorCode);      // 0x3FF = RPM/Gyro type, no error. 0x2XX = Error
                        }
                    default:
                        {
                            ErrorCode = 0x02;   // DataType Not recognized. 
                            break;
                        }
                }
            }
            return (ErrorCode);
        }
        #endregion
        
        #region//=====================================================================HybridFrame_ConvertedDate
        //=====================================================================
        // Purpose:  This code accommodate the converted data into string with translated binary to ASCII for converted filename
        // Input  :  take place after the HybridFrame_ProcessString.
        // Return :  data string to be saved as converted file name
        // Note   :  Syntax: $B;UDT(1970);DataType;<.....BinaryData.....>;Para1;Para2;$E\n.
        //=====================================================================
        public string HybridFrame_ConvertedDate(int DataType)
        {
            string ConvertedFrame = "";
            try
            {

                switch (DataType)
                {
                    case (0x1FF):   //Vibration
                        {
                            ConvertedFrame = "Vibration," + myVibration.uUDT1970.ToString() + ",";
                            ConvertedFrame += myVibration.sDataFile;
                            if (myVibration.lPara.Count >= 2)
                            {
                                ConvertedFrame += "0x" + myVibration.lPara[0].ToString("X") + ",";
                                ConvertedFrame += "0x" + myVibration.lPara[1].ToString("X");
                            }
                            break;
                        }
                    case (0x2FF):   //Shock
                        {
                            ConvertedFrame = "Shock," + myShock.uUDT1970.ToString() + ",";
                            ConvertedFrame += myShock.sDataFile;
                            if (myShock.lPara.Count >= 2)
                            {
                                ConvertedFrame += "0x" + myShock.lPara[0].ToString("X") + ",";
                                ConvertedFrame += "0x" + myShock.lPara[1].ToString("X");
                            }
                            break;
                        }
                    case (0x3FF):   //Gyro
                        {
                            ConvertedFrame = "Gyro(RPM)," + myGyro.uUDT1970.ToString() + ",";
                            ConvertedFrame += myGyro.sDataFile;
                            if (myGyro.lPara.Count >= 2)
                            {
                                ConvertedFrame += "0x" + myGyro.lPara[0].ToString("X") + ",";
                                ConvertedFrame += "0x" + myGyro.lPara[1].ToString("X");
                            }
                            break;
                        }
                    default:
                        {
                            ConvertedFrame = "Error in DataType Select <HybridFrame_ConvertedDate()>";
                            break;
                        }
                }
            }
            catch
            {
                ConvertedFrame = "Exception Occurred in <HybridFrame_ConvertedDate()>";
            }
            return ConvertedFrame;
        }
        #endregion

    }
    #endregion

    #region//=================================================================================ESM_HFData_i8XYZ (3 axis)
    public class ESM_HFData_i8XYZ
    {
        ITools Tools = new Tools();
        public int iCounts { get; private set; }      //1st parameter alway goes to iCounts.
        public UInt32 uUDT1970 { get; private set; }
        public string sEndTerm { get; private set; }
        public List<UInt32> lPara;
        public List<iVector3> lData;
        public List<sByteVector3> sbData;
        public string sDataFile { get; private set; }

        public ESM_HFData_i8XYZ()
        {
            iCounts = 0;
            uUDT1970 = 0;
            sEndTerm = "";
            lData = new List<iVector3>();
            lPara = new List<UInt32>();
            sbData = new List<sByteVector3>();
            for (int i = 0; i < 16; i++)            //
            {
                lPara.Add(new UInt32());
            }
        }

        #region//=====================================================================ProcessBinaryData
        // Purpose:  Process string which Convert 3 x Byte data into 3 axis vector (INT8) expressed in INT32
        // Input  :  binarydata string
        // Return :  true = Success, false = error in process. 
        // Note   : sByte = INT8 while Byte = uINT8. Must be in Unicode format. 
        //=====================================================================
        public bool ProcessBinaryData(string binarydata)            // 6 byte binary
        {
            iVector3 result = new iVector3();
            //byte[] buffer = System.Text.Encoding.Unicode.GetBytes(binarydata);
            byte[] buffer = System.Text.UTF8Encoding.Unicode.GetBytes(binarydata);
            for (int i = 0; i < buffer.Length; i += 3)
            {
                if ((buffer.Length - i) <= 2)       //anti exception error!
                    break;
                result.X = buffer[i + 0];
                result.Y = buffer[i + 1];
                result.Z = buffer[i + 2];
                lData.Add(result);
            }
            return true;
        }
        #endregion

        #region//=====================================================================ProcessBinaryData
        // Purpose:  Process string which Convert 3 x Byte data into 3 axis vector (INT8) expressed in INT32
        // Input  :  binarydata string
        // Return :  true = Success, false = error in process. 
        // Note   :  sByte = INT8 while Byte = uINT8. Must be in Unicode format. 
        //=====================================================================
        public bool ProcessBinaryData_sByte(string binarydata)            // 6 byte binary
        {
            sDataFile = "";
            sByteVector3 result = new sByteVector3();
            byte[] buffer = Encoding.Default.GetBytes(binarydata);
            sbyte[] sbBuffer = Array.ConvertAll(buffer, b => unchecked((sbyte)b));

            for (int i = 0; i < binarydata.Length; i += 3)
            {
                if ((binarydata.Length - i) <= 2)       //anti exception error!
                    break;
                result.X = sbBuffer[i];
                result.Y = sbBuffer[i + 1];
                result.Z = sbBuffer[i + 2];
                sbData.Add(result);
                sDataFile += result.X.ToString(NumberFormatInfo.InvariantInfo);
                sDataFile += ',';
                sDataFile += result.Y.ToString(NumberFormatInfo.InvariantInfo);
                sDataFile += ',';
                sDataFile += result.Z.ToString(NumberFormatInfo.InvariantInfo);
                sDataFile += ',';
            }
            //             //----------------------------------------
            //             //https://docs.microsoft.com/en-us/dotnet/api/system.sbyte.tostring?view=netframework-4.7.2
            //             //----------------------------------------
            //             NumberFormatInfo nfi = new NumberFormatInfo();
            //             nfi.NegativeSign = "~";
            //             foreach (sbyte value in sbBuffer)
            //             {
            //                 sDataFile += value.ToString(NumberFormatInfo.InvariantInfo);
            //                 sDataFile += ',';
            //             }
            return true;
        }
        #endregion

        #region//=====================================================================AddUDT1970
        // Purpose:  Convert UDT1970 as string into uINT32 number
        // Input  :  string UDT1970
        // Return :  true = Success, false = failed, leave UDT9170=0 as error.
        // Note   : 
        //=====================================================================
        public bool AddUDT1970(string sData)
        {
            uUDT1970 = Tools.AnyStringtoUInt32(sData);
            if ((uUDT1970 == 0) || (uUDT1970 == 975579))
            {
                uUDT1970 = 0;
                return false;
            }
            return true;
        }
        #endregion

        #region//=====================================================================AddCount
        // Purpose:  Convert UDT1970 as string into uINT32 number
        // Input  :  string UDT1970
        // Return :  true = Success, false = failed, leave UDT9170=0 as error.
        // Note   : 
        //=====================================================================
        public bool AddCount(string sData)
        {
            iCounts = Tools.AnyStringtoInt32(sData);
            if ((iCounts == 0) || (iCounts == 975579))
            {
                iCounts = 0;
                return false;
            }
            return true;
        }
        #endregion

        #region//=====================================================================AddEndTerm
        // Purpose:  Add end terminator which may contains CRC8 or not. 
        // Input  :  string CRC8$E\n or $E\n. 
        // Return :  true = Success, false = failed, leave sEndTerm ="";
        // Note   : 
        //=====================================================================
        public bool AddEndTerm(string sData)
        {
            if (sData.Contains("$E"))
            {
                sEndTerm = sData;
                return (true);
            }
            sEndTerm = "";
            return (true);
        }
        #endregion

        #region//=====================================================================UpdateParameter
        // Purpose:  Assigned parameter index and string parameter into list parameter
        // Input  :  HexData: string hex. Parameter Index (0 to 15).  
        // Return :  true = success!, false = failure/not updated.
        // Note   :  Para[0] is reserved as Counts INT8 in BinaryData. 
        //=====================================================================
        public bool UpdateParameter(string HexData, int index)
        {
            if (index >= 16)
                return (false);
            if (index == 0)
                iCounts = Tools.AnyStringtoInt32(HexData);
            lPara[index] = Tools.AnyStringtoUInt32(HexData);
            return (true);
        }
        #endregion

        // Sandbox
        //         string sData = "F0807F";
        //         ESM_HFData_i8XYZ mytest = new ESM_HFData_i8XYZ();
        //         iVector3 myresult = mytest.AddData(sData);
        //         myRtbTermMessageLF("SData=" + sData);
        //         myRtbTermMessageLF("X=" + myresult.X.ToString());
        //         myRtbTermMessageLF("Y=" + myresult.Y.ToString());
        //         myRtbTermMessageLF("Z=" + myresult.Z.ToString());

        //         result.X = (SByte) Convert.ToSByte(binarydata.Substring(0, 2), 16);
        //         result.Y = (SByte) Convert.ToSByte(binarydata.Substring(2, 2), 16);
        //         result.Z = (SByte) Convert.ToSByte(binarydata.Substring(4, 2), 16);
        //         binarydata = binarydata.Substring(7, binarydata.Length);
    }
    #endregion

    #region//=================================================================================ESM_HFData_i8Z (One Axis)
    public class ESM_HFData_i8Z
    {
        ITools Tools = new Tools();
        public int iCounts { get; private set; }      //1st parameter alway goes to iCounts.
        public UInt32 uUDT1970 { get; private set; }
        public string sEndTerm { get; private set; }
        public List<UInt32> lPara;
        public List<int> lData;

        public ESM_HFData_i8Z()
        {
            iCounts = 0;
            uUDT1970 = 0;
            sEndTerm = "";
            lData = new List<int>();
            lPara = new List<UInt32>();
            for (int i = 0; i < 16; i++)
            {
                lPara.Add(new UInt32());
            }
        }

        #region//=====================================================================ProcessBinaryData
        // Purpose:  Process string which Convert 1 x Byte data into 1 axis in INT8 expressed in INT32
        // Input  :  binarydata string
        // Return :  true = Success, false = error in process. 
        // Note   : sByte = INT8 while Byte = uINT8. 
        //=====================================================================
        public bool ProcessBinaryData(string binarydata)            // 6 byte binary
        {
            byte[] buffer = System.Text.Encoding.Unicode.GetBytes(binarydata);
            while (binarydata.Length != 0)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    lData.Add(buffer[i]);
                }
            }
            return true;
        }
        #endregion

        #region//=====================================================================AddCount
        // Purpose:  Convert UDT1970 as string into uINT32 number
        // Input  :  string UDT1970
        // Return :  true = Success, false = failed, leave UDT9170=0 as error.
        // Note   : 
        //=====================================================================
        public bool AddCount(string sData)
        {
            iCounts = Tools.AnyStringtoInt32(sData);
            if ((iCounts == 0) || (iCounts == 975579))
            {
                iCounts = 0;
                return false;
            }
            return true;
        }
        #endregion

        #region//=====================================================================AddEndTerm
        // Purpose:  Add end terminator which may contains CRC8 or not. 
        // Input  :  string CRC8$E\n or $E\n. 
        // Return :  true = Success, false = failed, leave sEndTerm ="";
        // Note   : 
        //=====================================================================
        public bool AddEndTerm(string sData)
        {
            if (sData.Contains("$E"))
            {
                sEndTerm = sData;
                return (true);
            }
            sEndTerm = "";
            return (true);
        }
        #endregion

        #region//=====================================================================UpdateParameter
        // Purpose:  Assigned parameter index and string parameter into list parameter
        // Input  :  HexData: string hex. Parameter Index (0 to 15).  
        // Return :  true = success!, false = failure/not updated.
        // Note   :  Para[0] is reserved as Counts INT8 in BinaryData. 
        //=====================================================================
        public bool UpdateParameter(string HexData, int index)
        {
            if (index >= 16)
                return (false);
            if (index == 0)
                iCounts = Tools.AnyStringtoInt32(HexData);
            lPara[index] = Tools.AnyStringtoUInt32(HexData);
            return (true);
        }
        #endregion

        #region//=====================================================================AddUDT1970
        // Purpose:  Convert UDT1970 as string into uINT32 number
        // Input  :  string UDT1970
        // Return :  true = Success, false = failed, leave UDT9170=0 as error.
        // Note   : 
        //=====================================================================
        public bool AddUDT1970(string sData)
        {
            uUDT1970 = Tools.AnyStringtoUInt32(sData);
            if ((uUDT1970 == 0) || (uUDT1970 == 975579))
            {
                uUDT1970 = 0;
                return false;
            }
            return true;
        }
        #endregion
    }
    #endregion

    //#########################################################################################################
    //########################################################################################## ESM Stuff
    //#########################################################################################################

    #region//=================================================================================ESM_LogMem_Report_Statistic
    public class ESM_LogMem_Report_Statistic
    {
        public List<int> liTypeNoA;
        public List<int> liTypeNoB;
        public List<string> lsCommentA;
        public List<string> lsCommentB;
        public List<UInt32> luDataA;
        public List<UInt32> luDataB;
        public bool isUpdated;
        public bool isUploadfinish;
        public bool isUDTUpdated;
        //----------------------------------------------------------------------------------------
        private string[] sCommentA = {
            "Frame StartTime (UDT)",
            "Frame StopTime (UDT)",
            "CountE (End of Frame)",
            "CountT (Format Frame)",
            "CountH (Header Frame)",
            "CountD (Data1 Frame)",
            "CountR (Report Frame)",
            "CountG (Data2 Frame)",
            "CountP (Period Data Frame)",
            "CountS (Shock Data Frame)",
            "CountUndef (Other Frame)",
            "FoundAdd (Found Start Frame Address)", //36 Length
            "FoundUDT (Found Start Frame UDT)",
            "FoundType (Found Start Frame Type)",
            "",
            "",
    };
        private string[] sCommentB = {
            "Global Address (Internal)",
            "Local Address (Internal)",
            "Bank Select (Internal <0-7>)",
            "Bank Status (Internal, 1=OK)",
    };
        private int[] iTypeNoA = { 0xE0, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xEB, 0xEC, 0xED, 0xEE, 0xEF };
        private int[] iTypeNoB = { 0xFB, 0xFC, 0xFD, 0xFE };
        //-----------------------------------------------------------Optional, LogMem low level parameter.	
        //----------------------------------------------------------------------------------------
        ITools Tools = new Tools();
        //----------------------------------------------------------------------------------------
        public ESM_LogMem_Report_Statistic()
        {
            isUpdated = false;
            isUploadfinish = false;
            isUDTUpdated = false;
            //------------------------------------------
            liTypeNoA = new List<int>();
            liTypeNoB = new List<int>();
            lsCommentA = new List<string>();
            lsCommentB = new List<string>();
            luDataA = new List<UInt32>();
            luDataB = new List<UInt32>();
            //------------------------------------------
            liTypeNoA.AddRange(iTypeNoA);
            liTypeNoB.AddRange(iTypeNoB);
            lsCommentA.AddRange(sCommentA);
            lsCommentB.AddRange(sCommentB);
            for (int i = 0; i < liTypeNoA.Count; i++)
                luDataA.Add(0xFFFFFFFF);
            for (int i = 0; i < liTypeNoB.Count; i++)
                luDataB.Add(0xFFFFFFFF);
            //------------------------------------------
        }

        public bool UpdateData(int TypeNo, UInt32 Data)
        {
            isUpdated = true;
            try
            {
                if ((TypeNo >= 0xFB) & (TypeNo <= 0xFE))
                {
                    luDataB[TypeNo - 0xFB] = Data;
                    return (true);
                }
                if ((TypeNo >= 0xE0) & (TypeNo <= 0xEF))
                {
                    luDataA[TypeNo - 0xE0] = Data;
                    return (true);
                }
            }
            catch { };
            return (false);
        }

        public bool ReadData(int TypeNo, out string Comment, out UInt32 Data)
        {
            Comment = "";
            Data = 0xFFFFFFFF;
            try
            {
                if ((TypeNo >= 0xFB) & (TypeNo <= 0xFE))
                {
                    Comment = lsCommentB[TypeNo - 0xFB];
                    Data = luDataB[TypeNo - 0xFB];
                    return (true);
                }
                if ((TypeNo >= 0xE0) & (TypeNo <= 0xEF))
                {
                    Comment = lsCommentA[TypeNo - 0xE0];
                    Data = luDataA[TypeNo - 0xE0];
                    return (true);
                }
            }
            catch { };
            return (false);
        }

        public string ReadMessage(int TypeNo)
        {
            string sMessage = "Null";
            if ((TypeNo >= 0xE0) & (TypeNo <= 0xEF))
            {
                TypeNo = TypeNo - 0xE0;
                sMessage = "TypeNo: 0x" + liTypeNoA[TypeNo].ToString("X2")
                    + " | Data: 0x" + luDataA[TypeNo].ToString("X8")
                    + " | " + lsCommentA[TypeNo] + Environment.NewLine;
            }
            if ((TypeNo >= 0xFB) & (TypeNo <= 0xFE))
            {
                TypeNo = TypeNo - 0xFB;
                sMessage = "TypeNo: 0x" + liTypeNoB[TypeNo].ToString("X2")
                + " | Data: 0x" + luDataB[TypeNo].ToString("X8")
                + " | " + lsCommentB[TypeNo] + Environment.NewLine;
            }
            return sMessage;
        }

        public int DecodeReportFrame(string sFrame)
        {
            int iTypeNo = -1;
            UInt32 uData = 0;
            char[] delimiterChars = { ',', ';' };
            if (sFrame.Contains("$R;"))
            {
                string[] mytoken = sFrame.Split(delimiterChars);
                if (mytoken.Length == 4)      // Alway 4 tokens, alway hex data. $R; TypeNo ; Data ; $E\n (No CRC8)
                {
                    if (Tools.IsString_Hex0x(mytoken[1]))
                    {
                        iTypeNo = Tools.HexStringtoInt32(mytoken[1]);
                        uData = Tools.HexStringtoUInt32(mytoken[2]);
                        UpdateData(iTypeNo, uData);
                    }
                }
                else return (-1);   // Incorrect frame data
            }
            else return (-1);       // Incorrect frame
                                    //--------------------------------------
            if (iTypeNo == 0xFE)
            {
                isUploadfinish = true;
                isUpdated = true;
            }
            return (iTypeNo);
        }
    }
    #endregion

    //#########################################################################################################
    //########################################################################################## ESM_STC_LogMem
    //#########################################################################################################

    #region//=================================================================================ESM_STC_LogMem
    public class ESM_STC_LogMem
    {
        //----------------------------------------------------------------------------------------
        public string[] sSelectMethod = {
                "0 = Reserved",                             // Pause
                "1 = Reserved",                             // Continue. 
                "2 = Frame / All Data",
                "3 = Frame / UDT-Start to UDT-Stop",
                "4 = Frame / Address & No of Frame",
                "5 = Page  / All Data (Per Bank)",
                "6 = Frame / UDT-Start & No of Frame",      // New for UDTTERM 78JE
                "7 = Spare",
                "8 = Spare",                                // Use $S and $P. 
                "9 = Reserved",                             // Transfer Cancelled/Reset.
                "10= Report/Error Frame Only",
                "11= Tool Data in Report Frame style",
                "12= Old Cmd LOG_DOWNLOAD()"        
        };
        //----------------------------------------------------------------------------------------
        public int SelectMethod;           // Selected Method to seen STCUPLOAD
        public UInt32 Parameter1;             // UDT-Start or Start Address 
        public UInt32 Parameter2;             // UDT-Stop or Number of Page/Frame
        public UInt32 TimerPauseActive;       // 4th Parameter, Transfer activity period in second. 
        public UInt32 TimerPauseWait;         // 5th Parameter, No activity period in second.
        private string sParameter1;
        private string sParameter2;
        public bool isPageMode;             // 0 = Frame by Frame, 1 = Page by Page. 
        //----------------------------------------------------------------------------------------
        ITools Tools = new Tools();
        //----------------------------------------------------------------------------------------
        ESM_LogMem_Report_Statistic myStatistic;
        public ESM_STC_LogMem()
        {
            SelectMethod = 2;
            sParameter1 = "";
            sParameter2 = "";
            Parameter1 = 0;
            Parameter2 = 0;
            TimerPauseActive = 0;
            TimerPauseWait = 0;
        }
        //----------------------------------------------------------------------------------------
        public string GenerateCommand_STCUPLOAD(int MethodIndex)
        {
            sParameter1 = "0";
            sParameter2 = "0";
            string command = "";
            switch (MethodIndex)
            {
                case (2):
                    {
                        command = "+STCUPLOAD(0x2)";
                        break;
                    }
                case (3):
                    {
                        sParameter1 = Parameter1.ToString("X");
                        sParameter2 = Parameter2.ToString("X");
                        command = "+STCUPLOAD(0x" + MethodIndex.ToString("X") + ";0x" + sParameter1 + ";0x" + sParameter2 + ")";
                        isPageMode = false;
                        break;
                    }
                case (4):
                    {
                        sParameter1 = Parameter1.ToString("X");
                        sParameter2 = Parameter2.ToString("X");
                        command = "+STCUPLOAD(0x" + MethodIndex.ToString("X") + ";0x" + sParameter1 + ";0x" + sParameter2 + ")";
                        isPageMode = false;
                        break;
                    }
                case (5):
                    {
                        sParameter1 = Parameter1.ToString("X");
                        command = "+STCUPLOAD(0x5;0x" + sParameter1 + ")";
                        break;
                    }
                case (6):
                    {
                        sParameter1 = Parameter1.ToString("X");
                        sParameter2 = Parameter2.ToString("X");
                        command = "+STCUPLOAD(0x" + MethodIndex.ToString("X") + ";0x" + sParameter1 + ";0x" + sParameter2 + ")";
                        isPageMode = true;
                        break;
                    }
                case (7):
                    {
                        sParameter1 = Parameter1.ToString("X");
                        sParameter2 = Parameter2.ToString("X");
                        command = "+STCUPLOAD(0x" + MethodIndex.ToString("X") + ";0x" + sParameter1 + ";0x" + sParameter2 + ")";
                        isPageMode = true;
                        break;
                    }
                case (10):
                    {
                        command = "+STCUPLOAD(0xA)";
                        break;
                    }
                case (11):
                    {
                        command = "+STCUPLOAD(0xB)";
                        break;
                    }
                case (12):
                    {
                        command = "LOG_DOWNLOAD()";
                        break;
                    }
                default:
                    break;
            }
            return command;
        }
        //----------------------------------------------------------------------------------------
        public void AppendPage(string sPage)
        {


        }

    }
    #endregion

    //#########################################################################################################
    //########################################################################################## CRC8CheckSum
    //#########################################################################################################

    #region//=================================================================================CRC8CheckSum
    public class CRC8CheckSum
    {
        // This is fast implementation of CRC which return 8 bit result as it avoid high order math. The code below should be identical 
        // https://github.com/WasatchPhotonics/CRC8_Example/blob/master/CRC8_Checksum_Example/MainWindow.xaml.cs 
        // http://oshgarage.com/the-crc8-checksum/
        // https://stackoverflow.com/questions/29214301/ios-how-to-calculate-crc-8-dallas-maxim-of-nsdata
        // This is better than additive method which has high chance of missing flip bits.

        public int CRC8FailCounter { get; set; }
        public int CRC8PassCounter { get; set; }
        ITools Tools = new Tools();
        byte[] CRC_8_TABLE =
        {
            0, 94,188,226, 97, 63,221,131,194,156,126, 32,163,253, 31, 65,
            157,195, 33,127,252,162, 64, 30, 95,  1,227,189, 62, 96,130,220,
            35,125,159,193, 66, 28,254,160,225,191, 93,  3,128,222, 60, 98,
            190,224,  2, 92,223,129, 99, 61,124, 34,192,158, 29, 67,161,255,
            70, 24,250,164, 39,121,155,197,132,218, 56,102,229,187, 89,  7,
            219,133,103, 57,186,228,  6, 88, 25, 71,165,251,120, 38,196,154,
            101, 59,217,135,  4, 90,184,230,167,249, 27, 69,198,152,122, 36,
            248,166, 68, 26,153,199, 37,123, 58,100,134,216, 91,  5,231,185,
            140,210, 48,110,237,179, 81, 15, 78, 16,242,172, 47,113,147,205,
            17, 79,173,243,112, 46,204,146,211,141,111, 49,178,236, 14, 80,
            175,241, 19, 77,206,144,114, 44,109, 51,209,143, 12, 82,176,238,
            50,108,142,208, 83, 13,239,177,240,174, 76, 18,145,207, 45,115,
            202,148,118, 40,171,245, 23, 73,  8, 86,180,234,105, 55,213,139,
            87,  9,235,181, 54,104,138,212,149,203, 41,119,244,170, 72, 22,
            233,183, 85, 11,136,214, 52,106, 43,117,151,201, 74, 20,246,168,
            116, 42,200,150, 21, 75,169,247,182,232, 10, 84,215,137,107, 53
        };

        public CRC8CheckSum()
        {
            CRC8CounterReset();
        }
        //=====================================================================
        //=====================================================================isContainCRC
        // Purpose:  check if CRC is included in the sFrame that need to be taken out
        // Input  :  sFrame             : full message frame 
        //        :  sFrameRemovedCRC   : full message minus the CRC$E\n ending. 
        // Return :  false: No CRC detected.
        //        :  true CRC detected.
        // Note   :
        //=====================================================================
        public bool isContainCRC(string sFrame, ref string sFrameRemovedCRC)
        {
            try
            {
                sFrameRemovedCRC = "";
                if (sFrame.Contains("$E") == false)
                    return (false);
                if (sFrame.Contains(";$E") == true)
                    return (false);
                if (sFrame.Contains(",$E") == true)
                    return (false);
                int end = sFrame.IndexOf("$E");
                sFrameRemovedCRC = sFrame.Substring(0, end - 2);
            }
            catch
            {
                return (false);
            }

            return (true);
        }

        //=====================================================================
        //=====================================================================Calc_CRC8
        // Purpose:  Calculate CRC8 byte array. 
        // Input  :  DataArray[] : derived from string to byte or read content i FLASH in byte or uINT8.
        //        :  Length: of the DataArray[] of which CRC is affected. 
        // Return :  Resultant CRC. 
        // Note   :
        //=====================================================================
        public byte Calc_CRC8(byte[] DataArray, int length)
        {
            int i;
            byte CRC = 0;
            for (i = 0; i < length; i++)
                CRC = CRC_8_TABLE[CRC ^ DataArray[i]];
            return CRC;
        }
        //=====================================================================
        //=====================================================================CRC8ExtractInUDTMessageFrame
        // Purpose: Extract CRC from frame based on UDT Protocol. 
        // Input  :  sFrame, must have $........CC$E\n style which is UDT Protocol.
        //        :  CRC is the result after process, otherwise 0 if error happen
        // Return :  Error Code, if 0 the CRC should have CRC extraction. 
        //            0 = No Error
        //           -1 = No CRC exist, it has ';' or other format. 
        //           -2 = $E is not contained in the frame. 
        // Note   : Message Frame: Must have ending with CC$E\n, where we need to take out CC and convert to Byte. 
        //=====================================================================
        public int CRC8ExtractInUDTMessageFrame(string sFrame, ref byte CRC)
        {
            CRC = 0;
            if (sFrame.Contains("$E") == false)
                return (-2);
            int end = sFrame.IndexOf("$E");                     //end point to $
            string sCRC = sFrame.Substring(end - 2, 2);       // end -3 to end-1 take out CC from CC$E\n as needed. 
            if (Tools.IsString_Hex(sCRC))
            {
                CRC = Tools.StringTwoASCIItoByte(sCRC);
                return 0;
            }
            return (-1);
        }

        //=====================================================================
        //=====================================================================CRC8CalculateOfUDTMessageFrame
        // Purpose:  Calculate CRC8 up to just before the "CC$E\n"
        // Input  :  sFrame, must have $........CC$E\n style which is UDT Protocol.
        //        :  CRC is the result after process, otherwise 0 if error happen.
        // Return :  Error Code, if 0 the CRC should have CRC extraction. 
        //            0 = No Error
        //           -2 = $E is not contained in the frame. 
        // Note   :
        //=====================================================================
        public int CRC8CalculateOfUDTMessageFrame(string sFrame, ref byte CRC)
        {
            CRC = 0;
            if (sFrame.Contains("$E") == false)
                return (-2);
            int end = sFrame.IndexOf("$E");                     //end point to $
            string sCRC = sFrame.Substring(0, (end - 2));         // take out sFrame up to before the CC$E\n
            byte[] CRCByte = Tools.StrToByteArray(sCRC);
            CRC = Calc_CRC8(CRCByte, CRCByte.Length);
            return (0);
        }

        //=====================================================================
        //=====================================================================UDTMessageFrameIsPassedCRC8
        // Purpose:  Complete service
        // Input  :  DataArray[] : derived from string to byte or read content i FLASH in byte or uINT8.
        //        :  Length: of the DataArray[] of which CRC is affected. 
        // Return :  Resultant CRC. 
        // Note   :
        //=====================================================================
        public bool UDTMessageFrameIsPassedCRC8(string sFrame)
        {
            byte CRCinFrame = 0;                                              // Take out CRC within the sFrame (CC$E\n)
            if (CRC8ExtractInUDTMessageFrame(sFrame, ref CRCinFrame) != 0)
                return false;
            byte CRCofFrame = 0;                                            // Calculate CRC of the sFrame up to before CC$E\n. 
            if (CRC8CalculateOfUDTMessageFrame(sFrame, ref CRCofFrame) != 0)
                return false;
            if (CRCinFrame != CRCofFrame)
            {
                CRC8FailCounter++;
                return false;
            }
            CRC8PassCounter++;
            return true;
        }

        public void CRC8CounterReset()
        {
            CRC8FailCounter = 0;
            CRC8PassCounter = 0;
        }
    }
    #endregion

    //#########################################################################################################
    //########################################################################################## ESM Sensor Configuration
    //#########################################################################################################

    #region//=================================================================================ESM Sensor Configuration (Old Design, to be obsleted out)
    //==================================================================
    //================================================================== CMD_NVM_ConfigSensor (Callback)
    // Purpose	: This update the configuration in MCU Flash (including write process). 
    // Input	: iPm[0] = Config type, iPm[1] = Config Data	
    //	    	: Master Send					Comment													ESM response (callback)
    // Note 	: +CMDCFGS(0x1)							: Read Setup ESMSConfigSensor (all sensor)					-CMDCFG(0xFF;0xESMSConfigSensor,0xESMSConfigSensorADXL372) 
    //          : +CMDCFGS(0x3)							: Read Setup OSMath for 3 sensor							-CMDCFG(0xFF;0xESMConfigMathADXL357;0xESMConfigMathADXL372;0xESMConfigMathBMG250)
    //---------------------------------------------------------------ADXL357 Section
    //			: +CMDCFGS(0x10;0xD;0xD)				: Write ESMSConfigSensor ADXL357 (ODR and HFP)				-CMDCFG(0xFF) = OKAY, 0xXX = Error code
    //			: +CMDCFGS(0x11;0xXXXXXXXX)				: Write ESMConfigMathADXL357								-CMDCFG(0xFF) = OKAY, 0xXX = Error code
    //			: +CMDCFGS(0x12;0xD)					: Write ESMSConfigSensor ADXL357 (Range 40/20/10G)			-CMDCFG(0xFF) = OKAY, 0xXX = Error code, default 40G
    //			: +CMDCFGS(0x13;0xD)					: Write ESMConfigINACTQualADXL357, INACT Qualifier			-CMDCFG(0xFF) = OKAY, 0xXX = Error code, default: Enabled, 5G Threshold on 1st 16 CBuffer capture. Use Peak threshold (rectified)	
    //			: +CMDCFGS(0x14;0xD)					: Write ESMConfigACTQualADXL357, ACT Qualifier				-CMDCFG(0xFF) = OKAY, 0xXX = Error code, default: Enabled, 5G Threshold on 1st 16 CBuffer capture. Use Peak threshold (rectified)
    //---------------------------------------------------------------ADXL372 Section (Instant On Mode)
    //			: +CMDCFGS(0x20;0xD;0xD;0xD)			: Write ESMSConfigSensorADXL372								-CMDCFG(0xFF) = OKAY, 0xXX = Error code	  Write Config ADXL372 (ODR, BW, 0Low1HighG)	
    //			: +CMDCFGS(0x21;0xXXXXXXXX)				: Write ESMConfigMathADXL372 								-CMDCFG(0xFF) = OKAY, 0xXX = Error code	
    //			: +CMDCFGS(0x22;0xXXXXXXXX)				: Write ESMConfigACTQualADXL372 							-CMDCFG(0xFF) = OKAY, 0xXX = Error code	  Write Config ACT Qualifier ADXL372
    //---------------------------------------------------------------BMG250 Section
    //			: +CMDCFGS(0x30;0xD;0xD)				: Write ESMSConfigSensor BMG250 (ODR and BW)				-CMDCFG(0xFF) = OKAY, 0xXX = Error code
    //			: +CMDCFGS(0x31;0xXXXXXXXX)				: ESMConfigMathBMG250  for BMG250							-CMDCFG(0xFF) = OKAY, 0xXX = Error code
    //---------------------------------------------------------------
    //			: +CMDCFGS(0xE0)						: Number or write cycle burned.								-CMDCFG(0xFF;0x002) response with number of write cycle done. >1000 = risk of issue.
    //			: +CMDCFGS(0xF0)						: Reset Default, OSMath/Config Only							-CMDCFG(0xFF) = OKAY, 0xXX = Error code
    //---------------------------------------------------------------
    //			: +CMDCFGS(0xXX)						: Not supported number get									-CMDCFG(0x00) Not supported command error. 
    //==================================================================

    [Serializable]
    public class ESM_NVM_SensorConfig
    {
        ITools Tools = new Tools();
        public int SelectSensor { get; set; }               // 0 = Generic, 1 = ADXL357, 2 = ADXL372, 3 = BMG250
        public UInt32 ConfigSensor_BMGo357 { get; set; }               // For ESMSConfigSensor structure (internal sensor parameter). 
        public UInt32 ConfigSensor_ADXL372 { get; set; }               // For ESMSConfigSensorADXL372 structure (internal sensor parameter). 
        public UInt16 ConfigOSMath_ADXL357 { get; set; }
        public UInt16 ConfigOSMath_ADXL372 { get; set; }
        public UInt16 ConfigOSMath_BMG250 { get; set; }
        public UInt16 FS_ADXL357 { get; set; }

        // public UInt16 FS_ADXL372          { get; set; }
        public UInt16 FS_BMG250 { get; set; }
        public UInt16 ADXL357Range { get; set; }               //0 = Error, 1=10G, 2=20G, 3=40G, Bit 16/17 of ConfigSensor_BMGo357
        public ESM_NVM_SensorConfig()
        {
            ConfigSensor_BMGo357 = 0;
            ConfigSensor_ADXL372 = 0;
            ConfigOSMath_ADXL357 = 0;
            ConfigOSMath_ADXL372 = 0;
            ConfigOSMath_BMG250 = 0;
            FS_ADXL357 = 10;
            // FS_ADXL372 = 10;
            FS_BMG250 = 10;
            ADXL357Range = 3;            //40G Default. 
        }
        //=========================================================================================Sensor Section
        public string WriteNVMConfigSensor()          // +CMDCFG(0x12;0xD)		Write Range setting
        {
            return ("+CMDCFGS(0x12;0x" + ADXL357Range.ToString("X") + ")\n");
        }
        public string ReadNVMConfigSensor()          // +CMDCFG(0x04)		callback via Callback_ReadNVMConfigSensor() 
        {
            return ("+CMDCFGS(0x04)\n");
        }
        public void Callback_ReadNVMConfigSensor(UInt32 InConfigSensor_BMGo357, UInt32 InConfigSensor_ADXL372)  // +CMDCFG(0x04)==> -CMDCFGS(0xFF;0xESMSConfigSensor;oxESMSConfigSensorADXL372)
        {
            ConfigSensor_BMGo357 = InConfigSensor_BMGo357;
            ConfigSensor_ADXL372 = InConfigSensor_ADXL372;
            ADXL357Range = (UInt16)Tools.Bits_UInt32_intRead2Bit(ConfigSensor_BMGo357, 16);
        }
        public void ConfigSensor_UpdateADXL357Range(int InADXL357Range)
        {
            if (ADXL357Range == 0)
                return;
            Tools.Bits_UInt32_intWrite2Bit(ConfigSensor_BMGo357, 16, (UInt32)InADXL357Range);
            ADXL357Range = (UInt16)InADXL357Range;
        }

        public string UpdateNVMCommandADXL357Range()        // +CMDCFG(0x13;0xD)		: Write ADXL357 Range. 
        {
            ADXL357Range = (UInt16)Tools.Bits_UInt32_intRead2Bit(ConfigSensor_BMGo357, 16);
            return ("+CMDCFGS(0x12;0x" + ADXL357Range.ToString("X") + ")\n");
        }

        //=========================================================================================OSMath Section
        public string ReadNVMDataOSMath()               // +CMDCFG(0x03)		callback via CallBack_ReadNVMDataOSMath() 
        {
            return ("+CMDCFGS(0x03)\n");
        }
        public void CallBack_ReadNVMDataOSMath(UInt32 ESMConfigMathADXL357, UInt32 ESMConfigMathADXL372, UInt32 ESMConfigMathBMG250)
        {
            ConfigOSMath_ADXL357 = (UInt16)(ESMConfigMathADXL357 >> 16);
            ConfigOSMath_ADXL372 = (UInt16)(ESMConfigMathADXL372 >> 16);
            ConfigOSMath_BMG250 = (UInt16)(ESMConfigMathBMG250 >> 16);
            FS_ADXL357 = (UInt16)(ESMConfigMathADXL357 & 0x0000FFFF);
            //FS_ADXL372 = (UInt16)(ESMConfigMathBMG250 & 0x0000FFFF);
            FS_BMG250 = (UInt16)(ESMConfigMathBMG250 & 0x0000FFFF);
        }
        //=========================================================================================
        public string UpdateNVMCommandADXL357()     // +CMDCFG(0x11;0xXXXXXXXX)		: Write FS and ConfigOSMath  for ADXL357
        {
            UInt32 DataADXL357 = ((UInt32)(ConfigOSMath_ADXL357) << 16) | ((UInt32)FS_ADXL357);
            return ("+CMDCFGS(0x11;0x" + DataADXL357.ToString("X") + ")\n");
        }
        public string UpdateNVMCommandADXL372()     // +CMDCFG(0x21;0xXXXXXXXX)		: Write FS and ConfigOSMath  for ADXL372
        {
            UInt32 DataADX372 = ((UInt32)(ConfigOSMath_ADXL372) << 16) & (0xFFFF0000);
            return ("+CMDCFGS(0x21;0x" + DataADX372.ToString("X") + ")\n");
        }
        public string UpdateNVMCommandMBG250()      // +CMDCFG(0x31;0xXXXXXXXX)		: Write FS and ConfigOSMath  for BMG250
        {
            UInt32 DataBMG250 = ((UInt32)(ConfigOSMath_BMG250) << 16) | ((UInt32)FS_BMG250);
            return ("+CMDCFGS(0x31;0x" + DataBMG250.ToString("X") + ")\n");
        }
        //=========================================================================================
        //-----------------------------------------------------------------------------------------Sensor Only
        public int iADXL357Range()
        {
            return Tools.Bits_UInt32_intRead2Bit(ConfigSensor_BMGo357, 16);
        }
        //-----------------------------------------------------------------------------------------OSMath Only
        public bool isAVG(UInt16 ConfigOSMathVariable)
        {
            return (Tools.Bits_UInt16_Read(ConfigOSMathVariable, 0));
        }
        public bool isRMS(UInt16 ConfigOSMathVariable)
        {
            return (Tools.Bits_UInt16_Read(ConfigOSMathVariable, 1));
        }
        public bool isFFT(UInt16 ConfigOSMathVariable)
        {
            return (Tools.Bits_UInt16_Read(ConfigOSMathVariable, 2));
        }
        public bool isMin(UInt16 ConfigOSMathVariable)
        {
            return (Tools.Bits_UInt16_Read(ConfigOSMathVariable, 3));
        }
        public bool isMax(UInt16 ConfigOSMathVariable)
        {
            return (Tools.Bits_UInt16_Read(ConfigOSMathVariable, 4));
        }
        public bool isFirst(UInt16 ConfigOSMathVariable)
        {
            return (Tools.Bits_UInt16_Read(ConfigOSMathVariable, 5));
        }
        public bool isData16(UInt16 ConfigOSMathVariable)
        {
            return (Tools.Bits_UInt16_Read(ConfigOSMathVariable, 6));
        }
        public bool isHFStream(UInt16 ConfigOSMathVariable)
        {
            return (Tools.Bits_UInt16_Read(ConfigOSMathVariable, 8));
        }
        public UInt16 ReadSelectFormat(UInt16 ConfigOSMathVariable)
        {
            return (UInt16)(ConfigOSMathVariable >> 9 & 0x00000003);
        }
        public UInt16 ReadAssignedData16(UInt16 ConfigOSMathVariable)
        {
            return (UInt16)(ConfigOSMathVariable >> 13 & 0x00000007);
        }
    }
    #endregion

    #region//=================================================================================ESM Sensor Configuration
    //==================================================================
    //================================================================== CMD_NVM_ConfigSensor (Callback)
    // Purpose	: This update the configuration in MCU Flash (including write process). 
    // Input	: iPm[0] = Config type, iPm[1] = Config Data	
    //	    	: Master Send					Comment													ESM response (callback)
    // Note 	: +CMDCFGS(0x1)							: Read Setup ESMSConfigSensor (all sensor)					-CMDCFG(0xFF;0xESMSConfigSensor,0xESMSConfigSensorADXL372) 
    //          : +CMDCFGS(0x3)							: Read Setup OSMath for 3 sensor							-CMDCFG(0xFF;0xESMConfigMathADXL357;0xESMConfigMathADXL372;0xESMConfigMathBMG250)
    //---------------------------------------------------------------ADXL357 Section
    //			: +CMDCFGS(0x10;0xD;0xD)				: Write ESMSConfigSensor ADXL357 (ODR and HFP)				-CMDCFG(0xFF) = OKAY, 0xXX = Error code
    //			: +CMDCFGS(0x11;0xXXXXXXXX)				: Write ESMConfigMathADXL357								-CMDCFG(0xFF) = OKAY, 0xXX = Error code
    //			: +CMDCFGS(0x12;0xD)					: Write ESMSConfigSensor ADXL357 (Range 40/20/10G)			-CMDCFG(0xFF) = OKAY, 0xXX = Error code, default 40G
    //			: +CMDCFGS(0x13;0xXXXXXXXX)				: Write ESMConfigINACTQualADXL357, INACT Qualifier			-CMDCFG(0xFF) = OKAY, 0xXX = Error code, default: Enabled, 5G Threshold on 1st 16 CBuffer capture. Use Peak threshold (rectified)	
    //			: +CMDCFGS(0x14;0xXXXXXXXX)				: Write ESMConfigACTQualADXL357, ACT Qualifier				-CMDCFG(0xFF) = OKAY, 0xXX = Error code, default: Enabled, 5G Threshold on 1st 16 CBuffer capture. Use Peak threshold (rectified)
    //---------------------------------------------------------------ADXL372 Section (Instant On Mode)
    //			: +CMDCFGS(0x20;0xD;0xD;0xD)			: Write ESMSConfigSensorADXL372								-CMDCFG(0xFF) = OKAY, 0xXX = Error code	  Write Config ADXL372 (ODR, BW, 0Low1HighG)	
    //			: +CMDCFGS(0x21;0xXXXXXXXX)				: Write ESMConfigMathADXL372 								-CMDCFG(0xFF) = OKAY, 0xXX = Error code	
    //			: +CMDCFGS(0x22;0xXXXXXXXX)				: Write ESMConfigACTQualADXL372 							-CMDCFG(0xFF) = OKAY, 0xXX = Error code	  Write Config ACT Qualifier ADXL372
    //---------------------------------------------------------------BMG250 Section
    //			: +CMDCFGS(0x30;0xD;0xD)				: Write ESMSConfigSensor BMG250 (ODR and BW)				-CMDCFG(0xFF) = OKAY, 0xXX = Error code
    //			: +CMDCFGS(0x31;0xXXXXXXXX)				: ESMConfigMathBMG250  for BMG250							-CMDCFG(0xFF) = OKAY, 0xXX = Error code
    //---------------------------------------------------------------
    //          : +CMDCFGS(0x40;ESMConfigSensor;ESMConfigSensorADXL372;ESMConfigACTQualADXL372;ESMConfigMathADXL357;ESMConfigMathBMG250;ESMConfigMathADXL372;ESMConfigINACTQualADXL357;ESMConfigACTQualADXL357)	
    //			: +CMDCFGS(0xE0)						: Number or write cycle burned.								-CMDCFG(0xFF;0x002) response with number of write cycle done. >1000 = risk of issue.
    //			: +CMDCFGS(0xF0)						: Reset Default, OSMath/Config Only							-CMDCFG(0xFF) = OKAY, 0xXX = Error code
    //---------------------------------------------------------------
    //			: +CMDCFGS(0xXX)						: Not supported number get									-CMDCFG(0x00) Not supported command error. 
    //==================================================================

    [Serializable]
    public class ESM_NVM_SensorConfigII
    {
        public ESMConfigSensor myESMConfigSensor;
        public ESMConfigSensorADXL372 myESMConfigSensorADXL372;
        public ESMConfigACTQualADXL372 myESMConfigACTQualADXL372;
        public ESMConfigMathADXL357 myESMConfigMathADXL357;
        public ESMConfigMathBMG250 myESMConfigMathBMG250;
        public ESMConfigMathADXL372 myESMConfigMathADXL372;
        public ESMConfigINACTQualADXL357 myESMConfigINACTQualADXL357;
        public ESMConfigACTQualADXL357 myESMConfigACTQualADXL357;
        public UInt32 myWriteCycle { get; set; }
        public string ErrorCode { get; set; }
        public ESM_NVM_SensorConfigII()
        {
            myESMConfigSensor = new ESMConfigSensor();
            myESMConfigSensorADXL372 = new ESMConfigSensorADXL372();
            myESMConfigACTQualADXL372 = new ESMConfigACTQualADXL372();
            myESMConfigMathADXL357 = new ESMConfigMathADXL357();
            myESMConfigMathBMG250 = new ESMConfigMathBMG250();
            myESMConfigMathADXL372 = new ESMConfigMathADXL372();
            myESMConfigINACTQualADXL357 = new ESMConfigINACTQualADXL357();
            myESMConfigACTQualADXL357 = new ESMConfigACTQualADXL357();
            myWriteCycle = 0;
            ErrorCode = "";
        }

        public void InitDefault()
        {
            //         myESMConfigSensor.ESMConfigSensor_Word = 0x000B0B02;
            //         myESMConfigSensorADXL372.ESMConfigSensorADXL372_Word = 0x00000033;
            //         myESMConfigACTQualADXL372.ESMConfigACTQualADXL372_Word = 0x05300110;
            //         myESMConfigMathADXL357.ESMConfigMathADXL357_Word = 0x6A010064;
            //         myESMConfigMathBMG250.ESMConfigMathBMG250_Word = 0x7A010064;
            //         myESMConfigMathADXL372.ESMConfigMathADXL372_Word = 0x520A0000;
            //         myESMConfigINACTQualADXL357.ESMConfigINACTQualADXL357_Word = 0x0F0F0100;
            //         myESMConfigACTQualADXL357.ESMConfigACTQualADXL357_Word = 0x0F07D001;

            myESMConfigSensor.ConfigDecodeWord(0x000B0B02);
            myESMConfigSensorADXL372.ConfigDecodeWord(0x00000033);
            myESMConfigACTQualADXL372.ConfigDecodeWord(0x05300110);
            myESMConfigMathADXL357.ConfigDecodeWord(0x6A010064);
            myESMConfigMathBMG250.ConfigDecodeWord(0x7A010064);
            myESMConfigMathADXL372.ConfigDecodeWord(0x520A0000);
            myESMConfigINACTQualADXL357.ConfigDecodeWord(0x0F0F0100);
            myESMConfigACTQualADXL357.ConfigDecodeWord(0x0F07D001);

        }


        //-----------------------------------------------------------------CMDCFG_GenericCallBack
        public UInt32 CMDCFG_GenericCallBack(List<UInt32> hPara)
        {
            ErrorCode = "";
            if (hPara[0] != 0xFF)
            {
                switch (hPara[0])
                {
                    case 0x11:
                        ErrorCode = "Parameter Error: ADXL357_ODR (0 to 10)";
                        break;
                    case 0x12:
                        ErrorCode = "Parameter Error: ADXL357_HPF (0 to 6)";
                        break;
                    case 0x1F:
                        ErrorCode = "Parameter Error: ConfigOSMath Not for ADXL357 Device";
                        break;
                    case 0x15:
                        ErrorCode = "Parameter Error: ADXL357 Range (1 to 3)";
                        break;
                    case 0x13:
                        ErrorCode = "Parameter Error: INACT ADXL372 Threshold (1 to 125)";
                        break;
                    case 0x14:
                        ErrorCode = "Parameter Error: ACT ADXL357 Threshold (1 to 125)";
                        break;
                    case 0x21:
                        ErrorCode = "Parameter Error: ADXL372_ODR  (0 to 5)";
                        break;
                    case 0x22:
                        ErrorCode = "Parameter Error: ADXL372_BW  (0 to 5)";
                        break;
                    case 0x23:
                        ErrorCode = "Parameter Error: ADXL372_TH  (0 to 1)";
                        break;
                    case 0x2F:
                        ErrorCode = "Parameter Error: ConfigOSMath Not for ADXL372 Device";
                        break;
                    case 0x31:
                        ErrorCode = "Parameter Error: BMG250_ODR  (6 to 13)";
                        break;
                    case 0x32:
                        ErrorCode = "Parameter Error: BMG250_BWP  (0 to 2)";
                        break;
                    case 0x3F:
                        ErrorCode = "Parameter Error: ConfigOSMath Not for BMG250 Device";
                        break;
                    case 0x9F:
                        ErrorCode = "Parameter Error: !Write NVM Failure!";
                        break;
                    default:
                        ErrorCode = "Parameter Error: Misc/Generic or 0x00";
                        break;

                }
                return hPara[0];
            }

            else
                return 0xFF;
        }
        //-----------------------------------------------------------------CMDCFG_Read_1
        public string CMDCFG_Read_1()           //+CMDCFGS(0x1)	: Read Setup ESMSConfigSensor (all sensor)	// -CMDCFG(0xFF;0xESMSConfigSensor,0xESMSConfigSensorADXL372)
        {
            return "+CMDCFGS(0x1)";
        }
        public UInt32 CMDCFG_CallBack_1(List<UInt32> hPara)
        {
            if (hPara[0] != 0xFF)
                return hPara[0];
            myESMConfigSensor.ConfigDecodeWord(hPara[1]);
            myESMConfigSensorADXL372.ConfigDecodeWord(hPara[2]);
            return (0xFF);
        }
        //-----------------------------------------------------------------CMDCFG_Read_3
        public string CMDCFG_Read_3()          // +CMDCFGS(0x3) : Read Setup OSMath for 3 sensor // -CMDCFG(0xFF;0xESMConfigMathADXL357;0xESMConfigMathADXL372;0xESMConfigMathBMG250)
        {
            return "+CMDCFGS(0x3)";
        }
        public UInt32 CMDCFG_CallBack_3(List<UInt32> hPara)
        {
            if (hPara[0] != 0xFF)
                return hPara[0];
            myESMConfigMathADXL357.ConfigDecodeWord(hPara[1]);
            myESMConfigMathADXL372.ConfigDecodeWord(hPara[2]);
            myESMConfigMathBMG250.ConfigDecodeWord(hPara[3]);
            return (0xFF);
        }
        //-----------------------------------------------------------------CMDCFG_Read_E0
        public string CMDCFG_Read_E0()          // +CMDCFGS(0xE0) : Number or write cycle burned. //-CMDCFG(0xFF;0x002) response with number of write cycle done. >1000 = risk of issue.
        {
            return "+CMDCFGS(0xE0)";
        }
        public UInt32 CMDCFG_CallBack_E0(List<UInt32> hPara)
        {
            if (hPara[0] != 0xFF)
                return hPara[0];
            myWriteCycle = hPara[1];
            return (0xFF);
        }
        //-----------------------------------------------------------------CMDCFG_CMD_F0
        public string CMDCFG_CMD_F0()          // Reset Default, OSMath/Config Only							                    -CMDCFG(0xFF) = OKAY, 0xXX = Error code
        {
            return "+CMDCFGS(0xF0)";
        }
        //-----------------------------------------------------------------CMDCFG_Write10
        public string CMDCFG_Write10()         // +CMDCFGS(0x10;0xD;0xD) : Write ESMSConfigSensor ADXL357(ODR and HFP)          -CMDCFG(0xFF) = OKAY, 0xXX = Error code
        {
            myESMConfigSensor.ConfigGenerateWord();
            string sCMD = "+CMDCFGS(0x10;";
            sCMD += "0x" + myESMConfigSensor.ADXL357_ODR.ToString("X") + ";";
            sCMD += "0x" + myESMConfigSensor.ADXL357_HPF.ToString("X") + ")\n";
            return sCMD;
        }
        //-----------------------------------------------------------------CMDCFG_Write11
        public string CMDCFG_Write11()         //+CMDCFGS(0x11;0xXXXXXXXX) : Write ESMConfigMathADXL357                         -CMDCFG(0xFF) = OKAY, 0xXX = Error code
        {
            myESMConfigMathADXL357.ConfigGenerateWord();
            string sCMD = "+CMDCFGS(0x11;";
            sCMD += "0x" + myESMConfigMathADXL357.ESMConfigMathADXL357_Word.ToString("X") + ")\n";
            return sCMD;
        }
        //-----------------------------------------------------------------CMDCFG_Write12
        public string CMDCFG_Write12()         //+CMDCFGS(0x12;0xD)	: Write ESMSConfigSensor ADXL357(Range 40/20/10G)           -CMDCFG(0xFF) = OKAY, 0xXX = Error code, default 40G
        {
            myESMConfigSensor.ConfigGenerateWord();
            string sCMD = "+CMDCFGS(0x12;";
            sCMD += "0x" + myESMConfigSensor.ADXL357_RANGE.ToString("X") + ")\n";
            return sCMD;
        }
        //-----------------------------------------------------------------CMDCFG_Write13
        public string CMDCFG_Write13()         //+CMDCFGS(0x13;0xXXXXXXXX)	: Write ESMConfigINACTQualADXL357, INACT Qualifier	-CMDCFG(0xFF) = OKAY
        {
            myESMConfigINACTQualADXL357.ConfigGenerateWord();
            string sCMD = "+CMDCFGS(0x13;";
            sCMD += "0x" + myESMConfigINACTQualADXL357.ESMConfigINACTQualADXL357_Word.ToString("X") + ")\n";
            return sCMD;
        }
        //-----------------------------------------------------------------CMDCFG_Write14
        public string CMDCFG_Write14()         //+CMDCFGS(0x14;0xXXXXXXXX)	: Write ESMConfigACTQualADXL357, ACT Qualifier		-CMDCFG(0xFF) = OKAY,
        {
            myESMConfigACTQualADXL357.ConfigGenerateWord();
            string sCMD = "+CMDCFGS(0x14;";
            sCMD += "0x" + myESMConfigACTQualADXL357.ESMConfigACTQualADXL357_Word.ToString("X") + ")\n";
            return sCMD;
        }
        //-----------------------------------------------------------------CMDCFG_Write20
        public string CMDCFG_Write20()         //+CMDCFGS(0x20;0xD;0xD;0xD)			: Write ESMSConfigSensorADXL372				-CMDCFG(0xFF) = OKAY,
        {
            myESMConfigSensorADXL372.ConfigGenerateWord();
            string sCMD = "+CMDCFGS(0x20;";
            sCMD += "0x" + myESMConfigSensorADXL372.ADXL372_ODR.ToString("X") + ";";
            sCMD += "0x" + myESMConfigSensorADXL372.ADXL372_BW.ToString("X") + ";";
            sCMD += "0x" + myESMConfigSensorADXL372.ADXL372_TH.ToString("X") + ")\n";
            return sCMD;
        }
        //-----------------------------------------------------------------CMDCFG_Write21
        public string CMDCFG_Write21()         //+CMDCFGS(0x21;0xXXXXXXXX)				: Write ESMConfigMathADXL372 			-CMDCFG(0xFF) = OKAY,
        {
            myESMConfigMathADXL372.ConfigGenerateWord();
            string sCMD = "+CMDCFGS(0x21;";
            sCMD += "0x" + myESMConfigMathADXL372.ESMConfigMathADXL372_Word.ToString("X") + ")\n";
            return sCMD;
        }
        //-----------------------------------------------------------------CMDCFG_Write22
        public string CMDCFG_Write22()         //+CMDCFGS(0x22;0xXXXXXXXX)				: Write ESMConfigACTQualADXL372 		-CMDCFG(0xFF) = OKAY,
        {
            myESMConfigACTQualADXL372.ConfigGenerateWord();
            string sCMD = "+CMDCFGS(0x22;";
            sCMD += "0x" + myESMConfigACTQualADXL372.ESMConfigACTQualADXL372_Word.ToString("X") + ")\n";
            return sCMD;
        }
        //-----------------------------------------------------------------CMDCFG_Write30
        public string CMDCFG_Write30()         //+CMDCFGS(0x30;0xD;0xD)				: Write ESMSConfigSensor BMG250 (ODR and BW)-CMDCFG(0xFF) = OKAY, 0xXX = Error code
        {
            myESMConfigSensor.ConfigGenerateWord();
            string sCMD = "+CMDCFGS(0x30;";
            sCMD += "0x" + myESMConfigSensor.BMG250_ODR.ToString("X") + ";";
            sCMD += "0x" + myESMConfigSensor.BMG250_BWP.ToString("X") + ")\n";
            return sCMD;
        }
        //-----------------------------------------------------------------CMDCFG_Write31
        public string CMDCFG_Write31()         //+CMDCFGS(0x31;0xXXXXXXXX)				: ESMConfigMathBMG250  for BMG250		-CMDCFG(0xFF) = OKAY, 0xXX = Error code
        {
            myESMConfigMathBMG250.ConfigGenerateWord();
            string sCMD = "+CMDCFGS(0x31;";
            sCMD += "0x" + myESMConfigMathBMG250.ESMConfigMathBMG250_Word.ToString("X") + ")\n";
            return sCMD;
        }
        //----------------------------------------------------------------- CMDCFG_Write90 Write all Config Data
        public string CMDCFG_Write90()         //+CMDCFGS(0x90;ESMConfigSensor;ESMConfigSensorADXL372;ESMConfigACTQualADXL372;ESMConfigMathADXL357;ESMConfigMathBMG250;ESMConfigMathADXL372;ESMConfigINACTQualADXL357;ESMConfigACTQualADXL357)				
        {
            //------------------------------------------------------------------------------------------------
            myESMConfigSensor.ConfigGenerateWord();
            myESMConfigSensorADXL372.ConfigGenerateWord();
            myESMConfigACTQualADXL372.ConfigGenerateWord();
            myESMConfigMathADXL357.ConfigGenerateWord();
            myESMConfigMathADXL372.ConfigGenerateWord();
            myESMConfigMathBMG250.ConfigGenerateWord();
            myESMConfigINACTQualADXL357.ConfigGenerateWord();
            myESMConfigACTQualADXL357.ConfigGenerateWord();
            //------------------------------------------------------------------------------------------------
            string sCMD = "+CMDCFGS(0x90;";
            sCMD += "0x" + myESMConfigSensor.ESMConfigSensor_Word.ToString("X") + ";";
            sCMD += "0x" + myESMConfigSensorADXL372.ESMConfigSensorADXL372_Word.ToString("X") + ";";
            sCMD += "0x" + myESMConfigACTQualADXL372.ESMConfigACTQualADXL372_Word.ToString("X") + ";";
            sCMD += "0x" + myESMConfigMathADXL357.ESMConfigMathADXL357_Word.ToString("X") + ";";
            sCMD += "0x" + myESMConfigMathBMG250.ESMConfigMathBMG250_Word.ToString("X") + ";";
            sCMD += "0x" + myESMConfigMathADXL372.ESMConfigMathADXL372_Word.ToString("X") + ";";
            sCMD += "0x" + myESMConfigINACTQualADXL357.ESMConfigINACTQualADXL357_Word.ToString("X") + ";";
            sCMD += "0x" + myESMConfigACTQualADXL357.ESMConfigACTQualADXL357_Word.ToString("X") + ")\n";
            return sCMD;
            //------------------------------------------------------------------------------------------------
        }
        //----------------------------------------------------------------- CMDCFG_Write91 REad All Config Data. 
        public string CMDCFG_Write91()         //+CMDCFGS(0x91)  -CMDCFGS(0xFF;ESMConfigSensor;ESMConfigSensorADXL372;ESMConfigACTQualADXL372;ESMConfigMathBMG250;ESMConfigMathADXL357;ESMConfigMathADXL372;ESMConfigINACTQualADXL357;ESMConfigACTQualADXL357)				
        {
            return "+CMDCFGS(0x91)";
        }
        public UInt32 CMDCFG_CallBack_91(List<UInt32> hPara)
        {
            if (hPara[0] != 0xFF)
                return hPara[0];
            if (hPara.Count<8)      // read issue. 
                return hPara[0];
            myESMConfigSensor.ConfigDecodeWord(hPara[1]);
            myESMConfigSensorADXL372.ConfigDecodeWord(hPara[2]);
            myESMConfigACTQualADXL372.ConfigDecodeWord(hPara[3]);
            myESMConfigMathADXL357.ConfigDecodeWord(hPara[4]);
            myESMConfigMathBMG250.ConfigDecodeWord(hPara[5]);
            myESMConfigMathADXL372.ConfigDecodeWord(hPara[6]);
            myESMConfigINACTQualADXL357.ConfigDecodeWord(hPara[7]);
            myESMConfigACTQualADXL357.ConfigDecodeWord(hPara[8]);
            return (0xFF);
        }
    }

    #endregion

    #region//=================================================================================ESM Block/Seq Configuration
    //==================================================================
    //================================================================== 
    // Purpose	: This update the configuration in MCU Flash (including write process).
    // Input	: iPm[0] = Config type, iPm[1] = Config Data
    // Note 	: +CMDCFGS(0x41)						: Read Setup ESMConfigQ 									-CMDCFG(0xFF;ESMConfigQ)
    // Note 	: +CMDCFGS(0x42)					    : Read Setup ESMMSeqBlock0-ESMMSeqBlock3 					-CMDCFG(0xFF;ESMMSeqBlock0;ESMMSeqBlock1;ESMMSeqBlock2;ESMMSeqBlock3)
    // Note 	: +CMDCFGS(0x43;Start;End)				: Read Setup ESMConfigSeqNo 								-CMDCFG(0xFF;ESMConfigSeqNo;ESMConfigSeqNo;ESMConfigSeqNo......ESMConfigSeqNo)
    // Note 	: +CMDCFGS(0x51;ESMMSeqBlock0;1;2;3)	: Write Setup ESMMSeqBlock0-ESMMSeqBlock3 					-CMDCFG(0xFF)       Max 4 Block
    // Note 	: +CMDCFGS(0x52;ESMConfigSeqNo...)		: Write Setup ESMConfigSeqNo 0 to 15 (Basic Design) 		-CMDCFG(0xFF)       Max 16 SeqNo or 8 Words (odd/even)
    //			: +CMDCFGS(0x7E)						: Number or write cycle burned.								-CMDCFG(0xFF;0x002) response with number of write cycle done. >1000 = risk of issue.
    //			: +CMDCFGS(0x7F)						: Reset Default, Qualifier/Sequencer Only
    [Serializable]
    public class ESM_NVM_BlockSeqConfigII
    {
        int NoOfBlock = 4;
        int NoOfSeq = 8*2;      // 8 Word but include 2 x 16 odd/even SeqNo so it 16 SeqNo Actual.
        public ESMConfigQ myESMConfigQ;
        public List<ESMConfigMasterBlock> myESMMasterBlock;
        public List<ESMConfigSeqNo16> myESMSeNo;
        public UInt32 myWriteCycle { get; set; }
        public string ErrorCode { get; set; }
        public ESM_NVM_BlockSeqConfigII()
        {
            myESMConfigQ = new ESMConfigQ();
            myESMMasterBlock = new List<ESMConfigMasterBlock>();
            myESMSeNo = new List<ESMConfigSeqNo16>();

            myWriteCycle = 0;
            ErrorCode = "";
        }

        public void InitDefault()
        {
            myESMConfigQ.ConfigDecodeWord(0x1E000010);                  // 30 Second, only Block 0/1 run via Periodic
            //-------------------------------------------------------------
            myESMMasterBlock.Clear();
            for (int i=0; i< NoOfBlock; i++)
            {
                myESMMasterBlock.Add(new ESMConfigMasterBlock());
            }
            myESMMasterBlock[0].ConfigDecodeWord(0x00001401);
            myESMMasterBlock[1].ConfigDecodeWord(0x02001402);
            myESMMasterBlock[2].ConfigDecodeWord(0x04001401);           // Should be 1 not 2 at begin.
            myESMMasterBlock[3].ConfigDecodeWord(0x06001402);           // Should be 1 not 2 at begin.
            //-------------------------------------------------------------
            myESMSeNo.Clear();
            for (int i = 0; i < NoOfSeq; i++)
            {
                myESMSeNo.Add(new ESMConfigSeqNo16());
            }
            myESMSeNo[0].ConfigDecodeWord(0x0203);      //Vibration 3ON-3OFF-3ON-3OFF Finish
            myESMSeNo[1].ConfigDecodeWord(0x0003);
            myESMSeNo[2].ConfigDecodeWord(0x0203);
            myESMSeNo[3].ConfigDecodeWord(0x0000);
            myESMSeNo[4].ConfigDecodeWord(0x0203);      //Gyro 3ON-3OFF-3ON-3OFF Finish
            myESMSeNo[5].ConfigDecodeWord(0x0003);
            myESMSeNo[6].ConfigDecodeWord(0x0203);
            myESMSeNo[7].ConfigDecodeWord(0x0000);
            for (int i = 8; i < NoOfSeq; i++)
            {
                myESMSeNo[i].ConfigDecodeWord(0);
            }
        }

        //-----------------------------------------------------------------CMDCFG_GenericCallBack
        public UInt32 CMDCFG_GenericCallBack(List<UInt32> hPara)
        {
            ErrorCode = "";
            if (hPara[0] != 0xFF)
            {
                switch (hPara[0])
                {
                    case 0x11:
                        ErrorCode = "Parameter Error: ADXL357_ODR (0 to 10)";
                        break;
                    case 0x12:
                        ErrorCode = "Parameter Error: ADXL357_HPF (0 to 6)";
                        break;
                    case 0x1F:
                        ErrorCode = "Parameter Error: ConfigOSMath Not for ADXL357 Device";
                        break;
                    case 0x15:
                        ErrorCode = "Parameter Error: ADXL357 Range (1 to 3)";
                        break;
                    case 0x13:
                        ErrorCode = "Parameter Error: INACT ADXL372 Threshold (1 to 125)";
                        break;
                    case 0x14:
                        ErrorCode = "Parameter Error: ACT ADXL357 Threshold (1 to 125)";
                        break;
                    case 0x21:
                        ErrorCode = "Parameter Error: ADXL372_ODR  (0 to 5)";
                        break;
                    case 0x22:
                        ErrorCode = "Parameter Error: ADXL372_BW  (0 to 5)";
                        break;
                    case 0x23:
                        ErrorCode = "Parameter Error: ADXL372_TH  (0 to 1)";
                        break;
                    case 0x2F:
                        ErrorCode = "Parameter Error: ConfigOSMath Not for ADXL372 Device";
                        break;
                    case 0x31:
                        ErrorCode = "Parameter Error: BMG250_ODR  (6 to 13)";
                        break;
                    case 0x32:
                        ErrorCode = "Parameter Error: BMG250_BWP  (0 to 2)";
                        break;
                    case 0x3F:
                        ErrorCode = "Parameter Error: ConfigOSMath Not for BMG250 Device";
                        break;
                    case 0x51:
                        ErrorCode = "Parameter Error: Block must be within 0 to 3 for this revision";
                        break;
                    case 0x52:
                        ErrorCode = "Parameter Error: SeqNo must be within 0 to 7 for this revision";
                        break;
                    case 0x9F:
                        ErrorCode = "Parameter Error: !Write NVM Failure!";
                        break;
                    default:
                        ErrorCode = "Parameter Error: Misc/Generic or 0x00";
                        break;

                }
                return hPara[0];
            }

            else
                return 0xFF;
        }
        //-----------------------------------------------------------------CMDCFG_Read_1
        public string CMDCFG_Read_41()          //+CMDCFGS(0x41) : Read Setup ESMConfigQ 	// -CMDCFG(0xFF; ESMConfigQ)
        {
            return "+CMDCFGS(0x41)";
        }
        public UInt32 CMDCFG_CallBack_41(List<UInt32> hPara)
        {
            try
            {
                if (hPara[0] != 0xFF)
                    return hPara[0];
                myESMConfigQ.ConfigDecodeWord(hPara[1]);
            }
            catch
            {
                return (0xFE);
            }
            return (0xFF);
        }

        //-----------------------------------------------------------------CMDCFG_Read_42
        public string CMDCFG_Read_42()           //+CMDCFGS(0x42) : Read Setup ESMMSeqBlock0-ESMMSeqBlock3 	// -CMDCFG(0xFF; ESMMSeqBlock0;ESMMSeqBlock1;ESMMSeqBlock2;ESMMSeqBlock3)
        {
            return "+CMDCFGS(0x42;4)";
        }
        public UInt32 CMDCFG_CallBack_42(List<UInt32> hPara)
        {
            try
            {
                //----------------------------------------------------
                if (hPara[0] != 0xFF)
                    return hPara[0];
                myESMMasterBlock[0].ConfigDecodeWord(0);
                myESMMasterBlock[1].ConfigDecodeWord(0);
                myESMMasterBlock[2].ConfigDecodeWord(0);
                myESMMasterBlock[3].ConfigDecodeWord(0);

                myESMMasterBlock[0].ConfigDecodeWord(hPara[1]);
                myESMMasterBlock[1].ConfigDecodeWord(hPara[2]);
                myESMMasterBlock[2].ConfigDecodeWord(hPara[3]);
                myESMMasterBlock[3].ConfigDecodeWord(hPara[4]);
            }
            catch
            {
                //myMainProg.myRtbTermMessageLF("#E: CMDCFG_CallBack_42: Missing hPara");
                return (0xFE);
            }
            //----------------------------------------------------
            return (0xFF);
        }

        
        //-----------------------------------------------------------------CMDCFG_Read_43
        public string CMDCFG_Read_43()          // +CMDCFGS(0x43;Start;End)	: Read Setup ESMConfigSeqNo // -CMDCFG(0xFF;ESMConfigSeqNo;ESMConfigSeqNo;ESMConfigSeqNo......ESMConfigSeqNo)
        {
            return "+CMDCFGS(0x43;0;3)";
        }
        public UInt32 CMDCFG_CallBack_43(List<UInt32> hPara)
        {
            if (hPara[0] != 0xFF)
                return hPara[0];
            try
            {
                //---------------------------------------Clear All
                for (int i = 0; i < NoOfSeq; i++)
                {
                    myESMSeNo[i].ConfigDecodeWord(0);
                }
                //---------------------------------------Update
                myESMSeNo[0].ConfigDecodeWord((UInt16)(hPara[1] & 0x0000FFFF));
                myESMSeNo[1].ConfigDecodeWord((UInt16)(((hPara[1] >> 16) & 0x0000FFFF)));
                myESMSeNo[2].ConfigDecodeWord((UInt16)(hPara[2] & 0x0000FFFF));
                myESMSeNo[3].ConfigDecodeWord((UInt16)(((hPara[2] >> 16) & 0x0000FFFF)));
                myESMSeNo[4].ConfigDecodeWord((UInt16)(hPara[3] & 0x0000FFFF));
                myESMSeNo[5].ConfigDecodeWord((UInt16)(((hPara[3] >> 16) & 0x0000FFFF)));
                myESMSeNo[6].ConfigDecodeWord((UInt16)(hPara[4] & 0x0000FFFF));
                myESMSeNo[7].ConfigDecodeWord((UInt16)(((hPara[4] >> 16) & 0x0000FFFF)));
                return (0xFF);
            }
            catch
            {
                //myMainProg.myRtbTermMessageLF("#E: Internal Issue in CMDCFG_CallBack_43: Missing hPara array");
                return (0xFE);
            }
        }

        //-----------------------------------------------------------------CMDCFG_Write51: Page 1 Write (Whole, include ConfigQ and Block. 
        public string CMDCFG_Write51()       //+CMDCFGS(0x51; 0xESMConfigQ;Block0;Block1;Block2;Block3.)	
        {
            //------------------------------------------------
            myESMConfigQ.ConfigGenerateWord();
            myESMMasterBlock[0].ConfigGenerateWord();
            myESMMasterBlock[1].ConfigGenerateWord();
            myESMMasterBlock[2].ConfigGenerateWord();
            myESMMasterBlock[3].ConfigGenerateWord();
            //------------------------------------------------
            string sCMD = "+CMDCFGS(0x51;";
            sCMD += "0x" + myESMConfigQ.ESMConfigQ_Word.ToString("X")+";";
            sCMD += "0x" + myESMMasterBlock[0].ESMConfigMasterBlock_Word.ToString("X8") + ";";
            sCMD += "0x" + myESMMasterBlock[1].ESMConfigMasterBlock_Word.ToString("X8") + ";";
            sCMD += "0x" + myESMMasterBlock[2].ESMConfigMasterBlock_Word.ToString("X8") + ";";
            sCMD += "0x" + myESMMasterBlock[3].ESMConfigMasterBlock_Word.ToString("X8") + ";";
            sCMD += ")\n";
            return sCMD;
        }

        //-----------------------------------------------------------------CMDCFG_Write52
        public string CMDCFG_Write52()        //+CMDCFGS(0x52;ESMConfigSeqNo0;1;2;etc): Write Setup ESMConfigSeqNo 0 to 15 (Whole Page) 	// -CMDCFG(0xFF)
        {
            int i = 0;
            for (i=0; i< 8; i++)
            {
                myESMSeNo[i].ConfigGenerateWord();
            }
            string sCMD = "+CMDCFGS(0x52;";
            i = 0;
            while (i<8)
            {
                string sEven = myESMSeNo[i].ESMConfigSeqNo16_Word.ToString("X4");
                string sOdd = myESMSeNo[i+1].ESMConfigSeqNo16_Word.ToString("X4");
                sCMD += "0x" + sOdd+sEven + ";";
                i += 2;
            }
            sCMD += ")\n";
            return sCMD;
        }

        //-----------------------------------------------------------------CMDCFG_Read_7E
        public string CMDCFG_Read_7E()                  //: Number or write cycle burned.	-CMDCFG(0xFF;0x002) response with number of write cycle done. >1000 = risk of issue.
        {
            return "+CMDCFGS(0x7E)";
        }
        public UInt32 CMDCFG_CallBack_7E(List<UInt32> hPara)
        {
            if (hPara[0] != 0xFF)
                return hPara[0];
            myWriteCycle = hPara[1];
            return (0xFF);
        }
        //-----------------------------------------------------------------CMDCFG_CMD_7F
        public string CMDCFG_CMD_7F()          // Reset Default, Qualifier/Sequencer Only
        {
            return "+CMDCFGS(0x7F)";
        }
    }

    #endregion

    #region//=================================================================================ESMConfigSensor
    //=============================================================================== ESMConfigSensor
    //     typedef struct tagzESMConfigSensor
    //     {
    //         union  {
    // 		uINT32 zWord;                               // 32 BITS
    //         struct  {									//			Description											Default if page is blank	Enum Ref
    //         uINT32 ADXL357_ODR:4;                   //(0-3)		ADXL357 ODR = Sample Rate, Limit to 0 to 10.		Default to 2 (1000Hz)		ADXL357ODRSetting
    //         uINT32 ADXL357_HPF:4;                   //(4-7)		ADXL357 HPF = High Pass filter.	Limit to 0 to 6.	Default to 0 (DC)			ADXL357HPFSetting
    //                                                 //----------------------BMG250
    //         uINT32 BMG250_ODR:4;                    //(8-11) 	BMG250 ODR = Sample Rate, Limit to 6 to 13.			Default to 11 (B) (800Hz)	BMG250ODRSetting
    //         uINT32 BMG250_BWP:4;                    //(12-15)	BMG250 BWP = BW-Filter, Limit to 0 to 2.			Default to 0. (Normal)		BMG250BWPSetting
    //                                                 //----------------------MISC
    //         uINT32 ADXL357_RANGE:2;                 //(16-17)	ADXL357 RANGE, 1 = 10G, 2 = 20G, 3 = 40G			Default to 3 (40G)			update to ADXL357Sensor.Range
    //         uINT32 BMG250_FORMAT:2;                 //(18-19)	BMG250 OutFormat 0 = Raw, 1 = deg/Sec 2 = rpm		Default to 2 (RPM)			
    //         uINT32 SpareS2:4;                       //(20-23)
    //         uINT32 SpareS3:4;                       //(24-27)
    //         uINT32 SpareS4:4;                       //(28-31)
    //          };
    //      };
    // } _ESMConfigSensor;

    [Serializable]
    public class ESMConfigSensor
    {
        ITools Tools = new Tools();
        public UInt32 ESMConfigSensor_Word { get; set; }
        public UInt32 ADXL357_ODR { get; set; }
        int ADXL357_ODR_Offset = 0;  //int ADXL357_ODR_Width = 4;
        public UInt32 ADXL357_HPF { get; set; }
        int ADXL357_HPF_Offset = 4;  //int ADXL357_HPF_Width = 4;
        public UInt32 BMG250_ODR { get; set; }
        int BMG250_ODR_Offset = 8; //int BMG250_ODR_Width = 4;
        public UInt32 BMG250_BWP { get; set; }
        int BMG250_BWP_Offset = 12; //int BMG250_BWP_Width = 4;
        public UInt32 ADXL357_RANGE { get; set; }
        int ADXL357_RANGE_Offset = 16; //int ADXL357_RANGE_Width = 2;
        public UInt32 BMG250_FORMAT { get; set; }
        int BMG250_FORMAT_Offset = 18; // int BMG250_FORMAT_Width = 2;

        public ESMConfigSensor()
        {
            ESMConfigSensor_Word = 0;
        }

        public void ConfigGenerateWord()
        {
            ESMConfigSensor_Word = 0;
            ESMConfigSensor_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigSensor_Word, ADXL357_ODR_Offset, ADXL357_ODR);
            ESMConfigSensor_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigSensor_Word, ADXL357_HPF_Offset, ADXL357_HPF);
            ESMConfigSensor_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigSensor_Word, BMG250_ODR_Offset, BMG250_ODR);
            ESMConfigSensor_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigSensor_Word, BMG250_BWP_Offset, BMG250_BWP);
            ESMConfigSensor_Word = Tools.Bits_UInt32_intWrite2Bit(ESMConfigSensor_Word, ADXL357_RANGE_Offset, ADXL357_RANGE);
            ESMConfigSensor_Word = Tools.Bits_UInt32_intWrite2Bit(ESMConfigSensor_Word, BMG250_FORMAT_Offset, BMG250_FORMAT);
        }

        public void ConfigDecodeWord(UInt32 uData32)
        {
            ADXL357_ODR = Tools.Bits_UInt32_uRead4Bit(uData32, ADXL357_ODR_Offset);
            ADXL357_HPF = Tools.Bits_UInt32_uRead4Bit(uData32, ADXL357_HPF_Offset);
            BMG250_ODR = Tools.Bits_UInt32_uRead4Bit(uData32, BMG250_ODR_Offset);
            BMG250_BWP = Tools.Bits_UInt32_uRead4Bit(uData32, BMG250_BWP_Offset);
            ADXL357_RANGE = Tools.Bits_UInt32_uRead2Bit(uData32, ADXL357_RANGE_Offset);
            BMG250_FORMAT = Tools.Bits_UInt32_uRead2Bit(uData32, BMG250_FORMAT_Offset);
            ConfigGenerateWord();
        }
    }
    #endregion

    #region//=================================================================================ESMConfigSensorADXL372
    // //--------------------------------------------------------------------------------------------------------ADXL372 and ACT/INACT Qualifier.
    // typedef struct tagzESMConfigSensorADXL372
    // {
    //     union  {
    // 		uINT32 zWord;                               // 32 BITS
    //     struct  {									//			Description											Default if page is blank	Enum Ref
    //     uINT32 ADXL372_ODR:4;                   //(0-3)		ADXL372_ODR = ODR 400/800/1600/3200/6400			(Select 3200 = 3)			ADXL372_ODR
    //     uINT32 ADXL372_BW:4;                    //(4-7)		ADXL372_BW = BW 200/400/800/1600/3200				(Select 1600 = 3)			ADXL372_BW
    //     uINT32 ADXL372_TH:4;                    //(8-11)	ADXL372_TH = 0: Low G (10 to 15), High G (15 to 30), Default 1
    //     uINT32 Spare4A:4;                       //(12-15)	
    //     uINT32 Spare4B:4;                       //(16-19)	
    //     uINT32 SpareB12:12;                     //(20-31)	
    // };
    // 	};
    // } _ESMConfigSensorADXL372;

    [Serializable]
    public class ESMConfigSensorADXL372
    {
        ITools Tools = new Tools();
        public UInt32 ESMConfigSensorADXL372_Word { get; set; }
        public UInt32 ADXL372_ODR { get; set; }
        int ADXL372_ODR_Offset = 0; //int ADXL372_ODR_Width = 4;
        public UInt32 ADXL372_BW { get; set; }
        int ADXL372_BW_Offset = 4; //int ADXL372_BW_Width = 4;
        public UInt32 ADXL372_TH { get; set; }
        int ADXL372_TH_Offset = 8; //int ADXL372_TH_Width = 4;

        public ESMConfigSensorADXL372()
        {
            ESMConfigSensorADXL372_Word = 0;
        }

        public void ConfigGenerateWord()
        {
            ESMConfigSensorADXL372_Word = 0;
            ESMConfigSensorADXL372_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigSensorADXL372_Word, ADXL372_ODR_Offset, ADXL372_ODR);
            ESMConfigSensorADXL372_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigSensorADXL372_Word, ADXL372_BW_Offset, ADXL372_BW);
            ESMConfigSensorADXL372_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigSensorADXL372_Word, ADXL372_TH_Offset, ADXL372_TH);
        }

        public void ConfigDecodeWord(UInt32 uData32)
        {
            ADXL372_ODR = Tools.Bits_UInt32_uRead4Bit(uData32, ADXL372_ODR_Offset);
            ADXL372_BW = Tools.Bits_UInt32_uRead4Bit(uData32, ADXL372_BW_Offset);
            ADXL372_TH = Tools.Bits_UInt32_uRead4Bit(uData32, ADXL372_TH_Offset);
            ConfigGenerateWord();
        }
    }
    #endregion

    #region//=================================================================================ESMConfigMathADXL357
    // //--------------------------------------------------------------------------------------------------------OS Math ADXL572
    // typedef struct tagzESMConfigMathADXL357
    // {
    //     union  {
    // 		uINT32 zWord;                               // 32 BITS
    //     struct  {
    //     uINT32 ADXL357_PVFS:16;                      //(0-15)			// Sample Freq for ADXL357,set 0 to disable. Default: 100Hz		(from 1000Hz Sensor ODR)
    //     _ConfigOSMath OSMathConfig;                  //(16-31)		
    // };
    // 	};
    // } _ESMConfigMathADXL357;
    // static const uINT32 ESMConfigMathADXL357Default = 0x6A010064;		// See ESM_SensorMath.h for CongigMath setting. 011

    [Serializable]
    public class ESMConfigMathADXL357
    {
        ITools Tools = new Tools();
        public UInt32 ESMConfigMathADXL357_Word { get; set; }
        public UInt32 ADXL357_PVFS { get; set; }
        int ADXL357_PVFS_Offset = 0; //int ADXL357_PVFS_Width = 16;
        public UInt32 OSMathConfig { get; set; }
        int OSMathConfig_Offset = 16;// int OSMathConfig_Width = 16;

        public ESMConfigMathADXL357()
        {
            ESMConfigMathADXL357_Word = 0x08000000;
        }

        public void ConfigGenerateWord()
        {
            ESMConfigMathADXL357_Word = 0x08000000;
            ESMConfigMathADXL357_Word |= Tools.Bits_UInt32_intWrite16Bit(ESMConfigMathADXL357_Word, ADXL357_PVFS_Offset, ADXL357_PVFS);
            ESMConfigMathADXL357_Word |= Tools.Bits_UInt32_intWrite16Bit(ESMConfigMathADXL357_Word, OSMathConfig_Offset, OSMathConfig);
        }

        public void ConfigDecodeWord(UInt32 uData32)
        {
            ADXL357_PVFS = Tools.Bits_UInt32_uRead16Bit(uData32, ADXL357_PVFS_Offset);
            OSMathConfig = Tools.Bits_UInt32_uRead16Bit(uData32, OSMathConfig_Offset);
            ConfigGenerateWord();
        }

        #region //============================Read OSMathConfig16
        public bool isAVG()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 0));
        }
        public bool isRMS()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 1));
        }
        public bool isFFT()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 2));
        }
        public bool isMin()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 3));
        }
        public bool isMax()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 4));
        }
        public bool isFirst()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 5));
        }
        public bool isData16()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 6));
        }
        public bool isHFStream()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 8));
        }
        public UInt16 ReadSelectFormat()
        {
            return (UInt16)(OSMathConfig >> 9 & 0x00000003);
        }
        public UInt16 ReadSensorType()
        {
            return (UInt16)(OSMathConfig >> 11 & 0x00000003);
        }
        public UInt16 ReadAssignedData16()
        {
            return (UInt16)(OSMathConfig >> 13 & 0x00000007);
        }
        #endregion

        #region //============================Write OSMathConfig16
        public void WriteAVG(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 0, state);
        }
        public void WriteRMS(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 1, state);
        }
        public void WriteFFT(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 2, state);
        }
        public void WriteMin(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 3, state);
        }
        public void WriteMax(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 4, state);
        }
        public void WriteFirst(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 5, state);
        }
        public void WriteData16(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 6, state);
        }
        public void WriteHFStream(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 8, state);
        }
        public void WriteSelectFormat(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite2Bit(OSMathConfig, 9, state);
            // return (UInt16)(OSMathConfig >> 9 & 0x00000003);
        }
        public void WriteSensorType(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite2Bit(OSMathConfig, 11, state);
            // return (UInt16)(OSMathConfig >> 11 & 0x00000003);
        }
        public void WriteAssignedData16(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite3Bit(OSMathConfig, 13, state);
            //return (UInt16)(OSMathConfig >> 13 & 0x00000007);
        }
        #endregion
    }
    #endregion

    #region//=================================================================================ESMConfigMathBMG250
    // //--------------------------------------------------------------------------------------------------------OS Math BMG250
    // typedef struct tagzESMConfigMathBMG250
    // {
    //     union  {
    // 		uINT32 zWord;                               // 32 BITS
    //     struct  {
    //     uINT32 BMG250_PGFS:16;                       //(0-15)			// Sample Freq for ADXL372, set 0 to disable: Default 100Hz		(from 800Hz Sensor ODR)
    //     _ConfigOSMath OSMathConfig;
    // };
    // 	};
    // } _ESMConfigMathBMG250;
    // static const uINT32 ESMConfigMathBMG250Default = 0x7A010064;		// See ESM_SensorMath.h for CongigMath setting. 
    [Serializable]
    public class ESMConfigMathBMG250
    {
        ITools Tools = new Tools();
        public UInt32 ESMConfigMathBMG250_Word { get; set; }
        public UInt32 BMG250_PGFS { get; set; }
        int BMG250_PGFS_Offset = 0; //int BMG250_PGFS_Width = 16;
        public UInt32 OSMathConfig { get; set; }
        int OSMathConfig_Offset = 16; //int OSMathConfig_Width = 16;

        public ESMConfigMathBMG250()
        {
            ESMConfigMathBMG250_Word = 0x18000000;
        }

        public void ConfigGenerateWord()
        {
            ESMConfigMathBMG250_Word = 0x18000000;
            ESMConfigMathBMG250_Word |= Tools.Bits_UInt32_intWrite16Bit(ESMConfigMathBMG250_Word, BMG250_PGFS_Offset, BMG250_PGFS);
            ESMConfigMathBMG250_Word |= Tools.Bits_UInt32_intWrite16Bit(ESMConfigMathBMG250_Word, OSMathConfig_Offset, OSMathConfig);
        }

        public void ConfigDecodeWord(UInt32 uData32)
        {
            BMG250_PGFS = Tools.Bits_UInt32_uRead16Bit(uData32, BMG250_PGFS_Offset);
            OSMathConfig = Tools.Bits_UInt32_uRead16Bit(uData32, OSMathConfig_Offset);
            ConfigGenerateWord();
        }

        #region //============================Read OSMathConfig16
        public bool isAVG()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 0));
        }
        public bool isRMS()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 1));
        }
        public bool isFFT()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 2));
        }
        public bool isMin()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 3));
        }
        public bool isMax()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 4));
        }
        public bool isFirst()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 5));
        }
        public bool isData16()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 6));
        }
        public bool isHFStream()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 8));
        }
        public UInt16 ReadSelectFormat()
        {
            return (UInt16)(OSMathConfig >> 9 & 0x00000003);
        }
        public UInt16 ReadSensorType()
        {
            return (UInt16)(OSMathConfig >> 11 & 0x00000003);
        }
        public UInt16 ReadAssignedData16()
        {
            return (UInt16)(OSMathConfig >> 13 & 0x00000007);
        }
        #endregion

        #region //============================Write OSMathConfig16
        public void WriteAVG(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 0, state);
        }
        public void WriteRMS(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 1, state);
        }
        public void WriteFFT(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 2, state);
        }
        public void WriteMin(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 3, state);
        }
        public void WriteMax(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 4, state);
        }
        public void WriteFirst(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 5, state);
        }
        public void WriteData16(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 6, state);
        }
        public void WriteHFStream(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 8, state);
        }
        public void WriteSelectFormat(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite2Bit(OSMathConfig, 9, state);
            // return (UInt16)(OSMathConfig >> 9 & 0x00000003);
        }
        public void WriteSensorType(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite2Bit(OSMathConfig, 11, state);
            // return (UInt16)(OSMathConfig >> 11 & 0x00000003);
        }
        public void WriteAssignedData16(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite3Bit(OSMathConfig, 13, state);
            //return (UInt16)(OSMathConfig >> 13 & 0x00000007);
        }
        #endregion
    }
    #endregion

    #region//=================================================================================ESMConfigMathADXL372
    // //--------------------------------------------------------------------------------------------------------OS Math ADXL372
    // typedef struct tagzESMConfigMathADXL372
    // {
    //     union  {
    // 		uINT32 zWord;                               // 32 BITS
    //     struct  {
    //     uINT32 ADXL372_SSFS:16;                      //(0-15)		// Sample Freq for ADXL372,set 0 to disable. Default: Disabled   (from 3200Hz Sensor ODR)
    //     _ConfigOSMathShock OSMathConfigShock;
    // };
    // 	};
    // } _ESMConfigMathADXL372;
    // static const uINT32 ESMConfigMathADXL372Default = 0x520A0000;		// See ESM_SensorMath.h for CongigMath setting. 
    [Serializable]
    public class ESMConfigMathADXL372
    {
        ITools Tools = new Tools();
        public UInt32 ESMConfigMathADXL372_Word { get; set; }
        public UInt32 ADXL372_SSFS { get; set; }
        int ADXL372_SSFS_Offset = 0; //int ADXL372_SSFS_Width = 16;
        public UInt32 OSMathConfig { get; set; }
        int OSMathConfig_Offset = 16; //int OSMathConfig_Width = 16;

        public ESMConfigMathADXL372()
        {
            ESMConfigMathADXL372_Word = 0x10000000;
        }

        public void ConfigGenerateWord()
        {
            ESMConfigMathADXL372_Word = 0x10000000;
            ESMConfigMathADXL372_Word |= Tools.Bits_UInt32_intWrite16Bit(ESMConfigMathADXL372_Word, ADXL372_SSFS_Offset, ADXL372_SSFS);
            ESMConfigMathADXL372_Word |= Tools.Bits_UInt32_intWrite16Bit(ESMConfigMathADXL372_Word, OSMathConfig_Offset, OSMathConfig);
        }

        public void ConfigDecodeWord(UInt32 uData32)
        {
            ADXL372_SSFS = Tools.Bits_UInt32_uRead16Bit(uData32, ADXL372_SSFS_Offset);
            OSMathConfig = Tools.Bits_UInt32_uRead16Bit(uData32, OSMathConfig_Offset);
            ConfigGenerateWord();
        }

        #region //============================Read OSMathConfig16
        public UInt16 ReadSelectRange()
        {
            return (UInt16)(OSMathConfig >> 0 & 0x00000003);
        }
        public bool isFFT()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 2));
        }
        public bool isGTotal()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 3));
        }
        public bool isMax5Peak()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 4));
        }
        public bool isMax1Peak()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 5));
        }
        public bool isData16()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 6));
        }
        public bool isHFStream()
        {
            return (Tools.Bits_UInt32_Read(OSMathConfig, 8));
        }
        public UInt16 ReadSelectFormat()
        {
            return (UInt16)(OSMathConfig >> 9 & 0x00000003);
        }
        public UInt16 ReadSensorType()
        {
            return (UInt16)(OSMathConfig >> 11 & 0x00000003);
        }
        public UInt16 ReadAssignedData16()
        {
            return (UInt16)(OSMathConfig >> 13 & 0x00000007);
        }
        #endregion

        #region //============================Write OSMathConfig16
        public void WriteSelectRange(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite2Bit(OSMathConfig, 0, state);
        }
        public void WriteFFT(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 2, state);
        }
        public void WriteGTotal(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 3, state);
        }
        public void WriteMax5Peak(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 4, state);
        }
        public void WriteMax1Peak(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 5, state);
        }
        public void WriteData16(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 6, state);
        }
        public void WriteHFStream(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite(OSMathConfig, 8, state);
        }
        public void WriteSelectFormat(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite2Bit(OSMathConfig, 9, state);
            // return (UInt16)(OSMathConfig >> 9 & 0x00000003);
        }
        public void WriteSensorType(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite2Bit(OSMathConfig, 11, state);
            // return (UInt16)(OSMathConfig >> 11 & 0x00000003);
        }
        public void WriteAssignedData16(UInt32 state)
        {
            OSMathConfig = Tools.Bits_UInt32_intWrite3Bit(OSMathConfig, 13, state);
            //return (UInt16)(OSMathConfig >> 13 & 0x00000007);
        }
        #endregion
    }
    #endregion

    #region//=================================================================================ESMConfigINACTQualADXL357
    // //--------------------------------------------------------------------------------------------------------
    // typedef struct tagzConfigINACTQualADXL357
    // {
    //     union  {
    // 		uINT32 zWord;                                   // 32 BITS
    //     struct  {													// Note:		+/-40G = +/-125 Data, 5G = 15 => 0xF.
    //     uINT32 Spare1:4;                                //(0-3)		
    //     uINT32 Spare2:4;                                //(4-7)
    //     uINT32 ConfigOp:4;                              //(8-11)		// Bit0: Enable/Disable.													Default 0x1 (enabled)
    //     uINT32 SelectMath:4;                            //(12-15)		// 0x0 = Peak Detects, 0x1 = Peak DetectII.									Default 0x0 (peak detect)
    //     uINT32 INACTThreshold:8;                        //(16-23)		// 0 to 125 Only. Threshold value which trigger INACT (less than value).	Default 5G=>0xF, so 0x0F 
    //     uINT32 INACTCBuffLength:8;                      //(24-31)		// CBuffer Threshold from the start, which process the INACT qualifier.		Default 0x0F
    // };
    // 	};
    // } _ESMConfigINACTQualADXL357;
    // static const uINT32 ESMConfigINACTQualADXL357Default = 0x0F0F0100;		// Turn on INACT feature.	
    // //static const uINT32 ESMConfigINACTQualADXL357Default = 0x0F0F0000;	// Turn off INACT feature
    [Serializable]
    public class ESMConfigINACTQualADXL357
    {
        ITools Tools = new Tools();
        public UInt32 ESMConfigINACTQualADXL357_Word { get; set; }
        public UInt32 ConfigOp { get; set; }
        int ConfigOp_Offset = 8; //int ConfigOp_Width = 4;
        public UInt32 SelectMath { get; set; }
        int SelectMath_Offset = 12; //int SelectMath_Width = 4;
        public UInt32 INACTThreshold { get; set; }
        int INACTThreshold_Offset = 16; //int INACTThreshold_Width = 8;
        public UInt32 INACTCBuffLength { get; set; }
        int INACTCBuffLength_Offset = 24; //int INACTCBuffLength_Width = 8;
        public bool isEnable { get; set; }      // from ConfigOp, 0 = Feature Disabled.
        public ESMConfigINACTQualADXL357()
        {
            ESMConfigINACTQualADXL357_Word = 0;
            isEnable = false;
        }

        public void ConfigGenerateWord()
        {
            ESMConfigINACTQualADXL357_Word = 0;
            if (isEnable == false)
                ConfigOp = 0;
            else
                ConfigOp = 1;
            ESMConfigINACTQualADXL357_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigINACTQualADXL357_Word, ConfigOp_Offset, ConfigOp);
            ESMConfigINACTQualADXL357_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigINACTQualADXL357_Word, SelectMath_Offset, SelectMath);
            ESMConfigINACTQualADXL357_Word = Tools.Bits_UInt32_intWrite8Bit(ESMConfigINACTQualADXL357_Word, INACTThreshold_Offset, INACTThreshold);
            ESMConfigINACTQualADXL357_Word = Tools.Bits_UInt32_intWrite8Bit(ESMConfigINACTQualADXL357_Word, INACTCBuffLength_Offset, INACTCBuffLength);
        }

        public void ConfigDecodeWord(UInt32 uData32)
        {
            ConfigOp = Tools.Bits_UInt32_uRead4Bit(uData32, ConfigOp_Offset);                   // Bit0: Enable/Disable.
            if (ConfigOp == 0)
                isEnable = false;
            else
                isEnable = true;
            SelectMath = Tools.Bits_UInt32_uRead4Bit(uData32, SelectMath_Offset);               // 0x0 = Peak Detects, 0x1 = Peak DetectII.
            INACTThreshold = Tools.Bits_UInt32_uRead8Bit(uData32, INACTThreshold_Offset);       // 0 to 125 Only. Threshold value which trigger INACT (less than value).
            INACTCBuffLength = Tools.Bits_UInt32_uRead8Bit(uData32, INACTCBuffLength_Offset);   // CBuffer Threshold from the start, which process the INACT qualifier.
            ConfigGenerateWord();
        }
    }
    #endregion

    #region//=================================================================================ESMConfigACTQualADXL357
    // //--------------------------------------------------------------------------------------------------------
    // //--------------------------------------------------------------------------------------------------------
    // typedef struct tagzConfigACTQualADXL357
    // {
    //     union  {
    // 		uINT32 zWord;                                   // 32 BITS
    //     struct  {													    // Note:		// It better to use 16 bit number than 8 bit number. OSMath and Cbuffer is not used.		
    //     uINT32 ConfigOp:4;                               //(0-3)			// Bit0/1: 0 = Disabled, 1=10G, 2=20G, 3=40G									Default 0x3 (enabled/40G)
    //                                                                      // Bit 2:  0 = DC, 1 = HPF Enabled.									            Default 0 (DC)
    //     uINT32 SelectMath:4;                             //(4-7)			// 0x0 = Peak Detects, 0x1 = Peak DetectII.										Default 0x0 (peak detect)
    //     uINT32 ACTThreshold:16;                          //(8-23)		// 0 to 40G = 0 to 40000 Threshold value which trigger ACT (more than value).	Default 2G => 2000. = 0x07D0
    //     uINT32 ACTCBuffLength:8;                         //(24-31)		// CBuffer Threshold from the start, which process the ACT qualifier.			Default 0x0F
    // };
    // 	};
    // } _ESMConfigACTQualADXL357;
    // static const uINT32 ESMConfigACTQualADXL357Default = 0x0F07D001;        // Turn on ACT feature.
    //                                                                         //static const uINT32 ESMConfigACTQualADXL357Default = 0x0F07D000;		// Turn off ACT feature
    [Serializable]
    public class ESMConfigACTQualADXL357
    {
        ITools Tools = new Tools();
        public UInt32 ESMConfigACTQualADXL357_Word { get; set; }
        public UInt32 ConfigOp { get; set; }
        int ConfigOp_Offset = 0; //int ConfigOp_Width = 4;
        public UInt32 SelectMath { get; set; }
        int SelectMath_Offset = 4; //int SelectMath_Width = 4;
        public UInt32 ACTThreshold { get; set; }
        int ACTThreshold_Offset = 8; //int ACTThreshold_Width = 16;
        public UInt32 ACTCBuffLength { get; set; }
        int ACTCBuffLength_Offset = 24; //int ACTCBuffLength_Width = 8;
        public bool isEnable { get; set; }      // from ConfigOp, Bit 0: 0 = Feature Disabled.
        public UInt32 Range357 { get; set; }       // from ConfigOp, Bit 1-2:  1 to 3 for 10,20,40G
        public bool isHPFEnable { get; set; }   // from ConfigOp, 0 = DC, 1 = HPF to fixed config (adjustable in firmware only).
        public ESMConfigACTQualADXL357()
        {
            ESMConfigACTQualADXL357_Word = 0;
            isEnable = false;
            Range357 = 3;
        }

        public void ConfigGenerateWord()
        {
            ConfigOp = 0;
            if (isEnable == true)
            {
                ConfigOp = Range357;            //1 to 3
                if (isHPFEnable == true)
                    ConfigOp = ConfigOp | 0x4;
            }
            ESMConfigACTQualADXL357_Word = 0;
            ESMConfigACTQualADXL357_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigACTQualADXL357_Word, ConfigOp_Offset, ConfigOp);
            ESMConfigACTQualADXL357_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigACTQualADXL357_Word, SelectMath_Offset, SelectMath);
            ESMConfigACTQualADXL357_Word = Tools.Bits_UInt32_intWrite16Bit(ESMConfigACTQualADXL357_Word, ACTThreshold_Offset, ACTThreshold);
            ESMConfigACTQualADXL357_Word = Tools.Bits_UInt32_intWrite8Bit(ESMConfigACTQualADXL357_Word, ACTCBuffLength_Offset, ACTCBuffLength);
        }

        public void ConfigDecodeWord(UInt32 uData32)
        {
            ConfigOp = Tools.Bits_UInt32_uRead4Bit(uData32, ConfigOp_Offset);               //0 = Disabled, 1=10G, 2=20G, 3=40G
            if (ConfigOp == 0)
                isEnable = false;
            else
            {
                isEnable = true;
                Range357 = ConfigOp & 0x3;                                                  // Bit0/1: 0 = Disabled, 1=10G, 2=20G, 3=40G
                if ((ConfigOp & 0x4) == 0)                                                  // Bit 2:  0 = DC, 1 = HPF Enabled.
                    isHPFEnable = false;
                else
                    isHPFEnable = true;
            }

            SelectMath = Tools.Bits_UInt32_uRead4Bit(uData32, SelectMath_Offset);           // 0x0 = Peak Detects, 0x1 = Peak DetectII.
            ACTThreshold = Tools.Bits_UInt32_uRead16Bit(uData32, ACTThreshold_Offset);      // 0 to 40G = 0 to 40000 Threshold value which trigger ACT (more than value).	Default 2G => 2000. = 0x07D0
            ACTCBuffLength = Tools.Bits_UInt32_uRead8Bit(uData32, ACTCBuffLength_Offset);   // CBuffer Threshold from the start, which process the ACT qualifier.
            ConfigGenerateWord();
        }
    }
    #endregion

    #region//=================================================================================ESMConfigACTQualADXL372
    // typedef struct tagzESMConfigACTQualADXL372
    // {
    //     union  {
    // 		uINT32 zWord;                               // 32 BITS
    //     struct  {									//			Description						
    //     uINT32 CountRef:4;                           //(0-3)		0 to 15: Part of SelectMath, number of peak count to qualify
    //     uINT32 Spare24:4;                            //(4-7)		
    //     uINT32 ConfigOp:4;                           //(8-11)	Bit 0: 0 = Disabled, 1 = Enable this feature.
    //     uINT32 SelectMath:4;                         //(12-15)	0 = Peak Detect (default), 1 = Rolling Average. 
    //     uINT32 ADXL372_ACTAVGPeriod:8;               //(16-23)	Rolling Average, must be 1 to 255. 1 = 1/1600 =  625uSec. Default = 5 for 3.125mSec.
    //                                                  //			Peak Detect, array length to inspects, must be less than FIFO array length.
    //     uINT32 ADXL372_ACTThreshold:8;               //(24-31)	0 to 200G Threshold. Do not use less than 5. 
    // };
    // 	};
    // } _ESMConfigACTQualADXL372;
    // //static const uINT32 ESMConfigACTQualADXL372Default = 0x05050010;	//Disabled.
    // static const uINT32 ESMConfigACTQualADXL372Default = 0x05300110;	//Enabled.
    [Serializable]
    public class ESMConfigACTQualADXL372
    {
        ITools Tools = new Tools();
        public UInt32 ESMConfigACTQualADXL372_Word { get; set; }
        public UInt32 CountRef { get; set; }
        int CountRef_Offset = 0; //int CountRef_Width = 4;
        public UInt32 ConfigOp { get; set; }
        int ConfigOp_Offset = 8; //int ConfigOp_Width = 4;
        public UInt32 SelectMath { get; set; }
        int SelectMath_Offset = 12; //int SelectMath_Width = 4;
        public UInt32 ADXL372_ACTAVGPeriod { get; set; }
        int ADXL372_ACTAVGPeriod_Offset = 16; //int ADXL372_ACTAVGPeriod_Width = 8;
        public UInt32 ADXL372_ACTThreshold { get; set; }
        int ADXL372_ACTThreshold_Offset = 24; //int ADXL372_ACTThreshold_Width = 8;
        public bool isEnable { get; set; }      // from ConfigOp, 0 = Feature Disabled.
        public ESMConfigACTQualADXL372()
        {
            ESMConfigACTQualADXL372_Word = 0;
            isEnable = false;
        }

        public void ConfigGenerateWord()
        {
            ESMConfigACTQualADXL372_Word = 0;
            if (isEnable == false)
                ConfigOp = 0;
            else
                ConfigOp = 1;
            ESMConfigACTQualADXL372_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigACTQualADXL372_Word, CountRef_Offset, CountRef);
            ESMConfigACTQualADXL372_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigACTQualADXL372_Word, ConfigOp_Offset, ConfigOp);
            ESMConfigACTQualADXL372_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigACTQualADXL372_Word, SelectMath_Offset, SelectMath);
            ESMConfigACTQualADXL372_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigACTQualADXL372_Word, ADXL372_ACTAVGPeriod_Offset, ADXL372_ACTAVGPeriod);
            ESMConfigACTQualADXL372_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigACTQualADXL372_Word, ADXL372_ACTThreshold_Offset, ADXL372_ACTThreshold);
        }

        public void ConfigDecodeWord(UInt32 uData32)
        {
            CountRef = Tools.Bits_UInt32_uRead4Bit(uData32, CountRef_Offset);                           //(0-3)		0 to 15: Part of SelectMath, number of peak count to qualify
            ConfigOp = Tools.Bits_UInt32_uRead4Bit(uData32, ConfigOp_Offset);                           //(8-11)	Bit 0: 0 = Disabled, 1 = Enable this feature.
            if (ConfigOp == 0)
                isEnable = false;
            else
                isEnable = true;                    
            SelectMath = Tools.Bits_UInt32_uRead4Bit(uData32, SelectMath_Offset);                       //(12-15)	0 = Peak Detect (default), 1 = Rolling Average. 
            ADXL372_ACTAVGPeriod = Tools.Bits_UInt32_uRead8Bit(uData32, ADXL372_ACTAVGPeriod_Offset);   //(16-23)	Rolling Average, must be 1 to 255. 1 = 1/1600 =  625uSec. Default = 5 for 3.125mSec.
            ADXL372_ACTThreshold = Tools.Bits_UInt32_uRead8Bit(uData32, ADXL372_ACTThreshold_Offset);   //(24-31)	0 to 200G Threshold. Do not use less than 5.
            ConfigGenerateWord();
        }
    }
    #endregion

    #region//=================================================================================ESMConfigQ
    //     typedef struct tagzESMConfigQ
    //     {
    //         union  {
    // 		uINT32 zWord;                                           // 32 BITS
    //         struct  {
    // 
    //             uINT32 StartBlockRTC:4;                             //(0-3)		= 0x0 Block 0 RTC for   BMG250
    //         uINT32 StopBlockRTC:4;                              //(4-7)		= 0x1 Block 1 RTC for   ADXL357
    //         uINT32 StartBlockShock:4;                           //(8-11)	= 0x2 Block 2 Shock for BMG250
    //         uINT32 StopBlockShock:4;                            //(12-15)	= 0x3 Block 3 Shock for ADXL357
    //         uINT32 QualRTCSetting:16;                           //(16-31)	= 30 Second Period 0x001E
    //     };
    // };
    // } _ESMConfigQ;													// Default 	0x1E000010

    [Serializable]
    public class ESMConfigQ
    {
        ITools Tools = new Tools();
        public UInt32 ESMConfigQ_Word { get; set; }
        public UInt32 StartBlockCH1 { get; set; }
        int StartBlockCH1_Offset = 0;  //Width = 4;
        public UInt32 StopBlockCH1 { get; set; }
        int StopBlockCH1_Offset = 4;  //Width = 4;
        public UInt32 StartBlockCH2 { get; set; }
        int StartBlockCH2_Offset = 8; //Width = 4;
        public UInt32 StopBlockCH2 { get; set; }
        int StopBlockCH2_Offset = 12; //Width = 4;
        public UInt32 QualRTCSetting { get; set; }
        int QualRTCSetting_Offset = 16; //Width = 16;

        public ESMConfigQ()
        {
            ESMConfigQ_Word = 0;
        }

        public void ConfigGenerateWord()
        {
            ESMConfigQ_Word = 0;
            ESMConfigQ_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigQ_Word, StartBlockCH1_Offset, StartBlockCH1);
            ESMConfigQ_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigQ_Word, StopBlockCH1_Offset, StopBlockCH1);
            ESMConfigQ_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigQ_Word, StartBlockCH2_Offset, StartBlockCH2);
            ESMConfigQ_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigQ_Word, StopBlockCH2_Offset, StopBlockCH2);
            ESMConfigQ_Word = Tools.Bits_UInt32_intWrite16Bit(ESMConfigQ_Word, QualRTCSetting_Offset, QualRTCSetting);
        }

        public void ConfigDecodeWord(UInt32 uData32)
        {
            StartBlockCH1 = Tools.Bits_UInt32_uRead4Bit(uData32, StartBlockCH1_Offset);
            StopBlockCH1 = Tools.Bits_UInt32_uRead4Bit(uData32, StopBlockCH1_Offset);
            StartBlockCH2 = Tools.Bits_UInt32_uRead4Bit(uData32, StartBlockCH2_Offset);
            StopBlockCH2 = Tools.Bits_UInt32_uRead4Bit(uData32, StopBlockCH2_Offset);
            QualRTCSetting = Tools.Bits_UInt32_uRead16Bit(uData32, QualRTCSetting_Offset);
            ConfigGenerateWord();
        }
    }
    #endregion

    #region//=================================================================================ESMConfigMasterBlock
    // -------------------------------------------------------
    // ESMConfigMasterSeq[0] // Periodic Timer ADXL357
    // ESMConfigMasterSeq[1] // Periodic Timer BMG250
    // ESMConfigMasterSeq[2] // Shock		   ADXL357
    // ESMConfigMasterSeq[3] // Shock		   BMG250
    // -------------------------------------------------------
    //     typedef struct tagzESMConfigMasterBlock
    //     {
    //         union  {
    // 		uINT32 zWord;                                           // 32 BITS
    //         struct  {
    // 
    //         uINT32 SensorMode:2;                                //(0-1)				This Seq applied to: 0 = None (Disabled), 1 = ADXL357, 2 = BMG250, 3 = ADXL372. 
    //         uINT32 Spare:6;                                     //(2-7)
    //         uINT32 MaxSeqNoCounter:4;                           //(8-11)			This max seq counter (last sequence) before reset. Fixed Value.							
    //         uINT32 BeginPolicy:4;                               //(12-15)			Begin Sequence policy, 0 = Default.
    //         uINT32 MiddlePolicy:4;                              //(16-19)			Middle Sequence policy or In Operation Sequence policy, 0 = Default.
    //         uINT32 FinishPolicy:4;                              //(20-23)			Finish Sequence policy, 0 = Default.
    //         uINT32 NVMAddressStart:8;                           //(24-31)			Start Sequencer Frame: NVMAddressStart = NVMAddressStart+SeqNoCounter (in ESMSquencerFlags[]).		
    //     };
    // };
    // } _ESMConfigMasterBlock;

    [Serializable]
    public class ESMConfigMasterBlock
    {
        ITools Tools = new Tools();
        public UInt32 ESMConfigMasterBlock_Word { get; set; }
        public UInt32 SensorMode { get; set; }
        int SensorMode_Offset = 0;  //Width = 2;
        public UInt32 Spare { get; set; }
        int Spare_Offset = 2;  //Width = 6;
        public UInt32 MaxSeqNoCounter { get; set; }
        int MaxSeqNoCounter_Offset = 8; //Width = 4;
        public UInt32 BeginPolicy { get; set; }
        int BeginPolicy_Offset = 12; //Width = 4;
        public UInt32 MiddlePolicy { get; set; }
        int MiddlePolicy_Offset = 16; //Width = 4;
        public UInt32 FinishPolicy { get; set; }
        int FinishPolicy_Offset = 20; //Width = 4;
        public UInt32 NVMAddressStart { get; set; }
        int NVMAddressStart_Offset = 24; //Width = 8;

        public ESMConfigMasterBlock()
        {
            ESMConfigMasterBlock_Word = 0;
        }

        public void ConfigGenerateWord()
        {
            ESMConfigMasterBlock_Word = 0;
            ESMConfigMasterBlock_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigMasterBlock_Word, SensorMode_Offset, SensorMode);
            ESMConfigMasterBlock_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigMasterBlock_Word, MaxSeqNoCounter_Offset, MaxSeqNoCounter);
            ESMConfigMasterBlock_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigMasterBlock_Word, BeginPolicy_Offset, BeginPolicy);
            ESMConfigMasterBlock_Word = Tools.Bits_UInt32_intWrite4Bit(ESMConfigMasterBlock_Word, MiddlePolicy_Offset, MiddlePolicy);
            ESMConfigMasterBlock_Word = Tools.Bits_UInt32_intWrite16Bit(ESMConfigMasterBlock_Word, FinishPolicy_Offset, FinishPolicy);
            ESMConfigMasterBlock_Word = Tools.Bits_UInt32_intWrite16Bit(ESMConfigMasterBlock_Word, NVMAddressStart_Offset, NVMAddressStart);
        }

        public void ConfigDecodeWord(UInt32 uData32)
        {
            SensorMode = Tools.Bits_UInt32_uRead4Bit(uData32, SensorMode_Offset);               //2bit + 2bit Spare  2
            MaxSeqNoCounter = Tools.Bits_UInt32_uRead4Bit(uData32, MaxSeqNoCounter_Offset);     // 8 Bit
            BeginPolicy = Tools.Bits_UInt32_uRead4Bit(uData32, BeginPolicy_Offset);             // 4 bit
            MiddlePolicy = Tools.Bits_UInt32_uRead4Bit(uData32, MiddlePolicy_Offset);
            FinishPolicy = Tools.Bits_UInt32_uRead4Bit(uData32, FinishPolicy_Offset);
            NVMAddressStart = Tools.Bits_UInt32_uRead16Bit(uData32, NVMAddressStart_Offset);
            ConfigGenerateWord();
        }


    }
    #endregion

    #region//=================================================================================ESMConfigSeqNo16
    //     typedef struct tagzESMConfigSeqNo16
    //     {
    //         union  {
    // 		uINT16 zWord;                                           // 16 BITS
    //         struct  {
    //         uINT16 Interval:9;                                  //(0-8)				Interval Period based on EvenActivityMode. 1 to 512 Second (8.5min)
    //         uINT16 ActivityMode:3;                              //(9-11)			0 = OFF, 1 = ON, 2-7 Spare.
    //         uINT16 Spare:4;                                     //(12-15)			Spare
    //     };
    // };
    // } _ESMConfigSeqNo16;
    [Serializable]
    public class ESMConfigSeqNo16
    {
        ITools Tools = new Tools();
        public UInt16 ESMConfigSeqNo16_Word { get; set; }
        public UInt16 Interval { get; set; }
        int Interval_Offset = 0;  //Width = 9;
        public UInt16 ActivityMode { get; set; }
        int ActivityMode_Offset = 9;  //Width = 3;
        public UInt16 Spare { get; set; }
        int Spare_Offset = 12; //Width = 4;

        public ESMConfigSeqNo16()
        {
            ESMConfigSeqNo16_Word = 0;
        }

        public void ConfigGenerateWord()
        {
            ESMConfigSeqNo16_Word = 0;
            ESMConfigSeqNo16_Word = (UInt16) Tools.Bits_UInt32_intWrite9Bit(ESMConfigSeqNo16_Word, Interval_Offset, Interval);
            ESMConfigSeqNo16_Word = (UInt16) Tools.Bits_UInt32_intWrite3Bit(ESMConfigSeqNo16_Word, ActivityMode_Offset, ActivityMode);
            ESMConfigSeqNo16_Word = (UInt16) Tools.Bits_UInt32_intWrite4Bit(ESMConfigSeqNo16_Word, Spare_Offset, Spare);
        }

        public void ConfigDecodeWord(UInt16 uData16)
        {
            Interval = (UInt16) Tools.Bits_UInt32_uRead9Bit(uData16, Interval_Offset);
            ActivityMode = (UInt16) Tools.Bits_UInt32_uRead3Bit(uData16, ActivityMode_Offset);
            Spare = (UInt16) Tools.Bits_UInt32_uRead4Bit(uData16, Spare_Offset);
            ConfigGenerateWord();
        }
    }
    #endregion


}

//#########################################################################################################
//########################################################################################## SANDBOX ARENA
//#########################################################################################################

//                 string[] namesArray = "Tom,Scott,Bob".Split(',');
//                 List<string> namesList = new List<string>(namesArray.Length);
//                 namesList.AddRange(namesArray);
//                 namesList.Reverse();
//  Array.Clear(b, 0, 600);
//=====================================================================
//             byte[] buffer = System.Text.UTF8Encoding.Unicode.GetBytes(binarydata);
//             sbyte[] sbBuffer = new sbyte[buffer.Length];
//             Buffer.BlockCopy(buffer, 0, sbBuffer, 0, buffer.Length);
//byte[] buffer = System.Text.Encoding.Unicode.GetBytes(binarydata);
//=====================================================================