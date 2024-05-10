using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Timers;
using System.Reflection;
using System.IO;

namespace UDT_Term_FFT
{
    public partial class ImportDataCVS : Form
    {
        private System.Windows.Forms.RichTextBox rtbImportConsole;        //taken from LoggerCSV.Designer, do not delete!
        ITools Tools = new Tools();
        USB_FTDI_Comm myUSBComm;                                        // FTDI device manager section (scan, open, close, read, write).
        GlobalBase myGlobalBase;
        //--------------------------------------Filename
        private string sFoldername;
        private string sFoldernameDefault;
        private string sFilename;
        private string RecievedData;
        IntPtr rtbOEMImport;

        #region//=============================================Reference
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
        public void MyUSBComm(USB_FTDI_Comm myUSBCommRef)
        {
            myUSBComm = myUSBCommRef;
        }

        public System.Windows.Forms.RichTextBox Ref_rtbImportConsole()
        {
            return rtbImportConsole;                                              // Passing object reference of the rtbImportConsole
        }

        #endregion

        public ImportDataCVS()
        {
            rtbOEMImport = new IntPtr();
            InitializeComponent();
            sFoldernameDefault = Tools.GetPathUserFolder();
            sFoldername = sFoldernameDefault;
            txtFolderName.Text = sFoldername;
            sFilename = System.IO.Path.Combine(sFoldername, "Imported_CVS"+DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss"));   // Default filename without SNAD, this is added automatically on demand.
            txtFilename.Text = "(Default)" + sFilename + "..csv";

        }

        #region//================================================================ImportCVS_RecievedData
        public void ImportCVS_RecievedData(string DataFrame)
        {
            int count = DataFrame.Count(f => f == '/');
            RecievedData += DataFrame;
            if (enableDataViewToolStripMenuItem.Checked == true)
            {
                myRtbImportConsoleMessage(DataFrame);
            }
            else
            {
                while (count != 0)
                {
                    myRtbImportConsoleMessage(".");     // '.' indicate number of row
                    count--;
                }
                myRtbImportConsoleMessage(",");           //',' indicate end of USB activity for observation purpose.
            }
            
            if (DataFrame.Contains("#E"))       // End Frame Detected.
            {
                ImportCVS_EndFrameDetected();
                return;
            }
            
        }
        #endregion

        #region//================================================================ImportCVS_OpenForm
        public void ImportCVS_StartFrameDetected()
        {
            myGlobalBase.IsImportRawDataActivate = true;
            myGlobalBase.ImportCVSOpertaionMode = true;
            this.Visible = true;
            this.Show();
            myRtbImportConsoleMessageLF("Detected Start Frame '#@#I'");
            RecievedData = "";
        }
        #endregion

        #region//================================================================ImportCVS_EndFrameDetected
        public void ImportCVS_EndFrameDetected()
        {
            myGlobalBase.IsImportRawDataActivate = false;
            myRtbImportConsoleMessageLF("\r\nDetected End Frame '#E'");
            //-------------------------Save string data to filename in CVS form.
            bool status = ImportCVS_OpenSaveDataCloseFile();
            //-------------------------MessagePopUp
            if (status == true)
            {
                //this.Visible = false;
                //this.Hide();
            }
        }
        #endregion

        #region //================================================================ImportCVS_OpenAppendDataCloseFile
        bool ImportCVS_OpenSaveDataCloseFile()
        {
            txtFolderName.Text = sFoldername;
            sFilename = System.IO.Path.Combine(sFoldername, "Imported_CVS" + DateTime.Now.ToString("_dd_MM_yyyy_HH_mm_ss") + ".csv");   // Default filename without SNAD, this is added automatically on demand.
            txtFilename.Text = sFilename;
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
                myRtbImportConsoleMessageLF("Imported Data Saved!");
                return true;
            }
            catch (Exception X)
            {
                myRtbImportConsoleMessageLF("#ERR: Unable to append file : " + sFilename + " Error Msg:" + X.ToString());
                MessageBox.Show("Problem in saving imported data into filename, try again",
                       "Import Data CVS",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Exclamation,
                       MessageBoxDefaultButton.Button1);
            }
            return false;

        }
        #endregion

        #region //==================================================saveDataToolStripMenuItem_Click
        private void saveDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool status = ImportCVS_OpenSaveDataCloseFile();
            if (status == true)
            {
                //this.Visible = false;
                //this.Hide();
            }
        }
        #endregion

        #region //==================================================enableDataViewToolStripMenuItem_Click
        private void enableDataViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enableDataViewToolStripMenuItem.Checked = !enableDataViewToolStripMenuItem.Checked;
            if (enableDataViewToolStripMenuItem.Checked == true)
            {
                rtbImportConsole.Clear();
                myRtbImportConsoleMessageLF(RecievedData);
            }
        }
        #endregion

        #region //================================================================btnFolder_Click
        private void btnFolder_Click(object sender, EventArgs e)
        {
            //
            // This event handler was created by double-clicking the window in the designer.
            // It runs on the program's startup routine.
            //
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
            txtFolderName.Text = sFoldername;
            txtFilename.Text = "To be Generated";
        }
        #endregion

        #region //================================================================BtnOpenFolder_Click
        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            string myPath = sFoldername;
            if (myPath == null)
                return;
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = myPath;
            prc.Start();
        }
        #endregion

        #region //================================================================cADTDataFrameLoggerToolStripMenuItem2_Click
        private void cADTDataFrameLoggerToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            sFoldername = "G:\\ADTImportDataCVS";
            dDataFrameLoggerCommonFolderNameUpdate();
        }
        private void cADTDataFrameLoggerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            sFoldername = "F:\\ADTImportDataCVS";
            dDataFrameLoggerCommonFolderNameUpdate();
        }

        private void cADTDataFrameLoggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sFoldername = "E:\\ADTImportDataCVS";
            dDataFrameLoggerCommonFolderNameUpdate();
        }

        private void cADTDataFrameLoggerToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            sFoldername = "H:\\ADTImportDataCVS";
            dDataFrameLoggerCommonFolderNameUpdate();
        }

        private void cADTDataFrameLoggerToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            sFoldername = "I:\\ADTImportDataCVS";
            dDataFrameLoggerCommonFolderNameUpdate();
        }

        private void cADTDataFrameLoggerToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            sFoldername = "J:\\ADTImportDataCVS";
            dDataFrameLoggerCommonFolderNameUpdate();
        }

        private void dDataFrameLoggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sFoldername = "D:\\ADTImportDataCVS";
            dDataFrameLoggerCommonFolderNameUpdate();
        }
        private void dDataFrameLoggerCommonFolderNameUpdate()
        {
            txtFolderName.Text = sFoldername;
            if (!Directory.Exists(sFoldername))                                 // If folder does not exisit, create new one 
            {
                DirectoryInfo di = Directory.CreateDirectory(sFoldername);  // Create folder if not exist. 
            }
            txtFolderName.Text = sFoldername;
            txtFilename.Text = "To be Generated";

        }
        #endregion

        #region //================================================================ImportDataCVS_FormClosing
        private void ImportDataCVS_FormClosing(object sender, FormClosingEventArgs e)
        {
            myGlobalBase.ImportCVSOpertaionMode = false;       // Cease Logger Terminal mode.
            myGlobalBase.IsImportRawDataActivate = false;
            e.Cancel = true;
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================instructionToolStripMenuItem_Click
        private void instructionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            TextBox tbx = new TextBox();
            frm.Controls.Add(tbx);
            tbx.Multiline = true;
            tbx.Height = frm.Height;
            tbx.Width = frm.Width;

            tbx.AppendText("Help Section for Data Importer CVS \r\n");
            tbx.AppendText("----------------------------------------------------------\r\n");
            tbx.AppendText("This feature allow to save collected data in CVS from MCU.\r\n");
            tbx.AppendText("The CVS is purely string style transmission from MCU\r\n");
            tbx.AppendText("    where ';' treat as column separator\r\n");
            tbx.AppendText("    where '\\r\\n' treat as row separator\r\n");
            tbx.AppendText("    To invoke this process, must have #@#I at the start\r\n");
            tbx.AppendText("    To end data stream must have #E\r\n");
            tbx.AppendText("NB: After the #@#I, allow small pause of >10mSec\r\n");
            tbx.AppendText("    before sending header frame and then data frame\r\n");
            tbx.AppendText("    Header frame is the description of column at top of CVS\r\n");
            tbx.AppendText("    Data frame is data in string, int, float, etc...\r\n");
            tbx.AppendText("Don't forget to enable it by checking the box in main window\r\n");
            tbx.AppendText("Window layout for filename is same as ADTLogger window\r\n");
            tbx.AppendText("\r\n");
            tbx.AppendText("Revision 1D, RPayne (18-Feb-15)\r\n");
            frm.ShowDialog();

        }
        #endregion

        #region //==================================================myRtbTermMessageLF
        //==========================================================
        // Purpose  : Append message in terminal window,. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void myRtbImportConsoleMessageLF(string Message)
        {
            Tools.rtb_StopRepaint(rtbImportConsole, rtbOEMImport);
            rtbImportConsole.SelectionFont = myGlobalBase.FontResponse;
            rtbImportConsole.SelectionColor = myGlobalBase.ColorResponse;
            rtbImportConsole.SelectionStart = rtbImportConsole.TextLength;
            rtbImportConsole.ScrollToCaret();
            rtbImportConsole.Select();
            rtbImportConsole.AppendText(Message + "\r\n");
            Tools.rtb_StartRepaint(rtbImportConsole, rtbOEMImport);
        }
        #endregion

        #region //==================================================myRtbTermMessage
        //==========================================================
        // Purpose  : Append message in terminal window,. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void myRtbImportConsoleMessage(string Message)
        {
            Tools.rtb_StopRepaint(rtbImportConsole, rtbOEMImport);
            rtbImportConsole.SelectionFont = myGlobalBase.FontResponse;
            rtbImportConsole.SelectionColor = myGlobalBase.ColorResponse;
            rtbImportConsole.SelectionStart = rtbImportConsole.TextLength;
            rtbImportConsole.ScrollToCaret();
            rtbImportConsole.Select();
            rtbImportConsole.AppendText(Message);
            Tools.rtb_StartRepaint(rtbImportConsole, rtbOEMImport);
        }
        #endregion

        #region//==================================================rtbImportConsole_MouseUp
        private void rtbImportConsole_MouseUp(object sender, MouseEventArgs e)
        { 
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {   
                ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
                MenuItem menuItem = new MenuItem("Cut");
                menuItem.Click += new EventHandler(CutAction2);
                contextMenu.MenuItems.Add(menuItem);
                menuItem = new MenuItem("Copy");
                menuItem.Click += new EventHandler(CopyAction2);
                contextMenu.MenuItems.Add(menuItem);
                menuItem = new MenuItem("Paste");
                menuItem.Click += new EventHandler(PasteAction2);
                contextMenu.MenuItems.Add(menuItem);

                rtbImportConsole.ContextMenu = contextMenu;
            }
        }
        void CutAction2(object sender, EventArgs e)
        {
            rtbImportConsole.Cut();
        }

        void CopyAction2(object sender, EventArgs e)
        {
            Clipboard.SetText(rtbImportConsole.SelectedText);
        }

        void PasteAction2(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.Rtf))
            {
                rtbImportConsole.Text += Clipboard.GetText(TextDataFormat.Text).ToString();
            }
        }
        #endregion

        #region//==================================================rtbImportConsole_MouseUp
        private void fillWithTestDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myGlobalBase.IsImportRawDataActivate = true;
            string test_data = "Date;Time;Pressure;Temperature\r\n 12/3/15;12:33;12323;25C\r\n12/3/15;12:43;45342;45C\r\n 12/3/15;12:33;12323;25C\r\n12/3/15;12:43;45342;45C\r\n#E";
            ImportCVS_RecievedData(test_data);
        }
        #endregion

    }
    #region//=============================================================DoubleBuffered property via reflection for datagridview, improve flickerless image.
    public static class ExtensionMethodsMessageImportCVS
    {
        public static void DoubleBufferedMessageImportCVS(this RichTextBox dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    #endregion
}
