using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Ordering.API.Models
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
            if (context.Exception !=null)
            {
                context.Result = new ObjectResult(context.Exception)
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                     Value=context.Exception.Message
                };
                context.ExceptionHandled = true;
            }
        }
    }


    public class HttpContextItemsMiddleware
    {
        readonly RequestDelegate _next;

        ILogger<HttpContextItemsMiddleware> _logger;

        public HttpContextItemsMiddleware(RequestDelegate next, ILogger<HttpContextItemsMiddleware> logger)
        {
            this._next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("Item 请求中间件..............");
            context.Session.SetString("fzf003", "f0092");
            context.User = new System.Security.Claims.ClaimsPrincipal()
            {
                
            };

            await this._next(context);
        }
    }

    public static class HttpContextItemsMiddlewareExtensions
    {
        public static IApplicationBuilder
            UseHttpContextItemsMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HttpContextItemsMiddleware>();
        }
    }
}
