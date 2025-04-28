using System;

namespace BookSphere.DTOs;

public class CartDto
{
        public Guid Id { get; set; }
        public List<CartItemDto> Items { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
        public bool QualifiesForBulkDiscount { get; set; }
        public bool HasLoyaltyDiscount { get; set; }
}
