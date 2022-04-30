namespace Hypnonema.Server.Scheduler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CitizenFX.Core;

    using EWSoftware.PDI;

    using Hypnonema.Shared.Models;

    using Newtonsoft.Json;

    using Quartz;
    using Quartz.Impl;
    using Quartz.Impl.Matchers;
    using Quartz.Logging;

    using Logger = Hypnonema.Server.Utils.Logger;

    public class Scheduler
    {
        private IScheduler scheduler;

        private Scheduler(IScheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        public static async Task<Scheduler> CreateScheduler()
        {
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();

            var jobListener = new JobListener();

            scheduler.ListenerManager.AddJobListener(jobListener, GroupMatcher<JobKey>.GroupEquals("group1"));

            return new Scheduler(scheduler);
        }

        public async Task RemoveSchedule(Schedule schedule)
        {
            var key = new JobKey($"{schedule.Id}Job");

            if (await this.scheduler.CheckExists(key))
            {
                await this.scheduler.DeleteJob(key);
            }
        }

        public async Task Schedule(List<Schedule> schedule)
        {
            await this.scheduler.Clear();

            foreach (var schedulable in schedule)
            {
                await this.Schedule(schedulable);
            }
        }

        public async Task Schedule(Schedule schedule)
        {
            var nextOccurrence = GetNextOccurrence(schedule);
            if (nextOccurrence == DateTime.MinValue || DateTime.Compare(nextOccurrence, DateTime.Now) < 0)
            {
                // no more schedules left
                Logger.Debug($"no more schedules left for scheduleId: {schedule.Id}.");
                return;
            }

            var job = JobBuilder.Create<PlayScheduleJob>().WithIdentity($"{schedule.Id}Job", "group1")
                .UsingJobData("url", schedule.Url).UsingJobData("scheduleId", schedule.Id).UsingJobData(
                    "screenJson",
                    JsonConvert.SerializeObject(schedule.Screen)).Build();

            var trigger = TriggerBuilder.Create().WithIdentity($"{schedule.Id}Trigger", "group1")
                .StartAt(nextOccurrence).UsingJobData("url", schedule.Url).UsingJobData(
                    "screenJson",
                    JsonConvert.SerializeObject(schedule.Screen)).Build();

            await BaseScript.Delay(0);
            Logger.Debug(
                $"scheduled: scheduleId {schedule.Id} screenName {schedule.Screen.Name}. next occurrence at {nextOccurrence}");

            await this.scheduler.ScheduleJob(job, trigger);
        }

        public async Task Start()
        {
            await this.scheduler.Start();
        }

        private Recurrence ConvertFromSchedule(Schedule schedule)
        {
            // to get the correct RecurFrequency we have to add 1 to schedule.Rule;
            var frequency = schedule.Rule + 1;
            
            return new Recurrence()
                       {
                           StartDateTime = schedule.StartDateTime.ToUniversalTime(),
                           Frequency = (RecurFrequency)frequency,
                           RecurUntil = schedule.EndDate,
                           Interval = schedule.Interval,
                       };
        }

        private DateTime GetNextOccurrence(Schedule schedule)
        {
            Recurrence next = ConvertFromSchedule(schedule);
            return next.NextInstance(DateTime.Now, true);
            /*
            var taskTime = task.StartTime;
            var taskDate = new DateTime(
                task.EndDate.Year,
                task.EndDate.Month,
                task.EndDate.Day,
                taskTime.Hour,
                taskTime.Minute,
                taskTime.Second);

            var rRecur = new Recurrence
                             {
                                 StartDateTime = taskDate,
                                 Frequency = 
                             };

            if (task.EndDate != default)
            {
                rRecur.RecurUntil = task.EndDate;
            }

            switch (task.Rule)
            {
                case "Daily":
                    rRecur.RecurDaily(1);
                    break;
                case "Weekly":
                    rRecur.RecurWeekly(Convert.ToInt32(task.Interval), (DaysOfWeek) task.DayOfWeek.GetValueOrDefault());
                    break;
                case "Monthly":
                    rRecur.RecurMonthly((int) task.DayOfWeek.GetValueOrDefault(), Convert.ToInt32(task.Interval));
                    break;
                case "Single":
                    rRecur.MaximumOccurrences = 1;
                    break;
            }

            return rRecur.NextInstance(DateTime.Now, true);*/
            
        }

        // simple log provider to get something to the console
        private class ConsoleLogProvider : ILogProvider
        {
            public Quartz.Logging.Logger GetLogger(string name)
            {
                return (level, func, exception, parameters) =>
                    {
                        if (level >= LogLevel.Info && func != null)
                        {
                            Logger.Verbose($"[ + {DateTime.Now.ToLongTimeString()}] [{level}] {func()}, {parameters}");
                        }

                        return true;
                    };
            }

            public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenNestedContext(string message)
            {
                throw new NotImplementedException();
            }
        }
    }
}