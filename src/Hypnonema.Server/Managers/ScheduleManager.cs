namespace Hypnonema.Server.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CitizenFX.Core;

    using Hypnonema.Server.Communications;
    using Hypnonema.Server.Scheduler;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Communications;
    using Hypnonema.Shared.Models;

    using LiteDB;

    using Logger = Hypnonema.Server.Utils.Logger;

    public class ScheduleManager
    {
        private readonly NetworkMethod<CreateScheduleMessage> createSchedule;

        private readonly NetworkMethod<DeleteScheduleMessage> deleteSchedule;

        private readonly NetworkMethod<EditScheduleMessage> editSchedule;

        public Scheduler scheduler;

        private readonly LiteCollection<Schedule> schedules;

        private readonly NetworkMethod<ScheduleListMessage> schedulesList;

        private ScheduleManager(LiteCollection<Schedule> scheduleCollection, Scheduler scheduler)
        {
            this.scheduler = scheduler;
            this.schedules = scheduleCollection;

            this.createSchedule =
                new NetworkMethod<CreateScheduleMessage>(Events.CreateSchedule, this.OnCreateSchedule);

            this.schedulesList = new NetworkMethod<ScheduleListMessage>(Events.GetScheduleList, this.OnGetScheduleList);

            this.editSchedule = new NetworkMethod<EditScheduleMessage>(Events.EditSchedule, this.OnEditSchedule);

            this.deleteSchedule =
                new NetworkMethod<DeleteScheduleMessage>(Events.DeleteSchedule, this.OnDeleteSchedule);

            BaseServer.Self.AddEvent(Events.ScheduleExecuted, new Action<int>(this.OnScheduleExecuted));
        }

        public static async Task<ScheduleManager> Create(LiteCollection<Schedule> scheduleCollection)
        {
            var scheduler = await Scheduler.CreateScheduler();

            var screenScheduleManager = new ScheduleManager(scheduleCollection, scheduler);

            return screenScheduleManager;
        }

        public async Task Start()
        {
            await this.scheduler.Start();

            await this.ScheduleSavedSchedules();
        }

        private async Task ScheduleSavedSchedules()
        {
            foreach (var schedule in this.schedules.Include(x => x.Screen).FindAll().ToList())
            {
                await this.scheduler.Schedule(schedule);
            }
        }

        private List<Schedule> getSchedules()
        {
            return this.schedules.Include(x => x.Screen).FindAll().ToList();
        }

        private async void OnCreateSchedule(Player p, CreateScheduleMessage createScheduleMessage)
        {
            if (!p.IsAceAllowed(Permission.CreateSchedule))
            {
                p.AddChatMessage(
                    $"You are not permitted to create a schedule. missing ace {Permission.CreateSchedule}",
                    new[] {255, 0, 0});
                return;
            }

            var id = this.schedules.Insert(createScheduleMessage.Schedule);
            createScheduleMessage.Schedule.Id = id;

            // dates need to be converted to localtime
            createScheduleMessage.Schedule.StartDateTime = createScheduleMessage.Schedule.StartDateTime.ToLocalTime();
            createScheduleMessage.Schedule.EndDate = createScheduleMessage.Schedule.EndDate.ToLocalTime();
            
            Logger.Debug($"scheduling scheduleId: {createScheduleMessage.Schedule.Id}");
            await this.scheduler.Schedule(createScheduleMessage.Schedule);

            this.createSchedule.Invoke(p, createScheduleMessage);

            var scheduleListMessage = new ScheduleListMessage(this.getSchedules());
            this.schedulesList.Invoke(null, scheduleListMessage);
        }

        private async void OnDeleteSchedule(Player p, DeleteScheduleMessage deleteScheduleMessage)
        {
            if (!p.IsAceAllowed(Permission.DeleteSchedule))
            {
                p.AddChatMessage(
                    $"You are not permitted to delete a schedule. missing ace {Permission.DeleteSchedule}",
                    new[] {255, 0, 0});
                return;
            }

            var schedule = this.schedules.FindById(deleteScheduleMessage.ScheduleId);
            if (schedule != null)
            {
                await this.scheduler.RemoveSchedule(schedule);
            }

            var count = this.schedules.Delete(s => s.Id == deleteScheduleMessage.ScheduleId);
            if (count == 0)
            {
                p.AddChatMessage(
                    $"Failed to delete schedule. Schedule with id \"{deleteScheduleMessage.ScheduleId}\" not found.",
                    new[] {255, 0, 0});
                return;
            }

            this.deleteSchedule.Invoke(p, deleteScheduleMessage);

            var scheduleListMessage = new ScheduleListMessage(this.getSchedules());
            this.schedulesList.Invoke(null, scheduleListMessage);
        }

        private async void OnEditSchedule(Player p, EditScheduleMessage editScheduleMessage)
        {
            if (!p.IsAceAllowed(Permission.EditSchedule))
            {
                p.AddChatMessage(
                    $"You are not permitted to edit a schedule. missing ace {Permission.EditSchedule}",
                    new[] {255, 0, 0});
                return;
            }

            var found = this.schedules.Update(editScheduleMessage.Schedule);
            if (!found)
            {
                p.AddChatMessage(
                    $"Editing failed. schedule with id \"{editScheduleMessage.Schedule.Id}\" not found.",
                    new[] {255, 0, 0});
                return;
            }
            
            Logger.Debug($"rescheduling schedule scheduleId: {editScheduleMessage.Schedule.Id}");
            await this.scheduler.RemoveSchedule(editScheduleMessage.Schedule);
            await this.scheduler.Schedule(editScheduleMessage.Schedule);


            this.editSchedule.Invoke(p, editScheduleMessage);

            var scheduleListMessage = new ScheduleListMessage(this.getSchedules());
            this.schedulesList.Invoke(null, scheduleListMessage);
        }

        private void OnGetScheduleList(Player p, ScheduleListMessage unused)
        {
            var scheduleListMessage = new ScheduleListMessage(this.getSchedules());
            this.schedulesList.Invoke(p, scheduleListMessage);
        }

        private async void OnScheduleExecuted(int scheduleId)
        {
            Logger.Debug($"executed schedule. scheduleId: {scheduleId}");

            var schedule = this.schedules.Include(s => s.Screen).FindById(scheduleId);
            if (schedule == null) return;

            Logger.Debug($"rescheduling scheduleId: {schedule.Id}");
            await this.scheduler.Schedule(schedule);
        }
    }
}