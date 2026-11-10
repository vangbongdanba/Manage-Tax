using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanHangThueVN.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }

        public int StockQuantity { get; set; }

        public int TaxGroupId { get; set; }
        
        [ForeignKey("TaxGroupId")]
        public virtual TaxGroup? TaxGroup { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
