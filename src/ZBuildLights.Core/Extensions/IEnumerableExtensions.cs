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
    }
}