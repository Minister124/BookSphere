using System;

namespace BookSphere.DTOs;

public class CartItemDto
{
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public BookDto Book { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime AddedDate { get; set; }
}
