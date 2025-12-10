namespace CollegeEventsApi.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string StudentNumber { get; set; } = null!; // e.g. Dk123456
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;

    
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }
}
