using System;
using BookSphere.DTOs;

namespace BookSphere.IServices;

public interface IWhiteListService
{
          //Get user's whitelist
          Task<WhiteListDto> GetWhiteLisTAsync(Guid userId);

          //Add book to whitelist
          Task<WhiteListDto> AddToWhiteListAsync(Guid userId, AddToWhiteListDto addToWhiteListDto);

          //Remove book from whitelist
          Task<WhiteListDto> RemoveFromWhiteListAsync(Guid userId, Guid bookId);

          //check if book is in whitelist
          Task<bool> IsInWhiteListAsync(Guid userId, Guid bookId);

          //Move item from whitelist to cart
          Task<CartDto> MoveTOCartAsync(Guid userId, Guid bookId, int quantity);
}
