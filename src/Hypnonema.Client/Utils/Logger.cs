namespace Hypnonema.Client.Utils
{
    using System;

    using CitizenFX.Core.Native;

    public class Logger
    {
        private static bool IsLoggingEnabled =>
            ConfigReader.GetConfigKeyValue(API.GetCurrentResourceName(), "hypnonema_logging_enabled", 0, false);

        public static void Debug(string message)
        {
            if (!IsLoggingEnabled) return;

            var prefix = $"^6[Hypnonema] [{DateTime.Now.ToShortTimeString()}] [DEBUG]";

            CitizenFX.Core.Debug.WriteLine($"{prefix} {message} ^7");
        }

        public static void Error(string message)
        {
            if (!IsLoggingEnabled) return;
            var prefix = $"^6[Hypnonema] [{DateTime.Now.ToShortTimeString()}] ^1[ERROR]";

            CitizenFX.Core.Debug.WriteLine($"{prefix} {message} ^7");
        }

        public static void Info(string message)
        {
            if (!IsLoggingEnabled) return;

            var prefix = $"^6[Hypnonema] [{DateTime.Now.ToShortTimeString()}] ^5[INFO]";

            CitizenFX.Core.Debug.WriteLine($"{prefix} {message} ^7");
        }

        public static void Verbose(string message)
        {
            if (!IsLoggingEnabled) return;

            var prefix = $"^6[Hypnonema] [{DateTime.Now.ToShortTimeString()}]^7";

            CitizenFX.Core.Debug.WriteLine($"{prefix} {message}");
        }

        public static void Warn(string message)
        {
            if (!IsLoggingEnabled) return;

            var prefix = $"^6[Hypnonema] [{DateTime.Now.ToShortTimeString()}] ^3[WARN]";

            CitizenFX.Core.Debug.WriteLine($"{prefix} {message} ^7");
        }
    }
}