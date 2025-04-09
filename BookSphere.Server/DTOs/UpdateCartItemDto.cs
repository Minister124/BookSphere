using System;
using System.ComponentModel.DataAnnotations;

namespace BookSphere.DTOs;

public class UpdateCartItemDto
{
        [Required]
        public Guid CartItemId { get; set; }
        
        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }
}
