using StudentRosterApi.DTOs;

namespace StudentRosterApi.Services;

public interface IStudentService
{
    Task<List<StudentDto>> GetAllStudentsAsync();
    Task<StudentDto?> GetStudentByIdAsync(int id);
    Task<List<StudentDto>> GetStudentsByCourseAsync(string courseName);
    Task<StudentDto> CreateStudentAsync(StudentCreateDto dto);
    Task<bool> UpdateStudentAsync(int id, StudentCreateDto dto);
    Task<bool> DeleteStudentAsync(int id);
}
