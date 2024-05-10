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
using System.Text.RegularExpressions;
using System.Reflection;
using UDT_Term;
using System.IO;
using System.Xml.Serialization;

namespace UDT_Term_FFT
{
    public partial class ESMBlockSeqNVM : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        USB_VCOM_Comm myUSBVCOMComm;
        MainProg myMainProg;
        DMFProtocol myDMFProtocol;
        DialogSupport mbDialogMessageBox;
        ESM_NVM_BlockSeqConfigII myESMBlockSeqII;
        //--------------------------------------   Master Class which include setup, etc. Used by this window and passed reference to MainProg, etc.
        private const string m_sESMFilenameDefaultTopProject = "ESMProject";
        //--------------------------------------

        #region//================================================================Reference
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
            //--------------------------------------Themes
            this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_Blank_1B;
            this.Refresh();
        }

        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        
        public void MyUSBVCOMComm(USB_VCOM_Comm myUSBVCOMCommRef)
        {
            myUSBVCOMComm = myUSBVCOMCommRef;
        }
        public void MyDMFProtocol(DMFProtocol myDMFProtocolRef)
        {
            myDMFProtocol = myDMFProtocolRef;
        }
        #endregion

        public ESMBlockSeqNVM()            
        {
            InitializeComponent();
            mbDialogMessageBox = new DialogSupport();
            myESMBlockSeqII = new ESM_NVM_BlockSeqConfigII();
            myESMBlockSeqII.InitDefault();
        }

        #region //================================================================ESMConfig_Setup_Load
        private void ESMConfig_Setup_Load(object sender, EventArgs e)
        {

        }
        #endregion

        #region //================================================================ESMConfig_Setup_FormClosing
        private void ESMConfig_Setup_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            myGlobalBase.ESMConfig_SetupOpen = false;                 // Cease Survey CVS Terminal mode.
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================ESMConfig_Show
        public void ESMConfig_Show()
        {
            this.BringToFront();
            this.TopMost = false;
            this.WindowState = FormWindowState.Normal;
//             string sFoldername = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
//             if (Environment.OSVersion.Version.Major >= 6)       //Window7/8/10
//             {
//                 sFoldername = Directory.GetParent(sFoldername).ToString();
//             }
            myGlobalBase.ESM_Config_FolderName = myGlobalBase.sSessionFoldername + "\\" + m_sESMFilenameDefaultTopProject;
            if (Directory.Exists(myGlobalBase.ESM_Config_FolderName)==false)
            {
                Directory.CreateDirectory(myGlobalBase.ESM_Config_FolderName);
            }
            txtProjectFilename.Text = myGlobalBase.ESM_Config_FolderName;
            myGlobalBase.ESMConfig_SetupOpen = true;
            this.Visible = true;
            this.Show();
            //------------------------------------------
            NVMWordToWindowNow();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### NVM word to Window Update. 
        //#####################################################################################################

        private void NVMWordToWindowNow()
        {
            string sEven;
            string sOdd;
            //========================================ESMConfigQ Section
            ESMConfigQWord.Text = "0x" + myESMBlockSeqII.myESMConfigQ.ESMConfigQ_Word.ToString("X8");
            tbConfigStartCH1.Text = myESMBlockSeqII.myESMConfigQ.StartBlockCH1.ToString();
            tbConfigStopCh1.Text = myESMBlockSeqII.myESMConfigQ.StopBlockCH1.ToString();
            tbConfigStartCh2.Text = myESMBlockSeqII.myESMConfigQ.StartBlockCH2.ToString();
            tbConfigStopCh2.Text = myESMBlockSeqII.myESMConfigQ.StartBlockCH2.ToString();
            tbConfigRTCPeriod.Text = myESMBlockSeqII.myESMConfigQ.QualRTCSetting.ToString();

            //========================================ESMMasterBlock0 Section
            ESMMasterBlock0Word.Text = "0x" + myESMBlockSeqII.myESMMasterBlock[0].ESMConfigMasterBlock_Word.ToString("X8");
            B0StartSeqNo.Text = myESMBlockSeqII.myESMMasterBlock[0].NVMAddressStart.ToString();
            B0NoSeqNo.Text = myESMBlockSeqII.myESMMasterBlock[0].MaxSeqNoCounter.ToString();
            cbBlock0Begin.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[0].BeginPolicy);
            cbBlock0Middle.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[0].MiddlePolicy);
            cbBlock0Finish.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[0].FinishPolicy);
            Block0Sensor.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[0].SensorMode);

            //========================================ESMMasterBlock1 Section
            ESMMasterBlock1Word.Text = "0x" + myESMBlockSeqII.myESMMasterBlock[1].ESMConfigMasterBlock_Word.ToString("X8");
            B1StartSeqNo.Text = myESMBlockSeqII.myESMMasterBlock[1].NVMAddressStart.ToString();
            B1NoSeqNo.Text = myESMBlockSeqII.myESMMasterBlock[1].MaxSeqNoCounter.ToString();
            cbBlock1Begin.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[1].BeginPolicy);
            cbBlock1Middle.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[1].MiddlePolicy);
            cbBlock1Finish.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[1].FinishPolicy);
            Block1Sensor.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[1].SensorMode);

            //========================================ESMMasterBlock2 Section
            ESMMasterBlock2Word.Text = "0x" + myESMBlockSeqII.myESMMasterBlock[2].ESMConfigMasterBlock_Word.ToString("X8");
            B2StartSeqNo.Text = myESMBlockSeqII.myESMMasterBlock[2].NVMAddressStart.ToString();
            B2NoSeqNo.Text = myESMBlockSeqII.myESMMasterBlock[2].MaxSeqNoCounter.ToString();
            cbBlock2Begin.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[2].BeginPolicy);
            cbBlock2Middle.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[2].MiddlePolicy);
            cbBlock2Finish.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[2].FinishPolicy);
            Block2Sensor.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[2].SensorMode);

            //========================================ESMMasterBlock3 Section
            ESMMasterBlock3Word.Text = "0x" + myESMBlockSeqII.myESMMasterBlock[3].ESMConfigMasterBlock_Word.ToString("X8");
            B3StartSeqNo.Text = myESMBlockSeqII.myESMMasterBlock[3].NVMAddressStart.ToString();
            B3NoSeqNo.Text = myESMBlockSeqII.myESMMasterBlock[3].MaxSeqNoCounter.ToString();
            cbBlock3Begin.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[3].BeginPolicy);
            cbBlock3Middle.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[3].MiddlePolicy);
            cbBlock3Finish.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[3].FinishPolicy);
            Block3Sensor.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMMasterBlock[3].SensorMode);

            //========================================ESMMasterSeqNo01 Section
            sEven = myESMBlockSeqII.myESMSeNo[0].ESMConfigSeqNo16_Word.ToString("X4");
            sOdd = myESMBlockSeqII.myESMSeNo[1].ESMConfigSeqNo16_Word.ToString("X4");
            tbSeq01Word.Text = "0x" + sOdd + sEven;
            cbSeqNo0_State.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMSeNo[0].ActivityMode);
            txSeqNo0_Period.Text = myESMBlockSeqII.myESMSeNo[0].Interval.ToString();
            cbSeqNo1_State.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMSeNo[1].ActivityMode);
            txSeqNo1_Period.Text = myESMBlockSeqII.myESMSeNo[1].Interval.ToString();

            //========================================ESMMasterSeqNo23 Section
            sEven = myESMBlockSeqII.myESMSeNo[2].ESMConfigSeqNo16_Word.ToString("X4");
            sOdd = myESMBlockSeqII.myESMSeNo[3].ESMConfigSeqNo16_Word.ToString("X4");
            tbSeq23Word.Text = "0x" + sOdd + sEven;
            cbSeqNo2_State.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMSeNo[2].ActivityMode);
            txSeqNo2_Period.Text = myESMBlockSeqII.myESMSeNo[2].Interval.ToString();
            cbSeqNo3_State.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMSeNo[3].ActivityMode);
            txSeqNo3_Period.Text = myESMBlockSeqII.myESMSeNo[3].Interval.ToString();

            //========================================ESMMasterSeqNo45 Section
            sEven = myESMBlockSeqII.myESMSeNo[4].ESMConfigSeqNo16_Word.ToString("X4");
            sOdd = myESMBlockSeqII.myESMSeNo[5].ESMConfigSeqNo16_Word.ToString("X4");
            tbSe45Word.Text = "0x" + sOdd + sEven;
            cbSeqNo4_State.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMSeNo[4].ActivityMode);
            txSeqNo4_Period.Text = myESMBlockSeqII.myESMSeNo[4].Interval.ToString();
            cbSeqNo5_State.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMSeNo[5].ActivityMode);
            txSeqNo5_Period.Text = myESMBlockSeqII.myESMSeNo[5].Interval.ToString();

            //========================================ESMMasterSeqNo67 Section
            sEven = myESMBlockSeqII.myESMSeNo[6].ESMConfigSeqNo16_Word.ToString("X4");
            sOdd = myESMBlockSeqII.myESMSeNo[7].ESMConfigSeqNo16_Word.ToString("X4");
            tbSeq67Word.Text = "0x" + sOdd + sEven;
            cbSeqNo6_State.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMSeNo[6].ActivityMode);
            txSeqNo6_Period.Text = myESMBlockSeqII.myESMSeNo[6].Interval.ToString();
            cbSeqNo7_State.SelectedIndex = Convert.ToInt32(myESMBlockSeqII.myESMSeNo[7].ActivityMode);
            txSeqNo7_Period.Text = myESMBlockSeqII.myESMSeNo[7].Interval.ToString();
            myMainProg.myRtbTermMessageLF("-I: NVM Block/Seq Window Updated based on myESMBlockSeqII Class");
        }

        //#####################################################################################################
        //###################################################################################### Save/Load
        //#####################################################################################################

        #region //================================================================btnLoad_Click
        private void btnLoad_Click(object sender, EventArgs e)
        {
            string sFoldername = myGlobalBase.ESM_Config_FolderName;
            string sFilename= myGlobalBase.ESM_Config_FilenameName;
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                OpenFileDialog ofd = openFileDialog;
                ofd.Title = "Setup Files";
                ofd.Filter = "xml files|*.xml";
                ofd.InitialDirectory = sFoldername;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    sFilename = ofd.FileName;
                    sFoldername = System.IO.Path.GetDirectoryName(ofd.FileName);
                    myGlobalBase.ESM_Config_FilenameName = sFilename;
                }
                else
                {
                    myMainProg.myRtbTermMessageLF("#E: User Cancelled 'Load' Operation");
                    return;
                }
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("#E: Filename Selection Report an Error");
                return;
            }
            txtProjectFilename.Text = sFoldername;
            try
            {
                myESMBlockSeqII = null;
                myESMBlockSeqII = new ESM_NVM_BlockSeqConfigII();
                myESMBlockSeqII = myGlobalBase.DeserializeFromFile<ESM_NVM_BlockSeqConfigII>(sFilename); 
                if (myESMBlockSeqII==null)
                {
                    string eMessage = "ERROR: Unable to de-serializes the selected filename";
                    mbDialogMessageBox.PopUpMessageBox(eMessage, "File System Error", 5, 12F);
                    myMainProg.myRtbTermMessageLF("#E:"+ eMessage);
                    return;
                }
            }
            catch
            {
                mbDialogMessageBox.PopUpMessageBox("ERROR: Unable to load Setup within designated folder, may not exist?", "File System Error", 5,12F);
                return;
            }
            mbDialogMessageBox.PopUpMessageBox("Successfully Loaded Setup File and updated Setup", "File System", 2,14F);
            NVMWordToWindowNow();
            this.Invalidate();
            this.Refresh();
            
        }
        #endregion

        #region //================================================================btnSave_Click
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtProjectFilename.Text=="")
            {
                mbDialogMessageBox.PopUpMessageBox("ERROR: Folder Not Specified. Please do that first! ", "File System Error", 5, 12F);
                return;
            }
            string pathString = txtProjectFilename.Text;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Xml|*.xml";
            saveFileDialog1.Title = "Save ESM Config File";
            saveFileDialog1.InitialDirectory = pathString;
            myGlobalBase.ESM_Config_FolderName = pathString;
            pathString = System.IO.Path.Combine(pathString, "ESMConfigBlockSeq_" + Tools.uDateTimeToUnixTimestampNowUTC().ToString() + ".xml");
            saveFileDialog1.FileName = pathString;

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                myMainProg.myRtbTermMessageLF("#E: User Cancelled 'Save' Operation");
                return;
            }

            if (saveFileDialog1.FileName == "")
                return;
            try
            {
                myGlobalBase.SerializeToFile<ESM_NVM_BlockSeqConfigII>(myESMBlockSeqII, saveFileDialog1.FileName);
                myGlobalBase.ESM_Config_FilenameName = saveFileDialog1.FileName;
            }
            catch
            {
                mbDialogMessageBox.PopUpMessageBox("ERROR: Unable to save ESM NVM Config within designated folder, is this protected folder?", "File System Error", 5, 12F);
                return;
            }
            mbDialogMessageBox.PopUpMessageBox("Successfully saved ToCo Setup File:\n"+ saveFileDialog1.FileName, "Setup File System", 2, 12F);
        }
        #endregion

        #region //================================================================btnClose_Click
        private void btnClose_Click(object sender, EventArgs e)
        {
            btnSave.PerformClick();
            this.Close();
        }
        #endregion

        #region //================================================================btnFolder_Click
        private void btnFolder_Click(object sender, EventArgs e)
        {
            string sFoldername = myGlobalBase.ESM_Config_FolderName;
            try
            {
                if (!Directory.Exists(sFoldername))                             // Default folder name for given drive. 
                {
                    DirectoryInfo di = Directory.CreateDirectory(sFoldername);  // Create folder if not exist. 
                }
            }
            catch
            {
                // failure to create folder due to missing drive then use default user
                sFoldername = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    sFoldername = Directory.GetParent(sFoldername).ToString();
                }
            }
            myGlobalBase.ESM_Config_FolderName = sFoldername;
            try
            {
                //----------------------------------------------------------
                DialogResult result = fdbESMConfig.ShowDialog();
                if (result == DialogResult.OK)
                {
                    sFoldername = fdbESMConfig.SelectedPath;
                    if (!Directory.Exists(sFoldername))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(sFoldername);  // Create folder if not exist. 
                    }
                }
                txtProjectFilename.Text = sFoldername;
                myGlobalBase.ESM_Config_FolderName = sFoldername;
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("###ERR: Folder Window Reported an Error");
            }
        }
        #endregion

        #region //================================================================BtnOpenFolder_Click
        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string myPath = myGlobalBase.ESM_Config_FolderName;
                if (Directory.Exists(myPath))                             // Default folder name for given drive. 
                {
                    System.Diagnostics.Process prc = new System.Diagnostics.Process();
                    prc.StartInfo.FileName = myPath;
                    prc.Start();
                }
                else
                {
                    myMainProg.myRtbTermMessageLF("###ERR: Unable to Open/Create Folder");
                    mbDialogMessageBox.PopUpMessageBox("ERROR! Unable to Open/Create Folder", "File Error in ESM Setup", 10);
                }
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("###ERR: Unable to Open/Create Folder");
                mbDialogMessageBox.PopUpMessageBox("ERROR! Unable to Open/Create Folder", "File Error in ESM Setup", 10);
            }
        }
        #endregion
        
        //#####################################################################################################
        //###################################################################################### Window to Generate NVM Words Update.
        //#####################################################################################################
     
        #region //================================================================Update Button
        private void btnUpdate0_Click(object sender, EventArgs e)
        {
            //========================================ESMConfigSensor Section
            myESMBlockSeqII.myESMConfigQ.StartBlockCH1 = Tools.ConversionStringtoUInt32(tbConfigStartCH1.Text);
            myESMBlockSeqII.myESMConfigQ.StopBlockCH1 = Tools.ConversionStringtoUInt32(tbConfigStopCh1.Text);
            myESMBlockSeqII.myESMConfigQ.StartBlockCH2 = Tools.ConversionStringtoUInt32(tbConfigStartCh2.Text);
            myESMBlockSeqII.myESMConfigQ.StopBlockCH2 = Tools.ConversionStringtoUInt32(tbConfigStopCh2.Text);
            myESMBlockSeqII.myESMConfigQ.QualRTCSetting = Tools.ConversionStringtoUInt32(tbConfigRTCPeriod.Text);
            myESMBlockSeqII.myESMConfigQ.ConfigGenerateWord();
            ESMConfigQWord.Text = "0x" + myESMBlockSeqII.myESMConfigQ.ESMConfigQ_Word.ToString("X8");
            myMainProg.myRtbTermMessageLF("Updated, Sync to myESMBlockSeqII Class From Window");
        }

        private void btnUpdate1_Click(object sender, EventArgs e)
        {
            //========================================ESMMasterBlock0 Section
            myESMBlockSeqII.myESMMasterBlock[0].NVMAddressStart= Tools.ConversionStringtoUInt32(B0StartSeqNo.Text);
            myESMBlockSeqII.myESMMasterBlock[0].MaxSeqNoCounter= Tools.ConversionStringtoUInt32(B0NoSeqNo.Text);
            myESMBlockSeqII.myESMMasterBlock[0].BeginPolicy = Convert.ToUInt32(cbBlock0Begin.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[0].MiddlePolicy= Convert.ToUInt32(cbBlock0Middle.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[0].FinishPolicy= Convert.ToUInt32(cbBlock0Finish.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[0].SensorMode= Convert.ToUInt32(Block0Sensor.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[0].ConfigGenerateWord();
            ESMMasterBlock0Word.Text = "0x" + myESMBlockSeqII.myESMMasterBlock[0].ESMConfigMasterBlock_Word.ToString("X8");
            myMainProg.myRtbTermMessageLF("Updated, Sync to myESMBlockSeqII Class From Window: Block0:" + ESMMasterBlock0Word.Text);
        }

        private void btnUpdate2_Click(object sender, EventArgs e)
        {
            //========================================ESMMasterBlock1 Section
            myESMBlockSeqII.myESMMasterBlock[1].NVMAddressStart = Tools.ConversionStringtoUInt32(B1StartSeqNo.Text);
            myESMBlockSeqII.myESMMasterBlock[1].MaxSeqNoCounter = Tools.ConversionStringtoUInt32(B1NoSeqNo.Text);
            myESMBlockSeqII.myESMMasterBlock[1].BeginPolicy = Convert.ToUInt32(cbBlock1Begin.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[1].MiddlePolicy = Convert.ToUInt32(cbBlock1Middle.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[1].FinishPolicy = Convert.ToUInt32(cbBlock1Finish.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[1].SensorMode = Convert.ToUInt32(Block1Sensor.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[1].ConfigGenerateWord();
            ESMMasterBlock1Word.Text = "0x" + myESMBlockSeqII.myESMMasterBlock[1].ESMConfigMasterBlock_Word.ToString("X8");
            myMainProg.myRtbTermMessageLF("Updated, Sync to myESMBlockSeqII Class From Window: Block1:" + ESMMasterBlock1Word.Text);
        }

        private void btnUpdate3_Click(object sender, EventArgs e)
        {
            //========================================ESMMasterBlock2 Section
            myESMBlockSeqII.myESMMasterBlock[2].NVMAddressStart = Tools.ConversionStringtoUInt32(B2StartSeqNo.Text);
            myESMBlockSeqII.myESMMasterBlock[2].MaxSeqNoCounter = Tools.ConversionStringtoUInt32(B2NoSeqNo.Text);
            myESMBlockSeqII.myESMMasterBlock[2].BeginPolicy = Convert.ToUInt32(cbBlock2Begin.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[2].MiddlePolicy = Convert.ToUInt32(cbBlock2Middle.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[2].FinishPolicy = Convert.ToUInt32(cbBlock2Finish.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[2].SensorMode = Convert.ToUInt32(Block2Sensor.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[2].ConfigGenerateWord();
            ESMMasterBlock2Word.Text = "0x" + myESMBlockSeqII.myESMMasterBlock[2].ESMConfigMasterBlock_Word.ToString("X8");
            myMainProg.myRtbTermMessageLF("Updated, Sync to myESMBlockSeqII Class From Window: Block2:"+ ESMMasterBlock2Word.Text);
        }

        private void btnUpdate4_Click(object sender, EventArgs e)
        {
            //========================================ESMMasterBlock3 Section
            myESMBlockSeqII.myESMMasterBlock[3].NVMAddressStart = Tools.ConversionStringtoUInt32(B3StartSeqNo.Text);
            myESMBlockSeqII.myESMMasterBlock[3].MaxSeqNoCounter = Tools.ConversionStringtoUInt32(B3NoSeqNo.Text);
            myESMBlockSeqII.myESMMasterBlock[3].BeginPolicy = Convert.ToUInt32(cbBlock3Begin.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[3].MiddlePolicy = Convert.ToUInt32(cbBlock3Middle.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[3].FinishPolicy = Convert.ToUInt32(cbBlock3Finish.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[3].SensorMode = Convert.ToUInt32(Block3Sensor.SelectedIndex);
            myESMBlockSeqII.myESMMasterBlock[3].ConfigGenerateWord();
            ESMMasterBlock3Word.Text = "0x" + myESMBlockSeqII.myESMMasterBlock[3].ESMConfigMasterBlock_Word.ToString("X8");
            myMainProg.myRtbTermMessageLF("Updated, Sync to myESMBlockSeqII Class From Window: Block3:" + ESMMasterBlock3Word.Text);
        }

        private void btnUpdate5_Click(object sender, EventArgs e)
        {
            //========================================ESMMasterSeqNo01 Section
            myESMBlockSeqII.myESMSeNo[0].ActivityMode = Convert.ToUInt16(cbSeqNo0_State.SelectedIndex);
            myESMBlockSeqII.myESMSeNo[0].Interval = (UInt16) Tools.ConversionStringtoUInt32(txSeqNo0_Period.Text);
            myESMBlockSeqII.myESMSeNo[1].ActivityMode = Convert.ToUInt16(cbSeqNo1_State.SelectedIndex);
            myESMBlockSeqII.myESMSeNo[1].Interval = (UInt16)Tools.ConversionStringtoUInt32(txSeqNo1_Period.Text);
            myESMBlockSeqII.myESMSeNo[0].ConfigGenerateWord();
            myESMBlockSeqII.myESMSeNo[1].ConfigGenerateWord();
            string sEven = myESMBlockSeqII.myESMSeNo[0].ESMConfigSeqNo16_Word.ToString("X4");
            string sOdd = myESMBlockSeqII.myESMSeNo[1].ESMConfigSeqNo16_Word.ToString("X4");
            tbSeq01Word.Text = "0x" + sOdd + sEven;
            myMainProg.myRtbTermMessageLF("Updated, Sync to myESMBlockSeqII Class From Window: SeqNo01:"+ tbSeq01Word.Text);
        }

        private void btnUpdate6_Click(object sender, EventArgs e)
        {
            //========================================ESMMasterSeqNo01 Section
            myESMBlockSeqII.myESMSeNo[2].ActivityMode = Convert.ToUInt16(cbSeqNo2_State.SelectedIndex);
            myESMBlockSeqII.myESMSeNo[2].Interval = (UInt16)Tools.ConversionStringtoUInt32(txSeqNo2_Period.Text);
            myESMBlockSeqII.myESMSeNo[3].ActivityMode = Convert.ToUInt16(cbSeqNo3_State.SelectedIndex);
            myESMBlockSeqII.myESMSeNo[3].Interval = (UInt16)Tools.ConversionStringtoUInt32(txSeqNo3_Period.Text);
            myESMBlockSeqII.myESMSeNo[2].ConfigGenerateWord();
            myESMBlockSeqII.myESMSeNo[3].ConfigGenerateWord();
            string sEven = myESMBlockSeqII.myESMSeNo[2].ESMConfigSeqNo16_Word.ToString("X4");
            string sOdd = myESMBlockSeqII.myESMSeNo[3].ESMConfigSeqNo16_Word.ToString("X4");
            tbSeq23Word.Text = "0x" + sOdd + sEven;
            myMainProg.myRtbTermMessageLF("Updated, Sync to myESMBlockSeqII Class From Window: SeqNo23:" + tbSeq23Word.Text);
        }

        private void btnUpdate7_Click(object sender, EventArgs e)
        {
            //========================================ESMMasterSeqNo01 Section
            myESMBlockSeqII.myESMSeNo[4].ActivityMode = Convert.ToUInt16(cbSeqNo4_State.SelectedIndex);
            myESMBlockSeqII.myESMSeNo[4].Interval = (UInt16)Tools.ConversionStringtoUInt32(txSeqNo4_Period.Text);
            myESMBlockSeqII.myESMSeNo[5].ActivityMode = Convert.ToUInt16(cbSeqNo5_State.SelectedIndex);
            myESMBlockSeqII.myESMSeNo[5].Interval = (UInt16)Tools.ConversionStringtoUInt32(txSeqNo5_Period.Text);
            myESMBlockSeqII.myESMSeNo[4].ConfigGenerateWord();
            myESMBlockSeqII.myESMSeNo[5].ConfigGenerateWord();
            string sEven = myESMBlockSeqII.myESMSeNo[4].ESMConfigSeqNo16_Word.ToString("X4");
            string sOdd = myESMBlockSeqII.myESMSeNo[5].ESMConfigSeqNo16_Word.ToString("X4");
            tbSe45Word.Text = "0x" + sOdd + sEven;
            myMainProg.myRtbTermMessageLF("Updated, Sync to myESMBlockSeqII Class From Window: SeqNo45:" + tbSe45Word.Text);
        }

        private void btnUpdate8_Click(object sender, EventArgs e)
        {
            //========================================ESMMasterSeqNo01 Section
            myESMBlockSeqII.myESMSeNo[6].ActivityMode = Convert.ToUInt16(cbSeqNo6_State.SelectedIndex);
            myESMBlockSeqII.myESMSeNo[6].Interval = (UInt16)Tools.ConversionStringtoUInt32(txSeqNo6_Period.Text);
            myESMBlockSeqII.myESMSeNo[7].ActivityMode = Convert.ToUInt16(cbSeqNo7_State.SelectedIndex);
            myESMBlockSeqII.myESMSeNo[7].Interval = (UInt16)Tools.ConversionStringtoUInt32(txSeqNo7_Period.Text);
            myESMBlockSeqII.myESMSeNo[6].ConfigGenerateWord();
            myESMBlockSeqII.myESMSeNo[7].ConfigGenerateWord();
            string sEven = myESMBlockSeqII.myESMSeNo[6].ESMConfigSeqNo16_Word.ToString("X4");
            string sOdd = myESMBlockSeqII.myESMSeNo[7].ESMConfigSeqNo16_Word.ToString("X4");
            tbSeq67Word.Text = "0x" + sOdd + sEven;
            myMainProg.myRtbTermMessageLF("Updated, Sync to myESMBlockSeqII Class From Window: SeqNo67:" + tbSeq67Word.Text);
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### NVM Read/Write with ESM. 
        //#####################################################################################################

        #region //================================================================DMFP_GenericCallBack
        public void DMFP_GenericCallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            string ErrorMessage = "ERROR: Callback Error Message: " + myESMBlockSeqII.ErrorCode;
            if (myESMBlockSeqII.CMDCFG_GenericCallBack(hPara) != 0xFF)
            {
                mbDialogMessageBox.PopUpMessageBox(ErrorMessage, "ESM NVM Error", 5, 12F);
                myMainProg.myRtbTermMessageLF(ErrorMessage);
            }

        }
        #endregion

        #region //================================================================btnReadNVM_Click
        private void btnReadNVM_Click(object sender, EventArgs e)
        {
            DMFP_Delegate fpCallBack = new DMFP_Delegate(DMFP_Read41Callback);
            myDMFProtocol.DMFP_Process_Command(fpCallBack, myESMBlockSeqII.CMDCFG_Read_41(), false, -1);
        }
        public void DMFP_Read41Callback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Error = myESMBlockSeqII.CMDCFG_CallBack_41(hPara);

            if (Error == 0xFE)
            {
                string ErrorMessage = "ERROR: Internal Issue in CMDCFG_CallBack_41(): Missing hPara array, check ESM firmware";
                mbDialogMessageBox.PopUpMessageBox(ErrorMessage, "INTERNAL NVM Error", 5, 12F);
                myMainProg.myRtbTermMessageLF(ErrorMessage);
                return;
            }
            if (Error != 0xFF)
            {
                string ErrorMessage = "ERROR: Callback41 Error Message: " + myESMBlockSeqII.ErrorCode;
                mbDialogMessageBox.PopUpMessageBox(ErrorMessage, "ESM NVM Error", 5, 12F);
                myMainProg.myRtbTermMessageLF(ErrorMessage);
                return;
            }
            Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
            DMFP_Delegate fpCallBack = new DMFP_Delegate(DMFP_Read42Callback);
            myDMFProtocol.DMFP_Process_Command(fpCallBack, myESMBlockSeqII.CMDCFG_Read_42(), false, -1);
        }
        public void DMFP_Read42Callback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Error = myESMBlockSeqII.CMDCFG_CallBack_42(hPara);
            if (Error == 0xFE)
            {
                string ErrorMessage = "ERROR: Internal Issue in CMDCFG_CallBack_42(): Missing hPara array, check ESM firmware";
                mbDialogMessageBox.PopUpMessageBox(ErrorMessage, "INTERNAL NVM Error", 5, 12F);
                myMainProg.myRtbTermMessageLF(ErrorMessage);
                return;
            }

            if (Error != 0xFF)
            {
                string ErrorMessage = "ERROR: Callback42 Error Message: " + myESMBlockSeqII.ErrorCode;
                mbDialogMessageBox.PopUpMessageBox(ErrorMessage, "ESM NVM Error", 5, 12F);
                myMainProg.myRtbTermMessageLF(ErrorMessage);
                return;
            }
            Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
            DMFP_Delegate fpCallBack = new DMFP_Delegate(DMFP_Read43Callback);
            myDMFProtocol.DMFP_Process_Command(fpCallBack, myESMBlockSeqII.CMDCFG_Read_43(), false, -1);
        }
        public void DMFP_Read43Callback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Error = myESMBlockSeqII.CMDCFG_CallBack_43(hPara);
            if (Error == 0xF3)
            {
                string ErrorMessage = "ERROR: Internal Issue in CMDCFG_CallBack_43(): Missing hPara array, check ESM firmware";
                mbDialogMessageBox.PopUpMessageBox(ErrorMessage, "INTERNAL NVM Error", 5, 12F);
                myMainProg.myRtbTermMessageLF(ErrorMessage);
                return;
            }
            if (Error != 0xFF)
            {
                string ErrorMessage = "ERROR: Callback43 Error Message: " + myESMBlockSeqII.ErrorCode;
                mbDialogMessageBox.PopUpMessageBox(ErrorMessage, "ESM NVM Error", 5, 12F);
                myMainProg.myRtbTermMessageLF(ErrorMessage);
                return;
            }
            NVMWordToWindowNow();
        }

        #endregion

        #region //================================================================btnDefaultNVM_Click
        private void btnDefaultNVM_Click(object sender, EventArgs e)
        {
            if (JR.Utils.GUI.Forms.FlexiMessageBox.Show("This will overwrite existing NVM within ESM Tool, Have you Save old Config?",
                "ESM NVM Warning",
                MessageBoxButtons.OKCancel,
                 MessageBoxIcon.Warning) == DialogResult.OK)
            {
                DMFP_Delegate fpCallBack = new DMFP_Delegate(DMFP_GenericCallBack);
                myDMFProtocol.DMFP_Process_Command(fpCallBack, myESMBlockSeqII.CMDCFG_CMD_7F(), false, -1);
                myMainProg.myRtbTermMessageLF("Default Applied in ESM Tools. You need to do NVM Read to refresh Window");
            }
        }
        #endregion

        #region //================================================================btnUpdateNVM_Click
        private void btnUpdateNVM_Click(object sender, EventArgs e)
        {
            
            if (JR.Utils.GUI.Forms.FlexiMessageBox.Show("Have you apply <Update> or <UpdateAll> Button?, if not Cancel now. This will overwrite existing NVM within ESM Tool, are you sure? ",
                "ESM NVM Warning",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning) == DialogResult.OK)
            {
                DMFP_Delegate fpCallBack = new DMFP_Delegate(DMFP_Write51Callback);
                myDMFProtocol.DMFP_Process_Command(fpCallBack, myESMBlockSeqII.CMDCFG_Write51(), false, -1);
            }
            myMainProg.myRtbTermMessageLF("Default Applied in ESM Tools. You need to do NVM Read to refresh Window");
        }
        #endregion

        #region //================================================================DMFP_Write51Callback
        public void DMFP_Write51Callback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            string ErrorMessage = "ERROR: Callback Error Message: " + myESMBlockSeqII.ErrorCode;
            if (myESMBlockSeqII.CMDCFG_GenericCallBack(hPara) != 0xFF)
            {
                mbDialogMessageBox.PopUpMessageBox(ErrorMessage, "ESM NVM Error", 5, 12F);
                myMainProg.myRtbTermMessageLF(ErrorMessage);
                return;
            }
            Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
            DMFP_Delegate fpCallBack = new DMFP_Delegate(DMFP_Write52Callback);
            myDMFProtocol.DMFP_Process_Command(fpCallBack, myESMBlockSeqII.CMDCFG_Write52(), false, -1);
        }
        #endregion

        #region //================================================================DMFP_Write52Callback
        public void DMFP_Write52Callback(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            string ErrorMessage = "ERROR: Callback Error Message: " + myESMBlockSeqII.ErrorCode;
            if (myESMBlockSeqII.CMDCFG_GenericCallBack(hPara) != 0xFF)
            {
                mbDialogMessageBox.PopUpMessageBox(ErrorMessage, "ESM NVM Error", 5, 12F);
                myMainProg.myRtbTermMessageLF(ErrorMessage);
                return;
            }
            Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
            DMFP_Delegate fpCallBack = new DMFP_Delegate(DMFP_GenericCallBack);
            myDMFProtocol.DMFP_Process_Command(fpCallBack, myESMBlockSeqII.CMDCFG_Write52(), false, -1);
        }
        #endregion

        #region //================================================================Quick Folder
        private void cADTSessionLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in quickFolderToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            txtProjectFilename.Text = "C:\\ESMProject";
            myGlobalBase.ESM_Config_FolderName = txtProjectFilename.Text;
        }

        private void cESMProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in quickFolderToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            txtProjectFilename.Text = "D:\\ESMProject";
            myGlobalBase.ESM_Config_FolderName = txtProjectFilename.Text;
        }

        private void cESMProjectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in quickFolderToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            txtProjectFilename.Text = "E:\\ESMProject";
            myGlobalBase.ESM_Config_FolderName = txtProjectFilename.Text;
        }

        private void cESMProjectToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in quickFolderToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            txtProjectFilename.Text = "F:\\ESMProject";
            myGlobalBase.ESM_Config_FolderName = txtProjectFilename.Text;
        }

        private void cESMProjectToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in quickFolderToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            txtProjectFilename.Text = "G:\\ESMProject";
            myGlobalBase.ESM_Config_FolderName = txtProjectFilename.Text;
        }

        private void cESMProjectToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in quickFolderToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            txtProjectFilename.Text = "H:\\ESMProject";
            myGlobalBase.ESM_Config_FolderName = txtProjectFilename.Text;
        }

        private void CESMProjectToolStripMenuItem8_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in quickFolderToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            txtProjectFilename.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\ADTSessionLog\\ESMProject";
            myGlobalBase.ESM_Config_FolderName = txtProjectFilename.Text;
        }

        private void openHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            TextBox tbx = new TextBox();
            frm.Controls.Add(tbx);
            frm.Width = 400;
            frm.Height = 400;
            tbx.Multiline = true;
            tbx.Height = frm.Height;
            tbx.Width = frm.Width;
            //-------------------------------------------------------------------------------------
            tbx.AppendText("NVM Configuration for Sensor and OSMath Only \r\n");
            tbx.AppendText("-------------------------------------------------------------\r\n");
            tbx.AppendText("This apply to Page 0 of the NVM Section in Atmel Flash Memory.\r\n");
            tbx.AppendText("Due to limited MCU resource, the following rule should apply\r\n");
            tbx.AppendText("   (1) ADXL357-ODR <= 1000Hz Max.\r\n");
            tbx.AppendText("   (2) MG250-ODR   <= 800Hz Max.\r\n");
            tbx.AppendText("   (3) The PVFS and PGFS setting must conform to this rule\r\n");
            tbx.AppendText("       ADXL357/PVFS = Must be non-fractional number. \r\n");
            tbx.AppendText("       BMG250/PGFS  = Must be non-fractional number. \r\n");
            tbx.AppendText("       NB: When ODR = P*FS, the oversample would be 1 \r\n");
            tbx.AppendText("Alway Read Sensor Datasheet before reconfiguring sensor setting.\r\n");
            tbx.AppendText("Before writing to NVM in ESM tool, make sure to update the setting,\r\n");
            tbx.AppendText("there Update All for all setting or discrete setting for each box\r\n");
            tbx.AppendText("-------------------------------------------------------------\r\n");
            tbx.AppendText("For more details, refer to Technote or Software Programmer Guide\r\n");
            tbx.AppendText("-------------------------------------------------------------\r\n");
            frm.ShowDialog();
            //-------------------------------------------------------------------------------------
        }

        private void UpdateAll_Click(object sender, EventArgs e)
        {
            btnUpdate0.PerformClick();
            btnUpdate1.PerformClick();
            btnUpdate2.PerformClick();
            btnUpdate3.PerformClick();
            btnUpdate4.PerformClick();
            btnUpdate5.PerformClick();
            btnUpdate6.PerformClick();
            btnUpdate7.PerformClick();
            btnUpdate8.PerformClick();
            myMainProg.myRtbTermMessageLF("Updated");
        }



        #endregion

        //#####################################################################################################
        //###################################################################################### Save/Load
        //#####################################################################################################
    }
}

