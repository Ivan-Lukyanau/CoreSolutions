using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CoreFireAPI.Models
{
    public class DaySchedule
    {
        private IEnumerable<Timeslot> _timeslots;

        public string Day { get; set; }

        public IEnumerable<Timeslot> Timeslots {
            get { return _timeslots; }
            set => _timeslots = value;
        }

        public DaySchedule(string day) : this()
        {
            Day = day;
        }
        public DaySchedule()
        {
            this.InitWithStandardTimeslots();
        }
        public void InitWithStandardTimeslots()
        {
            _timeslots = this.GetStandardTimeslots();
        }
        private IEnumerable<Timeslot> GetStandardTimeslots()
        {
            // from 8 a.m. till 19 p.m. we have a working hours 
            // and its equals to 12 values to arrange
            foreach (var i in Enumerable.Range(8, 12))
            {
                yield return new Timeslot(i, true);
            }
        }
    }
}
