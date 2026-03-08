using System.ComponentModel.DataAnnotations;
namespace StudentRegistrationSystem.DTOs;

public class StudentDto
{
    public Guid Id { get; set; }

    [Required]
    public string RollNumber { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Course { get; set; }

    [Required]
    public string Gender { get; set; }

    public bool IsActive { get; set; }

    public IFormFile? ProfilePhoto { get; set; }

    public string? ProfilePhotoPath { get; set; }
}