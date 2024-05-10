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
    public partial class CanConfig : Form
    {
        //##############################################################################################################
        //============================================================================================= Common items
        //##############################################################################################################
        ITools Tools = new Tools();
        GlobalBase myGlobalBase;
        DialogSupport mbOperationBusy;
        MainProg myMainProg;
        //------------------------------For future.
        //USB_FTDI_Comm myUSBComm;                    // FTDI device manager section (scan, open, close, read, write).
        //USB_VCOM_Comm myUSBVCOMComm;
        //DMFP_BulkFrame cDMFP_BulkFrame;
        //DMFProtocol myDMFProtocol;
        //------------------------------
        #region //============================================================Reference Object
        public void MyMainProg(MainProg myMainProgRef)
        {
            myMainProg = myMainProgRef;
        }
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
        #endregion

        //##############################################################################################################
        //============================================================================================= PCAN Stuff
        //##############################################################################################################

        //##############################################################################################################
        //============================================================================================= 
        //##############################################################################################################
        public CanConfig()
        {
            InitializeComponent();


        }
    }
}
