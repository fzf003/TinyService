using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyService.Cqrs.Query.Dispatchers
{
    public interface IQueryDispatcher
    {
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
        Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : class, IQuery<TResult>;
    }


    internal sealed class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceScopeFactory _serviceFactory;

        public QueryDispatcher(IServiceScopeFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
        {
            using (var scope = _serviceFactory.CreateScope())
            {
                var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
                dynamic handler = scope.ServiceProvider.GetRequiredService(handlerType);
                return await handler.HandleAsync((dynamic)query);
            }
        }

        public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : class, IQuery<TResult>
        {
            using (var scope = _serviceFactory.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
                return await handler.HandleAsync(query);
            }
        }
    }
}
