using System.Collections.Generic;

namespace CoreFireAPI.Models.Reservation
{
    public class DaysInMonthUpdate : WorkingMonth
    {
        public IEnumerable<string> WorkingDays { get; set; }
    }
}
