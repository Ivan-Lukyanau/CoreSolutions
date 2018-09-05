using System.Collections.Generic;
using System.Threading.Tasks;
using CoreFireAPI.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CoreFireAPI.DAL
{
    public class CustomerRespository : ICustomerRepository
    {
        private readonly string _connectionString;
        private FirebaseClient _firebase;

        //public CustomerRespository(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //    _connectionString = Configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        //    _firebase = new FirebaseClient(_connectionString);

        //}
        public CustomerRespository(IOptions<FireConnection> fireConnection)
        {
            _connectionString = fireConnection.Value.DefaultConnection;
            _firebase = new FirebaseClient(_connectionString);

        }

        public async Task<Customer> GetSingleCustomer(string customerId)
        {
            var customer = await _firebase.Child("customers").Child(customerId).OnceSingleAsync<object>();

            return JsonConvert.DeserializeObject<Customer>(customer.ToString());
        }

        public async Task<List<Customer>> GetCustomers()
        {
            var customers = await _firebase.Child("customers").OnceAsync<object>();
            var listResult = new List<Customer>();

            foreach (var item in customers)
            {
                Customer main = JsonConvert.DeserializeObject<Customer>(item.Object.ToString());
                main.Id = item.Key;
                listResult.Add(main);
            }
            return listResult;
        }

        public async Task InsertCustomer(Customer newCustomer)
        {
            await _firebase.Child("customers").PostAsync<Customer>(newCustomer);

            //int rowsAffected = this._db.Execute(@"INSERT Customer([CustomerFirstName],[CustomerLastName],[IsActive]) values (@CustomerFirstName, @CustomerLastName, @IsActive)", new { CustomerFirstName = ourCustomer.CustomerFirstName, CustomerLastName = ourCustomer.CustomerLastName, IsActive = true });
            //if (rowsAffected > 0)
            //{
            //    return true;
            //}
            //return false;

        }

        //public bool DeleteCustomer(int customerId)
        //{
        //    int rowsAffected = this._db.Execute(@"DELETE FROM [Customer] WHERE CustomerID = @CustomerID", new { CustomerID = customerId });

        //    if (rowsAffected > 0)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //public bool UpdateCustomer(Customer ourCustomer)
        //{
        //    int rowsAffected = this._db.Execute("UPDATE [Customer] SET [CustomerFirstName] = @CustomerFirstName ,[CustomerLastName] = @CustomerLastName, [IsActive] = @IsActive WHERE CustomerID = " + ourCustomer.CustomerID, ourCustomer);

        //    if (rowsAffected > 0)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }
}
