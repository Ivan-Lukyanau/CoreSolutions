using System;
using System.Threading.Tasks;
using CoreFireConsole.Models;
using Firebase.Database;
using Firebase.Database.Query;

namespace CoreFireConsole
{
    class Program
    {
        //private static void InsertAnyDino(FirebaseClient client)
        //{
        //    client.Child("dinosaurs").PostAsync<Dinosaur>(new Dinosaur("Rex", 1.25))
        //}

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Program.Run().Wait();
            Configuration.GetConnectionString("RazorPagesMovieContext")
            //var firebase = new FirebaseClient(“https://dinosaur-facts.firebaseio.com/");
            //var dinos = await firebase
            //   .Child(“dinosaurs”)
            //   .OrderByKey()
            //   .StartAt(“pterodactyl”)
            //   .LimitToFirst(2)
            //   .OnceAsync<Dinosaur>();
            //foreach (var dino in dinos)
            //{
            //    Console.WriteLine($”{ dino.Key} is { dino.Object.Height}
            //    m high.”);
            //}

            Console.ReadKey();
        }

        private async static Task Run()
        {
            var connectionLink = "https://firecoretest.firebaseio.com/";
            var firebase = new FirebaseClient(connectionLink);
            await firebase.Child("dinosaurs").PostAsync<Dinosaur>(new Dinosaur("Rex", 1.25));

            var dinos = await firebase.Child("dinosaurs")
                              .OrderByKey()
                              //.StartAt(“pterodactyl”)
                              //.LimitToFirst(2)
                              .OnceAsync<Dinosaur>();

            foreach (var dino in dinos)
            {
                Console.WriteLine($"{dino.Key} is {dino.Object.Height}m high.");
            }
        }
    }
}
