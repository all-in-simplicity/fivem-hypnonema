namespace Hypnonema.Server.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CitizenFX.Core;

    using Hypnonema.Server.Communications;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Models;

    using LiteDB;

    using Newtonsoft.Json;

    using Logger = Hypnonema.Server.Utils.Logger;

    public sealed class ScreenStorageManager
    {
        private LiteCollection<Screen> screenCollection;

        public NetworkMethod<Screen> CreateScreen { get; private set; }

        public NetworkMethod<string> DeleteScreen { get; private set; }

        public NetworkMethod<Screen> EditScreen { get; private set; }

        public NetworkMethod<IList<Screen>> GetScreenList { get; private set; }

        public bool IsInitialized { get; private set; }

        public void Initialize(LiteCollection<Screen> screenCollection)
        {
            if (this.IsInitialized) return;

            this.screenCollection = screenCollection;

            this.CreateScreen = new NetworkMethod<Screen>(Events.CreateScreen, this.OnCreateScreen);
            this.DeleteScreen = new NetworkMethod<string>(Events.DeleteScreen, this.OnDeleteScreen);
            this.EditScreen = new NetworkMethod<Screen>(Events.EditScreen, this.OnEditScreen);
            this.GetScreenList = new NetworkMethod<IList<Screen>>(Events.GetScreenList, this.OnGetScreenList);

            BaseServer.Self.AddExport(Events.GetScreenList, new Func<string>(this.OnGetScreenList));
            BaseServer.Self.AddExport(Events.CreateScreen, new Action<string>(this.OnCreateScreen));
            BaseServer.Self.AddExport(Events.EditScreen, new Action<string>(this.OnEditScreen));
            BaseServer.Self.AddExport(Events.DeleteScreen, new Action<string>(this.OnDeleteScreen));

            this.IsInitialized = true;
        }

        private IList<Screen> GetScreensList()
        {
            return this.screenCollection.FindAll().ToList();
        }

        private void OnCreateScreen(string jsonScreen)
        {
            var screen = JsonConvert.DeserializeObject<Screen>(jsonScreen);
            if (screen == null)
            {
                Logger.Error($"failed to create new screen. invalid argument");
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

            this.GetScreenList.Invoke(null, this.GetScreensList());
        }

        private void OnCreateScreen(Player p, Screen screen)
        {
            if (!p.IsAceAllowed(Permission.Create))
            {
                p.AddChatMessage(
                    $"You are not permitted to create a new screen. missing ace: ${Permission.Create}",
                    new[] { 255, 0, 0 });
                return;
            }

            var existingScreen = this.screenCollection.FindOne(s => s.Name == screen.Name);
            if (existingScreen != null)
            {
                p.AddChatMessage(
                    $"Failed to create a new screen. A screen with name \"{screen.Name}\" already exists.",
                    new[] { 255, 0, 0 });
                return;
            }

            var id = this.screenCollection.Insert(screen);
            screen.Id = id;

            this.CreateScreen.Invoke(p, screen);
            this.GetScreenList.Invoke(null, this.GetScreensList());
        }

        private void OnDeleteScreen(string screenName)
        {
            var count = this.screenCollection.Delete(s => s.Name == screenName);
            if (count == 0)
            {
                Logger.Error($"failed to delete screen. screen {screenName} not found.");
                return;
            }

            this.DeleteScreen.Invoke(null, screenName);
            this.GetScreenList.Invoke(null, this.GetScreensList());
        }

        private void OnDeleteScreen(Player p, string screenName)
        {
            if (!p.IsAceAllowed(Permission.Delete))
            {
                p.AddChatMessage(
                    $"You are not permitted to delete a screen. missing ace: ${Permission.Delete}",
                    new[] { 255, 0, 0 });
                return;
            }

            var count = this.screenCollection.Delete(s => s.Name == screenName);
            if (count == 0)
            {
                p.AddChatMessage($"Screen Deletion failed: Screen \"{screenName}\" not found.", new[] { 255, 0, 0 });
                return;
            }

            this.DeleteScreen.Invoke(null, screenName);
            this.GetScreenList.Invoke(null, this.GetScreensList());
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

            this.EditScreen.Invoke(null, screen);
            this.GetScreenList.Invoke(null, this.GetScreensList());
        }

        private void OnEditScreen(Player p, Screen screen)
        {
            if (!p.IsAceAllowed(Permission.Edit))
            {
                p.AddChatMessage(
                    $"You are not permitted to edit a screen. missing ace: {Permission.Edit}",
                    new[] { 255, 0, 0 });
                return;
            }

            var found = this.screenCollection.Update(screen);
            if (!found)
            {
                p.AddChatMessage($"Editing failed. screen \"{screen.Name}\" not found.");
                return;
            }

            this.EditScreen.Invoke(null, screen);
            this.GetScreenList.Invoke(null, this.GetScreensList());
        }

        private string OnGetScreenList()
        {
            var screens = this.GetScreensList();
            return JsonConvert.SerializeObject(screens);
        }

        private void OnGetScreenList(Player p, IList<Screen> unused)
        {
            var screens = this.GetScreensList();

            this.GetScreenList.Invoke(p, screens);
        }
    }
}