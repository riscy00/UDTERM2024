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
    public partial class BG_ToCo_Setup : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
        USB_VCOM_Comm myUSBVCOMComm;
        MainProg myMainProg;
        DMFProtocol myDMFProtocol;
        BGMasterSetup myBGMasterSetup;
        DialogSupport mbDialogMessageBox;
        BG_ReportViewer myReportViewer;
        //--------------------------------------   Master Class which include setup, etc. Used by this window and passed reference to MainProg, etc.
        public BGToCoReportClass MasterReportSetup;         
        //--------------------------------------  
        public bool isClientDataTransferDone;
        //--------------------------------------
        #region//================================================================Reference
        public void MyBGMasterSetup(BGMasterSetup myBGMasterSetupRef)
        {
            myBGMasterSetup = myBGMasterSetupRef;
        }

        public void MyReportViewer(BG_ReportViewer myReportViewerRef)
        {
            myReportViewer = myReportViewerRef;
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

        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }

        //public void MyUSBComm(USB_FTDI_Comm myUSBCommRef)
        //{
        //    myUSBComm = myUSBCommRef;
        //}

        public void MyUSBVCOMComm(USB_VCOM_Comm myUSBVCOMCommRef)
        {
            myUSBVCOMComm = myUSBVCOMCommRef;
        }
        public void MyDMFProtocol(DMFProtocol myDMFProtocolRef)
        {
            myDMFProtocol = myDMFProtocolRef;
        }
        #endregion

        public BG_ToCo_Setup()
        {
            InitializeComponent();

            for (int i = 0; i <= 9; i++)
            {
                this.dgvClientData.Rows.Add();
                this.dgvClientData.Rows[i].Cells[0].Value = i;
                this.dgvClientData.Rows[i].Cells[1].Value = "";
            }
            mbDialogMessageBox = new DialogSupport();
            this.dgvClientData.RowsDefaultCellStyle.BackColor = Color.Bisque;
            this.dgvClientData.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;           //Color.Bisque, Color.Beige
            MasterReportSetup = new BGToCoReportClass();
            isClientDataTransferDone = false;
        }

        #region //================================================================BG_ToCo_Setup_Load
        private void BG_ToCo_Setup_Load(object sender, EventArgs e)
        {
            txtBGProjectFilename.Text = MasterReportSetup.ToCo_FolderName;
            tbSamplePeriod.Text = MasterReportSetup.iSampleperoid.ToString();
        }
        #endregion

        #region //================================================================BG_ToCo_Setup_FormClosing
        private void BG_ToCo_Setup_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            myGlobalBase.BG_isToCoSetupOpen = false;                 // Cease Survey CVS Terminal mode.
            ToCoSetup_UpdateTable();                                 // Update MasterReportSetup before closing. 
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================ToCoSetup_Show
        public void ToCoSetup_Show()
        {
            this.BringToFront();
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;

            myGlobalBase.BG_isToCoSetupOpen = true;
            this.Visible = true;
            this.Show();
            //-------------------------------------------------

        }
        #endregion

        #region //================================================================ToCoSetup_ClearTable
        public void ToCoSetup_ClearTable()
        {
            for (int i = 0; i <= 9; i++)
            {
                this.dgvClientData.Rows[i].Cells[0].Value = i;
                this.dgvClientData.Rows[i].Cells[1].Value = "";
                MasterReportSetup.ClearData();
            }

        }
        #endregion

        #region //================================================================ToCoSetup_UpdateTable
        public void ToCoSetup_UpdateTable()
        {
            MasterReportSetup.ClientDataFrame = null;
            MasterReportSetup.ClientDataFrame = new List<BGReportList>();

            for (int i = 0; i <= 9; i++)
            {
                if (dgvClientData.Rows[i].Cells[1].Value.ToString() != "")
                {
                    BGReportList result = new BGReportList();
                    result.iNumber = (UInt32)(0x0020 + i);
                    result.sType = "Client Data:" + (0x0020 + i).ToString("X");
                    result.sNote = dgvClientData.Rows[i].Cells[1].Value.ToString();
                    if (result.sNote.Length>125)
                        result.sNote = result.sNote.Substring(0, 125);                      //Limit maximum string length. 
                    dgvClientData.Rows[i].Cells[1].Value = result.sNote;
                    MasterReportSetup.ClientDataFrame.Add(result);
                }
            }
            if (tbSamplePeriod.Text != MasterReportSetup.iSampleperoid.ToString())      // Has the period changed?
            {
                
                if (Tools.IsString_Numberic_Int32(tbSamplePeriod.Text)==true)
                {
                    MasterReportSetup.isSamplePeroidChanged = true;
                    MasterReportSetup.iSampleperoid = Tools.ConversionStringtoInt32(tbSamplePeriod.Text);
                }
                else
                {
                    mbDialogMessageBox.PopUpMessageBox("ERROR: Sample Period is not numeric, try again","Sample Period Error", 5);
                    tbSamplePeriod.Text = MasterReportSetup.iSampleperoid.ToString();
                }
            }

        }
        #endregion

        #region //================================================================btnCopyReport_Click (copy from report viewer ReportClass.ReportData to MasterReportSetup). 
        private void btnCopyReport_Click(object sender, EventArgs e)
        {
            int row;
            int i;
            try
            {
                if ((myReportViewer.ReportClass.ClientDataFrame == null) || (myReportViewer.ReportClass.ClientDataFrame.Count == 0))
                {
                    mbDialogMessageBox.PopUpMessageBox("No Client Data from Report Window to copy over", "Client Data", 5);
                    return;
                }
                //--------------------------------------------------------------------Client Data.
                for (i = 0; i < myReportViewer.ReportClass.ClientDataFrame.Count; i++)
                {
                    if (myReportViewer.ReportClass.ClientDataFrame[i].sNote != "")
                    {
                        row = (int) myReportViewer.ReportClass.ClientDataFrame[i].iNumber - 0x0020;
                        dgvClientData.Rows[row].Cells[1].Value = myReportViewer.ReportClass.ClientDataFrame[i].sNote;
                    }
                }
                i = 0;
                //--------------------------------------------------------------------Sample Peroid
                while (i < myReportViewer.ReportClass.ReportData.Count)
                {
                    if (myReportViewer.ReportClass.ReportData[i].iNumber==0x001A)
                    {
                        MasterReportSetup.iSampleperoid = (int)Tools.ConversionStringtoUInt32(myReportViewer.ReportClass.ReportData[i].sNote);
                        tbSamplePeriod.Text = MasterReportSetup.iSampleperoid.ToString();
                        break;
                    }
                    i++;
                }
            }
            catch
            {
                mbDialogMessageBox.PopUpMessageBox("Unable to copy Client Data from Report Window over", "Client Data", 5);
            }

        }
        #endregion



        //#####################################################################################################
        //###################################################################################### Save/Load
        //#####################################################################################################


        #region //================================================================btnLoad_Click
        private void btnLoad_Click(object sender, EventArgs e)
        {
            int row;
            int i;
            string sFoldername= MasterReportSetup.ToCo_FolderName;
            string sFilename="";
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Setup Files";
                ofd.Filter = "xml files|*.xml";
                ofd.InitialDirectory = sFoldername;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    sFilename = ofd.FileName;
                    sFoldername = System.IO.Path.GetDirectoryName(ofd.FileName);
                }
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("###ERR: Filename Selection Report an Error");
                return;
            }
            txtBGProjectFilename.Text = sFoldername;
            try
            {
                MasterReportSetup = null;
                MasterReportSetup = new BGToCoReportClass();
                MasterReportSetup = myGlobalBase.DeserializeFromFile<BGToCoReportClass>(sFilename);
               // MasterReportSetup.ToCo_FolderName = sFoldername;
               // MasterReportSetup.ToCo_Setup_FileName = sFilename;      
            }
            catch
            {
                mbDialogMessageBox.PopUpMessageBox("ERROR: Unable to load ToCOSetup within designated folder, may not exist?", "File System Error", 5,12F);
                return;
            }
            //--------------------------------------------------------------------Client Data.
            for (i = 0; i < MasterReportSetup.ClientDataFrame.Count; i++)
            {
                if (MasterReportSetup.ClientDataFrame[i].sNote != "")
                {
                    row = (int)MasterReportSetup.ClientDataFrame[i].iNumber - 0x0020;
                    dgvClientData.Rows[row].Cells[1].Value = MasterReportSetup.ClientDataFrame[i].sNote;
                }
            }
            i = 0;
            //--------------------------------------------------------------------Sample Period
            while (i < MasterReportSetup.ReportData.Count)
            {
                if (MasterReportSetup.ReportData[i].iNumber == 0x001A)
                {
                    MasterReportSetup.iSampleperoid = (int)Tools.ConversionStringtoUInt32(MasterReportSetup.ReportData[i].sNote);
                    tbSamplePeriod.Text = MasterReportSetup.iSampleperoid.ToString();
                    break;
                }
                i++;
            }
            ToCoSetup_UpdateTable();
            mbDialogMessageBox.PopUpMessageBox("Successfully Loaded ToCo Setup File and updated ToCo Setup", "File System", 2,14F);
            this.Invalidate();
            this.Refresh();
            
        }
        #endregion

        #region //================================================================btnSave_Click
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtBGProjectFilename.Text=="")
            {
                mbDialogMessageBox.PopUpMessageBox("ERROR: Folder Not Specified. Please do that first! ", "File System Error", 5, 12F);
                return;
            }
            ToCoSetup_UpdateTable();
            string pathString = MasterReportSetup.sToCo_Setup_FileName();       // No STCSTART TimeStamp included. Use UTC current timestamp instead.
            try
            {
                myGlobalBase.SerializeToFile<BGToCoReportClass>(MasterReportSetup, pathString);
            }
            catch
            {
                mbDialogMessageBox.PopUpMessageBox("ERROR: Unable to save ToCOSetup within designated folder, is this protected folder?", "File System Error", 5, 12F);
                return;
            }
            mbDialogMessageBox.PopUpMessageBox("Successfully saved ToCo Setup File:\n"+ pathString, "Setup File System", 2, 12F);
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
            string sFoldername = MasterReportSetup.ToCo_FolderName;
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
            MasterReportSetup.ToCo_FolderName = sFoldername;
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
                MasterReportSetup.ToCo_FolderName = sFoldername;

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
                string myPath = MasterReportSetup.ToCo_FolderName;
                if (!Directory.Exists(myPath))                             // Default folder name for given drive. 
                {
                    DirectoryInfo di = Directory.CreateDirectory(myPath);  // Create folder if not exist. 
                }
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = myPath;
                prc.Start();
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("###ERR: Unable to Open/Create Folder");
                mbDialogMessageBox.PopUpMessageBox("ERROR! Unable to Open/Create Folder", "File Error in ToCo Setup", 10);
            }
        }
        #endregion
    }
}

