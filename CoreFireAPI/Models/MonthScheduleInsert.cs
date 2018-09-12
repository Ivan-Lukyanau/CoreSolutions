using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreFireAPI.Models
{
    public class MonthScheduleInsert
    {
        public string Name { get; set; }
        public Dictionary<
            string,
            Dictionary<string,bool>
        > Days { get; set; }
    }
}
