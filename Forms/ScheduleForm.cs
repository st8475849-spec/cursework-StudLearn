using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentIS.Models;
using StudentIS.Services;

namespace StudentIS.Forms
{
    public class ScheduleForm : Form
    {
        private DataGridView grid;
        private ComboBox cmbFilterDay;
        private Button btnAdd, btnEdit, btnDelete;
        private Label lblCount;

        private readonly Color ColorPrimary = Color.FromArgb(30, 58, 138);
        private readonly Color ColorAccent = Color.FromArgb(59, 130, 246);
        private readonly Color ColorDanger = Color.FromArgb(239, 68, 68);

        private readonly DayOfWeek[] DayOrder = {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
            DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
        };

        public ScheduleForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Розклад занять";
            this.Size = new Size(900, 600);
            this.BackColor = Color.FromArgb(241, 245, 249);
            this.Font = new Font("Segoe UI", 9.5f);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.White };

            var lblTitle = new Label { Text = "Розклад занять", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = ColorPrimary, AutoSize = true, Location = new Point(15, 18) };

            var lblDay = new Label { Text = "День:", Location = new Point(220, 25), AutoSize = true, ForeColor = Color.FromArgb(100, 116, 139) };

            cmbFilterDay = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Size = new Size(140, 30), Location = new Point(265, 22) };
            cmbFilterDay.Items.Add("Всі дні");
            cmbFilterDay.Items.AddRange(new object[] { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця", "Субота", "Неділя" });
            cmbFilterDay.SelectedIndex = 0;
            cmbFilterDay.SelectedIndexChanged += (s, e) => LoadData();

            btnAdd = CreateButton("Додати", ColorAccent, new Point(425, 18));
            btnEdit = CreateButton("Змінити", Color.FromArgb(234, 179, 8), new Point(555, 18));
            btnDelete = CreateButton("Видалити", ColorDanger, new Point(685, 18));

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            panelTop.Controls.Add(lblTitle);
            panelTop.Controls.Add(lblDay);
            panelTop.Controls.Add(cmbFilterDay);
            panelTop.Controls.Add(btnAdd);
            panelTop.Controls.Add(btnEdit);
            panelTop.Controls.Add(btnDelete);

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
                RowTemplate = { Height = 42 }
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
            grid.CellFormatting += Grid_CellFormatting;
            grid.DoubleClick += (s, e) => BtnEdit_Click(s, e);

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Day", HeaderText = "День тижня", FillWeight = 18 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Time", HeaderText = "Час", FillWeight = 16 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subject", HeaderText = "Предмет", FillWeight = 28 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Тип", FillWeight = 16 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Room", HeaderText = "Аудиторія", FillWeight = 12 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Teacher", HeaderText = "Викладач", FillWeight = 22 });

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 35, BackColor = Color.White };
            lblCount = new Label { AutoSize = true, Location = new Point(15, 10), Font = new Font("Segoe UI", 8.5f), ForeColor = Color.FromArgb(100, 116, 139) };
            panelBottom.Controls.Add(lblCount);

            this.Controls.Add(grid);
            this.Controls.Add(panelTop);
            this.Controls.Add(panelBottom);
        }

        private void LoadData()
        {
            grid.Rows.Clear();

            var items = DataService.Data.Schedule.AsEnumerable();

            if (cmbFilterDay.SelectedIndex > 0)
            {
                DayOfWeek day = DayOrder[cmbFilterDay.SelectedIndex - 1];
                items = items.Where(i => i.DayOfWeek == day);
            }

            items = items
                .OrderBy(i => Array.IndexOf(DayOrder, i.DayOfWeek))
                .ThenBy(i => i.StartTime);

            foreach (var item in items)
            {
                Subject subject = DataService.Data.Subjects.FirstOrDefault(s => s.Id == item.SubjectId);
                string subjectName = subject != null ? subject.Name : "—";
                string teacherName = subject != null ? subject.Teacher : "—";

                int row = grid.Rows.Add(item.DayName, item.TimeString, subjectName, item.LessonType, "Ауд. " + item.Room, teacherName);
                grid.Rows[row].Tag = item.Id;

                if (item.DayOfWeek == DateTime.Now.DayOfWeek)
                    grid.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(240, 253, 244);
            }

            lblCount.Text = "Занять у розкладі: " + DataService.Data.Schedule.Count;
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (!(grid.Rows[e.RowIndex].Tag is Guid)) return;

            Guid id = (Guid)grid.Rows[e.RowIndex].Tag;
            ScheduleItem item = DataService.Data.Schedule.FirstOrDefault(s => s.Id == id);
            if (item == null) return;

            if (e.ColumnIndex == grid.Columns["Type"].Index)
            {
                switch (item.LessonType)
                {
                    case "Лекція": e.CellStyle.ForeColor = Color.FromArgb(30, 58, 138); break;
                    case "Практика": e.CellStyle.ForeColor = Color.FromArgb(4, 120, 87); break;
                    case "Лабораторна": e.CellStyle.ForeColor = Color.FromArgb(124, 45, 18); break;
                    default: e.CellStyle.ForeColor = Color.FromArgb(51, 65, 85); break;
                }
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (DataService.Data.Subjects.Count == 0)
            {
                MessageBox.Show("Спочатку додайте предмети!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var dialog = new ScheduleDialogForm(null);
            if (dialog.ShowDialog() == DialogResult.OK && dialog.Result != null)
            {
                DataService.Data.Schedule.Add(dialog.Result);
                DataService.Save();
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Оберіть заняття для редагування.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Guid id = (Guid)grid.SelectedRows[0].Tag;
            ScheduleItem item = DataService.Data.Schedule.FirstOrDefault(s => s.Id == id);
            if (item == null) return;

            var dialog = new ScheduleDialogForm(item);
            if (dialog.ShowDialog() == DialogResult.OK && dialog.Result != null)
            {
                item.DayOfWeek = dialog.Result.DayOfWeek;
                item.StartTime = dialog.Result.StartTime;
                item.EndTime = dialog.Result.EndTime;
                item.SubjectId = dialog.Result.SubjectId;
                item.Room = dialog.Result.Room;
                item.LessonType = dialog.Result.LessonType;
                DataService.Save();
                LoadData();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Оберіть заняття для видалення.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Guid id = (Guid)grid.SelectedRows[0].Tag;
            ScheduleItem item = DataService.Data.Schedule.FirstOrDefault(s => s.Id == id);
            if (item == null) return;

            if (MessageBox.Show("Видалити заняття у " + item.DayName + " о " + item.TimeString + "?",
                "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DataService.Data.Schedule.Remove(item);
                DataService.Save();
                LoadData();
            }
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


    public class ScheduleDialogForm : Form
    {
        public ScheduleItem Result { get; private set; }

        private ComboBox cmbDay, cmbSubject, cmbType;
        private DateTimePicker dtpStart, dtpEnd;
        private TextBox txtRoom;
        private ScheduleItem existing;

        private readonly DayOfWeek[] DayOrder = {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
            DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
        };

        public ScheduleDialogForm(ScheduleItem existingItem)
        {
            existing = existingItem;
            this.Text = (existing == null) ? "Додати заняття" : "Редагувати заняття";
            this.Size = new Size(400, 360);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            int y = 18;

        
            AddLabel("День тижня *", y);
            cmbDay = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(18, y + 22), Size = new Size(355, 30) };
            cmbDay.Items.AddRange(new object[] { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця", "Субота", "Неділя" });
            cmbDay.SelectedIndex = 0;
            this.Controls.Add(cmbDay);
            y += 62;

          
            AddLabel("Час початку", y);
            dtpStart = new DateTimePicker { Location = new Point(18, y + 22), Size = new Size(160, 30), Format = DateTimePickerFormat.Time, ShowUpDown = true, Value = DateTime.Today.AddHours(8) };
            this.Controls.Add(dtpStart);

            this.Controls.Add(new Label { Text = "Час закінчення", Location = new Point(200, y), Size = new Size(170, 20), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(51, 65, 85) });
            dtpEnd = new DateTimePicker { Location = new Point(200, y + 22), Size = new Size(173, 30), Format = DateTimePickerFormat.Time, ShowUpDown = true, Value = DateTime.Today.AddHours(9).AddMinutes(30) };
            this.Controls.Add(dtpEnd);
            y += 62;

         
            AddLabel("Предмет *", y);
            cmbSubject = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(18, y + 22), Size = new Size(355, 30) };
            foreach (var s in DataService.Data.Subjects.OrderBy(s => s.Name))
                cmbSubject.Items.Add(s);
            if (cmbSubject.Items.Count > 0) cmbSubject.SelectedIndex = 0;
            this.Controls.Add(cmbSubject);
            y += 62;

           
            AddLabel("Тип заняття", y);
            cmbType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(18, y + 22), Size = new Size(175, 30) };
            cmbType.Items.AddRange(new object[] { "Лекція", "Практика", "Лабораторна", "Семінар" });
            cmbType.SelectedIndex = 0;
            this.Controls.Add(cmbType);

            this.Controls.Add(new Label { Text = "Аудиторія", Location = new Point(210, y), Size = new Size(160, 20), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(51, 65, 85) });
            txtRoom = new TextBox { Location = new Point(210, y + 22), Size = new Size(163, 28), BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(txtRoom);
            y += 65;

      
            if (existing != null)
            {
                cmbDay.SelectedIndex = Array.IndexOf(DayOrder, existing.DayOfWeek);
                dtpStart.Value = DateTime.Today + existing.StartTime;
                dtpEnd.Value = DateTime.Today + existing.EndTime;
                txtRoom.Text = existing.Room;
                cmbType.SelectedItem = existing.LessonType;

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
                Location = new Point(130, y),
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
                Location = new Point(260, y),
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
            this.Controls.Add(new Label { Text = text, Location = new Point(18, y), Size = new Size(355, 20), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(51, 65, 85) });
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (cmbSubject.SelectedItem == null)
            {
                MessageBox.Show("Оберіть предмет.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Subject subject = (Subject)cmbSubject.SelectedItem;
            string lessonType = cmbType.SelectedItem != null ? cmbType.SelectedItem.ToString() : "Лекція";

            Result = new ScheduleItem
            {
                Id = (existing != null) ? existing.Id : Guid.NewGuid(),
                DayOfWeek = DayOrder[cmbDay.SelectedIndex],
                StartTime = dtpStart.Value.TimeOfDay,
                EndTime = dtpEnd.Value.TimeOfDay,
                SubjectId = subject.Id,
                Room = txtRoom.Text.Trim(),
                LessonType = lessonType
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}