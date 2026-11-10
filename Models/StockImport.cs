using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHangThueVN.Models
{
    public class StockImport
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportPrice { get; set; }

        public DateTime ImportDate { get; set; } = DateTime.Now;
        
        [StringLength(500)]
        public string Note { get; set; } = string.Empty;
    }
}
