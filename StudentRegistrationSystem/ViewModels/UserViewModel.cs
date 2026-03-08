using System.ComponentModel.DataAnnotations;

public class CreateUserViewModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public List<string> SelectedRoles { get; set; } = new();
}