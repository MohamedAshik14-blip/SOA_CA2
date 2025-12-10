namespace CollegeEventsApi.Models
{
    public class Registration
    {
        public int Id { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

      
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
    }
}
