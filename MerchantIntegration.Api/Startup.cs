using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MerchantIntegration.Api.Contracts;
using MerchantIntegration.Api.Model;
using MerchantIntegration.Core;
using MerchantIntegration.Core.Contracts;
using MerchantIntegration.Core.Contracts.Infrastruture;
using MerchantIntegration.Core.Contracts.Domain.Service;
using MerchantIntegration.Core.Contracts.Infrastruture.Repository;
using MerchantIntegration.Core.Contracts.Infrastruture.Service;
using MerchantIntegration.Core.Entity;
using MerchantIntegration.Infra.Gateway;
using MerchantIntegration.Infra.Gateway.Mundipagg;
using MerchantIntegration.Infra.Repository;
using MerchantIntegration.Infra.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RestSharp;
using Serilog;
using Serilog.Core;
using CustomerService = MerchantIntegration.Core.CustomerService;
using CustomerServiceInfra = MerchantIntegration.Infra.Gateway.Mundipagg.Service.CustomerService;
using CustomerModelInfra = MerchantIntegration.Infra.Gateway.Mundipagg.Model.Customer;

namespace MerchantIntegration.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var mapperConfiguration = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<CustomerModelInfra, Customer>()
                        .ForMember(
                            dest => dest.GatewayCustomerId,
                            origin => origin.MapFrom(a => a.Id)
                        )
                        .ForMember(
                            dest => dest.Id,
                            origin => origin.Ignore()
                        );

                    cfg.CreateMap<Customer, CustomerModelInfra>();
                }
            );

            var settings = new ConnectionMongoSettings();
            Configuration.GetSection("ConnectionMongoSettings").Bind(settings);

            var gatewayConfig = new GatewayConfig();
            Configuration.GetSection("GatewayConfig").Bind(gatewayConfig);


            services.AddSingleton<IMongoDatabase>(p =>
            {
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);

                return database;
            });

            services.AddSingleton<IRestRequest>(p =>
            {
                var request = new RestRequest();

                request.AddHeader("Content-Type", "application/json");

                var secretKey = System.Text.Encoding.UTF8.GetBytes($"{gatewayConfig.SecretKey}:");
                var base64String = Convert.ToBase64String(secretKey);

                request.AddHeader("Authorization", $"Basic {base64String}");

                return request;
            });

            services.AddSingleton<IRestClient, RestClient>(x => new RestClient(gatewayConfig.Url + "/{endpoint}"));

            services.AddSingleton<IGatewayCustomerService, CustomerServiceInfra>(x =>
            {
                var restRequest = (IRestRequest) x.GetService(typeof(IRestRequest));
                var restClient = (IRestClient) x.GetService(typeof(IRestClient));
                return new CustomerServiceInfra(restRequest, restClient, mapperConfiguration);
            });

            services.AddSingleton<ICustomerRepository, CustomerRepository>(x =>
            {
                var connectionMongo = (IMongoDatabase) x.GetService(typeof(IMongoDatabase));
                return new CustomerRepository(connectionMongo);
            });
            
            services.AddSingleton<ILogInfo, LogInfo>(x =>
            {
                Logger seq = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.Seq("http://localhost:655")
                    .CreateLogger();

                
                return new LogInfo(seq);
            });

            services.AddScoped<ICustomerService>(sp =>
            {
                var gatewayService = (IGatewayCustomerService) sp.GetService(typeof(IGatewayCustomerService));
                var customerRepository = (ICustomerRepository) sp.GetService(typeof(ICustomerRepository));
                var log = (ILogInfo) sp.GetService(typeof(ILogInfo));
                return new CustomerService(gatewayService, customerRepository, log);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //   app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}