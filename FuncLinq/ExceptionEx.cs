using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuncLinq
{
    internal static class ExceptionEx
    {
        public static Func<T> AsFunc<T>(this Exception exception)
        {
            return () =>
                       {
                           throw exception;
                       };
        }
    }
}
