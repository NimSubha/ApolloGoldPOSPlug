using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DecimalToText
{
    [ComDefaultInterface(typeof(IToArabic))]
    [ClassInterface(ClassInterfaceType.None)]
    public class ToArabic:IToArabic
    {
        public string ConvertToText(double amt)
        {
            CurrencyInfo Currency = new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia);
            try
            {
                ToText totext = new ToText((Decimal)amt, Currency);
                return totext.ConvertToArabic();
            }
            catch
            {
                return "";
            }
        }

        public string FromTextToText(string amt)
        {
            CurrencyInfo Currency = new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia);
            try
            {
                Decimal DAmt;
                Decimal.TryParse(amt,out DAmt);
                ToText totext = new ToText(DAmt, Currency);
                return totext.ConvertToArabic();
            }
            catch
            {
                return "";
            }
        }
    }
}
