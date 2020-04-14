using MerchantIntegration.Core.Contracts.Infrastruture.Repository;
using MerchantIntegration.Core.Entity;

namespace MerchantIntegration.Infra.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        public Customer find(int id)
        {
            return new Customer()
            {
                Id = 1,
                Code = "qr56q1rw56r",
                Name = "Fulano",
                GatewayCustomerId = "cus_6wtw7tw76wt"
            };
        }
    }
}