using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHangThueVN.Models
{
    public class InvoiceDetail
    {
        [Key]
        public int Id { get; set; }

        public int InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice? Invoice { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxRate { get; set; } // Thuế suất tại thời điểm bán (vd: 0.08)

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; } // Số tiền thuế

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; } // Thành tiền (đã bao gồm thuế)
    }
}
