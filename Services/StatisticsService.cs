using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHangThueVN.Data;
using QuanLyBanHangThueVN.Models;

namespace QuanLyBanHangThueVN.Services
{
    public class DashboardStats
    {
        public int TotalProductsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalTaxExpected { get; set; }
        public int CurrentStock { get; set; }
    }

    public class ProductSalesStat
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Profit { get; set; }
    }

    public class StatisticsService
    {
        private readonly AppDbContext _context;

        public StatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStats> GetDashboardStatsAsync(DateTime startDate, DateTime endDate)
        {
            var invoices = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Where(i => i.CreatedDate >= startDate && i.CreatedDate <= endDate)
                .ToListAsync();

            int totalSold = invoices.SelectMany(i => i.InvoiceDetails).Sum(d => d.Quantity);
            decimal revenue = invoices.Sum(i => i.TotalAmount); // Hoặc SubTotal tùy nghĩa vụ
            decimal tax = invoices.Sum(i => i.TaxAmount);
            
            // Tính giá vốn
            decimal cost = invoices.SelectMany(i => i.InvoiceDetails).Sum(d => d.Quantity * d.CostPrice);
            decimal profit = revenue - tax - cost; // Lợi nhuận gộp sau thuế (hoặc tùy định nghĩa, tạm tính)

            int stock = await _context.Products.Where(p => p.IsActive).SumAsync(p => p.StockQuantity);

            return new DashboardStats
            {
                TotalProductsSold = totalSold,
                TotalRevenue = revenue,
                TotalTaxExpected = tax, // Chỉ gồm VAT, chưa cộng TNCN nếu có, cần mở rộng
                TotalProfit = profit,
                CurrentStock = stock
            };
        }

        public async Task<List<ProductSalesStat>> GetTopSellingProductsAsync(DateTime startDate, DateTime endDate, int top = 5)
        {
            var details = await _context.InvoiceDetails
                .Include(d => d.Invoice)
                .Include(d => d.Product)
                .Where(d => d.Invoice!.CreatedDate >= startDate && d.Invoice.CreatedDate <= endDate)
                .ToListAsync();

            var query = details.GroupBy(d => d.ProductId)
                .Select(g => new ProductSalesStat
                {
                    ProductCode = g.First().Product!.ProductCode,
                    ProductName = g.First().Product!.ProductName,
                    QuantitySold = g.Sum(d => d.Quantity),
                    Revenue = g.Sum(d => d.Total),
                    TaxAmount = g.Sum(d => d.TaxAmount),
                    Profit = g.Sum(d => d.Total - d.TaxAmount - (d.Quantity * d.CostPrice))
                })
                .OrderByDescending(s => s.QuantitySold)
                .Take(top)
                .ToList();

            return query;
        }
    }
}
