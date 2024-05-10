using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static UDT_Term_FFT.MainProg;

namespace UDT_Term_FFT
{
    public partial class ResCalc_DGV_Result : Form
    {
        //---------------------------------------------------------Private

        //---------------------------------------------------------Public 

        //---------------------------------------------------------Reference
        BindingList<ResCalc_ResultTable> myResCalResultTable;
        GlobalBase myGlobalBase;
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }
        public void MyResCalcResultTable(BindingList<ResCalc_ResultTable> myResCalcSetupRef)
        {
            myResCalResultTable = myResCalcSetupRef;
        }
        public ResCalc_DGV_Result()
        {
            InitializeComponent();
        }
        #region --------------------------------------------------------ResCalc_DGV_Result_Load()
        private void ResCalc_DGV_Result_Load(object sender, EventArgs e)
        {
            sfdSaveResDataDialog.Filter = "cvs files (*.cvs)|*.cvs";
            sfdSaveResDataDialog.InitialDirectory = myGlobalBase.zCommon_DefaultFolder;
            sfdSaveResDataDialog.Title = "Export to CVS File for Setup";

            bsResCalcTable.DataSource = myResCalResultTable;
            dgvResCalc.DataSource = bsResCalcTable;
            dgvResCalc.Columns[0].HeaderText = "R1(TOP)";
            dgvResCalc.Columns[1].HeaderText = "R2(BOT)";
            dgvResCalc.Columns[2].HeaderText = "Error(mV)";
            dgvResCalc.Columns[3].HeaderText = "VOUT(mV)";
            dgvResCalc.Columns[0].DefaultCellStyle.Format = "#,0";
            dgvResCalc.Columns[1].DefaultCellStyle.Format = "#,0"; 
            dgvResCalc.Columns[2].DefaultCellStyle.Format = "#,0.00";
            dgvResCalc.Columns[3].DefaultCellStyle.Format = "#,0.00";
            //dgvResCalc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            SetGridHeightWidth(dgvResCalc, 471, 448);
            
            this.Invalidate();
        }
        #endregion

        #region --------------------------------------------------------ResCalc_DGV_Result_FormClosing()
        private void ResCalc_DGV_Result_FormClosing(object sender, FormClosingEventArgs e)
        {
            dgvResCalc.DataSource = null;
            myResCalResultTable = null;
        }
        #endregion

        #region --------------------------------------------------------SetGridHeightWidth()
        public DataGridView SetGridHeightWidth(DataGridView grd, int maxHeight, int maxWidth)
        {
            var height = 40;
            foreach (DataGridViewRow row in grd.Rows)
            {
                if (row.Visible)
                    height += row.Height;
            }

            if (height > maxHeight)
                height = maxHeight;

            grd.Height = height;

            var width = 60;
            foreach (DataGridViewColumn col in grd.Columns)
            {
                if (col.Visible)
                    width += col.Width;
            }

            if (width > maxWidth)
                width = maxWidth;

            grd.Width = width;

            return grd;
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.Refresh();
        }

        #region --------------------------------------------------------dgvResCalc_DataError()
        private void dgvResCalc_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            MessageBox.Show("Error happened " + anError.Context.ToString());

            if (anError.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Commit error");
            }
            if (anError.Context == DataGridViewDataErrorContexts.Parsing)
            {
                MessageBox.Show("Incorrect Parsing Data");
            }
            if (anError.Context == DataGridViewDataErrorContexts.LeaveControl)
            {
                MessageBox.Show("leave control error");
            }

            if ((anError.Exception) is ConstraintException)
            {
                DataGridView view = (DataGridView)sender;
                view.Rows[anError.RowIndex].ErrorText = "an error";
                view.Rows[anError.RowIndex].Cells[anError.ColumnIndex].ErrorText = "an error";

                anError.ThrowException = false;
            }
        }
        #endregion

        #region --------------------------------------------------------btnSaveCVS_Click()
        private void btnSaveCVS_Click(object sender, EventArgs e)
        {
            DialogResult dr = sfdSaveResDataDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                SaveDataGridViewToCSV(sfdSaveResDataDialog.FileName);
                myGlobalBase.zCommon_DefaultFolder = System.IO.Path.GetDirectoryName(sfdSaveResDataDialog.FileName);
            }
            else
            {
                MessageBox.Show("Save Configuration: Filename Error",
                    "Save Config File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        #endregion

        #region --------------------------------------------------------SaveDataGridViewToCSV()
        void SaveDataGridViewToCSV(string filename)
        {
            dgvResCalc.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;     // Choose whether to write header. Use EnableWithoutHeaderText instead to omit header.
            dgvResCalc.SelectAll();                                                                         // Select all the cells
            DataObject dataObject = dgvResCalc.GetClipboardContent();                                       // Copy selected cells to DataObject
            File.WriteAllText(filename, dataObject.GetText(TextDataFormat.CommaSeparatedValue));            // Get the text of the DataObject, and serialize it to a file
            dgvResCalc.ClearSelection();
            dgvResCalc.CurrentCell = null;
            
        }
        #endregion

        #region --------------------------------------------------------btnSaveClip_Click()
        private void btnSaveClip_Click(object sender, EventArgs e)
        {
            dgvResCalc.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;     // Choose whether to write header. Use EnableWithoutHeaderText instead to omit header.
            dgvResCalc.SelectAll();                                                                         // Select all the cells
            Clipboard.SetDataObject(this.dgvResCalc.GetClipboardContent());
            dgvResCalc.ClearSelection();
            dgvResCalc.CurrentCell = null;
        }
        #endregion

        #region --------------------------------------------------------btnOpenFolder_Click()
        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string myPath = myGlobalBase.zCommon_DefaultFolder;
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = myPath;
                prc.Start();
                /*
                string folderPath = sfdSaveResDataDialog.FileName;
                FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    folderPath = folderBrowserDialog1.SelectedPath;
                }
                */
            }
            catch
            {

            }
        }
        #endregion
    }


}
