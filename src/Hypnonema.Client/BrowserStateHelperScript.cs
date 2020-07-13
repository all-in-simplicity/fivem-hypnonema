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
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName))
            {
                Debug.WriteLine("Warning: received state response without valid screenName.");
                callback("");
                return callback;
            }

            var paused = ArgsReader.GetArgKeyValue<bool>(args, "paused");
            var repeat = ArgsReader.GetArgKeyValue<bool>(args, "repeat");
            var currentTime = ArgsReader.GetArgKeyValue<float>(args, "currentTime");
            var duration = ArgsReader.GetArgKeyValue<float>(args, "duration");
            var ended = ArgsReader.GetArgKeyValue<bool>(args, "ended");
            var currentSource = ArgsReader.GetArgKeyValue<string>(args, "currentSource");

            var state = new DuiState
                            {
                                CurrentTime = currentTime,
                                ScreenName = screenName,
                                Ended = ended,
                                IsPaused = paused,
                                Duration = duration,
                                CurrentSource = currentSource,
                                Repeat = repeat
                            };

            StateQueue.Enqueue(state);

            callback("OK");
            return callback;
        }
    }
}