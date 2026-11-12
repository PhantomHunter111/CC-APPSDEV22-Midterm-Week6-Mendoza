namespace StudentRosterApi.Models;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // Kept from Week 5 for backward compatibility.
    public string Course { get; set; } = string.Empty;
    public int YearLevel { get; set; }
    public string Email { get; set; } = string.Empty;

    // Nullable so existing Week 5 students can remain without a department.
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    // Many-to-many relationship with Course.
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
