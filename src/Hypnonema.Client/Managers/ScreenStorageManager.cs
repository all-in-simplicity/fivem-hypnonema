namespace Hypnonema.Client
{
    using System;
    using System.Collections.Generic;

    using CitizenFX.Core;

    using Hypnonema.Client.Communications;
    using Hypnonema.Client.Utils;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Models;

    public sealed class ScreenStorageManager : IDisposable
    {
        ~ScreenStorageManager()
        {
            this.Dispose();
        }

        public event EventHandler<ScreenDeletedEventArgs> ScreenDeleted;

        public NetworkMethod<Screen> CreateScreen { get; private set; }

        public NetworkMethod<string> DeleteScreen { get; private set; }

        public NetworkMethod<Screen> EditScreen { get; private set; }

        public NetworkMethod<List<Screen>> GetScreenList { get; private set; }

        public bool IsInitialized { get; private set; }

        public List<Screen> Screens { get; private set; }

        public void Dispose()
        {
            ClientScript.Self.UnregisterNuiCallback(Events.CreateScreen, this.OnCreateScreen);
            ClientScript.Self.UnregisterNuiCallback(Events.DeleteScreen, this.OnDeleteScreen);
            ClientScript.Self.UnregisterNuiCallback(Events.EditScreen, this.OnEditScreen);
            ClientScript.Self.UnregisterNuiCallback(Events.GetScreenList, this.OnGetScreenList);

            GC.SuppressFinalize(this);
        }

        public void Initialize()
        {
            if (this.IsInitialized) return;

            this.CreateScreen = new NetworkMethod<Screen>(Events.CreateScreen, this.OnCreateScreen);
            this.DeleteScreen = new NetworkMethod<string>(Events.DeleteScreen, this.OnDeleteScreen);
            this.EditScreen = new NetworkMethod<Screen>(Events.EditScreen, this.OnEditScreen);
            this.GetScreenList = new NetworkMethod<List<Screen>>(Events.GetScreenList, this.OnGetScreenList);

            ClientScript.Self.RegisterNuiCallback(Events.CreateScreen, this.OnCreateScreen);
            ClientScript.Self.RegisterNuiCallback(Events.DeleteScreen, this.OnDeleteScreen);
            ClientScript.Self.RegisterNuiCallback(Events.EditScreen, this.OnEditScreen);
            ClientScript.Self.RegisterNuiCallback(Events.GetScreenList, this.OnGetScreenList);

            this.IsInitialized = true;
        }

        private CallbackDelegate OnCreateScreen(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screen = ArgsReader.GetArgKeyValue<Screen>(args, "payload");
            if (screen == null)
            {
                callback("ERROR", "payload is empty");
                return callback;
            }

            this.CreateScreen.Invoke(screen);

            callback("OK");
            return callback;
        }

        private void OnCreateScreen(Screen screen)
        {
            Nui.SendMessage("createdScreen", screen);
        }

        private CallbackDelegate OnDeleteScreen(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName))
            {
                callback("ERROR", "screenName is empty");
                return callback;
            }

            this.DeleteScreen.Invoke(screenName);

            callback("OK");
            return callback;
        }

        private void OnDeleteScreen(string screenName)
        {
            Nui.SendMessage(Events.DeleteScreen, screenName);

            var eventArgs = new ScreenDeletedEventArgs() { ScreenName = screenName };
            this.OnScreenDeleted(eventArgs);
        }

        private CallbackDelegate OnEditScreen(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screen = ArgsReader.GetArgKeyValue<Screen>(args, "payload");
            if (screen == null)
            {
                callback("ERROR", "payload is empty");
                return callback;
            }

            this.EditScreen.Invoke(screen);

            callback("OK");
            return callback;
        }

        private void OnEditScreen(Screen screen)
        {
            Nui.SendMessage(Events.EditScreen, screen);
        }

        private CallbackDelegate OnGetScreenList(IDictionary<string, object> args, CallbackDelegate callback)
        {
            this.GetScreenList.InvokeNoArgs();

            callback("OK");
            return callback;
        }

        private void OnGetScreenList(List<Screen> screenList)
        {
            this.Screens = screenList;
            Nui.SendMessage(Events.GetScreenList, screenList);
        }

        private void OnScreenDeleted(ScreenDeletedEventArgs e)
        {
            var handler = this.ScreenDeleted;

            handler?.Invoke(this, e);
        }
    }

    public class ScreenDeletedEventArgs : EventArgs
    {
        public string ScreenName { get; set; }
    }
}