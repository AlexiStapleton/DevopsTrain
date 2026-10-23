using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class RegisterRequest
{
    [Required, MaxLength(50)]
    public required string LastName { get; set; }
    [Required, MaxLength(50)]
    public required string FirstName { get; set; }
    [Required, EmailAddress, MaxLength(254)]
    public required string Email { get; set; }
    [Required, MinLength(12), MaxLength(128)]
    public required string Password { get; set; }
    [Required, Compare(nameof(Password))] 
    public required string PasswordConfirm { get; set; }
}