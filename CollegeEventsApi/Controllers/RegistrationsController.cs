using System.Security.Claims;
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
    public class RegistrationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RegistrationsController(AppDbContext context) => _context = context;

        private int GetStudentId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idStr!);
        }

     


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Register([FromBody] RegistrationCreateDto dto)
        {
            var studentId = GetStudentId();

            var ev = await _context.Events.FindAsync(dto.EventId);
            if (ev == null) return NotFound("Event not found");

            var count = await _context.Registrations.CountAsync(r => r.EventId == dto.EventId);
            if (count >= ev.Capacity) return BadRequest("Event is full");

            var exists = await _context.Registrations.AnyAsync(r => r.StudentId == studentId && r.EventId == dto.EventId);
            if (exists) return BadRequest("Already registered");

            _context.Registrations.Add(new Registration
            {
                StudentId = studentId,
                EventId = dto.EventId
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = "Registered" });
        }

      
        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<RegistrationReadDto>>> My()
        {
            var studentId = GetStudentId();

            var regs = await _context.Registrations
                .Include(r => r.Event)
                .Include(r => r.Student)
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.RegisteredAt)
                .Select(r => new RegistrationReadDto
                {
                    Id = r.Id,
                    EventId = r.EventId,
                    EventTitle = r.Event.Title,
                    StudentId = r.StudentId,
                    StudentNumber = r.Student.StudentNumber,
                    StudentName = r.Student.FullName,
                    RegisteredAt = r.RegisteredAt,
                    CheckInTime = r.CheckInTime,
                    CheckOutTime = r.CheckOutTime
                })
                .ToListAsync();

            return Ok(regs);
        }

      
        [Authorize]
        [HttpDelete("{eventId:int}")]
        public async Task<IActionResult> Withdraw(int eventId)
        {
            var studentId = GetStudentId();

            var reg = await _context.Registrations
                .SingleOrDefaultAsync(r => r.StudentId == studentId && r.EventId == eventId);

            if (reg == null) return NotFound("Not registered");

            _context.Registrations.Remove(reg);
            await _context.SaveChangesAsync();
            return NoContent();
        }
