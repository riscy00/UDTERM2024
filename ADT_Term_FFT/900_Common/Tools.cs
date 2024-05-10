using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Globalization;
using System.Timers;
using System.Linq;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.InteropServices;
// Power Tool: insert QuickLaunch with "@tasks CollapseRegions" to close all region only.
// History
// 13/4/16:     Added statement to remove string prefix 0i, 0f, 0d on certain string to number conversion.
// 17/4/16:     Modified HexString_Remove_0x() accounting for 1x,2x,3x,4x hex number range.  
// 11/10/16:    Modified Conversion String to UINT32, INT32, DOUBLE, FLOAT to make use of try parse method than try/catch exception, more efficient operation. 

namespace UDT_Term_FFT
{
    interface ITools
    {
        //----------------------------------------------------------------------------File service
        string File_GetLine(string fileName, int myLineNumber);
        int File_NoOfLines(string fileName);                                            // Return number of line in txt file. 
        //----------------------------------------------------------------------------String Modify service
        string StringReplaceAt(string str, int index, int length, string replace);
        string TrimStartwithString(string source, string toTrim);                       // trim out start string with string (not char).
        string StrReplaceAtIndex(string text, int index, char c);
        //----------------------------------------------------------------------------String Misc
        int CountOccurrencesString(string s, string substring, bool aggressiveSearch);
        string[] SplitAndKeepDelimiters(string s, params char[] delimiters);            // mouse\ncat\n goes to 2 string mouse\n,cat\n
        string[] SplitAndKeepDelimitersSeperate(string s, params char[] delimiters);    // mouse\ncat\n goes to 4 string mouse,\n,cat,\n
        string[] SplitAndKeepDelimitersX(string s, char delimiters);           // mouse\ncat\n goes to 2 string mouse,\ncat\n  
        //----------------------------------------------------------------------------String Conversion Service
        UInt32 ConversionStringtoUInt32(string datas);
        Int32 ConversionStringtoInt32(string datas);
        UInt32 AnyStringtoUInt32(string sData);             // This remove 0x, 0i, 0u (not 0d). 
        Int32 AnyStringtoInt32(string sData);              // This remove 0x, 0i, 0u (not 0d). 
        //----------------------------------------------------------------------------Math Service
        double Math_DegreeToRadian(double angle);
        double Math_RadianToDegree(double angle);
        double MetersToFeet(double Meter);
        double FeetToMeters(double Feet);
        //----------------------------------------------------------------------------Binary Service
        int BinaryFalseTrue(bool data);
        //----------------------------------------------------------------------------Byte Service
        byte[] ByteAddByteToArray(byte[] bArray, byte newByte);     // Add byte to byte[].	
        string ByteArrayToStrASCII(byte[] characters);
        string ByteArrayToStrUTF8(byte[] characters);
        //string ByteArrayToStrDefault(byte[] characters);
        byte[] StrToByteArray(string str);
        byte[] StrToHexBytesNonASCII(string hex);                   // This will not remove 0x and 1x prefix. Array only.
        byte[] StrToHexBytesNonASCIILF(string hex);                 // This will not remove 0x and 1x prefix. Array only with LF. 
        byte StrToHexBytesNonASCIIOneByte(string hex);              // One byte, also remove 0x and 1x prefix. 
        byte StringTwoASCIItoByte(string ss);                       // two ASCII 12 => 0x12 Hex. 
        string ByteArrayToHexStr(byte[] bytes);
        string ByteArrayToHexStrSpace(byte[] bytes);
        string ByteArrayToHexStrSpaceLF(byte[] bytes);
        void UInt16toHexByte(uint uData16, out byte D0, out byte D1);
        void UInt32toHexByte(uint uData32, out byte D0, out byte D1, out byte D2, out byte D3);
        //----------------------------------------------------------------------------Contain service
        bool isStringContain0w(string text);
        bool isStringContain0q(string text);
        bool isStringContain0i(string text);
        bool isStringContain0l(string text);
        bool isStringContain0u(string text);
        bool isStringContain0s(string text);
        //----------------------------------------------------------------------------Double Service
        double ConversionStringtoDouble(string datas);
        string HexString_Remove_0d(string text);
        string HexString_Remove_0q0w(string text);  //for INT16 and uINT16
        bool isStringContain0d(string text);
        bool IsString_Double(string text);
        //----------------------------------------------------------------------------Float Service
        float ConversionStringtoFloat(string datas);
        float Conversion_MetricStringToFloat(string datas);
        string FormatFloatToMeterToString(float data);
        float[] CVSStringtoFloatArray(string sdata, int count, ref bool isfailed);
        //----------------------------------------------------------------------------Hex Service
        Int32 HexStringtoInt32X(string hex);
        UInt32 HexStringtoUInt32X(string hex);
        Byte HexStringtoByte(string hexString);             // This remove 0x
        UInt16 HexStringtoUInt16(string hexString);         // 0q This remove 0q for uINT16
        Int16 HexStringtoInt16(string hexString);            // 0w This remove 0w for uINT16
        UInt32 HexStringtoUInt32(string hexString);         // This remove 0x
        Int32 HexStringtoInt32(string hexString);            // This remove 0x, etc. 

        bool HexStringtoBool(string hex);
        string HexString_Remove_0x(string text);
        string HexStringtoStringBinary32(string hex);
        string HexStringtoStringBinary16(string hex);
        string HexStringtoStringBinary8(string hex);
        UInt16 HexBytetoUInterger16(byte D1, byte D0);
        uint HexBytetoUInterger32(byte D3, byte D2, byte D1, byte D0);
        bool IsString_Hex0x(string text);
        bool IsString_Hex(string text);
        bool isStringContain0x(string text);
        bool isStringContainxx(string text);
        char charToHexDigit(char c);                        // char to Hex
        bool IsHex(char c);                                 // Validate hex number char
        char IntToHexChar(int i);                           // Int to char conversion.
        //----------------------------------------------------------------------------Bit Processing (Added Nov 2015)
        UInt32 Bits_UInt32_Set(UInt32 Data32, int index);
        UInt16 Bits_UInt16_Set(UInt16 Data16, int index);
        UInt32 Bits_UInt32_Clear(UInt32 Data32, int index);
        UInt16 Bits_UInt16_Clear(UInt16 Data16, int index);
        bool Bits_UInt32_Read(UInt32 Data32, int index);
        bool Bits_UInt16_Read(UInt16 Data16, int index);
        //---------------------------------------------------------Read many bit
        int Bits_UInt32_intRead(UInt32 Data32, int index);
        int Bits_UInt32_intRead2Bit(UInt32 Data32, int index);
        int Bits_UInt32_intRead3Bit(UInt32 Data32, int index);
        int Bits_UInt32_intRead4Bit(UInt32 Data32, int index);
        //---------------------------------------------------------Write many bits Added 5/4/19 76OA. 
        UInt32 Bits_UInt32_uRead(UInt32 Data32, int index);
        UInt32 Bits_UInt32_uRead2Bit(UInt32 Data32, int index);
        UInt32 Bits_UInt32_uRead3Bit(UInt32 Data32, int index);
        UInt32 Bits_UInt32_uRead4Bit(UInt32 Data32, int index);
        UInt32 Bits_UInt32_uRead8Bit(UInt32 Data32, int index);
        UInt32 Bits_UInt32_uRead9Bit(UInt32 Data32, int index);
        UInt32 Bits_UInt32_uRead16Bit(UInt32 Data32, int index);
        //---------------------------------------------------------Write many bits Added 5/4/19 76OA. 
        UInt32 Bits_UInt32_intWrite(UInt32 Data32, int index, UInt32 data);
        UInt32 Bits_UInt32_intWrite2Bit(UInt32 Data32, int index, UInt32 data);
        UInt32 Bits_UInt32_intWrite3Bit(UInt32 Data32, int index, UInt32 data);
        UInt32 Bits_UInt32_intWrite4Bit(UInt32 Data32, int index, UInt32 data);
        UInt32 Bits_UInt32_intWrite8Bit(UInt32 Data32, int index, UInt32 data);
        UInt32 Bits_UInt32_intWrite9Bit(UInt32 Data32, int index, UInt32 data);
        UInt32 Bits_UInt32_intWrite16Bit(UInt32 Data32, int index, UInt32 data);

        //----------------------------------------------------------------------------Date Time and Unix Time Service
        string DateTimeNow(DateTime timenow);
        double DateTimeToUnixTimestamp(DateTime dateTime);
        UInt32 uDateTimeToUnixTimestampNowUTC();                // 10 digit UINT32 UTC (Not GMT, ignored time saving shift as in British time)
        UInt32 uDateTimeToUnixTimestampNowLocal();              // 10 digit UINT32 Local Time. 
        string sDateTimeToUnixTimestamp(DateTime dateTime);
        string sDateTimeToUnixTimestampLocal(DateTime dateTime);
        string sDateTimeToUnixTimestampHex(DateTime dateTime);   //Added 760B

        //--------------------------------------------------------------------------- CVS/CSV Service
        int[] CVSStringtoIntArray(string sdata, int count, ref bool isfailed);
        //--------------------------------------------------------------------------- Validate Type
        bool IsString_Numberic_Byte(string text);
        bool IsString_Numberic_Int32(string text);
        bool IsString_Numberic_UInt32(string text);
        bool IsString_Numberic_Bool(string text);
        bool IsString_Numberic_01(string text);
        bool IsString_Numberic_Double(string text);
        //------------------------------------------------------------------------ModBus
        UInt16 ModBus_RTU_CRC_calculate(byte[] buf, int len);
        string ModBus_ByteArrayToHexStrSpace(byte[] bytes);
        string ModBus_ByteArrayToHexStrSpaceLF(byte[] bytes);
        //--------------------------------------------------------------------------- Pause
        void InteractivePause(TimeSpan length);             // Replace Thread.Sleep(xx) without hanging up the window GUI or other thread. 
        //--------------------------------------------------------------------------- Misc
        bool StringContainsAny(string haystack, string[] needles);
        string XDAQ_FilterMessage(string data, bool mode);
        void RemoveWhiteSpace(ref byte[] bdata, ref int count);
        string RemoveWhiteSpaceString(byte[] bdata);
        UInt32 PrefixHexDecodeNoOfByte(string text);        // This decode 1x, 2x, 3x, 4x to 1,2,3,4    0x = 4. 
        //--------------------------------------------------------------------------- Misc
        string getOSInfo();
        string GetPathUserFolder();
        //--------------------------------------------------------------------------- Registry 
        string RegisteryReadLocalMachine(string Subkey, string GetValue);
        string[] RegisteryReadLocalMachine_GetSubKeyNames(string Subkey);
        //--------------------------------------------------------------------------- XML file.
        void SerializeToFile<T>(T item, string xmlFileName);
        T DeserializeFromFile<T>(string filenme) where T : class;
        //--------------------------------------------------------------------------- RichTextBox Speed up Hack
        // Recommended practice for all RTB window, it significantly speed up refresh. 
        // Take care not to muddle up the OldEventMask between function (Thread issue). 
        void rtb_StopRepaint(System.Windows.Forms.RichTextBox rtb, IntPtr OldEventMask);
        void rtb_StartRepaint(System.Windows.Forms.RichTextBox rtb, IntPtr OldEventMask);
        //--------------------------------------------------------------------------- RichTextBox, scroll to bottom Hack
        void ScrollToBottom(System.Windows.Forms.RichTextBox MyRichTextBox);
        //--------------------------------------------------------------------------- Ethernet tools
        bool Ethernet_ValidateIPv4(string ipString);
        int Ethernet_ValidatePortNumber(string sPort, int minRange, int maxRange);
    }

    public class Tools : ITools
    {
        public Tools()
        {

        }
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% RichTextBox Hack

        #region //-------------------------------------------------------------------ScrollToBottom (This work well!!)
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        private const int WM_VSCROLL = 277;
        private const int SB_PAGEBOTTOM = 7;

        public void ScrollToBottom(System.Windows.Forms.RichTextBox MyRichTextBox)
        {
            SendMessage(MyRichTextBox.Handle, WM_VSCROLL, (IntPtr)SB_PAGEBOTTOM, IntPtr.Zero);
        }
        #endregion

        #region //-------------------------------------------------------------------rtb speed up repaint.
        // Ref: https://weblogs.asp.net/jdanforth/88458
        // Ref: https://stackoverflow.com/questions/192413/how-do-you-prevent-a-richtextbox-from-refreshing-its-display
        // Worth looking into this: https://stackoverflow.com/questions/6547193/how-to-append-text-to-richtextbox-without-scrolling-and-losing-selection/47574181
        // And https://social.msdn.microsoft.com/Forums/sqlserver/en-US/9ae8374d-5593-4381-8054-158c649882a6/why-when-im-using-wmsetredraw-to-avoid-richtextbox1-flickering-when-updating-the-richtextbox1-text?forum=csharpgeneral

        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        private const int WM_USER = 0x0400;
        private const int EM_SETEVENTMASK = (WM_USER + 69);
        private const int EM_GETEVENTMASK = (WM_USER + 59);
        private const int WM_SETREDRAW = 0x0B;
        //private IntPtr OldEventMask;

        public void rtb_StopRepaint(System.Windows.Forms.RichTextBox rtb, IntPtr OldEventMask)
        {
            try
            {
                SendMessage(rtb.Handle, WM_SETREDRAW, 0, IntPtr.Zero);                      // Stop redrawing:   
                OldEventMask = SendMessage(rtb.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero);    // Stop sending of events:
            }
            catch {}
        }

        public void rtb_StartRepaint(System.Windows.Forms.RichTextBox rtb, IntPtr OldEventMask)
        {
            try
            {
                SendMessage(rtb.Handle, EM_SETEVENTMASK, 0, OldEventMask);                  // turn on event     
                SendMessage(rtb.Handle, WM_SETREDRAW, 1, IntPtr.Zero);                      // turn on redrawing
                rtb.Invalidate();                                                           // this forces a repaint, which for some reason is necessary in some cases.
            }
            catch { }
        }
        #endregion

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% User Folder Path

        #region //-------------------------------------------------------------------GetPathUserFolder
        //https://stackoverflow.com/questions/1140383/how-can-i-get-the-current-user-directory
        public string GetPathUserFolder()
        {
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }
            return path;
        }
        #endregion

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% XML

        #region //-------------------------------------------------------------------SerializeToFile
        // Source from: http://stackoverflow.com/questions/28499781/deserializing-xml-into-c-sharp-so-that-the-values-can-be-edited-and-re-serialize
        // An 'System.IO.FileNotFoundException' exception is thrown but handled by the XmlSerializer, so if you just ignore it everything should continue on fine.
        // Method-1 This is better due to \r\n for each XML statement but 
        public void SerializeToFile<T>(T item, string xmlFileName)
        {
            if (typeof(T).IsSerializable)       // Check before applying, in case class has missing [Serializable]
            {
                using (FileStream stream = new FileStream(xmlFileName, FileMode.Create, FileAccess.Write))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stream, item);
                }
            }
        }
        #endregion

        #region //-------------------------------------------------------------------DeserializeFromFile<>
        // This is better solution since there is no IO filename exception error. 
        public T DeserializeFromFile<T>(string filenme) where T : class
        {
            T result = null;
            if (typeof(T).IsSerializable)            // Check before applying, in case class has missing [Serializable]
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (XmlTextReader reader = new XmlTextReader(filenme))
                {
                    try
                    {
                        result = (T)serializer.Deserialize(reader);
                        //Console.WriteLine("-INFO: Deserialization successful! Got string: \n{0}", result);
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("###ERR: Failed to Deserialize object!!");
                    }
                }
            }
            return result;
            //return default(T); or this method, not sure which one best....! (see expert exchange)
        }
        #endregion

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% MODBUS

        #region //====================================ModBus_RTU_CRC
        //==========================================================
        // Purpose  : Compute the MODBUS RTU CRC
        // Input    : Buf[] to be calculated, len of the buf[] for CRC calculation.  
        // Output   : CRC in 16 bit or two byte. Check for swap as needed.  
        // Note     : https://ctlsys.com/support/how_to_compute_the_modbus_rtu_message_crc/
        //==========================================================
        public UInt16 ModBus_RTU_CRC_calculate(byte[] buf, int len)
        {
            UInt16 crc = 0xFFFF;
            for (int pos = 0; pos < len; pos++)
            {
                crc ^= (UInt16)buf[pos];            // XOR byte into least sig. byte of crc
                for (int i = 8; i != 0; i--)        // Loop over each bit
                {
                    if ((crc & 0x0001) != 0)        // If the LSB is set
                    {
                        crc >>= 1;                  // Shift right and XOR 0xA001
                        crc ^= 0xA001;
                    }
                    else                            // Else LSB is not set
                        crc >>= 1;                  // Just shift right
                }
            }
            return crc;                             // Note, this number has low and high bytes swapped, so use it accordingly (or swap bytes)
        }
        #endregion

        #region //====================================ModBus_ByteArrayToHexStrSpace
        // Bytes[1] = Function
        // Bytes[2][3] = Depend on below
        // Function 3    Byte[2]
        // Function 4    Byte[2]
        // Function 6    Fixed 5 byte
        // Function 18   Byte[2][3] = Byte Counts inc FIFO. Byte[3][4] = FIfo Count, reset are data
        // Function 2B   Treat as 128 Byte fixed.
        public string ModBus_ByteArrayToHexStrSpace(byte[] bytes)
        {
            string hexstring = string.Empty;
            int noofByte = 0;
            switch (bytes[1])
            {
                case (0x3):
                    {
                        noofByte = bytes[2]+3;
                        break;
                    }
                case (0x4):
                    {
                        noofByte = bytes[2]+3;
                        break;
                    }
                case (0x6):
                    {
                        noofByte = 6;
                        break;
                    }
                case (0x18):
                    {
                        noofByte = System.BitConverter.ToUInt16(bytes, 2)+2;
                        break;
                    }
                case (0x2B):
                    {
                        noofByte = bytes.Length;
                        if (noofByte > 64)
                            noofByte = 64;
                        break;
                    }
                default:
                    {
                        noofByte = bytes.Length;
                        break;
                    }
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < noofByte; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
                sb.Append(' ');
                //hexstring += bytes[i].ToString("X2");
                //hexstring += " ";
            }
            hexstring = sb.ToString();
            return (hexstring);
        }
        #endregion

        #region //====================================ModBus_ByteArrayToHexStrSpaceLF
        public string ModBus_ByteArrayToHexStrSpaceLF(byte[] bytes)
        {
            string hexstring = string.Empty;
            int noofByte = 0;
            switch (bytes[1])
            {
                case (0x3):
                    {
                        noofByte = bytes[2] + 3;
                        break;
                    }
                case (0x4):
                    {
                        noofByte = bytes[2] + 3;
                        break;
                    }
                case (0x6):
                    {
                        noofByte = 5;
                        break;
                    }
                case (0x18):
                    {
                        noofByte = System.BitConverter.ToUInt16(bytes, 2) + 2;
                        break;
                    }
                case (0x2B):
                    {
                        noofByte = bytes.Length;
                        if (noofByte > 64)
                            noofByte = 64;
                        break;
                    }
                default:
                    {
                        noofByte = bytes.Length;
                        break;
                    }
            }
            for (int i = 0; i < noofByte; i++)
            {
                hexstring += bytes[i].ToString("X2");
                if (bytes[i] == '\n')
                {
                    hexstring += "~\n";         // no need for '\r'
                    break;
                }
                hexstring += " ";
            }
            return (hexstring);
        }
        #endregion

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Misc/Recent

        #region //====================================StringContainsAny
        //string[] seek = { "$D", "$H", "$G", "$F" };
        //    if (Tools.StringContainsAny(sRecievedMessage, seek) ==false)
        public bool StringContainsAny(string haystack, string[] needles)
        {
            foreach (string needle in needles)
            {
                if (haystack.Contains(needle))
                    return true;
            }
            return false;
        }
        #endregion

        #region //====================================StringReplaceAt (double reply)
        // str - the source string
        // index- the start location to replace at (0-based)
        // length - the number of characters to be removed before inserting
        // replace - the string that is replacing characters
        public string StringReplaceAt(string str, int index, int length, string replace)
        {
            return str.Remove(index, Math.Min(length, str.Length - index))
                    .Insert(index, replace);
        }
        #endregion

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% DateTime Section

        #region //====================================DateTimeToUnixTimestamp in UTC (double reply)
        //http://stackoverflow.com/questions/249760/how-to-convert-a-unix-timestamp-to-datetime-and-vice-versa 
        public double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            TimeSpan span = (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            double unixTime = span.TotalSeconds;
            return unixTime;

            //return (TimeZoneInfo.ConvertTimeToUtc(dateTime,TimeZoneInfo.Local) -
            //       new DateTime(1970, 1, 1, 0, 0, 0, 0,System.DateTimeKind.Utc)).TotalSeconds;
        }
        #endregion

        #region //====================================uDateTimeToUnixTimestampNowUTC in UTC (UINT32 reply)
        public UInt32 uDateTimeToUnixTimestampNowUTC()
        {
            DateTime dtSTCStart = DateTime.Now;
            //long unixTime = ((DateTimeOffset)dtSTCStart).ToUnixTimeSeconds();     .NET4.6 but not local time.
            //UInt32 UDTime = Convert.ToUInt32(unixTime);

            DateTime t0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);  // Old Style Method...Local time, all are double value not UINT32. 
            TimeSpan span = dtSTCStart - t0;
            UInt32 UDTimex = Convert.ToUInt32(span.TotalSeconds);                   // 10 digit style which what we want. 
            return (UDTimex);
        }
        #endregion

        #region //====================================uDateTimeToUnixTimestampNowLocal in Local (UINT32 reply)
        public UInt32 uDateTimeToUnixTimestampNowLocal()
        {
            DateTime dtSTCStart = DateTime.Now;
            //long unixTime = ((DateTimeOffset)dtSTCStart).ToUnixTimeSeconds();     .NET4.6 but not local time.
            //UInt32 UDTime = Convert.ToUInt32(unixTime);

            DateTime t0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);  // Old Style Method...Local time, all are double value not UINT32. 
            TimeSpan span = dtSTCStart - t0;
            UInt32 UDTimex = Convert.ToUInt32(span.TotalSeconds);                   // 10 digit style which what we want. 
            return (UDTimex);
        }
        #endregion

        #region //====================================sDateTimeToUnixTimestamp in UTC (string reply)
        //http://stackoverflow.com/questions/249760/how-to-convert-a-unix-timestamp-to-datetime-and-vice-versa 
        public string sDateTimeToUnixTimestamp(DateTime dateTime)
        {
            TimeSpan span = (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));    
            double unixTime = span.TotalSeconds;
            return unixTime.ToString();

            //return (TimeZoneInfo.ConvertTimeToUtc(dateTime,TimeZoneInfo.Local) -
            //       new DateTime(1970, 1, 1, 0, 0, 0, 0,System.DateTimeKind.Utc)).TotalSeconds;
        }
        #endregion

        #region //====================================sDateTimeToUnixTimestampHex in UTC (string reply)
        public string sDateTimeToUnixTimestampHex(DateTime dateTime)
        {
            TimeSpan span = (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            int unixTime = Convert.ToInt32(span.TotalSeconds);
            return ("0X"+ unixTime.ToString("X"));
        }
        #endregion

        #region //====================================sDateTimeToUnixTimestampLocal in Local (string reply)
        //http://stackoverflow.com/questions/249760/how-to-convert-a-unix-timestamp-to-datetime-and-vice-versa 
        public string sDateTimeToUnixTimestampLocal(DateTime dateTime)
        {
            TimeSpan span = (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local));
            double unixTime = span.TotalSeconds;
            return unixTime.ToString();

            //return (TimeZoneInfo.ConvertTimeToUtc(dateTime,TimeZoneInfo.Local) -
            //       new DateTime(1970, 1, 1, 0, 0, 0, 0,System.DateTimeKind.Utc)).TotalSeconds;
        }
        #endregion

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        #region //====================================RadianToDegree
        public double Math_RadianToDegree(double angle) => angle * (180.0 / Math.PI);
        #endregion

        #region //====================================DegreeToRadian
        public double Math_DegreeToRadian(double angle) => Math.PI * angle / 180.0;

        #endregion

        #region //====================================PrefixDecodeNoOfByte
        public UInt32 PrefixHexDecodeNoOfByte(string text)
        {
            if (text.Contains("1x"))
                return 1;
            if (text.Contains("2x"))
                return 2;
            if (text.Contains("3x"))
                return 3;
            if (text.Contains("4x"))
                return 4;
            return 4;     //All else are 4(ie 0x)
        }
        #endregion

        #region //====================================MetersToFeet
        public double MetersToFeet(double Meters) => Meters / 0.304800610;

        #endregion
    
        #region //====================================FeetToMeters
        public double FeetToMeters(double Feet) => Feet * 0.304800610;

        #endregion

        #region //====================================StringTwoASCIItoByte
        public byte StringTwoASCIItoByte (string ss) 
		{
            //             byte b;      // Alternative method. 
            //             if (byte.TryParse(ss, NumberStyles.HexNumber, null, out b))
            //             {
            //                 return (b);
            //             }

            char[] c = new char[2];
			c = ss.ToCharArray(0,2);
			//char x1 = charToHexDigit(c[0]);
			//char x2 = charToHexDigit(c[1]);
			//byte res = (byte)((x1 << 4) + x2);
			//return res;
			return (byte) (charToHexDigit(c[0])*16 + charToHexDigit(c[1]));
		}
        #endregion

        #region //====================================charToHexDigit
        public char charToHexDigit(char c)
		{
			if (c >= '0' && c <= '9') 
				return (char)(c - '0'); 
			if (c >= 'A' && c <= 'F') 
				return (char) (c - 'A' + 10); 
			return (char) 0; 
		}
        #endregion

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        #region //====================================ConversionStringtoDouble
        public double ConversionStringtoDouble(string datas)
		{
            datas = datas.Replace("0f", "");        // Added 13/4/16
            datas = datas.Replace("0d", "");        // Added 13/4/16
            datas = datas.Replace("0s", "");        // Added 08/10/17 under MiniAD7794 Project. 
            double data;
            if (Double.TryParse(datas, out data) == false)
            {
                Debug.WriteLine("#ERR: String to Double Failed");
                return Double.NaN;
            }
            return data;
            /*
            try
			{
				data = System.Convert.ToDouble(datas);
			}
			catch (System.OverflowException)
			{
				Debug.WriteLine("#ERR: String to Double > Overflow error");
				return Double.NaN;
			}
			catch (System.FormatException)
			{
				Debug.WriteLine("#ERR:String to Double > not Double format");
				return Double.NaN;
			}
			return data;
            */
		}

		#endregion

		#region //====================================ConversionStringtoUInt32
		public UInt32 ConversionStringtoUInt32(string datas)
		{
            datas = datas.Replace("0i", "");        // Added 13/4/16
            datas = datas.Replace("0u", "");        // Added 11/4/18
            datas = datas.Replace("0l", "");        // Added 21/03/20
            datas = datas.Replace("0L", "");        // Added 21/03/20
            UInt32 datai;

            if (UInt32.TryParse(datas, out datai) == false)
            {
                Debug.WriteLine("#ERR: String to UInt32 Failed");
                return (UInt32)0xFFFFFFFF;
            }
            return datai;
            /*
            try
			{
				datai = System.Convert.ToUInt32(datas);
			}
			catch (System.OverflowException)
			{
				Debug.WriteLine("#ERR: String to UInt32 > Overflow error");
				return (UInt32)0xFFFFFFFF;
			}
			catch (System.FormatException)
			{
				Debug.WriteLine("#ERR:String to UInt32 > not UInt32 format");
				return (UInt32)0xFFFFFFFF;
			}
			catch (System.ArgumentNullException)
			{
				Debug.WriteLine("#ERR:String to UInt32 > null format");
				return (UInt32)0xFFFFFFFF;
			}
			return datai;
            */
		}

		#endregion

		#region //====================================ConversionStringtoInt32
		public Int32 ConversionStringtoInt32(string datas)
		{
            datas = datas.Replace("0i", "");        // Added 13/4/16
            Int32 datai;

            if (Int32.TryParse(datas, out datai) ==false)
            {
                Debug.WriteLine("#ERR: String to Int32 Failed");
                return -975579;
            }
            return datai;


            /*
            try 
			{
				datai =System.Convert.ToInt32(datas);
			} 
			catch (System.OverflowException)
			{
				Debug.WriteLine("#ERR: String to Int32 > Overflow error");
				return -975579;
			}
			catch (System.FormatException) 
			{
				Debug.WriteLine("#ERR:String to Int32 > not Int format");
				return -975579;
			}
			catch (System.ArgumentNullException) 
			{
				Debug.WriteLine("#ERR:String to Int32 > null format");
				return -975579;
			}
			return datai;
            */
		}

        #endregion

        #region //====================================ConversionStringtoFloat
        public float ConversionStringtoFloat(string datas)
		{
            datas = datas.Replace("0f", "");        // Added 13/4/16
            datas = datas.Replace("0d", "");        // Added 13/4/16
            float dataf;
            if (float.TryParse(datas, out dataf) == false)
            {
                Debug.WriteLine("#ERR: String to Double Failed");
                return -975.579f;
            }
            return dataf;
            /*
            try 
			{
				dataf = System.Convert.ToSingle(datas);
			} 
			catch (System.OverflowException)
			{
				Debug.WriteLine("#ERR:String to Float > Overflow error");
				return -975.579f;
			}
			catch (System.FormatException) 
			{
				Debug.WriteLine("#ERR:String to Float > not float format");
				return -975.579f;
			}
			catch (System.ArgumentNullException) 
			{
				Debug.WriteLine("#ERR:String to Float > null format");
				return -975.579f;
			}
			return dataf;
            */
		}

		#endregion

		#region //====================================Conversion_MetricStringToFloat
		public float Conversion_MetricStringToFloat(string datas)
		{
            datas = datas.Replace("0f", "");        // Added 13/4/16
            datas = datas.Replace("0d", "");        // Added 13/4/16
            float dataf;
			float scale=1;
			StringBuilder dataSB = new StringBuilder(datas);
			if (Regex.IsMatch(datas,"m")==true) // mV scale.
			{
				dataSB.Replace("m","");
				scale=1e-3f;
			}
			if (Regex.IsMatch(datas,"u")==true) // uV scale.
			{
				dataSB.Replace("u","");
				scale=1e-6f;
			}
			if (Regex.IsMatch(datas,"n")==true) // nV scale.
			{
				dataSB.Replace("n","");
				scale=1e-9f;
			}
			if (Regex.IsMatch(datas,"p")==true) // pV scale.
			{
				dataSB.Replace("p","");
				scale=1e-12f;
			}
			if (Regex.IsMatch(datas,"K")==true) // KV scale.
			{
				dataSB.Replace("K","");
				scale=1e3f;
			}
			if (Regex.IsMatch(datas,"M")==true) // MV scale.
			{
				dataSB.Replace("M","");
				scale=1e6f;
			}
			if (Regex.IsMatch(datas,"G")==true) // GV scale.
			{
				dataSB.Replace("G","");
				scale=1e9f;
			}
			try
			{
				dataf = float.Parse(dataSB.ToString(),NumberStyles.Number);
				dataf = dataf*scale; 
				return dataf;
			}
			catch( Exception ex)
			{
				Debug.WriteLine("#ERR:Float to String Failure :-" + ex.Message);
				return 9566599;
			}
		}

        #endregion

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        #region //====================================BinaryFalseTrue
        public int BinaryFalseTrue(bool data)
        {
            if (data == true)
                return (1);
            return (0);
        }
        #endregion

        #region //====================================ByteArrayToStrASCII
        //---------------------------------------------------------------------------------Byte[] to string
        //--------http://www.ondotnet.com/pub/a/dotnet/excerpt/csharpckbk_chap01/
        //--------------------------------------------------------------------------------------------------
        public string ByteArrayToStrASCII(byte[] characters)
		{
			ASCIIEncoding encoding = new ASCIIEncoding( );
			string constructedString = encoding.GetString(characters);

			return (constructedString);
		}
        #endregion

        #region //====================================ByteArrayToStrUTF8
        //---------------------------------------------------------------------------------Byte[] to string
        //--------http://www.ondotnet.com/pub/a/dotnet/excerpt/csharpckbk_chap01/
        //--------------------------------------------------------------------------------------------------
        public string ByteArrayToStrUTF8(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);

            return (constructedString);
        }
        #endregion

        //#region //====================================ByteArrayToStrDefault
//         //---------------------------------------------------------------------------------Byte[] to string
//         //--------http://www.ondotnet.com/pub/a/dotnet/excerpt/csharpckbk_chap01/
//         //--------------------------------------------------------------------------------------------------
//         public string ByteArrayToStrDefault(byte[] characters)
//         {
//             UTF8Encoding encoding = new UTF8Encoding();
//             string constructedString = encoding.GetString(characters);
// 
//             return (constructedString);
//         }
//         #endregion
        
        #region //====================================ByteArrayToHexStr
        //---------------------------------------------------------------------------------Byte[] to hex string
        //--------msdn solution "timster at home" / "sandeeprwt" 
        //--------------------------------------------------------------------------------------------------
        public string ByteArrayToHexStr(byte[] bytes)
		{
			string hexstring = string.Empty;
			for (int i=0; i<bytes.Length;i++)
			{
				hexstring += bytes[i].ToString("X2");
			}
			return (hexstring);
		}
		#endregion

	    //---------------------------------------------------------------------------------Byte[] to hex string with space between byte
		//--------msdn solution "timster at home" / "sandeeprwt" 
		//--------------------------------------------------------------------------------------------------

		#region //====================================ByteArrayToHexStrSpace
		public string ByteArrayToHexStrSpace(byte[] bytes)
		{
			string hexstring = string.Empty;
			for (int i=0; i<bytes.Length;i++)
			{
				hexstring += bytes[i].ToString("X2");
				hexstring +=" ";
			}
			return (hexstring);
		}
		#endregion

		//---------------------------------------------------------------------------------Byte[] to hex string with space between byte and LF if detected 0x0A
		//--------msdn solution "timster at home" / "sandeeprwt" 
		//--------------------------------------------------------------------------------------------------

		#region //====================================ByteArrayToHexStrSpaceLF
		public string ByteArrayToHexStrSpaceLF(byte[] bytes)
		{
			string hexstring = string.Empty;
			for (int i = 0; i < bytes.Length; i++)
			{
				hexstring += bytes[i].ToString("X2");
				if (bytes[i]=='\n')
				{
					hexstring += "~\n";         // no need for '\r'
					break;
				}
				hexstring += " ";
			}
			return (hexstring);
		}
		#endregion

		#region //====================================StrToHexBytesNonASCII
		// Warning: take care it does not like \r\n, it will crash this code
		public byte[] StrToHexBytesNonASCII(string hex)        // Non-ASCII
		{
			int l = hex.Length / 2;
			var b = new byte[l];
			for (int i = 0; i < l; ++i)
			{
				b[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
			}
			return b;
		}
		#endregion      // Use for Short Hop RX project. 

		#region //====================================StrToHexBytesNonASCIILF
		// Warning: take care it does not like \r\n, it will crash this code
		public byte[] StrToHexBytesNonASCIILF(string hex)        // Non-ASCII
		{
			int l = (hex.Length / 2)+2;
			var b = new byte[l];
			int i;
			for (i = 0; i < l-2; ++i)
			{
				b[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
			}
			b[i] = 0x0D;
			b[i + 1] = 0x0A;
			return b;
		}
        #endregion      // Use for Short Hop RX project.

        #region //====================================StrToHexBytesNonASCIIOneByte
        // Warning: take care it does not like \r\n, it will crash this code
        public byte StrToHexBytesNonASCIIOneByte(string hex)        // Non-ASCII, also remove 0x and 1x. 
        {
            hex = hex.Replace("0x", "");
            hex = hex.Replace("1x", "");
            byte bb = Convert.ToByte(hex.Substring(0, hex.Length), 16);
            return (bb);
        }
        #endregion      // Use for Short Hop RX project.

        #region //====================================addByteToArray
        public byte[] ByteAddByteToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 0);
            newArray[bArray.Length] = newByte;  // Add to last byte
            return newArray;
        }
        #endregion

		#region //====================================StrToByteArray
		//---------------------------------------------------------------------------------string to byte[]
		//--------http://www.chilkatsoft.com/faq/DotNetStrToBytes.html
		//--------------------------------------------------------------------------------------------------
		/*
		public byte[] StrToByteArray(string str)
		{
			System.Text.ASCIIEncoding  encoding=new System.Text.ASCIIEncoding();
			return encoding.GetBytes(str);
		}
		 */
		public byte[] StrToByteArray(string str)
		{
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
			return encoding.GetBytes(str);
		}
		#endregion

		#region //====================================FormatFloatToMeterToString
		public string FormatFloatToMeterToString(float data)		// converts 0.004 to 4mV formats
		{
			int loop=0;
			bool negNumber=false;
			string result;
			if (data<=0) 
			{
				data=-(data);			// Convert negative to positive
				negNumber=true;
			}
			if (data==0) return "0";	// zero number.
			if (data<1)					// for number less than 1
			{							// 03-05 = m, 06-08 = u, 09-11 = n, 12-15 = p
				loop=0;
				while (loop<=15)		// 03-05 = K, 06-08 = M, 09-11 = G
				{
					if (data>=1) break;		//Math.Round(data) will not work.
					data=data*10;
					loop++;
				}
				#region decode into string for less than 1
				switch (loop)
				{
					case (1):
						data*=100;							// 0.1234 => 1.234 => 123.4
						result=data.ToString("F1")+"m";		// 123.4m
						break;
					case (2):
						data*=10;							// 0.01234 => 1.234 => 12.34
						result=data.ToString("F2")+"m";		// 12.34m
						break;
					case (3):
						data*=1;							// 0.001234 => 1.234 => 1.234
						result=data.ToString("F3")+"m";		// 1.234m
						break;	
					case (4):
						data*=100;							// 1.234e4 => 1.234 => 123.4
						result=data.ToString("F1")+"u";		// 123.4u
						break;
					case (5):
						data*=10;							// 0.01234 => 1.234 => 12.34
						result=data.ToString("F2")+"u";		// 12.34u
						break;
					case (6):
						data*=1;							// 0.001234 => 1.234 => 1.234
						result=data.ToString("F3")+"u";		// 1.234u
						break;	
					case (7):
						data*=100;							// 1.234e4 => 1.234 => 123.4
						result=data.ToString("F1")+"n";		// 123.4n
						break;
					case (8):
						data*=10;							// 0.01234 => 1.234 => 12.34
						result=data.ToString("F2")+"n";		// 12.34n
						break;
					case (9):
						data*=1;							// 0.001234 => 1.234 => 1.234
						result=data.ToString("F3")+"n";		// 1.234n
						break;
					case (10):
						data*=100;							// 1.234e4 => 1.234 => 123.4
						result=data.ToString("F1")+"p";		// 123.4p
						break;
					case (12):
						data*=10;							// 0.01234 => 1.234 => 12.34
						result=data.ToString("F2")+"p";		// 12.34p
						break;
					case (13):
						data*=1;							// 0.001234 => 1.234 => 1.234
						result=data.ToString("F3")+"p";		// 1.234p
						break;	
					default:
						result="";						// out of range
						break;
				}
				#endregion
			}
			else
			{
				loop=0;
				while (loop<=12)
				{
					if (Math.Round(data)<10) break;
					data=data/10;
					loop++;
				}
				#region decode into string for more than 1
				switch (loop)
				{
					case (0):
						data*=1;							// 1.234 => 1.234 => 1.234
						result=data.ToString("F3");			// 1.234
						break;
					case (1):
						data*=10;							// 12.34 => 1.234 => 12.34
						result=data.ToString("F2");			// 12.34
						break;
					case (2):
						data*=100;							// 123.4 => 1.234 => 123.4
						result=data.ToString("F1");			// 123.4
						break;	
					case (3):
						data*=1;							// 1234 => 1.234 => 1.234
						result=data.ToString("F3")+"K";		// 1.234K
						break;
					case (4):
						data*=10;							// 12.34 => 1.234 => 12.34
						result=data.ToString("F2")+"K";		// 12.34K
						break;
					case (5):
						data*=100;							// 123.4 => 1.234 => 123.4
						result=data.ToString("F1")+"K";		// 123.4K
						break;	
					case (6):
						data*=1;							// 1234 => 1.234 => 1.234
						result=data.ToString("F3")+"M";		// 1.234M
						break;
					case (7):
						data*=10;							// 12.34 => 1.234 => 12.34
						result=data.ToString("F2")+"M";		// 12.34M
						break;
					case (8):
						data*=100;							// 123.4 => 1.234 => 123.4
						result=data.ToString("F1")+"M";		// 123.4M
						break;	
					case (9):
						data*=1;							// 1234 => 1.234 => 1.234
						result=data.ToString("F3")+"G";		// 1.234M
						break;
					case (10):
						data*=10;							// 12.34 => 1.234 => 12.34
						result=data.ToString("F2")+"G";		// 12.34M
						break;
					case (11):
						data*=100;							// 123.4 => 1.234 => 123.4
						result=data.ToString("F1")+"G";		// 123.4M
						break;					
					default:
						result="";
						break;
				}
				#endregion
			}
			if (negNumber==true) return "-"+result;	// negative number
			return result;							// positive number
		}
		#endregion
		
        #region //====================================DataTime to String
		public string DateTimeNow(DateTime timenow)		// formatted for timestamp the data file (for RCV project)
		{
			string timestamp=timenow.Year.ToString()+"-"+timenow.Month.ToString()+"-"+timenow.Day.ToString()+"__"
				+timenow.Hour.ToString()+"-"+timenow.Minute.ToString();
			return timestamp;
		}
		#endregion
		#region //====================================Filter response message from DAQ. mode==true remove \n
		public string XDAQ_FilterMessage(string data,bool mode)		// formatted for timestamp the data file (for RCV project)
		{
			int pointer;
			while (data.Length>0)							// in case of blank response
			{
				pointer=data.IndexOf("X>");				//delete 'X>' if any
				if (pointer == -1) break;
				data=data.Remove(pointer,2);
			}
			while (data.Length>0)							// in case of blank response
			{
				pointer=data.IndexOf('\r');				
				if (pointer == -1) break;
				data=data.Remove(pointer,1);
			}
			if (mode==true)
			{
				while (data.Length>0)							// in case of blank response
				{
					pointer=data.IndexOf('\n');				
					if (pointer == -1) break;
					data=data.Remove(pointer,1);
				}
			}
			return data;
		}
		#endregion
		#region //====================================RemoveWhiteSpace
		public void RemoveWhiteSpace(ref byte[] bdata,ref int count)	// if count=0, no return data, if count=1 return 1 byte data, etc.
		{
			count=0;
			int i=0;
			while (i<=bdata.Length)				// remove whitespace.
			{
				if (bdata[i]==0) 
				{
					bdata[count]=0;				// include null
					count++;
					break;							// end of string detected.
				}
				if (bdata[i]!=' ')					// if space, skip copy
				{
					bdata[count]=bdata[i];
					count++;						// increment pointer (non-space)
				}
				i++;
			}
		}
		#endregion
		#region //====================================RemoveWhiteSpaceString
		public string RemoveWhiteSpaceString(byte[] bdata)	// if count=0, no return data, if count=1 return 1 byte data, etc.
		{
			int count=0;
			int i=0;
			while (i<=bdata.Length)				// remove whitespace.
			{
				if (bdata[i]==0) 
				{
					bdata[count]=0;				// include null
					count++;
					break;							// end of string detected.
				}
				if (bdata[i]!=' ')					// if space, skip copy
				{
					bdata[count]=bdata[i];
					count++;						// increment pointer (non-space)
				}
				i++;
			}
			return Encoding.ASCII.GetString(bdata,0,count);
		}
        #endregion

        #region //====================================AnyStringtoInt32      // Handle 0x,1x,2x,3x,4x,0i,0u prefix type. 
        public Int32 AnyStringtoInt32(string sData)
        {
            Int32 idata;
            try
            {
                sData = sData.ToLower();
                if (sData.Contains("0i"))
                {
                    sData = sData.Replace("0i", "");
                    idata = Convert.ToInt32(sData, 10);
                }
                else if (sData.Contains("0u"))                  // Unsigned type for positive number range. 
                {
                    sData = sData.Replace("0u", "");
                    idata = Convert.ToInt32(sData, 10);
                }
                else if (sData.Contains("1x"))
                {
                    sData = sData.Replace("1x", "0x");
                    idata = Convert.ToInt32(sData, 16);
                }
                else if (sData.Contains("2x"))
                {
                    sData = sData.Replace("2x", "0x");
                    idata = Convert.ToInt32(sData, 16);
                }
                else if (sData.Contains("3x"))
                {
                    sData = sData.Replace("3x", "0x");
                    idata = Convert.ToInt32(sData, 16);
                }
                else if (sData.Contains("4x"))
                {
                    sData = sData.Replace("4x", "0x");
                    idata = Convert.ToInt32(sData, 16);
                }
                else if (sData.Contains("0x"))
                {
                    idata = Convert.ToInt32(sData, 16);
                }
                else
                {
                    idata = Convert.ToInt32(sData, 10);
                }
            }
            catch
            {
                Debug.WriteLine("#ERR: AnyStringtoInt32() Error....(Note:- 0x,1x,2x,3x,4x,0i allowed) ");
                idata = -975579;
            }
            return idata;
        }
        #endregion
        #region //====================================AnyStringtoUInt32     // Handle 0x,1x,2x,3x,4x,0i,0u prefix type.
        public UInt32 AnyStringtoUInt32(string sData)
        {
            UInt32 udata;
            try
            {
                sData = sData.ToLower();
                if (sData.Contains("0i"))                       // signed type as long it positive. 
                {
                    sData = sData.Replace("0i", "");
                    if (Convert.ToInt32(sData) < 0)               // Negative number not allowed. 
                    {
                        Debug.WriteLine("#ERR: AnyStringtoUInt32() Error....Negative number not allowed, return 0\n");
                        udata = 0;
                        return (udata);
                    }
                    udata = Convert.ToUInt32(sData, 10);
                }
                else if (sData.Contains("0u"))                  // Unsigned type
                {
                    sData = sData.Replace("0u", "");
                    udata = Convert.ToUInt32(sData, 10);
                }
                else if (sData.Contains("1x"))
                {
                    sData = sData.Replace("1x", "0x");
                    udata = Convert.ToUInt32(sData, 16);
                }
                else if (sData.Contains("2x"))
                {
                    sData = sData.Replace("2x", "0x");
                    udata = Convert.ToUInt32(sData, 16);
                }
                else if (sData.Contains("3x"))
                {
                    sData = sData.Replace("3x", "0x");
                    udata = Convert.ToUInt32(sData, 16);
                }
                else if (sData.Contains("4x"))
                {
                    sData = sData.Replace("4x", "0x");
                    udata = Convert.ToUInt32(sData, 16);
                }
                else if (sData.Contains("0x"))
                {
                    udata = Convert.ToUInt32(sData, 16);
                }
                else
                {
                    udata = Convert.ToUInt32(sData, 16);
                }
            }
            catch
            {
                Debug.WriteLine("#ERR: AnyStringtoUInt32() Error....(Note:- 0x,1x,2x,3x,4x,0i allowed) \n Message:");
                udata = 975579;
            }
            return udata;
        }
        #endregion  

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        #region //====================================HexStringtoInt32X
        public int HexStringtoInt32X(string hex)
		{
			Int32 idata;
			try
			{
                hex = hex.ToLower();
                hex = hex.Replace("1x", "0x");
                hex = hex.Replace("2x", "0x");
                hex = hex.Replace("3x", "0x");
                hex = hex.Replace("4x", "0x");
                idata =Convert.ToInt32(hex,16);
			}
			catch 
			{
				Debug.WriteLine("#ERR: Hex String to Int > not Hex format (Note:- 0X allowed) \n\n Message:");
				idata=-975579;
			}
		return idata;
		}
        #endregion
        #region //====================================HexStringtoUInt32X
        public UInt32 HexStringtoUInt32X(string hex)
        {
            UInt32 idata;
            try
            {
                hex = hex.ToLower();
                hex = hex.Replace("1x", "0x");
                hex = hex.Replace("2x", "0x");
                hex = hex.Replace("3x", "0x");
                hex = hex.Replace("4x", "0x");
                idata = Convert.ToUInt32(hex, 16);
            }
            catch 
            {
                Debug.WriteLine("#ERR: Hex String to UInt32X > not Hex format (Note:- 0x,1x,2x,3x,4x allowed) \n");
                idata = 0xFFFFFFFF;
            }
            return idata;
        }
        #endregion
        #region //====================================HexStringtoBool
        public bool HexStringtoBool(string hex)
        {
            Boolean myBool;
            hex = hex.ToLower();
            hex = hex.Replace("0x", "");
            hex = hex.Replace("0u", "");
            hex = hex.Replace("0b", "");
            try
            {
                myBool = FormatHelper.StringToBoolean(hex);
            }
            catch
            {
                Debug.WriteLine("#ERR: Hex String to UInt32 > not Hex format (Note:- 0X allowed) \n");
                myBool = false;
            }
            return (myBool);
        }
        #endregion
        #region //====================================HexStringtoStringBinary32
        public string  HexStringtoStringBinary32(string hex)  
		{
		string sbinary = ""; ;
			try
			{
				//sbinary = Convert.ToString(Convert.ToInt32(hex, 16), 2);
				sbinary = Int32.Parse(Convert.ToString(Convert.ToInt32(hex, 16), 2)).ToString("0000 0000 0000 0000 0000 0000 0000 0000");
			}
			catch (Exception)
			{
				Debug.WriteLine("#ERR: Hex String to Int > not Hex format (Note:- '0X' within string is allowed)");
				return "";
			}
			return sbinary;
		}
		#endregion
		#region //====================================HexStringtoStringBinary16
		public string HexStringtoStringBinary16(string hex) // thank to expert exchance for this solution. 
		{
			string sbinary = "";
			try
			{
				//sbinary = Convert.ToString(Convert.ToInt16(hex, 16), 2);
				sbinary = Int32.Parse(Convert.ToString(Convert.ToInt16(hex, 16), 2)).ToString("0000 0000 0000 0000");
			}
			catch (Exception)
			{
				Debug.WriteLine("#ERR: Hex String to Int > not Hex format (Note:- '0X' within string is allowed)");
				return "";
			}
			return sbinary;
		}
		#endregion
		#region //====================================HexStringtoStringBinary8
		public string HexStringtoStringBinary8(string hex) 
		{
			string sbinary = "";
			try
			{
				//sbinary = Convert.ToString(Convert.Byte(hex, 16), 2);
				sbinary = Int32.Parse(Convert.ToString(Convert.ToByte(hex, 16), 2)).ToString("0000 0000");
				//http://stackoverflow.com/questions/4829366/byte-to-binary-string-c-sharp-display-all-8-digits
			}
			catch (Exception)
			{
				Debug.WriteLine("#ERR: Hex String to Int > not Hex format (Note:- '0X' within string is allowed)");
				return "";
			}
			return sbinary;
		}
		#endregion
		#region //====================================HexBytetoUInterger16
		public UInt16 HexBytetoUInterger16(byte D1, byte D0)
		{
			byte[] bytes = new byte[2];
			UInt16 i=0;
			try
			{
				bytes[0] = D0;
				bytes[1] = D1;

				if (BitConverter.IsLittleEndian)
					Array.Reverse(bytes);

				i = BitConverter.ToUInt16(bytes, 0);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("#ERR:(HexBytetoUInterger16): " + ex.Message);
			} 
			return i;
		}
		#endregion

		#region //====================================HexBytetoUInterger32
		public uint HexBytetoUInterger32(byte D3, byte D2, byte D1, byte D0)
		{
			byte[] bytes = new byte[4];
			uint i=0;
			try
			{
				bytes[0] = D0;
				bytes[1] = D1;
				bytes[2] = D2;
				bytes[3] = D3;

				if (BitConverter.IsLittleEndian)
					Array.Reverse(bytes);

				i = BitConverter.ToUInt32(bytes, 0);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("#ERR; (HexBytetoUInterger32): " + ex.Message);
			} 
				
			return i;
		}
		#endregion

		#region //====================================UInt16toHexByte
		public void UInt16toHexByte(uint uData16, out byte D0, out byte D1)
		{
			byte[] bytes = new byte[2];
			try
			{
				bytes = BitConverter.GetBytes(uData16);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("#ERR: (UInt16toHexByte)...check uData16: " + ex.Message);
			}
			D0 = bytes[0];
			D1 = bytes[1];

		}
        #endregion

        #region //====================================HexBytetoUInt16
        public UInt16 HexBytetoUInt16(Byte[] bytes, UInt16 startindex)
        {
            UInt16 UD16=0;
            try
            {
                UD16 =  System.BitConverter.ToUInt16(bytes, startindex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("#ERR: (HexBytetoUInt16)...check bytes[]: " + ex.Message);
            }
            return UD16;
        }
        #endregion

        #region //====================================UInt32toHexByte
        public void UInt32toHexByte(uint uData32, out byte D0, out byte D1, out byte D2, out byte D3)
		{
			byte[] bytes = new byte[4];
			try
			{
				bytes = BitConverter.GetBytes(uData32);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("#ERR:(UInt32toHexByte)...check uData32: " + ex.Message);
			} 
			D0 = bytes[0];
			D1 = bytes[1];
			D2 = bytes[2];
			D3 = bytes[3];
		}
        #endregion

        #region=====================================CVSStringtoIntArray (for csv string data decode into array)
        public int[] CVSStringtoIntArray(string sdata,int count,ref bool isfailed)  // count=15 for XDAQRCV project
		{
			isfailed=false;
			int[] idata = new int[count+1];
			try
			{
				char [] delimiter = {','};
				string[] s = new string[count+1];
				s=sdata.Split(delimiter,count+1);
				for(int i=0;i<count;i++)
				{
					if (s[i]==null || s[i]=="\0"  || s[i]=="")
					{
						idata[i]=0;								// no data, fill with zero.
					}
					else
					{
						idata[i]=Convert.ToInt32(s[i]);
					}
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine("#ERR: (CVSStringtoIntArray)...Check CVS data for error:" + ex.Message);
				isfailed=true;
			}
			return idata;
		}
		#endregion

		#region=====================================CVSStringtoFloatArray (for csv string data decode into array)
		public float[] CVSStringtoFloatArray(string sdata,int count,ref bool isfailed)
		{
			isfailed=false;
			float[] fdata = new float[count+1];
			try
			{
				char [] delimiter = {','};
				string[] s = new string[count+1];
				s=sdata.Split(delimiter,count+1);
				for(int i=0;i<count;i++)
				{
					if (s[i]==null || s[i]=="\0" || s[i]=="")
					{
						fdata[i]=0f;								// no data, fill with zero.
					}
					else
					{
						fdata[i]=(float)Convert.ToDouble(s[i]);
					}
				}
			}
			catch (Exception ee)
			{
				Debug.WriteLine("#ERR: (CVSStringtoFloatArray)...Check CVS data for error:" + ee.Message);
				isfailed=true;
			}
			return fdata;
		}
        #endregion

        #region=====================================IsString_Hex0x, check for string that is hex type.
        public bool IsString_Hex0x(string hexString)      // Updated Rev 1D
		{
			return hexString.Select(currentCharacter =>                     //LINQ technology.......
			(currentCharacter >= '0' && currentCharacter <= '9') ||
			(currentCharacter >= 'a' && currentCharacter <= 'f') ||
			(currentCharacter == 'x')|| (currentCharacter == 'X') ||        // 0x or 0X
			(currentCharacter >= 'A' && currentCharacter <= 'F')).All(isHexCharacter => isHexCharacter);

			/* The code below has bug, will not work with <0000>, above code worked better 
			hexString = HexString_Remove_0x(hexString);
			UInt32 output;
			UInt32.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, null, out output);      // Based on LINQ technology 03/12/11
			if (output == 0)
			{
				return false;
			}
			return true;
			 */
		}
		// http://stackoverflow.com/questions/223832/check-a-string-to-see-if-all-characters-are-hexadecimal-values
		#endregion

		#region=====================================IsString_Hex, check for string that is hex type.
		public bool IsString_Hex(string hexString)      // Updated Rev 1D
		{
			return hexString.Select(currentCharacter =>                     //LINQ technology.......
			(currentCharacter >= '0' && currentCharacter <= '9') ||
			(currentCharacter >= 'a' && currentCharacter <= 'f') ||
			(currentCharacter >= 'A' && currentCharacter <= 'F')).All(isHexCharacter => isHexCharacter);
			
			/* The code below has bug, will not work with <0000>, above code worked better 
			hexString = HexString_Remove_0x(hexString);
			UInt32 output;
			UInt32.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, null, out output);      // Based on LINQ technology 03/12/11
			if (output == 0)
			{
				return false;
			}
			return true;
			 */
		}
		// http://stackoverflow.com/questions/223832/check-a-string-to-see-if-all-characters-are-hexadecimal-values
		#endregion

		#region=====================================HexStringtoByte, convert Hex string to byte
		public byte HexStringtoByte(string hexString)
		{
			hexString = HexString_Remove_0x(hexString);
			UInt32 output;
			UInt32.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, null, out output);      // Based on LINQ technology 03/12/11
			if (output > 0xFF) output = 0xFF; // Limits value.
			return (Byte) output;
		}
		// http://stackoverflow.com/questions/223832/check-a-string-to-see-if-all-characters-are-hexadecimal-values
		#endregion

		#region=====================================HexStringtoUInt16, convert Hex string to UInt16
		public UInt16 HexStringtoUInt16(string sString)       //0q
		{
            sString = HexString_Remove_0q0w(sString);
			UInt16 output;
			UInt16.TryParse(sString, System.Globalization.NumberStyles.HexNumber, null, out output);      // Based on LINQ technology 03/12/11
			if (output > 0xFFFF) output = 0xFFFF; // Limits value.
			return (UInt16) output;
		}
        #endregion

        #region=====================================HexStringtoInt16, convert Hex string to Int16
        public Int16 HexStringtoInt16(string sString)         //0w
        {
            sString = HexString_Remove_0q0w(sString);
            Int16 output;
            Int16.TryParse(sString, System.Globalization.NumberStyles.HexNumber, null, out output);      // Based on LINQ technology 03/12/11
            return (Int16)output;
        }
        #endregion

        #region=====================================HexStringtoUInt32, convert Hex string to UInt32
        public UInt32 HexStringtoUInt32(string hexString)
		{
			hexString = HexString_Remove_0x(hexString);
			UInt32 output;
			UInt32.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, null, out output);      // Based on LINQ technology 03/12/11
			return  output;
		}
        #endregion

        #region //====================================HexStringtoInt32
        public Int32 HexStringtoInt32(string hexString)
        {
            hexString = HexString_Remove_0x(hexString);
            Int32 output;
            Int32.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, null, out output);      // Based on LINQ technology 03/12/11
            return output;
        }
        #endregion

        #region=====================================HexString_Remove_0x, filter out 0x and its variation.
        public string HexString_Remove_0x(string text)
		{
			// Remove "0x" or "0X" or "Ox" or "OX" if exist
			if ((text.StartsWith("0x") == true) || (text.StartsWith("0X") == true) || (text.StartsWith("Ox") == true) || (text.StartsWith("OX") == true))
			{
				text = text.Remove(0, 2);
			}
            if ((text.StartsWith("1x") == true) || (text.StartsWith("1X") == true) || (text.StartsWith("2x") == true) || (text.StartsWith("2X") == true)
                || (text.StartsWith("3x") == true) || (text.StartsWith("3X") == true) || (text.StartsWith("4x") == true) || (text.StartsWith("4X") == true))
            {
                text = text.Remove(0, 2);
            }
            return text;
		}
		#endregion

		#region=====================================HexString_Remove_0d, filter out 0d and its variation.
		public string HexString_Remove_0d(string text)
		{
			// Remove "0d" or "0D" or "Od" or "OD" if exist
			if ((text.StartsWith("0d") == true) || (text.StartsWith("0D") == true) || (text.StartsWith("Od") == true) || (text.StartsWith("OD") == true))
			{
				text = text.Remove(0, 2);
			}
			return text;
		}
        #endregion

        #region=====================================HexString_Remove_0q and 0w, 
        public string HexString_Remove_0q0w(string text)
        {
            // Remove "0d" or "0D" or "Ow" or "OW" if exist
            if ((text.StartsWith("0q") == true) || (text.StartsWith("0Q") == true) || (text.StartsWith("Ow") == true) || (text.StartsWith("OW") == true))
            {
                text = text.Remove(0, 2);
            }
            return text;
        }
        #endregion

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        #region=====================================IsString_Numberic_Int32
        public bool IsString_Numberic_Int32(string text)
		{
			int i = 0;
			return (Int32.TryParse(text, out i));
		}
        #endregion

        #region=====================================IsString_Numberic_Byte
        public bool IsString_Numberic_Byte(string text)
        {
            byte b = 0;
            return (Byte.TryParse(text, out b));
        }
        #endregion

        #region=====================================IsString_Numberic_UInt32
        public bool IsString_Numberic_UInt32(string text)
		{
			UInt32 i = 0;
			return (UInt32.TryParse(text, out i));
		}
        #endregion

        #region=====================================IsString_Numberic_Double
        public bool IsString_Numberic_Double(string text)
        {
            Double i = 0;
            return (Double.TryParse(text, out i));
        }
        #endregion

        #region=====================================IsString_Numberic_Bool
        public bool IsString_Numberic_Bool(string text)
		{
			bool i = false;
			return (bool.TryParse(text, out i));
		}
		#endregion

		#region=====================================IsString_Numberic_01
		public bool IsString_Numberic_01(string text)
		{
			if ((text.Contains("0")) || (text.Contains("1")))
			{
				if (text.Length == 1)   // Only one digit is allowed. 
					return true; 
			}
			return false;
		}
		#endregion

		#region=====================================IsString_Double 
		public bool IsString_Double(string text)
		{
			double d = 0;
			return (double.TryParse(text, out d));
		}
		#endregion

		#region=====================================isStringContain0x (Hex)
		public bool isStringContain0x(string text)
		{
			if ((text.StartsWith("0x") == true) || (text.StartsWith("0X") == true))
			{
				return true;
			}
			return false;
		}
        #endregion

        #region=====================================isStringContainxx (Hex, 0x,1x,2x,3x,4x
        public bool isStringContainxx(string text)
        {
            if ((text.StartsWith("0x") == true) || (text.StartsWith("0X") == true))
            {
                return true;
            }
            if ((text.StartsWith("1x") == true) || (text.StartsWith("1X") == true))
            {
                return true;
            }
            if ((text.StartsWith("2x") == true) || (text.StartsWith("2X") == true))
            {
                return true;
            }
            if ((text.StartsWith("3x") == true) || (text.StartsWith("3X") == true))
            {
                return true;
            }
            if ((text.StartsWith("4x") == true) || (text.StartsWith("4X") == true))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region=====================================isStringContain0d (double)
        public bool isStringContain0d(string text)
		{
			if ((text.StartsWith("0d") == true) || (text.StartsWith("0D") == true))
			{
				return true;
			}
			return false;
		}
		#endregion

		#region=====================================isStringContain0i (signed int32)
		public bool isStringContain0i(string text)
		{
			if ((text.StartsWith("0i") == true) || (text.StartsWith("0I")==true))
			{
				return true;
			}
			return false;
		}
        #endregion

        #region=====================================isStringContain0l (unsigned int32)
        public bool isStringContain0l(string text)
        {
            if ((text.StartsWith("0l") == true) || (text.StartsWith("0L") == true))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region=====================================isStringContain0q (unsigned int32)
        public bool isStringContain0q(string text)
        {
            if ((text.StartsWith("0q") == true) || (text.StartsWith("0Q") == true))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region=====================================isStringContain0w (signed int16)
        public bool isStringContain0w(string text)
        {
            if ((text.StartsWith("0w") == true) || (text.StartsWith("0W") == true))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region=====================================isStringContain0u (unsigned int32)
        public bool isStringContain0u(string text)
		{
			if ((text.StartsWith("0u") == true) || (text.StartsWith("0u") == true))
			{
				return true;
			}
			return false;
		}
		#endregion

		#region=====================================isStringContain0s (string)
		public bool isStringContain0s(string text)
		{
			if ((text.StartsWith("0s") == true) || (text.StartsWith("0S") == true))
			{
				return true;
			}
			return false;
		}
        #endregion

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        #region=====================================StrReplaceAtIndex, modify char in the string
        /// <summary>
        /// Replace a string char at index with another char
        /// </summary>
        /// <param name="text">string to be replaced</param>
        /// <param name="index">position of the char to be replaced</param>
        /// <param name="c">replacement char</param>
        public string StrReplaceAtIndex(string text, int index, char c)
		{
			var stringBuilder = new StringBuilder(text);
			stringBuilder[index] = c;
			return stringBuilder.ToString();
		}
        #endregion

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        #region//====================================Bits_UInt32_Set()
        public UInt32 Bits_UInt32_Set(UInt32 Data32, int index)
		{
			return (Data32 | (UInt32)(1 << index));        // set bit
		}
        #endregion

        #region//====================================Bits_UInt32_Set()
        public UInt16 Bits_UInt16_Set(UInt16 Data16, int index)
        {
            return (UInt16)(Data16 | (UInt16)(1 << index));        // set bit
        }
        #endregion

        #region//====================================Bits_UInt32_Clear()
        public UInt32 Bits_UInt32_Clear(UInt32 Data32, int index)
		{
			return (Data32  & (UInt32)(~(1 << index)));     // clear bit
		}
        #endregion

        #region//====================================Bits_UInt16_Clear()
        public UInt16 Bits_UInt16_Clear(UInt16 Data16, int index)
        {
            return (UInt16)(Data16 & (UInt16)(~(1 << index)));     // clear bit
        }
        #endregion

        #region//====================================Bits_UInt32_Read()
        public bool Bits_UInt32_Read(UInt32 Data32, int index)
		{
			if (((Data32 >> index) & 1)==1)
				return true;
			return false;                // clear bit
		}
        #endregion

        #region//====================================Bits_UInt16_Read()
        public bool Bits_UInt16_Read(UInt16 Data16, int index)
        {
            if (((Data16 >> index) & 1) == 1)
                return true;
            return false;                // clear bit
        }
        #endregion

        #region//====================================Bits_UInt32_intRead()
        public int Bits_UInt32_intRead(UInt32 Data32, int index)
		{
			if (((Data32 >> index) & 1)==1)
				return 1;
			return 0;                // clear bit
		}
        #endregion
        #region//====================================Bits_UInt32_intRead2Bit()
        public int Bits_UInt32_intRead2Bit(UInt32 Data32, int index)
        {
            return (int)((Data32 >> index) & 0x3);
        }
        #endregion
        #region//====================================Bits_UInt32_intRead3Bit()
        public int Bits_UInt32_intRead3Bit(UInt32 Data32, int index)
        {
            return (int)((Data32 >> index) & 0x7);
        }
        #endregion
        #region//====================================Bits_UInt32_intRead4Bit()
        public int Bits_UInt32_intRead4Bit(UInt32 Data32, int index)
        {
            return (int)((Data32 >> index) & 0xF);
        }
        #endregion

        #region//====================================Bits_UInt32_uRead()
        public UInt32 Bits_UInt32_uRead(UInt32 Data32, int index)
        {
            if (((Data32 >> index) & 1) == 1)
                return 1;
            return 0;                // clear bit
        }
        #endregion
        #region//====================================Bits_UInt32_uRead2Bit()
        public UInt32 Bits_UInt32_uRead2Bit(UInt32 Data32, int index)
        {
            return (UInt32)((Data32 >> index) & 0x3);
        }
        #endregion
        #region//====================================Bits_UInt32_uRead3Bit()
        public UInt32 Bits_UInt32_uRead3Bit(UInt32 Data32, int index)
        {
            return (UInt32)((Data32 >> index) & 0x7);
        }
        #endregion
        #region//====================================Bits_UInt32_uRead4Bit()
        public UInt32 Bits_UInt32_uRead4Bit(UInt32 Data32, int index)
        {
            return (UInt32)((Data32 >> index) & 0xF);
        }
        #endregion
        #region//====================================Bits_UInt32_uRead8Bit()
        public UInt32 Bits_UInt32_uRead8Bit(UInt32 Data32, int index)
        {
            return (UInt32)((Data32 >> index) & 0xFF);
        }
        #endregion
        #region//====================================Bits_UInt32_uRead9Bit()
        public UInt32 Bits_UInt32_uRead9Bit(UInt32 Data32, int index)
        {
            return (UInt32)((Data32 >> index) & 0x1FF);
        }
        #endregion
        #region//====================================Bits_UInt32_uRead16Bit()
        public UInt32 Bits_UInt32_uRead16Bit(UInt32 Data32, int index)
        {
            return (UInt32)((Data32 >> index) & 0xFFFF);
        }
        #endregion

        #region//====================================Bits_UInt32_inWrite()
        public UInt32 Bits_UInt32_intWrite(UInt32 Data32, int index, UInt32 data)
        {
            UInt32 Data32o = (Data32 & (UInt32)(~(0x1 << index)));        // Clear two bits
            Data32o = (Data32o | (UInt32)(data << index));
            return Data32o;
        }
        #endregion
        #region//====================================Bits_UInt32_inWrite2Bit()
        public UInt32 Bits_UInt32_intWrite2Bit(UInt32 Data32, int index, UInt32 data)
        {
            UInt32 Data32o = (Data32 & (UInt32)(~(0x3 << index)));        // Clear two bits
            Data32o = (Data32o | (UInt32)(data << index));
            return Data32o;
        }
        #endregion
        #region//====================================Bits_UInt32_intWrite3Bit()
        public UInt32 Bits_UInt32_intWrite3Bit(UInt32 Data32, int index, UInt32 data)
        {
            UInt32 Data32o = (Data32 & (UInt32)(~(0x7 << index)));        // Clear two bits
            Data32o = (Data32o | (UInt32)(data << index));
            return Data32o;
        }
        #endregion
        #region//====================================Bits_UInt32_inWrite4Bit()
        public UInt32 Bits_UInt32_intWrite4Bit(UInt32 Data32, int index, UInt32 data)
        {
            UInt32 Data32o = (Data32 & (UInt32)(~(0xF << index)));        // Clear four bits
            Data32o = (Data32o | (UInt32)(data << index));
            return Data32o;
        }
        #endregion
        #region//====================================Bits_UInt32_inWrite8Bit()
        public UInt32 Bits_UInt32_intWrite8Bit(UInt32 Data32, int index, UInt32 data)
        {
            UInt32 Data32o = (Data32 & (UInt32)(~(0xFF << index)));        // Clear 8 bits
            Data32o = (Data32o | (UInt32)(data << index));
            return Data32o;
        }
        #endregion
        #region//====================================Bits_UInt32_inWrite9Bit()
        public UInt32 Bits_UInt32_intWrite9Bit(UInt32 Data32, int index, UInt32 data)
        {
            UInt32 Data32o = (Data32 & (UInt32)(~(0x1FF << index)));        // Clear 9 bits
            Data32o = (Data32o | (UInt32)(data << index));
            return Data32o;
        }
        #endregion
        #region//====================================Bits_UInt32_inWrite16Bit()
        public UInt32 Bits_UInt32_intWrite16Bit(UInt32 Data32, int index, UInt32 data)
        {
            UInt32 Data32o = (Data32 & (UInt32)(~(0xFFFF << index)));        // Clear 16 bits
            Data32o = (Data32o | (UInt32)(data << index));
            return Data32o;
        }
        #endregion

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        #region //====================================CountOccurrencesString()
        //http://stackoverflow.com/questions/541954/how-would-you-count-occurrences-of-a-string-within-a-string
        public int CountOccurrencesString(string s, string substring, bool aggressiveSearch = false)
		{
			// if s or substring is null or empty, substring cannot be found in s
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(substring))
				return 0;

			// if the length of substring is greater than the length of s,
			// substring cannot be found in s
			if (substring.Length > s.Length)
				return 0;
			int count = 0, n = 0;
			while ((n = s.IndexOf(substring, n, StringComparison.InvariantCulture)) != -1)
			{
				if (aggressiveSearch)
					n++;
				else
					n += substring.Length;
				count++;
			}
			return count;
		}
        #endregion

        #region//====================================getOSInfo
        // Based on http://stackoverflow.com/questions/2819934/detect-windows-7-in-net 
        public string getOSInfo()
		{
			//Get Operating system information.
			OperatingSystem os = Environment.OSVersion;
			//Get version information about the os.
			Version vs = os.Version;

			//Variable to hold our return value
			string operatingSystem = "";

			switch (os.Platform)
			{
			    case PlatformID.Win32Windows:
			    {
			        //This is a pre-NT version of Windows
			        switch (vs.Minor)
			        {
			            case 0:
			                operatingSystem = "95";
			                break;
			            case 10:
			                if (vs.Revision.ToString() == "2222A")
			                    operatingSystem = "98SE";
			                else
			                    operatingSystem = "98";
			                break;
			            case 90:
			                operatingSystem = "Me";
			                break;
			            default:
			                break;
			        }
			        break;
			    }
			    case PlatformID.Win32NT:
			    {
			        switch (vs.Major)
			        {
			            case (3):
			            {
			                operatingSystem = "NT 3.51";
			                break;
			            }
			            case (4):
			            {
			                operatingSystem = "NT 4.0";
			                break;
			            }
			            case (5):
			            {
			                if (vs.Minor == 0)
			                    operatingSystem = "2000";
			                else
			                    operatingSystem = "XP";
			                break;
			            }
			            case (6):
			            {
			                switch (vs.Minor)
			                {
			                    case (0):
			                        operatingSystem = "Vista"; //also 2008
			                        break;
			                    case (1):
			                        operatingSystem = "Window-7"; // also 2008-R2
			                        break;
			                    case (2):
			                        operatingSystem = "Window-8";
			                        break;
			                    case (3):
			                        operatingSystem = "Window-81";
			                        break;
			                    default:
			                        break;
			                }
			                break;
			            }
			            case (10):
			            {
			                operatingSystem = "Window-10";
			                break;
			            }
			            default:
			            {
			                operatingSystem = "No Such OS!";
			                break;
			            }
			        }
			        break;
			    }
			}
			return operatingSystem;
		}
        #endregion

        #region//====================================FormatHelper for StringToBoolean().
        public class FormatHelper
        {
            public static Boolean StringToBoolean(String str)
            {
                return StringToBoolean(str, false);
            }

            public static Boolean StringToBoolean(String str, Boolean bDefault)
            {
                String[] BooleanStringOff = { "0", "off", "no" };

                if (String.IsNullOrEmpty(str))
                    return bDefault;
                else if (BooleanStringOff.Contains(str, StringComparer.InvariantCultureIgnoreCase))
                    return false;

                Boolean result;
                if (!Boolean.TryParse(str, out result))
                    result = true;

                return result;
            }
        }
        #endregion

        #region//====================================TrimStart with string.
        public string TrimStartwithString(string source, string toTrim)
        {
            var s = source;
            while (s.StartsWith(toTrim))
            {
                s = s.Substring(toTrim.Length - 1);

            }
            return s;
        }
        #endregion

        #region//====================================InteractivePause (alternative to Thread.Sleep(xx).
        // This code create pause but does not lockup the UI interface,
        // http://stackoverflow.com/questions/1419363/pause-execution-of-a-method-without-locking-gui-c-sharp 
        public void InteractivePause(TimeSpan length)
        {
            DateTime start = DateTime.Now;
            TimeSpan restTime = new TimeSpan(100000); // 10 milliseconds
            while (true)
            {
                System.Windows.Forms.Application.DoEvents();
                TimeSpan remainingTime = start.Add(length).Subtract(DateTime.Now);
                if (remainingTime > restTime)
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("1: {0}", remainingTime));
                    // Wait an insignificant amount of time so that the
                    // CPU usage doesn't hit the roof while we wait.
                    System.Threading.Thread.Sleep(restTime);
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("2: {0}", remainingTime));
                    if (remainingTime.Ticks > 0)
                        System.Threading.Thread.Sleep(remainingTime);
                    break;
                }
            }
        }
        #endregion

        #region//====================================RegisteryReadLocalMachine 
        //using (RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\MySQL AB\\MySQL Connector\\Net"))
        public string RegisteryReadLocalMachine(string Subkey, string GetValue)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(Subkey))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue(GetValue);
                        if (o != null)
                        {
                            return o.ToString();
                            //Version version = new Version(o as String);  //"as" because it's REG_SZ...otherwise ToString() might be safe(r)
                            //do what you like with version
                        }
                    }
                }
            }
            catch { } //just for demonstration...it's always best to handle specific exceptions
            return "ERR";
        }
        #endregion

        #region//====================================RegisteryReadLocalMachine_GetSubKeyNames 
        public string[] RegisteryReadLocalMachine_GetSubKeyNames(string Subkey)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(Subkey);

            //return RegistryKey.GetSubKeyNames(string Subkey);

            //foreach (string subKeyName in key.GetSubKeyNames())
            //{
            //
            //}
            if (key != null) return key.GetSubKeyNames();
            return new[] {""};
        }

        #endregion

        #region//====================================File_GetLine
        // Return: -ERR1- filename not found
        // Return: -ERR2- Read Error occurred (catch).
        // Return: "" no readline found
        //
        public string File_GetLine(string fileName, int myLineNumber)
        {
            string sData = null;
            bool exists = File.Exists(fileName);
            if (exists == false)
                return "-ERR1-";
            try
            {
                using (var reader = new StreamReader(fileName))
                {
                    for (int i = 0; i < myLineNumber; ++i)
                    {
                        sData = reader.ReadLine();
                    }
                }
            }
            catch
            {
                return "-ERR2-";
            }
            return sData;
        }
        #endregion

        #region//====================================File_NoOfLines
        // Return: -1 filename not found
        // Return: -2 Read Error occurred (catch).
        // Return: posiive number = number of lines in txt file. 
        public int File_NoOfLines(string fileName)
        {
            int lineCount;
            bool exists = File.Exists(fileName);
            if (exists == false)
                return (-1);
            try
            {
                lineCount = File.ReadLines(fileName).Count();
            }
            catch
            {
                return (-2);
            }
            return lineCount;
        }
        #endregion

        #region//====================================SplitAndKeepDelimitersSeperate
        //https://stackoverflow.com/questions/4680128/split-a-string-with-delimiters-but-keep-the-delimiters-in-the-result-in-c-sharp
        // Splits the given string into a list of substrings, while outputting the splitting
        // delimiters(each in its own string) as well.It's just like String.Split() except
        // the delimiters are preserved.No empty strings are output.</summary>
        // <param name = "s" > String to parse. Can be null or empty.</param>
        // <param name = "delimiters" > The delimiting characters. Can be an empty array.</param>
        // example: string s = "mouse\n  test\n" with SplitAndKeepDelimitersSeperate(s, '\n') goes to string[] part results
        //  mouse
        //  \n
        //    test
        //  \n
        public string[] SplitAndKeepDelimitersSeperate(string s, params char[] delimiters)
        {
            var parts = new List<string>();
            string[] partss = null;
            if (!string.IsNullOrEmpty(s))
            {
                int iFirst = 0;
                do
                {
                    int iLast = s.IndexOfAny(delimiters, iFirst);
                    if (iLast >= 0)
                    {
                        if (iLast > iFirst)
                            parts.Add(s.Substring(iFirst, iLast - iFirst)); //part before the delimiter
                        parts.Add(new string(s[iLast], 1));//the delimiter
                        iFirst = iLast + 1;
                        continue;
                    }

                    //No delimiters were found, but at least one character remains. Add the rest and stop.
                    parts.Add(s.Substring(iFirst, s.Length - iFirst));
                    break;

                } while (iFirst < s.Length);
                partss = parts.ToArray();
            }
            return partss;
        }
        #endregion

        #region//====================================SplitAndKeepDelimiters
        //https://stackoverflow.com/questions/4680128/split-a-string-with-delimiters-but-keep-the-delimiters-in-the-result-in-c-sharp
        // Splits the given string into a list of substrings, while outputting the splitting
        // delimiters(each in its own string) as well.It's just like String.Split() except
        // the delimiters are preserved.No empty strings are output.</summary>
        // <param name = "s" > String to parse. Can be null or empty.</param>
        // <param name = "delimiters" > The delimiting characters. Can be an empty array.</param>
        // example: string s = "mouse\n  test\n" with SplitAndKeepDelimiters(s, '\n') goes to string[] part results
        //  mouse\n
        //    test\n
        // In case of '\n\' only, it add to string list (implemented by Riscy).
        // Used in <USB_Message_Manager> section. 
        public string[] SplitAndKeepDelimiters(string s, params char[] delimiters)
        {
            var parts = new List<string>();
            string[] partss = null;
            if (!string.IsNullOrEmpty(s))
            {
                int iFirst = 0;
                do
                {
                    int iLast = s.IndexOfAny(delimiters, iFirst);
                    if (iLast >= 0)
                    {
                        if (iLast > iFirst)                                                             // Riscy modified code.
                        {
                            parts.Add(s.Substring(iFirst, iLast - iFirst)+ (new string(s[iLast], 1)));  // part plus a delimiter
                        }
                        if (iLast==iFirst)                                                              // Riscy added code to capture single char '\n'
                        {
                            parts.Add(new string(s[iLast], 1));                                         // add single char 
                        }
                        iFirst = iLast + 1;
                        continue;
                    }
                    //No delimiters were found, but at least one character remains. Add the rest and stop.
                    parts.Add(s.Substring(iFirst, s.Length - iFirst));
                    break;

                } while (iFirst < s.Length);
                partss = parts.ToArray();       // List<> to String array
            }
            return partss;
        }
        #endregion

        #region//====================================SplitAndKeepDelimitersX
        // Below change the way the delimiter is passed to next string array.  
        public string[] SplitAndKeepDelimitersX(string s, char delimiters)
        {
            var parts = new List<string>();
            try
            {
                string ss = "";
                char[] b = new char[s.Length];
                b = s.ToCharArray();
                //-------------------------------------------
                int i = 0;
                if (b[0] == delimiters)            // First line has |Delay. 
                {
                    ss = b[0].ToString();
                    i++;
                }
                //-------------------------------------------
                while (i < s.Length)                // Index.
                {
                    while (i < s.Length)
                    {
                        if (b[i] == delimiters)        // 2nd line has |Delay detected.
                            break;
                        ss += b[i].ToString();
                        i++;
                    }
                    parts.Add(ss);
                    if (i >= s.Length)
                        break;
                    ss = "";
                    ss += b[i].ToString();
                    i++;
                }
            }
            catch
            {
                return null;
            }
           string[] partss = parts.ToArray();
           return partss;
        }
        #endregion

        #region//====================================IsHex
        public bool IsHex(char c)
        {
            return ((c >= '0' && c <= '9') ||
                   (c >= 'a' && c <= 'f') ||
                   (c >= 'A' && c <= 'F'));
        }
        #endregion

        #region//====================================IntToHexChar
        public char IntToHexChar(int i)
        {
            if ((i < 0) | (i > 15))
                return '\0';
            return (Char.Parse(i.ToString("X")));
        }
        #endregion

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Ethernet/IP

        #region//====================================Ethernet_ValidateIPv4
        // https://stackoverflow.com/questions/11412956/what-is-the-best-way-of-validating-an-ip-address
        public bool Ethernet_ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }
        #endregion

        #region//====================================Ethernet_ValidatePortNumber
        public int Ethernet_ValidatePortNumber(string sPort,int minRange, int maxRange)
        {
            if ((minRange < 0) || (minRange > 0xFFFF))    // Out of Range.
            {
                return -1;
            }
            if ((maxRange < 0) || (maxRange > 0xFFFF))  // Out of Range.
            {
                return -2;
            }
            if (minRange > maxRange)            // wrong way.
            {
                return -3;
            }
            int iPort;

            if (int.TryParse(sPort, out iPort) && (iPort >= minRange) && (iPort <= maxRange))
            {
                return iPort;
            }
            return -5;
        }
        #endregion




        //======End of program

    }
}
//======
