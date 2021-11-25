namespace Hypnonema.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public static class ArgsReader
    {
        public static T GetArgKeyValue<T>(IDictionary<string, object> args, string key, T defaultValue)
        {
            var result = defaultValue;

            try
            {
                var input = args.FirstOrDefault(arg => arg.Key == key).Value?.ToString();
                result = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input);
            }
            catch (Exception)
            {
                Utils.Debug.WriteLine($"failed to read {key}");
            }

            return result;
        }

        public static T GetArgKeyValue<T>(IDictionary<string, object> args, string key)
        {
            var result = default(T);
            try
            {
                result = GetArgKeyValue(args, key, default(T));
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
}