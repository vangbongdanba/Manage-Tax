using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using QuanLyBanHangThueVN.Services;

namespace QuanLyBanHangThueVN.Forms
{
    public partial class DashboardForm : Form
    {
        private readonly StatisticsService _statsService;
        private readonly TaxService _taxService;

        private Label lblTotalRevenue;
        private Label lblTotalProfit;
        private Label lblTotalTax;
        private Label lblStock;
        private Label lblWarning;
        private CartesianChart chartRevenue;
        private DataGridView dgvTopProducts;

        public DashboardForm(StatisticsService statsService, TaxService taxService)
        {
            _statsService = statsService;
            _taxService = taxService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;
            
            var pnlTop = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 120, Padding = new Padding(10) };
            
            lblTotalRevenue = CreateCard("DOANH THU", pnlTop);
            lblTotalProfit = CreateCard("LỢI NHUẬN TẠM TÍNH", pnlTop);
            lblTotalTax = CreateCard("THUẾ DỰ KIẾN", pnlTop);
            lblStock = CreateCard("TỒN KHO", pnlTop);

            this.Controls.Add(pnlTop);

            lblWarning = new Label 
            { 
                Dock = DockStyle.Top, 
                Height = 40, 
                Font = new Font("Arial", 12, FontStyle.Bold), 
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            this.Controls.Add(lblWarning);

            var pnlMain = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            pnlMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            pnlMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            chartRevenue = new CartesianChart { Dock = DockStyle.Fill, Margin = new Padding(10) };
            pnlMain.Controls.Add(chartRevenue, 0, 0);

            dgvTopProducts = new DataGridView 
            { 
                Dock = DockStyle.Fill, 
                Margin = new Padding(10),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            pnlMain.Controls.Add(dgvTopProducts, 1, 0);

            this.Controls.Add(pnlMain);
            this.Load += DashboardForm_Load;
        }

        private Label CreateCard(string title, FlowLayoutPanel parent)
        {
            var pnl = new Panel { Width = 250, Height = 100, BackColor = Color.LightBlue, Margin = new Padding(10) };
            var lblTitle = new Label { Text = title, Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Arial", 10, FontStyle.Bold), Height = 30 };
            var lblValue = new Label { Text = "0", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Arial", 16, FontStyle.Bold) };
            
            pnl.Controls.Add(lblValue);
            pnl.Controls.Add(lblTitle);
            parent.Controls.Add(pnl);
            
            return lblValue;
        }

        private async void DashboardForm_Load(object? sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var stats = await _statsService.GetDashboardStatsAsync(startDate, endDate);
            
            lblTotalRevenue.Text = stats.TotalRevenue.ToString("N0") + " ₫";
            lblTotalProfit.Text = stats.TotalProfit.ToString("N0") + " ₫";
            lblTotalTax.Text = stats.TotalTaxExpected.ToString("N0") + " ₫";
            lblStock.Text = stats.CurrentStock.ToString("N0") + " SP";

            // Threshold warning
            var (currentRevenue, threshold, isExceeded) = await _taxService.CheckRevenueThresholdAsync(DateTime.Now.Year, "Personal");
            if (isExceeded)
            {
                lblWarning.Text = $"CẢNH BÁO: Doanh thu năm 2026 ({currentRevenue:N0} ₫) đã VƯỢT NGƯỠNG THUẾ ({threshold:N0} ₫).";
                lblWarning.BackColor = Color.Red;
                lblWarning.ForeColor = Color.White;
                lblWarning.Visible = true;
            }
            else if (currentRevenue > threshold * 0.8m)
            {
                lblWarning.Text = $"CHÚ Ý: Doanh thu năm 2026 sắp chạm ngưỡng thuế (Còn {threshold - currentRevenue:N0} ₫).";
                lblWarning.BackColor = Color.Yellow;
                lblWarning.ForeColor = Color.Black;
                lblWarning.Visible = true;
            }

            var topProducts = await _statsService.GetTopSellingProductsAsync(startDate, endDate);
            dgvTopProducts.DataSource = topProducts;

            // Simple Chart
            chartRevenue.Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = new double[] { (double)stats.TotalRevenue },
                    Name = "Doanh thu tháng này"
                }
            };
        }
    }
}
