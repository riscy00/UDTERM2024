using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using UDT_Term;

/// <summary>
/// Inclusion of PEAK PCAN-Basic namespace
/// </summary>
using Peak.Can.Basic;
using TPCANHandle = System.UInt16;
//using TPCANBitrateFD = System.String;
using TPCANTimestampFD = System.UInt64;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
//using MicroLibrary;
using Multimedia;
using JR.Utils.GUI.Forms;
using System.Xml.Serialization;

/* ################################################### TASK List
 * Include information text stream into MsgLog
 * Review how to handle Error 
 * Do search on trace and see what it like.   Also add trace: see ConfigureTraceFile
 */

namespace UDT_Term_FFT
{
    public partial class CanPCAN : Form
    {
        //##############################################################################################################
        //============================================================================================= Common items
        //##############################################################################################################
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        DialogSupport mbDialogMessageBox;
        MainProg myMainProg;
        string sMessageBusStatus;
        CanBus myCanBus;
        IntPtr rtbOEM;

        private const string m_CanPCAN_FilenameDefaultTopProject = "CANProject";
        private const string m_CanPCAN_FilenameDefaultLogMsg = "CANProject\\LogMsg";

        //##############################################################################################################
        //============================================================================================= Reference
        //##############################################################################################################

        #region //============================================================UDT Reference Object
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }

        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
            //--------------------------------------Themes
            switch (myGlobalBase.CompanyName)
            {
                case (int)GlobalBase.eCompanyName.BGDS:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.BG_MainPage_1A;
                        break;
                    }
                case (int)GlobalBase.eCompanyName.ADT:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_1A;
                        break;
                    }
                case ((int)GlobalBase.eCompanyName.TVB):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPageTVB1D;
                        break;
                    }
                default:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_1A;
                        break;
                    }
            }
            this.Refresh();
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= Class.
        //##############################################################################################################

        public PCAN_Configuration myPCAN_Configuration;     //Public Member for Connect Purpose & XML (save/load)

        //##############################################################################################################
        //============================================================================================= TabMasterPCAN Variables.
        //##############################################################################################################
        private System.Windows.Forms.DataGridView[] dataGridArray;
        DataGridViewTextBoxColumn[] CmdColumn;
        DataGridViewCell TabToolCycleTXWindow;
        private string[] TabToolMessage;
        private const int NoOfTabs = 8;
        private string m_sEntryTxt;
        private string sCommandTableFileNamePCAN;
        private List<PCAN_StdFrame> myStdFrame;
        private int EditSelectedRow;

        //##############################################################################################################
        //============================================================================================= myDataGridCycle for TX/CMD Panel
        //##############################################################################################################

        private System.Windows.Forms.DataGridView myDataGridCycle;

        //##############################################################################################################
        //============================================================================================= my DataGridCycle for HeartBeat
        //##############################################################################################################

        private System.Windows.Forms.DataGridView myDGVdHeartBeat;
        private bool isHBSuspend;
        private PCAN_StdFrame PCANHBFrame;
        bool tmCanRXHeartBeatStatus = false;
        bool tmCanHB_TestRunMode = false;
        DataGridViewCell HBDGV_SelectedCellIndex;

        //##############################################################################################################
        //============================================================================================= MultiMedia Timer (precise 1mSec)
        //##############################################################################################################
        // The .NET timer is not very good for <10mSec resolution, so MultiMedia timer was adopted obtained from Peak CAN.
        // The MicroLibrary Timer is capable of uSec resolution but consume 10-20% of MCU resources. The Mutimedia Timer consume much less MCU resource.
        // This work very well for CAN loops application. 

        Multimedia.TimerX tmPCanOnemSec;
        int tm50mSecTick;                   // Update Display
        int tm100mSecTick;                  // Update HeartBeat
        bool tmCanTXCycleStatus = false;
        DataGridViewCell TXDGV_SelectedCellIndex;


        //##############################################################################################################
        //============================================================================================= CAN Loop process (CD(.....))
        //##############################################################################################################
        List<PCAN_StdFrame> myCycleArray;

        //##############################################################################################################
        //============================================================================================= Constructor
        //##############################################################################################################
        public CanPCAN(GlobalBase myGlobalBaseRef)
        {
            rtbOEM = new IntPtr();
            myGlobalBase = myGlobalBaseRef;
            tm50mSecTick = 0;
            //------------------------------------------------PCAN Stuff (to be changed later). 
            InitializeComponent();
            InitializeBasicComponents();
            //------------------------------------------------Configurable Setup, supporting XML Load/Save
            myPCAN_Configuration = new PCAN_Configuration();
            myPCAN_Configuration.Init_ListActiveDisplay();
            myPCAN_Configuration.Init_ListDataBase();
            myPCAN_Configuration.ConfigurationDefault();
            //------------------------------------------------CAN Cycle Section.
            PCAN_CycleInit();
            //------------------------------------------------HeadMaster (Top Panel)
            HeadMaster_Heartbeat_Init();
            //------------------------------------------------TabMaster for command, etc. 8 Tabs. 
            TabMaster_Init();
            tbTabMaster_SetupContextMenu();
            TabToolMessage = new string[2];
            //------------------------------------------------Log Terminal (read only) 
            PCANTerm_ContextMenu_Init();
            rtPCanTerm.Clear();
            //------------------------------------------------Double Buffer.
            rtPCanTerm.DoubleBufferedPCANTerm(true);
            lstMessages.DoubleBufferedPCANListView(true);
            TabMasterPCAN.DoubleBufferedTabMasterPCAN(true);
            myDataGridCycle.DoubleBufferedPCAN_DGV(true);
            myDGVdHeartBeat.DoubleBufferedPCAN_DGV(true);
        }

        //##############################################################################################################
        //============================================================================================= UDT Window Form 
        //##############################################################################################################

        #region //=======================================CanPCAN_Load
        private void CanPCAN_Load(object sender, EventArgs e)
        {
            m_IsFD = false;     // Alway false by force

        }
        #endregion

        #region //=======================================CanPCAN_FormClosing
        private void CanPCAN_FormClosing(object sender, FormClosingEventArgs e)
        {

            //=============================================Cease 1mSec. 
            tmPCanOnemSec.Stop();
            //=============================================
            e.Cancel = true;
            //=============================================Disconnect PCAN. 
            myPCAN_Configuration.ReleaseEnable = false;
            myMainProg.PCAN_PanelLayout_Mode(false);
            PCAN_Release_With_Device();
            //=============================================
            myGlobalBase.CanPCAN_OpenForm = false;                 // Cease Survey CVS Terminal mode.
            this.Visible = false;
            this.Hide();

        }
        #endregion

        #region //=======================================CanPCAN_Show
        public void CanPCAN_Show()
        {
            //###################################################################################################################  Folder Name setup Manager. 
            myGlobalBase.CanPCAN_Config_FolderName = myGlobalBase.sSessionFoldername + "\\" + m_CanPCAN_FilenameDefaultTopProject;
            myGlobalBase.CanPCAN_LogMsg_FolderName = myGlobalBase.sSessionFoldername + "\\" + m_CanPCAN_FilenameDefaultLogMsg;
            myGlobalBase.CanPCAN_TXCycleList_FolderName = myGlobalBase.sSessionFoldername + "\\" + m_CanPCAN_FilenameDefaultTopProject;
            //----------------------------------------------------------------Config
            if (!Directory.Exists(myGlobalBase.CanPCAN_Config_FolderName))
            {
                Directory.CreateDirectory(myGlobalBase.CanPCAN_Config_FolderName);
            }
            //----------------------------------------------------------------Log Msg
            if (!Directory.Exists(myGlobalBase.CanPCAN_LogMsg_FolderName))
            {
                Directory.CreateDirectory(myGlobalBase.CanPCAN_LogMsg_FolderName);
            }
            //----------------------------------------------------------------TX Cycle Command
            if (!Directory.Exists(myGlobalBase.CanPCAN_TXCycleList_FolderName))
            {
                Directory.CreateDirectory(myGlobalBase.CanPCAN_TXCycleList_FolderName);
            }
            //----------------------------------------------------------------Save to config for future use. 
            tsbFolderName.Text = myGlobalBase.CanPCAN_Config_FolderName;
            myPCAN_Configuration.sFolderName = myGlobalBase.CanPCAN_Config_FolderName;
            myPCAN_Configuration.sFolderNameLogMsg = myGlobalBase.CanPCAN_LogMsg_FolderName;
            myPCAN_Configuration.sFolderNameCommandList = myGlobalBase.CanPCAN_TXCycleList_FolderName;
            //###################################################################################################################

            this.BringToFront();
            this.TopMost = false;
            this.WindowState = FormWindowState.Normal;
            myGlobalBase.CanPCAN_OpenForm = true;
            this.Visible = true;
            this.Show();

            //============================================1mSec Tick (Precision), Alway Run on background. 
            TmPCANOnemSec_Init();
            tmCanTXCycleStatus = false;
            tmCanRXHeartBeatStatus = true;
            tmPCanOnemSec.Start();
            //======================
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= External config
        //##############################################################################################################

        #region //=======================================PCAN_Baudrates_IndexChanged
        public void PCAN_Baudrates_IndexChanged(int index)
        {
            myPCAN_Configuration.BaudrateIndex = index;
            switch (index)
            {
                case 0:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_1M;
                    break;
                case 1:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_800K;
                    break;
                case 2:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_500K;
                    break;
                case 3:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_250K;
                    break;
                case 4:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_125K;
                    break;
                case 5:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_100K;
                    break;
                case 6:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_95K;
                    break;
                case 7:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_83K;
                    break;
                case 8:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_50K;
                    break;
                case 9:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_47K;
                    break;
                case 10:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_33K;
                    break;
                case 11:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_20K;
                    break;
                case 12:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_10K;
                    break;
                case 13:
                    myPCAN_Configuration.Baudrate = TPCANBaudrate.PCAN_BAUD_5K;
                    break;
            }
        }
        #endregion

        #region //=======================================PCAN_FilterExt
        public void PCAN_FilterExt(bool checkstate, ref decimal FromValue, ref decimal ToValue)
        {
            int iMaxValue;
            //                          True      : False.
            iMaxValue = (checkstate) ? 0x1FFFFFFF : 0x7FF;

            // We check that the maximum value for a selected filter 
            // mode is used
            //
            if (ToValue > iMaxValue)
                ToValue = iMaxValue;
            //ToValue.Maximum = iMaxValue;
            myPCAN_Configuration.IndexTo = (int)ToValue;
            ToValue = iMaxValue;          // for window object.

            if (FromValue > iMaxValue)
                FromValue = iMaxValue;
            //FromValue.Maximum = iMaxValue;
            myPCAN_Configuration.IndexFrom = (int)FromValue;
            FromValue = iMaxValue;          // for window object.
        }
        #endregion 

        #region //=======================================PCAN_FilterExt_State
        public bool PCAN_FilterExt_State()      // for check box
        {
            if (myPCAN_Configuration.PCANMode == TPCANMode.PCAN_MODE_EXTENDED)
                return (true);          //TPCANMode.PCAN_MODE_EXTENDED
            return (false);             //TPCANMode.PCAN_MODE_STANDARD
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= HeadMaster: HeartBeat
        //##############################################################################################################

        //  tcHeadMaster             // This is header tool boxes
        //  ---> tp1HeartBeat        // Sub-Panel for heartbeat display, using DGV of 3 row, add column (ID, Name and Heartbeat)

        #region //=======================================HeadMaster_Heartbeat_Init
        private void HeadMaster_Heartbeat_Init()
        {
            // tcHeadMaster     // This is header tool boxes
            // tp1HeartBeat     // Sub-Panel.
            isHBSuspend = false;
            myDGVdHeartBeat = new DataGridView();
            PCANHBFrame = new PCAN_StdFrame();
            //-----------------------------------------
            myDGVdHeartBeat = new System.Windows.Forms.DataGridView();
            myDGVdHeartBeat.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            myDGVdHeartBeat.Size = new System.Drawing.Size(827, 69);    // Actual Tab827, 69
            myDGVdHeartBeat.Name = "DGVHB";
            myDGVdHeartBeat.TabIndex = 400;
            myDGVdHeartBeat.BackgroundColor = System.Drawing.Color.White;
            myDGVdHeartBeat.MultiSelect = false;
            myDGVdHeartBeat.ScrollBars = System.Windows.Forms.ScrollBars.None;
            myDGVdHeartBeat.RowHeadersVisible = false;
            myDGVdHeartBeat.AllowUserToAddRows = false;
            myDGVdHeartBeat.AllowUserToDeleteRows = false;
            myDGVdHeartBeat.AllowUserToResizeColumns = false;
            myDGVdHeartBeat.AllowUserToResizeRows = false;
            myDGVdHeartBeat.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            myDGVdHeartBeat.RowTemplate.Height = 22;           //21
            myDGVdHeartBeat.RowTemplate.MinimumHeight = 22;    //21
            myDGVdHeartBeat.BorderStyle = BorderStyle.Fixed3D;
            myDGVdHeartBeat.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            //DataGridView.RowsDefaultCellStyle.SelectionBackColor = System.Drawing.Color.Transparent;
            //myDGVdHeartBeat.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.CanTXCycle_CellContentClick);
            //myDGVdHeartBeat.CurrentCellDirtyStateChanged += new System.EventHandler(this.HB_CurrentCellDirtyStateChanged);
            //myDGVdHeartBeat.CellValueChanged += new DataGridViewCellEventHandler(CanTXCycle_CellValueChanged);
            myDGVdHeartBeat.CellMouseDown += new DataGridViewCellMouseEventHandler(HB_ColumnMouseDown);
            myDGVdHeartBeat.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(HB_ColumnMouseDown);

            //---------------------------------------------------------Button to send CMD once
            List<DataGridViewTextBoxColumn> txCol1 = new List<DataGridViewTextBoxColumn>();
            for (int i = 0; i < 12; i++)
            {
                txCol1.Add(new DataGridViewTextBoxColumn());
                txCol1[i].Width = 60;
                txCol1[i].ReadOnly = true;
                txCol1[i].HeaderText = "CH" + i.ToString();
                txCol1[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                txCol1[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                txCol1[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                myDGVdHeartBeat.Columns.Add(txCol1[i]);

            }
            myDGVdHeartBeat.Rows.Add(string.Empty);
            myDGVdHeartBeat.Rows.Add(string.Empty);
            tp1HeartBeat.Controls.Add(myDGVdHeartBeat);
            tmCanHB_TestRunMode = false;

            HB_SetupContextMenu();
        }
        #endregion

        #region //=======================================HB_ResetAll
        private void HB_ResetAll(bool isDisplayListCleared)
        {
            //-------------------Clear the cells
            for (int i = 0; i < myDGVdHeartBeat.Columns.Count; i++)
            {
                for (int j = 0; j < myDGVdHeartBeat.Rows.Count; j++)
                {
                    myDGVdHeartBeat.Rows[j].Cells[i].Value = string.Empty;
                }
                myDGVdHeartBeat.Columns[i].HeaderText = "CH" + i.ToString();
                myDGVdHeartBeat.Rows[1].Cells[i].Style.BackColor = Color.White;
            }
            if (isDisplayListCleared == true)
                myPCAN_Configuration.lsHeartBeatDisplayed.Clear();      // Remove all HB objects, disposed forever.

            cbHBResetAll.Checked = false;
            cbHBSpare1.Checked = false;
            tmCanHB_TestRunMode = false;
            cbHBSuspend.Text = "Suspend";
            cbHBSuspend.Checked = false;
            isHBSuspend = false;
        }
        #endregion

        #region //=======================================HB_SuspendAll
        private void HB_SuspendAll()
        {
            cbHBSuspend.Text = "Continue";
            isHBSuspend = true;
            tmCanRXHeartBeatStatus = false;
            for (int i = 0; i < myPCAN_Configuration.lsHeartBeatDisplayed.Count; i++)
            {
                PCAN_HeartBeat myHB = myPCAN_Configuration.lsHeartBeatDisplayed[i];
                myHB.BlinkOn = false;
                myHB.CountmSec = 0;
                myHB.CountState = 0;
                myDGVdHeartBeat.Rows[1].Cells[i].Style.BackColor = Color.LightBlue;     // Suspend State.
            }
        }
        #endregion

        #region //=======================================HB_ContinueAll
        private void HB_ContinueAll()
        {
            cbHBSuspend.Text = "Suspend";
            isHBSuspend = false;
            tmCanRXHeartBeatStatus = true;
            for (int i = 0; i < myPCAN_Configuration.lsHeartBeatDisplayed.Count; i++)
            {
                PCAN_HeartBeat myHB = myPCAN_Configuration.lsHeartBeatDisplayed[i];
                myHB.BlinkOn = false;
                myHB.CountmSec = 0;
                myHB.CountState = 1;
                myDGVdHeartBeat.Rows[1].Cells[i].Style.BackColor = Color.White;     // Suspend State.
            }
        }
        #endregion

        #region //=======================================HeadMaster_HB_BeatNow_Existing (when CAN RX received goes to here)
        private delegate void HeadMaster_HB_BeatNow_Existing_StartDelegate(int CanID);
        private void HeadMaster_HB_BeatNow_Existing(int CanID)
        {
            if (this.InvokeRequired)    //===========Create separate thread. 
            {
                this.BeginInvoke(new HeadMaster_HB_BeatNow_Existing_StartDelegate(HeadMaster_HB_BeatNow_Existing), new object[] { CanID });
                return;
            }
            string HBID = "";
            //------------------------------------------------------------------------- Make sure it is HB ID range. 
            if ((CanID < 0x701) | (CanID > 0x77F))
                return;
            myDGVdHeartBeat.SuspendLayout();
            //------------------------------------------------------------------------- Check for existing one on display
            for (int i = 0; i < myPCAN_Configuration.lsHeartBeatDisplayed.Count; i++)
            {
                PCAN_HeartBeat myHB = myPCAN_Configuration.lsHeartBeatDisplayed[i];
                if (Tools.HexStringtoInt32((myHB.CANID)) == (CanID - 0x700))
                {
                    int state = myHB.iCanHeartBeatEvent();
                    HB_SetBlinkColor(i, state, myHB);
                    myDGVdHeartBeat.ResumeLayout();
                    return;
                }
            }
            //------------------------------------------------------------------------- Check for from database list 
            foreach (PCAN_HeartBeat myHB in myPCAN_Configuration.lsHeartBeatDatabase)
            {
                //------------------------------------------
                if (myHB.CANID.Length == 2)
                    HBID = "7" + myHB.CANID;      // Upgrade to 3 digit, ie 20 to 720
                else
                    HBID = myHB.CANID;            // already 3 digit. 
                //------------------------------------------

                if (Tools.HexStringtoInt32(HBID) == CanID)
                {
                    //---------Now we add to column.
                    for (int i = 0; i < myDGVdHeartBeat.Columns.Count; i++)
                    {
                        if (myDGVdHeartBeat.Columns[i].HeaderText.Contains("CH") == true)
                        {
                            myDGVdHeartBeat.Columns[i].HeaderText = HBID;
                            myDGVdHeartBeat.Rows[0].Cells[i].Value = myHB.Name;
                            myDGVdHeartBeat.Rows[1].Cells[i].Value = myHB.Period;
                            myPCAN_Configuration.lsHeartBeatDisplayed.Add(myHB);
                            myHB.BlinkOn = true;
                            myHB.RunCounts();
                            HB_SetBlinkColor(i, 2, myHB);
                            break;
                        }
                    }
                    myDGVdHeartBeat.ResumeLayout();
                    return;
                }
            }
            //-----------------------------------------------------------------Add new HeartBeat Channel. 
            for (int i = 0; i < myDGVdHeartBeat.Columns.Count; i++)
            {
                if (myDGVdHeartBeat.Columns[i].HeaderText.Contains("CH") == true)
                {
                    PCAN_HeartBeat myHBNew = new PCAN_HeartBeat(CanID.ToString("X"), "NEW", 1000);
                    myDGVdHeartBeat.Columns[i].HeaderText = HBID;
                    myDGVdHeartBeat.Rows[0].Cells[i].Value = myHBNew.Name;
                    myDGVdHeartBeat.Rows[1].Cells[i].Value = myHBNew.Period;
                    myHBNew.BlinkOn = true;
                    myHBNew.RunCounts();
                    HB_SetBlinkColor(i, 2, myHBNew);
                    myPCAN_Configuration.lsHeartBeatDisplayed.Add(myHBNew);
                    break;
                }
            }
            myDGVdHeartBeat.ResumeLayout();
        }
        #endregion

        #region //=======================================HB_SetBlinkColor
        private void HB_SetBlinkColor(int Column, int state, PCAN_HeartBeat myHB)
        {
            switch (state)
            {
                case (0):           // Suspend, lightblue (iced)
                    {
                        myDGVdHeartBeat.Rows[1].Cells[Column].Style.BackColor = Color.LightBlue;
                        myHB.BlinkOn = false;
                        break;
                    }
                case (1):           // Busy, White. 
                    {
                        myDGVdHeartBeat.Rows[1].Cells[Column].Style.BackColor = Color.White;
                        myHB.BlinkOn = false;
                        break;
                    }
                case (2):           // On Target, blink Green
                    {
                        myHB.BlinkOn = true;
                        myHB.OffColor = Color.White;
                        myDGVdHeartBeat.Rows[1].Cells[Column].Style.BackColor = Color.Green;
                        break;
                    }
                case (3):           // Below target, blink light yellow then dark yellow.
                    {
                        myHB.BlinkOn = true;
                        myHB.OffColor = Color.Goldenrod;
                        myDGVdHeartBeat.Rows[1].Cells[Column].Style.BackColor = Color.LightYellow;
                        break;
                    }
                case (4):           // Above target, blink light red then dark red. 
                    {
                        myHB.BlinkOn = true;
                        myHB.OffColor = Color.Red;
                        myDGVdHeartBeat.Rows[1].Cells[Column].Style.BackColor = Color.LightPink;
                        break;
                    }
                case (5):           // Post blink revert color. 
                    {
                        myDGVdHeartBeat.Rows[1].Cells[Column].Style.BackColor = myHB.OffColor;
                        break;
                    }
                default:
                    break;

            }
        }
        #endregion

        #region //=======================================HB_UpdateDGV_Displayed_ListOnly
        private void HB_UpdateDGV_Displayed_ListOnly()
        {
            //-------------------Reset and Clear existing
            HB_ResetAll(false);     // Do not clear the display listed object.
            string HBID = "";
            foreach (PCAN_HeartBeat myHB in myPCAN_Configuration.lsHeartBeatDisplayed)
            {
                if (myHB.CANID.Length == 2)
                    HBID = "7" + myHB.CANID;
                else
                    HBID = myHB.CANID;
                //------------------------------
                for (int i = 0; i < myDGVdHeartBeat.Columns.Count; i++)
                {
                    if (myDGVdHeartBeat.Columns[i].HeaderText.Contains("CH") == true)
                    {
                        myDGVdHeartBeat.Columns[i].HeaderText = "0x" + HBID;
                        myDGVdHeartBeat.Rows[0].Cells[i].Value = myHB.Name;
                        myDGVdHeartBeat.Rows[1].Cells[i].Value = myHB.Period;
                        myDGVdHeartBeat.Rows[1].Cells[i].Style.BackColor = Color.White;
                        myPCAN_Configuration.lsHeartBeatDisplayed[i].CountState = 1;        // Start HeartBeat Monitor. 
                        break;
                    }
                }
            }
        }
        #endregion

        #region //=======================================CbHBSpare1_CheckedChanged
        private void CbHBSpare1_CheckedChanged(object sender, EventArgs e)
        {
            //if (cbHBSpare1.Checked == true)
            //{
            //    //-----------------------------------------------------------------------------Clear the cells
            //    for (int i = 0; i < myDGVdHeartBeat.Columns.Count; i++)
            //    {
            //        for (int j = 0; j < myDGVdHeartBeat.Rows.Count; j++)
            //        {
            //            myDGVdHeartBeat.Rows[j].Cells[i].Value = string.Empty;
            //        }
            //        myDGVdHeartBeat.Columns[i].HeaderText = "CH" + i.ToString();
            //    }
            //    myPCAN_Configuration.lsHeartBeatDisplayed.Clear();
            //    //-----------------------------------------------------------------------------Populate database. 
            //    string HBID = "";
            //    foreach (PCAN_HeartBeat myHB in myPCAN_Configuration.lsHeartBeatDatabase)
            //    {
            //        //------------------------------
            //        if (myHB.CANID.Length == 2)
            //            HBID = "7" + myHB.CANID;
            //        else
            //            HBID = myHB.CANID;
            //        //------------------------------
            //        for (int i = 0; i < myDGVdHeartBeat.Columns.Count; i++)
            //        {
            //            if (myDGVdHeartBeat.Columns[i].HeaderText.Contains("CH") == true)
            //            {
            //                myDGVdHeartBeat.Columns[i].HeaderText = "0x" + HBID;
            //                myDGVdHeartBeat.Rows[0].Cells[i].Value = myHB.Name;
            //                myDGVdHeartBeat.Rows[1].Cells[i].Value = myHB.Period;
            //                myDGVdHeartBeat.Rows[1].Cells[i].Style.BackColor = Color.White;
            //                myHB.CANID = HBID;
            //                myPCAN_Configuration.lsHeartBeatDisplayed.Add(myHB);
            //                myPCAN_Configuration.lsHeartBeatDisplayed[i].CountState = 1;        // Start HeartBeat Monitor. 
            //                break;
            //            }
            //        }
            //    }
            //    //---------------------------------------------------------------------------Emulate heartbeat event from the database (one short and one long).
            //    tmCanHB_TestRunMode = true;
            //}
            //else
            //{
            //    tmCanHB_TestRunMode = false;
            //}
        }
        #endregion

        #region //=======================================CbHBResetAll_CheckedChanged
        private void CbHBResetAll_CheckedChanged(object sender, EventArgs e)
        {
            if (cbHBResetAll.Checked)
                HB_ResetAll(true);
        }
        #endregion

        #region //=======================================CbHBSuspend_CheckedChanged
        private void CbHBSuspend_CheckedChanged(object sender, EventArgs e)
        {
            if (cbHBSuspend.Checked)
            {
                HB_SuspendAll();
            }
            else   // Restart
            {
                HB_ContinueAll();
            }
        }
        #endregion

        #region //=======================================HB_TestRun_Tick
        private void HB_TestRun_Tick()
        {
            //TPCANMsg CANMsg = new TPCANMsg();

            //for (int i = 0; i < myPCAN_Configuration.lsHeartBeatDisplayed.Count; i++)
            //{
            //    PCAN_HeartBeat myHB = myPCAN_Configuration.lsHeartBeatDisplayed[i];
            //    if (myHB.CountmSec >= myHB.Period - 1)
            //    {
            //        UInt32 CANID = Tools.AnyStringtoUInt32(myHB.CANID);
            //        CANMsg.LEN = 1;
            //        CANMsg.DATA = new byte[64];
            //        CANMsg.DATA[0] = 0x05;
            //        CANMsg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            //        CANMsg.ID = CANID;
            //        //------------------------------------------------ HeartBeat ID 0x701 to 0x77F only.  
            //        HeadMaster_HB_BeatNow_Existing((int)CANID);
            //        TPCANTimestamp newTimestamp = PCAN_GetTimestamp();
            //        //------------------------------------------------
            //        ProcessMessage(CANMsg, newTimestamp);           // We process the received message
            //    }
            //}
        }
        #endregion

        #region //==================================================HB_SetupContextMenu
        private void HB_SetupContextMenu()
        {
            tp1HeartBeat.ContextMenu = new ContextMenu();
            tp1HeartBeat.ContextMenu.MenuItems.Add("Edit Name", new EventHandler(evHBEditName));
            tp1HeartBeat.ContextMenu.MenuItems.Add("Edit Period", new EventHandler(evHBEditPeriod));
            tp1HeartBeat.ContextMenu.MenuItems.Add("Save Config", new EventHandler(evHBSaveConfig));
            tp1HeartBeat.ContextMenu.MenuItems.Add("Load Config", new EventHandler(evHBLoadConfig));
        }
        #endregion

        #region //==================================================CanTXCycle_ColumnMouseDown
        private void HB_ColumnMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if ((e.RowIndex == -1) & (e.ColumnIndex >= 0))            // Header Part
                    {
                        myDGVdHeartBeat.ClearSelection();
                        //this.myDataGridCycle.Rows[e.RowIndex].Selected = true;
                        this.myDGVdHeartBeat.CurrentCell = this.myDGVdHeartBeat.Rows[0].Cells[e.ColumnIndex];
                        HBDGV_SelectedCellIndex = this.myDGVdHeartBeat.CurrentCell;         //Save this for later code.
                        //-----------------------------------------------------------Working solution to position context box to cell. 
                        System.Drawing.Rectangle cellRect = this.myDGVdHeartBeat.GetCellDisplayRectangle(e.ColumnIndex, 0, true);
                        Point Position = new System.Drawing.Point(cellRect.Left, cellRect.Bottom);
                        myDGVdHeartBeat.ContextMenu.Show(this.myDGVdHeartBeat, Position);
                    }
                    if ((e.RowIndex >= 0) & (e.ColumnIndex >= 0))       // Avoid header. 
                    {
                        myDGVdHeartBeat.ClearSelection();
                        //this.myDataGridCycle.Rows[e.RowIndex].Selected = true;
                        this.myDGVdHeartBeat.CurrentCell = this.myDGVdHeartBeat.Rows[e.RowIndex].Cells[e.ColumnIndex];
                        HBDGV_SelectedCellIndex = this.myDGVdHeartBeat.CurrentCell;         //Save this for later code.
                        //-----------------------------------------------------------Working solution to position context box to cell. 
                        System.Drawing.Rectangle cellRect = this.myDGVdHeartBeat.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                        Point Position = new System.Drawing.Point(cellRect.Left, cellRect.Bottom);
                        myDGVdHeartBeat.ContextMenu.Show(this.myDGVdHeartBeat, Position);
                    }
                }
            }
            catch { }
        }
        #endregion

        #region //==================================================evHBEditName
        private void evHBEditName(object sender, EventArgs e)
        {
            int col = HBDGV_SelectedCellIndex.ColumnIndex;
            int row = 0;
            PopUpEntryBox myPoppy = new PopUpEntryBox();
            myPoppy.PopUpEntry_Setup("Set CAD-ID Name (max 10 letter)", myDGVdHeartBeat.Rows[row].Cells[col].FormattedValue.ToString(), myGlobalBase);
            myPoppy.ShowDialog();                       // Modal Method (which mean parent is not accessible)
            string result = myGlobalBase.PopUpForm_StringValue;

            if (string.IsNullOrEmpty(result))
                return;
            if (result == "##CANCEL##")
                return;
            result = result.Substring(0, Math.Min(result.Length, 10));      // Snip off excess length. 
            myDGVdHeartBeat.Rows[row].Cells[col].Value = result;
            myPCAN_Configuration.lsHeartBeatDisplayed[col].Name = result;
            myPoppy.Dispose();
        }
        #endregion

        #region //==================================================evHBEditPeriod
        private void evHBEditPeriod(object sender, EventArgs e)
        {
            int col = HBDGV_SelectedCellIndex.ColumnIndex;
            int row = 1;
            PopUpEntryBox myPoppy = new PopUpEntryBox();

            myPoppy.PopUpEntry_Setup("Set HeartBeat Period (mSec)", myDGVdHeartBeat.Rows[row].Cells[col].FormattedValue.ToString(), myGlobalBase);
            myPoppy.ShowDialog();                       // Modal Method (which mean parent is not accessible)
            string result = myGlobalBase.PopUpForm_StringValue;

            if (string.IsNullOrEmpty(result))
                return;
            if (result == "##CANCEL##")
                return;
            if (Tools.IsString_Numberic_UInt32(result) == false)
                return;
            int Period = Tools.AnyStringtoInt32(result);
            if (Period < 500)
            {
                if (mbDialogMessageBox == null)
                    mbDialogMessageBox = new DialogSupport();
                mbDialogMessageBox.PopUpMessageBox("ERROR: Period Too Short, must be greater than 500mSec", "HeartBeat Entry Error", 5, 12F);
                return;
            }
            myDGVdHeartBeat.Rows[row].Cells[col].Value = result;
            myPCAN_Configuration.lsHeartBeatDisplayed[col].Period = Period;
            myPCAN_Configuration.lsHeartBeatDisplayed[col].UpdatePeriodMinMax();
            myPoppy.Dispose();
        }
        #endregion

        #region //==================================================evHBLoadConfig
        private void evHBLoadConfig(object sender, EventArgs e)
        {
            HB_SuspendAll();
            PCAN_LoadConfigToFile();
            HB_UpdateDGV_Displayed_ListOnly();
            HB_ContinueAll();
        }
        #endregion

        #region //==================================================evHBSaveConfig
        private void evHBSaveConfig(object sender, EventArgs e)
        {
            HB_SuspendAll();
            PCAN_SaveConfigToFile();
            HB_ContinueAll();
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= TabMaster Init
        //##############################################################################################################

        #region //=======================================PCAN_Baudrates_IndexChanged
        public void TabMaster_Init()
        {
            #region //================================================TabMaster: Prelim Setup Variables

            sCommandTableFileNamePCAN = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            this.dataGridArray = new System.Windows.Forms.DataGridView[NoOfTabs];

            byte[] test = new byte[NoOfTabs];


            for (int i = 0; i < NoOfTabs; i++)
            {
                this.dataGridArray[i] = new System.Windows.Forms.DataGridView();
            }

            this.TabMasterPCAN.SuspendLayout();
            this.TabTool1.SuspendLayout();
            for (int i = 0; i < NoOfTabs; i++)
            {
                ((System.ComponentModel.ISupportInitialize)(this.dataGridArray[i])).BeginInit();
            }

            CmdColumn = new DataGridViewTextBoxColumn[3];

            this.TabTool1.Controls.Add(this.dataGridArray[0]);
            this.TabTool2.Controls.Add(this.dataGridArray[1]);
            this.TabTool3.Controls.Add(this.dataGridArray[2]);
            this.TabTool4.Controls.Add(this.dataGridArray[3]);
            this.TabTool5.Controls.Add(this.dataGridArray[4]);
            this.TabTool6.Controls.Add(this.dataGridArray[5]);
            this.TabTool7.Controls.Add(this.dataGridArray[6]);
            this.TabTool8.Controls.Add(this.dataGridArray[7]);

            #endregion

            #region //================================================TabMaster: Baseline DataGridView
            // 
            // dataGridView1
            // 
            for (int i = 0; i < NoOfTabs; i++)
            {
                // Added RGP 06/10/15------------------------------------------------ This fix row height. 
                this.dataGridArray[i].AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                this.dataGridArray[i].RowTemplate.Height = 21;
                this.dataGridArray[i].RowTemplate.MinimumHeight = 21;
                //this.dataGridArray[i].ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                //--------------------------------------------------------------------
                this.dataGridArray[i].Location = new System.Drawing.Point(0, 0);
                this.dataGridArray[i].Size = new System.Drawing.Size(378, 487);
                this.dataGridArray[i].TabIndex = 0;
                this.dataGridArray[i].AllowUserToAddRows = false;
                this.dataGridArray[i].AllowUserToDeleteRows = false;
                this.dataGridArray[i].AllowUserToResizeColumns = false;
                this.dataGridArray[i].AllowUserToResizeRows = false;
                this.dataGridArray[i].BackgroundColor = System.Drawing.Color.Gainsboro;
                this.dataGridArray[i].MultiSelect = false;
                this.dataGridArray[i].Name = "CmdBox" + i.ToString();
                this.dataGridArray[i].RowHeadersVisible = false;
                this.dataGridArray[i].ScrollBars = System.Windows.Forms.ScrollBars.None;
                this.dataGridArray[i].CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.CmdBox_CellContentClick);

            }
            #endregion

            #region //================================================TabMaster DataGridView1 (Request) Column Definition
            for (int j = 0; j < NoOfTabs; j++)
            {
                DataGridViewButtonColumn btnRequestPCAN = new DataGridViewButtonColumn();
                btnRequestPCAN.Name = "btnCommand";
                btnRequestPCAN.HeaderText = "Process";
                btnRequestPCAN.Width = 50;
                btnRequestPCAN.Resizable = DataGridViewTriState.False;
                btnRequestPCAN.SortMode = DataGridViewColumnSortMode.NotSortable;
                btnRequestPCAN.ReadOnly = true;
                dataGridArray[j].Columns.Add(btnRequestPCAN);
                dataGridArray[j].CellMouseDown += new DataGridViewCellMouseEventHandler(TabTools_ColumnMouseDown);
                for (int i = 0; i < 2; i++)
                {
                    CmdColumn[i] = new DataGridViewTextBoxColumn();
                    CmdColumn[i].Name = "txt" + myCommndColumnFormatPCAN[i].Label;
                    CmdColumn[i].HeaderText = myCommndColumnFormatPCAN[i].Label;
                    CmdColumn[i].Width = myCommndColumnFormatPCAN[i].Width;
                    CmdColumn[i].Resizable = DataGridViewTriState.False;
                    CmdColumn[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridArray[j].Columns.Add(CmdColumn[i]);

                }
            }

            for (int j = 0; j < NoOfTabs; j++)
            {
                for (int i = 0; i < 20; i++)
                {
                    this.dataGridArray[j].Rows.Add(new string[] { (i + (j * 20)).ToString("D2") });
                    //dataGridArray[j].DoubleBufferedPCAN_DGV(true);
                }
            }

            this.TabMasterPCAN.ResumeLayout(false);
            this.TabTool1.ResumeLayout(false);

            for (int i = 0; i < NoOfTabs; i++)
            {
                ((System.ComponentModel.ISupportInitialize)(this.dataGridArray[i])).EndInit();
            }
            this.TabMasterPCAN.SelectedTab = TabTool1;

            #endregion
        }

        #endregion

        #region //=======================================tbTabMaster_SetupContextMenu
        private void tbTabMaster_SetupContextMenu()
        {
            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Copy to 'TX/Cycle' Panel", new EventHandler(TabTools_CopyToTXCycle_TabBox));
            cm.MenuItems.Add("Rename Tab", new EventHandler(TabTools_RenameTabPageTextPCAN));
            cm.MenuItems.Add("Save", new EventHandler(TabTools_SaveTabMasterPCAN));
            cm.MenuItems.Add("Load", new EventHandler(TabTools_LoadTabMasterPCAN));
            TabTool1.ContextMenu = cm;
            TabTool2.ContextMenu = cm;
            TabTool3.ContextMenu = cm;
            TabTool4.ContextMenu = cm;
            TabTool5.ContextMenu = cm;
            TabTool6.ContextMenu = cm;
            TabTool7.ContextMenu = cm;
            TabTool8.ContextMenu = cm;
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= TabMaster PCAN Write Event
        //##############################################################################################################

        #region //=======================================CanTXCycle_ColumnMouseDown
        private void TabTools_ColumnMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            int cc = 0;
            if (this.TabMasterPCAN.SelectedTab == TabTool2) cc = 1;
            if (this.TabMasterPCAN.SelectedTab == TabTool3) cc = 2;
            if (this.TabMasterPCAN.SelectedTab == TabTool4) cc = 3;
            if (this.TabMasterPCAN.SelectedTab == TabTool5) cc = 4;
            if (this.TabMasterPCAN.SelectedTab == TabTool6) cc = 5;
            if (this.TabMasterPCAN.SelectedTab == TabTool7) cc = 6;
            if (this.TabMasterPCAN.SelectedTab == TabTool8) cc = 7;
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if ((e.RowIndex >= 0) & (e.ColumnIndex >= 0))       // Avoid header. 
                    {
                        dataGridArray[cc].ClearSelection();
                        //this.myDataGridCycle.Rows[e.RowIndex].Selected = true;
                        dataGridArray[cc].CurrentCell = dataGridArray[cc].Rows[e.RowIndex].Cells[e.ColumnIndex];
                        TabToolCycleTXWindow = dataGridArray[cc].CurrentCell;         //Save this for later code.
                        TabToolMessage[0] = dataGridArray[cc].Rows[e.RowIndex].Cells[1].Value.ToString();
                        TabToolMessage[1] = dataGridArray[cc].Rows[e.RowIndex].Cells[2].Value.ToString();
                        //-----------------------------------------------------------Working solution to position context box to cell. 
                        System.Drawing.Rectangle cellRect = dataGridArray[cc].GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                        Point Position = new System.Drawing.Point(cellRect.Left, cellRect.Bottom);
                        dataGridArray[cc].ContextMenu.Show(dataGridArray[cc], Position);
                    }
                }
            }
            catch { }
        }
        #endregion

        #region//=======================================CmdBox_CellContentClick
        private void CmdBox_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int column = e.ColumnIndex;
            if (column != 0) return;       // Only button is clickable!
            if (row < 0) return;
            //-----------------------
            string CommandString;
            string CommentString;
            int selectedtab = 0;
            int btn;
            if (this.TabMasterPCAN.SelectedTab == TabTool2) selectedtab = 1;
            if (this.TabMasterPCAN.SelectedTab == TabTool3) selectedtab = 2;
            if (this.TabMasterPCAN.SelectedTab == TabTool4) selectedtab = 3;
            if (this.TabMasterPCAN.SelectedTab == TabTool5) selectedtab = 4;
            if (this.TabMasterPCAN.SelectedTab == TabTool6) selectedtab = 5;
            if (this.TabMasterPCAN.SelectedTab == TabTool7) selectedtab = 6;
            if (this.TabMasterPCAN.SelectedTab == TabTool8) selectedtab = 7;

            if (dataGridArray[selectedtab].Rows[row].Cells[1].Value != null)        // Filter out null commands.
            {
                CommandString = dataGridArray[selectedtab].Rows[row].Cells[1].Value.ToString();
                CommentString = dataGridArray[selectedtab].Rows[row].Cells[2].Value.ToString();
                btn = row + (selectedtab * 20);
                //-------------------------------------------
                myPCANTermMessage("#BTN-" + btn.ToString() + ":" + CommandString + "::" + CommentString + "\r\n");
                m_sEntryTxt = CommandString;
                PCAN_CommandProcessor(CommandString);
            }
            TabMasterPCAN.Refresh();
        }
        #endregion

        #region//=======================================PCAN_CommandProcessor
        private void PCAN_CommandProcessor(string CommandString)
        {
            char[] delimiterChars = { ',', ';' };
            string[] delimiterString = new string[] { ";|" };
            string RecievedData = CommandString;
            string[] sDetokenParameter;
            string[] sFrame;
            //-------------------------------------------------------------------------------------------------------------
            if (myStdFrame == null)
                myStdFrame = new List<PCAN_StdFrame>();
            else
                myStdFrame.Clear();
            //---------------------------------------------------Detokeniser
            RecievedData = RecievedData.Replace(")", "");
            RecievedData = RecievedData.Replace(" ", "");
            PCAN_StdFrame CanMsg = new PCAN_StdFrame();
            //-------------------------------------------------------------------------------------------------------------
            //--------------CS(ID;DLC;D0;D1;D2;D3;D4;D5;D6;D7) 
            //--------------CS(ID;DLC;D0D1D2D3D4D5D6D7)
            if ((CommandString.StartsWith("CS(", StringComparison.Ordinal) == true))     //Format Section
            {
                CanMsg.Clear();
                try
                {
                    RecievedData = RecievedData.Replace("CS(", "");
                    sDetokenParameter = RecievedData.Split(delimiterChars);
                    CanMsg.CanID = Tools.AnyStringtoUInt32(sDetokenParameter[0]);
                    CanMsg.DLC = Tools.StrToHexBytesNonASCIIOneByte(sDetokenParameter[1]);

                    if (sDetokenParameter[2].Length > 2)                    // This is continuous D0D1D2D3D4D5D6D7 style. 
                    {
                        CanMsg.DATA = Tools.StrToHexBytesNonASCII(sDetokenParameter[2]);
                    }
                    else                                                    // This is discrete D0;D1;D2;D3;D4;D5;D;6D7 style, based on DLC.
                    {
                        for (int i = 0; i < CanMsg.DLC; i++)
                        {
                            CanMsg.DATA[i] = Tools.StrToHexBytesNonASCIIOneByte(sDetokenParameter[2 + i]);
                        }
                    }
                    myStdFrame.Add(CanMsg);
                    PCAN_WriteFrame(CanMsg, TPCANMessageType.PCAN_MESSAGE_STANDARD);
                }
                catch
                {
                    PCAN_InsertMessage("#E:Invalid CS() Command, check Typo: CS(ID;DLC;D0;D1;D2;D3;D4;D5;D6;D7)");
                    return;
                }
            }
            //--------------CD(ID; DLC; D0..D7; | Delay; ID; DLC; D0..D7; | Delay; ID; DLC; D0..D7, etc) 
            //--------------CD(|Delay; ID; DLC; D0..D7; | Delay; ID; DLC; D0..D7; | Delay; ID; DLC; D0..D7, etc), Max 8. 
            //------Example CD(620; 8; 2B204005DC050000;| 5500; 1C1; 5; 0000010000)
            //------Example CD(| 0;620; 8; 2B204005DC050000;| 5500; 1C1; 5; 0000010000)

            else if ((CommandString.StartsWith("CD(", StringComparison.Ordinal) == true))     //Format Section
            {
                try
                {
                    int index = 0;
                    RecievedData = RecievedData.Replace("CD(", "");
                    if (RecievedData.Contains("|"))
                    {
                        //sFrame = RecievedData.Split(delimiterString, StringSplitOptions.RemoveEmptyEntries);
                        sFrame = Tools.SplitAndKeepDelimitersX(RecievedData, '|');
                        if (sFrame == null)
                        {
                            PCAN_InsertMessage("#E:Invalid Command syntax, unable to decode");
                            return;
                        }
                    }
                    else
                    {
                        sFrame = new string[1];
                        sFrame[0] = RecievedData;
                    }
                    foreach (string sRecievedData in sFrame)
                    {
                        index = 0;
                        sDetokenParameter = sRecievedData.Split(delimiterChars);
                        CanMsg.Clear();
                        if (sDetokenParameter[index].Contains("|"))
                        {
                            string ddd = sDetokenParameter[0].Replace("|", "");
                            CanMsg.Delay = Tools.AnyStringtoInt32(ddd);
                            index++;
                        }
                        else
                        {
                            CanMsg.Delay = 0;
                        }
                        CanMsg.CanID = Tools.AnyStringtoUInt32(sDetokenParameter[index]);
                        index++;
                        CanMsg.DLC = CanMsg.DLC = Tools.StrToHexBytesNonASCIIOneByte(sDetokenParameter[index]);
                        index++;

                        if (sDetokenParameter[index].Length > 2)                      // This is continuous D0D1D2D3D4D5D6D7 style. 
                        {
                            CanMsg.DATA = Tools.StrToHexBytesNonASCII(sDetokenParameter[index]);
                            index++;
                        }
                        else                                                    // This is discrete D0;D1;D2;D3;D4;D5;D6;D7 style, based on DLC.
                        {
                            for (int i = index; i < (index + CanMsg.DLC); i++)
                            {
                                CanMsg.DATA[i] = Tools.StrToHexBytesNonASCIIOneByte(sDetokenParameter[2 + i]);
                            }
                            index += CanMsg.DLC;
                        }
                        myStdFrame.Add(CanMsg);
                        PCAN_Cycle_Add_List(CanMsg);
                    }
                }
                catch
                {
                    PCAN_InsertMessage("#E:Invalid CD() Command, check Typo: See Help for details");
                    return;
                }
                //----------------------------------------------------------------------
                myGlobalBase.isCanPCANTermScreenHalted = true;      //Suspend terminal to limit speed impacts.
                foreach (PCAN_StdFrame WriteFrame in myStdFrame)
                {
                    udelay(WriteFrame.Delay);
                    PCAN_WriteFrame(WriteFrame, TPCANMessageType.PCAN_MESSAGE_STANDARD);
                }
                myGlobalBase.isCanPCANTermScreenHalted = false;     //Continue terminal. 
            }
            // CL(Delay;CyclePeriod;NoOfLoop;ID;DLC;D0;D1;D2;D3;D4;D5;D6;D7) \r\n");
            else if ((CommandString.StartsWith("CL(", StringComparison.Ordinal) == true))     //Format Section
            {
                CanMsg.Clear();
                try
                {
                    RecievedData = RecievedData.Replace("|", "");               // We human :
                    RecievedData = RecievedData.Replace("CL(", "");
                    sDetokenParameter = RecievedData.Split(delimiterChars);
                    CanMsg.Delay = Tools.AnyStringtoInt32(sDetokenParameter[0]);
                    CanMsg.CyclePeriod = Tools.AnyStringtoInt32(sDetokenParameter[1]);
                    CanMsg.CycleNumber = Tools.AnyStringtoInt32(sDetokenParameter[2]);
                    CanMsg.CanID = Tools.AnyStringtoUInt32(sDetokenParameter[3]);      //Tools
                    CanMsg.DLC = Tools.StrToHexBytesNonASCIIOneByte(sDetokenParameter[4]);
                    if (sDetokenParameter[5].Length > 2)                    // This is continuous D0D1D2D3D4D5D6D7 style. 
                    {
                        CanMsg.DATA = Tools.StrToHexBytesNonASCII(sDetokenParameter[5]);
                    }
                    else                                                    // This is discrete D0;D1;D2;D3;D4;D5;D;6D7 style, based on DLC.
                    {
                        for (int i = 0; i < CanMsg.DLC; i++)
                        {
                            CanMsg.DATA[i] = Tools.StrToHexBytesNonASCIIOneByte(sDetokenParameter[5 + i]);
                        }
                    }
                    myStdFrame.Add(CanMsg);
                    PCAN_Cycle_Add_List(CanMsg);
                }
                catch
                {
                    PCAN_InsertMessage("#E:Invalid CL() Command, check Typo: CL(Delay;CyclePeriod;NoOfLoop;ID;DLC;D0;D1;D2;D3;D4;D5;D6;D7) ");
                    return;
                }
                //----------------------------------------------------------------------### We Need 1mSec Update clock and process the looped CAN frame
                myGlobalBase.isCanPCANTermScreenHalted = true;      //Suspend terminal to limit speed impacts.
                foreach (PCAN_StdFrame WriteFrame in myStdFrame)
                {
                    udelay(WriteFrame.Delay);
                    PCAN_WriteFrame(WriteFrame, TPCANMessageType.PCAN_MESSAGE_STANDARD);
                }
                myGlobalBase.isCanPCANTermScreenHalted = false;     //Continue terminal. 
            }
            else
            {
                PCAN_InsertMessage("#E: Invalid Command, check Typo");
            }
        }
        #endregion

        #region//=======================================PCAN_WriteFrame
        private void PCAN_WriteFrame(PCAN_StdFrame myFrame, TPCANMessageType CanType)
        {
            TPCANMsg CANMsg;
            MessageStatus msgStsCurrentMsg = new MessageStatus();
            CANMsg.DATA = myFrame.DATA;
            CANMsg.LEN = myFrame.DLC;
            CANMsg.ID = myFrame.CanID;
            CANMsg.MSGTYPE = CanType;
            TPCANStatus Result1 = TPCANStatus.PCAN_ERROR_OK;
            try
            {
                Result1 = PCANBasic.Write(m_PcanHandle, ref CANMsg);
            }
            catch
            {
                PCAN_InsertMessage("#E: Unable to write to PCAN device.");
            }
            TPCANTimestampFD TxTimestampFD = PCAN_GetTimestamp_uSec();

            myPCANTermMessage(msgStsCurrentMsg.PCAN_GenerateLogMessageTX(CANMsg, Result1, TxTimestampFD));
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= TabMaster PCAN Save/Load File. 
        //##############################################################################################################

        #region//=======================================TabTools_LoadTabMasterPCAN
        private void TabTools_LoadTabMasterPCAN(object sender, EventArgs e)
        {
            TabTools_Export_Filename();
        }
        #endregion

        #region//=======================================TabTools_CopyToTXCycle_TabBox
        private void TabTools_CopyToTXCycle_TabBox(object sender, EventArgs e)
        {
            char[] delimiterChars = { ',', ';' };
            string[] delimiterString = new string[] { ";|" };
            string RecievedData = TabToolMessage[0];
            string[] sDetokenParameter;
            //---------------------------------------------------Detokeniser
            RecievedData = RecievedData.Replace(")", "");
            RecievedData = RecievedData.Replace(" ", "");
            PCAN_StdFrame CanMsg = new PCAN_StdFrame();

            if ((TabToolMessage[0].StartsWith("CS(", StringComparison.Ordinal) == true))     //Format Section
            {
                try
                {
                    CanMsg.Clear();
                    RecievedData = RecievedData.Replace("CS(", "");
                    sDetokenParameter = RecievedData.Split(delimiterChars);
                    CanMsg.CanID = Tools.AnyStringtoUInt32(sDetokenParameter[0]);
                    CanMsg.DLC = Tools.StrToHexBytesNonASCIIOneByte(sDetokenParameter[1]);
                    CanMsg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                    if (sDetokenParameter[2].Length > 2)                    // This is continuous D0D1D2D3D4D5D6D7 style. 
                    {
                        CanMsg.DATA = Tools.StrToHexBytesNonASCII(sDetokenParameter[2]);
                    }
                    else                                                    // This is discrete D0;D1;D2;D3;D4;D5;D;6D7 style, based on DLC.
                    {
                        for (int i = 0; i < CanMsg.DLC; i++)
                        {
                            CanMsg.DATA[i] = Tools.StrToHexBytesNonASCIIOneByte(sDetokenParameter[2 + i]);
                        }
                    }
                    CanMsg.CycleNumber = 0;          // 10 loop
                    CanMsg.CyclePeriod = 0;          // 0Sec.
                    CanMsg.Delay = 0;                // No Delay. 
                    CanMsg.sComment = TabToolMessage[1];
                    PCAN_Cycle_Add_List(CanMsg);
                }
                catch
                {
                    PCAN_InsertMessage("#E:Invalid CS() Command, check Typo: CS(ID;DLC;D0;D1;D2;D3;D4;D5;D6;D7)");
                    return;
                }

            }
            else if ((TabToolMessage[0].StartsWith("CL(", StringComparison.Ordinal) == true))     //Format Section
            {

                CanMsg.Clear();
                try
                {
                    RecievedData = RecievedData.Replace("|", "");               // We human :
                    RecievedData = RecievedData.Replace("CL(", "");
                    sDetokenParameter = RecievedData.Split(delimiterChars);
                    CanMsg.Delay = Tools.AnyStringtoInt32(sDetokenParameter[0]);
                    CanMsg.CyclePeriod = Tools.AnyStringtoInt32(sDetokenParameter[1]);
                    CanMsg.CycleNumber = Tools.AnyStringtoInt32(sDetokenParameter[2]);
                    CanMsg.CanID = Tools.AnyStringtoUInt32(sDetokenParameter[3]);      //Tools
                    CanMsg.DLC = Tools.StrToHexBytesNonASCIIOneByte(sDetokenParameter[4]);
                    if (sDetokenParameter[5].Length > 2)                    // This is continuous D0D1D2D3D4D5D6D7 style. 
                    {
                        CanMsg.DATA = Tools.StrToHexBytesNonASCII(sDetokenParameter[5]);
                    }
                    else                                                    // This is discrete D0;D1;D2;D3;D4;D5;D;6D7 style, based on DLC.
                    {
                        for (int i = 0; i < CanMsg.DLC; i++)
                        {
                            CanMsg.DATA[i] = Tools.StrToHexBytesNonASCIIOneByte(sDetokenParameter[5 + i]);
                        }
                    }
                    PCAN_Cycle_Add_List(CanMsg);
                }
                catch
                {
                    PCAN_InsertMessage("#E:Invalid CL() Command, check Typo: CL(Delay;CyclePeriod;NoOfLoop;ID;DLC;D0;D1;D2;D3;D4;D5;D6;D7) ");
                    return;
                }
            }
            else
            {
                PCAN_InsertMessage("#E: CD(..) is not supported, otherwise check typo");
            }
        }
        #endregion

        #region//=======================================TabTools_SaveTabMasterPCAN
        private void TabTools_SaveTabMasterPCAN(object sender, EventArgs e)
        {
            TabTools_Import_Filename();
        }
        #endregion

        #region//=======================================TabTools_RenameTabPageTextPCAN
        private void TabTools_RenameTabPageTextPCAN(object sender, EventArgs e)
        {
            var stabshowdialog = this.TabShowDialog(TabMasterPCAN.SelectedTab.Text);
            TabMasterPCAN.SelectedTab.Text = stabshowdialog;
        }

        public string TabShowDialog(string TabText)
        {
            string backup = TabText;
            bool confirmchange = false;
#pragma warning disable IDE0017 // Simplify object initialization
            Form prompt = new Form();
#pragma warning restore IDE0017 // Simplify object initialization
            prompt.MinimizeBox = false;
            prompt.MaximizeBox = false;
            prompt.StartPosition = FormStartPosition.CenterParent;

            prompt.Width = 100;
            prompt.Height = 100;
            prompt.MaximumSize = new System.Drawing.Size(prompt.Width, prompt.Height);
            prompt.MinimumSize = new System.Drawing.Size(prompt.Width, prompt.Height);

            prompt.Text = "Rename Tab";
            Label textLabel = new Label() { Left = 10, Top = 10, Text = "Rename" };
            TextBox textBox = new TextBox() { Left = 10, Top = 35, Width = 100, Text = TabText, MaxLength = 6 };
            textBox.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    confirmchange = true;
                    prompt.Close();
                }
            };
            Button confirmation = new Button() { Text = "Ok", Left = 70, Width = 40, Top = 5 };
            confirmation.Click += (sender, e) =>
            {
                confirmchange = true;
                prompt.Close();
            };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.ShowDialog();
            if (confirmchange == false)
                return backup;
            return textBox.Text;
        }

        #endregion

        #region//=======================================btnImport_Click
        private void btnImport_Click(object sender, EventArgs e)
        {
            TabTools_Import_Filename();
        }
        #endregion

        #region//=======================================btnExport_Click
        private void btnExport_Click(object sender, EventArgs e)
        {
            TabTools_Export_Filename();
        }
        #endregion

        #region//=======================================TabTools_Export_Filename
        private void TabTools_Export_Filename()
        {
            TabMasterPCAN.Refresh();

            string strRowValue;

#pragma warning disable IDE0017 // Simplify object initialization
            SaveFileDialog sfd_SaveLinData = new SaveFileDialog();
#pragma warning restore IDE0017 // Simplify object initialization
            sfd_SaveLinData.Filter = "csv files (*.csv)|*.csv";
            sfd_SaveLinData.FileName = sCommandTableFileNamePCAN;
            sfd_SaveLinData.Title = "Export PCAN to CVS File, Alway Add 'PCAN' Prefix";

            StringBuilder sb = new StringBuilder();
            for (int v = 0; v < NoOfTabs; v++)
            {
                if (v == 0) sb.AppendLine(TabTool1.Text);
                if (v == 1) sb.AppendLine(TabTool2.Text);
                if (v == 2) sb.AppendLine(TabTool3.Text);
                if (v == 3) sb.AppendLine(TabTool4.Text);
                if (v == 4) sb.AppendLine(TabTool5.Text);
                if (v == 5) sb.AppendLine(TabTool6.Text);
                if (v == 6) sb.AppendLine(TabTool7.Text);
                if (v == 7) sb.AppendLine(TabTool8.Text);

                for (int m = 0; m < 20; m++)
                {
                    strRowValue = (m + (v * 20)).ToString("D3");
                    strRowValue += "," + dataGridArray[v].Rows[m].Cells[1].Value;
                    strRowValue += "," + dataGridArray[v].Rows[m].Cells[2].Value;
                    sb.AppendLine(strRowValue);
                }
            }
            DialogResult dr = sfd_SaveLinData.ShowDialog();
            if (dr == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(sfd_SaveLinData.FileName);
                sw.Write(sb.ToString());
                sw.Close();
                myPCANTermMessageLF("-I: PCAN Command Table Saved. FileName:" + sfd_SaveLinData.FileName.ToString());
            }
            else
            {
                myPCANTermMessageLF("+W: Unable to save PCAN Command Table. FileName:" + sfd_SaveLinData.FileName.ToString());
            }
        }
        #endregion  

        #region//=======================================TabTools_Import_Filename
        // Updated 03-Aug-2013: change text name on button and set reference to MyDocument in C drive so it repeatable for all PC.
        private void TabTools_Import_Filename()
        {
            string rowValue;
            string[] cellValue;
            //-------------------------Clear Array
            for (int v = 0; v < NoOfTabs; v++)
            {
                if (v == 0) TabTool1.Text = "Tool1";
                if (v == 1) TabTool2.Text = "Tool2";
                if (v == 2) TabTool3.Text = "Tool3";
                if (v == 3) TabTool4.Text = "Tool4";
                if (v == 4) TabTool5.Text = "Tool5";
                if (v == 5) TabTool6.Text = "Tool6";
                if (v == 6) TabTool7.Text = "Tool7";
                if (v == 7) TabTool8.Text = "Tool8";

                for (int m = 0; m < 20; m++)
                {
                    dataGridArray[v].Rows[m].Cells[1].Value = "";
                    dataGridArray[v].Rows[m].Cells[2].Value = "";
                }
            }

            OpenFileDialog ofd_ImportLinData = new OpenFileDialog();
            ofd_ImportLinData.Filter = "csv files (*.csv)|*.csv";
            ofd_ImportLinData.FileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ofd_ImportLinData.Title = "Load CVS File for Command Table";
            DialogResult dr = ofd_ImportLinData.ShowDialog();
            if (dr == DialogResult.OK)
            {
                sCommandTableFileNamePCAN = ofd_ImportLinData.FileName;
                StreamReader sr = new StreamReader(ofd_ImportLinData.FileName);
                try
                {
                    while (sr.Peek() != -1)
                    {
                        for (int v = 0; v < NoOfTabs; v++)
                        {
                            if (v == 0) TabTool1.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 1) TabTool2.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 2) TabTool3.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 3) TabTool4.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 4) TabTool5.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 5) TabTool6.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 6) TabTool7.Text = sr.ReadLine().Replace(",", ""); ;
                            if (v == 7) TabTool8.Text = sr.ReadLine().Replace(",", ""); ;

                            for (int m = 0; m < 20; m++)
                            {
                                rowValue = sr.ReadLine();
                                cellValue = rowValue.Split(',');
                                dataGridArray[v].Rows[m].Cells[1].Value = cellValue[1];
                                dataGridArray[v].Rows[m].Cells[2].Value = cellValue[2];
                            }
                        }
                        break;
                    }
                    myPCANTermMessageLF("-I: Command Data Loaded");
                }
                catch (Exception ex)
                {
                    myPCANTermMessageLF("+W:Problem with Import File: " + ex.Message);
                }
                sr.Close();
            }
            else
            {
                Trace.WriteLine("+WARN: Filename Not Valid / Rejected Import Procedure");
            }
            TabMasterPCAN.Refresh();
        }
        #endregion

        #region//=======================================TabTool_Insert_CMD_Comment
        private void TabTool_Insert_CMD_Comment(string[] Entry, int index)      // index=-1: seek empty line and fill. 
        {
            int v = 0;
            int m = 0;
            try  // Catch errors.
            {
                if (index == -1)    // Seek empty row for insertion.
                {
                    for (v = 0; v < NoOfTabs; v++)
                    {
                        for (m = 0; m < 20; m++)
                        {
                            if (string.IsNullOrEmpty(dataGridArray[v].Rows[v].Cells[1].Value as string) == false)
                            {
                                if (dataGridArray[v].Rows[m].Cells[1].Value.ToString() == null)
                                {
                                    goto end_of_loop;
                                }
                                if (dataGridArray[v].Rows[m].Cells[1].Value.ToString() == "")
                                {
                                    goto end_of_loop;
                                }
                            }
                            else
                            {
                                goto end_of_loop;
                            }
                        }
                    }
                }
                else
                {
                    v = index / 20;     //int div = a / b; //quotient is 1
                    m = index / 20;     //int mod = a % b; //remainder is 2
                }
            end_of_loop:

                if ((v * m) >= NoOfTabs * 20)
                    return;
                //------------------------------------------------------
                dataGridArray[v].Rows[m].Cells[1].Value = Entry[0];
                dataGridArray[v].Rows[m].Cells[2].Value = Entry[1];
            }
            catch (System.Exception ex)
            {
                return;
            }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= Help
        //##############################################################################################################

        #region //=======================================BtnCANHelpA
        public void BtnCANHelpA()
        {
            Form frm = new Form();
            RichTextBox tbx = new RichTextBox();
            Font font = new System.Drawing.Font("Monospac821 BT", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            tbx.Font = font;
            tbx.Height = 530;
            tbx.Width = 600;
            frm.Height = 540;
            frm.Width = 610;
            frm.Controls.Add(tbx);
            tbx.Multiline = true;
            tbx.ScrollBars = RichTextBoxScrollBars.Vertical;
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText(" Help Section for PCAN Driver and connect issue\r\n");
            tbx.AppendText(" Important: The driver must be version >4.3 (Sep19). \r\n");
            tbx.AppendText("--------------------------------------------------------\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" PCAN is the Peak CAN/USB Device. \r\n");
            tbx.AppendText(" It carried a nickname: 'White CAN' (to difference from Blue Can device).\r\n");
            tbx.AppendText(" It used for firmware boot loader and debug.\r\n");
            tbx.AppendText(" Important: PCAN is not passive listener device, it do modify ACK bit.\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText(" PCAN dll Driver Installation (Important Read Me First!)\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" You need to to copy the <PCANBasic.dll> manually to your Windows System.\r\n");
            tbx.AppendText(" directory for CANTerminal to work. From Peak <Win32> folder, copy into \r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" Copy <PCANBasic.dll> into Windows\\System32\r\n");
            tbx.AppendText(" and also.\r\n");
            tbx.AppendText(" Copy <PCANBasic.dll> into Windows\\SysWOW64\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" The item: <PCANBasic.dll> may be found in the UDTTerm folder\r\n");
            tbx.AppendText(" In case it missing, you can obtain it from Peak website (below link).\r\n");
            tbx.AppendText(" ....Make sure it come from <Win32> folder not <x64>.\r\n");
            tbx.AppendText("     NB: UDTTerm is not 64 bit encoded.\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" https://www.peak-system.com/PCAN-USB.199.0.html?&L=1 \r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText(" Connect and Release PCAN channel\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText(" <Check Drv> check if driver exist or installed correctly\r\n");
            tbx.AppendText("             Do not attempt to connect if not installed, see above\r\n");
            tbx.AppendText(" <BAUD>      Select BAUD Rate for project CAN Bus\n");
            tbx.AppendText(" <Filter>    WIP/TBA\n");
            tbx.AppendText(" <Connect>   Connect and activate PCAN device\n");
            tbx.AppendText(" <Release>   Dis-Connect PCAN device\n");
            tbx.AppendText(" When PCAN window is closed (top right 'x'), it will invoke <Release>\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText(" Connect Issue (due to USB disconnect/reconnect PCAN)\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" NB: The driver must be version >4.3 (Sep19) for this to work!. \r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" UDT does not contains feature to assist driver reset under an orphaned state.\r\n");
            tbx.AppendText(" Rather than resetting Laptop and start again, do the following step: \r\n");
            tbx.AppendText(" Type 'PEAKCPL' in Window Search Bar, click on 'PEAKCPL.exe' to run\r\n");
            tbx.AppendText(" This pop up Properties of Peak Hardware window.\r\n");
            tbx.AppendText(" On 'CAN Hardware' Tab, check if there active channel while disconnected\r\n");
            tbx.AppendText(" from USB port, select and click 'Delete' and try connect again from UDT.\r\n");
            tbx.AppendText(" It also display driver version, make sure it >4.3 or later.\r\n");
            tbx.AppendText(" For more details, visit to the following links. \r\n");
            tbx.AppendText(" https://www.peak-system.com/PEAK-Hardware-Control-Applet.412.0.html?&L=1 \r\n");
            tbx.AppendText(" and forum discussion related to this issue. \r\n");
            tbx.AppendText(" https://www.peak-system.com/forum/viewtopic.php?f=41&t=2893&sid=6b8e98551ec40f68d6c6d8730e956c5f&start=10 \r\n");
            frm.ShowDialog();
        }
        #endregion

        #region //=======================================BtnCANHelp_CommandSummary
        public void BtnCANHelp_CommandSummary()
        {
            Form frm = new Form();
            RichTextBox tbx = new RichTextBox();
            Font font = new System.Drawing.Font("Monospac821 BT", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            tbx.Font = font;
            tbx.Height = 690;
            tbx.Width = 600;
            frm.Height = 700;
            frm.Width = 610;
            frm.Font = font;
            frm.Controls.Add(tbx);
            tbx.Multiline = true;
            tbx.ScrollBars = RichTextBoxScrollBars.Vertical;
            tbx.AppendText("---------------------------------------------------------------------\r\n");
            tbx.AppendText(" Help Section for UDT Command Protocol \r\n");
            tbx.AppendText("---------------------------------------------------------------------\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" ------------------\r\n");
            tbx.AppendText(" Limitation \r\n");
            tbx.AppendText(" ------------------\r\n");
            tbx.AppendText(" Standard Frame: up to 8 byte data is supported. Max 1MHz.\r\n");
            tbx.AppendText(" Extended Frame is not supported. It can be arranged by request.\r\n");
            tbx.AppendText(" New Gen FD is not supported.\r\n");
            tbx.AppendText(" Bulk transfer is not supported. It can be arranged by request.\r\n");
            tbx.AppendText(" PCAN device provide precise timestamp in uSec on RX reception\r\n");
            tbx.AppendText(" But there no TX timestamp with PCAN Device. It rely on window\r\n");
            tbx.AppendText(" timestamp which is not precise in uSec context.\r\n");
            tbx.AppendText(" This also affect the delay between command.\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" ------------------\r\n");
            tbx.AppendText(" TASK \r\n");
            tbx.AppendText(" ------------------\r\n");
            tbx.AppendText(" (1) Filter included but not tested fully\r\n");
            tbx.AppendText(" (4) One of Tab is heartbeat including counter and missed\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText(" Single PCAN command: CS(....)\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText(" CS(ID;DLC;D0;D1;D2;D3;D4;D5;D6;D7) or \r\n");
            tbx.AppendText(" CS(ID;DLC;D0D1D2D3D4D5D6D7)\r\n");
            tbx.AppendText(" ID       = CAN ID \r\n");
            tbx.AppendText(" DLC      = Number of Data Length: 1 to 8 \r\n");
            tbx.AppendText(" D0 to D7 = Data in Hex \r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText(" Multiple PCAN command: CD(....) with delay\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText(" Multiple Command can be placed \r\n");
            tbx.AppendText(" CD(ID;DLC;D0..D7;|Delay;ID;DLC;D0..D7;|Delay;ID;DLC;D0..D7, etc) or \r\n");
            tbx.AppendText(" CD(|Delay;ID;DLC;D0..D7;|Delay;ID;DLC;D0..D7;|Delay;ID;DLC;D0..D7, etc) \r\n");
            tbx.AppendText(" |Delay    = Delay between command in uSec (Decimal)\r\n");
            tbx.AppendText(" D0..D7    = See SC().\r\n");
            tbx.AppendText(" NB: The ';|' act as command frame separator which begin with delay.\r\n");
            tbx.AppendText(" Limitation: Maximum 8 command frame in turns. Use CL(...) \r\n");
            tbx.AppendText(" Example CD(620; 8; 2B204005DC050000;| 5500; 1C1; 5; 0000010000) \r\n");
            tbx.AppendText("  or (is same as)\r\n");
            tbx.AppendText(" Example CD(| 0;620; 8; 2B204005DC050000;| 5500; 1C1; 5; 0000010000) \r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText(" Loop PCAN command: CL(....)\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText(" CL(Delay;CyclePeriod;NoOfLoop;ID;DLC;D0;D1;D2;D3;D4;D5;D6;D7) \r\n");
            tbx.AppendText(" Delay     = Pause before 1st transmits, in mSec \r\n");
            tbx.AppendText(" CyclePeriod= Period between loops, after 1st transmits \r\n");
            tbx.AppendText(" NoOfLoop  = Number of loop. 0 = Infinite, 1 one loop, etc \r\n");
            tbx.AppendText(" D0 to D7 = Data in Hex \r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText(" Open for new command idea, please email to Riscy00\r\n");
            tbx.AppendText(" -------------------------------------------------------\r\n");
            tbx.AppendText("\r\n");
            frm.ShowDialog();
        }
        #endregion

        #region //=======================================BtnCANHelp_GeneralUse
        public void BtnCANHelp_GeneralUse()
        {
            Form frm = new Form();
            RichTextBox tbx = new RichTextBox();
            Font font = new System.Drawing.Font("Monospac821 BT", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            tbx.Font = font;
            tbx.Height = 690;
            tbx.Width = 600;
            frm.Height = 700;
            frm.Width = 610;
            frm.Font = font;
            frm.Controls.Add(tbx);
            tbx.Multiline = true;
            tbx.ScrollBars = RichTextBoxScrollBars.Vertical;
            tbx.AppendText("---------------------------------------------------------------------\r\n");
            tbx.AppendText(" Help Section for UDT General Use \r\n");
            tbx.AppendText("---------------------------------------------------------------------\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText("UDT PCAN Performance Issue\r\n");
            tbx.AppendText("-----------------------------\r\n");
            tbx.AppendText("The PCAN/UDT code has been crafted for best possible processing to\r\n");
            tbx.AppendText("handle rapid PCAN Message. However the constraint depends on  \r\n");
            tbx.AppendText("Window 10 laptop, where >i7, good 16GB RAM, SSD is preferable. Slower\r\n");
            tbx.AppendText("Laptop or clogged by poor maintenance, running other app, Window 10 \r\n");
            tbx.AppendText("ongoing update, task distraction, ie network access, etc, etc,  \r\n");
            tbx.AppendText("will impairs the PCAN performance and especially heatbeat update.\r\n");
            tbx.AppendText("The PCAN in UDT is tested on ASUS N705UD while powered by a AC/DC cord.\r\n");
            tbx.AppendText("Running on battery: requires change in setting to max performance.\r\n");
            tbx.AppendText("It good practice to turn off Network/Internet access, avoid running\r\n");
            tbx.AppendText("other app and turn off window update activity and search indexer.\r\n");
            tbx.AppendText("Well maintained Laptop provides best result from UDT/PCAN App.\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText("Log Message Update Period (Realtime/1/2/5 Second)\r\n");
            tbx.AppendText("-------------------------------------------------\r\n");
            tbx.AppendText("To reduce MCU clogging up process resource due to rapid CAN message\r\n");
            tbx.AppendText("activity, in <Log Terms> Menu Tab, select 1/2/5 Second screen update\r\n");
            tbx.AppendText("This apply to RX/TX Log only. The RealTime disable this feature.\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText("Viewing the RX/TX Log Message\r\n");
            tbx.AppendText("-----------------------------\r\n");
            tbx.AppendText("The screen can be halted for inspection of PCAN log with scroll up/down.\r\n");
            tbx.AppendText("The screen is cleared if more than 30K ASCII if enabled. This may be\r\n");
            tbx.AppendText("removed in later revision.\r\n");
            tbx.AppendText("Copy and Paste of selected text is allowed.\r\n");
            tbx.AppendText("\r\n");
            frm.ShowDialog();
        }
        #endregion



        #region //=======================================PCANDriverInstallToolStripMenuItem_Click
        private void PCANDriverInstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BtnCANHelpA();
        }
        #endregion

        #region //=======================================PCANUDTCommandToolStripMenuItem_Click
        private void PCANUDTCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BtnCANHelp_CommandSummary();
        }
        #endregion

        #region //=======================================pCANGeneralUseToolStripMenuItem_Click
        private void pCANGeneralUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BtnCANHelp_GeneralUse();

        }
        #endregion

        //##############################################################################################################
        //============================================================================================= UDT PCAN Code
        //##############################################################################################################

        #region //=======================================PCAN_isPCANBasicDriverFitted
        //---------------------------------------------------------------
        // Purpose  : Make connection with PCANDevice
        // Input    :
        // Output   : true = fitted, false = no driver.
        // Status   :
        // Note     : PCAN Based code. Not sure if this any use, can it check if PCANDriver is fitted.
        //---------------------------------------------------------------
        public bool PCAN_isPCANBasicDriverFitted()
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;
            bool isFD;

            // Clears the Channel combioBox and fill it again with 
            // the PCAN-Basic handles for no-Plug&Play hardware and
            // the detected Plug&Play hardware
            //
            //cbbChannel.Items.Clear();
            try
            {
                for (int i = 0; i < m_HandlesArray.Length; i++)
                {
                    // Includes all no-Plug&Play Handles
                    if (m_HandlesArray[i] <= PCANBasic.PCAN_DNGBUS1)
                    {
                        //cbbChannel.Items.Add(FormatChannelName(m_HandlesArray[i]));
                    }
                    else
                    {
                        // Checks for a Plug&Play Handle and, according with the return value, includes it
                        // into the list of available hardware channels.
                        //
                        stsResult = PCANBasic.GetValue(m_HandlesArray[i], TPCANParameter.PCAN_CHANNEL_CONDITION, out iBuffer, sizeof(UInt32));
                        if ((stsResult == TPCANStatus.PCAN_ERROR_OK) && ((iBuffer & PCANBasic.PCAN_CHANNEL_AVAILABLE) == PCANBasic.PCAN_CHANNEL_AVAILABLE))
                        {
                            stsResult = PCANBasic.GetValue(m_HandlesArray[i], TPCANParameter.PCAN_CHANNEL_FEATURES, out iBuffer, sizeof(UInt32));
                            isFD = (stsResult == TPCANStatus.PCAN_ERROR_OK) && ((iBuffer & PCANBasic.FEATURE_FD_CAPABLE) == PCANBasic.FEATURE_FD_CAPABLE);
                            //cbbChannel.Items.Add(FormatChannelName(m_HandlesArray[i], isFD));
                        }
                    }
                }
                //cbbChannel.SelectedIndex = cbbChannel.Items.Count - 1;
                //btnInit.Enabled = cbbChannel.Items.Count > 0;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region //=======================================PCAN_Connect_With_Device
        //---------------------------------------------------------------
        // Purpose  : Make connection with PCANDevice
        // Input    :
        // Output   :
        // Status   :
        // Note     : PCAN Based code. FD not required.
        //---------------------------------------------------------------
        public bool PCAN_Connect_With_Device()
        {
            TPCANStatus stsResult;
            m_PcanHandle = myPCAN_Configuration.HandlesArray;
            m_Baudrate = myPCAN_Configuration.Baudrate;
            m_HwType = myPCAN_Configuration.HwType;

            stsResult = PCANBasic.Initialize(m_PcanHandle, m_Baudrate, m_HwType, myPCAN_Configuration.IOPort, myPCAN_Configuration.Interrupt);

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)     // Not okay.
            {
                if (stsResult != TPCANStatus.PCAN_ERROR_CAUTION)
                {
                    MessageBox.Show("+W: Connected by with Setup Issue:" + GetFormatedError(stsResult));
                }
                else
                {
                    myMainProg.myRtbTermMessageLF("******************************************************");
                    myMainProg.myRtbTermMessageLF("The bitrate being used is different than the given one");
                    myMainProg.myRtbTermMessageLF("******************************************************");
                    stsResult = TPCANStatus.PCAN_ERROR_OK;
                }
            }
            else // OK
            {
                // Prepares the PCAN-Basic's PCAN-Trace file
                //
                ConfigureTraceFile();                   //###TASK: Sort filename out if relevant. 
            }
            if (stsResult!= TPCANStatus.PCAN_ERROR_OK)
                return (false);
            // Sets the connection status of the main-form
            //
            SetConnectionStatus(stsResult == TPCANStatus.PCAN_ERROR_OK);
            return (true);
        }
        #endregion

        #region //=======================================PCAN_Release_With_Device
        //---------------------------------------------------------------
        // Purpose  : Make Disconnection with PCANDevice
        // Input    :
        // Output   :
        // Status   :
        // Note     : PCAN Based code. FD not required.
        //---------------------------------------------------------------
        public bool PCAN_Release_With_Device()
        {
            // Releases a current connected PCAN-Basic channel
            //
            PCANBasic.Uninitialize(m_PcanHandle);
            tmrRead.Enabled = false;
            if (m_ReadThread != null)
            {
                m_ReadThread.Abort();
                m_ReadThread.Join();
                m_ReadThread = null;
            }

            // Sets the connection status of the main-form
            //
            SetConnectionStatus(false);
            myGlobalBase.CanPCAN_isConnected = false;
            return (true);
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= UDT PCAD Files
        //##############################################################################################################

        #region //======================================= SaveConfigToolStripMenuItem_Click
        private void SaveConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCAN_SaveConfigToFile();
        }
        #endregion

        #region //======================================= PCAN_SaveConfigToFile
        private void PCAN_SaveConfigToFile()
        {
            if (mbDialogMessageBox == null)
                mbDialogMessageBox = new DialogSupport();
            //------------------------------------------------
            string pathString = myGlobalBase.CanPCAN_Config_FilenameName;
            if (myGlobalBase.CanPCAN_Config_FilenameName == "")
                pathString = myGlobalBase.CanPCAN_Config_FolderName;
            //------------------------------------------------
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //saveFileDialog1.Filter = "Xml|*.xml";
            saveFileDialog1.Filter = "Bin|*.bin";
            saveFileDialog1.Title = "Save CAN TX/Cycle  File";
            saveFileDialog1.InitialDirectory = pathString;
            //pathString = System.IO.Path.Combine(pathString, "PCANConfig_" + Tools.uDateTimeToUnixTimestampNowUTC().ToString() + ".xml");
            pathString = System.IO.Path.Combine(pathString, "PCANConfig_" + Tools.uDateTimeToUnixTimestampNowUTC().ToString() + ".bin");
            saveFileDialog1.FileName = pathString;
            //------------------------------------------------
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                myMainProg.myRtbTermMessageLF("#E: User Cancelled 'Save' Operation");
                return;
            }
            if (saveFileDialog1.FileName == "")
                return;
            //------------------------------------------------ Update folder/filename.
            //Path.GetDirectoryName(PCANsaveFileDialog1.FileName);      // Useful tools.
            //Path.GetFileName(PCANsaveFileDialog1.FileName);
            //Path.GetExtension(PCANsaveFileDialog1.FileName);

            myGlobalBase.CanPCAN_Config_FolderName = Path.GetDirectoryName(saveFileDialog1.FileName);
            myGlobalBase.CanPCAN_Config_FilenameName = saveFileDialog1.FileName;
            myPCAN_Configuration.sFolderName = Path.GetDirectoryName(saveFileDialog1.FileName);
            //---------------------------------------------------
            try
            {
                //myGlobalBase.SerializeToFile<PCAN_Configuration>(myPCAN_Configuration, saveFileDialog1.FileName);
                myGlobalBase.SerializeToFileBinary<PCAN_Configuration>(myPCAN_Configuration, saveFileDialog1.FileName);
            }
            catch
            {
                mbDialogMessageBox.PopUpMessageBox("ERROR: Unable to save CAN Config within designated folder, is this protected folder?", "File System Error", 5, 12F);
                return;
            }
            mbDialogMessageBox.PopUpMessageBox("Successfully saved ToCo Setup File:\n" + saveFileDialog1.FileName, "Setup File System", 2, 12F);
        }
        #endregion

        #region //======================================= PCAN_LoadConfigToFile
        private void PCAN_LoadConfigToFile()
        {
            if (mbDialogMessageBox == null)
                mbDialogMessageBox = new DialogSupport();
            //------------------------------------------------
            string sFoldername = myGlobalBase.CanPCAN_Config_FolderName;
            string sFilename = myGlobalBase.CanPCAN_Config_FilenameName;
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                OpenFileDialog ofd = openFileDialog;
                ofd.Title = "Setup Files";
                //ofd.Filter = "xml files|*.xml";
                ofd.Filter = "bin files|*.bin";
                ofd.InitialDirectory = sFoldername;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    sFilename = ofd.FileName;
                    sFoldername = System.IO.Path.GetDirectoryName(ofd.FileName);
                    myGlobalBase.CanPCAN_Config_FilenameName = sFilename;
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
            tsbFolderName.Text = sFoldername;
            try
            {
                myPCAN_Configuration = null;
                myPCAN_Configuration = new PCAN_Configuration();
                myPCAN_Configuration.Init_ListDataBase();               // Put default database back in. 
                //myPCAN_Configuration = myGlobalBase.DeserializeFromFile<PCAN_Configuration>(sFilename);
                myPCAN_Configuration = myGlobalBase.DeserializeFromFileBinary<PCAN_Configuration>(sFilename);
                //------------------------------------------------------------------------Update to GlobalBase. 
                myGlobalBase.CanPCAN_Config_FolderName = myPCAN_Configuration.sFolderName;
            }
            catch
            {
                mbDialogMessageBox.PopUpMessageBox("ERROR: Unable to load Setup within designated folder, may not exist?", "File System Error", 5, 12F);
                return;
            }
            mbDialogMessageBox.PopUpMessageBox("Successfully Loaded Setup File and updated Setup", "File System", 2, 14F);
            myGlobalBase.CanPCAN_Refresh = true;
            this.Invalidate();
            this.Refresh();
            //------------------------------------Refresh panel in UDT Tab
            myMainProg.PCAN_Update_TabPanel();

        }
        #endregion

        #region //======================================= LoadConfigToolStripMenuItem_Click
        private void LoadConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCAN_LoadConfigToFile();
        }
        #endregion

        #region //======================================= CreateFolderToolStripMenuItem1_Click
        private void CreateFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string sFoldername = myGlobalBase.CanPCAN_Config_FolderName;
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
            myGlobalBase.CanPCAN_Config_FolderName = sFoldername;
            try
            {
                //----------------------------------------------------------
                DialogResult result = fdbPCANConfig.ShowDialog();
                if (result == DialogResult.OK)
                {
                    sFoldername = fdbPCANConfig.SelectedPath;
                    if (!Directory.Exists(sFoldername))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(sFoldername);  // Create folder if not exist. 
                    }
                }
                tsbFolderName.Text = sFoldername;
                myGlobalBase.CanPCAN_Config_FolderName = sFoldername;
                myPCAN_Configuration.sFolderName = sFoldername;
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("###ERR: Folder Window Reported an Error");
            }
        }
        #endregion

        #region //======================================= OpenFolderToolStripMenuItem_Click
        private void OpenFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mbDialogMessageBox == null)
                mbDialogMessageBox = new DialogSupport();
            //------------------------------------------------
            try
            {
                string myPath = myGlobalBase.CanPCAN_Config_FolderName;
                if (Directory.Exists(myPath))                             // Default folder name for given drive. 
                {
                    System.Diagnostics.Process prc = new System.Diagnostics.Process();
                    prc.StartInfo.FileName = myPath;
                    prc.Start();
                }
                else
                {
                    myMainProg.myRtbTermMessageLF("###ERR: Unable to Open/Create Folder");
                    mbDialogMessageBox.PopUpMessageBox("ERROR! Unable to Open/Create Folder", "File Error in PCAN Setup", 10);
                }
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("###ERR: Unable to Open/Create Folder");
                mbDialogMessageBox.PopUpMessageBox("ERROR! Unable to Open/Create Folder", "File Error in PCAN Setup", 10);
            }
        }
        #endregion

        #region //======================================= OpenFolderToolStripMenuItem1_Click
        private void OpenFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (mbDialogMessageBox == null)
                mbDialogMessageBox = new DialogSupport();
            //------------------------------------------------
            try
            {
                string myPath = myGlobalBase.CanPCAN_Config_FolderName;
                if (Directory.Exists(myPath))                             // Default folder name for given drive. 
                {
                    System.Diagnostics.Process prc = new System.Diagnostics.Process();
                    prc.StartInfo.FileName = myPath;
                    prc.Start();
                }
                else
                {
                    myMainProg.myRtbTermMessageLF("###ERR: Unable to Open/Create Folder");
                    mbDialogMessageBox.PopUpMessageBox("ERROR! Unable to Open/Create Folder", "File Error in PCAN Setup", 10);
                }
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("###ERR: Unable to Open/Create Folder");
                mbDialogMessageBox.PopUpMessageBox("ERROR! Unable to Open/Create Folder", "File Error in PCAN Setup", 10);
            }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= PCAN Read Frame & Display
        //##############################################################################################################
        // Trace/Log Plan Layout.
        //Demo          // TimeStamp,CANID,RXTX,Type,DLC,Data
        //MicSig        // TimeStamp,CANID,Type,DLC,Data (Fixed Width 8*3=24),CRC,Err,Trig.
        //
        //UDT           // TimeStamp,CANID,RXTX,Type,Err,Count,DLC,Data (Fixed Width 8*3=24) || Decoded Data based on ID (via Scrip)
        //                 Type: SFF = Standard Frame, H = Heartbeat, EFF = Extended Frame.
        //                 Count: No of same CANID occurance for this session, cleared when reset.
        //                 Timestamp=fixed length 

        #region //=======================================ReadMessage
        private TPCANStatus ReadMessage()
        {
            TPCANMsg CANMsg;
            TPCANTimestamp CANTimeStamp;
            TPCANStatus stsResult;
            sMessageBusStatus = "";         //Clear Message

            // We execute the "Read" function of the PCANBasic                
            //
            stsResult = PCANBasic.Read(m_PcanHandle, out CANMsg, out CANTimeStamp);
            if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
            {
                //------------------------------------------------appears by more than 96 error points, the CAN-node work in the "error active" state 
                if (stsResult != TPCANStatus.PCAN_ERROR_BUSLIGHT)
                {
                    btnBusLight.BackColor = Color.LightGreen;
                }
                else
                {
                    sMessageBusStatus += "LIGHT ";
                    btnBusLight.BackColor = Color.Orange;
                }
                //------------------------------------------------ appears by more than 127 error points, the CAN-node work in the "error passive" state
                if (stsResult != TPCANStatus.PCAN_ERROR_BUSHEAVY)
                {
                    btnBusHeavy.BackColor = Color.Green;
                }
                else
                {
                    sMessageBusStatus += "HEAVY ";
                    btnBusHeavy.BackColor = Color.OrangeRed;
                }
                //------------------------------------------------ appears by more than 255 error points, the CAN-node is disconnected from the bus
                if (stsResult != TPCANStatus.PCAN_ERROR_BUSOFF)
                {
                    btnBusOFF.BackColor = Color.LightGreen;
                }
                else
                {
                    sMessageBusStatus += "OFF ";
                    btnBusOFF.BackColor = Color.Red;
                }
                //------------------------------------------------ At 128 error points the CAN-controller works in a "error passive" mode. 
                //                                                 The CAN-node is fully functional working in this state, but if the node detect an error he signalize this with a passive error flag. 
                //                                                 This flag don´t destroy the CAN-message and the CAN-node must wait some time before he can send a CAN-message. 
                //                                                 This prevented that a CAN-node with a high error-rate blocked the bus.
                if (stsResult != TPCANStatus.PCAN_ERROR_BUSPASSIVE)
                {
                    btnPassive.BackColor = Color.LightGreen;
                }
                else
                {
                    sMessageBusStatus += "PASSIVE ";
                    btnPassive.BackColor = Color.Yellow;
                }
                //------------------------------------------------ HeartBeat ID 0x701 to 0x77F only.  
                HeadMaster_HB_BeatNow_Existing((int)CANMsg.ID);
                //------------------------------------------------
                ProcessMessage(CANMsg, CANTimeStamp);           // We process the received message
            }
            return stsResult;
        }
        #endregion

        #region //=======================================ReadMessages (Top Level)
        private void ReadMessages()
        {
            TPCANStatus stsResult;

            // We read at least one time the queue looking for messages.
            // If a message is found, we look again trying to find more.
            // If the queue is empty or an error occurr, we get out from
            // the dowhile statement.
            //			
            do
            {
                stsResult = ReadMessage();
                if (stsResult == TPCANStatus.PCAN_ERROR_ILLOPERATION)
                    break;
            } while (myPCAN_Configuration.ReleaseEnable && (!Convert.ToBoolean(stsResult & TPCANStatus.PCAN_ERROR_QRCVEMPTY)));
        }
        #endregion

        #region //=======================================ProcessMessage (Standard)
        /// <summary>
        /// Processes a received message, in order to show it in the Message-ListView
        /// </summary>
        /// <param name="theMsg">The received PCAN-Basic message</param>
        /// <returns>True if the message must be created, false if it must be modified</returns>
        private void ProcessMessage(TPCANMsg theMsg, TPCANTimestamp itsTimeStamp)
        {
            TPCANMsgFD newMsg;
            TPCANTimestampFD newTimestamp;

            newMsg = new TPCANMsgFD();
            newMsg.DATA = new byte[64];
            newMsg.ID = theMsg.ID;
            newMsg.DLC = theMsg.LEN;
            for (int i = 0; i < ((theMsg.LEN > 8) ? 8 : theMsg.LEN); i++)
            {
                newMsg.DATA[i] = theMsg.DATA[i];
            }
            newMsg.MSGTYPE = theMsg.MSGTYPE;
            //DateTime fecha = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            //newTimestamp = Convert.ToUInt64(itsTimeStamp.micros + 1000 * itsTimeStamp.millis + 0x100000000 * 1000 * itsTimeStamp.millis_overflow);
            newTimestamp = PCAN_GetTimestamp_uSec();
            ProcessMessage(newMsg, newTimestamp);
        }
        #endregion

        #region //=======================================ProcessMessage (FD)
        /// <summary>
        /// Processes a received message, in order to show it in the Message-ListView
        /// </summary>
        /// <param name="theMsg">The received PCAN-Basic message</param>
        /// <returns>True if the message must be created, false if it must be modified</returns>
        private void ProcessMessage(TPCANMsgFD theMsg, TPCANTimestampFD itsTimeStamp)
        {
            // We search if a message (Same ID and Type) is 
            // already received or if this is a new message
            lock (m_LastMsgsList.SyncRoot)
            {
                foreach (MessageStatus msg in m_LastMsgsList)
                {
                    if ((msg.CANMsg.ID == theMsg.ID) && (msg.CANMsg.MSGTYPE == theMsg.MSGTYPE))
                    {
                        msg.Update(theMsg, itsTimeStamp);           // Modify the message and exit
                        goto LogSection;
                    }
                }
                InsertMsgEntry(theMsg, itsTimeStamp);               // Message not found. It will created
            }
        LogSection:
            LogMsgEntry(theMsg, itsTimeStamp);
            lstMessages.Refresh();
        }
        #endregion

        #region //=======================================DisplayMessages
        /// <summary>
        /// Display CAN messages in the Message-ListView
        /// </summary>
        private void DisplayMessages()
        {
            ListViewItem lviCurrentItem;

            lock (m_LastMsgsList.SyncRoot)
            {
                foreach (MessageStatus msgStatus in m_LastMsgsList)
                {
                    if (msgStatus.MarkedAsUpdated)              // Get the data to actualize
                    {
                        int iLength = GetLengthFromDLC(msgStatus.CANMsg.DLC, (msgStatus.CANMsg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == 0);
                        msgStatus.MarkedAsUpdated = false;
                        lviCurrentItem = lstMessages.Items[msgStatus.Position];
                        lviCurrentItem.SubItems[2].Text = iLength.ToString();
                        lviCurrentItem.SubItems[3].Text = msgStatus.Count.ToString();
                        lviCurrentItem.SubItems[4].Text = msgStatus.TimeString;
                        lviCurrentItem.SubItems[5].Text = msgStatus.DataString;
                        //-----------------------------------------------------------Comment out duplicate operation, below is redundant code 78JA onward. 
                        // The code: private void LogMsgEntry(TPCANMsgFD newMsg, TPCANTimestampFD timeStamp) take care of this. 
                        //uint CanID = msgStatus.CANMsg.ID;
                        if ((msgStatus.CANMsg.ID >= 0x700) & (msgStatus.CANMsg.ID <= 0x77F))        // HeartBeat
                        {
                            if (cbLogExcludeHeartBeat.Checked == false)
                            {
                                PCAN_InsertMsgToLogTerminal(msgStatus.PCAN_GenerateLogMessageRX(iLength));
                            }
                        }
                        //else
                        //{
                        //    PCAN_InsertMsgToLogTerminal(msgStatus.PCAN_GenerateLogMessageRX(iLength));
                        //}
                    }
                }
            }
        }
        #endregion

        #region //=======================================InsertMsgEntry 
        /// <summary>
        /// Inserts a new entry for a new message in the Message-ListView
        /// </summary>
        /// <param name="newMsg">The messasge to be inserted</param>
        /// <param name="timeStamp">The Timesamp of the new message</param>
        private void InsertMsgEntry(TPCANMsgFD newMsg, TPCANTimestampFD timeStamp)
        {
            MessageStatus msgStsCurrentMsg;
            ListViewItem lviCurrentItem;

            lock (m_LastMsgsList.SyncRoot)
            {
                //----------------------------------------------------------We add this status in the last message list
                msgStsCurrentMsg = new MessageStatus(newMsg, timeStamp, lstMessages.Items.Count);
                msgStsCurrentMsg.sErrorBusStatus = sMessageBusStatus;
                msgStsCurrentMsg.ShowingPeriod = chbShowPeriod.Checked;
                m_LastMsgsList.Add(msgStsCurrentMsg);
                int iLength = GetLengthFromDLC(newMsg.DLC, (newMsg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == 0);

                // Add the new ListView Item with the Type of the message
                //	
                lviCurrentItem = lstMessages.Items.Add(msgStsCurrentMsg.TypeString);
                // We set the ID of the message
                //
                lviCurrentItem.SubItems.Add(msgStsCurrentMsg.IdString);
                // We set the length of the Message
                //
                lviCurrentItem.SubItems.Add(iLength.ToString());
                // we set the message count message (this is the First, so count is 1)            
                //
                lviCurrentItem.SubItems.Add(msgStsCurrentMsg.Count.ToString());
                // Add time stamp information if needed
                //
                lviCurrentItem.SubItems.Add(msgStsCurrentMsg.TimeString);
                // We set the data of the message. 	
                //
                lviCurrentItem.SubItems.Add(msgStsCurrentMsg.DataString);
            }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= PCAN Emulation Test (HeartBeat)
        //##############################################################################################################
        // 1 second 740 MWD-EM
        // 1 second 730 MWD-DIR
        // 5 second 720 Gamma
        // 2.5 sec 750 Battery
        TPCANMsg HBEMU_740;
        TPCANMsg HBEMU_730;
        TPCANMsg HBEMU_720;
        TPCANMsg HBEMU_750;
        TPCANMsg[] HEMUFrame;

        int[] HBEMU_PeriodReset;
        int[] HBEMU_PeriodCounter;
        private enum ButtonID { Gamma = 0, MWDDIR, MWDEM, BATTERY };

        #region //=======================================heatBeatEmuTestToolStripMenuItem_Click
        private void HeatBeatEmu_Init()
        {
            //------------------------------------------------------------------
            HBEMU_740 = new TPCANMsg();
            HBEMU_740.ID = 0x740;
            HBEMU_740.LEN = 1;
            HBEMU_740.DATA = new byte[64];
            HBEMU_740.DATA[0] = 0x05;
            HBEMU_740.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            //------------------------------------------------------------------
            HBEMU_730 = new TPCANMsg();
            HBEMU_730.ID = 0x730;
            HBEMU_730.LEN = 1;
            HBEMU_730.DATA = new byte[64];
            HBEMU_730.DATA[0] = 0x05;
            HBEMU_730.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            //------------------------------------------------------------------
            HBEMU_720 = new TPCANMsg();
            HBEMU_720.ID = 0x720;
            HBEMU_720.LEN = 1;
            HBEMU_720.DATA = new byte[64];
            HBEMU_720.DATA[0] = 0x05;
            HBEMU_720.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            //------------------------------------------------------------------
            HBEMU_750 = new TPCANMsg();
            HBEMU_750.ID = 0x750;
            HBEMU_750.LEN = 1;
            HBEMU_750.DATA = new byte[64];
            HBEMU_750.DATA[0] = 0x05;
            HBEMU_750.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            //------------------------------------------------------------------
            //                            Gamma      MWD-DIR    MWD-EM     Battery
            HEMUFrame = new TPCANMsg[4] { HBEMU_720, HBEMU_730, HBEMU_740, HBEMU_750 };
            HBEMU_PeriodReset = new int[4] { 5000, 999, 1001, 2500 };      // Reset counter 
            HBEMU_PeriodCounter = new int[4] { 0, 0, 0, 0 };
        }
        #endregion

        #region //=======================================heatBeatEmuTestToolStripMenuItem_Click
        private void heatBeatEmuTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.CanPCAN_isConnected == true)     // Not allowed, safeguard. 
            {
                string msg = "#E: Unable to Run while PCAN is connected. Disconnect PCAN via release button and try again\n";
                PCAN_InsertMsgToLogTerminal(msg);
                myMainProg.myRtbTermMessage(msg);
                heatBeatEmuTestToolStripMenuItem.Checked = false;
                myGlobalBase.CanPCAN_isHeatBeatEmulationMode = false;
                return;
            }
            if (heatBeatEmuTestToolStripMenuItem.Checked == false)
            {
                PCAN_InsertMsgToLogTerminal("-I: Start MWD HeartBeat Emulation with 740,730,720 and 750\n");
                HeartBeatStartEmulation();
                heatBeatEmuTestToolStripMenuItem.Checked = true;

            }
            else
            {
                PCAN_InsertMsgToLogTerminal("-I: Cancel MWD HeartBeat Emulation\n");
                HeartBeatStopEmulation();
                heatBeatEmuTestToolStripMenuItem.Checked = false;
                myGlobalBase.CanPCAN_isHeatBeatEmulationMode = false;
            }
        }
        #endregion

        #region //=======================================HeartBeatStartEmulation
        private void HeartBeatStartEmulation()
        {
            if (HEMUFrame == null)
            {
                HeatBeatEmu_Init();
            }
            myGlobalBase.CanPCAN_isHeatBeatEmulationMode = true;
        }
        #endregion

        #region //=======================================HeartBeatStopEmulation
        private void HeartBeatStopEmulation()
        {
            myGlobalBase.CanPCAN_isHeatBeatEmulationMode = false;
        }
        #endregion

        #region //=======================================HeartBeatEmulation_Process_mSecTicks
        private void HeartBeatEmulation_Process_mSecTicks()
        {
            for (int i = 0; i < 4; i++)
            {
                HBEMU_PeriodCounter[i]++;
            }
            for (int i = 0; i < 4; i++)
            {
                if (HBEMU_PeriodCounter[i] >= HBEMU_PeriodReset[i])
                {
                    //------------------------------------------------ HeartBeat ID 0x701 to 0x77F only.  
                    HeadMaster_HB_BeatNow_Existing((int)HEMUFrame[i].ID);
                    ProcessMessage(HEMUFrame[i], PCAN_GetTimestamp());
                    HBEMU_PeriodCounter[i] = 0;
                }
            }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= PCAN Log Message
        //##############################################################################################################

        #region //=======================================PCAN_InsertMsgToLogTerminal 
        private void PCAN_InsertMsgToLogTerminal(string LogMsg)
        {
            if (myGlobalBase.isCanPCANTermScreenHalted == false)
            {
                //---------------------------------------------------------------------------------------
                if ((myGlobalBase.isCanPCANTermUpdateModeTimerActivated == false) & (myGlobalBase.iCanPCANTerminalUpdateMode != (int)GlobalBase.eTerminalUpdate.TermREAL))
                {
                    myGlobalBase.CanPCANTermHaltMessageBuf += LogMsg;
                    myGlobalBase.isCanPCANTermUpdateModeTimerActivated = true;
                    bgwPCANTermUpdate.RunWorkerAsync();
                    goto SkipSection;
                }
                //----------------------------------------------------------------------------------
                if (myGlobalBase.iCanPCANTerminalUpdateMode != (int)GlobalBase.eTerminalUpdate.TermREAL)   // Delay Time Terminal refresh. 
                {
                    myGlobalBase.CanPCANTermHaltMessageBuf += LogMsg;
                    goto SkipSection;
                }
                //----------------------------------------------------------------------------------
                if (myGlobalBase.isCanPCANTermScreenHalted == false)
                {
                    if (rtPCanTerm != null)
                    {
                        Tools.rtb_StopRepaint(rtPCanTerm, rtbOEM);
                        rtPCanTerm.SelectionColor = myGlobalBase.ColorMessage;
                        rtPCanTerm.AppendText(myGlobalBase.CanPCANTermHaltMessageBuf + LogMsg);
                        Tools.rtb_StartRepaint(rtPCanTerm, rtbOEM);
                        //ScrollToBottom(rtPCanTerm);                   // This is done in every 50mSec timer code. 
                    }
                    myGlobalBase.CanPCANTermHaltMessageBuf = "";
                }
                else
                {
                    myGlobalBase.CanPCANTermHaltMessageBuf += LogMsg;
                }
            }
            SkipSection:
            //-----------------------------------------------------------------------------------------Main Terminal
            if (tsmiExportLogToMainUDTTermtsmi.Checked == true)
            {
                Tools.rtb_StopRepaint(rtPCanTerm, rtbOEM);
                myMainProg.myRtbTermMessage(">>>PCAN:" + LogMsg);
                Tools.rtb_StartRepaint(rtPCanTerm, rtbOEM);
            }
        }
        #endregion

        #region //=======================================LogMsgEntry 
        private void LogMsgEntry(TPCANMsgFD newMsg, TPCANTimestampFD timeStamp)
        {
            if ((newMsg.ID >= 0x700) & (newMsg.ID <= 0x77F))        // Added since other heatbeat code provide the message. 
                return;
            MessageStatus msgStsCurrentMsg;
            //----------------------------------------------------------We add this status in the last message list
            msgStsCurrentMsg = new MessageStatus(newMsg, timeStamp, lstMessages.Items.Count);
            int iLength = GetLengthFromDLC(newMsg.DLC, (newMsg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == 0);

            PCAN_InsertMsgToLogTerminal(msgStsCurrentMsg.PCAN_GenerateLogMessageRX(iLength));
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= PCAN Write Frame
        //##############################################################################################################



        //##############################################################################################################
        //============================================================================================= 1mSec Tick (Precision 1mSec)
        //##############################################################################################################

        #region //==================================================TmPCANOnemSec_Init
        private void TmPCANOnemSec_Init()
        {
            tm50mSecTick = 0;
            if (tmPCanOnemSec == null)
            {
                tmPCanOnemSec = new Multimedia.TimerX();
                tmPCanOnemSec.SynchronizingObject = this;
                tmPCanOnemSec.Period = 1;      // 1mSec Period.        
                tmPCanOnemSec.Resolution = 1;  // 1ms
                tmPCanOnemSec.Mode = TimerMode.Periodic;
                tmPCanOnemSec.Tick += new System.EventHandler(this.TmPCanOnemSec_Tick);
            }
        }
        #endregion

        #region //==================================================TmPCanOnemSec_Tick
        private void TmPCanOnemSec_Tick(object sender, EventArgs e)
        {
            //-----------------------------------------------------------HeartBeat Emulation
            if (myGlobalBase.CanPCAN_isHeatBeatEmulationMode == true)
            {
                HeartBeatEmulation_Process_mSecTicks();
            }
            //-----------------------------------------------------------Update RX/TX Log Window (50mSec)
            if (tm50mSecTick > 50)
            {
                DisplayMessages();
                if ((cbLogAutoScroll.Checked==true) & (myGlobalBase.isCanPCANTermScreenHalted == false))
                {
                    Tools.rtb_StopRepaint(rtPCanTerm, rtbOEM);
                    Tools.ScrollToBottom(rtPCanTerm);
                    Tools.rtb_StartRepaint(rtPCanTerm, rtbOEM);
                }
                tm50mSecTick = 0;
            }
            tm50mSecTick++;
            //-------------------------------------------------------------Update HeartBeat Section (100mSec)...WIP ###TASK: Review Heatbeat buffering and update.
            if (tm100mSecTick > 100)
            {
                tm100mSecTick = 0;
            }
            tm100mSecTick++;
            //==========================================================HeartBeat Only
            if (tmCanRXHeartBeatStatus == true)
            {
                for (int ch = 0; ch < myPCAN_Configuration.lsHeartBeatDisplayed.Count; ch++)
                {
                    PCAN_HeartBeat myHB = myPCAN_Configuration.lsHeartBeatDisplayed[ch];
                    int state = myHB.iIncrementCounts();
                    switch (state)
                    {
                        case (1):       // Active
                            {
                                break;
                            }
                        case (4):       // Highlight expiry heartbeat situation. 
                            {
                                HB_SetBlinkColor(ch, 4, myHB);
                                break;
                            }
                        case (5):       // Signify change color back to white for next blink. 
                            {
                                HB_SetBlinkColor(ch, 5, myHB);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            ////==========================================================HB Test Run
            //if (tmCanHB_TestRunMode == true)
            //{
            //    HB_TestRun_Tick();
            //}
            //==========================================================Cycle Code Only
            if (tmCanTXCycleStatus == true)
            {
                foreach (PCAN_StdFrame myLoop in myCycleArray)
                {
                    if (myLoop.isCounterMatched() == true)
                    {
                        PCAN_WriteFrame(myLoop, TPCANMessageType.PCAN_MESSAGE_STANDARD);
                        myLoop.CycleNumberProcess();                     // Update loop counter. 
                    }
                }
                //-----------------------------------------------------Now check if all loop is completed. 
                int count = 0;
                foreach (PCAN_StdFrame myLoop in myCycleArray)
                {
                    if (myLoop.isCycleNumberProcessExpired() == false)
                        count++;
                }
                if (count == 0)
                    tmPCanOnemSec_Stop();
            }
        }
        #endregion

        #region //==================================================tmPCanOnemSec_Start
        private void tmPCanOnemSec_Start()
        {
            tmCanTXCycleStatus = true;

            //             if (tmCanTXCycleStatus == false)
            //             {
            //                 tmCanTXCycleStatus = true;
            //                 tmPCanOnemSec.Start();
            //             }
        }
        #endregion

        #region //==================================================tmPCanOnemSec_Stop
        private void tmPCanOnemSec_Stop()
        {
            tmCanTXCycleStatus = false;
            //tmPCanOnemSec.Stop();
        }
        #endregion


        //##############################################################################################################
        //============================================================================================= TimeStamp Helper
        //##############################################################################################################

        //our APIs and drivers work with high-resolution timestamps, with a precision of 1 millisecond.
        //They use QueryPerformanceFrequency and QueryPerformanceCounter for Timestamp generation.As far as I know, 
        //C# doesn't offer these functions, but it has a class, StopWatch (System.Diagnostics), that may also be used for timestamps acquisition. 
        //You can try using StopWatch or using PInvoke for the funcitons mentioned above.
        // https://www.peak-system.com/forum/viewtopic.php?f=41&t=3332&p=9970&hilit=Timestamp#p9970 
        // A single tick represents one hundred nanoseconds or one ten-millionth of a second. There are 10,000 ticks in a millisecond, 
        // Environment.TickCount //Only for mSec.
        // TPCANTimestampFD TxTimestamp = Convert.ToUInt64(itsTimeStamp.micros + 1000 * itsTimeStamp.millis + 0x100000000 * 1000 * itsTimeStamp.millis_overflow);
        #region //=======================================PCAN_GetTimestamp
        public TPCANTimestamp PCAN_GetTimestamp()
        {                                                   
            long capture = Stopwatch.GetTimestamp();                        // Assumed 100nSec resolution, 1mSec/100nSec = 10000.
            TPCANTimestamp timetstamp = new TPCANTimestamp();               
            timetstamp.millis = (uint) (((long)capture) / 10000L);
            timetstamp.micros = (ushort) ((((long)capture) & 9999L)/10);    // Filter out microsecond only with this method. 0 to 999uSec.
            timetstamp.millis_overflow = 0;                                 // Deal this later. 
            return timetstamp;
        }
        #endregion

        #region //=======================================PCAN_GetTimestamp_uSec
        public UInt64 PCAN_GetTimestamp_uSec()
        {
            UInt64 capture = Convert.ToUInt64(Stopwatch.GetTimestamp());   // Assumed 100nSec resolution, 1mSec/100nSec = 10000.
            return (capture / 10L);
        }
        #endregion

        #region //=======================================udelay
        static void udelay(long us)
        {
            var sw = Stopwatch.StartNew();
            long v = (us * Stopwatch.Frequency) / 1000000;
            while (sw.ElapsedTicks < v) { }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= PCAN Terminal/Viewer
        //##############################################################################################################

        #region //==================================================rtPCanTerm_StopRepaint/rtPCanTerm_StartRepaint trick to reduce repaint works while scrolling. 
        // Ref: https://weblogs.asp.net/jdanforth/88458
        // Ref: https://stackoverflow.com/questions/192413/how-do-you-prevent-a-richtextbox-from-refreshing-its-display
        // Worth looking into this: https://stackoverflow.com/questions/6547193/how-to-append-text-to-richtextbox-without-scrolling-and-losing-selection/47574181
        // And https://social.msdn.microsoft.com/Forums/sqlserver/en-US/9ae8374d-5593-4381-8054-158c649882a6/why-when-im-using-wmsetredraw-to-avoid-richtextbox1-flickering-when-updating-the-richtextbox1-text?forum=csharpgeneral
// 
//         [DllImport("user32", CharSet = CharSet.Auto)]
//         private extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
// 
//         private const int WM_USER = 0x0400;
//         private const int EM_SETEVENTMASK = (WM_USER + 69);
//         private const int EM_GETEVENTMASK = (WM_USER + 59);
//         private const int WM_SETREDRAW = 0x0B;
//         private IntPtr OldEventMask;
// 
//         private void rtPCanTerm_StopRepaint()
//         {
//             // Stop redrawing:
//             SendMessage(rtPCanTerm.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
//             // Stop sending of events:
//             OldEventMask = SendMessage(rtPCanTerm.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero);
//         }
// 
//         private void rtPCanTerm_StartRepaint()
//         {
//             // turn on events
//             SendMessage(rtPCanTerm.Handle, EM_SETEVENTMASK, 0, OldEventMask);
//             // turn on redrawing
//             SendMessage(rtPCanTerm.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
//             // this forces a repaint, which for some reason is necessary in some cases.
//             rtPCanTerm.Invalidate();
//         }
        #endregion

        #region//================================================== PCANTerm_ContextMenu_Init
        private void PCANTerm_ContextMenu_Init()
        {
            //https://stackoverflow.com/questions/18966407/enable-copy-cut-past-window-in-a-rich-text-box
            if (rtPCanTerm.ContextMenuStrip == null)
            {
                ContextMenuStrip cms = new ContextMenuStrip { ShowImageMargin = true };
                ToolStripMenuItem tsmiCut = new ToolStripMenuItem("Cut");
                tsmiCut.Click += (sender, e) => rtPCanTerm.Cut();
                cms.Items.Add(tsmiCut);
                ToolStripMenuItem tsmiCopy = new ToolStripMenuItem("Copy");
                tsmiCopy.Click += (sender, e) => rtPCanTerm.Copy();
                cms.Items.Add(tsmiCopy);
                ToolStripMenuItem tsmiPaste = new ToolStripMenuItem("Paste");
                tsmiPaste.Click += (sender, e) => rtPCanTerm.Paste();
                cms.Items.Add(tsmiPaste);
                rtPCanTerm.ContextMenuStrip = cms;
            }

        }
        #endregion

        #region //==================================================myPCANTermMessage
        //==========================================================
        // Purpose  : Append message in Can Log terminal window without LF/CR
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        
        public delegate void myPCANTermMessage_StartDelegate(string Message);
        public void myPCANTermMessage(string Message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new myPCANTermMessage_StartDelegate(myPCANTermMessage), new object[] { Message });
                return;
            }

            //----------------------------------------------------------------------------------
            if ((myGlobalBase.isCanPCANTermUpdateModeTimerActivated == false) & (myGlobalBase.iCanPCANTerminalUpdateMode != (int)GlobalBase.eTerminalUpdate.TermREAL))
            {
                myGlobalBase.CanPCANTermHaltMessageBuf += Message;
                myGlobalBase.isCanPCANTermUpdateModeTimerActivated = true;
                bgwPCANTermUpdate.RunWorkerAsync();
                return;
            }
            //----------------------------------------------------------------------------------
            if (myGlobalBase.iCanPCANTerminalUpdateMode != (int)GlobalBase.eTerminalUpdate.TermREAL)   // Delay Time Terminal refresh. 
            {
                myGlobalBase.CanPCANTermHaltMessageBuf += Message;
                return;
            }
            //----------------------------------------------------------------------------------
            if (myGlobalBase.isCanPCANTermScreenHalted == false)
            {
                if (rtPCanTerm != null)
                {
                    Tools.rtb_StopRepaint(rtPCanTerm, rtbOEM);
                    rtPCanTerm.SelectionColor = myGlobalBase.ColorResponse;
                    rtPCanTerm.AppendText(myGlobalBase.CanPCANTermHaltMessageBuf + Message);
                    Tools.rtb_StartRepaint(rtPCanTerm, rtbOEM);
                    //ScrollToBottom(rtPCanTerm);       // This is done in every 50mSec timer code.
                }
                myGlobalBase.CanPCANTermHaltMessageBuf = "";
            }
            else
            {
                myGlobalBase.CanPCANTermHaltMessageBuf += Message;
            }
        }
        #endregion

        #region //==================================================myRtbTermMessageLF
        //==========================================================
        // Purpose  : Append message in Can Log terminal window with LF/CR
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        public delegate void myPCANTermMessageLF_StartDelegate(string Message);
        public void myPCANTermMessageLF(string Message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new myPCANTermMessageLF_StartDelegate(myPCANTermMessageLF), new object[] { Message });
                return;
            }
            //----------------------------------------------------------------------------------
            if ((myGlobalBase.isCanPCANTermUpdateModeTimerActivated == false) & (myGlobalBase.iCanPCANTerminalUpdateMode != (int)GlobalBase.eTerminalUpdate.TermREAL))
            {
                myGlobalBase.CanPCANTermHaltMessageBuf += Message + "\r\n";
                myGlobalBase.isCanPCANTermUpdateModeTimerActivated = true;
                bgwPCANTermUpdate.RunWorkerAsync();
                return;
            }
            //----------------------------------------------------------------------------------
            if (myGlobalBase.iCanPCANTerminalUpdateMode != (int)GlobalBase.eTerminalUpdate.TermREAL)   // Delay Time Terminal refresh. 
            {
                myGlobalBase.CanPCANTermHaltMessageBuf += Message + "\r\n";
                return;
            }
            //----------------------------------------------------------------------------------
            if (myGlobalBase.isCanPCANTermScreenHalted == false)
            {
                if ((rtPCanTerm.TextLength>30000) | (cbAutoClearGreaterThan30K.Checked==true))
                {
                    //###TASK: Save text to filename before clearing. 
                    rtPCanTerm.Clear();
                }
                if (rtPCanTerm != null)
                {
                    Tools.rtb_StopRepaint(rtPCanTerm, rtbOEM);
                    rtPCanTerm.SelectionColor = myGlobalBase.ColorResponse;
                    rtPCanTerm.AppendText(myGlobalBase.CanPCANTermHaltMessageBuf + Message + "\r\n");
                    Tools.rtb_StartRepaint(rtPCanTerm, rtbOEM);
                    //ScrollToBottom(rtPCanTerm);       // This is done in every 50mSec timer code.
                }
                myGlobalBase.CanPCANTermHaltMessageBuf = "";
            }
            else
            {
                myGlobalBase.CanPCANTermHaltMessageBuf += Message + "\r\n";
            }
        }
        #endregion

        #region//==================================================bgwPCANTermUpdate_DoWork
        private void bgwPCANTermUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            if (bgwPCANTermUpdate.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            switch (myGlobalBase.iCanPCANTerminalUpdateMode)
            {
                case ((int)GlobalBase.eTerminalUpdate.TermSEC1):
                    {
                        Thread.Sleep(1000);
                        break;
                    }
                case ((int)GlobalBase.eTerminalUpdate.TermSEC2):
                    {
                        Thread.Sleep(2000);
                        break;
                    }
                case ((int)GlobalBase.eTerminalUpdate.TermSEC5):
                    {
                        Thread.Sleep(5000);
                        break;
                    }
                default:
                    break;

            }
        }
        #endregion

        #region//==================================================bgwPCANTermUpdate_RunWorkerCompleted
        private void bgwPCANTermUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (myGlobalBase.CanPCANTermHaltMessageBuf != "")
            {
                Tools.rtb_StopRepaint(rtPCanTerm, rtbOEM);
                rtPCanTerm.SelectionColor = myGlobalBase.ColorMessage;
                rtPCanTerm.AppendText(myGlobalBase.CanPCANTermHaltMessageBuf);
                myGlobalBase.CanPCANTermHaltMessageBuf = "";
                Tools.rtb_StartRepaint(rtPCanTerm, rtbOEM);
            }
            //---------------------------------------------------------------------
            
            if (myGlobalBase.iCanPCANTerminalUpdateMode != (int)GlobalBase.eTerminalUpdate.TermREAL)       // Keep looping. 
                bgwPCANTermUpdate.RunWorkerAsync();
            else
                myGlobalBase.isCanPCANTermUpdateModeTimerActivated = false;
        }
        #endregion

        #region//==================================================UpdateRealTimeToolStripMenuItem_Click
        private void UpdateRealTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateRealTimeToolStripMenuItem.CheckState = CheckState.Checked;
            update1SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            update2SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            update5SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            myGlobalBase.iCanPCANTerminalUpdateMode = (int)GlobalBase.eTerminalUpdate.TermREAL;
        }
        #endregion

        #region//==================================================Update1SecondToolStripMenuItem_Click
        private void Update1SecondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateRealTimeToolStripMenuItem.CheckState = CheckState.Unchecked;
            update1SecondToolStripMenuItem.CheckState = CheckState.Checked;
            update2SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            update5SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            myGlobalBase.iCanPCANTerminalUpdateMode = (int)GlobalBase.eTerminalUpdate.TermSEC1;
        }
        #endregion

        #region//==================================================Update2SecondToolStripMenuItem_Click
        private void Update2SecondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateRealTimeToolStripMenuItem.CheckState = CheckState.Unchecked;
            update1SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            update2SecondToolStripMenuItem.CheckState = CheckState.Checked;
            update5SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            myGlobalBase.iCanPCANTerminalUpdateMode = (int)GlobalBase.eTerminalUpdate.TermSEC2;
        }
        #endregion

        #region//==================================================Update5SecondToolStripMenuItem_Click
        private void Update5SecondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateRealTimeToolStripMenuItem.CheckState = CheckState.Unchecked;
            update1SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            update2SecondToolStripMenuItem.CheckState = CheckState.Unchecked;
            update5SecondToolStripMenuItem.CheckState = CheckState.Checked;
            myGlobalBase.iCanPCANTerminalUpdateMode = (int)GlobalBase.eTerminalUpdate.TermSEC5;
        }
        #endregion

        #region //==================================================RtPCanTerm_KeyDown
        private void RtPCanTerm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)           // CNTR+S, save command table (shortcut key)
            {
                SaveLogToolStripMenuItem_Click(null, e);
            }
        }
        #endregion

        #region //==================================================WordWrapToolStripMenuItem_Click
        private void WordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (wordWrapToolStripMenuItem.CheckState == CheckState.Checked)
            {
                rtPCanTerm.WordWrap = false;
                //rtPCanTerm.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
                wordWrapToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            else
            {
                rtPCanTerm.WordWrap = true;
                //rtPCanTerm.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
                wordWrapToolStripMenuItem.CheckState = CheckState.Checked;
            }
        }
        #endregion

        #region //==================================================SaveLogToolStripMenuItem_Click

        private void SaveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mbDialogMessageBox == null)
                mbDialogMessageBox = new DialogSupport();
            //------------------------------------------------
            using (MemoryStream PCAN_MemeoryStream = new MemoryStream())
            {
                
                // Save text to memorystream.
                rtPCanTerm.SaveFile(PCAN_MemeoryStream, RichTextBoxStreamType.PlainText);
                PCAN_MemeoryStream.WriteByte(13);
                //-----------------------------------------------
                string pathString = myGlobalBase.CanPCAN_LogMsg_FilenameName;
                if (myGlobalBase.CanPCAN_LogMsg_FilenameName == "")
                    pathString = myGlobalBase.CanPCAN_LogMsg_FolderName;
                //------------------------------------------------
                SaveFileDialog PCANsaveFileDialog1 = new SaveFileDialog();
                PCANsaveFileDialog1.CreatePrompt = true;
                PCANsaveFileDialog1.OverwritePrompt = true;
                PCANsaveFileDialog1.DefaultExt = "txt";
                PCANsaveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                PCANsaveFileDialog1.Title = "Save Log Message File";
                PCANsaveFileDialog1.InitialDirectory = pathString;
                pathString = System.IO.Path.Combine(pathString, "PCAN_LogMsg_" + Tools.uDateTimeToUnixTimestampNowUTC().ToString() + ".txt");
                PCANsaveFileDialog1.FileName = pathString;
                //------------------------------------------------
                if (PCANsaveFileDialog1.ShowDialog() != DialogResult.OK)
                {
                    myMainProg.myRtbTermMessageLF("#E: User Cancelled 'Save' Operation");
                    return;
                }
                if (PCANsaveFileDialog1.FileName == "")
                    return;
                //------------------------------------------------
                //Path.GetDirectoryName(PCANsaveFileDialog1.FileName);      // Useful tools.
                //Path.GetFileName(PCANsaveFileDialog1.FileName);
                //Path.GetExtension(PCANsaveFileDialog1.FileName);

                myGlobalBase.CanPCAN_LogMsg_FolderName = Path.GetDirectoryName(PCANsaveFileDialog1.FileName);
                myGlobalBase.CanPCAN_LogMsg_FilenameName = PCANsaveFileDialog1.FileName;
                myPCAN_Configuration.sFolderNameLogMsg = Path.GetDirectoryName(PCANsaveFileDialog1.FileName);
                //---------------------------------------------------
                Stream PCANfileStream;
                try
                {
                    PCANfileStream = PCANsaveFileDialog1.OpenFile();
                    PCAN_MemeoryStream.Position = 0;
                    PCAN_MemeoryStream.WriteTo(PCANfileStream);
                    PCANfileStream.Close();
                    
                }
                catch
                {
                    mbDialogMessageBox.PopUpMessageBox("ERROR: Unable to save CAN Config within designated folder, is this protected folder?", "File System Error", 5, 12F);
                    return;
                }
                mbDialogMessageBox.PopUpMessageBox("Successfully saved ToCo Setup File:\n" + PCANsaveFileDialog1.FileName, "Setup File System", 2, 12F);
            } // Within using loop, When finish, the PCAN_MemeoryStream is disposed. 
        }
        #endregion

        #region //==================================================BtnClear_Click
        private void BtnClear_Click(object sender, EventArgs e)
        {
            rtPCanTerm.Text = "";
            rtPCanTerm.Clear();
            rtPCanTerm.Refresh();
        }
        #endregion

        #region //==================================================BtnHalt_Click
        private void BtnHalt_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.isCanPCANTermScreenHalted == false)
            {
                rtPCanTerm.BackColor = System.Drawing.Color.DarkSlateBlue;
                rtPCanTerm.Refresh();
                myGlobalBase.isCanPCANTermScreenHalted = true;

            }
            else
            {
                rtPCanTerm.BackColor = System.Drawing.Color.DarkSlateGray;
                rtPCanTerm.Refresh();
                myGlobalBase.isCanPCANTermScreenHalted = false;
            }

        }
        #endregion

        #region //==================================================PCAN_InsertMessage
        private void PCAN_InsertMessage(string Message)
        {
            UInt64 TimeStamp = PCAN_GetTimestamp_uSec();
            string Msg  = "#I,";
                   Msg     += string.Format("{0,19},", TimeStamp);
                   Msg     += Message;
            myPCANTermMessageLF(Msg);
        }
        #endregion

        #region //==================================================APIVersionToolStripMenuItem_Click
        private void APIVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TPCANStatus stsResult;
            StringBuilder strTemp;
            string[] strArrayVersion;

            strTemp = new StringBuilder(256);

            // We get the version of the PCAN-Basic API
            //
            stsResult = PCANBasic.GetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_API_VERSION, strTemp, 256);
            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                PCAN_InsertMessage("API Version: " + strTemp.ToString());
                myMainProg.myRtbTermMessageLF("-I:PCAN: API Version: " + strTemp.ToString());
                // We get the driver version of the channel being used
                //
                stsResult = PCANBasic.GetValue(m_PcanHandle, TPCANParameter.PCAN_CHANNEL_VERSION, strTemp, 256);
                if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                {
                    // Because this information contains line control characters (several lines)
                    // we split this also in several entries in the Information List-Box
                    //
                    strArrayVersion = strTemp.ToString().Split(new char[] { '\n' });
                    PCAN_InsertMessage("Channel/Driver Version: ");
                    myMainProg.myRtbTermMessageLF("-I:PCAN: Channel/Driver Version:");
                    for (int i = 0; i < strArrayVersion.Length; i++)
                    {
                        PCAN_InsertMessage("     * " + strArrayVersion[i]);
                        myMainProg.myRtbTermMessageLF("-I:PCAN:     * " + strArrayVersion[i]);
                    }
                }
            }

            // If an error occurred, a message is shown
            //
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                MessageBox.Show(GetFormatedError(stsResult));
                myMainProg.myRtbTermMessageLF("#E:PCAN: Error "+ stsResult);
            }
        }
        #endregion

        #region //==================================================StatusToolStripMenuItem_Click
        private void StatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TPCANStatus stsResult;
            String errorName;

            // Gets the current BUS status of a PCAN Channel.
            //
            stsResult = PCANBasic.GetStatus(m_PcanHandle);

            // Switch On Error Name
            //
            switch (stsResult)
            {
                case TPCANStatus.PCAN_ERROR_INITIALIZE:
                    errorName = "PCAN_ERROR_INITIALIZE";
                    break;

                case TPCANStatus.PCAN_ERROR_BUSLIGHT:
                    errorName = "PCAN_ERROR_BUSLIGHT";
                    break;

                case TPCANStatus.PCAN_ERROR_BUSHEAVY: // TPCANStatus.PCAN_ERROR_BUSWARNING
                    errorName = m_IsFD ? "PCAN_ERROR_BUSWARNING" : "PCAN_ERROR_BUSHEAVY";
                    break;

                case TPCANStatus.PCAN_ERROR_BUSPASSIVE:
                    errorName = "PCAN_ERROR_BUSPASSIVE";
                    break;

                case TPCANStatus.PCAN_ERROR_BUSOFF:
                    errorName = "PCAN_ERROR_BUSOFF";
                    break;

                case TPCANStatus.PCAN_ERROR_OK:
                    errorName = "PCAN_ERROR_OK";
                    break;

                default:
                    errorName = "See Documentation";
                    break;
            }

            // Display Message
            //
            PCAN_InsertMessage(String.Format("Status: {0} ({1:X}h)", errorName, stsResult));
        }
        #endregion

        #region //==================================================ResetToolStripMenuItem_Click
        private void ResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TPCANStatus stsResult;

            // Resets the receive and transmit queues of a PCAN Channel.
            //
            stsResult = PCANBasic.Reset(m_PcanHandle);

            // If it fails, a error message is shown
            //
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                PCAN_InsertMessage(GetFormatedError(stsResult));
            else
                PCAN_InsertMessage("Receive and transmit queues successfully reset");
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= PCAN CAN Message Cycle
        //##############################################################################################################

        #region //==================================================PCAN_CycleInit (Setup DataGridView Layout)
        private void PCAN_CycleInit()
        {
            //-----------------------------------------Reset Check box. 
            cbCyclePause.Checked = true;
            cbCycleReset.Checked = false;
            cbCycleRun.Checked = false;
            //-----------------------------------------
            myDataGridCycle = new System.Windows.Forms.DataGridView();
            myDataGridCycle.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            myDataGridCycle.Size = new System.Drawing.Size(657, 454);
            myDataGridCycle.Location = new System.Drawing.Point(2, 25);     //(2, 19);
            myDataGridCycle.Name = "lstLoop";
            myDataGridCycle.TabIndex = 29;
            myDataGridCycle.BackgroundColor = System.Drawing.Color.Gainsboro;
            myDataGridCycle.MultiSelect = false;
            myDataGridCycle.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            myDataGridCycle.RowHeadersVisible = false;
            myDataGridCycle.AllowUserToAddRows = false;
            myDataGridCycle.AllowUserToDeleteRows = false;
            myDataGridCycle.AllowUserToResizeColumns = false;
            myDataGridCycle.AllowUserToResizeRows = false;
            myDataGridCycle.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            myDataGridCycle.RowTemplate.Height = 18;           //21
            myDataGridCycle.RowTemplate.MinimumHeight = 18;    //21
            myDataGridCycle.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.CanTXCycle_CellContentClick);
            myDataGridCycle.CurrentCellDirtyStateChanged += new System.EventHandler(this.CanTXCycle_CurrentCellDirtyStateChanged);
            myDataGridCycle.CellValueChanged += new DataGridViewCellEventHandler(CanTXCycle_CellValueChanged);
            myDataGridCycle.CellMouseDown += new DataGridViewCellMouseEventHandler(CanTXCycle_ColumnMouseDown);
            //---------------------------------------------------------
            CmdColumn = new DataGridViewTextBoxColumn[10];
            //---------------------------------------------------------Button to send CMD once
            DataGridViewButtonColumn btnCANTX = new DataGridViewButtonColumn();
            btnCANTX.Name = "btnCANTX";
            btnCANTX.HeaderText = "TX";
            btnCANTX.Width = 30;
            btnCANTX.Resizable = DataGridViewTriState.False;
            btnCANTX.SortMode = DataGridViewColumnSortMode.NotSortable;
            btnCANTX.ReadOnly = false;
            myDataGridCycle.Columns.Add(btnCANTX);
            //---------------------------------------------------------CheckBox for repeat Loop
            DataGridViewCheckBoxColumn cbCANCycle = new DataGridViewCheckBoxColumn();
            cbCANCycle.Name = "cbCANCycle";
            cbCANCycle.HeaderText = "CY";
            cbCANCycle.Width = 30;
            cbCANCycle.Resizable = DataGridViewTriState.False;
            cbCANCycle.SortMode = DataGridViewColumnSortMode.NotSortable;
            cbCANCycle.ReadOnly = false;
            myDataGridCycle.Columns.Add(cbCANCycle);
            //--------------------------------------------------------- 
            DataGridViewTextBoxColumn txCANType = new DataGridViewTextBoxColumn();
            txCANType.Width = 50;
            txCANType.HeaderText = "Type";
            txCANType.ReadOnly = true;
            myDataGridCycle.Columns.Add(txCANType);
            DataGridViewTextBoxColumn txCANID = new DataGridViewTextBoxColumn();
            txCANID.Width = 50;
            txCANID.HeaderText = "ID";
            txCANID.ReadOnly = true;
            myDataGridCycle.Columns.Add(txCANID);
            DataGridViewTextBoxColumn txCANLength = new DataGridViewTextBoxColumn();
            txCANLength.Width = 50;
            txCANLength.HeaderText = "DLC";
            txCANLength.ReadOnly = true;
            myDataGridCycle.Columns.Add(txCANLength);
            DataGridViewTextBoxColumn txCANCount = new DataGridViewTextBoxColumn();
            txCANCount.Width = 50;
            txCANCount.HeaderText = "Count";
            txCANCount.ReadOnly = true;
            myDataGridCycle.Columns.Add(txCANCount);
            DataGridViewTextBoxColumn txCANRCVDelay = new DataGridViewTextBoxColumn();
            txCANRCVDelay.Width = 70;
            txCANRCVDelay.HeaderText = "Delay(mS)";
            txCANRCVDelay.ReadOnly = true;
            myDataGridCycle.Columns.Add(txCANRCVDelay);
            DataGridViewTextBoxColumn txCANPeriod = new DataGridViewTextBoxColumn();
            txCANPeriod.Width = 70;
            txCANPeriod.HeaderText = "Period(mS)";
            txCANPeriod.ReadOnly = true;
            myDataGridCycle.Columns.Add(txCANPeriod);
            DataGridViewTextBoxColumn txCANData = new DataGridViewTextBoxColumn();
            txCANData.Width = 170;
            txCANData.HeaderText = "Data";
            txCANData.ReadOnly = true;
            myDataGridCycle.Columns.Add(txCANData);
            DataGridViewTextBoxColumn txCANComment = new DataGridViewTextBoxColumn();
            txCANComment.Width = 300;
            txCANComment.HeaderText = "Comment";
            txCANComment.ReadOnly = false;
            myDataGridCycle.Columns.Add(txCANComment);
            //--------------------------------------------Context Menu
            TmPCABCycle_SetupContextMenu();
            tbTabCycleTXWindow_SetupContextMenu();
            //--------------------------------------------Finally
            tbTabCycleTXWindow.Controls.Add(this.myDataGridCycle);
        }
        #endregion

        #region //==================================================tbTabCycleTXWindow_SetupContextMenu
        private void tbTabCycleTXWindow_SetupContextMenu()
        {
            tbTabCycleTXWindow.ContextMenu = new ContextMenu();
            tbTabCycleTXWindow.ContextMenu.MenuItems.Add("Add", new EventHandler(evTXCycleAdd));
            tbTabCycleTXWindow.ContextMenu.MenuItems.Add("Load TX-List", new EventHandler(evTXCycleLoadCMD));
            tbTabCycleTXWindow.ContextMenu.MenuItems.Add("Gamma Example", new EventHandler(GenerateExample_Click));
        }
        #endregion

        #region //==================================================TmPCABCycle_SetupContextMenu
        private void TmPCABCycle_SetupContextMenu()
        {
            myDataGridCycle.ContextMenu = new ContextMenu();
            myDataGridCycle.ContextMenu.MenuItems.Add("Edit", new EventHandler(evTXCycleEdit));
            myDataGridCycle.ContextMenu.MenuItems.Add("Add", new EventHandler(evTXCycleAdd));
            myDataGridCycle.ContextMenu.MenuItems.Add("Remove", new EventHandler(evTXCycleRemove));
            myDataGridCycle.ContextMenu.MenuItems.Add("Save TX-List", new EventHandler(evTXCycleSaveCMD));
            myDataGridCycle.ContextMenu.MenuItems.Add("Load TX-List", new EventHandler(evTXCycleLoadCMD));
            myDataGridCycle.ContextMenu.MenuItems.Add("Generate CL(..)", new EventHandler(evTXCycleGenerateCL));
            myDataGridCycle.ContextMenu.MenuItems.Add("Generate CS(..)", new EventHandler(evTXCycleGenerateCS));
        }
        #endregion

        #region //==================================================CbCycle_MouseClick
        private void CbCycle_MouseClick(object sender, MouseEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.Name.Equals("cbCyclePause"))
            {
                if (checkbox.Checked == false)
                    checkbox.Checked = true;
                cbCycleReset.Checked = false;
                cbCycleRun.Checked = false;
                myPCANTermMessageLF("-I: Cycle Counter Paused (But not reset)");
                tmPCanOnemSec_Stop();
            }
            if (checkbox.Name.Equals("cbCycleReset"))       // This restart the counter. 
            {
                if (checkbox.Checked == true)
                {
                    checkbox.Checked = false;
                    cbCyclePause.Checked = true;
                    cbCycleRun.Checked = false;
                    tmPCanOnemSec_Stop();
                    foreach (PCAN_StdFrame myPCAN_StdFrame in myCycleArray)
                    {
                        myPCAN_StdFrame.ResetCycleParam();
                    }
                    myPCANTermMessageLF("-I: Cycle Counter Stop & Reset");
                }
            }
            if (checkbox.Name.Equals("cbCycleRun"))
            {
                if (checkbox.Checked == false)
                    checkbox.Checked = true;
                cbCyclePause.Checked = false;
                cbCycleReset.Checked = false;
                myPCANTermMessageLF("-I: Cycle Counter Run");
                tmPCanOnemSec_Start();
            }
        }
        #endregion

        #region//==================================================CanTXCycle_CurrentCellDirtyStateChanged
        private void CanTXCycle_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (myDataGridCycle.IsCurrentCellDirty)
            {
                myDataGridCycle.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        #endregion

        #region//==================================================CanTXCycle_CellValueChanged (CheckBox Only)
        public void CanTXCycle_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;           // Also channel number. 
            int column = e.ColumnIndex;
            if (column != 1)                // Only checkbox is clickable!
                return;       
            if (row < 0)                    // Protection. 
                return;
            //-------------------------------------------------------------------------
            if (myDataGridCycle.Columns[e.ColumnIndex].Name == "cbCANCycle")
            {
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)myDataGridCycle.Rows[row].Cells[1];
                if (checkCell != null)
                {
                    if (checkCell.Value.Equals(true))
                    {
                        myCycleArray[row].isCycleEnable = true;
                        myCycleArray[row].ResetCycleParam();
                    }
                    else
                    {
                        myCycleArray[row].isCycleEnable = false;
                    }
                }
            }
        }
        #endregion

        #region//==================================================CmdBox_CellContentClick (Button Only)
        private void CanTXCycle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;           // Also channel number. 
            int column = e.ColumnIndex;
            if (column != 0)                 // Only button is clickable!
                return;
            if (row < 0)                    // Protection.
                return;
            //-------------------------------------------------------------------------
            if (myDataGridCycle.Columns[0].Name == "btnCANTX")
            {
                PCAN_WriteFrame(myCycleArray[row], TPCANMessageType.PCAN_MESSAGE_STANDARD);
            }
        }
        #endregion

        #region //==================================================Button1_Click(test only)
        private void GenerateExample_Click(object sender, EventArgs e)
        {
            PCAN_StdFrame myCycle1 = new PCAN_StdFrame();
            myCycle1.CanID = 0x1C1;
            myCycle1.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            myCycle1.DLC = 5;
            myCycle1.DATA[0] = 0x00;
            myCycle1.DATA[1] = 0x00;
            myCycle1.DATA[2] = 0x01;
            myCycle1.DATA[3] = 0x00;
            myCycle1.DATA[4] = 0x00;
            myCycle1.CycleNumber = 10;          // 10 loop
            myCycle1.CyclePeriod = 1000;        // 1Sec.
            myCycle1.Delay = 0;                 // No Delay. 
            myCycle1.sComment = "Gamma: Survey Request";
            PCAN_Cycle_Add_List(myCycle1);
            //------------------------------------------------------------------
            myCycle1.Clear();
            myCycle1.CanID = 0x620;
            myCycle1.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            myCycle1.DLC = 8;
            myCycle1.DATA[0] = 0x2B;
            myCycle1.DATA[1] = 0x20;
            myCycle1.DATA[2] = 0x40;
            myCycle1.DATA[3] = 0x05;
            myCycle1.DATA[4] = 0xDC;
            myCycle1.DATA[5] = 0x05;
            myCycle1.DATA[6] = 0x00;
            myCycle1.DATA[7] = 0x00;
            myCycle1.CycleNumber = 0;          
            myCycle1.CyclePeriod = 0;        
            myCycle1.Delay = 500;                
            myCycle1.sComment = "Gamma: Set 1500V and Run";
            PCAN_Cycle_Add_List(myCycle1);
            if (cbCycleRun.Checked==true)
                tmPCanOnemSec_Start();
        }
        #endregion

        #region //==================================================PCAN_Cycle_Add_List
        // The myDataGridCycle.Rows[1] should be in sync with myCycleArray[0]
        private bool PCAN_Cycle_Add_List(PCAN_StdFrame pCANEntry)
        {
            if (myCycleArray == null)
                myCycleArray = new List<PCAN_StdFrame>();
            //----------------------------------------------------
            myCycleArray.Add(new PCAN_StdFrame());
            int ch = myCycleArray.Count-1;
            myCycleArray[ch].MyDataGridCycle(myDataGridCycle);      // Reference datagridview object to array (to sync together)
            //----------------------------------------------------
            if (pCANEntry.CycleNumber == 0)
                myCycleArray[ch].isCycleEnable = false;
            else
                myCycleArray[ch].isCycleEnable = true;
            //----------------------------------------------------Copy pCANEntry into Array.
            myCycleArray[ch].CanID          = pCANEntry.CanID;
            myCycleArray[ch].DLC            = pCANEntry.DLC;
            myCycleArray[ch].MSGTYPE        = pCANEntry.MSGTYPE;
            myCycleArray[ch].Delay          = pCANEntry.Delay;
            myCycleArray[ch].CycleNumber    = pCANEntry.CycleNumber;
            myCycleArray[ch].CyclePeriod    = pCANEntry.CyclePeriod;
            myCycleArray[ch].sComment       = pCANEntry.sComment;
            for (int i = 0; i < 8; i++)
                myCycleArray[ch].DATA[i] = pCANEntry.DATA[i];
            myCycleArray[ch].ProcessAddNewChannel(ch);
            return true;
        }
        #endregion

        #region //==================================================PCAN_Cycle_Remove_List
        private bool PCAN_Cycle_Remove_List()
        {
            try
            {
                myCycleArray.RemoveAt(TXDGV_SelectedCellIndex.RowIndex);
                myDataGridCycle.Rows.RemoveAt(TXDGV_SelectedCellIndex.RowIndex);
            }
            catch
            {
                return (false);
            }
            return (true);
        }
        #endregion

        #region //==================================================CbCycle_MouseClick
        private void CbCycleClearAll_MouseClick(object sender, MouseEventArgs e)
        {
            cbCycleClearAll.Checked = false;
            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            var dialogResult = FlexiMessageBox.Show("Warning, All of this in the list will be lost?", "PCB Message Cycle", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.OK)
            { 
                myCycleArray.Clear();
                myDataGridCycle.Rows.Clear();
                myDataGridCycle.Invalidate();
            }
        }
        #endregion

        #region //==================================================CanTXCycle_ColumnMouseDown
        private void CanTXCycle_ColumnMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if ((e.RowIndex >= 0) & (e.ColumnIndex >= 0))       // Avoid header. 
                    {
                        myDataGridCycle.ClearSelection();
                        //this.myDataGridCycle.Rows[e.RowIndex].Selected = true;
                        this.myDataGridCycle.CurrentCell = this.myDataGridCycle.Rows[e.RowIndex].Cells[e.ColumnIndex];
                        TXDGV_SelectedCellIndex = this.myDataGridCycle.CurrentCell;         //Save this for later code.
                        //-----------------------------------------------------------Working solution to position context box to cell. 
                        System.Drawing.Rectangle cellRect = this.myDataGridCycle.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                        Point Position = new System.Drawing.Point(cellRect.Left, cellRect.Bottom);
                        
                        myDataGridCycle.ContextMenu.Show(this.myDataGridCycle, Position);
                        
                    }
                }
            }
            catch { }
        }
        #endregion

        #region //==================================================evTXCycleSaveAll
        private void evTXCycleSaveCMD(object sender, EventArgs e)
        {
            TXCyle_ExportCommandList();
        }
        #endregion

        #region //==================================================evTXCycleLoadCMD
        private void evTXCycleLoadCMD(object sender, EventArgs e)
        {
            TXCyle_ImportCommandList();
        }
        #endregion

        #region //==================================================evTXCycleGenerateCL
        private void evTXCycleGenerateCL(object sender, EventArgs e)
        {
            int row = TXDGV_SelectedCellIndex.RowIndex;
            string[] sCL = myCycleArray[row].Message_Generate_CL();
            TabTool_Insert_CMD_Comment(sCL, -1);
        }
        #endregion

        #region //==================================================evTXCycleGenerateCS
        private void evTXCycleGenerateCS(object sender, EventArgs e)
        {
            int row = TXDGV_SelectedCellIndex.RowIndex;
            string[] sCS = myCycleArray[row].Message_Generate_CS();
            TabTool_Insert_CMD_Comment(sCS, -1);
        }
        #endregion

        #region //==================================================evTXCycleRemove
        private void evTXCycleRemove(object sender, EventArgs e)
        {
            PCAN_Cycle_Remove_List();
        }
        #endregion

        #region //==================================================evTXCycleAdd
        private void evTXCycleAdd(object sender, EventArgs e)
        {
            PCAN_StdFrame myCycle1 = new PCAN_StdFrame();
            myCycle1.CanID = 0x7FF;
            myCycle1.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            myCycle1.DLC = 8;
            myCycle1.DATA[0] = 0x00;
            myCycle1.DATA[1] = 0x00;
            myCycle1.DATA[2] = 0x00;
            myCycle1.DATA[3] = 0x00;
            myCycle1.DATA[4] = 0x00;
            myCycle1.DATA[5] = 0x00;
            myCycle1.DATA[6] = 0x00;
            myCycle1.DATA[7] = 0x00;
            myCycle1.CycleNumber = 0;          // 10 loop
            myCycle1.CyclePeriod = 1000;        // 1Sec.
            myCycle1.Delay = 0;                 // No Delay. 
            myCycle1.isCycleEnable = false;
            myCycle1.sComment = "User Added Item";
            PCAN_Cycle_Add_List(myCycle1);
        }
        #endregion

        #region //==================================================evTXCycleEdit
        private void evTXCycleEdit(object sender, EventArgs e)
        {
            if (myCanBus==null)
                myCanBus = new CanBus();
            EditSelectedRow = TXDGV_SelectedCellIndex.RowIndex;
            myDataGridCycle.Rows[EditSelectedRow].Selected = true;
            myCanBus.CanFrame_Open_Editor(myCycleArray[EditSelectedRow], this);
            
        }
        #endregion

        #region //==================================================CanFrame_Editor_Done
        public void  CanFrame_Editor_Done(ref PCAN_StdFrame UpdatedCan )
        {
            myCycleArray[EditSelectedRow].MSGTYPE = UpdatedCan.MSGTYPE;
            myCycleArray[EditSelectedRow].CanID = UpdatedCan.CanID;
            myCycleArray[EditSelectedRow].DLC = UpdatedCan.DLC;
            for (int i=0; i<8; i++)
            {
                myCycleArray[EditSelectedRow].DATA[i] = UpdatedCan.DATA[i];
            }
            myCycleArray[EditSelectedRow].CycleNumber = UpdatedCan.CycleNumber;
            myCycleArray[EditSelectedRow].CyclePeriod = UpdatedCan.CyclePeriod;
            myCycleArray[EditSelectedRow].Delay = UpdatedCan.Delay;                 // No Delay. 
            myCycleArray[EditSelectedRow].isCycleEnable = UpdatedCan.isCycleEnable;
            myCycleArray[EditSelectedRow].sComment = UpdatedCan.sComment;
            myDataGridCycle.Rows[EditSelectedRow].Selected = false ;
            myCycleArray[EditSelectedRow].ProcessChannelRefresh(true);
        }
        #endregion

        #region//==================================================TXCyle_ExportCommandList
        private void TXCyle_ExportCommandList()
        {
            tcPCANViewer.Refresh();

            if (mbDialogMessageBox == null)
                mbDialogMessageBox = new DialogSupport();
            //------------------------------------------------
            string pathString = myGlobalBase.CanPCAN_TXCycleList_FolderName;
            if (myGlobalBase.CanPCAN_TXCycleList_FilenameName == "")
                pathString = myGlobalBase.CanPCAN_TXCycleList_FolderName;
            //------------------------------------------------
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Xml|*.xml";
            saveFileDialog1.Title = "Save CAN TX/Cycle Command List File";
            saveFileDialog1.InitialDirectory = pathString;
            pathString = System.IO.Path.Combine(pathString, "PCAN_CMD_LIST_" + Tools.uDateTimeToUnixTimestampNowUTC().ToString() + ".xml");
            saveFileDialog1.FileName = pathString;
            //------------------------------------------------
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                myMainProg.myRtbTermMessageLF("#E: User Cancelled 'Save' Operation");
                return;
            }
            if (saveFileDialog1.FileName == "")
                return;
            //------------------------------------------------ Update folder/filename.
            myGlobalBase.CanPCAN_TXCycleList_FolderName = Path.GetDirectoryName(saveFileDialog1.FileName);
            myGlobalBase.CanPCAN_TXCycleList_FilenameName = saveFileDialog1.FileName;
            //---------------------------------------------------
            try
            {
                //http://www.java2s.com/Code/CSharp/XML/SerializeListofObjects.htm 
                var fStream = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
                var xmlForamt = new XmlSerializer(typeof(List<PCAN_StdFrame>), new Type[] { typeof(PCAN_StdFrame)});
                xmlForamt.Serialize(fStream, myCycleArray);
                fStream.Close();
            }
            catch
            {
                mbDialogMessageBox.PopUpMessageBox("ERROR: Unable to save Cmd List within designated folder, is this protected folder?", "File System Error", 5, 12F);
                return;
            }
            mbDialogMessageBox.PopUpMessageBox("Successfully saved ToCo Cmd List:\n" + saveFileDialog1.FileName, "Setup File System", 2, 12F);
        }
        #endregion

        #region //==================================================TXCyle_ImportCommandList
        private void TXCyle_ImportCommandList()
        {
            if (mbDialogMessageBox == null)
                mbDialogMessageBox = new DialogSupport();
            //------------------------------------------------
            string sFoldername = myGlobalBase.CanPCAN_TXCycleList_FolderName;
            string sFilename = myGlobalBase.CanPCAN_TXCycleList_FilenameName;
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                OpenFileDialog ofd = openFileDialog;
                ofd.Title = "Load CAN TX/Cycle Command List File";
                ofd.Filter = "xml files|*.xml";
                ofd.InitialDirectory = sFoldername;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    sFilename = ofd.FileName;
                    sFoldername = System.IO.Path.GetDirectoryName(ofd.FileName);
                    myGlobalBase.CanPCAN_Config_FilenameName = sFilename;
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
            myGlobalBase.CanPCAN_TXCycleList_FolderName = Path.GetDirectoryName(sFilename);
            myGlobalBase.CanPCAN_TXCycleList_FilenameName = sFilename;
            try
            {
                //http://www.java2s.com/Code/CSharp/XML/SerializeListofObjects.htm 
                myCycleArray.Clear();
                myDataGridCycle.Rows.Clear();
                var xmlForamt = new XmlSerializer(typeof(List<PCAN_StdFrame>), new Type[] { typeof(PCAN_StdFrame) });
                var fStream = File.OpenRead(sFilename);
                //var fStream = new FileStream(sFilename, FileMode.Open);
                myCycleArray = (List<PCAN_StdFrame>)(xmlForamt.Deserialize(fStream));
                fStream.Close();
                for (int i=0; i< myCycleArray.Count; i++)
                {
                    myCycleArray[i].MyDataGridCycle(myDataGridCycle);
                    myCycleArray[i].ProcessAddNewChannel(i);
                }
            }
            catch
            {
                mbDialogMessageBox.PopUpMessageBox("ERROR: Unable to load Cmd List within designated folder, may not exist?", "File System Error", 5, 12F);
                return;
            }
            mbDialogMessageBox.PopUpMessageBox("Successfully Loaded Cmd List File", "File System", 2, 14F);
            myGlobalBase.CanPCAN_Refresh = true;
            this.Invalidate();
            this.Refresh();

        }
        #endregion


        //##############################################################################################################
        //============================================================================================= PEAK PCAN Code 
        //##############################################################################################################

        //============================================================================================= 

        #region //=======================================Delegates
        /// <summary>
        /// Read-Delegate Handler
        /// </summary>
        private delegate void ReadDelegateHandler();
        #endregion

        #region //=======================================Members
        /// <summary>
        /// Saves the desired connection mode
        /// </summary>
        private bool m_IsFD;
        /// <summary>
        /// Saves the handle of a PCAN hardware
        /// </summary>
        private TPCANHandle m_PcanHandle;
        /// <summary>
        /// Saves the baudrate register for a conenction
        /// </summary>
        private TPCANBaudrate m_Baudrate;
        /// <summary>
        /// Saves the type of a non-plug-and-play hardware
        /// </summary>
        private TPCANType m_HwType;
        /// <summary>
        /// Stores the status of received messages for its display
        /// </summary>
        private System.Collections.ArrayList m_LastMsgsList;
        /// <summary>
        /// Read Delegate for calling the function "ReadMessages"
        /// </summary>
        private ReadDelegateHandler m_ReadDelegate;
        /// <summary>
        /// Receive-Event
        /// </summary>
        private System.Threading.AutoResetEvent m_ReceiveEvent;
        /// <summary>
        /// Thread for message reading (using events)
        /// </summary>
        private System.Threading.Thread m_ReadThread;
        /// <summary>
        /// Handles of the current available PCAN-Hardware
        /// </summary>
        private TPCANHandle[] m_HandlesArray;
        #endregion

        #region //=======================================Help functions

        /// <summary>
        /// Convert a CAN DLC value into the actual data length of the CAN/CAN-FD frame.
        /// </summary>
        /// <param name="dlc">A value between 0 and 15 (CAN and FD DLC range)</param>
        /// <param name="isSTD">A value indicating if the msg is a standard CAN (FD Flag not checked)</param>
        /// <returns>The length represented by the DLC</returns>
        public static int GetLengthFromDLC(int dlc, bool isSTD)
        {
            if (dlc <= 8)
                return dlc;

            if (isSTD)
                return 8;

            switch (dlc)
            {
                case 9: return 12;
                case 10: return 16;
                case 11: return 20;
                case 12: return 24;
                case 13: return 32;
                case 14: return 48;
                case 15: return 64;
                default: return dlc;
            }
        }

        /// <summary>
        /// Initialization of PCAN-Basic components
        /// </summary>
        private void InitializeBasicComponents()
        {
            // Creates the list for received messages
            //
            m_LastMsgsList = new System.Collections.ArrayList();
            // Creates the delegate used for message reading
            //
            m_ReadDelegate = new ReadDelegateHandler(ReadMessages);
            // Creates the event used for signalize incoming messages 
            //
            m_ReceiveEvent = new System.Threading.AutoResetEvent(false);
            // Creates an array with all possible PCAN-Channels
            //
            m_HandlesArray = new TPCANHandle[]
            {
                PCANBasic.PCAN_ISABUS1,
                PCANBasic.PCAN_ISABUS2,
                PCANBasic.PCAN_ISABUS3,
                PCANBasic.PCAN_ISABUS4,
                PCANBasic.PCAN_ISABUS5,
                PCANBasic.PCAN_ISABUS6,
                PCANBasic.PCAN_ISABUS7,
                PCANBasic.PCAN_ISABUS8,
                PCANBasic.PCAN_DNGBUS1,
                PCANBasic.PCAN_PCIBUS1,
                PCANBasic.PCAN_PCIBUS2,
                PCANBasic.PCAN_PCIBUS3,
                PCANBasic.PCAN_PCIBUS4,
                PCANBasic.PCAN_PCIBUS5,
                PCANBasic.PCAN_PCIBUS6,
                PCANBasic.PCAN_PCIBUS7,
                PCANBasic.PCAN_PCIBUS8,
                PCANBasic.PCAN_PCIBUS9,
                PCANBasic.PCAN_PCIBUS10,
                PCANBasic.PCAN_PCIBUS11,
                PCANBasic.PCAN_PCIBUS12,
                PCANBasic.PCAN_PCIBUS13,
                PCANBasic.PCAN_PCIBUS14,
                PCANBasic.PCAN_PCIBUS15,
                PCANBasic.PCAN_PCIBUS16,
                PCANBasic.PCAN_USBBUS1,
                PCANBasic.PCAN_USBBUS2,
                PCANBasic.PCAN_USBBUS3,
                PCANBasic.PCAN_USBBUS4,
                PCANBasic.PCAN_USBBUS5,
                PCANBasic.PCAN_USBBUS6,
                PCANBasic.PCAN_USBBUS7,
                PCANBasic.PCAN_USBBUS8,
                PCANBasic.PCAN_USBBUS9,
                PCANBasic.PCAN_USBBUS10,
                PCANBasic.PCAN_USBBUS11,
                PCANBasic.PCAN_USBBUS12,
                PCANBasic.PCAN_USBBUS13,
                PCANBasic.PCAN_USBBUS14,
                PCANBasic.PCAN_USBBUS15,
                PCANBasic.PCAN_USBBUS16,
                PCANBasic.PCAN_PCCBUS1,
                PCANBasic.PCAN_PCCBUS2,
                PCANBasic.PCAN_LANBUS1,
                PCANBasic.PCAN_LANBUS2,
                PCANBasic.PCAN_LANBUS3,
                PCANBasic.PCAN_LANBUS4,
                PCANBasic.PCAN_LANBUS5,
                PCANBasic.PCAN_LANBUS6,
                PCANBasic.PCAN_LANBUS7,
                PCANBasic.PCAN_LANBUS8,
                PCANBasic.PCAN_LANBUS9,
                PCANBasic.PCAN_LANBUS10,
                PCANBasic.PCAN_LANBUS11,
                PCANBasic.PCAN_LANBUS12,
                PCANBasic.PCAN_LANBUS13,
                PCANBasic.PCAN_LANBUS14,
                PCANBasic.PCAN_LANBUS15,
                PCANBasic.PCAN_LANBUS16,
            };

            // Prepares the PCAN-Basic's debug-Log file
            //
            if (myGlobalBase.isPCANBasicInstalled == true)
                ConfigureLogFile();
        }

        /// <summary>
        /// Configures the Debug-Log file of PCAN-Basic
        /// </summary>
        private void ConfigureLogFile()
        {
            UInt32 iBuffer;

            // Sets the mask to catch all events
            //
            iBuffer = PCANBasic.LOG_FUNCTION_ALL;

            // Configures the log file. 
            // NOTE: The Log capability is to be used with the NONEBUS Handle. Other handle than this will 
            // cause the function fail.
            //
            try
            {
                PCANBasic.SetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_LOG_CONFIGURE, ref iBuffer, sizeof(UInt32));
            }
            catch
            {
                if (mbDialogMessageBox == null)
                    mbDialogMessageBox = new DialogSupport();
                mbDialogMessageBox.PopUpMessageBox("PCANBasic.dll not installed (https://www.peak-system.com/PCAN-Basic.239.0.html?&L=1 and download PCAN Basic API). Otherwise do not use PCAN", "Missing PCANBasic.dll driver", 5, 12F);
            }
        }

        /// <summary>
        /// Configures the PCAN-Trace file for a PCAN-Basic Channel
        /// </summary>
        private void ConfigureTraceFile()
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;

            // Configure the maximum size of a trace file to 5 megabytes
            //
            iBuffer = 5;
            stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_TRACE_SIZE, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                PCAN_InsertMessage(GetFormatedError(stsResult));
            // Configure the way how trace files are created: 

            // * Standard name is used
            // * Existing file is ovewritten, 
            // * Only one file is created.
            // * Recording stopts when the file size reaches 5 megabytes.
            //
            iBuffer = PCANBasic.TRACE_FILE_SINGLE | PCANBasic.TRACE_FILE_OVERWRITE;
            stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_TRACE_CONFIGURE, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                PCAN_InsertMessage(GetFormatedError(stsResult));
        }

        /// <summary>
        /// Help Function used to get an error as text
        /// </summary>
        /// <param name="error">Error code to be translated</param>
        /// <returns>A text with the translated error</returns>
        private string GetFormatedError(TPCANStatus error)
        {
            StringBuilder strTemp;

            // Creates a buffer big enough for a error-text
            //
            strTemp = new StringBuilder(256);
            // Gets the text using the GetErrorText API function
            // If the function success, the translated error is returned. If it fails,
            // a text describing the current error is returned.
            //
            if (PCANBasic.GetErrorText(error, 0, strTemp) != TPCANStatus.PCAN_ERROR_OK)
                return string.Format("An error occurred. Error-code's text ({0:X}) couldn't be retrieved", error);
            else
                return strTemp.ToString();
        }

        /// <summary>
        /// Gets the current status of the PCAN-Basic message filter
        /// </summary>
        /// <param name="status">Buffer to retrieve the filter status</param>
        /// <returns>If calling the function was successfull or not</returns>
        private bool GetFilterStatus(out uint status)
        {
            TPCANStatus stsResult;

            // Tries to get the sttaus of the filter for the current connected hardware
            //
            stsResult = PCANBasic.GetValue(m_PcanHandle, TPCANParameter.PCAN_MESSAGE_FILTER, out status, sizeof(UInt32));

            // If it fails, a error message is shown
            //
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                MessageBox.Show(GetFormatedError(stsResult));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Activates/deaactivates the different controls of the main-form according
        /// with the current connection status
        /// </summary>
        /// <param name="bConnected">Current status. True if connected, false otherwise</param>
        private void SetConnectionStatus(bool bConnected)
        {
            // Buttons
            //
            btnRead.Enabled = bConnected && rdbManual.Checked;
            //btnReset.Enabled = bConnected;

            // Check-Buttons
            //

            // Hardware configuration and read mode
            //
            if (!bConnected)
                cbbChannel_SelectedIndexChanged(this, new EventArgs());
            else
                rdbTimer_CheckedChanged(this, new EventArgs());

            // Display messages in grid
            //
            tmrDisplay.Enabled = bConnected;
        }

        /// <summary>
        /// Gets the formated text for a PCAN-Basic channel handle
        /// </summary>
        /// <param name="handle">PCAN-Basic Handle to format</param>
        /// <param name="isFD">If the channel is FD capable</param>
        /// <returns>The formatted text for a channel</returns>
        private string FormatChannelName(TPCANHandle handle, bool isFD)
        {
            TPCANDevice devDevice;
            byte byChannel;

            // Gets the owner device and channel for a 
            // PCAN-Basic handle
            //
            if (handle < 0x100)
            {
                devDevice = (TPCANDevice)(handle >> 4);
                byChannel = (byte)(handle & 0xF);
            }
            else
            {
                devDevice = (TPCANDevice)(handle >> 8);
                byChannel = (byte)(handle & 0xFF);
            }

            // Constructs the PCAN-Basic Channel name and return it
            //
            if (isFD)
                return string.Format("{0}:FD {1} ({2:X2}h)", devDevice, byChannel, handle);
            else
                return string.Format("{0} {1} ({2:X2}h)", devDevice, byChannel, handle);
        }

        /// <summary>
        /// Gets the formated text for a PCAN-Basic channel handle
        /// </summary>
        /// <param name="handle">PCAN-Basic Handle to format</param>
        /// <returns>The formatted text for a channel</returns>
        private string FormatChannelName(TPCANHandle handle)
        {
            return FormatChannelName(handle, false);
        }
        #endregion

        #region //=======================================Message - processing functions

        /// <summary>
        /// Thread-Function used for reading PCAN-Basic messages
        /// </summary>
        private void CANReadThreadFunc()
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;

            iBuffer = Convert.ToUInt32(m_ReceiveEvent.SafeWaitHandle.DangerousGetHandle().ToInt32());
            // Sets the handle of the Receive-Event.
            //
            stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_RECEIVE_EVENT, ref iBuffer, sizeof(UInt32));

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                MessageBox.Show(GetFormatedError(stsResult), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // While this mode is selected
            while (rdbEvent.Checked)
            {
                // Waiting for Receive-Event
                // 
                if (m_ReceiveEvent.WaitOne(50))
                    // Process Receive-Event using .NET Invoke function
                    // in order to interact with Winforms UI (calling the 
                    // function ReadMessages)
                    // 
                    this.Invoke(m_ReadDelegate);
            }
        }

        /// <summary>
        /// Function for reading messages on FD devices
        /// </summary>
        /// <returns>A TPCANStatus error code</returns>
        private TPCANStatus ReadMessageFD()
        {
            TPCANMsgFD CANMsg;
            TPCANTimestampFD CANTimeStamp;
            TPCANStatus stsResult;

            // We execute the "Read" function of the PCANBasic                
            //
            stsResult = PCANBasic.ReadFD(m_PcanHandle, out CANMsg, out CANTimeStamp);
            if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                // We process the received message
                //
                ProcessMessage(CANMsg, CANTimeStamp);

            return stsResult;
        }

        #endregion

        //============================================================================================= Form Related Code

        #region //=======================================cbbChannel_SelectedIndexChanged
        private void cbbChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
//             bool bNonPnP;
//             string strTemp;
// 
//             // Get the handle fromt he text being shown
//             //
//             strTemp = cbbChannel.Text;
//             strTemp = strTemp.Substring(strTemp.IndexOf('(') + 1, 3);
// 
//             strTemp = strTemp.Replace('h', ' ').Trim(' ');
// 
//             // Determines if the handle belong to a No Plug&Play hardware 
//             //
//             m_PcanHandle = Convert.ToUInt16(strTemp, 16);
// //             bNonPnP = m_PcanHandle <= PCANBasic.PCAN_DNGBUS1;
// //             // Activates/deactivates configuration controls according with the 
// //             // kind of hardware
// //             //
// //             cbbHwType.Enabled = bNonPnP;
// //             cbbIO.Enabled = bNonPnP;
// //             cbbInterrupt.Enabled = bNonPnP;
        }
        #endregion

        #region //=======================================btnHwRefresh_Click
        private void btnHwRefresh_Click(object sender, EventArgs e)
        {
//             UInt32 iBuffer;
//             TPCANStatus stsResult;
//             bool isFD;
// 
//             // Clears the Channel combioBox and fill it again with 
//             // the PCAN-Basic handles for no-Plug&Play hardware and
//             // the detected Plug&Play hardware
//             //
//             cbbChannel.Items.Clear();
//             try
//             {
//                 for (int i = 0; i < m_HandlesArray.Length; i++)
//                 {
//                     // Includes all no-Plug&Play Handles
//                     if (m_HandlesArray[i] <= PCANBasic.PCAN_DNGBUS1)
//                         cbbChannel.Items.Add(FormatChannelName(m_HandlesArray[i]));
//                     else
//                     {
//                         // Checks for a Plug&Play Handle and, according with the return value, includes it
//                         // into the list of available hardware channels.
//                         //
//                         stsResult = PCANBasic.GetValue(m_HandlesArray[i], TPCANParameter.PCAN_CHANNEL_CONDITION, out iBuffer, sizeof(UInt32));
//                         if ((stsResult == TPCANStatus.PCAN_ERROR_OK) && ((iBuffer & PCANBasic.PCAN_CHANNEL_AVAILABLE) == PCANBasic.PCAN_CHANNEL_AVAILABLE))
//                         {
//                             stsResult = PCANBasic.GetValue(m_HandlesArray[i], TPCANParameter.PCAN_CHANNEL_FEATURES, out iBuffer, sizeof(UInt32));
//                             isFD = (stsResult == TPCANStatus.PCAN_ERROR_OK) && ((iBuffer & PCANBasic.FEATURE_FD_CAPABLE) == PCANBasic.FEATURE_FD_CAPABLE);
//                             cbbChannel.Items.Add(FormatChannelName(m_HandlesArray[i], isFD));
//                         }
//                     }
//                 }
//                 cbbChannel.SelectedIndex = cbbChannel.Items.Count - 1;
//                 btnInit.Enabled = cbbChannel.Items.Count > 0;
//             }
//             catch (DllNotFoundException)
//             {
//                 MessageBox.Show("Unable to find the library: PCANBasic.dll !", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                 Environment.Exit(-1);
//             }
        }
        #endregion

        #region //=======================================btnRead_Click
        private void btnRead_Click(object sender, EventArgs e)
        {
            TPCANStatus stsResult;

            // We execute the "Read" function of the PCANBasic                
            //
            stsResult = m_IsFD ? ReadMessageFD() : ReadMessage();
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                PCAN_InsertMessage(GetFormatedError(stsResult));        // If an error occurred, an information message is included
        }
        #endregion

        #region //=======================================BtnMsgClear_Click
        private void BtnMsgClear_Click(object sender, EventArgs e)
        {
            // The information contained in the messages List-View
            // is cleared
            //
            lock (m_LastMsgsList.SyncRoot)
            {
                m_LastMsgsList.Clear();
                lstMessages.Items.Clear();
            }
        }
        #endregion

        //============================================================================================= Timer Form event-handler

        #region //=======================================tmrRead_Tick
        private void tmrRead_Tick(object sender, EventArgs e)
        { 
            // Checks if in the receive-queue are currently messages for read
            // 
            ReadMessages();
        }
        #endregion

        #region //=======================================tmrDisplay_Tick
        private void tmrDisplay_Tick(object sender, EventArgs e)
        {
            DisplayMessages();
        }
        #endregion

        //============================================================================================= Message Form Handler

        #region //=======================================lstMessages_DoubleClick
        private void lstMessages_DoubleClick(object sender, EventArgs e)
        {
            // Clears the content of the Message List-View
            //
            //btnMsgClear_Click(this, new EventArgs());
        }
        #endregion

        #region //=======================================lbxInfo_DoubleClick
        private void lbxInfo_DoubleClick(object sender, EventArgs e)
        {
            // Clears the content of the Information List-Box
            //
            //btnInfoClear_Click(this, new EventArgs());
        }
        #endregion


        //============================================================================================= Misc Form event-handler

        #region //=======================================chbShowPeriod_CheckedChanged
        private void chbShowPeriod_CheckedChanged(object sender, EventArgs e)
        {
            // According with the check-value of this checkbox,
            // the recieved time of a messages will be interpreted as 
            // period (time between the two last messages) or as time-stamp
            // (the elapsed time since windows was started)
            //
            lock (m_LastMsgsList.SyncRoot)
            {
                foreach (MessageStatus msg in m_LastMsgsList)
                    msg.ShowingPeriod = chbShowPeriod.Checked;
            }
        }
        #endregion

        #region //=======================================rdbTimer_CheckedChanged
        private void rdbTimer_CheckedChanged(object sender, EventArgs e)
        {
            if (!myPCAN_Configuration.ReleaseEnable)
                return;

            // According with the kind of reading, a timer, a thread or a button will be enabled
            //
            if (rdbTimer.Checked)
            {
                // Abort Read Thread if it exists
                //
                if (m_ReadThread != null)
                {
                    m_ReadThread.Abort();
                    m_ReadThread.Join();
                    m_ReadThread = null;
                }

                // Enable Timer
                //
                tmrRead.Enabled = myPCAN_Configuration.ReleaseEnable;
            }
            if (rdbEvent.Checked)
            {
                // Disable Timer
                //
                tmrRead.Enabled = false;
                // Create and start the tread to read CAN Message using SetRcvEvent()
                //
                System.Threading.ThreadStart threadDelegate = new System.Threading.ThreadStart(this.CANReadThreadFunc);
                m_ReadThread = new System.Threading.Thread(threadDelegate);
                m_ReadThread.IsBackground = true;
                m_ReadThread.Start();
            }
            if (rdbManual.Checked)
            {
                // Abort Read Thread if it exists
                //
                if (m_ReadThread != null)
                {
                    m_ReadThread.Abort();
                    m_ReadThread.Join();
                    m_ReadThread = null;
                }
                // Disable Timer
                //
                tmrRead.Enabled = false;
            }
            btnRead.Enabled = myPCAN_Configuration.ReleaseEnable && rdbManual.Checked;
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= Generator Specific 
        //##############################################################################################################
        
        #region //=======================================btnGEN_LoadCONFIGDump_Click
        private void btnGEN_LoadCONFIGDump_Click_1(object sender, EventArgs e)
        {
            Generator_LoadCONFIGDump("SDO GEN Read Dump");
        }
        #endregion

        #region //=======================================btnGEN_LoadCONFIGDump_Click
        public delegate void mLoadCONFIGDumStartDelegate(string Message);
        private void Generator_LoadCONFIGDump(string Message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new mLoadCONFIGDumStartDelegate(Generator_LoadCONFIGDump), new object[] { Message });
                return;
            }
            //----------------------------------------
            // 678 43 30 40 03 00 00 00 00  
            // Node ID: 78.
            // SDO Access 678 return 578
            // Access: 43 , 4 byte read.
            // OD: 0x4030
            // Index cycle 0 to 15. 1st index is number of element to read.
            //---------------------------------------Standard CAN Setup.
            CanPCAN.PCAN_StdFrame myCAN = new CanPCAN.PCAN_StdFrame();
            myCAN.CanID = 0x678;
            myCAN.DLC = 8;
            myCAN.CycleNumber = 1;              // 1 cycle only. 
            myCAN.Delay = 0;                    // No delay
            myCAN.isCycleEnable = false;        // No cycle.
            //----------------------------------------Read Index 0 for number of elements.
            myCAN.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            myCAN.sComment = Message;
            myCAN.DATA[0] = 0x43;               // Command: Read 4 byte.
            myCAN.DATA[1] = 0x30;               // Index 2 byte
            myCAN.DATA[2] = 0x40;
            myCAN.DATA[3] = 0x00;               // Index 0 to read number of element.
            //myCAN.Message_Generate_CS();
            //----------------------------------------Loop 
            for (Byte i = 0; i <= 15; i++)
            {
                myCAN.DATA[3] = i;
                PCAN_WriteFrame(myCAN, TPCANMessageType.PCAN_MESSAGE_STANDARD);
                Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            }
            //---------------------------------------There no callback solution in this PCAN, additional work to do :-)
        }
        #endregion


        //##############################################################################################################
        //============================================================================================= Classes
        //##############################################################################################################

        #region //====================================PCAN_HeartBeat
        [Serializable]
        public class PCAN_HeartBeat
        {
            public string CANID         { get; set; }
            public string Name          { get; set; }
            public int Period           { get; set; }
            public bool BlinkOn         { get; set; }
            public Color OffColor       { get; set; }
            public int CountmSec        { get; set; }
            public int CountState       { get; set; }   // 0 = Standby, 1 = Counting process, 
                                                        // 2 = Counting within Target +/-5%, 
                                                        // 3 = Counting less than target <95%, 4 = Counting exceed Target >105%
                                                        // 5 = Blink is completed, revert back to normal color. 
            private int MaxPeriod;
            private int MinPeriod;
            // ==================================================================constructor
            public PCAN_HeartBeat(string sCANID, string sName, int iPeriod)
            {
                CANID = sCANID;
                Name = sName;
                Period = iPeriod;
                MaxPeriod = (Period * 105) / 100 ;
                MinPeriod = (Period * 95) / 100;
                BlinkOn = false;
                OffColor = Color.White;
                CountmSec = 0;
                CountState = 0;                     // Standby
            }
            public int iCanHeartBeatEvent()
            {
                if (CountState == 0)                // Suspend counting. 
                    return (0);
                if ((CountmSec >= MinPeriod) & (CountmSec <= MaxPeriod))
                    CountState = 2;                 // Passed.
                else
                {
                    if (CountmSec < MinPeriod)
                        CountState = 3;             // Failed, less than target, defective MCU clock (1mSec issue)
                    if (CountmSec > MaxPeriod)
                        CountState = 4;             // Failed, more than target, CAN did not issue heartbeat, crashed CAN or MCU.
                }
                CountmSec = 0;
                return CountState;
            }
            public int iIncrementCounts()
            {
                CountmSec++;
                if (CountState == 0)                // Suspend counting. 
                    return (0);
                CountState = 1;
                if ((BlinkOn == true) & (CountmSec > 250))
                {
                    BlinkOn = false;
                    CountState = 5;                 // Signify change color back to white for next blink. 
                }         
                if (CountmSec > MaxPeriod)          // Highlight expiry heartbeat situation. 
                    CountState = 4;
                return CountState;
            }
            public void SusendCounts()              // Suspend counts
            {
                CountState = 0;
            }
            public void RunCounts()                 // Run this when new column channel is added. 
            {
                CountmSec = 0;
                CountState = 1;                     // Busy Counting activity. 
            }
            public void UpdatePeriodMinMax()
            {
                MaxPeriod = (Period * 105) / 100;
                MinPeriod = (Period * 95) / 100;
            }
        }
        #endregion

        #region //====================================PCAN MessageStatus Class
        private class MessageStatus
        {
            private TPCANMsgFD m_Msg;
            private TPCANTimestampFD m_TimeStamp;
            private TPCANTimestampFD m_oldTimeStamp;
            private int m_iIndex;
            private int m_Count;
            private bool m_bShowPeriod;
            private bool m_bWasChanged;

            public MessageStatus(TPCANMsgFD canMsg, TPCANTimestampFD canTimestamp, int listIndex)
            {
                m_Msg = canMsg;
                m_TimeStamp = canTimestamp;
                m_oldTimeStamp = canTimestamp;
                m_iIndex = listIndex;
                m_Count = 1;
                m_bShowPeriod = true;
                m_bWasChanged = false;
                sLogMsg = "";
                sErrorBusStatus = "";
            }

            public MessageStatus()
            {
//                 m_Msg = canMsg;
//                 m_TimeStamp = canTimestamp;
//                 m_oldTimeStamp = canTimestamp;
//                 m_iIndex = listIndex;
//                 m_Count = 1;
//                 m_bShowPeriod = true;
//                 m_bWasChanged = false;
//                 sLogMsg = "";
//                 sErrorBusStatus = "";
            }

            public void Update(TPCANMsgFD canMsg, TPCANTimestampFD canTimestamp)
            {
                m_Msg = canMsg;
                m_oldTimeStamp = m_TimeStamp;
                m_TimeStamp = canTimestamp;
                m_bWasChanged = true;
                m_Count += 1;
            }

            public TPCANMsgFD CANMsg
            {
                get { return m_Msg; }
            }

            public TPCANTimestampFD Timestamp
            {
                get { return m_TimeStamp; }
            }

            public int Position
            {
                get { return m_iIndex; }
            }

            public string TypeString
            {
                get { return GetMsgTypeString(); }
            }

            public string IdString
            {
                get { return GetIdString(); }
            }

            public string DataString
            {
                get { return GetDataString(); }
            }

            public int Count
            {
                get { return m_Count; }
            }

            public bool ShowingPeriod
            {
                get { return m_bShowPeriod; }
                set
                {
                    if (m_bShowPeriod ^ value)
                    {
                        m_bShowPeriod = value;
                        m_bWasChanged = true;
                    }
                }
            }

            public bool MarkedAsUpdated
            {
                get { return m_bWasChanged; }
                set { m_bWasChanged = value; }
            }

            public string TimeString
            {
                get { return GetTimeString(); }
            }
            //-----------------------------------------------------------------Added Public member
            public string sErrorBusStatus   { get; set; }
            public string sLogMsg           { get; set; }
            //-----------------------------------------------------------------Methods
            private string GetTimeString()
            {
                double fTime;

                fTime = (m_TimeStamp / 1000.0);
                if (m_bShowPeriod)
                    fTime -= (m_oldTimeStamp / 1000.0);
                return fTime.ToString("F1");
            }
            //------------------------------------------------------
            private string GetDataString()
            {
                string strTemp;

                strTemp = "";

                int Length = CanPCAN.GetLengthFromDLC(m_Msg.DLC, (m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == 0);

                if ((m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_RTR) == TPCANMessageType.PCAN_MESSAGE_RTR)
                    return "Remote Request";
                else
                    for (int i = 0; i < 8; i++)     // Standard only, will later upgrade to FD type. RGP Modified for Fixed length. 
                    {
                        if (i < Length)
                            strTemp += string.Format("{0:X2} ", m_Msg.DATA[i]);
                        else
                            strTemp += "   ";
                    }
                return strTemp;
            }
            // This is for TX message generator.
            private string GetDataString(TPCANMessageType MSGTYPE, Byte DLC, byte[] Data)
            {
                string strTemp;

                strTemp = "";

                int Length = CanPCAN.GetLengthFromDLC(DLC, (MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == 0);

                if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_RTR) == TPCANMessageType.PCAN_MESSAGE_RTR)
                    return "Remote Request";
                else
                    for (int i = 0; i < 8; i++)     // Standard only, will later upgrade to FD type. RGP Modified for Fixed length. 
                    {
                        if (i < Length)
                            strTemp += string.Format("{0:X2} ", Data[i]);
                        else
                            strTemp += "   ";
                    }
                return strTemp;
            }
            //------------------------------------------------------
            private string GetIdString()
            {
                // We format the ID of the message and show it
                //
                if ((m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_EXTENDED) == TPCANMessageType.PCAN_MESSAGE_EXTENDED)
                    return string.Format("{0:X8}h", m_Msg.ID);
                else
                    return string.Format("{0:X3}h", m_Msg.ID);
            }

            private string GetMsgTypeString()
            {
                string strTemp;
                //-----------------------------------------------------------------------------------------------
                if ((m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_STATUS) == TPCANMessageType.PCAN_MESSAGE_STATUS)
                    return      "STATUS";
                //-----------------------------------------------------------------------------------------------
                if ((m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_ERRFRAME) == TPCANMessageType.PCAN_MESSAGE_ERRFRAME)
                    return      "ERROR ";
                //-----------------------------------------------------------------------------------------------
                if ((m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_EXTENDED) == TPCANMessageType.PCAN_MESSAGE_EXTENDED)
                    strTemp =   "EXT   ";
                else
                    strTemp =   "STD   ";
                //-----------------------------------------------------------------------------------------------
                if ((m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_RTR) == TPCANMessageType.PCAN_MESSAGE_RTR)
                {
                    strTemp = strTemp.Replace(" ", "");
                    strTemp += "/RTR";
                }
                else
                {
                    if ((int)m_Msg.MSGTYPE > (int)TPCANMessageType.PCAN_MESSAGE_EXTENDED)
                    {
                        strTemp = strTemp.Replace(" ", "");
                        strTemp += "[";
                        if ((m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == TPCANMessageType.PCAN_MESSAGE_FD)
                            strTemp += "FD ";
                        if ((m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_BRS) == TPCANMessageType.PCAN_MESSAGE_BRS)
                            strTemp += "BRS";
                        if ((m_Msg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_ESI) == TPCANMessageType.PCAN_MESSAGE_ESI)
                            strTemp += "ESI";
                        strTemp += "]";
                    }
                }
                return strTemp;
            }
            private string GetMsgTypeString(TPCANMessageType MSGTYPE)
            {
                // This is for TX message generator.
                string strTemp;
                //-----------------------------------------------------------------------------------------------
                if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_STATUS) == TPCANMessageType.PCAN_MESSAGE_STATUS)
                    return      "STATUS";
                //-----------------------------------------------------------------------------------------------
                if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_ERRFRAME) == TPCANMessageType.PCAN_MESSAGE_ERRFRAME)
                    return      "ERROR ";
                //-----------------------------------------------------------------------------------------------
                if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_EXTENDED) == TPCANMessageType.PCAN_MESSAGE_EXTENDED)
                    strTemp =   "EXT   ";
                else
                    strTemp =   "STD   ";
                //-----------------------------------------------------------------------------------------------
                if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_RTR) == TPCANMessageType.PCAN_MESSAGE_RTR)
                {
                    strTemp = strTemp.Replace(" ", "");
                    strTemp += "/RTR";
                }
                else
                {
                    if ((int)MSGTYPE > (int)TPCANMessageType.PCAN_MESSAGE_EXTENDED)
                    {
                        strTemp = strTemp.Replace(" ", "");
                        strTemp += "[";
                        if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == TPCANMessageType.PCAN_MESSAGE_FD)
                            strTemp += "FD";
                        if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_BRS) == TPCANMessageType.PCAN_MESSAGE_BRS)
                            strTemp += "BRS";
                        if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_ESI) == TPCANMessageType.PCAN_MESSAGE_ESI)
                            strTemp += "ESI";
                        strTemp += "]";
                    }
                }
                return strTemp;
            }   

            public string PCAN_GenerateLogMessageRX(int iLength)   
            {
                //------------------------------------------------------------Form a Log Message
                //RXTX,TimeStamp,CANID,Type,Err,Count,DLC,Data(Fixed Width 8 * 3 = 24) || Decoded Data based on ID(via Scrip)
                // GetMsgTypeString(), GetIdString(), GetTimeString(), GetDataString()    

                string sLogMsgX1 = "RX,";
                sLogMsgX1 +=        string.Format("{0,19},", Timestamp);
                sLogMsgX1 +=        string.Format("0x{0:X3},", m_Msg.ID);
                sLogMsgX1 +=        GetMsgTypeString() + ",";
                sLogMsgX1 +=        "TBA,";                                 // ###TASK: Error State
                sLogMsgX1 +=        string.Format("{0,4},", Count);    
                sLogMsgX1 +=        string.Format("{0,1} ", iLength);
                sLogMsgX1 +=        ": " + DataString + "\n";
                return sLogMsgX1;
            }
            public string PCAN_GenerateLogMessageTX(TPCANMsg CANMsg, TPCANStatus Result, TPCANTimestampFD Timestamp)
            { 
                string sLogMsgX1 = "TX,";
                sLogMsgX1 +=        string.Format("{0,19},", Timestamp);
                sLogMsgX1 +=        string.Format("0x{0:X3},", CANMsg.ID);
                sLogMsgX1 +=        GetMsgTypeString(CANMsg.MSGTYPE) + ",";
                sLogMsgX1 +=        "TBA,";                                  // ###TASK: Error State
                sLogMsgX1 +=        "....,";                                 // ###TASK: Need to figure out count.
                sLogMsgX1 +=        string.Format("{0,1} ", CANMsg.LEN);
                sLogMsgX1 +=        ": " + GetDataString(CANMsg.MSGTYPE, CANMsg.LEN, CANMsg.DATA) + "\n";
                return sLogMsgX1;
            }
        }
        #endregion

        #region //====================================PCAN Configuration Class
        // Serialization of ArrayList and Generic List
        // The XmlSerializer cannot serialize or deserialize the following:
        //      Arrays of ArrayList
        //      Arrays of List
        // This is why it fails. 
        // Use BinSerializer instead. It work well for List/ArrayList type. 

        [Serializable]
        public class PCAN_Configuration
        {
            //-------------------------------PCAN configuration. 
            public TPCANBaudrate    Baudrate;
            public int              BaudrateIndex;
            //---Filter
            public int              IndexFrom;
            public int              IndexTo;
            //---Mode
            public TPCANMode        PCANMode;          // True = TPCANMode.PCAN_MODE_EXTENDED, False = PCANMode.PCAN_MODE_STANDARD
            //-------------------------------Master FolderName.
            public string sFolderName;
            public string sFolderNameLogMsg;
            public string sFolderNameCommandList;
            //-------------------------------HeartBeat
            [NonSerialized] public List<PCAN_HeartBeat> lsHeartBeatDatabase;        // [0] = CANID, [1] = Name, [2] = Heartbeat flasher (0, 1). 
            public List<PCAN_HeartBeat> lsHeartBeatDisplayed;
            //-------------------------------Internal
            [NonSerialized]  public TPCANType        HwType;              // NON PLUG and PLAY: The type of hardware and operation mode</param>
            [NonSerialized]  public UInt32           IOPort;              // NON PLUG and PLAY: The I/O address for the parallel port</param>
            [NonSerialized]  public UInt16           Interrupt;           // NON PLUG and PLAY: Interrupt number of the parallel por</param>
            [NonSerialized]  public TPCANHandle      HandlesArray;
            [NonSerialized]  public bool             ReleaseEnable;       // True when Connected, False when released/disconnected.
            [NonSerialized]  private TPCANHandle[] iHandlesArray = new TPCANHandle[]
            {
                PCANBasic.PCAN_ISABUS1,
                PCANBasic.PCAN_ISABUS2,
                PCANBasic.PCAN_ISABUS3,
                PCANBasic.PCAN_ISABUS4,
                PCANBasic.PCAN_ISABUS5,
                PCANBasic.PCAN_ISABUS6,
                PCANBasic.PCAN_ISABUS7,
                PCANBasic.PCAN_ISABUS8,
                PCANBasic.PCAN_DNGBUS1,
                PCANBasic.PCAN_PCIBUS1,
                PCANBasic.PCAN_PCIBUS2,
                PCANBasic.PCAN_PCIBUS3,
                PCANBasic.PCAN_PCIBUS4,
                PCANBasic.PCAN_PCIBUS5,
                PCANBasic.PCAN_PCIBUS6,
                PCANBasic.PCAN_PCIBUS7,
                PCANBasic.PCAN_PCIBUS8,
                PCANBasic.PCAN_PCIBUS9,
                PCANBasic.PCAN_PCIBUS10,
                PCANBasic.PCAN_PCIBUS11,
                PCANBasic.PCAN_PCIBUS12,
                PCANBasic.PCAN_PCIBUS13,
                PCANBasic.PCAN_PCIBUS14,
                PCANBasic.PCAN_PCIBUS15,
                PCANBasic.PCAN_PCIBUS16,
                PCANBasic.PCAN_USBBUS1,     //25
                PCANBasic.PCAN_USBBUS2,
                PCANBasic.PCAN_USBBUS3,
                PCANBasic.PCAN_USBBUS4,
                PCANBasic.PCAN_USBBUS5,
                PCANBasic.PCAN_USBBUS6,
                PCANBasic.PCAN_USBBUS7,
                PCANBasic.PCAN_USBBUS8,
                PCANBasic.PCAN_USBBUS9,
                PCANBasic.PCAN_USBBUS10,
                PCANBasic.PCAN_USBBUS11,
                PCANBasic.PCAN_USBBUS12,
                PCANBasic.PCAN_USBBUS13,
                PCANBasic.PCAN_USBBUS14,
                PCANBasic.PCAN_USBBUS15,
                PCANBasic.PCAN_USBBUS16,
                PCANBasic.PCAN_PCCBUS1,
                PCANBasic.PCAN_PCCBUS2,
                PCANBasic.PCAN_LANBUS1,
                PCANBasic.PCAN_LANBUS2,
                PCANBasic.PCAN_LANBUS3,
                PCANBasic.PCAN_LANBUS4,
                PCANBasic.PCAN_LANBUS5,
                PCANBasic.PCAN_LANBUS6,
                PCANBasic.PCAN_LANBUS7,
                PCANBasic.PCAN_LANBUS8,
                PCANBasic.PCAN_LANBUS9,
                PCANBasic.PCAN_LANBUS10,
                PCANBasic.PCAN_LANBUS11,
                PCANBasic.PCAN_LANBUS12,
                PCANBasic.PCAN_LANBUS13,
                PCANBasic.PCAN_LANBUS14,
                PCANBasic.PCAN_LANBUS15,
                PCANBasic.PCAN_LANBUS16,
            };
            // ==================================================================constructor
            public PCAN_Configuration()
            {
                // Leave blank, do not install default data here.
            }
            public void Init_ListActiveDisplay()
            {
                lsHeartBeatDisplayed = new List<PCAN_HeartBeat>();
            }
            public void Init_ListDataBase()
            {
                lsHeartBeatDatabase = new List<PCAN_HeartBeat>();
                lsHeartBeatDatabase.Add(new PCAN_HeartBeat("10", "Pulser", 1000));
                lsHeartBeatDatabase.Add(new PCAN_HeartBeat("20", "Gamma", 1000));
                lsHeartBeatDatabase.Add(new PCAN_HeartBeat("30", "MWD-Dir", 1000));
                lsHeartBeatDatabase.Add(new PCAN_HeartBeat("40", "MWD-EM", 1000));
                lsHeartBeatDatabase.Add(new PCAN_HeartBeat("50", "Batt1", 1000));
                lsHeartBeatDatabase.Add(new PCAN_HeartBeat("58", "Batt2", 1000));
                lsHeartBeatDatabase.Add(new PCAN_HeartBeat("60", "Batt3", 1000));
                lsHeartBeatDatabase.Add(new PCAN_HeartBeat("70", "EMT-PWD", 1000));
                lsHeartBeatDatabase.Add(new PCAN_HeartBeat("78", "MWD-GEN", 1000));
            }
            public void ConfigurationDefault()
            {
                HwType          = TPCANType.PCAN_TYPE_ISA;
                Baudrate        = TPCANBaudrate.PCAN_BAUD_250K;
                BaudrateIndex   = 3;
                HandlesArray    = iHandlesArray[25];        //PCANBasic.PCAN_USBBUS1 = 0x51, index 25
                IOPort          = 0x0100;
                Interrupt       = 3;
                IndexFrom       = 0;
                IndexTo         = 0x7FF;
                PCANMode        = TPCANMode.PCAN_MODE_STANDARD;
                ReleaseEnable   = false;
                sFolderName     = "";
                sFolderNameLogMsg = "";
                sFolderNameCommandList = "";
                //----------------------------------
            }
        }
        #endregion

        #region//====================================TabMaster: struct CommandStruct
        struct CommandStructPCAN
        {
            private readonly string label;
            private readonly int width;

            public CommandStructPCAN(string label, int width)
            {
                this.label = label;
                this.width = width;
            }

            public string Label { get { return label; } }
            public int Width { get { return width; } }

        }
        #endregion

        #region//====================================TabMaster: Setup Column Format
        static CommandStructPCAN[] myCommndColumnFormatPCAN =
            new CommandStructPCAN[]{
                new CommandStructPCAN("Command",120),
                new CommandStructPCAN("Description",220)
            };

        public object FlexibleMessageBox { get; private set; }
        #endregion

        #region //====================================PCAN CAN Frame
        [Serializable]
        public class PCAN_StdFrame
        {
            private System.Windows.Forms.DataGridView myDataGridCycle;
            public void MyDataGridCycle(System.Windows.Forms.DataGridView myDataGridCycleRef)
            {
                myDataGridCycle = myDataGridCycleRef;
            }

            public uint CanID;                     //11/29-bit message identifier
            [MarshalAs(UnmanagedType.U1)]
            public TPCANMessageType MSGTYPE;
            public byte DLC;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] DATA;
            //-------------------------------Note
            public string sComment;
            //-------------------------------Records
            public int ProcessCount;
            public int ProcessTimeStamp;
            //-------------------------------Loop Config
            public int Delay;                           // uSec, between command only for CD() only. For CL() command it would be mSec.
            public int CycleNumber;                     // 0 = Infinite, 1 = one loop then cease, etc. 
            public int CyclePeriod;                     // mSec
            public bool isCycleEnable;                  // True = active process. 
            //-------------------------------Loop active variable.
            private int DelayCounter;                   // Delay before 1st transmission loop counter.  
            private int LoopCounter;                    // Period counter in mSec.
            private int ResetCounter;                   // Threshold to reset counter in mSec
            private int CycleNumberSentCounter;         // Number of time it sent out. 
            private bool isCycleNumberExpired;          // True = expired loop number. 
            //-------------------------------Misc
            private int ch;                             // channel number reference for datagridcycle view. 
            //==================================================================Constructor
            public PCAN_StdFrame()
            {
                DATA = new byte[8];
                Clear();
                //-------------------Loop Counter
                DelayCounter = 0;
                ResetCounter = 0;   
                LoopCounter = 0;
                CycleNumberSentCounter = 0;
                ProcessCount = 0;
                ProcessTimeStamp = 0;
                sComment = "";
            }

            //==================================================================Message Generator.

            #region //=======================================GetMsgTypeString
            public string GetMsgTypeString()
            {
                string strTemp;
                //-----------------------------------------------------------------------------------------------
                if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_STATUS) == TPCANMessageType.PCAN_MESSAGE_STATUS)
                    return "STATUS";
                //-----------------------------------------------------------------------------------------------
                if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_ERRFRAME) == TPCANMessageType.PCAN_MESSAGE_ERRFRAME)
                    return "ERROR ";
                //-----------------------------------------------------------------------------------------------
                if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_EXTENDED) == TPCANMessageType.PCAN_MESSAGE_EXTENDED)
                    strTemp = "EXT   ";
                else
                    strTemp = "STD   ";
                //-----------------------------------------------------------------------------------------------
                if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_RTR) == TPCANMessageType.PCAN_MESSAGE_RTR)
                {
                    strTemp = strTemp.Replace(" ", "");
                    strTemp += "/RTR";
                }
                else
                {
                    if ((int)MSGTYPE > (int)TPCANMessageType.PCAN_MESSAGE_EXTENDED)
                    {
                        strTemp = strTemp.Replace(" ", "");
                        strTemp += "[";
                        if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == TPCANMessageType.PCAN_MESSAGE_FD)
                            strTemp += "FD ";
                        if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_BRS) == TPCANMessageType.PCAN_MESSAGE_BRS)
                            strTemp += "BRS";
                        if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_ESI) == TPCANMessageType.PCAN_MESSAGE_ESI)
                            strTemp += "ESI";
                        strTemp += "]";
                    }
                }
                return strTemp;
            }
            #endregion

            #region //=======================================GetDataString
            public string GetDataString()
            {
                string strTemp;

                strTemp = "";

                int Length = CanPCAN.GetLengthFromDLC(DLC, (MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == 0);

                if ((MSGTYPE & TPCANMessageType.PCAN_MESSAGE_RTR) == TPCANMessageType.PCAN_MESSAGE_RTR)
                    return "Remote Request";
                else
                    for (int i = 0; i < 8; i++)     // Standard only, will later upgrade to FD type. RGP Modified for Fixed length. 
                    {
                        if (i < Length)
                            strTemp += string.Format("{0:X2} ", DATA[i]);
                        else
                            strTemp += "   ";
                    }
                return strTemp;
            }
            #endregion

            //==================================================================Loop Process Methods. 

            #region //=======================================Clear
            public void Clear()
            {
                CanID = 0;
                MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                DLC = 8;
                for (int i = 0; i < 8; i++)
                {
                    DATA[i] = 0;
                }
                //------------Config Only. 
                Delay = 0;
                CycleNumber = 0;
                CyclePeriod = 0;
                isCycleEnable = false;
            }
            #endregion

            #region //=======================================ProcessAddNewChannel
            public void ProcessAddNewChannel(int ichannelno)
            {
                ResetCycleParam();
                ch = ichannelno;
                myDataGridCycle.Rows.Add(new string[] { myDataGridCycle.Rows.Count.ToString("D2")});
                //ch = myDataGridCycle.Rows.Count-1;
                //-----------------------------------------------------------------------------------------------
                ProcessChannelRefresh(true);
                //-----------------------------------------------------------------------------------------------
            }
            #endregion

            #region //=======================================ProcessChannelRefresh
            public void ProcessChannelRefresh(bool isAll)
            {
                if (isAll==true)
                {
                    myDataGridCycle.Rows[ch].Cells[1].Value = isCycleEnable;
                    myDataGridCycle.Rows[ch].Cells[2].Value = GetMsgTypeString();
                    myDataGridCycle.Rows[ch].Cells[3].Value = string.Format("0x{0:X3}", CanID);
                    myDataGridCycle.Rows[ch].Cells[4].Value = string.Format("{0,1}", DLC);
                }
                //-----------------------------------------------------------------------------------------------

                if (CycleNumber == 0)
                {
                    myDataGridCycle.Rows[ch].Cells[5].Value = string.Format("{0,3}", ProcessCount);
                    myDataGridCycle.Rows[ch].Cells[5].Style.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))); ;
                    myDataGridCycle.Rows[ch].Cells[5].Style.BackColor = Color.White;            // Mean Process Count.
                }
                else
                {
                    myDataGridCycle.Rows[ch].Cells[5].Value = string.Format("{0,3}", CycleNumber);
                    myDataGridCycle.Rows[ch].Cells[5].Style.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    if (isCycleNumberExpired == false)
                        myDataGridCycle.Rows[ch].Cells[5].Style.BackColor = Color.LightGreen;    // Mean Cycle Number.
                    else

                        myDataGridCycle.Rows[ch].Cells[5].Style.BackColor = Color.SeaShell;    // Mean Cycle Number completed.
                }
                myDataGridCycle.Rows[ch].Cells[6].Value = string.Format("{0,10}", Delay);
                myDataGridCycle.Rows[ch].Cells[7].Value = string.Format("{0,10}", CyclePeriod);
                myDataGridCycle.Rows[ch].Cells[8].Value = GetDataString();
                myDataGridCycle.Rows[ch].Cells[9].Value = sComment;
                //-----------------------------------------------------------------------------------------------
            }
            #endregion

            #region //=======================================ResetCycleParam
            public void ResetCycleParam()
            {
                DelayCounter = Delay;
                LoopCounter = 0;
                CycleNumberSentCounter = 0;
                if (ResetCounter < CyclePeriod)
                    ResetCounter = CyclePeriod;
                isCycleNumberExpired = false;
            }
            #endregion

            #region //=======================================isCounterMatched
            public bool isCounterMatched()
            {
                if ((isCycleEnable == true) & (isCycleNumberExpired==false))
                {
                    if (DelayCounter == 0)
                    {
                        LoopCounter++;
                        if (LoopCounter >= ResetCounter)
                        {
                            LoopCounter = 0;
                            return true;
                        }
                    }
                    else
                    {
                        DelayCounter--;
                    }
                }
                return false;
            }
            #endregion

            #region //=======================================CycleNumberProcess
            public void CycleNumberProcess()
            {
                ProcessCount++;
                if ((isCycleEnable == true) & (CycleNumber!=0)) //CycleNumber=0 mean Infinite loop. 
                {
                    CycleNumberSentCounter++;
                    if (CycleNumberSentCounter >= CycleNumber)
                    {
                        isCycleNumberExpired = true;
                    }
                }
                ProcessChannelRefresh(false);

            }
            #endregion

            #region //=======================================CycleNumberIncrement
            public void SentProcessCounterIncrement()
            {
                ProcessCount++;
                ProcessChannelRefresh(false);
            }
            #endregion

            #region //=======================================isCycleNumberProcessExpired
            public bool isCycleNumberProcessExpired()
            {
                return isCycleNumberExpired;
            }
            #endregion

            #region //=======================================Message_Generate_CS
            public string[] Message_Generate_CS()
            {
                //CS(ID;DLC;D0;D1;D2;D3;D4;D5;D6;D7) or \r\n");
                string[] sCS = new string[2];
                sCS[1] = sComment;
                sCS[0] = "CS(" +
                    "0x" + CanID.ToString("X3") + ";" +
                    "0x" + DLC.ToString("X2") + ";";
                for (int i = 0; i < DLC; i++)
                {
                    sCS[0] += DATA[i].ToString("X2") + ';';
                }
                sCS[0].Substring(sCS[0].Length - 1);
                sCS[0] += ")";
                return sCS;
            }
            #endregion

            #region //=======================================Message_Generate_CL
            public string[] Message_Generate_CL()
            {
                //CL(Delay;CyclePeriod;NoOfLoop;ID;DLC;D0;D1;D2;D3;D4;D5;D6;D7) \r\n");
                string[] sCL = new string[2];
                sCL[1] = sComment;
                sCL[0] = "CL(" +
                    Delay.ToString() + ";" +
                    CyclePeriod.ToString() + ";" +
                    CycleNumber.ToString() + ";" +
                    "0x" + CanID.ToString("X3") + ";" +
                    "0x" + DLC.ToString("X2") + ";";
                for (int i = 0; i < DLC; i++)
                {
                    sCL[0] += DATA[i].ToString("X2")+';';
                }
                sCL[0].Substring(sCL[0].Length - 1);
                sCL[0] += ")";
                return sCL;
            }
            #endregion

        }

        #endregion

    }

    #region//======================================================DoubleBuffered property via reflection for RichTextBox, improve flickerless image.
    public static class ExtensionMethodsPCANTerm
    {
        public static void DoubleBufferedPCANTerm(this RichTextBox dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    #endregion

    #region//======================================================DoubleBuffered property via reflection for RichTextBox, improve flickerless image.
    public static class ListViewExtensionsPCANMessage
    {
        public static void DoubleBufferedPCANListView(this ListView listView, bool value)
        {
            listView.GetType()
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(listView, value);
        }
    }
    #endregion

    #region//======================================================DoubleBuffered property via reflection for TabControl, improve flickerless image.
    public static class ExtensionMethodsTabMaterPCAN
    {
        public static void DoubleBufferedTabMasterPCAN(this TabControl dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    #endregion

    #region//======================================================DoubleBuffered property via reflection for dgvType, improve flickerless image.
    public static class ExtensionMethodsPCAN_DGV
    {
        public static void DoubleBufferedPCAN_DGV(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    #endregion

}


//##############################################################################################################
//============================================================================================= SanBox Region/Old Code
//##############################################################################################################

#region //=======================================SandBox

//             public string PCAN_GenerateLogMessageTX(UInt32 CanID, Byte CanLeN, TPCANMessageType CanMgsType, byte[] CanDATA, TPCANStatus Result, TPCANTimestampFD Timestamp)
//             {
//                 string sLogMsgX1 = "TX,";
//                 sLogMsgX1 += string.Format("{0,15:-####},", Timestamp / 1000);
//                 sLogMsgX1 += string.Format("0x{0:X3},", CanID);
//                 sLogMsgX1 += GetMsgTypeString(CanMgsType) + ",";
//                 sLogMsgX1 += "TBA,";                                  // ###TASK: Error State
//                 sLogMsgX1 += "....,";                                 // ###TASK: Need to figure out count.
//                 sLogMsgX1 += string.Format(" {0,1},", CanLeN);
//                 sLogMsgX1 += GetDataString(CanMgsType, CanLeN, CanDATA) + "\n";
//                 return sLogMsgX1;
//             }


//                 if (cbCycleRun.Checked == true)
//                 {
//                     cbCyclePause.Checked = false;
//                     cbCycleRun.Checked = true;
//                     cbCycleReset.Checked = false;
//                     foreach (PCAN_StdFrame myPCAN_StdFrame in myCycleArray)
//                     {
//                         myPCAN_StdFrame.ResetCycleParam();
//                     }
//                 }
//                 else
//                {

//         private TPCANStatus WriteFrame()
//         {
//             TPCANMsg CANMsg;
//             TPCANMsg CANMsg2;
//             MessageStatus msgStsCurrentMsg = new MessageStatus();
//             TextBox txtbCurrentTextBox;
//             TextBox txtbCurrentTextBox2;
//             TPCANStatus Result1;
//             TPCANStatus Result2;
//             TPCANTimestampFD TxTimestampFD;
//             // We create a TPCANMsg message structure 
//             //
//             CANMsg = new TPCANMsg();
//             CANMsg.DATA = new byte[8];
//             CANMsg2 = new TPCANMsg();
//             CANMsg2.DATA = new byte[8];
//             // We configurate the Message.  The ID,
//             // Length of the Data, Message Type
//             // and the data
//             //
//             CANMsg.ID = Convert.ToUInt32(txtID.Text, 16);
//             CANMsg.LEN = Convert.ToByte(nudLength.Value);
//             CANMsg.MSGTYPE = (chbExtended.Checked) ? TPCANMessageType.PCAN_MESSAGE_EXTENDED : TPCANMessageType.PCAN_MESSAGE_STANDARD;
//             CANMsg2.ID = Convert.ToUInt32(txtID2.Text, 16);
//             CANMsg2.LEN = Convert.ToByte(nudLength2.Value);
//             CANMsg2.MSGTYPE = (chbExtended.Checked) ? TPCANMessageType.PCAN_MESSAGE_EXTENDED : TPCANMessageType.PCAN_MESSAGE_STANDARD;
//             // If a remote frame will be sent, the data bytes are not important.
//             //
//             if (chbRemote.Checked)
//                 CANMsg.MSGTYPE |= TPCANMessageType.PCAN_MESSAGE_RTR;
//             else
//             {
//                 // We get so much data as the Len of the message
//                 //
//                 for (int i = 0; i < GetLengthFromDLC(CANMsg.LEN, true); i++)
//                 {
//                     txtbCurrentTextBox = (TextBox)this.Controls.Find("txtData" + i.ToString(), true)[0];
//                     CANMsg.DATA[i] = Convert.ToByte(txtbCurrentTextBox.Text, 16);
//                 }
//             }
// 
//             // The message is sent to the configured hardware
//             //
//             
//             Result1 = PCANBasic.Write(m_PcanHandle, ref CANMsg);
//             TxTimestampFD = PCAN_GetTimestamp_uSec();
//             myPCANTermMessageLF(msgStsCurrentMsg.PCAN_GenerateLogMessageTX(CANMsg, Result1, TxTimestampFD));
//             //---------------------------------------------------------------------------------------------------------------------------
//             //------------------------------------------------------------2nd Frame Message: txtData16
//             txtbCurrentTextBox = (TextBox)this.Controls.Find("txtData16", true)[0];
//             Byte DataB2 = Convert.ToByte(txtbCurrentTextBox.Text, 16);
//             if ((DataB2 == 0x00) & (txtID2.Text == "0"))
//             {
//                 return Result1;
//             }
//             //------------------------------------------------------------Pause between two CAN Message
//             int pause = Convert.ToInt32(tbPausemSec.Text);
//             //             if (pause != 0)
//             //                 Thread.Sleep(pause);
//             udelay(pause);
//             //------------------------------------------------------------Send 2nd frame
//             if (chbRemote.Checked)
//                 CANMsg2.MSGTYPE |= TPCANMessageType.PCAN_MESSAGE_RTR;
//             else
//             {
//                 // We get so much data as the Len of the message
//                 //
//                 for (int i = 16; i < 16 + GetLengthFromDLC(CANMsg2.LEN, true); i++)
//                 {
//                     txtbCurrentTextBox2 = (TextBox)this.Controls.Find("txtData" + i.ToString(), true)[0];
//                     CANMsg2.DATA[i - 16] = Convert.ToByte(txtbCurrentTextBox2.Text, 16);
//                 }
//             }
//             Result2 = PCANBasic.Write(m_PcanHandle, ref CANMsg2);
//             TxTimestampFD = PCAN_GetTimestamp_uSec();
//             myPCANTermMessageLF(msgStsCurrentMsg.PCAN_GenerateLogMessageTX(CANMsg2, Result2, TxTimestampFD));
//             
//             return Result2;
//         }
//         #endregion

//         #region //=======================================btnWrite_Click
//         private void btnWrite_Click(object sender, EventArgs e)
//         {
//             TPCANStatus stsResult;
// 
//             // Send the message
//             //
//             stsResult = WriteFrame();
// 
//             // The message was successfully sent
//             //
// //             if (stsResult == TPCANStatus.PCAN_ERROR_OK)
// //             {
// //                 PCAN_InsertMessage("Message was successfully SENT");
// //                 // Also add trace: see ConfigureTraceFile
// //             }
// 
//             // An error occurred.  We show the error.
//             //			
//             //else
//                 PCAN_InsertMessage(GetFormatedError(stsResult));
//         }

// #region //================================================List View for CAN Loop
// lstLoop.CheckBoxes = true;
//             clhLoopEnable = new ColumnHeader();
// ListViewItem item1 = new ListViewItem("Enable", 0);
// item1.Checked = true;
//             item1.Text = "Type";
//             
//             clhLoopType = new ColumnHeader();
// clhLoopType.Text = "Type";
//             clhLoopType.Width = 110;
//             clhLoopID = new ColumnHeader();
// clhLoopID.Text = "ID";
//             clhLoopID.Width = 50;
//             clhLoopLength = new ColumnHeader();
// clhLoopLength.Text = "Length";
//             clhLoopLength.Width = 50;
//             clhLoopCount = new ColumnHeader();
// clhLoopCount.Text = "Count";
//             clhLoopCount.Width = 49;
//             clhLoopRcvTime = new ColumnHeader();
// clhLoopRcvTime.Text = "Rcv Time";
//             clhLoopRcvTime.Width = 150;
//             clhLoopData = new ColumnHeader();
// clhLoopData.Text = "Data";
//             clhLoopData.Width = 300;
// 
//             
// 
//             lstLoop = new System.Windows.Forms.ListView();
//             lstLoop.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
//             item1,
//             clhLoopType,
//             clhLoopID,
//             clhLoopLength,
//             clhLoopCount,
//             clhLoopRcvTime,
//             clhLoopData});
//             //-----------------------------------------------
//             lstLoop.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//             lstLoop.FullRowSelect = true;
//             lstLoop.HideSelection = false;
//             lstLoop.GridLines = true;
//             lstLoop.Location = new System.Drawing.Point(2, 19);
//             lstLoop.MultiSelect = false;
//             lstLoop.Name = "lstLoop";
//             lstLoop.Size = new System.Drawing.Size(657, 446);
//             lstLoop.TabIndex = 29;
//             lstLoop.UseCompatibleStateImageBehavior = false;
//             lstLoop.View = System.Windows.Forms.View.Details;
//             tbTabCycleTXWindow.Controls.Add(this.lstLoop);



#endregion
