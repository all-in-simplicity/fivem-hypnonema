namespace Hypnonema.Server
{
    using System;

    using CitizenFX.Core;

    public static class Log
    {
        public static void WriteLine(string data)
        {
            Debug.Write($"^4[Hypnonema] [{DateTime.Now.ToShortTimeString()}]: ");
            Debug.WriteLine($"{data}^7");
        }

    }
}