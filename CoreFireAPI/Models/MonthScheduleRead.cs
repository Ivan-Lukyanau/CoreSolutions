using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreFireAPI.Models
{
    public class MonthScheduleRead
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<
            string,
            Dictionary<string, bool>
        > Days { get; set; }
    }
}
