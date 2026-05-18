using System;
using System.Drawing;
using System.Windows.Forms;

namespace InventoryApp.Views
{
    public class DashboardView : UserControl
    {
        private Label lblWelcome;
        private FlowLayoutPanel cardContainer;
        
        public event EventHandler? ProductManagementClicked;
        public event EventHandler? StockControlClicked;
        public event EventHandler? ReportingClicked;

        public DashboardView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.lblWelcome = new Label();

            this.BackColor = Color.White;
            this.Padding = new Padding(30);

            // lblWelcome
            this.lblWelcome.Text = "Inventory Management Dashboard";
            this.lblWelcome.Font = new Font("Segoe UI", 26, FontStyle.Bold);
            this.lblWelcome.ForeColor = Color.FromArgb(44, 62, 80);
            this.lblWelcome.Dock = DockStyle.Top;
            this.lblWelcome.Height = 150;
            this.lblWelcome.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(this.lblWelcome);

            // Central container for strict centering
            TableLayoutPanel centerLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
                BackColor = Color.White
            };
            this.Controls.Add(centerLayout);

            FlowLayoutPanel btnRow = new FlowLayoutPanel {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Anchor = AnchorStyles.None, // Vital for centering
                BackColor = Color.Transparent
            };
            centerLayout.Controls.Add(btnRow, 0, 0);

            // Create Buttons (Simple Titles)
            AddDashboardButton(btnRow, "Product Management", Color.FromArgb(46, 204, 113), (s, e) => ProductManagementClicked?.Invoke(this, EventArgs.Empty));
            AddDashboardButton(btnRow, "Stock Control", Color.FromArgb(52, 152, 219), (s, e) => StockControlClicked?.Invoke(this, EventArgs.Empty));
            AddDashboardButton(btnRow, "Reports", Color.FromArgb(155, 89, 182), (s, e) => ReportingClicked?.Invoke(this, EventArgs.Empty));
        }

        private void AddDashboardButton(FlowLayoutPanel container, string text, Color color, EventHandler onClick)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(250, 100),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Margin = new Padding(15, 0, 15, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += onClick;
            container.Controls.Add(btn);
        }
    }
}
