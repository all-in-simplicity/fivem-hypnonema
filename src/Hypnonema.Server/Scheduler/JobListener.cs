namespace Hypnonema.Server.Scheduler
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using CitizenFX.Core;

    using Hypnonema.Shared;

    using Quartz;

    public class JobListener : IJobListener
    {
        public string Name => "JobListener";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public async Task JobWasExecuted(
            IJobExecutionContext context,
            JobExecutionException jobException,
            CancellationToken token)
        {
            var dataMap = context.JobDetail.JobDataMap;

            var scheduleId = dataMap.GetIntValue("scheduleId");

            await BaseScript.Delay(500);
            BaseScript.TriggerEvent(Events.ScheduleExecuted, scheduleId);
        }
    }
}