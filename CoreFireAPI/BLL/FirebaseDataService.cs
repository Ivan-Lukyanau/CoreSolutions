using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreFireAPI.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Options;

namespace CoreFireAPI.BLL
{
    public class FirebaseDataService
    {
        private string _connectionString;
        private FirebaseClient _firebase;

        public FirebaseDataService(IOptions<FireConnection> fireConnection)
        {
            _connectionString = fireConnection.Value.DefaultConnection;

            // CREATE OPTIONS HERE TO 
            //_firebase = new FirebaseClient(_connectionString);

        }

        public FirebaseDataService(string fireConnection)
        {
            _connectionString = fireConnection;

        }

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

        public async Task SendIntoFireDatabase(IEnumerable<DaySchedule> monthSchedule)
        {
            _firebase = new FirebaseClient(_connectionString);

            var one = new DaySchedule();
            one.Day = "2018-09-10";
            one.Timeslots = new[]
            {
                new Timeslot(8, true),
                new Timeslot(9, true),
                new Timeslot(10, true)
            };
            var two = new DaySchedule();
            two.Day = "2018-09-10";
            two.Timeslots = new[]
            {
                new Timeslot(8, true),
                new Timeslot(9, true),
                new Timeslot(10, true)
            };
            var monthScheduleSave = new MonthSchedule { Days = new DaySchedule[] {one, two} };
            await _firebase.Child("schedule")
                .PostAsync<MonthSchedule>(monthScheduleSave);
        }

        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

    }
}
