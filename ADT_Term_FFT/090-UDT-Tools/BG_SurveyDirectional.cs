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

namespace UDT_Term_FFT
{
    public partial class BG_Directional : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        System.Windows.Forms.RichTextBox myRtbTerm;
        EE_LogMemSurvey myDownloadSurvey;
        BGMasterSetup myBGMasterSetup;

        //private string m_sEntryTxt;

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
        public BG_Directional()
        {
            InitializeComponent();
        }

        //#####################################################################################################
        //###################################################################################### Form Management
        //#####################################################################################################

        #region //================================================================BG_ToolSetup_Load
        private void BG_Driller_Load(object sender, EventArgs e)
        {
            BG_Driller_Setup_Init();
        }
        #endregion

        #region //================================================================EE_LogMemSurvey_Show
        public void BG_Driller_Show()
        {
            this.BringToFront();
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            myGlobalBase.BG_isDrillerOpen = true;
            this.Visible = true;
            this.Show();
        }
        #endregion

        #region //================================================================BG_ToolSetup_FormClosing
        private void BG_Driller_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            myGlobalBase.BG_isDrillerOpen = false;                 // Cease Survey CVS Terminal mode.
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================BGToolSetup_Setup_Init
        private void BG_Driller_Setup_Init()
        {


        }
        #endregion
        /*

        #region //------------------------------------------------------btnLoad_Click
        private void btnLoad_Click(object sender, EventArgs e)
        {
            //----------------------------------------------------------XML Load File Section. 
            BindingList<ScopeDataOp> blLoadData = null;
            ofdRiscyScope.Filter = "xml files (*.xml)|*.xml";
            ofdRiscyScope.InitialDirectory = sFoldername;
            ofdRiscyScope.Title = "Load XML Scope Data/Gain Setting Files";
            DialogResult dr = ofdRiscyScope.ShowDialog();
            if (dr == DialogResult.OK)
            {
                blLoadData = DeserializeFromFile<BindingList<ScopeDataOp>>(ofdRiscyScope.FileName);
                if (blLoadData == null)
                {
                    MessageBox.Show("Unable to Load Gain/Offset Setting Configuration File (XML)",
                        "Load Config File Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else
                {
                    for (int i = 0; i <= 3; i++)
                    {
                        blScopeDataOp[i].DataName = blLoadData[i].DataName;
                        blScopeDataOp[i].DataGain = blLoadData[i].DataGain;
                        blScopeDataOp[i].DataOffset = blLoadData[i].DataOffset;
                    }
                }
            }
            else
            {
                MessageBox.Show("Load Gain/Offset Setting: Filename Error",
                    "Load Gain/Offset Setting File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            //----------------------------------------------------------Process captures data.
            dgvScopeDataOp.Invalidate();
            ScopeDataOp_UpdateGainOffset(0);
            ScopeDataOp_UpdateGainOffset(1);
            ScopeDataOp_UpdateGainOffset(2);
            ScopeDataOp_UpdateGainOffset(3);

        }
        #endregion

        #region //------------------------------------------------------btnDataSave_Click
        private void btnDataSave_Click(object sender, EventArgs e)
        {
            sfdRiscyScope.Filter = "xml files (*.xml)|*.xml";
            sfdRiscyScope.InitialDirectory = sFoldername;
            sfdRiscyScope.FileName = sFilename1Title;
            sfdRiscyScope.Title = "Export to CVS File for Scope Gain/Offset Setting";

            DialogResult dr = sfdRiscyScope.ShowDialog();
            if (dr == DialogResult.OK)
            {
                SerializeToFile<BindingList<ScopeDataOp>>(blScopeDataOp, sfdRiscyScope.FileName);
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

        #region //------------------------------------------------------SerializeToFile
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

        #region //------------------------------------------------------DeserializeFromFile
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
                        Console.WriteLine("###ERR: Failed to deserialize object!!");
                    }
                }
            }
            return result;
            //return default(T); or this method, not sure which one best....! (see expert exchange)
        }
        #endregion
        */


    }

}
