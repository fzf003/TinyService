using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ordering.API.Models;
using Ordering.API.Services;

namespace Ordering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [FormatFilter]
    public class TestController : ControllerBase
    {

        readonly ILogger<TestController> _logger;

        readonly UserTokenService _userTokenService;

        public TestController(ILogger<TestController> logger, UserTokenService userTokenService)
        {
            _logger = logger;
            _userTokenService = userTokenService;
        }

        [HttpPost("gettoken")]
        public   UserToken GetToken(string username,string role)
        {
            
            return new UserToken {  Token = this._userTokenService.GenUserToken(username, role) };
        }

        [HttpPost("UpPets/{id:int}.{format?}")]
       // [Route("UpPets/{id}")]
       // [ProducesResponseType((int)HttpStatusCode.OK)]
       // [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpPets([Bind("Name")] Pets cancelOrder,int? id)
        {
            await Task.Delay(100);
            _logger.LogInformation($"Session:{ HttpContext.Session.Id}--fzf003:{HttpContext.Session.GetString("fzf003")}");
         
            if (!ModelState.IsValid)
            {
                _logger.LogInformation($"Success:{!ModelState.IsValid}");

                return this.NotFound(new {Error="出错了，不是你想的那个样子！" });
            }
            else
            {
                _logger.LogInformation($"id:{id}");
                //throw new Exception("my error!!!");
            }

            return this.Ok(cancelOrder);
        }

    }
}
