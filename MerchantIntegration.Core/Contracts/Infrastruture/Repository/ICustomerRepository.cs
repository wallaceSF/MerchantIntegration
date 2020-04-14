using MerchantIntegration.Core.Entity;

namespace MerchantIntegration.Core.Contracts.Infrastruture.Repository
{
    public interface ICustomerRepository
    {
        Customer find(int id);
        Customer Create(Customer customer);
    }
}