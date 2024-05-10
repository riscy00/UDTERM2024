/******************************************************************************
 *   Copyright (C) 2017 : Total Vision Bulgaria, Sofia, Bulgaria,
 *
 *   All Right Reserved.
 *****************************************************************************
 *
 *   FILENAME  	: Client Code 
 *   PURPOSE   	: 3rd Party Code goes here with supporting functions
 *   AUTHOR    	: Richard Payne, Total Vision Bulgaria, Sofia.
 *   REVISION  	: UDT LITE 61 (stripped down version from UDT full version).
 *   History   	:
 *   SETUP	   	: VS 2015 and .NET4.5.2. Can use VS2013 and earlier .NET. 
 *   NOTE       : This code make use of reference pattern to share function between top level classes (alternative from Static class/method).
 *   LEGAL      : This code is released to BG Drilling and Sofia University on understanding/agreement it will not be share or published to public domain or 3rd party vendor.
 *              : Please respect this confidentially. The source is private and copyright to Total Vision Bulgaria EOOD, Varna, rpayne@totalvision.pro. 11/6/17 
 ******************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDT_Term_FFT
{
    public partial class ClientCode : Form
    {
        private string RecievedData;
        private int CountByte;  // Number of byte received
        private int CountG;     // Gyro Frame
        private int CountD;     // Survey Frame
        private int CountF;     // Error Frame
        private int CountR;     // Report Frame            
        bool isTfound = false;
        bool isHfound = false;
        //List<string> sFrame;
        List<string> HeaderColumn;
        List<string> FormatColumn;
        List<string> RawDataColumn;
        DataTable DGVtable;
        string HeaderFrame;
        string FormatFrame;
        //private object[] myRow;
        //DateTime dtNow;
        int ColumnNumber;
        //---------------------------------------------------------------

        USB_VCOM_Comm myUSBVCOMComm;        // For Access to VCOM interface for commands.
        GlobalBase myGlobalBase;            // For Access of global variables
        MainProg myMainProg;                // For Access to terminal display
        DMFProtocol myDMFProtocol;          // Callback command and reception protocol
        ITools Tools = new Tools();         // Contains many useful code for conversion (variable to text) and so on.

        //##############################################################################################################
        //=============================================================================================Reference Patterm
        //##############################################################################################################

        #region// ============================================================Reference Object
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
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

        //##############################################################################################################
        //=============================================================================================InitializeComponent
        //##############################################################################################################
        public ClientCode()
        {
            InitializeComponent();
            dgvSurveyViewer.DoubleBufferedMessageSvySurveyDGV(true);

            // Tip: It may be better to init variable and window within ClientCode_Load() rather than here.
        }
        //##############################################################################################################
        //=============================================================================================Window Support Codes
        //##############################################################################################################

        #region //================================================================ClientCode_Load
        //==========================================================
        // Purpose  : This code run when window is loaded. Used to initializes variable once 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void ClientCode_Load(object sender, EventArgs e)
        {
            myMainProg.myRtbTermMessageLF("\n----------------------------------------------------------------------------------");
            myMainProg.myRtbTermMessage("Welcome to Client Code Section. This is template for 3rd party code development");
            myMainProg.myRtbTermMessage("'myMainProg.myRtbTermMessage()' is how to send message into terminal box\n");
            myMainProg.myRtbTermMessageLF("'myMainProg.myRtbTermMessageLF()' include LF");
            myMainProg.myRtbTermMessageLF("--------------------------------------------------------------------------------");
            //------------------------------------------Init your variable here.

            //-----------------------------------------------------------------
        }
        #endregion

        #region //================================================================ClientCode_FormClosing
        //==========================================================
        // Purpose  : This close client code window but not deactivated or disposed. 
        // Input    :  
        // Output   : 
        // Status   :This is required.
        //==========================================================
        private void ClientCode_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            myGlobalBase.ClientCodeWindowVisable = false;
            this.Visible = false;
            this.Hide();
        }
        #endregion

        //##############################################################################################################
        //###################################################################################### Received Survey Data (from ASYNC Logger)
        //##############################################################################################################

        #region//================================================================myClientCode_PreStartSurveySetup
        // Purpose  : This code must be run first before starting survey data. It clear and reset DGV and associated variables. 
        //          : This is activated when STC-START button is click within ASYNC section of the Logger window.
        // Input    :  
        // Output   : 
        // Status   : The code operate on separate threads to avoid locking up system or data transfer activity. 
        //==========================================================

        public delegate void ClientCode_PreStartSurveySetup_StartDelegate();
        //Solution: http://www.dreamincode.net/forums/topic/188209-cross-thread-calls-made-easy/ 
        public void myClientCode_PreStartSurveySetup()
        {
            if (this.Visible == false)
            {
                myGlobalBase.ClientCodeWindowVisable = true;
                this.Visible = true;
                this.Show();
            }
            // Check if we need to call BeginInvoke.
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ClientCode_PreStartSurveySetup_StartDelegate(myClientCode_PreStartSurveySetup));
                return;
            }
            ColumnNumber = 0;
            this.dgvSurveyViewer.RowsDefaultCellStyle.BackColor = Color.Bisque;
            this.dgvSurveyViewer.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;           //Color.Bisque, Color.Beige
            //-------------------------------------- .
            myGlobalBase.EE_isSurveyCVSRawDataEnable = true;
            myGlobalBase.EE_isSurveyCVSRawDataActivate = false;
            //---------------------------------------Reset Counter
            RecievedData = "";
            CountByte = 0;
            CountG = 0;
            CountD = 0;
            CountF = 0;
            CountR = 0;
            //-------------------------------------
            isTfound = false;
            isHfound = false;
            //-------------------------------------Data Table and DataGrid setup
            dgvSurveyViewer.DataSource = null;                  // clear table 1st. 
            HeaderFrame = null;                                  // Reset to null.
            FormatFrame = null;
            DGVtable = null;
            DGVtable = new DataTable();
            dgvSurveyViewer.DataSource = DGVtable;
            //-------------------------------------
            if (HeaderColumn == null)                            // if HeaderColumn is null then init. 
                HeaderColumn = new List<string>();
            else
                HeaderColumn.Clear();
            //--------------------------------------
            if (FormatColumn == null)                            // if FormatColumn is null then init. 
                FormatColumn = new List<string>();
            else
                FormatColumn.Clear();
            //--------------------------------------
            if (RawDataColumn == null)                            // if FormatColumn is null then init. 
                RawDataColumn = new List<string>();
            else
                RawDataColumn.Clear();

            //--------------------------------------Send Message to start download.
            Console.Beep(1000, 20);
        }
        #endregion

        #region //================================================================myClientCodeRecievedData
        // Purpose  : At the start of the process, the tool send header and format type data which goes to frame variable. This also setup datagrid view correctly
        // Input    :  
        // Output   : 
        // Status   : The code operate on separate threads to avoid locking up system or data transfer activity. 
        //==========================================================
        public delegate void myClientCodeRecievedData_StartDelegate(string DataFrame);
        public void myClientCodeRecievedData(string DataFrame)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new myClientCodeRecievedData_StartDelegate(myClientCodeRecievedData), new object[] { DataFrame });
                return;
            }

            if (DataFrame == "")
                return;
            DataFrame = DataFrame.Replace(" ", "");
            DataFrame = DataFrame.Replace("\0", "");        // Aug16
            RecievedData += DataFrame;
            CountByte += DataFrame.Length;

            CountD += Tools.CountOccurrencesString(DataFrame, "$D", false);     // Survey Frame with $D
            CountG += Tools.CountOccurrencesString(DataFrame, "$G", false);     // Gyro Frame with $G
            CountR += Tools.CountOccurrencesString(DataFrame, "$R", false);     // Report Frame with $R
            CountF += Tools.CountOccurrencesString(DataFrame, "$F", false);     // Error Frame with $F

            try
            {
                int n = DataFrame.IndexOf('$');
                DataFrame = DataFrame.Substring(n);
                if ((isTfound == false) | (isHfound == false))      // Format and Header not found?
                {
                    string[] words = DataFrame.Split('\n');
                    {
                        for (int i = 0; i < words.Length; i++)
                        {
                            if (words[i].Contains("$H"))            // Header Name of data frame.
                            {
                                SurveyLoggerCVS_AppendDGV(words[i]);
                                isHfound = true;
                            }
                            if (words[i].Contains("$T"))            // Format Type Frame
                            {
                                SurveyLoggerCVS_AppendDGV(words[i]);
                                isTfound = true;
                            }
                        }

                    }
                }
                else
                {
                    SurveyLoggerCVS_AppendDGV(DataFrame);
                    ClientCodeProcessData();
                }
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("#ERR###: Problem in Message Frame from Logger CVS (crashed exception error in myClientCodeRecievedData()):" + DataFrame);

            }
        }
        #endregion

        #region//================================================================ myClientCodeStop
        public void myClientCodeStop()
        {
            myMainProg.myRtbTermMessageLF("---End of Survey Data Transfer");
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Process Data into DGV and Variable for 3rd party client code use
        //#####################################################################################################

        #region//================================================================SurveyLoggerCVS_AppendDGV
        // Purpose  : This code decode and separate string data into column and then listed in DGV. 
        // Input    :  
        // Output   : 
        // Status   : The code operate on separate threads to avoid locking up system or data transfer activity. 
        //==========================================================
        private bool SurveyLoggerCVS_AppendDGV(string DataFrame)       // false = data not converted, true = data was converted. 
        {
            try
            {
                bool startRIT = false;
                int istartRIT = 0;
                int iendRIT = 0;
                char[] delimiterChars = { ',', ';' };
                //----------------------------------------Remove $E and /n end line
                DataFrame = DataFrame.Replace(",$E", "");
                DataFrame = DataFrame.Replace(";$E", "");
                DataFrame = DataFrame.Replace("\n", "");
                //-----------------------------------------Format Frame for translation. 
                if (DataFrame.Contains("$T"))
                {
                    FormatFrame = DataFrame;
                    string[] partt = FormatFrame.Split(delimiterChars);
                    FormatColumn = new List<string>(partt);
                }
                //---------------------------------------- Header Frame and setup columns       
                if (DataFrame.Contains("$H"))
                {
                    HeaderFrame = DataFrame;
                    string[] parth = HeaderFrame.Split(delimiterChars);
                    HeaderColumn = new List<string>(parth);
                    ColumnNumber = parth.Length;
                    //---------------------------------------------Add header to Column.
                    for (int i = 0; i < ColumnNumber; i++)
                    {
                        DGVtable.Columns.Add(parth[i]);
                    }
                    //--------------------------------------------Tidy Up DGV.
                    dgvSurveyViewer.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);    // Resize the master DataGridView columns to fit the newly loaded data.
                    dgvSurveyViewer.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dgvSurveyViewer.RowHeadersWidth = 60;

                }
                //-----------------------------------------
                if ((DataFrame.Contains("$D")) | (DataFrame.Contains("$G")))        // Data and Data2 Frame.
                {
                    string[] sData = DataFrame.Split(delimiterChars);
                    for (int y = 1; y < sData.Length; y++)
                    {
                        switch (FormatColumn[y])
                        {
                            case ("0w"):        // INT16 only
                                {
                                    sData[y] = Tools.HexStringtoInt16(sData[y]).ToString("D");
                                    break;
                                }
                            case ("0q"):        // uINT16 only
                                {
                                    sData[y] = Tools.HexStringtoUInt16(sData[y]).ToString("D");
                                    break;
                                }
                            case ("0u"):        // convert hex to unsigned integer. 
                                {
                                    // Already number (not hex, nothing)
                                    break;
                                }
                            case ("0i"):        // convert hex to  integer
                                {
                                    // Already number (not hex, nothing)
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
                            default:
                                break;
                        }

                    }
                    //------------------------------------------------------------------------
                    if (startRIT == false)
                    {
                        startRIT = true;
                        istartRIT = System.Convert.ToInt32(sData[1]);      // 1st RIT value. 
                    }
                    iendRIT = System.Convert.ToInt32(sData[1]);            // end RIT value.
                                                                           
                    DataFrame = string.Join(",", sData);
                    RawDataColumn.Clear();
                    RawDataColumn = new List<string>(sData);
                }
                else
                {
                    DataFrame = DataFrame.Replace(';', ',');
                }
                //#########################################################################################DataGridView
                if (DataFrame.Contains("$G"))
                {
                    string[] partdf = DataFrame.Split(delimiterChars);
                    DataRow dr = DGVtable.NewRow();
                    //---------------------------------------------Add header to Column.
                    for (int i = 0; i < ColumnNumber; i++)
                    {
                        if (i < partdf.Length)
                            dr[i] = partdf[i].ToString();
                        else
                            dr[i] = "";
                    }
                    DGVtable.Rows.Add(dr);
                    //DGVtable.Rows.Add(DataFrame.Split(','));
                }
                if (DataFrame.Contains("$D"))
                {
                    string[] partdf = DataFrame.Split(delimiterChars);
                    DataRow dr = DGVtable.NewRow();
                    //---------------------------------------------Add header to Column.
                    for (int i = 0; i < ColumnNumber; i++)
                    {
                        if (i < partdf.Length)
                            dr[i] = partdf[i].ToString();
                        else
                            dr[i] = "";
                    }
                    DGVtable.Rows.Add(dr);
                    //DGVtable.Rows.Add(DataFrame.Split(','));
                }
                if ((DataFrame.Contains("$F")) | (DataFrame.Contains("$R")))
                {
                    DGVtable.Rows.Add(DataFrame.Split(','));
                }
                foreach (DataGridViewRow row in dgvSurveyViewer.Rows)
                {
                    row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
                }
                Survey_AllDataCollected_Update_DGV();
            }
            catch { }
            //try  // Had to this to avoid crashes when RowCount=0 (negative number). 
            //{
            //    if (dgvSurveyViewer.RowCount != 0)
            //        dgvSurveyViewer.FirstDisplayedScrollingRowIndex = dgvSurveyViewer.RowCount - 1;     // view to last row. 
            //}
            //catch { }
            return (true);
        }
        #endregion

        #region//================================================================Survey_AllDataCollected_Update_DGV
        public delegate void Survey_AllDataCollected_Update_DGVDelegate();
        public void Survey_AllDataCollected_Update_DGV()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Survey_AllDataCollected_Update_DGVDelegate(Survey_AllDataCollected_Update_DGV));
                return;
            }
            if (dgvSurveyViewer == null)
            {
                Survey_AppendFile_with_Exception_Error("+WARN: Survey_AllDataCollected_Update_DGV() has null object detected, may ignore this message");
                return;
            }
            try     // This avoid JIT pop-up message when occurred due to null object exception, possibly in dgvSurveyViewer
            {
                dgvSurveyViewer.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                dgvSurveyViewer.PerformLayout();
                //---------------------------------------------Auto scroll management.
                if (cbSuspendScroll.Checked == false)
                {
                    if (dgvSurveyViewer.RowCount >= 1)
                    {
                        dgvSurveyViewer.FirstDisplayedScrollingRowIndex = dgvSurveyViewer.RowCount - 1;     // view to last row. 
                    }
                }
                dgvSurveyViewer.Invalidate();
            }
            catch
            {
                Survey_AppendFile_with_Exception_Error("+WARN: Survey_AllDataCollected_Update_DGV() has exception occurance, may ignore this message");
            }
        }
        #endregion

        #region//================================================================Survey_AppendFile_with_Exception_Error
        void Survey_AppendFile_with_Exception_Error(string text)
        {
            try
            {
                myMainProg.myRtbTermMessageLF(text);
                //if (sAppendFilename != "")
                //{
                //    text = text + "\r\n";
                //    System.IO.File.AppendAllText(sAppendFilename, text);
                //}
            }
            catch { }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### 3rd Party Client Code goes here.
        //#####################################################################################################

        #region//================================================================ClientCodeProcessData
        // Purpose  : THis is your client code for calibration, math, etc purpose.  
        // Input    :  
        // Output   : 
        // Status   : All data are UINT results. 
        //==========================================================
        private void ClientCodeProcessData()       // false = data not converted, true = data was converted. 
        {
            //FormatColumn,         Format Type
            //HeaderColumn          Header Name, which point to certain column of interest. 
            //RawDataColumn         Data in string (use Tool (ITools Tools = new Tools();) feature for conversion to int or uint)
            // In case you do not need Gyro, you can skip $G frame. 
            //Good luck. 

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Send Command Example
        //#####################################################################################################

        #region //================================================================Command_ASCII_Process()
        public void Survey_Command_ASCII_Process(string sCommand)       // Alway COMMAND(Para0;Para1;Para2;Para3).  Support 32 bits with prefix type: 0x as hex, 0i as int, 0u as uint and 0s string  ie SETUP(0x67FF; 0i2341; 0sMessage)
        {
            myUSBVCOMComm.VCOM_Message_Send(sCommand);                  // But it will not intercept response message to do this need DFMP protocol. Ask for more details. 
        }

        #endregion


        //#####################################################################################################
        //###################################################################################### DGV Right Click Mouse
        //#####################################################################################################

        #region //================================================================dgvSurveyViewer_CellMouseClick
        private void dgvSurveyViewer_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    DataGridViewCell clickedCell = (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex];

                    // Here you can do whatever you want with the cell
                    this.dgvSurveyViewer.CurrentCell = clickedCell;  // Select the clicked cell, for instance

                    //----------------------------------------------Add context menu
                    System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
                    System.Windows.Forms.MenuItem menuItem1 = new System.Windows.Forms.MenuItem("Save");
                    menuItem1.Click += new EventHandler(ContexMenuSaveAction);
                    System.Windows.Forms.MenuItem menuItem2 = new System.Windows.Forms.MenuItem("Copy");
                    menuItem2.Click += new EventHandler(ContexMenuCopyAction);
                    contextMenu.MenuItems.Add(menuItem1);
                    contextMenu.MenuItems.Add(menuItem2);
                    //----------------------------------------------
                    DataGridViewCell clickedStart = (sender as DataGridView).Rows[e.RowIndex].Cells[0];
                    DataGridViewCell clickedRTC = (sender as DataGridView).Rows[e.RowIndex].Cells[6];      // RTC column. 
                    DataGridViewCell clickedStatus = (sender as DataGridView).Rows[e.RowIndex].Cells[8];    // Status Infomation

                    //if (clickedStart.Value.ToString().Contains("$D") == true)
                    //{
                    //    TimeSpan time = TimeSpan.FromSeconds(Convert.ToDouble(clickedRTC.Value.ToString()));

                    //    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).Add(time);
                    //    contextMenu.MenuItems.Add(new MenuItem(string.Format("UTC: " + clickedRTC.Value.ToString())));
                    //    contextMenu.MenuItems.Add(new MenuItem(string.Format("Time: {0:HH/mm/ss}", dateTime)));
                    //    contextMenu.MenuItems.Add(new MenuItem(string.Format("Date: {0:dd/MM/yy}", dateTime)));
                    //}

                    var relativeMousePosition = dgvSurveyViewer.PointToClient(Cursor.Position);
                    contextMenu.Show(dgvSurveyViewer, relativeMousePosition);
                }
            }
        }
        #endregion

        #region //================================================================ContexMenuCopyAction
        //http://stackoverflow.com/questions/899350/how-to-copy-the-contents-of-a-string-to-the-clipboard-in-c 
        // The code is what you see is what you capture, ie taken from Datagridview not from sframe[].
        // It also sort polarity to ensure correct timing sequence.
        // It filter out spurious \r and then add \n. It has two comma left but no serious impact on excel or notepad view. 
        // Fully tested and working fine.
        // The outstanding issue is thread related to STA.....so far it not really needed. 
        private void ContexMenuCopyAction(object sender, EventArgs e)
        {
            string Rdata = HeaderFrame + '\n';
            string sb = "";
            //-----------------------------------------------reverse or forward order
            if (dgvSurveyViewer.SelectedRows[0].Index > dgvSurveyViewer.SelectedRows[dgvSurveyViewer.SelectedRows.Count - 1].Index)
            {
                List<DataGridViewRow> rows = (from DataGridViewRow row in dgvSurveyViewer.SelectedRows where !row.IsNewRow orderby row.Index select row).ToList<DataGridViewRow>();
                for (int i = 0; i < rows.Count; i++)
                {
                    sb = "";
                    for (int col = 0; col < rows[i].Cells.Count; col++)
                    {
                        if (rows[i].Cells[col].Value.ToString() != "")
                        {
                            sb = sb + rows[i].Cells[col].Value.ToString() + ',';
                            sb = sb.Replace("\r", "");                               // remove certain char that causing spurious issue in excel. 
                            sb = sb.Replace("\n", "");
                            sb = sb.Replace("\0", "");
                        }
                    }
                    Rdata = Rdata + sb + '\n'; // that will give you the string you desire

                }
            }
            else
            {
                foreach (DataGridViewRow row in dgvSurveyViewer.SelectedRows)
                {
                    sb = "";
                    for (int col = 0; col < row.Cells.Count; col++)
                    {
                        if (row.Cells[col].Value.ToString() != "")
                        {
                            sb = sb + row.Cells[col].Value.ToString() + ',';
                            sb = sb.Replace("\r", "");                               // remove certain char that causing spurious issue in excel. 
                            sb = sb.Replace("\n", "");
                            sb = sb.Replace("\0", "");
                        }
                    }
                    Rdata += sb + '\n'; // that will give you the string you desire
                }

            }
            System.Windows.Forms.Clipboard.SetText(Rdata);
            myMainProg.myRtbTermMessageLF("-INFO: Selected Data in Clipboard Successfully");
        }

        #endregion

        #region //================================================================ContexMenuSaveAction
        // The code is what you see is what you capture, ie taken from Datagridview not from sframe[].
        // It also sort polarity to ensure correct timing sequence.
        // It filter out spurious \r and then add \n. It has two comma left but no serious impact on excel or notepad view. 
        // Fully tested and working fine. 
        private void ContexMenuSaveAction(object sender, EventArgs e)
        {
            RecievedData = HeaderFrame + '\n';
            StringBuilder sb = new StringBuilder();
            //-----------------------------------------------reverse or forward order
            if (dgvSurveyViewer.SelectedRows[0].Index > dgvSurveyViewer.SelectedRows[dgvSurveyViewer.SelectedRows.Count - 1].Index)
            {
                List<DataGridViewRow> rows = (from DataGridViewRow row in dgvSurveyViewer.SelectedRows where !row.IsNewRow orderby row.Index select row).ToList<DataGridViewRow>();
                for (int i = 0; i < rows.Count; i++)
                {
                    sb.Clear();
                    for (int col = 0; col < rows[i].Cells.Count; col++)
                    {
                        if (rows[i].Cells[col].Value.ToString() != "")
                        {
                            sb.Append(rows[i].Cells[col].Value.ToString() + ',');
                            sb.Replace("\r", "");                               // remove certain char that causing spurious issue in excel. 
                            sb.Replace("\n", "");
                            sb.Replace("\0", "");

                        }
                    }
                    RecievedData += sb.ToString() + '\n'; // that will give you the string you desire
                }
            }
            else
            {
                foreach (DataGridViewRow row in dgvSurveyViewer.SelectedRows)
                {
                    sb.Clear();
                    for (int col = 0; col < row.Cells.Count; col++)
                    {
                        if (row.Cells[col].Value.ToString() != "")
                        {
                            sb.Append(row.Cells[col].Value.ToString() + ',');
                            sb.Replace("\r", "");                               // remove certain char that causing spurious issue in excel. 
                            sb.Replace("\n", "");
                            sb.Replace("\0", "");
                        }
                    }
                    RecievedData += sb.ToString() + '\n'; // that will give you the string you desire
                }

            }

            //-------------------------------------------------Save to filename under SelectSave_<timestamp>
            SurveyCVS_OpenSaveDataCloseFile("SnippedRow");
            myMainProg.myRtbTermMessageLF("-INFO: Selected Data Saved Successfully");
            RecievedData = null;
        }
        #endregion

        #region //================================================================SurveyCVS_OpenSaveDataCloseFile
        bool SurveyCVS_OpenSaveDataCloseFile(string sfilenametitle)
        {
            string sFoldername= "";
            if (myGlobalBase.sFoldername == "")
            {
                
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    //
                    // The user selected a folder and pressed the OK button.
                    // We print the number of files found.
                    //
                    sFoldername = folderBrowserDialog1.SelectedPath;
                    if (!Directory.Exists(sFoldername))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(sFoldername);  // Create folder if not exist. 
                    }
                }
                myGlobalBase.sFoldername = sFoldername;
            }
            string sFilename = System.IO.Path.Combine(myGlobalBase.sFoldername, sfilenametitle + DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + ".csv");
            myMainProg.myRtbTermMessageLF("-INFO: Saving Downloaded Filename: " + sFilename + ".csv");
            try
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(sFilename, true, Encoding.ASCII);
                    sw.Write(RecievedData);                                         // Dataframe (assuming it has \n\0 terminator)

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

        #region //================================================================openFolderToolStripMenuItem_Click
        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string myPath = myGlobalBase.sFoldername;
                if (myPath == null)             // No filename path, quit. 
                    return;
                if (myPath == "")
                    return;
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = myPath;
                prc.Start();
            }
            catch
            {
                myMainProg.myRtbTermMessageLF ("#ERROR: Open Folder has no folder/path or attempt to use restricted domains of Drive C");
            }
        }
        #endregion
    }

    #region//=============================================================DoubleBuffered property via reflection for datagridview, improve flickerless image.
    public static class ExtensionMethodsMessageSurveyDGV
    {
        public static void DoubleBufferedMessageSvySurveyDGV(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    #endregion
}
