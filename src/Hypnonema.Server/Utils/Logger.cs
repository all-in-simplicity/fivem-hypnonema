namespace Hypnonema.Server.Utils
{
    using System;

    using CitizenFX.Core;

    public static class Logger
    {
        public enum LogLevel
        {
            Verbose = 0,

            Debug = 1,

            Information = 2,

            Warning = 3,

            Error = 4
        }

        public static void WriteLine(string message, LogLevel logLevel = LogLevel.Debug)
        {
            if (!IsLoggingEnabled()) return;

            var prefix = $"[Hypnonema] [{DateTime.Now.ToShortTimeString()}]";

            switch (logLevel)
            {
                case LogLevel.Warning:
                    prefix = $"{prefix} [WARN]";
                    break;
                case LogLevel.Debug:
                    prefix = $"{prefix} [DEBUG]";
                    break;
                case LogLevel.Error:
                    prefix = $"{prefix} [ERROR]";
                    break;
                case LogLevel.Information:
                    prefix = $"{prefix} [INFO]";
                    break;
                case LogLevel.Verbose:
                    prefix = $"{prefix} ";
                    break;
            }

            Debug.WriteLine($"{prefix} {message}");
        }

        private static bool IsLoggingEnabled()
        {
            return ServerScript.IsLoggingEnabled;
        }
    }
}