using Microsoft.EntityFrameworkCore;
using QuanLyBanHangThueVN.Models;
using System.IO;
using System.Reflection;

namespace QuanLyBanHangThueVN.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<StockImport> StockImports { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<TaxConfiguration> TaxConfigurations { get; set; }
        public DbSet<TaxGroup> TaxGroups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Sử dụng SQLite, file DB đặt cùng thư mục với exe
                var dbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "QuanLyBanHang.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Thiết lập giá trị mặc định cho SQLite không hỗ trợ decimal nguyên bản
            // Entity Framework Core có thể tự map decimal sang double/TEXT trong SQLite,
            // nhưng thường không cần khai báo quá đặc biệt nếu chỉ dùng CRUD.
        }
    }
}
