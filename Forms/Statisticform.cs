using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentIS.Models;
using StudentIS.Services;

namespace StudentIS.Forms
{
    public class StatisticsForm : Form
    {
        private readonly Color ColorPrimary = Color.FromArgb(30, 58, 138);
        private readonly Color ColorAccent = Color.FromArgb(59, 130, 246);
        private readonly Color ColorSuccess = Color.FromArgb(34, 197, 94);
        private readonly Color ColorWarning = Color.FromArgb(234, 179, 8);
        private readonly Color ColorDanger = Color.FromArgb(239, 68, 68);
        private readonly Color ColorPurple = Color.FromArgb(168, 85, 247);

        public StatisticsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Статистика";
            this.Size = new Size(1000, 700);
            this.BackColor = Color.FromArgb(241, 245, 249);
            this.Font = new Font("Segoe UI", 9.5f);

            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = "Статистика навчання",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = ColorPrimary,
                AutoSize = true,
                Location = new Point(20, 20)
            };
            scroll.Controls.Add(lblTitle);

            int y = 65;

         
            var tasks = DataService.Data.Tasks;
            int total = tasks.Count;
            int done = tasks.Count(t => t.Status == TaskStatus.Completed);
            int progress = tasks.Count(t => t.Status == TaskStatus.InProgress);
            int notStart = tasks.Count(t => t.Status == TaskStatus.NotStarted);
            int overdue = tasks.Count(t => t.IsOverdue || t.Status == TaskStatus.Overdue);
            double completionRate = total > 0 ? (double)done / total * 100 : 0;

            scroll.Controls.Add(CreateSectionLabel("Завдання", y));
            y += 35;

            int cardX = 20;
            scroll.Controls.Add(CreateCard("Всього завдань", total.ToString(), ColorAccent, cardX, y)); cardX += 175;
            scroll.Controls.Add(CreateCard("Виконано", done.ToString(), ColorSuccess, cardX, y)); cardX += 175;
            scroll.Controls.Add(CreateCard("В процесі", progress.ToString(), ColorWarning, cardX, y)); cardX += 175;
            scroll.Controls.Add(CreateCard("Прострочено", overdue.ToString(), ColorDanger, cardX, y)); cardX += 175;
            scroll.Controls.Add(CreateCard("Не розпочато", notStart.ToString(), ColorPurple, cardX, y));
            y += 110;

            scroll.Controls.Add(new Label
            {
                Text = "Загальний прогрес: " + completionRate.ToString("F1") + "%",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(20, y)
            });
            y += 28;

            scroll.Controls.Add(CreateProgressBar(completionRate, y, 880));
            y += 28;

            y += 20;
            scroll.Controls.Add(CreateSectionLabel("Завдання по предметах", y));
            y += 38;

            var subjects = DataService.Data.Subjects;
            if (subjects.Count == 0)
            {
                scroll.Controls.Add(new Label { Text = "Предмети відсутні", Location = new Point(20, y), AutoSize = true, ForeColor = Color.FromArgb(100, 116, 139) });
                y += 30;
            }
            else
            {
                var grid = new DataGridView
                {
                    Location = new Point(20, y),
                    Size = new Size(880, Math.Min(200, subjects.Count * 38 + 42)),
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                    ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                    EnableHeadersVisualStyles = false,
                    GridColor = Color.FromArgb(226, 232, 240),
                    RowHeadersVisible = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    ReadOnly = true,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    RowTemplate = { Height = 36 }
                };

                grid.ColumnHeadersDefaultCellStyle.BackColor = ColorPrimary;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
                grid.ColumnHeadersHeight = 38;
                grid.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
                grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
                grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

                grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Предмет", FillWeight = 30 });
                grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Викладач", FillWeight = 25 });
                grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Всього завдань", FillWeight = 15 });
                grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Виконано", FillWeight = 15 });
                grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Прогрес %", FillWeight = 15 });

                foreach (var subj in subjects.OrderBy(s => s.Name))
                {
                    var subjTasks = tasks.Where(t => t.SubjectId == subj.Id).ToList();
                    int subjTotal = subjTasks.Count;
                    int subjDone = subjTasks.Count(t => t.Status == TaskStatus.Completed);
                    double pct = subjTotal > 0 ? (double)subjDone / subjTotal * 100 : 0;

                    grid.Rows.Add(subj.Name, subj.Teacher, subjTotal, subjDone, ((int)pct).ToString() + "%");
                }

                scroll.Controls.Add(grid);
                y += grid.Height + 20;
            }

            scroll.Controls.Add(CreateSectionLabel("Навантаження по днях тижня", y));
            y += 38;

            var dayOrder = new[] {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };
            var dayNames = new[] { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Нд" };

            var chartPanel = new Panel { Location = new Point(20, y), Size = new Size(880, 140), BackColor = Color.White };

            int maxLessons = 0;
            foreach (DayOfWeek d in dayOrder)
            {
                int cnt = DataService.Data.Schedule.Count(s => s.DayOfWeek == d);
                if (cnt > maxLessons) maxLessons = cnt;
            }

            int barWidth = 95, barGap = 28, maxHeight = 90;

            for (int i = 0; i < 7; i++)
            {
                int count = DataService.Data.Schedule.Count(s => s.DayOfWeek == dayOrder[i]);
                int barH = maxLessons > 0 ? (int)((double)count / maxLessons * maxHeight) : 0;
                bool isToday = dayOrder[i] == DateTime.Now.DayOfWeek;
                Color barColor = isToday ? ColorSuccess : ColorAccent;

                if (barH > 0)
                {
                    chartPanel.Controls.Add(new Panel
                    {
                        Location = new Point(i * (barWidth + barGap) + 15, maxHeight - barH + 10),
                        Size = new Size(barWidth, barH),
                        BackColor = barColor
                    });
                }

                chartPanel.Controls.Add(new Label
                {
                    Text = count.ToString(),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = barH > 0 ? barColor : Color.FromArgb(200, 200, 200),
                    Location = new Point(i * (barWidth + barGap) + 15, maxHeight - barH),
                    Size = new Size(barWidth, 18),
                    TextAlign = ContentAlignment.MiddleCenter
                });

                chartPanel.Controls.Add(new Label
                {
                    Text = dayNames[i] + (isToday ? " <" : ""),
                    Font = new Font("Segoe UI", 9.5f, isToday ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = isToday ? ColorSuccess : Color.FromArgb(51, 65, 85),
                    Location = new Point(i * (barWidth + barGap) + 15, 108),
                    Size = new Size(barWidth, 22),
                    TextAlign = ContentAlignment.MiddleCenter
                });
            }

            scroll.Controls.Add(chartPanel);
            y += 160;

            scroll.Controls.Add(CreateSectionLabel("Загальна інформація", y));
            y += 38;

            AddInfoRow(scroll, ref y,
                "Студент: " + DataService.Data.Student.Name,
                "Група: " + DataService.Data.Student.Group + "  |  Факультет: " + DataService.Data.Student.Faculty + "  |  " + DataService.Data.Student.Year + " курс");

            AddInfoRow(scroll, ref y,
                "Предметів: " + subjects.Count,
                "Кредитів загалом: " + subjects.Sum(s => s.Credits));

            AddInfoRow(scroll, ref y,
                "Занять у тижневому розкладі: " + DataService.Data.Schedule.Count,
                "Завдань активних: " + tasks.Count(t => t.Status != TaskStatus.Completed));

            this.Controls.Add(scroll);
        }

        private void AddInfoRow(Panel scroll, ref int y, string line1, string line2)
        {
            var panel = new Panel { Location = new Point(20, y), Size = new Size(880, 55), BackColor = Color.White };
            panel.Controls.Add(new Panel { Size = new Size(4, 55), Location = new Point(0, 0), BackColor = ColorAccent });
            panel.Controls.Add(new Label { Text = line1.ToString(), Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), AutoSize = false, Size = new Size(850, 22), Location = new Point(18, 7) });
            panel.Controls.Add(new Label { Text = line2.ToString(), Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(100, 116, 139), AutoSize = false, Size = new Size(850, 18), Location = new Point(18, 30) });
            scroll.Controls.Add(panel);
            y += 62;
        }

        private Label CreateSectionLabel(string text, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(20, y)
            };
        }

        private Panel CreateCard(string label, string value, Color color, int x, int y)
        {
            var card = new Panel { Size = new Size(160, 95), Location = new Point(x, y), BackColor = Color.White };
            card.Controls.Add(new Panel { Size = new Size(4, 95), Location = new Point(0, 0), BackColor = color });
            card.Controls.Add(new Label { Text = value, Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = color, AutoSize = false, Size = new Size(148, 48), Location = new Point(14, 8), TextAlign = ContentAlignment.MiddleLeft });
            card.Controls.Add(new Label { Text = label, Font = new Font("Segoe UI", 8.5f), ForeColor = Color.FromArgb(100, 116, 139), AutoSize = false, Size = new Size(148, 22), Location = new Point(14, 58), TextAlign = ContentAlignment.MiddleLeft });
            return card;
        }

        private Panel CreateProgressBar(double percent, int y, int width)
        {
            var container = new Panel { Location = new Point(20, y), Size = new Size(width, 18), BackColor = Color.FromArgb(226, 232, 240) };
            int fillWidth = (int)(width * percent / 100.0);
            if (fillWidth > 0)
            {
                Color fillColor;
                if (percent >= 75) fillColor = Color.FromArgb(34, 197, 94);
                else if (percent >= 40) fillColor = Color.FromArgb(234, 179, 8);
                else fillColor = Color.FromArgb(239, 68, 68);

                container.Controls.Add(new Panel { Location = new Point(0, 0), Size = new Size(fillWidth, 18), BackColor = fillColor });
            }
            return container;
        }
    }
}