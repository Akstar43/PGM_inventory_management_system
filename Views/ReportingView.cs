using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Printing;
using InventoryApp.Services;
using InventoryApp.Models;
using System.Collections.Generic;

namespace InventoryApp.Views
{
    public class ReportingView : UserControl
    {
        private readonly InventoryService _service;
        private Label lblTotalItems;
        private Label lblTotalValue;
        private Label lblLowStockCount;
        private DataGridView dgvSummary;
        private DataGridView dgvLowStock;
        private Button btnRefresh;
        private Button btnPrintCategories;
        private Button btnPrintLowStock;
        private Button btnPrintFull;
        
        private string _activeReportType = "";

        public ReportingView(InventoryService service)
        {
            _service = service;
            InitializeComponent();
            RefreshData();
        }

        private void InitializeComponent()
        {
            this.lblTotalItems = new Label();
            this.lblTotalValue = new Label();
            this.lblLowStockCount = new Label();
            this.dgvSummary = new DataGridView();
            this.dgvLowStock = new DataGridView();
            this.btnRefresh = new Button();
            this.btnPrintCategories = new Button();
            this.btnPrintLowStock = new Button();
            this.btnPrintFull = new Button();

            this.BackColor = Color.White;
            this.Padding = new Padding(30);
            this.AutoScroll = true; // Global fallback

            FlowLayoutPanel header = new FlowLayoutPanel { 
                Dock = DockStyle.Top, 
                AutoSize = true, 
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 0, 0, 20)
            };

            Label lblTitle = new Label { Text = "Reporting & Insights", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 5, 25, 0) };
            
            btnRefresh.Text = "Refresh Data";
            btnRefresh.Size = new Size(150, 35);
            btnRefresh.BackColor = Color.FromArgb(52, 152, 219);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Margin = new Padding(10, 0, 0, 0);
            btnRefresh.Click += (s, e) => RefreshData();

            btnPrintCategories.Text = "Print Category Summary";
            btnPrintCategories.Size = new Size(180, 35);
            btnPrintCategories.BackColor = Color.FromArgb(46, 204, 113);
            btnPrintCategories.ForeColor = Color.White;
            btnPrintCategories.FlatStyle = FlatStyle.Flat;
            btnPrintCategories.Margin = new Padding(10, 0, 0, 0);
            btnPrintCategories.Click += (s, e) => TriggerPrint("Categories");

            btnPrintLowStock.Text = "Print Low Stock Audit";
            btnPrintLowStock.Size = new Size(180, 35);
            btnPrintLowStock.BackColor = Color.FromArgb(231, 76, 60);
            btnPrintLowStock.ForeColor = Color.White;
            btnPrintLowStock.FlatStyle = FlatStyle.Flat;
            btnPrintLowStock.Margin = new Padding(10, 0, 0, 0);
            btnPrintLowStock.Click += (s, e) => TriggerPrint("LowStock");

            btnPrintFull.Text = "Print Full Audit";
            btnPrintFull.Size = new Size(150, 35);
            btnPrintFull.BackColor = Color.FromArgb(149, 165, 166);
            btnPrintFull.ForeColor = Color.White;
            btnPrintFull.FlatStyle = FlatStyle.Flat;
            btnPrintFull.Margin = new Padding(10, 0, 0, 0);
            btnPrintFull.Click += (s, e) => TriggerPrint("Full");

            header.Controls.Add(lblTitle);
            header.Controls.Add(btnRefresh);
            header.Controls.Add(btnPrintCategories);
            header.Controls.Add(btnPrintLowStock);
            header.Controls.Add(btnPrintFull);
            this.Controls.Add(header);

            TableLayoutPanel statsTable = new TableLayoutPanel { Dock = DockStyle.Top, Height = 120, ColumnCount = 3, RowCount = 1 };
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            this.Controls.Add(statsTable);

            statsTable.Controls.Add(CreateStatPanel("Total Products", lblTotalItems, Color.FromArgb(52, 152, 219)), 0, 0);
            statsTable.Controls.Add(CreateStatPanel("Inventory Value", lblTotalValue, Color.FromArgb(46, 204, 113)), 1, 0);
            statsTable.Controls.Add(CreateStatPanel("Low Stock Warnings", lblLowStockCount, Color.FromArgb(231, 76, 60)), 2, 0);

            TableLayoutPanel gridTable = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(0, 20, 0, 0) };
            gridTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            gridTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            gridTable.Controls.Add(CreateGridPanel("Category Stock Density", dgvSummary), 0, 0);
            gridTable.Controls.Add(CreateGridPanel("Current Low Stock Items", dgvLowStock), 1, 0);

            this.Controls.Add(gridTable);
            gridTable.BringToFront();
        }

        private Panel CreateStatPanel(string title, Label valLabel, Color color)
        {
            Panel p = new Panel { Dock = DockStyle.Fill, Margin = new Padding(10), BackColor = Color.FromArgb(245, 245, 245) };
            Label lbl = new Label { Text = title, Dock = DockStyle.Top, Height = 30, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10) };
            valLabel.Dock = DockStyle.Fill;
            valLabel.TextAlign = ContentAlignment.MiddleCenter;
            valLabel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            valLabel.ForeColor = color;
            p.Controls.Add(valLabel);
            p.Controls.Add(lbl);
            return p;
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
            lblTotalItems.Text = all.Count.ToString();
            
            // Valuation now based on Selling Price for revenue estimation
            lblTotalValue.Text = all.Sum(p => p.SellingPrice * p.StockQuantity).ToString("C0");
            lblLowStockCount.Text = all.Count(p => p.StockQuantity <= p.ReorderLevel).ToString();

            // Category Summary
            var summary = all.GroupBy(p => p.Category)
                             .Select(g => new { 
                                 Category = g.Key, 
                                 Items = g.Count(), 
                                 TotalValue = g.Sum(p => p.SellingPrice * p.StockQuantity).ToString("C0")
                             }).OrderByDescending(x => x.Items).ToList();

            dgvSummary.DataSource = summary;

            // Detailed Low Stock Report
            var lowStock = all.Where(p => p.StockQuantity <= p.ReorderLevel)
                              .Select(p => new {
                                  p.Name,
                                  p.Category,
                                  p.StockQuantity,
                                  p.ReorderLevel,
                                  Status = p.StockQuantity == 0 ? "OUT OF STOCK" : "Low"
                              }).ToList();

            dgvLowStock.DataSource = lowStock;

            // Human-readable names for Reports
            if (dgvSummary.Columns["Items"] != null) dgvSummary.Columns["Items"].HeaderText = "Item Count";
            if (dgvSummary.Columns["TotalValue"] != null) dgvSummary.Columns["TotalValue"].HeaderText = "Estimated Value";
            
            if (dgvLowStock.Columns["StockQuantity"] != null) dgvLowStock.Columns["StockQuantity"].HeaderText = "In Stock";
            if (dgvLowStock.Columns["ReorderLevel"] != null) dgvLowStock.Columns["ReorderLevel"].HeaderText = "Min Level";
        }

        private void TriggerPrint(string type)
        {
            _activeReportType = type;
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);
            
            // Set Landscape for Full Audit
            if (type == "Full") pd.DefaultPageSettings.Landscape = true;

            PrintDialog dlg = new PrintDialog { Document = pd };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            var all = _service.GetAllProducts();
            Graphics g = e.Graphics;
            float y = 50;
            Font fTitle = new Font("Segoe UI", 24, FontStyle.Bold);
            Font fSub = new Font("Segoe UI", 12, FontStyle.Italic);
            Font fHeader = new Font("Segoe UI", 11, FontStyle.Bold);
            Font fText = new Font("Segoe UI", 10);
            Brush b = Brushes.Black;

            // Report Header
            string title = _activeReportType == "Categories" ? "CATEGORY VALUATION SUMMARY" : 
                           (_activeReportType == "LowStock" ? "CRITICAL LOW STOCK AUDIT" : "FULL INVENTORY MASTER LIST");
            
            g.DrawString(title, fTitle, b, 50, y); y += 45;
            g.DrawString("Generated on: " + DateTime.Now.ToString("F"), fSub, b, 50, y); y += 40;
            g.DrawLine(new Pen(Color.Black, 2), 50, y, e.PageBounds.Width - 50, y); y += 30;

            if (_activeReportType == "Categories")
            {
                var summary = all.GroupBy(p => p.Category)
                             .Select(g => new { 
                                 Name = g.Key, 
                                 Items = g.Count(), 
                                 Value = g.Sum(p => p.SellingPrice * p.StockQuantity)
                             }).OrderByDescending(x => x.Items).ToList();

                g.DrawString("CATEGORY NAME", fHeader, b, 50, y);
                g.DrawString("ITEM COUNT", fHeader, b, 350, y);
                g.DrawString("TOTAL VALUATION", fHeader, b, 550, y);
                y += 30;

                foreach (var item in summary)
                {
                    g.DrawString(item.Name, fText, b, 50, y);
                    g.DrawString(item.Items.ToString(), fText, b, 350, y);
                    g.DrawString(item.Value.ToString("C0"), fText, b, 550, y);
                    y += 25;
                }
            }
            else if (_activeReportType == "LowStock")
            {
                var low = all.Where(p => p.StockQuantity <= p.ReorderLevel).ToList();
                g.DrawString("PRODUCT NAME", fHeader, b, 50, y);
                g.DrawString("STOCK", fHeader, b, 350, y);
                g.DrawString("REORDER LVL", fHeader, b, 500, y);
                g.DrawString("STATUS", fHeader, b, 650, y);
                y += 30;

                foreach (var p in low)
                {
                    Brush statusBrush = p.StockQuantity == 0 ? Brushes.Red : Brushes.Black;
                    g.DrawString(p.Name, fText, b, 50, y);
                    g.DrawString(p.StockQuantity.ToString(), fText, b, 350, y);
                    g.DrawString(p.ReorderLevel.ToString(), fText, b, 500, y);
                    g.DrawString(p.StockQuantity == 0 ? "OUT OF STOCK" : "Low", fText, statusBrush, 650, y);
                    y += 25;
                }
            }
            else // Full Audit
            {
                g.DrawString("NAME", fHeader, b, 50, y);
                g.DrawString("CATEGORY", fHeader, b, 300, y);
                g.DrawString("SUPPLIER", fHeader, b, 550, y);
                g.DrawString("STOCK", fHeader, b, 800, y);
                g.DrawString("PRICE", fHeader, b, 900, y);
                y += 30;

                foreach (var p in all)
                {
                    if (y > e.PageBounds.Height - 100) { e.HasMorePages = true; break; }
                    g.DrawString(p.Name, fText, b, 50, y);
                    g.DrawString(p.Category, fText, b, 300, y);
                    g.DrawString(p.Supplier, fText, b, 550, y);
                    g.DrawString(p.StockQuantity.ToString(), fText, b, 800, y);
                    g.DrawString(p.SellingPrice.ToString("C0"), fText, b, 900, y);
                    y += 25;
                }
            }

            // Footer
            y = e.PageBounds.Height - 60;
            g.DrawLine(Pens.LightGray, 50, y, e.PageBounds.Width - 50, y);
            g.DrawString("Inventory Management System Audit - Student Page 1", new Font("Segoe UI", 8), Brushes.Gray, 50, y + 10);
        }
    }
}
