namespace Hypnonema.Client.Utils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Hypnonema.Shared;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class ArgsReader
    {
        private static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                                                                                {
                                                                                    ContractResolver =
                                                                                        new
                                                                                            CamelCasePropertyNamesContractResolver()
                                                                                };

        public static T GetArgKeyValue<T>(IDictionary<string, object> args, string key, T defaultValue)
        {
            var result = defaultValue;

            try
            {
                var input = args.FirstOrDefault(arg => arg.Key == key).Value?.ToString();
                result = TypeCache<T>.IsSimpleType
                             ? (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input)
                             : JsonConvert.DeserializeObject<T>(input, serializerSettings);
            }
            catch (Exception)
            {
                Logger.Warn($"ArgsReader failed to read key \"{key}\".");
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