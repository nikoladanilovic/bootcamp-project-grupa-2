using MoviesWebApp.Model;
using MoviesWebApp.Repository.Common;
using MoviesWebApp.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Service
{
    public class ReviewService : IReviewService
    {
        public readonly IReviewRepository _reviewsRepository;
        public ReviewService(IReviewRepository reviewsRepository)
        {
            _reviewsRepository = reviewsRepository;
        }
        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _reviewsRepository.GetAllReviewsAsync();
        }
        public async Task<Review?> GetReviewByIdAsync(Guid id)
        {
            return await _reviewsRepository.GetReviewByIdAsync(id);
        }
        public async Task CreateReviewAsync(List<Review> reviews)
        {
            foreach (var review in reviews)
            {
                if (review.UserId == Guid.Empty)
                    throw new ArgumentException("UserId is required.");
                if (review.MovieId == Guid.Empty)
                    throw new ArgumentException("MovieId is required.");
                if (review.Rating < 1 || review.Rating > 10)
                    throw new ArgumentException("Rating must be between 1 and 10.");

                if (await _reviewsRepository.ReviewExistsAsync(review.UserId, review.MovieId))
                    throw new InvalidOperationException($"User {review.UserId} has already reviewed movie {review.MovieId}.");

                review.CreatedAt = DateTime.UtcNow;
            }
            await _reviewsRepository.CreateReviewAsync(reviews);
        }

        public async Task<bool> UpdateReviewAsync(Guid id, Review review)
        {
            if (review.UserId == Guid.Empty)
                throw new ArgumentException("UserId is required.");
            if (review.MovieId == Guid.Empty)
                throw new ArgumentException("MovieId is required.");
            if (review.Rating < 1 || review.Rating > 10)
                throw new ArgumentException("Rating must be between 1 and 10.");

            var existingReview = await _reviewsRepository.GetReviewByIdAsync(id);
            if (existingReview == null)
                return false;

            if (review.UserId != existingReview.UserId || review.MovieId != existingReview.MovieId)
            {
                if (await _reviewsRepository.ReviewExistsAsync(review.UserId, review.MovieId))
                    throw new InvalidOperationException($"User {review.UserId} has already reviewed movie {review.MovieId}.");
            }

            return await _reviewsRepository.UpdateReviewAsync(id, review);
        }

        public async Task DeleteReviewAsync(Guid id)
        {
            await _reviewsRepository.DeleteReviewAsync(id);
        }
        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId)
        {
            return await _reviewsRepository.GetReviewsByUserIdAsync(userId);
        }
    }
}
