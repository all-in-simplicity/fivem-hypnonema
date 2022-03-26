namespace Hypnonema.Shared
{
    // https://gist.github.com/ApocDev/037565557874c65f3198f962459eb447
    using System;

    /// <summary>
    ///     A simple type cache to alleviate the reflection lookups. Checking for simple types is done once per type
    ///     encountered. Rather than every time. (Saves many CPU cycles. Reflection is slow)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class TypeCache<T>
    {
        static TypeCache()
        {
            Type = typeof(T);
            IsSimpleType = true;
            switch (Type.GetTypeCode(Type))
            {
                case TypeCode.Object:
                case TypeCode.DBNull:
                case TypeCode.Empty:
                case TypeCode.DateTime:
                    IsSimpleType = false;
                    break;
            }
        }

        // ReSharper disable StaticMemberInGenericType
        public static bool IsSimpleType { get; }

        public static Type Type { get; }

        // ReSharper restore StaticMemberInGenericType
    }
}