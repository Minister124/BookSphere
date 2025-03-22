using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSphere.Models;

public class OrderItem
{
        [Key]
        public Guid Id {get; set;}

        [Required]
        public Guid BookId {get; set;}
        
        [Required]
        public Guid OrderId {get; set;}

        [Required]
        public int Quantity {get; set;}

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice {get; set;}
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercentage {get; set;}

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal {get; set;}

        [ForeignKey("OrderId")]
        public virtual Order Order {get; set;}
        
        [ForeignKey("BookId")]
        public virtual Book Book {get; set;}
}