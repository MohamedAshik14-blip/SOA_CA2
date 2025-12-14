namespace CollegeEventsApi.Dtos
{
    public class EventReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Capacity { get; set; }

    
        public int VenueId { get; set; }
        public string VenueName { get; set; } = "";

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public int RegistrationCount { get; set; }
    }

    public class EventCreateDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Capacity { get; set; }

    
        public int VenueId { get; set; }
        public int CategoryId { get; set; }
    }

    public class EventUpdateDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Capacity { get; set; }

  
        public int VenueId { get; set; }
        public int CategoryId { get; set; }
    }
}
