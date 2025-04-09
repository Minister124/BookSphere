using System;
using System.ComponentModel.DataAnnotations;

namespace BookSphere.DTOs;

public class CreateReviewDto
{
        [Required]
        public Guid BookId { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [StringLength(1000)]
        public string Comment { get; set; }
}
