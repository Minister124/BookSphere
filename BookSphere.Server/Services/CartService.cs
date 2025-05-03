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
                    if(!await _userService.IfUserExist(userId))
                    {
                              throw new KeyNotFoundException("User not found");
                    }

                    //check if book exists and is in stock
                    if(!await _bookService.IsInStockAsync(addToCartDto.BookId, addToCartDto.Quantity))
                    {
                              throw new InvalidOperationException("Book is not in stock with the requested quantity");
                    }

                    //Get Cart
                    var cart = await _context.Carts
                              .Include(c => c.Items)
                              .FirstOrDefaultAsync(u => u.UserId == userId);
                              
                    //Create Cart if it does not exist
                    if(cart == null)
                    {
                              cart = new Cart
                              {
                                        UserId = userId,
                                        LastUpdated = DateTime.UtcNow,
                                        Items = new List<CartItem>()
                              };
                              await _context.Carts.AddAsync(cart);
                    }
                    
                    //check if item already exist in cart
                    var existingItem = cart.Items.FirstOrDefault(c => c.BookId == addToCartDto.BookId);

                    if(existingItem != null)
                    {
                              //update quantity
                              existingItem.Quantity += addToCartDto.Quantity;
                              existingItem.AddedDate = DateTime.UtcNow;
                    }
                    else
                    {
                              //Add new item
                              var newItem = new CartItem
                              {
                                        CartId = cart.Id,
                                        BookId = addToCartDto.BookId,
                                        Quantity = addToCartDto.Quantity,
                                        AddedDate = DateTime.UtcNow
                              };
                              cart.Items.Add(newItem);
                    }
                    //update cart
                    cart.LastUpdated = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    
                    //Get update cart with books
                    return await GetCartAsync(userId);
          }

          public async Task<bool> ClearCartAsync(Guid userId)
          {
                    if(!await _userService.IfUserExist(userId)) throw new KeyNotFoundException("User not found");
                    
                    var cart = await _context.Carts
                              .Include(c => c.Items)
                              .FirstOrDefaultAsync(c => c.UserId == userId);
                    if(cart == null)
                    {
                              throw new KeyNotFoundException("Cart Not Found");
                    }

                    _context.CartItems.RemoveRange(cart.Items); //delete all the items in cart item
                    cart.Items.Clear(); //clear all the cart Items in cart

                    cart.LastUpdated = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    return true;
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

          public async Task<CartDto> RemoveFromCartAsync(Guid userId, Guid cartItemId)
          {
                    if(!await _userService.IfUserExist(userId)) throw new KeyNotFoundException("User not found");

                    var cart = await _context.Carts
                              .Include(c => c.Items)
                              .FirstOrDefaultAsync(c => c.UserId == userId);

                    if(cart == null) throw new KeyNotFoundException("Cart Not Found");

                    var cartItem = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

                    if(cartItem == null) throw new KeyNotFoundException("Cart Item Not Found");

                    cart.Items.Remove(cartItem);
                    _context.CartItems.Remove(cartItem);

                    cart.LastUpdated = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    return await GetCartAsync(userId);
          }

          public async Task<CartDto> UpdateCartItemAsync(Guid userId, UpdateCartItemDto updateCartItemDto)
          {
                    if(!await _userService.IfUserExist(userId)) throw new KeyNotFoundException("User Not Found");

                    var cart = await _context.Carts
                              .Include(c => c.Items)
                              .FirstOrDefaultAsync(u => u.UserId == userId);
                    
                    if(cart == null) throw new KeyNotFoundException("Cart not found");

                    //Find Cart Items
                    var cartItem = cart.Items.FirstOrDefault(i => i.Id == updateCartItemDto.CartItemId);

                    if(cartItem == null)
                    {
                              throw new KeyNotFoundException("Item not found on cart");
                    }

                    // check if book is in stock with the new Quantity
                    if(!await _bookService.IsInStockAsync(cartItem.BookId, updateCartItemDto.Quantity)) throw new InvalidOperationException("Book is not found in stock with this quantity");

                    //update Quantity
                    cartItem.Quantity = updateCartItemDto.Quantity;
                    cartItem.AddedDate = DateTime.UtcNow;

                    //Update Cart
                    cart.LastUpdated = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    //Get updated cart with books
                    return await GetCartAsync(userId);
          }

          public async Task<bool> ValidateCartAsync(Guid userId)
          {
                    if(!await _userService.IfUserExist(userId)) throw new KeyNotFoundException("User Not Found");

                    var cart = await _context.Carts
                              .Include(c => c.Items)
                              .ThenInclude(i => i.Book)
                              .FirstOrDefaultAsync(c => c.UserId == userId);
                    
                    if(cart == null || !cart.Items.Any()) throw new InvalidOperationException("Cart is Empty");

                    foreach(var item in cart.Items)
                    {
                              if(item.Book.StockQuantity < item.Quantity) 
                              {
                                        throw new InvalidOperationException($"Book {item.Book.Title} is not in stock with requested quantity");
                              }
                    }

                    return true;
          }
}
