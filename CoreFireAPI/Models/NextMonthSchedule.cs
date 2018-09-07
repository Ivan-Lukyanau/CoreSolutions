using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoreFireAPI.Controllers;

namespace CoreFireAPI.Models
{
    public class NextMonthSchedule
    {
        private IEnumerable<WorkingSchedule> _daysAndTimes;
        //{
        //    get => new List<WorkingSchedule>();
        //    set
        //    {
        //        if (value != null)
        //        {
        //            _daysAndTimes = value;
        //        }
        //    }
        //}

        public IEnumerable<WorkingSchedule> DaysAndTimes => _daysAndTimes;

        public string[] WorkingDays { get; private set; }

        public NextMonthSchedule(string[] workingDays)
        {
            WorkingDays = workingDays;
            _daysAndTimes = new List<WorkingSchedule>();
        }


        public void AddStandardTimeslotsToEveryWorkingDay()
        {
            if (WorkingDays == null)
            {
                throw new ArgumentNullException();
            }

            var listResult = new List<WorkingSchedule>();
            foreach (var day in WorkingDays)
            {
                listResult.Add(new WorkingSchedule()
                {
                    Day = day,
                    TimeSlots = Enumerable.Range(8, 12).ToArray()
            });
            }

            _daysAndTimes = listResult;
        }
    }
}
