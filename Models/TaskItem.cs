using System;

namespace StudentIS.Models
{
    public enum TaskStatus
    {
        NotStarted,  
        InProgress,  
        Completed,   
        Overdue      
    }

  
    public enum TaskPriority
    {
        Low,     
        Medium,  
        High     
    }

    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid SubjectId { get; set; }

        public TaskItem()
        {
            Id = Guid.NewGuid();
            Title = "";
            Description = "";
            CreatedAt = DateTime.Now;
            Deadline = DateTime.Now.AddDays(7);
            Status = TaskStatus.NotStarted;
            Priority = TaskPriority.Medium;
        }

        public TaskItem(string title, string description, DateTime deadline,
                        TaskPriority priority, Guid subjectId)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Deadline = deadline;
            CreatedAt = DateTime.Now;
            Status = TaskStatus.NotStarted;
            Priority = priority;
            SubjectId = subjectId;
        }

        public bool IsOverdue
        {
            get { return Deadline < DateTime.Now && Status != TaskStatus.Completed; }
        }

        public int DaysLeft
        {
            get { return (int)(Deadline - DateTime.Now).TotalDays; }
        }

        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case TaskStatus.NotStarted: return "Не розпочато";
                    case TaskStatus.InProgress: return "В процесі";
                    case TaskStatus.Completed: return "Виконано";
                    case TaskStatus.Overdue: return "Прострочено";
                    default: return "Невідомо";
                }
            }
        }

        public string PriorityText
        {
            get
            {
                switch (Priority)
                {
                    case TaskPriority.Low: return "Низький";
                    case TaskPriority.Medium: return "Середній";
                    case TaskPriority.High: return "Високий";
                    default: return "Невідомо";
                }
            }
        }

        public override string ToString()
        {
            return Title + " | " + Deadline.ToString("dd.MM.yyyy") + " | " + StatusText;
        }
    }
}