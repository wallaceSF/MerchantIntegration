using System.Collections.Generic;
using MerchantIntegration.Core.Entity;

namespace MerchantIntegration.Core.Contracts.Infrastruture.Repository
{
    public interface ICustomerRepository
    {
        Customer Create(Customer customer);
        Customer Find(string id);
        List<Customer> FindAll();
    }
}