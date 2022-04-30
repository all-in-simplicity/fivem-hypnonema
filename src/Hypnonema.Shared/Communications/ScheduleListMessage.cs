namespace Hypnonema.Shared.Communications
{
    using System.Collections.Generic;

    using Hypnonema.Shared.Models;

    public class ScheduleListMessage
    {
        public ScheduleListMessage(List<Schedule> schedules)
        {
            this.Schedules = schedules;
        }

        public List<Schedule> Schedules { get; set; }
    }
}