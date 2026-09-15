using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
    {
        var students = await _context.Students
            .AsNoTracking()
            .Select(student => new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Age = student.Age
            })
            .ToListAsync();

        return Ok(students);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentDto>> GetStudent(int id)
    {
        var student = await _context.Students
            .AsNoTracking()
            .Where(student => student.Id == id)
            .Select(student => new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Age = student.Age
            })
            .FirstOrDefaultAsync();

        if (student is null)
        {
            return NotFound();
        }

        return Ok(student);
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> CreateStudent(CreateStudentDto dto)
    {
        var student = new Student
        {
            Name = dto.Name,
            Email = dto.Email,
            Age = dto.Age
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var result = new StudentDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            Age = student.Age
        };

        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateStudent(int id, UpdateStudentDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("The route ID and body ID must match.");
        }

        var student = await _context.Students.FindAsync(id);

        if (student is null)
        {
            return NotFound();
        }

        student.Name = dto.Name;
        student.Email = dto.Email;
        student.Age = dto.Age;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student is null)
        {
            return NotFound();
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
