using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using System.Xml.Linq;

namespace UDT_Term_FFT
{
    public partial class AppConfigWindow : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        MainProg myMainProg;
        // ============================================================Reference Object
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }

        // ============================================================Private Variable

        //=============================================================Public Variable
        public bool isXML_File_Error { get; set; }       // No error during XML decoding. Error if XML fails or file is missing.
        public Configuration ConfigData;                 // This is class only
        private string filePath;                         // File path where XML is stored. 
        //=============================================================Constructor
        public AppConfigWindow()
        {
            InitializeComponent();
            isXML_File_Error = false;
            filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();      // Setup FilePath to Resource Folder Box.
            filePath += "Resources//XML_App_Config_78.xml";
            //this.Show();
        }

        #region //-------------------------------------------------------------------SetupTabList
        public void SetupTabList(string[] sIndex)
        {
            cbTabPageList.Items.AddRange(sIndex);
        }
        #endregion

        #region //-------------------------------------------------------------------AppConfigWindow_FormClosing
        private void AppConfigWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            myGlobalBase.SelectTools = (UInt32)cbSelectTool.SelectedIndex;
            //myGlobalBase.Select_Baudrate = (UInt32)cbBaudRateSelect.SelectedIndex;
            myGlobalBase.CompanyName = (UInt32)myGlobalBase.configb[0].CompanyID;
        }
        #endregion

        #region //-------------------------------------------------------------------AppConfigWindow_Load
        private void AppConfigWindow_Load(object sender, EventArgs e)
        {
            int i;
            if (this.cbBaudRateSelect.Items.Count==0)
            {
                for (i=0; i< myGlobalBase.iBaudRateList.Length; i++)
                {
                    this.cbBaudRateSelect.Items.Add(myGlobalBase.iBaudRateList[i]);
                }
            }
            if (this.cbCompanyThemes.Items.Count == 0)   // is empty?
                this.cbCompanyThemes.Items.AddRange(myGlobalBase.configb[0].ListCompanyName);
            for (i = 0; i < myGlobalBase.configb[0].ListCompanyID.Length; i++)
            {
                if (myGlobalBase.configb[0].CompanyID == myGlobalBase.configb[0].ListCompanyID[i])
                    break;
            }
            this.cbCompanyThemes.SelectedIndex = i;
            this.cbBaudRateSelect.SelectedIndex = (int)myGlobalBase.configb[0].Select_Baudrate;
            this.cbAutoCopyClip.Checked = myGlobalBase.configb[0].bOptionAutoCopyRXMessageToClipBoard;
            this.cbSkipFTDIScan.Checked = myGlobalBase.configb[0].bOptionSkipFTDIScan;
            this.cbAutoDetectEnable.Checked = myGlobalBase.configb[0].bOptionAutoDetectEnable;
            this.cbSelectTool.SelectedIndex = (int)myGlobalBase.configb[0].SelectTool;
            //-----------------------------------------------------------------------------78D
            this.tbUserBaudRate.Text = myGlobalBase.configb[0].User_Baudrate.ToString();
            this.tbFolderName.Text = myGlobalBase.configb[0].DefaultFolder;
            myMainProg.myFolder_UpdateSync(myGlobalBase.configb[0].DefaultFolder);
            myMainProg.myBaudRate_UpdateSync(myGlobalBase.configb[0].User_Baudrate);
            myMainProg.myTabPage_UpdateSync(myGlobalBase.configb[0].Select_TabPage);
            this.cbTabPageList.SelectedIndex = myGlobalBase.MainFeatureSelectedIndex;
            //-----------------------------------------------------------------------------
            this.Refresh();
        }
        #endregion

        #region //-------------------------------------------------------------------btnCancel_Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### XML_ConfigurationProject_1A
        //#####################################################################################################

        #region //========================================================== XML_ConfigurationProject_1A
        private void XML_ConfigurationProject_1A()
        {
            if (File.Exists(filePath))
            {
                // Deserialize the existing list or create an empty one if none exists yet.
                myGlobalBase.configb = DeserializeFromFile<BindingList<Configuration>>(filePath);
                if (myGlobalBase.configb == null)
                    isXML_File_Error = true;
            }
            else
            {
                // Serialize the existing list or create an empty one if none exists yet.  

                if (myGlobalBase.configb == null)
                {
                    myGlobalBase.configb = new BindingList<Configuration>();
                    myGlobalBase.configb.Add(new Configuration());     // Default Setting if filename not exist.
                    myGlobalBase.configb[0].CompanyID = myGlobalBase.configb[0].ListCompanyID[0];
                    myGlobalBase.configb[0].CompanyString = myGlobalBase.configb[0].ListCompanyName[0];
                    myGlobalBase.configb[0].Test1 = 10101;
                    myGlobalBase.configb[0].Select_Baudrate = 5;
                    myGlobalBase.configb[0].bOptionAutoCopyRXMessageToClipBoard = true;
                    myGlobalBase.configb[0].bOptionSkipFTDIScan = true;
                    myGlobalBase.configb[0].bOptionAutoDetectEnable = true;
                    myGlobalBase.configb[0].SelectTool = myGlobalBase.configb[0].SelectTool;
                    //----------------------------------------------------------------------------78D
                    myGlobalBase.configb[0].DefaultFolder = myGlobalBase.zCommon_DefaultFolder;
                    myGlobalBase.configb[0].User_Baudrate = 0;
                    myMainProg.myFolder_UpdateSync(myGlobalBase.configb[0].DefaultFolder);
                    myMainProg.myBaudRate_UpdateSync(myGlobalBase.configb[0].User_Baudrate);
                    myMainProg.myTabPage_UpdateSync(myGlobalBase.configb[0].Select_TabPage);
                }
                SerializeToFile<BindingList<Configuration>>(myGlobalBase.configb, filePath);
            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### UART 
        //#####################################################################################################

        #region //========================================================== Get_Selected_BaudRate
        public UInt32 Get_Selected_BaudRate()
        {
            return (UInt32)myGlobalBase.configb[0].Select_Baudrate;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### UTIL/Support
        //#####################################################################################################

        #region //========================================================== LoadConfigNow
        public void LoadConfigNow()
        {
            myGlobalBase.CompanyName = 50;       // Stay in TVB. 
            XML_ConfigurationProject_1A();
            //myGlobalBase.CompanyName = (UInt32)myGlobalBase.configb[0].CompanyID;
            //myGlobalBase.Select_Baudrate = myGlobalBase.configb[0].Select_Baudrate;
            myGlobalBase.SelectTools = myGlobalBase.configb[0].SelectTool;
            //-----------------------------------------------------------------------------78D
            this.tbUserBaudRate.Text = myGlobalBase.configb[0].User_Baudrate.ToString();
            this.tbFolderName.Text = myGlobalBase.configb[0].DefaultFolder;
            myMainProg.myFolder_UpdateSync(myGlobalBase.configb[0].DefaultFolder);
            myMainProg.myBaudRate_UpdateSync(myGlobalBase.configb[0].User_Baudrate);
            myMainProg.myTabPage_UpdateSync(myGlobalBase.configb[0].Select_TabPage);
            //-----------------------------------------------------------------------------

            if (myGlobalBase.configb[0].bOptionSkipFTDIScan == true)
                myGlobalBase.USB_SelectDeviceMode = (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM;  //VCOM Only. 
            else
                myGlobalBase.USB_SelectDeviceMode = 0;      //Both FTDI and VCOM. 
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Save/Load
        //#####################################################################################################

        #region //-------------------------------------------------------------------SerializeToFile
        // Source from: http://stackoverflow.com/questions/28499781/deserializing-xml-into-c-sharp-so-that-the-values-can-be-edited-and-re-serialize
        // An 'System.IO.FileNotFoundException' exception is thrown but handled by the XmlSerializer, so if you just ignore it everything should continue on fine.
        // Method-1 This is better due to \r\n for each XML statement but 
        private void SerializeToFile<T>(T item, string xmlFileName)
        {
            if (typeof(T).IsSerializable)       // Check before applying, in case class has missing [Serializable]
            {
                using (FileStream stream = new FileStream(xmlFileName, FileMode.Create, FileAccess.Write))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stream, item);
                }
            }
        }
        #endregion

        #region //-------------------------------------------------------------------DeserializeFromFile<>
        // This is better solution since there is no IO filename exception error. 
        private T DeserializeFromFile<T>(string filenme) where T : class
        {
            T result = null;
            if (typeof(T).IsSerializable)            // Check before applying, in case class has missing [Serializable]
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (XmlTextReader reader = new XmlTextReader(filenme))
                {
                    try
                    {
                        result = (T)serializer.Deserialize(reader);
                        //Console.WriteLine("-INFO: Deserialization successful! Got string: \n{0}", result);
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("###ERR: Failed to Deserialize object!!");
                    }
                }
            }
            return result;
            //return default(T); or this method, not sure which one best....! (see expert exchange)
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Save/Load
        //#####################################################################################################

        #region //-------------------------------------------------------------------btnSave_Click
        private void btnSave_Click(object sender, EventArgs e)
        {
            myGlobalBase.configb[0].Select_Baudrate = (UInt32) cbBaudRateSelect.SelectedIndex;
            myGlobalBase.configb[0].SelectTool = (UInt32)cbSelectTool.SelectedIndex;
            myGlobalBase.configb[0].Select_TabPage = myGlobalBase.MainFeatureSelectedIndex;
            //----------------------------------------------------------------------------------------------78D
            myGlobalBase.configb[0].User_Baudrate = Tools.ConversionStringtoUInt32(tbUserBaudRate.Text);
            myGlobalBase.configb[0].DefaultFolder = tbFolderName.Text;
            myMainProg.myFolder_UpdateSync(myGlobalBase.configb[0].DefaultFolder);
            myMainProg.myBaudRate_UpdateSync(myGlobalBase.configb[0].User_Baudrate);
            myMainProg.myTabPage_UpdateSync(myGlobalBase.configb[0].Select_TabPage);
            //----------------------------------------------------------------------------------------------
            myGlobalBase.SelectTools = (UInt32)cbSelectTool.SelectedIndex;
            SerializeToFile<BindingList<Configuration>>(myGlobalBase.configb, filePath);
            //myGlobalBase.Select_Baudrate = (UInt32)cbBaudRateSelect.SelectedIndex;
            myGlobalBase.CompanyName = (UInt32)myGlobalBase.configb[0].CompanyID;
            this.Close();
        }
        #endregion

        #region //-------------------------------------------------------------------btnReLoad_Click
        private void btnReLoad_Click(object sender, EventArgs e)
        {
            if (File.Exists(filePath))
            {
                // Deserialize the existing list or create an empty one if none exists yet.
                myGlobalBase.configb = DeserializeFromFile<BindingList<Configuration>>(filePath);
                if (myGlobalBase.configb == null)
                    isXML_File_Error = true;
                else
                {
                    myMainProg.myFolder_UpdateSync(myGlobalBase.configb[0].DefaultFolder);
                    myMainProg.myBaudRate_UpdateSync(myGlobalBase.configb[0].User_Baudrate);
                    myMainProg.myTabPage_UpdateSync(myGlobalBase.configb[0].Select_TabPage);
                }
            }
            else
            {
                MessageBox.Show("Error: Unable to load config file!", "File Error",
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Error);
            }
            this.Invalidate();
            this.Refresh();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Option Combo
        //#####################################################################################################

        #region //-------------------------------------------------------------------cbCompanyThemes_SelectedIndexChanged
        private void cbCompanyThemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            myGlobalBase.configb[0].CompanyID = myGlobalBase.configb[0].ListCompanyID[cbCompanyThemes.SelectedIndex];
            myGlobalBase.configb[0].CompanyString = myGlobalBase.configb[0].ListCompanyName[cbCompanyThemes.SelectedIndex];
        }
        #endregion

        #region //-------------------------------------------------------------------cbBaudRateSelect_SelectedIndexChanged
        private void cbBaudRateSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ConfigData != null)
            {
                ConfigData.Select_Baudrate = (UInt32)cbBaudRateSelect.SelectedIndex;
                myGlobalBase.Select_Baudrate = (UInt32)cbBaudRateSelect.SelectedIndex;
                Console.Beep(1000, 100);
            }
        }
        #endregion

        #region //-------------------------------------------------------------------cbSelectTool_SelectedIndexChanged
        private void cbSelectTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ConfigData != null)
            {
                ConfigData.SelectTool = (UInt32)cbSelectTool.SelectedIndex;
                myGlobalBase.SelectTools = (UInt32)cbSelectTool.SelectedIndex;
                Console.Beep(1000, 100);
            }
        }
        #endregion

        #region //-------------------------------------------------------------------cbTabPageList_SelectedIndexChanged
        private void cbTabPageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            myGlobalBase.MainFeatureSelectedIndex = cbTabPageList.SelectedIndex;
            myGlobalBase.configb[0].Select_TabPage = myGlobalBase.MainFeatureSelectedIndex;
            Console.Beep(1000, 100);
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Option CheckBox
        //#####################################################################################################

        #region //-------------------------------------------------------------------cbAutoCopyClip_CheckedChanged
        private void cbAutoCopyClip_CheckedChanged(object sender, EventArgs e)
        {
            myGlobalBase.configb[0].bOptionAutoCopyRXMessageToClipBoard = cbAutoCopyClip.Checked;

        }
        #endregion

        #region //-------------------------------------------------------------------cbSkipFTDIScan_CheckedChanged
        private void cbSkipFTDIScan_CheckedChanged(object sender, EventArgs e)
        {
             myGlobalBase.configb[0].bOptionSkipFTDIScan = this.cbSkipFTDIScan.Checked;
        }
        #endregion

        #region //-------------------------------------------------------------------cbAutoDetectEnable_CheckedChanged
        private void cbAutoDetectEnable_CheckedChanged(object sender, EventArgs e)
        {
            myGlobalBase.configb[0].bOptionAutoDetectEnable = this.cbAutoDetectEnable.Checked;
        }

        #endregion

        #region //-------------------------------------------------------------------btnSetFolder_Click
        private void btnSetFolder_Click(object sender, EventArgs e)
        {
            string sSessionFoldername = myGlobalBase.sSessionFoldername;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                //
                // The user selected a folder and pressed the OK button.
                // We print the number of files found.
                //
                sSessionFoldername = folderBrowserDialog1.SelectedPath;
                if (!Directory.Exists(sSessionFoldername))
                {
                    _ = Directory.CreateDirectory(sSessionFoldername);  // Create folder if not exist. 
                }
                myGlobalBase.configb[0].DefaultFolder = sSessionFoldername;
                tbFolderName.Text = sSessionFoldername;
                myMainProg.myFolder_UpdateSync(myGlobalBase.configb[0].DefaultFolder);
            }
        }
        #endregion

        #region //-------------------------------------------------------------------btnOpenFolder_Click
        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string myPath = myGlobalBase.configb[0].DefaultFolder;
                if (Directory.Exists(myPath))                             // Default folder name for given drive. 
                {
                    System.Diagnostics.Process prc = new System.Diagnostics.Process();
                    prc.StartInfo.FileName = myPath;
                    prc.Start();
                }
                else
                {
                    myMainProg.myRtbTermMessageLF("###ERR: Unable to Open Folder");
                }
            }
            catch
            {
                myMainProg.myRtbTermMessageLF("###ERR: Unable to Open Folder");
            }
        }
        #endregion

    }
}

