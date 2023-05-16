using System.Collections.Generic;
using System.Threading.Tasks;
using CoreFireAPI.BLL;
using CoreFireAPI.Models.Time;
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
        public async Task<IEnumerable<Timeslot>> Get(string monthName, string monthId, string date)
        {
            var result = await _firebaseDataService.GetTimelotsForDay(monthName, monthId, date);
            return Timeslot.FromDictionary(result);
        }

    }
}
