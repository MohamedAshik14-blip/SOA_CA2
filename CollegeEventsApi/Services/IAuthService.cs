using CollegeEventsApi.Models;

namespace CollegeEventsApi.Services
{
    public interface IAuthService
    {
        void CreatePasswordHash(string password, out byte[] hash, out byte[] salt);
        bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt);

        string GetRole(Student student);
        string CreateToken(Student student);
    }
}
