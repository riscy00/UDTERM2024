using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;

namespace UDT_Term_FFT
{
    public partial class HRM2300 : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        Mag_Data myMagData;
        Mag_Calibration myMagCal;
        //----------------------------------------------------------------

        //-----------------------------------------------------------------
        public int SampleRate { get; set; }
        public int HMR2300Test_Pointer { get; set; }    // Test array pointer

        #region //==================================================HMR2300Test Array
        public string[] HMR2300Test = new string[] {
            "- 5,433  - 4,452  - 9,450  ",
            "- 6,105  - 2,567  - 8,953  ",
            "- 6,293  -   777  - 8,644  ",
            "- 6,157      919  - 8,512  ",
            "- 5,164    3,491  - 8,453  ",
            "- 3,646    5,373  - 8,717  ",
            "    437    7,086  -10,551  ",
            "  2,087    6,843  -11,575  ",
            "- 2,797   10,305      960  ",
            "- 3,872    9,260  - 2,953  ",
        };
        #endregion

        #region//================================================================Reference

        //-----------------------------------------------------------------
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
                case (50):
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
        #endregion
        public HRM2300()
        {
            InitializeComponent();
            HMR2300Test_Pointer = 0;
            if (myMagData == null)
                myMagData = new Mag_Data();
            if (myMagCal == null)
                myMagCal = new Mag_Calibration();
            //--------------------------Default to mGuass.
            myMagData.DataFormat = 2;
        }

        //#####################################################################################################
        //###################################################################################### Form Management
        //#####################################################################################################

        public Mag_Data HRM2300_MagDataRef()
        {
            return myMagData;
        }

        #region //================================================================HRM2300_Load
        private void HRM2300_Load(object sender, EventArgs e)
        {
            SampleRate = 10;
            cbSelect_mG.Checked = true;
            cbSelect_uT.Checked = false;
            tbHMR_ReadOurX.Text = "0";
            tbHMR_ReadOurY.Text = "0";
            tbHMR_ReadOurZ.Text = "0";

        }
        #endregion

        #region //================================================================HRM2300_FormClosing
        private void HRM2300_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            myGlobalBase.TV_isHMR2300Open = false;                  // Cease Survey CVS Terminal mode.
            myGlobalBase.HMR_HideTextDisplay = false;               // Enable text display. 
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region //================================================================HMR2300_Show
        public void HMR2300_Show()
        {
            this.BringToFront();
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            this.Size = new System.Drawing.Size(483, 244);

            myGlobalBase.TV_isHMR2300Open = true;
            myGlobalBase.HMR_HideTextDisplay = true;
            this.Visible = true;
            this.Show();
        }
        #endregion

        #region //==================================================HMR_Update_Readout
        public delegate void HMR_Update_Readout_StartDelegate(string data);
        //Solution: http://www.dreamincode.net/forums/topic/188209-cross-thread-calls-made-easy/ 
        public void HMR_Update_Readout(string data)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new HMR_Update_Readout_StartDelegate(HMR_Update_Readout), new object[] { data });
                return;
            }
            HMR_FrametoMagData(data);
            //-----------------------------------Convert Data to results (mG)
            switch (myMagData.DataFormat)
            {
                case (0):       // This format is not saved to data. 
                    {
                        tbHMR_ReadOurX.Text = myMagData.sMagRaw.X;
                        tbHMR_ReadOurY.Text = myMagData.sMagRaw.Y;
                        tbHMR_ReadOurZ.Text = myMagData.sMagRaw.Z;
                        tbInclination.Text = "";
                        tbDeclination.Text = "";
                        tbNorthX.Text = "";
                        tbWestY.Text = "";
                        tbIntensity.Text = "";
                        break;
                    }
                case (1):   //uT
                    {
                        tbHMR_ReadOurX.Text = myMagData.MagRaw.X.ToString("N3") + "uT";
                        tbHMR_ReadOurY.Text = myMagData.MagRaw.Y.ToString("N3") + "uT";
                        tbHMR_ReadOurZ.Text = myMagData.MagRaw.Z.ToString("N3") + "uT";
                        HMR2300_Math_CalculateAll();
                        //HMR_Compass_Readout();
                        tbInclination.Text = Tools.Math_RadianToDegree(myMagData.dInclination).ToString("N1");
                        tbDeclination.Text = Tools.Math_RadianToDegree(myMagData.dDeclination).ToString("N1");
                        tbNorthX.Text = myMagData.dNorthX.ToString("N3");
                        tbWestY.Text = myMagData.dEastY.ToString("N3");
                        tbIntensity.Text = myMagData.dIntensity.ToString("N3");
                        break;
                    }
                case (2):   //mG
                    {
                        tbHMR_ReadOurX.Text = myMagData.MagRaw.X.ToString("N2") + "mG";
                        tbHMR_ReadOurY.Text = myMagData.MagRaw.Y.ToString("N2") + "mG";
                        tbHMR_ReadOurZ.Text = myMagData.MagRaw.Z.ToString("N2") + "mG";
                        HMR2300_Math_CalculateAll();
                        //HMR_Compass_Readout();
                        tbInclination.Text = Tools.Math_RadianToDegree(myMagData.dInclination).ToString("N1");
                        tbDeclination.Text = Tools.Math_RadianToDegree(myMagData.dDeclination).ToString("N1");
                        tbNorthX.Text = myMagData.dNorthX.ToString("N3");
                        tbWestY.Text = myMagData.dEastY.ToString("N3");
                        tbIntensity.Text = myMagData.dIntensity.ToString("N3");
                        break;
                    }
                default:
                    break;

            }
        }
        #endregion

        #region //==================================================HMR_FrametoMagData
        // Convert message frame to discrete double and string variables. 
       // public delegate void HMR_FrametoMagDataDelegate(string data);
        //Solution: http://www.dreamincode.net/forums/topic/188209-cross-thread-calls-made-easy/ 
        public void HMR_FrametoMagData(string data)
        {
            //if (this.InvokeRequired)
            //{
            //    this.BeginInvoke(new HMR_FrametoMagDataDelegate(HMR_FrametoMagData), new object[] { data });
            //    return;
            //}
            try
            {
                string sxx, syy, szz;

                try
                {
                    sxx = data.Substring(0, 8);
                    syy = data.Substring(9, 8);
                    szz = data.Substring(18, 8);
                }
                catch
                {
                    sxx = "0.0";
                    syy = "0.0";
                    szz = "0.0";
                }
                //------------------------------------Remove comma and whitespace.
                sxx = sxx.Replace(",", string.Empty);
                syy = syy.Replace(",", string.Empty);
                szz = szz.Replace(",", string.Empty);
                myMagData.sMagRaw.X = sxx.Replace(" ", string.Empty);
                myMagData.sMagRaw.Y = syy.Replace(" ", string.Empty);
                myMagData.sMagRaw.Z = szz.Replace(" ", string.Empty);
                //-----------------------------------Translate string to Int32.
                myMagData.MagRaw.X = Tools.ConversionStringtoDouble(myMagData.sMagRaw.X);
                myMagData.MagRaw.Y = Tools.ConversionStringtoDouble(myMagData.sMagRaw.Y);
                myMagData.MagRaw.Z = Tools.ConversionStringtoDouble(myMagData.sMagRaw.Z);
                switch (myMagData.DataFormat) //Math.Round(inputValue, 2, MidpointRounding.AwayFromZero)
                {
                    case (0): //Raw Data
                    {
                        // Do nothing. 
                        break;
                    }
                    case (1):
                        //mG, 4 decimal point is okay for survey data, make reading easier than long trailing 33333 or 66666, etc. 
                    {
                        myMagData.MagRaw.X = Math.Round((myMagData.MagRaw.X/150.0), 3, MidpointRounding.AwayFromZero);
                        myMagData.MagRaw.Y = Math.Round((myMagData.MagRaw.Y/150.0), 3, MidpointRounding.AwayFromZero);
                        myMagData.MagRaw.Z = Math.Round((myMagData.MagRaw.Z/150.0), 3, MidpointRounding.AwayFromZero);
                        break;
                    }
                    case (2): //mT
                    {
                        myMagData.MagRaw.X = Math.Round((myMagData.MagRaw.X/15.0), 4, MidpointRounding.AwayFromZero);
                        myMagData.MagRaw.Y = Math.Round((myMagData.MagRaw.Y/15.0), 4, MidpointRounding.AwayFromZero);
                        myMagData.MagRaw.Z = Math.Round((myMagData.MagRaw.Z/15.0), 4, MidpointRounding.AwayFromZero);
                        break;
                    }
                    default:
                        break;
                }
            }
            catch
            {
                Debug.Print("###ERR: HMR_FrametoMagData(), Part 1 of the code.");
            }
        }
        #endregion

        #region //==================================================HMR_Compass_Readout
        public delegate void HMR_Compass_Readout_StartDelegate();
        //Solution: http://www.dreamincode.net/forums/topic/188209-cross-thread-calls-made-easy/ 
        public void HMR_Compass_Readout()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new HMR_Compass_Readout_StartDelegate(HMR_Compass_Readout));
                return;
            }
            try
            {
                tbInclination.Text = Tools.Math_RadianToDegree(myMagData.dInclination).ToString("N1");
                tbDeclination.Text = Tools.Math_RadianToDegree(myMagData.dDeclination).ToString("N1");
                tbNorthX.Text = myMagData.dNorthX.ToString("N3");
                tbWestY.Text = myMagData.dEastY.ToString("N3");
                tbIntensity.Text = myMagData.dIntensity.ToString("N3");
            }
            catch { }
            //---------------------------------------------
        }
        #endregion

        #region //==================================================cbSelect_mG_MouseClick
        private void cbSelect_mG_MouseClick(object sender, MouseEventArgs e)
        {
            myMagData.DataFormat = 2;
            cbSelect_mG.Checked = true;
            cbSelect_uT.Checked = false;
            cbRawData.Checked = false;

        }
        #endregion

        #region //==================================================cbSelect_uT_MouseClick
        private void cbSelect_uT_MouseClick(object sender, MouseEventArgs e)
        {
            myMagData.DataFormat = 1;
            cbSelect_mG.Checked = false;
            cbSelect_uT.Checked = true;
            cbRawData.Checked = false;
        }


        #endregion

        #region //==================================================cbRawData_MouseClick
        private void cbRawData_MouseClick(object sender, MouseEventArgs e)
        {
            myMagData.DataFormat = 0;
            cbSelect_mG.Checked = false;
            cbSelect_uT.Checked = false;
            cbRawData.Checked = true;
        }
        #endregion

        #region //==================================================HMR2300_Math_CalculateAll
        public void HMR2300_Math_CalculateAll()
        {
            //---------------------Calibration (to be filled later)
            myMagData.MagCal.X = myMagData.MagRaw.X;
            myMagData.MagCal.Y = myMagData.MagRaw.Y;
            myMagData.MagCal.Z = myMagData.MagRaw.Z;

            //---------------------
            try
            {
                myMagData.dDeclination = Math.Atan(myMagData.MagCal.Y / myMagData.MagCal.X);
                myMagData.dHorizontal = Math.Sqrt((Math.Pow(myMagData.MagCal.X, 2) + Math.Pow(myMagData.MagCal.Y, 2)));
                myMagData.dIntensity = Math.Sqrt((Math.Pow(myMagData.MagCal.X, 2) + Math.Pow(myMagData.MagCal.Y, 2) + Math.Pow(myMagData.MagCal.Z, 2)));
                myMagData.dInclination = Math.Atan(myMagData.MagCal.Z / myMagData.dHorizontal);
                myMagData.dNorthX = myMagData.dHorizontal * Math.Cos(myMagData.dDeclination);
                myMagData.dEastY = myMagData.dHorizontal * Math.Sin(myMagData.dDeclination);
            }
            catch { }
        }
        #endregion

        #region //==================================================btnCompact_Click
        private void btnCompact_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            // 
            // tbInclination
            // 
            this.tbInclination.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbInclination.Location = new System.Drawing.Point(372, 35);
            this.tbInclination.MaximumSize = new System.Drawing.Size(120, 30);
            this.tbInclination.MinimumSize = new System.Drawing.Size(120, 30);
            this.tbInclination.Size = new System.Drawing.Size(120, 36);
            // 
            // tbDeclination
            // 
            this.tbDeclination.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDeclination.Location = new System.Drawing.Point(372, 71);
            this.tbDeclination.MaximumSize = new System.Drawing.Size(120, 30);
            this.tbDeclination.MinimumSize = new System.Drawing.Size(120, 30);
            this.tbDeclination.Size = new System.Drawing.Size(120, 36);
            // 
            // tbNorthX
            // 
            this.tbNorthX.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbNorthX.Location = new System.Drawing.Point(372, 107);
            this.tbNorthX.MaximumSize = new System.Drawing.Size(120, 30);
            this.tbNorthX.MinimumSize = new System.Drawing.Size(120, 30);
            this.tbNorthX.Size = new System.Drawing.Size(120, 36);
            // 
            // tbWestY
            // 
            this.tbWestY.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWestY.Location = new System.Drawing.Point(372, 143);
            this.tbWestY.MaximumSize = new System.Drawing.Size(120, 30);
            this.tbWestY.MinimumSize = new System.Drawing.Size(120, 30);
            this.tbWestY.Size = new System.Drawing.Size(120, 36);
            // 
            // tbIntensity
            // 
            this.tbIntensity.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbIntensity.Location = new System.Drawing.Point(372, 179);
            this.tbIntensity.MaximumSize = new System.Drawing.Size(120, 30);
            this.tbIntensity.MinimumSize = new System.Drawing.Size(120, 30);
            this.tbIntensity.Size = new System.Drawing.Size(120, 36);

            // 
            // tbHMR_ReadOurZ
            // 
            this.tbHMR_ReadOurZ.Font = new System.Drawing.Font("Consolas", 21.75F);
            this.tbHMR_ReadOurZ.Location = new System.Drawing.Point(43, 164);
            this.tbHMR_ReadOurZ.MaximumSize = new System.Drawing.Size(170, 45);
            this.tbHMR_ReadOurZ.MinimumSize = new System.Drawing.Size(170, 45);
            this.tbHMR_ReadOurZ.Size = new System.Drawing.Size(170, 41);
            // 
            // tbHMR_ReadOurY
            // 
            this.tbHMR_ReadOurY.Font = new System.Drawing.Font("Consolas", 21.75F);
            this.tbHMR_ReadOurY.Location = new System.Drawing.Point(43, 96);
            this.tbHMR_ReadOurY.MaximumSize = new System.Drawing.Size(170, 45);
            this.tbHMR_ReadOurY.MinimumSize = new System.Drawing.Size(170, 45);
            this.tbHMR_ReadOurY.Size = new System.Drawing.Size(170, 45);
            // 
            // tbHMR_ReadOurX
            // 
            this.tbHMR_ReadOurX.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbHMR_ReadOurX.Location = new System.Drawing.Point(43, 33);
            this.tbHMR_ReadOurX.MaximumSize = new System.Drawing.Size(170, 45);
            this.tbHMR_ReadOurX.MinimumSize = new System.Drawing.Size(170, 45);
            this.tbHMR_ReadOurX.Size = new System.Drawing.Size(170, 45);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 33);
            this.label1.Size = new System.Drawing.Size(35, 40);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 96);
            this.label2.Size = new System.Drawing.Size(34, 40);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 164);
            this.label3.Size = new System.Drawing.Size(34, 40);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(332, 37);
            this.label4.Size = new System.Drawing.Size(38, 25);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(320, 74);
            this.label5.Size = new System.Drawing.Size(50, 25);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(323, 110);
            this.label6.Size = new System.Drawing.Size(47, 25);
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(323, 145);
            this.label7.Size = new System.Drawing.Size(49, 25);
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(334, 181);
            this.label8.Size = new System.Drawing.Size(36, 25);

            this.ClientSize = new System.Drawing.Size(504, 221);
            this.MaximumSize = new System.Drawing.Size(520, 260);
            this.MinimumSize = new System.Drawing.Size(520, 260);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        #region //==================================================btnLarge_Click
        private void btnLarge_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            this.MaximumSize = new System.Drawing.Size(967, 531);
            this.MinimumSize = new System.Drawing.Size(967, 531);
            this.ClientSize = new System.Drawing.Size(951, 492);
            // 
            // tbHMR_ReadOurZ
            // 
            this.tbHMR_ReadOurZ.BackColor = System.Drawing.SystemColors.Info;
            this.tbHMR_ReadOurZ.Font = new System.Drawing.Font("Consolas", 60F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbHMR_ReadOurZ.Location = new System.Drawing.Point(114, 292);
            this.tbHMR_ReadOurZ.MaximumSize = new System.Drawing.Size(402, 120);
            this.tbHMR_ReadOurZ.MinimumSize = new System.Drawing.Size(402, 120);
            this.tbHMR_ReadOurZ.Size = new System.Drawing.Size(402, 120);
            // 
            // tbHMR_ReadOurY
            // 
            this.tbHMR_ReadOurY.Font = new System.Drawing.Font("Consolas", 60F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbHMR_ReadOurY.Location = new System.Drawing.Point(115, 166);
            this.tbHMR_ReadOurY.MaximumSize = new System.Drawing.Size(402, 120);
            this.tbHMR_ReadOurY.MinimumSize = new System.Drawing.Size(402, 120);
            this.tbHMR_ReadOurY.Size = new System.Drawing.Size(402, 120);
            // 
            // tbHMR_ReadOurX
            // 
            this.tbHMR_ReadOurX.Font = new System.Drawing.Font("Consolas", 60F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbHMR_ReadOurX.Location = new System.Drawing.Point(116, 40);
            this.tbHMR_ReadOurX.MaximumSize = new System.Drawing.Size(402, 120);
            this.tbHMR_ReadOurX.MinimumSize = new System.Drawing.Size(402, 120);
            this.tbHMR_ReadOurX.Size = new System.Drawing.Size(402, 120);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(-2, 43);
            this.label1.Size = new System.Drawing.Size(118, 128);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(-1, 169);
            this.label2.Size = new System.Drawing.Size(113, 128);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Segoe UI", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(-1, 295);
            this.label3.Size = new System.Drawing.Size(113, 128);
            // 
            // tbInclination
            // 
            this.tbInclination.Font = new System.Drawing.Font("Consolas", 48F, System.Drawing.FontStyle.Bold);
            this.tbInclination.Location = new System.Drawing.Point(627, 41);
            this.tbInclination.MinimumSize = new System.Drawing.Size(307, 82);
            this.tbInclination.MaximumSize = new System.Drawing.Size(307, 82);
            this.tbInclination.Size = new System.Drawing.Size(307, 82);
            // 
            // tbDeclination
            // 
            this.tbDeclination.Font = new System.Drawing.Font("Consolas", 48F, System.Drawing.FontStyle.Bold);
            this.tbDeclination.Location = new System.Drawing.Point(627, 129);
            this.tbDeclination.MinimumSize = new System.Drawing.Size(307, 82);
            this.tbDeclination.MaximumSize = new System.Drawing.Size(307, 82);
            this.tbDeclination.Size = new System.Drawing.Size(307, 82);
            // 
            // tbNorthX
            // 
            this.tbNorthX.Font = new System.Drawing.Font("Consolas", 48F, System.Drawing.FontStyle.Bold);
            this.tbNorthX.Location = new System.Drawing.Point(627, 217);
            this.tbNorthX.MinimumSize = new System.Drawing.Size(307, 82);
            this.tbNorthX.MaximumSize = new System.Drawing.Size(307, 82);
            this.tbNorthX.Size = new System.Drawing.Size(307, 82);
            // 
            // tbWestY
            // 
            this.tbWestY.Font = new System.Drawing.Font("Consolas", 48F, System.Drawing.FontStyle.Bold);
            this.tbWestY.Location = new System.Drawing.Point(627, 305);
            this.tbWestY.MinimumSize = new System.Drawing.Size(307, 82);
            this.tbWestY.MaximumSize = new System.Drawing.Size(307, 82);
            this.tbWestY.Size = new System.Drawing.Size(307, 82);
            // 
            // tbIntensity
            // 
            this.tbIntensity.Font = new System.Drawing.Font("Consolas", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbIntensity.Location = new System.Drawing.Point(627, 393);
            this.tbIntensity.MinimumSize = new System.Drawing.Size(307, 82);
            this.tbIntensity.MaximumSize = new System.Drawing.Size(307, 82);
            this.tbIntensity.Size = new System.Drawing.Size(307, 82);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(561, 44);
            this.label4.Size = new System.Drawing.Size(64, 45);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(539, 132);
            this.label5.Size = new System.Drawing.Size(85, 45);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(545, 221);
            this.label6.Size = new System.Drawing.Size(80, 45);
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(540, 308);
            this.label7.Size = new System.Drawing.Size(85, 45);
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(563, 396);
            this.label8.Size = new System.Drawing.Size(61, 45);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        #region //==================================================btnStayOnTop_Click
        private void btnStayOnTop_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
        }
        #endregion

        #region //==================================================Template
        #endregion

        //#####################################################################################################
        //###################################################################################### Class Mag_Data
        //#####################################################################################################

        #region --------------------------------------------------------Class Mag_Data
        [Serializable]
        public class Mag_Data
        {
            public sVector3 sMagRaw;       // Raw Data in string
            public Vector3 MagRaw;         // uT or mG. 
            public Vector3 MagCal;         // Calibrated data. 
            public fVector3 fMagCal;       // Calibrated data in float format for charting. 

            //======================================================Getter/Setter f===
            public int DataFormat { get; set; }             // 0= Raw, 1 = uT, 2 = mG
            public double dDeclination { get; set; }        // Radian
            public double dInclination { get; set; }        // Radian
            public double dHorizontal { get; set; }         // uT or mG
            public double dNorthX { get; set; }             // uT or mG
            public double dEastY { get; set; }              // uT or mG
            public double dIntensity { get; set; }          // uT or mG

            // =====================================================constructor
            public Mag_Data()
            {
                sMagRaw = new sVector3();
                MagRaw = new Vector3();
                MagCal = new Vector3();
            }
        }
        #endregion

        //#####################################################################################################
        //###################################################################################### Class Mag_Calibration
        //#####################################################################################################

        #region --------------------------------------------------------Class Mag_Calibration
        public class Mag_Calibration
        {
            public Vector3 MagIn;                   // Input Data
            public Vector3 MagCalibrated;           // Calibrated data. 
            //======================================================Getter/Setter f===


            // =====================================================constructor
            public Mag_Calibration()
            {
                MagIn = new Vector3();
                MagCalibrated = new Vector3();
            }
        }

        #endregion

    }
}
