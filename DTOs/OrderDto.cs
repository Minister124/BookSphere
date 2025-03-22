using System;

namespace BookSphere.DTOs;

public class OrderDto
{
        public Guid Id { get; set; }
        public string ClaimCode { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public bool AppliedBulkDiscount { get; set; }
        public bool AppliedLoyaltyDiscount { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public List<OrderItemDto> Items { get; set; }
}
