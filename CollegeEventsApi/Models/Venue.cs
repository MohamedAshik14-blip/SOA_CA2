namespace CollegeEventsApi.Models
{
    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
