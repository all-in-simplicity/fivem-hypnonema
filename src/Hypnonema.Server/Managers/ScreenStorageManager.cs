namespace Hypnonema.Server.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CitizenFX.Core;

    using Hypnonema.Server.Communications;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Communications;
    using Hypnonema.Shared.Models;

    using LiteDB;

    using Newtonsoft.Json;

    using Logger = Hypnonema.Server.Utils.Logger;

    public sealed class ScreenStorageManager
    {
        private LiteCollection<Screen> screenCollection;

        public NetworkMethod<CreateScreenMessage> CreateScreen { get; private set; }

        public NetworkMethod<DeleteScreenMessage> DeleteScreen { get; private set; }

        public NetworkMethod<EditScreenMessage> EditScreen { get; private set; }

        public NetworkMethod<ScreenListMessage> GetScreenList { get; private set; }

        public bool IsInitialized { get; private set; }

        public void Initialize(LiteCollection<Screen> screenCollection)
        {
            if (this.IsInitialized) return;

            this.screenCollection = screenCollection;

            this.CreateScreen = new NetworkMethod<CreateScreenMessage>(Events.CreateScreen, this.OnCreateScreen);
            this.DeleteScreen = new NetworkMethod<DeleteScreenMessage>(Events.DeleteScreen, this.OnDeleteScreen);
            this.EditScreen = new NetworkMethod<EditScreenMessage>(Events.EditScreen, this.OnEditScreen);
            this.GetScreenList = new NetworkMethod<ScreenListMessage>(Events.GetScreenList, this.OnGetScreenList);

            BaseServer.Self.AddExport(Events.GetScreenList, new Func<string>(this.OnGetScreenList));
            BaseServer.Self.AddExport(Events.CreateScreen, new Action<string>(this.OnCreateScreen));
            BaseServer.Self.AddExport(Events.EditScreen, new Action<string>(this.OnEditScreen));
            BaseServer.Self.AddExport(Events.DeleteScreen, new Action<string>(this.OnDeleteScreen));

            this.IsInitialized = true;
        }

        private List<Screen> GetScreensList()
        {
            return this.screenCollection.FindAll().ToList();
        }

        private void OnCreateScreen(string jsonScreen)
        {
            var screen = JsonConvert.DeserializeObject<Screen>(jsonScreen);
            if (screen == null)
            {
                Logger.Error("failed to create new screen. invalid argument");
                return;
            }

            var existingScreen = this.screenCollection.FindOne(s => s.Name == screen.Name);
            if (existingScreen != null)
            {
                Logger.Error($"failed to create a new screen. A screen with name \"{screen.Name}\" already exists.");
                return;
            }

            var id = this.screenCollection.Insert(screen);
            screen.Id = id;

            var screenListMessage = new ScreenListMessage(this.GetScreensList());
            this.GetScreenList.Invoke(null, screenListMessage);
        }

        private void OnCreateScreen(Player p, CreateScreenMessage createScreenMessage)
        {
            if (!p.IsAceAllowed(Permission.Create))
            {
                p.AddChatMessage(
                    $"You are not permitted to create a new screen. missing ace: ${Permission.Create}",
                    new[] {255, 0, 0});
                return;
            }

            var existingScreen = this.screenCollection.FindOne(s => s.Name == createScreenMessage.Screen.Name);
            if (existingScreen != null)
            {
                p.AddChatMessage(
                    $"Failed to create a new screen. A screen with name \"{createScreenMessage.Screen.Name}\" already exists.",
                    new[] {255, 0, 0});
                return;
            }

            var id = this.screenCollection.Insert(createScreenMessage.Screen);
            createScreenMessage.Screen.Id = id;

            this.CreateScreen.Invoke(p, createScreenMessage);

            var screenListMessage = new ScreenListMessage(this.GetScreensList());
            this.GetScreenList.Invoke(null, screenListMessage);
        }

        private void OnDeleteScreen(string screenName)
        {
            var count = this.screenCollection.Delete(s => s.Name == screenName);
            if (count == 0)
            {
                Logger.Error($"failed to delete screen. screen {screenName} not found.");
                return;
            }

            var deleteScreenMessage = new DeleteScreenMessage(screenName);

            this.DeleteScreen.Invoke(null, deleteScreenMessage);

            var screenListMessage = new ScreenListMessage(this.GetScreensList());
            this.GetScreenList.Invoke(null, screenListMessage);
        }

        private void OnDeleteScreen(Player p, DeleteScreenMessage deleteScreenMessage)
        {
            if (!p.IsAceAllowed(Permission.Delete))
            {
                p.AddChatMessage(
                    $"You are not permitted to delete a screen. missing ace: ${Permission.Delete}",
                    new[] {255, 0, 0});
                return;
            }

            var count = this.screenCollection.Delete(s => s.Name == deleteScreenMessage.ScreenName);
            if (count == 0)
            {
                p.AddChatMessage(
                    $"Screen Deletion failed: Screen \"{deleteScreenMessage.ScreenName}\" not found.",
                    new[] {255, 0, 0});
                return;
            }

            this.DeleteScreen.Invoke(null, deleteScreenMessage);

            var screenListMessage = new ScreenListMessage(this.GetScreensList());
            this.GetScreenList.Invoke(null, screenListMessage);
        }

        private void OnEditScreen(string jsonScreen)
        {
            var screen = JsonConvert.DeserializeObject<Screen>(jsonScreen);
            if (screen == null)
            {
                Logger.Error("Failed to edit screen. Received empty or wrong argument");
                return;
            }

            if (!screen.IsValid)
            {
                Logger.Error("Failed to edit screen. Received invalid screen argument");
                return;
            }

            var found = this.screenCollection.Update(screen);
            if (!found)
            {
                Logger.Error($"Editing failed. screen \"{screen.Name}\" not found.");
                return;
            }

            var editScreenMessage = new EditScreenMessage(screen);

            this.EditScreen.Invoke(null, editScreenMessage);

            var screenListMessage = new ScreenListMessage(this.GetScreensList());
            this.GetScreenList.Invoke(null, screenListMessage);
        }

        private void OnEditScreen(Player p, EditScreenMessage editScreenMessage)
        {
            if (!p.IsAceAllowed(Permission.Edit))
            {
                p.AddChatMessage(
                    $"You are not permitted to edit a screen. missing ace: {Permission.Edit}",
                    new[] {255, 0, 0});
                return;
            }

            var found = this.screenCollection.Update(editScreenMessage.Screen);
            if (!found)
            {
                p.AddChatMessage($"Editing failed. screen \"{editScreenMessage.Screen.Name}\" not found.");
                return;
            }

            this.EditScreen.Invoke(null, editScreenMessage);

            var screenListMessage = new ScreenListMessage(this.GetScreensList());
            this.GetScreenList.Invoke(null, screenListMessage);
        }

        private string OnGetScreenList()
        {
            var screens = this.GetScreensList();
            return JsonConvert.SerializeObject(screens);
        }

        private void OnGetScreenList(Player p, ScreenListMessage unused)
        {
            var screens = this.GetScreensList();

            var screenListMessage = new ScreenListMessage(screens);
            this.GetScreenList.Invoke(p, screenListMessage);
        }
    }
}