namespace Hypnonema.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Server.Utils;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Models;

    using LiteDB;

    using Newtonsoft.Json;

    using Logger = Hypnonema.Server.Utils.Logger;

    public class ServerScript : BaseScript
    {
        public static bool IsLoggingEnabled = true;

        private string cmdName = "hypnonema";

        private string connectionString = "Filename=hypnonema.db";

        private LiteDatabase database;

        private List<ScreenDuiState> lastKnownState;

        private LiteCollection<Screen> screenCollection;

        public ServerScript()
        {
            this.RegisterEventHandler("onResourceStart", new Action<string>(this.OnResourceStart));
            this.RegisterEventHandler("onResourceStop", new Action<string>(this.OnResourceStop));
            this.RegisterEventHandler(ServerEvents.OnInitialize, new Action<Player>(this.OnClientInitialize));
            this.RegisterEventHandler(ServerEvents.OnEditScreen, new Action<Player, string>(this.OnEditScreen));
            this.RegisterEventHandler(ServerEvents.OnCloseScreen, new Action<Player, string>(this.OnCloseScreen));
            this.RegisterEventHandler(ServerEvents.OnDeleteScreen, new Action<Player, string>(this.OnDeleteScreen));
            this.RegisterEventHandler(ServerEvents.OnPause, new Action<Player, string>(this.OnPause));
            this.RegisterEventHandler(ServerEvents.OnStateTick, new Action<Player, string>(this.OnStateTick));
            this.RegisterEventHandler(ServerEvents.OnResumeVideo, new Action<Player, string>(this.OnResume));
            this.RegisterEventHandler(
                ServerEvents.OnPlaybackReceived,
                new Action<Player, string, string>(this.OnPlaybackReceived));
            this.RegisterEventHandler(ServerEvents.OnCreateScreen, new Action<Player, string>(this.OnCreateScreen));
            this.RegisterEventHandler(ServerEvents.OnSetVolume, new Action<Player, float, string>(this.OnSetVolume));
            this.RegisterEventHandler(ServerEvents.OnStopVideo, new Action<Player, string>(this.OnStopVideo));
        }

        private static void RegisterCommand(string cmdName, InputArgument handler, bool restricted)
        {
            try
            {
                API.RegisterCommand(cmdName, handler, restricted);
            }
            catch (Exception e)
            {
                Logger.WriteLine(
                    $"failed to register the command {cmdName}. Error: {e.Message}",
                    Logger.LogLevel.Error);
                throw;
            }
        }

        private void AddChatMessage(Player player, string message, int[] color = null)
        {
            if (color == null) color = new[] { 0, 128, 128 };

            player.TriggerEvent("chat:addMessage", new { color, args = new[] { "[Hypnonema]", $"{message}" } });
        }

        private bool IsPlayerAllowed(Player player)
        {
            return API.IsPlayerAceAllowed(player.Handle, $"command.{this.cmdName}");
        }

        /// <summary>
        ///     Used to initialize the Client.
        ///     Triggered from Client at the first Tick.
        /// </summary>
        /// <param name="p"></param>
        private void OnClientInitialize([FromSource] Player p)
        {
            try
            {
                var filesCount = 0;
                var streamDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "stream");

                if (Directory.Exists(streamDirectory))
                {
                    var regExp = new Regex(@"hypnonema_texture_renderer\d+\.+gfx");
                    filesCount = Directory.GetFiles(streamDirectory, "*.gfx").Where(path => regExp.IsMatch(path))
                        .ToList().Count;
                }
                else
                {
                    Logger.WriteLine(
                        $"Failed to read stream directory at path \"{streamDirectory}\".",
                        Logger.LogLevel.Warning);
                }

                var q = this.screenCollection.Find(s => s.AlwaysOn);
                p.TriggerEvent(
                    ClientEvents.Initialize,
                    JsonConvert.SerializeObject(q),
                    filesCount,
                    this.IsPlayerAllowed(p),
                    JsonConvert.SerializeObject(this.lastKnownState));
            }
            catch (Exception e)
            {
                Logger.WriteLine($"failed to query database. error: {e.Message}");
            }
        }

        private void OnCloseScreen([FromSource] Player p, string screenName)
        {
            if (!this.IsPlayerAllowed(p))
            {
                this.AddChatMessage(p, "Error: You don't have sufficient permissions.", new[] { 255, 0, 0 });
                return;
            }

            TriggerClientEvent(ClientEvents.CloseScreen, screenName);
        }

        private void OnCreateScreen([FromSource] Player p, string jsonScreen)
        {
            if (!this.IsPlayerAllowed(p))
            {
                this.AddChatMessage(
                    p,
                    $"Error: You don't have permissions for: command.{this.cmdName}",
                    new[] { 255, 0, 0 });
                return;
            }

            var screen = JsonConvert.DeserializeObject<Screen>(jsonScreen);
            try
            {
                this.screenCollection.EnsureIndex(s => s.Name, true);
                this.screenCollection.EnsureIndex(s => s.AlwaysOn);

                var id = this.screenCollection.Insert(screen);
                screen.Id = id;

                p.TriggerEvent(ClientEvents.CreatedScreen, JsonConvert.SerializeObject(screen));
            }
            catch (Exception e)
            {
                Logger.WriteLine($"failed to create screen. Message: {e.Message}", Logger.LogLevel.Error);
                this.AddChatMessage(p, "Failed to create screen. See server-log for info");
                throw;
            }
        }

        private void OnDeleteScreen([FromSource] Player p, string screenName)
        {
            if (!this.IsPlayerAllowed(p))
            {
                this.AddChatMessage(p, "Error: You don't have sufficient permissions.", new[] { 255, 0, 0 });
                return;
            }

            var screen = this.screenCollection.FindOne(s => s.Name == screenName);
            var success = this.screenCollection.Delete(screen?.Id);

            if (success) TriggerClientEvent(ClientEvents.DeletedScreen, screenName);
            else this.AddChatMessage(p, $"Error: Screen \"{screenName}\" not found.", new[] { 255, 0, 0 });
        }

        private void OnEditScreen([FromSource] Player p, string jsonScreen)
        {
            if (!this.IsPlayerAllowed(p)) return;

            var screen = JsonConvert.DeserializeObject<Screen>(jsonScreen);

            if (this.screenCollection.Update(screen))
                TriggerClientEvent(ClientEvents.EditedScreen, JsonConvert.SerializeObject(screen));
            else this.AddChatMessage(p, "Error: screen not found.");
        }

        private void OnHypnonemaCommand(int source, List<object> args, string raw)
        {
            var p = this.Players[source];
            var isAceAllowed = this.IsPlayerAllowed(p);
            List<Screen> screens;

            try
            {
                screens = this.screenCollection.FindAll().ToList();
            }
            catch (Exception e)
            {
                this.AddChatMessage(p, "Failed to read database. See server-log for more info");
                Logger.WriteLine($"Failed to read database. Error: {e.Message}", Logger.LogLevel.Error);
                throw;
            }

            p.TriggerEvent(ClientEvents.ShowNUI, isAceAllowed, JsonConvert.SerializeObject(screens));
            this.AddChatMessage(p, "Showing Window");
        }

        private void OnPause([FromSource] Player p, string screenName)
        {
            if (!this.IsPlayerAllowed(p))
            {
                this.AddChatMessage(p, "Error: You don't have sufficient permissions", new[] { 255, 0, 0 });
                return;
            }

            TriggerClientEvent(ClientEvents.PauseVideo, screenName);
        }

        private void OnPlaybackReceived([FromSource] Player p, string videoUrl, string screenName)
        {
            if (!this.IsPlayerAllowed(p))
            {
                this.AddChatMessage(
                    p,
                    $"Error: You don't have permissions for: command.{this.cmdName}",
                    new[] { 255, 0, 0 });
                return;
            }

            Screen screen;
            try
            {
                screen = this.screenCollection.FindOne(s => s.Name == screenName);
            }
            catch (Exception e)
            {
                Logger.WriteLine(
                    $"playback attempt failed because of failing to query database. Error: ${e.Message}",
                    Logger.LogLevel.Error);
                return;
            }

            if (screen == null)
            {
                this.AddChatMessage(p, $"screen {screenName} not found. aborting playback");
                Logger.WriteLine($"screen {screenName} not found. aborting playback", Logger.LogLevel.Error);
                return;
            }

            TriggerClientEvent(ClientEvents.PlayVideo, videoUrl, JsonConvert.SerializeObject(screen));

            Logger.WriteLine($"playing {videoUrl} on screen {screenName}", Logger.LogLevel.Information);
        }

        private void OnResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;

            this.connectionString = ConfigReader.GetConfigKeyValue(
                resourceName,
                "hypnonema_db_connString",
                0,
                this.connectionString);

            try
            {
                this.database = new LiteDatabase(this.connectionString);
                this.screenCollection = this.database.GetCollection<Screen>("screens");
            }
            catch (Exception e)
            {
                Logger.WriteLine($"failed to open database. error: {e.Message}", Logger.LogLevel.Error);
                throw;
            }

            this.cmdName = ConfigReader.GetConfigKeyValue(resourceName, "hypnonema_command_name", 0, "hypnonema")
                .Replace(" ", string.Empty);

            if (this.cmdName != "hypnonema")
                Logger.WriteLine(
                    $"Using {this.cmdName} as command name. Type /{this.cmdName} to open the NUI window.",
                    Logger.LogLevel.Information);

            var loggingEnabled = ConfigReader.GetConfigKeyValue(resourceName, "hypnonema_logging_enabled", 0, false);
            IsLoggingEnabled = loggingEnabled;

            RegisterCommand(this.cmdName, new Action<int, List<object>, string>(this.OnHypnonemaCommand), true);
        }

        private void OnResourceStop(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;

            this.database?.Dispose();
        }

        private void OnResume([FromSource] Player p, string screenName)
        {
            if (!this.IsPlayerAllowed(p))
            {
                this.AddChatMessage(p, "Error: You don't have sufficient permissions.", new[] { 255, 0, 0 });
                return;
            }

            TriggerClientEvent(ClientEvents.ResumeVideo, screenName);
        }

        private void OnSetVolume([FromSource] Player p, float volume, string screenName)
        {
            if (this.IsPlayerAllowed(p)) TriggerClientEvent(ClientEvents.SetVolume, volume, screenName);
        }

        private void OnStateTick([FromSource] Player p, string jsonState)
        {
            if (!this.IsPlayerAllowed(p)) return;

            var states = JsonConvert.DeserializeObject<List<DuiState>>(jsonState);
            var lastState = (from duiState in states
                             let screen = this.screenCollection.FindOne(s => s.Name == duiState.ScreenName)
                             where screen != null
                             select new ScreenDuiState { Screen = screen, State = duiState }).ToList();

            if (lastState.Any()) this.lastKnownState = lastState;
        }

        private void OnStopVideo([FromSource] Player p, string screenName)
        {
            if (this.IsPlayerAllowed(p)) TriggerClientEvent(ClientEvents.StopVideo, screenName);
        }

        private void RegisterEventHandler(string eventName, Delegate actionDelegate)
        {
            this.EventHandlers.Add(eventName, actionDelegate);
        }
    }
}