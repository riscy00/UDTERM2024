using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDT_Term_FFT
{
   
    public partial class FFT_DataEditor : Form
    {

        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        FFTWindow myFFTWindow;

        private bool m_bUpdateFromFFTWindow;
        private UInt32 m_iFFTElementCount;
        private UInt32 m_iFFTSampleRate;
        private UInt32 m_iFFTYScale;
        private UInt32 m_iFFTStatusWord;

        private string m_sFFTFrame;

        #region//================================================================Reference
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }

        public void MyFFTWindow(FFTWindow myFFTWindowRef)
        {
            myFFTWindow = myFFTWindowRef;
        }

        #endregion

        public FFT_DataEditor()
        {
            InitializeComponent();
            //--------------------------------------Themes


            m_bUpdateFromFFTWindow = false;
            //--------------------------------------------------------------------------------------Add Copy/Paste in context menu. 
            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem menuItem = new System.Windows.Forms.MenuItem("Copy");
            menuItem.Click += new EventHandler(CopyAction);
            contextMenu.MenuItems.Add(menuItem);
            menuItem = new System.Windows.Forms.MenuItem("Paste");
            menuItem.Click += new EventHandler(PasteAction);
            contextMenu.MenuItems.Add(menuItem);
            rtbDataBox.ContextMenu = contextMenu;
            rtbDataBox.Font = new Font("Consolas", 12, FontStyle.Regular);
            rtbDataBox.ForeColor = Color.Yellow;
            rtbDataBox.BackColor = System.Drawing.Color.DarkSlateGray;
            // Background Control.
            this.Invalidate();

        }
        #region//==================================================rtbTerm_MouseUp
        void CopyAction(object sender, EventArgs e)
        {
            if (rtbDataBox.SelectedText!="")
                Clipboard.SetText(rtbDataBox.SelectedText);
        }

        void PasteAction(object sender, EventArgs e)
        {
            PasteClipUpdate();
        }
        #endregion

        #region//==================================================rtbDataBox_KeyDown
        private void rtbDataBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                PasteClipUpdate();
                e.SuppressKeyPress=true;       // avoid repeat
            }
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                if (rtbDataBox.SelectedText!="")
                    Clipboard.SetText(rtbDataBox.SelectedText);
                e.SuppressKeyPress = true; 
            }
            base.OnKeyDown(e);
            rtbDataBox.Font = new Font("Consolas", 12, FontStyle.Regular);
            rtbDataBox.ForeColor = Color.Yellow;
            rtbDataBox.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Invalidate();
        }
        #endregion

        #region //=========================================FFT_UpdateData
        //myFFT_DataEditor.FFT_UpdateData(FFTFrame, m_iFFTElementCount, m_iFFTSampleRate, m_iFFTYScale, m_iFFTStatusWord);
        public void FFT_UpdateData(string FFTFrame, UInt32 FFTElementCount, UInt32 FFTSamplerate, UInt32 FFTYScale, UInt32 StatusWord)
        {
            if (m_bUpdateFromFFTWindow == false)        // Do not update this window box.
                return; 
            m_sFFTFrame = FFTFrame;
            m_iFFTElementCount = FFTElementCount;
            m_iFFTSampleRate = FFTSamplerate;
            m_iFFTYScale = FFTYScale;
            m_iFFTStatusWord = StatusWord;
            tbxSampleRate.Text = m_iFFTSampleRate.ToString();
            tbxFFTElement.Text = m_iFFTElementCount.ToString();
            tbxYScale.Text = m_iFFTYScale.ToString();
            rtbDataBox.SuspendLayout();
            rtbDataBox.Font = new Font("Consolas", 12, FontStyle.Regular);
            rtbDataBox.ForeColor = Color.Yellow;
            rtbDataBox.BackColor = System.Drawing.Color.DarkSlateGray;
            rtbDataBox.Clear();                         // Clear text
            rtbDataBox.AppendText(FFTFrame);            // Append text
            rtbDataBox.ResumeLayout();
            this.Invalidate();                          // Update Window
        }
        #endregion

        #region //=========================================cbx_UpdateFromFFTWindow_CheckedChanged
        private void cbx_UpdateFromFFTWindow_CheckedChanged(object sender, EventArgs e)
        {
            if (m_bUpdateFromFFTWindow==false)
            {
                m_bUpdateFromFFTWindow = true;
                cbx_UpdateFromFFTWindow.Checked = true;
            }
            else
            {
                m_bUpdateFromFFTWindow = false;
                cbx_UpdateFromFFTWindow.Checked = false;
            }

        }
        #endregion

        #region //=========================================btnPasteClips_Click
        private void btnPasteClips_Click(object sender, EventArgs e)
        {
            PasteClipUpdate();
        }
        #endregion

        #region //=========================================PasteClipUpdate
        private void PasteClipUpdate()
        {
            tbxSampleRate.Text = "";
            tbxFFTElement.Text = "";
            tbxYScale.Text = "";
            if (Clipboard.ContainsText(TextDataFormat.Rtf))
            {
                rtbDataBox.Clear();
                rtbDataBox.SelectedRtf = Clipboard.GetData(DataFormats.Rtf).ToString();
                rtbDataBox.SelectAll();
                string myText = rtbDataBox.SelectedText;
                rtbDataBox.Text = myText;
            }
            else
            {
                if (Clipboard.ContainsText())
                {
                    rtbDataBox.Clear(); 
                    rtbDataBox.Text = Clipboard.GetText(TextDataFormat.Text).ToString();
                }
            }
            rtbDataBox.Font = new Font("Consolas", 12, FontStyle.Regular);
            rtbDataBox.ForeColor = Color.Yellow;
            rtbDataBox.BackColor = System.Drawing.Color.DarkSlateGray;
            this.Invalidate();
        }

        #endregion

        #region //=========================================btnSendFFTWindow_Click
        private void btnSendFFTWindow_Click(object sender, EventArgs e)
        {
            myFFTWindow.FFTDataEditor_UpdateFFTChart(rtbDataBox.Text);
        }
        #endregion

        #region //================================================================FFTWindow_FormClosing
        private void FFT_DataEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true; // this cancels the close event.
        }
        #endregion

        private void FFT_DataEditor_Load(object sender, EventArgs e)
        {
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
                default:
                    {
                        break;
                    }
            }
            this.Refresh();
        }

    }
}
