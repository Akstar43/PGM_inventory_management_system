using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using InventoryApp.Services;
using InventoryApp.Models;

namespace InventoryApp.Views
{
    public class StockControlView : UserControl
    {
        private DataGridView dgvStockIn;
        private DataGridView dgvStockOut;
        private Button btnRefresh;
        private ComboBox cmbProducts;
        private NumericUpDown numAdjustment;
        private Button btnStockIn;
        private Button btnStockOut;
        private readonly InventoryService _service;

        public StockControlView(InventoryService service)
        {
            _service = service;
            InitializeComponent();
            RefreshData();
        }

        private void InitializeComponent()
        {
            this.dgvStockIn = new DataGridView();
            this.dgvStockOut = new DataGridView();
            this.btnRefresh = new Button();
            this.cmbProducts = new ComboBox();
            this.numAdjustment = new NumericUpDown();
            this.btnStockIn = new Button();
            this.btnStockOut = new Button();

            this.BackColor = Color.White;
            this.Padding = new Padding(30);

            Panel top = new Panel { Dock = DockStyle.Top, AutoSize = true, MinimumSize = new Size(0, 130) };
            
            FlowLayoutPanel titleRow = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(0, 5, 0, 10) };
            Label lblTitle = new Label { Text = "Stock Control & Adjustments", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 5, 20, 0) };
            btnRefresh.Text = "Refresh Grid";
            btnRefresh.Height = 35;
            btnRefresh.Width = 120;
            btnRefresh.Click += (s, e) => RefreshData();
            titleRow.Controls.Add(lblTitle);
            titleRow.Controls.Add(btnRefresh);

            FlowLayoutPanel adjustmentRow = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(0, 5, 0, 10) };
            
            Label lblProd = new Label { Text = "Product:", AutoSize = true, Margin = new Padding(0, 10, 5, 0) };
            cmbProducts.Width = 200;
            cmbProducts.DropDownStyle = ComboBoxStyle.DropDownList;

            Label lblQty = new Label { Text = "Qty:", AutoSize = true, Margin = new Padding(15, 10, 5, 0) };
            numAdjustment.Width = 70;
            numAdjustment.Maximum = 1000;

            btnStockIn.Text = "Stock IN";
            btnStockIn.Size = new Size(110, 35);
            btnStockIn.BackColor = Color.FromArgb(46, 204, 113);
            btnStockIn.ForeColor = Color.White;
            btnStockIn.FlatStyle = FlatStyle.Flat;
            btnStockIn.Margin = new Padding(20, 0, 5, 0);
            btnStockIn.Click += (s, e) => HandleTransaction(true);

            btnStockOut.Text = "Stock OUT";
            btnStockOut.Size = new Size(120, 35);
            btnStockOut.BackColor = Color.FromArgb(231, 76, 60);
            btnStockOut.ForeColor = Color.White;
            btnStockOut.FlatStyle = FlatStyle.Flat;
            btnStockOut.Margin = new Padding(20, 0, 5, 0);
            btnStockOut.Click += (s, e) => HandleTransaction(false);

            adjustmentRow.Controls.Add(lblProd);
            adjustmentRow.Controls.Add(cmbProducts);
            adjustmentRow.Controls.Add(lblQty);
            adjustmentRow.Controls.Add(numAdjustment);
            adjustmentRow.Controls.Add(btnStockIn);
            adjustmentRow.Controls.Add(btnStockOut);

            top.Controls.Add(adjustmentRow);
            top.Controls.Add(titleRow);
            this.Controls.Add(top);

            TableLayoutPanel table = new TableLayoutPanel { 
                Dock = DockStyle.Fill, 
                ColumnCount = 2, 
                RowCount = 1, 
                Padding = new Padding(0, 20, 0, 0),
                BackColor = Color.White
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            this.Controls.Add(table);
            table.BringToFront();

            table.Controls.Add(CreateGridPanel("Current Stock (High Levels)", dgvStockIn), 0, 0);
            table.Controls.Add(CreateGridPanel("Current Stock (Low Levels)", dgvStockOut), 1, 0);
        }

        private Panel CreateGridPanel(string title, DataGridView dgv)
        {
            Panel p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), MinimumSize = new Size(0, 300) };
            Label lbl = new Label { Text = title, Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 12, FontStyle.Italic) };
            p.Controls.Add(lbl);

            dgv.Dock = DockStyle.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            p.Controls.Add(dgv);
            dgv.BringToFront();
            return p;
        }

        public void RefreshData()
        {
            var all = _service.GetAllProducts();
            dgvStockIn.DataSource = all.Where(p => p.StockQuantity > p.ReorderLevel).ToList();
            dgvStockOut.DataSource = all.Where(p => p.StockQuantity <= p.ReorderLevel).ToList();
            
            // Populate ComboBox
            cmbProducts.DataSource = null;
            cmbProducts.DataSource = all;
            cmbProducts.DisplayMember = "Name";
            cmbProducts.ValueMember = "Id";

            // Format columns
            FormatGrid(dgvStockIn);
            FormatGrid(dgvStockOut);
        }

        private void HandleTransaction(bool isStockIn)
        {
            if (cmbProducts.SelectedValue == null) return;
            int productId = (int)cmbProducts.SelectedValue;
            int qty = (int)numAdjustment.Value;
            if (qty <= 0) return;

            int change = isStockIn ? qty : -qty;
            string result = _service.ProcessTransaction(productId, change);

            MessageBox.Show(result, isStockIn ? "Stock In" : "Stock Out", 
                MessageBoxButtons.OK, 
                result.StartsWith("Error") ? MessageBoxIcon.Error : (result.StartsWith("Alert") ? MessageBoxIcon.Warning : MessageBoxIcon.Information));

            RefreshData();
            numAdjustment.Value = 0;
        }

        private void FormatGrid(DataGridView dgv)
        {
            if (dgv.Columns == null || dgv.Columns.Count == 0) return;
            if (dgv.Columns["Id"] != null) dgv.Columns["Id"].Visible = false;
            if (dgv.Columns["CreatedAt"] != null) dgv.Columns["CreatedAt"].Visible = false;
            if (dgv.Columns["TotalValue"] != null) dgv.Columns["TotalValue"].Visible = false;

            if (dgv.Columns["SellingPrice"] != null) dgv.Columns["SellingPrice"].HeaderText = "Selling Price";
            if (dgv.Columns["StockQuantity"] != null) dgv.Columns["StockQuantity"].HeaderText = "Current Stock";
            if (dgv.Columns["ReorderLevel"] != null) dgv.Columns["ReorderLevel"].HeaderText = "Alert Level";
        }
    }
}
