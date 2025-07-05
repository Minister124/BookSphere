using System;
using BookSphere.Data;
using BookSphere.DTOs;
using AutoMapper;
using BookSphere.Data;
using BookSphere.DTOs;
using BookSphere.IServices;
using BookSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace BookSphere.Services;

public class BookService : IBookServices
{
          private readonly BookSphereDbContext _context;
          private readonly IMapper _mapper;

          public BookService(BookSphereDbContext context, IMapper mapper)
          {
                    _context = context;
                    _mapper = mapper;
          }
          public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
          {
                    // Check if a book with the same ISBN already exists
                    if (await _context.Books.AnyAsync(b => b.ISBN == createBookDto.ISBN))
                    {
                              throw new InvalidOperationException("A book with the same ISBN already exists.");
                    }

                    var book = _mapper.Map<Book>(createBookDto);
                    book.Id = Guid.NewGuid();
                    book.ListedDate = DateTime.UtcNow;

                    _context.Books.Add(book);
                    await _context.SaveChangesAsync();

                    return _mapper.Map<BookDto>(book);
          }

          public async Task<bool> DeleteBookAsync(Guid id)
          {
                    var book = await _context.Books.FindAsync(id);
                    if (book == null)
                    {
                              return false; // Or throw KeyNotFoundException
                    }

                    _context.Books.Remove(book);
                    await _context.SaveChangesAsync();
                    return true;
          }

          public async Task<PaginatedResponse<BookDto>> GetBookAsync(int pageNumber, int pageSize, BookFIlterDto filterDto)
          {
                    var query = _context.Books.AsQueryable();

                    // Apply filters
                    if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
                    {
                              query = query.Where(b => b.Title.Contains(filterDto.SearchTerm) || b.Author.Contains(filterDto.SearchTerm) || b.ISBN.Contains(filterDto.SearchTerm));
                    }
                    if (!string.IsNullOrWhiteSpace(filterDto.Author))
                    {
                              query = query.Where(b => b.Author.Contains(filterDto.Author));
                    }
                    if (!string.IsNullOrWhiteSpace(filterDto.Genre))
                    {
                              query = query.Where(b => b.Genre == filterDto.Genre);
                    }
                    if (filterDto.InStock.HasValue)
                    {
                              query = query.Where(b => b.StockQuantity > 0 == filterDto.InStock.Value);
                    }
                    if (filterDto.PhysicalLibraryAccess.HasValue)
                    {
                        query = query.Where(b => b.PhysicalLibraryAccess == filterDto.PhysicalLibraryAccess.Value);
                    }
                    if (filterDto.MinPrice.HasValue)
                    {
                              query = query.Where(b => b.Price >= filterDto.MinPrice.Value);
                    }
                    if (filterDto.MaxPrice.HasValue)
                    {
                              query = query.Where(b => b.Price <= filterDto.MaxPrice.Value);
                    }
                     if (filterDto.MinRating.HasValue) // Assuming Reviews relation and Rating property exist
                    {
                        query = query.Where(b => b.Reviews.Any() && b.Reviews.Average(r => r.Rating) >= filterDto.MinRating.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(filterDto.Language))
                    {
                        query = query.Where(b => b.Language == filterDto.Language);
                    }
                    if (!string.IsNullOrWhiteSpace(filterDto.Format))
                    {
                        query = query.Where(b => b.Format == filterDto.Format);
                    }
                    if (!string.IsNullOrWhiteSpace(filterDto.Publisher))
                    {
                        query = query.Where(b => b.Publisher.Contains(filterDto.Publisher));
                    }
                    if (filterDto.IsOnSale.HasValue)
                    {
                        query = query.Where(b => b.IsOnSale == filterDto.IsOnSale.Value);
                    }
                    if (filterDto.IsAwardWinner.HasValue)
                    {
                        query = query.Where(b => b.IsAwardWinner == filterDto.IsAwardWinner.Value);
                    }
                    if (filterDto.IsBestseller.HasValue)
                    {
                        query = query.Where(b => b.IsBestSeller == filterDto.IsBestseller.Value); // Corrected to IsBestSeller
                    }
                    if (filterDto.IsNewRelease.HasValue) // Assuming New Release means published in last 30 days
                    {
                        query = query.Where(b => b.PublicationDate >= DateTime.UtcNow.AddDays(-30) == filterDto.IsNewRelease.Value);
                    }
                    if (filterDto.IsNewArrival.HasValue) // Assuming New Arrival means listed in last 30 days
                    {
                        query = query.Where(b => b.ListedDate >= DateTime.UtcNow.AddDays(-30) == filterDto.IsNewArrival.Value);
                    }
                    if (filterDto.IsComingSoon.HasValue)
                    {
                        query = query.Where(b => b.IsComingSoon == filterDto.IsComingSoon.Value);
                    }


                    // Apply sorting
                    if (!string.IsNullOrWhiteSpace(filterDto.SortBy))
                    {
                              switch (filterDto.SortBy.ToLower())
                              {
                                        case "title":
                                                  query = filterDto.SortDescending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title);
                                                  break;
                                        case "author":
                                                  query = filterDto.SortDescending ? query.OrderByDescending(b => b.Author) : query.OrderBy(b => b.Author);
                                                  break;
                                        case "price":
                                                  query = filterDto.SortDescending ? query.OrderByDescending(b => b.Price) : query.OrderBy(b => b.Price);
                                                  break;
                                        case "publicationdate":
                                                  query = filterDto.SortDescending ? query.OrderByDescending(b => b.PublicationDate) : query.OrderBy(b => b.PublicationDate);
                                                  break;
                                        case "rating": // Assuming Reviews relation and Rating property exist
                                            query = filterDto.SortDescending ? query.OrderByDescending(b => b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0) : query.OrderBy(b => b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0);
                                            break;
                                        default:
                                                  query = query.OrderBy(b => b.ListedDate); // Default sort
                                                  break;
                              }
                    }
                    else
                    {
                              query = query.OrderBy(b => b.ListedDate); // Default sort
                    }

                    var totalCount = await query.CountAsync();
                    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                    var books = await query
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

                    var bookDtos = _mapper.Map<List<BookDto>>(books);

                    return new PaginatedResponse<BookDto>
                    {
                              Items = bookDtos,
                              PageNumber = pageNumber,
                              PageSize = pageSize,
                              TotalCount = totalCount,
                              TotalPages = totalPages
                    };
          }

          public async Task<BookDetailsDto> GetBookDetailsAsync(Guid bookId)
          {
                    var book = await _context.Books
                                        .Include(b => b.Reviews)
                                        .ThenInclude(r => r.User) // Include User to map UserName in ReviewDto
                                        .FirstOrDefaultAsync(b => b.Id == bookId);

                    if (book == null)
                    {
                              throw new KeyNotFoundException("Book not found.");
                    }

                    var bookDetailsDto = _mapper.Map<BookDetailsDto>(book);
                    // AverageRating and ReviewCount are already mapped by AutoMapper profile if Reviews are loaded
                    return bookDetailsDto;
          }

          public async Task<bool> IsInStockAsync(Guid bookId, int quantity)
          {
                    var book = await _context.Books.FindAsync(bookId);

                    if(book == null) throw new KeyNotFoundException("Book Not Found");

                    return book.StockQuantity >= quantity;
          }

          public async Task<BookDto> SetDiscountAsync(Guid bookId, SetDiscountDto discountDto)
          {
                    if (bookId != discountDto.BookId)
                    {
                        throw new ArgumentException("BookId in path does not match BookId in payload.");
                    }

                    var book = await _context.Books.FindAsync(bookId);
                    if (book == null)
                    {
                              throw new KeyNotFoundException("Book not found.");
                    }

                    if (discountDto.StartDate >= discountDto.EndDate)
                    {
                        throw new ArgumentException("Discount start date must be before end date.");
                    }

                    book.IsOnSale = discountDto.IsOnSale;
                    book.DiscountPercentage = discountDto.DiscountPercentage;
                    book.DiscountStartDate = discountDto.StartDate;
                    book.DiscountEndDate = discountDto.EndDate;

                    await _context.SaveChangesAsync();

                    return _mapper.Map<BookDto>(book);
          }

          public async Task<BookDto> UpdateBookAsync(Guid id, UpdateBookDto updateBookDto)
          {
                    if (id != updateBookDto.Id)
                    {
                        throw new ArgumentException("ID in path does not match ID in payload.");
                    }

                    var book = await _context.Books.FindAsync(id);
                    if (book == null)
                    {
                              throw new KeyNotFoundException("Book not found.");
                    }

                    // Check if ISBN is being changed and if the new ISBN already exists for another book
                    if (book.ISBN != updateBookDto.ISBN && await _context.Books.AnyAsync(b => b.ISBN == updateBookDto.ISBN && b.Id != id))
                    {
                        throw new InvalidOperationException("Another book with the same new ISBN already exists.");
                    }

                    _mapper.Map(updateBookDto, book); // AutoMapper will update the existing book entity

                    await _context.SaveChangesAsync();

                    return _mapper.Map<BookDto>(book);
          }

          public async Task<BookDto> UpdateInventoryAsync(Guid bookId, int quantity)
          {
                    var book = await _context.Books.FindAsync(bookId);
                    if (book == null)
                    {
                              throw new KeyNotFoundException("Book not found.");
                    }

                    if (book.StockQuantity + quantity < 0)
                    {
                              throw new InvalidOperationException("Stock quantity cannot be negative.");
                    }

                    book.StockQuantity += quantity;
                    await _context.SaveChangesAsync();

                    return _mapper.Map<BookDto>(book);
          }
}
