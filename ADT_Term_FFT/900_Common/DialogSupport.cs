using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
//using Timer = System.Timers.Timer;        // Do not use this!
//using Timer = System.Threading.Timer;
//using Timer = System.Windows.Forms.Timer;

//http://www.abhisheksur.com/2011/03/all-about-net-timers-comparison.html       Article about different timer solution. Do not use System.Timers.Timer. DispatcherTimer fixed thread issue and simple to implements. . 

namespace UDT_Term
{
    class DialogSupport
    {
        Form promptPopUp;
        Form promptDone;
        Form promptError;
        Button btnAdd; 
        DispatcherTimer dispTimer;
        public bool isCloseTimerOut { get; set; }   // Only for PopUpMessageBox
        public bool isTopMostState { get; set; }    // Sometime we do not need topmost, especially debugging. 
        //==================================================================Constructor
        public DialogSupport()
        {
            isCloseTimerOut = false;
            dispTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);     // This solve thread problem with System.Timers.Timer(). 
            isTopMostState = true;
        }

        #region //================================================================PopUpMessageBox()
        // text             Message to be inserted
        // Windowcaption    Window Text
        // autocloseperiod  Automatic closure of window in case of failure or issue, 0 = Not enabled otherwise second.

        public void PopUpMessageBox(string text, string Windowcaption, int autocloseperiod)
        {
            PopUpMessageBox(text, Windowcaption, autocloseperiod, 18F);
        }
        #endregion

        #region //================================================================PopUpMessageBox()
        // text             Message to be inserted
        // Windowcaption    Window Text
        // autocloseperiod  Automatic closure of window in case of failure or issue, 0 = Not enabled otherwise second.
        // Font Size        18F default otherwise other value.
        public void PopUpMessageBox(string text, string Windowcaption, int autocloseperiod, float fontsize)
        {
            Label textLabel;
            isCloseTimerOut = false;
            if (promptPopUp != null)
            {
                promptPopUp.Close();
                promptPopUp.Dispose();
            }

            promptPopUp = new Form()
            {
                Width = 700,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowIcon = false,
                TopMost = isTopMostState,
                Opacity = 60,
                ControlBox = false,
                BackColor = System.Drawing.Color.SeaShell,
                Text = Windowcaption,
                StartPosition = FormStartPosition.CenterScreen
            };
            btnAdd = new Button()
            {
                //BackColor = Color.
                Text = "Close",
                Location = new System.Drawing.Point(620, 5),
                Size = new System.Drawing.Size(60, 25),

            };
            //------------------------------------Added 18/10/18
            if (text.Contains("ERR") | text.Contains("#E"))
            {
                promptPopUp.BackColor = System.Drawing.Color.MistyRose;
            }
            //------------------------------------Added 18/10/18
            else if (text.Contains("WARN") | text.Contains("+W")| text.Contains("Warn"))
            {
                promptPopUp.BackColor = System.Drawing.Color.Yellow;
            }
            //------------------------------------
            this.btnAdd.Click += new EventHandler(EE_ButtonClosePanel);
            textLabel = new Label() { Left = 50, Top = 33, Text = text, Width = 650, Height = 100 };
            textLabel.Font = new System.Drawing.Font("Arial", fontsize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            //prompt.Controls.Add(textBox);
            promptPopUp.Controls.Add(textLabel);
            promptPopUp.Controls.Add(btnAdd);
            promptPopUp.Show();
            //---------------------------------------Setup Timer to ensure it is closed.
            if (autocloseperiod == 0)               // Do not use this feature.
                return;
            dispTimer.Interval = TimeSpan.FromSeconds(autocloseperiod);
            dispTimer.Tick += new EventHandler(EE_timerClosePanel);
            dispTimer.IsEnabled = true;
        }
        #endregion


        #region //================================================================DoneThenCloseMessageBox
        public void DoneThenCloseMessageBox(int autocloseperiod)
        {
            promptDone = new Form()
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowIcon = false,
                TopMost = true,
                Opacity = 80,
                ControlBox = false,
                BackColor = System.Drawing.Color.Honeydew,
                Text = "Done Message Box",
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 100, Top = 20, Text = "Done!", Width = 200, Height = 80 };
            textLabel.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            //prompt.Controls.Add(textBox);
            promptDone.Controls.Add(textLabel);
            promptDone.Show();
            //---------------------------------------Setup Timer to ensure it is closed.
            if (autocloseperiod == 0)               // Do not use this feature.
                return;

            dispTimer.Interval = TimeSpan.FromSeconds(autocloseperiod);
            dispTimer.Tick += new EventHandler(EE_timerClosePanel);
            dispTimer.IsEnabled = true;
        }
            #endregion

        #region //================================================================ErrorThenCloseMessageBox
        public void ErrorThenCloseMessageBox(int autocloseperiod)
        {
            promptError = new Form()
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowIcon = false,
                TopMost = true,
                Opacity = 80,
                ControlBox = false,
                BackColor = System.Drawing.Color.MistyRose,
                Text = "Error Message Box",
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 100, Top = 20, Text = "# Error #", Width = 200, Height = 80 };
            textLabel.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            //prompt.Controls.Add(textBox);
            promptError.Controls.Add(textLabel);
            promptError.Show();
            //---------------------------------------Setup Timer to ensure it is closed.
            if (autocloseperiod == 0)               // Do not use this feature.
                return;
            dispTimer.Interval = TimeSpan.FromSeconds(autocloseperiod);
            dispTimer.Tick += new EventHandler(EE_timerClosePanel);
            dispTimer.IsEnabled = true;
        }
        #endregion

        #region //================================================================Close
        public void Close()
        {
            dispTimer.IsEnabled = false;
            try
            {
                if (promptDone != null)
                {
                    promptDone.Close();
                    promptDone.Dispose();
                }
                if (promptError != null)
                {
                    promptError.Close();
                    promptError.Dispose();
                }
                if (promptPopUp != null)
                {
                    promptPopUp.Close();
                    promptPopUp.Dispose();
                }
            }
            catch { }
            
            dispTimer.Tick -= EE_timerClosePanel;       // Unsubscribe
            dispTimer.Tick -= EE_ButtonClosePanel; // Unsubscribe
            isCloseTimerOut = false;
        }
        #endregion

        #region //================================================================EE_timerClosePanel
        private void EE_timerClosePanel(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region //================================================================EE_ButtonClosePanel
        private void EE_ButtonClosePanel(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

    }

}
