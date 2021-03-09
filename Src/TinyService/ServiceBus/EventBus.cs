using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TinyService.Core;
using TinyService.Logging;

namespace TinyService.ServiceBus
{
    public class EventBus : EventBus<object>
    {
        public static readonly EventBus Instance = new EventBus();

        private readonly ILogger _logger = Log.CreateLogger<EventBus>();

        internal EventBus()
        {
            Subscribe(msg =>
            {
                if (msg is DeadLetterEvent letter)
                {
                    _logger.LogInformation("[DeadLetter] '{0}' got '{1}:{2}' from '{3}'", letter.Pid.ToShortString(),
                         letter.Message.GetType().Name, letter.Message, letter.Sender?.ToShortString());
                }
            });
        }
    }
    public class EventBus<T>
    {
        private readonly ILogger _logger = Log.CreateLogger<EventBus<T>>();

        private readonly ConcurrentDictionary<Guid, Subscription<T>> _subscriptions =
            new ConcurrentDictionary<Guid, Subscription<T>>();

        internal EventBus()
        {
        }

        public Subscription<T> Subscribe(Action<T> action)
        {
            var sub = new Subscription<T>(this, x =>
            {
                action(x);
                return Task.CompletedTask;
            });
            _subscriptions.TryAdd(sub.Id, sub);
            return sub;
        }

        public Subscription<T> Subscribe(Func<T, Task> action)
        {
            var sub = new Subscription<T>(this, action);
            _subscriptions.TryAdd(sub.Id, sub);
            return sub;
        }

        public Subscription<T> Subscribe<TMsg>(Action<TMsg> action) where TMsg : T
        {
            var sub = new Subscription<T>(this, msg =>
            {
                if (msg is TMsg typed)
                {
                    action(typed);
                }
                return Task.CompletedTask;
            });

            _subscriptions.TryAdd(sub.Id, sub);
            return sub;
        }

        public void Publish(T msg)
        {

            foreach (var sub in _subscriptions.Values)
            {
                Task.Factory.StartNew(() =>
                {

                    try
                    {
                        sub.Action(msg);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        // _logger.LogWarning(0, ex, "Exception has occurred when publishing a message.");
                    }

                });


                //return Task.CompletedTask;

            }
        }

        public void Unsubscribe(Guid id)
        {
            _subscriptions.TryRemove(id, out var _);
        }
    }

    public class Subscription<T>
    {
        private readonly EventBus<T> _eventStream;

        public Subscription(EventBus<T> eventStream, Func<T, Task> action)
        {
            Id = Guid.NewGuid();
            _eventStream = eventStream;
            Action = action;
        }

        public Guid Id { get; }

        public Func<T, Task> Action { get; }

        public void Unsubscribe()
        {
            _eventStream.Unsubscribe(Id);
        }
    }
}