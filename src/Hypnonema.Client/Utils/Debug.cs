namespace Hypnonema.Client.Utils
{
    public class Debug
    {
        public static bool IsLoggingEnabled = false;

        public static void WriteLine(string data)
        {
            if (!IsLoggingEnabled) return;
            CitizenFX.Core.Debug.WriteLine(data);
        }

        public static void WriteLine(string format, params object[] args)
        {
            if (!IsLoggingEnabled) return;
            CitizenFX.Core.Debug.WriteLine(format, args);
        }
    }
}