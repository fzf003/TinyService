using System;
using System.Collections.Generic;
using System.Text;

namespace TinyService.ReactiveRabbit
{
    public static class ObservableExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> observable, IEndPoint<T> endPoint)
        {
            return observable.Subscribe(endPoint.PushMessage, endPoint.PushError);
        }
    }
}
