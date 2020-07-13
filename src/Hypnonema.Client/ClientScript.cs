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
    using Hypnonema.Shared;
    using Hypnonema.Shared.Models;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class ClientScript : BaseScript
    {
        private bool isInitialized;

        private VideoPlayerPool playerPool;

        private int syncInterval = 10000;

        public ClientScript()
        {
            this.RegisterEventHandler("onClientResourceStart", new Action<string>(this.OnClientResourceStart));
            this.RegisterEventHandler("onClientResourceStop", new Action<string>(this.OnResourceStop));

            this.RegisterEventHandler(ClientEvents.PlayVideo, new Func<string, string, Task>(this.OnPlay));
            this.RegisterEventHandler(ClientEvents.ShowNUI, new Action<bool, string>(this.OnShowNUI));
            this.RegisterEventHandler(
                ClientEvents.Initialize,
                new Func<string, int, bool, string, Task>(this.OnInitialize));
            this.RegisterEventHandler(ClientEvents.SetVolume, new Action<float, string>(this.SetVolume));
            this.RegisterEventHandler(ClientEvents.StopVideo, new Action<string>(this.StopVideo));
            this.RegisterEventHandler(ClientEvents.CloseScreen, new Action<string>(this.CloseScreen));
            this.RegisterEventHandler(ClientEvents.CreatedScreen, new Func<string, Task>(this.CreatedScreen));
            this.RegisterEventHandler(ClientEvents.EditedScreen, new Func<string, Task>(this.EditedScreen));
            this.RegisterEventHandler(ClientEvents.DeletedScreen, new Action<string>(this.DeletedScreen));
            this.RegisterEventHandler(ClientEvents.PauseVideo, new Action<string>(this.PauseVideo));
            this.RegisterEventHandler(ClientEvents.ResumeVideo, new Action<string>(this.ResumeVideo));
            this.RegisterEventHandler(ClientEvents.OnStateTick, new Func<Task>(this.OnStateTick));

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

        private void CloseScreen(string screenName)
        {
            this.playerPool.CloseScreen(screenName);
        }

        private async Task CreatedScreen(string jsonScreen)
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

        private void DeletedScreen(string screenName)
        {
            API.SendNuiMessage(JsonConvert.SerializeObject(new { type = "HypnonemaNUI.DeletedScreen", screenName }));
            this.CloseScreen(screenName);
        }

        private async Task EditedScreen(string jsonString)
        {
            var screen = JsonConvert.DeserializeObject<Screen>(jsonString);

            API.SendNuiMessage(
                JsonConvert.SerializeObject(
                    new { type = "HypnonemaNUI.EditedScreen", screen },
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));

            var player = this.playerPool.VideoPlayers.FirstOrDefault(s => s.ScreenName == screen.Name);
            if (player != null)
            {
                // we want the current state of the dui browser,
                // so we send a request and wait as long as duiStateResponse is null but max. 5500 ms
                player.Browser.SendMessage(new { type = "getState" });
                var state = await BrowserStateHelperScript.GetLastState();

                this.playerPool.CloseScreen(player.ScreenName);
                player = await this.playerPool.CreateVideoPlayerAsync(screen);

                // stateResponse can be null ( eg. if waiting time exceeded 5500ms)
                if (state != null)
                {
                    await Delay(500);
                    player.Browser.SendMessage(
                        new
                            {
                                type = "update",
                                src = state.CurrentSource,
                                currentTime = state.CurrentTime,
                                paused = state.IsPaused
                            });
                }

                this.playerPool.VideoPlayers.Add(player);
            }
        }

        private string GetAssemblyFileVersion()
        {
            var attribute = (AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Single();
            return attribute.Version;
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;

            var url = API.GetResourceMetadata(resourceName, "hypnonema_url", 0);

            if (!double.TryParse(
                    API.GetResourceMetadata(API.GetCurrentResourceName(), "hypnonema_sync_interval", 0),
                    out var syncInterval)) this.syncInterval = 10000;

            this.playerPool = new VideoPlayerPool(url);
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
            var alwaysOn = ArgsReader.GetArgKeyValue<bool>(args, "alwaysOn");
            var modelName = ArgsReader.GetArgKeyValue<string>(args, "modelName");
            var renderTargetName = ArgsReader.GetArgKeyValue<string>(args, "renderTargetName");
            var positionX = ArgsReader.GetArgKeyValue<float>(args, "positionX");
            var positionY = ArgsReader.GetArgKeyValue<float>(args, "positionY");
            var positionZ = ArgsReader.GetArgKeyValue<float>(args, "positionZ");
            var rotationX = ArgsReader.GetArgKeyValue<float>(args, "rotationX");
            var rotationY = ArgsReader.GetArgKeyValue<float>(args, "rotationY");
            var rotationZ = ArgsReader.GetArgKeyValue<float>(args, "rotationZ");
            var scaleX = ArgsReader.GetArgKeyValue<float>(args, "scaleX");
            var scaleY = ArgsReader.GetArgKeyValue<float>(args, "scaleY");
            var scaleZ = ArgsReader.GetArgKeyValue<float>(args, "scaleZ");
            var globalVolume = ArgsReader.GetArgKeyValue(args, "globalVolume", 100f);
            var soundAttenuation = ArgsReader.GetArgKeyValue<float>(args, "soundAttenuation", 5f);
            var soundMinDistance = ArgsReader.GetArgKeyValue<float>(args, "soundMinDistance", 15f);
            var soundMaxDistance = ArgsReader.GetArgKeyValue<float>(args, "soundMaxDistance", 100f);
            var is3DAudioEnabled = ArgsReader.GetArgKeyValue<bool>(args, "is3DAudioEnabled");
          
            var screen = new Screen
                             {
                                 AlwaysOn = alwaysOn,
                                 Name = screenName,
                                 Is3DRendered = is3DRendered,
                                 BrowserSettings =
                                     new DuiBrowserSettings
                                         {
                                             GlobalVolume = globalVolume,
                                             SoundMaxDistance = soundMaxDistance,
                                             SoundMinDistance = soundMinDistance,
                                             SoundAttenuation = soundAttenuation,
                                             Is3DAudioEnabled = is3DAudioEnabled
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
                                                          : null,
                                 TargetSettings = is3DRendered
                                                      ? null
                                                      : new RenderTargetSettings
                                                            {
                                                                ModelName = modelName,
                                                                RenderTargetName = renderTargetName
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
            var globalVolume = ArgsReader.GetArgKeyValue<float>(args, "globalVolume", 100f);
            var soundAttenuation = ArgsReader.GetArgKeyValue<float>(args, "soundAttenuation", 5f);
            var soundMinDistance = ArgsReader.GetArgKeyValue<float>(args, "soundMinDistance", 15f);
            var soundMaxDistance = ArgsReader.GetArgKeyValue<float>(args, "soundMaxDistance", 100f);
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

        private async Task OnInitialize(string jsonScreens, int rendererLimit, bool isAceAllowed, string lastKnownState)
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

            // Debug.WriteLine("Initialized..");
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

        private async Task OnPlay(string url, string jsonScreen)
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

        private void OnShowNUI(bool isAceAllowed, string jsonScreens)
        {
            var screens = JsonConvert.DeserializeObject<List<Screen>>(jsonScreens);
            if (screens.Any())
                API.SendNuiMessage(
                    JsonConvert.SerializeObject(
                        new
                            {
                                type = "HypnonemaNUI.ShowUI",
                                hypnonemaVersion = this.GetAssemblyFileVersion(),
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
                                type = "HypnonemaNUI.ShowUI",
                                hypnonemaVersion = this.GetAssemblyFileVersion(),
                                isAceAllowed
                            }));
            API.SetNuiFocus(true, true);
        }

        private async Task OnStateTick()
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

        private void PauseVideo(string screenName)
        {
            this.playerPool.PauseVideo(screenName);
        }

        private void RegisterEventHandler(string eventName, Delegate actionDelegate)
        {
            this.EventHandlers.Add(eventName, actionDelegate);
        }

        private void ResumeVideo(string screenName)
        {
            this.playerPool.ResumeVideo(screenName);
        }

        private void SetVolume(float volume, string screenName)
        {
            this.playerPool.SetVolume(screenName, volume);
        }

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