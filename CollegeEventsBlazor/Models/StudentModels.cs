namespace CollegeEventsBlazor.Models;

public class StudentReadDto
{
    public int Id { get; set; }
    public string StudentNumber { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
}

public class StudentAdminUpdateDto
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "Student";
}

public class StudentAdminPasswordDto
{
    public string NewPassword { get; set; } = "";
}
