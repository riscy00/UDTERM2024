using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace UDT_Term_FFT
{
    public partial class FCL3 : UserControl
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        USB_Message_Manager myUSB_Message_Manager;
        TVMiniPort_FCL30Setup FCL30Setup;
        List<TVMiniPort_AD7794Channel> ADC24Ch;
        TVMiniPort_AD7794TempCh ADC24TempCh;
        TVMiniPort_AD7794Setup ADC24Setup;
        List<TVMiniPort_ADCPost> ADCPostArray;
        MainProg myMainProg;
        DMFProtocol myDMFProtocol;
        Mag_DataMath myFLC30MagMath;
        TVMiniPort_TabEnable TabWindowEnable;

        #region//================================================================Reference
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }
        public void MyTabWindowEnable(TVMiniPort_TabEnable TabWindowEnableRef)
        {
            TabWindowEnable = TabWindowEnableRef;
        }
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        public void MyUSB_Message_Manager(USB_Message_Manager myUSB_Message_ManagerRef)
        {
            myUSB_Message_Manager = myUSB_Message_ManagerRef;
        }
        public void MyFCL30Setup(TVMiniPort_FCL30Setup FCL30SetupRef)
        {
            FCL30Setup = FCL30SetupRef;
        }
        public void MyADCPost(List<TVMiniPort_ADCPost> ADCPostArrayRef)
        {
            ADCPostArray = ADCPostArrayRef;
        }
        public void MyADC24Setup(TVMiniPort_AD7794Setup ADC24SetupRef)
        {
            ADC24Setup = ADC24SetupRef;
        }
        public void MyADC24TempCh(TVMiniPort_AD7794TempCh ADC24TempChRef)
        {
            ADC24TempCh = ADC24TempChRef;
        }
        public void MyADC24Ch(List<TVMiniPort_AD7794Channel> ADC24ChRef)
        {
            ADC24Ch = ADC24ChRef;
        }
        public void MyDMFProtocol(DMFProtocol myDMFProtocolRef)
        {
            myDMFProtocol = myDMFProtocolRef;
        }
        #endregion

        //------------------------------------------------------------------------Local Variable
        public bool isUserCellChange { get; set; }
        private int MiniPort_FCL30_ImageSelect { get; set; }
        private bool isMathFeatureSupported { get; set; }   // True Enable Math Display, False = turn off Math display. 

        public FCL3()
        {
            InitializeComponent();
        }
        #region//================================================================FCL30_Load
        private void FCL30_Load(object sender, EventArgs e)
        {
            MiniPort_FCL30_ImageSelect = 0;
            isUserCellChange = true;
            pB_FCL3_Image.Image = UDT_Term.Properties.Resources.FCL30_Diagram1C;
            SupportDataGridSetup();
            myFLC30MagMath = new Mag_DataMath();
            cbFCL30ExportSetup.Checked = true;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Tab Enable Manager
        //#####################################################################################################
        #region//=============================================================================================TabWindowEnableImplement
        public bool TabWindowEnableImplement()
        {
            if ((FCL30Setup == null) | (myFLC30MagMath == null))
                return (false);
            if (TabWindowEnable.bFLC370 == true)
            {
                chFCL30Enable.Checked = true;
                FCL307_Refresh_Window_Control();
                cbFCL30ExportSetup.Checked = true;
                return true;
            }
            else
            {
                chFCL30Enable.Checked = false;
                FCL307_Refresh_Window_Control();
                cbFCL30ExportSetup.Checked = true;
            }
            return false;
        }
        #endregion
        //#####################################################################################################
        //###################################################################################### Tab Enable Manager
        //#####################################################################################################

        #region//=============================================================================================FCL307_Refresh_Window_Control
        public void FCL307_Refresh_Window_Control()
        {
            isUserCellChange = false;
            if (TabWindowEnable.bFLC370 == false)
            {
                FCL30Setup.bIsEnable = false;
                chFCL30Enable.Checked = false;
                lbFCL30Disable.Visible = true;
                gbAxisPolicy.Visible = false;
                gbFormat.Visible = false;
                gbFCL30Data.Visible = false;
                gbADCPost.Visible = false;
                gpBasicMath.Visible = false;
                btnFCL30CaptureNow.Visible = false;
                cbFCL30ExportSetup.Visible = false;
                btnFCL30ImportSetting.Visible = false;
                btnFCL30ExportSetting.Visible = false;
            }
            else
            {
                //if (isEnableFirstTime==true)
                //{
                //    isEnableFirstTime = false;
                //    FCL30_InitDefaultSetupFirstTime();
                //}
                ADCPostGrid_RefreshUpdate();
                chFCL30Enable.Checked = true;
                FCL30Setup.bIsEnable = true;
                btnFCL30CaptureNow.Visible = true;
                cbFCL30ExportSetup.Visible = true;
                btnFCL30ImportSetting.Visible = true;
                btnFCL30ExportSetting.Visible = true;
                //------------------------------------------Enable or Disable Mode.
                if (lbFCL30Disable.Visible == true)
                {
                    lbFCL30Disable.Visible = false;
                    gbAxisPolicy.Visible = true;
                    gbFormat.Visible = true;
                    gbFCL30Data.Visible = true;
                    gbADCPost.Visible = true;
                    if (isMathFeatureSupported == true)
                        gpBasicMath.Visible = true;
                }
                //------------------------------------------Format Section
                switch (FCL30Setup.iFormatReadout)
                {
                    case (0):       //Raw Data
                        rbFCL30nT.Checked = false;
                        rbFCL30pT.Checked = false;
                        rbFCL30mG.Checked = false;
                        rbFCL30uG.Checked = false;
                        rbFCL30Raw.Checked = true;
                        gpBasicMath.Visible = false;        // Does not support math on raw data. 

                        break;
                    case (1):       //nT
                        rbFCL30pT.Checked = false;
                        rbFCL30mG.Checked = false;
                        rbFCL30uG.Checked = false;
                        rbFCL30Raw.Checked = false;
                        rbFCL30nT.Checked = true;
                        gpBasicMath.Visible = true;
                        break;
                    case (2):       //pT
                        rbFCL30nT.Checked = false;
                        rbFCL30mG.Checked = false;
                        rbFCL30uG.Checked = false;
                        rbFCL30Raw.Checked = false;
                        rbFCL30pT.Checked = true;
                        gpBasicMath.Visible = true;
                        break;
                    case (3):       //mG
                        rbFCL30nT.Checked = false;
                        rbFCL30pT.Checked = false;
                        rbFCL30uG.Checked = false;
                        rbFCL30Raw.Checked = false;
                        rbFCL30mG.Checked = true;
                        gpBasicMath.Visible = true;
                        break;
                    case (4):       //uG
                        rbFCL30nT.Checked = false;
                        rbFCL30pT.Checked = false;
                        rbFCL30mG.Checked = false;
                        rbFCL30Raw.Checked = false;
                        rbFCL30uG.Checked = true;
                        gpBasicMath.Visible = true;
                        break;
                    default:
                        break;
                }
                //----------------------------------------Axis Section

                switch (FCL30Setup.iAxisPolicy)
                {
                    case (0):   // OGC
                        rbFCL30XYZ.Checked = false;
                        rbFCL30MRC.Checked = false;
                        rbFCL30OGC.Checked = true;
                        break;
                    case (1):   // XYZ
                        rbFCL30OGC.Checked = false;
                        rbFCL30MRC.Checked = false;
                        rbFCL30XYZ.Checked = true;
                        break;
                    case (2):   //MRC
                        rbFCL30OGC.Checked = false;
                        rbFCL30XYZ.Checked = false;
                        rbFCL30MRC.Checked = true;
                        break;
                    default:
                        break;
                }
                SupportDataGrid_Update();
            }
            isUserCellChange = true;

        }
        #endregion
        //#####################################################################################################
        //###################################################################################### ADC Post (Offset/Gain)
        //#####################################################################################################

        #region//=============================================================================================Setup Column Format
        private System.Windows.Forms.DataGridView ADCPostGrid;
        DataGridViewTextBoxColumn[] ADCFCL30CmdColumn;
        static SupportDataGrid[] myADCPostLayout =
            new SupportDataGrid[]{
                new SupportDataGrid("Ch",30),
                new SupportDataGrid("Type",70),
                new SupportDataGrid("Offset",101),
                new SupportDataGrid("Gain",101),
                new SupportDataGrid("PostFix",52)
            };
        #endregion

        #region //=============================================================================================SupportDataGridSetup
        public void ADCPostGridSetup()
        {
            ADCPostGrid = new System.Windows.Forms.DataGridView();
            ADCPostGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            ADCPostGrid.RowTemplate.Height = 18;
            ADCPostGrid.RowTemplate.MinimumHeight = 18;
            //--------------------------------------------------------------------
            ADCPostGrid.Location = new System.Drawing.Point(0, 0);
            ADCPostGrid.Size = new System.Drawing.Size(354, 79);
            ADCPostGrid.MaximumSize = new System.Drawing.Size(354, 79);
            ADCPostGrid.MinimumSize= new System.Drawing.Size(354, 79);
            ADCPostGrid.TabIndex = 0;
            ADCPostGrid.AllowUserToAddRows = false;
            ADCPostGrid.AllowUserToDeleteRows = false;
            ADCPostGrid.AllowUserToResizeColumns = false;
            ADCPostGrid.AllowUserToResizeRows = false;
            ADCPostGrid.BackgroundColor = System.Drawing.Color.White;
            ADCPostGrid.MultiSelect = false;
            ADCPostGrid.Name = "PostOffsetGain";
            ADCPostGrid.RowHeadersVisible = false;
            ADCPostGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;

            //--------------------------------------------------------------------
            ADCFCL30CmdColumn = new DataGridViewTextBoxColumn[5];
            for (int i = 0; i < 5; i++)
            {
                ADCFCL30CmdColumn[i] = new DataGridViewTextBoxColumn();
                ADCFCL30CmdColumn[i].Name = "txt" + myADCPostLayout[i].Label;
                ADCFCL30CmdColumn[i].HeaderText = myADCPostLayout[i].Label;
                ADCFCL30CmdColumn[i].Width = myADCPostLayout[i].Width;
                ADCFCL30CmdColumn[i].Resizable = DataGridViewTriState.False;
                ADCFCL30CmdColumn[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                if ((i == 0) | (i == 1))
                    ADCFCL30CmdColumn[i].ReadOnly = true;
                this.ADCPostGrid.Columns.Add(ADCFCL30CmdColumn[i]);
            }
            //=========================================Add Row with color for ADC12/ADC24
            for (int i = 0; i < 3; i++)
            {
                this.ADCPostGrid.Rows.Add(new string[] { (i + 1).ToString("D1") });
            }
            ADCPostGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ADCPostGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            ADCPostGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            ADCPostGrid.AllowUserToResizeRows = false;
            this.gbADCPost.Controls.Add(this.ADCPostGrid);
            //this.ADCPostGrid.Location = new System.Drawing.Point(6, 19);
            //========================================Add Event
            this.ADCPostGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ADCPostGrid_CellValueChanged); // to validate the data in grid to alert user of error.
        }
        #endregion

        #region //=============================================================================================ADCPostGrid_CellValueChanged
        private void ADCPostGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isUserCellChange == false)
                return;
            cbFCL30ExportSetup.Checked = true;          // Need export to update.
            DataGridViewRow row = ADCPostGrid.Rows[e.RowIndex];
            try
            {
                if (e.ColumnIndex == 2) // Offset
                {
                    if (Tools.IsString_Numberic_Int32(row.Cells[2].Value.ToString()) == false)
                    {
                        row.Cells[2].Style.BackColor = Color.LightPink;
                    }
                    else
                    {
                        row.Cells[2].Style.BackColor = Color.White;
                        ADCPostGrid_ReadUpdate();
                    }
                }
                if (e.ColumnIndex == 3) // Gain
                {
                    if ((row.Cells[3].Value.ToString() == "0") | (row.Cells[3].Value.ToString() == "0=Disabled"))
                    {
                        row.Cells[3].Style.BackColor = Color.White;
                        ADCPostGrid_ReadUpdate();
                    }
                    else
                    {
                        if (Tools.IsString_Numberic_Double(row.Cells[3].Value.ToString()) == false)
                        {
                            row.Cells[3].Style.BackColor = Color.LightPink;
                        }
                        else
                        {
                            double result = Tools.ConversionStringtoDouble(row.Cells[3].Value.ToString());
                            ADCPostGrid_ReadUpdate();
                        }
                    }
                }
                if (e.ColumnIndex == 4) // UnitText
                {
                    if (row.Cells[4].Value.ToString().Length >= 8)
                    {
                        row.Cells[4].Style.BackColor = Color.LightPink;
                    }
                    else
                    {
                        row.Cells[4].Style.BackColor = Color.White;
                        ADCPostGrid_ReadUpdate();
                    }
                }
            }
            catch { }

        }
        #endregion

        #region //=============================================================================================ADCPostGrid_RefreshUpdate
        public void ADCPostGrid_RefreshUpdate()
        {
            isUserCellChange = false;
            for (int i = 0; i < 3; i++)
            {
                ADCPostGrid.Rows[i].Cells[0].Value = ADCPostArray[i+3].iCh + 1;      // For ADC24
                ADCPostGrid.Rows[i].Cells[1].Value = ADCPostArray[i+3].iType;
                ADCPostGrid.Rows[i].Cells[2].Value = ADCPostArray[i+3].iOffset;
                if (ADCPostArray[i+3].uGain == 0)
                {
                    ADCPostGrid.Rows[i].Cells[3].Value = "0=Disabled";
                }
                else
                {
                    ADCPostGrid.Rows[i].Cells[3].Value = ((double)ADCPostArray[i+3].uGain) / 1000000.0;    //NB: 1,000,000 = 1V/V, 100,000 = 0.1V/V
                }
                ADCPostGrid.Rows[i].Cells[4].Value = ADCPostArray[i+3].sUnitText;
            }
            ADCPostGrid.Refresh();
            isUserCellChange = true;
        }
        #endregion

        #region //=============================================================================================ADCPostGrid_ReadUpdate
        public void ADCPostGrid_ReadUpdate()
        {
            isUserCellChange = false;
            ADCPostGrid.Refresh();
            for (int i = 0; i < 3; i++)
            {
                //-----------Offset
                if (Tools.IsString_Numberic_Int32(ADCPostGrid.Rows[i].Cells[2].Value.ToString()) == true)
                    ADCPostArray[i+3].iOffset = Tools.ConversionStringtoInt32(ADCPostGrid.Rows[i].Cells[2].Value.ToString());
                //-----------Gain.
                if ((ADCPostGrid.Rows[i].Cells[3].Value.ToString() == "0") | (ADCPostGrid.Rows[i].Cells[3].Value.ToString() == "0=Disabled"))
                {
                    ADCPostArray[i+3].uGain = 0;
                    ADCPostGrid.Rows[i].Cells[3].Value = "0=Disabled";
                }
                else        //NB: 1,000,000 = 1V/V, 100,000 = 0.1V/V, maximum is 4293
                {
                    if (Tools.IsString_Numberic_Double(ADCPostGrid.Rows[i].Cells[3].Value.ToString()) == true)
                    {
                        double value = Tools.ConversionStringtoDouble(ADCPostGrid.Rows[i].Cells[3].Value.ToString());
                        if (value > 4294.967295)       // Out of range.
                            value = 4294.967295;
                        value = value * 1000000.0;
                        ADCPostArray[i+3].uGain = Convert.ToUInt32(value); // This is better than cast (Int32)value due to rounding methodology.
                        ADCPostGrid.Rows[i].Cells[3].Value = ((double)ADCPostArray[i+3].uGain) / 1000000.0;  // Update to show rounding impacts
                    }
                }
                ADCPostArray[i+3].sUnitText = ADCPostGrid.Rows[i].Cells[4].Value.ToString();
            }
            isUserCellChange = true;
            //###TASK: Report error if number out of range and not INT32 style. 
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### ADC24 Data Section
        //#####################################################################################################
        private System.Windows.Forms.DataGridView ADC24dataGrid;
        DataGridViewTextBoxColumn[] ADC24CmdColumn;

        #region//=============================================================================================Setup Column Format
        static SupportDataGrid[] myADC24DataLayout =
            new SupportDataGrid[]{
                new SupportDataGrid("Ch",30),
                new SupportDataGrid("HW-Mode",70),
                new SupportDataGrid("Format",60),
                new SupportDataGrid("Readout Data",140),
                new SupportDataGrid("Unit",50)
            };
        #endregion

        #region //=============================================================================================SupportDataGridSetup
        private void SupportDataGridSetup()
        {
            ADC24dataGrid = new System.Windows.Forms.DataGridView();
            ADC24dataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            ADC24dataGrid.RowTemplate.Height = 19;
            ADC24dataGrid.RowTemplate.MinimumHeight = 19;
            //--------------------------------------------------------------------
            ADC24dataGrid.Location = new System.Drawing.Point(0, 0);
            ADC24dataGrid.Size = new System.Drawing.Size(354, 100);
            ADC24dataGrid.MaximumSize = new System.Drawing.Size(354, 100);
            ADC24dataGrid.MinimumSize = new System.Drawing.Size(354, 100);
            ADC24dataGrid.TabIndex = 0;
            ADC24dataGrid.AllowUserToAddRows = false;
            ADC24dataGrid.AllowUserToDeleteRows = false;
            ADC24dataGrid.AllowUserToResizeColumns = false;
            ADC24dataGrid.AllowUserToResizeRows = false;
            ADC24dataGrid.BackgroundColor = System.Drawing.Color.White;
            ADC24dataGrid.MultiSelect = false;
            ADC24dataGrid.Name = "CaptureReadout";
            ADC24dataGrid.RowHeadersVisible = false;
            ADC24dataGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
            //--------------------------------------------------------------------
            ADC24CmdColumn = new DataGridViewTextBoxColumn[5];
            for (int i = 0; i < 5; i++)
            {
                ADC24CmdColumn[i] = new DataGridViewTextBoxColumn();
                ADC24CmdColumn[i].Name = "txt" + myADC24DataLayout[i].Label;
                ADC24CmdColumn[i].HeaderText = myADC24DataLayout[i].Label;
                ADC24CmdColumn[i].Width = myADC24DataLayout[i].Width;
                ADC24CmdColumn[i].Resizable = DataGridViewTriState.False;
                ADC24CmdColumn[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.ADC24dataGrid.Columns.Add(ADC24CmdColumn[i]);
            }
            //=========================================Add Row
            for (int i = 0; i < 3; i++)
            {
                this.ADC24dataGrid.Rows.Add(new string[] { (i + 4).ToString("D1") });
            }
            this.ADC24dataGrid.Rows.Add(new string[] {"7"});    // Temperature channel
            ADC24dataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ADC24dataGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            ADC24dataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            ADC24dataGrid.AllowUserToResizeRows = false;
            this.gbFCL30Data.Controls.Add(this.ADC24dataGrid);
        }
        #endregion

        #region //=============================================================================================SupportDataGrid_Update
        public void SupportDataGrid_Update()
        {
            if (ADC24dataGrid == null)      // Not Init yet, do this later. 
                return;
            #region-------------------------Channel 1 to 6
            for (int i = 0; i <3; i++)
            {
                if (ADC24Ch[i].iHardwareMode == 0)
                {
                    ADC24dataGrid.Rows[i].Cells[1].Value = "OFF";
                    ADC24dataGrid.Rows[i].Cells[2].Value = "";
                    ADC24dataGrid.Rows[i].Cells[3].Value = "";
                    ADC24dataGrid.Rows[i].Cells[4].Value = "";
                }
                else
                {
                    ADC24dataGrid.Rows[i].Cells[1].Value = ADC24Ch[i+3].iHardwareMode;
                    ADC24dataGrid.Rows[i].Cells[4].Value = "";
                    ADC24dataGrid.Rows[i].Cells[2].Value = "N/A";
                    switch (ADC24Ch[i+3].uReadoutFormat)
                    {
                        case (7):
                            {
                                ADC24dataGrid.Rows[i].Cells[2].Value = "[Double]";
                                ADC24dataGrid.Rows[i].Cells[4].Value = ADCPostArray[i + 3].sUnitText;
                                break;
                            }
                        case (8):
                            {
                                ADC24dataGrid.Rows[i].Cells[2].Value = "[8/B]";
                                ADC24dataGrid.Rows[i].Cells[4].Value = ADCPostArray[i + 3].sUnitText;
                                break;
                            }
                        case (9):
                            {
                                ADC24dataGrid.Rows[i].Cells[2].Value = "[9/U]";
                                ADC24dataGrid.Rows[i].Cells[4].Value = ADCPostArray[i + 3].sUnitText;
                                break;
                            }
                        default:
                            ADC24dataGrid.Rows[i].Cells[2].Value = "ERR";
                            break;
                    }
                    
                }
                #endregion

                #region-------------------Channel 7 for Temp
                if (ADC24Setup.bInternalTemp == false)
                {
                    ADC24dataGrid.Rows[3].Cells[1].Value = "OFF";
                    ADC24dataGrid.Rows[3].Cells[2].Value = "";
                    ADC24dataGrid.Rows[3].Cells[3].Value = "";
                }
                else
                {
                    ADC24dataGrid.Rows[3].Cells[1].Value = "ON";
                    switch (ADC24TempCh.uReadoutFormat)
                    {
                        case (13):
                            {
                                ADC24dataGrid.Rows[3].Cells[2].Value = "mK";
                                break;
                            }
                        case (14):
                            {
                                ADC24dataGrid.Rows[3].Cells[2].Value = "mC";
                                break;
                            }
                        case (15):
                            {
                                ADC24dataGrid.Rows[3].Cells[2].Value = "mF";
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            #endregion

        }
        #endregion

        #region //=============================================================================================SupportDataGrid_ClearData (clear invalid data)
        private void SupportDataGrid_ClearData()
        {
            if (ADC24dataGrid == null)      // Not Init yet, do this later. 
                return;
            for (int i = 0; i < 3; i++)
            {
                //ADC24dataGrid.Rows[i].Cells[2].Value = "";
                ADC24dataGrid.Rows[i].Cells[3].Value = "";
                //ADC24dataGrid.Rows[i].Cells[4].Value = "";
            }
            myFLC30MagMath.iMagRaw.X = 0;
            myFLC30MagMath.iMagRaw.Y = 0;
            myFLC30MagMath.iMagRaw.Z = 0;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Window
        //#####################################################################################################

        #region //=============================================================================================FCL30_InitDefaultSetupFirstTime  
        public void FCL30_InitDefaultSetupFirstTime()
        {
            int i;
            //----------------------------------------------------------- +FCL30SETUP (uG; XYZ ;Enabled)
            FCL30Setup.iFormatReadout = 4;
            FCL30Setup.iAxisPolicy = 1;
            FCL30Setup.bIsEnable = true;
            //---------------------------------------------------------- +ADC24SETUP(1; 0; 1; Speed; 1; Peroid; 1)
            ADC24Setup.bCapType = true;
            ADC24Setup.bRTDMode = false;
            ADC24Setup.bInternalTemp = true;
            //ADC24Setup.uSpeed = //Leave to ADC24 tabpage to configure.
            ADC24Setup.bChopped = true;
            //ADC24Setup.uPeroid = //Leave to ADC24 tabpage to configure.
            ADC24Setup.bIsEnable = true;
            //----------------------------------------------------------- +ADC24CH (Ch; 4; 0; 0; 8 ) for Channel 4,5,6
            for (i = 3; i <= 5; i++)
            {
                ADC24Ch[i].iHardwareMode = 4;
                ADC24Ch[i].bBufferMode = false;
                ADC24Ch[i].uGain = 0;
                ADC24Ch[i].uReadoutFormat = 8;      //Bipolar, Gain=1V/V, Enable Offset/Gain equation. 
            }
            //----------------------------------------------------------- +ADC24POST(4; 0; 350000; uG)
            for (i = 3; i <= 5; i++)
            {
                ADCPostArray[i].iOffset = 0;            // User can adjust that. 
                ADCPostArray[i].uGain = 350000U;        // 0.35V/V (Actual) 
            }
            ADCPostArray[3].sUnitText = "uG/X";         // Default XYZ (which is marked on body). 
            ADCPostArray[4].sUnitText = "uG/Y";
            ADCPostArray[5].sUnitText = "uG/Z";
            //----------------------------------------------------------- +ADC24TEMPCH7(Enable (ON); Format (mK))
            ADC24Setup.bInternalTemp = true;
            ADC24TempCh.uReadoutFormat = 13;
            //------------------------------------------
            cbFCL30ExportSetup.Checked = true;          // Need export to update.
        }
        #endregion

        #region //=============================================================================================pB_FCL3_Image_Click  
        private void pB_FCL3_Image_Click(object sender, EventArgs e)
        {
            switch (MiniPort_FCL30_ImageSelect)
            {
                case 0:
                    pB_FCL3_Image.Image = UDT_Term.Properties.Resources.FCL30_Diagram1A;
                    MiniPort_FCL30_ImageSelect++;
                    break;
                case 1:
                    pB_FCL3_Image.Image = UDT_Term.Properties.Resources.FCL30_Diagram1B;
                    MiniPort_FCL30_ImageSelect++;
                    break;
                case 2:
                    pB_FCL3_Image.Image = UDT_Term.Properties.Resources.FCL30_Diagram1C;
                    MiniPort_FCL30_ImageSelect = 0;
                    break;
                default:
                    break;
            }
        }


        #endregion

        #region //=============================================================================================chFCL30Enable_MouseClick  
        private void chFCL30Enable_MouseClick(object sender, MouseEventArgs e)
        {
            if (chFCL30Enable.Focused)
            {
                myGlobalBase.bTabWindowEnableUpdate = true;
                if (chFCL30Enable.Checked==true)
                {
                    TabWindowEnable.bFLC370 = true;
                    TabWindowEnable.bAD7794 = true;
                    TabWindowEnable.bADCSupport = true;

                    FCL30Setup.bIsEnable = true;

                }
                else
                {
                    TabWindowEnable.bFLC370 = false;
                    FCL30Setup.bIsEnable = false;           // This has no impact on Logger CVS because it step up the AD7794.

                }
                FCL307_Refresh_Window_Control();
                isUserCellChange = true;
                cbFCL30ExportSetup.Checked = true;          // Need export to update.
            }
        }
        #endregion

        #region //=============================================================================================rbFCL30Format_MouseClick 
        private void rbFCL30Format_MouseClick(object sender, MouseEventArgs e)
        {
            int OldValue = FCL30Setup.iFormatReadout;
            if (rbFCL30nT.Focused == true)
            {
                FCL30Setup.iFormatReadout = 1;
            }
            else if (rbFCL30pT.Focused == true)
            {
                FCL30Setup.iFormatReadout = 2;
            }
            else if (rbFCL30mG.Focused == true)
            {
                FCL30Setup.iFormatReadout = 3;
            }
            else if (rbFCL30uG.Focused == true)
            {
                FCL30Setup.iFormatReadout = 4;
            }
            else if (rbFCL30Raw.Focused == true)
            {
                FCL30Setup.iFormatReadout = 0;
            }
            if(FCL30Setup.iFormatReadout != OldValue)   // remove data readout if changed. 
            {
                SupportDataGrid_ClearData();
                UpdateGainOffsetofFormatReadout();
                ADCPostGrid_RefreshUpdate();
                FCL30_Math_ClearAllText();
            }
            FCL307_Refresh_Window_Control();
            cbFCL30ExportSetup.Checked = true;          // Need export to update.
        }
        #endregion

        #region //=============================================================================================rbFCL30Axis_MouseClick 
        private void rbFCL30Axis_MouseClick(object sender, MouseEventArgs e)
        {
            if (rbFCL30OGC.Focused == true)
            {
                FCL30Setup.iAxisPolicy = 0;
            }
            else if (rbFCL30MRC.Focused == true)
            {
                FCL30Setup.iAxisPolicy = 2;
            }
            else if (rbFCL30XYZ.Focused == true)
            {
                FCL30Setup.iAxisPolicy = 1;
            }
            FCL307_Refresh_Window_Control();
            cbFCL30ExportSetup.Checked = true;          // Need export to update.
        }

        #endregion

        //#####################################################################################################
        //###################################################################################### Export Setup
        //#####################################################################################################

        #region //=============================================================================================btnFCL30ExportSetting_Click 
        private void btnFCL30ExportSetting_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            // We need to read ADC24 Global Setup first and then do below.
            FCL30_Update_Setup_Data();
        }
        #endregion
        #region //=============================================================================================btnADC24CaptureNow_Click & CalBacks 
        public DMFP_BulkFrame FCL30_Update_Setup_Data()
        {
            ADC24Setup.bIsEnable = true;                    // Force AD7794 to enable mode.
            TabWindowEnable.bAD7794 = true;
            TabWindowEnable.bADCSupport = true;
            myGlobalBase.bTabWindowEnableUpdate = true;     // When Tab Window switched over, it force update. 
            cbFCL30ExportSetup.Checked = false;             // No need to export again.

            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();

            //---------------------------------------------------------------------------------------------------------FCL30 Initial Setup 
            //+FCL30SETUP (Format; AxiPolicy ;isEnabled)\n	    This if isEnable=1, trigger default Gain/Offset setting within firmware.
            string command = "+FCL30SETUP(";
            command += (FCL30Setup.iFormatReadout).ToString() + ";";
            command += (FCL30Setup.iAxisPolicy).ToString() + ";";
            command += (Tools.BinaryFalseTrue(FCL30Setup.bIsEnable)).ToString() + ")";
            cDMFP_BulkFrame.AddItem(command, null);
            //---------------------------------------------------------------------------------------------------------ADC24 Global Setup
            //+ADC24SETUP(1; 0; 1; Speed; 1; Peroid; 1)
            command = "+ADC24SETUP(1;0;1;";
            command += (ADC24Setup.uSpeed).ToString() + ";";
            command += "1;";
            command += "0x" + (ADC24Setup.uPeroid).ToString("X") + ";";
            command += "1)";
            cDMFP_BulkFrame.AddItem(command, null);
            //---------------------------------------------------------------------------------------------------------ADC24 Channel Setup
            // +ADC24CH (Ch; 4; 0; 0; 8 ) for Channel 4,5,6
            for (int i = 4; i <= 6; i++)
            {
                command = "+ADC24CH(";
                command += (i).ToString() + ";";
                command += "4;0;0;8)";
                cDMFP_BulkFrame.AddItem(command, null);
            }
            //---------------------------------------------------------------------------------------------------------ADC24 Offset/Gain
            // +ADC24POST (Ch; Offset; Gain; pT)
            for (int i = 4; i <= 6; i++)
            {
                command = "+ADC24POST(";
                command += (i).ToString() + ";";                               // Channel
                command += (ADCPostArray[i - 1].iOffset).ToString() + ";";     // Offset
                command += (ADCPostArray[i - 1].uGain).ToString() + ";";       // Gain
                command += "0s" + (ADCPostArray[i - 1].sUnitText) + ";";       // UnitText.
                command += ")";
                cDMFP_BulkFrame.AddItem(command, null);
            }
            //---------------------------------------------------------------------------------------------------------LoggerCVS Setup

            //---------------------------------------------------------------------------------------------------------Include ADC24 Temperature as well. 
            command = "+ADC24TEMPCH7(";
            command += (Tools.BinaryFalseTrue(ADC24Setup.bInternalTemp)).ToString() + ";";
            command += (ADC24TempCh.uReadoutFormat).ToString() + ")";
            cDMFP_BulkFrame.AddItem(command, null);

            //---------------------------------------------------------------------Send Bulk Frame
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 10;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);
            //------------------------------------------------------------------------------------

            return cDMFP_BulkFrame;
        }
        #endregion

        #region //=============================================================================================UpdateGainOffsetofFormatReadout
        private void UpdateGainOffsetofFormatReadout()
        {
            int i;
            switch (FCL30Setup.iFormatReadout)           // 0 = Raw Data, 1 = nT, 2 = pT, 3 =mG, 4=uG
            {
                case (1):           //nT
                    {
                        for (i = 3; i <= 5; i++)
                        {
                            ADCPostArray[i].iOffset = 0;            // User can adjust that. 
                            ADCPostArray[i].uGain = 35000U;        // 0.035V/V (Actual) 
                        }
                        ADCPostArray[3].sUnitText = "nT/X";         // Default XYZ (which is marked on body). 
                        ADCPostArray[4].sUnitText = "nT/Y";
                        ADCPostArray[5].sUnitText = "nT/Z";
                        break;
                    }
                case (2):           //pT
                    {
                        for (i = 3; i <= 5; i++)
                        {
                            ADCPostArray[i].iOffset = 0;            // User can adjust that. 
                            ADCPostArray[i].uGain = 35000000U;      // 35.0V/V (Actual) 
                        }
                        ADCPostArray[3].sUnitText = "pT/X";         // Default XYZ (which is marked on body). 
                        ADCPostArray[4].sUnitText = "pT/Y";
                        ADCPostArray[5].sUnitText = "pT/Z";
                        break;
                    }
                case (3):           //mG
                    {
                        for (i = 3; i <= 5; i++)
                        {
                            ADCPostArray[i].iOffset = 0;            // User can adjust that. 
                            ADCPostArray[i].uGain = 350U;           // 0.00035V/V (Actual) 
                        }
                        ADCPostArray[3].sUnitText = "mG/X";         // Default XYZ (which is marked on body). 
                        ADCPostArray[4].sUnitText = "mG/Y";
                        ADCPostArray[5].sUnitText = "mG/Z";
                        break;
                    }
                case (4):           //uG (Default)
                    {
                        for (i = 3; i <= 5; i++)
                        {
                            ADCPostArray[i].iOffset = 0;            // User can adjust that. 
                            ADCPostArray[i].uGain = 350000U;        // 0.35V/V (Actual) 
                        }
                        ADCPostArray[3].sUnitText = "uG/X";         // Default XYZ (which is marked on body). 
                        ADCPostArray[4].sUnitText = "uG/Y";
                        ADCPostArray[5].sUnitText = "uG/Z";
                        break;
                    }
                default:            // Raw Data
                    {
                        for (i = 3; i <= 5; i++)
                        {
                            ADCPostArray[i].iOffset = 0;
                            ADCPostArray[i].uGain = 1;
                        }
                        ADCPostArray[3].sUnitText = "Raw/X";
                        ADCPostArray[4].sUnitText = "Raw/Y";
                        ADCPostArray[5].sUnitText = "Raw/Z";
                        break;
                    }
            }
            
        }
        #endregion


        //#####################################################################################################
        //###################################################################################### Import Setup
        //#####################################################################################################

        #region //=============================================================================================btnFCL30ImportSetting_Click
        private void btnFCL30ImportSetting_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();
            //---------------------------------------------------------------------------------------------------------FCL30 Initial Setup 
            //+FCL30SETUP (Format; AxiPolicy ;isEnabled)\n	    This if isEnable=1, trigger default Gain/Offset setting within firmware.

            cDMFP_BulkFrame.AddItem("+FCL30SETUPR()", new DMFP_Delegate(DMFP_FCL30SETUPR_CallBack));
            //---------------------------------------------------------------------------------------------------------ADC24 Global Setup
            //+ADC24SETUP(1; 0; 1; Speed; 1; Peroid; 1)
            cDMFP_BulkFrame.AddItem("+ADC24SETUPR()", new DMFP_Delegate(DMFP_ADC24SETUPR_CallBack));
            //---------------------------------------------------------------------------------------------------------ADC24 Channel Setup
            // +ADC24CH (Ch; 4; 0; 0; 8 ) for Channel 4,5,6
            cDMFP_BulkFrame.AddItem("+ADC24CHR(0x04)", new DMFP_Delegate(DMFP_ADC24CHRX_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24CHR(0x05)", new DMFP_Delegate(DMFP_ADC24CHRX_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24CHR(0x06)", new DMFP_Delegate(DMFP_ADC24CHRX_CallBack));
            
            //---------------------------------------------------------------------------------------------------------ADC24 Offset/Gain
            // +ADC24POST (Ch; Offset; Gain; pT)
            cDMFP_BulkFrame.AddItem("+ADC24POSTR(0x04)", new DMFP_Delegate(DMFP_ADC24POSTR_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24POSTR(0x05)", new DMFP_Delegate(DMFP_ADC24POSTR_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24POSTR(0x06)", new DMFP_Delegate(DMFP_ADC24POSTR_CallBack));
            //---------------------------------------------------------------------------------------------------------LoggerCVS Setup



            //---------------------------------------------------------------------------------------------------------Include ADC24 Temperature as well.
            cDMFP_BulkFrame.AddItem("+ADC24TEMPCH7R()", new DMFP_Delegate(DMFP_DC24TEMPCH7R_CallBack));         // Keep this as last element (which used to trigger refresh tabpage)
            //---------------------------------------------------------------------Send Bulk Frame
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 10;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);
        }
        #endregion

        #region //=============================================================================================DMFP_FCL30SETUPR_CallBack
        public void DMFP_FCL30SETUPR_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            // +FCL30SETUP(Format; AxiPolicy; isEnabled)\n
            if (hPara[0] == 0xFFFF)
            {
                FCL30Setup.iFormatReadout = iPara[0];
                FCL30Setup.iAxisPolicy = iPara[1];
                FCL30Setup.bIsEnable = Convert.ToBoolean(iPara[2]);
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_FCL30SETUPR_CallBack()...has Error code: 0x" + hPara[0].ToString("X"));
            }
        }
        #endregion

        #region //=============================================================================================DMFP_ADC24SETUPR_CallBack
        public void DMFP_ADC24SETUPR_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] == 0xFFFF)
            {
                ADC24Setup.bCapType = Convert.ToBoolean(iPara[0]);
                ADC24Setup.bRTDMode = Convert.ToBoolean(iPara[1]);
                ADC24Setup.bInternalTemp = Convert.ToBoolean(iPara[2]);
                ADC24Setup.uSpeed = iPara[3];
                ADC24Setup.bChopped = Convert.ToBoolean(iPara[4]);
                ADC24Setup.uPeroid = (int)hPara[1];
                ADC24Setup.bIsEnable = Convert.ToBoolean(iPara[5]);
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_ADC24SETUPR_CallBack()...has Error code: 0x" + hPara[0].ToString("X"));
            }
        }
        #endregion

        #region //=============================================================================================DMFP_ADC24CHRX_CallBack
        public void DMFP_ADC24CHRX_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            int channel = iPara[0] - 1;     // Channel 
            if (hPara[0] == 0xFFFF)
            {
                if ((channel >= 0) & (channel <= 5))
                {
                    ADC24Ch[channel].iHardwareMode = iPara[1];
                    ADC24Ch[channel].bBufferMode = Convert.ToBoolean(iPara[2]);
                    ADC24Ch[channel].uGain = iPara[3];
                    ADC24Ch[channel].uReadoutFormat = iPara[4];
                }
                else
                {
                    myMainProg.myRtbTermMessageLF("#E: DMFP_ADC24CHRX_CallBack()...has channel error");
                }
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_ADC24CHRX_CallBack()...has Error code: 0x" + hPara[0].ToString("X"));
            }
        }
        #endregion

        #region //=============================================================================================DMFP_DC24TEMPCH7R_CallBack
        public void DMFP_DC24TEMPCH7R_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] == 0xFFFF)
            {
                ADC24Setup.bInternalTemp = Convert.ToBoolean(iPara[0]);
                if ((iPara[1] >= 13) & (iPara[1] <= 15))
                    ADC24TempCh.uReadoutFormat = iPara[1];

            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_DC24TEMPCH7R_CallBack()...has Error code: 0x" + hPara[0].ToString("X"));
            }
            FCL307_Refresh_Window_Control();
        }
        #endregion

        #region //=============================================================================================DMFP_ADC24POSTR_CallBack
        //printf("-ADC24POST(0xFFFF;");
        //printf("%d;", Channel);     //0 = disabled channel.
        //printf("%d;", PostData_Offset[Channel]);
        //printf("%u;", PostData_Gain[Channel]);
        //printf("0sNull;");
        public void DMFP_ADC24POSTR_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] == 0xFFFF)
            {
                int channel = iPara[1];
                if ((channel >= 1) & (channel <= 6))
                {
                    ADCPostArray[channel - 1].iOffset = iPara[2];
                    ADCPostArray[channel - 1].uGain = Convert.ToUInt32(iPara[3]);
                    ADCPostArray[channel - 1].sUnitText = sPara[4];
                    if (channel == 6)
                        ADCPostGrid_RefreshUpdate();
                }
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_ADC24POSTR_CallBack()...has Error code: 0x" + hPara[0].ToString("X"));
            }

        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Capture Data
        //#####################################################################################################

        #region //=============================================================================================btnADC24CaptureNow_Click & CalBacks 
        private void btnFCL30CaptureNow_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            //--------------------------------------------------------------Send Setup Data before Capture.
            FCL307_Refresh_Window_Control();
            if (cbFCL30ExportSetup.Checked == true)
            {
                DMFP_BulkFrame cDMFP_BulkFrame = FCL30_Update_Setup_Data();
                while (cDMFP_BulkFrame.isBusy == true)
                {
                    Tools.InteractivePause(TimeSpan.FromMilliseconds(25));
                }
                cbFCL30ExportSetup.Checked = false;
            }
            //--------------------------------------------------------------Send Capture Command
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ADC24DATA = new DMFP_Delegate(DMFP_ADC24_ADC24DATA_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+ADC24DATA()";
            if (myGlobalBase.bNoRFTermMessage == false)
                myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t3 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_ADC24DATA, sMessage, 250, (int)eUSBDeviceType.MiniAD7794));
            t3.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
        }
        #endregion

        #region //=============================================================================================DMFP_ADC24_ADC24DATA_CallBack  
        public void DMFP_ADC24_ADC24DATA_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Status = hPara[0];
            if (Status != 0xFFFF)       // No Error
            {
                myMainProg.myRtbTermMessageLF("#E:ADC24DATA reported error code:0x" + hPara[0].ToString("X"));
                return;
            }
            //---------------------------------------------------Update Readout.
            for (int i = 0; i < 3; i++)
            {
                ADC24dataGrid.Rows[i].Cells[3].Value = iPara[i+3];
            }
            if (iPara.Count == 7)
            {
                ADC24dataGrid.Rows[3].Cells[3].Value = iPara[6];
            }
            //---------------------------------------------------FCL30 Maths
            if ((FCL30Setup.iFormatReadout != 0))     // XYZ Axis and uG supported so far.....
            {
                gpBasicMath.Visible = true;
                isMathFeatureSupported = true;
                myFLC30MagMath.iMagRaw.X = iPara[3];
                myFLC30MagMath.iMagRaw.Y = iPara[4];
                myFLC30MagMath.iMagRaw.Z = iPara[5];
                // Normalize to standard XYZ axis before doing maths, see MiniAD7794 Jornal Note
                FCL30_DisplayCorrectedAxisPolicy();
                FCL30_Math_CalculateAll();
            }
            else
            {
                gpBasicMath.Visible = false;
                isMathFeatureSupported = false;
            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Math
        //#####################################################################################################

        #region //==================================================HMR2300_Math_CalculateAll
        public void FCL30_Math_CalculateAll()
        {
            //-----------------------------------------Flip Axis to standard Math (FCL30 XYZ to Honeywell Axis XYZ)
            iVector3 iResult = new iVector3();

            iResult.X = myFLC30MagMath.iMagRaw.X;
            iResult.Y = myFLC30MagMath.iMagRaw.Y;
            iResult.Z = myFLC30MagMath.iMagRaw.Z;

            myFLC30MagMath.iMagRaw.X = iResult.Z;
            myFLC30MagMath.iMagRaw.Y = -iResult.Y;
            myFLC30MagMath.iMagRaw.Z = iResult.X;

            //---------------------------------------------------------------FormatReadout, normalise to mG prior to caluclation. 
            switch (FCL30Setup.iFormatReadout)
            {
                case (1):   //nT
                    {
                        myFLC30MagMath.dMagRaw = myFLC30MagMath.ConvertINT32toDouble(100.0);       //nT to mG
                        break;
                    }
                case (2):   //pT
                    {
                        myFLC30MagMath.dMagRaw = myFLC30MagMath.ConvertINT32toDouble(100000.0);    //pT to mG
                        break;
                    }
                case (3):   //mG
                    {
                        myFLC30MagMath.dMagRaw = myFLC30MagMath.ConvertINT32toDouble(1.0);       //mG to mG
                        break;
                    }
                case (4):   //uG
                    {
                        myFLC30MagMath.dMagRaw = myFLC30MagMath.ConvertINT32toDouble(1000.0);    //uG to mG
                        break;
                    }
                default:    // This is error in code, should not happen to come here!
                    {
                        FCL30_Math_ClearAllText();
                        return;
                    }
            }
            myFLC30MagMath.dMagCal = myFLC30MagMath.ProcessCalibration(myFLC30MagMath.dMagRaw);
            if (myFLC30MagMath.ProcessMath(myFLC30MagMath.dMagCal) ==false)
            {
                tboInclination.Text = "";
                tboDeclination.Text = "";
                tboNorthX.Text = "";
                tboWestY.Text = "";
                tboIntensity.Text = "";
            }
            else
            {
                tboInclination.Text = Tools.Math_RadianToDegree(myFLC30MagMath.dInclination).ToString("N1");
                tboDeclination.Text = Tools.Math_RadianToDegree(myFLC30MagMath.dDeclination).ToString("N1");
                tboNorthX.Text = myFLC30MagMath.dNorthX.ToString("N2");
                tboWestY.Text = myFLC30MagMath.dEastY.ToString("N2");
                tboIntensity.Text = myFLC30MagMath.dIntensity.ToString("N2");
            }
            //----------------------------------------------------------------

        }
        #endregion

        #region //==================================================FCL30_DisplayCorrectedAxisPolicy
        // This code does not impact math variable as long it placed behind the FCL30_Math_CalculateAll()
        private void FCL30_DisplayCorrectedAxisPolicy()
        {
            Vector3 dResult = new Vector3();
            Vector3 dResult1 = new Vector3();
            //---------------------------------------------------------------FormatReadout, normalise to mG prior to caluclation. 
            switch (FCL30Setup.iFormatReadout)
            {
                case (1):   //nT
                    {
                        dResult = myFLC30MagMath.ConvertINT32toDouble(100.0);       //nT to mG
                        break;
                    }
                case (2):   //pT
                    {
                        dResult = myFLC30MagMath.ConvertINT32toDouble(100000.0);    //pT to mG
                        break;
                    }
                case (3):   //mG
                    {
                        dResult = myFLC30MagMath.ConvertINT32toDouble(1.0);       //mG to mG
                        break;
                    }
                case (4):   //uG
                    {
                        dResult = myFLC30MagMath.ConvertINT32toDouble(1000.0);    //uG to mG
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
            dResult1 = dResult;
            switch (FCL30Setup.iAxisPolicy)
            {
                case (0):   // OGC
                    {
                        dResult.X = -dResult1.Y;
                        dResult.Y = +dResult1.X;
                        dResult.Z = +dResult1.Z;
                        break;
                    }
                case (1):   // XYZ (Honeywell XYZ Standard prior to equation)
                    {
                        dResult.X = +dResult1.Z;
                        dResult.Y = -dResult1.Y;
                        dResult.Z = +dResult1.X;
                        break;
                    }
                case (2):   //MRC
                    {
                        dResult.X = +dResult1.X;
                        dResult.Y = +dResult1.Y;
                        dResult.Z = +dResult1.Z;
                        break;
                    }
                default:
                    break;
            }
            //-------------------------------------------------
            tbXAxis.Text = dResult.X.ToString("N2");
            tbYAxis.Text = dResult.Y.ToString("N2");
            tbZAxis.Text = dResult.Z.ToString("N2");
            //-------------------------------------------------
        }
        #endregion

        #region //==================================================FCL30_Math_ClearAllText
        public void FCL30_Math_ClearAllText()
        {
            tboInclination.Text = "";
            tboDeclination.Text = "";
            tboNorthX.Text = "";
            tboWestY.Text = "";
            tboIntensity.Text = "";
            tbXAxis.Text = "";
            tbYAxis.Text = "";
            tbZAxis.Text = "";
        }
        #endregion


        //#####################################################################################################
        //###################################################################################### LoggerCVS Script
        //#####################################################################################################

        #region //==================================================LoggerCVS Header
        public void FCL30_LoggerCVS_Setup()
        {
        }
        #endregion




    }
}