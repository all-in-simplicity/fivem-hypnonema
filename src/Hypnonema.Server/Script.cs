namespace Hypnonema.Server
{
    using System;
    using System.Collections.Generic;
    
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using EmbedIO;
    using EmbedIO.Actions;
    using EmbedIO.Files;

    using Hypnonema.Server.Utils;
    using Hypnonema.Shared;

    using Swan.Logging;

    public class Script : BaseScript
    {
        private string cmdName = "hypnonema";
        
        #region HTTP-Server related

        private bool useHttpServer = false;

        private string url = $"http://{IPAddress.Loopback.ToString()}:9414/";

        private const bool useFileCache = true;

        private static string HtmlRootPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");

        private WebServer server;

        private CancellationTokenSource ctSource;

        #endregion

        public Script()
        {
            this.EventHandlers["onResourceStart"] += new Action<string>(this.OnResourceStart);
            this.EventHandlers["onResourceStop"] += new Action<string>(this.OnResourceStop);

            Log.WriteLine($"You are using hypnonema version: {this.GetHypnonemaVersion()}");
        }

        private void OnResourceStop(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;

            if (this.useHttpServer)
            {
                Log.WriteLine("Stopping HTTP-Server..");
                try
                {
                    this.ctSource.Cancel();
                    this.server.Dispose();
                }
                catch (Exception e)
                {
                    Log.WriteLine($"^8Error during shutdown of HTTP-Server: {e.Message}");
                }
            }
        }

        private void OnResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;

            this.useHttpServer = ConfigReader.GetConfigKeyValue<bool>(resourceName, "hypnonema_http_server", 0, false);
            this.cmdName = ConfigReader
                .GetConfigKeyValue<string>(resourceName, "hypnonema_command_name", 0, "hypnonema")
                .Replace(" ", string.Empty);

            if (this.useHttpServer)
            {
                Logger.UnregisterLogger<ConsoleLogger>(); // unregister EmbedIO Logger
                var listenAddr = ConfigReader.GetConfigKeyValue<string>(
                    resourceName,
                    "hypnonema_listen_addr",
                    0,
                    IPAddress.Loopback.ToString());

                var port = ConfigReader.GetConfigKeyValue<int>(resourceName, "hypnonema_listen_port", 0, 9414);

                this.ctSource = new CancellationTokenSource();
                this.url = $"http://{listenAddr}:{port}";
                this.Tick += this.OnFirstTick;
            }


            RegisterCommand(this.cmdName, new Action<int, List<object>, string>(this.OnHypnonemaCommand), true);

            if (this.cmdName != "hypnonema")
                Log.WriteLine($"Using {this.cmdName} as command name. Type /{this.cmdName} to open the NUI window");

            this.RegisterEventHandler(
                ServerEvents.OnPlaybackReceived,
                new Action<Player, string, string>(this.OnPlaybackReceived));

            this.RegisterEventHandler(ServerEvents.OnPause, new Action<Player>(this.OnPause));
            this.RegisterEventHandler(ServerEvents.OnStopVideo, new Action<Player>(this.OnStopVideo));
            this.RegisterEventHandler(ServerEvents.OnResumeVideo, new Action<Player>(this.OnResumeVideo));
            this.RegisterEventHandler(ServerEvents.OnSetVolume, new Action<Player, string>(this.OnSetVolume));
            this.RegisterEventHandler(ServerEvents.OnToggleReplay, new Action<Player, bool>(this.OnToggleReplay));

            this.RegisterEventHandler(
                ServerEvents.OnStateTick,
                new Action<Player, bool, float, float, float, string, string>(this.OnStateTick));
            this.RegisterEventHandler(
                ServerEvents.OnSetSoundAttenuation,
                new Action<Player, float>(this.OnSetSoundAttenuation));
            this.RegisterEventHandler(
                ServerEvents.OnSetSoundMinDistance,
                new Action<Player, float>(this.OnSetSoundMinDistance));
        }

        private void OnToggleReplay([FromSource] Player player, bool replay)
        {
            if (this.IsPlayerAllowed(player))
            {
                TriggerClientEvent(ClientEvents.ToggleReplay, replay);
            }
            else
                this.AddChatMessage(
                    player,
                    $"Error: You don't have permissions for: command.{this.cmdName}",
                    new[] { 255, 0, 0 });
        }

        private string GetHypnonemaVersion()
        {
            var attribute = (AssemblyFileVersionAttribute)Assembly
               .GetExecutingAssembly()
               .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true)
               .Single();
           return attribute.Version;
        }


        private async Task OnFirstTick()
        {
            this.Tick -= this.OnFirstTick;

            this.server = CreateWebServer(this.url);

            Log.WriteLine("^8Warning: Using the built-in HTTP-Server. DO NOT USE THIS FOR PRODUCTION!");
            await this.server.RunAsync(this.ctSource.Token).ConfigureAwait(false);
        }

        private static void RegisterCommand(string cmdName, InputArgument handler, bool restricted)
        {
            try
            {
                API.RegisterCommand(cmdName, handler, restricted);
            }
            catch (Exception e)
            {
                Log.WriteLine($"^8Error: Failed to register the command {cmdName}");
                throw;
            }
        }

        private static WebServer CreateWebServer(string url)
        {
            var server = new WebServer(o => o.WithUrlPrefix(url).WithMode(HttpListenerMode.EmbedIO))
                .WithStaticFolder("/", HtmlRootPath, true, m => m.WithContentCaching(useFileCache)).WithModule(
                    new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })));

            return server;
        }

        private void OnSetSoundAttenuation([FromSource] Player player, float f)
        {
            if (this.IsPlayerAllowed(player)) TriggerClientEvent(ClientEvents.SetSoundAttenuation, f);
            else
                this.AddChatMessage(
                    player,
                    $"Error: You don't have permissions for: command.{this.cmdName}",
                    new[] { 255, 0, 0 });
        }

        private void OnSetSoundMinDistance([FromSource] Player player, float f)
        {
            if (this.IsPlayerAllowed(player)) TriggerClientEvent(ClientEvents.SetSoundMinDistance, f);
            else
                this.AddChatMessage(
                    player,
                    $"Error: You don't have permissions for: command.{this.cmdName}",
                    new[] { 255, 0, 0 });
        }

        private bool IsPlayerAllowed(Player player)
        {
            return API.IsPlayerAceAllowed(player.Handle, $"command.{this.cmdName}");
        }

        private void OnSetVolume([FromSource] Player player, string volume)
        {
            if (this.IsPlayerAllowed(player)) TriggerClientEvent(ClientEvents.SetVolume, volume);
            else
                this.AddChatMessage(
                    player,
                    $"Error: You don't have permissions for: command.{this.cmdName}",
                    new[] { 255, 0, 0 });
        }

        private void OnResumeVideo([FromSource] Player player)
        {
            if (this.IsPlayerAllowed(player)) TriggerClientEvent(ClientEvents.ResumeVideo);
        }

        private void OnStopVideo([FromSource] Player player)
        {
            if (this.IsPlayerAllowed(player)) TriggerClientEvent(ClientEvents.StopVideo);
            else
                this.AddChatMessage(
                    player,
                    $"Error: You don't have permissions for: command.{this.cmdName}",
                    new[] { 255, 0, 0 });
        }

        private void OnPause([FromSource] Player player)
        {
            if (this.IsPlayerAllowed(player)) TriggerClientEvent(ClientEvents.PauseVideo);
            else
                this.AddChatMessage(
                    player,
                    $"Error: You don't have permissions for: command.{this.cmdName}",
                    new[] { 255, 0, 0 });
        }

        private void OnStateTick(
            [FromSource] Player player,
            bool paused,
            float currentTime,
            float duration,
            float remainingTime,
            string currentSource,
            string currentType)
        {
            // we already checked it on client-side, but just to be sure.
            if (this.IsPlayerAllowed(player))
                TriggerClientEvent(
                    ClientEvents.UpdateState,
                    paused,
                    currentTime,
                    duration,
                    remainingTime,
                    currentSource,
                    currentType);
        }

        private void OnPlaybackReceived([FromSource] Player player, string videoURL, string videoType)
        {
            if (this.IsPlayerAllowed(player))
            {
                TriggerClientEvent(ClientEvents.PlayVideo, videoURL, videoType);
                Log.WriteLine($"Playing {videoType}:{videoURL}");
            }
            else
            {
                this.AddChatMessage(
                    player,
                    $"Error: You don't have permissions for: command.{this.cmdName}",
                    new[] { 255, 0, 0 });
            }
        }

        private void OnHypnonemaCommand(int source, List<object> args, string raw)
        {
            var p = this.Players[source];
            this.AddChatMessage(p, "Showing Hypnonema Window");
            p.TriggerEvent(ClientEvents.ShowNUI);
        }

        private void AddChatMessage(Player player, string message, int[] color = null)
        {
            player.TriggerEvent(
                "chat:addMessage",
                new { color = new[] { 0, 128, 128 }, args = new[] { "[Hypnonema]", $"{message}" } });
        }

        private void RegisterEventHandler(string eventName, Delegate actionDelegate)
        {
            this.EventHandlers.Add(eventName, actionDelegate);
        }
    }
}