using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;
using TinyService.Utils;
using System.Threading.Tasks;

namespace TinyService.ServiceBus
{
    public interface IServiceBus
    {
        System.Threading.Tasks.Task SendAsync<TMessage>(string ChannelName, TMessage message) where TMessage : class;
        Guid Subscribe<TMessage>(string ChannelName, Action<TMessage> handlerAction) where TMessage : class;
        Guid Subscribe<TMessage>(string ChannelName, Func<TMessage, System.Threading.Tasks.Task> handlerAction) where TMessage : class;
        void Unsubscribe(string ChannelName);
    }

    public static class ServicebusExt
    {
        public static Guid Subscribe<TMessage>(this IServiceBus servicebus, Action<TMessage> handlerAction) where TMessage : class
        {
            return servicebus.Subscribe<TMessage>(string.Empty, handlerAction);
        }

        public static Guid Subscribe<TMessage>(this IServiceBus servicebus, Func<TMessage, System.Threading.Tasks.Task> handlerAction) where TMessage : class
        {
            return servicebus.Subscribe<TMessage>(string.Empty, handlerAction);
        }

        public static Task SendAsync<TMessage>(this IServiceBus servicebus, TMessage message) where TMessage : class
        {
            return servicebus.SendAsync(string.Empty, message);
        }
    }

    public class DefaultServiceBus : IServiceBus
    {
        class Subscription
        {
            public Guid Id { get; private set; }
            public Func<object, Task> HandlerAction { get; private set; }

            public Subscription(Guid id, Func<object, Task> handlerAction)
            {
                Id = id;
                HandlerAction = handlerAction;
            }
        }

        private readonly ConcurrentQueue<Subscription> m_handlersToSubscribe = new ConcurrentQueue<Subscription>();

        private readonly ConcurrentQueue<Guid> m_idsToUnsubscribe = new ConcurrentQueue<Guid>();

        private readonly ActionBlock<Tuple<object, Action>> m_messageProcessor;


        public DefaultServiceBus()
        {

            var subscriptions = new List<Subscription>();

            m_messageProcessor = new ActionBlock<Tuple<object, Action>>(
                 tuple =>
                 {
                     var message = tuple.Item1;
                     var completedAction = tuple.Item2;

                     Guid idToUnsubscribe;
                     while (m_idsToUnsubscribe.TryDequeue(out idToUnsubscribe))
                     {
                         subscriptions.RemoveAll(s => s.Id == idToUnsubscribe);
                     }

                     Subscription handlerToSubscribe;
                     while (m_handlersToSubscribe.TryDequeue(out handlerToSubscribe))
                     {
                         subscriptions.Add(handlerToSubscribe);
                     }

                     foreach (var subscription in subscriptions)
                     {
                         subscription.HandlerAction(message);
                     }

                     completedAction();
                 });
        }

        public System.Threading.Tasks.Task SendAsync<TMessage>(string ChannelName, TMessage message) where TMessage : class
        {
            var tcs = new TaskCompletionSource<bool>();

            Action completedAction = () => tcs.SetResult(true);

            m_messageProcessor.Post(new Tuple<object, Action>(message, completedAction));

            return tcs.Task;
        }

        public Guid Subscribe<TMessage>(string ChannelName, Action<TMessage> handlerAction) where TMessage : class
        {
            return Subscribe<TMessage>(ChannelName,
                message =>
                {
                    handlerAction(message);

                    return Task.FromResult(false);
                });
        }

        public Guid Subscribe<TMessage>(string ChannelName, Func<TMessage, System.Threading.Tasks.Task> handlerAction) where TMessage : class
        {
            Func<object, Task> actionWithCheck = message =>
            {

                if (message is TMessage)
                    return handlerAction((TMessage)message);

                return Task.FromResult(0);
            };

            var id = Guid.NewGuid();
            m_handlersToSubscribe.Enqueue(new Subscription(id, actionWithCheck));
            return id;
        }

        //public void Unsubscribe(Guid subscriptionId)
        //{
        //    m_idsToUnsubscribe.Enqueue(subscriptionId);
        //}

        public void Unsubscribe(string ChannelName)
        {

        }


    }
}
