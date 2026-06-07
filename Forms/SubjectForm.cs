using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentIS.Models;
using StudentIS.Services;

namespace StudentIS.Forms
{
    public class SubjectsForm : Form
    {
        private DataGridView grid;
        private TextBox txtSearch;
        private Button btnAdd, btnEdit, btnDelete;
        private Label lblCount;

        private readonly Color ColorPrimary = Color.FromArgb(30, 58, 138);
        private readonly Color ColorAccent = Color.FromArgb(59, 130, 246);
        private readonly Color ColorDanger = Color.FromArgb(239, 68, 68);
        private readonly Color ColorBg = Color.FromArgb(241, 245, 249);

        public SubjectsForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Предмети";
            this.Size = new Size(900, 600);
            this.BackColor = ColorBg;
            this.Font = new Font("Segoe UI", 9.5f);

            var panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White
            };

            var lblTitle = new Label
            {
                Text = "Предмети",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = ColorPrimary,
                AutoSize = true,
                Location = new Point(15, 18)
            };

            txtSearch = new TextBox
            {
                Size = new Size(220, 30),
                Location = new Point(200, 20),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.Text = "Пошук предметів...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Enter += TxtSearch_Enter;
            txtSearch.Leave += TxtSearch_Leave;
            txtSearch.TextChanged += TxtSearch_TextChanged;

            btnAdd = CreateButton("Додати", ColorAccent, new Point(450, 20));
            btnEdit = CreateButton("Редагувати", Color.FromArgb(234, 179, 8), new Point(560, 20));
            btnDelete = CreateButton("Видалити", ColorDanger, new Point(690, 20));

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            panelTop.Controls.Add(lblTitle);
            panelTop.Controls.Add(txtSearch);
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
                RowTemplate = { Height = 40 }
            };

            grid.ColumnHeadersDefaultCellStyle.BackColor = ColorPrimary;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            grid.ColumnHeadersHeight = 38;
            grid.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            grid.DoubleClick += Grid_DoubleClick;

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Назва предмета", FillWeight = 30 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Teacher", HeaderText = "Викладач", FillWeight = 25 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Тип", FillWeight = 15 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Credits", HeaderText = "Кредити", FillWeight = 10 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Опис", FillWeight = 20 });

            var panelBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 35,
                BackColor = Color.White
            };

            lblCount = new Label
            {
                Text = "Усього: 0 предметів",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(15, 10)
            };

            panelBottom.Controls.Add(lblCount);
            this.Controls.Add(grid);
            this.Controls.Add(panelTop);
            this.Controls.Add(panelBottom);
        }

        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Пошук предметів...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void TxtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Пошук предметів...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text != "Пошук предметів...")
                LoadData(txtSearch.Text);
        }

        private void Grid_DoubleClick(object sender, EventArgs e)
        {
            BtnEdit_Click(sender, e);
        }

        private void LoadData(string filter = "")
        {
            grid.Rows.Clear();

            var items = DataService.Data.Subjects
                .Where(s => string.IsNullOrEmpty(filter) ||
                            s.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            s.Teacher.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(s => s.Name)
                .ToList();

            foreach (var s in items)
            {
                int row = grid.Rows.Add(s.Name, s.Teacher, s.Type, s.Credits + " кр.", s.Description);
                grid.Rows[row].Tag = s.Id;
            }

            lblCount.Text = "Усього: " + items.Count + " предметів";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var dialog = new SubjectDialogForm(null);
            if (dialog.ShowDialog() == DialogResult.OK && dialog.Result != null)
            {
                DataService.Data.Subjects.Add(dialog.Result);
                DataService.Save();
                LoadData();
                MessageBox.Show("Предмет успішно додано!", "Успіх",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Оберіть предмет для редагування.",
                    "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Guid id = (Guid)grid.SelectedRows[0].Tag;
            Subject subject = DataService.Data.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject == null) return;

            var dialog = new SubjectDialogForm(subject);
            if (dialog.ShowDialog() == DialogResult.OK && dialog.Result != null)
            {
                subject.Name = dialog.Result.Name;
                subject.Teacher = dialog.Result.Teacher;
                subject.Type = dialog.Result.Type;
                subject.Credits = dialog.Result.Credits;
                subject.Description = dialog.Result.Description;

                DataService.Save();
                LoadData();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Оберіть предмет для видалення.",
                    "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Guid id = (Guid)grid.SelectedRows[0].Tag;
            Subject subject = DataService.Data.Subjects.FirstOrDefault(s => s.Id == id);
            if (subject == null) return;

            DialogResult result = MessageBox.Show(
                "Видалити предмет \"" + subject.Name + "\"?\n\nТакож буде видалено пов'язані завдання та розклад.",
                "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                DataService.Data.Subjects.Remove(subject);
                DataService.Data.Tasks.RemoveAll(t => t.SubjectId == id);
                DataService.Data.Schedule.RemoveAll(s => s.SubjectId == id);
                DataService.Save();
                LoadData();
            }
        }

        private Button CreateButton(string text, Color color, Point location)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(120, 32),
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

    public class SubjectDialogForm : Form
    {
        public Subject Result { get; private set; }

        private TextBox txtName, txtTeacher, txtDescription;
        private ComboBox cmbType;
        private NumericUpDown numCredits;

        public SubjectDialogForm(Subject existing)
        {
            this.Text = (existing == null) ? "Додати предмет" : "Редагувати предмет";
            this.Size = new Size(420, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            int y = 20;

            AddLabel("Назва предмета *", y);
            txtName = new TextBox { Location = new Point(20, y + 22), Size = new Size(370, 28), BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(txtName);
            y += 62;

            AddLabel("Викладач *", y);
            txtTeacher = new TextBox { Location = new Point(20, y + 22), Size = new Size(370, 28), BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(txtTeacher);
            y += 62;

            AddLabel("Тип заняття", y);
            cmbType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(20, y + 22), Size = new Size(370, 28) };
            cmbType.Items.AddRange(new object[] { "Лекція", "Практика", "Лабораторна", "Семінар" });
            cmbType.SelectedIndex = 0;
            this.Controls.Add(cmbType);
            y += 62;

            AddLabel("Кількість кредитів", y);
            numCredits = new NumericUpDown { Minimum = 1, Maximum = 10, Value = 3, Location = new Point(20, y + 22), Size = new Size(100, 28) };
            this.Controls.Add(numCredits);
            y += 62;

            AddLabel("Опис (необов'язково)", y);
            txtDescription = new TextBox { Height = 55, Multiline = true, BorderStyle = BorderStyle.FixedSingle, Location = new Point(20, y + 22), Size = new Size(370, 55), ScrollBars = ScrollBars.Vertical };
            this.Controls.Add(txtDescription);
            y += 87;

            if (existing != null)
            {
                txtName.Text = existing.Name;
                txtTeacher.Text = existing.Teacher;
                cmbType.SelectedItem = existing.Type;
                numCredits.Value = existing.Credits;
                txtDescription.Text = existing.Description;
            }

            var btnOk = new Button
            {
                Text = "Зберегти",
                Size = new Size(120, 36),
                Location = new Point(170, y),
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
                Location = new Point(300, y),
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
            this.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(20, y),
                Size = new Size(370, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85)
            });
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtTeacher.Text))
            {
                MessageBox.Show("Заповніть обов'язкові поля (*).",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string typeSel = cmbType.SelectedItem != null ? cmbType.SelectedItem.ToString() : "Лекція";
            Result = new Subject(txtName.Text.Trim(), txtTeacher.Text.Trim(), typeSel,
                                 (int)numCredits.Value, txtDescription.Text.Trim());

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}