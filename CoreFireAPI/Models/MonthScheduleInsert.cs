using System.Collections.Generic;

namespace CoreFireAPI.Models
{
    public class MonthScheduleInsert
    {
        public string Name { get; set; }
        public int? MonthNumber { get; set; }
        public Dictionary<
            string,
            Dictionary<string,bool>
        > Days { get; set; }

        public string MonthRaw { get; set; }
        public bool Published { get; set; } = true;
    }
}
