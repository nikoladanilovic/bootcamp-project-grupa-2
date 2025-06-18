using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApp.Model;
using MoviesWebApp.RESTModels;
using MoviesWebApp.Service.Common;
using Npgsql;

namespace MoviesWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService reviewService;
        private readonly IMapper _mapper;

        public ReviewController(IReviewService reviewService, IMapper mapper)
        {
            this.reviewService = reviewService;
            _mapper = mapper;
        }

        [HttpGet("get-review")]
        public async Task<IActionResult> GetAllReviewAsync()
        {
            var reviews = await reviewService.GetAllReviewsAsync();
            var reviewRest = _mapper.Map<IEnumerable<ReviewREST>>(reviews);
            return Ok(reviewRest);
        }

        [HttpGet("get-review/{id}")]
        public async Task<ActionResult<ReviewREST>> GetReviewAsync(Guid id)
        {
            var review = await reviewService.GetReviewByIdAsync(id);
            if (review == null)
                return NotFound();

            var reviewRest = _mapper.Map<ReviewREST>(review);
            return Ok(reviewRest);
        }

        [HttpPost("create-review")]
        public async Task<IActionResult> CreateReviewAsync([FromBody] List<ReviewREST> reviewREST)
        {
            if (reviewREST == null || reviewREST.Count == 0)
                return BadRequest("Reviews cannot be null or empty.");

            try
            {
                var reviews = _mapper.Map<List<Review>>(reviewREST);
                await reviewService.CreateReviewAsync(reviews);
                return Ok(_mapper.Map<List<ReviewREST>>(reviews));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (PostgresException ex) when (ex.SqlState == "23505" && ex.ConstraintName == "reviews_user_movie_unique")
            {
                return BadRequest("This user has already reviewed this movie.");
            }
        }

        [HttpPut("update-review")]
        public async Task<IActionResult> UpdateReviewAsync(Guid id, [FromBody] ReviewREST reviewREST)
        {
            try
            {
                var review = _mapper.Map<Review>(reviewREST);
                bool updated = await reviewService.UpdateReviewAsync(id, review);
                if (!updated)
                    return NotFound("Review not found or not updated.");

                return Ok(_mapper.Map<ReviewREST>(review));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (PostgresException ex) when (ex.SqlState == "23505" && ex.ConstraintName == "reviews_user_movie_unique")
            {
                return BadRequest("This user has already reviewed this movie with that ID.");
            }
        }

        [HttpDelete("delete-review/{id}")]
        public async Task<IActionResult> DeleteReviewAsync(Guid id)
        {
            var review = await reviewService.GetReviewByIdAsync(id);
            if (review == null)
                return NotFound();

            await reviewService.DeleteReviewAsync(id);
            return NoContent();
        }
        [HttpGet("get-user-review{user_id}")]
        public async Task<IActionResult> GetReviewsByUserIdAsync(Guid user_id)
        {
            var reviews = await reviewService.GetReviewsByUserIdAsync(user_id);
            if (reviews == null || !reviews.Any())
                return NotFound("No reviews found for this user.");
            var reviewRest = _mapper.Map<IEnumerable<ReviewREST>>(reviews);
            return Ok(reviewRest);
        }

    }
}