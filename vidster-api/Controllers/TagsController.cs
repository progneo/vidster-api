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
    public class TagsController(VidsterContext context) : ControllerBase
    {
        // GET: api/Tags
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            return await context.Tags.ToListAsync();
        }

        // GET: api/Tags/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("{id:int}"), AllowAnonymous]
        public async Task<ActionResult<Tag>> GetTag(int id)
        {
            var tag = await context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return tag;
        }

        // PUT: api/Tags/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("{id:int}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> PutTag(int id, Tag tag)
        {
            if (id != tag.Id)
            {
                return BadRequest();
            }

            context.Entry(tag).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // POST: api/Tags
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost, Authorize(Roles = "admin")]
        public async Task<ActionResult<Tag>> PostTag(Tag tag)
        {
            context.Tags.Add(tag);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetTag", new { id = tag.Id }, tag);
        }

        // DELETE: api/Tags/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("{id:int}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            context.Tags.Remove(tag);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool TagExists(int id)
        {
            return context.Tags.Any(e => e.Id == id);
        }
    }
}