using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDT_Term_FFT
{
    public enum BGToCoToolA : int     
    {
        DISCONNECT  = 0,
        CONNECT     = 1,
        STARTMODE   = 2,
        COREMARKED  = 3,
        ORIENTATION = 4,
        END         = 5,
        ERROR       = 6,
        OFF         = 7
    }

    //public enum BGToCoToolType : int      //Tool Orientation
    //{
    //    TYPE_CO     = 0,    // Core Orientation
    //    TYPE_TO     = 1,    // Tool Orientation
    //    TYPE_TS     = 2,    // Tool Survey
    //                        // Spare 3 to D
    //    UNDEFINED   = 0xE,  // Not defined (reserved)
    //    ERROR       = 0xF   // Error due to out of range (3 to D) ToolType (software bug). 
    //}
    
    public enum BGToCoToolTOA : int      //Tool Orientation
    {
        DISCONNECT  = 0,
        CONNECT     = 1,
        STARTMODE   = 2,
        DONE        = 5,
        ERROR       = 6,
        OFF         = 7
    }

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++BGMasterSetup (Common, Setup Data and Filename)
    [Serializable]
    public class BGMasterSetup
    {
        public int BGToCoToolAState         { get; set; }
        public int BGToCoToolSelectMode     { get; set; }       // 0 = TOCO || 1 = TS .This based on CheckBox for TS or TOCO for TSTOCO board and TOCO Board    Rev.78JE
        //------------------------------------------------------------
        // Title Filename for the project
        private const string m_sFilenameDefaultTopProject = "SurveyProject";              // Master Folder for all below files. 
        private const string m_sFilenameDefaultJobSurvey = "JobSurvey";                  // BG_ToolSetup Window, Job section                     sync with BGMasterJobSurvey()
        private const string m_sFilenameDefaultConfiguration = "Configuration";              // BG_ToolSetup Window, tool config                     sync with BGMasterSetup()
        private const string m_sFilenameDefaultDrillerSurvey = "DrillerSurvey";              // BG_Driller Window save timestamp data into project.  sync with BGMasterDriller()
        private const string m_sFilenameDefaultDownloadedRaw = "DownloadedRaw";              // Download Window, raw data from tools                 sync with header structure from LPC1549 file. 
        private const string m_sFilenameDefaultDownloadedConverted = "DownloadedConverted";        // Download Window, converted data from tools.          sync with header structure from LPC1549 file. 
        private const string m_sFilenameDefaultDownloadedSelected = "DownloadedSelected";         // Download Window, selected data.                      sync with header structure from LPC1549 file. 
        private const string m_sFilenameDefaultSurveyResults = "SurveyReport";               // Survey Report Window, Directional maths goes here.   sync with BGSurveyDirectional() 

        //#####################################################################################################
        //###################################################################################### Getter/Setter, public
        //#####################################################################################################
        public string sFile_DriveAll { get; set; }                   // " J:"
        public string sFile_TopFolderProjectAll { get; set; }        // "J:/SurveyProject"
        public string sFile_JobProjectAll { get; set; }              // "J:/SurveyProject/JobName"
                                                                     //----------------------------------------------------------------------------------------------Title Filename for the project gettter Only
        public string sFilenameDefaultJobSurvey { get { return m_sFilenameDefaultJobSurvey; } }
        public string sFilenameDefaultDrillerSurvey { get { return m_sFilenameDefaultDrillerSurvey; } }
        public string sFilenameDefaultDownloadedRaw { get { return m_sFilenameDefaultDownloadedRaw; } }
        public string sFilenameDefaultDownloadedConverted { get { return m_sFilenameDefaultDownloadedConverted; } }
        public string sFilenameDefaultDownloadedSelected { get { return m_sFilenameDefaultDownloadedSelected; } }
        public string sFilenameDefaultSurveyResults { get { return m_sFilenameDefaultSurveyResults; } }
        public string sFilenameDefaultTopProject { get { return m_sFilenameDefaultTopProject; } }
        //----------------------------------------------------------------------------------------------------

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public BGMasterSetup()
        {
            BGToCoToolSelectMode = 0;                       // Select TO. 
            BGToCoToolAState = (int)BGToCoToolA.DISCONNECT;
            //sFile_DriveAll = "C:";
            //sFile_TopFolderProjectAll = sFile_DriveAll +"\\" + m_sFilenameDefaultTopProject;      // For all project. 
            //sFile_JobProjectAll = sFile_TopFolderProjectAll;

            string sFoldername = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                sFoldername = Directory.GetParent(sFoldername).ToString();
            }
            sFile_DriveAll = Path.GetPathRoot(Environment.SystemDirectory);
            sFile_DriveAll.Replace("/", string.Empty);
            sFile_TopFolderProjectAll = sFoldername + "\\" + m_sFilenameDefaultTopProject;      // For all project. 
            sFile_JobProjectAll = sFile_TopFolderProjectAll;
        }

    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++BGMasterSurvey (Tool Job/Project Data)
    [Serializable]
    public class BGMasterJobSurvey
    {
        //#####################################################################################################
        //###################################################################################### Getter/Setter, public
        //#####################################################################################################
        public string sJobRef { get; set; }         // String Name 
        public string sLocation { get; set; }       // 0 = No Offset, +/- data = offset
        public string sDriller { get; set; }         // 1 = 1V/V
        public string sBoreholeName { get; set; }   // Resultant data
        public string sBoreHoleDepth { get; set; }
        public string sNote { get; set; }
        public string sTimeStamp { get; set; }

        public string sGyroSurveyLoop { get; set; }
        public int iStationInterval { get; set; }
        public int iWaitPeroidSec { get; set; }
        public bool bisMeter { get; set; }       // true for meter, false for feet. 

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public BGMasterJobSurvey()
        {
            sJobRef = "Bug Bunny Invoice:762327";
            sLocation = "Sofia";
            sDriller = "Mr Bunny";
            sBoreholeName = "A Rabbit Hole";
            sBoreHoleDepth = "To Earth Center";
            sNote = "Comment about contract and job, the client want to harvest more carrots";
            sTimeStamp = "";
            sGyroSurveyLoop = "3";          // Default in tool. 
            iStationInterval = 500;         // Interval to capture Survey.  
            iWaitPeroidSec = 20;
            bisMeter = true;
        }
    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++BGMasterDriller (Driller Survey)
    [Serializable]
    public class BGReportList
    {
        //#####################################################################################################
        //###################################################################################### Getter/Setter, public
        //#####################################################################################################
        public UInt32 iNumber { get; set; }
        public string sType { get; set; }
        public string sNote { get; set; }
        public string sFormat { get; set; }
        public double dtUTC { get; set; }
        //------------------------------------------For Error/Report Frame, do not show in column. 
        public UInt32 EESysFlag { get; set; }
        public UInt32 EEBattState { get; set; }

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public BGReportList()
        {
            iNumber = 0;
            sType = "";
            sNote = "";
            sFormat = "";
            dtUTC = 0;
            EESysFlag = 0;     // Not valid data.
            EEBattState = 0;   // Not valid data.
        }

        public UInt32 Get_EESysFlag()
        {
            return EESysFlag;

        }
        public UInt32 Get_EEBattState()
        {
            return EEBattState;

        }
        public void Set_EESysFlag(UInt32 data)
        {
            EESysFlag = data;

        }
        public void Set_EEBattState(UInt32 data)
        {
            EEBattState = data;

        }
    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++BGCapturedList (Captured Data )
    [Serializable]
    public class BGCapturedList
    {
        //#####################################################################################################
        //###################################################################################### Getter/Setter, public
        //#####################################################################################################
        public UInt32 TimeStamp { get; set; }
        public Int32 MRC_Gx { get; set; }
        public Int32 MRC_Gy { get; set; }
        public Int32 MRC_Gz { get; set; }
        public UInt32 Temp { get; set; }

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public BGCapturedList()
        {
            TimeStamp = 0;
            MRC_Gx = 0;
            MRC_Gy = 0;
            MRC_Gz = 0;
            Temp = 0;
        }
    }
    #endregion

    #region//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++BG_AccelMath
    [Serializable]
    public class BGAccelMath_MRC
    {
        ITools Tools = new Tools();
        public Vector3 dAccel;
        //public iVector3 iAccel;

        public double dAInclination { get; set; }            // Radian
        public double dAToolFace { get; set; }
        public double dAMagnitide { get; set; }
        //public UInt32 Temp { get; set; }                    // K/10 format (2953 => 295.3 Kelvin)

        public BGAccelMath_MRC()
        {
            dAccel = new Vector3();
            // Temp = 0;
            dAInclination = 0.0;
            dAToolFace = 0.0;
            dAMagnitide = 0.0;
        }

        #region //------------------------------------ClearAllData
        public void ClearAllData()
        {
            dAccel.X = 0.0;
            dAccel.Y = 0.0;
            dAccel.Z = 0.0;
            //Temp = 0;
            dAInclination = 0.0;
            dAToolFace = 0.0;
            dAMagnitide = 0.0;
        }

        #endregion

        #region //------------------------------------AccelProcessMathNoCal (Working)
        public bool AccelProcessMathNoCal()     // This is Riscy Math (Old), Do not use. 
        {
            //Code within ToCo, but no calibration solution. 
            dAMagnitide = Math.Sqrt((Math.Pow(dAccel.X, 2) + Math.Pow(dAccel.Y, 2) + Math.Pow(dAccel.Z, 2)));
            double dd = Math.Sqrt((Math.Pow(dAccel.X, 2) + Math.Pow(dAccel.Y, 2)));
            dAInclination = 90 - Tools.Math_RadianToDegree(Math.Atan2(dd, dAccel.Z));
            dAToolFace = Tools.Math_RadianToDegree(Math.Atan2(dAccel.Y, dAccel.X));
            return (true);
        }
        #endregion

        #region //------------------------------------AccelProcessMathRiscy (Not optimized yet)
        public bool AccelProcessMathRiscy(BGMasterCalData myBGMasterCalData, out string debug)       // This is Riscy Math. 
        {
            double Axx, Axy, Axz, Ayx, Ayy, Ayz, Azx, Azy, Azz;
            double bx, by, bz;
            double Gx, Gy, Gz;
            //---------------------------------------------- Calibration Section
            Axx = 1.0;
            Axy = 0.0;
            Axz = 0.0;
            Ayx = 0.0;
            Ayy = 1.0;
            Ayz = 0.0;
            Azx = 0.0;
            Azy = 0.0;
            Azz = 1.0;
            bx = 0.0;
            by = 0.0;
            bz = 0.0;

            //Axx = myBGMasterCalData.CalData[0].dData;
            //Axy = myBGMasterCalData.CalData[1].dData;
            //Axz = myBGMasterCalData.CalData[2].dData;
            //Ayx = myBGMasterCalData.CalData[3].dData;
            //Ayy = myBGMasterCalData.CalData[4].dData;
            //Ayz = myBGMasterCalData.CalData[5].dData;
            //Azx = myBGMasterCalData.CalData[6].dData;
            //Azy = myBGMasterCalData.CalData[7].dData;
            //Azz = myBGMasterCalData.CalData[8].dData;
            //bx = myBGMasterCalData.CalData[9].dData;
            //by = myBGMasterCalData.CalData[10].dData;
            //bz = myBGMasterCalData.CalData[11].dData;

            Gx = (Axx * dAccel.X + Axy * dAccel.Y + Axz * dAccel.Z) + bx;
            Gy = (Ayx * dAccel.X + Ayy * dAccel.Y + Ayz * dAccel.Z) + by;
            Gz = (Azx * dAccel.X + Azy * dAccel.Y + Azz * dAccel.Z) + bz;
            //---------------------------------------------- Magnitude (below will not work with calibration data at the moment). 
            dAMagnitide = Math.Sqrt((Math.Pow(Gx, 2) + Math.Pow(Gy, 2) + Math.Pow(Gz, 2)));
            double dd = Math.Sqrt((Math.Pow(Gx, 2) + Math.Pow(Gy, 2)));
            dAInclination = 90 - Tools.Math_RadianToDegree(Math.Atan2(dd, Gz));
            dAToolFace = Tools.Math_RadianToDegree(Math.Atan2(Gy, Gx));
            debug = dAToolFace.ToString("N");
            return (true);
        }
        #endregion

        #region //------------------------------------AccelProcessMath_SL_Method_3A  (110-Release_MathNote_Incl_for_ToCoTool_17Nov18-S001.pdf) and then TF with 
        public bool AccelProcessMath_SL_Method_3A(BGMasterCalData myBGMasterCalData, out string debug)
        {
            double Axx, Axy, Axz, Ayx, Ayy, Ayz, Azx, Azy, Azz;
            double bx, by, bz;
            double Gx, Gy, Gz;
            Axx = 1.0;
            Axy = 0.0;
            Axz = 0.0;
            Ayx = 0.0;
            Ayy = 1.0;
            Ayz = 0.0;
            Azx = 0.0;
            Azy = 0.0;
            Azz = 1.0;
            bx = 0.0;
            by = 0.0;
            bz = 0.0;

            Gx = (Axx * dAccel.X + Axy * dAccel.Y + Axz * dAccel.Z) + bx;
            Gy = (Ayx * dAccel.X + Ayy * dAccel.Y + Ayz * dAccel.Z) + by;
            Gz = (Azx * dAccel.X + Azy * dAccel.Y + Azz * dAccel.Z) + bz;
            dAMagnitide = Math.Sqrt((Math.Pow(Gx, 2) + Math.Pow(Gy, 2) + Math.Pow(Gz, 2)));
            //dAToolFace = Tools.Math_RadianToDegree(Math.Atan2(Gy, Gx));

            //-----------------------------------------------------------------SL solution code but .NET framework do not support native #include <math.h>
            //             TF = 180.0 / PI * atan2(Gx, Gy);
            //             if (TF < 0.0)
            //             {
            //                 TF = 360.0 - abs(TF);
            //             }

            //             if (isgreater(90 - (180.0 / PI * (acos(Gz / sqrt(Gx * Gx + Gy * Gy + Gz * Gz)))), 60))
            //             {
            //                 dAInclination = 90 - 180.0 / PI * asin((sqrt(Gx * Gx + Gy * Gy)) / sqrt(Gx * Gx + Gy * Gy + Gz * Gz));
            //             }
            //             else
            //             {
            //                 dAInclination = 90.0 - 180.0 / PI * (acos(Gz / sqrt(Gx * Gx + Gy * Gy + Gz * Gz)));
            //             }
            //-----------------------------------------------------------------RP Modified version of the above

            dAToolFace = 180.0 / Math.PI * Math.Atan2(Gx, Gy);
            if (dAToolFace < 0.0)
            {
                dAToolFace = 360.0 - Math.Abs(dAToolFace);
            }

            if ((90 - (180.0 / Math.PI * (Math.Acos(Gz / Math.Sqrt(Gx * Gx + Gy * Gy + Gz * Gz))))) > 60.0)
            {
                dAInclination = 90.0 - 180.0 / Math.PI * Math.Asin((Math.Sqrt((Gx * Gx) + (Gy * Gy))) / dAMagnitide);
            }
            else
            {
                dAInclination = 90.0 - 180.0 / Math.PI * (Math.Acos(Gz / dAMagnitide));
            }
            debug = dAToolFace.ToString("N");
            return (true);
        }
        #endregion

        #region //------------------------------------AccelConvertINT32toDouble
        public bool AccelConvertINT32toDouble(iVector3 iAccelIN)    // 1G = 10,000.
        {
            dAccel.X = (Convert.ToDouble(iAccelIN.X)) / 10000.0;    //mG to G readout. 
            dAccel.Y = (Convert.ToDouble(iAccelIN.Y)) / 10000.0;
            dAccel.Z = (Convert.ToDouble(iAccelIN.Z)) / 10000.0;
            return (true);
        }
        #endregion

        #region //------------------------------------AccelProcessCalibration
        public bool AccelProcessCalibration()
        {
            return (true);
        }
        #endregion

    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++BGToCoProjectFileManager 
    //--------------------------------BGToCoProjectFileManager created (moved from BGToCoReportClass) in Rev 76I to unify one location for filename manager
    [Serializable]
    public class BGToCoProjectFileManager
    {
        ITools Tools = new Tools();
        //#####################################################################################################
        //###################################################################################### Getter/Setter, public
        //#####################################################################################################
        public string ToCo_FolderNameRef;
        public string ToCo_FolderDriveRef;
        public string ToCo_FolderName;
        public string ToCo_FolderDrive;
        public string ToCo_Master_FileName;             //ToCo_Master_<STCSTART UTC TimeStamp>.txt
        public string ToCo_Setup_FileName;              //ToCo_Setup_<UTC Timestamp>.txt
        public BGToCoReportClass myToCoReportClass;

        

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public BGToCoProjectFileManager(BGToCoReportClass BGToCoReportClassRef)
        {
            myToCoReportClass = BGToCoReportClassRef;
            //--------------------------------------------------------------------Setup Default Folder Name via User Folder. Create ToCoProject folder if not exist. 
            string sFoldername = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                sFoldername = Directory.GetParent(sFoldername).ToString();
            }
            ToCo_FolderDrive = Path.GetPathRoot(Environment.SystemDirectory);
            ToCo_FolderDrive.Replace("/", string.Empty);
            ToCo_FolderName = sFoldername + "\\" + "ToCoProject";      // For all project. 
            ToCo_Setup_FileName = "";
            ToCo_Master_FileName = "";
            try
            {
                if (!Directory.Exists(ToCo_FolderName))                             // Default folder name for given drive. 
                {
                    DirectoryInfo di = Directory.CreateDirectory(ToCo_FolderName);  // Create folder if not exist. 
                }

            }
            catch { };
        }
        //#####################################################################################################
        //###################################################################################### Methods
        //#####################################################################################################
        #region----------------------------------------------------------------------sToCo_Setup_FileName (Created at any time, not related to STC_START). 
        public string sToCo_Setup_FileName()        // Does not depend on timestamp from STCSTART. That is for TOCO_Master. For now rely on UTC timeastamp as serial number. 
        {
            ToCo_Setup_FileName = System.IO.Path.Combine(ToCo_FolderName, "ToCoSetup_" + Tools.uDateTimeToUnixTimestampNowUTC().ToString() + ".xml");
            return ToCo_Setup_FileName;
        }
        #endregion

        #region----------------------------------------------------------------------sToCo_Master_FileName (Created when STC_START is clicked, not before)
        public string sToCo_Master_FileName()
        {
            ToCo_Master_FileName = "";
            if (myToCoReportClass.bSTCSTART_Button_TimeStampFound == true)
            {
                ToCo_Master_FileName = System.IO.Path.Combine(ToCo_FolderName, "ToCoMaster_" + myToCoReportClass.sSTCSTART_Button_TimeStamp + ".txt");
            }
            else
            {
                ToCo_Master_FileName = System.IO.Path.Combine(ToCo_FolderName, "ToCoMaster_NoSTCStart.txt");
            }
            return ToCo_Master_FileName;
        }
        #endregion

        #region----------------------------------------------------------------------sToCo_Master_Insert_TimeStamp
        public bool sToCo_Master_Insert_TimeStamp()
        {
            try
            {
                if (File.Exists(ToCo_Master_FileName))               // This text is added only once to the file.
                {
                    myToCoReportClass.STC_CoreCapture_Survey_TimeStamp = Tools.uDateTimeToUnixTimestampNowUTC();
                    File.AppendAllText(ToCo_Master_FileName, "CoreCapture_TimeStamp = " + myToCoReportClass.STC_CoreCapture_Survey_TimeStamp.ToString("D") + "\n");
                    return (true);
                }
            }
            catch
            {
                return (false);     // Failure
            }
            return (false);         // Failure
        }
        #endregion

        #region----------------------------------------------------------------------sToCo_Master_Insert_CaptureDataSection
        public bool sToCo_Master_Insert_CaptureDataSection()
        {
            if (myToCoReportClass.CaptureDataFrame == null)
                return (false);
            if (myToCoReportClass.CaptureDataFrame.Count != 5)
                return (false);
            string s = "";
            try
            {
                if (File.Exists(ToCo_Master_FileName))               // This text is added only once to the file.
                {
                    File.AppendAllText(ToCo_Master_FileName, "\nSample Period:" + myToCoReportClass.iSampleperoid.ToString() + " (Sec)\n");
                    File.AppendAllText(ToCo_Master_FileName, "Survey Start Data:" + myToCoReportClass.Survey_StartTimeStamp.ToString() + "\n");
                    File.AppendAllText(ToCo_Master_FileName, "Survey End Data:" + myToCoReportClass.Survey_EndTimeStamp.ToString() + "\n");
                    File.AppendAllText(ToCo_Master_FileName, "Index;UDT;MRC-Gx;MRC-Gy;MRC-Gz;Temp(K/10); <Unsorted>\n");
                    for (int i = 0; i < 5; i++)
                    {
                        s = i.ToString();
                        s += ";" + myToCoReportClass.CaptureDataFrame[i].TimeStamp.ToString();
                        s += ";" + myToCoReportClass.CaptureDataFrame[i].MRC_Gx.ToString();
                        s += ";" + myToCoReportClass.CaptureDataFrame[i].MRC_Gy.ToString();
                        s += ";" + myToCoReportClass.CaptureDataFrame[i].MRC_Gz.ToString();
                        s += ";" + myToCoReportClass.CaptureDataFrame[i].Temp.ToString();
                        File.AppendAllText(ToCo_Master_FileName, s + "\n");
                    }
                    return (true);
                }
            }
            catch
            {
                return (false);     // Failure
            }
            return (false);         // Failure
        }
        #endregion

        #region----------------------------------------------------------------------sToCo_Master_Filename_Append_String
        public bool sToCo_Master_Filename_Append_String(string sMessage)
        {
            try
            {
                if (File.Exists(ToCo_Master_FileName))               // This text is added only once to the file.
                {
                    File.AppendAllText(ToCo_Master_FileName, "---MSG: " + sMessage + "\n");
                    return (true);
                }
            }
            catch
            {
                return (false);     // Failure
            }
            return (false);         // Failure
        }
        #endregion

        #region----------------------------------------------------------------------sToCo_Master_FileName (Created when STC_START is clicked, not before)
        public bool sToCo_Master_Insert_ClibrationData()
        {
            return (false);     // Failure
        }
        #endregion

        #region----------------------------------------------------------------------ToCo_Master_STCSTART_Operation (Run this when STC START button clicked)
        public bool ToCo_Master_STCSTART_Operation(UInt32 SerialNo)
        {
            myToCoReportClass.bSTCSTART_Button_TimeStampFound = true;
            myToCoReportClass.STCSTART_Button_TimeStamp = Tools.uDateTimeToUnixTimestampNowLocal();
            myToCoReportClass.sSTCSTART_Button_TimeStamp = myToCoReportClass.STCSTART_Button_TimeStamp.ToString("D");
            myToCoReportClass.Survey_StartTimeStamp = 0;
            myToCoReportClass.Survey_EndTimeStamp = 0;
            try
            {
                string pathString = sToCo_Master_FileName();        // Create filename 
                // This create new filename and insert text. 
                File.WriteAllText(pathString, "STCSTART_TimeStamp = " + myToCoReportClass.sSTCSTART_Button_TimeStamp + "\n");
                File.AppendAllText(pathString, "SERIAL_NO = " + SerialNo.ToString() + "\n");
                return (true);
            }
            catch { };
            return (false);
        }
        #endregion

    }
    #endregion

    #region//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++BGToCoReportClass 
    //--------------------------------Report Class which get updated when connected or ReportView is invoked. 
    [Serializable]
    public class BGToCoReportClass
    {
        ITools Tools = new Tools();
        //#####################################################################################################
        //###################################################################################### Getter/Setter, public
        //#####################################################################################################
        public int CountF;     // Error Frame within LOGGER region. 
        public int CountR;     // Report Frame within LOGGER region.   
        public int CountCD;    // Client Data Frame within LOGGER region.
        public int CountRD;    // Report Data Frame with CONFIG region    

        public List<BGReportList> ReportRef;            // Reference only, Read Only
        public List<BGReportList> ReportData;           // Data and reference, can be cleared. Some copied over from ReportRef during inserting
        public List<BGReportList> ErrorFrame;
        public List<BGReportList> ReporFrame;
        public List<BGReportList> ClientDataFrame;
        public List<BGReportList> RentalDataFrame;
        public List<BGCapturedList> CaptureDataFrame;   // Used for orientation.

        public UInt32 Survey_StartTimeStamp;
        public UInt32 Survey_EndTimeStamp;
        //------------------------------------------------Only for ToCo_Setup and Start/Stop Operation, not for Report Viewer.
        public bool bSTCSTART_Button_TimeStampFound;        //true if update, false = blank. 
        public string sSTCSTART_Button_TimeStamp;
        public UInt32 STCSTART_Button_TimeStamp;
        //-----------------------------------------------
        public UInt32 STC_CoreCapture_Survey_TimeStamp;
        public List<UInt32> STC_TO_Survey_TimeStamp;
        public UInt32 STC_Filename_Survey_SerialNo;
        //-----------------------------------------------

        public int iSampleperoid;
        public bool isSamplePeroidChanged;
        public bool isClientDataTransferDone;
        //------------------------------------------------Serial No Section
        public UInt32 SerialNumber;                     //0x0000
        //----------------------------------------------- Tool Type CO/TO/TS
        //public int ToCo_ToolType;                       //0x0021 Bit 31-28 (4 bits)
        //------------------------------------------------Battery Section
        public UInt32 EESysFlag;                        //0x0013    
        public UInt32 EEBattState;                      //0x0014
        public int BatCapacity;                         //0x0015
        public UInt32 BatLastRechargeTimeStamp;         //0x0016
        public int BatLastRechargeTemp;                 //0x0017
        public UInt32 BatLowVoltageTimeStamp;           //0x0018
        //------------------------------------------------
        public int ReportDataCounter;
        public int ReportDataNotMatchedCounter;         // Not matched counter
        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public BGToCoReportClass()
        {
            // Support    Format Type
            //	N		: 0i = signed int, 32 bits
            //	Y		: 0u = unsigned int, 32 bits
            //	Y		: 0x = generic Hex
            //	N		: 1x = 8 bit hex, 2x = 16 bit hex, 3x = 24 bits hex, 4x = 32 bits hex
            //	N		: 0d = double
            //	N*		: 0s = string       // Not for config
            //	N		: 0z = time element
            //	N		: 0y = date element
            //  Y       : 0t = datetime (UDT Epoch) 
            //  Y       : 0p = password mask control
            //  Y       : 0c = Hex to ASCII

            ReportRef = new List<BGReportList>();

            ClearData();

            //--------------------------------------------------------------------
            sSTCSTART_Button_TimeStamp = "";
            STCSTART_Button_TimeStamp = 0u;
            iSampleperoid = 5;                 // 5 Second.
            isSamplePeroidChanged = false;
            //ToCo_ToolType = 0;                  // CO type.
            SerialNumber = 0;
            isClientDataTransferDone = false;   // Transferred from BG_ClientData
            //--------------------------------------------------------------------

            ReportRef.Add(new BGReportList { iNumber = 0x0010, sType = "CONFIG SerialNo", sFormat = "0u" });                    // Copied from special place in TOCO Logger memory
            ReportRef.Add(new BGReportList { iNumber = 0x0011, sType = "CONFIG PartNo1 ", sFormat = "0c" });
            ReportRef.Add(new BGReportList { iNumber = 0x0012, sType = "CONFIG PartNo2", sFormat = "0c" });
            ReportRef.Add(new BGReportList { iNumber = 0x0013, sType = "CONFIG EESysFlag", sFormat = "0x" });
            ReportRef.Add(new BGReportList { iNumber = 0x0014, sType = "CONFIG EEBattState", sFormat = "0x" });
            ReportRef.Add(new BGReportList { iNumber = 0x0015, sType = "CONFIG FW Revision(4B/78)", sFormat = "0x" });                             //TOCO 2N: MK2 has no Isense anymore so this is not needed. 
            ReportRef.Add(new BGReportList { iNumber = 0x0016, sType = "CONFIG BatLastRechargeTimeStamp", sFormat = "0t" });
            ReportRef.Add(new BGReportList { iNumber = 0x0017, sType = "CONFIG BatLastRechargeTemp (K/10)", sFormat = "0u" });
            ReportRef.Add(new BGReportList { iNumber = 0x0018, sType = "CONFIG BatLowVoltageTimeStamp", sFormat = "0t" });
            ReportRef.Add(new BGReportList { iNumber = 0x0019, sType = "CONFIG uAHr per Survey(4B/78)", sFormat = "0u" });
            ReportRef.Add(new BGReportList { iNumber = 0x001A, sType = "CONFIG SurveyEventPeroid(Sec)", sFormat = "0u" });

            ReportRef.Add(new BGReportList { iNumber = 0x0020, sType = "RD0_OrderNo", sFormat = "0u" });
            ReportRef.Add(new BGReportList { iNumber = 0x0021, sType = "RD1_UDT_Start", sFormat = "0t" });
            ReportRef.Add(new BGReportList { iNumber = 0x0022, sType = "RD2_UDT_Expiry", sFormat = "0t" });
            ReportRef.Add(new BGReportList { iNumber = 0x0023, sType = "RD3_PolicyNumber", sFormat = "0u" });
            ReportRef.Add(new BGReportList { iNumber = 0x0024, sType = "RD4_ExtensionPW (0 = Used)", sFormat = "0p" });

            ReportRef.Add(new BGReportList { iNumber = 0x0030, sType = "CD0", sFormat = "0s" });
            ReportRef.Add(new BGReportList { iNumber = 0x0031, sType = "CD1", sFormat = "0s" });
            ReportRef.Add(new BGReportList { iNumber = 0x0032, sType = "CD2", sFormat = "0s" });
            ReportRef.Add(new BGReportList { iNumber = 0x0033, sType = "CD3", sFormat = "0s" });
            ReportRef.Add(new BGReportList { iNumber = 0x0034, sType = "CD4", sFormat = "0s" });
            ReportRef.Add(new BGReportList { iNumber = 0x0035, sType = "CD5", sFormat = "0s" });
            ReportRef.Add(new BGReportList { iNumber = 0x0036, sType = "CD6", sFormat = "0s" });
            ReportRef.Add(new BGReportList { iNumber = 0x0037, sType = "CD7", sFormat = "0s" });
            ReportRef.Add(new BGReportList { iNumber = 0x0038, sType = "CD8", sFormat = "0s" });
            ReportRef.Add(new BGReportList { iNumber = 0x0039, sType = "CD9", sFormat = "0s" });

            ReportRef.Add(new BGReportList { iNumber = 0x0100, sType = "$D Number of Survey Elements,", sFormat = "0u" });
            ReportRef.Add(new BGReportList { iNumber = 0x0101, sType = "$G Number of SurveyII Elements", sFormat = "0u" });
            ReportRef.Add(new BGReportList { iNumber = 0x0102, sType = "$F Number of Error Elements", sFormat = "0u" });
            ReportRef.Add(new BGReportList { iNumber = 0x0103, sType = "$R Number of Report Elements", sFormat = "0u" });
            ReportRef.Add(new BGReportList { iNumber = 0x0104, sType = "CD Number of ClientData Elements", sFormat = "0u" });
            ReportRef.Add(new BGReportList { iNumber = 0x0105, sType = "Start Timestamp", sFormat = "0t" });
            ReportRef.Add(new BGReportList { iNumber = 0x0106, sType = "End Timestamp", sFormat = "0t" });
            ReportRef.Add(new BGReportList { iNumber = 0x0107, sType = "STCSTART TimeStamp", sFormat = "0t" });
            ReportRef.Add(new BGReportList { iNumber = 0x0110, sType = "FLASH capacity Used (KB)", sFormat = "0u" });
            ReportRef.Add(new BGReportList { iNumber = 0x0111, sType = "FLASH capacity in %.", sFormat = "0u" });

            //--------------------------------------------------------Report Frame. 
            //          0x0200-FF   $R Report Frame
            //          0x0300-FF   $F Error Frame
            //---------------------------------------------------------
        }

        #region----------------------------------------------------------------------ClearData
        public void ClearData()     // Clear clear data for next batch. 
        {
            ReportData = null;
            ErrorFrame = null;
            ReporFrame = null;
            ClientDataFrame = null;
            RentalDataFrame = null;
            STC_TO_Survey_TimeStamp = null;

            ReportData = new List<BGReportList>();
            ErrorFrame = new List<BGReportList>();
            ReporFrame = new List<BGReportList>();
            ClientDataFrame = new List<BGReportList>();
            RentalDataFrame = new List<BGReportList>();
            STC_TO_Survey_TimeStamp = new List<UInt32>();

            ReportDataCounter = 0;
            ReportDataNotMatchedCounter = 0;
            CountF = 0;
            CountR = 0;
            CountCD = 0;
            Survey_StartTimeStamp = 0;
            Survey_EndTimeStamp = 0;
            //--------------------------------
            SerialNumber = 0xFFFFFFFF;      // Undefined Serial Number.
            //ToCo_ToolType = 0;              // CO Default. 
            //--------------------------------
            EESysFlag = 0;
            EEBattState = 0;
            BatCapacity = 0;
            BatLastRechargeTimeStamp = 0;
            BatLastRechargeTemp = 0;
            BatLowVoltageTimeStamp = 0;
            //--------------------------------
            bSTCSTART_Button_TimeStampFound = false;
        }
        #endregion

        #region----------------------------------------------------------------------CaptureDataFrameReset
        public void CaptureDataFrameReset()     // Clear clear data for next batch. 
        {
            CaptureDataFrame = null;
            CaptureDataFrame = new List<BGCapturedList>();
            for (int i = 0; i < 5; i++)
            {
                CaptureDataFrame.Add(new BGCapturedList());
            }
        }
        #endregion

        #region----------------------------------------------------------------------isEmpty
        public bool isEmpty()
        {
            if (ReportData.Count == 0)
                return (true);
            return (false);

        }
        #endregion

        #region----------------------------------------------------------------------Math_Calculate_Directional
        public void Math_Calculate_Directional()
        {


        }
        #endregion

        #region----------------------------------------------------------------------ReportDataInsert
        public void ReportDataInsert(List<string> sPara, List<UInt32> HexPara)
        {
            int i;
            if (sPara.Count == 0)      // Received hex data only
            {
                try
                {
                    BGReportList result = ReportRef.First(x => x.iNumber == HexPara[0]);     // Hex Address type (1st Para)
                    switch (result.sFormat)
                    {
                        case ("0w"):        // INT16 only
                            {
                                //sData[y] = Tools.HexStringtoInt16(sData[y]).ToString();
                                break;
                            }
                        case ("0q"):        // uINT16 only
                            {
                                //sData[y] = Tools.HexStringtoUInt16(sData[y]).ToString();
                                break;
                            }
                        case ("0u"):
                            {
                                if (HexPara[1] == 0xFFFFFFFF)
                                    result.sNote = "Not Declared";
                                else
                                {
                                    result.sNote = HexPara[1].ToString("D");                // Extract Hex Data (2nd Para)
                                    if (HexPara[0] == 0x0010)
                                        SerialNumber = HexPara[1];
                                    if (HexPara[0] == 0x0015)
                                        BatCapacity = (int)HexPara[1];
                                    if (HexPara[0] == 0x0017)
                                        BatLastRechargeTemp = (int)HexPara[1];
                                    if (HexPara[0] == 0x0020)
                                        RentalDataFrame.Add(result);
                                    if (HexPara[0] == 0x0023)                                   //RD3: Policy No with tooltype.
                                    {
                                        RentalDataFrame.Add(result);
                                        UInt32 uRD3 = Tools.ConversionStringtoUInt32(result.sNote);
                                        //ToCo_ToolType = (int)((uRD3 >> 28) & 0x0000000F);
                                    }
                                    if (HexPara[0] == 0x001A)
                                        iSampleperoid = (int)HexPara[1];
                                }
                                break;
                            }
                        case ("0x"):
                            {
                                result.sNote = "0x" + HexPara[1].ToString("X");
                                if (HexPara[0] == 0x0013)
                                    EESysFlag = HexPara[1];
                                if (HexPara[0] == 0x0014)
                                    EEBattState = HexPara[1];
                                break;
                            }
                        case ("0t"):
                            {
                                if (HexPara[1] == 0)
                                    result.sNote = "Error";
                                else if (HexPara[1] == 0xFFFFFFFF)
                                    result.sNote = "Not Declared";
                                else
                                {
                                    //result.iNumber = HexPara[0];
                                    result.dtUTC = HexPara[1];
                                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(HexPara[1]);
                                    result.sNote = dateTimeOffset.ToString(" < dddd  ||  dd/MM/yyyy  ||  HH:mm:ss>");
                                    if (HexPara[0] == 0x0016)
                                        BatLastRechargeTimeStamp = HexPara[1];
                                    if (HexPara[0] == 0x0018)
                                        BatLowVoltageTimeStamp = HexPara[1];
                                    if (HexPara[0] == 0x0105)
                                        Survey_StartTimeStamp = HexPara[1];
                                    if (HexPara[0] == 0x0106)
                                        Survey_EndTimeStamp = HexPara[1];
                                    if (HexPara[0] == 0x0107)                   // <STCSTART> TimeStamp. 
                                    {
                                        bSTCSTART_Button_TimeStampFound = true;
                                        STCSTART_Button_TimeStamp = HexPara[1];
                                        sSTCSTART_Button_TimeStamp = HexPara[1].ToString("D");
                                    }
                                    if (HexPara[0] == 0x0021)
                                        RentalDataFrame.Add(result);
                                    if (HexPara[0] == 0x0022)
                                        RentalDataFrame.Add(result);

                                }
                                break;
                            }
                        case ("0p"):
                            {
                                if (HexPara[1] == 0)
                                    result.sNote = "Expired/Once Used";
                                result.sNote = "0x" + HexPara[1].ToString("X");     //###TASK: Mask password?
                                if (HexPara[0] == 0x0024)
                                    RentalDataFrame.Add(result);
                                break;
                            }
                        case ("0c"):
                            {
                                byte[] bytes = new byte[4];
                                bytes = BitConverter.GetBytes(HexPara[1]);
                                string s = Encoding.ASCII.GetString(bytes);
                                result.sNote = "0x" + HexPara[1].ToString("X") + " || " + s;     //###TASK: Mask password?
                                break;
                            }
                        default:
                            {
                                //###TASK: Not matched type
                                break;
                            }
                    }
                    ReportData.Add(result);

                }
                catch { };
            }
            else                // Received string data
            {
                try
                {
                    //-----------------------------------------------------------------Put data into list.
                    if (sPara[0].Contains("$F"))
                    {
                        BGReportList resultF = new BGReportList();
                        CountF++;
                        resultF.iNumber = HexPara[0];
                        resultF.dtUTC = HexPara[1];
                        if (HexPara.Count >= 3)
                            resultF.Set_EESysFlag(HexPara[2]);
                        if (HexPara.Count >= 4)
                            resultF.Set_EEBattState(HexPara[3]);
                        resultF.sType = "Error Entry No." + HexPara[0].ToString("X");
                        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(HexPara[1]);
                        resultF.sNote = dateTimeOffset.ToString("< dd/MM/yyyy  ||  HH:mm:ss >   ");
                        resultF.sNote += sPara[1];
                        ReportData.Add(resultF);
                        ErrorFrame.Add(resultF);
                    }
                    else if (sPara[0].Contains("$R"))
                    {
                        BGReportList resultR = new BGReportList();

                        CountR++;
                        resultR.iNumber = HexPara[0];
                        resultR.dtUTC = HexPara[1];
                        if (HexPara.Count >= 3)
                            resultR.Set_EESysFlag(HexPara[2]);
                        if (HexPara.Count >= 4)
                            resultR.Set_EEBattState(HexPara[3]);
                        resultR.sType = "Report Entry No." + HexPara[0].ToString("X");
                        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(HexPara[1]);
                        resultR.sNote = dateTimeOffset.ToString("< dd/MM/yyyy  ||  HH:mm:ss >   ");
                        resultR.sNote += sPara[1];
                        ReportData.Add(resultR);
                        ReporFrame.Add(resultR);
                        ////-------------------------------------------Special provision to records 
                        //if (sPara[1].Contains("<STCSTART>"))
                        //{
                        //    sSTC_START_Survey_TimeStamp = HexPara[1].ToString();
                        //    bSTC_START_Survey_TimeStampFound = true;
                        //}
                    }
                    else if (sPara[0].Contains("$CD"))
                    {
                        for (i = 0; i <= 9; i++)                // CD0 to CD9 supported only. 
                        {
                            if (sPara[0].Contains("$CD" + i.ToString()))
                            {
                                BGReportList result = new BGReportList();
                                CountCD = i;
                                result.iNumber = (UInt32)(0x0020 + i);
                                result.sType = "Client Data:" + (0x0020 + i).ToString("X");
                                result.sNote = sPara[1];
                                ReportData.Add(result);
                                ClientDataFrame.Add(result);
                            }
                        }

                    }
                }
                catch { };
            }

        }
        #endregion
    }
    #endregion

    #region//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++BGMasterDriller (Driller Survey)
    [Serializable]
    public class BGMasterCalData
    {
        #pragma warning disable IDE0044 // Add readonly modifier
        ITools Tools = new Tools();
        #pragma warning restore IDE0044 // Add readonly modifier
        //#####################################################################################################
        //###################################################################################### Variable
        //#####################################################################################################

        public List<BGToCoCalDataInfomation>    CalInfo;      // $I goes there
        public List<BGToCoCalData>              CalData;      // $S to $E goes there.
        
        private string sData;
        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public BGMasterCalData()
        {
        }

        public bool CalDataConvert(string sDataIN)
        {
            int i = 0;
            bool isCalData = false;
            List<string> sFrame;
            sData = sDataIN;            // Back up for debug later
            //-------------------------Reset Data
            sFrame = null;
            sFrame = new List<string>();

            CalInfo = null;
            CalInfo = new List<BGToCoCalDataInfomation>();

            CalData = null;
            CalData = new List<BGToCoCalData>();
            //-------------------------Detokenise sDataIN string into Info and Data
            sDataIN = sDataIN.Replace("\r", string.Empty);              // filter out unwanted '\r'
            sDataIN = sDataIN.Replace(" ", string.Empty);
            sFrame = sDataIN.Split('\n').ToList();
            try
            {
                for (i = 0; i < sFrame.Count; i++)
                {
                    if (sFrame[i].Contains("$I"))
                    {
                        CalInfo.Add(new BGToCoCalDataInfomation(sFrame[i]));
                    }
                    if (isCalData == true)
                    {
                        if (sFrame[i].Contains("$E") == true)
                            break;
                        else
                            CalData.Add(new BGToCoCalData(sFrame[i]));
                    }
                    else if (sFrame[i].Contains("$S"))
                    {
                        isCalData = true;
                    }
                }
            }
            catch
            {
                return (false);
            }
            return (true);
        }

    }
    #endregion

    #region//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++BGToCoCalDataInfomation 
    [Serializable]
    public class BGToCoCalDataInfomation
    {
        public string Name;
        public string data;
        public BGToCoCalDataInfomation(string sData)
        {
            List<string> stringList = sData.Split(',').ToList();
            if (stringList.Count >= 2)
            {
                Name = stringList[0];
                data = stringList[1];
            }
        }
    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++BGToCoCalData 
    [Serializable]
    public class BGToCoCalData
    {
        ITools Tools = new Tools();
        public string Name;
        public double dData;
        public BGToCoCalData(string sData)
        {
            List<string> stringList = sData.Split(',').ToList();
            if (stringList.Count >= 2)
            {
                Name = stringList[0];
                dData = Tools.ConversionStringtoDouble(stringList[1]);
                //if (dData == Double.NaN)       
                //    dData = 1.0;
            }
        }
    }
    #endregion

    #region//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++BGMasterDriller (Driller Survey)
    [Serializable]
    public class BGMasterDriller
    {
        //#####################################################################################################
        //###################################################################################### Getter/Setter, public
        //#####################################################################################################
        public string sDate { get; set; }
        public string sTime { get; set; }
        public double dtUTC { get; set; }
        public int iStationInterval { get; set; }
        public string sNote { get; set; }

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public BGMasterDriller()
        {
            DateTime dtx = DateTime.Now;
            TimeSpan span = (dtx - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            dtUTC = span.TotalSeconds;
            sDate = string.Format("{0:dd/MM/yy}", dtx);
            sTime = string.Format("{0:HH/mm/ss}", dtx);
            iStationInterval = 0;
            sNote = "### START SURVEY ###";
        }
    }
    #endregion
}
