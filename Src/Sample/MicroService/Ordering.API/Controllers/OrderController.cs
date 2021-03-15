using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ordering.API.Services;
using Ordering.Application.Commands;
using Ordering.Application.Dto;
using Ordering.Application.Query;
using Ordering.Domain;
using TinyService.Cqrs.Commands.Dispatchers;
using TinyService.Cqrs.Query.Dispatchers;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Produces(contentType:"application/json"]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles ="admin")]

    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;

        private readonly IQueryDispatcher _queryDispatcher;

        readonly ICommandDispatcher _commandDispatcher;

        readonly ISecurityContextAccessor _securityContextAccessor;

        public OrderController(ILogger<OrderController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ISecurityContextAccessor securityContextAccessor)
        {
            _logger = logger;

            _queryDispatcher = queryDispatcher;

            _commandDispatcher = commandDispatcher;

            _securityContextAccessor = securityContextAccessor;
        }

       

        [HttpGet("TokenInfo")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Token()
        {
            return this.Ok(this._securityContextAccessor);
        }


        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<List<OrderDto>> Get()
        {
            _logger.LogInformation(this.User.Identity.Name);
            var response = await _queryDispatcher.QueryAsync(GetOrderAll.Instance);

            return response;
        }

        [HttpPost]
        [Route("CreateOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public Task CreateOrder([FromBody] CreateOrderCommand createOrder)
        {
             return _commandDispatcher.SendAsync(createOrder);
        }


        [HttpPut]
        [Route("CancelOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public Task CancelOrder([FromBody] CancelOrderCommand cancelOrder)
        {
            return _commandDispatcher.SendAsync(cancelOrder);
        }

        [HttpGet]
        [Route("QueryOrderAll")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<List<OrderDto>> QueryOrder()
        {
            
            var response = await _queryDispatcher.QueryAsync(GetOrderAll.Instance);

            return response;
        }

        [HttpGet]
        [Route("{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<OrderDto> QueryDetail([FromRoute]long id)
        {
            var response = await _queryDispatcher.QueryAsync(new QueryOrderById(id)).ConfigureAwait(false);

            return response;
        }



    }
}
