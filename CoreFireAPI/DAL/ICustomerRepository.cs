using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreFireAPI.Models;

namespace CoreFireAPI.DAL
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetCustomers();

        Task<Customer> GetSingleCustomer(string customerId);

        Task InsertCustomer(Customer ourCustomer);

        //bool DeleteCustomer(int customerId);

        //bool UpdateCustomer(Customer ourCustomer);
    }
}
