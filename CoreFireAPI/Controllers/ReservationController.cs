using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreFireAPI.BLL;
using CoreFireAPI.Models.Time;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreFireAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly FirebaseDataService _firebaseDataService;

        public ReservationController(FirebaseDataService firebaseDataService)
        {
            _firebaseDataService = firebaseDataService;
        }

        [HttpPost]
        public async Task Post([FromBody] TimeslotReservationDTO reservation)
        {
            // ObjectResult
            await _firebaseDataService.MakeReservation(reservation);
        }

        [HttpGet("{monthName}/{monthId}/{date}")]
        public async Task<IEnumerable<Timeslot>> Get(string monthName, string monthId, string date)
        {
            var result = await _firebaseDataService.GetReservationsForDay(monthName, monthId, date);
            return Timeslot.FromDictionary(result);
        }

        //// GET: api/Reservation/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// PUT: api/Reservation/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
