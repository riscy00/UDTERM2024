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

namespace UDT_Term
{
    public partial class RiscyTimer : Form
    {
        System.Windows.Forms.Timer timer;
        private int iTimerReset;
        private int iTimer;
        public bool forceclose { get; set; }
        string title;
        public delegate void DMFP_Delegate(string title);
        public DMFP_Delegate ddfunctionpointer { get; set; }
        public RiscyTimer()
        {
            InitializeComponent();
        }

        private void RiscyTimer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (forceclose==true)
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Tick -= new EventHandler(timer_Tick);
                    timer.Dispose();
                    timer = null;
                }
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
        }

        public void RunRisyTimer(int peroid, string sTitle)
        {
            forceclose = false;
            this.Show();
            this.TopMost = true;
            title = sTitle;
            this.BackColor = Color.Honeydew;
            lbTimer.BackColor = Color.Honeydew;
            lbTimer.ForeColor = Color.Black;
            ddfunctionpointer = null;
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
        private void timer_Tick(object sender, EventArgs e)
        {
            iTimer--;
            lbTimer.Text = iTimer.ToString();
            if (lbTimer.ForeColor == Color.Black)
                lbTimer.ForeColor = Color.Blue;
            else
                lbTimer.ForeColor = Color.Black;
            if (iTimer <= 5)
            {
                this.BackColor = Color.Red;
                lbTimer.BackColor = Color.Red;
                lbTimer.ForeColor = Color.Yellow;
            }
            if (iTimer==0)
            {
                this.Close();
            }
        }

        public void RunRisyTimer(int peroid, string sTitle, DMFP_Delegate dfunctionpointer)
        {
            forceclose = false;
            title = sTitle;
            this.BackColor = Color.Honeydew;
            lbTimer.BackColor = Color.Honeydew;
            lbTimer.ForeColor = Color.Black;
            ddfunctionpointer = dfunctionpointer;
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

        public void ForceClose()
        {
            forceclose = true;
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            iTimer = iTimerReset;
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
             this.Close();
        }


    }
}
