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
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using System.Xml;
using System.Text.RegularExpressions;
using JR.Utils.GUI.Forms;

namespace UDT_Term_FFT
{
    public partial class BG_ToolSetup : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        EE_DownloadSurvey myDownloadSurvey;
        MainProg myMainProg;
        Timer EEtimer;
        BGMasterSetup myBGMasterSetup;

        BindingList<BGMasterJobSurvey> blBGMasterJobSurvey;                 // Binding List for basic setup.
        BindingList<AvdToolSetup> blAvdToolSetup;                           // Binding List for advanced setup.

        private string m_sEntryTxt;
        private bool isAdvancedViewShow;

        #region//================================================================Reference
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }

        public void MyDownloadSurvey(EE_DownloadSurvey myDownloadSurveyRef)
        {
            myDownloadSurvey = myDownloadSurveyRef;
        }
        public void MyBGMasterSetup(BGMasterSetup myBGMasterSetupRef)
        {
            myBGMasterSetup = myBGMasterSetupRef;
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

        public void MyBGMasterJobSurvey(BGMasterJobSurvey myMasterJobSurveyRef)
        {
            if (blBGMasterJobSurvey == null)
            {
                blBGMasterJobSurvey = new BindingList<BGMasterJobSurvey>();
                blBGMasterJobSurvey.Add(new BGMasterJobSurvey());     // Default Setting if filename not exist.
                blBGMasterJobSurvey[0] = myMasterJobSurveyRef;
            }
            else
            {
                blBGMasterJobSurvey[0] = myMasterJobSurveyRef;
            }
        }

        #endregion

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public BG_ToolSetup()
        {
            InitializeComponent();
            blBGMasterJobSurvey = null;
            isAdvancedViewShow = false;
            this.MaximumSize = new System.Drawing.Size(716, 392);
            this.Size = new System.Drawing.Size(716, 392);
            rbFeetSetup.Checked = false;
            rbMeterSetup.Checked = true;
        }
        //#####################################################################################################
        //###################################################################################### Form Management
        //#####################################################################################################

        #region //================================================================BG_ToolSetup_Load
        private void BG_ToolSetup_Load(object sender, EventArgs e)
        {
            BG_JobSurvey_UpdateDisplay();
            BG_AdvancedSetup_Init();
        }
        #endregion

        #region //================================================================BG_JobSurvey_UpdateDisplay
        private void BG_JobSurvey_UpdateDisplay()
        {
            txtBGProjectFilename.Text = myBGMasterSetup.sFile_JobProjectAll;

            tbNote.Text = blBGMasterJobSurvey[0].sNote;
            tbJobRef.Text = blBGMasterJobSurvey[0].sJobRef;
            tbLocation.Text = blBGMasterJobSurvey[0].sLocation;
            tbDriller.Text = blBGMasterJobSurvey[0].sDriller;
            tbBoreholeName.Text = blBGMasterJobSurvey[0].sBoreholeName;
            tbBoreHoleDepth.Text = blBGMasterJobSurvey[0].sBoreHoleDepth;
            tbInterval.Text = (blBGMasterJobSurvey[0].iStationInterval / 10).ToString() + "M";
            tbGryoSurvey.Text = blBGMasterJobSurvey[0].sGyroSurveyLoop;
            tbWaitSec.Text = blBGMasterJobSurvey[0].iWaitPeroidSec.ToString("N0");
        }
        #endregion

        #region //================================================================EE_DownloadSurvey_Show
        public void BG_ToolSetup_Show()
        {
            this.BringToFront();
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            myGlobalBase.BG_isToolSetupOpen = true;
            this.Visible = true;
            this.Show();
            this.Focus();
            this.TopMost = false;
        }
        #endregion

        #region //================================================================BG_ToolSetup_FormClosing
        private void BG_ToolSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            //---------------------------------- Save Survey. 
            btnSave.PerformClick();
            //---------------------------------- Close window.
            myGlobalBase.BG_isToolSetupOpen = false;                 // Cease Survey CVS Terminal mode.
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================btnClose_Click
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Validate Data
        //#####################################################################################################

        #region //================================================================BG_Interval_Input
        private bool BG_Interval_Input()
        {
            int Interval;
            string sInterval = tbInterval.Text;
            if (sInterval.Contains(".") == true)        // Decimal point in string
            {
                sInterval = sInterval.Replace("M", "").Replace("m", "").Replace("ft", "").Replace("FT", "").Replace("cm", "").Replace(" ", "").Replace(",", "").Replace(".", "");
                Interval = Tools.ConversionStringtoInt32(sInterval);
            }
            else                                        // No Decimal point, treat as whole number. 
            {
                sInterval = sInterval.Replace("M", "").Replace("m", "").Replace("ft", "").Replace("FT", "").Replace("cm", "").Replace(" ", "").Replace(",", "").Replace(".", "");
                Interval = Tools.ConversionStringtoInt32(sInterval) * 10;     //500 = 50.0M
            }
            if (Interval == -9755790)
            {
                tbInterval.Text = "ERR";
                tbInterval.BackColor = Color.Red;
                return (false) ;
            }
            blBGMasterJobSurvey[0].iStationInterval = Interval;          // remember 50M mean 500 
            tbInterval.BackColor = Color.Honeydew;
            tbInterval.Text = (Interval / 10).ToString() + "M";
            return (true);
        }
        #endregion

        #region //================================================================BG_Wait_Peroid
        private bool BG_Wait_Peroid()
        {
            string sInt = tbWaitSec.Text;
            Regex digitsOnly = new Regex(@"[^\d]");
            sInt = digitsOnly.Replace(sInt, "");
            blBGMasterJobSurvey[0].iWaitPeroidSec = Tools.ConversionStringtoInt32(sInt);
            if (blBGMasterJobSurvey[0].iWaitPeroidSec <= 3)         // Minimum is 3 second. 
                blBGMasterJobSurvey[0].iWaitPeroidSec = 3;
            tbWaitSec.Text = blBGMasterJobSurvey[0].iWaitPeroidSec.ToString("N0"); 
            return (true);
        }
        #endregion

        #region //================================================================BG_Gyro_Input
        private bool BG_Gyro_Input()
        {
            int GyroSurvey = Tools.ConversionStringtoInt32(tbGryoSurvey.Text);
            if (GyroSurvey == -975579)
            {
                tbGryoSurvey.Text = "ERR";
                tbGryoSurvey.BackColor = Color.Red;
                return (false);
            }
            if (GyroSurvey <= 1)
            {
                tbGryoSurvey.Text = "ERR";
                tbGryoSurvey.BackColor = Color.Red;
                return (false);
            }
            //---------------------------------------------------------------------------------------Update Tool.
            if (Tools.ConversionStringtoInt32(blBGMasterJobSurvey[0].sGyroSurveyLoop) != GyroSurvey)
            {
                blBGMasterJobSurvey[0].sGyroSurveyLoop = tbGryoSurvey.Text;
                m_sEntryTxt = "STC_COUNT(" + tbGryoSurvey.Text + ")";
                myMainProg.myRtbTermMessageLF(m_sEntryTxt);
                myMainProg.Command_ASCII_Process_UARTs(m_sEntryTxt);
                tbGryoSurvey.BackColor = Color.Honeydew;
            }
            return (true);
        }
        #endregion


        //#####################################################################################################
        //###################################################################################### File Management
        //#####################################################################################################

        #region //================================================================btnProjectFolder_Click
        private void btnFolder_Click(object sender, EventArgs e)
        {
            string sFoldername = myBGMasterSetup.sFile_TopFolderProjectAll;
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
            fbdToolSetup.SelectedPath = sFoldername;
            try
            { 
                //----------------------------------------------------------
                DialogResult result = fbdToolSetup.ShowDialog();
                if (result == DialogResult.OK)
                {
                    sFoldername = fbdToolSetup.SelectedPath;
                    if (!Directory.Exists(sFoldername))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(sFoldername);  // Create folder if not exist. 
                    }
                }
                txtBGProjectFilename.Text = sFoldername;
                myBGMasterSetup.sFile_JobProjectAll = sFoldername;
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("###ERR: Folder Dialogue Reported an Error");
            }

        }
        #endregion

        #region //================================================================BtnOpenFolder_Click
        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string myPath = myBGMasterSetup.sFile_JobProjectAll;
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = myPath;
                prc.Start();
            }
            catch
            {
                txtBGProjectFilename.Text += "#ERR####";
                myMainProg.myRtbTermMessageLF("###ERR: Unable to Open Folder");
            }

        }
        #endregion

        #region //================================================================btnSave_Click
        private void btnSave_Click(object sender, EventArgs e)
        {
            //----------------------------------Validate GyroSurvey setting and if changed, update tool with new GyroSurvey Loop Setting. 
            if (BG_Gyro_Input() == false)
            {
                MessageBox.Show("Error in Gyro, 1 to 100", "Gyro Setup", MessageBoxButtons.OK);
                return;
            }
            //----------------------------------Validate GyroSurvey setting and if changed, update tool with new GyroSurvey Loop Setting. 
            if (BG_Interval_Input() == false)
            {
                MessageBox.Show("Error in Survey Interval, 1.0M to 1000.0M", "Interval Setup", MessageBoxButtons.OK);
                return;
            }
            //----------------------------------Convert Wait Period to int number in second.
            BG_Wait_Peroid();
            //----------------------------------
            DialogResult dr = MessageBox.Show("Review entry and click <ok> to save JobSurvey, otherwise <Cancel>", "Save JobSurvey", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.Cancel)
            {
                return;
            }

            //---------------------------------------------------------------Meter/Feet, NB, the driller log are meter, all feet are converted to meter. 
            if (rbFeetSetup.Checked && rbMeterSetup.Checked)             // assumed meter 
            {
                rbFeetSetup.Checked = false;
                rbFeetSetup.Refresh();
                rbMeterSetup.Refresh();
            }
            if (rbFeetSetup.Checked == true)
            {
                blBGMasterJobSurvey[0].bisMeter = false;
            }
            if (rbMeterSetup.Checked == true)
            {
                blBGMasterJobSurvey[0].bisMeter = true;
            }
            //---------------------------------------------------------------Capture text
            //dtpTimeStamp
            blBGMasterJobSurvey[0].sTimeStamp = dtpTimeStamp.Value.ToString("dd-MMM-yyyy hh:mm:ss");
            blBGMasterJobSurvey[0].sNote = tbNote.Text;
            blBGMasterJobSurvey[0].sJobRef = tbJobRef.Text;
            blBGMasterJobSurvey[0].sLocation = tbLocation.Text;
            blBGMasterJobSurvey[0].sDriller = tbDriller.Text;
            blBGMasterJobSurvey[0].sBoreholeName = tbBoreholeName.Text;
            blBGMasterJobSurvey[0].sBoreHoleDepth = tbBoreHoleDepth.Text;
            string filename = myBGMasterSetup.sFile_JobProjectAll + "\\" + myBGMasterSetup.sFilenameDefaultJobSurvey + ".xml";
            //---------------------------------------------------------------Save to Filename
            try
            {
                myGlobalBase.SerializeToFile<BindingList<BGMasterJobSurvey>>(blBGMasterJobSurvey, filename);
                MessageBox.Show("Successfully Saved Job Survey File!", "Saved Job File",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Error: Unable to save Job Survey File, try different drive/folder", "Error Save Job File",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        #endregion

        #region //================================================================btnLoad_Click

        private void btnLoad_Click(object sender, EventArgs e)
        {
            BindingList<BGMasterJobSurvey> blBGMasterJobSurvey = null;
            ofdBGToolSetup.Filter = "xml files (*.xml)|*.xml";
            ofdBGToolSetup.InitialDirectory = myBGMasterSetup.sFile_JobProjectAll;
            ofdBGToolSetup.Title = "Load XML Scope Data/Gain Setting Files";
            DialogResult dr = ofdBGToolSetup.ShowDialog();
            if (dr == DialogResult.OK)
            {
                blBGMasterJobSurvey = myGlobalBase.DeserializeFromFile<BindingList<BGMasterJobSurvey>>(ofdBGToolSetup.FileName);
                if (blBGMasterJobSurvey == null)
                {
                    MessageBox.Show("Unable to Load Gain/Offset Setting Configuration File (XML)",
                        "Load Config File Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    tbNote.Text = "Error";
                    tbJobRef.Text = "Error";
                    tbLocation.Text = "Error";
                    tbDriller.Text = "Error";
                    tbBoreholeName.Text = "Error";
                    tbBoreHoleDepth.Text = "Error";
                    return;
                }
                BG_JobSurvey_UpdateDisplay();
            }
        }
        #endregion

        private void btnSetupSave_Click(object sender, EventArgs e)
        {

        }

        private void btnSetupLoad_Click(object sender, EventArgs e)
        {

        }


        //#####################################################################################################
        //###################################################################################### Basic Setup (Driller)
        //#####################################################################################################

        #region //================================================================btnDefault_Click
        private void btnDefault_Click(object sender, EventArgs e)
        {
            blBGMasterJobSurvey[0].sGyroSurveyLoop = "4";
            blBGMasterJobSurvey[0].iStationInterval = 500;
            blBGMasterJobSurvey[0].iWaitPeroidSec = 20;
            tbWaitSec.Text = "20";
            tbInterval.Text = "50.0M";
            tbGryoSurvey.Text = blBGMasterJobSurvey[0].sGyroSurveyLoop;
            //--------------------------------------------------------Update Tool 
            m_sEntryTxt = "STC_COUNT(4)";
            myMainProg.myRtbTermMessageLF(m_sEntryTxt);
            myMainProg.Command_ASCII_Process_UARTs(m_sEntryTxt);
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Tool Check / Battery
        //#####################################################################################################

        #region //================================================================btnBatteryReset_Click
        private void btnBatteryReset_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("!!WARNING!!: This apply only if you changed tool battery. <Are you sure?>", "Battery Reset", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand);
            if (dr == DialogResult.Cancel)
                return;
            m_sEntryTxt = "BAT_RESETCAP()";
            myMainProg.myRtbTermMessageLF(m_sEntryTxt);
            myMainProg.Command_ASCII_Process_UARTs(m_sEntryTxt);            // This reset the battery capacity in EEPROM location. 
            Thread.Sleep(100);
            m_sEntryTxt = "BAT_TEST()";
            myMainProg.myRtbTermMessageLF(m_sEntryTxt);
            myMainProg.Command_ASCII_Process_UARTs(m_sEntryTxt);
        }
        #endregion

        #region //================================================================btnTestNow_Click
        private void btnTestNow_Click(object sender, EventArgs e)
        {
            //--------------------------Sensor/Diag Check Command
            tbSensorCheck.Text = "####";
            tbPSUCheck.Text = "####";
            tbDiagCommCheck.Text = "Failed";
            //---------------------------
            m_sEntryTxt = "BAT_TEST()";
            myMainProg.myRtbTermMessageLF(m_sEntryTxt);
            myMainProg.Command_ASCII_Process_UARTs(m_sEntryTxt);
            //---------------------------Start timer for non communication test. 
            if (EEtimer == null)
            {
                EEtimer = new Timer();
                EEtimer.Elapsed += new System.Timers.ElapsedEventHandler(CommTestTimer);
            }

            EEtimer.Interval = 2000;
            EEtimer.AutoReset = false;
            EEtimer.Start();
        }
        #endregion

        #region //================================================================zBG_Async_Battery
        //Async: ~BATTERY(ToolStatus; VRAWReadout; IRAWReadout; V3V3Readout; CapacityLeft; SpecCapacity; SpecVoltage)\n
        public void zBG_Async_Battery(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, IList<string> Asynclistparameter)
        {
            try
            {
                UInt32 ToolStatus = hPara[0];
                UInt32 TransData_Vraw = hPara[1];
                UInt32 TransData_Iraw = hPara[2];
                UInt32 TransData_3V3 = hPara[3];
                int BatRemainingCap = (int)hPara[4];
                int BatSpecCap = (int)hPara[5];
                UInt32 BatSpecVolt = hPara[7];

                //----------------------------------------------Communication Test. 
                EEtimer.Stop();
                tbDiagCommCheck.Text = "Passed";
                //----------------------------------------------Battery Capacity
                tbBatteryStatus.Text = ( BatSpecCap/ BatRemainingCap).ToString("P1");
                if (BatSpecCap / BatRemainingCap  < 0.25)
                    tbBatteryStatus.BackColor = Color.Red;
                else
                    tbBatteryStatus.BackColor = Color.White;
                //----------------------------------------------PSU checking
                if ((TransData_3V3 > 2700) | (TransData_3V3 < 3800))
                    tbPSUCheck.Text = "Passed";
                else
                    tbPSUCheck.Text = "Failed";
                //----------------------------------------------Display Report in debug window.
                myMainProg.myRtbTermMessageLF("INFO: STC Tool Status      :0x" + hPara[0].ToString("X"));
                myMainProg.myRtbTermMessageLF("INFO: VRAW TransData(mV)   :" + hPara[1].ToString());
                myMainProg.myRtbTermMessageLF("INFO: IRAW TransData(uA)   :" + hPara[2].ToString());
                myMainProg.myRtbTermMessageLF("INFO: 3V3 TransData (mV)   :" + hPara[3].ToString());
                myMainProg.myRtbTermMessageLF("INFO: Bat Remain  (uAHr)   :" + hPara[4].ToString());
                myMainProg.myRtbTermMessageLF("INFO: Bat Spec    (uAHr)   :" + hPara[5].ToString());
                myMainProg.myRtbTermMessageLF("INFO: Deduct/Frame  (uA)   :" + hPara[6].ToString());
                myMainProg.myRtbTermMessageLF("INFO: Bat Spec Volt (mV)   :" + hPara[7].ToString());
            }
            catch { }
            //-------------------------------------------------------------------Now call for BGCHECK()
            m_sEntryTxt = "BGCHECK()";
            myMainProg.myRtbTermMessageLF(m_sEntryTxt);
            myMainProg.Command_ASCII_Process_UARTs(m_sEntryTxt);
        }
        #endregion

        #region //================================================================zBG_Async_BGCheck
        //Async: ~BATTERY(ToolStatus; VRAWReadout; IRAWReadout; V3V3Readout; CapacityLeft; SpecCapacity; SpecVoltage)\n
        public void zBG_Async_BGCheck(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, IList<string> Asynclistparameter)
        {
            Thread.Sleep(200);      // Short pause for comm to settle down. 
            try
            {
                UInt32 ToolStatus = hPara[0];
                //----------------------------------------------Battery Capacity
                if (ToolStatus == 0x0F)
                    tbSensorCheck.Text = "Passed";
                else
                    tbSensorCheck.Text = "Failed";

            }
            catch
            { }
        }
        #endregion

        #region //================================================================CommTestTimer
        void CommTestTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                EEtimer.Stop();
                EEtimer.Elapsed -= CommTestTimer;  // Unsubscribe
                EEtimer = null;
                myMainProg.myRtbTermMessageLF("###ERR: No Communication from Tool <BAT_TEST() or BGCHECK()>, check cable and FT232Rl device");
            }
            catch { }
        }
        #endregion



        //#####################################################################################################
        //###################################################################################### Advanced Tool Configuration (DGV)
        //#####################################################################################################


        private void btnSetupDefault_Click(object sender, EventArgs e)
        {

        }

        private void btnDownhole_Click(object sender, EventArgs e)
        {

        }

        private void btnUpLoad_Click(object sender, EventArgs e)
        {

        }
        private void btnHelp_Click(object sender, EventArgs e)
        {

        }

        //#####################################################################################################
        //###################################################################################### Default Setup.
        //#####################################################################################################

        #region --------------------------------------------------------zEVKIT_SetupDefault()
        private void BG_AdvancedSetup_Init()      // GPIO updated default setting (4/11/15) which in sync with the zEVKit board firmware. 
        {
            if (blAvdToolSetup == null)
            {
                blAvdToolSetup = new BindingList<AvdToolSetup>();
                //for (int i = 0; i <= 10; i++)
                //{
                //    blAvdToolSetup.Add(new AvdToolSetup());
                //}
            }
            blAvdToolSetup.Clear();
            //-------------------------------------------------------------------------------------------------Setup FIle list
            BG_AdvSetup_Init("Sensor", "Use_FX_Accel",      "1", "0b", "Include Freescale Accel into Capture/Survey");
            BG_AdvSetup_Init("Sensor", "Use_MS_Accel",      "1", "0b", "Include MS9002 Accel into Capture/Survey");
            BG_AdvSetup_Init("Sensor", "Use_KXBR_Accel",    "1", "0b", "Include KXBR Accel into Capture/Survey");
            BG_AdvSetup_Init("Sensor", "Use_FX_Mag",        "1", "0b", "Include Freescale Magnetometer into Capture/Survey");
            BG_AdvSetup_Init("Sensor", "Use_FX_Gyro",       "1", "0b", "Include Freescale Gyro into Capture/Survey");
            //-----------------------------------------------Now we bind the above to datagridview
            //BindingList<AvdToolSetup> blEVKITADCData;

            var source = new BindingSource(blAvdToolSetup, null);
            dgvAdvSetup.DataSource = source;
            dgvAdvSetup.RowTemplate.Height = 21;
            dgvAdvSetup.RowsDefaultCellStyle.BackColor = Color.Bisque;
            dgvAdvSetup.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;           //Color.Bisque, Color.Beige
            #region-------------------------------------------------------Setup Column
            //------------------------------------------------------------Column 0: Class
            DataGridViewColumn column0 = dgvAdvSetup.Columns[0];
            column0.Width = 50;
            column0.FillWeight = 50F;
            column0.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column0.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            column0.ReadOnly = true;
            column0.ToolTipText = "Class of configuration in tool setting";
            //------------------------------------------------------------Column 1: Name
            DataGridViewColumn column1 = dgvAdvSetup.Columns[1];
            column1.Width = 100;
            column1.FillWeight = 100F;
            column1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            column1.ReadOnly = true;
            column1.ToolTipText = "Name of Variable to Adjust in tool, take care with them!";
            //------------------------------------------------------------Column 2: Data
            DataGridViewColumn column2 = dgvAdvSetup.Columns[2];
            column2.Width = 100;
            column2.FillWeight = 100F;
            column2.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            column2.ReadOnly = false;
            column2.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            column2.ToolTipText = "User can modify data here";   
            //------------------------------------------------------------Column 3: Type Format 
            DataGridViewColumn column3 = dgvAdvSetup.Columns[3];
            column3.Width = 50;
            column3.FillWeight = 50F;
            column3.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            column3.ReadOnly = true;
            column3.ToolTipText = "Format type that data is decoded into, Support Only 0b,0x,0u,0i,0d,0s";
            //------------------------------------------------------------Column 4: Description
            DataGridViewColumn column4 = dgvAdvSetup.Columns[4];
            column4.Width = 345;
            column4.FillWeight = 345F;
            column4.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            column4.ReadOnly = true;
            column4.ToolTipText = "Description of Setting row";
            #endregion


        }
        #endregion

        #region --------------------------------------------------------zEVKIT_DefaultSetup
        private void BG_AdvSetup_Init(string sClass, string sName, string sSetting, string sType, string sDescription)
        {
            blAvdToolSetup.Add(new AvdToolSetup());
            blAvdToolSetup[blAvdToolSetup.Count - 1].Class = sClass;
            blAvdToolSetup[blAvdToolSetup.Count - 1].Name = sName;
            blAvdToolSetup[blAvdToolSetup.Count - 1].Setting = sSetting;
            blAvdToolSetup[blAvdToolSetup.Count - 1].sType = sType;
            blAvdToolSetup[blAvdToolSetup.Count - 1].Description = sDescription;
        }
        #endregion

        #region --------------------------------------------------------brnAdvanced_Click
        private void brnAdvanced_Click(object sender, EventArgs e)
        {
            if (isAdvancedViewShow == true)
            {
                isAdvancedViewShow = false;
                this.MaximumSize = new System.Drawing.Size(716, 392);
                this.Size = new System.Drawing.Size(716, 392);
            }
            else
            {
                isAdvancedViewShow = true;
                this.MaximumSize = new System.Drawing.Size(716, 673);
                this.Size = new System.Drawing.Size(716, 673);
            }
        }
        #endregion
    }

    //#####################################################################################################
    //###################################################################################### Class Object for Advanced Setup.
    //#####################################################################################################

    #region --------------------------------------------------------Class ScopeData_Operation
    [Serializable]
        public class AvdToolSetup
        {
            public string Class { get; set; }           // Class Name
            public string Name { get; set; }            // Name of the setup
            public string Setting { get; set; }         // Parameter to change, for bool, use 0 or 1 (not true/false), use auto convert.
            public string sType { get; set; }            // Format Type, ie 0x, 0s, 0i, 0u, 0d standard (see download data)
            public string Description { get; set; }     // Description of the data. 
            // =====================================================Constructor
            public AvdToolSetup()
            {
                Class = "";
                Name = "";
                Setting = "";
                sType = "";
                Description = "";
                Class = "";
            }

        }
        #endregion
}


