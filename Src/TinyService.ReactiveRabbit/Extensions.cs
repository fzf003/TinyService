using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TinyService.ReactiveRabbit.Brocker;
using TinyService.ReactiveRabbit.RabbitFactory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

namespace TinyService.ReactiveRabbit
{
    public static class Extensions
    {
        public static IServiceCollection AddReactiveRabbit(this IServiceCollection services, Action<ConnectionSetting> settingaction)
        {
            services.AddLogging();

            services.AddSingleton<IChannelFactory>((provider) =>
            {
                var settings = new ConnectionSetting();
                settingaction(settings);
                return new ChannelFactory(settings);
            });

            services.AddSingleton<IMessageBroker, MessageBroker>();
 
            return services;
        }
    }
}
