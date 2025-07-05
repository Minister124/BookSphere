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
    public class ReviewService : IReviewService
    {
        private readonly BookSphereDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService; // To check if user exists

        public ReviewService(BookSphereDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<bool> CanReviewBookAsync(Guid userId, Guid bookId)
        {
            if (!await _userService.IfUserExist(userId))
            {
                throw new KeyNotFoundException("User not found.");
            }

            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
            {
                throw new KeyNotFoundException("Book not found.");
            }

            // Check if there's any order item for this user and book
            // And the order status is considered "completed" (e.g. not "Cancelled" or "Pending" indefinitely)
            // For simplicity, we assume any order means purchased. Refine with order status if needed.
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .AnyAsync(oi => oi.BookId == bookId && oi.Order.UserId == userId && oi.Order.Status != "Cancelled");
        }

        public async Task<ReviewDto> CreateReviewAsync(Guid userId, CreateReviewDto createReviewDto)
        {
            if (!await _userService.IfUserExist(userId))
            {
                throw new KeyNotFoundException("User not found.");
            }

            var book = await _context.Books.FindAsync(createReviewDto.BookId);
            if (book == null)
            {
                throw new KeyNotFoundException("Book not found.");
            }

            // Check if user is eligible to review (purchased the book)
            if (!await CanReviewBookAsync(userId, createReviewDto.BookId))
            {
                throw new InvalidOperationException("User has not purchased this book and cannot leave a review.");
            }

            // Check if user has already reviewed this book
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == createReviewDto.BookId);

            if (existingReview != null)
            {
                throw new InvalidOperationException("User has already reviewed this book.");
            }

            var review = _mapper.Map<Review>(createReviewDto);
            review.Id = Guid.NewGuid();
            review.UserId = userId;
            review.ReviewDate = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // It's good practice to load related User to populate UserName in ReviewDto
            // However, our mapping profile for Review to ReviewDto already does this.
            // To ensure it works, we need to make sure the User is loaded for the mapping.
            // The simplest way is to fetch the review again with the User included.
            var createdReviewWithUser = await _context.Reviews
                                            .Include(r => r.User)
                                            .FirstAsync(r => r.Id == review.Id);

            return _mapper.Map<ReviewDto>(createdReviewWithUser);
        }

        public async Task<bool> DeleteReviewAsync(Guid userId, Guid reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                throw new KeyNotFoundException("Review not found.");
            }

            // Optional: Check if the user owns the review or if an admin is deleting
            if (review.UserId != userId)
            {
                // Further check if current user is admin/staff if such roles can delete any review
                // For now, only owner can delete
                var user = await _context.Users.FindAsync(userId);
                if (user == null || (user.Role != "Admin" && user.Role != "Staff"))
                {
                    throw new UnauthorizedAccessException("User is not authorized to delete this review.");
                }
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetBookAverageRatingAsync(Guid bookId)
        {
            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
            {
                throw new KeyNotFoundException("Book not found.");
            }

            var reviews = await _context.Reviews.Where(r => r.BookId == bookId).ToListAsync();
            if (!reviews.Any())
            {
                return 0; // Or handle as no reviews yet
            }

            return reviews.Average(r => r.Rating);
        }

        public async Task<List<ReviewDto>> GetBookReviewsAsync(Guid bookId)
        {
            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
            {
                throw new KeyNotFoundException("Book not found.");
            }

            var reviews = await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.User) // Include User to populate UserName in ReviewDto
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto> GetReviewAsync(Guid reviewId)
        {
            var review = await _context.Reviews
                .Include(r => r.User) // Include User to populate UserName
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
            {
                throw new KeyNotFoundException("Review not found.");
            }
            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<ReviewDto> UpdateReviewAsync(Guid userId, Guid reviewId, CreateReviewDto updateReviewDto)
        {
            var review = await _context.Reviews
                                .Include(r => r.User) // Include user for mapping later
                                .FirstOrDefaultAsync(r => r.Id == reviewId);
            if (review == null)
            {
                throw new KeyNotFoundException("Review not found.");
            }

            if (review.UserId != userId)
            {
                throw new UnauthorizedAccessException("User is not authorized to update this review.");
            }

            // Cannot change the book for a review, so we ignore updateReviewDto.BookId if it differs
            if (review.BookId != updateReviewDto.BookId)
            {
                // Log this attempt or handle as an error, but generally a review is for a specific book.
                // For this implementation, we will assume BookId in DTO must match or is ignored.
                // Let's enforce it must match:
                 throw new InvalidOperationException("Cannot change the book association of a review. The BookId in your request does not match the existing review's BookId.");
            }


            // Update properties
            review.Rating = updateReviewDto.Rating;
            review.Comment = updateReviewDto.Comment;
            review.ReviewDate = DateTime.UtcNow; // Update review date to reflect modification

            // _mapper.Map(updateReviewDto, review); // Alternative if not manually setting
            // Be careful with AutoMapper here if CreateReviewDto has BookId and you don't want it to change.

            await _context.SaveChangesAsync();
            return _mapper.Map<ReviewDto>(review); // User should already be loaded
        }
    }
}
