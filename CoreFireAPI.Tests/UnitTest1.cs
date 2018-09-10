using System;
using System.IO;
using System.Linq;
using CoreFireAPI.BLL;
using CoreFireAPI.Models;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Moq;
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

            var b = new NextMonthSchedule(null);
           b.SetupWorkingDays(a);

            // act
            b.AddStandardTimeslotsToEveryWorkingDay();

            // assert
            Assert.NotEmpty(b.DaysAndTimes);
            Assert.Equal(4, b.DaysAndTimes.Count());
            Assert.Equal(12, b.DaysAndTimes.First().Timeslots.Count());
        }

        [Fact]
        public void ShouldSaveScheduleIntoFireDatabase()
        {
            // arrange
            var a = new string[]
            {
                "2018-09-07",
                "2018-09-08",
                "2018-09-11",
                "2018-09-12",
            };

            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    .AddEnvironmentVariables();
            //ConfigurationManager<>.Configuration = builder.Build();
            //services.Configure<FireConnection>(Configuration.GetSection("ConnectionStrings"));

            //var services = new ServiceCollection();
            //services.AddTransient<FirebaseDataService>();
            //services.AddOptions();

            //var serviceProvider = services.BuildServiceProvider();
            //FirebaseDataService firebaseDataService = serviceProvider.GetService<FirebaseDataService>();
            //firebaseDataService.SetConnectionString("https://firecoretest.firebaseio.com/");
            //var mockedOptions = Mock<IOptions<FireConnection>>;
            //FirebaseDataService firebaseDataService = firebaseDataService(null);
            var b = new NextMonthSchedule(null);
            b.SetupWorkingDays(a);

            // act
            b.AddStandardTimeslotsToEveryWorkingDay();

            // assert
            b.SaveIntoFireStorage();
        }
    }
}
