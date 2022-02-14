namespace Hypnonema.Client
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Client.Utils;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Models;

    using Newtonsoft.Json;

    public class ClientScript : BaseScript
    {
        private readonly ScreenPlaybackManager screenPlaybackManager = new ScreenPlaybackManager();

        private readonly ScreenStorageManager screenStorageManager = new ScreenStorageManager();

        public ClientScript()
        {
            Self = this;
            this.Tick += this.OnFirstTick;
        }

        public static ClientScript Self { get; private set; }

        public static void AddChatMessage(string message, int[] color = null)
        {
            if (color == null) color = new[] { 0, 128, 128 };

            TriggerEvent("chat:addMessage", new { color, args = new[] { "[Hypnonema]", $"{message}" } });
        }

        public dynamic GetState(string key)
        {
            return this.GlobalState.Get(key);
        }

        public void RegisterEvent(string eventName, Delegate action)
        {
            this.EventHandlers[eventName] += action;
        }

        public void RegisterNuiCallback(
            string msg,
            Func<IDictionary<string, object>, CallbackDelegate, CallbackDelegate> callback)
        {
            API.RegisterNuiCallbackType(msg);

            this.EventHandlers[$"__cfx_nui:{msg}"] += new Action<ExpandoObject, CallbackDelegate>(
                (body, resultCallback) => { callback.Invoke(body, resultCallback); });
        }

        public void RegisterTick(Func<Task> tickHandler)
        {
            this.Tick += tickHandler;
        }

        public void UnregisterEvent(string eventName, Delegate action)
        {
            this.EventHandlers[eventName] -= action;
        }

        public void UnregisterNuiCallback(
            string msg,
            Func<IDictionary<string, object>, CallbackDelegate, CallbackDelegate> callback)
        {
            this.EventHandlers[$"__cfx_nui:{msg}"] -= new Action<ExpandoObject, CallbackDelegate>(
                (body, resultCallback) => { callback.Invoke(body, resultCallback); });
        }

        public void UnregisterTick(Func<Task> tickHandler)
        {
            this.Tick -= tickHandler;
        }

        private void OnCommand()
        {
            Nui.SendMessage("showUI", true);
            API.SetNuiFocus(true, true);

            this.screenStorageManager.GetScreenList.InvokeNoArgs();
        }

        private async Task OnFirstTick()
        {
            this.Tick -= this.OnFirstTick;

            this.screenStorageManager.Initialize();
            this.screenPlaybackManager.Initialize();

            this.RegisterCommand();

            this.RegisterNuiCallback(Events.HideUI, this.OnHideUI);

            await this.SynchronizeState();
        }

        private CallbackDelegate OnHideUI(IDictionary<string, object> args, CallbackDelegate callback)
        {
            Nui.SendMessage("showUI", false);
            API.SetNuiFocus(false, false);

            callback("OK");
            return callback;
        }

        private void RegisterCommand()
        {
            var cmdName = ConfigReader.GetConfigKeyValue(
                API.GetCurrentResourceName(),
                "hypnonema_command_name",
                0,
                "hypnonema");

            API.RegisterCommand(cmdName, new Action(this.OnCommand), false);
        }

        // Synchronize states (if any) on first start
        private async Task SynchronizeState()
        {
            string stateJson = this.GetState("hypnonema");
            if (stateJson != null)
            {
                IList<DuiState> duiStates = JsonConvert.DeserializeObject<List<DuiState>>(stateJson);
                foreach (var duiState in duiStates)
                {
                    if (duiState.Ended && !duiState.Repeat) continue;

                    await this.screenPlaybackManager.SynchronizeState(duiState);
                }
            }
            else
            {
                Logger.Debug("state is empty. skipping synchronization");
            }
        }
    }
}