using System;
using System.ComponentModel.DataAnnotations;

namespace BookSphere.DTOs;

public class SetDiscountDto
{
        [Required]
        public Guid BookId { get; set; }
        
        [Required]
        [Range(1, 100)]
        public int DiscountPercentage { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public bool IsOnSale { get; set; }
}
