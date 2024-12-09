using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStudy.Model
{
    public class Schedule
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Unique identifier for each schedule entry

        public string? TaskName { get; set; } // Name of the Schedule

        public string? TaskDescription { get; set; } // Description of the Schedule

        public TimeSpan StartTime { get; set; } // Start time of the Schedule

        public TimeSpan EndTime { get; set; } // End time of the Schedule

        public string? DayOfWeek { get; set; } // Day of the week (e.g., "Monday")

        public string? Priority { get; set; } // Priority level (e.g., "Major", "Minor", "Retake")
    }
}