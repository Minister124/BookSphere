using System;

namespace BookSphere.DTOs;

public class BookDto
{
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string Format { get; set; }
        public int StockQuantity { get; set; }
        public bool PhysicalLibraryAccess { get; set; }
        public DateTime ListedDate { get; set; }
        public bool IsOnSale { get; set; }
        public int DiscountPercentage { get; set; }
        public decimal DiscountedPrice => Price - (Price * DiscountPercentage / 100);
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public int SoldCount { get; set; }
        public bool IsAwardWinner { get; set; }
        public bool IsBestseller { get; set; }
        public bool IsComingSoon { get; set; }
        public string ImageUrl { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
}
