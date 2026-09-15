using System.ComponentModel.DataAnnotations;

namespace StudentApi.DTOs;

public class UpdateStudentDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Range(1, 120)]
    public int Age { get; set; }
}
