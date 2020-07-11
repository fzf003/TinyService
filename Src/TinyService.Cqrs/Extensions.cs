using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TinyService.Cqrs.Commands;
using TinyService.Cqrs.Events;
using TinyService.Cqrs.Commands.Dispatchers;
using TinyService.Cqrs.Events.Dispatchers;
using TinyService.Cqrs.Query.Dispatchers;

namespace TinyService.Cqrs
{
    public  static  class Extensions
    {
        public static IServiceCollection AddCommandHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.Scan(s =>
                   s.FromAssemblies(assemblies)
                    .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());
                 
             return services;
        }

        public static IServiceCollection AddEventHandlers(this IServiceCollection services,params Assembly[] assemblies) 
        {
             
            services.Scan(s =>
                s.FromAssemblies(assemblies)
                   .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
                   .AsImplementedInterfaces()
                   .WithTransientLifetime());
            return services;
        }

        public static IServiceCollection AddQueryHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.Scan(s =>
                  s.FromAssemblies(assemblies)
                    .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());
            return services;
        }



        public static IServiceCollection AddDispatcher(this IServiceCollection services)
        {
            services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
            services.AddSingleton<IEventDispatcher, EventDispatcher>();
            services.AddSingleton<IQueryDispatcher, QueryDispatcher>();
            return services;
        }
    }
}
