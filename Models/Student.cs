namespace StudentIS.Models
{
    public class Student
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string Faculty { get; set; }
        public int Year { get; set; }

        public Student()
        {
            Name = "Студент";
            Group = "";
            Faculty = "";
            Year = 1;
        }

        public Student(string name, string group, string faculty, int year)
        {
            Name = name;
            Group = group;
            Faculty = faculty;
            Year = year;
        }

        public override string ToString()
        {
            return Name + " | Група: " + Group + " | " + Faculty + " | " + Year + " курс";
        }
    }
}