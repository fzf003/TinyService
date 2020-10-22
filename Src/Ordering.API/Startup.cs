using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.Application.Mapper;
using Ordering.Application.Query;
using Ordering.Domain.Repository;
using Ordering.Infrastructure;
using Ordering.Infrastructure.DataContext;
using TinyService.Cqrs;
using TinyService.Discovery.Consul;

namespace Ordering.API
{
    public class Startup
    {
        const string ConnectionStr = "server=.,14330;Initial Catalog=OrderDb;User ID=sa;Password=!fzf123456;MultipleActiveResultSets=true";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            services.AddLogging();
            services.AddControllers();
            services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
            services.AddDiscovery();
            services.AddAutoMapper(typeof(OrderMappingProfile));
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddDbContext<OrderContext>(c=>c.UseSqlServer(ConnectionStr, x=> {x.EnableRetryOnFailure(3);}))
                    .AddDispatcher()
                    .AddEventHandlers(assemblies)
                    .AddCommandHandlers(assemblies)
                    .AddQueryHandlers(assemblies);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDiscovery();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

             

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/ping", async  context => {
                    await context.Response.WriteAsync("pong!!!");
                });

                endpoints.MapGet("/", async (c) => {

                    var orderContext=c.RequestServices.GetService<OrderContext>();
                    var loggerfactory = c.RequestServices.GetService<ILoggerFactory>();
                      
                     await OrderContextSeed.SeedAsync(orderContext, loggerfactory, 3);
                     await c.Response.WriteAsync(Guid.NewGuid().ToString("N"));
                });
            });
        }
    }
}
