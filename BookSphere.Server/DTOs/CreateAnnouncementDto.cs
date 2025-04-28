using System;
using System.ComponentModel.DataAnnotations;

namespace BookSphere.DTOs;

public class CreateAnnouncementDto
{
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Content { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        public string Type { get; set; }
        
        [StringLength(500)]
        public string ImageUrl { get; set; }
}
