using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncLinq
{
    public static class FEnumerableEx
    {
        public static Func<Func<Func<T>>> AsFEnumerable<T>(this IEnumerable<T> source)
        {
            return () =>
                       {
                           var e = source.GetEnumerator();
                           return () =>
                                  e.MoveNext() ? e.Current.AsFunc() : null;
                       };
        }
    }
}
