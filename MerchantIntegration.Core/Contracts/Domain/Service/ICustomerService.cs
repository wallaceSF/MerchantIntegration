using System.Collections.Generic;
using MerchantIntegration.Core.Entity;

namespace MerchantIntegration.Core.Contracts.Domain.Service
{
    public interface ICustomerService
    {
        Customer Find(string id);
        Customer Create(Customer customer);
        List<Customer> FindAll();
    }
}