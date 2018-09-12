using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CoreFireAPI.Models
{
    public class MonthScheduleBase
    {
        public string Name
        {
            get
            {
                var output = "Undefined";

                if (this.Days != null)
                {
                    if (DateTime.TryParse(this.Days.First().Day, out DateTime dateResultParse))
                    {
                        output = dateResultParse.ToString(
                            "MMMM",
                            CultureInfo.CreateSpecificCulture("ru"));
                    }
                }

                return output;
            }
        }

        public IEnumerable<DaySchedule> Days { get; set; }

        public MonthScheduleInsert TransformIntoInsertObject()
        {
            var days = new Dictionary<string, Dictionary<string, bool>>();
            this.Days.ToList().ForEach(el =>
            {
                var timeslotDict = new Dictionary<string, bool>();
                
                el.Timeslots.ToList().ForEach(e =>
                {
                    var elTime = TimeSpan.FromHours(e.Time).ToString(@"hh\:mm");
                    timeslotDict.Add(elTime, e.Available);
                });
                days.Add(el.Day, timeslotDict);
            });
            return new MonthScheduleInsert()
            {
                Name = this.Name,
                Days = days
            };
        }
    }
}
