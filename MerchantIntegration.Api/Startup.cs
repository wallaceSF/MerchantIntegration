using System;
using System.Diagnostics;
using System.IO;
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
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;


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
            //configure snakecase request/response
            services.AddControllers().AddJsonOptions(
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy =
                        SnakeCaseNamingPolicy.Instance;
                });

            //automapper
            var mapperConfiguration = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<CustomerModelInfra, Customer>()
                        .ForMember(
                            dest => dest.GatewayCustomerId,
                            origin => origin.MapFrom(customer => customer.Id)
                        )
                        .ForMember(
                            dest => dest.DocumentUser,
                            origin => origin.MapFrom(customer => customer.document)
                        )
                        .ForMember(
                            dest => dest.Id,
                            origin => origin.Ignore()
                        );

                    cfg.CreateMap<Customer, CustomerModelInfra>()
                        .ForMember(
                            dest => dest.type,
                            origin => origin.MapFrom(_ => "individual")
                        )
                        .ForMember(
                        dest => dest.document,
                        origin => origin.MapFrom(customer => customer.DocumentUser)
                        );
                }
            );

            //configuration mongo and gateway
            var settings = new ConnectionMongoSettings();
            Configuration.GetSection("ConnectionMongoSettings").Bind(settings);

            var gatewayConfig = new GatewayConfig();
            Configuration.GetSection("GatewayConfig").Bind(gatewayConfig);
            
            var seqConfig = new SeqConfig();
            Configuration.GetSection("SeqConfig").Bind(seqConfig);

            if (String.IsNullOrEmpty(gatewayConfig.SecretKey))
            {
                gatewayConfig.SecretKey = Environment.GetEnvironmentVariable("AppMerch_SecretKey");
            }
            
            if (String.IsNullOrEmpty(gatewayConfig.Url))
            {
                gatewayConfig.Url = Environment.GetEnvironmentVariable("AppMerch_GatewayUrl");
            }

            Console.WriteLine(settings.ConnectionString);
            Console.WriteLine(gatewayConfig.Url);

            // connection mongo and create request    
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

            //service core
            services.AddScoped<ICustomerService>(container =>
            {
                var gatewayService = (IGatewayCustomerService) container.GetService(typeof(IGatewayCustomerService));
                var customerRepository = (ICustomerRepository) container.GetService(typeof(ICustomerRepository));
                var log = (ILogInfo) container.GetService(typeof(ILogInfo));

                return new CustomerService(gatewayService, customerRepository, log);
            });

            //services infra
            services.AddSingleton<IGatewayCustomerService, CustomerServiceInfra>(container =>
            {
                var restRequest = (IRestRequest) container.GetService(typeof(IRestRequest));
                var restClient = (IRestClient) container.GetService(typeof(IRestClient));

                return new CustomerServiceInfra(restRequest, restClient, mapperConfiguration);
            });

            //repositories
            services.AddSingleton<ICustomerRepository, CustomerRepository>(container =>
            {
                var connectionMongo = (IMongoDatabase) container.GetService(typeof(IMongoDatabase));
                return new CustomerRepository(connectionMongo);
            });

            // seq log
            services.AddSingleton<ILogInfo, LogInfo>(_ =>
            {
                // @todo remove url, add json.config
                var seq = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.Seq(seqConfig.Url)
                    .CreateLogger();

                return new LogInfo(seq);
            });

            //heathCheck
            var serviceProvider = services.BuildServiceProvider();
            var mongoService = serviceProvider.GetService<IMongoDatabase>();

            services.AddHealthChecks().AddCheck(
                "Mongo-check",
                new MongoHeathCheck(mongoService),
                HealthStatus.Unhealthy
            );

            //doc swagger
            services.AddSwaggerGen(container =>
            {
                container.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"});
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHealthChecks("/hc");
                }
            );

            app.UseHealthChecks("/health",
                new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });


            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}