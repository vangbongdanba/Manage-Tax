using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHangThueVN.Data;
using QuanLyBanHangThueVN.Models;

namespace QuanLyBanHangThueVN.Services
{
    public class SaleItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
    }

    public class SalesService
    {
        private readonly AppDbContext _context;
        private readonly TaxService _taxService;

        public SalesService(AppDbContext context, TaxService taxService)
        {
            _context = context;
            _taxService = taxService;
        }

        public async Task<Invoice> CreateInvoiceAsync(List<SaleItem> items, string businessType)
        {
            if (items == null || !items.Any()) throw new Exception("Giỏ hàng trống!");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invoice = new Invoice
                {
                    InvoiceCode = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    CreatedDate = DateTime.Now
                };

                decimal totalSubTotal = 0;
                decimal totalTax = 0;
                decimal totalDiscount = 0;

                foreach (var item in items)
                {
                    var product = await _context.Products.Include(p => p.TaxGroup).FirstOrDefaultAsync(p => p.Id == item.Product.Id);
                    if (product == null) throw new Exception($"Sản phẩm {item.Product.ProductName} không tồn tại.");
                    
                    if (product.StockQuantity < item.Quantity)
                        throw new Exception($"Sản phẩm {product.ProductName} không đủ số lượng tồn (Tồn: {product.StockQuantity}).");

                    product.StockQuantity -= item.Quantity; // Trừ tồn kho

                    decimal itemRevenue = item.Quantity * item.UnitPrice;
                    
                    // Tính thuế
                    var taxResult = await _taxService.CalculateTaxAsync(itemRevenue, businessType, product.TaxGroup!, invoice.CreatedDate);

                    var invoiceDetail = new InvoiceDetail
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        CostPrice = product.PurchasePrice,
                        TaxRate = taxResult.VatRate,
                        TaxAmount = taxResult.VatAmount,
                        Total = itemRevenue + taxResult.VatAmount - item.Discount
                    };

                    invoice.InvoiceDetails.Add(invoiceDetail);

                    totalSubTotal += itemRevenue;
                    totalTax += taxResult.VatAmount;
                    totalDiscount += item.Discount;
                }

                invoice.SubTotal = totalSubTotal;
                invoice.TaxAmount = totalTax;
                invoice.Discount = totalDiscount;
                invoice.TotalAmount = totalSubTotal + totalTax - totalDiscount;

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return invoice;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
