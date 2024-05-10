using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDT_Term_FFT;

namespace UDT_Term
{

    #region //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++SNADNetwork
    [Serializable]
    public class SNADNetwork
    {
        ITools Tools = new Tools();
        public bool isNetworkEnabled { get; set; }      // SNAD is activated, ready for use. 
        public bool isInitSuccess { get; set; }          // SNAD Init procedure recieved reply
        public bool isMatchNotFound { get; set; }       // SNAD device did not accept command (no match event)
        public int ChannelSelect { get; set; }         // Selected Channel via UDT setup (outgoing message)
        public int ChannelRecieved { get; set; }         // Recieved Channel from device (incoming message)
        public int ChannelRecievedLogCVS{get; set;}      // Recieved Channel from device via LoggerCVS
        public int  chMinRange { get; set; }            // Alway 1
        public int  chMaxRange{ get; set; }             // Maximum channel number, ie 2 for two tool in SNAD network

        public int[] EnableChannel;                     // based on SNAD init, number of 

        //#####################################################################################################
        //###################################################################################### Constructor
        //#####################################################################################################
        public SNADNetwork()
        {
            ChannelRecievedLogCVS = -1;
            ChannelRecieved = -1;
            isNetworkEnabled = false;
            isInitSuccess = false;
            isMatchNotFound = false;
            ChannelSelect = -1;                         // Not selected. 
            chMinRange = 1;
            chMaxRange = 1;
            EnableChannel = new int[15];                // 1 to 15 only
            clearEnableChannel();
        }
        //#####################################################################################################
        //###################################################################################### Methods
        //#####################################################################################################
        //------------------------------------------Clear Enable channels
        public void clearEnableChannel()
        {
            for (int i = 0; i < 15; i++)
                EnableChannel[i] = 0;
        }
        //-----------------------------------------This add SNAD Address to command. 
        public string SNAD_Modify_UDT_Command(string sCommandMessage)
        {
            if ((ChannelSelect != -1) & (isNetworkEnabled == true))
            {
                if (sCommandMessage.IndexOf(')') > 1)      //----Seek ')'
                {
                    string sMessage = sCommandMessage.Replace("\n", "");
                    sMessage = sMessage.Replace("\n", ""); // Just to make sure.
                    sMessage = sMessage + ChannelSelect.ToString("X");
                    sMessage = sMessage + "\n";
                    sCommandMessage = sMessage;
                }
            }
            return sCommandMessage;
        }

        //-----------------------------------------Read SNAD Number if exist, return true if found.
        public bool SNAD_Read_SNAD_From_UDT_Command(string sRecievedMessage)
        {
            ChannelRecieved = -1;
            int pos = sRecievedMessage.IndexOf(')');        //----Seek ')'
            if (pos > 1)      
            {
                string ss = sRecievedMessage.Substring(pos + 1,1);
                if (ss == "\n")
                    return (false);
                bool isHex = ss.All("0123456789abcdefABCDEF".Contains);
                if (isHex == false)
                    return (false);
                ChannelRecieved = (Int32.Parse(ss, NumberStyles.HexNumber));
                return (true);
            }
            return (false);
        }
        //-----------------------------------------Read SNAD Number from LoggerCVS, All message to have standard $D,SNAD,SYSTICK or $D;SNAD;SYSTICK 
        public bool SNAD_Read_SNAD_From_LoggerCVS(string sRecievedMessage)
        {
            string[] seek = { "$D","$G"};                                   // Leave $H and $F out for now. 
            if (Tools.StringContainsAny(sRecievedMessage, seek) ==false)
                return false;
            ChannelRecievedLogCVS = -1;         // Treat as non SNAD Network setup. 
            //--------------------------------------------------------------Detoken
            char[] delimiterChars = { ',', ';' };                               
            string[] sTokenDatas = sRecievedMessage.Split(delimiterChars);
            if (sTokenDatas.Length < 1)
                return false;
            //--------------------------------------------------------------Obtain SNAD string
            string sSNAD = sTokenDatas[1];
            //--------------------------------------------------------------Check SNAD and then to number. 
            bool isHex = sSNAD.All("0123456789abcdefABCDEF".Contains);
            if (isHex == false)
                return false;
            ChannelRecievedLogCVS = (Int32.Parse(sSNAD, NumberStyles.HexNumber));
            return (true);
        }
    }
    #endregion
}
