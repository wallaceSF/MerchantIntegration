using System;
using AutoMapper;
using MerchantIntegration.Api.Model;
using MerchantIntegration.Core.Contracts.Domain.Service;
using MerchantIntegration.Core.Contracts.Infrastruture.Repository;
using MerchantIntegration.Core.Contracts.Infrastruture.Service;
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
using HealthChecks.Uris;
using MerchantIntegration.Infra.Gateway.Mundipagg.Mapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using static System.Net.Http.HttpMethod;


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
                cfg => { cfg.AddProfile(new CustomerMapper()); }
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
            Console.WriteLine(gatewayConfig.SecretKey);

            // connection mongo and create request    
            services.AddSingleton<IMongoDatabase>(_ =>
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

            services.AddSingleton<IRestClient>(_ => new RestClient(gatewayConfig.Url + "/{endpoint}"));
            
            // seq log
            services.AddSingleton<ILogInfo>(_ =>
            {
                var seq = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.Seq(seqConfig.Url)
                    .CreateLogger();

                return new LogInfo(seq);
            });

            //service core
            services.AddScoped<ICustomerService, CustomerService>();

            //services infra
            services.AddSingleton<IGatewayCustomerService, CustomerServiceInfra>(container =>
            {
                var restRequest = (IRestRequest) container.GetService(typeof(IRestRequest));
                var restClient = (IRestClient) container.GetService(typeof(IRestClient));
                var logInfo = (ILogInfo) container.GetService(typeof(ILogInfo));

                return new CustomerServiceInfra(restRequest, restClient, mapperConfiguration, logInfo);
            });

            //repositories
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            //heathCheck
            services
                .AddHealthChecks()
                .AddMongoDb(settings.ConnectionString, name: "MongoCheck", timeout: TimeSpan.FromSeconds(2))
                .AddUrlGroup(
                    options =>
                    {
                        options.AddUri(new Uri($"{gatewayConfig.Url}/customers"));
                        options.UseHttpMethod(Post);
                        options.ExpectHttpCodes(200, 401);
                    },
                    "MundipaggGatewayCheck",
                    timeout: TimeSpan.FromSeconds(2)
                );

            //doc swagger
            services.AddSwaggerGen(container =>
                {
                    container.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"});
                }
            );
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