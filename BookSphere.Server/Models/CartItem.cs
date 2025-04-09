using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSphere.Models;

public class CartItem
{
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid CartId { get; set; }
        
        [Required]
        public Guid BookId { get; set; }
        
        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; }
        
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
}
