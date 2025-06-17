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
            await _reviewsRepository.CreateReviewAsync(reviews);
        }
        public async Task<bool> UpdateReviewAsync(Guid id, Review review)
        {
            return await _reviewsRepository.UpdateReviewAsync(id, review);
        }
        public async Task DeleteReviewAsync(Guid id)
        {
            await _reviewsRepository.DeleteReviewAsync(id);
        }
    }
}
