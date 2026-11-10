using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace QuanLyBanHangThueVN.Forms
{
    public partial class MainForm : Form
    {
        private Panel pnlContent;
        private MenuStrip menuStrip;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Hệ thống Quản lý Hàng hóa, Bán hàng và Tính thuế";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            menuStrip = new MenuStrip();
            
            var mnuTongQuan = new ToolStripMenuItem("Tổng quan");
            mnuTongQuan.Click += (s, e) => OpenForm<DashboardForm>();

            var mnuHangHoa = new ToolStripMenuItem("Hàng hóa");
            mnuHangHoa.Click += (s, e) => OpenForm<ProductForm>();

            var mnuNhapHang = new ToolStripMenuItem("Nhập hàng");
            mnuNhapHang.Click += (s, e) => OpenForm<ImportForm>();

            var mnuBanHang = new ToolStripMenuItem("Bán hàng (POS)");
            mnuBanHang.Click += (s, e) => OpenForm<POSForm>();

            var mnuBaoCao = new ToolStripMenuItem("Báo cáo thuế");
            mnuBaoCao.Click += (s, e) => OpenForm<ReportForm>();

            var mnuCauHinh = new ToolStripMenuItem("Cấu hình thuế");
            mnuCauHinh.Click += (s, e) => OpenForm<TaxConfigForm>();

            menuStrip.Items.AddRange(new ToolStripItem[] {
                mnuTongQuan, mnuHangHoa, mnuNhapHang, mnuBanHang, mnuBaoCao, mnuCauHinh
            });

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke
            };
            this.Controls.Add(pnlContent);
            pnlContent.BringToFront();

            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            OpenForm<DashboardForm>();
        }

        private void OpenForm<T>() where T : Form
        {
            var form = Program.ServiceProvider.GetRequiredService<T>();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(form);
            form.Show();
        }
    }
}
