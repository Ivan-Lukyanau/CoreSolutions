using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public CreatorController(ICustomerRepository customerRespository)
        {
            _customerRepo = customerRespository;
        }

        // GET: api/Creator
        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
            return await _customerRepo.GetCustomers();
            //return new string[] { "value1", "value2" };
        }

        // GET: api/Creator/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<Customer> Get(string id)
        {
            return await _customerRepo.GetSingleCustomer(id);
        }

        // POST: api/Creator
        [HttpPost]
        public async void Post([FromBody] Customer customer)
        {
            await _customerRepo.InsertCustomer(customer);
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
