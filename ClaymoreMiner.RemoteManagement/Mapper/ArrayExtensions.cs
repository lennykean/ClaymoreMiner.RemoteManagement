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

        public static(int value, bool success) TryParseInt(this string[] array, int index)
        {
            var (stringValue, success) = array.TryGet(index);

            if (success && int.TryParse(stringValue, out var value))
                return (value, true);

            return (0, false);
        }

        public static(TimeSpan value, bool success) TryParseTimeSpanMinutes(this string[] array, int index)
        {
            var (value, success) = array.TryParseInt(index);

            if (success)
                return (TimeSpan.FromMinutes(value), true);

            return (TimeSpan.Zero, false);
        }
    }
}