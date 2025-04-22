using System;
using BookSphere.DTOs;

namespace BookSphere.IServices;

public interface ICartService
{
          //Get user's cart
          Task<CartDto> GetCartAsync(Guid userId);

          //Add item to cart
          Task<CartDto> AddToCartAsync(Guid userId, AddToCartDto addToCartDto);

          //Updte cart item quantity
          Task<CartDto> UpdateCartItemAsync(Guid userId, UpdateCartItemDto updateCartItemDto);

          //Remove item from cart
          Task<CartDto> RemoveFromCartAsync(Guid userId, Guid cartItemId);

          //Clear cart
          Task<bool> ClearCartAsync(Guid userId);

          //Check if cart is valid for check(items in stock, etc)
          Task<bool> ValidateCartAsync(Guid userId);
}
