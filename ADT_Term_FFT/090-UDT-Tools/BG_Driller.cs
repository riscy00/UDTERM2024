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
using System.Windows.Threading;
using System.Globalization;
using System.Diagnostics;

// Power Tool: insert QuickLaunch with "@tasks CollapseRegions" to close all region only. 

namespace UDT_Term_FFT
{
    public partial class BG_Driller : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        System.Windows.Forms.RichTextBox myRtbTerm;
        EE_LogMemSurvey myDownloadSurvey;
        DispatcherTimer dispTimer;

        BGMasterSetup myBGMasterSetup;
        BindingList<BGMasterJobSurvey> blBGMasterJobSurvey=null;                 // Binding List for basic setup.
        BindingList<BGMasterDriller> blBGMasterDrillerFile = null;

        private int caretPosition = 0;
        private bool isDGVactive;
        private bool isIntervalMeter;
        private int iTcounter;
        private int CurrentDepthValue;     //10000 = 1000.0 meter or feet.
        private int SurveyIndex;

        List<string> HeaderColumn = new List<string>(new string[] { "Data", "Time", "UTC", "Length", "Description/Note                      " });
        List<int> HeaderWidth = new List<int>(new int[] { 60, 60, 110, 50, 350 });

        #region//================================================================Reference
        public void MyDownloadSurvey(EE_LogMemSurvey myDownloadSurveyRef)
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

        public void MyRtbTerm(System.Windows.Forms.RichTextBox myrtbTermref)
        {
            myRtbTerm = myrtbTermref;
        }
        #endregion


        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public BG_Driller()
        {
            InitializeComponent();
            dgvViewRecords.DoubleBufferedMessageSurveyCVSX(true);
            isDGVactive = false;
            isIntervalMeter = true;
            this.dgvViewRecords.RowsDefaultCellStyle.BackColor = Color.Bisque;
            this.dgvViewRecords.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;           //Color.Bisque, Color.Beige
            BGDriller_Init_DGV();
        }

        //#####################################################################################################
        //###################################################################################### Form Management
        //#####################################################################################################

        #region //================================================================BG_Driller_Load
        private void BG_Driller_Load(object sender, EventArgs e)
        {
            if (dispTimer==null)
            {
                dispTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
                dispTimer.Interval = TimeSpan.FromSeconds(1);
                dispTimer.Tick += new EventHandler(dispTimerEvent);
            }
        }
        #endregion

        #region //================================================================BG_Driller_FormClosing
        private void BG_Driller_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            myGlobalBase.BG_isDrillerOpen = false;                 // Cease Survey CVS Terminal mode.
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================BG_Driller_Show
        public void BG_Driller_Show()
        {
            this.BringToFront();
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            this.Size = new System.Drawing.Size(281, 458);

            myGlobalBase.BG_isDrillerOpen = true;
            this.Visible = true;
            this.Show();
            //-------------------------------------------------------Load JobSurvey File to fetch 
            if (Load_JobSurveyFile() == false)
            {
                MessageBox.Show("Unable to Load Job Survey File: <..\\..\\JobSurvey.xml>. Go back and setup and save <JobSurvey> first",
                    "Load Tool Setup File",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.Close();
                return;
            }
            isIntervalMeter = blBGMasterJobSurvey[0].bisMeter;
            CurrentDepthValue = blBGMasterJobSurvey[0].iStationInterval;
            string interval = CurrentDepthValue.ToString("D5");
            rtbNote.Text = "";
            interval = interval.Insert(interval.Length - 1, ".");
            if (isIntervalMeter == true)
            {
                cbFeet.Checked = false;
                cbMeter.Checked = true;
                interval += "m";
            }
            else
            {
                cbFeet.Checked = true;
                cbMeter.Checked = false;
                interval += "f";
            }
            this.tbValue.Text = interval;
            ChangeCaretPosition(0);
            //------------------------------------------------------Setup Driller file.
            blBGMasterDrillerFile = null;
            blBGMasterDrillerFile = new BindingList<BGMasterDriller>();
            blBGMasterDrillerFile.Add(new BGMasterDriller());
            Save_DrillerFile();
        }
        #endregion


        // ###TASK
        // (9) Sync datetime to logger survey display and automatically select save selected data. 
        // (10) Extended view includes Note entry window and button for DGV list. 
        //---------------When finish, starting coding into directional data on the LHS screen. 
        //---------------Start coding for simple riscyoffset. 

        //#####################################################################################################
        //###################################################################################### File Manager
        //#####################################################################################################

        #region //================================================================Load_ToolSetupFile
        private bool Load_JobSurveyFile()
        {
            blBGMasterJobSurvey = null;
            string filename = myBGMasterSetup.sFile_JobProjectAll + "\\" + myBGMasterSetup.sFilenameDefaultJobSurvey + ".xml";
            blBGMasterJobSurvey = myGlobalBase.DeserializeFromFile<BindingList<BGMasterJobSurvey>>(filename);
            if (blBGMasterJobSurvey == null)
                return (false);
            return (true);
        }
        #endregion

        #region //================================================================Save_DrillerFile

        private bool Save_DrillerFile()
        {
            string filename = myBGMasterSetup.sFile_JobProjectAll + "\\" + myBGMasterSetup.sFilenameDefaultDrillerSurvey + ".xml";
            myGlobalBase.SerializeToFile<BindingList<BGMasterDriller>>(blBGMasterDrillerFile, filename);

            return (true);
        }
        #endregion

        #region //================================================================Load_DrillerFile
        private bool Load_DrillerFile()
        {
            blBGMasterDrillerFile = null;
            string filename = myBGMasterSetup.sFile_JobProjectAll + "\\" + myBGMasterSetup.sFilenameDefaultDrillerSurvey + ".xml";
            blBGMasterDrillerFile = myGlobalBase.DeserializeFromFile<BindingList<BGMasterDriller>>(filename);
            if (blBGMasterDrillerFile == null)
                return (false);
            return (true);
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### User Control
        //#####################################################################################################

        #region //=============================================================================================Interval Display Section

        #region //================================================================OnClick
        private void OnClick(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button btn = sender as Button;
                if (btn.Equals(btnBGDown))
                    ChangeValue(-1);
                else if (btn.Equals(btnBGLeft))
                    ChangeCaretPosition(-1);
                else if (btn.Equals(btnBGRight))
                    ChangeCaretPosition(1);
                else if (btn.Equals(btnBGUp))
                    ChangeValue(1);
            }
        }
        #endregion

        #region //================================================================ProcessCmdKey
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //capture up arrow key
            if (keyData == Keys.Up)
            {
                ChangeValue(1);
                return true;
            }
            //capture down arrow key
            if (keyData == Keys.Down)
            {
                ChangeValue(-1);
                return true;
            }
            //capture left arrow key
            if (keyData == Keys.Left)
            {
                ChangeCaretPosition(-1);
                return true;
            }
            //capture right arrow key
            if (keyData == Keys.Right)
            {
                ChangeCaretPosition(1);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region //================================================================ChangeCaretPosition
        int newPosition;
        private void ChangeCaretPosition(int value)
        {
            newPosition = caretPosition + value;
            if (newPosition < 0)
                newPosition = 0;
            else if (newPosition >= tbValue.Text.Length - 1)
                newPosition = tbValue.Text.Length - 2;

            if (Char.IsDigit(tbValue.Text, caretPosition))
            {
                // Remove the underline from the previous position
                tbValue.SelectionStart = caretPosition;
                tbValue.SelectionLength = 1;
                //tbValue.SelectionFont = new Font(tbValue.SelectionFont, FontStyle.Bold);
                tbValue.SelectionBackColor = Color.Ivory;
            }
            if (Char.IsDigit(tbValue.Text, newPosition)== false)
                newPosition += value;
            // Ensure that the new position represents a digit
            //while (!Char.IsDigit(tbValue.Text, newPosition))
            //     newPosition += value;

            // Highlight Digit. 
            refreshcaret();
        }
        #endregion

        #region //================================================================ChangeValue
        private void ChangeValue(int value)
        {
            char[] array = tbValue.Text.ToCharArray();
            int code = 0;
            // The caret should always be positioned on a digit, but we will do a sanity check
            if (Char.IsDigit(tbValue.Text, caretPosition))
            {
                for (int i = caretPosition; i >= 0; i--)
                {
                    code = (char)array[i];
                    if (code >= 48 && code <= 57)
                    {
                        if (code + value > 57)
                            array[i] = (char)48;
                        else if (code + value < 48)
                            array[i] = (char)57;
                        else
                        {
                            array[i] = (char)(code + value);
                            break;
                        }
                    }
                }
                string newValue = string.Join("", array);      // .Alternative
                //newValue = new string(array);
                string newValuex = newValue.Replace("m", "");                   // Remove m and f text
                newValuex = newValuex.Replace("f", "");
                newValuex = newValuex.Replace("M", "");
                newValuex = newValuex.Replace("F", "");
                newValuex = newValuex.Substring(0, newValuex.Length - 2);       // remove .0 element.
                int numeric = Tools.ConversionStringtoInt32(newValuex);
                //decimal numeric = Decimal.Parse(newValuex);                   // Do not use this code due to bugs at SL laptop, why is not clear. Even on .NET 4.6.2  !!. 
                if (numeric <=1)
                {
                    if (isIntervalMeter == true)
                        newValue = "0001.0m";
                    else
                        newValue = "0001.0f";
                }
                if (numeric > 9999)
                {
                    if (isIntervalMeter == true)
                        newValue = "9999.0m";
                    else
                        newValue = "9999.0f";
                }
                tbValue.SelectionFont = new Font(tbValue.SelectionFont, FontStyle.Bold);
                tbValue.Text = newValue;
                ReadCurrentDepthValueText();        // Update from CurrentDepthValue
                ChangeCaretPosition(0);
            }
        }
        #endregion
       
        #endregion

        #region //================================================================btnBGSurveyTimeStamp_Click
        private void btnBGSurveyTimeStamp_Click(object sender, EventArgs e)
        {
            Console.Beep(1000, 50);
            btnBGSurveyTimeStamp.BackColor = Color.MistyRose;
            btnBGSurveyTimeStamp.Text = "Wait/Static";
            btnBGSurveyTimeStamp.Refresh();
            DateTime dt = DateTime.Now;
            //----------------------------------------------------Load Driller File
            Load_DrillerFile();
            //----------------------------------------------------Add new array and update index. 
            blBGMasterDrillerFile.Add(new BGMasterDriller());
            SurveyIndex = blBGMasterDrillerFile.Count-1;
            blBGMasterDrillerFile[SurveyIndex].sDate = string.Format("{0:dd/MM/yy}", dt);
            blBGMasterDrillerFile[SurveyIndex].sTime = string.Format("{0:HH/mm/ss}", dt);
            blBGMasterDrillerFile[SurveyIndex].dtUTC = Tools.DateTimeToUnixTimestamp(dt);
            blBGMasterDrillerFile[SurveyIndex].sNote = rtbNote.Text;
            rtbNote.Text = "";      // clear text as done. 
            //---------------------------------------Remove M/m or F/f or non-number, including decimal points
            //string sInt = tbValue.Text;
            
            //Regex digitsOnly = new Regex(@"[^\d]");
            //sInt =  digitsOnly.Replace(sInt, "");
            //--------------------------------------------------------------Feet and Meter, Feet is converted to Meter.
            if (isIntervalMeter == true)            // Meter setting
            {
                blBGMasterDrillerFile[SurveyIndex].iStationInterval = CurrentDepthValue; // Tools.ConversionStringtoInt32(sInt);
            }
            else                                    // Feet setting
            {
                double dInt = Convert.ToDouble((CurrentDepthValue), CultureInfo.CurrentCulture);
                dInt = dInt / 10;
                //dInt =((double)(Tools.ConversionStringtoInt32(sInt))) / 10;
                double result = Tools.FeetToMeters(dInt) * 10;
                blBGMasterDrillerFile[SurveyIndex].iStationInterval = Convert.ToInt32(result);
            }
            //--------------------------------------Now Append to file in XML
            Save_DrillerFile();
            Console.Beep(1000, 50);
            //-------------------------------------Flash timer box.
            Thread.Sleep(2000);
            iTcounter = blBGMasterJobSurvey[0].iWaitPeroidSec-2;
            btnBGSurveyTimeStamp.Font = new System.Drawing.Font("Courier New", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnBGSurveyTimeStamp.Text = (iTcounter.ToString("N0"));
            dispTimer.Start();
            btnBGSurveyTimeStamp.BackColor = Color.Red;
            btnBGSurveyTimeStamp.Refresh();
            Thread.Sleep(50);
            Console.Beep(1000, 20);
            btnBGSurveyTimeStamp.BackColor = Color.MistyRose;
            btnBGSurveyTimeStamp.Refresh();
            Console.Beep(1000, 50);
        }
        #endregion

        #region //================================================================dispTimerEvent
        private void dispTimerEvent(object sender, EventArgs e)
        {
            
            iTcounter--;
            btnBGSurveyTimeStamp.Text = (iTcounter.ToString("N0"));
            if (iTcounter == 0)
            {
                dispTimer.Stop();
                EndOfSurveyPeriod();
            }
            else
            {
                btnBGSurveyTimeStamp.BackColor = Color.Red;
                btnBGSurveyTimeStamp.Refresh();
                Thread.Sleep(50);
                Console.Beep(1000, 20);
                btnBGSurveyTimeStamp.BackColor = Color.MistyRose;
                btnBGSurveyTimeStamp.Refresh();
                Console.Beep(1000, 50);
            }
        }

        #endregion

        #region //================================================================EndOfSurveyPeriod
        private void EndOfSurveyPeriod()
        {
            Console.Beep(1000, 50);
            btnBGSurveyTimeStamp.Text = "Yippee!";
            btnBGSurveyTimeStamp.BackColor = Color.Honeydew;
            btnBGSurveyTimeStamp.Refresh();
            Console.Beep(1000, 750);
            btnBGSurveyTimeStamp.Font = new System.Drawing.Font("Courier New", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnBGSurveyTimeStamp.Text = "Survey";
            //----------------------------------------Update for Next Interval. 
            CurrentDepthValue = CurrentDepthValue + blBGMasterJobSurvey[0].iStationInterval;
            UpdateCurrentDepthValueText();
            //----------------------------------------End
            if (isDGVactive == true)
                BGDriller_Update_DGV();
        }
        #endregion

        #region //================================================================btnBGFinish_Click
        private void btnBGFinish_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            rtbNote.Text = "### FINISH JOB.CLOSED SURVEY ###";      // clear text as done. 
            //----------------------------------------------------Load Driller File
            Load_DrillerFile();
            //----------------------------------------------------Add new array and update index. 
            blBGMasterDrillerFile.Add(new BGMasterDriller());
            SurveyIndex = blBGMasterDrillerFile.Count - 1;
            blBGMasterDrillerFile[SurveyIndex].sDate = string.Format("{0:dd/MM/yy}", dt);
            blBGMasterDrillerFile[SurveyIndex].sTime = string.Format("{0:HH/mm/ss}", dt);
            blBGMasterDrillerFile[SurveyIndex].dtUTC = Tools.DateTimeToUnixTimestamp(dt);
            blBGMasterDrillerFile[SurveyIndex].sNote = rtbNote.Text;
            //---------------------------------------Remove M or F or non-number.
            blBGMasterDrillerFile[SurveyIndex].iStationInterval = CurrentDepthValue;
            //--------------------------------------Now Append to file in XML
            Save_DrillerFile();
            this.Close();
        }
        #endregion

        #region //================================================================cbExtendView_CheckedChanged
        private void cbExtendView_CheckedChanged(object sender, EventArgs e)
        {
            if (cbExtendView.Checked == false)
            {
                this.Size = new System.Drawing.Size(281, 458);
            }
            else
            {
                this.Size = new System.Drawing.Size(281, 562);
            }
        }
        #endregion

        #region //================================================================cbFeet_MouseClick
        private void cbFeet_MouseClick(object sender, MouseEventArgs e)
        {
            cbFeet.Checked = true;
            cbMeter.Checked = false;
            isIntervalMeter = false;
            UpdateCurrentDepthValueText();
            refreshcaret();
            this.Refresh();
        }
        #endregion

        #region //================================================================cbMeter_MouseClick
        private void cbMeter_MouseClick(object sender, MouseEventArgs e)
        {
            cbFeet.Checked = false;
            cbMeter.Checked = true;
            isIntervalMeter = true;
            UpdateCurrentDepthValueText();
            refreshcaret();
            this.Refresh();
        }
        #endregion

        #region //================================================================UpdateCurrentDepthValueText
        private void UpdateCurrentDepthValueText()
        {
            string interval = CurrentDepthValue.ToString("D5");       // Reset Interval. 
            interval = interval.Insert(interval.Length - 1, ".");
            if (isIntervalMeter == true)
                interval += "m";
            else
                interval += "f";
            this.tbValue.Text = interval;
            ChangeCaretPosition(1);
        }
        #endregion

        #region //================================================================ReadCurrentDepthValueText
        private void ReadCurrentDepthValueText()
        {
            try
            {
                Regex digitsOnly = new Regex(@"[^\d]");
                string sInt = digitsOnly.Replace(tbValue.Text, "");
                CurrentDepthValue = Tools.ConversionStringtoInt32(sInt);
                
            }
            catch
            {
                Debug.WriteLine("#E: Driller Survey Text Box is corrupted in ReadCurrentDepthValueText()");
            }
            
        }
        #endregion

        #region //================================================================refreshcaret
        private void refreshcaret()
        {
            //--------------------------------------------Reset Color
            for (int i = 0; i < tbValue.TextLength; i++)
            {
                tbValue.SelectionStart = i;
                tbValue.SelectionBackColor = Color.Ivory;
            }
            //-------------------------------------------Insert color.
            tbValue.SelectionStart = newPosition;
            tbValue.SelectionLength = 1;
            //tbValue.SelectionFont = new Font(tbValue.SelectionFont, FontStyle.Underline | FontStyle.Bold);
            tbValue.SelectionBackColor = Color.LightGreen;
            caretPosition = newPosition;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### DatGridView Records
        //#####################################################################################################

        #region //================================================================dgvViewList_CheckedChanged
        private void dgvViewList_CheckedChanged(object sender, EventArgs e)
        {
            if (dgvViewList.Checked == false)
            {
                this.MaximumSize = new System.Drawing.Size(281, 562);
                this.Size = new System.Drawing.Size(281, 562);
                isDGVactive = false;
            }
            else
            {
                this.MaximumSize = new System.Drawing.Size(1000, 558);
                this.Size = new System.Drawing.Size(1000, 558);
                isDGVactive = true;
                BGDriller_Update_DGV();
            }
        }
        #endregion

        #region //================================================================BGDriller_Init_DGV
        private void BGDriller_Init_DGV()
        {                                

        }
        #endregion

        #region //================================================================BGDriller_Update_DGV
        private void BGDriller_Update_DGV()
        {
            //--------------------------------------------Add row
            if (blBGMasterDrillerFile == null)              // Empty Row, do not update
                return;
            //--------------------------------------------Link Data Source to bindinglist. 
            dgvViewRecords.DataSource = blBGMasterDrillerFile;

            //var source = new BindingSource(blBGMasterDrillerFile, null);
            //dgvViewRecords.DataSource = source;

            //---------------------------------------------Insert Row number on Row header for reference purpose.
            foreach (DataGridViewRow row in dgvViewRecords.Rows)
            {
                row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
            }
            //--------------------------------------------Tidy Up DGV.
            for (int i = 0; i < HeaderColumn.Count; i++)
            {
                dgvViewRecords.Columns[i].HeaderText = (HeaderColumn[i]);
                dgvViewRecords.Columns[i].Resizable = DataGridViewTriState.True;
                dgvViewRecords.Columns[i].Width = HeaderWidth[i];
            }
            //dgvViewRecords.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);    // Resize the master DataGridView columns to fit the newly loaded data.
            dgvViewRecords.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvViewRecords.RowHeadersWidth = 60;

        }


        #endregion


    }

    #region//=============================================================DoubleBuffered property via reflection for datagridview, improve flickerless image.
    public static class ExtensionMethodsMessageDownloadSurveyCVSX
    {
        public static void DoubleBufferedMessageSurveyCVSX(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    #endregion

}
