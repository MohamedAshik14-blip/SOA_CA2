namespace CollegeEventsApi.Dtos
{
    public class StudentRegisterDto
    {
        public string StudentNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class StudentLoginDto
    {
        public string StudentNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class StudentReadDto
    {
        public int Id { get; set; }
        public string StudentNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
      public class StudentAdminUpdateDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;

  
        public string Role { get; set; } = "Student";
    }


    public class StudentAdminPasswordDto
    {
        public string NewPassword { get; set; } = null!;
    }
}
