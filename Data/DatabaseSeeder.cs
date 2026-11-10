using System;
using System.Linq;
using QuanLyBanHangThueVN.Models;

namespace QuanLyBanHangThueVN.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated(); // Đảm bảo db được tạo

            if (!context.TaxGroups.Any())
            {
                var grp1 = new TaxGroup { Name = "Hàng hóa VAT 10%", DefaultVatRate = 0.10m, IsVatReductionEligible = false, Description = "Nhóm không được giảm thuế" };
                var grp2 = new TaxGroup { Name = "Hàng hóa VAT 10% (Được giảm 8%)", DefaultVatRate = 0.10m, IsVatReductionEligible = true, Description = "Thuộc diện giảm VAT theo NQ 204" };
                var grp3 = new TaxGroup { Name = "Hàng hóa VAT 5%", DefaultVatRate = 0.05m, IsVatReductionEligible = false, Description = "Các mặt hàng thiết yếu" };
                var grp4 = new TaxGroup { Name = "Hàng hóa không chịu VAT", DefaultVatRate = 0m, IsVatReductionEligible = false, Description = "Không chịu thuế GTGT" };
                
                context.TaxGroups.AddRange(grp1, grp2, grp3, grp4);
                context.SaveChanges();
            }

            if (!context.TaxConfigurations.Any())
            {
                var conf1 = new TaxConfiguration 
                { 
                    TaxCode = "VAT_REDUCTION_8", 
                    TaxName = "Giảm thuế GTGT 8%", 
                    BusinessType = "All", 
                    Rate = 0.08m, 
                    EffectiveFrom = new DateTime(2025, 7, 1), 
                    EffectiveTo = new DateTime(2026, 12, 31),
                    LegalDocument = "Nghị quyết 204/2025/QH15",
                    Description = "Giảm thuế GTGT từ 10% xuống 8%" 
                };

                var conf2 = new TaxConfiguration 
                { 
                    TaxCode = "TNCN_HKD", 
                    TaxName = "Thuế TNCN Hộ KD (Phân phối)", 
                    BusinessType = "Personal", 
                    BusinessSector = "Phân phối, cung cấp hàng hóa",
                    Rate = 0.005m, // 0.5%
                    EffectiveFrom = new DateTime(2020, 1, 1),
                    LegalDocument = "Thông tư 40/2021 & TT 18/2026",
                    Description = "Thuế TNCN tính trên doanh thu" 
                };

                var conf3 = new TaxConfiguration
                {
                    TaxCode = "REVENUE_THRESHOLD",
                    TaxName = "Ngưỡng doanh thu Hộ KD",
                    BusinessType = "Personal",
                    RevenueThreshold = 1000000000m, // 1 tỷ theo bối cảnh mới hoặc 100tr cũ
                    EffectiveFrom = new DateTime(2026, 1, 1),
                    LegalDocument = "Thông tư 18/2026",
                    Description = "Ngưỡng không phải nộp thuế"
                };

                context.TaxConfigurations.AddRange(conf1, conf2, conf3);
                context.SaveChanges();
            }

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { CategoryName = "Xe đạp điện" },
                    new Category { CategoryName = "Xe điện 3 bánh" },
                    new Category { CategoryName = "Phụ tùng xe điện" }
                );
                context.SaveChanges();
            }

            if (!context.Products.Any())
            {
                var catEbike = context.Categories.First(c => c.CategoryName == "Xe đạp điện").Id;
                var catEbike3w = context.Categories.First(c => c.CategoryName == "Xe điện 3 bánh").Id;
                var catParts = context.Categories.First(c => c.CategoryName == "Phụ tùng xe điện").Id;
                
                var tax10Reduced = context.TaxGroups.First(t => t.Name.Contains("Được giảm 8%")).Id;
                var tax10 = context.TaxGroups.First(t => t.DefaultVatRate == 0.10m && !t.IsVatReductionEligible).Id;

                var products = new[]
                {
                    new Product { ProductCode = "XDD01", ProductName = "Xe đạp điện Asama", CategoryId = catEbike, Unit = "Chiếc", PurchasePrice = 5000000, SalePrice = 6500000, StockQuantity = 10, TaxGroupId = tax10Reduced },
                    new Product { ProductCode = "XDD02", ProductName = "Xe đạp điện Yadea", CategoryId = catEbike, Unit = "Chiếc", PurchasePrice = 9000000, SalePrice = 11500000, StockQuantity = 5, TaxGroupId = tax10Reduced },
                    new Product { ProductCode = "XD301", ProductName = "Xe điện 3 bánh Super", CategoryId = catEbike3w, Unit = "Chiếc", PurchasePrice = 12000000, SalePrice = 15000000, StockQuantity = 3, TaxGroupId = tax10Reduced },
                    new Product { ProductCode = "PT001", ProductName = "Ắc quy 48V-12Ah", CategoryId = catParts, Unit = "Bình", PurchasePrice = 300000, SalePrice = 450000, StockQuantity = 20, TaxGroupId = tax10 },
                    new Product { ProductCode = "PT002", ProductName = "Lốp xe 14x2.5", CategoryId = catParts, Unit = "Cái", PurchasePrice = 80000, SalePrice = 150000, StockQuantity = 50, TaxGroupId = tax10 }
                };

                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }
    }
}
