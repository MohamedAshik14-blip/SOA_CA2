namespace CollegeEventsApi.Dtos
{
    public class RegistrationReadDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = null!;
        public DateTime RegisteredAt { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
    }

    public class RegistrationCreateDto
    {
        public int EventId { get; set; }
    }
}
