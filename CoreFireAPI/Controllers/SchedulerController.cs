using System.Collections.Generic;
using System.Threading.Tasks;
using CoreFireAPI.BLL;
using CoreFireAPI.Extensions;
using CoreFireAPI.Models;
using CoreFireAPI.Models.Reservation;
using Microsoft.AspNetCore.Mvc;

namespace CoreFireAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerController : ControllerBase
    {
        private readonly FirebaseDataService _firebaseDataService;

        public SchedulerController(FirebaseDataService firebaseDataService)
        {
            _firebaseDataService = firebaseDataService;
        }

        [HttpPost]
        public async Task Post([FromBody] ScheduleCreate schedule)
        {
            // init month schedule
            var days = this.ExtractDaysScheduleArray(schedule.WorkingDays);
            var monthSchedule = new MonthScheduleBase {Days = days};

            // save the schedule into db
            await _firebaseDataService.SendIntoFireDatabase(monthSchedule);
        }

        /// <summary>
        /// Returns list of Month's we have in the schedule
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<WorkingMonth>> Get()
        {
            return await _firebaseDataService.GetMonthSchedule();
        }

        [HttpGet("{monthNumber}")]
        public async Task<MonthScheduleRead> Get(int monthNumber)
        {
            var monthName = monthNumber.GetMonthNameByMonthNumber();

            // getting data about month schedule
            return await _firebaseDataService.GetMonthSchedule(monthName);
        }

        [HttpGet("{monthName}/{monthId}")]
        public async Task<IEnumerable<string>> Get(string monthName, string monthId)
        {
            // getting data about month schedule
            return await _firebaseDataService.GetWorkingDaysInMonthByKey(monthName, monthId);
        }

        [HttpGet("{id}/{monthNumber}/{time}")]
        public async Task Get(string id, int monthNumber, int time)
        {
            var monthName = monthNumber.GetMonthNameByMonthNumber();

            // getting data about month schedule
            await _firebaseDataService.BookTime(id, monthName, time);
        }

        [HttpPatch]
        public async Task Patch(BookTimeRequest req)
        {
            await _firebaseDataService.BookTime(req);
        }

        [HttpPut]
        public async Task Put([FromBody] DaysInMonthUpdate updateModel)
        {

            await _firebaseDataService.UpdateWDForMonth(updateModel);
        }

        private IEnumerable<DaySchedule> ExtractDaysScheduleArray(string[] days)
        {
            foreach (var day in days)
            {
                yield return new DaySchedule(day);
            }
        }

    }
}
