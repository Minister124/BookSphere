using System;
using BookSphere.DTOs;

namespace BookSphere.IServices;

public interface IBookServices
{
            Task<PaginatedResponse<BookDto>> GetBookAsync(int pageNumber, int pageSize, BookFIlterDto? fIlterDto); //for browsing and filtering books

            Task<BookDetailsDto> GetBookDetailsAsync(Guid bookId); //Get book details

            //Admin CRUD opration
            Task<BookDto> CreateBookAsync(CreateBookDto create);
            Task<BookDto> UpdateBookAsync(Guid id, UpdateBookDto update);

            Task<bool> DeleteBookAsync(Guid id);

            //Admin Inventory Management
            Task<BookDto> UpdateInventoryAsync(Guid bookId, int quantity);
            Task<BookDto> SetDiscountAsync(Guid bookId, SetDiscountDto discount);
            Task<bool> IsInStockAsync(Guid bookId, int quantity);
}
