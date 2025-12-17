using CollegeEventsApi.Data;
using CollegeEventsApi.Dtos;
using CollegeEventsApi.Models;
using CollegeEventsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeEventsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _auth;

        public AuthController(AppDbContext context, IAuthService auth)
        {
            _context = context;
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(StudentRegisterDto dto)
        {
            dto.StudentNumber = dto.StudentNumber.Trim();

            if (await _context.Students.AnyAsync(s => s.StudentNumber == dto.StudentNumber))
                return BadRequest("Student number is already registered");

            _auth.CreatePasswordHash(dto.Password, out var hash, out var salt);

            var student = new Student
            {
                StudentNumber = dto.StudentNumber,
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                Role = dto.StudentNumber.Equals("ADMIN001", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Student",
                PasswordHash = hash,
                PasswordSalt = salt
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registered" });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(StudentLoginDto dto)
        {
            dto.StudentNumber = dto.StudentNumber.Trim();

            var student = await _context.Students.SingleOrDefaultAsync(s => s.StudentNumber == dto.StudentNumber);
            if (student == null) return Unauthorized("Invalid credentials");

            if (!_auth.VerifyPassword(dto.Password, student.PasswordHash, student.PasswordSalt))
                return Unauthorized("Invalid credentials");

            var token = _auth.CreateToken(student);
            var role = _auth.GetRole(student);

            return Ok(new
            {
                token,
                role,
                studentNumber = student.StudentNumber,
                fullName = student.FullName
            });
        }
    }
}
