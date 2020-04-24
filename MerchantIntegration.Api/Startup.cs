using System;
using AutoMapper;
using MerchantIntegration.Api.Contracts;
using MerchantIntegration.Api.Model;
using MerchantIntegration.Core.Contracts.Domain.Service;
using MerchantIntegration.Core.Contracts.Infrastruture.Repository;
using MerchantIntegration.Core.Contracts.Infrastruture.Service;
using MerchantIntegration.Core.Entity;
using MerchantIntegration.Infra.Repository;
using MerchantIntegration.Infra.SeedWork;
using MerchantIntegration.Infra.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using RestSharp;
using Serilog;
using CustomerService = MerchantIntegration.Core.CustomerService;
using CustomerServiceInfra = MerchantIntegration.Infra.Gateway.Mundipagg.Service.CustomerService;
using CustomerModelInfra = MerchantIntegration.Infra.Gateway.Mundipagg.Model.Customer;
using HealthChecks.UI;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;


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
            services.AddControllers().AddJsonOptions(
                options => {
                    options.JsonSerializerOptions.PropertyNamingPolicy = 
                        SnakeCaseNamingPolicy.Instance;
                });

            var mapperConfiguration = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<CustomerModelInfra, Customer>()
                        .ForMember(
                            dest => dest.GatewayCustomerId,
                            origin => origin.MapFrom(customer => customer.Id)
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


            services.AddSingleton(_ =>
            {
                var client = new MongoClient(settings.ConnectionString);
                return client.GetDatabase(settings.DatabaseName);
            });

            services.AddSingleton<IRestRequest>(_ =>
            {
                var request = new RestRequest();

                request.AddHeader("Content-Type", "application/json");

                var secretKey = System.Text.Encoding.UTF8.GetBytes($"{gatewayConfig.SecretKey}:");
                var base64String = Convert.ToBase64String(secretKey);

                request.AddHeader("Authorization", $"Basic {base64String}");

                return request;
            });

            services.AddSingleton<IRestClient, RestClient>(_ => new RestClient(gatewayConfig.Url + "/{endpoint}"));

            services.AddSingleton<IGatewayCustomerService, CustomerServiceInfra>(container =>
            {
                var restRequest = (IRestRequest) container.GetService(typeof(IRestRequest));
                var restClient = (IRestClient) container.GetService(typeof(IRestClient));
                return new CustomerServiceInfra(restRequest, restClient, mapperConfiguration);
            });

            services.AddSingleton<ICustomerRepository, CustomerRepository>(container =>
            {
                var connectionMongo = (IMongoDatabase) container.GetService(typeof(IMongoDatabase));
                return new CustomerRepository(connectionMongo);
            });
            
            services.AddSingleton<ILogInfo, LogInfo>(_ =>
            {
                var seq = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.Seq("http://seqteste:80")
                    .CreateLogger();

                return new LogInfo(seq);
            });

            services.AddScoped<ICustomerService>(container =>
            {
                var gatewayService = (IGatewayCustomerService) container.GetService(typeof(IGatewayCustomerService));
                var customerRepository = (ICustomerRepository) container.GetService(typeof(ICustomerRepository));
                var log = (ILogInfo) container.GetService(typeof(ILogInfo));
                return new CustomerService(gatewayService, customerRepository, log);
            });
            
            var serviceProvider = services.BuildServiceProvider();
            var myService = serviceProvider.GetService<IMongoDatabase>();


            services.AddHealthChecks().AddCheck(
                "Mongo-check",
                new MongoHeathCheck(myService),
                HealthStatus.Unhealthy
            );
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
            
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHealthChecks("/hc");
                }
            );
            
            
            
            app.UseHealthChecks("/healthz",
                new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}