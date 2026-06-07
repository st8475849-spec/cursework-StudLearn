using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentIS.Models;
using StudentIS.Services;

namespace StudentIS.Forms
{
    public class TasksForm : Form
    {
        private DataGridView grid;
        private TextBox txtSearch;
        private ComboBox cmbFilterStatus, cmbFilterSubject, cmbSort;
        private Button btnAdd, btnEdit, btnDelete, btnMarkDone;
        private Label lblCount;

        private readonly Color ColorPrimary = Color.FromArgb(30, 58, 138);
        private readonly Color ColorAccent = Color.FromArgb(59, 130, 246);
        private readonly Color ColorDanger = Color.FromArgb(239, 68, 68);
        private readonly Color ColorSuccess = Color.FromArgb(34, 197, 94);
        private readonly Color ColorWarning = Color.FromArgb(234, 179, 8);

        public TasksForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Завдання";
            this.Size = new Size(1050, 650);
            this.BackColor = Color.FromArgb(241, 245, 249);
            this.Font = new Font("Segoe UI", 9.5f);

            var panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 105,
                BackColor = Color.White
            };

            var lblTitle = new Label
            {
                Text = "Завдання",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = ColorPrimary,
                AutoSize = true,
                Location = new Point(15, 12)
            };

            txtSearch = new TextBox
            {
                Size = new Size(200, 30),
                Location = new Point(15, 52),
                Font = new Font("Segoe UI", 9.5f),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.Text = "Пошук завдань...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Enter += TxtSearch_Enter;
            txtSearch.Leave += TxtSearch_Leave;
            txtSearch.TextChanged += (s, e) => LoadData();

            var lblStatus = new Label { Text = "Статус:", Location = new Point(230, 55), AutoSize = true, ForeColor = Color.FromArgb(100, 116, 139) };

            cmbFilterStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Size = new Size(140, 28), Location = new Point(285, 52) };
            cmbFilterStatus.Items.AddRange(new object[] { "Всі статуси", "Не розпочато", "В процесі", "Виконано", "Прострочено" });
            cmbFilterStatus.SelectedIndex = 0;
            cmbFilterStatus.SelectedIndexChanged += (s, e) => LoadData();

            var lblSubj = new Label { Text = "Предмет:", Location = new Point(440, 55), AutoSize = true, ForeColor = Color.FromArgb(100, 116, 139) };

            cmbFilterSubject = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Size = new Size(160, 28), Location = new Point(505, 52) };
            RefreshSubjectFilter();
            cmbFilterSubject.SelectedIndexChanged += (s, e) => LoadData();

            var lblSort = new Label { Text = "Сортування:", Location = new Point(680, 55), AutoSize = true, ForeColor = Color.FromArgb(100, 116, 139) };

            cmbSort = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Size = new Size(150, 28), Location = new Point(765, 52) };
            cmbSort.Items.AddRange(new object[] { "За дедлайном", "За пріоритетом", "За назвою", "За датою" });
            cmbSort.SelectedIndex = 0;
            cmbSort.SelectedIndexChanged += (s, e) => LoadData();

            btnAdd = CreateButton("Додати", ColorAccent, new Point(15, 10));
            btnEdit = CreateButton("Змінити", ColorWarning, new Point(145, 10));
            btnMarkDone = CreateButton("Виконано", ColorSuccess, new Point(275, 10));
            btnDelete = CreateButton("Видалити", ColorDanger, new Point(405, 10));

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnMarkDone.Click += BtnMarkDone_Click;
            btnDelete.Click += BtnDelete_Click;

            panelTop.Controls.Add(lblTitle);
            panelTop.Controls.Add(btnAdd);
            panelTop.Controls.Add(btnEdit);
            panelTop.Controls.Add(btnMarkDone);
            panelTop.Controls.Add(btnDelete);
            panelTop.Controls.Add(txtSearch);
            panelTop.Controls.Add(lblStatus);
            panelTop.Controls.Add(cmbFilterStatus);
            panelTop.Controls.Add(lblSubj);
            panelTop.Controls.Add(cmbFilterSubject);
            panelTop.Controls.Add(lblSort);
            panelTop.Controls.Add(cmbSort);

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(226, 232, 240),
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowTemplate = { Height = 40 }
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
            grid.DoubleClick += (s, e) => BtnEdit_Click(s, e);
            grid.CellFormatting += Grid_CellFormatting;

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Title", HeaderText = "Назва завдання", FillWeight = 25 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subject", HeaderText = "Предмет", FillWeight = 20 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Deadline", HeaderText = "Дедлайн", FillWeight = 12 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "DaysLeft", HeaderText = "Залишилось", FillWeight = 12 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Priority", HeaderText = "Пріоритет", FillWeight = 12 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Статус", FillWeight = 15 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Created", HeaderText = "Створено", FillWeight = 12 });

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 35, BackColor = Color.White };
            lblCount = new Label { AutoSize = true, Location = new Point(15, 10), Font = new Font("Segoe UI", 8.5f), ForeColor = Color.FromArgb(100, 116, 139) };
            panelBottom.Controls.Add(lblCount);

            this.Controls.Add(grid);
            this.Controls.Add(panelTop);
            this.Controls.Add(panelBottom);
        }

        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Пошук завдань...") { txtSearch.Text = ""; txtSearch.ForeColor = Color.Black; }
        }

        private void TxtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Пошук завдань..."; txtSearch.ForeColor = Color.Gray; }
        }

        private void RefreshSubjectFilter()
        {
            cmbFilterSubject.Items.Clear();
            cmbFilterSubject.Items.Add("Всі предмети");
            foreach (var s in DataService.Data.Subjects.OrderBy(s => s.Name))
                cmbFilterSubject.Items.Add(s);
            cmbFilterSubject.SelectedIndex = 0;
        }

        private void LoadData()
        {
            grid.Rows.Clear();

            foreach (var t in DataService.Data.Tasks)
            {
                if (t.IsOverdue && (t.Status == TaskStatus.InProgress || t.Status == TaskStatus.NotStarted))
                    t.Status = TaskStatus.Overdue;
            }

            var tasks = DataService.Data.Tasks.AsEnumerable();

            string search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search) && search != "Пошук завдань...")
            {
                tasks = tasks.Where(t =>
                    t.Title.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    t.Description.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            switch (cmbFilterStatus.SelectedIndex)
            {
                case 1: tasks = tasks.Where(t => t.Status == TaskStatus.NotStarted); break;
                case 2: tasks = tasks.Where(t => t.Status == TaskStatus.InProgress); break;
                case 3: tasks = tasks.Where(t => t.Status == TaskStatus.Completed); break;
                case 4: tasks = tasks.Where(t => t.Status == TaskStatus.Overdue); break;
            }

            if (cmbFilterSubject.SelectedItem is Subject selectedSubj)
                tasks = tasks.Where(t => t.SubjectId == selectedSubj.Id);

            switch (cmbSort.SelectedIndex)
            {
                case 0: tasks = tasks.OrderBy(t => t.Deadline); break;
                case 1: tasks = tasks.OrderByDescending(t => t.Priority); break;
                case 2: tasks = tasks.OrderBy(t => t.Title); break;
                case 3: tasks = tasks.OrderByDescending(t => t.CreatedAt); break;
            }

            var list = tasks.ToList();

            foreach (var t in list)
            {
                var subject = DataService.Data.Subjects.FirstOrDefault(s => s.Id == t.SubjectId);
                string subjectName = subject != null ? subject.Name : "—";

                string daysText;
                if (t.Status == TaskStatus.Completed)
                    daysText = "—";
                else if (t.IsOverdue)
                    daysText = "-" + (-t.DaysLeft) + " дн.";
                else if (t.DaysLeft == 0)
                    daysText = "Сьогодні!";
                else
                    daysText = t.DaysLeft + " дн.";

                int row = grid.Rows.Add(
                    t.Title,
                    subjectName,
                    t.Deadline.ToString("dd.MM.yyyy"),
                    daysText,
                    t.PriorityText,
                    t.StatusText,
                    t.CreatedAt.ToString("dd.MM.yyyy"));

                grid.Rows[row].Tag = t.Id;
            }

            int total = list.Count;
            int done = list.Count(t => t.Status == TaskStatus.Completed);
            int overdue = list.Count(t => t.Status == TaskStatus.Overdue);
            lblCount.Text = "Показано: " + total + "  |  Виконано: " + done + "  |  Прострочено: " + overdue;

            DataService.Save();
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= grid.Rows.Count) return;
            if (!(grid.Rows[e.RowIndex].Tag is Guid)) return;

            Guid id = (Guid)grid.Rows[e.RowIndex].Tag;
            TaskItem task = DataService.Data.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return;

            Color rowColor;
            switch (task.Status)
            {
                case TaskStatus.Completed: rowColor = Color.FromArgb(240, 253, 244); break;
                case TaskStatus.Overdue: rowColor = Color.FromArgb(254, 242, 242); break;
                case TaskStatus.InProgress: rowColor = Color.FromArgb(239, 246, 255); break;
                default: rowColor = Color.White; break;
            }
            grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = rowColor;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (DataService.Data.Subjects.Count == 0)
            {
                MessageBox.Show("Спочатку додайте хоча б один предмет!",
                    "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dialog = new TaskDialogForm(null);
            if (dialog.ShowDialog() == DialogResult.OK && dialog.Result != null)
            {
                DataService.Data.Tasks.Add(dialog.Result);
                DataService.Save();
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { ShowSelectWarning(); return; }

            Guid id = (Guid)grid.SelectedRows[0].Tag;
            TaskItem task = DataService.Data.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return;

            var dialog = new TaskDialogForm(task);
            if (dialog.ShowDialog() == DialogResult.OK && dialog.Result != null)
            {
                task.Title = dialog.Result.Title;
                task.Description = dialog.Result.Description;
                task.Deadline = dialog.Result.Deadline;
                task.Priority = dialog.Result.Priority;
                task.SubjectId = dialog.Result.SubjectId;
                task.Status = dialog.Result.Status;
                DataService.Save();
                LoadData();
            }
        }

        private void BtnMarkDone_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { ShowSelectWarning(); return; }

            Guid id = (Guid)grid.SelectedRows[0].Tag;
            TaskItem task = DataService.Data.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return;

            task.Status = TaskStatus.Completed;
            DataService.Save();
            LoadData();
            MessageBox.Show("Завдання позначено як виконане!", "Відмінно!",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { ShowSelectWarning(); return; }

            Guid id = (Guid)grid.SelectedRows[0].Tag;
            TaskItem task = DataService.Data.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return;

            if (MessageBox.Show("Видалити завдання \"" + task.Title + "\"?",
                "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DataService.Data.Tasks.Remove(task);
                DataService.Save();
                LoadData();
            }
        }

        private void ShowSelectWarning()
        {
            MessageBox.Show("Оберіть завдання зі списку.", "Увага",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private Button CreateButton(string text, Color color, Point location)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(120, 34),
                Location = location,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }

    public class TaskDialogForm : Form
    {
        public TaskItem Result { get; private set; }

        private TextBox txtTitle, txtDescription;
        private DateTimePicker dtpDeadline;
        private ComboBox cmbSubject, cmbPriority, cmbStatus;
        private TaskItem existing;

        public TaskDialogForm(TaskItem existingTask)
        {
            existing = existingTask;
            this.Text = (existing == null) ? "Нове завдання" : "Редагувати завдання";
            this.Size = new Size(430, 460);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            int y = 20;

            AddLabel("Назва завдання *", y);
            txtTitle = new TextBox { Location = new Point(20, y + 22), Size = new Size(380, 28), BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(txtTitle);
            y += 62;

            AddLabel("Опис", y);
            txtDescription = new TextBox { Location = new Point(20, y + 22), Size = new Size(380, 55), Multiline = true, BorderStyle = BorderStyle.FixedSingle, ScrollBars = ScrollBars.Vertical };
            this.Controls.Add(txtDescription);
            y += 87;

            AddLabel("Дедлайн", y);
            dtpDeadline = new DateTimePicker { Location = new Point(20, y + 22), Size = new Size(200, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Now.AddDays(7) };
            this.Controls.Add(dtpDeadline);
            y += 62;

            AddLabel("Предмет *", y);
            cmbSubject = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(20, y + 22), Size = new Size(380, 28) };
            foreach (var s in DataService.Data.Subjects.OrderBy(s => s.Name))
                cmbSubject.Items.Add(s);
            if (cmbSubject.Items.Count > 0) cmbSubject.SelectedIndex = 0;
            this.Controls.Add(cmbSubject);
            y += 62;

            AddLabel("Пріоритет", y);
            cmbPriority = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(20, y + 22), Size = new Size(175, 28) };
            cmbPriority.Items.AddRange(new object[] { "Низький", "Середній", "Високий" });
            cmbPriority.SelectedIndex = 1;
            this.Controls.Add(cmbPriority);

            AddLabelAt("Статус", new Point(215, y));
            cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(215, y + 22), Size = new Size(185, 28) };
            cmbStatus.Items.AddRange(new object[] { "Не розпочато", "В процесі", "Виконано" });
            cmbStatus.SelectedIndex = 0;
            this.Controls.Add(cmbStatus);
            y += 65;

            if (existing != null)
            {
                txtTitle.Text = existing.Title;
                txtDescription.Text = existing.Description;
                dtpDeadline.Value = existing.Deadline;
                cmbPriority.SelectedIndex = (int)existing.Priority;
                cmbStatus.SelectedIndex = (existing.Status == TaskStatus.Overdue) ? 0 : (int)existing.Status;

                Subject subj = DataService.Data.Subjects.FirstOrDefault(s => s.Id == existing.SubjectId);
                if (subj != null)
                {
                    foreach (Subject item in cmbSubject.Items)
                    {
                        if (item.Id == subj.Id) { cmbSubject.SelectedItem = item; break; }
                    }
                }
            }

            var btnOk = new Button
            {
                Text = "Зберегти",
                Size = new Size(120, 36),
                Location = new Point(180, y),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            btnOk.FlatAppearance.BorderSize = 0;

            var btnCancel = new Button
            {
                Text = "Скасувати",
                DialogResult = DialogResult.Cancel,
                Size = new Size(110, 36),
                Location = new Point(310, y),
                BackColor = Color.FromArgb(226, 232, 240),
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            btnOk.Click += BtnOk_Click;
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void AddLabel(string text, int y)
        {
            this.Controls.Add(new Label { Text = text, Location = new Point(20, y), Size = new Size(380, 20), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(51, 65, 85) });
        }

        private void AddLabelAt(string text, Point loc)
        {
            this.Controls.Add(new Label { Text = text, Location = loc, Size = new Size(180, 20), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(51, 65, 85) });
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || cmbSubject.SelectedItem == null)
            {
                MessageBox.Show("Заповніть обов'язкові поля (*).", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Subject selectedSubject = (Subject)cmbSubject.SelectedItem;
            TaskPriority priority = (TaskPriority)cmbPriority.SelectedIndex;

            TaskStatus status;
            switch (cmbStatus.SelectedIndex)
            {
                case 1: status = TaskStatus.InProgress; break;
                case 2: status = TaskStatus.Completed; break;
                default: status = TaskStatus.NotStarted; break;
            }

            Result = new TaskItem
            {
                Id = (existing != null) ? existing.Id : Guid.NewGuid(),
                Title = txtTitle.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                Deadline = dtpDeadline.Value.Date,
                Priority = priority,
                Status = status,
                SubjectId = selectedSubject.Id,
                CreatedAt = (existing != null) ? existing.CreatedAt : DateTime.Now
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}