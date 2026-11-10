using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHangThueVN.Data;
using QuanLyBanHangThueVN.Models;

namespace QuanLyBanHangThueVN.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.TaxGroup)
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            if (await _context.Products.AnyAsync(p => p.ProductCode == product.ProductCode))
            {
                throw new Exception("Mã sản phẩm đã tồn tại!");
            }
            if (product.PurchasePrice < 0 || product.SalePrice < 0 || product.StockQuantity < 0)
            {
                throw new Exception("Giá trị không hợp lệ (không được âm)!");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new Exception("Không tìm thấy sản phẩm");

            // Chuyển trạng thái IsActive = false thay vì xóa cứng để không mất lịch sử hóa đơn
            product.IsActive = false;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task ImportStockAsync(int productId, int quantity, decimal importPrice, string note)
        {
            if (quantity <= 0) throw new Exception("Số lượng nhập phải > 0");
            
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new Exception("Không tìm thấy sản phẩm");

            // Tạo transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Thêm lịch sử nhập
                var import = new StockImport
                {
                    ProductId = productId,
                    Quantity = quantity,
                    ImportPrice = importPrice,
                    Note = note,
                    ImportDate = DateTime.Now
                };
                _context.StockImports.Add(import);

                // Cập nhật tồn kho
                product.StockQuantity += quantity;
                product.PurchasePrice = importPrice; // Có thể giữ giá nhập mới nhất hoặc trung bình trọng số
                _context.Products.Update(product);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        
        public async Task<List<TaxGroup>> GetTaxGroupsAsync()
        {
            return await _context.TaxGroups.ToListAsync();
        }
    }
}
