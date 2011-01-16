using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncLinq
{
    public static class FObservable
    {
        public static Action<Action<Func<T>>> Empty<T>()
        {
            return o => o(null);
        }

        public static Action<Action<Func<T>>> Return<T>(T value)
        {
            return o =>
                       {
                           o(value.AsFunc());
                           o(null);
                       };
        }

        public static Action<Action<Func<T>>> Ana<T>(T seed, Func<T,bool> condition, Func<T,T> next)
        {
            return o =>
                       {
                           for (T t = seed; condition(t); t = next(t))
                           {
                               o(t.AsFunc());
                           }
                       };
        }

        public static Action<Action<Func<TOut>>> Bind<TIn, TOut>(this Action<Action<Func<TIn>>> source, Func<TIn, Action<Action<Func<TOut>>>> selector)
        {
            return o => source(x =>
            {
                if (x == null)
                {
                    o(null);
                }
                else
                {
                    selector(x())(y =>
                    {
                        if (y != null)
                            o(y);
                    });
                }
            });
        }

        public static TResult Cata<TSource, TResult>(this Action<Action<Func<TSource>>> source, TResult seed, Func<TResult, TSource, TResult> f)
        {
            TResult result = seed;

            bool end = false;
            source(x =>
            {
                if (x != null && !end)
                    result = f(result, x());
                else
                    end = true; // or break using exception
            });

            return result;
        }

        public static IObservable<T> AsObservable<T>(this Action<Action<Func<T>>> source)
        {
            return new GenericObservable<T>(source);
        }

        private sealed class GenericObservable<T> : IObservable<T>
        {
            private Action<T> _onNext = t => { };
            private Action<Exception> _onError = e => { };
            private Action _onCompleted = () => { };

            public GenericObservable(Action<Action<Func<T>>> fobservable)
            {
                fobservable(f =>
                                {
                                    if (f != null)
                                    {
                                        try
                                        {
                                            OnNext(f());
                                        }
                                        catch (Exception ex)
                                        {
                                            OnError(ex);
                                        }
                                    }
                                    else
                                    {
                                        OnCompleted();
                                    }
                                });
            }

            private Action<T> OnNext { get { return _onNext; } }
            private Action<Exception> OnError { get { return _onError; } }
            private Action OnCompleted { get { return _onCompleted; } }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                _onNext += observer.OnNext;
                _onError += observer.OnError;
                _onCompleted += observer.OnCompleted;

                return new Unsub(() =>
                                     {
                                         _onNext -= observer.OnNext;
                                         _onError -= observer.OnError;
                                         _onCompleted -= observer.OnCompleted;
                                     });
            }

            private class Unsub : IDisposable
            {
                private readonly Action _unsubscribe;

                public Unsub(Action unsubscribe)
                {
                    _unsubscribe = unsubscribe;
                }

                public void Dispose()
                {
                    _unsubscribe();
                }
            }
        }
    }
}
