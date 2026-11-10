using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuanLyBanHangThueVN.Services;

namespace QuanLyBanHangThueVN.Forms
{
    public partial class ImportForm : Form
    {
        private readonly InventoryService _inventoryService;
        private ComboBox cboProduct;
        private NumericUpDown numQuantity;
        private NumericUpDown numPrice;
        private TextBox txtNote;

        public ImportForm(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;
            var layout = new TableLayoutPanel { Dock = DockStyle.Top, Height = 300, ColumnCount = 2, RowCount = 5, Padding = new Padding(20) };
            
            layout.Controls.Add(new Label { Text = "Sản phẩm:" }, 0, 0);
            cboProduct = new ComboBox { Width = 300, DisplayMember = "ProductName", ValueMember = "Id", DropDownStyle = ComboBoxStyle.DropDownList };
            layout.Controls.Add(cboProduct, 1, 0);

            layout.Controls.Add(new Label { Text = "Số lượng:" }, 0, 1);
            numQuantity = new NumericUpDown { Width = 300, Maximum = 1000000, Minimum = 1 };
            layout.Controls.Add(numQuantity, 1, 1);

            layout.Controls.Add(new Label { Text = "Giá nhập mới:" }, 0, 2);
            numPrice = new NumericUpDown { Width = 300, Maximum = 1000000000, DecimalPlaces = 0 };
            layout.Controls.Add(numPrice, 1, 2);

            layout.Controls.Add(new Label { Text = "Ghi chú:" }, 0, 3);
            txtNote = new TextBox { Width = 300, Multiline = true, Height = 60 };
            layout.Controls.Add(txtNote, 1, 3);

            var btnImport = new Button { Text = "Xác nhận nhập kho", Width = 150, Height = 40, BackColor = Color.Green, ForeColor = Color.White };
            btnImport.Click += BtnImport_Click;
            layout.Controls.Add(btnImport, 1, 4);

            this.Controls.Add(layout);
            this.Load += ImportForm_Load;
        }

        private async void ImportForm_Load(object? sender, EventArgs e)
        {
            var products = await _inventoryService.GetAllProductsAsync();
            cboProduct.DataSource = products;
        }

        private async void BtnImport_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cboProduct.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm để nhập kho!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int productId = (int)cboProduct.SelectedValue;
                int quantity = (int)numQuantity.Value;
                decimal price = numPrice.Value;
                string note = txtNote.Text;

                await _inventoryService.ImportStockAsync(productId, quantity, price, note);
                MessageBox.Show("Nhập kho thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                numQuantity.Value = 1;
                txtNote.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
