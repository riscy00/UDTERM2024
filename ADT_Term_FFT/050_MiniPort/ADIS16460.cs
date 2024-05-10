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
using System.Windows.Documents;

namespace UDT_Term_FFT
{
    public partial class ADIS16460 : UserControl
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        USB_Message_Manager myUSB_Message_Manager;
        MainProg myMainProg;
        DMFProtocol myDMFProtocol;
        TVMiniPort_ADIS16460Setup ADIS16460Setup;
        TVMiniPort_ADIS16460DataMath ADIS16460Data;
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
        public void MyADIS16460Setup(TVMiniPort_ADIS16460Setup ADIS16460SetupRef)
        {
            ADIS16460Setup = ADIS16460SetupRef;
        }
        public void MyADIS16460DataMath(TVMiniPort_ADIS16460DataMath ADIS16460DataRef)
        {
            ADIS16460Data = ADIS16460DataRef;
        }
        public void MyDMFProtocol(DMFProtocol myDMFProtocolRef)
        {
            myDMFProtocol = myDMFProtocolRef;
        }
        #endregion

        //------------------------------------------------------------------------Local Variable
        public ADIS16460()
        {
            InitializeComponent();
        }

        #region//================================================================ADIS16460_Load
        private void ADIS16460_Load(object sender, EventArgs e)
        {
            btnADISCaptureNow.Enabled = true;
            lbEnableText.Visible = true;
            ADISReadoutGridSetup();
            ADIS16460_Refresh_Window_Write();
            chADISEnable.Checked = false;
            gbSetup.Visible = false;
            gbADISData.Visible = false;
            gpBasicMath.Visible = false;
            btnADISCaptureNow.Visible = false;
            cbADISExportSetup.Visible = false;
            btnADISImportSetting.Visible = false;
            btnADISExportSetting.Visible = false;
            btnRun.Visible = false;
            btnStop.Visible = false;
            gbConnectError.Visible = false;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Tab Enable Manager
        //#####################################################################################################

        #region //=============================================================================================TabWindowEnableImplement 
        public bool TabWindowEnableImplement()  
        {
            if (myDMFProtocol.ValidateRef_MyADIS16460() == false)     // null ref due to timing between various window. This ensure it synced.
                myDMFProtocol.MyMiniADIS1640(this);

            if (TabWindowEnable.bADIS16460 == true)
            {
                cbADISExportSetup.Checked = true;                   // Need export to update.
                chADISEnable.Checked = true;
                ADIS16460_Refresh_Window_Control(false);
                return true;
            }
            else
            {
                cbADISExportSetup.Checked = true;                   // Need export to update.
                chADISEnable.Checked = true;
                ADIS16460_Refresh_Window_Control(false);
            }
            return false;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Data Readout
        //#####################################################################################################

        #region//=============================================================================================Setup Column Format
        private System.Windows.Forms.DataGridView ADISReadoutGrid;
        DataGridViewTextBoxColumn[] ADISCmdColumn;
        static SupportDataGrid[] myADISLayout = new SupportDataGrid[]
            {
                new SupportDataGrid("Ch",30),
                new SupportDataGrid("Name",100),
                //new SupportDataGrid("Format",50),     // Do this later
                new SupportDataGrid("Data",150),
            };
        #endregion

        #region //=============================================================================================ADISReadoutGridSetup
        public void ADISReadoutGridSetup()
        {
            ADISReadoutGrid = new System.Windows.Forms.DataGridView();
            ADISReadoutGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            ADISReadoutGrid.RowTemplate.Height = 18;
            ADISReadoutGrid.RowTemplate.MinimumHeight = 18;
            //--------------------------------------------------------------------
            ADISReadoutGrid.Location = new System.Drawing.Point(0, 0);
            ADISReadoutGrid.Size = new System.Drawing.Size(354, 175);
            ADISReadoutGrid.MaximumSize = new System.Drawing.Size(354, 175);
            ADISReadoutGrid.MinimumSize= new System.Drawing.Size(354, 175);
            ADISReadoutGrid.TabIndex = 0;
            ADISReadoutGrid.AllowUserToAddRows = false;
            ADISReadoutGrid.AllowUserToDeleteRows = false;
            ADISReadoutGrid.AllowUserToResizeColumns = false;
            ADISReadoutGrid.AllowUserToResizeRows = false;
            ADISReadoutGrid.BackgroundColor = System.Drawing.Color.White;
            ADISReadoutGrid.MultiSelect = false;
            ADISReadoutGrid.Name = "ADIS_Readout";
            ADISReadoutGrid.RowHeadersVisible = false;
            ADISReadoutGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

            //--------------------------------------------------------------------
            ADISCmdColumn = new DataGridViewTextBoxColumn[5];
            for (int i = 0; i < 3; i++)
            {
                ADISCmdColumn[i] = new DataGridViewTextBoxColumn();
                ADISCmdColumn[i].Name = "txt" + myADISLayout[i].Label;
                ADISCmdColumn[i].HeaderText = myADISLayout[i].Label;
                ADISCmdColumn[i].Width = myADISLayout[i].Width;
                ADISCmdColumn[i].Resizable = DataGridViewTriState.False;
                ADISCmdColumn[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                ADISCmdColumn[i].ReadOnly = true;
                this.ADISReadoutGrid.Columns.Add(ADISCmdColumn[i]);
            }
            //=========================================Add Row with color for ADC12/ADC24
            for (int i = 0; i <= 15; i++)
            {
                this.ADISReadoutGrid.Rows.Add(new string[] { (i + 1).ToString("D1") });
            }
            ADISReadoutGrid.Rows[0].Cells[1].Value = "Accel-X";
            ADISReadoutGrid.Rows[1].Cells[1].Value = "Accel-Y";
            ADISReadoutGrid.Rows[2].Cells[1].Value = "Accel-Z";
            ADISReadoutGrid.Rows[3].Cells[1].Value = "Gyro-X";
            ADISReadoutGrid.Rows[4].Cells[1].Value = "Gyro-Y";
            ADISReadoutGrid.Rows[5].Cells[1].Value = "Gyro-Z";
            ADISReadoutGrid.Rows[6].Cells[1].Value = "Inclination";
            ADISReadoutGrid.Rows[7].Cells[1].Value = "Toolface";
            ADISReadoutGrid.Rows[8].Cells[1].Value = "Magnitude";
            ADISReadoutGrid.Rows[9].Cells[1].Value = "Temp(K/10)";
            ADISReadoutGrid.Rows[10].Cells[1].Value = "Velocity-X";
            ADISReadoutGrid.Rows[11].Cells[1].Value = "Velocity-Y";
            ADISReadoutGrid.Rows[12].Cells[1].Value = "Velocity-Z";
            ADISReadoutGrid.Rows[13].Cells[1].Value = "DeltaAngle-X";
            ADISReadoutGrid.Rows[14].Cells[1].Value = "DeltaAngle-Y";
            ADISReadoutGrid.Rows[15].Cells[1].Value = "DeltaAngle-Z";

            ADISReadoutGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ADISReadoutGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            ADISReadoutGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            ADISReadoutGrid.AllowUserToResizeRows = false;
            this.gbADISData.Controls.Add(this.ADISReadoutGrid);
            //this.gbADISData.Location = new System.Drawing.Point(6, 19);
            //========================================Add Event
        }
        #endregion

        #region //=============================================================================================ADISReadout_RefreshUpdate

        private void ADISReadout_ProcessRow(Int32 RowNo, Int32 Format, Int32 iData, double dData)
        {
            if (Format==1)  // Double
                ADISReadoutGrid.Rows[RowNo].Cells[2].Value = dData;
            else
                ADISReadoutGrid.Rows[RowNo].Cells[2].Value = iData;
        }

        public void ADISReadout_RefreshUpdate()
        {
            if (ADISReadoutGrid == null)
                return;
            //--------------------Accel
            ADISReadout_ProcessRow(0, ADIS16460Setup.iAccelFormat, ADIS16460Data.iAccel.X, ADIS16460Data.dAccel.X);
            ADISReadout_ProcessRow(1, ADIS16460Setup.iAccelFormat, ADIS16460Data.iAccel.Y, ADIS16460Data.dAccel.Y);
            ADISReadout_ProcessRow(2, ADIS16460Setup.iAccelFormat, ADIS16460Data.iAccel.Z, ADIS16460Data.dAccel.Z);
            //--------------------Gyro
            ADISReadout_ProcessRow(3, ADIS16460Setup.iGyroFormat, ADIS16460Data.iGyro.X, ADIS16460Data.dGyro.X);
            ADISReadout_ProcessRow(4, ADIS16460Setup.iGyroFormat, ADIS16460Data.iGyro.Y, ADIS16460Data.dGyro.Y);
            ADISReadout_ProcessRow(5, ADIS16460Setup.iGyroFormat, ADIS16460Data.iGyro.Z, ADIS16460Data.dGyro.Z);
            //--------------------Velocity & Delat Angle
            if (ADIS16460Setup.bAdvFrame == true)
            {
                ADISReadoutGrid.Rows[10].Cells[1].Value = "Velocity-X";
                ADISReadoutGrid.Rows[11].Cells[1].Value = "Velocity-Y";
                ADISReadoutGrid.Rows[12].Cells[1].Value = "Velocity-Z";
                ADISReadoutGrid.Rows[13].Cells[1].Value = "DeltaAngle-X";
                ADISReadoutGrid.Rows[14].Cells[1].Value = "DeltaAngle-Y";
                ADISReadoutGrid.Rows[15].Cells[1].Value = "DeltaAngle-Z";
                ADISReadout_ProcessRow(10, 0, ADIS16460Data.iVelocity.X, 0.0);
                ADISReadout_ProcessRow(11, 0, ADIS16460Data.iVelocity.Y, 0.0);
                ADISReadout_ProcessRow(12, 0, ADIS16460Data.iVelocity.Z, 0.0);
                ADISReadout_ProcessRow(13, 0, ADIS16460Data.iDeltaAngle.X, 0.0);
                ADISReadout_ProcessRow(14, 0, ADIS16460Data.iDeltaAngle.Y, 0.0);
                ADISReadout_ProcessRow(15, 0, ADIS16460Data.iDeltaAngle.Z, 0.0);
            }
            else
            {
                for (int i = 1; i <= 2; i++)
                {
                    ADISReadoutGrid.Rows[10].Cells[i].Value = "";
                    ADISReadoutGrid.Rows[11].Cells[i].Value = "";
                    ADISReadoutGrid.Rows[12].Cells[i].Value = "";
                    ADISReadoutGrid.Rows[13].Cells[i].Value = "";
                    ADISReadoutGrid.Rows[14].Cells[i].Value = "";
                    ADISReadoutGrid.Rows[15].Cells[i].Value = "";
                }
            }
            //--------------------Temp
            ADISReadout_ProcessRow(9, 0, ADIS16460Data.Temp, 0.0);
            //--------------------Math
            if (ADIS16460Setup.bAccelMath == true)
            {
                ADISReadoutGrid.Rows[6].Cells[1].Value = "Inclination";
                ADISReadoutGrid.Rows[7].Cells[1].Value = "Toolface";
                ADISReadoutGrid.Rows[8].Cells[1].Value = "Magnitude";
                ADISReadout_ProcessRow(6, 1, 0, ADIS16460Data.dAInclination);
                ADISReadout_ProcessRow(7, 1, 0, ADIS16460Data.dAToolFace);
                ADISReadout_ProcessRow(8, 1, 0, ADIS16460Data.dAMagnitide);
            }
            else
            {
                for (int i = 1; i <= 2; i++)
                {
                    ADISReadoutGrid.Rows[6].Cells[i].Value = "";
                    ADISReadoutGrid.Rows[7].Cells[i].Value = "";
                    ADISReadoutGrid.Rows[8].Cells[i].Value = "";
                }
            }
            //--------------------Done!
            ADISReadoutGrid.Refresh();
            //------------------------------------Update Gravity Data Box
            if (ADIS16460Setup.bAccelMath == true)
            {
                gpBasicMath.Visible = true;
                lbEnableText.Visible = false;
                tboMag.Text = ADIS16460Data.dAMagnitide.ToString("F5");
                tboInc.Text = ADIS16460Data.dAInclination.ToString("F3");
                tboToolFace.Text = ADIS16460Data.dAToolFace.ToString("F3");
                if (ADIS16460Setup.iAccelFormat==1) // Double Value
                {
                    tbXAxis.Text = ADIS16460Data.dAccel.X.ToString("F4");
                    tbYAxis.Text = ADIS16460Data.dAccel.Y.ToString("F4");
                    tbZAxis.Text = ADIS16460Data.dAccel.Z.ToString("F4");
                }
                else
                {
                    tbXAxis.Text = ADIS16460Data.iAccel.X.ToString();
                    tbYAxis.Text = ADIS16460Data.iAccel.Y.ToString();
                    tbZAxis.Text = ADIS16460Data.iAccel.Z.ToString();
                }
            }
            else
            {
                gpBasicMath.Visible = false;
            }



        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Window
        //#####################################################################################################

        #region //=============================================================================================ADIS16460_Refresh_Window_Read  
        public void ADIS16460_Refresh_Window_Control(bool bEnableRefreashRead)
        {
            if (TabWindowEnable.bADIS16460 == false)            // Hide Elements
            {
                ADIS_RunMode(0, true);                          // Issue Stop command.
                lbEnableText.Visible = true;
                chADISEnable.Checked = false;
                gbSetup.Visible = false;
                gbADISData.Visible = false;
                gpBasicMath.Visible = false;
                btnADISCaptureNow.Visible = false;
                cbADISExportSetup.Visible = false;
                btnADISImportSetting.Visible = false;
                btnADISExportSetting.Visible = false;
                btnRun.Visible = false;
                btnStop.Visible = false;
                btnADISCaptureNow.Enabled = false;
            }
            else                                                // Display Elements
            {
                cbADISExportSetup.Checked = true;                   // Need export to update.
                chADISEnable.Checked = true;
                gbSetup.Visible = true;
                gbADISData.Visible = true;
                gpBasicMath.Visible = true;
                btnADISCaptureNow.Visible = true;
                cbADISExportSetup.Visible = true;
                btnADISImportSetting.Visible = true;
                btnADISExportSetting.Visible = true;
                btnADISCaptureNow.Enabled = true;
                ADIS_Connect();                             // Make connection.
                Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
                if (bEnableRefreashRead==true)
                    ADIS16460_Refresh_Window_Read();
            }
        }
        #endregion

        #region //=============================================================================================ADIS16460_Refresh_Window_Read  
        public void ADIS16460_Refresh_Window_Read()
        {
            if (ADIS16460Setup == null)
                return;
            if (TabWindowEnable.bADIS16460 == false)
                return;
            ADIS16460Setup.iAccelFormat = clbAccelFormat.SelectedIndex;
            ADIS16460Setup.iGyroFormat = clbGyroFormat.SelectedIndex;
            ADIS16460Setup.iAxisPolicy = clbAxisPolicy.SelectedIndex;
            ADIS16460Setup.bAccelMath = cbAccelMath.Checked;
            ADIS16460Setup.bAdvFrame = cbAdvanceFrame.Checked;
            ADISReadout_RefreshUpdate();
        }
        #endregion

        #region //=============================================================================================ADIS16460_Refresh_Window_Write  
        public void ADIS16460_Refresh_Window_Write()
        {
            if (ADIS16460Setup == null)
                return;
            if (TabWindowEnable.bADIS16460 == false)
                return;
            clbAccelFormat.ItemCheck -= clbAccelFormat_ItemCheck;
            clbGyroFormat.ItemCheck -= clbGyroFormat_ItemCheck;
            clbAxisPolicy.ItemCheck -= clbAxisPolicy_ItemCheck;

            clbAccelFormat.ClearSelected();
            clbGyroFormat.ClearSelected();
            clbAxisPolicy.ClearSelected();
            for (int i = 0; i < 3; i++)
            {
                clbAccelFormat.SetItemCheckState(i, CheckState.Unchecked);
                clbGyroFormat.SetItemCheckState(i, CheckState.Unchecked);
                clbAxisPolicy.SetItemCheckState(i, CheckState.Unchecked);
            }

            if (ADIS16460Setup.iAccelFormat!=-1)
                clbAccelFormat.SetItemCheckState(ADIS16460Setup.iAccelFormat,CheckState.Checked);
                //clbAccelFormat.SetItemChecked(ADIS16460Setup.iAccelFormat, true);
            if (ADIS16460Setup.iGyroFormat != -1)
                clbGyroFormat.SetItemCheckState(ADIS16460Setup.iGyroFormat, CheckState.Checked);
               // clbGyroFormat.SetItemChecked(ADIS16460Setup.iGyroFormat, true);
            if (ADIS16460Setup.iAxisPolicy != -1)
                clbAxisPolicy.SetItemCheckState(ADIS16460Setup.iAxisPolicy, CheckState.Checked);
            //clbAxisPolicy.SetItemChecked(ADIS16460Setup.iAxisPolicy, true);

            clbAccelFormat.SelectedIndex = ADIS16460Setup.iAccelFormat;
            clbGyroFormat.SelectedIndex = ADIS16460Setup.iGyroFormat;
            clbAxisPolicy.SelectedIndex = ADIS16460Setup.iAxisPolicy;

            clbAccelFormat.Update();
            clbGyroFormat.Update();
            clbAxisPolicy.Update();

            clbAccelFormat.ItemCheck += clbAccelFormat_ItemCheck;
            clbGyroFormat.ItemCheck += clbGyroFormat_ItemCheck;
            clbAxisPolicy.ItemCheck += clbAxisPolicy_ItemCheck;


            //clbAxisPolicy.Invalidate();

            //clbAccelFormat.SelectedIndex = ADIS16460Setup.iAccelFormat;     //This highlight blue.
            //clbGyroFormat.SelectedIndex = ADIS16460Setup.iGyroFormat;       //This highlight blue.
            //clbAxisPolicy.SelectedIndex = ADIS16460Setup.iAxisPolicy;       //This highlight blue.
            cbAccelMath.Checked = ADIS16460Setup.bAccelMath;
            cbAdvanceFrame.Checked = ADIS16460Setup.bAdvFrame;
            gbSetup.Invalidate();
            gbSetup.Refresh();

            ADISReadout_RefreshUpdate();;
        }
        #endregion

        #region //=============================================================================================clbAccelFormat_ItemCheck  
        private void clbAccelFormat_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (clbAccelFormat.CheckedItems.Count == 1)
            {
                Boolean isCheckedItemBeingUnchecked = (e.CurrentValue == CheckState.Checked);
                if (isCheckedItemBeingUnchecked)
                {
                    e.NewValue = CheckState.Checked;
                }
                else
                {
                    Int32 checkedItemIndex = clbAccelFormat.CheckedIndices[0];
                    clbAccelFormat.ItemCheck -= clbAccelFormat_ItemCheck;
                    clbAccelFormat.SetItemChecked(checkedItemIndex, false);
                    clbAccelFormat.ItemCheck += clbAccelFormat_ItemCheck;
                }
                ADIS16460_Refresh_Window_Read();
                cbADISExportSetup.Checked = true;
                return;
            }
        }
        #endregion

        #region //=============================================================================================clbGyroFormat_ItemCheck  
        private void clbGyroFormat_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (clbGyroFormat.CheckedItems.Count == 1)
            {
                Boolean isCheckedItemBeingUnchecked = (e.CurrentValue == CheckState.Checked);
                if (isCheckedItemBeingUnchecked)
                {
                    e.NewValue = CheckState.Checked;
                }
                else
                {
                    Int32 checkedItemIndex = clbGyroFormat.CheckedIndices[0];
                    clbGyroFormat.ItemCheck -= clbGyroFormat_ItemCheck;
                    clbGyroFormat.SetItemChecked(checkedItemIndex, false);
                    clbGyroFormat.ItemCheck += clbGyroFormat_ItemCheck;
                }
                ADIS16460_Refresh_Window_Read();
                cbADISExportSetup.Checked = true;
                return;
            }
        }
        #endregion

        #region //=============================================================================================clbAxisPolicy_ItemCheck  
        private void clbAxisPolicy_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (clbAxisPolicy.CheckedItems.Count == 1)
            {
                Boolean isCheckedItemBeingUnchecked = (e.CurrentValue == CheckState.Checked);
                if (isCheckedItemBeingUnchecked)
                {
                    e.NewValue = CheckState.Checked;
                }
                else
                {
                    Int32 checkedItemIndex = clbAxisPolicy.CheckedIndices[0];
                    clbAxisPolicy.ItemCheck -= clbAxisPolicy_ItemCheck;
                    clbAxisPolicy.SetItemChecked(checkedItemIndex, false);
                    clbAxisPolicy.ItemCheck += clbAxisPolicy_ItemCheck;
                }
                ADIS16460_Refresh_Window_Read();
                cbADISExportSetup.Checked = true;
                return;
            }
        }
        #endregion

        #region //=============================================================================================ADIS16460_InitDefaultSetupFirstTime  
        public void ADIS16460_InitDefaultSetupFirstTime()
        {
            //----------------------------------------------------------- +FCL30SETUP (uG; XYZ ;Enabled)
            ADIS16460Setup.iAccelFormat = 2;
            ADIS16460Setup.iGyroFormat = 2;
            ADIS16460Setup.iAxisPolicy = 0;
            ADIS16460Setup.bAccelMath = true;
            ADIS16460Setup.bAdvFrame = false;      // No need for velocity and delta-angle.
            ADIS16460Setup.bRunStop = false;      // Because it not connected yet, do this when bIsPlugged is set.
            ADIS16460Setup.bIsPlugged = false;     // Read only from MiniAD7794. 
            //------------------------------------------
            cbADISExportSetup.Checked = true;          // Need export to update.
        }
        #endregion

        #region //=============================================================================================chADISEnable_MouseClick  
        private void chADISEnable_MouseClick(object sender, MouseEventArgs e)
        {
            if (myDMFProtocol.ValidateRef_MyADIS16460() == false)     // null ref due to timing between various window. This ensure it synced.
                myDMFProtocol.MyMiniADIS1640(this);
            myGlobalBase.bTabWindowEnableUpdate = true;
            if (chADISEnable.Focused)
            {
                cbADISExportSetup.Checked = true;                   // Need export to update.
                TabWindowEnable.bADIS16460 = chADISEnable.Checked;
                ADIS16460_Refresh_Window_Control(true);
            }
        }
        #endregion

        #region //=============================================================================================cbAccelMath_MouseClick 
        private void cbAccelMath_MouseClick(object sender, MouseEventArgs e)
        {
            if (cbAccelMath.Focused)
            {
                ADIS16460Setup.bAccelMath = cbAccelMath.Checked;
                ADIS16460_Refresh_Window_Write();
                cbADISExportSetup.Checked = true;          // Need export to update.
            }
        }
        #endregion

        #region //=============================================================================================cbAdvanceFrame_MouseClick 
        private void cbAdvanceFrame_MouseClick(object sender, MouseEventArgs e)
        {
            if (cbAdvanceFrame.Focused)
            {
                ADIS16460Setup.bAdvFrame = cbAdvanceFrame.Checked;
                ADIS16460_Refresh_Window_Write();
                cbADISExportSetup.Checked = true;          // Need export to update.
            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Capture Data
        //#####################################################################################################

        #region //=============================================================================================btnADISCaptureNow_Click & CalBacks 
        private void btnADISCaptureNow_Click(object sender, EventArgs e)
        {
            if (myDMFProtocol.ValidateRef_MyADIS16460() == false)     // null ref due to timing between various window. This ensure it synced.
                myDMFProtocol.MyMiniADIS1640(this);
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            //--------------------------------------------------------------Send Setup Data before Capture.
            ADIS16460_Refresh_Window_Read();
            if (cbADISExportSetup.Checked == true)
            {                      
                /*
                if (ADIS_RunMode(0,true) == null)                        // Issue Stop command.
                    return;
                Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
                */
                DMFP_BulkFrame cDMFP_BulkFrame = ADIS_Update_Setup_Data(true);      // Set True, Required in order to activate the ID procedure if device is fitted or not. 
                while (cDMFP_BulkFrame.isBusy == true)
                {
                    Tools.InteractivePause(TimeSpan.FromMilliseconds(25));
                }
                Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
                if (ADIS_RunMode(1,true) == null)
                    return;
                cbADISExportSetup.Checked = false;
                Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
                btnRun.Visible = false;
                btnStop.Visible = true;
                btnADISCaptureNow.Enabled = false;
            }
            else
            {
                if (ADIS_RunMode(1, true) == null)                        // Issue Stop command.
                    return;
                btnRun.Visible = false;
                btnStop.Visible = true;
                btnADISCaptureNow.Enabled = false;
                Tools.InteractivePause(TimeSpan.FromMilliseconds(100));
            }
            /*
            //--------------------------------------------------------------Send Capture Command
            //-----------------------------------Setup call-back delegate. 
            DMFP_Delegate fpCallBack_ADISDATA = new DMFP_Delegate(DMFP_ADISDATA_CallBack);
            //-----------------------------------Place Message to rtbTerm Window.
            string sMessage = "+ADISDATA()";
            if (myGlobalBase.bNoRFTermMessage == false)
                myMainProg.myRtbTermMessageLF("#DMFP:" + sMessage);
            Tools.InteractivePause(TimeSpan.FromMilliseconds(50));
            //-----------------------------------Create separate thread so Sent message via TX operation avoid hanging the form. 
            Thread t3 = new Thread(() => myDMFProtocol.DMFP_VCOMArry_Send_Command(fpCallBack_ADISDATA, sMessage, 250, (int)eUSBDeviceType.MiniAD7794));
            t3.Start();
            myDMFProtocol.DMFProtocol_UpdateList();
            */
        }
        #endregion

        #region //=============================================================================================DMFP_ADISDATA_CallBack  
        public void DMFP_ADISDATA_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            int icount = 0;
            int scount = 0;
            //int dcount = 0;
            UInt32 Status = hPara[0];
            if (Status != 0xFFFF)       // No Error
            {
                myMainProg.myRtbTermMessageLF("#E:ADISDATA reported error code:0x" + hPara[0].ToString("X"));
                return;
            }
            //------------------------------In case the MiniAD7794 firmware is left running mode while app restart. 
            if (ADIS16460Data == null)
                return;
            //------------------------------
            ADIS16460Data.ClearAllData();
            //---------------------------------------------------Accel Section
            if (ADIS16460Setup.iAccelFormat==1) // Double
            {
                ADIS16460Data.dAccel.X = Tools.ConversionStringtoDouble(sPara[0]);
                ADIS16460Data.dAccel.Y = Tools.ConversionStringtoDouble(sPara[1]);
                ADIS16460Data.dAccel.Z = Tools.ConversionStringtoDouble(sPara[2]);
                scount = 3;
            }
            else                                // Integer
            {
                ADIS16460Data.iAccel.X = iPara[0];
                ADIS16460Data.iAccel.Y = iPara[1];
                ADIS16460Data.iAccel.Z = iPara[2];
                icount = 3;
            }
            //---------------------------------------------------Accel Section
            if (ADIS16460Setup.iGyroFormat == 1) // Double
            {
                ADIS16460Data.dGyro.X = Tools.ConversionStringtoDouble(sPara[scount]);
                ADIS16460Data.dGyro.Y = Tools.ConversionStringtoDouble(sPara[scount + 1]); 
                ADIS16460Data.dGyro.Z = Tools.ConversionStringtoDouble(sPara[scount + 2]);
                scount += 3;
            }
            else                                // Integer
            {
                ADIS16460Data.iGyro.X = iPara[icount];
                ADIS16460Data.iGyro.Y = iPara[icount + 1];
                ADIS16460Data.iGyro.Z = iPara[icount + 2];
                icount += 3;
            }
            //---------------------------------------------------Temp Section
            ADIS16460Data.Temp = iPara[icount];
            icount += 1;
            //---------------------------------------------------Advance Frame Section
            if (ADIS16460Setup.bAdvFrame == true)
            {
                if (icount+6 < iPara.Count)
                {
                    ADIS16460Data.iVelocity.X = iPara[icount];
                    ADIS16460Data.iVelocity.Y = iPara[icount + 1];
                    ADIS16460Data.iVelocity.Z = iPara[icount + 2];
                    ADIS16460Data.iDeltaAngle.X = iPara[icount + 3];
                    ADIS16460Data.iDeltaAngle.Y = iPara[icount + 4];
                    ADIS16460Data.iDeltaAngle.Z = iPara[icount + 5];
                    icount += 6;
                }
            }
            //---------------------------------------------------Accel Math 
            if (ADIS16460Setup.bAccelMath == true)  // All double
            {

                ADIS16460Data.dAInclination = Tools.ConversionStringtoDouble(sPara[scount]);
                ADIS16460Data.dAToolFace = Tools.ConversionStringtoDouble(sPara[scount+1]);
                ADIS16460Data.dAMagnitide = Tools.ConversionStringtoDouble(sPara[scount+2]);
                scount += 3;

            }
            //====================Math her, future project task.
            ADISReadout_RefreshUpdate();
        }
        #endregion

        public void MiniAD9974_Async_ADISDATA_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage, IList<string> Asynclistparameter)
        {
            if (chADISEnable.Checked == false)
                return;
            DMFP_ADISDATA_CallBack(iPara, hPara, sPara, dPara, CmdMessage,FullMessage);
            btnRun.Visible = false;
            btnStop.Visible = true;
            btnADISCaptureNow.Enabled = false;
        }

        //#####################################################################################################
        //###################################################################################### Export Setup
        //#####################################################################################################

        #region //=============================================================================================btnADISExportSetting_Click 
        private void btnADISExportSetting_Click(object sender, EventArgs e)
        {
            ADIS_Update_Setup_Data(false);
        }
        #endregion

        #region //=============================================================================================ADIS_Update_Setup_Data & CalBacks 
        public DMFP_BulkFrame ADIS_Update_Setup_Data(bool forceSetBRunStop)
        {
            if (ADIS16460Setup == null)
                return (null);
            if (ADIS16460Setup.bRunStop == true)        //
            {
                myMainProg.myRtbTermMessageLF("#E: On-going real time data collection click <STOP> button first");
                return (null);
            }
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return (null);
            }
            if (forceSetBRunStop == true)
            {
                ADIS16460Setup.bRunStop = true;
            }
            ADIS16460_Refresh_Window_Read();
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();

            //---------------------------------------------------------------------------------------------------------FCL30 Initial Setup 
            //+ADISSETUP (AccelFormat; GyroFormat; AxiPolicy; AccelMath; isAdvFrame; isEnabled)\n
            string command = "+ADISSETUP(";
            command += (ADIS16460Setup.iAccelFormat).ToString() + ";";
            command += (ADIS16460Setup.iGyroFormat).ToString() + ";";
            command += (ADIS16460Setup.iAxisPolicy).ToString() + ";";
            command += (Tools.BinaryFalseTrue(ADIS16460Setup.bAccelMath)).ToString() + ";";
            command += (Tools.BinaryFalseTrue(ADIS16460Setup.bAdvFrame)).ToString() + ";";
            command += (Tools.BinaryFalseTrue(ADIS16460Setup.bRunStop)).ToString() + ")";
            cDMFP_BulkFrame.AddItem(command, new DMFP_Delegate(DMFP_ADISSETUP_CallBack));
            //---------------------------------------------------------------------Send Bulk Frame
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 10;
            cDMFP_BulkFrame.isNoErrorReport = true;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);
            //------------------------------------------------------------------------------------
            return cDMFP_BulkFrame;
        }
        #endregion

        #region //=============================================================================================DMFP_ADISSETUP_CallBack
        
        public void DMFP_ADISSETUP_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFFFF)
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_ADISSETUP_CallBack()...has Error code: 0x" + hPara[0].ToString("X4") + "| DataCode 0x" + hPara[1].ToString("X4"));

                if (hPara[0]==0xF000)   // Device not fitted error, therefore not enabled. 
                    ADIS16460Setup.bRunStop = false;
            }
        }
        #endregion

        #region //=============================================================================================DMFP_ADISRUNSTOP_CallBack
        public void DMFP_ADISRUNSTOP_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0] != 0xFFFF)
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_ADISRUNSTOP_CallBack()...has Error code: 0x" + hPara[0].ToString("X4") + "| DataCode 0x" + hPara[1].ToString("X4"));
            }
        }

        #endregion

        //#####################################################################################################
        //###################################################################################### Import Setup
        //#####################################################################################################

        #region //=============================================================================================btnADISImportSetting_Click
        private void btnADISImportSetting_Click(object sender, EventArgs e)
        {
            if (ADIS16460Setup == null)
                return;
            if (ADIS16460Setup.bRunStop == true)        //
            {
                myMainProg.myRtbTermMessageLF("#E: On-going real time data collection click <STOP> button first");
                return;
            }
            ADIS_RunMode(0,true);                        // Issue Stop command.
            btnRun.Visible = false;
            btnStop.Visible = false;
            btnADISCaptureNow.Enabled = true;

            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return;
            }
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();
            //---------------------------------------------------------------------------------------------------------FCL30 Initial Setup 
            //+ADISRUNSTOP (OpMode)\n   //Set to Run.		  
            cDMFP_BulkFrame.AddItem("+ADISSETUPR()", new DMFP_Delegate(DMFP_ADISSETUPR_CallBack));
            //---------------------------------------------------------------------------------------------------------ADC24 Global Setup      //---------------------------------------------------------------------Send Bulk Frame
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 10;
            cDMFP_BulkFrame.isNoErrorReport = true;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);
        }
        #endregion

        #region //=============================================================================================DMFP_ADISETUPR_CallBack
        public void DMFP_ADISSETUPR_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (ADIS16460Setup == null)
                return;
            //-ADISSETUPR(Errorcode; AccelFormat; GyroFormat; AxiPolicy; AccelMath; isAdvFarme, isEnabled ,isPlugged)\n
            if (hPara[0] == 0xFFFF)
            {
                ADIS16460Setup.iAccelFormat = iPara[0];
                ADIS16460Setup.iGyroFormat = iPara[1];
                ADIS16460Setup.iAxisPolicy  = iPara[2];
                ADIS16460Setup.bAccelMath = Convert.ToBoolean(iPara[3]);
                ADIS16460Setup.bAdvFrame = Convert.ToBoolean(iPara[4]);
                ADIS16460Setup.bRunStop = Convert.ToBoolean(iPara[5]);
                ADIS16460Setup.bIsPlugged = Convert.ToBoolean(iPara[6]);
            }
            else
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_ADISSETUPR_CallBack()...has Error code: 0x" + hPara[0].ToString("X4") + "| DataCode 0x" + hPara[1].ToString("X4"));
            }
            ADIS16460_Refresh_Window_Write();
            cbADISExportSetup.Checked = true;
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Run/Stop
        //#####################################################################################################

        #region //=============================================================================================btnRun_Click
        private void btnRun_Click(object sender, EventArgs e)
        {
            if (ADIS_RunMode(1,false) == null)
                return;
            btnRun.Visible = false;
            btnStop.Visible = true;
            btnADISCaptureNow.Enabled = false;
        }
        #endregion

        #region //=============================================================================================ADIS_RunMode     
        public DMFP_BulkFrame ADIS_RunMode(int RunStop, bool bWaitLoop)
        {
            int loop = 0;
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return (null);
            }
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();
            //---------------------------------------------------------------------------------------------------------
            //+ADISRUNSTOP (x)\n	x=0: Stop, x=1: Run debug, x=2: Run LoggerCVS (both debug and flash). 
            switch (RunStop)
            {
                case (1):   // Run Debug Mode
                    {
                        cDMFP_BulkFrame.AddItem("+ADISRUNSTOP(1)", new DMFP_Delegate(DMFP_ADISRUNSTOP_CallBack));
                        ADIS16460Setup.bRunStop = true;
                        break;
                    }
                case (2):   //Run LoggerCVS Mode
                    {
                        cDMFP_BulkFrame.AddItem("+ADISRUNSTOP(2)", new DMFP_Delegate(DMFP_ADISRUNSTOP_CallBack));
                        ADIS16460Setup.bRunStop = false;
                        break;
                    }
                default:    // (also 0) Stop
                    {
                        cDMFP_BulkFrame.AddItem("+ADISRUNSTOP(0)", new DMFP_Delegate(DMFP_ADISRUNSTOP_CallBack));
                        ADIS16460Setup.bRunStop = false;
                        break;
                    }
            }
            //---------------------------------------------------------------------------------------------------------     
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 10;
            cDMFP_BulkFrame.isNoErrorReport = true;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);

            if (bWaitLoop == true)
            {
                while (cDMFP_BulkFrame.isBusy == true)
                {
                    Tools.InteractivePause(TimeSpan.FromMilliseconds(25));
                    loop++;
                    if (loop > 40)      // Second too long to wait. 
                        return (null);
                }
            }
            return (cDMFP_BulkFrame);
        }
        #endregion

        #region //=============================================================================================btnStop_Click
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (ADIS_RunMode(0,false) == null)
                return;
            btnRun.Visible = true;
            btnStop.Visible = false;
            btnADISCaptureNow.Enabled = true;
        }
        #endregion

        #region //=============================================================================================ADIS_Connect
        private DMFP_BulkFrame ADIS_Connect()
        {
            int loop = 0;
            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].isSerial_Server_Connected == false)
            {
                myMainProg.myRtbTermMessageLF("#E: The MiniPort VCOM Channel is not open. Try again with Connect Button");
                return (null);
            }
            DMFP_BulkFrame cDMFP_BulkFrame = new DMFP_BulkFrame();
            //---------------------------------------------------------------------------------------------------------
            //+ADISCONNECT (x)\n	x=0: Stop, x=1: Run

            cDMFP_BulkFrame.AddItem("+ADISCONNECT(1)", new DMFP_Delegate(DMFP_ADISCONNECT_CallBack));
            //---------------------------------------------------------------------------------------------------------     
            cDMFP_BulkFrame.DMFPBulkMessageCounter = 0;
            cDMFP_BulkFrame.DMFP_USBDeviceChannel = (int)eUSBDeviceType.MiniAD7794;
            cDMFP_BulkFrame.DMFP_Delay = 10;
            cDMFP_BulkFrame.isNoErrorReport = true;
            myUSB_Message_Manager.DMFProtocol_BulkMessage(cDMFP_BulkFrame);
            while (cDMFP_BulkFrame.isBusy == true)
            {
                Tools.InteractivePause(TimeSpan.FromMilliseconds(25));
                loop++;
                if (loop > 40)      // Second too long to wait. 
                    break;
            }
            return (cDMFP_BulkFrame);
        }
        #endregion

        #region //=============================================================================================DMFP_ADISCONNECT_CallBack
        public void DMFP_ADISCONNECT_CallBack(List<int> iPara, List<UInt32> hPara, List<string> sPara, List<double> dPara, string CmdMessage, string FullMessage)
        {
            if (hPara[0]==0x1000)
            {
                gbConnectError.Visible = false;
                btnADISImportSetting.Visible = true;
                btnADISExportSetting.Visible = true;
                btnADISCaptureNow.Enabled = true;
                return;
            }
            if (hPara[0]==0xF000)
            {
                gbConnectError.Visible = true;
                btnADISImportSetting.Visible = false;
                btnADISExportSetting.Visible = false;
                btnADISCaptureNow.Enabled = false;
                return;
            }
            if (hPara[0] != 0xFFFF)
            {
                myMainProg.myRtbTermMessageLF("#E: DMFP_ADISCONNECT_CallBack()...has Error code: 0x" + hPara[0].ToString("X4") + "| DataCode 0x" + hPara[1].ToString("X4"));
                gbConnectError.Visible = true;
                btnADISImportSetting.Visible = false;
                btnADISExportSetting.Visible = false;
                btnADISCaptureNow.Enabled = false;
            }
        }

        #endregion


        //#####################################################################################################
        //###################################################################################### LoggerCVS Script
        //#####################################################################################################

        #region //==================================================LoggerCVS Header
        public void ADIS16460_LoggerCVS_Setup()
        {

        }

        #endregion


    }
}