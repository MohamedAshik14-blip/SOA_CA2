namespace CollegeEventsApi.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Capacity { get; set; }
        public int RegistrationCount { get; set; }


        public int VenueId { get; set; }
        public Venue Venue { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }
}
