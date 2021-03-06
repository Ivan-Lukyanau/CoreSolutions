﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoreFireAPI.BLL;
using CoreFireAPI.Controllers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CoreFireAPI.Models
{
    public class NextMonthSchedule
    {
        private IEnumerable<DaySchedule> _daysAndTimes;
        private FirebaseDataService _fireDataService;

        public IEnumerable<DaySchedule> DaysAndTimes => _daysAndTimes;

        public string[] WorkingDays { get; private set; }

        public void SetupWorkingDays(string[] workingDays)
        {
            WorkingDays = workingDays;
            _daysAndTimes = new List<DaySchedule>();
        }

        public NextMonthSchedule(FirebaseDataService fireDataService)
        {
            _fireDataService = fireDataService;
        }

        public void AddStandardTimeslotsToEveryWorkingDay()
        {
            if (WorkingDays == null)
            {
                throw new ArgumentNullException();
            }

            var listResult = new List<DaySchedule>();

            foreach (var day in WorkingDays)
            {
                listResult.Add(new DaySchedule(day));
            }

            _daysAndTimes = listResult;
        }



        public async Task SaveIntoFireStorage()
        {
            if (_fireDataService == null) // to fix tests only
            {
                _fireDataService = new FirebaseDataService("https://firecoretest.firebaseio.com/");
            }
            await _fireDataService.SendIntoFireDatabase(this.DaysAndTimes);
        }
    }
}
