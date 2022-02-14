namespace Hypnonema.Server.Screens
{
    using System.Collections.Generic;
    using System.Linq;

    using CitizenFX.Core;

    using Hypnonema.Server.Communications;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Models;

    using LiteDB;

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

            this.IsInitialized = true;
        }

        private IList<Screen> GetScreensList()
        {
            return this.screenCollection.FindAll().ToList();
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

        private void OnGetScreenList(Player p, IList<Screen> unused)
        {
            var screens = this.GetScreensList();

            this.GetScreenList.Invoke(p, screens);
        }
    }
}