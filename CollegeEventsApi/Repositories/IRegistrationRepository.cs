using CollegeEventsApi.Models;

namespace CollegeEventsApi.Repositories
{
    public interface IRegistrationRepository : IRepository<Registration>
    {
        Task<int> CountRegistrationsForEventAsync(int eventId);
        Task<Registration?> GetByStudentAndEventAsync(int studentId, int eventId);
        Task<IEnumerable<Registration>> GetForStudentAsync(int studentId);
    }
}
