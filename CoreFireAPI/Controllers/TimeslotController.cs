using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreFireAPI.BLL;
using Microsoft.AspNetCore.Mvc;

namespace CoreFireAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeslotController : ControllerBase
    {
        private readonly FirebaseDataService _firebaseDataService;

        public TimeslotController(FirebaseDataService firebaseDataService)
        {
            _firebaseDataService = firebaseDataService;
        }
        [HttpGet("{monthName}/{monthId}/{date}")]
        public async Task<Dictionary<string, bool>> Get(string monthName, string monthId, string date)
        {
            return await _firebaseDataService.GetTimelotsForDay(monthName, monthId, date);
        }

        //// GET: api/Timeslot/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Timeslot
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/Timeslot/5
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
