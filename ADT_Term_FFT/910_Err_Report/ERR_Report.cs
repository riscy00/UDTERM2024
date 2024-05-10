using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDT_Term_FFT
{
    public class ERR_Report
    {
        ITools Tools = new Tools();
        
        // ============================================================Reference Object

        // ============================================================Private Variable

        //=============================================================Public Variable
        public List<ErrReport> iErrReport;
        //=============================================================Constructor
        public ERR_Report()
        {
            iErrReport = new List<ErrReport>();
            //------------------------------------------Battery Section
            iErrReport.Add(new ErrReport(0x0020, "Battery: Charge failure due to activate Survey/Logger Operation, this need to be canceled (issue STOP command)."));
            iErrReport.Add(new ErrReport(0x0021, "Battery: VUSB less than 4V0, poor quality cable, poor quality adapter (cheap china craps), damaged cable. Power Cuts, Pulled USB cable."));
            iErrReport.Add(new ErrReport(0x0022, "Battery: Battery Charging is Activated"));
            iErrReport.Add(new ErrReport(0x0023, "Battery: VUSB less than 4V0, poor quality cable, poor quality adapter (cheap china craps), damaged cable. Power Cuts, Pulled USB cable."));
            iErrReport.Add(new ErrReport(0x0024, "Battery: VBAT has <2000mV, likely to be defective, take back to service for replacement tool or replace battery."));
            iErrReport.Add(new ErrReport(0x0025, "Battery: Battery Not connected (debug/test only). All field tool has battery. However some application use external power source....."));
            iErrReport.Add(new ErrReport(0x0026, "Battery: Tool too Cold (<0degC) to charge battery, ideally Within 10C to 45C  (JEITA1 SPEC)."));
            iErrReport.Add(new ErrReport(0x0027, "Battery: Tool too Hot (>60degC) to charge battery, ideally Within 10C to 45C  (JEITA1 SPEC)."));
            iErrReport.Add(new ErrReport(0x0028, "Battery: During Adapter Mode, the device was not disconnected on time prior to transfer to adapter charger."));
            iErrReport.Add(new ErrReport(0x0029, "Battery: During Adapter Mode, the device was not reconnected on time or adapter not activated."));
            iErrReport.Add(new ErrReport(0x002A, "Battery: Battery charging interrupted by STOP command."));
            iErrReport.Add(new ErrReport(0x002B, "Battery: No Command (UART/BLE) detected, proceed to battery charging request."));
            iErrReport.Add(new ErrReport(0xFFF0, "Battery: Passed as fully charged battery, no error to report."));
            //------------------------------------------
        }
    }

    //#########################################################################################################
    //########################################################################################## UDT Configuration Class 
    //#########################################################################################################

    #region -- Configuration Class --
    [Serializable]
    public class ErrReport
    {
        // ==================================================================Getter/Setter
        public int ErrNumber { get; set; }
        public string ErrString { get; set; }

        // ==================================================================constructor
        public ErrReport(int iErrNumber, string sErrString)
        {
            this.ErrNumber = iErrNumber;
            this.ErrString = sErrString;

        }
    }
    #endregion
}

