using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSphere.Models;

public class Book
{
        [Key]
        public Guid Id {get; set;}

        [Required]
        [StringLength(100)]
        public string Title {get; set;}
        
        [Required]
        [StringLength(100)]
        public string Author {get; set;}
        
        [Required]
        [StringLength(50)]
        public string Genre {get; set;}
        
        [Required]
        [StringLength(30)]
        public string ISBN {get; set;}
        
        [Required]
        [StringLength(2000)]
        public string Description {get; set;}
        
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, 10000)]
        public decimal Price {get; set;}
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime PublicationDate {get; set;}
        
        [Required]
        [StringLength(50)]
        public string Publisher {get; set;}
        
        [Required]
        [StringLength(30)]
        public string Language {get; set;}
        
        [Required]
        [StringLength(30)]
        public string Format {get; set;} //Paper, Hardcover etc
        
        public int StockQuantity {get; set;}
        
        public bool PhysicalLibraryAccess {get; set;}
        
        [DataType(DataType.DateTime)]
        public DateTime ListedDate {get; set;} = DateTime.UtcNow;
        
        public bool IsOnSale {get; set;}
        
        [Range(0, 100)]
        public int DiscountPercentage {get; set;}
        
        [DataType(DataType.DateTime)]
        public DateTime? DiscountStartDate {get; set;}
        
        [DataType(DataType.DateTime)]
        public DateTime? DiscountEndDate {get; set;}
        
        public int SoldCount {get; set;}
        
        public bool IsAwardWinner {get; set;}
        
        public bool IsBestSeller {get; set;}
        
        public bool IsComingSoon {get; set;}
        
        [StringLength(500)]
        public string ImageUrl {get; set;}
}
