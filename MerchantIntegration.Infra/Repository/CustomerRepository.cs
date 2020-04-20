using System.Collections.Generic;
using System.Linq;
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
           // customer.Id = new ObjectId().ToString();
            return base.Create(customer);
        }

        public Customer Find(string id)
        {
            var filter = Builders<Customer>.Filter.Eq(x => x.Id, ObjectId.Parse(id));
            var customer = base.Find(filter);
            customer.Id = customer.Id.ToString();
            return customer;
        }

        public List<Customer> FindAll()
        {
            var listCustomer =  base.FindAll();

            foreach (var customer in listCustomer)
            {
                customer.Id = customer.Id.ToString();
            }
            
            return listCustomer;
        }
        
    }
}