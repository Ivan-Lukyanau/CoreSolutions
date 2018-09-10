using System;
using System.Globalization;

namespace CoreFireAPI.Models
{
    public class MonthSchedule
    {
        public string Name
        {
            get
            {
                var output = "Undefined";

                if (this.Days != null)
                {
                    //var dateResultParse = DateTime.Today;
                    if (DateTime.TryParse(this.Days[0].Day, out DateTime dateResultParse))
                    {
                        output = dateResultParse.ToString("MMM", CultureInfo.InvariantCulture);
                    }
                }

                return output;
            }
        }

        public DaySchedule[] Days { get; set; }
    }
}
