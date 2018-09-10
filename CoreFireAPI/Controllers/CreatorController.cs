using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreFireAPI.BLL;
using CoreFireAPI.DAL;
using CoreFireAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CoreFireAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreatorController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly FirebaseDataService _firebaseDataService;

        public CreatorController(
            ICustomerRepository customerRespository,
            FirebaseDataService firebaseDataService
            ) {
            _customerRepo = customerRespository;
            _firebaseDataService = firebaseDataService;
        }

        // GET: api/Creator
        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
            return await _customerRepo.GetCustomers();
        }

        // GET: api/Creator/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<Customer> Get(string id)
        {
            return await _customerRepo.GetSingleCustomer(id);
        }

        // POST: api/Creator
        [HttpPost]
        public async void Post([FromBody] Schedule workingDays)
        {
            var scheduleForNextMonth = new NextMonthSchedule(_firebaseDataService);
            scheduleForNextMonth.SetupWorkingDays(workingDays.WorkingDays);
            scheduleForNextMonth.AddStandardTimeslotsToEveryWorkingDay();
            await scheduleForNextMonth.SaveIntoFireStorage();
        }

        public class Schedule
        {
            public string[] WorkingDays { get; set; }
        }

        // PUT: api/Creator/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
