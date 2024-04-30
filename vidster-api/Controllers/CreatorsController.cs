using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vidster_api.Context;
using vidster_api.Models;
using vidster_api.Tools;
using vidster_api.ViewModels;

namespace vidster_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class CreatorsController(VidsterContext context) : ControllerBase
    {
        // GET: api/Creators
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<PaginatedList<Creator>>> GetCreators([FromQuery] CreatorsFilter filter)
        {
            IEnumerable<Creator> query = context.Creators
                .Include(creator => creator.Reviews)
                .Include(c => c.TagsInCreator)
                .ThenInclude(t => t.Tag);

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(c => c.Username.ToLower().Contains(filter.Name.ToLower()));
            }

            if (filter.Tags is { Count: > 0 })
            {
                query = query.Where(c => c.TagsInCreator.Any(t => t.Tag != null && filter.Tags.Contains(t.Tag.Name)));
            }

            query = filter.SortBy.ToLower() switch
            {
                "rating" => query.OrderByDescending(c => c.Reviews.Average(r => r.Rating)),
                "date" => query.OrderByDescending(c => c.CreatedAt),
                _ => query.OrderBy(c => c.Username)
            };

            var list = await PaginatedList<Creator>.Create(query, filter.CurrentPage, 10);

            return list;
        }

        // GET: api/Creators/Top
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Top"), AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Creator>>> GetTopCreators()
        {
            var topCreatorsWithReviews = await context.Creators
                .Include(creator => creator.Reviews)
                .Include(c => c.TagsInCreator)
                .ThenInclude(t => t.Tag)
                .Where(c => c.Reviews.Count != 0)
                .OrderByDescending(c => c.Reviews.Average(r => r.Rating))
                .Take(4)
                .ToListAsync();

            if (topCreatorsWithReviews.Count >= 4) return topCreatorsWithReviews;
            {
                var needed = 4 - topCreatorsWithReviews.Count;
                var creatorsWithoutReviews = await context.Creators
                    .Include(creator => creator.Reviews)
                    .Include(c => c.TagsInCreator)
                    .ThenInclude(t => t.Tag)
                    .Where(c => c.Reviews.Count == 0)
                    .Take(needed)
                    .ToListAsync();

                topCreatorsWithReviews.AddRange(creatorsWithoutReviews);
            }

            return topCreatorsWithReviews;
        }

        // GET: api/Creators/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}"), AllowAnonymous]
        public async Task<ActionResult<Creator>> GetCreator(int id)
        {
            var creator = await context.Creators
                .Include(c => c.Works)
                .Include(c => c.TagsInCreator)
                .ThenInclude(t => t.Tag)
                .Include(c => c.Reviews)
                .Include(c => c.ServiceOfCreator)
                .ThenInclude(s => s.Service)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (creator == null)
            {
                return NotFound();
            }

            return creator;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("user/{id:int}"), AllowAnonymous]
        public async Task<ActionResult<Creator>> GetCreatorByUserId(int id)
        {
            var creator = await context.Creators
                .Include(c => c.Works)
                .Include(c => c.TagsInCreator)
                .ThenInclude(t => t.Tag)
                .Include(c => c.Reviews)
                .Include(c => c.ServiceOfCreator)
                .ThenInclude(s => s.Service)
                .FirstOrDefaultAsync(c => c.UserId == id);

            if (creator == null)
            {
                return NotFound();
            }

            return creator;
        }

        // PUT: api/Creators/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut, Authorize(Roles = "creator")]
        public async Task<IActionResult> PutCreator(NewCreatorViewModel creator)
        {
            var email = User.FindFirst(c => c.Type == ClaimsIdentity.DefaultNameClaimType)!.Value;
            var user = context.Users.First(u => u.Email == email);
            var userCreator = context.Creators.FirstOrDefault(c => c.UserId == user.Id);

            if (userCreator == null) return NotFound();

            var tags = await context.TagInCreators.Where(t => t.CreatorId == userCreator.Id).ToListAsync();

            context.TagInCreators.RemoveRange(tags);
            await context.SaveChangesAsync();

            userCreator.Username = creator.Username;
            userCreator.Avatar = creator.Avatar;
            userCreator.Thumbnail = creator.Thumbnail;
            userCreator.Description = creator.Description;
            userCreator.Address = creator.Address;
            userCreator.UpdatedAt = new DateOnly();

            context.Entry(userCreator).State = EntityState.Modified;
            await context.SaveChangesAsync();

            foreach (var tagId in creator.Tags)
            {
                var newTagInCreator = new TagInCreator
                {
                    CreatorId = userCreator.Id,
                    TagId = tagId
                };

                context.TagInCreators.Add(newTagInCreator);
                await context.SaveChangesAsync();
            }

            return NoContent();
        }

        // POST: api/Creators
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost, Authorize(Roles = "creator")]
        public async Task<ActionResult<Creator>> PostCreator(NewCreatorViewModel creator)
        {
            var email = User.FindFirst(c => c.Type == ClaimsIdentity.DefaultNameClaimType)!.Value;
            var user = context.Users.First(u => u.Email == email);

            if (context.Creators.Any(c => c.UserId.Equals(user.Id)))
            {
                return Conflict("Creator with this user ID already registered");
            }

            var newCreator = new Creator
            {
                UserId = user.Id,
                Username = creator.Username,
                Avatar = creator.Avatar,
                Thumbnail = creator.Thumbnail,
                Description = creator.Description,
                Address = creator.Address
            };

            context.Creators.Add(newCreator);
            await context.SaveChangesAsync();

            foreach (var tagId in creator.Tags)
            {
                var newTagInCreator = new TagInCreator
                {
                    CreatorId = newCreator.Id,
                    TagId = tagId
                };

                context.TagInCreators.Add(newTagInCreator);
                await context.SaveChangesAsync();
            }

            return CreatedAtAction("GetCreator", new { id = newCreator.Id }, newCreator);
        }

        // DELETE: api/Creators/5
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCreator(int id)
        {
            var creator = await context.Creators.FindAsync(id);
            if (creator == null)
            {
                return NotFound();
            }

            context.Creators.Remove(creator);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}