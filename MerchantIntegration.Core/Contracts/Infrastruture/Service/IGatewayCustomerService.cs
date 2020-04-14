using MerchantIntegration.Core.Entity;

namespace MerchantIntegration.Core.Contracts.Infrastruture.Service
{
    public interface IGatewayCustomerService
    {
        string gt();

        Customer CreateCustomerAtGateway(Customer customer);
    }
}