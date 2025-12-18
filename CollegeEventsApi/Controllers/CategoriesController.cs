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
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoriesController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetAll()
            => Ok(await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryReadDto { Id = c.Id, Name = c.Name })
                .ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryReadDto>> GetOne(int id)
        {
            var c = await _context.Categories.FindAsync(id);
            if (c == null) return NotFound();
            return Ok(new CategoryReadDto { Id = c.Id, Name = c.Name });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CategoryReadDto>> Create(CategoryCreateDto dto)
        {
            var c = new Category { Name = dto.Name.Trim() };
            _context.Categories.Add(c);
            await _context.SaveChangesAsync();

            return Ok(new CategoryReadDto { Id = c.Id, Name = c.Name });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
        {
            var c = await _context.Categories.FindAsync(id);
            if (c == null) return NotFound();

            c.Name = dto.Name.Trim();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _context.Categories
                .Include(x => x.Events)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (c == null) return NotFound();
            if (c.Events.Any()) return BadRequest("Cannot delete category with events.");

            _context.Categories.Remove(c);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
