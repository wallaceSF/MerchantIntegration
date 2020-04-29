using System;
using System.Net;
using AutoMapper;
using MerchantIntegration.Core.Contracts.Infrastruture.Service;
using Newtonsoft.Json;
using RestSharp;
using Customer = MerchantIntegration.Core.Entity.Customer;
using CustomerModelMundipagg = MerchantIntegration.Infra.Gateway.Mundipagg.Model.Customer;

namespace MerchantIntegration.Infra.Gateway.Mundipagg.Service
{
    public class CustomerService : IGatewayCustomerService
    {
        private readonly IRestClient _restClient;
        private readonly ILogInfo _logInfo;
        private IRestRequest RestRequest { get; }
        private IConfigurationProvider ConfigurationProvider { get; }

        private const string ENDPOINT = "customers";

        public CustomerService(
            IRestRequest restRequest,
            IRestClient restClient,
            IConfigurationProvider configurationProvider,
            ILogInfo logInfo
        ) {
            _restClient = restClient;
            _logInfo = logInfo;
            this.RestRequest = restRequest;
            this.ConfigurationProvider = configurationProvider;

            this.RestRequest.AddUrlSegment("endpoint", ENDPOINT);
        }

        public Customer CreateCustomerAtGateway(Customer customer)
        {
            var mapper = this.ConfigurationProvider.CreateMapper();
            
            var customerGateway = mapper.Map<Customer, CustomerModelMundipagg>(customer);

            this.RestRequest.Method = Method.POST;
            this.RestRequest.AddJsonBody(customerGateway);

            var response = this._restClient.Execute(this.RestRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                this._logInfo.InfoMessage(response.Content);
                throw new Exception("Não foi possível criar customer no gateway");
            }

            var customerInfra = JsonConvert.DeserializeObject<CustomerModelMundipagg>(response.Content);

            return mapper.Map<CustomerModelMundipagg, Customer>(customerInfra);
        }
    }
}