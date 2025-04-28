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

          public Task<bool> IsInWhiteListAsync(Guid userId, Guid bookId)
          {
                    throw new NotImplementedException();
          }

          public Task<CartDto> MoveTOCartAsync(Guid userId, Guid bookId, int quantity)
          {
                    throw new NotImplementedException();
          }

          public Task<WhiteListDto> RemoveFromWhiteListAsync(Guid userId, Guid bookId)
          {
                    throw new NotImplementedException();
          }
}
