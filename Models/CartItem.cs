using System.ComponentModel.DataAnnotations;

namespace BookSphere.Models;

public class CartItem
{
        [Key]
        public Guid Id {get; set;}

        [Required]
        [Range(1, 100)]
        public int Quantity {get; set;}

        [DataType(DataType.DateTime)]
        public DateTime AddedDate {get; set;} = DateTime.UtcNow;
}
