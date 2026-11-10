using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHangThueVN.Models
{
    public class TaxGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Mức VAT chuẩn (VD: 0.10)
        [Column(TypeName = "decimal(18,4)")]
        public decimal DefaultVatRate { get; set; }

        // Có được giảm VAT 8% theo chính sách hiện hành không?
        public bool IsVatReductionEligible { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
