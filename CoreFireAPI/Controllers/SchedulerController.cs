using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreFireAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerController : ControllerBase
    {
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
        
    }
}
