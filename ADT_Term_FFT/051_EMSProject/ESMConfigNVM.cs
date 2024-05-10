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
    public partial class ESMConfigNVM : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        USB_VCOM_Comm myUSBVCOMComm;
        MainProg myMainProg;
        DMFProtocol myDMFProtocol;
        DialogSupport mbDialogMessageBox;
        ESM_NVM_SensorConfigII myESM_NVM_SensorConfigII;
        //--------------------------------------  
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

        public ESMConfigNVM()            
        {
            InitializeComponent();
            mbDialogMessageBox = new DialogSupport();
            myESM_NVM_SensorConfigII = new ESM_NVM_SensorConfigII();
            myESM_NVM_SensorConfigII.InitDefault();
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
            myGlobalBase.ESM_Config_FolderName = myGlobalBase.sSessionFoldername +"\\"+ m_sESMFilenameDefaultTopProject;

            if (!Directory.Exists(myGlobalBase.ESM_Config_FolderName))
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
            //========================================ESMConfigSensor Section
            tbESMConfigSensorFull.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigSensor.ESMConfigSensor_Word.ToString("X8");
            ADXL357_ODR.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigSensor.ADXL357_ODR);
            ADXL357_HPF.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigSensor.ADXL357_HPF);
            ADXL357_Range.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigSensor.ADXL357_RANGE) - 1;
            BMG250_ODR.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigSensor.BMG250_ODR) - 6;
            BMG250_BWP.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigSensor.BMG250_BWP);
            BMG250_Format.SelectedIndex = 0; //= Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigSensor.BMG250_FORMAT);     // Not required. 

            //========================================ESMConfigSensorADXL372
            tbESMConfigSensorADXL372Full.Text = "0x"+ myESM_NVM_SensorConfigII.myESMConfigSensorADXL372.ESMConfigSensorADXL372_Word.ToString("X8");
            ADXL372_ODR.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigSensorADXL372.ADXL372_ODR);
            ADXL372_BWP.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigSensorADXL372.ADXL372_BW);
            if (myESM_NVM_SensorConfigII.myESMConfigSensorADXL372.ADXL372_TH == 0)
            {
                rbADXL372LowG.Checked = true;
                rbADXL372HighG.Checked = false;
            }
            else
            {
                rbADXL372LowG.Checked = false;
                rbADXL372HighG.Checked = true;
            }
            //========================================ESMConfigMathADXL357
            tbESMConfigMathADXL357Full.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigMathADXL357.ESMConfigMathADXL357_Word.ToString("X8");
            //---------------Clear Checkbox in group
            foreach (Control c in gbVSelectMath.Controls)  
            {
                if (c is CheckBox)
                {
                    CheckBox cb = (CheckBox)c;
                    cb.Checked = false;
                }
            }
            cbVSerialOut.Checked = false;
            cbVData16.Checked = false;
            //--------------- Update checkboxes
            cbVAVG.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL357.isAVG();
            cbVRMS.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL357.isRMS();
            cbVFFT.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL357.isFFT();
            cbVMIN.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL357.isMin();
            cbVMAX.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL357.isMax();
            cbVFIRST.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL357.isFirst();
            cbVSerialOut.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL357.isHFStream();
            cbVData16.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL357.isData16();

            cbVDebug.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigMathADXL357.ReadAssignedData16());
            cbVFormat.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigMathADXL357.ReadSelectFormat());
            tbPVFS.Text = myESM_NVM_SensorConfigII.myESMConfigMathADXL357.ADXL357_PVFS.ToString();

            if (cbVData16.Checked == false)
            {
                cbVDebug.Hide();
                label13.Hide();
                cbVFormat.Hide();
            }
            else
            {
                cbVDebug.Show();
                label13.Show();
                cbVFormat.Show();
            }
            //========================================ESMConfigMathBMG250
            tbESMConfigMathBMG250Full.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigMathBMG250.ESMConfigMathBMG250_Word.ToString("X8");
            //---------------Clear Checkbox in group
            foreach (Control c in gbGSelectMath.Controls)
            {
                if (c is CheckBox)
                {
                    CheckBox cb = (CheckBox)c;
                    cb.Checked = false;
                }
            }
            cbGSerialOut.Checked = false;
            cbGData16.Checked = false;
            //--------------- Update checkboxes
            cbGAVG.Checked = myESM_NVM_SensorConfigII.myESMConfigMathBMG250.isAVG();
            cbGRMS.Checked = myESM_NVM_SensorConfigII.myESMConfigMathBMG250.isRMS();
            cbGFFT.Checked = myESM_NVM_SensorConfigII.myESMConfigMathBMG250.isFFT();
            cbGMIN.Checked = myESM_NVM_SensorConfigII.myESMConfigMathBMG250.isMin();
            cbGMAX.Checked = myESM_NVM_SensorConfigII.myESMConfigMathBMG250.isMax();
            cbGFIRST.Checked = myESM_NVM_SensorConfigII.myESMConfigMathBMG250.isFirst();
            cbGSerialOut.Checked = myESM_NVM_SensorConfigII.myESMConfigMathBMG250.isHFStream();
            cbGData16.Checked = myESM_NVM_SensorConfigII.myESMConfigMathBMG250.isData16();

            cbGDebug.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigMathBMG250.ReadAssignedData16());
            cbGFormat.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigMathBMG250.ReadSelectFormat());
            tbPGFS.Text = myESM_NVM_SensorConfigII.myESMConfigMathBMG250.BMG250_PGFS.ToString();
            if (cbGData16.Checked == false)
            {
                cbGDebug.Hide();
            }
            else
            {
                cbGDebug.Show();
            }
            //========================================ESMConfigMathADXL372

            tbESMConfigMathADXL372Full.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigMathADXL372.ESMConfigMathADXL372_Word.ToString("X8");
            //---------------Clear Checkbox in group
            foreach (Control c in gbSSelectMath.Controls)
            {
                if (c is CheckBox)
                {
                    CheckBox cb = (CheckBox)c;
                    cb.Checked = false;
                }
            }
            cbSSerialOut.Checked = false;
            cbSData16.Checked = false;
            //--------------- Update checkboxes
            cbSFFT.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL372.isFFT();
            cbSGTotal.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL372.isGTotal();
            cbS5Peak.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL372.isMax5Peak();
            cbS1Peak.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL372.isMax1Peak();
            cbSSerialOut.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL372.isHFStream();
            cbSData16.Checked = myESM_NVM_SensorConfigII.myESMConfigMathADXL372.isData16();

            cbSDebug.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigMathADXL372.ReadAssignedData16());
            cbSFormat.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigMathADXL372.ReadSelectFormat());
            cbSLength.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigMathADXL372.ReadSelectRange());
            if (cbSData16.Checked == false)
            {
                cbSDebug.Hide();
            }
            else
            {
                cbSDebug.Show();
            }

            //========================================ESMConfigINACTQualADXL357

            tbESMConfigINACTQualADXL357Full.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigINACTQualADXL357.ESMConfigINACTQualADXL357_Word.ToString("X8");
            tb357INACTTH.Text = myESM_NVM_SensorConfigII.myESMConfigINACTQualADXL357.INACTThreshold.ToString();
            tb357INACTLength.Text = myESM_NVM_SensorConfigII.myESMConfigINACTQualADXL357.INACTCBuffLength.ToString();
            cb357INACTSelectMath.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigINACTQualADXL357.SelectMath);
            tb357INACTConfigOp.Text = myESM_NVM_SensorConfigII.myESMConfigINACTQualADXL357.ConfigOp.ToString();
            cb357INACTEnable.Checked = myESM_NVM_SensorConfigII.myESMConfigINACTQualADXL357.isEnable;

            //========================================ESMConfigACTQualADXL357

            tbESMConfigACTQualADXL357Full.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.ESMConfigACTQualADXL357_Word.ToString("X8");
            tb357ACTTH.Text = myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.ACTThreshold.ToString();
            tb357ACTLength.Text = myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.ACTCBuffLength.ToString();
            cb357ACTSelectMath.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.SelectMath);
            tb357ACTConfigOp.Text = myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.ConfigOp.ToString();
            if (myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.Range357 != 0)
            {
                cb357ACTRange.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.Range357) - 1;
            }
            cb357ACTHPF.Checked = myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.isHPFEnable;
            cb357ACTEnable.Checked = myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.isEnable;

            //========================================ESMConfigACTQualADXL372

            tbESMConfigACTQualADXL372Full.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.ESMConfigACTQualADXL372_Word.ToString("X8");
            tb372ACTTH.Text = myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.ADXL372_ACTThreshold.ToString();
            tb372ACTLength.Text = myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.ADXL372_ACTAVGPeriod.ToString();
            cb372ACTSelectMath.SelectedIndex = Convert.ToInt32(myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.SelectMath);
            tb372ACTConfigOp.Text = myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.ConfigOp.ToString();
            tb372ACTCounter.Text = myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.CountRef.ToString();
            cb372ACTEnable.Checked = myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.isEnable;
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
                myMainProg.myRtbTermMessageLF("###ERR: Filename Selection Report an Error");
                return;
            }
            txtProjectFilename.Text = sFoldername;
            try
            {
                myESM_NVM_SensorConfigII = null;
                myESM_NVM_SensorConfigII = new ESM_NVM_SensorConfigII();
                myESM_NVM_SensorConfigII = myGlobalBase.DeserializeFromFile<ESM_NVM_SensorConfigII>(sFilename);    
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
            pathString = System.IO.Path.Combine(pathString, "ESMConfigSensor_" + Tools.uDateTimeToUnixTimestampNowUTC().ToString() + ".xml");
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
                myGlobalBase.SerializeToFile<ESM_NVM_SensorConfigII>(myESM_NVM_SensorConfigII, saveFileDialog1.FileName);
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
            myESM_NVM_SensorConfigII.myESMConfigSensor.ADXL357_ODR = Convert.ToUInt32(ADXL357_ODR.SelectedIndex);
            myESM_NVM_SensorConfigII.myESMConfigSensor.ADXL357_HPF = Convert.ToUInt32(ADXL357_HPF.SelectedIndex);
            myESM_NVM_SensorConfigII.myESMConfigSensor.ADXL357_RANGE = Convert.ToUInt32(ADXL357_Range.SelectedIndex) + 1;
            myESM_NVM_SensorConfigII.myESMConfigSensor.BMG250_ODR = Convert.ToUInt32(BMG250_ODR.SelectedIndex) + 6;
            myESM_NVM_SensorConfigII.myESMConfigSensor.BMG250_BWP = Convert.ToUInt32(BMG250_BWP.SelectedIndex);
            myESM_NVM_SensorConfigII.myESMConfigSensor.BMG250_FORMAT = 0; //= Convert.ToUInt32(BMG250_Format.SelectedIndex);        15/10/2019 not required.
            myESM_NVM_SensorConfigII.myESMConfigSensor.ConfigGenerateWord();

            tbESMConfigSensorFull.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigSensor.ESMConfigSensor_Word.ToString("X8");

        }

        private void btnUpdate1_Click(object sender, EventArgs e)
        {
            if (cbVData16.Checked==false)
            {
                cbVDebug.Hide(); 
                label13.Hide();
                cbVFormat.Hide();
            }
            else
            {
                cbVDebug.Show();
                label13.Show();
                cbVFormat.Show();
            }
            //========================================ESMConfigMathADXL357
            myESM_NVM_SensorConfigII.myESMConfigMathADXL357.WriteAVG(Convert.ToUInt32(cbVAVG.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL357.WriteRMS(Convert.ToUInt32(cbVRMS.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL357.WriteFFT(Convert.ToUInt32(cbVFFT.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL357.WriteMin(Convert.ToUInt32(cbVMIN.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL357.WriteMax(Convert.ToUInt32(cbVMAX.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL357.WriteFirst(Convert.ToUInt32(cbVFIRST.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL357.WriteHFStream(Convert.ToUInt32(cbVSerialOut.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL357.WriteData16(Convert.ToUInt32(cbVData16.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL357.WriteAssignedData16(Convert.ToUInt32(cbVDebug.SelectedIndex));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL357.WriteSelectFormat(Convert.ToUInt32(cbVFormat.SelectedIndex));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL357.ADXL357_PVFS = Tools.ConversionStringtoUInt32(tbPVFS.Text);
            myESM_NVM_SensorConfigII.myESMConfigMathADXL357.ConfigGenerateWord();

            tbESMConfigMathADXL357Full.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigMathADXL357.ESMConfigMathADXL357_Word.ToString("X8");

        }

        private void btnUpdate2_Click(object sender, EventArgs e)
        {
            if (cbGData16.Checked==false)
            {
                cbGDebug.Hide();
            }
            else
            {
                cbGDebug.Show();
            }
            //========================================ESMConfigMathBMG250
            myESM_NVM_SensorConfigII.myESMConfigMathBMG250.WriteAVG(Convert.ToUInt32(cbGAVG.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathBMG250.WriteRMS(Convert.ToUInt32(cbGRMS.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathBMG250.WriteFFT(Convert.ToUInt32(cbGFFT.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathBMG250.WriteMin(Convert.ToUInt32(cbGMIN.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathBMG250.WriteMax(Convert.ToUInt32(cbGMAX.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathBMG250.WriteFirst(Convert.ToUInt32(cbGFIRST.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathBMG250.WriteHFStream(Convert.ToUInt32(cbGSerialOut.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathBMG250.WriteData16(Convert.ToUInt32(cbGData16.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathBMG250.WriteAssignedData16(Convert.ToUInt32(cbGDebug.SelectedIndex));
            myESM_NVM_SensorConfigII.myESMConfigMathBMG250.WriteSelectFormat(Convert.ToUInt32(cbGFormat.SelectedIndex));
            myESM_NVM_SensorConfigII.myESMConfigMathBMG250.BMG250_PGFS = Tools.ConversionStringtoUInt32(tbPGFS.Text);
            myESM_NVM_SensorConfigII.myESMConfigMathBMG250.ConfigGenerateWord();

            tbESMConfigMathBMG250Full.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigMathBMG250.ESMConfigMathBMG250_Word.ToString("X8");
        }

        private void btnUpdate3_Click(object sender, EventArgs e)
        {
            if (cbSData16.Checked == false)
            {
                cbSDebug.Hide();
            }
            else
            {
                cbSDebug.Show();
            }
            //========================================ESMConfigMathADXL372
            myESM_NVM_SensorConfigII.myESMConfigMathADXL372.WriteFFT(Convert.ToUInt32(cbSFFT.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL372.WriteGTotal(Convert.ToUInt32(cbSGTotal.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL372.WriteMax1Peak(Convert.ToUInt32(cbS5Peak.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL372.WriteMax5Peak(Convert.ToUInt32(cbS1Peak.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL372.WriteHFStream(Convert.ToUInt32(cbSSerialOut.Checked));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL372.WriteData16(Convert.ToUInt32(cbSData16.Checked));

            myESM_NVM_SensorConfigII.myESMConfigMathADXL372.WriteAssignedData16(Convert.ToUInt32(cbSDebug.SelectedIndex));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL372.WriteSelectFormat(Convert.ToUInt32(cbSFormat.SelectedIndex));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL372.WriteSelectRange(Convert.ToUInt32(cbSLength.SelectedIndex));
            myESM_NVM_SensorConfigII.myESMConfigMathADXL372.ConfigGenerateWord();

            tbESMConfigMathADXL372Full.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigMathADXL372.ESMConfigMathADXL372_Word.ToString("X8");
        }

        private void btnUpdate4_Click(object sender, EventArgs e)
        {
            //========================================ESMConfigINACTQualADXL357
            myESM_NVM_SensorConfigII.myESMConfigINACTQualADXL357.INACTThreshold = Tools.ConversionStringtoUInt32(tb357INACTTH.Text);
            myESM_NVM_SensorConfigII.myESMConfigINACTQualADXL357.INACTCBuffLength = Tools.ConversionStringtoUInt32(tb357INACTLength.Text);
            myESM_NVM_SensorConfigII.myESMConfigINACTQualADXL357.SelectMath = Convert.ToUInt32(cb357INACTSelectMath.SelectedIndex);
            myESM_NVM_SensorConfigII.myESMConfigINACTQualADXL357.ConfigOp = Tools.ConversionStringtoUInt32(tb357INACTConfigOp.Text);
            myESM_NVM_SensorConfigII.myESMConfigINACTQualADXL357.isEnable = cb357INACTEnable.Checked;
            myESM_NVM_SensorConfigII.myESMConfigINACTQualADXL357.ConfigGenerateWord();

            tbESMConfigINACTQualADXL357Full.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigINACTQualADXL357.ESMConfigINACTQualADXL357_Word.ToString("X8");

        }

        private void btnUpdate5_Click(object sender, EventArgs e)
        {
            //========================================ESMConfigACTQualADXL357
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.ACTThreshold = Tools.ConversionStringtoUInt32(tb357ACTTH.Text);
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.ACTCBuffLength = Tools.ConversionStringtoUInt32(tb357ACTLength.Text);
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.SelectMath = Convert.ToUInt32(cb357ACTSelectMath.SelectedIndex);
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.ConfigOp = Tools.ConversionStringtoUInt32(tb357ACTConfigOp.Text);
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.isEnable = cb357ACTEnable.Checked;
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.isHPFEnable = cb357ACTHPF.Checked;
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.Range357 = Convert.ToUInt32(cb357ACTRange.SelectedIndex) + 1;
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.ConfigGenerateWord();
            tbESMConfigACTQualADXL357Full.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigACTQualADXL357.ESMConfigACTQualADXL357_Word.ToString("X8");     // Bug Fix 78F
        }

        private void btnUpdate6_Click(object sender, EventArgs e)
        {
            //========================================ESMConfigACTQualADXL372
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.ADXL372_ACTThreshold = Tools.ConversionStringtoUInt32(tb372ACTTH.Text);
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.ADXL372_ACTAVGPeriod = Tools.ConversionStringtoUInt32(tb372ACTLength.Text);
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.SelectMath = Convert.ToUInt32(cb372ACTSelectMath.SelectedIndex);
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.ConfigOp = Tools.ConversionStringtoUInt32(tb372ACTConfigOp.Text);
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.CountRef = Tools.ConversionStringtoUInt32(tb372ACTCounter.Text);
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.isEnable = cb372ACTEnable.Checked;
            myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.ConfigGenerateWord();

            tbESMConfigACTQualADXL372Full.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigACTQualADXL372.ESMConfigACTQualADXL372_Word.ToString("X8");

        }

        private void btnUpdate7_Click(object sender, EventArgs e)
        {
            tbESMConfigMathADXL3572IIFull.Text = "WIP WIP WIP";
        }

        private void btnUpdate9_Click(object sender, EventArgs e)
        {
            //========================================ESMConfigSensorADXL372
            myESM_NVM_SensorConfigII.myESMConfigSensorADXL372.ADXL372_ODR = Convert.ToUInt32(ADXL372_ODR.SelectedIndex);
            myESM_NVM_SensorConfigII.myESMConfigSensorADXL372.ADXL372_BW = Convert.ToUInt32(ADXL372_BWP.SelectedIndex);
            if (rbADXL372HighG.Checked == true)
                myESM_NVM_SensorConfigII.myESMConfigSensorADXL372.ADXL372_TH = 1;
            else
                myESM_NVM_SensorConfigII.myESMConfigSensorADXL372.ADXL372_TH = 0;
            myESM_NVM_SensorConfigII.myESMConfigSensorADXL372.ConfigGenerateWord();

            tbESMConfigSensorADXL372Full.Text = "0x" + myESM_NVM_SensorConfigII.myESMConfigSensorADXL372.ESMConfigSensorADXL372_Word.ToString("X8");

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### NVM Read/Write with ESM. 
        //#####################################################################################################

        #region //================================================================DMFP_GenericCallBack
        public void DMFP_GenericCallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            string ErrorMessage = "ERROR: Callback Error Message: " + myESM_NVM_SensorConfigII.ErrorCode;
            if (myESM_NVM_SensorConfigII.CMDCFG_GenericCallBack(hPara) != 0xFF)
            {
                mbDialogMessageBox.PopUpMessageBox(ErrorMessage, "ESM NVM Error", 5, 12F);
                myMainProg.myRtbTermMessageLF(ErrorMessage);
            }

        }
        #endregion

        #region //================================================================btnReadNVM_Click
        private void btnReadNVM_Click(object sender, EventArgs e)
        {
            DMFP_Delegate fpCallBack_ToolStop = new DMFP_Delegate(DMFP_ReadNVM);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolStop, myESM_NVM_SensorConfigII.CMDCFG_Write91(), false, -1);      // Generic VCOM.
        }
        #endregion

        #region //================================================================DMFP_ReadNVM
        public void DMFP_ReadNVM(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (myESM_NVM_SensorConfigII.CMDCFG_CallBack_91(hPara)!=0xFF)
            {
                string ErrorMessage = "ERROR: Callback from +CMDCFGS(0x41) reported Error Code: " + hPara[0].ToString();
                mbDialogMessageBox.PopUpMessageBox(ErrorMessage, "ESM NVM Error", 5, 12F);
                myMainProg.myRtbTermMessageLF(ErrorMessage);
            }
            NVMWordToWindowNow();
        }
        #endregion

        #region //================================================================btnDefaultNVM_Click
        private void btnDefaultNVM_Click(object sender, EventArgs e)
        {
            DMFP_Delegate fpCallBack_ToolStop = new DMFP_Delegate(DMFP_GenericCallBack);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolStop, myESM_NVM_SensorConfigII.CMDCFG_CMD_F0(), false, -1);
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
                DMFP_Delegate fpCallBack_ToolStop = new DMFP_Delegate(DMFP_GenericCallBack);
                myDMFProtocol.DMFP_Process_Command(fpCallBack_ToolStop, myESM_NVM_SensorConfigII.CMDCFG_Write90(), false, -1);
            }
        }
        #endregion

        #region //================================================================Quick Folder
        private void cADTSessionLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in quickFolderToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            txtProjectFilename.Text = "C:\\ESMProject";
            myGlobalBase.ESM_Config_FolderName = txtProjectFilename.Text;
        }

        private void cESMProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in quickFolderToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            txtProjectFilename.Text = "D:\\ESMProject";
            myGlobalBase.ESM_Config_FolderName = txtProjectFilename.Text;
        }

        private void cESMProjectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in quickFolderToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            txtProjectFilename.Text = "E:\\ESMProject";
            myGlobalBase.ESM_Config_FolderName = txtProjectFilename.Text;
        }

        private void cESMProjectToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in quickFolderToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            txtProjectFilename.Text = "F:\\ESMProject";
            myGlobalBase.ESM_Config_FolderName = txtProjectFilename.Text;
        }

        private void cESMProjectToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in quickFolderToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            txtProjectFilename.Text = "G:\\ESMProject";
            myGlobalBase.ESM_Config_FolderName = txtProjectFilename.Text;
        }

        private void cESMProjectToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in quickFolderToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
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
            txtProjectFilename.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\ADTSessionLog\\ESMProject";      //C:\Users\Public\Documents
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
        }




        #endregion

        //#####################################################################################################
        //###################################################################################### Save/Load
        //#####################################################################################################
    }
}

