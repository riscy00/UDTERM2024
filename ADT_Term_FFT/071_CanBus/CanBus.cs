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
using System.Text.RegularExpressions;
using JR.Utils.GUI.Forms;
using UDT_Term;

/// <summary>
/// Inclusion of PEAK PCAN-Basic namespace
/// </summary>
// using Peak.Can.Basic;
// using TPCANHandle = System.UInt16;
// using TPCANBitrateFD = System.String;
// using TPCANTimestampFD = System.UInt64;

// Power Tool: insert QuickLaunch with "@tasks CollapseRegions" to close all region only. 

namespace UDT_Term_FFT
{
    public partial class CanBus : Form
    {
        ITools Tools = new Tools();
        CanPCAN myCanPCAN;
        CanPCAN.PCAN_StdFrame myCAN;
        TextBox[] myData;
        TextBox[] mySDOData;
        bool isSDOActivated = false;

        public CanBus()
        {

            InitializeComponent();

            #region //==================================================Insert Data elements
            this.SuspendLayout();
            //----------------------------------------------Main Section.
            myData = new TextBox[8];
            for (int i = 0; i < 8; i++)
            {
                myData[i] = new TextBox();
                myData[i].Text = "00";
                myData[i].Size = new Size(26, 22);
                myData[i].AcceptsReturn = true;
                myData[i].AcceptsTab = true;
                myData[i].Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                myData[i].MaxLength = 2;
                myData[i].Name = "tbAD" + i;
                myData[i].TabIndex = 10 + i;
                myData[i].UseWaitCursor = false;
                myData[i].WordWrap = false;
                myData[i].CharacterCasing = CharacterCasing.Upper;
                //myData[i].TextChanged += new System.EventHandler(this.TbData_TextChanged);
                myData[i].KeyDown += new KeyEventHandler(this.TbData_KeyDown);
                myData[i].PreviewKeyDown += new PreviewKeyDownEventHandler(this.TbData_PrevieKeyDown);
                myData[i].Leave += new System.EventHandler(this.TbCanData_Leave);
            }
            myData[0].Location = new System.Drawing.Point(15, 70);
            myData[1].Location = new System.Drawing.Point(47, 70);
            myData[2].Location = new System.Drawing.Point(79, 70);
            myData[3].Location = new System.Drawing.Point(111, 70);
            myData[4].Location = new System.Drawing.Point(143, 70);
            myData[5].Location = new System.Drawing.Point(175, 70);
            myData[6].Location = new System.Drawing.Point(207, 70);
            myData[7].Location = new System.Drawing.Point(239, 70);
            for (int i = 0; i < 8; i++)
                this.Controls.Add(myData[i]);
            //--------------------------------------------------------SDO Section
            mySDOData = new TextBox[4];
            for (int i = 0; i < 4; i++)
            {
                mySDOData[i] = new TextBox();
                mySDOData[i].Text = "00";
                mySDOData[i].Size = new Size(26, 22);
                mySDOData[i].AcceptsReturn = true;
                mySDOData[i].AcceptsTab = true;
                mySDOData[i].Font = new System.Drawing.Font("Monospac821 BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                mySDOData[i].MaxLength = 2;
                mySDOData[i].Name = "tbSDOD" + i;
                mySDOData[i].TabIndex = 304 + i;
                mySDOData[i].UseWaitCursor = false;
                mySDOData[i].WordWrap = false;
                mySDOData[i].CharacterCasing = CharacterCasing.Upper;
                //myData[i].TextChanged += new System.EventHandler(this.TbData_TextChanged);
                mySDOData[i].KeyDown += new KeyEventHandler(this.TbSDOData_KeyDown);
                mySDOData[i].PreviewKeyDown += new PreviewKeyDownEventHandler(this.TbSDOData_PrevieKeyDown);
                mySDOData[i].Leave += new System.EventHandler(this.TbDSOData_Leave);
            }
            mySDOData[0].Location = new System.Drawing.Point(147,35);
            mySDOData[1].Location = new System.Drawing.Point(180,35);
            mySDOData[2].Location = new System.Drawing.Point(211,35);
            mySDOData[3].Location = new System.Drawing.Point(243,35);

            for (int i = 0; i < 4; i++)
                this.gBSDO.Controls.Add(mySDOData[i]);
            this.ResumeLayout();
            #endregion
        }

        //##############################################################################################################
        //============================================================================================= UDT Window Form 
        //##############################################################################################################

        #region //=======================================CanPCAN_Load
        private void CanBus_Load(object sender, EventArgs e)
        {
            //myCAN = new CanPCAN.PCAN_StdFrame();
            //myCAN.Clear();

        }
        #endregion

        #region //=======================================CanBus_FormClosing
        private void CanBus_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //=======================================CanPCAN_Show
        public void CanPCAN_Show()
        {
            this.BringToFront();
            this.TopMost = false;
            this.WindowState = FormWindowState.Normal;
            this.Visible = true;
            this.Show();
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= Main Code
        //##############################################################################################################

        #region //==================================================BtnOK_Click
        private void BtnOK_Click(object sender, EventArgs e)
        {
            CanFrame_Close_Editor();
        }
        #endregion

        #region //==================================================BtnCancel_Click
        private void BtnCancel_Click(object sender, EventArgs e)
        {
//             FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//             var dialogResult = FlexiMessageBox.Show("Warning: The change will be lost!", "CAN Frame Editor", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
//             if (dialogResult == DialogResult.OK)
//             {
                this.Close();
 //           }
        }
        #endregion

        #region //==================================================CanFrame_Close_Editor
        public void CanFrame_Close_Editor()
        {
            myCanPCAN.CanFrame_Editor_Done(ref myCAN);
            this.Close();
        }
        #endregion

        #region //==================================================CanFrame_Open_Editor
        public void CanFrame_Open_Editor(CanPCAN.PCAN_StdFrame myCANStdFrameRef, CanPCAN myCanPCANRef)
        {
            myCAN = null;
            myCAN = myCANStdFrameRef;
            //-----------------------------------Reference to CanPCAN Window. (Not Can Frame!). 
            if (myCanPCAN == null)
                myCanPCAN = myCanPCANRef;
            //-----------------------------------Update SDO section
            tbSDOSpec.Text = myCAN.DATA[0].ToString("X2");
            tbSDOIndex.Text = myCAN.DATA[2].ToString("X2") + myCAN.DATA[1].ToString("X2");
            tbSDOSub.Text = myCAN.DATA[3].ToString("X2");
            for (int i=4; i<8; i++)
            {
                mySDOData[i-4].Text = myCAN.DATA[i].ToString("X2");
            }
            UInt32 Spec = myCAN.DATA[0];
            tbBSpecCMD.Text = Tools.Bits_UInt32_intRead3Bit(Spec, 0).ToString("X");
            tbBSpecNofB.Text = Tools.Bits_UInt32_intRead2Bit(Spec, 4).ToString("X");
            tbBSpecExp.Text = Tools.Bits_UInt32_intRead(Spec, 6).ToString("X");
            tbBSpecS.Text = Tools.Bits_UInt32_intRead(Spec, 7).ToString("X");
            //-----------------------------------
            CanFrame_Refresh();
            CanPCAN_Show();
        }
        #endregion

        #region //==================================================CanFrame_Refresh
        private void CanFrame_Refresh()
        {
            //---------------------------------------------------------------
            if ((myCAN.CanID >= 0x580) & (myCAN.CanID <= 0x6F7))
            {
                this.Size = new Size(423, 304);
                isSDOActivated = true;
                if (myCAN.DLC <= 4)      // Reset DLC if less than 4 (illegal)
                    myCAN.DLC = 8;
                CanFrame_Refresh_SDO_UpdateD0D7();
            }
            else
            {
                this.Size = new Size(423, 200);
                isSDOActivated = false;
            }

            string sText64Bit = "";
            if (myCAN == null)
                return;
            tbCanID.Text = myCAN.CanID.ToString("X3");
            tbDLC.Text = myCAN.DLC.ToString();
            for (int i = 0; i < 8; i++)
            {
                if (i < myCAN.DLC)
                {
                    myData[i].BackColor = Color.White;
                    myData[i].Text = myCAN.DATA[i].ToString("X2");
                    sText64Bit += myData[i].Text;
                }
                else
                {
                    myData[i].BackColor = Color.Gainsboro;
                    myData[i].Text = "";
                    sText64Bit += "00";
                }
            }
            tbD0D7.Text = sText64Bit;
            chCycle.Checked = myCAN.isCycleEnable;
            tbDelay.Text = myCAN.Delay.ToString("D6");
            tbCycle.Text = myCAN.CyclePeriod.ToString("D6");
            tbCounts.Text = myCAN.CycleNumber.ToString("D4");
            tbComment.Text = myCAN.sComment;

        }
        #endregion

        #region //==================================================CanFrame_Refresh_SDO_UpdateD0D7
        private void CanFrame_Refresh_SDO_UpdateD0D7()
        {
            lbHexError.Visible = false;
            btnOK.Enabled = true;
            //----------------------------------------SDO/Main Section
            for (int i = 0; i < 8; i++)
            {
                if (i < myCAN.DLC)
                {
                    if (i >= 4)
                    {
                        mySDOData[i - 4].BackColor = Color.White;
                        mySDOData[i - 4].Text = myCAN.DATA[i].ToString("X2");
                    }
                    myData[i].BackColor = Color.White;
                    myData[i].Text = myCAN.DATA[i].ToString("X2");
                }
                else
                {
                    if (i >= 4)
                    {
                        mySDOData[i - 4].BackColor = Color.Gainsboro;
                        mySDOData[i - 4].Text = "";
                    }
                    myData[i].BackColor = Color.Gainsboro;
                    myData[i].Text = "";
                }
            }
            //---------------------------------------Main Data D0D7 only. 
            string sText64Bit = "";
            for (int i = 0; i < 8; i++)
            {
                if (i < myCAN.DLC)
                    sText64Bit += myData[i].Text;
                else
                    sText64Bit += "00";
            }
            tbD0D7.Text = sText64Bit;
            //---------------------------------------Update the Spec discrete elements
            UInt32 Spec = myCAN.DATA[0];

            tbBSpecCMD.Text = Tools.Bits_UInt32_intRead3Bit(Spec,5).ToString("X");
            tbBSpecNofB.Text = Tools.Bits_UInt32_intRead2Bit(Spec, 2).ToString("X");
            tbBSpecExp.Text = Tools.Bits_UInt32_intRead(Spec, 1).ToString("X");
            tbBSpecS.Text = Tools.Bits_UInt32_intRead(Spec, 0).ToString("X");

        }
        #endregion

        //##############################################################################################################
        //============================================================================================= D0 to D7 and DOD7 Editor
        //##############################################################################################################

        #region //==================================================Process DO-D7 Text Boxes.
        private void TbData_PrevieKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessKeyDown(e.KeyCode, textBox);
        }

        private void TbData_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessKeyDown(e.KeyCode, textBox);
        }
        private void TbCanData_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessKeyDown(Keys.Return, textBox);
        }
        private void ProcessKeyDown(Keys keyCode, TextBox textBox)
        {
            bool isAllHex = true;
            if ((keyCode == Keys.Return) || (keyCode == Keys.Tab))
            {
                for (int i = 0; i < myCAN.DLC; i++)
                {
                    myData[i].Text.ToUpper();
                    if (Tools.IsString_Hex(myData[i].Text) == false)
                    {
                        myData[i].BackColor = Color.LightPink;
                        isAllHex = false;
                        if (keyCode == Keys.Tab)
                        {
                            int ch = textBox.TabIndex - 10;
                            myData[ch - 1].Focus();                 // Go Back to correct. 
                        }
                    }
                    else
                    {
                        myCAN.DATA[i] = Tools.HexStringtoByte(myData[i].Text);
                        myData[i].BackColor = Color.White;
                    }
                }
                if (isAllHex == true)
                {
                    lbHexError.Visible = false;
                    btnOK.Enabled = true;

                    string sText64Bit = "";
                    for (int i = 0; i < 8; i++)
                    {
                        if (i < myCAN.DLC)
                        {
                            myData[i].BackColor = Color.White;
                            myData[i].Text = myCAN.DATA[i].ToString("X2");
                            sText64Bit += myData[i].Text;
                        }
                        else
                        {
                            myData[i].BackColor = Color.Gainsboro;
                            myData[i].Text = "";
                            sText64Bit += "00";
                        }
                    }
                    tbD0D7.Text = sText64Bit;
                }
                else
                {
                    lbHexError.Visible = true;
                    btnOK.Enabled = false;
                    lbHexError.Text = "HEX ERROR";

                }
            }
        }
        #endregion

        #region //==================================================Process DOD7 Text Box
        private void TbD0D7_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ProcessKeyDownDOD7(e.KeyCode);
        }

        private void TbD0D7_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyDownDOD7(e.KeyCode);
        }
        private void TbD0D7_Leave(object sender, EventArgs e)
        {
            ProcessKeyDownDOD7(Keys.Tab);
        }
        private void ProcessKeyDownDOD7(Keys keyCode)
        {
            if ((keyCode == Keys.Return) || (keyCode == Keys.Tab))
            {
                if (Tools.IsString_Hex(tbD0D7.Text) == false)
                {
                    FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    var dialogResult = FlexiMessageBox.Show("Note: This is 64 Bit (8 Byte), you can edit. Press Tab or Return to update.", "CAN Frame Setting", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tbD0D7.BackColor = Color.LightPink;
                    lbHexError.Visible = true;
                    btnOK.Enabled = false;
                    lbHexError.Text = "HEX ERROR";
                    if (keyCode == Keys.Tab)
                        myData[7].Focus();
                    //tbD0D7.Focus();                 // Go Back to correct. 
                }
                else
                {
                    byte[] mybyte = new byte[8];
                    mybyte = Tools.StrToHexBytesNonASCII(tbD0D7.Text);
                    for (int i = 0; i < myCAN.DLC; i++)
                    {
                        myCAN.DATA[i] = mybyte[i];
                        tbD0D7.BackColor = Color.White;
                        myData[i].Text = mybyte[i].ToString("X2");
                    }
                    lbHexError.Visible = false;
                    btnOK.Enabled = true;
                }
            }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= DLC Editor
        //##############################################################################################################

        #region //==================================================Process DLC Text Box
        private void TbDLC_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ProcessKeyDownDLC(e.KeyCode);
        }

        private void TbDLC_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyDownDLC(e.KeyCode);
        }
        private void TbDLC_Leave(object sender, EventArgs e)
        {
            ProcessKeyDownDLC(Keys.Tab);
        }

        private void ProcessKeyDownDLC(Keys keyCode)
        {
            bool isError = false;
            if ((keyCode == Keys.Return) || (keyCode == Keys.Tab))
            {
                if (Tools.IsString_Numberic_Byte(tbDLC.Text) == false)
                {
                    isError = true;
                }
                else
                {
                    byte DLC = Convert.ToByte(tbDLC.Text, 10);
                    if (DLC<=8)
                        myCAN.DLC = DLC;
                    else
                        isError = true;
                }
                if (isError==true)
                {
                    FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    var dialogResult = FlexiMessageBox.Show("Error: DLC must be 0 to 8", "CAN Frame Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tbDLC.BackColor = Color.LightPink;
                    lbHexError.Visible = true;
                    lbHexError.Text = "DLC ERROR";
                    btnOK.Enabled = false;
                    if (keyCode == Keys.Tab)            // Go Back to correct. 
                        tbDLC.Focus();
                }
                else
                {
                    tbDLC.BackColor = Color.White;
                    CanFrame_Refresh();
                    lbHexError.Visible = false;
                    btnOK.Enabled = true;
                }
            }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= CANID Editor
        //##############################################################################################################

        #region //==================================================Process CANID Text Box
        private void TbCanID_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ProcessKeyDownID(e.KeyCode);
        }

        private void TbCanID_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyDownID(e.KeyCode);
        }
        private void TbCanID_Leave(object sender, EventArgs e)
        {
            ProcessKeyDownID(Keys.Tab);
        }

        private void ProcessKeyDownID(Keys keyCode)
        {
            bool isError = false;
            if ((keyCode == Keys.Return) || (keyCode == Keys.Tab))
            {
                if (Tools.IsString_Hex(tbCanID.Text) == false)
                {
                    isError = true;
                }
                else
                {
                    UInt32 CANID = Tools.HexStringtoUInt32(tbCanID.Text);
                    if (CANID <= 0x7FF)
                        myCAN.CanID = CANID;
                    else
                        isError = true;
                }
                if (isError == true)
                {
                    FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    var dialogResult = FlexiMessageBox.Show("Error: CANID be less than 0x7FF (STD Frame)", "CAN Frame Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tbCanID.BackColor = Color.LightPink;
                    lbHexError.Visible = true;
                    lbHexError.Text = "ID ERROR";
                    btnOK.Enabled = false;
                    if (keyCode == Keys.Tab)            // Go Back to correct. 
                        tbCanID.Focus();
                }
                else
                {
                    tbCanID.BackColor = Color.White;
                    CanFrame_Refresh();
                    lbHexError.Visible = false;
                    btnOK.Enabled = true;
                }

            }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= DELAY Editor
        //##############################################################################################################

        #region //==================================================Process DELAY Text Box
        private void TbDelay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ProcessKeyDownDelay(e.KeyCode);
        }

        private void TbDelay_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyDownDelay(e.KeyCode);
        }

        private void TbDelay_Leave(object sender, EventArgs e)
        {
            ProcessKeyDownDelay(Keys.Tab);
        }

        private void ProcessKeyDownDelay(Keys keyCode)
        {
            bool isError = false;
            if ((keyCode == Keys.Return) || (keyCode == Keys.Tab))
            {
                if (Tools.IsString_Numberic_Int32(tbDelay.Text) == false)
                {
                    isError = true;
                }
                else
                {
                    int Delay = Tools.ConversionStringtoInt32(tbDelay.Text);
                    myCAN.Delay = Delay;
                }
                if (isError == true)
                {
                    FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    var dialogResult = FlexiMessageBox.Show("Note: Delay to Sent Frame/Cycle (mSec)", "CAN Frame Setting", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tbDelay.BackColor = Color.LightPink;
                    lbHexError.Visible = true;
                    lbHexError.Text = "DLY ERROR";
                    btnOK.Enabled = false;
                    if (keyCode == Keys.Tab)            // Go Back to correct. 
                        tbDelay.Focus();
                }
                else
                {
                    tbDelay.BackColor = Color.White;
                    CanFrame_Refresh();
                    lbHexError.Visible = false;
                    btnOK.Enabled = true;
                }
            }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= CYCLE Editor
        //##############################################################################################################

        #region //==================================================Process CYCLE Text Box
        private void TbCycle_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ProcessKeyDownCycle(e.KeyCode);
        }

        private void TbCycle_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyDownCycle(e.KeyCode);
        }
        private void TbCycle_Leave(object sender, EventArgs e)
        {
            ProcessKeyDownCycle(Keys.Tab);
        }
        private void ProcessKeyDownCycle(Keys keyCode)
        {
            bool isError = false;
            if ((keyCode == Keys.Return) || (keyCode == Keys.Tab))
            {
                if (Tools.IsString_Numberic_Int32(tbCycle.Text) == false)
                {
                    isError = true;
                }
                else
                {
                    int Cycle = Tools.ConversionStringtoInt32(tbCycle.Text);
                    if (Cycle == 0)
                        Cycle = 1;
                    myCAN.CyclePeriod = Cycle;
                    
                }
                if (isError == true)
                {
                    FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    var dialogResult = FlexiMessageBox.Show("Note: Repeat Cycle Process in mSec. Min=1", "CAN Frame Setting", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tbCycle.BackColor = Color.LightPink;
                    lbHexError.Visible = true;
                    lbHexError.Text = "CYC ERROR";
                    btnOK.Enabled = false;
                    if (keyCode == Keys.Tab)            // Go Back to correct. 
                        tbCycle.Focus();
                }
                else
                {
                    CanFrame_Refresh();
                    tbCycle.BackColor = Color.White;
                    lbHexError.Visible = false;
                    btnOK.Enabled = true;
                }
            }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= COUNTS Editor
        //##############################################################################################################

        #region //==================================================Process Counts Text Box
        private void TbCounts_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ProcessKeyDownCount(e.KeyCode);
        }

        private void TbCounts_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyDownCount(e.KeyCode);
        }
        private void TbCounts_Leave(object sender, EventArgs e)
        {
            ProcessKeyDownCount(Keys.Tab);
        }

        private void ProcessKeyDownCount(Keys keyCode)
        {
            bool isError = false;
            if ((keyCode == Keys.Return) || (keyCode == Keys.Tab))
            {
                if (Tools.IsString_Numberic_Int32(tbCounts.Text) == false)
                {
                    isError = true;
                }
                else
                {
                    int Counts = Tools.ConversionStringtoInt32(tbCounts.Text);
                    myCAN.CycleNumber = Counts;
                }
                if (isError == true)
                {
                    FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    var dialogResult = FlexiMessageBox.Show("Note: 0 = Infinite Cycle, otherwise cycle until expired", "CAN Frame Setting", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tbCounts.BackColor = Color.LightPink;
                    lbHexError.Visible = true;
                    lbHexError.Text = "CYC ERROR";
                    btnOK.Enabled = false;
                    if (keyCode == Keys.Tab)            // Go Back to correct. 
                        tbCounts.Focus();
                }
                else
                {
                    CanFrame_Refresh();
                    tbCounts.BackColor = Color.White;
                    lbHexError.Visible = false;
                    btnOK.Enabled = true;
                }
            }
        }

        #endregion

        //##############################################################################################################
        //============================================================================================= Comment Editor
        //##############################################################################################################

        #region //==================================================Process Comment Text Box
        private void TbComment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            ProcessKeyDownComment(e.KeyCode);
        }

        private void TbComment_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyDownComment(e.KeyCode);
        }

        private void TbComment_Leave(object sender, EventArgs e)
        {
            ProcessKeyDownComment(Keys.Tab);
        }

        private void ProcessKeyDownComment(Keys keyCode)
        {
            if ((keyCode == Keys.Return) || (keyCode == Keys.Tab))
            {
                myCAN.sComment = tbComment.Text;
                CanFrame_Refresh();
                lbHexError.Visible = false;
                btnOK.Enabled = true;
            }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= CountEnable Editor
        //##############################################################################################################

        #region //==================================================Process CountEnable Text Box
        private void ChCycle_MouseClick(object sender, MouseEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            myCAN.isCycleEnable = checkbox.Checked;
            CanFrame_Refresh();
            lbHexError.Visible = false;
            btnOK.Enabled = true;
        }
        #endregion

        //==================================================================================================================================
        //==================================================================================================================================SDO
        //==================================================================================================================================

        #region //==================================================BtnSDOHelp_Click
        private void BtnSDOHelp_Click(object sender, EventArgs e)
        {

        }
        #endregion

        //##############################################################################################################
        //============================================================================================= SDO Editor: D4-D7
        //##############################################################################################################

        #region //==================================================Process SDO: D4-D7 Text Boxes.
        private void TbSDOData_PrevieKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessSDOKeyDown(e.KeyCode, textBox);
        }

        private void TbSDOData_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessSDOKeyDown(e.KeyCode, textBox);
        }
        private void TbDSOData_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessSDOKeyDown(Keys.Tab, textBox);
        }

        private void ProcessSDOKeyDown(Keys keyCode, TextBox textBox)
        {
            bool isAllHex = true;
            if ((keyCode == Keys.Return) || (keyCode == Keys.Tab))
            {
                for (int i = 0; i < (myCAN.DLC-4); i++)     // Limit to D4 to D7.
                {
                    mySDOData[i].Text.ToUpper();
                    if (Tools.IsString_Hex(mySDOData[i].Text) == false)
                    {
                        mySDOData[i].BackColor = Color.LightPink;
                        isAllHex = false;
                        if (keyCode == Keys.Tab)
                        {
                            int ch = textBox.TabIndex - 304;
                            mySDOData[ch - 1].Focus();                 // Go Back to correct. 
                        }
                    }
                    else
                    {
                        myCAN.DATA[i+4] = Tools.HexStringtoByte(mySDOData[i].Text);
                        mySDOData[i].BackColor = Color.White;
                    }
                }
                if (isAllHex == true)
                {
                    CanFrame_Refresh_SDO_UpdateD0D7();
                }
                else
                {
                    lbHexError.Visible = true;
                    btnOK.Enabled = false;
                    lbHexError.Text = "HEX ERROR";
                }
            }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= SDO Editor: Sub Index
        //##############################################################################################################

        #region //==================================================Process SDO: Sub Index
        private void tbSDOSub_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessSubIndexKeyDown(e.KeyCode, textBox);
        }

        private void tbSDOSub_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessSubIndexKeyDown(e.KeyCode, textBox);
        }
        private void TbSDOSub_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessSubIndexKeyDown(Keys.Tab, textBox);
        }
        private void ProcessSubIndexKeyDown(Keys keyCode, TextBox textBox)
        {
            bool isError = false;
            if ((keyCode == Keys.Return) || (keyCode == Keys.Tab))
            {
                if (Tools.IsString_Hex(tbSDOSub.Text) == false)
                {
                    isError = true;
                }
                else
                {
                    Byte Sub = Tools.HexStringtoByte(tbSDOSub.Text);
                    myCAN.DATA[3] = Sub;
                }
                if (isError == true)
                {
                    tbSDOSub.BackColor = Color.LightPink;
                    lbHexError.Visible = true;
                    lbHexError.Text = "HEX ERROR";
                    btnOK.Enabled = false;
                    if (keyCode == Keys.Tab)            // Go Back to correct. 
                        tbSDOSub.Focus();
                }
                else
                {
                    tbSDOSub.BackColor = Color.White;
                    CanFrame_Refresh_SDO_UpdateD0D7();
                    lbHexError.Visible = false;
                    btnOK.Enabled = true;
                }
            }
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= SDO Editor: Main Index
        //##############################################################################################################

        #region //==================================================Process SDO:Main Index
        private void TbBSDOIndex_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessMainIndexKeyDown(e.KeyCode, textBox);
        }

        private void TbBSDOIndex_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessMainIndexKeyDown(e.KeyCode, textBox);
        }
        private void TbSDOIndex_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessMainIndexKeyDown(Keys.Tab, textBox);
        }
        private void ProcessMainIndexKeyDown(Keys keyCode, TextBox textBox)
        {
            bool isError = false;
            if ((keyCode == Keys.Return) || (keyCode == Keys.Tab))
            {
                if (Tools.IsString_Hex(tbSDOIndex.Text) == false)
                {
                    isError = true;
                }
                else
                {
                    Byte[] Index = Tools.StrToHexBytesNonASCII(tbSDOIndex.Text);
                    myCAN.DATA[1] = Index[1];
                    myCAN.DATA[2] = Index[0];
                }
                if (isError == true)
                {
                    tbSDOIndex.BackColor = Color.LightPink;
                    lbHexError.Visible = true;
                    lbHexError.Text = "HEX ERROR";
                    btnOK.Enabled = false;
                    if (keyCode == Keys.Tab)            // Go Back to correct. 
                        tbSDOIndex.Focus();
                }
                else
                {
                    tbSDOIndex.BackColor = Color.White;
                    CanFrame_Refresh_SDO_UpdateD0D7();
                    lbHexError.Visible = false;
                    btnOK.Enabled = true;
                }
            }
        }
        #endregion


        //##############################################################################################################
        //============================================================================================= SDO Editor: Spec
        //##############################################################################################################

        #region //==================================================Process SDO: Spec 
        private void TbBSDOSpec_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessSpecKeyDown(e.KeyCode, textBox);
        }

        private void TbBSDOSpec_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessSpecKeyDown(e.KeyCode, textBox);
        }
        private void TbSDOSpec_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ProcessSpecKeyDown(Keys.Tab, textBox);
        }
        private void ProcessSpecKeyDown(Keys keyCode, TextBox textBox)
        {
            bool isError = false;
            if ((keyCode == Keys.Return) || (keyCode == Keys.Tab))
            {
                if (Tools.IsString_Hex(tbSDOSpec.Text) == false)
                {
                    isError = true;
                }
                else
                {
                    Byte Spec = Tools.HexStringtoByte(tbSDOSpec.Text);
                    myCAN.DATA[0] = Spec;
                }
                if (isError == true)
                {
                    tbSDOSpec.BackColor = Color.LightPink;
                    lbHexError.Visible = true;
                    lbHexError.Text = "HEX ERROR";
                    btnOK.Enabled = false;
                    if (keyCode == Keys.Tab)            // Go Back to correct. 
                        tbSDOSpec.Focus();
                }
                else
                {
                    tbSDOSpec.BackColor = Color.White;
                    CanFrame_Refresh_SDO_UpdateD0D7();
                    lbHexError.Visible = false;
                    btnOK.Enabled = true;
                }
            }
        }









        #endregion


    }
}
