using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UDT_Term_FFT;
using Newtonsoft.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace UDT_Term
{
    public partial class RiscyTimerII : Form
    {
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        MainProg myMainProg;
        System.Windows.Forms.Timer timer;
        private int iTimerReset;
        private int iTimer;
        public bool forceclose { get; set; }
        string title;
        public delegate void DMFP_Delegate(string title);
        public DMFP_Delegate ddfunctionpointer { get; set; }

        //-------------------------------------------AWS

        #region//================================================================Reference
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }

        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================Window.
        //##############################################################################################################
        public RiscyTimerII()
        {
            InitializeComponent();
        }

        #region //================================================================RiscyTimerII_Show
        public void RiscyTimerII_Show()
        {
            this.BringToFront();
            this.TopMost = false;
            this.WindowState = FormWindowState.Normal;
            myGlobalBase.RiscyTimerII_OpenForm = true;
            this.Visible = true;
            this.Show();
        }
        #endregion

        #region //================================================================RiscyTimerII_Load
        private void RiscyTimerII_Load(object sender, EventArgs e)
        {
            lbTimer.Text = iTimer.ToString();
            btnReset.Text = "START";
            myGlobalBase.RiscyTimerII_OpenForm = true;
            tbPeriod.Text = myGlobalBase.RiscyTimerII_Period.ToString();
            tbPhoneNumber.Text = myGlobalBase.RiscyTimerII_PhoneNoSMS.ToString();
            cbSendSMS.Checked = myGlobalBase.RiscyTimerII_SMSEnabled;
            cbRepeat.Checked = myGlobalBase.RiscyTimerII_Repeat;
            this.BringToFront();
            this.TopMost = false;
            this.WindowState = FormWindowState.Normal;
            this.Visible = true;
            this.Show();
        }
        #endregion

        #region //================================================================RiscyTimer_FormClosing
        private void RiscyTimer_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            myGlobalBase.RiscyTimerII_OpenForm = false;
            if (forceclose == true)
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Tick -= new EventHandler(timer_Tick);
                    timer.Dispose();
                    timer = null;
                }
                this.Visible = false;
                this.Hide();
                forceclose = false;
                return;
            }
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= new EventHandler(timer_Tick);
                timer.Dispose();
                timer = null;
                for (int i = 0; i < 2; i++)
                {
                    Console.Beep(1000, 50);
                    Thread.Sleep(50);
                    Console.Beep(2500, 25);
                    Thread.Sleep(50);
                    Console.Beep(750, 50);
                    Thread.Sleep(50);
                    Console.Beep(2000, 25);
                    Thread.Sleep(50);
                }
            }
            if (ddfunctionpointer != null)
                ddfunctionpointer(title);       // Callback 
            this.Visible = false;
            this.Hide();
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================RiscyTimerII
        //##############################################################################################################

        #region //================================================================RunRisyTimer
        public void RunRisyTimer(int peroid, string sTitle)
        {
            //-------------------------------------
            if (btnReset.Text == "START")
                btnReset.Text = "RESET";
            //-------------------------------------
            forceclose = false;
            this.Show();
            this.TopMost = true;
            title = sTitle;
            this.BackColor = Color.Honeydew;
            lbTimer.BackColor = Color.Honeydew;
            lbTimer.ForeColor = Color.Black;
            ddfunctionpointer = null;
            tbPeriod.Text = peroid.ToString();
            iTimerReset = peroid;
            iTimer = peroid;
            lbTimer.Text = iTimer.ToString();
            lbSource.Text = sTitle;
            if (timer == null)
                timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            timer.Interval = (1000) * (1);              // Timer will tick event second
            timer.Enabled = true;                       // Enable the timer
            timer.Start();
        }
        #endregion

        #region //================================================================RunRisyTimer
        public void RunRisyTimer(int peroid, string sTitle, DMFP_Delegate dfunctionpointer)
        {
            //-------------------------------------
            if (btnReset.Text == "START")
                btnReset.Text = "RESET";
            //-------------------------------------
            forceclose = false;
            title = sTitle;
            this.BackColor = Color.Honeydew;
            lbTimer.BackColor = Color.Honeydew;
            lbTimer.ForeColor = Color.Black;
            ddfunctionpointer = dfunctionpointer;
            tbPeriod.Text = peroid.ToString();
            iTimerReset = peroid;
            iTimer = peroid;
            lbTimer.Text = iTimer.ToString();
            lbSource.Text = sTitle;
            if (timer == null)
                timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            timer.Interval = (1000) * (1);              // Timer will tick event second
            timer.Enabled = true;                       // Enable the timer
            timer.Start();
        }
        #endregion

        #region //================================================================timer_Tick
        private void timer_Tick(object sender, EventArgs e)
        {
            iTimer--;
            lbTimer.Text = iTimer.ToString();
            //-------------------------------------- Alternate colour for max attention. 
            if (lbTimer.ForeColor == Color.Black)
                lbTimer.ForeColor = Color.Blue;
            else
                lbTimer.ForeColor = Color.Black;
            //-------------------------------------- Timer nearly expiring. 
            if (iTimer <= 5)
            {
                this.BackColor = Color.Red;
                lbTimer.BackColor = Color.Red;
                lbTimer.ForeColor = Color.Yellow;
            }
            //-------------------------------------- Timer expired, time for action. 
            if (iTimer == 0)
            {
                this.Refresh();
                if (cbSendSMS.Checked == true)      // Send SMS message now.
                {
                    string PN = tbPhoneNumber.Text;     // Must be E164 style  See https://support.twilio.com/hc/en-us/articles/223183008-Formatting-International-Phone-Numbers
                    if ((PN.Contains("+")) & (PN.Length > 11))
                    {
                        tbPhoneNumber.ForeColor = Color.Black;
                        myGlobalBase.RiscyTimerII_PhoneNoSMS = PN;
                        AWS_SendSMS(PN, "UDT Timer Alert Event");
                    }
                    else
                    {
                        tbPhoneNumber.ForeColor = Color.Red;        // Error PN. 
                    }
                }
                if (cbRepeat.Checked == true)       // Repeat timer.
                {
                    this.BackColor = Color.Honeydew;
                    lbTimer.BackColor = Color.Honeydew;
                    lbTimer.ForeColor = Color.Black;
                    int Per = Tools.ConversionStringtoInt32(tbPeriod.Text);
                    if (Per == -975579)
                    {
                        tbPeriod.ForeColor = Color.Red;
                    }
                    if (Per >= 1)
                    {
                        myGlobalBase.RiscyTimerII_Period = Per;
                        lbTimer.Text = myGlobalBase.RiscyTimerII_Period.ToString();
                        iTimer = myGlobalBase.RiscyTimerII_Period;
                    }
                    return;
                }
                //--------------------------------- This is one shot or cease 
                btnReset.Text = "START";
                this.Close();
            }
        }
        #endregion

        #region //================================================================ForceClose
        public void ForceClose()
        {
            forceclose = true;
            this.Close();
        }
        #endregion

        #region //================================================================btnReset_Click
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (btnReset.Text == "START")
            {
                int Per = Tools.ConversionStringtoInt32(tbPeriod.Text);
                if (Per == -975579)
                {
                    tbPeriod.ForeColor = Color.Red;
                    return;
                }
                if (Per == 0)
                    return;
                this.BackColor = Color.Honeydew;
                lbTimer.BackColor = Color.Honeydew;
                lbTimer.ForeColor = Color.Black;
                tbPeriod.ForeColor = Color.Black;
                myGlobalBase.RiscyTimerII_Period = Per;
                lbTimer.Text = myGlobalBase.RiscyTimerII_Period.ToString();
                RunRisyTimer(Per, "UserSetup");
                return;
            }
            if (btnReset.Text == "RESET")
            {
                btnReset.Text = "START";
                int Per = Tools.ConversionStringtoInt32(tbPeriod.Text);
                if (Per == -975579)
                {
                    tbPeriod.ForeColor = Color.Red;
                    
                    return;
                }
                if (Per == 0)
                    return;
                
                this.BackColor = Color.Honeydew;
                lbTimer.BackColor = Color.Honeydew;
                lbTimer.ForeColor = Color.Black;
                tbPeriod.ForeColor = Color.Black;
                myGlobalBase.RiscyTimerII_Period = Per;
                lbTimer.Text = myGlobalBase.RiscyTimerII_Period.ToString();
                //----------------------------------
                if (timer != null)
                {
                    timer.Stop();
                    timer = null;
                }
            }
        }
        #endregion

        #region //================================================================btnDone_Click
        private void btnDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        //##############################################################################################################
        //=============================================================================================AWS SNS
        //##############################################################################################################
        //https://dzone.com/articles/amazon-sns-simple-notification-service-using-visua
        //https://support.twilio.com/hc/en-us/articles/223183008-Formatting-International-Phone-Numbers
        // Refer to E:\010_WorkAOT\010MyGit19\001-UDTProtocol Document: 301-Master-UDT Protocol_1A.docx for notes. 
        #region //================================================================AWS_SendSMS
        // Refer to 
        private void AWS_SendSMS(string args, string message)       
        {
            // US phone numbers must be in the correct format:
            // +1 (nnn) nnn-nnnn OR +1nnnnnnnnnn
            string number = args;
            message += ":"+DateTime.Now.ToShortTimeString();

            var client = new AmazonSimpleNotificationServiceClient(region: Amazon.RegionEndpoint.USWest2);
            var request = new PublishRequest
            {
                Message = message,
                PhoneNumber = number
            };

            try
            {
                var response = client.Publish(request);

                Console.WriteLine("Message sent to " + number + ":");
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception publishing request:");
                Console.WriteLine(ex.Message);
            }
        }
        #endregion
//      Make sure to include this key in app.config in project folder. This is IAM = Riscy2019 
//             </userSettings>
//               <appSettings>
//                 <add key = "AWSAccessKey" value="AKIA2I2EEP4NI3IJFWX6" />
//                 <add key = "AWSSecretKey" value="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" />
//                 <add key = "AWSRegion" value="us-west-2" />
//               </appSettings>
//             </configuration>
    }
}
