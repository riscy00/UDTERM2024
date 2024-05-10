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
    public partial class PopUpEntryBox : Form
    {
        public bool isCancel;

        GlobalBase myGlobalBase;

        //##############################################################################################################
        //============================================================================================= Reference
        //##############################################################################################################

        #region //============================================================UDT Reference Object
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
            //--------------------------------------Themes
            switch (myGlobalBase.CompanyName)
            {
                case (int)GlobalBase.eCompanyName.BGDS:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.BG_MainPage_1A;
                        break;
                    }
                case (int)GlobalBase.eCompanyName.ADT:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_1A;
                        break;
                    }
                case ((int)GlobalBase.eCompanyName.TVB):
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPageTVB1D;
                        break;
                    }
                default:
                    {
                        this.BackgroundImage = global::UDT_Term.Properties.Resources.MainPage_1A;
                        break;
                    }
            }
            this.Refresh();
        }
        #endregion

        //##############################################################################################################
        //============================================================================================= Constructor
        //##############################################################################################################
        public PopUpEntryBox()
        {
            InitializeComponent();
        }

        //##############################################################################################################
        //============================================================================================= UDT Window Form 
        //##############################################################################################################

        #region //=======================================PopUpEntryBox_Load
        private void PopUpEntryBox_Load(object sender, EventArgs e)
        {

        }
        #endregion

        #region //=======================================PopUpEntry_Show
        public void PopUpEntry_Setup(string sDescription, string OldValue, GlobalBase myGlobalBaseRef)
        {
            MyGlobalBase(myGlobalBaseRef);
            lbMessage.Text = sDescription;
            myGlobalBase.PopUpForm_StringValue = OldValue;
            tbEntry.Text = OldValue;
        }
        #endregion

        #region //=======================================BtnOkay_Click
        private void BtnOkay_Click(object sender, EventArgs e)
        {
            myGlobalBase.PopUpForm_StringValue = tbEntry.Text;
            this.Close();
        }
        #endregion

        #region //=======================================BtnCancel_Click
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            myGlobalBase.PopUpForm_StringValue = "##CANCEL##";
            this.Close();
        }
        #endregion

        #region //=======================================TbEntry_KeyDown
        private void TbEntry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                myGlobalBase.PopUpForm_StringValue = "##CANCEL##";
                this.Close();
            }
            //-----------------------------------------Enter
            if (e.KeyData == Keys.Enter)
            {
                myGlobalBase.PopUpForm_StringValue = tbEntry.Text;
                this.Close();
            }
        }
#endregion
    }
}
