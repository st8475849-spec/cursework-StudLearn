using System;

namespace StudentIS.Models
{
    public class Subject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
        public string Type { get; set; }
        public int Credits { get; set; }
        public string Description { get; set; }

        public Subject()
        {
            Id = Guid.NewGuid();
            Name = "";
            Teacher = "";
            Type = "Лекція";
            Credits = 3;
            Description = "";
        }

        public Subject(string name, string teacher, string type, int credits, string description)
        {
            Id = Guid.NewGuid();
            Name = name;
            Teacher = teacher;
            Type = type;
            Credits = credits;
            Description = description;
        }

        public override string ToString()
        {
            return Name + " (" + Teacher + ")";
        }
    }
}