using JR.Utils.GUI.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using UDT_Term;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace UDT_Term_FFT
{
    public partial class MiniAD7794Setup : Form
    {
        #region//================================================================Speed up Window Form and Controls. 
        //----------------------------------------------Speed up WinForm and Control by doing it background and then push to paint.
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;    // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        // https://social.msdn.microsoft.com/Forums/windows/en-US/aaed00ce-4bc9-424e-8c05-c30213171c2c/flickerfree-painting?forum=winforms
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        USB_VCOM_Comm myUSBVCOMComm;
        MainProg myMainProg;
        LoggerCSV myLoggerCSV;
        DMFProtocol myDMFProtocol;
        USB_Message_Manager myUSB_Message_Manager;
        //------------------------------------------Tab Window Manager
        TVMiniPort_TabEnable TabWindowEnable;
        //------------------------------------------AD7794
        List<TVMiniPort_AD7794Channel> ADC24Ch;
        TVMiniPort_AD7794TempCh ADC24TempCh;
        TVMiniPort_AD7794Setup ADC24Setup;
        List<TVMiniPort_ADCPost> ADCPostArray;   // Post Readout Gain/Offset calculation. 
        //------------------------------------------Application Specific Devices
        TVMiniPort_FCL30Setup FCL30Setup;
        TVMiniPort_ADIS16460Setup ADIS16460Setup; 
        TVMiniPort_ADIS16460DataMath ADIS16460Data;
        TVMiniPort_LoggerCVS TabLoggerCVSSetup;
        //------------------------------------------MiniPort UserControl
        ADCSupport ADCSupportTab;
        ADC24AD7794 ADCAD7794Tab;
        FCL3 FCL30Tab;
        ADIS16460 ADIS16460Tab;
        TabLoggerCVS LoggerCVSTab;

        //------------------------------------------MiniPort Setup
        BindingList<zMiniAD7794_ConfigSetup> blMiniPortSetup;
        public List<zMiniAD7794_ListData> zMiniData;
        zMiniAD7794_ListName myMiniListName;
        //------------------------------------------Class for Load and Save
        MiniAD7794SuperClass scMiniAD7794SuperClass;

        #region//================================================================Reference
        public ADIS16460 MyADIS16460()
        {
            return ADIS16460Tab;
        }
        public void MyUSB_Message_Manager(USB_Message_Manager myUSB_Message_ManagerRef)
        {
            myUSB_Message_Manager = myUSB_Message_ManagerRef;
        }
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        public void MyDMFProtocol(DMFProtocol myDMFProtocolRef)
        {
            myDMFProtocol = myDMFProtocolRef;
        }
        public void MyLoggerCVS(LoggerCSV myLoggerCSVRef)
        {
            myLoggerCSV = myLoggerCSVRef;
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
                case (50):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPageTVBBlank_1B;
                        break;
                    }
                case (100):
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
        public void MyUSBVCOMComm(USB_VCOM_Comm myUSBVCOMCommRef)
        {
            myUSBVCOMComm = myUSBVCOMCommRef;
        }
        #endregion

        //------------------------------------------------------------------------Local Variable

        //------------------------------------------------------------------------Constructor
        public MiniAD7794Setup()
        {
            InitializeComponent();
            ADIS16460Setup = new TVMiniPort_ADIS16460Setup();
            ADIS16460Setup.ADIS6460SetupInit();
            zMiniAD7794_SetupList_Blank_SetupList_Blank();
        }

        //#####################################################################################################
        //###################################################################################### TabWindow 
        //#####################################################################################################

        void TabWindowEnable_Refresh()        // true = Read, false = Write
        {
            myGlobalBase.bTabWindowEnableUpdate = false;
            ADCSupportTab.TabWindowEnableImplement();
            ADCAD7794Tab.TabWindowEnableImplement();
            ADIS16460Tab.TabWindowEnableImplement();
            FCL30Tab.TabWindowEnableImplement();
            LoggerCVSTab.TabWindowEnableImplement();
        }

        //#####################################################################################################
        //###################################################################################### Window Forms
        //#####################################################################################################

        #region //=============================================================================================MiniAD7794Setup_FormClosing
        private void MiniAD7794Setup_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            //---------------------------------- Save Survey. 
            //btnSave.PerformClick();
            //---------------------------------- Close window.
            myGlobalBase.TV_isMiniPortOpen = false;                 // Cease Survey CVS Terminal mode.
            this.Visible = false;
            this.Hide();

        }
        #endregion

        #region //=============================================================================================btnClose_Click
        private void btnClose_Click(object sender, EventArgs e)
        {
            btnSave.PerformClick();
            this.Close();
        }
        #endregion

        #region //=============================================================================================MiniAD7794Setup_Load
        private void MiniAD7794Setup_Load(object sender, EventArgs e)       // Include binding usercontrol to tabpage. 
        {
            myGlobalBase.sMiniAD7794_Filename = "";
            myGlobalBase.sMiniAD7794_Foldername = myGlobalBase.zCommon_DefaultFolder;

            #region---------------------------------------------TabWindowEnable
            TabWindowEnable = new TVMiniPort_TabEnable();
            TabWindowEnable.TabEnableDefaultSetupInit();
            #endregion

            #region---------------------------------------------Setup AD7794 classes
            //----------------------------------------------------------Class Objects
            ADC24TempCh = new TVMiniPort_AD7794TempCh();
            ADC24Setup = new TVMiniPort_AD7794Setup();

            ADC24Ch = new List<TVMiniPort_AD7794Channel>(6)            //6 channels only
            {
                new TVMiniPort_AD7794Channel(0),
                new TVMiniPort_AD7794Channel(1),
                new TVMiniPort_AD7794Channel(2),
                new TVMiniPort_AD7794Channel(3),
                new TVMiniPort_AD7794Channel(4),
                new TVMiniPort_AD7794Channel(5)
            };

            //---------------------------------------------Offset/Gain Section for ADC12 and ADC24. 
            ADCPostArray = new List<TVMiniPort_ADCPost>(9)
            {
                new TVMiniPort_ADCPost(0,"ADC24-CH1"),
                new TVMiniPort_ADCPost(1,"ADC24-CH2"),
                new TVMiniPort_ADCPost(2,"ADC24-CH3"),
                new TVMiniPort_ADCPost(3,"ADC24-CH4"),
                new TVMiniPort_ADCPost(4,"ADC24-CH5"),
                new TVMiniPort_ADCPost(5,"ADC24-CH6"),
                new TVMiniPort_ADCPost(10,"ADC12-CH1"),
                new TVMiniPort_ADCPost(11,"ADC12-CH2"),
                new TVMiniPort_ADCPost(12,"ADC12-CH3")
            };
            #endregion

            #region---------------------------------------------ADC24 Support Section (AD7794)
            ADCAD7794Tab = new ADC24AD7794();
            ADCAD7794Tab.MyGlobalBase(myGlobalBase);
            ADCAD7794Tab.MyFCL30Setup(FCL30Setup);
            ADCAD7794Tab.MyUSB_Message_Manager(myUSB_Message_Manager);
            ADCAD7794Tab.MyADC24Ch(ADC24Ch);
            ADCAD7794Tab.MyADC24TempCh(ADC24TempCh);
            ADCAD7794Tab.MyADC24Setup(ADC24Setup);
            ADCAD7794Tab.MyADCPost(ADCPostArray);
            ADCAD7794Tab.MyMainProg(myMainProg);
            ADCAD7794Tab.MyDMFProtocol(myDMFProtocol);
            ADCAD7794Tab.MyTabWindowEnable(TabWindowEnable);
            this.tpAD7794.Controls.Add(this.ADCAD7794Tab);          // Add Control Form to TabPage.
            //ADCAD7794Tab.ADC24_Refresh_Window();
            #endregion

            #region---------------------------------------------ADC Support Section
            ADCSupportTab = new ADCSupport();
            ADCSupportTab.MyADCPost(ADCPostArray);
            ADCSupportTab.MyMainProg(myMainProg);
            ADCSupportTab.MyGlobalBase(myGlobalBase);
            ADCSupportTab.MyTabWindowEnable(TabWindowEnable);
            ADCSupportTab.MyUSB_Message_Manager(myUSB_Message_Manager);
            this.tpADC.Controls.Add(this.ADCSupportTab);        // Add Control Form to TabPage.
            ADCSupportTab.ADCPostGridSetup();
            ADCSupportTab.ADCPostGrid_RefreshUpdate();
            
            #endregion

            #region---------------------------------------------FCL30 Section
            FCL30Setup = new TVMiniPort_FCL30Setup();
            FCL30Tab = new FCL3();
            FCL30Tab.MyGlobalBase(myGlobalBase);
            FCL30Tab.MyMainProg(myMainProg);
            FCL30Tab.MyFCL30Setup(FCL30Setup);
            FCL30Tab.MyUSB_Message_Manager(myUSB_Message_Manager);
            FCL30Tab.MyADC24Ch(ADC24Ch);
            FCL30Tab.MyADC24TempCh(ADC24TempCh);
            FCL30Tab.MyADC24Setup(ADC24Setup);
            FCL30Tab.MyADCPost(ADCPostArray);
            FCL30Tab.MyDMFProtocol(myDMFProtocol);
            FCL30Tab.MyTabWindowEnable(TabWindowEnable);
            this.tpFLC370.Controls.Add(this.FCL30Tab);              // Add Control Form to TabPage.
            FCL30Tab.ADCPostGridSetup();
            FCL30Tab.ADCPostGrid_RefreshUpdate();
            FCL30Tab.FCL307_Refresh_Window_Control();

            #endregion

            #region---------------------------------------------ADIS16460 Section
            ADIS16460Data = new TVMiniPort_ADIS16460DataMath();
            ADIS16460Tab = new ADIS16460();
            ADIS16460Tab.MyGlobalBase(myGlobalBase);
            ADIS16460Tab.MyMainProg(myMainProg);
            ADIS16460Tab.MyADIS16460DataMath(ADIS16460Data);
            ADIS16460Tab.MyADIS16460Setup(ADIS16460Setup);
            ADIS16460Tab.MyUSB_Message_Manager(myUSB_Message_Manager);
            ADIS16460Tab.MyDMFProtocol(myDMFProtocol);
            ADIS16460Tab.MyTabWindowEnable(TabWindowEnable);
            this.tpADIS16460.Controls.Add(this.ADIS16460Tab);          // Add Control Form to TabPage.
            #endregion

            #region---------------------------------------------LoggerCVS Section
            TabLoggerCVSSetup = new TVMiniPort_LoggerCVS();
            TabLoggerCVSSetup.LoggerCVS_SetupInit();
            LoggerCVSTab = new TabLoggerCVS();
            LoggerCVSTab.MyGlobalBase(myGlobalBase);
            LoggerCVSTab.MyMainProg(myMainProg);
            LoggerCVSTab.MyLoggerCVSSetup(TabLoggerCVSSetup);
            LoggerCVSTab.MyUSB_Message_Manager(myUSB_Message_Manager);
            LoggerCVSTab.MyDMFProtocol(myDMFProtocol);
            LoggerCVSTab.MyTabWindowEnable(TabWindowEnable);
            LoggerCVSTab.MyLoggerCVS(myLoggerCSV);
            this.tpLoggerCVS.Controls.Add(this.LoggerCVSTab);          // Add Control Form to TabPage.
            #endregion

            #region//-------------------------------------------SuperClasses for Load/Save Setup into files.
            scMiniAD7794SuperClass = new MiniAD7794SuperClass();
            scMiniAD7794SuperClass.CreateNewObjects();
            scMiniAD7794SuperClass.AD7794SetupRefIn(ADC24Setup);
            scMiniAD7794SuperClass.AD7794TempChURefIn(ADC24TempCh);
            scMiniAD7794SuperClass.AD7794ChannelRefIn(ADC24Ch);
            scMiniAD7794SuperClass.ADCPostArrayRefIn(ADCPostArray);
            scMiniAD7794SuperClass.FCL30SetupRef(FCL30Setup);
            scMiniAD7794SuperClass.ADIS16460SetupRef(ADIS16460Setup);
            scMiniAD7794SuperClass.TabLoggerCVSSetupRef(TabLoggerCVSSetup);
            scMiniAD7794SuperClass.TabEnableRefIn(TabWindowEnable);
            #endregion

            //-------------------------------------------------- Init Objects Values.
            FCL30Tab.FCL30_InitDefaultSetupFirstTime();
            //-------------------------------------------------- Default TabPage.
            tcMainFeature.SelectedTab = tpLoggerCVS;
        }
        #endregion

        #region //=============================================================================================MiniSetup_Show
        public void MiniSetup_Show()
        {
            this.BringToFront();
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            myGlobalBase.TV_isMiniPortOpen = true;
            this.Visible = true;
            this.Show();
            this.Focus();
            this.TopMost = false;
        }

        #region //=============================================================================================MiniSetup_Show
        private void tcMainFeature_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (myGlobalBase.bTabWindowEnableUpdate==true)
            {
                myGlobalBase.bTabWindowEnableUpdate = false;
                ADCSupportTab.TabWindowEnableImplement();
                ADCAD7794Tab.TabWindowEnableImplement();
                ADIS16460Tab.TabWindowEnableImplement();
                FCL30Tab.TabWindowEnableImplement();
            }
            switch (tcMainFeature.SelectedIndex)
            {
                case (2):       //ADCSupportTab
                    {
                        ADCSupportTab.isUserCellChange = false;     // Block events when cell in datagridview is changed (which cause update issue)
                        FCL30Tab.isUserCellChange = false;
                        FCL30Tab.ADCPostGrid_ReadUpdate();
                        ADCSupportTab.ADCPostGrid_RefreshUpdate();
                        ADCSupportTab.isUserCellChange = true;
                        FCL30Tab.isUserCellChange = true;
                        break;
                    }
                case (3):       //AD7794Tab
                    {
                        FCL30Tab.isUserCellChange = false;
                        FCL30Tab.ADCPostGrid_RefreshUpdate();
                        FCL30Tab.FCL307_Refresh_Window_Control();
                        ADCAD7794Tab.ADC24_Refresh_Window();
                        FCL30Tab.isUserCellChange = true;
                        break;
                    }
                case (8):       //ADIS16460
                    {
                        ADIS16460Tab.ADIS16460_Refresh_Window_Control(true);
                        break;
                    }
                case (9):       //FCL30Tab
                    {
                        ADCSupportTab.isUserCellChange = false;     // Block events when cell in datagridview is changed (which cause update issue)
                        FCL30Tab.isUserCellChange = false;
                        ADCAD7794Tab.ADC24_Refresh_Window();
                        ADCSupportTab.ADCPostGrid_RefreshUpdate();
                        FCL30Tab.ADCPostGrid_RefreshUpdate();
                        FCL30Tab.FCL307_Refresh_Window_Control();
                        ADCSupportTab.isUserCellChange = true;
                        FCL30Tab.isUserCellChange = true;
                        break;
                    }
                default:
                    break;
            }

        }
        #endregion


        private void btnUpdate_Click(object sender, EventArgs e)
        {

        }

        #endregion

        //#####################################################################################################
        //###################################################################################### MiniAD7794 MiniPort Supports
        //#####################################################################################################

        #region --------------------------------------------------------zMiniAD7794_SetupList_Blank_SetupList_Blank()
        private void zMiniAD7794_SetupList_Blank_SetupList_Blank()
        {
            if (blMiniPortSetup == null)
            {
                blMiniPortSetup = new BindingList<zMiniAD7794_ConfigSetup>();
                zMiniData = new List<zMiniAD7794_ListData>();
                for (int i = 0; i < 10; i++)
                {
                    blMiniPortSetup.Add(new zMiniAD7794_ConfigSetup());
                    zMiniData.Add(new zMiniAD7794_ListData());
                }
            }
            zMiniAD7794_SetupList_Blank_Config_Clear();
        }
        #endregion

        #region --------------------------------------------------------zMiniAD7794_SetupList_Blank_SetupDefault()
        private void zMiniAD7794_SetupList_Blank_SetupDefault()      // GPIO updated default setting (4/11/15) which in sync with the zEVKit board firmware. 
        {
            zMiniAD7794_SetupList_Blank_Config_Clear();
            //------------------------------------------------------------------------------------------------Config Ana Port
            //-------------------------------
            for (int i = 0; i <10; i++)
            {
                blMiniPortSetup[i].wConfigGPIO = (UInt32)(1 << (int)zMiniAD7794_ConfigSetup.eConfigGPIO.GIN);
                zMiniData[i].isGIN = true;
            }
        }
        #endregion

        #region --------------------------------------------------------zMiniAD7794_SetupList_Blank_Config_Clear()
        private void zMiniAD7794_SetupList_Blank_Config_Clear()
        {
            for (int i = 0; i < 10; i++)
            {
                blMiniPortSetup[i].wConfigMixed = 0;
                blMiniPortSetup[i].wConfigGPIO = 0;
                blMiniPortSetup[i].wConfigSerial = 0;
                blMiniPortSetup[i].wConfigUtil = 0;
            }

        }
        #endregion

        #region --------------------------------------------------------zMiniAD7794_SetupList_Blank_Data_Clear()
        private void zMiniAD7794_SetupList_Blank_Data_Clear()
        {
            for (int i = 0; i < 22; i++)
            {
                zMiniData[i].ClearAllX();
                zMiniData[i].GState = false;
            }

        }
        #endregion


        //#####################################################################################################
        //###################################################################################### Save/Load Setup (with superclass)
        //#####################################################################################################

        #region //-------------------------------------------------------------------selectFolderToolStripMenuItem_Click
        private void selectFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = myGlobalBase.sMiniAD7794_Foldername;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                //
                // The user selected a folder and pressed the OK button.
                // We print the number of files found.
                //
                myGlobalBase.sMiniAD7794_Foldername = folderBrowserDialog1.SelectedPath;
                if (!Directory.Exists(myGlobalBase.sMiniAD7794_Foldername))
                {
                    DirectoryInfo di = Directory.CreateDirectory(myGlobalBase.sMiniAD7794_Foldername);  // Create folder if not exist. 
                }
            }
            //txtFolderName.Text = sFoldername;
            //txtFilename.Text = "To be Generated";

            //sAppendFileName = System.IO.Path.Combine(sFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_AppendNote.csv"));
            //sErrorFileName = System.IO.Path.Combine(sFoldername, (DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + "_ExceptionReport.txt"));
        }
        #endregion

        #region //-------------------------------------------------------------------btnLoad_Click
        private void btnLoad_Click(object sender, EventArgs e)
        {
            Load_AllSetupFile();
        }
        #endregion

        #region //-------------------------------------------------------------------LoadSetupFile
        public bool Load_AllSetupFile()
        {
            MiniAD7794SuperClass LoadClass;
            //-------------------------------------------------------------------------
            ofLoadSetup.Filter = "xml files (*.xml)|*.xml";
            ofLoadSetup.InitialDirectory = myGlobalBase.sMiniAD7794_Foldername;
            ofLoadSetup.Title = "Load XML MiniAD7794 Setting Files";
            DialogResult dr = ofLoadSetup.ShowDialog();
            if (dr == DialogResult.OK)
            {
                myGlobalBase.sMiniAD7794_Foldername = Path.GetDirectoryName(ofLoadSetup.FileName);
                myGlobalBase.sMiniAD7794_Filename = Path.GetFileName(ofLoadSetup.FileName);      // Just a filename, no folder path. 
                LoadClass = myGlobalBase.DeserializeFromFile<MiniAD7794SuperClass>(ofLoadSetup.FileName);

                if (scMiniAD7794SuperClass == null)
                {
                    MessageBox.Show("Unable to Load Gain/Offset Setting Configuration File (XML)",
                        "Load Config File Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    myMainProg.myRtbTermMessageLF("#E: MiniAD7794: Possible Serialize/filename error, revert to previous setting");
                    return (false);
                }
                myMainProg.myRtbTermMessageLF("-I: Successfully Load Setup File from: " + ofLoadSetup.FileName.ToString());
                //-----------------------------------------------------------------------------
                scMiniAD7794SuperClass.CopyObjects(LoadClass);
                scMiniAD7794SuperClass.TabWindowUpdateReadData(TabWindowEnable);
                scMiniAD7794SuperClass.ADIS16460UpdateReadData(ADIS16460Setup);
                scMiniAD7794SuperClass.FCL307UpdateReadData(FCL30Setup);
                scMiniAD7794SuperClass.LoggerCVSUpdateReadData(TabLoggerCVSSetup);
                //-----------------------------------------------------------------------------
                TabWindowEnable_Refresh();
                ADCAD7794Tab.ADC24_Refresh_Window();
                ADCSupportTab.ADCPostGrid_RefreshUpdate();
                FCL30Tab.ADCPostGrid_RefreshUpdate();
                FCL30Tab.FCL307_Refresh_Window_Control();
                ADIS16460Tab.ADIS16460_Refresh_Window_Write();
                LoggerCVSTab.LoggerCVS_Refresh_Window_Write();
                return (true);
            }
            return (false);
        }
        #endregion

        #region //-------------------------------------------------------------------btnSave_Click
        private void btnSave_Click(object sender, EventArgs e)
        {
            ADCAD7794Tab.ADC24_Refresh_Window();
            ADCSupportTab.ADCPostGrid_RefreshUpdate();
            FCL30Tab.ADCPostGrid_RefreshUpdate();
            FCL30Tab.FCL307_Refresh_Window_Control();
            ADIS16460Tab.ADIS16460_Refresh_Window_Read();
            LoggerCVSTab.LoggerCVS_Refresh_Window_Read();
            //-----------------------------------------------------------------------------
            scMiniAD7794SuperClass.TabWindowUpdateWriteData(TabWindowEnable);
            scMiniAD7794SuperClass.ADIS16460UpdateWriteData(ADIS16460Setup);
            scMiniAD7794SuperClass.FCL307UpdateWriteData(FCL30Setup);
            scMiniAD7794SuperClass.LoggerCVSUpdateWriteData(TabLoggerCVSSetup);
            //-----------------------------------------------------------------------------
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = myGlobalBase.sMiniAD7794_Foldername;
            saveFileDialog1.Filter = "xml files (*.xml)|*.xml";
            saveFileDialog1.Title = "Save an XML Save MiniAD7794 Setup Files";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                myGlobalBase.sMiniAD7794_Foldername = Path.GetDirectoryName(saveFileDialog1.FileName);
                myGlobalBase.sMiniAD7794_Filename = Path.GetFileName(saveFileDialog1.FileName);      // Just a filename, no folder path. 
                try
                {
                    myGlobalBase.SerializeToFile<MiniAD7794SuperClass>(scMiniAD7794SuperClass, saveFileDialog1.FileName);
                    myMainProg.myRtbTermMessageLF("-I: Successfully Save Setup File to: "+ saveFileDialog1.FileName.ToString());
                    Clipboard.SetDataObject(myGlobalBase.sMiniAD7794_Foldername);   // Copy filename to clipboard to open later
                }
                catch
                {
                    MessageBox.Show("Error: Unable to save Setup File. Possible Serialize/filename error.", "Error Save Job File",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    myMainProg.myRtbTermMessageLF("#E: MiniAD7794: Possible Serialize/filename error during setup save");
                }
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: MiniAD7794: Save Setup failed due to Filename Error");
            }
            Clipboard.SetDataObject(saveFileDialog1.FileName);                                 // Copy (set clipboard)
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Export and Import All
        //#####################################################################################################

        #region --------------------------------------------------------btnExportAll_Click()
        private void btnExportAll_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            Export_AllSetupToMiniAD7794();
        }
        #endregion

        #region --------------------------------------------------------Export_AllSetupToMiniAD7794()
        public void Export_AllSetupToMiniAD7794()
        {
            //myDMFProtocol.DMFProtocol_Reset();      // Clear old issue, clean slate.
            //-----------------------------------------------------Reset Device (all Default OFF State)
            //###TASK issue command with password, ie, this turn off all module and set Miniport to GIN state as well as PSU, 
            //###TASK +MINIRESET(0x6712)
            //###TASK -MINIRESET(0xFFFF;0x00)   //Success!.
            myMainProg.myRtbTermMessageLF("-------------------------------------------");
            //-----------------------------------------------------AD7794
            if (TabWindowEnable.bAD7794 == true)
            {
                DMFP_BulkFrame cDMFP_BulkFrame = ADCAD7794Tab.ADC24_ONOFF(true);
                while (cDMFP_BulkFrame.isBusy == true)
                {
                    Tools.InteractivePause(TimeSpan.FromMilliseconds(25));
                }
                cDMFP_BulkFrame = ADCAD7794Tab.ADC24_Update_Setup_Data();
                while (cDMFP_BulkFrame.isBusy == true)
                {
                    Tools.InteractivePause(TimeSpan.FromMilliseconds(25));
                }
                myMainProg.myRtbTermMessageLF("#I:Bulk Export: AD7794 Setup Done");
            }
            //-----------------------------------------------------Support
            if (TabWindowEnable.bADCSupport == true)
            {
                DMFP_BulkFrame cDMFP_BulkFrame = ADCSupportTab.ADCPost_ExportSetup_Click();
                while (cDMFP_BulkFrame.isBusy == true)
                {
                    Tools.InteractivePause(TimeSpan.FromMilliseconds(25));
                }
                myMainProg.myRtbTermMessageLF("#I:Bulk Export: AD7794 Post/Support Setup Done");
            }
            //-----------------------------------------------------FCL370
            if (TabWindowEnable.bFLC370 == true)
            {
                DMFP_BulkFrame cDMFP_BulkFrame = FCL30Tab.FCL30_Update_Setup_Data();
                while (cDMFP_BulkFrame.isBusy == true)
                {
                    Tools.InteractivePause(TimeSpan.FromMilliseconds(25));
                }
                myMainProg.myRtbTermMessageLF("#I:Bulk Export: FCL7-30 Setup Done");
            }
            //-----------------------------------------------------ADIS16460
            if (TabWindowEnable.bADIS16460 == true)
            {
                DMFP_BulkFrame cDMFP_BulkFrame = ADIS16460Tab.ADIS_Update_Setup_Data(false);
                while (cDMFP_BulkFrame.isBusy == true)
                {
                    Tools.InteractivePause(TimeSpan.FromMilliseconds(25));
                }
                myMainProg.myRtbTermMessageLF("#I:Bulk Export: ADIS16460 Setup Done");
            }
            //-----------------------------------------------------bSerialUART
            if (TabWindowEnable.bSerialUART == true)
            {

            }
            //-----------------------------------------------------bSerialSPI
            if (TabWindowEnable.bSerialSPI == true)
            {

            }
            //-----------------------------------------------------bSerialI2C
            if (TabWindowEnable.bSerialI2C == true)
            {

            }
            //-----------------------------------------------------bTabMiniPort (includes Minilocks and PSU Control feature)
            if (TabWindowEnable.bTabMiniPort == true)
            {

            }
            //-----------------------------------------------------bADC12 (PSU Readout and MiniPort Channels)
            if (TabWindowEnable.bADC12 == true)
            {

            }
            //-----------------------------------------------------bLogMemory
            if (TabWindowEnable.bLogMemory == true)
            {
                myMainProg.myRtbTermMessageLF("#I:Bulk Export: Logger FLASH Setup Done");
            }
            //-----------------------------------------------------bLoggerCVS
            if (TabWindowEnable.bLoggerCVS == true)
            {
                DMFP_BulkFrame cDMFP_BulkFrame = LoggerCVSTab.LoggerCVS_Update_Setup_Data();
                while (cDMFP_BulkFrame.isBusy == true)
                {
                    Tools.InteractivePause(TimeSpan.FromMilliseconds(25));
                }
                myMainProg.myRtbTermMessageLF("#I:Bulk Export: LoggerCVS Setup Done");
            }
            myMainProg.myRtbTermMessageLF("-------------------------------------------");
        }
        #endregion

        #region --------------------------------------------------------btnImportAll_Click()
        private void btnImportAll_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }

        }
        #endregion

        #region --------------------------------------------------------btnLoadExportAll_Click()
        private void btnLoadExportAll_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            if (Load_AllSetupFile()==false)
            {
                myMainProg.myRtbTermMessageLF("#E: LOAD File Error or corrupted data, unable to export setup to device");
                return;
            }
            Export_AllSetupToMiniAD7794();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### LoggerCVS
        //#####################################################################################################

        #region //=============================================================================================LogCVS_MiniAD7794_ASYNC_Send_RTC_StartLog()
        public DMFP_BulkFrame LoggerCVS_MiniAD7794_ASYNC_Send_RTC_StartLog()
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return null;
            }
            myLoggerCSV.Download_Data_Processor();
            DateTime dt2 = DateTime.Now;
            DateTime dt = dt2.ToLocalTime();
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();
            string command = "+RTCSETTIME(";
            command += String.Format("{0:HH;mm;ss}", dt);
            command += ";" + String.Format("{0:yyyy;MM;dd}", dt);
            command += ")";
            cDMFP_BulkFrame.AddItem(command, new DMFP_Delegate(DMFP_MiniAD7794_RTCSETTIME_CallBack));
            //---------------------------------------------------------------------------------------------------------
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 10;
            cDMFP_BulkFrame.isNoErrorReport = true;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);
            return (cDMFP_BulkFrame);
        }
        #endregion

        #region //=============================================================================================DMFP_LOGCVSSETUP_CallBack and +LOGCVSPROJNAME(....)
        public void DMFP_MiniAD7794_RTCSETTIME_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFFFF)
            {
                myMainProg.myRtbTermMessageLF("#E: +RTCSETTIME(...)...has Error code: 0x" + hPara[0].ToString("X4") + "| DataCode 0x" + hPara[1].ToString("X4"));
                return;
            }
            if ((TabWindowEnable.bADIS16460 == true))// && (ADIS16460Setup.bIsPlugged == true))
            {
                if (ADIS16460Tab.ADIS_RunMode(2, true) == null)
                {
                    myMainProg.myRtbTermMessageLF("#E: +ADISRUNSTOP(2) returned an error, unable to run LoggerCVS operation, this is bug, please report");
                    return;
                }
            }
            //---------------------------------
            //----------------------------------
            myUSB_Message_Manager.LoggerMessageRX = "";
            string sLoggerEntryTxt = "STC_START(1)";             // Enable Export CVS and EEPROM store. 
            myMainProg.myRtbTermMessageLF(sLoggerEntryTxt);
            myGlobalBase.LoggerOpertaionMode = true;
            myUSB_Message_Manager.endoflinedetected = false;
            myLoggerCSV.MiniAD7794_Logger_Command_ASCII_Process(sLoggerEntryTxt);
        }
        #endregion
    }

    //#####################################################################################################
    //###################################################################################### All MiniPort Ports Classes must go here!
    //#####################################################################################################

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++zMiniAD7794_ListData
    public class zMiniAD7794_ListData
    {
        // =====================================================constructor
        public zMiniAD7794_ListData()
        {
        }
        //======================================================Getter/Setter
        public bool isGIN { get; set; }             // true = GIN type
        public bool isGOUT { get; set; }            // true = GOUT type
        public bool isGOD { get; set; }            // true = GOUT type
        public bool isADC12 { get; set; }           // true = ADC12 type
        public bool isI2C { get; set; }             // true = I2C type
        public bool isSPI { get; set; }             // true = SPI type
        public bool isSPICS { get; set; }           // true = SPI-ChipSelect type
        public bool isIOCOMM { get; set; }          // true = IOCOMM type
        public bool isUART { get; set; }            // true = UART type
        public bool GState { get; set; }            // Low or HIGH state
        public Int32 ADCData12 { get; set; }         // ADC12 captured data
        public Int32 ADCData16 { get; set; }         // ADC16 captured data

        public void ClearAllX()
        {
            isGIN = false;
            isGOUT = false;
            isGOD = false;
            isADC12 = false;
            isI2C = false;
            isSPI = false;
            isIOCOMM = false;
            isUART = false;
            isSPICS = false;
            GState = false;
            ADCData12 = 0;
            ADCData16 = 0;
        }

        public void UpdateSelection(int index, zMiniAD7794_ConfigSetup Setup)
        {
            UInt32 state;
            ClearAllX();
            //------------------------------------------------------------------------------------------Mixed
            state = (Setup.wConfigMixed) & ((UInt32)(1 << (int)zMiniAD7794_ConfigSetup.eConfigMixed.ADC12));
            if (state != 0)
            {
                ADCData12 = 0;
                isADC12 = true;
            }
            //------------------------------------------------------------------------------------------GPIO
            state = (Setup.wConfigGPIO) & ((UInt32)(1 << (int)zMiniAD7794_ConfigSetup.eConfigGPIO.GIN));
            if (state != 0)
            {
                isGIN = true;
            }
            state = (Setup.wConfigGPIO) & ((UInt32)(1 << (int)zMiniAD7794_ConfigSetup.eConfigGPIO.GOUT));
            if (state != 0)
            {
                isGOUT = true;
            }
            state = (Setup.wConfigGPIO) & ((UInt32)(1 << (int)zMiniAD7794_ConfigSetup.eConfigGPIO.GOD));
            if (state != 0)
            {
                isGOD = true;
            }
            //----------------------------------------------------------------------------------------Serial
            state = (Setup.wConfigSerial) & ((UInt32)(1 << (int)zMiniAD7794_ConfigSetup.eConfigSerial.SPI0));
            if (state != 0)
            {
                isSPI = true;
            }
            state = (Setup.wConfigSerial) & ((UInt32)(1 << (int)zMiniAD7794_ConfigSetup.eConfigSerial.I2C));
            if (state != 0)
            {
                isI2C = true;
            }
            state = (Setup.wConfigSerial) & ((UInt32)(1 << (int)zMiniAD7794_ConfigSetup.eConfigSerial.IOCOMM));
            if (state != 0)
            {
                isIOCOMM = true;
            }
            state = (Setup.wConfigSerial) & ((UInt32)(1 << (int)zMiniAD7794_ConfigSetup.eConfigSerial.UART));
            if (state != 0)
            {
                isUART = true;
            }
            state = (Setup.wConfigSerial) & ((UInt32)(1 << (int)zMiniAD7794_ConfigSetup.eConfigSerial.SPICS));
            if (state != 0)
            {
                isSPICS = true;
            }
            //----------------------------------------------------------------------------------------Util
        }

    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++zMiniAD7794_ListName
    [Serializable]
    public class zMiniAD7794_ListName
    {
        public List<string> sPortMixed;
        public List<string> sPortGPIO;
        public List<string> sPortSerial;
        public List<string> sPortUtil;

        // ==================================================================Constructor
        public zMiniAD7794_ListName()
        {
            sPortMixed = new List<string>(new string[] { "DAC12", "ADC12", "ADC16_TRIG", "ANACOMP" });
            sPortGPIO = new List<string>(new string[] { "GIN", "GOUT", "GOD", "GINT", "GPullUp", "GPullDn" });
            sPortSerial = new List<string>(new string[] { "I2C", "SPI0", "SPICS", "UART", "IOCOMM" });
            sPortUtil = new List<string>(new string[] { "PWM_OUT", "SCT_IN0", "SCT_IN1", "MRT0", "MRT1", "RTCSec" });
        }

        //====================================================================Get and Set Config and Policy Bits. 

        #region --------------------------------------------------------GetConfigIndex (Read Config bit)
        public UInt32 GetConfigIndex(UInt32 Data32, UInt32 index)
        {
            return ((Data32 >> (int)index) & 1);
        }
        #endregion

        #region --------------------------------------------------------SetConfigIndex (Set Config bit)
        public UInt32 SetConfigIndex(UInt32 Data32, UInt32 index, int setting)
        {
            if (setting == 0)
            {
                Data32 = Data32 & (UInt32)(~(1 << (int)index));     // clear bit
            }
            else
            {
                Data32 = Data32 | (UInt32)(1 << (int)index);
            }
            return Data32;
        }
        #endregion

        #region --------------------------------------------------------UInt32 GetStringBinary(string s)
        public UInt32 GetStringBinary(string s)
        {
            return Convert.ToUInt32(s, 2);
        }
        #endregion

        #region --------------------------------------------------------string GetBinaryString(UInt32 n)
        public string GetBinaryString(UInt32 n)
        {
            char[] b = new char[32];
            int pos = 31;
            int i = 0;

            while (i < 32)
            {
                if ((n & (1 << i)) != 0)
                    b[pos] = '1';
                else
                    b[pos] = '0';
                pos--;
                i++;
            }
            return new string(b);
        }
        #endregion

    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++zMiniAD7794_ConfigSetup
    // Set of class object's of member being transformed into element in XML.
    // When change take place, ie added element (member), must delete old XMLConfig file. 
    [Serializable]
    public class zMiniAD7794_ConfigSetup
    {
        // ==================================================================Getter/Setter
        public bool isLoggerEnabled { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        public UInt32 wConfigMixed { get; set; }
        public UInt32 wConfigGPIO { get; set; }
        public UInt32 wConfigSerial { get; set; }
        public UInt32 wConfigUtil { get; set; }


        //===================================================================Enum_Config
        public enum eConfigMixed
        {
            DAC12 = 0,
            ADC12 = 1,
            ADC12_TRIG = 2,
            ANACOMP = 3
        }
        [Flags]
        public enum eConfigGPIO
        {
            GIN = 0,
            GOUT = 1,
            GOD = 2,
            GINT = 3,
            GPullUp = 4,
            GPullDown = 6
        }
        [Flags]
        public enum eConfigSerial
        {
            I2C = 0,
            SPI0 = 1,
            SPICS = 2,
            UART = 3,
            IOCOMM = 4
        }

        [Flags]
        public enum eConfigUtil
        {
            PWM_OUT = 0,
            SCT_IN0 = 1,
            SCT_IN1 = 2,
            MRT0 = 3,
            MRT1 = 4,
            RTCSec = 5
        }

        // ==================================================================Constructor
        public zMiniAD7794_ConfigSetup()
        {
        }
        // ==================================================================Supports
        public void DefaultSetup()      // Set to GIN
        {
            isLoggerEnabled = false;
            wConfigMixed = 0;
            wConfigGPIO = 0x0001;
            wConfigSerial = 0;
            wConfigUtil = 0;
            sDescription = "";
            sName = "";
        }

    }
    #endregion

    //#####################################################################################################
    //###################################################################################### All Application Classes must go here!
    //#####################################################################################################

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++TVMiniPort_TabEnable
    [Serializable]
    public class TVMiniPort_TabEnable
    {
        // This code enable and disable Tab Window (Master for all). 
        //#####################################################################################################
        //###################################################################################### TVMiniPort_TabEnable Getter/Setter, public
        //#####################################################################################################
        public bool bLoggerCVS { get; set; }
        public bool bLogMemory { get; set; }
        public bool bADCSupport { get; set; }          
        public bool bAD7794 { get; set; }          
        public bool bADC12 { get; set; }
        public bool bTabMiniPort { get; set; }
        public bool bSerialI2C { get; set; }           
        public bool bSerialSPI { get; set; }
        public bool bSerialUART { get; set; }
        public bool bSerialIOComm { get; set; }
        public bool bADIS16460 { get; set; }
        public bool bFLC370 { get; set; }

        // private so it will not be saved
        //#####################################################################################################
        //###################################################################################### TVMiniPort_TabEnable Constructor
        //#####################################################################################################
        public TVMiniPort_TabEnable()     // Default setup, No parameter allowed (NET bug)
        {
        }
        public void TabEnableDefaultSetupInit()
        {
            bLoggerCVS = true;
            bLogMemory = false;
            bADCSupport = false;
            bAD7794 = false;
            bADC12 = false;
            bTabMiniPort = false;
            bSerialI2C = false;
            bSerialSPI = false;
            bSerialUART = false;
            bSerialIOComm = false;
            bADIS16460 = true;
            bFLC370 = true;
        }
    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++TVMiniPort_LoggerCVS
    [Serializable]
    public class TVMiniPort_LoggerCVS
    {
        //#####################################################################################################
        //###################################################################################### LoggerCVS Getter/Setter, public
        //#####################################################################################################
        public int iSelectTrigger { get; set; }       // 0 = Timer, 1 = ADIS16460 (Gyro), 2 = etc.....10 = MiniPort0, 11 = MiniPort1, etc
        public int iTimerPeroid { get; set; }         // 0 = Not Used, 1 to ...... , mSec. 
        public int iCounterToCease{ get; set; }       // 0 = Forever, 1 = one capture only, 10 = ten capture only
        public bool isSleepEnabled { get; set; }      // 0 = Keep powered (drain battery), 1 = goto sleep. This is linkd to iCounterToCease
        public bool bAsyncUDTEnabled { get; set; }    // 0 = disable, 1 = enabled, export to UDT via USB Port
        public bool bAsyncLogEnabled { get; set; }    // 0 = disable, 1 = enabled, export to Logger FLASH memory.
        public bool bIsEnable { get; set; }           // 0 = Not Active or Logging Mode, 1 = Active Logging Mode, cannot repeat command. 
        public string sProjectName { get; set; }       // Description about project (max 128 char)

        //#####################################################################################################
        //###################################################################################### LoggerCVS Constructor
        //#####################################################################################################
        public TVMiniPort_LoggerCVS()     // Default setup, No parameter allowed (NET bug)
        {
        }
        public void LoggerCVS_SetupInit()
        {
            iSelectTrigger = 0;
            iTimerPeroid = 1000;        // 1 second
            iCounterToCease = 0;        // Forever
            isSleepEnabled = false;
            bAsyncUDTEnabled = true;    // Export to UDT
            bAsyncLogEnabled = false;   // Export to logger flash. (Future project). 
            bIsEnable = false;
            sProjectName = "Do not exceed 128 chars!";
        }
        //---------------------------------------------------------------------List Trigger (ASYNC)
        private List<string> SelectTrigger = new List<string>(new string[] {
        "0=Timer Trigger",
        "1=ADIS16460 (Gyro)",
        "2=Spare (Not Used)",
        "3=Spare (Not Used)",
        "4=Spare (Not Used)"
        });
        public List<string> lSelectTrigger()
        {
            return (SelectTrigger);
        }

    }
        #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++TVMiniPort_ADIS16460Setup
    [Serializable]
    public class TVMiniPort_ADIS16460Setup
    {
        //#####################################################################################################
        //###################################################################################### ADC24 Getter/Setter, public
        //#####################################################################################################
        public int iAccelFormat { get; set; }         // iParameter[0] 0 = RAW, 1 = G (Double), 2 = uG (INT32)
        public int iGyroFormat { get; set; }          // iParameter[1]  0 = RAW, 1 = deg/Sec (Double), 2 = uDeg/Sec (INT32)
        public int iAxisPolicy { get; set; }          // iParameter[2]  0 = OGT, 1 = XYZ, 2 = MRC
        public bool bAccelMath { get; set; }          // iParameter[3]  0 = No Accel Math, 1 = Accel Math (to UDT and Debug).
        public bool bAdvFrame { get; set; }           // iParameter[4]  0 = Exclude Velicity and DeltaAngle. 1 = Include all.
        public bool bRunStop { get; set; }            // iParameter[5]  0 = Stop Operation, 1 = Run Operation. 
        public bool bIsPlugged { get; set; }          // iParameter[6] Read Only, 1 = ADIS16460 is connected and passed serial ID test, 0 = ADIS16460 is not connected, set MiniPort to safe mode.

        //#####################################################################################################
        //###################################################################################### ADC24 Constructor
        //#####################################################################################################
        public TVMiniPort_ADIS16460Setup()     // Default setup, No parameter allowed (NET bug)
        {
        }
        public void ADIS6460SetupInit()
        {
            iAccelFormat = 2;
            iGyroFormat = 2;
            iAxisPolicy = 2;                // Select MRC for now since this math is working correctly.
            bAccelMath = true;
            bAdvFrame = false;              // No need for velocity and delta-angle.
            bRunStop = false;              // Because it not connected yet, do this when bIsPlugged is set.
            bIsPlugged = false;             // Read only from MiniAD7794. 
        }
    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++TVMiniPort_ADIS16460DataMath
    public class TVMiniPort_ADIS16460DataMath
    {
        public Vector3 dAccel;
        public Vector3 dGyro;
        public iVector3 iAccel;
        public iVector3 iGyro;

        public iVector3 iVelocity;
        public iVector3 iDeltaAngle;
        public double dAInclination { get; set; }            // Radian
        public double dAToolFace    { get; set; }
        public double dAMagnitide   { get; set; }
        public Int32 Temp           { get; set; }

        public TVMiniPort_ADIS16460DataMath()
        {
            dAccel = new Vector3();
            dGyro = new Vector3();
            iAccel = new iVector3();
            iGyro = new iVector3();
            iVelocity = new iVector3();
            iDeltaAngle = new iVector3();
            Temp = 0;
            dAInclination = 0.0;
            dAToolFace = 0.0;
            dAMagnitide = 0.0;
        }
        //------------------------------------AccelProcessMath
        public bool AccelProcessMath(Vector3 dAccIN)
        {
            //###TASK: WIP, the math is done within the firmware, not sure if this is needed or not. FCL30 math is done within UDT at present design. 
            return (false);
        }
        //------------------------------------GyroProcessMath
        public bool GyroProcessMath(Vector3 dGyroIN)
        {
            //###TASK: WIP, the math is done within the firmware, not sure if this is needed or not. FCL30 math is done within UDT at present design. 
            return (false);
        }
        //------------------------------------AccelConvertINT32toDouble
        public Vector3 AccelConvertINT32toDouble(double dScale)
        {
            Vector3 dAccelOUT = new Vector3();
            dAccelOUT.X = (Convert.ToDouble(dAccel.X)) / dScale;
            dAccelOUT.Y = (Convert.ToDouble(dAccel.Y)) / dScale;
            dAccelOUT.Z = (Convert.ToDouble(dAccel.Z)) / dScale;
            return dAccelOUT;
        }
        //------------------------------------GyroConvertINT32toDouble
        public Vector3 GyroConvertINT32toDouble(double dScale)
        {
            Vector3 dGyroOUT = new Vector3();
            dGyroOUT.X = (Convert.ToDouble(dGyro.X)) / dScale;
            dGyroOUT.Y = (Convert.ToDouble(dGyro.Y)) / dScale;
            dGyroOUT.Z = (Convert.ToDouble(dGyro.Z)) / dScale;
            return dGyroOUT;
        }
        //------------------------------------AccelProcessCalibration
        public Vector3 AccelProcessCalibration(Vector3 dAccelIN)
        {
            Vector3 dAccelOUT = new Vector3();
            dAccelOUT.X = dAccelIN.X;
            dAccelOUT.Y = dAccelIN.Y;
            dAccelOUT.Z = dAccelIN.Z;
            return dAccelOUT;
        }
        //------------------------------------Normal ADIS16460 Raw Data axis to Honeywell XYZ for math. (The axis flix is done by MiniPort firmeware. 
        public Vector3 AccelGyroNormaliseAxisForMath(Vector3 dAccelIN)
        {
            //----------------------------Accel Section
            Vector3 dAccelOUT = new Vector3();
            dAccelOUT.X = dAccelIN.Z;
            dAccelOUT.Y = -dAccelIN.Y;
            dAccelOUT.Z = dAccelIN.X;
            //----------------------------Gyro Section

            return dAccelOUT;
        }
        //------------------------------------ClearAllData
        public void ClearAllData()
        {
            dAccel.X = 0.0;
            dAccel.Y = 0.0;
            dAccel.Z = 0.0;

            dGyro.X = 0.0;
            dGyro.Y = 0.0;
            dGyro.Z = 0.0;

            iAccel.X = 0;
            iAccel.Y = 0;
            iAccel.Z = 0;

            iGyro.X = 0;
            iGyro.Y = 0;
            iGyro.Z = 0;

            iVelocity.X = 0;
            iVelocity.Y = 0;
            iVelocity.Z = 0;

            iDeltaAngle.X = 0;
            iDeltaAngle.Y = 0;
            iDeltaAngle.Z = 0;

            Temp = 0;
            dAInclination = 0.0;
            dAToolFace = 0.0;
            dAMagnitide = 0.0;
        }

    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++TVMiniPort_AD7794Setup
    [Serializable]
    public class TVMiniPort_AD7794Setup
    {
        //#####################################################################################################
        //###################################################################################### ADC24 Getter/Setter, public
        //#####################################################################################################
        public bool bCapType { get; set; }          // iParameter[0] CapType	0 = Single Read (MODE/CONFIG), 1 = Continuous Capture(CONFIG ONLY)
        public bool bRTDMode { get; set; }          // iParameter[1] RTD		0 = Disabled, 1 = RTD sensor activated
        public bool bInternalTemp { get; set; }     // iParameter[2] IntTerm	0 = Disabled, 1 = Ch-7 AD&794 Temp Sensor Activated.
        public int uSpeed { get; set; }            // iParameter[3] Speed		0 = Slow, 1 = Mid, 2 = Fast, 3 = Max
        public bool bChopped { get; set; }          // iParameter[4] Chopped	0 = No Chop, 1 = Chopped (low offset)
        public int uPeroid { get; set; }           // iParameter[5] Period	    0 = SYNC Mode.  1+ = ASYNC with Period in mSec resolution up to 276 Minute. 
        public bool bIsEnable { get; set; }         // iParameter[6] isEnable	0 = AD7794 disabled, 1 = Enabled

        //#####################################################################################################
        //###################################################################################### ADC24 Constructor
        //#####################################################################################################
        public TVMiniPort_AD7794Setup()     // Default setup
        {
            bCapType = true;
            bRTDMode = false;
            bInternalTemp = false;
            uSpeed = 2;
            bChopped = false;
            uPeroid = 0;        // 0 = SYNC >1 ASYNC
            bIsEnable = true;
        }
        //---------------------------------------------------------------------Chanel timing parameter and filters
        //                                                                          Slow,               Mid,                Fast,           Max
        private List<string> Taqutime_UpdateRateSetting = new List<string>(new string[] { "0b1111", "0x1010", "0b0011", "0b0001" });
        private List<string> Taqutime_Filter = new List<string>(new string[] { "-74dB (50/60Hz)", "-65dB (50/60Hz)", "None", "None" });
        private List<int> Taqutime_SingleReadNoChopped = new List<int>(new int[] { 2450, 630, 98, 37 });           //mSec/10
        private List<int> Taqutime_SingleReadChopped = new List<int>(new int[] { 4900, 1260, 196, 74 });           //mSec/10
        private List<int> Taqutime_ContinousCapNoChopped = new List<int>(new int[] { 2450, 630, 98, 37 });           //mSec/10
        private List<int> Taqutime_ContinousCapChopped = new List<int>(new int[] { 4900, 1260, 196, 74 });           //mSec/10
                                                                                                                    //---------------------------------------------------------------------ReadoutFormat
        private List<string> ReadoutFormat = new List<string>(new string[] {
        "0=Default via Hardware Mode setting",
        "1=Unipolar with 2V5REF, Unipolar Config (0 to 2V5)",
        "2=Unipolar with 2V5REF, Bipolar  Config (0 to 5V0)",
        "3=Bipolar  with 2V5REF, Bipolar  Config (-/+ 2V5)",
        "4=Bipolar  with 2V5REF, Bipolar  Config (-/+ 2V5) with Gain",
        "5=RAWData in 16 bits (unsigned int)",
        "6=RAWData in 32 bits (unsigned long)",
        "7=Double (WIP, do not use)",
        "8=Bipolar  with 2V5REF & Offset/Gain (+/-2.5V)",     //Based on (2)
        "9=Unipolar with 2V5REF & Offset/Gain (0 to 5V0)"     //Based on (3) or (4).
        });
        //---------------------------------------------------------------------GainSetting
        private List<string> GainSetting = new List<string>(new string[] { "1V/V", "2V/V", "4V/V", "8V/V", "16V/V", "32V/V", "64V/V", "128V/V" });
        //---------------------------------------------------------------------HardwareMode
        private List<string> HardwareMode = new List<string>(new string[] {
        "0=Channel=>Disabled",
        "1=SE,   Unipolar, +AIN=VINP, -AIN=0V,     (Readout= 0V to 2V5)",
        "2=SE,   Bipolar,  +AIN=VINP, -AIN=2V5REF, (Readout= 0V to 5V0)",
        "3=SE,   Bipolar,  +AIN=VINP, -AIN=2V5REF, (Readout= -2V5 to +2V5)",
        "4=Diff, Bipolar,  AIN(DIFF),   (Readout= -2V5 to +2V5), Gain=1V/V",
        "5=Diff, Bipolar,  AIN(DIFF),   (Readout= -2V5 to +2V5), Gain=Any"
        //"6 = Reserved",
        //"7 = Reserved"
    });

        public List<string> lHardwareMode()
        {
            return (HardwareMode);
        }
        public List<string> lGainSetting()
        {
            return (GainSetting);
        }
        public List<string> lReadoutFormat()
        {
            return (ReadoutFormat);
        }
        public List<string> lTaqutime_Filter()
        {
            return (Taqutime_Filter);
        }
        public List<string> lTaqutime_UpdateRateSetting()
        {
            return (Taqutime_UpdateRateSetting);
        }
        public int iTaqutime_ContinousCapChopped(int index)
        {
            return (Taqutime_ContinousCapChopped[index]);
        }
        public int iTaqutime_ContinousCapNoChopped(int index)
        {
            return (Taqutime_ContinousCapNoChopped[index]);
        }
        public int iTaqutime_SingleReadChopped(int index)
        {
            return (Taqutime_SingleReadChopped[index]);
        }
        public int iTaqutime_SingleReadNoChopped(int index)
        {
            return (Taqutime_SingleReadNoChopped[index]);
        }



    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++TVMiniPort_AD7794Channel
    [Serializable]
    public class TVMiniPort_AD7794Channel
    {
        //#####################################################################################################
        //###################################################################################### ADC24 Getter/Setter, public
        //#####################################################################################################
        public int uChannel { get; set; }          //iParameter[0]	CHANNEL		1 to 6	Select channel as part of setup, (Exclude CH7 for IntTemp)
        public int iHardwareMode { get; set; }     //iParameter[1]	H/W_MODE	0 to 7	Hardware configuration, see document
        public bool bBufferMode { get; set; }       //iParameter[2]	BUFF		0 or 1	0 = No Buff, 1 = Bufferd (AD7794)
        public int uGain { get; set; }             //iParameter[3]	GAIN		0 to 7	Depend on H/W_MODE, set AD7794 Gain
        public int uReadoutFormat { get; set; }    //iParameter[4]	FORMAT		0 to F  Readout Format

        //#####################################################################################################
        //###################################################################################### ADC24 Constructor
        //#####################################################################################################
        public TVMiniPort_AD7794Channel(int i)
        {
            uChannel = i;
            iHardwareMode = 2;
            bBufferMode = false;
            uGain = 0;                  //1V/V
            uReadoutFormat = 0;         // Firmware Default     
        }
        public TVMiniPort_AD7794Channel()
        {
        }
    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++TVMiniPort_AD7794TempCh
    [Serializable]
    public class TVMiniPort_AD7794TempCh
    {
        //#####################################################################################################
        //###################################################################################### ADC24 Getter/Setter, public
        //#####################################################################################################
        public int uChannel { get; set; }          //iParameter[0]	CHANNEL		1 to 6	Select channel as part of setup, (Exclude CH7 for IntTemp)
        public int uReadoutFormat { get; set; }    //iParameter[4]	FORMAT		13 = K, 14 = C 15 = F

        //bInternalTemp via ADC24Setup. 

        //#####################################################################################################
        //###################################################################################### ADC24 Constructor
        //#####################################################################################################
        public TVMiniPort_AD7794TempCh()
        {
            uChannel = 7;              // Fixed Channe for reference.
            uReadoutFormat = 13;       // Kelvin Mode.  
                                        //13 = Temp in  ͦK   (Default)
                                        //14 = Temp in  ͦC
                                        //15 = Temp in  ͦF
        }
    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++TVMiniPort_FCL30Setup
    [Serializable]
    public class TVMiniPort_FCL30Setup
    {
        //#####################################################################################################
        //###################################################################################### Getter/Setter, public
        //#####################################################################################################
        public int iFormatReadout { get; set; }          // iParameter[1]      0 = Raw Data, 1 = nT, 2 = pT, 3 =mG, 4=uG
        public int iAxisPolicy { get; set; }            // iParameter[2] 	   0 = OGT, 1 = XYZ, 2 = MRC (same logic as ADIS16460)
        public bool bIsEnable { get; set; }             // iParameter[3]       0 = FCY30 is disabled, 1 = Enabled 

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public TVMiniPort_FCL30Setup()
        {
            iFormatReadout = 2;        // pT
            iAxisPolicy = 1;           // XYZ 
        }
    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++TVMiniPort_ADCPost
        [Serializable]
        public class TVMiniPort_ADCPost
        {
            //#####################################################################################################
            //###################################################################################### Getter/Setter
            //#####################################################################################################
            public int iCh { get; set; }       // Channel Reference 0 to 5 reserved for AD7794. 10 to 12 reserved for ADC12.  
            public string iType { get; set; }       // Name of channel, ie ADC24-1, ADC12-1, etc.
            public UInt32 uGain { get; set; }       // 1,000,000 = 1V/V, 100,000 = 0.1V/V, 10,000,000 = 10V/V Max to  4,294,967,296 (4295V/V). 0 = Skipped Gain calculation.
            public int iOffset { get; set; }       // 0 = no offset    -10 = 10uV, +10 = +10uV offset. Must be in uV expression. For Raw data, treat as Raw Data.
            public string sUnitText { get; set; }       // post text to express data value, ie mV, uT, mG, mW, NB: Limited to 8 Char.      

            //#####################################################################################################
            //###################################################################################### Constructor
            //#####################################################################################################
            public TVMiniPort_ADCPost(int Channel, string ChannelType)
            {
                iCh = Channel;
                iType = ChannelType;
                uGain = 0;                  // Default No Gain calculation (skipped gain code). 
                iOffset = 0;                // Default No Offset 
                sUnitText = "uV";           // Default text
            }
            public TVMiniPort_ADCPost()
            {
            }

            public bool isADC24(int channel)
                {
                    if ((channel >= 0) & (channel <= 5))
                        return true;
                    return false;
                }
                public bool isADC12(int channel)
                {
                    if ((channel == 10) | (channel == 11) | (channel == 12))
                        return true;
                    return false;
                }
            }
    #endregion


    //#####################################################################################################
    //###################################################################################### Special Classes
    //#####################################################################################################

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++Struct SupportDataGrid
    struct SupportDataGrid
    {
        private readonly string label;
        private readonly int width;

        public SupportDataGrid(string label, int width)
        {
            this.label = label;
            this.width = width;
        }

        public string Label { get { return label; } }
        public int Width { get { return width; } }

    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++Mag_Data_Math
    [Serializable]
    public class Mag_DataMath
    {
        public Vector3 dMagRaw;         // double raw
        public iVector3 iMagRaw;       // integer raw
        public Vector3 dMagCal;         // Calibrated data. 
        public fVector3 fMagCal;       // Calibrated data in float format for charting. 

        //======================================================Getter/Setter f===
        public int DataFormat { get; set; }             // 0= Raw, 1 = uT, 2 = nT, 3 = mG, 4 = uG
        public double dDeclination { get; set; }        // Radian
        public double dInclination { get; set; }        // Radian
        public double dHorizontal { get; set; }         // uT or mG
        public double dNorthX { get; set; }             // uT or mG
        public double dEastY { get; set; }              // uT or mG
        public double dIntensity { get; set; }          // uT or mG

        // =====================================================constructor
        public Mag_DataMath()
        {
            dMagRaw = new Vector3();
            iMagRaw = new iVector3();
            dMagCal = new Vector3();
            fMagCal = new fVector3();
        }

        public bool ProcessMath(Vector3 dMagIN)
        {
            try
            {
                dDeclination = Math.Atan(dMagIN.Y / dMagIN.X);
                dHorizontal = Math.Sqrt((Math.Pow(dMagIN.X, 2) + Math.Pow(dMagIN.Y, 2)));
                dIntensity = Math.Sqrt((Math.Pow(dMagIN.X, 2) + Math.Pow(dMagIN.Y, 2) + Math.Pow(dMagIN.Z, 2)));
                dInclination = Math.Atan(dMagIN.Z / dHorizontal);
                dNorthX = dHorizontal * Math.Cos(dDeclination);
                dEastY = dHorizontal * Math.Sin(dDeclination);
            }
            catch
            {
                return (false);     // Failed maths process.
            }
            return (true);          // Success maths process.
        }
        //------------------------------------Convert interger to double
        public Vector3 ConvertINT32toDouble(double dScale)
        {
            Vector3 dMagOUT = new Vector3();
            dMagOUT.X = (Convert.ToDouble(iMagRaw.X)) / dScale;
            dMagOUT.Y = (Convert.ToDouble(iMagRaw.Y)) / dScale;
            dMagOUT.Z = (Convert.ToDouble(iMagRaw.Z)) / dScale;
            return dMagOUT;
        }
        //------------------------------------Convert interger to double
        public Vector3 ProcessCalibration(Vector3 dMagIN)
        {
            Vector3 dMagOUT = new Vector3();
            dMagOUT.X=dMagRaw.X;
            dMagOUT.Y=dMagRaw.Y;
            dMagOUT.Z=dMagRaw.Z;
            return dMagOUT;
        }
        //------------------------------------Normal FCL30-7 Raw Data axis to Honeywell XYZ for math. Tested for FCL30-7 device.
        public Vector3 NormaliseAxisForMath(Vector3 dMagIN )
        {
            Vector3 dMagOUT = new Vector3();
            dMagOUT.X = dMagIN.Z;
            dMagOUT.Y = -dMagIN.Y;
            dMagOUT.Z = dMagIN.X;

            //myFLC30MagMath.iMagRaw.X = ZZ;
            //myFLC30MagMath.iMagRaw.Y = -YY;
            //myFLC30MagMath.iMagRaw.Z = XX;

            return dMagOUT;
        }
    }
    #endregion

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++MiniAD7794SuperClass
    [Serializable]
    public class MiniAD7794SuperClass
    {
        //---------------------------------------------------------------------------------
        public TVMiniPort_TabEnable scTabEnable;
        public TVMiniPort_AD7794TempCh scADC24TempCh;
        public TVMiniPort_AD7794Setup scADC24Setup;
        public List<TVMiniPort_ADCPost> scADCPostArray;
        public List<TVMiniPort_AD7794Channel> scADC24Ch;
        public TVMiniPort_FCL30Setup scFCL30Setup;
        public TVMiniPort_ADIS16460Setup scADIS16460Setup;
        public TVMiniPort_LoggerCVS scLoggerCVSSetup;
        //---------------------------------------------------------------------------------
        public MiniAD7794SuperClass()
        {
        }
        //---------------------------------------------------------------------------------
        public void CreateNewObjects()
        {
            scADC24TempCh = new TVMiniPort_AD7794TempCh();
            scADC24Setup = new TVMiniPort_AD7794Setup();
            scTabEnable = new TVMiniPort_TabEnable();
            //---------------------------------------------Offset/Gain Section for ADC12 and ADC24. 
            scADCPostArray = new List<TVMiniPort_ADCPost>(9)
            {
                new TVMiniPort_ADCPost(0,"ADC24-CH1"),
                new TVMiniPort_ADCPost(1,"ADC24-CH2"),
                new TVMiniPort_ADCPost(2,"ADC24-CH3"),
                new TVMiniPort_ADCPost(3,"ADC24-CH4"),
                new TVMiniPort_ADCPost(4,"ADC24-CH5"),
                new TVMiniPort_ADCPost(5,"ADC24-CH6"),
                new TVMiniPort_ADCPost(10,"ADC12-CH1"),
                new TVMiniPort_ADCPost(11,"ADC12-CH2"),
                new TVMiniPort_ADCPost(12,"ADC12-CH3")
            };
            scADC24Ch = new List<TVMiniPort_AD7794Channel>(6)            //6 channels only
            {
                new TVMiniPort_AD7794Channel(0),
                new TVMiniPort_AD7794Channel(1),
                new TVMiniPort_AD7794Channel(2),
                new TVMiniPort_AD7794Channel(3),
                new TVMiniPort_AD7794Channel(4),
                new TVMiniPort_AD7794Channel(5)
            };
            scFCL30Setup = new TVMiniPort_FCL30Setup();
            scADIS16460Setup = new TVMiniPort_ADIS16460Setup();
            scLoggerCVSSetup = new TVMiniPort_LoggerCVS();
        }
        //---------------------------------------------------------------------------------
        public void CopyObjects(MiniAD7794SuperClass oObject)
        {
            this.scADC24TempCh = oObject.scADC24TempCh;
            this.scADC24Setup = oObject.scADC24Setup;
            this.scADIS16460Setup = oObject.scADIS16460Setup;     //This does not work!
            this.scFCL30Setup = oObject.scFCL30Setup;
            this.scTabEnable = oObject.scTabEnable;
            this.scLoggerCVSSetup = oObject.scLoggerCVSSetup;
            //-----------------------------------------------------------
            for (int i = 0; i < 9; i++)
                this.scADCPostArray[i] = oObject.scADCPostArray[i];
            for (int i = 0; i < 6; i++)
                this.scADC24Ch[i] = oObject.scADC24Ch[i];
        }
        //---------------------------------------------------------------------------------
        public void TabWindowUpdateReadData(TVMiniPort_TabEnable RefIN)
        {
            RefIN.bLoggerCVS = scTabEnable.bLoggerCVS;
            RefIN.bLogMemory = scTabEnable.bLogMemory;
            RefIN.bADCSupport = scTabEnable.bADCSupport;
            RefIN.bAD7794 = scTabEnable.bAD7794;
            RefIN.bADC12 = scTabEnable.bADC12;
            RefIN.bTabMiniPort = scTabEnable.bTabMiniPort;
            RefIN.bSerialI2C = scTabEnable.bSerialI2C;
            RefIN.bSerialSPI = scTabEnable.bSerialSPI;
            RefIN.bSerialUART = scTabEnable.bSerialUART;
            RefIN.bSerialIOComm = scTabEnable.bSerialIOComm;
            RefIN.bADIS16460 = scTabEnable.bADIS16460;
            RefIN.bFLC370 = scTabEnable.bFLC370;
        }
        public void TabWindowUpdateWriteData(TVMiniPort_TabEnable RefIN)
        {
            scTabEnable.bLoggerCVS = RefIN.bLoggerCVS;
            scTabEnable.bLogMemory = RefIN.bLogMemory;
            scTabEnable.bADCSupport = RefIN.bADCSupport;
            scTabEnable.bAD7794 = RefIN.bAD7794;
            scTabEnable.bADC12 = RefIN.bADC12;
            scTabEnable.bTabMiniPort = RefIN.bTabMiniPort;
            scTabEnable.bSerialI2C = RefIN.bSerialI2C;
            scTabEnable.bSerialSPI = RefIN.bSerialSPI;
            scTabEnable.bSerialUART = RefIN.bSerialUART;
            scTabEnable.bSerialIOComm = RefIN.bSerialIOComm;
            scTabEnable.bADIS16460 = RefIN.bADIS16460;
            scTabEnable.bFLC370 = RefIN.bFLC370;
        }
        //---------------------------------------------------------------------------------
        public void ADIS16460UpdateReadData(TVMiniPort_ADIS16460Setup RefIN)
        {
            RefIN.bAccelMath = scADIS16460Setup.bAccelMath;
            RefIN.bAdvFrame = scADIS16460Setup.bAdvFrame;
            RefIN.bRunStop = scADIS16460Setup.bRunStop;
            RefIN.iAccelFormat = scADIS16460Setup.iAccelFormat;
            RefIN.iAxisPolicy = scADIS16460Setup.iAxisPolicy;
            RefIN.iGyroFormat = scADIS16460Setup.iGyroFormat;
            //RefIN.isADIS16460TabEnable = scADIS16460Setup.isADIS16460TabEnable;
        }
        public void ADIS16460UpdateWriteData(TVMiniPort_ADIS16460Setup RefIN)
        {
            scADIS16460Setup.bAccelMath = RefIN.bAccelMath;
            scADIS16460Setup.bAdvFrame = RefIN.bAdvFrame;
            scADIS16460Setup.bRunStop = RefIN.bRunStop;
            scADIS16460Setup.iAccelFormat = RefIN.iAccelFormat;
            scADIS16460Setup.iAxisPolicy = RefIN.iAxisPolicy;
            scADIS16460Setup.iGyroFormat = RefIN.iGyroFormat;
            //scADIS16460Setup.isADIS16460TabEnable = RefIN.isADIS16460TabEnable;
        }
        //---------------------------------------------------------------------------------
        public void FCL307UpdateWriteData(TVMiniPort_FCL30Setup RefIN)
        {
            RefIN.bIsEnable = scFCL30Setup.bIsEnable;
            RefIN.iAxisPolicy = scFCL30Setup.iAxisPolicy;
            RefIN.iFormatReadout = scFCL30Setup.iFormatReadout;
        }
        public void FCL307UpdateReadData(TVMiniPort_FCL30Setup RefIN)
        {
            scFCL30Setup.bIsEnable = RefIN.bIsEnable;
            scFCL30Setup.iAxisPolicy = RefIN.iAxisPolicy;
            scFCL30Setup.iFormatReadout = RefIN.iFormatReadout;
        }
        //---------------------------------------------------------------------------------
        public void LoggerCVSUpdateReadData(TVMiniPort_LoggerCVS RefIN)
        {
            RefIN.iSelectTrigger = scLoggerCVSSetup.iSelectTrigger;
            RefIN.iTimerPeroid = scLoggerCVSSetup.iTimerPeroid;
            RefIN.iCounterToCease = scLoggerCVSSetup.iCounterToCease;
            RefIN.isSleepEnabled = scLoggerCVSSetup.isSleepEnabled;
            RefIN.bAsyncUDTEnabled = scLoggerCVSSetup.bAsyncUDTEnabled;
            RefIN.bAsyncLogEnabled = scLoggerCVSSetup.bAsyncLogEnabled;
            RefIN.bIsEnable = scLoggerCVSSetup.bIsEnable;
            RefIN.sProjectName = scLoggerCVSSetup.sProjectName;
        }
        public void LoggerCVSUpdateWriteData(TVMiniPort_LoggerCVS RefIN)
        {
            scLoggerCVSSetup.iSelectTrigger = RefIN.iSelectTrigger;
            scLoggerCVSSetup.iTimerPeroid = RefIN.iTimerPeroid;
            scLoggerCVSSetup.iCounterToCease = RefIN.iCounterToCease;
            scLoggerCVSSetup.isSleepEnabled = RefIN.isSleepEnabled;
            scLoggerCVSSetup.bAsyncUDTEnabled = RefIN.bAsyncUDTEnabled;
            scLoggerCVSSetup.bAsyncLogEnabled = RefIN.bAsyncLogEnabled;
            scLoggerCVSSetup.bIsEnable = RefIN.bIsEnable;
            scLoggerCVSSetup.sProjectName = RefIN.sProjectName;
        }
        //---------------------------------------------------------------------------------
        public void TabEnableRefIn(TVMiniPort_TabEnable RefIN)
        {
            this.scTabEnable = RefIN;
        }
        //---------------------------------------------------------------------------------
        public void AD7794TempChURefIn(TVMiniPort_AD7794TempCh RefIN)
        {
            this.scADC24TempCh = RefIN;
        }
        public void AD7794SetupRefIn(TVMiniPort_AD7794Setup RefIN)
        {
            this.scADC24Setup = RefIN;
        }
        public void AD7794ChannelRefIn(List<TVMiniPort_AD7794Channel> RefIN)
        {
            this.scADC24Ch = RefIN;
        }
        public void ADCPostArrayRefIn(List<TVMiniPort_ADCPost> RefIN)
        {
            this.scADCPostArray = RefIN;
        }
        public void FCL30SetupRef(TVMiniPort_FCL30Setup RefIN)
        {
            this.scFCL30Setup = RefIN;
        }
        public void ADIS16460SetupRef(TVMiniPort_ADIS16460Setup RefIN)
        {
            this.scADIS16460Setup = RefIN;
        }
        public void TabLoggerCVSSetupRef(TVMiniPort_LoggerCVS RefIN)
        {
            this.scLoggerCVSSetup = RefIN;
        }
    }
    #endregion



}
