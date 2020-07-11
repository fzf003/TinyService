using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyService.Cqrs
{
    public interface IQuery
    {
    }

    public interface IQuery<T> : IQuery
    {
    }

    public interface IQueryHandler<in TQuery, TResult> where TQuery : class, IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }


   




}
