using AutoMapper;
using BookSphere.Data;
using BookSphere.DTOs;
using BookSphere.IServices;
using BookSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace BookSphere.Services;

public class CartService : ICartService
{
          private readonly BookSphereDbContext _context;
          private readonly IUserService _userService;

          private readonly IBookServices _bookService;

          private readonly IMapper _mapper;

          public CartService(BookSphereDbContext context, IUserService userService, IBookServices bookService, IMapper mapper)
          {
                    _context = context;
                    _userService = userService;
                    _bookService = bookService;
                    _mapper = mapper;
          }
          public async Task<CartDto> AddToCartAsync(Guid userId, AddToCartDto addToCartDto)
          {
                    //check if user exist
                    //check if book exists and is in stock
                    //Get Cart
                    //Create Cart if it does not exist
                    //check if item already exist in cart
                    //update quantity else add new item
                    //update cart
                    //Get update cart with books
          }

          public Task<bool> ClearCartAsync(Guid userId)
          {
                    throw new NotImplementedException();
          }

          public async Task<CartDto> GetCartAsync(Guid userId)
          {
                    //check if user exist
                    if(!await _userService.IfUserExist(userId))
                    {
                              throw new KeyNotFoundException("User not found");
                    }
                    //Get cart with items and books
                    var cart = await _context.Carts
                              .Include(c => c.Items)
                              .ThenInclude(c => c.Book)
                              .FirstOrDefaultAsync(u => u.UserId == userId);
                    //throw if it is null
                    if (cart == null)
                    {
                              throw new KeyNotFoundException("Cart not found");
                    }
                    //Map to DTO
                    var CartDto = _mapper.Map<CartDto>(cart);
                    //Calculate total
                    await CalculateCartTotal(CartDto, userId);

                    return CartDto;
          }

          private async Task CalculateCartTotal(CartDto cartDto, Guid userId)
          {
                    decimal subTotal = 0;
                    decimal discountAmount = 0;

                    //Calculate subtotal and discount amount
                    foreach(var item in cartDto.Items)
                    {
                              var originalPrice = item.UnitPrice * item.Quantity;
                              var discountPrice = item.DiscountedPrice * item.Quantity;

                              subTotal += originalPrice;
                              discountAmount += (originalPrice - discountPrice);
                    }

                    cartDto.SubTotal = subTotal;
                    cartDto.DiscountAmount = discountAmount;

                    //check if cart qualifies for bulk discount +5 bookes
                    int totalQuanity = cartDto.Items.Sum(i => i.Quantity);
                    cartDto.QualifiesForBulkDiscount = totalQuanity >= 5;

                    //Check if user has loyalty discount
                    cartDto.HasLoyaltyDiscount = await _userService.HasStackableDiscount(userId);

                    //calcualte total
                    cartDto.Total = subTotal - discountAmount;

                    //Apply bulk discount if application (5%)
                    if(cartDto.QualifiesForBulkDiscount)
                    {
                              decimal bulkDiscount = cartDto.Total * 0.05m;
                              cartDto.DiscountAmount += bulkDiscount;
                              cartDto.Total -= bulkDiscount;
                    }

                    //Apply loyalty discount if applicable (10%)
                    if(cartDto.HasLoyaltyDiscount)
                    {
                              decimal loyaltyDiscountAmount = cartDto.Total * 0.1m;
                              cartDto.DiscountAmount += loyaltyDiscountAmount;
                              cartDto.Total -= loyaltyDiscountAmount;
                    }
          }

          public Task<CartDto> RemoveFromCartAsync(Guid userId, Guid cartItemId)
          {
                    throw new NotImplementedException();
          }

          public Task<CartDto> UpdateCartItemAsync(Guid userId, UpdateCartItemDto updateCartItemDto)
          {
                    throw new NotImplementedException();
          }

          public Task<bool> ValidateCartAsync(Guid userId)
          {
                    throw new NotImplementedException();
          }
}
