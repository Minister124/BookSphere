using System;
using BookSphere.DTOs;

namespace BookSphere.IServices;

public interface IOrderService
{
          // Create a new order from cart
          Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto createOrderDto);  

          //Get order by ID
          Task<OrderDto> GetOrderDto(Guid orderId);  

          // Get all order of a users
          Task<List<OrderDto>> GetUserOrderAsync(Guid userId);  

          // Cancle an order
          Task<OrderDto> CancleOrderAsync(Guid userId, Guid orderId);  

          //Process an order with claim code (for staff)
          Task<OrderDto> ProcessOrderAsync(string staffId, ClaimCodeProcessDto claimCodeProcessDto);  

          //Get all pending orders (for staff/admin)
          Task<List<OrderDto>> GetPendingOrdersAsync();  

          //Apply discounts to order
          Task<OrderDto> ApplyDiscountsAsync(Guid orderId, Guid userId);  

          // Generate claim code
          Task<OrderDto> GenerateClaimCodeAsync();  

          //Send order confrimation email
          Task<OrderDto> SendOrderConfirmationAsync(Guid orderId);  
}
