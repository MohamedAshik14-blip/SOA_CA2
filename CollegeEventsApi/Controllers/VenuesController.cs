using CollegeEventsApi.Data;
using CollegeEventsApi.Dtos;
using CollegeEventsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeEventsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VenuesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public VenuesController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VenueReadDto>>> GetAll()
            => Ok(await _context.Venues
                .OrderBy(v => v.Name)
                .Select(v => new VenueReadDto { Id = v.Id, Name = v.Name, Address = v.Address })
                .ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VenueReadDto>> GetOne(int id)
        {
            var v = await _context.Venues.FindAsync(id);
            if (v == null) return NotFound();
            return Ok(new VenueReadDto { Id = v.Id, Name = v.Name, Address = v.Address });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<VenueReadDto>> Create(VenueCreateDto dto)
        {
            var v = new Venue { Name = dto.Name.Trim(), Address = dto.Address?.Trim() };
            _context.Venues.Add(v);
            await _context.SaveChangesAsync();

            return Ok(new VenueReadDto { Id = v.Id, Name = v.Name, Address = v.Address });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, VenueUpdateDto dto)
        {
            var v = await _context.Venues.FindAsync(id);
            if (v == null) return NotFound();

            v.Name = dto.Name.Trim();
            v.Address = dto.Address?.Trim();
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var v = await _context.Venues
                .Include(x => x.Events)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (v == null) return NotFound();
            if (v.Events.Any()) return BadRequest("Cannot delete venue with events.");

            _context.Venues.Remove(v);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
