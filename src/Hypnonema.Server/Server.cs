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
    using Hypnonema.Shared.Events;
    using Hypnonema.Shared.Models;

    using LiteDB;

    using Newtonsoft.Json;

    using Logger = Hypnonema.Server.Utils.Logger;

    public class Server : BaseScript
    {
        public static bool IsLoggingEnabled = true;

        private string cmdName = "hypnonema";

        private bool isCommandRestricted = true;

        private string connectionString = "Filename=hypnonema.db";

        private LiteDatabase database;

        private ScreenDuiStateList lastKnownState = new ScreenDuiStateList();

        private LiteCollection<Screen> screenCollection;

        private int syncInterval = 5000;

        private static void RegisterCommand(string cmdName, InputArgument handler, bool restricted)
        {
            API.RegisterCommand(cmdName, handler, restricted);
        }

        private void AddChatMessage(Player player, string message, int[] color = null)
        {
            if (color == null) color = new[] { 0, 128, 128 };

            player.TriggerEvent("chat:addMessage", new { color, args = new[] { "[Hypnonema]", $"{message}" } });
        }

        private bool IsPlayerAllowed(Player player)
        {
            return !this.isCommandRestricted || API.IsPlayerAceAllowed(player.Handle, $"command.{this.cmdName}");
        }

        [EventHandler(ServerEvents.OnInitialize)]
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

                // if we didn't receive a syncInterval for 3 times, we flush the lastKnownState
                if (DateTime.UtcNow > this.lastKnownState.Timestamp + new TimeSpan(0, 0, 0, 0, this.syncInterval * 3))
                    this.lastKnownState = new ScreenDuiStateList();

                p.TriggerEvent(
                    ClientEvents.Initialize,
                    JsonConvert.SerializeObject(q),
                    filesCount,
                    this.IsPlayerAllowed(p),
                    JsonConvert.SerializeObject(this.lastKnownState.StateList));
            }
            catch (Exception e)
            {
                Logger.WriteLine($"failed to query database. error: {e.Message}");
            }
        }

        [EventHandler(ServerEvents.OnCloseScreen)]
        private void OnCloseScreen([FromSource] Player p, string screenName)
        {
            if (!this.IsPlayerAllowed(p))
            {
                this.AddChatMessage(p, "Error: You don't have sufficient permissions.", new[] { 255, 0, 0 });
                return;
            }

            TriggerClientEvent(ClientEvents.CloseScreen, screenName);
        }

        [EventHandler(ServerEvents.OnCreateScreen)]
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

        [EventHandler(ServerEvents.OnDeleteScreen)]
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

        [EventHandler(ServerEvents.OnEditScreen)]
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

        [EventHandler(ServerEvents.OnPause)]
        private void OnPause([FromSource] Player p, string screenName)
        {
            if (!this.IsPlayerAllowed(p))
            {
                this.AddChatMessage(p, "Error: You don't have sufficient permissions", new[] { 255, 0, 0 });
                return;
            }

            TriggerClientEvent(ClientEvents.PauseVideo, screenName);
        }

        [EventHandler(ServerEvents.OnPlaybackReceived)]
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

        [EventHandler("onResourceStart")]
        private void OnResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;

            this.connectionString = ConfigReader.GetConfigKeyValue(
                resourceName,
                "hypnonema_db_connString",
                0,
                this.connectionString);
            this.cmdName = ConfigReader.GetConfigKeyValue(resourceName, "hypnonema_command_name", 0, this.cmdName)
                .Replace(" ", string.Empty);
            this.syncInterval = ConfigReader.GetConfigKeyValue(
                resourceName,
                "hypnonema_sync_interval",
                0,
                this.syncInterval);
            this.isCommandRestricted = ConfigReader.GetConfigKeyValue(
                resourceName,
                "hypnonema_restrict_command",
                0,
                this.isCommandRestricted);
            IsLoggingEnabled = ConfigReader.GetConfigKeyValue(
                resourceName,
                "hypnonema_logging_enabled",
                0,
                IsLoggingEnabled);

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

            // Create Example Screen if Database is empty
            this.PopulateDatabaseIfEmpty();

            if (this.cmdName != "hypnonema")
                Logger.WriteLine(
                    $"Using '{this.cmdName}' as command name. Type /{this.cmdName} to open the NUI window.",
                    Logger.LogLevel.Information);

            if (!this.isCommandRestricted)
                Logger.WriteLine($"Command '{this.cmdName}' is NOT restricted", Logger.LogLevel.Information);

            RegisterCommand(
                this.cmdName,
                new Action<int, List<object>, string>(this.OnHypnonemaCommand),
                this.isCommandRestricted);
        }

        [EventHandler("onResourceStop")]
        private void OnResourceStop(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;

            this.database?.Dispose();
        }

        [EventHandler(ServerEvents.OnResumeVideo)]
        private void OnResume([FromSource] Player p, string screenName)
        {
            if (!this.IsPlayerAllowed(p))
            {
                this.AddChatMessage(p, "Error: You don't have sufficient permissions.", new[] { 255, 0, 0 });
                return;
            }

            TriggerClientEvent(ClientEvents.ResumeVideo, screenName);
        }

        [EventHandler(ServerEvents.OnSetVolume)]
        private void OnSetVolume([FromSource] Player p, float volume, string screenName)
        {
            if (this.IsPlayerAllowed(p)) TriggerClientEvent(ClientEvents.SetVolume, volume, screenName);
        }

        [EventHandler(ServerEvents.OnStateTick)]
        private void OnStateTick([FromSource] Player p, string jsonState)
        {
            if (!this.IsPlayerAllowed(p)) return;

            var states = JsonConvert.DeserializeObject<List<DuiState>>(jsonState);
            var lastState = (from duiState in states
                             let screen = this.screenCollection.FindOne(s => s.Name == duiState.ScreenName)
                             where screen != null
                             select new ScreenDuiState { Screen = screen, State = duiState }).ToList();

            if (!lastState.Any()) return;
            this.lastKnownState.StateList = lastState;
            this.lastKnownState.Timestamp = DateTime.UtcNow;
        }

        [EventHandler(ServerEvents.OnStopVideo)]
        private void OnStopVideo([FromSource] Player p, string screenName)
        {
            if (this.IsPlayerAllowed(p)) TriggerClientEvent(ClientEvents.StopVideo, screenName);
        }

        [EventHandler(ServerEvents.OnToggleRepeat)]
        private void OnToggleRepeat([FromSource] Player p, string screenName)
        {
            if (this.IsPlayerAllowed(p)) TriggerClientEvent(ClientEvents.ToggleRepeat, screenName);
        }

        private void PopulateDatabaseIfEmpty()
        {
            if (this.screenCollection.Count() >= 1) return;

            var exampleScreen = new Screen()
                                    {
                                        AlwaysOn = false,
                                        BrowserSettings =
                                            new DuiBrowserSettings()
                                                {
                                                    GlobalVolume = 100f,
                                                    Is3DAudioEnabled = true,
                                                    SoundAttenuation = 10f,
                                                    SoundMaxDistance = 200f,
                                                    SoundMinDistance = 10f
                                                },
                                        Is3DRendered = true,
                                        Name = "Hypnonema Example Screen",
                                        PositionalSettings = new PositionalSettings()
                                                                 {
                                                                     PositionX = -1678.949f,
                                                                     PositionY = -928.3431f,
                                                                     PositionZ = 20.6290932f,
                                                                     RotationX = 0f,
                                                                     RotationY = 0f,
                                                                     RotationZ = -140f,
                                                                     ScaleX = 0.969999969f,
                                                                     ScaleY = 0.484999985f,
                                                                     ScaleZ = -0.1f
                                                                 }
                                    };

            try
            {
                this.screenCollection.Insert(exampleScreen);
            }
            catch (Exception e)
            {
                Logger.WriteLine($"Failed to create example screen: {e.Message}", Logger.LogLevel.Error);
                throw;
            }

            Logger.WriteLine($"Created example screen.", Logger.LogLevel.Information);
        }
    }
}