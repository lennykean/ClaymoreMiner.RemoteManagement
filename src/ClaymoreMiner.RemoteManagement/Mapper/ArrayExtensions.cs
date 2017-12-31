using System;

namespace ClaymoreMiner.RemoteManagement.Mapper
{
    internal static class ArrayExtensions
    {
        public static(T value, bool success) TryGet<T>(this T[] array, int index)
        {
            if (array?.Length > index)
                return (array[index], true);

            return (default(T), false);
        }

        public static(int value, bool success) TryGetAndParse(this string[] array, int index)
        {
            var (stringValue, success) = array.TryGet(index);

            if (success && int.TryParse(stringValue, out var value))
                return (value, true);

            return (0, false);
        }
    }
}