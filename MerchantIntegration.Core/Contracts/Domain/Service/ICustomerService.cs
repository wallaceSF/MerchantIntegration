using MerchantIntegration.Core.Entity;

namespace MerchantIntegration.Core.Contracts.Domain.Service
{
    public interface ICustomerService
    {
        string getTeste();

        Customer Find(int id);
    }
}