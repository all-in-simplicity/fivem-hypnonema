namespace Hypnonema.Client.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Hypnonema.Client.Utils;

    using Newtonsoft.Json;

    public static class DictionaryExtensions
    {
        public static T GetTypedValue<T>(this IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            var result = defaultValue;

            try
            {
                var input = dictionary.FirstOrDefault(arg => arg.Key == key).Value?.ToString();
                result = IsPrimitive<T>()
                             ? (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input)
                             : JsonConvert.DeserializeObject<T>(input, Nui.NuiSerializerSettings);
            }
            catch (Exception)
            {
                Logger.Warn($"failed to read key \"{key}\" from nui sent data.");
            }

            return result;
        }

        public static T GetTypedValue<T>(this IDictionary<string, object> dictionary, string key)
        {
            var result = default(T);
            try
            {
                result = GetTypedValue(dictionary, key, default(T));
            }
            catch (Exception)
            {
            }

            return result;
        }

        public static bool IsPrimitive<T>()
        {
            var type = typeof(T);

            return type.IsPrimitive || type == typeof(decimal) || type == typeof(string) || type == typeof(DateTime)
                   || type == typeof(TimeSpan) || type == typeof(DateTimeOffset);
        }
    }
}