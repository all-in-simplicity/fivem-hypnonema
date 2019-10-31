namespace Hypnonema.Client
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Shared;

    public class BrowserStateHelperScript : BaseScript
    {
        private static readonly Queue<DuiState> StateQueue = new Queue<DuiState>();

        public BrowserStateHelperScript()
        {
            this.RegisterNuiCallback(ClientEvents.GetStateResponse, this.GetStateResponse);
        }

        public static async Task<DuiState> GetLastState(int timeout = 5500)
        {
            var endTime = DateTime.UtcNow + new TimeSpan(0, 0, 0, 0, timeout);
            while (StateQueue.Count == 0)
            {
                await Delay(0);

                if (DateTime.UtcNow >= endTime) return null;
            }

            return StateQueue.Dequeue();
        }

        protected void RegisterNuiCallback(
            string msg,
            Func<IDictionary<string, object>, CallbackDelegate, CallbackDelegate> callback)
        {
            API.RegisterNuiCallbackType(msg);

            this.EventHandlers[$"__cfx_nui:{msg}"] += new Action<ExpandoObject, CallbackDelegate>(
                (body, resultCallback) => { callback(body, resultCallback); });
        }

        private CallbackDelegate GetStateResponse(IDictionary<string, object> args, CallbackDelegate callback)
        {
            bool.TryParse(args.FirstOrDefault(arg => arg.Key == "paused").Value?.ToString(), out var paused);
            float.TryParse(args.FirstOrDefault(arg => arg.Key == "currentTime").Value?.ToString(), out var currentTime);
            float.TryParse(args.FirstOrDefault(arg => arg.Key == "duration").Value?.ToString(), out var duration);
            bool.TryParse(args.FirstOrDefault(arg => arg.Key == "ended").Value?.ToString(), out var ended);
            var currentSource = args.FirstOrDefault(arg => arg.Key == "currentSource").Value?.ToString();

            var screenName = args.FirstOrDefault(arg => arg.Key == "screenName").Value?.ToString();
            if (string.IsNullOrEmpty(screenName))
            {
                Debug.WriteLine("error: received state response without valid screenName.");
                return callback;
            }

            var state = new DuiState
                            {
                                CurrentTime = currentTime,
                                ScreenName = screenName,
                                Ended = ended,
                                IsPaused = paused,
                                Duration = duration,
                                CurrentSource = currentSource
                            };

            StateQueue.Enqueue(state);
            return callback;
        }
    }
}