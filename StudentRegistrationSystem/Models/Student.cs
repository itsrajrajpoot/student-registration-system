using System.ComponentModel.DataAnnotations;

namespace StudentRegistrationSystem.Models;

public class Student
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string RollNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Course { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    public string? ProfilePhotoPath { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}