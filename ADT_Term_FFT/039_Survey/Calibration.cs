using JR.Utils.GUI.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace UDT_Term_FFT
{
    public partial class Calibration : Form
    {
        MainProg myMainProg;
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        Survey mySurvey;
        System.Windows.Forms.DataGridView dgvSurveyViewer;
        List<RowCalConfig> myRowCalConfig;
        private bool suspendevent;

        //--------------------------------------Public Getter/Setter
        public bool isMinimise { get; set; }
        public string sDefaultFolder { get; set; }
        public string sFilanameDGV { get; set; }

        #region //============================================================Reference Object
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        public void MydgvSurveyViewer(System.Windows.Forms.DataGridView dgvSurveyViewerRef)
        {
            dgvSurveyViewer = dgvSurveyViewerRef;
        }
        public void MySurveyRef(Survey mySurveyRef)
        {
            mySurvey = mySurveyRef;
        }
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
            //--------------------------------------Themes
            switch (myGlobalBase.CompanyName)
            {
                case (int)GlobalBase.eCompanyName.BGDS:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.BG_MainPage_Blank_1B;
                        break;
                    }
                case (int)GlobalBase.eCompanyName.ADT:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_Blank_1B;
                        break;
                    }
                case ((int)GlobalBase.eCompanyName.TVB):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPageTVBBlank_1B;
                        break;
                    }
                default:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_Blank_1B;
                        break;
                    }
            }
            this.Refresh();
        }
        #endregion
        public Calibration()
        {
            InitializeComponent();
            dgvCalListx.DoubleBufferedMessageSvySurveyDGV(true);
            isMinimise = false;
            sFilanameDGV = @"CalSetup";
        }

        //#####################################################################################################
        //###################################################################################### Form Manager
        //#####################################################################################################

        #region //================================================================Calibration_FormClosing
        private void Calibration_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            myGlobalBase.Svy_isCalibrationOpen = false;                 // Cease Survey CVS Terminal mode.
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================Calibration_Show
        public void Calibration_Show()
        {
            //-------------------------------------------------Manage window. 
            this.BringToFront();
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            myGlobalBase.Svy_isCalibrationOpen = true;
            this.Visible = true;
            this.Show();
        }
        #endregion

        #region //================================================================Survey_Load
        private void Calibration_Load(object sender, EventArgs e)
        {
            Calibration_Init();

        }
        #endregion

        #region //================================================================Calibration_Init
        public void Calibration_Init()
        {
            suspendevent = true;
            this.dgvCalListx.RowsDefaultCellStyle.BackColor = Color.Bisque;
            this.dgvCalListx.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;           //Color.Bisque, Color.Beige
            if (myRowCalConfig == null)
            {
                myRowCalConfig = new List<RowCalConfig>();
            }
            myRowCalConfig.Clear();
            for (int i = 4; i < dgvSurveyViewer.ColumnCount; i++)
            {
                //---------------------------------------------------------------------------Setup Array
                myRowCalConfig.Add(new RowCalConfig());
                myRowCalConfig[i - 4].ColNumber = i;
                myRowCalConfig[i - 4].ColName = dgvSurveyViewer.Columns[i].Name;
            }
            Cal_UpdateDGVTableColumn();
            Cal_UpdateDGVTableRows();
            dgvCalListx.CellValueChanged += new DataGridViewCellEventHandler(dgvRegBit_CellValueChanged);
            dgvCalListx.CurrentCellDirtyStateChanged += new EventHandler(dgvRegBit_CurrentCellDirtyStateChanged);
            suspendevent = false;

        }
        #endregion


        //#####################################################################################################
        //###################################################################################### Process Data
        //#####################################################################################################
        #region //================================================================Cal_ProcessRow
        // StartRow which link to sensor column in Survey, for simple type, then it for each sensor, for Method-1/-2 type then it must start from X of the sensor X-Y-Z and following sensor X-Y-Z fashion.
        // rawData is the List double array which come from column data in Survey. 
        public List<double> Cal_ProcessRow(int startrow, List<double> rawData)
        {
            if ((myRowCalConfig==null) || (myRowCalConfig.Count==0) || (startrow >= myRowCalConfig.Count))
                return rawData;
            if (myRowCalConfig[startrow].isEnable == false)     // is this feature enabled or not..... inspect at the start row only for non-simple method.  
                return rawData;
            //------------------------------
            List<double> calData = new List<double>();
            for (int i = 0; i < rawData.Count; i++)
            {
                calData.Add(0);
            }
            //------------------------------
            switch (myRowCalConfig[startrow].SelectMethod)
            {
                case (0):       // Simple
                    {
                        double calD;
                        calD = rawData[0] + myRowCalConfig[startrow].Offset;     // oFFSET and then GAIN in this sequence. predecease order. 
                        calD = calD * myRowCalConfig[startrow].Gain;
                        calData[0] = Math.Round(calD, 4, MidpointRounding.AwayFromZero);
                        break;
                    }
                case (1):       // Method-1
                    {
                        calData = Cal_ProcessMethod1(startrow, rawData);
                        break;
                    }
                case (2):       // Method-2
                    {
                        calData = Cal_ProcessMethod2(startrow, rawData);
                        break;
                    }
            }
            return calData;
        }
        #endregion
        #region //================================================================Cal_ProcessMethod1
        private List<double> Cal_ProcessMethod1(int row, List<double> rawData)
        {
            //-------------------------------Process Method 1
            //-------------------------------Return Data
            return rawData;

        }
        #endregion
        #region //================================================================Cal_ProcessMethod2
        private List<double> Cal_ProcessMethod2(int row, List<double> rawData)
        {
            //-------------------------------Process Method 2
            //-------------------------------Return Data
            return rawData;

        }
        #endregion


        //#####################################################################################################
        //###################################################################################### DataGridView Manager
        //#####################################################################################################

        #region //================================================================Cal_UpdateDGVTableColumn
        private void Cal_UpdateDGVTableColumn()
        {
            dgvCalListx.Rows.Clear();
            dgvCalListx.Columns.Clear();
            System.Windows.Forms.DataGridViewCellStyle dgvStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            dgvStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            var colNumber = new DataGridViewTextBoxColumn();
            var colName = new DataGridViewTextBoxColumn();
            var colEnable = new DataGridViewCheckBoxColumn(); //new DataGridViewTextBoxColumn();
            var colSelMethod = new DataGridViewComboBoxColumn();
            var colVariableRef = new DataGridViewTextBoxColumn();
            var colGain = new DataGridViewTextBoxColumn();
            var colOffset = new DataGridViewTextBoxColumn();
            var colNote = new DataGridViewTextBoxColumn();

            // 
            #region// colNumber (reference to column number of the data, range from 4 to many)
            // 
            colNumber.DefaultCellStyle = dgvStyle1;
            colNumber.HeaderText = "No";
            colNumber.Name = "colNo";
            colNumber.ReadOnly = true;
            colNumber.Width = 40;
            colNumber.MinimumWidth = 40;
            #endregion
            // 
            #region// colName, sensor name, linked to survey header name. 
            // 
            colName.DefaultCellStyle = dgvStyle1;
            colName.HeaderText = "Name";
            colName.Name = "colName";
            colName.ReadOnly = true;
            colName.Width = 80;
            colName.MinimumWidth = 80;
            #endregion
            // 
            #region// colEnable, checkbox that enable or disable calibration feature, disable mean simple gain/offset is not processed. 
            //
            colEnable.DefaultCellStyle = dgvStyle1;
            colEnable.HeaderText = "Enable";
            colEnable.Name = "colEnable";
            colEnable.ValueType = typeof(bool);
            colEnable.FalseValue = false;
            colEnable.TrueValue = true;
            colEnable.Width = 60;
            colEnable.MinimumWidth = 60;

            #endregion
            // 
            #region// colSelMethod. Select Method which is Simple (Gain/Offset), Method-1, Method-2, etc. 
            // 
            colSelMethod.DefaultCellStyle = dgvStyle1;
            colSelMethod.HeaderText = "Cal Method";
            colSelMethod.Name = "colMethod";
            colSelMethod.ReadOnly = false;
            colSelMethod.Width = 90;
            colSelMethod.MinimumWidth = 90;
            colSelMethod.DataPropertyName = "calMethod";
            colSelMethod.ValueType = typeof(String);

            var list11 = new List<string>() { "Simple", "Method-1", "Method-2" };
            colSelMethod.DataSource = list11;

            //colSelMethod.MaxDropDownItems = 3;
            //colSelMethod.Items.Add("Simple");
            //colSelMethod.Items.Add("Method-1");
            //colSelMethod.Items.Add("Method-2");

            #endregion
            // 
            #region// colVarRef. data is passed to calibration code into co-efficient equation and provide result back to same column. 
            // 
            colVariableRef.DefaultCellStyle = dgvStyle1;
            colVariableRef.HeaderText = "Cal Ref";
            colVariableRef.Name = "colVarRef";
            colVariableRef.ReadOnly = false;
            colVariableRef.Width = 80;
            colVariableRef.MinimumWidth = 80;
            #endregion    
            // 
            #region// colGain. gain setting. 
            // 
            colGain.DefaultCellStyle = dgvStyle1;
            colGain.HeaderText = "Gain";
            colGain.Name = "colGain";
            colGain.ReadOnly = false;
            colGain.Width = 80;
            colGain.MinimumWidth = 80;
            #endregion   
            // 
            #region// colOffset. Offset setting. 
            // 
            colOffset.DefaultCellStyle = dgvStyle1;
            colOffset.HeaderText = "Offset";
            colOffset.Name = "colOffset";
            colOffset.ReadOnly = false;
            colOffset.Width = 80;
            colOffset.MinimumWidth = 80;

            #endregion   
            // 
            #region// colNote. short note for reference.  
            // 
            colNote.DefaultCellStyle = dgvStyle1;
            colNote.HeaderText = "Note";
            colNote.Name = "colNote";
            colNote.ReadOnly = false;
            colNote.Width = 300;
            colNote.MinimumWidth = 300;
            #endregion

            dgvCalListx.Columns.AddRange(new DataGridViewColumn[] { colNumber, colName, colEnable, colSelMethod, colVariableRef, colGain, colOffset, colNote });
        }
        #endregion

        #region //================================================================Cal_UpdateDGVTableRow
        private void Cal_UpdateDGVTableRows()
        {
            int i;
            suspendevent = true;
            dgvCalListx.Rows.Clear();
            for (i = 0; i < dgvSurveyViewer.ColumnCount - 4; i++)
            {
                Cal_AddRow(i);
            }
            suspendevent = false;

        }
        #endregion

        #region //================================================================Cal_AddRow
        private void Cal_AddRow(int i)
        {
            DataGridViewRow newRow = (DataGridViewRow)dgvCalListx.RowTemplate.Clone();
            newRow.CreateCells(dgvCalListx);
            var index = dgvCalListx.Rows.Add(newRow);
            dgvCalListx.Rows[index].Cells["colNo"].Value = myRowCalConfig[i].ColNumber.ToString();
            dgvCalListx.Rows[index].Cells["colName"].Value = myRowCalConfig[i].ColName;
            //-----------------------------------------------------CheckBox
            dgvCalListx.Rows[index].Cells["colEnable"].Value = myRowCalConfig[i].isEnable;
            //-----------------------------------------------------colMethod
            switch (myRowCalConfig[i].SelectMethod)
            {
                case (0):
                    dgvCalListx.Rows[index].Cells["colMethod"].Value = "Simple";
                    break;
                case (1):
                    dgvCalListx.Rows[index].Cells["colMethod"].Value = "Method-1";
                    break;
                case (2):
                    dgvCalListx.Rows[index].Cells["colMethod"].Value = "Method-2";
                    break;
            }
            //-----------------------------------------------------
            dgvCalListx.Rows[index].Cells["colVarRef"].Value = myRowCalConfig[i].SelectVariableRef;
            dgvCalListx.Rows[index].Cells["colGain"].Value = myRowCalConfig[i].Gain.ToString();
            dgvCalListx.Rows[index].Cells["colOffset"].Value = myRowCalConfig[i].Offset.ToString();
            dgvCalListx.Rows[index].Cells["colNote"].Value = myRowCalConfig[i].Note;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Cell value change event.
        //#####################################################################################################

        #region //================================================================dgvRegBit_CurrentCellDirtyStateChanged
        // This event handler manually raises the CellValueChanged event 
        // by calling the CommitEdit method. 
        void dgvRegBit_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dgvCalListx.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                dgvCalListx.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        #endregion

        #region //================================================================dgvRegBit_CellValueChanged
        private void dgvRegBit_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dgvRegBit_UpdateToClass();
        }
        #endregion

        #region //================================================================dgvRegBit_Update
        private void dgvRegBit_UpdateToClass()
        {
            int row;
            int col;
            if (suspendevent == true)
                return;
            if (dgvCalListx == null)
                return;
            for (row = 0; row < dgvCalListx.Rows.Count; row++)
            {
                for (col = 0; col < dgvCalListx.Columns.Count; col++)
                {
                    //try
                    //{
                        switch (col)
                        {
                            case (0):       // Co
                                {
                                    break;
                                }
                            case (1):       // Name (Skipped)
                                {

                                    break;
                                }
                            case (2):       // isEnable (Skipped).
                                {
                                    myRowCalConfig[row].isEnable = (bool) dgvCalListx.Rows[row].Cells["colEnable"].Value;
                                    break;
                                }
                            case (3):       // Simple or method.
                                {
                                    DataGridViewComboBoxCell dgvcmbcell = dgvCalListx["colMethod", row] as DataGridViewComboBoxCell;
                                    switch ((string)dgvcmbcell.Value)
                                    {
                                        case ("Simple"):
                                        {
                                            myRowCalConfig[row].SelectMethod = 0;
                                            break;
                                        }
                                        case ("Method-1"):
                                        {
                                            myRowCalConfig[row].SelectMethod = 1;
                                            break;
                                        }
                                        case ("Method-2"):
                                        {
                                            myRowCalConfig[row].SelectMethod = 2;
                                            break;
                                        }
                                        case ("Method-3"):
                                        {
                                            myRowCalConfig[row].SelectMethod = 3;
                                            break;
                                        }
                                    default:
                                            break;
                                    }
                                    break;
                                }
                            case (4):   // VarRef for coff data input to Method1/2 equation  
                                {
                                    try
                                    {
                                        if (dgvCalListx["colVarRef", row].Value != null)
                                            myRowCalConfig[row].SelectVariableRef =
                                                (string)dgvCalListx["colVarRef", row].Value;
                                    }
                                    catch
                                    {
                                        dgvCalListx["colVarRef", row].Value = (string)myRowCalConfig[row].SelectVariableRef;
                                    }
                                    //#### Validate variable to ensure error free. 

                                    break;
                                }
                            case (5):   // Gain
                                {

                                    if ((dgvCalListx["colGain", row].Value != null) && (Tools.IsString_Double((string)dgvCalListx["colGain", row].Value) == true))
                                        myRowCalConfig[row].Gain =
                                                Tools.ConversionStringtoDouble((string)dgvCalListx["colGain", row].Value);
                                    else
                                    {
                                        dgvCalListx["colGain", row].Value = myRowCalConfig[row].Gain.ToString();
                                    }
                                    break;

                                }
                            case (6):   // Offset
                                {

                                    if ((dgvCalListx["colOffset", row].Value != null) && (Tools.IsString_Double((string)dgvCalListx["colOffset", row].Value) == true))
                                        myRowCalConfig[row].Offset =
                                            Tools.ConversionStringtoDouble((string)dgvCalListx["colOffset", row].Value);
                                    else
                                    {
                                        dgvCalListx["colOffset", row].Value = myRowCalConfig[row].Offset.ToString();
                                    }
                                    break;
                                }
                            case (7):   // Note
                                {
                                    if (dgvCalListx["colNote", row].Value != null)
                                        myRowCalConfig[row].Note = (string)dgvCalListx["colNote", row].Value;
                                    break;
                                }
                            default:
                                break;
                        }

                    //}
                    //catch
                    //{
                    //    Debug.Print("###ERR: dgvRegBit_Update(), reported exception error, check code");
                    //}

                }
            }

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Checkbox value change event.
        //#####################################################################################################

        #region //================================================================dgvCalListx_CellContentClick
        private void dgvCalListx_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)                 // Check box only
            {
                //DataGridViewCheckBoxCell ch1 = new DataGridViewCheckBoxCell();
                //ch1 = (DataGridViewCheckBoxCell)dgvCalListx.Rows[e.RowIndex].Cells[e.ColumnIndex];
                dgvRegBit_UpdateToClass();
            }
        }
        #endregion



        //#####################################################################################################
        //###################################################################################### File Manager
        //#####################################################################################################

        #region //================================================================cScope_DefaultFolder
        public void cCal_DefaultFolder(string sdefaultFolder)   // If default folder left blank, then use user document file location. 
        {
            try
            {
                if (sDefaultFolder == "")
                {
                    if (sdefaultFolder == "")       // If empty then use user myDocument location. 
                    {
                        sDefaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        sDefaultFolder = sDefaultFolder + @"\Calibration";
                    }
                    else                            // User defined folder. 
                    {
                        sDefaultFolder = sdefaultFolder;
                    }
                    System.IO.Directory.CreateDirectory(sDefaultFolder);
                }
                else  // sDefaultFolder was defined, so check folder for existence if not create it. 
                {
                    sDefaultFolder = sdefaultFolder + @"\Calibration";
                    System.IO.Directory.CreateDirectory(sDefaultFolder);
                }
            }
            catch
            {
                sDefaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                sDefaultFolder = sDefaultFolder + @"\Calibration";
                System.IO.Directory.CreateDirectory(sDefaultFolder);
            }


        }


        #endregion

        #region //================================================================btnLoad_Click
        private void btnLoad_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofdCalibration = new OpenFileDialog();
            //----------------------------------------------------------XML Load File Section. 
            BindingList<RowCalConfig> blLoadData = null;
            ofdCalibration.Filter = @"xml files (*.xml)|*.xml";
            ofdCalibration.InitialDirectory = sDefaultFolder;
            ofdCalibration.Title = @"Load XML Calibration Setup";
            DialogResult dr = ofdCalibration.ShowDialog();
            if (dr == DialogResult.OK)
            {
                blLoadData = myGlobalBase.DeserializeFromFile<BindingList<RowCalConfig>>(ofdCalibration.FileName);
                if (blLoadData == null)
                {
                    MessageBox.Show(@"Unable to Load Calibration Setting Configuration File (XML)",
                        @"Load Calibration File Error",
                        MessageBoxButtons.OK,
                        icon: MessageBoxIcon.Error);
                }
                else
                {
                    myRowCalConfig.Clear();
                    for (int i = 0; i < blLoadData.Count; i++)
                    {
                        myRowCalConfig.Add(new RowCalConfig());
                        myRowCalConfig[i] = blLoadData[i];
                    }
                    Cal_UpdateDGVTableRows();

                }
            }
            else
            {
                MessageBox.Show(@"Load Calibration Setup: Filename Error",
                    @"Load Calibration Setup File Error",
                    MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error);
            }
        }
        #endregion

        #region //================================================================btnSave_Click
        private void btnSave_Click(object sender, EventArgs e)
        {
            dgvRegBit_UpdateToClass();
            BindingList<RowCalConfig> blDataOp = new BindingList<RowCalConfig>(myRowCalConfig);

            System.Windows.Forms.SaveFileDialog sfdCalibration = new SaveFileDialog();
            string sTimeStampNow = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
            sfdCalibration.Filter = @"xml files (*.xml)|*.xml";
            sfdCalibration.InitialDirectory = sDefaultFolder;
            sfdCalibration.FileName = System.IO.Path.Combine(sDefaultFolder, sFilanameDGV + "_" + sTimeStampNow);
            sfdCalibration.Title = @"Export to XML File for Scope Gain/Offset Setting";

            DialogResult dr = sfdCalibration.ShowDialog();
            if (dr == DialogResult.OK)
            {
                myGlobalBase.SerializeToFile<BindingList<RowCalConfig>>(blDataOp, sfdCalibration.FileName);
            }
            else
            {
                if ((dr == DialogResult.Cancel) || (dr == DialogResult.No))
                {
                    return;
                }
                MessageBox.Show(@"Save Calibration Setup: Filename Error",
                    @"Save Calibration Setup Error",
                    MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error);
            }
        }
        #endregion

        #region //================================================================btnReset_Click
        private void btnReset_Click(object sender, EventArgs e)
        {
            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            DialogResult response = FlexiMessageBox.Show("Warning, you about to reset all preset data"
            + Environment.NewLine + "OK     = Proceed reset data. All data will be lost!"
            + Environment.NewLine + "Cancel = Cancel, maybe I should save data first?",
            "Reset Calibration Table",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Warning);
            if (response != DialogResult.OK)
                return;
            //-----------------------------------------------
            myRowCalConfig.Clear();
            for (int i = 4; i < dgvSurveyViewer.ColumnCount; i++)
            {
                //---------------------------------------------------------------------------Setup Array
                myRowCalConfig.Add(new RowCalConfig());
                myRowCalConfig[i - 4].ColNumber = i;
                myRowCalConfig[i - 4].ColName = dgvSurveyViewer.Columns[i].Name;
            }
            Cal_UpdateDGVTableRows();
        }
        #endregion

        #region //================================================================BtnOpenFolder_Click
        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string myPath = sDefaultFolder;
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = myPath;
                prc.Start();

            }
            catch
            {
                Debug.Print("###ERR: Unable to open folder. BtnOpenFolder_Click() in Calibration Window");

            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Calibration Class Object
        //#####################################################################################################
        #region --------------------------------------------------------Class ScopeData_Operation
        [Serializable]
        public class RowCalConfig
        {
            public int ColNumber { get; set; }                              // Column number from 4 onward (0 to 3 is not sensor data)
            public string ColName { get; set; }                             // String Name of the column for reference only. 
            public bool isEnable { get; set; }                              // false, not used, raw data only. True = enable data processing. 
            public int SelectMethod { get; set; }                           // 0= Simple (Gain/Offset), 1=Method1 (Calibration), 2=Method2 (Calibration), etc. 
            public string SelectVariableRef { get; set; }                   // Data entry to referenced calibration variable, -1 = Not selected. 
            public double Gain { get; set; }                                // Simple Gain
            public double Offset { get; set; }                              // Simple Offset
            public string Note { get; set; }                                // short description and note. 
                                                                            // =====================================================Constructor
            public RowCalConfig()
            {
                ColNumber = -1;
                ColName = "";
                isEnable = false;
                SelectMethod = 0;                   // use index pair?
                SelectVariableRef = "N/A";
                Gain = 1;
                Offset = 0;
                Note = "";
            }
        }
        #endregion
    }

    //#####################################################################################################
    //###################################################################################### Class Mag_Data
    //#####################################################################################################

    #region//=============================================================DoubleBuffered property via reflection for datagridview, improve flickerless image.
    public static class ExtensionMethodsCalibrationDGV
    {
        public static void DoubleBufferedCalibrationCVS(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    #endregion

}
