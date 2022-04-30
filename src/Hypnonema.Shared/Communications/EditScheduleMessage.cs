namespace Hypnonema.Shared.Communications
{
    using Hypnonema.Shared.Models;

    public class EditScheduleMessage
    {
        public EditScheduleMessage(Schedule schedule)
        {
            this.Schedule = schedule;
        }

        public Schedule Schedule { get; set; }
    }
}