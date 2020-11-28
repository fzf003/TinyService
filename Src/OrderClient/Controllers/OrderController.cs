using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrderClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
 
        private readonly ILogger<OrderController> _logger;

        readonly OrderClientService swaggerClient;

        public OrderController(ILogger<OrderController> logger, OrderClientService swaggerClient)
        {

             _logger = logger;

            this.swaggerClient = swaggerClient;
        }

        [HttpGet]
        public Task<ICollection<OrderDto>> Get()
        {
            return swaggerClient.QueryOrderAllAsync();
        }
    }
}
