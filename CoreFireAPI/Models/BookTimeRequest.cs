using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CoreFireAPI.Models
{
    public class BookTimeRequest
    {
        public string Id { get; set; }
        public string Date { get; set; }
        //public string Time { get; set; }
        //public bool Value { get; set; }
        public Dictionary<string,bool> Timeslots { get; set; }

        public string GetMonthName()
        {
            if (DateTime.TryParse(this.Date, out var resultParse))
            {
                return resultParse.ToString("MMMM", CultureInfo.CreateSpecificCulture("ru"));
            }
            else
            {
                throw new Exception("Incorrect date format to parse.");
            }
        }
    }
}
