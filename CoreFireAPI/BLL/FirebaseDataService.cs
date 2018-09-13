using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
            _firebase = new FirebaseClient(_connectionString);
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
            _firebase = new FirebaseClient(_connectionString);

            var insertionModel = monthScheduleBase.TransformIntoInsertObject();

            await _firebase.Child("schedule")
                .Child(monthScheduleBase.Name)
                .PostAsync<MonthScheduleInsert>(insertionModel);
        }

        public async Task<IEnumerable<string>> GetMonthSchedule()
        {
            _firebase = new FirebaseClient(_connectionString);
            var months = await _firebase.Child("schedule").OrderByKey().OnceAsync<object>();

            return (months.Select(e => e.Key));
        }

        public async Task<MonthScheduleRead> GetMonthSchedule(string monthName)
        {

            _firebase = new FirebaseClient(_connectionString);
            var result = await _firebase.Child("schedule")
                        .Child(monthName)
                        .OnceAsync<object>();

            if (result.Count < 1)
            {
                throw new Exception("The entry was not found.");
            }
            
            return this.GetReadModel(result);

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
            _firebase = new FirebaseClient(_connectionString);

            await _firebase.Child("schedule")
                .Child(monthName)
                .Child(id)
                .Child("Days")
                .Child("2018-07-09")
                .PatchAsync("{\"19:00\": \"false\"}");
        }
        public async Task BookTime(BookTimeRequest req)
        {
            _firebase = new FirebaseClient(_connectionString);

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

    }
}

