using MerchantIntegration.Core.Contracts;
using MerchantIntegration.Core.Contracts.Infrastruture;
using MerchantIntegration.Core.Contracts.Domain.Service;
using MerchantIntegration.Core.Contracts.Infrastruture.Repository;
using MerchantIntegration.Core.Contracts.Infrastruture.Service;
using MerchantIntegration.Core.Entity;

namespace MerchantIntegration.Core
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        public IGatewayCustomerService GatewayService { get; }

        public CustomerService(IGatewayCustomerService gatewayService, ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            GatewayService = gatewayService;
        }

        public string getTeste()
        {
            var z = GatewayService.gt();
            return "funciona cara";
        }

        public Customer Find(int id)
        {
            return _customerRepository.find(id);
        }
        
        public Customer Create(Customer customer)
        {
            return _customerRepository.Create(customer);
        }
    }
}