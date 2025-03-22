using System;
using System.ComponentModel.DataAnnotations;

namespace BookSphere.DTOs;

public class CreateBookDto
{
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Author { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Genre { get; set; }
        
        [Required]
        [StringLength(20)]
        public string ISBN { get; set; }
        
        [StringLength(2000)]
        public string Description { get; set; }
        
        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }
        
        [Required]
        public DateTime PublicationDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Publisher { get; set; }
        
        [Required]
        [StringLength(30)]
        public string Language { get; set; }
        
        [Required]
        [StringLength(30)]
        public string Format { get; set; }
        
        [Required]
        public int StockQuantity { get; set; }
        
        public bool PhysicalLibraryAccess { get; set; }
        
        public bool IsAwardWinner { get; set; }
        
        public bool IsBestseller { get; set; }
        
        public bool IsComingSoon { get; set; }
        
        [StringLength(500)]
        public string ImageUrl { get; set; }
}
