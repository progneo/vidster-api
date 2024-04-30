
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vidster_api.Context;
using vidster_api.Models;

namespace vidster_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class ReviewsController(VidsterContext context) : ControllerBase
    {
        // GET: api/Reviews
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            return await context.Reviews.ToListAsync();
        }

        // GET: api/Reviews/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}"), AllowAnonymous]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            var review = await context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        // PUT: api/Reviews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut, Authorize(Roles = "admin")]
        public async Task<IActionResult> PutReview(int id, Review review)
        {
            if (id != review.Id)
            {
                return BadRequest();
            }

            context.Entry(review).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // POST: api/Reviews
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost, Authorize(Roles = "creator,customer")]
        public async Task<ActionResult<Review>> PostReview(ReviewViewModel review)
        {
            var creator = await context.Creators.FindAsync(review.CreatorId);
            if (creator == null)
            {
                return NotFound();
            }

            var newReview = new Review
            {
                CreatorId = review.CreatorId,
                Rating = review.Rating
            };
            
            context.Reviews.Add(newReview);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetReview", new { id = newReview.Id }, newReview);
        }

        // DELETE: api/Reviews/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            context.Reviews.Remove(review);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReviewExists(int id)
        {
            return context.Reviews.Any(e => e.Id == id);
        }
    }
}
