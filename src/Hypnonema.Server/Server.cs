namespace Hypnonema.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Shared;

    using NHttp;

    public class Server : BaseScript
    {
        private HttpServer server;

        private bool useHttpServer;

        public Server()
        {
            this.EventHandlers["onResourceStart"] += new Action<string>(this.OnResourceStart);
            this.EventHandlers["onResourceStop"] += new Action<string>(this.OnResourceStop);
        }

        private void ServerOnRequestReceived(object sender, HttpRequestEventArgs e)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
            using (var fs = File.OpenRead(Path.Combine(path, "index.html")))
            {
                e.Response.Headers.Add("content-type","text/html; charset=UTF-8");
                e.Response.Headers.Add("x-content-type-options","nosniff");
                e.Response.Headers.Add("x-xss-protection","1; mode=block");
                e.Response.Headers.Add("x-frame-options","DENY");
                e.Response.Headers.Add("cache-control","no-store no-cache, must-revalidate");
                e.Response.Headers.Add("pragma","no-cache");
                e.Response.Headers.Add("Server", "hypnonema");

                byte[] buffer = new byte[1024 * 32];
                int nbytes;
                while ((nbytes = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    e.Response.OutputStream.Write(buffer, 0, nbytes);
                }
                e.Context.Response.OutputStream.Flush();
                e.Context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
        }

        private void OnResourceStop(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;

            if (this.useHttpServer)
            {
                Log.WriteLine("Stopping HTTP-Server..");
                try
                {
                    this.server.Stop();
                    this.server.Dispose();
                }
                catch (Exception e)
                {
                    Log.WriteLine($"Error during shutdown of HTTP-Server: {e.Message}");
                }
            }
        }

        private void OnResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName)
            {
                return;
            }

            bool httpServer;
            if (bool.TryParse(API.GetResourceMetadata(resourceName, "hypnonema_http_server", 0), out httpServer))
            {
                this.useHttpServer = httpServer;
            }
            else
            {
                this.useHttpServer = false;
                Log.WriteLine($"^8Error: Failed to parse hypnonema_http_server. Using default value: {this.useHttpServer.ToString().ToLowerInvariant()}. Please recheck your config!");
            }

            if (this.useHttpServer)
            {
                IPAddress listenAddr;
                if (!IPAddress.TryParse(API.GetResourceMetadata(resourceName, "hypnonema_listen_addr", 0), out listenAddr))
                {
                    listenAddr = IPAddress.Loopback;
                    Log.WriteLine($"^8Error: Failed to parse hypnonema_listen_addr. Using default value: {listenAddr.ToString()}. Please recheck your config!");
                }

                int port;
                if (!int.TryParse(API.GetResourceMetadata(resourceName, "hypnonema_listen_port", 0), out port))
                {
                    port = 9414;
                    Log.WriteLine($"^8Error: Failed to parse hypnonema_listen_port. Using default value: {port}. Please recheck your config!");
                }

                Log.WriteLine("^8Warning: Using the built-in HTTP-Server. DO NOT USE THIS FOR PRODUCTION!");

                this.server = new HttpServer
                                  {
                                      EndPoint = new IPEndPoint(listenAddr, port)
                                  };
                this.server.RequestReceived += this.ServerOnRequestReceived;

                try
                {
                    this.server.Start();
                    Log.WriteLine($"Listening on port {port}.");
                }
                catch (Exception e)
                {
                    Log.WriteLine("Error: Failed to start the HTTP-Server.");
                    Log.WriteLine(e.Message);
                }
            }

            API.RegisterCommand("hypnonema", new Action<int, List<object>, string>(this.OnHypnonemaCommand), true);

            this.EventHandlers[ServerEvents.OnPlaybackReceived] +=
                new Action<Player, string, string>(this.OnPlaybackReceived);
            this.EventHandlers[ServerEvents.OnPause] += new Action<Player>(this.OnPause);
            this.EventHandlers[ServerEvents.OnStopVideo] += new Action<Player>(this.OnStopVideo);
            this.EventHandlers[ServerEvents.OnResumeVideo] += new Action<Player>(this.OnResumeVideo);
            this.EventHandlers[ServerEvents.OnSetVolume] += new Action<Player, string>(this.OnSetVolume);
            this.EventHandlers[ServerEvents.OnSetSoundAttenuation] +=
                new Action<Player, float>(this.OnSetSoundAttenuation);
            this.EventHandlers[ServerEvents.OnSetSoundMinDistance] +=
                new Action<Player, float>(this.OnSetSoundMinDistance);
        }

        private void OnSetSoundAttenuation([FromSource] Player player, float f)
        {
            if (API.IsPlayerAceAllowed(player.Handle, "command.hypnonema"))
            {
                BaseScript.TriggerClientEvent(ClientEvents.SetSoundAttenuation, f);
            }
            else
            {
                this.AddChatMessage(player.Handle, "Error: You don't have permissions for: command.hypnonema", new[] { 255, 0, 0 });
            }
        }

        private void OnSetSoundMinDistance([FromSource] Player player, float f)
        {
            if(this.IsPlayerAllowed(player))
            {
                BaseScript.TriggerClientEvent(ClientEvents.SetSoundMinDistance, f);
            }
            else
            {
                this.AddChatMessage(player.Handle, "Error: You don't have permissions for: command.hypnonema", new[] { 255, 0, 0 });
            }
        }

        private bool IsPlayerAllowed(Player player)
        {
            return API.IsPlayerAceAllowed(player.Handle, "command.hypnonema");
        }

        private void OnSetVolume([FromSource] Player player, string volume)
        {
            if (this.IsPlayerAllowed(player))
            {
                BaseScript.TriggerClientEvent(ClientEvents.SetVolume, volume);
            }
            else
            {
                this.AddChatMessage(player.Handle, "Error: You don't have permissions for: command.hypnonema", new[] { 255, 0, 0 });
            }
        }

        private void OnResumeVideo([FromSource] Player player)
        {
            if (this.IsPlayerAllowed(player))
            {
                BaseScript.TriggerClientEvent(ClientEvents.ResumeVideo);
            }
        }

        private void OnStopVideo([FromSource] Player player)
        {
            if (this.IsPlayerAllowed(player))
            {
                BaseScript.TriggerClientEvent(ClientEvents.StopVideo);
            }
            else
            {
                this.AddChatMessage(player.Handle, "Error: You don't have permissions for: command.hypnonema", new[] { 255, 0, 0 });
            }
        }

        private void OnPause([FromSource] Player player)
        {
            if (this.IsPlayerAllowed(player))
            {
                BaseScript.TriggerClientEvent(ClientEvents.PauseVideo);
            }
            else
            {
                this.AddChatMessage(player.Handle, "Error: You don't have permissions for: command.hypnonema", new[] { 255, 0, 0 });
            }

        }

        private void OnPlaybackReceived([FromSource] Player player, string videoURL, string videoType)
        {
            if (this.IsPlayerAllowed(player))
            {
                Log.WriteLine($"Playing {videoType}:{videoURL}");
                BaseScript.TriggerClientEvent(ClientEvents.PlayVideo, videoURL, videoType);
            }
            else
            {
                this.AddChatMessage(player.Handle, "Error: You don't have permissions for: command.hypnonema", new[] { 255, 0, 0 });
            }
        }

        private void OnHypnonemaCommand(int source, List<object> args, string raw)
        {
            var p = Players[source];
            p.TriggerEvent("chat:addMessage", new { color = new[] { 0, 128, 128} , args = new[] { "[Hypnonema]", "Showing Hypnonema Window" } });
            p.TriggerEvent(ClientEvents.ShowNUI);
        }
        private void AddChatMessage(string source, string message, int[] color = null, bool multiline = true)
        {
            if (color == null)
            {
                color = new[] { 0, 128, 128 };
            }

            var p = Players[source];
            p.TriggerEvent("chat:addMessage",
                new
                    {
                        color,
                        args = new[] { "[Hypnonema]", $"{message}" }
                    });
                
        }
    }
}