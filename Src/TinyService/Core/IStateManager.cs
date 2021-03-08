using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TinyService.Core
{
    public interface IStateManager
    {
        
        Task AddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default);
         
        Task<T> GetStateAsync<T>(string stateName, CancellationToken cancellationToken = default);
 
        Task SetStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default);

   
        Task RemoveStateAsync(string stateName, CancellationToken cancellationToken = default);

        
        Task<bool> TryAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default);

        Task<T> TryGetStateAsync<T>(string stateName, CancellationToken cancellationToken = default);

        
        Task<bool> TryRemoveStateAsync(string stateName, CancellationToken cancellationToken = default);

       
        Task<bool> ContainsStateAsync(string stateName, CancellationToken cancellationToken = default);

        
        Task<T> GetOrAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default);

       
        Task<T> AddOrUpdateStateAsync<T>(string stateName, T addValue, Func<string, T, T> updateValueFactory, CancellationToken cancellationToken = default);

        
        Task ClearCacheAsync(CancellationToken cancellationToken = default);

       
        Task SaveStateAsync(CancellationToken cancellationToken = default);
    }
}
