namespace Hypnonema.Shared.Models
{
    using System;
    
    public class Schedule
    {
        public DayOfWeek? DayOfWeek { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime StartDateTime { get; set; }

        public int Id { get; set; }

        public int Interval { get; set; }

        public int Rule { get; set; }

        public Screen Screen { get; set; }

        public string Url { get; set; }
    }
}