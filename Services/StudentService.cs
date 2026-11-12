using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentRosterApi.Data;
using StudentRosterApi.DTOs;
using StudentRosterApi.Models;

namespace StudentRosterApi.Services;

public class StudentService : IStudentService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public StudentService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    private IQueryable<Student> StudentsWithRelations() =>
        _context.Students
            .Include(student => student.Department)
            .Include(student => student.Courses);

    public async Task<List<StudentDto>> GetAllStudentsAsync()
    {
        var students = await StudentsWithRelations().AsNoTracking().ToListAsync();
        return _mapper.Map<List<StudentDto>>(students);
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id)
    {
        var student = await StudentsWithRelations()
            .AsNoTracking()
            .FirstOrDefaultAsync(student => student.Id == id);

        return student == null ? null : _mapper.Map<StudentDto>(student);
    }

    public async Task<List<StudentDto>> GetStudentsByCourseAsync(string courseName)
    {
        var students = await StudentsWithRelations()
            .AsNoTracking()
            .Where(student => student.Course.ToLower() == courseName.ToLower())
            .ToListAsync();

        return _mapper.Map<List<StudentDto>>(students);
    }

    public async Task<StudentDto> CreateStudentAsync(StudentCreateDto dto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(department => department.Id == dto.DepartmentId);

            if (department == null)
                throw new KeyNotFoundException($"Department ID {dto.DepartmentId} does not exist.");

            var student = _mapper.Map<Student>(dto);
            student.DepartmentId = department.Id;

            if (dto.CourseIds.Count > 0)
            {
                var requestedIds = dto.CourseIds.Distinct().ToList();
                var courses = await _context.Courses
                    .Where(course => requestedIds.Contains(course.Id))
                    .ToListAsync();

                if (courses.Count != requestedIds.Count)
                    throw new KeyNotFoundException("One or more CourseIds do not exist.");

                student.Courses = courses;
            }

            _context.Students.Add(student);
            _context.AuditLogs.Add(new AuditLog
            {
                Action = $"Created student: {student.FirstName} {student.LastName}"
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var saved = await StudentsWithRelations()
                .FirstAsync(item => item.Id == student.Id);

            return _mapper.Map<StudentDto>(saved);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateStudentAsync(int id, StudentCreateDto dto)
    {
        var student = await _context.Students
            .Include(item => item.Courses)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (student == null) return false;

        var departmentExists = await _context.Departments
            .AnyAsync(department => department.Id == dto.DepartmentId);

        if (!departmentExists)
            throw new KeyNotFoundException($"Department ID {dto.DepartmentId} does not exist.");

        var requestedIds = dto.CourseIds.Distinct().ToList();
        var courses = await _context.Courses
            .Where(course => requestedIds.Contains(course.Id))
            .ToListAsync();

        if (courses.Count != requestedIds.Count)
            throw new KeyNotFoundException("One or more CourseIds do not exist.");

        student.FirstName = dto.FirstName;
        student.LastName = dto.LastName;
        student.Course = dto.Course;
        student.YearLevel = dto.YearLevel;
        student.Email = dto.Email;
        student.DepartmentId = dto.DepartmentId;
        student.Courses = courses;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var student = await _context.Students
                .Include(item => item.Courses)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (student == null)
            {
                await transaction.RollbackAsync();
                return false;
            }

            var name = $"{student.FirstName} {student.LastName}";
            student.Courses.Clear();
            _context.Students.Remove(student);

            _context.AuditLogs.Add(new AuditLog
            {
                Action = $"Deleted student: {name}"
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
