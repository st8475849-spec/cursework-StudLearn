using System;

namespace StudentIS.Models
{
    public class ScheduleItem
    {
        public Guid Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Guid SubjectId { get; set; }
        public string Room { get; set; }
        public string LessonType { get; set; }

        // Конструктор за замовчуванням
        public ScheduleItem()
        {
            Id = Guid.NewGuid();
            Room = "";
            LessonType = "";
            DayOfWeek = DayOfWeek.Monday;
            StartTime = new TimeSpan(8, 0, 0);
            EndTime = new TimeSpan(9, 30, 0);
        }

        public ScheduleItem(DayOfWeek day, TimeSpan start, TimeSpan end,
                            Guid subjectId, string room, string lessonType)
        {
            Id = Guid.NewGuid();
            DayOfWeek = day;
            StartTime = start;
            EndTime = end;
            SubjectId = subjectId;
            Room = room;
            LessonType = lessonType;
        }

        // Назва дня тижня українською
        // ВИПРАВЛЕНО: switch expression => не підтримується .NET 4.7.2, замінено на switch statement
        public string DayName
        {
            get
            {
                switch (DayOfWeek)
                {
                    case DayOfWeek.Monday: return "Понеділок";
                    case DayOfWeek.Tuesday: return "Вівторок";
                    case DayOfWeek.Wednesday: return "Середа";
                    case DayOfWeek.Thursday: return "Четвер";
                    case DayOfWeek.Friday: return "П'ятниця";
                    case DayOfWeek.Saturday: return "Субота";
                    case DayOfWeek.Sunday: return "Неділя";
                    default: return "Невідомо";
                }
            }
        }

        public string TimeString
        {
            get
            {
                return string.Format("{0:hh\\:mm} - {1:hh\\:mm}", StartTime, EndTime);
            }
        }

        public override string ToString()
        {
            return DayName + " | " + TimeString + " | Ауд. " + Room;
        }
    }
}