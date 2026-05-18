using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using InventoryApp.Models;
using InventoryApp.Services;

namespace InventoryApp.Views
{
    public class ProductManagementView : UserControl
    {
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private DataGridView dgvProducts;
        private Panel topPanel;
        
        private readonly InventoryService _service;
        
        public event EventHandler? AddClicked;
        public event EventHandler<Product>? EditClicked;
        public event EventHandler<int>? DeleteClicked;

        public ProductManagementView(InventoryService service)
        {
            _service = service;
            InitializeComponent();
            RefreshData();
        }

        private void InitializeComponent()
        {
            this.topPanel = new Panel();
            this.txtSearch = new TextBox();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.dgvProducts = new DataGridView();

            this.BackColor = Color.White;
            this.Padding = new Padding(30);

            // topPanel (Strict horizontal layout)
            TableLayoutPanel topLayout = new TableLayoutPanel {
                Dock = DockStyle.Top,
                Height = 85,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(0, 0, 0, 10)
            };
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            this.Controls.Add(topLayout);

            // txtSearch
            this.txtSearch.Dock = DockStyle.Fill;
            this.txtSearch.Margin = new Padding(0, 15, 20, 0);
            this.txtSearch.Font = new Font("Segoe UI", 12);
            this.txtSearch.PlaceholderText = "Search for Products...";
            this.txtSearch.TextChanged += (s, e) => {
                var results = _service.SearchProducts(txtSearch.Text);
                dgvProducts.DataSource = results;
            };
            topLayout.Controls.Add(this.txtSearch, 0, 0);

            // btnAdd
            this.btnAdd.Text = "➕ Add";
            this.btnAdd.BackColor = Color.FromArgb(46, 204, 113);
            this.btnAdd.ForeColor = Color.White;
            this.btnAdd.FlatStyle = FlatStyle.Flat;
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.Dock = DockStyle.Fill;
            this.btnAdd.Height = 45;
            this.btnAdd.Margin = new Padding(10, 15, 0, 0);
            this.btnAdd.Cursor = Cursors.Hand;
            this.btnAdd.Click += (s, e) => AddClicked?.Invoke(this, EventArgs.Empty);
            topLayout.Controls.Add(this.btnAdd, 1, 0);

            // btnEdit
            this.btnEdit.Text = "✏️ Edit";
            this.btnEdit.BackColor = Color.FromArgb(52, 152, 219);
            this.btnEdit.ForeColor = Color.White;
            this.btnEdit.FlatStyle = FlatStyle.Flat;
            this.btnEdit.Dock = DockStyle.Fill;
            this.btnEdit.Height = 45;
            this.btnEdit.Margin = new Padding(10, 15, 0, 0);
            this.btnEdit.Enabled = false;
            this.btnEdit.Click += (s, e) => HandleEdit();
            topLayout.Controls.Add(this.btnEdit, 2, 0);

            // btnDelete
            this.btnDelete.Text = "🗑️ Delete";
            this.btnDelete.BackColor = Color.FromArgb(231, 76, 60);
            this.btnDelete.ForeColor = Color.White;
            this.btnDelete.FlatStyle = FlatStyle.Flat;
            this.btnDelete.Dock = DockStyle.Fill;
            this.btnDelete.Height = 45;
            this.btnDelete.Margin = new Padding(10, 15, 0, 0);
            this.btnDelete.Enabled = false;
            this.btnDelete.Click += (s, e) => HandleDelete();
            topLayout.Controls.Add(this.btnDelete, 3, 0);

            // dgvProducts
            this.dgvProducts.Dock = DockStyle.Fill;
            this.dgvProducts.BackgroundColor = Color.White;
            this.dgvProducts.BorderStyle = BorderStyle.None;
            this.dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProducts.RowTemplate.Height = 40;
            this.dgvProducts.AllowUserToAddRows = false;
            this.dgvProducts.ReadOnly = true;
            this.dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            
            this.dgvProducts.SelectionChanged += (s, e) => {
                bool hasSelection = dgvProducts.SelectedRows.Count > 0;
                btnEdit.Enabled = hasSelection;
                btnDelete.Enabled = hasSelection;
            };

            this.Controls.Add(this.dgvProducts);
            this.dgvProducts.BringToFront();
        }

        public void RefreshData()
        {
            dgvProducts.DataSource = null;
            dgvProducts.DataSource = _service.GetAllProducts();
            
            if (dgvProducts.Columns["Id"] != null) dgvProducts.Columns["Id"].Visible = false;
            if (dgvProducts.Columns["CreatedAt"] != null) dgvProducts.Columns["CreatedAt"].Visible = false;
            
            // Human-readable names
            if (dgvProducts.Columns["SellingPrice"] != null) dgvProducts.Columns["SellingPrice"].HeaderText = "Selling Price";
            if (dgvProducts.Columns["StockQuantity"] != null) dgvProducts.Columns["StockQuantity"].HeaderText = "Current Stock";
            if (dgvProducts.Columns["ReorderLevel"] != null) dgvProducts.Columns["ReorderLevel"].HeaderText = "Alert Level";
            if (dgvProducts.Columns["TotalValue"] != null) dgvProducts.Columns["TotalValue"].HeaderText = "Total Value $";
        }

        private void HandleEdit()
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var product = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                EditClicked?.Invoke(this, product);
            }
        }

        private void HandleDelete()
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var product = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show($"Are you sure you want to delete '{product.Name}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    DeleteClicked?.Invoke(this, product.Id);
                    RefreshData();
                }
            }
        }
    }
}
