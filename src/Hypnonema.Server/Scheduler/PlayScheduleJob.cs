namespace Hypnonema.Server.Scheduler
{
    using System.Threading.Tasks;

    using CitizenFX.Core;

    using Hypnonema.Server.Utils;
    using Hypnonema.Shared.Communications;
    using Hypnonema.Shared.Models;

    using Newtonsoft.Json;

    using Quartz;

    public class PlayScheduleJob : IJob
    {
        private const string playEventName = "hyp:S2C:play";

        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;

            var url = dataMap.GetString("url");
            var screenJson = dataMap.GetString("screenJson");

            if (screenJson == null || url == null)
            {
                Logger.Error(
                    "Failed to execute job because attempt to parse job data returned empty values. Aborting..");
                return;
            }

            var screen = JsonConvert.DeserializeObject<Screen>(screenJson);

            var playMessage = new PlayMessage() {Screen = screen, Url = url};

            await BaseScript.Delay(0);

            Logger.Debug($"executing playJob. playing url \"{url}\" on {screen?.Name}");

            BaseServer.Self.PlaybackManager.OnPlay(screen.Name, url);

            // BaseScript.TriggerClientEvent(playEventName, JsonConvert.SerializeObject(playMessage));
        }
    }
}