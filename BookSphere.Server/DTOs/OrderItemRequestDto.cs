using System;
using System.ComponentModel.DataAnnotations;

namespace BookSphere.DTOs;

public class OrderItemRequestDto
{
        [Required]
        public Guid BookId { get; set; }
        
        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }
}
