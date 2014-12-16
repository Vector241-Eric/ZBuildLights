using System.Linq;

namespace ZBuildLights.Core.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] AddToEnd<T>(this T[] array, T member)
        {
            var list = (array ?? new T[0]).ToList();
            list.Add(member);
            return list.ToArray();
        }
    }
}