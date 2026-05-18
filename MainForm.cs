using System;
using System.Drawing;
using System.Windows.Forms;
using InventoryApp.Services;
using InventoryApp.Views;
using InventoryApp.Models;

namespace InventoryApp
{
    public partial class MainForm : Form
    {
        private Panel sidePanel;
        private Panel contentPanel;
        private Label titleLabel;
        private Button btnDashboard;
        private Button btnProducts;
        private Button btnStock;
        private Button btnReports;
        
        private readonly DatabaseService _dbService;
        private readonly InventoryService _inventoryService;

        // Views
        private DashboardView dashboardView;
        private ProductManagementView productView;
        private AddProductView addProductView;
        private StockControlView stockView;
        private ReportingView reportingView;

        public MainForm()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            _inventoryService = new InventoryService(_dbService);
            
            InitializeViews();
            ShowDashboard();
        }

        private void InitializeViews()
        {
            dashboardView = new DashboardView();
            dashboardView.ProductManagementClicked += (s, e) => ShowProducts();
            dashboardView.StockControlClicked += (s, e) => ShowStock();
            dashboardView.ReportingClicked += (s, e) => ShowReports();

            productView = new ProductManagementView(_inventoryService);
            productView.AddClicked += (s, e) => ShowAddProduct();
            productView.EditClicked += (s, product) => ShowEditProduct(product);
            productView.DeleteClicked += (s, id) => {
                _inventoryService.DeleteProduct(id);
                productView.RefreshData();
            };

            addProductView = new AddProductView();
            addProductView.SaveClicked += (s, product) => {
                if (product.Id == 0) _inventoryService.AddProduct(product);
                else _inventoryService.UpdateProduct(product);
                ShowProducts();
            };
            addProductView.CancelClicked += (s, e) => ShowProducts();

            stockView = new StockControlView(_inventoryService);
            reportingView = new ReportingView(_inventoryService);
        }

        private void InitializeComponent()
        {
            this.sidePanel = new Panel();
            this.contentPanel = new Panel();
            this.titleLabel = new Label();
            this.btnDashboard = new Button();
            this.btnProducts = new Button();
            this.btnStock = new Button();
            this.btnReports = new Button();

            this.Text = "SAFARI RETAIL IMS";
            this.MinimumSize = new Size(1100, 750);
            this.Size = new Size(1200, 800);
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(44, 62, 80); // Sidebar color as base
            this.Font = new Font("Segoe UI", 10);

            // Container for strict separation
            TableLayoutPanel mainLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.FromArgb(44, 62, 80)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            this.Controls.Add(mainLayout);

            // sidePanel
            this.sidePanel.Dock = DockStyle.Fill;
            this.sidePanel.BackColor = Color.FromArgb(44, 62, 80);
            mainLayout.Controls.Add(this.sidePanel, 0, 0);

            // titleLabel
            this.titleLabel.Text = "SRIMS";
            this.titleLabel.ForeColor = Color.White;
            this.titleLabel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            this.titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.titleLabel.Dock = DockStyle.Top;
            this.titleLabel.Height = 120;
            this.sidePanel.Controls.Add(this.titleLabel);

            // Nav container for reliable vertical stacking
            FlowLayoutPanel navContainer = new FlowLayoutPanel {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent
            };
            this.sidePanel.Controls.Add(navContainer);
            navContainer.BringToFront();

            // Clear controls and add in specific order
            AddNavButton(navContainer, btnDashboard, "Dashboard", (s, e) => ShowDashboard());
            AddNavButton(navContainer, btnProducts, "Product Management", (s, e) => ShowProducts());
            AddNavButton(navContainer, btnStock, "Stock Control", (s, e) => ShowStock());
            AddNavButton(navContainer, btnReports, "Reports & Monitoring", (s, e) => ShowReports());

            // contentPanel
            this.contentPanel.Dock = DockStyle.Fill;
            this.contentPanel.AutoScroll = true;
            this.contentPanel.BackColor = Color.White;
            mainLayout.Controls.Add(this.contentPanel, 1, 0);
        }

        private void AddNavButton(FlowLayoutPanel container, Button btn, string text, EventHandler onClick)
        {
            btn.Text = text;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.ForeColor = Color.White;
            btn.Height = 60;
            btn.Width = 250; // Match sidebar width
            btn.Margin = new Padding(0);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(30, 0, 0, 0);
            btn.Click += onClick;
            btn.Cursor = Cursors.Hand;
            container.Controls.Add(btn);
        }

        private void SetActiveButton(Button activeBtn)
        {
            foreach (Control ctrl in sidePanel.Controls)
            {
                if (ctrl is Button btn) btn.BackColor = Color.Transparent;
            }
            activeBtn.BackColor = Color.FromArgb(52, 73, 94);
        }

        private void ShowView(UserControl view)
        {
            contentPanel.Controls.Clear();
            view.Dock = DockStyle.Fill;
            view.AutoScroll = true;
            // Force the view to rethink its layout on docking
            view.Size = contentPanel.ClientSize; 
            contentPanel.Controls.Add(view);
        }

        private void ShowDashboard()
        {
            SetActiveButton(btnDashboard);
            ShowView(dashboardView);
        }

        private void ShowProducts()
        {
            SetActiveButton(btnProducts);
            productView.RefreshData();
            ShowView(productView);
        }

        private void ShowAddProduct()
        {
            addProductView.Clear();
            ShowView(addProductView);
        }

        private void ShowEditProduct(Product product)
        {
            addProductView.LoadProduct(product);
            ShowView(addProductView);
        }

        private void ShowStock()
        {
            SetActiveButton(btnStock);
            stockView.RefreshData();
            ShowView(stockView);
        }

        private void ShowReports()
        {
            SetActiveButton(btnReports);
            reportingView.RefreshData();
            ShowView(reportingView);
        }
    }
}
