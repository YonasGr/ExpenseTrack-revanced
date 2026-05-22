using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Expense_Tracker
{
    public partial class Form1 : Form
    {
        private List<Expense> expenses = new List<Expense>();
        private int selectedExpenseId = -1;
        private int nextId = 1;
        private string filePath = "expenses.json";

        public Form1()
        {
            InitializeComponent();
            this.Text = "Expense Tracker System";
            SetupDataGridView();
            LoadData();
            RefreshDataGridView();
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;

            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Id", "ID");
            dataGridView1.Columns.Add("Amount", "Amount");
            dataGridView1.Columns.Add("Category", "Category");
            dataGridView1.Columns.Add("Description", "Description");
            dataGridView1.Columns.Add("Date", "Date");

            dataGridView1.Columns["Id"].Width = 50;
            dataGridView1.Columns["Amount"].Width = 100;
            dataGridView1.Columns["Category"].Width = 120;
            dataGridView1.Columns["Description"].Width = 200;
            dataGridView1.Columns["Date"].Width = 100;

            dataGridView1.Columns["Amount"].DefaultCellStyle.Format = "C2";
            dataGridView1.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void RefreshDataGridView(List<Expense> dataSource = null)
        {
            dataGridView1.Rows.Clear();
            List<Expense> listToDisplay = dataSource ?? expenses;

            foreach (var expense in listToDisplay)
            {
                dataGridView1.Rows.Add(
                    expense.Id,
                    expense.Amount,
                    expense.Category,
                    expense.Description,
                    expense.Date.ToShortDateString()
                );
            }

            UpdateTotal(listToDisplay);
        }

        private void UpdateTotal(List<Expense> list = null)
        {
            decimal total = (list ?? expenses).Sum(e => e.Amount);
            lblTotal.Text = $"Total: {total:C2}";
        }

        private void SaveData()
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(expenses);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var serializer = new JavaScriptSerializer();
                    expenses = serializer.Deserialize<List<Expense>>(json) ?? new List<Expense>();
                    if (expenses.Count > 0)
                    {
                        nextId = expenses.Max(e => e.Id) + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                expenses = new List<Expense>();
            }
        }

        private void ClearInputs()
        {
            txtAmount.Clear();
            txtDescription.Clear();
            cmbCategory.SelectedIndex = -1;
            dteDate.Value = DateTime.Now;
            selectedExpenseId = -1;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                MessageBox.Show("Please enter an amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return false;
            }

            // Remove dollar sign if present
            string cleanAmount = txtAmount.Text.Replace("$", "").Trim();

            if (!decimal.TryParse(cleanAmount, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid positive amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return false;
            }

            if (cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return false;
            }

            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            // Remove dollar sign and parse
            string cleanAmount = txtAmount.Text.Replace("$", "").Trim();
            decimal amount = decimal.Parse(cleanAmount);
            string category = cmbCategory.SelectedItem.ToString();
            string description = txtDescription.Text;
            DateTime date = dteDate.Value;

            Expense newExpense = new Expense
            {
                Id = nextId++,
                Amount = amount,
                Category = category,
                Description = description,
                Date = date
            };

            expenses.Add(newExpense);
            SaveData();
            RefreshDataGridView();
            ClearInputs();

            MessageBox.Show("Expense added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedExpenseId == -1)
            {
                MessageBox.Show("Please select an expense to update from the list.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs())
                return;

            var expense = expenses.FirstOrDefault(ex => ex.Id == selectedExpenseId);
            if (expense != null)
            {
                string cleanAmount = txtAmount.Text.Replace("$", "").Trim();
                expense.Amount = decimal.Parse(cleanAmount);
                expense.Category = cmbCategory.SelectedItem.ToString();
                expense.Description = txtDescription.Text;
                expense.Date = dteDate.Value;

                SaveData();
                RefreshDataGridView();
                ClearInputs();

                MessageBox.Show("Expense updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedExpenseId == -1)
            {
                MessageBox.Show("Please select an expense to delete from the list.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this expense?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var expense = expenses.FirstOrDefault(ex => ex.Id == selectedExpenseId);
                if (expense != null)
                {
                    expenses.Remove(expense);
                    SaveData();
                    RefreshDataGridView();
                    ClearInputs();

                    MessageBox.Show("Expense deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                RefreshDataGridView();
                return;
            }

            var filteredExpenses = expenses.Where(ex =>
                ex.Amount.ToString().Contains(searchText) ||
                ex.Category.ToLower().Contains(searchText) ||
                ex.Description.ToLower().Contains(searchText)
            ).ToList();

            RefreshDataGridView(filteredExpenses);

            if (filteredExpenses.Count == 0)
            {
                MessageBox.Show("No expenses found matching your search.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshDataGridView();
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedExpenseId = Convert.ToInt32(row.Cells["Id"].Value);

                var expense = expenses.FirstOrDefault(ex => ex.Id == selectedExpenseId);
                if (expense != null)
                {
                    txtAmount.Text = expense.Amount.ToString();
                    cmbCategory.SelectedItem = expense.Category;
                    txtDescription.Text = expense.Description;
                    dteDate.Value = expense.Date;
                }
            }
        }


        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits, decimal point, and control characters
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Allow only one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

}