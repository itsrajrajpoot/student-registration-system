using System.ComponentModel.DataAnnotations;

namespace StudentRegistrationSystem.ViewModels;

public class CreateUserViewModel
{
    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}