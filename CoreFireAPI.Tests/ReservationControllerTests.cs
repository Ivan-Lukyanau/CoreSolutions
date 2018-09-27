using System;
using System.Threading.Tasks;
using System.Web.Http;
using CoreFireAPI.BLL;
using CoreFireAPI.Controllers;
using Microsoft.AspNetCore.Http;
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
    }
}
