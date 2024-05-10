using JR.Utils.GUI.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Management;
using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

namespace UDT_Term_FFT
{
    public partial class USBComPortManager : Form
    {
        ITools Tools = new Tools();
        //---------------------------------------------------------Private
        private int iSelectedFTDIDevice;
        private int iSelectedVComDevice;
        //private int iSelectedHMRVComDevice;
        private bool isSerialPortArray;
        private int iSelectedPortArrayIndex;
        //---------------------------------------------------------Supplemental Forms
        //---------------------------------------------------------Reference
        GlobalBase myGlobalBase;
        USB_FTDI_Comm myUSBComm;
        USB_VCOM_Comm myUSBVCOMComm;
        List<USBDeviceInfo> VCOMdevices;            // Single VCOM list linked to datagridview and USB device information. This is early project support FTDI/VCOM
        List<USBDeviceInfo> VCOMdevicesArray;       // Index Array VCOM list linked to datagridview and USB device information. This is VCOM for tool/instrument device.
        IntPtr rtbOEMcom;

        System.Windows.Forms.RichTextBox myRtbTerm;
        //--------------------------------------------------------Generic/Classes
        List<string> FTDIs0 = new List<string>();
        List<string> FTDIs1 = new List<string>();
        List<string> FTDIs2 = new List<string>();

        //---------------------------------------------------------Public
        #region//---------------------------------------------------------Reference 
        public void MyUSBVCOMComm(USB_VCOM_Comm myUSBVCOMCommRef)
        {
            myUSBVCOMComm = myUSBVCOMCommRef;
        }
        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;
        }
        public void MyRtbTerm(System.Windows.Forms.RichTextBox myrtbTermref)
        {
            myRtbTerm = myrtbTermref;
        }
        public void MyUSBFTDIComm(USB_FTDI_Comm myUSB_FTDI_CommRef)
        {
            myUSBComm = myUSB_FTDI_CommRef;
        }
        #endregion

        //----------------------------------------------------------Constructor
        public USBComPortManager()
        {
            InitializeComponent();
            //-------------------------------------------Add datagridview column
            SetupDataGridViewColumn();
            isSerialPortArray = false;
            rtbOEMcom = new IntPtr();

        }

        #region//==================================================USBComPortManager_FormClosing
        private void USBComPortManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        #endregion

        #region//==================================================USBComPortManager_Load
        private void USBComPortManager_Load(object sender, EventArgs e)
        {
            CompanyName_Theme_Update();
            //dgvComList.TabStop = false;
            iSelectedFTDIDevice = -1;       // Not found.
            iSelectedVComDevice = -1;       // Not found.
        }
        #endregion


        //##########################################################################################
        //############################################################################# DatGrd View
        //##########################################################################################

        #region//==================================================SetupDataGridViewColumn
        private void SetupDataGridViewColumn()
        {
            dgvComList.Rows.Clear();
            dgvComList.Columns.Clear();

            var col0 = new DataGridViewTextBoxColumn();     // Com type
            var col1 = new DataGridViewTextBoxColumn();     // COM Number
            var col2 = new DataGridViewButtonColumn();      // Twinkle Button
            var col3 = new DataGridViewTextBoxColumn();    // Info

            // 
            // colType
            // 
            col0.HeaderText = "Type";
            col0.Name = "colType";
            col0.ReadOnly = true;
            col0.Width = 60;
            // 
            // colComNumber
            // 
            col1.HeaderText = "Com No.";
            col1.Name = "colComNumber";
            col1.ReadOnly = true;
            col1.Width = 50;
            // 
            // colTwinkle
            // 
            col2.HeaderText = "Twinkle";
            col2.Name = "colTwinkle";
            col2.ReadOnly = false;
            col2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            col2.Text = "Blink";
            col2.UseColumnTextForButtonValue = true;
            col2.ToolTipText = "Click to twinkle LED to identify the device.";
            col2.Width = 50;
            // 
            // colDeviceInfo
            // 
            col3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            col3.HeaderText = "Info";
            col3.MinimumWidth = 350;
            col3.Name = "colDeviceInfo";
            col3.ReadOnly = true;
            col3.Width = 375;

            dgvComList.Columns.AddRange(new DataGridViewColumn[] { col0, col1, col2, col3 });

        }
        #endregion

        //##########################################################################################
        //############################################################################# Single VCOM 
        //##########################################################################################

        #region//==================================================ScanPortNow
        public void ScanPortNow()
        {
            isSerialPortArray = false;
            dgvComList.Rows.Clear();        // Clear all old COM data.
            dgvComList.Refresh();
            FTDIs0.Clear();
            FTDIs1.Clear();
            FTDIs2.Clear();
            // Update debug message. 
            myRtbTerm.Clear();
            myRtbTerm.ForeColor = myGlobalBase.ColorMessage;
            myRtbTermMessage("Identified VCOM and FT232RL device\n");
            myRtbTermMessage("----------------------------------\n");

            switch (myGlobalBase.VCOMMode)
            {
                case (0):           // Both FTDI and VCOM
                    {
                        ScanFTDINow();
                        ScanVcomNow();
                        break;
                    }
                case (1):           // VCOM Only
                    {
                        ScanVcomNow();
                        break;
                    }
                case (2):           // FTDI Only
                    {
                        ScanFTDINow();
                        if (FTDIs0.Count==1)        // if only one then no need to pop up window. 
                        {
                            USBCommPort_Start_FTDI(0);
                            this.Hide();
                        }
                        break;
                    }
                default:
                    return;
            }
            myRtbTermMessage("----------------------------------\n");

        }
        #endregion

        #region//==================================================ScanFTDINow
        private void ScanFTDINow()
        {
            int i = 0;
            myUSBComm.FTDI_Scan_Device();
            while (i < myUSBComm.FTDI_Device_Count)
            {
                string s0 = "";
                string s1 = "";
                string s2 = "";
                myUSBComm.FTDI_Scan_GetData(i, ref s0, ref s1, ref s2);
                FTDIs0.Add(s0);
                FTDIs1.Add(s1);
                FTDIs2.Add(s2);
                i++;
            }
            try
            {
                for (i = 0; i < FTDIs0.Count; i++)
                {
                    string[] row1 = new string[] { "FT323RL", "N/A", "", (FTDIs0[i] + "//" + FTDIs1[i] + "//" + FTDIs2[i]) };
                    dgvComList.Rows.Add(row1);
                    row1 = null;

                    myRtbTermMessage("==>FT232RL Found (under D2XX Driver) " +
                        "\n,    Device Data#1: " + FTDIs0[i] +
                        "\n,    Device Data#2: " + FTDIs1[i] +
                        "\n,    Device Data#3: " + FTDIs2[i] +
                        "\n\n");

                }
                dgvComList.Refresh();
            }
            catch
            {
                MessageBox.Show("Error: DataGridView Corruption, Post Scan", "Error in VCOM/FTDI Display in DataGridView (After Scan)",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        #endregion

        #region//==================================================ScanVcomNow
        private void ScanVcomNow()
        {
            GetUSBDevices();
            try
            {
                foreach (var usbDevice in VCOMdevices)
                {
                    string[] row1 = new string[] { "VCOM", usbDevice.DeviceID.ToString(), "", usbDevice.PnpDeviceID.ToString() };
                    dgvComList.Rows.Add(row1);
                    row1 = null;

                    myRtbTermMessage("==>VCOM Found No: " + usbDevice.DeviceID.ToString() +
                        "\n,    Description: " + usbDevice.PnpDeviceID.ToString() +
                        "\n,    Status: " + usbDevice.status.ToString() +
                        "\n\n");
                }
            }
            catch
            {
                MessageBox.Show("Error: VCOM scan or datagridview issue", "Error in VCOM/FTDI Display in DataGridView (After Scan)",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            }
            dgvComList.Refresh();
        }
        #endregion

        #region//==================================================ScanVcomNowBGPING
        public string ScanVcomNowBGPING()
        {
            string rxmess = "";
            string scanCom = "";
            bool err = false;
            int loopcount;
            iSelectedFTDIDevice = -1;                                   // Not selected
            GetUSBDevices();
            try
            {
                foreach (var usbDevice in VCOMdevices)
                {
                    //-------------------Step 1: filter out the Bluetooth channels, behaving alike VCOM channels. 
                    if (!((usbDevice.Description.Contains("Blue") == true)  | (usbDevice.Description.Contains("blue") == true) | (usbDevice.Description.Contains("BTE") == true)))    
                    {
                        err = false;
                        loopcount = 0;
                        rxmess = "";
                        scanCom = usbDevice.DeviceID.ToString();
                        //-------------------Step 2: attempt to open port 
                        try
                        {
                            myUSBVCOMComm.VCOM_SerialInit(scanCom);
                            myUSBVCOMComm.serialPort1.Open();
                        }
                        catch
                        {
                            myRtbTermMessage("###ERR: Unable to open Port Name " + scanCom + " :- It does not exist or in use by other app\n");
                            err = true;
                        }
                        //-------------------Close port if error happen.
                        if (err == true)
                        {
                            try
                            {
                                if (myUSBVCOMComm.serialPort1.IsOpen == true)
                                    myUSBVCOMComm.serialPort1.Close();
                            }
                            catch
                            {
                                myRtbTermMessage("###ERR: Unable to close Serial Port Name: " + scanCom + " :- Use Manual approach!\n");
                                return "";
                            }
                        }
                        else
                        {
                            //-------------------Step 3: Validate open port
                            while ((myUSBVCOMComm.serialPort1.IsOpen == false) & (loopcount < 10))
                            {
                                loopcount = 0;
                                Thread.Sleep(10);
                                loopcount++;
                            }
                            if (loopcount >= 9)
                            {
                                myRtbTermMessage("###ERR: Unable to validate isOpn on Serial Port Name: " + scanCom + " :- Use Manual approach!\n");
                            }
                            else
                            {
                                //-------------------Step 4: Send BGPING() message
                                try
                                {
                                    err = false;
                                    myUSBVCOMComm.serialPort1.WriteLine("BGPING()");
                                    Trace.WriteLine("-INFO : VCOM (String) Transmits Success :'BGPING()'");
                                }
                                catch
                                {
                                    err = true;
                                }
                                //-------------------Step 4: Read Message, <100mSec to get results. 
                                if (err == false)
                                {
                                    loopcount = 0;
                                    while (true)
                                    {
                                        Thread.Sleep(10);
                                        string rx = myUSBVCOMComm.serialPort1.ReadExisting();
                                        rx=rx.Replace("\0", string.Empty);
                                        rx = rx.Replace(" ", string.Empty);
                                        rxmess += rx;
                                        if (rxmess.Contains("\n"))
                                        {
                                            //-------------------Step 5: Detected Message and finish
                                            if (rxmess.Contains("~BGPING(0x1)") == true)
                                            {
                                                loopcount = 0;
                                                myGlobalBase.is_Serial_Server_Connected = true;
                                                myRtbTermMessage("==>VCOM Found No: " + scanCom +
                                                    "\n    Description: " + usbDevice.PnpDeviceID.ToString() +
                                                    "\n    Status: " + usbDevice.status.ToString() + "\n");
                                                myRtbTermMessage("==>BGPING() confirm correct response from the Tool, VCOM channel is now open\n");
                                                myUSBVCOMComm.VCOM_StartThread_RX_Operation();
                                                return scanCom;
                                            }
                                        }
                                        loopcount++;
                                        if (loopcount > 10)     // Expires 100mSec. 
                                            break;
                                    }
                                }
                                //-------------------Step 6: Close Port and try next one. 
                                try
                                {
                                    if (myUSBVCOMComm.serialPort1.IsOpen == true)
                                        myUSBVCOMComm.serialPort1.Close();
                                    Thread.Sleep(10);
                                }
                                catch
                                {
                                    myRtbTermMessage("###ERR: Unable to close Serial Port Name: " + scanCom + " :- Use Manual approach!\n");
                                    return "";
                                }
                            }
                        }
                    }

                }
            }
            catch
            {
                myRtbTermMessage("###ERR: Serial Port Name Failure!: " + scanCom + " :- Use Manual approach!\n");
            }
            return "";
        }
        #endregion

        #region//==================================================ScanArrayVcomNowBGDRILLING_BGPING
        public string ScanArrayVcomNowBGDRILLING_BGPING()
        {
            string scanCom = "";
            iSelectedFTDIDevice = -1;                                  // Not selected
            ScanPortNowArray((int)eUSBDeviceType.BGDRILLING, true);    // This also set up iSelectedPortArrayIndex = (int)eUSBDeviceType.BGDRILLING
            try
            {
                int index = -1;
                foreach (var usbDevice in VCOMdevicesArray)
                {
                    index++;
                    bool err = false;
                    int loopcount;
                    //-------------------Step 1: filter out the Bluetooth channels, behaving alike VCOM channels. 
                    if (!((usbDevice.Description.Contains("Blue")) | (usbDevice.Description.Contains("blue")) | (usbDevice.Description.Contains("BTE"))))
                    {
                        err = false;
                        loopcount = 0;
                        scanCom = usbDevice.DeviceID.ToString();
                        //-------------------Step 2: attempt to open port 
                        try
                        {
                            myUSBVCOMComm.VCOMArray_SerialInit(VCOMdevicesArray[index].DeviceID, (int)eUSBDeviceType.BGDRILLING);   // Comm Port Number (ie COM9)
                            myUSBVCOMComm.VCOMArray_Start_RX_Operation((int)eUSBDeviceType.BGDRILLING);
                        }
                        catch
                        {
                            myRtbTermMessage("###ERR: Unable to open Port Name " + scanCom + " :- It does not exist or in use by other app\n");
                            err = true;
                        }
                        //-------------------Close port if error happen.
                        if (err == true)
                        {
                            if (USBCommPort_Close_VCOMArray((int)eUSBDeviceType.BGDRILLING) == false)
                            {
                                myRtbTermMessage("###ERR: Unable to close Serial Port Name: " + scanCom + " :- Try Manual approach!\n");
                                return "";
                            }
                        }
                        else
                        {
                            myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].MessTempRX = "";
                            //-------------------Step 3: Validate open port
                            while ((myUSBVCOMComm.VCOMserialPortArray[(int)eUSBDeviceType.BGDRILLING].IsOpen == false) & (loopcount < 10))
                            {
                                loopcount = 0;
                                Tools.InteractivePause(TimeSpan.FromMilliseconds(10));
                                loopcount++;
                            }
                            if (loopcount >= 9)
                            {
                                myRtbTermMessage("###ERR: Unable to validate isOpen on Serial Port Name: " + scanCom + " :- Try Manual approach!\n");
                            }
                            else
                            {
                                Tools.InteractivePause(TimeSpan.FromMilliseconds(2000)); // Allow 1 Sec time for Main/ZMDI board to power up. 
                                //-------------------Step 4: Send +BGPING() message
                                try
                                {
                                    err = false;
                                    Thread.Sleep(100);
                                    myUSBVCOMComm.VCOMserialPortArray[(int)eUSBDeviceType.BGDRILLING].Write("BGPING()\n");
                                    Thread.Sleep(100);
                                    myUSBVCOMComm.VCOMserialPortArray[(int)eUSBDeviceType.BGDRILLING].Write("BGPING()\n");
                                    //Trace.WriteLine("-INFO : VCOM (String) Transmits Success :'BGPING()'");
                                }
                                catch
                                {
                                    err = true;
                                }
                                //-------------------Step 4: Read Message, <100mSec to get results. 
                                if (err == false)
                                {
                                    loopcount = 0;
                                    while (true)
                                    {
                                        Tools.InteractivePause(TimeSpan.FromMilliseconds(10));
                                        if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].MessTempRX.Contains("\n"))
                                        {
                                            //-------------------Step 5: Detected Message and finish
                                            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.BGDRILLING].MessTempRX.Contains("~BGPING(0x1)") == true)
                                            {
                                                myRtbTermMessage("==>VCOM Found No: " + scanCom +
                                                    "\n    Description: " + usbDevice.PnpDeviceID.ToString() +
                                                    "\n    Status: " + usbDevice.status.ToString() + "\n");
                                                myRtbTermMessage("==>BGPING() confirm correct response from the Tool, VCOM channel is now open\n");
//                                                 if (myUSBVCOMComm.VCOMArray_Start_RX_Operation((int)eUSBDeviceType.BGDRILLING) == false)
//                                                 {
//                                                     iSelectedVComDevice = -1;           // Not selected.
//                                                     return "";                          // 
//                                                 }
                                                return scanCom;
                                            }
                                        }
                                        loopcount++;
                                        if (loopcount > 10)     // Expires 100mSec. 
                                            break;
                                    }
                                }
                                //-------------------Step 6: Close Port and try next one. 
                                if (myUSBVCOMComm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.BGDRILLING) == false)
                                {
                                    myRtbTermMessage("###ERR: Unable to close Serial Port Name: " + scanCom + " :- Try Manual approach\n");
                                    return "";
                                }
                                Tools.InteractivePause(TimeSpan.FromMilliseconds(10));
                            }
                        }
                    }
                }

                
            }
            catch
            {
                myRtbTermMessage("###ERR: Serial Port Name (BGDRILLING) Failure!: " + scanCom + " :- Try Again or Manual Approach\n");
            }
            return "";
        }
        #endregion

        #region//==================================================ScanArrayVcomNowMiniAD7794PING
        public string ScanArrayVcomNowMiniAD7794PING()
        {
            string scanCom = "";
            iSelectedFTDIDevice = -1;                                   // Not selected
            ScanPortNowArray((int)eUSBDeviceType.MiniAD7794,true);           // This also set up iSelectedPortArrayIndex = (int)eUSBDeviceType.MiniAD7794
            try
            {
                int index = -1;
                foreach (var usbDevice in VCOMdevicesArray)
                {
                    index++;
                    bool err = false;
                    int loopcount;
                    //-------------------Step 1: filter out the Bluetooth channels, behaving alike VCOM channels. 
                    if (!((usbDevice.Description.Contains("Blue")) | (usbDevice.Description.Contains("blue")) | (usbDevice.Description.Contains("BTE"))))
                    {
                        if (usbDevice.PnpDeviceID.Contains("MINIAD7794"))
                        {
                            err = false;
                            loopcount = 0;
                            scanCom = usbDevice.DeviceID.ToString();
                            //-------------------Step 2: attempt to open port 
                            try
                            {
                                myUSBVCOMComm.VCOMArray_SerialInit(VCOMdevicesArray[index].DeviceID, (int)eUSBDeviceType.MiniAD7794);   // Comm Port Number (ie COM9)
                                myUSBVCOMComm.VCOMArray_Start_RX_Operation((int)eUSBDeviceType.MiniAD7794);
                            }
                            catch
                            {
                                myRtbTermMessage("###ERR: Unable to open Port Name " + scanCom + " :- It does not exist or in use by other app\n");
                                err = true;
                            }
                            //-------------------Close port if error happen.
                            if (err == true)
                            {
                                if (USBCommPort_Close_VCOMArray((int)eUSBDeviceType.MiniAD7794) == false)
                                {
                                    myRtbTermMessage("###ERR: Unable to close Serial Port Name: " + scanCom + " :- Try Manual approach!\n");
                                    return "";
                                }
                            }
                            else
                            {
                                //-------------------Step 3: Validate open port
                                while ((myUSBVCOMComm.VCOMserialPortArray[(int)eUSBDeviceType.MiniAD7794].IsOpen == false) & (loopcount < 10))
                                {
                                    loopcount = 0;
                                    Tools.InteractivePause(TimeSpan.FromMilliseconds(10));
                                    loopcount++;
                                }
                                if (loopcount >= 9)
                                {
                                    myRtbTermMessage("###ERR: Unable to validate isOpen on Serial Port Name: " + scanCom + " :- Try Manual approach!\n");
                                }
                                else
                                {
                                    Tools.InteractivePause(TimeSpan.FromMilliseconds(2000)); // Allow 1 Sec time for Main/ZMDI board to power up. 
                                    //-------------------Step 4: Send IDTPING() message
                                    try
                                    {
                                        err = false;
                                        myUSBVCOMComm.VCOMserialPortArray[(int)eUSBDeviceType.MiniAD7794].WriteLine("+MINIPING()");
                                        Trace.WriteLine("-INFO : VCOM (String) Transmits Success :'MINIPING()'");
                                    }
                                    catch
                                    {
                                        err = true;
                                    }
                                    //-------------------Step 4: Read Message, <100mSec to get results. 
                                    if (err == false)
                                    {
                                        loopcount = 0;
                                        while (true)
                                        {
                                            Tools.InteractivePause(TimeSpan.FromMilliseconds(10));
                                            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].MessTempRX.Contains("\n"))
                                            {
                                                //-------------------Step 5: Detected Message and finish
                                                if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.MiniAD7794].MessTempRX.Contains("-MINIPING") == true)
                                                {
                                                    //myGlobalBase.is_Serial_Server_Connected = true;
                                                    myRtbTermMessage("==>VCOM Found No: " + scanCom +
                                                        "\n    Description: " + usbDevice.PnpDeviceID.ToString() +
                                                        "\n    Status: " + usbDevice.status.ToString() + "\n");
                                                    myRtbTermMessage("==>MINIPING() confirm correct response from the Tool, VCOM channel is now open\n");
                                                    //if (myUSBVCOMComm.VCOMArray_Start_RX_Operation((int)eUSBDeviceType.MiniAD7794) == false)
                                                    //{
                                                    //    iSelectedVComDevice = -1;           // Not selected.
                                                    //    return "";                          // 
                                                    //}
                                                    return scanCom;
                                                }
                                            }
                                            loopcount++;
                                            if (loopcount > 50)     // Expires 500mSec. 
                                                break;
                                        }
                                    }
                                    //-------------------Step 6: Close Port and try next one. 
                                    //if (USBCommPort_Close_VCOMArray((int)eUSBDeviceType.ADIS) == false)
                                    if (myUSBVCOMComm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.MiniAD7794) == false)
                                    {
                                        myRtbTermMessage("###ERR: Unable to close Serial Port Name: " + scanCom + " :- Try Manual approach\n");
                                        return "";
                                    }
                                    Tools.InteractivePause(TimeSpan.FromMilliseconds(10));
                                }
                            }
                        }
                    }

                }
            }
            catch
            {
                myRtbTermMessage("###ERR: Serial Port Name Failure!: " + scanCom + " :- Try Again or Manual Approach\n");
            }
            return "";
        }
        #endregion

        #region//==================================================ScanVcomNowIDTPING
        public string ScanVcomNowIDTPING()
        {
            string rxmess = "";
            string scanCom = "";
            iSelectedFTDIDevice = -1;                                   // Not selected
            GetUSBDevices();
            try
            {
                foreach (var usbDevice in VCOMdevices)
                {
                bool err = false;
                 int loopcount;
                    //-------------------Step 1: filter out the Bluetooth channels, behaving alike VCOM channels. 
                    if (!((usbDevice.Description.Contains("Blue") == true) | (usbDevice.Description.Contains("blue") == true) | (usbDevice.Description.Contains("BTE") == true)))
                    {
                        err = false;
                        loopcount = 0;
                        rxmess = "";
                        scanCom = usbDevice.DeviceID.ToString();
                        //-------------------Step 2: attempt to open port 
                        try
                        {
                            myUSBVCOMComm.VCOM_SerialInit(scanCom);
                            myUSBVCOMComm.serialPort1.Open();
                        }
                        catch
                        {
                            myRtbTermMessage("###ERR: Unable to open Port Name " + scanCom + " :- It does not exist or in use by other app\n");
                            err = true;
                        }
                        //-------------------Close port if error happen.
                        if (err == true)
                        {
                            try
                            {
                                if (myUSBVCOMComm.serialPort1.IsOpen == true)
                                    myUSBVCOMComm.serialPort1.Close();
                            }
                            catch
                            {
                                myRtbTermMessage("###ERR: Unable to close Serial Port Name: " + scanCom + " :- Try Manual approach!\n");
                                return "";
                            }
                        }
                        else
                        {
                            //-------------------Step 3: Validate open port
                            while ((myUSBVCOMComm.serialPort1.IsOpen == false) & (loopcount < 10))
                            {
                                loopcount = 0;
                                Thread.Sleep(10);
                                loopcount++;
                            }
                            if (loopcount >= 9)
                            {
                                myRtbTermMessage("###ERR: Unable to validate isOpn on Serial Port Name: " + scanCom + " :- Try Manual approach!\n");
                            }
                            else
                            {
                                Tools.InteractivePause(TimeSpan.FromMilliseconds(1000)); // Allow 1 Sec time for Main/ZMDI board to power up. 
                                //-------------------Step 4: Send IDTPING() message
                                try
                                {
                                    
                                    err = false;
                                    myUSBVCOMComm.serialPort1.WriteLine("IDTPING()");
                                    Trace.WriteLine("-INFO : VCOM (String) Transmits Success :'IDTPING()'");
                                }
                                catch
                                {
                                    err = true;
                                }
                                //-------------------Step 4: Read Message, <100mSec to get results. 
                                if (err == false)
                                {
                                    loopcount = 0;
                                    while (true)
                                    {
                                        Thread.Sleep(10);
                                        string rx = myUSBVCOMComm.serialPort1.ReadExisting();
                                        rx = rx.Replace("\0", string.Empty);
                                        rx = rx.Replace(" ", string.Empty);
                                        rxmess += rx;
                                        if (rxmess.Contains("\n"))
                                        {
                                            //-------------------Step 5: Detected Message and finish
                                            if (rxmess.Contains("~IDTPING") == true)
                                            {
                                                myGlobalBase.is_Serial_Server_Connected = true;
                                                myRtbTermMessage("==>VCOM Found No: " + scanCom +
                                                    "\n    Description: " + usbDevice.PnpDeviceID.ToString() +
                                                    "\n    Status: " + usbDevice.status.ToString() + "\n");
                                                myRtbTermMessage("==>IDTPING() confirm correct response from the Tool, VCOM channel is now open\n");
                                                myUSBVCOMComm.VCOM_StartThread_RX_Operation();
                                                return scanCom;
                                            }
                                        }
                                        loopcount++;
                                        if (loopcount > 50)     // Expires 500mSec. 
                                            break;
                                    }
                                }
                                //-------------------Step 6: Close Port and try next one. 
                                try
                                {
                                    if (myUSBVCOMComm.serialPort1.IsOpen == true)
                                        myUSBVCOMComm.serialPort1.Close();
                                    Thread.Sleep(10);
                                }
                                catch
                                {
                                    myRtbTermMessage("###ERR: Unable to close Serial Port Name: " + scanCom + " :- TRy Manual approach!\n");
                                    return "";
                                }
                            }
                        }
                    }

                }
            }
            catch
            {
                myRtbTermMessage("###ERR: Serial Port Name Failure!: " + scanCom + " :- Try Manual approach!\n");
            }
            return "";
        }
        #endregion

        #region//==================================================ScanArrayVcomNowADISPING
        public string ScanArrayVcomNowADISPING()
        {
            string scanCom = "";
            iSelectedFTDIDevice = -1;                                   // Not selected
            ScanPortNowArray((int)eUSBDeviceType.ADIS,true);                 // This also set up iSelectedPortArrayIndex = (int)eUSBDeviceType.ADIS
            try
            {
                int index = -1;
                foreach(var usbDevice in VCOMdevicesArray)
                {
                    index++;
                    bool err = false;
                    int loopcount;
                    //-------------------Step 1: filter out the Bluetooth channels, behaving alike VCOM channels. 
                    if (!((usbDevice.Description.Contains("Blue") == true) | (usbDevice.Description.Contains("blue") == true) | (usbDevice.Description.Contains("BTE") == true)))
                    {
                        err = false;
                        loopcount = 0;
                        scanCom = usbDevice.DeviceID.ToString();
                        //-------------------Step 2: attempt to open port 
                        try
                        {
                            myUSBVCOMComm.VCOMArray_SerialInit(VCOMdevicesArray[index].DeviceID, (int)eUSBDeviceType.ADIS);   // Comm Port Number (ie COM9)
                            //myUSBVCOMComm.VCOMserialPortArray[(int)eUSBDeviceType.ADIS].Open();
                            myUSBVCOMComm.VCOMArray_Start_RX_Operation((int)eUSBDeviceType.ADIS);
                        }
                        catch
                        {
                            myRtbTermMessage("###ERR: Unable to open Port Name " + scanCom + " :- It does not exist or in use by other app\n");
                            err = true;
                        }
                        //-------------------Close port if error happen.
                        if (err == true)
                        {
                            if (USBCommPort_Close_VCOMArray((int)eUSBDeviceType.ADIS) == false)
                            {
                                myRtbTermMessage("###ERR: Unable to close Serial Port Name: " + scanCom + " :- Try Manual approach!\n");
                                return "";
                            }
                        }
                        else
                        {
                            //-------------------Step 3: Validate open port
                            while ((myUSBVCOMComm.VCOMserialPortArray[(int)eUSBDeviceType.ADIS].IsOpen == false) & (loopcount < 10))
                            {
                                loopcount = 0;
                                Tools.InteractivePause(TimeSpan.FromMilliseconds(10));
                                loopcount++;
                            }
                            if (loopcount >= 9)
                            {
                                myRtbTermMessage("###ERR: Unable to validate isOpn on Serial Port Name: " + scanCom + " :- Try Manual approach!\n");
                            }
                            else
                            {
                                Tools.InteractivePause(TimeSpan.FromMilliseconds(500)); // Allow 1 Sec time for Main/ZMDI board to power up. 
                                //-------------------Step 4: Send IDTPING() message
                                try
                                {
                                    err = false;
                                    myUSBVCOMComm.VCOMserialPortArray[(int)eUSBDeviceType.ADIS].WriteLine("ADISPING()");
                                    Trace.WriteLine("-INFO : VCOM (String) Transmits Success :'ADISPING()'");
                                }
                                catch
                                {
                                    err = true;
                                }
                                //-------------------Step 4: Read Message, <100mSec to get results. 
                                if (err == false)
                                {
                                    loopcount = 0;
                                    while (true)
                                    {
                                        Tools.InteractivePause(TimeSpan.FromMilliseconds(10));
                                        if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].MessTempRX.Contains("\n"))
                                        {
                                            //-------------------Step 5: Detected Message and finish
                                            if (myGlobalBase.myUSBVCOMArray[(int)eUSBDeviceType.ADIS].MessTempRX.Contains("~ADISPING") == true)
                                            {
                                                //myGlobalBase.is_Serial_Server_Connected = true;
                                                myRtbTermMessage("==>VCOM Found No: " + scanCom +
                                                    "\n    Description: " + usbDevice.PnpDeviceID.ToString() +
                                                    "\n    Status: " + usbDevice.status.ToString() + "\n");
                                                myRtbTermMessage("==>ADISPING() confirm correct response from the Tool, VCOM channel is now open\n");
                                                //if (myUSBVCOMComm.VCOMArray_Start_RX_Operation((int)eUSBDeviceType.ADIS) == false)
                                                //{
                                                //    iSelectedVComDevice = -1;           // Not selected.
                                                //    return "";                          // 
                                                //}
                                                return scanCom;
                                            }
                                        }
                                        loopcount++;
                                        if (loopcount > 50)     // Expires 500mSec. 
                                            break;
                                    }
                                }
                                //-------------------Step 6: Close Port and try next one. 
                                //if (USBCommPort_Close_VCOMArray((int)eUSBDeviceType.ADIS) == false)
                                if (myUSBVCOMComm.VCOMArray_Message_ClosePort((int)eUSBDeviceType.ADIS)==false)
                                {
                                    myRtbTermMessage("###ERR: Unable to close Serial Port Name: " + scanCom + " :- Try Manual approach\n");
                                    return "";
                                }
                                Tools.InteractivePause(TimeSpan.FromMilliseconds(10));
                            }
                        }
                    }

                }
            }
            catch
            {
                myRtbTermMessage("###ERR: Serial Port Name Failure!: " + scanCom + " :- Try Again or Manual Approach\n");
            }
            return "";
        }
        #endregion

        #region//==================================================GetUSBDevices().....WPI Method (preferred)
        //private List<USBDeviceInfo> GetUSBDevices()   
        private void GetUSBDevices()
        {
            if (VCOMdevices == null)
            {
                VCOMdevices = new List<USBDeviceInfo>();
            }
            VCOMdevices.Clear();
            ManagementObjectCollection collection;
            //--------------------------------------------
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\""))
                collection = searcher.Get();
            foreach (ManagementObject queryObj in collection)
            {
                string name = (string)queryObj.GetPropertyValue("Name");
                if (name.Contains("(COM"))
                    name = name.Substring(name.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")", string.Empty);

                VCOMdevices.Add(new USBDeviceInfo(
                name,                                                 //(string)queryObj.GetPropertyValue("DeviceID"),
                (string)queryObj.GetPropertyValue("PNPDeviceID"),     // Include VID/PID and Serial String (with EVKIT for pro or EVKIM for mini)
                (string)queryObj.GetPropertyValue("Description"),
                (string)queryObj.GetPropertyValue("Name"),
                (string)queryObj.GetPropertyValue("Status")
                ));
                #region
                /*
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("DeviceID: {0}", queryObj["DeviceID"]);
                Console.WriteLine("PNPDeviceID: {0}", queryObj["PNPDeviceID"]);
                Console.WriteLine("Description: {0}", queryObj["Description"]);
                Console.WriteLine("Name: {0}", queryObj["Name"]);
                Console.WriteLine("Status: {0}", queryObj["Status"]);
                Console.WriteLine("Manufacturer: {0}", queryObj["Manufacturer"]);
                Console.WriteLine("PNPClass: {0}", queryObj["PNPClass"]);
                Console.WriteLine("Service: {0}", queryObj["Service"]);
                Console.WriteLine("Caption: {0}", queryObj["Caption"]);
                Console.WriteLine("PNPClass: {0}", queryObj["PNPClass"]);
                Console.WriteLine("CreationClassName: {0}", queryObj["CreationClassName"]);
                Console.WriteLine("SystemCreationClassName: {0}", queryObj["SystemCreationClassName"]);
                Console.WriteLine("SystemName: {0}", queryObj["SystemName"]);
                */
                #endregion
            }
            collection.Dispose();
        }
        #endregion

        #region//==================================================USBCommPort_Start_FTDI
        private void USBCommPort_Start_FTDI(int index)
        {
            iSelectedFTDIDevice = index;                            // Selected Device
            iSelectedVComDevice = -1;                               // Not selected
            myUSBComm.FTDI_Device_Count = (uint)(index + 1);        //selected device
            myGlobalBase.USB_SelectDeviceMode = (int)GlobalBase.eSerialDeviceSelect.USB_UART_FTDI;
            myUSBComm.FTDI_Seek_Open_FT232R_USB_UART_Module(true);  // true = Use FTDI_Device_Count to select. 
            myGlobalBase.is_Serial_Server_Connected = true;

        }
        #endregion

        #region//==================================================USBCommPort_Start_VCOM
        private bool USBCommPort_Start_VCOM(int index)
        {
            myGlobalBase.USB_SelectDeviceMode = (int)GlobalBase.eSerialDeviceSelect.USB_UART_VCOM;
            iSelectedFTDIDevice = -1;                                   // Not selected
            iSelectedVComDevice = index;                 // Selected Device
            /*
            if (VCOMdevices[iSelectedVComDevice].PnpDeviceID.Contains("EVKI") == false)
            {
                myRtbTermMessage("+WARN: ----------------------------------\n");
                myRtbTermMessage("+WARN: This is not approved EZKIT Device!\n");
                myRtbTermMessage("+WARN: ----------------------------------\n");
            }
            else
            {
                myRtbTermMessage("-INFO: Confirmed Attached EVKIT Module\n");
            }
            */
            myUSBVCOMComm.VCOM_SerialInit(VCOMdevices[iSelectedVComDevice].DeviceID);   // Comm Port Number (ie COM9)
            if (myUSBVCOMComm.VCOM_Start_RX_Operation() == true)
            {
                myGlobalBase.is_Serial_Server_Connected = true;
                return true;        // True = close this window.
            }
            else
            {
                iSelectedVComDevice = -1;
            }
            return false;           // False = Do not close window.
        }
        #endregion

        //##########################################################################################
        //############################################################################# Array VCOM 
        //##########################################################################################

        #region//==================================================ScanPortNowArray
        public void ScanPortNowArray(int eSelectType, bool noText)       // Use eUSBDeviceType to select 
        {
            iSelectedPortArrayIndex = eSelectType;
            isSerialPortArray = true;
            dgvComList.Rows.Clear();        // Clear all old COM data.
            dgvComList.Refresh();
            // Update debug message. 
            if (noText == false)
            {
                myRtbTerm.ForeColor = myGlobalBase.ColorMessage;
                myRtbTermMessage("Identifying VCOM for Selected Device No:" + iSelectedPortArrayIndex.ToString() + "\n");
                myRtbTermMessage("----------------------------------\n");
            }
            ScanVcomNowArray(noText);
            if (noText == false)
            { 
                myRtbTermMessage("----------------------------------\n");
            }


        }
        #endregion

        #region//==================================================ScanVcomNowArray
        private void ScanVcomNowArray(bool NoText)
        {
            GetUSBDevicesArray();
            try
            {
                foreach (var usbDevice in VCOMdevicesArray)
                {
                    string[] row1 = new string[] { "VCOM", usbDevice.DeviceID.ToString(), "", usbDevice.PnpDeviceID.ToString() };
                    dgvComList.Rows.Add(row1);
                    row1 = null;
                    if (NoText == false)
                    {
                        myRtbTermMessage("==>VCOM Found No: " + usbDevice.DeviceID.ToString() +
                            "\n,    Description: " + usbDevice.PnpDeviceID.ToString() +
                            "\n,    Status: " + usbDevice.status.ToString() +
                            "\n\n");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error: VCOM scan or datagridview issue", "Error in VCOM/FTDI Display in DataGridView (After Scan)",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            }
            dgvComList.Refresh();
        }
        #endregion

        #region//==================================================GetUSBDevicesArray().....WPI Method (preferred)
        //private List<U=SBDeviceInfo> GetUSBDevices()   
        private void GetUSBDevicesArray()
        {
            if (VCOMdevicesArray == null)
            {
                VCOMdevicesArray = new List<USBDeviceInfo>();
            }
            VCOMdevicesArray.Clear();
            ManagementObjectCollection collection;
            //--------------------------------------------
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\""))
                collection = searcher.Get();
            foreach (ManagementObject queryObj in collection)
            {
                string name = (string)queryObj.GetPropertyValue("Name");
                if (name.Contains("(COM"))
                    name = name.Substring(name.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")", string.Empty);

                VCOMdevicesArray.Add(new USBDeviceInfo(
                name,                                                 //(string)queryObj.GetPropertyValue("DeviceID"),
                (string)queryObj.GetPropertyValue("PNPDeviceID"),     // Include VID/PID and Serial String (with EVKIT for pro or EVKIM for mini)
                (string)queryObj.GetPropertyValue("Description"),
                (string)queryObj.GetPropertyValue("Name"),
                (string)queryObj.GetPropertyValue("Status")
                ));
            }
            collection.Dispose();
        }
        #endregion

        #region//==================================================USBCommPort_Start_VCOMArray
        private bool USBCommPort_Start_VCOMArray(int index)
        {
            iSelectedVComDevice = index;                                             // Selected Device
            myUSBVCOMComm.VCOMArray_SerialInit(VCOMdevicesArray[iSelectedVComDevice].DeviceID, iSelectedPortArrayIndex);   // Comm Port Number (ie COM9)
            if (myUSBVCOMComm.VCOMArray_Start_RX_Operation(iSelectedPortArrayIndex) == false)
            {
                iSelectedVComDevice = -1;        // Not selected.
                return true;                    // False = Do not close window. 
            }
            return true;                        // True = close this window.
        }
        #endregion

        #region//==================================================USBCommPort_Close_VCOMArray
        private bool USBCommPort_Close_VCOMArray(int eSelectType)       // Use eUSBDeviceType to select
        {
            if (myUSBVCOMComm.VCOMserialPortArray[eSelectType].IsOpen == false)
                return false;
            myUSBVCOMComm.VCOMserialPortArray[eSelectType].Close();
            return true;
        }
        #endregion

        //##########################################################################################
        //############################################################################# Other codes
        //##########################################################################################

        #region//==================================================GetListWin32SerialPort()
        public void GetListWin32SerialPort()
        {
            ConnectionOptions options = ProcessConnection.ProcessConnectionOptions();
            ManagementScope connectionScope = ProcessConnection.ConnectionScope(Environment.MachineName, options, @"\root\CIMV2");
            ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM win32_serialport ");
            ManagementObjectSearcher win32SerialPortSearcher = new ManagementObjectSearcher(connectionScope, objectQuery);
            List<CWin32SerialPort> win32SerialPortList = null;
            using (win32SerialPortSearcher)
            {
                foreach (ManagementObject obj in win32SerialPortSearcher.Get())
                {
                    if (win32SerialPortList == null)
                    {
                        win32SerialPortList = new List<CWin32SerialPort>();
                    }
                    
                    CWin32SerialPort win32SerialPort = new CWin32SerialPort();

                    win32SerialPort.Name = obj["Name"].ToString();
                    win32SerialPort.Description = obj["Description"].ToString();
                    win32SerialPort.Caption = obj["Caption"].ToString();
                    win32SerialPort.DeviceID = obj["DeviceID"].ToString();
                    win32SerialPort.PNPDeviceID = obj["PNPDeviceID"].ToString();
                    win32SerialPortList.Add(win32SerialPort);
                }
            }
        }
        #endregion

        #region//==================================================CWin32SerialPort Class
        public class CWin32SerialPort
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Caption { get; set; }
            public string DeviceID { get; set; }
            public string PNPDeviceID { get; set; }

            public CWin32SerialPort()
            {
            }
        }

        #endregion

        #region//==================================================USBCommPort_Twinkle_Led
        private void USBCommPort_Twinkle_Led(int index)
        {
            if (isSerialPortArray == false)
            {
                if (index < FTDIs0.Count)
                {
                    USBCommPort_Start_FTDI(index); // Open Port
                    myUSBComm.FTDI_Message_Send("TWINKLE()"); // Send message
                    myUSBComm.FTDI_Message_ClosePort(); // Close Port
                    return;
                }
                //-------------------------VCOM Only
                USBCommPort_Start_VCOM(index - FTDIs0.Count); // Open Port
                myUSBVCOMComm.VCOM_Message_Send("TWINKLE()"); // Send message
                myUSBVCOMComm.VCOM_Message_ClosePort(); // Close Port
            }
            else
            {
                USBCommPort_Start_VCOM(index); // Open Port
                myUSBVCOMComm.VCOM_Message_Send("TWINKLE()"); // Send message
                myUSBVCOMComm.VCOM_Message_ClosePort(); // Close Port
            }
        }
        #endregion

        #region//==================================================dgvComList_CellClick
        private void dgvComList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            int index = e.RowIndex;
            if (e.ColumnIndex == 2)       // Twinkle
            {
                USBCommPort_Twinkle_Led(index);
                return;
            }
            myGlobalBase.is_Serial_Server_Connected = false;
            dgvComList.Rows[index].Selected = true;
            if (isSerialPortArray == false)     // Single VCOM or FTDI port. 
            {
                //-----------------------------------------------FTDI device
                if (index < FTDIs0.Count)
                {
                    USBCommPort_Start_FTDI(index);
                    this.Hide();
                    return;
                }
                //----------------------------------------------VCOM device
                if (USBCommPort_Start_VCOM(index - FTDIs0.Count) == true)
                    this.Hide();
            }
            else // Array Port Device just VCOM type only (no direct FTDI under Dxxx.
            {
                if (USBCommPort_Start_VCOMArray(index) == true)
                    this.Hide();
            }

        }
        #endregion

        #region//==================================================dgvComList_CellMouseLeave
        private void dgvComList_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) //column header / row headers
            {
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                this.dgvComList.Rows[e.RowIndex].Cells[i].Style.BackColor = Color.White;
            }
        }
        #endregion

        #region//==================================================dgvComList_CellMouseEnter
        private void dgvComList_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) //column header / row headers
            {
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                this.dgvComList.Rows[e.RowIndex].Cells[i].Style.BackColor = Color.LightSkyBlue;      //LightYellow;
            }
        }
        #endregion

        #region//==================================================CompanyName_Theme_Update
        private void CompanyName_Theme_Update()
        {
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
                        this.BackgroundImage = null;       // Default
                        break;
                    }
            }
            this.Refresh();
        }
        #endregion

        #region//==================================================btnRescan_Click
        private void btnRescan_Click(object sender, EventArgs e)
        {
            if (isSerialPortArray == true)
                ScanPortNowArray(iSelectedPortArrayIndex,false);
            else
                ScanPortNow();
        }
        #endregion

        #region//==================================================btnExit_Click
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        #endregion

        #region//==================================================btnHelp_Click
        private void btnHelp_Click(object sender, EventArgs e)
        {
            FlexiMessageBox.FONT = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            FlexiMessageBox.Show("This window provides information about the attached device for user to select."
                + Environment.NewLine + "It support two kind of serial ports attached to USB 2.0 (USB 3.0 support under review)"
                + Environment.NewLine + "  i) FTDI USB/UART device based on FT232RL chipset only. This operate under D2XX driver by FTDI."
                + Environment.NewLine + " ii) Virtual Comm for any kind of device with VCOM protocol. This operate under .NET SerialPort driver."
                + Environment.NewLine + "To identify which board that is connected, click Blink button, this would twinkle on board's LED"
                + Environment.NewLine + "The range of COM number is between 1 and 64. If selected from the list, it automatically open this port."
                + Environment.NewLine + "In case of one FTDI device in the list and FTDI checked only then it will automatically open this port."
                + Environment.NewLine + "For EVKIT Module, make sure to select EVKIT or EVKIM within description under VCOM (FTDI may be unchecked)."
                + Environment.NewLine + "                  in case of non-EVKIT module, it give warning only."
                + Environment.NewLine + "NB: Only one serial connection is supported per UDT instance."
                + Environment.NewLine + "NB: Validate Baud rate setting to ensure match between them. Refer to manual for more details.",
                "Serial Comm Port Selection Manager",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

        }
        #endregion

        #region //==================================================myRtbTermMessage
        //==========================================================
        // Purpose  : Append message in terminal window,. 
        // Input    :  
        // Output   : 
        // Status   :
        //==========================================================
        private void myRtbTermMessage(string Message)
        {
            if (myGlobalBase.isTermScreenHalted == false)
            {
                Tools.rtb_StopRepaint(myRtbTerm, rtbOEMcom);
                myRtbTerm.SelectionFont = myGlobalBase.FontResponse;
                myRtbTerm.SelectionColor = myGlobalBase.ColorResponse;
                myRtbTerm.SelectionStart = myRtbTerm.TextLength;
                myRtbTerm.ScrollToCaret();
                myRtbTerm.Select();
                myRtbTerm.AppendText(myGlobalBase.TermHaltMessageBuf + Message);
                myGlobalBase.TermHaltMessageBuf = "";
                Tools.rtb_StartRepaint(myRtbTerm, rtbOEMcom);
            }
            else
            {
                myGlobalBase.TermHaltMessageBuf += Message;
            }
        }
        #endregion

        //##########################################################################################
        //############################################################################# Test Bed
        //##########################################################################################

        //################################################################################## USB Experiment Code using WPI, but will not get descriptor.
        #region----Old Stuff
        //GetListWin32SerialPort();
        //return VCOMdevices;
        /*
        // Below is the old method, which did not alway work best, we try Win32_PnPEntity and see how it goes. 
        using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_SerialPort "))
            collection = searcher.Get();
        foreach (var device in collection)
        {
            VCOMdevices.Add(new USBDeviceInfo(
            (string)device.GetPropertyValue("DeviceID"),
            (string)device.GetPropertyValue("PNPDeviceID"),     // Include VID/PID and Serial String (with EVKIT for pro or EVKIM for mini)
            (string)device.GetPropertyValue("Description"),
            (string)device.GetPropertyValue("Name"),
            (string)device.GetPropertyValue("Status")
            ));
        }
        collection.Dispose();
        */
        /*
        ManagementObjectCollection collection;
        using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub "))
            collection = searcher.Get();
        foreach (var device in collection)
        {
        VCOMdevices.Add(new USBDeviceInfo(
        (string)device.GetPropertyValue("DeviceID"),
        (string)device.GetPropertyValue("PNPDeviceID"),     // Include VID/PID and Serial String (with EVKIT for pro or EVKIM for mini)
        (string)device.GetPropertyValue("Description"),
        (string)device.GetPropertyValue("Name"),
        (string)device.GetPropertyValue("Status")
        ));
        //-----Example
        /*
        DeviceIDString = (string)device.GetPropertyValue("PNPDeviceID");
        if (DeviceIDString.Contains("VID_1FC9"))
        {
            DeviceIDString = "";
        }
        */
        //TASK is to filter the data to detect VID and PID that match to the device or to detect IDT in message "USB\VID_1FC9&PID_0083\IDT-EVKIT-VCOM-RV001"
        //Use string find the VID and PID for match plus EVKIT. 

        // Win32_SerialPort provide below result which linked COM6 and VID/PID
        /*
            Device ID: COM9
            , PNP Device ID: USB\VID_1FC9&PID_0083\EVKIT_SR123456789ABC
            , Description: LPC USB VCom Port
            , Name: LPC USB VCom Port
            , Status: OK
        */
        // Win32_USBHub provide below results
        /*
            Device ID: USB\VID_1FC9&PID_0083\EVKIT_SR123456789ABC
            , PNP Device ID: USB\VID_1FC9&PID_0083\EVKIT_SR123456789ABC
            , Description: LPC USB VCom Port
            , Name: LPC USB VCom Port
        */
        // Conclusion
        //--------------
        // The issue with LibUSBDotNet that it need to run  additional task (ie libusb filter) to make LibUSBDotNet driver work with VCOM, this is not ideal arrangement. 
        // So we need Win32_SerialPort 
        // and then use Device ID and PNP Device ID
        // Device ID provide COM number
        // PNP Device ID can be broken down to
        //    VID
        //    PID
        //    Name EVKIT = EVKIT Main Board, EVKIM = EVKIT Mini Board type (no reference to specific ZMDI chip)
        //    SR = Serial Number until end of line. 
        // No need to obtain descriptor. 
        // Name and description are useless, it come from ROM element. 
        // Rely on LED twinkle to identify the the device. 
        // Within the EVKIT, it is important to get descriptor right
        //    VID = 0x1FC9
        //    PID = 0x0083 for PRO, 0x0084 for Mini
        //    Serial: has two part string: EVKIT_SR123456789ABC
        //    Other descriptor are not used.
        // 
        // Further Note of different WPI
        // -----------------------------
        // Win32_SerialPort (which give VID/PID and COM)
        // Win32_USBHub (which give VID/PID and Descriptor)  // 
        // using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity"))
        // Win32_USBController this is for chip set only, not related to SerialPort. 
        // Win32_SerialPortConfiguration
        #endregion

        #region//==================================================Experiment using WPI Method (preferred)
        public void Experiment()
        {
            GetUSBDevices();
            try
            {
                foreach (var usbDevice in VCOMdevices)
                {
                    myRtbTerm.SelectionColor = myGlobalBase.ColorResponse;
                    myRtbTermMessage("Device ID: " + usbDevice.DeviceID.ToString() +
                        "\n, PNP Device ID: " + usbDevice.PnpDeviceID.ToString() +
                        "\n, Description: " + usbDevice.Description.ToString() +
                        "\n, Name: " + usbDevice.Description.ToString() +
                        "\n, Status: " + usbDevice.status.ToString() +
                        "\n\n\n");
                }
            }
            catch { }
        }
        #endregion

        #region//==================================================Experiment2 (LibUSBDotNet)......obsolete use. 
        //#################################################################################USB Experimental Code, using LibUSBDotNet which get descriptor.
        //http://libusbdotnet.sourceforge.net/V2/html/6ab39c2b-8611-46bf-8066-bfe79ae9f1f9.htm
        public static UsbDevice MyUsbDevice;

        public void Experiment2()
        {
            // Dump all devices and descriptor information to console output.
            UsbRegDeviceList allDevices = UsbDevice.AllDevices;
            foreach (UsbRegistry usbRegistry in allDevices)
            {
                if (usbRegistry.Open(out MyUsbDevice))
                {
                    myRtbTermMessage((MyUsbDevice.Info.ToString() + "\n"));
                    for (int iConfig = 0; iConfig < MyUsbDevice.Configs.Count; iConfig++)
                    {
                        UsbConfigInfo configInfo = MyUsbDevice.Configs[iConfig];
                        myRtbTermMessage((configInfo.ToString() + "\n"));

                        /* Interface and Endpoint is not needed for this project
                        ReadOnlyCollection<UsbInterfaceInfo> interfaceList = configInfo.InterfaceInfoList;
                        for (int iInterface = 0; iInterface < interfaceList.Count; iInterface++)
                        {
                            UsbInterfaceInfo interfaceInfo = interfaceList[iInterface];
                            Console.WriteLine(interfaceInfo.ToString());

                            ReadOnlyCollection<UsbEndpointInfo> endpointList = interfaceInfo.EndpointInfoList;
                            for (int iEndpoint = 0; iEndpoint < endpointList.Count; iEndpoint++)
                            {
                                Console.WriteLine(endpointList[iEndpoint].ToString());
                            }
                        }
                        */
                    }
                }
            }
            // Free usb resources.
            // This is necessary for libusb-1.0 and Linux compatibility.
            UsbDevice.Exit();
        }


        //This Generate the code
        /*
            Length:18
            DescriptorType:Device
            BcdUsb:0x0200
            Class:239
            SubClass:0x02
            Protocol:0x01
            MaxPacketSize0:64
            VendorID:0x1FC9
            ProductID:0x0083
            BcdDevice:0x0100
            ManufacturerStringIndex:1
            ProductStringIndex:2
            SerialStringIndex:3
            ConfigurationCount:1
            ManufacturerString:IDT
            ProductString:EVKIT_R010
            SerialString:EVKIT_SR123456789ABC

            Length:9
            DescriptorType:Configuration
            TotalLength:75
            InterfaceCount:2
            ConfigID:1
            StringIndex:0
            Attributes:0xC0
            MaxPower:250
            ConfigString:
            */
        #endregion

        #region//==================================================Experimental code, obtain COM and device description. 
        internal class ProcessConnection
        {

            public static ConnectionOptions ProcessConnectionOptions()
            {
                ConnectionOptions options = new ConnectionOptions();
                options.Impersonation = ImpersonationLevel.Impersonate;
                options.Authentication = AuthenticationLevel.Default;
                options.EnablePrivileges = true;
                return options;
            }

            public static ManagementScope ConnectionScope(string machineName, ConnectionOptions options, string path)
            {
                ManagementScope connectScope = new ManagementScope();
                connectScope.Path = new ManagementPath(@"\\" + machineName + path);
                connectScope.Options = options;
                connectScope.Connect();
                return connectScope;
            }
        }

        public class COMPortInfo
        {
            public string Name { get; set; }
            public string Description { get; set; }

            public COMPortInfo() { }

            public static List<COMPortInfo> GetCOMPortsInfo()
            {
                List<COMPortInfo> comPortInfoList = new List<COMPortInfo>();

                ConnectionOptions options = ProcessConnection.ProcessConnectionOptions();
                ManagementScope connectionScope = ProcessConnection.ConnectionScope(Environment.MachineName, options, @"\root\CIMV2");

                ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0");
                ManagementObjectSearcher comPortSearcher = new ManagementObjectSearcher(connectionScope, objectQuery);

                using (comPortSearcher)
                {
                    string caption = null;
                    foreach (ManagementObject obj in comPortSearcher.Get())
                    {
                        if (obj != null)
                        {
                            object captionObj = obj["Caption"];
                            if (captionObj != null)
                            {
                                caption = captionObj.ToString();
                                if (caption.Contains("(COM"))
                                {
                                    COMPortInfo comPortInfo = new COMPortInfo();
                                    comPortInfo.Name = caption.Substring(caption.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")",
                                                                         string.Empty);
                                    comPortInfo.Description = caption;
                                    comPortInfoList.Add(comPortInfo);
                                }
                            }
                        }
                    }
                }
                return comPortInfoList;
            }
        }

        public void Experiment_COM_Description_Method()
        {
            // https://dariosantarelli.wordpress.com/2010/10/18/c-how-to-programmatically-find-a-com-port-by-friendly-name/
            foreach (COMPortInfo comPort in COMPortInfo.GetCOMPortsInfo())
            {
                //Console.WriteLine(string.Format("{0} – {1}", comPort.Name, comPort.Description));
                myRtbTermMessage("Name: " + comPort.Name + " || Desc: " + comPort.Description + "\n\n\n");


            }


        }
        #endregion
    }
    #region//==================================================Class USBDeviceInfo
    class USBDeviceInfo
        {
        public USBDeviceInfo(string deviceID, string pnpDeviceID, string description, string name, string status)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
            this.Name = name;
            this.status = status;
        }
        public string DeviceID { get; private set; }
        public string PnpDeviceID { get; private set; }
        public string Description { get; private set; }
        public string Name { get; private set; }
        public string status { get; private set; }
    }
    #endregion


}



