using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TinyService.Core
{
    public class StateManager : IStateManager
    {
       
        //readonly JsonSerializerOptions _serializerOptions;
        private readonly IDictionary<string, StateMetadata> stateChangeTracker;

        public StateManager()
        {
            this.stateChangeTracker = new ConcurrentDictionary<string, StateMetadata>();

           /* _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };*/
        }

        public async Task<T> AddOrUpdateStateAsync<T>(string stateName, T addValue, Func<string, T, T> updateValueFactory, CancellationToken cancellationToken = default)
        {

            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                var stateMetadata = this.stateChangeTracker[stateName];

                if (stateMetadata.ChangeAction == StateChangeAction.Remove)
                {
                    this.stateChangeTracker[stateName] = StateMetadata.Create(addValue, StateChangeAction.Update);
                    return addValue;
                }

                var newValue = updateValueFactory.Invoke(stateName, (T)stateMetadata.Value);
                stateMetadata.Value = newValue;

                if (stateMetadata.ChangeAction == StateChangeAction.None)
                {
                    stateMetadata.ChangeAction = StateChangeAction.Update;
                }

                return newValue;
            }

            var conditionalResult =await this.TryGetStateAsync<T>(stateName, cancellationToken);
            if (conditionalResult!=null)
            {
                var newValue = updateValueFactory(stateName, conditionalResult);
                this.stateChangeTracker.Add(stateName, StateMetadata.Create(newValue, StateChangeAction.Update));

                return newValue;
            }

            this.stateChangeTracker[stateName] = StateMetadata.Create(addValue, StateChangeAction.Add);
            return addValue;
        }

        public Task AddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default)
        {
            this.stateChangeTracker.Add(stateName, StateMetadata.Create<T>(value,StateChangeAction.Add));
            return Task.CompletedTask;
        }

        public Task ClearCacheAsync(CancellationToken cancellationToken = default)
        {
            this.stateChangeTracker.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> ContainsStateAsync(string stateName, CancellationToken cancellationToken = default)
        {
            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                var stateMetadata = this.stateChangeTracker[stateName];

                 return Task.FromResult(stateMetadata.ChangeAction != StateChangeAction.Remove);
            }

            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public async Task<T> GetOrAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default)
        {
            var condRes = await this.ContainsStateAsync(stateName);

            if (condRes)
            {
                return await this.GetStateAsync<T>(stateName, cancellationToken);
            }
 
            this.stateChangeTracker[stateName] = StateMetadata.Create(value, StateChangeAction.Add);
            return value;
        }

        public  Task<T> GetStateAsync<T>(string stateName, CancellationToken cancellationToken = default)
        {
            return this.TryGetStateAsync<T>(stateName, cancellationToken);
        }

        public Task RemoveStateAsync(string stateName, CancellationToken cancellationToken = default)
        {
            return this.TryRemoveStateAsync(stateName, cancellationToken);
        }

        public Task SaveStateAsync(CancellationToken cancellationToken = default)
        {

            //Console.WriteLine("保存:{0}",this.stateChangeTracker.Count);
            foreach(var item in this.stateChangeTracker)
            {
               // Console.WriteLine("保存:{0}--{1}",Thread.CurrentThread.ManagedThreadId,item.Key+"--"+item.Value.Type+"--"+JsonConvert.SerializeObject(item.Value.Value) +"---"+item.Value.ChangeAction);
                //this.stateChangeTracker.Remove(item.Key);
               // Console.WriteLine($"开始保存{item.Value.Value}.....");
                Console.WriteLine("保存:" + item.Key + "--" + item.Value.Type + "--" + JsonConvert.SerializeObject(item.Value.Value) + "---" + item.Value.ChangeAction);
                this.stateChangeTracker.Remove(item.Key);
                //Console.WriteLine($"开始结束{item.Value.Value}.....");

            }

            return Task.CompletedTask;
        }

        public  Task SetStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default)
        {
            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                var stateMetadata = this.stateChangeTracker[stateName];
                stateMetadata.Value = value;
                stateMetadata.ChangeAction = StateChangeAction.Update;
                
            }
            else if (!this.stateChangeTracker.ContainsKey(stateName))
            {
                this.stateChangeTracker.Add(stateName, StateMetadata.Create(value, StateChangeAction.Add));
            }
            else
            {
                this.stateChangeTracker[stateName] = StateMetadata.Create(value, StateChangeAction.Update);
            }

            return Task.CompletedTask;
        }

        public Task<bool> TryAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default)
        {
            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                var stateMetadata = this.stateChangeTracker[stateName];

                if (stateMetadata.ChangeAction == StateChangeAction.Remove)
                {
                    this.stateChangeTracker[stateName] = StateMetadata.Create(value, StateChangeAction.Update);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }

            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                return Task.FromResult(false);
            }

            this.stateChangeTracker[stateName] = StateMetadata.Create(value, StateChangeAction.Add);
            return Task.FromResult(true);
        }

        public Task<T> TryGetStateAsync<T>(string stateName, CancellationToken cancellationToken = default)
        {
            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                var stateMetadata = this.stateChangeTracker[stateName];
                
                return Task.FromResult((T)stateMetadata.Value);
            }

            return default;
        }

        public Task<bool> TryRemoveStateAsync(string stateName, CancellationToken cancellationToken = default)
        {
            if (this.stateChangeTracker.ContainsKey(stateName))
            {
                var stateMetadata = this.stateChangeTracker[stateName];

                switch (stateMetadata.ChangeAction)
                {
                    case StateChangeAction.Remove:
                        return Task.FromResult(false);
                    case StateChangeAction.Add:
                        this.stateChangeTracker.Remove(stateName);
                        return Task.FromResult(true);
                }

                stateMetadata.ChangeAction = StateChangeAction.Remove;
                return Task.FromResult(true);
            }

            if (!this.stateChangeTracker.ContainsKey(stateName))
            {
                this.stateChangeTracker.Add(stateName, StateMetadata.CreateForRemove());
                return Task.FromResult(true);
            }

            return Task.FromResult(true);

        }
    }
}
