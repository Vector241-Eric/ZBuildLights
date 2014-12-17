using System;
using System.Collections.Generic;
using System.Linq;

namespace ZBuildLights.Core.Extensions
{
    public static class IEnumerableExtensions
    {
        public static T[] ExceptInstance<T>(this IEnumerable<T> e, T instance)
        {
            return e.Except(new[] {instance}).ToArray();
        }

        public static bool None<T>(this IEnumerable<T> e, Func<T, bool> check)
        {
            return !e.Any(check);
        }
    }
}