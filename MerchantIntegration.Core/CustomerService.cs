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

        public CustomerService(
            IGatewayCustomerService gatewayService,
            ICustomerRepository customerRepository,
            ILogInfo logInfo
        ) {
            this._customerRepository = customerRepository;
            this._logInfo = logInfo;
            this.GatewayService = gatewayService;
        }

        public Customer Find(string id)
        {
            return this._customerRepository.Find(id);
        }

        public Customer Create(Customer customer)
        {
            this._logInfo.InfoMessage(customer);

            var customerCreatedAtGateway = this.GatewayService.CreateCustomerAtGateway(customer);
            return this._customerRepository.Create(customerCreatedAtGateway);
        }

        public List<Customer> FindAll()
        {
            return this._customerRepository.FindAll();
        }
    }
}