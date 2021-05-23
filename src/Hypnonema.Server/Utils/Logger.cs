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

            var prefix = $"^6[Hypnonema] [{DateTime.Now.ToShortTimeString()}]^7";

            switch (logLevel)
            {
                case LogLevel.Warning:
                    prefix = $"{prefix} ^3[WARN]";
                    break;
                case LogLevel.Debug:
                    prefix = $"{prefix} [DEBUG]";
                    break;
                case LogLevel.Error:
                    prefix = $"{prefix} ^1[ERROR]";
                    break;
                case LogLevel.Information:
                    prefix = $"{prefix} ^5[INFO]";
                    break;
                case LogLevel.Verbose:
                    prefix = $"{prefix} ";
                    break;
            }

            Debug.WriteLine($"{prefix} {message} ^7");
        }

        private static bool IsLoggingEnabled()
        {
            return Server.IsLoggingEnabled;
        }
    }
}