using CollegeEventsApi.Data;
using CollegeEventsApi.Dtos;
using CollegeEventsApi.Models;
using CollegeEventsApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeEventsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _auth;

        public StudentsController(AppDbContext context, IAuthService auth)
        {
            _context = context;
            _auth = auth;
        }

  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentReadDto>>> GetAll()
        {
            var students = await _context.Students
                .AsNoTracking()
                .OrderBy(s => s.StudentNumber)
                .Select(s => new StudentReadDto
                {
                    Id = s.Id,
                    StudentNumber = s.StudentNumber,
                    FullName = s.FullName,
                    Email = s.Email,
                    Role = s.Role
                })
                .ToListAsync();

            return Ok(students);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<StudentReadDto>> GetOne(int id)
        {
            var s = await _context.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();

            return Ok(new StudentReadDto
            {
                Id = s.Id,
                StudentNumber = s.StudentNumber,
                FullName = s.FullName,
                Email = s.Email,
                Role = s.Role
            });
        }

    
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, StudentAdminUpdateDto dto)
        {
            var s = await _context.Students.FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();

            var fullName = dto.FullName.Trim();
            var email = dto.Email.Trim();
            var role = (dto.Role ?? "Student").Trim();

            if (string.IsNullOrWhiteSpace(fullName)) return BadRequest("FullName is required");
            if (string.IsNullOrWhiteSpace(email)) return BadRequest("Email is required");
            if (role != "Admin" && role != "Student") return BadRequest("Role must be 'Admin' or 'Student'");

         
            if (s.Role == "Admin" && role != "Admin")
            {
                var adminCount = await _context.Students.CountAsync(x => x.Role == "Admin");
                if (adminCount <= 1) return BadRequest("Cannot remove the last admin.");
            }

           
            var emailUsed = await _context.Students.AnyAsync(x => x.Id != id && x.Email == email);
            if (emailUsed) return BadRequest("Email already used by another student.");

            s.FullName = fullName;
            s.Email = email;
            s.Role = role;

            await _context.SaveChangesAsync();
            return NoContent();
        }

      
        [HttpPut("{id:int}/password")]
        public async Task<IActionResult> ResetPassword(int id, StudentAdminPasswordDto dto)
        {
            var s = await _context.Students.FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();

            var newPw = dto.NewPassword?.Trim();
            if (string.IsNullOrWhiteSpace(newPw)) return BadRequest("NewPassword is required");
            if (newPw.Length < 6) return BadRequest("Password must be at least 6 characters");

            _auth.CreatePasswordHash(newPw, out var hash, out var salt);
            s.PasswordHash = hash;
            s.PasswordSalt = salt;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _context.Students
                .Include(x => x.Registrations) // assumes Student has Registrations nav property
                .FirstOrDefaultAsync(x => x.Id == id);

            if (s == null) return NotFound();

        
            if (s.Role == "Admin")
            {
                var adminCount = await _context.Students.CountAsync(x => x.Role == "Admin");
                if (adminCount <= 1) return BadRequest("Cannot delete the last admin.");
            }

         

            _context.Students.Remove(s);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
