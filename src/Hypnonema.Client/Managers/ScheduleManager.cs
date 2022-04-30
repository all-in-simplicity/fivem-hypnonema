namespace Hypnonema.Client
{
    using System.Collections.Generic;

    using Hypnonema.Client.Communications;
    using Hypnonema.Client.Extensions;
    using Hypnonema.Client.Utils;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Communications;
    using Hypnonema.Shared.Models;

    public class ScheduleManager
    {
        public ScheduleManager()
        {
        }

        public NetworkMethod<CreateScheduleMessage> CreateSchedule { get; private set; }

        public NetworkMethod<DeleteScheduleMessage> DeleteSchedule { get; private set; }

        public NetworkMethod<EditScheduleMessage> EditSchedule { get; private set; }

        public bool IsInitialized { get; private set; }

        public NetworkMethod<ScheduleListMessage> ScheduleList { get; private set; }

        public void Initialize()
        {
            if (this.IsInitialized)
            {
                return;
            }

            this.CreateSchedule =
                new NetworkMethod<CreateScheduleMessage>(Events.CreateSchedule, this.OnCreateSchedule);
            this.EditSchedule = new NetworkMethod<EditScheduleMessage>(Events.EditSchedule, this.OnEditSchedule);
            this.DeleteSchedule =
                new NetworkMethod<DeleteScheduleMessage>(Events.DeleteSchedule, this.OnDeleteSchedule);

            this.ScheduleList = new NetworkMethod<ScheduleListMessage>(Events.GetScheduleList, this.OnScheduleList);

            ClientScript.Self.RegisterCallback(Events.CreateSchedule, this.OnCreateSchedule);
            ClientScript.Self.RegisterCallback(Events.DeleteSchedule, this.OnDeleteSchedule);
            ClientScript.Self.RegisterCallback(Events.EditSchedule, this.OnEditSchedule);
            ClientScript.Self.RegisterCallback(Events.GetScheduleList, this.OnScheduleList);

            this.IsInitialized = true;
        }

        private void OnCreateSchedule(IDictionary<string, object> data)
        {
            var schedule = data.GetTypedValue<Schedule>("payload");

            if (schedule == null)
            {
                Logger.Error("failed to create schedule. payload is empty");
                return;
            }

            var createScheduleMessage = new CreateScheduleMessage(schedule);

            this.CreateSchedule.Invoke(createScheduleMessage);
        }

        private void OnCreateSchedule(CreateScheduleMessage createScheduleMessage)
        {
            Nui.SendMessage(Events.CreateSchedule, createScheduleMessage.Schedule);
        }

        private void OnDeleteSchedule(IDictionary<string, object> data)
        {
            var scheduleId = data.GetTypedValue<int>("scheduleId");

            if (default == scheduleId)
            {
                ClientScript.AddChatMessage("Failed to delete schedule. scheduleId is null");
                return;
            }

            var deleteScheduleMessage = new DeleteScheduleMessage(scheduleId);

            this.DeleteSchedule.Invoke(deleteScheduleMessage);
        }

        private void OnDeleteSchedule(DeleteScheduleMessage deleteScheduleMessage)
        {
            Nui.SendMessage(Events.DeleteSchedule, deleteScheduleMessage.ScheduleId);
        }

        private void OnEditSchedule(IDictionary<string, object> data)
        {
            var schedule = data.GetTypedValue<Schedule>("payload");

            if (schedule == null)
            {
                ClientScript.AddChatMessage("Failed to edit schedule. payload is null");
                return;
            }

            var editScheduleMessage = new EditScheduleMessage(schedule);

            this.EditSchedule.Invoke(editScheduleMessage);
        }

        private void OnEditSchedule(EditScheduleMessage editScheduleMessage)
        {
            Nui.SendMessage(Events.EditSchedule, editScheduleMessage.Schedule);
        }

        private void OnScheduleList(IDictionary<string, object> data)
        {
            var scheduleListMessage = new ScheduleListMessage(null);

            this.ScheduleList.Invoke(scheduleListMessage);
        }

        private void OnScheduleList(ScheduleListMessage scheduleListMessage)
        {
            Nui.SendMessage(Events.GetScheduleList, scheduleListMessage.Schedules);
        }
    }
}