using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreFireAPI.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreFireAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonthController : ControllerBase
    {
        private readonly FirebaseDataService _firebaseDataService;

        public MonthController(FirebaseDataService firebaseDataService)
        {
            _firebaseDataService = firebaseDataService;
        }

        [HttpDelete("{monthName}/{monthId}")]
        public async Task Delete(string monthName, string monthId)
        {
            await _firebaseDataService.DeleteMonth(monthName, monthId);
        }
    }
}
