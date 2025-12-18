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
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EventsController(AppDbContext context) => _context = context;

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventReadDto>>> GetAll()
        {
            var events = await _context.Events
                .AsNoTracking()
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .Include(e => e.Registrations)
                .OrderBy(e => e.StartTime)
                .Select(e => new EventReadDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Capacity = e.Capacity,
                    VenueId = e.VenueId,
                    VenueName = e.Venue.Name,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.Name,
                    RegistrationCount = e.Registrations.Count
                })
                .ToListAsync();

            return Ok(events);
        }

     
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EventReadDto>> GetOne(int id)
        {
            var e = await _context.Events
                .AsNoTracking()
                .Include(x => x.Venue)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (e == null) return NotFound();

            return Ok(new EventReadDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Capacity = e.Capacity,
                VenueId = e.VenueId,
                VenueName = e.Venue.Name,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name
            });
        }

      
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<EventReadDto>> Create(EventCreateDto dto)
        {
       
            var venueExists = await _context.Venues.AnyAsync(v => v.Id == dto.VenueId);
            if (!venueExists) return BadRequest("Venue not found");

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists) return BadRequest("Category not found");

            if (dto.EndTime <= dto.StartTime)
                return BadRequest("EndTime must be after StartTime");

            if (dto.Capacity <= 0)
                return BadRequest("Capacity must be at least 1");

            var ev = new Event
            {
                Title = dto.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Capacity = dto.Capacity,
                VenueId = dto.VenueId,
                CategoryId = dto.CategoryId
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

         
            var saved = await _context.Events
                .AsNoTracking()
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .FirstAsync(e => e.Id == ev.Id);

            return Ok(new EventReadDto
            {
                Id = saved.Id,
                Title = saved.Title,
                Description = saved.Description,
                StartTime = saved.StartTime,
                EndTime = saved.EndTime,
                Capacity = saved.Capacity,
                VenueId = saved.VenueId,
                VenueName = saved.Venue.Name,
                CategoryId = saved.CategoryId,
                CategoryName = saved.Category.Name
            });
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, EventUpdateDto dto)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

   
            var venueExists = await _context.Venues.AnyAsync(v => v.Id == dto.VenueId);
            if (!venueExists) return BadRequest("Venue not found");

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists) return BadRequest("Category not found");

            if (dto.EndTime <= dto.StartTime)
                return BadRequest("EndTime must be after StartTime");

            if (dto.Capacity <= 0)
                return BadRequest("Capacity must be at least 1");

            ev.Title = dto.Title.Trim();
            ev.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            ev.StartTime = dto.StartTime;
            ev.EndTime = dto.EndTime;
            ev.Capacity = dto.Capacity;
            ev.VenueId = dto.VenueId;
            ev.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
