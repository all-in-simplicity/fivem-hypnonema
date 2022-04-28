namespace Hypnonema.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Client.Dui;
    using Hypnonema.Client.Utils;
    using Hypnonema.Shared;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
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

        public DuiStateHelper DuiStateHelper { get; private set; }

        public static void AddChatMessage(string message, int[] color = null)
        {
            if (color == null) color = new[] {0, 128, 128};

            TriggerEvent("chat:addMessage", new {color, args = new[] {"[Hypnonema]", $"{message}"}});
        }

        public void AddEvent(string eventName, Delegate action)
        {
            this.EventHandlers[eventName] += action;
        }

        public void AddTick(Func<Task> tickHandler)
        {
            this.Tick += tickHandler;
        }

        public void RegisterCallback(string @event, Action<IDictionary<string, object>> action)
        {
            API.RegisterNuiCallbackType(@event);

            this.EventHandlers[$"__cfx_nui:{@event}"] += new Action<IDictionary<string, object>, CallbackDelegate>(
                (data, callback) =>
                    {
                        action(data);
                        callback("OK");
                    });
        }

        public async void RegisterCallback<TReturn>(string @event, Func<Task<TReturn>> action)
        {
            API.RegisterNuiCallbackType(@event);

            this.EventHandlers[$"__cfx_nui:{@event}"] += new Func<IDictionary<string, object>, CallbackDelegate, Task>(
                async (data, callback) =>
                    {
                        var result = await action();
                        callback(callback(result));
                    });
        }

        public void RemoveEvent(string eventName, Delegate action)
        {
            this.EventHandlers[eventName] -= action;
        }

        public void RemoveTick(Func<Task> tickHandler)
        {
            this.Tick -= tickHandler;
        }

        private static void ShowUI(bool show)
        {
            Nui.SendMessage("showUI", show);
            API.SetNuiFocus(show, show);
        }

        private void OnCommand(int source, List<object> args, string raw)
        {
            if (args.Count == 0)
            {
                // no arguments provided. show nui window
                ShowUI(true);
                this.screenStorageManager.GetScreenList.InvokeNoArgs();

                return;
            }

            var firstArg = args[0].ToString();
            switch (firstArg)
            {
                case "volume":
                    if (args[1] == null) return;

                    var value = (float) TypeDescriptor.GetConverter(typeof(float))
                        .ConvertFromString(args[1].ToString());
                    if (value < 0 || value > 100)
                    {
                        AddChatMessage("volume value is out of range. allowed range: 0-100");
                        return;
                    }

                    TriggerEvent(Events.ClientVolume, value);

                    break;
            }
        }

        private async Task OnFirstTick()
        {
            this.Tick -= this.OnFirstTick;

            this.DuiStateHelper = new DuiStateHelper();

            this.screenStorageManager.Initialize();
            this.screenPlaybackManager.Initialize();

            this.RegisterCommand();

            this.RegisterCallback(Events.HideUI, this.OnHideUI);

            await this.SynchronizeState();
        }

        private void OnHideUI(IDictionary<string, object> data)
        {
            ShowUI(false);
        }

        private void RegisterCommand()
        {
            var cmdName = ConfigReader.GetConfigKeyValue(
                API.GetCurrentResourceName(),
                "hypnonema_command_name",
                0,
                "hypnonema");

            API.RegisterCommand(cmdName, new Action<int, List<object>, string>(this.OnCommand), false);
        }

        // Synchronize states (if any) on first start
        private async Task SynchronizeState()
        {
            var duiStates = await this.DuiStateHelper.RequestDuiStateAsync();

            if (duiStates != null)
                foreach (var duiState in duiStates.Where(duiState => !duiState.Ended))
                    await this.screenPlaybackManager.SynchronizeState(duiState);
            else
                Logger.Debug("state is empty. skipping synchronization");
        }
    }
}