using System;
using System.Drawing;
using System.Windows.Forms;
using InventoryApp.Models;

namespace InventoryApp.Views
{
    public class AddProductView : UserControl
    {
        private TextBox txtName;
        private ComboBox cmbCategory;
        private NumericUpDown numPrice;
        private NumericUpDown numSellingPrice;
        private TextBox txtSupplier;
        private NumericUpDown numQuantity;
        private NumericUpDown numReorderLevel;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTitle;
        
        private int _editingId = 0;

        public event EventHandler<Product>? SaveClicked;
        public event EventHandler? CancelClicked;

        public AddProductView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.txtName = new TextBox();
            this.cmbCategory = new ComboBox();
            this.numPrice = new NumericUpDown();
            this.numSellingPrice = new NumericUpDown();
            this.txtSupplier = new TextBox();
            this.numQuantity = new NumericUpDown();
            this.numReorderLevel = new NumericUpDown();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            this.BackColor = Color.White;
            this.Padding = new Padding(30);

            // lblTitle
            this.lblTitle.Text = "Add New Product";
            this.lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            this.lblTitle.Dock = DockStyle.Top;
            this.lblTitle.Height = 50;
            this.Controls.Add(this.lblTitle);

            // Use a TableLayoutPanel for strict form structure
            TableLayoutPanel formTable = new TableLayoutPanel {
                Dock = DockStyle.Top, // Stick to top but will be centered in a scrollable panel
                ColumnCount = 2,
                RowCount = 7,
                AutoSize = true,
                Padding = new Padding(0, 20, 0, 0)
            };
            formTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
            formTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400));
            
            // Container to center the form table
            FlowLayoutPanel formCentering = new FlowLayoutPanel {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(20)
            };
            this.Controls.Add(formCentering);
            formCentering.Controls.Add(formTable);

            int row = 0;
            AddFormRow(formTable, "Product Name", txtName, row++);
            AddFormRow(formTable, "Category", cmbCategory, row++);
            AddFormRow(formTable, "Cost Price", numPrice, row++);
            AddFormRow(formTable, "Selling Price", numSellingPrice, row++);
            AddFormRow(formTable, "Supplier Name", txtSupplier, row++);
            AddFormRow(formTable, "Current Stock Quantity", numQuantity, row++);
            AddFormRow(formTable, "Low Stock Reorder Level", numReorderLevel, row++);

            // Buttons row
            FlowLayoutPanel btnPanel = new FlowLayoutPanel { 
                AutoSize = true, 
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(200, 20, 0, 0) 
            };
            btnPanel.Controls.Add(btnSave);
            btnPanel.Controls.Add(btnCancel);
            formCentering.Controls.Add(btnPanel);

            // ComboBox Items
            cmbCategory.Items.AddRange(new string[] { "Electronics", "Furniture", "Clothing", "Food", "Other" });
            cmbCategory.SelectedIndex = 0;

            // Numeric Setup
            numPrice.Maximum = 1000000;
            numSellingPrice.Maximum = 1000000;
            numQuantity.Maximum = 10000;
            numReorderLevel.Maximum = 1000;
            numReorderLevel.Value = 5; // Default

            // btnSave properties
            this.btnSave.Text = "Save Product";
            this.btnSave.BackColor = Color.FromArgb(46, 204, 113);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.btnSave.Size = new Size(150, 45);
            this.btnSave.Cursor = Cursors.Hand;
            this.btnSave.Click += OnSave;

            // btnCancel properties
            this.btnCancel.Text = "Cancel";
            this.btnCancel.BackColor = Color.FromArgb(149, 165, 166);
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Size = new Size(100, 45);
            this.btnCancel.Cursor = Cursors.Hand;
            this.btnCancel.Click += (s, e) => CancelClicked?.Invoke(this, EventArgs.Empty);
        }

        private void AddFormRow(TableLayoutPanel table, string labelText, Control input, int row)
        {
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Extra height per row
            Label lbl = new Label { Text = labelText, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Font = new Font("Segoe UI", 11) };
            input.Dock = DockStyle.Fill;
            input.Margin = new Padding(0, 10, 0, 10);
            if (input is TextBox || input is ComboBox || input is NumericUpDown) input.Font = new Font("Segoe UI", 12);
            
            table.Controls.Add(lbl, 0, row);
            table.Controls.Add(input, 1, row);
        }

        public void LoadProduct(Product product)
        {
            _editingId = product.Id;
            lblTitle.Text = "Edit Product";
            txtName.Text = product.Name;
            cmbCategory.Text = product.Category;
            numPrice.Value = product.Price;
            numSellingPrice.Value = product.SellingPrice;
            txtSupplier.Text = product.Supplier;
            numQuantity.Value = product.StockQuantity;
            numReorderLevel.Value = product.ReorderLevel;
            btnSave.Text = "Update Product";
        }

        public void Clear()
        {
            _editingId = 0;
            lblTitle.Text = "Add New Product";
            txtName.Clear();
            cmbCategory.SelectedIndex = 0;
            numPrice.Value = 0;
            numSellingPrice.Value = 0;
            txtSupplier.Clear();
            numQuantity.Value = 0;
            numReorderLevel.Value = 5;
            btnSave.Text = "Save Product";
        }

        private void OnSave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a product name.");
                return;
            }

            var product = new Product
            {
                Id = _editingId,
                Name = txtName.Text,
                Category = cmbCategory.Text,
                Price = numPrice.Value,
                SellingPrice = numSellingPrice.Value,
                Supplier = txtSupplier.Text,
                StockQuantity = (int)numQuantity.Value,
                ReorderLevel = (int)numReorderLevel.Value
            };

            SaveClicked?.Invoke(this, product);
        }
    }
}
