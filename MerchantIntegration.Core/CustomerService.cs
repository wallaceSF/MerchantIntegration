using System.Collections.Generic;
using MerchantIntegration.Core.Contracts.Domain.Service;
using MerchantIntegration.Core.Contracts.Infrastruture.Repository;
using MerchantIntegration.Core.Contracts.Infrastruture.Service;
using MerchantIntegration.Core.Entity;

namespace MerchantIntegration.Core
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogInfo _logInfo;
        private IGatewayCustomerService GatewayService { get; }

        public CustomerService(IGatewayCustomerService gatewayService, ICustomerRepository customerRepository, ILogInfo logInfo)
        {
            _customerRepository = customerRepository;
            _logInfo = logInfo;
            GatewayService = gatewayService;
        }

        public Customer Find(string id)
        {
            return _customerRepository.Find(id);
        }
        
        public Customer Create(Customer customer)
        {
          //  var z = new LogInfo();
         //   z.InfoMessage<Customer>("teste", customer);
         
         _logInfo.InfoMessage<Customer>("erro aqui", customer);
            
            var customerCreatedAtGateway = GatewayService.CreateCustomerAtGateway(customer);
            return _customerRepository.Create(customerCreatedAtGateway);
        }

        public List<Customer> FindAll()
        {
            return _customerRepository.FindAll();
        }
    }
}