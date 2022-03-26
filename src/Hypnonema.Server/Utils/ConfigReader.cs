﻿namespace Hypnonema.Server.Utils
{
    using System;
    using System.ComponentModel;

    using CitizenFX.Core.Native;

    public static class ConfigReader
    {
        public static T GetConfigKeyValue<T>(string resourceName, string metadataKey, int index, T defaultValue)
        {
            var result = defaultValue;

            try
            {
                var input = API.GetResourceMetadata(resourceName, metadataKey, index);
                result = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input);
            }
            catch (Exception)
            {
                Logger.Error($"Failed to parse {metadataKey}. Using default value {defaultValue}");
            }

            return result;
        }
    }
}