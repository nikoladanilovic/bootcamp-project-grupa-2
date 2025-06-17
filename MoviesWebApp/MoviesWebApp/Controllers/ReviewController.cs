using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApp.Model;
using MoviesWebApp.RESTModels;
using MoviesWebApp.Service.Common;

namespace MoviesWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        public readonly IReviewService reviewService;
        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        [HttpGet("get-review")]
        public async Task<IActionResult> GetAllReviewAsync()
        {
            var review = await reviewService.GetAllReviewsAsync();
            var reviewRest = review.Select(u => new ReviewREST
            {
                Id = u.Id,
                UserId = u.UserId,
                MovieId = u.MovieId,
                Rating = u.Rating,
                Comment = u.Comment
            });

            return Ok(reviewRest); 
        }

        [HttpGet("get-review/{id}")]
        public async Task<ActionResult<ReviewREST>> GetReviewAsync(Guid id)
        {
            var review = await reviewService.GetReviewByIdAsync(id);
            if (review == null)
                return NotFound();

            var reviewRest = new ReviewREST
            {
                Id = review.Id,
                UserId = review.UserId,
                MovieId = review.MovieId,
                Rating = review.Rating,
                Comment = review.Comment
            };

            return Ok(reviewRest);
        }

        [HttpPost("create-Review")]
        public async Task<IActionResult> CreateReviewAsync([FromBody] List<ReviewREST> reviewREST)
        {
            if (reviewREST == null || reviewREST.Count == 0)
            {
                return BadRequest("Review cannot be null or empty.");
            }

            var reviews = reviewREST.Select(review => new Review
            {
                Id = review.Id,
                UserId = review.UserId,
                MovieId = review.MovieId,
                Rating = review.Rating,
                Comment = review.Comment
            }).ToList(); 

            await reviewService.CreateReviewAsync(reviews); 
            return Ok();
        }

        [HttpPut("update-Review")]
        public async Task<IActionResult> UpdateReviewAsync(Guid id, [FromBody] ReviewREST reviewREST)
        {
            if (reviewREST == null)
                return BadRequest("Review cannot be null.");

            var review = new Review
            {
                Id = reviewREST.Id,
                UserId = reviewREST.UserId,
                MovieId = reviewREST.MovieId,
                Rating = reviewREST.Rating,
                Comment = reviewREST.Comment
            };

            bool updated = await reviewService.UpdateReviewAsync(id, review);
            if (!updated)
                return NotFound("Review not found or not updated.");

            return Ok("Review updated.");
        }

        [HttpDelete("delete-Review/{id}")]
        public async Task<IActionResult> DeletePlayerAsync(Guid id)
        {
            await reviewService.DeleteReviewAsync(id); 
            return NoContent();
        }
    }
}
