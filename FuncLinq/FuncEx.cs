using System;

namespace FuncLinq
{
    internal static class FuncEx
    {
        public static Func<T> AsFunc<T>(this T value)
        {
            return () => value;
        }
    }
}