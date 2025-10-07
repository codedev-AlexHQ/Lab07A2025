using System.Collections.Generic;
using Data;
using Entity;

namespace Business
{
    public class BCustomer
    {
        private DCustomer dataCustomer = new DCustomer();

        public List<Customer> Read() => dataCustomer.Read();
        public void Create(Customer customer) => dataCustomer.Create(customer);
        public void Update(Customer customer) => dataCustomer.Update(customer);
        public void Delete(int customerId) => dataCustomer.Delete(customerId);
        public List<Customer> Search(string searchTerm) => dataCustomer.Search(searchTerm);
    }
}