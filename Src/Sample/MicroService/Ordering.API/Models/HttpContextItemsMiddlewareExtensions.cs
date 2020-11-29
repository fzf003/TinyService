using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Models
{
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
