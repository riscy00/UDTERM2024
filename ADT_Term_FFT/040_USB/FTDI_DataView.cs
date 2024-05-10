using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FTD2XX_NET;

namespace UDT_Term_FFT
{
    public partial class FTDI_DataView : Form
    {
        FTDI FTDI_DeviceData; 

        public FTDI_DataView()
        {
            InitializeComponent();
        }

        public void UpdateFTDIDevice(FTDI FTDI_DeviceDataRef)       // Copy reference from USB_FTI_Comm into this window (for read only access data)
        {
            FTDI_DeviceData = FTDI_DeviceDataRef;
        }
        
        private void FTDI_DataView_Load(object sender, EventArgs e) // When form open, it activate this routine.
        {
            string data;
            FTDI.FT232R_EEPROM_STRUCTURE data_status = new FTDI.FT232R_EEPROM_STRUCTURE();
            FTDI.FT_DEVICE data_device = new FTDI.FT_DEVICE();
            DataView.DataSource = null;                    // Clear Data
            DataView.RowCount=0;
            FTDI_DeviceData.GetCOMPort(out data);
            insertRow("Comm Port", data);
            FTDI_DeviceData.GetDescription(out data);
            insertRow("Description", data);
            FTDI_DeviceData.GetSerialNumber(out data);
            insertRow("Serial Number", data);
            FTDI_DeviceData.GetDeviceType(ref data_device);
            insertRow("Device Type", data_device.ToString());
            insertRow("Baud Rate Set:", "19200");
            insertRow("Data Bit Set:","8");
            insertRow("Stop Bit Set:", "1");
            insertRow("Parity Set:", "None");
         
            /*
            if (data_device == FTDI.FT_DEVICE.FT_DEVICE_232R)
            {
                try
                {
                FTDI_DeviceData.ReadFT232REEPROM(data_status);
                insertRow("Product ID", data_status.ProductID.ToString());
                insertRow("Vendor ID", data_status.VendorID.ToString());
                }
                catch (FTDI.FT_EXCEPTION)
                {
                    insertRow("Product ID", "Unable to read EEPROM Content");
                    insertRow("Vendor ID", "Unable to read EEPROM Content");
                }

            }
             */
            //----------------------------------------Optimise Window Box for better view
            int HeightRow = 25 * (DataView.RowCount);
            this.DataView.Size = new System.Drawing.Size(308, HeightRow);
            this.Size = new System.Drawing.Size(346, HeightRow+60);  
        }
        //=============================================================== insertRow
        private void insertRow(string sObject, string sData)
        {
            int n;
            n = DataView.Rows.Add();
            DataView.Rows[n].Cells[0].Value = sObject;
            DataView.Rows[n].Cells[1].Value = sData;
        }
    }
}

/*

 * 
 * 
        //
        // Summary:
        //     Gets the description of the current device.
        //
        // Parameters:
        //   Description:
        //     The description of the current device.
        //
        // Returns:
        //     FT_STATUS value from FT_GetDeviceInfo in FTD2XX.DLL
        public FTDI.FT_STATUS GetDescription(out string Description);
        //
        // Summary:
        //     Gets the Vendor ID and Product ID of the current device.
        //
        // Parameters:
        //   DeviceID:
        //     The device ID (Vendor ID and Product ID) of the current device.
        //
        // Returns:
        //     FT_STATUS value from FT_GetDeviceInfo in FTD2XX.DLL
        public FTDI.FT_STATUS GetDeviceID(ref uint DeviceID);
        //
        // Summary:
        //     Gets information on all of the FTDI devices available.
        //
        // Parameters:
        //   devicelist:
        //     An array of type FT_DEVICE_INFO_NODE to contain the device information for
        //     all available devices.
        //
        // Returns:
        //     FT_STATUS value from FT_GetDeviceInfoDetail in FTD2XX.DLL
        //
        // Exceptions:
        //   FTD2XX_NET.FTDI.FT_EXCEPTION:
        //     Thrown when the supplied buffer is not large enough to contain the device
        //     info list.
        public FTDI.FT_STATUS GetDeviceList(FTDI.FT_DEVICE_INFO_NODE[] devicelist);
        //
        // Summary:
        //     Gets the chip type of the current device.
        //
        // Parameters:
        //   DeviceType:
        //     The FTDI chip type of the current device.
        //
        // Returns:
        //     FT_STATUS value from FT_GetDeviceInfo in FTD2XX.DLL
        public FTDI.FT_STATUS GetDeviceType(ref FTDI.FT_DEVICE DeviceType);
        //
        // Summary:
        //     Gets the current FTDIBUS.SYS driver version number.
        //
        // Parameters:
        //   DriverVersion:
        //     The current driver version number.
        //
        // Returns:
        //     FT_STATUS value from FT_GetDriverVersion in FTD2XX.DLL
        public FTDI.FT_STATUS GetDriverVersion(ref uint DriverVersion);
        //
        // Summary:
        //     Gets the event type after an event has fired. Can be used to distinguish
        //     which event has been triggered when waiting on multiple event types.
        //
        // Parameters:
        //   EventType:
        //     The type of event that has occurred.
        //
        // Returns:
        //     FT_STATUS value from FT_GetStatus in FTD2XX.DLL
        public FTDI.FT_STATUS GetEventType(ref uint EventType);
        //
        // Summary:
        //     Gets the value of the latency timer. Default value is 16ms.
        //
        // Parameters:
        //   Latency:
        //     The latency timer value in ms.
        //
        // Returns:
        //     FT_STATUS value from FT_GetLatencyTimer in FTD2XX.DLL
        public FTDI.FT_STATUS GetLatency(ref byte Latency);
        //
        // Summary:
        //     Gets the current FTD2XX.DLL driver version number.
        //
        // Parameters:
        //   LibraryVersion:
        //     The current library version.
        //
        // Returns:
        //     FT_STATUS value from FT_GetLibraryVersion in FTD2XX.DLL
        public FTDI.FT_STATUS GetLibraryVersion(ref uint LibraryVersion);
        //
        // Summary:
        //     Gets the current line status.
        //
        // Parameters:
        //   LineStatus:
        //     A bit map representaion of the current line status.
        //
        // Returns:
        //     FT_STATUS value from FT_GetModemStatus in FTD2XX.DLL
        public FTDI.FT_STATUS GetLineStatus(ref byte LineStatus);
        //
        // Summary:
        //     Gets the current modem status.
        //
        // Parameters:
        //   ModemStatus:
        //     A bit map representaion of the current modem status.
        //
        // Returns:
        //     FT_STATUS value from FT_GetModemStatus in FTD2XX.DLL
        public FTDI.FT_STATUS GetModemStatus(ref byte ModemStatus);
        //
        // Summary:
        //     Gets the number of FTDI devices available.
        //
        // Parameters:
        //   devcount:
        //     The number of FTDI devices available.
        //
        // Returns:
        //     FT_STATUS value from FT_CreateDeviceInfoList in FTD2XX.DLL
        public FTDI.FT_STATUS GetNumberOfDevices(ref uint devcount);
        //
        // Summary:
        //     Gets the instantaneous state of the device IO pins.
        //
        // Parameters:
        //   BitMode:
        //     A bitmap value containing the instantaneous state of the device IO pins
        //
        // Returns:
        //     FT_STATUS value from FT_GetBitMode in FTD2XX.DLL
        public FTDI.FT_STATUS GetPinStates(ref byte BitMode);
        //
        // Summary:
        //     Gets the number of bytes available in the receive buffer.
        //
        // Parameters:
        //   RxQueue:
        //     The number of bytes available to be read.
        //
        // Returns:
        //     FT_STATUS value from FT_GetQueueStatus in FTD2XX.DLL
        public FTDI.FT_STATUS GetRxBytesAvailable(ref uint RxQueue);
        //
        // Summary:
        //     Gets the serial number of the current device.
        //
        // Parameters:
        //   SerialNumber:
        //     The serial number of the current device.
        //
        // Returns:
        //     FT_STATUS value from FT_GetDeviceInfo in FTD2XX.DLL
        public FTDI.FT_STATUS GetSerialNumber(out string SerialNumber);
        //
        // Summary:
        //     Gets the number of bytes waiting in the transmit buffer.
        //
        // Parameters:
        //   TxQueue:
        //     The number of bytes waiting to be sent.
        //
        // Returns:
        //     FT_STATUS value from FT_GetStatus in FTD2XX.DLL
        public FTDI.FT_STATUS GetTxBytesWaiting(ref uint TxQueue);
 */
