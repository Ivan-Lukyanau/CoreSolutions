using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreFireAPI.Models
{
    public class WorkingSchedule
    {
        public string Day { get; set; }
        public int[] TimeSlots { get; set; }
    }
}
