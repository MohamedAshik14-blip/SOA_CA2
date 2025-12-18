namespace CollegeEventsBlazor.Models;

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
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
