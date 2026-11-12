namespace StudentRosterApi.DTOs;

public class StudentDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public int YearLevel { get; set; }
    public string DepartmentName { get; set; } = "Unassigned";
    public List<string> CourseTitles { get; set; } = new();

    // Email is intentionally excluded from the response DTO.
}
