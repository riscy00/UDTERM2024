using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JR.Utils.GUI.Forms;
using UDT_Term;
using System.Threading;

namespace UDT_Term_FFT
{
    public partial class ADC24AD7794 : UserControl
    {
        List<Image> ADC24Image;
        ToolTip toolTip1;
        DialogSupport mbOperationBusy;
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        MainProg myMainProg;
        USB_Message_Manager myUSB_Message_Manager;
        TVMiniPort_FCL30Setup FCL30Setup;
        List<TVMiniPort_AD7794Channel> ADC24Ch;
        TVMiniPort_AD7794TempCh ADC24TempCh;
        TVMiniPort_AD7794Setup ADC24Setup;
        List<TVMiniPort_ADCPost> ADCPostArray;
        DMFProtocol myDMFProtocol;
        TVMiniPort_TabEnable TabWindowEnable;


        #region//================================================================Reference
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }
        public void MyTabWindowEnable (TVMiniPort_TabEnable TabWindowEnableRef)
        {
            TabWindowEnable = TabWindowEnableRef;
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
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        public void MyDMFProtocol(DMFProtocol myDMFProtocolRef)
        {
            myDMFProtocol = myDMFProtocolRef;
        }
        #endregion
        public ADC24AD7794()
        {
            InitializeComponent();
        }

        private Boolean ibMouseClicked = false;

        //#####################################################################################################
        //###################################################################################### Window Section
        //#####################################################################################################

        #region //=============================================================================================ADC24AD7794_Load  
        private void ADC24AD7794_Load(object sender, EventArgs e)
        {
            SupportDataGridSetup();
            #region //---------------Setup Combo Boxes
            //----------------------------------------------------------------------------------CH1
            if (this.cbADC24CH1_Format.Items.Count == 0)
            {
                cbADC24CH1_Format.Items.AddRange(ADC24Setup.lReadoutFormat().ToArray<object>());
            }
            if (this.cbADC24CH1_Hardware.Items.Count == 0)
            {
                cbADC24CH1_Hardware.Items.AddRange(ADC24Setup.lHardwareMode().ToArray<object>());
            }
            if (this.cbADC24CH1_Gain.Items.Count == 0)
            {
                cbADC24CH1_Gain.Items.AddRange(ADC24Setup.lGainSetting().ToArray<object>());
            }
            //----------------------------------------------------------------------------------CH2
            if (this.cbADC24CH2_Format.Items.Count == 0)
            {
                cbADC24CH2_Format.Items.AddRange(ADC24Setup.lReadoutFormat().ToArray<object>());
            }
            if (this.cbADC24CH2_Hardware.Items.Count == 0)
            {
                cbADC24CH2_Hardware.Items.AddRange(ADC24Setup.lHardwareMode().ToArray<object>());
            }
            if (this.cbADC24CH2_Gain.Items.Count == 0)
            {
                cbADC24CH2_Gain.Items.AddRange(ADC24Setup.lGainSetting().ToArray<object>());
            }
            //----------------------------------------------------------------------------------CH3
            if (this.cbADC24CH3_Format.Items.Count == 0)
            {
                cbADC24CH3_Format.Items.AddRange(ADC24Setup.lReadoutFormat().ToArray<object>());
            }
            if (this.cbADC24CH3_Hardware.Items.Count == 0)
            {
                cbADC24CH3_Hardware.Items.AddRange(ADC24Setup.lHardwareMode().ToArray<object>());
            }
            if (this.cbADC24CH3_Gain.Items.Count == 0)
            {
                cbADC24CH3_Gain.Items.AddRange(ADC24Setup.lGainSetting().ToArray<object>());
            }
            //----------------------------------------------------------------------------------CH4
            if (this.cbADC24CH4_Format.Items.Count == 0)
            {
                cbADC24CH4_Format.Items.AddRange(ADC24Setup.lReadoutFormat().ToArray<object>());
            }
            if (this.cbADC24CH4_Hardware.Items.Count == 0)
            {
                cbADC24CH4_Hardware.Items.AddRange(ADC24Setup.lHardwareMode().ToArray<object>());
            }
            if (this.cbADC24CH4_Gain.Items.Count == 0)
            {
                cbADC24CH4_Gain.Items.AddRange(ADC24Setup.lGainSetting().ToArray<object>());
            }
            //----------------------------------------------------------------------------------CH5
            if (this.cbADC24CH5_Format.Items.Count == 0)
            {
                cbADC24CH5_Format.Items.AddRange(ADC24Setup.lReadoutFormat().ToArray<object>());
            }
            if (this.cbADC24CH5_Hardware.Items.Count == 0)
            {
                cbADC24CH5_Hardware.Items.AddRange(ADC24Setup.lHardwareMode().ToArray<object>());
            }
            if (this.cbADC24CH5_Gain.Items.Count == 0)
            {
                cbADC24CH5_Gain.Items.AddRange(ADC24Setup.lGainSetting().ToArray<object>());
            }
            //----------------------------------------------------------------------------------CH6
            if (this.cbADC24CH6_Format.Items.Count == 0)
            {
                cbADC24CH6_Format.Items.AddRange(ADC24Setup.lReadoutFormat().ToArray<object>());
            }
            if (this.cbADC24CH6_Hardware.Items.Count == 0)
            {
                cbADC24CH6_Hardware.Items.AddRange(ADC24Setup.lHardwareMode().ToArray<object>());
            }
            if (this.cbADC24CH6_Gain.Items.Count == 0)
            {
                cbADC24CH6_Gain.Items.AddRange(ADC24Setup.lGainSetting().ToArray<object>());
            }
            #endregion
            cbADC24RTDMode.Enabled = false;                             // Stay disabled until supported later (###TASK: RTD firmware support). 
            cbADC2CaptureExportSetup.Checked = false;
            ADC24Image = new List<Image>(5)
            {
                UDT_Term.Properties.Resources.Hardware0,
                UDT_Term.Properties.Resources.Hardware1,
                UDT_Term.Properties.Resources.Hardware2,
                UDT_Term.Properties.Resources.Hardware3,
                UDT_Term.Properties.Resources.Hardware4,
                UDT_Term.Properties.Resources.Hardware5
            };
            ADC24_Refresh_Window();
            lbNotFitted.Visible = false;
        }
        #endregion

        #region //=============================================================================================transparentTabControl1_SelectedIndexChanged

        private void transparentTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected = ttcADC24Channel.SelectedIndex;
            ADC24_Update_Format(selected);
        }

        #endregion

        //#####################################################################################################
        //###################################################################################### Tab Enable Manager
        //#####################################################################################################

        #region //=============================================================================================TabWindowEnableImplement
        public bool TabWindowEnableImplement()
        {
            if ((ADCPostArray == null)|(ADC24Ch==null))            // null ref due to timing between various window. This ensure it synced.
                return (false);

            if ((TabWindowEnable.bAD7794 == true) | (TabWindowEnable.bADCSupport == true))
            {
                TabWindowEnable.bAD7794 = true;                     // Alway
                TabWindowEnable.bADCSupport = true;
                ADC24_Refresh_Window();
                return true;
            }
            else
            {
                TabWindowEnable.bAD7794 = false;                    // Alway
                TabWindowEnable.bADCSupport = false;
                ADCSupport_Refresh_Window_Control();
            }
            return false;
        }
        #endregion
        
        //#####################################################################################################
        //###################################################################################### Window Control ON/OFF
        //#####################################################################################################

        #region //=============================================================================================ADCSupport_Refresh_Window_Control
        void ADCSupport_Refresh_Window_Control()
        {
            if (TabWindowEnable.bAD7794 == true)
            {
                TabWindowEnable.bADCSupport = true;
                lbAD7794Disable.Visible = false;
                ADC24Ch7_mK.Enabled = true;
                ADC24Ch7_mF.Enabled = true;
                ADC24Ch7_mC.Enabled = true;
                cbIntTempEnable.Enabled = true;
                rbADC24Max.Enabled = true;
                rbADC24Fast.Enabled = true;
                rbADC24Medium.Enabled = true;
                rbADC24Slow.Enabled = true;
                //----------------------------------Enable/Disable Window based on ON/OFF setting
                ttcADC24Channel.Enabled = true;
                gpADC24Setup.Visible = true;
                gpADC24Info.Visible = true;
                btnADC24ImportSetting.Visible = true;
                btnADC24ExportSetting.Visible = true;
                cbADC2CaptureExportSetup.Checked = true;
                cbADC24Enable.Checked = true;
            }
            else
            {
                TabWindowEnable.bADCSupport = false;
                lbAD7794Disable.Visible = true;
                ADC24Ch7_mK.Enabled = false;
                ADC24Ch7_mF.Enabled = false;
                ADC24Ch7_mC.Enabled = false;
                cbIntTempEnable.Enabled = false;
                rbADC24Max.Enabled = false;
                rbADC24Fast.Enabled = false;
                rbADC24Medium.Enabled = false;
                rbADC24Slow.Enabled = false;
                //----------------------------------Enable/Disable Window based on ON/OFF setting
                ttcADC24Channel.Enabled = false;
                gpADC24Setup.Visible = false;
                gpADC24Info.Visible = false;
                btnADC24ImportSetting.Visible = false;
                btnADC24ExportSetting.Visible = false;
                cbADC2CaptureExportSetup.Checked = true;
                cbADC24Enable.Checked = false;
            }
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
                new SupportDataGrid("HW-Mode",60),
                new SupportDataGrid("Format",60),
                new SupportDataGrid("Readout Data",159)
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
            ADC24dataGrid.Size = new System.Drawing.Size(309, 157);
            ADC24dataGrid.TabIndex = 0;
            ADC24dataGrid.AllowUserToAddRows = false;
            ADC24dataGrid.AllowUserToDeleteRows = false;
            ADC24dataGrid.AllowUserToResizeColumns = false;
            ADC24dataGrid.AllowUserToResizeRows = false;
            ADC24dataGrid.BackgroundColor = System.Drawing.Color.Gainsboro;
            ADC24dataGrid.MultiSelect = false;
            ADC24dataGrid.Name = "CmdBox";
            ADC24dataGrid.RowHeadersVisible = false;
            ADC24dataGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
            //--------------------------------------------------------------------
            ADC24CmdColumn = new DataGridViewTextBoxColumn[4];
            for (int i = 0; i < 4; i++)
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
            for (int i = 0; i < 7; i++)
            {
                this.ADC24dataGrid.Rows.Add(new string[] { (i + 1).ToString("D1") });
            }
            ADC24dataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ADC24dataGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            ADC24dataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            ADC24dataGrid.AllowUserToResizeRows = false;
            this.gpADC24Info.Controls.Add(this.ADC24dataGrid);
        }
        #endregion

        #region //=============================================================================================SupportDataGrid_Update
        public void SupportDataGrid_Update()
        {
            if (ADC24dataGrid == null)
                return;
            #region-------------------------Channel 1 to 6
            for (int i = 0; i < 6; i++)
            {
                if (ADC24dataGrid == null)
                    return;
                if (ADC24Ch[i].iHardwareMode == 0)
                {
                    ADC24dataGrid.Rows[i].Cells[1].Value = "OFF";
                    ADC24dataGrid.Rows[i].Cells[2].Value = "";
                    ADC24dataGrid.Rows[i].Cells[3].Value = "";
                }
                else
                {
                    ADC24dataGrid.Rows[i].Cells[1].Value = ADC24Ch[i].iHardwareMode;
                    if (ADC24Ch[i].uReadoutFormat == 0)
                    {
                        switch (ADC24Ch[i].iHardwareMode)
                        {
                            case (0):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "";
                                    break;
                                }
                            case (1):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "[1/U].uV";
                                    break;
                                }
                            case (2):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "[2/U].uV";
                                    break;
                                }
                            case (3):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "[3/B].uV";
                                    break;
                                }
                            case (4):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "[3/B].uV";
                                    break;
                                }
                            case (5):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "[5/B].uV";
                                    break;
                                }
                            case (6):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "WIP";
                                    break;
                                }
                            case (7):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "WIP";
                                    break;
                                }
                            default:
                                ADC24dataGrid.Rows[i].Cells[2].Value = "ERR";
                                break;
                        }
                    }
                    else
                    {
                        switch (ADC24Ch[i].uReadoutFormat)
                        {
                            case (1):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "[1/U].uV";
                                    break;
                                }
                            case (2):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "[2/U].uV";
                                    break;
                                }
                            case (3):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "[3/B].uV";
                                    break;
                                }
                            case (4):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "[4/B].uV";
                                    break;
                                }
                            case (5):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "RAW16";
                                    break;
                                }
                            case (6):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "RAW24";
                                    break;
                                }
                            case (7):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "[Double].V";
                                    break;
                                }
                            case (8):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "[8/B].uV";
                                    break;
                                }
                            case (9):
                                {
                                    ADC24dataGrid.Rows[i].Cells[2].Value = "[9/U].uV";
                                    break;
                                }
                            default:
                                ADC24dataGrid.Rows[i].Cells[2].Value = "ERR";
                                break;
                        }
                    }
                }
                #endregion

            #region-------------------Channel 7 for Temp
            if (ADC24Setup.bInternalTemp==false)
            {
                ADC24dataGrid.Rows[6].Cells[1].Value = "OFF";
                ADC24dataGrid.Rows[6].Cells[2].Value = "";
                ADC24dataGrid.Rows[6].Cells[3].Value = "";
            }
            else
            {
                ADC24dataGrid.Rows[6].Cells[1].Value = "ON";
                switch (ADC24TempCh.uReadoutFormat)
                {
                    case (13):
                        {
                            ADC24dataGrid.Rows[6].Cells[2].Value = "mK";
                            break;
                        }
                    case (14):
                        {
                            ADC24dataGrid.Rows[6].Cells[2].Value = "mC";
                            break;
                        }
                    case (15):
                        {
                            ADC24dataGrid.Rows[6].Cells[2].Value = "mF";
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

        #region //=============================================================================================cbADC24_Hardware_MouseDown  
        private void cbADC24_Hardware_MouseDown(object sender, MouseEventArgs e)
        {
            
            //ibMouseClicked = true;
        }
        #endregion

        #region //=============================================================================================cbADC24_Hardware_MouseUp  
        private void cbADC24_Hardware_MouseUp(object sender, MouseEventArgs e)
        {
            ibMouseClicked = true;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### ADC24 Update Window Controls
        //#####################################################################################################

        #region //=============================================================================================cbADC24CHX_Format_SelectedIndexChanged
        private void cbADC24CH1_Format_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ibMouseClicked == false)
                return;
            ADC24_Update_Format(0);
            ADC24_Refresh_Window();
        }
        private void cbADC24CH2_Format_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ibMouseClicked == false)
                return;
            ADC24_Update_Format(1);
            ADC24_Refresh_Window();
        }
        private void cbADC24CH3_Format_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ibMouseClicked == false)
                return;
            ADC24_Update_Format(2);
            ADC24_Refresh_Window();
        }
        private void cbADC24CH4_Format_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ibMouseClicked == false)
                return;
            ADC24_Update_Format(3);
            ADC24_Refresh_Window();
        }
        private void cbADC24CH5_Format_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ibMouseClicked == false)
                return;
            ADC24_Update_Format(4);
            ADC24_Refresh_Window();
        }
        private void cbADC24CH6_Format_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ibMouseClicked == false)
                return;
            ADC24_Update_Format(5);
            ADC24_Refresh_Window();
        }
        #endregion

        #region //=============================================================================================cbADC24CHX_Hardware_SelectedIndexChanged
        private void cbADC24CH1_Hardware_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ibMouseClicked == false)
                return;
            ADC24_Update_HardwareMode(0, cbADC24CH1_Hardware.SelectedIndex);
            ADC24_Update_GainSetting(0);
            ADC24_Refresh_Window();
        }
        private void cbADC24CH2_Hardware_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ibMouseClicked == false)
                return;
            ADC24_Update_HardwareMode(1, cbADC24CH2_Hardware.SelectedIndex);
            ADC24_Update_GainSetting(1);
            ADC24_Refresh_Window();
        }

        private void cbADC24CH3_Hardware_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ibMouseClicked == false)
                return;
            ADC24_Update_HardwareMode(2, cbADC24CH3_Hardware.SelectedIndex);
            ADC24_Update_GainSetting(2);
            ADC24_Refresh_Window();
        }

        private void cbADC24CH4_Hardware_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ibMouseClicked == false)
                return;
            ADC24_Update_HardwareMode(3, cbADC24CH4_Hardware.SelectedIndex);
            ADC24_Update_GainSetting(3);
            ADC24_Refresh_Window();
        }

        private void cbADC24CH5_Hardware_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ibMouseClicked == false)
                return;
            ADC24_Update_HardwareMode(4, cbADC24CH5_Hardware.SelectedIndex);
            ADC24_Update_GainSetting(4);
            ADC24_Refresh_Window();
        }
        private void cbADC24CH6_Hardware_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ibMouseClicked == false)
                return;
            ADC24_Update_HardwareMode(5, cbADC24CH6_Hardware.SelectedIndex);
            ADC24_Update_GainSetting(5);
            ADC24_Refresh_Window();
        }
        #endregion

        #region //=============================================================================================cbADC24CHX_Gain_SelectedIndexChanged
        private void cbADC24CH1_Gain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ADC24_Update_GainSetting(0);
            ADC24_Refresh_Window();
        }
        private void cbADC24CH2_Gain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ADC24_Update_GainSetting(1);
            ADC24_Refresh_Window();
        }

        private void cbADC24CH3_Gain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ADC24_Update_GainSetting(2);
            ADC24_Refresh_Window();
        }

        private void cbADC24CH4_Gain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ADC24_Update_GainSetting(3);
            ADC24_Refresh_Window();
        }

        private void cbADC24CH5_Gain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ADC24_Update_GainSetting(4);
            ADC24_Refresh_Window();
        }

        private void cbADC24CH6_Gain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ADC24_Update_GainSetting(5);
            ADC24_Refresh_Window();
        }
        #endregion

        #region //=============================================================================================ADC24_Update_Format
        public void ADC24_Update_Format(int channel)
        {
            switch (channel)
            {
                case (0):       //CH1
                    {
                        if (cbADC24CH1_Format.SelectedIndex > 9)        // Limit Range. 
                            cbADC24CH1_Format.SelectedIndex = 8;
                        ADC24Ch[0].uReadoutFormat = cbADC24CH1_Format.SelectedIndex;
                        //cbADC24CH1_Format.Refresh();
                        break;
                    }
                case (1):       //CH2
                    {
                        if (cbADC24CH2_Format.SelectedIndex > 9)        // Limit Range. 
                            cbADC24CH2_Format.SelectedIndex = 8;
                        ADC24Ch[1].uReadoutFormat = cbADC24CH2_Format.SelectedIndex;
                        //cbADC24CH2_Format.Refresh();
                        break;
                    }
                case (2):       //CH3
                    {
                        if (cbADC24CH3_Format.SelectedIndex > 9)        // Limit Range. 
                            cbADC24CH3_Format.SelectedIndex = 8;
                        ADC24Ch[2].uReadoutFormat = cbADC24CH3_Format.SelectedIndex;
                        //cbADC24CH3_Format.Refresh();
                        break;
                    }
                case (3):       //CH4
                    {
                        if (cbADC24CH4_Format.SelectedIndex > 9)        // Limit Range. 
                            cbADC24CH4_Format.SelectedIndex = 8;
                        ADC24Ch[3].uReadoutFormat = cbADC24CH4_Format.SelectedIndex;
                        //cbADC24CH4_Format.Refresh();
                        break;
                    }
                case (4):       //CH5
                    {
                        if (cbADC24CH5_Format.SelectedIndex > 9)        // Limit Range. 
                            cbADC24CH5_Format.SelectedIndex = 8;
                        ADC24Ch[4].uReadoutFormat = cbADC24CH5_Format.SelectedIndex;
                        //cbADC24CH5_Format.Refresh();
                        break;
                    }
                case (5):       //Ch6
                    {
                        if (cbADC24CH6_Format.SelectedIndex > 9)        // Limit Range. 
                            cbADC24CH6_Format.SelectedIndex = 8;
                        ADC24Ch[5].uReadoutFormat = cbADC24CH6_Format.SelectedIndex;
                        //cbADC24CH6_Format.Refresh();
                        break;
                    }
                default:
                    break;
            }
            //ADC24_Refresh_Format(channel);
        }
        #endregion

        #region //=============================================================================================ADC24_Update_BufferMode
        public void ADC24_Update_BufferMode(int channel)
        {
            switch (channel)
            {
                case (0):       //CH1
                    {
                        ADC24Ch[0].bBufferMode = cbADC24CH1_Buffer.Checked;
                        //cbADC24CH1_Buffer.Refresh();
                        break;
                    }
                case (1):       //CH2
                    {
                        ADC24Ch[1].bBufferMode = cbADC24CH2_Buffer.Checked;
                        //cbADC24CH2_Buffer.Refresh();
                        break;
                    }
                case (2):       //CH3
                    {
                        ADC24Ch[2].bBufferMode = cbADC24CH3_Buffer.Checked;
                        //cbADC24CH3_Buffer.Refresh();
                        break;
                    }
                case (3):       //CH4
                    {
                        ADC24Ch[3].bBufferMode = cbADC24CH4_Buffer.Checked;
                        //cbADC24CH4_Buffer.Refresh();
                        break;
                    }
                case (4):       //CH5
                    {
                        ADC24Ch[4].bBufferMode = cbADC24CH5_Buffer.Checked;
                        //cbADC24CH5_Buffer.Refresh();
                        break;
                    }
                case (5):       //Ch6
                    {
                        ADC24Ch[5].bBufferMode = cbADC24CH6_Buffer.Checked;
                        //cbADC24CH6_Buffer.Refresh();
                        break;
                    }
                default:
                    break;
            }
            ADC24_Refresh_BufferMode(channel);
        }
        #endregion

        #region //=============================================================================================ADC24_Update_GainSetting
        public void ADC24_Update_GainSetting(int channel)
        {
            switch (channel)
            {
                case (0):       //CH1
                    {
                        if (ADC24Ch[0].iHardwareMode == 5)
                        {
                            ADC24Ch[0].uGain = cbADC24CH1_Gain.SelectedIndex;
                            cbADC24CH1_Gain.Visible = true;
                            lbADCGainCH1.Visible = true;
                            lbADCGainFixedCH1.Visible = false;
                            //cbADC24CH1_Gain.Refresh();
                        }
                        else
                        {
                            ADC24Ch[0].uGain = 0;               // Default 1V/V
                            cbADC24CH1_Gain.Visible = false;
                            lbADCGainCH1.Visible = false;
                            lbADCGainFixedCH1.Visible = true;
                            //cbADC24CH1_Gain.Refresh();
                        }
                        break;
                    }
                case (1):       //CH2
                    {
                        if (ADC24Ch[1].iHardwareMode == 5)
                        {
                            ADC24Ch[1].uGain = cbADC24CH2_Gain.SelectedIndex;
                            cbADC24CH2_Gain.Visible = true;
                            lbADCGainCH2.Visible = true;
                            lbADCGainFixedCH2.Visible = false;
                            //cbADC24CH2_Gain.Refresh();
                        }
                        else
                        {
                            ADC24Ch[1].uGain = 0;               // Default 1V/V
                            cbADC24CH2_Gain.Visible = false;
                            lbADCGainCH2.Visible = false;
                            lbADCGainFixedCH2.Visible = true;
                            //cbADC24CH2_Gain.Refresh();
                        }
                        break;
                    }
                case (2):       //CH3
                    {
                        if (ADC24Ch[2].iHardwareMode == 5)
                        {
                            ADC24Ch[2].uGain = cbADC24CH3_Gain.SelectedIndex;
                            cbADC24CH3_Gain.Visible = true;
                            lbADCGainCH3.Visible = true;
                            lbADCGainFixedCH3.Visible = false;
                            // cbADC24CH3_Gain.Refresh();
                        }
                        else
                        {
                            ADC24Ch[2].uGain = 0;               // Default 1V/V
                            cbADC24CH3_Gain.Visible = false;
                            lbADCGainCH3.Visible = false;
                            lbADCGainFixedCH3.Visible = true;
                            //cbADC24CH3_Gain.Refresh();
                        }
                        break;
                    }
                case (3):       //CH4
                    {
                        if (ADC24Ch[3].iHardwareMode == 5)
                        {
                            ADC24Ch[3].uGain = cbADC24CH4_Gain.SelectedIndex;
                            cbADC24CH4_Gain.Visible = true;
                            lbADCGainCH4.Visible = true;
                            lbADCGainFixedCH4.Visible = false;
                            //cbADC24CH4_Gain.Refresh();
                        }
                        else
                        {
                            ADC24Ch[3].uGain = 0;               // Default 1V/V
                            cbADC24CH4_Gain.Visible = false;
                            lbADCGainCH4.Visible = false;
                            lbADCGainFixedCH4.Visible = true;
                            //cbADC24CH4_Gain.Refresh();
                        }

                        break;
                    }
                case (4):       //CH5
                    {
                        if (ADC24Ch[4].iHardwareMode == 5)
                        {
                            ADC24Ch[4].uGain = cbADC24CH5_Gain.SelectedIndex;
                            cbADC24CH5_Gain.Visible = true;
                            lbADCGainCH5.Visible = true;
                            lbADCGainFixedCH5.Visible = false;
                            //cbADC24CH5_Gain.Refresh();
                        }
                        else
                        {
                            ADC24Ch[4].uGain = 0;               // Default 1V/V
                            cbADC24CH5_Gain.Visible = false;
                            lbADCGainCH5.Visible = false;
                            lbADCGainFixedCH5.Visible = true;
                            // cbADC24CH5_Gain.Refresh();
                        }
                        break;
                    }
                case (5):       //Ch6
                    {
                        if (ADC24Ch[5].iHardwareMode == 5)
                        {
                            ADC24Ch[5].uGain = cbADC24CH6_Gain.SelectedIndex;
                            cbADC24CH6_Gain.Visible = true;
                            lbADCGainCH6.Visible = true;
                            lbADCGainFixedCH6.Visible = false;
                            //cbADC24CH6_Gain.Refresh();
                        }
                        else
                        {
                            ADC24Ch[5].uGain = 0;               // Default 1V/V
                            cbADC24CH6_Gain.Visible = false;
                            lbADCGainCH6.Visible = false;
                            lbADCGainFixedCH6.Visible = true;
                            //cbADC24CH6_Gain.Refresh();
                        }
                        break;
                    }
                default:
                    break;
            }
            //ADC24_Refresh_GainSetting(channel);
        }
        #endregion

        #region //=============================================================================================ADC24_Update_HardwareMode
        public void ADC24_Update_HardwareMode(int channel,int cbindex)
        {
            switch (channel)
            {
                case (0):       //CH1
                    {
                        if (ADC24Ch[0].iHardwareMode != cbindex)    // Change Hardware Mode?
                        {
                            cbADC24CH1_Format.SelectedIndex = 0;                                  // Reset back to default setting. 
                            if (cbADC24CH1_Format.SelectedIndex > 8)        // Limit Range. 
                                cbADC24CH1_Format.SelectedIndex = 7;
                            ADC24Ch[0].uReadoutFormat = cbADC24CH1_Format.SelectedIndex;
                        }
                        ADC24Ch[0].iHardwareMode = cbindex;
                        break;
                    }
                case (1):
                    {
                        if (ADC24Ch[1].iHardwareMode != cbindex)    // Change Hardware Mode?
                        {
                            cbADC24CH2_Format.SelectedIndex = 0;                            // Reset back to default setting. 
                            if (cbADC24CH2_Format.SelectedIndex > 8)        // Limit Range. 
                                cbADC24CH2_Format.SelectedIndex = 7;
                            ADC24Ch[1].uReadoutFormat = cbADC24CH2_Format.SelectedIndex;
                        }
                        ADC24Ch[1].iHardwareMode = cbindex;
                        break;
                    }
                case (2):
                    {
                        if (ADC24Ch[2].iHardwareMode != cbindex)    // Change Hardware Mode?
                        {
                            cbADC24CH3_Format.SelectedIndex = 0;                             // Reset back to default setting. 
                            if (cbADC24CH3_Format.SelectedIndex > 8)        // Limit Range. 
                                cbADC24CH3_Format.SelectedIndex = 7;
                            ADC24Ch[2].uReadoutFormat = cbADC24CH3_Format.SelectedIndex;
                        }
                        ADC24Ch[2].iHardwareMode = cbindex;
                        break;
                    }
                case (3):
                    {
                        if (ADC24Ch[3].iHardwareMode != cbindex)    // Change Hardware Mode?
                        {
                            cbADC24CH4_Format.SelectedIndex = 0;                              // Reset back to default setting. 
                            if (cbADC24CH4_Format.SelectedIndex > 8)        // Limit Range. 
                                cbADC24CH4_Format.SelectedIndex = 7;
                            ADC24Ch[3].uReadoutFormat = cbADC24CH4_Format.SelectedIndex;
                        }
                        ADC24Ch[3].iHardwareMode = cbindex;
                        break;
                    }
                case (4):
                    {
                        if (ADC24Ch[4].iHardwareMode != cbindex)    // Change Hardware Mode?
                        {
                            cbADC24CH5_Format.SelectedIndex = 0;                             // Reset back to default setting. 
                            if (cbADC24CH5_Format.SelectedIndex > 8)        // Limit Range. 
                                cbADC24CH5_Format.SelectedIndex = 7;
                            ADC24Ch[4].uReadoutFormat = cbADC24CH5_Format.SelectedIndex;
                        }
                        ADC24Ch[4].iHardwareMode = cbindex;
                        break;
                    }
                case (5): //Ch6
                    {
                        if (ADC24Ch[5].iHardwareMode != cbindex)    // Change Hardware Mode?
                        {
                            cbADC24CH6_Format.SelectedIndex = 0;                             // Reset back to default setting. 
                            if (cbADC24CH6_Format.SelectedIndex > 8)        // Limit Range. 
                                cbADC24CH6_Format.SelectedIndex = 7;
                            ADC24Ch[5].uReadoutFormat = cbADC24CH6_Format.SelectedIndex;
                        }
                        ADC24Ch[5].iHardwareMode = cbindex;
                        break;
                    }
                default:
                    break;
            }
            //ADC24_Refresh_HardwareMode(channel);
        }
        #endregion

        #region //=============================================================================================ADC24_Refresh_Format
        public void ADC24_Refresh_Format(int channel)
        {
            switch (channel)
            {
                case (0):       //CH1
                    {
                        if (cbADC24CH1_Format.Items.Count == 0)
                            return;
                        if (ADC24Ch[0].uReadoutFormat <= 9)
                        {
                            cbADC24CH1_Format.SelectedIndex = ADC24Ch[0].uReadoutFormat;
                            cbADC24CH1_Format.Refresh();
                        }
                        break;
                    }
                case (1):       //CH2
                    {
                        if (cbADC24CH2_Format.Items.Count == 0)
                            return;
                        if (ADC24Ch[1].uReadoutFormat <= 9)
                        {
                            cbADC24CH2_Format.SelectedIndex = ADC24Ch[1].uReadoutFormat;
                            cbADC24CH2_Format.Refresh();
                        }
                        break;
                    }
                case (2):       //CH3
                    {
                        if (cbADC24CH3_Format.Items.Count == 0)
                            return;
                        if (ADC24Ch[2].uReadoutFormat <= 9)
                        {
                            cbADC24CH3_Format.SelectedIndex = ADC24Ch[2].uReadoutFormat;
                            cbADC24CH3_Format.Refresh();
                        }
                        break;
                    }
                case (3):       //CH4
                    {
                        if (cbADC24CH4_Format.Items.Count == 0)
                            return;
                        if (ADC24Ch[3].uReadoutFormat <= 9)
                        {
                            cbADC24CH4_Format.SelectedIndex = ADC24Ch[3].uReadoutFormat;
                            cbADC24CH4_Format.Refresh();
                        }
                        break;
                    }
                case (4):       //CH5
                    {
                        if (cbADC24CH5_Format.Items.Count == 0)
                            return;
                        if (ADC24Ch[4].uReadoutFormat <= 9)
                        {
                            cbADC24CH5_Format.SelectedIndex = ADC24Ch[4].uReadoutFormat;
                            cbADC24CH5_Format.Refresh();
                        }
                        break;
                    }
                case (5):       //Ch6
                    {
                        if (cbADC24CH6_Format.Items.Count == 0)
                            return;
                        if (ADC24Ch[5].uReadoutFormat <= 9)
                        {
                            cbADC24CH6_Format.SelectedIndex = ADC24Ch[5].uReadoutFormat;
                            cbADC24CH6_Format.Refresh();
                        }
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region //=============================================================================================ADC24_Refresh_BufferMode
        public void ADC24_Refresh_BufferMode(int channel)
        {
            switch (channel)
            {
                case (0):       //CH1
                    {
                        cbADC24CH1_Buffer.Checked = ADC24Ch[0].bBufferMode;
                        cbADC24CH1_Buffer.Refresh();
                        break;
                    }
                case (1):       //CH2
                    {
                        cbADC24CH2_Buffer.Checked = ADC24Ch[1].bBufferMode;
                        cbADC24CH2_Buffer.Refresh();
                        break;
                    }
                case (2):       //CH3
                    {
                        cbADC24CH3_Buffer.Checked = ADC24Ch[2].bBufferMode;
                        cbADC24CH3_Buffer.Refresh();
                        break;
                    }
                case (3):       //CH4
                    {
                        cbADC24CH4_Buffer.Checked = ADC24Ch[3].bBufferMode;
                        cbADC24CH4_Buffer.Refresh();
                        break;
                    }
                case (4):       //CH5
                    {
                        cbADC24CH5_Buffer.Checked = ADC24Ch[4].bBufferMode;
                        cbADC24CH5_Buffer.Refresh();
                        break;
                    }
                case (5):       //Ch6
                    {
                        cbADC24CH6_Buffer.Checked = ADC24Ch[5].bBufferMode;
                        cbADC24CH6_Buffer.Refresh();
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region //=============================================================================================ADC24_Refresh_GainSetting
        public void ADC24_Refresh_GainSetting(int channel)
        {
            switch (channel)
            {
                case (0):       //CH1
                    {
                        if (cbADC24CH1_Gain.Items.Count == 0)
                            return;
                        cbADC24CH1_Gain.SelectedIndex = ADC24Ch[0].uGain;
                        cbADC24CH1_Gain.Refresh();
                        break;
                    }
                case (1):       //CH2
                    {
                        if (cbADC24CH2_Gain.Items.Count == 0)
                            return;
                        cbADC24CH2_Gain.SelectedIndex = ADC24Ch[1].uGain;
                        cbADC24CH2_Gain.Refresh();
                        break;
                    }
                case (2):       //CH3
                    {
                        if (cbADC24CH3_Gain.Items.Count == 0)
                            return;
                        cbADC24CH3_Gain.SelectedIndex = ADC24Ch[2].uGain;
                        cbADC24CH3_Gain.Refresh();
                        break;
                    }
                case (3):       //CH4
                    {
                        if (cbADC24CH4_Gain.Items.Count == 0)
                            return;
                        cbADC24CH4_Gain.SelectedIndex = ADC24Ch[3].uGain;
                        cbADC24CH4_Gain.Refresh();
                        break;
                    }
                case (4):       //CH5
                    {
                        if (cbADC24CH5_Gain.Items.Count == 0)
                            return;
                        cbADC24CH5_Gain.SelectedIndex = ADC24Ch[4].uGain;
                        cbADC24CH5_Gain.Refresh();
                        break;
                    }
                case (5):       //Ch6
                    {
                        if (cbADC24CH6_Gain.Items.Count == 0)
                            return;
                        cbADC24CH6_Gain.SelectedIndex = ADC24Ch[5].uGain;
                        cbADC24CH6_Gain.Refresh();
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region //=============================================================================================ADC24_Refresh_HardwareMode
        public void ADC24_Refresh_HardwareMode(int channel)
        {
            switch (channel)
            {
                case (0):       //CH1
                    {
                        if (cbADC24CH1_Hardware.Items.Count == 0)
                            return;
                        cbADC24CH1_Hardware.SelectedIndex = ADC24Ch[0].iHardwareMode;
                        cbADC24CH1_Hardware.Refresh();
                        pbADC24CH1.Image = ADC24Image[ADC24Ch[0].iHardwareMode];
                        pbADC24CH1.Refresh();
                        break;
                    }
                case (1):       //CH2
                    {
                        if (cbADC24CH2_Hardware.Items.Count == 0)
                            return;
                        cbADC24CH2_Hardware.SelectedIndex = ADC24Ch[1].iHardwareMode;
                        cbADC24CH2_Hardware.Refresh();
                        pbADC24CH2.Image = ADC24Image[ADC24Ch[1].iHardwareMode];
                        pbADC24CH2.Refresh();
                        break;
                    }
                case (2):       //CH3
                    {
                        if (cbADC24CH3_Hardware.Items.Count == 0)
                            return;
                        cbADC24CH3_Hardware.SelectedIndex = ADC24Ch[2].iHardwareMode;
                        cbADC24CH3_Hardware.Refresh();
                        pbADC24CH3.Image = ADC24Image[ADC24Ch[2].iHardwareMode];
                        pbADC24CH3.Refresh();
                        break;
                    }
                case (3):       //CH4
                    {
                        if (cbADC24CH4_Hardware.Items.Count == 0)
                            return;
                        if (ADC24Setup.bRTDMode == true)
                        {
                            pbADC24CH4.Image = ADC24Image[0];
                            tpADC24CH4.Enabled = false;
                            pbADC24CH4.Refresh();
                        }
                        else
                        {
                            tpADC24CH4.Enabled = true;
                            cbADC24CH4_Hardware.SelectedIndex = ADC24Ch[3].iHardwareMode;
                            cbADC24CH4_Hardware.Refresh();
                            pbADC24CH4.Image = ADC24Image[ADC24Ch[3].iHardwareMode];
                            pbADC24CH4.Refresh();
                        }
                        break;
                    }
                case (4):       //CH5
                    {
                        if (cbADC24CH5_Hardware.Items.Count == 0)
                            return;
                        if (ADC24Setup.bRTDMode == true)
                        {
                            pbADC24CH5.Image = ADC24Image[0];
                            tpADC24CH5.Enabled = false;
                            pbADC24CH5.Refresh();
                        }
                        else
                        {
                            tpADC24CH5.Enabled = true;
                            cbADC24CH5_Hardware.SelectedIndex = ADC24Ch[4].iHardwareMode;
                            cbADC24CH5_Hardware.Refresh();
                            pbADC24CH5.Image = ADC24Image[ADC24Ch[4].iHardwareMode];
                            pbADC24CH5.Refresh();
                        }
                        break;
                    }
                case (5):       //Ch6
                    {
                        if (cbADC24CH6_Hardware.Items.Count == 0)
                            return;
                        cbADC24CH6_Hardware.SelectedIndex = ADC24Ch[5].iHardwareMode;
                        cbADC24CH6_Hardware.Refresh();
                        pbADC24CH6.Image = ADC24Image[ADC24Ch[5].iHardwareMode];
                        pbADC24CH6.Refresh();
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region //=============================================================================================ADC24_Refresh_Window
        public void ADC24_Refresh_Window()
        {
            ibMouseClicked = false;                                             // Disable self event control 
            //TabWindowEnable.bAD7794 = false;                                    // Disable window control.
            //ADCSupport_Refresh_Window_Control();
            lbADC4Peroid.Text = ADC24Setup.uPeroid.ToString();
            //----------------------------------------------------------------Setup
            cbADC24CaptureMethod.Checked = ADC24Setup.bCapType;
            cbADC24RTDMode.Checked = ADC24Setup.bRTDMode;
            cbADC24Chopped.Checked = ADC24Setup.bChopped;
            cbADC24Enable.Checked = ADC24Setup.bIsEnable;
            ADC24_Update_SampleSpeed();
            if (ADC24Setup.uPeroid == 0)      // 0 = SYNC >1 ASYNC
                cbLoggerCVSSync.Checked = true;
            else
                cbLoggerCVSSync.Checked = false;
            //----------------------------------------------------------------Channel
            for (int i = 0; i < 6; i++)
            {
                ADC24_Refresh_HardwareMode(i);
                ADC24_Refresh_Format(i);
                ADC24_Refresh_BufferMode(i);
                ADC24_Refresh_GainSetting(i);
            }
            //--------------------------------------------------------------Temp Channel
            cbIntTempEnable.Checked = ADC24Setup.bInternalTemp;
            switch (ADC24TempCh.uReadoutFormat)
            {
                case (13):
                    {
                        ADC24Ch7_mK.Checked = true;
                        ADC24Ch7_mF.Checked = false;
                        ADC24Ch7_mC.Checked = false;
                        break;
                    }
                case (14):
                    {
                        ADC24Ch7_mK.Checked = false;
                        ADC24Ch7_mF.Checked = false;
                        ADC24Ch7_mC.Checked = true;
                        break;
                    }
                case (15):
                    {
                        ADC24Ch7_mK.Checked = false;
                        ADC24Ch7_mF.Checked = true;
                        ADC24Ch7_mC.Checked = false;
                        break;
                    }
                default:
                    break;
            }
            //TabWindowEnable.bAD7794 = true;
            ADCSupport_Refresh_Window_Control();
            SupportDataGrid_Update();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### ADC24 Temperature Section CH7
        //#####################################################################################################

        #region //=============================================================================================ADC24Ch7_Temp_CheckedChanged
        private void ADC24Ch7_Temp_CheckedChanged(object sender, EventArgs e)
        {
            if (ADC24Ch7_mK.Checked == true)
            {
                ADC24Ch7_mK.Checked = true;
                ADC24Ch7_mF.Checked = false;
                ADC24Ch7_mC.Checked = false;
                ADC24TempCh.uReadoutFormat = 13;

            }
            else if (ADC24Ch7_mC.Checked == true)
            {
                ADC24Ch7_mK.Checked = false;
                ADC24Ch7_mF.Checked = false;
                ADC24Ch7_mC.Checked = true;
                ADC24TempCh.uReadoutFormat = 14;

            }
            else if (ADC24Ch7_mF.Checked == true)
            {
                ADC24Ch7_mK.Checked = false;
                ADC24Ch7_mF.Checked = true;
                ADC24Ch7_mC.Checked = false;
                ADC24TempCh.uReadoutFormat = 15;

            }
            SupportDataGrid_Update();
        }
        #endregion

        #region //=============================================================================================cbIntTempEnable_CheckedChanged
        private void cbIntTempEnable_CheckedChanged(object sender, EventArgs e)
        {
            ADC24Setup.bInternalTemp = cbIntTempEnable.Checked;
            ADC24_Refresh_Window();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### ADC24 Setup Section
        //#####################################################################################################

        #region //=============================================================================================ADC24_Update_SampleSpeed
        private void ADC24_Update_SampleSpeed()
        {
            rbADC24Max.Checked = false;
            rbADC24Fast.Checked = false;
            rbADC24Medium.Checked = false;
            rbADC24Slow.Checked = false;
            switch (ADC24Setup.uSpeed)
            {
                case (3):
                    rbADC24Max.Checked = true;
                    break;
                case (2):
                    rbADC24Fast.Checked = true;
                    break;
                case (1):
                    rbADC24Medium.Checked = true;
                    break;
                case (0):
                    rbADC24Slow.Checked = true;
                    break;
                default:
                    break;
            }

        }
        #endregion

        #region //=============================================================================================rbADC24SampleSpeed_CheckedChanged
        private void rbADC24SampleSpeed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbADC24Max.Checked == true)
            {
                rbADC24Fast.Checked = false;
                rbADC24Medium.Checked = false;
                rbADC24Slow.Checked = false;
                ADC24Setup.uSpeed = 3;

            }
            else if (rbADC24Fast.Checked == true)
            {
                rbADC24Max.Checked = false;
                rbADC24Medium.Checked = false;
                rbADC24Slow.Checked = false;
                ADC24Setup.uSpeed = 2;

            }
            else if (rbADC24Medium.Checked == true)
            {
                rbADC24Max.Checked = false;
                rbADC24Fast.Checked = false;
                rbADC24Slow.Checked = false;
                ADC24Setup.uSpeed = 1;

            }
            else if (rbADC24Slow.Checked == true)
            {
                rbADC24Max.Checked = false;
                rbADC24Fast.Checked = false;
                rbADC24Medium.Checked = false;
                ADC24Setup.uSpeed = 0;
            }
        }
        #endregion

        #region //=============================================================================================cbADC24RTDMode_CheckedChanged
        private void cbADC24RTDMode_CheckedChanged(object sender, EventArgs e)
        {
            ADC24Setup.bRTDMode = cbADC24RTDMode.Checked;
            ADC24_Refresh_Window();
        }
        #endregion

        #region //=============================================================================================cbADC24CaptureMethod_CheckedChanged
        private void cbADC24CaptureMethod_CheckedChanged(object sender, EventArgs e)
        {
            ADC24Setup.bCapType = cbADC24CaptureMethod.Checked;
            ADC24_Refresh_Window();
        }
        #endregion

        #region //=============================================================================================cbADC24Chopped_CheckedChanged
        private void cbADC24Chopped_CheckedChanged(object sender, EventArgs e)
        {
            ADC24Setup.bChopped = cbADC24Chopped.Checked;
            ADC24_Refresh_Window();
        }
        #endregion

        #region //=============================================================================================cbLoggerCVSSync_CheckedChanged
        private void cbLoggerCVSSync_CheckedChanged(object sender, EventArgs e)
        {
            List<int> SpeedChNoChopped = new List<int>(new int[] { 250, 70, 12, 5 });        //0 = Slow, 1 = Mid, 2 = Fast, 3 = Max
            int chopped = 1;
            if (cbLoggerCVSSync.Checked == true)
                ADC24Setup.uPeroid = 0;         // 0 = Sync Mode.
            else
            {
                if (ADC24Setup.bChopped == true)
                    chopped = 2;
                ADC24Setup.uPeroid = SpeedChNoChopped[ADC24Setup.uSpeed] * ADC24_Count_Active_Channels() * chopped;      // Default 1 Second ASYNC
            }

            ADC24_Refresh_Window();
        }
        #endregion

        #region //=============================================================================================ADC24_Count_Active_Channels
        private int ADC24_Count_Active_Channels()
        {
            int ActiveChannel = 0;
            for (int i = 0; i < 6; i++)
            {
                if (ADC24Ch[i].iHardwareMode != 0)
                    ActiveChannel++;
            }
            if (ADC24Setup.bInternalTemp == true)
                ActiveChannel++;
            return (ActiveChannel);
        }
        #endregion

        #region //=============================================================================================cbADC24CH6_Buffer_MouseClick   
        private void cbADC24CH6_Buffer_MouseClick(object sender, MouseEventArgs e)
        {
            ADC24Ch[ttcADC24Channel.SelectedIndex].bBufferMode = cbADC24CH6_Buffer.Checked;
            ADC24_Refresh_Window();
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### ADC24 Tool Tips
        //#####################################################################################################

        #region //=============================================================================================toolTipBufferMode_Popup    
        private void toolTipBufferMode_Popup(object sender, PopupEventArgs e)
        {
            if (toolTipBufferMode.ToolTipTitle == "ADC24Buf")
            {
                FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                DialogResult response =
                   FlexiMessageBox.Show("Buffer Mode is part of AFE within AD7794 which is enabled when checked"
                + Environment.NewLine
                + Environment.NewLine + "It is used to limits loading against high impedance circuit (say >10K Ohms)"
                + Environment.NewLine + "otherwise significant measurement error may occurred."
                + Environment.NewLine
                + Environment.NewLine + "While in buffer mode, the bias current is reduced to +/-1nA."
                + Environment.NewLine + "NB: Common Mode Range is reduced to less than 100mV from the rails."
                + Environment.NewLine + "    In other word, the buffer when activated is not true rail to rail input."
                + Environment.NewLine
                + Environment.NewLine + "While in unbuffered mode, the bias current can be up to +/-400nA during capture"
                + Environment.NewLine + "operation. This is transient error is limited by external 1K and 1nF at the ADC I/P."
                + Environment.NewLine + "NB: The Common Mode Range was improved by 30mV from the rails (ie 30mV to 4970mV)"
                + Environment.NewLine
                + Environment.NewLine + "When Gain setting is 4V/V or higher, the buffer mode is internally activated."
                + Environment.NewLine + "Regardless of Buffer Mode, it has no impact on voltage offset and drift."
                + Environment.NewLine + "For more details refer to Page 31 of the AD7794 Datasheet.",
                "Tool Tips: AD7794 Chopped Mode",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
        }
        #endregion

        #region //=============================================================================================toolTip2Chopped_Popup  
        private void toolTip2Chopped_Popup(object sender, PopupEventArgs e)
        {
            if (toolTip2Chopped.ToolTipTitle == "ADC24Chop")
            {
                FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                DialogResult response =
                   FlexiMessageBox.Show("Chopped is part of AFE within AD7794 which is enabled when checked"
                + Environment.NewLine
                + Environment.NewLine + "It is used to reduce DC offset and drift and thus improve measurement accuracy"
                + Environment.NewLine + "This setting applied to all channels (ie this is global setting)"
                + Environment.NewLine
                + Environment.NewLine + "While in Chopped Mode.The voltage offset drift:"
                + Environment.NewLine + "   Offset Error     = 1uV(typ)          Drift:10nV/C (typ)"
                + Environment.NewLine + "   Full Scale Error = 10uV(typ)"
                + Environment.NewLine
                + Environment.NewLine + "Without Chopped Mode. The voltage offset drift is worst and gain dependents:"
                + Environment.NewLine + "   Offset Error     = 100u/Gain uV(typ) Drift:100/Gain nV/C(typ)"
                + Environment.NewLine + "   Full Scale Error = 10uV(typ)"
                + Environment.NewLine
                + Environment.NewLine + "NB: There is minor change in Effective Resolution Bits (due to ADC's noise)."
                + Environment.NewLine + "Regardless of Chopping setting, the AMP-CM=0 for maximum common mode range"
                + Environment.NewLine + "at expense of PSU ripple rejection. For more details refer to AD7794 Datasheet.",
                "Tool Tips: AD7794 Buffer Mode",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
        }
        #endregion

        #region //=============================================================================================toolTipSampleRate_Popup
        private void toolTipSampleRate_Popup(object sender, PopupEventArgs e)
        {
            if (toolTipSampleRate.ToolTipTitle == "ADC24SamRate")
            {
                FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                DialogResult response =
                   FlexiMessageBox.Show("The application supports 4 sample rate mode which has significant impacts on "
                + Environment.NewLine + "measurement accuracy versa real time data captures between multiplex inputs."
                + Environment.NewLine
                + Environment.NewLine + "AD7794 feature 6 channels input (plus 7th channel for temperature) which is"
                + Environment.NewLine + "multiplexed to single ADC module. It capture diff. input voltage in sequence"
                + Environment.NewLine + "according to activate channel setting (see hardware mode list box & Image)."
                + Environment.NewLine
                + Environment.NewLine + "Unlike SAR type ADC (which is fast), this is Sigma-Delta ADC which has slower "
                + Environment.NewLine + "settling time and capture period. It requires input waveform to be stable "
                + Environment.NewLine + "during capture process, since it lack Sample and Hold feature (as in SAR)."
                + Environment.NewLine + "For more details about SAR and Sigma-Delta ADC should refer to Wiki/etc."
                + Environment.NewLine
                + Environment.NewLine + "The following basic specification for different sample setting speed are"
                + Environment.NewLine + "----------------------------------------------------------------------"
                + Environment.NewLine + "Option     Rate        Sample Time*    50/60Hz Rejection   EOB**"
                + Environment.NewLine + "----------------------------------------------------------------------"
                + Environment.NewLine + "Slow       4.17Hz      245  mSec/Ch    74dB                23"
                + Environment.NewLine + "Mid        16.7Hz       63  mSec/Ch    65dB                21.5"
                + Environment.NewLine + "Fast       123Hz         9.8mSec/Ch    n/a                 20"
                + Environment.NewLine + "Max        470Hz         3.7mSec/Ch    n/a                 18.5"
                + Environment.NewLine + "----------------------------------------------------------------------"
                + Environment.NewLine + "*  This is Non-Chopped Mode, In Chopped Mode, it double the Sample Time."
                + Environment.NewLine + "** EOB = Effective Resolution Bit for Gain=1V/V. The EOB will degrades with"
                + Environment.NewLine + "   with increased Gain setting, refer to page 13/14 of AD7794 Datasheet."
                ,
                "Tool Tips: AD7794 Sample Rate Setting",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }

        }
        #endregion

        #region //=============================================================================================toolTipRTDMode_Popup
        private void toolTipRTDMode_Popup(object sender, PopupEventArgs e)
        {
            if (toolTipRTDMode.ToolTipTitle == "ADC24RTDMode")
            {
                FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                DialogResult response =
                   FlexiMessageBox.Show("When enabled, activated the Channel 4/5 into RTD Mode for temperature measurement"
                + Environment.NewLine + "with PTD100 or PTD1000 device. It provide accurate temperature measurement."
                + Environment.NewLine + "At the time of writing, it remains under development.Please leave unchecked.",
                "Tool Tips: AD7794 RTD Temperature Mode",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }

        }
        #endregion

        #region //=============================================================================================toolTipCaptureMethod_Popup 
        private void toolTipCaptureMethod_Popup(object sender, PopupEventArgs e)
        {
            if (toolTipCaptureMethod.ToolTipTitle == "ADC24CapMode")
            {
                FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                DialogResult response =
                   FlexiMessageBox.Show("The firmware in AD7794 operate as Continuous Capture Mode when checked (default)"
                + Environment.NewLine
                + Environment.NewLine + "The firmware supports Continuous Capture and Single Read Mode (see datasheet)"
                + Environment.NewLine + "Continuous Capture allows discrete channel to have own setting including "
                + Environment.NewLine + "GAIN, Buffer and Readout format (Bipolar or Unipolar setting). This setting"
                + Environment.NewLine + "is optional for advance application and recommended to leave it checked.",
                "Tool Tips: AD7794 Capture Method",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### AD7794 Function ON or OFF
        //#####################################################################################################

        #region //=============================================================================================cbADC24Enable_MouseClick & ADC24ONOFF()   
        private void cbADC24Enable_MouseClick(object sender, MouseEventArgs e)
        {
            TabWindowEnable.bADCSupport = cbADC24Enable.Checked;
            myGlobalBase.bTabWindowEnableUpdate = true;
            if (TabWindowEnable.bADCSupport == true)
            {
                TabWindowEnable.bAD7794 = true;
                TabWindowEnable.bADCSupport = true;
                ADC24Setup.bIsEnable = true;
                ADC24_Refresh_Window();
            }
            else
            {
                TabWindowEnable.bAD7794 = false;
                TabWindowEnable.bADCSupport = false;
                ADC24Setup.bIsEnable = false;
                ADCSupport_Refresh_Window_Control();
            }
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            ADC24_Refresh_Window();
            //###TASK issue command to turn on/off the AD7794 device.
            ADC24_ONOFF(TabWindowEnable.bAD7794);
        }
        #endregion

        #region //=============================================================================================ADC24_ONOFF()   
        public DMFP_BulkFrame ADC24_ONOFF(bool bOnOff)       // 1 = ON, 0 = OFF
        {
            //-----------------------------------Setup call-back delegate. 
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();
            //-----------------------------------Place Message to rtbTerm Window.
            string command = "";
            if (bOnOff == true)
                command = "+ADC24ONOFF(1)";
            else
                command = "+ADC24ONOFF(0)";
            //if (myGlobalBase.bNoRFTermMessage == false)
            //    myMainProg.myRtbTermMessageLF("#DMFP:" + command);
            cDMFP_BulkFrame.AddItem(command, new DMFP_Delegate(DMFP_ADC24_ADC24ONOFF_CallBack));
            //---------------------------------------------------------------------Send Bulk Frame
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 10;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);
            return cDMFP_BulkFrame;
        }
        #endregion

        #region //=============================================================================================DMFP_ADC24_ADC24ONOFF_CallBack  
        public void DMFP_ADC24_ADC24ONOFF_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            UInt32 Status = hPara[0];
            lbNotFitted.Visible = false;
            if (Status == 0xF000)                                   // Not Fitted AD7794 Device.
            {
                lbNotFitted.Visible = true;
                TabWindowEnable.bAD7794 = false;                    // Alway
                TabWindowEnable.bADCSupport = false;
                ADCSupport_Refresh_Window_Control();
                myMainProg.myRtbTermMessageLF("#E:AD7794 is not fitted or defective.");
            }
                
            if (Status != 0xFFFF)       // No Error
            {
                myMainProg.myRtbTermMessageLF("#E:ADC24ONOFF reported error code:0x" + hPara[0].ToString("X"));
            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### ADC24 Export/Import Setup
        //#####################################################################################################

        #region //=============================================================================================btnADC24ImportSetting_Click
        private void btnADC24ImportSetting_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();
            cDMFP_BulkFrame.AddItem("+ADC24SETUPR()", new DMFP_Delegate(DMFP_ADC24SETUPR_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24CHR(0x01)", new DMFP_Delegate(DMFP_ADC24CHRX_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24CHR(0x02)", new DMFP_Delegate(DMFP_ADC24CHRX_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24CHR(0x03)", new DMFP_Delegate(DMFP_ADC24CHRX_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24CHR(0x04)", new DMFP_Delegate(DMFP_ADC24CHRX_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24CHR(0x05)", new DMFP_Delegate(DMFP_ADC24CHRX_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24CHR(0x06)", new DMFP_Delegate(DMFP_ADC24CHRX_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24TEMPCH7R()", new DMFP_Delegate(DMFP_DC24TEMPCH7R_CallBack));
            //---------------------------------------------------------------------Send Bulk Frame
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 10;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);
            //------------------------------------------------------------------------------------
        }

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
                ADC24_Refresh_Window();
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_ADC24SETUPR_CallBack()...has Error code: 0x" + hPara[0].ToString("X"));
            }
        }
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
                    try
                    {
                        ADC24_Refresh_HardwareMode(channel);
                        ADC24_Refresh_Format(channel);
                        ADC24_Refresh_BufferMode(channel);
                        ADC24_Refresh_GainSetting(channel);
                    }
                    catch { };
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
            ADC24_Refresh_Window();
        }
        #endregion

        #region //=============================================================================================btnADC24ExportSetting_Click
        private void btnADC24ExportSetting_Click(object sender, EventArgs e)
        {
            ADC24_Update_Setup_Data();
        }
        #endregion

        #region //=============================================================================================ADC24_Update_Setup_Data
        public DMFP_BulkFrame ADC24_Update_Setup_Data()
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return (null);
            }
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();
            ADC24Setup.bIsEnable = TabWindowEnable.bAD7794;
            //---------------------------------------------------------------------------------------------------------ADC24 Global Setup 
            //+ADC24SETUP ( CapType; RTD; IntTerm; Speed; Chopped; Period; isEnable)\n	
            string command = "+ADC24SETUP(";
            command += (Tools.BinaryFalseTrue(ADC24Setup.bCapType)).ToString() + ";";
            command += (Tools.BinaryFalseTrue(ADC24Setup.bRTDMode)).ToString() + ";";
            command += (Tools.BinaryFalseTrue(ADC24Setup.bInternalTemp)).ToString() + ";";
            command += (ADC24Setup.uSpeed).ToString() + ";";
            command += (Tools.BinaryFalseTrue(ADC24Setup.bChopped)).ToString() + ";";
            command += "0x" + (ADC24Setup.uPeroid).ToString("X") + ";";
            command += (Tools.BinaryFalseTrue(ADC24Setup.bIsEnable)).ToString() + ")";

            cDMFP_BulkFrame.AddItem(command, new DMFP_Delegate(DMFP_ADC24_ADC24SETUP_CallBack));
            //---------------------------------------------------------------------------------------------------------ADC24 Discrete Channel to update.
            if (ADC24Setup.bIsEnable == true)       // Update channel only if AD7794 is enabled. 
            {
                // +ADC24CH (Channel; H/W_MODE; BUFF; GAIN; FORMAT )\n	
                for (int i = 0; i < 6; i++)
                {
                    command = "+ADC24CH(";
                    command += (ADC24Ch[i].uChannel + 1).ToString() + ";";
                    command += ADC24Ch[i].iHardwareMode.ToString() + ";";
                    command += (Tools.BinaryFalseTrue(ADC24Ch[i].bBufferMode)).ToString() + ";";
                    command += ADC24Ch[i].uGain.ToString() + ";";
                    command += ADC24Ch[i].uReadoutFormat.ToString() + ")";
                    cDMFP_BulkFrame.AddItem(command, null);
                }
            }
            //---------------------------------------------------------------------------------------------------------ADC24 Tem
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

        #region //=============================================================================================DMFP_ADC24_ADC24SETUP_CallBack  
        public void DMFP_ADC24_ADC24SETUP_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            lbNotFitted.Visible = false;
            UInt32 Status = hPara[0];
            if (Status == 0xF000)                                   // Not Fitted AD7794 Device.
            {
                lbNotFitted.Visible = true;
                TabWindowEnable.bAD7794 = false;                    // Alway
                TabWindowEnable.bADCSupport = false;
                ADCSupport_Refresh_Window_Control();
                myMainProg.myRtbTermMessageLF("#E:AD7794 is not fitted or defective.");
            }

            if (Status != 0xFFFF)       // No Error
            {
                myMainProg.myRtbTermMessageLF("#E:ADC24ONOFF reported error code:0x" + hPara[0].ToString("X"));
            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### ADC24 Capture Readout
        //#####################################################################################################

        #region //=============================================================================================btnADC24CaptureNow_Click & CalBacks 
        private void btnADC24CaptureNow_Click(object sender, EventArgs e)
        {
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            //--------------------------------------------------------------Send Setup Data before Capture.
            ADC24_Refresh_Window();
            if (cbADC2CaptureExportSetup.Checked == true)
            {
                DMFP_BulkFrame cDMFP_BulkFrame = ADC24_Update_Setup_Data();
                while (cDMFP_BulkFrame.isBusy == true)
                {
                    Tools.InteractivePause(TimeSpan.FromMilliseconds(25));
                }
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
            lbNotFitted.Visible = false;
            if (Status == 0xF000)                                   // Not Fitted AD7794 Device.
            {
                lbNotFitted.Visible = true;
                TabWindowEnable.bAD7794 = false;                    // Alway
                TabWindowEnable.bADCSupport = false;
                ADCSupport_Refresh_Window_Control();
                myMainProg.myRtbTermMessageLF("#E:AD7794 is not fitted or defective.");
            }
            if (Status != 0xFFFF)       // No Error
            {
                myMainProg.myRtbTermMessageLF("#E:ADC24DATA reported error code:0x" + hPara[0].ToString("X"));
                return;
            }
            //---------------------------------------------------Update Readout.
            for (int i = 0; i < 6; i++)
            {
                ADC24dataGrid.Rows[i].Cells[3].Value = iPara[i];
                //ADC24dataGrid.Rows[i].Cells[1].Value = ADC24Ch[i].iHardwareMode;
                //ADC24dataGrid.Rows[i].Cells[2].Value = ADC24Ch[i].uReadoutFormat;
            }
            if (iPara.Count == 7)
            {
                ADC24dataGrid.Rows[6].Cells[3].Value = iPara[6];
            }

        }

        #endregion


    }


}

