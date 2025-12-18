namespace CollegeEventsBlazor.Models;

public class LoginRequest
{
    public string StudentNumber { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginResponse
{
    public string Token { get; set; } = "";
    public string Role { get; set; } = "Student";
    public string StudentNumber { get; set; } = "";
    public string FullName { get; set; } = "";
}

public class RegisterRequest
{
    public string StudentNumber { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
