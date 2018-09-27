using System;
using System.Collections.Generic;
using static System.Diagnostics.Debug;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Http;
using System.Threading.Tasks;
using CoreFireAPI.BLL;
using CoreFireAPI.Models.Client;
using CoreFireAPI.Models.Time;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;

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
            /*
             if (!ModelState.IsValid)
             {
                return BadRequest(ModelState);
             }
             
             */
            // ObjectResult
            await _firebaseDataService.MakeReservation(reservation);
        }

        [HttpGet("{monthName}/{monthId}/{date}")]
        public async Task<ActionResult<IEnumerable<ReservationInfoBase>>> Get(string monthName, string monthId, string date)
        {
            try
            {
                return Ok(await _firebaseDataService.GetReservationsForDay(monthName, monthId, date));
            }
            catch (Exception e)
            {
                WriteLine(e);
                return new InternalServerErrorResult();
            }
        }

    }

}
