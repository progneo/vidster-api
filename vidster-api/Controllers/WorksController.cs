using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vidster_api.Context;
using vidster_api.Models;
using vidster_api.ViewModels;

namespace vidster_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class WorksController(VidsterContext context) : ControllerBase
    {
        // GET: api/Works
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Work>>> GetWorks()
        {
            return await context.Works.ToListAsync();
        }

        // GET: api/Works/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}"), AllowAnonymous]
        public async Task<ActionResult<Work>> GetWork(int id)
        {
            var work = await context.Works.FindAsync(id);

            if (work == null)
            {
                return NotFound();
            }

            return work;
        }

        // PUT: api/Works/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:int}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> PutWork(int id, Work work)
        {
            if (id != work.Id)
            {
                return BadRequest();
            }

            context.Entry(work).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // POST: api/Works
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost, Authorize(Roles = "creator")]
        public async Task<ActionResult<Work>> PostWork(WorkViewModel work)
        {
            var email = User.FindFirst(c => c.Type == ClaimsIdentity.DefaultNameClaimType)!.Value;
            var user = context.Users.First(u => u.Email == email);
            var creator = context.Creators.FirstOrDefault(c => c.UserId == user.Id);
            
            if (creator == null) return Forbid();

            var newWork = new Work
            {
                CreatorId = creator.Id,
                Url = work.Url
            };

            context.Works.Add(newWork);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetWork", new { id = newWork.Id }, newWork);
        }

        // DELETE: api/Works/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}"), Authorize(Roles = "creator")]
        public async Task<IActionResult> DeleteWork(int id)
        {
            var email = User.FindFirst(c => c.Type == ClaimsIdentity.DefaultNameClaimType)!.Value;
            var user = context.Users.First(u => u.Email == email);
            var creator = context.Creators.FirstOrDefault(c => c.UserId == user.Id);
            
            if (creator == null) return Forbid();
            
            var work = await context.Works.FindAsync(id);
            if (work == null)
            {
                return NotFound();
            }

            if (work.CreatorId != creator.Id) return BadRequest();

            context.Works.Remove(work);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkExists(int id)
        {
            return context.Works.Any(e => e.Id == id);
        }
    }
}