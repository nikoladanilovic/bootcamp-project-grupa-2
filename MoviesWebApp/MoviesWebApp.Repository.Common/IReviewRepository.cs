using MoviesWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApp.Repository.Common
{
   public interface IReviewRepository
    {
        public Task<IEnumerable<Review>> GetAllReviewsAsync();
        public Task<Review?> GetReviewByIdAsync(Guid id);
        public Task CreateReviewAsync(List<Review> reviews);
        public Task<bool> UpdateReviewAsync(Guid id, Review review);
        public Task DeleteReviewAsync(Guid id);
        public Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId);
        public Task<bool> ReviewExistsAsync(Guid userId, Guid movieId);

    }
}
