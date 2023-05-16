using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CoreFireAPI.Extensions
{
    public static class DateTimeHelpers
    {
        public static string GetMonthNameByMonthNumber(this int number)
        {
            return CultureInfo.CreateSpecificCulture("ru").DateTimeFormat.GetMonthName(number);
        }
    }
}
