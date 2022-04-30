namespace Hypnonema.Shared.Communications
{
    using System;

    public class DeleteScheduleMessage
    {
        public DeleteScheduleMessage(int scheduleId)
        {
            this.ScheduleId = scheduleId;
        }

        public int ScheduleId { get; set; }
    }
}