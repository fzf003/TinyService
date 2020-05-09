using System;
using TinyService.Core;
using TinyService.Logging;
using TinyService.Schedule;
using TinyService.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace TinyService
{
    public static class TinyServiceExtensions
    {

        public static IServiceCollection UseTinyService(this IServiceCollection container, Action<ActorPropsRegistry> registerAction = null)
        {
            container.AddSingleton<ILoggerFactory, Log4NetLoggerFactory>();

            container.AddSingleton<ISerializationService, DefaultSerializationService>();

            container.AddSingleton<IScheduleService, TimerScheduleService>();
 
            container.AddSingleton<IActorFactory, ActorFactory>();

            var registry = new ActorPropsRegistry();

            registerAction?.Invoke(registry);

            container.AddSingleton(registry);

            return container;
        }
 
    }
}
