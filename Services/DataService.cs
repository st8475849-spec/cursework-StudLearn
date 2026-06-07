using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using StudentIS.Models;

namespace StudentIS.Services
{
    public class AppData
    {
        public Student Student { get; set; }
        public List<Subject> Subjects { get; set; }
        public List<TaskItem> Tasks { get; set; }
        public List<ScheduleItem> Schedule { get; set; }

        public AppData()
        {
            Student = new Student();
            Subjects = new List<Subject>();
            Tasks = new List<TaskItem>();
            Schedule = new List<ScheduleItem>();
        }
    }

    public static class DataService
    {
        private static readonly string DataFolder =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

        private static readonly string DataFile =
            Path.Combine(DataFolder, "data.json");

        public static AppData Data { get; private set; } = new AppData();

        public static void Load()
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                    Directory.CreateDirectory(DataFolder);

                if (File.Exists(DataFile))
                {
                    string json = File.ReadAllText(DataFile);
                    AppData loaded = JsonConvert.DeserializeObject<AppData>(json);
                    if (loaded != null)
                        Data = loaded;
                }
                else
                {
                    SeedTestData();
                    Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження даних:\n" + ex.Message,
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Data = new AppData();
                SeedTestData();
            }
        }

        public static void Save()
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                    Directory.CreateDirectory(DataFolder);

                string json = JsonConvert.SerializeObject(Data, Formatting.Indented);
                File.WriteAllText(DataFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка збереження даних:\n" + ex.Message,
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void SeedTestData()
        {
            Data.Student = new Student("Іваненко Іван", "КН-21", "Факультет інформатики", 2);

            var math = new Subject("Вища математика", "Петрович О.В.", "Лекція", 5, "Математичний аналіз та лінійна алгебра");
            var prog = new Subject("Програмування", "Міхалич М.І.", "Практика", 6, "C# та .NET");
            var db = new Subject("Бази даних", "Непереможна В.П.", "Лабораторна", 4, "SQL та NoSQL");

            Data.Subjects.Add(math);
            Data.Subjects.Add(prog);
            Data.Subjects.Add(db);

            Data.Tasks.Add(new TaskItem(
                "Лабораторна робота №1",
                "Розробити алгоритм сортування",
                DateTime.Now.AddDays(3),
                TaskPriority.High,
                prog.Id));

            Data.Tasks.Add(new TaskItem(
                "Контрольна з математики",
                "Теми: інтеграли, похідні",
                DateTime.Now.AddDays(7),
                TaskPriority.Medium,
                math.Id));

            Data.Tasks.Add(new TaskItem(
                "Звіт з БД",
                "ER-діаграма та SQL-запити",
                DateTime.Now.AddDays(14),
                TaskPriority.Low,
                db.Id));

            var done = new TaskItem(
                "Реферат з програмування",
                "Тема: патерни проектування",
                DateTime.Now.AddDays(-5),
                TaskPriority.Medium,
                prog.Id);
            done.Status = TaskStatus.Completed;
            Data.Tasks.Add(done);

            Data.Schedule.Add(new ScheduleItem(DayOfWeek.Monday,
                new TimeSpan(8, 0, 0), new TimeSpan(9, 30, 0), math.Id, "101", "Лекція"));

            Data.Schedule.Add(new ScheduleItem(DayOfWeek.Monday,
                new TimeSpan(10, 0, 0), new TimeSpan(11, 30, 0), prog.Id, "205", "Практика"));

            Data.Schedule.Add(new ScheduleItem(DayOfWeek.Wednesday,
                new TimeSpan(8, 0, 0), new TimeSpan(9, 30, 0), db.Id, "310", "Лабораторна"));

            Data.Schedule.Add(new ScheduleItem(DayOfWeek.Thursday,
                new TimeSpan(13, 0, 0), new TimeSpan(14, 30, 0), math.Id, "101", "Практика"));

            Data.Schedule.Add(new ScheduleItem(DayOfWeek.Friday,
                new TimeSpan(10, 0, 0), new TimeSpan(11, 30, 0), prog.Id, "205", "Лекція"));
        }
    }
}