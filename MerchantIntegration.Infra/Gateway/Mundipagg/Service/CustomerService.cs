using System;
using System.Net;
using AutoMapper;
using MerchantIntegration.Core.Contracts.Infrastruture.Service;
using MerchantIntegration.Infra.SeedWork;
using Newtonsoft.Json;
using RestSharp;
using Customer = MerchantIntegration.Core.Entity.Customer;
using CustomerModelMundipagg = MerchantIntegration.Infra.Gateway.Mundipagg.Model.Customer;

namespace MerchantIntegration.Infra.Gateway.Mundipagg.Service
{
    public class CustomerService : IGatewayCustomerService
    {
        private readonly IRestClient _restClient;
        private IRestRequest RestRequest { get; }
        private IConfigurationProvider ConfigurationProvider { get; }

        private const string Endpoint = "customers";

        public CustomerService(
            IRestRequest restRequest,
            IRestClient restClient,
            IConfigurationProvider configurationProvider
        ) {
            _restClient = restClient;
            RestRequest = restRequest;
            ConfigurationProvider = configurationProvider;

            RestRequest.AddUrlSegment("endpoint", Endpoint);
        }

        public Customer CreateCustomerAtGateway(Customer customer)
        {
            var mapper = ConfigurationProvider.CreateMapper();
            
            var customerGateway = mapper.Map<Customer, CustomerModelMundipagg>(customer);
            var objectGeneric = TransformRequestToGateway.TreatObject(customerGateway);

            RestRequest.Method = Method.POST;
            RestRequest.AddJsonBody(objectGeneric);

            var response = _restClient.Execute(RestRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("erro aqui");
            }

            var customerInfra = JsonConvert.DeserializeObject<CustomerModelMundipagg>(response.Content);

            return mapper.Map<CustomerModelMundipagg, Customer>(customerInfra);
        }
    }
}