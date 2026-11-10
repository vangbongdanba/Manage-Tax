using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHangThueVN.Models
{
    public class TaxConfiguration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TaxCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string TaxName { get; set; } = string.Empty;

        // "Personal" hoặc "Business"
        [StringLength(50)]
        public string BusinessType { get; set; } = string.Empty;

        // Lĩnh vực kinh doanh, ví dụ "Phân phối, cung cấp hàng hóa"
        [StringLength(200)]
        public string BusinessSector { get; set; } = string.Empty;

        // Thuế suất (VD: 0.01 cho 1%, 0.08 cho 8%)
        [Column(TypeName = "decimal(18,4)")]
        public decimal Rate { get; set; }

        // Ngưỡng doanh thu chịu thuế (nếu có, ví dụ 1 tỷ)
        [Column(TypeName = "decimal(18,2)")]
        public decimal RevenueThreshold { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        [StringLength(200)]
        public string LegalDocument { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
