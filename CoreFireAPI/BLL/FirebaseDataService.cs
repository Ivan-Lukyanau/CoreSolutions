﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreFireAPI.Controllers;
using CoreFireAPI.Models;
using CoreFireAPI.Models.Client;
using CoreFireAPI.Models.Reservation;
using CoreFireAPI.Models.Time;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoreFireAPI.BLL
{
    public interface IFirebaseDataService
    {
        Task<IEnumerable<ReservationInfoBase>> GetReservationsForDay(string monthName, string monthId, string date);
        Task MakeReservation(TimeslotReservationDTO reservation);
        Task UpdateTimeAvailabilityForDay(
            string monthName,
            string monthId,
            string day,
            string time,
            bool availability);
    }

    public class FirebaseDataService : IFirebaseDataService
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

        public async Task<IEnumerable<ReservationInfoBase>> GetReservationsForDay(string monthName, string monthId, string date)
        {
            IReadOnlyCollection<FirebaseObject<object>> result = await _firebase.Child("reservation")
                .Child(monthName)
                .Child(monthId)
                .Child("Days")
                .Child(date)
                .OnceAsync<object>();

            if (result == null)
            {
                throw new Exception("Result is not defined!");
            }
            var resultSet = new List<ReservationInfoBase>();

            foreach (var item in result)
            {
                var clientAndTime = JsonConvert
                    .DeserializeObject<Dictionary<string, ClientInfoBase>>(item.Object.ToString()).FirstOrDefault();
                resultSet.Add(new ReservationInfoBase()
                {
                    Id = item.Key,
                    Time = clientAndTime.Key,
                    ClientName = clientAndTime.Value.ClientName,
                    ClientPhone = clientAndTime.Value.ClientPhone
                });
            }

            return resultSet;
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
                var item = m.Properties().First();
                return new WorkingMonth()
                {
                    Id = item.Name,
                    Name = e.Key,
                    MonthRaw = item.Value.SelectToken("MonthRaw").Value<string>(),
                    Published = item.Value.SelectToken("Published")?.Value<bool>() ?? true
                };
            });

            return this.SortMonth(monthsResult);
        }

        private IOrderedEnumerable<WorkingMonth> SortMonth(IEnumerable<WorkingMonth> months)
        {
            return months.OrderBy(s => DateTime.ParseExact(s.Name, "MMMM", new CultureInfo("ru")));
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

        public async Task MakeReservation(TimeslotReservationDTO reservation)
        {
            dynamic client = new System.Dynamic.ExpandoObject();
            client.clientName = reservation.ClientName;
            client.clientPhone = reservation.ClientPhone;
            var postObject = new Dictionary<string, dynamic> {{reservation.Time, client}};
            var dataString = JsonConvert.SerializeObject(postObject);

            await _firebase.Child("reservation")
                .Child(reservation.MonthName)
                .Child(reservation.MonthId)
                .Child("Days")
                .Child(reservation.Day)
                .PostAsync(dataString);
        }

        // TODO : Test it and change the logic of creating json serialized object through the dynamic as above
        public async Task BookTime(BookTimeRequest req)
        {
            var builder = new StringBuilder();

            foreach (var slot in req.Timeslots)
            {
                builder.Append($"\"{slot.Time}\": \"{slot.Available}\"");
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
        public async Task UpdateWDForMonth(DaysInMonthUpdate daysInMonthUpdate)
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

            // update Published valued for Month schedule
            await _firebase.Child("schedule")
                .Child(daysInMonthUpdate.Name)
                .Child(daysInMonthUpdate.Id)
                .Child("Published")
                .PutAsync(daysInMonthUpdate.Published);

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

        public async Task UpdateTimeAvailabilityForDay(
            string monthName,
            string monthId,
            string day,
            string time,
            bool availability)
        {
            //var dayInsert = new Dictionary<string, bool>
            //{
            //    { time, availability }
            //};

            //var dayInsert = $"\"{time}\": \"{availability}\"";
            //var toPatch = "{ " + dayInsert + " }";

            await _firebase.Child("schedule")
                .Child(monthName)
                .Child(monthId)
                .Child("Days")
                .Child(day)
                .Child(time)
                .PutAsync(availability);
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
                    try
                    {
                        timeslotDict.Add(item.Key, Boolean.Parse(item.Object.ToString()));
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return timeslotDict;
        }
    }
}

