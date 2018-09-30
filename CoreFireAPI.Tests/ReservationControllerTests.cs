using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using CoreFireAPI.BLL;
using CoreFireAPI.Controllers;
using CoreFireAPI.Models.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CoreFireAPI.Tests
{
    public class ReservationControllerTests
    {
        [Fact]
        public async Task ShouldReturn_InternaServerErrorResult_WhenServiceIsCrushed()
        {
            var fireDataService = new Mock<IFirebaseDataService>();
            fireDataService.Setup(m => m.GetReservationsForDay(
                It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>()
            )).Throws(new Exception());

            var controller = new ReservationController(fireDataService.Object);

            var result = await controller.Get("Октябрь", "-LMqlospC7dgfbb9DzYP", "2018-10-20");

            Assert.Equal(new InternalServerErrorResult().StatusCode,(result.Result as InternalServerErrorResult).StatusCode);
        }

        [Fact]
        public async Task ShouldReturn_OkResult_WhenServiceWorksFine()
        {
            var positiveResult = new List<ReservationInfoBase>().AsEnumerable();

            var fireDataService = new Mock<IFirebaseDataService>();
            fireDataService.Setup(m => m.GetReservationsForDay(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
            )).Returns(Task.FromResult(positiveResult));

            var controller = new ReservationController(fireDataService.Object);

            var result = await controller.Get("Октябрь", "-LMqlospC7dgfbb9DzYP", "2018-10-20");

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task ShouldReturn_BadRequestResult_WhenModelInValid()
        {
            var fireDataService = new Mock<IFirebaseDataService>();

            var controller = new ReservationController(fireDataService.Object);

            var result = await controller.Get("Октябрь", "-LMqlospC7dgfbb9DzYP", "20018-10-20");

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void ShouldParseMonthNameIntoDateWithoutErrors()
        {
            var monthName = "Октябрь";

            var parseResult = DateTime.TryParseExact(
                monthName,
                "MMMM",
                CultureInfo.CreateSpecificCulture("ru"), DateTimeStyles.None,
                out var resultDateTime);

            Assert.True(parseResult);
        }

        [Fact]
        public void ShouldFailed_WhenParsedMonthName_Is_Invalid()
        {
            const string monthName = "Октяб";

            bool parseResult = DateTime.TryParseExact(
                monthName,
                "MMMM",
                CultureInfo.CreateSpecificCulture("ru"), DateTimeStyles.None,
                out var resultDateTime);
            
            Assert.False(parseResult);
        }

        [Theory]
        [InlineData("Январь")]
        [InlineData("Февраль")]
        [InlineData("Март")]
        [InlineData("Апрель")]
        [InlineData("Май")]
        [InlineData("Июнь")]
        [InlineData("Июль")]
        [InlineData("Август")]
        [InlineData("Сентябрь")]
        [InlineData("Октябрь")]
        [InlineData("Ноябрь")]
        [InlineData("Декабрь")]

        public void NegativeResult_WhenMonthName_Is_Invalid(string monthName)
        {
            var validationResult =
                ReservationController.ValidateParamsForReservationInfo(monthName, "123", "2018-12-12");

            Assert.True(validationResult);
        }

        [Fact]
        public void NegativeResult_WhenMonthId_Is_Invalid()
        {
            var validationResult =
                ReservationController.ValidateParamsForReservationInfo("Октябрь", "", "2018-12-12");

            Assert.False(validationResult);
        }

        [Fact]
        public void NegativeResult_WhenDate_Is_Invalid()
        {
            var validationResult =
                ReservationController.ValidateParamsForReservationInfo("Октябрь", "-anyID", "20018-12-12");

            Assert.False(validationResult);
        }
    }

}
