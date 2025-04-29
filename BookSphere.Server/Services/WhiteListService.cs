using System;
using AutoMapper;
using BookSphere.Data;
using BookSphere.DTOs;
using BookSphere.IServices;
using BookSphere.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookSphere.Services;

public class WhiteListService : IWhiteListService
{

          private readonly BookSphereDbContext _context;

          private readonly IMapper _mapper;

          private readonly IUserService _userService;

          private readonly ICartService _cartService;

          public WhiteListService(BookSphereDbContext context, IMapper mapper, IUserService userService, ICartService cartService)
          {
                    _context = context;
                    _mapper = mapper;
                    _userService = userService;
                    _cartService = cartService;
          }
          public async Task<WhiteListDto> AddToWhiteListAsync(Guid userId, AddToWhiteListDto addToWhiteListDto)
          {
                    //Check if user exist
                    if(!await _userService.IfUserExist(userId)) throw new KeyNotFoundException("User not found");

                    // check if book exist
                    var book = await _context.Books.FindAsync(addToWhiteListDto.BookId);
                    if (book == null) throw new KeyNotFoundException("Book not found");

                    // check if whitelist exist
                    var whiteList = await _context.WhiteLists
                              .Include(w => w.WhiteListItems)
                              .FirstOrDefaultAsync(w => w.UserId == userId);

                    if(whiteList == null)
                    {
                              // create whiteList if it does not exist.
                              whiteList = new Models.WhiteList
                              {
                                        UserId = userId,
                                        CreatedDate = DateTime.UtcNow,
                                        WhiteListItems = new List<WhiteListItem>()
                              };
                              await _context.WhiteLists.AddAsync(whiteList);
                    }

                    //Add item to whitelist
                    var item = new WhiteListItem
                    {
                              WhiteListId = whiteList.Id,
                              BookId = addToWhiteListDto.BookId,
                              AddedDate = DateTime.UtcNow
                    };
                    whiteList.WhiteListItems.Add(item);

                    //Save changes
                    await _context.SaveChangesAsync();

                    //return update whitelist books
                    return await GetWhiteLisTAsync(userId);

          }

          public async Task<WhiteListDto> GetWhiteLisTAsync(Guid userId)
          {
                    //checking if user exist
                    if(!await _userService.IfUserExist(userId)) throw new KeyNotFoundException("User not found");

                    //Get whitelist with items and books
                    var whiteList = await _context.WhiteLists
                              .Include(w => w.WhiteListItems)
                              .ThenInclude(w => w.Book)
                              .FirstOrDefaultAsync(w => w.UserId == userId);

                    if(whiteList == null)
                    {
                              throw new KeyNotFoundException("Whitelist not Found");
                    }

                    //Mapping To DTO
                    return _mapper.Map<WhiteListDto>(whiteList);
          }

          // Only check if any book item is in whitelist or not.
          public async Task<bool> IsInWhiteListAsync(Guid userId, Guid bookId)
          {
                    // Check if user exists
                    if(!await _userService.IfUserExist(userId)) throw new KeyNotFoundException("User not found");

                    //Get whitelist of user
                    var whiteList = await _context.WhiteLists
                              .Include(w => w.WhiteListItems)
                              .FirstOrDefaultAsync(w => w.UserId == userId);
                    
                    //check if whitelist is null
                    if(whiteList == null)
                    {
                              return false;
                    }

                    // check if book is in whitelist and return
                    return whiteList.WhiteListItems.Any(w => w.BookId == bookId);
          }

          public async Task<CartDto> MoveTOCartAsync(Guid userId, Guid bookId, int quantity)
          { 
                   //check if user exist
                   if(!await _userService.IfUserExist(userId)) throw new KeyNotFoundException("User not found");

                   //check if book item is in whitelist
                   if(!await IsInWhiteListAsync(userId, bookId)) throw new KeyNotFoundException("Item is not in whitelist");

                    var addToCart = new AddToCartDto
                    {
                              BookId = bookId,
                              Quantity = quantity
                    };

                    var cartDto = await _cartService.AddToCartAsync(userId, addToCart);

                    await RemoveFromWhiteListAsync(userId, bookId);

                    return cartDto; 
          }

          public async Task<WhiteListDto> RemoveFromWhiteListAsync(Guid userId, Guid bookId)
          {
                    if(!await _userService.IfUserExist(userId)) throw new KeyNotFoundException("User not found");
                    
                    //Get whitelist
                    var whiteList = await _context.WhiteLists
                              .Include(W => W.WhiteListItems)
                              .FirstOrDefaultAsync(W => W.UserId == userId);
                    
                    if(whiteList == null) throw new KeyNotFoundException("WhiteList Not Found");

                    var whiteListItem = whiteList.WhiteListItems.FirstOrDefault(i => i.BookId == bookId);

                    if (whiteListItem == null) throw new KeyNotFoundException("Book not found in Whitelist");

                    // Remove item from whitelist
                    whiteList.WhiteListItems.Remove(whiteListItem);
                    _context.WhiteListsItems.Remove(whiteListItem);

                    await _context.SaveChangesAsync();

                    return await GetWhiteLisTAsync(userId);
          }
}
