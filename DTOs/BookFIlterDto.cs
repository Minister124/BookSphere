using System;

namespace BookSphere.DTOs;

public class BookFIlterDto
{
        public string SearchTerm { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public bool? InStock { get; set; }
        public bool? PhysicalLibraryAccess { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinRating { get; set; }
        public string Language { get; set; }
        public string Format { get; set; }
        public string Publisher { get; set; }
        public bool? IsOnSale { get; set; }
        public bool? IsAwardWinner { get; set; }
        public bool? IsBestseller { get; set; }
        public bool? IsNewRelease { get; set; }
        public bool? IsNewArrival { get; set; }
        public bool? IsComingSoon { get; set; }
        public string SortBy { get; set; }
        public bool SortDescending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
}
