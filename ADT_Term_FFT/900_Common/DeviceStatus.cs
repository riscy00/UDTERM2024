using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;

namespace LINFLASHER
{
    public class StatusDataType
    {

            //===================================================================Hex Data 
            byte[] hexIntelData;

            //===================================================================RXDataBuffer (for USB-LIN device) (ASCII DATA ONLY)
            byte[] m_RXDataBuffer;                       // Raw LIN data buffer

            private int m_RXDataBufferPointer;      // Array pointer
            public int RXDataBufferPointer      { get { return m_RXDataBufferPointer; } }
            public byte[] RXDataBuffer           { get { return m_RXDataBuffer; } }

            //===================================================================Status Parameter
            private byte m_Flash;               // Flash Status Register
            private byte m_Debug;               // Debug Status Register
            private byte m_Operation;           // Operation Status Register
            private byte m_Dataframe;           // Dataframe Status Register
            private byte m_FaultCounter;        // Fault counter report in case of issue within the device
            private UInt16 m_LINCounter;        // for LIN testing purpose, apply for command 0x00 only

            //===================================================================Internal Parameter
            private bool m_IsValidData;         // If true, the above data is valid otherwise false, where value is not updated due to error.
            private bool m_isCRCAdded;          // false, do not add CRC, true Add CRC 
            private bool m_isLin2X;             // true for 2.0 and 2.1 LIN Protocol for CRC calculation. 
            private bool m_isLIN_PID_Type;      // true if PID is used, other false for ID
            private bool m_isUSBLIN_Connected;  // true if USBLIN device is connected, otherwise false.     
            private bool m_isDevice_Connected;  // true if device is connected under LIN calling loop, otherwise false (prevent further activity).

            //===================================================================Device Parameter
            private int m_RecordNumberPerPage;  //((32 * 16 byte per record = 512 byte data) loop 0 - 31 = 32 records 
            private UInt16 m_BytePerPage;       // 0x200 = 512 byte per page, 0x100 = 256 byte per page
            private int m_StartDeviceAddress;   // Start Address to flash. 
            private int m_StopDeviceAddress;    // Maximum Flash Memory Limits

            //===================================================================Device Information: Operation (downloaded from the device)

            private Byte    m_OP10_BootloaderVersionMajor;
            private Byte    m_OP10_BootloaderVersionMinor;
            private UInt32  m_OP11_DeviceRevisionNumber;
            private UInt32  m_OP12_DeviceProjectNumber;
            private UInt32  m_OP13_DeviceProjectVariant;
            private Byte    m_OP14_DeviceD4_OSCH_TRIM;
            private Byte    m_OP14_DeviceD5_OSCL_TRIM;
            private Byte    m_OP14_DeviceD6_CSREF_TRIM;
            private Byte    m_OP14_DeviceD7_Spare;

            private UInt16  m_OP00_CallingCode;           // This is used for Master to validate device by calling, if it read 0x5A5A, then it LIN communication is active. 

            //===================================================================Device Information (downloaded from the device)


            private UInt16 m_FC01_MCU_ADDRRAM_PAGE;            // Location of SRAM for flash controller service, fixed by bootloader (page format)
            private UInt16 m_FC02_MCU_FLASHADDR_PAGE;          // Location of FLASH to be flashed over from SRAM. This need to be set after dataframe transfer. (page format, 7 = 0x0000_0E00)
           
            private UInt16 m_FC03_Page_ByteSize;          // Page definition size, typically 512 for MIRA device (512 = 0x200)
            private UInt16 m_FC05_EraseStopPage;          // Define Stop Page for erase 
            private UInt16 m_FC05_EraseStartPage;         // Define Start Page for erase
            private Byte   m_FC08_VerifyPageCounterError; // Number of mismatch error during byte to byte compare between SRAM and FLASH (select by SelectFlashPage)
            private UInt32 m_FC09_CheckSumResultFlash;    // Result of the checksum on flash page selected by SelectFlashPage            
            private UInt32 m_FC0A_CheckSumResultSRAM;     // Result of the checksum on flash page selected by SelectFlashPage

            // ==================================================================Getter/Setter

            public byte Flash               { get { return m_Flash; }                   set { m_Flash = value; } }
            public byte Debug               { get { return m_Debug; }                   set { m_Debug = value; } }
            public byte Operation           { get { return m_Operation; }               set { m_Operation = value; } }
            public byte Dataframe           { get { return m_Dataframe; }               set { m_Dataframe = value; } }
            public byte FaultCounter        { get { return m_FaultCounter; }            set { m_FaultCounter = value; } }
            public UInt16 LINCounter        { get { return m_LINCounter; }              set { m_LINCounter = value; } }
            public UInt16 OP_CONFIG         { get { return m_OP_CONFIG; }               set { m_OP_CONFIG = value; } }
            public UInt16 OP_CONFIG_Spare   { get { return m_OP_CONFIG_Spare; }         set { m_OP_CONFIG_Spare = value; } }
            public int RecordNumberPerPage  { get { return m_RecordNumberPerPage; }     set { m_RecordNumberPerPage = value; } }
            public int StartDeviceAddress   { get { return m_StartDeviceAddress; }      set { m_StartDeviceAddress = value; } }
            public int StopDeviceAddress    { get { return m_StopDeviceAddress; }       set { m_StopDeviceAddress = value; } }

            public UInt16 OP00_CallingCode  { get { return m_OP00_CallingCode; }        set { m_OP00_CallingCode = value; } }
            public bool IsValidData         { get { return m_IsValidData; }             set { m_IsValidData = value; } }
            public bool isCRCAdded          { get { return m_isCRCAdded; }              set { m_isCRCAdded = value; } }
            public bool isLin2X             { get { return m_isLin2X; }                 set { m_isLin2X = value; } }
            public bool isLIN_PID_Type      { get { return m_isLIN_PID_Type; }          set { m_isLIN_PID_Type = value; } }
            public bool isUSBLIN_Connected  { get { return m_isUSBLIN_Connected; }      set { m_isUSBLIN_Connected = value; } }
            public bool isDevice_Connected  { get { return m_isDevice_Connected; }      set { m_isDevice_Connected = value; } }


            // =================================================================Getter/Setter for Operation

            public Byte OP10_BootloaderVersionMajor     { get { return m_OP10_BootloaderVersionMajor; }     set { m_OP10_BootloaderVersionMajor = value; } }
            public Byte OP10_BootloaderVersionMinor     { get { return m_OP10_BootloaderVersionMinor; }     set { m_OP10_BootloaderVersionMinor = value; } }
            public UInt32 OP11_DeviceRevisionNumber     { get { return m_OP11_DeviceRevisionNumber; }       set { m_OP11_DeviceRevisionNumber = value; } }
            public UInt32 OP12_DeviceProjectNumber      { get { return m_OP12_DeviceProjectNumber; }        set { m_OP12_DeviceProjectNumber = value; } }
            public UInt32 OP13_DeviceProjectVariant     { get { return m_OP13_DeviceProjectVariant; }       set { m_OP13_DeviceProjectVariant = value; } }
            public Byte   OP14_DeviceD4_OSCH_TRIM       { get { return m_OP14_DeviceD4_OSCH_TRIM; }         set { m_OP14_DeviceD4_OSCH_TRIM = value; } }
            public Byte   OP14_DeviceD5_OSCL_TRIM       { get { return m_OP14_DeviceD5_OSCL_TRIM; }         set { m_OP14_DeviceD5_OSCL_TRIM = value; } }
            public Byte   OP14_DeviceD6_CSREF_TRIM      { get { return m_OP14_DeviceD6_CSREF_TRIM; }        set { m_OP14_DeviceD6_CSREF_TRIM = value; } }
            public Byte   OP14_DeviceD7_Spare           { get { return m_OP14_DeviceD7_Spare; }             set { m_OP14_DeviceD7_Spare = value; } }

            // =================================================================Getter/Setter for Flash

            public UInt16   FC01_MCU_ADDRRAM_PAGE        { get { return m_FC01_MCU_ADDRRAM_PAGE; }          set { m_FC01_MCU_ADDRRAM_PAGE = value; } }
            public UInt16   FC02_MCU_FLASHADDR_PAGE      { get { return m_FC02_MCU_FLASHADDR_PAGE; }        set { m_FC02_MCU_FLASHADDR_PAGE = value; } }
            public UInt16   FC03_Page_ByteSize           { get { return m_FC03_Page_ByteSize; }             set { m_FC03_Page_ByteSize = value; } }
            public UInt16   FC05_EraseStopPage           { get { return m_FC05_EraseStopPage; }             set { m_FC05_EraseStopPage = value; } }
            public UInt16   FC05_EraseStartPage          { get { return m_FC05_EraseStartPage; }            set { m_FC05_EraseStartPage = value; } }
            public Byte     FC08_VerifyPageCounterError  { get { return m_FC08_VerifyPageCounterError; }    set { m_FC08_VerifyPageCounterError = value; } }
            public UInt32   FC09_CheckSumResultFlash     { get { return m_FC09_CheckSumResultFlash; }       set { m_FC09_CheckSumResultFlash = value; } }
            public UInt32   FC0A_CheckSumResultSRAM      { get { return m_FC0A_CheckSumResultSRAM; }        set { m_FC0A_CheckSumResultSRAM = value; } }

            // ==================================================================Constructor
            public StatusDataType()
            {
                DefaultStatus();
                DefaultFlash();
                Setup_ZAMC4100();
                hexIntelData = new byte[m_BytePerPage];      // 512 byte as default.
                //-----------------------------RX Terminal Data Buffer
                m_RXDataBuffer = new byte[128];         // Hopefully enough for to accomodate error message!      

                //-----------------------------
                m_IsValidData = true;
                m_isCRCAdded = false;                   // Add CRC byte at the end of the LIN Message. 
                m_isLin2X = true;                       // Include PID as CRC
                m_isLIN_PID_Type = false;               // Use ID number in LIN during TX message for ZMDI SSC Comm Board. 
            }

            // ==================================================================Status Default
            public void DefaultStatus()
            {
                m_Flash = 0;
                m_Debug = 0;
                m_Operation = 0;
                m_Dataframe = 0;
                m_FaultCounter = 0;
                m_LINCounter = 0;
            }
            // ==================================================================Status Default
            public void DefaultFlash()
            {
                m_FC02_MCU_FLASHADDR_PAGE = 7; // start from 0x0E00.
            }

            // ==================================================================Device Spec

            public void Setup_ZAMC4100()
            {
                //==================================Default Setup for MIRA device (ZMAC4100)
                m_RecordNumberPerPage = 32;
                m_BytePerPage = 0x200;              // 512 byte. 
                m_StartDeviceAddress = 0x00000E00;  // 0x0000 - 0x0DFF reserved for bootloader.
                m_StopDeviceAddress = 0x00001FFF;   // 32K FLASH
            }

            //=================================================================Configuration Bits 
            private UInt16 m_OP_CONFIG;     // Configuration register, define dataframe and operation behaviour. 
            private UInt16 m_OP_CONFIG_Spare;
            public enum OP_CONFIGBit
            {                           // Bits
                CONFIG_TRANSFER_MODE    = 0,        //  1 = Download (Device to PC),      0 = Upload (PC to Device)
                CONFIG_FORMAT           = 1,        //  1 = Intel Hex File format,        0 = Pure Hex only (future)	
                CONFIG_ENABLE_FLASHING  = 2,        //  1 = FLASHING is permitted,         0 = FLASHING is not permitted.
                CONFIG_LIN_VERSION      = 3         //  1 = LIN1.3,                       0 = LIN2.0
            }

            public void OP_CONFIG_Set(OP_CONFIGBit bit)
            {
               m_OP_CONFIG = (UInt16) Set((UInt32)m_OP_CONFIG, (int)bit); 
            }
            public void OP_CONFIG_Clear(OP_CONFIGBit bit)
            {
                m_OP_CONFIG = (UInt16)Clear((UInt32)m_OP_CONFIG, (int)bit); 
            }
            public bool OP_CONFIG_IsSet(OP_CONFIGBit bit)
            {
                return IsSet((UInt32)m_OP_CONFIG, (int)bit);
            }

            //===================================================================Bit Access tools
            //--------------------UInt32, overload.
            private UInt32 Set(UInt32 ba, int bit)
            {
                ba |= 1U << bit;
                return ba;
            }
            private UInt32 Clear(UInt32 ba, int bit)
            {
                UInt32 mask = 1U << bit;
                mask = ~mask;
                ba &= mask;
                return ba;
            }
            private bool IsSet(UInt32 ba, int bit)
            {
                UInt32 mask = 1U << bit;
                return (ba & mask) != 0;
            }

            //===================================================================RXDataBufferAppend
            public void RXDataBufferAppend(byte[] RXbyte)
            {
                int i = 0;
                while (i < RXbyte.Length)
                {
                    if (RXbyte[i] == 0x00) break;
                    RXDataBuffer[m_RXDataBufferPointer] = RXbyte[i];
                    if (m_RXDataBufferPointer == RXDataBuffer.Length)
                    {
                        Trace.WriteLine("### ERROR: RXDataBufferAppend(): RX Data Buffer Overloaded (limit 127 byte)");
                        break;
                    }
                    m_RXDataBufferPointer++;
                    i++;
                }
             }
            //===================================================================RXDataBufferReset
            public void RXDataBufferReset()
            {
                for (int i = 0; i < m_RXDataBuffer.Length; i++)
                {
                    m_RXDataBuffer[i] = 0x00;
                }
                m_RXDataBufferPointer = 0;
            }
            //===================================================================RXDataBufferGetString
            public string RXDataBufferGetString()
            {
                string hexstring = string.Empty;
                int i = 0;
                while(i < m_RXDataBuffer.Length)
                {
                    hexstring += Convert.ToChar(m_RXDataBuffer[i]);
                    i++;
                    if (m_RXDataBuffer[i] == 0x00) break;       //(null)
                    if (m_RXDataBuffer[i] == 0x0D) break;       // '/r'
                }
                return (hexstring);
            }



    }
}
