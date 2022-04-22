using Hypnonema.Client.Extensions;
using Hypnonema.Shared.Communications;

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

        public NetworkMethod<CreateScreenMessage> CreateScreen { get; private set; }

        public NetworkMethod<DeleteScreenMessage> DeleteScreen { get; private set; }

        public NetworkMethod<EditScreenMessage> EditScreen { get; private set; }

        public NetworkMethod<ScreenListMessage> GetScreenList { get; private set; }

        public bool IsInitialized { get; private set; }

        public List<Screen> Screens { get; private set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Initialize()
        {
            if (this.IsInitialized) return;

            this.CreateScreen = new NetworkMethod<CreateScreenMessage>(Events.CreateScreen, this.OnCreateScreen);
            this.DeleteScreen = new NetworkMethod<DeleteScreenMessage>(Events.DeleteScreen, this.OnDeleteScreen);
            this.EditScreen = new NetworkMethod<EditScreenMessage>(Events.EditScreen, this.OnEditScreen);
            this.GetScreenList = new NetworkMethod<ScreenListMessage>(Events.GetScreenList, this.OnGetScreenList);
            
            ClientScript.Self.RegisterCallback(Events.CreateScreen, this.OnCreateScreen);
            ClientScript.Self.RegisterCallback(Events.DeleteScreen, this.OnDeleteScreen);
            ClientScript.Self.RegisterCallback(Events.EditScreen, this.OnEditScreen);
            ClientScript.Self.RegisterCallback(Events.GetScreenList, this.OnGetScreenList);

            this.IsInitialized = true;
        }

        private void OnCreateScreen(IDictionary<string, object> data)
        {
            var screen = data.GetTypedValue<Screen>("payload");
            if (screen == null)
            {
                ClientScript.AddChatMessage("Failed to create screen. screen is null or empty");
                return;
            }

            var createScreenMessage = new CreateScreenMessage(screen);

            this.CreateScreen.Invoke(createScreenMessage);
        }

        private void OnCreateScreen(CreateScreenMessage createScreenMessage)
        {
            Nui.SendMessage("createdScreen", createScreenMessage.Screen);
        }

        private void OnDeleteScreen(IDictionary<string, object> data)
        {
            var screenName = data.GetTypedValue<string>("screenName");
            if (string.IsNullOrEmpty(screenName))
            {
                ClientScript.AddChatMessage("Failed to delete Screen. screenName is empty");
                return;
            }

            var deleteScreenMessage = new DeleteScreenMessage(screenName);
            
            this.DeleteScreen.Invoke(deleteScreenMessage);
        }

        private void OnDeleteScreen(DeleteScreenMessage deleteScreenMessage)
        {
            Nui.SendMessage(Events.DeleteScreen, deleteScreenMessage.ScreenName);
        }

        private void OnEditScreen(IDictionary<string, object> data)
        {
            var screen = data.GetTypedValue<Screen>("payload");
            if (screen == null)
            {
                ClientScript.AddChatMessage("Failed to edit screen. payload is missing");
                return;
            }

            var editScreenMessage = new EditScreenMessage(screen);

            this.EditScreen.Invoke(editScreenMessage);
        }

        private void OnEditScreen(EditScreenMessage editScreenMessage)
        {
            Nui.SendMessage(Events.EditScreen, editScreenMessage.Screen);
        }

        private void OnGetScreenList(IDictionary<string, object> data)
        {
            this.GetScreenList.InvokeNoArgs();
        }

        private void OnGetScreenList(ScreenListMessage screenListMessage)
        {
            this.Screens = screenListMessage.Screens;

            Logger.Debug($"received {screenListMessage.Screens.Count} screens");

            Nui.SendMessage(Events.GetScreenList, screenListMessage.Screens);
        }
    }
}