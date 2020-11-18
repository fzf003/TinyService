using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

namespace TinyServer.ReactiveSocket
{
    public static class ObservableExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> observable, IMessageHandle<T> endPoint)
        {
            
            return observable.Subscribe(endPoint.OnMessage, endPoint.OnError, endPoint.OnCompleted);
        }

        public static IObservable<T> PrintHandler<T>(this IObservable<T> observable,IMessageHandle<T> messageHandle)
        {
            return observable.Do(messageHandle.OnMessage, messageHandle.OnError, messageHandle.OnCompleted);
        }

        public static IObservable<TSource> TakeWhile<TSource>(this IObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return Observable.Create<TSource>(
                observer => source.Subscribe(
                    item =>
                    {
                        observer.OnNext(item);
                        if (predicate(item))
                            observer.OnCompleted();
                    },
                    observer.OnError,
                    observer.OnCompleted
                    )
                );
        }
    }
}
