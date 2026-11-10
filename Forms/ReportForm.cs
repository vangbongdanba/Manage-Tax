using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHangThueVN.Data;
using QuanLyBanHangThueVN.Services;

namespace QuanLyBanHangThueVN.Forms
{
    public partial class ReportForm : Form
    {
        private readonly AppDbContext _context;
        private DataGridView dgvReport;
        private ComboBox cboMonth;
        private ComboBox cboYear;
        private Button btnView;
        private Label lblTotal;

        public ReportForm(AppDbContext context)
        {
            _context = context;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;
            
            var pnlTop = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            
            pnlTop.Controls.Add(new Label { Text = "Tháng:", AutoSize = true, Margin = new Padding(5, 10, 5, 0) });
            cboMonth = new ComboBox { Width = 60, DropDownStyle = ComboBoxStyle.DropDownList };
            for(int i = 1; i <= 12; i++) cboMonth.Items.Add(i);
            cboMonth.SelectedItem = DateTime.Now.Month;
            pnlTop.Controls.Add(cboMonth);

            pnlTop.Controls.Add(new Label { Text = "Năm:", AutoSize = true, Margin = new Padding(15, 10, 5, 0) });
            cboYear = new ComboBox { Width = 80, DropDownStyle = ComboBoxStyle.DropDownList };
            for(int i = 2020; i <= 2030; i++) cboYear.Items.Add(i);
            cboYear.SelectedItem = DateTime.Now.Year;
            pnlTop.Controls.Add(cboYear);

            btnView = new Button { Text = "Xem báo cáo", Width = 100, BackColor = Color.Blue, ForeColor = Color.White };
            btnView.Click += BtnView_Click;
            pnlTop.Controls.Add(btnView);

            this.Controls.Add(pnlTop);

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 120, Padding = new Padding(10) };
            lblTotal = new Label { Dock = DockStyle.Fill, Font = new Font("Arial", 12, FontStyle.Regular) };
            pnlBottom.Controls.Add(lblTotal);
            
            var lblDisclaimer = new Label { Text = "Các số liệu thuế trong phần mềm phục vụ mục đích quản lý và ước tính. Nghĩa vụ thuế thực tế cần được xác định theo quy định pháp luật và hướng dẫn của cơ quan thuế tại thời điểm kê khai.", Dock = DockStyle.Bottom, Height = 40, Font = new Font("Arial", 9, FontStyle.Italic), ForeColor = Color.Gray };
            pnlBottom.Controls.Add(lblDisclaimer);
            this.Controls.Add(pnlBottom);

            dgvReport = new DataGridView 
            { 
                Dock = DockStyle.Fill, 
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            this.Controls.Add(dgvReport);
        }

        private async void BtnView_Click(object? sender, EventArgs e)
        {
            int month = (int)cboMonth.SelectedItem;
            int year = (int)cboYear.SelectedItem;

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var invoices = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Where(i => i.CreatedDate >= startDate && i.CreatedDate <= endDate)
                .ToListAsync();

            decimal totalRev = invoices.Sum(i => i.TotalAmount);
            decimal totalVat = invoices.Sum(i => i.TaxAmount);
            
            // Lấy thuế TNCN
            decimal tncnRate = 0;
            var tncnConfig = await _context.TaxConfigurations
                    .Where(c => c.TaxCode == "TNCN_HKD" && c.IsActive && c.EffectiveFrom <= startDate)
                    .FirstOrDefaultAsync();
            if (tncnConfig != null) tncnRate = tncnConfig.Rate;

            decimal totalTncn = totalRev * tncnRate;

            lblTotal.Text = $"TỔNG DOANH THU: {totalRev:N0} ₫\n" +
                            $"TỔNG VAT: {totalVat:N0} ₫\n" +
                            $"TỔNG THUẾ TNCN DỰ KIẾN: {totalTncn:N0} ₫\n" +
                            $"TỔNG NGHĨA VỤ THUẾ: {(totalVat + totalTncn):N0} ₫";

            var reportData = invoices.Select(i => new {
                Ngày = i.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                Mã_HĐ = i.InvoiceCode,
                Tiền_Hàng = i.SubTotal,
                VAT = i.TaxAmount,
                Chiết_Khấu = i.Discount,
                Tổng_Cộng = i.TotalAmount
            }).ToList();

            dgvReport.DataSource = reportData;
        }
    }
}
