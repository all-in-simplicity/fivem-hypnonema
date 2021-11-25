namespace Hypnonema.Client
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Client.Graphics;
    using Hypnonema.Client.Players;
    using Hypnonema.Shared.Events;
    using Hypnonema.Shared.Models;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Debug = Hypnonema.Client.Utils.Debug;

    public class ClientScript : BaseScript
    {
        private bool isInitialized;

        private VideoPlayerPool playerPool;

        private int syncInterval = 10000;

        public ClientScript()
        {
            this.RegisterNuiCallback(ClientEvents.OnPlay, this.OnPlay);
            this.RegisterNuiCallback(ClientEvents.OnStopVideo, this.OnStopVideo);
            this.RegisterNuiCallback(ClientEvents.OnHideNUI, this.OnHideNUI);
            this.RegisterNuiCallback(ClientEvents.OnVolumeChange, this.OnVolumeChange);
            this.RegisterNuiCallback(ClientEvents.OnCreateScreen, this.OnCreateScreen);
            this.RegisterNuiCallback(ClientEvents.OnEditScreen, this.OnEditScreen);
            this.RegisterNuiCallback(ClientEvents.OnCloseScreen, this.OnCloseScreen);

            this.RegisterNuiCallback(ClientEvents.OnDeleteScreen, this.OnDeleteScreen);
            this.RegisterNuiCallback(ClientEvents.OnRequestState, this.OnRequestState);
            this.RegisterNuiCallback(ClientEvents.OnPause, this.OnPause);
            this.RegisterNuiCallback(ClientEvents.OnResumeVideo, this.OnResume);
            this.RegisterNuiCallback(ClientEvents.OnSeek, this.OnSeek);
            this.RegisterNuiCallback(ClientEvents.OnToggleRepeat, this.OnToggleRepeat);
        }

        protected void RegisterNuiCallback(
            string msg,
            Func<IDictionary<string, object>, CallbackDelegate, CallbackDelegate> callback)
        {
            API.RegisterNuiCallbackType(msg);

            this.EventHandlers[$"__cfx_nui:{msg}"] += new Action<ExpandoObject, CallbackDelegate>(
                (body, resultCallback) => { callback.Invoke(body, resultCallback); });
        }

        protected async void RegisterNuiCallback(
            string msg,
            Func<IDictionary<string, object>, CallbackDelegate, Task<CallbackDelegate>> callback)
        {
            API.RegisterNuiCallbackType(msg);

            this.EventHandlers[$"__cfx_nui:{msg}"] += new Func<ExpandoObject, CallbackDelegate, Task>(
                async (body, resultCallback) => { await callback.Invoke(body, resultCallback); });

            await Task.FromResult(0);
        }

        private static string GetAssemblyFileVersion()
        {
            var attribute = (AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Single();
            return attribute.Version;
        }

        [EventHandler(ClientEvents.CloseScreen)]
        private void CloseScreen(string screenName)
        {
            this.playerPool.CloseScreen(screenName);
        }

        [EventHandler(ClientEvents.CreatedScreen)]
        private async void CreatedScreen(string jsonScreen)
        {
            var screen = JsonConvert.DeserializeObject<Screen>(jsonScreen);
            if (screen.AlwaysOn)
            {
                var player = await this.playerPool.CreateVideoPlayerAsync(screen);
                this.playerPool.VideoPlayers.Add(player);
            }

            API.SendNuiMessage(
                JsonConvert.SerializeObject(
                    new { type = "HypnonemaNUI.CreatedScreen", screen },
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }

        [EventHandler(ClientEvents.DeletedScreen)]
        private void DeletedScreen(string screenName)
        {
            API.SendNuiMessage(JsonConvert.SerializeObject(new { type = "HypnonemaNUI.DeletedScreen", screenName }));
            this.CloseScreen(screenName);
        }

        [EventHandler(ClientEvents.EditedScreen)]
        private async void EditedScreen(string jsonString)
        {
            var screen = JsonConvert.DeserializeObject<Screen>(jsonString);

            API.SendNuiMessage(
                JsonConvert.SerializeObject(
                    new { type = "HypnonemaNUI.EditedScreen", screen },
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));

            var player = this.playerPool.VideoPlayers.FirstOrDefault(s => s.ScreenName == screen.Name);
            if (player == null) return;

            // we want the current state of the dui browser,
            // so we send a request and wait as long as duiStateResponse is null but max. 5500 ms
            player.Browser.GetState();
            var state = await BrowserStateHelperScript.GetLastState();

            this.playerPool.CloseScreen(player.ScreenName);
            player = await this.playerPool.CreateVideoPlayerAsync(screen);

            // stateResponse can be null ( eg. if waiting time exceeded 5500ms)
            if (state != null)
            {
                await Delay(500);
                player.Browser.Update(state.IsPaused, state.CurrentTime, state.CurrentSource, state.Repeat);
            }

            this.playerPool.VideoPlayers.Add(player);
        }

        [EventHandler("onClientResourceStart")]
        private void OnClientResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;

            var url = API.GetResourceMetadata(resourceName, "hypnonema_url", 0);
            var posterUrl = API.GetResourceMetadata(resourceName, "hypnonema_poster_url", 0);

            if (!int.TryParse(
                    API.GetResourceMetadata(API.GetCurrentResourceName(), "hypnonema_sync_interval", 0),
                    out var syncInterval)) this.syncInterval = 10000;
            this.syncInterval = syncInterval;

            if (!bool.TryParse(
                    API.GetResourceMetadata(resourceName, "hypnonema_logging_enabled", 0),
                    out var isLoggingEnabled)) Debug.IsLoggingEnabled = false;
            Debug.IsLoggingEnabled = isLoggingEnabled;

            this.playerPool = new VideoPlayerPool(url, resourceName, posterUrl);
            this.Tick += this.OnFirstTick;
            this.Tick += this.OnTick;
        }

        private CallbackDelegate OnCloseScreen(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName)) return callback;

            TriggerServerEvent(ServerEvents.OnCloseScreen, screenName);

            callback("OK");
            return callback;
        }

        private CallbackDelegate OnCreateScreen(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName))
            {
                Debug.WriteLine("failed to create screen: screenName is missing");
                return callback;
            }

            var is3DRendered = ArgsReader.GetArgKeyValue<bool>(args, "is3DRendered");

            var screen = new Screen
                             {
                                 AlwaysOn = ArgsReader.GetArgKeyValue<bool>(args, "alwaysOn"),
                                 Name = screenName,
                                 Is3DRendered = is3DRendered,
                                 BrowserSettings =
                                     new DuiBrowserSettings
                                         {
                                             GlobalVolume = ArgsReader.GetArgKeyValue(args, "globalVolume", 100f),
                                             SoundMaxDistance =
                                                 ArgsReader.GetArgKeyValue(args, "soundMaxDistance", 100f),
                                             SoundMinDistance =
                                                 ArgsReader.GetArgKeyValue(args, "soundMinDistance", 15f),
                                             SoundAttenuation = ArgsReader.GetArgKeyValue(args, "soundAttenuation", 5f),
                                             Is3DAudioEnabled = ArgsReader.GetArgKeyValue<bool>(
                                                 args,
                                                 "is3DAudioEnabled")
                                         },
                                 PositionalSettings =
                                     is3DRendered
                                         ? new PositionalSettings
                                               {
                                                   PositionX = ArgsReader.GetArgKeyValue<float>(args, "positionX"),
                                                   PositionY = ArgsReader.GetArgKeyValue<float>(args, "positionY"),
                                                   PositionZ = ArgsReader.GetArgKeyValue<float>(args, "positionZ"),
                                                   RotationX = ArgsReader.GetArgKeyValue<float>(args, "rotationX"),
                                                   RotationY = ArgsReader.GetArgKeyValue<float>(args, "rotationY"),
                                                   RotationZ = ArgsReader.GetArgKeyValue<float>(args, "rotationZ"),
                                                   ScaleX = ArgsReader.GetArgKeyValue<float>(args, "scaleX"),
                                                   ScaleY = ArgsReader.GetArgKeyValue<float>(args, "scaleY"),
                                                   ScaleZ = ArgsReader.GetArgKeyValue<float>(args, "scaleZ")
                                               }
                                         : null,
                                 TargetSettings = is3DRendered
                                                      ? null
                                                      : new RenderTargetSettings
                                                            {
                                                                ModelName =
                                                                    ArgsReader.GetArgKeyValue<string>(
                                                                        args,
                                                                        "modelName"),
                                                                RenderTargetName = ArgsReader.GetArgKeyValue<string>(
                                                                    args,
                                                                    "renderTargetName")
                                                            }
                             };

            // TODO: IsAceAllowed() to reduce server load
            TriggerServerEvent(ServerEvents.OnCreateScreen, JsonConvert.SerializeObject(screen));

            callback("OK");
            return callback;
        }

        private CallbackDelegate OnDeleteScreen(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName))
            {
                Debug.WriteLine("screen deletion failed: no screenName provided.");
                return callback;
            }

            TriggerServerEvent(ServerEvents.OnDeleteScreen, screenName);

            callback("OK");
            return callback;
        }

        private CallbackDelegate OnEditScreen(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            var id = ArgsReader.GetArgKeyValue<int>(args, "id");
            var is3DRendered = ArgsReader.GetArgKeyValue<bool>(args, "is3DRendered");
            var alwaysOn = ArgsReader.GetArgKeyValue<bool>(args, "alwaysOn");
            var modelName = ArgsReader.GetArgKeyValue<string>(args, "modelName");
            var renderTargetName = ArgsReader.GetArgKeyValue<string>(args, "renderTargetName");
            var globalVolume = ArgsReader.GetArgKeyValue(args, "globalVolume", 100f);
            var soundAttenuation = ArgsReader.GetArgKeyValue(args, "soundAttenuation", 5f);
            var soundMinDistance = ArgsReader.GetArgKeyValue(args, "soundMinDistance", 15f);
            var soundMaxDistance = ArgsReader.GetArgKeyValue(args, "soundMaxDistance", 100f);
            var positionX = ArgsReader.GetArgKeyValue<float>(args, "positionX");
            var positionY = ArgsReader.GetArgKeyValue<float>(args, "positionY");
            var positionZ = ArgsReader.GetArgKeyValue<float>(args, "positionZ");
            var rotationX = ArgsReader.GetArgKeyValue<float>(args, "rotationX");
            var rotationY = ArgsReader.GetArgKeyValue<float>(args, "rotationY");
            var rotationZ = ArgsReader.GetArgKeyValue<float>(args, "rotationZ");
            var scaleX = ArgsReader.GetArgKeyValue<float>(args, "scaleX");
            var scaleY = ArgsReader.GetArgKeyValue<float>(args, "scaleY");
            var scaleZ = ArgsReader.GetArgKeyValue<float>(args, "scaleZ");
            var is3DAudioEnabled = ArgsReader.GetArgKeyValue<bool>(args, "is3DAudioEnabled");

            var screen = new Screen
                             {
                                 Id = id,
                                 Name = screenName,
                                 AlwaysOn = alwaysOn,
                                 Is3DRendered = is3DRendered,
                                 BrowserSettings =
                                     new DuiBrowserSettings
                                         {
                                             GlobalVolume = globalVolume,
                                             SoundMaxDistance = soundMaxDistance,
                                             SoundAttenuation = soundAttenuation,
                                             SoundMinDistance = soundMinDistance,
                                             Is3DAudioEnabled = is3DAudioEnabled
                                         },
                                 TargetSettings =
                                     is3DRendered
                                         ? null
                                         : new RenderTargetSettings
                                               {
                                                   ModelName = modelName, RenderTargetName = renderTargetName
                                               },
                                 PositionalSettings = is3DRendered
                                                          ? new PositionalSettings
                                                                {
                                                                    PositionX = positionX,
                                                                    PositionY = positionY,
                                                                    PositionZ = positionZ,
                                                                    RotationX = rotationX,
                                                                    RotationY = rotationY,
                                                                    RotationZ = rotationZ,
                                                                    ScaleX = scaleX,
                                                                    ScaleY = scaleY,
                                                                    ScaleZ = scaleZ
                                                                }
                                                          : null
                             };

            TriggerServerEvent(ServerEvents.OnEditScreen, JsonConvert.SerializeObject(screen));

            callback("OK");
            return callback;
        }

        private async Task OnFirstTick()
        {
            this.Tick -= this.OnFirstTick;

            TriggerServerEvent(ServerEvents.OnInitialize);
            await Task.FromResult(0);
        }

        private CallbackDelegate OnHideNUI(IDictionary<string, object> args, CallbackDelegate callback)
        {
            API.SetNuiFocus(false, false);
            API.SendNuiMessage(JsonConvert.SerializeObject(new { type = "HypnonemaNUI.HideUI" }));

            callback("OK");
            return callback;
        }

        [EventHandler(ClientEvents.Initialize)]
        private async void OnInitialize(string jsonScreens, int rendererLimit, bool isAceAllowed, string lastKnownState)
        {
            TextureRendererPool.MaxActiveScaleforms = rendererLimit;

            var screens = JsonConvert.DeserializeObject<List<Screen>>(jsonScreens);
            foreach (var screen in screens.Where(screen => screen.AlwaysOn))
            {
                var player = await this.playerPool.CreateVideoPlayerAsync(screen);
                this.playerPool.VideoPlayers?.Add(player);
            }

            if (isAceAllowed) this.TriggerStateTick();

            var state = JsonConvert.DeserializeObject<List<ScreenDuiState>>(lastKnownState);
            if (state != null)
            {
                await Delay(5000);
                foreach (var screenDuiState in state)
                    await this.playerPool.SynchronizeState(screenDuiState.State, screenDuiState.Screen);
            }

            this.isInitialized = true;
        }

        private CallbackDelegate OnPause(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName)) return callback;

            TriggerServerEvent(ServerEvents.OnPause, screenName);

            callback("OK");
            return callback;
        }

        [EventHandler(ClientEvents.PlayVideo)]
        private async void OnPlay(string url, string jsonScreen)
        {
            var screen = JsonConvert.DeserializeObject<Screen>(jsonScreen);
            await this.playerPool.Play(url, screen);
        }

        private CallbackDelegate OnPlay(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var videoUrl = ArgsReader.GetArgKeyValue<string>(args, "videoUrl");
            var screen = ArgsReader.GetArgKeyValue<string>(args, "screen");

            if (!string.IsNullOrEmpty(videoUrl) && !string.IsNullOrEmpty(screen))
                TriggerServerEvent(ServerEvents.OnPlaybackReceived, videoUrl, screen);

            callback("OK");
            return callback;
        }

        private async Task<CallbackDelegate> OnRequestState(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenStates = new List<DuiState>();

            foreach (var player in this.playerPool.VideoPlayers)
            {
                player.Browser.GetState();
                var state = await BrowserStateHelperScript.GetLastState();
                if (state == null)
                {
                    Debug.WriteLine($"received empty dui-state for screen: {player.ScreenName}");
                    continue;
                }

                screenStates.Add(state);
            }

            API.SendNuiMessage(
                JsonConvert.SerializeObject(
                    new { type = "HypnonemaNUI.UpdateStatuses", screenStates },
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));

            callback("OK");
            return callback;
        }

        [EventHandler("onClientResourceStop")]
        private void OnResourceStop(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;

            // clean up if not null
            this.playerPool?.Dispose();
        }

        private CallbackDelegate OnResume(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName)) return callback;

            TriggerServerEvent(ServerEvents.OnResumeVideo, screenName);

            callback("OK");
            return callback;
        }

        private CallbackDelegate OnSeek(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName)) return callback;

            var time = ArgsReader.GetArgKeyValue<float>(args, "time");

            this.playerPool.Seek(screenName, time);

            callback("OK");
            return callback;
        }

        [EventHandler(ClientEvents.ShowNUI)]
        private void OnShowNUI(bool isAceAllowed, string jsonScreens)
        {
            var screens = JsonConvert.DeserializeObject<List<Screen>>(jsonScreens);
            if (screens.Any())
                API.SendNuiMessage(
                    JsonConvert.SerializeObject(
                        new
                            {
                                type = "HypnonemaNUI.ShowUI",
                                hypnonemaVersion = GetAssemblyFileVersion(),
                                isAceAllowed,
                                screens
                            },
                        new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            }));
            else
                API.SendNuiMessage(
                    JsonConvert.SerializeObject(
                        new
                            {
                                type = "HypnonemaNUI.ShowUI", hypnonemaVersion = GetAssemblyFileVersion(), isAceAllowed
                            }));
            API.SetNuiFocus(true, true);
        }

        [EventHandler(ClientEvents.OnStateTick)]
        private async void OnStateTick()
        {
            if (!this.isInitialized || this.playerPool?.VideoPlayers?.Count == 0) return;

            var stateList = new List<DuiState>();
            var videoPlayers = this.playerPool?.VideoPlayers;

            if (videoPlayers != null)
            {
                foreach (var player in videoPlayers)
                {
                    player.Browser.GetState();
                    var duiState = await BrowserStateHelperScript.GetLastState();
                    if (duiState == null) continue;

                    stateList.Add(duiState);
                }

                TriggerServerEvent(ServerEvents.OnStateTick, JsonConvert.SerializeObject(stateList));
            }
        }

        private CallbackDelegate OnStopVideo(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName)) return callback;

            TriggerServerEvent(ServerEvents.OnStopVideo, screenName);

            callback("OK");
            return callback;
        }

        private async Task OnTick()
        {
            await this.playerPool.OnTick();
        }

        private CallbackDelegate OnToggleRepeat(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName)) return callback;

            TriggerServerEvent(ServerEvents.OnToggleRepeat, screenName);

            callback("OK");
            return callback;
        }

        private CallbackDelegate OnVolumeChange(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var volume = ArgsReader.GetArgKeyValue<string>(args, "volume");
            if (string.IsNullOrEmpty(volume)) return callback;

            TriggerServerEvent(ServerEvents.OnSetVolume, volume);

            callback("OK");
            return callback;
        }

        [EventHandler(ClientEvents.PauseVideo)]
        private void PauseVideo(string screenName)
        {
            this.playerPool.PauseVideo(screenName);
        }

        private void RegisterEventHandler(string eventName, Delegate actionDelegate)
        {
            this.EventHandlers.Add(eventName, actionDelegate);
        }

        [EventHandler(ClientEvents.ResumeVideo)]
        private void ResumeVideo(string screenName)
        {
            this.playerPool.ResumeVideo(screenName);
        }

        [EventHandler(ClientEvents.SetVolume)]
        private void SetVolume(float volume, string screenName)
        {
            this.playerPool.SetVolume(screenName, volume);
        }

        [EventHandler(ClientEvents.StopVideo)]
        private void StopVideo(string screenName)
        {
            this.playerPool.StopVideo(screenName);
        }

        [EventHandler(ClientEvents.ToggleRepeat)]
        private void ToggleRepeat(string screenName)
        {
            this.playerPool.ToggleRepeat(screenName);
        }

        private async Task TriggerStateTick()
        {
            while (true)
            {
                TriggerEvent(ClientEvents.OnStateTick);

                await Delay(this.syncInterval);
            }
        }
    }
}