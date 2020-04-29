using System.Collections.Generic;
using MerchantIntegration.Core.Contracts.Domain.Service;
using MerchantIntegration.Core.Entity;
using Microsoft.AspNetCore.Mvc;

namespace MerchantIntegration.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        public ICustomerService CoreService { get; }

        public CustomerController(ICustomerService coreService)
        {
            this.CoreService = coreService;
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var customer = this.CoreService.Find(id);

            if (customer == null)
            {
                return NotFound(new {not_found = id});
            }

            return Ok(customer);
        }

        [HttpGet]
        public List<Customer> GetAll()
        {
            return this.CoreService.FindAll();
        }

        [HttpPost]
        public IActionResult Create([FromBody] Customer customer)
        {
            var customerObject = this.CoreService.Create(customer);
            return Created(customerObject.Id, customerObject);
        }
    }
}