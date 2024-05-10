using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDT_Term_FFT
{
    public partial class ADCSupport : UserControl
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        List<TVMiniPort_ADCPost> myADCPostArray;
        MainProg myMainProg;
        USB_Message_Manager myUSB_Message_Manager;
        TVMiniPort_TabEnable TabWindowEnable;

        public bool isUserCellChange { get; set; }


        #region//================================================================Reference
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }
        public void MyTabWindowEnable(TVMiniPort_TabEnable TabWindowEnableRef)
        {
            TabWindowEnable = TabWindowEnableRef;
        }
        public void MyADCPost(List<TVMiniPort_ADCPost> myADCPostRef)
        {
            myADCPostArray = myADCPostRef;
        }
        public void MyUSB_Message_Manager(USB_Message_Manager myUSB_Message_ManagerRef)
        {
            myUSB_Message_Manager = myUSB_Message_ManagerRef;
        }
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        #endregion

        public ADCSupport()
        {
            InitializeComponent();
        }

        #region//================================================================ADCSupport_Load
        private void ADCSupport_Load(object sender, EventArgs e)
        {
            ADCSupport_Refresh_Window_Control();
            isUserCellChange = true;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Tab Enable Manager
        //#####################################################################################################
        public bool TabWindowEnableImplement()
        {
            if (myADCPostArray == null)     // null ref due to timing between various window. This ensure it synced.
                return (false);

            if ((TabWindowEnable.bAD7794 == true)| (TabWindowEnable.bADCSupport == true))
            {
                TabWindowEnable.bAD7794 = true;                     // Alway
                TabWindowEnable.bADCSupport = true;                 
                ADCSupport_Refresh_Window_Control();
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
        //#####################################################################################################
        //###################################################################################### Window Control ON/OFF
        //#####################################################################################################
        void ADCSupport_Refresh_Window_Control()
        {
            if (TabWindowEnable.bADCSupport==true)
            {
                lbAD7794Disable.Visible = false;
                gbADCPost.Visible = true;
                btnADC24ImportSetup.Visible = true;
                btnADCExportSetup.Visible = true;
                gbUserGuide.Visible = true;
            }
            else
            {
                lbAD7794Disable.Visible = true;
                gbADCPost.Visible = false;
                btnADC24ImportSetup.Visible = false;
                btnADCExportSetup.Visible = false;
                gbUserGuide.Visible = false;
            }
            

        }


        //#####################################################################################################
        //###################################################################################### T
        //#####################################################################################################

        #region//=============================================================================================Setup Column Format
        private System.Windows.Forms.DataGridView ADCPostGrid;
        DataGridViewTextBoxColumn[] ADCPostCmdColumn;
        static SupportDataGrid[] myADCPostLayout =
            new SupportDataGrid[]{
                new SupportDataGrid("Ch",30),
                new SupportDataGrid("Type",70),
                new SupportDataGrid("Offset(uV)",100),
                new SupportDataGrid("Gain (V/V)",100),
                new SupportDataGrid("PostFix",60)
            };
        #endregion

        #region //=============================================================================================SupportDataGridSetup
        public void ADCPostGridSetup()
        {
            ADCPostGrid = new System.Windows.Forms.DataGridView();
            ADCPostGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            ADCPostGrid.RowTemplate.Height = 19;
            ADCPostGrid.RowTemplate.MinimumHeight = 19;
            //--------------------------------------------------------------------
            ADCPostGrid.Location = new System.Drawing.Point(0, 0);
            ADCPostGrid.Size = new System.Drawing.Size(360, 196);
            ADCPostGrid.MaximumSize = new System.Drawing.Size(360, 196);
            ADCPostGrid.MinimumSize = new System.Drawing.Size(360, 196);
            ADCPostGrid.TabIndex = 0;
            ADCPostGrid.AllowUserToAddRows = false;
            ADCPostGrid.AllowUserToDeleteRows = false;
            ADCPostGrid.AllowUserToResizeColumns = false;
            ADCPostGrid.AllowUserToResizeRows = false;
            ADCPostGrid.BackgroundColor = System.Drawing.Color.Gainsboro;
            ADCPostGrid.MultiSelect = false;
            ADCPostGrid.Name = "CmdBox";
            ADCPostGrid.RowHeadersVisible = false;
            ADCPostGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;

            //--------------------------------------------------------------------
            ADCPostCmdColumn = new DataGridViewTextBoxColumn[5];
            for (int i = 0; i < 5; i++)
            {
                ADCPostCmdColumn[i] = new DataGridViewTextBoxColumn();
                ADCPostCmdColumn[i].Name = "txt" + myADCPostLayout[i].Label;
                ADCPostCmdColumn[i].HeaderText = myADCPostLayout[i].Label;
                ADCPostCmdColumn[i].Width = myADCPostLayout[i].Width;
                ADCPostCmdColumn[i].Resizable = DataGridViewTriState.False;
                ADCPostCmdColumn[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                if ((i == 0) | (i == 1))
                    ADCPostCmdColumn[i].ReadOnly = true;
                this.ADCPostGrid.Columns.Add(ADCPostCmdColumn[i]);
            }
            //=========================================Add Row with color for ADC12/ADC24
            for (int i = 0; i < 9; i++)
            {
                this.ADCPostGrid.Rows.Add(new string[] { (i + 1).ToString("D1") });
                if (i < 6)
                {
                    ADCPostGrid.Rows[i].Cells[1].Style.BackColor = Color.LightYellow;
                    ADCPostGrid.Rows[i].Cells[0].Style.BackColor = Color.LightYellow;
                }
                else
                {
                    ADCPostGrid.Rows[i].Cells[1].Style.BackColor = Color.LightGreen;
                    ADCPostGrid.Rows[i].Cells[0].Style.BackColor = Color.LightGreen;
                }
            }
            ADCPostGrid.ColumnHeadersHeightSizeMode =DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ADCPostGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            ADCPostGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            ADCPostGrid.AllowUserToResizeRows = false;
            this.gbADCPost.Controls.Add(this.ADCPostGrid);
            this.ADCPostGrid.Location = new System.Drawing.Point(6, 19);
            //========================================Add Event
            this.ADCPostGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ADCPostGrid_CellValueChanged); // to validate the data in grid to alert user of error.
        }
        #endregion

        #region //=============================================================================================ADCPostGrid_CellValueChanged
        private void ADCPostGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isUserCellChange == false)
                return;
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
                            /*
                            if (result > 4294.967295)       // Out of range.
                            {
                                result = 4294.967295;       // Shown limit range
                                row.Cells[3].Value = result;
                                row.Cells[3].Style.BackColor = Color.LightPink;
                            }
                            else
                            {
                                row.Cells[3].Style.BackColor = Color.White;
                                
                            }
                            */
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
            for (int i = 0; i < 9; i++)
            {
                if (i <= 5)
                {
                    ADCPostGrid.Rows[i].Cells[0].Value = myADCPostArray[i].iCh + 1;      // For ADC24
                }
                else
                {
                    ADCPostGrid.Rows[i].Cells[0].Value = myADCPostArray[i].iCh - 9;   // For ADC12    Ar
                }
                ADCPostGrid.Rows[i].Cells[1].Value = myADCPostArray[i].iType;
                ADCPostGrid.Rows[i].Cells[2].Value = myADCPostArray[i].iOffset;
                if (myADCPostArray[i].uGain == 0)
                {
                    ADCPostGrid.Rows[i].Cells[3].Value = "0=Disabled";
                }
                else
                {
                    ADCPostGrid.Rows[i].Cells[3].Value = ((double)myADCPostArray[i].uGain) / 1000000.0;    //NB: 1,000,000 = 1V/V, 100,000 = 0.1V/V
                }
                ADCPostGrid.Rows[i].Cells[4].Value = myADCPostArray[i].sUnitText;
            }
            ADCPostGrid.Invalidate();
            isUserCellChange = true;
        }
        #endregion

        #region //=============================================================================================ADCPostGrid_ReadUpdate
        public void ADCPostGrid_ReadUpdate()
        {
            isUserCellChange = false;
            ADCPostGrid.Refresh();
            for (int i = 0; i < 9; i++)
            {
                //-----------Offset
                if (Tools.IsString_Numberic_Int32(ADCPostGrid.Rows[i].Cells[2].Value.ToString()) == true)
                    myADCPostArray[i].iOffset = Tools.ConversionStringtoInt32(ADCPostGrid.Rows[i].Cells[2].Value.ToString());
                //-----------Gain.
                if ((ADCPostGrid.Rows[i].Cells[3].Value.ToString() == "0") | (ADCPostGrid.Rows[i].Cells[3].Value.ToString() == "0=Disabled"))
                {
                    myADCPostArray[i].uGain = 0;
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
                        myADCPostArray[i].uGain = Convert.ToUInt32(value); // This is better than cast (Int32)value due to rounding methodology.
                        ADCPostGrid.Rows[i].Cells[3].Value = ((double)myADCPostArray[i].uGain) / 1000000.0;  // Update to show rounding impacts
                    }
                }
                myADCPostArray[i].sUnitText = ADCPostGrid.Rows[i].Cells[4].Value.ToString();
                isUserCellChange = true;
            }
            //###TASK: Report error if number out of range and not INT32 style. 
        }
        #endregion

        #region //=============================================================================================btnADCImportSetup_Click
        private void btnADC24ImportSetup_Click(object sender, EventArgs e)
        {
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();
            cDMFP_BulkFrame.AddItem("+ADC24POSTR(0x01)", new DMFP_Delegate(DMFP_ADC24POSTR_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24POSTR(0x02)", new DMFP_Delegate(DMFP_ADC24POSTR_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24POSTR(0x03)", new DMFP_Delegate(DMFP_ADC24POSTR_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24POSTR(0x04)", new DMFP_Delegate(DMFP_ADC24POSTR_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24POSTR(0x05)", new DMFP_Delegate(DMFP_ADC24POSTR_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC24POSTR(0x06)", new DMFP_Delegate(DMFP_ADC24POSTR_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC12POSTR(0x01)", new DMFP_Delegate(DMFP_ADC12POSTR_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC12POSTR(0x02)", new DMFP_Delegate(DMFP_ADC12POSTR_CallBack));
            cDMFP_BulkFrame.AddItem("+ADC12POSTR(0x03)", new DMFP_Delegate(DMFP_ADC12POSTR_CallBack));
            //---------------------------------------------------------------------Send Bulk Frame
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 10;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);
            //------------------------------------------------------------------------------------
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
                    myADCPostArray[channel - 1].iOffset = iPara[2];
                    myADCPostArray[channel - 1].uGain = Convert.ToUInt32(iPara[3]);
                    myADCPostArray[channel - 1].sUnitText = sPara[4];
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

        #region //=============================================================================================DMFP_ADC12POSTR_CallBack
        public void DMFP_ADC12POSTR_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] == 0xFFFF)
            {
                int channel = iPara[1];
                if ((channel >= 1) & (channel <= 3))
                {
                    myADCPostArray[channel + 5].iOffset = iPara[2];
                    myADCPostArray[channel + 5].uGain = Convert.ToUInt32(iPara[3]);
                    myADCPostArray[channel + 5].sUnitText = sPara[4];
                }
                if (channel == 3)
                    ADCPostGrid_RefreshUpdate();
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_ADC12POSTR_CallBack()...has Error code: 0x" + hPara[0].ToString("X"));
            }
        }
        #endregion

        #region //=============================================================================================btnADCExportSetup_Click
        private void btnADCExportSetup_Click(object sender, EventArgs e)
        {
            ADCPost_ExportSetup_Click();
        }
        #endregion

        #region //=============================================================================================ADCPost_ExportSetup_Click
        public DMFP_BulkFrame ADCPost_ExportSetup_Click()
        {
            if (myADCPostArray == null)
                return (null);
            ADCPostGrid_ReadUpdate();
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();
            string command;
            //---------------------------------------------------------------------------------------------------------ADC24 Discrete Channel to update.
            // +ADC12POST (Channel; OFFSET; GAIN; UnitFormat )\n	ADCPostArray[ch] where ch=0,1..5 (array)
            for (int i = 0; i <= 5; i++)
            {
                command = "+ADC24POST(";
                command += (i + 1).ToString() + ";";                              // Channel 1,2,3,4,5,6
                command += (myADCPostArray[i].iOffset).ToString() + ";";     // Offset
                command += (myADCPostArray[i].uGain).ToString() + ";";       // Gain
                command += "0s" + (myADCPostArray[i].sUnitText);       // UnitText.
                command += ")";
                cDMFP_BulkFrame.AddItem(command, null);
            }
            // +ADC12POST (Channel; OFFSET; GAIN; UnitFormat )\n	ADCPostArray[ch] where ch=6,7,8 (array)
            for (int i = 6; i <= 8; i++)
            {
                command = "+ADC12POST(";
                command += (i - 5).ToString() + ";";                              // Channel 1,2,3
                command += (myADCPostArray[i].iOffset).ToString() + ";";          // Offset
                command += (myADCPostArray[i].uGain).ToString() + ";";            // Gain
                command += "0s" + (myADCPostArray[i].sUnitText);            // UnitText.
                command += ")";
                cDMFP_BulkFrame.AddItem(command, null);
            }
            //---------------------------------------------------------------------Send Bulk Frame
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 10;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);
            //--------------------------------------------------------------------
            return cDMFP_BulkFrame;
        }
        #endregion


    }


}
 
 