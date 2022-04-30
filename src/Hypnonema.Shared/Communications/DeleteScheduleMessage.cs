namespace Hypnonema.Shared.Communications
{
    public class DeleteScheduleMessage
    {
        public DeleteScheduleMessage(int scheduleId)
        {
            this.ScheduleId = scheduleId;
        }

        public int ScheduleId { get; set; }
    }
}