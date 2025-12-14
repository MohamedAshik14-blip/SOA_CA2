namespace CollegeEventsApi.Dtos
{
    public class VenueReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
    }

    public class VenueCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
    }

    public class VenueUpdateDto
    {
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
    }
}
