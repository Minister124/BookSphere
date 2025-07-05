using AutoMapper;
using BookSphere.Data;
using BookSphere.DTOs;
using BookSphere.IServices;
using BookSphere.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookSphere.Services
{
    public class OrderService : IOrderService
    {
        private readonly BookSphereDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICartService _cartService;
        private readonly IBookServices _bookService;
        private readonly IUserService _userService;
        private readonly IAnnouncementService _announcementService; // Will be used later

        public OrderService(
            BookSphereDbContext context,
            IMapper mapper,
            ICartService cartService,
            IBookServices bookService,
            IUserService userService,
            IAnnouncementService announcementService)
        {
            _context = context;
            _mapper = mapper;
            _cartService = cartService;
            _bookService = bookService;
            _userService = userService;
            _announcementService = announcementService;
        }

        public Task<OrderDto> ApplyDiscountsAsync(Guid orderId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDto> CancleOrderAsync(Guid userId, Guid orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto createOrderDto)
        {
            // 1. Validate user
            if (!await _userService.IfUserExist(userId))
            {
                throw new KeyNotFoundException("User not found.");
            }

            // 2. Get and validate cart
            var cartDto = await _cartService.GetCartAsync(userId);
            if (cartDto == null || !cartDto.Items.Any())
            {
                throw new InvalidOperationException("Cart is empty or not found.");
            }
            await _cartService.ValidateCartAsync(userId); // Throws if invalid

            // 3. Create Order entity
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending", // Initial status
                ClaimCode = GenerateOrderClaimCode(), // Helper method to generate a unique claim code
                TotalAmount = cartDto.SubTotal, // Original total before discounts
                DiscountAmount = cartDto.DiscountAmount, // Total discount amount from cart
                FinalAmount = cartDto.Total, // Final amount after cart discounts
                AppliedBulkDiscount = cartDto.QualifiesForBulkDiscount,
                AppliedLoyaltyDiscount = cartDto.HasLoyaltyDiscount,
                OrderItems = new List<OrderItem>()
            };

            // 4. Create OrderItems and update book stock
            foreach (var cartItem in cartDto.Items)
            {
                var book = await _context.Books.FindAsync(cartItem.BookId); // Fetch book for current price/discount
                if (book == null) throw new KeyNotFoundException($"Book with ID {cartItem.BookId} not found during order creation.");

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = book.Price, // Use current book price at time of order
                    DiscountPercentage = book.IsOnSale ? book.DiscountPercentage : 0, // Use current discount at time of order
                };
                orderItem.SubTotal = (orderItem.UnitPrice - (orderItem.UnitPrice * orderItem.DiscountPercentage / 100)) * orderItem.Quantity;
                order.OrderItems.Add(orderItem);

                // Update book stock
                await _bookService.UpdateInventoryAsync(cartItem.BookId, -cartItem.Quantity); // Decrease stock

                // Update sold count
                book.SoldCount += cartItem.Quantity;
            }

            // Recalculate totals based on actual book prices at the time of order, as cart's might be slightly stale
            // This ensures order reflects prices/discounts at exact moment of purchase.
            order.TotalAmount = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
            var totalDiscountFromItems = order.OrderItems.Sum(oi => (oi.UnitPrice * oi.DiscountPercentage / 100) * oi.Quantity);

            order.DiscountAmount = totalDiscountFromItems; // Base discount from item sales
            order.FinalAmount = order.TotalAmount - order.DiscountAmount;

            // Re-apply cart-level bulk/loyalty discounts if they were applicable
            if (order.AppliedBulkDiscount) {
                decimal bulkDiscountValue = order.FinalAmount * 0.05m;
                order.DiscountAmount += bulkDiscountValue;
                order.FinalAmount -= bulkDiscountValue;
            }
            if (order.AppliedLoyaltyDiscount) {
                decimal loyaltyDiscountValue = order.FinalAmount * 0.1m; // Applied after bulk if any
                order.DiscountAmount += loyaltyDiscountValue;
                order.FinalAmount -= loyaltyDiscountValue;
            }


            _context.Orders.Add(order);

            // 5. Clear cart
            await _cartService.ClearCartAsync(userId);

            // 6. Update user's successful order count
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.SuccessfulOrder += 1;
                // Potentially update HasStackableDiscount logic here if needed
                // e.g., if (user.SuccessfulOrder >= 10 && !user.HasStackableDiscount) user.HasStackableDiscount = true;
            }

            // 7. Save changes to DB
            await _context.SaveChangesAsync();

            // 8. Trigger announcement (placeholder for now)
            try
            {
                // Assuming a simple announcement DTO for now.
                // This part will be properly implemented when AnnouncementService is done.
                var firstBookTitle = order.OrderItems.FirstOrDefault()?.Book?.Title ?? "a book";

                // The CreateAnnouncementDto might require adminId. For system generated ones, this might be null or a system user Id.
                // For now, let's assume a simplified CreateAnnouncementAsync that doesn't require adminId or handles it.
                 await _announcementService.CreateAnnouncementAsync(Guid.Empty, new CreateAnnouncementDto {
                     Title = "New Purchase!",
                     Message = $"A new order ({order.Id}) including '{firstBookTitle}' was just placed!",
                     Type = "Purchase",
                     // StartDate and EndDate might be relevant for how long it's active
                     StartDate = DateTime.UtcNow,
                     EndDate = DateTime.UtcNow.AddDays(7) // Show for 7 days
                 });
            }
            catch (NotImplementedException)
            {
                // Silently catch if AnnouncementService or its Create method is not yet implemented
                // In a real scenario, log this.
            }
            catch (Exception ex)
            {
                // Log other exceptions related to announcement creation
                Console.WriteLine($"Error creating announcement for order {order.Id}: {ex.Message}");
            }


            return _mapper.Map<OrderDto>(order);
        }

        public Task<OrderDto> GenerateClaimCodeAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<OrderDto> GetOrderDto(Guid orderId) // Matches interface IOrderService
        {
            var order = await _context.Orders
                                .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.Book)
                                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<List<OrderDto>> GetUserOrderAsync(Guid userId)
        {
            if (!await _userService.IfUserExist(userId))
            {
                throw new KeyNotFoundException("User not found.");
            }

            var orders = await _context.Orders
                                 .Where(o => o.UserId == userId)
                                 .Include(o => o.OrderItems)
                                 .ThenInclude(oi => oi.Book)
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToListAsync();

            return _mapper.Map<List<OrderDto>>(orders);
        }

        public Task<List<OrderDto>> GetPendingOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<OrderDto> ProcessOrderAsync(string staffId, ClaimCodeProcessDto claimCodeProcessDto)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDto> SendOrderConfirmationAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }

        private string GenerateOrderClaimCode()
        {
            // Simple claim code generator, ensure uniqueness in a real app
            return $"BSO-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}
