using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.API.Models;
using Ordering.Application.Mapper;
using Ordering.Application.Query;
using Ordering.Domain.Repository;
using Ordering.Infrastructure;
using Ordering.Infrastructure.DataContext;
using TinyService.Cqrs;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

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
            services.AddControllers(options =>
            {
                options.Filters.Add(new HttpResponseExceptionFilter());

            }).AddXmlSerializerFormatters();
            //services.AddDistributedMemoryCache();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "127.0.0.1:6379,allowAdmin=true,defaultdatabase=0";
                options.InstanceName = "SampleInstance";
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false; //这里要改为false，默认是true，true的时候session无效
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(1*60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
            services.AddHttpContextAccessor();
             
           // services.AddMediatR(assemblies);
            services.AddAutoMapper(typeof(OrderMappingProfile));
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddDbContext<OrderContext>(c=>c.UseSqlServer(ConnectionStr, x=> {x.EnableRetryOnFailure(3);}))
                    .AddDispatcher()
                    .AddEventHandlers(assemblies)
                    .AddCommandHandlers(assemblies)
                    .AddQueryHandlers(assemblies);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.API", Version = "v1" });
            });

            services.AddHealthChecksUI()
                   .AddInMemoryStorage();

            services.AddHealthChecks()
                    .AddConsul(o =>
                    {
                        o.HostName = "localhost";
                        o.Port = 8500;
                    }).AddRedis("127.0.0.1:6379")
                      .AddSqlServer(this.Configuration.GetConnectionString("OrderDb"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderSercice v1"));

            app.UseHealthChecks("/Health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksUI();


            
            app.UseHttpsRedirection();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.UseHttpContextItemsMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/ping", async  context => {
                    await context.Response.WriteAsync("pong!!!");
                });

                endpoints.Map("/ui",  context => {

                    context.Response.Redirect("/HealthChecks-UI");
                    return Task.CompletedTask;
                });

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello from order One");
                });

                endpoints.MapGet("/InitDb", async (c) => {

                    var orderContext=c.RequestServices.GetService<OrderContext>();
                    var loggerfactory = c.RequestServices.GetService<ILoggerFactory>();
                      
                     await OrderContextSeed.SeedAsync(orderContext, loggerfactory, 3);
                     await c.Response.WriteAsync(Guid.NewGuid().ToString("N"));
                });
            });
        }
    }
}
