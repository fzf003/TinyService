using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;

        private readonly IQueryDispatcher _queryDispatcher;

        readonly ICommandDispatcher _commandDispatcher;


        public OrderController(ILogger<OrderController> logger, IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;

            _queryDispatcher = queryDispatcher;

            _commandDispatcher = commandDispatcher;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<List<OrderDto>> Get()
        {
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
        [Route("QueryOrder/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<OrderDto> QueryDetail([FromRoute]long id)
        {
            var response = await _queryDispatcher.QueryAsync(new QueryOrderById(id)).ConfigureAwait(false);

            return response;
        }



    }
}
