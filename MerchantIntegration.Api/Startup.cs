using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MerchantIntegration.Api.Contracts;
using MerchantIntegration.Api.Model;
using MerchantIntegration.Core;
using MerchantIntegration.Core.Contracts;
using MerchantIntegration.Core.Contracts.Infrastruture;
using MerchantIntegration.Core.Contracts.Domain.Service;
using MerchantIntegration.Core.Contracts.Infrastruture.Repository;
using MerchantIntegration.Core.Contracts.Infrastruture.Service;
using MerchantIntegration.Infra.Gateway;
using MerchantIntegration.Infra.Gateway.Mundipagg;
using MerchantIntegration.Infra.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CustomerService = MerchantIntegration.Core.CustomerService;
using CustomerServiceInfra = MerchantIntegration.Infra.Gateway.Mundipagg.Service.CustomerService;

//using MerchantIntegration.Infra.Gateway.Mundipagg.CustomerService;

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


            var settings = new ConnectionMongoSettings();
            Configuration.GetSection("ConnectionMongoSettings").Bind(settings);

//            services.AddSingleton<ConnectionMongoSettings>(sp =>
//                sp.GetRequiredService<IOptions<ConnectionMongoSettings>>().Value
//            );

            //  services.AddScoped<IConnectionMongoSettings, ConnectionMongoSettings>();
            
            

            services.AddSingleton<IMongoConnection>(p =>
            {
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);

                return database;
            });
            
            services.AddSingleton<IGatewayCustomerService, CustomerServiceInfra>();
            services.AddSingleton<ICustomerRepository, CustomerRepository>();

            //services.AddScoped<IConnectionMongoSettings, ConnectionMongoSettings>();
            services.AddScoped<ICustomerService>(sp =>
            {
                var gatewayService = (IGatewayCustomerService) sp.GetService(typeof(IGatewayCustomerService));
                var customerRepository = (ICustomerRepository) sp.GetService(typeof(ICustomerRepository));
                return new CustomerService(gatewayService, customerRepository);
            });

            //  services.AddSingleton<CustomerService>();

//            services.AddScoped<IConnectionMongoSettings, ConnectionMongoSettings>();
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