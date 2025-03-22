using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSphere.Models;

public class Cart
{
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        public virtual ICollection<CartItem> Items { get; set; }
}