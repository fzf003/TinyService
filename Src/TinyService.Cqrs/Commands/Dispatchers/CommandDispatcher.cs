using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyService.Cqrs.Commands.Dispatchers
{
    public interface ICommandDispatcher
    {
        Task SendAsync<T>(T command) where T : class, ICommand;
    }


    internal sealed class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceScopeFactory _serviceFactory;

        public CommandDispatcher(IServiceScopeFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public async Task SendAsync<T>(T command) where T : class, ICommand
        {
            using (var scope = _serviceFactory.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<T>>();
                await handler.HandleAsync(command);
            }
        }
    }
}
