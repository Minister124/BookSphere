using System;
using BookSphere.DTOs;

namespace BookSphere.IServices;

public interface IReviewService
{
          //Get all reviews for a book
          Task<List<ReviewDto>> GetBookReviewsAsync(Guid bookId);

          //Get a specific review
          Task<ReviewDto> GetReviewAsync(Guid reviewId);

          //Create a review
          Task<ReviewDto> CreateReviewAsync(Guid userID, CreateReviewDto createReviewDto);

          //update a review
          Task<ReviewDto> UpdateReviewAsync(Guid userId, Guid reviewId, CreateReviewDto updateReview);
          
          //Delete a review
          Task<bool> DeleteReviewAsync(Guid userId, Guid reviewId);

          //Check if user has purchased the book (eligble to review)
          Task<bool> CanReviewBookAsync(Guid userId, Guid bookId);
          
          //Get average rating for a book
          Task<double> GetBookAverageRatingAsync(Guid bookId);
}
