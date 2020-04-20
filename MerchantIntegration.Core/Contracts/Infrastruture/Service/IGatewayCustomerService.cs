using MerchantIntegration.Core.Entity;

namespace MerchantIntegration.Core.Contracts.Infrastruture.Service
{
    public interface IGatewayCustomerService
    {
        Customer CreateCustomerAtGateway(Customer customer);
    }
}