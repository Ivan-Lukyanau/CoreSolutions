using System;
using System.Collections.Generic;
using System.Globalization;
using static System.Diagnostics.Debug;
using System.Web.Http;
using System.Threading.Tasks;
using CoreFireAPI.BLL;
using CoreFireAPI.Models.Client;
using CoreFireAPI.Models.Reservation;
using CoreFireAPI.Models.Time;
using Microsoft.AspNetCore.Mvc;

namespace CoreFireAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IFirebaseDataService _firebaseDataService;

        public ReservationController(IFirebaseDataService firebaseDataService)
        {
            _firebaseDataService = firebaseDataService;
        }

        [HttpPost]
        public async Task Post([FromBody] TimeslotReservationDTO reservation)
        {
            await _firebaseDataService.MakeReservation(reservation);
        }

        [HttpGet("{monthName}/{monthId}/{date}")]
        public async Task<ActionResult<IEnumerable<ReservationInfoBase>>> Get(
            string monthName,
            string monthId,
            string date)
        {
            try
            {
                if (ValidateParamsForReservationInfo(monthName, monthId, date) == false)
                {
                    WriteLine($"Model is not valid: monthName = {monthName}, monthId = {monthId}, date = {date}");
                    return BadRequest("You've passed an incorrect request.");
                }
                return Ok(await _firebaseDataService.GetReservationsForDay(monthName, monthId, date));
            }
            catch (Exception e)
            {
                WriteLine(e);
                return new InternalServerErrorResult();
            }
        }

        [HttpPut]
        public async Task UpdateTimeAvailabilityForDay([FromBody] UpdateTimeslotAvailability model)
        {
            await _firebaseDataService.UpdateTimeAvailabilityForDay(
                model.MonthName, model.MonthId, model.Day, model.Time, model.Availability);
        }

        public static bool ValidateParamsForReservationInfo(string monthName, string monthId, string date)
        {
            if (monthName == null
                || !DateTime.TryParseExact(
                    monthName,
                    "MMMM",
                    CultureInfo.CreateSpecificCulture("ru"), DateTimeStyles.None,
                    out var resultMonthParse)
                || monthId == null
                || monthId.Length < 1
                || date == null 
                ||!DateTime.TryParse(date, out var resultDateParse)
            )
            {
                return false;
            }

            return true;
        }

    }
}
