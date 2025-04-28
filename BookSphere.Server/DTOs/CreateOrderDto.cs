using System;

namespace BookSphere.DTOs;

public class CreateOrderDto
{
        public List<OrderItemRequestDto> Items { get; set; }
}
