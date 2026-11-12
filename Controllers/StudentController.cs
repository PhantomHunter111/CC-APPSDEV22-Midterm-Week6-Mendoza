using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentRosterApi.DTOs;
using StudentRosterApi.Services;

namespace StudentRosterApi.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllStudents()
        => Ok(await _studentService.GetAllStudentsAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetStudentById(int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        return student == null
            ? NotFound(new { message = $"Student with ID {id} not found." })
            : Ok(student);
    }

    [HttpGet("course/{courseName}")]
    public async Task<IActionResult> GetStudentsByCourse(string courseName)
        => Ok(await _studentService.GetStudentsByCourseAsync(courseName));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStudent([FromBody] StudentCreateDto dto)
    {
        var created = await _studentService.CreateStudentAsync(dto);
        return CreatedAtAction(nameof(GetStudentById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentCreateDto dto)
    {
        var updated = await _studentService.UpdateStudentAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var deleted = await _studentService.DeleteStudentAsync(id);
        return deleted
            ? NoContent()
            : NotFound(new { message = $"Student with ID {id} not found." });
    }
}
