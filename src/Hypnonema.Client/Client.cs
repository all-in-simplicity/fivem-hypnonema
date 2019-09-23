namespace Hypnonema.Client
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Shared;

    using Newtonsoft.Json;

    using Debug = CitizenFX.Core.Debug;

    public class Client : BaseScript
    {
        private const string SfName = "hypnonema_texture_renderer";

        private string TxdName;

        private const string TxnName = "render_txn";

        private int width = 1280;

        private int height = 720;

        private float sndMinDistance = 10.0f;

        private float sndAttenuation = 5.0f;

        private float sndGlobalVolume = 100f;

        private bool txdHasBeenSet = false;

        private bool scaleformTickActive = false;

        private bool isAceAllowed = false;

        private bool isInitialized = false;

        private long duiObj = 0;

        private Scaleform scaleform;
        
        private Vector3 scaleformPos = new Vector3(-1678.949f, -928.3431f, 20.6290932f);

        private Vector3 scaleformRot = new Vector3(0f, 0f, -140f);

        private Vector3 scaleformScale = new Vector3(0.969999969f, 0.484999985f, -0.1f);

        public Client()
        {
            this.EventHandlers["onClientResourceStart"] += new Func<string, Task>(this.OnClientResourceStart);
            this.EventHandlers["onClientResourceStop"] += new Func<string, Task>(this.OnClientResourceStop);

            this.RegisterEventHandler(ClientEvents.PlayVideo, new Func<string, string, Task>(this.OnPlay));
            this.RegisterEventHandler(ClientEvents.PauseVideo, new Action(this.PauseVideo));
            this.RegisterEventHandler(ClientEvents.StopVideo, new Action(this.StopVideo));
            this.RegisterEventHandler(ClientEvents.ResumeVideo, new Action(this.ResumeVideo));
            this.RegisterEventHandler(ClientEvents.SetVolume, new Action<float>(this.SetVolume));
            this.RegisterEventHandler(ClientEvents.ShowNUI, new Action(this.OnShowNUI));
            this.RegisterEventHandler(ClientEvents.SetSoundAttenuation, new Action<float>(this.SetSoundAttenuation));
            this.RegisterEventHandler(ClientEvents.SetSoundMinDistance, new Action<float>(this.SetSoundMinDistance));
            this.RegisterEventHandler(
                ClientEvents.UpdateState,
                new Action<bool, float, float, float, string, string>(this.UpdateState));
            this.RegisterEventHandler(ClientEvents.ToggleReplay, new Action<bool>(this.ToggleReplay));

            this.RegisterNuiCallback(ClientEvents.OnStateTick, this.OnStateTick);
            this.RegisterNuiCallback(ClientEvents.OnPlay, this.OnPlay);
            this.RegisterNuiCallback(ClientEvents.OnPause, this.OnPause);
            this.RegisterNuiCallback(ClientEvents.OnHideNUI, this.OnHideNUI);
            this.RegisterNuiCallback(ClientEvents.OnStopVideo, this.OnStopVideo);
            this.RegisterNuiCallback(ClientEvents.OnResumeVideo, this.OnResumeVideo);
            this.RegisterNuiCallback(ClientEvents.OnVolumeChange, this.OnVolumeChange);
            this.RegisterNuiCallback(ClientEvents.OnSoundMinDistanceChange, this.OnSoundMinDistanceChange);
            this.RegisterNuiCallback(ClientEvents.OnSoundAttuenationChange, this.OnSoundAttenuationChange);
            this.RegisterNuiCallback(ClientEvents.OnToggleReplay, this.OnToggleReplay);
        }

        private void ToggleReplay(bool replay)
        {
            API.SendDuiMessage(this.duiObj, JsonConvert.SerializeObject(new { type = "toggleReplay", value = replay }));
        }

        private CallbackDelegate OnToggleReplay(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var replay = args.FirstOrDefault(arg => arg.Key == "replay").Value as bool?;
            if (replay == null)
            {
                return callback;
            }

            TriggerServerEvent(ServerEvents.OnToggleReplay, replay);

            return callback;
        }

        private void UpdateState(
            bool paused,
            float currentTime,
            float duration,
            float remainingTime,
            string currentSource,
            string currentType)
        {
            if (!this.scaleformTickActive && !paused && this.isInitialized)
            {
                Debug.WriteLine("synchronizing playback state..");
                API.SendDuiMessage(
                    this.duiObj,
                    JsonConvert.SerializeObject(
                        new { type = "update", currentTime, src = new { type = currentType, url = currentSource } }));
                this.scaleformTickActive = true;
                this.Tick += this.ShowVideo;
            }
        }

        private CallbackDelegate OnStateTick(IDictionary<string, object> args, CallbackDelegate callback)
        {
            // TODO: try to keep packets send to the server as low as possible by
            // finding a way to make IsAceAllowed work, so we basically only send state ticks from allowed users
            // if (API.IsAceAllowed($"command.{cmdName}"))
            var paused = args.FirstOrDefault(arg => arg.Key == "paused").Value as bool?;
            if (paused == null)
            {
                return callback;
            }

            var currentTime = args.FirstOrDefault(arg => arg.Key == "currentTime").Value?.ToString();
            if (string.IsNullOrEmpty(currentTime))
            {
                return callback;
            }

            var duration = args.FirstOrDefault(arg => arg.Key == "duration").Value?.ToString();
            if (string.IsNullOrEmpty(duration))
            {
                return callback;
            }

            var remainingTime = args.FirstOrDefault(arg => arg.Key == "remainingTime").Value?.ToString();
            if (string.IsNullOrEmpty(remainingTime))
            {
                return callback;
            }

            var currentSource = args.FirstOrDefault(arg => arg.Key == "currentSource").Value?.ToString();
            if (string.IsNullOrEmpty(currentSource))
            {
                return callback;
            }

            var currentType = args.FirstOrDefault(arg => arg.Key == "currentType").Value?.ToString();
            if (string.IsNullOrEmpty(currentType))
            {
                return callback;
            }

            TriggerServerEvent(
                ServerEvents.OnStateTick,
                paused,
                currentTime,
                duration,
                remainingTime,
                currentSource,
                currentType);

            return callback;
        }

        private CallbackDelegate OnSoundAttenuationChange(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var attenuation = args.FirstOrDefault(arg => arg.Key == "soundAttenuation").Value?.ToString();
            if (!string.IsNullOrEmpty(attenuation))
                TriggerServerEvent(ServerEvents.OnSetSoundAttenuation, attenuation);
            return callback;
        }

        private CallbackDelegate OnSoundMinDistanceChange(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var soundMinDistance = args.FirstOrDefault(arg => arg.Key == "minDistance").Value?.ToString();
            if (!string.IsNullOrEmpty(soundMinDistance))
                TriggerServerEvent(ServerEvents.OnSetSoundMinDistance, soundMinDistance);
            return callback;
        }

        private void SetSoundAttenuation(float f)
        {
            this.sndAttenuation = f;
        }

        private void SetSoundMinDistance(float f)
        {
            this.sndMinDistance = f;
        }

        private void SetVolume(float volume)
        {
            this.sndGlobalVolume = volume;
        }

        private float CalculateSoundFactor(float distance)
        {
            return this.sndMinDistance / (this.sndMinDistance
                                          + this.sndAttenuation
                                          * (Math.Max(distance, this.sndMinDistance) - this.sndMinDistance));
        }

        private CallbackDelegate OnVolumeChange(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var volume = args.FirstOrDefault(arg => arg.Key == "volume").Value?.ToString();
            if (!string.IsNullOrEmpty(volume)) TriggerServerEvent(ServerEvents.OnSetVolume, volume);

            return callback;
        }

        private CallbackDelegate OnResumeVideo(IDictionary<string, object> args, CallbackDelegate callback)
        {
            TriggerServerEvent(ServerEvents.OnResumeVideo);
            return callback;
        }

        private void ResumeVideo()
        {
            // check scaleformTick, can be false after stop
            if (!this.scaleformTickActive)
            {
                this.Tick += this.ShowVideo;
                this.scaleformTickActive = true;
            }

            API.SendDuiMessage(this.duiObj, JsonConvert.SerializeObject(new { type = "resume" }));
        }

        private CallbackDelegate OnStopVideo(IDictionary<string, object> args, CallbackDelegate callback)
        {
            TriggerServerEvent(ServerEvents.OnStopVideo);
            return callback;
        }

        private void StopVideo()
        {
            if (this.scaleformTickActive)
            {
                this.scaleformTickActive = false;
                this.Tick -= this.ShowVideo;
            }

            API.SendDuiMessage(this.duiObj, JsonConvert.SerializeObject(new { type = "stop" }));
        }

        private string GetHypnonemaVersion()
        {
            var attribute = (AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Single();
            return attribute.Version;
        }

        private CallbackDelegate OnPause(IDictionary<string, object> args, CallbackDelegate callback)
        {
            TriggerServerEvent(ServerEvents.OnPause);
            return callback;
        }

        private void OnShowNUI()
        {
            API.SendNuiMessage(
                JsonConvert.SerializeObject(
                    new { type = "HypnonemaNUI.ShowUI", hypnonemaVersion = this.GetHypnonemaVersion() }));
            API.SetNuiFocus(true, true);
        }

        private CallbackDelegate OnHideNUI(IDictionary<string, object> args, CallbackDelegate callback)
        {
            API.SetNuiFocus(false, false);
            API.SendNuiMessage(JsonConvert.SerializeObject(new { type = "HypnonemaNUI.HideUI" }));
            return callback;
        }

        private CallbackDelegate OnPlay(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var videoURL = args.FirstOrDefault(arg => arg.Key == "videoURL").Value?.ToString();
            var videoType = args.FirstOrDefault(arg => arg.Key == "videoType").Value?.ToString();

            if (!string.IsNullOrEmpty(videoURL) && !string.IsNullOrEmpty(videoType))
                TriggerServerEvent(ServerEvents.OnPlaybackReceived, videoURL, videoType);

            return callback;
        }

        private void PauseVideo()
        {
            API.SendDuiMessage(this.duiObj, JsonConvert.SerializeObject(new { type = "pause" }));
        }

        private async Task OnPlay(string videoURL, string videoType)
        {
            API.SendDuiMessage(
                this.duiObj,
                JsonConvert.SerializeObject(new { type = "play", src = new { type = videoType, url = videoURL } }));
            if (!this.scaleformTickActive)
            {
                this.scaleformTickActive = true;
                this.Tick += this.ShowVideo;
            }

            await Task.FromResult(0);
        }

        private async Task OnClientResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;

            Debug.WriteLine($"Using hypnonema version: {this.GetHypnonemaVersion()}");

            Debug.WriteLine("creating new scaleform");
            this.scaleform = new Scaleform(SfName);
            Debug.WriteLine("loaded new scaleform");

            var numberFormat = new CultureInfo("en-US").NumberFormat;
            this.TxdName = Guid.NewGuid().ToString();

            while (!this.scaleform.IsLoaded) await Delay(0);

            var url = API.GetResourceMetadata(resourceName, "hypnonema_url", 0);
            var posX = API.GetResourceMetadata(resourceName, "hypnonema_position", 0);
            var posY = API.GetResourceMetadata(resourceName, "hypnonema_position", 1);
            var posZ = API.GetResourceMetadata(resourceName, "hypnonema_position", 2);
            this.scaleformPos = new Vector3(
                float.Parse(posX, numberFormat),
                float.Parse(posY, numberFormat),
                float.Parse(posZ, numberFormat));

            var rotX = API.GetResourceMetadata(resourceName, "hypnonema_rotation", 0);
            var rotY = API.GetResourceMetadata(resourceName, "hypnonema_rotation", 1);
            var rotZ = API.GetResourceMetadata(resourceName, "hypnonema_rotation", 2);
            this.scaleformRot = new Vector3(
                float.Parse(rotX, numberFormat),
                float.Parse(rotY, numberFormat),
                float.Parse(rotZ, numberFormat));

            var scaleX = API.GetResourceMetadata(resourceName, "hypnonema_scale", 0);
            var scaleY = API.GetResourceMetadata(resourceName, "hypnonema_scale", 1);
            var scaleZ = API.GetResourceMetadata(resourceName, "hypnonema_scale", 2);
            this.scaleformScale = new Vector3(
                float.Parse(scaleX, numberFormat),
                float.Parse(scaleY, numberFormat),
                float.Parse(scaleZ, numberFormat));

            this.height = int.Parse(API.GetResourceMetadata(resourceName, "hypnonema_height", 0));
            this.width = int.Parse(API.GetResourceMetadata(resourceName, "hypnonema_width", 0));

            var txd = API.CreateRuntimeTxd(this.TxdName);
            this.duiObj = API.CreateDui(url, this.width, this.height);
            var dui = API.GetDuiHandle(this.duiObj);

            var txn = Function.Call<long>(Hash.CREATE_RUNTIME_TEXTURE_FROM_DUI_HANDLE, txd, TxnName, dui);

            this.isInitialized = true;
            this.isAceAllowed = API.IsAceAllowed(
                $"command.{API.GetResourceMetadata(resourceName, "hypnonema_command_name", 0)}");
            Debug.WriteLine($"dui runtime texture handle: {txn}");
            await Delay(0);
        }

        private async Task OnClientResourceStop(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;

            if (this.duiObj != 0)
            {
                API.DestroyDui(this.duiObj);
                this.scaleform.Dispose();
                this.txdHasBeenSet = false;
            }

            await Delay(0);
        }

        private async Task ShowVideo()
        {
            // draw call wrapped inside try block to be able to stop video playback on error
            try
            {
                if (this.scaleform.IsValid && !this.txdHasBeenSet)
                {
                    this.scaleform.CallFunction("SET_TEXTURE", this.TxdName, TxnName, 0, 0, this.width, this.height);
                    this.txdHasBeenSet = true;
                }

                if (this.scaleform.IsValid)
                {
                    this.scaleform.Render3D(this.scaleformPos, this.scaleformRot, this.scaleformScale);
                    //this.scaleform2.Render3D(this.scaleformPos + (Vector3.UnitX * 20), this.scaleformRot, this.scaleformScale);
                    var playerPos = Game.PlayerPed.Position;
                    var distance = API.GetDistanceBetweenCoords(
                        playerPos.X,
                        playerPos.Y,
                        playerPos.Z,
                        this.scaleformPos.X,
                        this.scaleformPos.Y,
                        this.scaleformPos.Z,
                        true);

                    var sndFactor = this.CalculateSoundFactor(distance);
                    var volume = sndFactor * this.sndGlobalVolume;
                    API.SendDuiMessage(
                        this.duiObj,
                        JsonConvert.SerializeObject(new { type = "volume", volume = volume / 100 }));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[Hypnonema]: Exception occured at attempt to play video: {e.Message}");
                this.StopVideo();
            }

            await Task.FromResult(0);
        }

        protected void RegisterEventHandler(string eventName, Delegate actionDelegate)
        {
            this.EventHandlers.Add(eventName, actionDelegate);
        }

        protected void RegisterNuiCallback(
            string msg,
            Func<IDictionary<string, object>, CallbackDelegate, CallbackDelegate> callback)
        {
            API.RegisterNuiCallbackType(msg);

            this.EventHandlers[$"__cfx_nui:{msg}"] += new Action<ExpandoObject, CallbackDelegate>(
                (body, resultCallback) => { callback.Invoke(body, resultCallback); });
        }
    }
}