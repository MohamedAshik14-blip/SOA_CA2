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

     
        [Authorize]
        [HttpPost("{eventId:int}/checkin")]
        public async Task<IActionResult> CheckIn(int eventId)
        {
            var studentId = GetStudentId();

            var reg = await _context.Registrations
                .SingleOrDefaultAsync(r => r.StudentId == studentId && r.EventId == eventId);

            if (reg == null) return NotFound("Not registered");
            if (reg.CheckInTime != null) return BadRequest("Already checked in");

            reg.CheckInTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok();
        }


        [Authorize]
        [HttpPost("{eventId:int}/checkout")]
        public async Task<IActionResult> CheckOut(int eventId)
        {
            var studentId = GetStudentId();

            var reg = await _context.Registrations
                .SingleOrDefaultAsync(r => r.StudentId == studentId && r.EventId == eventId);

            if (reg == null) return NotFound("Not registered");
            if (reg.CheckInTime == null) return BadRequest("Check-in first");
            if (reg.CheckOutTime != null) return BadRequest("Already checked out");

            reg.CheckOutTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok();
        }

      
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistrationReadDto>>> All()
        {
            var regs = await _context.Registrations
                .Include(r => r.Event)
                .Include(r => r.Student)
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

     
        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/remove")]
        public async Task<IActionResult> AdminRemove([FromQuery] int studentId, [FromQuery] int eventId)
        {
            var reg = await _context.Registrations
                .SingleOrDefaultAsync(r => r.StudentId == studentId && r.EventId == eventId);

            if (reg == null) return NotFound("Registration not found");

            _context.Registrations.Remove(reg);
            await _context.SaveChangesAsync();
            return NoContent();
        }

      
        [Authorize(Roles = "Admin")]
        [HttpPost("admin/checkin")]
        public async Task<IActionResult> AdminForceCheckIn([FromQuery] int studentId, [FromQuery] int eventId)
        {
            var reg = await _context.Registrations
                .SingleOrDefaultAsync(r => r.StudentId == studentId && r.EventId == eventId);

            if (reg == null) return NotFound("Registration not found");

            reg.CheckInTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok();
        }

   
        [Authorize(Roles = "Admin")]
        [HttpPost("admin/checkout")]
        public async Task<IActionResult> AdminForceCheckOut([FromQuery] int studentId, [FromQuery] int eventId)
        {
            var reg = await _context.Registrations
                .SingleOrDefaultAsync(r => r.StudentId == studentId && r.EventId == eventId);

            if (reg == null) return NotFound("Registration not found");

            reg.CheckOutTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
