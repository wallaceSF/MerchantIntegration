using MerchantIntegration.Core.Contracts;
using MerchantIntegration.Core.Contracts.Infrastruture;
using MerchantIntegration.Core.Contracts.Infrastruture.Service;
using MerchantIntegration.Core.Entity;

namespace MerchantIntegration.Infra.Gateway.Mundipagg.Service
{
    public class CustomerService : IGatewayCustomerService
    {

        public string gt()
        {
            return "aaaaa";
        }

        public Customer CreateCustomerAtGateway(Customer customer)
        {
            //conexão lá na mundipagg
            throw new System.NotImplementedException();
        }
    }
}