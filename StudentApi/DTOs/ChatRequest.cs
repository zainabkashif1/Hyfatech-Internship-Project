using System.ComponentModel.DataAnnotations;

namespace StudentApi.DTOs;

public sealed class ChatRequest
{
    [Required]
    [StringLength(4000, MinimumLength = 1)]
    public string Message { get; set; } = string.Empty;
}
