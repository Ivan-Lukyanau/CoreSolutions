using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CoreFireAPI.Controllers;
using CoreFireAPI.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace CoreFireAPI.BLL
{
    public class FirebaseDataService
    {
        private string _connectionString;
        private FirebaseClient _firebase;

        public FirebaseDataService(IOptions<FireConnection> fireConnection)
        {
            _connectionString = fireConnection.Value.DefaultConnection;
            _firebase = new FirebaseClient(_connectionString);
        }

        public FirebaseDataService(string fireConnection)
        {
            _connectionString = fireConnection;

        }

        // reinvent given the auth strategy in the future
        public async Task SendIntoFireDatabase(NextMonthSchedule schedule, string authToken)
        {
            if (authToken == null)
            {
                _firebase = new FirebaseClient(_connectionString);
            }
            else
            {
                _firebase = new FirebaseClient(
                    _connectionString,
                    new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = async () => await Task.Run(() => authToken)
                    }
                );
            }

            await _firebase.Child("schedule")
                .PostAsync<NextMonthSchedule>(schedule);
        }

        // deprecated
        public async Task SendIntoFireDatabase(IEnumerable<DaySchedule> monthSchedule)
        {
            var monthScheduleSave = new MonthScheduleBase { Days = monthSchedule };
            // just for Keys test
            var insertionModel = monthScheduleSave.TransformIntoInsertObject();

            await _firebase.Child("schedule")
                .PostAsync<MonthScheduleInsert>(insertionModel);
        }



        // just for test only
        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task SendIntoFireDatabase(MonthScheduleBase monthScheduleBase)
        {
            var insertionModel = monthScheduleBase.TransformIntoInsertObject();
            var isAlreadyExists = await _firebase.Child("schedule")
                .Child(insertionModel.Name).OnceAsync<object>();
            if (isAlreadyExists.Count > 0)
            {
                throw new InvalidOperationException("Month is already exists. Just modify it.");
            }
            await _firebase.Child("schedule")
                .Child(monthScheduleBase.Name)
                .PostAsync<MonthScheduleInsert>(insertionModel);
        }

        public async Task<IEnumerable<WorkingMonth>> GetMonthSchedule()
        {
            var months = await _firebase.Child("schedule").OrderByKey().OnceAsync<object>();

            var monthsResult = months.Select(e =>
            {
                var m = JObject.Parse(e.Object.ToString());
                return new WorkingMonth()
                {
                    Id = m.Properties().First().Name,
                    Name = e.Key
                    //MonthNumber = m.Property("MonthNumber")?.Value.ToString() ?? "Unavailable"};
                };
            });

            return this.SortMonth(monthsResult);
        }

        private IOrderedEnumerable<WorkingMonth> SortMonth(IEnumerable<WorkingMonth> months)
        {
            return months.OrderBy(s => DateTime.ParseExact(s.Name, "MMMM", new CultureInfo("ru")));
        }

        public class WorkingMonth
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
        public async Task<MonthScheduleRead> GetMonthSchedule(string monthName)
        {
            var result = await _firebase.Child("schedule")
                        .Child(monthName)
                        .OnceAsync<object>();

            if (result.Count < 1)
            {
                throw new Exception("The entry was not found.");
            }
            
            return this.GetReadModel(result);

        }

        public async Task<IEnumerable<string>> GetWorkingDaysInMonth(string monthName)
        {
            var result = await _firebase.Child("schedule")
                .Child(monthName)
                .OnceAsync<object>();

            return (result as IEnumerable<string>);
        }

        public async Task<IEnumerable<string>> GetWorkingDaysInMonthByKey(string monthName, string monthId)
        {
            var result = await _firebase.Child("schedule")
                .Child(monthName)
                .Child(monthId)
                .Child("Days")
                .OnceAsync<object>();
            
            return result.Select(_ => _.Key);
        }

        private MonthScheduleRead GetReadModel(IReadOnlyCollection<FirebaseObject<object>> model)
        {
            var firstEntry = model.First();
            var monthSchedule = JsonConvert.DeserializeObject<MonthScheduleRead>(firstEntry.Object.ToString());
            monthSchedule.Id = firstEntry.Key;
            return monthSchedule;
        }

        public async Task BookTime(string id, string monthName, int time)
        {
            await _firebase.Child("schedule")
                .Child(monthName)
                .Child(id)
                .Child("Days")
                .Child("2018-07-09")
                .PatchAsync("{\"19:00\": \"false\"}");
        }
        public async Task BookTime(BookTimeRequest req)
        {
            var builder = new StringBuilder();
            foreach (var slot in req.Timeslots)
            {
                builder.Append($"\"{slot.Key}\": \"{slot.Value}\"");
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1);

            //var toPatch = "{ "+ builder.ToString().TrimEnd(',') + " }";
            var toPatch = "{ " + builder + " }";

            await _firebase.Child("schedule")
                .Child(req.GetMonthName())
                .Child(req.Id)
                .Child("Days")
                .Child(req.Date)
                .PatchAsync(toPatch);
            //.PatchAsync($"{{ {segment}, \"16:00\": \"False\", \"15:00\": \"False\" }}");
        }
        // todo make a refactoring the method logic works but could be shrinked a bit
        public async Task UpdateWDForMonth(SchedulerController.DaysInMonthUpdate daysInMonthUpdate)
        {
            var days = new Dictionary<string, Dictionary<string, bool>>();
            
            var daysAvailable = await _firebase.Child("schedule")
                .Child(daysInMonthUpdate.Name)
                .Child(daysInMonthUpdate.Id)
                .Child("Days")
                .OnceAsync<object>();

            foreach (var dayAvailable in daysAvailable)
            {
                var timetable = JsonConvert.DeserializeObject<Dictionary<string, bool>>(dayAvailable.Object.ToString());
                days.Add(dayAvailable.Key, timetable);
            }

            // if we face date that doesnt exists in collection
            // we create new new DaySchedule model and add it into target before 
            var daysToAdd = new Dictionary<string, Dictionary<string, bool>>();

            foreach (var day in daysInMonthUpdate.WorkingDays)
            {
                if (!days.ContainsKey(day))
                {
                    // TODO something with initialization standart time slots
                    // maybe we need inherited class from Dictionary to make it easier
                    var timeslots = new DaySchedule(day).Timeslots;
                    var timeslotDict = new Dictionary<string, bool>();
                    timeslots.ToList().ForEach(e =>
                    {
                        var elTime = TimeSpan.FromHours(e.Time).ToString(@"hh\:mm");
                        timeslotDict.Add(elTime, e.Available);
                    });
                    daysToAdd.Add(day, timeslotDict);
                }
            }
            var daysToStay = new Dictionary<string, Dictionary<string, bool>>();
            foreach (var day in days)
            {
                if (daysInMonthUpdate.WorkingDays.Contains(day.Key))
                {
                    daysToStay.Add(day.Key, day.Value);
                }
            }
            // merge arrays
            foreach (var dayToAdd in daysToAdd)
            {
                daysToStay.Add(dayToAdd.Key, dayToAdd.Value);
            }

            await _firebase.Child("schedule")
                .Child(daysInMonthUpdate.Name)
                .Child(daysInMonthUpdate.Id)
                .Child("Days")
                .PutAsync(daysToStay);

            //daysInMonthUpdate.WorkingDays.ToList().ForEach(el =>
            //{
            //    var timeslotDict = new Dictionary<string, bool>();

            //    el.Timeslots.ToList().ForEach(e =>
            //    {
            //        var elTime = TimeSpan.FromHours(e.Time).ToString(@"hh\:mm");
            //        timeslotDict.Add(elTime, e.Available);
            //    });
            //    days.Add(el.Day, timeslotDict);
            //});
        }


        public async Task DeleteMonth(string monthName, string monthId)
        {
            await _firebase.Child("schedule")
                .Child(monthName)
                .Child(monthId)
                .DeleteAsync();
        }

        public async Task<Dictionary<string, bool>> GetTimelotsForDay(string monthName, string monthId, string date)
        {
            IReadOnlyCollection<FirebaseObject<object>> result = await _firebase.Child("schedule")
                        .Child(monthName)
                        .Child(monthId)
                        .Child("Days")
                        .Child(date)
                        .OnceAsync<object>();
            var timeslotDict = new Dictionary<string, bool>();
            if (result != null)
            {
                foreach (var item in result)
                {
                    timeslotDict.Add(item.Key, (bool)item.Object);
                }
            }

            return timeslotDict;
        }
    }
}

