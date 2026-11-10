using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuanLyBanHangThueVN.Models;
using QuanLyBanHangThueVN.Services;

namespace QuanLyBanHangThueVN.Forms
{
    public class AddProductForm : Form
    {
        private readonly InventoryService _inventoryService;

        private TextBox txtProductCode;
        private TextBox txtProductName;
        private ComboBox cboCategory;
        private TextBox txtUnit;
        private NumericUpDown numPurchasePrice;
        private NumericUpDown numSalePrice;
        private ComboBox cboTaxGroup;
        private Button btnSave;
        private Button btnCancel;

        public bool IsSuccess { get; private set; }

        public AddProductForm(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
            InitializeComponent();
            LoadDataAsync();
        }

        private void InitializeComponent()
        {
            this.Text = "Thêm Sản Phẩm Mới";
            this.Size = new Size(400, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 8, Padding = new Padding(20) };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));

            layout.Controls.Add(new Label { Text = "Mã sản phẩm:", Dock = DockStyle.Fill }, 0, 0);
            txtProductCode = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtProductCode, 1, 0);

            layout.Controls.Add(new Label { Text = "Tên sản phẩm:", Dock = DockStyle.Fill }, 0, 1);
            txtProductName = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtProductName, 1, 1);

            layout.Controls.Add(new Label { Text = "Danh mục:", Dock = DockStyle.Fill }, 0, 2);
            cboCategory = new ComboBox { Dock = DockStyle.Fill, DisplayMember = "CategoryName", ValueMember = "Id", DropDownStyle = ComboBoxStyle.DropDownList };
            layout.Controls.Add(cboCategory, 1, 2);

            layout.Controls.Add(new Label { Text = "Đơn vị tính:", Dock = DockStyle.Fill }, 0, 3);
            txtUnit = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtUnit, 1, 3);

            layout.Controls.Add(new Label { Text = "Giá nhập:", Dock = DockStyle.Fill }, 0, 4);
            numPurchasePrice = new NumericUpDown { Dock = DockStyle.Fill, Maximum = 1000000000, DecimalPlaces = 0 };
            layout.Controls.Add(numPurchasePrice, 1, 4);

            layout.Controls.Add(new Label { Text = "Giá bán:", Dock = DockStyle.Fill }, 0, 5);
            numSalePrice = new NumericUpDown { Dock = DockStyle.Fill, Maximum = 1000000000, DecimalPlaces = 0 };
            layout.Controls.Add(numSalePrice, 1, 5);

            layout.Controls.Add(new Label { Text = "Nhóm thuế:", Dock = DockStyle.Fill }, 0, 6);
            cboTaxGroup = new ComboBox { Dock = DockStyle.Fill, DisplayMember = "Name", ValueMember = "Id", DropDownStyle = ComboBoxStyle.DropDownList };
            layout.Controls.Add(cboTaxGroup, 1, 6);

            var pnlButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnCancel = new Button { Text = "Hủy", Width = 80 };
            btnCancel.Click += (s, e) => this.Close();
            btnSave = new Button { Text = "Lưu", Width = 80, BackColor = Color.Green, ForeColor = Color.White };
            btnSave.Click += BtnSave_Click;
            
            pnlButtons.Controls.Add(btnCancel);
            pnlButtons.Controls.Add(btnSave);

            layout.Controls.Add(pnlButtons, 1, 7);

            this.Controls.Add(layout);
        }

        private async void LoadDataAsync()
        {
            var categories = await _inventoryService.GetCategoriesAsync();
            cboCategory.DataSource = categories;

            var taxGroups = await _inventoryService.GetTaxGroupsAsync();
            cboTaxGroup.DataSource = taxGroups;
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductCode.Text) || string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Vui lòng nhập mã và tên sản phẩm!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var product = new Product
            {
                ProductCode = txtProductCode.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                CategoryId = (int)cboCategory.SelectedValue,
                Unit = txtUnit.Text.Trim(),
                PurchasePrice = numPurchasePrice.Value,
                SalePrice = numSalePrice.Value,
                TaxGroupId = (int)cboTaxGroup.SelectedValue,
                StockQuantity = 0, // Nhập kho sau
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            try
            {
                await _inventoryService.AddProductAsync(product);
                IsSuccess = true;
                MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
