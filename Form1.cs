using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Candel_Week05_Work04
{
    public partial class ProductForm : Form
    {
        public ProductForm()
        {
            InitializeComponent();
            txtCost.TextChanged += CalculatePrice;
            txtMarkup.TextChanged += CalculatePrice;
        }

        private void CalculatePrice(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtCost.Text.Trim(), out decimal cost) &&
                decimal.TryParse(txtMarkup.Text.Trim(), out decimal markup))
            {
                decimal price = cost + (cost * (markup / 100));

                txtPrice.Text = price.ToString("F2");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string barcode = txtBarcode.Text.Trim();
            string name = txtName.Text.Trim();
            string description = txtDesc.Text.Trim();
            string category = comboxCateg.SelectedItem != null ? comboxCateg.SelectedItem.ToString() : "";
            string origin = rbLocal.Checked ? "Local" : (rbAbroad.Checked ? "Abroad" : "Unspecified");
            string status = cbActive.Checked ? "Active" : "Inactive";
            string cost = txtCost.Text.Trim();
            string price = txtPrice.Text.Trim();
            string markup = txtMarkup.Text.Trim();

            if (string.IsNullOrEmpty(barcode) || string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter at least the Barcode and Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string productSummary = $"Product Information:\n\n" +
                                    $"Barcode: {barcode}\n" +
                                    $"Name: {name}\n" +
                                    $"Description: {description}\n" +
                                    $"Category: {category}\n" +
                                    $"Origin: {origin}\n" +
                                    $"Status: {status}\n" +
                                    $"Cost: {cost}\n" +
                                    $"Price: {price}\n" +
                                    $"Markup (%): {markup}";

            MessageBox.Show(productSummary, "Product Saved Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string fileRow = $"{barcode},{name},{description},{category},{origin},{status},{cost},{price},{markup}";
            string filePath = "products.txt";

            File.AppendAllText(filePath, fileRow + Environment.NewLine);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtBarcode.Clear();
            txtName.Clear();
            txtDesc.Clear();
            if (comboxCateg.Items.Count > 0) comboxCateg.SelectedIndex = -1;
            rbLocal.Checked = false;
            rbAbroad.Checked = false;
            cbActive.Checked = false;
            txtCost.Clear();
            txtPrice.Clear();
            txtMarkup.Clear();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnViewProducts_Click(object sender, EventArgs e)
        {
            string filePath = "products.txt";

            if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
            {
                MessageBox.Show("No saved products found yet.", "Saved Products", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Form tableForm = new Form
            {
                Text = "Saved Products Inventory",
                Width = 800,
                Height = 450,
                StartPosition = FormStartPosition.CenterParent
            };

            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgv.Columns.Add("Barcode", "Barcode");
            dgv.Columns.Add("Name", "Name");
            dgv.Columns.Add("Description", "Description");
            dgv.Columns.Add("Category", "Category");
            dgv.Columns.Add("Origin", "Origin");
            dgv.Columns.Add("Status", "Status");
            dgv.Columns.Add("Cost", "Cost");
            dgv.Columns.Add("Price", "Price");
            dgv.Columns.Add("Markup", "Markup (%)");

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    string[] fields = line.Split(',');
                    if (fields.Length >= 9)
                    {
                        dgv.Rows.Add(fields[0], fields[1], fields[2], fields[3], fields[4], fields[5], fields[6], fields[7], fields[8]);
                    }
                }
            }

            tableForm.Controls.Add(dgv);
            tableForm.ShowDialog();
        }
    }
}
