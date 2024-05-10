using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDT_Term.Static
{
    public static class ExtensionMethods
    {
        public static UInt32 ConversionStringtoUInt32(this string item)
        {
            UInt32 datai;

            if (UInt32.TryParse(item, out datai) == false)
            {
                //Debug.WriteLine("#ERR: String to UInt32 Failed");
                return (UInt32)0xFFFFFFFF;
            }
            return datai;

        }

    }
}
