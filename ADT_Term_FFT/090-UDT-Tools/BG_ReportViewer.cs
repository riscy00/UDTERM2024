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

// Power Tool: insert QuickLaunch with "@tasks CollapseRegions" to close all region only. 

// Use Command  +REPORT(0xXX)                   This invoke to start transfer data. 0xXX: 0x00 = Reset Counter, 0xFF = All Good. 
//              -REPORT(0xYYYY;string)\n       YYYY = type of report. 0xFFFF = End of stream.          
// Step (1) Download the following data in sequence into several list string until end of data stream -REPORT(0xFFFF; "END")
//---------------------------------------------------------Config section of FLASH.
//          0x0010      CONFIG SerialNo
//          0x0011      CONFIG PartNo1   
//          0x0012      CONFIG PartNo2
//          0x0013      CONFIG EESysFlag
//          0x0014      CONFIG EEBattState
//          0x0015      COFNIG FW Revision Code.   
//          0x0016      CONFIG BatLastRechargeTimeStamp
//          0x0017      CONFIG BatLastRechargeTemp
//          0x0018      CONFIG BatLowVoltageTimeStamp
//          0x0019      CONFIG Survey Consumption per Survey in uAHr. 
//          0x001A      CONFIG SurveyEventPeroid 
//----------------------------------------------------------Below is encoded data in string (later task)
//          0x0020      CONFIG RD0_OrderNo
//          0x0021      CONFIG RD1_UDT_Start
//          0x0022      CONFIG RD2_UDT_Expiry
//          0x0023      CONFIG RD3_PolicyNumber 
//          0x0024      CONFIG RD4_ExtensionPW     
//--------------------------------------------------------Flash content summary       
//          0x0100      $D Number of Survey Data Elements, DO NOT DOWNLOAD DATA.
//          0x0101      $G Number of secondary data elements. DO NOT DOWNLOAD DATA.
//          0x0102      $T Number of Type Data Elements.DO NOT DOWNLOAD DATA.
//          0x0103      $H Number of Header Data Elements.DO NOT DOWNLOAD DATA.
//          0x0104      Number or client data
//          0x0105      Start Timestamp (1st survey data)
//          0x0106      End Timestamp (last survey data)
//          0x0107      Reference Start TimeStamp (for filename sync)
//          0x0110      FLASH capacity Used. (After the Scan)
//          0x0111      FLASH capacity in %. (After the Scan)
//--------------------------------------------------------Client Data, Error, Report Frame. 
//          0x0030-39   $CDX Client Data $CDX (0 to 9) as long it listed.
//          0x0200-FF   $R Report Frame
//          0x0300-FF   $F Error Frame
//----------------------------------------------------------End of Scan Report
//          
// Step (2) Pop Up Window with all list of data, CONFIG part can be emended but not FLASH data. 
// NOTE: DO NOT ERASE YET!
// NOTE: DOES NOT NECESSARY START TOOLS
// NOTE: Validate Battery State, pop up error if battery low.
// NOTE: Validate Memory capacity, pop up error if full or nearly full.

namespace UDT_Term_FFT
{
    public partial class BG_ReportViewer : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
        //USB_FTDI_Comm myUSBComm;                    // FTDI device manager section (scan, open, close, read, write).
        USB_VCOM_Comm myUSBVCOMComm;
        MainProg myMainProg;
        DMFProtocol myDMFProtocol;
        BGMasterSetup myBGMasterSetup;
        DialogSupport mbOperationBusy;
        public bool isComm_isBusy { get; set; }
        public bool isEnableShowWindow { get; set; }
        //----------------------------------------------------
        //DataTable DGVtable;

        List<string> HeaderColumn = new List<string>(new string[] { "ID", "Type", "String", "F", "UDT Epoch","EESysFlag","EEBattState"});
        List<int> HeaderWidth = new List<int>(new int[] { 60, 200, 400, 30, 80,100,100 });
        List<int> FormatType = new List<int>(new int[] { 1, 2, 2, 2, 0, 1, 1 });        // 0= UINT32, 1 = hex, 2 = string 
        //----------------------------------------------------
        BindingList<BGReportList> blBGReportList = null;
        public BGToCoReportClass ReportClass = null;
        public BGToCoProjectFileManager myProjectFileManager = null;
        //private int DMFPCounter;


        #region//================================================================Reference
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
        public void MyBGProjectFileManager(BGToCoProjectFileManager myBGToCoProjectFileManagerRef)      // MainProg only have one reference for all project!
        {
            myProjectFileManager = myBGToCoProjectFileManagerRef;
        }
        #endregion


        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public BG_ReportViewer()
        {
            InitializeComponent();
            mbOperationBusy = new DialogSupport();
            this.dgvReportView.RowsDefaultCellStyle.BackColor = Color.Bisque;
            this.dgvReportView.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;           //Color.Bisque, Color.Beige
            dgvReportView.DoubleBufferedReportViewerDGV(true);
            ReportClass = new BGToCoReportClass();
            blBGReportList = new BindingList<BGReportList>();
            //-------------------------------------------------Initial state.
            isEnableShowWindow = false;
            isComm_isBusy = false;
        }
        
        //#####################################################################################################
        //###################################################################################### Form Management
        //#####################################################################################################

        #region //================================================================BG_Driller_Load
        private void BG_ReportView_Load(object sender, EventArgs e)
        {

        }
        #endregion

        #region //================================================================BG_Driller_FormClosing
        private void BG_ReportView_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            myGlobalBase.BG_isReportViewerOpen = false;                 // Cease Survey CVS Terminal mode.

            //btnSave.PerformClick();

            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================Report_Show
        public void Report_Show()
        {
            if (isEnableShowWindow == true)
            {
                this.BringToFront();
                this.TopMost = true;
                this.WindowState = FormWindowState.Normal;
                this.Size = new System.Drawing.Size(281, 458);
                myGlobalBase.BG_isReportViewerOpen = true;
                this.Visible = true;
                this.Show();
                //------------------------------------------------------Setup Driller file.
                if (Report_Update_DGV() == false)       // DGV is empty so we close it. 
                    this.Close();
            }
            else
            {
                myGlobalBase.BG_isReportViewerOpen = false;                 // Cease Survey CVS Terminal mode.
                this.Visible = false;
                this.Hide();
            }
        }
        #endregion

        #region //================================================================Report_CloseNow
        public void Report_CloseNow()
        {
            if (myGlobalBase.BG_isReportViewerOpen == false)
                return;
            this.Close();
        }
        #endregion

        #region //================================================================dgvDrillerView_KeyDown
        // https://stackoverflow.com/questions/1718389/right-click-context-menu-for-datagridview
        private void dgvReportView_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.F10 && e.Shift) || e.KeyCode == Keys.Apps)
            {
                e.SuppressKeyPress = true;
                DataGridViewCell currentCell = (sender as DataGridView).CurrentCell;
                if (currentCell != null)
                {
                    ContextMenuStrip cms = currentCell.ContextMenuStrip;
                    if (cms != null)
                    {
                        Rectangle r = currentCell.DataGridView.GetCellDisplayRectangle(currentCell.ColumnIndex, currentCell.RowIndex, false);
                        Point p = new Point(r.X + r.Width, r.Y + r.Height);
                        cms.Show(currentCell.DataGridView, p);
                    }
                }
            }
        }
        #endregion

        #region //================================================================dgvDrillerView_CellMouseDown
        // https://stackoverflow.com/questions/1718389/right-click-context-menu-for-datagridview
        private void dgvReportView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
                if (!c.Selected)
                {
                    c.DataGridView.ClearSelection();
                    c.DataGridView.CurrentCell = c;
                    c.Selected = true;
                }
            }
        }

        #endregion

        //#####################################################################################################
        //###################################################################################### Data Transfer
        //#####################################################################################################

        #region //================================================================Report_ClearData
        public void Report_ClearData()
        {
            dgvReportView.DataSource = null;
            dgvReportView.Refresh();
            ReportClass.ClearData();

        }
        #endregion

        //private int activitycount;                  // When TSTOCO response, it get counted via seperate thread.
        #region //================================================================Report_Download_WithAttempts
        public bool Report_Download_WithAttempts()
        {
            int loopAttempt = 3;
            int counter;
            while (loopAttempt >0)
            {
                loopagain:
                //activitycount = 0;                  
                counter = 0;
                isComm_isBusy = true;
                Report_Download();                  // Implement +REPORT(...) with callback service on separate thread.
                //----------------------------------------------Below operate to monitor activity on Report Download.                  
                while (counter <=100)               //100 * 100mSec = 10Sec.
                {
                    Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
                    //------------------------------------------No response from TOCO then it must be busy scanning LogMem (take 20 second max)
                    if (isComm_isBusy == true)
                    {
                        if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].MessTempRX.Contains("\n"))
                        {
                            //-------------------Step 5: Detected Message and finish
                            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].MessTempRX.Contains("~STCWAIT(0xFF)") == true)
                            {
                                myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].MessTempRX = "";
                                mbOperationBusy.PopUpMessageBox("INFO: Report Frame scan busy, please wait <STCWAIT()>", "Report Frame/Connect", 1, 12F);
                                counter = 0;
                            }
                        }
                        //-----------------------No STCWAIT message after 6 second so something not right, retry. 
                        if (counter>=60)
                        {
                            loopAttempt--;
                            goto loopagain;
                        }
                    }
                    else                             // Global flag that change to false when report frame is processed.
                    {
                        loopAttempt = 0;
                        for (int i = 0; i < ReportClass.ReportData.Count; i++)          // 76K: Fixed Reported Bug
                        {
                            if (ReportClass.ReportData[i].sType.Contains("RD1") == true)
                            {
                                if ((ReportClass.ReportData[i].dtUTC != 0x00000000) & (ReportClass.ReportData[i].dtUTC != 0xFFFFFFFF))
                                {
                                    return true;
                                }
                            }
                        }
                        break;
                    }
                    counter++;
                }
                counter = 0;                   
                loopAttempt--;
            }
            return false;
        }
        #endregion

//                         if (myBGReportViewer.ReportClass.ReportData[i].sType.Contains("RD1") == true)
//                 {
//                     if (Tools.IsString_Double(tbRD1.Text))
//                     {
//                         dtUTC1 = Tools.ConversionStringtoDouble(tbRD1.Text);
//                         RentalUpdate += "0i" + tbRD1.Text + ";";
//                     }
//                     else
//                     {
//                         mbOperationBusy.PopUpMessageBox("ERROR: RD1 text is not valid UDT format", "ToCo Rental Update", 30, 12F);
//                         return;
//                     }
//                     if (dtUTC1< 1523801721)
//                     {
//                         mbOperationBusy.PopUpMessageBox("ERROR: RD1 text is not valid UDT format (must be >1523801721)", "ToCo Rental Update", 30, 12F);
//                         return;
//                     }
//                 }

        #region //================================================================Report_Download
        public delegate void Report_DownloadDelegate();
        public void Report_Download()
        {
            if (this.InvokeRequired)        // Check if we need to call BeginInvoke.
            {
                this.BeginInvoke(new Report_DownloadDelegate(Report_Download));
                return;
            }
            //isComm_isBusy = true;
            Report_ClearData();
            Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ReportDownloadLoopI = new DMFP_Delegate(DMFP_ReportDownloadLoopI_CallBack);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ReportDownloadLoopI, "+REPORT(0x00)",false, (int)eUSBDeviceType.BGDRILLING);
        }
        #endregion

        #region //================================================================DMFP_DownloadLoopI_CallBack  (-REPORT(0xYYYY;string)\n)
        public void DMFP_ReportDownloadLoopI_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if ((hPara[0] == 0xFFFF) || (hPara[0] == 0x00))                     // Check for end of frame or message response issue. 
            {
                if (hPara[0] == 0x00)
                {
                    mbOperationBusy.PopUpMessageBox("ERROR: TOCO Tool Report Transfer Failure, seek support", "TOCO COMM Interface", 10);
                    Report_Show();
                    myMainProg.zBG_Update_Battery_Button();
                    isComm_isBusy = false;
                    return;
                }
                else
                {
                    Report_Show();
                    isComm_isBusy = false;                                      // Finish Report Download Task
                    myGlobalBase.ToCoSerialNumber = ReportClass.SerialNumber;
                    myMainProg.BGToCo_Connected_UpdateToolType();
                    myMainProg.zBG_Update_Battery_Button();
                    //--------------------------------------------------------------------76G: Moved from main.
                    if (SeekReportFrame_STCSTART() != "")
                    {
                        myMainProg.myRtbTermMessageLF("-INFO: <STC_START> Report frame found\n");
                    }
                    else
                    {
                        myMainProg.myRtbTermMessageLF("#ERR: <STC_START> Report frame NOT found!\n");
                    }
                    return;
                }
            }
            else
            {
                if (hPara.Count == 1)
                    ReportClass.ReportDataInsert(sPara, hPara);       // String message
                else
                    ReportClass.ReportDataInsert(sPara, hPara);       // Hex message (all must be 0x type)
            }
            //-----------------------------------------------------------------Next Data to collects
            DMFP_Delegate fpCallBack_ReportDownloadLoopI = new DMFP_Delegate(DMFP_ReportDownloadLoopI_CallBack);
            myDMFProtocol.DMFP_Process_Command(fpCallBack_ReportDownloadLoopI, "+REPORT(0xFF)", false, (int)eUSBDeviceType.BGDRILLING);
            Thread.Sleep(10);
        }
        #endregion

        #region //================================================================SeekReportFrame_STCSTART (take place after connect and report download). 
        public string SeekReportFrame_STCSTART()
        {
            if (ReportClass.bSTCSTART_Button_TimeStampFound == true)
            {
                return (ReportClass.sSTCSTART_Button_TimeStamp);
            }
            return ("");
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### DatGridView Records
        //#####################################################################################################

        #region //================================================================BGDriller_Update_DGV
        private bool Report_Update_DGV()
        {
            if (ReportClass.isEmpty() == true)              // Empty Row, do not update
                return (false);
            try
            {
                //--------------------------------------------Link Data Source to bindinglist. 
                //dgvDrillerView.DataSource = blBGReportList;

                var bindingList = new BindingList<BGReportList>(ReportClass.ReportData);
                var source = new BindingSource(bindingList, null);
                dgvReportView.DataSource = source;

                //var source = new BindingSource(blBGMasterDrillerFile, null);
                //dgvViewRecords.DataSource = source;

                //---------------------------------------------Insert Row number on Row header for reference purpose.
                foreach (DataGridViewRow row in dgvReportView.Rows)
                {
                    row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
                }
                //--------------------------------------------Tidy Up DGV.
                for (int i = 0; i < HeaderColumn.Count; i++)
                {
                    dgvReportView.Columns[i].HeaderText = (HeaderColumn[i]);
                    dgvReportView.Columns[i].Resizable = DataGridViewTriState.True;
                    dgvReportView.Columns[i].Width = HeaderWidth[i];

                    if (FormatType[i]==1)       // 0= UINT32, 1 = hex, 2 = string 
                        dgvReportView.Columns[i].DefaultCellStyle.Format = "X04";

                }
                //dgvViewRecords.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);    // Resize the master DataGridView columns to fit the newly loaded data.
                dgvReportView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvReportView.RowHeadersWidth = 60;
            }
            catch { };

            return (true);

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### File Manager
        //#####################################################################################################

        #region //================================================================btnLoad_Click
        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (ReportCVS_LoadFilename("") == false)
            {
                MessageBox.Show("Error: Unable to load Driller Survey file, check folder!", "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            Report_Update_DGV();
        }
        #endregion

        #region //================================================================btnClose_Click
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();

        }
        #endregion

        #region //================================================================btnSave_Click
        private void btnSave_Click(object sender, EventArgs e)
        {
            //DialogResult dr = MessageBox.Show("Saving Driller File....Okay to proceed?", "File", MessageBoxButtons.YesNo);
            //if (dr == DialogResult.No)
            //{
            //    return;
            //}
            if (ReportCVS_SaveFilename("") == true)
            {
                AutoClosingMessageBox.Show("File Saved!", "File", 1000);
            }
            else
            {
                MessageBox.Show("Error: Unable to Save Driller Survey file", "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        #endregion

        #region //================================================================BtnOpenFolder_Click
        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string myPath = txtFolderName.Text;
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = myPath;
                prc.Start();
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("#ERR: Unable to Open Folder: myPath");
                txtFolderName.Text = "";
            }

        }
        #endregion

        #region //================================================================SurveyCVS_OpenSaveDataCloseFile
        bool ReportCVS_SaveFilename(string sfilenametitle)
        {
            if (sfilenametitle == "")
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "CSV|*.csv";
                saveFileDialog1.Title = "Save an CSV File";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName == "")
                {
                    myMainProg.myRtbTermMessageLF("#ERR: No Filename, canceled operation");
                    return (false);
                }
                sfilenametitle = saveFileDialog1.FileName;
                txtFolderName.Text = sfilenametitle;

            }
            myMainProg.myRtbTermMessageLF("-INFO: Saving Report with Filename: " + sfilenametitle + ".csv");
            try
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(sfilenametitle, true, Encoding.ASCII);
                    string sdata = "";
                    //--------------------------------------------Header Top
                    for (int i = 0; i < HeaderColumn.Count; i++)
                    {
                        sdata += HeaderColumn[i] + ",";
                    }
                    sdata += "\n";
                    sw.Write(sdata);

                    foreach (BGReportList rc in ReportClass.ReportData)
                    {
                        sdata = rc.iNumber.ToString() + "," + rc.sType + "," + rc.sNote + "," + rc.sFormat + "," +  rc.dtUTC.ToString() + "\n";
                        sw.Write(sdata);
                    }
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Dispose();
                    }
                }
                return true;
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("###ERR: Unable to save file, try changing drive/folder and repeat download");
                MessageBox.Show("Unable to save file, try changing drive/folder and repeat download",
                       "Survey CVS filename Error",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Exclamation,
                       MessageBoxDefaultButton.Button1);
            }
            return false;

        }
        #endregion

        #region //================================================================ReportCVS_ReadRawFile
        bool ReportCVS_LoadFilename(string filename)     // if filename is "", it open the load file dialogue for you.
        {
            //string sFilename = ""; ;
            //OpenFileDialog ofdReadSurvey = null;
            //myMainProg.myRtbTermMessageLF("-INFO: Loading Raw/Converted Filename");
            //if (sFrame == null)                            // if Frame is null then init. 
            //    sFrame = new List<string>();
            //else
            //    sFrame.Clear();
            //try
            //{
            //    if (filename != "")                                 // filename
            //    {
            //        sFilename = filename;
            //    }
            //    else                                                // No filename, allow user to select.
            //    {
            //        if (openDialogue == false)                      // Open dialogue is not allowed.
            //        {
            //            return false;
            //        }
            //        ofdReadSurvey = null;
            //        ofdReadSurvey = new OpenFileDialog();
            //        ofdReadSurvey.FileName = myBGMasterSetup.sFile_JobProjectAll;            //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //        ofdReadSurvey.RestoreDirectory = true;
            //        ofdReadSurvey.AutoUpgradeEnabled = false;
            //        ofdReadSurvey.Filter = "csv files (*.csv)|*.csv";

            //        ofdReadSurvey.Title = "Load Survey CVS File";
            //        DialogResult dr = ofdReadSurvey.ShowDialog();
            //        if (dr == DialogResult.OK)
            //        {
            //            sFilename = ofdReadSurvey.FileName;

            //        }
            //        else
            //        {
            //            myRtbTermMessageLF("###ERR: Unable to load file");
            //            return false;
            //        }
            //    }
            //    using (StreamReader reader = File.OpenText(ofdReadSurvey.FileName))
            //    {
            //        Encoding sourceEndocing = reader.CurrentEncoding;
            //        string file = reader.ReadToEnd();
            //        string[] part = file.Split('\n');
            //        sFrame = new List<string>(part);
            //    }
            //}
            //catch
            //{
            //    myRtbTermMessageLF("###ERR: Unable to load file");
            //    return false;
            //}
            return false;
        }
        #endregion




    }
    #region//=============================================================DoubleBuffered property via reflection for datagridview, improve flickerless image.
    public static class ExtensionMethodsBufferedReportViewerDGV
    {
        public static void DoubleBufferedReportViewerDGV(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    #endregion

}

