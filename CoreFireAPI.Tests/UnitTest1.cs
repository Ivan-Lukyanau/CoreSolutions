using System;
using System.Linq;
using CoreFireAPI.Models;
using Xunit;

namespace CoreFireAPI.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void ShouldReturnsProperOjectOfWorkingSchedule()
        {
            // arrange
            var a = new string[]
            {
                "2018-09-07",
                "2018-09-08",
                "2018-09-11",
                "2018-09-12",
            };

            var b = new NextMonthSchedule(a);

            // act
            b.AddStandardTimeslotsToEveryWorkingDay();

            // assert
            Assert.NotEmpty(b.DaysAndTimes);
            Assert.Equal(4, b.DaysAndTimes.Count());
            Assert.Equal(12, b.DaysAndTimes.First().TimeSlots.Count());
        }
    }
}
