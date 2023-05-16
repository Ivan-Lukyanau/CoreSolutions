using System;
using System.Collections.Generic;
using System.Text;
using CoreFireAPI.Extensions;
using Xunit;

namespace CoreFireAPI.Tests
{
    public class DateTimeHelpersTests
    {
        [Theory]
        [InlineData(1, "Январь")]
        [InlineData(2, "Февраль")]
        [InlineData(3, "Март")]
        [InlineData(4, "Апрель")]
        [InlineData(5, "Май")]
        [InlineData(6, "Июнь")]
        [InlineData(7, "Июль")]
        [InlineData(8, "Август")]
        [InlineData(9, "Сентябрь")]
        [InlineData(10, "Октябрь")]
        [InlineData(11, "Ноябрь")]
        [InlineData(12, "Декабрь")]
        private void GetMonthNameByMonthNumber_Returns_Proper_MonthName(int monthNumber, string expected)
        {
            var monthName = monthNumber.GetMonthNameByMonthNumber();

            Assert.Equal(expected, monthName);
        }
    }
}
