using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyBanHangThueVN.Models;
using QuanLyBanHangThueVN.Services;

namespace QuanLyBanHangThueVN.Forms
{
    public partial class ProductForm : Form
    {
        private readonly InventoryService _inventoryService;
        private DataGridView dgvProducts;
        
        public ProductForm(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;
            
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            var btnAdd = new Button { Text = "Thêm sản phẩm", Width = 150, Dock = DockStyle.Left };
            var btnRefresh = new Button { Text = "Làm mới", Width = 100, Dock = DockStyle.Left };
            var btnDelete = new Button { Text = "Xóa", Width = 100, Dock = DockStyle.Left, BackColor = Color.Red, ForeColor = Color.White };
            
            btnRefresh.Click += async (s, e) => await LoadDataAsync();
            btnDelete.Click += BtnDelete_Click;
            btnAdd.Click += BtnAdd_Click;
            
            pnlTop.Controls.Add(btnDelete);
            pnlTop.Controls.Add(btnRefresh);
            pnlTop.Controls.Add(btnAdd);
            this.Controls.Add(pnlTop);

            dgvProducts = new DataGridView 
            { 
                Dock = DockStyle.Fill, 
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            this.Controls.Add(dgvProducts);

            this.Load += ProductForm_Load;
        }

        private async void ProductForm_Load(object? sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var products = await _inventoryService.GetAllProductsAsync();
            var displayList = products.Select(p => new
            {
                p.Id,
                Mã = p.ProductCode,
                Tên = p.ProductName,
                Danh_Mục = p.Category?.CategoryName,
                Giá_Nhập = p.PurchasePrice,
                Giá_Bán = p.SalePrice,
                Tồn_Kho = p.StockQuantity,
                Nhóm_Thuế = p.TaxGroup?.Name
            }).ToList();
            
            dgvProducts.DataSource = displayList;
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    int productId = (int)dgvProducts.SelectedRows[0].Cells["Id"].Value;
                    await _inventoryService.DeleteProductAsync(productId);
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            var addForm = new AddProductForm(_inventoryService);
            addForm.ShowDialog(this);
            if (addForm.IsSuccess)
            {
                await LoadDataAsync();
            }
        }
    }
}
