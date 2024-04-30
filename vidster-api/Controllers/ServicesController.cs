using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
    public class ServicesController(VidsterContext context) : ControllerBase
    {
        // GET: api/Service
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            return await context.Services.ToListAsync();
        }
        
        // POST: api/Service
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost, Authorize(Roles = "creator")]
        public async Task<IActionResult> PostServicesOfCreator(ServicesViewModel services)
        {
            var email = User.FindFirst(c => c.Type == ClaimsIdentity.DefaultNameClaimType)!.Value;
            var user = context.Users.First(u => u.Email == email);
            var creator = context.Creators.FirstOrDefault(c => c.UserId == user.Id);

            if (creator == null) return Forbid();

            var oldServices = await context.ServicesOfCreator.Where(s => s.CreatorId == creator.Id).ToListAsync();

            context.ServicesOfCreator.RemoveRange(oldServices);
            await context.SaveChangesAsync();

            foreach (var serviceViewModel in services.Services)
            {
                var service = await context.Services.FirstOrDefaultAsync(s => s.Name == serviceViewModel.Name);

                if (service == null) continue;

                var newServiceOfCreator = new ServiceOfCreator
                {
                    CreatorId = creator.Id,
                    ServiceId = service.Id,
                    Price = serviceViewModel.Price
                };
                context.ServicesOfCreator.Add(newServiceOfCreator);
            }

            await context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Service/5
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteServiceOfCreator(int id)
        {
            var serviceOfCreator = await context.ServicesOfCreator.FindAsync(id);
            if (serviceOfCreator == null)
            {
                return NotFound();
            }

            context.ServicesOfCreator.Remove(serviceOfCreator);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}