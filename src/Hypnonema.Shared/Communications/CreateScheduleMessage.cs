namespace Hypnonema.Shared.Communications
{
    using Hypnonema.Shared.Models;

    public class CreateScheduleMessage
    {
        public CreateScheduleMessage(Schedule schedule)
        {
            this.Schedule = schedule;
        }

        public Schedule Schedule { get; set; }
    }
}