using System.Collections.Generic;
using MerchantIntegration.Core.Contracts.Infrastruture.Repository;
using MerchantIntegration.Core.Entity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MerchantIntegration.Infra.Repository
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }

        public new Customer Create(Customer customer)
        {
            customer.Id = ObjectId.GenerateNewId();
            
            var customerObject = base.Create(customer);
            customerObject.Id = customerObject.Id.ToString();
            
            return customerObject;
        }

        public Customer Find(string id)
        {
            var filter = Builders<Customer>.Filter.Eq(customerObject => customerObject.Id, ObjectId.Parse(id));
            var customer = base.Find(filter);

            if (customer == null)
            {
                return null;
            }
            
            customer.Id = customer.Id.ToString();
            return customer;
        }

        public List<Customer> FindAll()
        {
            var listCustomer =  base.FindAll();

            foreach (var customer in listCustomer)
            {
                if (customer.Id == null)
                {
                    continue;
                }
                
                customer.Id = customer.Id.ToString();
            }
            
            return listCustomer;
        }
        
    }
}