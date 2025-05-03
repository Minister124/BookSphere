using System;
using BookSphere.Data;
using BookSphere.DTOs;
using BookSphere.IServices;

namespace BookSphere.Services;

public class BookService : IBookServices
{
          private readonly BookSphereDbContext _context;

          public BookService(BookSphereDbContext context)
          {
                    _context = context;
          }
          public Task<BookDto> CreateBookAsync(CreateBookDto create)
          {
                    throw new NotImplementedException();
          }

          public Task<bool> DeleteBookAsync(Guid id)
          {
                    throw new NotImplementedException();
          }

          public Task<PaginatedResponse<BookDto>> GetBookAsync(int pageNumber, int pageSize, BookFIlterDto fIlterDto)
          {
                    throw new NotImplementedException();
          }

          public Task<BookDetailsDto> GetBookDetailsAsync(Guid bookId)
          {
                    throw new NotImplementedException();
          }

          public async Task<bool> IsInStockAsync(Guid bookId, int quantity)
          {
                    var book = await _context.Books.FindAsync(bookId);

                    if(book == null) throw new KeyNotFoundException("Book Not Found");

                    return book.StockQuantity >= quantity;
          }

          public Task<BookDto> SetDiscountAsync(Guid bookId, SetDiscountDto discount)
          {
                    throw new NotImplementedException();
          }

          public Task<BookDto> UpdateBookAsync(Guid id, UpdateBookDto update)
          {
                    throw new NotImplementedException();
          }

          public Task<BookDto> UpdateInventoryAsync(Guid bookId, int quantity)
          {
                    throw new NotImplementedException();
          }
}
