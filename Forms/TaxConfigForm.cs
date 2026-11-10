using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHangThueVN.Data;
using QuanLyBanHangThueVN.Models;

namespace QuanLyBanHangThueVN.Forms
{
    public partial class TaxConfigForm : Form
    {
        private readonly AppDbContext _context;
        private DataGridView dgvConfig;

        public TaxConfigForm(AppDbContext context)
        {
            _context = context;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;
            
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            var btnSave = new Button { Text = "Lưu thay đổi", Width = 120, Dock = DockStyle.Left, BackColor = Color.Blue, ForeColor = Color.White };
            btnSave.Click += BtnSave_Click;
            
            pnlTop.Controls.Add(btnSave);
            this.Controls.Add(pnlTop);

            dgvConfig = new DataGridView 
            { 
                Dock = DockStyle.Fill, 
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true
            };
            this.Controls.Add(dgvConfig);

            this.Load += TaxConfigForm_Load;
        }

        private async void TaxConfigForm_Load(object? sender, EventArgs e)
        {
            await _context.TaxConfigurations.LoadAsync();
            dgvConfig.DataSource = _context.TaxConfigurations.Local.ToBindingList();
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                await _context.SaveChangesAsync();
                MessageBox.Show("Đã lưu cấu hình thuế!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
