using System.ComponentModel.DataAnnotations;

namespace StudentRosterApi.DTOs;

public class StudentCreateDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(100)]
    public string Course { get; set; } = string.Empty;

    [Range(1, 6)]
    public int YearLevel { get; set; }

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "A valid DepartmentId is required.")]
    public int DepartmentId { get; set; }

    // IDs of existing Course records to associate with this student.
    public List<int> CourseIds { get; set; } = new();
}
