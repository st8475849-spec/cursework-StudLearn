using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentIS.Models;
using StudentIS.Services;

namespace StudentIS.Forms
{
    public class MainForm : Form
    {
        private Panel panelSidebar;
        private Panel panelContent;
        private Label lblStudentName;
        private Label lblStudentInfo;
        private Label lblClock;
        private System.Windows.Forms.Timer clockTimer;

        private Button btnSubjects;
        private Button btnTasks;
        private Button btnSchedule;
        private Button btnStatistics;

        private readonly Color ColorPrimary = Color.FromArgb(30, 58, 138);
        private readonly Color ColorAccent = Color.FromArgb(59, 130, 246);
        private readonly Color ColorSuccess = Color.FromArgb(34, 197, 94);
        private readonly Color ColorDanger = Color.FromArgb(239, 68, 68);
        private readonly Color ColorSidebar = Color.FromArgb(15, 23, 42);
        private readonly Color ColorBg = Color.FromArgb(241, 245, 249);

        public MainForm()
        {
            InitializeComponent();
            LoadDashboard();
        }

        private void InitializeComponent()
        {
            this.Text = "Система управління навчальним процесом";
            this.Size = new Size(1100, 700);
            this.MinimumSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ColorBg;
            this.Font = new Font("Segoe UI", 9.5f);

            panelSidebar = new Panel
            {
                Width = 220,
                Dock = DockStyle.Left,
                BackColor = ColorSidebar
            };

            var panelLogo = new Panel
            {
                Height = 100,
                Dock = DockStyle.Top,
                BackColor = ColorPrimary
            };

            var lblLogo = new Label
            {
                Text = "S",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(50, 45),
                Location = new Point(15, 10),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblAppName = new Label
            {
                Text = "StudyPlanner",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(135, 25),
                Location = new Point(65, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblAppSub = new Label
            {
                Text = "Навчальна система",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = false,
                Size = new Size(135, 18),
                Location = new Point(65, 42),
                TextAlign = ContentAlignment.MiddleLeft
            };

            panelLogo.Controls.Add(lblLogo);
            panelLogo.Controls.Add(lblAppName);
            panelLogo.Controls.Add(lblAppSub);

            var panelStudent = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(30, 41, 59)
            };

            lblStudentName = new Label
            {
                Text = DataService.Data.Student.Name,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(190, 22),
                Location = new Point(15, 12),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblStudentInfo = new Label
            {
                Text = "Гр. " + DataService.Data.Student.Group + " | " + DataService.Data.Student.Year + " курс",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = false,
                Size = new Size(190, 18),
                Location = new Point(15, 36),
                TextAlign = ContentAlignment.MiddleLeft
            };

            panelStudent.Controls.Add(lblStudentName);
            panelStudent.Controls.Add(lblStudentInfo);

            string[] navTexts = { "  Головна", "  Предмети", "  Завдання", "  Розклад", "  Статистика" };
            string[] navTags = { "main", "subjects", "tasks", "schedule", "statistics" };

            var navPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorSidebar
            };

            int navY = 15;
            for (int i = 0; i < navTexts.Length; i++)
            {
                var btn = CreateNavButton(navTexts[i], navTags[i], navY);
                navPanel.Controls.Add(btn);
                navY += 52;

                switch (navTags[i])
                {
                    case "subjects": btnSubjects = btn; break;
                    case "tasks": btnTasks = btn; break;
                    case "schedule": btnSchedule = btn; break;
                    case "statistics": btnStatistics = btn; break;
                }
            }

            lblClock = new Label
            {
                Text = DateTime.Now.ToString("HH:mm  dd.MM.yyyy"),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 116, 139),
                Dock = DockStyle.Bottom,
                Height = 35,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(15, 20, 35)
            };

            clockTimer = new System.Windows.Forms.Timer();
            clockTimer.Interval = 1000;
            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Start();

            panelSidebar.Controls.Add(navPanel);
            panelSidebar.Controls.Add(panelStudent);
            panelSidebar.Controls.Add(panelLogo);
            panelSidebar.Controls.Add(lblClock);

            panelContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorBg,
                Padding = new Padding(20)
            };

            this.Controls.Add(panelContent);
            this.Controls.Add(panelSidebar);

            this.FormClosed += MainForm_FormClosed;
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            lblClock.Text = DateTime.Now.ToString("HH:mm  dd.MM.yyyy");
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DataService.Save();
        }

        private Button CreateNavButton(string text, string tag, int y)
        {
            var btn = new Button
            {
                Text = text,
                Tag = tag,
                Size = new Size(198, 44),
                Location = new Point(1, y),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(203, 213, 225),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += NavBtn_MouseEnter;
            btn.MouseLeave += NavBtn_MouseLeave;
            btn.Click += NavButton_Click;
            return btn;
        }

        private void NavBtn_MouseEnter(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.BackColor = Color.FromArgb(30, 41, 59);
            btn.ForeColor = Color.White;
        }

        private void NavBtn_MouseLeave(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.BackColor = Color.Transparent;
            btn.ForeColor = Color.FromArgb(203, 213, 225);
        }

        private void NavButton_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            string tag = btn.Tag != null ? btn.Tag.ToString() : "";
            switch (tag)
            {
                case "main": LoadDashboard(); break;
                case "subjects": OpenForm(new SubjectsForm()); break;
                case "tasks": OpenForm(new TasksForm()); break;
                case "schedule": OpenForm(new ScheduleForm()); break;
                case "statistics": OpenForm(new StatisticsForm()); break;
            }
        }

        private void OpenForm(Form form)
        {
            panelContent.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.BackColor = ColorBg;
            panelContent.Controls.Add(form);
            form.Show();
        }

        private void LoadDashboard()
        {
            panelContent.Controls.Clear();

            var scroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            var lblTitle = new Label
            {
                Text = "Вітаємо, " + DataService.Data.Student.Name + "!",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            System.Globalization.CultureInfo ukCulture =
                new System.Globalization.CultureInfo("uk-UA");
            var lblDate = new Label
            {
                Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", ukCulture),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(10, 50)
            };

            scroll.Controls.Add(lblTitle);
            scroll.Controls.Add(lblDate);


            int subjCount = DataService.Data.Subjects.Count;
            int taskCount = DataService.Data.Tasks.Count;
            int doneCount = DataService.Data.Tasks.Count(t => t.Status == TaskStatus.Completed);
            int overdueCount = DataService.Data.Tasks.Count(t => t.IsOverdue);

            int cardX = 10, cardY = 85;
            scroll.Controls.Add(CreateStatCard("Предметів", subjCount.ToString(), Color.FromArgb(59, 130, 246), cardX, cardY)); cardX += 175;
            scroll.Controls.Add(CreateStatCard("Завдань", taskCount.ToString(), Color.FromArgb(168, 85, 247), cardX, cardY)); cardX += 175;
            scroll.Controls.Add(CreateStatCard("Виконано", doneCount.ToString(), Color.FromArgb(34, 197, 94), cardX, cardY)); cardX += 175;
            scroll.Controls.Add(CreateStatCard("Прострочено", overdueCount.ToString(), Color.FromArgb(239, 68, 68), cardX, cardY));

            int sectionY = cardY + 120;

            var lblUpcoming = new Label
            {
                Text = "Найближчі завдання",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(10, sectionY)
            };
            scroll.Controls.Add(lblUpcoming);
            sectionY += 35;

            var upcoming = DataService.Data.Tasks
                .Where(t => t.Status != TaskStatus.Completed)
                .OrderBy(t => t.Deadline)
                .Take(5)
                .ToList();

            if (upcoming.Count == 0)
            {
                scroll.Controls.Add(new Label
                {
                    Text = "Усі завдання виконано!",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Location = new Point(10, sectionY)
                });
            }
            else
            {
                foreach (var task in upcoming)
                {
                    var subject = DataService.Data.Subjects
                        .FirstOrDefault(s => s.Id == task.SubjectId);
                    string subjectName = subject != null ? subject.Name : "Без предмета";
                    scroll.Controls.Add(CreateTaskRow(task, subjectName, sectionY));
                    sectionY += 60;
                }
            }

            sectionY += 20;
            var lblTodaySchedule = new Label
            {
                Text = "Заняття сьогодні",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(10, sectionY)
            };
            scroll.Controls.Add(lblTodaySchedule);
            sectionY += 35;

            var todayItems = DataService.Data.Schedule
                .Where(s => s.DayOfWeek == DateTime.Now.DayOfWeek)
                .OrderBy(s => s.StartTime)
                .ToList();

            if (todayItems.Count == 0)
            {
                scroll.Controls.Add(new Label
                {
                    Text = "Сьогодні занять немає",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Location = new Point(10, sectionY)
                });
            }
            else
            {
                foreach (var item in todayItems)
                {
                    var subject = DataService.Data.Subjects
                        .FirstOrDefault(s => s.Id == item.SubjectId);
                    string subjectName = subject != null ? subject.Name : "?";
                    scroll.Controls.Add(CreateScheduleRow(item, subjectName, sectionY));
                    sectionY += 55;
                }
            }

            panelContent.Controls.Add(scroll);
        }

        private Panel CreateStatCard(string label, string value, Color color, int x, int y)
        {
            var card = new Panel
            {
                Size = new Size(160, 100),
                Location = new Point(x, y),
                BackColor = Color.White
            };

            var accent = new Panel
            {
                Size = new Size(5, 100),
                Location = new Point(0, 0),
                BackColor = color
            };

            var lblVal = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = color,
                AutoSize = false,
                Size = new Size(140, 50),
                Location = new Point(18, 12),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblLbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = false,
                Size = new Size(140, 22),
                Location = new Point(18, 62),
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.Add(accent);
            card.Controls.Add(lblVal);
            card.Controls.Add(lblLbl);
            return card;
        }

        private Panel CreateTaskRow(TaskItem task, string subjectName, int y)
        {
            var panel = new Panel
            {
                Size = new Size(680, 50),
                Location = new Point(10, y),
                BackColor = Color.White
            };

            Color leftColor;
            if (task.IsOverdue)
                leftColor = Color.FromArgb(239, 68, 68);
            else if (task.Priority == TaskPriority.High)
                leftColor = Color.FromArgb(234, 179, 8);
            else
                leftColor = Color.FromArgb(59, 130, 246);

            var accent = new Panel
            {
                Size = new Size(4, 50),
                Location = new Point(0, 0),
                BackColor = leftColor
            };

            var lblName = new Label
            {
                Text = task.Title,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = false,
                Size = new Size(300, 20),
                Location = new Point(14, 7)
            };

            var lblSubj = new Label
            {
                Text = subjectName,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = false,
                Size = new Size(200, 17),
                Location = new Point(14, 27)
            };

            int daysLeft = task.DaysLeft;
            string deadlineText;
            if (task.IsOverdue)
                deadlineText = "Прострочено на " + (-daysLeft) + " дн.";
            else if (daysLeft == 0)
                deadlineText = "Сьогодні!";
            else
                deadlineText = "Через " + daysLeft + " дн.";

            var lblDeadline = new Label
            {
                Text = task.Deadline.ToString("dd.MM.yyyy") + "  " + deadlineText,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = task.IsOverdue ? Color.FromArgb(239, 68, 68) : Color.FromArgb(100, 116, 139),
                AutoSize = false,
                Size = new Size(280, 35),
                Location = new Point(350, 8),
                TextAlign = ContentAlignment.MiddleRight
            };

            panel.Controls.Add(accent);
            panel.Controls.Add(lblName);
            panel.Controls.Add(lblSubj);
            panel.Controls.Add(lblDeadline);
            return panel;
        }

        private Panel CreateScheduleRow(ScheduleItem item, string subjectName, int y)
        {
            var panel = new Panel
            {
                Size = new Size(680, 46),
                Location = new Point(10, y),
                BackColor = Color.White
            };

            var accent = new Panel
            {
                Size = new Size(4, 46),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(34, 197, 94)
            };

            var lblTime = new Label
            {
                Text = item.TimeString,
                Font = new Font("Courier New", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                AutoSize = false,
                Size = new Size(120, 40),
                Location = new Point(14, 4),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblSubj = new Label
            {
                Text = subjectName,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = false,
                Size = new Size(300, 20),
                Location = new Point(140, 5)
            };

            var lblRoom = new Label
            {
                Text = "Ауд. " + item.Room + " | " + item.LessonType,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = false,
                Size = new Size(200, 16),
                Location = new Point(140, 25)
            };

            panel.Controls.Add(accent);
            panel.Controls.Add(lblTime);
            panel.Controls.Add(lblSubj);
            panel.Controls.Add(lblRoom);
            return panel;
        }
    }
}