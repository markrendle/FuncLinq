using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncLinq
{
    /// <summary>
    /// So this is essentially MinLinq as implemented by Bart de Smet, but he used
    /// an additional class, Option&lt;T&gt>, as the output from the functional enumerable,
    /// and this uses another Func which will in turn, if called, return the Current value.
    /// I'm blogging it soon. It'll become clear.
    /// </summary>
    public static class FEnumerable
    {
        public static Func<Func<Func<T>>> Empty<T>()
        {
            return () => () => null;
        }

        public static Func<Func<Func<T>>> Return<T>(T value)
        {
            return () =>
            {
                int i = 0;
                return () =>
                    i++ == 0
                    ? new Func<T>(() => value)
                    : null;
            };
        }

        public static Func<Func<Func<T>>> Ana<T>(T seed, Func<T,bool> condition, Func<T,T> next)
        {
            return () =>
            {
                Func<T> value = null;
                return () =>
                           {
                               if (value == null)
                               {
                                   value = () => seed;
                               }
                               else
                               {
                                   var output = next(value());
                                   value = () => output;
                               }

                               return condition(value()) ? value : null;
                           };
            };
        }

        public static Func<Func<Func<TResult>>> Bind<TSource, TResult>(this Func<Func<Func<TSource>>> source, Func<TSource, Func<Func<Func<TResult>>>> selector)
        {
            return () =>
                       {
                           var sourceMoveNext = source();
                           Func<TResult> lastInner = null;
                           Func<Func<TResult>> innerEnumerator = null;

                           return () =>
                                      {
                                          do
                                          {
                                              while (lastInner == null)
                                              {
                                                  var lastSourceItem = sourceMoveNext();

                                                  if (lastSourceItem == null)
                                                  {
                                                      return null;
                                                  }

                                                  innerEnumerator = selector(lastSourceItem())();

                                                  lastInner = innerEnumerator();
                                                  if (lastInner != null)
                                                  {
                                                      return lastInner;
                                                  }
                                              }

                                              lastInner = innerEnumerator();
                                          } while (lastInner == null);

                                          return lastInner;
                                      };
                       };
        }

        public static TResult Cata<TSource, TResult>(this Func<Func<Func<TSource>>> source, TResult seed, Func<TResult, TSource, TResult> f)
        {
            var moveNext = source();

            Func<TSource> value;
            TResult result = seed;
            while ((value = moveNext()) != null)
            {
                result = f(result, value());
            }

            return result;
        }

        public static IEnumerable<T> AsEnumerable<T>(this Func<Func<Func<T>>> source)
        {
            var moveNext = source();
            Func<T> next;
            while ((next = moveNext()) != null)
            {
                yield return next();
            }
        }
    }
}
