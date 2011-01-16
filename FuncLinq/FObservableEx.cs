using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncLinq
{
    public static class FObservableEx
    {
        public static Action<Action<Func<T>>> AsFObservable<T>(this IObservable<T> source)
        {
            return o => source.Subscribe(new GenericObserver<T>(
                                             t => o(t.AsFunc()),
                                             e => o(e.AsFunc<T>()),
                                             () => o(null)
                                             ));
        }

        private class GenericObserver<T> : IObserver<T>
        {
            private readonly Action<T> _onNext;
            private readonly Action<Exception> _onError;
            private readonly Action _onCompleted;

            public GenericObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
            {
                _onNext = onNext;
                _onCompleted = onCompleted;
                _onError = onError;
            }

            public void OnNext(T value)
            {
                _onNext(value);
            }

            public void OnError(Exception error)
            {
                _onError(error);
            }

            public void OnCompleted()
            {
                _onCompleted();
            }
        }
    }
}
