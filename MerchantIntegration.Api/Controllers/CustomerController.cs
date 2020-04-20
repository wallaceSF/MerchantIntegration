using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MerchantIntegration.Core;
using MerchantIntegration.Core.Contracts.Domain.Service;
using MerchantIntegration.Core.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MerchantIntegration.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        public ICustomerService CoreService { get; }

        public CustomerController(ICustomerService coreService)
        {
            CoreService = coreService;
        }

        [HttpGet("{id}")]
        public Customer Get(string id)
        {
            return CoreService.Find(id);
        }
        
        [HttpGet]
        public List<Customer> GetAll()
        {
            return CoreService.FindAll();
        }
        
        [HttpPost]
        public Customer Create([FromBody] Customer customer)
        {
            return CoreService.Create(customer);
        }
    }
}