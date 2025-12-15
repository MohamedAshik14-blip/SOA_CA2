using CollegeEventsApi.Data;
using CollegeEventsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeEventsApi.Repositories
{
    public class RegistrationRepository : Repository<Registration>, IRegistrationRepository
    {
        private readonly AppDbContext _context;

        public RegistrationRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> CountRegistrationsForEventAsync(int eventId)
        {
            return await _context.Registrations
                .CountAsync(r => r.EventId == eventId);
        }

        public async Task<Registration?> GetByStudentAndEventAsync(int studentId, int eventId)
        {
            return await _context.Registrations
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.StudentId == studentId && r.EventId == eventId);
        }

        public async Task<IEnumerable<Registration>> GetForStudentAsync(int studentId)
        {
            return await _context.Registrations
                .Include(r => r.Event)
                .Where(r => r.StudentId == studentId)
                .ToListAsync();
        }
    }
}
