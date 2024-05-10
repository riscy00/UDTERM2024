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
    public partial class PDFViewer : Form
    {
        public string filename { get; set; }
        public PDFViewer()
        {
            InitializeComponent();
            filename = "";
        }

        private void PDFViewer_Load(object sender, EventArgs e)
        {
            try
            {
                if (filename != "")
                {
                    webBrowser1.Navigate(filename);
                }
            }
            catch
            {
                this.Visible = false;
                this.Hide();
            }

        }

        private void PDFViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
            this.Hide();
        }
    }
}
