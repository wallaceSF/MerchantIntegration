using System;
using System.ComponentModel;
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
            return base.Create(customer);
        }

        public Customer find(int id)
        {
            return new Customer()
            {
                Id = ObjectId.GenerateNewId(),
                Code = "qr56q1rw56r",
                Name = "Fulano",
                GatewayCustomerId = "cus_6wtw7tw76wt"
            };
        }
    }
}