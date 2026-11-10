using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHangThueVN.Data;
using QuanLyBanHangThueVN.Models;

namespace QuanLyBanHangThueVN.Services
{
    public class TaxResult
    {
        public decimal VatRate { get; set; }
        public decimal VatAmount { get; set; }
        public decimal PersonalIncomeTaxRate { get; set; } // TNCN (nếu là hộ KD)
        public decimal PersonalIncomeTaxAmount { get; set; }
        public bool IsVatReduced { get; set; }
        public string AppliedVatRule { get; set; } = string.Empty;
        public bool IsRevenueThresholdExceeded { get; set; }
    }

    public class TaxService
    {
        private readonly AppDbContext _context;

        public TaxService(AppDbContext context)
        {
            _context = context;
        }

        // Tính thuế cho 1 sản phẩm bán ra
        public async Task<TaxResult> CalculateTaxAsync(decimal revenue, string businessType, TaxGroup taxGroup, DateTime transactionDate)
        {
            var result = new TaxResult();

            // 1. Xác định thuế GTGT (VAT)
            // Lấy mức VAT mặc định từ nhóm thuế
            decimal appliedVatRate = taxGroup.DefaultVatRate;
            result.IsVatReduced = false;
            result.AppliedVatRule = "Mức chuẩn";

            // Kiểm tra xem sản phẩm có được giảm VAT 8% không (nếu mức chuẩn là 10%)
            if (taxGroup.DefaultVatRate == 0.10m && taxGroup.IsVatReductionEligible)
            {
                // Tìm cấu hình giảm thuế VAT (nếu có hiệu lực tại ngày giao dịch)
                var reductionConfig = await _context.TaxConfigurations
                    .Where(c => c.TaxCode == "VAT_REDUCTION_8" && c.IsActive && 
                                c.EffectiveFrom <= transactionDate && 
                                (c.EffectiveTo == null || c.EffectiveTo >= transactionDate))
                    .FirstOrDefaultAsync();

                if (reductionConfig != null)
                {
                    appliedVatRate = reductionConfig.Rate; // Thường là 0.08
                    result.IsVatReduced = true;
                    result.AppliedVatRule = "Giảm thuế theo " + reductionConfig.LegalDocument;
                }
            }

            result.VatRate = appliedVatRate;
            result.VatAmount = revenue * appliedVatRate;

            // 2. Nếu là Hộ Kinh Doanh, tính thêm thuế trực tiếp (VD: GTGT tỷ lệ, TNCN)
            if (businessType == "Personal")
            {
                // Lấy cấu hình thuế TNCN (Hộ KD phân phối hàng hóa thường là 0.5%)
                var tncnConfig = await _context.TaxConfigurations
                    .Where(c => c.TaxCode == "TNCN_HKD" && c.IsActive &&
                                c.EffectiveFrom <= transactionDate &&
                                (c.EffectiveTo == null || c.EffectiveTo >= transactionDate))
                    .FirstOrDefaultAsync();

                if (tncnConfig != null)
                {
                    result.PersonalIncomeTaxRate = tncnConfig.Rate;
                    result.PersonalIncomeTaxAmount = revenue * tncnConfig.Rate;
                }
            }

            return result;
        }

        // Kiểm tra cảnh báo doanh thu năm
        public async Task<(decimal currentRevenue, decimal threshold, bool isExceeded)> CheckRevenueThresholdAsync(int year, string businessType)
        {
            if (businessType != "Personal") return (0, 0, false);

            var config = await _context.TaxConfigurations
                .Where(c => c.TaxCode == "REVENUE_THRESHOLD" && c.IsActive)
                .FirstOrDefaultAsync();

            decimal threshold = config?.RevenueThreshold ?? 100000000m; // Mặc định 100 triệu nếu không có cấu hình

            // Tính tổng doanh thu năm nay
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31, 23, 59, 59);

            var invoices = await _context.Invoices
                .Where(i => i.CreatedDate >= startDate && i.CreatedDate <= endDate)
                .Select(i => i.TotalAmount)
                .ToListAsync();

            decimal currentRevenue = invoices.Sum(); // Tùy thuộc định nghĩa doanh thu tính thuế (có thể là SubTotal)

            return (currentRevenue, threshold, currentRevenue >= threshold);
        }
    }
}
