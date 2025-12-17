namespace CollegeEventsBlazor.Models;

public class RegistrationCreateDto
{
    public int EventId { get; set; }
}

public class RegistrationReadDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string EventTitle { get; set; } = "";

    public int StudentId { get; set; }     
    public string StudentNumber { get; set; } = "";
    public string StudentName { get; set; } = "";

    public DateTime RegisteredAt { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
}
