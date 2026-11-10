using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHangThueVN.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
