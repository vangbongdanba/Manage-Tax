using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyBanHangThueVN.Models;
using QuanLyBanHangThueVN.Services;

namespace QuanLyBanHangThueVN.Forms
{
    public partial class POSForm : Form
    {
        private readonly InventoryService _inventoryService;
        private readonly SalesService _salesService;
        
        private ComboBox cboProduct;
        private NumericUpDown numQuantity;
        private Button btnAdd;
        private DataGridView dgvCart;
        private Label lblTotal;
        private Button btnCheckout;

        private List<SaleItem> _cart = new List<SaleItem>();
        private List<Product> _products = new List<Product>();

        public POSForm(InventoryService inventoryService, SalesService salesService)
        {
            _inventoryService = inventoryService;
            _salesService = salesService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;
            
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 80, Padding = new Padding(10) };
            
            pnlTop.Controls.Add(new Label { Text = "Sản phẩm:", Location = new Point(10, 20), AutoSize = true });
            cboProduct = new ComboBox { Location = new Point(80, 15), Width = 250, DisplayMember = "ProductName", ValueMember = "Id" };
            pnlTop.Controls.Add(cboProduct);

            pnlTop.Controls.Add(new Label { Text = "Số lượng:", Location = new Point(350, 20), AutoSize = true });
            numQuantity = new NumericUpDown { Location = new Point(410, 15), Width = 100, Minimum = 1, Maximum = 1000 };
            pnlTop.Controls.Add(numQuantity);

            btnAdd = new Button { Text = "Thêm vào giỏ", Location = new Point(530, 15), Width = 120, BackColor = Color.LightBlue };
            btnAdd.Click += BtnAdd_Click;
            pnlTop.Controls.Add(btnAdd);

            this.Controls.Add(pnlTop);

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 100, Padding = new Padding(10) };
            
            lblTotal = new Label { Text = "Tổng tiền: 0 ₫", Font = new Font("Arial", 16, FontStyle.Bold), ForeColor = Color.Red, AutoSize = true, Location = new Point(20, 30) };
            pnlBottom.Controls.Add(lblTotal);

            btnCheckout = new Button { Text = "Thanh toán", Font = new Font("Arial", 14, FontStyle.Bold), Width = 200, Height = 60, Location = new Point(400, 20), BackColor = Color.Green, ForeColor = Color.White };
            btnCheckout.Click += BtnCheckout_Click;
            pnlBottom.Controls.Add(btnCheckout);

            this.Controls.Add(pnlBottom);

            dgvCart = new DataGridView 
            { 
                Dock = DockStyle.Fill, 
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            this.Controls.Add(dgvCart);

            this.Load += POSForm_Load;
        }

        private async void POSForm_Load(object? sender, EventArgs e)
        {
            _products = await _inventoryService.GetAllProductsAsync();
            cboProduct.DataSource = _products;
            RefreshCartGrid();
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (cboProduct.SelectedValue == null) return;
            int productId = (int)cboProduct.SelectedValue;
            var product = _products.FirstOrDefault(p => p.Id == productId);
            
            if (product == null) return;

            int qty = (int)numQuantity.Value;

            var existingItem = _cart.FirstOrDefault(c => c.Product.Id == productId);
            if (existingItem != null)
            {
                if (existingItem.Quantity + qty > product.StockQuantity)
                {
                    MessageBox.Show("Số lượng vượt quá tồn kho!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                existingItem.Quantity += qty;
            }
            else
            {
                if (qty > product.StockQuantity)
                {
                    MessageBox.Show("Số lượng vượt quá tồn kho!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                _cart.Add(new SaleItem
                {
                    Product = product,
                    Quantity = qty,
                    UnitPrice = product.SalePrice,
                    Discount = 0
                });
            }

            RefreshCartGrid();
        }

        private void RefreshCartGrid()
        {
            dgvCart.DataSource = null;
            var displayList = _cart.Select(c => new
            {
                Mã = c.Product.ProductCode,
                Tên = c.Product.ProductName,
                Số_Lượng = c.Quantity,
                Đơn_Giá = c.UnitPrice.ToString("N0") + " ₫",
                Thành_Tiền = (c.Quantity * c.UnitPrice).ToString("N0") + " ₫"
            }).ToList();
            
            dgvCart.DataSource = displayList;

            decimal total = _cart.Sum(c => c.Quantity * c.UnitPrice); // Tạm tính chưa VAT
            // Note: Để tính chính xác cần gọi TaxService lúc này nếu muốn show VAT trước khi thanh toán,
            // nhưng để đơn giản ta tạm show thành tiền hàng ở đây.
            lblTotal.Text = $"Tạm tính (chưa VAT): {total:N0} ₫";
        }

        private async void BtnCheckout_Click(object? sender, EventArgs e)
        {
            if (!_cart.Any())
            {
                MessageBox.Show("Giỏ hàng trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var invoice = await _salesService.CreateInvoiceAsync(_cart, "Personal");
                MessageBox.Show($"Thanh toán thành công!\nMã hóa đơn: {invoice.InvoiceCode}\nTổng thu: {invoice.TotalAmount:N0} ₫\nThuế GTGT: {invoice.TaxAmount:N0} ₫", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                _cart.Clear();
                RefreshCartGrid();
                
                // Refresh list product để update tồn kho
                _products = await _inventoryService.GetAllProductsAsync();
                cboProduct.DataSource = _products;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
