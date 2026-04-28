using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Expense_Tracker
{
    public partial class Form1 : Form
    {
        private List<Expense> expenses = new List<Expense>();
        private int selectedExpenseId = -1;
        private int nextId = 1;

        public Form1()
        {
            InitializeComponent();
            SetupDataGridView();
            UpdateTotal();
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Id", "ID");
            dataGridView1.Columns.Add("Amount", "Amount");
            dataGridView1.Columns.Add("Category", "Category");
            dataGridView1.Columns.Add("Description", "Description");
            dataGridView1.Columns.Add("Date", "Date");

            dataGridView1.Columns["Id"].Width = 50;
            dataGridView1.Columns["Amount"].Width = 80;
            dataGridView1.Columns["Category"].Width = 100;
            dataGridView1.Columns["Description"].Width = 150;
            dataGridView1.Columns["Date"].Width = 100;

            dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void RefreshDataGridView()
        {
            dataGridView1.Rows.Clear();

            foreach (var expense in expenses)
            {
                dataGridView1.Rows.Add(
                    expense.Id,
                    expense.Amount.ToString("N2"),
                    expense.Category,
                    expense.Description,
                    expense.Date.ToShortDateString()
                );
            }

            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal total = expenses.Sum(e => e.Amount);
            lblTotal.Text = $"Total: {total:N2}";
        }

        private void ClearInputs()
        {
            txtAmount.Clear();
            textBox2.Clear();
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
            string description = textBox2.Text;
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
                expense.Description = textBox2.Text;
                expense.Date = dteDate.Value;

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
                    RefreshDataGridView();
                    ClearInputs();

                    MessageBox.Show("Expense deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                RefreshDataGridView();
                return;
            }

            // Remove dollar sign if present
            string cleanSearch = searchText.Replace("$", "").Trim();

            if (decimal.TryParse(cleanSearch, out decimal searchAmount))
            {
                var filteredExpenses = expenses.Where(ex => ex.Amount == searchAmount).ToList();
                DisplayFilteredResults(filteredExpenses);
            }
            else
            {
                MessageBox.Show("Please enter a valid numeric amount to search.", "Invalid Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DisplayFilteredResults(List<Expense> filteredExpenses)
        {
            dataGridView1.Rows.Clear();

            foreach (var expense in filteredExpenses)
            {
                dataGridView1.Rows.Add(
                    expense.Id,
                    expense.Amount.ToString("N2"),
                    expense.Category,
                    expense.Description,
                    expense.Date.ToShortDateString()
                );
            }

            decimal total = filteredExpenses.Sum(e => e.Amount);
            lblTotal.Text = $"Total: {total:N2}";

            if (filteredExpenses.Count == 0)
            {
                MessageBox.Show("No expenses found with the specified amount.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    textBox2.Text = expense.Description;
                    dteDate.Value = expense.Date;
                }
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
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

    public class Expense
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}