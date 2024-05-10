using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDT_Term_FFT
{
    public partial class wfSectorErasePara : Form
    {
        GlobalBase myGlobalBase;
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
            //--------------------------------------Themes
            switch (myGlobalBase.CompanyName)
            {
                case (int)GlobalBase.eCompanyName.BGDS:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.BG_MainPage_Blank_1B;
                        break;
                    }
                case (int)GlobalBase.eCompanyName.ADT:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_Blank_1B;
                        break;
                    }
                case ((int)GlobalBase.eCompanyName.TVB):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPageTVBBlank_1B;
                        break;
                    }
                default:
                    {
                            this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_Blank_1B;
                            break;
                    }
            }
            this.Refresh();
        }
        public wfSectorErasePara()
        {
            InitializeComponent();
            label3.Text = @"M25P16 has 32 sectors which each or many can be erased. For 1 sector erase only, make 'Start Sector' equal to 'End Sector' . NB: Bulk Erase erase all 32 sectors";
        }

        private void wfSectorErasePara_Load(object sender, EventArgs e)
        {
            tbSEStart.Text = myGlobalBase.EE_Sector_Start.ToString();
            tbSEEnd.Text = myGlobalBase.EE_Sector_Stop.ToString();
            myGlobalBase.EE_isEraseOkay = false;
        }

        private void btnProceed_Click(object sender, EventArgs e)
        {
            int Start;
            int End;
            bool res1 = int.TryParse(tbSEStart.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out Start);
            bool res2 = int.TryParse(tbSEEnd.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out End);
            if ((res1 == true) & (res2 == true))
            {
                if (Start >= End)
                {
                    Start = End;
                }
                if ((Start <= 31) & (End <= 31))
                {
                    myGlobalBase.EE_Sector_Start = Start;
                    myGlobalBase.EE_Sector_Stop = End;
                    tbSEStart.Text = myGlobalBase.EE_Sector_Start.ToString();
                    tbSEEnd.Text = myGlobalBase.EE_Sector_Stop.ToString();
                    myGlobalBase.EE_isEraseOkay = true;
                    this.Close();
                }
                else
                {
                    Console.Beep(1000, 50);
                    Thread.Sleep(300);
                    Console.Beep(1000, 50);
                    Thread.Sleep(300);
                    Console.Beep(1000, 50);
                    myGlobalBase.EE_isEraseOkay = false;
                }
            }
            else
            {
                Console.Beep(1000, 50);
                Thread.Sleep(300);
                Console.Beep(1000, 50);
                Thread.Sleep(300);
                Console.Beep(1000, 50);
                myGlobalBase.EE_isEraseOkay = false;
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            myGlobalBase.EE_isEraseOkay = false;
        }
    }
}
